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

    class Mob : DrawableGameComponent, ITarget
    {
        private static Random rand = new Random();

        private SpriteBatch spriteBatch;
        private Animation animation;

        private List<Vector2> path;

        private Vector2 position;
        private Vector2 velocity;
        private int initalHp;
        private int hp;

        private ProgressBar hpBar;

        public event EventHandler<DeathEventArgs> Died;
        
        public Vector2 Position
        {
            get { return position; }
        }

        public Vector2 Center
        {
            get { return new Vector2(position.X + 16, position.Y + 16); }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
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
                velocityFactor = value;
            }
        }

        public Mob(Game game, List<Vector2> path, Vector2 velocity, int initialHealth) : base(game)
        {
            DrawOrder = 9;
            spriteBatch = GameHelper.GetService<SpriteBatch>();
            animation = new Animation(game, new[] { Game.Content.Load<Texture2D>("1"), Game.Content.Load<Texture2D>("2") }, 200);

            this.path = path;
            position = path[0] - new Vector2(16, 16) + new Vector2(0, 4 - rand.Next(9));
            this.velocity = velocity;
            this.initalHp = initialHealth;
            this.hp = initalHp;
            VelocityFactor = 1.0f;

            hpBar = new ProgressBar(game, new Rectangle((int)position.X, (int)position.Y, 20, 8));
            hpBar.ForegroundColor = Color.Red;
            hpBar.Percentage = 100;

            foreach (GameState state in GameHelper.GetService<GameStateManager>().GetStates<MainGameState>())
            {
                state.AddComponent(this);
            }
        }

        protected override void UnloadContent()
        {
            foreach (GameState state in GameHelper.GetService<GameStateManager>().GetStates<MainGameState>())
            {
                state.RemoveComponent(hpBar);
            }   

            base.UnloadContent();
        }

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

            if (closest != path.Count - 1)
            {
                Vector2 newVelocity = path[closest + 1] - path[closest];
                newVelocity.Normalize();
                newVelocity *= Velocity.Length();
                Velocity = newVelocity;
            }
            else
            {
                hp = 0;
                OnMobDied(CauseOfDeath.LeftMap);
            }

            float elapsedSeconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            position += velocity * elapsedSeconds * VelocityFactor;
            hpBar.Bounds = new Rectangle((int)position.X, (int)position.Y, 30, 6);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(animation.Texture, Center, null, Color.White, (float)Math.Atan2(velocity.Y, velocity.X), 
                new Vector2(16, 16), 0.6f, SpriteEffects.None, 0.0f);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public bool DoDamage(int amount)
        {
            hp -= amount;
            hpBar.Percentage = (int)((100.0f / initalHp) * hp);

            new MovingText(Game, amount.ToString(), TheGame.GetFont(Font.MobMovingText), position, new Vector2(position.X, position.Y - 20), 500);

            if (hp <= 0)
            {
                OnMobDied(CauseOfDeath.Killed);
                return true;
            }
            return false;
        }

        protected virtual void OnMobDied(CauseOfDeath cause)
        {
            if (Died != null)
            {
                Died(this, new DeathEventArgs(cause));
            }
        }
    }
}
