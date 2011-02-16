using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TD
{
    class Map : DrawableGameComponent, IMobContainer
    {
        struct Sprite
        {
            public int SheetIndex { get; set; }
            public Rectangle Source { get; set; }

            public Sprite(int sheetIndex, Rectangle source)
                : this()
            {
                SheetIndex = sheetIndex;
                Source = source;
            }
        }

        struct Tile
        {
            public int SpriteIndex { get; set; }
            public bool Walkable { get; set; }

            public Tile(int spriteIndex, bool walkable)
                : this()
            {
                SpriteIndex = spriteIndex;
                Walkable = walkable;
            }
        }

        private SpriteBatch spriteBatch;

        private Texture2D[] textures;
        private Sprite[] sprites;
        
        private int rows, cols;
        private Tile[,] tiles;

        private Tower[,] towers;

        private Vector2 spawn;
        private Vector2 exit;
        private List<Vector2> path;

        private LinkedList<Mob> mobs;
        public IEnumerable<ITarget> Mobs
        {
            get { return mobs; }
        }
        private MobSpawner spawner;
        private Queue<Mob> removeThese;

        public Vector2 SpawnPoint { get; private set; }

        public event EventHandler<MapClickArgs> Click;
        public event EventHandler<MapClickArgs> MouseTileEnter;
        public event EventHandler<MapClickArgs> MouseTileLeave;
        
        public Map(Game game, int cols, int rows) : base(game)
        {
            this.rows = rows;
            this.cols = cols;

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

            //Load(@"Maps\test.map");
            GeneratePath();
            SmoothPath(3);
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
            return towers[row, col] == null && !tiles[row, col].Walkable;
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

        int prevCol;
        int prevRow;
        
        public override void Update(GameTime gameTime)
        {
            MouseState current = Mouse.GetState();
            KeyboardState currentKey = Keyboard.GetState();

            int mCol = (current.X - (current.X % 32)) / 32;
            int mRow = (current.Y - (current.Y % 32)) / 32;

            if (0 <= mCol && mCol < 20 && 0 <= mRow && mRow < 15)
            {
                if (prev.LeftButton == ButtonState.Released &&
                    current.LeftButton == ButtonState.Pressed)
                {
                    OnClick(new MapClickArgs(mRow, mCol));
                }

                if (mCol != prevCol || mRow != prevRow)
                {
                    OnMouseTileLeave(new MapClickArgs(prevRow, prevCol));
                    OnMouseTileEnter(new MapClickArgs(mRow, mCol));
                }

                prevCol = mCol;
                prevRow = mRow;
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
                    if ((mob.Center - new Vector2(path[i].X * 32 + 16, path[i].Y * 32 + 16)).Length() < 4.0f)
                    {
                        if (i + 1 == path.Count)
                        {
                            removeThese.Enqueue(mob);
                            break;
                        }

                        Vector2 velocity = new Vector2(path[i + 1].X * 32 + 16, path[i + 1].Y * 32 + 16) -
                            new Vector2(path[i].X * 32 + 16, path[i].Y * 32 + 16);
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
                    spriteBatch.Draw(textures[sprites[tiles[row, col].SpriteIndex].SheetIndex], 
                        new Rectangle(col * 32, row * 32, 32, 32), sprites[tiles[row, col].SpriteIndex].Source, Color.White);
                }
            }
            spriteBatch.End();
#if DEBUG            
            for (int i = 0; i < path.Count - 1; i++)
            {
                XNATools.Draw.Line(path[i] * 32 + new Vector2(16, 16), path[i + 1] * 32 + new Vector2(16, 16), Color.Red);
            }
            foreach (Vector2 p in path)
            {
                XNATools.Draw.FilledRect(new Rectangle((int)(p.X * 32) + 14, (int)(p.Y * 32) + 14, 4, 4), Color.Black);
            }
#endif
            base.Draw(gameTime);
        }

        private void GeneratePath()
        {
            LinkedList<Node> openList = new LinkedList<Node>();
            LinkedList<Node> closedList = new LinkedList<Node>();

            Node startNode = new Node(spawn.X, spawn.Y);
            Node targetNode = new Node(exit.X, exit.Y);
            Node current = null;
            Vector2 currentP = new Vector2();
            openList.AddLast(startNode);

            while (openList.Count > 0)
            {
                current = openList.Where(n => n.f == openList.Min(no => no.f)).First();
                currentP.X = current.x;
                currentP.Y = current.y;

                if (currentP == exit)
                {
                    path = new List<Vector2>();
                    do
                    {
                        path.Add(new Vector2(current.y, current.x));
                    } while ((current = current.parent) != startNode);

                    path.Add(new Vector2(spawn.Y, spawn.X));
                    path.Reverse();
                    break;
                }

                openList.Remove(current);
                closedList.AddLast(current);

                foreach (Vector2 p in Neighbours(currentP))
                {
                    if (p.X < 0 || p.X >= rows || p.Y < 0 || p.Y >= cols) continue;

                    if (!tiles[(int)p.X, (int)p.Y].Walkable) continue;

                    if (closedList.Where(n => n.x == p.X && n.y == p.Y).Count() > 0) continue;

                    float g = (p.X != current.x && p.Y != current.y) ? current.g + 1.4f : current.g + 1f;
                    float h = Math.Abs(p.X - targetNode.x) + Math.Abs(p.Y - targetNode.y);

                    var q = openList.Where(n => n.x == p.X && n.y == p.Y);
                    Node onOpen = q.Count() > 0 ? q.First() : null;

                    if (onOpen == null)
                    {
                        openList.AddLast(new Node(p.X, p.Y, g, h, current));
                    }
                    else
                    {
                        if (g < onOpen.g)
                        {
                            onOpen.parent = current;
                            onOpen.g = g;
                            onOpen.f = onOpen.g + onOpen.h;
                        }
                    }
                }
            }
        }

        private void SmoothPath(int subdivs)
        {
            for (int i = 1; i < path.Count - 1; i++)
            {
                Vector2 p = path[i];
                Vector2 v1 = path[i] - path[i - 1];
                Vector2 v2 = path[i + 1] - path[i];
                Vector2 v12 = v1 + v2;

                if (v1 != v2)
                {
                    Vector2 apex = -0.5f * v1 + 0.5f * v2;
                    apex += p;

                    Vector2 dir = p - apex;
                    dir.Normalize();

                    float applePie = 0.0f;
                    
                    if (dir.X > 0.0f)
                    {
                        if (v12.Y > 0.0f)
                        {
                            dir = Vector2.Transform(dir, Matrix.CreateRotationZ(-MathHelper.PiOver4));
                            applePie = subdivs;
                        }
                        else
                        {
                            dir = Vector2.Transform(dir, Matrix.CreateRotationZ(MathHelper.PiOver4));
                            applePie = -subdivs;
                        }
                    }
                    else
                    {
                        if (v12.Y > 0.0f)
                        {
                            dir = Vector2.Transform(dir, Matrix.CreateRotationZ(MathHelper.PiOver4));
                            applePie = -subdivs;
                        }
                        else
                        {
                            dir = Vector2.Transform(dir, Matrix.CreateRotationZ(-MathHelper.PiOver4));
                            applePie = subdivs;
                        }
                    }

                    path.RemoveAt(i);
                    for (int j = 0; j < subdivs; j++)
                    {
                        path.Insert(i, apex + dir * 0.5f);
                        dir = Vector2.Transform(dir, Matrix.CreateRotationZ(MathHelper.PiOver2 / applePie));
                        i++;
                    }
                    path.Insert(i, apex + dir * 0.5f);
                    i++;
                }
            }
        }

        private IEnumerable<Vector2> Neighbours(Vector2 p)
        {
            //for (int x = p.X - 1; x <= p.X + 1; x++)
            //{
            //    for (int y = p.Y - 1; y <= p.Y + 1; y++)
            //    {
            //        if (!(x == p.X && y == p.Y))
            //        {
            //            yield return new Point(x, y);
            //        }
            //    }
            //}
            yield return p - new Vector2(1, 0);
            yield return p - new Vector2(0, 1);
            yield return p + new Vector2(1, 0);
            yield return p + new Vector2(0, 1);
        }

        private class Node
        {
            public Node parent;
            public float f, g, h, x, y;

            public Node(float x, float y)
                : this(x, y, 0f, 0f, null)
            {
            }

            public Node(float x, float y, float g, float h, Node parent)
            {
                this.x = x;
                this.y = y;
                this.g = g;
                this.h = h;
                this.parent = parent;
                f = g + h;
            }
        }

        protected virtual void OnClick(MapClickArgs args)
        {
            if (Click != null)
            {
                Click(this, args);
            }
        }

        protected virtual void OnMouseTileEnter(MapClickArgs args)
        {
            if (MouseTileEnter != null)
            {
                MouseTileEnter(this, args);
            }
        }

        protected virtual void OnMouseTileLeave(MapClickArgs args)
        {
            if (MouseTileLeave != null)
            {
                MouseTileLeave(this, args);
            }
        }

        public class MapReader : ContentTypeReader<Map>
        {
            protected override Map Read(ContentReader input, Map existingInstance)
            {
                Map map = new Map(GameHelper.Game, 20, 15);

                map.textures = new Texture2D[input.ReadInt32()];
                for (int i = 0; i < map.textures.Length; i++)
                {
                    map.textures[i] = GameHelper.Game.Content.Load<Texture2D>(Path.GetFileNameWithoutExtension(input.ReadString()));
                }

                map.sprites = new Sprite[input.ReadInt32()];
                for (int i = 0; i < map.sprites.Length; i++)
                {
                    map.sprites[i] = new Sprite(input.ReadInt32(),
                        new Rectangle(input.ReadInt32(), input.ReadInt32(), input.ReadInt32(), input.ReadInt32()));
                }

                map.spawn = new Vector2(input.ReadInt32(), input.ReadInt32());
                map.exit = new Vector2(input.ReadInt32(), input.ReadInt32());

                map.tiles = new Tile[map.rows, map.cols];
                for (int row = 0; row < map.rows; row++)
                {
                    for (int col = 0; col < map.cols; col++)
                    {
                        map.tiles[row, col] = new Tile(input.ReadInt32(), input.ReadBoolean());
                    }
                }

                return map;
            }
        }
    }
}
