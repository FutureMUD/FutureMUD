#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static readonly string[] RenaissanceClothingWearProfiles =
	[
		"Leg Wraps", "Overshoes", "Head Veil", "Hood", "Detachable Sleeves", "Skirt Support", "Partlet",
		"Long Open Robe", "Breechcloth"
	];

	private static readonly string[] RenaissanceClothingMaterials =
	[
		"ramie cloth", "barkcloth", "camelid wool", "raffia cloth"
	];

	private static readonly string[] RenaissanceClothingTags =
	[
		"Culture / Renaissance / Shared / Western European Renaissance",
		"Culture / Renaissance / Shared / Iberian Atlantic",
		"Culture / Renaissance / Shared / Central European",
		"Culture / Renaissance / Shared / Northern Baltic",
		"Culture / Renaissance / Shared / Central Eastern Frontier",
		"Culture / Renaissance / Shared / Eastern Orthodox Northern",
		"Culture / Renaissance / Shared / Ottoman Islamicate",
		"Culture / Renaissance / Shared / Persianate Indo-Persian",
		"Culture / Renaissance / Shared / South Asian",
		"Culture / Renaissance / Shared / East Asian Literati",
		"Culture / Renaissance / Shared / Japanese",
		"Culture / Renaissance / Shared / Maritime East Asian",
		"Culture / Renaissance / Shared / South-east Asian Mainland",
		"Culture / Renaissance / Shared / Maritime South-east Asian",
		"Culture / Renaissance / Shared / Steppe and Caravan",
		"Culture / Renaissance / Shared / African Court Atlantic",
		"Culture / Renaissance / Shared / Sahelian Islamic",
		"Culture / Renaissance / Shared / Red Sea",
		"Culture / Renaissance / Shared / Indian Ocean",
		"Culture / Renaissance / Shared / Mesoamerican",
		"Culture / Renaissance / Shared / Andean",
		"Culture / Renaissance / Shared / Caribbean Contact",
		"Culture / Renaissance / Shared / North American Contact",
		"Culture / Renaissance / Shared / Colonial Atlantic",
		"Culture / Renaissance / Shared / Global Maritime",
		"Market / Clothing / Work Clothing",
		"Market / Clothing / Ceremonial Clothing",
		"Market / Clothing / Religious Clothing",
		"Market / Clothing / Maritime Clothing",
		"Functions / Worn Items / Robes",
		"Functions / Worn Items / Outerwear",
		"Functions / Worn Items / Facewear",
		"Functions / Worn Items / Neckwear",
		"Functions / Worn Items / Detachable Garment Parts",
		"Functions / Worn Items / Footwear",
		"Functions / Worn Items / Headwear",
		"Functions / Worn Items / Gloves",
		"Functions / Worn Items / Scarves",
		"Functions / Worn Items / Underwear",
		"Functions / Worn Items / Bodywear",
		"Institution / Court",
		"Institution / Religious",
		"Institution / Guild",
		"Institution / Maritime",
		"Institution / Performance",
		"Institution / Service Household"
	];

	private static readonly string[] RenaissanceClothingComponents =
	[
		"Wear_Shirt", "Wear_Shorts", "Wear_Bra", "Wear_Vest", "Wear_Jacket", "Wear_Trousers", "Wear_Chausses",
		"Wear_Skirt", "Wear_Long_Skirt", "Wear_Gown", "Wear_Dress", "Wear_Robe", "Wear_Stockings",
		"Wear_Cloak_(Open)", "Wear_Cloak_(Closed)", "Wear_Mantle", "Wear_Shoes", "Wear_Boots", "Wear_Sandals",
		"Wear_Hat", "Wear_Turban", "Wear_Veil", "Wear_Mask", "Wear_Gloves", "Wear_Scarf", "Wear_Leg_Wraps",
		"Wear_Overshoes", "Wear_Head_Veil", "Wear_Hood", "Wear_Detachable_Sleeves", "Wear_Skirt_Support",
		"Wear_Partlet", "Wear_Long_Open_Robe", "Wear_Breechcloth"
	];

	private void SeedRenaissanceClothingAndAccessories()
	{
		if (_context is null)
		{
			throw new InvalidOperationException("The item seeder context must be initialised before Renaissance clothing is seeded.");
		}

		var issues = ValidateRenaissanceClothingDependencies(
			_context.WearProfiles.Select(x => x.Name),
			_materials.Keys,
			_tagsByFullPath.Keys,
			_components.Keys);
		if (issues.Count > 0)
		{
			throw new InvalidOperationException(
				"Renaissance clothing cannot be seeded because required dependencies are missing:" +
				Environment.NewLine + string.Join(Environment.NewLine, issues.Select(x => $" - {x}")));
		}
	}

	private static IReadOnlyList<string> ValidateRenaissanceClothingDependencies(
		IEnumerable<string> wearProfiles,
		IEnumerable<string> materials,
		IEnumerable<string> tags,
		IEnumerable<string> components)
	{
		var issues = new List<string>();
		AddMissing("wear profile", RenaissanceClothingWearProfiles, wearProfiles, issues);
		AddMissing("material", RenaissanceClothingMaterials, materials, issues);
		AddMissing("tag", RenaissanceClothingTags, tags, issues);
		AddMissing("seeded component", RenaissanceClothingComponents, components, issues);
		return issues;
	}

	private static void AddMissing(string dependencyType, IEnumerable<string> required,
		IEnumerable<string> available, ICollection<string> issues)
	{
		var availableSet = available.ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (var name in required.Where(x => !availableSet.Contains(x)))
		{
			issues.Add($"Missing {dependencyType}: {name}");
		}
	}

	internal static IReadOnlyList<string> RenaissanceClothingWearProfilesForTesting => RenaissanceClothingWearProfiles;
	internal static IReadOnlyList<string> RenaissanceClothingMaterialsForTesting => RenaissanceClothingMaterials;
	internal static IReadOnlyList<string> RenaissanceClothingTagsForTesting => RenaissanceClothingTags;
	internal static IReadOnlyList<string> RenaissanceClothingComponentsForTesting => RenaissanceClothingComponents;

	internal static IReadOnlyList<string> ValidateRenaissanceClothingDependenciesForTesting(
		IEnumerable<string> wearProfiles,
		IEnumerable<string> materials,
		IEnumerable<string> tags,
		IEnumerable<string> components)
	{
		return ValidateRenaissanceClothingDependencies(wearProfiles, materials, tags, components);
	}
}
