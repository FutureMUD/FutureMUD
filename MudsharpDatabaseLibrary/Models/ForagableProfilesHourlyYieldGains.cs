using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ForagableProfilesHourlyYieldGains
    {
        public long ForagableProfileId { get; set; }
        public int ForagableProfileRevisionNumber { get; set; }
        public string ForageType { get; set; }
        public double Yield { get; set; }

        public virtual ForagableProfile ForagableProfile { get; set; }
    }
}
