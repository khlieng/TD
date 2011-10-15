using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNATools.UI
{
    public interface ITooltipProvider
    {
        event EventHandler ShowTooltip;
        event EventHandler HideTooltip;
        //Vector2 TooltipPosition { get; }
    }
}
