#nullable enable
using System;
using System.Linq;
using System.Text.Json;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonConditionStockDefinitionTests
{
	[TestMethod]
	public void DiseasePair_LockedInventoryFloorsSchoolsGradesAndDependenciesRemainExact()
	{
		using var manifest = JsonDocument.Parse(ArmageddonStockManifest.Read());
		var entries = manifest.RootElement.GetProperty("entries").EnumerateArray().ToDictionary(x => x.GetProperty("key").GetString()!);
		var contents = ArmageddonConditionSpellContent.Create(new(5, 102));
		Assert.AreEqual(2, contents.Count);
		foreach (var contribution in contents)
		{
			var content = contribution.Content; var entry = entries[content.Key];
			Assert.AreEqual(entry.GetProperty("name").GetString(), content.Name);
			Assert.AreEqual("arm.capability." + entry.GetProperty("native_school").GetString()!.ToLowerInvariant(), contribution.NativeCapabilityKey);
			Assert.AreEqual(entry.GetProperty("minimum_energy").GetDouble(), content.MinimumEnergy);
			Assert.IsTrue(entry.GetProperty("prototype_dependencies").EnumerateArray().Any(x => x.GetString() == "arm.effect.disease"));
			var definition = content.BuildDefinition(201, 202, 0); var profile = definition.Element("ControlledPower")!;
			Assert.AreEqual(entry.GetProperty("classic_elemental_admissions")[0].GetProperty("opening").GetInt32(), (int)profile.Attribute("openingSkill")!);
			CollectionAssert.AreEqual(Enumerable.Range(1, 7).ToArray(), profile.Elements("Grade").Select(x => (int)x.Attribute("number")!).ToArray());
			CollectionAssert.AreEqual(Enumerable.Range((int)SpellPower.ExtremelyWeak, 7).ToArray(), profile.Elements("Grade").Select(x => (int)x.Attribute("power")!).ToArray());
			Assert.AreEqual("source", (string?)profile.Element("Efficiency")!.Attribute("type"));
			Assert.AreEqual(content.MinimumEnergy, (double)profile.Element("Efficiency")!.Attribute("minimum")!);
			Assert.IsNull(profile.Element("Area")); Assert.IsNull(profile.Element("Incantation"));
			Assert.IsFalse(content.SpellRow(301, 302, 303).ScrollInscriptionAllowed);
			Assert.IsNull(content.EligibilityRow(301)); Assert.AreEqual(0L, (long)definition.Element("Trigger")!.Element("TargetFilterProg")!);
			Assert.AreEqual(0, definition.Element("Plan")!.Elements().Count());
			Assert.AreEqual(0, definition.Element("CasterEffects")!.Elements().Count());
			var effect = definition.Element("Effects")!.Elements().Single();
			Assert.AreEqual(5, (int)effect.Element("InfectionType")!);
			Assert.AreEqual("arm.effect.disease", effect.Element("OwnedFamily")!.Value);
			Assert.AreEqual((int)DrugVector.Touched, (int)effect.Element("ExposureVector")!);
			Assert.AreEqual("grade", effect.Element("IntensityFormula")!.Value); Assert.AreEqual("1", effect.Element("VirulenceFormula")!.Value);
			Assert.AreEqual((int)Difficulty.Normal, (int)effect.Element("VirulenceDifficulty")!);
			var sow = content.Key == "arm.spell.sow_sickness";
			Assert.AreEqual(sow ? "60*grade" : "0", content.DurationFormula);
			Assert.AreEqual(sow ? "disease" : "removedisease", (string?)effect.Attribute("type"));
			Assert.AreEqual(sow ? 102L : null, content.ResistingTraitId);
			Assert.AreEqual(sow ? Difficulty.Normal : null, content.ResistingDifficulty);
			for (var grade = 1; grade <= 7; grade++)
				Assert.AreEqual($"{content.MinimumEnergy}*grade", content.CostRow(300).Expression);
			definition.Element("Effects")!.RemoveNodes();
			Assert.AreEqual(1, content.BuildDefinition(201, 202, 0).Element("Effects")!.Elements().Count());
		}
	}

	[TestMethod]
	[DataRow(0L)]
	[DataRow(-1L)]
	[DataRow(1L)]
	[DataRow(4L)]
	public void DiseasePair_OnlyExactResolvedNativeS3KindIsAccepted(long invalidKind) =>
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonConditionSpellContent.Create(new(invalidKind, 102)));

	[TestMethod]
	[DataRow(0L)]
	[DataRow(-1L)]
	public void DiseasePair_UnresolvedTraitsResourcesExpressionsRefuseWithoutFallback(long invalid)
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonConditionSpellContent.Create(new(5, invalid)));
		foreach (var contribution in ArmageddonConditionSpellContent.Create(new(5, 102)))
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => contribution.Content.BuildDefinition(invalid, 202, 0));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => contribution.Content.BuildDefinition(201, invalid, 0));
			if (invalid < 0) Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => contribution.Content.BuildDefinition(201, 202, invalid));
		}
	}
}
