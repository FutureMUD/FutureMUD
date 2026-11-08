#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonItemNamingTests
{
	[TestMethod]
	[TestCategory("Integration")]
	[DataRow("weapons", false)]
	[DataRow("weapons", true)]
	[DataRow("provisions", false)]
	[DataRow("provisions", true)]
	[DataRow("specials", false)]
	[DataRow("specials", true)]
	public void CatalogueLabels_RepairLegacyFieldsAndRetainBuilderOverrides(string family, bool previouslyOwnedUniqueName)
	{
		using var db = family switch
		{
			"weapons" => ArmageddonCatalogueWeaponTests.Fixture(ArmageddonProgressionMode.Simple, false),
			"provisions" => ArmageddonCatalogueProvisionsTests.Fixture(ArmageddonProgressionMode.Simple, false),
			_ => ArmageddonCatalogueInstallerTests.Fixture()
		};
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		ArmageddonInstallResult Install() => family switch
		{
			"weapons" => ArmageddonCatalogueWeapons.Install(factory),
			"provisions" => ArmageddonCatalogueProvisions.Install(factory),
			_ => ArmageddonCatalogueSpecials.Install(factory)
		};
		if (family == "specials") Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueSupplies.Install(factory).Status);
		var first = Install();
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status, string.Join("\n", first.Messages));
		db.ChangeTracker.Clear();
		var keys = first.Identities.Keys.ToArray();
		var records = db.SeederManagedRecords.Where(x => x.EntityType == nameof(GameItemProto) && keys.Contains(x.StableKey)).ToArray();
		var expected = records.Select(x => db.GameItemProtos.Single(y => y.Id == x.LogicalId && y.RevisionNumber == x.RevisionNumber))
			.Select(x => (x.Id, x.Name, x.UniqueName)).ToArray();
		Assert.AreEqual(family == "weapons" ? 49 : family == "provisions" ? 4 : 6, expected.Length);
		Assert.IsTrue(expected.All(x => !x.Name.Any(char.IsWhiteSpace) && x.UniqueName.StartsWith("Armageddon ")));
		foreach (var entry in expected) MakeLegacy(db, db.GameItemProtos.Single(x => x.Id == entry.Id), previouslyOwnedUniqueName);
		var custom = db.GameItemProtos.Single(x => x.Id == expected[0].Id);
		custom.Name = "customnoun";
		custom.UniqueName = "Builder's retained item";
		db.SaveChanges();
		db.ChangeTracker.Clear();
		for (var run = 0; run < 2; run++)
		{
			var result = Install();
			Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
			CollectionAssert.AreEquivalent(first.Identities.ToArray(), result.Identities.ToArray());
			db.ChangeTracker.Clear();
			foreach (var entry in expected)
			{
				var actual = db.GameItemProtos.Single(x => x.Id == entry.Id);
				Assert.AreEqual(entry.Id == custom.Id ? "customnoun" : entry.Name, actual.Name);
				Assert.AreEqual(entry.Id == custom.Id ? "Builder's retained item" : entry.UniqueName, actual.UniqueName);
			}
		}
	}

	internal static void MakeLegacy(FuturemudDatabaseContext db, GameItemProto item, bool previouslyOwnedUniqueName = false)
	{
		var record = db.SeederManagedRecords.Single(x => x.EntityType == nameof(GameItemProto) && x.LogicalId == item.Id);
		var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
		baseline["Name"] = JsonSerializer.Serialize(item.UniqueName);
		if (previouslyOwnedUniqueName) baseline["UniqueName"] = "null";
		else baseline.Remove("UniqueName");
		record.SeedBaseline = JsonSerializer.Serialize(baseline);
		item.Name = item.UniqueName;
		item.UniqueName = null!;
	}
}
