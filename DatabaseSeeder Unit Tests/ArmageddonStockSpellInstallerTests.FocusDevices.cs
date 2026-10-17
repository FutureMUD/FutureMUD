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
	[DataTestMethod]
	[DataRow(ArmageddonProgressionMode.Classic)]
	[DataRow(ArmageddonProgressionMode.Simple)]
	public void FocusDevices_LockedTwelveHaveNativeRecipesRealAllowListsAndDisabledGates(ArmageddonProgressionMode mode)
	{
		using var db = Fixture(mode);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var spells = ArmageddonStockSpellInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, spells.Status, string.Join("\n", spells.Messages));
		var result = ArmageddonStockFocusDeviceInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		Assert.AreEqual(12, result.Identities.Count);
		db.ChangeTracker.Clear();
		var claims = db.SeederManagedRecords.ToDictionary(x => x.StableKey);
		var progression = new ArmageddonStockProgression(mode);
		var use = db.FutureProgs.Single(x => x.Id == claims[ArmageddonStockFocusDeviceInstaller.UseGateKey].LogicalId);
		var craftGate = db.FutureProgs.Single(x => x.Id == claims[ArmageddonStockFocusDeviceInstaller.CraftGateKey].LogicalId);
		Assert.AreEqual("return false", use.FunctionText); Assert.AreEqual("return false", craftGate.FunctionText);
		Assert.AreEqual(2, db.FutureProgsParameters.Count(x => x.FutureProgId == use.Id));
		var kilograms = db.UnitsOfMeasure.Single(x => x.Id == ArmageddonInstallation.Read(db)!.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier;
		foreach (var blueprint in new ArmageddonStockCatalogue().Blueprints.Where(x => x.DeferredToS4))
		{
			var item = db.GameItemProtos.Include(x => x.GameItemProtosGameItemComponentProtos).Single(x => x.Id == result.Identities[blueprint.Key]);
			Assert.AreEqual(blueprint.Configuration.GetProperty("weight_kilograms").GetDouble() * kilograms, item.Weight, 0.00001);
			Assert.AreEqual(2, item.GameItemProtosGameItemComponentProtos.Count);
			var component = db.GameItemComponentProtos.Single(x => x.Id == claims[blueprint.Key + ".component"].LogicalId);
			Assert.AreEqual("ChargedMagicDevice", component.Type);
			var xml = XElement.Parse(component.Definition);
			var focus = blueprint.Kind == ArmageddonBlueprintKind.Focus;
			Assert.AreEqual((int)(focus ? MagicDeviceRole.Focus : MagicDeviceRole.Dual), (int)xml.Element("Role")!);
			Assert.AreEqual((int)MagicDeviceEligibility.Caster, (int)xml.Element("Eligibility")!);
			Assert.AreEqual(use.Id, (long)xml.Element("UseProg")!);
			Assert.AreEqual(focus ? 1 : blueprint.Configuration.GetProperty("capacity").GetInt32(), (int)xml.Element("Capacity")!);
			Assert.AreEqual(60, (int)xml.Element("Seconds")!);
			var keys = focus ? progression.Capabilities.Single(x => x.Key == blueprint.Key.Replace("arm.focus.", "arm.capability.")).SpellKeys : new[] { "arm.spell.mend_flesh" };
			CollectionAssert.AreEquivalent(keys.Select(x => claims[x].LogicalId!.Value).ToArray(), xml.Elements("Spell").Select(x => (long)x).ToArray());
			var craft = db.Crafts.Include(x => x.CraftInputs).Include(x => x.CraftProducts).Include(x => x.CraftPhases).Single(x => x.Id == claims[blueprint.Key + ".recipe"].LogicalId);
			Assert.AreEqual(craftGate.Id, craft.CanUseProgId); Assert.AreEqual(craftGate.Id, craft.AppearInCraftsListProgId);
			Assert.AreEqual(blueprint.Recipe!.Seconds, craft.CraftPhases.Sum(x => x.PhaseLengthInSeconds));
			Assert.AreEqual(2, craft.CraftPhases.Count); Assert.AreEqual(0.0, craft.CraftPhases.Single(x => x.PhaseNumber == 2).PhaseLengthInSeconds);
			Assert.AreEqual(blueprint.Recipe.Inputs.Count, craft.CraftInputs.Count);
			foreach (var input in blueprint.Recipe.Inputs)
			{
				var actual = craft.CraftInputs.Select(x => XElement.Parse(x.Definition)).Single(x => (long)x.Element("TargetItemId")! == claims[input.Key].LogicalId);
				Assert.AreEqual((int)input.Value, (int)actual.Element("Quantity")!);
			}
			Assert.AreEqual(item.Id, (long)XElement.Parse(craft.CraftProducts.Single().Definition).Element("ProductProducedId")!);
		}
		Assert.AreEqual(0, db.GameItems.Count());
		var retained = db.GameItemProtos.Single(x => x.Id == result.Identities["arm.focus.earth"]);
		retained.FullDescription = "Builder customised focus."; db.SaveChanges();
		var before = db.SeederManagedRecords.Count();
		var rerun = ArmageddonStockFocusDeviceInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages));
		db.ChangeTracker.Clear(); Assert.AreEqual(before, db.SeederManagedRecords.Count());
		Assert.AreEqual("Builder customised focus.", db.GameItemProtos.Single(x => x.Id == retained.Id).FullDescription);
		CollectionAssert.AreEquivalent(result.Identities.Values.ToArray(), rerun.Identities.Values.ToArray());
		foreach (var gateKey in new[] { ArmageddonStockFocusDeviceInstaller.UseGateKey, ArmageddonStockFocusDeviceInstaller.CraftGateKey })
		{
			var gate = db.FutureProgs.Single(x => x.Id == claims[gateKey].LogicalId); gate.FunctionText = "return true"; db.SaveChanges();
			var refused = ArmageddonStockFocusDeviceInstaller.Install(factory);
			Assert.AreEqual(ArmageddonInstallStatus.Failed, refused.Status); Assert.IsTrue(refused.Messages.Any(x => x.Contains(gateKey)));
			db.ChangeTracker.Clear(); gate = db.FutureProgs.Single(x => x.Id == claims[gateKey].LogicalId);
			Assert.AreEqual("return true", gate.FunctionText); gate.FunctionText = "return false"; db.SaveChanges();
		}

	}

	[TestMethod]
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
