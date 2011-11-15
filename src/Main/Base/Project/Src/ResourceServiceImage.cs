using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Drawing;
using ICSharpCode.Core.Presentation;
using ICSharpCode.Core.WinForms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
    /// <summary>
    /// Represents an image that gets loaded from a ResourceService.
    /// </summary>
    public class ResourceServiceImage : IImage
    {
        readonly string resourceName;

        /// <summary>
        /// Creates a new ResourceServiceImage.
        /// </summary>
        /// <param name="resourceName">The name of the image resource.</param>
        public ResourceServiceImage(string resourceName)
        {
            if (resourceName == null)
                throw new ArgumentNullException("resourceName");
            this.resourceName = resourceName;
        }

        /// <inheritdoc/>
        public ImageSource ImageSource
        {
            get
            {
                return PresentationResourceService.GetBitmapSource(resourceName);
            }
        }

        /// <inheritdoc/>
        public Bitmap Bitmap
        {
            get
            {
                return WinFormsResourceService.GetBitmap(resourceName);
            }
        }

        /// <inheritdoc/>
        public Icon Icon
        {
            get
            {
                Icon icon = WinFormsResourceService.GetIcon(resourceName);
                if (icon == null)
                    throw new ResourceNotFoundException(resourceName);
                return icon;
            }
        }
    }
}
