// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
    /// <summary>
    /// Default implementation for ISolutionFolderContainer. Thread-safe.
    /// </summary>
    public interface IAbstractSolutionFolder : ILocalizedObject, ISolutionFolder
    {
        

        /// <summary>
        /// Gets the solution this project belongs to. Returns null for projects that are not (yet) added to
        /// any solution; or are added to a solution folder that is not added to any solution.
        /// </summary>
        [Browsable(false)]
        ISolution ParentSolution
        {
            get;
        }





        
    }
}
