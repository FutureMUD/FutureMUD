using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Health;
using MudSharp.Health.Wounds;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Body.Disfigurements;

public static class ScarGeneration
{
    private sealed record ScarCandidate(IScarTemplate Template, double Chance);

    public static void TryApplyScar(ICharacter owner, IWound wound)
    {
        IFuturemud gameworld = owner.Gameworld;
        if (!gameworld.GetStaticBool("ScarringEnabled"))
        {
            return;
        }

        if (wound.Bodypart is not IExternalBodypart bodypart)
        {
            return;
        }

        if (!owner.Body.Bodyparts.Contains(bodypart))
        {
            return;
        }

        List<ScarCandidate> candidates = GetCandidates(owner, wound, bodypart).ToList();
        if (!candidates.Any())
        {
            return;
        }

        double overallChance = 1.0 - candidates.Aggregate(1.0, (current, candidate) => current * (1.0 - candidate.Chance));
        if (!RandomUtilities.Roll(1.0, Math.Max(0.0, Math.Min(gameworld.GetStaticDouble("ScarGenerationOverallChanceUpperBound"), overallChance))))
        {
            return;
        }

        ScarCandidate selected = candidates.GetWeightedRandom(x => x.Chance);
        if (selected is null)
        {
            return;
        }

        owner.Body.AddScar(selected.Template.ProduceScar(owner, bodypart));
    }

    private static IEnumerable<ScarCandidate> GetCandidates(ICharacter owner, IWound wound, IBodypart bodypart)
    {
        IFuturemud gameworld = owner.Gameworld;
        return owner.Gameworld.DisfigurementTemplates
                    .OfType<IScarTemplate>()
                    .Where(x => x.Status == RevisionStatus.Current)
                    .Where(x => x.CanBeAppliedToBodypart(owner.Body, bodypart))
                    .Select(x => (Template: x, Chance: GetChance(gameworld, x, wound)))
                    .Where(x => x.Chance > 0.0)
                    .Select(x => new ScarCandidate(x.Template, x.Chance));
    }

    private static double GetChance(IFuturemud gameworld, IScarTemplate template, IWound wound)
    {
        return wound switch
        {
            SimpleOrganicWound organicWound => GetOrganicWoundChance(gameworld, template, organicWound),
            HealingSimpleWound healingWound => GetHealingWoundChance(gameworld, template, healingWound),
            _ => 0.0
        };
    }

    private static double GetOrganicWoundChance(IFuturemud gameworld, IScarTemplate template, SimpleOrganicWound wound)
    {
        if (wound.ScarSurgicalProcedureType is SurgicalProcedureType surgicalProcedureType)
        {
            if (!template.CanBeAppliedFromSurgery(surgicalProcedureType))
            {
                return 0.0;
            }

            return ClampChance(
                gameworld,
                template.SurgeryHealingScarChance +
                ((int)wound.Severity * gameworld.GetStaticDouble("ScarGenerationOrganicSurgerySeverityPerLevel")) +
                CheckDegreesModifier(gameworld, wound.ScarSurgeryCheckDegrees) +
                TendedModifier(gameworld, wound.BestTendedOutcome) +
                (wound.HadInfection ? gameworld.GetStaticDouble("ScarGenerationOrganicSurgeryHadInfectionModifier") : 0.0) +
                (wound.WasCleaned
                    ? gameworld.GetStaticDouble("ScarGenerationOrganicSurgeryCleanedModifier")
                    : gameworld.GetStaticDouble("ScarGenerationOrganicSurgeryUncleanModifier")) +
                (wound.WasAntisepticTreated ? gameworld.GetStaticDouble("ScarGenerationOrganicSurgeryAntisepticModifier") : 0.0) +
                (wound.WasClosed
                    ? gameworld.GetStaticDouble("ScarGenerationOrganicSurgeryClosedModifier")
                    : wound.WasTraumaControlled
                        ? gameworld.GetStaticDouble("ScarGenerationOrganicSurgeryTraumaControlledModifier")
                        : gameworld.GetStaticDouble("ScarGenerationOrganicSurgeryOpenModifier")));
        }

        if (!template.CanBeAppliedFromDamage(wound.DamageType, wound.Severity))
        {
            return 0.0;
        }

        return ClampChance(
            gameworld,
            template.DamageHealingScarChance +
            ((int)wound.Severity * gameworld.GetStaticDouble("ScarGenerationOrganicDamageSeverityPerLevel")) +
            TendedModifier(gameworld, wound.BestTendedOutcome) +
            (wound.HadInfection ? gameworld.GetStaticDouble("ScarGenerationOrganicDamageHadInfectionModifier") : 0.0) +
            (wound.WasCleaned
                ? gameworld.GetStaticDouble("ScarGenerationOrganicDamageCleanedModifier")
                : gameworld.GetStaticDouble("ScarGenerationOrganicDamageUncleanModifier")) +
            (wound.WasAntisepticTreated ? gameworld.GetStaticDouble("ScarGenerationOrganicDamageAntisepticModifier") : 0.0) +
            (wound.WasClosed
                ? gameworld.GetStaticDouble("ScarGenerationOrganicDamageClosedModifier")
                : wound.WasTraumaControlled
                    ? gameworld.GetStaticDouble("ScarGenerationOrganicDamageTraumaControlledModifier")
                    : gameworld.GetStaticDouble("ScarGenerationOrganicDamageOpenModifier")));
    }

    private static double GetHealingWoundChance(IFuturemud gameworld, IScarTemplate template, HealingSimpleWound wound)
    {
        if (wound.ScarSurgicalProcedureType is SurgicalProcedureType surgicalProcedureType)
        {
            if (!template.CanBeAppliedFromSurgery(surgicalProcedureType))
            {
                return 0.0;
            }

            return ClampChance(
                gameworld,
                template.SurgeryHealingScarChance +
                ((int)wound.Severity * gameworld.GetStaticDouble("ScarGenerationHealingSurgerySeverityPerLevel")) +
                CheckDegreesModifier(gameworld, wound.ScarSurgeryCheckDegrees) +
                TendedModifier(gameworld, wound.BestTendedOutcome));
        }

        if (!template.CanBeAppliedFromDamage(wound.DamageType, wound.Severity))
        {
            return 0.0;
        }

        return ClampChance(
            gameworld,
            template.DamageHealingScarChance +
            ((int)wound.Severity * gameworld.GetStaticDouble("ScarGenerationHealingDamageSeverityPerLevel")) +
            TendedModifier(gameworld, wound.BestTendedOutcome));
    }

    private static double TendedModifier(IFuturemud gameworld, Outcome outcome)
    {
        return outcome switch
        {
            Outcome.MajorPass => gameworld.GetStaticDouble("ScarGenerationTendedOutcomeMajorPassModifier"),
            Outcome.Pass => gameworld.GetStaticDouble("ScarGenerationTendedOutcomePassModifier"),
            Outcome.MinorPass => gameworld.GetStaticDouble("ScarGenerationTendedOutcomeMinorPassModifier"),
            Outcome.MinorFail => gameworld.GetStaticDouble("ScarGenerationTendedOutcomeMinorFailModifier"),
            Outcome.Fail => gameworld.GetStaticDouble("ScarGenerationTendedOutcomeFailModifier"),
            Outcome.MajorFail => gameworld.GetStaticDouble("ScarGenerationTendedOutcomeMajorFailModifier"),
            _ => gameworld.GetStaticDouble("ScarGenerationTendedOutcomeDefaultModifier")
        };
    }

    private static double CheckDegreesModifier(IFuturemud gameworld, int checkDegrees)
    {
        return checkDegrees switch
        {
            >= 3 => gameworld.GetStaticDouble("ScarGenerationSurgeryCheckDegreesAtLeastThreeModifier"),
            2 => gameworld.GetStaticDouble("ScarGenerationSurgeryCheckDegreesTwoModifier"),
            1 => gameworld.GetStaticDouble("ScarGenerationSurgeryCheckDegreesOneModifier"),
            0 => gameworld.GetStaticDouble("ScarGenerationSurgeryCheckDegreesZeroModifier"),
            -1 => gameworld.GetStaticDouble("ScarGenerationSurgeryCheckDegreesMinusOneModifier"),
            -2 => gameworld.GetStaticDouble("ScarGenerationSurgeryCheckDegreesMinusTwoModifier"),
            _ => gameworld.GetStaticDouble("ScarGenerationSurgeryCheckDegreesMinusThreeOrLessModifier")
        };
    }

    private static double ClampChance(IFuturemud gameworld, double chance)
    {
        double maximum = gameworld.GetStaticDouble("ScarGenerationChanceClampMaximum");
        return Math.Max(0.0, Math.Min(maximum, chance));
    }
}
