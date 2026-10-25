#nullable enable
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSpellInstallerTests
{
	[TestMethod]
	[TestCategory("Integration")]
	public void StoneSkin_OrdinaryDexterityFallbackIsExplicitAndRetainedAfterAgilityAppears()
	{
		using var db = Fixture();
		db.TraitDefinitions.Single(x => x.Alias == "agi").Alias = "customagi";
		var dexterity = new TraitDefinition { Name = "Ordinary Dexterity", Alias = "dex", Type = 1, DerivedType = 0, OwnerScope = 0, TraitGroup = "Physical" };
		db.TraitDefinitions.Add(dexterity); db.SaveChanges();
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var installed = ArmageddonStockSpellInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, installed.Status, string.Join("\n", installed.Messages));
		db.ChangeTracker.Clear();
		var claim = db.SeederManagedRecords.Single(x => x.StableKey == "arm.binding.trait.agility");
		using var alias = JsonDocument.Parse(claim.SeedBaseline!);
		Assert.AreEqual(nameof(TraitDefinition), alias.RootElement.GetProperty("NativeType").GetString());
		Assert.AreEqual(dexterity.Id, alias.RootElement.GetProperty("NativeId").GetInt64());
		var spellId = installed.Identities["arm.spell.stone_skin"];
		var spell = db.MagicSpells.Single(x => x.Id == spellId);
		Assert.AreEqual(dexterity.Id, (long)XElement.Parse(spell.Definition).Element("Effects")!.Elements().Single(x => (string?)x.Attribute("type") == "boost").Attribute("trait")!);
		StringAssert.Contains(spell.Description, "Ordinary Dexterity");
		db.TraitDefinitions.Single(x => x.Alias == "customagi").Alias = "agi"; db.SaveChanges();
		var rerun = ArmageddonStockSpellInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual(spellId, rerun.Identities["arm.spell.stone_skin"]);
		Assert.AreEqual(dexterity.Id, (long)XElement.Parse(db.MagicSpells.Single(x => x.Id == spellId).Definition).Element("Effects")!.Elements().Single(x => (string?)x.Attribute("type") == "boost").Attribute("trait")!);
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void StoneSkin_AmbiguousPhysicalMobilityFallbackHoldsWithoutNewAttributeOrSpell()
	{
		using var db = Fixture();
		db.TraitDefinitions.Single(x => x.Alias == "agi").Alias = "customagi";
		foreach (var name in new[] { "First native Dexterity", "Second native Dexterity" })
			db.TraitDefinitions.Add(new() { Name = name, Alias = "dex", Type = 1, DerivedType = 0, OwnerScope = 0, TraitGroup = "Physical" });
		db.SaveChanges(); var attributes = db.TraitDefinitions.Count(x => x.Type == 1);
		var installed = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, installed.Status);
		Assert.AreEqual(attributes, db.TraitDefinitions.Count(x => x.Type == 1));
		Assert.IsFalse(db.SeederManagedRecords.Any(x => x.StableKey == "arm.binding.trait.agility" || x.StableKey == "arm.spell.stone_skin"));
	}
}
