// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
    /// <summary>
    /// Interface for changing project or solution configuration.
    /// IProject implementors should implement this interface, but only the SharpDevelop methods
    /// Solution.RenameProjectPlatform etc. may call the interface members.
    /// </summary>
    public interface IProjectAllowChangeConfigurations
    {
        bool RenameProjectConfiguration(string oldName, string newName);
        bool RenameProjectPlatform(string oldName, string newName);
        bool AddProjectConfiguration(string newName, string copyFrom);
        bool AddProjectPlatform(string newName, string copyFrom);
        bool RemoveProjectConfiguration(string name);
        bool RemoveProjectPlatform(string name);
    }
}
