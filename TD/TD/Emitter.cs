using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    class Emitter : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private Texture2D texture;

        private Random rand = new Random();        

        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }

        public int MaxDirectionDevation { get; set; }
        public int MinVelocity { get; set; }
        public int MaxVelocity { get; set; }
        public int MinDuration { get; set; }
        public int MaxDuration { get; set; }
        public float MinScale { get; set; }
        public float MaxScale { get; set; }
        public int EmitOffset { get; set; }

        private bool emitting;
        public bool Emitting
        {
            get { return emitting; }
            set { if (!instant) emitting = value; }
        }
        public bool Additive { get; set; }

        private int interval;
        private int elapsed;

        private LinkedList<Particle> particles = new LinkedList<Particle>();
        private Queue<Particle> remove = new Queue<Particle>();
        private Queue<Particle> add = new Queue<Particle>();

        private bool instant;

        public Emitter(Game game, Texture2D texture, Vector2 position)
            : base(game)
        {
            spriteBatch = GameHelper.GetService<SpriteBatch>();
            this.texture = texture;

            Position = position;
            Direction = new Vector2(0, 1);
            Additive = true;

            MaxScale = 1.0f;
            MinScale = 1.0f;

            instant = true;

            game.Components.Add(this);
        }        
        
        public Emitter(Game game, Texture2D texture, Vector2 position, int interval)
            : base(game)
        {
            spriteBatch = GameHelper.GetService<SpriteBatch>();
            this.texture = texture;

            Position = position;
            Direction = new Vector2(0, 1);
            this.interval = interval;
            Emitting = true;
            Additive = true;

            MaxScale = 1.0f;
            MinScale = 1.0f;

            game.Components.Add(this);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public void Add(Particle particle)
        {
            add.Enqueue(particle);
        }

        public void Remove(Particle particle)
        {
            remove.Enqueue(particle);
        }

        public void Emit(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                particles.AddLast(CreateParticle());
            }
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
                while (elapsed >= interval)
                {
                    elapsed -= interval;
                    CreateParticle();
                }
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

        private Particle CreateParticle()
        {
            float deviationAngle = (rand.Next(MaxDirectionDevation * 2 + 1) - MaxDirectionDevation);
            Vector2 pDirection = Vector2.Transform(Direction, Matrix.CreateRotationZ(MathHelper.ToRadians(deviationAngle)));

            float velocity = rand.Next(MaxVelocity - MinVelocity + 1) + MinVelocity;
            int duration = rand.Next(MaxDuration - MinDuration + 1) + MinDuration;
            float scale = rand.Next((int)(MaxScale * 100) - (int)(MinScale * 100) + 1) / 100.0f + MinScale;

            return new Particle(this, texture, Position + pDirection * EmitOffset, pDirection, velocity, scale, duration);
        }
    }
}
