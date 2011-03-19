using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TD
{
    class GameStateManager : DrawableGameComponent
    {
        private List<GameState> states;
        public IEnumerable<GameState> States
        {
            get { return states; }
        }

        public void Add(GameState state)
        {
            states.Add(state);
            state.Manager = this;
            state.Initialize();
        }

        public void Remove(GameState state)
        {
            state.UnloadContent();
            states.Remove(state);
        }

        public void Swap(GameState state, GameState newState)
        {
            Remove(state);
            Add(newState);
        }

        public GameState GetState<T>()
        {
            return states.Where(state => state.GetType().Equals(typeof(T))).First();
        }

        //public IEnumerable<GameState> GetStates<T>()
        //{
        //    foreach (GameState state in states.Where(state => state.GetType().Equals(typeof(T))))
        //    {
        //        yield return state;
        //    }
        //}

        public GameStateManager(Game game)
            : base(game)
        {
            states = new List<GameState>();            
        }

        public override void Initialize()
        {
            foreach (GameState state in states)
            {
                state.Initialize();
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            foreach (GameState state in states)
            {
                state.LoadContent(Game.Content);
            }

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            //foreach (GameState state in states)
            //{
            //    state.Update(gameTime);
            //}

            for (int i = 0; i < states.Count; i++)
            {
                states[i].Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //foreach (GameState state in states)
            //{
            //    state.Draw(gameTime);
            //}

            for (int i = 0; i < states.Count; i++)
            {
                states[i].Draw(gameTime);
            }

            base.Draw(gameTime);
        }
    }
}
