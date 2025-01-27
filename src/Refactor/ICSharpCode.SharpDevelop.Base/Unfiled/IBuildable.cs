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
    /// A project or solution.
    /// The IBuildable interface members are thread-safe.
    /// </summary>
    public interface IBuildable
    {
        /// <summary>
        /// Gets the list of projects on which this project depends.
        /// This method is thread-safe.
        /// </summary>
        ICollection<IBuildable> GetBuildDependencies(IProjectBuildOptions buildOptions);

        /// <summary>
        /// Starts building the project using the specified options.
        /// This member must be implemented thread-safe.
        /// </summary>
        void StartBuild(IProjectBuildOptions buildOptions, IBuildFeedbackSink feedbackSink);

        /// <summary>
        /// Gets the name of the buildable item.
        /// This property is thread-safe.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the parent solution.
        /// This property is thread-safe.
        /// </summary>
        ISolution ParentSolution { get; }

        /// <summary>
        /// Creates the project-specific build options.
        /// This member must be implemented thread-safe.
        /// </summary>
        /// <param name="options">The global build options.</param>
        /// <param name="isRootBuildable">Specifies whether this project is the main buildable item.
        /// The root buildable is the buildable for which <see cref="BuildOptions.ProjectTarget"/> and <see cref="BuildOptions.ProjectAdditionalProperties"/> apply.
        /// The dependencies of that root buildable are the non-root buildables.</param>
        /// <returns>The project-specific build options.</returns>
        IProjectBuildOptions CreateProjectBuildOptions(IBuildOptions options, bool isRootBuildable);
    }
}
