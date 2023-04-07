using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Gas
    {
        public Gas()
        {
            GasesTags = new HashSet<GasesTags>();
            InverseCountAs = new HashSet<Gas>();
            RacesBreathableGases = new HashSet<RacesBreathableGases>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Density { get; set; }
        public double ThermalConductivity { get; set; }
        public double ElectricalConductivity { get; set; }
        public bool Organic { get; set; }
        public double SpecificHeatCapacity { get; set; }
        public double BoilingPoint { get; set; }
        public long? CountAsId { get; set; }
        public int? CountsAsQuality { get; set; }
        public string DisplayColour { get; set; }
        public long? PrecipitateId { get; set; }
        public double SmellIntensity { get; set; }
        public string SmellText { get; set; }
        public string VagueSmellText { get; set; }
        public double Viscosity { get; set; }
        public long? DrugId { get; set; }
        public double DrugGramsPerUnitVolume { get; set; }
        public virtual Drug Drug { get; set; }

        public virtual Gas CountAs { get; set; }
        public virtual Liquid Precipitate { get; set; }
        public virtual ICollection<GasesTags> GasesTags { get; set; }
        public virtual ICollection<Gas> InverseCountAs { get; set; }
        public virtual ICollection<RacesBreathableGases> RacesBreathableGases { get; set; }
    }
}
