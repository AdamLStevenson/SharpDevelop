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
    public sealed class ProjectConfigurationPlatformMatching : IProjectConfigurationPlatformMatching
	{
		public readonly IProject project;

        public IProject Project
        {
            get
            {
                return project;
            }
        }

        public string Configuration
        {
            get;
            set;
        }
        public string Platform
        {
            get;
            set;
        }
        public ISolutionItem SolutionItem
        {
            get;
            set;
        }
			
		public ProjectConfigurationPlatformMatching(IProject pProject, string configuration, string platform, SolutionItem solutionItem)
		{
            project = pProject;
			this.Configuration = configuration;
			this.Platform = platform;
			this.SolutionItem = solutionItem;
		}
			
		public void SetSolutionConfigurationPlatform(IProjectSection section, string newConfiguration, string newPlatform)
		{
			if (this.SolutionItem == null)
				return;
			string oldName = this.SolutionItem.Name;
			this.SolutionItem.Name = this.Project.IdGuid + "." + newConfiguration + "|" + newPlatform + ".Build.0";
			string newName = this.SolutionItem.Name;
			if (StripBuild0(ref oldName) && StripBuild0(ref newName)) {
				oldName += ".ActiveCfg";
				newName += ".ActiveCfg";
				foreach (SolutionItem item in section.Items) {
					if (item.Name == oldName)
						item.Name = newName;
				}
			}
		}
			
		public void SetProjectConfigurationPlatform(IProjectSection section, string newConfiguration, string newPlatform)
		{
			this.Configuration = newConfiguration;
			this.Platform = newPlatform;
			if (this.SolutionItem == null)
				return;
			this.SolutionItem.Location = newConfiguration + "|" + newPlatform;
			string thisName = this.SolutionItem.Name;
			if (StripBuild0(ref thisName)) {
				thisName += ".ActiveCfg";
				foreach (SolutionItem item in section.Items) {
					if (item.Name == thisName)
						item.Location = this.SolutionItem.Location;
				}
			}
		}
			
		internal static bool StripBuild0(ref string s)
		{
			if (s.EndsWith(".Build.0")) {
				s = s.Substring(0, s.Length - ".Build.0".Length);
				return true;
			} else {
				return false;
			}
		}
	}
}
