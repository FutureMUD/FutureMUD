using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Bloodtype
    {
        public Bloodtype()
        {
            BloodModelsBloodtypes = new HashSet<BloodModelsBloodtypes>();
            BloodtypesBloodtypeAntigens = new HashSet<BloodtypesBloodtypeAntigens>();
            Bodies = new HashSet<Body>();
            PopulationBloodModelsBloodtypes = new HashSet<PopulationBloodModelsBloodtype>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BloodModelsBloodtypes> BloodModelsBloodtypes { get; set; }
        public virtual ICollection<BloodtypesBloodtypeAntigens> BloodtypesBloodtypeAntigens { get; set; }
        public virtual ICollection<Body> Bodies { get; set; }
        public virtual ICollection<PopulationBloodModelsBloodtype> PopulationBloodModelsBloodtypes { get; set; }
    }
}
