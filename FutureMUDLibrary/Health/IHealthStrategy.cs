using MudSharp.Body;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;

namespace MudSharp.Health {
    public enum HealthTickResult {
        None,
        Paralyzed,
        Unconscious,
        PassOut,
        Dead
    }

    public enum HealthDamageType {
        Damage,
        Stun,
        Shock,
        Pain
    }

    public enum HealthStrategyOwnerType {
        Character,
        GameItem
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
        Default = 0,        //Original FutureMUD 2 row prompt
        Classic = 1,        //Classic RPI prompt
        Full = 2,           //Original FutureMUD 2 row prompt
        FullBrief = 4,      //Original FutureMUD 2 row prompt but with fields trimmed out if they're at/near default values
        SpeakInfo = 8,      //Adds speaking language field
        PositionInfo = 16,  //Toggles whether or not the Position field appears in Brief or Full
        StealthInfo = 32,
        Brief = 64,
        IncludeMagic = 128,
    }

    public interface IHealthStrategy : IFrameworkItem {
        string HealthStrategyType { get; }
        HealthStrategyOwnerType OwnerType { get; }
        bool KidneyFunctionActive { get; }
        bool RequiresSpinalCord { get; }
        IWound SufferDamage(IHaveWounds owner, IDamage damage, IBodypart bodypart);
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