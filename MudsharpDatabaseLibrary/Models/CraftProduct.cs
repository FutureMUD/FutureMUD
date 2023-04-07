using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CraftProduct
    {
        public long Id { get; set; }
        public long CraftId { get; set; }
        public int CraftRevisionNumber { get; set; }
        public string ProductType { get; set; }
        public string Definition { get; set; }
        public DateTime OriginalAdditionTime { get; set; }
        public bool IsFailProduct { get; set; }
        public int? MaterialDefiningInputIndex { get; set; }

        public virtual Craft Craft { get; set; }
    }
}
