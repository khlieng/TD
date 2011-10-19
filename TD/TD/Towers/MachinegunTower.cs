using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    class MachinegunTower : Tower
    {
        private Emitter bulletEmitter;

        public MachinegunTower(Game game, int row, int col, IMobContainer mobs)
            : base(game, row, col, mobs)
        {
            Name = "Machinegun Tower";

            upgradeLevels.Add(new TowerData() { Damage = 3, Speed = 0.05f, Range = 70.0f, Cost = 100 });
            upgradeLevels.Add(new TowerData() { Damage = 6, Speed = 0.05f, Range = 80.0f, Cost = 50 });
            upgradeLevels.Add(new TowerData() { Damage = 9, Speed = 0.05f, Range = 90.0f, Cost = 50 });
            upgradeLevels.Add(new TowerData() { Damage = 12, Speed = 0.05f, Range = 100.0f, Cost = 50 });
            upgradeLevels.Add(new TowerData() { Damage = 16, Speed = 0.05f, Range = 120.0f, Cost = 50 });
            SetStats(0);
        }

        protected override void LoadContent()
        {
            Texture = new Texture2D(Game.GraphicsDevice, 1, 1);
            Texture.SetData<Color>(new[] { Color.White });

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (Target != null && hot)
            {
                XNATools.Draw.Line(center, Target.Center, Color.Yellow);
            }

            base.Draw(gameTime);
        }

        protected override void Fire()
        {
            Projectile.FireRay(center, Target, mobs, damage, range, 2, (int)(damage * 0.25f));
            
            base.Fire();
        }
    }
}
