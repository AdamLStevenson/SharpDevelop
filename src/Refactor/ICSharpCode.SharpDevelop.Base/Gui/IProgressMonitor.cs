using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ICSharpCode.SharpDevelop.Gui
{
    /// <summary>
    /// Represents a target where an active task reports progress to.
    /// </summary>
    /// <remarks>
    /// This interface is not thread-safe, but it can be used to report progress from background threads which will be
    /// automatically sent to the correct GUI thread.
    /// Using a progress monitor from multiple threads is possible if the user synchronizes the access.
    /// </remarks>
    public interface IProgressMonitor : IDisposable
    {
        /// <summary>
        /// Gets/Sets the amount of work already done within this task.
        /// Always uses a scale from 0 to 1 local to this progress monitor.
        /// </summary>
        double Progress { get; set; }

        /// <summary>
        /// Creates a nested task.
        /// </summary>
        /// <param name="workAmount">The amount of work this sub-task performs in relation to the work of this task.
        /// That means, this parameter is used as a scaling factor for work performed within the subtask.</param>
        /// <returns>A new progress monitor representing the sub-task.
        /// Multiple child progress monitors can be used at once; even concurrently on multiple threads.</returns>
        IProgressMonitor CreateSubTask(double workAmount);

        /// <summary>
        /// Gets/Sets the name to show while the task is active.
        /// </summary>
        string TaskName { get; set; }

        /// <summary>
        /// Gets/sets if the task current shows a modal dialog. Set this property to true to make progress
        /// dialogs windows temporarily invisible while your modal dialog is showing.
        /// </summary>
        bool ShowingDialog
        { // TODO: get rid of this. Don't mix calculations and UI!
            get;
            set;
        }

        /// <summary>
        /// Gets the cancellation token.
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Gets/Sets the operation status.
        /// 
        /// Note: the status of the whole operation is the most severe status of all nested monitors.
        /// The more severe value persists even if the child monitor gets disposed.
        /// </summary>
        OperationStatus Status { get; set; }
    }
}
