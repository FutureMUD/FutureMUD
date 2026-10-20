#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.CharacterCreation;
using MudSharp.Models;
using MudSharp.FutureProg;
using FutureProg = MudSharp.Models.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonNamingTests
{
	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void NamingComposition_PreservesCompatibleToonAndAnyParameterDelegates(bool anyParameters)
	{
		using var db = ArmageddonHeritageTests.Fixture();
		var original = new FutureProg { FunctionName = "BuilderNamingStyles", FunctionText = "var styles as nameculture collection\nreturn @styles",
			ReturnTypeDefinition = (ProgVariableTypes.NameCulture | ProgVariableTypes.Collection).ToStorageString(), AcceptsAnyParameters = anyParameters };
		db.FutureProgs.Add(original); db.SaveChanges();
		if (!anyParameters) db.FutureProgsParameters.Add(new FutureProgsParameter { FutureProgId = original.Id, ParameterIndex = 0, ParameterName = "ch", ParameterTypeDefinition = ProgVariableTypes.Toon.ToStorageString() });
		var storyboard = db.ChargenScreenStoryboards.Single(x => x.ChargenStage == (int)ChargenStage.SelectName);
		var xml = XElement.Parse(storyboard.StageDefinition); xml.SetElementValue("NameCultureOptionsProg", original.Id); storyboard.StageDefinition = xml.ToString(); db.SaveChanges();
		for (var attempt = 0; attempt < 2; attempt++)
		{
			db.ChangeTracker.Clear(); ArmageddonHeritage.Install(db, new List<string>());
			var wrapper = db.FutureProgs.Single(x => x.FunctionName == "ArmageddonNamingStyles101");
			StringAssert.Contains(wrapper.FunctionText, "styles = @BuilderNamingStyles(@ch)");
			Assert.AreEqual(original.FunctionText, db.FutureProgs.Single(x => x.Id == original.Id).FunctionText);
		}
	}

	[TestMethod]
	public void IncompatibleNamingDelegate_RefusesBeforeHeritageWrites()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		var storyboard = db.ChargenScreenStoryboards.Single(x => x.ChargenStage == (int)ChargenStage.SelectName);
		var xml = XElement.Parse(storyboard.StageDefinition); xml.SetElementValue("NameCultureOptionsProg", 1); storyboard.StageDefinition = xml.ToString(); db.SaveChanges();
		var before = db.SeederManagedRecords.Count();
		Assert.ThrowsException<InvalidOperationException>(() => ArmageddonHeritage.Install(db, new List<string>()));
		Assert.AreEqual(before, db.SeederManagedRecords.Count()); Assert.AreEqual(0, db.NameCultures.Count());
	}
	[TestMethod]
	public void ApprovedManifest_HasAllPoolsDescriptionsAndConstrainedComponents()
	{
		var data = ArmageddonNaming.Data;
		var cultures = ArmageddonNaming.Cultures;
		Assert.AreEqual(15, cultures.Length);
		Assert.AreEqual(78, cultures.Sum(x => x.GetProperty("elements").GetArrayLength()));
		var descriptions = string.Join("\n", cultures.SelectMany(c => c.GetProperty("elements").EnumerateArray()
			.Select(e => ArmageddonNaming.Id(c) + "\n" + e.GetProperty("id").GetString() + "\n" + e.GetProperty("description").GetString())));
		Assert.AreEqual("9173A85FE57040D4F0AA7200B5C245F5A202CCED0B8B745BD9F8E576D71C47E0", Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(descriptions))), "Approved Page descriptions changed.");
		Assert.AreEqual("user_approved_Dark_Sun_inspired_adaptation", data.GetProperty("source").GetProperty("classification").GetString());
		Assert.AreEqual("5a8ce2cf4bc84bc6482db977cd86c58dc5a89b8e36ab7cd3256fb01e548fecc9", data.GetProperty("source").GetProperty("sha256").GetString());
		var pools = data.GetProperty("personal_pools").EnumerateObject().ToArray();
		Assert.AreEqual(8, pools.Length);
		var names = pools.SelectMany(x => new[] { "male", "female", "gender_unspecified" }
			.SelectMany(k => x.Value.GetProperty(k).EnumerateArray().Select(n => n.GetString()!))).ToArray();
		Assert.AreEqual(321, names.Length); Assert.AreEqual(321, names.Distinct(StringComparer.Ordinal).Count());
		Assert.IsTrue(names.All(x => x.Length <= 100 && x.IsNormalized(NormalizationForm.FormC) && !x.Any(char.IsSurrogate)));
		Assert.AreEqual(401, cultures.Where(x => x.GetProperty("kind").GetString() == "racial_tradition").Sum(x => ArmageddonNaming.Eligible(x, 0).Length));
		var components = data.GetProperty("component_collections").EnumerateObject().ToArray();
		Assert.AreEqual(62, components.Sum(x => x.Value.GetProperty("values") is var values && values.ValueKind == JsonValueKind.Array ? values.GetArrayLength() : values.EnumerateObject().Sum(v => v.Value.GetArrayLength())));
		Assert.IsTrue(components.All(x => !x.Value.GetProperty("generic_random_enabled").GetBoolean()));
		var clans = data.GetProperty("component_collections").GetProperty("elf_clans_by_tribe");
		CollectionAssert.Contains(clans.GetProperty("values").GetProperty("Night Runner Tribe").EnumerateArray().Select(x => x.GetString()!).ToArray(), "Dark Moons");
		Assert.AreEqual(0, clans.GetProperty("values").GetProperty("Shadow Tribe").GetArrayLength());
		Assert.AreEqual("explicit_confirmed_completed_apprenticeship_only", data.GetProperty("component_collections").GetProperty("pterran_path_names").GetProperty("selection").GetString());
		var human = cultures.Single(x => ArmageddonNaming.Id(x) == "human");
		CollectionAssert.Contains(ArmageddonNaming.Eligible(human, 2), "Agis");
		CollectionAssert.DoesNotContain(ArmageddonNaming.Eligible(human, 2), "Sadira");
		CollectionAssert.Contains(ArmageddonNaming.Eligible(human, 3), "Sadira");
		foreach (var key in new[] { "half_elf", "half_giant" })
			CollectionAssert.AreEquivalent(ArmageddonNaming.Eligible(human, 0), ArmageddonNaming.Eligible(cultures.Single(x => ArmageddonNaming.Id(x) == key), 0));
		var mul = cultures.Single(x => ArmageddonNaming.Id(x) == "mul");
		foreach (var gender in Enumerable.Range(0, 5)) CollectionAssert.Contains(ArmageddonNaming.Eligible(mul, gender), "Atlalak");
	}

	[TestMethod]
	public void NamingInstall_StoresExactDescriptionsAndPersonalOnlyGenderProfilesWithoutNewRaces()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		ArmageddonHeritage.Install(db, new List<string>());
		db.ChangeTracker.Clear();
		Assert.AreEqual(16, db.NameCultures.Count()); Assert.AreEqual(75, db.RandomNameProfiles.Count());
		Assert.AreEqual(10, db.Races.Count()); Assert.AreEqual(6, db.Languages.Count());
		Assert.IsFalse(db.Races.Any(x => x.Name.Contains("Pterran") || x.Name.Contains("Thri-kreen") || x.Name.Contains("Halfling")));
		foreach (var culture in ArmageddonNaming.Cultures)
		{
			var native = db.NameCultures.Single(x => x.Name == ArmageddonNaming.Name(culture));
			var xml = XElement.Parse(native.Definition);
			Assert.AreEqual("true", xml.Element("PreserveNameCase")!.Value);
			Assert.AreEqual("true", xml.Element("ExactRandomProfileGender")!.Value);
			var personal = culture.GetProperty("elements").EnumerateArray().Single(x => x.GetProperty("id").GetString() == "personal_name");
			Assert.AreEqual(personal.GetProperty("description").GetString(), xml.Element("Elements")!.Elements().Single().Value);
			using var curated = JsonDocument.Parse(xml.Element("CuratedNamingDefinition")!.Value);
			Assert.AreEqual(JsonSerializer.Serialize(culture), JsonSerializer.Serialize(curated.RootElement.GetProperty("culture")));
			foreach (var profile in db.RandomNameProfiles.Where(x => x.NameCultureId == native.Id))
			{
				var entries = db.RandomNameProfilesElements.Where(x => x.RandomNameProfileId == profile.Id).ToArray();
				Assert.IsTrue(entries.All(x => x.NameUsage == 0 && x.Weighting == 1));
				CollectionAssert.AreEquivalent(ArmageddonNaming.Eligible(culture, profile.Gender), entries.Select(x => x.Name).ToArray());
				Assert.AreEqual("1", db.RandomNameProfilesDiceExpressions.Single(x => x.RandomNameProfileId == profile.Id).DiceExpression);
			}
		}
		Assert.AreEqual(0, db.EthnicitiesNameCultures.Count(x => x.EthnicityId != 100), "Upbringing remains the default, including City Elf outside Labyrinth.");
		var screen = XElement.Parse(db.ChargenScreenStoryboards.Single(x => x.ChargenStage == (int)ChargenStage.SelectName).StageDefinition);
		Assert.AreEqual("Retain", screen.Element("BuilderNote")!.Value); Assert.AreEqual("true", screen.Element("AllowUnicodeNames")!.Value);
		var prog = db.FutureProgs.Single(x => x.Id == long.Parse(screen.Element("NameCultureOptionsProg")!.Value));
		StringAssert.Contains(prog.FunctionText, "ToNameCulture(");
	}

	[TestMethod]
	public void NamingReruns_PreserveEditedWeightEntryDiceCultureAndStoryboard()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		var messages = new List<string>(); ArmageddonHeritage.Install(db, messages);
		var profile = db.RandomNameProfiles.Single(x => x.Name == "Armageddon Thri-kreen naming tradition - 2");
		var profileId = profile.Id; profile.Name = "Builder profile";
		db.RandomNameProfilesElements.Single(x => x.RandomNameProfileId == profile.Id && x.Name == "Ka’Cha").Weighting = 9;
		db.RandomNameProfilesElements.Remove(db.RandomNameProfilesElements.Single(x => x.RandomNameProfileId == profile.Id && x.Name == "Ka’Ka’Kyl"));
		db.RandomNameProfilesElements.Add(new RandomNameProfilesElements { RandomNameProfileId = profile.Id, NameUsage = 0, Name = "Builder’Name", Weighting = 3 });
		db.RandomNameProfilesDiceExpressions.Single(x => x.RandomNameProfileId == profile.Id).DiceExpression = "0";
		var native = db.NameCultures.Single(x => x.Id == profile.NameCultureId);
		native.Definition = native.Definition.Replace("Required one personal name.", "Builder guidance.", StringComparison.Ordinal);
		var definition = native.Definition;
		var storyboard = db.ChargenScreenStoryboards.Single(x => x.ChargenStage == (int)ChargenStage.SelectName);
		var xml = XElement.Parse(storyboard.StageDefinition); xml.SetElementValue("AllowUnicodeNames", "false"); storyboard.StageDefinition = xml.ToString();
		db.SaveChanges();
		var identities = db.SeederManagedRecords.ToDictionary(x => x.StableKey, x => x.LogicalId);
		for (var attempt = 0; attempt < 2; attempt++)
		{
			db.ChangeTracker.Clear(); ArmageddonHeritage.Install(db, messages); db.ChangeTracker.Clear();
			Assert.AreEqual("Builder profile", db.RandomNameProfiles.Single(x => x.Id == profileId).Name);
			Assert.AreEqual(9, db.RandomNameProfilesElements.Single(x => x.RandomNameProfileId == profileId && x.Name == "Ka’Cha").Weighting);
			Assert.IsFalse(db.RandomNameProfilesElements.Any(x => x.RandomNameProfileId == profileId && x.Name == "Ka’Ka’Kyl"));
			Assert.AreEqual(3, db.RandomNameProfilesElements.Single(x => x.RandomNameProfileId == profileId && x.Name == "Builder’Name").Weighting);
			Assert.AreEqual("0", db.RandomNameProfilesDiceExpressions.Single(x => x.RandomNameProfileId == profileId).DiceExpression);
			Assert.AreEqual(definition, db.NameCultures.Single(x => x.Id == native.Id).Definition);
			Assert.AreEqual("false", XElement.Parse(db.ChargenScreenStoryboards.Single(x => x.Id == storyboard.Id).StageDefinition).Element("AllowUnicodeNames")!.Value);
			Assert.AreEqual(identities.Count, db.SeederManagedRecords.Count()); Assert.IsTrue(db.SeederManagedRecords.All(x => identities[x.StableKey] == x.LogicalId));
		}
		Assert.IsTrue(messages.Any(x => x.Contains("builder edit")));
	}
}
