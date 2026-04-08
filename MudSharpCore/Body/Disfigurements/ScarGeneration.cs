using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Body.Disfigurements;

public static class ScarGeneration
{
	private sealed record ScarCandidate(IScarTemplate Template, double Weight);

	public static void TryApplyScar(ICharacter owner, IWound wound)
	{
		var gameworld = owner.Gameworld;
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

		var context = ScarGenerationSupport.GetContext(wound);
		var candidates = GetCandidates(owner, bodypart, context).ToList();
		if (!candidates.Any())
		{
			return;
		}

		var overallChance = Math.Min(
			gameworld.GetStaticDouble("ScarGenerationOverallChanceUpperBound"),
			GetOccurrenceChance(owner, wound));
		if (!RandomUtilities.Roll(1.0, overallChance))
		{
			return;
		}

		var selected = candidates.GetWeightedRandom(x => x.Weight);
		if (selected is null)
		{
			return;
		}

		owner.Body.AddScar(selected.Template.ProduceScar(owner, bodypart));
	}

	internal static IEnumerable<IScarTemplate> GetCandidateTemplates(ICharacter owner, IWound wound, IBodypart bodypart)
	{
		var context = ScarGenerationSupport.GetContext(wound);
		return ScarTemplateIndex.GetSnapshot(owner.Gameworld).GetCandidates(owner.Body, bodypart, context);
	}

	internal static double GetOccurrenceChance(ICharacter owner, IWound wound)
	{
		var context = ScarGenerationSupport.GetContext(wound);
		var staticChance = GetStaticOccurrenceChance(owner.Gameworld, context);
		return ClampChance(owner.Gameworld, ApplyMeritModifiers(owner, wound, staticChance));
	}

	internal static double ApplyMeritModifiers(IHaveMerits owner, IWound wound, double baseChance)
	{
		var merits = owner.Merits
			.OfType<IScarChanceMerit>()
			.Where(x => x.Applies(owner) && x.AppliesTo(wound))
			.ToList();
		if (!merits.Any())
		{
			return baseChance;
		}

		var flatModifier = merits.Sum(x => x.FlatModifier);
		var multiplier = merits.Aggregate(1.0, (current, merit) => current * merit.Multiplier);
		return (baseChance + flatModifier) * multiplier;
	}

	private static IEnumerable<ScarCandidate> GetCandidates(ICharacter owner, IBodypart bodypart, ScarWoundContext context)
	{
		return ScarTemplateIndex.GetSnapshot(owner.Gameworld)
			.GetCandidates(owner.Body, bodypart, context)
			.Select(x => new ScarCandidate(x, GetSelectionWeight(x, context)))
			.Where(x => x.Weight > 0.0);
	}

	private static double GetSelectionWeight(IScarTemplate template, ScarWoundContext context)
	{
		return context.IsSurgery ? template.SurgeryHealingScarWeight : template.DamageHealingScarWeight;
	}

	private static double GetStaticOccurrenceChance(IFuturemud gameworld, ScarWoundContext context)
	{
		var baseChance = ScarGenerationSupport.GetBaseChance(gameworld, context) +
						TendedModifier(gameworld, context.BestTendedOutcome);
		if (context.IsSurgery)
		{
			return ClampChance(
				gameworld,
				baseChance +
				CheckDegreesModifier(gameworld, context.SurgeryCheckDegrees) +
				(context.HadInfection ? gameworld.GetStaticDouble("ScarGenerationOrganicSurgeryHadInfectionModifier") : 0.0) +
				(context.WasCleaned
					? gameworld.GetStaticDouble("ScarGenerationOrganicSurgeryCleanedModifier")
					: gameworld.GetStaticDouble("ScarGenerationOrganicSurgeryUncleanModifier")) +
				(context.WasAntisepticTreated ? gameworld.GetStaticDouble("ScarGenerationOrganicSurgeryAntisepticModifier") : 0.0) +
				(context.WasClosed
					? gameworld.GetStaticDouble("ScarGenerationOrganicSurgeryClosedModifier")
					: context.WasTraumaControlled
						? gameworld.GetStaticDouble("ScarGenerationOrganicSurgeryTraumaControlledModifier")
						: gameworld.GetStaticDouble("ScarGenerationOrganicSurgeryOpenModifier")));
		}

		return ClampChance(
			gameworld,
			baseChance +
			(context.HadInfection ? gameworld.GetStaticDouble("ScarGenerationOrganicDamageHadInfectionModifier") : 0.0) +
			(context.WasCleaned
				? gameworld.GetStaticDouble("ScarGenerationOrganicDamageCleanedModifier")
				: gameworld.GetStaticDouble("ScarGenerationOrganicDamageUncleanModifier")) +
			(context.WasAntisepticTreated ? gameworld.GetStaticDouble("ScarGenerationOrganicDamageAntisepticModifier") : 0.0) +
			(context.WasClosed
				? gameworld.GetStaticDouble("ScarGenerationOrganicDamageClosedModifier")
				: context.WasTraumaControlled
					? gameworld.GetStaticDouble("ScarGenerationOrganicDamageTraumaControlledModifier")
					: gameworld.GetStaticDouble("ScarGenerationOrganicDamageOpenModifier")));
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
		var maximum = gameworld.GetStaticDouble("ScarGenerationChanceClampMaximum");
		return Math.Max(0.0, Math.Min(maximum, chance));
	}
}
