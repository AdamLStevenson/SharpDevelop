// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Struct for strongly-typed passing of item types
	/// - we don't want to use strings everywhere.
	/// Basically this is something like a typedef for C# (without implicit conversions).
	/// </summary>
    public interface IItemType : IEquatable<IItemType>, IComparable<IItemType>
	{
        string ItemName
        {
            get;
        }

        
	}
}
