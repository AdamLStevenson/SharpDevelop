﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.VBNet;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.VBNetBinding
{
	public class VBNetProject : CompilableProject, IVBNetOptionProvider
	{
		protected override void OnPropertyChanged(ProjectPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.PropertyName == "OutputType") {
				switch (this.OutputType) {
					case OutputType.WinExe:
						SetProperty(e.Configuration, e.Platform,
						            "MyType", "WindowsForms", e.NewLocation, true);
						break;
					case OutputType.Exe:
						SetProperty(e.Configuration, e.Platform,
						            "MyType", "Console", e.NewLocation, true);
						break;
					default:
						SetProperty(e.Configuration, e.Platform,
						            "MyType", "Windows", e.NewLocation, true);
						break;
				}
			}
		}
		
		public override IAmbience GetAmbience()
		{
			return new VBNetAmbience();
		}
		
		public VBNetProject(ProjectLoadInformation info)
			: base(info)
		{
			InitVB();
		}
		
		public const string DefaultTargetsFile = @"$(MSBuildBinPath)\Microsoft.VisualBasic.Targets";
		public const string ExtendedTargetsFile = @"$(SharpDevelopBinPath)\SharpDevelop.Build.VisualBasic.targets";
		
		public VBNetProject(ProjectCreateInformation info)
			: base(info)
		{
			InitVB();
			
			this.AddImport(DefaultTargetsFile, null);
			
			SetProperty("Debug", null, "DefineConstants", "DEBUG=1,TRACE=1",
			            PropertyStorageLocations.ConfigurationSpecific, true);
			SetProperty("Release", null, "DefineConstants", "TRACE=1",
			            PropertyStorageLocations.ConfigurationSpecific, true);
		}
		
		protected override IParseProjectContent CreateProjectContent()
		{
			IParseProjectContent pc = base.CreateProjectContent();
			MyNamespaceBuilder.BuildNamespace(this, pc);
			return pc;
		}
		
		public override IEnumerable<ReferenceProjectItem> ResolveAssemblyReferences(CancellationToken cancellationToken)
		{
			ReferenceProjectItem[] additionalItems = {
				new ReferenceProjectItem(this, "mscorlib"),
				new ReferenceProjectItem(this, "Microsoft.VisualBasic"),
			};
			return MSBuildInternals.ResolveAssemblyReferences(this, additionalItems);
		}
		
		void InitVB()
		{
			reparseReferencesSensitiveProperties.Add("TargetFrameworkVersion");
			reparseCodeSensitiveProperties.Add("DefineConstants");
		}
		
		public override string Language {
			get { return VBNetProjectBinding.LanguageName; }
		}
		
		public override LanguageProperties LanguageProperties {
			get { return LanguageProperties.VBNet; }
		}
		
		public override IItemType GetDefaultItemType(string fileName)
		{
			if (string.Equals(Path.GetExtension(fileName), ".vb", StringComparison.OrdinalIgnoreCase))
				return ItemType.Compile;
			else
				return base.GetDefaultItemType(fileName);
		}
		
		public override void StartBuild(IProjectBuildOptions options, IBuildFeedbackSink feedbackSink)
		{
			if (this.MinimumSolutionVersion == Solution.SolutionVersionVS2005) {
				MSBuildEngine.StartBuild(this,
				                         options,
				                         feedbackSink,
				                         MSBuildEngine.AdditionalTargetFiles.Concat(
				                         	new [] { Path.Combine(MSBuildEngine.SharpDevelopBinPath, "SharpDevelop.CheckMSBuild35Features.targets") }));
			} else {
				base.StartBuild(options, feedbackSink);
			}
		}
		
		/*
		protected override void AddOrRemoveExtensions()
		{
			// Test if SharpDevelop-Build extensions are required
			bool needExtensions = false;
			
			foreach (var p in GetAllProperties("TargetFrameworkVersion")) {
				if (p.IsImported == false) {
					if (p.Value.StartsWith("CF")) {
						needExtensions = true;
					}
				}
			}
			
			foreach (Microsoft.Build.BuildEngine.Import import in MSBuildProject.Imports) {
				if (needExtensions) {
					if (DefaultTargetsFile.Equals(import.ProjectPath, StringComparison.OrdinalIgnoreCase)) {
						//import.ProjectPath = extendedTargets;
						MSBuildInternals.SetImportProjectPath(this, import, ExtendedTargetsFile);
						// Workaround for SD2-1490. It would be better if the project browser could refresh itself
						// when necessary.
						ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
						break;
					}
				} else {
					if (ExtendedTargetsFile.Equals(import.ProjectPath, StringComparison.OrdinalIgnoreCase)) {
						//import.ProjectPath = defaultTargets;
						MSBuildInternals.SetImportProjectPath(this, import, DefaultTargetsFile);
						// Workaround for SD2-1490. It would be better if the project browser could refresh itself
						// when necessary.
						ProjectBrowserPad.Instance.ProjectBrowserControl.RefreshView();
						break;
					}
				}
			}
		}*/
		
		public Nullable<bool> OptionInfer {
			get { return GetValue("OptionInfer", false); }
		}
		
		public Nullable<bool> OptionStrict {
			get { return GetValue("OptionStrict", false); }
		}
		
		public Nullable<bool> OptionExplicit {
			get { return GetValue("OptionExplicit", true); }
		}
		
		public Nullable<CompareKind> OptionCompare {
			get {
				string val = GetEvaluatedProperty("OptionCompare");
				
				if ("Text".Equals(val, StringComparison.OrdinalIgnoreCase))
					return CompareKind.Text;
				
				return CompareKind.Binary;
			}
		}
		
		bool? GetValue(string name, bool defaultVal)
		{
			string val;
			try {
				val = GetEvaluatedProperty(name);
			} catch (ObjectDisposedException) {
				// This can happen when the project is disposed but the resolver still tries
				// to access Option Infer (or similar).
				val = null;
			}
			
			if (val == null)
				return defaultVal;
			
			return "On".Equals(val, StringComparison.OrdinalIgnoreCase);
		}
	}
}
