using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.SharpDevelop.Project.Converter
{
    public interface ICompilerVersion
    {
        Version MSBuildVersion { get; }
		string DisplayName { get; }
		
		IEnumerable<ITargetFramework> GetSupportedTargetFrameworks();
		

		
		
    }
}
