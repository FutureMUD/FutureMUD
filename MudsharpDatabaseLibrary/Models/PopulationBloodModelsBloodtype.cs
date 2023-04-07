using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class PopulationBloodModelsBloodtype
    {
        public long BloodtypeId { get; set; }
        public long PopulationBloodModelId { get; set; }
        public double Weight { get; set; }

        public virtual Bloodtype Bloodtype { get; set; }
        public virtual PopulationBloodModel PopulationBloodModel { get; set; }
    }
}
