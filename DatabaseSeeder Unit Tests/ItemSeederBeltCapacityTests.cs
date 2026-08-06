#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederBeltCapacityTests
{
	[TestMethod]
	public void EnsureBeltCapacityComponents_BeltLikeWearables_AddAppropriateCapacity()
	{
		AssertBeltCapacity(
			"belt",
			"a plain leather belt",
			["Functions / Worn Items / Belts"],
			["Holdable", "Wear_Waist"],
			"Belt_2");
		AssertBeltCapacity(
			"belt",
			"a heavy war belt",
			["Functions / Military Equipment", "Functions / Worn Items / Belts"],
			["Holdable", "Wear_Waist"],
			"Belt_4");
		AssertBeltCapacity(
			"sash",
			"a woven officer's sash",
			["Functions / Worn Items / Belts"],
			["Holdable", "Wear_Sash"],
			"Belt_6");
		AssertBeltCapacity(
			"crossbelt",
			"a broad leather service crossbelt",
			["Functions / Military Equipment"],
			["Holdable", "Wear_Bandolier"],
			"Belt_6");
		AssertBeltCapacity(
			"baldric",
			"a leather officer's baldric",
			["Functions / Military Equipment"],
			["Holdable", "Wear_Waist"],
			"Belt_6");
	}

	[TestMethod]
	public void EnsureBeltCapacityComponents_NonBeltItemsAndExistingCapacities_ArePreserved()
	{
		var beltAxeComponents = ItemSeeder.EnsureBeltCapacityComponentsForTesting(
			"axe",
			"a serviceable belt axe",
			["Functions / Military Equipment"],
			["Holdable", "Beltable"]);
		CollectionAssert.DoesNotContain(beltAxeComponents.ToArray(), "Belt_2");
		CollectionAssert.DoesNotContain(beltAxeComponents.ToArray(), "Belt_4");
		CollectionAssert.DoesNotContain(beltAxeComponents.ToArray(), "Belt_6");

		var plateHarnessComponents = ItemSeeder.EnsureBeltCapacityComponentsForTesting(
			"harness",
			"a fitted plate harness",
			["Functions / Military Equipment"],
			["Holdable", "Wear_FullPlateHarness"]);
		CollectionAssert.DoesNotContain(plateHarnessComponents.ToArray(), "Belt_6");

		var explicitCapacityComponents = ItemSeeder.EnsureBeltCapacityComponentsForTesting(
			"belt",
			"a sword belt",
			["Functions / Military Equipment"],
			["Holdable", "Wear_Waist", "Belt_4"]);
		Assert.AreEqual(1, explicitCapacityComponents.Count(x => x.StartsWith("Belt_", StringComparison.Ordinal)));
		CollectionAssert.Contains(explicitCapacityComponents.ToArray(), "Belt_4");

		var legacyMilitaryBeltComponents = ItemSeeder.EnsureBeltCapacityComponentsForTesting(
			"belt",
			"a simple sword belt",
			["Functions / Military Equipment"],
			["Holdable", "Wear_Waist", "Belt_2"]);
		CollectionAssert.Contains(legacyMilitaryBeltComponents.ToArray(), "Belt_4");
		CollectionAssert.DoesNotContain(legacyMilitaryBeltComponents.ToArray(), "Belt_2");
	}

	private static void AssertBeltCapacity(
		string noun,
		string shortDescription,
		IReadOnlyCollection<string> tags,
		IReadOnlyCollection<string> components,
		string expectedComponent)
	{
		var result = ItemSeeder.EnsureBeltCapacityComponentsForTesting(noun, shortDescription, tags, components);
		CollectionAssert.Contains(result.ToArray(), expectedComponent);
	}
}
