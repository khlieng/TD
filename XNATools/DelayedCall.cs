using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNATools
{
    public class DelayedCall : GameComponent
    {
        private Action method;
        private int delay;
        private int elapsed;

        public bool TimerRunning { get; set; }

        public DelayedCall(Game game, Action method, int delay)
            : base(game)
        {
            this.method = method;
            this.delay = delay;

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
            if (elapsed >= delay)
            {
                method();
                TimerRunning = false;
                Game.Components.Remove(this);
            }

            base.Update(gameTime);
        }
    }
}
