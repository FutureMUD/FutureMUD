using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CraftInput
    {
        public long Id { get; set; }
        public long CraftId { get; set; }
        public int CraftRevisionNumber { get; set; }
        public string InputType { get; set; }
        public double InputQualityWeight { get; set; }
        public string Definition { get; set; }
        public DateTime OriginalAdditionTime { get; set; }

        public virtual Craft Craft { get; set; }
    }
}
