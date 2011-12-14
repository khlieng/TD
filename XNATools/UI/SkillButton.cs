using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XNATools.UI
{
    public class SkillButton : UIControl
    {
        private Texture2D texture;

        private int stacks;
        public int Stacks
        {
            get { return stacks; }
            set
            {
                int prevStacks = stacks;
                if (value < prevStacks && AllowDecrease)
                {
                    stacks = value;
                }
                else if (value > prevStacks && AllowIncrease)
                {
                    stacks = value;
                }
                Text = stacks + "/" + MaxStacks;
                if (stacks != prevStacks)
                {
                    OnStacksChanged();
                }
            }
        }
        public int MaxStacks { get; set; }
        public bool AllowIncrease { get; set; }
        public bool AllowDecrease { get; set; }

        public event EventHandler StacksChanged;

        public SkillButton(Game game, Vector2 position, Texture2D texture, SpriteFont font, int stacks, int maxStacks)
            : base(game, position)
        {
            this.texture = texture;
            Font = font;
            Stacks = stacks;
            MaxStacks = maxStacks;
            Text = stacks + "/" + maxStacks;
            Bounds = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            AllowIncrease = true;
            AllowDecrease = true;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture, Bounds, Color.White);
            if (Enabled)
            {
                spriteBatch.DrawString(Font, Text, new Vector2(Position.X + 4, Bounds.Bottom - textSize.Y), Color.Orange);
            }
            else
            {
                spriteBatch.DrawString(Font, Text, new Vector2(Position.X + 4, Bounds.Bottom - textSize.Y), Color.Gray);
            }
            spriteBatch.End();
            XNATools.Draw.Rect(Position - new Vector2(0, 0), new Vector2(Bounds.Width - 1, Bounds.Height - 1), Color.Orange);
            
            base.Draw(gameTime);
        }

        protected override void OnClick(MouseEventArgs args)
        {
            if (args.State.LeftButton == ButtonState.Pressed && Stacks < MaxStacks)
            {
                Stacks++;
            }
            else if (args.State.RightButton == ButtonState.Pressed && Stacks > 0)
            {
                Stacks--;
            }

            base.OnClick(args);
        }

        protected virtual void OnStacksChanged()
        {
            if (StacksChanged != null)
            {
                StacksChanged(this, EventArgs.Empty);
            }
        }
    }
}
