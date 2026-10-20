#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using FutureProg = MudSharp.Models.FutureProg;
using ChargenScreenStoryboard = MudSharp.Models.ChargenScreenStoryboard;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonHeritageTests
{
	private static FuturemudDatabaseContext Fixture()
	{
		var db = ArmageddonSettingPackageTests.Fixture();
		new SkillPackageSeeder().SeedData(db, ArmageddonSettingPackageTests.Answers);
		foreach (var prog in db.FutureProgs.Where(x => x.FunctionName == "AlwaysTrue" || x.FunctionName == "AlwaysFalse"))
		{
			prog.FunctionText = prog.FunctionName == "AlwaysTrue" ? "return true" : "return false";
			prog.ReturnType = (long)ProgVariableTypes.Boolean; prog.AcceptsAnyParameters = true;
		}
		db.Calendars.Add(new Calendar { Id = 100, Definition = "", FeedClockId = 100 });
		db.BodyProtos.Add(new BodyProto { Id = 100, Name = "Organic Humanoid" });
		db.HealthStrategies.Add(new HealthStrategy { Id = 100, Name = "Living" });
		db.PopulationBloodModels.Add(new PopulationBloodModel { Id = 100, Name = "Human blood" });
		db.HeightWeightModels.AddRange(Enumerable.Range(100, 4).Select(x => new HeightWeightModel { Id = x, Name = "Height " + x }));
		db.Races.Add(new Race { Id = 100, Name = "Organic Humanoid", BaseBodyId = 100 });
		db.Races.Add(new Race { Id = 101, Name = "Human", ParentRaceId = 100, BaseBodyId = 100, DefaultHealthStrategyId = 100,
			AvailabilityProgId = 1, AllowedGenders = "1 2", DefaultHeightWeightModelMaleId = 100, DefaultHeightWeightModelFemaleId = 101,
			DefaultHeightWeightModelNeuterId = 102, DefaultHeightWeightModelNonBinaryId = 103, SweatRateInLitresPerMinute = 0.8, HungerRate = 1, ThirstRate = 1 });
		db.Cultures.Add(new Culture { Id = 100, Name = "Admin", PrimaryCalendarId = 100 });
		db.Ethnicities.Add(new Ethnicity { Id = 100, Name = "Admin", ParentRaceId = 101, PopulationBloodModelId = 100 });
		db.CharacteristicDefinitions.Add(new CharacteristicDefinition { Id = 100, Name = "Shape" });
		db.CharacteristicProfiles.Add(new CharacteristicProfile { Id = 100, Name = "All shapes", TargetDefinitionId = 100 });
		db.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics { EthnicityId = 100, CharacteristicDefinitionId = 100, CharacteristicProfileId = 100 });
		var original = new FutureProg { FunctionName = "CustomFreeSkills", FunctionText = "// Builder delegate preserved\nvar skills as trait collection\nreturn @skills", ReturnType = (long)(ProgVariableTypes.Trait | ProgVariableTypes.Collection) };
		db.FutureProgs.Add(original); db.SaveChanges();
		db.FutureProgsParameters.Add(new FutureProgsParameter { FutureProgId = original.Id, ParameterIndex = 0, ParameterName = "ch", ParameterType = (long)ProgVariableTypes.Toon });
		db.ChargenScreenStoryboards.Add(new ChargenScreenStoryboard { Id = 100, ChargenStage = (int)ChargenStage.SelectSkills, ChargenType = "SkillPicker",
			StageDefinition = "<Screen><FreeSkillsProg>CustomFreeSkills</FreeSkillsProg><Blurb>Preserve this builder text</Blurb></Screen>" });
		db.SaveChanges(); return db;
	}

	[TestMethod]
	public void ApprovedMatrix_PreservesExactlyOneNativeAndExplicitSecondaryValues()
	{
		Assert.AreEqual(7, ArmageddonHeritage.Profiles.Count);
		Assert.AreEqual(5, ArmageddonHeritage.Cultures.Count);
		var compatible = 0;
		foreach (var profile in ArmageddonHeritage.Profiles)
		foreach (var culture in ArmageddonHeritage.Cultures.Where(x => ArmageddonHeritage.Compatible(profile.Key, x)))
		{
			compatible++;
			var grants = ArmageddonHeritage.Grants(profile, culture);
			Assert.AreEqual(1, grants.Count(x => x.Value == 200));
			if (profile.Key == "city_elf" || profile.Key == "dwarf") Assert.AreEqual(100, grants["sirihish"]);
			if (profile.Key == "half_elf") Assert.AreEqual(60, grants["allundean"]);
			if (profile.Key == "mul") Assert.AreEqual(100, grants["mirukkim"]);
			if (culture.Key == "nomadic") { Assert.AreEqual(200, grants["bendune"]); Assert.IsFalse(grants.ContainsKey("sirihish")); }
		}
		Assert.AreEqual(20, compatible);
	}

	[TestMethod]
	public void HeritageInstall_CompilesGrantGraphKeepsProfilesInactiveAndPreservesRerunEdits()
	{
		using var db = Fixture();
		var messages = new List<string>();
		var original = db.FutureProgs.Single(x => x.FunctionName == "CustomFreeSkills");
		var originalBody = original.FunctionText;
		ArmageddonHeritage.Install(db, messages);
		Assert.AreEqual(8, db.Races.Count(x => x.Name.StartsWith("Armageddon ")));
		Assert.IsTrue(db.Races.Where(x => x.Name.StartsWith("Armageddon ")).All(x => x.AvailabilityProgId == 2));
		Assert.AreEqual(7, db.Ethnicities.Count(x => x.Name.StartsWith("Armageddon ")));
		Assert.AreEqual(5, db.Cultures.Count(x => x.Name.StartsWith("Armageddon ")));
		Assert.AreEqual(6, db.Languages.Count());
		Assert.AreEqual(15, db.Accents.Count(x => x.Language.Name != "Admin Speech"));
		Assert.AreEqual(0, db.RandomNameProfiles.Count());
		Assert.AreEqual(0, db.Scripts.Count());
		Assert.AreEqual(originalBody, original.FunctionText);
		var storyboard = db.ChargenScreenStoryboards.Single();
		var definition = XElement.Parse(storyboard.StageDefinition);
		Assert.AreEqual("Preserve this builder text", definition.Element("Blurb")!.Value);
		var wrapper = db.FutureProgs.Single(x => x.Id == long.Parse(definition.Element("FreeSkillsProg")!.Value));
		StringAssert.Contains(wrapper.FunctionText, "@CustomFreeSkills(@ch)");
		var dwarf = db.Races.Single(x => x.Name == "Armageddon Dwarf");
		Assert.AreEqual(100L, dwarf.DefaultHeightWeightModelMaleId, "Copy mapped public fields as well as CLR properties.");
		Assert.AreEqual(103L, dwarf.DefaultHeightWeightModelNonBinaryId);
		var elfParent = db.Races.Single(x => x.Name == "Armageddon Elf");
		Assert.AreEqual(elfParent.Id, db.Races.Single(x => x.Name == "Armageddon City Elf").ParentRaceId);
		var dwarfEthnicity = db.Ethnicities.Single(x => x.Name == "Armageddon Dwarf");
		Assert.AreEqual(dwarf.Id, dwarfEthnicity.ParentRaceId);
		Assert.AreEqual(db.Languages.Single(x => x.Name == "Mirukkim").Id, dwarfEthnicity.NativeLanguageId);
		Assert.AreEqual(5, db.EthnicitiesNameCultures.Count(x => x.EthnicityId == dwarfEthnicity.Id));
		Assert.AreEqual(1, db.EthnicitiesCharacteristics.Count(x => x.EthnicityId == dwarfEthnicity.Id));
		var ids = db.SeederManagedRecords.ToDictionary(x => x.StableKey, x => x.LogicalId);
		dwarf.Name = "Builder Dwarf"; dwarf.SweatRateInLitresPerMinute = 0.01;
		db.Races.Single(x => x.Name == "Human").SweatRateInLitresPerMinute = 99; // Bootstrap changes must not become new stock defaults.
		var culture = db.Cultures.Single(x => x.Name == "Armageddon Allanaki");
		culture.PersonWordMale = "Builder person";
		db.CulturesNameCultures.Remove(db.CulturesNameCultures.Single(x => x.CultureId == culture.Id && x.Gender == 0));
		db.SaveChanges();
		ArmageddonHeritage.Install(db, messages);
		Assert.AreEqual("Builder Dwarf", dwarf.Name);
		Assert.AreEqual(0.01, dwarf.SweatRateInLitresPerMinute);
		Assert.AreEqual(0.8, db.Races.Single(x => x.Name == "Armageddon Mul").SweatRateInLitresPerMinute);
		Assert.AreEqual("Builder person", culture.PersonWordMale);
		Assert.AreEqual(4, db.CulturesNameCultures.Count(x => x.CultureId == culture.Id));
		Assert.AreEqual(originalBody, original.FunctionText);
		Assert.AreEqual(ids.Count, db.SeederManagedRecords.Count());
		Assert.IsTrue(db.SeederManagedRecords.All(x => ids[x.StableKey] == x.LogicalId));
		Assert.IsTrue(messages.Any(x => x.Contains("builder edit")));
	}

	[TestMethod]
	public void InactiveLifecycleScaffolds_PersistInitialAgesAndPreserveBuilderChangesAcrossReruns()
	{
		using var db = Fixture();
		var human = db.Races.Single(x => x.Name == "Human");
		human.ChildAge = 3;
		human.YouthAge = 10;
		human.YoungAdultAge = 16;
		human.AdultAge = 25;
		human.ElderAge = 50;
		human.VenerableAge = 75;
		db.SaveChanges();
		var messages = new List<string>();
		ArmageddonHeritage.Install(db, messages);
		var ids = db.Races.Where(x => x.Name.StartsWith("Armageddon ")).Select(x => x.Id).ToArray();
		var dwarfId = db.Races.Single(x => x.Name == "Armageddon Dwarf").Id;
		var baseline = new[] { 3, 10, 16, 25, 50, 75 };
		static int[] Ages(Race race) =>
			[race.ChildAge, race.YouthAge, race.YoungAdultAge, race.AdultAge, race.ElderAge, race.VenerableAge];

		// Reload persisted rows rather than checking the installer's tracked objects.
		db.ChangeTracker.Clear();
		Assert.AreEqual(8, ids.Length);
		foreach (var race in db.Races.Where(x => ids.Contains(x.Id)))
		{
			CollectionAssert.AreEqual(baseline, Ages(race));
			Assert.AreEqual(2L, race.AvailabilityProgId);
		}

		// These are deliberately authored test overrides, not proposed Armageddon ages.
		var dwarf = db.Races.Single(x => x.Id == dwarfId);
		dwarf.ChildAge = 5;
		dwarf.YouthAge = 12;
		dwarf.YoungAdultAge = 21;
		dwarf.AdultAge = 31;
		dwarf.ElderAge = 62;
		dwarf.VenerableAge = 82;
		db.Races.Single(x => x.Name == "Human").YoungAdultAge = 18;
		db.SaveChanges();
		for (var attempt = 0; attempt < 2; attempt++)
		{
			db.ChangeTracker.Clear();
			ArmageddonHeritage.Install(db, messages);
			db.ChangeTracker.Clear();
			foreach (var race in db.Races.Where(x => ids.Contains(x.Id)))
			{
				CollectionAssert.AreEqual(race.Id == dwarfId ? new[] { 5, 12, 21, 31, 62, 82 } : baseline,
					Ages(race), $"Lifecycle override/baseline changed for {race.Name} on rerun {attempt}.");
				Assert.AreEqual(2L, race.AvailabilityProgId);
			}
			Assert.AreEqual(18, db.Races.Single(x => x.Name == "Human").YoungAdultAge);
		}
		Assert.IsTrue(messages.Any(x => x.Contains("builder edit")));
	}

	[TestMethod]
	public void MissingScaffoldModel_BlocksBeforeHeritageWrites()
	{
		using var db = Fixture();
		db.Races.Single(x => x.Name == "Human").DefaultHeightWeightModelNonBinaryId = 999;
		db.SaveChanges();
		var before = db.SeederManagedRecords.Count();
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonHeritage.Install(db, new List<string>()));
		Assert.AreEqual(before, db.SeederManagedRecords.Count());
		Assert.AreEqual(1, db.Languages.Count());
	}

	[TestMethod]
	public void HeritageNameCollision_BlocksBeforeAnyHeritageWrite()
	{
		using var db = Fixture();
		db.Cultures.Add(new Culture { Name = "Armageddon Tuluki" }); db.SaveChanges();
		var before = db.SeederManagedRecords.Count();
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonHeritage.Install(db, new List<string>()));
		Assert.AreEqual(before, db.SeederManagedRecords.Count());
		Assert.AreEqual(0, db.Races.Count(x => x.Name.StartsWith("Armageddon ")));
		Assert.AreEqual(1, db.Languages.Count());
	}
}
