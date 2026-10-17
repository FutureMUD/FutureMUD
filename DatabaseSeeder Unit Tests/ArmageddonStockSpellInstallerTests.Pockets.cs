#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSpellInstallerTests
{
	[DataTestMethod]
	[DataRow(ArmageddonProgressionMode.Classic)]
	[DataRow(ArmageddonProgressionMode.Simple)]
	public void PocketAndUnravel_ExactOwnedBindingsAndLockedDefaults_NoPlayerOrRoomCreation(ArmageddonProgressionMode mode)
	{
		using var db = Fixture(mode); var rooms = db.Rooms.Count();
		var installed = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		var carrier = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.folded_pocket");
		var spell = db.MagicSpells.Single(x => x.Id == installed.Identities["arm.spell.folded_pocket"]);
		var definition = XElement.Parse(spell.Definition);
		var configuration = SpellPocketConfiguration.Load(definition.Element("Effects")!.Elements().Single().Element("Pocket")!);
		var kilograms = db.UnitsOfMeasure.Single(x => x.Id == ArmageddonInstallation.Read(db)!.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier;
		Assert.AreEqual(carrier.LogicalId, configuration.PrototypeId); Assert.AreEqual(2 * kilograms, configuration.CapacityPerGrade);
		Assert.AreEqual(300.0, configuration.SecondsPerGrade); Assert.AreEqual(SpellPocketAccess.Bearer, configuration.Access);
		Assert.AreEqual(SizeCategory.VeryLarge, configuration.MaximumSize); Assert.AreEqual(73L, configuration.FallbackRoomId);
		Assert.AreEqual(7.0, (double)definition.Element("ControlledPower")!.Element("Efficiency")!.Attribute("minimum")!);
		foreach (var grade in new[] { 1, 7 })
		{
			Assert.AreEqual(2 * grade * kilograms, new SpellPocketAnchor(900, grade, configuration).Capacity);
			Assert.AreEqual(TimeSpan.FromSeconds(300 * grade), configuration.DurationForGrade(grade));
		}
		var unravel = db.MagicSpells.Single(x => x.Id == installed.Identities["arm.spell.unravel_enchantment"]);
		var dispel = XElement.Parse(unravel.Definition);
		Assert.AreEqual("character", (string?)dispel.Element("Trigger")!.Attribute("type"));
		Assert.AreEqual("0", db.TraitExpressions.Single(x => x.Id == unravel.EffectDurationExpressionId).Expression);
		var effect = dispel.Element("Effects")!.Elements().Single();
		Assert.AreEqual("dispelmagic", (string?)effect.Attribute("type")); Assert.AreEqual("any", effect.Element("EffectKey")!.Value);
		Assert.IsTrue((bool)effect.Element("Contest")!); Assert.IsTrue((bool)effect.Element("AllowHostile")!);
		Assert.AreEqual(7.0, (double)dispel.Element("ControlledPower")!.Element("Efficiency")!.Attribute("minimum")!);
		Assert.AreEqual(rooms, db.Rooms.Count()); Assert.AreEqual(0, db.GameItems.Count()); Assert.AreEqual(0, db.Characters.Count());
	}

	[DataTestMethod]
	[DataRow("missing-carrier")][DataRow("retired-carrier")][DataRow("wrong-component")]
	[DataRow("unapproved-component")][DataRow("small-capacity")][DataRow("small-size")][DataRow("tag-filter")][DataRow("closable")]
	public void Pocket_UnsafeTypedCarrier_HoldsWithoutAllocatingItsContribution(string change)
	{
		using var db = Fixture();
		var carrierClaim = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.folded_pocket");
		var carrier = db.GameItemProtos.Single(x => x.Id == carrierClaim.LogicalId && x.RevisionNumber == carrierClaim.RevisionNumber);
		var componentClaim = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.folded_pocket.component");
		var component = db.GameItemComponentProtos.Single(x => x.Id == componentClaim.LogicalId && x.RevisionNumber == componentClaim.RevisionNumber);
		if (change == "missing-carrier") db.GameItemProtos.Remove(carrier);
		if (change == "retired-carrier") carrierClaim.Retired = true;
		if (change == "wrong-component") component.Type = "Container";
		if (change == "unapproved-component") db.EditableItems.Single(x => x.Id == component.EditableItemId).RevisionStatus = (int)RevisionStatus.UnderDesign;
		if (change is "small-capacity" or "small-size" or "tag-filter" or "closable")
		{
			var xml = XElement.Parse(component.Definition);
			if (change == "small-capacity") xml.SetAttributeValue("Weight", 1);
			if (change == "small-size") xml.SetAttributeValue("MaxSize", (int)SizeCategory.Tiny);
			if (change == "tag-filter") xml.Element("AllowedTags")!.Add(new XElement("Tag", 999));
			if (change == "closable") xml.SetAttributeValue("Closable", true);
			component.Definition = xml.ToString();
		}
		db.SaveChanges(); db.ChangeTracker.Clear();
		var result = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status, string.Join("\n", result.Messages));
		Assert.IsTrue(db.SeederManagedRecords.Any(x => x.StableKey == "arm.spell.unravel_enchantment"));
		Assert.IsTrue(db.SeederManagedRecords.Any(x => x.StableKey == "arm.spell.severing_refuge"));
		Assert.IsFalse(db.SeederManagedRecords.Any(x => x.StableKey == "arm.spell.folded_pocket"));
	}
}
