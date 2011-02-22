using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XNATools
{
    public static class GameHelper
    {
        public static Game Game { get; set; }
        public static ContentManager Content
        {
            get { return Game.Content; }
        }

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
