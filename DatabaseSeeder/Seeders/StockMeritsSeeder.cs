#nullable enable

using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.CharacterCreation;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public class StockMeritsSeeder : IDatabaseSeeder
{
    private const string IsDarkProgName = "StockMeritsIsDark";
    private const string IsNaturalTerrainProgName = "StockMeritsIsNaturalTerrain";
    private const string IsHumanInfluencedTerrainProgName = "StockMeritsIsHumanInfluencedTerrain";
    private const string IsUrbanTerrainProgName = "StockMeritsIsUrbanTerrain";
    private const string IsRuralTerrainProgName = "StockMeritsIsRuralTerrain";
    private const string IsTerrestrialTerrainProgName = "StockMeritsIsTerrestrialTerrain";
    private const string IsRiparianTerrainProgName = "StockMeritsIsRiparianTerrain";
    private const string IsLittoralTerrainProgName = "StockMeritsIsLittoralTerrain";
    private const string IsAquaticTerrainProgName = "StockMeritsIsAquaticTerrain";
    private const string DarkFearMinorProgName = "StockMeritsDarkFearMinor";
    private const string DarkFearModerateProgName = "StockMeritsDarkFearModerate";
    private const string DarkFearMajorProgName = "StockMeritsDarkFearMajor";
    private const string WildAffinityMinorProgName = "StockMeritsWildAffinityMinor";
    private const string WildAffinityModerateProgName = "StockMeritsWildAffinityModerate";
    private const string WildAffinityMajorProgName = "StockMeritsWildAffinityMajor";
    private const string UrbanAffinityMinorProgName = "StockMeritsUrbanAffinityMinor";
    private const string UrbanAffinityModerateProgName = "StockMeritsUrbanAffinityModerate";
    private const string UrbanAffinityMajorProgName = "StockMeritsUrbanAffinityMajor";

    private static readonly string[] HelperProgNames =
    [
        IsDarkProgName,
        IsNaturalTerrainProgName,
        IsHumanInfluencedTerrainProgName,
        IsUrbanTerrainProgName,
        IsRuralTerrainProgName,
        IsTerrestrialTerrainProgName,
        IsRiparianTerrainProgName,
        IsLittoralTerrainProgName,
        IsAquaticTerrainProgName,
        DarkFearMinorProgName,
        DarkFearModerateProgName,
        DarkFearMajorProgName,
        WildAffinityMinorProgName,
        WildAffinityModerateProgName,
        WildAffinityMajorProgName,
        UrbanAffinityMinorProgName,
        UrbanAffinityModerateProgName,
        UrbanAffinityMajorProgName
    ];

    private static readonly StockMeritBlueprint[] StockMerits =
    [
        new("Steady Presence", "All Check Bonus", MeritType.Merit,
            context => AllCheckMerit(context, 1.0,
                "You are steady, reliable, and hard to rattle.",
                "$0 have|has a steady presence.")),
        new("Chronic Distractibility", "All Check Bonus", MeritType.Flaw,
            context => AllCheckMerit(context, -1.0,
                "Your attention is always slipping away from the task in front of you.",
                "$0 are|is chronically distractible.")),

        new("Robust Immune System", "All Infection Bonus", MeritType.Merit,
            context => AllInfectionMerit(context, 2,
                "Your body shrugs off infections better than most.",
                "$0 have|has a robust immune system.")),
        new("Infection Prone", "All Infection Bonus", MeritType.Flaw,
            context => AllInfectionMerit(context, -2,
                "Small infections have an unfortunate habit of becoming serious.",
                "$0 are|is infection prone.")),

        new("Wilderness Instincts", "All Skill Bonus", MeritType.Merit,
            context => AllSkillMerit(context, 1.0, IsNaturalTerrainProgName,
                "You are at your sharpest when the walls fall away and the world turns wild.",
                "$0 have|has strong wilderness instincts.")),
        new("Street-Schooled", "All Skill Bonus", MeritType.Merit,
            context => AllSkillMerit(context, 1.0, IsUrbanTerrainProgName,
                "You understand civilisation, crowds, and enclosed spaces intuitively.",
                "$0 are|is street-schooled.")),
        new("Out of Your Element", "All Skill Bonus", MeritType.Flaw,
            context => AllSkillMerit(context, -1.0, IsNaturalTerrainProgName,
                "The wilderness leaves you uneasy and less capable than usual.",
                "$0 are|is out of #0 element.")),

        new("Quick Stride", "All Speed Multiplier", MeritType.Merit,
            context => AllSpeedMerit(context, 1.10,
                "You move with a quick, efficient stride.",
                "$0 move|moves with a quick stride.")),
        new("Slowpoke", "All Speed Multiplier", MeritType.Flaw,
            context => AllSpeedMerit(context, 0.90,
                "You simply never seem to get anywhere in a hurry.",
                "$0 are|is a slowpoke.")),

        new("Ambidextrous", "Ambidextrous", MeritType.Merit,
            context => SimpleMerit(context,
                "You can use either hand without the usual penalties.",
                "$0 are|is ambidextrous.")),

        new("Dense Bones", "BoneHealth", MeritType.Merit,
            context => BoneHealthMerit(context, 1.15,
                "Your bones are unusually sturdy.",
                "$0 have|has dense bones.")),
        new("Brittle Bones", "BoneHealth", MeritType.Flaw,
            context => BoneHealthMerit(context, 0.85,
                "Your bones are more fragile than they ought to be.",
                "$0 have|has brittle bones.")),

        new("Battlefield Calm", "Check Category Bonus", MeritType.Merit,
            context => CheckCategoryMerit(context, 2.0, hostile: true,
                blurb: "You remain focused when danger is close and violence is imminent.",
                description: "$0 remain|remains calm in battle.")),
        new("Combat Paralysis", "Check Category Bonus", MeritType.Flaw,
            context => CheckCategoryMerit(context, -2.0, hostile: true,
                blurb: "When combat erupts, your body wants to freeze instead of act.",
                description: "$0 freeze|freezes in combat.")),

        new("Reckless Fighter", "Combat Recklessness", MeritType.Flaw,
            context => SimpleMerit(context,
                "You disregard your own safety once the fighting starts.",
                "$0 fight|fights with reckless disregard for &0 safety.")),

        new("Low-Light Vision", "Darksight", MeritType.Merit,
            context => DarksightMerit(context, Difficulty.Easy,
                "Your eyes perform unusually well in the dark.",
                "$0 have|has low-light vision.")),
        new("Moonlit Eyes", "Darksight", MeritType.Merit,
            context => DarksightMerit(context, Difficulty.Automatic,
                "You can function in darkness that would hinder most people.",
                "$0 have|has moonlit eyes.")),

        new("Strong Pain Tolerance", "Drug Effect Resistance", MeritType.Merit,
            context => DrugEffectResistanceMerit(context,
                new Dictionary<DrugType, double>
                {
                    [DrugType.Analgesic] = 0.25,
                    [DrugType.Nausea] = 0.15
                },
                "Drugs that would floor other people often hit you less strongly.",
                "$0 are|is resistant to some drug effects.")),
        new("Medication Sensitive", "Drug Effect Resistance", MeritType.Flaw,
            context => DrugEffectResistanceMerit(context,
                new Dictionary<DrugType, double>
                {
                    [DrugType.Analgesic] = -0.20,
                    [DrugType.VisionImpairment] = -0.20
                },
                "Even mild medication tends to hit you harder than it should.",
                "$0 are|is unusually sensitive to drugs.")),

        new("Tunnel Rat", "ExitSize", MeritType.Merit,
            context => ExitSizeMerit(context, -1,
                "You can squeeze through tighter openings than your size would suggest.",
                "$0 are|is a tunnel rat.")),
        new("Broad Shoulders", "ExitSize", MeritType.Flaw,
            context => ExitSizeMerit(context, 1,
                "You need a little more room than most people to get through confined exits.",
                "$0 have|has broad shoulders.")),

        new("Trail-Hardened", "Movement Stamina Multiplier", MeritType.Merit,
            context => MovementStaminaMerit(context, 0.85, MovementType.Upright,
                "Long journeys on foot tire you less than they tire others.",
                "$0 are|is trail-hardened.")),
        new("Easily Winded", "Movement Stamina Multiplier", MeritType.Flaw,
            context => MovementStaminaMerit(context, 1.15, MovementType.Upright,
                "You tire more quickly than most people when travelling on foot.",
                "$0 are|is easily winded.")),

        new("Shadow Scout", "Multi Check Bonus", MeritType.Merit,
            context => MultiCheckMerit(context, 1.5, [CheckType.HideCheck, CheckType.SneakCheck],
                "You are good at remaining unseen and unheard.",
                "$0 are|is adept at moving unseen.")),
        new("Heavy-Footed", "Multi Check Bonus", MeritType.Flaw,
            context => MultiCheckMerit(context, -1.5, [CheckType.HideCheck, CheckType.SneakCheck],
                "You make too much noise and leave too much sign when trying to stay hidden.",
                "$0 are|is heavy-footed.")),

        new("Sturdy Frame", "Multi Trait Bonus", MeritType.Merit,
            context => MultiTraitMerit(context, 1.0, ["Strength", "Constitution"],
                "You are naturally stronger and hardier than average.",
                "$0 have|has a sturdy frame.")),
        new("Frail Frame", "Multi Trait Bonus", MeritType.Flaw,
            context => MultiTraitMerit(context, -1.0, ["Strength", "Constitution"],
                "You are lighter-framed and less robust than average.",
                "$0 have|has a frail frame.")),

        new("Mute", "Mute", MeritType.Flaw,
            context => MuteMerit(context, PermitLanguageOptions.LanguageIsMuffling,
                "You cannot speak normally.",
                "$0 are|is mute.")),

        new("Near-Sighted", "Myopia", MeritType.Flaw,
            context => MyopiaMerit(context, true, context.AlwaysTrueProgId,
                "Distance vision is difficult for you without proper correction.",
                "$0 have|has near-sightedness.")),
        new("Night Blindness", "Myopia", MeritType.Flaw,
            context => MyopiaMerit(context, false, context.Prog(IsDarkProgName),
                "Low light causes major trouble for your vision.",
                "$0 have|has night blindness.")),

        new("Hard Punches", "Natural Attack Quality", MeritType.Merit,
            context => NaturalAttackMerit(context, MeleeWeaponVerb.Punch, 1,
                "Your punches land with unusual force.",
                "$0 have|has hard punches.")),
        new("Pillow Punches", "Natural Attack Quality", MeritType.Flaw,
            context => NaturalAttackMerit(context, MeleeWeaponVerb.Punch, -1,
                "Your punches never seem to land as hard as they should.",
                "$0 have|has pillow punches.")),
        new("Iron Kicks", "Natural Attack Quality", MeritType.Merit,
            context => NaturalAttackMerit(context, MeleeWeaponVerb.Kick, 1,
                "Your kicks hit with punishing force.",
                "$0 have|has iron kicks.")),
        new("Soft Kicks", "Natural Attack Quality", MeritType.Flaw,
            context => NaturalAttackMerit(context, MeleeWeaponVerb.Kick, -1,
                "Your kicks lack weight and stopping power.",
                "$0 have|has soft kicks.")),
        new("Predator's Bite", "Natural Attack Quality", MeritType.Merit,
            context => NaturalAttackMerit(context, MeleeWeaponVerb.Bite, 1,
                "You have an unusually dangerous bite.",
                "$0 have|has a predator's bite.")),
        new("Weak Bite", "Natural Attack Quality", MeritType.Flaw,
            context => NaturalAttackMerit(context, MeleeWeaponVerb.Bite, -1,
                "Your bite lacks force and commitment.",
                "$0 have|has a weak bite.")),

        new("Efficient Metabolism", "Needs Rate Change", MeritType.Merit,
            context => NeedRateMerit(context, 0.85, 0.90, 0.90,
                "You burn through food, water, and alcohol more slowly than most people.",
                "$0 have|has an efficient metabolism.")),
        new("Ravenous", "Needs Rate Change", MeritType.Flaw,
            context => NeedRateMerit(context, 1.25, 1.10, 1.15,
                "You get hungry, thirsty, and intoxicated faster than most people.",
                "$0 are|is ravenous.")),

        new("Survivor", "Organ Hit Reduction", MeritType.Merit,
            context => OrganHitMerit(context, WoundSeverity.Severe, WoundSeverity.Horrifying, "chance*0.75",
                [BodypartTypeEnum.Brain, BodypartTypeEnum.Heart, BodypartTypeEnum.Lung],
                "You have an uncanny knack for surviving wounds that should have finished you.",
                "$0 are|is a survivor.")),
        new("Tender Core", "Organ Hit Reduction", MeritType.Flaw,
            context => OrganHitMerit(context, WoundSeverity.Severe, WoundSeverity.Horrifying, "chance*1.25",
                [BodypartTypeEnum.Brain, BodypartTypeEnum.Heart, BodypartTypeEnum.Lung],
                "Critical hits to your vital organs are more likely than they should be.",
                "$0 have|has a tender core.")),

        new("Morning Person (Minor)", "RestedBonus", MeritType.Merit,
            context => RestedBonusMerit(context, 1.25,
                "You get a small extra lift from a good sleep.",
                "$0 are|is a morning person.")),
        new("Morning Person (Moderate)", "RestedBonus", MeritType.Merit,
            context => RestedBonusMerit(context, 1.50,
                "You get a solid extra lift from a good sleep.",
                "$0 are|is very much a morning person.")),
        new("Morning Person (Major)", "RestedBonus", MeritType.Merit,
            context => RestedBonusMerit(context, 2.00,
                "A proper night's sleep leaves you noticeably sharper than most people.",
                "$0 thrive|thrives after a good night's sleep.")),
        new("Fitful Sleeper (Minor)", "RestedBonus", MeritType.Flaw,
            context => RestedBonusMerit(context, 0.85,
                "Even decent sleep does not restore you as effectively as it should.",
                "$0 are|is a fitful sleeper.")),
        new("Fitful Sleeper (Moderate)", "RestedBonus", MeritType.Flaw,
            context => RestedBonusMerit(context, 0.70,
                "Sleep helps you, but never as much as it helps other people.",
                "$0 rarely feel|feels fully rested.")),
        new("Fitful Sleeper (Major)", "RestedBonus", MeritType.Flaw,
            context => RestedBonusMerit(context, 0.50,
                "Even when you sleep well, you wake feeling only half-recovered.",
                "$0 almost never wake|wakes fully refreshed.")),

        new("Afraid of the Dark (Minor)", "Scaled Check Category Bonus", MeritType.Flaw,
            context => ScaledCheckCategoryMerit(context, DarkFearMinorProgName, IsDarkProgName, hostile: true, active: true, perception: true,
                blurb: "Darkness puts you badly off-balance.",
                description: "$0 are|is afraid of the dark.")),
        new("Afraid of the Dark (Moderate)", "Scaled Check Category Bonus", MeritType.Flaw,
            context => ScaledCheckCategoryMerit(context, DarkFearModerateProgName, IsDarkProgName, hostile: true, active: true, perception: true,
                blurb: "Darkness is deeply unsettling for you.",
                description: "$0 are|is very afraid of the dark.")),
        new("Afraid of the Dark (Major)", "Scaled Check Category Bonus", MeritType.Flaw,
            context => ScaledCheckCategoryMerit(context, DarkFearMajorProgName, IsDarkProgName, hostile: true, active: true, perception: true,
                blurb: "You never feel safe or capable when the lights go out.",
                description: "$0 are|is terrified of the dark.")),

        new("Child of the Wild (Minor)", "Scaled Check Category Bonus", MeritType.Merit,
            context => ScaledCheckCategoryMerit(context, WildAffinityMinorProgName, IsNaturalTerrainProgName,
                healing: true, friendly: true, hostile: true, active: true, perception: true,
                blurb: "The natural world steadies and strengthens you.",
                description: "$0 are|is a child of the wild.")),
        new("Child of the Wild (Moderate)", "Scaled Check Category Bonus", MeritType.Merit,
            context => ScaledCheckCategoryMerit(context, WildAffinityModerateProgName, IsNaturalTerrainProgName,
                healing: true, friendly: true, hostile: true, active: true, perception: true,
                blurb: "You draw substantial confidence from untamed places.",
                description: "$0 draw|draws strength from the wild.")),
        new("Child of the Wild (Major)", "Scaled Check Category Bonus", MeritType.Merit,
            context => ScaledCheckCategoryMerit(context, WildAffinityMajorProgName, IsNaturalTerrainProgName,
                healing: true, friendly: true, hostile: true, active: true, perception: true,
                blurb: "In wilderness terrain you are clearly operating at your best.",
                description: "$0 thrive|thrives in the wild.")),

        new("At Home in the City (Minor)", "Scaled Check Category Bonus", MeritType.Merit,
            context => ScaledCheckCategoryMerit(context, UrbanAffinityMinorProgName, IsUrbanTerrainProgName,
                friendly: true, active: true, perception: true, language: true,
                blurb: "Civilisation is where your instincts feel sharpest.",
                description: "$0 are|is at home in the city.")),
        new("At Home in the City (Moderate)", "Scaled Check Category Bonus", MeritType.Merit,
            context => ScaledCheckCategoryMerit(context, UrbanAffinityModerateProgName, IsUrbanTerrainProgName,
                friendly: true, active: true, perception: true, language: true,
                blurb: "Urban life puts you in your element.",
                description: "$0 flourish|flourishes in the city.")),
        new("At Home in the City (Major)", "Scaled Check Category Bonus", MeritType.Merit,
            context => ScaledCheckCategoryMerit(context, UrbanAffinityMajorProgName, IsUrbanTerrainProgName,
                friendly: true, active: true, perception: true, language: true,
                blurb: "You are most capable when surrounded by civilisation and structure.",
                description: "$0 are|is deeply at home in the city.")),

        new("Steady Shot", "Scatter Chance", MeritType.Merit,
            context => ScatterChanceMerit(context, 0.85,
                "Your ranged attacks are less likely to go badly astray.",
                "$0 have|has a steady shot.")),
        new("Wild Shooter", "Scatter Chance", MeritType.Flaw,
            context => ScatterChanceMerit(context, 1.15,
                "Your ranged attacks are more likely than usual to scatter.",
                "$0 are|is a wild shooter.")),

        new("Second Wind", "SecondWind", MeritType.Merit,
            context => SecondWindMerit(context,
                "$0 draw|draws a deep breath and seem|seems to find a sudden reserve of strength.",
                "You feel your second wind fade away.",
                TimeSpan.FromMinutes(20),
                "In desperate moments, you can find an extra burst of endurance.",
                "$0 have|has a second wind.")),

        new("Woodswise Learner (Minor)", "Skill Learning", MeritType.Merit,
            context => SkillLearningMerit(context, ["survival", "perception"], 1.05, 1.15, Difficulty.Automatic, Difficulty.Hard, IsNaturalTerrainProgName,
                "The wild helps you make sense of practical lessons quickly.",
                "$0 learn|learns wilderness skills quickly.")),
        new("Woodswise Learner (Moderate)", "Skill Learning", MeritType.Merit,
            context => SkillLearningMerit(context, ["survival", "perception"], 1.10, 1.30, Difficulty.Automatic, Difficulty.VeryHard, IsNaturalTerrainProgName,
                "In natural terrain, you improve your practical wilderness skills quickly.",
                "$0 are|is a strong wilderness learner.")),
        new("Woodswise Learner (Major)", "Skill Learning", MeritType.Merit,
            context => SkillLearningMerit(context, ["survival", "perception"], 1.20, 1.50, Difficulty.Automatic, Difficulty.ExtremelyHard, IsNaturalTerrainProgName,
                "The wilderness seems to teach you almost effortlessly.",
                "$0 learn|learns exceptionally well in the wild.")),
        new("Sheltered Learner (Minor)", "Skill Learning", MeritType.Flaw,
            context => SkillLearningMerit(context, ["survival", "perception"], 0.95, 0.90, Difficulty.Automatic, Difficulty.Hard, IsNaturalTerrainProgName,
                "Natural terrain leaves you struggling to absorb practical lessons.",
                "$0 are|is a sheltered learner in the wild.")),
        new("Sheltered Learner (Moderate)", "Skill Learning", MeritType.Flaw,
            context => SkillLearningMerit(context, ["survival", "perception"], 0.90, 0.80, Difficulty.Automatic, Difficulty.VeryHard, IsNaturalTerrainProgName,
                "The wilderness is a poor classroom for you.",
                "$0 learn|learns poorly in the wild.")),
        new("Sheltered Learner (Major)", "Skill Learning", MeritType.Flaw,
            context => SkillLearningMerit(context, ["survival", "perception"], 0.85, 0.70, Difficulty.Automatic, Difficulty.ExtremelyHard, IsNaturalTerrainProgName,
                "Natural terrain leaves you badly out of your depth when trying to learn.",
                "$0 are|is profoundly sheltered in the wild.")),

        new("Eye for Tracks", "Specific Check Bonus", MeritType.Merit,
            context => SpecificCheckMerit(context, CheckType.SearchForTracksCheck, 2.0,
                "You are particularly sharp at reading sign and spoor.",
                "$0 have|has an eye for tracks.")),
        new("Vertigo", "Specific Check Bonus", MeritType.Flaw,
            context => SpecificCheckMerit(context, CheckType.ClimbCheck, -2.0,
                "Heights make it hard for you to keep your nerve and judgment.",
                "$0 suffer|suffers from vertigo.")),

        new("Sprinter", "Specific Speed Multiplier", MeritType.Merit,
            context => SpecificSpeedMerit(context, 1.15, ["run", "sprint"],
                "You are unusually quick at the faster end of human movement.",
                "$0 are|is a sprinter.")),
        new("Slow Starter", "Specific Speed Multiplier", MeritType.Flaw,
            context => SpecificSpeedMerit(context, 0.85, ["run", "sprint"],
                "You struggle to build speed when pushing into a run.",
                "$0 are|is a slow starter.")),

        new("Keen-Eyed", "Specific Trait Bonus", MeritType.Merit,
            context => SpecificTraitMerit(context, "Perception", 1.0,
                "You pick up details other people often miss.",
                "$0 are|is keen-eyed.")),
        new("Weak-Willed", "Specific Trait Bonus", MeritType.Flaw,
            context => SpecificTraitMerit(context, "Willpower", -1.0,
                "You find it harder than most people to hold firm under pressure.",
                "$0 are|is weak-willed.")),

        new("Tidy Surgeon", "Surgery Finalisation", MeritType.Merit,
            context => SurgeryMerit(context, 2,
                "Patients recover more cleanly from surgeries you perform.",
                "$0 are|is a tidy surgeon.")),
        new("Sloppy Surgeon", "Surgery Finalisation", MeritType.Flaw,
            context => SurgeryMerit(context, -2,
                "Your surgeries leave a more difficult recovery behind them.",
                "$0 are|is a sloppy surgeon.")),

        new("Strong Swimmer", "Swimming Stamina Multiplier", MeritType.Merit,
            context => SwimmingStaminaMerit(context, 0.80,
                "You are efficient and enduring in the water.",
                "$0 are|is a strong swimmer.")),
        new("Lead-Limbed", "Swimming Stamina Multiplier", MeritType.Flaw,
            context => SwimmingStaminaMerit(context, 1.20,
                "Swimming drains you much faster than it drains most people.",
                "$0 are|is lead-limbed in the water.")),

        new("Will to Live", "Will To Live", MeritType.Merit,
            context => WillToLiveMerit(context, 0.03, 0.03, 0.75,
                "You cling to life longer than most people when the situation turns dire.",
                "$0 have|has the will to live.")),
        new("Fading Spark", "Will To Live", MeritType.Flaw,
            context => WillToLiveMerit(context, -0.03, -0.03, 1.25,
                "When the worst happens, you have less reserve to cling on than most people do.",
                "$0 have|has a fading spark.")),
    ];

    public bool SafeToRunMoreThanOnce => true;

    public IEnumerable<(string Id, string Question,
        Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
        Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions => [];

    public int SortOrder => 102;
    public string Name => "Merits and Flaws";
    public string Tagline => "Stock merits, flaws, and helper progs for character creation";
    public string FullDescription =>
        @"This package installs a stock catalogue of merits and flaws for character creation, along with a small set of reusable helper FutureProgs.

The catalogue is designed to be mode-neutral. It does not switch chargen between MeritPicker and QuirkPicker, and it does not assign any chargen-resource costs by default. Builders can later price or rebalance these merits however they like.

The included examples emphasise conditional terrain- and darkness-based merits, wilderness and city affinities, rested and stamina ladders, and a broad spread of the practical character-merit types supported by the engine.";

    internal static IReadOnlyCollection<string> HelperProgNamesForTesting => HelperProgNames;
    internal static IReadOnlyCollection<string> StockMeritNamesForTesting => StockMerits.Select(x => x.Name).ToArray();

    public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
    {
        FutureProg alwaysTrue = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
        SeedHelperProgs(context);
        context.SaveChanges();

        StockMeritContext stockContext = new(context, alwaysTrue.Id);
        foreach (StockMeritBlueprint merit in StockMerits)
        {
            EnsureCharacterMerit(context, merit, stockContext);
        }

        context.SaveChanges();
        return "Package successfully applied.";
    }

    public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
    {
        if (!context.Accounts.Any())
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        if (!context.Races.Any(x => x.Name == "Human"))
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        if (!HasMeritSelectionStoryboard(context))
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        return SeederRepeatabilityHelper.ClassifyByPresence(
            HelperProgNames.Select(name => context.FutureProgs.Any(x => x.FunctionName == name))
                .Concat(StockMerits.Select(merit => context.Merits.Any(x => x.Name == merit.Name))));
    }

    private static bool HasMeritSelectionStoryboard(FuturemudDatabaseContext context)
    {
        return context.ChargenScreenStoryboards.Any(x =>
            x.ChargenStage == (int)ChargenStage.SelectMerits &&
            (x.ChargenType == "MeritPicker" || x.ChargenType == "QuirkPicker")
            );
    }

    private static void SeedHelperProgs(FuturemudDatabaseContext context)
    {
        EnsureBooleanHelperProg(context, IsDarkProgName,
            "Returns true when a character is in sufficiently low light for stock darkness merits and flaws.",
            """
			if (isnull(@ch.Location))
				return false
			end if
			return Getlight(@ch) <= 10
			""");
        EnsureTerrainTagHelperProg(context, IsNaturalTerrainProgName, "Wild",
            "Returns true when a character is in stock wilderness or natural terrain.");
        EnsureTerrainTagHelperProg(context, IsHumanInfluencedTerrainProgName, "Human Influenced",
            "Returns true when a character is in terrain tagged as human influenced.");
        EnsureTerrainTagHelperProg(context, IsUrbanTerrainProgName, "Urban",
            "Returns true when a character is in terrain tagged as urban.");
        EnsureTerrainTagHelperProg(context, IsRuralTerrainProgName, "Rural",
            "Returns true when a character is in terrain tagged as rural.");
        EnsureTerrainTagHelperProg(context, IsTerrestrialTerrainProgName, "Terrestrial",
            "Returns true when a character is in terrain tagged as terrestrial.");
        EnsureTerrainTagHelperProg(context, IsRiparianTerrainProgName, "Riparian",
            "Returns true when a character is in terrain tagged as riparian.");
        EnsureTerrainTagHelperProg(context, IsLittoralTerrainProgName, "Littoral",
            "Returns true when a character is in terrain tagged as littoral.");
        EnsureTerrainTagHelperProg(context, IsAquaticTerrainProgName, "Aquatic",
            "Returns true when a character is in terrain tagged as aquatic.");

        EnsureNumberHelperProg(context, DarkFearMinorProgName,
            "Returns the minor stock darkness penalty for scaled darkness flaws.",
            "return -1.0");
        EnsureNumberHelperProg(context, DarkFearModerateProgName,
            "Returns the moderate stock darkness penalty for scaled darkness flaws.",
            "return -2.0");
        EnsureNumberHelperProg(context, DarkFearMajorProgName,
            "Returns the major stock darkness penalty for scaled darkness flaws.",
            "return -3.0");
        EnsureNumberHelperProg(context, WildAffinityMinorProgName,
            "Returns the minor stock wilderness bonus for scaled wilderness merits.",
            "return 0.75");
        EnsureNumberHelperProg(context, WildAffinityModerateProgName,
            "Returns the moderate stock wilderness bonus for scaled wilderness merits.",
            "return 1.5");
        EnsureNumberHelperProg(context, WildAffinityMajorProgName,
            "Returns the major stock wilderness bonus for scaled wilderness merits.",
            "return 2.25");
        EnsureNumberHelperProg(context, UrbanAffinityMinorProgName,
            "Returns the minor stock city bonus for scaled urban merits.",
            "return 0.75");
        EnsureNumberHelperProg(context, UrbanAffinityModerateProgName,
            "Returns the moderate stock city bonus for scaled urban merits.",
            "return 1.5");
        EnsureNumberHelperProg(context, UrbanAffinityMajorProgName,
            "Returns the major stock city bonus for scaled urban merits.",
            "return 2.25");
    }

    private static void EnsureTerrainTagHelperProg(FuturemudDatabaseContext context, string functionName, string tagName,
        string comment)
    {
        EnsureBooleanHelperProg(context, functionName, comment,
            $"""
			if (isnull(@ch.Location))
				return false
			end if
			if (isnull(@ch.Location.Terrain))
				return false
			end if
			return istagged(@ch.Location.Terrain, "{tagName}")
			""");
    }

    private static void EnsureBooleanHelperProg(FuturemudDatabaseContext context, string functionName, string comment,
        string text)
    {
        SeederRepeatabilityHelper.EnsureProg(context,
            functionName,
            "Character",
            "Merits",
            ProgVariableTypes.Boolean,
            comment,
            text,
            false,
            false,
            FutureProgStaticType.NotStatic,
            (ProgVariableTypes.Character, "ch"));
    }

    private static void EnsureNumberHelperProg(FuturemudDatabaseContext context, string functionName, string comment,
        string text)
    {
        SeederRepeatabilityHelper.EnsureProg(context,
            functionName,
            "Character",
            "Merits",
            ProgVariableTypes.Number,
            comment,
            text,
            false,
            false,
            FutureProgStaticType.NotStatic,
            (ProgVariableTypes.Character, "ch"));
    }

    private static void EnsureCharacterMerit(FuturemudDatabaseContext context, StockMeritBlueprint blueprint,
        StockMeritContext stockContext)
    {
        Merit merit = SeederRepeatabilityHelper.EnsureNamedEntity(
            context.Merits,
            blueprint.Name,
            x => x.Name,
            () =>
            {
                Merit created = new();
                context.Merits.Add(created);
                return created;
            });

        merit.Name = blueprint.Name;
        merit.Type = blueprint.Type;
        merit.MeritType = (int)blueprint.MeritType;
        merit.MeritScope = (int)MeritScope.Character;
        merit.ParentId = null;
        merit.Definition = blueprint.Definition(stockContext).ToString();
    }

    private static XElement SimpleMerit(StockMeritContext context, string blurb, string description,
        long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId);
    }

    private static XElement AllCheckMerit(StockMeritContext context, double bonus, string blurb, string description,
        long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId, new XAttribute("bonus", bonus));
    }

    private static XElement AllInfectionMerit(StockMeritContext context, int bonus, string blurb, string description,
        long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId, new XAttribute("bonus", bonus));
    }

    private static XElement AllSkillMerit(StockMeritContext context, double bonus, string applicabilityProgName,
        string blurb, string description)
    {
        return MeritRoot(context, blurb, description, context.Prog(applicabilityProgName),
            new XAttribute("bonus", bonus));
    }

    private static XElement AllSpeedMerit(StockMeritContext context, double multiplier, string blurb, string description,
        long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId, new XAttribute("multiplier", multiplier));
    }

    private static XElement BoneHealthMerit(StockMeritContext context, double modifier, string blurb, string description,
        long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XElement("Modifier", modifier));
    }

    private static XElement CheckCategoryMerit(StockMeritContext context, double bonus, bool healing = false,
        bool friendly = false, bool hostile = false, bool active = false, bool perception = false,
        bool language = false, string blurb = "", string description = "", long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XAttribute("bonus", bonus),
            new XAttribute("healing", healing),
            new XAttribute("friendly", friendly),
            new XAttribute("hostile", hostile),
            new XAttribute("active", active),
            new XAttribute("perception", perception),
            new XAttribute("language", language));
    }

    private static XElement DarksightMerit(StockMeritContext context, Difficulty minimumDifficulty, string blurb,
        string description)
    {
        return MeritRoot(context, blurb, description,
            new XElement("MinimumEffectiveDifficulty", (int)minimumDifficulty));
    }

    private static XElement DrugEffectResistanceMerit(StockMeritContext context,
        IDictionary<DrugType, double> resistances, string blurb, string description, long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XElement("Resistances",
                resistances.Select(resistance => new XElement("Resistance",
                    new XAttribute("type", (int)resistance.Key),
                    new XAttribute("value", resistance.Value)))));
    }

    private static XElement ExitSizeMerit(StockMeritContext context, int sizeOffset, string blurb, string description,
        long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XElement("SizeOffset", sizeOffset));
    }

    private static XElement MovementStaminaMerit(StockMeritContext context, double multiplier, MovementType moveTypes,
        string blurb, string description, long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XAttribute("multiplier", multiplier),
            new XAttribute("movetypes", (int)moveTypes));
    }

    private static XElement MultiCheckMerit(StockMeritContext context, double bonus, IEnumerable<CheckType> checkTypes,
        string blurb, string description, long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XAttribute("bonus", bonus),
            new XElement("Checks",
                checkTypes.Select(check => new XElement("Check", new XAttribute("type", (int)check)))));
    }

    private static XElement MultiTraitMerit(StockMeritContext context, double bonus, IEnumerable<string> traitNames,
        string blurb, string description, long? applicabilityProgId = null, params TraitBonusContext[] contexts)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XAttribute("bonus", bonus),
            new XElement("Traits",
                traitNames.Select(traitName => new XElement("Trait", new XAttribute("id", context.Trait(traitName))))),
            new XElement("Contexts",
                contexts.Distinct().Select(item => new XElement("Context", (int)item))));
    }

    private static XElement MuteMerit(StockMeritContext context, PermitLanguageOptions option, string blurb,
        string description)
    {
        return MeritRoot(context, blurb, description,
            new XElement("PermitLanguageOption", (int)option));
    }

    private static XElement MyopiaMerit(StockMeritContext context, bool correctedByGlasses, long applicabilityProgId,
        string blurb, string description)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XAttribute("glasses", correctedByGlasses));
    }

    private static XElement NaturalAttackMerit(StockMeritContext context, MeleeWeaponVerb verb, int boosts, string blurb,
        string description, long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XAttribute("verb", (int)verb),
            new XAttribute("boosts", boosts));
    }

    private static XElement NeedRateMerit(StockMeritContext context, double hunger, double thirst, double alcohol,
        string blurb, string description, long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XAttribute("hunger", hunger),
            new XAttribute("thirst", thirst),
            new XAttribute("alcohol", alcohol));
    }

    private static XElement OrganHitMerit(StockMeritContext context, WoundSeverity minimumSeverity,
        WoundSeverity maximumSeverity, string chanceFormula, IEnumerable<BodypartTypeEnum> organs, string blurb,
        string description, long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XAttribute("minseverity", (int)minimumSeverity),
            new XAttribute("maxseverity", (int)maximumSeverity),
            new XElement("Chance", new XCData(chanceFormula)),
            new XElement("Organs",
                organs.Select(organ => new XElement("Organ", (int)organ))));
    }

    private static XElement RestedBonusMerit(StockMeritContext context, double multiplier, string blurb,
        string description, long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XElement("Multiplier", multiplier));
    }

    private static XElement ScaledCheckCategoryMerit(StockMeritContext context, string bonusProgName,
        string applicabilityProgName, bool healing = false, bool friendly = false, bool hostile = false,
        bool active = false, bool perception = false, bool language = false, string blurb = "", string description = "")
    {
        return MeritRoot(context, blurb, description, context.Prog(applicabilityProgName),
            new XAttribute("bonusprog", context.Prog(bonusProgName)),
            new XAttribute("healing", healing),
            new XAttribute("friendly", friendly),
            new XAttribute("hostile", hostile),
            new XAttribute("active", active),
            new XAttribute("perception", perception),
            new XAttribute("language", language));
    }

    private static XElement ScatterChanceMerit(StockMeritContext context, double multiplier, string blurb,
        string description, long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XElement("Multiplier", multiplier));
    }

    private static XElement SecondWindMerit(StockMeritContext context, string emote, string recoveryMessage,
        TimeSpan recoveryDuration, string blurb, string description)
    {
        return MeritRoot(context, blurb, description,
            new XElement("Emote", new XCData(emote)),
            new XElement("RecoveryMessage", new XCData(recoveryMessage)),
            new XElement("RecoveryDuration", recoveryDuration.TotalSeconds));
    }

    private static XElement SkillLearningMerit(StockMeritContext context, IEnumerable<string> groups,
        double branching, double improving, Difficulty minimumDifficulty, Difficulty maximumDifficulty,
        string applicabilityProgName, string blurb, string description)
    {
        return MeritRoot(context, blurb, description, context.Prog(applicabilityProgName),
            new XAttribute("branching", branching),
            new XAttribute("improving", improving),
            new XAttribute("min_difficulty", (int)minimumDifficulty),
            new XAttribute("max_difficulty", (int)maximumDifficulty),
            new XElement("Groups",
                groups.Select(group => new XElement("Group", group))));
    }

    private static XElement SpecificCheckMerit(StockMeritContext context, CheckType checkType, double bonus,
        string blurb, string description, long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XAttribute("bonus", bonus),
            new XAttribute("type", (int)checkType));
    }

    private static XElement SpecificSpeedMerit(StockMeritContext context, double multiplier, IEnumerable<string> speeds,
        string blurb, string description, long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XAttribute("multiplier", multiplier),
            new XElement("Speeds",
                speeds.Select(speed => new XElement("Speed", new XAttribute("id", context.MoveSpeed(speed))))));
    }

    private static XElement SpecificTraitMerit(StockMeritContext context, string traitName, double bonus, string blurb,
        string description, long? applicabilityProgId = null, params TraitBonusContext[] contexts)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XAttribute("bonus", bonus),
            new XAttribute("trait", context.Trait(traitName)),
            new XElement("Contexts",
                contexts.Distinct().Select(item => new XElement("Context", (int)item))));
    }

    private static XElement SurgeryMerit(StockMeritContext context, int bonus, string blurb, string description,
        long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XAttribute("bonus", bonus));
    }

    private static XElement SwimmingStaminaMerit(StockMeritContext context, double multiplier, string blurb,
        string description, long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XAttribute("multiplier", multiplier));
    }

    private static XElement WillToLiveMerit(StockMeritContext context, double brainBonus, double heartBonus,
        double hypoxia, string blurb, string description, long? applicabilityProgId = null)
    {
        return MeritRoot(context, blurb, description, applicabilityProgId,
            new XAttribute("brainbonus", brainBonus),
            new XAttribute("heartbonus", heartBonus),
            new XAttribute("hypoxia", hypoxia));
    }

    private static XElement MeritRoot(StockMeritContext context, string blurb, string description,
        params object[] content)
    {
        return MeritRoot(context, blurb, description, null, content);
    }

    private static XElement MeritRoot(StockMeritContext context, string blurb, string description,
        long? applicabilityProgId, params object[] content)
    {
        List<object> nodes = new()
        {
            new XElement("ChargenAvailableProg", context.AlwaysTrueProgId),
            new XElement("ApplicabilityProg", applicabilityProgId ?? context.AlwaysTrueProgId),
            new XElement("ChargenBlurb", new XCData(blurb)),
            new XElement("DescriptionText", new XCData(description))
        };
        nodes.AddRange(content);
        return new XElement("Merit", nodes.ToArray());
    }

    private sealed record StockMeritBlueprint(
        string Name,
        string Type,
        MeritType MeritType,
        Func<StockMeritContext, XElement> Definition);

    private sealed class StockMeritContext
    {
        private readonly FuturemudDatabaseContext _context;

        public StockMeritContext(FuturemudDatabaseContext context, long alwaysTrueProgId)
        {
            _context = context;
            AlwaysTrueProgId = alwaysTrueProgId;
        }

        public long AlwaysTrueProgId { get; }

        public long Prog(string functionName)
        {
            return _context.FutureProgs.AsEnumerable()
                .First(x => x.FunctionName.Equals(functionName, StringComparison.OrdinalIgnoreCase))
                .Id;
        }

        public long Trait(string traitName)
        {
            return _context.TraitDefinitions.AsEnumerable()
                .First(x => x.Name.Equals(traitName, StringComparison.OrdinalIgnoreCase))
                .Id;
        }

        public long MoveSpeed(string aliasOrName)
        {
            return _context.MoveSpeeds.AsEnumerable()
                .First(x =>
                    x.Alias.Equals(aliasOrName, StringComparison.OrdinalIgnoreCase))
                .Id;
        }
    }
}
