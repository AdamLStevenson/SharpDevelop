﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public class TargetFramework:ITargetFramework
	{
		public readonly static TargetFramework Net20 = new TargetFramework("v2.0", ".NET Framework 2.0") {
			SupportedRuntimeVersion = "v2.0.50727",
			MinimumMSBuildVersion = new Version(2, 0)
		};
		public readonly static TargetFramework Net30 = new TargetFramework("v3.0", ".NET Framework 3.0") {
			SupportedRuntimeVersion = "v2.0.50727",
			BasedOn = Net20,
			MinimumMSBuildVersion = new Version(3, 5)
		};
		public readonly static TargetFramework Net35 = new TargetFramework("v3.5", ".NET Framework 3.5") {
			SupportedRuntimeVersion = "v2.0.50727",
			BasedOn = Net30,
			MinimumMSBuildVersion = new Version(3, 5)
		};
		public readonly static TargetFramework Net35Client = new ClientProfileTargetFramework(Net35) {
			RequiresAppConfigEntry = true
		};
		public readonly static TargetFramework Net40 = new TargetFramework("v4.0", ".NET Framework 4.0") {
			BasedOn = Net35,
			MinimumMSBuildVersion = new Version(4, 0),
			SupportedSku = ".NETFramework,Version=v4.0",
			RequiresAppConfigEntry = true
		};
		public readonly static TargetFramework Net40Client = new ClientProfileTargetFramework(Net40) {
			BasedOn = Net35Client
		};
		
		public readonly static TargetFramework[] TargetFrameworks = {
			Net40, Net40Client, Net35, Net35Client, Net30, Net20
		};
		
		public readonly static TargetFramework DefaultTargetFramework = Net40Client;
		
		public static TargetFramework GetByName(string name)
		{
			foreach (TargetFramework tf in TargetFrameworks) {
				if (tf.Name == name)
					return tf;
			}
			throw new ArgumentException("No target framework '" + name + "' exists");
		}
		
		string name, displayName;
		
		public TargetFramework(string name, string displayName)
		{
			this.name = name;
			this.displayName = displayName;
			this.SupportedRuntimeVersion = name;
		}
		
		public string Name {
			get { return name; }
		}
		
		public string DisplayName {
			get { return displayName; }
		}
		
		/// <summary>
		/// Supported runtime version string for app.config
		/// </summary>
		public string SupportedRuntimeVersion { get; set; }
		
		/// <summary>
		/// Supported SKU string for app.config.
		/// </summary>
		public string SupportedSku { get; set; }
		
		/// <summary>
		/// Specifies whether this target framework requires an explicit app.config entry.
		/// </summary>
		public bool RequiresAppConfigEntry { get; set; }
		
		/// <summary>
		/// Gets the minimum MSBuild version required to build projects with this target framework.
		/// </summary>
		public Version MinimumMSBuildVersion { get; set; }
		
		/// <summary>
		/// Gets the previous release of this target framework.
		/// </summary>
        public ITargetFramework BasedOn { get; set; }

        public bool IsBasedOn(ITargetFramework potentialBase)
		{
            ITargetFramework tmp = this;
			while (tmp != null) {
				if (tmp == potentialBase)
					return true;
				tmp = tmp.BasedOn;
			}
			return false;
		}
		
		public override string ToString()
		{
			return DisplayName;
		}
	}
	
	public class ClientProfileTargetFramework : TargetFramework
	{
		public TargetFramework FullFramework { get; private set; }
		
		public ClientProfileTargetFramework(TargetFramework fullFramework)
			: base(fullFramework.Name + "Client", fullFramework.DisplayName + " Client Profile")
		{
			this.FullFramework = fullFramework;
			this.SupportedRuntimeVersion = fullFramework.SupportedRuntimeVersion;
			this.MinimumMSBuildVersion = fullFramework.MinimumMSBuildVersion;
			if (fullFramework.SupportedSku != null)
				this.SupportedSku = fullFramework.SupportedSku + ",Profile=Client";
			else
				this.SupportedSku = "Client";
		}
	}
}
