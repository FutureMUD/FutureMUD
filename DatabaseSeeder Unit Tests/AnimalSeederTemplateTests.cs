#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Health;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AnimalSeederTemplateTests
{
    private static FuturemudDatabaseContext BuildContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new FuturemudDatabaseContext(options);
    }

    private static FuturemudDatabaseContext BuildExpandedAnimalCatalogueContext(
        bool includeBeetleBody = true,
        bool includeCentipedeBody = true,
        bool includeAcidSpit = true,
        string? omittedRaceName = null,
        bool beetleOnCorrectBody = true)
    {
        FuturemudDatabaseContext context = BuildContext();
        long nextBodyId = 1;
        long nextRaceId = 1;
        Dictionary<string, BodyProto> bodies = new(StringComparer.OrdinalIgnoreCase);

        void AddBody(string name)
        {
            if (bodies.ContainsKey(name))
            {
                return;
            }

            BodyProto body = new()
            {
                Id = nextBodyId++,
                Name = name,
                ConsiderString = string.Empty,
                WielderDescriptionSingle = "mouth",
                WielderDescriptionPlural = "mouths",
                WearSizeParameterId = 1,
                MinimumLegsToStand = 4,
                MinimumWingsToFly = 0,
                LegDescriptionSingular = "leg",
                LegDescriptionPlural = "legs",
                NameForTracking = name
            };
            bodies[name] = body;
            context.BodyProtos.Add(body);
        }

        AddBody("Humanoid");
        AddBody("Quadruped Base");

        foreach (string bodyKey in AnimalSeeder.RaceTemplatesForTesting.Values
                     .Select(x => x.BodyKey)
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if ((!includeBeetleBody && bodyKey == "Beetle") ||
                (!includeCentipedeBody && bodyKey == "Centipede"))
            {
                continue;
            }

            AddBody(bodyKey);
        }

        context.NameCultures.Add(new NameCulture
        {
            Name = "Simple",
            Definition = "<NameCulture />"
        });

        foreach (string modelName in AnimalSeeder.HeightWeightTemplatesForTesting.Keys)
        {
            context.HeightWeightModels.Add(new HeightWeightModel
            {
                Name = modelName,
                MeanHeight = 100,
                MeanBmi = 20,
                StddevHeight = 10,
                StddevBmi = 2,
                Bmimultiplier = 1.0
            });
        }

        if (includeAcidSpit)
        {
            context.WeaponAttacks.Add(new MudSharp.Models.WeaponAttack
            {
                Name = "Acid Spit",
                AdditionalInfo = string.Empty,
                RequiredPositionStateIds = string.Empty
            });
        }

        BodyProto fallbackBody = bodies.TryGetValue("Insectoid", out BodyProto? insectoidBody)
            ? insectoidBody
            : bodies["Quadruped Base"];

        foreach ((string raceName, AnimalSeeder.AnimalRaceTemplate template) in AnimalSeeder.RaceTemplatesForTesting)
        {
            if (string.Equals(raceName, omittedRaceName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            BodyProto baseBody = raceName == "Beetle" && !beetleOnCorrectBody
                ? fallbackBody
                : bodies.GetValueOrDefault(template.BodyKey, fallbackBody);

            context.Races.Add(new Race
            {
                Id = nextRaceId++,
                Name = raceName,
                Description = $"{raceName} test race",
                BaseBodyId = baseBody.Id,
                AllowedGenders = "1 2 3 4",
                AttributeBonusProgId = 1,
                AttributeTotalCap = 1,
                IndividualAttributeCap = 1,
                DiceExpression = "1",
                IlluminationPerceptionMultiplier = 1.0,
                CorpseModelId = 1,
                DefaultHealthStrategyId = 1,
                CanUseWeapons = false,
                CanAttack = true,
                CanDefend = true,
                NeedsToBreathe = true,
                BreathingModel = "simple",
                SizeStanding = 1,
                SizeProne = 1,
                SizeSitting = 1,
                CommunicationStrategyType = "humanoid",
                DefaultHandedness = 1,
                HandednessOptions = "1",
                MaximumDragWeightExpression = "1",
                MaximumLiftWeightExpression = "1",
                RaceUsesStamina = true,
                CanEatCorpses = false,
                BiteWeight = 1.0,
                EatCorpseEmoteText = string.Empty,
                CanEatMaterialsOptIn = false,
                TemperatureRangeFloor = 0,
                TemperatureRangeCeiling = 40,
                BodypartSizeModifier = 0,
                BodypartHealthMultiplier = 1.0,
                BreathingVolumeExpression = "1",
                HoldBreathLengthExpression = "1",
                CanClimb = false,
                CanSwim = true,
                MinimumSleepingPosition = 1,
                ChildAge = 1,
                YouthAge = 2,
                YoungAdultAge = 3,
                AdultAge = 4,
                ElderAge = 5,
                VenerableAge = 6
            });
        }

        context.SaveChanges();
        return context;
    }

    private static int ParagraphCount(string text)
    {
        return text
            .Split(["\r\n\r\n", "\n\n"], System.StringSplitOptions.RemoveEmptyEntries)
            .Length;
    }

    [TestMethod]
    public void ValidateTemplateCatalogForTesting_CurrentCatalog_HasNoIssues()
    {
        IReadOnlyList<string> issues = AnimalSeeder.ValidateTemplateCatalogForTesting();
        Assert.AreEqual(0, issues.Count, string.Join("\n", issues));
    }

    [TestMethod]
    public void RaceTemplatesForTesting_WeaselAndRhinocerous_UseExpectedBodyAssignments()
    {
        AnimalSeeder.AnimalRaceTemplate weasel = AnimalSeeder.RaceTemplatesForTesting["Weasel"];
        Assert.AreEqual("Toed Quadruped", weasel.BodyKey, "Weasel should use the toed quadruped body.");

        AnimalSeeder.AnimalRaceTemplate rhino = AnimalSeeder.RaceTemplatesForTesting["Rhinocerous"];
        Assert.AreEqual("Ungulate", rhino.BodyKey, "Rhinocerous should continue to use the ungulate-derived body.");
        Assert.IsNotNull(rhino.AdditionalBodypartUsages, "Rhinocerous should define horn bodypart usages.");
        Assert.IsTrue(
            rhino.AdditionalBodypartUsages!.Any(x => x.BodypartAlias == "horn" && x.Usage == "general"),
            "Rhinocerous should expose the shared horn usage.");
    }

    [TestMethod]
    public void RaceTemplatesForTesting_ExpandedCatalogueEntries_UseExpectedBodyAssignments()
    {
        Assert.AreEqual("Beetle", AnimalSeeder.RaceTemplatesForTesting["Beetle"].BodyKey,
            "Beetles should now use their dedicated beetle body.");
        Assert.AreEqual("Centipede", AnimalSeeder.RaceTemplatesForTesting["Centipede"].BodyKey,
            "Centipedes should use the dedicated centipede body.");
        Assert.AreEqual("Insectoid", AnimalSeeder.RaceTemplatesForTesting["Mantis"].BodyKey,
            "Mantises should continue to use the shared insectoid body.");
        Assert.AreEqual("Ungulate", AnimalSeeder.RaceTemplatesForTesting["Mammoth"].BodyKey,
            "Mammoths should reuse the ungulate body.");
        Assert.AreEqual("Toed Quadruped", AnimalSeeder.RaceTemplatesForTesting["Sabretooth Tiger"].BodyKey,
            "Sabretooth tigers should reuse the toed quadruped body.");
    }

    [TestMethod]
    public void RaceTemplatesForTesting_CombatStrategyKeys_MapRepresentativeAnimalsToExpectedStyles()
    {
        Assert.AreEqual("Beast Coward", AnimalSeeder.RaceTemplatesForTesting["Rabbit"].CombatStrategyKey);
        Assert.AreEqual("Beast Skirmisher", AnimalSeeder.RaceTemplatesForTesting["Cheetah"].CombatStrategyKey);
        Assert.AreEqual("Beast Artillery", AnimalSeeder.RaceTemplatesForTesting["Llama"].CombatStrategyKey);
        Assert.AreEqual("Beast Swooper", AnimalSeeder.RaceTemplatesForTesting["Eagle"].CombatStrategyKey);
        Assert.AreEqual("Beast Clincher", AnimalSeeder.RaceTemplatesForTesting["Python"].CombatStrategyKey);
        Assert.AreEqual("Beast Behemoth", AnimalSeeder.RaceTemplatesForTesting["Elephant"].CombatStrategyKey);
    }

    [TestMethod]
    public void StockDescriptions_RaceEthnicityAndCulture_AreThreeParagraphsAndRepresentative()
    {
        string dogRaceDescription = AnimalSeeder.BuildRaceDescriptionForTesting(AnimalSeeder.RaceTemplatesForTesting["Dog"]);
        Assert.AreEqual(3, ParagraphCount(dogRaceDescription));
        StringAssert.Contains(dogRaceDescription, "Adults are most often represented");

        string dogEthnicityDescription = AnimalSeeder.BuildEthnicityDescriptionForTesting("Dog", "Retriever");
        Assert.AreEqual(3, ParagraphCount(dogEthnicityDescription));
        StringAssert.Contains(dogEthnicityDescription, "downed game");

        string bearEthnicityDescription = AnimalSeeder.BuildEthnicityDescriptionForTesting("Bear", "Brown Bear");
        Assert.AreEqual(3, ParagraphCount(bearEthnicityDescription));
        StringAssert.Contains(bearEthnicityDescription, "hump-backed shoulder power");

        Assert.AreEqual(3, ParagraphCount(AnimalSeeder.AnimalCultureDescriptionForTesting));
        StringAssert.Contains(AnimalSeeder.AnimalCultureDescriptionForTesting, "instinct, territory, routine");
    }

    [TestMethod]
    public void AttackLoadoutsForTesting_NewAnimalGroups_HaveExpectedAttacks()
    {
        AnimalSeeder.AnimalAttackLoadoutTemplate birdLoadout = AnimalSeeder.AttackLoadoutsForTesting["bird-small"];
        CollectionAssert.AreEquivalent(
            new[] { "beakpeck", "talonstrike", "beakbite" },
            birdLoadout.ShapeMatchedAttacks.Select(x => x.AttackKey).ToArray(),
            "Small birds should keep both free-fighting and clinch-capable avian attacks.");

        AnimalSeeder.AnimalAttackLoadoutTemplate insectLoadout = AnimalSeeder.AttackLoadoutsForTesting["insect-stinger"];
        Assert.IsTrue(insectLoadout.ShapeMatchedAttacks.Any(x => x.AttackKey == "mandiblebite"),
            "Stinging insects should still have a mundane mandible attack.");
        Assert.IsTrue(insectLoadout.ShapeMatchedAttacks.Any(x => x.AttackKey == "headram"),
            "Stinging insects should now have a non-clinch fallback attack.");
        Assert.IsTrue(insectLoadout.VenomAttacks?.Any(x => x.VenomProfileKey == "irritant") == true,
            "Stinging insects should seed irritant venom.");

        AnimalSeeder.AnimalAttackLoadoutTemplate whaleLoadout = AnimalSeeder.AttackLoadoutsForTesting["baleen-whale"];
        Assert.IsTrue(whaleLoadout.ShapeMatchedAttacks.Any(x => x.AttackKey == "headbutt"),
            "Baleen whales should now have a clinch-capable fallback attack.");
        Assert.IsTrue(whaleLoadout.AliasAttacks?.Any(x => x.AttackKey == "headram") == true,
            "Baleen whales should have a head ram attack.");
        Assert.IsTrue(whaleLoadout.AliasAttacks?.Any(x => x.AttackKey == "tailslap") == true,
            "Baleen whales should have a tail slap attack.");
    }

    [TestMethod]
    public void AttackLoadoutsForTesting_AllStockLoadouts_HaveClinchAndNonClinchCoverage()
    {
        foreach ((string key, AnimalSeeder.AnimalAttackLoadoutTemplate loadout) in AnimalSeeder.AttackLoadoutsForTesting)
        {
            List<string> attackKeys = loadout.ShapeMatchedAttacks
                .Select(x => x.AttackKey)
                .Concat(loadout.AliasAttacks?.Select(x => x.AttackKey) ?? Enumerable.Empty<string>())
                .ToList();
            bool hasClinch = attackKeys.Any(x => x is "bite" or "carnivoreclinchbite" or "carnivoreclinchhighbite" or
                "carnivoreclinchhighestbite" or "herbivorebite" or "smallbite" or "smalllowbite" or "fishbite" or
                "fishquickbite" or "headbutt" or "beakbite" or "fangbite" or "mandiblebite" or "clawclamp") ||
                            (loadout.VenomAttacks?.Any(x => x.MoveType == BuiltInCombatMoveType.EnvenomingAttackClinch) ?? false);
            bool hasNonClinch = attackKeys.Any(x => x is "carnivorebite" or "carnivoresmashbite" or "carnivorelowbite" or
                "carnivorehighbite" or "carnivorelowestbite" or "carnivoredownbite" or "herbivoresmashbite" or
                "smallsmashbite" or "smalldownedbite" or "clawswipe" or "clawsmashswipe" or "clawlowswipe" or
                "clawhighswipe" or "hoofstomp" or "hoofstompsmash" or "barge" or "bargesmash" or "clinchbarge" or
                "gorehorn" or "goreantler" or "goretusk" or "tusksweep" or "crabpinch" or "sharkbite" or
                "sharkreelbite" or "beakpeck" or "talonstrike" or "arachnidclaw" or "headram" or "tailslap" or
                "tendrillash") ||
                           (loadout.VenomAttacks?.Any(x => x.MoveType != BuiltInCombatMoveType.EnvenomingAttackClinch) ?? false);

            Assert.IsTrue(hasClinch, $"Attack loadout {key} should include a clinch-capable attack.");
            Assert.IsTrue(hasNonClinch, $"Attack loadout {key} should include a non-clinch attack.");
        }
    }

    [TestMethod]
    public void BodyAuditProfilesForTesting_NewFamilies_HaveExpectedCoverageRules()
    {
        AnimalSeeder.AnimalBodyAuditProfile avian = AnimalSeeder.BodyAuditProfilesForTesting["avian"];
        CollectionAssert.Contains(avian.RequiredBones.ToList(), "rhumerus");
        CollectionAssert.Contains(avian.RequiredLimbs.ToList(), "Right Wing");

        AnimalSeeder.AnimalBodyAuditProfile scorpion = AnimalSeeder.BodyAuditProfilesForTesting["scorpion"];
        CollectionAssert.Contains(scorpion.RequiredBodyparts.ToList(), "stinger");
        Assert.AreEqual(AnimalSeeder.AnimalBoneExpectation.Forbidden, scorpion.BoneExpectation,
            "Scorpions should not require bones.");

        AnimalSeeder.AnimalBodyAuditProfile cephalopod = AnimalSeeder.BodyAuditProfilesForTesting["cephalopod"];
        CollectionAssert.Contains(cephalopod.RequiredBodyparts.ToList(), "arm8");
        Assert.AreEqual(AnimalSeeder.AnimalBoneExpectation.Forbidden, cephalopod.BoneExpectation,
            "Cephalopods should not require bones.");

        AnimalSeeder.AnimalBodyAuditProfile serpent = AnimalSeeder.BodyAuditProfilesForTesting["serpent"];
        CollectionAssert.Contains(serpent.RequiredBones.ToList(), "cavertebrae");
        CollectionAssert.Contains(serpent.RequiredLimbs.ToList(), "Tail");

        AnimalSeeder.AnimalBodyAuditProfile fish = AnimalSeeder.BodyAuditProfilesForTesting["fish"];
        CollectionAssert.Contains(fish.RequiredBodyparts.ToList(), "rgill");
        CollectionAssert.Contains(fish.RequiredBones.ToList(), "cavertebrae");
    }

    [TestMethod]
    public void BodyAuditProfilesAndHeightWeightTemplatesForTesting_ExpandedCatalogueEntries_ArePresent()
    {
        CollectionAssert.Contains(AnimalSeeder.HeightWeightTemplatesForTesting.Keys.ToList(), "Centipede");
        CollectionAssert.Contains(AnimalSeeder.HeightWeightTemplatesForTesting.Keys.ToList(), "Giant Centipede");
        CollectionAssert.Contains(AnimalSeeder.HeightWeightTemplatesForTesting.Keys.ToList(), "Giant Insect");

        AnimalSeeder.AnimalBodyAuditProfile beetle = AnimalSeeder.BodyAuditProfilesForTesting["beetle"];
        CollectionAssert.Contains(beetle.RequiredBodyparts.ToList(), "mandibles");
        Assert.AreEqual(AnimalSeeder.AnimalBoneExpectation.Forbidden, beetle.BoneExpectation,
            "Beetles should continue to forbid bones.");

        AnimalSeeder.AnimalBodyAuditProfile centipede = AnimalSeeder.BodyAuditProfilesForTesting["centipede"];
        CollectionAssert.Contains(centipede.RequiredBodyparts.ToList(), "midbody");
        CollectionAssert.Contains(centipede.RequiredLimbs.ToList(), "Left Leg 6");
        Assert.AreEqual(AnimalSeeder.AnimalBoneExpectation.Forbidden, centipede.BoneExpectation,
            "Centipedes should forbid bones.");
    }

    [TestMethod]
    public void RaceTemplatesForTesting_DefaultDisfigurementHooks_AreEmptyByDefault()
    {
        Assert.IsTrue(
            AnimalSeeder.RaceTemplatesForTesting.Values.All(x => x.TattooTemplates is null || x.TattooTemplates.Count == 0),
            "Animal stock templates should default to no tattoo templates until a later content pass fills them in.");
    }

    [TestMethod]
    public void BuildAuditPartLookup_DuplicateInheritedAliases_PrefersMostSpecificBody()
    {
        List<BodypartProto> parts = new()
        {
            new() { BodyId = 2, Name = "carapace", Description = "base carapace" },
            new() { BodyId = 1, Name = "carapace", Description = "derived carapace" },
            new() { BodyId = 2, Name = "mouth", Description = "base mouth" }
        };

        IReadOnlyDictionary<string, BodypartProto> lookup = AnimalSeeder.BuildAuditPartLookup(new long[] { 1, 2 }, parts);

        Assert.AreEqual("derived carapace", lookup["carapace"].Description);
        Assert.AreEqual("base mouth", lookup["mouth"].Description);
    }

    [TestMethod]
    public void GetAvianSpinalOrganAliasForLimb_KnownLimbNames_ReturnExpectedMappings()
    {
        Assert.AreEqual("uspinalcord", AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Torso"));
        Assert.AreEqual("mspinalcord", AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Right Wing"));
        Assert.AreEqual("mspinalcord", AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Left Wing"));
        Assert.AreEqual("lspinalcord", AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Left Leg"));
        Assert.AreEqual("lspinalcord", AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Tail"));
        Assert.IsNull(AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Unknown Limb"));
    }

    [TestMethod]
    public void VenomProfilesForTesting_SeededProfiles_HaveExpectedEffects()
    {
        AnimalSeeder.AnimalVenomProfileTemplate neurotoxic = AnimalSeeder.VenomProfilesForTesting["neurotoxic"];
        Assert.IsTrue(neurotoxic.Effects.Any(x => x.DrugType == DrugType.Paralysis),
            "Neurotoxic venom should inflict paralysis.");

        AnimalSeeder.AnimalVenomProfileTemplate mixed = AnimalSeeder.VenomProfilesForTesting["mixed"];
        Assert.IsTrue(mixed.Effects.Any(x => x.DrugType == DrugType.BodypartDamage),
            "Mixed venom should include organ or bodypart damage.");

        foreach ((string _, AnimalSeeder.AnimalVenomProfileTemplate profile) in AnimalSeeder.VenomProfilesForTesting)
        {
            Assert.IsTrue(profile.Effects.Count > 0, $"Venom profile {profile.Key} should have at least one effect.");
        }
    }

    [TestMethod]
    public void ShouldSeedData_ExistingCatalogueMissingExpandedBodiesOrRace_ReturnsExtraPackagesAvailable()
    {
        using FuturemudDatabaseContext context = BuildExpandedAnimalCatalogueContext(
            includeBeetleBody: false,
            includeCentipedeBody: false,
            omittedRaceName: "Mammoth");

        Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, new AnimalSeeder().ShouldSeedData(context),
            "Rerunnable animal seeders should advertise the expanded catalogue when beetle and centipede bodies or new races are missing.");
    }

    [TestMethod]
    public void ShouldSeedData_ExistingCatalogueWithBeetleStillUsingInsectoid_ReturnsExtraPackagesAvailable()
    {
        using FuturemudDatabaseContext context = BuildExpandedAnimalCatalogueContext(beetleOnCorrectBody: false);

        Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, new AnimalSeeder().ShouldSeedData(context),
            "A legacy beetle race that still points at the insectoid body should trigger the rerun path.");
    }

    [TestMethod]
    public void ShouldSeedData_ExpandedCatalogueAlreadyPresent_ReturnsMayAlreadyBeInstalled()
    {
        using FuturemudDatabaseContext context = BuildExpandedAnimalCatalogueContext();

        Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, new AnimalSeeder().ShouldSeedData(context),
            "Once the expanded animal catalogue is present, the seeder should stop advertising extra stock content.");
    }
}
