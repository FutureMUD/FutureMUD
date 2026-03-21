#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
	private void EnsureOrganicHumanoidDescriptionProgScope()
	{
		var dirty = false;
		foreach (var prog in _context.FutureProgs
			         .Where(x => x.Subcategory == "Descriptions" && x.FunctionName.StartsWith("IsHumanoid"))
			         .ToList())
		{
			var updatedText = HumanSeeder.UpdateHumanoidDescriptionProgScope(prog.FunctionText);
			if (updatedText == prog.FunctionText)
			{
				continue;
			}

			prog.FunctionText = updatedText;
			dirty = true;
		}

		if (dirty)
		{
			_context.SaveChanges();
		}
	}

	private void SeedSimpleEthnicity(Race race, RobotRaceTemplate template)
	{
		if (_context.Ethnicities.Any(x => x.ParentRaceId == race.Id && x.Name == $"{template.Name} Stock"))
		{
			return;
		}

		_context.Ethnicities.Add(new Ethnicity
		{
			Name = $"{template.Name} Stock",
			ChargenBlurb = BuildEthnicityDescriptionForTesting(template),
			AvailabilityProg = template.Playable ? _alwaysTrue : _alwaysFalse,
			ParentRace = race,
			EthnicGroup = template.Name,
			EthnicSubgroup = "Stock",
			PopulationBloodModel = _humanRace.Ethnicities.First().PopulationBloodModel,
			TolerableTemperatureFloorEffect = 0,
			TolerableTemperatureCeilingEffect = 0
		});
		_context.SaveChanges();
		ApplyRobotNameCultures(_context.Ethnicities.First(x =>
			x.ParentRaceId == race.Id &&
			x.Name == $"{template.Name} Stock"));
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
		var variants = template.UsesHumanoidCharacteristics
			? template.OverlayDescriptionVariants
			: template.DescriptionVariants;
		if (variants == null || variants.Count == 0)
		{
			return;
		}

		var prog = EnsureRaceDescriptionApplicabilityProg(race);
		EnsureEntityDescriptionPatterns(prog, variants, !template.UsesHumanoidCharacteristics);
		_context.SaveChanges();
	}

	private FutureProg EnsureRaceDescriptionApplicabilityProg(Race race)
	{
		var functionName = $"Is{race.Name.CollapseString()}";
		var prog = _context.FutureProgs.FirstOrDefault(x => x.FunctionName == functionName);
		if (prog is null)
		{
			prog = new FutureProg
			{
				FunctionName = functionName
			};
			_context.FutureProgs.Add(prog);
		}

		prog.Category = "Character";
		prog.Subcategory = "Descriptions";
		prog.FunctionComment = $"True if the character is a {race.Name}";
		prog.ReturnType = (long)ProgVariableTypes.Boolean;
		prog.StaticType = 0;
		prog.AcceptsAnyParameters = false;
		prog.Public = true;
		prog.FunctionText = $"return @ch.Race == ToRace(\"{race.Name}\")";

		if (!prog.FutureProgsParameters.Any(x => x.ParameterIndex == 0))
		{
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				ParameterIndex = 0,
				ParameterName = "ch",
				ParameterType = (long)ProgVariableTypes.Toon
			});
		}

		_context.SaveChanges();
		return prog;
	}

	private void EnsureEntityDescriptionPatterns(FutureProg prog, IEnumerable<StockDescriptionVariant> variants,
		bool replaceExisting)
	{
		if (replaceExisting)
		{
			var existingPatterns = _context.EntityDescriptionPatterns
				.Where(x => x.ApplicabilityProgId == prog.Id && (x.Type == 0 || x.Type == 1))
				.ToList();
			if (existingPatterns.Any())
			{
				_context.EntityDescriptionPatterns.RemoveRange(existingPatterns);
				_context.SaveChanges();
			}
		}

		var existingShortPatterns = _context.EntityDescriptionPatterns
			.Where(x => x.ApplicabilityProgId == prog.Id && x.Type == 0)
			.OrderBy(x => x.Id)
			.ToList();
		var existingFullPatterns = _context.EntityDescriptionPatterns
			.Where(x => x.ApplicabilityProgId == prog.Id && x.Type == 1)
			.OrderBy(x => x.Id)
			.ToList();

		foreach (var variant in variants)
		{
			if (!existingShortPatterns.Any(x => x.Pattern == variant.ShortDescription))
			{
				_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
				{
					Type = 0,
					ApplicabilityProg = prog,
					RelativeWeight = 100,
					Pattern = variant.ShortDescription
				});
			}

			if (!existingFullPatterns.Any(x => x.Pattern == variant.FullDescription))
			{
				_context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
				{
					Type = 1,
					ApplicabilityProg = prog,
					RelativeWeight = 100,
					Pattern = variant.FullDescription
				});
			}
		}
	}
}
