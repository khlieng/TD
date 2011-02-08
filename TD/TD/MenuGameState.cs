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
        public MenuGameState(Game game)
            : base(game)
        {
        }

        public override void LoadContent(ContentManager content)
        {
            Label label = new Label(Game, new Vector2(100, 80), "Some TD", TheGame.Fonts["Calibri 12"]);
            label.Color = Color.Orange;
            label.DropShadow = true;            
            
            TextButton button = new TextButton(Game, new Vector2(100, 100), "GO!", TheGame.Fonts["Calibri 12"]);
            button.DropShadow = true;
            TextButton buttonExit = new TextButton(Game, new Vector2(100, 120), "Exit", TheGame.Fonts["Calibri 12"]);
            buttonExit.DropShadow = true;

            button.Click += (o, e) =>
                {
                    Game.Components.Remove(button);
                    Game.Components.Remove(buttonExit);
                    Game.Components.Remove(label);

                    Manager.Swap(this, new MainGameState(Game));
                };

            buttonExit.Click += (o, e) => Game.Exit();
        }
    }
}
