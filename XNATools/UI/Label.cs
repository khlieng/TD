using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace XNATools.UI
{
    public class Label : UIControl
    {
        public int Rotation { get; set; }

        public Label(Game game, Vector2 position, String text, SpriteFont font) 
            : base(game, position)
        {
            Font = font;
            Text = text;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (DropShadow)
            {
                spriteBatch.DrawString(Font, Text, Position + Vector2.One, ShadowColor,
                    MathHelper.ToRadians(Rotation), Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            }
            spriteBatch.DrawString(Font, Text, Position, Color, 
                MathHelper.ToRadians(Rotation), Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
