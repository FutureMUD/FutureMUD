#nullable enable

using MudSharp.Body.Traits;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	private const string AnimalIntelligenceDiceExpression = "2d3";
	private const string AnimalAuraDiceExpression = "1d2";

	private static readonly NonHumanAttributeProfile AnimalMentalBaseline =
		new(0, 0, 0, 0,
			WillpowerBonus: -2,
			IntelligenceDiceExpression: AnimalIntelligenceDiceExpression,
			AuraDiceExpression: AnimalAuraDiceExpression);

	private static readonly IReadOnlyDictionary<string, NonHumanAttributeProfile> AnimalLoadoutProfiles =
		new Dictionary<string, NonHumanAttributeProfile>(StringComparer.OrdinalIgnoreCase)
		{
			["nuisance-bite"] = new(-5, -4, 2, 1, WillpowerBonus: -3, PerceptionBonus: 1),
			["small-herbivore"] = new(-4, -3, 2, 1, WillpowerBonus: -3, PerceptionBonus: 2),
			["small-predator"] = new(-1, -1, 2, 1, WillpowerBonus: -1, PerceptionBonus: 2),
			["cat"] = new(-1, -1, 3, 2, PerceptionBonus: 3),
			["doglike"] = new(1, 1, 1, 0, WillpowerBonus: 1, PerceptionBonus: 2),
			["wolfpack"] = new(3, 2, 1, 1, WillpowerBonus: 2, PerceptionBonus: 2),
			["big-cat"] = new(4, 3, 2, 1, WillpowerBonus: 3, PerceptionBonus: 2),
			["bear"] = new(5, 5, -1, -1, WillpowerBonus: 4, PerceptionBonus: 1),
			["goat"] = new(1, 1, 1, 0, PerceptionBonus: 1),
			["herbivore-charge"] = new(3, 3, 1, -1, WillpowerBonus: 1, PerceptionBonus: 1),
			["tusked-herbivore"] = new(5, 4, 0, -1, WillpowerBonus: 2),
			["camelid-spitter"] = new(1, 1, 1, 0, PerceptionBonus: 1),
			["antlered-herbivore"] = new(2, 2, 1, -1, WillpowerBonus: -1, PerceptionBonus: 2),
			["bovid"] = new(3, 3, 0, -1, WillpowerBonus: -1),
			["hippo"] = new(5, 5, -2, -2, WillpowerBonus: 5),
			["rhino"] = new(6, 5, -2, -2, WillpowerBonus: 4),
			["elephant"] = new(7, 7, -2, -2, WillpowerBonus: 4, PerceptionBonus: 1, IntelligenceDiceExpression: "2d4"),
			["fish"] = new(-1, 0, 2, 0, WillpowerBonus: -1),
			["shark"] = new(5, 3, 2, 0, WillpowerBonus: 3, PerceptionBonus: 2),
			["crab-small"] = new(-2, 0, 1, 0, WillpowerBonus: -1),
			["crab-large"] = new(0, 1, 0, 0),
			["crab-giant"] = new(3, 3, -1, -1, WillpowerBonus: 1),
			["cephalopod"] = new(1, 0, 2, 1, PerceptionBonus: 2, IntelligenceDiceExpression: "2d4"),
			["pinniped"] = new(1, 2, 0, 0, PerceptionBonus: 2),
			["dolphin"] = new(2, 1, 2, 0, WillpowerBonus: 2, PerceptionBonus: 3, IntelligenceDiceExpression: "2d4"),
			["orca"] = new(7, 6, 1, -1, WillpowerBonus: 4, PerceptionBonus: 3, IntelligenceDiceExpression: "2d4"),
			["toothed-whale"] = new(6, 6, 0, -1, WillpowerBonus: 2, PerceptionBonus: 3, IntelligenceDiceExpression: "2d4"),
			["baleen-whale"] = new(5, 8, -2, -2, PerceptionBonus: 1),
			["bird-small"] = new(-2, -2, 3, 2, WillpowerBonus: -1, PerceptionBonus: 3),
			["bird-fowl"] = new(-1, -1, 1, 0, WillpowerBonus: -1, PerceptionBonus: 2),
			["bird-raptor"] = new(1, 0, 3, 2, WillpowerBonus: 2, PerceptionBonus: 4),
			["bird-flightless"] = new(2, 2, 2, -1, WillpowerBonus: 1, PerceptionBonus: 2),
			["serpent-constrictor"] = new(2, 4, 0, -1, WillpowerBonus: 2, PerceptionBonus: 1),
			["serpent-neurotoxic"] = new(1, 0, 2, 1, WillpowerBonus: 1, PerceptionBonus: 2),
			["serpent-hemotoxic"] = new(1, 0, 2, 1, WillpowerBonus: 1, PerceptionBonus: 2),
			["serpent-cytotoxic"] = new(1, 0, 2, 1, WillpowerBonus: 1, PerceptionBonus: 2),
			["jellyfish"] = new(-5, -5, 0, 0, WillpowerBonus: -5, PerceptionBonus: -2),
			["insect-mandible"] = new(-3, -3, 2, 1, WillpowerBonus: -1),
			["bombardier-beetle"] = new(-3, -2, 1, 1),
			["insect-stinger"] = new(-3, -3, 2, 1),
			["spider"] = new(-3, -3, 2, 2, PerceptionBonus: 1),
			["spider-venomous"] = new(-3, -3, 2, 2, PerceptionBonus: 1),
			["tarantula"] = new(-2, -2, 1, 1, PerceptionBonus: 1),
			["scorpion"] = new(-2, -1, 1, 1, WillpowerBonus: 1, PerceptionBonus: 1),
			["reptile"] = new(1, 1, 0, -1, PerceptionBonus: 1),
			["crocodilian"] = new(4, 4, 0, -1, WillpowerBonus: 3, PerceptionBonus: 1),
			["chelonian"] = new(-1, 4, -2, -2),
			["anuran"] = new(-4, -4, 2, 1, WillpowerBonus: -3, PerceptionBonus: 1)
		};

	private static readonly IReadOnlyDictionary<string, NonHumanAttributeProfile> AnimalSpecificProfiles =
		new Dictionary<string, NonHumanAttributeProfile>(StringComparer.OrdinalIgnoreCase)
		{
			["Mouse"] = new(-2, -2, 2, 1, WillpowerBonus: -1, PerceptionBonus: 1),
			["Rat"] = new(-1, -1, 2, 1, PerceptionBonus: 1, IntelligenceDiceExpression: "2d4"),
			["Rabbit"] = new(-1, -1, 2, 1, WillpowerBonus: -1, PerceptionBonus: 1),
			["Hare"] = new(0, 0, 2, 1, PerceptionBonus: 1),
			["Cat"] = new(0, -1, 1, 1, PerceptionBonus: 1),
			["Wolf"] = new(1, 1, 0, 0, WillpowerBonus: 1, PerceptionBonus: 1),
			["Lion"] = new(2, 1, 0, 0, WillpowerBonus: 1),
			["Tiger"] = new(3, 2, 0, 0, WillpowerBonus: 1),
			["Sabretooth Tiger"] = new(4, 3, -1, -1, WillpowerBonus: 1),
			["Cheetah"] = new(-3, -2, 2, 0, PerceptionBonus: 1),
			["Leopard"] = new(0, 0, 1, 0, WillpowerBonus: 1, PerceptionBonus: 1),
			["Panther"] = new(0, 0, 1, 0, WillpowerBonus: 1, PerceptionBonus: 1),
			["Jaguar"] = new(1, 1, 0, -1, WillpowerBonus: 1),
			["Deer"] = new(-1, -1, 1, 0, WillpowerBonus: -2, PerceptionBonus: 1),
			["Bear"] = new(2, 2, -1, -1, WillpowerBonus: 1),
			["Sheep"] = new(-2, -1, 0, 0, WillpowerBonus: -2),
			["Horse"] = new(-1, -1, 2, 1, PerceptionBonus: 1),
			["Donkey"] = new(0, 1, 0, -1, WillpowerBonus: 3, PerceptionBonus: 1),
			["Mule"] = new(0, 2, 0, -1, WillpowerBonus: 3, PerceptionBonus: 1),
			["Cow"] = new(-1, -1, 0, 0, WillpowerBonus: -1),
			["Giraffe"] = new(-1, -2, 2, 0, PerceptionBonus: 1),
			["Moose"] = new(2, 2, -1, -1, WillpowerBonus: 1),
			["Rhinocerous"] = new(2, 2, -1, -1, WillpowerBonus: 1),
			["Hippopotamus"] = new(3, 3, -1, -1, WillpowerBonus: 1),
			["Elephant"] = new(2, 3, -1, -1, WillpowerBonus: 1, PerceptionBonus: 1, IntelligenceDiceExpression: "2d4"),
			["Mammoth"] = new(3, 4, -2, -2, WillpowerBonus: 1, PerceptionBonus: 1, IntelligenceDiceExpression: "2d4"),
			["Shark"] = new(2, 1, 0, 0, WillpowerBonus: 1),
			["Orca"] = new(3, 2, 0, 0, WillpowerBonus: 1, PerceptionBonus: 1, IntelligenceDiceExpression: "2d4"),
			["Toothed Whale"] = new(2, 2, -1, -1, WillpowerBonus: 1, PerceptionBonus: 1, IntelligenceDiceExpression: "2d4"),
			["Baleen Whale"] = new(1, 3, -1, -1, PerceptionBonus: 1, IntelligenceDiceExpression: "2d4"),
			["Giant Squid"] = new(2, 1, 0, 0, WillpowerBonus: 1, PerceptionBonus: 1, IntelligenceDiceExpression: "2d4"),
			["Crocodile"] = new(2, 2, 0, -1, WillpowerBonus: 1),
			["Alligator"] = new(1, 1, 0, -1),
			["Emu"] = new(-1, -1, 2, 0, PerceptionBonus: 1),
			["Ostrich"] = new(0, 0, 2, 0, WillpowerBonus: 1, PerceptionBonus: 1),
			["Moa"] = new(2, 2, 0, -1, WillpowerBonus: 1, PerceptionBonus: 1)
		};

	internal static NonHumanAttributeProfile GetAnimalAttributeProfileForTesting(AnimalRaceTemplate template)
	{
		return GetAnimalAttributeProfile(template);
	}

	private static NonHumanAttributeProfile GetAnimalAttributeProfile(AnimalRaceTemplate template)
	{
		var profile = AnimalMentalBaseline
			.Add(GetAnimalSizeProfile(template.Size))
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

}
