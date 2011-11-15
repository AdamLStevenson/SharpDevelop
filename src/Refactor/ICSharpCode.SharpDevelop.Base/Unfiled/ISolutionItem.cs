// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;

namespace ICSharpCode.SharpDevelop.Project
{
	public interface ISolutionItem
	{
        string Name
        {
            get;
            set;
        }

        string Location
        {
            get;
            set;
        }

        void AppendItem(StringBuilder sb, string indentString);
	}
}
