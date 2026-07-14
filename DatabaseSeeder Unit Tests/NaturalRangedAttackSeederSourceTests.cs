#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NaturalRangedAttackSeederSourceTests
{
	[TestMethod]
	public void AnimalSeeder_EnsuresNaturalRangedCatalogueForFreshAndRepeatInstalls()
	{
		string source = ReadSource("DatabaseSeeder", "Seeders", "AnimalSeeder.cs");

		StringAssert.Contains(source, "EnsureNaturalRangedAttackSeedData(damageExpressions);");
		StringAssert.Contains(source, "_attacks[\"llamaspit\"]");
		StringAssert.Contains(source, "_attacks[\"acidspit\"]");
		StringAssert.Contains(source, "_attacks[\"dragonfirebreath\"]");
		StringAssert.Contains(source, "_attacks[\"wingbuffet\"]");
		StringAssert.Contains(source, "_attacks[\"tailspike\"]");
		StringAssert.Contains(source, "_attacks[\"bombardierspray\"]");
	}

	[TestMethod]
	public void AnimalSeeder_AcidAndDragonfireCarryTheirRuntimePayloadMetadata()
	{
		string source = ReadSource("DatabaseSeeder", "Seeders", "AnimalSeeder.cs");

		StringAssert.Contains(source, "animalAcid.SurfaceReactionInfo = new XElement(\"Reactions\"");
		StringAssert.Contains(source, "new XAttribute(\"DamageType\", (int)DamageType.Chemical)");
		StringAssert.Contains(source, "new XElement(\"Tag\", animalSkinTag.Id)");
		StringAssert.Contains(source, "new XElement(\"ExtinguishTags\", new XElement(\"Tag\", waterTag.Id))");
		StringAssert.Contains(source, "new XElement(\"OffensiveAdvantagePerDegree\", 0.1)");
		StringAssert.Contains(source, "new XElement(\"DefensiveAdvantagePerDegree\", 0.15)");
		StringAssert.Contains(source, "new XElement(\"InflictsDamage\", true)");
		StringAssert.Contains(source, "_attacks[\"dragonfirebreath\"].AdditionalInfo = breathXml.ToString();");
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
