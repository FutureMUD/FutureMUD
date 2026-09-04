#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.Movement;
using MudSharp.NPC.AI;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PathingAICommonOptionLoadTests
{
	private static IEnumerable<object[]> DerivedTypes =>
	[
		[typeof(PathToLocationAI)],
		[typeof(AggressivePatherAI)],
		[typeof(SemiAggressiveAI)],
		[typeof(TrackingAggressorAI)]
	];

	[TestMethod]
	[DynamicData(nameof(DerivedTypes))]
	public void ConstructorLoad_AllDerivedTypesRestoreCommonOptionsAndSpecialisedFieldsExactlyOnce(Type aiType)
	{
		var (ai, progs) = Load(aiType, includeCommonOptions: true);

		Assert.IsTrue(ai.OpenDoors);
		Assert.IsFalse(ai.UseKeys);
		Assert.IsTrue(ai.SmashLockedDoors);
		Assert.IsTrue(ai.CloseDoorsBehind);
		Assert.IsFalse(ai.UseDoorguards);
		Assert.IsTrue(ai.MoveEvenIfObstructionInWay);
		Assert.AreEqual(77L, ai.DoorSmashDelayProg?.Id);

		var saved = SavedDefinition(ai);
		Assert.AreEqual("true", saved.Element("OpenDoors")?.Value.ToLowerInvariant());
		Assert.AreEqual("false", saved.Element("UseKeys")?.Value.ToLowerInvariant());
		Assert.AreEqual("true", saved.Element("SmashLockedDoors")?.Value.ToLowerInvariant());
		Assert.AreEqual("true", saved.Element("CloseDoorsBehind")?.Value.ToLowerInvariant());
		Assert.AreEqual("false", saved.Element("UseDoorguards")?.Value.ToLowerInvariant());
		Assert.AreEqual("true", saved.Element("MoveEvenIfObstructionInWay")?.Value.ToLowerInvariant());
		Assert.AreEqual("77", saved.Element("DoorSmashDelayProg")?.Value);

		foreach (var id in new long[] { 10, 11, 12, 13, 14, 77 }.Concat(SpecialisedProgIds(aiType)))
		{
			progs.Verify(x => x.Get(id), Times.Once);
		}

		AssertSpecialisedFields(aiType, saved);
	}

	[TestMethod]
	[DynamicData(nameof(DerivedTypes))]
	public void ConstructorLoad_MissingLegacyCommonOptionsRemainFalse(Type aiType)
	{
		var (ai, _) = Load(aiType, includeCommonOptions: false);

		Assert.IsFalse(ai.OpenDoors);
		Assert.IsFalse(ai.UseKeys);
		Assert.IsFalse(ai.SmashLockedDoors);
		Assert.IsFalse(ai.CloseDoorsBehind);
		Assert.IsFalse(ai.UseDoorguards);
		Assert.IsFalse(ai.MoveEvenIfObstructionInWay);
		Assert.AreEqual(77L, ai.DoorSmashDelayProg?.Id);
	}

	[TestMethod]
	public void LoadedSmashLockedDoors_FirstRouteSuitabilityConsumerIncludesClosedDoor()
	{
		var (ai, _) = Load(typeof(PathToLocationAI), includeCommonOptions: true, openDoors: false);
		var door = new Mock<IDoor>();
		door.SetupGet(x => x.IsOpen).Returns(false);
		var exit = new Mock<IExit>();
		exit.SetupGet(x => x.Door).Returns(door.Object);
		var cellExit = new Mock<ICellExit>();
		cellExit.SetupGet(x => x.Exit).Returns(exit.Object);
		var character = new Mock<ICharacter>();
		character.Setup(x => x.CanCross(cellExit.Object)).Returns((false, null!));
		character.Setup(x => x.CanMove(cellExit.Object, It.IsAny<CanMoveFlags>()))
			.Returns(new CanMoveResponse { Result = false, ErrorMessage = "blocked" });
		var method = typeof(PathingAIBase).GetMethod("GetSuitabilityFunction",
			BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.IsNotNull(method);
		var suitability = (Func<ICellExit, bool>)method.Invoke(ai, new object[] { character.Object, true })!;

		Assert.IsTrue(suitability(cellExit.Object));
	}

	private static (PathingAIWithProgTargetsBase Ai, Mock<IUneditableAll<IFutureProg>> Progs) Load(
		Type aiType,
		bool includeCommonOptions,
		bool openDoors = true)
	{
		var progCache = Enumerable.Range(1, 100).ToDictionary(
			x => (long)x,
			x =>
			{
				var prog = new Mock<IFutureProg>();
				prog.SetupGet(y => y.Id).Returns(x);
				prog.SetupGet(y => y.Name).Returns($"Prog {x}");
				return prog.Object;
			});
		var progs = new Mock<IUneditableAll<IFutureProg>>();
		progs.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => progCache[id]);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(progs.Object);
		var model = new ArtificialIntelligence
		{
			Id = 1,
			Name = aiType.Name,
			Type = aiType.Name,
			Definition = BuildDefinition(aiType, includeCommonOptions, openDoors).ToString()
		};
		var constructor = aiType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
			null, [typeof(ArtificialIntelligence), typeof(IFuturemud)], null);
		Assert.IsNotNull(constructor);
		return ((PathingAIWithProgTargetsBase)constructor.Invoke([model, gameworld.Object]), progs);
	}

	private static XElement BuildDefinition(Type aiType, bool includeCommonOptions, bool openDoors)
	{
		var root = new XElement("Definition",
			new XElement("PathingEnabledProg", 10),
			new XElement("OnStartToPathProg", 11),
			new XElement("TargetLocationProg", 12),
			new XElement("FallbackLocationProg", 13),
			new XElement("WayPointsProg", 14),
			new XElement("DoorSmashDelayProg", 77));
		if (includeCommonOptions)
		{
			root.Add(
				new XElement("OpenDoors", openDoors),
				new XElement("UseKeys", false),
				new XElement("SmashLockedDoors", true),
				new XElement("CloseDoorsBehind", true),
				new XElement("UseDoorguards", false),
				new XElement("MoveEvenIfObstructionInWay", true));
		}

		if (aiType == typeof(AggressivePatherAI) || aiType == typeof(TrackingAggressorAI))
		{
			root.Add(
				new XElement("WillAttackProg", 20),
				new XElement("EngageDelayDiceExpression", "123+1d4"),
				new XElement("EngageEmote", "engages exactly once"));
			if (aiType == typeof(TrackingAggressorAI))
			{
				root.Add(new XElement("MaximumRange", 7));
			}
		}
		else if (aiType == typeof(SemiAggressiveAI))
		{
			root.Add(
				new XElement("WillAttackProg", 20),
				new XElement("WillPostureProg", 21),
				new XElement("WillFleeProg", 22),
				new XElement("WillAttackPostureEscalationProg", 23),
				new XElement("PostureEmoteProg", 24),
				new XElement("AttackEmoteProg", 25),
				new XElement("FleeEmoteProg", 26),
				new XElement("FleeLocationsProg", 27),
				new XElement("PostureTimeSpanDiceExpression", "17+1d4"),
				new XElement("ThreatPerEscalationTick", 0.6),
				new XElement("ThreatPerInventoryChange", 0.07),
				new XElement("ThreatPerHostilePreCombatAction", 1.2),
				new XElement("ThreatEscalationPerAdditionalTarget", 0.4));
		}

		return root;
	}

	private static IEnumerable<long> SpecialisedProgIds(Type aiType)
	{
		if (aiType == typeof(AggressivePatherAI) || aiType == typeof(TrackingAggressorAI))
		{
			return [20];
		}

		return aiType == typeof(SemiAggressiveAI)
			? [20, 21, 22, 23, 24, 25, 26, 27]
			: [];
	}

	private static XElement SavedDefinition(PathingAIWithProgTargetsBase ai)
	{
		var method = typeof(ArtificialIntelligenceBase).GetMethod("DefinitionForSave",
			BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.IsNotNull(method);
		return XElement.Parse((string)method.Invoke(ai, null)!);
	}

	private static void AssertSpecialisedFields(Type aiType, XElement saved)
	{
		if (aiType == typeof(AggressivePatherAI) || aiType == typeof(TrackingAggressorAI))
		{
			Assert.AreEqual("20", saved.Element("WillAttackProg")?.Value);
			Assert.AreEqual("123+1d4", saved.Element("EngageDelayDiceExpression")?.Value);
			Assert.AreEqual("engages exactly once", saved.Element("EngageEmote")?.Value);
		}

		if (aiType == typeof(TrackingAggressorAI))
		{
			Assert.AreEqual("7", saved.Element("MaximumRange")?.Value);
		}

		if (aiType == typeof(SemiAggressiveAI))
		{
			Assert.AreEqual("20", saved.Element("WillAttackProg")?.Value);
			Assert.AreEqual("27", saved.Element("FleeLocationsProg")?.Value);
			Assert.AreEqual("17+1d4", saved.Element("PostureTimeSpanDiceExpression")?.Value);
		}
	}
}
