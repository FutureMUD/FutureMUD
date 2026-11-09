#nullable enable

using System;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSpellInstallerTests
{

	[TestMethod]
	[TestCategory("Integration")]
	public void FixedWaterBreathing_RefusesEarlierModuleOwnershipWithoutAdoption()
	{
		using var db = Fixture();
		db.SeederManagedRecords.Add(new()
		{
			Seeder = ArmageddonMagicInstaller.Package,
			Module = ArmageddonWaterSeeInstaller.Module,
			StableKey = ArmageddonWaterSeeInstaller.WaterBreathingKey,
			EntityType = nameof(MudSharp.Models.MagicSpell),
			LogicalId = 987654,
			ManifestVersion = ArmageddonWaterSeeInstaller.Version,
			SeedBaseline = "{}"
		});
		db.SaveChanges();
		var result = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status);
		Assert.IsFalse(result.Identities.ContainsKey("arm.spell.water_breathing"));
		Assert.AreEqual(ArmageddonWaterSeeInstaller.Module,
			db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonWaterSeeInstaller.WaterBreathingKey).Module);
	}

	[TestMethod]
	[TestCategory("Integration")]
	[DataRow("strength")][DataRow("viewer")][DataRow("reserve")]
	public void PrimitiveDependencyLoss_HoldsNewBatchAndPreservesEarlierCommittedSlices(string missing)
	{
		using var db = Fixture();
		var settings = ArmageddonInstallation.Read(db)!;
		if (missing == "strength") db.TraitDefinitions.Remove(db.TraitDefinitions.Single(x => x.Alias == "str"));
		else if (missing == "viewer") db.FutureProgs.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]).AcceptsAnyParameters = false;
		else db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonProgressionInstaller.ManaResourceKey).Retired = true;
		db.SaveChanges();
		var result = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status, string.Join("\n", result.Messages));
		Assert.IsTrue(db.SeederManagedRecords.Any(x => x.StableKey == ArmageddonStockSpellContent.ReadAttunementKey));
		Assert.IsFalse(db.SeederManagedRecords.Any(x => x.StableKey == "arm.spell.borrowed_tongues"));
		Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
	}
}
