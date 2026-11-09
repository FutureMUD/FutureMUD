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
}
