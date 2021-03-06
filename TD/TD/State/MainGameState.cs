﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNATools;
using XNATools.UI;

namespace TD
{
    class MainGameState : GameState
    {
        private Texture2D overlay;
        private Label timeLabel;
        private Label fpsLabel;
        private Label moneyLabel;
        private Label livesLabel;
        private Label towerInfoLabel;
        private TextButton buttonUpgrade;
        private TextButton buttonSell;
        private Tooltip sellTooltip;
        private ToggleGroup towerButtons;
        private Timeline waveTimeline;
        private ProgressBar xpBar;

        private Map map;
        //private Building currentlyBuilding;
        private Func<Building> currentlyBuilding;
        private Tower selectedTower;
        private PulsatingCircle selectionCircle;

        private Vector2 radiusCircle;
        private float radius;
        private float upgradedRadius;

        private int mCol, mRow;
        private MouseState prevMouse;

        private double gameSpeedMultiplier = 1.0;

        public MainGameState(Game game)
            : base(game)
        {
            new Input(game).KeyPressed += key => { if (key == Keys.Escape) Manager.Swap(this, new MenuGameState(game)); };
        }

        public override void Initialize()
        {
            Player.Init(20, 500);
#if !DEBUG
            Game.GetService<Input>().KeyPressed += (key) =>
                {
                    if (key == Keys.F1)
                    {
                        fpsLabel.Visible = !fpsLabel.Visible;
                        timeLabel.Visible = !timeLabel.Visible;
                    }
                };
#endif            
            base.Initialize();
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            overlay = Game.Content.Load<Texture2D>("overlay3");
            
            map = Game.Content.Load<Map>(@"Maps\test2");
            map.Click += (o, e) =>
                {
                    if (map.GetBuilding(e.Row, e.Col) is Tower)
                    {
                        selectedTower = (Tower)map.GetBuilding(e.Row, e.Col);
                    }
                    else if (map.GetBuilding(e.Row, e.Col) == null)
                    {
                        selectedTower = null;
                    }
                
                    if (selectionCircle != null)
                    {
                        RemoveComponent(selectionCircle);
                    }
                    if (selectedTower != null)
                    {
                        radiusCircle = new Vector2(e.Col * 32 + 16, e.Row * 32 + 16);
                        radius = selectedTower.GetStats().Range;

                        Color c = Color.Red;
                        c.A = 255;
                        selectionCircle = new PulsatingCircle(Game, new Vector2(selectedTower.Col * 32 + 15.6f,
                            selectedTower.Row * 32 + 15.6f), 16.5f, 16, c, false, 2.0f);
                        AddComponent(selectionCircle);
                    }
                    else
                    {
                        radius = 0f;
                    }
                                
                    if (currentlyBuilding != null && map.Build(e.Row, e.Col, currentlyBuilding()))
                    {
                        Building added = map.GetBuilding(e.Row, e.Col);
                        Vector2 startPos = new Vector2(e.Col * 32, e.Row * 32);
                        new MovingText(Game, "- $" + added.Cost, TheGame.GetFont(Font.Small), Color.Red, startPos, startPos - new Vector2(0, 20), 500);                        
                    }
                };

            map.MouseTileEnter += (o, e) =>
                {
                    if (!map.CanBuild(e.Row, e.Col))
                    {
                        radius = 0.0f;
                    }
                };
            map.MouseTileLeave += (o, e) =>
                {
                    if (selectedTower == null && currentlyBuilding == null)
                    {
                        radius = 0.0f;
                    }
                };

            SetupUI();
                        
            base.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            if (gameSpeedMultiplier != 1.0)
            {
                gameTime = new GameTime(
                    TimeSpan.FromTicks((long)(gameTime.TotalGameTime.Ticks * gameSpeedMultiplier)),
                    TimeSpan.FromTicks((long)(gameTime.ElapsedGameTime.Ticks * gameSpeedMultiplier)),
                    gameTime.IsRunningSlowly);
            }

            MouseState currentMouse = Mouse.GetState();
            mCol = (currentMouse.X - (currentMouse.X % 32)) / 32;
            mRow = (currentMouse.Y - (currentMouse.Y % 32)) / 32;

            if (currentMouse.RightButton == ButtonState.Pressed &&
                prevMouse.RightButton == ButtonState.Released)
            {
                currentlyBuilding = null;
                towerButtons.UnToggleAll();

                if (map.GetBuilding(mRow, mCol) == null)
                {
                    radius = 0.0f;
                }
            }

            UpdateTowerInfo();

            timeLabel.Text = String.Format("{0}:{1:00}", gameTime.TotalGameTime.Minutes,
                gameTime.TotalGameTime.Seconds);        

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
                XNATools.Draw.FilledCircle(radiusCircle, radius, 32, Color.FromNonPremultiplied(0, 0, 0, 16));
                if (upgradedRadius > 0.0f)
                {
                    float delta = upgradedRadius - radius;
                    Circle c = new Circle(radiusCircle, radius + delta / 2, 32, Color.FromNonPremultiplied(0, 255, 0, 16), false, delta);
                    XNATools.Draw.Shape(c);
                }
                XNATools.Draw.Circle(radiusCircle, radius, 32, Color.FromNonPremultiplied(0, 0, 0, 64));
                if (upgradedRadius > 0.0f)
                {
                    float delta = upgradedRadius - radius;
                    XNATools.Draw.Circle(radiusCircle, radius + delta, 32, Color.FromNonPremultiplied(0, 255, 0, 64));
                }
            }
        }

        private void DrawCurrentlyBuilding()
        {
            //if (currentlyBuilding != TowerType.None && 0 <= mCol && mCol < 20 && 0 <= mRow && mRow < 15)
            //{
            //    Texture2D towerTexture = null;
            //    Color color = new Color();

            //    Tower tower = null;
            //    switch (currentlyBuilding)
            //    {
            //        case TowerType.Machinegun:
            //            tower = new MachinegunTower(Game, 0, 0, null);
            //            break;

            //        case TowerType.Rocket:
            //            tower = new RocketTower(Game, 0, 0, null);
            //            break;

            //        case TowerType.Slow:
            //            tower = new SlowTower(Game, 0, 0, null);
            //            break;

            //        case TowerType.Flame:
            //            tower = new FlameTower(Game, 0, 0, null);
            //            break;
            //    }

            //    if (map.CanBuild(mRow, mCol) && Tower.TextureNames.ContainsKey(currentlyBuilding))
            //    {
            //        if (Tower.TextureNames[currentlyBuilding] != string.Empty)
            //        {
            //            towerTexture = Game.Content.Load<Texture2D>(Tower.TextureNames[currentlyBuilding]);
            //        }
            //        radiusCircle = new Vector2(mCol * 32 + 16, mRow * 32 + 16);
            //        radius = tower.GetStats().Range;
            //        color = Color.White;
            //        color.A = 128;
            //    }
            //    else if (!map.CanBuild(mRow, mCol) && map.GetBuilding(mRow, mCol) == null && Tower.TextureNames.ContainsKey(currentlyBuilding))
            //    {
            //        color = Color.Red;
            //        color.A = 64;
            //        XNATools.Draw.FilledCircle(new Vector2(mCol * 32 + 16, mRow * 32 + 16), 14.0f, 16, color);
            //    }

            //    if (towerTexture != null)
            //    {
            //        spriteBatch.Begin();
            //        spriteBatch.Draw(towerTexture, new Vector2(mCol * 32, mRow * 32), color);
            //        spriteBatch.End();
            //    }
            //}
            if (currentlyBuilding != null && 0 <= mCol && mCol < 20 && 0 <= mRow && mRow < 15)
            {
                Color color = Color.White;
                if (!map.CanBuild(mRow, mCol))
                {
                    color = Color.Red;
                }
                color.A = 128;

                spriteBatch.Begin();
                spriteBatch.Draw(currentlyBuilding().Texture, new Vector2(mCol * 32, mRow * 32), color);
                spriteBatch.End();
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
                    int deltaDamage = upgradedData.Damage - data.Damage;
                    float deltaSpeed = SpeedToAPS(upgradedData.Speed) - SpeedToAPS(data.Speed);
                    float deltaRange = upgradedData.Range - data.Range;

                    towerInfoLabel.Text = selectedTower.Name + "\n\n";
                    towerInfoLabel.Text += string.Format("Damage: {0} {1}\n", data.Damage, 
                        deltaDamage != 0 ? "+ " + deltaDamage : "");
                    towerInfoLabel.Text += string.Format("Speed: {0:0.00} {1}\n", SpeedToAPS(data.Speed), 
                        deltaSpeed != 0 ? string.Format("+ {0:0.00}", deltaSpeed) : "");
                    towerInfoLabel.Text += string.Format("Range: {0:0} {1}\n", data.Range, 
                        deltaRange != 0 ? string.Format("+ {0:0}", deltaRange) : "");
                    if (data.SlowPercentage != null)
                    {
                        int? deltaSlow = upgradedData.SlowPercentage - data.SlowPercentage;
                        towerInfoLabel.Text += string.Format("Slow: {0}% {1}\n", data.SlowPercentage,
                            deltaSlow != 0 ? "+ " + deltaSlow + "%" : "");
                    }
                }
                else
                {
                    towerInfoLabel.Text = selectedTower.Name + "\n\n";
                    towerInfoLabel.Text += string.Format("Damage: {0}\n", data.Damage);
                    towerInfoLabel.Text += string.Format("Speed: {0:0.00}\n", SpeedToAPS(data.Speed));
                    towerInfoLabel.Text += string.Format("Range: {0:0}\n", data.Range);
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

                sellTooltip.Text = "Returns " + (int)(selectedTower.Cost * 0.75) + "$";
            }
            else
            {
                towerInfoLabel.Text = string.Empty;
                buttonUpgrade.Visible = false;
                buttonUpgrade.Enabled = false;
            }

            buttonSell.Visible = selectedTower != null;
            buttonSell.Enabled = selectedTower != null;
        }

        //private string GetTowerName(Tower tower)
        //{
        //    if (tower is RocketTower)
        //        return "Rocket Tower";
        //    else if (tower is SlowTower)
        //        return "Slow Tower";
        //    else if (tower is FlameTower)
        //        return "Flame Tower";
        //    else if (tower is MachinegunTower)
        //        return "Machinegun Tower";
        //    else
        //        return string.Empty;
        //}

        private float SpeedToAPS(float speed)
        {
            return 1.0f / speed;
        }

        private void SetupUI()
        {
            SpriteFont fontLarge = TheGame.GetFont(Font.Large);

            Label towersLabel = new Label(Game, new Vector2(20, 492), "// Towers //", TheGame.GetFont(Font.Large));
            towersLabel.DropShadow = true;
            towersLabel.Color = Color.Orange;
            TextButton buttonMachinegun = new TextButton(Game, new Vector2(20, 520), "Machinegun", TheGame.GetFont(Font.Large));
            TextButton buttonRocket = new TextButton(Game, new Vector2(buttonMachinegun.Bounds.Right + 20, 520), "Rocket", TheGame.GetFont(Font.Large));
            TextButton buttonSlow = new TextButton(Game, new Vector2(buttonRocket.Bounds.Right + 20, 520), "Slow", TheGame.GetFont(Font.Large));
            TextButton buttonFlame = new TextButton(Game, new Vector2(buttonSlow.Bounds.Right + 20, 520), "Flame", TheGame.GetFont(Font.Large));
            buttonMachinegun.ToggleAble = true;
            buttonMachinegun.DropShadow = true;
            buttonRocket.ToggleAble = true;
            buttonRocket.DropShadow = true;
            buttonSlow.ToggleAble = true;
            buttonSlow.DropShadow = true;
            buttonFlame.ToggleAble = true;
            buttonFlame.DropShadow = true;

            TextButton buttonBank = new TextButton(Game, new Vector2(buttonFlame.Bounds.Right + 20, 520), "Bank", TheGame.GetFont(Font.Large));
            buttonBank.ToggleAble = true;
            buttonBank.DropShadow = true;

            towerButtons = new ToggleGroup(buttonMachinegun, buttonRocket, buttonSlow, buttonFlame, buttonBank);

            var machinegunTowerData = new MachinegunTower(Game, null).GetStats();
            Tooltip machinegunTooltip = new Tooltip(Game, buttonMachinegun,
                string.Format("A tower that fires bullets\n\nDamage: {0}\nSpeed: {1:0.0}\nRange: {2:0}\nCost: {3}",
                machinegunTowerData.Damage, SpeedToAPS(machinegunTowerData.Speed), machinegunTowerData.Range, machinegunTowerData.Cost), TheGame.GetFont(Font.Small)) { TextColor = Color.DarkOrange };

            var rocketTowerData = new RocketTower(Game, null).GetStats();
            Tooltip rocketTooltip = new Tooltip(Game, buttonRocket,
                string.Format("A tower that fires rockets\nwhich deals AOE damage\n\nDamage: {0}\nSpeed: {1:0.0}\nRange: {2:0}\nCost: {3}",
                rocketTowerData.Damage, SpeedToAPS(rocketTowerData.Speed), rocketTowerData.Range, rocketTowerData.Cost), TheGame.GetFont(Font.Small)) { TextColor = Color.DarkOrange };

            Tooltip slowTooltip = new Tooltip(Game, buttonSlow, "A tower that fires rays of\ncoldness, slowing enemies\nin an area around its target\n\nDamage: 5\nSpeed: 0.5\nRange: 100\nSlow: 25%\nCost: 150", TheGame.GetFont(Font.Small)) { TextColor = Color.DarkOrange };
            Tooltip flameTooltip = new Tooltip(Game, buttonFlame, "A tower that sprays out a whirl\nof flames, dealing rapid damage to\nthe enemies it hits\n\nDamage: 2\nSpeed: 0.1\nRange: 100\nCost: 100", TheGame.GetFont(Font.Small)) { TextColor = Color.DarkOrange };

            buttonUpgrade = new TextButton(Game, new Vector2(650, 200), "Upgrade!", TheGame.GetFont(Font.Large));
            buttonUpgrade.DropShadow = true;
            buttonUpgrade.Visible = false;

            buttonSell = new TextButton(Game, new Vector2(650, 230), "Sell", TheGame.GetFont(Font.Large));
            buttonSell.DropShadow = true;
            buttonSell.Visible = false;
            sellTooltip = new Tooltip(Game, buttonSell, string.Empty, TheGame.GetFont(Font.Small));

            fpsLabel = new FPSLabel(Game, new Vector2(640, 575), TheGame.GetFont(Font.Large));
            timeLabel = new Label(Game, new Vector2(717, 575), String.Empty, TheGame.GetFont(Font.Large));
#if !DEBUG
            fpsLabel.Visible = false;
            timeLabel.Visible = false;
#endif
            moneyLabel = new Label(Game, new Vector2(650, 5), "Cash: $" + Player.Money, TheGame.GetFont(Font.Large));
            moneyLabel.Color = Color.Yellow;
            moneyLabel.DropShadow = true;
            livesLabel = new Label(Game, new Vector2(650, 25), "Lives: " + Player.Lives, TheGame.GetFont(Font.Large));
            livesLabel.DropShadow = true;
            towerInfoLabel = new Label(Game, new Vector2(650, 60), string.Empty, TheGame.GetFont(Font.Small));
            towerInfoLabel.DropShadow = true;
            
            //buttonMachinegun.Click += (o, e) => currentlyBuilding = buttonMachinegun.Toggled ? TowerType.Machinegun : TowerType.None;
            //buttonRocket.Click += (o, e) => currentlyBuilding = buttonRocket.Toggled ? TowerType.Rocket : TowerType.None;
            //buttonSlow.Click += (o, e) => currentlyBuilding = buttonSlow.Toggled ? TowerType.Slow : TowerType.None;
            //buttonFlame.Click += (o, e) => currentlyBuilding = buttonFlame.Toggled ? TowerType.Flame : TowerType.None;
            buttonMachinegun.Click += (o, e) => currentlyBuilding = () => new MachinegunTower(Game, map);
            buttonRocket.Click += (o, e) => currentlyBuilding = () => new RocketTower(Game, map);
            buttonSlow.Click += (o, e) => currentlyBuilding = () => new SlowTower(Game, map);
            buttonFlame.Click += (o, e) => currentlyBuilding = () => new FlameTower(Game, map);
            buttonBank.Click += (o, e) => currentlyBuilding = () => new Bank(Game, 5, 2000);

            buttonUpgrade.Click += (o, e) =>
                {
                    if (Player.TryTakeMoney(selectedTower.UpgradeCost()))
                    {
                        selectedTower.Upgrade();
                        radius = selectedTower.GetStats().Range;
                    }
                };
            buttonUpgrade.MouseEnter += (o, e) =>
                {
                    Tower.TowerData current = selectedTower.GetStats();
                    Tower.TowerData upgraded = selectedTower.GetNextUpgradeStats();

                    if (upgraded.Range - current.Range > 0.0f)
                    {
                        upgradedRadius = upgraded.Range;
                    }
                };
            buttonUpgrade.MouseLeave += (o, e) =>
                {
                    upgradedRadius = 0f;
                };
            buttonSell.Click += (o, e) =>
                {
                    map.RemoveTower(selectedTower.Row, selectedTower.Col);
                    Player.AddMoney((int)(0.75 * selectedTower.Cost));
                    selectedTower = null;
                    RemoveComponent(selectionCircle);
                    selectionCircle = null;
                };

            xpBar = new ProgressBar(Game, new Rectangle(0, 480, 640, 8), Color.Black, Color.Orange);
            Player.XpGained += (o, e) => xpBar.Percentage += 3;
            Tooltip xpTooltip = new Tooltip(Game, xpBar, "XP: 500 / 25000", TheGame.GetFont(Font.Small));

            Player.MoneyChanged += (o, e) => moneyLabel.Text = "Cash: $" + Player.Money;
            Player.LifeLost += (o, e) => livesLabel.Text = "Lives: " + Player.Lives;

            waveTimeline = new Timeline(Game, new Rectangle(10, 560, 500, 30), 15.0f, TheGame.GetFont(Font.Small));
            waveTimeline.Add(20.0f, 30.0f, "1: Regular");
            waveTimeline.Add(35.0f, 45.0f, "2: Fast");
            waveTimeline.Add(50.0f, 60.0f, "3: Flying");

            TextButton buttonNextWave = new TextButton(Game, new Vector2(520, 565), "Next wave!", TheGame.GetFont(Font.Large));
            buttonNextWave.DropShadow = true;
            buttonNextWave.Click += (o, e) => waveTimeline.JumpToNextItem();

            SkillButton skillButton = new SkillButton(Game, new Vector2(100, 100), 
                Game.Content.Load<Texture2D>("SkillTest"), TheGame.GetFont(Font.Small), 2, 5);
            Tooltip skillTooltip = new Tooltip(Game, skillButton, "Increases your damage by an additional 5% per point", TheGame.GetFont(Font.Small));

            SkillTree tree = new SkillTree(Game, new Vector2(5, 5));
            tree.MaxPoints = 10;
            tree.AddSkill("Roast Beef", new SkillButton(Game, new Vector2(0, 0), Game.Content.Load<Texture2D>("SkillTest"), TheGame.GetFont(Font.Small), 0, 5), 0);
            tree.AddSkill("Cheese", new SkillButton(Game, new Vector2(0, 0), Game.Content.Load<Texture2D>("SkillTest"), TheGame.GetFont(Font.Small), 0, 4), 1);
            tree.AddSkill("Pie", new SkillButton(Game, new Vector2(0, 0), Game.Content.Load<Texture2D>("SkillTest"), TheGame.GetFont(Font.Small), 0, 3), 2);
            tree.AddSkill("Flameburst", new SkillButton(Game, new Vector2(0, 0), Game.Content.Load<Texture2D>("SkillTest"), TheGame.GetFont(Font.Small), 0, 1), 2);

            AddComponent(timeLabel);
            AddComponent(fpsLabel);
            AddComponent(moneyLabel);
            AddComponent(livesLabel);
            AddComponent(towerInfoLabel);
            AddComponent(towersLabel);
            AddComponent(buttonMachinegun);
            AddComponent(buttonRocket);
            AddComponent(buttonSlow);
            AddComponent(buttonFlame);
            AddComponent(buttonBank);
            AddComponent(buttonUpgrade);
            AddComponent(buttonSell);
            AddComponent(xpBar);
            AddComponent(xpTooltip);
            AddComponent(machinegunTooltip);
            AddComponent(rocketTooltip);
            AddComponent(slowTooltip);
            AddComponent(flameTooltip);
            AddComponent(sellTooltip);
            AddComponent(waveTimeline);
            AddComponent(buttonNextWave);
            //AddComponent(skillButton);
            //AddComponent(skillTooltip);
            AddComponent(tree);
        }
    }
}
