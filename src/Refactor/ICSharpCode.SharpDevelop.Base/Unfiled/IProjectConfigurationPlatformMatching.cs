// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
    public interface IProjectConfigurationPlatformMatching
	{
        IProject Project
        {
            get;
        }

        string Configuration
        {
            get;
            set;
        }
        string Platform
        {
            get;
            set;
        }
        ISolutionItem SolutionItem
        {
            get;
            set;
        }


        void SetSolutionConfigurationPlatform(IProjectSection section, string newConfiguration, string newPlatform);

        void SetProjectConfigurationPlatform(IProjectSection section, string newConfiguration, string newPlatform);
			
		
	}
}
