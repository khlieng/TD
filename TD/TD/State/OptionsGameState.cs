using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    class OptionsGameState : GameState
    {
        Label labelOptions;
        TextButton buttonAA;
        TextButton buttonBloom;
        TextButton buttonBack;

        bool aa = true;
        bool bloom = true;

        public OptionsGameState(Game game)
            : base(game)
        {
        }

        public override void LoadContent(ContentManager content)
        {
            labelOptions = new Label(Game, new Vector2(100, 80), "Options", TheGame.GetFont(Font.Large));
            labelOptions.Color = Color.Orange;
            labelOptions.DropShadow = true;

            buttonAA = new TextButton(Game, new Vector2(100, 100), "Anti-Alias: ON", TheGame.GetFont(Font.Large));
            buttonAA.DropShadow = true;
            buttonBloom = new TextButton(Game, new Vector2(100, 120), "Bloom: ON", TheGame.GetFont(Font.Large));
            buttonBloom.DropShadow = true;
            buttonBack = new TextButton(Game, new Vector2(100, 140), "Back", TheGame.GetFont(Font.Large));
            buttonBack.DropShadow = true;

            buttonAA.Click += (o, e) =>
                {
                    aa = !aa;
                    if (aa)
                    {
                        buttonAA.Text = "Anti-Alias: ON";
                    }
                    else
                    {
                        buttonAA.Text = "Anti-Alias: OFF";
                    }
                };

            buttonBloom.Click += (o, e) =>
                {
                    bloom = !bloom;
                    if (bloom)
                    {
                        buttonBloom.Text = "Bloom: ON";
                    }
                    else
                    {
                        buttonBloom.Text = "Bloom: OFF";
                    }
                };

            buttonBack.Click += (o, e) =>
                {
                    CleanUp();
                    Manager.Swap(this, new MenuGameState(Game));
                };

            base.LoadContent(content);
        }

        private void CleanUp()
        {
            Game.Components.Remove(labelOptions);
            Game.Components.Remove(buttonAA);
            Game.Components.Remove(buttonBloom);
            Game.Components.Remove(buttonBack);
        }
    }
}
