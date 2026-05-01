#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.CharacterCreation;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Merits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ProgVariableTypes = MudSharp.FutureProg.ProgVariableTypes;
using PlanarInteractionKind = MudSharp.Planes.PlanarInteractionKind;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StockMeritsSeederTests
{
    private static FuturemudDatabaseContext BuildContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new FuturemudDatabaseContext(options);
    }

    private static void SeedAccount(FuturemudDatabaseContext context)
    {
        context.Accounts.Add(new Account
        {
            Id = 1,
            Name = "SeederTest",
            Password = "password",
            Salt = 1,
            AccessStatus = 0,
            Email = "seeder@example.com",
            LastLoginIp = "127.0.0.1",
            FormatLength = 80,
            InnerFormatLength = 78,
            UseMxp = false,
            UseMsp = false,
            UseMccp = false,
            ActiveCharactersAllowed = 1,
            UseUnicode = true,
            TimeZoneId = "UTC",
            CultureName = "en-AU",
            RegistrationCode = string.Empty,
            IsRegistered = true,
            RecoveryCode = string.Empty,
            UnitPreference = "metric",
            CreationDate = DateTime.UtcNow,
            PageLength = 22,
            PromptType = 0,
            TabRoomDescriptions = false,
            CodedRoomDescriptionAdditionsOnNewLine = false,
            CharacterNameOverlaySetting = 0,
            AppendNewlinesBetweenMultipleEchoesPerPrompt = false,
            ActLawfully = false,
            HasBeenActiveInWeek = true,
            HintsEnabled = true,
            AutoReacquireTargets = false
        });
    }

    private static FutureProg CreateProg(long id, string name, ProgVariableTypes returnType, string text)
    {
        return new FutureProg
        {
            Id = id,
            FunctionName = name,
            FunctionComment = $"{name} test prog",
            FunctionText = text,
            ReturnType = (long)returnType,
            Category = "Tests",
            Subcategory = "StockMeritsSeeder",
            Public = true,
            AcceptsAnyParameters = false,
            StaticType = 0
        };
    }

    private static TraitDecorator CreateTraitDecorator(long id)
    {
        return new TraitDecorator
        {
            Id = id,
            Name = "Test Decorator",
            Type = "Basic",
            Contents = "<Decorator />"
        };
    }

    private static TraitDefinition CreateTraitDefinition(long id, string name, string traitGroup, int type)
    {
        return new TraitDefinition
        {
            Id = id,
            Name = name,
            Type = type,
            DecoratorId = 1,
            TraitGroup = traitGroup,
            DerivedType = 0,
            ChargenBlurb = $"{name} blurb",
            BranchMultiplier = 1.0,
            Alias = name.ToLowerInvariant(),
            TeachDifficulty = 0,
            LearnDifficulty = 0,
            ValueExpression = "0",
            DisplayOrder = (int)id,
            DisplayAsSubAttribute = false,
            ShowInScoreCommand = true,
            ShowInAttributeCommand = true
        };
    }

    private static BodyProto CreateBodyProto(long id, string name)
    {
        return new BodyProto
        {
            Id = id,
            Name = name,
            ConsiderString = string.Empty,
            LegDescriptionPlural = "legs",
            LegDescriptionSingular = "leg",
            WielderDescriptionPlural = "hands",
            WielderDescriptionSingle = "hand",
            NameForTracking = name
        };
    }

    private static bool HasNaturalAttackExample(FuturemudDatabaseContext context, MeleeWeaponVerb verb, MeritType meritType)
    {
        return context.Merits
            .AsEnumerable()
            .Any(x =>
            x.Type == "Natural Attack Quality" &&
            x.MeritType == (int)meritType &&
            int.Parse(XElement.Parse(x.Definition).Attribute("verb")?.Value ?? "-1") == (int)verb);
    }

    private static bool PlanarElementContains(XElement? element, long planeId)
    {
        return element?
            .Elements("Plane")
            .Any(x => long.Parse(x.Attribute("id")?.Value ?? "0") == planeId) == true;
    }

    private static void SeedPrerequisites(FuturemudDatabaseContext context, params string[] omittedTraitNames)
    {
        HashSet<string> omittedTraits = new(omittedTraitNames, StringComparer.OrdinalIgnoreCase);
        SeedAccount(context);
        context.FutureProgs.Add(CreateProg(1, "AlwaysTrue", ProgVariableTypes.Boolean, "return true"));
        context.Races.Add(new Race
        {
            Id = 1,
            Name = "Human",
            Description = "Human test race",
            BaseBodyId = 1,
            AllowedGenders = "Male Female Neuter NonBinary",
            DiceExpression = "1d100",
            CorpseModelId = 0,
            DefaultHealthStrategyId = 0,
            BreathingModel = "Simple",
            CommunicationStrategyType = "HumanoidCommunicationStrategy",
            HandednessOptions = "Left,Right",
            MaximumDragWeightExpression = "100",
            MaximumLiftWeightExpression = "100",
            EatCorpseEmoteText = "@ eat|eats $0.",
            BreathingVolumeExpression = "1",
            HoldBreathLengthExpression = "1"
        });
        context.TraitDecorators.Add(CreateTraitDecorator(1));
        context.TraitDefinitions.AddRange(
            CreateTraitDefinition(1, "Strength", "attribute", 0),
            CreateTraitDefinition(2, "Constitution", "attribute", 0),
            CreateTraitDefinition(3, "Perception", "attribute", 0),
            CreateTraitDefinition(4, "Willpower", "attribute", 0));
        foreach (TraitDefinition trait in context.TraitDefinitions.Local.ToList())
        {
            if (omittedTraits.Contains(trait.Name))
            {
                context.TraitDefinitions.Remove(trait);
            }
        }

        BodyProto body = CreateBodyProto(1, "Humanoid");
        context.BodyProtos.Add(body);
        context.MoveSpeeds.AddRange(
            new MoveSpeed
            {
                Id = 1,
                BodyProto = body,
                BodyProtoId = body.Id,
                Alias = "run",
                FirstPersonVerb = "run",
                ThirdPersonVerb = "runs",
                PresentParticiple = "running",
                PositionId = 1,
                Multiplier = 1.0,
                StaminaMultiplier = 1.0
            },
            new MoveSpeed
            {
                Id = 2,
                BodyProto = body,
                BodyProtoId = body.Id,
                Alias = "sprint",
                FirstPersonVerb = "sprint",
                ThirdPersonVerb = "sprints",
                PresentParticiple = "sprinting",
                PositionId = 1,
                Multiplier = 1.0,
                StaminaMultiplier = 1.0
            });

        context.ChargenScreenStoryboards.Add(new MudSharp.Models.ChargenScreenStoryboard
        {
            Id = 1,
            ChargenStage = (int)ChargenStage.SelectMerits,
            ChargenType = "MeritPicker",
            Order = 1,
            StageDefinition = "<Definition stage=\"select-merits\" />",
            NextStage = 0
        });
        context.SaveChanges();
    }

    [TestMethod]
    public void ShouldSeedData_WithPrerequisitesAndNoStockPackage_ReturnsReadyToInstall()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context);
        StockMeritsSeeder seeder = new();

        Assert.AreEqual(ShouldSeedResult.ReadyToInstall, seeder.ShouldSeedData(context));
    }

    [TestMethod]
    public void SeedData_RerunDoesNotDuplicateStockMeritsOrHelperProgs()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context);
        StockMeritsSeeder seeder = new();

        seeder.SeedData(context, new Dictionary<string, string>());
        seeder.SeedData(context, new Dictionary<string, string>());

        foreach (string meritName in StockMeritsSeeder.StockMeritNamesForTesting)
        {
            Assert.AreEqual(1, context.Merits.Count(x => x.Name == meritName), $"Expected a single stock merit named {meritName}.");
        }

        foreach (string progName in StockMeritsSeeder.HelperProgNamesForTesting)
        {
            Assert.AreEqual(1, context.FutureProgs.Count(x => x.FunctionName == progName), $"Expected a single helper prog named {progName}.");
        }
    }

    [TestMethod]
    public void SeedData_SeedsReusablePlanarPlanesAndMerits()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context);
        StockMeritsSeeder seeder = new();

        seeder.SeedData(context, new Dictionary<string, string>());

        Plane prime = context.Planes.Single(x => x.Name == "Prime Material");
        Plane astral = context.Planes.Single(x => x.Name == "Astral Plane");
        Assert.IsTrue(prime.IsDefault);
        Assert.AreEqual("Astral Plane {0}", astral.RoomNameFormat);
        Assert.AreEqual("({0})", astral.RemoteObservationTag);
        Assert.IsFalse(string.IsNullOrWhiteSpace(astral.RoomDescriptionAddendum));
        foreach (string meritName in new[]
                 {
                     "Always Astral",
                     "Astral Manifestation",
                     "Astral Ignorant of Prime Material",
                     "Astral Visual Manifestation",
                     "Astral Sight",
                     "Dual Natured"
                 })
        {
            Assert.IsTrue(context.Merits.Any(x => x.Name == meritName && x.Type == "Planar State"),
                $"Expected stock planar merit {meritName}.");
        }

        XElement visualRoot = XElement.Parse(context.Merits.Single(x => x.Name == "Astral Visual Manifestation").Definition);
        XElement visualPlanarData = visualRoot.Element("PlanarData")!;
        Assert.IsTrue(PlanarElementContains(visualPlanarData.Element("VisibleTo"), prime.Id));
        Assert.IsTrue(PlanarElementContains(visualPlanarData.Element("VisibleTo"), astral.Id));
        XElement visualPhysical = visualPlanarData.Element("Interactions")!
                                                  .Elements("Interaction")
                                                  .Single(x => x.Attribute("kind")?.Value == PlanarInteractionKind.Physical.ToString());
        Assert.IsFalse(PlanarElementContains(visualPhysical, prime.Id));
        Assert.IsTrue(PlanarElementContains(visualPhysical, astral.Id));

        XElement dualRoot = XElement.Parse(context.Merits.Single(x => x.Name == "Dual Natured").Definition);
        XElement dualPlanarData = dualRoot.Element("PlanarData")!;
        Assert.IsTrue(PlanarElementContains(dualPlanarData.Element("Presence"), prime.Id));
        Assert.IsTrue(PlanarElementContains(dualPlanarData.Element("Presence"), astral.Id));
        XElement dualPhysical = dualPlanarData.Element("Interactions")!
                                              .Elements("Interaction")
                                              .Single(x => x.Attribute("kind")?.Value == PlanarInteractionKind.Physical.ToString());
        Assert.IsTrue(PlanarElementContains(dualPhysical, prime.Id));
        Assert.IsTrue(PlanarElementContains(dualPhysical, astral.Id));
    }

    [TestMethod]
    public void SeedData_MissingPerceptionAttribute_SkipsInapplicableTraitMeritAndStillCountsAsInstalled()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context, "Perception");
        StockMeritsSeeder seeder = new();

        seeder.SeedData(context, new Dictionary<string, string>());

        Assert.IsFalse(context.Merits.Any(x => x.Name == "Keen-Eyed"));
        Assert.IsTrue(context.Merits.Any(x => x.Name == "Weak-Willed"));
        Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, seeder.ShouldSeedData(context));
    }

    [TestMethod]
    public void SeedData_HelperProgsUseTerrainTagsRatherThanTerrainNames()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context);
        StockMeritsSeeder seeder = new();

        seeder.SeedData(context, new Dictionary<string, string>());

        List<string> terrainProgTexts = context.FutureProgs
            .Where(x => StockMeritsSeeder.HelperProgNamesForTesting.Contains(x.FunctionName) &&
                        x.FunctionName.StartsWith("StockMeritsIs", StringComparison.Ordinal) &&
                        !x.FunctionName.Equals("StockMeritsIsDark", StringComparison.Ordinal))
            .Select(x => x.FunctionText)
            .ToList();

        Assert.IsTrue(terrainProgTexts.Count > 0);
        Assert.IsTrue(terrainProgTexts.All(x => x.Contains("istagged(@ch.Location.Terrain,")));
        Assert.IsTrue(terrainProgTexts.All(x => !x.Contains("\"Beach\"")));
        Assert.IsTrue(terrainProgTexts.All(x => !x.Contains("\"City\"")));
        Assert.IsTrue(terrainProgTexts.All(x => !x.Contains("switch")));
    }

    [TestMethod]
    public void SeedData_NaturalAttackQualityExamplesExistForPunchKickAndBiteOnBothSides()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context);
        StockMeritsSeeder seeder = new();

        seeder.SeedData(context, new Dictionary<string, string>());

        foreach (MeleeWeaponVerb verb in new[] { MeleeWeaponVerb.Punch, MeleeWeaponVerb.Kick, MeleeWeaponVerb.Bite })
        {
            Assert.IsTrue(HasNaturalAttackExample(context, verb, MeritType.Merit), $"Expected a merit-side natural attack example for {verb}.");
            Assert.IsTrue(HasNaturalAttackExample(context, verb, MeritType.Flaw), $"Expected a flaw-side natural attack example for {verb}.");
        }
    }

    [TestMethod]
    public void SeedData_DoesNotAlterExistingChargenMeritStoryboard()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedPrerequisites(context);
        StockMeritsSeeder seeder = new();
        MudSharp.Models.ChargenScreenStoryboard storyboard = context.ChargenScreenStoryboards.Single();
        string originalStageDefinition = storyboard.StageDefinition;
        string originalChargenType = storyboard.ChargenType;
        int originalCount = context.ChargenScreenStoryboards.Count();

        seeder.SeedData(context, new Dictionary<string, string>());

        Assert.AreEqual(originalCount, context.ChargenScreenStoryboards.Count());
        Assert.AreEqual(originalChargenType, context.ChargenScreenStoryboards.Single().ChargenType);
        Assert.AreEqual(originalStageDefinition, context.ChargenScreenStoryboards.Single().StageDefinition);
    }
}
