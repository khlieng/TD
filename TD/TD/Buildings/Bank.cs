using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace TD
{
    class Bank : Building
    {
        private int amount;
        private int interval;
        private int elapsed;

        public Bank(Game game, int amount, int interval)
            : base(game)
        {
            this.amount = amount;
            this.interval = interval;

            Cost = 400;
        }

        protected override void LoadContent()
        {
            Texture = Game.Content.Load<Texture2D>("bank");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            elapsed += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsed >= interval)
            {
                elapsed -= interval;
                Player.AddMoney(amount);
                new MovingText(Game, "+ $" + amount, TheGame.GetFont(Font.Small), Color.Yellow, position, position - new Vector2(0, 20), 500);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Texture, position, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
