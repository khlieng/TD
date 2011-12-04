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
using XNATools.UI;
using Tools;

namespace TD
{
    public enum Font { Small, Large, MobMovingText, Huge }

    public class TheGame : Game
    {
        public static SpriteFont GetFont(Font font)
        {
            switch (font)
            {
                case Font.Small:
                    return GameHelper.Content.Load<SpriteFont>("Calibri_8");
                    
                case Font.Large:
                    return GameHelper.Content.Load<SpriteFont>("Miramonte_14");

                case Font.MobMovingText:
                    return GameHelper.Content.Load<SpriteFont>("Arial_8");

                case Font.Huge:
                    return GameHelper.Content.Load<SpriteFont>("huge");

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
        Input input;
        TextOutput output;

        Texture2D cursor;
        Emitter cursorEmitter;

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
            IsMouseVisible = true;
            
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
            input = new Input(this);            
            input.KeyPressed += (key) =>
                {
                    switch (key)
                    {
                        case Keys.Escape:
                            //Exit();
                            break;

                        case Keys.F2:
                            output.Visible = !output.Visible;
                            break;

                        case Keys.F5:
                            dumpIt = true;
                            break;

                        case Keys.Q:
                            if (Config.Bloom)
                            {
                                bloom.Visible = !bloom.Visible;
                            }
                            break;
                    }
                };

            //Components.Add(input);
            this.AddService(input);
            
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
            this.AddService(spriteBatch);
            
            XNATools.Draw.Init(GraphicsDevice);
            
            cursor = Content.Load<Texture2D>("glowing_cursor");            
            
            stateManager = new GameStateManager(this);            
            Components.Add(stateManager);
            this.AddService(stateManager);

            //stateManager.Add(new MainGameState(this));
            stateManager.Add(new MenuGameState(this));

            cursorEmitter = new Emitter(this, Vector2.Zero, 0.5f, Content.Load<Texture2D>("fireOrb"));
            cursorEmitter.MinScale = 0.5f;
            cursorEmitter.MaxScale = 0.5f;
            cursorEmitter.MinDuration = 600;
            cursorEmitter.MaxDuration = 600;
            cursorEmitter.AlphaDecayTimeFraction = 0.2f;
            cursorEmitter.ScaleDecayTimeFraction = 1.0f;
            //cursorEmitter.Emitting = true;

            output = new TextOutput(this, new Vector2(5, 5), GetFont(Font.Small), bufferSize: 5);
            output.DropShadow = true;
            output.Visible = false;
            Components.Add(output);
            this.AddService(output);

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
            MouseState currentMouseState = Mouse.GetState();

            cursorEmitter.Position = new Vector2(currentMouseState.X, currentMouseState.Y);

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
                dumpIt = false;

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
