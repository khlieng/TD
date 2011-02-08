﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

            target.Died += (o, e) =>
                {
                    lostTarget = true;
                    direction = target.Center - position;
                    direction.Normalize();                    
                };

            if (texture != null)
            {
                spriteBatch = GameHelper.GetService<SpriteBatch>();
            }

            foreach (GameState state in GameHelper.GetService<GameStateManager>().GetStates<MainGameState>())
            {
                state.AddComponent(this);
            }            
        }
        
        public override void Update(GameTime gameTime)
        {
            float timeDelta = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (!lostTarget)
            {
                direction = target.Center - position;
                float distance = direction.Length();
                direction.Normalize();
                
                if (distance > velocity * timeDelta)
                {
                    position += direction * velocity * timeDelta;
                }
                else
                {
                    position += direction * distance;
                }
            }
            else
            {
                position += direction * velocity * timeDelta;
                
            }

            if (!lostTarget)
            {
                if ((target.Center - position).Length() < 5.0f)
                {
                    target.DoDamage(damage);
                    OnHit();
                }
            }

            if (position.X < 0 || position.X > 640 || position.Y < 0 || position.Y > 480)
            {
                foreach (GameState state in GameHelper.GetService<GameStateManager>().GetStates<MainGameState>())
                {
                    state.RemoveComponent(this);
                    Dispose(true);                    
                }   
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

        //protected virtual void PositionChanged()
        //{
        //}

        //protected virtual void DirectionChanged()
        //{
        //}        

        protected virtual void OnHit()
        {
            foreach (GameState state in GameHelper.GetService<GameStateManager>().GetStates<MainGameState>())
            {
                state.RemoveComponent(this);
                Dispose(true);                
            }   

            if (Hit != null)
            {
                Hit(this, EventArgs.Empty);
            }
        }
    }
}
