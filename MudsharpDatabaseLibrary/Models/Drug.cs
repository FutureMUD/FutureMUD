using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Drug
    {
        public Drug()
        {
            BodiesDrugDoses = new HashSet<BodyDrugDose>();
            DrugsIntensities = new HashSet<DrugIntensity>();
            Liquids = new HashSet<Liquid>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public int DrugVectors { get; set; }
        public double IntensityPerGram { get; set; }
        public double RelativeMetabolisationRate { get; set; }

        public virtual ICollection<BodyDrugDose> BodiesDrugDoses { get; set; }
        public virtual ICollection<DrugIntensity> DrugsIntensities { get; set; }
        public virtual ICollection<Liquid> Liquids { get; set; }
    }
}
