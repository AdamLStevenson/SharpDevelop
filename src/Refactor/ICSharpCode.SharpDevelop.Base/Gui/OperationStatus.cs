using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.SharpDevelop.Gui
{
    /// <summary>
    /// Represents the status of a operation with progress monitor.
    /// </summary>
    public enum OperationStatus : byte
    {
        /// <summary>
        /// Everything is normal.
        /// </summary>
        Normal,
        /// <summary>
        /// There was at least one warning.
        /// </summary>
        Warning,
        /// <summary>
        /// There was at least one error.
        /// </summary>
        Error
    }
}
