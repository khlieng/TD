using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNATools
{
    public static class GameExtensions
    {
        public static void AddService<T>(this Game game, T service)
        {
            game.Services.AddService(typeof(T), service);
        }

        public static T GetService<T>(this Game game)
        {
            return (T)game.Services.GetService(typeof(T));
        }
    }
}
