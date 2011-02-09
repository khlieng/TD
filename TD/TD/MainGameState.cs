using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TD
{
    class MainGameState : GameState
    {
        Texture2D overlay;
        Label timeLabel;
        Label fpsLabel;
        Label moneyLabel;
        Label livesLabel;
        Label towerInfoLabel;
        TextButton buttonUpgrade;

        Map map;
        Tower selectedTower;

        KeyboardState prevKeyState;

        static int money;
        static public void AddMoney(int amount)
        {
            money += amount;
        }

        static public bool TakeMoney(int amount)
        {
            if (money >= amount)
            {
                money -= amount;
                return true;
            }
            return false;
        }

        static int lives;
        static public void LifeLost()
        {
            lives--;
        }

        TowerType selected;

        public MainGameState(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            money = 500;
            lives = 20;

            base.Initialize();
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            overlay = Game.Content.Load<Texture2D>("overlay3");
            SetupUI();

            map = new Map(Game, 20, 15);
            map.Click += (o, e) =>
            {
                selectedTower = map.GetTower(e.Row, e.Col);
                map.AddTower(e.Row, e.Col, selected);
            };

            

            base.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            timeLabel.Text = String.Format("{0:00}:{1:00}:{2}", gameTime.TotalGameTime.Minutes,
                gameTime.TotalGameTime.Seconds, gameTime.TotalGameTime.Milliseconds);
            moneyLabel.Text = "Cash: " + money + "$";
            livesLabel.Text = "Lives: " + lives;

            if (selectedTower != null)
            {
                Tower.TowerData data = selectedTower.GetStats();
                if (buttonUpgrade.Hovered && selectedTower.UpgradeAvailable())
                {
                    Tower.TowerData upgradedData = selectedTower.GetNextUpgradeStats();
                    towerInfoLabel.Text = string.Format("Damage: {0} + {1}\n", data.Damage, upgradedData.Damage - data.Damage);
                    towerInfoLabel.Text += string.Format("Speed: {0:0.0} + {1:0.0}\n", data.Speed, upgradedData.Speed - data.Speed);
                    towerInfoLabel.Text += string.Format("Range: {0:0.0} + {1:0.0}\n", data.Range, upgradedData.Range - data.Range);
                    if (data.SlowPercentage != null)
                    {
                        towerInfoLabel.Text += string.Format("Slow: {0}% + {1}%\n", data.SlowPercentage, 
                            upgradedData.SlowPercentage - data.SlowPercentage);
                    }
                }
                else
                {
                    towerInfoLabel.Text = string.Format("Damage: {0}\n", data.Damage);
                    towerInfoLabel.Text += string.Format("Speed: {0:0.0}\n", data.Speed);
                    towerInfoLabel.Text += string.Format("Range: {0:0.0}\n", data.Range);
                    if (data.SlowPercentage != null)
                    {
                        towerInfoLabel.Text += string.Format("Slow: {0}%\n", data.SlowPercentage);
                    }
                }

                if (selectedTower.UpgradeAvailable())
                {
                    towerInfoLabel.Text += "Upgrade Cost: " + selectedTower.UpgradeCost();
                }

                buttonUpgrade.Visible = selectedTower.UpgradeAvailable();
                buttonUpgrade.Enabled = selectedTower.UpgradeAvailable();
            }
            else
            {
                towerInfoLabel.Text = string.Empty;
                buttonUpgrade.Visible = false;
                buttonUpgrade.Enabled = false;
            }
#if !DEBUG
            KeyboardState keyState = Keyboard.GetState();
            if (prevKeyState.IsKeyUp(Keys.F1) && keyState.IsKeyDown(Keys.F1))
            {
                timeLabel.Visible = !timeLabel.Visible;
                fpsLabel.Visible = !fpsLabel.Visible;
            }
            prevKeyState = keyState;
#endif
            if (lives <= 0)
            {
                Game.Exit();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(overlay, new Vector2(0, 0), Color.White);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        private void SetupUI()
        {
            TextButton buttonRocket = new TextButton(Game, new Vector2(20, 500), "Rocket Tower", TheGame.Fonts["Calibri 12"]);
            TextButton buttonSlow = new TextButton(Game, new Vector2(20, 540), "Slow Tower", TheGame.Fonts["Calibri 12"]);
            buttonRocket.ToggleAble = true;
            buttonRocket.DropShadow = true;
            buttonSlow.ToggleAble = true;
            buttonSlow.DropShadow = true;
            new ToggleGroup(buttonRocket, buttonSlow);
            new Tooltip(Game, buttonRocket, "Cost: 100") { TextColor = Color.Yellow };
            new Tooltip(Game, buttonSlow, "Cost: 150") { TextColor = Color.Yellow };

            buttonUpgrade = new TextButton(Game, new Vector2(650, 200), "Upgrade!", TheGame.Fonts["Calibri 12"]);
            buttonUpgrade.DropShadow = true;
            buttonUpgrade.Visible = false;

            fpsLabel = new FPSLabel(Game, new Vector2(650, 580), TheGame.Fonts["Calibri 12"]);
            timeLabel = new Label(Game, new Vector2(730, 580), String.Empty, TheGame.Fonts["Calibri 12"]);
#if !DEBUG
            fpsLabel.Visible = false;
            timeLabel.Visible = false;
#endif
            moneyLabel = new Label(Game, new Vector2(650, 5), "Cash: " + money + "$", TheGame.Fonts["Calibri 12"]);
            moneyLabel.Color = Color.Yellow;
            moneyLabel.DropShadow = true;
            livesLabel = new Label(Game, new Vector2(650, 25), "Lives: " + lives, TheGame.Fonts["Calibri 12"]);
            livesLabel.DropShadow = true;
            towerInfoLabel = new Label(Game, new Vector2(650, 60), string.Empty, TheGame.Fonts["Calibri 12"]);
            towerInfoLabel.DropShadow = true;
            
            buttonRocket.Click += (o, e) =>
                {
                    if (buttonRocket.Toggled)
                    {
                        selected = TowerType.Rocket;
                    }
                    else
                    {
                        selected = TowerType.None;
                    }
                };

            buttonSlow.Click += (o, e) =>
                {
                    if (buttonSlow.Toggled)
                    {
                        selected = TowerType.Slow;
                    }
                    else
                    {
                        selected = TowerType.None;
                    }
                };

            buttonUpgrade.Click += (o, e) =>
                {
                    if (TakeMoney(selectedTower.UpgradeCost()))
                    {
                        selectedTower.Upgrade();
                    }
                };
        }
    }
}
