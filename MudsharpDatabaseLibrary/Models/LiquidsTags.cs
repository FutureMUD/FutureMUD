using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class LiquidsTags
    {
        public long LiquidId { get; set; }
        public long TagId { get; set; }

        public virtual Liquid Liquid { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
