﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.XamlBinding.PowerToys.Dialogs
{
	/// <summary>
	/// Interaction logic for EditGridColumnsAndRowsDialog.xaml
	/// </summary>
	public partial class EditGridColumnsAndRowsDialog : Window
	{
		static readonly XName rowDefsName = XName.Get("Grid.RowDefinitions", CompletionDataHelper.WpfXamlNamespace);
		static readonly XName colDefsName = XName.Get("Grid.ColumnDefinitions", CompletionDataHelper.WpfXamlNamespace);
		
		static readonly XName rowDefName = XName.Get("RowDefinition", CompletionDataHelper.WpfXamlNamespace);
		static readonly XName colDefName = XName.Get("ColumnDefinition", CompletionDataHelper.WpfXamlNamespace);
		
		XElement gridTree;
		XElement rowDefitions;
		XElement colDefitions;
		IList<XElement> additionalProperties;
		
		bool gridLengthInvalid;
		
		class UndoStep
		{
			public XElement Tree { get; set; }
			public XElement RowDefinitions { get; set; }
			public XElement ColumnDefinitions { get; set; }
			public IList<XElement> AdditionalProperties { get; set; }
			
			public static UndoStep CreateStep(XElement tree, XElement rows, XElement cols, IEnumerable<XElement> properties)
			{
				XElement rowCopy = new XElement(rows);
				XElement colCopy = new XElement(cols);
				XElement treeCopy = new XElement(tree);
				
				IList<XElement> propertiesCopy = properties.Select(item => new XElement(item)).ToList();
				
				return new UndoStep() {
					Tree = treeCopy,
					RowDefinitions = rowCopy,
					ColumnDefinitions = colCopy,
					AdditionalProperties = propertiesCopy
				};
			}
		}
		
		Stack<UndoStep> undoStack;
		Stack<UndoStep> redoStack;
		
		public EditGridColumnsAndRowsDialog(XElement gridTree)
		{
			InitializeComponent();
			
			this.gridTree = gridTree;
			this.rowDefitions = gridTree.Element(rowDefsName) ?? new XElement(rowDefsName);
			this.colDefitions = gridTree.Element(colDefsName) ?? new XElement(colDefsName);
			
			if (this.rowDefitions.Parent != null)
				this.rowDefitions.Remove();
			if (this.colDefitions.Parent != null)
				this.colDefitions.Remove();
			
			this.rowDefitions
				.Elements()
				.Select(row => row.Attribute("Height"))
				.ForEach(
					height => {
						if (height.Value.Trim() == "1*")
							height.Value = "*";
						else
							height.Value = height.Value.Trim();
					}
				);
			
			this.colDefitions
				.Elements()
				.Select(col => col.Attribute("Width"))
				.ForEach(
					width => {
						if (width.Value.Trim() == "1*")
							width.Value = "*";
						else
							width.Value = width.Value.Trim();
					}
				);
			
			this.additionalProperties = gridTree.Elements().Where(e => e.Name.LocalName.Contains(".")).ToList();
			this.additionalProperties.ForEach(item => { if (item.Parent != null) item.Remove(); });
			
			this.redoStack = new Stack<UndoStep>();
			this.undoStack = new Stack<UndoStep>();
			
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, delegate { UndoItemClick(null, null); }));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, delegate { RedoItemClick(null, null); }));
			
			RebuildGrid();
		}
		
		static MenuItem CreateItem(string header, Action<StackPanel> clickAction, StackPanel senderItem)
		{
			MenuItem item = new MenuItem();
			
			item.Header = header;
			item.Click += delegate { clickAction(senderItem); };
			
			return item;
		}
		
		void InsertAbove(StackPanel block)
		{
			UpdateUndoRedoState();
			
			int row = (int)block.GetValue(Grid.RowProperty);
			
			var newRow = new XElement(rowDefName);
			newRow.SetAttributeValue(XName.Get("Height"), "*");
			var items = rowDefitions.Elements().Skip(row);
			var selItem = items.FirstOrDefault();
			if (selItem != null)
				selItem.AddBeforeSelf(newRow);
			else
				rowDefitions.Add(newRow);
			
			var controls = gridTree
				.Elements()
				.Where(
					element => {
						var rowAttrib = element.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						int rowAttribValue = 0;
						if (int.TryParse(rowAttrib.Value, out rowAttribValue))
							return rowAttribValue >= row;
						
						return false;
					}
				);
			
			controls.ForEach(
				item => {
					var rowAttrib = item.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
					item.SetAttributeValue(XName.Get("Grid.Row"), int.Parse(rowAttrib.Value, CultureInfo.InvariantCulture) + 1);
				}
			);
			
			RebuildGrid();
		}
		
		void InsertBelow(StackPanel block)
		{
			UpdateUndoRedoState();
			
			int row = (int)block.GetValue(Grid.RowProperty);
			
			var newRow = new XElement(rowDefName);
			newRow.SetAttributeValue(XName.Get("Height"), "*");
			var items = rowDefitions.Elements().Skip(row);
			var selItem = items.FirstOrDefault();
			if (selItem != null)
				selItem.AddAfterSelf(newRow);
			else
				rowDefitions.Add(newRow);
			
			var controls = gridTree
				.Elements()
				.Where(
					element => {
						var rowAttrib = element.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						int rowAttribValue = 0;
						if (int.TryParse(rowAttrib.Value, out rowAttribValue))
							return rowAttribValue > row;
						
						return false;
					}
				);
			
			controls.ForEach(
				item => {
					var rowAttrib = item.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
					item.SetAttributeValue(XName.Get("Grid.Row"), int.Parse(rowAttrib.Value, CultureInfo.InvariantCulture) + 1);
				}
			);
			
			RebuildGrid();
		}
		
		void MoveUp(StackPanel block)
		{
			int row = (int)block.GetValue(Grid.RowProperty);
			if (row > 0) {
				UpdateUndoRedoState();
				
				var items = rowDefitions.Elements().Skip(row);
				var selItem = items.FirstOrDefault();
				if (selItem == null)
					return;
				selItem.Remove();
				items = rowDefitions.Elements().Skip(row - 1);
				var before = items.FirstOrDefault();
				if (before == null)
					return;
				before.AddBeforeSelf(selItem);
				
				var controls = gridTree
					.Elements()
					.Where(
						element => {
							var rowAttrib = element.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
							int rowAttribValue = 0;
							if (int.TryParse(rowAttrib.Value, out rowAttribValue))
								return rowAttribValue == row;
							
							return false;
						}
					).ToList();
				
				var controlsDown = gridTree
					.Elements()
					.Where(
						element2 => {
							var rowAttrib = element2.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
							int rowAttribValue = 0;
							if (int.TryParse(rowAttrib.Value, out rowAttribValue))
								return rowAttribValue == (row - 1);
							
							return false;
						}
					).ToList();
				
				controls.ForEach(
					item => {
						var rowAttrib = item.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						item.SetAttributeValue(XName.Get("Grid.Row"), int.Parse(rowAttrib.Value, CultureInfo.InvariantCulture) - 1);
					}
				);
				
				controlsDown.ForEach(
					item2 => {
						var rowAttrib = item2.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						item2.SetAttributeValue(XName.Get("Grid.Row"), int.Parse(rowAttrib.Value, CultureInfo.InvariantCulture) + 1);
					}
				);
				
				RebuildGrid();
			}
		}
		
		void MoveDown(StackPanel block)
		{
			int row = (int)block.GetValue(Grid.RowProperty);
			if (row < rowDefitions.Elements().Count() - 1) {
				UpdateUndoRedoState();

				var items = rowDefitions.Elements().Skip(row);
				var selItem = items.FirstOrDefault();
				if (selItem == null)
					return;
				selItem.Remove();
				items = rowDefitions.Elements().Skip(row - 1);
				var before = items.FirstOrDefault();
				if (before == null)
					return;
				before.AddBeforeSelf(selItem);
				
				var controls = gridTree
					.Elements()
					.Where(
						element => {
							var rowAttrib = element.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
							int rowAttribValue = 0;
							if (int.TryParse(rowAttrib.Value, out rowAttribValue))
								return rowAttribValue == row;
							
							return false;
						}
					).ToList();
				
				var controlsUp = gridTree
					.Elements()
					.Where(
						element2 => {
							var rowAttrib = element2.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
							int rowAttribValue = 0;
							if (int.TryParse(rowAttrib.Value, out rowAttribValue))
								return rowAttribValue == (row + 1);
							
							return false;
						}
					).ToList();
				
				controls.ForEach(
					item => {
						var rowAttrib = item.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						item.SetAttributeValue(XName.Get("Grid.Row"), int.Parse(rowAttrib.Value, CultureInfo.InvariantCulture) + 1);
					}
				);
				
				controlsUp.ForEach(
					item2 => {
						var rowAttrib = item2.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						item2.SetAttributeValue(XName.Get("Grid.Row"), int.Parse(rowAttrib.Value, CultureInfo.InvariantCulture) - 1);
					}
				);
				
				RebuildGrid();
			}
		}
		
		void DeleteRow(StackPanel block)
		{
			int row = (int)block.GetValue(Grid.RowProperty);
			UpdateUndoRedoState();

			var items = rowDefitions.Elements().Skip(row);
			var selItem = items.FirstOrDefault();
			if (selItem != null)
				selItem.Remove();
			
			var controls = gridTree
				.Elements()
				.Where(
					element => {
						var rowAttrib = element.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						int rowAttribValue = 0;
						if (int.TryParse(rowAttrib.Value, out rowAttribValue))
							return rowAttribValue >= row;
						
						return false;
					}
				);
			
			controls.ForEach(
				item => {
					var rowAttrib = item.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
					item.SetAttributeValue(XName.Get("Grid.Row"), int.Parse(rowAttrib.Value, CultureInfo.InvariantCulture) - 1);
				}
			);
			
			RebuildGrid();
		}
		
		void InsertBefore(StackPanel block)
		{
			int column = (int)block.GetValue(Grid.ColumnProperty);
			UpdateUndoRedoState();

			var newColumn = new XElement(colDefName);
			newColumn.SetAttributeValue(XName.Get("Width"), "*");
			var items = colDefitions.Elements().Skip(column);
			var selItem = items.FirstOrDefault();
			if (selItem != null)
				selItem.AddBeforeSelf(newColumn);
			else
				colDefitions.Add(newColumn);
			
			var controls = gridTree
				.Elements()
				.Where(
					element => {
						var colAttrib = element.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						int colAttribValue = 0;
						if (int.TryParse(colAttrib.Value, out colAttribValue))
							return colAttribValue >= column;
						
						return false;
					}
				);
			
			controls.ForEach(
				item => {
					var colAttrib = item.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
					item.SetAttributeValue(XName.Get("Grid.Column"), int.Parse(colAttrib.Value, CultureInfo.InvariantCulture) + 1);
				}
			);
			
			RebuildGrid();
		}
		
		void InsertAfter(StackPanel block)
		{
			int column = (int)block.GetValue(Grid.ColumnProperty);
			UpdateUndoRedoState();

			var newColumn = new XElement(colDefName);
			newColumn.SetAttributeValue(XName.Get("Width"), "*");
			var items = colDefitions.Elements().Skip(column);
			var selItem = items.FirstOrDefault();
			if (selItem != null)
				selItem.AddAfterSelf(newColumn);
			else
				colDefitions.Add(newColumn);
			
			var controls = gridTree
				.Elements()
				.Where(
					element => {
						var colAttrib = element.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						int colAttribValue = 0;
						if (int.TryParse(colAttrib.Value, out colAttribValue))
							return colAttribValue > column;
						
						return false;
					}
				);
			
			controls.ForEach(
				item => {
					var colAttrib = item.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
					item.SetAttributeValue(XName.Get("Grid.Column"), int.Parse(colAttrib.Value, CultureInfo.InvariantCulture) + 1);
				}
			);
			
			RebuildGrid();
		}
		
		void MoveLeft(StackPanel block)
		{
			int column = (int)block.GetValue(Grid.ColumnProperty);
			
			if (column > 0) {
				UpdateUndoRedoState();
				
				var items = colDefitions.Elements().Skip(column);
				var selItem = items.FirstOrDefault();
				if (selItem == null)
					return;
				selItem.Remove();
				items = colDefitions.Elements().Skip(column - 1);
				var before = items.FirstOrDefault();
				if (before == null)
					return;
				before.AddBeforeSelf(selItem);
				
				var controls = gridTree
					.Elements()
					.Where(
						element => {
							var colAttrib = element.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
							int colAttribValue = 0;
							if (int.TryParse(colAttrib.Value, out colAttribValue))
								return colAttribValue == column;
							
							return false;
						}
					).ToList();
				
				var controlsLeft = gridTree
					.Elements()
					.Where(
						element2 => {
							var colAttrib = element2.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
							int colAttribValue = 0;
							if (int.TryParse(colAttrib.Value, out colAttribValue))
								return colAttribValue == (column - 1);
							
							return false;
						}
					).ToList();
				
				controls.ForEach(
					item => {
						var colAttrib = item.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						item.SetAttributeValue(XName.Get("Grid.Column"), int.Parse(colAttrib.Value, CultureInfo.InvariantCulture) - 1);
					}
				);
				
				controlsLeft.ForEach(
					item2 => {
						var colAttrib = item2.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						item2.SetAttributeValue(XName.Get("Grid.Column"), int.Parse(colAttrib.Value, CultureInfo.InvariantCulture) + 1);
					}
				);
				
				RebuildGrid();
			}
		}
		
		void MoveRight(StackPanel block)
		{
			int column = (int)block.GetValue(Grid.ColumnProperty);
			
			if (column < colDefitions.Elements().Count() - 1) {
				UpdateUndoRedoState();
				
				var items = colDefitions.Elements().Skip(column);
				var selItem = items.FirstOrDefault();
				if (selItem == null)
					return;
				selItem.Remove();
				items = colDefitions.Elements().Skip(column - 1);
				var before = items.FirstOrDefault();
				if (before == null)
					return;
				before.AddBeforeSelf(selItem);
				
				var controls = gridTree
					.Elements()
					.Where(
						element => {
							var colAttrib = element.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
							int colAttribValue = 0;
							if (int.TryParse(colAttrib.Value, out colAttribValue))
								return colAttribValue == column;
							
							return false;
						}
					).ToList();
				
				var controlsRight = gridTree
					.Elements()
					.Where(
						element2 => {
							var colAttrib = element2.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
							int colAttribValue = 0;
							if (int.TryParse(colAttrib.Value, out colAttribValue))
								return colAttribValue == (column + 1);
							
							return false;
						}
					).ToList();
				
				controls.ForEach(
					item => {
						var colAttrib = item.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						item.SetAttributeValue(XName.Get("Grid.Column"), int.Parse(colAttrib.Value, CultureInfo.InvariantCulture) + 1);
					}
				);
				
				controlsRight.ForEach(
					item2 => {
						var colAttrib = item2.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						item2.SetAttributeValue(XName.Get("Grid.Column"), int.Parse(colAttrib.Value, CultureInfo.InvariantCulture) - 1);
					}
				);
				
				RebuildGrid();
			}
		}
		
		void DeleteColumn(StackPanel block)
		{
			int column = (int)block.GetValue(Grid.ColumnProperty);
			UpdateUndoRedoState();

			var items = colDefitions.Elements().Skip(column);
			var selItem = items.FirstOrDefault();
			if (selItem != null)
				selItem.Remove();
			
			var controls = gridTree
				.Elements()
				.Where(
					element => {
						var colAttrib = element.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						int colAttribValue = 0;
						if (int.TryParse(colAttrib.Value, out colAttribValue))
							return colAttribValue >= column;
						
						return false;
					}
				);
			
			controls.ForEach(
				item => {
					var colAttrib = item.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
					item.SetAttributeValue(XName.Get("Grid.Column"), int.Parse(colAttrib.Value, CultureInfo.InvariantCulture) - 1);
				}
			);
			
			RebuildGrid();
		}
		
		void BtnCancelClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
		
		void BtnOKClick(object sender, RoutedEventArgs e)
		{
			if (gridLengthInvalid) {
				MessageService.ShowError("Grid is invalid, please check the row heights and column widths!");
				return;
			}
			
			this.DialogResult = true;
		}
		
		void RebuildGrid()
		{
			this.marker = null;
			this.gridDisplay.Children.Clear();
			this.gridDisplay.RowDefinitions.Clear();
			this.gridDisplay.ColumnDefinitions.Clear();
			
			this.columnWidthGrid.ColumnDefinitions.Clear();
			this.columnWidthGrid.Children.Clear();
			
			this.rowHeightGrid.RowDefinitions.Clear();
			this.rowHeightGrid.Children.Clear();
			
			int rows = rowDefitions.Elements().Count();
			int cols = colDefitions.Elements().Count();
			
			if (rows == 0) {
				rowDefitions.Add(new XElement(rowDefName).AddAttribute("Height", "*"));
				rows = 1;
			}
			if (cols == 0) {
				colDefitions.Add(new XElement(colDefName).AddAttribute("Width", "*"));
				cols = 1;
			}
			
			for (int i = 0; i < cols; i++) {
				this.gridDisplay.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
				this.columnWidthGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
				GridLengthEditor editor = new GridLengthEditor(Orientation.Horizontal, i, (colDefitions.Elements().ElementAt(i).Attribute("Width") ?? new XAttribute("Width", "")).Value);
				editor.SelectedValueChanged += new EventHandler<GridLengthSelectionChangedEventArgs>(EditorSelectedValueChanged);
				editor.Deleted += new EventHandler<GridLengthSelectionChangedEventArgs>(EditorDeleted);
				editor.Added += new EventHandler<GridLengthSelectionChangedEventArgs>(EditorAdded);
				this.columnWidthGrid.Children.Add(editor);
			}
			
			for (int i = 0; i < rows; i++) {
				this.gridDisplay.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
				
				this.rowHeightGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
				GridLengthEditor editor = new GridLengthEditor(Orientation.Vertical, i, (rowDefitions.Elements().ElementAt(i).Attribute("Height") ?? new XAttribute("Height", "")).Value);
				editor.SelectedValueChanged += new EventHandler<GridLengthSelectionChangedEventArgs>(EditorSelectedValueChanged);
				editor.Deleted += new EventHandler<GridLengthSelectionChangedEventArgs>(EditorDeleted);
				editor.Added += new EventHandler<GridLengthSelectionChangedEventArgs>(EditorAdded);
				this.rowHeightGrid.Children.Add(editor);
				
				for (int j = 0; j < cols; j++) {
					StackPanel displayRect = new StackPanel() {
						Margin = new Thickness(5),
						Background = Brushes.LightGray,
						Orientation = Orientation.Vertical
					};
					
					displayRect.AllowDrop = true;
					
					displayRect.Drop += new DragEventHandler(DisplayRectDrop);
					displayRect.DragOver += new DragEventHandler(DisplayRectDragOver);
					
					displayRect.Children.AddRange(BuildItemsForCell(i, j));
					
					displayRect.SetValue(Grid.RowProperty, i);
					displayRect.SetValue(Grid.ColumnProperty, j);
					
					displayRect.ContextMenuOpening += new ContextMenuEventHandler(DisplayRectContextMenuOpening);
					
					this.gridDisplay.Children.Add(displayRect);
				}
			}
			
			this.InvalidateVisual();
		}

		void EditorAdded(object sender, GridLengthSelectionChangedEventArgs e)
		{
			if (e.Type == Orientation.Horizontal)
				InsertBefore(gridDisplay.Children.OfType<StackPanel>().First(item => (int)item.GetValue(Grid.ColumnProperty) == e.Cell));
			else
				InsertAbove(gridDisplay.Children.OfType<StackPanel>().First(item => (int)item.GetValue(Grid.RowProperty) == e.Cell));
		}

		void EditorDeleted(object sender, GridLengthSelectionChangedEventArgs e)
		{
			if (e.Type == Orientation.Horizontal)
				DeleteColumn(gridDisplay.Children.OfType<StackPanel>().First(item => (int)item.GetValue(Grid.ColumnProperty) == e.Cell));
			else
				DeleteRow(gridDisplay.Children.OfType<StackPanel>().First(item => (int)item.GetValue(Grid.RowProperty) == e.Cell));
		}

		void EditorSelectedValueChanged(object sender, GridLengthSelectionChangedEventArgs e)
		{
			UpdateUndoRedoState();
			
			string value = "Invalid";
			
			if (e.Value.HasValue) {
				if (e.Value.Value.IsAuto)
					value = "Auto";
				if (e.Value.Value.IsStar)
					value = (e.Value.Value.Value == 1) ? "*" : e.Value.Value.Value + "*";
				if (e.Value.Value.IsAbsolute)
					value = e.Value.Value.Value + "px";
			}

			if (e.Type == Orientation.Horizontal)
				colDefitions.Elements().ElementAt(e.Cell).SetAttributeValue("Width", value);
			else
				rowDefitions.Elements().ElementAt(e.Cell).SetAttributeValue("Height", value);
			
			gridLengthInvalid = colDefitions.Elements().Any(col => (col.Attribute("Width") ?? new XAttribute("Width", "*")).Value == "Invalid")
				|| rowDefitions.Elements().Any(row => (row.Attribute("Height") ?? new XAttribute("Height", "*")).Value == "Invalid");
		}
		
		class DragDropMarkerAdorner : Adorner
		{
			DragDropMarkerAdorner(UIElement adornedElement)
				: base(adornedElement)
			{
			}
			
			Point start, end;
			
			protected override void OnRender(DrawingContext drawingContext)
			{
				base.OnRender(drawingContext);
				
				drawingContext.DrawLine(new Pen(Brushes.Black, 1), start, end);
			}
			
			protected override Size MeasureOverride(Size constraint)
			{
				return new Size(1, 1); // dummy values
			}
			
			protected override Size ArrangeOverride(Size finalSize)
			{
				return new Size(1, 1); // dummy values
			}
			
			public static DragDropMarkerAdorner CreateAdorner(StackPanel panel, FrameworkElement aboveElement)
			{
				DragDropMarkerAdorner adorner;
				
				if (aboveElement is StackPanel) {
					aboveElement = (panel.Children.Count > 0 ? panel.Children[panel.Children.Count - 1] : panel) as FrameworkElement;
					adorner = new DragDropMarkerAdorner(aboveElement);
					adorner.start = new Point(5, 5 + aboveElement.DesiredSize.Height);
					adorner.end = new Point(panel.ActualWidth - 10, 5 + aboveElement.DesiredSize.Height);
				} else {
					aboveElement = aboveElement.TemplatedParent as FrameworkElement;
					adorner = new DragDropMarkerAdorner(aboveElement);
					adorner.start = new Point(5, 0);
					adorner.end = new Point(panel.ActualWidth - 10, 0);
				}
				
				AdornerLayer.GetAdornerLayer(aboveElement).Add(adorner);
				
				return adorner;
			}
		}
		
		DragDropMarkerAdorner marker = null;

		void DisplayRectDragOver(object sender, DragEventArgs e)
		{
			StackPanel target = sender as StackPanel;
			
			if (marker != null) {
				AdornerLayer.GetAdornerLayer(marker.AdornedElement).Remove(marker);
				marker = null;
			}
			
			if (target != null) {
				Point p = e.GetPosition(target);
				FrameworkElement element = target.InputHitTest(p) as FrameworkElement;
				
				if (element is StackPanel || element.TemplatedParent is Label) {
					marker = DragDropMarkerAdorner.CreateAdorner(target, element);

					e.Effects = DragDropEffects.Move;
					e.Handled = true;
				} else {
					e.Effects = DragDropEffects.None;
					e.Handled =  true;
				}
			}
		}

		void DisplayRectDrop(object sender, DragEventArgs e)
		{
			try {
				XElement data = e.Data.GetData(typeof(XElement)) as XElement;
				if (data != null) {
					UpdateUndoRedoState();
					
					StackPanel target = sender as StackPanel;
					int x = (int)target.GetValue(Grid.ColumnProperty);
					int y = (int)target.GetValue(Grid.RowProperty);
					
					data.SetAttributeValue(XName.Get("Grid.Column"), x);
					data.SetAttributeValue(XName.Get("Grid.Row"), y);
					
					Point p = e.GetPosition(target);
					TextBlock block = target.InputHitTest(p) as TextBlock;
					
					if (block != null) {
						XElement element = block.Tag as XElement;
						data.MoveBefore(element);
					} else {
						XElement parent = gridTree;
						XElement element = parent.Elements().LastOrDefault();
						if (data.Parent != null)
							data.Remove();
						if (element == null)
							parent.Add(data);
						else {
							if (element.Parent == null)
								parent.Add(data);
							else
								element.AddAfterSelf(data);
						}
					}
					
					RebuildGrid();
				}
			} catch (InvalidOperationException ex) {
				Core.LoggingService.Error(ex);
			}
		}
		
		IEnumerable<UIElement> BuildItemsForCell(int row, int column)
		{
			var controls = gridTree
				.Elements()
				.Where(
					element => {
						var rowAttrib = element.Attribute(XName.Get("Grid.Row")) ?? new XAttribute(XName.Get("Grid.Row"), 0);
						var colAttrib = element.Attribute(XName.Get("Grid.Column")) ?? new XAttribute(XName.Get("Grid.Column"), 0);
						return 	row.ToString() == rowAttrib.Value && column.ToString() == colAttrib.Value;
					}
				);
			
			foreach (var control in controls) {
				var nameAttrib = control.Attribute(XName.Get("Name", CompletionDataHelper.XamlNamespace)) ?? control.Attribute(XName.Get("Name"));
				StringBuilder builder = new StringBuilder(control.Name.LocalName);
				if (nameAttrib != null)
					builder.Append(" (" + nameAttrib.Value + ")");
				
				Label label = new Label() {
					Content = builder.ToString(),
					Template = this.Resources["itemTemplate"] as ControlTemplate,
					AllowDrop = true,
					Tag = control
				};
				
				label.MouseLeftButtonDown += new MouseButtonEventHandler(LabelMouseLeftButtonDown);

				Debug.Assert(label.Tag != null);
				
				yield return label;
			}
		}

		void LabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragDropEffects allowedEffects = DragDropEffects.Move;
			Label label = sender as Label;
			DragDrop.DoDragDrop(label, label.Tag, allowedEffects);
		}
		
		void UpdateUndoRedoState()
		{
			this.undoStack.Push(UndoStep.CreateStep(gridTree, rowDefitions, colDefitions, additionalProperties));
			this.redoStack.Clear();
		}
		
		public XElement ConstructedTree
		{
			get {
				gridTree.AddFirst(additionalProperties);
				gridTree.AddFirst(colDefitions);
				gridTree.AddFirst(rowDefitions);
				
				return gridTree;
			}
		}

		void DisplayRectContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			MenuItem undoItem = new MenuItem();
			undoItem.Header = "Undo";
			undoItem.IsEnabled = undoStack.Count > 0;
			undoItem.Click += new RoutedEventHandler(UndoItemClick);
			
			MenuItem redoItem = new MenuItem();
			redoItem.Header = "Redo";
			redoItem.IsEnabled = redoStack.Count > 0;
			redoItem.Click += new RoutedEventHandler(RedoItemClick);
			
			StackPanel block = sender as StackPanel;
			
			ContextMenu menu = new ContextMenu() {
				Items = {
					undoItem,
					redoItem,
					new Separator(),
					new MenuItem() {
						Header = "Row",
						Items = {
							CreateItem("Insert above", InsertAbove, block),
							CreateItem("Insert below", InsertBelow, block),
							new Separator(),
							CreateItem("Move up", MoveUp, block),
							CreateItem("Move down", MoveDown, block),
							new Separator(),
							CreateItem("Delete", DeleteRow, block)
						}
					},
					new MenuItem() {
						Header = "Column",
						Items = {
							CreateItem("Insert before", InsertBefore, block),
							CreateItem("Insert after", InsertAfter, block),
							new Separator(),
							CreateItem("Move left", MoveLeft, block),
							CreateItem("Move right", MoveRight, block),
							new Separator(),
							CreateItem("Delete", DeleteColumn, block)
						}
					}
				}
			};
			
			menu.IsOpen = true;
		}

		void RedoItemClick(object sender, RoutedEventArgs e)
		{
			if (redoStack.Count > 0)
				HandleSteps(redoStack, undoStack);
		}

		void UndoItemClick(object sender, RoutedEventArgs e)
		{
			if (undoStack.Count > 0)
				HandleSteps(undoStack, redoStack);
		}
		
		void HandleSteps(Stack<UndoStep> stack1, Stack<UndoStep> stack2)
		{
			UndoStep step = stack1.Pop();
			
			stack2.Push(UndoStep.CreateStep(gridTree, rowDefitions, colDefitions, additionalProperties));
			
			this.additionalProperties = step.AdditionalProperties;
			this.rowDefitions = step.RowDefinitions;
			this.colDefitions = step.ColumnDefinitions;
			this.gridTree = step.Tree;
			
			RebuildGrid();
		}
		
		void BtnDeleteItemClick(object sender, RoutedEventArgs e)
		{
			Button source = e.OriginalSource as Button;
			XElement item = source.Tag as XElement;
			if (item != null) {
				UpdateUndoRedoState();
				item.Remove();
			}
			
			RebuildGrid();
		}
		
		void BtnAddRowClick(object sender, RoutedEventArgs e)
		{
			InsertBelow(gridDisplay.Children.OfType<StackPanel>().First(item => (int)item.GetValue(Grid.RowProperty) == rowDefitions.Elements().Count() - 1));
		}
		
		void BtnAddColumnClick(object sender, RoutedEventArgs e)
		{
			InsertAfter(gridDisplay.Children.OfType<StackPanel>().First(item => (int)item.GetValue(Grid.ColumnProperty) == colDefitions.Elements().Count() - 1));
		}
	}
}