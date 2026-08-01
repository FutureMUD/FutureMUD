#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Grouping;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MedicalOutputRegressionTests
{
	[TestMethod]
	public void DescribeGroups_IgnoresSuccessfulRulesThatMatchedNoBodyparts()
	{
		var thigh = new Mock<IBodypart>();
		var emptyRule = new Mock<IBodypartGroupDescriber>();
		emptyRule.Setup(x => x.Match(It.IsAny<IEnumerable<IBodypart>>()))
		         .Returns(new BodypartGroupResult(true, 100, "head", [], [thigh.Object]));
		var thighRule = new Mock<IBodypartGroupDescriber>();
		thighRule.Setup(x => x.Match(It.IsAny<IEnumerable<IBodypart>>()))
		         .Returns(new BodypartGroupResult(true, 50, "right thigh", [thigh.Object], []));

		var result = BodypartGroupDescriber.DescribeGroups(
			new[] { emptyRule.Object, thighRule.Object },
			new[] { thigh.Object });

		Assert.AreEqual("right thigh", result);
	}

	[TestMethod]
	public void MedicalPlayerOutput_DoesNotContainObservedGrammarAndDescriptionDefects()
	{
		var healthModule = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules", "HealthModule.cs"));
		var cleaning = File.ReadAllText(GetSourcePath("MudSharpCore", "Effects", "Concrete", "CleaningWounds.cs"));
		var ivBag = File.ReadAllText(GetSourcePath("MudSharpCore", "GameItems", "Components", "IVBagGameItemComponent.cs"));
		var wearable = File.ReadAllText(GetSourcePath("MudSharpCore", "GameItems", "Components", "WearableGameItemComponent.cs"));
		var fracture = File.ReadAllText(GetSourcePath("MudSharpCore", "Health", "Wounds", "BoneFracture.cs"));

		Assert.IsFalse(healthModule.Contains("can't see any wounds of", StringComparison.Ordinal));
		StringAssert.Contains(healthModule, "@ begin|begins tending to @'s wounds.");
		StringAssert.Contains(cleaning, "You require antiseptics to clean your wounds any further.");
		StringAssert.Contains(ivBag, "{LiquidMixture.ColouredLiquidDescription} begins to flow into @");
		StringAssert.Contains(wearable, "Profiles.Select(x => x.DesignedBody).Distinct()");
		Assert.IsFalse(fracture.Contains("The pain levels in your", StringComparison.Ordinal));
	}

	[TestMethod]
	public void DefibrillatorOutput_DeduplicatesObstructionsAndReportsShockResult()
	{
		var source = File.ReadAllText(GetSourcePath("MudSharpCore", "GameItems", "Components",
			"DefibrillatorGameItemComponent.cs"));

		StringAssert.Contains(source, ".Distinct()");
		StringAssert.Contains(source, "You are conscious and therefore not in need of defibrillation.");
		StringAssert.Contains(source, "reports that the heart rhythm has stabilised.");
		StringAssert.Contains(source, "reports no effective change in heart rhythm.");
	}

	private static string GetSourcePath(params string[] parts)
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts)));
	}
}
