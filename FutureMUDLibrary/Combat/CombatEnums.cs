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
        GrappleForIncapitation,
        GrappleForKill,
        MeleeShooter,
        MeleeMagic
    }

    public enum RangedWeaponType {
        Thrown,
        Firearm,
        ModernFirearm,
        Laser,
        Bow,
        Crossbow,
        Sling
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
        None = 0,

        /// <summary>
        ///     An attack is a combat move that is considered hostile to a target
        /// </summary>
        Attack = 1,

        /// <summary>
        ///     A disarm is a combat move that attempts to remove or damage an opponent's weapon
        /// </summary>
        Disarm = 2,

        /// <summary>
        ///     A wound is a combat move that attempts to cause a wound (or has the significant potential to)
        /// </summary>
        Wound = 4,

        /// <summary>
        ///     A kill is a combat move that attempts to cause the death of the opponent (or has the significant potential to)
        /// </summary>
        Kill = 8,

        /// <summary>
        ///     A submit is a combat move that attempts to place its opponent in a position of surrender
        /// </summary>
        Submit = 16,

        /// <summary>
        ///     A grapple is a combat move that attempts to restrict the movement of an opponent
        /// </summary>
        Grapple = 32,

        /// <summary>
        ///     A disadvantage is a combat move that attempts to cause disadvantage to the foe, so that their actions are harder
        /// </summary>
        Disadvantage = 64,

        /// <summary>
        ///     An advantage is a combat move that attempts to give advantage to the assailant, so that their actions are easier
        /// </summary>
        Advantage = 128,

        /// <summary>
        ///     An attention is a combat move that attempts to make it harder for the foe to focus on anyone but the assailant
        /// </summary>
        Attention = 256,

        /// <summary>
        ///     A flank is a combat move that attempts to circumvent frontal defenses of the target
        /// </summary>
        Flank = 512,

        /// <summary>
        ///     A trip is a combat move that attempts to knock an opponent to the ground
        /// </summary>
        Trip = 1024,

        /// <summary>
        ///     A stun is a combat move that is substantially about stunning the target
        /// </summary>
        Stun = 2048,

        /// <summary>
        ///     A pain is a combat move that is substantially about causing the target to be in pain
        /// </summary>
        Pain = 4096,

        /// <summary>
        ///     A Coup-de-Grace is a combat move designed to be used against a helpless opponent
        /// </summary>
        CoupDeGrace = 8192,

        /// <summary>
        ///     A dirty is a combat move that would be considered dishonourable
        /// </summary>
        Dirty = 16384,

        /// <summary>
        ///     A savage attack is a combat move that would be seen as wild or savage
        /// </summary>
        Savage = 32768,

        /// <summary>
        ///     A training attack is a combat move specifically intended to be used in a training environment
        /// </summary>
        Training = 65536,

        /// <summary>
        ///     A flashy attack is a combat move which is inherently sylish or over the top
        /// </summary>
        Flashy = 131072,

        /// <summary>
        ///     A distraction attack is a combat move which is designed to draw attention away from the assailant and make them
        ///     more difficult to hit
        /// </summary>
        Distraction = 262144,

        /// <summary>
        ///     A hinder attack is an attack that is designed to prevent an opponent from running away or moving
        /// </summary>
        Hinder = 524288,

        /// <summary>
        ///     A cripple attack is focused on injuries that incapacitate the foe
        /// </summary>
        Cripple = 1048576,

        Risky = 2097152,

        Fast = 4194304,

        Slow = 8388608,

        Aggressive = 16777216,

        Defensive = 33554432,

        Cautious = 67108864,

        Cruel = 134217728,

        SelfDamaging = 268435456,

        Hard = 536870912,

        Easy = 1073741824,

        Shield = 2147483648,

        Desperate = 4294967296
    }
}