﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Drawing;
using System.Windows.Forms;

using Debugger;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Services
{
	internal sealed partial class DebuggeeExceptionForm
	{
		Process process;
		
		DebuggeeExceptionForm(Process process)
		{
			InitializeComponent();
			
			this.process = process;
			
			this.process.Exited += ProcessHandler;
			this.process.Resumed += ProcessHandler;
			
			this.FormClosed += FormClosedHandler;
			
			this.WindowState = DebuggingOptions.Instance.DebuggeeExceptionWindowState;
			FormLocationHelper.Apply(this, "DebuggeeExceptionForm", true);
			
			this.MinimizeBox = this.MaximizeBox = this.ShowIcon = false;
			
			this.exceptionView.Font = WinFormsResourceService.DefaultMonospacedFont;
			this.exceptionView.DoubleClick += ExceptionViewDoubleClick;
			this.exceptionView.WordWrap = false;
			
			this.btnBreak.Text = StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Break}");
			this.btnStop.Text  = StringParser.Parse("${res:MainWindow.Windows.Debug.ExceptionForm.Terminate}");
			
			this.btnBreak.Image = WinFormsResourceService.GetBitmap("Icons.16x16.Debug.Break");
			this.btnStop.Image = WinFormsResourceService.GetBitmap("Icons.16x16.StopProcess");
		}

		void ProcessHandler(object sender, EventArgs e)
		{
			this.Close();
		}
		
		void FormClosedHandler(object sender, EventArgs e)
		{
			this.process.Exited -= ProcessHandler;
			this.process.Resumed -= ProcessHandler;
		}
		
		public static void Show(Process process, string title, string message, string stacktrace, Bitmap icon)
		{
			DebuggeeExceptionForm form = new DebuggeeExceptionForm(process);
			form.Text = title;
			form.pictureBox.Image = icon;
			form.lblExceptionText.Text = message;
			form.exceptionView.Text = stacktrace;
			
			form.Show(WorkbenchSingleton.Instance.MainWin32Window);
		}
		
		void ExceptionViewDoubleClick(object sender, EventArgs e)
		{
			string fullText = exceptionView.Text;
			// Any text?
			if (fullText.Length > 0) {
				//int line = textEditorControl.ActiveTextAreaControl.Caret.Line;
				//string textLine = TextUtilities.GetLineAsString(textEditorControl.Document, line);
				Point clickPos = exceptionView.PointToClient(Control.MousePosition);
				int index = exceptionView.GetCharIndexFromPosition(clickPos);
				int start = index;
				// find start of current line
				while (--start > 0 && fullText[start - 1] != '\n');
				// find end of current line
				while (++index < fullText.Length && fullText[index] != '\n');
				
				string textLine = fullText.Substring(start, index - start);
				
				FileLineReference lineReference = OutputTextLineParser.GetFileLineReference(textLine);
				if (lineReference != null) {
					// Open matching file.
					FileService.JumpToFilePosition(lineReference.FileName, lineReference.Line, lineReference.Column);
				}
			}
		}
		
		void FormResize(object sender, EventArgs e)
		{
			DebuggingOptions.Instance.DebuggeeExceptionWindowState = WindowState;
		}
		
		void BtnBreakClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void BtnStopClick(object sender, EventArgs e)
		{
			this.process.Terminate();
			Close();
		}
	}
}
