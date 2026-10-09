#nullable enable

using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.RPG.Law;
using MudSharp.RPG.Law.PatrolStrategies;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests.RPG.Law;

[TestClass]
public class ExecutionPatrolTargetSecurityTests
{
	[DataTestMethod]
	[DataRow("removed")]
	[DataRow("postponed")]
	[DataRow("other-authority")]
	public void HandlePatrolTick_CachedSentenceNoLongerDue_AbortsBeforeKilling(string state)
	{
		var fixture = new Fixture();
		fixture.SetField("_stage", ExecutionPatrolStage.Killing);
		var effect = new AwaitingExecution(fixture.Target.Object,
			state == "other-authority" ? Mock.Of<ILegalAuthority>() : fixture.Authority.Object,
			fixture.Now + MudTimeSpan.FromDays(state == "postponed" ? 1 : -1));
		fixture.SetSentence(state == "removed" ? [] : [effect]);

		fixture.Strategy.HandlePatrolTick(fixture.Patrol.Object);

		fixture.Patrol.Verify(x => x.AbortPatrol(), Times.Once);
		fixture.AssertReset();
		fixture.Target.Verify(x => x.AddEffect(It.IsAny<ExecutionPatrolNoQuit>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void PatrolLifecycle_EndingExternally_ClearsTargetProgressAndNoQuit(bool completed)
	{
		var fixture = new Fixture();
		fixture.SetField("_stage", ExecutionPatrolStage.ConfirmingDeath);
		fixture.SetField("_executionAttempts", 4);
		fixture.SetField("_scriptIndex", 2);
		fixture.SetField("_lastWordsEmoteSent", true);

		if (completed)
		{
			fixture.Strategy.HandlePatrolCompleted(fixture.Patrol.Object);
		}
		else
		{
			fixture.Strategy.HandlePatrolAborted(fixture.Patrol.Object);
		}

		fixture.AssertReset();
		fixture.Target.Verify(x => x.RemoveAllEffects<ExecutionPatrolNoQuit>(
			It.Is<Predicate<ExecutionPatrolNoQuit>>(p => p(new ExecutionPatrolNoQuit(fixture.Target.Object, fixture.Patrol.Object))), true), Times.Once);
		fixture.Target.Verify(x => x.RemoveAllEffects<AwaitingExecution>(It.IsAny<Predicate<AwaitingExecution>>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void HandlePatrolTick_DueTargetAlreadyDead_CompletesSentence()
	{
		var fixture = new Fixture();
		fixture.Target.SetupGet(x => x.State).Returns(CharacterState.Dead);
		fixture.SetSentence([new AwaitingExecution(fixture.Target.Object, fixture.Authority.Object, fixture.Now)]);
		var crime = new Mock<ICrime>();
		crime.SetupGet(x => x.ExecutionPunishment).Returns(true);
		crime.SetupProperty(x => x.SentenceHasBeenServed, false);
		fixture.Authority.Setup(x => x.ResolvedCrimesForIndividual(fixture.Target.Object)).Returns([crime.Object]);

		fixture.Strategy.HandlePatrolTick(fixture.Patrol.Object);

		fixture.Patrol.Verify(x => x.CompletePatrol(), Times.Once);
		fixture.Patrol.Verify(x => x.AbortPatrol(), Times.Never);
		fixture.Target.Verify(x => x.RemoveAllEffects<AwaitingExecution>(It.IsAny<Predicate<AwaitingExecution>>(), true), Times.Once);
		Assert.IsTrue(crime.Object.SentenceHasBeenServed);
		fixture.AssertReset();
	}

	[TestMethod]
	public void EnsureCondemned_CurrentDueSentence_PreservesTargetAndProgress()
	{
		var fixture = new Fixture();
		fixture.SetField("_stage", ExecutionPatrolStage.LastWords);
		fixture.SetSentence([new AwaitingExecution(fixture.Target.Object, fixture.Authority.Object, fixture.Now)]);

		var result = (bool)typeof(ExecutionPatrolStrategy).GetMethod("EnsureCondemned", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(fixture.Strategy, [fixture.Patrol.Object])!;

		Assert.IsTrue(result);
		Assert.AreSame(fixture.Target.Object, fixture.GetField("_condemned"));
		Assert.AreEqual(ExecutionPatrolStage.LastWords, fixture.GetField("_stage"));
		fixture.Patrol.Verify(x => x.AbortPatrol(), Times.Never);
	}

	private sealed class Fixture
	{
		public ExecutionPatrolStrategy Strategy { get; } = TestObjectFactory.CreateUninitialized<ExecutionPatrolStrategy>();
		public Mock<ICharacter> Target { get; } = new();
		public Mock<ILegalAuthority> Authority { get; } = new();
		public Mock<IPatrol> Patrol { get; } = new();
		public MudDateTime Now { get; }

		public Fixture()
		{
			var time = CelestialTestFactory.CreateEarthSystem();
			Now = time.Calendar.CurrentDateTime;
			typeof(PatrolStrategyBase).GetField("<Gameworld>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
				.SetValue(Strategy, time.Gameworld);
			Target.SetupGet(x => x.Id).Returns(42L);
			Patrol.SetupGet(x => x.LegalAuthority).Returns(Authority.Object);
			Patrol.SetupGet(x => x.PatrolPhase).Returns(PatrolPhase.Patrol);
			Patrol.SetupGet(x => x.PatrolMembers).Returns(Array.Empty<ICharacter>());
			SetField("_condemned", Target.Object);
			SetField("_condemnedId", 42L);
		}

		public void SetSentence(AwaitingExecution[] effects) => Target
			.Setup(x => x.EffectsOfType<AwaitingExecution>(It.IsAny<Predicate<AwaitingExecution>>()))
			.Returns((Predicate<AwaitingExecution>? p) => effects.Where(x => p?.Invoke(x) ?? true));

		public void SetField(string name, object value) => typeof(ExecutionPatrolStrategy)
			.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(Strategy, value);

		public object? GetField(string name) => typeof(ExecutionPatrolStrategy)
			.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(Strategy);

		public void AssertReset()
		{
			Assert.IsNull(GetField("_condemned"));
			Assert.AreEqual(0L, GetField("_condemnedId"));
			Assert.AreEqual(ExecutionPatrolStage.SelectingTarget, GetField("_stage"));
			Assert.AreEqual(0, GetField("_executionAttempts"));
			Assert.AreEqual(0, GetField("_scriptIndex"));
			Assert.AreEqual(false, GetField("_lastWordsEmoteSent"));
		}
	}
}
