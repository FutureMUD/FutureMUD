#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	private static readonly IReadOnlyDictionary<string, NonHumanAttributeProfile> AnimalLoadoutProfiles =
		new Dictionary<string, NonHumanAttributeProfile>(StringComparer.OrdinalIgnoreCase)
		{
			["nuisance-bite"] = new(-5, -4, 2, 1),
			["small-herbivore"] = new(-4, -3, 2, 1),
			["small-predator"] = new(-1, -1, 2, 1),
			["cat"] = new(-1, -1, 3, 2),
			["doglike"] = new(1, 1, 1, 0),
			["wolfpack"] = new(3, 2, 1, 1),
			["big-cat"] = new(4, 3, 2, 1),
			["bear"] = new(5, 5, -1, -1),
			["goat"] = new(1, 2, 0, -1),
			["herbivore-charge"] = new(3, 4, -1, -1),
			["tusked-herbivore"] = new(4, 4, -1, -1),
			["camelid-spitter"] = new(1, 2, 0, 0),
			["antlered-herbivore"] = new(2, 3, 0, -1),
			["bovid"] = new(2, 3, -1, -1),
			["hippo"] = new(5, 5, -2, -2),
			["rhino"] = new(6, 5, -2, -2),
			["elephant"] = new(7, 7, -2, -2),
			["fish"] = new(-1, 0, 2, 0),
			["shark"] = new(5, 3, 2, 0),
			["crab-small"] = new(-2, 0, 1, 0),
			["crab-large"] = new(0, 1, 0, 0),
			["crab-giant"] = new(3, 3, -1, -1),
			["cephalopod"] = new(1, 0, 2, 1),
			["pinniped"] = new(1, 2, 0, 0),
			["dolphin"] = new(2, 1, 2, 0),
			["orca"] = new(7, 6, 1, -1),
			["toothed-whale"] = new(6, 6, 0, -1),
			["baleen-whale"] = new(5, 8, -2, -2),
			["bird-small"] = new(-2, -2, 3, 2),
			["bird-fowl"] = new(-1, -1, 1, 0),
			["bird-raptor"] = new(1, 0, 3, 2),
			["bird-flightless"] = new(1, 2, 1, -1),
			["serpent-constrictor"] = new(2, 4, 0, -1),
			["serpent-neurotoxic"] = new(1, 0, 2, 1),
			["serpent-hemotoxic"] = new(1, 0, 2, 1),
			["serpent-cytotoxic"] = new(1, 0, 2, 1),
			["jellyfish"] = new(-5, -5, 0, 0),
			["insect-mandible"] = new(-3, -3, 2, 1),
			["bombardier-beetle"] = new(-3, -2, 1, 1),
			["insect-stinger"] = new(-3, -3, 2, 1),
			["spider"] = new(-3, -3, 2, 2),
			["spider-venomous"] = new(-3, -3, 2, 2),
			["tarantula"] = new(-2, -2, 1, 1),
			["scorpion"] = new(-2, -1, 1, 1),
			["reptile"] = new(1, 1, 0, -1),
			["crocodilian"] = new(4, 4, 0, -1),
			["chelonian"] = new(-1, 4, -2, -2),
			["anuran"] = new(-4, -4, 2, 1)
		};

	private static readonly IReadOnlyDictionary<string, NonHumanAttributeProfile> AnimalSpecificProfiles =
		new Dictionary<string, NonHumanAttributeProfile>(StringComparer.OrdinalIgnoreCase)
		{
			["Mouse"] = new(-2, -2, 2, 1),
			["Rat"] = new(-1, -1, 2, 1),
			["Rabbit"] = new(-1, -1, 2, 1),
			["Hare"] = new(0, 0, 2, 1),
			["Cat"] = new(0, -1, 1, 1),
			["Wolf"] = new(1, 1, 0, 0),
			["Lion"] = new(2, 1, 0, 0),
			["Tiger"] = new(3, 2, 0, 0),
			["Bear"] = new(2, 2, -1, -1),
			["Moose"] = new(2, 2, -1, -1),
			["Rhinocerous"] = new(2, 2, -1, -1),
			["Hippopotamus"] = new(3, 3, -1, -1),
			["Elephant"] = new(2, 3, -1, -1),
			["Shark"] = new(2, 1, 0, 0),
			["Orca"] = new(3, 2, 0, 0),
			["Toothed Whale"] = new(2, 2, -1, -1),
			["Baleen Whale"] = new(1, 3, -1, -1),
			["Giant Squid"] = new(2, 1, 0, 0),
			["Crocodile"] = new(2, 2, 0, -1),
			["Alligator"] = new(1, 1, 0, -1)
		};

	private NonHumanAttributeProfile GetAnimalAttributeProfile(AnimalRaceTemplate template)
	{
		var profile = GetAnimalSizeProfile(template.Size)
			.Add(GetAnimalBodypartHealthProfile(template.BodypartHealthMultiplier));

		if (AnimalLoadoutProfiles.TryGetValue(template.AttackLoadoutKey, out var loadoutProfile))
		{
			profile = profile.Add(loadoutProfile);
		}

		if (AnimalSpecificProfiles.TryGetValue(template.Name, out var specificProfile))
		{
			profile = profile.Add(specificProfile);
		}

		return profile.Clamp(-10, 18);
	}

	private static NonHumanAttributeProfile GetAnimalSizeProfile(SizeCategory size)
	{
		return size switch
		{
			SizeCategory.Tiny => new(-6, -5, 3, 2),
			SizeCategory.VerySmall => new(-4, -3, 2, 1),
			SizeCategory.Small => new(-1, 0, 1, 1),
			SizeCategory.Normal => new(1, 1, 0, 0),
			SizeCategory.Large => new(4, 4, -1, -1),
			SizeCategory.VeryLarge => new(7, 7, -2, -2),
			_ => new(0, 0, 0, 0)
		};
	}

	private static NonHumanAttributeProfile GetAnimalBodypartHealthProfile(double bodypartHealthMultiplier)
	{
		var strengthBonus = (int)Math.Round(
			(bodypartHealthMultiplier - 1.0) * 2.0,
			MidpointRounding.AwayFromZero);
		var constitutionBonus = (int)Math.Round(
			(bodypartHealthMultiplier - 1.0) * 4.0,
			MidpointRounding.AwayFromZero);
		return new(strengthBonus, constitutionBonus, 0, 0);
	}

	private FutureProg CreateAnimalAttributeBonusProg(string raceName, NonHumanAttributeProfile profile)
	{
		var progText = NonHumanAttributeScalingHelper.BuildAttributeBonusProgText(
			_context.TraitDefinitions.Where(x => x.Type == (int)TraitType.Attribute),
			profile);

		var attributeBonusProg = new FutureProg
		{
			FunctionName = $"{raceName.CollapseString()}AttributeBonus",
			FunctionComment = $"Racial attribute bonuses for the {raceName} race",
			AcceptsAnyParameters = false,
			Category = "Character",
			Subcategory = "Attributes",
			ReturnType = (long)ProgVariableTypes.Number,
			FunctionText = progText
		};

		attributeBonusProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = attributeBonusProg,
			ParameterIndex = 0,
			ParameterName = "trait",
			ParameterType = (long)ProgVariableTypes.Trait
		});

		_context.FutureProgs.Add(attributeBonusProg);
		return attributeBonusProg;
	}
}
