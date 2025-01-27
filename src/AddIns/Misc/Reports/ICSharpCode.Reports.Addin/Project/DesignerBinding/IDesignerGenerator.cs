﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.CodeDom;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop;
namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of IDesignerGenerator.
	/// </summary>
	public interface IDesignerGenerator
	{
		System.CodeDom.Compiler.CodeDomProvider CodeDomProvider {
			get;
		}
		void Attach(ReportDesignerView viewContent);
		void Detach();
		ReportDesignerView ViewContent { get; }
		/// <summary>
		/// Gets the collection of OpenedFiles that contain code which belongs
		/// to the designed form, not including resource files.
		/// </summary>
		/// <param name="designerCodeFile">Receives the file which contains the code to be modified by the forms designer.</param>
		/// <returns>A collection of OpenedFiles that contain code which belongs to the designed form.</returns>
		/// <remarks>The returned collection must include the <paramref name="designerCodeFile"/>.</remarks>
		IEnumerable<IOpenedFile> GetSourceFiles(out IOpenedFile designerCodeFile);
		void MergeFormChanges(CodeCompileUnit unit);
		bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out string file, out int position);
	}
}
