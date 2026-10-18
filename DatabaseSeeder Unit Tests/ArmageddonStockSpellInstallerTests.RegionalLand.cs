#nullable enable
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;
using RegionalProfile = MudSharp.Magic.Environment.RegionalGatheringProfile;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSpellInstallerTests
{
	[TestMethod]
	public void RegionalLand_PresetsCoverTerrainFamiliesAndRerunPreservesOwnersAndBuilderEdits()
	{
		using var db = FreshFixture();
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var spells = ArmageddonStockSpellInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, spells.Status, string.Join("\n", spells.Messages));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonProgressionInstaller.InstallGatheringFoundations(factory).Status);
		var gathering = ArmageddonStockGatheringInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, gathering.Status, string.Join("\n", gathering.Messages));
		foreach (var name in ArmageddonRegionalLandInstaller.Presets.Select(x => x.BaseTerrain).Distinct())
			db.Terrains.Add(new Terrain { Name = name, TerrainEditorText = "xy", TagInformation = "", TerrainBehaviourMode = "outdoors", DefaultTerrain = false });
		db.SaveChanges();
		var originalTerrains = db.Terrains.AsNoTracking().ToDictionary(x => x.Id, x => (x.Name, x.EnvironmentalMagicProfileId, x.ForagableProfileId, x.DefaultTerrain));
		var installed = ArmageddonRegionalLandInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual(10, db.RegionalGatheringProfiles.Count());
		Assert.AreEqual(0, db.RegionalGatheringPools.Count());
		var mixedRow = db.RegionalGatheringProfiles.Single(x => x.Id == installed.Identities[ArmageddonRegionalLandInstaller.MixedProfileKey]);
		var mixed = RegionalProfile.Parse(mixedRow.Id, mixedRow.Name, mixedRow.Revision, mixedRow.Definition);
		Assert.AreEqual(9, mixed.TerrainContributions.Count);
		Assert.AreEqual(0.0, mixed.DefaultContribution);
		foreach (var preset in ArmageddonRegionalLandInstaller.Presets)
		{
			Assert.AreEqual(preset.Life, mixed.Contribution(installed.Identities[ArmageddonRegionalLandInstaller.TerrainKey(preset.Key)]));
			var profileRow = db.RegionalGatheringProfiles.Single(x => x.Id == installed.Identities[ArmageddonRegionalLandInstaller.RegionalKey(preset.Key)]);
			var profile = RegionalProfile.Parse(profileRow.Id, profileRow.Name, profileRow.Revision, profileRow.Definition);
			Assert.AreEqual(preset.Life, profile.DefaultContribution);
			Assert.AreEqual(.1, profile.RecoveryPerHour);
			var terrain = db.Terrains.Single(x => x.Id == installed.Identities[ArmageddonRegionalLandInstaller.TerrainKey(preset.Key)]);
			Assert.IsFalse(terrain.DefaultTerrain);
			var forage = db.ForagableProfiles.Include(x => x.ForagableProfilesMaximumYields).Include(x => x.ForagableProfilesHourlyYieldGains).Single(x => x.Id == terrain.ForagableProfileId);
			Assert.AreEqual(preset.Life, forage.ForagableProfilesMaximumYields.SingleOrDefault()?.Yield ?? 0);
			Assert.AreEqual(0.0, forage.ForagableProfilesHourlyYieldGains.SingleOrDefault()?.Yield ?? 0);
		}
		foreach (var old in originalTerrains)
		{
			var row = db.Terrains.Single(x => x.Id == old.Key);
			Assert.AreEqual(old.Value, (row.Name, row.EnvironmentalMagicProfileId, row.ForagableProfileId, row.DefaultTerrain));
		}
		mixedRow.Name = "Builder mixed land";
		mixedRow.Definition = new RegionalProfile(mixedRow.Id, mixedRow.Name, 2, 0, .3, mixed.TerrainContributions).Definition();
		mixedRow.Revision = 2;
		var fertile = db.Terrains.Single(x => x.Id == installed.Identities[ArmageddonRegionalLandInstaller.TerrainKey("fertile")]);
		fertile.Name = "Builder grassland";
		db.SaveChanges();
		var claims = db.SeederManagedRecords.Where(x => x.StableKey.StartsWith("arm.land.")).ToDictionary(x => x.StableKey, x => x.LogicalId);
		var rerun = ArmageddonRegionalLandInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual("Builder mixed land", db.RegionalGatheringProfiles.Single(x => x.Id == mixedRow.Id).Name);
		Assert.AreEqual(2L, db.RegionalGatheringProfiles.Single(x => x.Id == mixedRow.Id).Revision);
		Assert.AreEqual("Builder grassland", db.Terrains.Single(x => x.Id == fertile.Id).Name);
		Assert.AreEqual(0, db.RegionalGatheringPools.Count());
		foreach (var claim in claims) Assert.AreEqual(claim.Value, db.SeederManagedRecords.Single(x => x.StableKey == claim.Key).LogicalId);
	}
}
