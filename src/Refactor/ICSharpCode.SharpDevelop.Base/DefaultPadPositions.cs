using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.SharpDevelop.Gui
{
    /// <summary>
    /// Indicates the default position for a pad.
    /// This is a bit-flag enum, Hidden can be combined with the directions.
    /// </summary>
    [Flags]
    public enum DefaultPadPositions
    {
        None = 0,
        Right = 1,
        Left = 2,
        Bottom = 4,
        Top = 8,
        Hidden = 16
    }
    
}
