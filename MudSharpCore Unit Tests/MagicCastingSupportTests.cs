using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Merits.Interfaces;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicCastingSupportTests
{
	private static MagicCastingFixture Bridge()
	{
		var f = new MagicCastingFixture();
		var identify = f.NewSpell(2, "Identify", "<Effect type='boost' trait='1' bonus='0' context='0'/>");
		Assert.IsTrue(identify.BuildingCommand(f.Actor.Object, new StringStack("grades fixture")));
		foreach (var cmd in new[] { "casting entry add 2", "casting entry trait 2 2", "casting support add 3 30 90 off",
			"casting support prerequisite spell 3 1 1 80", "casting prerequisite trait 2 3 80" })
			Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack(cmd)), cmd);
		var merit = new Mock<IMagicCapabilityMerit>(); merit.Setup(x => x.Applies(f.Actor.Object)).Returns(true);
		merit.SetupGet(x => x.Capabilities).Returns(() => f.ActiveCapabilities);
		f.Actor.SetupGet(x => x.Merits).Returns([merit.Object]); f.Skills.Remove(3);
		Assert.AreEqual(0, f.Earth.CastingConfigurationErrors().Count);
		return f;
	}

	[TestMethod]
	public void TypedBridge_EnrolledParentAt80_OpensSupport30_AndNativeSupport80AcquiresIdentify()
	{
		var f = Bridge(); Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "native bridge").Allowed);
		Assert.IsFalse(f.Skills.ContainsKey(3)); f.Skills[1] = 80; f.Service.NotifyProgress(f.Actor.Object, traitId: 1);
		Assert.AreEqual(30.0, f.Skills[3]); Assert.IsNull(f.Service.Acquisition(f.Actor.Object, 2));
		Assert.AreEqual(90.0, f.Service.RawSkillImprovementCap(f.Actor.Object, 3));
		f.Skills[3] = 79.99; f.Service.NotifyProgress(f.Actor.Object, traitId: 3); Assert.IsNull(f.Service.Acquisition(f.Actor.Object, 2));
		f.Skills[3] = 80; f.Service.NotifyProgress(f.Actor.Object, traitId: 3);
		Assert.AreEqual(1, f.Service.Acquisition(f.Actor.Object, 2)!.ControlledGrade);
		Assert.IsNull(f.Service.Acquisition(f.Actor.Object, 3), "A support is never a dummy spell.");
		Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Samples); Assert.AreEqual(100.0, f.Balances[f.Resources[1]]);
		Assert.AreEqual(0, f.Store.Unresolved().Count);
	}

	[TestMethod]
	public void TypedBridge_UnscopedNativeSkillAndTemporaryPermission_DoNotAcquire()
	{
		var f = Bridge(); f.Acquire(); f.Skills[1] = 80; f.Skills[3] = 100;
		f.Service.NotifyProgress(f.Actor.Object, traitId: 3); Assert.IsNull(f.Service.Acquisition(f.Actor.Object, 2));
		f.Actor.SetupGet(x => x.Merits).Returns(Array.Empty<MudSharp.RPG.Merits.IMerit>());
		Assert.IsFalse(f.Service.EnrolAuthorised(f.Actor.Object, 1, "authored temporary permission").Allowed);
		Assert.IsNull(f.Store.Enrolment(100, f.Earth.CastingPolicy!.Identity));
	}

	[TestMethod]
	public void TypedBridge_InterruptedNativeOpening_ReconcileRepairsRecordedOpeningWithoutRepeatedGrant()
	{
		var f = Bridge(); f.Skills[1] = 80;
		f.Actor.Setup(x => x.AddTrait(f.Traits[2], It.IsAny<double>())).Returns(false);
		Assert.IsFalse(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "interrupted").Allowed);
		var support = f.Earth.CastingPolicy!.Supports.Single();
		Assert.IsNotNull(f.Store.SupportGrant(100, f.Earth.CastingPolicy.Identity, support.Key));
		Assert.IsFalse(f.Skills.ContainsKey(3));
		f.Actor.Setup(x => x.AddTrait(f.Traits[2], It.IsAny<double>())).Returns<ITraitDefinition, double>((t, n) => { f.Skills[t.Id] = n; return true; });
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "retry interrupted opening").Allowed);
		Assert.AreEqual(30.0, f.Skills[3]);
		f.Restart(); f.Service.Reconcile(f.Actor.Object);
		f.Skills[3] = 83; var writes = f.Store.Writes; f.Service.Reconcile(f.Actor.Object);
		Assert.AreEqual(83.0, f.Skills[3]); Assert.IsNotNull(f.Service.Acquisition(f.Actor.Object, 2));
		Assert.AreEqual(writes + 1, f.Store.Writes);
	}

	[TestMethod]
	public void TypedGraph_MixedCycleMissingScopedTraitAndXmlReload_FailClosedOrRetainKeys()
	{
		var f = Bridge(); var reload = f.NewCapability(3, 1, 11, true, XElement.Parse(f.Earth.SaveToXml()).Element("Casting"));
		Assert.AreEqual(f.Earth.CastingPolicy!.Supports.Single().Key, reload.CastingPolicy!.Supports.Single().Key);
		Assert.AreEqual(MagicCastingPrerequisiteKind.SupportTrait, reload.CastingPolicy.Admissions.Single(x => x.SpellId == 2).Prerequisites.Single().Kind);
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting prerequisite trait 1 3 80")));
		Assert.IsTrue(f.Earth.CastingConfigurationErrors().Any(x => x.Contains("cycle")));
		Assert.IsFalse(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "cycle").Allowed);
		Assert.IsTrue(reload.BuildingCommand(f.Actor.Object, new StringStack("casting support remove 3")));
		Assert.IsTrue(reload.CastingConfigurationErrors().Any(x => x.Contains("unscoped")));
	}

	[TestMethod]
	public void AuthoredEnrolment_CompiledTypedProg_IsPermanentIdempotentAndProvenanced()
	{
		FutureProgTestBootstrap.EnsureInitialised(); var f = Bridge();
		var prog = new FutureProg(f.World.Object, "testEnrolment", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Character, "actor"), Tuple.Create(ProgVariableTypes.MagicCapability, "cap"), Tuple.Create(ProgVariableTypes.Text, "reason")],
			"return enrolchannelcasting(@actor, @cap, @reason)");
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		Assert.IsTrue(prog.ExecuteBool(f.Actor.Object, f.Earth, "selected NPC creation"));
		Assert.IsTrue(f.Service.Acquisition(f.Actor.Object, 1)!.Provenance.Contains("selected NPC creation"));
		var writes = f.Store.Writes; Assert.IsTrue(prog.ExecuteBool(f.Actor.Object, f.Earth, "repeat")); Assert.AreEqual(writes, f.Store.Writes);
		Assert.AreEqual(100.0, f.Balances[f.Resources[1]]);
		Assert.IsFalse(prog.ExecuteBool(f.Actor.Object, f.Earth, ""));
	}

	[DataTestMethod]
	[DataRow(false, false)]
	[DataRow(false, true)]
	[DataRow(true, false)]
	[DataRow(true, true)]
	public void TypedDiamond_PermutedAdmissionsAndSupports_OpeningEnablesEveryParentInSameCascade(bool reverseSpells, bool reverseSupports)
	{
		var f = Bridge(); var xml = XElement.Parse(f.Earth.SaveToXml()).Element("Casting")!;
		xml.Element("SupportGrant")!.SetAttributeValue("opening", 80);
		xml.Add(new XElement("SupportGrant", new XAttribute("key", Guid.NewGuid()), new XAttribute("trait", 2),
			new XAttribute("opening", 80), new XAttribute("rawCap", 90), new XAttribute("starting", false),
			new XElement("Prerequisite", new XAttribute("key", Guid.NewGuid()), new XAttribute("spell", 1), new XAttribute("grade", 1), new XAttribute("proficiency", 80))));
		xml.Elements("Admission").Single(x => (long)x.Attribute("spell")! == 2).Add(new XElement("Prerequisite",
			new XAttribute("key", Guid.NewGuid()), new XAttribute("kind", "trait"), new XAttribute("trait", 2), new XAttribute("proficiency", 80)));
		if (reverseSpells) { var nodes = xml.Elements("Admission").ToArray(); nodes.Remove(); xml.Add(nodes.Reverse()); }
		if (reverseSupports) { var nodes = xml.Elements("SupportGrant").ToArray(); nodes.Remove(); xml.Add(nodes.Reverse()); }
		var cap = f.NewCapability(91, 1, 11, true, xml); f.ActiveCapabilities.Clear(); f.ActiveCapabilities.Add(cap);
		f.Skills.Remove(2); f.Skills[1] = 80;
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, cap.Id, "typed diamond").Allowed);
		Assert.AreEqual(80.0, f.Skills[2]); Assert.AreEqual(80.0, f.Skills[3]);
		Assert.AreEqual(1, f.Service.Acquisition(f.Actor.Object, 2)!.ControlledGrade);
		Assert.AreEqual(2, f.Store.Operations.Values.Count(x => MagicCastingStateStore.IsSupportRecord(x.Stage)));
		var writes = f.Store.Writes; f.Service.Reconcile(f.Actor.Object); Assert.AreEqual(writes, f.Store.Writes);
		Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Samples);
	}

	[TestMethod]
	public void TypedSupport_FailedAuthorisationWrite_CannotOpenSkill_ThenDurableRetryRepairs()
	{
		var f = Bridge(); f.Skills[1] = 80;
		f.Store.BeforeWrite = op => { if (op is not null && MagicCastingStateStore.IsSupportRecord(op.Stage)) throw new InvalidOperationException("injected support write failure"); };
		Assert.IsFalse(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "failed support write").Allowed);
		Assert.IsFalse(f.Skills.ContainsKey(3)); Assert.IsNull(f.Service.Acquisition(f.Actor.Object, 2));
		Assert.AreEqual(0, f.Store.Operations.Values.Count(x => MagicCastingStateStore.IsSupportRecord(x.Stage)));
		f.Store.BeforeWrite = null;
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "retry failed support write").Allowed);
		Assert.AreEqual(30.0, f.Skills[3]); Assert.AreEqual(1, f.Store.Operations.Values.Count(x => MagicCastingStateStore.IsSupportRecord(x.Stage)));
	}

	[TestMethod]
	public void TypedSupport_RetiredDefinition_PreservesCapHistoryAndImmutableAuthorisation()
	{
		var f = Bridge(); f.Skills[1] = 80; f.Skills[3] = 84;
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "existing native support").Allowed);
		Assert.AreEqual(84.0, f.Skills[3]);
		var record = f.Store.Operations.Values.Single(x => MagicCastingStateStore.IsSupportRecord(x.Stage));
		var writes = f.Store.Writes;
		Assert.IsFalse(f.Service.ReconcileOperation(f.Staff.Object, f.Actor.Object, record.Id, "cannot erase support cap history").Changed);
		Assert.AreEqual(writes, f.Store.Writes);
		Assert.AreEqual(record, f.Store.Operation(record.Id));
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting prerequisite traitremove 2 3")));
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting support remove 3")));
		f.Restart(); Assert.AreEqual(0.0, f.Service.RawSkillImprovementCap(f.Actor.Object, 3));
		Assert.AreEqual(84.0, f.Skills[3]); Assert.AreEqual(0, f.Store.Unresolved().Count);
	}

	[TestMethod]
	public void TypedSupport_SecondFocusedBody_SharesGrantSkillAcquisitionAndRawCapWithoutDuplicatingRoots()
	{
		var f = Bridge(); var identity = new Mock<ICharacterIdentity>();
		var primary = new Mock<ICharacterInstance>(); var secondary = new Mock<ICharacterInstance>();
		primary.SetupGet(x => x.Id).Returns(100); primary.SetupGet(x => x.Identity).Returns(identity.Object);
		primary.SetupGet(x => x.Body).Returns(f.Body.Object); primary.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		primary.SetupGet(x => x.IsPlayerCharacter).Returns(true); primary.SetupGet(x => x.Capabilities).Returns(f.ActiveCapabilities);
		primary.Setup(x => x.HasTrait(It.IsAny<ITraitDefinition>())).Returns<ITraitDefinition>(x => f.Skills.ContainsKey(x.Id));
		primary.Setup(x => x.TraitRawValue(It.IsAny<ITraitDefinition>())).Returns<ITraitDefinition>(x => f.Skills.TryGetValue(x.Id, out var value) ? value : 0.0);
		primary.Setup(x => x.AddTrait(It.IsAny<ITraitDefinition>(), It.IsAny<double>())).Returns<ITraitDefinition, double>((t, n) => { f.Skills[t.Id] = n; return true; });
		secondary.SetupGet(x => x.Id).Returns(500); secondary.SetupGet(x => x.Identity).Returns(identity.Object);
		secondary.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		secondary.SetupGet(x => x.Capabilities).Returns(f.ActiveCapabilities); secondary.SetupGet(x => x.Body).Returns(f.Body.Object);
		var merit = Mock.Get((IMagicCapabilityMerit)f.Actor.Object.Merits.Single());
		merit.Setup(x => x.Applies(primary.Object)).Returns(true); merit.Setup(x => x.Applies(secondary.Object)).Returns(true);
		primary.SetupGet(x => x.Merits).Returns([merit.Object]); secondary.SetupGet(x => x.Merits).Returns([merit.Object]);
		identity.SetupGet(x => x.PrimaryInstance).Returns(primary.Object); identity.SetupGet(x => x.FocusedInstance).Returns(primary.Object);
		f.Actor.SetupGet(x => x.Identity).Returns(identity.Object); f.Skills[1] = 80;
		Assert.IsTrue(f.Service.Enrol(f.Staff.Object, f.Actor.Object, 1, "canonical support").Allowed); Assert.AreEqual(30.0, f.Skills[3]);
		identity.SetupGet(x => x.FocusedInstance).Returns(secondary.Object); f.Skills[3] = 80;
		f.Service.NotifyProgress(secondary.Object, traitId: 3); Assert.AreEqual(1, f.Service.Acquisition(secondary.Object, 2)!.ControlledGrade);
		Assert.IsNotNull(f.Store.Acquisition(100, 2)); Assert.IsNull(f.Store.Acquisition(500, 2));
		var writes = f.Store.Writes;
		Assert.IsTrue(f.Service.EnrolAuthorised(secondary.Object, 1, "second body retry").Allowed);
		Assert.AreEqual(writes, f.Store.Writes); Assert.AreEqual(1, f.Store.Operations.Values.Count(x => MagicCastingStateStore.IsSupportRecord(x.Stage)));
		Assert.AreEqual(90.0, f.Service.RawSkillImprovementCap(secondary.Object, 3));
		var definition = new SkillDefinition(new MudSharp.Models.TraitDefinition { Id = 3, Name = "Shared support" }, f.World.Object)
			{ Cap = new MudSharp.Body.Traits.TraitExpression("100", f.World.Object) };
		var skill = new Skill(definition, 89, secondary.Object); skill.Value += 10; Assert.AreEqual(90.0, skill.RawValue);
		secondary.SetupGet(x => x.Merits).Returns(Array.Empty<MudSharp.RPG.Merits.IMerit>());
		f.Service.Reconcile(secondary.Object); Assert.AreEqual(0.0, f.Service.RawSkillImprovementCap(secondary.Object, 3));
		skill.Value += 10; Assert.AreEqual(90.0, skill.RawValue);
		Assert.AreEqual(1, f.Store.Enrolments.Count); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.Samples);
	}
}
