// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Tests.StringTagProvider
{
	/// <summary>
	/// Mock IProject implementation to test the SharpDevelopStringTagProvider.
	/// </summary>
	public class MockProjectForTagProvider : IProject
	{
		string fileName = String.Empty;
		string directory = String.Empty;
		string outputAssemblyFullPath = String.Empty;
		string name = String.Empty;
		
		public MockProjectForTagProvider()
		{
		}
		
		public ReadOnlyCollection<IProjectItem> Items {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICollection<IItemType> AvailableFileItemTypes {
			get {
				throw new NotImplementedException();
			}
		}
		
		public List<IProjectSection> ProjectSections {
			get {
				throw new NotImplementedException();
			}
		}
		
		public LanguageProperties LanguageProperties {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string FileName {
			get { return fileName; }
			set { fileName = value; }
		}
		
		public string Directory {
			get { return directory; }
			set { directory = value; }
		}
		
		public bool ReadOnly {
			get { return false; }
		}
		
		public string AssemblyName {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string RootNamespace {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string OutputAssemblyFullPath {
			get { return outputAssemblyFullPath; }
			set { outputAssemblyFullPath = value; }
		}
		
		public string Language {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string AppDesignerFolder {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string ActiveConfiguration {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string ActivePlatform {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public event EventHandler ActiveConfigurationChanged { add {} remove {} }
		
		public event EventHandler ActivePlatformChanged { add {} remove {} }
		
		public ICollection<string> ConfigurationNames {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICollection<string> PlatformNames {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsStartable {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int MinimumSolutionVersion {
			get {
				throw new NotImplementedException();
			}
		}
		
		public object SyncRoot {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ISolutionFolderContainer Parent {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public ISolution ParentSolution {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string TypeGuid {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string IdGuid {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string Location {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		public IEnumerable<IProjectItem> GetItemsOfType(IItemType type)
		{
			throw new NotImplementedException();
		}
		
		public IItemType GetDefaultItemType(string fileName)
		{
			throw new NotImplementedException();
		}
		
		public ICSharpCode.SharpDevelop.Dom.IAmbience GetAmbience()
		{
			throw new NotImplementedException();
		}
		
		public void Save()
		{
			throw new NotImplementedException();
		}
		
		public bool IsFileInProject(string fileName)
		{
			throw new NotImplementedException();
		}
		
		public IFileProjectItem FindFile(string fileName)
		{
			throw new NotImplementedException();
		}
		
		public void Start(bool withDebugging)
		{
			throw new NotImplementedException();
		}
		
		public IParseProjectContent CreateProjectContent()
		{
			throw new NotImplementedException();
		}
		
		public IProjectItem CreateProjectItem(IProjectItemBackendStore item)
		{
			throw new NotImplementedException();
		}
		
		public void ResolveAssemblyReferences()
		{
			throw new NotImplementedException();
		}
		
		public ICollection<IBuildable> GetBuildDependencies(IProjectBuildOptions buildOptions)
		{
			throw new NotImplementedException();
		}
		
		public void StartBuild(IProjectBuildOptions buildOptions, IBuildFeedbackSink feedbackSink)
		{
			throw new NotImplementedException();
		}
		
		public IProjectBuildOptions CreateProjectBuildOptions(IBuildOptions options, bool isRootBuildable)
		{
			throw new NotImplementedException();
		}
		
		public void Dispose()
		{
			throw new NotImplementedException();
		}
		
		public Properties CreateMemento()
		{
			throw new NotImplementedException();
		}
		
		public void SetMemento(ICSharpCode.Core.Properties memento)
		{
			throw new NotImplementedException();
		}		
		
		public void ProjectCreationComplete()
		{
			throw new NotImplementedException();
		}
		
		public XElement LoadProjectExtensions(string name)
		{
			throw new NotImplementedException();
		}
		
		public void SaveProjectExtensions(string name, XElement element)
		{
			throw new NotImplementedException();
		}
		
		public Properties ProjectSpecificProperties {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
