// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public abstract class AbstractPadContent : IPadContent
	{
		/// <inheritdoc/>
		public abstract object Control {
			get;
		}
		
		/// <inheritdoc/>
		public virtual object InitiallyFocusedControl {
			get {
				return null;
			}
		}
		
		public virtual void Dispose()
		{
		}
		
		public void BringToFront()
		{
			IPadDescriptor d = this.PadDescriptor;
			if (d != null)
				d.BringPadToFront();
		}
		
		protected virtual IPadDescriptor PadDescriptor {
			get {
				if (WorkbenchSingleton.Instance.Workbench == null || WorkbenchSingleton.Instance.Workbench.WorkbenchLayout == null)
					return null;
				return WorkbenchSingleton.Instance.Workbench.GetPad(GetType());
			}
		}
	}
}
