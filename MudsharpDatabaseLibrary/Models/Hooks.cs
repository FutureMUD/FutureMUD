using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Hooks
    {
        public Hooks()
        {
            DefaultHooks = new HashSet<DefaultHook>();
            HooksPerceivables = new HashSet<HooksPerceivable>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public int TargetEventType { get; set; }

        public virtual ICollection<DefaultHook> DefaultHooks { get; set; }
        public virtual ICollection<HooksPerceivable> HooksPerceivables { get; set; }
    }
}
