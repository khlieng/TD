using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNATools
{
    public class DelayedCall<T> : GameComponent
    {
        private Action<T> method;
        private T val;
        private int time;
        private int elapsed;

        public bool TimerRunning { get; set; }

        public DelayedCall(Game game, Action<T> method, T val, int time)
            : base(game)
        {
            this.method = method;
            this.val = val;
            this.time = time;

            TimerRunning = true;
            game.Components.Add(this);
        }

        public void Reset()
        {
            elapsed = 0;
            if (!TimerRunning)
            {
                TimerRunning = true;
                Game.Components.Add(this);
            }
        }

        public override void Update(GameTime gameTime)
        {
            elapsed += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsed >= time)
            {
                method(val);
                TimerRunning = false;
                Game.Components.Remove(this);
            }

            base.Update(gameTime);
        }
    }
}
