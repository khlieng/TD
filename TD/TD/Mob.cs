using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNATools;

namespace TD
{
    enum CauseOfDeath { Killed, LeftMap }

    class Mob : MovingAgent, ITarget
    {
        private static Random rand = new Random();

        private SpriteBatch spriteBatch;
        private Animation animation;

        private List<Vector2> path;

        private int initialHp;
        private int hp;

        private ProgressBar hpBar;
        private MovingTextStream damageStream;

        public event EventHandler<DeathEventArgs> Died;

        public Vector2 Center
        {
            get { return Position; }
        }

        public int Health
        {
            get { return hp; }
        }

        public bool Modified { get; private set; }
        private float velocityFactor;
        public float VelocityFactor
        {
            get { return velocityFactor; }
            set
            {
                if (value == 1.0f)
                {
                    Modified = false;
                }
                else
                {
                    Modified = true;
                }
                float prev = velocityFactor;
                velocityFactor = value;
                if (prev > 0.0f)
                {
                    MaxSpeed = (MaxSpeed / prev) * velocityFactor;
                }
            }
        }

        public Mob(Game game, List<Vector2> path, float velocity, int initialHealth) 
            : base(game, path[0] + new Vector2(0, 4 - rand.Next(9)), velocity, 0.5f)
        {
            DrawOrder = 9;
            spriteBatch = GameHelper.GetService<SpriteBatch>();
            animation = new Animation(game, new[] { Game.Content.Load<Texture2D>("1"), Game.Content.Load<Texture2D>("2") }, 200);

            this.path = path;
            this.initialHp = initialHealth;
            this.hp = initialHp;
            VelocityFactor = 1.0f;
            Steering.Seek = true;

            hpBar = new ProgressBar(game, new Rectangle((int)Position.X - 16, (int)Position.Y - 16, 30, 6));
            hpBar.ForegroundColor = Color.Red;
            hpBar.Percentage = 100;
            damageStream = new MovingTextStream(Game, TheGame.GetFont(Font.MobMovingText), Color.White, -50.0f);

            GameState mainState = GameHelper.GetService<GameStateManager>().GetState<MainGameState>();
            mainState.AddComponent(this);
            mainState.AddComponent(hpBar);
            mainState.AddComponent(damageStream);

            burningTest = new Emitter(game, Center, 10.0f, 
                game.Content.Load<Texture2D>("fireOrb"), 
                game.Content.Load<Texture2D>("smoke"), 
                game.Content.Load<Texture2D>("fire"),
                game.Content.Load<Texture2D>("smoke"));
            burningTest.MinVelocity = 50;
            burningTest.MaxVelocity = 75;
            burningTest.MinScale = 0.2f;
            burningTest.MaxScale = 0.8f;
            burningTest.MinDuration = 500;
            burningTest.MaxDuration = 1500;
            burningTest.MinAcceleration = -200;
            burningTest.MaxAcceleration = 100;
            burningTest.MaxDirectionDevation = 10;
            burningTest.ScaleDecayTimeFraction = 0.8f;
            burningTest.EmitOffset = 4;
            burningTest.Direction = new Vector2(0, -1);
            //burningTest.Emitting = true;

        }
        Emitter burningTest;
        protected override void UnloadContent()
        {
            GameHelper.GetService<GameStateManager>().GetState<MainGameState>().RemoveComponent(hpBar);
            new DelayedCall(Game, () => Game.Components.Remove(damageStream), 1000);
            burningTest.RemoveAfter(2000);

            base.UnloadContent();
        }

        int currentTarget = -1;
        public override void Update(GameTime gameTime)
        {
            int closest = 0;
            for (int i = 1; i < path.Count; i++)
            {
                if ((path[i] - Center).LengthSquared() < (path[closest] - Center).LengthSquared())
                {
                    closest = i;
                }
            }

            if (currentTarget != closest + 1)
            {
                if (closest != path.Count - 1)
                {
                    Steering.Target = path[closest + 1] + new Vector2(rand.Next(9) - 4, rand.Next(9) - 4);
                    currentTarget = closest + 1;
                }
                else
                {
                    hp = 0;
                    OnMobDied(CauseOfDeath.LeftMap);
                }
            }
            burningTest.Position = Center;
            //burningTest.Direction = Vector2.Transform(burningTest.Direction, Matrix.CreateRotationZ(MathHelper.ToRadians(10.0f)));
            hpBar.Position = Position - new Vector2(16);
            damageStream.Position = Position - new Vector2(0, 16);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(animation.Texture, Position, null, Color.White, (float)Math.Atan2(Direction.Y, Direction.X), 
                new Vector2(16, 16), 0.6f, SpriteEffects.None, 0.0f);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public bool DoDamage(int amount)
        {
            hp -= amount;
            hpBar.Percentage = (int)((100.0f / initialHp) * hp);

            //new MovingText(Game, amount.ToString(), TheGame.GetFont(Font.MobMovingText), Position - new Vector2(16, 16), 
            //    new Vector2(Position.X - 16, Position.Y - 36), 500);
            damageStream.Add(amount);

            if (hp <= 0)
            {
                OnMobDied(CauseOfDeath.Killed);
                return true;
            }
            return false;
        }

        protected virtual void OnMobDied(CauseOfDeath cause)
        {
            burningTest.Emitting = false;
            if (Died != null)
            {
                Died(this, new DeathEventArgs(cause));
            }
        }
    }
}
