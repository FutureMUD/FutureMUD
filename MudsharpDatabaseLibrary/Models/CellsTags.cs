using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CellsTags
    {
        public long CellId { get; set; }
        public long TagId { get; set; }

        public virtual Cell Cell { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
