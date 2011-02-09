using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TD
{
    class Tooltip : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private ITooltipProvider provider;

        public string Text { get; set; }
        public Color TextColor { get; set; }
        public SpriteFont Font { get; set; }

        Texture2D temp;

        public Tooltip(Game game, ITooltipProvider provider, string text)
            : base(game)
        {
            spriteBatch = GameHelper.GetService<SpriteBatch>();
            Font = TheGame.Fonts["Calibri 8"];

            this.provider = provider;
            Text = text;
            TextColor = Color.White;
            Enabled = false;
            Visible = false;

            provider.ShowTooltip += (o, e) => Visible = true;
            provider.HideTooltip += (o, e) => Visible = false;

            temp = new Texture2D(game.GraphicsDevice, 1, 1);
            temp.SetData<Color>(new[] { Color.FromNonPremultiplied(0, 0, 0, 230) });

            game.Components.Add(this);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 textSize = Font.MeasureString(Text);
            Vector2 position = new Vector2(Mouse.GetState().X - 10, Mouse.GetState().Y - (textSize.Y + 20));

            spriteBatch.Begin();
            spriteBatch.Draw(temp, new Rectangle((int)position.X, (int)position.Y, 
                (int)textSize.X + 20, (int)textSize.Y + 10), Color.White);
            spriteBatch.DrawString(Font, Text, position + new Vector2(10, 5), TextColor);            
            spriteBatch.End();

            XNATools.Draw.Rect(position, 
                new Vector2(textSize.X + 19, textSize.Y + 9), Color.FromNonPremultiplied(40, 40, 40, 255));
            XNATools.Draw.Rect(position + Vector2.One,
                new Vector2(textSize.X + 17, textSize.Y + 7), Color.FromNonPremultiplied(40, 40, 40, 255));

            base.Draw(gameTime);
        }
    }
}
