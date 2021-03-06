﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace XNATools.UI
{
    public enum Direction { Left, Right, Up, Down }

    public class ProgressBar : UIControl
    {
        private Rectangle foregroundBounds;

        private int percentage;
        public int Percentage
        {
            get { return percentage; }
            set
            {
                percentage = (int)MathHelper.Clamp(value, 0, 100);
            }
        }

        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public int ForegroundInset { get; set; }
        public Direction BarDirection { get; set; }

        public ProgressBar(Game game, Rectangle bounds)
            : this(game, bounds, Color.Black, Color.White)
        {
        }

        public ProgressBar(Game game, Rectangle bounds, Color background, Color foreground)
            : base(game, new Vector2(bounds.X, bounds.Y))
        {
            Bounds = bounds;
            BackgroundColor = background;
            ForegroundColor = foreground;
            ForegroundInset = 1;
            BarDirection = Direction.Right;
        }

        public override void Draw(GameTime gameTime)
        {
            XNATools.Draw.FilledRect(Bounds, BackgroundColor);
            CalculateForeground();
            XNATools.Draw.FilledRect(foregroundBounds, ForegroundColor);

            base.Draw(gameTime);
        }

        private void CalculateForeground()
        {
            switch (BarDirection)
            {
                case Direction.Right:
                    foregroundBounds = new Rectangle(Bounds.X + ForegroundInset, Bounds.Y + ForegroundInset,
                        (int)((Bounds.Width - ForegroundInset * 2) * Percentage / 100.0f), Bounds.Height - ForegroundInset * 2);
                    break;

                case Direction.Left:
                    int barWidth = (int)((Bounds.Width - ForegroundInset * 2) * Percentage / 100.0f);
                    foregroundBounds = new Rectangle(Bounds.X + ForegroundInset + ((Bounds.Width - ForegroundInset * 2) - barWidth), Bounds.Y + ForegroundInset,
                        barWidth, Bounds.Height - ForegroundInset * 2);
                    break;

                case Direction.Down:
                    foregroundBounds = new Rectangle(Bounds.X + ForegroundInset, Bounds.Y + ForegroundInset,
                        Bounds.Width - ForegroundInset * 2, (int)((Bounds.Height - ForegroundInset * 2) * Percentage / 100.0f));
                    break;

                case Direction.Up:
                    int barHeight = (int)((Bounds.Height - ForegroundInset * 2) * Percentage / 100.0f);
                    foregroundBounds = new Rectangle(Bounds.X + ForegroundInset, Bounds.Y + ForegroundInset + ((Bounds.Height - ForegroundInset * 2) - barHeight),
                        Bounds.Width - ForegroundInset * 2, barHeight);
                    break;
            }
        }
    }
}
