﻿// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Type representing resolve errors.
	/// </summary>
	sealed class UnknownType : AbstractType
	{
		public override string Name {
			get { return "?"; }
		}
		
		public override bool? IsReferenceType {
			get { return null; }
		}
		
		public override bool Equals(IType other)
		{
			return other is UnknownType;
		}
		
		public override int GetHashCode()
		{
			return 950772036;
		}
	}
}
