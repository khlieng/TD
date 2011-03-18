using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace TD
{
    class Timeline : DrawableGameComponent
    {
        struct TimelineItem
        {
            public float Start;
            public float End;
            public string Text;

            public TimelineItem(float start, float end, string text)
                : this()
            {
                Start = start;
                End = end;
                Text = text;
            }
        }

        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private Rectangle bounds;
        private Rect background;
        private Color backgroundColor;
                
        private List<TimelineItem> items;
        private float currentTime;
        private float pixelsPerSecond;

        public float Padding { get; set; }
        public Color ItemColor { get; set; }
        public Color TextColor { get; set; }
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                background = new Rect(bounds, backgroundColor, true);
            }
        }

        public Timeline(Game game, Rectangle bounds, float pixelsPerSecond)
            : base(game)
        {
            spriteBatch = game.GetService<SpriteBatch>();
            font = TheGame.GetFont(Font.Large);
            
            this.bounds = bounds;
            this.pixelsPerSecond = pixelsPerSecond;

            Padding = 4.0f;
            ItemColor = Color.Orange;
            TextColor = Color.Black;
            BackgroundColor = Color.Black;

            items = new List<TimelineItem>();
        }

        public override void Update(GameTime gameTime)
        {            
            currentTime += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            XNATools.Draw.Shape(background);

            foreach (var item in items)
            {
                if (TimeOverlap(item))
                {
                    Vector2 itemPos = new Vector2(bounds.X + (item.Start - currentTime) * pixelsPerSecond + Padding, bounds.Y + Padding);                    
                    Vector2 itemSize = new Vector2((item.End - item.Start) * pixelsPerSecond - Padding * 2, bounds.Height - Padding * 2);

                    if (itemPos.X < bounds.Left + Padding)
                    {
                        itemSize.X = (itemPos.X + itemSize.X) - bounds.Left - Padding;
                        itemPos.X = bounds.Left + Padding;
                    }

                    if (itemPos.X + itemSize.X > bounds.Right - Padding)
                    {
                        itemSize.X = bounds.Right - itemPos.X - Padding;
                    }

                    XNATools.Draw.FilledRect(itemPos, itemSize, ItemColor);

                    Vector2 textSize = font.MeasureString(item.Text);
                    if (textSize.X < itemSize.X)
                    {
                        Vector2 textPos = new Vector2(itemPos.X + itemSize.X / 2.0f - textSize.X / 2.0f,
                            itemPos.Y + itemSize.Y / 2.0f - textSize.Y / 2.0f);

                        spriteBatch.Begin();
                        spriteBatch.DrawString(font, item.Text, textPos, TextColor);
                        spriteBatch.End();
                    }
                }
            }

            base.Draw(gameTime);
        }

        public void Add(float startTime, float endTime, string text)
        {
            items.Add(new TimelineItem(startTime, endTime, text));
        }

        public void Start()
        {
            Enabled = true;
        }

        public void Stop()
        {
            Enabled = false;
        }

        public void ResetTime()
        {
            currentTime = 0;
        }

        public void JumpToNextItem()
        {
            if (items.Count > 0)
            {
                var q = from item in items
                        where item.Start > currentTime
                        orderby item.Start
                        select item.Start;

                if (q.Count() > 0)
                {
                    currentTime = q.Min();
                }
            }
        }

        private bool TimeOverlap(TimelineItem item)
        {
            return item.Start < currentTime + bounds.Width / pixelsPerSecond && item.End > currentTime;
        }
    }
}
