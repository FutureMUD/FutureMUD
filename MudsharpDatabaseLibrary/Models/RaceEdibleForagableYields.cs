using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RaceEdibleForagableYields
    {
        public long RaceId { get; set; }
        public string YieldType { get; set; }
        public double BiteYield { get; set; }
        public double CaloriesPerYield { get; set; }
        public double HungerPerYield { get; set; }
        public double WaterPerYield { get; set; }
        public double ThirstPerYield { get; set; }
        public double AlcoholPerYield { get; set; }
        public string EatEmote { get; set; }

        public virtual Race Race { get; set; }
    }
}
