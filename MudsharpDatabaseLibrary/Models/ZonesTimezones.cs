using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ZonesTimezones
    {
        public long ZoneId { get; set; }
        public long ClockId { get; set; }
        public long TimezoneId { get; set; }

        public virtual Zone Zone { get; set; }
    }
}
