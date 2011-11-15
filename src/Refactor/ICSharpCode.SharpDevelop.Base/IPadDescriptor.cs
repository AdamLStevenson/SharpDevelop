using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
    public interface IPadDescriptor : IDisposable
    {
		/// <summary>
		/// Returns the title of the pad.
		/// </summary>
		string Title {
			get;
		}
		
		/// <summary>
		/// Returns the icon bitmap resource name of the pad. May be an empty string
		/// if the pad has no icon defined.
		/// </summary>
		string Icon {
			get ;
		}
		
		/// <summary>
		/// Returns the category (this is used for defining where the menu item to
		/// this pad goes)
		/// </summary>
		string Category {
			get;
			set;
		}
		
		/// <summary>
		/// Returns the menu shortcut for the view menu item.
		/// </summary>
		string Shortcut {
			get;
			set;
		}
		
		/// <summary>
		/// Gets the name of the pad class.
		/// </summary>
		string Class {
			get;
		}
		
		/// <summary>
		/// Gets/sets the default position of the pad.
		/// </summary>
		DefaultPadPositions DefaultPosition { get; set; }
		
		IPadContent PadContent {
			get;
		}
		
		void Dispose();
		
		void CreatePad();
		
		void BringPadToFront();
		
		
    }
}
