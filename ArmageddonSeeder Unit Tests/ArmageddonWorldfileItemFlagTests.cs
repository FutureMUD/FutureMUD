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
		SeedSourceMagicFixtures(db);
		var field = typeof(ArmageddonWorldfileWriter).GetField("_effectiveItems", BindingFlags.NonPublic | BindingFlags.Instance)!;
		var effective = (Dictionary<long, (ArmWorldItem Source, int[] Values)>)field.GetValue(writer)!;
		var selected = new[] { 0, 1, 4, 5, 6, 7, 12, 13, 14, 15, 16, 17, 18, 25, 29 }
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
			var cursed = (data.Source.Flags[1] & ((1 << 4) | (1 << 7))) != 0;
			if (restrictions != ItemInteractionRestriction.None || cursed)
			{
				var permission = components.Single(x => x.Type == "Item Permissions");
				Assert.AreEqual((int)restrictions, (int)XElement.Parse(permission.Definition).Element("Restrictions")!);
				Assert.AreEqual(cursed, (bool)XElement.Parse(permission.Definition).Element("Cursed")!);
				Assert.AreEqual((data.Source.Flags[1] & (1 << 7)) != 0 ? 6 : 0,
					(int)XElement.Parse(permission.Definition).Element("CurseRemovableRestrictions")!);
				Assert.IsTrue(instance.GameItemComponents.Any(x => x.GameItemComponentProtoId == permission.Id));
			}
			if ((data.Source.Flags[1] & (1 << 25)) != 0)
			{
				var retirement = components.Single(x => x.Type == "Transient Item");
				Assert.IsTrue((bool)XElement.Parse(retirement.Definition).Element("RetireOnReload")!);
			}
			var flags = data.Source.Flags[1];
			var loadTexts = proto.GameItemProtosOnLoadProgs.Select(x => db.FutureProgs.Local.Single(p => p.Id == x.FutureProgId).FunctionText).ToArray();
			if ((flags & (1 << 18)) != 0)
			{
				Assert.IsTrue(loadTexts.Any(x => x.Contains("addpickuphazard", StringComparison.Ordinal)));
				Assert.IsTrue(loadTexts.Any(x => x.Contains("flag:glyph", StringComparison.Ordinal)));
				var itemEffects = XElement.Parse(instance.EffectData!).Elements("Effect").ToArray();
				var hazard = itemEffects.Single(x => (string?)x.Element("Type") == "ItemPickupHazard");
				Assert.AreEqual(3, (int)hazard.Element("Hazard")!.Element("MinimumDispelGrade")!);
				var initialised = itemEffects.Single(x => (string?)x.Element("Type") == "ItemInitialisation").Element("Keys")!.Elements("Key").Select(x => x.Value).ToArray();
				CollectionAssert.Contains(initialised, "flag:glyph");
				CollectionAssert.Contains(initialised, "flag:magic");
			}
			if ((flags & ((1 << 14) | (1 << 29))) != 0)
			{
				Assert.IsTrue(loadTexts.Any(x => x.Contains("addetherealitemaffinity", StringComparison.Ordinal)));
				var affinity = XElement.Parse(instance.EffectData!).Elements("Effect").Single(x => (string?)x.Element("Type") == "EtherealItemAffinity");
				Assert.AreEqual((flags & (1 << 29)) != 0, (bool)affinity.Element("Definition")!.Element("ReadyForTransition")!);
				Assert.AreEqual((flags & (1 << 14)) != 0, (bool)affinity.Element("Definition")!.Element("VisibleToPlane")!);
			}
		}
		var glyphSpell = XElement.Parse(db.MagicSpells.Local.Single(x => x.Id == 82002).Definition);
		var glyphEffect = glyphSpell.Element("Effects")!.Elements().Single();
		Assert.AreEqual("pickuphazard", (string?)glyphEffect.Attribute("type"));
		Assert.AreEqual(3, (int)glyphEffect.Element("MinimumDispelGrade")!);
		Assert.IsTrue((long)glyphEffect.Element("ImmunityProg")! > 0);
		var immunity = db.FutureProgs.Local.Single(x => x.FunctionName == "ArmageddonWorldfileGlyphVeilImmunity");
		StringAssert.Contains(immunity.FunctionText, "activespells(@actor)");
		StringAssert.Contains(immunity.FunctionText, "tomagicspell(82001)");
		var unravel = XElement.Parse(db.MagicSpells.Local.Single(x => x.Id == 82003).Definition);
		Assert.AreEqual(true, (bool)unravel.Element("Trigger")!.Element("CanTargetItems")!);
		var dispel = unravel.Element("Effects")!.Elements().Single();
		Assert.AreEqual(true, (bool)dispel.Element("IncludeIntrinsicItemEffects")!);
		Assert.AreEqual(true, (bool)dispel.Element("Contest")!);
		var passage = XElement.Parse(db.MagicSpells.Local.Single(x => x.Id == 82004).Definition);
		Assert.AreEqual(true, (bool)passage.Element("Trigger")!.Element("CanTargetItems")!);
		var attunement = passage.Element("Effects")!.Elements().Single();
		Assert.AreEqual("etherealitemattunement", (string?)attunement.Attribute("type"));
		Assert.AreEqual(true, (bool)attunement.Element("AutoAdvance")!);
		Assert.AreEqual(3000.0, (double)attunement.Element("MassPerGrade")!);
		Assert.AreEqual(1, (int)attunement.Element("RiskPercentPerGrade")!);
		Assert.AreEqual(82000, (long)attunement.Element("CharacterPlanarData")!.Element("PlanarData")!.Element("Plane")!);
		Assert.AreEqual("true", (string?)attunement.Element("CharacterPlanarData")!.Element("PlanarData")!.Element("Flags")!.Attribute("retainPlaneSpecificInventory"));
		var returnSpell = XElement.Parse(db.MagicSpells.Local.Single(x => x.Id == 82005).Definition);
		Assert.AreEqual(true, (bool)returnSpell.Element("Trigger")!.Element("CanTargetItems")!);
		Assert.AreEqual(true, (bool)returnSpell.Element("Effects")!.Elements().Single().Element("ClearItemEtherealVisibility")!);
		// Exercise the real offline registry/compiler, including all generated effect constructors and glyph callbacks.
		typeof(ArmageddonWorldfileWriter).GetMethod("CompileProgs", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(writer, null);
	}

	private static void SeedSourceMagicFixtures(MudSharp.Database.FuturemudDatabaseContext db)
	{
		db.Planes.Add(new Db.Plane { Id = 82000, Name = "Shadow" });
		db.SeederManagedRecords.Add(new Db.SeederManagedRecord { Seeder = "Fixture", StableKey = "arm.support.plane.shadow", EntityType = nameof(Db.Plane), LogicalId = 82000 });
		void Spell(long id, string key, string name, XElement effect, string target = "character")
		{
			db.MagicSpells.Add(new Db.MagicSpell
			{
				Id = id, Name = name, Definition = new XElement("Definition",
					new XElement("Trigger", new XAttribute("type", target), new XElement("TargetFilterProg", 0)),
					new XElement("Effects", effect)).ToString()
			});
			db.SeederManagedRecords.Add(new Db.SeederManagedRecord { Seeder = "Fixture", StableKey = key, EntityType = nameof(Db.MagicSpell), LogicalId = id });
		}
		Spell(82001, "arm.spell.unyielding_veil", "Unyielding Veil", new XElement("Effect", new XAttribute("type", "spellarmour")));
		Spell(82002, "arm.spell.custodian_glyph", "Custodian Glyph", new XElement("Effect", new XAttribute("type", "owneditemmark")), "item");
		Spell(82003, "arm.spell.unravel_enchantment", "Unravel Enchantment",
			new XElement("Effect", new XAttribute("type", "dispelmagic"), new XElement("Contest", true)));
		Spell(82004, "arm.spell.shadow_passage", "Shadow Passage",
			new XElement("Effect", new XAttribute("type", "planarstate"), new XElement("PlanarData", new XElement("Plane", 82000))));
		Spell(82005, "arm.spell.draw_into_flesh", "Draw Into Flesh", new XElement("Effect", new XAttribute("type", "removeplanarstate")));
	}
}
