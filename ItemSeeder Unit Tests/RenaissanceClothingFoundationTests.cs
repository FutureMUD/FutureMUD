#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RenaissanceClothingFoundationTests
{

	[TestMethod]
	public void RenaissanceClothingDependencyValidation_AcceptsCompleteFoundationAndReportsExactMissingRows()
	{
		IReadOnlyList<string> complete = ItemSeeder.ValidateRenaissanceClothingDependenciesForTesting(
			ItemSeeder.RenaissanceClothingWearProfilesForTesting,
			ItemSeeder.RenaissanceClothingMaterialsForTesting,
			ItemSeeder.RenaissanceClothingTagsForTesting,
			ItemSeeder.RenaissanceClothingComponentsForTesting);
		Assert.AreEqual(0, complete.Count, string.Join(Environment.NewLine, complete));

		IReadOnlyList<string> missing = ItemSeeder.ValidateRenaissanceClothingDependenciesForTesting(
			ItemSeeder.RenaissanceClothingWearProfilesForTesting,
			ItemSeeder.RenaissanceClothingMaterialsForTesting.Where(x => x != "barkcloth"),
			ItemSeeder.RenaissanceClothingTagsForTesting,
			ItemSeeder.RenaissanceClothingComponentsForTesting);
		CollectionAssert.AreEqual(new[] { "Missing material: barkcloth" }, missing.ToArray());
	}
}
