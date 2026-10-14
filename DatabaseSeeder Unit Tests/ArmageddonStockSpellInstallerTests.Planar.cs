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
}
