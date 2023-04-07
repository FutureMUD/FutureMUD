using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Material
    {
        public Material()
        {
            BodypartProto = new HashSet<BodypartProto>();
            Liquids = new HashSet<Liquid>();
            MaterialsTags = new HashSet<MaterialsTags>();
            Races = new HashSet<Race>();
            RacesEdibleMaterials = new HashSet<RacesEdibleMaterials>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string MaterialDescription { get; set; }
        public double Density { get; set; }
        public bool Organic { get; set; }
        public int Type { get; set; }
        public int? BehaviourType { get; set; }
        public double ThermalConductivity { get; set; }
        public double ElectricalConductivity { get; set; }
        public double SpecificHeatCapacity { get; set; }
        public long? LiquidFormId { get; set; }
        public double? Viscosity { get; set; }
        public double? MeltingPoint { get; set; }
        public double? BoilingPoint { get; set; }
        public double? IgnitionPoint { get; set; }
        public double? HeatDamagePoint { get; set; }
        public double? ImpactFracture { get; set; }
        public double? ImpactYield { get; set; }
        public double? ImpactStrainAtYield { get; set; }
        public double? ShearFracture { get; set; }
        public double? ShearYield { get; set; }
        public double? ShearStrainAtYield { get; set; }
        public double? YoungsModulus { get; set; }
        public long? SolventId { get; set; }
        public double SolventVolumeRatio { get; set; }
        public string ResidueSdesc { get; set; }
        public string ResidueDesc { get; set; }
        public string ResidueColour { get; set; }
        public double Absorbency { get; set; }

        public virtual ICollection<BodypartProto> BodypartProto { get; set; }
        public virtual ICollection<Liquid> Liquids { get; set; }
        public virtual ICollection<MaterialsTags> MaterialsTags { get; set; }
        public virtual ICollection<Race> Races { get; set; }
        public virtual ICollection<RacesEdibleMaterials> RacesEdibleMaterials { get; set; }
    }
}
