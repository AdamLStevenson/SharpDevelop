// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Struct for strongly-typed passing of item types
	/// - we don't want to use strings everywhere.
	/// Basically this is something like a typedef for C# (without implicit conversions).
	/// </summary>
	public struct ItemType : IItemType
	{
		// ReferenceProjectItem
		public static readonly IItemType Reference = new ItemType("Reference");
		public static readonly IItemType ProjectReference = new ItemType("ProjectReference");
		public static readonly IItemType COMReference = new ItemType("COMReference");
		
		public static readonly ReadOnlyCollectionWrapper<IItemType> ReferenceItemTypes
			= new ReadOnlyCollectionWrapper<IItemType>(new IItemType[] { Reference, ProjectReference, COMReference });
		
		/// <summary>
		/// Item type for imported VB namespaces
		/// </summary>
		public static readonly IItemType Import = new ItemType("Import");
		
		public static readonly IItemType WebReferenceUrl = new ItemType("WebReferenceUrl");
		
		// FileProjectItem
		public static readonly IItemType Compile = new ItemType("Compile");
		public static readonly IItemType EmbeddedResource = new ItemType("EmbeddedResource");
		public static readonly IItemType None = new ItemType("None");
		public static readonly IItemType Content = new ItemType("Content");
		public static readonly IItemType ApplicationDefinition = new ItemType("ApplicationDefinition");
		public static readonly IItemType Page = new ItemType("Page");
		public static readonly IItemType BootstrapperFile = new ItemType("BootstrapperFile");
		public static readonly IItemType Header = new ItemType("Header");
		
		// vcxproj-only (c++ project) items
		public static readonly IItemType ClCompile = new ItemType("ClCompile");
		public static readonly IItemType ClInclude = new ItemType("ClInclude");
		
		/// <summary>
		/// Gets a collection of item types that are used for files.
		/// </summary>
		public static readonly ReadOnlyCollectionWrapper<IItemType> DefaultFileItems
			= new ReadOnlyCollectionWrapper<IItemType>(new IItemType[] { Compile, EmbeddedResource, None, Content });
		
		public static readonly IItemType Resource = new ItemType("Resource");
		public static readonly IItemType Folder = new ItemType("Folder");
		public static readonly IItemType WebReferences = new ItemType("WebReferences");
		
		/// <summary>
		/// Gets a collection of item types that are known not to be used for files.
		/// </summary>
		public static readonly ReadOnlyCollectionWrapper<IItemType> NonFileItemTypes
			= new ReadOnlyCollectionWrapper<IItemType>(new List<IItemType>(ReferenceItemTypes) {Folder, WebReferences, Import});
		
		readonly string itemName;
		
		public string ItemName {
			get { return itemName; }
		}
		
		public ItemType(string itemName)
		{
			if (itemName == null)
				throw new ArgumentNullException("itemName");
			this.itemName = itemName;
		}
		
		public override string ToString()
		{
			return itemName;
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			if (obj is ItemType)
				return Equals((ItemType)obj); // use Equals method below
			else
				return false;
		}
		
		public bool Equals(IItemType other)
		{
			return this.itemName == other.ItemName;
		}
		
		public override int GetHashCode()
		{
			return itemName.GetHashCode();
		}
		
		public static bool operator ==(ItemType lhs, ItemType rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(ItemType lhs, ItemType rhs)
		{
			return !(lhs.Equals(rhs)); // use operator == and negate result
		}

   

		#endregion
		
		public int CompareTo(IItemType other)
		{
			return itemName.CompareTo(other.ItemName);
		}
	}
}
