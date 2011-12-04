using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TD
{
    class DotEffect : IProjectileEffect
    {
        private ITarget target;
        private int tickDamage;
        private int tickFrequency;
        private int duration;
        private int elapsed;
        private int totalElapsed;

        public bool Finished { get; private set; }

        public DotEffect(ITarget target, int tickDamage, int tickFrequency, int duration)
        {
            this.target = target;
            this.tickDamage = tickDamage;
            this.tickFrequency = tickFrequency;
            this.duration = duration;
        }

        public void Apply(ITarget target)
        {
            if (!target.Effects.Any(e => e.GetType() == typeof(DotEffect)))
            {
                target.Effects.Add(this);
            }
        }

        public void Update(GameTime time)
        {
            elapsed += time.ElapsedGameTime.Milliseconds;
            totalElapsed += time.ElapsedGameTime.Milliseconds;

            if (elapsed >= tickFrequency)
            {
                elapsed -= tickFrequency;
                target.DoDamage(tickDamage);
            }

            if (totalElapsed >= duration)
            {
                Finished = true;
            }
        }
    }
}
