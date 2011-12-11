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
        private Texture2D cannonTexture;

        public RocketTower(Game game, IMobContainer mobs)
            : base(game, mobs)
        {
            Name = "Rocket Tower";

            upgradeLevels.Add(new TowerData() { Damage = 20, Speed = 2.0f, Range = 150.0f, Cost = 100 });
            upgradeLevels.Add(new TowerData() { Damage = 35, Speed = 1.9f, Range = 150.0f, Cost = 50 });
            upgradeLevels.Add(new TowerData() { Damage = 50, Speed = 1.8f, Range = 150.0f, Cost = 50 });
            upgradeLevels.Add(new TowerData() { Damage = 65, Speed = 1.7f, Range = 150.0f, Cost = 50 });
            upgradeLevels.Add(new TowerData() { Damage = 90, Speed = 1.5f, Range = 150.0f, Cost = 50 });
            SetStats(0);
        }

        protected override void LoadContent()
        {
            Texture = Game.Content.Load<Texture2D>("tower_base");
            cannonTexture = Game.Content.Load<Texture2D>("rocket_top");

            base.LoadContent();
        }
        bool b = false;
        float off;
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Vector2 c = center;
            if (hot && !b)
            {
                b = true;
                off = 5.0f;
            }
            if (b && off > 0)
            {
                c -= direction * off;

                off -= 0.1f;
                
            }
            if (off == 0 && !hot)
                b = false;

            spriteBatch.Begin();
            spriteBatch.Draw(cannonTexture, new Rectangle((int)c.X, (int)c.Y, 32, 32), null, Color.White, 
                rotation, new Vector2(16, 16), SpriteEffects.None, 0);
            spriteBatch.End();
        }
        
        protected override void Fire()
        {
            new Rocket(Game, center, Target, mobs, 300.0f, damage);
            
            // Smoke <3
            Emitter emitter = new Emitter(Game, center, 1, Game.Content.Load<Texture2D>("smoke"));
            emitter.MaxDirectionDevation = 180;
            emitter.MinVelocity = 15;
            emitter.MaxVelocity = 30;
            emitter.MinDuration = 200;
            emitter.MaxDuration = 1000;
            emitter.MinScale = 0.4f;
            emitter.MaxScale = 0.8f;
            emitter.EmitOffset = 8;
            emitter.EmitFor(100);
            emitter.RemoveAfter(1200);       
            
            base.Fire();
        }
    }
}
