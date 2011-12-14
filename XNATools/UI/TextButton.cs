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
    public class TextButton : UIControl, IToggleAble
    {
        public Color HoveredColor { get; set; }
        public bool ToggleAble { get; set; }
        public Color ToggledColor { get; set; }
        
        private bool toggled;

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

        public TextButton(Game game, Vector2 position, String text, SpriteFont font)
            : base(game, position)
        {
            Font = font;
            Text = text;
            Bounds = new Rectangle(Bounds.X, Bounds.Y, (int)textSize.X, (int)textSize.Y);
            HoveredColor = Color.Gray;
            ToggledColor = Color.Red;
        }

        public override void Update(GameTime gameTime)
        {
            Bounds = new Rectangle(Bounds.X, Bounds.Y, (int)textSize.X, (int)textSize.Y);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (DropShadow)
            {
                spriteBatch.DrawString(Font, Text, Position + Vector2.One, ShadowColor);
            }
            if (toggled)
            {
                spriteBatch.DrawString(Font, Text, Position, ToggledColor);
            }
            else if (Hovered)
            {
                spriteBatch.DrawString(Font, Text, Position, HoveredColor);                
            }
            else
            {
                spriteBatch.DrawString(Font, Text, Position, Color);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void OnClick(MouseEventArgs args)
        {
            Toggled = !toggled;
            
            base.OnClick(args);
        }

        protected virtual void OnToggleChanged()
        {
            if (ToggledChanged != null)
            {
                ToggledChanged(this, EventArgs.Empty);
            }
        }
    }
}
