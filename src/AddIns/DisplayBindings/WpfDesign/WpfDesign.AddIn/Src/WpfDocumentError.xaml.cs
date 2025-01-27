﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// A friendly error window displayed when the WPF document does not parse correctly.
	/// </summary>
	public partial class WpfDocumentError : UserControl
	{
		public WpfDocumentError()
		{
			InitializeComponent();
		}
		
		void ViewXaml(object sender,RoutedEventArgs e)
		{
			WorkbenchSingleton.Instance.Workbench.ActiveWorkbenchWindow.SwitchView(0);
		}
	}
}
