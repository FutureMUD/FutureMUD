#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RacialDescriptionRewriteTests
{
	private const string ApprovedRewriteHash =
		"a672c9240cd52e5ba4d718e06211b4f7568c15a86f9340dae6ef8002fe0b177c";

	[TestMethod]
	public void SeededRaceDescriptions_MatchApprovedRewriteSet()
	{
		var descriptions = GetSeededRaceDescriptions().ToList();

		Assert.AreEqual(305, descriptions.Count);
		Assert.AreEqual(201, descriptions.Count(x => x.Seeder == "Animal Seeder"));
		Assert.AreEqual(43, descriptions.Count(x => x.Seeder == "Mythical Animal Seeder"));
		Assert.AreEqual(15, descriptions.Count(x => x.Seeder == "Robot Seeder"));
		Assert.AreEqual(46, descriptions.Count(x => x.Seeder == "Supernatural Seeder"));
		Assert.IsFalse(descriptions.Any(x => string.IsNullOrWhiteSpace(x.Description)));
		Assert.IsTrue(descriptions.All(x => ParagraphCount(x.Description) == 3));

		var canonical = new StringBuilder();
		foreach (var row in descriptions.OrderBy(x => x.Seeder, StringComparer.Ordinal)
			         .ThenBy(x => x.Race, StringComparer.Ordinal))
		{
			canonical
				.Append(row.Seeder)
				.Append('\t')
				.Append(row.Race)
				.Append('\t')
				.Append(NormaliseLineEndings(row.Description))
				.Append('\n');
		}

		var hash = Convert
			.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical.ToString())))
			.ToLowerInvariant();
		Assert.AreEqual(ApprovedRewriteHash, hash);
	}

	private static IEnumerable<(string Seeder, string Race, string Description)> GetSeededRaceDescriptions()
	{
		foreach ((var raceName, var template) in AnimalSeeder.RaceTemplatesForTesting)
		{
			yield return ("Animal Seeder", raceName, AnimalSeeder.BuildRaceDescriptionForTesting(template));
		}

		foreach ((var raceName, var template) in MythicalAnimalSeeder.TemplatesForTesting)
		{
			yield return ("Mythical Animal Seeder", raceName, MythicalAnimalSeeder.BuildRaceDescriptionForTesting(template));
		}

		foreach ((var raceName, var template) in RobotSeeder.TemplatesForTesting)
		{
			yield return ("Robot Seeder", raceName, RobotSeeder.BuildRaceDescriptionForTesting(template));
		}

		foreach ((var raceName, var template) in SupernaturalSeeder.TemplatesForTesting)
		{
			yield return ("Supernatural Seeder", raceName, SupernaturalSeeder.BuildRaceDescriptionForTesting(template));
		}
	}

	private static string NormaliseLineEndings(string text)
	{
		return text
			.Replace("\r\n", "\n", StringComparison.Ordinal)
			.Replace("\r", "\n", StringComparison.Ordinal);
	}

	private static int ParagraphCount(string text)
	{
		return NormaliseLineEndings(text)
			.Split(["\n\n"], StringSplitOptions.RemoveEmptyEntries)
			.Length;
	}
}
