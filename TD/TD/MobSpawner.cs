﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNATools;

namespace TD
{
    class MobSpawner : GameComponent
    {
        private IMobContainer mobContainer;
        private Random rand;
        private TimeSpan acc = TimeSpan.Zero;
        private TimeSpan timeBetweenMobs;
        private float interval;
        private int mobsSent;
        private bool sendingMobs;
        private int amount;
        private int level;

        public MobSpawner(Game game, IMobContainer container, float interval) : base(game)
        {
            mobContainer = container;
            rand = new Random();            
            this.interval = interval;
            amount = 50;
            timeBetweenMobs = TimeSpan.FromSeconds((rand.NextDouble() + 0.5) * (interval / amount));

            game.GetService<GameStateManager>().GetState<MainGameState>().AddComponent(this);   
        }

        public override void Update(GameTime gameTime)
        {
            if (sendingMobs)
            {
                acc += gameTime.ElapsedGameTime;               

                if (acc > timeBetweenMobs)
                {
                    acc -= timeBetweenMobs;
                    timeBetweenMobs = TimeSpan.FromSeconds((rand.NextDouble() + 0.5) * (interval / amount));

                    SendMob();
                }

                if (mobsSent == amount)
                {
                    WaveDone();                    
                }
            }

            base.Update(gameTime);
        }

        public void SendWave()
        {
            if (!sendingMobs)
            {
                sendingMobs = true;
            }
        }

        private void WaveDone()
        {
            sendingMobs = false;
            mobsSent = 0;

            amount++;
            level++;
        }

        private void SendMob()
        {
            Mob mob = new Mob(Game, mobContainer.Path, 40.0f + 2.0f * level, 150 + 40 * level * level);            
            mobContainer.AddMob(mob);
            mob.Died += (o, e) =>
            {
                switch (e.Cause)
                {
                    case CauseOfDeath.Killed:
                        Player.AddMoney(6 + 2 * level);
                        Player.AddXp();
                        break;

                    case CauseOfDeath.LeftMap:
                        Player.LoseLife();
                        break;
                }
                mobContainer.RemoveMob(mob);
            };
            mobsSent++;
        }
    }
}
