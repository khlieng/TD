using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNATools.UI
{
    public interface IToggleAble
    {
        bool Toggled { get; set; }
        event EventHandler ToggledChanged;
    }
}
