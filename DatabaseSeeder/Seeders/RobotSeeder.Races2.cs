#nullable enable

using System.Linq;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
	private void SeedSimpleEthnicity(Race race, RobotRaceTemplate template)
	{
		if (_context.Ethnicities.Any(x => x.ParentRaceId == race.Id && x.Name == $"{template.Name} Stock"))
		{
			return;
		}

		_context.Ethnicities.Add(new Ethnicity
		{
			Name = $"{template.Name} Stock",
			ChargenBlurb = template.Description,
			AvailabilityProg = template.Playable ? _alwaysTrue : _alwaysFalse,
			ParentRace = race,
			EthnicGroup = template.Name,
			EthnicSubgroup = "Stock",
			PopulationBloodModel = _humanRace.Ethnicities.First().PopulationBloodModel,
			TolerableTemperatureFloorEffect = 0,
			TolerableTemperatureCeilingEffect = 0
		});
		_context.SaveChanges();
	}

	private void AddEthnicityCharacteristic(Ethnicity ethnicity, string definitionName, string profileName)
	{
		var definition = _context.CharacteristicDefinitions.FirstOrDefault(x => x.Name == definitionName);
		var profile = _context.CharacteristicProfiles.FirstOrDefault(x => x.Name == profileName);
		if (definition is null || profile is null)
		{
			return;
		}

		if (_context.EthnicitiesCharacteristics.Any(x =>
			    x.EthnicityId == ethnicity.Id && x.CharacteristicDefinitionId == definition.Id))
		{
			return;
		}

		_context.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics
		{
			Ethnicity = ethnicity,
			CharacteristicDefinition = definition,
			CharacteristicProfile = profile
		});
		_context.SaveChanges();
	}

	private void SeedRacialBodypartUsages(Race race, RobotRaceTemplate template)
	{
		if (template.BodypartUsages is null)
		{
			return;
		}

		foreach (var usage in template.BodypartUsages)
		{
			if (FindBodypartOnBody(race.BaseBody, usage.BodypartAlias) is not { } bodypart)
			{
				continue;
			}

			if (_context.RacesAdditionalBodyparts.Any(x => x.RaceId == race.Id && x.BodypartId == bodypart.Id &&
			                                              x.Usage == usage.Usage))
			{
				continue;
			}

			_context.RacesAdditionalBodyparts.Add(new RacesAdditionalBodyparts
			{
				Race = race,
				Bodypart = bodypart,
				Usage = usage.Usage
			});
		}

		_context.SaveChanges();
	}

	private void SeedNaturalAttacks(Race race, RobotRaceTemplate template)
	{
		foreach (var attackTemplate in template.Attacks)
		{
			var attack = _context.WeaponAttacks.First(x => x.Name == attackTemplate.AttackName);
			foreach (var alias in attackTemplate.BodypartAliases)
			{
				if (FindBodypartOnBody(race.BaseBody, alias) is not { } bodypart)
				{
					continue;
				}

				if (_context.RacesWeaponAttacks.Any(x =>
					    x.RaceId == race.Id && x.WeaponAttackId == attack.Id && x.BodypartId == bodypart.Id))
				{
					continue;
				}

				_context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
				{
					Race = race,
					Bodypart = bodypart,
					WeaponAttack = attack,
					Quality = (int)ItemQuality.Standard
				});
			}
		}

		_context.SaveChanges();
	}

	private void SeedDefaultDescriptions(Race race, RobotRaceTemplate template)
	{
		if (template.ShortDescriptionPattern is null || template.FullDescriptionPattern is null)
		{
			return;
		}

		if (_context.FutureProgs.Any(x => x.FunctionName == $"Is{race.Name.CollapseString()}"))
		{
			return;
		}

		var prog = new FutureProg
		{
			FunctionName = $"Is{race.Name.CollapseString()}",
			Category = "Character",
			Subcategory = "Descriptions",
			FunctionComment = $"True if the character is a {race.Name}",
			ReturnType = (long)ProgVariableTypes.Boolean,
			StaticType = 0,
			AcceptsAnyParameters = false,
			Public = true,
			FunctionText = $"return @ch.Race == ToRace(\"{race.Name}\")"
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Toon
		});
		_context.FutureProgs.Add(prog);
		_context.SaveChanges();

		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 0,
			ApplicabilityProg = prog,
			RelativeWeight = 100,
			Pattern = template.ShortDescriptionPattern
		});
		_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			Type = 1,
			ApplicabilityProg = prog,
			RelativeWeight = 100,
			Pattern = template.FullDescriptionPattern
		});
		_context.SaveChanges();
	}
}
