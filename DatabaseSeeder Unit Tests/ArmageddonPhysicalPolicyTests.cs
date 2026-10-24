#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.CharacterCreation;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonPhysicalPolicyTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void EarlierRaceBaseline_AdoptsNewUnsetAgeDescriberButPreservesCustomReference(bool custom)
	{
		using var db = ArmageddonHeritageTests.Fixture();
		var messages = new List<string>(); ArmageddonHeritage.Install(db, messages);
		var record = db.SeederManagedRecords.Single(x => x.StableKey == "arm.race.human");
		var race = db.Races.Single(x => x.Id == record.LogicalId);
		var expected = race.AgeDescriptionProgId;
		var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
		baseline.Remove(nameof(Race.AgeDescriptionProgId));
		record.SeedBaseline = JsonSerializer.Serialize(baseline);
		record.AppliedFingerprint = System.Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(record.SeedBaseline))).ToLowerInvariant();
		race.AgeDescriptionProgId = custom ? db.FutureProgs.Single(x => x.FunctionName == "CustomMinimumAgeProg").Id : null;
		var customId = race.AgeDescriptionProgId;
		db.SaveChanges(); db.ChangeTracker.Clear();
		ArmageddonHeritage.Install(db, messages); db.ChangeTracker.Clear();
		Assert.AreEqual(custom ? customId : expected, db.Races.Single(x => x.Id == race.Id).AgeDescriptionProgId);
		if (!custom)
		{
			db.Races.Single(x => x.Id == race.Id).AgeDescriptionProgId = null;
			db.SaveChanges(); ArmageddonHeritage.Install(db, messages); db.ChangeTracker.Clear();
			Assert.IsNull(db.Races.Single(x => x.Id == race.Id).AgeDescriptionProgId, "An explicit removal after adoption is a builder choice.");
		}
	}

	[TestMethod]
	public void AcceptsAnyNumericDelegate_RemainsUsableForPhysicalPolicy()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		var original = db.FutureProgs.Single(x => x.FunctionName == "CustomMaximumAgeProg");
		original.AcceptsAnyParameters = true;
		db.FutureProgsParameters.RemoveRange(db.FutureProgsParameters.Where(x => x.FutureProgId == original.Id));
		db.SaveChanges(); ArmageddonHeritage.Install(db, new List<string>());
	}
	[TestMethod]
	public void ChargenOnlyDelegates_KeepCompatibleWrapperSignatures()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		var original = db.FutureProgs.Single(x => x.FunctionName == "CustomMinimumAgeProg");
		db.FutureProgsParameters.Single(x => x.FutureProgId == original.Id).ParameterType = (long)MudSharp.FutureProg.ProgVariableTypes.Chargen;
		db.SaveChanges();
		ArmageddonHeritage.Install(db, new List<string>());
		var screen = db.ChargenScreenStoryboards.Single(x => x.ChargenStage == (int)ChargenStage.SelectBirthday);
		var id = long.Parse(XElement.Parse(screen.StageDefinition).Element("MinimumAgeProg")!.Value);
		Assert.AreEqual((long)MudSharp.FutureProg.ProgVariableTypes.Chargen, db.FutureProgsParameters.Single(x => x.FutureProgId == id).ParameterType);
	}
	[TestMethod]
	[DataRow("human", 10, 20, 30, 61, 81, 80)]
	[DataRow("city_elf", 12, 24, 35, 71, 94, 93)]
	[DataRow("desert_elf", 12, 24, 35, 71, 94, 93)]
	[DataRow("dwarf", 22, 44, 66, 135, 179, 178)]
	[DataRow("half_elf", 11, 22, 33, 66, 88, 87)]
	[DataRow("half_giant", 18, 36, 54, 110, 146, 145)]
	[DataRow("mul", 8, 15, 23, 46, 61, 60)]
	public void ApprovedAgePolicy_PersistsExplicitLifecyclesAndSeparateCreationBounds(string key, int youth, int youngAdult, int adult, int elder, int venerable, int maximum)
	{
		using var db = ArmageddonHeritageTests.Fixture();
		ArmageddonHeritage.Install(db, new List<string>());
		db.ChangeTracker.Clear();
		var id = db.SeederManagedRecords.Single(x => x.StableKey == "arm.race." + key).LogicalId;
		var race = db.Races.Single(x => x.Id == id);
		CollectionAssert.AreEqual(new[] { 3, youth, youngAdult, adult, elder, venerable },
			new[] { race.ChildAge, race.YouthAge, race.YoungAdultAge, race.AdultAge, race.ElderAge, race.VenerableAge });
		Assert.AreEqual(2L, race.AvailabilityProgId);
		Assert.IsNotNull(race.AgeDescriptionProgId);
		var screen = db.ChargenScreenStoryboards.Single(x => x.ChargenStage == (int)ChargenStage.SelectBirthday);
		var xml = XElement.Parse(screen.StageDefinition);
		var minimumBody = db.FutureProgs.Single(x => x.Id == long.Parse(xml.Element("MinimumAgeProg")!.Value)).FunctionText;
		var maximumBody = db.FutureProgs.Single(x => x.Id == long.Parse(xml.Element("MaximumAgeProg")!.Value)).FunctionText;
		StringAssert.Contains(minimumBody, $"ToRace({id}))\n  return {youngAdult}\n");
		StringAssert.Contains(maximumBody, $"ToRace({id}))\n  return {maximum}\n");
		StringAssert.Contains(minimumBody, "return @CustomMinimumAgeProg(@ch)");
		StringAssert.Contains(maximumBody, "return @CustomMaximumAgeProg(@ch)");
	}

	[TestMethod]
	public void PhysicalRerun_PreservesModelsDisplayProgsAppearanceAndIndependentAgeExceptions()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		var messages = new List<string>();
		ArmageddonHeritage.Install(db, messages);
		var dwarf = db.Races.Single(x => x.Name == "Armageddon Dwarf");
		var customModel = db.HeightWeightModels.Single(x => x.Id == dwarf.DefaultHeightWeightModelMaleId);
		customModel.Name = "Builder physical model";
		var customDescription = db.FutureProgs.Single(x => x.Id == dwarf.AgeDescriptionProgId);
		customDescription.FunctionText = "return \"builder age word\"";
		dwarf.YoungAdultAge = 45;
		var screen = db.ChargenScreenStoryboards.Single(x => x.ChargenStage == (int)ChargenStage.SelectBirthday);
		var xml = XElement.Parse(screen.StageDefinition);
		var minimum = db.FutureProgs.Single(x => x.Id == long.Parse(xml.Element("MinimumAgeProg")!.Value));
		minimum.FunctionText = "// Explicit builder exception\nreturn 17";
		xml.Add(new XElement("BuilderNote", "Retain")); screen.StageDefinition = xml.ToString();
		var hair = db.CharacteristicProfiles.Single(x => x.Name == "Armageddon hairless.hair Appearance");
		hair.Description = "Builder hair policy";
		var ethnicity = db.Ethnicities.Single(x => x.ParentRaceId == dwarf.Id);
		var ears = db.EthnicitiesCharacteristics.Single(x => x.EthnicityId == ethnicity.Id && x.CharacteristicDefinitionId == 110);
		db.EthnicitiesCharacteristics.Remove(ears);
		db.SaveChanges();
		var ids = db.SeederManagedRecords.ToDictionary(x => x.StableKey, x => x.LogicalId);
		for (var attempt = 0; attempt < 2; attempt++)
		{
			db.ChangeTracker.Clear(); ArmageddonHeritage.Install(db, messages); db.ChangeTracker.Clear();
			Assert.AreEqual("Builder physical model", db.HeightWeightModels.Single(x => x.Id == customModel.Id).Name);
			Assert.AreEqual("return \"builder age word\"", db.FutureProgs.Single(x => x.Id == customDescription.Id).FunctionText);
			Assert.AreEqual(45, db.Races.Single(x => x.Id == dwarf.Id).YoungAdultAge);
			Assert.AreEqual("// Explicit builder exception\nreturn 17", db.FutureProgs.Single(x => x.Id == minimum.Id).FunctionText);
			Assert.AreEqual("Retain", XElement.Parse(db.ChargenScreenStoryboards.Single(x => x.Id == screen.Id).StageDefinition).Element("BuilderNote")!.Value);
			Assert.AreEqual("Builder hair policy", db.CharacteristicProfiles.Single(x => x.Id == hair.Id).Description);
			Assert.IsFalse(db.EthnicitiesCharacteristics.Any(x => x.EthnicityId == ethnicity.Id && x.CharacteristicDefinitionId == 110));
			Assert.IsTrue(db.SeederManagedRecords.All(x => ids[x.StableKey] == x.LogicalId));
		}
	}

	[TestMethod]
	public void Appearance_UsesExplicitNativeHairlessProfilesWithoutChangingSharedHumanChoices()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		var original = db.CharacteristicProfiles.Single(x => x.Id == 112).Definition;
		ArmageddonHeritage.Install(db, new List<string>());
		foreach (var name in new[] { "Dwarf", "Mul" })
		{
			var ethnicity = db.Ethnicities.Single(x => x.Name == "Armageddon " + name);
			foreach (var definition in new long[] { 112, 113 })
			{
				var id = db.EthnicitiesCharacteristics.Single(x => x.EthnicityId == ethnicity.Id && x.CharacteristicDefinitionId == definition).CharacteristicProfileId;
				var xml = XElement.Parse(db.CharacteristicProfiles.Single(x => x.Id == id).Definition);
				Assert.AreEqual(true, (bool?)xml.Attribute("RestrictStyles"));
				Assert.AreEqual(1, xml.Elements("Value").Count());
			}
		}
		Assert.AreEqual(original, db.CharacteristicProfiles.Single(x => x.Id == 112).Definition);
	}
}
