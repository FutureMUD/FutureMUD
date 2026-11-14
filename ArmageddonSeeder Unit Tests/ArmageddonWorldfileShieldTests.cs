#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.Models;
using GameItemProto = MudSharp.Models.GameItemProto;
using GameItemComponentProto = MudSharp.Models.GameItemComponentProto;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfileShieldTests
{
	[TestMethod]
	public void OpeningShopStock_Shield_SerializesAnUnwieldedNativeComponent()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var source = new ArmWorldItem(new("fixture", 1), 7, -1, "shield", "a shield", "", "",
			[9, 0, 1, 0, 0], [0, 0, 0, 0, 0, 0], [1, 1, 0], [], [], [], []);
		var writer = new ArmageddonWorldfileWriter(db, new ArmageddonWorldfilePackage { Items = [source] });
		var shield = new GameItemComponentProto { Id = 8001, Type = "Shield", Definition = "<Definition><ShieldType>1</ShieldType></Definition>" };
		var holdable = new GameItemComponentProto { Id = 8002, Type = "Holdable", Definition = "<Definition/>" };
		db.GameItemComponentProtos.AddRange(shield, holdable);
		var proto = new GameItemProto { Id = 8003, Name = "fixture shield", RevisionNumber = 0, MaterialId = 1 };
		foreach (var component in new[] { shield, holdable })
			proto.GameItemProtosGameItemComponentProtos.Add(new()
			{
				GameItemProto = proto, GameItemProtoId = proto.Id,
				GameItemComponent = component, GameItemComponentProtoId = component.Id
			});
		db.GameItemProtos.Add(proto);
		ArmageddonGuardVendorSeederTests.Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems")
			.Add(proto.Id, (source, source.Values));
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		var planType = typeof(ArmageddonWorldfileWriter).GetNestedType("ShopStockPlan", BindingFlags.NonPublic)!;
		var children = Activator.CreateInstance(typeof(List<>).MakeGenericType(planType));
		var plan = Activator.CreateInstance(planType, proto, source.Vnum, children);
		var stock = (MudSharp.Models.GameItem)typeof(ArmageddonWorldfileWriter).GetMethod("SeedShopStockItem", flags)!
			.Invoke(writer, [plan, new Room { Id = 8004 }, new Shop { Id = 8005 }, null])!;
		var state = stock.GameItemComponents.Single(x => x.GameItemComponentProtoId == shield.Id);
		Assert.AreEqual("Definition", XElement.Parse(state.Definition).Name.LocalName);
		Assert.AreEqual(0L, (long?)XElement.Parse(state.Definition).Element("Wielded"));
		Assert.AreEqual(2, stock.GameItemComponents.Count);
	}

	[TestMethod]
	public void ShieldClassification_UsesAuthoredObjectsRatherThanArmourPointsOrIncidentalWords()
	{
		ArmWorldItem Item(int type, string name) => new(new("fixture", 1), 1, -1, name, name, "", "", [type, 0, 1, 0, 0], [0, 0, 0, 0, 0, 0], [1, 1, 0], [], [], [], []);
		Assert.AreEqual("Buckler", ArmageddonWorldfileWriter.SourceShieldType(Item(9, "a small buckler")));
		Assert.AreEqual("Kite", ArmageddonWorldfileWriter.SourceShieldType(Item(9, "a kite shield")));
		Assert.AreEqual("Tower", ArmageddonWorldfileWriter.SourceShieldType(Item(9, "a tower shield")));
		Assert.AreEqual("Round", ArmageddonWorldfileWriter.SourceShieldType(Item(9, "a heather shield")));
		Assert.IsNull(ArmageddonWorldfileWriter.SourceShieldType(Item(9, "a cloud")));
		Assert.IsNull(ArmageddonWorldfileWriter.SourceShieldType(Item(9, "a leather breastplate")));
		Assert.IsNull(ArmageddonWorldfileWriter.SourceShieldType(Item(19, "shield-wall oysters")));
		Assert.IsNull(ArmageddonWorldfileWriter.SourceShieldType(Item(12, "Armor and Shields")));
	}

	[TestMethod]
	public void PrivateCorpus_ShieldsAndEffectiveVariantsReuseNativeComponentsAndReadyInHandResets()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var source = ArmageddonWorldfileTestData.Load().ForInstallation();
		var writer = new ArmageddonWorldfileWriter(db, source);
		var shields = source.Items.Where(x => ArmageddonWorldfileWriter.SourceShieldType(x) is not null).ToArray();
		Assert.AreEqual(40, shields.Length);
		InstallNativeComponents(db);
		var flags = BindingFlags.Instance | BindingFlags.NonPublic;
		void Set(string name, object value) => typeof(ArmageddonWorldfileWriter).GetField(name, flags)!.SetValue(writer, value);
		Set("_holdableComponentId", 9001L); Set("_humanBodyId", 1L); Set("_trueProgId", 1L);
		var locations = (IReadOnlyDictionary<int, string[]>)typeof(ArmageddonWorldfileWriter).GetField("WearBitLocations", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var id = 100L;
		foreach (var name in locations.Values.SelectMany(x => x).Distinct()) db.BodypartProtos.Add(new BodypartProto { Id = id++, BodyId = 1, Name = name });
		var materials = ArmageddonGuardVendorSeederTests.Field<Dictionary<int, Material>>(writer, "_materials");
		foreach (var material in shields.Select(x => x.Material).Distinct()) materials[material] = db.Materials.First();
		db.SaveChanges();
		var componentCount = db.GameItemComponentProtos.Count(x => x.Type == "Shield");
		var traitCount = db.TraitDefinitions.Count();
		var create = typeof(ArmageddonWorldfileWriter).GetMethod("CreateItemProto", flags)!;
		var resetItem = typeof(ArmageddonWorldfileWriter).GetMethod("GetResetItem", flags)!;
		var items = ArmageddonGuardVendorSeederTests.Field<Dictionary<int, GameItemProto>>(writer, "_items");
		foreach (var item in shields)
		{
			var proto = (GameItemProto)create.Invoke(writer, [item, item.Values, "base", null, 0])!;
			items.Add(item.Vnum, proto);
			Assert.IsTrue(ArmageddonWorldfileWriter.HasShieldComponent(proto), "Immediate component identity " + item.Vnum);
			var values = item.Values.ToArray(); values[0] = values[0] == 0 ? 1 : 0;
			var variant = (GameItemProto)resetItem.Invoke(writer, [item, values, "shield-test", null, false, 0])!;
			foreach (var candidate in new[] { proto, variant })
			{
				var component = candidate.GameItemProtosGameItemComponentProtos.Single(x => x.GameItemComponent.Type == "Shield").GameItemComponent;
				Assert.AreEqual("Shield_" + ArmageddonWorldfileWriter.SourceShieldType(item), component.Name);
				Assert.IsFalse(candidate.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemComponent.Type == "Armour"), "No passive worn-armour substitute on a shield.");
				Assert.AreEqual(item.ShortDescription, candidate.ShortDescription);
			}
		}
		db.SaveChanges();
		Assert.AreEqual(componentCount, db.GameItemComponentProtos.Count(x => x.Type == "Shield"), "Reuse installed components rather than duplicating native shield definitions.");
		Assert.AreEqual(traitCount, db.TraitDefinitions.Count(), "No invented Shield Use skill or training.");
		var handResets = source.Resets.Where(x => x.Opcode == "E" && x.ReferenceIssues.Length == 0 && x.Arguments[3] is 16 or 17 or 19 && items.ContainsKey(x.Arguments[1])).ToArray();
		Assert.AreEqual(65, handResets.Length);
		foreach (var reset in handResets)
		{
			var proto = items[reset.Arguments[1]];
			Assert.AreEqual(OutfitTemplateItemPlacement.Wielded, ArmageddonWorldfileWriter.ResetShieldPlacement(reset, proto, OutfitTemplateItemPlacement.Inventory));
			Assert.AreEqual(OutfitTemplateItemPlacement.Inventory, ArmageddonWorldfileWriter.ResetShieldPlacement(reset with { Opcode = "G" }, proto, OutfitTemplateItemPlacement.Inventory));
			var back = reset.Arguments.ToArray(); back[3] = 9;
			Assert.AreEqual(OutfitTemplateItemPlacement.Worn, ArmageddonWorldfileWriter.ResetShieldPlacement(reset with { Arguments = back }, proto, OutfitTemplateItemPlacement.Worn));
		}
		Assert.IsNull(ArmageddonWorldfileWriter.SourceShieldType(source.Items.Single(x => x.Vnum == 30602)), "The cloud remains ordinary source armour.");
	}

	[TestMethod]
	public void PrivateCorpus_ShieldPreflightRequiresApprovedNativeBindings()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		var source = ArmageddonWorldfileTestData.Load().ForInstallation();
		Assert.AreEqual(4, ArmageddonWorldfileWriter.ShieldBindingErrors(db, source).Count());
		InstallNativeComponents(db);
		Assert.AreEqual(0, ArmageddonWorldfileWriter.ShieldBindingErrors(db, source).Count());
		db.GameItemComponentProtos.Single(x => x.Name == "Shield_Round").EditableItem.RevisionStatus = (int)RevisionStatus.PendingRevision;
		db.SaveChanges();
		Assert.AreEqual(1, ArmageddonWorldfileWriter.ShieldBindingErrors(db, source).Count());
	}

	private static void InstallNativeComponents(MudSharp.Database.FuturemudDatabaseContext db)
	{
		db.GameItemComponentProtos.Add(new() { Id = 9001, Type = "Holdable", Definition = "<Definition/>" });
		var id = 9010L;
		foreach (var name in new[] { "Buckler", "Kite", "Round", "Tower", "Heater" })
		{
			db.ShieldTypes.Add(new ShieldType { Id = id, Name = name, BlockTraitId = 2, StaminaPerBlock = 8 });
			db.GameItemComponentProtos.Add(new() { Id = id, Name = "Shield_" + name, Type = "Shield",
				Definition = $"<Definition><ShieldType>{id}</ShieldType></Definition>", EditableItem = new() { RevisionStatus = (int)RevisionStatus.Current } });
			id++;
		}
		db.SaveChanges();
	}
}
