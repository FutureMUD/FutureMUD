using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RacesEdibleMaterials
    {
        public long RaceId { get; set; }
        public long MaterialId { get; set; }
        public double CaloriesPerKilogram { get; set; }
        public double HungerPerKilogram { get; set; }
        public double ThirstPerKilogram { get; set; }
        public double WaterPerKilogram { get; set; }
        public double AlcoholPerKilogram { get; set; }

        public virtual Material Material { get; set; }
        public virtual Race Race { get; set; }
    }
}
