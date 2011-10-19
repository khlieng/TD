using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    class FlameTower : Tower
    {
        private Emitter emitter;
        private float angle;

        public FlameTower(Game game, int row, int col, IMobContainer mobs)
            : base(game, row, col, mobs)
        {
            Name = "Flame Tower";

            upgradeLevels.Add(new TowerData() { Damage = 2, Speed = 0.1f, Range = 100.0f, Cost = 100 });
            upgradeLevels.Add(new TowerData() { Damage = 4, Speed = 0.1f, Range = 100.0f, Cost = 50 });
            upgradeLevels.Add(new TowerData() { Damage = 6, Speed = 0.1f, Range = 100.0f, Cost = 50 });
            upgradeLevels.Add(new TowerData() { Damage = 8, Speed = 0.1f, Range = 100.0f, Cost = 50 });
            upgradeLevels.Add(new TowerData() { Damage = 12, Speed = 0.1f, Range = 100.0f, Cost = 50 });
            SetStats(0);
        }

        protected override void LoadContent()
        {
            Texture = new Texture2D(Game.GraphicsDevice, 1, 1);
            Texture.SetData<Color>(new[] { Color.White });

            emitter = new Emitter(Game, center, 1, Game.Content.Load<Texture2D>("fire"));
            emitter.MaxDirectionDevation = 90;
            emitter.MinVelocity = 50;
            emitter.MaxVelocity = 100;
            emitter.MinDuration = 50;
            emitter.MaxDuration = 500;
            emitter.EmitOffset = 16;
            emitter.Emitting = true;

            base.LoadContent();
        }
        
        public override void Update(GameTime gameTime)
        {
            angle+= 5.0f;
            if (angle > 360.0f)
                angle = 0.0f;
            Vector2 direction = Vector2.Transform(Vector2.UnitX, Matrix.CreateRotationZ(MathHelper.ToRadians(angle)));
            direction.Normalize();
            emitter.Direction = direction;
            //if (Target != null)
            //{
            //    Vector2 direction = Target.Center - center;
            //    direction.Normalize();
            //    emitter.Direction = direction;
            //}
            //emitter.Emitting = Target != null;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //base.Draw(gameTime);
        }

        protected override void Fire()
        {
            Target.DoDamage(damage);

            base.Fire();
        }
    }
}
