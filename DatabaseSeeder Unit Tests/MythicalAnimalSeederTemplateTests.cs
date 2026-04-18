using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

    [TestMethod]
    public void ValidateTemplateCatalogForTesting_CurrentCatalog_HasNoIssues()
    {
        IReadOnlyList<string> issues = MythicalAnimalSeeder.ValidateTemplateCatalogForTesting();
        Assert.AreEqual(0, issues.Count, string.Join("\n", issues));
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
    }

    [TestMethod]
    public void TemplatesForTesting_CombatStrategyKeys_MapRepresentativeMythicRacesToExpectedStyles()
    {
        Assert.AreEqual("Beast Artillery", MythicalAnimalSeeder.TemplatesForTesting["Dragon"].CombatStrategyKey);
        Assert.AreEqual("Beast Swooper", MythicalAnimalSeeder.TemplatesForTesting["Griffin"].CombatStrategyKey);
        Assert.AreEqual("Beast Behemoth", MythicalAnimalSeeder.TemplatesForTesting["Unicorn"].CombatStrategyKey);
        Assert.AreEqual("Beast Clincher", MythicalAnimalSeeder.TemplatesForTesting["Basilisk"].CombatStrategyKey);
        Assert.AreEqual("Beast Artillery", MythicalAnimalSeeder.TemplatesForTesting["Wyvern"].CombatStrategyKey);
        Assert.AreEqual("Melee (Auto)", MythicalAnimalSeeder.TemplatesForTesting["Centaur"].CombatStrategyKey);
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
