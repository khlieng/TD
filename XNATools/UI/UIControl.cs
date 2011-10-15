using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNATools;

namespace XNATools.UI
{
    public abstract class UIControl : DrawableGameComponent, ITooltipProvider
    {
        protected SpriteBatch spriteBatch;

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

        public Color Color { get; set; }
        public SpriteFont Font { get; set; }

        protected Vector2 textSize;
        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                textSize = Font.MeasureString(text);
                Bounds = new Rectangle(Bounds.X, Bounds.Y, (int)textSize.X, (int)textSize.Y);
            }
        }

        private bool hovered;
        public bool Hovered
        {
            get { return hovered; }
        }

        public bool DropShadow { get; set; }
        public Color ShadowColor { get; set; }

        public event EventHandler Click;
        public event EventHandler PositionChanged;
        public event EventHandler BoundsChanged;

        protected UIControl(Game game, Vector2 position)
            : base(game)
        {
            spriteBatch = game.GetService<SpriteBatch>();

            Position = position;
            Color = Color.White;
            ShadowColor = Color.Black;

            VisibleChanged += (o, e) =>
            {
                if (!Visible)
                {
                    OnHideTooltip();
                }
            };
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

            hovered = IsMouseOver();

            if (IsMouseOver(mouseState) && !IsMouseOver(prevMouseState))
            {
                OnShowTooltip();
            }

            if (!IsMouseOver(mouseState) && IsMouseOver(prevMouseState))
            {
                OnHideTooltip();
            }
            
            prevMouseState = mouseState;

            base.Update(gameTime);
        }

        protected virtual bool IsMouseOver(MouseState state)
        {
            return bounds.Intersects(new Rectangle(state.X, state.Y, 1, 1));
        }

        protected bool IsMouseOver()
        {
            return IsMouseOver(Mouse.GetState());
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

        #region ITooltipProvider implementation
        public event EventHandler ShowTooltip;
        public event EventHandler HideTooltip;

        protected virtual void OnShowTooltip()
        {
            if (ShowTooltip != null)
            {
                ShowTooltip(this, EventArgs.Empty);
            }
        }

        protected virtual void OnHideTooltip()
        {
            if (HideTooltip != null)
            {
                HideTooltip(this, EventArgs.Empty);
            }
        }
        #endregion
    }
}
