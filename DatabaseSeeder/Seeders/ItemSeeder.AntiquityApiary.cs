using MudSharp.GameItems;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string AntiquityBeekeepingToolsTagPath = "Functions / Tools / Agricultural Tools / Beekeeping Tools";

	private void SeedAntiquityApiaryItems()
	{
		EnsureAntiquityTagPath(AntiquityBeekeepingToolsTagPath);
		foreach (var tag in new[]
		         {
			         "Bee Hive",
			         "Hive Stand",
			         "Bee Smoke Pot",
			         "Honey Knife",
			         "Honey Press",
			         "Honey Strainer"
		         })
		{
			EnsureAntiquityTagPath($"{AntiquityBeekeepingToolsTagPath} / {tag}");
		}

		CreateAntiquityApiaryEquipment("antiquity_wicker_beehive", "hive", "a woven wicker beehive",
			"A domed wicker hive daubed and sealed for keeping a small bee colony.", SizeCategory.Normal, 2800.0,
			12.0m, "willow", "Functions / Tools / Agricultural Tools / Beekeeping Tools / Bee Hive");
		CreateAntiquityApiaryEquipment("antiquity_clay_tube_hive", "hive", "a clay tube beehive",
			"A fired clay tube hive with capped ends and a small working mouth for an apiary colony.", SizeCategory.Large,
			12000.0, 16.0m, "fired clay", "Functions / Tools / Agricultural Tools / Beekeeping Tools / Bee Hive");
		CreateAntiquityApiaryEquipment("antiquity_wooden_hive_stand", "stand", "a wooden hive stand",
			"A sturdy wooden stand that keeps hives raised, level, and away from damp ground.", SizeCategory.Large,
			9000.0, 18.0m, "oak", "Functions / Tools / Agricultural Tools / Beekeeping Tools / Hive Stand");
		CreateAntiquityApiaryEquipment("antiquity_honey_press", "press", "a wooden honey press",
			"A compact screw-and-basket press for squeezing honey from broken comb.", SizeCategory.VeryLarge, 22000.0,
			30.0m, "oak", "Functions / Tools / Agricultural Tools / Beekeeping Tools / Honey Press");

		CreateAntiquityApiaryTool("antiquity_bee_smoke_pot", "pot", "a clay bee smoke pot",
			"A lidded clay pot with a small spout for directing cool smoke around opened hives.", SizeCategory.Small,
			1200.0, 7.0m, "fired clay", "Functions / Tools / Agricultural Tools / Beekeeping Tools / Bee Smoke Pot");
		CreateAntiquityApiaryTool("antiquity_honey_knife", "knife", "a bronze honey knife",
			"A broad bronze knife used to loosen and cut honeycomb from hive bodies.", SizeCategory.Small, 220.0, 8.0m,
			"bronze", "Functions / Tools / Agricultural Tools / Beekeeping Tools / Honey Knife");
		CreateAntiquityApiaryTool("antiquity_honey_strainer", "strainer", "a linen honey strainer",
			"A tight linen strainer stretched on a small hoop for cleaning pressed honey.", SizeCategory.Small, 180.0,
			5.0m, "linen", "Functions / Tools / Agricultural Tools / Beekeeping Tools / Honey Strainer");
	}

	private void CreateAntiquityApiaryTool(string stableReference, string noun, string shortDescription,
		string fullDescription, SizeCategory size, double weightInGrams, decimal cost, string material, string toolTagPath)
	{
		CreateItem(stableReference, noun, shortDescription, null, fullDescription, size, ItemQuality.Standard,
			weightInGrams, cost, false, false, material,
			[
				toolTagPath,
				"Market / Professional Tools / Standard Tools"
			],
			["Holdable", "Destroyable_Misc"], null, null, null, null);
	}

	private void CreateAntiquityApiaryEquipment(string stableReference, string noun, string shortDescription,
		string fullDescription, SizeCategory size, double weightInGrams, decimal cost, string material, string toolTagPath)
	{
		CreateItem(stableReference, noun, shortDescription, null, fullDescription, size, ItemQuality.Standard,
			weightInGrams, cost, false, false, material,
			[
				toolTagPath,
				"Market / Professional Tools / Standard Tools"
			],
			["Holdable", (int)size >= (int)SizeCategory.Large ? "Destroyable_Furniture" : "Destroyable_Misc"],
			null, null, null, null);
	}
}
