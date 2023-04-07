using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class GasesTags
    {
        public long GasId { get; set; }
        public long TagId { get; set; }

        public virtual Gas Gas { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
