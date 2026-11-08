#nullable enable

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TrackingPursuitOnlyTests
{
	private sealed class Tracker(MudSharp.Models.ArtificialIntelligence row, IFuturemud world) : TrackingAggressorAI(row, world);

	[TestMethod]
	[DataRow(false, true)]
	[DataRow(true, false)]
	public void PursuitOnlyChecksNativeCombatAdmissionWithoutChangingLegacyTracking(bool pursuitOnly, bool expected)
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var prog = new Mock<IFutureProg>(); prog.SetupGet(x => x.Id).Returns(1);
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(true);
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg> { prog.Object });
		var row = new MudSharp.Models.ArtificialIntelligence { Id = 1, Name = "pursuit", Type = "TrackingAggressor",
			Definition = $"<Definition><WillAttackProg>1</WillAttackProg><PathingEnabledProg>1</PathingEnabledProg><MaximumRange>9</MaximumRange><EngageDelayDiceExpression>0</EngageDelayDiceExpression><PursuitOnly>{pursuitOnly.ToString().ToLowerInvariant()}</PursuitOnly></Definition>" };
		var ai = new Tracker(row, world.Object);
		var actor = new Mock<ICharacter>(); var target = Mock.Of<ICharacter>();
		actor.Setup(x => x.CanSee(target, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		actor.Setup(x => x.CanEngage(target)).Returns(false);
		var predicate = (Func<IPerceivable, bool>)typeof(TrackingAggressorAI)
			.GetMethod("GetTargetFunction", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(ai, [actor.Object])!;
		Assert.AreEqual(expected, predicate(target));
		Assert.AreEqual(!pursuitOnly, ai.CountsAsAggressive);
		if (pursuitOnly) Assert.IsFalse(ai.CheckForAttack(actor.Object, target));
		if (pursuitOnly)
		{
			var destination = Mock.Of<IRoom>(x => x.Characters == new[] { target });
			var exit = Mock.Of<IRoomExit>(x => x.Destination == destination);
			var episode = new FollowingPath(actor.Object, new[] { exit }) { PathingOwner = ai };
			actor.Setup(x => x.EffectsOfType<FollowingPath>(It.IsAny<Predicate<FollowingPath>>())).Returns(new[] { episode });
			var enabled = typeof(TrackingAggressorAI).GetMethod("IsPathingEnabled", BindingFlags.Instance | BindingFlags.NonPublic)!;
			Assert.IsFalse((bool)enabled.Invoke(ai, [actor.Object])!, "An active episode must stop when its destination's target fails native admission.");
			actor.Setup(x => x.CanEngage(target)).Returns(true);
			Assert.IsTrue((bool)enabled.Invoke(ai, [actor.Object])!, "An admitted target at the episode destination permits continuation.");
		}
		row.Definition = (string)typeof(TrackingAggressorAI).GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(ai, null)!;
		Assert.AreEqual(pursuitOnly, new Tracker(row, world.Object).PursuitOnly);
	}
}
