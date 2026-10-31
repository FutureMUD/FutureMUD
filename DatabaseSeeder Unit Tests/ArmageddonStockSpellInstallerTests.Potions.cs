#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.Magic;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonStockSpellInstallerTests
{
	[TestMethod]
	[TestCategory("Integration")]
	public void PotionImport_AllPrivatePayloadsPreserveOrderDuplicatesEmptyItemsAndOpeningStock()
	{
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		using var db = Fixture();
		var result = ArmageddonStockSpellInstaller.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		var writer = new ArmageddonWorldfileWriter(db, archive);
		var items = (Dictionary<int, Db.GameItemProto>)typeof(ArmageddonWorldfileWriter)
			.GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
		var effective = (Dictionary<long, (ArmWorldItem Source, int[] Values)>)typeof(ArmageddonWorldfileWriter)
			.GetField("_effectiveItems", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
		var candidates = archive.Items.Where(x => x.ItemType == 10).ToArray();
		foreach (var source in candidates)
		{
			var proto = new Db.GameItemProto { Id = 1_000_000 + source.Vnum, Name = "Private potion fixture" };
			db.GameItemProtos.Add(proto); items.Add(source.Vnum, proto); effective.Add(proto.Id, (source, source.Values));
			db.GameItems.Add(new Db.GameItem { Id = proto.Id, GameItemProtoId = proto.Id, EffectData = "<Effects/>" });
		}
		var variantSource = candidates.First(x => x.Values[0] is >= 1 and <= 7 && x.Values.Skip(1).Take(3).Count(v => v > 0 && v is not (28 or 33 or 36 or 42)) > 1);
		var variantValues = variantSource.Values.ToArray();
		variantValues[0] = variantValues[0] == 7 ? 6 : variantValues[0] + 1;
		Array.Reverse(variantValues, 1, 3);
		var variant = new Db.GameItemProto { Id = 2_000_000, Name = "Effective reset potion fixture" };
		db.GameItemProtos.Add(variant);
		((Dictionary<string, Db.GameItemProto>)typeof(ArmageddonWorldfileWriter).GetField("_resetItemVariants", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!).Add("potion-test", variant);
		effective.Add(variant.Id, (variantSource, variantValues));
		db.GameItems.Add(new Db.GameItem { Id = variant.Id, GameItemProtoId = variant.Id, EffectData = "<Effects/>" });
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallPotions", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		using var manifest = JsonDocument.Parse(ArmageddonStockManifest.Read());
		var sourceKeys = manifest.RootElement.GetProperty("entries").EnumerateArray()
			.Where(x => x.TryGetProperty("historical_source", out var s) && s.ValueKind == JsonValueKind.Object &&
				s.TryGetProperty("skill_id", out var skill) && skill.ValueKind == JsonValueKind.Number)
			.ToDictionary(x => x.GetProperty("historical_source").GetProperty("skill_id").GetInt32(), x => x.GetProperty("key").GetString()!);
		var mappings = db.SeederManagedRecords.Where(x => !x.Retired && x.EntityType == nameof(Db.MagicSpell) && x.LogicalId != null)
			.ToDictionary(x => x.StableKey, x => x.LogicalId!.Value);
		var installed = 0;
		foreach (var source in candidates)
		{
			var payloads = source.Values.Skip(1).Take(3).Where(x => x > 0 && x is not (28 or 33 or 36 or 42)).ToArray();
			var proto = items[source.Vnum];
			if (payloads.Length > 0 && source.Values[0] < 1)
			{
				Assert.AreEqual(0, db.GameItemComponents.Count(x => x.GameItemId == proto.Id)); continue;
			}
			installed++;
			Assert.AreEqual(1, db.GameItemComponents.Count(x => x.GameItemId == proto.Id), $"Opening stock potion {source.Vnum}");
			var row = db.MagicalSubstances.SingleOrDefault(x => x.Name == "Vnum" + source.Vnum + " Potion " + proto.Id);
			if (payloads.Length == 0) { Assert.IsNull(row); continue; }
			Assert.IsNotNull(row);
			var entries = XElement.Parse(row.Definition).Elements("Entry").Select(MudSharp.Magic.MagicalSubstance.LoadEntry).ToArray();
			CollectionAssert.AreEqual(payloads.Select(x => mappings[x == 65 ? "potion.rewind" : sourceKeys[x]]).ToArray(), entries.Select(x => x.SpellId).ToArray());
			Assert.IsTrue(entries.All(x => x.NativeSpellGrade == Math.Min(source.Values[0], 7)));
			Assert.AreEqual(entries.Length, entries.Select(x => x.Key).Distinct().Count(), "Duplicate spells need independent activation identities.");
		}
		Assert.AreEqual(candidates.Count(x => x.Values.Skip(1).Take(3).All(v => v <= 0 || v is 28 or 33 or 36 or 42) || x.Values[0] >= 1), installed);
		foreach (var vnum in new[] { 66, 67, 19112, 65305 })
		{
			var source = candidates.Single(x => x.Vnum == vnum);
			var proto = items[vnum];
			var substance = XElement.Parse(db.MagicalSubstances.Single(x => x.Name == "Vnum" + vnum + " Potion " + proto.Id).Definition);
			Assert.IsTrue(substance.Elements("Entry").All(x => (int)x.Attribute("nativegrade")! == 7));
			using var provenance = JsonDocument.Parse(db.SeederManagedRecords.Single(x => x.StableKey == "potion." + proto.Id).SeedBaseline);
			Assert.AreEqual(source.Values[0], provenance.RootElement.GetProperty("SourceLevel").GetInt32());
			Assert.AreEqual(7, provenance.RootElement.GetProperty("NativeGrade").GetInt32());
		}
		var variantEntries = XElement.Parse(db.MagicalSubstances.Single(x => x.Name == "Vnum" + variantSource.Vnum + " Potion " + variant.Id).Definition)
			.Elements("Entry").Select(MudSharp.Magic.MagicalSubstance.LoadEntry).ToArray();
		CollectionAssert.AreEqual(variantValues.Skip(1).Take(3).Where(x => x > 0 && x is not (28 or 33 or 36 or 42))
			.Select(x => mappings[x == 65 ? "potion.rewind" : sourceKeys[x]]).ToArray(), variantEntries.Select(x => x.SpellId).ToArray());
		Assert.IsTrue(variantEntries.All(x => x.NativeSpellGrade == variantValues[0]));
		Assert.AreEqual(1, db.GameItemComponents.Count(x => x.GameItemId == variant.Id));
		var hint = XElement.Parse(db.MagicSpells.Single(x => x.Id == mappings["potion.rewind"]).Definition);
		Assert.AreEqual("delayedlocationhint", (string?)hint.Element("Effects")!.Elements().Single().Attribute("type"));
		Assert.AreEqual(1.875, (double)hint.Element("Effects")!.Elements().Single().Element("DelayPerGrade")!);
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		foreach (var prog in db.FutureProgs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfilePotion"))) compiler.Compile(prog.Id);
		ArmageddonWorldfileTestData.AssertNoPrivateCorpusEmbedded();
	}
}
