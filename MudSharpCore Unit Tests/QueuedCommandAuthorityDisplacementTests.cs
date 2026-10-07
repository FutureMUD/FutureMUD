#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.NPC.AI;

namespace MudSharp_Unit_Tests;

// Receipt-unit proposal only. Native Cell/Room membership recovery, boarding and forced
// movement require the separate native scenarios; these mocks do not establish them.
public partial class QueuedCommandAuthorityTests
{
	[DataTestMethod]
	[DataRow("source", "expire")]
	[DataRow("source", "replacement-grant")]
	[DataRow("source", "body-replacement")]
	[DataRow("source", "combat-replacement")]
	[DataRow("source", "combat-null")]
	[DataRow("source", "retirement")]
	[DataRow("source", "policy")]
	[DataRow("gap", "expire")]
	[DataRow("gap", "replacement-grant")]
	[DataRow("gap", "body-replacement")]
	[DataRow("gap", "combat-replacement")]
	[DataRow("gap", "combat-null")]
	[DataRow("gap", "retirement")]
	[DataRow("gap", "policy")]
	[DataRow("arrival", "expire")]
	[DataRow("arrival", "replacement-grant")]
	[DataRow("arrival", "body-replacement")]
	[DataRow("arrival", "combat-replacement")]
	[DataRow("arrival", "combat-null")]
	[DataRow("arrival", "retirement")]
	[DataRow("arrival", "policy")]
	public void OrderedDisplacement_AdversarialAuthorityChangesRefuseContinuation(string stage, string change)
	{
		using var d = new DisplacementFixture();
		d.Reach(stage);
		switch (change)
		{
			case "expire": d.F.Grant = null; break;
			case "replacement-grant": d.F.Grant = d.F.Grant! with { Id = Guid.NewGuid() }; break;
			case "body-replacement":
				var replacementBody = new Mock<IBody>();
				replacementBody.SetupGet(x => x.Actor).Returns(d.F.Actor.Object);
				d.F.Actor.SetupGet(x => x.Body).Returns(replacementBody.Object);
				break;
			case "combat-replacement":
				var replacementCombat = new Mock<ICombat>();
				replacementCombat.SetupGet(x => x.Combatants).Returns([d.F.Actor.Object]);
				d.F.Actor.SetupGet(x => x.Combat).Returns(replacementCombat.Object);
				break;
			case "combat-null": d.F.Actor.SetupGet(x => x.Combat).Returns((ICombat?)null); break;
			case "retirement": d.F.ActorInstances.Remove((ICharacterInstance)d.F.Actor.Object); break;
			case "policy": d.F.Allowed = false; break;
		}

		Assert.IsFalse(d.Receipt.Continue(), $"{stage}/{change} must refuse.");
		Assert.IsFalse(d.Receipt.Complete());
		d.Receipt.Dispose();
		Assert.IsFalse(CommandExecutionScope.TryContinue(d.F.Actor.Object), "Refusal stays sticky after the membership receipt ends.");
	}

	[TestMethod]
	public void OrderedDisplacement_ExactSelfCommanderCanCrossMembershipGap()
	{
		using var d = new DisplacementFixture(true);
		d.Reach("gap");
		Assert.IsTrue(d.Receipt.Continue());
		Assert.IsFalse(CommandExecutionAuthority.MayExecute(d.Move, d.F.Actor.Object));
		d.Arrive();
		Assert.IsTrue(d.Receipt.Complete());
	}

	[TestMethod]
	public void OrderedDisplacement_ExactMembershipGapDoesNotAuthorizeOrdinaryContinuation()
	{
		using var d = new DisplacementFixture();
		d.Reach("gap");
		Assert.IsTrue(d.Receipt.Continue(), "The exact native continuation may cross its membership gap.");
		Assert.IsFalse(CommandExecutionAuthority.MayExecute(d.Move, d.F.Actor.Object), "Ordinary authority must still require cell membership.");
		d.Arrive();
		Assert.IsTrue(d.Receipt.Complete());
		d.Receipt.Dispose();
		Assert.IsTrue(CommandExecutionScope.TryContinue(d.F.Actor.Object));
	}

	[TestMethod]
	public void OrderedDisplacement_DestinationMembershipDuringMoveToCallbackRemainsValid()
	{
		using var d = new DisplacementFixture();
		d.Reach("gap");
		Assert.IsTrue(d.Receipt.BeginEnter(d.F.Actor.Object, d.Destination.Object));
		d.DestinationCharacters.Add(d.F.Actor.Object);
		d.Location = d.Destination.Object;
		d.F.Actor.Object.RoomLayer = RoomLayer.InAir;
		Assert.IsTrue(d.Receipt.Continue(), "MoveTo callbacks observe destination membership before AfterMoveTo records Arrived.");
		Assert.IsTrue(d.Receipt.AfterMoveTo(d.F.Actor.Object));
		Assert.IsTrue(d.Receipt.Complete());
	}

	[TestMethod]
	public void OrderedDisplacement_WrongDestinationAndRepeatedEntryCannotConsumeReceipt()
	{
		using var d = new DisplacementFixture();
		d.Reach("gap");
		Assert.IsFalse(d.Receipt.BeginEnter(d.F.Actor.Object, Mock.Of<IRoom>()));
		Assert.IsTrue(d.Receipt.BeginEnter(d.F.Actor.Object, d.Destination.Object));
		Assert.IsFalse(d.Receipt.BeginEnter(d.F.Actor.Object, d.Destination.Object));
		d.DestinationCharacters.Add(d.F.Actor.Object);
		d.Location = d.Destination.Object;
		d.F.Actor.Object.RoomLayer = RoomLayer.InAir;
		Assert.IsTrue(d.Receipt.AfterMoveTo(d.F.Actor.Object));
		Assert.IsTrue(d.Receipt.Complete());
		d.Receipt.Dispose();
		Assert.IsFalse(d.Receipt.Continue());
	}

	[TestMethod]
	public void OrderedDisplacement_ReentrantTravelCannotReuseCurrentReceipt()
	{
		using var d = new DisplacementFixture();
		d.Reach("gap");
		Assert.IsNull(CommandExecutionScope.BeginDisplacement(d.F.Actor.Object, d.Destination.Object, RoomLayer.InAir));
		Assert.IsFalse(d.Receipt.Continue());
	}

	[TestMethod]
	public void OrderedDisplacement_IndependentFrameCannotUseReceiptAndDoesNotInheritExpiredGrant()
	{
		using var d = new DisplacementFixture();
		d.Reach("gap");
		using (CommandExecutionScope.EnterIndependent())
		{
			Assert.IsFalse(d.Receipt.Continue(), "A foreign execution frame cannot use this native capability.");
			d.F.Grant = null;
			Assert.IsTrue(CommandExecutionScope.TryContinue(d.F.Actor.Object));
			Assert.IsNull(CommandExecutionScope.BeginDisplacement(d.F.Actor.Object, d.Destination.Object, RoomLayer.InAir));
		}
		Assert.IsFalse(d.Receipt.Continue(), "The ordered frame still refuses its expired grant.");
	}

	private sealed class DisplacementFixture : IDisposable
	{
		internal Fixture F { get; } = new();
		internal Mock<IRoom> Destination { get; } = new();
		internal List<ICharacter> DestinationCharacters { get; } = [];
		internal IRoom Location { get; set; }
		internal ICombatMove Move { get; }
		internal NativeDisplacementReceipt Receipt { get; }
		private readonly IDisposable _execution;

		internal DisplacementFixture(bool self = false)
		{
			Location = F.Actor.Object.Location;
			F.Actor.SetupGet(x => x.Location).Returns(() => Location);
			F.Actor.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
			Destination.SetupGet(x => x.Characters).Returns(DestinationCharacters);
			if (self)
			{
				F.Owned = false;
				Move = new MudSharp.Combat.Moves.TooExhaustedMove { Assailant = F.Actor.Object };
				CommandExecutionAuthority.Prepare(F.Actor.Object, F.Actor.Object, "advance", () => true).Bind(Move);
			}
			else { F.Order(); Move = F.Queued!.GetMove(F.Actor.Object)!; }
			Assert.IsNotNull(Move);
			_execution = CommandExecutionScope.EnterMove(Move);
			Receipt = CommandExecutionScope.BeginDisplacement(F.Actor.Object, Destination.Object, RoomLayer.InAir)!;
			Assert.IsNotNull(Receipt);
		}

		internal void Reach(string stage)
		{
			if (stage == "source") return;
			Assert.IsTrue(Receipt.BeginLeave(F.Actor.Object));
			F.RoomCharacters.RemoveAll(x => ReferenceEquals(x, F.Actor.Object));
			Assert.IsFalse(F.RoomCharacters.Exists(x => ReferenceEquals(x, F.Actor.Object)), "The fixture must create a real membership gap.");
			Assert.IsTrue(Receipt.Continue());
			if (stage == "arrival") Arrive();
		}

		internal void Arrive()
		{
			Assert.IsTrue(Receipt.BeginEnter(F.Actor.Object, Destination.Object));
			DestinationCharacters.Add(F.Actor.Object);
			Location = Destination.Object;
			F.Actor.Object.RoomLayer = RoomLayer.InAir;
			Assert.IsTrue(Receipt.AfterMoveTo(F.Actor.Object));
		}

		public void Dispose()
		{
			Receipt.Dispose();
			_execution.Dispose();
		}
	}
}
