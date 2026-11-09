#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Grouping;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RobotSeederTemplateTests
{

    private static FuturemudDatabaseContext BuildContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
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

    private static Race CreateRace(long id, string name, long baseBodyId, long corpseModelId)
    {
        return new Race
        {
            Id = id,
            Name = name,
            Description = $"{name} test race",
            BaseBodyId = baseBodyId,
            AllowedGenders = "Male Female Neuter NonBinary",
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

    [TestMethod]
    public void BodypartGroupIDDescriber_Match_InheritedOverridePart_MatchesParentPrototype()
    {
        BodypartGroupIDDescriber describer = new(new MudSharp.Models.BodypartGroupDescriber
        {
            Id = 1,
            DescribedAs = "hand pair",
            Comment = string.Empty
        }, new Mock<MudSharp.Framework.IFuturemud>().Object);
        Mock<IBodypart> parentPart = new();
        Mock<IBodypart> childOverride = new();
        childOverride.Setup(x => x.CountsAs(parentPart.Object)).Returns(true);

        FieldInfo? field = typeof(BodypartGroupIDDescriber).GetField("Prototypes", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, "Could not find the direct bodypart prototype lookup.");
        Dictionary<IBodypart, bool> prototypes = (System.Collections.Generic.Dictionary<IBodypart, bool>)field.GetValue(describer)!;
        prototypes[parentPart.Object] = true;

        BodypartGroupResult result = describer.Match([childOverride.Object]);

        Assert.IsTrue(result.IsMatch,
            "Inherited override bodyparts should satisfy direct group describers when they CountAs the parent prototype.");
        Assert.AreEqual(1, result.MatchScore);
        CollectionAssert.AreEqual(new[] { childOverride.Object }, result.Matches);
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
        using FuturemudDatabaseContext context = BuildContext();
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
            CreateRace(1, "Human", 1, 1),
            CreateRace(2, "Humanoid", 1, 2));
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

        ShouldSeedResult result = new RobotSeeder().ShouldSeedData(context);

        Assert.AreEqual(ShouldSeedResult.ReadyToInstall, result,
            "The robot seeder should no longer hard-fail when avian anatomy is absent.");
    }

}
