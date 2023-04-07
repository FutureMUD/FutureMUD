using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CellOverlayExit
    {
        public long CellOverlayId { get; set; }
        public long ExitId { get; set; }

        public virtual CellOverlay CellOverlay { get; set; }
        public virtual Exit Exit { get; set; }
    }
}
