using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Winforms.Controls
{
    /// <summary>
    /// The interface support control applies palette when drawing
    /// </summary>
    public interface ISupportPalette
    {
        PaletteMode PaletteMode { get; set; }
    }
}
