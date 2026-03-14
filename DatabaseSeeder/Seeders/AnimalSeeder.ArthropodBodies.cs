#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	private void SeedInsectoidBody(BodyProto insectProto, bool winged)
	{
		ResetCachedParts();
		var order = 1;
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Bodyparts...");

		AddBodypart(insectProto, "thorax", "thorax", "Insect Thorax", BodypartTypeEnum.Wear, null,
			Alignment.Front, Orientation.Centre, 50, -1, 100, order++, "chitin", SizeCategory.Small, "Torso", true,
			isVital: true, implantSpace: 2, stunMultiplier: 0.2);
		AddBodypart(insectProto, "head", "head", "head", BodypartTypeEnum.BonyDrapeable, "thorax",
			Alignment.Front, Orientation.High, 40, -1, 80, order++, "chitin", SizeCategory.Tiny, "Head", true,
			isVital: true, implantSpace: 1, stunMultiplier: 1.0);
		AddBodypart(insectProto, "abdomen", "abdomen", "Insect Abdomen", BodypartTypeEnum.Wear, "thorax",
			Alignment.Rear, Orientation.Low, 50, -1, 90, order++, "chitin", SizeCategory.Small, "Torso", true,
			isVital: true, implantSpace: 1, stunMultiplier: 0.2);
		AddBodypart(insectProto, "rantenna", "right antenna", "Antenna", BodypartTypeEnum.Wear, "head",
			Alignment.FrontRight, Orientation.Highest, 5, -1, 10, order++, "chitin", SizeCategory.Tiny, "Head", false);
		AddBodypart(insectProto, "lantenna", "left antenna", "Antenna", BodypartTypeEnum.Wear, "head",
			Alignment.FrontLeft, Orientation.Highest, 5, -1, 10, order++, "chitin", SizeCategory.Tiny, "Head", false);
		AddBodypart(insectProto, "mandibles", "mandibles", "Mandible", BodypartTypeEnum.Mouth, "head",
			Alignment.Front, Orientation.High, 10, -1, 10, order++, "chitin", SizeCategory.Tiny, "Head", false);
		AddBodypart(insectProto, "reye", "right eye", "Compound Eye", BodypartTypeEnum.Eye, "head",
			Alignment.FrontRight, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head", true,
			isVital: true);
		AddBodypart(insectProto, "leye", "left eye", "Compound Eye", BodypartTypeEnum.Eye, "head",
			Alignment.FrontLeft, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head", true,
			isVital: true);

		for (var i = 1; i <= 3; i++)
		{
			AddBodypart(insectProto, $"rleg{i}", $"right leg {i}", "Upper Leg", BodypartTypeEnum.Standing,
				"thorax", Alignment.Right, Orientation.Low, 25, -1, 20, order++, "chitin", SizeCategory.Small,
				$"Right Leg {i}", true);
			AddBodypart(insectProto, $"lleg{i}", $"left leg {i}", "Upper Leg", BodypartTypeEnum.Standing,
				"thorax", Alignment.Left, Orientation.Low, 25, -1, 20, order++, "chitin", SizeCategory.Small,
				$"Left Leg {i}", true);
		}

		if (winged)
		{
			AddBodypart(insectProto, "rwingbase", "right wing base", "Wing Base", BodypartTypeEnum.BonyDrapeable,
				"thorax", Alignment.FrontRight, Orientation.High, 20, -1, 30, order++, "chitin", SizeCategory.Small,
				"Right Wing");
			AddBodypart(insectProto, "lwingbase", "left wing base", "Wing Base", BodypartTypeEnum.BonyDrapeable,
				"thorax", Alignment.FrontLeft, Orientation.High, 20, -1, 30, order++, "chitin", SizeCategory.Small,
				"Left Wing");
			AddBodypart(insectProto, "rwing", "right wing", "Wing", BodypartTypeEnum.Wing, "rwingbase",
				Alignment.FrontRight, Orientation.High, 20, 30, 80, order++, "chitin", SizeCategory.Small, "Right Wing");
			AddBodypart(insectProto, "lwing", "left wing", "Wing", BodypartTypeEnum.Wing, "lwingbase",
				Alignment.FrontLeft, Orientation.High, 20, 30, 80, order++, "chitin", SizeCategory.Small, "Left Wing");
			AddBodypart(insectProto, "stinger", "stinger", "Stinger", BodypartTypeEnum.Wear, "abdomen",
				Alignment.Rear, Orientation.Lowest, 10, 15, 20, order++, "chitin", SizeCategory.Tiny, "Tail", false,
				isCore: false);
		}

		_context.SaveChanges();

		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Organs...");
		AddOrgan(insectProto, "brain", "brain", BodypartTypeEnum.Brain, 0.5, 15, 0.0, 0.2, 0.1, stunModifier: 1.0);
		AddOrgan(insectProto, "heart", "heart", BodypartTypeEnum.Heart, 0.4, 15, 0.0, 0.3, 0.3);
		AddOrgan(insectProto, "gut", "gut", BodypartTypeEnum.Intestines, 0.8, 15, 0.0, 0.3, 0.2);
		AddOrgan(insectProto, "spiracles", "spiracles", BodypartTypeEnum.Trachea, 0.5, 15, 0.0, 0.3, 0.2);

		AddOrganCoverage("brain", "head", 80, true);
		AddOrganCoverage("brain", "reye", 30);
		AddOrganCoverage("brain", "leye", 30);
		AddOrganCoverage("heart", "thorax", 60, true);
		AddOrganCoverage("gut", "abdomen", 75, true);
		AddOrganCoverage("spiracles", "thorax", 40, true);
		AddOrganCoverage("spiracles", "abdomen", 40);
		_context.SaveChanges();

		foreach (var (child, parent) in _cachedBodypartUpstreams)
		{
			_context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				Child = child.Id,
				Parent = parent.Id
			});
		}

		var limbs = new Dictionary<string, Limb>(StringComparer.OrdinalIgnoreCase);

		void AddLimb(string name, LimbType type, string rootPart, double damageThreshold = 0.4, double painThreshold = 0.4)
		{
			var limb = new Limb
			{
				Name = name,
				LimbType = (int)type,
				RootBody = insectProto,
				RootBodypart = _cachedBodyparts[rootPart],
				LimbDamageThresholdMultiplier = damageThreshold,
				LimbPainThresholdMultiplier = painThreshold
			};
			_context.Limbs.Add(limb);
			limbs[name] = limb;
		}

		AddLimb("Torso", LimbType.Torso, "thorax", 1.0, 1.0);
		AddLimb("Head", LimbType.Head, "head", 0.6, 0.6);
		for (var i = 1; i <= 3; i++)
		{
			AddLimb($"Right Leg {i}", LimbType.Leg, $"rleg{i}");
			AddLimb($"Left Leg {i}", LimbType.Leg, $"lleg{i}");
		}

		if (winged)
		{
			AddLimb("Right Wing", LimbType.Wing, "rwingbase");
			AddLimb("Left Wing", LimbType.Wing, "lwingbase");
			AddLimb("Tail", LimbType.Appendage, "stinger", 0.2, 0.2);
		}

		_context.SaveChanges();

		foreach (var limb in limbs.Values)
		{
			foreach (var part in _cachedLimbs[limb.Name])
			{
				_context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });
			}
		}

		AddBodypartGroupDescriberShape(insectProto, "body", "The whole body of an insect",
			("Insect Thorax", 1, 1),
			("Insect Abdomen", 1, 1),
			("head", 0, 1)
		);
		AddBodypartGroupDescriberShape(insectProto, "legs", "The legs of an insect",
			("Upper Leg", 6, 6)
		);
		AddBodypartGroupDescriberShape(insectProto, "head", "An insect head",
			("head", 1, 1),
			("Compound Eye", 0, 2),
			("Antenna", 0, 2),
			("Mandible", 0, 1)
		);
		AddBodypartGroupDescriberShape(insectProto, "eyes", "The eyes of an insect",
			("Compound Eye", 0, 2)
		);
		AddBodypartGroupDescriberShape(insectProto, "antennae", "The antennae of an insect",
			("Antenna", 2, 2)
		);

		if (winged)
		{
			AddBodypartGroupDescriberShape(insectProto, "wings", "The wings of an insect",
				("Wing Base", 2, 2),
				("Wing", 2, 2)
			);
			AddBodypartGroupDescriberDirect(insectProto, "stinger", "The stinger of an insect",
				("stinger", true)
			);
		}

		_context.SaveChanges();
		AuditBody(insectProto, winged ? "winged-insectoid" : "insectoid");
	}

	private void SeedDecapodBody(BodyProto crabProto)
	{
		ResetCachedParts();
		var order = 1;
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Crabs...");

		AddBodypart(crabProto, "carapace", "carapace", "Body", BodypartTypeEnum.Wear, null, Alignment.Front,
			Orientation.Centre, 60, -1, 100, order++, "chitin", SizeCategory.Small, "Torso", true, isVital: true,
			implantSpace: 2, stunMultiplier: 0.2);
		AddBodypart(crabProto, "underbelly", "soft underbelly", "belly", BodypartTypeEnum.Wear, "carapace",
			Alignment.Irrelevant, Orientation.Lowest, 30, -1, 45, order++, "Flesh", SizeCategory.Small, "Torso", true,
			isVital: true, stunMultiplier: 0.2);
		AddBodypart(crabProto, "mouth", "mouthparts", "Mandible", BodypartTypeEnum.Mouth, "carapace",
			Alignment.Front, Orientation.Low, 15, -1, 15, order++, "chitin", SizeCategory.Tiny, "Head", false);
		AddBodypart(crabProto, "reye", "right eye", "Compound Eye", BodypartTypeEnum.Eye, "carapace",
			Alignment.FrontRight, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head");
		AddBodypart(crabProto, "leye", "left eye", "Compound Eye", BodypartTypeEnum.Eye, "carapace",
			Alignment.FrontLeft, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head");
		AddBodypart(crabProto, "rantenna", "right antenna", "Antenna", BodypartTypeEnum.Wear, "carapace",
			Alignment.FrontRight, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head", false);
		AddBodypart(crabProto, "lantenna", "left antenna", "Antenna", BodypartTypeEnum.Wear, "carapace",
			Alignment.FrontLeft, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head", false);
		AddBodypart(crabProto, "rclaw", "right claw", "Claw", BodypartTypeEnum.Grabbing, "carapace",
			Alignment.FrontRight, Orientation.Centre, 35, 45, 25, order++, "chitin", SizeCategory.Small, "Right Claw");
		AddBodypart(crabProto, "lclaw", "left claw", "Claw", BodypartTypeEnum.Grabbing, "carapace",
			Alignment.FrontLeft, Orientation.Centre, 35, 45, 25, order++, "chitin", SizeCategory.Small, "Left Claw");

		for (var i = 1; i <= 4; i++)
		{
			AddBodypart(crabProto, $"rleg{i}", $"right leg {i}", "Upper Leg", BodypartTypeEnum.Standing, "carapace",
				Alignment.Right, Orientation.Low, 20, -1, 12, order++, "chitin", SizeCategory.Small, $"Right Leg {i}");
			AddBodypart(crabProto, $"lleg{i}", $"left leg {i}", "Upper Leg", BodypartTypeEnum.Standing, "carapace",
				Alignment.Left, Orientation.Low, 20, -1, 12, order++, "chitin", SizeCategory.Small, $"Left Leg {i}");
		}
		AddBodypart(crabProto, "gillcluster", "gill cluster", "gill", BodypartTypeEnum.Gill, "underbelly",
			Alignment.Irrelevant, Orientation.Low, 15, -1, 20, order++, "Flesh", SizeCategory.Small, "Torso", true,
			isVital: true, stunMultiplier: 1.0);

		_context.SaveChanges();

		AddOrgan(crabProto, "brain", "brain", BodypartTypeEnum.Brain, 0.4, 15, 0.0, 0.2, 0.1);
		AddOrgan(crabProto, "heart", "heart", BodypartTypeEnum.Heart, 0.4, 15, 0.0, 0.3, 0.3);
		AddOrgan(crabProto, "stomach", "stomach", BodypartTypeEnum.Stomach, 0.5, 15, 0.0, 0.3, 0.2);
		AddOrganCoverage("brain", "carapace", 30, true);
		AddOrganCoverage("brain", "reye", 20);
		AddOrganCoverage("brain", "leye", 20);
		AddOrganCoverage("heart", "underbelly", 40, true);
		AddOrganCoverage("heart", "carapace", 20);
		AddOrganCoverage("stomach", "underbelly", 45, true);
		AddOrganCoverage("stomach", "carapace", 15);
		_context.SaveChanges();

		foreach (var (child, parent) in _cachedBodypartUpstreams)
		{
			_context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				Child = child.Id,
				Parent = parent.Id
			});
		}

		var limbs = new Dictionary<string, Limb>(StringComparer.OrdinalIgnoreCase);

		void AddLimb(string name, LimbType type, string rootPart, double damageThreshold = 0.4, double painThreshold = 0.4)
		{
			var limb = new Limb
			{
				Name = name,
				LimbType = (int)type,
				RootBody = crabProto,
				RootBodypart = _cachedBodyparts[rootPart],
				LimbDamageThresholdMultiplier = damageThreshold,
				LimbPainThresholdMultiplier = painThreshold
			};
			_context.Limbs.Add(limb);
			limbs[name] = limb;
		}

		AddLimb("Torso", LimbType.Torso, "carapace", 1.0, 1.0);
		AddLimb("Head", LimbType.Head, "mouth", 0.5, 0.5);
		AddLimb("Right Claw", LimbType.Arm, "rclaw");
		AddLimb("Left Claw", LimbType.Arm, "lclaw");
		for (var i = 1; i <= 4; i++)
		{
			AddLimb($"Right Leg {i}", LimbType.Leg, $"rleg{i}");
			AddLimb($"Left Leg {i}", LimbType.Leg, $"lleg{i}");
		}

		_context.SaveChanges();

		foreach (var limb in limbs.Values)
		{
			foreach (var part in _cachedLimbs[limb.Name])
			{
				_context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });
			}
		}

		AddBodypartGroupDescriberShape(crabProto, "body", "The whole body of a crab",
			("Body", 1, 1),
			("belly", 0, 1),
			("gill", 0, 1),
			("Mandible", 0, 1)
		);
		AddBodypartGroupDescriberShape(crabProto, "claws", "The claws of a crab",
			("Claw", 2, 2)
		);
		AddBodypartGroupDescriberShape(crabProto, "legs", "The walking legs of a crab",
			("Upper Leg", 8, 8)
		);
		AddBodypartGroupDescriberShape(crabProto, "eyes", "The eyes of a crab",
			("Compound Eye", 0, 2)
		);
		AddBodypartGroupDescriberShape(crabProto, "antennae", "The antennae of a crab",
			("Antenna", 2, 2)
		);
		AddBodypartGroupDescriberDirect(crabProto, "underbelly", "The soft underbelly of a crab",
			("underbelly", true)
		);
		AddBodypartGroupDescriberDirect(crabProto, "gills", "The gill cluster of a crab",
			("gillcluster", true)
		);

		_context.SaveChanges();
		AuditBody(crabProto, "decapod");
	}

	private void SeedMalacostracanBody(BodyProto body)
	{
		ResetCachedParts();
		var order = 1;

		AddBodypart(body, "carapace", "carapace", "Body", BodypartTypeEnum.Wear, null, Alignment.Front,
			Orientation.Centre, 55, -1, 100, order++, "chitin", SizeCategory.Small, "Torso", true, isVital: true);
		AddBodypart(body, "abdomen", "abdomen", "Body", BodypartTypeEnum.Wear, "carapace", Alignment.Rear,
			Orientation.Low, 45, -1, 70, order++, "chitin", SizeCategory.Small, "Tail", true, isVital: true);
		AddBodypart(body, "underbelly", "soft underbelly", "belly", BodypartTypeEnum.Wear, "carapace",
			Alignment.Irrelevant, Orientation.Lowest, 25, -1, 35, order++, "Flesh", SizeCategory.Small, "Torso", true,
			isVital: true, stunMultiplier: 0.2);
		AddBodypart(body, "tailfan", "tail fan", "Tail", BodypartTypeEnum.Fin, "abdomen", Alignment.Rear,
			Orientation.Low, 20, 30, 20, order++, "chitin", SizeCategory.Small, "Tail");
		AddBodypart(body, "mouth", "mouthparts", "Mandible", BodypartTypeEnum.Mouth, "carapace",
			Alignment.Front, Orientation.Low, 10, -1, 10, order++, "chitin", SizeCategory.Tiny, "Head", false);
		AddBodypart(body, "reye", "right eye", "Compound Eye", BodypartTypeEnum.Eye, "carapace",
			Alignment.FrontRight, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head");
		AddBodypart(body, "leye", "left eye", "Compound Eye", BodypartTypeEnum.Eye, "carapace",
			Alignment.FrontLeft, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head");
		AddBodypart(body, "rantenna", "right antenna", "Antenna", BodypartTypeEnum.Wear, "carapace",
			Alignment.FrontRight, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head", false);
		AddBodypart(body, "lantenna", "left antenna", "Antenna", BodypartTypeEnum.Wear, "carapace",
			Alignment.FrontLeft, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head", false);
		AddBodypart(body, "rclaw", "right claw", "Claw", BodypartTypeEnum.Grabbing, "carapace",
			Alignment.FrontRight, Orientation.Centre, 25, 35, 25, order++, "chitin", SizeCategory.Small, "Right Claw");
		AddBodypart(body, "lclaw", "left claw", "Claw", BodypartTypeEnum.Grabbing, "carapace",
			Alignment.FrontLeft, Orientation.Centre, 25, 35, 25, order++, "chitin", SizeCategory.Small, "Left Claw");
		for (var i = 1; i <= 4; i++)
		{
			AddBodypart(body, $"rleg{i}", $"right leg {i}", "Upper Leg", BodypartTypeEnum.Standing, "carapace",
				Alignment.Right, Orientation.Low, 15, -1, 10, order++, "chitin", SizeCategory.Small, $"Right Leg {i}");
			AddBodypart(body, $"lleg{i}", $"left leg {i}", "Upper Leg", BodypartTypeEnum.Standing, "carapace",
				Alignment.Left, Orientation.Low, 15, -1, 10, order++, "chitin", SizeCategory.Small, $"Left Leg {i}");
		}
		AddBodypart(body, "gillcluster", "gill cluster", "gill", BodypartTypeEnum.Gill, "underbelly",
			Alignment.Irrelevant, Orientation.Low, 10, -1, 18, order++, "Flesh", SizeCategory.Small, "Torso", true,
			isVital: true, stunMultiplier: 1.0);

		_context.SaveChanges();

		AddOrgan(body, "brain", "brain", BodypartTypeEnum.Brain, 0.3, 10, 0.0, 0.2, 0.1);
		AddOrgan(body, "heart", "heart", BodypartTypeEnum.Heart, 0.3, 10, 0.0, 0.3, 0.3);
		AddOrgan(body, "stomach", "stomach", BodypartTypeEnum.Stomach, 0.4, 10, 0.0, 0.3, 0.2);
		AddOrganCoverage("brain", "carapace", 25, true);
		AddOrganCoverage("brain", "reye", 20);
		AddOrganCoverage("brain", "leye", 20);
		AddOrganCoverage("heart", "underbelly", 30, true);
		AddOrganCoverage("heart", "carapace", 15);
		AddOrganCoverage("stomach", "underbelly", 35, true);
		AddOrganCoverage("stomach", "abdomen", 20);
		_context.SaveChanges();

		foreach (var (child, parent) in _cachedBodypartUpstreams)
		{
			_context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				Child = child.Id,
				Parent = parent.Id
			});
		}

		var limbs = new Dictionary<string, Limb>(StringComparer.OrdinalIgnoreCase);

		void AddLimb(string name, LimbType type, string rootPart, double damageThreshold = 0.4, double painThreshold = 0.4)
		{
			var limb = new Limb
			{
				Name = name,
				LimbType = (int)type,
				RootBody = body,
				RootBodypart = _cachedBodyparts[rootPart],
				LimbDamageThresholdMultiplier = damageThreshold,
				LimbPainThresholdMultiplier = painThreshold
			};
			_context.Limbs.Add(limb);
			limbs[name] = limb;
		}

		AddLimb("Torso", LimbType.Torso, "carapace", 1.0, 1.0);
		AddLimb("Head", LimbType.Head, "mouth", 0.5, 0.5);
		AddLimb("Tail", LimbType.Appendage, "abdomen", 0.5, 0.5);
		AddLimb("Right Claw", LimbType.Arm, "rclaw");
		AddLimb("Left Claw", LimbType.Arm, "lclaw");
		for (var i = 1; i <= 4; i++)
		{
			AddLimb($"Right Leg {i}", LimbType.Leg, $"rleg{i}");
			AddLimb($"Left Leg {i}", LimbType.Leg, $"lleg{i}");
		}
		_context.SaveChanges();

		foreach (var limb in limbs.Values)
		{
			foreach (var part in _cachedLimbs[limb.Name])
			{
				_context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });
			}
		}

		AddBodypartGroupDescriberShape(body, "body", "The whole body of a crustacean",
			("Body", 1, 2),
			("belly", 0, 1),
			("gill", 0, 1),
			("Tail", 0, 1),
			("Mandible", 0, 1)
		);
		AddBodypartGroupDescriberShape(body, "claws", "The claws of a crustacean",
			("Claw", 2, 2)
		);
		AddBodypartGroupDescriberShape(body, "legs", "The walking legs of a crustacean",
			("Upper Leg", 8, 8)
		);
		AddBodypartGroupDescriberShape(body, "antennae", "The antennae of a crustacean",
			("Antenna", 2, 2)
		);
		AddBodypartGroupDescriberDirect(body, "underbelly", "The soft underbelly of a crustacean",
			("underbelly", true)
		);
		AddBodypartGroupDescriberDirect(body, "gills", "The gill cluster of a crustacean",
			("gillcluster", true)
		);

		_context.SaveChanges();
		AuditBody(body, "malacostracan");
	}

	private void SeedArachnidBody(BodyProto body, bool scorpion)
	{
		ResetCachedParts();
		var order = 1;

		AddBodypart(body, "cephalothorax", "cephalothorax", "Body", BodypartTypeEnum.Wear, null, Alignment.Front,
			Orientation.Centre, 45, -1, 100, order++, "chitin", SizeCategory.Small, "Torso", true, isVital: true);
		AddBodypart(body, "abdomen", "abdomen", "Body", BodypartTypeEnum.Wear, "cephalothorax", Alignment.Rear,
			Orientation.Low, 40, -1, 80, order++, "chitin", SizeCategory.Small, scorpion ? "Tail" : "Torso", true,
			isVital: true);
		AddBodypart(body, "reye", "right eye", "Compound Eye", BodypartTypeEnum.Eye, "cephalothorax",
			Alignment.FrontRight, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head");
		AddBodypart(body, "leye", "left eye", "Compound Eye", BodypartTypeEnum.Eye, "cephalothorax",
			Alignment.FrontLeft, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head");
		AddBodypart(body, "rfang", "right fang", "Fang", BodypartTypeEnum.Wear, "cephalothorax",
			Alignment.FrontRight, Orientation.Low, 10, 15, 8, order++, "chitin", SizeCategory.Tiny, "Head", false);
		AddBodypart(body, "lfang", "left fang", "Fang", BodypartTypeEnum.Wear, "cephalothorax",
			Alignment.FrontLeft, Orientation.Low, 10, 15, 8, order++, "chitin", SizeCategory.Tiny, "Head", false);
		AddBodypart(body, "rclaw", "right pedipalp", "Claw", BodypartTypeEnum.Grabbing, "cephalothorax",
			Alignment.FrontRight, Orientation.Centre, 15, 20, 12, order++, "chitin", SizeCategory.Small, "Right Claw");
		AddBodypart(body, "lclaw", "left pedipalp", "Claw", BodypartTypeEnum.Grabbing, "cephalothorax",
			Alignment.FrontLeft, Orientation.Centre, 15, 20, 12, order++, "chitin", SizeCategory.Small, "Left Claw");
		for (var i = 1; i <= 4; i++)
		{
			AddBodypart(body, $"rleg{i}", $"right leg {i}", "Upper Leg", BodypartTypeEnum.Standing, "cephalothorax",
				Alignment.Right, Orientation.Low, 18, -1, 10, order++, "chitin", SizeCategory.Small, $"Right Leg {i}");
			AddBodypart(body, $"lleg{i}", $"left leg {i}", "Upper Leg", BodypartTypeEnum.Standing, "cephalothorax",
				Alignment.Left, Orientation.Low, 18, -1, 10, order++, "chitin", SizeCategory.Small, $"Left Leg {i}");
		}

		if (scorpion)
		{
			AddBodypart(body, "tail", "tail", "Tail", BodypartTypeEnum.Wear, "abdomen", Alignment.Rear,
				Orientation.High, 20, 30, 15, order++, "chitin", SizeCategory.Small, "Tail");
			AddBodypart(body, "stinger", "stinger", "Stinger", BodypartTypeEnum.Wear, "tail", Alignment.Rear,
				Orientation.Highest, 10, 15, 8, order++, "chitin", SizeCategory.Tiny, "Tail", false, isCore: false);
		}

		_context.SaveChanges();

		AddOrgan(body, "brain", "brain", BodypartTypeEnum.Brain, 0.4, 10, 0.0, 0.2, 0.1);
		AddOrgan(body, "heart", "heart", BodypartTypeEnum.Heart, 0.4, 10, 0.0, 0.3, 0.3);
		AddOrgan(body, "gut", "gut", BodypartTypeEnum.Intestines, 0.5, 10, 0.0, 0.3, 0.2);
		AddOrgan(body, "booklungs", "book lungs", BodypartTypeEnum.Lung, 0.5, 10, 0.0, 0.3, 0.2);

		AddOrganCoverage("brain", "cephalothorax", 40, true);
		AddOrganCoverage("brain", "reye", 20);
		AddOrganCoverage("brain", "leye", 20);
		AddOrganCoverage("heart", "abdomen", 35, true);
		AddOrganCoverage("gut", "abdomen", 45, true);
		AddOrganCoverage("booklungs", "abdomen", 40, true);
		_context.SaveChanges();

		foreach (var (child, parent) in _cachedBodypartUpstreams)
		{
			_context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				Child = child.Id,
				Parent = parent.Id
			});
		}

		var limbs = new Dictionary<string, Limb>(StringComparer.OrdinalIgnoreCase);

		void AddLimb(string name, LimbType type, string rootPart, double damageThreshold = 0.4, double painThreshold = 0.4)
		{
			var limb = new Limb
			{
				Name = name,
				LimbType = (int)type,
				RootBody = body,
				RootBodypart = _cachedBodyparts[rootPart],
				LimbDamageThresholdMultiplier = damageThreshold,
				LimbPainThresholdMultiplier = painThreshold
			};
			_context.Limbs.Add(limb);
			limbs[name] = limb;
		}

		AddLimb("Torso", LimbType.Torso, scorpion ? "cephalothorax" : "abdomen", 1.0, 1.0);
		AddLimb("Head", LimbType.Head, "cephalothorax", 0.6, 0.6);
		AddLimb("Right Claw", LimbType.Arm, "rclaw");
		AddLimb("Left Claw", LimbType.Arm, "lclaw");
		for (var i = 1; i <= 4; i++)
		{
			AddLimb($"Right Leg {i}", LimbType.Leg, $"rleg{i}");
			AddLimb($"Left Leg {i}", LimbType.Leg, $"lleg{i}");
		}

		if (scorpion)
		{
			AddLimb("Tail", LimbType.Appendage, "tail", 0.4, 0.4);
		}

		_context.SaveChanges();

		foreach (var limb in limbs.Values)
		{
			foreach (var part in _cachedLimbs[limb.Name])
			{
				_context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });
			}
		}

		AddBodypartGroupDescriberShape(body, "body", scorpion ? "The whole body of a scorpion" : "The whole body of an arachnid",
			("Body", 1, 2),
			("Tail", scorpion ? 1 : 0, scorpion ? 1 : 0)
		);
		AddBodypartGroupDescriberShape(body, "legs", scorpion ? "The walking legs of a scorpion" : "The walking legs of an arachnid",
			("Upper Leg", 8, 8)
		);
		AddBodypartGroupDescriberShape(body, "fangs", scorpion ? "The mouthparts of a scorpion" : "The fangs of an arachnid",
			("Fang", 2, 2)
		);
		AddBodypartGroupDescriberShape(body, "claws", scorpion ? "The claws of a scorpion" : "The pedipalps of an arachnid",
			("Claw", 2, 2)
		);
		AddBodypartGroupDescriberShape(body, "eyes", "The eyes of an arachnid",
			("Compound Eye", 0, 2)
		);

		if (scorpion)
		{
			AddBodypartGroupDescriberDirect(body, "tail", "The tail of a scorpion",
				("tail", true),
				("stinger", true)
			);
		}

		_context.SaveChanges();
		AuditBody(body, scorpion ? "scorpion" : "arachnid");
	}
}
