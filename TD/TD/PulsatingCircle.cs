using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace TD
{
    class PulsatingCircle : DrawableGameComponent
    {
        private Circle circle;
        private Vector2 center;
        private float radius;
        private int subdivs;
        private Color color;
        private bool filled;
        private float thickness;
        private float scale;
        private float minScale = 0.9f;
        private float maxScale = 1.1f;
        private bool rising;

        public PulsatingCircle(Game game, Vector2 center, float radius, int subdivs,
            Color color, bool filled, float thickness = 1.0f)
            : base(game)
        {
            this.center = center;
            this.radius = radius;
            this.subdivs = subdivs;
            this.color = color;
            this.filled = filled;
            this.thickness = thickness;

            circle = new Circle(center, radius, subdivs, color, filled, thickness);
            scale = 1.0f;
            rising = true;
        }

        public override void Update(GameTime gameTime)
        {
            float step = 0.4f * (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            if (rising)
            {
                scale += step;
                if (scale >= maxScale)
                    rising = false;
            }
            else
            {
                scale -= step;
                if (scale <= minScale)
                    rising = true;
            }
            circle = new Circle(center, radius * scale, subdivs, color, filled, thickness);
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            XNATools.Draw.Shape(circle);

            base.Draw(gameTime);
        }
    }
}
