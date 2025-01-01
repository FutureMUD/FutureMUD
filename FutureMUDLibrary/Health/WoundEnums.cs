namespace MudSharp.Health {
    public enum WoundHealingTickResult {
        NoHealBleeding,
        NoHealInCombat,
        NoHealLodged,
        NoHealCantBreathe,
        NoHealInfected,
        NoHealNotSutured,
        NoHealNotSuturedAutoClosed,
        Healed
    }
    public enum ExertionType {
        NonExertive = 0, // the action does not register as a physical exertion
        Reflexive = 1, // breathing, etc
        VeryLight = 2,
        Light = 3,
        Medium = 4,
        Heavy = 5,
        VeryHeavy = 6,
        Extreme = 7
    }

    public enum TreatmentType {
        None = 0,
        Antiseptic = 1, // stops infection
        Trauma = 2, // stops bleeding
        Clean = 3, // prevents infection from starting
        Close = 4, // closes open wounds to allow them to heal
        Set = 5, // immobolises broken bones
        Relocation = 6, // relocates dislocated joints
        Remove = 7, // removes lodged items
        AntiInflammatory = 8, // do not use,
        Repair = 9, // repairing an item
        Mend = 10, // Mend represents magical healing and such
        Tend = 11, // Medical or surgical help that represents tending to the wound
        SurgicalSet = 12, // Surgical setting and reinforcement of a broken bone
    }

    public enum WoundSeverity {
        None = 0,
        Superficial = 1,
        Minor = 2,
        Small = 3,
        Moderate = 4,
        Severe = 5,
        VerySevere = 6,
        Grievous = 7,
        Horrifying = 8
    }

    public enum DamageType {
        Slashing = 0,
        Chopping = 1,
        Crushing = 2,
        Piercing = 3,
        Ballistic = 4,
        Burning = 5,
        Freezing = 6,
        Chemical = 7,
        Shockwave = 8,
        Bite = 9,
        Claw = 10,
        Electrical = 11,
        Hypoxia = 12,
        Cellular = 13,
        Sonic = 14,
        Shearing = 15,
        BallisticArmourPiercing = 16,
        Wrenching = 17,
        Shrapnel = 18,
        Necrotic = 19,
        Falling = 20,
        Eldritch = 21,
        Arcane = 22,
        ArmourPiercing = 23
    }

    public enum WoundExaminationType {
        /// <summary>
        ///     A glance is a look in a stressful scenario, for example when in combat
        /// </summary>
        Glance = 0,

        /// <summary>
        ///     A look is when someone is merely looked at by another
        /// </summary>
        Look = 1,

        Triage = 2,

        /// <summary>
        ///     An examination is a slower, more thorough and deliberate examination of a person's wounds
        /// </summary>
        Examination = 3,

        /// <summary>
        ///     A surgical examination is an examination with limited surgical intervention to determine the extent of injuries
        /// </summary>
        SurgicalExamination = 4,

        /// <summary>
        ///     An omniscient examination knows everything about the wound
        /// </summary>
        Omniscient = 5,

        /// <summary>
        /// Similar to look, but the person doing the looking is themselves, so knows about pain etc
        /// </summary>
        Self = 6
    }

    public enum BleedStatus {
        /// <summary>
        ///     NeverBled means this wound was never bleeding and never will be
        /// </summary>
        NeverBled,

        /// <summary>
        ///     Bleeding means that the wound is actively bleeding
        /// </summary>
        Bleeding,

        /// <summary>
        ///     TraumaControlled means that the wound is not bleeding, but has not been properly closed. It will open really easily
        ///     with exertion.
        /// </summary>
        TraumaControlled,

        /// <summary>
        ///     Closed means that the wound has been closed up. It may still open again with a lot of exertion.
        /// </summary>
        Closed
    }
}