#nullable enable

using MudSharp.Body;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	internal const string ActiveNoThirstNeedsRegister = "useactivenothirstneeds";

	internal const string WhichNeedsModelFunctionText = @"if (@ch.Guest)
  return ""Passive""
else
  if (GetRegister(@ch.Race, ""UseActiveNoThirstNeeds""))
	return ""ActiveNoThirst""
  end if
  if (not(@ch.NPC) or GetRegister(@ch.Race, ""UseActiveNeeds""))
	return ""Active""
  else
	return ""NoNeeds""
  end if
end if";

	internal static bool HasMissingAnimalNeedsModelConfiguration(FuturemudDatabaseContext context)
	{
		VariableDefinition? definition = context.VariableDefinitions.AsEnumerable().FirstOrDefault(x =>
			x.OwnerType == (long)ProgVariableTypes.Race &&
			string.Equals(x.Property, ActiveNoThirstNeedsRegister, StringComparison.OrdinalIgnoreCase));
		if (definition?.ContainedType != (long)ProgVariableTypes.Boolean)
		{
			return true;
		}

		VariableDefault? variableDefault = context.VariableDefaults.AsEnumerable().FirstOrDefault(x =>
			x.OwnerType == (long)ProgVariableTypes.Race &&
			string.Equals(x.Property, ActiveNoThirstNeedsRegister, StringComparison.OrdinalIgnoreCase));
		if (!string.Equals(variableDefault?.DefaultValue, "<var>False</var>", StringComparison.Ordinal))
		{
			return true;
		}

		FutureProg? needsProg = context.FutureProgs.AsEnumerable().FirstOrDefault(x =>
			string.Equals(x.FunctionName, "WhichNeedsModel", StringComparison.OrdinalIgnoreCase));
		if (needsProg is null || !string.Equals(needsProg.FunctionText, WhichNeedsModelFunctionText, StringComparison.Ordinal))
		{
			return true;
		}

		foreach (AnimalRaceTemplate template in RaceTemplates.Values)
		{
			Race? race = context.Races.AsEnumerable().FirstOrDefault(x => x.Name == template.Name);
			if (race is null || HasTrueRegisterValue(context, race.Id, ActiveNoThirstNeedsRegister) !=
				template.UsesActiveNoThirstNeeds)
			{
				return true;
			}
		}

		return false;
	}

	internal static void EnsureAnimalNeedsModelConfiguration(FuturemudDatabaseContext context)
	{
		VariableDefinition? definition = context.VariableDefinitions.Local.FirstOrDefault(x =>
			x.OwnerType == (long)ProgVariableTypes.Race &&
			string.Equals(x.Property, ActiveNoThirstNeedsRegister, StringComparison.OrdinalIgnoreCase)) ??
			context.VariableDefinitions.AsEnumerable().FirstOrDefault(x =>
				x.OwnerType == (long)ProgVariableTypes.Race &&
				string.Equals(x.Property, ActiveNoThirstNeedsRegister, StringComparison.OrdinalIgnoreCase));
		if (definition is null)
		{
			definition = new VariableDefinition
			{
				OwnerType = (long)ProgVariableTypes.Race,
				Property = ActiveNoThirstNeedsRegister,
				ContainedType = (long)ProgVariableTypes.Boolean
			};
			context.VariableDefinitions.Add(definition);
		}

		definition.OwnerType = (long)ProgVariableTypes.Race;
		definition.Property = ActiveNoThirstNeedsRegister;
		definition.ContainedType = (long)ProgVariableTypes.Boolean;

		VariableDefault? variableDefault = context.VariableDefaults.Local.FirstOrDefault(x =>
			x.OwnerType == (long)ProgVariableTypes.Race &&
			string.Equals(x.Property, ActiveNoThirstNeedsRegister, StringComparison.OrdinalIgnoreCase)) ??
			context.VariableDefaults.AsEnumerable().FirstOrDefault(x =>
				x.OwnerType == (long)ProgVariableTypes.Race &&
				string.Equals(x.Property, ActiveNoThirstNeedsRegister, StringComparison.OrdinalIgnoreCase));
		if (variableDefault is null)
		{
			variableDefault = new VariableDefault
			{
				OwnerType = (long)ProgVariableTypes.Race,
				Property = ActiveNoThirstNeedsRegister,
				DefaultValue = "<var>False</var>"
			};
			context.VariableDefaults.Add(variableDefault);
		}

		variableDefault.OwnerType = (long)ProgVariableTypes.Race;
		variableDefault.Property = ActiveNoThirstNeedsRegister;
		variableDefault.DefaultValue = "<var>False</var>";

		SeederRepeatabilityHelper.EnsureProg(
			context,
			"WhichNeedsModel",
			"Character",
			"Biology",
			ProgVariableTypes.Text,
			"Determines the needs model to use for a character",
			WhichNeedsModelFunctionText,
			true,
			false,
			FutureProgStaticType.NotStatic,
			(ProgVariableTypes.Character, "ch"));

		HashSet<string> aquaticNames = RaceTemplates.Values
			.Where(x => x.UsesActiveNoThirstNeeds)
			.Select(x => x.Name)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (Race race in context.Races
			         .AsEnumerable()
			         .Where(x => RaceTemplates.ContainsKey(x.Name))
			         .ToList())
		{
			EnsureRegisterValue(context, race.Id, ActiveNoThirstNeedsRegister, aquaticNames.Contains(race.Name));
		}
	}

	private static bool HasTrueRegisterValue(FuturemudDatabaseContext context, long raceId, string property)
	{
		return context.VariableValues.AsEnumerable().Any(x =>
			x.ReferenceType == (long)ProgVariableTypes.Race &&
			x.ReferenceId == raceId &&
			string.Equals(x.ReferenceProperty, property, StringComparison.OrdinalIgnoreCase) &&
			string.Equals(x.ValueDefinition, "<var>True</var>", StringComparison.Ordinal));
	}

	private static void EnsureRegisterValue(FuturemudDatabaseContext context, long raceId, string property, bool value)
	{
		VariableValue? existing = context.VariableValues.Local.FirstOrDefault(x =>
			x.ReferenceType == (long)ProgVariableTypes.Race &&
			x.ReferenceId == raceId &&
			string.Equals(x.ReferenceProperty, property, StringComparison.OrdinalIgnoreCase)) ??
			context.VariableValues.AsEnumerable().FirstOrDefault(x =>
				x.ReferenceType == (long)ProgVariableTypes.Race &&
				x.ReferenceId == raceId &&
				string.Equals(x.ReferenceProperty, property, StringComparison.OrdinalIgnoreCase));

		if (!value)
		{
			if (existing is not null)
			{
				context.VariableValues.Remove(existing);
			}

			return;
		}

		if (existing is null)
		{
			existing = new VariableValue
			{
				ReferenceType = (long)ProgVariableTypes.Race,
				ReferenceId = raceId,
				ReferenceProperty = property,
				ValueType = (long)ProgVariableTypes.Boolean,
				ValueDefinition = "<var>True</var>"
			};
			context.VariableValues.Add(existing);
		}

		existing.ReferenceType = (long)ProgVariableTypes.Race;
		existing.ReferenceId = raceId;
		existing.ReferenceProperty = property;
		existing.ValueType = (long)ProgVariableTypes.Boolean;
		existing.ValueDefinition = "<var>True</var>";
	}

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

		if (!context.WeaponAttacks.Any(x => x.Name == "Acid Spit") ||
		    !context.WeaponAttacks.Any(x => x.Name == "Massive Claw Sweep"))
		{
			return true;
		}

		if (RaceTemplates.Keys.Any(raceName => !context.Races.Any(x => x.Name == raceName)))
		{
			return true;
		}

		if (RaceTemplates.Values.Any(template =>
			    context.Races.FirstOrDefault(x => x.Name == template.Name) is { } race &&
			    !SatiationLimitSeederHelper.MatchesLimits(
				    race,
				    template.MaximumFoodSatiatedHours,
				    template.MaximumDrinkSatiatedHours)))
		{
			return true;
		}

		if (HasLegacyAnimalFluidNames(context))
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
		RefreshExistingAnimalBaseBodies();
		RepairLegacyAnimalFluidNames(_context);

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
		EnsureAnimalNeedsModelConfiguration(_context);
		_context.SaveChanges();
	}

	private void RefreshExistingAnimalBaseBodies()
	{
		BodyProto? avianBody = _context.BodyProtos.FirstOrDefault(x => x.Name == "Avian");
		if (avianBody is null)
		{
			return;
		}

		bool dirty = false;
		foreach (BodypartProto bodypart in _context.BodypartProtos
			         .Where(x => x.BodyId == avianBody.Id && AvianCoreWingAliases.Contains(x.Name))
			         .ToList())
		{
			if (bodypart.IsCore)
			{
				continue;
			}

			bodypart.IsCore = true;
			dirty = true;
		}

		if (dirty)
		{
			_context.SaveChanges();
		}
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
