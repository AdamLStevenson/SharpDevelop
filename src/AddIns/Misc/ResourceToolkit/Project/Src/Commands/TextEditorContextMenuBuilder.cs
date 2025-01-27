﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Forms;

using Hornung.ResourceToolkit.Gui;
using Hornung.ResourceToolkit.Refactoring;
using Hornung.ResourceToolkit.Resolver;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;
using MenuItem = System.Windows.Controls.MenuItem;

namespace Hornung.ResourceToolkit.Commands
{
	/// <summary>
	/// Builds context menu for editing string resources.
	/// </summary>
	public sealed class TextEditorContextMenuBuilder : IMenuItemBuilder
	{
		static readonly System.Windows.Controls.Control[] EmptyControlArray = new System.Windows.Controls.Control[0];
		
		System.Collections.ICollection IMenuItemBuilder.BuildItems(Codon codon, object owner)
		{
			ITextEditor editor = owner as ITextEditor;
			if (editor == null) {
				ITextEditorProvider provider = owner as ITextEditorProvider;
				if (provider == null) {
					return EmptyControlArray;
				}
				editor = provider.TextEditor;
			}
			
			ResourceResolveResult result = ResourceResolverService.Resolve(editor, null);
			if (result != null && result.ResourceFileContent != null && result.Key != null) {
				
				List<MenuItem> items = new List<MenuItem>();
				MenuItem item = new MenuItem();
				
				// add resource (if key does not exist) / edit resource (if key exists)
				if (result.ResourceFileContent.ContainsKey(result.Key)) {
					item.Header = MenuService.ConvertLabel(StringParser.Parse("${res:Hornung.ResourceToolkit.TextEditorContextMenu.EditResource}"));
				} else {
					item.Header = MenuService.ConvertLabel(StringParser.Parse("${res:Hornung.ResourceToolkit.TextEditorContextMenu.AddResource}"));
				}
				item.Click += this.EditResource;
				item.Tag = result;
				items.Add(item);
				
				// find references
				item = new MenuItem();
				item.Header = MenuService.ConvertLabel(StringParser.Parse("${res:SharpDevelop.Refactoring.FindReferencesCommand}"));
				item.Click += this.FindReferences;
				item.Tag = result;
				items.Add(item);
				
				// rename
				item = new MenuItem();
				item.Header = MenuService.ConvertLabel(StringParser.Parse("${res:SharpDevelop.Refactoring.RenameCommand}"));
				item.Click += this.Rename;
				item.Tag = result;
				items.Add(item);
				
				
				// put the resource menu items into a submenu
				// with the resource key as title
				item = new MenuItem();
				item.Header = result.Key;
				item.ItemsSource = items;
				return new System.Windows.Controls.Control[] { item, new Separator() };
				
			}
			
			return EmptyControlArray;
		}
		
		// ********************************************************************************************************************************
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "ICSharpCode.Core.MessageService.ShowWarning(System.String)")]
		void EditResource(object sender, EventArgs e)
		{
			MenuItem item = sender as MenuItem;
			if (item == null) {
				return;
			}
			
			ResourceResolveResult result = item.Tag as ResourceResolveResult;
			if (result == null) {
				return;
			}
			
			object value;
			string svalue = null;
			if (result.ResourceFileContent.TryGetValue(result.Key, out value)) {
				svalue = value as string;
				if (svalue == null) {
					MessageService.ShowWarning("${res:Hornung.ResourceToolkit.ResourceTypeNotSupported}");
					return;
				}
			}
			
			EditStringResourceDialog dialog = new EditStringResourceDialog(result.ResourceFileContent, result.Key, svalue, false);
			if (svalue == null) {
				dialog.Text = String.Format(CultureInfo.CurrentCulture, StringParser.Parse("${res:Hornung.ResourceToolkit.CodeCompletion.AddNewDescription}"), result.ResourceFileContent.FileName);
			}
			if (dialog.ShowDialog(WorkbenchSingleton.Instance.MainWin32Window) == DialogResult.OK) {
				if (svalue == null) {
					// Add new resource.
					result.ResourceFileContent.Add(dialog.Key, dialog.Value);
				} else {
					// Modify existing resource.
					result.ResourceFileContent.SetValue(result.Key, dialog.Value);
				}
			}
			
		}
		
		// ********************************************************************************************************************************
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body")]
		void FindReferences(object sender, EventArgs e)
		{
			MenuItem item = sender as MenuItem;
			if (item == null) {
				return;
			}
			
			ResourceResolveResult result = item.Tag as ResourceResolveResult;
			if (result == null) {
				return;
			}
			
			// Allow the menu to close
			Application.DoEvents();
			using(AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog(ResourceService.GetString("SharpDevelop.Refactoring.FindReferences"))) {
				FindReferencesAndRenameHelper.ShowAsSearchResults(
					StringParser.Parse("${res:Hornung.ResourceToolkit.ReferencesToResource}",
					                   new StringTagPair("ResourceFileName", System.IO.Path.GetFileName(result.FileName)),
					                   new StringTagPair("ResourceKey", result.Key)),
					ResourceRefactoringService.FindReferences(result.FileName, result.Key, monitor));
			}
		}
		
		void Rename(object sender, EventArgs e)
		{
			MenuItem item = sender as MenuItem;
			if (item == null) {
				return;
			}
			
			ResourceResolveResult result = item.Tag as ResourceResolveResult;
			if (result == null) {
				return;
			}
			
			// Allow the menu to close
			Application.DoEvents();
			ResourceRefactoringService.Rename(result);
		}
	}
}
