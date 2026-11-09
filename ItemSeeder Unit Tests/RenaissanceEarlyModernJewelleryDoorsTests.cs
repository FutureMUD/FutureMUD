#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RenaissanceEarlyModernJewelleryDoorsTests
{
	private sealed record CatalogueContract(string FileName, int ExpectedRows);

	[TestMethod]
	public void GeneratedDependencyContractIsSelfConsistentAndReportsMissingPrerequisites()
	{
		var materials = ItemSeeder.RenaissanceEarlyModernJewelleryDoorsMaterialsForTesting;
		var tags = ItemSeeder.RenaissanceEarlyModernJewelleryDoorsTagsForTesting;
		var components = ItemSeeder.RenaissanceEarlyModernJewelleryDoorsComponentsForTesting;
		Assert.AreEqual(0, ItemSeeder.ValidateRenaissanceEarlyModernJewelleryDoorsDependenciesForTesting(
			materials, tags, components).Count);

		var missingMaterialIssues = ItemSeeder.ValidateRenaissanceEarlyModernJewelleryDoorsDependenciesForTesting(
			materials.Where(x => !x.Equals("gold", StringComparison.OrdinalIgnoreCase)), tags, components);
		CollectionAssert.Contains(missingMaterialIssues.ToArray(), "Missing material: gold");
	}
}
