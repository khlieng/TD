using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    public class Label : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;

        public Vector2 Position { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; }
        public SpriteFont Font { get; set; }
        public bool DropShadow { get; set; }
        public Color ShadowColor { get; set; }

        public Label(Game game, Vector2 position, String text, SpriteFont font) : base(game)
        {
            Position = position;
            Text = text;
            Font = font;
            Color = Color.White;
            ShadowColor = Color.Black;
            
            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));

            game.Components.Add(this);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (DropShadow)
            {
                spriteBatch.DrawString(Font, Text, Position + Vector2.One, ShadowColor);
            }
            spriteBatch.DrawString(Font, Text, Position, Color);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
