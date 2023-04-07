using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class MaterialsTags
    {
        public long MaterialId { get; set; }
        public long TagId { get; set; }

        public virtual Material Material { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
