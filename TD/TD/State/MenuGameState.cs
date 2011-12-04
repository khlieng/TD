using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNATools.UI;

namespace TD
{
    class MenuGameState : GameState
    {
        private Menu menu;
        private LineEmitter emitter;

        public MenuGameState(Game game)
            : base(game)
        {
        }

        public override void LoadContent(ContentManager content)
        {
            TypeWriter writer = new TypeWriter(Game, new Vector2(200, 175), "Tower Defense!", TheGame.GetFont(Font.Huge), Color.Orange);
            writer.Start(200);
            AddComponent(writer);

            menu = new Menu(Game, new Vector2(320, 200), TheGame.GetFont(Font.Large));
            menu.Spacing = 5;
            //menu.AddLabel("title", "Some TD");
            menu.AddButton("go", "Go!");
            menu.AddButton("options", "Options");
            menu.AddButton("exit", "Exit");
            menu.DropShadow = true;

            menu.Position = new Vector2(400.0f - menu.Size.X / 2.0f, 300.0f - menu.Size.Y / 2.0f);

            //(menu["title"] as Label).Color = Color.Orange;
            menu["go"].Click += (o, e) => Manager.Swap(this, new MainGameState(Game));
            menu["options"].Click += (o, e) => Manager.Swap(this, new OptionsGameState(Game));
            menu["exit"].Click += (o, e) => { Game.Exit(); emitter.Emitting = false; emitter.RemoveAfter(2000); };

            AddComponent(menu);

            emitter = new LineEmitter(Game, new Vector2(0, -50), new Vector2(800, -50), 10.0f,
                Game.Content.Load<Texture2D>("dot"));
            emitter.MinVelocity = 10;
            emitter.MaxVelocity = 20;
            emitter.MinScale = 0.2f;
            emitter.MaxScale = 0.8f;
            emitter.MinDuration = 1000;
            emitter.MaxDuration = 3000;
            emitter.MinAcceleration = 50;
            emitter.MaxAcceleration = 300;
            emitter.MaxDirectionDevation = 10;
            emitter.ScaleDecayTimeFraction = 0.8f;
            emitter.Color = Color.Orange;
            emitter.Emitting = true;
        }

        public override void UnloadContent()
        {
            emitter.RemoveAfter(0);

            base.UnloadContent();
        }
#if DEBUG
        public override void Draw(GameTime gameTime)
        {
            XNATools.Draw.Rect(menu.Position, menu.Size, Color.Red);
            XNATools.Draw.FilledRect(new Rectangle(0, 180, 800, 5), Color.White);
            XNATools.Draw.FilledRect(new Rectangle(0, 185, 800, 49), Color.Black);
            XNATools.Draw.FilledRect(new Rectangle(0, 185 + 49, 800, 5), Color.White);

            base.Draw(gameTime);
        }
#endif
    }
}
