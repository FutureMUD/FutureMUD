#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using MudSharp.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonExplosiveTrapTests
{
	[TestMethod]
	public void ExplosiveTraps_SeedEffectiveMaterialsAndArmedContainers_CompileProgs()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var writer = new ArmageddonWorldfileWriter(db, archive);
		var effective = (Dictionary<long, (ArmWorldItem Source, int[] Values)>)typeof(ArmageddonWorldfileWriter).GetField("_effectiveItems", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
		db.TraitDefinitions.Add(new Db.TraitDefinition { Id = 990001, Name = "Trap" });
		db.SeederManagedRecords.Add(new Db.SeederManagedRecord { Seeder = "Fixture", StableKey = "arm.skill.trap", EntityType = nameof(Db.TraitDefinition), LogicalId = 990001 });
		foreach (var source in archive.Items.Where(x => x.ItemType == 7 || x.ItemType == 15))
		{
			var proto = new Db.GameItemProto { Id = source.Vnum, Name = source.ShortDescription };
			effective.Add(proto.Id, (source, source.Values));
			db.GameItemProtos.Add(proto);
			db.GameItems.Add(new Db.GameItem { Id = proto.Id, GameItemProtoId = proto.Id, EffectData = "<Effects/>" });
		}
		var powder = archive.Items.First(x => x.ItemType == 7 && x.Values[0] == 2);
		var values = powder.Values.ToArray(); values[1] = 11;
		db.GameItemProtos.Add(new Db.GameItemProto { Id = 990002, Name = "Reset powder" });
		effective.Add(990002, (powder, values));
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallExplosiveTraps", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		Assert.AreEqual(2, db.TrapTemplates.Count());
		foreach (var (id, entry) in effective)
		{
			var isPowder = entry.Source.ItemType == 7 && entry.Values[0] == 2;
			Assert.AreEqual(isPowder ? 1 : 0, db.GameItemProtos.Local.Single(x => x.Id == id).GameItemProtosTags.Count);
			if (id == 990002) continue;
			var isTrapped = entry.Source.ItemType == 15 && (entry.Values[1] & 16) != 0;
			Assert.AreEqual(isTrapped, XElement.Parse(db.GameItems.Local.Single(x => x.Id == id).EffectData).Elements("Effect").Any());
			Assert.AreEqual(isTrapped ? 1 : 0, db.GameItemProtos.Local.Single(x => x.Id == id).GameItemProtosOnLoadProgs.Count);
		}
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		foreach (var prog in db.FutureProgs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfile"))) compiler.Compile(prog.Id);
	}
}
