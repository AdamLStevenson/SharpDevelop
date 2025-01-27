﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project.Converter;
using ICSharpCode.SharpDevelop.Util;


namespace ICSharpCode.SharpDevelop.Project
{
	public enum OutputType {
		[Description("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Exe}")]
		Exe,
		[Description("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.WinExe}")]
		WinExe,
		[Description("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Library}")]
		Library,
		[Description("${res:Dialog.Options.PrjOptions.Configuration.CompileTarget.Module}")]
		Module
	}
	
	/// <summary>
	/// A compilable project based on MSBuild.
	/// </summary>
	public abstract class CompilableProject : MSBuildBasedProject, IUpgradableProject
	{
		public const string LocalHost = "http://localhost";
		
		#region Static methods
		/// <summary>
		/// Gets the file extension of the assembly created when building a project
		/// with the specified output type.
		/// Example: OutputType.Exe => ".exe"
		/// </summary>
		public static string GetExtension(OutputType outputType)
		{
			switch (outputType) {
				case OutputType.WinExe:
				case OutputType.Exe:
					return ".exe";
				case OutputType.Module:
					return ".netmodule";
				default:
					return ".dll";
			}
		}
		#endregion
		
		/// <summary>
		/// A list of project properties that cause reparsing of references when they are changed.
		/// </summary>
		protected readonly ISet<string> reparseReferencesSensitiveProperties = new SortedSet<string>();
		
		/// <summary>
		/// A list of project properties that cause reparsing of code when they are changed.
		/// </summary>
		protected readonly ISet<string> reparseCodeSensitiveProperties = new SortedSet<string>();
		
		protected CompilableProject(ICSharpCode.SharpDevelop.Internal.Templates.ProjectCreateInformation information)
			: base(information)
		{
			this.OutputType = OutputType.Exe;
			this.RootNamespace = information.RootNamespace;
			this.AssemblyName = information.ProjectName;
			
			ClientProfileTargetFramework clientProfile = information.TargetFramework as ClientProfileTargetFramework;
			if (clientProfile != null) {
				SetProperty(null, null, "TargetFrameworkVersion", clientProfile.FullFramework.Name, PropertyStorageLocations.Base, true);
				SetProperty(null, null, "TargetFrameworkProfile", "Client", PropertyStorageLocations.Base, true);
			} else if (information.TargetFramework != null) {
				SetProperty(null, null, "TargetFrameworkVersion", information.TargetFramework.Name, PropertyStorageLocations.Base, true);
			}
			
			SetProperty("Debug", null, "OutputPath", @"bin\Debug\",
			            PropertyStorageLocations.ConfigurationSpecific, true);
			SetProperty("Release", null, "OutputPath", @"bin\Release\",
			            PropertyStorageLocations.ConfigurationSpecific, true);
			InvalidateConfigurationPlatformNames();
			
			SetProperty("Debug", null, "DebugSymbols", "True",
			            PropertyStorageLocations.ConfigurationSpecific, true);
			SetProperty("Release", null, "DebugSymbols", "False",
			            PropertyStorageLocations.ConfigurationSpecific, true);
			
			SetProperty("Debug", null, "DebugType", "Full",
			            PropertyStorageLocations.ConfigurationSpecific, true);
			SetProperty("Release", null, "DebugType", "None",
			            PropertyStorageLocations.ConfigurationSpecific, true);
			
			SetProperty("Debug", null, "Optimize", "False",
			            PropertyStorageLocations.ConfigurationSpecific, true);
			SetProperty("Release", null, "Optimize", "True",
			            PropertyStorageLocations.ConfigurationSpecific, true);
		}
		
		protected CompilableProject(ProjectLoadInformation information)
			: base(information)
		{
		}
		
		/// <summary>
		/// Gets the path where temporary files are written to during compilation.
		/// </summary>
		[Browsable(false)]
		public string IntermediateOutputFullPath {
			get {
				string outputPath = GetEvaluatedProperty("IntermediateOutputPath");
				if (string.IsNullOrEmpty(outputPath)) {
					outputPath = GetEvaluatedProperty("BaseIntermediateOutputPath");
					if (string.IsNullOrEmpty(outputPath)) {
						outputPath = "obj";
					}
					outputPath = Path.Combine(outputPath, this.ActiveConfiguration);
				}
				return Path.Combine(Directory, outputPath);
			}
		}
		
		/// <summary>
		/// Gets the full path to the xml documentation file generated by the project, or
		/// <c>null</c> if no xml documentation is being generated.
		/// </summary>
		[Browsable(false)]
		public string DocumentationFileFullPath {
			get {
				string file = GetEvaluatedProperty("DocumentationFile");
				if (string.IsNullOrEmpty(file))
					return null;
				return Path.Combine(Directory, file);
			}
		}
		
		// Make Language abstract again to ensure backend-binding implementers don't forget
		// to set it.
		public abstract override string Language {
			get;
		}
		
		public abstract override ICSharpCode.SharpDevelop.Dom.LanguageProperties LanguageProperties {
			get;
		}
		
		[Browsable(false)]
		public string TargetFrameworkVersion {
			get { return GetEvaluatedProperty("TargetFrameworkVersion") ?? "v2.0"; }
			set { SetProperty("TargetFrameworkVersion", value); }
		}
		
		[Browsable(false)]
		public string TargetFrameworkProfile {
			get { return GetEvaluatedProperty("TargetFrameworkProfile"); }
			set { SetProperty("TargetFrameworkProfile", value); }
		}
		
		public override string AssemblyName {
			get { return GetEvaluatedProperty("AssemblyName") ?? Name; }
			set { SetProperty("AssemblyName", value); }
		}
		
		public override string RootNamespace {
			get { return GetEvaluatedProperty("RootNamespace") ?? ""; }
			set { SetProperty("RootNamespace", value); }
		}
		
		/// <summary>
		/// The full path of the assembly generated by the project.
		/// </summary>
		public override string OutputAssemblyFullPath {
			get {
				string outputPath = GetEvaluatedProperty("OutputPath") ?? "";
				return FileUtility.NormalizePath(Path.Combine(Path.Combine(Directory, outputPath), AssemblyName + GetExtension(OutputType)));
			}
		}
		
		/// <summary>
		/// The full path of the folder where the project's primary output files go.
		/// </summary>
		public string OutputFullPath {
			get {
				string outputPath = GetEvaluatedProperty("OutputPath");
				// FileUtility.NormalizePath() cleans up any back references.
				// e.g. C:\windows\system32\..\system becomes C:\windows\system
				return FileUtility.NormalizePath(Path.Combine(Directory, outputPath));
			}
		}
		
		[Browsable(false)]
		public OutputType OutputType {
			get {
				try {
					return (OutputType)Enum.Parse(typeof(OutputType), GetEvaluatedProperty("OutputType") ?? "Exe", true);
				} catch (ArgumentException) {
					return OutputType.Exe;
				}
			}
			set {
				SetProperty("OutputType", value.ToString());
			}
		}
		
		protected override IParseProjectContent CreateProjectContent()
		{
			ParseProjectContent newProjectContent = new ParseProjectContent(this);
			return newProjectContent;
		}
		
		#region Starting (debugging)
		public override bool IsStartable {
			get {
				if (IsSilverlightProject) {
					return TestPageFileName.Length > 0;
				}
				if (IsWebProject)
					return true;
				
				switch (this.StartAction) {
					case StartAction.Project:
						return OutputType == OutputType.Exe || OutputType == OutputType.WinExe;
					case StartAction.Program:
						return this.StartProgram.Length > 0;
					case StartAction.StartURL:
						return this.StartUrl.Length > 0;
					default:
						return false;
				}
			}
		}
		
		static string RemoveQuotes(string text)
		{
			if (text.StartsWith("\"") && text.EndsWith("\""))
				return text.Substring(1, text.Length - 2);
			else
				return text;
		}
		
		/// <summary>
		/// Creates a <see cref="ProcessStartInfo"/> for the specified program, using
		/// arguments and working directory from the project options.
		/// </summary>
		protected ProcessStartInfo CreateStartInfo(string program)
		{
			program = RemoveQuotes(program);
			if (!FileUtility.IsValidPath(program)) {
				throw new ProjectStartException(program + " is not a valid path; the process cannot be started.");
			}
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = Path.Combine(Directory, program);
			string workingDir = StringParser.Parse(this.StartWorkingDirectory);
			
			if (workingDir.Length == 0) {
				psi.WorkingDirectory = Path.GetDirectoryName(psi.FileName);
			} else {
				workingDir = RemoveQuotes(workingDir);
				
				if (!FileUtility.IsValidPath(workingDir)) {
					throw new ProjectStartException("Working directory '" + workingDir + "' is invalid; the process cannot be started. You can specify the working directory in the project options.");
				}
				psi.WorkingDirectory = Path.Combine(Directory, workingDir);
			}
			psi.Arguments = StringParser.Parse(this.StartArguments);
			
			if (!File.Exists(psi.FileName)) {
				throw new ProjectStartException(psi.FileName + " does not exist and cannot be started.");
			}
			if (!System.IO.Directory.Exists(psi.WorkingDirectory)) {
				throw new ProjectStartException("Working directory " + psi.WorkingDirectory + " does not exist; the process cannot be started. You can specify the working directory in the project options.");
			}
			return psi;
		}
		
		public override ProcessStartInfo CreateStartInfo()
		{
			if (IsSilverlightProject) {
				string pagePath = "file:///" + Path.Combine(OutputFullPath, TestPageFileName);
				return new  ProcessStartInfo(pagePath);
			}
			
			switch (this.StartAction) {
				case StartAction.Project:
					if (IsWebProject)
						return new ProcessStartInfo(LocalHost);
					
					return CreateStartInfo(this.OutputAssemblyFullPath);
				case StartAction.Program:
					return CreateStartInfo(this.StartProgram);
				case StartAction.StartURL:
					string url = this.StartUrl;
					if (!FileUtility.IsUrl(url))
						url = "http://" + url;
					return new ProcessStartInfo(url);
				default:
					throw new System.ComponentModel.InvalidEnumArgumentException("StartAction", (int)this.StartAction, typeof(StartAction));
			}
		}
		
		[Browsable(false)]
		public string StartProgram {
			get {
				return GetEvaluatedProperty("StartProgram") ?? "";
			}
			set {
				SetProperty("StartProgram", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		
		[Browsable(false)]
		public string StartUrl {
			get {
				return GetEvaluatedProperty("StartURL") ?? "";
			}
			set {
				SetProperty("StartURL", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		
		[Browsable(false)]
		public StartAction StartAction {
			get {
				try {
					return (StartAction)Enum.Parse(typeof(StartAction), GetEvaluatedProperty("StartAction") ?? "Project");
				} catch (ArgumentException) {
					return StartAction.Project;
				}
			}
			set {
				SetProperty("StartAction", value.ToString());
			}
		}
		
		[Browsable(false)]
		public string StartArguments {
			get {
				return GetEvaluatedProperty("StartArguments") ?? "";
			}
			set {
				SetProperty("StartArguments", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		
		[Browsable(false)]
		public string StartWorkingDirectory {
			get {
				return GetEvaluatedProperty("StartWorkingDirectory") ?? "";
			}
			set {
				SetProperty("StartWorkingDirectory", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		
		[Browsable(false)]
		public bool IsSilverlightProject {
			get {
				string guids = GetEvaluatedProperty("ProjectTypeGuids") ?? "";
				return guids.Contains("A1591282-1198-4647-A2B1-27E5FF5F6F3B");
			}
		}
		
		[Browsable(false)]
		public override bool IsWebProject {
			get {
				string guids = GetEvaluatedProperty("ProjectTypeGuids") ?? "";
				return guids.Contains("349c5851-65df-11da-9384-00065b846f21");
			}
		}
		
		[Browsable(false)]
		public string TestPageFileName {
			get {
				return GetEvaluatedProperty("TestPageFileName") ?? "";
			}
			set {
				SetProperty("TestPageFileName", string.IsNullOrEmpty(value) ? null : value);
			}
		}
		#endregion
		
		protected override void OnActiveConfigurationChanged(EventArgs e)
		{
			base.OnActiveConfigurationChanged(e);
			if (!isLoading) {
				ParserService.Reparse(this, true, true);
			}
		}
		
		protected override void OnActivePlatformChanged(EventArgs e)
		{
			base.OnActivePlatformChanged(e);
			if (!isLoading) {
				ParserService.Reparse(this, true, true);
			}
		}
		
		protected override void OnPropertyChanged(ProjectPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.PropertyName == "TargetFrameworkVersion")
				CreateItemsListFromMSBuild();
			if (!isLoading) {
				if (reparseReferencesSensitiveProperties.Contains(e.PropertyName)) {
					ParserService.Reparse(this, true, false);
				}
				if (reparseCodeSensitiveProperties.Contains(e.PropertyName)) {
					ParserService.Reparse(this, false, true);
				}
			}
		}
		
		[Browsable(false)]
		public override string TypeGuid {
			get {
				return ProjectBindingService.GetCodonPerLanguageName(Language).Guid;
			}
			set {
				throw new NotSupportedException();
			}
		}
		
		public override IItemType GetDefaultItemType(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (".resx".Equals(extension, StringComparison.OrdinalIgnoreCase)
			    || ".resources".Equals(extension, StringComparison.OrdinalIgnoreCase))
			{
				return ItemType.EmbeddedResource;
			} else if (".xaml".Equals(extension, StringComparison.OrdinalIgnoreCase)) {
				return ItemType.Page;
			} else {
				return base.GetDefaultItemType(fileName);
			}
		}
		
		public override void ProjectCreationComplete()
		{
			ITargetFramework fx = this.CurrentTargetFramework;
			if (fx != null && (fx.IsBasedOn(TargetFramework.Net35) || fx.IsBasedOn(TargetFramework.Net35Client))) {
				AddDotnet35References();
			}
			if (fx != null && (fx.IsBasedOn(TargetFramework.Net40) || fx.IsBasedOn(TargetFramework.Net40Client))) {
				AddDotnet40References();
			}
			if (fx != null)
				UpdateAppConfig(fx);
			base.ProjectCreationComplete();
		}
		
		protected virtual void AddDotnet35References()
		{
			AddReferenceIfNotExists("System.Core", "3.5");
			
			if (GetItemsOfType(ItemType.Reference).Any(r => string.Equals(r.Include, "System.Data", StringComparison.OrdinalIgnoreCase))) {
				AddReferenceIfNotExists("System.Data.DataSetExtensions", "3.5");
			}
			if (GetItemsOfType(ItemType.Reference).Any(r => string.Equals(r.Include, "System.Xml", StringComparison.OrdinalIgnoreCase))) {
				AddReferenceIfNotExists("System.Xml.Linq", "3.5");
			}
		}
		
		protected virtual void RemoveDotnet35References()
		{
			// undo "AddDotnet35References"
			RemoveReference("System.Core");
			RemoveReference("System.Data.DataSetExtensions");
			RemoveReference("System.Xml.Linq");
		}
		
		protected virtual void AddDotnet40References()
		{
			if (GetItemsOfType(ItemType.Reference).Any(r => string.Equals(r.Include, "WindowsBase", StringComparison.OrdinalIgnoreCase))) {
				AddReferenceIfNotExists("System.Xaml", "4.0");
			}
		}
		
		protected virtual void RemoveDotnet40References()
		{
			RemoveReference("System.Xaml");
		}
		
		void AddReferenceIfNotExists(string name, string requiredTargetFramework)
		{
			if (!(GetItemsOfType(ItemType.Reference).Any(r => string.Equals(r.Include, name, StringComparison.OrdinalIgnoreCase)))) {
				ReferenceProjectItem rpi = new ReferenceProjectItem(this, name);
				if (requiredTargetFramework != null)
					rpi.SetMetadata("RequiredTargetFramework", requiredTargetFramework);
				ProjectService.AddProjectItem(this, rpi);
			}
		}
		
		void RemoveReference(string name)
		{
			IProjectItem reference = GetItemsOfType(ItemType.Reference).FirstOrDefault(r => string.Equals(r.Include, name, StringComparison.OrdinalIgnoreCase));
			if (reference != null)
				ProjectService.RemoveProjectItem(this, reference);
		}
		
		protected virtual void AddOrRemoveExtensions()
		{
		}
		
		
		#region IUpgradableProject
		[Browsable(false)]
		public virtual bool UpgradeDesired {
			get {
				return MinimumSolutionVersion < Solution.SolutionVersionVS2010;
			}
		}
		
		static readonly CompilerVersion msbuild20 = new CompilerVersion(new Version(2, 0), "MSBuild 2.0");
		static readonly CompilerVersion msbuild35 = new CompilerVersion(new Version(3, 5), "MSBuild 3.5");
		static readonly CompilerVersion msbuild40 = new CompilerVersion(new Version(4, 0), "MSBuild 4.0");
		
		public virtual ICompilerVersion CurrentCompilerVersion {
			get {
				switch (MinimumSolutionVersion) {
					case Solution.SolutionVersionVS2005:
						return msbuild20;
					case Solution.SolutionVersionVS2008:
						return msbuild35;
					case Solution.SolutionVersionVS2010:
						return msbuild40;
					default:
						throw new NotSupportedException();
				}
			}
		}
		
		public virtual ITargetFramework CurrentTargetFramework {
			get {
				string fxVersion = this.TargetFrameworkVersion;
				string fxProfile = this.TargetFrameworkProfile;
				if (string.Equals(fxProfile, "Client", StringComparison.OrdinalIgnoreCase)) {
					foreach (ClientProfileTargetFramework fx in TargetFramework.TargetFrameworks.OfType<ClientProfileTargetFramework>())
						if (fx.FullFramework.Name == fxVersion)
							return fx;
				} else {
					foreach (TargetFramework fx in TargetFramework.TargetFrameworks)
						if (fx.Name == fxVersion)
							return fx;
				}
				return null;
			}
		}
		
		public virtual IEnumerable<ICompilerVersion> GetAvailableCompilerVersions()
		{
			return new[] { msbuild20, msbuild35, msbuild40 };
		}
		
		public virtual void UpgradeProject(ICompilerVersion newVersion, ITargetFramework newFramework)
		{
			if (!this.ReadOnly) {
				lock (SyncRoot) {
					ITargetFramework oldFramework = this.CurrentTargetFramework;
					if (newVersion != null && GetAvailableCompilerVersions().Contains(newVersion)) {
						SetToolsVersion(newVersion.MSBuildVersion.Major + "." + newVersion.MSBuildVersion.Minor);
					}
					if (newFramework != null) {
						UpdateAppConfig(newFramework);
						
						ClientProfileTargetFramework clientProfile = newFramework as ClientProfileTargetFramework;
						if (clientProfile != null) {
							newFramework = clientProfile.FullFramework;
							SetProperty(null, null, "TargetFrameworkProfile", "Client", PropertyStorageLocations.Base, true);
						} else {
							SetProperty(null, null, "TargetFrameworkProfile", "", PropertyStorageLocations.Base, true);
						}
						SetProperty(null, null, "TargetFrameworkVersion", newFramework.Name, PropertyStorageLocations.Base, true);
						
						if (oldFramework is ClientProfileTargetFramework)
							oldFramework = ((ClientProfileTargetFramework)oldFramework).FullFramework;
						
						if (oldFramework != null && !oldFramework.IsBasedOn(TargetFramework.Net35) && newFramework.IsBasedOn(TargetFramework.Net35))
							AddDotnet35References();
						else if (oldFramework != null && oldFramework.IsBasedOn(TargetFramework.Net35) && !newFramework.IsBasedOn(TargetFramework.Net35))
							RemoveDotnet35References();
						
						if (oldFramework != null && !oldFramework.IsBasedOn(TargetFramework.Net40) && newFramework.IsBasedOn(TargetFramework.Net40))
							AddDotnet40References();
						else if (oldFramework != null && oldFramework.IsBasedOn(TargetFramework.Net40) && !newFramework.IsBasedOn(TargetFramework.Net40))
							RemoveDotnet40References();
					}
					AddOrRemoveExtensions();
					Save();
				}
			}
		}
		
		public static FileName GetAppConfigFile(IProject project, bool createIfNotExists)
		{
			FileName appConfigFileName = Core.FileName.Create(Path.Combine(project.Directory, "app.config"));
			
			if (!File.Exists(appConfigFileName)) {
				if (createIfNotExists) {
					File.WriteAllText(appConfigFileName,
					                  "<?xml version=\"1.0\"?>" + Environment.NewLine +
					                  "<configuration>" + Environment.NewLine
					                  + "</configuration>");
				} else {
					return null;
				}
			}
			
			if (!project.IsFileInProject(appConfigFileName)) {
				FileProjectItem fpi = new FileProjectItem(project, ItemType.None, "app.config");
				ProjectService.AddProjectItem(project, fpi);
				FileService.FireFileCreated(appConfigFileName, false);
				ProjectBrowserPad.RefreshViewAsync();
			}
			return appConfigFileName;
		}
		
		void UpdateAppConfig(ITargetFramework newFramework)
		{
			// When changing the target framework, update any existing app.config
			// Also, for applications (not libraries), create an app.config is it is required for the target framework
			bool createAppConfig = newFramework.RequiresAppConfigEntry && (this.OutputType != OutputType.Library && this.OutputType != OutputType.Module);
			
			string appConfigFileName = GetAppConfigFile(this, createAppConfig);
			if (appConfigFileName == null)
				return;
			
			using (FakeXmlViewContent xml = new FakeXmlViewContent(appConfigFileName)) {
				if (xml.Document != null) {
					XElement configuration = xml.Document.Root;
					XElement startup = configuration.Element("startup");
					if (startup == null) {
						startup = new XElement("startup");
						if (configuration.HasElements && configuration.Elements().First().Name == "configSections") {
							// <configSections> must be first element
							configuration.Elements().First().AddAfterSelf(startup);
						} else {
							startup = configuration.AddFirstWithIndentation(startup);
						}
					}
					XElement supportedRuntime = startup.Element("supportedRuntime");
					if (supportedRuntime == null) {
						supportedRuntime = startup.AddFirstWithIndentation(new XElement("supportedRuntime"));
					}
					supportedRuntime.SetAttributeValue("version", newFramework.SupportedRuntimeVersion);
					supportedRuntime.SetAttributeValue("sku", newFramework.SupportedSku);
				}
			}
		}
		#endregion
	}
}
