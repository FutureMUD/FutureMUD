#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MaterialBehaviourType = MudSharp.Form.Material.MaterialBehaviourType;
using TraitOwnerScope = MudSharp.Body.Traits.TraitOwnerScope;
using TraitType = MudSharp.Body.Traits.TraitType;
using SizeCategory = MudSharp.GameItems.SizeCategory;

using static MudSharp_Unit_Tests.UsefulItemTestFixture;

namespace MudSharp_Unit_Tests;

[TestClass]
public class UsefulSeederItemPackageTests
{

	[TestMethod]
	public void ItemSeeder_AntiquityComponentGapItems_RerunDoesNotDuplicateAndUsesReportComponents()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedGeneralPrerequisites(context);
		context.Tags.AddRange(
			new Tag { Id = 1, Name = "Functions" },
			new Tag { Id = 2, Name = "Tools", ParentId = 1 },
			new Tag { Id = 3, Name = "Scientific Tools", ParentId = 2 },
			new Tag { Id = 4, Name = "Measurement Tools", ParentId = 3 },
			new Tag { Id = 5, Name = "Military Equipment", ParentId = 1 },
			new Tag { Id = 6, Name = "Military Ammunition", ParentId = 5 },
			new Tag { Id = 7, Name = "Paper Cartridges", ParentId = 6 },
			new Tag { Id = 8, Name = "Wooden Powder Charges", ParentId = 6 });
		context.SaveChanges();
		SeedMarketCategories(context);
		UsefulSeeder usefulSeeder = new();
		new CoreDataSeeder().SeedTagsForTesting(context);
		usefulSeeder.SeedAntiquityComponentGapCoverageForTesting(context);
		usefulSeeder.SeedEraDependencyComponentsForTesting(context);
		usefulSeeder.SeedGeneralCoverageForTesting(context);
		var earthenware = context.Materials.SingleOrDefault(x => x.Name == "earthenware");
		if (earthenware is null)
		{
			earthenware = new Material
			{
				Id = context.Materials.Any() ? context.Materials.Max(x => x.Id) + 1 : 1,
				Name = "earthenware",
				MaterialDescription = "earthenware",
				BehaviourType = (int)MaterialBehaviourType.Ceramic,
				ResidueColour = "white"
			};
			context.Materials.Add(earthenware);
		}
		else
		{
			earthenware.ResidueColour = "white";
		}
		context.SaveChanges();
		EnsureComponentMarkers(context,
			"Destroyable_Misc",
			"Destroyable_Furniture",
			"Destroyable_WoodenHeavy",
			"Destroyable_HeavyMetal",
			"Destroyable_Clothing",
			"Destroyable_Armour",
			"Destroyable_Shield",
			"Container_Tray",
			"Container_Pouch",
			"Container_Small_Drum",
			"LContainer_DrinkingGlass",
			"LContainer_WineGlass",
			"LContainer_Amphora_Urna",
			"Wear_Ring",
			"Wear_Waist",
			"Wear_Saddle",
			"Wear_Bridle",
			"Armour_LeatherScale",
			"Container_PreIndustrial_LiddedHamper",
			"Keyring_Large",
			"LockingContainer_Lockbox",
			"Lantern",
			"Dice_d6");
		ItemSeeder itemSeeder = new();

		itemSeeder.SeedAntiquityComponentGapItemsForTesting(context);
		var ids = context.GameItemProtos.Select(x => x.Id).OrderBy(x => x).ToArray();
		Assert.IsTrue(ids.Length > 0);
		itemSeeder.SeedAntiquityComponentGapItemsForTesting(context);
		CollectionAssert.AreEqual(ids, context.GameItemProtos.Select(x => x.Id).OrderBy(x => x).ToArray());

		var oilCup = LoadItem(context, "antiquity_oil_measure_cup");
		var measurementTag = context.Tags.Single(x => x.Name == "Measurement Tools" && x.ParentId == 2);
		CollectionAssert.Contains(context.GameItemProtosTags.Where(x => x.GameItemProtoId == oilCup.Id)
			.Select(x => x.TagId).ToArray(), measurementTag.Id);
	}
}
