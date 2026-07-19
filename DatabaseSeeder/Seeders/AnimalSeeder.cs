#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder : IDatabaseSeeder
{
    private readonly Dictionary<string, WeaponAttack> _attacks =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, BodypartProto> _cachedBodyparts =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly List<(BodypartProto Child, BodypartProto Parent)> _cachedBodypartUpstreams =
        new();

    private readonly Dictionary<string, BodypartProto> _cachedBones =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly CollectionDictionary<string, BodypartProto> _cachedLimbs =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, Material> _cachedMaterials =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, BodypartProto> _cachedOrgans =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, BodypartShape> _cachedShapes =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly List<Liquid> _freshWaters = new();
    private static readonly string[] AvianCoreWingAliases =
    [
        "rwingbase",
        "lwingbase",
        "rwing",
        "lwing"
    ];

    private readonly Dictionary<string, HeightWeightModel> _hwModels = new(StringComparer.OrdinalIgnoreCase);

    private FutureProg _alwaysFalse = null!;
    private FutureProg _alwaysTrue = null!;
    private IEnumerable<TraitDefinition> _attributes = null!;
    private Liquid _bloodLiquid = null!;
    private ArmourType _boneArmour = null!;

    private Liquid _brackishWater = null!;

    private Gas _breathableAir = null!;
    private FuturemudDatabaseContext _context = null!;

    private CorpseModel _defaultCorpseModel = null!;
    private HealthStrategy _healthStrategy = null!;
    private TraitDefinition _healthTrait = null!;

    private ArmourType _naturalArmour = null!;
    private ArmourType _organArmour = null!;
    private IReadOnlyDictionary<string, string> _questionAnswers = null!;
    private Liquid _saltWater = null!;
	private bool _sever;
	private TraitExpression _snakeBiteDamage = null!;

	private readonly Stopwatch _stopwatch = new();
	private TraitDefinition _strengthTrait = null!;
	private Liquid _sweatLiquid = null!;

    internal static IReadOnlyList<string> AvianCoreWingAliasesForTesting => AvianCoreWingAliases;

	private static readonly string[] AnimalMinorSeverKeywords =
	[
		"eye",
		"ear",
		"antenna",
		"fang",
		"horn",
		"hoof",
		"claw",
		"stinger",
		"mandible"
	];

	private static string BuildNaturalArmourDefinition(
		IEnumerable<(DamageType From, DamageType To, WoundSeverity Threshold)> transforms,
		Func<DamageType, string> damageDissipate,
		Func<DamageType, string> painDissipate,
		Func<DamageType, string> stunDissipate,
		Func<DamageType, string> damageAbsorb,
		Func<DamageType, string> painAbsorb,
		Func<DamageType, string> stunAbsorb)
	{
		static XElement BuildExpressionSet(string name, Func<DamageType, string> factory)
		{
			return new XElement(name,
				Enum.GetValues(typeof(DamageType))
					.OfType<DamageType>()
					.Select(type => new XElement("Expression",
						new XAttribute("damagetype", (int)type),
						factory(type))));
		}

		var root = new XElement("ArmourType",
			new XElement("DamageTransformations",
				transforms.Select(x => new XElement("Transform",
					new XAttribute("fromtype", (int)x.From),
					new XAttribute("totype", (int)x.To),
					new XAttribute("severity", (int)x.Threshold)))),
			BuildExpressionSet("DissipateExpressions", damageDissipate),
			BuildExpressionSet("DissipateExpressionsPain", painDissipate),
			BuildExpressionSet("DissipateExpressionsStun", stunDissipate),
			BuildExpressionSet("AbsorbExpressions", damageAbsorb),
			BuildExpressionSet("AbsorbExpressionsPain", painAbsorb),
			BuildExpressionSet("AbsorbExpressionsStun", stunAbsorb)
		);

		return root.ToString(SaveOptions.DisableFormatting);
	}

	private static IEnumerable<(DamageType From, DamageType To, WoundSeverity Threshold)> RelaxedAnimalFleshDamageTransforms()
	{
		yield return (DamageType.Slashing, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Chopping, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Piercing, DamageType.Crushing, WoundSeverity.Superficial);
		yield return (DamageType.Ballistic, DamageType.Crushing, WoundSeverity.Superficial);
		yield return (DamageType.Bite, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Claw, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Shearing, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Wrenching, DamageType.Crushing, WoundSeverity.Small);
		yield return (DamageType.Shrapnel, DamageType.Crushing, WoundSeverity.Superficial);
		yield return (DamageType.ArmourPiercing, DamageType.ArmourPiercing, WoundSeverity.Horrifying);
	}

	private static bool IsAnimalCutLikeDamage(DamageType damageType)
	{
		return damageType is DamageType.Slashing or
			DamageType.Chopping or
			DamageType.Piercing or
			DamageType.Ballistic or
			DamageType.Bite or
			DamageType.Claw or
			DamageType.Shearing or
			DamageType.BallisticArmourPiercing or
			DamageType.ArmourPiercing or
			DamageType.Shrapnel;
	}

	private static bool IsAnimalImpactLikeDamage(DamageType damageType)
	{
		return damageType is DamageType.Crushing or
			DamageType.Shockwave or
			DamageType.Sonic or
			DamageType.Wrenching or
			DamageType.Falling;
	}

	private static string AnimalNaturalDissipateExpression(DamageType damageType, string valueName)
	{
		if (IsAnimalCutLikeDamage(damageType))
		{
			return $"{valueName} - (quality * strength/25000 * 0.75)";
		}

		if (IsAnimalImpactLikeDamage(damageType))
		{
			return $"{valueName} - (quality * strength/10000 * 0.75)";
		}

		return $"{valueName} - (quality * 0.75)";
	}

	private static string AnimalNaturalDamageAbsorbExpression(DamageType damageType, string valueName)
	{
		return damageType switch
		{
			DamageType.Hypoxia or DamageType.Cellular => "0",
			DamageType.Slashing or
			DamageType.Chopping or
			DamageType.Piercing or
			DamageType.Ballistic or
			DamageType.Bite or
			DamageType.Claw or
			DamageType.Shearing or
			DamageType.BallisticArmourPiercing or
			DamageType.ArmourPiercing or
			DamageType.Shrapnel => $"{valueName}*0.9",
			DamageType.Crushing or
			DamageType.Shockwave or
			DamageType.Sonic or
			DamageType.Wrenching or
			DamageType.Falling => $"{valueName}*0.85",
			_ => $"{valueName}*0.8"
		};
	}

	private static string AnimalNaturalStunAbsorbExpression(DamageType damageType, string valueName)
	{
		return damageType switch
		{
			DamageType.Hypoxia or DamageType.Cellular => "0",
			_ => valueName
		};
	}

	private static int NormalizeAnimalSeverThreshold(string alias, int severThreshold, SizeCategory size)
	{
		if (severThreshold <= 0)
		{
			return severThreshold;
		}

		if (AnimalMinorSeverKeywords.Any(alias.Contains))
		{
			return Math.Min(severThreshold, 18);
		}

		return size switch
		{
			SizeCategory.Tiny => Math.Min(severThreshold, 12),
			SizeCategory.VerySmall => Math.Min(severThreshold, 18),
			_ => Math.Min(severThreshold, 27)
		};
	}

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
        Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions
        => NonHumanSeederQuestions.GetQuestions();

    #region Core Methods
    public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
    {
        context.Database.BeginTransaction();
        _context = context;
        _questionAnswers = CombatBalanceProfileHelper.MergeQuestionAnswersWithRecordedChoice(context, questionAnswers);
        _questionAnswers = CombatSeederMessageStyleHelper.MergeQuestionAnswersWithRecordedChoice(context, _questionAnswers);
        _combatBalanceProfile = CombatBalanceProfileHelper.GetSelectedProfile(context, _questionAnswers);
        bool hasMissingDisfigurementTemplates = HasMissingAnimalDisfigurementTemplates(_context);
        if (_context.BodyProtos.Any(x => x.Name == "Quadruped Base"))
        {
            ResetSeeder();
            _healthTrait = context.TraitDefinitions
                .Where(x => x.Type == 1)
                .AsEnumerable()
                .First(x => x.Name.In("Constitution", "Body", "Physique", "Endurance", "Hardiness", "Stamina"));
            _strengthTrait = context.TraitDefinitions
                .Where(x => x.Type == 1)
                .AsEnumerable()
                .First(x => x.Name.In("Strength", "Physique", "Body", "Upper Body Strength"));
            RefreshExistingAnimalBaseBodies();
            bool hasMissingAnimalWearProfiles = HasMissingAnimalWearProfiles(_context);
            bool hasMissingCatalogue = HasMissingAnimalCatalogue(_context);
            bool hasMissingAnimalAiTemplates = HasMissingAnimalAIStockTemplates(_context);
            bool hasMissingDietSettings = HasMissingAnimalDietSettings(_context);
            RefreshExistingAnimalCombatBalance();

            if (hasMissingDisfigurementTemplates)
            {
                SeedExistingAnimalDisfigurementTemplates();
            }

            if (hasMissingCatalogue)
            {
                BackfillAnimalCatalogue();
            }

            bool updatedAnimalWearProfiles = EnsureStockUngulateWearProfiles();

            if (hasMissingAnimalAiTemplates)
            RefreshExistingAnimalDietSettings();
            {
                SeedAnimalAIStockTemplates();
            }

            RefreshExistingAnimalDietSettings();
            CombatAuxiliarySeedResult auxiliaryResult = CombatAuxiliarySeederHelper.EnsureAnimalAuxiliaryLinks(context);
            context.Database.CommitTransaction();
            List<string> updates = ["Updated the animal combat balance profile"];
            if (hasMissingCatalogue)
            {
                updates.Add("backfilled missing animal catalogue content");
            }

            if (hasMissingAnimalWearProfiles || updatedAnimalWearProfiles)
            {
                updates.Add("installed or repaired animal wear profiles");
            }

            if (hasMissingDisfigurementTemplates)
            {
                updates.Add("installed additional animal disfigurement templates");
            }

            if (hasMissingAnimalAiTemplates)
            {
                updates.Add("installed stock animal AI templates");
            }

            if (hasMissingDietSettings)
            {
                updates.Add("refreshed stock animal diet settings");
            }

            if (auxiliaryResult.HasChanges)
            {
                updates.Add($"refreshed animal auxiliary combat links ({auxiliaryResult.RaceLinks} links)");
            }

            return $"{string.Join(", ", updates)}.";
        }

        Console.WriteLine("Performing initial setup...");
        _stopwatch.Start();
        ResetSeeder();
        SetupCorpseModel();
        SeedCombatStrategies();
        _sever = _questionAnswers.ContainsKey("sever") && _questionAnswers["sever"].ToLowerInvariant().In("yes", "y");

        bool firsttime = _context.TraitExpressions.All(x => x.Name != "Non-Human Max HP Formula");

        #region Health Strategy

        _healthTrait = context.TraitDefinitions
            .Where(x => x.Type == 1)
            .AsEnumerable()
            .First(x => x.Name.In("Constitution", "Body", "Physique", "Endurance", "Hardiness", "Stamina"));

        TraitExpression hpExpression = new()
        {
            Name = "Non-Human Max HP Formula",
            Expression = $"100+con:{_healthTrait.Id}"
        };
        context.TraitExpressions.Add(hpExpression);
        TraitExpression hpTick = new()
        {
            Name = "Non-Human HP Heal Per Tick",
            Expression = $"100+con:{_healthTrait.Id}"
        };
        context.TraitExpressions.Add(hpTick);
        TraitDefinition secondaryTrait = context.TraitDefinitions
                                 .Where(x => x.Type == 1)
                                 .AsEnumerable()
                                 .FirstOrDefault(x => x.Name.In("Willpower", "Resilience", "Mind")) ??
                             _healthTrait;

        _strengthTrait = context.TraitDefinitions
            .Where(x => x.Type == 1)
            .AsEnumerable()
            .First(x => x.Name.In("Strength", "Physique", "Body", "Upper Body Strength"));
        context.SaveChanges();

        HealthStrategy strategy = SetupHealthModel(context, _questionAnswers, hpExpression, hpTick, secondaryTrait);

        #endregion

        // Progs
        FutureProg staminaRecoveryProg = new()
        {
            FunctionName = "NonHumanStaminaRecovery",
            Category = "Character",
            Subcategory = "Stamina",
            FunctionComment = "Determines the stamina gain per 10 seconds for non-humans",
            ReturnType = 2,
            AcceptsAnyParameters = false,
            Public = true,
            StaticType = 0,
            FunctionText = $"return GetTrait(@ch, ToTrait({_healthTrait.Id}))"
        };
        staminaRecoveryProg.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = staminaRecoveryProg,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = 8200
        });
        context.FutureProgs.Add(staminaRecoveryProg);
        context.SaveChanges();

        // Cultures
        Culture animalCulture = new()
        {
            Name = "Animal",
            Description = AnimalCultureDescriptionForTesting,
            PersonWordMale = "Animal",
            PersonWordFemale = "Animal",
            PersonWordIndeterminate = "Animal",
            PersonWordNeuter = "Animal",
            SkillStartingValueProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysZero"),
            AvailabilityProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse"),
            TolerableTemperatureCeilingEffect = 0,
            TolerableTemperatureFloorEffect = 0,
            PrimaryCalendarId = context.Calendars.First().Id
        };
        context.Cultures.Add(animalCulture);
        NameCulture simpleNC = context.NameCultures.First(x => x.Name == "Simple");
        animalCulture.CulturesNameCultures.Add(new CulturesNameCultures
        { Culture = animalCulture, NameCulture = simpleNC, Gender = (short)Gender.Male });
        animalCulture.CulturesNameCultures.Add(new CulturesNameCultures
        { Culture = animalCulture, NameCulture = simpleNC, Gender = (short)Gender.Female });
        animalCulture.CulturesNameCultures.Add(new CulturesNameCultures
        { Culture = animalCulture, NameCulture = simpleNC, Gender = (short)Gender.Neuter });
        animalCulture.CulturesNameCultures.Add(new CulturesNameCultures
        { Culture = animalCulture, NameCulture = simpleNC, Gender = (short)Gender.NonBinary });
        animalCulture.CulturesNameCultures.Add(new CulturesNameCultures
        { Culture = animalCulture, NameCulture = simpleNC, Gender = (short)Gender.Indeterminate });

        context.SaveChanges();

        SetupHeightWeightModels();

        // Body Prototypes
        WearableSizeParameterRule wearSize = new()
        {
            BodyProtoId = 0,
            MinHeightFactor = 0.5,
            MaxHeightFactor = 1.5,
            MinWeightFactor = 0.5,
            MaxWeightFactor = 1.5,
            IgnoreTrait = true,
            WeightVolumeRatios =
                "<Ratios><Ratio Item=\"5\" Min=\"0\" Max=\"0.75\"/><Ratio Item=\"4\" Min=\"0.75\" Max=\"0.95\"/><Ratio Item=\"3\" Min=\"0.95\" Max=\"0.99\"/><Ratio Item=\"2\" Min=\"0.99\" Max=\"1.01\"/><Ratio Item=\"3\" Min=\"1.01\" Max=\"1.05\"/><Ratio Item=\"1\" Min=\"1.05\" Max=\"1.3\"/><Ratio Item=\"0\" Min=\"1.3\" Max=\"1000\"/></Ratios>",
            TraitVolumeRatios =
                "<Ratios><Ratio Item=\"5\" Min=\"0\" Max=\"0.75\"/><Ratio Item=\"4\" Min=\"0.75\" Max=\"0.95\"/><Ratio Item=\"3\" Min=\"0.95\" Max=\"0.99\"/><Ratio Item=\"2\" Min=\"0.99\" Max=\"1.01\"/><Ratio Item=\"3\" Min=\"1.01\" Max=\"1.05\"/><Ratio Item=\"1\" Min=\"1.05\" Max=\"1.3\"/><Ratio Item=\"0\" Min=\"1.3\" Max=\"1000\"/></Ratios>",
            HeightLinearRatios =
                "<Ratios><Ratio Item=\"0\" Min=\"0\" Max=\"0.90\"/><Ratio Item=\"1\" Min=\"0.90\" Max=\"0.95\"/><Ratio Item=\"2\" Min=\"0.95\" Max=\"0.99\"/><Ratio Item=\"3\" Min=\"0.99\" Max=\"1.01\"/><Ratio Item=\"2\" Min=\"1.01\" Max=\"1.05\"/><Ratio Item=\"4\" Min=\"1.05\" Max=\"1.1\"/><Ratio Item=\"5\" Min=\"1.1\" Max=\"1000\"/></Ratios>"
        };
        context.WearableSizeParameterRule.Add(wearSize);
        context.SaveChanges();

        SetupArmourTypes();
        SetupShapesAndMaterials();
        SetupAttacks(firsttime);

        long nextId = context.BodyProtos.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;
        Console.WriteLine("Installing quadrupeds...");
        BodyProto quadrupedBody = new()
        {
            Id = nextId++,
            Name = "Quadruped Base",
            ConsiderString = "",
            WielderDescriptionSingle = "mouth",
            WielderDescriptionPlural = "mouths",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 3,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(quadrupedBody);
        context.SaveChanges();

        BodyProto ungulateBody = new()
        {
            Id = nextId++,
            CountsAs = quadrupedBody,
            Name = "Ungulate",
            ConsiderString = "",
            WielderDescriptionSingle = "mouth",
            WielderDescriptionPlural = "mouths",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 3,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(ungulateBody);
        context.SaveChanges();

        BodyProto toedQuadruped = new()
        {
            Id = nextId++,
            CountsAs = quadrupedBody,
            Name = "Toed Quadruped",
            ConsiderString = "",
            WielderDescriptionSingle = "mouth",
            WielderDescriptionPlural = "mouths",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 3,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(toedQuadruped);
        context.SaveChanges();

        SeedQuadruped(quadrupedBody, ungulateBody, toedQuadruped);
        EnsureStockUngulateWearProfiles(ungulateBody);

        Console.WriteLine("Installing avians...");
        BodyProto avianBody = new()
        {
            Id = nextId++,
            Name = "Avian",
            ConsiderString = "",
            WielderDescriptionSingle = "talon",
            WielderDescriptionPlural = "talons",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 2,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(avianBody);
        context.SaveChanges();
        SeedAvian(avianBody);

        Console.WriteLine("Installing serpents...");
        BodyProto wormBody = new()
        {
            Id = nextId++,
            Name = "Vermiform",
            ConsiderString = "",
            WielderDescriptionSingle = "mouth",
            WielderDescriptionPlural = "mouths",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 3,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(wormBody);
        context.SaveChanges();

        BodyProto serpentineBody = new()
        {
            Id = nextId++,
            CountsAs = wormBody,
            Name = "Serpentine",
            ConsiderString = "",
            WielderDescriptionSingle = "mouth",
            WielderDescriptionPlural = "mouths",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 3,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(serpentineBody);
        context.SaveChanges();
        SeedSerpents(wormBody, serpentineBody);

        Console.WriteLine("Installing aquatic animals...");
        BodyProto fishBody = new()
        {
            Id = nextId++,
            Name = "Piscine",
            ConsiderString = "",
            WielderDescriptionSingle = "mouth",
            WielderDescriptionPlural = "mouths",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 3,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(fishBody);
        context.SaveChanges();

        BodyProto crabBody = new()
        {
            Id = nextId++,
            Name = "Decapod",
            ConsiderString = "",
            WielderDescriptionSingle = "pincer",
            WielderDescriptionPlural = "pincers",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 5,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(crabBody);
        context.SaveChanges();

        BodyProto malacostracanBody = new()
        {
            Id = nextId++,
            CountsAs = crabBody,
            Name = "Malacostracan",
            ConsiderString = "",
            WielderDescriptionSingle = "claw",
            WielderDescriptionPlural = "claws",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 4,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(malacostracanBody);
        context.SaveChanges();

        BodyProto cephalopod = new()
        {
            Id = nextId++,
            Name = "Cephalopod",
            ConsiderString = "",
            WielderDescriptionSingle = "tentacle",
            WielderDescriptionPlural = "tentacles",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 3,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "tentacle",
            LegDescriptionSingular = "tentacles",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(cephalopod);
        context.SaveChanges();

        BodyProto jellyfish = new()
        {
            Id = nextId++,
            Name = "Jellyfish",
            ConsiderString = "",
            WielderDescriptionSingle = "tentacle",
            WielderDescriptionPlural = "tentacles",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 3,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(jellyfish);
        context.SaveChanges();

        BodyProto pinniped = new()
        {
            Id = nextId++,
            Name = "Pinniped",
            ConsiderString = "",
            WielderDescriptionSingle = "mouth",
            WielderDescriptionPlural = "mouths",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 3,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(pinniped);
        context.SaveChanges();

        BodyProto cetacean = new()
        {
            Id = nextId++,
            Name = "Cetacean",
            ConsiderString = "",
            WielderDescriptionSingle = "mouth",
            WielderDescriptionPlural = "mouths",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 3,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(cetacean);
        context.SaveChanges();

        SeedAquatic(fishBody, crabBody, malacostracanBody, cephalopod, jellyfish, pinniped, cetacean);

        Console.WriteLine("Installing insectoids...");
        BodyProto insectBody = new()
        {
            Id = nextId++,
            Name = "Insectoid",
            ConsiderString = "",
            WielderDescriptionSingle = "mandible",
            WielderDescriptionPlural = "mandibles",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 6,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(insectBody);
        context.SaveChanges();

        BodyProto wingedInsectBody = new()
        {
            Id = nextId++,
            Name = "Winged Insectoid",
            ConsiderString = "",
            WielderDescriptionSingle = "mandible",
            WielderDescriptionPlural = "mandibles",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 6,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(wingedInsectBody);
        context.SaveChanges();

        BodyProto beetleBody = new()
        {
            Id = nextId++,
            CountsAs = insectBody,
            Name = "Beetle",
            ConsiderString = "",
            WielderDescriptionSingle = "mandible",
            WielderDescriptionPlural = "mandibles",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 6,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(beetleBody);
        context.SaveChanges();

        BodyProto centipedeBody = new()
        {
            Id = nextId++,
            CountsAs = insectBody,
            Name = "Centipede",
            ConsiderString = "",
            WielderDescriptionSingle = "mandible",
            WielderDescriptionPlural = "mandibles",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 8,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(centipedeBody);
        context.SaveChanges();

        SeedInsectoid(insectBody);
        SeedWingedInsectoid(wingedInsectBody);
        SeedBeetle(insectBody, beetleBody);
        SeedCentipede(centipedeBody);

        BodyProto arachnidBody = new()
        {
            Id = nextId++,
            CountsAs = insectBody,
            Name = "Arachnid",
            ConsiderString = "",
            WielderDescriptionSingle = "fang",
            WielderDescriptionPlural = "fangs",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 6,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(arachnidBody);
        context.SaveChanges();

        BodyProto scorpionBody = new()
        {
            Id = nextId++,
            CountsAs = arachnidBody,
            Name = "Scorpion",
            ConsiderString = "",
            WielderDescriptionSingle = "claw",
            WielderDescriptionPlural = "claws",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 6,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(scorpionBody);
        context.SaveChanges();

        SeedArachnidBody(arachnidBody, false);
        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");
        SeedAnimalRaces(GetArachnidRaceTemplates().Where(x => x.BodyKey == "Arachnid"),
            ("Arachnid", arachnidBody));

        SeedArachnidBody(scorpionBody, true);
        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");
        SeedAnimalRaces(GetArachnidRaceTemplates().Where(x => x.BodyKey == "Scorpion"),
            ("Scorpion", scorpionBody));

        BodyProto reptilianBody = new()
        {
            Id = nextId++,
            CountsAs = toedQuadruped,
            Name = "Reptilian",
            ConsiderString = "",
            WielderDescriptionSingle = "mouth",
            WielderDescriptionPlural = "mouths",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 3,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(reptilianBody);
        context.SaveChanges();

        BodyProto anuranBody = new()
        {
            Id = nextId++,
            CountsAs = reptilianBody,
            Name = "Anuran",
            ConsiderString = "",
            WielderDescriptionSingle = "mouth",
            WielderDescriptionPlural = "mouths",
            StaminaRecoveryProgId = staminaRecoveryProg.Id,
            MinimumLegsToStand = 2,
            MinimumWingsToFly = 2,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WearSizeParameter = wearSize
        };
        context.BodyProtos.Add(anuranBody);
        context.SaveChanges();

        SeedReptilianBodies(toedQuadruped, reptilianBody, anuranBody);
        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");
        SeedAnimalRaces(GetReptileAmphibianRaceTemplates(),
            ("Reptilian", reptilianBody),
            ("Anuran", anuranBody));

        SetupPositions(quadrupedBody, avianBody, serpentineBody, fishBody, crabBody, cephalopod, jellyfish, pinniped,
                cetacean, wormBody, insectBody, wingedInsectBody);
        SetupSpeeds(quadrupedBody, avianBody, serpentineBody, fishBody, crabBody, cephalopod, jellyfish, pinniped,
                cetacean, wormBody, insectBody, wingedInsectBody);
        CloneBodyPositionsAndSpeeds(crabBody, malacostracanBody);
        CloneBodyPositionsAndSpeeds(insectBody, arachnidBody);
        CloneBodyPositionsAndSpeeds(insectBody, scorpionBody);
        CloneBodyPositionsAndSpeeds(insectBody, beetleBody);
        CloneBodyPositionsAndSpeeds(insectBody, centipedeBody);
        CloneBodyPositionsAndSpeeds(toedQuadruped, reptilianBody);
        CloneBodyPositionsAndSpeeds(toedQuadruped, anuranBody);
        ApplyDefaultCombatSettingsToSeededRaces();
        SeedAnimalAIStockTemplates();
        CombatAuxiliarySeedResult freshAuxiliaryResult = CombatAuxiliarySeederHelper.EnsureAnimalAuxiliaryLinks(context);

        context.Database.CommitTransaction();

        return freshAuxiliaryResult.HasChanges
            ? $"Successfully installed animal prototypes, stock animal AI templates, and {freshAuxiliaryResult.RaceLinks} animal auxiliary combat links."
            : "Successfully installed animal prototypes and stock animal AI templates.";
    }

    public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
    {
        if (!context.BodyProtos.Any(x => x.Name == "Humanoid") || !context.NameCultures.Any(x => x.Name == "Simple"))
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        if (context.BodyProtos.Any(x => x.Name == "Quadruped Base"))
        {
            return HasMissingAnimalDisfigurementTemplates(context) ||
                   HasMissingAnimalCatalogue(context) ||
                   HasMissingAnimalWearProfiles(context) ||
                   HasMissingAnimalAIStockTemplates(context) ||
                   HasMissingAnimalDietSettings(context)
                ? ShouldSeedResult.ExtraPackagesAvailable
                : ShouldSeedResult.MayAlreadyBeInstalled;
        }

        return ShouldSeedResult.ReadyToInstall;
    }
    private void ResetSeeder()
    {
        _attacks.Clear();
        _cachedBodyparts.Clear();
        _cachedBodypartUpstreams.Clear();
        _cachedLimbs.Clear();
        _cachedOrgans.Clear();
        _cachedBones.Clear();
        _cachedMaterials.Clear();
        _cachedShapes.Clear();
        _hwModels.Clear();
        _sever = false;
        _breathableAir = _context.Gases.First();
        // TODO - make these optional
        _saltWater = _context.Liquids.First(x => x.Name == "salt water");
        _brackishWater = _context.Liquids.First(x => x.Name == "brackish water");
        _freshWaters.Clear();
        _freshWaters.Add(_context.Liquids.First(x => x.Name == "swamp water"));
        _freshWaters.Add(_context.Liquids.First(x => x.Name == "lake water"));
        _freshWaters.Add(_context.Liquids.First(x => x.Name == "river water"));
        _freshWaters.Add(_context.Liquids.First(x => x.Name == "pool water"));
        _freshWaters.Add(_context.Liquids.First(x => x.Name == "spring water"));
        _freshWaters.Add(_context.Liquids.First(x => x.Name == "tap water"));
        _freshWaters.Add(_context.Liquids.First(x => x.Name == "rain water"));
        _freshWaters.Add(_context.Liquids.First(x => x.Name == "water"));
			var bloodLiquid = _context.Liquids.FirstOrDefault(x => x.Name == "blood");
			if (bloodLiquid is null)
			{
				Material driedBlood = new()
            {
                Name = "dried Blood",
                MaterialDescription = "dried blood",
                Density = 1520,
                Organic = true,
                Type = 0,
                BehaviourType = 19,
                ThermalConductivity = 0.2,
                ElectricalConductivity = 0.0001,
                SpecificHeatCapacity = 420,
                IgnitionPoint = 555.3722,
                HeatDamagePoint = 412.0389,
                ImpactFracture = 1000,
                ImpactYield = 1000,
                ImpactStrainAtYield = 2,
                ShearFracture = 1000,
                ShearYield = 1000,
                ShearStrainAtYield = 2,
                YoungsModulus = 0.1,
                SolventId = 1,
                SolventVolumeRatio = 4,
                ResidueDesc = "It is covered in {0}dried blood",
                ResidueColour = "red",
                Absorbency = 0
            };
            _context.Materials.Add(driedBlood);
            Liquid blood = new()
            {
                Name = "Blood",
                Description = "blood",
                LongDescription = "a virtually opaque dark red fluid",
                TasteText = "It has a sharply metallic, umami taste",
                VagueTasteText = "It has a metallic taste",
                SmellText = "It has a metallic, coppery smell",
                VagueSmellText = "It has a faintly metallic smell",
                TasteIntensity = 200,
                SmellIntensity = 10,
                AlcoholLitresPerLitre = 0,
                WaterLitresPerLitre = 0.8,
                DrinkSatiatedHoursPerLitre = 6,
                FoodSatiatedHoursPerLitre = 4,
                Viscosity = 1,
                Density = 1,
                Organic = true,
                ThermalConductivity = 0.609,
                ElectricalConductivity = 0.005,
                SpecificHeatCapacity = 4181,
                FreezingPoint = -20,
                BoilingPoint = 100,
                DisplayColour = "bold red",
                DampDescription = "It is damp with blood",
                WetDescription = "It is wet with blood",
                DrenchedDescription = "It is drenched with blood",
                DampShortDescription = "(blood damp)",
                WetShortDescription = "(bloody)",
                DrenchedShortDescription = "(blood drenched)",
                SolventId = 1,
                SolventVolumeRatio = 5,
                InjectionConsequence = (int)LiquidInjectionConsequence.BloodReplacement,
                ResidueVolumePercentage = 0.05,
                DriedResidue = driedBlood
				};
				_context.Liquids.Add(blood);
				bloodLiquid = blood;
			}
			_bloodLiquid = bloodLiquid;

			var sweatLiquid = _context.Liquids.FirstOrDefault(x => x.Name == "sweat");
			if (sweatLiquid is null)
			{
				Material driedSweat = new()
            {
                Name = "dried Sweat",
                MaterialDescription = "dried sweat",
                Density = 1520,
                Organic = true,
                Type = 0,
                BehaviourType = 19,
                ThermalConductivity = 0.2,
                ElectricalConductivity = 0.0001,
                SpecificHeatCapacity = 420,
                IgnitionPoint = 555.3722,
                HeatDamagePoint = 412.0389,
                ImpactFracture = 1000,
                ImpactYield = 1000,
                ImpactStrainAtYield = 2,
                ShearFracture = 1000,
                ShearYield = 1000,
                ShearStrainAtYield = 2,
                YoungsModulus = 0.1,
                SolventId = 1,
                SolventVolumeRatio = 3,
                ResidueDesc = "It is covered in {0}dried sweat",
                ResidueColour = "yellow",
                Absorbency = 0
            };
            _context.Materials.Add(driedSweat);
            Liquid sweat = new()
            {
                Name = "Sweat",
                Description = "sweat",
                LongDescription = "a relatively clear, translucent fluid that smells strongly of body odor",
                TasteText = "It tastes like a pungent, salty lick of someone's underarms",
                VagueTasteText = "It tastes very unpleasant, like underarm stench",
                SmellText = "It has the sharp, pungent smell of body odor",
                VagueSmellText = "It has the sharp, pungent smell of body odor",
                TasteIntensity = 200,
                SmellIntensity = 200,
                AlcoholLitresPerLitre = 0,
                WaterLitresPerLitre = 0.95,
                DrinkSatiatedHoursPerLitre = 5,
                FoodSatiatedHoursPerLitre = 0,
                Viscosity = 1,
                Density = 1,
                Organic = true,
                ThermalConductivity = 0.609,
                ElectricalConductivity = 0.005,
                SpecificHeatCapacity = 4181,
                FreezingPoint = -20,
                BoilingPoint = 100,
                DisplayColour = "yellow",
                DampDescription = "It is damp with sweat",
                WetDescription = "It is wet and smelly with sweat",
                DrenchedDescription = "It is soaking wet and smelly with sweat",
                DampShortDescription = "(sweat-damp)",
                WetShortDescription = "(sweaty)",
                DrenchedShortDescription = "(sweat-drenched)",
                SolventId = 1,
                SolventVolumeRatio = 5,
                InjectionConsequence = (int)LiquidInjectionConsequence.Harmful,
                ResidueVolumePercentage = 0.05,
                DriedResidue = driedSweat
				};
				_context.Liquids.Add(sweat);
				sweatLiquid = sweat;
			}
			_sweatLiquid = sweatLiquid;

        _alwaysFalse = _context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse");
        _alwaysTrue = _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
        _attributes = _context.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3).ToList();
    }

    private void ResetCachedParts()
    {
        _cachedBodyparts.Clear();
        _cachedBodypartUpstreams.Clear();
        _cachedBones.Clear();
        _cachedLimbs.Clear();
        _cachedOrgans.Clear();
    }
    #endregion
    public int SortOrder => 300;
    public string Name => "Animal Seeder";
    public string Tagline => "Installs body types for animals";

    public string FullDescription =>
        @"This seeder will install a variety of animal bodies, races, descriptions and attacks. It currently includes a variety of terrestrial animals that can easily be extended to include fantasy or science-fiction races.

Warning: There is an enormous amount of data contained in this seeder, and it may take a long time to run.";

    public bool Enabled => true;



    private void SetupCorpseModel()
    {
        _defaultCorpseModel = new CorpseModel
        {
            Name = "Organic Animal Corpse",
            Description = "This corpse will decay over time and eventually disappear",
            Type = "Standard",
            Definition = @"<?xml version=""1.0""?>
  <CorpseModel>
	<EdiblePercentage>0.35</EdiblePercentage>
	<Ranges>
	  <Range state=""0"" lower=""0"" upper=""7200""/>
	  <Range state=""1"" lower=""7200"" upper=""21600""/>
	  <Range state=""2"" lower=""21600"" upper=""65000""/>
	  <Range state=""3"" lower=""65000"" upper=""400000""/>
	  <Range state=""4"" lower=""400000"" upper=""2650000""/>
	  <Range state=""5"" lower=""2650000"" upper=""2650001""/>
	</Ranges>
	<Terrains default=""10"">
	  <Terrain terrain=""void"" rate=""0""/>
	</Terrains>
	<Descriptions>
	  <PartDescriptions>
	  <Description state=""0""><![CDATA[&a_an[@@shorteat[, ]freshly severed {1} {0}]]]></Description>
	  <Description state=""1""><![CDATA[&a_an[@@shorteat[, ]severed {1} {0}]]]></Description>
	  <Description state=""2""><![CDATA[&a_an[@@shorteat[, ]decaying severed {1} {0}]]]></Description>
	  <Description state=""3""><![CDATA[&a_an[@@shorteat[, ]decayed severed {0}]]]></Description>
	  <Description state=""4""><![CDATA[&a_an[@@shorteat[, ]heavily decayed severed {0}]]]></Description>
	  <Description state=""5""><![CDATA[the @@shorteat[, ]skeletal remains of &a_an[severed {0}]]]></Description>
	</PartDescriptions>
	  <ShortDescriptions>
		<Description state=""0""><![CDATA[the @@shorteat[, ]fresh corpse of @@sdesc]]></Description>
		<Description state=""1""><![CDATA[the @@shorteat[, ]corpse of @@sdesc]]></Description>
		<Description state=""2""><![CDATA[the @@shorteat[, ]decaying corpse of @@sdesc]]></Description>
		<Description state=""3""><![CDATA[the @@shorteat[, ]decayed corpse of &a_an[&male &race]]]></Description>
		<Description state=""4""><![CDATA[the @@shorteat[, ]heavily decayed &race corpse]]></Description>
		<Description state=""5""><![CDATA[the @@shorteat[, ]skeletal remains of &a_an[&race]]]></Description>
	  </ShortDescriptions>
	  <FullDescriptions>
		<Description state=""0""><![CDATA[@@desc
  #3Death has taken hold of this critter, but it still looks much the same as it did in life.#0
  
  @@eaten[
  
  ]@@wounds
  
  @@inv]]></Description>
		<Description state=""1""><![CDATA[@@desc
  #3Rigor Mortis has set in with this critter, and the blood has begun to pool in the extremities. Flies and other carrion insects have begun to lay eggs in its skin.#0
  
  @@eaten[
  
  ]@@wounds
  
  @@inv]]></Description>
		<Description state=""2""><![CDATA[@@desc
  #3This corpse has begun to bloat and putrefy as decay sets in. Maggots and other carrion insects have firmly taken hold of the corpse.#0
  
  @@eaten[
  
  ]@@wounds
  
  @@inv]]></Description>
		<Description state=""3""><![CDATA[This is the decayed corpse of a &male &race. The process of decay is far enough advanced that it is difficult to identify any individual features any longer.
  #3This corpse is well along the process of decay. The flesh has sloughed off in places and particularly soft parts such as the eyes have been eaten by carrion insects.#0
  
  @@eaten[
  
  ]@@wounds
  
  @@inv]]></Description>
		<Description state=""4""><![CDATA[This is the heavily decayed corpse of &a_an[&race] of indeterminate gender. Very little identifiable features remain.
  #3This corpse in a very advanced state of decay. The flesh is almost entirely gone and the corpse has largely liquified. At this stage, the only parts remaining are what the carrion eaters find tough to digest.#0
  
  @@eaten[
  
  ]@@inv]]></Description>
		<Description state=""5""><![CDATA[This is the skeletal remains of &a_an[&race]. Nothing remains of its other features.
  
  @@eaten[
  
  ]@@inv]]></Description>
	  </FullDescriptions>
	  <ContentsDescriptions>
		<Description state=""0""><![CDATA[@@desc
  #3Death has taken hold of this critter, but it still looks much the same as it did in life.#0
  
  @@eaten[
  
  ]@@wounds
  
  @@inv]]></Description>
		<Description state=""1""><![CDATA[@@desc
  #3Rigor Mortis has set in with this critter, and the blood has begun to pool in the extremities. Flies and other carrion insects have begun to lay eggs in the skin.#0
  
  @@eaten[
  
  ]@@wounds
  
  @@inv]]></Description>
		<Description state=""2""><![CDATA[@@desc
  #3This corpse has begun to bloat and putrefy as decay sets in. Maggots and other carrion insects have firmly taken hold of the corpse.#0
  
  @@eaten[
  
  ]@@wounds
  
  @@inv]]></Description>
		<Description state=""3""><![CDATA[This is the decayed corpse of a &male &race. The process of decay is far enough advanced that it is difficult to identify any individual features any longer.
  #3This corpse is well along the process of decay. The flesh has sloughed off in places and particularly soft parts such as the eyes have been eaten by carrion insects.#0
  
  @@eaten[
  
  ]@@wounds
  
  @@inv]]></Description>
		<Description state=""4""><![CDATA[This is the heavily decayed corpse of &a_an[&race] of indeterminate gender. Very little identifiable features remain.
  #3This corpse in a very advanced state of decay. The flesh is almost entirely gone and the corpse has largely liquified. At this stage, the only parts remaining are what the carrion eaters find tough to digest.#0
  
  @@eaten[
  
  ]@@inv]]></Description>
		<Description state=""5""><![CDATA[This is the skeletal remains of &a_an[&race]. Nothing remains of their other features.
  
  @@eaten[
  
  ]@@inv]]></Description>
	  </ContentsDescriptions>
	</Descriptions>
  </CorpseModel>"
        };
        _context.CorpseModels.Add(_defaultCorpseModel);
        _context.SaveChanges();
    }

    private HealthStrategy SetupHealthModel(FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, string> questionAnswers,
        TraitExpression hpExpression, TraitExpression hpTick, TraitDefinition secondaryTrait)
    {
        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Health Models...");
        HealthStrategy hpStrategy = new()
        {
            Name = NonHumanSeederHealthStrategyHelper.HpStrategyName,
            Type = "BrainHitpoints",
            Definition =
                $"<Definition> <MaximumHitPointsExpression>{hpExpression.Id}</MaximumHitPointsExpression> <HealingTickDamageExpression>{hpTick.Id}</HealingTickDamageExpression><PercentageHealthPerPenalty>0.2</PercentageHealthPerPenalty><LodgeDamageExpression>max(0, damage-15)</LodgeDamageExpression> <SeverityRanges> <Severity value=\"0\" lower=\"-1\" upper=\"0\"/> <Severity value=\"1\" lower=\"0\" upper=\"2\"/> <Severity value=\"2\" lower=\"2\" upper=\"4\"/> <Severity value=\"3\" lower=\"4\" upper=\"7\"/> <Severity value=\"4\" lower=\"7\" upper=\"12\"/> <Severity value=\"5\" lower=\"12\" upper=\"18\"/> <Severity value=\"6\" lower=\"18\" upper=\"27\"/> <Severity value=\"7\" lower=\"27\" upper=\"40\"/> <Severity value=\"8\" lower=\"40\" upper=\"100\"/> </SeverityRanges><CheckHeart>false</CheckHeart> <UseHypoxiaDamage>false</UseHypoxiaDamage><KnockoutOnCritical>true</KnockoutOnCritical><KnockoutDuration>240</KnockoutDuration></Definition>"
        };
        context.HealthStrategies.Add(hpStrategy);

        HealthStrategy hpPlusStrategy = new()
        {
            Name = NonHumanSeederHealthStrategyHelper.HpPlusStrategyName,
            Type = "BrainHitpoints",
            Definition =
                $"<Definition> <MaximumHitPointsExpression>{hpExpression.Id}</MaximumHitPointsExpression> <HealingTickDamageExpression>{hpTick.Id}</HealingTickDamageExpression><PercentageHealthPerPenalty>0.2</PercentageHealthPerPenalty><LodgeDamageExpression>max(0,damage-15)</LodgeDamageExpression> <SeverityRanges> <Severity value=\"0\" lower=\"-1\" upper=\"0\"/> <Severity value=\"1\" lower=\"0\" upper=\"2\"/> <Severity value=\"2\" lower=\"2\" upper=\"4\"/> <Severity value=\"3\" lower=\"4\" upper=\"7\"/> <Severity value=\"4\" lower=\"7\" upper=\"12\"/> <Severity value=\"5\" lower=\"12\" upper=\"18\"/> <Severity value=\"6\" lower=\"18\" upper=\"27\"/> <Severity value=\"7\" lower=\"27\" upper=\"40\"/> <Severity value=\"8\" lower=\"40\" upper=\"100\"/> </SeverityRanges> <CheckHeart>true</CheckHeart> <UseHypoxiaDamage>true</UseHypoxiaDamage><KnockoutOnCritical>true</KnockoutOnCritical><KnockoutDuration>240</KnockoutDuration> </Definition>"
        };
        context.HealthStrategies.Add(hpPlusStrategy);

        TraitExpression stunExpression = new()
        {
            Name = "Non-Human Max Stun Formula",
            Expression = $"100+(con:{_healthTrait.Id}+wil:{secondaryTrait.Id})/2"
        };
        context.TraitExpressions.Add(stunExpression);

        TraitExpression painExpression = new()
        {
            Name = "Non-Human Max Pain Formula",
            Expression = $"100+wil:{secondaryTrait.Id}"
        };
        context.TraitExpressions.Add(painExpression);

        TraitExpression stunTick = new()
        {
            Name = "Non-Human Stun Heal Per Tick",
            Expression = $"100+con:{_healthTrait.Id}"
        };
        context.TraitExpressions.Add(stunTick);

        TraitExpression painTick = new()
        {
            Name = "Non-Human Pain Heal Per Tick",
            Expression = $"100+con:{_healthTrait.Id}"
        };
        context.TraitExpressions.Add(painTick);
        context.SaveChanges();

		HealthStrategy fullStrategy = new()
		{
			Name = NonHumanSeederHealthStrategyHelper.FullStrategyName,
			Type = "ComplexLiving",
			Definition =
				$"<Definition> <MaximumHitPointsExpression>{hpExpression.Id}</MaximumHitPointsExpression> <MaximumStunExpression>{stunExpression.Id}</MaximumStunExpression> <MaximumPainExpression>{painExpression.Id}</MaximumPainExpression> <HealingTickDamageExpression>{hpTick.Id}</HealingTickDamageExpression> <HealingTickStunExpression>{stunTick.Id}</HealingTickStunExpression> <HealingTickPainExpression>{painTick.Id}</HealingTickPainExpression> <LodgeDamageExpression>max(0, damage - 30)</LodgeDamageExpression> <PercentageHealthPerPenalty>0.2</PercentageHealthPerPenalty> <PercentageStunPerPenalty>0.2</PercentageStunPerPenalty> <PercentagePainPerPenalty>0.2</PercentagePainPerPenalty> <SeverityRanges> <Severity value=\"0\" lower=\"-2\" upper=\"-1\" lowerpec=\"-100\" upperperc=\"0\"/> <Severity value=\"1\" lower=\"-1\" upper=\"2\" lowerpec=\"0\" upperperc=\"0.15\"/> <Severity value=\"2\" lower=\"2\" upper=\"4\" lowerpec=\"0.15\" upperperc=\"0.30\"/> <Severity value=\"3\" lower=\"4\" upper=\"7\" lowerpec=\"0.30\" upperperc=\"0.45\"/> <Severity value=\"4\" lower=\"7\" upper=\"12\" lowerpec=\"0.45\" upperperc=\"0.60\"/> <Severity value=\"5\" lower=\"12\" upper=\"18\" lowerpec=\"0.60\" upperperc=\"0.75\"/> <Severity value=\"6\" lower=\"18\" upper=\"27\" lowerpec=\"0.75\" upperperc=\"0.87\"/> <Severity value=\"7\" lower=\"27\" upper=\"40\" lowerpec=\"0.87\" upperperc=\"0.95\"/> <Severity value=\"8\" lower=\"40\" upper=\"100\" lowerpec=\"0.95\" upperperc=\"100\"/> </SeverityRanges> </Definition>"
		};
        context.HealthStrategies.Add(fullStrategy);

        _healthStrategy = questionAnswers["model"].ToLowerInvariant() switch
        {
            "hp" => hpStrategy,
            "hpplus" => hpPlusStrategy,
            "full" => fullStrategy,
            _ => throw new ArgumentOutOfRangeException()
        };

        _context.SaveChanges();
        return _healthStrategy;
    }

    private void SetupHeightWeightModels()
    {
        Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Height Weight Models...");
        foreach (AnimalHeightWeightTemplate template in HeightWeightTemplates.Values)
        {
            AddHWModel(template.Name, template.MeanHeight, template.StandardDeviationHeight, template.MeanBmi,
                template.StandardDeviationBmi);
        }
    }

    private void AddHWModel(string name, double meanHeight, double stddevheight, double meanbmi, double stddevbmi)
    {
        HeightWeightModel hwModel = _context.HeightWeightModels.FirstOrDefault(x => x.Name == name) ?? new HeightWeightModel
        {
            Name = name
        };
        hwModel.MeanHeight = meanHeight;
        hwModel.MeanBmi = meanbmi;
        hwModel.StddevHeight = stddevheight;
        hwModel.StddevBmi = stddevbmi;
        hwModel.Bmimultiplier = 0.1;
        if (hwModel.Id == 0)
        {
            _context.Add(hwModel);
        }

        _hwModels[name] = hwModel;
        _context.SaveChanges();
    }
}
