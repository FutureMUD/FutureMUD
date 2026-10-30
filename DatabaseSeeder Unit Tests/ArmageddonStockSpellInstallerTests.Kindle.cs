#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
	public void Kindle_LegacyOwnedWeaponDefinitionReconcilesInPlace_AndRerunIsUnchanged(ArmageddonProgressionMode mode)
	{
		using var db=Fixture(mode);var factory=ArmageddonInstallationSettingsTests.Factory(db);
		var first=ArmageddonStockSpellInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed,first.Status,string.Join("\n",first.Messages));db.ChangeTracker.Clear();
		var key=ArmageddonKindleContent.Key;
		var claim=db.SeederManagedRecords.Single(x => x.StableKey==key); var id=claim.LogicalId!.Value;
		var spell=db.MagicSpells.Single(x => x.Id==id); var xml=XElement.Parse(spell.Definition);
		xml.Element("Trigger")!.SetAttributeValue("type","item");
		xml.Element("Effects")!.ReplaceNodes(new XElement("Effect",new XAttribute("type","itemenchant"),new XElement("AttackCheckBonus",0)));
		xml.Element("ControlledPower")!.Element("ScalarBindings")!.ReplaceNodes(new XElement("Binding",new XAttribute("list","target"),
			new XAttribute("index",0),new XAttribute("effect","itemenchant"),new XAttribute("field","AttackCheckBonus"),"grade"));
		spell.Definition=xml.ToString();
		var baseline=JsonSerializer.Deserialize<Dictionary<string,string>>(claim.SeedBaseline!)!;
		baseline["Definition"]=JsonSerializer.Serialize(spell.Definition);claim.SeedBaseline=JsonSerializer.Serialize(baseline);
		var eligibilityClaim=db.SeederManagedRecords.Single(x => x.StableKey==key+".eligibility");
		var filterId=eligibilityClaim.LogicalId!.Value;var prog=db.FutureProgs.Single(x => x.Id==filterId);
		prog.FunctionText="return @target.isweapon";
		baseline=JsonSerializer.Deserialize<Dictionary<string,string>>(eligibilityClaim.SeedBaseline!)!;
		baseline["FunctionText"]=JsonSerializer.Serialize(prog.FunctionText);eligibilityClaim.SeedBaseline=JsonSerializer.Serialize(baseline);
		db.SaveChanges();db.ChangeTracker.Clear();
		var upgraded=ArmageddonStockSpellInstaller.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed,upgraded.Status,string.Join("\n",upgraded.Messages));db.ChangeTracker.Clear();
		Assert.AreEqual(id,db.SeederManagedRecords.Single(x => x.StableKey==key).LogicalId);
		Assert.AreEqual(filterId,db.SeederManagedRecords.Single(x => x.StableKey==key+".eligibility").LogicalId);
		Assert.AreEqual("return true",db.FutureProgs.Single(x => x.Id==filterId).FunctionText);
		xml=XElement.Parse(db.MagicSpells.Single(x => x.Id==id).Definition);
		Assert.AreEqual("itemchoice",(string?)xml.Element("Trigger")!.Attribute("type"));
		var effect=xml.Element("Effects")!.Elements().Single();Assert.AreEqual("itemempower",(string?)effect.Attribute("type"));
		Assert.AreEqual(8,effect.Elements("Option").Count());
		var mana = effect.Elements("Option").Where(x => (string?)x.Attribute("kind") == nameof(EquippedItemBonusKind.MagicResource)).Single();
		Assert.AreEqual("mana", (string?)mana.Attribute("name"));
		Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonProgressionInstaller.ManaResourceKey).LogicalId, (long?)mana.Attribute("target"));
		Assert.IsFalse(effect.Elements("Option").Any(x => (string?)x.Attribute("name")=="casting"));
		var definition=xml.ToString();var records=db.SeederManagedRecords.OrderBy(x => x.Id).AsEnumerable().Select(x => (x.Id,x.SeedBaseline,x.AppliedAt)).ToArray();
		Assert.AreEqual(ArmageddonInstallStatus.Completed,ArmageddonStockSpellInstaller.Install(factory).Status);db.ChangeTracker.Clear();
		Assert.AreEqual(definition,db.MagicSpells.Single(x => x.Id==id).Definition);
		CollectionAssert.AreEqual(records,db.SeederManagedRecords.OrderBy(x => x.Id).AsEnumerable().Select(x => (x.Id,x.SeedBaseline,x.AppliedAt)).ToArray());
	}
}
