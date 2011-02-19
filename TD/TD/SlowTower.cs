using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    class SlowTower : Tower
    {
        private float speedReduction = 0.25f;

        public SlowTower(Game game, int row, int col, IMobContainer mobs)
            : base(game, row, col, mobs)
        {
            upgradeLevels.Add(new TowerData() { Damage = 5, Speed = 0.5f, Range = 100.0f, Cost = 150, SlowPercentage = 25 });
            upgradeLevels.Add(new TowerData() { Damage = 10, Speed = 0.5f, Range = 110.0f, Cost = 75, SlowPercentage = 30 });
            upgradeLevels.Add(new TowerData() { Damage = 15, Speed = 0.5f, Range = 120.0f, Cost = 75, SlowPercentage = 35 });
            upgradeLevels.Add(new TowerData() { Damage = 20, Speed = 0.5f, Range = 130.0f, Cost = 75, SlowPercentage = 40 });
            upgradeLevels.Add(new TowerData() { Damage = 25, Speed = 0.5f, Range = 150.0f, Cost = 75, SlowPercentage = 45 });
            SetStats(0);
        }

        protected override void LoadContent()
        {
            Texture = Game.Content.Load<Texture2D>("slow_tower");

            base.LoadContent();
        }

        public override void Upgrade()
        {
            if (upgradeLevels.Count > currentUpgrade + 1)
            {
                // 5% more slow ^^
                speedReduction += 0.05f;
            }

            base.Upgrade();
        }

        public override void Draw(GameTime gameTime)
        {
            if (Target != null && hot)
            {
                XNATools.Draw.Line(center, Target.Center, Color.LightSteelBlue);
            }

            base.Draw(gameTime);
        }

        protected override void Fire()
        {
            new AOEProjectile(Game, position, Target, mobs, 1000.0f, damage, 100.0f)
                .Effects.Add(new SlowEffect(speedReduction, 1000));

            base.Fire();
        }
    }    
}
