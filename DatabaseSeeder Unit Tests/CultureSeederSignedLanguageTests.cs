#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureSeederSignedLanguageTests
{
	[TestMethod]
	public void ModernSignedLanguageCatalogue_HasExpectedIndependentCoverage()
	{
		var names = CultureSeeder.ModernSignedLanguageNamesForTesting;
		Assert.AreEqual(24, names.Count);
		Assert.AreEqual(names.Count, names.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		CollectionAssert.Contains(names.ToArray(), "American Sign Language");
		CollectionAssert.Contains(names.ToArray(), "British Sign Language");
		CollectionAssert.Contains(names.ToArray(), "Auslan");
		CollectionAssert.Contains(names.ToArray(), "New Zealand Sign Language");
	}

	[TestMethod]
	public void BritishSignLanguageCatalogue_HasEightRegionalVarieties()
	{
		var varieties = CultureSeeder.BritishSignLanguageRegionalVarietiesForTesting;
		Assert.AreEqual(8, varieties.Count);
		CollectionAssert.AreEquivalent(
			new[] { "Belfast", "Birmingham", "Bristol", "Cardiff", "Glasgow", "London", "Manchester", "Newcastle" },
			varieties.ToArray());
	}

	[TestMethod]
	public void SignedLanguageQuestion_IsIndependentAndModernOnly()
	{
		var question = new CultureSeeder().SeederQuestions.Single(x => x.Id == "seedsignedlanguages");
		Assert.IsTrue(question.Filter(null!, new Dictionary<string, string> { ["culturepacks"] = "Earth-Modern" }));
		Assert.IsFalse(question.Filter(null!, new Dictionary<string, string> { ["culturepacks"] = "Earth-Antiquity" }));
		Assert.IsTrue(question.Validator("yes", null!).Success);
		Assert.IsFalse(question.Validator("maybe", null!).Success);
	}
}
