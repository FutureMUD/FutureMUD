using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ClimateModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Definition { get; set; }
        public int MinuteProcessingInterval { get; set; }
        public int MinimumMinutesBetweenFlavourEchoes { get; set; }
        public double MinuteFlavourEchoChance { get; set; }
    }
}
