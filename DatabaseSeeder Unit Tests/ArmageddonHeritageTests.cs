#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
	internal static FuturemudDatabaseContext Fixture()
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
		db.ChargenScreenStoryboards.Add(new ChargenScreenStoryboard { Id = 101, ChargenStage = (int)ChargenStage.SelectName, ChargenType = "NamePicker",
			StageDefinition = "<Screen><AllowUnicodeNames>false</AllowUnicodeNames><BuilderNote>Retain</BuilderNote></Screen>" });
		foreach (var (stage, fields, id) in new[]
		{
			(ChargenStage.SelectBirthday, new[] { "MinimumAgeProg", "MaximumAgeProg" }, 102),
			(ChargenStage.SelectHeight, new[] { "MinimumHeightProg", "MaximumHeightProg" }, 103),
			(ChargenStage.SelectWeight, new[] { "MinimumWeightProg", "MaximumWeightProg" }, 104)
		})
		{
			foreach (var field in fields)
			{
				var prog = new FutureProg { FunctionName = "Custom" + field, FunctionText = "return 42", ReturnType = (long)ProgVariableTypes.Number };
				db.FutureProgs.Add(prog); db.SaveChanges();
				db.FutureProgsParameters.Add(new FutureProgsParameter { FutureProgId = prog.Id, ParameterIndex = 0, ParameterName = "ch", ParameterType = (long)ProgVariableTypes.Toon });
			}
			db.ChargenScreenStoryboards.Add(new ChargenScreenStoryboard { Id = id, ChargenStage = (int)stage,
				ChargenType = stage == ChargenStage.SelectBirthday ? "BirthdayPicker" : stage == ChargenStage.SelectHeight ? "HeightPicker" : "WeightPicker",
				StageDefinition = new XElement("Screen", new XElement("Blurb", "Preserve physical builder text"),
					new XElement("AgeSelectionBlurb", "Choose an age"), new XElement("DateSelectionBlurb", "Choose a date"),
					fields.Select(x => new XElement(x, "Custom" + x))).ToString() });
		}
		foreach (var (id, name, value) in new[] { (110, "Ears", "ordinary"), (111, "Frame", "ordinary"),
			(112, "Hair Style", "bald"), (113, "Facial Hair Style", "clean shave") })
		{
			db.CharacteristicDefinitions.Add(new CharacteristicDefinition { Id = id, Name = name });
			db.CharacteristicValues.Add(new CharacteristicValue { Id = id, DefinitionId = id, Name = value });
			db.CharacteristicProfiles.Add(new CharacteristicProfile { Id = id, Name = "Shared " + name, TargetDefinitionId = id, Type = "Standard", Definition = $"<Definition><Value>{id}</Value></Definition>" });
			db.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics { EthnicityId = 100, CharacteristicDefinitionId = id, CharacteristicProfileId = id });
		}
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
	public void HeritageInstall_CompilesGrantGraphEnablesRequestedProfilesAndPreservesRerunEdits()
	{
		using var db = Fixture();
		var messages = new List<string>();
		var original = db.FutureProgs.Single(x => x.FunctionName == "CustomFreeSkills");
		var originalBody = original.FunctionText;
		ArmageddonHeritage.Install(db, messages);
		Assert.AreEqual(8, db.Races.Count(x => x.Id != 100 && x.Id != 101));
		Assert.IsTrue(db.Races.Where(x => x.Id != 100 && x.Id != 101).All(x => x.AvailabilityProgId == ((x.Name == "Elf" || x.Name == "Half-Elf") ? 2 : 1)));
		Assert.AreEqual("Admin Human", db.Races.Single(x => x.Id == 101).Name);
		Assert.AreEqual(7, db.Ethnicities.Count(x => x.Name != "Admin"));
		Assert.AreEqual(5, db.Cultures.Count(x => x.Name != "Admin"));
		Assert.AreEqual(11, db.Languages.Count());
		Assert.AreEqual(30, db.Accents.Count(x => x.Language.Name != "Admin Speech"));
		Assert.AreEqual(75, db.RandomNameProfiles.Count());
		Assert.AreEqual(0, db.Scripts.Count());
		Assert.AreEqual(originalBody, original.FunctionText);
		var storyboard = db.ChargenScreenStoryboards.Single(x => x.ChargenStage == (int)ChargenStage.SelectSkills);
		var definition = XElement.Parse(storyboard.StageDefinition);
		Assert.AreEqual("Preserve this builder text", definition.Element("Blurb")!.Value);
		var wrapper = db.FutureProgs.Single(x => x.Id == long.Parse(definition.Element("FreeSkillsProg")!.Value));
		StringAssert.Contains(wrapper.FunctionText, "@CustomFreeSkills(@ch)");
		var dwarf = db.Races.Single(x => x.Name == "Dwarf");
		Assert.AreEqual(db.SeederManagedRecords.Single(x => x.StableKey == "arm.physical.dwarf.model").LogicalId, dwarf.DefaultHeightWeightModelMaleId);
		Assert.AreEqual(dwarf.DefaultHeightWeightModelMaleId, dwarf.DefaultHeightWeightModelNonBinaryId);
		var elfParent = db.Races.Single(x => x.Name == "Elf");
		Assert.AreEqual(elfParent.Id, db.Races.Single(x => x.Name == "City Elf").ParentRaceId);
		var dwarfEthnicity = db.Ethnicities.Single(x => x.Name == "Dwarf");
		Assert.AreEqual(dwarf.Id, dwarfEthnicity.ParentRaceId);
		Assert.AreEqual(db.Languages.Single(x => x.Name == "Mirukkim").Id, dwarfEthnicity.NativeLanguageId);
		Assert.AreEqual(0, db.EthnicitiesNameCultures.Count(x => x.EthnicityId == dwarfEthnicity.Id));
		Assert.AreEqual(5, db.EthnicitiesCharacteristics.Count(x => x.EthnicityId == dwarfEthnicity.Id));
		var ids = db.SeederManagedRecords.ToDictionary(x => x.StableKey, x => x.LogicalId);
		dwarf.Name = "Builder Dwarf"; dwarf.SweatRateInLitresPerMinute = 0.01;
		db.Races.Single(x => x.Id == 101).SweatRateInLitresPerMinute = 99; // Bootstrap changes must not become new stock defaults.
		var culture = db.Cultures.Single(x => x.Name == "Allanaki");
		culture.PersonWordMale = "Builder person";
		db.CulturesNameCultures.Remove(db.CulturesNameCultures.Single(x => x.CultureId == culture.Id && x.Gender == 0));
		db.SaveChanges();
		ArmageddonHeritage.Install(db, messages);
		Assert.AreEqual("Builder Dwarf", dwarf.Name);
		Assert.AreEqual(0.01, dwarf.SweatRateInLitresPerMinute);
		Assert.AreEqual(0.03, db.Races.Single(x => x.Name == "Mul").SweatRateInLitresPerMinute);
		Assert.AreEqual("Builder person", culture.PersonWordMale);
		Assert.AreEqual(4, db.CulturesNameCultures.Count(x => x.CultureId == culture.Id));
		Assert.AreEqual(originalBody, original.FunctionText);
		Assert.AreEqual(ids.Count, db.SeederManagedRecords.Count());
		Assert.IsTrue(db.SeederManagedRecords.All(x => ids[x.StableKey] == x.LogicalId));
		Assert.IsTrue(messages.Any(x => x.Contains("builder edit")));
	}

	[TestMethod]
	public void SweatDefaults_PersistApprovedRosterAndPreserveCustomZeroLiquidAndAvailability()
	{
		using var db = Fixture();
		var messages = new List<string>();
		var human = db.Races.Single(x => x.Id == 101);
		db.Liquids.AddRange(new Liquid { Id = 901, Name = "Test sweat" }, new Liquid { Id = 902, Name = "Custom sweat" });
		human.SweatLiquidId = 901;
		db.SaveChanges();
		var adminCultureCount = db.Cultures.Count(x => x.Name == "Admin");
		var characterCount = db.Characters.Count();
		ArmageddonHeritage.Install(db, messages);
		var expected = new Dictionary<string, double>
		{
			["Human"] = 0.02, ["Elf"] = 0.02, ["City Elf"] = 0.02, ["Desert Elf"] = 0.02,
			["Dwarf"] = 0.02, ["Half-Elf"] = 0.02, ["Half-Giant"] = 0.10, ["Mul"] = 0.03
		};
		db.ChangeTracker.Clear();
		foreach (var (name, rate) in expected)
		{
			var race = db.Races.Single(x => x.Name == name);
			Assert.AreEqual(rate, race.SweatRateInLitresPerMinute);
			Assert.AreEqual(901L, race.SweatLiquidId);
			Assert.AreEqual(race.Name is "Elf" or "Half-Elf" ? 2L : 1L, race.AvailabilityProgId);
		}
		foreach (var (key, rate) in ArmageddonHeritage.Profiles.Select(x => (x.Key, expected[x.Name])))
		{
			Assert.AreEqual(rate, ArmageddonHeritage.SweatRateForProfile(key));
			var hourly = key switch
			{
				"half_giant" => new[] { 2.40, 4.20, 5.40 },
				"mul" => new[] { 0.72, 1.26, 1.62 },
				_ => new[] { 0.48, 0.84, 1.08 }
			};
			foreach (var (factor, index) in new[] { 24.0, 42.0, 54.0 }.Select((x, i) => (x, i)))
				Assert.AreEqual(hourly[index], ArmageddonHeritage.SweatRateForProfile(key) * factor, 1e-12);
		}
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonHeritage.SweatRateForProfile("unknown"));
		// Reproduce the former recorded stock, then upgrade only through the normal installer.
		var giant = db.Races.Single(x => x.Name == "Half-Giant");
		var record = db.SeederManagedRecords.Single(x => x.StableKey == "arm.race.half_giant");
		var oldBaseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
		oldBaseline[nameof(Race.SweatRateInLitresPerMinute)] = "0.8";
		record.SeedBaseline = JsonSerializer.Serialize(oldBaseline);
		giant.SweatRateInLitresPerMinute = 0.8;
		db.SaveChanges(); db.ChangeTracker.Clear();
		ArmageddonHeritage.Install(db, messages); db.ChangeTracker.Clear();
		Assert.AreEqual(0.10, db.Races.Single(x => x.Name == "Half-Giant").SweatRateInLitresPerMinute);
		var dwarf = db.Races.Single(x => x.Name == "Dwarf");
		dwarf.SweatRateInLitresPerMinute = 0.007; dwarf.SweatLiquidId = 902; dwarf.AvailabilityProgId = 2;
		var mul = db.Races.Single(x => x.Name == "Mul");
		mul.SweatRateInLitresPerMinute = 0.0; mul.SweatLiquidId = null;
		db.SaveChanges();
		for (var attempt = 0; attempt < 2; attempt++)
		{
			db.ChangeTracker.Clear();
			ArmageddonHeritage.Install(db, messages);
			db.ChangeTracker.Clear();
			dwarf = db.Races.Single(x => x.Name == "Dwarf");
			mul = db.Races.Single(x => x.Name == "Mul");
			Assert.AreEqual(0.007, dwarf.SweatRateInLitresPerMinute);
			Assert.AreEqual(902L, dwarf.SweatLiquidId); Assert.AreEqual(2L, dwarf.AvailabilityProgId);
			Assert.AreEqual(0.0, mul.SweatRateInLitresPerMinute); Assert.IsNull(mul.SweatLiquidId);
			Assert.AreEqual(0.8, db.Races.Single(x => x.Id == 101).SweatRateInLitresPerMinute);
			Assert.AreEqual(adminCultureCount, db.Cultures.Count(x => x.Name == "Admin"));
			Assert.AreEqual(characterCount, db.Characters.Count());
		}
		Assert.IsTrue(messages.Any(x => x.Contains("builder edit")));
	}

	[TestMethod]
	public void ApprovedLifecycles_PersistAndPreserveBuilderChangesAcrossReruns()
	{
		using var db = Fixture();
		var human = db.Races.Single(x => x.Id == 101);
		human.ChildAge = 3;
		human.YouthAge = 10;
		human.YoungAdultAge = 16;
		human.AdultAge = 25;
		human.ElderAge = 50;
		human.VenerableAge = 75;
		db.SaveChanges();
		var messages = new List<string>();
		ArmageddonHeritage.Install(db, messages);
		var ids = db.Races.Where(x => x.Id != 100 && x.Id != 101).Select(x => x.Id).ToArray();
		var dwarfId = db.Races.Single(x => x.Name == "Dwarf").Id;
		var baseline = db.Races.Where(x => ids.Contains(x.Id)).ToDictionary(x => x.Id,
			x => new[] { x.ChildAge, x.YouthAge, x.YoungAdultAge, x.AdultAge, x.ElderAge, x.VenerableAge });
		static int[] Ages(Race race) =>
			[race.ChildAge, race.YouthAge, race.YoungAdultAge, race.AdultAge, race.ElderAge, race.VenerableAge];

		// Reload persisted rows rather than checking the installer's tracked objects.
		db.ChangeTracker.Clear();
		Assert.AreEqual(8, ids.Length);
		foreach (var race in db.Races.Where(x => ids.Contains(x.Id)))
		{
			CollectionAssert.AreEqual(baseline[race.Id], Ages(race));
			Assert.AreEqual(race.Name is "Elf" or "Half-Elf" ? 2L : 1L, race.AvailabilityProgId);
		}

		// These are deliberately authored test overrides, not proposed Armageddon ages.
		var dwarf = db.Races.Single(x => x.Id == dwarfId);
		dwarf.ChildAge = 5;
		dwarf.YouthAge = 12;
		dwarf.YoungAdultAge = 21;
		dwarf.AdultAge = 31;
		dwarf.ElderAge = 62;
		dwarf.VenerableAge = 82;
		db.Races.Single(x => x.Id == 101).YoungAdultAge = 18;
		db.SaveChanges();
		for (var attempt = 0; attempt < 2; attempt++)
		{
			db.ChangeTracker.Clear();
			ArmageddonHeritage.Install(db, messages);
			db.ChangeTracker.Clear();
			foreach (var race in db.Races.Where(x => ids.Contains(x.Id)))
			{
				CollectionAssert.AreEqual(race.Id == dwarfId ? new[] { 5, 12, 21, 31, 62, 82 } : baseline[race.Id],
					Ages(race), $"Lifecycle override/baseline changed for {race.Name} on rerun {attempt}.");
				Assert.AreEqual(race.Name is "Elf" or "Half-Elf" ? 2L : 1L, race.AvailabilityProgId);
			}
			Assert.AreEqual(18, db.Races.Single(x => x.Id == 101).YoungAdultAge);
		}
		Assert.IsTrue(messages.Any(x => x.Contains("builder edit")));
	}

	[TestMethod]
	public void MissingScaffoldModel_BlocksBeforeHeritageWrites()
	{
		using var db = Fixture();
		db.Races.Single(x => x.Id == 101).DefaultHeightWeightModelNonBinaryId = 999;
		db.SaveChanges();
		var before = db.SeederManagedRecords.Count();
		Assert.ThrowsExactly<InvalidOperationException>(() => ArmageddonHeritage.Install(db, new List<string>()));
		Assert.AreEqual(before, db.SeederManagedRecords.Count());
		Assert.AreEqual(1, db.Languages.Count());
	}

	[TestMethod]
	public void HeritageNameCollision_BlocksBeforeAnyHeritageWrite()
	{
		using var db = Fixture();
		db.Cultures.Add(new Culture { Name = "Tuluki" }); db.SaveChanges();
		var before = db.SeederManagedRecords.Count();
		Assert.ThrowsExactly<InvalidOperationException>(() => ArmageddonHeritage.Install(db, new List<string>()));
		Assert.AreEqual(before, db.SeederManagedRecords.Count());
		Assert.AreEqual(0, db.Races.Count(x => x.Id != 100 && x.Id != 101));
		Assert.AreEqual(1, db.Languages.Count());
	}
}
