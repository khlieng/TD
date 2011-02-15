using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace TD
{
    class SlowingProjectile : Projectile
    {
        private static Dictionary<ITarget, DelayedCall<float>> targetMap = new Dictionary<ITarget, DelayedCall<float>>();

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
            //
            // This will be used for a machinegun-ish tower
            //
            //Emitter bloodEmitter = new Emitter(Game, target.Center, Game.Content.Load<Texture2D>("blood2"));
            //bloodEmitter.Additive = false;
            //bloodEmitter.Direction = direction;
            //bloodEmitter.MaxDirectionDevation = 15;
            //bloodEmitter.MinVelocity = 30;
            //bloodEmitter.MaxVelocity = 400;
            //bloodEmitter.MinDuration = 20;
            //bloodEmitter.MaxDuration = 300;
            //bloodEmitter.MinScale = 0.01f;
            //bloodEmitter.MaxScale = 0.2f;
            //bloodEmitter.EmitOffset = 8;
            //bloodEmitter.Emit(50);
            //new DelayedCall(Game, () => Game.Components.Remove(bloodEmitter), 500);   

            Mob mob = (Mob)target;
            mob.VelocityFactor = 1.0f - speedReduction;

            if (targetMap.ContainsKey(target))
            {
                targetMap[target].Reset();
            }
            else
            {
                targetMap.Add(target, new DelayedCall<float>(Game, f => mob.VelocityFactor = f, 1.0f, duration));
            }

            base.OnHit();
        }
    }
}
