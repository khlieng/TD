using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNATools.UI
{
    public class CheckBox : UIControl, IToggleAble
    {
        private Rectangle foregroundBounds;

        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }

        private int foregroundInset;
        public int ForegroundInset
        {
            get { return foregroundInset; }
            set
            {
                foregroundInset = value;
                CalculateForeground();
            }
        }

        private bool toggled;
        public bool Toggled
        {
            get { return toggled; }
            set
            {
                bool changing = toggled != value;
                toggled = value;
                if (changing)
                {
                    OnToggleChanged();
                }
            }
        }

        public event EventHandler ToggledChanged;

        public CheckBox(Game game, Rectangle bounds)
            : this(game, bounds, Color.Black, Color.White)
        {
        }

        public CheckBox(Game game, Rectangle bounds, Color background, Color foreground)
            : base(game, new Vector2(bounds.X, bounds.Y))
        {
            Bounds = bounds;
            BackgroundColor = background;
            ForegroundColor = foreground;
            ForegroundInset = 2;
        }

        public override void Draw(GameTime gameTime)
        {
            XNATools.Draw.FilledRect(Bounds, BackgroundColor);

            if (toggled)
            {
                XNATools.Draw.FilledRect(foregroundBounds, ForegroundColor);
            }

            base.Draw(gameTime);
        }

        private void CalculateForeground()
        {
            foregroundBounds = new Rectangle(Bounds.X + ForegroundInset, Bounds.Y + ForegroundInset,
                Bounds.Width - ForegroundInset * 2, Bounds.Height - ForegroundInset * 2);
        }

        protected override void OnClick()
        {
            Toggled = !toggled;

            base.OnClick();
        }

        protected override void OnBoundsChanged()
        {
            CalculateForeground();

            base.OnBoundsChanged();
        }

        protected virtual void OnToggleChanged()
        {
            if (ToggledChanged != null)
            {
                ToggledChanged(null, EventArgs.Empty);
            }
        }
    }
}
