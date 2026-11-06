#nullable enable

using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AggressorAcquisitionRangeTests
{
	private sealed class RecordingAggressor(MudSharp.Models.ArtificialIntelligence definition, IFuturemud world) : AggressorAI(definition, world)
	{
		public List<ICharacter> Considered { get; } = [];
		public override bool CheckForAttack(ICharacter aggressor, ICharacter target) { Considered.Add(target); return false; }
	}

	[TestMethod]
	public void AcquisitionRange_ControlsUnarmedScanAndRoundTripsBuilderChanges()
	{
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var prog = Mock.Of<IFutureProg>(x => x.Id == 1); world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg> { prog });
		var row = new MudSharp.Models.ArtificialIntelligence { Id = 4, Name = "test", Type = "Aggressor", Definition = "<Definition><WillAttackProg>1</WillAttackProg><EngageDelayDiceExpression>1</EngageDelayDiceExpression></Definition>" };
		var ai = new RecordingAggressor(row, world.Object);
		Assert.IsNull(ai.AcquisitionRange, "Old definitions preserve weapon-derived scanning.");
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake); actor.SetupGet(x => x.Effects).Returns(new List<IEffect>());
		var body = new Mock<IBody>(); body.SetupGet(x => x.WieldedItems).Returns(new List<IGameItem>()); actor.SetupGet(x => x.Body).Returns(body.Object);
		var rooms = new[] { new Mock<IRoom>(), new Mock<IRoom>(), new Mock<IRoom>() };
		var targets = new[] { Mock.Of<ICharacter>(), Mock.Of<ICharacter>() };
		for (var index = 0; index < rooms.Length; index++)
		{
			rooms[index].SetupGet(x => x.Id).Returns(index + 1); rooms[index].SetupGet(x => x.Location).Returns(rooms[index].Object);
			rooms[index].SetupGet(x => x.Characters).Returns(index == 0 ? [] : new[] { targets[index - 1] });
			var exits = new List<IRoomExit>();
			if (index < 2)
			{
				var exit = new Mock<IRoomExit>(); exit.SetupGet(x => x.Destination).Returns(rooms[index + 1].Object);
				exit.SetupGet(x => x.OutboundDirection).Returns(CardinalDirection.East); exit.SetupGet(x => x.Exit).Returns(Mock.Of<IExit>()); exits.Add(exit.Object);
			}
			rooms[index].Setup(x => x.ExitsFor(It.IsAny<IPerceiver>(), It.IsAny<bool>())).Returns(exits);
		}
		actor.SetupGet(x => x.Location).Returns(rooms[0].Object);
		Assert.IsTrue(ai.BuildingCommand(actor.Object, new StringStack("range 1")));
		ai.HandleEvent(EventType.TenSecondTick, actor.Object);
		CollectionAssert.AreEqual(new[] { targets[0] }, ai.Considered);
		ai.Considered.Clear(); Assert.IsTrue(ai.BuildingCommand(actor.Object, new StringStack("range 2")));
		ai.HandleEvent(EventType.TenSecondTick, actor.Object); CollectionAssert.AreEqual(targets, ai.Considered);
		var xml = (string)typeof(AggressorAI).GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(ai, null)!;
		row.Definition = xml; Assert.AreEqual(2u, new AggressorAI(row, world.Object).AcquisitionRange);
		ai.Considered.Clear(); ai.BuildingCommand(actor.Object, new StringStack("range 0")); ai.HandleEvent(EventType.TenSecondTick, actor.Object);
		Assert.AreEqual(0, ai.Considered.Count);
		Assert.IsFalse(ai.BuildingCommand(actor.Object, new StringStack("range -1")));
		Assert.IsTrue(ai.BuildingCommand(actor.Object, new StringStack("range weapon"))); Assert.IsNull(ai.AcquisitionRange);
	}
}
