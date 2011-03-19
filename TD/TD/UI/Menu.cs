using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    class Menu : DrawableGameComponent
    {
        private Dictionary<string, UIControl> items;
        private SpriteFont font;

        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                PositionControls();
            }
        }

        public Vector2 Size
        {
            get
            {
                Vector2 size = new Vector2();
                int i = 0;
                foreach (var item in items.Values)
                {
                    Vector2 stringSize = font.MeasureString(item.Text);
                    size.Y += (font.LineSpacing + spacing);

                    if (stringSize.X > size.X)
                    {
                        size.X = stringSize.X;
                    }
                    i++;
                }

                if (items.Count > 0)
                {
                    size.Y -= spacing;
                }
                return size;
            }
        }

        public bool DropShadow
        {
            set
            {
                foreach (var item in items.Values)
                {
                    item.DropShadow = value;
                }
            }
        }

        private float spacing;
        public float Spacing
        {
            get { return spacing; }
            set
            {
                spacing = value;
                PositionControls();
            }
        }

        public Menu(Game game, Vector2 position, SpriteFont font)
            : base(game)
        {
            items = new Dictionary<string, UIControl>();

            Position = position;
            this.font = font;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var item in items.Values)
            {
                item.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var item in items.Values)
            {
                item.Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        public void AddButton(string name, string text)
        {
            items.Add(name, new TextButton(Game, 
                new Vector2(Position.X, Position.Y + items.Count * (font.LineSpacing + spacing)), text, font));
        }

        public void AddLabel(string name, string text)
        {
            items.Add(name, new Label(Game, 
                new Vector2(Position.X, Position.Y + items.Count * (font.LineSpacing + spacing)), text, font));
        }

        public void Remove(string name)
        {
            items.Remove(name);
        }
        
        public UIControl this[string name]
        {
            get { return items[name]; }
        }

        private void PositionControls()
        {
            int i = 0;
            foreach (var item in items.Values)
            {
                item.Position = Position + new Vector2(0, i++ * (font.LineSpacing + spacing));
                item.Position = new Vector2((float)Math.Round(item.Position.X), (float)Math.Round(item.Position.Y));
            }
        }
    }
}
