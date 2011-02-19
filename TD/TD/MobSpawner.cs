using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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
            amount = 20;
            timeBetweenMobs = TimeSpan.FromSeconds(interval / amount);
            
            foreach (GameState state in GameHelper.GetService<GameStateManager>().GetStates<MainGameState>())
            {
                state.AddComponent(this);
            }             
        }

        public override void Update(GameTime gameTime)
        {
            if (sendingMobs)
            {
                acc += gameTime.ElapsedGameTime;               

                if (acc > timeBetweenMobs)
                {
                    acc -= timeBetweenMobs;

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
            Mob mob = new Mob(Game, mobContainer.SpawnPoint + new Vector2(0, 4 - rand.Next(9)), new Vector2(60.0f + 2.0f * level, 0), 150 + 40 * level * level);
            mobContainer.AddMob(mob);
            mob.Died += (o, e) =>
            {
                if (e.Cause == CauseOfDeath.Killed)
                {
                    Player.AddMoney(6 + 2 * level);
                }
                mobContainer.RemoveMob(mob);
            };
            mobsSent++;
        }
    }
}
