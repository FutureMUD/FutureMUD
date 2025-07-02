using System;

namespace MudSharp.Combat {
    [Flags]
    public enum DefenseType {
        None = 0,
        Dodge = 1 << 0,
        Block = 1 << 1,
        Parry = 1 << 2
    }

    public enum AttackHandednessOptions {
        Any = 0,
        OneHandedOnly = 1,
        TwoHandedOnly = 2,
        DualWieldOnly = 3,
        SwordAndBoardOnly = 4
    }

    public enum PursuitMode {
        NeverPursue,
        OnlyAttemptToStop,
        OnlyPursueIfWholeGroupPursue,
        AlwaysPursue
    }

    public enum BuiltInCombatMoveType {
        UseWeaponAttack,
        NaturalWeaponAttack,
        Dodge,
        Parry,
        Block,
        Disarm,
        Flee,
        RetrieveItem,
        ChargeToMelee,
        MoveToMelee,
        AdvanceAndFire,
        ReceiveCharge,

        /// <summary>
        ///     A Ward Defense is the person warding off the attack
        /// </summary>
        WardDefense,

        /// <summary>
        ///     A Ward Counter is used to oppose a Ward Defense
        /// </summary>
        WardCounter,

        /// <summary>
        ///     A Ward Free Attack is used when someone chooses to ignore a ward check
        /// </summary>
        WardFreeAttack,
        WardFreeUnarmedAttack,
        StartClinch,
        ResistClinch,
        BreakClinch,
        ResistBreakClinch,
        ClinchAttack,
        ClinchUnarmedAttack,
        ClinchDodge,
        DodgeRange,
        BlockRange,
        StandAndFire,
        SkirmishAndFire,
        RangedWeaponAttack,
        AimRangedWeapon,
        CoupDeGrace,
        Rescue,
        MeleeWeaponSmashItem,
        UnarmedSmashItem,
        StaggeringBlow,
        StaggeringBlowUnarmed,
        UnbalancingBlow,
        UnbalancingBlowUnarmed,
        InitiateGrapple,
        DodgeGrapple,
        CounterGrapple,
        ExtendGrapple,
        WrenchAttack,
        StrangleAttack,
        DodgeExtendGrapple,
        BeamAttack,
        ScreechAttack,
        OverpowerGrapple,
        StrangleAttackExtendGrapple,
        DesperateDodge,
        DesperateParry,
        DesperateBlock,
        DownedAttack,
        DownedAttackUnarmed,
        MagicPowerAttack,
        TakedownMove,
        Breakout,
        UnbalancingBlowClinch,
        StaggeringBlowClinch,
        SwoopAttack,
        SwoopAttackUnarmed,
        EnvenomingAttack,
        EnvenomingAttackClinch,
        AuxiliaryMove
    }

    public enum WeaponClassification {
        None,
        NonLethal,
        Lethal,
        Military,
        Exotic,
        Ceremonial,
        Training,
        Natural,
        Improvised,
        Shield
    }

    public enum CombatStrategyMode {
        StandardMelee,
        StandardRange,
        CoveringFire,
        FireAndAdvance,
        CoverAndAdvance,
        FullAdvance,
        FullCover,
        FullDefense,
        Ward,
        Clinch,
        Skirmish,
        FullSkirmish,
        FireNoCover,
        Flee,
        GrappleForControl,
        GrappleForIncapacitation,
        GrappleForKill,
        MeleeShooter,
        MeleeMagic,
        Swooper
    }

    public enum RangedWeaponType {
        Thrown,
        Firearm,
        ModernFirearm,
        Laser,
        Bow,
        Crossbow,
        Sling,
        Musket
    }

    public enum RangedScatterType {
        Arcing,
        Ballistic,
        Light,
        Spread
    }

    public enum MeleeWeaponVerb {
        Swing,
        Chop,
        Thrust,
        Jab,
        Stab,
        Whirl,
        Bash,
        Bite,
        Claw,
        Kick,
        Punch,
        Strike,
        Beam,
        Sweep,
        Grapple,
        Slam,
        Blast,
        Swipe,
    }

    [Flags]
    /// <summary>
    /// These enum flags determine what the core intention of a combat move is, for use in preferencing
    /// </summary>
    public enum CombatMoveIntentions : long {
        None = 0b_0,

        /// <summary>
        ///     An attack is a combat move that is considered hostile to a target
        /// </summary>
        Attack = 0b_1,

        /// <summary>
        ///     A disarm is a combat move that attempts to remove or damage an opponent's weapon
        /// </summary>
        Disarm = 0b_10,

        /// <summary>
        ///     A wound is a combat move that attempts to cause a wound (or has the significant potential to)
        /// </summary>
        Wound = 0b_100,

        /// <summary>
        ///     A kill is a combat move that attempts to cause the death of the opponent (or has the significant potential to)
        /// </summary>
        Kill = 0b_1000,

        /// <summary>
        ///     A submit is a combat move that attempts to place its opponent in a position of surrender
        /// </summary>
        Submit = 0b_10000,

        /// <summary>
        ///     A grapple is a combat move that attempts to restrict the movement of an opponent
        /// </summary>
        Grapple = 0b_100000,

        /// <summary>
        ///     A disadvantage is a combat move that attempts to cause disadvantage to the foe, so that their actions are harder
        /// </summary>
        Disadvantage = 0b_1000000,

        /// <summary>
        ///     An advantage is a combat move that attempts to give advantage to the assailant, so that their actions are easier
        /// </summary>
        Advantage = 0b_10000000,

        /// <summary>
        ///     An attention is a combat move that attempts to make it harder for the foe to focus on anyone but the assailant
        /// </summary>
        Attention = 0b_1_00000000,

        /// <summary>
        ///     A flank is a combat move that attempts to circumvent frontal defenses of the target
        /// </summary>
        Flank = 0b_10_00000000,

        /// <summary>
        ///     A trip is a combat move that attempts to knock an opponent to the ground
        /// </summary>
        Trip = 0b_100_00000000,

        /// <summary>
        ///     A stun is a combat move that is substantially about stunning the target
        /// </summary>
        Stun = 0b_1000_00000000,

        /// <summary>
        ///     A pain is a combat move that is substantially about causing the target to be in pain
        /// </summary>
        Pain = 0b_10000_00000000,

        /// <summary>
        ///     A Coup-de-Grace is a combat move designed to be used against a helpless opponent
        /// </summary>
        CoupDeGrace = 0b_100000_00000000,

        /// <summary>
        ///     A dirty is a combat move that would be considered dishonourable
        /// </summary>
        Dirty = 0b_1000000_00000000,

        /// <summary>
        ///     A savage attack is a combat move that would be seen as wild or savage
        /// </summary>
        Savage = 0b_10000000_00000000,

        /// <summary>
        ///     A training attack is a combat move specifically intended to be used in a training environment
        /// </summary>
        Training = 0b_1_00000000_00000000,

        /// <summary>
        ///     A flashy attack is a combat move which is inherently sylish or over the top
        /// </summary>
        Flashy = 0b_10_00000000_00000000,

        /// <summary>
        ///     A distraction attack is a combat move which is designed to draw attention away from the assailant and make them
        ///     more difficult to hit
        /// </summary>
        Distraction = 0b_100_00000000_00000000,

        /// <summary>
        ///     A hinder attack is an attack that is designed to prevent an opponent from running away or moving
        /// </summary>
        Hinder = 0b_1000_00000000_00000000,

        /// <summary>
        ///     A cripple attack is focused on injuries that incapacitate the foe
        /// </summary>
        Cripple = 0b_10000_00000000_00000000,

        Risky = 0b_100000_00000000_00000000,

        Fast = 0b_1000000_00000000_00000000,

        Slow = 0b_10000000_00000000_00000000,

        Aggressive = 0b_1_00000000_00000000_00000000,

        Defensive = 0b_10_00000000_00000000_00000000,

        Cautious = 0b_100_00000000_00000000_00000000,

        Cruel = 0b_1000_00000000_00000000_00000000,

        SelfDamaging = 0b_10000_00000000_00000000_00000000,

        Hard = 0b_100000_00000000_00000000_00000000,

        Easy = 0b_1000000_00000000_00000000_00000000,

        Shield = 0b_10000000_00000000_00000000_00000000,

        Desperate = 0b_1_00000000_00000000_00000000_00000000,

        Slashing = 0b_10_00000000_00000000_00000000_00000000,

        Crushing = 0b_100_00000000_00000000_00000000_00000000,

        Piercing = 0b_1000_00000000_00000000_00000000_00000000,

        Chopping = 0b_10000_00000000_00000000_00000000_00000000,

        Burning = 0b_100000_00000000_00000000_00000000_00000000,

        Bite = 0b_1000000_00000000_00000000_00000000_00000000,

        Claw = 0b_10000000_00000000_00000000_00000000_00000000,

        Freezing = 0b_1_00000000_00000000_00000000_00000000_00000000,

        Chemical = 0b_10_00000000_00000000_00000000_00000000_00000000,

        ArmourPiercing = 0b_100_00000000_00000000_00000000_00000000_00000000,
    }
}