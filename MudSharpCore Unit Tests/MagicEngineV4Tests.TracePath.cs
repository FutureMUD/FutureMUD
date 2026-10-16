#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Magic.SpellTriggers;
using MudSharp.Movement;

namespace MudSharp_Unit_Tests;

public partial class MagicEngineV4Tests
{
	[DataTestMethod]
	[DataRow(1)] [DataRow(2)] [DataRow(3)] [DataRow(4)] [DataRow(5)] [DataRow(6)] [DataRow(7)]
	public void TracePath_AllGradesRevealOnlyThePermittedBoundWithoutMovement(int grade)
	{
		var f = new TraceFixture(10 * grade);
		var result = f.Cast(grade);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(0, f.Pair.Moves);
		Assert.IsTrue(f.Pair.F.Messages.Any(x => x.StartsWith("The permitted path leads east", StringComparison.Ordinal)));
		Assert.IsTrue(f.Pair.F.Balances[f.Pair.F.Resources[1]] < 100.0);
		var beyond = new TraceFixture(10 * grade + 1);
		Assert.AreEqual(MagicCastingStatus.Refused, beyond.Cast(grade).Status);
		Assert.AreEqual(100.0, beyond.Pair.F.Balances[beyond.Pair.F.Resources[1]]);
	}

	[DataTestMethod]
	[DataRow("closed")] [DataRow("hidden")] [DataRow("impassable")] [DataRow("unverified")] [DataRow("guard")]
	[DataRow("concealed")] [DataRow("bodydub")] [DataRow("ambiguous")] [DataRow("offline")]
	public void TracePath_UnsafeRouteOrUnprovenIdentityRefusesBeforePayment(string reason)
	{
		var f = new TraceFixture(2);
		switch (reason)
		{
			case "closed": f.Doors[0].SetupGet(x => x.IsOpen).Returns(false); break;
			case "hidden": f.Pair.F.Actor.Setup(x => x.CanSee(f.Edges[0].Exit, It.IsAny<PerceiveIgnoreFlags>())).Returns(false); break;
			case "impassable": f.Pair.F.Actor.Setup(x => x.CanMoveForPathPlanning(f.Edges[0], It.IsAny<CanMoveFlags>())).Returns(new CanMoveResponse { Result = false, ErrorMessage = "Blocked" }); break;
			case "guard": f.Pair.F.Actor.Setup(x => x.CanCross(f.Edges[0])).Returns((false, (MudSharp.PerceptionEngine.IEmoteOutput)null!)); break;
			case "unverified": f.Dub.SetupGet(x => x.HasVerifiedIdentity).Returns(false); break;
			case "concealed": f.Pair.Target.Setup(x => x.IdentityIsObscuredTo(f.Pair.F.Actor.Object)).Returns(true); break;
			case "bodydub": f.Dub.SetupGet(x => x.TargetType).Returns("Body"); break;
			case "ambiguous": f.Dubs.Add(f.Dub.Object); break;
			case "offline": f.Pair.F.World.SetupGet(x => x.Actors).Returns(MagicCastingFixture.Collection(() => new[] { f.Pair.F.Actor.Object })); break;
		}
		Assert.AreEqual(MagicCastingStatus.Refused, f.Cast(1).Status);
		Assert.AreEqual(100.0, f.Pair.F.Balances[f.Pair.F.Resources[1]]); Assert.AreEqual(0, f.Pair.Moves);
		Assert.IsFalse(f.Pair.F.Messages.Any(x => x.StartsWith("The permitted path leads", StringComparison.Ordinal)));
	}

	[DataTestMethod]
	[DataRow("door")] [DataRow("dub")] [DataRow("position")] [DataRow("topology")]
	public void TracePath_FinalAdmissionRejectsFrozenInputDrift(string change)
	{
		var f = new TraceFixture(2);
		f.Pair.F.Checkpoint = stage =>
		{
			if (stage != "BeforePayment") return;
			if (change == "door") f.Doors[0].SetupGet(x => x.IsOpen).Returns(false);
			if (change == "dub") f.Dub.SetupGet(x => x.HasVerifiedIdentity).Returns(false);
			if (change == "position") f.Pair.TargetLocation = f.Pair.Source.Object;
			if (change == "topology") Mock.Get(f.Pair.Source.Object.CurrentOverlay).SetupGet(x => x.ExitIDs).Returns([]);
		};
		Assert.AreEqual(MagicCastingStatus.Refused, f.Cast(1).Status);
		Assert.AreEqual(100.0, f.Pair.F.Balances[f.Pair.F.Resources[1]]);
	}

	[TestMethod]
	public void TracePath_LaterEdgeCallbackCannotNarrowAnEarlierEdgeBeforePayment()
	{
		var f = new TraceFixture(2); var armed = false;
		f.Pair.F.Actor.Setup(x => x.CanCross(f.Edges[1])).Returns(() =>
		{
			if (armed) Mock.Get(f.Edges[0].Exit).SetupGet(x => x.MaximumSizeToEnter).Returns(SizeCategory.Miniscule);
			return (true, (MudSharp.PerceptionEngine.IEmoteOutput)null!);
		});
		f.Pair.F.Checkpoint = stage => { if (stage == "BeforePayment") armed = true; };
		Assert.AreEqual(MagicCastingStatus.Refused, f.Cast(1).Status);
		Assert.AreEqual(100.0, f.Pair.F.Balances[f.Pair.F.Resources[1]]);
	}

	[TestMethod]
	public void TracePath_NativeOriginHiddenExitNeverQualifiesAsPermittedRoute()
	{
		var f = new TraceFixture(1);
		var origin = TestObjectFactory.CreateUninitialized<MudSharp.Construction.Room>();
		typeof(PerceivedItem).GetProperty(nameof(origin.Gameworld))!.SetValue(origin, f.Pair.F.World.Object);
		typeof(PerceivedItem).GetProperty(nameof(origin.EffectHandler))!.SetValue(origin, new EffectHandler(origin));
		origin.EffectHandler.AddEffect(new ExitHidden(origin, f.Edges[0].Exit));
		Mock.Get(f.Edges[0]).SetupGet(x => x.Origin).Returns(origin);
		f.Pair.F.Actor.Setup(x => x.CanSee(It.IsAny<IRoom>(), f.Edges[0], It.IsAny<PerceiveIgnoreFlags>()))
			.Returns<IRoom, IRoomExit, PerceiveIgnoreFlags>((room, edge, flags) => room.IsExitVisible(f.Pair.F.Actor.Object, edge, PerceptionTypes.AllVisual, flags));
		Assert.IsFalse(SpellPermittedPath.Admitted(f.Pair.F.Actor.Object, f.Edges[0]));
	}

	[TestMethod]
	public void RecognisedCharacter_ActiveWornIdentityObscurerRefusesEvenWithLegacyFalseHook()
	{
		var f = new TraceFixture(1); var item = new Mock<IGameItem>(); var mask = new Mock<IObscureIdentity>();
		mask.SetupGet(x => x.CurrentlyApplies).Returns(true); item.SetupGet(x => x.Components).Returns([mask.Object]);
		f.Pair.TargetBody.SetupGet(x => x.WornItems).Returns([item.Object]);
		Assert.IsNull(CastingTriggerRecognisedCharacter.Resolve(f.Pair.F.Actor.Object, "traveller"));
		Assert.IsNull(CastingTriggerRecognisedCharacter.Resolve(f.Pair.F.Actor.Object, "#101"));
		Assert.IsNull(CastingTriggerRecognisedCharacter.Resolve(f.Pair.F.Actor.Object, "101"));
		Assert.AreEqual(MagicCastingStatus.Refused, f.Cast(1).Status);
	}

	[DataTestMethod]
	[DataRow(null, false)] [DataRow(true, false)] [DataRow(false, true)]
	public void RecognisedCharacter_LoadedDubPreservesUnknownConcealedAndVerifiedProvenance(bool? concealed, bool verified)
	{
		var f = new TraceFixture(1);
		var row = new MudSharp.Models.Dub { Id = 77, TargetId = 101, TargetType = "Character", Keywords = "traveller", WasIdentityConcealed = concealed };
		var dub = new Dub(row, f.Pair.F.Actor.Object, f.Pair.F.World.Object);
		Assert.AreEqual(verified, dub.HasVerifiedIdentity);
		Assert.AreEqual(concealed ?? false, dub.WasIdentityConcealed);
		dub.WasIdentityConcealed = false; Assert.IsTrue(dub.HasVerifiedIdentity);
	}

	private sealed class TraceFixture
	{
		public ExchangeFixture Pair { get; } = new();
		public Mock<IDub> Dub { get; } = new();
		public List<IDub> Dubs { get; } = [];
		public List<IRoomExit> Edges { get; } = [];
		public List<Mock<IDoor>> Doors { get; } = [];
		public TraceFixture(int distance)
		{
			var f = Pair.F;
			Pair.Target.SetupGet(x => x.IsPrimaryInstance).Returns(true);
			Pair.Target.SetupGet(x => x.PositionState).Returns(MudSharp.Body.Position.PositionStates.PositionStanding.Instance);
			Pair.TargetBody.SetupGet(x => x.WornItems).Returns([]);
			Dub.SetupGet(x => x.Id).Returns(77); Dub.SetupGet(x => x.Owner).Returns(f.Actor.Object);
			Dub.SetupGet(x => x.TargetId).Returns(101); Dub.SetupGet(x => x.TargetType).Returns("Character");
			Dub.SetupGet(x => x.HasVerifiedIdentity).Returns(true); Dub.SetupGet(x => x.Keywords).Returns(["traveller"]);
			Dubs.Add(Dub.Object); f.Actor.SetupGet(x => x.Dubs).Returns(Dubs);
			f.Actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			f.Actor.Setup(x => x.CanSee(It.IsAny<IRoom>(), It.IsAny<IRoomExit>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			f.Actor.Setup(x => x.CanMoveForPathPlanning(It.IsAny<IRoomExit>(), It.IsAny<CanMoveFlags>())).Returns(CanMoveResponse.True);
			f.Actor.Setup(x => x.CanCross(It.IsAny<IRoomExit>())).Returns((true, (MudSharp.PerceptionEngine.IEmoteOutput)null!));
			var rooms = new List<IRoom> { Pair.Source.Object };
			for (var i = 1; i < distance; i++)
			{
				var room = new Mock<IRoom> { DefaultValue = DefaultValue.Mock }; room.SetupGet(x => x.Id).Returns(1000 + i);
				room.SetupGet(x => x.Gameworld).Returns(f.World.Object); room.SetupGet(x => x.OwningZone).Returns(Pair.Source.Object.OwningZone);
				rooms.Add(room.Object);
			}
			rooms.Add(Pair.Destination.Object);
			f.World.SetupGet(x => x.Rooms).Returns(MagicCastingFixture.Collection(() => rooms));
			for (var i = 0; i < rooms.Count - 1; i++)
			{
				var edge = new Mock<IRoomExit>(); var exit = new Mock<IExit>(); var door = new Mock<IDoor>();
				Doors.Add(door); door.SetupGet(x => x.IsOpen).Returns(true);
				exit.SetupGet(x => x.Id).Returns(2000 + i); exit.SetupGet(x => x.Door).Returns(door.Object);
				edge.SetupGet(x => x.Exit).Returns(exit.Object); edge.SetupGet(x => x.Origin).Returns(rooms[i]); edge.SetupGet(x => x.Destination).Returns(rooms[i + 1]);
				edge.SetupGet(x => x.OutboundDirectionDescription).Returns("east");
				Mock.Get(rooms[i]).Setup(x => x.ExitsFor(f.Actor.Object, false)).Returns([edge.Object]);
				Mock.Get(rooms[i].CurrentOverlay).SetupGet(x => x.ExitIDs).Returns([exit.Object.Id]);
				Edges.Add(edge.Object);
			}
			var stock = ArmageddonTracePathSpellContent.Create().Content; var row = stock.SpellRow(1, 1, 1);
			row.Id = 1; row.EffectDurationExpressionId = 2; row.Definition = stock.BuildDefinition(10, 1, 0).ToString();
			f.Spells.Clear(); f.Spells.Add(new MagicSpell(row, f.World.Object) { EffectDurationExpression = new MudSharp.Body.Traits.TraitExpression("0", f.World.Object) });
		}
		public MagicCastingResult Cast(int grade) => Pair.F.Service.Cast(new(Pair.F.Actor.Object, Pair.F.Earth.Id, 1, grade, false, "traveller", OriginId: Guid.NewGuid()));
	}
}
