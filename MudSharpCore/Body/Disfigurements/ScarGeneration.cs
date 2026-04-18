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
using MudSharp.TimeAndDate;

namespace MudSharp.Body.Disfigurements;

public static class ScarGeneration
{
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

		var overallChance = Math.Min(
			gameworld.GetStaticDouble("ScarGenerationOverallChanceUpperBound"),
			GetOccurrenceChance(owner, wound));
		if (!RandomUtilities.Roll(1.0, overallChance))
		{
			return;
		}

		var context = ScarGenerationSupport.GetContext(wound);
		owner.Body.AddScar(GenerateScar(gameworld, owner.Race, bodypart, context, owner.Location.DateTime()));
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

	internal static IScar GenerateScar(
		IFuturemud gameworld,
		Character.Heritage.IRace race,
		IBodypart bodypart,
		ScarWoundContext context,
		MudDateTime timeOfScarring,
		int? variantSeed = null)
	{
		return ScarGenerationSupport.CreateScar(gameworld, race, bodypart, context, timeOfScarring, variantSeed);
	}

	internal static IReadOnlyList<IScar> GenerateScarOptions(
		IFuturemud gameworld,
		Character.Heritage.IRace race,
		IBodypart bodypart,
		ScarWoundContext context,
		MudDateTime timeOfScarring,
		int count)
	{
		HashSet<string> seen = new(StringComparer.InvariantCultureIgnoreCase);
		List<IScar> scars = [];
		var attempts = Math.Max(count * 4, count + 2);
		for (var i = 0; i < attempts && scars.Count < count; i++)
		{
			var scar = GenerateScar(gameworld, race, bodypart, context, timeOfScarring, i);
			var key = $"{scar.ShortDescription}|{scar.FullDescription}";
			if (!seen.Add(key))
			{
				continue;
			}

			scars.Add(scar);
		}

		if (!scars.Any())
		{
			scars.Add(GenerateScar(gameworld, race, bodypart, context, timeOfScarring, 0));
		}

		return scars;
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
