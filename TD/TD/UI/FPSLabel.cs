using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    public class FPSLabel : Label
    {
        TimeSpan elapsed = TimeSpan.Zero;
        int frames;

        public FPSLabel(Game game, Vector2 position, SpriteFont font)
            : base(game, position, string.Empty, font)
        {
        }

        public override void Update(GameTime gameTime)
        {
            elapsed += gameTime.ElapsedGameTime;

            if (elapsed > TimeSpan.FromSeconds(1))
            {
                elapsed -= TimeSpan.FromSeconds(1);
                Text = "FPS: " + frames;
                frames = 0;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            frames++;
            
            base.Draw(gameTime);
        }
    }
}
