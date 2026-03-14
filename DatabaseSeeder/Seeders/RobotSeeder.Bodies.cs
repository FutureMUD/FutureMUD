#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
	private static readonly string[] HumanoidRobotOrganAliases = ["powercore", "positronicbrain", "sensorarray", "speechsynth"];
	private static readonly string[] NonSpeakingRobotOrganAliases = ["powercore", "positronicbrain", "sensorarray"];

	private Dictionary<string, BodyProto> EnsureBodyCatalogue(RobotSeedSummary summary)
	{
		var bodies = new Dictionary<string, BodyProto>(StringComparer.OrdinalIgnoreCase)
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
		var existing = _context.BodyProtos.FirstOrDefault(x => x.Name == name);
		if (existing is not null)
		{
			return existing;
		}

		var created = factory();
		summary.BodiesAdded++;
		return created;
	}

	private BodyProto CreateRobotHumanoidBody()
	{
		var body = CloneBody("Robot Humanoid", _humanoidBody, _humanoidBody);
		ConfigureRobotBodyMaterials(body, false);
		AddRobotHumanoidOrgans(body);
		EnsureDefaultSmashingBodypart(body, "rhand");
		return body;
	}

	private BodyProto CreateRobotQuadrupedBody()
	{
		var body = CloneBody("Robot Quadruped", _toedQuadrupedBody, _toedQuadrupedBody);
		RemoveOrganicInternals(body);
		ConfigureRobotBodyMaterials(body, true);
		AddQuadrupedRobotOrgans(body);
		EnsureDefaultSmashingBodypart(body, "rfpaw");
		return body;
	}

	private BodyProto CreateRobotInsectoidBody()
	{
		var body = CloneBody("Robot Insectoid", _insectoidBody, _insectoidBody);
		RemoveOrganicInternals(body);
		ConfigureRobotBodyMaterials(body, true);
		AddInsectoidRobotOrgans(body);
		EnsureDefaultSmashingBodypart(body, "mandibles");
		return body;
	}

	private BodyProto CreateRobotUtilityBody()
	{
		var body = new BodyProto
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

		var chassis = AddBodypart(body, "chassis", "service chassis", "Chassis", BodypartTypeEnum.Wear, null,
			Alignment.Irrelevant, Orientation.Centre, 220, 120, 220, 1, _chassisAlloy, _robotLightPlatingArmour,
			SizeCategory.Small, significant: true, damageModifier: 0.8, stunModifier: 0.8);
		var sensorPod = AddBodypart(body, "sensorpod", "sensor pod", "Chassis", BodypartTypeEnum.Wear, "chassis",
			Alignment.Front, Orientation.High, 100, 90, 120, 2, _chassisAlloy, _robotLightPlatingArmour,
			SizeCategory.Small, significant: true);
		var rightEye = AddBodypart(body, "reye", "right optical lens", "eye", BodypartTypeEnum.Eye, "sensorpod",
			Alignment.Right, Orientation.High, 35, 15, 20, 3, _chassisAlloy, _robotLightPlatingArmour, SizeCategory.Tiny);
		var leftEye = AddBodypart(body, "leye", "left optical lens", "eye", BodypartTypeEnum.Eye, "sensorpod",
			Alignment.Left, Orientation.High, 35, 15, 20, 4, _chassisAlloy, _robotLightPlatingArmour, SizeCategory.Tiny);
		var rightWheel = AddBodypart(body, "rdrivewheel", "right drive wheel", "Wheel", BodypartTypeEnum.Standing, "chassis",
			Alignment.Right, Orientation.Lowest, 90, 75, 110, 5, _chassisAlloy, _robotLightPlatingArmour, SizeCategory.Small);
		var leftWheel = AddBodypart(body, "ldrivewheel", "left drive wheel", "Wheel", BodypartTypeEnum.Standing, "chassis",
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

	private BodyProto CloneBody(string newName, BodyProto source, BodyProto? countsAs)
	{
		var body = new BodyProto
		{
			Name = newName,
			CountsAs = countsAs,
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

		SeederBodyUtilities.CloneBodyDefinition(_context, source, body);
		SeederBodyUtilities.CloneBodyPositionsAndSpeeds(_context, source, body);
		ApplyAliasCountAs(body, source);
		CopyDefaultSmashingBodypart(body, source);
		return body;
	}

	private void ApplyAliasCountAs(BodyProto targetBody, BodyProto sourceBody)
	{
		var sourceParts = _context.BodypartProtos
			.Where(x => x.BodyId == sourceBody.Id)
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		foreach (var part in _context.BodypartProtos.Where(x => x.BodyId == targetBody.Id).ToList())
		{
			if (part.CountAsId.HasValue)
			{
				continue;
			}

			if (!sourceParts.TryGetValue(part.Name, out var sourcePart))
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

		var sourceAlias = _context.BodypartProtos.FirstOrDefault(x => x.Id == sourceBody.DefaultSmashingBodypartId.Value)?.Name;
		if (sourceAlias is null)
		{
			return;
		}

		var targetPart = FindBodypartOnBody(targetBody, sourceAlias);
		if (targetPart is null)
		{
			return;
		}

		targetBody.DefaultSmashingBodypart = targetPart;
		_context.SaveChanges();
	}

	private void RemoveOrganicInternals(BodyProto body)
	{
		var aliases = _context.BodypartProtos
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
		foreach (var part in _context.BodypartProtos.Where(x => x.BodyId == body.Id).ToList())
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
}
