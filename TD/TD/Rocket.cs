using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    class Rocket : Projectile
    {
        private Emitter emitter;

        public Rocket(Game game, Vector2 position, ITarget target, float velocity, int onHitDamage)
            : base(game, position, target, velocity, onHitDamage, game.Content.Load<Texture2D>("rocket"))
        {
            emitter = new Emitter(game, game.Content.Load<Texture2D>("fire"), position, 5);
            emitter.MaxDirectionDevation = 180;
            emitter.MinVelocity = 20;
            emitter.MaxVelocity = 100;
            emitter.MinDuration = 50;
            emitter.MaxDuration = 200;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            emitter.Direction = -direction;
            emitter.Position = position - direction * 12;            
        }

        protected override void OnHit()
        {
            base.OnHit();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Game.Components.Remove(emitter);
            emitter.Dispose();
        }
    }
}
