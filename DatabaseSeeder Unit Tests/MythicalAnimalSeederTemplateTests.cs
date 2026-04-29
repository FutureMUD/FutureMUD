using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MythicalAnimalSeederTemplateTests
{
    private static int ParagraphCount(string text)
    {
        return text
            .Split(["\r\n\r\n", "\n\n"], StringSplitOptions.RemoveEmptyEntries)
            .Length;
    }

    private static FuturemudDatabaseContext BuildContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new FuturemudDatabaseContext(options);
    }

    private static MudSharp.Models.FutureProg CreateAnimalAiProg(long id, string name, ProgVariableTypes returnType, string text)
    {
        return new MudSharp.Models.FutureProg
        {
            Id = id,
            FunctionName = name,
            FunctionComment = $"{name} test prog",
            FunctionText = text,
            ReturnType = (long)returnType,
            Category = "Tests",
            Subcategory = "MythicalAnimalAI",
            Public = true,
            AcceptsAnyParameters = true,
            StaticType = (int)FutureProgStaticType.FullyStatic
        };
    }

    private static void SeedAnimalAiPrerequisiteProgs(FuturemudDatabaseContext context)
    {
        context.FutureProgs.AddRange(
            CreateAnimalAiProg(1, "AlwaysTrue", ProgVariableTypes.Boolean, "return true"),
            CreateAnimalAiProg(2, "AlwaysFalse", ProgVariableTypes.Boolean, "return false"),
            CreateAnimalAiProg(3, "AlwaysOne", ProgVariableTypes.Number, "return 1"));
        context.SaveChanges();
    }

    private static void AssertSatiationCadence(
        (double MaximumFoodSatiatedHours, double MaximumDrinkSatiatedHours) limits,
        double expectedFoodHours,
        double expectedDrinkHours)
    {
        const double thresholdFraction = 0.75;
        Assert.AreEqual(expectedFoodHours / thresholdFraction, limits.MaximumFoodSatiatedHours, 0.0001,
            "Food satiation maxima should preserve the intended cadence before starvation.");
        Assert.AreEqual(expectedDrinkHours / thresholdFraction, limits.MaximumDrinkSatiatedHours, 0.0001,
            "Drink satiation maxima should preserve the intended cadence before becoming parched.");
    }

    [TestMethod]
    public void ValidateTemplateCatalogForTesting_CurrentCatalog_HasNoIssues()
    {
        IReadOnlyList<string> issues = MythicalAnimalSeeder.ValidateTemplateCatalogForTesting();
        Assert.AreEqual(0, issues.Count, string.Join("\n", issues));
    }

    [TestMethod]
    public void TemplatesForTesting_ExpandedCatalogue_HasFortyEntries()
    {
        Assert.AreEqual(40, MythicalAnimalSeeder.TemplatesForTesting.Count,
            "The mythical catalogue should include worm-beasts, tree-spirits, giant arthropods, and non-European mythic beasts.");
    }

    [TestMethod]
    public void MythicalAnimalAIRecommendations_CoverEverySeededMythicalRace()
    {
        IReadOnlyDictionary<string, string> recommendations =
            MythicalAnimalSeeder.MythicalAnimalAIRecommendationsForTesting;
        List<string> expectedRaceNames = MythicalAnimalSeeder.TemplatesForTesting.Keys
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToList();
        List<string> actualRaceNames = recommendations.Keys
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToList();

        CollectionAssert.AreEqual(expectedRaceNames, actualRaceNames,
            "Every stock mythical race should have exactly one recommended individual AnimalAI template.");

        HashSet<string> knownTemplateNames = AnimalSeeder.StockAnimalAITemplatesForTesting
            .Select(x => x.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (string templateName in recommendations.Values.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            Assert.IsTrue(knownTemplateNames.Contains(templateName),
                $"Mythical animal AI recommendation {templateName} must point to a seeded template.");
        }
    }

    [TestMethod]
    public void TemplatesForTesting_StockDiets_UseSeededTerrainYieldTypes()
    {
        HashSet<string> stockTerrainYields = CoreDataSeeder.StockTerrainForageYieldTypesForTesting
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (string raceName in MythicalAnimalSeeder.TemplatesForTesting.Keys)
        {
            IReadOnlyCollection<string> edibleYields = MythicalAnimalSeeder.GetEdibleYieldTypesForTesting(raceName);
            Assert.IsTrue(edibleYields.Count > 0, $"{raceName} should have at least one stock edible forage yield.");
            CollectionAssert.IsSubsetOf(edibleYields.ToArray(), stockTerrainYields.ToArray(),
                $"{raceName} should only reference forage yields seeded by CoreDataSeeder.Terrain.");
        }
    }

    [TestMethod]
    public void SeedMythicalAnimalAIStockTemplates_RerunRestoresMissingTemplatesWithoutDuplicates()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedAnimalAiPrerequisiteProgs(context);

        MythicalAnimalSeeder.SeedMythicalAnimalAIStockTemplatesForTesting(context);
        Assert.AreEqual(MythicalAnimalSeeder.StockMythicalAnimalAITemplateNamesForTesting.Count,
            context.ArtificialIntelligences.Count(),
            "Mythical animal seeder should install one AI row per stock mythical recommendation template.");

        ArtificialIntelligence removed = context.ArtificialIntelligences
            .First(x => x.Name == MythicalAnimalSeeder.StockMythicalAnimalAITemplateNamesForTesting.First());
        context.ArtificialIntelligences.Remove(removed);
        context.SaveChanges();

        MythicalAnimalSeeder.SeedMythicalAnimalAIStockTemplatesForTesting(context);

        foreach (string name in MythicalAnimalSeeder.StockMythicalAnimalAITemplateNamesForTesting)
        {
            ArtificialIntelligence ai = context.ArtificialIntelligences.Single(x => x.Name == name);
            Assert.AreEqual("Animal", ai.Type);
            Assert.AreEqual("Definition", XElement.Parse(ai.Definition).Name.LocalName);
        }
    }

    [TestMethod]
    public void TemplatesForTesting_RepresentativeDiets_MatchRacePhysiology()
    {
        CollectionAssert.Contains(MythicalAnimalSeeder.GetEdibleYieldTypesForTesting("Dragon").ToArray(), "tiny-fish",
            "Dragons should have stock small-prey forage options.");
        CollectionAssert.Contains(MythicalAnimalSeeder.GetEdibleYieldTypesForTesting("Unicorn").ToArray(), "grass",
            "Unicorns should graze like other equine herbivores.");
        CollectionAssert.Contains(MythicalAnimalSeeder.GetEdibleYieldTypesForTesting("Mermaid").ToArray(), "tiny-fish",
            "Merfolk should have aquatic animal forage available.");
        CollectionAssert.Contains(MythicalAnimalSeeder.GetEdibleYieldTypesForTesting("Myconid").ToArray(), "mushrooms",
            "Myconids should eat fungal forage.");
        CollectionAssert.Contains(MythicalAnimalSeeder.GetEdibleYieldTypesForTesting("Plantfolk").ToArray(), "aquatic-plants",
            "Plantfolk should use the broader plant-matter forage profile.");
        CollectionAssert.Contains(MythicalAnimalSeeder.GetEdibleYieldTypesForTesting("Qilin").ToArray(), "grass",
            "Qilin should browse and graze like sacred ungulate beasts.");
        CollectionAssert.Contains(MythicalAnimalSeeder.GetEdibleYieldTypesForTesting("Bunyip").ToArray(), "tiny-fish",
            "Bunyips should have wetland prey forage available.");

        Assert.IsTrue(MythicalAnimalSeeder.CanEatCorpsesForTesting("Dragon"),
            "Apex carnivorous mythic beasts should be corpse eaters.");
        Assert.IsTrue(MythicalAnimalSeeder.CanEatCorpsesForTesting("Yacumama"),
            "Giant river-serpent myths should be corpse-eating predators.");
        Assert.IsTrue(MythicalAnimalSeeder.CanEatCorpsesForTesting("Giant Spider"),
            "Giant predatory arthropods should be corpse eaters.");
        Assert.IsFalse(MythicalAnimalSeeder.CanEatCorpsesForTesting("Unicorn"),
            "Herbivorous mythic beasts should not be corpse eaters.");
    }

    [TestMethod]
    public void MythicalDietMath_PerBiteSatiation_StaysBelowNeedCaps()
    {
        foreach ((string Race, SizeCategory Size) in new[]
                 {
                     ("Dragon", SizeCategory.VeryLarge),
                     ("Giant Ant", SizeCategory.Normal),
                     ("Unicorn", SizeCategory.Large),
                     ("Myconid", SizeCategory.Normal)
                 })
        {
            double biteYield = NonHumanForageDietSeederHelper.ForageYieldPerBiteForTesting(Size);
            foreach (StockForageYieldEdibility yield in NonHumanForageDietSeederHelper.GetYieldEdibilitiesForTesting(
                         MythicalAnimalSeeder.GetDietProfilesForTesting(Race), Size))
            {
                Assert.IsTrue(yield.HungerMultiplier * biteYield <= NonHumanForageDietSeederHelper.MaximumFoodSatiationHours,
                    $"{Race} should not be able to exceed maximum satiation from one bite of {yield.YieldType}.");
            }
        }
    }

    [TestMethod]
    public void BuildBodypartAliasLookup_DuplicateAliases_GroupsAndOrdersDeterministically()
    {
        BodypartProto[] parts = new[]
        {
            new BodypartProto { Id = 3, Name = "tail", DisplayOrder = 20 },
            new BodypartProto { Id = 2, Name = "tail", DisplayOrder = 10 },
            new BodypartProto { Id = 1, Name = "tail", DisplayOrder = 10 },
            new BodypartProto { Id = 4, Name = "head", DisplayOrder = 5 }
        };

        IReadOnlyDictionary<string, IReadOnlyList<BodypartProto>> groupedLookup = SeederBodyUtilities.BuildBodypartAliasLookup(parts);
        CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, groupedLookup["tail"].Select(x => x.Id).ToArray(),
            "Duplicate aliases should be retained in stable display-order then id order.");

        IReadOnlyDictionary<string, BodypartProto> lookup = SeederBodyUtilities.BuildBodypartLookup(parts);
        Assert.AreEqual(1L, lookup["tail"].Id,
            "Single-part lookups should resolve duplicate aliases to the earliest stable entry.");
    }

    [TestMethod]
    public void GetEffectiveBodyparts_OverrideAndRemoval_UsesMostSpecificRemainingParts()
    {
        using FuturemudDatabaseContext context = BuildContext();
        BodyProto parent = new()
        {
            Id = 10,
            Name = "Humanoid",
            ConsiderString = "a humanoid",
            WielderDescriptionSingle = "hand",
            WielderDescriptionPlural = "hands",
            LegDescriptionSingular = "leg",
            LegDescriptionPlural = "legs"
        };
        BodyProto child = new()
        {
            Id = 11,
            Name = "Robot Humanoid",
            CountsAs = parent,
            ConsiderString = "a robot",
            WielderDescriptionSingle = "hand",
            WielderDescriptionPlural = "hands",
            LegDescriptionSingular = "leg",
            LegDescriptionPlural = "legs"
        };
        context.BodyProtos.AddRange(parent, child);
        context.SaveChanges();

        BodypartProto parentTorso = new() { Id = 100, Body = parent, BodyId = parent.Id, Name = "torso", Description = "torso", BodypartType = 0, IsOrgan = 0, DisplayOrder = 1 };
        BodypartProto parentHand = new() { Id = 101, Body = parent, BodyId = parent.Id, Name = "rhand", Description = "right hand", BodypartType = 0, IsOrgan = 0, DisplayOrder = 2 };
        BodypartProto childTorso = new() { Id = 110, Body = child, BodyId = child.Id, Name = "torso", Description = "robot torso", BodypartType = 0, IsOrgan = 0, DisplayOrder = 1, CountAs = parentTorso };
        BodypartProto childHand = new() { Id = 111, Body = child, BodyId = child.Id, Name = "rhand", Description = "robot hand", BodypartType = 0, IsOrgan = 0, DisplayOrder = 2, CountAs = parentHand };
        context.BodypartProtos.AddRange(parentTorso, parentHand, childTorso, childHand);
        context.SaveChanges();

        context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
        {
            ChildNavigation = parentHand,
            ParentNavigation = parentTorso
        });
        context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
        {
            ChildNavigation = childHand,
            ParentNavigation = childTorso
        });
        context.BodyProtosAdditionalBodyparts.Add(new BodyProtosAdditionalBodyparts
        {
            BodyProto = child,
            Bodypart = childHand,
            Usage = "remove"
        });
        context.SaveChanges();

        IReadOnlyList<BodypartProto> effectiveParts = SeederBodyUtilities.GetEffectiveBodyparts(context, child);
        CollectionAssert.AreEqual(new[] { "torso" }, effectiveParts.Select(x => x.Name).ToArray(),
            "Effective body composition should keep the child torso override and remove the inherited hand subtree.");
        Assert.AreEqual(childTorso.Id, effectiveParts.Single().Id,
            "The most specific child override should replace the inherited parent bodypart.");
    }

    [TestMethod]
    public void CloneBodypartSubtree_TargetParentOnAncestor_AttachesToInheritedParent()
    {
        using FuturemudDatabaseContext context = BuildContext();
        BodyProto parent = new()
        {
            Id = 20,
            Name = "Organic Humanoid",
            ConsiderString = "a humanoid",
            WielderDescriptionSingle = "hand",
            WielderDescriptionPlural = "hands",
            LegDescriptionSingular = "leg",
            LegDescriptionPlural = "legs"
        };
        BodyProto child = new()
        {
            Id = 21,
            Name = "Winged Humanoid",
            CountsAs = parent,
            ConsiderString = "a winged humanoid",
            WielderDescriptionSingle = "hand",
            WielderDescriptionPlural = "hands",
            LegDescriptionSingular = "leg",
            LegDescriptionPlural = "legs"
        };
        BodyProto source = new()
        {
            Id = 22,
            Name = "Avian",
            ConsiderString = "an avian",
            WielderDescriptionSingle = "claw",
            WielderDescriptionPlural = "claws",
            LegDescriptionSingular = "leg",
            LegDescriptionPlural = "legs"
        };
        context.BodyProtos.AddRange(parent, child, source);
        context.SaveChanges();

        BodypartProto parentBack = new() { Id = 200, Body = parent, BodyId = parent.Id, Name = "uback", Description = "upper back", BodypartType = 0, IsOrgan = 0, DisplayOrder = 1 };
        BodypartProto sourceBack = new() { Id = 201, Body = source, BodyId = source.Id, Name = "uback", Description = "avian upper back", BodypartType = 0, IsOrgan = 0, DisplayOrder = 1 };
        BodypartProto wingBase = new() { Id = 202, Body = source, BodyId = source.Id, Name = "rwingbase", Description = "right wing base", BodypartType = 0, IsOrgan = 0, DisplayOrder = 2 };
        BodypartProto wing = new() { Id = 203, Body = source, BodyId = source.Id, Name = "rwing", Description = "right wing", BodypartType = 0, IsOrgan = 0, DisplayOrder = 3 };
        context.BodypartProtos.AddRange(parentBack, sourceBack, wingBase, wing);
        context.SaveChanges();

        context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
        {
            ChildNavigation = wingBase,
            ParentNavigation = sourceBack
        });
        context.BodypartProtoBodypartProtoUpstream.Add(new BodypartProtoBodypartProtoUpstream
        {
            ChildNavigation = wing,
            ParentNavigation = wingBase
        });
        context.SaveChanges();

        IReadOnlyDictionary<long, BodypartProto> clonedParts = SeederBodyUtilities.CloneBodypartSubtree(context, source, child, "rwingbase", "uback");
        BodypartProto clonedWingBase = clonedParts[wingBase.Id];
        BodypartProtoBodypartProtoUpstream upstream = context.BodypartProtoBodypartProtoUpstream.Single(x => x.Child == clonedWingBase.Id);

        Assert.AreEqual(parentBack.Id, upstream.Parent,
            "Cloned subtrees should be able to attach to a parent bodypart inherited through the CountsAs chain.");
    }

    [TestMethod]
    public void GetExternalBodypartsWithoutLimbCoverage_MissingHornMembership_ReturnsUncoveredHorn()
    {
        using FuturemudDatabaseContext context = BuildContext();
        BodyProto body = new()
        {
            Id = 1,
            Name = "Horned Humanoid",
            ConsiderString = "a horned humanoid",
            WielderDescriptionSingle = "hand",
            WielderDescriptionPlural = "hands",
            LegDescriptionSingular = "leg",
            LegDescriptionPlural = "legs"
        };
        BodypartProto neck = new()
        {
            Id = 10,
            Body = body,
            BodyId = body.Id,
            Name = "neck",
            Description = "neck",
            BodypartType = 0,
            IsOrgan = 0
        };
        BodypartProto horn = new()
        {
            Id = 11,
            Body = body,
            BodyId = body.Id,
            Name = "rhorn",
            Description = "right horn",
            BodypartType = 0,
            IsOrgan = 0
        };
        Limb headLimb = new()
        {
            Id = 100,
            Name = "Head",
            RootBody = body,
            RootBodyId = body.Id,
            RootBodypart = neck,
            RootBodypartId = neck.Id
        };

        context.BodyProtos.Add(body);
        context.BodypartProtos.AddRange(neck, horn);
        context.Limbs.Add(headLimb);
        context.LimbsBodypartProto.Add(new LimbBodypartProto
        {
            Limb = headLimb,
            LimbId = headLimb.Id,
            BodypartProto = neck,
            BodypartProtoId = neck.Id
        });
        context.SaveChanges();

        IReadOnlyList<BodypartProto> uncoveredParts = SeederBodyUtilities.GetExternalBodypartsWithoutLimbCoverage(context, body);
        CollectionAssert.AreEqual(new[] { "rhorn" }, uncoveredParts.Select(x => x.Name).ToArray(),
            "Unattached horn bodyparts should be flagged by the seeder audit.");

        context.LimbsBodypartProto.Add(new LimbBodypartProto
        {
            Limb = headLimb,
            LimbId = headLimb.Id,
            BodypartProto = horn,
            BodypartProtoId = horn.Id
        });
        context.SaveChanges();

        Assert.AreEqual(0, SeederBodyUtilities.GetExternalBodypartsWithoutLimbCoverage(context, body).Count,
            "Once the horn is attached to the head limb, the audit should pass.");
    }

    [TestMethod]
    public void GetExternalBodypartsWithoutLimbCoverage_MissingWingMembership_ReturnsUncoveredWing()
    {
        using FuturemudDatabaseContext context = BuildContext();
        BodyProto body = new()
        {
            Id = 2,
            Name = "Eastern Dragon",
            ConsiderString = "an eastern dragon",
            WielderDescriptionSingle = "mouth",
            WielderDescriptionPlural = "mouths",
            LegDescriptionSingular = "leg",
            LegDescriptionPlural = "legs"
        };
        BodypartProto wingBase = new()
        {
            Id = 20,
            Body = body,
            BodyId = body.Id,
            Name = "rwingbase",
            Description = "right wing base",
            BodypartType = 0,
            IsOrgan = 0
        };
        BodypartProto wing = new()
        {
            Id = 21,
            Body = body,
            BodyId = body.Id,
            Name = "rwing",
            Description = "right wing",
            BodypartType = 0,
            IsOrgan = 0
        };
        Limb wingLimb = new()
        {
            Id = 200,
            Name = "Right Wing",
            RootBody = body,
            RootBodyId = body.Id,
            RootBodypart = wingBase,
            RootBodypartId = wingBase.Id
        };

        context.BodyProtos.Add(body);
        context.BodypartProtos.AddRange(wingBase, wing);
        context.Limbs.Add(wingLimb);
        context.LimbsBodypartProto.Add(new LimbBodypartProto
        {
            Limb = wingLimb,
            LimbId = wingLimb.Id,
            BodypartProto = wingBase,
            BodypartProtoId = wingBase.Id
        });
        context.SaveChanges();

        IReadOnlyList<BodypartProto> uncoveredParts = SeederBodyUtilities.GetExternalBodypartsWithoutLimbCoverage(context, body);
        CollectionAssert.AreEqual(new[] { "rwing" }, uncoveredParts.Select(x => x.Name).ToArray(),
            "Unattached wing bodyparts should be flagged by the seeder audit.");

        context.LimbsBodypartProto.Add(new LimbBodypartProto
        {
            Limb = wingLimb,
            LimbId = wingLimb.Id,
            BodypartProto = wing,
            BodypartProtoId = wing.Id
        });
        context.SaveChanges();

        Assert.AreEqual(0, SeederBodyUtilities.GetExternalBodypartsWithoutLimbCoverage(context, body).Count,
            "Once the wing is attached to the wing limb, the audit should pass.");
    }

    [TestMethod]
    public void RemoveBodyparts_ClonedHindlegSubtree_RemovesDescendantsFromClone()
    {
        using FuturemudDatabaseContext context = BuildContext();
        BodyProto source = new()
        {
            Id = 3,
            Name = "Quadruped",
            ConsiderString = "a quadruped",
            WielderDescriptionSingle = "mouth",
            WielderDescriptionPlural = "mouths",
            LegDescriptionSingular = "leg",
            LegDescriptionPlural = "legs"
        };
        BodyProto target = new()
        {
            Id = 4,
            Name = "Hippocamp",
            ConsiderString = "a hippocamp",
            WielderDescriptionSingle = "mouth",
            WielderDescriptionPlural = "mouths",
            LegDescriptionSingular = "leg",
            LegDescriptionPlural = "legs"
        };
        BodypartProto back = new()
        {
            Id = 30,
            Body = source,
            BodyId = source.Id,
            Name = "lback",
            Description = "lower back",
            BodypartType = 0,
            IsOrgan = 0,
            DisplayOrder = 1
        };
        BodypartProto upperHindleg = new()
        {
            Id = 31,
            Body = source,
            BodyId = source.Id,
            Name = "ruhindleg",
            Description = "right upper hindleg",
            BodypartType = 0,
            IsOrgan = 0,
            DisplayOrder = 2
        };
        BodypartProto knee = new()
        {
            Id = 32,
            Body = source,
            BodyId = source.Id,
            Name = "rrknee",
            Description = "right rear knee",
            BodypartType = 0,
            IsOrgan = 0,
            DisplayOrder = 3
        };
        BodypartProto lowerHindleg = new()
        {
            Id = 33,
            Body = source,
            BodyId = source.Id,
            Name = "rlhindleg",
            Description = "right lower hindleg",
            BodypartType = 0,
            IsOrgan = 0,
            DisplayOrder = 4
        };
        BodypartProto hock = new()
        {
            Id = 34,
            Body = source,
            BodyId = source.Id,
            Name = "rrhock",
            Description = "right rear hock",
            BodypartType = 0,
            IsOrgan = 0,
            DisplayOrder = 5
        };

        context.BodyProtos.AddRange(source, target);
        context.BodypartProtos.AddRange(back, upperHindleg, knee, lowerHindleg, hock);
        context.SaveChanges();

        context.BodypartProtoBodypartProtoUpstream.AddRange(
            new BodypartProtoBodypartProtoUpstream
            {
                ChildNavigation = upperHindleg,
                ParentNavigation = back
            },
            new BodypartProtoBodypartProtoUpstream
            {
                ChildNavigation = knee,
                ParentNavigation = upperHindleg
            },
            new BodypartProtoBodypartProtoUpstream
            {
                ChildNavigation = lowerHindleg,
                ParentNavigation = knee
            },
            new BodypartProtoBodypartProtoUpstream
            {
                ChildNavigation = hock,
                ParentNavigation = lowerHindleg
            });

        Limb torsoLimb = new()
        {
            Id = 300,
            Name = "Torso",
            RootBody = source,
            RootBodyId = source.Id,
            RootBodypart = back,
            RootBodypartId = back.Id
        };
        Limb hindlegLimb = new()
        {
            Id = 301,
            Name = "Right Hindleg",
            RootBody = source,
            RootBodyId = source.Id,
            RootBodypart = upperHindleg,
            RootBodypartId = upperHindleg.Id
        };

        context.Limbs.AddRange(torsoLimb, hindlegLimb);
        context.LimbsBodypartProto.AddRange(
            new LimbBodypartProto
            {
                Limb = torsoLimb,
                LimbId = torsoLimb.Id,
                BodypartProto = back,
                BodypartProtoId = back.Id
            },
            new LimbBodypartProto
            {
                Limb = hindlegLimb,
                LimbId = hindlegLimb.Id,
                BodypartProto = upperHindleg,
                BodypartProtoId = upperHindleg.Id
            },
            new LimbBodypartProto
            {
                Limb = hindlegLimb,
                LimbId = hindlegLimb.Id,
                BodypartProto = knee,
                BodypartProtoId = knee.Id
            },
            new LimbBodypartProto
            {
                Limb = hindlegLimb,
                LimbId = hindlegLimb.Id,
                BodypartProto = lowerHindleg,
                BodypartProtoId = lowerHindleg.Id
            },
            new LimbBodypartProto
            {
                Limb = hindlegLimb,
                LimbId = hindlegLimb.Id,
                BodypartProto = hock,
                BodypartProtoId = hock.Id
            });
        context.SaveChanges();

        SeederBodyUtilities.CloneBodyDefinition(context, source, target);
        SeederBodyUtilities.RemoveBodyparts(context, target, ["ruhindleg"]);

        CollectionAssert.AreEqual(
            new[] { "lback" },
            context.BodypartProtos
                .Where(x => x.BodyId == target.Id)
                .OrderBy(x => x.DisplayOrder ?? 0)
                .ThenBy(x => x.Id)
                .Select(x => x.Name)
                .ToArray(),
            "Removing a cloned hindleg root should also remove its cloned knee, lower hindleg, and hock descendants.");
        Assert.AreEqual(0, SeederBodyUtilities.GetExternalBodypartsWithoutLimbCoverage(context, target).Count,
            "Removing a cloned subtree should not leave uncovered descendant bodyparts behind.");
    }

    [TestMethod]
    public void TemplatesForTesting_KeyRaces_UseExpectedBodyAssignments()
    {
        Assert.AreEqual("Toed Quadruped", MythicalAnimalSeeder.TemplatesForTesting["Dragon"].BodyKey,
            "Dragons should reuse the toed quadruped base body.");
        Assert.AreEqual("Eastern Dragon", MythicalAnimalSeeder.TemplatesForTesting["Eastern Dragon"].BodyKey,
            "Eastern dragons should use their dedicated wingless dragon body.");
        Assert.AreEqual("Griffin", MythicalAnimalSeeder.TemplatesForTesting["Griffin"].BodyKey,
            "Griffins should use their dedicated hybrid body.");
        Assert.AreEqual("Mermaid", MythicalAnimalSeeder.TemplatesForTesting["Mermaid"].BodyKey,
            "Merfolk should use the humanoid-piscine hybrid body.");
        Assert.AreEqual("Winged Humanoid", MythicalAnimalSeeder.TemplatesForTesting["Owlkin"].BodyKey,
            "Owlkin should use the shared winged humanoid body.");
        Assert.AreEqual("Centaur", MythicalAnimalSeeder.TemplatesForTesting["Centaur"].BodyKey,
            "Centaurs should use the dedicated centaur hybrid body.");
        Assert.AreEqual("Organic Humanoid", MythicalAnimalSeeder.TemplatesForTesting["Myconid"].BodyKey,
            "Myconids should share the stock humanoid body for equipment and surgery compatibility.");
        Assert.AreEqual("Ungulate", MythicalAnimalSeeder.TemplatesForTesting["Pegacorn"].BodyKey,
            "Pegacorns should reuse the ungulate body that already supports horns and wings.");
        Assert.AreEqual("Toed Quadruped", MythicalAnimalSeeder.TemplatesForTesting["Warg"].BodyKey,
            "Wargs should reuse the stock canid-compatible quadruped body.");
        Assert.AreEqual("Toed Quadruped", MythicalAnimalSeeder.TemplatesForTesting["Dire-Wolf"].BodyKey,
            "Dire-wolves should reuse the stock canid-compatible quadruped body.");
        Assert.AreEqual("Toed Quadruped", MythicalAnimalSeeder.TemplatesForTesting["Dire-Bear"].BodyKey,
            "Dire-bears should reuse the stock bear-compatible quadruped body.");
        Assert.AreEqual("Beetle", MythicalAnimalSeeder.TemplatesForTesting["Giant Beetle"].BodyKey,
            "Giant beetles should use the dedicated beetle body.");
        Assert.AreEqual("Insectoid", MythicalAnimalSeeder.TemplatesForTesting["Giant Ant"].BodyKey,
            "Giant ants should reuse the shared insectoid body.");
        Assert.AreEqual("Insectoid", MythicalAnimalSeeder.TemplatesForTesting["Giant Mantis"].BodyKey,
            "Giant mantises should continue to use the shared insectoid body.");
        Assert.AreEqual("Arachnid", MythicalAnimalSeeder.TemplatesForTesting["Giant Spider"].BodyKey,
            "Giant spiders should use the shared arachnid body.");
        Assert.AreEqual("Scorpion", MythicalAnimalSeeder.TemplatesForTesting["Giant Scorpion"].BodyKey,
            "Giant scorpions should use the dedicated scorpion body.");
        Assert.AreEqual("Centipede", MythicalAnimalSeeder.TemplatesForTesting["Giant Centipede"].BodyKey,
            "Giant centipedes should use the dedicated centipede body.");
        Assert.AreEqual("Centipede", MythicalAnimalSeeder.TemplatesForTesting["Ankheg"].BodyKey,
            "Ankhegs should use the dedicated centipede body.");
        Assert.AreEqual("Vermiform", MythicalAnimalSeeder.TemplatesForTesting["Giant Worm"].BodyKey,
            "Giant worms should reuse the stock vermiform body.");
        Assert.AreEqual("Vermiform", MythicalAnimalSeeder.TemplatesForTesting["Colossal Worm"].BodyKey,
            "Colossal worms should reuse the stock vermiform body.");
        Assert.AreEqual("Organic Humanoid", MythicalAnimalSeeder.TemplatesForTesting["Ent"].BodyKey,
            "Ents should reuse the stock organic humanoid body.");
        Assert.AreEqual("Organic Humanoid", MythicalAnimalSeeder.TemplatesForTesting["Dryad"].BodyKey,
            "Dryads should reuse the stock organic humanoid body.");
        Assert.AreEqual("Ungulate", MythicalAnimalSeeder.TemplatesForTesting["Qilin"].BodyKey,
            "Qilin should reuse the horn-capable ungulate body.");
        Assert.AreEqual("Avian", MythicalAnimalSeeder.TemplatesForTesting["Garuda"].BodyKey,
            "Garuda should reuse the avian body.");
        Assert.AreEqual("Toed Quadruped", MythicalAnimalSeeder.TemplatesForTesting["Bunyip"].BodyKey,
            "Bunyips should reuse the stock quadruped predator body.");
        Assert.AreEqual("Serpentine", MythicalAnimalSeeder.TemplatesForTesting["Yacumama"].BodyKey,
            "Yacumama should reuse the serpentine body.");
    }

    [TestMethod]
    public void TemplatesForTesting_CombatStrategyKeys_MapRepresentativeMythicRacesToExpectedStyles()
    {
        Assert.AreEqual("Beast Artillery", MythicalAnimalSeeder.TemplatesForTesting["Dragon"].CombatStrategyKey);
        Assert.AreEqual("Beast Dropper", MythicalAnimalSeeder.TemplatesForTesting["Griffin"].CombatStrategyKey);
        Assert.AreEqual("Beast Skirmisher", MythicalAnimalSeeder.TemplatesForTesting["Unicorn"].CombatStrategyKey);
        Assert.AreEqual("Beast Clincher", MythicalAnimalSeeder.TemplatesForTesting["Basilisk"].CombatStrategyKey);
        Assert.AreEqual("Beast Dropper", MythicalAnimalSeeder.TemplatesForTesting["Wyvern"].CombatStrategyKey);
        Assert.AreEqual("Beast Skirmisher", MythicalAnimalSeeder.TemplatesForTesting["Warg"].CombatStrategyKey);
        Assert.AreEqual("Beast Brawler", MythicalAnimalSeeder.TemplatesForTesting["Dire-Wolf"].CombatStrategyKey);
        Assert.AreEqual("Beast Behemoth", MythicalAnimalSeeder.TemplatesForTesting["Dire-Bear"].CombatStrategyKey);
        Assert.AreEqual("Beast Behemoth", MythicalAnimalSeeder.TemplatesForTesting["Giant Beetle"].CombatStrategyKey);
        Assert.AreEqual("Beast Clincher", MythicalAnimalSeeder.TemplatesForTesting["Giant Ant"].CombatStrategyKey);
        Assert.AreEqual("Beast Skirmisher", MythicalAnimalSeeder.TemplatesForTesting["Giant Mantis"].CombatStrategyKey);
        Assert.AreEqual("Beast Skirmisher", MythicalAnimalSeeder.TemplatesForTesting["Giant Spider"].CombatStrategyKey);
        Assert.AreEqual("Beast Brawler", MythicalAnimalSeeder.TemplatesForTesting["Giant Scorpion"].CombatStrategyKey);
        Assert.AreEqual("Beast Clincher", MythicalAnimalSeeder.TemplatesForTesting["Giant Centipede"].CombatStrategyKey);
        Assert.AreEqual("Beast Artillery", MythicalAnimalSeeder.TemplatesForTesting["Ankheg"].CombatStrategyKey);
        Assert.AreEqual("Melee (Auto)", MythicalAnimalSeeder.TemplatesForTesting["Centaur"].CombatStrategyKey);
        Assert.AreEqual("Beast Skirmisher", MythicalAnimalSeeder.TemplatesForTesting["Qilin"].CombatStrategyKey);
        Assert.AreEqual("Beast Dropper", MythicalAnimalSeeder.TemplatesForTesting["Garuda"].CombatStrategyKey);
        Assert.AreEqual("Beast Drowner", MythicalAnimalSeeder.TemplatesForTesting["Bunyip"].CombatStrategyKey);
        Assert.AreEqual("Beast Clincher", MythicalAnimalSeeder.TemplatesForTesting["Yacumama"].CombatStrategyKey);
    }

    [TestMethod]
    public void TemplatesForTesting_DropperMythicPredators_HaveTalonCarryAttack()
    {
        Assert.IsTrue(MythicalAnimalSeeder.TemplatesForTesting["Griffin"].Attacks.Any(x => x.AttackName == "Talon Carry"));
        Assert.IsTrue(MythicalAnimalSeeder.TemplatesForTesting["Wyvern"].Attacks.Any(x => x.AttackName == "Talon Carry"));
    }

    [TestMethod]
    public void TemplatesForTesting_SecondPassAttributeProfiles_ReflectMythicBodyPlans()
    {
        static void AssertProfile(string raceName, int strength, int constitution, int agility, int dexterity,
            int willpower, int perception, int aura, string intelligenceDice, string auraDice, string message)
        {
            NonHumanAttributeProfile profile = MythicalAnimalSeeder.TemplatesForTesting[raceName].AttributeProfile;
            Assert.AreEqual(strength, profile.StrengthBonus, $"{raceName} strength bonus");
            Assert.AreEqual(constitution, profile.ConstitutionBonus, $"{raceName} constitution bonus");
            Assert.AreEqual(agility, profile.AgilityBonus, $"{raceName} agility bonus");
            Assert.AreEqual(dexterity, profile.DexterityBonus, $"{raceName} dexterity bonus");
            Assert.AreEqual(willpower, profile.WillpowerBonus, $"{raceName} willpower bonus");
            Assert.AreEqual(perception, profile.PerceptionBonus, $"{raceName} perception bonus");
            Assert.AreEqual(aura, profile.AuraBonus, $"{raceName} aura bonus");
            Assert.AreEqual(intelligenceDice, profile.IntelligenceDiceExpression, $"{raceName} intelligence dice");
            Assert.AreEqual(auraDice, profile.AuraDiceExpression, message);
        }

        AssertProfile("Dragon", 12, 11, 0, -2, 6, 3, 5, null, null,
            "True dragons should remain the top brute-force mythic baseline.");
        AssertProfile("Eastern Dragon", 10, 9, 2, 0, 6, 3, 5, null, null,
            "Eastern dragons should be less blocky and more sinuous than western dragons.");
        AssertProfile("Unicorn", 6, 5, 4, 1, 4, 3, 5, null, null,
            "Unicorns should read as powerful but unusually graceful equines.");
        AssertProfile("Pegasus", 5, 4, 5, 1, 2, 3, 3, "2d3", null,
            "Pegasi should be driven more by flight athletics than raw mass.");
        AssertProfile("Phoenix", 2, 2, 5, 3, 4, 4, 6, "2d3", null,
            "Phoenixes should be high-agility aerial threats rather than heavy bruisers.");
        AssertProfile("Ent", 7, 9, -3, -3, 5, 1, 4, null, null,
            "Ents should be massively strong and durable but ponderous.");
        AssertProfile("Dryad", -1, 1, 2, 2, 2, 2, 5, null, null,
            "Dryads should favour grace and finesse over raw strength.");
        AssertProfile("Centaur", 6, 5, 2, 0, 2, 1, 0, null, null,
            "Centaurs should preserve horse-body strength while gaining open-country mobility.");
        AssertProfile("Giant Ant", 6, 6, 2, -2, 3, 1, 0, "2d3", "1d2",
            "Giant insects should remain animal-minded and spiritually minimal.");
        AssertProfile("Giant Spider", 6, 5, 5, 1, 2, 2, 0, "2d3", "1d2",
            "Giant spiders should read as fast ambush arthropods rather than generic heavy monsters.");
        AssertProfile("Basilisk", 5, 6, 2, 0, 4, 2, 2, "2d3", null,
            "Magical animal-minded monsters should keep low intelligence without suppressing supernatural aura.");
    }

    [TestMethod]
    public void TemplatesForTesting_HumanoidVarietyRaces_MatchExpectedCatalogue()
    {
        string[] humanoidVarietyRaces = MythicalAnimalSeeder.TemplatesForTesting
            .Where(x => x.Value.HumanoidVariety)
            .Select(x => x.Key)
            .ToArray();

        CollectionAssert.AreEquivalent(
            new[]
            {
                "Minotaur",
                "Naga",
                "Mermaid",
                "Selkie",
                "Dryad",
                "Owlkin",
                "Avian Person",
                "Centaur"
            },
            humanoidVarietyRaces,
            "The humanoid-form catalogue should cover the races expected to reuse human-style variation.");
        Assert.IsTrue(
            MythicalAnimalSeeder.TemplatesForTesting
                .Where(x => x.Value.HumanoidVariety)
                .All(x => x.Value.CanUseWeapons),
            "Humanoid variety races should continue to support weapon use.");
    }

    [TestMethod]
    public void BreathingProfileNameForTesting_KeyMythicFamilies_UseExpectedModels()
    {
        Assert.AreEqual("simple-air", MythicalAnimalSeeder.GetBreathingProfileNameForTesting("Dragon"));
        Assert.AreEqual("marine-amphibious", MythicalAnimalSeeder.GetBreathingProfileNameForTesting("Mermaid"));
        Assert.AreEqual("marine-amphibious", MythicalAnimalSeeder.GetBreathingProfileNameForTesting("Hippocamp"));
        Assert.AreEqual("partless-air", MythicalAnimalSeeder.GetBreathingProfileNameForTesting("Giant Spider"));
        Assert.AreEqual("partless-air", MythicalAnimalSeeder.GetBreathingProfileNameForTesting("Ent"));
        Assert.AreEqual("partless-air", MythicalAnimalSeeder.GetBreathingProfileNameForTesting("Dryad"));
    }

    [TestMethod]
    public void TemplatesForTesting_RepresentativeMythicalRaces_UseExpectedSatiationCadences()
    {
        AssertSatiationCadence(
            MythicalAnimalSeeder.GetMythicalSatiationLimitsForTesting(MythicalAnimalSeeder.TemplatesForTesting["Dragon"]),
            720.0,
            168.0);
        AssertSatiationCadence(
            MythicalAnimalSeeder.GetMythicalSatiationLimitsForTesting(MythicalAnimalSeeder.TemplatesForTesting["Warg"]),
            12.0,
            8.0);
        AssertSatiationCadence(
            MythicalAnimalSeeder.GetMythicalSatiationLimitsForTesting(MythicalAnimalSeeder.TemplatesForTesting["Basilisk"]),
            720.0,
            168.0);
        AssertSatiationCadence(
            MythicalAnimalSeeder.GetMythicalSatiationLimitsForTesting(MythicalAnimalSeeder.TemplatesForTesting["Mermaid"]),
            24.0,
            48.0);
        AssertSatiationCadence(
            MythicalAnimalSeeder.GetMythicalSatiationLimitsForTesting(MythicalAnimalSeeder.TemplatesForTesting["Giant Spider"]),
            336.0,
            168.0);
        AssertSatiationCadence(
            MythicalAnimalSeeder.GetMythicalSatiationLimitsForTesting(MythicalAnimalSeeder.TemplatesForTesting["Colossal Worm"]),
            720.0,
            336.0);
        AssertSatiationCadence(
            MythicalAnimalSeeder.GetMythicalSatiationLimitsForTesting(MythicalAnimalSeeder.TemplatesForTesting["Ent"]),
            720.0,
            168.0);
        AssertSatiationCadence(
            MythicalAnimalSeeder.GetMythicalSatiationLimitsForTesting(MythicalAnimalSeeder.TemplatesForTesting["Dryad"]),
            72.0,
            48.0);
        AssertSatiationCadence(
            MythicalAnimalSeeder.GetMythicalSatiationLimitsForTesting(MythicalAnimalSeeder.TemplatesForTesting["Centaur"]),
            12.0,
            8.0);
        AssertSatiationCadence(
            MythicalAnimalSeeder.GetMythicalSatiationLimitsForTesting(MythicalAnimalSeeder.TemplatesForTesting["Qilin"]),
            48.0,
            24.0);
        AssertSatiationCadence(
            MythicalAnimalSeeder.GetMythicalSatiationLimitsForTesting(MythicalAnimalSeeder.TemplatesForTesting["Yacumama"]),
            720.0,
            168.0);
    }

    [TestMethod]
    public void HybridMovementAliasesForTesting_KeyBodies_ExposeExpectedTraversalModes()
    {
        CollectionAssert.AreEquivalent(
            new[] { "slither", "slowslither", "quickslither" },
            MythicalAnimalSeeder.GetHybridMovementAliasesForTesting("Naga").ToArray(),
            "Naga should expose serpentine travel speeds.");
        CollectionAssert.AreEquivalent(
            new[] { "flop" },
            MythicalAnimalSeeder.GetHybridMovementAliasesForTesting("Mermaid").ToArray(),
            "Merfolk should keep a land fallback movement mode in addition to swimming.");
        CollectionAssert.AreEquivalent(
            new[] { "stalk", "amble", "pace", "trot", "gallop" },
            MythicalAnimalSeeder.GetHybridMovementAliasesForTesting("Centaur").ToArray(),
            "Centaurs should inherit quadruped gait options rather than only humanoid walking verbs.");
        CollectionAssert.AreEquivalent(
            new[] { "slowfly", "fly", "franticfly" },
            MythicalAnimalSeeder.GetHybridMovementAliasesForTesting("Eastern Dragon").ToArray(),
            "Eastern dragons should retain a dedicated flight profile even without physical wings.");
    }

    [TestMethod]
    public void TemplatesForTesting_LegacyFantasyRaces_KeepExpectedSapienceAndCharacteristics()
    {
        MythicalAnimalSeeder.MythicalRaceTemplate myconid = MythicalAnimalSeeder.TemplatesForTesting["Myconid"];
        Assert.IsFalse(myconid.HumanoidVariety,
            "Myconids should not inherit the full human characteristic matrix.");
        Assert.IsTrue(myconid.CanUseWeapons,
            "Myconids should remain tool-using humanoid-bodied races.");
        Assert.IsTrue(
            myconid.AdditionalCharacteristics?.Any(x => x.DefinitionName == "Fungus Colour") == true,
            "Myconids should keep their legacy fungus colour characteristic.");

        MythicalAnimalSeeder.MythicalRaceTemplate ent = MythicalAnimalSeeder.TemplatesForTesting["Ent"];
        Assert.IsTrue(
            ent.AdditionalCharacteristics?.Any(x => x.DefinitionName == "Bark Tone") == true,
            "Ents should expose their bark-tone characteristic.");

        MythicalAnimalSeeder.MythicalRaceTemplate dragon = MythicalAnimalSeeder.TemplatesForTesting["Dragon"];
        Assert.IsTrue(
            dragon.AdditionalCharacteristics?.Any(x => x.DefinitionName == "Scale Colour") == true,
            "Dragons should retain their scale colour characteristic.");
    }

    [TestMethod]
    public void TemplatesForTesting_DefaultDisfigurementHooks_AreEmptyByDefault()
    {
        Assert.IsTrue(
            MythicalAnimalSeeder.TemplatesForTesting.Values.All(x => x.TattooTemplates is null || x.TattooTemplates.Count == 0),
            "Mythical stock templates should default to no tattoo templates until a later content pass fills them in.");
    }

    [TestMethod]
    public void TemplatesForTesting_SignatureUsagesAndAttacks_ArePresentForMythicSpecialCases()
    {
        MythicalAnimalSeeder.MythicalRaceTemplate unicorn = MythicalAnimalSeeder.TemplatesForTesting["Unicorn"];
        Assert.IsTrue(
            unicorn.BodypartUsages?.Any(x => x.BodypartAlias == "horn" && x.Usage == "general") == true,
            "Unicorns should expose their horn as a general-purpose additional bodypart.");
        CollectionAssert.Contains(unicorn.Attacks.Select(x => x.AttackName).ToList(), "Bite");
        CollectionAssert.Contains(unicorn.Attacks.Select(x => x.AttackName).ToList(), "Horn Gore");

        MythicalAnimalSeeder.MythicalRaceTemplate pegasus = MythicalAnimalSeeder.TemplatesForTesting["Pegasus"];
        CollectionAssert.AreEquivalent(
            new[] { "rwingbase", "lwingbase", "rwing", "lwing" },
            pegasus.BodypartUsages!.Select(x => x.BodypartAlias).ToArray(),
            "Pegasi should expose both wing roots and both wings.");
        CollectionAssert.Contains(pegasus.Attacks.Select(x => x.AttackName).ToList(), "Bite");

        MythicalAnimalSeeder.MythicalRaceTemplate pegacorn = MythicalAnimalSeeder.TemplatesForTesting["Pegacorn"];
        CollectionAssert.Contains(pegacorn.Attacks.Select(x => x.AttackName).ToList(), "Bite");
        CollectionAssert.Contains(pegacorn.Attacks.Select(x => x.AttackName).ToList(), "Horn Gore");

        MythicalAnimalSeeder.MythicalRaceTemplate phoenix = MythicalAnimalSeeder.TemplatesForTesting["Phoenix"];
        CollectionAssert.AreEquivalent(
            new[] { "Beak Peck", "Beak Bite", "Talon Strike" },
            phoenix.Attacks.Select(x => x.AttackName).ToArray(),
            "Phoenixes should keep the avian peck, clinch beak strike, and talon loadout.");

        MythicalAnimalSeeder.MythicalRaceTemplate ankheg = MythicalAnimalSeeder.TemplatesForTesting["Ankheg"];
        CollectionAssert.Contains(ankheg.Attacks.Select(x => x.AttackName).ToList(), "Acid Spit");
        CollectionAssert.AreEquivalent(
            new[] { "mandibles" },
            ankheg.Attacks
                .Where(x => x.AttackName == "Acid Spit")
                .SelectMany(x => x.BodypartAliases)
                .Distinct()
                .ToArray(),
            "Ankhegs should deliver acid spit through their mandibles.");

        MythicalAnimalSeeder.MythicalRaceTemplate giantScorpion = MythicalAnimalSeeder.TemplatesForTesting["Giant Scorpion"];
        CollectionAssert.Contains(giantScorpion.Attacks.Select(x => x.AttackName).ToList(), "Tail Spike");
        CollectionAssert.AreEquivalent(
            new[] { "stinger" },
            giantScorpion.Attacks
                .Where(x => x.AttackName == "Tail Spike")
                .SelectMany(x => x.BodypartAliases)
                .Distinct()
                .ToArray(),
            "Giant scorpions should deliver their tail spike through the stinger bodypart.");

        MythicalAnimalSeeder.MythicalRaceTemplate giantSpider = MythicalAnimalSeeder.TemplatesForTesting["Giant Spider"];
        Assert.IsTrue(giantSpider.CanClimb, "Giant spiders should keep their climbing capability.");
        CollectionAssert.AreEquivalent(
            new[] { "rfang", "lfang" },
            giantSpider.Attacks
                .Where(x => x.AttackName == "Carnivore Bite")
                .SelectMany(x => x.BodypartAliases)
                .Distinct()
                .ToArray(),
            "Giant spiders should deliver their heavy bite through paired fangs.");
    }

    [TestMethod]
    public void TemplatesForTesting_Warg_RemainsBestialAndNonPlayable()
    {
        MythicalAnimalSeeder.MythicalRaceTemplate warg = MythicalAnimalSeeder.TemplatesForTesting["Warg"];

        Assert.IsFalse(warg.HumanoidVariety,
            "Wargs should remain bestial rather than reusing humanoid variety handling.");
        Assert.IsFalse(warg.Playable,
            "Wargs should remain non-playable stock mythic beasts.");
        Assert.IsFalse(warg.CanUseWeapons,
            "Wargs should not be able to use weapons.");
    }

    [TestMethod]
    public void TemplatesForTesting_AllMythicRaces_HaveClinchAndNonClinchAttacks()
    {
        foreach ((string name, MythicalAnimalSeeder.MythicalRaceTemplate template) in MythicalAnimalSeeder.TemplatesForTesting)
        {
            Assert.IsTrue(
                template.Attacks.Any(x => x.AttackName is "Bite" or "Beak Bite" or "Herbivore Bite" or "Elbow"),
                $"Mythical race {name} should expose at least one clinch-capable natural attack.");
            Assert.IsTrue(
                template.Attacks.Any(x => x.AttackName is "Carnivore Bite" or "Claw Swipe" or "Horn Gore" or "Tail Slap" or
                    "Beak Peck" or "Hoof Stomp" or "Head Ram" or "Talon Strike" or "Jab"),
                $"Mythical race {name} should expose at least one non-clinch natural attack.");
        }
    }

    [TestMethod]
    public void TemplatesForTesting_Descriptions_UseThreeParagraphProseAndHumanoidOverlayVariants()
    {
        MythicalAnimalSeeder.MythicalRaceTemplate dragon = MythicalAnimalSeeder.TemplatesForTesting["Dragon"];
        Assert.IsNotNull(dragon.DescriptionVariants);
        Assert.AreEqual(3, ParagraphCount(MythicalAnimalSeeder.BuildRaceDescriptionForTesting(dragon)));
        Assert.AreEqual(3, ParagraphCount(MythicalAnimalSeeder.BuildEthnicityDescriptionForTesting(dragon)));
        StringAssert.Contains(MythicalAnimalSeeder.BuildRaceDescriptionForTesting(dragon), "broad wings, curving horns");
        StringAssert.Contains(MythicalAnimalSeeder.BuildEthnicityDescriptionForTesting(dragon), "overlapping scales");

        MythicalAnimalSeeder.MythicalRaceTemplate centaur = MythicalAnimalSeeder.TemplatesForTesting["Centaur"];
        Assert.IsNotNull(centaur.OverlayDescriptionVariants);
        Assert.AreEqual(3, ParagraphCount(MythicalAnimalSeeder.BuildRaceDescriptionForTesting(centaur)));
        Assert.AreEqual(3, ParagraphCount(MythicalAnimalSeeder.BuildEthnicityDescriptionForTesting(centaur)));
        StringAssert.Contains(MythicalAnimalSeeder.BuildRaceDescriptionForTesting(centaur), "powerful equine lower body");
        StringAssert.Contains(MythicalAnimalSeeder.BuildEthnicityDescriptionForTesting(centaur), "broad horse-frame");
    }
}
