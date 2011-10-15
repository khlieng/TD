using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TD
{
    static class Player
    {
        private static int lives;
        public static int Lives
        {
            get { return lives; }
        }

        private static int money;
        public static int Money
        {
            get { return money; }
        }

        public static event EventHandler LifeLost;
        public static event EventHandler MoneyChanged;
        public static event EventHandler XpGained;

        public static void Init(int lives, int money)
        {
            Player.lives = lives;
            Player.money = money;
        }

        public static void LoseLife()
        {
            lives--;
            OnLifeLost();
        }

        public static void AddMoney(int amount)
        {
            money += amount;
            OnMoneyChanged();
        }

        public static bool TryTakeMoney(int amount)
        {
            if (money >= amount)
            {
                money -= amount;
                OnMoneyChanged();
                return true;
            }
            return false;
        }

        public static void AddXp()
        {
            OnXpGained();
        }

        private static void OnLifeLost()
        {
            if (LifeLost != null)
            {
                LifeLost(null, EventArgs.Empty);
            }
        }

        private static void OnMoneyChanged()
        {
            if (MoneyChanged != null)
            {
                MoneyChanged(null, EventArgs.Empty);
            }
        }

        private static void OnXpGained()
        {
            if (XpGained != null)
            {
                XpGained(null, EventArgs.Empty);
            }
        }
    }
}
