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
    }
}
