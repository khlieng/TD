using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNATools
{
    public class TimedSetter<T> : GameComponent
    {
        private Action<T> setter;
        private T val;
        private int time;
        private int elapsed;

        public bool TimerRunning { get; set; }

        public TimedSetter(Game game, Action<T> setter, T val, int time)
            : base(game)
        {
            this.setter = setter;
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
                setter(val);
                TimerRunning = false;
                Game.Components.Remove(this);
            }

            base.Update(gameTime);
        }
    }
}
