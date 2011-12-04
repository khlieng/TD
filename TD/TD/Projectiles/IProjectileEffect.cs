using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TD
{
    interface IProjectileEffect
    {
        bool Finished { get; }
        void Apply(ITarget target);
        void Update(GameTime time);
    }
}
