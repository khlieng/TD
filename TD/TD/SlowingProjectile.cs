using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XNATools;

namespace TD
{
    class SlowingProjectile : Projectile
    {
        private static Dictionary<ITarget, TimedSetter<float>> targetMap = new Dictionary<ITarget, TimedSetter<float>>();

        private float speedReduction;
        private int duration;

        public SlowingProjectile(Game game, Vector2 position, ITarget target, float velocity, int onHitDamage, float speedReduction, int duration)
            : base(game, position, target, velocity, onHitDamage)
        {
            this.speedReduction = speedReduction;
            this.duration = duration;
        }

        protected override void OnHit()
        {
            Mob mob = (Mob)target;
            mob.VelocityFactor = 1.0f - speedReduction;

            if (targetMap.ContainsKey(target))
            {
                targetMap[target].Reset();
            }
            else
            {
                targetMap.Add(target, new TimedSetter<float>(Game, f => mob.VelocityFactor = f, 1.0f, duration));
            }

            base.OnHit();
        }
    }
}
