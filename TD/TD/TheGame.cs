using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TD
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TheGame : Microsoft.Xna.Framework.Game
    {
        public static Dictionary<string, SpriteFont> Fonts { get; private set; }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameStateManager stateManager;
        BloomComponent bloom;

        Texture2D cursor;

        KeyboardState prevKeyState;
        MouseState prevMouseState;

        public TheGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;

            bloom = new BloomComponent(this);
            bloom.Settings = BloomSettings.PresetSettings[5];
            bloom.DrawOrder = 20;
            Components.Add(bloom);

            GameHelper.Game = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {   
            base.Initialize();
        }

        Emitter e;

        List<Vector2> points = new List<Vector2>();
        string text = string.Empty;
        List<Vector2> apexList;
        Vector2[] path = new[]
                {
                    new Vector2(0, 16),
                    new Vector2(32, 16),
                    new Vector2(64, 16),
                    new Vector2(96, 16),
                    new Vector2(96, 48),
                    new Vector2(96, 48 + 32),
                    new Vector2(96, 48 + 64),
                    new Vector2(64, 48 + 64),
                    new Vector2(32, 48 + 64),
                    new Vector2(0, 48 + 64),
                    new Vector2(0, 48 + 96),
                    new Vector2(0, 48 + 128),
                    new Vector2(0, 48 + 128 + 32),
                    new Vector2(32, 48 + 128 + 32),
                    new Vector2(64, 48 + 128 + 32)
                };
        List<Vector2> pathList;
        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            GameHelper.AddService<SpriteBatch>(spriteBatch);

            XNATools.Draw.Init(GraphicsDevice);
            
            cursor = Content.Load<Texture2D>("glowing_cursor");            

            Fonts = new Dictionary<string, SpriteFont>();
            Fonts.Add("Calibri 8", Content.Load<SpriteFont>("Calibri_8"));
            Fonts.Add("Calibri 12", Content.Load<SpriteFont>("Miramonte_14"));

            stateManager = new GameStateManager(this);
            GameHelper.AddService<GameStateManager>(stateManager);
            Components.Add(stateManager);
            
            stateManager.Add(new MainGameState(this));

            e = new Emitter(this, Vector2.Zero, 0.5f, Content.Load<Texture2D>("fireOrb"));
            e.MinScale = 0.5f;
            e.MaxScale = 0.5f;
            e.MinDuration = 600;
            e.MaxDuration = 600;
            e.AlphaDecayTimeFraction = 1.0f;
            e.ScaleDecayTimeFraction = 1.0f;
            e.Emitting = true;

            pathList = new List<Vector2>(path);
            apexList = new List<Vector2>();
            
            //Point p = new Point(16, 16);
            //Vector2 apex = new Vector2(0, 32);
            //Vector2 direction = new Vector2(0, -1);
            //float r = 16.0f;
            //points.Add(apex + direction * r);
            //for (int i = 0; i < 10; i++)
            //{
            //    direction = Vector2.Transform(direction, Matrix.CreateRotationZ((float)(Math.PI / 2) / 10.0f));
            //    points.Add(apex + direction * r);
            //}

            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        bool dumpIt;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();

            e.Position = new Vector2(currentMouseState.X, currentMouseState.Y);
            //e.Position += new Vector2(10, 10);

            if (currentKeyState.IsKeyDown(Keys.P) && prevKeyState.IsKeyUp(Keys.P))
            {
                dumpIt = true;
            }
            else
            {
                dumpIt = false;
            }

            if (currentKeyState.IsKeyDown(Keys.Q) && prevKeyState.IsKeyUp(Keys.Q))
                bloom.Visible = !bloom.Visible;

            prevKeyState = currentKeyState;
            prevMouseState = currentMouseState;

            base.Update(gameTime);
        }
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            bloom.BeginDraw();
            
            GraphicsDevice.Clear(Color.FromNonPremultiplied(20, 20, 20, 255));            
            
            base.Draw(gameTime);

            //spriteBatch.Begin();
            //spriteBatch.Draw(cursor, new Rectangle(Mouse.GetState().X - 10, Mouse.GetState().Y - 5, 32, 32), Color.White);
            //spriteBatch.End();

            spriteBatch.Begin();
            for (int i = 0; i < pathList.Count - 1; i++)
            {
                XNATools.Draw.Line(pathList[i], pathList[i + 1], Color.Red);                
            }

            foreach (Vector2 apex in apexList)
            {
                XNATools.Draw.FilledRect(apex - Vector2.One, new Vector2(2, 2), Color.Red);
            }
            //spriteBatch.DrawString(Fonts["Calibri 8"], text, new Vector2(10, 10), Color.White);
            spriteBatch.End();

            if (dumpIt)
            {
                Color[] data = new Color[800 * 600];
                GraphicsDevice.GetBackBufferData<Color>(data);

                Texture2D dump = new Texture2D(GraphicsDevice, 800, 600);
                dump.SetData<Color>(data);
                
                using (System.IO.Stream file = System.IO.File.Create(@"C:\Users\Ken-Håvard\Documents\Dump.png"))
                {
                    dump.SaveAsPng(file, 800, 600);
                }
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TheGame game = new TheGame())
            {
                game.Run();
            }
        }
    }
}
