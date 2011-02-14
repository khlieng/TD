using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

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
            emitter.MinScale = 0.1f;
            emitter.MaxScale = 0.8f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            emitter.Direction = -direction;
            emitter.Position = position - direction * 12;            
        }

        protected override void OnHit()
        {
            Emitter explosionEmitter = new Emitter(Game, Game.Content.Load<Texture2D>("fire"), target.Center);
            explosionEmitter.MaxDirectionDevation = 180;
            explosionEmitter.MinVelocity = 5;
            explosionEmitter.MaxVelocity = 50;
            explosionEmitter.MinDuration = 300;
            explosionEmitter.MaxDuration = 500;
            explosionEmitter.MinScale = 0.5f;
            explosionEmitter.MaxScale = 1.0f;
            explosionEmitter.Emit(100);
            new DelayedCall(Game, () => Game.Components.Remove(explosionEmitter), 600);

            Emitter smokeEmitter = new Emitter(Game, Game.Content.Load<Texture2D>("smoke"), target.Center, 5);
            smokeEmitter.Additive = false;
            smokeEmitter.MaxDirectionDevation = 180;
            smokeEmitter.MinVelocity = 15;
            smokeEmitter.MaxVelocity = 50;
            smokeEmitter.MinDuration = 200;
            smokeEmitter.MaxDuration = 800;
            smokeEmitter.MinScale = 0.4f;
            smokeEmitter.MaxScale = 0.8f;
            smokeEmitter.EmitOffset = 4;
            new DelayedCall(Game, () => smokeEmitter.Emitting = false, 100);
            new DelayedCall(Game, () => Game.Components.Remove(smokeEmitter), 1000);   

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
