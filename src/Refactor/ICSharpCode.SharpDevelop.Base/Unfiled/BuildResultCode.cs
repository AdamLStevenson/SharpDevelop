// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.SharpDevelop.Project
{
    public enum BuildResultCode
    {
        None,
        /// <summary>Build finished successful.</summary>
        Success,
        /// <summary>A build error occurred, see BuildResults.Error collection</summary>
        Error,
        /// <summary>A project build file is not valid</summary>
        BuildFileError,
        /// <summary>Build was not executed because another build is running</summary>
        MSBuildAlreadyRunning,
        /// <summary>Build was cancelled.</summary>
        Cancelled
    }
}
