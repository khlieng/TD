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
        ToggleGroup towerButtons;

        Map map;
        TowerType currentlyBuilding;
        Tower selectedTower;

        Vector2 radiusCircle;
        float radius;

        int mCol, mRow;
        MouseState prevMouse;

        public MainGameState(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            Player.Init(20, 500);

            base.Initialize();
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            overlay = Game.Content.Load<Texture2D>("overlay3");
            SetupUI();

            map = Game.Content.Load<Map>(@"Maps\test2");
            map.Click += (o, e) =>
            {
                selectedTower = map.GetTower(e.Row, e.Col);
                if (map.AddTower(e.Row, e.Col, currentlyBuilding))
                {
                    Tower added = map.GetTower(e.Row, e.Col);
                    Vector2 startPos = new Vector2(e.Col * 32, e.Row * 32);
                    new MovingText(Game, added.Cost + "$", Color.Yellow, startPos, startPos - new Vector2(0, 20), 500);
                }
            };

            map.MouseTileEnter += (o, e) =>
                {
                    Tower tower = map.GetTower(e.Row, e.Col);
                    if (tower != null)
                    {
                        radiusCircle = new Vector2(e.Col * 32 + 16, e.Row * 32 + 16);
                        radius = tower.GetStats().Range;
                    }
                };
            map.MouseTileLeave += (o, e) =>
                {
                    radius = 0.0f;
                };

            base.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            MouseState currentMouse = Mouse.GetState();
            mCol = (currentMouse.X - (currentMouse.X % 32)) / 32;
            mRow = (currentMouse.Y - (currentMouse.Y % 32)) / 32;

            if (currentMouse.RightButton == ButtonState.Pressed &&
                prevMouse.RightButton == ButtonState.Released)
            {
                currentlyBuilding = TowerType.None;
                towerButtons.UnToggleAll();

                if (map.GetTower(mRow, mCol) == null)
                {
                    radius = 0.0f;
                }
            }

            UpdateTowerInfo();

            timeLabel.Text = String.Format("{0:00}:{1:00}:{2}", gameTime.TotalGameTime.Minutes,
                gameTime.TotalGameTime.Seconds, gameTime.TotalGameTime.Milliseconds);        
#if !DEBUG
            KeyboardState keyState = Keyboard.GetState();
            if (prevKeyState.IsKeyUp(Keys.F1) && keyState.IsKeyDown(Keys.F1))
            {
                timeLabel.Visible = !timeLabel.Visible;
                fpsLabel.Visible = !fpsLabel.Visible;
            }
            prevKeyState = keyState;
#endif
            if (Player.Lives <= 0)
            {
                Game.Exit();
            }

            prevMouse = currentMouse;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();            
            spriteBatch.Draw(overlay, new Vector2(0, 0), Color.White);            
            spriteBatch.End();
            
            base.Draw(gameTime);

            DrawCurrentlyBuilding();

            if (radius > 0.0f)
            {
                XNATools.Draw.Circle(radiusCircle, radius, 32, Color.FromNonPremultiplied(255, 255, 255, 30));
            }
        }

        private void DrawCurrentlyBuilding()
        {
            if (currentlyBuilding != TowerType.None && 0 <= mCol && mCol < 20 && 0 <= mRow && mRow < 15)
            {
                Texture2D towerTexture = null;
                Color color = new Color();

                Tower tower = null;
                switch (currentlyBuilding)
                {
                    case TowerType.Rocket:
                        tower = new RocketTower(Game, 0, 0, null);
                        break;

                    case TowerType.Slow:
                        tower = new SlowTower(Game, 0, 0, null);
                        break;

                    case TowerType.Flame:
                        tower = new FlameTower(Game, 0, 0, null);
                        break;
                }

                if (map.CanAddTower(mRow, mCol) && Tower.TextureNames.ContainsKey(currentlyBuilding))
                {
                    if (Tower.TextureNames[currentlyBuilding] != string.Empty)
                    {
                        towerTexture = Game.Content.Load<Texture2D>(Tower.TextureNames[currentlyBuilding]);
                    }
                    radiusCircle = new Vector2(mCol * 32 + 16, mRow * 32 + 16);
                    radius = tower.GetStats().Range;
                    color = Color.White;
                    color.A = 128;
                }
                else if (!map.CanAddTower(mRow, mCol) && map.GetTower(mRow, mCol) == null && Tower.TextureNames.ContainsKey(currentlyBuilding))
                {
                    if (Tower.TextureNames[currentlyBuilding] != string.Empty)
                    {
                        towerTexture = Game.Content.Load<Texture2D>(Tower.TextureNames[currentlyBuilding]);
                    }
                    radius = 0.0f;
                    color = Color.Red;
                    color.A = 128;
                }

                if (towerTexture != null)
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(towerTexture, new Vector2(mCol * 32, mRow * 32), color);
                    spriteBatch.End();
                }
            }
        }

        private void UpdateTowerInfo()
        {
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
        }

        private void SetupUI()
        {
            Label towersLabel = new Label(Game, new Vector2(20, 490), "Towers:", TheGame.GetFont(Font.Large));
            towersLabel.DropShadow = true;
            TextButton buttonRocket = new TextButton(Game, new Vector2(20, 520), "Rocket", TheGame.GetFont(Font.Large));
            TextButton buttonSlow = new TextButton(Game, new Vector2(100, 520), "Slow", TheGame.GetFont(Font.Large));
            TextButton buttonFlame = new TextButton(Game, new Vector2(160, 520), "Flame", TheGame.GetFont(Font.Large));
            buttonRocket.ToggleAble = true;
            buttonRocket.DropShadow = true;
            buttonSlow.ToggleAble = true;
            buttonSlow.DropShadow = true;
            buttonFlame.ToggleAble = true;
            buttonFlame.DropShadow = true;
            towerButtons = new ToggleGroup(buttonRocket, buttonSlow, buttonFlame);
            new Tooltip(Game, buttonRocket, "Shootz dem rokkitz!\nDamange: 20\nSpeed: 2.0\nRange 150\nCost: 100") { TextColor = Color.Red };
            new Tooltip(Game, buttonSlow, "Slow jaaaa o_O\nDamage: 5\nSpeed: 0.5\nRange: 100\nSlow: 25%\nCost: 150") { TextColor = Color.LightSkyBlue };
            new Tooltip(Game, buttonFlame, "HAWT ^,^\nDamage: 2\nSpeed: 0.1\nRange: 100\nCost: 100") { TextColor = Color.OrangeRed };

            buttonUpgrade = new TextButton(Game, new Vector2(650, 200), "Upgrade!", TheGame.GetFont(Font.Large));
            buttonUpgrade.DropShadow = true;
            buttonUpgrade.Visible = false;

            fpsLabel = new FPSLabel(Game, new Vector2(640, 575), TheGame.GetFont(Font.Large));
            timeLabel = new Label(Game, new Vector2(717, 575), String.Empty, TheGame.GetFont(Font.Large));
#if !DEBUG
            fpsLabel.Visible = false;
            timeLabel.Visible = false;
#endif
            moneyLabel = new Label(Game, new Vector2(650, 5), "Cash: " + Player.Money + "$", TheGame.GetFont(Font.Large));
            moneyLabel.Color = Color.Yellow;
            moneyLabel.DropShadow = true;
            livesLabel = new Label(Game, new Vector2(650, 25), "Lives: " + Player.Lives, TheGame.GetFont(Font.Large));
            livesLabel.DropShadow = true;
            towerInfoLabel = new Label(Game, new Vector2(650, 60), string.Empty, TheGame.GetFont(Font.Large));
            towerInfoLabel.DropShadow = true;
            
            buttonRocket.Click += (o, e) => currentlyBuilding = buttonRocket.Toggled ? TowerType.Rocket : TowerType.None;
            buttonSlow.Click += (o, e) => currentlyBuilding = buttonSlow.Toggled ? TowerType.Slow : TowerType.None;
            buttonFlame.Click += (o, e) => currentlyBuilding = buttonFlame.Toggled ? TowerType.Flame : TowerType.None;

            buttonUpgrade.Click += (o, e) =>
                {
                    if (Player.TryTakeMoney(selectedTower.UpgradeCost()))
                    {
                        selectedTower.Upgrade();
                    }
                };

            Player.MoneyChanged += (o, e) => moneyLabel.Text = "Cash: " + Player.Money + "$";
            Player.LifeLost += (o, e) => livesLabel.Text = "Lives: " + Player.Lives;
        }
    }
}
