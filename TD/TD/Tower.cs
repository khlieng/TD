using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    enum TowerType { None, Rocket, Slow }

    abstract class Tower : DrawableGameComponent
    {
        protected SpriteBatch spriteBatch;
        protected Texture2D texture;
        
        public struct TowerData
        {
            public int Damage;
            public float Speed;
            public float Range;
            public int Cost;
        }
        protected int currentUpgrade;
        protected List<TowerData> upgradeLevels;

        protected int damage;
        private bool isCooling;
        protected bool hot;
        private float cooldown;
        protected float speed;
        protected float range;
        protected Vector2 position;
        protected Vector2 center;
        protected IMobContainer mobs;
        protected Rectangle? sourceRect;

        public ITarget Target { get; set; }
        public int Cost { get; private set; }

        private static int count = 0;
        private int id;

        public Tower(Game game, int row, int col, IMobContainer mobs) : base(game)
        {
            DrawOrder = 10;
            spriteBatch = GameHelper.GetService<SpriteBatch>();
            upgradeLevels = new List<TowerData>();
            
            position = new Vector2(col * 32, row * 32);
            center = new Vector2(position.X + 16, position.Y + 16);
            this.mobs = mobs;

            id = count;
            count++;
        }

        public TowerData GetStats()
        {
            return upgradeLevels[currentUpgrade];
        }

        public TowerData GetNextUpgradeStats()
        {
            if (UpgradeAvailable())
            {
                return upgradeLevels[currentUpgrade + 1];
            }
            else
            {
                return new TowerData();
            }
        }

        protected void SetStats(int level)
        {
            damage = upgradeLevels[level].Damage;
            speed = upgradeLevels[level].Speed;
            range = upgradeLevels[level].Range;
            Cost = upgradeLevels[level].Cost;
            currentUpgrade = level;
        }

        public virtual void Upgrade()
        {
            if (upgradeLevels.Count > currentUpgrade + 1)
            {
                currentUpgrade++;
                SetStats(currentUpgrade);
            }
        }

        public bool UpgradeAvailable()
        {
            return upgradeLevels.Count > currentUpgrade + 1;
        }

        public int UpgradeCost()
        {
            if (UpgradeAvailable())
            {
                return upgradeLevels[currentUpgrade + 1].Cost;
            }
            else
            {
                return -1;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (isCooling)
            {
                KeepCooling(gameTime);
            }

            if (Target == null || range < (Target.Center - center).Length())
            {
                Target = null;
                TryFindTarget();
            }

            if (Target != null && !isCooling)
            {
                Fire();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (hot)
            {
                spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, 32, 32), sourceRect, Color.Red);
            }
            else
            {
                spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, 32, 32), sourceRect, Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void KeepCooling(GameTime gameTime)
        {
            float timeDelta = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            cooldown += timeDelta;

            // The tower is in the hot state of 1/8th of the cooldown
            if (cooldown > speed / 8.0f)
            {
                hot = false;
            }

            if (cooldown > speed)
            {
                cooldown = 0.0f;
                isCooling = false;
            }
        }

        protected virtual void Fire()
        {
            isCooling = true;
            hot = true;
        }

        private void TryFindTarget()
        {
            foreach (ITarget mob in mobs.Mobs)
            {
                if ((mob.Center - center).Length() < range)
                {
                    Target = mob;
                    Target.Died += (o, e) => Target = null;
                    break;
                }
            }
        }
    }
}
