using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ProgSchedule
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int IntervalType { get; set; }
        public int IntervalModifier { get; set; }
        public int IntervalOther { get; set; }
        public string ReferenceTime { get; set; }
        public string ReferenceDate { get; set; }
        public long FutureProgId { get; set; }

        public virtual FutureProg FutureProg { get; set; }
    }
}
