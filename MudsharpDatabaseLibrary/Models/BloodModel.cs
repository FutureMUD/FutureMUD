using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BloodModel
    {
        public BloodModel()
        {
            BloodModelsBloodtypes = new HashSet<BloodModelsBloodtypes>();
            Races = new HashSet<Race>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BloodModelsBloodtypes> BloodModelsBloodtypes { get; set; }
        public virtual ICollection<Race> Races { get; set; }
    }
}
