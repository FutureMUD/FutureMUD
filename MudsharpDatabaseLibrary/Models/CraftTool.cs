using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CraftTool
    {
        public long Id { get; set; }
        public long CraftId { get; set; }
        public int CraftRevisionNumber { get; set; }
        public DateTime OriginalAdditionTime { get; set; }
        public string ToolType { get; set; }
        public double ToolQualityWeight { get; set; }
        public int DesiredState { get; set; }
        public string Definition { get; set; }
        public bool UseToolDuration { get; set; }

        public virtual Craft Craft { get; set; }
    }
}
