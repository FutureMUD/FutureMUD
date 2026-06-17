using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MudSharp.Health
{

    [Flags]
    public enum DrugVector
    {
        None = 0,
        Injected = 1 << 0,
        Ingested = 1 << 1,
        Inhaled = 1 << 2,
        Touched = 1 << 3
    }

    public class DrugDosage
    {
        public IDrug Drug { get; init; }
        public double Grams { get; set; }
        public DrugVector OriginalVector { get; init; }
        public object Originator { get; init; }
    }

    public abstract class DrugAdditionalInfo
    {
        public abstract string DatabaseString { get; }
    }

    public class NeutraliseDrugAdditionalInfo : DrugAdditionalInfo
    {
        public required List<DrugType> NeutralisedTypes { get; set; }

        #region Overrides of DrugAdditionalInfo

        /// <inheritdoc />
        public override string DatabaseString => NeutralisedTypes.Select(x => ((int)x).ToString(System.Globalization.CultureInfo.InvariantCulture)).ListToCommaSeparatedValues(" ");

        #endregion
    }

    public class NeutraliseSpecificDrugAdditionalInfo : DrugAdditionalInfo
    {
        public required List<long> NeutralisedIds { get; set; }

        #region Overrides of DrugAdditionalInfo

        /// <inheritdoc />
        public override string DatabaseString => NeutralisedIds.Select(x => x.ToString(System.Globalization.CultureInfo.InvariantCulture)).ListToCommaSeparatedValues(" ");

        #endregion
    }

    public class BodypartDamageAdditionalInfo : DrugAdditionalInfo
    {
        public required List<BodypartTypeEnum> BodypartTypes { get; set; }
        public override string DatabaseString => BodypartTypes.Select(x => ((int)x).ToString(System.Globalization.CultureInfo.InvariantCulture)).ListToCommaSeparatedValues(" ");
    }

    public class HealingRateAdditionalInfo : DrugAdditionalInfo
    {
        public required double HealingRateIntensity { get; set; }
        public required double HealingDifficultyIntensity { get; set; }

        #region Overrides of DrugAdditionalInfo

        /// <inheritdoc />
        public override string DatabaseString => $"{HealingRateIntensity:R} {HealingDifficultyIntensity:R}";

        #endregion
    }

    public class MagicAbilityAdditionalInfo : DrugAdditionalInfo
    {
        public required List<long> MagicCapabilityIds { get; set; }

        #region Overrides of DrugAdditionalInfo

        /// <inheritdoc />
        public override string DatabaseString => MagicCapabilityIds.Select(x => x.ToString(System.Globalization.CultureInfo.InvariantCulture)).ListToCommaSeparatedValues(" ");

        #endregion
    }

    public class OrganFunctionAdditionalInfo : DrugAdditionalInfo
    {
        public required List<BodypartTypeEnum> OrganTypes { get; set; }

        public override string DatabaseString => OrganTypes.Select(x => ((int)x).ToString(System.Globalization.CultureInfo.InvariantCulture)).ListToCommaSeparatedValues(" ");
    }

    public class PlanarStateAdditionalInfo : DrugAdditionalInfo
    {
        public required string State { get; set; }
        public required long PlaneId { get; set; }
        public required bool VisibleToDefaultPlane { get; set; }

        public override string DatabaseString => $"{State} {PlaneId} {VisibleToDefaultPlane}";
    }

    public class CoagulationAdditionalInfo : DrugAdditionalInfo
    {
        public required double ExternalBleedingMultiplier { get; set; }
        public required double WoundReopenMultiplier { get; set; }
        public required double InternalBleedingMultiplier { get; set; }

        public override string DatabaseString =>
            $"{ExternalBleedingMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {WoundReopenMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {InternalBleedingMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}";
    }

    public class RespirationAdditionalInfo : DrugAdditionalInfo
    {
        public required double BreathingDriveMultiplier { get; set; }
        public required double HypoxiaDamageMultiplier { get; set; }
        public required double AirwayToleranceMultiplier { get; set; }

        public override string DatabaseString =>
            $"{BreathingDriveMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {HypoxiaDamageMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {AirwayToleranceMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}";
    }

    public class NeedRateAdditionalInfo : DrugAdditionalInfo
    {
        public required double HungerMultiplier { get; set; }
        public required double ThirstMultiplier { get; set; }
        public required double DrunkennessMultiplier { get; set; }
        public required bool AppliesToPassive { get; set; }
        public required bool AppliesToActive { get; set; }

        public override string DatabaseString =>
            $"{HungerMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {ThirstMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {DrunkennessMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {AppliesToPassive} {AppliesToActive}";
    }

    [Flags]
    public enum DrugArousalMode
    {
        None = 0,
        SleepInducing = 1 << 0,
        SleepPreventing = 1 << 1,
        PassOutResistance = 1 << 2,
        Knockout = 1 << 3,
        Stimulant = 1 << 4,
        Sedative = 1 << 5
    }

    public class ArousalAdditionalInfo : DrugAdditionalInfo
    {
        public required DrugArousalMode Mode { get; set; }
        public required double CheckBonusPerIntensity { get; set; }
        public required double SleepIntensityThreshold { get; set; }
        public required double KnockoutIntensityThreshold { get; set; }
        public required double PainPassOutThresholdMultiplier { get; set; }
        public required double StunUnconsciousThresholdMultiplier { get; set; }
        public required double AnesthesiaUnconsciousThresholdMultiplier { get; set; }
        public required double StaminaRegenMultiplier { get; set; }
        public required double StaminaCostMultiplier { get; set; }

        public override string DatabaseString =>
            $"{(int)Mode} {CheckBonusPerIntensity.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {SleepIntensityThreshold.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {KnockoutIntensityThreshold.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {PainPassOutThresholdMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {StunUnconsciousThresholdMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {AnesthesiaUnconsciousThresholdMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {StaminaRegenMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {StaminaCostMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}";
    }

    public class DrugDependenceAdditionalInfo : DrugAdditionalInfo
    {
        public required double ExposureGainPerGram { get; set; }
        public required double ExposureDecayPerDay { get; set; }
        public required double ToleranceThreshold { get; set; }
        public required double MinimumToleranceMultiplier { get; set; }
        public required double WithdrawalThreshold { get; set; }
        public required double WithdrawalDecayPerDay { get; set; }
        public required List<DrugType> AffectedDrugTypes { get; set; }
        public required double WithdrawalCheckPenalty { get; set; }
        public required double WithdrawalHungerMultiplier { get; set; }
        public required double WithdrawalThirstMultiplier { get; set; }
        public required double WithdrawalStaminaRegenMultiplier { get; set; }
        public required double WithdrawalStaminaCostMultiplier { get; set; }
        public required double WithdrawalNauseaIntensity { get; set; }
        public required double WithdrawalRageIntensity { get; set; }
        public required double SleepPreventionThreshold { get; set; }

        public override string DatabaseString =>
            $"{ExposureGainPerGram.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {ExposureDecayPerDay.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {ToleranceThreshold.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {MinimumToleranceMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {WithdrawalThreshold.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {WithdrawalDecayPerDay.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {WithdrawalCheckPenalty.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {WithdrawalHungerMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {WithdrawalThirstMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {WithdrawalStaminaRegenMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {WithdrawalStaminaCostMultiplier.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {WithdrawalNauseaIntensity.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {WithdrawalRageIntensity.ToString("R", System.Globalization.CultureInfo.InvariantCulture)} {SleepPreventionThreshold.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}|{AffectedDrugTypes.Select(x => ((int)x).ToString(System.Globalization.CultureInfo.InvariantCulture)).ListToCommaSeparatedValues(" ")}";
    }

    public interface IDrug : IEditableItem, IProgVariable
    {
        DrugVector DrugVectors { get; }
        IEnumerable<DrugType> DrugTypes { get; }
        T AdditionalInfoFor<T>(DrugType type) where T : DrugAdditionalInfo;
        double IntensityPerGram { get; }
        double RelativeMetabolisationRate { get; }
        double IntensityForType(DrugType type);
        string DescribeEffect(DrugType type, IPerceiver voyeur);
        IDrug Clone(string newName);
    }

    public static class DrugExtensions
    {
        public static string Describe(this DrugVector type)
        {
            List<string> list = new();
            foreach (Enum @enum in type.GetFlags())
            {
                DrugVector subtype = (DrugVector)@enum;
                switch (subtype)
                {
                    case DrugVector.Ingested:
                        list.Add("Ingested");
                        break;
                    case DrugVector.Inhaled:
                        list.Add("Inhaled");
                        break;
                    case DrugVector.Injected:
                        list.Add("Injected");
                        break;
                    case DrugVector.Touched:
                        list.Add("Touched");
                        break;
                }
            }

            return list.ListToString(conjunction: "or ");
        }
    }
}
