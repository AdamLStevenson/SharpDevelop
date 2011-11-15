using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpDevelop.Gui;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
    /// <summary>
    /// Represents an opened file.
    /// </summary>
    public interface IOpenedFile : ICanBeDirty
    {
        

        #region IsDirty
     
        event EventHandler IsDirtyChanged;

        /// <summary>
        /// Gets/sets if the file is has unsaved changes.
        /// </summary>
        bool IsDirty
        {
            get;
            set;
        }

        /// <summary>
        /// Marks the file as dirty if it currently is not in a load operation.
        /// </summary>
        void MakeDirty();

        #endregion

        

        /// <summary>
        /// Gets if the file is untitled. Untitled files show a "Save as" dialog when they are saved.
        /// </summary>
        bool IsUntitled
        {
            get;
        }

        

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        FileName FileName
        {
            get;
            set;
        }

        /// <summary>
        /// Occurs when the file name has changed.
        /// </summary>
        event EventHandler FileNameChanged;

        event EventHandler FileClosed;

        /// <summary>
        /// Use this method to save the file to disk using a new name.
        /// </summary>
        void SaveToDisk(string newFileName);

        void RegisterView(IViewContent view);
        void UnregisterView(IViewContent view);

        void CloseIfAllViewsClosed();

        /// <summary>
        /// Forces initialization of the specified view.
        /// </summary>
        void ForceInitializeView(IViewContent view);

        /// <summary>
        /// Gets the list of view contents registered with this opened file.
        /// </summary>
        IList<IViewContent> RegisteredViewContents
        {
            get;
        }

        /// <summary>
        /// Gets the view content that currently edits this file.
        /// If there are multiple view contents registered, this returns the view content that was last
        /// active. The property might return null even if view contents are registered if the last active
        /// content was closed. In that case, the file is stored in-memory and loaded when one of the
        /// registered view contents becomes active.
        /// </summary>
        IViewContent CurrentView
        {
            get;
        }

        /// <summary>
        /// Opens the file for reading.
        /// </summary>
        Stream OpenRead();

        /// <summary>
        /// Sets the internally stored data to the specified byte array.
        /// This method should only be used when there is no current view or by the
        /// current view.
        /// </summary>
        /// <remarks>
        /// Use this method to specify the initial file content if you use a OpenedFile instance
        /// for a file that doesn't exist on disk but should be automatically created when a view
        /// with the file is saved, e.g. for .resx files created by the forms designer.
        /// </remarks>
        void SetData(byte[] fileData);

        /// <summary>
        /// Save the file to disk using the current name.
        /// </summary>
        void SaveToDisk();

        //		/// <summary>
        //		/// Called before saving the current view. This event is raised both when saving to disk and to memory (for switching between views).
        //		/// </summary>
        //		public event EventHandler SavingCurrentView;
        //
        //		/// <summary>
        //		/// Called after saving the current view. This event is raised both when saving to disk and to memory (for switching between views).
        //		/// </summary>
        //		public event EventHandler SavedCurrentView;






        void SwitchedToView(IViewContent newView);


        void ReloadFromDisk();

    }
}
