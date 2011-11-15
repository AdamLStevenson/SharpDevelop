// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
    /// <summary>
    /// A project item is based either on an MSBuild build item, or "manually" saves the
    /// type/include/metadata. The project item is strictly bound to it's parent project.
    /// The MSBuild build item is used while the item is added to the project (IsAddedToProject
    /// is true). During that time, Include may not be an empty string.
    /// However, prior to the item being added to the project, Include may be an empty string
    /// (this is also the default for new items created using the (IProject, ItemType) constructor.
    /// </summary>
    public interface IProjectItem : IDisposable, ICloneable
    {
        [Browsable(false)]
        IProject Project
        {
            get;
        }

        object SyncRoot
        {
            get;
        }

        /// <summary>
        /// Gets if the item is added to it's owner project.
        /// </summary>
        [Browsable(false)]
        bool IsAddedToProject
        {
            get;
        }

        [Browsable(false)]
        bool TreatIncludeAsLiteral
        {
            get;
            set;
        }

        IProjectItemBackendStore BuildItem
        {
            get;
            set;
        }

        [Browsable(false)]
        IItemType ItemType
        {
            get;
            set;
        }

        [Browsable(false)]
        string Include
        {
            get;
            set;
        }

        #region Metadata access
        bool HasMetadata(string metadataName);

        /// <summary>
        /// Gets the evaluated value of the metadata item with the specified name.
        /// Returns an empty string for non-existing meta data items.
        /// </summary>
        string GetEvaluatedMetadata(string metadataName);

        /// <summary>
        /// Gets the value of the metadata item with the specified name.
        /// Returns defaultValue for non-existing meta data items.
        /// </summary>
        T GetEvaluatedMetadata<T>(string metadataName, T defaultValue);

        /// <summary>
        /// Gets the escaped/unevaluated value of the metadata item with the specified name.
        /// Returns an empty string for non-existing meta data items.
        /// </summary>
        string GetMetadata(string metadataName);

        /// <summary>
        /// Sets the value of the specified meta data item. The value is escaped before
        /// setting it to ensure characters like ';' or '$' are not interpreted by MSBuild.
        /// Setting value to null or an empty string results in removing the metadata item.
        /// </summary>
        void SetEvaluatedMetadata(string metadataName, string value);

        /// <summary>
        /// Sets the value of the specified meta data item. The value is escaped before
        /// setting it to ensure characters like ';' or '$' are not interpreted by MSBuild.
        /// </summary>
        void SetEvaluatedMetadata<T>(string metadataName, T value);

        /// <summary>
        /// Sets the value of the specified meta data item.
        /// Setting value to null or an empty string results in removing the metadata item.
        /// </summary>
        void SetMetadata(string metadataName, string value);

        /// <summary>
        /// Removes the specified meta data item.
        /// </summary>
        void RemoveMetadata(string metadataName);

        /// <summary>
        /// Gets the names of all existing meta data items on this project item. The resulting collection
        /// is a copy that will not be affected by future changes to the project item.
        /// </summary>
        [Browsable(false)]
        IEnumerable<string> MetadataNames
        {
            get;
        }
        #endregion

        /// <summary>
        /// Copies all meta data from this item to the target item.
        /// </summary>
        void CopyMetadataTo(IProjectItem targetItem);

        /// <summary>
        /// Clones this project item. Unless overridden, cloning works by cloning the underlying
        /// MSBuild item and creating a new project item for it.
        /// Using the default Clone() implementation requires that the item is has the Project
        /// property set - cloning a ProjectItem without a project will result in a NotSupportedException.
        /// </summary>
        IProjectItem Clone();

        /// <summary>
        /// Clones this project item by cloning the underlying
        /// MSBuild item and creating a new project item in the target project for it.
        /// </summary>
        IProjectItem CloneFor(IProject targetProject);

        /// <summary>
        /// Gets/Sets the full path of the file represented by "Include".
        /// For ProjectItems that are not assigned to any project, the getter returns the value of Include
        /// and the setter throws a NotSupportedException.
        /// </summary>
        [Browsable(false)]
        string FileName
        {
            get;
            set;
        }

        void Dispose();

        [Browsable(false)]
        bool IsDisposed
        {
            get;
        }

        void InformSetValue(PropertyDescriptor propertyDescriptor, object component, object value);
    }
}
