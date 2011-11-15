// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project.Converter
{


    public class CompilerVersion : ICompilerVersion
	{
		public Version MSBuildVersion { get; private set; }
		public string DisplayName { get; private set; }
		
		public virtual IEnumerable<ITargetFramework> GetSupportedTargetFrameworks()
		{
			return from fx in TargetFramework.TargetFrameworks
				where fx.MinimumMSBuildVersion != null
				where MSBuildVersion >= fx.MinimumMSBuildVersion
				select fx;
		}
		
		public CompilerVersion(Version msbuildVersion, string displayName)
		{
			if (msbuildVersion == null)
				throw new ArgumentNullException("msbuildVersion");
			this.MSBuildVersion = msbuildVersion;
			this.DisplayName = displayName;
		}
		
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (GetType() != obj.GetType())
				return false;
			CompilerVersion v = (CompilerVersion)obj;
			return this.MSBuildVersion == v.MSBuildVersion;
		}
		
		public override int GetHashCode()
		{
			return MSBuildVersion.GetHashCode();
		}
		
		public override string ToString()
		{
			return DisplayName;
		}
	}
}
