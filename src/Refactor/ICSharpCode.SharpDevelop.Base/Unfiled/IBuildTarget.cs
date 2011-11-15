// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;


namespace ICSharpCode.SharpDevelop.Project
{
    public interface IBuildTarget : IEquatable<IBuildTarget>, IComparable<IBuildTarget>
    {
		string TargetName
        {
			get;
		}
    }
}
