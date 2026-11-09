#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class IndustrialisedItemCatalogueTests
{

	[TestMethod]
	public void TechnologyProfileAnswers_NormalizeSelectionsAndCustomValues()
	{
		var normalized = new ItemSeeder().NormalizeAnswers(new Dictionary<string, string>
		{
			["eras"] = "revolution medieval industrial",
			["technologyprofile"] = " Custom ",
			["technologypower"] = "Zulu, alpha, zulu",
			["technologypaper"] = "Letter, A4"
		});
		Assert.AreEqual("medieval industrial", normalized["eras"]);
		Assert.AreEqual("custom", normalized["technologyprofile"]);
		Assert.AreEqual("alpha, Zulu", normalized["technologypower"]);
		Assert.AreEqual("A4, Letter", normalized["technologypaper"]);
	}
}
