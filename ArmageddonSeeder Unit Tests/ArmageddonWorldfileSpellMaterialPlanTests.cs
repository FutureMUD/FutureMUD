#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfileSpellMaterialPlanTests
{
	[TestMethod]
	public void SourceMaterialPlans_PreserveRanksCostsExemptionsAndOptionalReturnPolicy()
	{
		using var db = CreateFixture(out _);
		Assert.AreEqual(-1, (int)Actions("join_traveller").Single().Element("GradeRank")!.Attribute("offset")!);
		Assert.AreEqual(-1, (int)Actions("draw_traveller").Single().Element("GradeRank")!.Attribute("offset")!);
		Assert.AreEqual("explicit", (string?)Actions("consuming_flame").Single().Element("GradeRank")!.Attribute("selection"));
		Assert.AreEqual(7, Actions("consuming_flame").Single().Element("GradeRank")!.Elements("Rank").Count());
		Assert.AreEqual(7, Actions("sheltering_veil").Single().Element("GradeRank")!.Elements("Rank").Count());
		Assert.AreEqual(2, Actions("kindle_enchantment").Count());
		Assert.IsTrue(Actions("kindle_enchantment").All(x => (int?)x.Attribute("grade") == 7));
		Assert.IsTrue((bool)Actions("return_tether").Single().Attribute("castoptional")!);
		Assert.AreEqual("return.duration", Xml("return_tether").Element("Effects")!.Element("Effect")!.Element("OptionalDurationComponent")!.Value);
		Assert.AreEqual(7, Xml("exchange_places").Element("Effects")!.Element("Effect")!.Element("RetainedRanks")!.Elements().Count());
		var mount = db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileMaterialMount").FunctionText;
		Assert.IsTrue(mount.IndexOf("if (@actor.npc)", StringComparison.Ordinal) < mount.IndexOf("torole(9991)", StringComparison.Ordinal));
		var ordinary = db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileMaterialOrdinary").FunctionText;
		StringAssert.Contains(ordinary, "race.id == 920001"); StringAssert.Contains(ordinary, "race.id == 920003");
		Assert.IsFalse(ordinary.Contains("920002", StringComparison.Ordinal), "Sub-Elemental is not the source's Elemental exemption.");
		Assert.AreEqual(0, db.GameItems.Count()); Assert.AreEqual(0, db.GameItemProtos.Count()); Assert.AreEqual(0, db.Crafts.Count());
		using var compiler = new MudSharp.Framework.OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		foreach (var prog in db.FutureProgs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfileMaterial")).ToArray()) compiler.Compile(prog.Id);
		return;
		XElement Xml(string key) => XElement.Parse(db.MagicSpells.Single(x => x.Name == key).Definition);
		XElement[] Actions(string key) => Xml(key).Element("Plan")!.Descendants("Action").ToArray();
	}
	internal static MudSharp.Database.FuturemudDatabaseContext CreateFixture(out ArmageddonWorldfileWriter writer)
	{
		var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var source = new ArmageddonWorldfilePackage { Races =
		[
			new(new("fixture", 1), 1, "Elemental", "e", [], [], "", [], []),
			new(new("fixture", 2), 2, "Sub-Elemental", "s", [], [], "", [], []),
			new(new("fixture", 3), 3, "Fiend", "f", [], [], "", [], [])
		] };
		writer = new ArmageddonWorldfileWriter(db, source);
		var races = (System.Collections.Generic.Dictionary<int, Db.Race>)typeof(ArmageddonWorldfileWriter).GetField("_races", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(writer)!;
		foreach (var race in source.Races) races.Add(race.Index, new Db.Race { Id = 920000 + race.Index, Name = race.Name });
		long next = 800000;
		foreach (var sphere in new[] { "abjuration", "alteration", "conjuration", "enchantment", "illusion", "invocation", "divination", "necromancy", "clerical", "nature", "teleportation", "creation" })
		{
			long? parent = null;
			for (var rank = 0; rank <= 4; rank++)
			{
				var tag = new Db.Tag { Id = next++, Name = sphere + rank, ParentId = sphere is "alteration" or "divination" or "clerical" or "teleportation" ? parent : null };
				db.Tags.Add(tag); Identity($"arm.material.{sphere}.rank{rank}.tag", nameof(Db.Tag), tag.Id); parent = tag.Id;
			}
		}
		Identity("arm.class.earth", nameof(Db.ChargenRole), 9991);
		Identity("arm.class.southern_templar", nameof(Db.ChargenRole), 9992);
		var keys = new[] { "compelling_regard", "consuming_flame", "shadow_passage", "haunting_shape", "storm_spear", "storm_locus",
			"wind_courier", "earth_mount", "open_threshold", "borrow_the_dead", "join_traveller", "draw_traveller", "walking_shadow",
			"seeking_wind", "anchor_rune", "air_guardian", "flame_knife", "spark_lash", "shadow_blade", "sand_knife", "mist_shield",
			"wind_shield", "kindle_enchantment", "call_outsider", "see_the_unbodied", "sheltering_veil", "exchange_places", "return_tether" };
		foreach (var key in keys)
		{
			var spell = new Db.MagicSpell { Id = next++, Name = key, Definition = $"<Definition><Plan><Phase><Action state='consumed' tag='1' quantity='1'/></Phase></Plan><Effects><Effect type='{(key == "exchange_places" ? "exchangeplaces" : key == "return_tether" ? "returntether" : "noop")}'><RetainedRanks/></Effect></Effects></Definition>" };
			db.MagicSpells.Add(spell); Identity("arm.spell." + key, nameof(Db.MagicSpell), spell.Id);
		}
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallSourceSpellMaterialPlans", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(writer, null);
		db.SaveChanges();
		return db;
		void Identity(string key, string type, long id) => db.SeederManagedRecords.Add(new Db.SeederManagedRecord { Seeder = "fixture", Module = "materials", StableKey = key, EntityType = type, LogicalId = id });
	}

}
