// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace ICSharpCode.SharpDevelop.Gui
{
	
	
	
	
	/// <summary>
	/// Adapter IDomProgressMonitor -> IProgressMonitor
	/// </summary>
	public sealed class DomProgressMonitor : Dom.IDomProgressMonitor
	{
		IProgressMonitor monitor;
		
		private DomProgressMonitor(IProgressMonitor monitor)
		{
			if (monitor == null)
				throw new ArgumentNullException("monitor");
			this.monitor = monitor;
		}
		
		public static Dom.IDomProgressMonitor Wrap(IProgressMonitor monitor)
		{
			if (monitor == null)
				return null;
			else
				return new DomProgressMonitor(monitor);
		}
		
		public bool ShowingDialog {
			get { return monitor.ShowingDialog; }
			set { monitor.ShowingDialog = value; }
		}
	}
	
	/// <summary>
	/// Dummy progress monitor implementation that does not report the progress anywhere.
	/// </summary>
	public sealed class DummyProgressMonitor : IProgressMonitor
	{
		public string TaskName { get; set; }
		
		public bool ShowingDialog { get; set; }
		
		public OperationStatus Status { get; set; }
		
		public CancellationToken CancellationToken { get; set; }
		
		public double Progress { get; set; }
		
		public IProgressMonitor CreateSubTask(double workAmount)
		{
			return new DummyProgressMonitor() { CancellationToken = this.CancellationToken };
		}
		
		public void Dispose()
		{
		}
	}
}
