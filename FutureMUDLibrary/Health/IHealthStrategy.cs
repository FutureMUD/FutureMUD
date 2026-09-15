using MudSharp.Body;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;

namespace MudSharp.Health
{
    public enum HealthTickResult
    {
        None,
        Paralyzed,
        Unconscious,
        PassOut,
        Dead
    }

    public enum HealthDamageType
    {
        Damage,
        Stun,
        Shock,
        Pain
    }

    [Flags]
    public enum DirectHealthCostChannels
    {
        None = 0,
        Damage = 1,
        Pain = 2,
        Stun = 4
    }

    /// <summary>
    /// A health-cost application that has been checked against a particular body part and, where
    /// applicable, a particular existing wound. The inputs are the values that must be supplied to
    /// the native wound implementation; the expected values are the visible costs after its
    /// modifiers have been applied.
    /// </summary>
    public sealed record DirectHealthCostPlan(
        IBodypart Bodypart,
        IWound ExistingWound,
        double DamageInput,
        double PainInput,
        double StunInput,
        double ExpectedDamage,
        double ExpectedPain,
        double ExpectedStun);

    public enum HealthStrategyOwnerType
    {
        Character,
        GameItem
    }

    public enum HealthStateModel
    {
        Organic = 0,
        Robot = 1,
        Construct = 2,
        GameItem = 3
    }

    public enum BodyTemperatureStatus
    {
        CriticalHypothermia,
        SevereHypothermia,
        ModerateHypothermia,
        MildHypothermia,
        VeryMildHypothermia,
        NormalTemperature,
        VeryMildHyperthermia,
        MildHyperthermia,
        ModerateHyperthermia,
        SevereHyperthermia,
        CriticalHyperthermia
    }

    [Flags]
    public enum PromptType
    {
        Default = 0,
        Classic = 1,
        Full = 2,
        FullBrief = 4,
        SpeakInfo = 8,
        PositionInfo = 16,
        StealthInfo = 32,
        Brief = 64,
        IncludeMagic = 128,
    }

    public interface IHealthStrategy : IEditableItem
    {
		/// <summary>
		/// Whether owners using this strategy require periodic health processing after wound changes.
		/// Static item-health strategies can opt out because their status is evaluated synchronously.
		/// </summary>
		bool RequiresPeriodicHealthTick => true;

		/// <summary>
		/// The independent wound channels this strategy can safely price and apply as a direct
		/// health cost. Strategies opt in explicitly so callers do not guess at their wound
		/// semantics.
		/// </summary>
		DirectHealthCostChannels SupportedDirectHealthCostChannels => DirectHealthCostChannels.None;

		/// <summary>
		/// Attempts to create a deterministic, side-effect-free direct health-cost plan for a
		/// specific body part. Implementations must reject plans whose native result cannot be
		/// predicted safely.
		/// </summary>
		bool TryPlanDirectHealthCost(IHaveWounds owner, IBodypart bodypart, double damage, double pain,
			double stun, WoundSeverity maximumSeverity, out DirectHealthCostPlan plan, out string error)
		{
			plan = null;
			error = "This health strategy does not support direct independent health costs.";
			return false;
		}

		/// <summary>
		/// Applies a previously accepted direct health-cost plan and returns every native wound that
		/// was changed. Callers use that set for the bounded persistence checkpoint.
		/// </summary>
		IReadOnlyList<IWound> ApplyDirectHealthCost(IHaveWounds owner, DirectHealthCostPlan plan)
		{
			throw new NotSupportedException("This health strategy does not support direct independent health costs.");
		}
        string HealthStrategyType { get; }
        HealthStrategyOwnerType OwnerType { get; }
        HealthStateModel HealthStateModel { get; }
        bool KidneyFunctionActive { get; }
        bool RequiresSpinalCord { get; }
        IHealthStrategy Clone(string name);
        bool CanTransferBodyStateTo(IHealthStrategy other);
        IEnumerable<IWound> SufferDamage(IHaveWounds owner, IDamage damage, IBodypart bodypart);
        WoundSeverity GetSeverityFor(IWound wound, IHaveWounds owner);
        WoundSeverity GetSeverity(double damage);
        double GetSeverityFloor(WoundSeverity severity, bool usePercentageModel = false);
        double GetSeverityCeiling(WoundSeverity severity, bool usePercentageModel = false);
        HealthTickResult PerformHealthTick(IHaveWounds thing);
        HealthTickResult EvaluateStatus(IHaveWounds thing);
        string ReportConditionPrompt(IHaveWounds owner, PromptType type);
        double GetHealingTickAmount(IWound wound, Outcome outcome, HealthDamageType type);
        double WoundPenaltyFor(IHaveWounds owner);
        void InjectedLiquid(IHaveWounds owner, LiquidMixture mixture);
        void PerformBloodGain(IHaveWounds owner);
        BodyTemperatureStatus CurrentTemperatureStatus(IHaveWounds owner);
        double CurrentHealthPercentage(IHaveWounds owner);
        double MaxHP(IHaveWounds owner);
        double MaxPain(IHaveWounds owner);
        double MaxStun(IHaveWounds owner);
        bool IsCriticallyInjured(IHaveWounds owner);
        void PerformKidneyFunction(IBody owner);
        void PerformLiverFunction(IBody owner);
        void PerformSpleenFunction(IBody owner);
    }
}
