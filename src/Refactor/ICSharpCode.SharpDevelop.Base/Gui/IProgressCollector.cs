using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Gui
{
    /// <summary>
    /// Collects progress using nested IProgressMonitors and provides it to a different thread using events.
    /// </summary>
    public interface IProgressCollector : INotifyPropertyChanged
    {
		event EventHandler ProgressMonitorDisposed;
		event PropertyChangedEventHandler PropertyChanged;

        double Progress
        {
            get;
        }

        bool ShowingDialog
        {
            get;
            set;
        }
		
		string TaskName
        {
			get;
		}
		
		OperationStatus Status
        {
			get;
		}
		
		IProgressMonitor ProgressMonitor
        {
			get;
		}
		
		/// <summary>
		/// Gets whether the root progress monitor was disposed.
		/// </summary>
        bool ProgressMonitorIsDisposed
        {
            get;
        }
    }
}
