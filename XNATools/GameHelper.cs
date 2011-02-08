using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNATools
{
    public static class GameHelper
    {
        public static Game Game { get; set; }

        public static void AddService<T>(T obj)
        {
            Game.Services.AddService(typeof(T), obj);
        }

        public static T GetService<T>()
        {
            return (T)Game.Services.GetService(typeof(T));
        }        
    }
}
