#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.NPC.AI;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NpcAiRegressionTests
{
	private static T CreatePrivateParameterless<T>() where T : class
	{
		var ctor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
		Assert.IsNotNull(ctor, $"Could not find a private parameterless constructor for {typeof(T).Name}.");
		return (T)ctor.Invoke(null);
	}

	[TestMethod]
	public void ArenaParticipantAI_GetOpponents_ReturnsOnlyOpposingSideParticipants()
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Id).Returns(1L);
		var ally = new Mock<ICharacter>();
		ally.SetupGet(x => x.Id).Returns(2L);
		var enemy = new Mock<ICharacter>();
		enemy.SetupGet(x => x.Id).Returns(3L);

		var eventMock = new Mock<IArenaEvent>();
		eventMock.SetupGet(x => x.Participants).Returns(new[]
		{
			MockParticipant(actor.Object, 0),
			MockParticipant(ally.Object, 0),
			MockParticipant(enemy.Object, 1)
		});

		var opponents = ArenaParticipantAI.GetOpponents(eventMock.Object, actor.Object).ToList();

		Assert.AreEqual(1, opponents.Count);
		Assert.AreSame(enemy.Object, opponents[0].Character);
	}

	[TestMethod]
	public void ArborealWandererAI_CellSupportsTreeLayers_DetectsTreeCapableTerrain()
	{
		var character = new Mock<ICharacter>();
		var treeTerrain = new Mock<ITerrain>();
		treeTerrain.SetupGet(x => x.TerrainLayers).Returns(new[] { RoomLayer.GroundLevel, RoomLayer.InTrees });
		var groundTerrain = new Mock<ITerrain>();
		groundTerrain.SetupGet(x => x.TerrainLayers).Returns(new[] { RoomLayer.GroundLevel });

		var treeCell = new Mock<ICell>();
		treeCell.Setup(x => x.Terrain(character.Object)).Returns(treeTerrain.Object);
		var groundCell = new Mock<ICell>();
		groundCell.Setup(x => x.Terrain(character.Object)).Returns(groundTerrain.Object);

		Assert.IsTrue(ArborealWandererAI.CellSupportsTreeLayers(character.Object, treeCell.Object));
		Assert.IsFalse(ArborealWandererAI.CellSupportsTreeLayers(character.Object, groundCell.Object));
	}

	[TestMethod]
	public void LairScavengerAI_GetScavengedItems_ReturnsOnlyProgApprovedHeldItems()
	{
		var item1 = new Mock<IGameItem>();
		var item2 = new Mock<IGameItem>();
		var body = new Mock<IBody>();
		body.SetupGet(x => x.HeldOrWieldedItems).Returns(new[] { item1.Object, item2.Object });
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Body).Returns(body.Object);

		var prog = new Mock<IFutureProg>();
		prog.Setup(x => x.ExecuteBool(false,
				It.Is<object[]>(vars => vars.Length == 2 &&
				                        ReferenceEquals(vars[0], character.Object) &&
				                        ReferenceEquals(vars[1], item1.Object))))
			.Returns(true);
		prog.Setup(x => x.ExecuteBool(false,
				It.Is<object[]>(vars => vars.Length == 2 &&
				                        ReferenceEquals(vars[0], character.Object) &&
				                        ReferenceEquals(vars[1], item2.Object))))
			.Returns(false);

		var items = LairScavengerAI.GetScavengedItems(character.Object, prog.Object).ToList();

		CollectionAssert.AreEqual(new[] { item1.Object }, items);
	}

	[TestMethod]
	public void DenBuilderAI_SelectAnchorItem_SkipsActiveCraftItemsAndUsesProgApproval()
	{
		var roomLayer = RoomLayer.GroundLevel;
		var activeCraftItem = new Mock<IGameItem>();
		activeCraftItem.Setup(x => x.GetItemType<IActiveCraftGameItemComponent>())
			.Returns((IActiveCraftGameItemComponent?)new Mock<IActiveCraftGameItemComponent>().Object);
		var anchorItem = new Mock<IGameItem>();
		anchorItem.Setup(x => x.GetItemType<IActiveCraftGameItemComponent>()).Returns((IActiveCraftGameItemComponent?)null);

		var location = new Mock<ICell>();
		location.Setup(x => x.LayerGameItems(roomLayer)).Returns(new[] { activeCraftItem.Object, anchorItem.Object });
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Location).Returns(location.Object);
		character.SetupGet(x => x.RoomLayer).Returns(roomLayer);

		var prog = new Mock<IFutureProg>();
		prog.Setup(x => x.ExecuteBool(
				It.Is<object[]>(vars => vars.Length == 2 &&
				                        ReferenceEquals(vars[0], character.Object) &&
				                        ReferenceEquals(vars[1], anchorItem.Object))))
			.Returns(true);

		var selected = DenBuilderAI.SelectAnchorItem(character.Object, prog.Object);

		Assert.AreSame(anchorItem.Object, selected);
	}

	[TestMethod]
	public void TerritorialWanderer_TryParseWanderChance_AcceptsPercentagesWithinRange()
	{
		Assert.IsTrue(TerritorialWanderer.TryParseWanderChance("25%", out var validValue));
		Assert.AreEqual(0.25, validValue, 0.0001);
		Assert.IsFalse(TerritorialWanderer.TryParseWanderChance("250%", out _));
	}

	[TestMethod]
	public void ScavengeAI_ValidationHelpers_DistinguishExpectedProgShapes()
	{
		var willProg = new Mock<IFutureProg>();
		willProg.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		willProg.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);

		var onProg = new Mock<IFutureProg>();
		onProg.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Void);
		onProg.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);

		Assert.IsTrue(ScavengeAI.IsValidWillScavengeProg(willProg.Object));
		Assert.IsFalse(ScavengeAI.IsValidOnScavengeProg(willProg.Object));
		Assert.IsTrue(ScavengeAI.IsValidOnScavengeProg(onProg.Object));
		Assert.IsFalse(ScavengeAI.IsValidWillScavengeProg(onProg.Object));
	}

	[TestMethod]
	public void AggressiveAiTypes_HandleCharacterEnterCellWitnessEvent()
	{
		var aggressivePather = CreatePrivateParameterless<AggressivePatherAI>();
		var trackingAggressor = CreatePrivateParameterless<TrackingAggressorAI>();

		Assert.IsTrue(aggressivePather.HandlesEvent(EventType.CharacterEnterCellWitness));
		Assert.IsTrue(trackingAggressor.HandlesEvent(EventType.CharacterEnterCellWitness));
	}

	private static IArenaParticipant MockParticipant(ICharacter character, int sideIndex)
	{
		var participant = new Mock<IArenaParticipant>();
		participant.SetupGet(x => x.Character).Returns(character);
		participant.SetupGet(x => x.CharacterId).Returns(character.Id);
		participant.SetupGet(x => x.SideIndex).Returns(sideIndex);
		return participant.Object;
	}
}
