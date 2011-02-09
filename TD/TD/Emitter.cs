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
        SpriteBatch spriteBatch;
        Texture2D tempTexture;
        Random rand = new Random();
        public Vector2 Position { get; set; }
        private Vector2 direction;
        public Vector2 Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        public int MaxDirectionDevation { get; set; }
        public int MinVelocity { get; set; }
        public int MaxVelocity { get; set; }
        public int MinDuration { get; set; }
        public int MaxDuration { get; set; }

        public bool Emitting { get; set; }
        public bool Additive { get; set; }

        int interval;
        int elapsed;

        LinkedList<Particle> particles = new LinkedList<Particle>();
        Queue<Particle> remove = new Queue<Particle>();
        Queue<Particle> add = new Queue<Particle>();
        
        public Emitter(Game game, Texture2D texture, Vector2 position, int interval)
            : base(game)
        {
            spriteBatch = GameHelper.GetService<SpriteBatch>();
            tempTexture = texture;

            Position = position;
            Direction = new Vector2(0, 1);
            this.interval = interval;
            Emitting = true;
            Additive = true;

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

        public override void Update(GameTime gameTime)
        {
            foreach (Particle p in particles)
            {
                p.Update(gameTime);
            }

            if (Emitting)
            {
                while (add.Count > 0)
                    particles.AddLast(add.Dequeue());

                while (remove.Count > 0)
                    particles.Remove(remove.Dequeue());

                elapsed += gameTime.ElapsedGameTime.Milliseconds;
                while (elapsed >= interval)
                {
                    elapsed -= interval;

                    float deviationAngle = (rand.Next(MaxDirectionDevation * 2 + 1) - MaxDirectionDevation);
                    Vector2 pDirection = Vector2.Transform(direction, Matrix.CreateRotationZ(MathHelper.ToRadians(deviationAngle)));

                    float velocity = rand.Next(MaxVelocity - MinVelocity + 1) + MinVelocity;
                    int duration = rand.Next(MaxDuration - MinDuration + 1) + MinDuration;

                    new Particle(this, tempTexture, Position, pDirection, velocity, duration);
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
    }
}
