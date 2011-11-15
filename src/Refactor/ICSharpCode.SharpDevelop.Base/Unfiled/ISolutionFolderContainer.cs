// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ISolutionFolderContainer.
	/// </summary>
	public interface ISolutionFolderContainer:IAbstractSolutionFolder
	{
        [Browsable(false)]
        string TypeGuid
        {
            get;
            set;
        }

        [Browsable(false)]
        bool IsEmpty
        {
            get;
        }
		
		#region ISolutionFolderContainer implementation
		
		
		[Browsable(false)]
		List<IProjectSection> Sections {
			get;
		}
		
		[Browsable(false)]
		List<ISolutionFolder> Folders {
			get;
		}
		
		[Browsable(false)]
		IProjectSection SolutionItems {
			get;
		}
		
		void AddFolder(ISolutionFolder folder);
		
		void RemoveFolder(ISolutionFolder folder);
		
		bool IsAncestorOf(ISolutionFolder folder);
		#endregion
		
		


        
	}
}
