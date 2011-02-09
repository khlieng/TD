using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TD
{
    class DeathEventArgs : EventArgs
    {
        public CauseOfDeath Cause { get; private set; }

        public DeathEventArgs(CauseOfDeath cause)
        {
            Cause = cause;
        }
    }
}
