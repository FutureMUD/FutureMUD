using System;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Health {
    public class BleedResult {
        public double BloodAmount { get; set; }
        public bool Visible { get; set; }
        public IGameItem CoverItem { get; set; }
        public IBodypart Bodypart { get; set; }

        public static BleedResult NoBleed { get; } = new();
    }

    public interface IWound : IPerceivable {
        bool IsFriendlyWound { get; }
        bool Repairable { get; }
        /// <summary>
        ///     Contains an IGameItem with the game item that is lodged in this wound. It will be null if there is no lodged
        ///     object.
        /// </summary>
        IGameItem Lodged { get; set; }

        BleedStatus BleedStatus { get; set; }

        double CurrentPain { get; set; }
        double CurrentStun { get; set; }
        double CurrentShock { get; set; }

        double OriginalDamage { get; set; }
        double CurrentDamage { get; set; }

        WoundSeverity Severity { get; }
        DamageType DamageType { get; }

        /// <summary>
        ///     Whether or not this is an internal wound
        /// </summary>
        bool Internal { get; }

        /// <summary>
        ///     The parent IDamageable to which the IWound belongs
        /// </summary>
        IHaveWounds Parent { get; set; }

        /// <summary>
        ///     The body part on which this wound has been inflicted, if any
        /// </summary>
        IBodypart Bodypart { get; }

        IBodypart SeveredBodypart { get; set; }

        /// <summary>
        ///     An optional IPerceivable parameter that specifies what IPerceivable was responsible for the damage
        ///     Note that the result may not be loaded into the game if it is not already in game, and thus this value should not
        ///     be propogated or stored beyond immediate comparison.
        ///     Additionally, reference equality may not be true so use value equality
        /// </summary>
        IGameItem ToolOrigin { get; }

        /// <summary>
        ///     An optional ICharacter parameter that specifies what ICharacter was the Actor responsible for whatever caused
        ///     damage
        ///     Note that the result may not be loaded into the game if it is not already in game, and thus this value should not
        ///     be propogated or stored beyond immediate comparison.
        ///     Additionally, reference equality may not be true so use value equality
        /// </summary>
        ICharacter ActorOrigin { get; }

        IInfection Infection { get; set; }

        /// <summary>
        ///     Returns a string describing the current state of the wound
        /// </summary>
        /// <param name="type">The type of examination being performed</param>
        /// <param name="outcome">The outcome of the examination test</param>
        /// <returns>A string describing the current state of the wound</returns>
        string Describe(WoundExaminationType type, Outcome outcome);

        /// <summary>
        ///     Returns a CheckDifficulty indicating the difficulty and/or possibility of treating this wound in a particular way
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Difficulty CanBeTreated(TreatmentType type);

        /// <summary>
        ///     Returns a string designed to be returned as an error message as to why a particular kind of treatment cannot be
        ///     performed on this wound
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string WhyCannotBeTreated(TreatmentType type);

        /// <summary>
        ///     Performs a treatment on the item, with the boolean representing whether or not it was successful
        /// </summary>
        /// <param name="treater"></param>
        /// <param name="type"></param>
        /// <param name="treatmentItem"></param>
        /// <param name="testOutcome"></param>
        /// <param name="silent"></param>
        void Treat(IPerceiver treater, TreatmentType type, ITreatment treatmentItem, Outcome testOutcome, bool silent);

        /// <summary>
        ///     Performs a bleed tick and returns the amount of blood lost in ml
        /// </summary>
        /// <returns></returns>
        BleedResult Bleed(double currentBloodLitres, ExertionLevel activityExertionLevel, double totalBloodLitres);

        /// <summary>
        ///     Returns the amount of blood that wound be lost if this wound bled in ml
        /// </summary>
        /// <param name="bloodTotal"></param>
        /// <param name="activityExertionLevel"></param>
        /// <returns></returns>
        double PeekBleed(double bloodTotal, ExertionLevel activityExertionLevel);

        /// <summary>
        ///     Performs a check to see if there is a chance the wound opens up, gets worse etc. with exertion, and returns the
        ///     pain
        /// </summary>
        /// <param name="exertion">The ExertionType representing the level of exersion this wound is exposed to</param>
        /// <returns>An number representing the pain of exertion</returns>
        double Exert(ExertionType exertion);

        /// <summary>
        ///     Performs a healing tick and returns the new pain of the wound
        /// </summary>
        /// <returns></returns>
        bool HealingTick(double externalRateMultiplier, double externalCheckBonus);

        bool EligableForInfection();

        void Delete(bool ignoreDatabaseDeletion = false);

        void DoOfflineHealing(TimeSpan timePassed, double externalRateMultiplier, double externalCheckBonus);

        bool ShouldWoundBeRemoved();
        void OnWoundSuffered();
        bool UseDamagePercentageSeverities { get; }
        Difficulty ConcentrationDifficulty { get; }
        void SufferAdditionalDamage(IDamage damage);
        void SetNewOwner(IHaveWounds newOwner);
    }
}