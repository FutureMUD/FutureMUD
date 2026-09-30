using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicCastingAcquisitionTests
{
	[TestMethod]
	public void Reconcile_ReverseOrderedChain_AcquiresEntireCascadeInOneCall()
	{
		var f = new MagicCastingFixture();
		var cap = Configure(f, Chain(4));
		Seed(f, cap);
		var writes = f.Store.Writes;
		f.Service.Reconcile(f.Actor.Object);
		AssertAcquired(f, 1, 2, 3, 4);
		Assert.AreEqual(writes + 3, f.Store.Writes);
		AssertNoCasting(f);
	}

	[TestMethod]
	public void Enrol_ReverseOrderedLongChain_AcquiresEntireCascadeBeforeReturning()
	{
		var f = new MagicCastingFixture();
		var cap = Configure(f, Chain(5));
		f.Skills.Clear();
		var result = f.Service.Enrol(f.Staff.Object, f.Actor.Object, cap.Id, "cascade regression");
		Assert.IsTrue(result.Allowed, result.Message);
		AssertAcquired(f, 1, 2, 3, 4, 5);
		Assert.AreEqual(cap.CastingPolicy!.StartingGrantVersion, f.Store.Enrolment(100, cap.CastingPolicy.Identity)!.StartingVersion);
		Assert.AreEqual(10.0, f.Skills[1]);
		Assert.AreEqual(1, f.Flushes);
		var writes = f.Store.Writes;
		Assert.IsFalse(f.Service.Enrol(f.Staff.Object, f.Actor.Object, cap.Id, "repeat").Changed);
		Assert.AreEqual(writes, f.Store.Writes);
		Assert.AreEqual(1, f.Flushes);
		f.Actor.Verify(x => x.AddTrait(It.IsAny<ITraitDefinition>(), It.IsAny<double>()), Times.Once);
		AssertNoCasting(f);
	}

	[TestMethod]
	public void Reconcile_DiamondAdmissionPermutations_AcquiresJoinOnce()
	{
		var cases = 0;
		foreach (var order in Permutations(new[] { 1, 2, 3, 4 }))
		foreach (var reverseEdges in new[] { false, true })
		{
			var f = new MagicCastingFixture();
			var nodes = Diamond(reverseEdges).ToDictionary(x => (int)x.Attribute("spell")!);
			var cap = Configure(f, order.Select(x => nodes[x]));
			Seed(f, cap);
			var writes = f.Store.Writes;
			f.Service.Reconcile(f.Actor.Object);
			AssertAcquired(f, 1, 2, 3, 4);
			Assert.AreEqual(writes + 3, f.Store.Writes, $"Order {string.Join(',', order)}, reverse edges {reverseEdges}");
			cases++;
		}
		Assert.AreEqual(48, cases);
	}

	[DataTestMethod]
	[DataRow(2, 0.0)]
	[DataRow(1, 43.0)]
	public void Reconcile_DiamondUnmetBranch_LeavesJoinAbsent(int grade, double proficiency)
	{
		var f = new MagicCastingFixture();
		var cap = Configure(f, new[] { Node(4, Edge(2), Edge(3, grade, proficiency)), Node(3, Edge(1)), Node(2, Edge(1)), Node(1) });
		Seed(f, cap);
		f.Service.Reconcile(f.Actor.Object);
		AssertAcquired(f, 1, 2, 3);
		AssertNoCasting(f);
	}

	[DataTestMethod]
	[DataRow("spell", false)]
	[DataRow("trait", false)]
	[DataRow("both", false)]
	[DataRow("spell", true)]
	[DataRow("trait", true)]
	[DataRow("both", true)]
	public void NotifyProgress_SharedTraitAndSpellSeeds_CompleteAffectedCascade(string seed, bool reverse)
	{
		var f = new MagicCastingFixture();
		var nodes = new[] { Node(1), Node(2, Edge(1, 2)), Node(3, Edge(2)), Node(4, Edge(1, 2), Edge(2), Edge(3)) };
		var cap = Configure(f, reverse ? nodes.Reverse() : nodes);
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, cap.Id, "root below threshold").Allowed);
		AssertAcquired(f, 1);
		f.Acquire(2);
		var writes = f.Store.Writes;
		f.Service.NotifyProgress(f.Actor.Object, traitId: seed == "spell" ? null : 1, spellId: seed == "trait" ? null : 1);
		AssertAcquired(f, 1, 2, 3, 4);
		Assert.AreEqual(writes + 3, f.Store.Writes);
		AssertNoCasting(f);
	}

	[TestMethod]
	public void NotifyProgress_PrerequisiteThresholdsAndBindings_GrantOnlyWhenAllSatisfied()
	{
		var f = new MagicCastingFixture();
		var root = Node(1); root.SetAttributeValue("trait", 2);
		var branch = Node(2, Edge(1, 2, 50)); branch.SetAttributeValue("trait", 3);
		var cap = Configure(f, new[] { Node(3, Edge(2, 1, 10)), branch, root });
		// An available independent route's high trait cannot satisfy this route's binding.
		f.Capabilities.Add(f.Earth); f.ActiveCapabilities.Add(f.Earth);
		f.Skills[1] = 100; f.Skills[2] = 49; f.Skills.Remove(3);
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, cap.Id, "thresholds").Allowed);
		f.Skills[2] = 50;
		f.Service.NotifyProgress(f.Actor.Object, traitId: 2, spellId: 1);
		AssertAcquired(f, 1);
		f.Skills[2] = 49; f.Acquire(2);
		f.Service.NotifyProgress(f.Actor.Object, traitId: 1, spellId: 1);
		AssertAcquired(f, 1);
		var opened = false;
		f.Actor.Setup(x => x.AddTrait(It.IsAny<ITraitDefinition>(), It.IsAny<double>()))
			.Returns<ITraitDefinition, double>((trait, value) =>
			{
				Assert.AreEqual(3L, trait.Id);
				Assert.IsNotNull(f.Store.Acquisition(100, 2), "Acquisition must precede native skill opening.");
				Assert.IsNull(f.Store.Acquisition(100, 3));
				f.Skills[trait.Id] = value; opened = true;
				// A native trait notification must not re-enter this identity's cascade.
				f.Service.NotifyProgress(f.Actor.Object, traitId: trait.Id);
				return true;
			});
		f.Skills[2] = 50;
		f.Service.NotifyProgress(f.Actor.Object, traitId: 2);
		Assert.IsTrue(opened);
		Assert.AreEqual(10.0, f.Skills[3]);
		Assert.AreEqual(1, f.Flushes);
		AssertAcquired(f, 1, 2, 3);
		AssertNoCasting(f);
	}

	[DataTestMethod]
	[DataRow("no enrolment")]
	[DataRow("disabled")]
	[DataRow("unavailable")]
	[DataRow("missing merit")]
	[DataRow("non-applicable merit")]
	[DataRow("temporary only")]
	[DataRow("unavailable focused body")]
	[DataRow("native skill only")]
	public void NotifyProgress_RestrictedRoute_DoesNotAcquireBranches(string restriction)
	{
		var f = new MagicCastingFixture();
		var cap = Configure(f, Chain(3), enabled: restriction != "disabled");
		Seed(f, cap);
		switch (restriction)
		{
			case "no enrolment": f.Store.Enrolments.Clear(); break;
			case "unavailable": f.ActiveCapabilities.Clear(); break;
			case "missing merit": f.Actor.SetupGet(x => x.Merits).Returns(Array.Empty<IMerit>()); break;
			case "non-applicable merit": Permanent(f, cap, false); break;
			case "temporary only":
				f.Actor.SetupGet(x => x.Merits).Returns(Array.Empty<IMerit>());
				var effect = new Mock<IGiveMagicCapabilityEffect>();
				effect.SetupGet(x => x.Capabilities).Returns(new[] { cap });
				f.Actor.Setup(x => x.CombinedEffectsOfType<IGiveMagicCapabilityEffect>()).Returns(new[] { effect.Object });
				break;
			case "unavailable focused body":
				var focused = new Mock<ICharacterInstance>();
				focused.SetupGet(x => x.Capabilities).Returns(Array.Empty<IMagicCapability>());
				focused.SetupGet(x => x.Merits).Returns(Array.Empty<IMerit>());
				var identity = new Mock<ICharacterIdentity>();
				var primary = new Mock<ICharacterInstance>();
				primary.SetupGet(x => x.Id).Returns(100);
				primary.SetupGet(x => x.IsPlayerCharacter).Returns(true);
				primary.SetupGet(x => x.Identity).Returns(identity.Object);
				primary.SetupGet(x => x.Capabilities).Returns(f.ActiveCapabilities);
				primary.SetupGet(x => x.Merits).Returns(f.Actor.Object.Merits);
				identity.SetupGet(x => x.PrimaryInstance).Returns(primary.Object);
				identity.SetupGet(x => x.FocusedInstance).Returns(focused.Object);
				f.Actor.SetupGet(x => x.Identity).Returns(identity.Object);
				f.Actor.SetupGet(x => x.IsPlayerCharacter).Returns(true);
				break;
			case "native skill only": f.Store.Acquired.Clear(); break;
		}
		var writes = f.Store.Writes;
		f.Service.NotifyProgress(f.Actor.Object, traitId: 1, spellId: 1);
		AssertAcquired(f, restriction == "native skill only" ? Array.Empty<int>() : new[] { 1 });
		Assert.AreEqual(writes, f.Store.Writes, restriction);
		Assert.AreEqual(0, f.Flushes);
		AssertNoCasting(f);
	}

	[TestMethod]
	public void NotifyProgress_OverlappingRoutes_AcquiresOneCanonicalSpell()
	{
		var f = new MagicCastingFixture();
		var cap = Configure(f, Chain(3)); Seed(f, cap);
		var other = f.NewCapability(91, 1, 11, true, Policy(Chain(3), 91));
		f.ActiveCapabilities.Add(other); Permanent(f, cap, true, other);
		SeedEnrolment(f, other);
		var writes = f.Store.Writes;
		f.Service.NotifyProgress(f.Actor.Object, traitId: 1, spellId: 1);
		AssertAcquired(f, 1, 2, 3);
		Assert.AreEqual(writes + 2, f.Store.Writes);
		AssertNoCasting(f);
	}

	[DataTestMethod]
	[DataRow("prerequisite spell")]
	[DataRow("prerequisite trait")]
	[DataRow("target spell")]
	[DataRow("target trait")]
	[DataRow("reserve")]
	public void NotifyProgress_QuarantinedPrerequisiteOrTarget_DoesNotAcquire(string blocked)
	{
		var f = new MagicCastingFixture();
		var root = Node(1); root.SetAttributeValue("trait", 2);
		var branch = Node(2, Edge(1)); branch.SetAttributeValue("trait", 3);
		var cap = Configure(f, new[] { Node(3, Edge(2)), branch, root }); Seed(f, cap);
		var independent = f.NewCapability(91, 1, 12, false, Policy(new[] { Node(4), Node(5, Edge(4)) }, 91, 12));
		f.ActiveCapabilities.Add(independent); Permanent(f, cap, true, independent);
		SeedEnrolment(f, independent);
		f.Store.Write(acquired: new(100, 4, 1, 1, f.Now, "independent root", DateTime.UnixEpoch, 0));
		f.Store.Write(new CastingOperation(Guid.NewGuid(), 100, 100, 200, cap.Id,
			blocked == "prerequisite spell" ? 1 : blocked == "target spell" ? 2 : 99,
			blocked == "prerequisite trait" ? 2 : blocked == "target trait" ? 3 : 99,
			blocked == "reserve" ? 11 : 99, "NeedsReview", "<Casting version='1'/>", f.Now, f.Now));
		var writes = f.Store.Writes;
		f.Service.NotifyProgress(f.Actor.Object, traitId: 1, spellId: 1);
		AssertAcquired(f, 1, 4, 5);
		Assert.AreEqual(writes + 1, f.Store.Writes, blocked);
		Assert.AreEqual(1, f.Store.Unresolved(100).Count);
		AssertNoCasting(f);
	}

	[TestMethod]
	public void NotifyProgress_RepeatedAndRestartedCascade_PreservesExistingState()
	{
		var f = new MagicCastingFixture();
		var cap = Configure(f, new[] { Node(5, Edge(4, 2, 100)), Node(4, Edge(3)), Node(3, Edge(2)), Node(2, Edge(1)), Node(1) });
		Seed(f, cap);
		f.Store.Write(acquired: f.Store.Acquisition(100, 1)! with { ControlledGrade = 2, NextMasteryUtc = f.Now.AddHours(1) });
		f.Store.Write(opportunity: new(100, 1, f.Now.AddMinutes(5), 0));
		f.Service.NotifyProgress(f.Actor.Object, traitId: 1, spellId: 1);
		AssertAcquired(f, 1, 2, 3, 4);
		var acquired = f.Store.Acquired.Values.OrderBy(x => x.SpellId).ToArray();
		var opportunities = f.Store.Opportunities.Values.ToArray();
		var skills = f.Skills.OrderBy(x => x.Key).ToArray();
		var balances = f.Balances.OrderBy(x => x.Key.Id).ToArray();
		var enrolments = f.Store.Enrolments.Values.ToArray();
		var writes = f.Store.Writes; var flushes = f.Flushes;
		for (var i = 0; i < 3; i++) f.Service.NotifyProgress(f.Actor.Object, traitId: 1, spellId: 1);
		f.Restart();
		f.Service.NotifyProgress(f.Actor.Object, traitId: 1, spellId: 1);
		f.Service.Reconcile(f.Actor.Object);
		CollectionAssert.AreEqual(acquired, f.Store.Acquired.Values.OrderBy(x => x.SpellId).ToArray());
		CollectionAssert.AreEqual(opportunities, f.Store.Opportunities.Values.ToArray());
		CollectionAssert.AreEqual(skills, f.Skills.OrderBy(x => x.Key).ToArray());
		CollectionAssert.AreEqual(balances, f.Balances.OrderBy(x => x.Key.Id).ToArray());
		CollectionAssert.AreEqual(enrolments, f.Store.Enrolments.Values.ToArray());
		Assert.AreEqual(writes, f.Store.Writes);
		Assert.AreEqual(flushes, f.Flushes);
		AssertNoCasting(f);
	}

	[DataTestMethod]
	[DataRow("multi-node cycle", "cycle")]
	[DataRow("self-cycle", "cycle")]
	[DataRow("duplicate prerequisite", "duplicate")]
	[DataRow("missing prerequisite", "not admitted")]
	public void NotifyProgress_InvalidPolicy_DoesNotAcquireOrLoop(string policy, string error)
	{
		var f = new MagicCastingFixture();
		var nodes = policy switch
		{
			"multi-node cycle" => new[] { Node(1, Edge(3)), Node(2, Edge(1)), Node(3, Edge(2)) },
			"self-cycle" => new[] { Node(1), Node(2, Edge(2), Edge(1)) },
			"duplicate prerequisite" => new[] { Node(1), Node(2, Edge(1), Edge(1)) },
			_ => new[] { Node(1), Node(2, Edge(1), Edge(99)) }
		};
		var cap = Configure(f, nodes, valid: false);
		Assert.IsTrue(cap.CastingConfigurationErrors().Any(x => x.Contains(error)), string.Join(';', cap.CastingConfigurationErrors()));
		Seed(f, cap);
		var writes = f.Store.Writes;
		f.Service.NotifyProgress(f.Actor.Object, traitId: 1, spellId: 1);
		AssertAcquired(f, 1);
		Assert.AreEqual(writes, f.Store.Writes);
		AssertNoCasting(f);
	}

	private static IEnumerable<XElement> Chain(int count) => Enumerable.Range(1, count).Reverse()
		.Select(id => id == 1 ? Node(id) : Node(id, Edge(id - 1)));

	private static XElement[] Diamond(bool reverse) => new[]
	{
		Node(1), Node(2, Edge(1)), Node(3, Edge(1)),
		reverse ? Node(4, Edge(3), Edge(2)) : Node(4, Edge(2), Edge(3))
	};

	private static XElement Node(int spell, params XElement[] edges)
	{
		var node = MagicCastingFixture.Admission(spell);
		node.SetAttributeValue("starting", spell == 1);
		node.Add(edges);
		return node;
	}

	private static XElement Edge(int spell, int grade = 1, double proficiency = 0) => new("Prerequisite",
		new XAttribute("key", Guid.NewGuid()), new XAttribute("spell", spell),
		new XAttribute("grade", grade), new XAttribute("proficiency", proficiency));

	private static XElement Policy(IEnumerable<XElement> nodes, int identity = 90, long reserve = 11, bool enabled = true) => new("Casting",
		new XAttribute("version", 1), new XAttribute("identity", new Guid(identity, 0, 0, new byte[8])),
		new XAttribute("enabled", enabled), new XAttribute("trait", 1), new XAttribute("source", 10),
		new XAttribute("reserve", reserve), new XAttribute("passive", reserve == 11), new XAttribute("startingVersion", 1), nodes);

	private static SkillLevelBasedMagicCapability Configure(MagicCastingFixture f, IEnumerable<XElement> nodes, bool enabled = true, bool valid = true)
	{
		var admissions = nodes.ToArray();
		f.Capabilities.Clear(); f.ActiveCapabilities.Clear();
		for (var id = 2; id <= Math.Max(5, admissions.Max(x => (int)x.Attribute("spell")!)); id++)
		{
			var spell = f.NewSpell(id, $"Spell {id}", "<Effect type='personaltagward'/>");
			Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("grades fixture")));
		}
		var cap = f.NewCapability(90, 1, 11, true, Policy(admissions, enabled: enabled));
		if (valid) Assert.AreEqual(0, cap.CastingConfigurationErrors().Count, string.Join(';', cap.CastingConfigurationErrors()));
		f.ActiveCapabilities.Add(cap); Permanent(f, cap);
		return cap;
	}

	private static void Permanent(MagicCastingFixture f, IMagicCapability cap, bool applies = true, params IMagicCapability[] others)
	{
		var merit = new Mock<IMagicCapabilityMerit>();
		merit.Setup(x => x.Applies(f.Actor.Object)).Returns(applies);
		merit.SetupGet(x => x.Capabilities).Returns(new[] { cap }.Concat(others));
		f.Actor.SetupGet(x => x.Merits).Returns(new IMerit[] { merit.Object });
	}

	private static void Seed(MagicCastingFixture f, SkillLevelBasedMagicCapability cap)
	{
		SeedEnrolment(f, cap); f.Acquire(1);
	}

	private static void SeedEnrolment(MagicCastingFixture f, SkillLevelBasedMagicCapability cap) =>
		f.Store.Write(enrolment: new(100, cap.CastingPolicy!.Identity, cap.Id, f.Now, cap.CastingPolicy.StartingGrantVersion));

	private static void AssertAcquired(MagicCastingFixture f, params int[] spells)
	{
		CollectionAssert.AreEqual(spells.Select(x => (long)x).Order().ToArray(), f.Store.Acquired.Values.Select(x => x.SpellId).Order().ToArray());
		foreach (var state in f.Store.Acquired.Values)
		{
			Assert.AreEqual(100L, state.CharacterId);
			Assert.AreEqual(((IControlledMagicSpell)f.Spells.Single(x => x.Id == state.SpellId)).GradeProfile!.Version, state.ProfileVersion);
			if (state.SpellId == 1) continue;
			Assert.AreEqual(1, state.ControlledGrade);
			Assert.AreEqual(1L, state.Version, $"Spell {state.SpellId} must be acquired once.");
		}
	}

	private static void AssertNoCasting(MagicCastingFixture f)
	{
		Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.SkillUses); Assert.AreEqual(0, f.Samples);
		foreach (var balance in f.Balances.Values) Assert.AreEqual(100.0, balance);
		f.Actor.Verify(x => x.UseResource(It.IsAny<IMagicResource>(), It.IsAny<double>()), Times.Never);
	}

	private static IEnumerable<int[]> Permutations(int[] values)
	{
		if (values.Length == 0) { yield return Array.Empty<int>(); yield break; }
		foreach (var first in values)
		foreach (var rest in Permutations(values.Where(x => x != first).ToArray()))
			yield return new[] { first }.Concat(rest).ToArray();
	}
}
