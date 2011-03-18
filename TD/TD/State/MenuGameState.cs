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
        private Menu menu;

        public MenuGameState(Game game)
            : base(game)
        {
        }

        public override void LoadContent(ContentManager content)
        {
            menu = new Menu(Game, new Vector2(100, 80), TheGame.GetFont(Font.Large));
            menu.Spacing = 5;
            menu.AddLabel("title", "Some TD");
            menu.AddButton("go", "Go!");
            menu.AddButton("options", "Options");
            menu.AddButton("exit", "Exit");
            menu.DropShadow = true;

            (menu["title"] as Label).Color = Color.Orange;
            menu["go"].Click += (o, e) => Manager.Swap(this, new MainGameState(Game));
            menu["options"].Click += (o, e) => Manager.Swap(this, new OptionsGameState(Game));
            menu["exit"].Click += (o, e) => Game.Exit();

            AddComponent(menu);
        }
    }
}
