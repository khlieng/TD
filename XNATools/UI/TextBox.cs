using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XNATools.UI
{
    public class TextBox : UIControl
    {
        private static Input input;

        public Color BackgroundColor { get; set; }

        private float elapsedSeconds;

        public TextBox(Game game, Vector2 position, int width, SpriteFont font)
            : base(game, position)
        {
            Font = font;
            Text = "";
            Bounds = new Rectangle(Bounds.X, Bounds.Y, width, (int)font.MeasureString("A").Y);

            Input input = new Input(game);            
            input.KeyPressed += (key) =>
                {
                    if (HasFocus)
                    {
                        if (key == Keys.Space)
                        {
                            Text += " ";
                        }
                        else if (key == Keys.Back)
                        {
                            Text = Text.Remove(Text.Length - 1, 1);
                        }
                        else
                        {
                            Text += key.ToString();
                        }
                    }
                };
        }

        public override void Update(GameTime gameTime)
        {
            elapsedSeconds += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            XNATools.Draw.FilledRect(Bounds, BackgroundColor);

            spriteBatch.Begin();
            spriteBatch.DrawString(Font, Text, Position + new Vector2(2, 0), Color);
            spriteBatch.End();

            if (HasFocus && elapsedSeconds > 0.5f)
            {
                if (elapsedSeconds > 1.0f)
                {
                    elapsedSeconds -= 1.0f;
                }

                if (string.IsNullOrEmpty(Text))
                {
                    XNATools.Draw.FilledRect(Position + new Vector2(2, 2), new Vector2(2, Bounds.Height - 4), Color);
                }
                else
                {
                    XNATools.Draw.FilledRect(Position + new Vector2(3 + textSize.X, 2), new Vector2(2, Bounds.Height - 4), Color);
                }
            }

            base.Draw(gameTime);
        }
    }
}
