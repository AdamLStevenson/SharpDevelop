﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.Profiler.AddIn.Dialogs;
using ICSharpCode.Profiler.AddIn.OptionPanels;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Profiler.AddIn
{
	/// <summary>
	/// Description of ProfilerRunner.
	/// </summary>
	public class ProfilerRunner
	{
		public event EventHandler RunFinished;
		ProfilerControlWindow controlWindow;
		
		protected virtual void OnRunFinished(EventArgs e)
		{
			if (RunFinished != null) {
				RunFinished(this, e);
			}
		}
		
		Controller.Profiler profiler;
		IProfilingDataWriter writer;
		TempFileDatabase database;
		
		public ICSharpCode.Profiler.Controller.Profiler Profiler {
			get { return profiler; }
		}
		
		/// <summary>
		/// Creates a new ProfilerRunner using a ProcessStartInfo and a data writer.
		/// </summary>
		public ProfilerRunner(ProcessStartInfo startInfo, bool useTempFileDatabase, IProfilingDataWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			if (startInfo == null)
				throw new ArgumentNullException("startInfo");
			
			if (useTempFileDatabase) {
				this.database = new TempFileDatabase();
				this.writer = writer;
				this.profiler = new Controller.Profiler(startInfo, this.database.GetWriter(), OptionWrapper.CreateProfilerOptions());
			} else {
				this.database = null;
				this.writer = writer;
				this.profiler = new Controller.Profiler(startInfo, writer, OptionWrapper.CreateProfilerOptions());
			}
			
			PrintProfilerOptions();
			this.profiler.RegisterFailed += delegate { MessageService.ShowError("${res:AddIns.Profiler.Messages.RegisterFailed}"); };
			this.profiler.DeregisterFailed += delegate { MessageService.ShowError("${res:AddIns.Profiler.Messages.UnregisterFailed}"); };
			this.profiler.OutputUpdated += delegate { SetOutputText(profiler.ProfilerOutput); };
			this.profiler.SessionEnded += delegate { FinishSession(); };
		}
		
		void PrintProfilerOptions()
		{
			var options = OptionWrapper.CreateProfilerOptions();
			LoggingService.Info("Profiler settings:");
			LoggingService.Info("Shared memory size: " + options.SharedMemorySize + " (" + (options.SharedMemorySize / 1024 / 1024) + " MB)");
			LoggingService.Info("Combine recursive calls: " + options.CombineRecursiveFunction);
			LoggingService.Info("Enable DC: " + options.EnableDC);
			LoggingService.Info("Profile .NET internals: " + (!options.DoNotProfileDotNetInternals));
			LoggingService.Info("Track events: " + options.TrackEvents);
		}
		
		void FinishSession()
		{
			try {
				using (AsynchronousWaitDialog dlg = AsynchronousWaitDialog.ShowWaitDialog(StringParser.Parse("${res:AddIns.Profiler.Messages.PreparingForAnalysis}"), true)) {
					profiler.Dispose();

					WorkbenchSingleton.SafeThreadAsyncCall(() => { controlWindow.AllowClose = true; this.controlWindow.Close(); });
					if (database != null) {
						database.WriteTo(writer, progress => {
						                 	dlg.Progress = progress;
						                 	return !dlg.CancellationToken.IsCancellationRequested;
						                 });
						writer.Close();
						database.Close();
					} else {
						writer.Close();
					}
					
					if (!dlg.CancellationToken.IsCancellationRequested)
						OnRunFinished(EventArgs.Empty);
				}
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		public void Run()
		{
			WorkbenchSingleton.Instance.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
			this.controlWindow = new ProfilerControlWindow(this);
			profiler.Start();
			this.controlWindow.Show();
		}
		
		public void Stop()
		{
			profiler.Stop();
		}
		
		public static ProfilerRunner CreateRunner(IProfilingDataWriter writer)
		{
			AbstractProject currentProj = ProjectService.CurrentProject as AbstractProject;
			
			if (currentProj == null)
				return null;
			
			if (!currentProj.IsStartable) {
				if (MessageService.AskQuestion("${res:AddIns.Profiler.Messages.NoStartableProjectWantToProfileStartupProject}")) {
					currentProj = ProjectService.OpenSolution.StartupProject as AbstractProject;
					if (currentProj == null) {
						MessageService.ShowError("${res:AddIns.Profiler.Messages.NoStartableProjectFound}");
						return null;
					}
				} else
					return null;
			}
			if (!File.Exists(currentProj.OutputAssemblyFullPath)) {
				MessageService.ShowError("${res:AddIns.Profiler.Messages.FileNotFound}");
				return null;
			}
			
			ProcessStartInfo startInfo;
			try {
				startInfo = currentProj.CreateStartInfo();
			} catch (ProjectStartException ex) {
				MessageService.ShowError(ex.Message);
				return null;
			}
			ProfilerRunner runner = new ProfilerRunner(startInfo, true, writer);
			return runner;
		}
		
		#region MessageView Management
		static MessageViewCategory profileCategory = null;
		
		static void EnsureProfileCategory()
		{
			if (profileCategory == null) {
				MessageViewCategory.Create(ref profileCategory, "Profile", StringParser.Parse("${res:AddIns.Profiler.MessageViewCategory}"));
			}
		}
		
		public static void SetOutputText(string text)
		{
			EnsureProfileCategory();
			profileCategory.SetText(StringParser.Parse(text));
		}
		
		public static void AppendOutputText(string text)
		{
			EnsureProfileCategory();
			profileCategory.AppendText(StringParser.Parse(text));
		}
		
		public static void AppendOutputLine(string text)
		{
			EnsureProfileCategory();
			profileCategory.AppendLine(StringParser.Parse(text));
		}
		#endregion
	}
}
