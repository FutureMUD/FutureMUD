#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RenaissanceEarlyModernDependencyFoundationTests
{
	private static readonly (string Gerund, string Imperative)[] SpecialistSkills =
	[
		("Movable Type Printing", "Printer"),
		("Gunsmithing", "Gunsmith"),
		("Powdermaking", "Powdermaker"),
		("Lensmaking", "Lensmaker"),
		("Clockmaking", "Clockmaker"),
		("Instrument Making", "Instrument Maker"),
		("Navigation", "Navigator"),
		("Cartography", "Cartographer"),
		("Surveying", "Surveyor"),
		("Engraving", "Engraver")
	];

	private static readonly string[] FoundationMaterials =
	[
		"taffeta", "ribbon", "calico", "chintz", "logwood", "cochineal", "tobacco leaf", "type metal",
		"printing ink", "molasses", "sugar loaf", "tobacco twist", "snuff", "roasted coffee", "cacao bean",
		"cacao nibs", "chocolate block", "tea brick", "cotton fibre", "glass blank", "indigo dye cake"
	];

	[TestMethod]
	public void SkillPackageSeeder_ProvidesEraSpecialistSkillsInBothNamingModes()
	{
		var source = ReadSource("DatabaseSeeder", "Seeders", "SkillPackageSeeder.cs");
		var nonGerundNames = new SkillPackageSeeder().ComplexNonGerundSkillNamesForTesting;

		foreach (var (gerund, imperative) in SpecialistSkills)
		{
			Assert.IsTrue(source.Contains($"new SkillDetails(\"{gerund}\", \"{imperative}\"", StringComparison.Ordinal),
				$"SkillPackageSeeder should seed the {gerund}/{imperative} skill pair.");
			Assert.IsTrue(nonGerundNames.Contains(imperative),
				$"The non-gerund stock skill package should expose {imperative}.");
		}
	}

	[TestMethod]
	public void SeededMaterialsCatalogue_ContainsEraFoundationMaterialsWithoutDuplicates()
	{
		using var document = JsonDocument.Parse(ReadSource("Design Documents", "Data", "Seeded_Materials.json"));
		var names = document.RootElement
			.EnumerateArray()
			.Select(x => x.GetProperty("Material Name").GetString()!)
			.ToArray();

		Assert.AreEqual(names.Length, names.Distinct(StringComparer.InvariantCultureIgnoreCase).Count(),
			"Seeded_Materials.json should not contain duplicate material names.");
		foreach (var material in FoundationMaterials)
		{
			Assert.IsTrue(names.Contains(material, StringComparer.InvariantCultureIgnoreCase),
				$"Seeded_Materials.json should contain {material}.");
		}
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts))));
	}
}
