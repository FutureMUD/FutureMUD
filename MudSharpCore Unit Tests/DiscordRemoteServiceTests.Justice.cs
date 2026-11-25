#nullable enable

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Network;
using MudSharp.RPG.Law;

namespace MudSharp_Unit_Tests;

public partial class DiscordRemoteServiceTests
{
	[TestMethod]
	public void JusticeStatus_RequiresCurrentStaffAuthorityAndResolvesJurisdictionsUnambiguously()
	{
		var f = new Fixture();
		var jurisdictions = new All<ILegalAuthority>();
		f.World.SetupGet(x => x.LegalAuthorities).Returns(jurisdictions);
		var request = f.Request("justice.status");
		f.Authority.SetupGet(x => x.Level).Returns(PermissionLevel.Guide);
		Assert.AreEqual("denied", f.Service.Process(request).Status);
		f.World.VerifyGet(x => x.LegalAuthorities, Times.Never);
		f.Authority.SetupGet(x => x.Level).Returns(PermissionLevel.JuniorAdmin);
		StringAssert.Contains(f.Service.Process(request).Text, "No jurisdictions");
		foreach (var id in Enumerable.Range(1, 7))
		{
			jurisdictions.Add(Mock.Of<ILegalAuthority>(x => x.Id == id && x.Name == "Council"));
		}
		string Run(string arguments) => f.Service.Process(f.Request("justice.status") with { Arguments = arguments }).Text;
		StringAssert.Contains(Run("{}"), "/justice status page:2");
		StringAssert.Contains(Run("""{"page":2}"""), "#7 Council");
		StringAssert.Contains(Run("""{"jurisdiction":"Council"}"""), "share that name");
		StringAssert.Contains(Run("""{"jurisdiction":"Coun"}"""), "Unknown jurisdiction");
		StringAssert.Contains(Run("""{"jurisdiction":"#7"}"""), "does not have a courtroom");
		StringAssert.Contains(Run("""{"page":2147483647}"""), "past the end");
		f.Account.SetupGet(x => x.AccountStatus).Returns(AccountStatus.Suspended);
		Assert.AreEqual("denied", f.Service.Process(request).Status);
		f.Store.Link = null;
		Assert.AreEqual("denied", f.Service.Process(request).Status);
	}

	[TestMethod]
	public void JusticeStatus_ShowsScopedWaitingDefendantsCourtAvailabilityAndLaunchedPatrols()
	{
		var f = new Fixture();
		var court = new Mock<IRoom>();
		var remand = new Mock<IRoom>();
		var local = new Mock<ILegalAuthority>();
		local.SetupGet(x => x.Id).Returns(7);
		local.SetupGet(x => x.Name).Returns("Harbour Court");
		local.SetupGet(x => x.CourtLocation).Returns(court.Object);
		local.SetupGet(x => x.RoomLocations).Returns([remand.Object]);
		var other = Mock.Of<ILegalAuthority>(x => x.Id == 8 && x.Name == "Hill Court");
		f.World.SetupGet(x => x.LegalAuthorities).Returns(new All<ILegalAuthority> { local.Object, other });
		var crimes = new List<ICrime>();
		local.SetupGet(x => x.KnownCrimes).Returns(crimes);
		// Older charges can point to a different body of the same identity.
		Defendant(2, "Earlier body outside custody");
		var waiting = Defendant(2, "Ada");
		waiting.SetupGet(x => x.Location).Returns(remand.Object);
		waiting.Setup(x => x.AffectedBy<AwaitingSentencing>(local.Object)).Returns(true);
		crimes.Add(crimes.Last());
		var bail = Defendant(3, "Beth");
		bail.Setup(x => x.AffectedBy<OnBail>(local.Object)).Returns(true);
		var absent = Defendant(4, "Cai");
		absent.Setup(x => x.AffectedBy<AwaitingSentencing>(local.Object)).Returns(true);
		var hearing = Defendant(5, "Already Heard");
		hearing.Setup(x => x.AffectedBy<OnTrial>(local.Object)).Returns(true);
		hearing.Setup(x => x.AffectedBy<AwaitingSentencing>(local.Object)).Returns(true);
		Defendant(6, "Elsewhere").Setup(x => x.AffectedBy<OnBail>(other)).Returns(true);
		var judge = new Mock<ICharacter>();
		judge.SetupGet(x => x.Location).Returns(remand.Object);
		var patrol = Mock.Of<IPatrol>(x => x.Id == 12 && x.PatrolRoute.Name == "Morning Bench" &&
			x.PatrolStrategy.Name == "Judge" && x.PatrolLeader == judge.Object && x.PatrolPhase == PatrolPhase.Deployment);
		string Report() => f.Service.Process(f.Request("justice.status") with { Arguments = """{"jurisdiction":"harbour court"}""" }).Text;
		StringAssert.Contains(Report(), "does not have any hearings scheduled");
		local.SetupGet(x => x.Patrols).Returns([patrol]);
		var report = Report();
		StringAssert.Contains(report, "Waiting for trial (3)");
		StringAssert.Contains(report, "Ada — in remand");
		StringAssert.Contains(report, "Beth — on bail");
		StringAssert.Contains(report, "Cai — at large / bail revoked");
		Assert.IsFalse(report.Contains("Already Heard") || report.Contains("Elsewhere"));
		StringAssert.Contains(report, "does not have a judge available");
		StringAssert.Contains(report, "#12 Morning Bench — Judge, Deployment");
		judge.SetupGet(x => x.Location).Returns(court.Object);
		var player = Mock.Of<ICharacter>(x => x.Location == court.Object && x.State == CharacterState.Awake);
		local.Setup(x => x.GetEnforcementAuthority(player)).Returns(Mock.Of<IEnforcementAuthority>(x => x.CanConvict));
		var connection = Mock.Of<IPlayerConnection>(x => x.State == ConnectionState.Open && x.ControlPuppet.Actor == player);
		f.World.SetupGet(x => x.Connections).Returns([connection]);
		report = Report();
		StringAssert.Contains(report, "NPC judge available in court");
		StringAssert.Contains(report, "player-held trials in court: 1");
		court.SetupGet(x => x.Characters).Returns([hearing.Object]);
		StringAssert.Contains(Report(), "already has a trial being heard");
		for (var id = 20; id < 24; id++)
		{
			Defendant(id, $"Defendant {id}").Setup(x => x.AffectedBy<OnBail>(local.Object)).Returns(true);
		}
		StringAssert.Contains(Report(), "/justice status jurisdiction:7 page:2");
		var last = f.Service.Process(f.Request("justice.status") with { Arguments = """{"jurisdiction":"7","page":2}""" }).Text;
		StringAssert.Contains(last, "#23 Defendant 23");
		Assert.IsFalse(last.Contains("Morning Bench"));

		Mock<ICharacter> Defendant(long id, string name)
		{
			var defendant = new Mock<ICharacter>();
			defendant.SetupGet(x => x.PersonalName).Returns(Mock.Of<IPersonalName>(x => x.GetName(NameStyle.FullName) == name));
			crimes.Add(Mock.Of<ICrime>(x => x.CriminalId == id && x.Criminal == defendant.Object));
			return defendant;
		}
	}
}
