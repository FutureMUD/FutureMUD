#nullable enable

using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Performances;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PerformanceAuthoringTests
{
	[TestMethod]
	public void Editor_RestrictsWorkIdSelectionAndExplainsSingleCueBounds()
	{
		var player = Character(1, false);
		var staff = Character(2, true);
		var service = new PerformanceService(Mock.Of<IFuturemud>());
		var work = new PerformanceWork
		{
			Id = 345, OwnerId = 1, Name = "An address",
			Cues = [new PerformanceCue { Verb = PerformanceVerb.Wait }]
		};
		service.Works.Add(work);
		var output = new Mock<IOutputHandler>();
		player.SetupGet(x => x.OutputHandler).Returns(output.Object);
		staff.SetupGet(x => x.OutputHandler).Returns(output.Object);

		PerformanceAuthoring.Execute(player.Object, new StringStack("show 345"), service);
		output.Verify(x => x.Send(It.Is<string>(s => s.Contains("No visible work") && s.Contains("performance work list")), true, false));
		PerformanceAuthoring.Execute(staff.Object, new StringStack("show 345"), service);
		output.Verify(x => x.Send(It.Is<string>(s => s.Contains("An address") && s.Contains("Cues")), true, false));

		PerformanceAuthoring.Execute(player.Object, new StringStack("edit \"An address\" cue remove 0"), service);
		output.Verify(x => x.Send(It.Is<string>(s => s.Contains("Cue number 0") && s.Contains("cue remove 1")), true, false));
		Assert.AreEqual(1, work.Cues.Count);
	}

	[TestMethod]
	public void ValidateDefinition_FixturesAreValidWithoutAWorldOrBuilder()
	{
		Assert.IsNull(PerformanceAuthoring.ValidateDefinition(PerformanceAuthoring.CreateFixtureTemplate("mass")));
		Assert.IsNull(PerformanceAuthoring.ValidateDefinition(PerformanceAuthoring.CreateFixtureTemplate("concert")));
		Assert.IsNull(PerformanceAuthoring.ValidateDefinition(PerformanceAuthoring.CreateFixtureTemplate("procession")));
	}

	[TestMethod]
	public void ValidateDefinition_ReportsBrokenCueReferenceAndCleanupPolicy()
	{
		var work = PerformanceAuthoring.CreateFixtureTemplate("procession");
		work.Cues[0].Target = "torch";
		StringAssert.Contains(PerformanceAuthoring.ValidateDefinition(work), "unknown prop 'torch'");

		work.Cues[0].Target = "lantern";
		work.LeaveProps.Add("torch");
		StringAssert.Contains(PerformanceAuthoring.ValidateDefinition(work), "cleanup policy");
	}

	[TestMethod]
	public void NonNegativeIntegerError_ExplainsBadTextAndLowerBound()
	{
		StringAssert.Contains(PerformanceAuthoring.NonNegativeIntegerError("5!0", "Cue duration"), "'5!0'");
		StringAssert.Contains(PerformanceAuthoring.NonNegativeIntegerError("5!0", "Cue duration"), "0 or more");
		StringAssert.Contains(PerformanceAuthoring.NonNegativeIntegerError("-2", "Cue duration"), "You entered -2");
		Assert.IsNull(PerformanceAuthoring.NonNegativeIntegerError("0", "Cue duration"));
		Assert.IsNull(PerformanceAuthoring.NonNegativeIntegerError("12", "Cue duration"));
	}

	[TestMethod]
	public void FixtureTemplates_AreStructurallyValidAndHaveExpectedCastAndCueSequence()
	{
		var mass = PerformanceAuthoring.CreateFixtureTemplate("mass");
		var concert = PerformanceAuthoring.CreateFixtureTemplate("concert");
		var procession = PerformanceAuthoring.CreateFixtureTemplate("procession");

		Assert.IsNull(PerformanceAuthoring.Validate(null!, mass));
		Assert.IsNull(PerformanceAuthoring.Validate(null!, concert));
		Assert.IsNull(PerformanceAuthoring.Validate(null!, procession));

		CollectionAssert.AreEqual(new[] { "celebrant", "reader", "server" }, mass.Roles.Select(x => x.Key).ToArray());
		CollectionAssert.AreEqual(new[]
		{
			PerformanceVerb.Get, PerformanceVerb.Travel, PerformanceVerb.Speak, PerformanceVerb.Give,
			PerformanceVerb.Speak, PerformanceVerb.Give, PerformanceVerb.Speak
		}, mass.Cues.Select(x => x.Verb).ToArray());
		Assert.AreEqual(PerformanceMode.Either, mass.Roles.Single(x => x.Key == "reader").Mode);
		Assert.IsTrue(mass.Cues.Any(x => x.Verb == PerformanceVerb.Give && x.Target == "missal" && x.Text == "reader"));

		CollectionAssert.AreEqual(new[] { "guitarist", "singer", "introducer", "usher-stalls", "usher-gallery" },
		                         concert.Roles.Select(x => x.Key).ToArray());
		Assert.AreEqual("Here Comes the Sun", concert.Name);
		Assert.AreEqual(PerformanceMode.Npc, concert.Roles.Single(x => x.Key == "guitarist").Mode);
		Assert.AreEqual(PerformanceMode.Pc, concert.Roles.Single(x => x.Key == "introducer").Mode);
		Assert.IsFalse(concert.Roles.Single(x => x.Key == "introducer").Required);
		CollectionAssert.AreEqual(new[]
		{
			PerformanceVerb.Travel, PerformanceVerb.Guard, PerformanceVerb.Travel, PerformanceVerb.Guard, PerformanceVerb.Stage, PerformanceVerb.Stage,
			PerformanceVerb.Speak, PerformanceVerb.Play, PerformanceVerb.Sing, PerformanceVerb.Stop,
			PerformanceVerb.Emote, PerformanceVerb.Unstage, PerformanceVerb.Unstage
		}, concert.Cues.Select(x => x.Verb).ToArray());
		Assert.IsTrue(concert.Cues.Where(x => x.Verb is PerformanceVerb.Stage or PerformanceVerb.Unstage)
			.All(x => string.IsNullOrEmpty(x.Role) && !x.Target.Contains(' ')));
		Assert.IsTrue(concert.Cues.Where(x => x.Verb == PerformanceVerb.Play).All(x => x.Text.Length == 0));

		CollectionAssert.AreEqual(new[] { "chapel", "square", "shrine" }, procession.Places.ToArray());
		Assert.AreEqual(PerformanceMode.Pc, procession.Roles.Single(x => x.Key == "petitioner").Mode);
		Assert.AreEqual(2, procession.Exits.Count);
		Assert.AreEqual(6, procession.Cues.Count(x => x.Verb == PerformanceVerb.Travel));
		Assert.AreEqual("square", procession.Cues.First(x => x.Target == "chapel-square" && x.Verb == PerformanceVerb.Travel).Place);
		Assert.AreEqual("shrine", procession.Cues.First(x => x.Target == "square-shrine" && x.Verb == PerformanceVerb.Travel).Place);
		Assert.AreEqual(PerformanceVerb.Give, procession.Cues[^2].Verb);
	}

	[TestMethod]
	public void CanViewAndEdit_RespectOwnershipPublicationAndStaffAuthority()
	{
		var owner = Character(10, false);
		var other = Character(20, false);
		var staff = Character(30, true);
		var draft = new PerformanceWork { OwnerId = 10 };
		var published = new PerformanceWork { Id = 321, OwnerId = 10, Published = true, StaffAuthored = true };

		Assert.IsTrue(PerformanceAuthoring.CanView(owner.Object, draft));
		Assert.IsFalse(PerformanceAuthoring.CanView(other.Object, draft));
		Assert.IsTrue(PerformanceAuthoring.CanView(staff.Object, draft));
		Assert.IsTrue(PerformanceAuthoring.CanView(other.Object, published));
		Assert.IsTrue(PerformanceAuthoring.CanEdit(owner.Object, draft));
		Assert.IsFalse(PerformanceAuthoring.CanEdit(other.Object, draft));
		Assert.IsTrue(PerformanceAuthoring.CanEdit(staff.Object, published));
		Assert.IsNull(PerformanceAuthoring.CreateCopy(other.Object, draft, ""));

		var copy = PerformanceAuthoring.CreateCopy(other.Object, published, "Shared copy");
		Assert.IsNotNull(copy);
		Assert.AreEqual(20L, copy.OwnerId);
		Assert.AreEqual("Shared copy", copy.Name);
		Assert.IsFalse(copy.Published);
		Assert.IsFalse(copy.StaffAuthored);
		Assert.AreNotEqual(published.Id, copy.Id);
	}

	[TestMethod]
	public void Publish_WhenValidWorkIsPublished_RecordsWhetherPublisherIsStaff()
	{
		var player = Character(10, false);
		var staff = Character(30, true);
		var playerWork = PerformanceAuthoring.CreateFixtureTemplate("mass");
		playerWork.OwnerId = 10;
		var staffWork = PerformanceAuthoring.CreateFixtureTemplate("concert");
		staffWork.OwnerId = 10;

		Assert.IsNull(PerformanceAuthoring.Publish(player.Object, playerWork));
		Assert.IsTrue(playerWork.Published);
		Assert.IsFalse(playerWork.StaffAuthored);
		Assert.IsNull(PerformanceAuthoring.Publish(staff.Object, staffWork));
		Assert.IsTrue(staffWork.Published);
		Assert.IsTrue(staffWork.StaffAuthored);
	}

	[TestMethod]
	public void Publish_WhenWorkIsInvalid_LeavesDraftStateUntouched()
	{
		var actor = Character(10, false);
		var work = PerformanceAuthoring.CreateFixtureTemplate("mass");
		work.OwnerId = 10;
		work.Cues[0].Target = "missing-prop";

		Assert.IsNotNull(PerformanceAuthoring.Publish(actor.Object, work));
		Assert.IsFalse(work.Published);
		Assert.IsFalse(work.StaffAuthored);
	}

	[TestMethod]
	public void Validate_WhenCueReferencesUnknownRole_ExplainsTheBrokenReference()
	{
		var work = PerformanceAuthoring.CreateFixtureTemplate("mass");
		work.Cues.Add(new PerformanceCue { Role = "unknown", Verb = PerformanceVerb.Speak });

		StringAssert.Contains(PerformanceAuthoring.Validate(null!, work), "unknown role 'unknown'");
	}

	[TestMethod]
	public void Validate_WhenCueHasUndefinedNumericVerb_RejectsIt()
	{
		var work = PerformanceAuthoring.CreateFixtureTemplate("mass");
		work.Cues[0].Verb = (PerformanceVerb)999;

		Assert.AreEqual("A cue has an unknown action.", PerformanceAuthoring.Validate(null!, work));
	}

	[TestMethod]
	public void CloneForEdit_WhenForkingPublishedRevision_CreatesIndependentNextRevision()
	{
		var source = PerformanceAuthoring.CreateFixtureTemplate("mass");
		source.Id = 91;
		source.OwnerId = 10;
		source.Revision = 4;
		source.Published = true;
		var fork = PerformanceAuthoring.CloneForEdit(source, 20, forkRevision: true);

		Assert.AreEqual(0L, fork.Id);
		Assert.AreEqual(20L, fork.OwnerId);
		Assert.AreEqual(5, fork.Revision);
		Assert.IsFalse(fork.Published);
		Assert.AreNotSame(source.Roles[0], fork.Roles[0]);
		fork.Roles[0].Mark = "changed";
		Assert.AreEqual("sanctuary", source.Roles[0].Mark);
	}

	[TestMethod]
	public void CloneForEdit_WhenEditingDraft_PreservesItsRevisionIdentity()
	{
		var source = PerformanceAuthoring.CreateFixtureTemplate("concert");
		source.Id = 92;
		source.Revision = 2;
		source.OwnerId = 123;
		var candidate = PerformanceAuthoring.CloneForEdit(source, 456, forkRevision: false);

		Assert.AreEqual(source.OwnerId, candidate.OwnerId);
		Assert.AreEqual(source.Id, candidate.Id);
		Assert.AreEqual(source.Revision, candidate.Revision);
		Assert.AreNotSame(source.Cues[0], candidate.Cues[0]);
	}

	[TestMethod]
	public void PlayerProgLookupFiltersVisibilityAndSignatureBeforeSelectingAnOverload()
	{
		var world = new Mock<IFuturemud>();
		var progs = new All<IFutureProg>();
		world.SetupGet(x => x.FutureProgs).Returns(progs);
		var player = Character(1, false);
		player.SetupGet(x => x.Gameworld).Returns(world.Object);
		var staff = Character(2, true);
		staff.SetupGet(x => x.Gameworld).Returns(world.Object);
		foreach (var id in new[] { 11L, 12L, 13L })
		{
			var rule = new Mock<IFutureProg>();
			rule.SetupGet(x => x.Id).Returns(id);
			rule.SetupGet(x => x.FunctionName).Returns("Eligible");
			rule.SetupGet(x => x.Public).Returns(id != 11);
			rule.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
			rule.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(id != 12);
			progs.Add(rule.Object);
		}
		Assert.AreEqual(13L, PerformanceAuthoring.FindRule(player.Object, "eligible", ProgVariableTypes.Character, false)?.Id);
		Assert.IsNull(PerformanceAuthoring.FindRule(player.Object, "13", ProgVariableTypes.Character, false));
		Assert.AreEqual(11L, PerformanceAuthoring.FindRule(staff.Object, "11", ProgVariableTypes.Character, true)?.Id);
	}

	private static Mock<ICharacter> Character(long id, bool staff)
	{
		var character = new Mock<ICharacter>();
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Account).Returns(account.Object);
		character.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(staff);
		return character;
	}
}
