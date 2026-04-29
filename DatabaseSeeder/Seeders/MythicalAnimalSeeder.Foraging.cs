#nullable enable

using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class MythicalAnimalSeeder
{
	private static readonly IReadOnlyDictionary<string, StockNonHumanDietProfile[]> MythicalDietProfiles =
		new ReadOnlyDictionary<string, StockNonHumanDietProfile[]>(
			new Dictionary<string, StockNonHumanDietProfile[]>(StringComparer.OrdinalIgnoreCase)
			{
				["Dragon"] = [StockNonHumanDietProfile.Carnivore],
				["Griffin"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Piscivore],
				["Hippogriff"] = [StockNonHumanDietProfile.GeneralHerbivore, StockNonHumanDietProfile.Carnivore],
				["Unicorn"] = [StockNonHumanDietProfile.Grazer, StockNonHumanDietProfile.Browser],
				["Pegasus"] = [StockNonHumanDietProfile.Grazer, StockNonHumanDietProfile.Browser],
				["Warg"] = [StockNonHumanDietProfile.Carnivore],
				["Dire-Wolf"] = [StockNonHumanDietProfile.Carnivore],
				["Dire-Bear"] = [StockNonHumanDietProfile.ScavengerOmnivore],
				["Minotaur"] = [StockNonHumanDietProfile.GeneralHerbivore, StockNonHumanDietProfile.Omnivore],
				["Eastern Dragon"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Piscivore],
				["Naga"] = [StockNonHumanDietProfile.Carnivore],
				["Mermaid"] = [StockNonHumanDietProfile.Piscivore, StockNonHumanDietProfile.AquaticOmnivore],
				["Manticore"] = [StockNonHumanDietProfile.Carnivore],
				["Wyvern"] = [StockNonHumanDietProfile.Carnivore],
				["Phoenix"] = [StockNonHumanDietProfile.Omnivore, StockNonHumanDietProfile.Nectarivore],
				["Basilisk"] = [StockNonHumanDietProfile.Carnivore],
				["Cockatrice"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Omnivore],
				["Giant Beetle"] = [StockNonHumanDietProfile.Detritivore],
				["Giant Ant"] = [StockNonHumanDietProfile.ScavengerOmnivore, StockNonHumanDietProfile.Detritivore],
				["Giant Mantis"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
				["Giant Spider"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
				["Giant Scorpion"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
				["Giant Centipede"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
				["Giant Worm"] = [StockNonHumanDietProfile.Detritivore, StockNonHumanDietProfile.Fungivore],
				["Colossal Worm"] = [StockNonHumanDietProfile.Detritivore, StockNonHumanDietProfile.Fungivore],
				["Ankheg"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
				["Hippocamp"] = [StockNonHumanDietProfile.AquaticOmnivore, StockNonHumanDietProfile.Grazer],
				["Selkie"] = [StockNonHumanDietProfile.Piscivore],
				["Myconid"] = [StockNonHumanDietProfile.Fungivore, StockNonHumanDietProfile.Detritivore],
				["Plantfolk"] = [StockNonHumanDietProfile.PlantMatter],
				["Ent"] = [StockNonHumanDietProfile.PlantMatter],
				["Dryad"] = [StockNonHumanDietProfile.PlantMatter, StockNonHumanDietProfile.Nectarivore],
				["Owlkin"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
				["Avian Person"] = [StockNonHumanDietProfile.Omnivore],
				["Centaur"] = [StockNonHumanDietProfile.GeneralHerbivore],
				["Pegacorn"] = [StockNonHumanDietProfile.Grazer, StockNonHumanDietProfile.Browser],
				["Qilin"] = [StockNonHumanDietProfile.Grazer, StockNonHumanDietProfile.Browser],
				["Garuda"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Piscivore],
				["Bunyip"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Piscivore],
				["Yacumama"] = [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Piscivore]
			});

	internal static IReadOnlyList<StockNonHumanDietProfile> GetDietProfilesForTesting(string raceName)
	{
		return GetDietProfiles(Templates[raceName]);
	}

	internal static IReadOnlyCollection<string> GetEdibleYieldTypesForTesting(string raceName)
	{
		return NonHumanForageDietSeederHelper.GetYieldTypes(GetDietProfilesForTesting(raceName));
	}

	internal static bool CanEatCorpsesForTesting(string raceName)
	{
		return NonHumanForageDietSeederHelper.ProfilesEatCorpses(GetDietProfilesForTesting(raceName));
	}

	private static StockNonHumanDietProfile[] GetDietProfiles(MythicalRaceTemplate template)
	{
		if (MythicalDietProfiles.TryGetValue(template.Name, out var profiles))
		{
			return profiles;
		}

		return template.BodyKey switch
		{
			"Ungulate" or "Centaur" => [StockNonHumanDietProfile.GeneralHerbivore],
			"Piscine" or "Mermaid" or "Hippocamp" => [StockNonHumanDietProfile.Piscivore, StockNonHumanDietProfile.AquaticOmnivore],
			"Insectoid" or "Arachnid" or "Beetle" or "Centipede" or "Scorpion" => [StockNonHumanDietProfile.Carnivore, StockNonHumanDietProfile.Insectivore],
			"Vermiform" => [StockNonHumanDietProfile.Detritivore],
			"Avian" or "Winged Humanoid" => [StockNonHumanDietProfile.Omnivore],
			_ => template.HumanoidVariety ? [StockNonHumanDietProfile.Omnivore] : [StockNonHumanDietProfile.Carnivore]
		};
	}

	private void ApplyMythicalDietSettings(Race race, MythicalRaceTemplate template)
	{
		NonHumanForageDietSeederHelper.ApplyDiet(_context, race, template.Size, GetDietProfiles(template));
	}

	private static bool HasMissingMythicalDietSettings(FuturemudDatabaseContext context)
	{
		var stockMaterialNames = NonHumanForageDietSeederHelper.StockCorpseMaterialNamesForTesting
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		foreach (var template in Templates.Values)
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
