#nullable enable

using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	private void SeedBeetle(BodyProto insectProto, BodyProto beetleProto)
	{
		CloneBodyDefinition(insectProto, beetleProto, cloneAdditionalUsages: false);
		AuditBody(beetleProto, "beetle");
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");
		SeedAnimalRaces(GetInsectRaceTemplates().Where(x => x.BodyKey == "Beetle"),
			("Beetle", beetleProto));
	}

	private void SeedCentipede(BodyProto centipedeProto)
	{
		SeedCentipedeBody(centipedeProto);
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");
		SeedAnimalRaces(GetInsectRaceTemplates().Where(x => x.BodyKey == "Centipede"),
			("Centipede", centipedeProto));
	}

	private void SeedCentipedeBody(BodyProto body)
	{
		ResetCachedParts();
		int order = 1;
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Centipedes...");

		AddBodypart(body, "thorax", "thorax", "Insect Thorax", BodypartTypeEnum.Wear, null,
			Alignment.Front, Orientation.Centre, 45, -1, 100, order++, "chitin", SizeCategory.Small, "Torso", true,
			isVital: true, implantSpace: 2, stunMultiplier: 0.2);
		AddBodypart(body, "head", "head", "head", BodypartTypeEnum.BonyDrapeable, "thorax",
			Alignment.Front, Orientation.High, 35, -1, 70, order++, "chitin", SizeCategory.Tiny, "Head", true,
			isVital: true, implantSpace: 1, stunMultiplier: 1.0);
		AddBodypart(body, "midbody", "midbody", "Insect Abdomen", BodypartTypeEnum.Wear, "thorax",
			Alignment.Rear, Orientation.Low, 40, -1, 80, order++, "chitin", SizeCategory.Small, "Torso", true,
			isVital: true, implantSpace: 1, stunMultiplier: 0.2);
		AddBodypart(body, "hindbody", "hindbody", "Insect Abdomen", BodypartTypeEnum.Wear, "midbody",
			Alignment.Rear, Orientation.Low, 35, -1, 70, order++, "chitin", SizeCategory.Small, "Torso", true,
			isVital: true, implantSpace: 1, stunMultiplier: 0.2);
		AddBodypart(body, "tail", "tail", "Tail", BodypartTypeEnum.Wear, "hindbody",
			Alignment.Rear, Orientation.Lowest, 20, 25, 25, order++, "chitin", SizeCategory.Small, "Tail", false,
			isCore: false);
		AddBodypart(body, "rantenna", "right antenna", "Antenna", BodypartTypeEnum.Wear, "head",
			Alignment.FrontRight, Orientation.Highest, 5, -1, 10, order++, "chitin", SizeCategory.Tiny, "Head", false);
		AddBodypart(body, "lantenna", "left antenna", "Antenna", BodypartTypeEnum.Wear, "head",
			Alignment.FrontLeft, Orientation.Highest, 5, -1, 10, order++, "chitin", SizeCategory.Tiny, "Head", false);
		AddBodypart(body, "mandibles", "mandibles", "Mandible", BodypartTypeEnum.Mouth, "head",
			Alignment.Front, Orientation.High, 12, -1, 10, order++, "chitin", SizeCategory.Tiny, "Head", false);
		AddBodypart(body, "reye", "right eye", "Compound Eye", BodypartTypeEnum.Eye, "head",
			Alignment.FrontRight, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head", true,
			isVital: true);
		AddBodypart(body, "leye", "left eye", "Compound Eye", BodypartTypeEnum.Eye, "head",
			Alignment.FrontLeft, Orientation.High, 5, -1, 5, order++, "chitin", SizeCategory.Tiny, "Head", true,
			isVital: true);

		for (int i = 1; i <= 6; i++)
		{
			string parentAlias = i switch
			{
				<= 2 => "thorax",
				<= 4 => "midbody",
				_ => "hindbody"
			};
			AddBodypart(body, $"rleg{i}", $"right leg {i}", "Upper Leg", BodypartTypeEnum.Standing,
				parentAlias, Alignment.Right, Orientation.Low, 18, -1, 16, order++, "chitin", SizeCategory.Small,
				$"Right Leg {i}", true);
			AddBodypart(body, $"lleg{i}", $"left leg {i}", "Upper Leg", BodypartTypeEnum.Standing,
				parentAlias, Alignment.Left, Orientation.Low, 18, -1, 16, order++, "chitin", SizeCategory.Small,
				$"Left Leg {i}", true);
		}

		_context.SaveChanges();

		AddOrgan(body, "brain", "brain", BodypartTypeEnum.Brain, 0.5, 15, 0.0, 0.2, 0.1, stunModifier: 1.0);
		AddOrgan(body, "heart", "heart", BodypartTypeEnum.Heart, 0.4, 15, 0.0, 0.3, 0.3);
		AddOrgan(body, "gut", "gut", BodypartTypeEnum.Intestines, 0.8, 15, 0.0, 0.3, 0.2);
		AddOrgan(body, "spiracles", "spiracles", BodypartTypeEnum.Trachea, 0.5, 15, 0.0, 0.3, 0.2);

		AddOrganCoverage("brain", "head", 80, true);
		AddOrganCoverage("brain", "reye", 25);
		AddOrganCoverage("brain", "leye", 25);
		AddOrganCoverage("heart", "thorax", 45, true);
		AddOrganCoverage("gut", "midbody", 55, true);
		AddOrganCoverage("gut", "hindbody", 35);
		AddOrganCoverage("spiracles", "thorax", 25, true);
		AddOrganCoverage("spiracles", "midbody", 25);
		AddOrganCoverage("spiracles", "hindbody", 25);
		_context.SaveChanges();

		foreach ((BodypartProto? child, BodypartProto? parent) in _cachedBodypartUpstreams)
		{
			_context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				Child = child.Id,
				Parent = parent.Id
			});
		}

		Dictionary<string, Limb> limbs = new(System.StringComparer.OrdinalIgnoreCase);

		void AddLimb(string name, LimbType type, string rootPart, double damageThreshold = 0.4, double painThreshold = 0.4)
		{
			Limb limb = new()
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

		AddLimb("Torso", LimbType.Torso, "thorax", 1.0, 1.0);
		AddLimb("Head", LimbType.Head, "head", 0.6, 0.6);
		AddLimb("Tail", LimbType.Appendage, "tail", 0.3, 0.3);
		for (int i = 1; i <= 6; i++)
		{
			AddLimb($"Right Leg {i}", LimbType.Leg, $"rleg{i}");
			AddLimb($"Left Leg {i}", LimbType.Leg, $"lleg{i}");
		}

		_context.SaveChanges();

		foreach (Limb limb in limbs.Values)
		{
			foreach (BodypartProto part in _cachedLimbs[limb.Name])
			{
				_context.LimbsBodypartProto.Add(new LimbBodypartProto { BodypartProto = part, Limb = limb });
			}
		}

		AddBodypartGroupDescriberShape(body, "body", "The long body of a centipede",
			("Insect Thorax", 1, 1),
			("Insect Abdomen", 2, 2),
			("Tail", 0, 1),
			("head", 0, 1)
		);
		AddBodypartGroupDescriberShape(body, "legs", "The many legs of a centipede",
			("Upper Leg", 12, 12)
		);
		AddBodypartGroupDescriberShape(body, "head", "The head of a centipede",
			("head", 1, 1),
			("Compound Eye", 0, 2),
			("Antenna", 0, 2),
			("Mandible", 0, 1)
		);
		AddBodypartGroupDescriberShape(body, "eyes", "The eyes of a centipede",
			("Compound Eye", 0, 2)
		);
		AddBodypartGroupDescriberShape(body, "antennae", "The antennae of a centipede",
			("Antenna", 2, 2)
		);
		AddBodypartGroupDescriberDirect(body, "tail", "The tail of a centipede",
			("tail", true)
		);

		_context.SaveChanges();
		AuditBody(body, "centipede");
	}
}
