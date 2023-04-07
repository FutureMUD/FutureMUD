using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class SkyDescriptionTemplatesValue
    {
        public long SkyDescriptionTemplateId { get; set; }
        public double LowerBound { get; set; }
        public double UpperBound { get; set; }
        public string Description { get; set; }

        public virtual SkyDescriptionTemplate SkyDescriptionTemplate { get; set; }
    }
}
