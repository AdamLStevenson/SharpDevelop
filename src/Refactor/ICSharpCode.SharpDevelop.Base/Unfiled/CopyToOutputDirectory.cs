using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;


namespace ICSharpCode.SharpDevelop.Project
{
    public enum CopyToOutputDirectory
    {
        Never,
        Always,
        PreserveNewest
    }
}
