using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.SharpDevelop.Project
{
    public interface ITargetFramework
    {
        string Name
        {
            get;
        }

        string DisplayName
        {
            get;
        }

        /// <summary>
        /// Supported runtime version string for app.config
        /// </summary>
        string SupportedRuntimeVersion { get; set; }

        /// <summary>
        /// Supported SKU string for app.config.
        /// </summary>
        string SupportedSku { get; set; }

        /// <summary>
        /// Specifies whether this target framework requires an explicit app.config entry.
        /// </summary>
        bool RequiresAppConfigEntry { get; set; }

        /// <summary>
        /// Gets the minimum MSBuild version required to build projects with this target framework.
        /// </summary>
        Version MinimumMSBuildVersion { get; set; }

        /// <summary>
        /// Gets the previous release of this target framework.
        /// </summary>
        ITargetFramework BasedOn { get; set; }

        bool IsBasedOn(ITargetFramework potentialBase);

        
    }
}
