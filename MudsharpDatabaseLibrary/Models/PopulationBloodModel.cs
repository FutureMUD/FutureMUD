using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class PopulationBloodModel
    {
        public PopulationBloodModel()
        {
            Ethnicities = new HashSet<Ethnicity>();
            PopulationBloodModelsBloodtypes = new HashSet<PopulationBloodModelsBloodtype>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Ethnicity> Ethnicities { get; set; }
        public virtual ICollection<PopulationBloodModelsBloodtype> PopulationBloodModelsBloodtypes { get; set; }
    }
}
