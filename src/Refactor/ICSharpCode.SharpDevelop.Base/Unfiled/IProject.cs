﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Base interface for projects.
	/// Thread-safe members lock on the SyncRoot. Non-thread-safe members may only be called from the main thread.
	/// 
	/// When you implement IProject, you should also implement IProjectItemListProvider and IProjectAllowChangeConfigurations
	/// </summary>
	public interface IProject
		: IBuildable, ISolutionFolder, IDisposable, IMementoCapable
	{
		/// <summary>
		/// Gets the list of items in the project. This member is thread-safe.
		/// The returned collection is guaranteed not to change - adding new items or removing existing items
		/// will create a new collection.
		/// </summary>
		ReadOnlyCollection<IProjectItem> Items {
			get;
		}
		
		/// <summary>
		/// Gets all items in the project that have the specified item type.
		/// This member is thread-safe.
		/// </summary>
		IEnumerable<IProjectItem> GetItemsOfType(IItemType type);
		
		/// <summary>
		/// Gets the default item type the specified file should have.
		/// </summary>
		/// <param name="fileName">The full path to the file to determine the item type for</param>
		IItemType GetDefaultItemType(string fileName);
		
		/// <summary>
		/// Gets the list of available file item types. This member is thread-safe.
		/// </summary>
		ICollection<IItemType> AvailableFileItemTypes {
			get;
		}
		
		/// <summary>
		/// Gets a list of project sections stored in the solution file for this project.
		/// </summary>
        List<IProjectSection> ProjectSections
        {
			get;
		}
		
		/// <summary>
		/// Gets the language properties used for this project. This member is thread-safe.
		/// </summary>
		ICSharpCode.SharpDevelop.Dom.LanguageProperties LanguageProperties {
			get;
		}
		
		/// <summary>
		/// Gets the ambience used for the project. This member is thread-safe.
		/// Because the IAmbience interface is not thread-safe, every call returns a new instance.
		/// </summary>
		ICSharpCode.SharpDevelop.Dom.IAmbience GetAmbience();
		
		/// <summary>
		/// Gets the name of the project file.
		/// (Full file name, example: @"D:\Serralongue\SharpDevelop\samples\CustomPad\CustomPad.csproj")
		/// 
		/// Only the getter is thread-safe.
		/// </summary>
		string FileName {
			get;
			set;
		}
		
		/// <summary>
		/// Gets/Sets the name of the project.
		/// 
		/// Only the getter is thread-safe.
		/// </summary>
		/// <remarks>
		/// Name already exists in ISolutionFolder, it's repeated here to prevent
		/// the ambiguity with IBuildable.Name.
		/// </remarks>
		new string Name {
			get;
			set;
		}
		
		/// <summary>
		/// Gets the directory of the project file.
		/// This is equivalent to Path.GetDirectoryName(project.FileName);
		/// (Example: @"D:\Serralongue\SharpDevelop\samples\CustomPad")
		/// 
		/// This member is thread-safe.
		/// </summary>
		string Directory {
			get;
		}
		
		/// <summary>
		/// <para>
		/// True if the project is readonly. For project based files this means
		/// the project file has the readonly attribute set. For solution folder
		/// based projects this means that the sln file containing the project
		/// has the readonly attribute set.
		/// </para>
		/// <para>This member is thread-safe.</para>
		/// </summary>
		bool ReadOnly {
			get;
		}
		
		#region MSBuild properties used inside SharpDevelop base
		/// <summary>
		/// Gets/Sets the assembly name of the assembly created when building this project.
		/// Equivalent to MSBuild property "AssemblyName".
		/// </summary>
		string AssemblyName {
			get;
			set;
		}
		
		/// <summary>
		/// Gets/Sets the root namespace of the project.
		/// </summary>
		string RootNamespace {
			get;
			set;
		}
		
		/// <summary>
		/// Gets the full path of the output assembly.
		/// Returns null when the project does not output any assembly.
		/// </summary>
		string OutputAssemblyFullPath {
			get;
		}
		
		/// <summary>
		/// Gets the name of the language binding used for the project.
		/// </summary>
		string Language {
			get;
		}
		
		/// <summary>
		/// Gets the name of the directory being the "Properties" folder of the application,
		/// relative to the project directory.
		/// This folder gets a node type in the project browser.
		/// Equivalent to MSBuild property "AppDesignerFolder".
		/// </summary>
		string AppDesignerFolder {
			get;
		}
		#endregion
		
		#region Configuration / Platform management
		/// <summary>
		/// Gets/Sets the active configuration.
		/// </summary>
		string ActiveConfiguration {
			get;
			set;
		}
		
		/// <summary>
		/// Gets/Sets the active platform.
		/// </summary>
		string ActivePlatform {
			get;
			set;
		}
		/// <summary>
		/// Gets the list of available configuration names.
		/// </summary>
		ICollection<string> ConfigurationNames { get; }
		
		/// <summary>
		/// Gets the list of available platform names.
		/// </summary>
		ICollection<string> PlatformNames { get; }
		
		/// <summary>
		/// Is raised after the ActiveConfiguration property has changed.
		/// </summary>
		event EventHandler ActiveConfigurationChanged;
		
		/// <summary>
		/// Is raised after the ActivePlatform property has changed.
		/// </summary>
		event EventHandler ActivePlatformChanged;
		#endregion
		
		/// <summary>
		/// Saves the project using it's current file name.
		/// </summary>
		void Save();
		
		/// <summary>
		/// Returns true, if a specific file (given by it's name) is inside this project.
		/// This member is thread-safe.
		/// </summary>
		/// <param name="fileName">The <b>fully qualified</b> file name of the file</param>
		bool IsFileInProject(string fileName);
		
		/// <summary>
		/// Returns the project item for a specific file; or null if the file is not found in the project.
		/// This member is thread-safe.
		/// </summary>
		/// <param name="fileName">The <b>fully qualified</b> file name of the file</param>
        IFileProjectItem FindFile(string fileName);
		
		/// <summary>
		/// Gets if the project can be started.
		/// </summary>
		bool IsStartable { get; }
		
		/// <summary>
		/// Gets project specific properties.
		/// </summary>
		Properties ProjectSpecificProperties { get; }
		
		/// <summary>
		/// Starts the project.
		/// </summary>
		/// <param name="withDebugging">True, if a debugger should be used for the project.</param>
		void Start(bool withDebugging);
		
		/// <summary>
		/// Creates a new project content for this project.
		/// This method should only be called by ParserService.LoadSolutionProjectsInternal()!
		/// Return null if you don't want to create any project content.
		/// </summary>
		IParseProjectContent CreateProjectContent();
		
		/// <summary>
		/// Creates a new ProjectItem for the passed MSBuild item.
		/// </summary>
		IProjectItem CreateProjectItem(IProjectItemBackendStore item);
		
		/// <summary>
		/// Gets the minimum version the solution must have to support this project type.
		/// </summary>
		int MinimumSolutionVersion { get; }
		
		/// <summary>
		/// Retrieve the fully qualified assembly names and file location of referenced assemblies.
		/// This method is thread safe.
		/// </summary>
		void ResolveAssemblyReferences();
		
		/// <summary>
		/// Notifies the project that it was succesfully created from a project template.
		/// </summary>
		void ProjectCreationComplete();
		
		/// <summary>
		/// Loads the project extension content with the specified name.
		/// </summary>
		XElement LoadProjectExtensions(string name);
		
		/// <summary>
		/// Saves the project extension content with the specified name.
		/// </summary>
		void SaveProjectExtensions(string name, XElement element);
	}
	
	
	
	
	
	
}
