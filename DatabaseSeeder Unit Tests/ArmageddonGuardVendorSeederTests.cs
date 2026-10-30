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
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public partial class ArmageddonGuardVendorSeederTests
{
	internal static T Field<T>(ArmageddonWorldfileWriter writer, string name) =>
		(T)typeof(ArmageddonWorldfileWriter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;

	[TestMethod]
	public void GuardsAndVendors_CompileAndPreserveExactSourceCommandAttachmentsAcrossResetVariants()
	{
		using var db = Fixture(out var writer, out var source);
		foreach (var method in new[] { "InstallClanLeaderAppointments", "InstallGuardSpecials", "InstallMantisVendors" })
			typeof(ArmageddonWorldfileWriter).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		foreach (var prog in db.FutureProgs.ToArray()) compiler.Compile(prog.Id);
		foreach (var npc in source.Npcs)
		{
			var template = db.NpcTemplates.Single(x => x.Id == npc.Vnum);
			var hooks = XElement.Parse(template.Definition).Element("TemplateLoadAdditions")!.Element("Hooks")!.Elements("Hook")
				.Select(x => db.Hooks.Single(h => h.Id == (long)x.Attribute("id")!)).ToArray();
			var sourceAsk = npc.Programs.Any(x => x is "C ask kohmar_body_guards" or "C ask gyp_gate_guard" or "C ask gyp_oasis_guard" or "C ask mantis_vendor");
			Assert.AreEqual(sourceAsk, hooks.Any(x => XElement.Parse(x.Definition).Element("TargetCommand")?.Value == "ask"), $"Vnum{npc.Vnum} ASK attachment");
			var variant = db.NpcTemplates.Single(x => x.Id == npc.Vnum + 100000);
			Assert.AreEqual(XElement.Parse(template.Definition).Element("TemplateLoadAdditions")!.Element("Hooks")!.ToString(),
				XElement.Parse(variant.Definition).Element("TemplateLoadAdditions")!.Element("Hooks")!.ToString());
		}
		Assert.IsFalse(XElement.Parse(db.NpcTemplates.Single(x => x.Id == 6001).Definition).Element("TemplateLoadAdditions")!.Element("Hooks")!.Elements("Hook")
			.Any(x => XElement.Parse(db.Hooks.Single(h => h.Id == (long)x.Attribute("id")!).Definition).Element("TargetCommand")?.Value == "ask"),
			"Raj has no source ASK callback even though his callback's C function contains an ASK branch.");
		var pitHook = db.Hooks.Single(x => x.Name == "Worldfile Mantis Pit");
		foreach (var vnum in new[] { 24015, 24016, 24017 })
			Assert.AreEqual(vnum == 24015, XElement.Parse(db.NpcTemplates.Single(x => x.Id == vnum).Definition)
				.Element("TemplateLoadAdditions")!.Element("Hooks")!.Elements("Hook").Any(x => (long)x.Attribute("id")! == pitHook.Id));
		Assert.AreEqual(0, db.Shops.Count(), "The vendors dispense free stock; they are not monetary shops.");
		Assert.AreEqual(0, db.ClanMembershipsAppointments.Count(), "Leadership remains explicitly assigned by builders.");
	}

	[TestMethod]
	public void InitialStates_ComposeCashOnLoadWithNativeEffectsAndPreserveUnsignedFlags()
	{
		using var db = Fixture(out var writer, out var source);
		source = Field<ArmageddonWorldfilePackage>(writer, "_source");
		var index = Array.FindIndex(source.Npcs, x => x.Vnum == 46704);
		var flags = source.Npcs[index].Flags.ToArray();
		flags[2] = unchecked((int)(0x80000000u | (1u << 0) | (1u << 1) | (1u << 3) | (1u << 4) | (1u << 5) | (1u << 6) | (1u << 7) |
			(1u << 8) | (1u << 11) | (1u << 12) | (1u << 13) | (1u << 16) | (1u << 17) | (1u << 19) |
			(1u << 20) | (1u << 23) | (1u << 24) | (1u << 26) | (1u << 27) | (1u << 30)));
		source.Npcs[index] = source.Npcs[index] with { Flags = flags };
		db.Planes.Add(new Plane { Id = 8000, Name = "Shadow", Alias = "shadow", Description = "", RoomDescriptionAddendum = "", RoomNameFormat = "", RemoteObservationTag = "" });
		db.SeederManagedRecords.Add(new SeederManagedRecord { Seeder = "Fixture", StableKey = "arm.support.plane.shadow", EntityType = nameof(Plane), LogicalId = 8000 });
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallNpcInitialStates", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		var cash = db.FutureProgs.Single(x => x.FunctionName == "FixtureCash46704");
		StringAssert.Contains(cash.FunctionText, "setregister @npc \"fixture_cash\" 123");
		foreach (var name in new[] { "addblindnesseffect", "addvisualinviseffect", "adddetectinvisibleeffect", "adddetectmagickeffect", "adddetectetherealeffect",
			"addflighteffect", "addinfravisioneffect", "addsneakeffect", "addhideeffect", "adddetectpoisoneffect", "addsilenceeffect",
			"addclimbingeffect", "addspottingeffect", "addlisteningeffect", "addpacifismeffect" })
			StringAssert.Contains(cash.FunctionText, name + "(@npc)");
		StringAssert.Contains(cash.FunctionText, "addmovementtempoeffect(@npc, 0.5)");
		StringAssert.Contains(cash.FunctionText, "addmovementtempoeffect(@npc, 2)");
		StringAssert.Contains(cash.FunctionText, "adddamageprotectioneffect(@npc, 0, 1)");
		StringAssert.Contains(cash.FunctionText, "adddamageprotectioneffect(@npc, 0.5, 0)");
		StringAssert.Contains(cash.FunctionText, "applyplanarstate(@npc, \"noncorporeal\", \"8000\", false, false, false)");
		Assert.IsTrue(db.SeederManagedRecords.Any(x => x.StableKey.EndsWith("npc-initial-state.46704") && x.SeedBaseline!.Contains("2147483648")),
			"The high PSI bit is retained as provenance rather than lost through signed masking.");
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		compiler.Compile(cash.Id);
	}

	[TestMethod]
	public void NobleGate_UsesNativeDoorguardAndProtectsTheOpenPassageWithoutReplacingCashOnLoad()
	{
		using var db = Fixture(out var writer, out _);
		foreach (var method in new[] { "InstallClanLeaderAppointments", "InstallGuardSpecials" })
			typeof(ArmageddonWorldfileWriter).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		Assert.IsFalse(Field<Dictionary<int, Clan>>(writer, "_clans").ContainsKey(58));
		StringAssert.Contains(db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileNobleGatePermission").FunctionText, "false or isclanmember");
		var ai = db.ArtificialIntelligences.Single();
		Assert.AreEqual("Doorguard", ai.Type);
		foreach (var id in new long[] { 50205, 150205 })
			Assert.IsTrue(db.NpcTemplates.Single(x => x.Id == id).NpctemplatesArtificalIntelligences.Any(x => x.AiId == ai.Id));
		var cash = db.FutureProgs.Single(x => x.FunctionName == "FixtureCash50205");
		StringAssert.Contains(cash.FunctionText, "setregister @npc \"fixture_cash\" 123");
		Assert.AreEqual(1, cash.FunctionText.Split("force @npc \"doorguard\"").Length - 1);
		var room = Field<Dictionary<int, Room>>(writer, "_rooms")[50881];
		StringAssert.Contains(room.EffectData, "RoomExitPolicy");
		StringAssert.Contains(room.EffectData, db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileNobleGateAdmission").Id.ToString());
	}

	internal static FuturemudDatabaseContext Fixture(out ArmageddonWorldfileWriter writer, out ArmageddonWorldfilePackage source)
	{
		var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var names = new[] { "kohmar_body_guards", "gyp_gate_guard", "gyp_oasis_guard", "noble_gate", "mantis_vendor" };
		source = new ArmageddonWorldfilePackage
		{
			Npcs = archive.Npcs.Where(x => x.Programs.Any(p => names.Any(s => p.EndsWith(" " + s, StringComparison.Ordinal)))).ToArray(),
			Items = archive.Items.Where(x => x.ItemType == 19 && x.Values[1] == 0).Take(1).ToArray()
		};
		writer = new ArmageddonWorldfileWriter(db, source);
		db.Races.Add(new Race { Id = 4000, Name = "Mantis" });
		foreach (var clanId in new[] { 7, 12, 17, 22, 23, 24, 26, 28, 36, 37, 39, 41 })
		{
			var clan = new Clan { Id = clanId + 1000, Name = "Clan " + clanId };
			db.Clans.Add(clan); Field<Dictionary<int, Clan>>(writer, "_clans").Add(clanId, clan);
		}
		var rank = new Rank { Id = 4026, ClanId = 1026, RankNumber = 2 };
		db.Ranks.Add(rank); Field<Dictionary<(int, int), Rank>>(writer, "_ranks").Add((26, 2), rank);
		foreach (var id in new[] { 20040, 50881, 6592, 13999, 13601 })
		{
			var room = new Room { Id = id + 100000, UniqueName = "Vnum" + id, EffectData = "<Effects/>" };
			db.Rooms.Add(room); Field<Dictionary<int, Room>>(writer, "_rooms").Add(id, room);
		}
		db.VariableDefinitions.Add(new VariableDefinition { OwnerTypeDefinition = ProgVariableTypes.Character.ToStorageString(), Property = "fixture_cash", ContainedTypeDefinition = ProgVariableTypes.Number.ToStorageString() });
		foreach (var npc in source.Npcs)
		{
			var cash = new MudSharp.Models.FutureProg { Id = 200000 + npc.Vnum, FunctionName = "FixtureCash" + npc.Vnum,
				FunctionText = "setregister @npc \"fixture_cash\" 123\nreturn true", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString() };
			cash.FutureProgsParameters.Add(new FutureProgsParameter { ParameterIndex = 0, ParameterName = "npc", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
			db.FutureProgs.Add(cash); Field<List<MudSharp.Models.FutureProg>>(writer, "_progs").Add(cash);
			var definition = $"<Definition><OnLoadProg>{cash.Id}</OnLoadProg><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>";
			var template = new NpcTemplate { Id = npc.Vnum, RevisionNumber = 0, Definition = definition };
			var variant = new NpcTemplate { Id = npc.Vnum + 100000, RevisionNumber = 0, Definition = definition };
			db.NpcTemplates.AddRange(template, variant);
			Field<Dictionary<int, NpcTemplate>>(writer, "_npcs").Add(npc.Vnum, template);
			Field<Dictionary<int, long[]>>(writer, "_npcFamilyIds").Add(npc.Vnum, [template.Id, variant.Id]);
		}
		foreach (var item in source.Items)
		{
			var proto = new GameItemProto { Id = item.Vnum };
			db.GameItemProtos.Add(proto); Field<Dictionary<int, GameItemProto>>(writer, "_items").Add(item.Vnum, proto);
		}
		db.SaveChanges();
		return db;
	}
}
