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
		itemSeeder.SeedAntiquityComponentGapItemsForTesting(context);

		var expectedStableReferences = ItemSeeder.AntiquityComponentGapItemStableReferencesForTesting;
		foreach (var stableReference in expectedStableReferences)
		{
			Assert.AreEqual(1, context.GameItemProtos.Count(x => x.UniqueName == stableReference),
				$"Expected one item prototype named {stableReference}.");
		}

		GameItemProto bronzeSignet = LoadItem(context, "antiquity_bronze_signet_ring");
		CollectionAssert.Contains(ComponentNames(bronzeSignet), "SealStamp_Antiquity_BronzeSignet");
		CollectionAssert.Contains(ComponentNames(bronzeSignet), "Wear_Ring");

		GameItemProto sealBox = LoadItem(context, "antiquity_tax_office_seal_box");
		CollectionAssert.Contains(ComponentNames(sealBox), "LockingContainer_Lockbox");
		CollectionAssert.Contains(ComponentNames(sealBox), "Sealable_Container_Wax");

		GameItemProto oilCup = LoadItem(context, "antiquity_oil_measure_cup");
		CollectionAssert.Contains(ComponentNames(oilCup), "LContainer_DrinkingGlass");
		CollectionAssert.Contains(ComponentNames(oilCup), "MeasuringInstrument_Antiquity_OilCup");
		Assert.AreEqual(2, context.Tags.Count(x => x.Name == "Measurement Tools"),
			"Full tag paths are distinct stable identities, even when their leaf names match.");
		var measurementTag = context.Tags.Single(x => x.Name == "Measurement Tools" && x.ParentId == 2);
		CollectionAssert.Contains(
			context.GameItemProtosTags
			       .Where(x => x.GameItemProtoId == oilCup.Id)
			       .Select(x => x.TagId)
			       .ToArray(),
			measurementTag.Id);

		GameItemProto publicWell = LoadItem(context, "antiquity_stone_public_well");
		CollectionAssert.Contains(ComponentNames(publicWell), "WaterSource_Antiquity_PublicWell");

		GameItemProto measuringRod = LoadItem(context, "antiquity_wooden_measuring_rod");
		CollectionAssert.DoesNotContain(ComponentNames(measuringRod), "MeasuringInstrument_Antiquity_BalanceScale",
			"The rod is intentionally a static prop and should not receive an unrelated measuring component.");

		GameItemProto censer = LoadItem(context, "antiquity_bronze_incense_censer");
		CollectionAssert.Contains(ComponentNames(censer), "IncenseBurner_Antiquity_BronzeCenser");

		GameItemProto incense = LoadItem(context, "antiquity_resin_incense_pellets");
		CollectionAssert.Contains(ComponentNames(incense), "Holdable");
		Assert.IsTrue(incense.GameItemProtosTags
		                     .Select(x => context.Tags.Single(tag => tag.Id == x.TagId).Name)
		                     .Contains("Incense Fuel"));

		GameItemProto altar = LoadItem(context, "antiquity_household_altar");
		CollectionAssert.Contains(ComponentNames(altar), "OfferingReceiver_Antiquity_HouseholdAltar");

		GameItemProto votiveBasin = LoadItem(context, "antiquity_votive_offering_basin");
		CollectionAssert.Contains(ComponentNames(votiveBasin), "OfferingReceiver_Antiquity_VotiveBasin");

		GameItemProto funeralTray = LoadItem(context, "antiquity_funeral_offering_tray");
		CollectionAssert.Contains(ComponentNames(funeralTray), "OfferingReceiver_Antiquity_FuneralTray");

		GameItemProto libationTable = LoadItem(context, "antiquity_temple_libation_table");
		CollectionAssert.Contains(ComponentNames(libationTable), "OfferingReceiver_Antiquity_TempleLibationTable");
		GameItemProto lampShrine = LoadItem(context, "antiquity_oil_lamp_shrine");
		CollectionAssert.IsSubsetOf(
			new[] { "OfferingReceiver_Antiquity_OilLampShrine", "Lantern" },
			ComponentNames(lampShrine));
		GameItemProto oracularTripod = LoadItem(context, "antiquity_oracular_tripod");
		CollectionAssert.Contains(ComponentNames(oracularTripod), "OfferingReceiver_Antiquity_OracularTripod");
		GameItemProto bloodBowl = LoadItem(context, "antiquity_blood_offering_bowl");
		CollectionAssert.Contains(ComponentNames(bloodBowl), "OfferingReceiver_Antiquity_BloodOfferingBowl");

		var libationTag = context.Tags.Single(x => x.Name == "Libation");
		Assert.AreEqual(1, context.LiquidsTags.Count(x => x.LiquidId == 1 && x.TagId == libationTag.Id),
			"Rerunning the item seeder should preserve one stable water-to-libation association.");

		GameItemProto bridle = LoadItem(context, "antiquity_leather_bridle");
		CollectionAssert.IsSubsetOf(new[] { "Wear_Bridle", "RidingGear_Bridle" }, ComponentNames(bridle));
		GameItemProto panniers = LoadItem(context, "antiquity_mule_pannier_set");
		CollectionAssert.IsSubsetOf(
			new[] { "Wear_Saddle", "RidingGear_PackSaddle", "Container_PreIndustrial_LiddedHamper" },
			ComponentNames(panniers));
		GameItemProto yoke = LoadItem(context, "antiquity_ox_yoke");
		CollectionAssert.Contains(ComponentNames(yoke), "HitchGear_Yoke");
		GameItemProto harness = LoadItem(context, "antiquity_chariot_harness");
		CollectionAssert.Contains(ComponentNames(harness), "HitchGear_Harness");
		GameItemProto barding = LoadItem(context, "antiquity_warhorse_barding_harness");
		CollectionAssert.IsSubsetOf(
			new[] { "RidingGear_RidingHarness", "Armour_LeatherScale", "Wear_Saddle" },
			ComponentNames(barding));
		GameItemProto halter = LoadItem(context, "antiquity_rope_lead_halter");
		CollectionAssert.IsSubsetOf(
			new[] { "RidingGear_BitlessBridle", "HitchGear_LeadRope", "Wear_Bridle" },
			ComponentNames(halter));
	}
}
