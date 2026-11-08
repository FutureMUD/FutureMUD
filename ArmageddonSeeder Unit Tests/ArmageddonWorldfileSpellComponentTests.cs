#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfileSpellComponentTests
{
	[TestMethod]
	public void Components_AllFamiliesKeepExactRanksAndPreserveUnrelatedTagsAndModifiedHierarchies()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var source = ArmageddonWorldfileTestData.Load().ForInstallation();
		Assert.IsNull(ArmageddonWorldfileWriter.NamedGatePackageError(source));
		Assert.IsNotNull(ArmageddonWorldfileWriter.NamedGatePackageError(new ArmageddonWorldfilePackage()));
		var writer = new ArmageddonWorldfileWriter(db, source);
		var placed = source.Resets.Where(x => x.Opcode is "O" or "G" or "E" or "P").Select(x => x.Arguments[1]).ToHashSet();
		var ids = new Dictionary<(string, int), long>();
		long next = 800000;
		foreach (var sphere in new[] { "abjuration", "alteration", "conjuration", "enchantment", "illusion", "invocation", "divination", "necromancy", "clerical", "nature", "teleportation", "creation" })
		for (var rank = 0; rank <= 4; rank++)
		{
			var id = next++;
			ids.Add((sphere, rank), id);
			db.Tags.Add(new Db.Tag { Id = id, Name = sphere + rank, ParentId = rank > 0 && sphere is ("alteration" or "divination" or "clerical" or "teleportation") ? ids[(sphere, rank - 1)] : null });
			db.SeederManagedRecords.Add(new Db.SeederManagedRecord { Seeder = "fixture", Module = "components", StableKey = $"arm.material.{sphere}.rank{rank}.tag", EntityType = nameof(Db.Tag), LogicalId = id });
		}
		var customTag = new Db.Tag { Id = 900000, Name = "Builder custom tag" };
		db.Tags.Add(customTag);
		var items = Field<Dictionary<int, Db.GameItemProto>>(writer, "_items");
		var effective = Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems");
		foreach (var item in source.Items.Where(x => x.ItemType == 2))
		{
			var proto = new Db.GameItemProto { Id = item.Vnum, Name = item.ShortDescription };
			proto.GameItemProtosTags.Add(new Db.GameItemProtosTags { TagId = customTag.Id });
			db.GameItemProtos.Add(proto);
			db.GameItems.Add(new Db.GameItem { Id = proto.Id, GameItemProtoId = proto.Id, EffectData = "<Effects custom='preserved'/>" });
			items.Add(item.Vnum, proto);
			effective.Add(proto.Id, (item, item.Values));
		}
		// Effective reset values must win over the base definition's rank.
		var variantSource = source.Items.First(x => x.ItemType == 2 && placed.Contains(x.Vnum) && x.Values[0] == 1);
		var variant = new Db.GameItemProto { Id = 990001, Name = "reset fixture" };
		db.GameItemProtos.Add(variant);
		Field<Dictionary<string, Db.GameItemProto>>(writer, "_resetItemVariants").Add("component fixture", variant);
		effective.Add(variant.Id, (variantSource, new[] { 1, 0, 0, 0, 0, 0 }));
		db.SaveChanges();
		Install();
		db.SaveChanges();
		var mapped = 0;
		foreach (var (id, value) in effective)
		{
			var spheres = new[] { "abjuration", "alteration", "conjuration", "enchantment", "illusion", "invocation", "divination", "necromancy", "clerical", "nature", "teleportation", "creation" };
			var sphere = value.Values[0] is >= 0 and < 12 ? spheres[value.Values[0]] : null;
			var expected = sphere is not null && value.Values[1] is >= 0 and <= 6;
			var tags = db.GameItemProtosTags.Where(x => x.GameItemProtoId == id && x.TagId != customTag.Id).ToArray();
			Assert.AreEqual(expected ? 1 : 0, tags.Length, $"Vnum {value.Source.Vnum}, proto {id}");
			if (!expected) continue;
			mapped++;
			var expectedId = db.SeederManagedRecords.Single(x => x.StableKey == $"arm.material.{sphere}.rank{value.Values[1]}.tag").LogicalId;
			Assert.AreEqual(expectedId, tags[0].TagId);
		}
		Assert.IsTrue(mapped > 1);
		Assert.IsTrue(db.GameItems.All(x => x.EffectData == "<Effects custom='preserved'/>"));
		Assert.AreEqual(items.Count, db.GameItemProtosTags.Count(x => x.TagId == customTag.Id));
		var before = db.GameItemProtosTags.Count();
		Install();
		db.SaveChanges();
		Assert.AreEqual(before, db.GameItemProtosTags.Count());
		// A builder's changed hierarchy must not be repaired or silently accepted on a fresh binding.
		var altered = db.Tags.Find(ids[("alteration", 1)])!;
		altered.ParentId = customTag.Id;
		var blocked = new Db.GameItemProto { Id = 990002, Name = "modified hierarchy fixture" };
		db.GameItemProtos.Add(blocked);
		Field<Dictionary<string, Db.GameItemProto>>(writer, "_resetItemVariants").Add("blocked fixture", blocked);
		effective.Add(blocked.Id, (variantSource, new[] { 1, 1, 0, 0, 0, 0 }));
		db.SaveChanges();
		Install();
		db.SaveChanges();
		Assert.AreEqual(customTag.Id, altered.ParentId);
		Assert.AreEqual(0, db.GameItemProtosTags.Count(x => x.GameItemProtoId == blocked.Id));
		return;

		void Install() => typeof(ArmageddonWorldfileWriter).GetMethod("InstallPlacedSpellComponents", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
	}

	private static T Field<T>(object value, string name) => (T)value.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(value)!;
}
