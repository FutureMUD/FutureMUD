using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class UnitOfMeasure
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PrimaryAbbreviation { get; set; }
        public string Abbreviations { get; set; }
        public double BaseMultiplier { get; set; }
        public double PreMultiplierBaseOffset { get; set; }
        public double PostMultiplierBaseOffset { get; set; }
        public int Type { get; set; }
        public bool Describer { get; set; }
        public bool SpaceBetween { get; set; }
        public string System { get; set; }
        public bool DefaultUnitForSystem { get; set; }
    }
}
