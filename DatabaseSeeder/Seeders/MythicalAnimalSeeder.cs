#nullable enable

using DatabaseSeeder.Seeders;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseSeeder.Seeders;

public partial class MythicalAnimalSeeder : IDatabaseSeeder
{
    private static readonly string[] HumanoidFullLegRemovalAliases =
    [
        "rhip",
        "rthigh",
        "rthighback",
        "rknee",
        "rkneeback",
        "rshin",
        "rcalf",
        "rankle",
        "rheel",
        "rfoot",
        "rbigtoe",
        "rindextoe",
        "rmiddletoe",
        "rringtoe",
        "rpinkytoe",
        "lhip",
        "lthigh",
        "lthighback",
        "lknee",
        "lkneeback",
        "lshin",
        "lcalf",
        "lankle",
        "lheel",
        "lfoot",
        "lbigtoe",
        "lindextoe",
        "lmiddletoe",
        "lringtoe",
        "lpinkytoe"
    ];

    private FuturemudDatabaseContext _context = null!;
    private Race _humanRace = null!;
    private Race _organicHumanoidRace = null!;
    private BodyProto _organicHumanoidBody = null!;
    private BodyProto _quadrupedBody = null!;
    private BodyProto _ungulateBody = null!;
    private BodyProto _toedQuadrupedBody = null!;
    private BodyProto _avianBody = null!;
    private BodyProto _insectoidBody = null!;
    private BodyProto _arachnidBody = null!;
    private BodyProto _beetleBody = null!;
    private BodyProto _centipedeBody = null!;
    private BodyProto _vermiformBody = null!;
    private BodyProto _serpentineBody = null!;
    private BodyProto _piscineBody = null!;
    private BodyProto _scorpionBody = null!;
    private HealthStrategy _healthStrategy = null!;
    private FutureProg _alwaysTrue = null!;
    private FutureProg _alwaysFalse = null!;
    private FutureProg _alwaysZero = null!;
    private TraitDefinition _healthTrait = null!;
    private TraitDefinition _strengthTrait = null!;
    private CorpseModel _humanoidCorpse = null!;
    private CorpseModel _animalCorpse = null!;
    private Liquid _blood = null!;
    private Liquid _saltWater = null!;
    private Liquid _brackishWater = null!;
    private Liquid? _sweat;
    private Gas _breathableAir = null!;
    private PopulationBloodModel? _defaultPopulationBloodModel;
    private CharacteristicDefinition _personWordDefinition = null!;
    private ArmourType? _humanoidNaturalArmour;
    private ArmourType? _animalNaturalArmour;
    private long _nextBodyProtoId;
    private static readonly string[] WingAliases =
    [
        "rwingbase",
        "lwingbase",
        "rwing",
        "lwing"
    ];

    public IEnumerable<(string Id, string Question,
        Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
        Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions
        => NonHumanSeederQuestions.GetQuestions();

    public int SortOrder => 302;
    public string Name => "Mythical Animal Seeder";
    public string Tagline => "Installs a catalogue of mythic beasts and hybrid folk";
    public string FullDescription =>
        @"Seeds a dedicated catalogue of mythical creatures and hybrid folk, including draconic, avian, serpentine, merfolk and centaur variants. This package depends on both the Human and Animal seeders so it can reuse their anatomy, combat, corpse and characteristic infrastructure.";
    public bool SafeToRunMoreThanOnce => true;

    public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
    {
        _context = context;
        _context.Database.BeginTransaction();
        IReadOnlyDictionary<string, string> effectiveAnswers =
            CombatBalanceProfileHelper.MergeQuestionAnswersWithRecordedChoice(context, questionAnswers);
        LoadSharedSeederData(effectiveAnswers);
        bool hasMissingDisfigurementTemplates = HasMissingMythicalDisfigurementTemplates(_context);
        List<MythicalRaceTemplate> templatesToSeed = Templates.Values
            .Where(template => !_context.Races.Any(x => x.Name == template.Name))
            .ToList();
        Dictionary<string, BodyProto> bodyLookup = BuildBodyCatalogue(Templates.Values);
        RefreshExistingMythicalRaceDefaults();
        if (templatesToSeed.Count == 0 && !hasMissingDisfigurementTemplates)
        {
            RefreshExistingMythicalCombatBalance();
            _context.Database.CommitTransaction();
            return "Mythical races are already installed and their breathing, mobility, and combat balance profiles have been refreshed.";
        }

        foreach (MythicalRaceTemplate template in templatesToSeed)
        {
            SeedRace(template, bodyLookup[template.BodyKey]);
        }

        RefreshExistingMythicalRaceDefaults();
        SeedMythicalDisfigurementTemplates(bodyLookup);
        RefreshExistingMythicalCombatBalance();
        _context.SaveChanges();
        _context.Database.CommitTransaction();
        int skippedCount = Templates.Count - templatesToSeed.Count;
        if (templatesToSeed.Count == 0)
        {
            return "Installed additional mythical disfigurement templates.";
        }

        if (hasMissingDisfigurementTemplates)
        {
            return skippedCount > 0
                ? $"Successfully installed {templatesToSeed.Count} mythical races, updated disfigurement templates, and skipped {skippedCount} that already existed."
                : $"Successfully installed {templatesToSeed.Count} mythical races and updated disfigurement templates.";
        }

        return skippedCount > 0
            ? $"Successfully installed {templatesToSeed.Count} mythical races and skipped {skippedCount} that already existed."
            : "Successfully installed mythical races.";
    }

    public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
    {
        if (!HasPrerequisites(context))
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        if (Templates.Keys.All(name => context.Races.Any(x => x.Name == name)))
        {
            return HasMissingMythicalDisfigurementTemplates(context) || HasMythicalSatiationLimitUpdates(context)
                ? ShouldSeedResult.ExtraPackagesAvailable
                : ShouldSeedResult.MayAlreadyBeInstalled;
        }

        return ShouldSeedResult.ReadyToInstall;
    }

	private static bool HasMythicalSatiationLimitUpdates(FuturemudDatabaseContext context)
	{
		return Templates.Values.Any(template =>
			context.Races.FirstOrDefault(x => x.Name == template.Name) is { } race &&
			!SatiationLimitSeederHelper.MatchesLimits(
				race,
				template.MaximumFoodSatiatedHours,
				template.MaximumDrinkSatiatedHours));
	}

    private static bool HasPrerequisites(FuturemudDatabaseContext context)
    {
        string[] requiredBodies = new[]
        {
            "Organic Humanoid",
            "Quadruped Base",
            "Ungulate",
            "Toed Quadruped",
            "Avian",
            "Insectoid",
            "Arachnid",
            "Beetle",
            "Centipede",
            "Vermiform",
            "Serpentine",
            "Piscine",
            "Scorpion"
        };
        string[] requiredRaces = new[]
        {
            "Human",
            "Organic Humanoid"
        };
        string[] requiredProfiles = new[]
        {
            "All Eye Colours",
            "All Eye Shapes",
            "All Noses",
            "All Ears",
            "All Hair Colours",
            "All Facial Hair Colours",
            "All Hair Styles",
            "All Skin Colours",
            "All Frames",
            "Person Word"
        };
        HashSet<string> requiredHeightWeightModels = Templates.Values
            .SelectMany(x => new[] { x.MaleHeightWeightModel, x.FemaleHeightWeightModel })
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return requiredBodies.All(body => context.BodyProtos.Any(x => x.Name == body)) &&
               requiredRaces.All(race => context.Races.Any(x => x.Name == race)) &&
               requiredProfiles.All(profile =>
                   context.CharacteristicProfiles.Any(x => x.Name == profile) ||
                   context.CharacteristicDefinitions.Any(x => x.Name == profile)) &&
               requiredHeightWeightModels.All(model => context.HeightWeightModels.Any(x => x.Name == model)) &&
               context.WeaponAttacks.Any(x => x.Name == "Acid Spit") &&
               context.CorpseModels.Any(x => x.Name == "Organic Human Corpse") &&
               context.CorpseModels.Any(x => x.Name == "Organic Animal Corpse") &&
               NonHumanSeederHealthStrategyHelper.AllStrategyNames.All(name =>
                   context.HealthStrategies.Any(x => x.Name == name));
    }

    private void LoadSharedSeederData(IReadOnlyDictionary<string, string> answers)
    {
        _combatBalanceProfile = CombatBalanceProfileHelper.GetSelectedProfile(_context, answers);
        _humanRace = _context.Races.First(x => x.Name == "Human");
        _organicHumanoidRace = _context.Races.First(x => x.Name == "Organic Humanoid");
        _organicHumanoidBody = _context.BodyProtos.First(x => x.Name == "Organic Humanoid");
        _quadrupedBody = _context.BodyProtos.First(x => x.Name == "Quadruped Base");
        _ungulateBody = _context.BodyProtos.First(x => x.Name == "Ungulate");
        _toedQuadrupedBody = _context.BodyProtos.First(x => x.Name == "Toed Quadruped");
        _avianBody = _context.BodyProtos.First(x => x.Name == "Avian");
        _insectoidBody = _context.BodyProtos.First(x => x.Name == "Insectoid");
        _arachnidBody = _context.BodyProtos.First(x => x.Name == "Arachnid");
        _beetleBody = _context.BodyProtos.First(x => x.Name == "Beetle");
        _centipedeBody = _context.BodyProtos.First(x => x.Name == "Centipede");
        _vermiformBody = _context.BodyProtos.First(x => x.Name == "Vermiform");
        _serpentineBody = _context.BodyProtos.First(x => x.Name == "Serpentine");
        _piscineBody = _context.BodyProtos.First(x => x.Name == "Piscine");
        _scorpionBody = _context.BodyProtos.First(x => x.Name == "Scorpion");
        _alwaysTrue = _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
        _alwaysFalse = _context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse");
        _alwaysZero = _context.FutureProgs.First(x => x.FunctionName == "AlwaysZero");
        _humanoidCorpse = _context.CorpseModels.First(x => x.Name == "Organic Human Corpse");
        _animalCorpse = _context.CorpseModels.First(x => x.Name == "Organic Animal Corpse");
        _blood = _context.Liquids.First(x => x.Name == "blood");
        _saltWater = _context.Liquids.First(x => x.Name == "salt water");
        _brackishWater = _context.Liquids.First(x => x.Name == "brackish water");
        _sweat = _context.Liquids.FirstOrDefault(x => x.Name == "sweat");
        _breathableAir = _context.Gases.AsEnumerable().First(x => x.Name.Contains("Breathable Atmosphere", StringComparison.OrdinalIgnoreCase));
        _defaultPopulationBloodModel = _humanRace.Ethnicities.FirstOrDefault()?.PopulationBloodModel ??
                                       _context.PopulationBloodModels.FirstOrDefault();
        _personWordDefinition = _context.CharacteristicDefinitions.First(x => x.Name == "Person Word");
		_humanoidNaturalArmour = _context.ArmourTypes.FirstOrDefault(x => x.Name == "Human Racial Tissue Armour");
		_animalNaturalArmour = _context.ArmourTypes.FirstOrDefault(x => x.Name == "Non-Human Natural Armour");
        _nextBodyProtoId = _context.BodyProtos.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;

        _healthTrait = _context.TraitDefinitions
            .Where(x => x.Type == (int)TraitType.Attribute)
            .AsEnumerable()
            .First(x => x.Name.In("Constitution", "Body", "Physique", "Endurance", "Hardiness", "Stamina"));
        _strengthTrait = _context.TraitDefinitions
            .Where(x => x.Type == (int)TraitType.Attribute)
            .AsEnumerable()
            .First(x => x.Name.In("Strength", "Physique", "Body", "Upper Body Strength"));

        _healthStrategy = _context.HealthStrategies.First(x =>
            x.Name == NonHumanSeederHealthStrategyHelper.GetStrategyName(answers["model"]));
    }

    private Dictionary<string, BodyProto> BuildBodyCatalogue(IEnumerable<MythicalRaceTemplate> templates)
    {
        HashSet<string> requiredBodyKeys = templates
            .Select(x => x.BodyKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, BodyProto> bodies = new(StringComparer.OrdinalIgnoreCase);
        void AddBaseBody(string key, BodyProto body)
        {
            if (requiredBodyKeys.Contains(key))
            {
                bodies[key] = body;
            }
        }

        AddBaseBody("Organic Humanoid", _organicHumanoidBody);
        AddBaseBody("Toed Quadruped", _toedQuadrupedBody);
        AddBaseBody("Ungulate", _ungulateBody);
        AddBaseBody("Avian", _avianBody);
        AddBaseBody("Insectoid", _insectoidBody);
        AddBaseBody("Arachnid", _arachnidBody);
        AddBaseBody("Beetle", _beetleBody);
        AddBaseBody("Centipede", _centipedeBody);
        AddBaseBody("Vermiform", _vermiformBody);
        AddBaseBody("Serpentine", _serpentineBody);
        AddBaseBody("Scorpion", _scorpionBody);

        if (requiredBodyKeys.Contains("Horned Humanoid"))
        {
            bodies["Horned Humanoid"] = GetOrCreateBody("Horned Humanoid", CreateHornedHumanoidBody);
        }

        if (requiredBodyKeys.Contains("Winged Humanoid"))
        {
            bodies["Winged Humanoid"] = GetOrCreateBody("Winged Humanoid", CreateWingedHumanoidBody);
        }

        if (requiredBodyKeys.Contains("Naga"))
        {
            bodies["Naga"] = GetOrCreateBody("Naga", CreateNagaBody);
        }

        if (requiredBodyKeys.Contains("Eastern Dragon"))
        {
            bodies["Eastern Dragon"] = GetOrCreateBody("Eastern Dragon", CreateEasternDragonBody);
        }

        if (requiredBodyKeys.Contains("Mermaid"))
        {
            bodies["Mermaid"] = GetOrCreateBody("Mermaid", CreateMermaidBody);
        }

        if (requiredBodyKeys.Contains("Centaur"))
        {
            bodies["Centaur"] = GetOrCreateBody("Centaur", CreateCentaurBody);
        }

        if (requiredBodyKeys.Contains("Griffin"))
        {
            bodies["Griffin"] = GetOrCreateBody("Griffin", CreateGriffinBody);
        }

        if (requiredBodyKeys.Contains("Hippogriff"))
        {
            bodies["Hippogriff"] = GetOrCreateBody("Hippogriff", CreateHippogriffBody);
        }

        if (requiredBodyKeys.Contains("Manticore"))
        {
            bodies["Manticore"] = GetOrCreateBody("Manticore", CreateManticoreBody);
        }

        if (requiredBodyKeys.Contains("Wyvern"))
        {
            bodies["Wyvern"] = GetOrCreateBody("Wyvern", CreateWyvernBody);
        }

        if (requiredBodyKeys.Contains("Hippocamp"))
        {
            bodies["Hippocamp"] = GetOrCreateBody("Hippocamp", CreateHippocampBody);
        }

        return bodies;
    }

    private BodyProto GetOrCreateBody(string name, Func<BodyProto> factory)
    {
        BodyProto body = _context.BodyProtos.FirstOrDefault(x => x.Name == name) ?? factory();
        RefreshCustomBodyCapabilities(body);
        RepairAndValidateCustomBody(body);
        return body;
    }

    private BodyProto CreateHornedHumanoidBody()
    {
        BodyProto body = CreateInheritedBodyShell("Horned Humanoid", _organicHumanoidBody);
        PopulateHornedHumanoidBody(body);
        return body;
    }

    private BodyProto CreateWingedHumanoidBody()
    {
        BodyProto body = CreateInheritedBodyShell("Winged Humanoid", _organicHumanoidBody, minimumWingsToFly: 2);
        PopulateWingedHumanoidBody(body);
        return body;
    }

    private void PopulateHornedHumanoidBody(BodyProto body)
    {
        ConfigureBodyShell(body, _organicHumanoidBody,
            _organicHumanoidBody.MinimumLegsToStand,
            _organicHumanoidBody.MinimumWingsToFly,
            _organicHumanoidBody.WielderDescriptionSingle,
            _organicHumanoidBody.WielderDescriptionPlural);
        AddHumanoidHorns(body);
    }

    private void PopulateWingedHumanoidBody(BodyProto body)
    {
        ConfigureBodyShell(body, _organicHumanoidBody,
            _organicHumanoidBody.MinimumLegsToStand,
            2,
            _organicHumanoidBody.WielderDescriptionSingle,
            _organicHumanoidBody.WielderDescriptionPlural);
        AddAvianWings(body);
        AddFlyMovement(body);
    }

    private BodyProto CreateNagaBody()
    {
        BodyProto body = CreateInheritedBodyShell("Naga", _organicHumanoidBody, minimumLegsToStand: 0);
        PopulateNagaBody(body);
        return body;
    }

    private void PopulateNagaBody(BodyProto body)
    {
        ConfigureBodyShell(body, _organicHumanoidBody, 0, _organicHumanoidBody.MinimumWingsToFly,
            _organicHumanoidBody.WielderDescriptionSingle, _organicHumanoidBody.WielderDescriptionPlural);
        AddBodypartRemoval(body, HumanoidFullLegRemovalAliases);
        SeederBodyUtilities.CloneBodypartSubtree(_context, _vermiformBody, body, "ubody", "groin", cloneLimbs: false);
        AddLimb(body, "Tail", LimbType.Appendage, "ubody", "ubody", "mbody", "lbody", "tail");
        AddSwimMovement(body);
    }

    private BodyProto CreateEasternDragonBody()
    {
        BodyProto body = CreateQuadrupedBody("Eastern Dragon");
        PopulateEasternDragonBody(body);
        return body;
    }

    private void PopulateEasternDragonBody(BodyProto body)
    {
        PopulateQuadrupedBody(body);
        AddToedFeet(body);
        SeederBodyUtilities.RemoveBodyparts(_context, body, ["rhorn", "lhorn", "rwingbase", "lwingbase", "rwing", "lwing"]);
    }

    private BodyProto CreateMermaidBody()
    {
        BodyProto body = CreateInheritedBodyShell("Mermaid", _organicHumanoidBody, minimumLegsToStand: 0);
        PopulateMermaidBody(body);
        return body;
    }

    private void PopulateMermaidBody(BodyProto body)
    {
        ConfigureBodyShell(body, _organicHumanoidBody, 0, _organicHumanoidBody.MinimumWingsToFly,
            _organicHumanoidBody.WielderDescriptionSingle, _organicHumanoidBody.WielderDescriptionPlural);
        AddBodypartRemoval(body, HumanoidFullLegRemovalAliases);
        SeederBodyUtilities.CloneBodypartSubtree(_context, _piscineBody, body, "peduncle", "groin");
        AddSwimMovement(body);
    }

    private BodyProto CreateCentaurBody()
    {
        BodyProto body = CreateInheritedBodyShell("Centaur", _organicHumanoidBody, minimumLegsToStand: 4);
        PopulateCentaurBody(body);
        return body;
    }

    private void PopulateCentaurBody(BodyProto body)
    {
        ConfigureBodyShell(body, _organicHumanoidBody, 4, _organicHumanoidBody.MinimumWingsToFly,
            _organicHumanoidBody.WielderDescriptionSingle, _organicHumanoidBody.WielderDescriptionPlural);
        AddBodypartRemoval(body, HumanoidFullLegRemovalAliases);
        SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "ruforeleg", "abdomen");
        SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "luforeleg", "abdomen");
        SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "ruhindleg", "groin");
        SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "luhindleg", "groin");
        SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "utail", "lback");
        AddHooves(body, includeFront: true, includeRear: true);
    }

    private BodyProto CreateGriffinBody()
    {
        BodyProto body = CreateQuadrupedBody("Griffin");
        PopulateGriffinBody(body);
        return body;
    }

    private void PopulateGriffinBody(BodyProto body)
    {
        PopulateQuadrupedBody(body);
        AddToedFeet(body);
        SeederBodyUtilities.RemoveBodyparts(_context, body, ["neck"]);
        SeederBodyUtilities.CloneBodypartSubtree(_context, _avianBody, body, "neck", "uback");
        SeederBodyUtilities.RemoveBodyparts(_context, body,
            ["rjaw", "ljaw", "muzzle", "rhorn", "lhorn", "horn", "rantler", "lantler", "rtusk", "ltusk"]);
        EnsureLimb(body, "Head", LimbType.Head, "neck",
            "neck", "bneck", "throat", "head", "bhead", "rcheek", "lcheek", "reyesocket", "leyesocket",
            "reye", "leye", "rear", "lear", "beak", "tongue", "nose");
    }

    private BodyProto CreateHippogriffBody()
    {
        BodyProto body = CreateQuadrupedBody("Hippogriff");
        PopulateHippogriffBody(body);
        return body;
    }

    private void PopulateHippogriffBody(BodyProto body)
    {
        PopulateQuadrupedBody(body);
        AddHooves(body, includeFront: true, includeRear: true);
        SeederBodyUtilities.RemoveBodyparts(_context, body, ["neck"]);
        SeederBodyUtilities.CloneBodypartSubtree(_context, _avianBody, body, "neck", "uback");
        SeederBodyUtilities.RemoveBodyparts(_context, body,
            ["rjaw", "ljaw", "muzzle", "rhorn", "lhorn", "horn", "rantler", "lantler", "rtusk", "ltusk"]);
        EnsureLimb(body, "Head", LimbType.Head, "neck",
            "neck", "bneck", "throat", "head", "bhead", "rcheek", "lcheek", "reyesocket", "leyesocket",
            "reye", "leye", "rear", "lear", "beak", "tongue", "nose");
    }

    private BodyProto CreateManticoreBody()
    {
        BodyProto body = CreateQuadrupedBody("Manticore");
        PopulateManticoreBody(body);
        return body;
    }

    private void PopulateManticoreBody(BodyProto body)
    {
        PopulateQuadrupedBody(body);
        AddToedFeet(body);
        SeederBodyUtilities.CloneBodypartSubtree(_context, _scorpionBody, body, "stinger", "ltail", cloneLimbs: false);
        AddBodypartsToLimb(body, "Tail", "stinger");
    }

    private BodyProto CreateWyvernBody()
    {
        BodyProto body = CreateFullCloneBody("Wyvern", _avianBody);
        PopulateWyvernBody(body);
        return body;
    }

    private void PopulateWyvernBody(BodyProto body)
    {
        PopulateFullCloneBody(body, _avianBody);
        SeederBodyUtilities.RemoveBodyparts(_context, body, ["neck"]);
        SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "neck", "uback");
        EnsureLimb(body, "Head", LimbType.Head, "neck",
            "neck", "bneck", "throat", "head", "bhead", "rjaw", "ljaw", "rcheek", "lcheek", "reyesocket",
            "leyesocket", "reye", "leye", "rear", "lear", "muzzle", "mouth", "tongue", "nose", "rhorn",
            "lhorn", "horn", "rantler", "lantler", "rtusk", "ltusk");
    }

    private BodyProto CreateHippocampBody()
    {
        BodyProto body = CreateQuadrupedBody("Hippocamp", minimumLegsToStand: 2);
        PopulateHippocampBody(body);
        return body;
    }

    private void PopulateHippocampBody(BodyProto body)
    {
        PopulateQuadrupedBody(body, minimumLegsToStand: 2);
        AddHooves(body, includeFront: true, includeRear: false);
        SeederBodyUtilities.RemoveBodyparts(_context, body, ["ruhindleg", "luhindleg"]);
        SeederBodyUtilities.CloneBodypartSubtree(_context, _piscineBody, body, "peduncle", "lback");
        AddSwimMovement(body);
    }

    private BodyProto CreateQuadrupedBody(string name, int? minimumLegsToStand = null)
    {
        BodyProto body = CreateBodyShell(name, _quadrupedBody,
            minimumLegsToStand ?? _quadrupedBody.MinimumLegsToStand,
            _quadrupedBody.MinimumWingsToFly,
            _quadrupedBody.WielderDescriptionSingle,
            _quadrupedBody.WielderDescriptionPlural);
        return body;
    }

    private void PopulateQuadrupedBody(BodyProto body, int? minimumLegsToStand = null)
    {
        ConfigureBodyShell(body, _quadrupedBody,
            minimumLegsToStand ?? _quadrupedBody.MinimumLegsToStand,
            _quadrupedBody.MinimumWingsToFly,
            _quadrupedBody.WielderDescriptionSingle,
            _quadrupedBody.WielderDescriptionPlural);
        SeederBodyUtilities.ClearBodyDefinition(_context, body);
        SeederBodyUtilities.CloneBodyDefinition(_context, _quadrupedBody, body);
        SeederBodyUtilities.CloneBodyPositionsAndSpeeds(_context, _quadrupedBody, body);
    }

    private BodyProto CreateFullCloneBody(string name, BodyProto source, int? minimumLegsToStand = null,
        int? minimumWingsToFly = null)
    {
        BodyProto body = CreateBodyShell(name, source,
            minimumLegsToStand ?? source.MinimumLegsToStand,
            minimumWingsToFly ?? source.MinimumWingsToFly,
            source.WielderDescriptionSingle,
            source.WielderDescriptionPlural);
        return body;
    }

    private BodyProto CreateBodyShell(string name, BodyProto source, int minimumLegsToStand, int minimumWingsToFly,
        string wielderSingle, string wielderPlural)
    {
        BodyProto body = new() { Id = _nextBodyProtoId++, Name = name };
        ConfigureBodyShell(body, source, minimumLegsToStand, minimumWingsToFly, wielderSingle, wielderPlural);
        _context.BodyProtos.Add(body);
        _context.SaveChanges();
        return body;
    }

    private BodyProto CreateInheritedBodyShell(string name, BodyProto source, int? minimumLegsToStand = null,
        int? minimumWingsToFly = null)
    {
        BodyProto body = new()
        {
            Id = _nextBodyProtoId++,
            Name = name,
            CountsAs = source
        };
        ConfigureBodyShell(body, source,
            minimumLegsToStand ?? source.MinimumLegsToStand,
            minimumWingsToFly ?? source.MinimumWingsToFly,
            source.WielderDescriptionSingle,
            source.WielderDescriptionPlural);
        _context.BodyProtos.Add(body);
        _context.SaveChanges();
        SeederBodyUtilities.CloneBodyPositionsAndSpeeds(_context, source, body);
        return body;
    }

    private void ConfigureBodyShell(BodyProto body, BodyProto source, int minimumLegsToStand, int minimumWingsToFly,
        string wielderSingle, string wielderPlural)
    {
        body.ConsiderString = source.ConsiderString;
        body.WielderDescriptionSingle = wielderSingle;
        body.WielderDescriptionPlural = wielderPlural;
        body.StaminaRecoveryProgId = source.StaminaRecoveryProgId;
        body.MinimumLegsToStand = minimumLegsToStand;
        body.MinimumWingsToFly = minimumWingsToFly;
        body.LegDescriptionPlural = source.LegDescriptionPlural;
        body.LegDescriptionSingular = source.LegDescriptionSingular;
        body.WearSizeParameter = source.WearSizeParameter;
    }

    private void PopulateFullCloneBody(BodyProto body, BodyProto source, int? minimumLegsToStand = null,
        int? minimumWingsToFly = null)
    {
        ConfigureBodyShell(body, source,
            minimumLegsToStand ?? source.MinimumLegsToStand,
            minimumWingsToFly ?? source.MinimumWingsToFly,
            source.WielderDescriptionSingle,
            source.WielderDescriptionPlural);
        SeederBodyUtilities.ClearBodyDefinition(_context, body);
        SeederBodyUtilities.CloneFlattenedBodyDefinition(_context, source, body);
        SeederBodyUtilities.CloneFlattenedBodyPositionsAndSpeeds(_context, source, body);
    }

    private void AddAvianWings(BodyProto body)
    {
        SeederBodyUtilities.CloneBodypartSubtree(_context, _avianBody, body, "rwingbase", "uback");
        SeederBodyUtilities.CloneBodypartSubtree(_context, _avianBody, body, "lwingbase", "uback");
    }

    private void AddHumanoidHorns(BodyProto body)
    {
        SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "rhorn", "head", cloneLimbs: false);
        SeederBodyUtilities.CloneBodypartSubtree(_context, _quadrupedBody, body, "lhorn", "head", cloneLimbs: false);
        AddBodypartsToLimb(body, "Head", "rhorn", "lhorn");
    }

    private void AddToedFeet(BodyProto body)
    {
        SeederBodyUtilities.CloneBodypartSubtree(_context, _toedQuadrupedBody, body, "rfpaw", "rfhock", cloneLimbs: false);
        SeederBodyUtilities.CloneBodypartSubtree(_context, _toedQuadrupedBody, body, "lfpaw", "lfhock", cloneLimbs: false);
        SeederBodyUtilities.CloneBodypartSubtree(_context, _toedQuadrupedBody, body, "rrpaw", "rrhock", cloneLimbs: false);
        SeederBodyUtilities.CloneBodypartSubtree(_context, _toedQuadrupedBody, body, "lrpaw", "lrhock", cloneLimbs: false);
        AddBodypartsToLimb(body, "Right Foreleg", "rfpaw", "rfclaw");
        AddBodypartsToLimb(body, "Left Foreleg", "lfpaw", "lfclaw");
        AddBodypartsToLimb(body, "Right Hindleg", "rrpaw", "rrclaw", "rrdewclaw");
        AddBodypartsToLimb(body, "Left Hindleg", "lrpaw", "lrclaw", "lrdewclaw");
    }

    private void AddHooves(BodyProto body, bool includeFront, bool includeRear)
    {
        if (includeFront)
        {
            SeederBodyUtilities.CloneBodypartSubtree(_context, _ungulateBody, body, "rfhoof", "rfhock", cloneLimbs: false);
            SeederBodyUtilities.CloneBodypartSubtree(_context, _ungulateBody, body, "lfhoof", "lfhock", cloneLimbs: false);
            AddBodypartsToLimb(body, "Right Foreleg", "rfhoof", "rffrog");
            AddBodypartsToLimb(body, "Left Foreleg", "lfhoof", "lffrog");
        }

        if (includeRear)
        {
            SeederBodyUtilities.CloneBodypartSubtree(_context, _ungulateBody, body, "rrhoof", "rrhock", cloneLimbs: false);
            SeederBodyUtilities.CloneBodypartSubtree(_context, _ungulateBody, body, "lrhoof", "lrhock", cloneLimbs: false);
            AddBodypartsToLimb(body, "Right Hindleg", "rrhoof", "rrfrog");
            AddBodypartsToLimb(body, "Left Hindleg", "lrhoof", "lrfrog");
        }
    }

    private void AddBodypartsToLimb(BodyProto body, string limbName, params string[] aliases)
    {
        Limb? limb = FindLimbOnBody(body, limbName);
        if (limb is null)
        {
            return;
        }

        HashSet<long> existing = _context.LimbsBodypartProto
            .Where(x => x.LimbId == limb.Id)
            .Select(x => x.BodypartProtoId)
            .ToHashSet();
        List<long> bodyIds = SeederBodyUtilities.GetBodyAndAncestorIds(_context, body);
        IReadOnlyDictionary<string, IReadOnlyList<BodypartProto>> lookup = SeederBodyUtilities.BuildBodypartAliasLookup(_context.BodypartProtos
            .Where(x => bodyIds.Contains(x.BodyId) && aliases.Contains(x.Name))
            .ToList());

        foreach (string alias in aliases)
        {
            if (!lookup.TryGetValue(alias, out IReadOnlyList<BodypartProto>? matchingParts))
            {
                continue;
            }

            foreach (BodypartProto? part in matchingParts.Where(x => !existing.Contains(x.Id)))
            {
                _context.LimbsBodypartProto.Add(new LimbBodypartProto
                {
                    Limb = limb,
                    BodypartProto = part
                });
                existing.Add(part.Id);
            }
        }

        _context.SaveChanges();
    }

    private int RepairAndValidateCustomBody(BodyProto body)
    {
        switch (body.Name)
        {
            case "Horned Humanoid":
                AddBodypartsToLimb(body, "Head", "rhorn", "lhorn");
                break;
            case "Winged Humanoid":
                AddBodypartsToLimb(body, "Right Wing", "rwingbase", "rwing");
                AddBodypartsToLimb(body, "Left Wing", "lwingbase", "lwing");
                break;
            case "Naga":
                AddBodypartsToLimb(body, "Tail", "ubody", "mbody", "lbody", "tail");
                break;
            case "Mermaid":
                AddBodypartsToLimb(body, "Tail", "peduncle", "caudalfin");
                break;
            case "Centaur":
                AddBodypartsToLimb(body, "Right Foreleg", "rfhoof", "rffrog");
                AddBodypartsToLimb(body, "Left Foreleg", "lfhoof", "lffrog");
                AddBodypartsToLimb(body, "Right Hindleg", "rrhoof", "rrfrog");
                AddBodypartsToLimb(body, "Left Hindleg", "lrhoof", "lrfrog");
                AddBodypartsToLimb(body, "Tail", "utail", "mtail", "ltail");
                break;
            case "Eastern Dragon":
            case "Manticore":
                AddBodypartsToLimb(body, "Right Foreleg", "rfpaw", "rfclaw");
                AddBodypartsToLimb(body, "Left Foreleg", "lfpaw", "lfclaw");
                AddBodypartsToLimb(body, "Right Hindleg", "rrpaw", "rrclaw", "rrdewclaw");
                AddBodypartsToLimb(body, "Left Hindleg", "lrpaw", "lrclaw", "lrdewclaw");
                break;
            case "Griffin":
                EnsureLimb(body, "Head", LimbType.Head, "neck",
                    "neck", "bneck", "throat", "head", "bhead", "rcheek", "lcheek", "reyesocket", "leyesocket",
                    "reye", "leye", "rear", "lear", "beak", "tongue", "nose", "mouth");
                AddBodypartsToLimb(body, "Right Foreleg", "rfpaw", "rfclaw");
                AddBodypartsToLimb(body, "Left Foreleg", "lfpaw", "lfclaw");
                AddBodypartsToLimb(body, "Right Hindleg", "rrpaw", "rrclaw", "rrdewclaw");
                AddBodypartsToLimb(body, "Left Hindleg", "lrpaw", "lrclaw", "lrdewclaw");
                break;
            case "Hippogriff":
                EnsureLimb(body, "Head", LimbType.Head, "neck",
                    "neck", "bneck", "throat", "head", "bhead", "rcheek", "lcheek", "reyesocket", "leyesocket",
                    "reye", "leye", "rear", "lear", "beak", "tongue", "nose", "mouth");
                AddBodypartsToLimb(body, "Right Foreleg", "rfhoof", "rffrog");
                AddBodypartsToLimb(body, "Left Foreleg", "lfhoof", "lffrog");
                AddBodypartsToLimb(body, "Right Hindleg", "rrhoof", "rrfrog");
                AddBodypartsToLimb(body, "Left Hindleg", "lrhoof", "lrfrog");
                break;
            case "Wyvern":
                EnsureLimb(body, "Head", LimbType.Head, "neck",
                    "neck", "bneck", "throat", "head", "bhead", "rjaw", "ljaw", "rcheek", "lcheek", "reyesocket",
                    "leyesocket", "reye", "leye", "rear", "lear", "muzzle", "mouth", "tongue", "nose", "rhorn",
                    "lhorn", "horn", "rantler", "lantler", "rtusk", "ltusk");
                AddBodypartsToLimb(body, "Right Wing", "rwingbase", "rwing");
                AddBodypartsToLimb(body, "Left Wing", "lwingbase", "lwing");
                break;
            case "Hippocamp":
                AddBodypartsToLimb(body, "Right Foreleg", "rfhoof", "rffrog");
                AddBodypartsToLimb(body, "Left Foreleg", "lfhoof", "lffrog");
                AddBodypartsToLimb(body, "Tail", "peduncle", "caudalfin");
                break;
        }

        AddBodypartsToLimb(body, "Tail", "stinger");

        IReadOnlyList<BodypartProto> uncoveredParts = SeederBodyUtilities.GetExternalBodypartsWithoutLimbCoverage(_context, body);
        if (uncoveredParts.Any())
        {
            throw new InvalidOperationException(
                $"Mythical body {body.Name} has external bodyparts without limb coverage: {string.Join(", ", uncoveredParts.Select(x => x.Name))}");
        }

        return 0;
    }

    private void EnsureLimb(BodyProto body, string limbName, LimbType limbType, string rootAlias, params string[] aliases)
    {
        if (FindLimbOnBody(body, limbName) is not null)
        {
            AddBodypartsToLimb(body, limbName, aliases);
            return;
        }

        AddLimb(body, limbName, limbType, rootAlias, aliases);
    }

    private void AddLimb(BodyProto body, string name, LimbType type, string rootAlias, params string[] aliases)
    {
        List<long> bodyIds = SeederBodyUtilities.GetBodyAndAncestorIds(_context, body);
        IReadOnlyDictionary<string, IReadOnlyList<BodypartProto>> bodyparts = SeederBodyUtilities.BuildBodypartAliasLookup(_context.BodypartProtos
            .Where(x => bodyIds.Contains(x.BodyId) && aliases.Contains(x.Name))
            .ToList());
        if (!bodyparts.TryGetValue(rootAlias, out IReadOnlyList<BodypartProto>? rootParts))
        {
            return;
        }
        BodypartProto root = rootParts[0];

        Limb limb = new()
        {
            Name = name,
            LimbType = (int)type,
            RootBody = body,
            RootBodypart = root,
            LimbDamageThresholdMultiplier = 0.5,
            LimbPainThresholdMultiplier = 0.5
        };
        _context.Limbs.Add(limb);
        _context.SaveChanges();
        HashSet<long> addedPartIds = new();

        foreach (string alias in aliases)
        {
            if (!bodyparts.TryGetValue(alias, out IReadOnlyList<BodypartProto>? matchingParts))
            {
                continue;
            }

            foreach (BodypartProto? part in matchingParts.Where(x => addedPartIds.Add(x.Id)))
            {
                _context.LimbsBodypartProto.Add(new LimbBodypartProto
                {
                    Limb = limb,
                    BodypartProto = part
                });
            }
        }

        _context.SaveChanges();
    }

    private Limb? FindLimbOnBody(BodyProto body, string limbName)
    {
        List<long> bodyIds = SeederBodyUtilities.GetBodyAndAncestorIds(_context, body);
        return _context.Limbs
            .Where(x => bodyIds.Contains(x.RootBodyId) && x.Name == limbName)
            .AsEnumerable()
            .OrderBy(x => bodyIds.IndexOf(x.RootBodyId))
            .ThenBy(x => x.Id)
            .FirstOrDefault();
    }

    private enum MythicalBreathingProfile
    {
        SimpleAir,
        PartlessAir,
        MarineAmphibious
    }

    private static MythicalBreathingProfile GetBreathingProfile(MythicalRaceTemplate template)
    {
        if (template.Name.EqualToAny("Mermaid", "Hippocamp"))
        {
            return MythicalBreathingProfile.MarineAmphibious;
        }

        if (template.Name.EqualToAny("Myconid", "Plantfolk", "Ent", "Dryad") ||
            template.BodyKey.EqualToAny("Insectoid", "Arachnid", "Beetle", "Centipede", "Scorpion"))
        {
            return MythicalBreathingProfile.PartlessAir;
        }

        return MythicalBreathingProfile.SimpleAir;
    }

    internal static string GetBreathingProfileNameForTesting(string raceName)
    {
        return GetBreathingProfile(Templates[raceName]) switch
        {
            MythicalBreathingProfile.SimpleAir => "simple-air",
            MythicalBreathingProfile.PartlessAir => "partless-air",
            MythicalBreathingProfile.MarineAmphibious => "marine-amphibious",
            _ => "simple-air"
        };
    }

    internal static IReadOnlyList<string> GetHybridMovementAliasesForTesting(string bodyName)
    {
        return bodyName switch
        {
            "Naga" => ["slither", "slowslither", "quickslither"],
            "Mermaid" => ["flop"],
            "Centaur" => ["stalk", "amble", "pace", "trot", "gallop"],
            "Eastern Dragon" => ["slowfly", "fly", "franticfly"],
            _ => []
        };
    }

    private void RefreshCustomBodyCapabilities(BodyProto body)
    {
        switch (body.Name)
        {
            case "Winged Humanoid":
                PromoteCoreBodyparts(body, WingAliases);
                AddFlyMovement(body);
                break;
            case "Naga":
                AddSerpentineMovement(body);
                AddSwimMovement(body);
                break;
            case "Eastern Dragon":
                body.MinimumWingsToFly = 0;
                AddFlyMovement(body);
                break;
            case "Mermaid":
                AddFishGroundMovement(body);
                AddSwimMovement(body);
                break;
            case "Centaur":
                AddQuadrupedMovement(body);
                break;
            case "Griffin":
            case "Hippogriff":
            case "Manticore":
            case "Wyvern":
                PromoteCoreBodyparts(body, WingAliases);
                AddFlyMovement(body);
                break;
            case "Hippocamp":
                AddSwimMovement(body);
                break;
        }
    }

    private void RefreshExistingMythicalRaceDefaults()
    {
        foreach (MythicalRaceTemplate template in Templates.Values)
        {
            Race? race = _context.Races.FirstOrDefault(x => x.Name == template.Name);
            if (race is null)
            {
                continue;
            }

            race.CanClimb = template.CanClimb;
            race.CanSwim = template.CanSwim;
            race.MinimumSleepingPosition = 4;
			SatiationLimitSeederHelper.ApplyLimits(
				race,
				template.MaximumFoodSatiatedHours,
				template.MaximumDrinkSatiatedHours);
            ApplyBreathingProfile(race, GetBreathingProfile(template));
        }

        _context.SaveChanges();
    }

    private void ApplyBreathingProfile(Race race, MythicalBreathingProfile profile)
    {
        _context.RacesBreathableGases.RemoveRange(_context.RacesBreathableGases.Where(x => x.RaceId == race.Id).ToList());
        _context.RacesBreathableLiquids.RemoveRange(_context.RacesBreathableLiquids.Where(x => x.RaceId == race.Id).ToList());

        race.NeedsToBreathe = true;
        race.BreathingVolumeExpression = "7";
        race.HoldBreathLengthExpression = $"90+(5*con:{_healthTrait.Id})";

        _context.RacesBreathableGases.Add(new RacesBreathableGases
        {
            Race = race,
            Gas = _breathableAir,
            Multiplier = 1.0
        });

        switch (profile)
        {
            case MythicalBreathingProfile.SimpleAir:
                race.BreathingModel = "simple";
                break;
            case MythicalBreathingProfile.PartlessAir:
                race.BreathingModel = "partless";
                break;
            case MythicalBreathingProfile.MarineAmphibious:
                race.BreathingModel = "partless";
                _context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
                {
                    Race = race,
                    Liquid = _saltWater,
                    Multiplier = 1.0
                });
                _context.RacesBreathableLiquids.Add(new RacesBreathableLiquids
                {
                    Race = race,
                    Liquid = _brackishWater,
                    Multiplier = 0.5
                });
                break;
        }
    }

    private void PromoteCoreBodyparts(BodyProto body, params string[] aliases)
    {
        bool dirty = false;
        foreach (BodypartProto bodypart in _context.BodypartProtos
                     .Where(x => x.BodyId == body.Id && aliases.Contains(x.Name))
                     .ToList())
        {
            if (bodypart.IsCore)
            {
                continue;
            }

            bodypart.IsCore = true;
            dirty = true;
        }

        if (dirty)
        {
            _context.SaveChanges();
        }
    }

    private void AddFlyMovement(BodyProto body)
    {
        EnsurePosition(body, 18);
        CloneSpeedAliases(_avianBody, body, "slowfly", "fly", "franticfly");
    }

    private void AddSwimMovement(BodyProto body)
    {
        EnsurePosition(body, 16);
        CloneSpeedAliases(_piscineBody, body, "swim", "slowswim", "quickswim");
    }

    private void AddFishGroundMovement(BodyProto body)
    {
        EnsurePosition(body, 6);
        CloneSpeedAliases(_piscineBody, body, "flop");
    }

    private void AddSerpentineMovement(BodyProto body)
    {
        EnsurePosition(body, 1);
        CloneSpeedAliasesToPosition(_serpentineBody, body, 1, "slither", "slowslither", "quickslither");
    }

    private void AddQuadrupedMovement(BodyProto body)
    {
        EnsurePosition(body, 1);
        CloneSpeedAliases(_quadrupedBody, body, "stalk", "amble", "pace", "trot", "gallop");
    }

    private void EnsurePosition(BodyProto body, int positionId)
    {
        if (_context.BodyProtosPositions.Any(x => x.BodyProtoId == body.Id && x.Position == positionId))
        {
            return;
        }

        _context.BodyProtosPositions.Add(new BodyProtosPositions
        {
            BodyProto = body,
            Position = positionId
        });
        _context.SaveChanges();
    }

    private void CloneSpeedAliases(BodyProto source, BodyProto target, params string[] aliases)
    {
        CloneSpeedAliasesToPosition(source, target, null, aliases);
    }

    private void CloneSpeedAliasesToPosition(BodyProto source, BodyProto target, int? positionIdOverride,
        params string[] aliases)
    {
        HashSet<string> existingAliases = _context.MoveSpeeds
            .Where(x => x.BodyProtoId == target.Id)
            .Select(x => x.Alias)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        long nextId = _context.MoveSpeeds.Select(x => x.Id).AsEnumerable().DefaultIfEmpty(0).Max() + 1;

        foreach (MoveSpeed? speed in _context.MoveSpeeds
                     .Where(x => x.BodyProtoId == source.Id && aliases.Contains(x.Alias))
                     .OrderBy(x => x.Id)
                     .ToList())
        {
            if (existingAliases.Contains(speed.Alias))
            {
                continue;
            }

            _context.MoveSpeeds.Add(new MoveSpeed
            {
                Id = nextId++,
                BodyProto = target,
                PositionId = positionIdOverride ?? speed.PositionId,
                Alias = speed.Alias,
                FirstPersonVerb = speed.FirstPersonVerb,
                ThirdPersonVerb = speed.ThirdPersonVerb,
                PresentParticiple = speed.PresentParticiple,
                Multiplier = speed.Multiplier,
                StaminaMultiplier = speed.StaminaMultiplier
            });
        }

        _context.SaveChanges();
    }

    private void SeedRace(MythicalRaceTemplate template, BodyProto body)
    {
        bool usesHumanoidDefaults = template.HumanoidVariety ||
                                  (template.CanUseWeapons && template.BodyKey.EqualTo("Organic Humanoid"));
        Race race = new()
        {
            Name = template.Name,
            Description = BuildRaceDescriptionForTesting(template),
            BaseBody = body,
            AllowedGenders = usesHumanoidDefaults ? _organicHumanoidRace.AllowedGenders : "2 3",
            ParentRace = template.HumanoidVariety ? _organicHumanoidRace : null,
            AttributeTotalCap = _context.TraitDefinitions.Count(x => x.Type == (int)TraitType.Attribute) * 12,
            IndividualAttributeCap = 20,
            DiceExpression = "3d6+1",
            IlluminationPerceptionMultiplier = 1.0,
            AvailabilityProg = template.Playable ? _alwaysTrue : _alwaysFalse,
            CorpseModel = usesHumanoidDefaults ? _humanoidCorpse : _animalCorpse,
            DefaultHealthStrategy = _healthStrategy,
            DefaultCombatSetting = CombatStrategySeederHelper.EnsureCombatStrategy(_context, template.CombatStrategyKey),
            CanUseWeapons = template.CanUseWeapons,
            CanAttack = true,
            CanDefend = true,
            NeedsToBreathe = true,
            SizeStanding = (int)template.Size,
            SizeProne = (int)template.Size,
            SizeSitting = (int)template.Size,
            CommunicationStrategyType = "humanoid",
            HandednessOptions = template.CanUseWeapons ? _organicHumanoidRace.HandednessOptions : "1 3",
            DefaultHandedness = 1,
            ChildAge = template.AgeProfile.ChildAge,
            YouthAge = template.AgeProfile.YouthAge,
            YoungAdultAge = template.AgeProfile.YoungAdultAge,
            AdultAge = template.AgeProfile.AdultAge,
            ElderAge = template.AgeProfile.ElderAge,
            VenerableAge = template.AgeProfile.VenerableAge,
            CanClimb = template.CanClimb,
            CanSwim = template.CanSwim,
            MinimumSleepingPosition = 4,
            BodypartHealthMultiplier = template.BodypartHealthMultiplier,
            BodypartSizeModifier = 0,
            TemperatureRangeCeiling = 40,
            TemperatureRangeFloor = 0,
            CanEatCorpses = false,
            CanEatMaterialsOptIn = false,
            BiteWeight = 1000,
            EatCorpseEmoteText = string.Empty,
            RaceUsesStamina = true,
            NaturalArmourQuality = 2,
            SweatLiquid = usesHumanoidDefaults ? _sweat : null,
            SweatRateInLitresPerMinute = usesHumanoidDefaults && _sweat is not null ? 0.5 : 0.0,
            BloodLiquid = _blood,
            BloodModel = usesHumanoidDefaults ? _humanRace.BloodModel : null,
            BreathingModel = "simple",
            BreathingVolumeExpression = "7",
            HoldBreathLengthExpression = $"90+(5*con:{_healthTrait.Id})",
            MaximumLiftWeightExpression = $"str:{_strengthTrait.Id}*10000",
            MaximumDragWeightExpression = $"str:{_strengthTrait.Id}*40000",
			MaximumFoodSatiatedHours = template.MaximumFoodSatiatedHours,
			MaximumDrinkSatiatedHours = template.MaximumDrinkSatiatedHours,
            DefaultHeightWeightModelMale = _context.HeightWeightModels.First(x => x.Name == template.MaleHeightWeightModel),
			DefaultHeightWeightModelFemale = _context.HeightWeightModels.First(x => x.Name == template.FemaleHeightWeightModel),
			DefaultHeightWeightModelNeuter = _context.HeightWeightModels.First(x => x.Name == template.MaleHeightWeightModel),
			DefaultHeightWeightModelNonBinary = _context.HeightWeightModels.First(x => x.Name == template.FemaleHeightWeightModel),
			NaturalArmourType = usesHumanoidDefaults ? _humanoidNaturalArmour : null
		};

        _context.Races.Add(race);
        _context.SaveChanges();

        foreach (TraitDefinition? attribute in _context.TraitDefinitions
                     .Where(x => x.Type == (int)TraitType.Attribute || x.Type == (int)TraitType.DerivedAttribute))
        {
            _context.RacesAttributes.Add(new RacesAttributes
            {
                Race = race,
                Attribute = attribute,
                IsHealthAttribute = attribute.TraitGroup == "Physical",
                AttributeBonus = GetMythicalAttributeBonus(attribute, template)
            });
        }
        ApplyBreathingProfile(race, GetBreathingProfile(template));

        Ethnicity ethnicity = SeedEthnicity(race, template);
        SeedAdditionalCharacteristics(race, ethnicity, template);
        SeedRacialBodypartUsages(race, template);
        SeedNaturalAttacks(race, template);

        if (!template.HumanoidVariety)
        {
            SeedDefaultDescriptions(race, template);
        }

        _context.SaveChanges();
    }

	private void ApplyDefaultCombatSettingsToSeededRaces()
	{
		foreach (MythicalRaceTemplate template in Templates.Values)
		{
			Race? race = _context.Races.FirstOrDefault(x => x.Name == template.Name);
            if (race is null)
            {
                continue;
            }

			CharacterCombatSetting setting = CombatStrategySeederHelper.EnsureCombatStrategy(_context, template.CombatStrategyKey);
			bool usesHumanoidDefaults = template.HumanoidVariety ||
			                          (template.CanUseWeapons && template.BodyKey.EqualTo("Organic Humanoid"));
			ArmourType? expectedArmour = usesHumanoidDefaults ? _humanoidNaturalArmour : _animalNaturalArmour;
            double expectedHealthMultiplier = ResolveMythicalHealthMultiplier(template);
			if (race.DefaultCombatSettingId == setting.Id &&
			    race.NaturalArmourTypeId == expectedArmour?.Id &&
                Math.Abs(race.BodypartHealthMultiplier - expectedHealthMultiplier) < 0.0001)
			{
				continue;
			}

			race.DefaultCombatSetting = setting;
			race.NaturalArmourType = expectedArmour;
            race.BodypartHealthMultiplier = expectedHealthMultiplier;
		}

        _context.SaveChanges();
    }

    private Ethnicity SeedEthnicity(Race race, MythicalRaceTemplate template)
    {
        Ethnicity ethnicity = new()
        {
            Name = $"{template.Name} Stock",
            ChargenBlurb = BuildEthnicityDescriptionForTesting(template),
            AvailabilityProg = template.Playable ? _alwaysTrue : _alwaysFalse,
            ParentRace = race,
            EthnicGroup = template.Name,
            EthnicSubgroup = "Stock",
            PopulationBloodModel = _defaultPopulationBloodModel,
            TolerableTemperatureFloorEffect = 0,
            TolerableTemperatureCeilingEffect = 0
        };
        _context.Ethnicities.Add(ethnicity);
        _context.SaveChanges();

        if (!template.HumanoidVariety)
        {
            return ethnicity;
        }

        AddEthnicityCharacteristic(ethnicity, "Eye Colour", "All Eye Colours");
        AddEthnicityCharacteristic(ethnicity, "Eye Shape", "All Eye Shapes");
        AddEthnicityCharacteristic(ethnicity, "Nose", "All Noses");
        AddEthnicityCharacteristic(ethnicity, "Ears", "All Ears");
        AddEthnicityCharacteristic(ethnicity, "Hair Colour", "All Hair Colours");
        AddEthnicityCharacteristic(ethnicity, "Facial Hair Colour", "All Facial Hair Colours");
        AddEthnicityCharacteristic(ethnicity, "Hair Style", "All Hair Styles");
        AddEthnicityCharacteristic(ethnicity, "Facial Hair Style",
            template.FacialHairProfileName ?? "All Facial Hair Styles");
        AddEthnicityCharacteristic(ethnicity, "Skin Colour", "All Skin Colours");
        AddEthnicityCharacteristic(ethnicity, "Frame", "All Frames");
        AddEthnicityCharacteristic(ethnicity, "Person Word",
            EnsureStandardProfile(_personWordDefinition, $"{template.Name} Person Words", template.PersonWords!).Name);

        if (_context.CharacteristicDefinitions.Any(x => x.Name == "Distinctive Feature") &&
            _context.CharacteristicProfiles.Any(x => x.Name == "All Distinctive Features"))
        {
            AddEthnicityCharacteristic(ethnicity, "Distinctive Feature", "All Distinctive Features");
        }

        return ethnicity;
    }

    private void AddEthnicityCharacteristic(Ethnicity ethnicity, string definitionName, string profileName)
    {
        CharacteristicDefinition? definition = _context.CharacteristicDefinitions.FirstOrDefault(x => x.Name == definitionName);
        CharacteristicProfile? profile = _context.CharacteristicProfiles.FirstOrDefault(x => x.Name == profileName);
        if (definition is null || profile is null)
        {
            return;
        }

        if (_context.EthnicitiesCharacteristics.Any(x =>
                x.EthnicityId == ethnicity.Id && x.CharacteristicDefinitionId == definition.Id))
        {
            return;
        }

        _context.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics
        {
            Ethnicity = ethnicity,
            CharacteristicDefinition = definition,
            CharacteristicProfile = profile
        });
        _context.SaveChanges();
    }

    private void SeedAdditionalCharacteristics(Race race, Ethnicity ethnicity, MythicalRaceTemplate template)
    {
        if (template.AdditionalCharacteristics is null)
        {
            return;
        }

        foreach (MythicalCharacteristicTemplate characteristic in template.AdditionalCharacteristics)
        {
            CharacteristicDefinition definition = EnsureCharacteristicDefinition(characteristic.DefinitionName,
                $"{characteristic.DefinitionName} for {template.Name}",
                characteristic.Type);
            CharacteristicProfile profile = EnsureStandardProfile(definition, $"All {characteristic.DefinitionName}", characteristic.Values);

            if (!_context.RacesAdditionalCharacteristics.Any(x =>
                    x.RaceId == race.Id && x.CharacteristicDefinitionId == definition.Id))
            {
                _context.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
                {
                    Race = race,
                    CharacteristicDefinition = definition,
                    Usage = characteristic.Usage
                });
            }

            if (!_context.EthnicitiesCharacteristics.Any(x =>
                    x.EthnicityId == ethnicity.Id && x.CharacteristicDefinitionId == definition.Id))
            {
                _context.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics
                {
                    Ethnicity = ethnicity,
                    CharacteristicDefinition = definition,
                    CharacteristicProfile = profile
                });
            }
        }

        _context.SaveChanges();
    }

    private CharacteristicDefinition EnsureCharacteristicDefinition(string name, string description,
        CharacteristicType type = CharacteristicType.Standard)
    {
        CharacteristicDefinition? existing = _context.CharacteristicDefinitions.FirstOrDefault(x => x.Name == name);
        if (existing is not null)
        {
            return existing;
        }

        CharacteristicDefinition definition = new()
        {
            Name = name,
            Type = (int)type,
            Pattern = name.ToLowerInvariant().Replace(" ", string.Empty),
            Description = description,
            ChargenDisplayType = (int)CharacterGenerationDisplayType.DisplayAll,
            Model = "standard",
            Definition = string.Empty
        };
        _context.CharacteristicDefinitions.Add(definition);
        _context.SaveChanges();
        return definition;
    }

    private CharacteristicProfile EnsureStandardProfile(CharacteristicDefinition definition, string profileName,
        IEnumerable<string> values)
    {
        CharacteristicProfile? existing = _context.CharacteristicProfiles.FirstOrDefault(x => x.Name == profileName);
        if (existing is not null)
        {
            return existing;
        }

        List<string> valueList = values.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        foreach (string? value in valueList)
        {
            if (_context.CharacteristicValues.Any(x => x.DefinitionId == definition.Id && x.Name == value))
            {
                continue;
            }

            _context.CharacteristicValues.Add(new CharacteristicValue
            {
                Name = value,
                Definition = definition,
                Value = value,
                AdditionalValue = string.Empty,
                Default = false,
                Pluralisation = 0
            });
        }

        StringBuilder definitionXml = new();
        definitionXml.Append("<Values>");
        foreach (string? value in valueList)
        {
            definitionXml.Append($" <Value>{value}</Value>");
        }

        definitionXml.Append(" </Values>");

        CharacteristicProfile profile = new()
        {
            Name = profileName,
            TargetDefinition = definition,
            Type = "Standard",
            Description = $"Auto-generated profile for {profileName}",
            Definition = definitionXml.ToString()
        };
        _context.CharacteristicProfiles.Add(profile);
        _context.SaveChanges();
        return profile;
    }

    private void SeedRacialBodypartUsages(Race race, MythicalRaceTemplate template)
    {
        if (template.BodypartUsages is null)
        {
            return;
        }

        foreach (MythicalBodypartUsageTemplate usage in template.BodypartUsages)
        {
            BodypartProto? bodypart = FindBodypartOnBody(race.BaseBody, usage.BodypartAlias);
            if (bodypart is null)
            {
                continue;
            }

            _context.RacesAdditionalBodyparts.Add(new RacesAdditionalBodyparts
            {
                Race = race,
                Bodypart = bodypart,
                Usage = usage.Usage
            });
        }

        _context.SaveChanges();
    }

    private void SeedNaturalAttacks(Race race, MythicalRaceTemplate template)
    {
        foreach (MythicalAttackTemplate attackTemplate in template.Attacks)
        {
            WeaponAttack attack = _context.WeaponAttacks.First(x => x.Name == attackTemplate.AttackName);
            foreach (string alias in attackTemplate.BodypartAliases)
            {
                BodypartProto? bodypart = FindBodypartOnBody(race.BaseBody, alias);
                if (bodypart is null)
                {
                    continue;
                }

                _context.RacesWeaponAttacks.Add(new RacesWeaponAttacks
                {
                    Race = race,
                    Bodypart = bodypart,
                    WeaponAttack = attack,
                    Quality = (int)attackTemplate.Quality
                });
            }
        }

        _context.SaveChanges();
    }

    private void SeedDefaultDescriptions(Race race, MythicalRaceTemplate template)
    {
        IReadOnlyList<StockDescriptionVariant>? variants = template.HumanoidVariety
            ? template.OverlayDescriptionVariants
            : template.DescriptionVariants;
        if (variants == null || variants.Count == 0)
        {
            return;
        }

        FutureProg prog = EnsureRaceDescriptionApplicabilityProg(race);
        EnsureDescriptionVariants(prog, variants, !template.HumanoidVariety);
        _context.SaveChanges();
    }

    private FutureProg EnsureRaceDescriptionApplicabilityProg(Race race)
    {
        string functionName = $"Is{race.Name.CollapseString()}";
        FutureProg? prog = _context.FutureProgs.FirstOrDefault(x => x.FunctionName == functionName);
        if (prog is null)
        {
            prog = new FutureProg
            {
                FunctionName = functionName
            };
            _context.FutureProgs.Add(prog);
        }

        prog.Category = "Character";
        prog.Subcategory = "Descriptions";
        prog.FunctionComment = $"True if the character is a {race.Name}";
        prog.ReturnType = (long)ProgVariableTypes.Boolean;
        prog.StaticType = 0;
        prog.AcceptsAnyParameters = false;
        prog.Public = true;
        prog.FunctionText = $"return @ch.Race == ToRace(\"{race.Name}\")";

        if (!prog.FutureProgsParameters.Any(x => x.ParameterIndex == 0))
        {
            prog.FutureProgsParameters.Add(new FutureProgsParameter
            {
                FutureProg = prog,
                ParameterIndex = 0,
                ParameterName = "ch",
                ParameterType = (long)ProgVariableTypes.Toon
            });
        }

        _context.SaveChanges();
        return prog;
    }

    private void EnsureDescriptionVariants(FutureProg prog, IEnumerable<StockDescriptionVariant> variants,
        bool replaceExisting)
    {
        List<StockDescriptionVariant> variantList = variants.ToList();
        if (replaceExisting)
        {
            List<EntityDescriptionPattern> existing = _context.EntityDescriptionPatterns
                .Where(x => x.ApplicabilityProgId == prog.Id && (x.Type == 0 || x.Type == 1))
                .ToList();
            if (existing.Any())
            {
                _context.EntityDescriptionPatterns.RemoveRange(existing);
                _context.SaveChanges();
            }
        }

        foreach (StockDescriptionVariant variant in variantList)
        {
            if (!_context.EntityDescriptionPatterns.Any(x =>
                    x.ApplicabilityProgId == prog.Id &&
                    x.Type == 0 &&
                    x.Pattern == variant.ShortDescription))
            {
                _context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
                {
                    Type = 0,
                    ApplicabilityProg = prog,
                    RelativeWeight = 100,
                    Pattern = variant.ShortDescription
                });
            }

            if (!_context.EntityDescriptionPatterns.Any(x =>
                    x.ApplicabilityProgId == prog.Id &&
                    x.Type == 1 &&
                    x.Pattern == variant.FullDescription))
            {
                _context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
                {
                    Type = 1,
                    ApplicabilityProg = prog,
                    RelativeWeight = 100,
                    Pattern = variant.FullDescription
                });
            }
        }
    }

    private BodypartProto? FindBodypartOnBody(BodyProto body, string alias)
    {
        return SeederBodyUtilities.FindBodypartOnBodyOrAncestors(_context, body, alias);
    }

    private void AddBodypartRemoval(BodyProto body, params string[] aliases)
    {
        bool dirty = false;
        foreach (string? alias in aliases.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            BodypartProto? bodypart = FindBodypartOnBody(body, alias);
            if (bodypart is null)
            {
                continue;
            }

            if (_context.BodyProtosAdditionalBodyparts.Any(x =>
                    x.BodyProtoId == body.Id &&
                    x.BodypartId == bodypart.Id &&
                    x.Usage == "remove"))
            {
                continue;
            }

            _context.BodyProtosAdditionalBodyparts.Add(new BodyProtosAdditionalBodyparts
            {
                BodyProto = body,
                Bodypart = bodypart,
                Usage = "remove"
            });
            dirty = true;
        }

        if (dirty)
        {
            _context.SaveChanges();
        }
    }
}
