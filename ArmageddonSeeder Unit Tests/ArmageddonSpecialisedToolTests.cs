#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonSpecialisedToolTests
{
	[TestMethod]
	public void SpecialisedTools_MapEffectiveValuesAndPersistOpeningStock()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var writer = new ArmageddonWorldfileWriter(db, archive);
		var items = (Dictionary<int, Db.GameItemProto>)typeof(ArmageddonWorldfileWriter).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
		var effective = (Dictionary<long, (ArmWorldItem Source, int[] Values)>)typeof(ArmageddonWorldfileWriter).GetField("_effectiveItems", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
		foreach (var source in archive.Items.Where(x => x.ItemType == 7))
		{
			var proto = new Db.GameItemProto { Id = source.Vnum, Name = source.ShortDescription };
			items.Add(source.Vnum, proto); effective.Add(proto.Id, (source, source.Values));
			db.GameItemProtos.Add(proto);
			db.GameItems.Add(new Db.GameItem { Id = proto.Id, GameItemProtoId = proto.Id, EffectData = "<Effects/>" });
		}
		var variantSource = archive.Items.First(x => x.ItemType == 7 && x.Values[0] == 1);
		var variantValues = variantSource.Values.ToArray(); variantValues[1] = 9;
		db.GameItemProtos.Add(new Db.GameItemProto { Id = 990001, Name = "Reset climbing aid" });
		effective.Add(990001, (variantSource, variantValues));
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallSpecialisedTools", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		foreach (var (id, entry) in effective)
		{
			var component = db.GameItemProtos.Local.Single(x => x.Id == id).GameItemProtosGameItemComponentProtos.SingleOrDefault();
			if (entry.Values[0] is not (1 or 6)) { Assert.IsNull(component); continue; }
			Assert.IsNotNull(component);
			var definition = db.GameItemComponentProtos.Local.Single(x => x.Id == component.GameItemComponentProtoId);
			var xml = XElement.Parse(definition.Definition);
			Assert.AreEqual((double)entry.Values[1], (double)xml.Element("CheckBonus")!);
			if (entry.Values[0] == 1) Assert.IsTrue((bool)xml.Element("WorksWorn")!);
			else
			{
				Assert.IsTrue((bool)xml.Element("Breakable")!);
				Assert.IsFalse((bool)xml.Element("UsableForConfiguration")!);
				Assert.AreEqual(0, (int)xml.Element("DifficultyAdjustment")!);
			}
			if (id != 990001) Assert.AreEqual(1, db.GameItems.Local.Single(x => x.Id == id).GameItemComponents.Count);
		}
	}
}
