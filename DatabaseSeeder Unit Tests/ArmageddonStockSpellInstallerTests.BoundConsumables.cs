#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSpellInstallerTests
{
	[TestMethod]
	[TestCategory("Integration")]
	[DataRow(ArmageddonProgressionMode.Classic)]
	[DataRow(ArmageddonProgressionMode.Simple)]
	public void BoundConsumables_UseExactOwnedPrototypesPaidGradeAndOneUseLifecycle(ArmageddonProgressionMode mode)
	{
		using var db = Fixture(mode);
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var installed = ArmageddonStockSpellInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		foreach (var (spellKey, prototypeKey, trigger, minimum, lifecycle, family, seconds) in new[]
		{
			(ArmageddonBoundConsumableSpellContent.EmberSeedKey, "arm.item.ember_seed", "character", 20.0,
				"TemporaryCleanup", "ember-seed", "300*grade"),
			(ArmageddonBoundConsumableSpellContent.RestorativeMudKey, "arm.item.restorative_mud", "item", 7.0,
				"Permanent", "restorative-mud", (string?)null)
		})
		{
			var spell = db.MagicSpells.Single(x => x.Id == installed.Identities[spellKey]);
			var definition = XElement.Parse(spell.Definition);
			var effect = definition.Element("Effects")!.Elements().Single();
			var itemLifecycle = effect.Element("Lifecycle")!;
			Assert.AreEqual(trigger, (string?)definition.Element("Trigger")!.Attribute("type"), spellKey);
			Assert.AreEqual("createitem", (string?)effect.Attribute("type"), spellKey);
			Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == prototypeKey).LogicalId,
				(long?)effect.Element("ItemPrototypeId"), spellKey);
			Assert.AreEqual(1, (int)effect.Element("Quantity")!, spellKey);
			Assert.AreEqual(0, (long)effect.Element("ItemSkinId")!, spellKey);
			Assert.AreEqual("", effect.Element("LoadString")!.Value, spellKey);
			Assert.AreEqual(lifecycle, (string?)itemLifecycle.Attribute("mode"), spellKey);
			Assert.AreEqual(family, itemLifecycle.Element("Family")!.Value, spellKey);
			Assert.AreEqual(seconds, itemLifecycle.Element("Seconds")?.Value, spellKey);
			Assert.AreEqual(minimum, (double)definition.Element("ControlledPower")!.Element("Efficiency")!.Attribute("minimum")!, spellKey);
			Assert.AreEqual($"{minimum.ToString(System.Globalization.CultureInfo.InvariantCulture)}*grade",
				db.TraitExpressions.Single(x => x.Id == installed.Identities[spellKey + ".cost"]).Expression, spellKey);
			Assert.AreEqual("0", db.TraitExpressions.Single(x => x.Id == installed.Identities[spellKey + ".duration"]).Expression, spellKey);
			Assert.AreEqual(0, definition.Element("Plan")!.Elements().Count(), spellKey);
		}
		Assert.AreEqual(0, db.CharacterCastingEnrolments.Count());
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonStockSpellInstaller.Install(factory).Status);
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void RestorativeMud_RejectsAnOverBudgetOwnedDoseBeforeInstallingStock()
	{
		using var db = Fixture(ArmageddonProgressionMode.Classic);
		var componentId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.restorative_mud.component").LogicalId!.Value;
		var component = db.GameItemComponentProtos.Single(x => x.Id == componentId);
		var definition = XElement.Parse(component.Definition);
		definition.Element("NativeHealingDose")!.SetAttributeValue("budgetPerGrade", 11);
		component.Definition = definition.ToString(SaveOptions.DisableFormatting);
		db.SaveChanges();

		var installed = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, installed.Status);
		StringAssert.Contains(string.Join("\n", installed.Messages), "arm.item.restorative_mud");
	}
}
