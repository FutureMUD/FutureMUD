#nullable enable

using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	private static readonly IReadOnlyDictionary<string, StockNonHumanDietProfile[]> AnimalDietOverrides =
		new ReadOnlyDictionary<string, StockNonHumanDietProfile[]>(
			new Dictionary<string, StockNonHumanDietProfile[]>(StringComparer.OrdinalIgnoreCase)
			{
				["Beaver"] = [StockNonHumanDietProfile.Browser, StockNonHumanDietProfile.AquaticOmnivore],
				["Otter"] = [StockNonHumanDietProfile.Piscivore, StockNonHumanDietProfile.Carnivore],
				["Fox"] = [StockNonHumanDietProfile.ScavengerOmnivore, StockNonHumanDietProfile.Carnivore],
				["Coyote"] = [StockNonHumanDietProfile.ScavengerOmnivore, StockNonHumanDietProfile.Carnivore],
				["Hyena"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.ScavengerOmnivore],
				["Bear"] = [StockNonHumanDietProfile.ScavengerOmnivore],
				["Pig"] = [StockNonHumanDietProfile.ScavengerOmnivore],
				["Boar"] = [StockNonHumanDietProfile.ScavengerOmnivore],
				["Warthog"] = [StockNonHumanDietProfile.ScavengerOmnivore],
				["Mouse"] = [StockNonHumanDietProfile.Omnivore],
				["Rat"] = [StockNonHumanDietProfile.ScavengerOmnivore],
				["Hamster"] = [StockNonHumanDietProfile.Omnivore],
				["Guinea Pig"] = [StockNonHumanDietProfile.Grazer, StockNonHumanDietProfile.GeneralHerbivore],
				["Ferret"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
				["Stoat"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
				["Weasel"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
				["Polecat"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
				["Mink"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Piscivore],
				["Shrew"] = [StockNonHumanDietProfile.Insectivore],
				["Badger"] = [StockNonHumanDietProfile.ScavengerOmnivore],
				["Wolverine"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.ScavengerOmnivore],
				["Hippopotamus"] = [StockNonHumanDietProfile.Grazer, StockNonHumanDietProfile.AquaticOmnivore],
				["Giraffe"] = [StockNonHumanDietProfile.Browser],
				["Carp"] = [StockNonHumanDietProfile.AquaticOmnivore, StockNonHumanDietProfile.GeneralHerbivore],
				["Koi"] = [StockNonHumanDietProfile.AquaticOmnivore, StockNonHumanDietProfile.GeneralHerbivore],
				["Herring"] = [StockNonHumanDietProfile.FilterFeeder],
				["Mackerel"] = [StockNonHumanDietProfile.FilterFeeder, StockNonHumanDietProfile.Piscivore],
				["Anchovy"] = [StockNonHumanDietProfile.FilterFeeder],
				["Sardine"] = [StockNonHumanDietProfile.FilterFeeder],
				["Pilchard"] = [StockNonHumanDietProfile.FilterFeeder],
				["Small Crab"] = [StockNonHumanDietProfile.AquaticOmnivore, StockNonHumanDietProfile.ScavengerOmnivore],
				["Crab"] = [StockNonHumanDietProfile.AquaticOmnivore, StockNonHumanDietProfile.ScavengerOmnivore],
				["Giant Crab"] = [StockNonHumanDietProfile.AquaticOmnivore, StockNonHumanDietProfile.ScavengerOmnivore],
				["Lobster"] = [StockNonHumanDietProfile.AquaticOmnivore, StockNonHumanDietProfile.ScavengerOmnivore],
				["Shrimp"] = [StockNonHumanDietProfile.AquaticOmnivore, StockNonHumanDietProfile.Detritivore],
				["Prawn"] = [StockNonHumanDietProfile.AquaticOmnivore, StockNonHumanDietProfile.Detritivore],
				["Crayfish"] = [StockNonHumanDietProfile.AquaticOmnivore, StockNonHumanDietProfile.Detritivore],
				["Jellyfish"] = [StockNonHumanDietProfile.FilterFeeder],
				["Baleen Whale"] = [StockNonHumanDietProfile.FilterFeeder],
				["Pigeon"] = [StockNonHumanDietProfile.Omnivore],
				["Parrot"] = [StockNonHumanDietProfile.Nectarivore, StockNonHumanDietProfile.Omnivore],
				["Swallow"] = [StockNonHumanDietProfile.Insectivore],
				["Sparrow"] = [StockNonHumanDietProfile.Omnivore],
				["Finch"] = [StockNonHumanDietProfile.Omnivore],
				["Robin"] = [StockNonHumanDietProfile.Insectivore, StockNonHumanDietProfile.Omnivore],
				["Wren"] = [StockNonHumanDietProfile.Insectivore],
				["Duck"] = [StockNonHumanDietProfile.AquaticOmnivore, StockNonHumanDietProfile.Grazer],
				["Goose"] = [StockNonHumanDietProfile.Grazer, StockNonHumanDietProfile.AquaticOmnivore],
				["Swan"] = [StockNonHumanDietProfile.Grazer, StockNonHumanDietProfile.AquaticOmnivore],
				["Seagull"] = [StockNonHumanDietProfile.ScavengerOmnivore, StockNonHumanDietProfile.Piscivore],
				["Albatross"] = [StockNonHumanDietProfile.Piscivore],
				["Heron"] = [StockNonHumanDietProfile.Piscivore, StockNonHumanDietProfile.Insectivore],
				["Crane"] = [StockNonHumanDietProfile.Omnivore, StockNonHumanDietProfile.Piscivore],
				["Flamingo"] = [StockNonHumanDietProfile.FilterFeeder, StockNonHumanDietProfile.AquaticOmnivore],
				["Ibis"] = [StockNonHumanDietProfile.Piscivore, StockNonHumanDietProfile.Insectivore],
				["Pelican"] = [StockNonHumanDietProfile.Piscivore],
				["Crow"] = [StockNonHumanDietProfile.ScavengerOmnivore],
				["Raven"] = [StockNonHumanDietProfile.ScavengerOmnivore],
				["Emu"] = [StockNonHumanDietProfile.GeneralHerbivore, StockNonHumanDietProfile.Omnivore],
				["Ostrich"] = [StockNonHumanDietProfile.GeneralHerbivore, StockNonHumanDietProfile.Omnivore],
				["Moa"] = [StockNonHumanDietProfile.GeneralHerbivore, StockNonHumanDietProfile.Omnivore],
				["Vulture"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.ScavengerOmnivore],
				["Hawk"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Piscivore],
				["Eagle"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Piscivore],
				["Falcon"] = [StockNonHumanDietProfile.Carnivore],
				["Owl"] = [StockNonHumanDietProfile.Carnivore],
				["Woodpecker"] = [StockNonHumanDietProfile.Insectivore],
				["Kingfisher"] = [StockNonHumanDietProfile.Piscivore],
				["Stork"] = [StockNonHumanDietProfile.Piscivore, StockNonHumanDietProfile.Insectivore],
				["Penguin"] = [StockNonHumanDietProfile.Piscivore],
				["Iguana"] = [StockNonHumanDietProfile.GeneralHerbivore, StockNonHumanDietProfile.Browser],
				["Monitor Lizard"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
				["Turtle"] = [StockNonHumanDietProfile.AquaticOmnivore],
				["Tortoise"] = [StockNonHumanDietProfile.Grazer, StockNonHumanDietProfile.Browser],
				["Frog"] = [StockNonHumanDietProfile.Insectivore],
				["Toad"] = [StockNonHumanDietProfile.Insectivore],
				["Ant"] = [StockNonHumanDietProfile.ScavengerOmnivore, StockNonHumanDietProfile.Detritivore],
				["Beetle"] = [StockNonHumanDietProfile.Detritivore],
				["Centipede"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
				["Mantis"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
				["Dragonfly"] = [StockNonHumanDietProfile.Insectivore],
				["Bee"] = [StockNonHumanDietProfile.Nectarivore],
				["Butterfly"] = [StockNonHumanDietProfile.Nectarivore],
				["Wasp"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Nectarivore],
				["Hornet"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Nectarivore],
				["Moth"] = [StockNonHumanDietProfile.Nectarivore],
				["Grasshopper"] = [StockNonHumanDietProfile.Grazer],
				["Cockroach"] = [StockNonHumanDietProfile.Detritivore, StockNonHumanDietProfile.ScavengerOmnivore],
				["Spider"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
				["Tarantula"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
				["Scorpion"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore]
			});

	internal static IReadOnlyList<StockNonHumanDietProfile> GetDietProfilesForTesting(string raceName)
	{
		return GetDietProfiles(RaceTemplates[raceName]);
	}

	internal static IReadOnlyCollection<string> GetEdibleYieldTypesForTesting(string raceName)
	{
		return NonHumanForageDietSeederHelper.GetYieldTypes(GetDietProfilesForTesting(raceName));
	}

	internal static bool CanEatCorpsesForTesting(string raceName)
	{
		return NonHumanForageDietSeederHelper.ProfilesEatCorpses(GetDietProfilesForTesting(raceName));
	}

	private static StockNonHumanDietProfile[] GetDietProfiles(AnimalRaceTemplate template)
	{
		if (AnimalDietOverrides.TryGetValue(template.Name, out var profiles))
		{
			return profiles;
		}

		return template.AttackLoadoutKey switch
		{
			"small-herbivore" or
				"goat" or
				"herbivore-charge" or
				"tusked-herbivore" or
				"camelid-spitter" or
				"antlered-herbivore" or
				"bovid" or
				"rhino" or
				"elephant" => [StockNonHumanDietProfile.GeneralHerbivore],
			"doglike" or "wolfpack" or "big-cat" or "bear" or "shark" or "orca" or "toothed-whale" or
				"pinniped" or "serpent-constrictor" or "serpent-neurotoxic" or "serpent-hemotoxic" or
				"serpent-cytotoxic" or "crocodilian" => [StockNonHumanDietProfile.Carnivore],
			"small-predator" or "cat" => [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
			"hippo" => [StockNonHumanDietProfile.Grazer, StockNonHumanDietProfile.AquaticOmnivore],
			"fish" or "cephalopod" or "dolphin" => [StockNonHumanDietProfile.Piscivore],
			"crab-small" or "crab-large" or "crab-giant" => [StockNonHumanDietProfile.AquaticOmnivore, StockNonHumanDietProfile.ScavengerOmnivore],
			"baleen-whale" or "jellyfish" => [StockNonHumanDietProfile.FilterFeeder],
			"bird-fowl" or "bird-flightless" => [StockNonHumanDietProfile.GeneralHerbivore, StockNonHumanDietProfile.Omnivore],
			"bird-raptor" => [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Piscivore],
			"bird-small" => [StockNonHumanDietProfile.Omnivore],
			"reptile" or "anuran" => [StockNonHumanDietProfile.Insectivore],
			"chelonian" => [StockNonHumanDietProfile.GeneralHerbivore, StockNonHumanDietProfile.AquaticOmnivore],
			"insect-mandible" or "bombardier-beetle" => [StockNonHumanDietProfile.Detritivore],
			"insect-stinger" => [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Nectarivore],
			"spider" or "spider-venomous" or "tarantula" or "scorpion" => [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
			_ => [StockNonHumanDietProfile.Omnivore]
		};
	}

	private void ApplyAnimalDietSettings(Race race, AnimalRaceTemplate template)
	{
		NonHumanForageDietSeederHelper.ApplyDiet(_context, race, template.Size, GetDietProfiles(template));
	}

	private void RefreshExistingAnimalDietSettings()
	{
		foreach (var template in RaceTemplates.Values)
		{
			var race = _context.Races.FirstOrDefault(x => x.Name == template.Name);
			if (race is null)
			{
				continue;
			}

			ApplyAnimalDietSettings(race, template);
		}

		_context.SaveChanges();
	}

	private static bool HasMissingAnimalDietSettings(FuturemudDatabaseContext context)
	{
		var stockMaterialNames = NonHumanForageDietSeederHelper.StockCorpseMaterialNamesForTesting
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		foreach (var template in RaceTemplates.Values)
		{
			var race = context.Races.FirstOrDefault(x => x.Name == template.Name);
			if (race is null)
			{
				continue;
			}

			var profiles = GetDietProfiles(template);
			var desiredYields = NonHumanForageDietSeederHelper.GetYieldTypes(profiles);
			var existingYields = context.RaceEdibleForagableYields
				.Where(x => x.RaceId == race.Id)
				.Select(x => x.YieldType)
				.AsEnumerable()
				.ToHashSet(StringComparer.OrdinalIgnoreCase);
			if (desiredYields.Any(x => !existingYields.Contains(x)))
			{
				return true;
			}

			if (!NonHumanForageDietSeederHelper.ProfilesEatCorpses(profiles))
			{
				continue;
			}

			if (!race.CanEatCorpses)
			{
				return true;
			}

			var edibleMaterialNames =
				(from row in context.RacesEdibleMaterials
				 where row.RaceId == race.Id
				 join material in context.Materials on row.MaterialId equals material.Id
				 select material.Name)
				.AsEnumerable()
				.ToHashSet(StringComparer.OrdinalIgnoreCase);
			if (stockMaterialNames.Any(x => !edibleMaterialNames.Contains(x)))
			{
				return true;
			}
		}

		return false;
	}
}
