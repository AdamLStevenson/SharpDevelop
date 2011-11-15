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
    /// Interface for adding and removing items from a project. Not part of the IProject
    /// interface because in nearly all cases, ProjectService.Add/RemoveProjectItem should
    /// be used instead!
    /// So IProject implementors should implement this interface, but only the SharpDevelop methods
    /// ProjectService.AddProjectItem and RemoveProjectItem may call the interface members.
    /// </summary>
    public interface IProjectItemListProvider
    {
        /// <summary>
        /// Gets a list of items in the project.
        /// </summary>
        ReadOnlyCollection<IProjectItem> Items
        {
            get;
        }

        /// <summary>
        /// Adds a new entry to the Items-collection
        /// </summary>
        void AddProjectItem(IProjectItem item);

        /// <summary>
        /// Removes an entry from the Items-collection
        /// </summary>
        bool RemoveProjectItem(IProjectItem item);
    }
}
