#nullable enable
using System;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSpellInstallerTests
{
	[TestMethod]
	[TestCategory("Integration")]
	[DataRow(ArmageddonProgressionMode.Classic)]
	[DataRow(ArmageddonProgressionMode.Simple)]
	public void FocusDevices_RerunPreservesBuilderEditsAndRepairsLegacyNames(ArmageddonProgressionMode mode)
	{
		using var db = Fixture(mode);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var spells = ArmageddonStockSpellInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, spells.Status, string.Join("\n", spells.Messages));
		var result = ArmageddonStockFocusDeviceInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		var claims = db.SeederManagedRecords.ToDictionary(x => x.StableKey);
		Assert.AreEqual(0, db.GameItems.Count());
		var retained = db.GameItemProtos.Single(x => x.Id == result.Identities["arm.focus.earth"]);
		var waterFocus = db.GameItemProtos.Single(x => x.Id == result.Identities["arm.focus.water"]);
		ArmageddonItemNamingTests.MakeLegacy(db, waterFocus);
		retained.Name = "talisman";
		retained.UniqueName = "Builder's focus";
		retained.FullDescription = "Builder customised focus."; db.SaveChanges();
		var before = db.SeederManagedRecords.Count();
		var rerun = ArmageddonStockFocusDeviceInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages));
		db.ChangeTracker.Clear(); Assert.AreEqual(before, db.SeederManagedRecords.Count());
		Assert.AreEqual("Builder customised focus.", db.GameItemProtos.Single(x => x.Id == retained.Id).FullDescription);
		Assert.AreEqual("talisman", db.GameItemProtos.Single(x => x.Id == retained.Id).Name);
		Assert.AreEqual("Builder's focus", db.GameItemProtos.Single(x => x.Id == retained.Id).UniqueName);
		Assert.AreEqual("focus", db.GameItemProtos.Single(x => x.Id == waterFocus.Id).Name);
		Assert.AreEqual("Armageddon stock Water focus", db.GameItemProtos.Single(x => x.Id == waterFocus.Id).UniqueName);
		CollectionAssert.AreEquivalent(result.Identities.Values.ToArray(), rerun.Identities.Values.ToArray());
		foreach (var gateKey in new[] { ArmageddonStockFocusDeviceInstaller.UseGateKey, ArmageddonStockFocusDeviceInstaller.CraftGateKey })
		{
			var gate = db.FutureProgs.Single(x => x.Id == claims[gateKey].LogicalId); gate.FunctionText = "return true"; db.SaveChanges();
			var refused = ArmageddonStockFocusDeviceInstaller.Install(factory);
			Assert.AreEqual(ArmageddonInstallStatus.Completed, refused.Status, string.Join("\n", refused.Messages));
			db.ChangeTracker.Clear(); gate = db.FutureProgs.Single(x => x.Id == claims[gateKey].LogicalId);
			Assert.AreEqual("return true", gate.FunctionText); gate.FunctionText = "return false"; db.SaveChanges();
		}

	}

	[TestMethod]
	[TestCategory("Integration")]
	public void FocusDevices_MissingSpellRefusesBeforeAnyContentAndLostAcknowledgementResumes()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var before = db.SeederManagedRecords.Count();
		Assert.AreEqual(ArmageddonInstallStatus.Failed, ArmageddonStockFocusDeviceInstaller.Install(factory).Status);
		Assert.AreEqual(before, db.SeederManagedRecords.Count());
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSpellInstaller.Install(factory).Status);
		var lost = ArmageddonStockFocusDeviceInstaller.Install(factory, stage => { if (stage == ArmageddonInstallCheckpoint.AfterCommit) throw new InvalidOperationException("Lost acknowledgement"); });
		Assert.AreEqual(ArmageddonInstallStatus.CommittedConfirmationFailed, lost.Status);
		var rerun = ArmageddonStockFocusDeviceInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages));
		CollectionAssert.AreEquivalent(lost.Identities.Values.ToArray(), rerun.Identities.Values.ToArray());
	}
}
