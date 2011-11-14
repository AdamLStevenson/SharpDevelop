// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class SelectNextWindow : AbstractMenuCommand
	{
		public override void Run()
		{
			if (WorkbenchSingleton.Instance.Workbench.ActiveWorkbenchWindow == null ||
			   		WorkbenchSingleton.Instance.Workbench.WorkbenchWindowCollection.Count == 0) {
				return;
			}
			int index = WorkbenchSingleton.Instance.Workbench.WorkbenchWindowCollection.IndexOf(WorkbenchSingleton.Instance.Workbench.ActiveWorkbenchWindow);
			WorkbenchSingleton.Instance.Workbench.WorkbenchWindowCollection[(index + 1) % WorkbenchSingleton.Instance.Workbench.WorkbenchWindowCollection.Count].SelectWindow();
		}
	}
	
	public class SelectPrevWindow : AbstractMenuCommand
	{
		public override void Run()
		{
			if (WorkbenchSingleton.Instance.Workbench.ActiveWorkbenchWindow == null ||
					WorkbenchSingleton.Instance.Workbench.WorkbenchWindowCollection.Count == 0) {
				return;
			}
			int index = WorkbenchSingleton.Instance.Workbench.WorkbenchWindowCollection.IndexOf(WorkbenchSingleton.Instance.Workbench.ActiveWorkbenchWindow);
			WorkbenchSingleton.Instance.Workbench.WorkbenchWindowCollection[(index + WorkbenchSingleton.Instance.Workbench.WorkbenchWindowCollection.Count - 1) % WorkbenchSingleton.Instance.Workbench.WorkbenchWindowCollection.Count].SelectWindow();
		}
	}
	
	public class CloseAllWindows : AbstractMenuCommand
	{
		public override void Run()
		{
			WorkbenchSingleton.Instance.Workbench.CloseAllViews();
		}
	}
	
}
