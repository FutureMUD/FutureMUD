#nullable enable
using System;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSupportInstallerTests
{
	[TestMethod]
	public void SourceGathering_PoliciesCompileAndRerunWithoutInstanceWrites()
	{
		using var db = Fixture();
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		foreach (var result in new[] { ArmageddonStockSupportInstaller.Install(factory),
			ArmageddonProgressionInstaller.InstallGatheringFoundations(factory), ArmageddonStockGatheringInstaller.Install(factory) })
			Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		var claims = db.SeederManagedRecords.AsNoTracking().ToDictionary(x => x.StableKey, x => x.LogicalId);
		Assert.AreEqual(13, db.FutureProgs.Count(x => x.Subcategory == "Gathering"));
		Assert.AreEqual(3, db.VariableDefinitions.Count());
		Assert.AreEqual(0, db.VariableValues.Count());
		var profile = XElement.Parse(db.MagicGenerators.Single(x => x.Id == claims[ArmageddonProgressionInstaller.LandProfileKey]).Definition);
		Assert.AreEqual("0", profile.Element("Outputs")!.Element("Output")!.Attribute("baserate")!.Value);
		Assert.AreEqual("0", profile.Element("NaturalRepairPerMinute")!.Value);
		var again = ArmageddonStockGatheringInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, again.Status, string.Join("\n", again.Messages));
		db.ChangeTracker.Clear();
		foreach (var claim in db.SeederManagedRecords) Assert.AreEqual(claims[claim.StableKey], claim.LogicalId);
		Assert.AreEqual(0, db.VariableValues.Count());
		Assert.IsFalse(db.MagicCapabilities.AsEnumerable().Any(x => x.Definition.Contains("<Gathering")));
	}
	[TestMethod]
	[DataRow(1.0)] [DataRow(100.0)]
	public void SourceGathering_ScarIsOneTotalPerAction_ConversionConsumesOneLifeUnit(double amount)
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSupportInstaller.Install(factory).Status);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonProgressionInstaller.InstallGatheringFoundations(factory).Status);
		var installed = ArmageddonStockGatheringInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		db.ChangeTracker.Clear();
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray(), db.VariableDefinitions.ToArray());
		var scar = compiler.Compile(installed.Identities["arm.prog.gather.land.scar"]);
		var ratio = compiler.Compile(installed.Identities["arm.prog.gather.land.ratio"]);
		Assert.AreEqual(1.0, scar.ExecuteDouble(null!, null!, null!, "land", amount, null!, null!));
		Assert.AreEqual(1.0, amount * ratio.ExecuteDouble(null!, null!, null!, "land", amount, null!, "pasture"), 0.0000001);
	}

	[TestMethod]
	[DataRow("register_type")] [DataRow("register_claim")] [DataRow("default")]
	[DataRow("policy_signature")] [DataRow("policy_body")] [DataRow("gate")]
	public void SourceGathering_InvalidRetainedContractsBlockBeforeWrites(string corruption)
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSupportInstaller.Install(factory).Status);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonProgressionInstaller.InstallGatheringFoundations(factory).Status);
		var first = ArmageddonStockGatheringInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status, string.Join("\n", first.Messages));
		db.ChangeTracker.Clear();
		switch (corruption)
		{
			case "register_type": db.VariableDefinitions.Single(x => x.Property == "armageddon_ecology").ContainedTypeDefinition = ProgVariableTypes.Text.ToStorageString(); break;
			case "register_claim": db.SeederManagedRecords.Single(x => x.StableKey == "arm.register.gather.ecology").LogicalId = 99; break;
			case "default": db.VariableDefaults.Single(x => x.Property == "armageddon_ecology").DefaultValue = "<var>101</var>"; break;
			case "policy_signature": db.FutureProgs.Single(x => x.Id == first.Identities["arm.prog.gather.self.permission"]).AcceptsAnyParameters = true; break;
			case "policy_body": db.FutureProgs.Single(x => x.Id == first.Identities["arm.prog.gather.self.damage"]).FunctionText = "not a program"; break;
			case "gate": db.FutureProgs.Single(x => x.Id == db.SeederManagedRecords.Single(y => y.StableKey == ArmageddonStockSupportInstaller.GatheringAvailabilityKey).LogicalId).ReturnTypeDefinition = ProgVariableTypes.Text.ToStorageString(); break;
		}
		db.SaveChanges(); db.ChangeTracker.Clear();
		var before = db.SeederManagedRecords.ToDictionary(x => x.StableKey, x => (x.LogicalId, x.AppliedAt, x.SeedBaseline));
		var result = ArmageddonStockGatheringInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status, string.Join("\n", result.Messages));
		Assert.AreEqual(0, result.Identities.Count); db.ChangeTracker.Clear();
		foreach (var row in db.SeederManagedRecords) Assert.AreEqual(before[row.StableKey], (row.LogicalId, row.AppliedAt, row.SeedBaseline));
	}

	[TestMethod]
	public void SourceGathering_RerunPreservesBuilderDefaultPolicyAndExistingInstanceValue()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSupportInstaller.Install(factory).Status);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonProgressionInstaller.InstallGatheringFoundations(factory).Status);
		var first = ArmageddonStockGatheringInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status, string.Join("\n", first.Messages));
		db.ChangeTracker.Clear();
		db.VariableDefaults.Single(x => x.Property == "armageddon_ecology").DefaultValue = "<var>17</var>";
		db.FutureProgs.Single(x => x.Id == first.Identities["arm.prog.gather.self.damage"]).FunctionText = "return 3";
		var gateId = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSupportInstaller.GatheringAvailabilityKey).LogicalId;
		db.FutureProgs.Single(x => x.Id == gateId).FunctionText = "return true";
		db.VariableValues.Add(new VariableValue { ReferenceTypeDefinition = ProgVariableTypes.Character.ToStorageString(), ReferenceId = 123,
			ReferenceProperty = "armageddon_ecology", ValueTypeDefinition = ProgVariableTypes.Number.ToStorageString(), ValueDefinition = "<var>-37</var>" });
		db.SaveChanges(); db.ChangeTracker.Clear();
		var result = ArmageddonStockGatheringInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual("<var>17</var>", db.VariableDefaults.Single(x => x.Property == "armageddon_ecology").DefaultValue);
		Assert.AreEqual("return 3", db.FutureProgs.Single(x => x.Id == first.Identities["arm.prog.gather.self.damage"]).FunctionText);
		Assert.AreEqual("return true", db.FutureProgs.Single(x => x.Id == gateId).FunctionText);
		Assert.AreEqual("<var>-37</var>", db.VariableValues.Single().ValueDefinition);
	}

}
