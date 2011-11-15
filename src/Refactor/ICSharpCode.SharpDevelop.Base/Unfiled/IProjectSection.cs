// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ProjectSection.
	/// </summary>
	public interface IProjectSection
	{
		string Name {
			get; 
		}

        string SectionType
        {
            get;
        }

        List<ISolutionItem> Items
        {
            get;
        }

        void AppendSection(StringBuilder sb, string indentString);
		
		
		
		
	}
}
