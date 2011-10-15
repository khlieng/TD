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
            menu.AddButton("aa", "Anti-Alias: " + (config.AA ? "ON" : "OFF"));
            menu.AddButton("bloom", "Bloom: " + (config.Bloom ? "ON" : "OFF"));
            menu.AddButton("back", "Back");
            menu.DropShadow = true;

            menu.Position = new Vector2(400.0f - menu.Size.X / 2.0f, 300.0f - menu.Size.Y / 2.0f);

            (menu["title"] as Label).Color = Color.Orange;

            menu["aa"].Click += (o, e) =>
                {
                    config.AA = !config.AA;
                    (menu["aa"] as TextButton).Text = "Anti-Alias: " + (config.AA ? "ON" : "OFF");
                };

            menu["bloom"].Click += (o, e) =>
                {
                    config.Bloom = !config.Bloom;
                    (menu["bloom"] as TextButton).Text = "Bloom: " + (config.Bloom ? "ON" : "OFF");
                };

            menu["back"].Click += (o, e) => Manager.Swap(this, new MenuGameState(Game));

            AddComponent(menu);

            base.LoadContent(content);
        }
    }
}
