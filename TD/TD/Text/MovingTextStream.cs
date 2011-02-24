using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace TD
{
    class MovingTextStream : DrawableGameComponent
    {
        struct TextItem
        {
            public Vector2 position;
            public string text;
            public float distance;
            public byte alpha;

            public TextItem(Vector2 position, string text)
                : this()
            {
                this.position = position;
                this.text = text;
                alpha = 255;
            }
        }

        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private Color color;

        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                Vector2 transform = value - position;
                position = value;
                for (int i = 0; i < items.Count; i++)
                {
                    items[i] = new TextItem(items[i].position + transform, items[i].text) 
                    { 
                        distance = items[i].distance,
                        alpha = items[i].alpha
                    };
                }
            }
        }

        private float direction;
        private float initialVerticalSpeed;
        private float verticalSpeed;
        private float verticalSpaceNeeded;
        private float itemDistance;

        private List<TextItem> items;
        private Queue<TextItem> addQueue;

        public MovingTextStream(Game game, SpriteFont font, Color color, float verticalSpeed)
            : base(game)
        {
            spriteBatch = GameHelper.GetService<SpriteBatch>();

            this.font = font;
            this.color = color;
            initialVerticalSpeed = verticalSpeed;
            direction = verticalSpeed < 0.0f ? -1.0f : 1.0f;
            verticalSpaceNeeded = font.MeasureString("A").Y;
            itemDistance = 50.0f;

            items = new List<TextItem>();
            addQueue = new Queue<TextItem>();
        }

        public void Add(string text)
        {
            addQueue.Enqueue(new TextItem(position, text));
        }

        public override void Update(GameTime gameTime)
        {
            if (items.Count > 0)
            {
                float verticalSpace = position.Y - items[items.Count - 1].position.Y;
                while (verticalSpace > verticalSpaceNeeded && addQueue.Count > 0)
                {
                    verticalSpace -= verticalSpaceNeeded;
                    items.Add(addQueue.Dequeue());
                }
            }
            else
            {
                if (addQueue.Count > 0)
                    items.Add(addQueue.Dequeue());
            }

            verticalSpeed = initialVerticalSpeed + direction * 20.0f * addQueue.Count;

            float elapsedSeconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            for (int i = 0; i < items.Count; i++)
            {
                float distance = items[i].distance;
                items[i] = new TextItem(items[i].position + new Vector2(0, verticalSpeed * elapsedSeconds), items[i].text) 
                {
                    distance = distance + Math.Abs(verticalSpeed * elapsedSeconds)
                };

                if (items[i].distance >= itemDistance)
                {
                    items.RemoveAt(i);
                }
                else if (items[i].distance > itemDistance / 2.0f)
                {
                    float delta = items[i].distance - itemDistance / 2.0f;
                    float alphaStep = 255.0f / (itemDistance / 2.0f);
                    
                    items[i] = new TextItem(items[i].position, items[i].text) 
                    {
                        distance = items[i].distance, 
                        alpha = (byte)(255.0f - alphaStep * delta) 
                    };
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            foreach (var item in items)
            {
                spriteBatch.DrawString(font, item.text, item.position, 
                    Color.FromNonPremultiplied(color.R, color.G, color.B, item.alpha));
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
