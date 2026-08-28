#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed record StockNPCSkillPackageRace(
	string RaceName,
	bool CanFly,
	bool CanSwim,
	bool CanClimb,
	NonHumanCombatTier CombatTier);

internal sealed record NPCSkillPackageSeedResult(int PackagesChanged, int RaceLinksAdded)
{
	internal bool HasChanges => PackagesChanged > 0 || RaceLinksAdded > 0;
}

internal static class NPCSkillPackageSeederHelper
{
	internal static readonly string[] StockPackageNames =
	[
		"Universal Common",
		"Flying Race",
		"Swimming Race",
		"Climbing Race",
		"Low-Level Beast Attacker",
		"Competent Beast Attacker",
		"Dangerous Beast Attacker",
		"Terrifying Beast Attacker",
		"Apex Beast Attacker"
	];

	internal static NPCSkillPackageSeedResult EnsureUniversalPackage(
		FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, TraitDefinition>? seededSkills = null)
	{
		var utilitySkills = ResolveDistinctSkills(context, seededSkills,
			["Listening", "Listen", "Perception"],
			["Searching", "Search", "Perception"],
			["Spotting", "Spot", "Scan", "Perception"],
			["Climbing", "Climb", "Athletics"],
			["Balancing", "Balance", "Athletics"],
			["Running", "Run", "Athletics"]);

		var changed = UpsertPackage(context, "Universal Common",
			utilitySkills.Select(x => (x, 1.0, 25.0, 5.0, 0.0)));
		return new NPCSkillPackageSeedResult(changed ? 1 : 0, 0);
	}

	internal static NPCSkillPackageSeedResult EnsureCombatPackages(FuturemudDatabaseContext context)
	{
		var combatSkills = ResolveDistinctSkills(context, null,
			["Brawling", "Brawl"],
			["Dodging", "Dodge"]);
		if (combatSkills.Count == 0)
		{
			return new NPCSkillPackageSeedResult(0, 0);
		}

		var packages = new (string Name, double Mean)[]
		{
			("Low-Level Beast Attacker", 25.0),
			("Competent Beast Attacker", 45.0),
			("Dangerous Beast Attacker", 65.0),
			("Terrifying Beast Attacker", 75.0),
			("Apex Beast Attacker", 85.0)
		};
		var changed = packages.Count(package => UpsertPackage(context, package.Name,
			combatSkills.Select(x => (x, 1.0, package.Mean, Math.Max(5.0, package.Mean * 0.10), 0.0))));
		return new NPCSkillPackageSeedResult(changed, 0);
	}

	internal static NPCSkillPackageSeedResult EnsureRacePackages(
		FuturemudDatabaseContext context,
		IEnumerable<StockNPCSkillPackageRace> races)
	{
		var packagesChanged = 0;
		var fly = ResolveDistinctSkills(context, null, ["Flying", "Fly", "Athletics"]);
		var swim = ResolveDistinctSkills(context, null, ["Swimming", "Swim", "Athletics"]);
		var climb = ResolveDistinctSkills(context, null, ["Climbing", "Climb", "Athletics"]);
		packagesChanged += fly.Count > 0 && UpsertPackage(context, "Flying Race",
			fly.Select(x => (x, 1.0, 35.0, 7.5, 0.0))) ? 1 : 0;
		packagesChanged += swim.Count > 0 && UpsertPackage(context, "Swimming Race",
			swim.Select(x => (x, 1.0, 35.0, 7.5, 0.0))) ? 1 : 0;
		packagesChanged += climb.Count > 0 && UpsertPackage(context, "Climbing Race",
			climb.Select(x => (x, 1.0, 35.0, 7.5, 0.0))) ? 1 : 0;
		packagesChanged += EnsureCombatPackages(context).PackagesChanged;

		var packages = context.NpcSkillPackages
			.Where(x => StockPackageNames.Contains(x.Name))
			.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		var linksAdded = 0;
		foreach (var stockRace in races.DistinctBy(x => x.RaceName, StringComparer.OrdinalIgnoreCase))
		{
			var race = context.Races.FirstOrDefault(x => x.Name == stockRace.RaceName);
			if (race is null)
			{
				continue;
			}

			context.Entry(race).Collection(x => x.NpcSkillPackages).Load();
			var requiredPackages = new List<string>();
			if (stockRace.CanFly && fly.Count > 0)
			{
				requiredPackages.Add("Flying Race");
			}

			if (stockRace.CanSwim && swim.Count > 0)
			{
				requiredPackages.Add("Swimming Race");
			}

			if (stockRace.CanClimb && climb.Count > 0)
			{
				requiredPackages.Add("Climbing Race");
			}

			requiredPackages.Add(CombatPackageFor(stockRace));
			foreach (var name in requiredPackages.Distinct(StringComparer.OrdinalIgnoreCase))
			{
				if (!packages.TryGetValue(name, out var package) || race.NpcSkillPackages.Any(x => x.Id == package.Id))
				{
					continue;
				}

				race.NpcSkillPackages.Add(package);
				linksAdded++;
			}
		}

		context.SaveChanges();
		return new NPCSkillPackageSeedResult(packagesChanged, linksAdded);
	}

	internal static bool HasMissingUniversalPackage(FuturemudDatabaseContext context)
	{
		var utilitySkills = ResolveDistinctSkills(context, null,
			["Listening", "Listen", "Perception"],
			["Searching", "Search", "Perception"],
			["Spotting", "Spot", "Scan", "Perception"],
			["Climbing", "Climb", "Athletics"],
			["Balancing", "Balance", "Athletics"],
			["Running", "Run", "Athletics"]);
		return !PackageMatches(context, "Universal Common", utilitySkills, 1.0, 25.0, 5.0, 0.0);
	}

	internal static bool HasMissingStockRacePackages(FuturemudDatabaseContext context,
		IEnumerable<StockNPCSkillPackageRace> races)
	{
		var fly = ResolveDistinctSkills(context, null, ["Flying", "Fly", "Athletics"]);
		var swim = ResolveDistinctSkills(context, null, ["Swimming", "Swim", "Athletics"]);
		var climb = ResolveDistinctSkills(context, null, ["Climbing", "Climb", "Athletics"]);
		if (fly.Count > 0 && !PackageMatches(context, "Flying Race", fly, 1.0, 35.0, 7.5, 0.0) ||
		    swim.Count > 0 && !PackageMatches(context, "Swimming Race", swim, 1.0, 35.0, 7.5, 0.0) ||
		    climb.Count > 0 && !PackageMatches(context, "Climbing Race", climb, 1.0, 35.0, 7.5, 0.0) ||
		    HasMissingCombatPackages(context))
		{
			return true;
		}

		var packages = context.NpcSkillPackages
			.Where(x => StockPackageNames.Contains(x.Name))
			.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		foreach (var stockRace in races.DistinctBy(x => x.RaceName, StringComparer.OrdinalIgnoreCase))
		{
			var race = context.Races
				.Include(x => x.NpcSkillPackages)
				.FirstOrDefault(x => x.Name == stockRace.RaceName);
			if (race is null)
			{
				continue;
			}

			var requiredPackages = new List<string>();
			if (stockRace.CanFly && fly.Count > 0)
			{
				requiredPackages.Add("Flying Race");
			}

			if (stockRace.CanSwim && swim.Count > 0)
			{
				requiredPackages.Add("Swimming Race");
			}

			if (stockRace.CanClimb && climb.Count > 0)
			{
				requiredPackages.Add("Climbing Race");
			}

			requiredPackages.Add(CombatPackageFor(stockRace));
			if (requiredPackages
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.Any(name => !packages.TryGetValue(name, out var package) ||
				             race.NpcSkillPackages.All(x => x.Id != package.Id)))
			{
				return true;
			}
		}

		return false;
	}

	internal static bool HasMissingCombatPackages(FuturemudDatabaseContext context)
	{
		var combatSkills = ResolveDistinctSkills(context, null,
			["Brawling", "Brawl"],
			["Dodging", "Dodge"]);
		if (combatSkills.Count == 0)
		{
			return false;
		}

		return new (string Name, double Mean)[]
		{
			("Low-Level Beast Attacker", 25.0),
			("Competent Beast Attacker", 45.0),
			("Dangerous Beast Attacker", 65.0),
			("Terrifying Beast Attacker", 75.0),
			("Apex Beast Attacker", 85.0)
		}.Any(package => !PackageMatches(context, package.Name, combatSkills, 1.0, package.Mean,
			Math.Max(5.0, package.Mean * 0.10), 0.0));
	}

	internal static string CombatPackageFor(StockNPCSkillPackageRace race)
	{
		if (race.RaceName.Equals("Warg", StringComparison.OrdinalIgnoreCase))
		{
			return "Terrifying Beast Attacker";
		}

		return race.CombatTier switch
		{
			NonHumanCombatTier.Nuisance or NonHumanCombatTier.MinorThreat => "Low-Level Beast Attacker",
			NonHumanCombatTier.SeriousThreat => "Competent Beast Attacker",
			NonHumanCombatTier.EliteThreat => "Dangerous Beast Attacker",
			NonHumanCombatTier.Monster or NonHumanCombatTier.GreatBeast => "Terrifying Beast Attacker",
			_ => "Apex Beast Attacker"
		};
	}

	internal static bool IsFlyingRace(string raceName, string bodyKey)
	{
		return bodyKey.Contains("Avian", StringComparison.OrdinalIgnoreCase) ||
		       bodyKey.Contains("Winged", StringComparison.OrdinalIgnoreCase) ||
		       bodyKey.Contains("Dragon", StringComparison.OrdinalIgnoreCase) ||
		       raceName.In("Pegasus", "Pegacorn", "Griffin", "Hippogriff", "Phoenix", "Cockatrice",
			       "Manticore", "Wyvern", "Fell Beast", "Garuda", "Giant Eagle", "Dragon",
			       "Eastern Dragon", "Qilin");
	}

	internal static NonHumanCombatTier TierForAnimal(SizeCategory size)
	{
		return size switch
		{
			<= SizeCategory.VerySmall => NonHumanCombatTier.Nuisance,
			SizeCategory.Small => NonHumanCombatTier.MinorThreat,
			SizeCategory.Normal => NonHumanCombatTier.SeriousThreat,
			SizeCategory.Large => NonHumanCombatTier.EliteThreat,
			SizeCategory.VeryLarge => NonHumanCombatTier.Monster,
			SizeCategory.Huge => NonHumanCombatTier.GreatBeast,
			SizeCategory.Enormous or SizeCategory.Gigantic => NonHumanCombatTier.PartyBoss,
			_ => NonHumanCombatTier.Avatar
		};
	}

	private static IReadOnlyList<TraitDefinition> ResolveDistinctSkills(
		FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, TraitDefinition>? seededSkills,
		params string[][] candidates)
	{
		var result = new List<TraitDefinition>();
		foreach (var names in candidates)
		{
			var skill = names
				.Select(name => seededSkills?.FirstOrDefault(x => x.Key.Equals(name,
					StringComparison.OrdinalIgnoreCase)).Value ??
					context.TraitDefinitions.AsEnumerable().FirstOrDefault(x =>
						x.Type == 0 && x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
				.FirstOrDefault(x => x is not null);
			if (skill is not null && result.All(x => x.Id != skill.Id))
			{
				result.Add(skill);
			}
		}

		return result;
	}

	private static bool UpsertPackage(
		FuturemudDatabaseContext context,
		string name,
		IEnumerable<(TraitDefinition Skill, double Chance, double Mean, double Deviation, double Skewness)> entries)
	{
		var expected = entries
			.DistinctBy(x => x.Skill.Id)
			.ToList();
		var package = context.NpcSkillPackages
			.Include(x => x.Skills)
			.FirstOrDefault(x => x.Name == name);
		var changed = false;
		if (package is null)
		{
			package = new NpcSkillPackage { Name = name };
			context.NpcSkillPackages.Add(package);
			changed = true;
		}

		var expectedIds = expected.Select(x => x.Skill.Id).ToHashSet();
		foreach (var stale in package.Skills.Where(x => !expectedIds.Contains(x.TraitDefinitionId)).ToList())
		{
			context.NpcSkillPackageSkills.Remove(stale);
			changed = true;
		}

		foreach (var entry in expected)
		{
			var existing = package.Skills.FirstOrDefault(x => x.TraitDefinitionId == entry.Skill.Id);
			if (existing is null)
			{
				package.Skills.Add(new NpcSkillPackageSkill
				{
					TraitDefinition = entry.Skill,
					Chance = entry.Chance,
					Mean = entry.Mean,
					StandardDeviation = entry.Deviation,
					Skewness = entry.Skewness
				});
				changed = true;
				continue;
			}

			if (existing.Chance == entry.Chance && existing.Mean == entry.Mean &&
				existing.StandardDeviation == entry.Deviation && existing.Skewness == entry.Skewness)
			{
				continue;
			}

			existing.Chance = entry.Chance;
			existing.Mean = entry.Mean;
			existing.StandardDeviation = entry.Deviation;
			existing.Skewness = entry.Skewness;
			changed = true;
		}

		context.SaveChanges();
		return changed;
	}

	private static bool PackageMatches(
		FuturemudDatabaseContext context,
		string name,
		IReadOnlyCollection<TraitDefinition> skills,
		double chance,
		double mean,
		double standardDeviation,
		double skewness)
	{
		var package = context.NpcSkillPackages
			.Include(x => x.Skills)
			.FirstOrDefault(x => x.Name == name);
		return package is not null &&
		       package.Skills.Count == skills.Count &&
		       skills.All(skill => package.Skills.Any(entry =>
			       entry.TraitDefinitionId == skill.Id &&
			       entry.Chance == chance &&
			       entry.Mean == mean &&
			       entry.StandardDeviation == standardDeviation &&
			       entry.Skewness == skewness));
	}
}
