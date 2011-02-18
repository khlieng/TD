using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    class AOEProjectile : Projectile
    {
        protected IMobContainer mobContainer;
        protected float aoeRadius;

        public AOEProjectile(Game game, Vector2 position, ITarget target, IMobContainer mobContainer, 
            float velocity, int onHitDamage, float aoeRadius, Texture2D texture)
            : base(game, position, target, velocity, onHitDamage, texture)
        {
            this.mobContainer = mobContainer;
            this.aoeRadius = aoeRadius;
        }

        protected override void OnHit()
        {
            foreach (ITarget otherTarget in mobContainer.Mobs)
            {
                if (otherTarget != target && (otherTarget.Center - position).Length() < aoeRadius)
                {
                    otherTarget.DoDamage(damage);
                }
            }

            base.OnHit();
        }
    }
}
