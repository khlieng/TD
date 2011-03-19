using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace TD
{
    class LineEmitter : Emitter
    {
        private Vector2 lineDirection;
        private float lineLength;

        public Color Color { get; set; }

        public LineEmitter(Game game, Vector2 lineStart, Vector2 lineEnd, params Texture2D[] textures)
            : this(game, lineStart, lineEnd, 0.0f, textures)
        {
        }

        public LineEmitter(Game game, Vector2 lineStart, Vector2 lineEnd, float interval, params Texture2D[] textures)
            : base(game, lineStart, interval, textures)
        {
            lineDirection = lineEnd - lineStart;
            lineLength = lineDirection.Length();
            lineDirection.Normalize();
            Color = Color.White;
        }

        protected override Particle CreateParticle(Microsoft.Xna.Framework.Vector2 offset)
        {
            float deviationAngle = (rand.Next(MaxDirectionDevation * 2 + 1) - MaxDirectionDevation);
            Vector2 pDirection = Vector2.Transform(Direction, Matrix.CreateRotationZ(MathHelper.ToRadians(deviationAngle)));
            
            float velocity = rand.Between(MinVelocity, MaxVelocity);
            float acceleration = rand.Between(MinAcceleration, MaxAcceleration);
            int duration = rand.Between(MinDuration, MaxDuration);
            float scale = rand.Between(MinScale, MaxScale);

            currentTexture = ++currentTexture % textures.Length;

            return new Particle(this, textures[currentTexture], Position + lineDirection * ((float)rand.NextDouble() * lineLength) + offset, 
                pDirection, Color, velocity, scale, acceleration, duration, AlphaDecayTimeFraction, ScaleDecayTimeFraction);
        }
    }
}
