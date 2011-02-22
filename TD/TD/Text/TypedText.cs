using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace TD
{
    class TypedText : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;

        private Vector2 position;
        private string text;
        private SpriteFont font;
        private Color color;
        private StringBuilder builder;
        private int interval;
        private int elapsed;
        private int currentChar;

        public bool DropShadow { get; set; }

        public TypedText(Game game, Vector2 position, string text, SpriteFont font, Color color, int interval)
            : base(game)
        {
            spriteBatch = GameHelper.GetService<SpriteBatch>();

            this.position = position;
            this.text = text;
            this.font = font;
            this.color = color;
            this.interval = interval;

            builder = new StringBuilder();
        }

        public override void Update(GameTime gameTime)
        {
            if (currentChar < text.Length)
            {
                elapsed += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsed > interval)
                {
                    elapsed -= interval;

                    builder.Append(text[currentChar]);
                    currentChar++;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (DropShadow)
            {
                spriteBatch.DrawString(font, builder, position + Vector2.One, Color.Black);
            }
            spriteBatch.DrawString(font, builder, position, color);            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
