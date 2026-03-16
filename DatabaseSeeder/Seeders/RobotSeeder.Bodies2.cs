#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
	private BodyProto CreateSpiderCrawlerBody()
	{
		var source = _context.BodyProtos.First(x => x.Name == "Robot Humanoid");
		var body = CreateInheritedBody("Spider Crawler Robot", source, minimumLegsToStand: 4);
		AddBodypartRemoval(body, HumanoidFullLegRemovalAliases);
		foreach (var alias in new[] { "rleg1", "lleg1", "rleg2", "lleg2", "rleg3", "lleg3", "rleg4", "lleg4" })
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
		var avianBody = _avianBody ?? throw new InvalidOperationException("Winged Robot requires the Avian body.");
		var source = _context.BodyProtos.First(x => x.Name == "Robot Humanoid");
		var body = CreateInheritedBody("Winged Robot", source, minimumWingsToFly: 2);
		SeederBodyUtilities.CloneBodypartSubtree(_context, avianBody, body, "rwingbase", "uback");
		SeederBodyUtilities.CloneBodypartSubtree(_context, avianBody, body, "lwingbase", "uback");
		AddMissingBodyMovement(avianBody, body);
		ConfigureRobotBodyMaterials(body, false);
		return body;
	}

	private BodyProto CreateJetRobotBody()
	{
		var avianBody = _avianBody ?? throw new InvalidOperationException("Jet Robot requires the Avian body.");
		var source = _context.BodyProtos.First(x => x.Name == "Robot Humanoid");
		var body = CreateInheritedBody("Jet Robot", source, minimumWingsToFly: 2);
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
		var source = _context.BodyProtos.First(x => x.Name == "Robot Humanoid");
		var body = CreateInheritedBody("Mandible Robot", source);
		SeederBodyUtilities.CloneBodypartSubtree(_context, _insectoidBody, body, "mandibles", "mouth");
		AddLimbPart(body, "neck", "mandibles");
		ConfigureRobotBodyMaterials(body, false);
		return body;
	}

	private BodyProto CreateWheeledRobotBody()
	{
		var source = _context.BodyProtos.First(x => x.Name == "Robot Humanoid");
		var body = CreateInheritedBody("Wheeled Robot", source);
		AddBodypartRemoval(body, HumanoidLowerLegRemovalAliases);
		var rightWheel = AddBodypart(body, "rwheel", "right wheel assembly", "Wheel", BodypartTypeEnum.Standing, "rshin",
			Alignment.Right, Orientation.Lowest, 80, 80, 100, 1000, _chassisAlloy, _robotPlatingArmour, SizeCategory.Small,
			countAs: FindBodypartOnBody(source, "rfoot"));
		var leftWheel = AddBodypart(body, "lwheel", "left wheel assembly", "Wheel", BodypartTypeEnum.Standing, "lshin",
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
		var source = _context.BodyProtos.First(x => x.Name == "Robot Humanoid");
		var body = CreateInheritedBody("Tracked Robot", source);
		AddBodypartRemoval(body, HumanoidLowerLegRemovalAliases);
		var rightTrack = AddBodypart(body, "rtrack", "right track pod", "Track", BodypartTypeEnum.Standing, "rshin",
			Alignment.Right, Orientation.Lowest, 80, 90, 120, 1000, _chassisAlloy, _robotPlatingArmour, SizeCategory.Small,
			countAs: FindBodypartOnBody(source, "rfoot"));
		var leftTrack = AddBodypart(body, "ltrack", "left track pod", "Track", BodypartTypeEnum.Standing, "lshin",
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
		var source = _context.BodyProtos.First(x => x.Name == "Robot Humanoid");
		return CreateInheritedBody("Cyborg Humanoid", source);
	}

	private BodyProto CreateRoombaBody()
	{
		var source = _context.BodyProtos.First(x => x.Name == "Robot Utility");
		return CloneBody("Roomba Robot", source);
	}

	private BodyProto CreateTrackedUtilityBody()
	{
		var source = _context.BodyProtos.First(x => x.Name == "Robot Utility");
		var body = CloneBody("Tracked Utility Robot", source);
		SeederBodyUtilities.RemoveBodyparts(_context, body, ["rdrivewheel", "ldrivewheel"]);
		var rightTrack = AddBodypart(body, "rtrack", "right compact track", "Track", BodypartTypeEnum.Standing, "chassis",
			Alignment.Right, Orientation.Lowest, 85, 70, 110, 1000, _chassisAlloy, _robotLightPlatingArmour, SizeCategory.Small);
		var leftTrack = AddBodypart(body, "ltrack", "left compact track", "Track", BodypartTypeEnum.Standing, "chassis",
			Alignment.Left, Orientation.Lowest, 85, 70, 110, 1001, _chassisAlloy, _robotLightPlatingArmour, SizeCategory.Small);
		AddLimb(body, "Right Track", LimbType.Leg, rightTrack, [rightTrack]);
		AddLimb(body, "Left Track", LimbType.Leg, leftTrack, [leftTrack]);
		body.DefaultSmashingBodypart = rightTrack;
		_context.SaveChanges();
		return body;
	}

	private BodyProto CreateRobotDogBody()
	{
		var source = _context.BodyProtos.First(x => x.Name == "Robot Quadruped");
		return CloneBody("Robot Dog", source);
	}

	private BodyProto CreateRobotCockroachBody()
	{
		var source = _context.BodyProtos.First(x => x.Name == "Robot Insectoid");
		return CloneBody("Robot Cockroach", source);
	}

	private BodyProto CreateHandAttachmentVariant(string bodyName, string rightAlias, string leftAlias,
		string attachmentDescription, string shapeName)
	{
		var source = _context.BodyProtos.First(x => x.Name == "Robot Humanoid");
		var body = CreateInheritedBody(bodyName, source);
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
