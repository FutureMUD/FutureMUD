using System;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MythicalAnimalSeederTemplateTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void ValidateTemplateCatalogForTesting_CurrentCatalog_HasNoIssues()
	{
		var issues = MythicalAnimalSeeder.ValidateTemplateCatalogForTesting();
		Assert.AreEqual(0, issues.Count, string.Join("\n", issues));
	}

	[TestMethod]
	public void BuildBodypartAliasLookup_DuplicateAliases_GroupsAndOrdersDeterministically()
	{
		var parts = new[]
		{
			new BodypartProto { Id = 3, Name = "tail", DisplayOrder = 20 },
			new BodypartProto { Id = 2, Name = "tail", DisplayOrder = 10 },
			new BodypartProto { Id = 1, Name = "tail", DisplayOrder = 10 },
			new BodypartProto { Id = 4, Name = "head", DisplayOrder = 5 }
		};

		var groupedLookup = SeederBodyUtilities.BuildBodypartAliasLookup(parts);
		CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, groupedLookup["tail"].Select(x => x.Id).ToArray(),
			"Duplicate aliases should be retained in stable display-order then id order.");

		var lookup = SeederBodyUtilities.BuildBodypartLookup(parts);
		Assert.AreEqual(1L, lookup["tail"].Id,
			"Single-part lookups should resolve duplicate aliases to the earliest stable entry.");
	}

	[TestMethod]
	public void GetExternalBodypartsWithoutLimbCoverage_MissingHornMembership_ReturnsUncoveredHorn()
	{
		using var context = BuildContext();
		var body = new BodyProto
		{
			Id = 1,
			Name = "Horned Humanoid",
			ConsiderString = "a horned humanoid",
			WielderDescriptionSingle = "hand",
			WielderDescriptionPlural = "hands",
			LegDescriptionSingular = "leg",
			LegDescriptionPlural = "legs"
		};
		var neck = new BodypartProto
		{
			Id = 10,
			Body = body,
			BodyId = body.Id,
			Name = "neck",
			Description = "neck",
			BodypartType = 0,
			IsOrgan = 0
		};
		var horn = new BodypartProto
		{
			Id = 11,
			Body = body,
			BodyId = body.Id,
			Name = "rhorn",
			Description = "right horn",
			BodypartType = 0,
			IsOrgan = 0
		};
		var headLimb = new Limb
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

		var uncoveredParts = SeederBodyUtilities.GetExternalBodypartsWithoutLimbCoverage(context, body);
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
		using var context = BuildContext();
		var body = new BodyProto
		{
			Id = 2,
			Name = "Eastern Dragon",
			ConsiderString = "an eastern dragon",
			WielderDescriptionSingle = "mouth",
			WielderDescriptionPlural = "mouths",
			LegDescriptionSingular = "leg",
			LegDescriptionPlural = "legs"
		};
		var wingBase = new BodypartProto
		{
			Id = 20,
			Body = body,
			BodyId = body.Id,
			Name = "rwingbase",
			Description = "right wing base",
			BodypartType = 0,
			IsOrgan = 0
		};
		var wing = new BodypartProto
		{
			Id = 21,
			Body = body,
			BodyId = body.Id,
			Name = "rwing",
			Description = "right wing",
			BodypartType = 0,
			IsOrgan = 0
		};
		var wingLimb = new Limb
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

		var uncoveredParts = SeederBodyUtilities.GetExternalBodypartsWithoutLimbCoverage(context, body);
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
		using var context = BuildContext();
		var source = new BodyProto
		{
			Id = 3,
			Name = "Quadruped",
			ConsiderString = "a quadruped",
			WielderDescriptionSingle = "mouth",
			WielderDescriptionPlural = "mouths",
			LegDescriptionSingular = "leg",
			LegDescriptionPlural = "legs"
		};
		var target = new BodyProto
		{
			Id = 4,
			Name = "Hippocamp",
			ConsiderString = "a hippocamp",
			WielderDescriptionSingle = "mouth",
			WielderDescriptionPlural = "mouths",
			LegDescriptionSingular = "leg",
			LegDescriptionPlural = "legs"
		};
		var back = new BodypartProto
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
		var upperHindleg = new BodypartProto
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
		var knee = new BodypartProto
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
		var lowerHindleg = new BodypartProto
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
		var hock = new BodypartProto
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

		var torsoLimb = new Limb
		{
			Id = 300,
			Name = "Torso",
			RootBody = source,
			RootBodyId = source.Id,
			RootBodypart = back,
			RootBodypartId = back.Id
		};
		var hindlegLimb = new Limb
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
	public void TemplatesForTesting_HumanoidVarietyRaces_MatchExpectedCatalogue()
	{
		var humanoidVarietyRaces = MythicalAnimalSeeder.TemplatesForTesting
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
		var myconid = MythicalAnimalSeeder.TemplatesForTesting["Myconid"];
		Assert.IsFalse(myconid.HumanoidVariety,
			"Myconids should not inherit the full human characteristic matrix.");
		Assert.IsTrue(myconid.CanUseWeapons,
			"Myconids should remain tool-using humanoid-bodied races.");
		Assert.IsTrue(
			myconid.AdditionalCharacteristics?.Any(x => x.DefinitionName == "Fungus Colour") == true,
			"Myconids should keep their legacy fungus colour characteristic.");

		var dragon = MythicalAnimalSeeder.TemplatesForTesting["Dragon"];
		Assert.IsTrue(
			dragon.AdditionalCharacteristics?.Any(x => x.DefinitionName == "Scale Colour") == true,
			"Dragons should retain their scale colour characteristic.");
	}

	[TestMethod]
	public void TemplatesForTesting_SignatureUsagesAndAttacks_ArePresentForMythicSpecialCases()
	{
		var unicorn = MythicalAnimalSeeder.TemplatesForTesting["Unicorn"];
		Assert.IsTrue(
			unicorn.BodypartUsages?.Any(x => x.BodypartAlias == "horn" && x.Usage == "general") == true,
			"Unicorns should expose their horn as a general-purpose additional bodypart.");
		CollectionAssert.AreEquivalent(
			new[] { "Hoof Stomp", "Horn Gore" },
			unicorn.Attacks.Select(x => x.AttackName).ToArray(),
			"Unicorns should stomp and gore.");

		var pegasus = MythicalAnimalSeeder.TemplatesForTesting["Pegasus"];
		CollectionAssert.AreEquivalent(
			new[] { "rwingbase", "lwingbase", "rwing", "lwing" },
			pegasus.BodypartUsages!.Select(x => x.BodypartAlias).ToArray(),
			"Pegasi should expose both wing roots and both wings.");

		var pegacorn = MythicalAnimalSeeder.TemplatesForTesting["Pegacorn"];
		CollectionAssert.AreEquivalent(
			new[] { "Hoof Stomp", "Horn Gore" },
			pegacorn.Attacks.Select(x => x.AttackName).ToArray(),
			"Pegacorns should preserve the combined pegasus and unicorn attack profile.");

		var phoenix = MythicalAnimalSeeder.TemplatesForTesting["Phoenix"];
		CollectionAssert.AreEquivalent(
			new[] { "Beak Peck", "Talon Strike" },
			phoenix.Attacks.Select(x => x.AttackName).ToArray(),
			"Phoenixes should keep the avian peck and talon loadout.");
	}
}
