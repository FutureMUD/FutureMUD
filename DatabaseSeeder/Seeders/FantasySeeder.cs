using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.Form.Shape;
using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.Body.PartProtos;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Form.Characteristics;

namespace DatabaseSeeder.Seeders;
internal class FantasySeeder : IDatabaseSeeder
{
	#region Implementation of IDatabaseSeeder

	/// <inheritdoc />
	public IEnumerable<(string Id, string Question,
			Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
			Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions
			=> new List<(string, string, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>, Func<string, FuturemudDatabaseContext, (bool, string)>)>();

	/// <inheritdoc />
	public int SortOrder => 301;

	/// <inheritdoc />
	public string Name => "Fantasy Seeder";

	/// <inheritdoc />
	public string Tagline => "Install Fantasy Races";

	/// <inheritdoc />
	public string FullDescription => @"Adds a number of common fantasy races based on existing human and animal templates.";

	private BodypartShape GetOrCreateShape(FuturemudDatabaseContext context, string name)
	{
		var shape = context.BodypartShapes.FirstOrDefault(x => x.Name == name);
		if (shape is null)
		{
			shape = new BodypartShape { Name = name };
			context.BodypartShapes.Add(shape);
			context.SaveChanges();
		}

		return shape;
	}

	private BodypartProto AddBodypart(FuturemudDatabaseContext context, BodyProto body, string alias, string shape,
			BodypartTypeEnum type, Dictionary<string, BodypartProto> map, int order,
			Alignment alignment = Alignment.Irrelevant, Orientation orientation = Orientation.Irrelevant,
			string? parent = null)
	{
		var material = context.Materials.First(x => x.Name == "flesh");
		var part = new BodypartProto
		{
			Body = body,
			Name = alias
			.Replace("right ", "r")
			.Replace("left ", "l")
			.CollapseString()
			.ToLowerInvariant(),
			Description = alias,
			BodypartShape = GetOrCreateShape(context, shape),
			BodypartType = (int)type,
			Alignment = (int)alignment,
			Location = (int)orientation,
			MaxLife = 100,
			SeveredThreshold = -1,
			IsCore = true,
			IsVital = alias == "head" || alias == "torso",
			Significant = true,
			RelativeHitChance = 100,
			DisplayOrder = order,
			DefaultMaterial = material,
			Size = (int)SizeCategory.Normal,
			BleedModifier = 1.0,
			DamageModifier = 1.0,
			PainModifier = 1.0,
			StunModifier = 0.0
		};
		context.BodypartProtos.Add(part);
		context.SaveChanges();
		map[alias] = part;
		if (parent != null)
		{
			context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
			{
				Child = part.Id,
				Parent = map[parent].Id
			});
			context.SaveChanges();
		}

		return part;
	}

	private BodypartProto AddOrgan(FuturemudDatabaseContext context, BodyProto body, string alias,
			BodypartTypeEnum type, Dictionary<string, BodypartProto> map, int order, string parent)
	{
		return AddBodypart(context, body, alias, "Organ", type, map, order,
				Alignment.Irrelevant, Orientation.Irrelevant, parent);
	}

	private void AddLimb(FuturemudDatabaseContext context, BodyProto body, string name, LimbType type,
			string rootPart, Dictionary<string, BodypartProto> map, params string[] parts)
	{
		var limb = new Limb
		{
			Name = name,
			LimbType = (int)type,
			RootBody = body,
			RootBodypart = map[rootPart],
			LimbDamageThresholdMultiplier = 1.0,
			LimbPainThresholdMultiplier = 1.0
		};
		context.Limbs.Add(limb);
		context.SaveChanges();

		foreach (var part in parts.Distinct())
		{
			if (map.TryGetValue(part, out var bp))
				context.LimbsBodypartProto.Add(new LimbBodypartProto { Limb = limb, BodypartProto = bp });
		}
		context.SaveChanges();
	}

	private void AddRaceDescription(FuturemudDatabaseContext context, Race race, string description)
	{
		var prog = new FutureProg
		{
			FunctionName = $"Is{race.Name.CollapseString()}",
			FunctionComment = $"Determines whether a character is a {race.Name}",
			AcceptsAnyParameters = false,
			Category = "Character",
			Subcategory = "Descriptions",
			ReturnType = (long)ProgVariableTypes.Boolean,
			FunctionText = $"return @ch.Race == ToRace(\"{race.Name}\")"
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterType = (long)ProgVariableTypes.Toon
		});
		context.FutureProgs.Add(prog);

		context.EntityDescriptionPatterns.Add(new EntityDescriptionPattern
		{
			ApplicabilityProg = prog,
			Type = (int)EntityDescriptionType.FullDescription,
			RelativeWeight = 100,
			Pattern = description
		});
	}

	private void AddColourCharacteristic(FuturemudDatabaseContext context, Race race, Ethnicity ethnicity,
			string name, IEnumerable<string> colours)
	{
		var def = new CharacteristicDefinition
		{
			Name = name,
			Type = (int)CharacteristicType.Standard,
			Pattern = name.ToLowerInvariant().Replace(" ", string.Empty),
			Description = $"{name} for {race.Name}",
			ChargenDisplayType = (int)CharacterGenerationDisplayType.DisplayAll,
			Model = "standard",
			Definition = string.Empty
		};
		context.CharacteristicDefinitions.Add(def);

		var profile = new CharacteristicProfile
		{
			Name = $"All {name}",
			Definition = "<Profile/>",
			Description = $"All values for {name}",
			TargetDefinition = def,
			Type = "all"
		};
		context.CharacteristicProfiles.Add(profile);

		race.RacesAdditionalCharacteristics.Add(new RacesAdditionalCharacteristics
		{
			Race = race,
			CharacteristicDefinition = def,
			Usage = "base"
		});
		ethnicity.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics
		{
			Ethnicity = ethnicity,
			CharacteristicDefinition = def,
			CharacteristicProfile = profile
		});

		foreach (var colour in colours)
		{
			context.CharacteristicValues.Add(new CharacteristicValue
			{
				Definition = def,
				Name = colour,
				Value = colour
			});
		}
	}

	private void AddStandardOrgans(FuturemudDatabaseContext context, BodyProto body,
			Dictionary<string, BodypartProto> map, ref int order)
	{
		AddOrgan(context, body, "brain", BodypartTypeEnum.Brain, map, order++, "head");
		AddOrgan(context, body, "esophagus", BodypartTypeEnum.Esophagus, map, order++, "neck");
		AddOrgan(context, body, "trachea", BodypartTypeEnum.Trachea, map, order++, "neck");
		AddOrgan(context, body, "heart", BodypartTypeEnum.Heart, map, order++, "torso");
		AddOrgan(context, body, "liver", BodypartTypeEnum.Liver, map, order++, "torso");
		AddOrgan(context, body, "spleen", BodypartTypeEnum.Spleen, map, order++, "torso");
		AddOrgan(context, body, "stomach", BodypartTypeEnum.Stomach, map, order++, "torso");
		AddOrgan(context, body, "small intestines", BodypartTypeEnum.Intestines, map, order++, "torso");
		AddOrgan(context, body, "large intestines", BodypartTypeEnum.Intestines, map, order++, "torso");
		AddOrgan(context, body, "left kidney", BodypartTypeEnum.Kidney, map, order++, "torso");
		AddOrgan(context, body, "right kidney", BodypartTypeEnum.Kidney, map, order++, "torso");
		AddOrgan(context, body, "left lung", BodypartTypeEnum.Lung, map, order++, "torso");
		AddOrgan(context, body, "right lung", BodypartTypeEnum.Lung, map, order++, "torso");
		AddOrgan(context, body, "upper spine", BodypartTypeEnum.Spine, map, order++, "neck");
		AddOrgan(context, body, "middle spine", BodypartTypeEnum.Spine, map, order++, "torso");
		AddOrgan(context, body, "lower spine", BodypartTypeEnum.Spine, map, order++, "torso");
	}

	private void AddHeadFeatures(FuturemudDatabaseContext context, BodyProto body,
			Dictionary<string, BodypartProto> map, ref int order)
	{
		AddBodypart(context, body, "mouth", "Mouth", BodypartTypeEnum.Mouth, map, order++, Alignment.Front,
				Orientation.Highest, "head");
		AddBodypart(context, body, "tongue", "Tongue", BodypartTypeEnum.Tongue, map, order++, Alignment.Front,
				Orientation.Highest, "mouth");
		AddBodypart(context, body, "nose", "Nose", BodypartTypeEnum.Wear, map, order++, Alignment.Front,
				Orientation.Highest, "head");
		AddBodypart(context, body, "left eye", "Eye", BodypartTypeEnum.Eye, map, order++, Alignment.Left,
				Orientation.Highest, "head");
		AddBodypart(context, body, "right eye", "Eye", BodypartTypeEnum.Eye, map, order++, Alignment.Right,
				Orientation.Highest, "head");
		AddBodypart(context, body, "left ear", "Ear", BodypartTypeEnum.Ear, map, order++, Alignment.Left,
				Orientation.Highest, "head");
		AddBodypart(context, body, "right ear", "Ear", BodypartTypeEnum.Ear, map, order++, Alignment.Right,
				Orientation.Highest, "head");
	}

	/// <inheritdoc />
	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		context.Database.BeginTransaction();

		var humanoid = context.BodyProtos.First(x => x.Name == "Humanoid");
		var quadruped = context.BodyProtos.First(x => x.Name == "Quadruped Base");
		var ungulate = context.BodyProtos.First(x => x.Name == "Ungulate");
		var health = context.HealthStrategies.First(x => x.Name == "Non-Human HP");
		var corpse = context.CorpseModels.First(x => x.Name == "Organic Animal Corpse");
		var alwaysTrue = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
		var alwaysZero = context.FutureProgs.First(x => x.FunctionName == "AlwaysZero");

		var wearSize = humanoid.WearSizeParameter;

		var dragonBody = new BodyProto
		{
			Name = "Dragon",
			CountsAs = quadruped,
			WielderDescriptionSingle = "claw",
			WielderDescriptionPlural = "claws",
			StaminaRecoveryProgId = quadruped.StaminaRecoveryProgId,
			MinimumLegsToStand = 3,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(dragonBody);
		context.SaveChanges();

		var parts = new Dictionary<string, BodypartProto>();
		var order = 1;
		AddBodypart(context, dragonBody, "torso", "Body", BodypartTypeEnum.Wear, parts, order++);
		AddBodypart(context, dragonBody, "neck", "Neck", BodypartTypeEnum.Wear, parts, order++);
		AddBodypart(context, dragonBody, "head", "Head", BodypartTypeEnum.Wear, parts, order++, Alignment.Front, Orientation.Highest, "torso");
		AddBodypart(context, dragonBody, "left foreleg", "Upper Foreleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontLeft, Orientation.Low, "torso");
		AddBodypart(context, dragonBody, "right foreleg", "Upper Foreleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontRight, Orientation.Low, "torso");
		AddBodypart(context, dragonBody, "left hindleg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.RearLeft, Orientation.Low, "torso");
		AddBodypart(context, dragonBody, "right hindleg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.RearRight, Orientation.Low, "torso");
		AddBodypart(context, dragonBody, "left wingbase", "Wing", BodypartTypeEnum.Wear, parts, order++, Alignment.Left, Orientation.High, "torso");
		AddBodypart(context, dragonBody, "left wingbase", "Wing", BodypartTypeEnum.Wear, parts, order++, Alignment.Right, Orientation.High, "torso");
		AddBodypart(context, dragonBody, "left wing", "Wing", BodypartTypeEnum.Wing, parts, order++, Alignment.Left, Orientation.High, "torso");
		AddBodypart(context, dragonBody, "right wing", "Wing", BodypartTypeEnum.Wing, parts, order++, Alignment.Right, Orientation.High, "torso");
		AddBodypart(context, dragonBody, "tail", "Tail", BodypartTypeEnum.Wear, parts, order++, Alignment.Rear, Orientation.Low, "torso");
		AddHeadFeatures(context, dragonBody, parts, ref order);
		AddStandardOrgans(context, dragonBody, parts, ref order);
		AddLimb(context, dragonBody, "Torso", LimbType.Torso, "torso", parts,
				"torso", "heart", "liver", "spleen", "stomach", "small intestines", "large intestines",
				"left kidney", "right kidney", "left lung", "right lung", "tail");
		AddLimb(context, dragonBody, "Head", LimbType.Head, "head", parts,
				"head", "neck", "mouth", "tongue", "nose", "left eye", "right eye", "left ear", "right ear", "brain");
		AddLimb(context, dragonBody, "Left Foreleg", LimbType.Leg, "left foreleg", parts, "left foreleg");
		AddLimb(context, dragonBody, "Right Foreleg", LimbType.Leg, "right foreleg", parts, "right foreleg");
		AddLimb(context, dragonBody, "Left Hindleg", LimbType.Leg, "left hindleg", parts, "left hindleg");
		AddLimb(context, dragonBody, "Right Hindleg", LimbType.Leg, "right hindleg", parts, "right hindleg");
		AddLimb(context, dragonBody, "Left Wing", LimbType.Wing, "left wing", parts, "left wing");
		AddLimb(context, dragonBody, "Right Wing", LimbType.Wing, "right wing", parts, "right wing");
		AddLimb(context, dragonBody, "Tail", LimbType.Appendage, "tail", parts, "tail");

		var wyvernBody = new BodyProto
		{
			Name = "Wyvern",
			CountsAs = dragonBody,
			WielderDescriptionSingle = "claw",
			WielderDescriptionPlural = "claws",
			StaminaRecoveryProgId = quadruped.StaminaRecoveryProgId,
			MinimumLegsToStand = 2,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(wyvernBody);

		parts.Clear();
		order = 1;
		AddBodypart(context, wyvernBody, "torso", "Body", BodypartTypeEnum.Wear, parts, order++);
		AddBodypart(context, wyvernBody, "head", "Head", BodypartTypeEnum.Wear, parts, order++, Alignment.Front, Orientation.Highest, "torso");
		AddBodypart(context, wyvernBody, "left leg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontLeft, Orientation.Low, "torso");
		AddBodypart(context, wyvernBody, "right leg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontRight, Orientation.Low, "torso");
		AddBodypart(context, wyvernBody, "left wing", "Wing", BodypartTypeEnum.Wing, parts, order++, Alignment.Left, Orientation.High, "torso");
		AddBodypart(context, wyvernBody, "right wing", "Wing", BodypartTypeEnum.Wing, parts, order++, Alignment.Right, Orientation.High, "torso");
		AddBodypart(context, wyvernBody, "tail", "Tail", BodypartTypeEnum.Wear, parts, order++, Alignment.Rear, Orientation.Low, "torso");
		AddHeadFeatures(context, wyvernBody, parts, ref order);
		AddStandardOrgans(context, wyvernBody, parts, ref order);
		AddLimb(context, wyvernBody, "Torso", LimbType.Torso, "torso", parts,
				"torso", "heart", "liver", "spleen", "stomach", "small intestines", "large intestines",
				"left kidney", "right kidney", "left lung", "right lung", "tail");
		AddLimb(context, wyvernBody, "Head", LimbType.Head, "head", parts,
				"head", "mouth", "tongue", "nose", "left eye", "right eye", "left ear", "right ear", "brain");
		AddLimb(context, wyvernBody, "Left Leg", LimbType.Leg, "left leg", parts, "left leg");
		AddLimb(context, wyvernBody, "Right Leg", LimbType.Leg, "right leg", parts, "right leg");
		AddLimb(context, wyvernBody, "Left Wing", LimbType.Wing, "left wing", parts, "left wing");
		AddLimb(context, wyvernBody, "Right Wing", LimbType.Wing, "right wing", parts, "right wing");
		AddLimb(context, wyvernBody, "Tail", LimbType.Appendage, "tail", parts, "tail");

		var easternBody = new BodyProto
		{
			Name = "Eastern Dragon",
			CountsAs = dragonBody,
			WielderDescriptionSingle = "claw",
			WielderDescriptionPlural = "claws",
			StaminaRecoveryProgId = quadruped.StaminaRecoveryProgId,
			MinimumLegsToStand = 4,
			MinimumWingsToFly = 0,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(easternBody);

		parts.Clear();
		order = 1;
		AddBodypart(context, easternBody, "torso", "Body", BodypartTypeEnum.Wear, parts, order++);
		AddBodypart(context, easternBody, "head", "Head", BodypartTypeEnum.Wear, parts, order++, Alignment.Front, Orientation.Highest, "torso");
		AddBodypart(context, easternBody, "left foreleg", "Upper Foreleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontLeft, Orientation.Low, "torso");
		AddBodypart(context, easternBody, "right foreleg", "Upper Foreleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontRight, Orientation.Low, "torso");
		AddBodypart(context, easternBody, "left hindleg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.RearLeft, Orientation.Low, "torso");
		AddBodypart(context, easternBody, "right hindleg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.RearRight, Orientation.Low, "torso");
		AddBodypart(context, easternBody, "tail", "Tail", BodypartTypeEnum.Wear, parts, order++, Alignment.Rear, Orientation.Low, "torso");
		AddHeadFeatures(context, easternBody, parts, ref order);
		AddStandardOrgans(context, easternBody, parts, ref order);
		AddLimb(context, easternBody, "Torso", LimbType.Torso, "torso", parts,
				"torso", "heart", "liver", "spleen", "stomach", "small intestines", "large intestines",
				"left kidney", "right kidney", "left lung", "right lung", "tail");
		AddLimb(context, easternBody, "Head", LimbType.Head, "head", parts,
				"head", "mouth", "tongue", "nose", "left eye", "right eye", "left ear", "right ear", "brain");
		AddLimb(context, easternBody, "Left Foreleg", LimbType.Leg, "left foreleg", parts, "left foreleg");
		AddLimb(context, easternBody, "Right Foreleg", LimbType.Leg, "right foreleg", parts, "right foreleg");
		AddLimb(context, easternBody, "Left Hindleg", LimbType.Leg, "left hindleg", parts, "left hindleg");
		AddLimb(context, easternBody, "Right Hindleg", LimbType.Leg, "right hindleg", parts, "right hindleg");
		AddLimb(context, easternBody, "Tail", LimbType.Appendage, "tail", parts, "tail");

		var centaurBody = new BodyProto
		{
			Name = "Centaur",
			CountsAs = humanoid,
			WielderDescriptionSingle = "hand",
			WielderDescriptionPlural = "hands",
			StaminaRecoveryProgId = humanoid.StaminaRecoveryProgId,
			MinimumLegsToStand = 4,
			MinimumWingsToFly = 0,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(centaurBody);

		parts.Clear();
		order = 1;
		AddBodypart(context, centaurBody, "torso", "Body", BodypartTypeEnum.Wear, parts, order++);
		AddBodypart(context, centaurBody, "head", "Head", BodypartTypeEnum.Wear, parts, order++, Alignment.Front, Orientation.Highest, "torso");
		AddBodypart(context, centaurBody, "left arm", "Upper Arm", BodypartTypeEnum.Wear, parts, order++, Alignment.Left, Orientation.Low, "torso");
		AddBodypart(context, centaurBody, "right arm", "Upper Arm", BodypartTypeEnum.Wear, parts, order++, Alignment.Right, Orientation.Low, "torso");
		AddBodypart(context, centaurBody, "horse body", "Body", BodypartTypeEnum.Wear, parts, order++, Alignment.Rear, Orientation.Centre, "torso");
		AddBodypart(context, centaurBody, "left foreleg", "Upper Foreleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontLeft, Orientation.Low, "horse body");
		AddBodypart(context, centaurBody, "right foreleg", "Upper Foreleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontRight, Orientation.Low, "horse body");
		AddBodypart(context, centaurBody, "left hindleg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.RearLeft, Orientation.Low, "horse body");
		AddBodypart(context, centaurBody, "right hindleg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.RearRight, Orientation.Low, "horse body");
		AddHeadFeatures(context, centaurBody, parts, ref order);
		AddStandardOrgans(context, centaurBody, parts, ref order);
		AddLimb(context, centaurBody, "Human Torso", LimbType.Torso, "torso", parts,
				"torso", "head", "left arm", "right arm", "heart", "liver", "spleen", "stomach",
				"small intestines", "large intestines", "left kidney", "right kidney", "left lung", "right lung");
		AddLimb(context, centaurBody, "Head", LimbType.Head, "head", parts,
				"head", "mouth", "tongue", "nose", "left eye", "right eye", "left ear", "right ear", "brain");
		AddLimb(context, centaurBody, "Left Arm", LimbType.Arm, "left arm", parts, "left arm");
		AddLimb(context, centaurBody, "Right Arm", LimbType.Arm, "right arm", parts, "right arm");
		AddLimb(context, centaurBody, "Left Foreleg", LimbType.Leg, "left foreleg", parts, "left foreleg");
		AddLimb(context, centaurBody, "Right Foreleg", LimbType.Leg, "right foreleg", parts, "right foreleg");
		AddLimb(context, centaurBody, "Left Hindleg", LimbType.Leg, "left hindleg", parts, "left hindleg");
		AddLimb(context, centaurBody, "Right Hindleg", LimbType.Leg, "right hindleg", parts, "right hindleg");

		var myconidBody = new BodyProto
		{
			Name = "Myconid",
			CountsAs = humanoid,
			WielderDescriptionSingle = "hand",
			WielderDescriptionPlural = "hands",
			StaminaRecoveryProgId = humanoid.StaminaRecoveryProgId,
			MinimumLegsToStand = 2,
			MinimumWingsToFly = 0,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(myconidBody);

		parts.Clear();
		order = 1;
		AddBodypart(context, myconidBody, "torso", "Body", BodypartTypeEnum.Wear, parts, order++);
		AddBodypart(context, myconidBody, "head", "Fungus Cap", BodypartTypeEnum.Wear, parts, order++, Alignment.Front, Orientation.Highest, "torso");
		AddBodypart(context, myconidBody, "left arm", "Upper Foreleg", BodypartTypeEnum.Wear, parts, order++, Alignment.Left, Orientation.Low, "torso");
		AddBodypart(context, myconidBody, "right arm", "Upper Foreleg", BodypartTypeEnum.Wear, parts, order++, Alignment.Right, Orientation.Low, "torso");
		AddBodypart(context, myconidBody, "left leg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontLeft, Orientation.Low, "torso");
		AddBodypart(context, myconidBody, "right leg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontRight, Orientation.Low, "torso");
		AddHeadFeatures(context, myconidBody, parts, ref order);
		AddStandardOrgans(context, myconidBody, parts, ref order);
		AddLimb(context, myconidBody, "Torso", LimbType.Torso, "torso", parts,
				"torso", "heart", "liver", "spleen", "stomach", "small intestines", "large intestines",
				"left kidney", "right kidney", "left lung", "right lung");
		AddLimb(context, myconidBody, "Head", LimbType.Head, "head", parts,
				"head", "mouth", "tongue", "nose", "left eye", "right eye", "left ear", "right ear", "brain");
		AddLimb(context, myconidBody, "Left Arm", LimbType.Arm, "left arm", parts, "left arm");
		AddLimb(context, myconidBody, "Right Arm", LimbType.Arm, "right arm", parts, "right arm");
		AddLimb(context, myconidBody, "Left Leg", LimbType.Leg, "left leg", parts, "left leg");
		AddLimb(context, myconidBody, "Right Leg", LimbType.Leg, "right leg", parts, "right leg");

		var plantBody = new BodyProto
		{
			Name = "Plantfolk",
			CountsAs = myconidBody,
			WielderDescriptionSingle = "tendril",
			WielderDescriptionPlural = "tendrils",
			StaminaRecoveryProgId = humanoid.StaminaRecoveryProgId,
			MinimumLegsToStand = 2,
			MinimumWingsToFly = 0,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(plantBody);

		parts.Clear();
		order = 1;
		AddBodypart(context, plantBody, "torso", "Body", BodypartTypeEnum.Wear, parts, order++);
		AddBodypart(context, plantBody, "head", "Fungus Cap", BodypartTypeEnum.Wear, parts, order++, Alignment.Front, Orientation.Highest, "torso");
		AddBodypart(context, plantBody, "left arm", "Upper Foreleg", BodypartTypeEnum.Wear, parts, order++, Alignment.Left, Orientation.Low, "torso");
		AddBodypart(context, plantBody, "right arm", "Upper Foreleg", BodypartTypeEnum.Wear, parts, order++, Alignment.Right, Orientation.Low, "torso");
		AddBodypart(context, plantBody, "left leg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontLeft, Orientation.Low, "torso");
		AddBodypart(context, plantBody, "right leg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontRight, Orientation.Low, "torso");
		AddHeadFeatures(context, plantBody, parts, ref order);
		AddStandardOrgans(context, plantBody, parts, ref order);
		AddLimb(context, plantBody, "Torso", LimbType.Torso, "torso", parts,
				"torso", "heart", "liver", "spleen", "stomach", "small intestines", "large intestines",
				"left kidney", "right kidney", "left lung", "right lung");
		AddLimb(context, plantBody, "Head", LimbType.Head, "head", parts,
				"head", "mouth", "tongue", "nose", "left eye", "right eye", "left ear", "right ear", "brain");
		AddLimb(context, plantBody, "Left Arm", LimbType.Arm, "left arm", parts, "left arm");
		AddLimb(context, plantBody, "Right Arm", LimbType.Arm, "right arm", parts, "right arm");
		AddLimb(context, plantBody, "Left Leg", LimbType.Leg, "left leg", parts, "left leg");
		AddLimb(context, plantBody, "Right Leg", LimbType.Leg, "right leg", parts, "right leg");

		var unicornBody = new BodyProto
		{
			Name = "Unicorn",
			CountsAs = ungulate,
			WielderDescriptionSingle = "hoof",
			WielderDescriptionPlural = "hooves",
			StaminaRecoveryProgId = ungulate.StaminaRecoveryProgId,
			MinimumLegsToStand = 4,
			MinimumWingsToFly = 0,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(unicornBody);

		parts.Clear();
		order = 1;
		AddBodypart(context, unicornBody, "torso", "Body", BodypartTypeEnum.Wear, parts, order++);
		AddBodypart(context, unicornBody, "head", "Head", BodypartTypeEnum.Wear, parts, order++, Alignment.Front, Orientation.Highest, "torso");
		AddBodypart(context, unicornBody, "left foreleg", "Upper Foreleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontLeft, Orientation.Low, "torso");
		AddBodypart(context, unicornBody, "right foreleg", "Upper Foreleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontRight, Orientation.Low, "torso");
		AddBodypart(context, unicornBody, "left hindleg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.RearLeft, Orientation.Low, "torso");
		AddBodypart(context, unicornBody, "right hindleg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.RearRight, Orientation.Low, "torso");
		AddBodypart(context, unicornBody, "horn", "Horn", BodypartTypeEnum.Wear, parts, order++, Alignment.Front, Orientation.Highest, "head");
		AddHeadFeatures(context, unicornBody, parts, ref order);
		AddStandardOrgans(context, unicornBody, parts, ref order);
		AddLimb(context, unicornBody, "Torso", LimbType.Torso, "torso", parts,
				"torso", "heart", "liver", "spleen", "stomach", "small intestines", "large intestines",
				"left kidney", "right kidney", "left lung", "right lung", "horn");
		AddLimb(context, unicornBody, "Head", LimbType.Head, "head", parts,
				"head", "mouth", "tongue", "nose", "left eye", "right eye", "left ear", "right ear", "brain", "horn");
		AddLimb(context, unicornBody, "Left Foreleg", LimbType.Leg, "left foreleg", parts, "left foreleg");
		AddLimb(context, unicornBody, "Right Foreleg", LimbType.Leg, "right foreleg", parts, "right foreleg");
		AddLimb(context, unicornBody, "Left Hindleg", LimbType.Leg, "left hindleg", parts, "left hindleg");
		AddLimb(context, unicornBody, "Right Hindleg", LimbType.Leg, "right hindleg", parts, "right hindleg");

		var pegasusBody = new BodyProto
		{
			Name = "Pegasus",
			CountsAs = ungulate,
			WielderDescriptionSingle = "hoof",
			WielderDescriptionPlural = "hooves",
			StaminaRecoveryProgId = ungulate.StaminaRecoveryProgId,
			MinimumLegsToStand = 4,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(pegasusBody);

		parts.Clear();
		order = 1;
		AddBodypart(context, pegasusBody, "torso", "Body", BodypartTypeEnum.Wear, parts, order++);
		AddBodypart(context, pegasusBody, "head", "Head", BodypartTypeEnum.Wear, parts, order++, Alignment.Front, Orientation.Highest, "torso");
		AddBodypart(context, pegasusBody, "left foreleg", "Upper Foreleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontLeft, Orientation.Low, "torso");
		AddBodypart(context, pegasusBody, "right foreleg", "Upper Foreleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontRight, Orientation.Low, "torso");
		AddBodypart(context, pegasusBody, "left hindleg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.RearLeft, Orientation.Low, "torso");
		AddBodypart(context, pegasusBody, "right hindleg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.RearRight, Orientation.Low, "torso");
		AddBodypart(context, pegasusBody, "left wing", "Wing", BodypartTypeEnum.Wing, parts, order++, Alignment.Left, Orientation.High, "torso");
		AddBodypart(context, pegasusBody, "right wing", "Wing", BodypartTypeEnum.Wing, parts, order++, Alignment.Right, Orientation.High, "torso");
		AddHeadFeatures(context, pegasusBody, parts, ref order);
		AddStandardOrgans(context, pegasusBody, parts, ref order);
		AddLimb(context, pegasusBody, "Torso", LimbType.Torso, "torso", parts,
				"torso", "heart", "liver", "spleen", "stomach", "small intestines", "large intestines",
				"left kidney", "right kidney", "left lung", "right lung");
		AddLimb(context, pegasusBody, "Head", LimbType.Head, "head", parts,
				"head", "mouth", "tongue", "nose", "left eye", "right eye", "left ear", "right ear", "brain");
		AddLimb(context, pegasusBody, "Left Foreleg", LimbType.Leg, "left foreleg", parts, "left foreleg");
		AddLimb(context, pegasusBody, "Right Foreleg", LimbType.Leg, "right foreleg", parts, "right foreleg");
		AddLimb(context, pegasusBody, "Left Hindleg", LimbType.Leg, "left hindleg", parts, "left hindleg");
		AddLimb(context, pegasusBody, "Right Hindleg", LimbType.Leg, "right hindleg", parts, "right hindleg");
		AddLimb(context, pegasusBody, "Left Wing", LimbType.Wing, "left wing", parts, "left wing");
		AddLimb(context, pegasusBody, "Right Wing", LimbType.Wing, "right wing", parts, "right wing");

		var pegacornBody = new BodyProto
		{
			Name = "Pegacorn",
			CountsAs = ungulate,
			WielderDescriptionSingle = "hoof",
			WielderDescriptionPlural = "hooves",
			StaminaRecoveryProgId = ungulate.StaminaRecoveryProgId,
			MinimumLegsToStand = 4,
			MinimumWingsToFly = 2,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WearSizeParameter = wearSize
		};
		context.BodyProtos.Add(pegacornBody);

		parts.Clear();
		order = 1;
		AddBodypart(context, pegacornBody, "torso", "Body", BodypartTypeEnum.Wear, parts, order++);
		AddBodypart(context, pegacornBody, "head", "Head", BodypartTypeEnum.Wear, parts, order++, Alignment.Front, Orientation.Highest, "torso");
		AddBodypart(context, pegacornBody, "left foreleg", "Upper Foreleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontLeft, Orientation.Low, "torso");
		AddBodypart(context, pegacornBody, "right foreleg", "Upper Foreleg", BodypartTypeEnum.Wear, parts, order++, Alignment.FrontRight, Orientation.Low, "torso");
		AddBodypart(context, pegacornBody, "left hindleg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.RearLeft, Orientation.Low, "torso");
		AddBodypart(context, pegacornBody, "right hindleg", "Upper Hindleg", BodypartTypeEnum.Wear, parts, order++, Alignment.RearRight, Orientation.Low, "torso");
		AddBodypart(context, pegacornBody, "left wing", "Wing", BodypartTypeEnum.Wing, parts, order++, Alignment.Left, Orientation.High, "torso");
		AddBodypart(context, pegacornBody, "right wing", "Wing", BodypartTypeEnum.Wing, parts, order++, Alignment.Right, Orientation.High, "torso");
		AddBodypart(context, pegacornBody, "horn", "Horn", BodypartTypeEnum.Wear, parts, order++, Alignment.Front, Orientation.Highest, "head");
		AddHeadFeatures(context, pegacornBody, parts, ref order);
		AddStandardOrgans(context, pegacornBody, parts, ref order);
		AddLimb(context, pegacornBody, "Torso", LimbType.Torso, "torso", parts,
				"torso", "heart", "liver", "spleen", "stomach", "small intestines", "large intestines",
				"left kidney", "right kidney", "left lung", "right lung", "horn");
		AddLimb(context, pegacornBody, "Head", LimbType.Head, "head", parts,
				"head", "mouth", "tongue", "nose", "left eye", "right eye", "left ear", "right ear", "brain", "horn");
		AddLimb(context, pegacornBody, "Left Foreleg", LimbType.Leg, "left foreleg", parts, "left foreleg");
		AddLimb(context, pegacornBody, "Right Foreleg", LimbType.Leg, "right foreleg", parts, "right foreleg");
		AddLimb(context, pegacornBody, "Left Hindleg", LimbType.Leg, "left hindleg", parts, "left hindleg");
		AddLimb(context, pegacornBody, "Right Hindleg", LimbType.Leg, "right hindleg", parts, "right hindleg");
		AddLimb(context, pegacornBody, "Left Wing", LimbType.Wing, "left wing", parts, "left wing");
		AddLimb(context, pegacornBody, "Right Wing", LimbType.Wing, "right wing", parts, "right wing");

		context.SaveChanges();

		(Race race, Ethnicity ethnicity) AddRace(string name, BodyProto body, string description)
		{
			var race = new Race
			{
				Name = name,
				Description = description,
				BaseBody = body,
				AllowedGenders = "2 3",
				AttributeBonusProg = alwaysZero,
				AttributeTotalCap = context.TraitDefinitions.Count(x => x.Type == 1) * 12,
				IndividualAttributeCap = 20,
				DiceExpression = "3d6+1",
				IlluminationPerceptionMultiplier = 1.0,
				AvailabilityProg = alwaysTrue,
				CorpseModel = corpse,
				DefaultHealthStrategy = health,
				CanUseWeapons = false,
				CanAttack = true,
				CanDefend = true,
				NeedsToBreathe = true,
				SizeStanding = 6,
				SizeProne = 6,
				SizeSitting = 6,
				CommunicationStrategyType = "humanoid",
				HandednessOptions = "1 3",
				DefaultHandedness = 1,
				ChildAge = 1,
				YouthAge = 5,
				YoungAdultAge = 12,
				AdultAge = 18,
				ElderAge = 50,
				VenerableAge = 80,
				CanClimb = false,
				CanSwim = true,
				MinimumSleepingPosition = 4,
				BodypartHealthMultiplier = 1.0,
				BodypartSizeModifier = 0,
				TemperatureRangeCeiling = 40,
				TemperatureRangeFloor = 0,
				CanEatCorpses = false,
				CanEatMaterialsOptIn = false,
				BiteWeight = 1000,
				EatCorpseEmoteText = string.Empty,
				RaceUsesStamina = true,
				NaturalArmourQuality = 2,
				SweatLiquid = context.Liquids.FirstOrDefault(x => x.Name == "sweat"),
				SweatRateInLitresPerMinute = 0.5,
				BloodLiquid = context.Liquids.First(x => x.Name == "blood"),
				BreathingVolumeExpression = "7",
				HoldBreathLengthExpression = "90",
				MaximumLiftWeightExpression = "10000",
				MaximumDragWeightExpression = "40000",
				DefaultHeightWeightModelMale = context.HeightWeightModels.First(),
				DefaultHeightWeightModelFemale = context.HeightWeightModels.First(),
				DefaultHeightWeightModelNeuter = context.HeightWeightModels.First(),
				DefaultHeightWeightModelNonBinary = context.HeightWeightModels.First()
			};
			context.Races.Add(race);
			var ethnicity = new Ethnicity
			{
				Name = $"{name} Stock",
				ParentRace = race,
				EthnicGroup = name,
				PopulationBloodModel = context.PopulationBloodModels.First()
			};
			context.Ethnicities.Add(ethnicity);
			AddRaceDescription(context, race, description);
			return (race, ethnicity);
		}

		var (dragon, dragonEth) = AddRace("Dragon", dragonBody, "A mighty winged reptile that breathes fire.");
		AddColourCharacteristic(context, dragon, dragonEth, "Scale Colour", new[] { "red", "green", "black", "gold" });

		AddRace("Wyvern", wyvernBody, "A two-legged dragon with wings.");
		AddRace("Eastern Dragon", easternBody, "A serpentine dragon without wings.");
		AddRace("Centaur", centaurBody, "A being with a human torso and horse body.");

		var (myconid, myconidEth) = AddRace("Myconid", myconidBody, "A humanoid shaped fungus creature.");
		AddColourCharacteristic(context, myconid, myconidEth, "Fungus Colour", new[] { "white", "brown", "red", "purple" });

		AddRace("Plantfolk", plantBody, "A humanoid plant creature.");
		AddRace("Unicorn", unicornBody, "A horse with a single magical horn.");
		AddRace("Pegasus", pegasusBody, "A winged horse able to fly.");
		AddRace("Pegacorn", pegacornBody, "A winged unicorn.");

		context.SaveChanges();
		context.Database.CommitTransaction();
		return "Fantasy races installed.";
	}

	/// <inheritdoc />
	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.BodyProtos.Any(x => x.Name == "Humanoid") ||
			!context.BodyProtos.Any(x => x.Name == "Quadruped Base") ||
			!context.CorpseModels.Any(x => x.Name == "Organic Animal Corpse"))
			return ShouldSeedResult.PrerequisitesNotMet;

		if (context.Races.Any(x => x.Name == "Dragon"))
			return ShouldSeedResult.MayAlreadyBeInstalled;

		return ShouldSeedResult.ReadyToInstall;
	}

	/// <inheritdoc />
	public bool Enabled => false;

	#endregion
}
