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

        public Vector2 Position { get; set; }

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

                int i = 0;
                foreach (var item in items.Values)
                {
                    item.Position = Position + new Vector2(0, i++ * (font.LineSpacing + spacing));
                }
            }
        }

        public Menu(Game game, Vector2 position, SpriteFont font)
            : base(game)
        {
            Position = position;
            this.font = font;            

            items = new Dictionary<string, UIControl>();
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
    }
}
