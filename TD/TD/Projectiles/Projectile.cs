using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace TD
{
    class Projectile : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private Texture2D texture;

        protected Vector2 position;
        protected Vector2 direction;
        protected ITarget target;
        protected float velocity;
        protected int damage;

        private bool lostTarget;

        public event EventHandler Hit;

        public List<IProjectileEffect> Effects { get; private set; }

        public Projectile(Game game, Vector2 position, ITarget target, float velocity, int onHitDamage)
            : this(game, position, target, velocity, onHitDamage, null)
        {
        }

        public Projectile(Game game, Vector2 position, ITarget target, float velocity, int onHitDamage, Texture2D texture)
            : base(game)
        {
            DrawOrder = 12;
            this.position = position;
            this.target = target;
            this.velocity = velocity;
            damage = onHitDamage;
            this.texture = texture;
            Effects = new List<IProjectileEffect>();

            target.Died += (o, e) =>
                {
                    lostTarget = true;
                    direction = target.Center - position;
                    direction.Normalize();                    
                };

            if (texture != null)
            {
                spriteBatch = game.GetService<SpriteBatch>();
            }

            game.GetService<GameStateManager>().GetState<MainGameState>().AddComponent(this);
        }
        
        public override void Update(GameTime gameTime)
        {
            float timeDelta = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (!lostTarget)
            {
                direction = target.Center - position;
                float distanceSquared = direction.LengthSquared();
                direction.Normalize();
                
                if (distanceSquared > (velocity * timeDelta) * (velocity * timeDelta))
                {
                    position += direction * velocity * timeDelta;
                }
                else
                {
                    position += direction * (float)Math.Sqrt(distanceSquared);
                }
            }
            else
            {
                position += direction * velocity * timeDelta;
                
            }

            if (!lostTarget)
            {
                if ((target.Center - position).LengthSquared() < 25.0f)
                {
                    target.DoDamage(damage);

                    ApplyEffects();

                    OnHit();
                }
            }

            if (position.X < 0 || position.X > 640 || position.Y < 0 || position.Y > 480)
            {
                Game.GetService<GameStateManager>().GetState<MainGameState>().RemoveComponent(this);
                Dispose(true);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (texture != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(texture, position, null, Color.White, (float)Math.Atan2(direction.Y, direction.X), new Vector2(16, 16), 1.0f, SpriteEffects.None, 0.0f);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        protected virtual void ApplyEffects()
        {
            ApplyEffects(target);
        }

        protected virtual void ApplyEffects(ITarget target)
        {
            foreach (IProjectileEffect effect in Effects)
            {
                effect.Apply(target);
            }
        }

        protected virtual void OnHit()
        {
            Game.GetService<GameStateManager>().GetState<MainGameState>().RemoveComponent(this);
            Dispose(true);

            if (Hit != null)
            {
                Hit(this, EventArgs.Empty);
            }
        }

        public static IEnumerable<Vector2> FireRay(Vector2 position, ITarget target, IMobContainer mobContainer, int damage,
            float range, int maxPassThrough, int passThroughDamageReduction)
        {
            Vector2 direction = target.Center - position;
            direction.Normalize();
            Ray ray = new Ray(new Vector3(position, 0), new Vector3(direction, 0));

            var mobsInRange = from mob in mobContainer.Mobs
                              let distance = (mob.Center - new Vector2(ray.Position.X, ray.Position.Y)).Length()
                              where mob != target && distance < range
                              orderby distance
                              select mob;

            target.DoDamage(damage);
            damage -= passThroughDamageReduction;
            int passThroughs = 1;
            yield return target.Center;

            foreach (ITarget mob in mobsInRange)
            {
                if (passThroughs < maxPassThrough && ray.Intersects(new BoundingSphere(new Vector3(mob.Center, 0), 10)) != null)
                {
                    mob.DoDamage(damage);
                    damage -= passThroughDamageReduction;
                    passThroughs++;
                    yield return mob.Center;
                }
            }
        }
    }
}
