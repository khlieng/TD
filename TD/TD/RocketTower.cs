using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    class RocketTower : Tower
    {
        //private Texture2D projectileTexture;

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
            //projectileTexture = Game.Content.Load<Texture2D>("rocket");

            base.LoadContent();
        }

        protected override void Fire()
        {
            //new Projectile(Game, center, Target, 250.0f, damage, projectileTexture);            
            new Rocket(Game, center, Target, 300.0f, damage);
            
            base.Fire();
        }
    }
}
