using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    class BouncingProjectile : Projectile
    {
        protected IMobContainer mobContainer;
        private int maxBounces;
        private int bounces;
        private float bounceRange;

        public BouncingProjectile(Game game, Vector2 position, ITarget target, IMobContainer mobContainer, 
            float velocity, int onHitDamage, int maxBounces, float bounceRange, Texture2D texture)
            : base(game, position, target, velocity, onHitDamage, texture)
        {
            this.mobContainer = mobContainer;
            this.maxBounces = maxBounces;
            this.bounceRange = bounceRange;
        }

        protected override void OnHit()
        {
            var mobsInRange = from mob in mobContainer.Mobs
                              let range = (mob.Center - position).Length()
                              where mob != target && range < bounceRange
                              orderby range
                              select mob;

            if (bounces < maxBounces && mobsInRange.Count() > 0)
            {
                bounces++;
                target = mobsInRange.First();
            }
            else
            {
                base.OnHit();
            }
        }
    }
}
