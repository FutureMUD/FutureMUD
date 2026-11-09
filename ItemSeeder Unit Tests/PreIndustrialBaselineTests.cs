#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PreIndustrialBaselineTests
{

	[TestMethod]
	public void SharedBaselineEraSelection_IncludesAllFourPreIndustrialErasOnlyWhenPresent()
	{
		Assert.IsTrue(ItemSeeder.ShouldSeedSharedPreIndustrialBaselineForTesting("antiquity"));
		Assert.IsTrue(ItemSeeder.ShouldSeedSharedPreIndustrialBaselineForTesting("medieval"));
		Assert.IsTrue(ItemSeeder.ShouldSeedSharedPreIndustrialBaselineForTesting("renaissance"));
		Assert.IsTrue(ItemSeeder.ShouldSeedSharedPreIndustrialBaselineForTesting("earlymodern"));
		Assert.IsFalse(ItemSeeder.ShouldSeedSharedPreIndustrialBaselineForTesting(null));
		Assert.IsFalse(ItemSeeder.ShouldSeedSharedPreIndustrialBaselineForTesting(string.Empty));
		Assert.IsFalse(ItemSeeder.ShouldSeedSharedPreIndustrialBaselineForTesting("modern"));
	}

	[TestMethod]
	public void ItemSeederScope_OnlySelectsTheExplicitBlackPowderRepair()
	{
		Assert.IsTrue(ItemSeeder.IsBlackPowderOnlyScopeForTesting(
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["scope"] = "blackpowder" }));
		Assert.IsTrue(ItemSeeder.IsBlackPowderOnlyScopeForTesting(
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["scope"] = "BLACKPOWDER" }));
		Assert.IsFalse(ItemSeeder.IsBlackPowderOnlyScopeForTesting(
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["scope"] = "all" }));
		Assert.IsFalse(ItemSeeder.IsBlackPowderOnlyScopeForTesting(null));
	}
}
