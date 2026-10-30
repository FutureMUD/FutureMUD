#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonNarrativeSeederTests
{
	private static void Invoke(ArmageddonWorldfileWriter writer, string name) =>
		typeof(ArmageddonWorldfileWriter).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
	private static T Field<T>(ArmageddonWorldfileWriter writer, string name) =>
		(T)typeof(ArmageddonWorldfileWriter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;

	[TestMethod]
	public void Narrative_UsesClanGatedCompleteLoreAndThreeLanguageReadingsWithoutAutomaticUntalGrants()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		foreach (var (id, name) in new[] { (10L, "Allundean"), (11L, "Bendune") })
		{
			db.TraitDefinitions.Add(new TraitDefinition { Id = id, Name = name, Type = 0, Alias = name.ToLowerInvariant() });
			db.Languages.Add(new Language { Id = id, Name = name, LinkedTraitId = id, DifficultyModel = 1 });
			db.Accents.Add(new Accent { Id = id, Name = name, LanguageId = id });
		}
		db.SaveChanges();
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var source = new ArmageddonWorldfilePackage { Rooms = [archive.Rooms.Single(x => x.Vnum == 70500)],
			Npcs = archive.Npcs.Where(x => x.Vnum is 21012 or 21229).ToArray() };
		var writer = new ArmageddonWorldfileWriter(db, source);
		var clan = new Clan { Id = 25, Name = "Blackwing" }; db.Clans.Add(clan);
		Field<Dictionary<int, Clan>>(writer, "_clans").Add(25, clan);
		var room = new Room { Id = 10000, UniqueName = "Vnum70500", EffectData = "<Effects/>" }; db.Rooms.Add(room);
		Field<Dictionary<int, Room>>(writer, "_rooms").Add(70500, room);
		foreach (var npc in source.Npcs)
		{
			var template = new NpcTemplate { Id = npc.Vnum, RevisionNumber = 0, Definition = "<Definition><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>" };
			db.NpcTemplates.Add(template); Field<Dictionary<int, NpcTemplate>>(writer, "_npcs").Add(npc.Vnum, template);
		}
		Invoke(writer, "InstallLegacyUntal"); Invoke(writer, "InstallNarrativeSpecials"); db.SaveChanges();
		var untal = db.Languages.Single(x => x.Name == "Untal");
		Assert.AreEqual(1, db.Accents.Count(x => x.LanguageId == untal.Id));
		var untalSkill = db.TraitDefinitions.Single(x => x.Id == untal.LinkedTraitId);
		Assert.AreEqual("Untal", untalSkill.Name);
		Assert.AreEqual(ArmageddonInstallation.Read(db)!.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg], untalSkill.AvailabilityProgId);
		Assert.AreEqual(untalSkill.AvailabilityProgId, db.Accents.Single(x => x.LanguageId == untal.Id).ChargenAvailabilityProgId);
		Assert.AreEqual(0, db.CharacterTraits.Count(x => x.TraitDefinitionId == untal.LinkedTraitId));
		Assert.AreEqual(0, db.ChargenRolesTraits.Count(x => x.TraitId == untal.LinkedTraitId));
		Assert.AreEqual(4, db.Hooks.Count()); Assert.AreEqual(3, db.HooksPerceivables.Count());
		var historian = db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileBlackwingHistorian");
		StringAssert.Contains(historian.FunctionText, "isclanmember(@actor, toclan(25))");
		StringAssert.Contains(historian.FunctionText, "targetcharacter");
		StringAssert.Contains(historian.FunctionText, "Fylas"); StringAssert.Contains(historian.FunctionText, "Dolsarius");
		var writing = db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileCaveWriting");
		StringAssert.Contains(writing.FunctionText, "@proficiencyUntal > 80");
		StringAssert.Contains(writing.FunctionText, "@proficiencyUntal > 50"); StringAssert.Contains(writing.FunctionText, "@proficiencyUntal > 0");
		using var privateText = ArmageddonWorldfilePackage.LoadSpecialText();
		foreach (var reading in privateText.RootElement.GetProperty("CaveReadings").EnumerateArray())
		{
			var expected = reading.GetProperty("Full").GetString()!.Replace("\r", "").Replace("\n", "\\n");
			Assert.IsTrue(writing.FunctionText.Contains(expected), "Generated inscription must preserve the private reading.");
		}
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		foreach (var prog in db.FutureProgs.ToArray()) compiler.Compile(prog.Id);
	}

	[TestMethod]
	public void LegacyUntal_ReusesExistingNativeLanguageWithoutChangingItsConfiguration()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		db.TraitDefinitions.Add(new TraitDefinition { Id = 50, Name = "Custom Untal Skill", Alias = "customuntal", Type = 0 });
		db.Languages.Add(new Language { Id = 50, Name = "Untal", LinkedTraitId = 50, UnknownLanguageDescription = "a builder's tongue" });
		db.Accents.Add(new Accent { Id = 50, LanguageId = 50, Name = "Builder Untal" });
		db.SaveChanges();
		var writer = new ArmageddonWorldfileWriter(db, new ArmageddonWorldfilePackage());
		Invoke(writer, "InstallLegacyUntal"); db.SaveChanges();
		Assert.AreEqual(1, db.Languages.Count(x => x.Name == "Untal"));
		Assert.AreEqual(50L, db.Languages.Single(x => x.Name == "Untal").LinkedTraitId);
		Assert.AreEqual("a builder's tongue", db.Languages.Single(x => x.Name == "Untal").UnknownLanguageDescription);
		Assert.AreEqual("Builder Untal", db.Accents.Single(x => x.LanguageId == 50).Name);
		Assert.IsFalse(db.TraitDefinitions.Any(x => x.Name == "Untal"));
	}
}
