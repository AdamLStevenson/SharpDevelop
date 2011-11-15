// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
    public interface ISolutionPreferences : IMementoCapable
    {
        Properties Properties
        {
            get;
        }

        IProject StartupProject
        {
            get;
            set;
        }

        

        event EventHandler StartupProjectChanged;

        string ActiveConfiguration
        {
            get;
            set;
        }

        string ActivePlatform
        {
            get;
            set;
        }
    }
}
