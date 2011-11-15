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
	public interface ISolution : ISolutionFolderContainer, IDisposable, IBuildable
	{
        void BeforeAddFolderToSolution(ISolutionFolder folder);

        void AfterAddFolderToSolution(ISolutionFolder folder);

        List<IProjectConfigurationPlatformMatching> GetActiveConfigurationsAndPlatformsForProjects(string solutionConfiguration, string solutionPlatform);
		
		
		[BrowsableAttribute(false)]
		Microsoft.Build.Evaluation.ProjectCollection MSBuildProjectCollection { get; }
		
		#region Enumerate projects/folders
        IProject FindProjectContainingFile(string fileName);

        [Browsable(false)]
        IEnumerable<IProject> Projects
        {
            get;
        }

        [Browsable(false)]
        IEnumerable<ISolutionFolderContainer> SolutionFolderContainers
        {
            get;
        }

        [Browsable(false)]
        IEnumerable<ISolutionFolder> SolutionFolders
        {
            get;
        }
		
		/// <summary>
		/// Returns the startup project. If no startup project is set in the solution preferences,
		/// returns any project that is startable.
		/// </summary>
        [Browsable(false)]
        IProject StartupProject
        {
            get;
        }

        ISolutionFolder GetSolutionFolder(string guid);

        ISolutionFolderContainer CreateFolder(string folderName);

		#endregion
		
		#region Properties
        [Browsable(false)]
        bool HasProjects
        {
            get;
        }

        [Browsable(false)]
        string FileName
        {
            get;
            set;
        }

        [Browsable(false)]
        string Directory
        {
            get;
        }



        [Browsable(false)]
        ISolutionPreferences Preferences
        {
            get;
        }
		
		/// <summary>Returns true if the solution is readonly.</summary>
        [Browsable(false)]
        bool ReadOnly
        {
            get;
        }

		#endregion
		
		#region ISolutionFolderContainer implementations
        [Browsable(false)]
        new ISolution ParentSolution
        {
            get;
        }

        IProjectSection SolutionItems
        {
            get;
        }

        void AddFolder(ISolutionFolder folder);
		
		#endregion
		
		#region Save
        void Save();

        void Save(string fileName);

		#endregion
		
        #region Configuration/Platform management

        #region Section management

        IProjectSection GetSolutionConfigurationsSection();

        IProjectSection GetProjectConfigurationsSection();

        ISolutionItem GetProjectConfiguration(string guid);

        bool FixSolutionConfiguration(IEnumerable<IProject> projects);

		#endregion
		
		#region GetProjectConfigurationsSection/GetPlatformNames

        IList<string> GetConfigurationNames();

        IList<string> GetPlatformNames();

		#endregion
		
		#region Solution - project configuration matching

        void ApplySolutionConfigurationAndPlatformToProjects();
		
		#endregion
		
		#region Rename Solution Configuration/Platform

        void RenameSolutionConfiguration(string oldName, string newName);

        void RenameSolutionPlatform(string oldName, string newName);

		#endregion
		
		#region Rename Project Configuration/Platform
        bool RenameProjectConfiguration(IProject project, string oldName, string newName);

        bool RenameProjectPlatform(IProject project, string oldName, string newName);
		#endregion
		
		#region Add Solution Configuration/Platform
		/// <summary>
		/// Creates a new solution configuration.
		/// </summary>
		/// <param name="newName">Name of the new configuration</param>
		/// <param name="copyFrom">Name of existing configuration to copy values from, or null</param>
		/// <param name="createInProjects">true to also create the new configuration in all projects</param>
        void AddSolutionConfiguration(string newName, string copyFrom, bool createInProjects);

        void AddSolutionPlatform(string newName, string copyFrom, bool createInProjects);
		
		#endregion
		
		#region Remove Solution Configuration/Platform

        void RemoveSolutionConfiguration(string name);

        void RemoveSolutionPlatform(string name);

		#endregion
		
		#region Remove Project Configuration/Platform

        bool RemoveProjectConfiguration(IProject project, string name);

        bool RemoveProjectPlatform(IProject project, string name);

		#endregion
        #endregion

        #region Load

        #endregion


        
    }
}