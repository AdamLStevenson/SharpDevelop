// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.Project
{
    /// <summary>
    /// Specifies options when starting a build.
    /// </summary>
    public interface IBuildOptions
    {
        

        /// <summary>
        /// Specifies whether dependencies should be built.
        /// </summary>
        bool BuildDependentProjects { get; set; }

        /// <summary>
        /// Specifies the solution configuration used for the build.
        /// </summary>
        string SolutionConfiguration { get; set; }

        /// <summary>
        /// Specifies the solution platform used for the build.
        /// </summary>
        string SolutionPlatform { get; set; }

        /// <summary>
        /// Specifies the number of projects that should be built in parallel.
        /// </summary>
        int ParallelProjectCount { get; set; }

        /// <summary>
        /// Gets/Sets the verbosity of build output.
        /// </summary>
        BuildOutputVerbosity BuildOutputVerbosity { get; set; }

        

        /// <summary>
        /// Gets the method to call when the build has finished.
        /// </summary>
        BuildCallback Callback
        {
            get;
        }

        /// <summary>
        /// The target to build for the project being built.
        /// </summary>
        IBuildTarget ProjectTarget
        {
            get;
        }

        /// <summary>
        /// The target to build for dependencies of the project being built.
        /// </summary>
        IBuildTarget TargetForDependencies { get; set; }

        /// <summary>
        /// Additional properties used for the build, both for the project being built and its dependencies.
        /// </summary>
        IDictionary<string, string> GlobalAdditionalProperties
        {
            get;
        }

        /// <summary>
        /// Additional properties used only for the project being built but not for its dependencies.
        /// </summary>
        IDictionary<string, string> ProjectAdditionalProperties
        {
            get;
        }
    }
}
