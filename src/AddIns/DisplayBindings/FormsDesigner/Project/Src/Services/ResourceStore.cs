﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.FormsDesigner.Services
{
	/// <summary>
	/// Manages the resource files that belong to an open forms designer view.
	/// </summary>
	internal sealed class ResourceStore : IDisposable
	{
		readonly FormsDesignerViewContent viewContent;
		
		public ResourceStore(FormsDesignerViewContent viewContent)
		{
			if (viewContent == null)
				throw new ArgumentNullException("viewContent");
			this.viewContent = viewContent;
		}
		
		// Culture name (or empty string) => Resources
		readonly Dictionary<string, ResourceStorage> resources = new Dictionary<string, ResourceStorage>();
		readonly Dictionary<IOpenedFile, ResourceStorage> resourceByFile = new Dictionary<IOpenedFile, ResourceStorage>();
		
		ResourceStorage GetResourceStorage(CultureInfo culture)
		{
			ResourceStorage storage;
			if (!resources.TryGetValue(culture.Name, out storage)) {
				storage = resources[culture.Name] = new ResourceStorage(culture.Name);
				string fileName = CalcResourceFileName(viewContent.PrimaryFileName, culture.Name);
				CreateOpenedFileForStorage(storage, fileName, File.Exists(fileName));
			}
			return storage;
		}
		
		public IResourceReader GetReader(CultureInfo info)
		{
			return this.GetResourceStorage(info).GetReader();
		}
		
		public IResourceWriter GetWriter(CultureInfo info)
		{
			return this.GetResourceStorage(info).GetWriter();
		}
		
		void CreateOpenedFileForStorage(ResourceStorage storage, string fileName, bool isExistingFile)
		{
			storage.OpenedFile = FileService.GetOrCreateOpenedFile(fileName);
			storage.IsNewFile = !isExistingFile;
			if (!isExistingFile && storage.OpenedFile.RegisteredViewContents.Count == 0) {
				storage.OpenedFile.SetData(new byte[0]);
			}
			resourceByFile[storage.OpenedFile] = storage;
			// adding the opened file to the view content will load the file content into ResourceStorage.buffer
			viewContent.AddResourceFile(storage.OpenedFile);
		}
		
		public void Dispose()
		{
			foreach (ResourceStorage rs in this.resources.Values) {
				rs.Dispose();
			}
			this.resources.Clear();
			this.resourceByFile.Clear();
		}
		
		public void Load(IOpenedFile file, Stream stream)
		{
			resourceByFile[file].Load(stream);
		}
		
		public void Save(IOpenedFile file, Stream stream)
		{
			resourceByFile[file].Save(stream, this);
		}
		
		public void MarkResourceFilesAsDirty()
		{
			foreach (ResourceStorage rs in resourceByFile.Values) {
				if (rs.OpenedFile != null) {
					rs.OpenedFile.MakeDirty();
				}
			}
		}
		
		public void CommitAllResourceChanges()
		{
			foreach (ResourceStorage rs in this.resources.Values) {
				rs.WriteResourcesToBuffer();
			}
		}
		
		#region ResourceStorage
		sealed class ResourceStorage
		{
			MemoryStream stream;
			IResourceWriter writer;
			byte[] buffer;
			readonly string cultureName;
			internal OpenedFile OpenedFile;
			internal bool IsNewFile;
			
			public ResourceStorage(string cultureName)
			{
				this.cultureName = cultureName;
			}
			
			public void Dispose()
			{
				if (this.stream != null) {
					this.writer.Dispose();
					this.stream.Dispose();
					this.writer = null;
					this.stream = null;
				}
				this.buffer = null;
			}
			
			/// <summary>
			/// Writes the byte array containing the most recent version of the resource
			/// represented by this instance into the private field "buffer", if a
			/// resource writer has previously been requested for this resource.
			/// </summary>
			public void WriteResourcesToBuffer()
			{
				if (this.stream != null) {
					this.writer.Close();
					this.writer.Dispose();
					this.buffer = this.stream.ToArray();
					this.writer = null;
					this.stream.Dispose();
					this.stream = null;
				}
			}
			
			// load from OpenedFile into memory
			public void Load(Stream stream)
			{
				Dispose();
				
				this.buffer = new byte[stream.Length];
				int pos = 0;
				while (pos < buffer.Length)
					pos += stream.Read(buffer, pos, buffer.Length - pos);
			}
			
			/// <summary>
			/// Returns a new resource reader for this resource based on the most recent
			/// version available (either in memory or on disk).
			/// </summary>
			public IResourceReader GetReader()
			{
				if (this.buffer == null) {
					if (OpenedFile != null) {
						return CreateResourceReader(OpenFileContentAsMemoryStream(OpenedFile), GetResourceType(OpenedFile.FileName));
					} else {
						return null;
					}
				} else {
					ResourceType type = (OpenedFile != null) ? GetResourceType(OpenedFile.FileName) : ResourceType.Resx;
					return CreateResourceReader(new MemoryStream(this.buffer, false), type);
				}
			}
			
			static MemoryStream OpenFileContentAsMemoryStream(OpenedFile file)
			{
				Stream stream = file.OpenRead();
				MemoryStream ms = stream as MemoryStream;
				if (ms == null) {
					// copy stream content to memory
					try {
						ms = new MemoryStream();
						byte[] buffer = new byte[4096];
						int c;
						do {
							c = stream.Read(buffer, 0, buffer.Length);
							ms.Write(buffer, 0, c);
						} while (c > 0);
						ms.Position = 0;
					} finally {
						stream.Dispose();
					}
				}
				return ms;
			}
			
			/// <summary>
			/// Returns a new resource writer for this resource.
			/// According to the SDK documentation of IResourceService.GetResourceWriter,
			/// a new writer needs to be returned every time one is requested, discarding any
			/// data written by previously returned writers.
			/// </summary>
			public IResourceWriter GetWriter()
			{
				this.stream = new MemoryStream();
				this.writer = CreateResourceWriter(this.stream, GetResourceType(OpenedFile.FileName));
				return this.writer;
			}
			
			public void Save(Stream stream, ResourceStore resourceStore)
			{
				this.WriteResourcesToBuffer();
				if (buffer == null || buffer.Length == 0) return;
				stream.Write(buffer, 0, buffer.Length);
				// SD2-1588:
				// The possible call to AddFileToProject below
				// can cause a Subversion add command for the file to be issued.
				// For this command to succeed,
				// Subversion seems to require delete access to the file.
				// Since the OpenedFile implementation does not grant
				// delete access (what is very reasonable imho),
				// close the stream here to "unlock" it.
				// Of course this only works as long as the OpenedFile
				// implementation does not require the stream to be left open
				// after the save operation.
				stream.Close();
				
				// Check for file existance before adding it to the project
				// because this may as well be a save operation to a
				// MemoryStream before the file has been written to disk
				// for the first time.
				if (IsNewFile && File.Exists(this.OpenedFile.FileName)) {
					resourceStore.AddFileToProject(this);
					IsNewFile = false;
				}
			}
		}
		#endregion
		
		internal enum ResourceType {
			Resx = 0,
			Resources = 1
		};
		
		static IProject GetProject(string formFileName)
		{
			if (ProjectService.OpenSolution != null && formFileName != null)
				return ProjectService.OpenSolution.FindProjectContainingFile(formFileName);
			else
				return null;
		}
		
		void AddFileToProject(ResourceStorage storage)
		{
			string resourceFileName = storage.OpenedFile.FileName;
			string formFileName = viewContent.PrimaryFileName;
			IProject project = GetProject(formFileName);
			
			// Add this resource file to the project
			if (project != null && !project.IsFileInProject(resourceFileName)) {
				FileProjectItem newFileProjectItem = new FileProjectItem(project, ItemType.EmbeddedResource);
				newFileProjectItem.DependentUpon = Path.GetFileName(formFileName);
				newFileProjectItem.Include = FileUtility.GetRelativePath(project.Directory, resourceFileName);
				ProjectService.AddProjectItem(project, newFileProjectItem);
				FileService.FireFileCreated(resourceFileName, false);

				IPadDescriptor pd = WorkbenchSingleton.Instance.Workbench.GetPad(typeof(ProjectBrowserPad));
				FileNode formFileNode = ((ProjectBrowserPad)pd.PadContent).ProjectBrowserControl.FindFileNode(formFileName);
				if (formFileNode != null) {
					LoggingService.Info("FormFileNode found, adding subitem");
					FileNode fileNode = new FileNode(resourceFileName, FileNodeStatus.BehindFile);
					fileNode.AddTo(formFileNode);
					fileNode.ProjectItem = newFileProjectItem;
				}
				project.Save();
			}
		}
		
		static string CalcResourceFileName(string formFileName, string cultureName)
		{
			StringBuilder resourceFileName = null;
			IProject project = GetProject(formFileName);
			
			if (formFileName != null && formFileName != String.Empty) {
				resourceFileName = new StringBuilder(Path.GetDirectoryName(formFileName));
			} else if (project != null) {
				resourceFileName = new StringBuilder(project.Directory);
			} else {
				// required for untitled files. Untitled files should NOT save their resources.
				resourceFileName = new StringBuilder(Path.GetTempPath());
			}
			resourceFileName.Append(Path.DirectorySeparatorChar);
			string sourceFileName = null;
			if (project != null && formFileName != null) {
				// Try to find the source file name by using the project dependencies first.
				IFileProjectItem sourceItem = project.FindFile(formFileName);
				if (sourceItem != null && sourceItem.DependentUpon != null && sourceItem.DependentUpon.Length > 0) {
					sourceFileName = Path.GetFileNameWithoutExtension(sourceItem.DependentUpon);
				}
			}
			if (sourceFileName == null) {
				// If the source file name cannot be found using the project dependencies,
				// assume the resource file name to be equal to the current source file name.
				// Remove the ".Designer" part if present.
				sourceFileName = Path.GetFileNameWithoutExtension(formFileName);
				if (sourceFileName != null && sourceFileName.ToLowerInvariant().EndsWith(".designer")) {
					sourceFileName = sourceFileName.Substring(0, sourceFileName.Length - 9);
				}
			}
			resourceFileName.Append(sourceFileName);
			
			if (!string.IsNullOrEmpty(cultureName)) {
				resourceFileName.Append('.');
				resourceFileName.Append(cultureName);
			}
			
			// Use .resources filename if file exists.
			if (File.Exists(resourceFileName.ToString() + ".resources")) {
				resourceFileName.Append(".resources");
			} else {
				resourceFileName.Append(".resx");
			}
			
			return resourceFileName.ToString();
		}
		
		internal static IResourceReader CreateResourceReader(Stream stream, ResourceType type)
		{
			if (stream.Length == 0)
				return null;
			if (type == ResourceType.Resources) {
				return new ResourceReader(stream);
			}
			return new ResXResourceReader(stream);
		}
		
		internal static IResourceWriter CreateResourceWriter(Stream stream, ResourceType type)
		{
			if (type == ResourceType.Resources) {
				return new ResourceWriter(stream);
			}
			return new ResXResourceWriter(stream);
		}
		
		internal static ResourceType GetResourceType(string fileName)
		{
			if (Path.GetExtension(fileName).ToLowerInvariant() == ".resx") {
				return ResourceType.Resx;
			}
			return ResourceType.Resources;
		}
	}
}
