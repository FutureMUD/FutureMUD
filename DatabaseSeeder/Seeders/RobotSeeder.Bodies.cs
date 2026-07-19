#nullable enable

using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
private static readonly string[] HumanoidRobotOrganAliases = ["powercore", "positronicbrain", "sensorarray", "speechsynth"];
    private static readonly string[] NonSpeakingRobotOrganAliases = ["powercore", "positronicbrain", "sensorarray"];
    private static readonly string[] HumanoidHandRemovalAliases =
    [
        "rhand",
        "rthumb",
        "rindexfinger",
        "rmiddlefinger",
        "rringfinger",
        "rpinkyfinger",
        "lhand",
        "lthumb",
        "lindexfinger",
        "lmiddlefinger",
        "lringfinger",
        "lpinkyfinger"
    ];
    private static readonly string[] HumanoidLowerLegRemovalAliases =
    [
        "rankle",
        "rheel",
        "rfoot",
        "rbigtoe",
        "rindextoe",
        "rmiddletoe",
        "rringtoe",
        "rpinkytoe",
        "lankle",
        "lheel",
        "lfoot",
        "lbigtoe",
        "lindextoe",
        "lmiddletoe",
        "lringtoe",
        "lpinkytoe"
    ];
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
    private static readonly IReadOnlyDictionary<string, IReadOnlyList<(string LimbRootAlias, string PartAlias)>> CustomRobotLimbMemberships =
        new Dictionary<string, IReadOnlyList<(string LimbRootAlias, string PartAlias)>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Mandible Robot"] = [("neck", "mandibles")],
            ["Wheeled Robot"] = [("rhip", "rwheel"), ("lhip", "lwheel")],
            ["Tracked Robot"] = [("rhip", "rtrack"), ("lhip", "ltrack")]
        };

    internal static IReadOnlyDictionary<string, IReadOnlyList<(string LimbRootAlias, string PartAlias)>> CustomLimbMembershipsForTesting =>
        CustomRobotLimbMemberships;

    private Dictionary<string, BodyProto> EnsureBodyCatalogue(RobotSeedSummary summary)
    {
        Dictionary<string, BodyProto> bodies = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Robot Humanoid"] = GetOrCreateBody("Robot Humanoid", CreateRobotHumanoidBody, summary),
            ["Robot Quadruped"] = GetOrCreateBody("Robot Quadruped", CreateRobotQuadrupedBody, summary),
            ["Robot Insectoid"] = GetOrCreateBody("Robot Insectoid", CreateRobotInsectoidBody, summary),
            ["Robot Utility"] = GetOrCreateBody("Robot Utility", CreateRobotUtilityBody, summary)
        };

        bodies["Spider Crawler Robot"] = GetOrCreateBody("Spider Crawler Robot", CreateSpiderCrawlerBody, summary);
        bodies["Circular Saw Robot"] = GetOrCreateBody("Circular Saw Robot", CreateCircularSawBody, summary);
        bodies["Pneumatic Hammer Robot"] = GetOrCreateBody("Pneumatic Hammer Robot", CreatePneumaticHammerBody, summary);
        bodies["Sword-Hand Robot"] = GetOrCreateBody("Sword-Hand Robot", CreateSwordHandBody, summary);
        if (CanSeedOptionalBody("Winged Robot"))
        {
            bodies["Winged Robot"] = GetOrCreateBody("Winged Robot", CreateWingedRobotBody, summary);
        }

        if (CanSeedOptionalBody("Jet Robot"))
        {
            bodies["Jet Robot"] = GetOrCreateBody("Jet Robot", CreateJetRobotBody, summary);
        }

        bodies["Mandible Robot"] = GetOrCreateBody("Mandible Robot", CreateMandibleRobotBody, summary);
        bodies["Wheeled Robot"] = GetOrCreateBody("Wheeled Robot", CreateWheeledRobotBody, summary);
        bodies["Tracked Robot"] = GetOrCreateBody("Tracked Robot", CreateTrackedRobotBody, summary);
        bodies["Cyborg Humanoid"] = GetOrCreateBody("Cyborg Humanoid", CreateCyborgHumanoidBody, summary);
        bodies["Roomba Robot"] = GetOrCreateBody("Roomba Robot", CreateRoombaBody, summary);
        bodies["Tracked Utility Robot"] = GetOrCreateBody("Tracked Utility Robot", CreateTrackedUtilityBody, summary);
        bodies["Robot Dog"] = GetOrCreateBody("Robot Dog", CreateRobotDogBody, summary);
        bodies["Robot Cockroach"] = GetOrCreateBody("Robot Cockroach", CreateRobotCockroachBody, summary);

        return bodies;
    }

    private bool CanSeedOptionalBody(string bodyName)
    {
        return _context.BodyProtos.Any(x => x.Name == bodyName) ||
               (_avianBody is not null && CanSupportBodyKey(new[] { "Avian" }, bodyName));
    }

    private BodyProto GetOrCreateBody(string name, Func<BodyProto> factory, RobotSeedSummary summary)
    {
        BodyProto? existing = _context.BodyProtos.FirstOrDefault(x => x.Name == name);
        if (existing is not null)
        {
            return existing;
        }

        BodyProto created = factory();
        ValidateRobotBody(created);
        summary.BodiesAdded++;
        return created;
    }

    private void ValidateRobotBody(BodyProto body)
    {
        ApplyCustomLimbMemberships(body);

        IReadOnlyList<BodypartProto> uncoveredParts = SeederBodyUtilities.GetExternalBodypartsWithoutLimbCoverage(_context, body);
        if (uncoveredParts.Any())
        {
            throw new InvalidOperationException(
                $"Robot body {body.Name} has external bodyparts without limb coverage: {string.Join(", ", uncoveredParts.Select(x => x.Name))}");
        }
    }

    private void ApplyCustomLimbMemberships(BodyProto body)
    {
        if (!CustomRobotLimbMemberships.TryGetValue(body.Name, out IReadOnlyList<(string LimbRootAlias, string PartAlias)>? mappings))
        {
            return;
        }

        foreach ((string? limbRootAlias, string? partAlias) in mappings)
        {
            AddLimbPart(body, limbRootAlias, partAlias);
        }
    }

    private BodyProto CreateRobotHumanoidBody()
    {
        BodyProto body = CreateInheritedBody("Robot Humanoid", _humanoidBody, cloneSourceDefinition: true);
        ApplyAliasCountAs(body, _humanoidBody);
        ConfigureRobotBodyMaterials(body, false);
        AddRobotHumanoidOrgans(body);
        EnsureDefaultSmashingBodypart(body, "rhand");
        return body;
    }

    private BodyProto CreateRobotQuadrupedBody()
    {
        BodyProto body = CloneBody("Robot Quadruped", _toedQuadrupedBody);
        RemoveOrganicInternals(body);
        ConfigureRobotBodyMaterials(body, true);
        AddQuadrupedRobotOrgans(body);
        EnsureDefaultSmashingBodypart(body, "rfpaw");
        return body;
    }

    private BodyProto CreateRobotInsectoidBody()
    {
        BodyProto body = CloneBody("Robot Insectoid", _insectoidBody);
        RemoveOrganicInternals(body);
        ConfigureRobotBodyMaterials(body, true);
        AddInsectoidRobotOrgans(body);
        EnsureDefaultSmashingBodypart(body, "mandibles");
        return body;
    }

    private BodyProto CreateRobotUtilityBody()
    {
        BodyProto body = new()
        {
            Name = "Robot Utility",
            CountsAs = null,
            ConsiderString = string.Empty,
            WielderDescriptionSingle = "manipulator",
            WielderDescriptionPlural = "manipulators",
            StaminaRecoveryProgId = _robotStaminaRecoveryProg.Id,
            MinimumLegsToStand = 2,
            MinimumWingsToFly = 0,
            LegDescriptionPlural = "drive assemblies",
            LegDescriptionSingular = "drive assembly",
            WearSizeParameter = _humanoidBody.WearSizeParameter,
            NameForTracking = "robot"
        };
        _context.BodyProtos.Add(body);
        _context.SaveChanges();

        AddMissingBodyMovement(_humanoidBody, body);

        BodypartProto chassis = AddBodypart(body, "chassis", "service chassis", "Chassis", BodypartTypeEnum.Wear, null,
            Alignment.Irrelevant, Orientation.Centre, 220, 120, 220, 1, _chassisAlloy, _robotLightPlatingArmour,
            SizeCategory.Small, significant: true, damageModifier: 0.8, stunModifier: 0.8);
        BodypartProto sensorPod = AddBodypart(body, "sensorpod", "sensor pod", "Chassis", BodypartTypeEnum.Wear, "chassis",
            Alignment.Front, Orientation.High, 100, 90, 120, 2, _chassisAlloy, _robotLightPlatingArmour,
            SizeCategory.Small, significant: true);
        BodypartProto rightEye = AddBodypart(body, "reye", "right optical lens", "eye", BodypartTypeEnum.Eye, "sensorpod",
            Alignment.Right, Orientation.High, 35, 15, 20, 3, _chassisAlloy, _robotLightPlatingArmour, SizeCategory.Tiny);
        BodypartProto leftEye = AddBodypart(body, "leye", "left optical lens", "eye", BodypartTypeEnum.Eye, "sensorpod",
            Alignment.Left, Orientation.High, 35, 15, 20, 4, _chassisAlloy, _robotLightPlatingArmour, SizeCategory.Tiny);
        BodypartProto rightWheel = AddBodypart(body, "rdrivewheel", "right drive wheel", "Wheel", BodypartTypeEnum.Standing, "chassis",
            Alignment.Right, Orientation.Lowest, 90, 75, 110, 5, _chassisAlloy, _robotLightPlatingArmour, SizeCategory.Small);
        BodypartProto leftWheel = AddBodypart(body, "ldrivewheel", "left drive wheel", "Wheel", BodypartTypeEnum.Standing, "chassis",
            Alignment.Left, Orientation.Lowest, 90, 75, 110, 6, _chassisAlloy, _robotLightPlatingArmour, SizeCategory.Small);

        AddRobotOrgan(body, "positronicbrain", "positronic brain", "Processor", BodypartTypeEnum.PositronicBrain,
            "sensorpod", Alignment.Irrelevant, Orientation.Centre, SizeCategory.Small, true, true,
            ["sensorpod", "reye", "leye"]);
        AddRobotOrgan(body, "sensorarray", "sensor array", "Sensor Cluster", BodypartTypeEnum.SensorArray,
            "sensorpod", Alignment.Irrelevant, Orientation.Centre, SizeCategory.Small, false, false,
            ["sensorpod", "reye", "leye"]);
        AddRobotOrgan(body, "powercore", "power core", "Power Core", BodypartTypeEnum.PowerCore,
            "chassis", Alignment.Irrelevant, Orientation.Centre, SizeCategory.Small, true, true, ["chassis"]);

        AddLimb(body, "Sensor Pod", LimbType.Head, sensorPod, [sensorPod, rightEye, leftEye]);
        AddLimb(body, "Right Drive", LimbType.Leg, rightWheel, [rightWheel]);
        AddLimb(body, "Left Drive", LimbType.Leg, leftWheel, [leftWheel]);
        AddLimb(body, "Chassis", LimbType.Torso, chassis, [chassis]);
        body.DefaultSmashingBodypart = rightWheel;
        _context.SaveChanges();
        return body;
    }

    private BodyProto CloneBody(string newName, BodyProto source)
    {
        BodyProto body = new()
        {
            Name = newName,
            CountsAs = null,
            ConsiderString = source.ConsiderString,
            WielderDescriptionPlural = source.WielderDescriptionPlural,
            WielderDescriptionSingle = source.WielderDescriptionSingle,
            StaminaRecoveryProgId = _robotStaminaRecoveryProg.Id,
            MinimumLegsToStand = source.MinimumLegsToStand,
            MinimumWingsToFly = source.MinimumWingsToFly,
            LegDescriptionSingular = source.LegDescriptionSingular,
            LegDescriptionPlural = source.LegDescriptionPlural,
            WearSizeParameter = source.WearSizeParameter,
            NameForTracking = string.IsNullOrWhiteSpace(source.NameForTracking) ? "robot" : source.NameForTracking
        };
        _context.BodyProtos.Add(body);
        _context.SaveChanges();

        SeederBodyUtilities.CloneFlattenedBodyDefinition(_context, source, body);
        SeederBodyUtilities.CloneFlattenedBodyPositionsAndSpeeds(_context, source, body);
        ApplyAliasCountAs(body, source);
        CopyDefaultSmashingBodypart(body, source);
        return body;
    }

    private BodyProto CreateInheritedBody(
        string newName,
        BodyProto source,
        int? minimumLegsToStand = null,
        int? minimumWingsToFly = null,
        bool cloneSourceDefinition = false)
    {
        BodyProto body = new()
        {
            Name = newName,
            CountsAs = source,
            ConsiderString = source.ConsiderString,
            WielderDescriptionPlural = source.WielderDescriptionPlural,
            WielderDescriptionSingle = source.WielderDescriptionSingle,
            StaminaRecoveryProgId = _robotStaminaRecoveryProg.Id,
            MinimumLegsToStand = minimumLegsToStand ?? source.MinimumLegsToStand,
            MinimumWingsToFly = minimumWingsToFly ?? source.MinimumWingsToFly,
            LegDescriptionSingular = source.LegDescriptionSingular,
            LegDescriptionPlural = source.LegDescriptionPlural,
            WearSizeParameter = source.WearSizeParameter,
            NameForTracking = string.IsNullOrWhiteSpace(source.NameForTracking) ? "robot" : source.NameForTracking
        };
        _context.BodyProtos.Add(body);
        _context.SaveChanges();

        AddMissingBodyMovement(source, body);
        if (cloneSourceDefinition)
        {
            SeederBodyUtilities.CloneBodyDefinition(_context, source, body, cloneAdditionalUsages: false,
                cloneGroupDescribers: false);
        }

        return body;
    }

    private void ApplyAliasCountAs(BodyProto targetBody, BodyProto sourceBody)
    {
        Dictionary<string, BodypartProto> sourceParts = new(StringComparer.OrdinalIgnoreCase);
        Stack<BodyProto> sourceBodies = new();
        BodyProto? currentBody = sourceBody;
        while (currentBody is not null)
        {
            sourceBodies.Push(currentBody);
            currentBody = currentBody.CountsAsId.HasValue
                ? _context.BodyProtos.Find(currentBody.CountsAsId.Value)
                : null;
        }

        while (sourceBodies.Count > 0)
        {
            foreach (BodypartProto? sourcePart in _context.BodypartProtos.Where(x => x.BodyId == sourceBodies.Pop().Id).ToList())
            {
                sourceParts[sourcePart.Name] = sourcePart;
            }
        }

        foreach (BodypartProto? part in _context.BodypartProtos.Where(x => x.BodyId == targetBody.Id).ToList())
        {
            if (!sourceParts.TryGetValue(part.Name, out BodypartProto? sourcePart))
            {
                continue;
            }

            part.CountAsId = sourcePart.Id;
        }

        _context.SaveChanges();
    }

    private void CopyDefaultSmashingBodypart(BodyProto targetBody, BodyProto sourceBody)
    {
        if (!sourceBody.DefaultSmashingBodypartId.HasValue)
        {
            return;
        }

        string? sourceAlias = _context.BodypartProtos.FirstOrDefault(x => x.Id == sourceBody.DefaultSmashingBodypartId.Value)?.Name;
        if (sourceAlias is null)
        {
            return;
        }

        BodypartProto? targetPart = FindBodypartOnBody(targetBody, sourceAlias);
        if (targetPart is null)
        {
            return;
        }

        targetBody.DefaultSmashingBodypart = targetPart;
        _context.SaveChanges();
    }

    private void RemoveOrganicInternals(BodyProto body)
    {
        List<string> aliases = _context.BodypartProtos
            .Where(x => x.BodyId == body.Id)
            .Where(x =>
                x.IsOrgan == 1 ||
                x.BodypartType == (int)BodypartTypeEnum.Bone ||
                x.BodypartType == (int)BodypartTypeEnum.NonImmobilisingBone ||
                x.BodypartType == (int)BodypartTypeEnum.MinorBone ||
                x.BodypartType == (int)BodypartTypeEnum.MinorNonImobilisingBone)
            .Select(x => x.Name)
            .ToList();
        SeederBodyUtilities.RemoveBodyparts(_context, body, aliases);
    }

    private void ConfigureRobotBodyMaterials(BodyProto body, bool lightArmour)
    {
        foreach (BodypartProto? part in _context.BodypartProtos.Where(x => x.BodyId == body.Id).ToList())
        {
            part.DefaultMaterial = part.IsOrgan == 1 ? _circuitryMaterial : _chassisAlloy;
            part.ArmourType = part.IsOrgan == 1 ? _robotInternalArmour : (lightArmour ? _robotLightPlatingArmour : _robotPlatingArmour);
            part.RelativeInfectability = 0.0;
            part.PainModifier = 0.0;
            if (part.BodypartType == (int)BodypartTypeEnum.Eye || part.BodypartType == (int)BodypartTypeEnum.Ear)
            {
                part.BleedModifier = 0.0;
            }
        }

        _context.SaveChanges();
    }

private BodyProto CreateSpiderCrawlerBody()
    {
        BodyProto source = _context.BodyProtos.First(x => x.Name == "Robot Humanoid");
        BodyProto body = CreateInheritedBody("Spider Crawler Robot", source, minimumLegsToStand: 4);
        AddBodypartRemoval(body, HumanoidFullLegRemovalAliases);
        foreach (string? alias in new[] { "rleg1", "lleg1", "rleg2", "lleg2", "rleg3", "lleg3", "rleg4", "lleg4" })
        {
            SeederBodyUtilities.CloneBodypartSubtree(_context, _arachnidBody, body, alias, "abdomen");
        }

        ConfigureRobotBodyMaterials(body, false);
        body.MinimumLegsToStand = 4;
        body.LegDescriptionSingular = "crawler leg";
        body.LegDescriptionPlural = "crawler legs";
        EnsureDefaultSmashingBodypart(body, "rleg1");
        _context.SaveChanges();
        return body;
    }

    private BodyProto CreateCircularSawBody()
    {
        return CreateHandAttachmentVariant("Circular Saw Robot", "rsaw", "lsaw", "circular saw", "Circular Saw");
    }

    private BodyProto CreatePneumaticHammerBody()
    {
        return CreateHandAttachmentVariant("Pneumatic Hammer Robot", "rhammer", "lhammer", "pneumatic hammer", "Hammer Head");
    }

    private BodyProto CreateSwordHandBody()
    {
        return CreateHandAttachmentVariant("Sword-Hand Robot", "rblade", "lblade", "sword-hand blade", "Sword Blade");
    }

    private BodyProto CreateWingedRobotBody()
    {
        BodyProto avianBody = _avianBody ?? throw new InvalidOperationException("Winged Robot requires the Avian body.");
        BodyProto source = _context.BodyProtos.First(x => x.Name == "Robot Humanoid");
        BodyProto body = CreateInheritedBody("Winged Robot", source, minimumWingsToFly: 2);
        SeederBodyUtilities.CloneBodypartSubtree(_context, avianBody, body, "rwingbase", "uback");
        SeederBodyUtilities.CloneBodypartSubtree(_context, avianBody, body, "lwingbase", "uback");
        AddMissingBodyMovement(avianBody, body);
        ConfigureRobotBodyMaterials(body, false);
        return body;
    }

    private BodyProto CreateJetRobotBody()
    {
        BodyProto avianBody = _avianBody ?? throw new InvalidOperationException("Jet Robot requires the Avian body.");
        BodyProto source = _context.BodyProtos.First(x => x.Name == "Robot Humanoid");
        BodyProto body = CreateInheritedBody("Jet Robot", source, minimumWingsToFly: 2);
        AddBodypart(body, "rjet", "right jet pod", "Jet Pod", BodypartTypeEnum.Wing, "uback",
            Alignment.Right, Orientation.High, 50, 50, 70, 1000, _chassisAlloy, _robotPlatingArmour, SizeCategory.Small,
            significant: true);
        AddBodypart(body, "ljet", "left jet pod", "Jet Pod", BodypartTypeEnum.Wing, "uback",
            Alignment.Left, Orientation.High, 50, 50, 70, 1001, _chassisAlloy, _robotPlatingArmour, SizeCategory.Small,
            significant: true);
        AddLimbPart(body, "rupperarm", "rjet");
        AddLimbPart(body, "lupperarm", "ljet");
        AddMissingBodyMovement(avianBody, body);
        ConfigureRobotBodyMaterials(body, false);
        return body;
    }

    private BodyProto CreateMandibleRobotBody()
    {
        BodyProto source = _context.BodyProtos.First(x => x.Name == "Robot Humanoid");
        BodyProto body = CreateInheritedBody("Mandible Robot", source);
        SeederBodyUtilities.CloneBodypartSubtree(_context, _insectoidBody, body, "mandibles", "mouth");
        AddLimbPart(body, "neck", "mandibles");
        ConfigureRobotBodyMaterials(body, false);
        return body;
    }

    private BodyProto CreateWheeledRobotBody()
    {
        BodyProto source = _context.BodyProtos.First(x => x.Name == "Robot Humanoid");
        BodyProto body = CreateInheritedBody("Wheeled Robot", source);
        AddBodypartRemoval(body, HumanoidLowerLegRemovalAliases);
        BodypartProto rightWheel = AddBodypart(body, "rwheel", "right wheel assembly", "Wheel", BodypartTypeEnum.Standing, "rshin",
            Alignment.Right, Orientation.Lowest, 80, 80, 100, 1000, _chassisAlloy, _robotPlatingArmour, SizeCategory.Small,
            countAs: FindBodypartOnBody(source, "rfoot"));
        BodypartProto leftWheel = AddBodypart(body, "lwheel", "left wheel assembly", "Wheel", BodypartTypeEnum.Standing, "lshin",
            Alignment.Left, Orientation.Lowest, 80, 80, 100, 1001, _chassisAlloy, _robotPlatingArmour, SizeCategory.Small,
            countAs: FindBodypartOnBody(source, "lfoot"));
        AddLimbPart(body, "rhip", "rwheel");
        AddLimbPart(body, "lhip", "lwheel");
        body.LegDescriptionSingular = "wheel";
        body.LegDescriptionPlural = "wheels";
        body.DefaultSmashingBodypart = rightWheel;
        _context.SaveChanges();
        return body;
    }

    private BodyProto CreateTrackedRobotBody()
    {
        BodyProto source = _context.BodyProtos.First(x => x.Name == "Robot Humanoid");
        BodyProto body = CreateInheritedBody("Tracked Robot", source);
        AddBodypartRemoval(body, HumanoidLowerLegRemovalAliases);
        BodypartProto rightTrack = AddBodypart(body, "rtrack", "right track pod", "Track", BodypartTypeEnum.Standing, "rshin",
            Alignment.Right, Orientation.Lowest, 80, 90, 120, 1000, _chassisAlloy, _robotPlatingArmour, SizeCategory.Small,
            countAs: FindBodypartOnBody(source, "rfoot"));
        BodypartProto leftTrack = AddBodypart(body, "ltrack", "left track pod", "Track", BodypartTypeEnum.Standing, "lshin",
            Alignment.Left, Orientation.Lowest, 80, 90, 120, 1001, _chassisAlloy, _robotPlatingArmour, SizeCategory.Small,
            countAs: FindBodypartOnBody(source, "lfoot"));
        AddLimbPart(body, "rhip", "rtrack");
        AddLimbPart(body, "lhip", "ltrack");
        body.LegDescriptionSingular = "track pod";
        body.LegDescriptionPlural = "track pods";
        body.DefaultSmashingBodypart = rightTrack;
        _context.SaveChanges();
        return body;
    }

    private BodyProto CreateCyborgHumanoidBody()
    {
        BodyProto source = _context.BodyProtos.First(x => x.Name == "Robot Humanoid");
        return CreateInheritedBody("Cyborg Humanoid", source);
    }

    private BodyProto CreateRoombaBody()
    {
        BodyProto source = _context.BodyProtos.First(x => x.Name == "Robot Utility");
        return CloneBody("Roomba Robot", source);
    }

    private BodyProto CreateTrackedUtilityBody()
    {
        BodyProto source = _context.BodyProtos.First(x => x.Name == "Robot Utility");
        BodyProto body = CloneBody("Tracked Utility Robot", source);
        SeederBodyUtilities.RemoveBodyparts(_context, body, ["rdrivewheel", "ldrivewheel"]);
        BodypartProto rightTrack = AddBodypart(body, "rtrack", "right compact track", "Track", BodypartTypeEnum.Standing, "chassis",
            Alignment.Right, Orientation.Lowest, 85, 70, 110, 1000, _chassisAlloy, _robotLightPlatingArmour, SizeCategory.Small);
        BodypartProto leftTrack = AddBodypart(body, "ltrack", "left compact track", "Track", BodypartTypeEnum.Standing, "chassis",
            Alignment.Left, Orientation.Lowest, 85, 70, 110, 1001, _chassisAlloy, _robotLightPlatingArmour, SizeCategory.Small);
        AddLimb(body, "Right Track", LimbType.Leg, rightTrack, [rightTrack]);
        AddLimb(body, "Left Track", LimbType.Leg, leftTrack, [leftTrack]);
        body.DefaultSmashingBodypart = rightTrack;
        _context.SaveChanges();
        return body;
    }

    private BodyProto CreateRobotDogBody()
    {
        BodyProto source = _context.BodyProtos.First(x => x.Name == "Robot Quadruped");
        return CloneBody("Robot Dog", source);
    }

    private BodyProto CreateRobotCockroachBody()
    {
        BodyProto source = _context.BodyProtos.First(x => x.Name == "Robot Insectoid");
        return CloneBody("Robot Cockroach", source);
    }

    private BodyProto CreateHandAttachmentVariant(string bodyName, string rightAlias, string leftAlias,
        string attachmentDescription, string shapeName)
    {
        BodyProto source = _context.BodyProtos.First(x => x.Name == "Robot Humanoid");
        BodyProto body = CreateInheritedBody(bodyName, source);
        AddBodypartRemoval(body, HumanoidHandRemovalAliases);
        AddBodypart(body, rightAlias, $"right {attachmentDescription}", shapeName, BodypartTypeEnum.Wear, "rwrist",
            Alignment.Right, Orientation.Appendage, 45, 45, 60, 1000, _chassisAlloy, _robotPlatingArmour,
            SizeCategory.Small);
        AddBodypart(body, leftAlias, $"left {attachmentDescription}", shapeName, BodypartTypeEnum.Wear, "lwrist",
            Alignment.Left, Orientation.Appendage, 45, 45, 60, 1001, _chassisAlloy, _robotPlatingArmour,
            SizeCategory.Small);
        AddLimbPart(body, "rupperarm", rightAlias);
        AddLimbPart(body, "lupperarm", leftAlias);
        _context.SaveChanges();
        return body;
    }

    private void AddRobotHumanoidOrgans(BodyProto body)
    {
        AddRobotOrgan(body, "positronicbrain", "positronic brain", "Processor", BodypartTypeEnum.PositronicBrain,
            "bhead", Alignment.Irrelevant, Orientation.Centre, SizeCategory.Small, true, true,
            ["bhead", "face", "reye", "leye", "rear", "lear"]);
        AddRobotOrgan(body, "sensorarray", "sensor array", "Sensor Cluster", BodypartTypeEnum.SensorArray,
            "bhead", Alignment.Irrelevant, Orientation.Centre, SizeCategory.Small, false, false,
            ["bhead", "face", "reye", "leye", "rear", "lear"]);
        AddRobotOrgan(body, "speechsynth", "speech synthesizer", "Processor", BodypartTypeEnum.SpeechSynthesizer,
            "throat", Alignment.Irrelevant, Orientation.Centre, SizeCategory.Small, false, false,
            ["throat", "mouth"]);
        AddRobotOrgan(body, "powercore", "power core", "Power Core", BodypartTypeEnum.PowerCore,
            "abdomen", Alignment.Irrelevant, Orientation.Centre, SizeCategory.Small, true, true,
            ["abdomen", "belly", "uback", "lback"]);
    }

    private void AddQuadrupedRobotOrgans(BodyProto body)
    {
        AddRobotOrgan(body, "positronicbrain", "positronic brain", "Processor", BodypartTypeEnum.PositronicBrain,
            "head", Alignment.Irrelevant, Orientation.Centre, SizeCategory.Small, true, true,
            ["head", "muzzle", "reye", "leye", "rear", "lear"]);
        AddRobotOrgan(body, "sensorarray", "sensor array", "Sensor Cluster", BodypartTypeEnum.SensorArray,
            "head", Alignment.Irrelevant, Orientation.Centre, SizeCategory.Small, false, false,
            ["head", "muzzle", "reye", "leye", "rear", "lear"]);
        AddRobotOrgan(body, "powercore", "power core", "Power Core", BodypartTypeEnum.PowerCore,
            "abdomen", Alignment.Irrelevant, Orientation.Centre, SizeCategory.Small, true, true,
            ["abdomen", "belly", "uback", "lback"]);
    }

    private void AddInsectoidRobotOrgans(BodyProto body)
    {
        AddRobotOrgan(body, "positronicbrain", "positronic brain", "Processor", BodypartTypeEnum.PositronicBrain,
            "head", Alignment.Irrelevant, Orientation.Centre, SizeCategory.Small, true, true,
            ["head", "reye", "leye", "rantenna", "lantenna"]);
        AddRobotOrgan(body, "sensorarray", "sensor array", "Sensor Cluster", BodypartTypeEnum.SensorArray,
            "head", Alignment.Irrelevant, Orientation.Centre, SizeCategory.Small, false, false,
            ["head", "reye", "leye", "rantenna", "lantenna"]);
        AddRobotOrgan(body, "powercore", "power core", "Power Core", BodypartTypeEnum.PowerCore,
            "thorax", Alignment.Irrelevant, Orientation.Centre, SizeCategory.Small, true, true,
            ["thorax", "abdomen"]);
    }
}
