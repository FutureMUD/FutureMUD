#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Traits;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

namespace DatabaseSeeder.Seeders;

public partial class CombatSeeder
{
    private void SeedUnarmedCombatMessage(FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        void AddCombatMessage(string message, string? failmessage, BuiltInCombatMoveType move, double chance,
            int priority,
            MeleeWeaponVerb? verb, Outcome? outcome)
        {
            context.CombatMessages.Add(new CombatMessage
            {
                Message = message,
                FailureMessage = failmessage ?? message,
                Type = (int)move,
                Chance = chance,
                Priority = priority,
                Verb = verb.HasValue ? (int)verb.Value : null,
                Outcome = outcome.HasValue ? (int)outcome.Value : null
            });
            context.SaveChanges();
        }

        SeedCombatMessageStyle messageStyle = CombatSeederMessageStyleHelper.Parse(questionAnswers["messagestyle"]);
        string attackAddendum = CombatSeederMessageStyleHelper.AttackSuffix(messageStyle);
        string Standalone(string message)
        {
            return CombatSeederMessageStyleHelper.FormatStandaloneMessage(message);
        }

        #region Attack Fallbacks

        AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 1, null, null);
        AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 1, null, null);
        AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 1, null, null);
        AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 1, null, null);

        AddCombatMessage($"$0 swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Swing, null);
        AddCombatMessage($"$0 thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Thrust, null);
        AddCombatMessage($"$0 jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Jab, null);
        AddCombatMessage($"$0 stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Stab, null);
        AddCombatMessage($"$0 attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Bash, null);
        AddCombatMessage($"$0 attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Bite, null);
        AddCombatMessage($"$0 rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Claw, null);
        AddCombatMessage($"$0 attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Kick, null);
        AddCombatMessage($"$0 attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Strike, null);
        AddCombatMessage($"$0 sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Sweep, null);
        AddCombatMessage($"$0 slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Slam, null);
        AddCombatMessage($"$0 attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 2, MeleeWeaponVerb.Punch, null);
        AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 1, null, null);

        AddCombatMessage($"$0 clumsily swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorFail);

        AddCombatMessage($"$0 adeptly swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.NaturalWeaponAttack, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorPass);

        #region Clinc

        AddCombatMessage($"$0 swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Swing, null);
        AddCombatMessage($"$0 thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Thrust, null);
        AddCombatMessage($"$0 jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Jab, null);
        AddCombatMessage($"$0 stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Stab, null);
        AddCombatMessage($"$0 attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Bash, null);
        AddCombatMessage($"$0 attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Bite, null);
        AddCombatMessage($"$0 rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Claw, null);
        AddCombatMessage($"$0 attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Kick, null);
        AddCombatMessage($"$0 attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Strike, null);
        AddCombatMessage($"$0 sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Sweep, null);
        AddCombatMessage($"$0 slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Slam, null);
        AddCombatMessage($"$0 attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 2, MeleeWeaponVerb.Punch, null);
        AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 1, null, null);

        AddCombatMessage($"$0 clumsily swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorFail);

        AddCombatMessage($"$0 adeptly swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.ClinchUnarmedAttack, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorPass);

        #endregion

        #region Trip

        AddCombatMessage($"$0 swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Swing, null);
        AddCombatMessage($"$0 thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Thrust, null);
        AddCombatMessage($"$0 jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Jab, null);
        AddCombatMessage($"$0 stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Stab, null);
        AddCombatMessage($"$0 attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Bash, null);
        AddCombatMessage($"$0 attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Bite, null);
        AddCombatMessage($"$0 rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Claw, null);
        AddCombatMessage($"$0 attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Kick, null);
        AddCombatMessage($"$0 attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Strike, null);
        AddCombatMessage($"$0 sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Sweep, null);
        AddCombatMessage($"$0 slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Slam, null);
        AddCombatMessage($"$0 attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Punch, null);
        AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 1, null, null);

        AddCombatMessage($"$0 clumsily swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorFail);

        AddCombatMessage($"$0 adeptly swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorPass);

        AddCombatMessage($"$0 swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Swing, null);
        AddCombatMessage($"$0 thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Thrust, null);
        AddCombatMessage($"$0 jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Jab, null);
        AddCombatMessage($"$0 stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Stab, null);
        AddCombatMessage($"$0 attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Bash, null);
        AddCombatMessage($"$0 attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Bite, null);
        AddCombatMessage($"$0 rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Claw, null);
        AddCombatMessage($"$0 attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Kick, null);
        AddCombatMessage($"$0 attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Strike, null);
        AddCombatMessage($"$0 sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Sweep, null);
        AddCombatMessage($"$0 slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Slam, null);
        AddCombatMessage($"$0 attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 2, MeleeWeaponVerb.Punch, null);
        AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 1, null, null);

        AddCombatMessage($"$0 clumsily swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorFail);

        AddCombatMessage($"$0 adeptly swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.UnbalancingBlowClinch, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorPass);

        #endregion

        #region Stagger

        AddCombatMessage($"$0 swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Swing, null);
        AddCombatMessage($"$0 thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Thrust, null);
        AddCombatMessage($"$0 jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Jab, null);
        AddCombatMessage($"$0 stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Stab, null);
        AddCombatMessage($"$0 attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Bash, null);
        AddCombatMessage($"$0 attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Bite, null);
        AddCombatMessage($"$0 rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Claw, null);
        AddCombatMessage($"$0 attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Kick, null);
        AddCombatMessage($"$0 attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Strike, null);
        AddCombatMessage($"$0 sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Sweep, null);
        AddCombatMessage($"$0 slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Slam, null);
        AddCombatMessage($"$0 attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 2, MeleeWeaponVerb.Punch, null);
        AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 1, null, null);

        AddCombatMessage($"$0 clumsily swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorFail);

        AddCombatMessage($"$0 adeptly swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowUnarmed, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorPass);

        AddCombatMessage($"$0 swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Swing, null);
        AddCombatMessage($"$0 thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Thrust, null);
        AddCombatMessage($"$0 jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Jab, null);
        AddCombatMessage($"$0 stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Stab, null);
        AddCombatMessage($"$0 attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Bash, null);
        AddCombatMessage($"$0 attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Bite, null);
        AddCombatMessage($"$0 rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Claw, null);
        AddCombatMessage($"$0 attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Kick, null);
        AddCombatMessage($"$0 attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Strike, null);
        AddCombatMessage($"$0 sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Sweep, null);
        AddCombatMessage($"$0 slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Slam, null);
        AddCombatMessage($"$0 attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 2, MeleeWeaponVerb.Punch, null);
        AddCombatMessage($"$0 attack|attacks $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 1, null, null);

        AddCombatMessage($"$0 clumsily swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorFail);
        AddCombatMessage($"$0 clumsily attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorFail);

        AddCombatMessage($"$0 adeptly swing|swings &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Swing, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly thrust|thrusts &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Thrust, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly jab|jabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Jab, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly stab|stabs at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Stab, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to bash $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Bash, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to bite $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Bite, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly rake|rakes at $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Claw, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to kick $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Kick, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to strike $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Strike, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly sweep|sweeps &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Sweep, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly slams|slams &0's {{0}} at $1{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Slam, Outcome.MajorPass);
        AddCombatMessage($"$0 adeptly attempt|attempts to punch $1 with &0's {{0}}{attackAddendum}", null,
            BuiltInCombatMoveType.StaggeringBlowClinch, 1.0, 3, MeleeWeaponVerb.Punch, Outcome.MajorPass);

        #endregion

        #endregion

        #region Built in Moves

        AddCombatMessage(Standalone("$0 rescue|rescues $1 from combat with $2"),
            Standalone("$0 attempt|attempts to rescue $1 from combat with $2, but is unsuccessful"), BuiltInCombatMoveType.Rescue,
            1.0, 1, null, null);
        AddCombatMessage(Standalone("$0 attempt|attempts to grapple $1"), null,
            BuiltInCombatMoveType.InitiateGrapple, 1.0, 1, null, null);
        AddCombatMessage(Standalone("$0 attempt|attempts to put $1's {1} into a lock"), null,
            BuiltInCombatMoveType.ExtendGrapple, 1.0, 1, null, null);
        AddCombatMessage(Standalone("$0 wrench|wrenches $1's {1} in an attempt to disable it"), null,
            BuiltInCombatMoveType.WrenchAttack, 1.0, 1, null, null);
        AddCombatMessage(Standalone("$0 viciously strangle|strangles $1's {1}"), null, BuiltInCombatMoveType.StrangleAttack, 1.0, 1,
            null, null);
        AddCombatMessage(Standalone("$0 attempt|attempts to disarm $1 with $2"), null, BuiltInCombatMoveType.Disarm, 1.0, 1, null,
            null);
        AddCombatMessage(Standalone("$0 attempt|attempts to flee from combat"), null, BuiltInCombatMoveType.Flee, 1.0, 1, null,
            null);
        AddCombatMessage(Standalone("$0 attempt|attempts to grab $1"), null, BuiltInCombatMoveType.RetrieveItem, 1.0, 1, null,
            null);
        AddCombatMessage(Standalone("$0 charge|charges into melee range with $1"),
            Standalone("$0 attempt|attempts to charge into melee range with $1, but fall|falls short"),
            BuiltInCombatMoveType.ChargeToMelee, 1.0, 1, null, null);
        AddCombatMessage(Standalone("$0 advance|advances into melee range with $1"),
            Standalone("$0 attempt|attempts to advance into melee range with $1, but fall|falls short"),
            BuiltInCombatMoveType.MoveToMelee, 1.0, 1, null, null);
        AddCombatMessage(Standalone("$0 {0} $2 at $1 as #0 %0|advance|advances into melee range with &1"),
            Standalone("$0 {0} $2 at $1 as #0 %0|attempt|attempts to advance into melee range with &1, but %0|fall|falls short"),
            BuiltInCombatMoveType.AdvanceAndFire, 1.0, 1, null, null);
        AddCombatMessage(Standalone("$0 receive|receives $1's charge with an attack of $2"), null,
            BuiltInCombatMoveType.ReceiveCharge, 1.0, 1, null, null);
        AddCombatMessage(Standalone("$0 step|steps close to $1 and attempt|attempts to begin a clinch with &1"), null,
            BuiltInCombatMoveType.StartClinch, 1.0, 1, null, null);
        AddCombatMessage(Standalone("$0 try|tries to break free of the clinch with $1"), null, BuiltInCombatMoveType.BreakClinch,
            1.0, 1, null, null);
        AddCombatMessage(Standalone("$0 drive|drives $1 back with $2"), null,
            BuiltInCombatMoveType.Pushback, 1.0, 1, null, null);
        AddCombatMessage(Standalone("@ drive|drives $1 back with &0's {0}"), null,
            BuiltInCombatMoveType.PushbackUnarmed, 1.0, 1, null, null);
        AddCombatMessage(Standalone("@ force|forces $1 back with &0's {0}"), null,
            BuiltInCombatMoveType.PushbackClinch, 1.0, 1, null, null);
        AddCombatMessage(Standalone("$0 force|forces $1 to move with $2"), null,
            BuiltInCombatMoveType.ForcedMovement, 1.0, 1, null, null);
        AddCombatMessage(Standalone("@ haul|hauls $1 with &0's {0}"), null,
            BuiltInCombatMoveType.ForcedMovementUnarmed, 1.0, 1, null, null);
        AddCombatMessage(Standalone("@ wrench|wrenches $1 into motion with &0's {0}"), null,
            BuiltInCombatMoveType.ForcedMovementClinch, 1.0, 1, null, null);
        AddCombatMessage(Standalone("@ stand|stands and {0} $2 at $1"), null, BuiltInCombatMoveType.StandAndFire, 1.0, 1, null,
            null);
        AddCombatMessage(Standalone("@ {0} $2 at $1 as &0 fall|falls back"), null, BuiltInCombatMoveType.SkirmishAndFire, 1.0, 1,
            null, null);
        AddCombatMessage(Standalone("@ {0} $2 at $1"), null, BuiltInCombatMoveType.RangedWeaponAttack, 1.0, 1, null, null);
        AddCombatMessage(Standalone("@ unleash|unleashes &0's {0} at $1"), null,
            BuiltInCombatMoveType.RangedNaturalAttack, 1.0, 1, null, null);
        AddCombatMessage(Standalone("@ exhale|exhales &0's {0} at $1"), null,
            BuiltInCombatMoveType.BreathWeaponAttack, 1.0, 1, null, null);
        AddCombatMessage(Standalone("@ spit|spits &0's {0} at $1"), null,
            BuiltInCombatMoveType.SpitNaturalAttack, 1.0, 1, null, null);
        AddCombatMessage(Standalone("@ hurl|hurls &0's {0} at $1"), null,
            BuiltInCombatMoveType.ExplosiveNaturalAttack, 1.0, 1, null, null);
        AddCombatMessage(Standalone("@ buffet|buffets $1 with &0's {0}"), null,
            BuiltInCombatMoveType.BuffetingNaturalAttack, 1.0, 1, null, null);
        AddCombatMessage(Standalone("@ aim|aims $2 at $1"), Standalone("@ continue|continues to aim $2 at $1"),
            BuiltInCombatMoveType.AimRangedWeapon, 1.0, 1, null, null);
        AddCombatMessage(Standalone("$0 attempt|attempts to coup-de-grace $1 with a blow to &1's {0} from $2"), null,
            BuiltInCombatMoveType.CoupDeGrace, 1.0, 1, null, null);
        AddCombatMessage(Standalone("$0 attack|attacks $1$?2| on $2||$ with &0's {0}, causing {1}"), null,
            BuiltInCombatMoveType.UnarmedSmashItem, 1.0, 1, null, null);
        AddCombatMessage(Standalone("$0 emit|emits a beam of energy towards $1"), null,
            BuiltInCombatMoveType.BeamAttack, 1.0, 1, null, null);
        AddCombatMessage(Standalone("$0 emit|emits a horrid screeching that hurts your ears"), null,
            BuiltInCombatMoveType.ScreechAttack, 1.0, 1, null, null);
        AddCombatMessage(Standalone("$0 reach|reaches out and attempt|attempts to get $1's {1} in a position to strangle &1"), null,
            BuiltInCombatMoveType.StrangleAttackExtendGrapple, 1.0, 1, null, null);

        #endregion

        #region Defenses

        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 %1|dodge|dodges out of the way"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 %1|attempt|attempts to dodge out of the way", "hit on &1's {1}"),
            BuiltInCombatMoveType.Dodge, 1.0, 1, null, null);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 %1|parry|parries with $3"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 %1|attempt|attempts to parry with $3", "hit on &1's {1}"),
            BuiltInCombatMoveType.Parry, 1.0, 1, null, null);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 %1|block|blocks with $3"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 %1|attempt|attempts to block with $3", "hit on &1's {1}"),
            BuiltInCombatMoveType.Block, 1.0, 1, null, null);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 gracelessly %1|dodge|dodges out of the way"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 gracelessly %1|attempt|attempts to dodge out of the way", "hit on &1's {1}"),
            BuiltInCombatMoveType.Dodge, 1.0, 2, null, Outcome.MajorFail);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 ineptly %1|parry|parries with $3"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 ineptly %1|attempt|attempts to parry with $3", "hit on &1's {1}"),
            BuiltInCombatMoveType.Parry, 1.0, 2, null, Outcome.MajorFail);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 ineptly %1|block|blocks with $3"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 ineptly %1|attempt|attempts to block with $3", "hit on &1's {1}"),
            BuiltInCombatMoveType.Block, 1.0, 2, null, Outcome.MajorFail);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 awkwardly %1|dodge|dodges out of the way"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 awkwardly %1|attempt|attempts to dodge out of the way", "hit on &1's {1}"),
            BuiltInCombatMoveType.Dodge, 1.0, 2, null, Outcome.Fail);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 clumsily %1|parry|parries with $3"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 clumsily %1|attempt|attempts to parry with $3", "hit on &1's {1}"),
            BuiltInCombatMoveType.Parry, 1.0, 2, null, Outcome.Fail);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 clumsily %1|block|blocks with $3"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 clumsily %1|attempt|attempts to block with $3", "hit on &1's {1}"),
            BuiltInCombatMoveType.Block, 1.0, 2, null, Outcome.Fail);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 nimbly %1|dodge|dodges out of the way"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 nimbly %1|attempt|attempts to dodge out of the way", "hit on &1's {1}"),
            BuiltInCombatMoveType.Dodge, 1.0, 2, null, Outcome.Pass);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 skillfully %1|parry|parries with $3"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 skillfully %1|attempt|attempts to parry with $3", "hit on &1's {1}"),
            BuiltInCombatMoveType.Parry, 1.0, 2, null, Outcome.Pass);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 skillfully %1|block|blocks with $3"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 skillfully %1|attempt|attempts to block with $3", "hit on &1's {1}"),
            BuiltInCombatMoveType.Block, 1.0, 2, null, Outcome.Pass);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 deftly %1|dodge|dodges out of the way"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 deftly %1|attempt|attempts to dodge out of the way", "hit on &1's {1}"),
            BuiltInCombatMoveType.Dodge, 1.0, 2, null, Outcome.MajorPass);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 masterfully %1|parry|parries with $3"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 masterfully %1|attempt|attempts to parry with $3", "hit on &1's {1}"),
            BuiltInCombatMoveType.Parry, 1.0, 2, null, Outcome.MajorPass);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 masterfully %1|block|blocks with $3"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 masterfully %1|attempt|attempts to block with $3", "hit on &1's {1}"),
            BuiltInCombatMoveType.Block, 1.0, 2, null, Outcome.MajorPass);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 desperately %1|dodge|dodges out of the way"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 desperately %1|attempt|attempts to dodge out of the way", "hit on &1's {1}"),
            BuiltInCombatMoveType.DesperateDodge, 1.0, 1, null, null);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 desperately %1|parry|parries with $3"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 desperately %1|attempt|attempts to parry with $3", "hit on &1's {1}"),
            BuiltInCombatMoveType.DesperateParry, 1.0, 1, null, null);
        AddCombatMessage(CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 desperately %1|block|blocks with $3"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 desperately %1|attempt|attempts to block with $3", "hit on &1's {1}"),
            BuiltInCombatMoveType.DesperateBlock, 1.0, 1, null, null);

        AddCombatMessage(
            CombatSeederMessageStyleHelper.BuildDefenseSuccess(messageStyle,
                "#1 %1|manage|manages to partially dodge the worst of the blow"),
            CombatSeederMessageStyleHelper.BuildDefenseFailure(messageStyle,
                "#1 %1|offer|offers no defense", "hit on &1's {1}", SeedCombatHitVerb.BeHit),
            BuiltInCombatMoveType.ClinchDodge, 1.0, 1, null, null);
        AddCombatMessage(Standalone("@ dodge|dodges out of the way"),
            Standalone("@ try|tries to dodge out of the way but fail|fails"),
            BuiltInCombatMoveType.DodgeRange, 1.0, 1, null, null);
        AddCombatMessage(Standalone("@ manage|manages to put $3 in the way"),
            Standalone("@ try|tries to put $3 in the way but fail|fails"),
            BuiltInCombatMoveType.BlockRange, 1.0, 1, null, null);

        AddCombatMessage($"{CombatSeederMessageStyleHelper.SuccessPrefix(messageStyle)}#1 %1|are|is able to avoid the attempt",
            $"{CombatSeederMessageStyleHelper.FailurePrefix(messageStyle)}#1 %1|aren't|isn't able to avoid it", BuiltInCombatMoveType.DodgeGrapple, 1.0, 1, null,
            null);
        AddCombatMessage($"{CombatSeederMessageStyleHelper.SuccessPrefix(messageStyle)}#1 %1|manage|manages to wriggle free",
            $"{CombatSeederMessageStyleHelper.FailurePrefix(messageStyle)}#1 %1|aren't|isn't able to wriggle free", BuiltInCombatMoveType.DodgeExtendGrapple, 1.0,
            1, null, null);
        AddCombatMessage($"{CombatSeederMessageStyleHelper.SuccessPrefix(messageStyle)}#1 %1|manage|manages to turn it around and become the grappler!",
            $"{CombatSeederMessageStyleHelper.FailurePrefix(messageStyle)}#1 %1|attempt|attempts to turn the grapple around, but is unsuccessful!",
            BuiltInCombatMoveType.CounterGrapple, 1.0, 1, null, null);

        #endregion
    }

    private void SeedShields(FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, string> questionAnswers,
        IReadOnlyDictionary<string, TraitDefinition> skills)
    {
        Account dbaccount = context.Accounts.First();
        DateTime now = DateTime.UtcNow;

        void CreateShieldComponent(ShieldType type)
        {
            GameItemComponentProto component = new()
            {
                Id = context.GameItemComponentProtos.Max(x => x.Id) + 1,
                RevisionNumber = 0,
                EditableItem = new EditableItem
                {
                    RevisionNumber = 0,
                    RevisionStatus = 4,
                    BuilderAccountId = dbaccount.Id,
                    BuilderDate = now,
                    BuilderComment = "Auto-generated by the system",
                    ReviewerAccountId = dbaccount.Id,
                    ReviewerComment = "Auto-generated by the system",
                    ReviewerDate = now
                },
                Type = "Shield",
                Name = $"Shield_{type.Name.Replace(' ', '_')}",
                Description = $"Turns an item into a {type.Name} shield",
                Definition =
                    $"<Definition><ShieldType>{type.Id}</ShieldType></Definition>"
            };
            context.GameItemComponentProtos.Add(component);
            context.SaveChanges();
        }

        ArmourType shieldArmour = new()
        {
            Name = "Shield Armour",
            MinimumPenetrationDegree = 1,
            BaseDifficultyDegrees = 0,
            StackedDifficultyDegrees = 0,
            Definition = @"<ArmourType>

	<!-- Damage Transformations change damage passed on to bones/organs/items into a different damage type when severity is under a certain  threshold 
		
		Damage Types:
		
		Slashing = 0
		Chopping = 1
		Crushing = 2
		Piercing = 3
		Ballistic = 4
		Burning = 5
		Freezing = 6
		Chemical = 7
		Shockwave = 8
		Bite = 9
		Claw = 10
		Electrical = 11
		Hypoxia = 12
		Cellular = 13
		Sonic = 14
		Shearing = 15
		ArmourPiercing = 16
		Wrenching = 17
		Shrapnel = 18
		Necrotic = 19
		Falling = 20
		Eldritch = 21
		Arcane = 22
		
		Severity Values:
		
		None = 0
		Superficial = 1
		Minor = 2
		Small = 3
		Moderate = 4
		Severe = 5
		VerySevere = 6
		Grievous = 7
		Horrifying = 8
	-->
	<DamageTransformations>
		<Transform fromtype=""0"" totype=""2"" severity=""6""></Transform> <!-- Slashing to Crushing when <= VerySevere -->
		<Transform fromtype=""1"" totype=""2"" severity=""6""></Transform> <!-- Chopping to Crushing when <= VerySevere -->
		<Transform fromtype=""3"" totype=""2"" severity=""5""></Transform> <!-- Piercing to Crushing when <= Severe -->
		<Transform fromtype=""4"" totype=""2"" severity=""5""></Transform> <!-- Ballistic to Crushing when <= Severe -->
		<Transform fromtype=""9"" totype=""2"" severity=""6""></Transform> <!-- Bite to Crushing when <= VerySevere -->
		<Transform fromtype=""10"" totype=""2"" severity=""6""></Transform> <!-- Claw to Crushing when <= VerySevere -->
		<Transform fromtype=""15"" totype=""2"" severity=""6""></Transform> <!-- Shearing to Crushing when <= VerySevere -->
		<Transform fromtype=""16"" totype=""2"" severity=""3""></Transform> <!-- ArmourPiercing to Crushing when <= Small -->
		<Transform fromtype=""17"" totype=""2"" severity=""5""></Transform> <!-- Wrenching to Crushing when <= Severe -->
	</DamageTransformations>
	<!-- 
	
		Dissipate expressions are applied before the item/part takes damage. 
		If they reduce the damage to zero, it neither suffers nor passes on any damage. 
		
		Parameters: 
		* damage, pain or stun (as appropriate) = the raw damage/pain/stun suffered
		* quality = the quality of the armour, rated 0 (Abysmal) to 11 (Legendary)
		* angle = the angle in radians of the attack (e.g. 1.5708rad = 90 degrees)
		* density = the density in kg/m3 of the material that the armour is made from
		* electrical = the electrical conductivity of the material that the armour is made from (1/ohm metres)
		* thermal = the thermal conductivity of the material that the armour is made from (watts per meter3 per kelvin)
		* organic = if the material that the armour is made from is organic (1 for true, 0 for false)
		* strength = either ImpactYield or ShearYield of the armour material depending on the damage type, in Pascals.
		
		Hint: 25000 can be considered ""base"" ShearYield and 10000 can be considered ""base"" ImpactYield
	-->
	<DissipateExpressions>
		<Expression damagetype=""0"">damage - (quality * strength/25000 * 0.25)</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">damage - (quality * strength/25000 * 0.25)</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">damage - (quality * strength/10000 * 0.25)</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">damage - (quality * strength/25000 * 0.25)</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">damage - (quality * strength/25000 * 0.25)</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">damage - (quality * 0.25)</Expression>    			      <!-- Burning -->
		<Expression damagetype=""6"">damage - (quality * 0.25)</Expression>                     <!-- Freezing -->
		<Expression damagetype=""7"">damage - (quality * 0.25)</Expression>                     <!-- Chemical -->
		<Expression damagetype=""8"">damage - (quality * strength/10000 * 0.25)</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">damage - (quality * strength/25000 * 0.25)</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">damage - (quality * strength/25000 * 0.25)</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">damage - (quality * 0.25)</Expression>                    <!-- Electrical -->
		<Expression damagetype=""12"">damage - (quality * 0.25)</Expression>                    <!-- Hypoxia -->
		<Expression damagetype=""13"">damage - (quality * 0.25)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">damage - (quality * strength/10000 * 0.25)</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">damage - (quality * strength/25000 * 0.25)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">damage - (quality * strength/25000 * 0.25)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage - (quality * strength/10000 * 0.25)</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">damage - (quality * strength/25000 * 0.25)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage - (quality * 0.25)</Expression>                    <!-- Necrotic -->   
		<Expression damagetype=""20"">damage - (quality * strength/10000 * 0.25)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage - (quality * 0.25)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">damage - (quality * 0.25)</Expression>                    <!-- Arcane -->   
	</DissipateExpressions>  
	<DissipateExpressionsPain>
		<Expression damagetype=""0"">pain - (quality * strength/25000 * 0.25)</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">pain - (quality * strength/25000 * 0.25)</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">pain - (quality * strength/10000 * 0.25)</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">pain - (quality * strength/25000 * 0.25)</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">pain - (quality * strength/25000 * 0.25)</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">pain - (quality * 0.25)</Expression>    			        <!-- Burning -->
		<Expression damagetype=""6"">pain - (quality * 0.25)</Expression>                     <!-- Freezing -->
		<Expression damagetype=""7"">pain - (quality * 0.25)</Expression>                     <!-- Chemical -->
		<Expression damagetype=""8"">pain - (quality * strength/10000 * 0.25)</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">pain - (quality * strength/25000 * 0.25)</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">pain - (quality * strength/25000 * 0.25)</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">pain - (quality * 0.25)</Expression>                    <!-- Electrical -->
		<Expression damagetype=""12"">pain - (quality * 0.25)</Expression>                    <!-- Hypoxia -->
		<Expression damagetype=""13"">pain - (quality * 0.25)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">pain - (quality * strength/10000 * 0.25)</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">pain - (quality * strength/25000 * 0.25)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">pain - (quality * strength/25000 * 0.25)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain - (quality * strength/10000 * 0.25)</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">pain - (quality * strength/25000 * 0.25)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain - (quality * 0.25)</Expression>                    <!-- Necrotic -->   
		<Expression damagetype=""20"">pain - (quality * strength/10000 * 0.25)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain - (quality * 0.25)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">pain - (quality * 0.25)</Expression>                    <!-- Arcane -->   
	</DissipateExpressionsPain>  
	<DissipateExpressionsStun>
		<Expression damagetype=""0"">stun - (quality * strength/25000 * 0.25)</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">stun - (quality * strength/25000 * 0.25)</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">stun - (quality * strength/10000 * 0.25)</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">stun - (quality * strength/25000 * 0.25)</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">stun - (quality * strength/25000 * 0.25)</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">stun - (quality * 0.25)</Expression>    			        <!-- Burning -->
		<Expression damagetype=""6"">stun - (quality * 0.25)</Expression>                     <!-- Freezing -->
		<Expression damagetype=""7"">stun - (quality * 0.25)</Expression>                     <!-- Chemical -->
		<Expression damagetype=""8"">stun - (quality * strength/10000 * 0.25)</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">stun - (quality * strength/25000 * 0.25)</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">stun - (quality * strength/25000 * 0.25)</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">stun - (quality * 0.25)</Expression>                    <!-- Electrical -->
		<Expression damagetype=""12"">stun - (quality * 0.25)</Expression>                    <!-- Hypoxia -->
		<Expression damagetype=""13"">stun - (quality * 0.25)</Expression>                    <!-- Cellular -->
		<Expression damagetype=""14"">stun - (quality * strength/10000 * 0.25)</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">stun - (quality * strength/25000 * 0.25)</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">stun - (quality * strength/25000 * 0.25)</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun - (quality * strength/10000 * 0.25)</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">stun - (quality * strength/25000 * 0.25)</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun - (quality * 0.25)</Expression>                    <!-- Necrotic -->   
		<Expression damagetype=""20"">stun - (quality * strength/10000 * 0.25)</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun - (quality * 0.25)</Expression>                    <!-- Eldritch -->   
		<Expression damagetype=""22"">stun - (quality * 0.25)</Expression>                    <!-- Arcane -->   
	</DissipateExpressionsStun>  
	<!-- 
	
		Absorb expressions are applied after dissipate expressions and item/part damage. 
		The after-absorb values are what is passed on to anything ""below"" e.g. bones, organs, parts worn under armour, etc 
		
		Parameters: 
		* damage, pain or stun (as appropriate) = the residual damage/pain/stun after dissipate step
		* quality = the quality of the armour, rated 0 (Abysmal) to 11 (Legendary)
		* angle = the angle in radians of the attack (e.g. 1.5708rad = 90 degrees)
		* density = the density in kg/m3 of the material that the armour is made from
		* electrical = the electrical conductivity of the material that the armour is made from (1/ohm metres)
		* thermal = the thermal conductivity of the material that the armour is made from (watts per meter3 per kelvin)
		* organic = if the material that the armour is made from is organic (1 for true, 0 for false)
		* strength = either ImpactYield or ShearYield of the armour material depending on the damage type, in Pascals.
		
		Hint: 25000 can be considered ""base"" ShearYield and 10000 can be considered ""base"" ImpactYield
		
		-->
	<AbsorbExpressions>
		<Expression damagetype=""0"">damage*0.2</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">damage*0.2</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">damage*0.2</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">damage*0.2</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">damage*0.2</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">damage*0.2</Expression>    <!-- Burning -->
		<Expression damagetype=""6"">damage*0.2</Expression>    <!-- Freezing -->
		<Expression damagetype=""7"">damage*0.2</Expression>    <!-- Chemical -->
		<Expression damagetype=""8"">damage*0.2</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">damage*0.2</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">damage*0.2</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">damage*0.2</Expression>   <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>        <!-- Hypoxia -->
		<Expression damagetype=""13"">0</Expression>        <!-- Cellular -->
		<Expression damagetype=""14"">damage*0.2</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">damage*0.2</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">damage*0.2</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">damage*0.2</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">damage*0.2</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">damage*0.2</Expression>   <!-- Necrotic -->   
		<Expression damagetype=""20"">damage*0.2</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">damage*0.2</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">damage*0.2</Expression>   <!-- Arcane -->   
	</AbsorbExpressions>  
	<AbsorbExpressionsPain>
		<Expression damagetype=""0"">pain*0.2</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">pain*0.2</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">pain*0.2</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">pain*0.2</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">pain*0.2</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">pain*0.2</Expression>    <!-- Burning -->
		<Expression damagetype=""6"">pain*0.2</Expression>    <!-- Freezing -->
		<Expression damagetype=""7"">pain*0.2</Expression>    <!-- Chemical -->
		<Expression damagetype=""8"">pain*0.2</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">pain*0.2</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">pain*0.2</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">pain*0.2</Expression>   <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>        <!-- Hypoxia -->
		<Expression damagetype=""13"">0</Expression>        <!-- Cellular -->
		<Expression damagetype=""14"">pain*0.2</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">pain*0.2</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">pain*0.2</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">pain*0.2</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">pain*0.2</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">pain*0.2</Expression>   <!-- Necrotic -->   
		<Expression damagetype=""20"">pain*0.2</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">pain*0.2</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">pain*0.2</Expression>   <!-- Arcane -->   
	</AbsorbExpressionsPain>  
	<AbsorbExpressionsStun>
		<Expression damagetype=""0"">stun</Expression>    <!-- Slashing -->
		<Expression damagetype=""1"">stun</Expression>    <!-- Chopping -->  
		<Expression damagetype=""2"">stun</Expression>    <!-- Crushing -->  
		<Expression damagetype=""3"">stun</Expression>    <!-- Piercing -->  
		<Expression damagetype=""4"">stun</Expression>    <!-- Ballistic -->  
		<Expression damagetype=""5"">stun</Expression>    <!-- Burning -->
		<Expression damagetype=""6"">stun</Expression>    <!-- Freezing -->
		<Expression damagetype=""7"">stun</Expression>    <!-- Chemical -->
		<Expression damagetype=""8"">stun</Expression>    <!-- Shockwave -->
		<Expression damagetype=""9"">stun</Expression>    <!-- Bite -->
		<Expression damagetype=""10"">stun</Expression>   <!-- Claw -->
		<Expression damagetype=""11"">stun</Expression>   <!-- Electrical -->
		<Expression damagetype=""12"">0</Expression>        <!-- Hypoxia -->
		<Expression damagetype=""13"">0</Expression>        <!-- Cellular -->
		<Expression damagetype=""14"">stun</Expression>   <!-- Sonic -->
		<Expression damagetype=""15"">stun</Expression>   <!-- Shearing --> 
		<Expression damagetype=""16"">stun</Expression>   <!-- ArmourPiercing -->
		<Expression damagetype=""17"">stun</Expression>   <!-- Wrenching -->
		<Expression damagetype=""18"">stun</Expression>   <!-- Shrapnel -->   
		<Expression damagetype=""19"">stun</Expression>   <!-- Necrotic -->   
		<Expression damagetype=""20"">stun</Expression>   <!-- Falling -->   
		<Expression damagetype=""21"">stun</Expression>   <!-- Eldritch -->   
		<Expression damagetype=""22"">stun</Expression>   <!-- Arcane -->   
	</AbsorbExpressionsStun>
 </ArmourType>"
        };
        context.ArmourTypes.Add(shieldArmour);
        context.SaveChanges();

        TraitDefinition skill = context.TraitDefinitions.First(x => x.Name == "Blocking" || x.Name == "Block");

        ShieldType shield = new()
        {
            Name = "Improvised",
            EffectiveArmourType = shieldArmour,
            BlockBonus = -1.0,
            BlockTrait = skill,
            StaminaPerBlock = 10.0
        };
        context.ShieldTypes.Add(shield);
        context.SaveChanges();
        CreateShieldComponent(shield);

        shield = new ShieldType
        {
            Name = "Buckler",
            EffectiveArmourType = shieldArmour,
            BlockBonus = -1.0,
            BlockTrait = skill,
            StaminaPerBlock = 5.0
        };
        context.ShieldTypes.Add(shield);
        context.SaveChanges();
        CreateShieldComponent(shield);

        shield = new ShieldType
        {
            Name = "Kite",
            EffectiveArmourType = shieldArmour,
            BlockBonus = 0.0,
            BlockTrait = skill,
            StaminaPerBlock = 8.0
        };
        context.ShieldTypes.Add(shield);
        context.SaveChanges();
        CreateShieldComponent(shield);

        shield = new ShieldType
        {
            Name = "Round",
            EffectiveArmourType = shieldArmour,
            BlockBonus = 0.0,
            BlockTrait = skill,
            StaminaPerBlock = 8.0
        };
        context.ShieldTypes.Add(shield);
        context.SaveChanges();
        CreateShieldComponent(shield);

        shield = new ShieldType
        {
            Name = "Heater",
            EffectiveArmourType = shieldArmour,
            BlockBonus = 0.0,
            BlockTrait = skill,
            StaminaPerBlock = 8.0
        };
        context.ShieldTypes.Add(shield);
        context.SaveChanges();
        CreateShieldComponent(shield);

        shield = new ShieldType
        {
            Name = "Tower",
            EffectiveArmourType = shieldArmour,
            BlockBonus = 1.0,
            BlockTrait = skill,
            StaminaPerBlock = 10.0
        };
        context.ShieldTypes.Add(shield);
        context.SaveChanges();
        CreateShieldComponent(shield);
    }

    private void SeedDataUnarmed(FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, string> questionAnswers,
        IReadOnlyDictionary<string, TraitDefinition> skills)
    {
        Race human = context.Races.First(x => x.Name == "Humanoid");

        BodypartShape handshape = context.BodypartShapes.First(x => x.Name == "Hand");
        BodypartShape footshape = context.BodypartShapes.First(x => x.Name == "Foot");
        BodypartShape elbowshape = context.BodypartShapes.First(x => x.Name == "Elbow");
        BodypartShape kneeshape = context.BodypartShapes.First(x => x.Name == "Knee");
        BodypartShape shouldershape = context.BodypartShapes.First(x => x.Name == "Shoulder");
        BodypartShape foreheadshape = context.BodypartShapes.First(x => x.Name == "Forehead");
        BodypartShape mouthshape = context.BodypartShapes.First(x => x.Name == "Mouth");

        Dictionary<string, TraitDefinition> attributes = context.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3)
            .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
        TraitDefinition strength =
            attributes.GetValueOrDefault("Strength") ??
            attributes.GetValueOrDefault("Physique") ??
            attributes["Body"];
        TraitDefinition dex =
            attributes.GetValueOrDefault("Agility") ??
            attributes.GetValueOrDefault("Dexterity") ??
            attributes.GetValueOrDefault("Agility") ??
            attributes.GetValueOrDefault("Speed") ??
            attributes["Body"];
        context.SaveChanges();

        context.StaticConfigurations.Add(new StaticConfiguration
        {
            SettingName = "StaggeringBlowExpressionAttacker",
            Definition = $"(2*{strength.Alias}:{strength.Id})+(damage/6)+(stun/12)"
        });
        context.StaticConfigurations.Add(new StaticConfiguration
        {
            SettingName = "StaggeringBlowExpressionDefender",
            Definition = $"(2*{strength.Alias}:{strength.Id})"
        });

        context.StaticConfigurations.Add(new StaticConfiguration
        {
            SettingName = "UnbalancingBlowExpressionAttacker",
            Definition = $"((2*{dex.Alias}:{dex.Id})+(damage/6)+(stun/12)) * ((3 + degree) / 3)"
        });
        context.StaticConfigurations.Add(new StaticConfiguration
        {
            SettingName = "UnbalancingBlowExpressionDefender",
            Definition = $"(2*{dex.Alias}:{dex.Id})* ((3 + degree - 2 * limbs)/3)"
        });

        context.StaticConfigurations.Add(new StaticConfiguration
        {
            SettingName = "BreakoutAttackerStrengthExpression",
            Definition = $"((2*{strength.Alias}:{strength.Id})-(3*limbs)"
        });
        context.StaticConfigurations.Add(new StaticConfiguration
        {
            SettingName = "BreakoutDefenderStrengthExpression",
            Definition = $"(2*{strength.Alias}:{strength.Id})"
        });

        context.StaticConfigurations.Add(new StaticConfiguration
        {
            SettingName = "DownedMeleeStaggerEffectLength",
            Definition = "12000"
        });

        string attackAddendum = "";
        switch (questionAnswers["messagestyle"].ToLowerInvariant())
        {
            case "sentences":
            case "sparse":
                attackAddendum = ".";
                break;
        }

        IReadOnlyDictionary<string, string> damageExpressions = BuildUnarmedDamageExpressions(strength.Id, questionAnswers);
        TraitExpression terribleDamage = UpsertTraitExpression(context, "Unarmed Damage - Terrible",
            damageExpressions["Unarmed Damage - Terrible"]);
        TraitExpression badDamage = UpsertTraitExpression(context, "Unarmed Damage - Bad",
            damageExpressions["Unarmed Damage - Bad"]);
        TraitExpression normalDamage = UpsertTraitExpression(context, "Unarmed Damage - Normal",
            damageExpressions["Unarmed Damage - Normal"]);
        TraitExpression goodDamage = UpsertTraitExpression(context, "Unarmed Damage - Good",
            damageExpressions["Unarmed Damage - Good"]);
        TraitExpression greatDamage = UpsertTraitExpression(context, "Unarmed Damage - Great",
            damageExpressions["Unarmed Damage - Great"]);

        void AddAttack(string name, BuiltInCombatMoveType moveType, MeleeWeaponVerb verb, Difficulty attacker,
            Difficulty dodge, Difficulty parry, Difficulty block, Alignment alignment, Orientation orientation,
            double stamina, double relativeSpeed, BodypartShape shape, TraitExpression damage, string attackMessage,
            DamageType damageType = DamageType.Crushing, double weighting = 100,
            CombatMoveIntentions intentions = CombatMoveIntentions.Attack | CombatMoveIntentions.Wound,
			string? additionalInfo = null)
        {
            WeaponAttack attack = new()
            {
                Verb = (int)verb,
                BaseAttackerDifficulty = (int)attacker,
                BaseBlockDifficulty = (int)block,
                BaseDodgeDifficulty = (int)dodge,
                BaseParryDifficulty = (int)parry,
                MoveType = (int)moveType,
                RecoveryDifficultySuccess = (int)Difficulty.Easy,
                RecoveryDifficultyFailure = (int)Difficulty.Hard,
                Intentions = (long)intentions,
                Weighting = weighting,
                ExertionLevel = (int)ExertionLevel.Heavy,
                DamageType = (int)damageType,
                DamageExpression = damage,
                StunExpression = damage,
                PainExpression = damage,
                BodypartShapeId = shape.Id,
                StaminaCost = stamina,
                BaseDelay = relativeSpeed,
                Name = name,
                Orientation = (int)orientation,
                Alignment = (int)alignment,
                HandednessOptions = 0,
                AdditionalInfo = additionalInfo
            };
            context.WeaponAttacks.Add(attack);
            context.SaveChanges();

            foreach (BodypartProto? bodypart in context.BodypartProtos.Where(x => x.BodypartShapeId == shape.Id))
            {
                context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
                {
                    Bodypart = bodypart,
                    Race = human,
                    WeaponAttack = attack,
                    Quality = (int)ItemQuality.Standard
                });
            }

            context.SaveChanges();

            CombatMessage message = new()
            {
                Type = (int)moveType,
                Message = attackMessage,
                Priority = 50,
                Verb = (int)verb,
                Chance = 1.0,
                FailureMessage = attackMessage
            };
            message.CombatMessagesWeaponAttacks.Add(new CombatMessagesWeaponAttacks
            { CombatMessage = message, WeaponAttack = attack });
            context.CombatMessages.Add(message);
            context.SaveChanges();
        }

        AddAttack("Jab", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Normal,
            Difficulty.Hard, Difficulty.Hard, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 3.0, 0.75,
            handshape, badDamage, $"@ throw|throws a quick @hand-hand jab at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Fast);
        AddAttack("High Jab", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Normal,
            Difficulty.Hard, Difficulty.Hard, Difficulty.Normal, Alignment.FrontRight, Orientation.Highest, 3.0, 0.8,
            handshape, badDamage, $"@ throw|throws a high @hand-hand jab at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Fast |
                        CombatMoveIntentions.Stun);
        AddAttack("Low Jab", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Normal,
            Difficulty.Hard, Difficulty.Hard, Difficulty.Normal, Alignment.FrontRight, Orientation.Centre, 3.0, 0.8,
            handshape, badDamage, $"@ throw|throws a low @hand-hand jab at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Fast);

        AddAttack("Hook", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            handshape, normalDamage, $"@ throw|throws a @hand-hand hook punch at $1{attackAddendum}");
        AddAttack("High Hook", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.Highest, 5.0,
            1.05, handshape, normalDamage, $"@ throw|throws a high @hand-hand hook punch at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun);
        AddAttack("Low Hook", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.Centre, 5.0, 1.05,
            handshape, normalDamage, $"@ throw|throws a low @hand-hand hook punch at $1{attackAddendum}");

        AddAttack("Cross", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Punch, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.High, 5.0, 1.0,
            handshape, normalDamage, $"@ throw|throws a @hand-hand cross punch at $1{attackAddendum}");
        AddAttack("High Cross", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Punch, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.Front, Orientation.Highest, 5.0, 1.05,
            handshape, normalDamage, $"@ throw|throws a high @hand-hand cross punch at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun);
        AddAttack("Low Cross", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Punch, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Centre, 5.0, 1.05,
            handshape, normalDamage, $"@ throw|throws a low @hand-hand cross punch at $1{attackAddendum}");

        AddAttack("Haymaker", BuiltInCombatMoveType.StaggeringBlowUnarmed, MeleeWeaponVerb.Jab, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.FrontRight, Orientation.High, 6.0, 1.25,
            handshape, goodDamage, $"@ throw|throws a @hand-hand haymaker punch at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Slow,
            additionalInfo: "4");
        AddAttack("High Haymaker", BuiltInCombatMoveType.StaggeringBlowUnarmed, MeleeWeaponVerb.Jab, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Alignment.FrontRight, Orientation.Highest, 6.0, 1.3,
            handshape, goodDamage, $"@ throw|throws a high @hand-hand haymaker punch at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Slow |
                        CombatMoveIntentions.Stun, additionalInfo: "4");
        AddAttack("Low Haymaker", BuiltInCombatMoveType.StaggeringBlowUnarmed, MeleeWeaponVerb.Jab, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.FrontRight, Orientation.Centre, 6.0, 1.3,
            handshape, goodDamage, $"@ throw|throws a low @hand-hand haymaker punch at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Slow,
            additionalInfo: "4");

        AddAttack("Uppercut", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Strike, Difficulty.Hard,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 6.0, 1.25,
            handshape, greatDamage, $"@ throw|throws a @hand-hand uppercut at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Slow |
                        CombatMoveIntentions.Hard);
        AddAttack("High Uppercut", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Strike, Difficulty.Hard,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Alignment.Front, Orientation.Highest, 6.0, 1.3,
            handshape, greatDamage, $"@ throw|throws a high @hand-hand uppercut at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Slow |
                        CombatMoveIntentions.Stun | CombatMoveIntentions.Hard);
        AddAttack("Low Uppercut", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Strike, Difficulty.Hard,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.Centre, 6.0, 1.3,
            handshape, greatDamage, $"@ throw|throws a low @hand-hand uppercut at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Slow |
                        CombatMoveIntentions.Hard);

        AddAttack("Check Hook", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            handshape, normalDamage,
            $"@ step|steps back and throw|throws a @hand-hand check hook counter-punch at $1{attackAddendum}");
        AddAttack("High Check Hook", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.05, handshape, normalDamage,
            $"@ step|steps back and throw|throws a high @hand-hand check hook counter-punch at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun);
        AddAttack("Low Check Hook", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.Centre, 5.0,
            1.05, handshape, normalDamage,
            $"@ step|steps back and throw|throws a low @hand-hand check hook counter-punch at $1{attackAddendum}");

        AddAttack("Stepback Jab", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Jab, Difficulty.Easy,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 2.0, 0.5,
            handshape, badDamage,
            $"@ step|steps back and throw|throws a quick @hand-hand jab counter-punch at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Fast | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);
        AddAttack("High Stepback Jab", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 2.0, 0.5, handshape, badDamage,
            $"@ step|steps back and throw|throws a high @hand-hand jab counter-punch at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Fast | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Stun);
        AddAttack("Low Stepback Jab", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Jab, Difficulty.Easy,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.Centre, 2.0, 0.5,
            handshape, badDamage,
            $"@ step|steps back and throw|throws a low @hand-hand jab counter-punch at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Fast | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);

        AddAttack("Stepback Roundhouse", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Kick,
            Difficulty.VeryHard, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Right, Orientation.High,
            7.0, 1.0, footshape, greatDamage,
            $"@ step|steps back and throw|throws a @hand-leg roundhouse counter-kick at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);
        AddAttack("High Stepback Roundhouse", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Kick,
            Difficulty.VeryHard, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Right,
            Orientation.Highest, 7.0, 1.0, footshape, greatDamage,
            $"@ step|steps back and throw|throws a high @hand-leg roundhouse counter-kick at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Stun);
        AddAttack("Low Stepback Roundhouse", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Kick,
            Difficulty.VeryHard, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Right, Orientation.Centre,
            7.0, 1.0, footshape, greatDamage,
            $"@ step|steps back and throw|throws a low @hand-leg roundhouse counter-kick at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);

        AddAttack("Stepback Snap Kick", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Kick,
            Difficulty.Hard, Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.Right, Orientation.High,
            7.0, 1.0, footshape, goodDamage,
            $"@ step|steps back and throw|throws a @hand-leg snap counter-kick at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);
        AddAttack("High Stepback Snap Kick", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Kick,
            Difficulty.Hard, Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Alignment.Right,
            Orientation.Highest, 7.0, 1.0, footshape, goodDamage,
            $"@ step|steps back and throw|throws a high  @hand-leg snap counter-kick at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Stun);
        AddAttack("Low Stepback Snap Kick", BuiltInCombatMoveType.WardFreeUnarmedAttack, MeleeWeaponVerb.Kick,
            Difficulty.Hard, Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.Right, Orientation.Centre,
            7.0, 1.0, footshape, goodDamage,
            $"@ step|steps back and throw|throws a low @hand-leg snap counter-kick at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);

        AddAttack("Roundhouse", BuiltInCombatMoveType.StaggeringBlowUnarmed, MeleeWeaponVerb.Kick, Difficulty.VeryHard,
            Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Right, Orientation.High, 7.0, 1.5, footshape,
            greatDamage, $"@ throw|throws a @hand-leg roundhouse kick at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound,
            additionalInfo: "6");
        AddAttack("High Roundhouse", BuiltInCombatMoveType.StaggeringBlowUnarmed, MeleeWeaponVerb.Kick,
            Difficulty.VeryHard, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Right,
            Orientation.Highest, 7.0, 1.5, footshape, greatDamage,
            $"@ throw|throws a high @hand-leg roundhouse kick at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Stun, additionalInfo: "6");
        AddAttack("Low Roundhouse", BuiltInCombatMoveType.StaggeringBlowUnarmed, MeleeWeaponVerb.Kick,
            Difficulty.VeryHard, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Alignment.Right, Orientation.Centre,
            7.0, 1.5, footshape, greatDamage, $"@ throw|throws a low @hand-leg roundhouse kick at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound,
            additionalInfo: "6");

        AddAttack("Snap Kick", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Kick, Difficulty.Hard,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.Right, Orientation.High, 7.0, 1.0, footshape,
            goodDamage, $"@ throw|throws a @hand-leg snap kick at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);
        AddAttack("High Snap Kick", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Kick, Difficulty.Hard,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Alignment.Right, Orientation.Highest, 7.0, 1.0,
            footshape, goodDamage, $"@ throw|throws a high  @hand-leg snap kick at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Stun);
        AddAttack("Low Snap Kick", BuiltInCombatMoveType.NaturalWeaponAttack, MeleeWeaponVerb.Kick, Difficulty.Hard,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.Right, Orientation.Centre, 7.0, 1.0,
            footshape, goodDamage, $"@ throw|throws a low @hand-leg snap kick at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound);

        AddAttack("Sweep Kick", BuiltInCombatMoveType.UnbalancingBlowUnarmed, MeleeWeaponVerb.Kick, Difficulty.Hard,
            Difficulty.Hard, Difficulty.Hard, Difficulty.VeryHard, Alignment.Right, Orientation.High, 6.0, 1.2,
            footshape, normalDamage, $"@ throw|throws a @hand-leg snap counter-kick at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound,
            additionalInfo: "7");
        AddAttack("Low Sweep Kick", BuiltInCombatMoveType.UnbalancingBlowUnarmed, MeleeWeaponVerb.Kick, Difficulty.Hard,
            Difficulty.Hard, Difficulty.Hard, Difficulty.ExtremelyHard, Alignment.Right, Orientation.High, 6.0, 1.2,
            footshape, normalDamage, $"@ throw|throws a low @hand-leg sweep kick at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Hard | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound,
            additionalInfo: "8");

        AddAttack("Prone Body Kick", BuiltInCombatMoveType.DownedAttackUnarmed, MeleeWeaponVerb.Kick,
            Difficulty.VeryEasy, Difficulty.Hard, Difficulty.Hard, Difficulty.VeryHard, Alignment.Front,
            Orientation.Centre, 4.0, 1.2, footshape, normalDamage,
            $"@ throw|throws a cruel @hand-leg kick at $1's prone body{attackAddendum}",
            intentions: CombatMoveIntentions.Easy | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Cruel | CombatMoveIntentions.Kill, additionalInfo: "6");
        AddAttack("Prone Back Kick", BuiltInCombatMoveType.DownedAttackUnarmed, MeleeWeaponVerb.Kick,
            Difficulty.VeryEasy, Difficulty.Hard, Difficulty.Hard, Difficulty.VeryHard, Alignment.Rear,
            Orientation.Centre, 4.0, 1.2, footshape, normalDamage,
            $"@ throw|throws a cruel @hand-leg kick at $1's prone back{attackAddendum}",
            intentions: CombatMoveIntentions.Easy | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Cruel | CombatMoveIntentions.Kill, additionalInfo: "6");
        AddAttack("Prone Head Kick", BuiltInCombatMoveType.DownedAttackUnarmed, MeleeWeaponVerb.Kick, Difficulty.Easy,
            Difficulty.Hard, Difficulty.Hard, Difficulty.VeryHard, Alignment.Front, Orientation.Highest, 4.0, 1.2,
            footshape, normalDamage, $"@ throw|throws a cruel @hand-leg kick at $1's head{attackAddendum}",
            intentions: CombatMoveIntentions.Easy | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Cruel | CombatMoveIntentions.Kill, additionalInfo: "6");
        AddAttack("Prone Leg Kick", BuiltInCombatMoveType.DownedAttackUnarmed, MeleeWeaponVerb.Kick,
            Difficulty.ExtremelyEasy, Difficulty.Hard, Difficulty.Hard, Difficulty.VeryHard, Alignment.Front,
            Orientation.Low, 4.0, 1.2, footshape, normalDamage,
            $"@ throw|throws a cruel @hand-leg kick at $1's prone legs{attackAddendum}",
            intentions: CombatMoveIntentions.Easy | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Cruel | CombatMoveIntentions.Hinder, additionalInfo: "2");
        AddAttack("Prone Arm Kick", BuiltInCombatMoveType.DownedAttackUnarmed, MeleeWeaponVerb.Kick,
            Difficulty.ExtremelyEasy, Difficulty.Hard, Difficulty.Hard, Difficulty.VeryHard, Alignment.Front,
            Orientation.Appendage, 4.0, 1.2, footshape, normalDamage,
            $"@ throw|throws a cruel @hand-leg kick at $1's prone arms{attackAddendum}",
            intentions: CombatMoveIntentions.Easy | CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Cruel | CombatMoveIntentions.Hinder, additionalInfo: "2");

        AddAttack("Body Punch", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Punch, Difficulty.Easy,
            Difficulty.ExtremelyHard, Difficulty.ExtremelyHard, Difficulty.Easy, Alignment.Right, Orientation.Centre,
            3.0, 0.7, handshape, badDamage, $"@ throw|throws a @hand-hand punch at $1's side{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Easy);
        AddAttack("Front Body Punch", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Punch,
            Difficulty.Normal, Difficulty.ExtremelyHard, Difficulty.ExtremelyHard, Difficulty.Easy,
            Alignment.FrontRight, Orientation.Centre, 3.0, 0.7, handshape, badDamage,
            $"@ throw|throws a @hand-hand punch at $1's mid-section{attackAddendum}");
        AddAttack("Overhand Punch", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Punch, Difficulty.Hard,
            Difficulty.Hard, Difficulty.ExtremelyHard, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 5.0,
            0.7, handshape, goodDamage, $"@ throw|throws a @hand-handed drop punch at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun |
                        CombatMoveIntentions.Hard);
        AddAttack("Headbutt", BuiltInCombatMoveType.StaggeringBlowClinch, MeleeWeaponVerb.Strike, Difficulty.VeryHard,
            Difficulty.VeryHard, Difficulty.ExtremelyHard, Difficulty.VeryHard, Alignment.Front, Orientation.Highest,
            5.0, 1.0, foreheadshape, greatDamage,
            $"@ lunge|lunges forward and throw|throws a headbutt at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun |
                        CombatMoveIntentions.Hard | CombatMoveIntentions.SelfDamaging | CombatMoveIntentions.Risky |
                        CombatMoveIntentions.Savage, additionalInfo: "6");
        AddAttack("Bite", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Bite, Difficulty.VeryHard,
            Difficulty.Normal, Difficulty.ExtremelyHard, Difficulty.VeryHard, Alignment.Front, Orientation.High, 3.0,
            1.4, mouthshape, normalDamage, $"@ lean|leans in and try|tries to bite $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun |
                        CombatMoveIntentions.Hard | CombatMoveIntentions.Risky | CombatMoveIntentions.Savage);
        AddAttack("Elbow", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Strike, Difficulty.Hard,
            Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.Easy, Alignment.FrontRight, Orientation.Highest, 5.0,
            1.0, elbowshape, greatDamage, $"@ try|tries to strike $1 with &0's {{0}}{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun |
                        CombatMoveIntentions.Risky | CombatMoveIntentions.Hard);
        AddAttack("Foot Stomp", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Kick, Difficulty.Hard,
            Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.Easy, Alignment.FrontRight, Orientation.Lowest, 2.0,
            0.8, footshape, goodDamage, $"@ try|tries to stomp on $1 with &0's {{0}}{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Stun |
                        CombatMoveIntentions.Risky | CombatMoveIntentions.Hard);
        AddAttack("Roundhouse Knee", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Kick, Difficulty.Hard,
            Difficulty.Hard, Difficulty.ExtremelyHard, Difficulty.Easy, Alignment.Right, Orientation.Centre, 6.0, 0.8,
            kneeshape, greatDamage, $"@ swing|swings &0's @hand leg in a roundhouse knee strike at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Hard);
        AddAttack("Low Roundhouse Knee", BuiltInCombatMoveType.ClinchUnarmedAttack, MeleeWeaponVerb.Kick,
            Difficulty.Hard, Difficulty.Hard, Difficulty.ExtremelyHard, Difficulty.Easy, Alignment.Right,
            Orientation.Low, 6.0, 0.8, kneeshape, greatDamage,
            $"@ swing|swings &0's @hand leg in a low roundhouse knee strike at $1{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Hard);
        AddAttack("Shoulder Push", BuiltInCombatMoveType.UnbalancingBlowClinch, MeleeWeaponVerb.Punch,
            Difficulty.VeryEasy, Difficulty.Hard, Difficulty.ExtremelyHard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 3.0, 0.8, shouldershape, badDamage,
            $"@ strike|strikes $1 with &0's @hand shoulder in an attempt to knock &1 back{attackAddendum}",
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Hard,
            additionalInfo: "5");
    }
}
