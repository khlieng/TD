using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace TD
{
    class TextOutput : UIControl
    {
        private string[] buffer;
        private int currentIndex;
        private int outputSize;

        public TextOutput(Game game, Vector2 position, SpriteFont font, int outputSize = 10, int bufferSize = 512)
            : base(game, position)
        {
            Font = font;
            buffer = new string[bufferSize];
            this.outputSize = outputSize;
        }

        public override void Draw(GameTime gameTime)
        {
            int line = 0;
            int start = currentIndex - outputSize;
            if (start < 0)
            {
                start = 0;
            }
            int end = start + Math.Min(outputSize, buffer.Length);
            
            spriteBatch.Begin();            
            for (int i = start; i < end; i++)
            {
                if (buffer[i] == null)
                {
                    continue;
                }

                if (DropShadow)
                {
                    spriteBatch.DrawString(Font, buffer[i], 
                        new Vector2(Position.X, Position.Y + line * Font.LineSpacing) + Vector2.One, ShadowColor);
                }
                spriteBatch.DrawString(Font, buffer[i], new Vector2(Position.X, Position.Y + line * Font.LineSpacing), Color);

                line++;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void Write<T>(T text)
        {
            if (currentIndex == buffer.Length - 1)
            {
                if (buffer[currentIndex] == null)
                {
                    WriteToBuffer(text);
                }
                else
                {
                    RewindBuffer();
                    WriteToBuffer(text);
                }
            }
            else
            {
                WriteToBuffer(text);
            }

            if (currentIndex < buffer.Length - 1)
            {
                currentIndex++;
            }
        }

        public void WriteLine<T>(T text)
        {
            Write(text + Environment.NewLine);
        }

        private void WriteToBuffer<T>(T text)
        {
            buffer[currentIndex] = text.ToString();
        }

        private void RewindBuffer()
        {
            for (int i = 0; i < buffer.Length - 1; i++)
            {
                buffer[i] = buffer[i + 1];
            }
        }
    }
}
