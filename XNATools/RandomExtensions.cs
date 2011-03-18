using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNATools
{
    public static class RandomExtensions
    {
        public static int Between(this Random rand, int min, int max)
        {
            return rand.Next(max - min + 1) + min;
        }

        public static float Between(this Random rand, float min, float max)
        {
            return rand.Next((int)(max * 1000.0f) - (int)(min * 1000.0f) + 1) / 1000.0f + min;
        }
    }
}
