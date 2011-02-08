using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TD
{
    class MapClickArgs : EventArgs
    {
        public int Row { get; private set; }
        public int Col { get; private set; }

        public MapClickArgs(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
}
