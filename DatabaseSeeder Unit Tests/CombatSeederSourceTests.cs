using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.GameItems;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatSeederSourceTests
{
	private sealed record CombatAttackSeed(
		string Name,
		string MoveType,
		string AttackerDifficulty,
		double Stamina,
		double Delay,
		string WeaponVariable,
		string DamageVariable
	);

	[TestMethod]
	public void LiveWeaponSuites_ExpectedChassis_HaveRequiredMoveCoverage()
	{
		var attacks = ParseCombatSeederAttacks();
		var requiredMoves = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
		{
			["knife"] = ["UseWeaponAttack", "ClinchAttack", "MeleeWeaponSmashItem", "CoupDeGrace"],
			["dagger"] = ["UseWeaponAttack", "ClinchAttack", "MeleeWeaponSmashItem", "CoupDeGrace"],
			["club"] = ["UseWeaponAttack", "ClinchAttack", "MeleeWeaponSmashItem", "CoupDeGrace"],
			["mace"] = ["UseWeaponAttack", "ClinchAttack", "MeleeWeaponSmashItem", "CoupDeGrace"],
			["improvised"] = ["UseWeaponAttack", "ClinchAttack", "MeleeWeaponSmashItem", "CoupDeGrace"],
			["shortsword"] = ["UseWeaponAttack", "ClinchAttack", "MeleeWeaponSmashItem", "CoupDeGrace"],
			["longsword"] = ["UseWeaponAttack", "ClinchAttack", "WardFreeAttack", "MeleeWeaponSmashItem", "CoupDeGrace"],
			["zweihander"] = ["UseWeaponAttack", "WardFreeAttack", "DownedAttack", "MeleeWeaponSmashItem", "CoupDeGrace"],
			["rapier"] = ["UseWeaponAttack", "ClinchAttack", "WardFreeAttack", "MeleeWeaponSmashItem", "CoupDeGrace"],
			["axe"] = ["UseWeaponAttack", "MeleeWeaponSmashItem", "CoupDeGrace"],
			["halberd"] = ["UseWeaponAttack", "UnbalancingBlow", "WardFreeAttack", "MeleeWeaponSmashItem", "CoupDeGrace"],
			["spear"] = ["UseWeaponAttack", "UnbalancingBlow", "WardFreeAttack", "MeleeWeaponSmashItem", "CoupDeGrace"],
			["longspear"] = ["UseWeaponAttack", "UnbalancingBlow", "WardFreeAttack", "MeleeWeaponSmashItem", "CoupDeGrace"],
			["mattock"] = ["UseWeaponAttack", "ClinchAttack", "DownedAttack", "CoupDeGrace"],
			["warhammer"] = ["StaggeringBlow", "ClinchAttack", "MeleeWeaponSmashItem", "DownedAttack", "CoupDeGrace"],
			["shield"] = ["UseWeaponAttack", "StaggeringBlow", "DownedAttack", "CoupDeGrace"]
		};

		var failures = new List<string>();
		foreach (var (weaponVariable, required) in requiredMoves)
		{
			var actualMoves = attacks
				.Where(x => x.WeaponVariable.Equals(weaponVariable, StringComparison.OrdinalIgnoreCase))
				.Select(x => x.MoveType)
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToHashSet(StringComparer.OrdinalIgnoreCase);

			var missing = required.Where(x => !actualMoves.Contains(x)).ToList();
			if (missing.Count > 0)
			{
				failures.Add(
					$"{weaponVariable} is missing {string.Join(", ", missing)}. Actual: {string.Join(", ", actualMoves.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))}");
			}
		}

		Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures));
	}

	[TestMethod]
	public void TrainingWeaponSuites_MirrorLiveSuites_WithoutBecomingBroaderOrMoreDamaging()
	{
		var attacks = ParseCombatSeederAttacks();
		var trainingPairs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			["knife"] = "trainingKnife",
			["dagger"] = "trainingDagger",
			["club"] = "trainingClub",
			["mace"] = "trainingmace",
			["shortsword"] = "trainingshortsword",
			["longsword"] = "traininglongsword",
			["zweihander"] = "trainingzweihander",
			["rapier"] = "trainingrapier",
			["axe"] = "trainingaxe",
			["halberd"] = "trainingHalberd",
			["spear"] = "trainingspear",
			["mattock"] = "trainingmattock",
			["warhammer"] = "trainingwarhammer"
		};

		var failures = new List<string>();
		foreach (var (liveWeaponVariable, trainingWeaponVariable) in trainingPairs)
		{
			var liveAttacks = attacks
				.Where(x => x.WeaponVariable.Equals(liveWeaponVariable, StringComparison.OrdinalIgnoreCase))
				.ToList();
			var trainingAttacks = attacks
				.Where(x => x.WeaponVariable.Equals(trainingWeaponVariable, StringComparison.OrdinalIgnoreCase))
				.ToList();

			var liveMoves = liveAttacks
				.Select(x => x.MoveType)
				.ToHashSet(StringComparer.OrdinalIgnoreCase);
			var extraTrainingMoves = trainingAttacks
				.Select(x => x.MoveType)
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.Where(x => !liveMoves.Contains(x))
				.ToList();
			if (extraTrainingMoves.Count > 0)
			{
				failures.Add($"{trainingWeaponVariable} introduces extra move types: {string.Join(", ", extraTrainingMoves)}.");
			}

			foreach (var trainingAttack in trainingAttacks)
			{
				if (!trainingAttack.DamageVariable.Equals("trainingDamage", StringComparison.OrdinalIgnoreCase))
				{
					failures.Add($"{trainingAttack.Name} should use trainingDamage but uses {trainingAttack.DamageVariable}.");
				}

				var liveName = trainingAttack.Name.Replace("Training ", "", StringComparison.Ordinal);
				var liveAttack = liveAttacks.FirstOrDefault(x =>
					x.Name.Equals(liveName, StringComparison.OrdinalIgnoreCase));
				if (liveAttack is null)
				{
					failures.Add($"{trainingAttack.Name} has no live counterpart in {liveWeaponVariable}.");
				}
			}
		}

		Assert.AreEqual(0, failures.Count, string.Join(Environment.NewLine, failures));
	}

	[TestMethod]
	public void WeaponBalanceBands_KeyWeaponFamilies_TrackIntendedSpeedAndDamageRoles()
	{
		var attacks = ParseCombatSeederAttacks();

		var rapierPrimaryStamina = AverageStamina(attacks, "rapier", ["UseWeaponAttack", "WardFreeAttack"]);
		var spearPrimaryStamina = AverageStamina(attacks, "spear", ["UseWeaponAttack", "WardFreeAttack"]);
		var warhammerPrimaryStamina = AverageStamina(attacks, "warhammer", ["StaggeringBlow"]);

		Assert.IsTrue(rapierPrimaryStamina < spearPrimaryStamina,
			$"Rapiers should be lighter on stamina than spears ({rapierPrimaryStamina} vs {spearPrimaryStamina}).");
		Assert.IsTrue(spearPrimaryStamina < warhammerPrimaryStamina,
			$"Spears should be lighter on stamina than warhammers ({spearPrimaryStamina} vs {warhammerPrimaryStamina}).");

		var improvisedPrimaryDelay = AverageDelay(attacks, "improvised", ["UseWeaponAttack"]);
		var clubPrimaryDelay = AverageDelay(attacks, "club", ["UseWeaponAttack", "StaggeringBlow"]);
		Assert.IsTrue(improvisedPrimaryDelay > clubPrimaryDelay,
			$"Improvised weapons should swing more slowly than clubs ({improvisedPrimaryDelay} vs {clubPrimaryDelay}).");

		var improvisedPrimaryPeakDamage = PeakDamageRank(attacks, "improvised", ["UseWeaponAttack"]);
		var clubPrimaryPeakDamage = PeakDamageRank(attacks, "club", ["UseWeaponAttack", "StaggeringBlow"]);
		Assert.IsTrue(improvisedPrimaryPeakDamage <= clubPrimaryPeakDamage,
			$"Improvised weapons should not out-damage clubs in their primary attacks ({improvisedPrimaryPeakDamage} vs {clubPrimaryPeakDamage}).");
	}

	[TestMethod]
	public void WeaponDamageBands_StandardWeapons_MapToExpectedSeverityBands()
	{
		const double nominalStrength = 10.0;
		const double degree = 0.0;
		const double staticModeStartingMultiplier = 0.6;
		var standardQuality = (int)ItemQuality.Standard;

		var terribleDamage = RepresentativeStaticDamage(0.1, nominalStrength, standardQuality, degree,
			staticModeStartingMultiplier);
		var badDamage = RepresentativeStaticDamage(0.2, nominalStrength, standardQuality, degree,
			staticModeStartingMultiplier);
		var poorDamage = RepresentativeStaticDamage(0.25, nominalStrength, standardQuality, degree,
			staticModeStartingMultiplier);
		var normalDamage = RepresentativeStaticDamage(0.3, nominalStrength, standardQuality, degree,
			staticModeStartingMultiplier);
		var goodDamage = RepresentativeStaticDamage(0.4, nominalStrength, standardQuality, degree,
			staticModeStartingMultiplier);
		var greatDamage = RepresentativeStaticDamage(0.5, nominalStrength, standardQuality, degree,
			staticModeStartingMultiplier);
		var coupDeGraceDamage = RepresentativeStaticDamage(1.0, nominalStrength, standardQuality, degree,
			staticModeStartingMultiplier);

		Assert.IsTrue(terribleDamage < 4.0,
			$"Terrible damage should stay in the light-wound bands for a standard weapon ({terribleDamage}).");
		Assert.IsTrue(badDamage < 7.0,
			$"Bad damage should stay below the severe thresholds for a standard weapon ({badDamage}).");
		Assert.IsTrue(poorDamage >= 7.0 && poorDamage < 12.0,
			$"Poor damage should land in the ordinary combat wound band ({poorDamage}).");
		Assert.IsTrue(normalDamage >= 7.0 && normalDamage < 12.0,
			$"Normal damage should stay in the expected ordinary-hit band ({normalDamage}).");
		Assert.IsTrue(goodDamage >= 12.0 && goodDamage < 18.0,
			$"Good damage should move into the heavier but non-exceptional band ({goodDamage}).");
		Assert.IsTrue(greatDamage >= 12.0 && greatDamage < 18.0,
			$"Great damage should remain dangerous without becoming coup-de-grace level ({greatDamage}).");
		Assert.IsTrue(coupDeGraceDamage >= 27.0,
			$"Coup de grace damage should sit in the exceptional lethality band ({coupDeGraceDamage}).");
	}

	private static double AverageStamina(IEnumerable<CombatAttackSeed> attacks, string weaponVariable,
		IReadOnlyCollection<string> moveTypes)
	{
		return attacks
			.Where(x => x.WeaponVariable.Equals(weaponVariable, StringComparison.OrdinalIgnoreCase))
			.Where(x => moveTypes.Contains(x.MoveType, StringComparer.OrdinalIgnoreCase))
			.Average(x => x.Stamina);
	}

	private static double AverageDelay(IEnumerable<CombatAttackSeed> attacks, string weaponVariable,
		IReadOnlyCollection<string> moveTypes)
	{
		return attacks
			.Where(x => x.WeaponVariable.Equals(weaponVariable, StringComparison.OrdinalIgnoreCase))
			.Where(x => moveTypes.Contains(x.MoveType, StringComparer.OrdinalIgnoreCase))
			.Average(x => x.Delay);
	}

	private static int PeakDamageRank(IEnumerable<CombatAttackSeed> attacks, string weaponVariable,
		IReadOnlyCollection<string> moveTypes)
	{
		return attacks
			.Where(x => x.WeaponVariable.Equals(weaponVariable, StringComparison.OrdinalIgnoreCase))
			.Where(x => moveTypes.Contains(x.MoveType, StringComparer.OrdinalIgnoreCase))
			.Max(x => DamageRank(x.DamageVariable));
	}

	private static int DamageRank(string damageVariable)
	{
		return damageVariable switch
		{
			"trainingDamage" => 0,
			"terribleDamage" => 1,
			"badDamage" => 2,
			"poorDamage" => 3,
			"normalDamage" => 4,
			"goodDamage" => 5,
			"veryGoodDamage" => 6,
			"greatDamage" => 7,
			"coupdegraceDamage" => 8,
			_ => throw new InvalidOperationException($"Unexpected damage variable {damageVariable}.")
		};
	}

	private static double RepresentativeStaticDamage(double baseMultiplier, double strength, int quality, double degree,
		double startingMultiplier)
	{
		return Math.Max(1.0, baseMultiplier * startingMultiplier * (strength * quality) * Math.Sqrt(degree + 1.0));
	}

	private static IReadOnlyList<CombatAttackSeed> ParseCombatSeederAttacks()
	{
		var source = File.ReadAllText(GetCombatSeederSourcePath());
		var regex = new Regex(
			"""AddAttack\("(?<name>[^"]+)",\s*BuiltInCombatMoveType\.(?<move>[A-Za-z0-9_]+),\s*MeleeWeaponVerb\.[A-Za-z0-9_]+,\s*Difficulty\.(?<attacker>[A-Za-z0-9_]+),\s*Difficulty\.[A-Za-z0-9_]+,\s*Difficulty\.[A-Za-z0-9_]+,\s*Difficulty\.[A-Za-z0-9_]+,\s*Alignment\.[A-Za-z0-9_]+,\s*Orientation\.[A-Za-z0-9_]+,\s*(?<stamina>[0-9.]+),\s*(?<delay>[0-9.]+),\s*(?<weapon>[A-Za-z0-9_]+),\s*(?<damage>[A-Za-z0-9_]+)""",
			RegexOptions.Singleline | RegexOptions.CultureInvariant);

		return regex.Matches(source)
			.Select(x => new CombatAttackSeed(
				x.Groups["name"].Value,
				x.Groups["move"].Value,
				x.Groups["attacker"].Value,
				double.Parse(x.Groups["stamina"].Value, CultureInfo.InvariantCulture),
				double.Parse(x.Groups["delay"].Value, CultureInfo.InvariantCulture),
				x.Groups["weapon"].Value,
				x.Groups["damage"].Value))
			.ToList();
	}

	private static string GetCombatSeederSourcePath()
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			"DatabaseSeeder",
			"Seeders",
			"CombatSeeder.cs"));
	}
}
