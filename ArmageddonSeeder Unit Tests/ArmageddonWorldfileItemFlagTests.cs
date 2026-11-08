#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.GameItems.Interfaces;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfileItemFlagTests
{
	[TestMethod]
	public void Permissions_SourceFlagsKeepDropGiveSeparateFromContainersAndExplicitTargeting()
	{
		var mask = ArmageddonWorldfileWriter.ResolveItemRestrictions((1 << 7) | (1 << 13) | (1 << 16) | (1 << 17), 0);
		Assert.IsTrue(mask.HasFlag(ItemInteractionRestriction.Pickup));
		Assert.IsTrue(mask.HasFlag(ItemInteractionRestriction.Drop));
		Assert.IsTrue(mask.HasFlag(ItemInteractionRestriction.Give));
		Assert.IsTrue(mask.HasFlag(ItemInteractionRestriction.OneHandedWield));
		Assert.IsTrue(mask.HasFlag(ItemInteractionRestriction.Sheathe));
		Assert.IsTrue(mask.HasFlag(ItemInteractionRestriction.RoomListing));
		Assert.IsFalse(mask.HasFlag(ItemInteractionRestriction.Put));
		Assert.AreEqual(ItemInteractionRestriction.None, ArmageddonWorldfileWriter.ResolveItemRestrictions(0, 1));
	}

	[TestMethod]
	public void ImportedFlags_ReachEffectiveVariantsAndOpeningStock_WithoutDiscardingEffects()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var source = ArmageddonWorldfileTestData.Load().ForInstallation();
		var writer = new ArmageddonWorldfileWriter(db, source);
		var field = typeof(ArmageddonWorldfileWriter).GetField("_effectiveItems", BindingFlags.NonPublic | BindingFlags.Instance)!;
		var effective = (Dictionary<long, (ArmWorldItem Source, int[] Values)>)field.GetValue(writer)!;
		var selected = new[] { 0, 1, 5, 6, 7, 12, 13, 14, 15, 16, 17, 18, 25 }
			.Select(bit => source.Items.FirstOrDefault(x => (x.Flags[1] & (1 << bit)) != 0))
			.Where(x => x is not null).DistinctBy(x => x!.Vnum).ToArray();
		long next = 800000;
		foreach (var item in selected)
		{
			foreach (var variant in new[] { false, true })
			{
				var id = next++;
				var proto = new Db.GameItemProto { Id = id, Name = variant ? "variant" : "base" };
				db.GameItemProtos.Add(proto);
				db.GameItems.Add(new Db.GameItem { Id = id, GameItemProtoId = id, EffectData = "<Effects preserved='true'/>" });
				effective.Add(id, (item!, item!.Values));
			}
		}
		var npcField = typeof(ArmageddonWorldfileWriter).GetField("_npcs", BindingFlags.NonPublic | BindingFlags.Instance)!;
		var npcs = (Dictionary<int, Db.NpcTemplate>)npcField.GetValue(writer)!;
		var eligible = source.Npcs.Where(x => (x.Flags[0] & (1 << 15)) != 0 && (x.Flags[2] & (1 << 14)) != 0).ToArray();
		foreach (var npc in eligible)
		{
			var template = new Db.NpcTemplate { Id = next++, Name = "disposal fixture", Definition = "<Definition><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>" };
			db.NpcTemplates.Add(template); npcs.Add(npc.Vnum, template);
		}
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallSourceInventoryDisposal", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(writer, null);
		foreach (var npc in eligible)
			Assert.AreEqual(1, XElement.Parse(npcs[npc.Vnum].Definition).Descendants("Hook").Count());
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallItemFlags", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(writer, null);
		foreach (var (id, data) in effective)
		{
			var proto = db.GameItemProtos.Local.Single(x => x.Id == id);
			var instance = db.GameItems.Local.Single(x => x.Id == id);
			Assert.AreEqual("true", (string?)XElement.Parse(instance.EffectData!).Attribute("preserved"));
			var restrictions = ArmageddonWorldfileWriter.ResolveItemRestrictions(data.Source.Flags[1], data.Source.Flags[2]);
			var components = proto.GameItemProtosGameItemComponentProtos.Select(link => db.GameItemComponentProtos.Local.Single(x => x.Id == link.GameItemComponentProtoId)).ToArray();
			if (restrictions != ItemInteractionRestriction.None)
			{
				var permission = components.Single(x => x.Type == "Item Permissions");
				Assert.AreEqual((int)restrictions, (int)XElement.Parse(permission.Definition).Element("Restrictions")!);
				Assert.IsTrue(instance.GameItemComponents.Any(x => x.GameItemComponentProtoId == permission.Id));
			}
			if ((data.Source.Flags[1] & (1 << 25)) != 0)
			{
				var retirement = components.Single(x => x.Type == "Transient Item");
				Assert.IsTrue((bool)XElement.Parse(retirement.Definition).Element("RetireOnReload")!);
			}
		}
		// Exercise the real offline registry/compiler, including all generated effect constructors and glyph callbacks.
		typeof(ArmageddonWorldfileWriter).GetMethod("CompileProgs", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(writer, null);
	}
}
