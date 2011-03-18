using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TD
{
    public abstract class UIControl : DrawableGameComponent
    {
        private Vector2 position;
        private Rectangle bounds;

        private MouseState prevMouseState;

        public Vector2 Position
        {
            get { return position; }
            set
            {
                bool changing = position != value;
                position = value;
                if (changing)
                {
                    OnPositionChanged();
                    bounds.X = (int)position.X;
                    bounds.Y = (int)position.Y;
                }
            }
        }

        public Rectangle Bounds
        {
            get { return bounds; }
            set
            {
                bool changing = bounds != value;
                bounds = value;
                if (changing)
                {
                    OnBoundsChanged();
                    position.X = bounds.X;
                    position.Y = bounds.Y;
                }
            }
        }

        public bool DropShadow { get; set; }
        public Color ShadowColor { get; set; }

        public event EventHandler Click;
        public event EventHandler PositionChanged;
        public event EventHandler BoundsChanged;

        protected UIControl(Game game, Vector2 position)
            : base(game)
        {
            Position = position;
            ShadowColor = Color.Black;
        }

        public override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Released &&
                prevMouseState.LeftButton == ButtonState.Pressed &&
                IsMouseOver())
            {
                OnClick();
            }

            prevMouseState = mouseState;

            base.Update(gameTime);
        }

        protected virtual bool IsMouseOver()
        {
            MouseState mouseState = Mouse.GetState();
            return bounds.Intersects(new Rectangle(mouseState.X, mouseState.Y, 1, 1));
        }

        protected virtual void OnClick()
        {
            if (Click != null)
            {
                Click(this, EventArgs.Empty);
            }
        }

        protected virtual void OnPositionChanged()
        {
            if (PositionChanged != null)
            {
                PositionChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnBoundsChanged()
        {
            if (BoundsChanged != null)
            {
                BoundsChanged(this, EventArgs.Empty);
            }
        }
    }
}
