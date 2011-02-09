using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TD
{
    public class TextButton : DrawableGameComponent, IToggleAble, ITooltipProvider
    {
        private SpriteBatch spriteBatch;

        public Vector2 Position { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; }
        public Color HoveredColor { get; set; }
        public bool ToggleAble { get; set; }
        public Color ToggledColor { get; set; }
        public SpriteFont Font { get; set; }
        public bool DropShadow { get; set; }
        public Color ShadowColor { get; set; }

        private Vector2 textSize;
        private bool hovered;
        private bool toggled;

        private MouseState prev;

        public event EventHandler Click;
        public event EventHandler ToggledChanged;
                
        public bool Toggled
        {
            get { return toggled; }
            set 
            {
                if (ToggleAble)
                {
                    bool changing = toggled != value;
                    toggled = value;
                    if (changing)
                    {
                        OnToggleChanged();
                    }
                }
            }
        }

        public bool Hovered
        {
            get { return hovered; }
        }

        public TextButton(Game game, Vector2 position, String text, SpriteFont font)
            : base(game)
        {
            Position = position;
            Text = text;            
            Font = font;
            Color = Color.White;
            ShadowColor = Color.Black;
            HoveredColor = Color.Gray;
            ToggledColor = Color.Red;
            textSize = font.MeasureString(text);

            spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));

            game.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            MouseState current = Mouse.GetState();

            if (MouseOver(current))
            {
                if (!toggled)
                {
                    hovered = true;
                }

                if (prev.LeftButton == ButtonState.Pressed && current.LeftButton == ButtonState.Released)
                {                    
                    Toggled = !toggled;
                    hovered = !toggled;
                    OnClick();
                }
            }
            else
            {
                hovered = false;
            }

            if (MouseOver(current) && !MouseOver(prev))
            {
                OnShowTooltip();
            }

            if (!MouseOver(current) && MouseOver(prev))
            {
                OnHideTooltip();
            }

            prev = current;

            base.Update(gameTime);
        }

        private bool MouseOver(MouseState state)
        {
            return state.X > Position.X && state.X < Position.X + textSize.X &&
                state.Y > Position.Y && state.Y < Position.Y + textSize.Y;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (DropShadow)
            {
                spriteBatch.DrawString(Font, Text, Position + Vector2.One, ShadowColor);
            }
            if (hovered)
            {
                spriteBatch.DrawString(Font, Text, Position, HoveredColor);
            }
            else if (toggled)
            {
                spriteBatch.DrawString(Font, Text, Position, ToggledColor);
            }
            else
            {
                spriteBatch.DrawString(Font, Text, Position, Color);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected virtual void OnClick()
        {
            if (Click != null)
            {
                Click(this, EventArgs.Empty);
            }
        }

        protected virtual void OnToggleChanged()
        {
            if (ToggledChanged != null)
            {
                ToggledChanged(this, EventArgs.Empty);
            }
        }

        #region ITooltipProvider implementation
        public event EventHandler ShowTooltip;
        public event EventHandler HideTooltip;

        //public Vector2 TooltipPosition
        //{
        //    get
        //    {
        //        MouseState state = Mouse.GetState();
        //        return new Vector2(state.X, state.Y - 80);
        //    }
        //}

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
