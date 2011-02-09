using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    class Tooltip : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private ITooltipProvider provider;

        public string Text { get; set; }

        public Tooltip(Game game, ITooltipProvider provider, string text)
            : base(game)
        {
            spriteBatch = GameHelper.GetService<SpriteBatch>();

            this.provider = provider;
            Text = text;
            Enabled = false;
            Visible = false;

            provider.ShowTooltip += (o, e) => Visible = true;
            provider.HideTooltip += (o, e) => Visible = false;

            game.Components.Add(this);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(TheGame.Fonts["Calibri 8"], Text, provider.TooltipPosition, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
