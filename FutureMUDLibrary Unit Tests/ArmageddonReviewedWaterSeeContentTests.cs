#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonReviewedWaterSeeContentTests
{
	// XML hashes captured from the independently cleared MudSharp.dll, not the extracted implementation.
	// Source 56b342791 / tested code 55922a080; core SHA256 34b68f0e85a20a5082b3ac17292b412524047abfc862c64e7b045fcc8172a423.
	private const string WaterEligibility = "var posture = positionid(@caster)\nreturn @caster.incombat == false and (@posture == 1 or @posture == 9 or @posture == 10 or @posture == 11 or @posture == 14 or @posture == 15 or @posture == 13 or @posture == 16 or @posture == 17 or @posture == 18 or @posture == 19 or @posture == 20)";
	private const string SeeEligibility = "if (@target != @caster)\nreturn false\nend if\nvar posture = positionid(@caster)\nreturn @caster.incombat == false and (@posture == 1 or @posture == 9 or @posture == 10 or @posture == 11 or @posture == 14 or @posture == 15 or @posture == 13 or @posture == 16 or @posture == 17 or @posture == 18 or @posture == 19 or @posture == 20)";
	private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
	private static ArmageddonUtilitySpellContent Content(bool see) => see
		? ArmageddonReviewedWaterSeeContent.SeeTheUnbodied(301, 303, [401, 403, 405, 407, 409])
		: ArmageddonReviewedWaterSeeContent.WaterBreathing([201, 203]);

	[DataTestMethod, DataRow(false), DataRow(true)]
	public void PureContribution_MatchesClearedDefinitionEligibilityAndDeclaredContent(bool see)
	{
		var content = Content(see);
		var xml = content.BuildDefinition(101, 103, 107);
		Assert.AreEqual(see ? "e7fd8f8326f44b24ab05a30e7ea7848e26a92f5b5093e812f79bee6b00ac6e7d" : "1bd104111bb00a58c4ac3772a9f987bfec2c20d9906aef7ea8bbd3c66f687582", Hash(xml.ToString(SaveOptions.DisableFormatting)));
		Assert.AreEqual(see ? "2a28f3cb45702080cf8ffe0ac4722aa8309816013f4b0940ba036dd102c804c1" : "ba47b54dd090406de6db12d017e67ac0d48e205b34f05c52e956b50848a45dc1", Hash(content.Description));
		Assert.AreEqual(see ? SeeEligibility : WaterEligibility, content.EligibilitySource);
		Assert.AreEqual(see ? "arm.spell.see_the_unbodied" : "arm.spell.water_breathing", content.Key);
		Assert.AreEqual(see ? "See the Unbodied" : "Water Breathing", content.Name);
		Assert.AreEqual(see ? "1800*grade" : "0", content.DurationFormula);
		Assert.AreEqual(see ? 7.0 : 20.0, content.MinimumEnergy);
		Assert.AreEqual(see ? "$0 open|opens $0's senses to the unbodied." : "$0 weave|weaves a water breathing enchantment around $1.", content.Emote);
		var row = content.SpellRow(11, 13, 17);
		Assert.AreEqual(content.Name, row.Name); Assert.AreEqual(content.Description, row.Description);
		Assert.AreEqual(11L, row.MagicSchoolId); Assert.AreEqual(13L, row.CastingTraitDefinitionId);
		Assert.AreEqual(17L, row.SpellKnownProgId); Assert.IsTrue(row.AppliedEffectsAreExclusive);
		Assert.IsFalse(row.ScrollInscriptionAllowed);
		Assert.AreEqual(content.EligibilitySource, content.EligibilityRow(19)!.FunctionText);
		Assert.AreEqual(content.DurationFormula, content.DurationRow(19).Expression);
		Assert.AreEqual(see ? "7*grade" : "20*grade", content.CostRow(19).Expression);
	}

	[TestMethod]
	public void WaterContribution_CopiesInputsAndReturnsIndependentDefinitionTrees()
	{
		var liquids = new List<long> { 201, 203 };
		var content = ArmageddonReviewedWaterSeeContent.WaterBreathing(liquids);
		liquids[0] = 999; liquids.Clear();
		var first = content.BuildDefinition(101, 103, 107);
		first.Descendants("Liquid").First().SetAttributeValue("id", 999);
		var next = content.BuildDefinition(101, 103, 107);
		CollectionAssert.AreEqual(new long[] { 201, 203 }, next.Descendants("Liquid").Select(x => (long)x.Attribute("id")!).ToArray());
		Assert.AreEqual("1bd104111bb00a58c4ac3772a9f987bfec2c20d9906aef7ea8bbd3c66f687582", Hash(next.ToString(SaveOptions.DisableFormatting)));
		Assert.AreEqual(211L, (long)content.BuildDefinition(211, 223, 227).Element("Costs")!.Element("Cost")!.Attribute("resource")!);
	}

	[TestMethod]
	public void SeeContribution_CopiesInputsAndReturnsIndependentPlanAndScopeTrees()
	{
		var ranks = new List<long> { 401, 403, 405, 407, 409 };
		var content = ArmageddonReviewedWaterSeeContent.SeeTheUnbodied(301, 303, ranks);
		ranks[0] = 999; ranks.Clear();
		var first = content.BuildDefinition(101, 103, 107);
		first.Descendants("Rank").First().SetAttributeValue("tag", 999);
		first.Descendants("SourceScope").Single().SetAttributeValue("shadow", 999);
		var next = content.BuildDefinition(101, 103, 107);
		CollectionAssert.AreEqual(new long[] { 401, 403, 405, 407, 409 }, next.Descendants("Rank").Select(x => (long)x.Attribute("tag")!).ToArray());
		Assert.AreEqual(303L, (long)next.Descendants("SourceScope").Single().Attribute("shadow")!);
		Assert.AreEqual("e7fd8f8326f44b24ab05a30e7ea7848e26a92f5b5093e812f79bee6b00ac6e7d", Hash(next.ToString(SaveOptions.DisableFormatting)));
	}

	[TestMethod]
	public void SeeContribution_AllowsDeliberateSiltShadowOverlapWithoutInferringNativeAncestry()
	{
		var content = ArmageddonReviewedWaterSeeContent.SeeTheUnbodied(301, 301, [409, 401, 407, 403, 405]);
		var scope = content.BuildDefinition(1, 2, 3).Descendants("SourceScope").Single();
		Assert.AreEqual((long)scope.Attribute("silt")!, (long)scope.Attribute("shadow")!);
	}

	[DataTestMethod, DataRow("empty"), DataRow("zero"), DataRow("negative"), DataRow("duplicate"), DataRow("too many")]
	public void WaterContribution_InvalidIdShapeRefuses(string scenario)
	{
		long[] ids = scenario switch { "empty" => [], "zero" => [0], "negative" => [-1], "duplicate" => [201, 201], _ => Enumerable.Range(1, 129).Select(x => (long)x).ToArray() };
		Assert.ThrowsException<ArgumentException>(() => ArmageddonReviewedWaterSeeContent.WaterBreathing(ids));
	}

	[DataTestMethod, DataRow("empty"), DataRow("four"), DataRow("six"), DataRow("zero"), DataRow("negative"), DataRow("duplicate")]
	public void SeeContribution_InvalidRankShapeRefuses(string scenario)
	{
		long[] ids = scenario switch { "empty" => [], "four" => [1, 2, 3, 4], "six" => [1, 2, 3, 4, 5, 6], "zero" => [0, 2, 3, 4, 5], "negative" => [-1, 2, 3, 4, 5], _ => [1, 2, 3, 4, 4] };
		Assert.ThrowsException<ArgumentException>(() => ArmageddonReviewedWaterSeeContent.SeeTheUnbodied(301, 303, ids));
	}

	[DataTestMethod, DataRow(0L, 303L), DataRow(-1L, 303L), DataRow(301L, 0L), DataRow(301L, -1L)]
	public void SeeContribution_NonpositiveTerrainIdsRefuse(long silt, long shadow) =>
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArmageddonReviewedWaterSeeContent.SeeTheUnbodied(silt, shadow, [1, 2, 3, 4, 5]));

	[TestMethod]
	public void PureContributions_NullListsRefuse()
	{
		Assert.ThrowsException<ArgumentNullException>(() => ArmageddonReviewedWaterSeeContent.WaterBreathing(null!));
		Assert.ThrowsException<ArgumentNullException>(() => ArmageddonReviewedWaterSeeContent.SeeTheUnbodied(301, 303, null!));
	}
}
