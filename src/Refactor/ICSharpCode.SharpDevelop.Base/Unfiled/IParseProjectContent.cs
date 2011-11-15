// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop
{
    public interface IParseProjectContent :IProjectContent
    {

        void Initialize1(IProgressMonitor progressMonitor);

        void Initialize2(IProgressMonitor progressMonitor);

        int GetInitializationWorkAmount();

        string ProjectName
        {
            get;
        }
    }
}
