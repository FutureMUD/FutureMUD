using System;
using System.Collections.Generic;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;

namespace MudSharp.Body {
    public enum WearlocGrabResult {
        Success,
        FailNoTake,
        FailFull,
        FailDamaged,
        FailTooBig,
        FailNoStackMerge
    }

    public enum WearlocDropResult {
        Success,
        FailClutch
    }

    [Flags]
    public enum CanUseLimbResult {
        CanUse = 0,
        CantUsePain = 1,
        CantUseDamage = 2,
        CantUseSevered = 4,
        CantUseGrappled = 8,
        CantUseRestrained = 16,
        CantUseMissingBone = 32,
        CantUseSpinalDamage = 64,
    }

    [Flags]
    public enum CanUseBodypartResult {
        CanUse = 0,
        CantUseLimbPain = 1,
        CantUsePartPain = 2,
        CantUseLimbDamage = 4,
        CantUsePartDamage = 8,
        CantUseSevered = 16,
        CantUseNonFunctionalProsthetic = 32,
        CantUseLimbGrappled = 64,
        CantUseMissingBone = 128,
        CantUseSpinalDamage = 256,
    }

    public enum EncumbranceLevel
    {
        Unencumbered,
        LightlyEncumbered,
        ModeratelyEncumbered,
        HeavilyEncumbered,
        CriticallyEncumbered
    }

    /// <summary>
    ///     An object implementing the IGrab interface implies that it can take IGameItems and can pick up / drop them - it is
    ///     an inventory or "hands" loc.
    /// </summary>
    public interface IGrab : IExternalBodypart
    {
        bool ItemsVisible();
        bool Unary { get; }
        WearlocGrabResult CanGrab(IGameItem item, IInventory body);
        WearlocDropResult CanDrop(IGameItem item, IInventory body);
    }

    public enum WearableItemCoverStatus {
        Uncovered,
        TransparentlyCovered,
        Covered,
        NoCoverInformation // this is used to designate an erroneous request for wearableitemcoverstatus
    }

    public interface IBodypartWithInventoryDisplayOrder : IBodypart
    {
        int DisplayOrder { get; }
    }

    /// <summary>
    ///     An object implementing the IWear interface implies that it can have wearable IGameItems associated with it - it is
    ///     a wearloc
    /// </summary>
    public interface IWear : IExternalBodypart
    {
        WearableItemCoverStatus HowCovered(IGameItem item, IBody body);
        Tuple<WearableItemCoverStatus, IGameItem> CoverInformation(IGameItem item, IBody body);

        bool CanWear(IGameItem item, IInventory body);
        bool CanRemove(IGameItem item, IInventory body);
    }

    public interface IHaveBodyparts {
        IEnumerable<ILimb> Limbs { get; }

        IEnumerable<IBodypart> Bodyparts { get; }

        IEnumerable<IOrganProto> Organs { get; }

        IEnumerable<IBone> Bones { get; }

        IEnumerable<IWear> WearLocs { get; }

        IEnumerable<IGrab> HoldLocs { get; }

        IEnumerable<IWield> WieldLocs { get; }

        IBodypart RandomBodypart { get; }

        IBodypart RandomBodypartOrOrgan { get; }

        IEnumerable<IBodypart> SeveredRoots { get; }
        
        IEnumerable<IBodypart> BodypartsForLimb(ILimb limb);

        IBodypart RandomBodyPartGeometry(Orientation orientation, Alignment alignment, Facing facing,
            bool appendagesActive = false);

        IBodypart RandomVitalBodypart(Facing facing);

        /// <summary>
        /// This function can return bones, organs and bodyparts
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        IBodypart GetTargetPart(string target);
        IBone GetTargetBone(string target);
        IBodypart GetTargetBodypart(string target);
        IBodypart GetTargetBodypart(string target, Alignment alignment, Orientation orientation);

        ILimb GetLimbFor(IBodypart bodypart);
        IEnumerable<IWound> GetWoundsForLimb(ILimb limb);
        bool ReevaluateLimbAndPartDamageEffects();
        void RecalculatePartsAndOrgans();
        bool CanSeverBodypart(IBodypart bodypart);
        IGameItem SeverBodypart(IBodypart bodypart);
        void RestoreBodypart(IBodypart bodypart);
        IGameItem ExciseOrgan(IOrganProto organ);
        void RestoreOrgan(IOrganProto organ);
        void RestoreAllBodypartsOrgansAndBones();

        IMaterial GetMaterial(IBodypart bodypart);
        IMaterial GetEffectiveMaterial(IBodypart bodypart);

        bool CanStand(bool ignoreAids);
        bool CanKneel(bool ignoreAids);
        bool CanSitUp();

        IEnumerable<IBone> BonesForPart(IBodypart part);
        IEnumerable<IOrganProto> OrgansForPart(IBodypart part);
        IReadOnlyDictionary<IOrganProto, BodypartInternalInfo> OrganInfosForPart(IBodypart part);
        IReadOnlyDictionary<IBone, BodypartInternalInfo> BoneInfosForPart(IBodypart part);

        double HitpointsForBodypart(IBodypart part);

        void CalculateOrganFunctions(bool initialCalculation = false);

        #region Prosthetics
        void InstallProsthetic(IProsthetic prosthetic);
        void RemoveProsthetic(IProsthetic prosthetic);
        IEnumerable<IProsthetic> Prosthetics { get; }
        #endregion
        #region Implants
        IEnumerable<IImplant> Implants { get; }
        void InstallImplant(IImplant implant);
        void RemoveImplant(IImplant implant);
        #endregion
    }

    public enum IWieldItemWieldResult {
        Success,
        NotWieldable,
        AlreadyWielding,
        TooDamaged,
        GrabbingWielderHoldOtherItem,
        Unknown
    }

    public enum IWieldItemUnwieldResult {
        Success,
        NotWielding,
        Unknown
    }

    /// <summary>
    ///     An object implementing the IWield interface implies that it can wield a wieldable item such as a weapon or tool
    /// </summary>
    public interface IWield : IExternalBodypart
    {
        SizeCategory MaxSingleSize { get; }

        /// <summary>
        ///     A self unwielder is a wielding location that is also an IGrab - in these cases we do not need to check to see if we
        ///     can send the object somewhere when we unwield
        /// </summary>
        /// <returns></returns>
        bool SelfUnwielder();

        IWieldItemWieldResult CanWield(IGameItem item, IInventory body);
        IWieldItemUnwieldResult CanUnwield(IGameItem item, IInventory body);

        /// <summary>
        ///     Specifies how many "hands" worth of wielding space that this loc gives for a particular item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int Hands(IGameItem item);
    }

    public enum ExertionLevel {
        Stasis = 0,
        Sleep,
        Rest,
        Low,
        Normal,
        Heavy,
        VeryHeavy,
        ExtremelyHeavy
    }

    public static class StaminaHelperClass {
        public static ExertionLevel StageUp(this ExertionLevel level) {
            if (level == ExertionLevel.ExtremelyHeavy) {
                return level;
            }
            return (ExertionLevel) ((int) level + 1);
        }

        public static ExertionLevel StageDown(this ExertionLevel level) {
            if (level == ExertionLevel.Stasis) {
                return level;
            }
            return (ExertionLevel) ((int) level - 1);
        }

        public static string Describe(this ExertionLevel level) {
            switch (level) {
                case ExertionLevel.Stasis:
                    return "Statis";
                case ExertionLevel.Sleep:
                    return "Sleep";
                case ExertionLevel.Rest:
                    return "Rest";
                case ExertionLevel.Low:
                    return "Low";
                case ExertionLevel.Normal:
                    return "Normal";
                case ExertionLevel.Heavy:
                    return "Heavy";
                case ExertionLevel.VeryHeavy:
                    return "Very Heavy";
                case ExertionLevel.ExtremelyHeavy:
                    return "Extremely Heavily";
                default:
                    throw new NotSupportedException("Invalid ExertionLevel in ExertionLevel.Describe");
            }
        }
    }

    public delegate void ExertionEvent (ExertionLevel oldExertion, ExertionLevel newExertion);

    public interface IHaveStamina {
        /// <summary>
        ///     The Maximum Stamina this IHaveStamina can have
        /// </summary>
        double MaximumStamina { get; }

        /// <summary>
        ///     The Current Level of Stamina.
        /// </summary>
        double CurrentStamina { get; set; }

        /// <summary>
        ///     The Exertion Level of this current IHaveStamina
        /// </summary>
        ExertionLevel CurrentExertion { get; }

        ExertionLevel LongtermExertion { get; }

        event ExertionEvent OnExertionChanged;

        void StaminaTenSecondHeartbeat();
        void StaminaMinuteHeartbeat();

        /// <summary>
        /// Sets the exertion to specifically the level mentioned
        /// </summary>
        /// <param name="level"></param>
        void SetExertion(ExertionLevel level);

        /// <summary>
        /// Sets the exertion to the higher of the current exertion and this supplied value
        /// </summary>
        /// <param name="level"></param>
        void SetExertionToMinimumLevel(ExertionLevel level);

        void InitialiseStamina();

        bool CanSpendStamina(double amount);

        void SpendStamina(double amount);
        void GainStamina(double amount);
        EncumbranceLevel Encumbrance { get; }
        double EncumbrancePercentage { get; }
    }
}