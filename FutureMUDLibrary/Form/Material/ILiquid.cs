using System;
using MudSharp.FutureProg;
using MudSharp.GameItems;

namespace MudSharp.Form.Material {
    public enum LiquidInjectionConsequence {
        Benign,
        Hydrating,
        BloodVolume,
        BloodReplacement,
        Harmful,
        Deadly,
        KidneyWaste
    }

    public static class LiquidExtensions {
        public static string Describe(this LiquidInjectionConsequence consequence) {
            switch (consequence) {
                case LiquidInjectionConsequence.Benign:
                    return "Benign";
                case LiquidInjectionConsequence.BloodVolume:
                    return "Blood Volume";
                case LiquidInjectionConsequence.BloodReplacement:
                    return "Blood Replacement";
                case LiquidInjectionConsequence.Deadly:
                    return "Deadly";
                case LiquidInjectionConsequence.Harmful:
                    return "Harmful";
                case LiquidInjectionConsequence.Hydrating:
                    return "Hydrating";
                case LiquidInjectionConsequence.KidneyWaste:
                    return "Waste Products Removed by Kidneys";
            }

            throw new NotImplementedException("Unknown Liquid Consequence");
        }
    }

    public interface ILiquid : IFluid {
        double TasteIntensity { get; }
        string TasteText { get; }
        string VagueTasteText { get; }

        string Description { get; }

        double AlcoholLitresPerLitre { get; }
        double WaterLitresPerLitre { get; }
        double FoodSatiatedHoursPerLitre { get; }
        double DrinkSatiatedHoursPerLitre { get; }
        double CaloriesPerLitre { get; }

        IFutureProg DraughtProg { get; }
        double SolventVolumeRatio { get; }
        ILiquid Solvent { get; }
        ISolid DriedResidue { get; }
        double ResidueVolumePercentage { get; }
        bool LeaveResiduesInRooms { get; }
        double RelativeEnthalpy { get; }
        ILiquid CountsAs { get; }
        ItemQuality CountsAsQuality { get; }
        double? FreezingPoint { get; }
        double? IgnitionPoint { get; }
        IGas GasForm { get; }

        /// <summary>
        ///     Boiling point of the material in Kelvin
        /// </summary>
        double? BoilingPoint { get; }

        string DampDescription { get; }
        string WetDescription { get; }
        string DrenchedDescription { get; }
        string DampShortDescription { get; }
        string WetShortDescription { get; }
        string DrenchedShortDescription { get; }
        LiquidInjectionConsequence InjectionConsequence { get; }
        bool LiquidCountsAs(ILiquid otherLiquid);
        ItemQuality LiquidCountsAsQuality(ILiquid otherLiquid);
        ILiquid Clone(string newName);
    }
}