#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using FutureProg = MudSharp.Models.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonHealerSeederTests
{
	private static T Field<T>(ArmageddonWorldfileWriter writer, string name) =>
		(T)typeof(ArmageddonWorldfileWriter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
	[TestMethod]
	public void Healers_ReuseNativeHealingAndIsolateNpcCastingAndConfiguredVenoms()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var source = new ArmageddonWorldfilePackage { Npcs = archive.Npcs.Where(x => x.Vnum is 21240 or 39009).ToArray() };
		var original = new MagicSpell { Id = 502, Name = "Purge Toxin", Definition = "<Definition><Effects><Effect type='removepoison'><Drug>503</Drug><Vector>8</Vector><Formula>0.001*grade</Formula><OwnedFamily>arm.effect.poison</OwnedFamily></Effect></Effects></Definition>" };
		db.MagicSpells.AddRange(new MagicSpell { Id = 501, Name = "Mend Flesh", Definition = "<Definition/>" }, original);
		var water = new MagicCapability { Id = 500, Name = "Water", CapabilityModel = "skilllevel", Definition =
			$"<Definition><Regenerators><Regenerator>5</Regenerator></Regenerators><Casting version='1' identity='{Guid.NewGuid()}' enabled='true' trait='11' source='1' reserve='2' passive='true' startingVersion='1'><Admission key='{Guid.NewGuid()}' spell='501' trait='11' starting='false' min='1' max='7' opening='30'><Prerequisite spell='123'/></Admission><Admission key='{Guid.NewGuid()}' spell='502' trait='12' starting='false' min='1' max='7' opening='40'/><Admission key='{Guid.NewGuid()}' spell='504' starting='true' min='1' max='7'/></Casting></Definition>" };
		db.MagicCapabilities.Add(water);
		db.Drugs.AddRange(new Drug { Id = 503, Name = "Owned Magical Toxin" }, new Drug { Id = 504, Name = "Cobra Venom" }, new Drug { Id = 505, Name = "Medicine" });
		db.Races.Add(new Race { Id = 10, Name = "Halfling" });
		foreach (var (key, type, id) in new[] { ("arm.capability.water", nameof(MagicCapability), 500L), ("arm.spell.mend_flesh", nameof(MagicSpell), 501L),
			("arm.spell.purge_toxin", nameof(MagicSpell), 502L), ("arm.effect.poison", nameof(Drug), 503L) })
			db.SeederManagedRecords.Add(new SeederManagedRecord { Seeder = "Fixture", StableKey = key, EntityType = type, LogicalId = id });
		db.SaveChanges();
		var originalWater = water.Definition; var originalCure = original.Definition;
		var writer = new ArmageddonWorldfileWriter(db, source);
		foreach (var id in new[] { 9, 25 }) Field<Dictionary<int, Clan>>(writer, "_clans").Add(id, new Clan { Id = id });
		foreach (var npc in source.Npcs)
		{
			var template = new NpcTemplate { Id = npc.Vnum, RevisionNumber = 0, Definition = "<Definition><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>" };
			db.NpcTemplates.Add(template); Field<Dictionary<int, NpcTemplate>>(writer, "_npcs").Add(npc.Vnum, template);
		}
		var cash = new FutureProg { Id = 10000, FunctionName = "ArmageddonWorldfileNpcCash21240", FunctionText = "return true", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString() };
		cash.FutureProgsParameters.Add(new FutureProgsParameter { ParameterIndex = 0, ParameterName = "npc", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		db.FutureProgs.Add(cash); Field<List<FutureProg>>(writer, "_progs").Add(cash); db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallHealerSpecials", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		Assert.AreEqual(1, db.MagicSpells.Count(x => x.Name == "Mend Flesh"));
		Assert.AreEqual(originalWater, water.Definition); Assert.AreEqual(originalCure, original.Definition);
		var cure = db.MagicSpells.Single(x => x.Name == "Cleanse Venom");
		CollectionAssert.AreEqual(new[] { 504L }, XElement.Parse(cure.Definition).Descendants("AdditionalDrugs").Elements("Drug").Select(x => (long)x).ToArray());
		var privatePolicy = XElement.Parse(db.MagicCapabilities.Single(x => x.Name == "Blackwing Healer").Definition).Element("Casting")!;
		Assert.AreEqual(2, privatePolicy.Elements("Admission").Count());
		Assert.IsTrue(privatePolicy.Elements("Admission").All(x => (bool)x.Attribute("starting")! && (int)x.Attribute("max")! == 1 && !x.Elements().Any()));
		StringAssert.Contains(cash.FunctionText, "enrolchannelcasting"); StringAssert.Contains(cash.FunctionText, "magicresourcecapacity");
		Assert.AreEqual(0, db.ChargenRolesMerits.Count());
		StringAssert.Contains(db.NpcTemplates.Single(x => x.Id == 21240).Definition, "<Merit>");
		var kelwin = db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileKelwinHealer");
		StringAssert.Contains(kelwin.FunctionText, "healthremaining(@patient, \"health\") > 20");
		StringAssert.Contains(kelwin.FunctionText, "channelspell");
		StringAssert.Contains(db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileHalflingHealer").FunctionText, "purgedrugdosages");
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		foreach (var prog in db.FutureProgs.ToArray()) compiler.Compile(prog.Id);
	}
}
