﻿using System;
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
            emitter = new Emitter(game, position, 5, game.Content.Load<Texture2D>("fire"));
            emitter.MaxDirectionDevation = 180;
            emitter.MinVelocity = 20;
            emitter.MaxVelocity = 100;
            emitter.MinDuration = 50;
            emitter.MaxDuration = 200;
            emitter.MinScale = 0.1f;
            emitter.MaxScale = 0.8f;
            emitter.DecayTimeFraction = 0.2f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            emitter.Direction = -direction;
            emitter.Position = position - direction * 12;            
        }

        protected override void OnHit()
        {
            Emitter explosionEmitter = new Emitter(Game, target.Center, Game.Content.Load<Texture2D>("fire"));
            explosionEmitter.MaxDirectionDevation = 180;
            explosionEmitter.MinVelocity = 5;
            explosionEmitter.MaxVelocity = 50;
            explosionEmitter.MinDuration = 400;
            explosionEmitter.MaxDuration = 750;
            explosionEmitter.MinScale = 0.5f;
            explosionEmitter.MaxScale = 1.0f;
            explosionEmitter.DecayTimeFraction = 0.1f;
            explosionEmitter.Emit(100);            
            explosionEmitter.RemoveAfter(800);

            Emitter sparkEmitter = new Emitter(Game, target.Center, Game.Content.Load<Texture2D>("fireOrb"));
            sparkEmitter.MaxDirectionDevation = 180;
            sparkEmitter.MinVelocity = 100;
            sparkEmitter.MaxVelocity = 200;
            sparkEmitter.MaxDuration = 500;
            sparkEmitter.MinScale = 0.1f;
            sparkEmitter.MaxScale = 0.3f;
            sparkEmitter.DecayTimeFraction = 0.8f;
            sparkEmitter.Emit(50);
            sparkEmitter.RemoveAfter(600);
            
            Emitter smokeEmitter = new Emitter(Game, target.Center, 25, Game.Content.Load<Texture2D>("smoke"));
            smokeEmitter.Additive = false;
            smokeEmitter.MaxDirectionDevation = 180;
            smokeEmitter.MinVelocity = 15;
            smokeEmitter.MaxVelocity = 50;
            smokeEmitter.MinDuration = 200;
            smokeEmitter.MaxDuration = 750;
            smokeEmitter.MinScale = 0.4f;
            smokeEmitter.MaxScale = 0.8f;
            smokeEmitter.EmitOffset = 8;
            smokeEmitter.DecayTimeFraction = 0.2f;
            smokeEmitter.EmitFor(500);
            smokeEmitter.RemoveAfter(1500);

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
