using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace TD
{
    class ProgressBar : UIControl
    {
        private static Texture2D temp;

        private SpriteBatch spriteBatch;
        private Rectangle foregroundBounds;        

        public int Percentage { get; set; }
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public int ForegroundInset { get; set; }

        public ProgressBar(Game game, Rectangle bounds)
            : this(game, bounds, Color.Black, Color.White)
        {
        }

        public ProgressBar(Game game, Rectangle bounds, Color background, Color foreground)
            : base(game, new Vector2(bounds.X, bounds.Y))
        {
            spriteBatch = game.GetService<SpriteBatch>();

            Bounds = bounds;
            BackgroundColor = background;
            ForegroundColor = foreground;
            ForegroundInset = 1;
        }

        protected override void LoadContent()
        {
            if (temp == null)
            {
                temp = new Texture2D(GraphicsDevice, 1, 1);
                temp.SetData<Color>(new[] { Color.White });
            }

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(temp, Bounds, BackgroundColor);
            CalculateForeground();
            spriteBatch.Draw(temp, foregroundBounds, ForegroundColor);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void CalculateForeground()
        {
            foregroundBounds = new Rectangle(Bounds.X + ForegroundInset, Bounds.Y + ForegroundInset,
                (int)((Bounds.Width - ForegroundInset * 2) * Percentage / 100.0f), Bounds.Height - ForegroundInset * 2);
        }
    }
}
