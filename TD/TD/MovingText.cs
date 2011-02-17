using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    class MovingText : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;

        private string text;
        private Color color;
        private Vector2 position;
        private Vector2 direction;
        private float distance;
        private float distanceMoved;
        private int time;

        public MovingText(Game game, string text, Vector2 start, Vector2 end, int time)
            : this(game, text, Color.White, start, end, time)
        {
        }

        public MovingText(Game game, string text, Color color, Vector2 start, Vector2 end, int time)
            : base(game)
        {
            spriteBatch = GameHelper.GetService<SpriteBatch>();

            this.text = text;
            this.color = color;
            this.time = time;            

            position = start;

            direction = end - start;
            distance = direction.Length();
            direction.Normalize();

            game.Components.Add(this);
        }        

        public override void Update(GameTime gameTime)
        {
            float distanceDelta = ((distance / time) * gameTime.ElapsedGameTime.Milliseconds);
            position += direction * distanceDelta;
            distanceMoved += distanceDelta;
            if (distanceMoved >= distance)
            {
                Game.Components.Remove(this);
                Dispose(true);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(TheGame.GetFont(Font.Small), text, position, color);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
