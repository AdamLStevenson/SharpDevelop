using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.WinForms
{
    /// <summary>
    /// This Class contains two ResourceManagers, which handle string and image resources
    /// for the application. It do handle localization strings on this level.
    /// </summary>
    public interface IWinFormsResourceService
    {
		
		
		#region Font loading
		
        Font DefaultMonospacedFont
        {
            get;
        }
		
		/// <summary>
		/// Loads the default monospaced font (Consolas or Courier New).
		/// </summary>
        Font LoadDefaultMonospacedFont(FontStyle style);
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="fontName">The name of the font to load.</param>
		/// <param name="size">The size of the font to load.</param>
		/// <returns>
		/// The font to load or the menu font, if the requested font couldn't be loaded.
		/// </returns>
        Font LoadFont(string fontName, int size);
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="fontName">The name of the font to load.</param>
		/// <param name="size">The size of the font to load.</param>
		/// <param name="style">The <see cref="System.Drawing.FontStyle"/> of the font</param>
		/// <returns>
		/// The font to load or the menu font, if the requested font couldn't be loaded.
		/// </returns>
        Font LoadFont(string fontName, int size, FontStyle style);
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="fontName">The name of the font to load.</param>
		/// <param name="size">The size of the font to load.</param>
		/// <param name="unit">The <see cref="System.Drawing.GraphicsUnit"/> of the font</param>
		/// <returns>
		/// The font to load or the menu font, if the requested font couldn't be loaded.
		/// </returns>
        Font LoadFont(string fontName, int size, GraphicsUnit unit);
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="fontName">The name of the font to load.</param>
		/// <param name="size">The size of the font to load.</param>
		/// <param name="style">The <see cref="System.Drawing.FontStyle"/> of the font</param>
		/// <param name="unit">The <see cref="System.Drawing.GraphicsUnit"/> of the font</param>
		/// <returns>
		/// The font to load or the menu font, if the requested font couldn't be loaded.
		/// </returns>
        Font LoadFont(string fontName, int size, FontStyle style, GraphicsUnit unit);
		
		/// <summary>
		/// The LoadFont routines provide a safe way to load fonts.
		/// </summary>
		/// <param name="baseFont">The existing font from which to create the new font.</param>
		/// <param name="newStyle">The new style of the font.</param>
		/// <returns>
		/// The font to load or the baseFont (if the requested font couldn't be loaded).
		/// </returns>
        Font LoadFont(Font baseFont, FontStyle newStyle);

		#endregion
		
		/// <summary>
		/// Returns a icon from the resource database, it handles localization
		/// transparent for the user. In the resource database can be a bitmap
		/// instead of an icon in the dabase. It is converted automatically.
		/// </summary>
		/// <returns>
		/// The icon in the (localized) resource database, or null, if the icon cannot
		/// be found.
		/// </returns>
		/// <param name="name">
		/// The name of the requested icon.
		/// </param>
        Icon GetIcon(string name);
		
		/// <summary>
		/// Converts a bitmap into an icon.
		/// </summary>
        Icon BitmapToIcon(Bitmap bmp);
		
		/// <summary>
		/// Returns a bitmap from the resource database, it handles localization
		/// transparent for the user.
		/// The bitmaps are reused, you must not dispose the Bitmap!
		/// </summary>
		/// <returns>
		/// The bitmap in the (localized) resource database.
		/// </returns>
		/// <param name="name">
		/// The name of the requested bitmap.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
        Bitmap GetBitmap(string name);
	}
}
