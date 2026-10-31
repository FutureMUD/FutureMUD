#nullable enable
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Db = MudSharp.Models;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonItemAffectsTests
{
	[TestMethod]
	public void OrdinaryAffects_SumDuplicateSlotsAndPreserveNegativeValues_AndMapApprovedConversions()
	{
		var bonuses = ArmageddonWorldfileWriter.ResolveItemAffects(
			[[1, 2], [1, 3], [2, -4], [12, 10], [14, -5], [348, 30], [13, 20], [18, 8], [25, 5], [26, 6], [27, 7], [28, 8], [29, -2], [17, 15], [24, 4], [0, 0]],
			location => location + 1000, () => 99);
		Assert.AreEqual(14, bonuses.Count);
		Assert.AreEqual(5.0, bonuses.Single(x => x.TargetId == 1001).Bonus);
		Assert.AreEqual(-4.0, bonuses.Single(x => x.TargetId == 1002).Bonus);
		Assert.AreEqual(30.0, bonuses.Single(x => x.TargetId == 1348).Bonus);
		Assert.AreEqual(10.0, bonuses.Single(x => x.Kind == EquippedItemBonusKind.MagicResource && x.TargetId == 99).Bonus);
		Assert.AreEqual(20.0, bonuses.Single(x => x.Kind == EquippedItemBonusKind.Health).Bonus);
		Assert.AreEqual(8.0, bonuses.Single(x => x.Kind == EquippedItemBonusKind.CombatAttack).Bonus);
		Assert.AreEqual(-2.0, bonuses.Single(x => x.Kind == EquippedItemBonusKind.CombatDefence).Bonus);
		Assert.AreEqual(1.5, bonuses.Single(x => x.Kind == EquippedItemBonusKind.PhysicalAbsorption).Bonus);
		Assert.AreEqual(4.0, bonuses.Single(x => x.Kind == EquippedItemBonusKind.SpellResistance).Bonus);
		foreach (var (id, amount) in new[] { (1025L, 5.0), (1026L, 6.0), (1027L, 7.0), (1028L, 8.0) })
			Assert.AreEqual(amount, bonuses.Single(x => x.TargetId == id).Bonus);
		Assert.AreEqual(-5.0, bonuses.Single(x => x.Kind == EquippedItemBonusKind.Stamina).Bonus);
	}

	[TestMethod]
	public void ItemAffects_InstallOnEffectiveVariantsAndPersistedStock()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var source = archive.Items.First(x => x.Affects.Any(a => a[0] == 14));
		var writer = new ArmageddonWorldfileWriter(db, archive);
		var effective = (Dictionary<long, (ArmWorldItem Source, int[] Values)>)typeof(ArmageddonWorldfileWriter)
			.GetField("_effectiveItems", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
		var profiles = (Dictionary<int, Db.WearProfile[]>)typeof(ArmageddonWorldfileWriter)
			.GetField("_wearProfiles", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
		profiles.Add(1 << 9, [new Db.WearProfile { Id = 123 }]);
		// Use the source movement modifier while isolating this installation boundary from unrelated attribute dependencies.
		source = source with { Affects = source.Affects.Where(a => a[0] == 14).ToArray() };
		foreach (var id in new long[] { 990001, 990002 })
		{
			db.GameItemProtos.Add(new Db.GameItemProto { Id = id, Name = "Affect fixture" });
			db.GameItems.Add(new Db.GameItem { Id = id, GameItemProtoId = id, EffectData = "<Effects/>" });
			effective.Add(id, (source, source.Values));
		}
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallItemAffects", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		foreach (var id in effective.Keys)
		{
			var link = db.GameItemProtos.Local.Single(x => x.Id == id).GameItemProtosGameItemComponentProtos.Single();
			var component = db.GameItemComponentProtos.Local.Single(x => x.Id == link.GameItemComponentProtoId);
			Assert.AreEqual("Equipment Bonuses", component.Type);
			var xml = XElement.Parse(component.Definition);
			Assert.AreEqual(123L, (long)xml.Element("ExcludedProfiles")!.Element("Profile")!);
			Assert.AreEqual((double)source.Affects.Sum(x => x[1]), (double)xml.Element("Bonus")!.Attribute("value")!);
			Assert.AreEqual(1, db.GameItems.Local.Single(x => x.Id == id).GameItemComponents.Count);
		}
	}
}
