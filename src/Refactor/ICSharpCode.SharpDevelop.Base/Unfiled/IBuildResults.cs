// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace ICSharpCode.SharpDevelop.Project
{
    /// <summary>
    /// Class wrapping the results of a build run.
    /// </summary>
    public interface IBuildResults 
    {
        

        /// <summary>
        /// Adds a build error/warning to the results.
        /// This method is thread-safe.
        /// </summary>
        void Add(IBuildError error);

        /// <summary>
        /// Adds a project to the list of built projects.
        /// This method is thread-safe.
        /// </summary>
        void AddBuiltProject(IBuildable buildable);

        /// <summary>
        /// Gets the list of build errors or warnings.
        /// This property is thread-safe.
        /// </summary>
        ReadOnlyCollection<IBuildError> Errors
        {
            get;
        }

        /// <summary>
        /// Gets the list of projects that were built. This property is thread-safe.
        /// </summary>
        ReadOnlyCollection<IBuildable> BuiltProjects
        {
            get;
        }

        BuildResultCode Result
        {
            get;
            set;
        }

        int ErrorCount
        {
            get;
        }

        int WarningCount
        {
            get;
        }
    }
}
