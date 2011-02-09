using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace TD
{
    class RocketTower : Tower
    {
        public RocketTower(Game game, int row, int col, IMobContainer mobs)
            : base(game, row, col, mobs)
        {
            upgradeLevels.Add(new TowerData() { Damage = 20, Speed = 2.0f, Range = 150.0f, Cost = 100 });
            upgradeLevels.Add(new TowerData() { Damage = 35, Speed = 1.9f, Range = 150.0f, Cost = 50 });
            upgradeLevels.Add(new TowerData() { Damage = 50, Speed = 1.8f, Range = 150.0f, Cost = 50 });
            upgradeLevels.Add(new TowerData() { Damage = 65, Speed = 1.7f, Range = 150.0f, Cost = 50 });
            upgradeLevels.Add(new TowerData() { Damage = 90, Speed = 1.5f, Range = 150.0f, Cost = 50 });
            SetStats(0);
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("rocket_tower");

            base.LoadContent();
        }
        
        protected override void Fire()
        {          
            new Rocket(Game, center, Target, 300.0f, damage);
            
            // Smoke <3
            Emitter emitter = new Emitter(Game, Game.Content.Load<Texture2D>("smoke"), center, 1);
            emitter.MaxDirectionDevation = 180;
            emitter.MinVelocity = 5;
            emitter.MaxVelocity = 30;
            emitter.MinDuration = 500;
            emitter.MaxDuration = 2000;
            new DelayedCall(Game, () => emitter.Emitting = false, 100);
            new DelayedCall(Game, () => Game.Components.Remove(emitter), 5000);            
            
            base.Fire();
        }
    }
}
