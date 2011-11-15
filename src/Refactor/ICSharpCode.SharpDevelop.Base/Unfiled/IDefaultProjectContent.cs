// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace ICSharpCode.SharpDevelop.Dom
{
    public interface IDefaultProjectContent : IProjectContent
    {
        IUsing DefaultImports
        {
            get;
            set;
        }

        object Project
        {
            get;
        }

        bool IsUpToDate
        {
            get;
        }

        List<Dictionary<string, IClass>> ClassLists
        {
            get;
        }

        ICollection<string> NamespaceNames
        {
            get;
        }

        XmlDoc XmlDoc
        {
            get;
        }

        ICollection<IClass> Classes
        {
            get;
        }

        SystemTypes systemTypes;

        /// <summary>
        /// Gets a class that allows to conveniently access commonly used types in the system
        /// namespace.
        /// </summary>
        SystemTypes SystemTypes
        {
            get
            {
                if (systemTypes == null)
                {
                    systemTypes = new SystemTypes(this);
                }
                return systemTypes;
            }
        }

        ICollection<IProjectContent> ReferencedContents
        {
            get;
        }

        

        /// <summary>
        /// Gets/Sets the properties of the language this project content was written in.
        /// </summary>
        LanguageProperties Language
        {
            [DebuggerStepThrough]
            get;
            set;
        }

        string GetXmlDocumentation(string memberTag);

        virtual void Dispose();

        [Conditional("DEBUG")]
        void CheckNotDisposed();

        void AddClassToNamespaceList(IClass addClass);



        virtual IList<IAttribute> GetAssemblyAttributes();

        void RemoveCompilationUnit(ICompilationUnit unit);

        void UpdateCompilationUnit(ICompilationUnit oldUnit, ICompilationUnit parserOutput, string fileName);

        

        #region Default Parser Layer dependent functions
        IClass GetClass(string typeName, int typeParameterCount);

        IClass GetClass(string typeName, int typeParameterCount, LanguageProperties language, GetClassOptions options);

        List<ICompletionEntry> GetNamespaceContents(string nameSpace);

        List<ICompletionEntry> GetAllContents();

        /// <summary>
        /// Adds the contents of all namespaces in this project to the <paramref name="list"/>.
        /// </summary>
        /// <param name="lookInReferences">If true, contents of referenced projects will be added as well (not recursive - just 1 level deep).</param>
        void AddAllContents(List<ICompletionEntry> list, LanguageProperties language, bool lookInReferences);

        /// <summary>
        /// Adds the contents of the specified <paramref name="nameSpace"/> to the <paramref name="list"/>.
        /// </summary>
        /// <param name="lookInReferences">If true, contents of referenced projects will be added as well (not recursive - just 1 level deep).</param>
        void AddNamespaceContents(List<ICompletionEntry> list, string nameSpace, LanguageProperties language, bool lookInReferences);

        bool NamespaceExists(string name);

        bool NamespaceExists(string name, LanguageProperties language, bool lookInReferences);

        SearchTypeResult SearchType(SearchTypeRequest request);

        /// <summary>
        /// Gets the position of a member in this project content (not a referenced one).
        /// </summary>
        /// <param name="fullMemberName">The full class name in Reflection syntax (always case sensitive, ` for generics)</param>
        /// <param name="lookInReferences">Whether to search in referenced project contents.</param>
        IClass GetClassByReflectionName(string className, bool lookInReferences);

        FilePosition GetPosition(IEntity d);
        #endregion

        void AddReferencedContent(IProjectContent pc);

        event EventHandler ReferencedContentsChanged;

        bool InternalsVisibleTo(IProjectContent otherProjectContent);

        /// <inheritdoc/>
        string AssemblyName
        {
            get;
        }
    }
}
