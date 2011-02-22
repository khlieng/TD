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

        protected SpriteBatch spriteBatch;
        public Game Game { get; private set; }
        public GameStateManager Manager { get; set; }

        public GameState(Game game)
        {
            Game = game;
            spriteBatch = GameHelper.GetService<SpriteBatch>();
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

        public virtual void Update(GameTime gameTime)
        {
            foreach (GameComponent component in components)
            {
                component.Update(gameTime);
            }

            while (removeThese.Count > 0)
            {
                GameComponent component = removeThese.Dequeue();
                components.Remove(component);
                component.Dispose();
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
            foreach (GameComponent component in components)
            {
                if (component is DrawableGameComponent)
                {
                    ((DrawableGameComponent)component).Draw(gameTime);
                }
            }
        }
    }
}
