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
	public void SignedLanguageQuestion_IsIndependentAndModernOnly()
	{
		var question = new CultureSeeder().SeederQuestions.Single(x => x.Id == "seedsignedlanguages");
		Assert.IsTrue(question.Filter(null!, new Dictionary<string, string> { ["culturepacks"] = "Earth-Modern" }));
		Assert.IsFalse(question.Filter(null!, new Dictionary<string, string> { ["culturepacks"] = "Earth-Antiquity" }));
		Assert.IsTrue(question.Validator("yes", null!).Success);
		Assert.IsFalse(question.Validator("maybe", null!).Success);
	}
}
