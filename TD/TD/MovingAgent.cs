using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    class MovingAgent : DrawableGameComponent
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Direction { get; private set; }
        public float MaxSpeed { get; set; }
        public float Mass { get; set; }

        public Steering Steering { get; private set; }

        public MovingAgent(Game game, Vector2 position, float maxSpeed, float mass)
            : base(game)
        {
            Position = position;
            MaxSpeed = maxSpeed;
            Mass = mass;

            Steering = new Steering(this);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            Vector2 acceleration = Steering.Calculate() / Mass;

            Velocity += acceleration * elapsedSeconds;

            if (Velocity.LengthSquared() > MaxSpeed * MaxSpeed)
            {
                Velocity = Vector2.Normalize(Velocity);
                Velocity *= MaxSpeed;
            }

            Position += Velocity * elapsedSeconds;

            if (Velocity.LengthSquared() > 0.0f)
            {
                Direction = Vector2.Normalize(Velocity);
            }

            base.Update(gameTime);
        }
    }
}
