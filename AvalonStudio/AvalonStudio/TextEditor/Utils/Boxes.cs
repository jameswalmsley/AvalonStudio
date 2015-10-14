using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonStudio.TextEditor.Utils
{
    /// <summary>
    /// Reuse the same instances for boxed booleans.
    /// </summary>
    static class Boxes
    {
        public static readonly object True = true;
        public static readonly object False = false;

        public static object Box(bool value)
        {
            return value ? True : False;
        }
    }

}
