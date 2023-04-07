using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CulturesChargenResources
    {
        public long CultureId { get; set; }
        public long ChargenResourceId { get; set; }
        public int Amount { get; set; }
        public bool RequirementOnly { get; set; }

        public virtual ChargenResource ChargenResource { get; set; }
        public virtual Culture Culture { get; set; }
    }
}
