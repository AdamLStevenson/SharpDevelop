using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Converter;

namespace ICSharpCode.SharpDevelop.Project
{
    /// <summary>
    /// A project with support for the UpgradeView
    /// </summary>
    public interface IUpgradableProject
    {
        /// <summary>
        /// Gets the project name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets whether an upgrade is desired (controls whether the upgrade view should pop
        /// up automatically)
        /// </summary>
        bool UpgradeDesired { get; }

        /// <summary>
        /// Gets the supported compiler versions.
        /// </summary>
        IEnumerable<ICompilerVersion> GetAvailableCompilerVersions();

        /// <summary>
        /// Gets the current compiler version.
        /// </summary>
        ICompilerVersion CurrentCompilerVersion { get; }

        /// <summary>
        /// Gets the current target framework.
        /// </summary>
        ITargetFramework CurrentTargetFramework { get; }

        /// <summary>
        /// Upgrades the selected compiler and target framework.
        /// </summary>
        /// <param name="newVersion">The new compiler version. If this property is null, the compiler version is not changed.</param>
        /// <param name="newFramework">The new target framework. If this property is null, the target framework is not changed.</param>
        void UpgradeProject(ICompilerVersion newVersion, ITargetFramework newFramework);
    }
}
