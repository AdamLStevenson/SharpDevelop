// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
    
    public interface IBuildError
    {
        string HelpKeyword
        {
            get;
            set;
        }

        string Subcategory
        {
            get;
            set;
        }

        int Column
        {
            get;
            set;
        }

        string ErrorCode
        {
            get;
            set;
        }

        string ErrorText
        {
            get;
            set;
        }

        string FileName
        {
            get;
            set;
        }

        int Line
        {
            get;
            set;
        }

        bool IsWarning
        {
            get;
            set;
        }

        /// <summary>
        /// Allows to store any object with this error. An object might be attached by a custom
        /// MSBuild logger and later read by the context menu command.
        /// </summary>
        /// <remarks>The Tag property is [NonSerialized], which shouldn't be a problem
        /// because both custom loggers and context menu commands are guaranteed to run
        /// in the main AppDomain.</remarks>
        object Tag
        {
            get;
            set;
        }

        string ContextMenuAddInTreeEntry
        {
            get;
            set;
        }

    }
}
