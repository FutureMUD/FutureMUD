#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using MudSharp.Magic.Powers;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class MagicEngineV4Tests
{
	private sealed class ContactFixture
	{
		public Mock<IFuturemud> World = CreateGameworld();
		public Mock<ICharacter> Actor;
		public Mock<ICharacter> Target;
		public ConnectMindEffect Contact;
		public List<IEffect> Outgoing = [];
		public List<IEffect> Incoming = [];
		public ContactFixture()
		{
			World.SetupGet(x => x.HeartbeatManager).Returns(Mock.Of<IHeartbeatManager>());
			var room = CreateRoom(1, World.Object, CreateZone(1).Object);
			Actor = CreateCharacter(1, World.Object, room.Object);
			Target = CreateCharacter(2, World.Object, room.Object);
			Actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
			Target.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
			Actor.SetupGet(x => x.Effects).Returns(() => Outgoing.ToArray());
			Target.SetupGet(x => x.Effects).Returns(() => Incoming.ToArray());
			Target.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(Incoming.Add);
			Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), true)).Callback<IEffect, bool>((effect, _) => { effect.RemovalEffect(); Outgoing.Remove(effect); });
			Target.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), true)).Callback<IEffect, bool>((effect, _) => { effect.RemovalEffect(); Incoming.Remove(effect); });
			var model = CreateConnectMindModel(0);
			var xml = XElement.Parse(model.Definition);
			xml.SetElementValue("PowerDistance", (int)MagicPowerDistance.SameLocationOnly);
			model.Definition = xml.ToString();
			Contact = new ConnectMindEffect(Actor.Object, Target.Object, (ConnectMindPower)MagicPowerFactory.LoadPower(model, World.Object));
			Outgoing.Add(Contact);
		}
	}

	[TestMethod]
	[DataRow("quit")]
	[DataRow("death")]
	[DataRow("delete")]
	[DataRow("move")]
	[DataRow("remove")]
	public void Contact_TargetLifecycle_RemovesBothEnds(string action)
	{
		var f = new ContactFixture();
		f.Contact.Login(); // Repeated login must not duplicate event subscriptions.
		switch (action)
		{
			case "quit": f.Target.Raise(x => x.OnQuit += null, f.Target.Object); break;
			case "death": f.Target.Raise(x => x.OnDeath += null, f.Target.Object); break;
			case "delete": f.Target.Raise(x => x.OnDeleted += null, f.Target.Object); break;
			case "move":
				f.Target.SetupGet(x => x.Location).Returns(CreateRoom(2, f.World.Object, CreateZone(1).Object).Object);
				f.Target.Raise(x => x.OnLocationChanged += null, f.Target.Object, null);
				break;
			default: f.Target.Object.RemoveEffect(f.Contact.ChildEffect, true); break;
		}
		Assert.AreEqual(0, f.Outgoing.Count);
		Assert.AreEqual(0, f.Incoming.Count);
		f.Actor.Invocations.Clear();
		f.Target.Raise(x => x.OnQuit += null, f.Target.Object);
		f.Actor.Verify(x => x.RemoveEffect(It.IsAny<IEffect>(), true), Times.Never);
	}

	[TestMethod]
	[DataRow(Outcome.Pass, false)]
	[DataRow(Outcome.MajorFail, true)]
	public void Contact_ConcentrationUsesConfiguredTrait_AndRetainsFailurePolicy(Outcome outcome, bool removed)
	{
		var f = new ContactFixture();
		var trait = CreateTrait().Object;
		var capability = new Mock<IMagicCapability>();
		capability.SetupGet(x => x.School).Returns(f.Contact.School);
		capability.SetupGet(x => x.ConcentrationTrait).Returns(trait);
		capability.Setup(x => x.ConcentrationAbility(f.Actor.Object)).Returns(2);
		capability.Setup(x => x.GetConcentrationDifficulty(f.Actor.Object, .5, 1)).Returns(Difficulty.Normal);
		f.Actor.SetupGet(x => x.Capabilities).Returns([capability.Object]);
		var check = new Mock<ICheck>();
		check.Setup(x => x.Check(f.Actor.Object, Difficulty.Normal, trait, null, 0, TraitUseType.Practical, It.IsAny<(string Parameter, object value)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.MagicConcentrationOnWounded, outcome));
		f.World.Setup(x => x.GetCheck(CheckType.MagicConcentrationOnWounded)).Returns(check.Object);
		f.Contact.ChallengeConcentration(Difficulty.Normal);
		check.Verify(x => x.Check(f.Actor.Object, Difficulty.Normal, trait, null, 0, TraitUseType.Practical, It.IsAny<(string Parameter, object value)[]>()), Times.Once);
		Assert.AreEqual(removed ? 0 : 1, f.Outgoing.Count);
	}
}
