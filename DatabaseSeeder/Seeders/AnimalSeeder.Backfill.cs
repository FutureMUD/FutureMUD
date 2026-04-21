#nullable enable

using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	private static bool HasMissingAnimalCatalogue(FuturemudDatabaseContext context)
	{
		if (new[] { "Beetle", "Centipede" }.Any(bodyName => !context.BodyProtos.Any(x => x.Name == bodyName)))
		{
			return true;
		}

		if (HeightWeightTemplates.Keys.Any(modelName => !context.HeightWeightModels.Any(x => x.Name == modelName)))
		{
			return true;
		}

		if (!context.WeaponAttacks.Any(x => x.Name == "Acid Spit"))
		{
			return true;
		}

		if (RaceTemplates.Keys.Any(raceName => !context.Races.Any(x => x.Name == raceName)))
		{
			return true;
		}

		Race? beetleRace = context.Races.FirstOrDefault(x => x.Name == "Beetle");
		if (beetleRace is not null && context.BodyProtos.FirstOrDefault(x => x.Id == beetleRace.BaseBodyId)?.Name != "Beetle")
		{
			return true;
		}

		return false;
	}

	private void BackfillAnimalCatalogue()
	{
		SetupHeightWeightModels();
		SetupAttacks(false);

		Dictionary<string, BodyProto> bodyLookup = EnsureBackfillAnimalBodies();
		MigrateBeetleRace(bodyLookup["Beetle"]);

		List<AnimalRaceTemplate> missingTemplates = RaceTemplates.Values
			.Where(template => !_context.Races.Any(x => x.Name == template.Name))
			.ToList();
		if (missingTemplates.Any())
		{
			SeedAnimalRaces(missingTemplates, bodyLookup.Select(x => (x.Key, x.Value)).ToArray());
		}

		ApplyDefaultCombatSettingsToSeededRaces();
	}

	private Dictionary<string, BodyProto> EnsureBackfillAnimalBodies()
	{
		BodyProto insectBody = _context.BodyProtos.First(x => x.Name == "Insectoid");
		WearableSizeParameterRule wearSize = _context.WearableSizeParameterRule.First();

		BodyProto beetleBody = _context.BodyProtos.FirstOrDefault(x => x.Name == "Beetle") ??
			CreateAnimalBodyShell("Beetle", insectBody, wearSize, "mandible", "mandibles", 6);
		if (!_context.BodypartProtos.Any(x => x.BodyId == beetleBody.Id))
		{
			CloneBodyDefinition(insectBody, beetleBody, cloneAdditionalUsages: false);
			CloneBodyPositionsAndSpeeds(insectBody, beetleBody);
		}

		BodyProto centipedeBody = _context.BodyProtos.FirstOrDefault(x => x.Name == "Centipede") ??
			CreateAnimalBodyShell("Centipede", insectBody, wearSize, "mandible", "mandibles", 8);
		if (!_context.BodypartProtos.Any(x => x.BodyId == centipedeBody.Id))
		{
			SeedCentipedeBody(centipedeBody);
			CloneBodyPositionsAndSpeeds(insectBody, centipedeBody);
		}

		return RaceTemplates.Values
			.Select(x => x.BodyKey)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToDictionary(
				x => x,
				x => _context.BodyProtos.First(body => body.Name == x),
				StringComparer.OrdinalIgnoreCase);
	}

	private BodyProto CreateAnimalBodyShell(string name, BodyProto countsAs, WearableSizeParameterRule wearSize,
		string wielderSingle, string wielderPlural, int minimumLegsToStand)
	{
		long nextId = _context.BodyProtos.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
		BodyProto body = new()
		{
			Id = nextId,
			CountsAs = countsAs,
			Name = name,
			ConsiderString = "",
			WielderDescriptionSingle = wielderSingle,
			WielderDescriptionPlural = wielderPlural,
			StaminaRecoveryProgId = countsAs.StaminaRecoveryProgId,
			MinimumLegsToStand = minimumLegsToStand,
			MinimumWingsToFly = countsAs.MinimumWingsToFly,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		_context.BodyProtos.Add(body);
		_context.SaveChanges();
		return body;
	}

	private void MigrateBeetleRace(BodyProto beetleBody)
	{
		Race? beetleRace = _context.Races.FirstOrDefault(x => x.Name == "Beetle");
		if (beetleRace is null || beetleRace.BaseBodyId == beetleBody.Id)
		{
			return;
		}

		beetleRace.BaseBody = beetleBody;
		_context.RacesWeaponAttacks.RemoveRange(_context.RacesWeaponAttacks.Where(x => x.RaceId == beetleRace.Id).ToList());
		_context.RacesAdditionalBodyparts.RemoveRange(_context.RacesAdditionalBodyparts.Where(x => x.RaceId == beetleRace.Id).ToList());
		_context.SaveChanges();

		CreateRaceAttacks(beetleRace);
		if (TryGetRaceTemplate(beetleRace.Name, out AnimalRaceTemplate? template) &&
		    template.AdditionalBodypartUsages is not null)
		{
			foreach (AnimalBodypartUsageTemplate usage in template.AdditionalBodypartUsages)
			{
				AddRacialBodypartUsage(usage.BodypartAlias, usage.Usage, beetleRace);
			}
		}

		_context.SaveChanges();
	}
}
