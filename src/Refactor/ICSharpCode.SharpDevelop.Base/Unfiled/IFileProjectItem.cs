// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
    public interface IFileProjectItem : IProjectItem
    {
        string BuildAction
        {
            get;
            set;
        }

        CopyToOutputDirectory CopyToOutputDirectory
        {
            get;
            set;
        }

        string CustomTool
        {
            get;
            set;
        }

        [Browsable(false)]
        string DependentUpon
        {
            get;
            set;
        }

        [Browsable(false)]
        string SubType
        {
            get;
            set;
        }

        [Browsable(false)]
        bool IsLink
        {
            get;
        }

        /// <summary>
        /// Gets the name of the file in the virtual project file system.
        /// This is normally the same as Include, except for linked files, where it is
        /// the value of Properties["Link"].
        /// </summary>
        [Browsable(false)]
        string VirtualName
        {
            get;
        }
    }
}
