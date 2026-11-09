#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleSeederTests
{

	[TestMethod]
	public void VehicleExampleCatalogue_PassesAuthoringValidation()
	{
		ItemSeeder.ValidateVehicleExamplesForTesting();
	}

	[TestMethod]
	public void VehicleEraParser_NormalisesFiltersAndDeduplicates()
	{
		var parsed = ItemSeeder.ParseVehicleEraTokensForTesting(
			"Antiquity, medieval medieval earlymodern unknown industrial revolution nuclear atomic information COMPUTER");
		CollectionAssert.AreEquivalent(
			new[] { "antiquity", "medieval", "earlymodern", "revolution", "atomic", "computer" },
			parsed.ToArray());
		Assert.AreEqual(0, ItemSeeder.ParseVehicleEraTokensForTesting(null).Count);
		Assert.AreEqual(0, ItemSeeder.ParseVehicleEraTokensForTesting("unknown").Count);
	}
}
