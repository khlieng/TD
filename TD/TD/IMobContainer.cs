using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TD
{
    interface IMobContainer
    {
        Vector2 SpawnPoint { get; }
        IEnumerable<ITarget> Mobs { get; }
        void AddMob(Mob mob);
        void RemoveMob(Mob mob);
    }
}
