using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TD
{
    public interface IToggleAble
    {
        bool Toggled { get; set; }
        event EventHandler ToggledChanged;
    }
}
