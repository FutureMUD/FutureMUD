#nullable enable
using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonPrimitiveStockDefinitionTests
{
	private static ArmageddonPrimitiveSpellBindings Bindings => new(101, 102, 103, 104, Difficulty.Normal);

	[TestMethod]
	public void FalsePresence_UsesOnlyViewerPolicyAndLockedWindDuration()
	{
		var contribution = ArmageddonPrimitiveSpellContent.FalsePresence(103);
		Assert.AreEqual("arm.capability.wind", contribution.NativeCapabilityKey);
		Assert.AreEqual("arm.spell.false_presence", contribution.Content.Key);
		Assert.AreEqual(7.0, contribution.Content.MinimumEnergy);
		Assert.AreEqual("60*grade", contribution.Content.DurationFormula);
		var definition = contribution.Content.BuildDefinition(201, 202, 0);
		Assert.AreEqual("character", (string?)definition.Element("Trigger")!.Attribute("type"));
		Assert.AreEqual(40, (int)definition.Element("ControlledPower")!.Attribute("openingSkill")!);
		var effect = definition.Element("Effects")!.Elements().Single();
		Assert.AreEqual("invisibility", (string?)effect.Attribute("type"));
		Assert.AreEqual(103L, (long)effect.Element("FilterProg")!);
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonPrimitiveSpellContent.FalsePresence(0));
	}

	[TestMethod]
	public void Create_EveryDefinitionMatchesLockedIdentitySchoolFloorAndNativeGrades()
	{
		using var manifest = JsonDocument.Parse(ArmageddonStockManifest.Read());
		var entries = manifest.RootElement.GetProperty("entries").EnumerateArray()
			.ToDictionary(x => x.GetProperty("key").GetString()!);
		var contributions = ArmageddonPrimitiveSpellContent.Create(Bindings);
		Assert.AreEqual(18, contributions.Count);
		Assert.AreEqual(18, contributions.Select(x => x.Content.Key).Distinct().Count());
		foreach (var contribution in contributions)
		{
			var content = contribution.Content;
			var entry = entries[content.Key];
			Assert.AreEqual("existing_primitive_configuration", entry.GetProperty("classification").GetString(), content.Key);
			Assert.AreEqual(entry.GetProperty("name").GetString(), content.Name, content.Key);
			Assert.AreEqual("arm.capability." + entry.GetProperty("native_school").GetString()!.ToLowerInvariant(),
				contribution.NativeCapabilityKey, content.Key);
			Assert.AreEqual(entry.GetProperty("minimum_energy").GetDouble(), content.MinimumEnergy, content.Key);
			var definition = content.BuildDefinition(201, 202, 203);
			Assert.AreEqual(content.Key, definition.Element("StockIdentity")!.Value);
			Assert.AreEqual("character", (string?)definition.Element("Trigger")!.Attribute("type"));
			Assert.AreEqual(203L, (long)definition.Element("Trigger")!.Element("TargetFilterProg")!);
			var cost = definition.Element("Costs")!.Elements().Single();
			Assert.AreEqual(201L, (long)cost.Attribute("resource")!);
			Assert.AreEqual(202L, (long)cost.Attribute("expression")!);
			var profile = definition.Element("ControlledPower")!;
			var opening = entry.TryGetProperty("classic_sorcerer_row", out var sorcerer) && sorcerer.ValueKind == JsonValueKind.Object
				? sorcerer.GetProperty("opening").GetInt32()
				: entry.GetProperty("classic_elemental_admissions")[0].GetProperty("opening").GetInt32();
			Assert.AreEqual(opening, (int)profile.Attribute("openingSkill")!, content.Key);
			Assert.AreEqual(content.MinimumEnergy, (double)profile.Element("Efficiency")!.Attribute("minimum")!);
			Assert.AreEqual("source", (string?)profile.Element("Efficiency")!.Attribute("type"));
			CollectionAssert.AreEqual(Enumerable.Range(1, 7).ToArray(), profile.Elements("Grade").Select(x => (int)x.Attribute("number")!).ToArray());
			CollectionAssert.AreEqual(Enumerable.Range((int)SpellPower.ExtremelyWeak, 7).ToArray(), profile.Elements("Grade").Select(x => (int)x.Attribute("power")!).ToArray());
			CollectionAssert.AreEqual(new[] { 0.0, 20, 40, 55, 70, 85, 95 }, profile.Elements("Grade").Select(x => (double)x.Attribute("skill")!).ToArray());
			CollectionAssert.AreEqual(new[] { 0, 0, 1, 1, 2, 2, 3 }, profile.Elements("Grade").Select(x => (int)x.Attribute("difficulty")!).ToArray());
			Assert.AreEqual($"{content.MinimumEnergy.ToString(CultureInfo.InvariantCulture)}*grade", content.CostRow(300).Expression);
			Assert.IsNull(profile.Element("Area"));
			Assert.IsNull(profile.Element("Incantation"));
			Assert.AreEqual(0, definition.Element("Plan")!.Elements().Count());
			Assert.AreEqual(0, definition.Element("CasterEffects")!.Elements().Count());
			Assert.AreEqual(1, definition.Element("Effects")!.Elements().Count());
			var row = content.SpellRow(401, 402, 403);
			Assert.IsFalse(row.ScrollInscriptionAllowed);
			Assert.IsTrue(row.AppliedEffectsAreExclusive);
			Assert.AreEqual(401L, row.MagicSchoolId);
			Assert.AreEqual(402L, row.CastingTraitDefinitionId);
			Assert.AreEqual(403L, row.SpellKnownProgId);
			Assert.IsNull(content.EligibilityRow(300));
			var isInstantaneous = content.Key is "arm.spell.clear_eyes" or "arm.spell.lift_hex" or "arm.spell.reveal_the_hidden"
				or "arm.spell.second_breath" or "arm.spell.empty_aura";
			Assert.AreEqual(isInstantaneous ? "0" : "60*grade", content.DurationFormula, content.Key);
		}
	}

	[TestMethod]
	[DataRow("borrowed_tongues", "comprehendlanguage")]
	[DataRow("hush_hearing", "deafness")]
	[DataRow("seal_voice", "silence")]
	[DataRow("heavy_slumber", "sleep")]
	[DataRow("earthen_strength", "boost")]
	[DataRow("sapping_weight", "boost")]
	[DataRow("veil_from_sight", "invisibility")]
	[DataRow("reveal_the_hidden", "removeinvisibility")]
	[DataRow("shroud_eyes", "blindness")]
	[DataRow("clear_eyes", "removeblindness")]
	[DataRow("lift_hex", "removecurse")]
	[DataRow("dread_presence", "fear")]
	[DataRow("night_eyes", "infravision")]
	[DataRow("clinging_hex", "curse")]
	[DataRow("second_breath", "staminadelta")]
	[DataRow("wakeful_mind", "insomnia")]
	[DataRow("still_limbs", "paralysis")]
	[DataRow("empty_aura", "magicresourcedelta")]
	public void Create_CanonicalEntryUsesDefiningNativeEffect(string suffix, string expectedEffect)
	{
		var content = ArmageddonPrimitiveSpellContent.Create(Bindings).Single(x => x.Content.Key == "arm.spell." + suffix).Content;
		var effect = content.BuildDefinition(201, 202, 203).Element("Effects")!.Elements().Single();
		Assert.AreEqual(expectedEffect, (string?)effect.Attribute("type"));
		if (suffix == "second_breath") Assert.AreEqual("5*grade", effect.Element("Formula")!.Value);
		if (suffix == "empty_aura") Assert.AreEqual("-5*grade", effect.Element("Formula")!.Value);
	}

	[TestMethod]
	public void Create_ExactDependenciesAndResistanceAreExplicitAndDefinitionsAreIndependent()
	{
		var contents = ArmageddonPrimitiveSpellContent.Create(Bindings).ToDictionary(x => x.Content.Key, x => x.Content);
		foreach (var key in new[] { "arm.spell.earthen_strength", "arm.spell.sapping_weight" })
		{
			var definition = contents[key].BuildDefinition(201, 202, 203);
			Assert.AreEqual(101L, (long)definition.Element("Effects")!.Elements().Single().Attribute("trait")!);
			Assert.AreEqual(key.EndsWith("sapping_weight") ? "-grade" : "grade", definition.Element("ControlledPower")!.Element("ScalarBindings")!.Elements().Single().Value);
			definition.Element("Effects")!.RemoveNodes();
			Assert.AreEqual(1, contents[key].BuildDefinition(201, 202, 203).Element("Effects")!.Elements().Count());
		}
		Assert.AreEqual(103L, (long)contents["arm.spell.veil_from_sight"].BuildDefinition(201, 202, 203).Element("Effects")!.Elements().Single().Element("FilterProg")!);
		Assert.AreEqual(104L, (long)contents["arm.spell.empty_aura"].BuildDefinition(201, 202, 203).Element("Effects")!.Elements().Single().Element("Resource")!);
		Assert.AreEqual(102L, contents["arm.spell.still_limbs"].SpellRow(401, 402, 403).ResistingTraitDefinitionId);
		Assert.AreEqual((int)Difficulty.Normal, contents["arm.spell.still_limbs"].SpellRow(401, 402, 403).ResistingDifficulty);
		Assert.IsTrue(contents.Where(x => x.Key != "arm.spell.still_limbs").All(x => x.Value.ResistingTraitId is null));
	}

	[TestMethod]
	[DataRow(0L)]
	[DataRow(-1L)]
	public void Create_UnresolvedNativeBindingsRefuseWithoutFallback(long invalid)
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonPrimitiveSpellContent.Create(Bindings with { StrengthTraitId = invalid }));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonPrimitiveSpellContent.Create(Bindings with { ResistanceTraitId = invalid }));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonPrimitiveSpellContent.Create(Bindings with { InvisibilityApplicabilityProgId = invalid }));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonPrimitiveSpellContent.Create(Bindings with { EmptyAuraResourceId = invalid }));
		foreach (var contribution in ArmageddonPrimitiveSpellContent.Create(Bindings))
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => contribution.Content.BuildDefinition(invalid, 202, 203));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => contribution.Content.BuildDefinition(201, invalid, 203));
			if (invalid < 0)
				Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => contribution.Content.BuildDefinition(201, 202, invalid));
		}
	}

	[TestMethod]
	public void Create_NoEligibilitySourceUsesNativeZeroFilterWithoutSupportProg()
	{
		foreach (var contribution in ArmageddonPrimitiveSpellContent.Create(Bindings))
		{
			Assert.IsNull(contribution.Content.EligibilitySource);
			Assert.IsNull(contribution.Content.EligibilityRow(300));
			var definition = contribution.Content.BuildDefinition(201, 202, 0);
			Assert.AreEqual(0L, (long)definition.Element("Trigger")!.Element("TargetFilterProg")!, contribution.Content.Key);
		}
	}
}
