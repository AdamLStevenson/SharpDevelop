// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// IBuildable implementation that builds several projects.
	/// </summary>
	public class MultipleProjectBuildable : IBuildable
	{
		readonly IBuildable[] projects;
		
		public MultipleProjectBuildable(IEnumerable<IBuildable> projects)
		{
			this.projects = projects.ToArray();
		}
		
		public string Name {
			get { return string.Empty; }
		}
		
		public ISolution ParentSolution {
			get { return projects.Length > 0 ? projects[0].ParentSolution : null; }
		}
		
		public ICollection<IBuildable> GetBuildDependencies(IProjectBuildOptions buildOptions)
		{
			return projects;
		}
		
		public void StartBuild(IProjectBuildOptions buildOptions, IBuildFeedbackSink feedbackSink)
		{
			// SharpDevelop already has built our dependencies, so we're done immediately.
			feedbackSink.Done(true);
		}

        public IProjectBuildOptions CreateProjectBuildOptions(ICSharpCode.SharpDevelop.Project.IBuildOptions options, bool isRootBuildable)
		{
			return null;
		}
	}
}
