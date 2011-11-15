using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.SharpDevelop.Gui
{
    public class SaveEventArgs : System.EventArgs, ISaveEventArgs
    {
        bool successful;

        public bool Successful
        {
            get
            {
                return successful;
            }
        }

        public SaveEventArgs(bool successful)
        {
            this.successful = successful;
        }
    }
}
