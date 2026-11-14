#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.Magic;
using FutureProg = MudSharp.Models.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfileProgConsolidationTests
{
	private static OwnedInMemorySeederContext Database() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);

	private static FutureProg Prog(long id, string name, string text = "return 7") => new()
	{
		Id = id, FunctionName = name, FunctionText = text, ReturnTypeDefinition = ProgVariableTypes.Number.ToStorageString(),
		Category = "Armageddon", Subcategory = "Worldfile", FunctionComment = "fixture",
		FutureProgsParameters = [new FutureProgsParameter
		{
			FutureProgId = id, ParameterIndex = 0, ParameterName = "value", ParameterTypeDefinition = ProgVariableTypes.Number.ToStorageString()
		}]
	};

	[TestMethod]
	public void CompletedPayloads_RelinkForeignKeysXmlAndOnLoadKeysAndSurviveColdReload()
	{
		using var db = Database();
		var first = Prog(1, "First"); var second = Prog(2, "Second");
		db.FutureProgs.AddRange(first, second);
		var item = new GameItemProto { Id = 20, RevisionNumber = 0 };
		db.GameItemProtos.Add(item);
		db.GameItemProtosOnLoadProgs.Add(new GameItemProtosOnLoadProgs
		{
			GameItemProtoId = 20, GameItemProtoRevisionNumber = 0, GameItemProto = item, FutureProgId = 2, FutureProg = second
		});
		db.Crafts.Add(new Craft { Id = 30, RevisionNumber = 0, CanUseProgId = 2, CanUseProg = second });
		db.NpcTemplates.Add(new NpcTemplate { Id = 2, RevisionNumber = 0, Definition = "<Definition><OnLoadProg>2</OnLoadProg><TemplateId>2</TemplateId></Definition>" });
		db.ArtificialIntelligences.Add(new ArtificialIntelligence { Id = 2, Definition = "<Definition><WillAttackProg>2</WillAttackProg><MaximumRange>2</MaximumRange></Definition>" });
		db.Hooks.Add(new Hooks { Id = 2, Definition = "<Definition><FutureProg>2</FutureProg></Definition>" });
		db.Rooms.Add(new Room { Id = 2, EffectData = "<Effects><Effect><OnLoadProg>2</OnLoadProg><TemplateRoomId>2</TemplateRoomId></Effect></Effects>" });
		db.SeederManagedRecords.AddRange(
			new SeederManagedRecord { Seeder = "fixture", Module = "fixture", StableKey = "source-prog", EntityType = nameof(FutureProg), LogicalId = 2, SeedBaseline = "{\"HistoricalProg\":2}" },
			new SeederManagedRecord { Seeder = "fixture", Module = "fixture", StableKey = "source-item", EntityType = nameof(GameItemProto), LogicalId = 2, SeedBaseline = "{}" });
		db.SaveChanges();

		var aliases = ArmageddonWorldfileProgConsolidation.Apply(db, [first, second]);
		Assert.IsTrue(db.ChangeTracker.AutoDetectChangesEnabled);
		Assert.HasCount(1, aliases); Assert.AreEqual(2L, aliases[0].OriginalId); Assert.AreEqual(1L, aliases[0].CanonicalId);
		db.SaveChanges(); db.ChangeTracker.Clear();
		Assert.AreEqual(1, db.FutureProgs.Count()); Assert.AreEqual(1, db.FutureProgsParameters.Count());
		Assert.AreEqual(1L, db.Crafts.Include(x => x.CanUseProg).Single().CanUseProg.Id);
		Assert.AreEqual(1L, db.GameItemProtosOnLoadProgs.Single().FutureProgId);
		var npc = XElement.Parse(db.NpcTemplates.Single().Definition);
		Assert.AreEqual("1", npc.Element("OnLoadProg")!.Value); Assert.AreEqual("2", npc.Element("TemplateId")!.Value);
		Assert.AreEqual("1", XElement.Parse(db.ArtificialIntelligences.Single().Definition).Element("WillAttackProg")!.Value);
		Assert.AreEqual("1", XElement.Parse(db.Hooks.Single().Definition).Element("FutureProg")!.Value);
		Assert.AreEqual("1", XElement.Parse(db.Rooms.Single().EffectData).Descendants("OnLoadProg").Single().Value);
		Assert.AreEqual(1L, db.SeederManagedRecords.Single(x => x.StableKey == "source-prog").LogicalId);
		Assert.AreEqual("{\"HistoricalProg\":2}", db.SeederManagedRecords.Single(x => x.StableKey == "source-prog").SeedBaseline);
		Assert.AreEqual(2L, db.SeederManagedRecords.Single(x => x.StableKey == "source-item").LogicalId);
	}

	[TestMethod]
	public void NamedCallTargetRetainsIdentityAndCompilesAfterOtherEqualRowsAreRemoved()
	{
		using var db = Database();
		var first = Prog(1, "First"); var named = Prog(2, "NamedTarget"); var third = Prog(3, "Third");
		var caller = Prog(4, "Caller", "return @NamedTarget(@value)");
		var existing = Prog(50, "BuilderOwned");
		db.FutureProgs.AddRange(first, named, third, caller, existing); db.SaveChanges();
		var aliases = ArmageddonWorldfileProgConsolidation.Apply(db, [first, named, third, caller]);
		Assert.HasCount(2, aliases); Assert.IsTrue(aliases.All(x => x.CanonicalId == 2));
		db.SaveChanges(); db.ChangeTracker.Clear();
		Assert.IsTrue(db.FutureProgs.Any(x => x.Id == 50 && x.FunctionName == "BuilderOwned"));
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), []);
		compiler.Compile(2);
		var runtime = compiler.Compile(4);
		Assert.AreEqual(7.0, runtime.ExecuteDouble(99.0));
	}

	[TestMethod]
	public void SignatureVisibilityStaticModeAndFinalMutationAreNotCombined()
	{
		using var db = Database();
		var progs = Enumerable.Range(1, 7).Select(x => Prog(x, "Generated" + x)).ToArray();
		progs[1].FutureProgsParameters.Single().ParameterName = "other";
		progs[2].AcceptsAnyParameters = true;
		progs[3].Public = true;
		progs[4].StaticType = 2;
		db.FutureProgs.AddRange(progs); db.SaveChanges();
		progs[5].FunctionText = "return 8"; // A late per-source setup pass changes only this program.
		db.ChangeTracker.AutoDetectChangesEnabled = false;
		var aliases = ArmageddonWorldfileProgConsolidation.Apply(db, progs);
		Assert.IsFalse(db.ChangeTracker.AutoDetectChangesEnabled);
		Assert.HasCount(1, aliases); Assert.AreEqual(7L, aliases[0].OriginalId);
		db.SaveChanges(); db.ChangeTracker.Clear();
		Assert.AreEqual("return 8", db.FutureProgs.Single(x => x.Id == 6).FunctionText);
		Assert.AreEqual("return 7", db.FutureProgs.Single(x => x.Id == 1).FunctionText);
	}

	[TestMethod]
	public void EqualOnLoadRowsOnSamePrototypeRemainTwoSeparateInvocations()
	{
		using var db = Database();
		var first = Prog(1, "First"); var second = Prog(2, "Second"); db.FutureProgs.AddRange(first, second);
		var item = new GameItemProto { Id = 20, RevisionNumber = 0 }; db.GameItemProtos.Add(item);
		foreach (var prog in new[] { first, second }) db.GameItemProtosOnLoadProgs.Add(new GameItemProtosOnLoadProgs
		{
			GameItemProtoId = 20, GameItemProtoRevisionNumber = 0, GameItemProto = item, FutureProgId = prog.Id, FutureProg = prog
		});
		db.SaveChanges();
		Assert.HasCount(0, ArmageddonWorldfileProgConsolidation.Apply(db, [first, second]));
		db.SaveChanges(); db.ChangeTracker.Clear();
		Assert.AreEqual(2, db.GameItemProtosOnLoadProgs.Count());
		Assert.AreEqual(2, db.FutureProgs.Count());
	}

	[TestMethod]
	public void XmlReferencesAreTypedAndPreserveUnrelatedNumbersAndLiteralMarkup()
	{
		var xml = "<Definition scalarProg=\"2\" Amount=\"2\"><OnLoadProg>2</OnLoadProg><Prog Id=\"2\"/><Amount>2</Amount><Text><![CDATA[<OnLoadProg>2</OnLoadProg>]]></Text></Definition>";
		var result = XElement.Parse(ArmageddonWorldfileProgConsolidation.RewriteXml(xml,
			new System.Collections.Generic.Dictionary<long, FutureProg> { [2] = Prog(1, "Canonical") }));
		Assert.AreEqual("1", result.Attribute("scalarProg")!.Value); Assert.AreEqual("2", result.Attribute("Amount")!.Value);
		Assert.AreEqual("1", result.Element("OnLoadProg")!.Value); Assert.AreEqual("1", result.Element("Prog")!.Attribute("Id")!.Value);
		Assert.AreEqual("2", result.Element("Amount")!.Value); Assert.AreEqual("<OnLoadProg>2</OnLoadProg>", result.Element("Text")!.Value);
	}

	[TestMethod]
	public void TrapJsonAndConfiguredProgramIdsResolveAfterColdReload()
	{
		using var db = Database();
		var first = Prog(1, "First"); var second = Prog(2, "Second"); db.FutureProgs.AddRange(first, second);
		db.TrapTemplates.Add(new TrapTemplate { Id = 20, RevisionNumber = 0, Definition = "<TrapTemplate><Payloads><Payload type=\"ExecuteProg\"><Parameter name=\"prog\">2</Parameter><Parameter name=\"amount\">2</Parameter></Payload></Payloads></TrapTemplate>" });
		var policy = new ConditionalMagicItemPolicy { Kind = ConditionalMagicItemKind.ItemIssuer, EligibilityProgId = 2, IssuedPrototypeIds = [2] };
		db.GameItemComponentProtos.Add(new GameItemComponentProto { Id = 30, RevisionNumber = 0, Definition = policy.Save().ToString() });
		db.StaticConfigurations.Add(new StaticConfiguration { SettingName = "DefaultWearableScalarSizeProg", Definition = "2" });
		db.StaticConfigurations.Add(new StaticConfiguration { SettingName = "SomeItemId", Definition = "2" });
		db.SaveChanges();
		Assert.HasCount(1, ArmageddonWorldfileProgConsolidation.Apply(db, [first, second]));
		db.SaveChanges(); db.ChangeTracker.Clear();
		var trap = XElement.Parse(db.TrapTemplates.Single().Definition);
		Assert.AreEqual("1", trap.Descendants("Parameter").Single(x => (string?)x.Attribute("name") == "prog").Value);
		Assert.AreEqual("2", trap.Descendants("Parameter").Single(x => (string?)x.Attribute("name") == "amount").Value);
		var loaded = ConditionalMagicItemPolicy.Load(XElement.Parse(db.GameItemComponentProtos.Single().Definition));
		Assert.AreEqual(1L, loaded.EligibilityProgId); CollectionAssert.AreEqual(new long[] { 2 }, loaded.IssuedPrototypeIds);
		Assert.AreEqual("1", db.StaticConfigurations.Single(x => x.SettingName == "DefaultWearableScalarSizeProg").Definition);
		Assert.AreEqual("2", db.StaticConfigurations.Single(x => x.SettingName == "SomeItemId").Definition);
	}

	[TestMethod]
	public void NumericProgCallRetainsItsTargetAndLiteralText()
	{
		using var db = Database();
		var first = Prog(1, "First"); var second = Prog(2, "Second");
		var caller = Prog(3, "Caller", "setweaponpoison(@item, 2)\nreturn true");
		db.FutureProgs.AddRange(first, second, caller); db.SaveChanges();
		var aliases = ArmageddonWorldfileProgConsolidation.Apply(db, [first, second, caller]);
		Assert.HasCount(1, aliases); Assert.AreEqual(1L, aliases[0].OriginalId); Assert.AreEqual(2L, aliases[0].CanonicalId);
		db.SaveChanges(); db.ChangeTracker.Clear();
		Assert.AreEqual(2L, db.FutureProgs.Single(x => x.FunctionName == "Second").Id);
		Assert.AreEqual("setweaponpoison(@item, 2)\nreturn true", db.FutureProgs.Single(x => x.Id == 3).FunctionText);
	}

	[TestMethod]
	public void DynamicNumericProgCallConservativelyPreventsIdentityRemoval()
	{
		using var db = Database();
		var first = Prog(1, "First"); var second = Prog(2, "Second");
		var caller = Prog(3, "Caller", "setweaponpoison(@item, getregister(@item, \"delivery\"))\nreturn true");
		db.FutureProgs.AddRange(first, second, caller); db.SaveChanges();
		Assert.HasCount(0, ArmageddonWorldfileProgConsolidation.Apply(db, [first, second, caller]));
		db.SaveChanges(); db.ChangeTracker.Clear(); Assert.AreEqual(3, db.FutureProgs.Count());
	}

	[TestMethod]
	public void LiquidDraughtScalarReferenceSurvivesAliasRemovalAndColdReload()
	{
		using var db = Database();
		var first = Prog(1, "First"); var second = Prog(2, "Second"); db.FutureProgs.AddRange(first, second);
		db.Liquids.Add(new Liquid { Id = 2, DraughtProgId = 2 }); db.SaveChanges();
		Assert.HasCount(1, ArmageddonWorldfileProgConsolidation.Apply(db, [first, second]));
		db.SaveChanges(); db.ChangeTracker.Clear();
		Assert.AreEqual(1L, db.Liquids.Single(x => x.Id == 2).DraughtProgId);
		Assert.IsTrue(db.FutureProgs.Any(x => x.Id == db.Liquids.Single().DraughtProgId));
	}
}
