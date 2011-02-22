using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNATools;

namespace TD
{
    class SlowEffect : IProjectileEffect
    {
        private static Dictionary<ITarget, DelayedCall<float>> targetMap = new Dictionary<ITarget, DelayedCall<float>>();

        private float speedReduction;
        private int duration;

        public SlowEffect(float speedReduction, int duration)
        {
            this.speedReduction = speedReduction;
            this.duration = duration;
        }

        public void Apply(ITarget target)
        {
            Mob mob = (Mob)target;
            mob.VelocityFactor = 1.0f - speedReduction;

            if (targetMap.ContainsKey(target))
            {
                targetMap[target].Reset();
            }
            else
            {
                targetMap.Add(target, new DelayedCall<float>(GameHelper.Game, f => mob.VelocityFactor = f, 1.0f, duration));
            }
        }
    }
}
