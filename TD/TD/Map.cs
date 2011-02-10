using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TD
{
    class Map : DrawableGameComponent, IMobContainer
    {
        private SpriteBatch spriteBatch;

        enum TileType { Grass, Dirt }
        private static Texture2D[] textures;

        private Texture2D temp;
        private Rectangle[] sources = { new Rectangle(0, 0, 16, 16), new Rectangle(16, 0, 16, 16), new Rectangle(32, 0, 16, 16),
                                        new Rectangle(0, 16, 16, 16), new Rectangle(16, 16, 16, 16), new Rectangle(32, 16, 16, 16),
                                        new Rectangle(0, 32, 16, 16), new Rectangle(16, 32, 16, 16), new Rectangle(32, 32, 16, 16) };

        private int[,] tiles;
        private Tower[,] towers;

        private List<Point> path;

        private LinkedList<Mob> mobs;
        public IEnumerable<ITarget> Mobs
        {
            get { return mobs; }
        }
        private MobSpawner spawner;
        private Queue<Mob> removeThese;

        public Vector2 SpawnPoint { get; private set; }

        public event EventHandler<MapClickArgs> Click;
        
        public Map(Game game, int cols, int rows) : base(game)
        {
            foreach (GameState state in GameHelper.GetService<GameStateManager>().GetStates<MainGameState>())
            {
                state.AddComponent(this);
            }   
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            DrawOrder = 8;
            spriteBatch = GameHelper.GetService<SpriteBatch>();
            temp = Game.Content.Load<Texture2D>("tempsheet");
            
            if (textures == null)
            {
                textures = new Texture2D[2];
                textures[(int)TileType.Grass] = new Texture2D(Game.GraphicsDevice, 1, 1);
                textures[(int)TileType.Dirt] = new Texture2D(Game.GraphicsDevice, 1, 1);

                textures[(int)TileType.Grass].SetData<Color>(new[] { Color.DarkGreen });
                textures[(int)TileType.Dirt].SetData<Color>(new[] { Color.SaddleBrown });
            }

            //tiles = new int[rows, cols];
            tiles = tempMap;
            GeneratePath();
            SpawnPoint = new Vector2(path[0].X * 32, path[0].Y * 32);
            towers = new Tower[15, 20];

            mobs = new LinkedList<Mob>();
            spawner = new MobSpawner(Game, this, 20.0f);
            removeThese = new Queue<Mob>();

            base.LoadContent();
        }

        public bool AddTower(int row, int col, TowerType type)
        {
            if (CanAddTower(row, col))
            {
                Tower towerAdded = null;
                Tower towerToAdd = null;
                switch (type)
                {
                    case TowerType.Rocket:
                        towerToAdd = new RocketTower(Game, row, col, this);
                        break;

                    case TowerType.Slow:
                        towerToAdd = new SlowTower(Game, row, col, this);
                        break;

                    case TowerType.Flame:
                        towerToAdd = new FlameTower(Game, row, col, this);
                        break;
                }

                if (towerToAdd != null && MainGameState.TakeMoney(towerToAdd.Cost))
                {
                    towerAdded = towerToAdd;
                }

                if (towerAdded != null)
                {
                    towers[row, col] = towerAdded;
                    foreach (GameState state in GameHelper.GetService<GameStateManager>().GetStates<MainGameState>())
                    {
                        state.AddComponent(towerAdded);
                    }
                    return true;
                }                
            }
            return false;
        }

        public Tower GetTower(int row, int col)
        {
            return towers[row, col];
        }

        public bool CanAddTower(int row, int col)
        {
            return towers[row, col] == null && tiles[row, col] == (int)TileType.Grass;
        }

        public void AddMob(Mob mob)
        {
            mobs.AddLast(mob);            
        }

        public void RemoveMob(Mob mob)
        {
            if (selectedMob == mob)
            {
                selectedMob = null;
            }

            mobs.Remove(mob);
            foreach (GameState state in GameHelper.GetService<GameStateManager>().GetStates<MainGameState>())
            {
                state.RemoveComponent(mob);
            }
            mob.Dispose();
        }

        MouseState prev;
        KeyboardState prevKey;
        Mob selectedMob;
        public Mob SelectedMob
        {
            get { return selectedMob; }
        }
        
        public override void Update(GameTime gameTime)
        {
            MouseState current = Mouse.GetState();
            KeyboardState currentKey = Keyboard.GetState();

            if (prev.LeftButton == ButtonState.Released &&
                current.LeftButton == ButtonState.Pressed)
            {
                int mCol = (current.X - (current.X % 32)) / 32;
                int mRow = (current.Y - (current.Y % 32)) / 32;

                if (0 <= mCol && mCol < 20 && 0 <= mRow && mRow < 15)
                {
                    OnClick(new MapClickArgs(mRow, mCol));
                }
            }

            if (prevKey.IsKeyUp(Keys.Space) &&
                currentKey.IsKeyDown(Keys.Space))
            {
                spawner.SendWave();
            }
            
            foreach (Mob mob in mobs)
            {
                if (current.LeftButton == ButtonState.Pressed && prev.LeftButton == ButtonState.Released &&
                    current.X > mob.Position.X && current.X < mob.Position.X + 32 &&
                    current.Y > mob.Position.Y && current.Y < mob.Position.Y + 32)
                {
                    selectedMob = mob;
                }

                for (int i = 0; i < path.Count; i++)
                {
                    if ((mob.Center - new Vector2(path[i].X * 32 + 16, path[i].Y * 32 + 16)).Length() < 5.0f)
                    {
                        if (i + 1 == path.Count)
                        {
                            removeThese.Enqueue(mob);
                            break;
                        }

                        Vector2 velocity = new Vector2(path[i + 1].X * 32 + 16, path[i + 1].Y * 32 + 16) -
                            new Vector2(path[i].X * 32 + 16, path[i].Y * 32 + 16);
                            //mob.Center;
                        velocity.Normalize();
                        velocity *= mob.Velocity.Length();
                        mob.Velocity = velocity;
                    }
                }
            }

            while (removeThese.Count > 0)
            {
                MainGameState.LifeLost();
                Mob mob = removeThese.Dequeue();
                mob.LeftMap();
                RemoveMob(mob);
            }

            prev = current;
            prevKey = currentKey;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            for (int row = 0; row < 15; row++)
            {
                for (int col = 0; col < 20; col++)
                {                    
                    //spriteBatch.Draw(textures[tiles[row, col]], new Rectangle(col * 32, row * 32, 32, 32), Color.White);     
                    spriteBatch.Draw(temp, new Rectangle(col * 32, row * 32, 32, 32), sources[tempMap2[row, col]], Color.White);
                }
            }            
#if DEBUG
            foreach (Point p in path)
            {
                spriteBatch.Draw(textures[(int)TileType.Grass], new Rectangle(p.X * 32 + 14, p.Y * 32 + 14, 4, 4), Color.Black);
            }
#endif
            spriteBatch.End();

            //int mCol = (Mouse.GetState().X - (Mouse.GetState().X % 32)) / 32;
            //int mRow = (Mouse.GetState().Y - (Mouse.GetState().Y % 32)) / 32;
            
            //if (0 <= mCol && mCol < 20 && 0 <= mRow && mRow < 15)
            //{
            //    if (tiles[mRow, mCol] == 0)
            //    {
            //        XNATools.Draw.Rect(new Rectangle(mCol * 32, mRow * 32, 31, 31), Color.White);
            //    }
            //    else
            //    {
            //        XNATools.Draw.Rect(new Rectangle(mCol * 32, mRow * 32, 31, 31), Color.Red);
            //    }
            //}

            base.Draw(gameTime);
        }

        private void GeneratePath()
        {
            path = new List<Point>();
            Point current = new Point();
            int col = 0, row = 0;

            bool foundStart = false;
            while (!foundStart)
            {
                if (tiles[row, col] == 1)
                {
                    current.X = col;
                    current.Y = row;
                    path.Add(current);
                    foundStart = true;
                }

                row++;
            }

            row = current.Y;
            int prev = 0;
            while (col < 19)
            {
                if (prev != 4 && tiles[row, col + 1] == 1)
                {
                    // Go right until it change direction
                    while (col < 19 && tiles[row, col + 1] == 1)
                    {
                        col++;
                    }
                    current.X = col;
                    current.Y = row;
                    path.Add(current);
                    prev = 1;
                }
                else if (prev != 3 && row > 0 && tiles[row - 1, col] == 1)
                {
                    // Go up until dir change
                    while (row > 0 && tiles[row - 1, col] == 1)
                    {
                        row--;
                    }
                    current.X = col;
                    current.Y = row;
                    path.Add(current);
                    prev = 2;
                }
                else if (prev != 2 && tiles[row + 1, col] == 1)
                {
                    // Go down until dir change
                    while (row < 14 && tiles[row + 1, col] == 1)
                    {
                        row++;
                    }
                    current.X = col;
                    current.Y = row;
                    path.Add(current);
                    prev = 3;
                }
                else if (prev != 1 && col > 0 && tiles[row, col - 1] == 1)
                {
                    // Go backwards until dir change
                    while (col > 0 && tiles[row, col - 1] == 1)
                    {
                        col--;
                    }
                    current.X = col;
                    current.Y = row;
                    path.Add(current);
                    prev = 4;
                }
                else
                {
                    // Error in map!
                }
            }
        }

        protected virtual void OnClick(MapClickArgs args)
        {
            if (Click != null)
            {
                Click(this, args);
            }
        }

        static int[,] tempMap =
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 },
            { 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        };

        static int[,] tempMap2 =
        {
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 4, 4 },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4 },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4 },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4 },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4 },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, 4, 4 },
            { 4, 4, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 8, 4, 4 },
            { 4, 4, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 },
            { 4, 4, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 },
            { 4, 4, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 },
            { 4, 4, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 },
            { 4, 4, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 },
            { 4, 4, 6, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 },
        };
    }
}
