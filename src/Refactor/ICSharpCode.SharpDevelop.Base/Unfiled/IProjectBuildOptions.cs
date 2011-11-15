// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.Project
{
    

    /// <summary>
    /// Specifies options for building a single project.
    /// </summary>
    public interface IProjectBuildOptions
    {
        IBuildTarget Target
        {
            get;
        }

        IDictionary<string, string> Properties
        {
            get;
        }

        /// <summary>
        /// Specifies the project configuration used for the build.
        /// </summary>
        string Configuration { get; set; }

        /// <summary>
        /// Specifies the project platform used for the build.
        /// </summary>
        string Platform { get; set; }

        /// <summary>
        /// Gets/Sets the verbosity of build output.
        /// </summary>
        BuildOutputVerbosity BuildOutputVerbosity { get; set; }
    }

    

    
}
