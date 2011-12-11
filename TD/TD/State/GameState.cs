using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace TD
{
    abstract class GameState
    {
        private LinkedList<GameComponent> components = new LinkedList<GameComponent>();
        private Queue<GameComponent> addThese = new Queue<GameComponent>();
        private Queue<GameComponent> removeThese = new Queue<GameComponent>();

        public ICollection<GameComponent> Components
        {
            get { return components; }
        }

        protected SpriteBatch spriteBatch;
        public Game Game { get; private set; }
        public GameStateManager Manager { get; set; }

        public bool Enabled { get; set; }
        public bool Visible { get; set; }

        public GameState(Game game)
        {
            Game = game;
            spriteBatch = game.GetService<SpriteBatch>();

            Enabled = true;
            Visible = true;
        }
        
        public void AddComponent(GameComponent component)
        {
            addThese.Enqueue(component);            
        }

        public void RemoveComponent(GameComponent component)
        {
            removeThese.Enqueue(component);
        }

        public virtual void Initialize()
        {
            foreach (GameComponent component in components)
            {
                component.Initialize();
            }

            LoadContent(Game.Content);
        }

        public virtual void LoadContent(ContentManager content)
        {
        }

        public virtual void UnloadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                foreach (GameComponent component in components)
                {
                    if (component.Enabled)
                    {
                        component.Update(gameTime);
                    }
                }
            }

            while (removeThese.Count > 0)
            {
                GameComponent component = removeThese.Dequeue();
                components.Remove(component);
            }

            while (addThese.Count > 0)
            {
                GameComponent component = addThese.Dequeue();
                component.Initialize();
                components.AddLast(component);
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (Visible)
            {
                foreach (GameComponent component in components)
                {
                    if (component is DrawableGameComponent && (component as DrawableGameComponent).Visible)
                    {
                        (component as DrawableGameComponent).Draw(gameTime);
                    }
                }
            }
        }
    }
}
