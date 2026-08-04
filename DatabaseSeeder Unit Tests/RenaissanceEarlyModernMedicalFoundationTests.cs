#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RenaissanceEarlyModernMedicalFoundationTests
{
	[TestMethod]
	public void HealthSeeder_ExposesHistoricalTiersAndFumigationPrerequisite()
	{
		var source = Read("DatabaseSeeder", "Seeders", "HealthSeeder.cs");
		StringAssert.Contains(source, "\"renaissance\" or \"earlymodern\" when TagPathExists");
		StringAssert.Contains(source, "\"renaissance\" when !context.Gases.Any");
		StringAssert.Contains(source, "ExpectedMedicalGasBindingsForTier");
		StringAssert.Contains(source, "SeedRenaissanceDrugs()");
		StringAssert.Contains(source, "SeedEarlyModernDrugs()");
		StringAssert.Contains(source, "Benzoin Fumigation Smoke");
		StringAssert.Contains(source, "Hartshorn Vapour");
	}

	[TestMethod]
	public void MaintainedExports_ContainMedicalFoundationsWithoutDuplicates()
	{
		AssertJsonNames("Seeded_Materials.json", "Material Name", "benzoin resin", "camphor", "cinchona bark", "myrrh resin", "opium gum", "Peruvian balsam", "Epsom salts", "calomel", "tartar emetic", "bezoar", "ergot", "green vitriol");
		AssertJsonNames("Seeded_Gases.json", "Gas Name", "Benzoin Fumigation Smoke", "Hartshorn Vapour");
		AssertJsonNames("Seeded_Liquids.json", "Liquid Name", "opium tincture", "dover's powder draught", "tartar emetic solution", "calomel draught", "godfrey's cordial");
		AssertJsonNames("Seeded_Item_Components.json", "Component Name",
			"Pill_Opium_Tincture", "TopicalCream_Aqua_Vitae_Wash", "Smokeable_Benzoin_Fumigant",
			"LContainer_Medicine_Opium_Tincture_100ml", "LContainer_Medicine_Dover_s_Powder_100ml",
			"Repair_Clockwork", "Repair_Firearm", "Repair_Medical_Instrument", "Repair_Optical_Instrument",
			"Repair_Printing_Equipment", "Repair_Scientific_Instrument");
		var tags = File.ReadAllLines(Path("Design Documents", "Data", "SeededTagHierarchy.csv"));
		foreach (var path in new[] { "Functions / Medical Treatment / Chemical Remedy", "Functions / Repairing / Clockwork", "Market / Medicine / Chemical Medicine", "Market / Repair Supplies / Precision Repair Supplies" }) Assert.IsTrue(tags.Any(x => x.EndsWith(path, StringComparison.Ordinal)));
	}

	private static void AssertJsonNames(string file, string property, params string[] expected)
	{
		using var document = JsonDocument.Parse(Read("Design Documents", "Data", file));
		var names = document.RootElement.EnumerateArray().Select(x => x.GetProperty(property).GetString()!).ToArray();
		Assert.AreEqual(names.Length, names.Distinct(StringComparer.OrdinalIgnoreCase).Count());
		foreach (var name in expected) CollectionAssert.Contains(names, name);
	}

	private static string Read(params string[] parts) => File.ReadAllText(Path(parts));
	private static string Path(params string[] parts) => System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", System.IO.Path.Combine(parts)));
}
