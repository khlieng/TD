using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace TD
{
    abstract class Building : DrawableGameComponent
    {
        protected SpriteBatch spriteBatch;

        public Texture2D Texture { get; protected set; }

        public int Row { get; private set; }
        public int Col { get; private set; }
        protected Vector2 position;
        protected Vector2 center;

        public int Cost { get; protected set; }

        public Building(Game game)
            : base(game)
        {
            Enabled = false;
            Visible = false;

            spriteBatch = game.GetService<SpriteBatch>();
            LoadContent();
        }

        public virtual void Place(int row, int col)
        {
            Row = row;
            Col = col;
            position = new Vector2(Col * 32, Row * 32);
            center = new Vector2(position.X + 16, position.Y + 16);

            Enabled = true;
            Visible = true;
        }
    }
}
