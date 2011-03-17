using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Dynamic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XNATools;
using Tools;

namespace TD
{
    public enum Font { Small, Large, MobMovingText }

    public class TheGame : Game
    {
        public static SpriteFont GetFont(Font font)
        {
            switch (font)
            {
                case Font.Small:
                    return GameHelper.Game.Content.Load<SpriteFont>("Calibri_8");
                    
                case Font.Large:
                    return GameHelper.Game.Content.Load<SpriteFont>("Miramonte_14");

                case Font.MobMovingText:
                    return GameHelper.Game.Content.Load<SpriteFont>("Arial_8");

                default:
                    return null;
            }
        }

        static readonly string APPDATA = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static readonly string CONFIG_PATH = APPDATA + @"\TD\Settings.cfg";

        public static dynamic Config
        {
            get;
            private set;
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameStateManager stateManager;
        BloomComponent bloom;

        Texture2D cursor;
        Emitter cursorEmitter;

        KeyboardState prevKeyState;
        MouseState prevMouseState;

        bool dumpIt;

        public TheGame()
        {
            if (File.Exists(CONFIG_PATH))
            {
                Config = new Config(CONFIG_PATH);
            }
            else
            {
                Config = new Config(CONFIG_PATH);
                Config.Width = 800;
                Config.Height = 600;
                Config.Bloom = true;
                Config.AA = true;
            }

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = Config.Width;
            graphics.PreferredBackBufferHeight = Config.Height;
            graphics.PreferMultiSampling = Config.AA;

            if (Config.Bloom)
            {
                bloom = new BloomComponent(this);
                bloom.Settings = BloomSettings.PresetSettings[5];
                bloom.DrawOrder = 20;
                Components.Add(bloom);
            }

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
            
            stateManager = new GameStateManager(this);
            GameHelper.AddService<GameStateManager>(stateManager);
            Components.Add(stateManager);
            
            stateManager.Add(new MainGameState(this));

            cursorEmitter = new Emitter(this, Vector2.Zero, 0.5f, Content.Load<Texture2D>("fireOrb"));
            cursorEmitter.MinScale = 0.5f;
            cursorEmitter.MaxScale = 0.5f;
            cursorEmitter.MinDuration = 600;
            cursorEmitter.MaxDuration = 600;
            cursorEmitter.AlphaDecayTimeFraction = 1.0f;
            cursorEmitter.ScaleDecayTimeFraction = 1.0f;
            cursorEmitter.Emitting = true;

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
        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyState = Keyboard.GetState();
            MouseState currentMouseState = Mouse.GetState();

            if (currentKeyState.IsKeyDown(Keys.Escape) && prevKeyState.IsKeyUp(Keys.Escape))
            {
                Exit();
            }

            cursorEmitter.Position = new Vector2(currentMouseState.X, currentMouseState.Y);

            if (currentKeyState.IsKeyDown(Keys.P) && prevKeyState.IsKeyUp(Keys.P))
            {
                dumpIt = true;
            }
            else
            {
                dumpIt = false;
            }

            if (currentKeyState.IsKeyDown(Keys.Q) && prevKeyState.IsKeyUp(Keys.Q) && Config.Bloom)
            {
                bloom.Visible = !bloom.Visible;
            }

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
            if (bloom != null)
            {
                bloom.BeginDraw();
            }
            
            GraphicsDevice.Clear(Color.FromNonPremultiplied(20, 20, 20, 255));

            base.Draw(gameTime);

            //spriteBatch.Begin();
            //spriteBatch.Draw(cursor, new Rectangle(Mouse.GetState().X - 10, Mouse.GetState().Y - 5, 32, 32), Color.White);
            //spriteBatch.End();

            if (dumpIt)
            {
                Color[] data = new Color[800 * 600];
                GraphicsDevice.GetBackBufferData<Color>(data);

                Texture2D dump = new Texture2D(GraphicsDevice, 800, 600);
                dump.SetData<Color>(data);
                
                string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                using (Stream file = File.Create(CreateUniqueFileName(documents + @"\Dump.png")))
                {
                    dump.SaveAsPng(file, 800, 600);
                }
            }
        }

        private string CreateUniqueFileName(string path)
        {
            if (File.Exists(path))
            {
                string pathWithoutExtension = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);
                string extension = Path.GetExtension(path);

                string result;
                int number = 0;
                while (File.Exists(result = pathWithoutExtension + number + extension))
                {
                    number++;
                }
                return result;
            }
            return path;
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
