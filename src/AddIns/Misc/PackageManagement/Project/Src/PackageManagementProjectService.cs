﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementProjectService : IPackageManagementProjectService
	{
		public IProject CurrentProject {
			get { return ProjectService.CurrentProject; }
		}
		
		public Solution OpenSolution {
			get { return ProjectService.OpenSolution; }
		}
		
		public void RefreshProjectBrowser()
		{
			if (WorkbenchSingleton.InvokeRequired) {
				WorkbenchSingleton.SafeThreadAsyncCall(RefreshProjectBrowser);
			} else {
				var refreshCommand = new RefreshProjectBrowser();
				refreshCommand.Run();
			}
		}
		
		public IEnumerable<IProject> GetOpenProjects()
		{
			Solution solution = OpenSolution;
			if (solution != null) {
				return solution.Projects;
			}
			return new IProject[0];
		}
		
		public void AddProjectItem(IProject project, IProjectItem item)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				Action<IProject, IProjectItem> action = AddProjectItem;
				WorkbenchSingleton.SafeThreadCall<IProject, IProjectItem>(action, project, item);
			} else {
				ProjectService.AddProjectItem(project, item);
			}
		}
		
		public void RemoveProjectItem(IProject project, IProjectItem item)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				Action<IProject, IProjectItem> action = RemoveProjectItem;
				WorkbenchSingleton.SafeThreadCall<IProject, IProjectItem>(action, project, item);
			} else {
				ProjectService.RemoveProjectItem(project, item);
			}
		}
		
		public void Save(IProject project)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				Action<IProject> action = Save;
				WorkbenchSingleton.SafeThreadCall<IProject>(action, project);
			} else {
				project.Save();
			}
		}
		
		public event ProjectEventHandler ProjectAdded {
			add { ProjectService.ProjectAdded += value; }
			remove { ProjectService.ProjectAdded -= value; }
		}
	
		public event EventHandler SolutionClosed {
			add { ProjectService.SolutionClosed += value; }
			remove { ProjectService.SolutionClosed -= value; }
		}
		
		public event EventHandler<SolutionEventArgs> SolutionLoaded {
			add { ProjectService.SolutionLoaded += value; }
			remove { ProjectService.SolutionLoaded -= value; }
		}
		
		public event SolutionFolderEventHandler SolutionFolderRemoved {
			add { ProjectService.SolutionFolderRemoved += value; }
			remove { ProjectService.SolutionFolderRemoved -= value; }
		}
	}
}
