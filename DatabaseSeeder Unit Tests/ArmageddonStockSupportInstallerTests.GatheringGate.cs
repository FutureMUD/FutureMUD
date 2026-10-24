#nullable enable
using System;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSupportInstallerTests
{
	[TestMethod]
	public void GatheringGate_IsExactTypedFalseAndRerunsWithoutGrantingMethods()
	{
		using var db = Fixture();
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonStockSupportInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status, string.Join("\n", first.Messages));
		db.ChangeTracker.Clear();
		var id = first.Identities[ArmageddonStockSupportInstaller.GatheringAvailabilityKey];
		var gate = db.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.Id == id);
		Assert.AreEqual("return false", gate.FunctionText);
		Assert.IsFalse(gate.AcceptsAnyParameters);
		CollectionAssert.AreEqual(new[] { ProgVariableTypes.Character, ProgVariableTypes.Character,
			ProgVariableTypes.MagicCapability, ProgVariableTypes.Text, ProgVariableTypes.Number, ProgVariableTypes.Location },
			gate.FutureProgsParameters.OrderBy(x => x.ParameterIndex)
				.Select(x => ProgVariableTypes.FromStorageString(x.ParameterTypeDefinition)).ToArray());
		using var compiler = new OfflineProgCompilation([gate]);
		var compiled = compiler.Compile(id);
		Assert.IsFalse(compiled.ExecuteBool(new object[] { null!, null!, null!, "self", 1.0, null! }));
		Assert.IsFalse(db.MagicCapabilities.AsEnumerable().Any(x => x.Definition.Contains("<Gathering")));
		var snapshot = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSupportInstaller.GatheringAvailabilityKey).SeedBaseline;
		var second = ArmageddonStockSupportInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, second.Status, string.Join("\n", second.Messages));
		Assert.AreEqual(id, second.Identities[ArmageddonStockSupportInstaller.GatheringAvailabilityKey]);
		db.ChangeTracker.Clear();
		Assert.AreEqual(snapshot, db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSupportInstaller.GatheringAvailabilityKey).SeedBaseline);
	}

	[TestMethod]
	public void GatheringGate_BuilderActivationIsPreservedOnRerun()
	{
		using var db = Fixture();
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonStockSupportInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status);
		var id = first.Identities[ArmageddonStockSupportInstaller.GatheringAvailabilityKey];
		db.FutureProgs.Single(x => x.Id == id).FunctionText = "return true";
		db.SaveChanges();
		var rerun = ArmageddonStockSupportInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual("return true", db.FutureProgs.Single(x => x.Id == id).FunctionText);
	}

	[TestMethod]
	[DataRow("static")]
	[DataRow("return_type")]
	[DataRow("any_parameters")]
	[DataRow("parameter_type")]
	[DataRow("parameter_name")]
	[DataRow("parameter_index")]
	[DataRow("ownership")]
	public void GatheringGate_EditedAvailabilityBlocksBeforeWritesAndPreservesEdit(string edit)
	{
		using var db = Fixture();
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonStockSupportInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status, string.Join("\n", first.Messages));
		db.ChangeTracker.Clear();
		var claim = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSupportInstaller.GatheringAvailabilityKey);
		var gate = db.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.Id == claim.LogicalId);
		switch (edit)
		{
			case "enabled": gate.FunctionText = "return true"; break;
			case "static": gate.StaticType = 1; break;
			case "return_type": gate.ReturnTypeDefinition = ProgVariableTypes.Number.ToStorageString(); break;
			case "any_parameters": gate.AcceptsAnyParameters = true; break;
			case "parameter_type": gate.FutureProgsParameters.Single(x => x.ParameterIndex == 0).ParameterTypeDefinition = ProgVariableTypes.Text.ToStorageString(); break;
			case "parameter_name": gate.FutureProgsParameters.Single(x => x.ParameterIndex == 0).ParameterName = "changed"; break;
			case "parameter_index":
				var parameter = gate.FutureProgsParameters.Single(x => x.ParameterIndex == 0);
				db.FutureProgsParameters.Remove(parameter);
				gate.FutureProgsParameters.Add(new() { ParameterIndex = 6, ParameterName = "actor", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
				break;
			case "ownership": claim.Module = "other-module"; break;
		}
		db.SaveChanges();
		db.ChangeTracker.Clear();
		var before = db.SeederManagedRecords.AsNoTracking().ToDictionary(x => x.StableKey,
			x => (x.LogicalId, x.SeedBaseline, x.AppliedAt, x.Module));
		var checkpointReached = false;
		var result = ArmageddonStockSupportInstaller.Install(factory, _ => checkpointReached = true);
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status, string.Join("\n", result.Messages));
		Assert.IsFalse(checkpointReached);
		Assert.AreEqual(0, result.Identities.Count);
		db.ChangeTracker.Clear();
		Assert.AreEqual(before.Count, db.SeederManagedRecords.Count());
		foreach (var row in db.SeederManagedRecords.AsNoTracking())
			Assert.AreEqual(before[row.StableKey], (row.LogicalId, row.SeedBaseline, row.AppliedAt, row.Module));
		Assert.AreEqual(edit == "enabled" ? "return true" : "return false", db.FutureProgs.Single(x => x.Id == gate.Id).FunctionText);
	}
}
