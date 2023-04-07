using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Liquid
    {
        public Liquid()
        {
            Gases = new HashSet<Gas>();
            InverseCountAs = new HashSet<Liquid>();
            InverseSolvent = new HashSet<Liquid>();
            LiquidsTags = new HashSet<LiquidsTags>();
            RacesBloodLiquid = new HashSet<Race>();
            RacesBreathableLiquids = new HashSet<RacesBreathableLiquids>();
            RacesSweatLiquid = new HashSet<Race>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public string TasteText { get; set; }
        public string VagueTasteText { get; set; }
        public string SmellText { get; set; }
        public string VagueSmellText { get; set; }
        public double TasteIntensity { get; set; }
        public double SmellIntensity { get; set; }
        public double AlcoholLitresPerLitre { get; set; }
        public double WaterLitresPerLitre { get; set; }
        public double FoodSatiatedHoursPerLitre { get; set; }
        public double DrinkSatiatedHoursPerLitre { get; set; }
        public double CaloriesPerLitre { get; set; }
        public double Viscosity { get; set; }
        public double Density { get; set; }
        public bool Organic { get; set; }
        public double ThermalConductivity { get; set; }
        public double ElectricalConductivity { get; set; }
        public double SpecificHeatCapacity { get; set; }
        public double? IgnitionPoint { get; set; }
        public double? FreezingPoint { get; set; }
        public double? BoilingPoint { get; set; }
        public long? DraughtProgId { get; set; }
        public long? SolventId { get; set; }
        public long? CountAsId { get; set; }
        public int CountAsQuality { get; set; }
        public string DisplayColour { get; set; }
        public string DampDescription { get; set; }
        public string WetDescription { get; set; }
        public string DrenchedDescription { get; set; }
        public string DampShortDescription { get; set; }
        public string WetShortDescription { get; set; }
        public string DrenchedShortDescription { get; set; }
        public double SolventVolumeRatio { get; set; }
        public long? DriedResidueId { get; set; }
        public long? DrugId { get; set; }
        public double DrugGramsPerUnitVolume { get; set; }
        public int InjectionConsequence { get; set; }
        public double ResidueVolumePercentage { get; set; }
        public double RelativeEnthalpy { get; set; }
        public long? GasFormId { get; set; }

        public virtual Liquid CountAs { get; set; }
        public virtual Material DriedResidue { get; set; }
        public virtual Drug Drug { get; set; }
        public virtual Liquid Solvent { get; set; }
        public virtual Gas GasForm { get; set; }
        public virtual ICollection<Gas> Gases { get; set; }
        public virtual ICollection<Liquid> InverseCountAs { get; set; }
        public virtual ICollection<Liquid> InverseSolvent { get; set; }
        public virtual ICollection<LiquidsTags> LiquidsTags { get; set; }
        public virtual ICollection<Race> RacesBloodLiquid { get; set; }
        public virtual ICollection<RacesBreathableLiquids> RacesBreathableLiquids { get; set; }
        public virtual ICollection<Race> RacesSweatLiquid { get; set; }
    }
}
