using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TD
{
    interface ITarget
    {
        Vector2 Center { get; }
        bool DoDamage(int damage);
        event EventHandler Died;
    }
}
