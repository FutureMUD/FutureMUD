#nullable enable

using System;
using DatabaseSeeder;
using System.Linq;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RobotSeederTemplateTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static BodyProto CreateBodyProto(long id, string name, string wielderPlural, string wielderSingle)
	{
		return new BodyProto
		{
			Id = id,
			Name = name,
			ConsiderString = string.Empty,
			LegDescriptionPlural = "legs",
			LegDescriptionSingular = "leg",
			WielderDescriptionPlural = wielderPlural,
			WielderDescriptionSingle = wielderSingle,
			NameForTracking = name
		};
	}

	private static Race CreateRace(long id, string name, long baseBodyId, long corpseModelId, long attributeBonusProgId)
	{
		return new Race
		{
			Id = id,
			Name = name,
			Description = $"{name} test race",
			BaseBodyId = baseBodyId,
			AllowedGenders = "Male Female Neuter NonBinary",
			AttributeBonusProgId = attributeBonusProgId,
			DiceExpression = "1d100",
			CorpseModelId = corpseModelId,
			DefaultHealthStrategyId = 1,
			BreathingModel = "Simple",
			CommunicationStrategyType = "HumanoidCommunicationStrategy",
			HandednessOptions = "Left,Right",
			MaximumDragWeightExpression = "100",
			MaximumLiftWeightExpression = "100",
			EatCorpseEmoteText = "@ eat|eats $0.",
			BreathingVolumeExpression = "1",
			HoldBreathLengthExpression = "1"
		};
	}

	private static CharacteristicDefinition CreateCharacteristicDefinition(long id, string name)
	{
		return new CharacteristicDefinition
		{
			Id = id,
			Name = name,
			Pattern = "*",
			Description = $"{name} description",
			Model = "Simple",
			Definition = "<Definition />"
		};
	}

	private static FutureProg CreateFutureProg(long id, string functionName)
	{
		return new FutureProg
		{
			Id = id,
			FunctionName = functionName,
			FunctionComment = $"{functionName} test prog",
			FunctionText = "return true;",
			ReturnType = 0,
			Category = "Tests",
			Subcategory = "RobotSeeder",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
	}

	private static WeaponAttack CreateWeaponAttack(long id, string name)
	{
		return new WeaponAttack
		{
			Id = id,
			Name = name,
			AdditionalInfo = string.Empty,
			RequiredPositionStateIds = string.Empty
		};
	}

	private static CorpseModel CreateCorpseModel(long id, string name)
	{
		return new CorpseModel
		{
			Id = id,
			Name = name,
			Description = $"{name} description",
			Definition = "<Definition />",
			Type = "SimpleCorpseModel"
		};
	}

	private static void SetPrivateField<T>(RobotSeeder seeder, string fieldName, T value)
	{
		var field = typeof(RobotSeeder).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.IsNotNull(field, $"Could not find private field {fieldName} on {nameof(RobotSeeder)}.");
		field.SetValue(seeder, value);
	}

	[TestMethod]
	public void ValidateTemplateCatalogForTesting_CurrentCatalog_HasNoIssues()
	{
		var issues = RobotSeeder.ValidateTemplateCatalogForTesting();
		Assert.AreEqual(0, issues.Count, string.Join("\n", issues));
	}

	[TestMethod]
	public void TemplatesForTesting_KeyVariants_UseExpectedBodyAssignments()
	{
		Assert.AreEqual("Robot Humanoid", RobotSeeder.TemplatesForTesting["Robot Humanoid"].BodyKey);
		Assert.AreEqual("Spider Crawler Robot", RobotSeeder.TemplatesForTesting["Spider Crawler Robot"].BodyKey);
		Assert.AreEqual("Winged Robot", RobotSeeder.TemplatesForTesting["Winged Robot"].BodyKey);
		Assert.AreEqual("Cyborg Humanoid", RobotSeeder.TemplatesForTesting["Cyborg"].BodyKey);
		Assert.AreEqual("Roomba Robot", RobotSeeder.TemplatesForTesting["Roomba Robot"].BodyKey);
		Assert.AreEqual("Robot Dog", RobotSeeder.TemplatesForTesting["Robot Dog"].BodyKey);
		Assert.AreEqual("Robot Cockroach", RobotSeeder.TemplatesForTesting["Robot Cockroach"].BodyKey);
	}

	[TestMethod]
	public void TemplatesForTesting_AttachmentVariants_UseIntegratedWeaponBodies()
	{
		var sawRobot = RobotSeeder.TemplatesForTesting["Circular Saw Robot"];
		Assert.IsFalse(sawRobot.CanUseWeapons, "Saw-hand robots should not retain normal weapon handling.");
		CollectionAssert.AreEquivalent(
			new[] { "Circular Saw Slash", "Elbow", "Bite", "Snap Kick" },
			sawRobot.Attacks.Select(x => x.AttackName).ToArray());
		CollectionAssert.AreEquivalent(
			new[] { "rsaw", "lsaw" },
			sawRobot.BodypartUsages!.Select(x => x.BodypartAlias).ToArray());

		var hammerRobot = RobotSeeder.TemplatesForTesting["Pneumatic Hammer Robot"];
		Assert.IsFalse(hammerRobot.CanUseWeapons, "Hammer-hand robots should not retain normal weapon handling.");
		CollectionAssert.Contains(hammerRobot.Attacks.Select(x => x.AttackName).ToList(), "Pneumatic Hammer Blow");

		var swordRobot = RobotSeeder.TemplatesForTesting["Sword-Hand Robot"];
		Assert.IsFalse(swordRobot.CanUseWeapons, "Sword-hand robots should not retain normal weapon handling.");
		CollectionAssert.Contains(swordRobot.Attacks.Select(x => x.AttackName).ToList(), "Sword-Hand Lunge");
	}

	[TestMethod]
	public void TemplatesForTesting_HumanoidRobotDescriptions_AreDefinedAndDistinctive()
	{
		var expectations = new[]
		{
			("Robot Humanoid", "service robot", "sensor-packed head"),
			("Spider Crawler Robot", "crawler robot", "crawler base"),
			("Circular Saw Robot", "circular-saw", "circular saws"),
			("Pneumatic Hammer Robot", "hammer-armed", "pneumatic hammers"),
			("Sword-Hand Robot", "sword-handed", "monoblade sword"),
			("Winged Robot", "winged", "articulated wings"),
			("Jet Robot", "jet-backed", "jet pods"),
			("Mandible Robot", "mandible-faced", "shearing mandibles"),
			("Wheeled Robot", "wheeled", "wheel assemblies"),
			("Tracked Robot", "tracked", "track pods")
		};

		foreach (var (name, shortSnippet, fullSnippet) in expectations)
		{
			var template = RobotSeeder.TemplatesForTesting[name];
			Assert.IsFalse(string.IsNullOrWhiteSpace(template.ShortDescriptionPattern),
				$"{name} should define a stock short description pattern.");
			Assert.IsFalse(string.IsNullOrWhiteSpace(template.FullDescriptionPattern),
				$"{name} should define a stock full description pattern.");
			StringAssert.Contains(template.ShortDescriptionPattern!, shortSnippet);
			StringAssert.Contains(template.FullDescriptionPattern!, fullSnippet);
		}

		Assert.IsTrue(string.IsNullOrWhiteSpace(RobotSeeder.TemplatesForTesting["Cyborg"].ShortDescriptionPattern),
			"Cyborgs should continue to use the organic humanoid description path.");
	}

	[TestMethod]
	public void HumanSeeder_OrganicHumanoidDescriptionScope_ExcludesMechanicalHumanoids()
	{
		Assert.AreEqual(
			"SameRace(@ch.Race, ToRace(\"Organic Humanoid\"))",
			HumanSeeder.OrganicHumanoidDescriptionRaceCondition);

		var updated = HumanSeeder.UpdateHumanoidDescriptionProgScope(
			"return SameRace(@ch.Race, ToRace(\"Humanoid\")) and @ch.AgeCategory == \"Adult\"");

		StringAssert.Contains(updated, "Organic Humanoid");
		Assert.IsFalse(updated.Contains("ToRace(\"Humanoid\")"),
			"Humanoid description progs should no longer scope against the mechanical humanoid parent race.");
	}

	[TestMethod]
	public void TemplatesForTesting_MobilityVariants_ExposeExpectedFlavourAndLiquids()
	{
		var spider = RobotSeeder.TemplatesForTesting["Spider Crawler Robot"];
		CollectionAssert.AreEquivalent(
			new[] { "rleg1", "lleg1", "rleg2", "lleg2", "rleg3", "lleg3", "rleg4", "lleg4" },
			spider.BodypartUsages!.Select(x => x.BodypartAlias).ToArray(),
			"Spider crawlers should expose all crawler legs as flavour bodyparts.");

		var wheeled = RobotSeeder.TemplatesForTesting["Wheeled Robot"];
		Assert.IsFalse(wheeled.CanClimb, "Wheeled robots should not be seeded as climbers.");
		CollectionAssert.AreEquivalent(
			new[] { "rwheel", "lwheel" },
			wheeled.BodypartUsages!.Select(x => x.BodypartAlias).ToArray());

		var trackedUtility = RobotSeeder.TemplatesForTesting["Tracked Utility Robot"];
		Assert.AreEqual("machine oil", trackedUtility.BloodLiquidName,
			"Utility robots should use machine oil as their circulatory fluid.");
		CollectionAssert.AreEquivalent(
			new[] { "rtrack", "ltrack" },
			trackedUtility.BodypartUsages!.Select(x => x.BodypartAlias).ToArray());
	}

	[TestMethod]
	public void CustomLimbMembershipsForTesting_DerivedRobotBodies_MapExpectedAssemblies()
	{
		CollectionAssert.AreEquivalent(
			new[] { "Mandible Robot", "Tracked Robot", "Wheeled Robot" },
			RobotSeeder.CustomLimbMembershipsForTesting.Keys.ToArray(),
			"The robot seeder should explicitly map every derived assembly that is grafted onto an existing limb.");

		var mandibleMappings = RobotSeeder.CustomLimbMembershipsForTesting["Mandible Robot"].ToArray();
		Assert.AreEqual(1, mandibleMappings.Length);
		Assert.AreEqual("neck", mandibleMappings[0].LimbRootAlias);
		Assert.AreEqual("mandibles", mandibleMappings[0].PartAlias);

		CollectionAssert.AreEquivalent(
			new[] { "rhip:rwheel", "lhip:lwheel" },
			RobotSeeder.CustomLimbMembershipsForTesting["Wheeled Robot"]
				.Select(x => $"{x.LimbRootAlias}:{x.PartAlias}")
				.ToArray());
		CollectionAssert.AreEquivalent(
			new[] { "rhip:rtrack", "lhip:ltrack" },
			RobotSeeder.CustomLimbMembershipsForTesting["Tracked Robot"]
				.Select(x => $"{x.LimbRootAlias}:{x.PartAlias}")
				.ToArray());
	}

	[TestMethod]
	public void TemplatesForTesting_Cyborg_RemainsOnlyPlayableRobotRace()
	{
		var playableRaces = RobotSeeder.TemplatesForTesting
			.Where(x => x.Value.Playable)
			.Select(x => x.Key)
			.ToArray();

		CollectionAssert.AreEquivalent(new[] { "Cyborg" }, playableRaces);
		Assert.IsTrue(RobotSeeder.TemplatesForTesting["Cyborg"].UsesHumanoidCharacteristics,
			"Cyborgs should retain the humanoid characteristic matrix.");
		Assert.AreEqual("Human", RobotSeeder.TemplatesForTesting["Cyborg"].ParentRaceName,
			"Cyborgs should inherit from the human race line for presentation purposes.");
	}

	[TestMethod]
	public void TemplatesForTesting_GenderAndAttackCoverage_FollowRobotRules()
	{
		Assert.IsTrue(RobotSeeder.TemplatesForTesting["Cyborg"].UsesHumanGenders,
			"Cyborgs should remain the only robot race using the human gender matrix.");
		Assert.IsTrue(
			RobotSeeder.TemplatesForTesting
				.Where(x => x.Key != "Cyborg")
				.All(x => !x.Value.UsesHumanGenders),
			"All non-mechanical-human robot races should be neuter-only.");

		foreach (var (name, template) in RobotSeeder.TemplatesForTesting)
		{
			Assert.IsTrue(
				template.Attacks.Any(x => x.AttackName is "Elbow" or "Bite" or "Mandible Shear" or "Wheel Grind Close" or
					"Track Crush" or "Mandible Bite"),
				$"Robot race {name} should expose a clinch-capable natural attack.");
			Assert.IsTrue(
				template.Attacks.Any(x => x.AttackName is "Jab" or "Cross" or "Hook" or "Circular Saw Slash" or
					"Pneumatic Hammer Blow" or "Sword-Hand Lunge" or "Wing Buffet" or "Jet Ram" or "Wheel Ram" or
					"Track Grind" or "Snap Kick" or "Carnivore Bite" or "Claw Low Swipe" or "Claw High Swipe" or
					"Mandible Snap"),
				$"Robot race {name} should expose a non-clinch natural attack.");
		}
	}

	[TestMethod]
	public void CanSupportBodyKeyForTesting_AvianVariants_AreOptionalWhenAvianBodyMissing()
	{
		Assert.IsFalse(RobotSeeder.CanSupportBodyKeyForTesting(new[] { "Humanoid" }, "Winged Robot"));
		Assert.IsFalse(RobotSeeder.CanSupportBodyKeyForTesting(new[] { "Humanoid" }, "Jet Robot"));
		Assert.IsTrue(RobotSeeder.CanSupportBodyKeyForTesting(new[] { "Humanoid", "Avian" }, "Winged Robot"));
		Assert.IsTrue(RobotSeeder.CanSupportBodyKeyForTesting(new[] { "Winged Robot" }, "Winged Robot"));
	}

	[TestMethod]
	public void ShouldSeedData_MissingAvianButOtherPrerequisitesPresent_IsReadyToInstall()
	{
		using var context = BuildContext();
		context.BodyProtos.AddRange(
			CreateBodyProto(1, "Humanoid", "hands", "hand"),
			CreateBodyProto(2, "Toed Quadruped", "paws", "paw"),
			CreateBodyProto(3, "Insectoid", "mandibles", "mandible"),
			CreateBodyProto(4, "Arachnid", "mandibles", "mandible"));
		context.HealthStrategies.Add(new HealthStrategy
		{
			Id = 1,
			Name = "Placeholder Strategy",
			Type = "TestStrategy",
			Definition = "<Definition />"
		});
		context.Races.AddRange(
			CreateRace(1, "Human", 1, 1, 1),
			CreateRace(2, "Humanoid", 1, 2, 1));
		context.CharacteristicDefinitions.AddRange(
			CreateCharacteristicDefinition(1, "All Eye Colours"),
			CreateCharacteristicDefinition(2, "All Eye Shapes"),
			CreateCharacteristicDefinition(3, "All Noses"),
			CreateCharacteristicDefinition(4, "All Ears"),
			CreateCharacteristicDefinition(5, "All Hair Colours"),
			CreateCharacteristicDefinition(6, "All Facial Hair Colours"),
			CreateCharacteristicDefinition(7, "All Hair Styles"),
			CreateCharacteristicDefinition(8, "All Skin Colours"),
			CreateCharacteristicDefinition(9, "All Frames"),
			CreateCharacteristicDefinition(10, "Person Word"));
		context.FutureProgs.AddRange(
			CreateFutureProg(1, "AlwaysTrue"),
			CreateFutureProg(2, "AlwaysFalse"));
		context.Tags.AddRange(
			new Tag { Id = 1, Name = "Arterial Clamp" },
			new Tag { Id = 2, Name = "Bonesaw" },
			new Tag { Id = 3, Name = "Forceps" },
			new Tag { Id = 4, Name = "Scalpel" },
			new Tag { Id = 5, Name = "Surgical Suture Needle" });
		context.WeaponAttacks.AddRange(
			CreateWeaponAttack(1, "Jab"),
			CreateWeaponAttack(2, "Cross"),
			CreateWeaponAttack(3, "Hook"),
			CreateWeaponAttack(4, "Elbow"),
			CreateWeaponAttack(5, "Bite"),
			CreateWeaponAttack(6, "Snap Kick"),
			CreateWeaponAttack(7, "Carnivore Bite"),
			CreateWeaponAttack(8, "Claw Low Swipe"),
			CreateWeaponAttack(9, "Claw High Swipe"),
			CreateWeaponAttack(10, "Mandible Bite"),
			CreateWeaponAttack(11, "Animal Barge"),
			CreateWeaponAttack(12, "Hoof Stomp Smash"));
		context.CorpseModels.AddRange(
			CreateCorpseModel(1, "Organic Human Corpse"),
			CreateCorpseModel(2, "Organic Animal Corpse"));
		context.SaveChanges();

		var result = new RobotSeeder().ShouldSeedData(context);

		Assert.AreEqual(ShouldSeedResult.ReadyToInstall, result,
			"The robot seeder should no longer hard-fail when avian anatomy is absent.");
	}

	[TestMethod]
	public void CloneBody_SourceWithCountsAsAncestor_CreatesSelfContainedFlattenedRobotBody()
	{
		using var context = BuildContext();
		var shape = new BodypartShape { Id = 1, Name = "plate" };
		var material = new Material
		{
			Id = 1,
			Name = "alloy",
			MaterialDescription = "alloy",
			Density = 1.0,
			Organic = false,
			Type = 0,
			ThermalConductivity = 1.0,
			ElectricalConductivity = 1.0,
			SpecificHeatCapacity = 1.0,
			SolventVolumeRatio = 0.0,
			ResidueSdesc = "residue",
			ResidueDesc = "residue",
			ResidueColour = "grey",
			Absorbency = 0.0
		};
		var armour = new ArmourType
		{
			Id = 1,
			Name = "plating",
			Definition = "<ArmourType />"
		};
		var recoveryProg = CreateFutureProg(1, "RobotRecovery");
		context.BodypartShapes.Add(shape);
		context.Materials.Add(material);
		context.ArmourTypes.Add(armour);
		context.FutureProgs.Add(recoveryProg);

		var baseBody = CreateBodyProto(1, "Quadruped", "paws", "paw");
		baseBody.StaminaRecoveryProgId = recoveryProg.Id;
		baseBody.MinimumLegsToStand = 4;
		baseBody.MinimumWingsToFly = 0;
		var derivedBody = CreateBodyProto(2, "Toed Quadruped", "paws", "paw");
		derivedBody.StaminaRecoveryProgId = recoveryProg.Id;
		derivedBody.MinimumLegsToStand = 4;
		derivedBody.MinimumWingsToFly = 0;
		derivedBody.CountsAs = baseBody;
		context.BodyProtos.AddRange(baseBody, derivedBody);
		context.SaveChanges();

		var torso = new BodypartProto
		{
			Id = 1,
			Body = baseBody,
			Name = "torso",
			Description = "torso",
			BodypartShape = shape,
			BodypartType = 0,
			Alignment = 0,
			Location = 0,
			RelativeHitChance = 100,
			SeveredThreshold = 10,
			MaxLife = 10,
			DisplayOrder = 1,
			DefaultMaterial = material,
			ArmourType = armour,
			Size = 0,
			IsCore = true,
			Significant = true,
			ImplantSpace = 0.0,
			ImplantSpaceOccupied = 0.0
		};
		var paw = new BodypartProto
		{
			Id = 2,
			Body = derivedBody,
			Name = "paw",
			Description = "paw",
			BodypartShape = shape,
			BodypartType = 0,
			Alignment = 0,
			Location = 0,
			RelativeHitChance = 50,
			SeveredThreshold = 10,
			MaxLife = 10,
			DisplayOrder = 2,
			DefaultMaterial = material,
			ArmourType = armour,
			Size = 0,
			IsCore = true,
			Significant = true,
			ImplantSpace = 0.0,
			ImplantSpaceOccupied = 0.0
		};
		context.BodypartProtos.AddRange(torso, paw);
		context.SaveChanges();

		var seeder = new RobotSeeder();
		SetPrivateField(seeder, "_context", context);
		SetPrivateField(seeder, "_robotStaminaRecoveryProg", recoveryProg);

		var cloneMethod = typeof(RobotSeeder).GetMethod("CloneBody", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.IsNotNull(cloneMethod, "Could not find the private CloneBody helper.");

		var clonedBody = (BodyProto)cloneMethod.Invoke(seeder, new object[] { "Robot Quadruped", derivedBody })!;
		Assert.IsNull(clonedBody.CountsAsId,
			"Robot full-clone bodies should be self-contained so the runtime does not append the donor body's anatomy a second time.");

		var clonedParts = context.BodypartProtos
			.Where(x => x.BodyId == clonedBody.Id)
			.OrderBy(x => x.DisplayOrder)
			.ToList();
		CollectionAssert.AreEquivalent(new[] { "torso", "paw" }, clonedParts.Select(x => x.Name).ToArray());
		Assert.IsTrue(clonedParts.All(x => x.BodyId == clonedBody.Id));
		Assert.AreEqual(baseBody.Id, clonedParts.Single(x => x.Name == "torso").CountAsId);
		Assert.AreEqual(paw.Id, clonedParts.Single(x => x.Name == "paw").CountAsId);
	}
}
