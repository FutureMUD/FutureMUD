using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        IReadOnlyList<CombatAttackSeed> attacks = ParseCombatSeederAttacks();
        Dictionary<string, string[]> requiredMoves = new(StringComparer.OrdinalIgnoreCase)
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

        List<string> failures = new();
        foreach ((string weaponVariable, string[] required) in requiredMoves)
        {
            HashSet<string> actualMoves = attacks
                .Where(x => x.WeaponVariable.Equals(weaponVariable, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.MoveType)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            List<string> missing = required.Where(x => !actualMoves.Contains(x)).ToList();
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
        IReadOnlyList<CombatAttackSeed> attacks = ParseCombatSeederAttacks();
        Dictionary<string, string> trainingPairs = new(StringComparer.OrdinalIgnoreCase)
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

        List<string> failures = new();
        foreach ((string liveWeaponVariable, string trainingWeaponVariable) in trainingPairs)
        {
            List<CombatAttackSeed> liveAttacks = attacks
                .Where(x => x.WeaponVariable.Equals(liveWeaponVariable, StringComparison.OrdinalIgnoreCase))
                .ToList();
            List<CombatAttackSeed> trainingAttacks = attacks
                .Where(x => x.WeaponVariable.Equals(trainingWeaponVariable, StringComparison.OrdinalIgnoreCase))
                .ToList();

            HashSet<string> liveMoves = liveAttacks
                .Select(x => x.MoveType)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            List<string> extraTrainingMoves = trainingAttacks
                .Select(x => x.MoveType)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Where(x => !liveMoves.Contains(x))
                .ToList();
            if (extraTrainingMoves.Count > 0)
            {
                failures.Add($"{trainingWeaponVariable} introduces extra move types: {string.Join(", ", extraTrainingMoves)}.");
            }

            foreach (CombatAttackSeed trainingAttack in trainingAttacks)
            {
                if (!trainingAttack.DamageVariable.Equals("trainingDamage", StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add($"{trainingAttack.Name} should use trainingDamage but uses {trainingAttack.DamageVariable}.");
                }

                string liveName = trainingAttack.Name.Replace("Training ", "", StringComparison.Ordinal);
                CombatAttackSeed liveAttack = liveAttacks.FirstOrDefault(x =>
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
        IReadOnlyList<CombatAttackSeed> attacks = ParseCombatSeederAttacks();

        double rapierPrimaryStamina = AverageStamina(attacks, "rapier", ["UseWeaponAttack", "WardFreeAttack"]);
        double spearPrimaryStamina = AverageStamina(attacks, "spear", ["UseWeaponAttack", "WardFreeAttack"]);
        double warhammerPrimaryStamina = AverageStamina(attacks, "warhammer", ["StaggeringBlow"]);

        Assert.IsTrue(rapierPrimaryStamina < spearPrimaryStamina,
            $"Rapiers should be lighter on stamina than spears ({rapierPrimaryStamina} vs {spearPrimaryStamina}).");
        Assert.IsTrue(spearPrimaryStamina < warhammerPrimaryStamina,
            $"Spears should be lighter on stamina than warhammers ({spearPrimaryStamina} vs {warhammerPrimaryStamina}).");

        double improvisedPrimaryDelay = AverageDelay(attacks, "improvised", ["UseWeaponAttack"]);
        double clubPrimaryDelay = AverageDelay(attacks, "club", ["UseWeaponAttack", "StaggeringBlow"]);
        Assert.IsTrue(improvisedPrimaryDelay > clubPrimaryDelay,
            $"Improvised weapons should swing more slowly than clubs ({improvisedPrimaryDelay} vs {clubPrimaryDelay}).");

        int improvisedPrimaryPeakDamage = PeakDamageRank(attacks, "improvised", ["UseWeaponAttack"]);
        int clubPrimaryPeakDamage = PeakDamageRank(attacks, "club", ["UseWeaponAttack", "StaggeringBlow"]);
        Assert.IsTrue(improvisedPrimaryPeakDamage <= clubPrimaryPeakDamage,
            $"Improvised weapons should not out-damage clubs in their primary attacks ({improvisedPrimaryPeakDamage} vs {clubPrimaryPeakDamage}).");
    }

    [TestMethod]
    public void CombatSeederSource_ForcedPositioningMoves_HaveChecksMessagesAndStockAttacks()
    {
        string source = File.ReadAllText(GetCombatSeederSourcePath());

        StringAssert.Contains(source, "CheckType.PushbackCheck");
        StringAssert.Contains(source, "CheckType.OpposePushbackCheck");
        StringAssert.Contains(source, "CheckType.ForcedMovementCheck");
        StringAssert.Contains(source, "CheckType.OpposeForcedMovementCheck");
        StringAssert.Contains(source, "BuiltInCombatMoveType.Pushback");
        StringAssert.Contains(source, "BuiltInCombatMoveType.ForcedMovement");
        StringAssert.Contains(source, "\"Shield Drive\"");
        StringAssert.Contains(source, "\"Shield Shove\"");
        StringAssert.Contains(source, "ForcedMovementAttackData(Difficulty.Normal, ForcedMovementTypes.All");
    }

	[TestMethod]
	public void CombatAuxiliarySeederSource_HumanCatalogue_HasTwentyNamedMovesAndNewEffectTypes()
	{
		string source = File.ReadAllText(GetSeederSourcePath("CombatAuxiliarySeederHelper.cs"));
		string[] expectedNames =
		[
			"Circle to Flank",
			"Sidestep and Press",
			"Bind and Step",
			"Low Line Feint",
			"High Line Feint",
			"Distracting Flourish",
			"False Opening",
			"Retreating Guard",
			"Guarded Shuffle",
			"Shield Jostle",
			"Shield Glint",
			"Shoulder Check",
			"Foot Sweep",
			"Shove Off Balance",
			"Pommel Beat",
			"Wrist Check",
			"Beat the Weapon",
			"Hook and Pull",
			"Sand in the Eyes",
			"Dirt Kick"
		];

		Match catalogue = Regex.Match(source,
			@"private static readonly string\[\] HumanAuxiliaryMoveNames\s*=\s*\[(?<body>.*?)\];",
			RegexOptions.Singleline | RegexOptions.CultureInvariant);
		Assert.IsTrue(catalogue.Success, "Could not find the human auxiliary catalogue.");
		List<string> names = Regex.Matches(catalogue.Groups["body"].Value, @"""(?<name>[^""]+)""")
		                          .Select(x => x.Groups["name"].Value)
		                          .ToList();

		Assert.IsTrue(names.Count >= 20, $"Expected at least 20 human auxiliary moves, found {names.Count}.");
		CollectionAssert.IsSubsetOf(expectedNames, names);

		foreach (string effectType in new[] { "targetdelay", "facing", "targetstamina", "positionchange", "disarm" })
		{
			StringAssert.Contains(source, $"Common(\"{effectType}\"");
		}

		StringAssert.Contains(source, "new XAttribute(\"position\", PositionSprawledId)");
		StringAssert.Contains(source, "new XAttribute(\"selection\", \"Best\")");
		StringAssert.Contains(source, "new XAttribute(\"subject\", \"Attacker\")");
		StringAssert.Contains(source, "new XAttribute(\"direction\", \"Improve\")");
	}

	[TestMethod]
	public void CombatAuxiliarySeederSource_GatedProgsTagsMessagesAndRerunHooks_ArePresent()
	{
		string helper = File.ReadAllText(GetSeederSourcePath("CombatAuxiliarySeederHelper.cs"));
		string combatSeeder = File.ReadAllText(GetCombatSeederSourcePath());

		StringAssert.Contains(combatSeeder, "EnsureStockAuxiliaryContent(context)");
		StringAssert.Contains(combatSeeder, "auxiliaryResult");
		StringAssert.Contains(helper, "EnsureTag(context, \"Shiny\", functions)");
		StringAssert.Contains(helper, "EnsureTag(context, \"Reflective\", functions)");
		StringAssert.Contains(helper, "\"Auxiliary_CanThrowSandOrDirt\"");
		StringAssert.Contains(helper, "\"Auxiliary_CanShieldGlint\"");
		StringAssert.Contains(helper, "\"Diggable Soil\"");
		StringAssert.Contains(helper, "\"Foragable Sand\"");
		StringAssert.Contains(helper, "\"Vacuum\"");
		StringAssert.Contains(helper, "\"Space\"");
		StringAssert.Contains(helper, "isunderwater(@ch.Location, @ch.Layer)");
		StringAssert.Contains(helper, "istagged(@item, \"\"Shiny\"\") or istagged(@item, \"\"Reflective\"\")");
		StringAssert.Contains(helper, "celestialelevation(@ch.Location");
		StringAssert.Contains(helper, "> 0.26");
		StringAssert.Contains(helper, "CombatMessagesCombatActions");
		StringAssert.Contains(helper, "RacesCombatActions");
		StringAssert.Contains(helper, "RequiredPositionStateIds");
		StringAssert.Contains(helper, "progs.TryGetValue(definition.UsabilityProgName");
		StringAssert.Contains(helper, "ApplyStockAuxiliaryPercentages(context)");
	}

	[TestMethod]
	public void CombatAuxiliarySeederSource_NonHumanSeeders_LinkRaceAppropriateAuxiliaryMoves()
	{
		string helper = File.ReadAllText(GetSeederSourcePath("CombatAuxiliarySeederHelper.cs"));
		Dictionary<string, string> expectedActions = new(StringComparer.OrdinalIgnoreCase)
		{
			["Canid Harry"] = "Dog",
			["Feline Pounce Feint"] = "Cat",
			["Equine Shoulder Barge"] = "Horse",
			["Avian Wing Buffet"] = "Eagle",
			["Serpent Coil Feint"] = "Snake",
			["Ursine Maul-Feint"] = "Bear",
			["Dragon Wing Shadow"] = "Dragon",
			["Gryphon Buffet"] = "Gryphon",
			["Unicorn Dazzling Feint"] = "Unicorn",
			["Hydra Many-Head Feint"] = "Hydra",
			["Basilisk Glare Feint"] = "Basilisk",
			["Myconid Spore Cloud"] = "Myconid",
			["Servo Jostle"] = "Robot",
			["Hydraulic Shove"] = "Robot",
			["Sensor Flash"] = "Robot",
			["Magnetic Wrench"] = "Robot",
			["Ghostly Misdirection"] = "Ghost",
			["Infernal Glare"] = "Demon",
			["Angelic Dazzle"] = "Angel",
			["Werewolf Lunge Feint"] = "Werewolf",
			["Undead Bone-Rattle"] = "Skeleton",
			["Fiend Tail Hook"] = "Fiend"
		};

		foreach ((string action, string raceHint) in expectedActions)
		{
			StringAssert.Contains(helper, $"[\"{action}\"]");
			StringAssert.Contains(helper, $"\"{raceHint}\"");
			StringAssert.Contains(helper, $"Def(\"{action}\"");
		}

		Dictionary<string, string> seederHooks = new(StringComparer.OrdinalIgnoreCase)
		{
			["AnimalSeeder.cs"] = "EnsureAnimalAuxiliaryLinks",
			["MythicalAnimalSeeder.cs"] = "EnsureMythicalAuxiliaryLinks",
			["RobotSeeder.cs"] = "EnsureRobotAuxiliaryLinks",
			["SupernaturalSeeder.cs"] = "EnsureSupernaturalAuxiliaryLinks"
		};

		foreach ((string fileName, string hook) in seederHooks)
		{
			string source = File.ReadAllText(GetSeederSourcePath(fileName));
			Assert.IsTrue(source.Contains(hook, StringComparison.Ordinal),
				$"{fileName} should call {hook}.");
		}
	}

	[TestMethod]
	public void CombatAuxiliarySeederSource_StockStrategies_ReceiveAuxiliaryPercentagesWithoutOverfilling()
	{
		string helper = File.ReadAllText(GetSeederSourcePath("CombatAuxiliarySeederHelper.cs"));
		string strategyHelper = File.ReadAllText(GetSeederSourcePath("CombatStrategySeederHelper.cs"));

		foreach (string strategyName in new[]
		         {
			         "Melee", "Shielder", "Skirmisher", "Brawler", "Pitfighter", "Beast Brawler",
			         "Beast Swooper", "Construct Brawler", "Construct Artillery"
		         })
		{
			StringAssert.Contains(helper, $"[\"{strategyName}\"]");
		}

		StringAssert.Contains(helper, "setting.AuxiliaryPercentage = desired");
		StringAssert.Contains(helper, "if (total > 1.0)");
		StringAssert.Contains(helper, "setting.WeaponUsePercentage -= reduction");
		StringAssert.Contains(helper, "setting.NaturalWeaponPercentage -= reduction");
		StringAssert.Contains(strategyHelper, "CombatAuxiliarySeederHelper.ApplyStockAuxiliaryPercentage");
	}

	[TestMethod]
	public void CombatSeederSource_PrimitiveRangedWeapons_SeedAndRepairSlingAndBlowgunStock()
	{
		string source = File.ReadAllText(GetCombatSeederSourcePath());

		StringAssert.Contains(source, "EnsurePrimitiveRangedContent(context, skills);");
		StringAssert.Contains(source, "primitiveRangedCount = EnsurePrimitiveRangedContent(context);");
		StringAssert.Contains(source, "EnsureVariableCheck(CheckType.FireBlowgun);");
		StringAssert.Contains(source, "EnsureRangedType(");
		StringAssert.Contains(source, "\"Sling\"");
		StringAssert.Contains(source, "\"Staff Sling\"");
		StringAssert.Contains(source, "\"Blowgun\"");
		StringAssert.Contains(source, "RangedWeaponType.Blowgun");
		StringAssert.Contains(source, "EnsureComponent(\"Sling\"");
		StringAssert.Contains(source, "EnsureComponent(\"Blowgun\"");
		StringAssert.Contains(source, "EnsureAmmoType(\"Sling Bullet\"");
		StringAssert.Contains(source, "EnsureAmmoType(\"Lead Sling Bullet\"");
		StringAssert.Contains(source, "EnsureAmmoType(\"Blowgun Dart\"");
		StringAssert.Contains(source, "EnsureAmmoType(\"Barbed Blowgun Dart\"");
		StringAssert.Contains(source, "EnsureComponent(\"Ammunition\", $\"Ammo_{name.CollapseString()}\"");
	}

    [TestMethod]
    public void WeaponDamageBands_StandardWeapons_MapToExpectedSeverityBands()
    {
        const double nominalStrength = 10.0;
        const double degree = 0.0;
        const double staticModeStartingMultiplier = 0.6;
        int standardQuality = (int)ItemQuality.Standard;

        double terribleDamage = RepresentativeStaticDamage(0.1, nominalStrength, standardQuality, degree,
            staticModeStartingMultiplier);
        double badDamage = RepresentativeStaticDamage(0.2, nominalStrength, standardQuality, degree,
            staticModeStartingMultiplier);
        double poorDamage = RepresentativeStaticDamage(0.25, nominalStrength, standardQuality, degree,
            staticModeStartingMultiplier);
        double normalDamage = RepresentativeStaticDamage(0.3, nominalStrength, standardQuality, degree,
            staticModeStartingMultiplier);
        double goodDamage = RepresentativeStaticDamage(0.4, nominalStrength, standardQuality, degree,
            staticModeStartingMultiplier);
        double greatDamage = RepresentativeStaticDamage(0.5, nominalStrength, standardQuality, degree,
            staticModeStartingMultiplier);
        double coupDeGraceDamage = RepresentativeStaticDamage(1.0, nominalStrength, standardQuality, degree,
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
        string source = File.ReadAllText(GetCombatSeederSourcePath());
        Regex regex = new(
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
		return GetSeederSourcePath("CombatSeeder.cs");
    }

	private static string GetSeederSourcePath(string fileName)
	{
        return Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "DatabaseSeeder",
            "Seeders",
            fileName));
    }
}
