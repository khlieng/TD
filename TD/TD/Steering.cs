using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TD
{
    class Steering
    {
        private static Random rand = new Random();

        private MovingAgent agent;
        private Vector2 steeringForce;

        public Vector2 Target { get; set; }

        public bool Seek { get; set; }

        public Steering(MovingAgent agent)
        {
            this.agent = agent;
        }

        public Vector2 Calculate()
        {
            steeringForce = Vector2.Zero;

            if (Seek)
            {
                AddSeek();
            }

            return steeringForce;
        }

        private void AddSeek()
        {
            Vector2 desiredVelocity = Target - agent.Position;
            desiredVelocity.Normalize();
            desiredVelocity *= agent.MaxSpeed;
            steeringForce += desiredVelocity - agent.Velocity;
        }
    }
}
