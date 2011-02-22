using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TD
{
    class MenuGameState : GameState
    {
        Label labelTitle;
        TextButton buttonGo;
        TextButton buttonOptions;
        TextButton buttonExit;

        public MenuGameState(Game game)
            : base(game)
        {
        }

        public override void LoadContent(ContentManager content)
        {
            labelTitle = new Label(Game, new Vector2(100, 80), "Some TD", TheGame.GetFont(Font.Large));
            labelTitle.Color = Color.Orange;
            labelTitle.DropShadow = true;            
            
            buttonGo = new TextButton(Game, new Vector2(100, 100), "GO!", TheGame.GetFont(Font.Large));
            buttonGo.DropShadow = true;
            buttonOptions = new TextButton(Game, new Vector2(100, 120), "Options", TheGame.GetFont(Font.Large));
            buttonOptions.DropShadow = true;
            buttonExit = new TextButton(Game, new Vector2(100, 140), "Exit", TheGame.GetFont(Font.Large));
            buttonExit.DropShadow = true;

            buttonGo.Click += (o, e) =>
                {
                    CleanUp();
                    Manager.Swap(this, new MainGameState(Game));
                };

            buttonOptions.Click += (o, e) =>
                {
                    CleanUp();
                    Manager.Swap(this, new OptionsGameState(Game));
                };

            buttonExit.Click += (o, e) => Game.Exit();
        }

        private void CleanUp()
        {
            Game.Components.Remove(buttonGo);
            Game.Components.Remove(buttonOptions);
            Game.Components.Remove(buttonExit);
            Game.Components.Remove(labelTitle);
        }
    }
}
