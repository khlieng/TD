using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TD
{
    interface IProjectileEffect
    {
        void Apply(ITarget target);
    }
}
