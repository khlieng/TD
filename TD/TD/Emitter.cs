using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace TD
{
    class Emitter : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private Texture2D[] textures;
        private int currentTexture;

        private int interval;
        private int elapsed;
        private bool instant;

        private Random rand = new Random();

        private Vector2 position;        
        public Vector2 Position
        {
            get { return position; }
            set
            {
                positionChangeDirection = value - position;
                positionChangeAmount = positionChangeDirection.Length();
                positionChangeDirection.Normalize();
                position = value;
            }
        }
        private Vector2 positionChangeDirection;
        private float positionChangeAmount;

        public Vector2 Direction { get; set; }

        public int MaxDirectionDevation { get; set; }
        public int MinVelocity { get; set; }
        public int MaxVelocity { get; set; }
        public int MinDuration { get; set; }
        public int MaxDuration { get; set; }
        public float MinScale { get; set; }
        public float MaxScale { get; set; }
        public int EmitOffset { get; set; }
        public float DecayTimeFraction { get; set; }
        public bool Additive { get; set; }

        private bool emitting;
        public bool Emitting
        {
            get { return emitting; }
            set { if (!instant) emitting = value; }
        }

        private LinkedList<Particle> particles = new LinkedList<Particle>();
        private Queue<Particle> remove = new Queue<Particle>();
        private Queue<Particle> add = new Queue<Particle>();

        public Emitter(Game game, Vector2 position, params Texture2D[] textures)
            : this(game, textures, position, 0)
        {
        }

        public Emitter(Game game, Vector2 position, int interval, params Texture2D[] textures)
            : this(game, textures, position, interval)
        {
        }

        private Emitter(Game game, Texture2D[] textures, Vector2 position, int interval)
            : base(game)
        {
            spriteBatch = GameHelper.GetService<SpriteBatch>();
            this.textures = textures;
            currentTexture = textures.Length - 1;

            Position = position;
            positionChangeDirection = Vector2.Zero;
            positionChangeAmount = 0.0f;
            Direction = new Vector2(0, 1);

            this.interval = interval;
            instant = interval < 1;

            Additive = true;
            MaxScale = 1.0f;
            MinScale = 1.0f;
            DecayTimeFraction = 0.5f;

            game.Components.Add(this);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public void Emit(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                particles.AddLast(CreateParticle(Vector2.Zero));
            }
        }

        public void EmitFor(int ms)
        {
            emitting = true;
            new DelayedCall(Game, () => emitting = false, ms);
        }

        public void RemoveAfter(int ms)
        {
            new DelayedCall(Game, () => Game.Components.Remove(this), ms);
        }

        public void Add(Particle particle)
        {
            add.Enqueue(particle);
        }

        public void Remove(Particle particle)
        {
            remove.Enqueue(particle);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Particle p in particles)
            {
                p.Update(gameTime);
            }

            while (remove.Count > 0)
                particles.Remove(remove.Dequeue());

            if (Emitting)
            {
                while (add.Count > 0)
                    particles.AddLast(add.Dequeue());
                
                elapsed += gameTime.ElapsedGameTime.Milliseconds;
                int initialElapsed = elapsed;
                
                Vector2 previousPositon = position - positionChangeDirection * positionChangeAmount;                
                Vector2 offset = Vector2.Zero;

                int emitTime = 0;
                while (elapsed >= interval)
                {
                    emitTime = elapsed - elapsed % interval;
                    elapsed -= interval;

                    if (positionChangeAmount > 0.0f)
                    {
                        Vector2 actualPosition = previousPositon + 
                            positionChangeDirection * ((positionChangeAmount / initialElapsed) * emitTime);
                        offset = actualPosition - position;
                    }

                    Particle particle = CreateParticle(offset);
                    particle.Update(new GameTime(gameTime.TotalGameTime, TimeSpan.FromMilliseconds(emitTime)));
                    Add(particle);
                }
                positionChangeAmount = 0.0f;
            }

            base.Update(gameTime);
        }
        
        public override void Draw(GameTime gameTime)
        {
            if (Additive)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            }
            else
            {
                spriteBatch.Begin();
            }
            
            foreach (Particle p in particles)
            {
                p.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        private Particle CreateParticle(Vector2 offset)
        {
            float deviationAngle = (rand.Next(MaxDirectionDevation * 2 + 1) - MaxDirectionDevation);
            Vector2 pDirection = Vector2.Transform(Direction, Matrix.CreateRotationZ(MathHelper.ToRadians(deviationAngle)));

            float velocity = rand.Next(MaxVelocity - MinVelocity + 1) + MinVelocity;
            int duration = rand.Next(MaxDuration - MinDuration + 1) + MinDuration;
            float scale = rand.Next((int)(MaxScale * 100) - (int)(MinScale * 100) + 1) / 100.0f + MinScale;

            currentTexture = ++currentTexture % textures.Length;

            return new Particle(this, textures[currentTexture], Position + pDirection * EmitOffset + offset, 
                pDirection, velocity, scale, duration, DecayTimeFraction);
        }
    }
}
