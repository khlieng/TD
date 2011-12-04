using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XNATools.UI;

namespace TD
{
    class OptionsGameState : GameState
    {
        private dynamic config;
        private Menu menu;

        public OptionsGameState(Game game)
            : base(game)
        {
            config = TheGame.Config;
        }

        public override void LoadContent(ContentManager content)
        {
            menu = new Menu(Game, new Vector2(100, 80), TheGame.GetFont(Font.Large));
            menu.Spacing = 5;
            menu.AddLabel("title", "Options");
            menu.AddLabel("aa", "Anti-Alias");
            menu.AddLabel("bloom", "Bloom");
            menu.AddButton("back", "Back");
            menu.DropShadow = true;
            
            menu.Position = new Vector2(400.0f - menu.Size.X / 2.0f, 300.0f - menu.Size.Y / 2.0f);

            (menu["title"] as Label).Color = Color.Orange;

            menu["back"].Click += (o, e) => Manager.Swap(this, new MenuGameState(Game));

            CheckBox checkBoxAA = new CheckBox(Game, new Rectangle((int)menu["aa"].Position.X - 20, (int)menu["aa"].Position.Y + 1, 15, 15));
            checkBoxAA.ForegroundColor = Color.Orange;
            checkBoxAA.Toggled = config.AA;
            checkBoxAA.ToggledChanged += (o, e) => config.AA = checkBoxAA.Toggled;
            menu["aa"].Click += (o, e) => checkBoxAA.Toggled = !checkBoxAA.Toggled;

            CheckBox checkBoxBloom = new CheckBox(Game, new Rectangle((int)menu["bloom"].Position.X - 20, (int)menu["bloom"].Position.Y + 1, 15, 15));
            checkBoxBloom.ForegroundColor = Color.Orange;
            checkBoxBloom.Toggled = config.Bloom;            
            checkBoxBloom.ToggledChanged += (o, e) => config.Bloom = checkBoxBloom.Toggled;
            menu["bloom"].Click += (o, e) => checkBoxBloom.Toggled = !checkBoxBloom.Toggled;

            AddComponent(menu);
            AddComponent(checkBoxAA);
            AddComponent(checkBoxBloom);

            base.LoadContent(content);
        }
    }
}
