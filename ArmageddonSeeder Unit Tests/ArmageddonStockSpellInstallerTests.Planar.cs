#nullable enable
using System;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSpellInstallerTests
{
	[TestMethod]
	[TestCategory("Integration")]
	public void PlanarDefinitions_BindOwnedShadowPlaneAndPreserveTransitionBounds()
	{
		using var db = Fixture();
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var installed = ArmageddonStockSpellInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		db.ChangeTracker.Clear();
		var shadow = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSupportInstaller.ShadowPlaneKey);
		Assert.AreEqual(nameof(Plane), shadow.EntityType);
		foreach (var (key, type, school, minimum, physical) in new[]
		{
			(ArmageddonPlanarSpellContent.ShadowPassageKey, "planarstate", "shadow", 20, false),
			(ArmageddonPlanarSpellContent.CrossTheVeilKey, "planeshift", "void", 10, true)
		})
		{
			var spell = db.MagicSpells.Single(x => x.Id == installed.Identities[key]);
			var definition = XElement.Parse(spell.Definition);
			var effect = definition.Element("Effects")!.Element("Effect")!;
			var planar = effect.Element("PlanarData")!;
			Assert.AreEqual(type, (string?)effect.Attribute("type"), key);
			Assert.AreEqual(shadow.LogicalId, (long?)planar.Element("Presence")!.Element("Plane")!.Attribute("id"), key);
			Assert.AreEqual(false, (bool?)planar.Element("Flags")!.Attribute("canCrossMagicalBarriers"), key);
			Assert.AreEqual(false, (bool?)planar.Element("Flags")!.Attribute("canCrossClosedDoors"), key);
			Assert.AreEqual(physical, (bool?)planar.Element("Flags")!.Attribute("propagatesInventory"), key);
			Assert.AreEqual(physical ? "manifested" : "noncorporeal", (string?)planar.Element("Transition")!.Attribute("profile"), key);
			Assert.AreEqual($"{minimum}*grade", db.TraitExpressions.Single(x => x.Id == installed.Identities[key + ".cost"]).Expression, key);
			Assert.AreEqual("60*grade", db.TraitExpressions.Single(x => x.Id == spell.EffectDurationExpressionId).Expression, key);
			Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == "arm.school." + school).LogicalId, spell.MagicSchoolId, key);
		}
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void EtherealDefinitions_BindOwnedProjectionAndSelectiveRemoval()
	{
		using var db = Fixture();
		var installed = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		db.ChangeTracker.Clear();
		var shadow = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonStockSupportInstaller.ShadowPlaneKey).LogicalId;
		var journey = db.MagicSpells.Single(x => x.Id == installed.Identities["arm.spell.unbodied_journey"]);
		Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == "arm.school.void").LogicalId, journey.MagicSchoolId);
		var journeyDefinition = XElement.Parse(journey.Definition);
		var projection = journeyDefinition.Element("Effects")!.Element("Effect")!;
		Assert.AreEqual("createprojection", (string?)projection.Attribute("type"));
		var policy = projection.Element("Policy")!;
		Assert.AreEqual(shadow, (long?)policy.Element("Plane"));
		Assert.AreEqual("UnbodiedJourney", (string?)policy.Element("Kind"));
		Assert.AreEqual(10, (int?)policy.Element("RequestedGradeRange")!.Attribute("perGrade"));
		Assert.AreEqual("60*grade", db.TraitExpressions.Single(x => x.Id == journey.EffectDurationExpressionId).Expression);
		Assert.AreEqual("12*grade", db.TraitExpressions.Single(x => x.Id == installed.Identities["arm.spell.unbodied_journey.cost"]).Expression);
		var flesh = db.MagicSpells.Single(x => x.Id == installed.Identities[ArmageddonEtherealRemovalContent.Key]);
		Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == "arm.school.shadow").LogicalId, flesh.MagicSchoolId);
		var removal = XElement.Parse(flesh.Definition).Element("Effects")!.Element("Effect")!;
		Assert.AreEqual("removeplanarstate", (string?)removal.Attribute("type"));
		Assert.AreEqual(true, (bool?)removal.Element("OnlySpellOwnedEthereal"));
		Assert.AreEqual("7*grade", db.TraitExpressions.Single(x => x.Id == installed.Identities[ArmageddonEtherealRemovalContent.Key + ".cost"]).Expression);
	}
}
