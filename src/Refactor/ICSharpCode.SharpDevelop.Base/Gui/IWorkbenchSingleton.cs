using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;

namespace ICSharpCode.SharpDevelop.Gui
{
    public interface IWorkbenchSingleton
    {
        /// <summary>
        /// Is called, when the workbench is created
        /// </summary>
        event EventHandler WorkbenchCreated;

        /// <summary>
        /// Is called, when the workbench is unloaded
        /// </summary>
        event EventHandler WorkbenchUnloaded;

        /// <summary>
        /// Gets the main form. Returns null in unit-testing mode.
        /// </summary>
        IWin32Window MainWin32Window
        {
            get;
        }

        /// <summary>
        /// Gets the main window. Returns null in unit-testing mode.
        /// </summary>
        Window MainWindow
        {
            get;
        }

        /// <summary>
        /// Gets the workbench. Returns null in unit-testing mode.
        /// </summary>
        IWorkbench Workbench
        {
            get;
            
        }

        void OnWorkbenchUnloaded();
    }
}
