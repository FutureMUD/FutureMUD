using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ItemGroupForm
    {
        public long Id { get; set; }
        public long ItemGroupId { get; set; }
        public string Type { get; set; }
        public string Definition { get; set; }

        public virtual ItemGroup ItemGroup { get; set; }
    }
}
