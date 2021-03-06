﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;
using XNATools.UI;

namespace TD
{
    //enum TowerType { None, Machinegun, Rocket, Slow, Flame }    

    abstract class Tower : Building
    {
        public struct TowerData
        {
            public int Damage;
            public float Speed;
            public float Range;
            public int? SlowPercentage;
            public int Cost;
        }
        protected int currentUpgrade;
        protected List<TowerData> upgradeLevels;

        public string Name { get; protected set; }

        protected IntStat damage;
        private bool isCooling;
        protected bool hot;
        private float cooldown;
        protected FloatStat speed;
        protected FloatStat range;

        protected float rotation;
        protected Vector2 direction;
        
        protected IMobContainer mobs;
        protected Rectangle? sourceRect;

        private ProgressBar cooldownBar;

        public ITarget Target { get; set; }
        
        private static int count = 0;
        private int id;

        public Tower(Game game, IMobContainer mobs) : base(game)
        {
            DrawOrder = 10;
            spriteBatch = game.GetService<SpriteBatch>();
            upgradeLevels = new List<TowerData>();

            this.mobs = mobs;

            id = count;
            count++;
        }

        protected override void UnloadContent()
        {
            Game.GetService<GameStateManager>().GetState<MainGameState>().RemoveComponent(cooldownBar);

            base.UnloadContent();
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
            Cost += upgradeLevels[level].Cost;
            currentUpgrade = level;
        }

        public virtual void Upgrade()
        {
            if (upgradeLevels.Count > currentUpgrade + 1)
            {
                currentUpgrade++;
                SetStats(currentUpgrade);
                new MovingText(Game, "  UP", TheGame.GetFont(Font.Small), position, position - new Vector2(0, 20), 500);
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

            if (Target != null)
            {
                direction = Target.Center - center;
                direction.Normalize();
                rotation = (float)(Math.Atan2(direction.Y, direction.X) + MathHelper.PiOver2);

                if (!isCooling)
                {
                    Fire();
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (hot)
            {
                spriteBatch.Draw(Texture, new Rectangle((int)position.X, (int)position.Y, 32, 32), sourceRect, Color.White);
            }
            else
            {
                spriteBatch.Draw(Texture, new Rectangle((int)position.X, (int)position.Y, 32, 32), sourceRect, Color.White);
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
