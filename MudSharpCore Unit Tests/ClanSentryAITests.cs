#nullable enable

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Form.Shape;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ClanSentryAITests
{
	[TestMethod]
	public void Departure_PrivateClanAlertUsesSentrySightAndNeverPublicRoomOutput()
	{
		var world = new Mock<IFuturemud>(); var clan = new Mock<IClan>(); clan.SetupGet(x => x.Id).Returns(7);
		var clans = new All<IClan>(); clans.Add(clan.Object); world.SetupGet(x => x.Clans).Returns(clans);
		var origin = new Mock<IRoom>();
		var sentry = new Mock<ICharacter>(); sentry.SetupGet(x => x.Location).Returns(origin.Object);
		sentry.SetupGet(x => x.State).Returns(CharacterState.Awake);
		var traveller = new Mock<ICharacter>();
		traveller.SetupGet(x => x.ClanMemberships).Returns(Array.Empty<IClanMembership>());
		traveller.Setup(x => x.HowSeen(sentry.Object, true, DescriptionType.Short, true, PerceiveIgnoreFlags.None)).Returns("A hooded traveller");
		sentry.Setup(x => x.CanSee(traveller.Object, PerceiveIgnoreFlags.None)).Returns(true);
		var member = new Mock<ICharacter>(); member.SetupGet(x => x.State).Returns(CharacterState.Awake);
		member.SetupGet(x => x.Id).Returns(101);
		var membership = new Mock<IClanMembership>(); membership.SetupGet(x => x.Clan).Returns(clan.Object);
		member.SetupGet(x => x.ClanMemberships).Returns([membership.Object]);
		var outsider = new Mock<ICharacter>(); outsider.SetupGet(x => x.State).Returns(CharacterState.Awake);
		outsider.SetupGet(x => x.Id).Returns(102);
		outsider.SetupGet(x => x.ClanMemberships).Returns(Array.Empty<IClanMembership>());
		var privateOutput = new Mock<IOutputHandler>(); var publicOutput = new Mock<IOutputHandler>();
		member.SetupGet(x => x.OutputHandler).Returns(privateOutput.Object); outsider.SetupGet(x => x.OutputHandler).Returns(publicOutput.Object);
		var actors = new All<ICharacter>(); actors.Add(member.Object); actors.Add(outsider.Object);
		world.SetupGet(x => x.Actors).Returns(actors);
		var ai = new ClanSentryAI(new MudSharp.Models.ArtificialIntelligence { Id = 100, Type = "ClanSentry", Name = "Sentry",
			Definition = new XElement("Definition", new XElement("Clan", 7), new XElement("HideAfterAlert", true),
				new XElement("Directions", new XElement("Direction", (int)CardinalDirection.East))).ToString() }, world.Object);
		var exit = new Mock<IRoomExit>(); exit.SetupGet(x => x.Origin).Returns(origin.Object); exit.SetupGet(x => x.OutboundDirection).Returns(CardinalDirection.East);
		Assert.IsTrue(ai.HandleEvent(EventType.CharacterLeaveRoomWitness, traveller.Object, origin.Object, exit.Object, sentry.Object));
		privateOutput.Verify(x => x.Send(It.Is<string>(s => s.Contains("hooded traveller") && s.Contains("East")), true, false), Times.Once);
		publicOutput.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
		sentry.Verify(x => x.ExecuteCommand("hide"), Times.Once);
		sentry.Setup(x => x.CanSee(traveller.Object, PerceiveIgnoreFlags.None)).Returns(false);
		Assert.IsFalse(ai.HandleEvent(EventType.CharacterLeaveRoomWitness, traveller.Object, origin.Object, exit.Object, sentry.Object));
		exit.SetupGet(x => x.OutboundDirection).Returns(CardinalDirection.North);
		Assert.IsFalse(ai.HandleEvent(EventType.CharacterLeaveRoomWitness, traveller.Object, origin.Object, exit.Object, sentry.Object));
	}
}
