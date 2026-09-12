using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic.Vancian;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class VancianRecoveryTests
{
	[TestMethod]
	public void IdentityInstancesShareSelectionsAndSpentLedgerWithoutDuplicatingGrants()
	{
		var f = new VancianTestFixture(); var identity = new Mock<ICharacterIdentity>(); var projection = new Mock<ICharacterInstance>();
		identity.SetupGet(x => x.PrimaryInstance).Returns(f.Actor.Object); identity.SetupGet(x => x.Instances).Returns([f.Actor.Object,projection.Object]);
		f.Actor.SetupGet(x => x.Identity).Returns(identity.Object); projection.SetupGet(x => x.Identity).Returns(identity.Object);
		projection.SetupGet(x => x.Id).Returns(99); projection.SetupGet(x => x.Capabilities).Returns([f.Capability.Object,f.Capability.Object]);
		projection.SetupGet(x => x.State).Returns(CharacterState.Awake);
		f.Select(2); f.Plan(2,2); f.Refresh(); var state = f.State; state.Slots[0].Status = VancianSlotStatus.Spent; f.Store.Commit(state,state.Version);
		Assert.AreEqual(10,f.Service.State(projection.Object,f.Capability.Object).OwnerId);
		Assert.AreEqual(VancianSlotStatus.Spent,f.Service.Slots(projection.Object,f.Capability.Object)[0].Status);
		Assert.IsFalse(f.Service.CanCast(projection.Object,f.Capability.Object,f.Rules[0].Key,f.Allowances[0].Key,f.Spells[1],1).Available);
		Assert.IsTrue(f.Service.CanCast(projection.Object,f.Capability.Object,f.Rules[0].Key,f.Allowances[0].Key,f.Spells[1],2).Available);
		Assert.AreEqual(1,f.Store.States.Count);
		f.HasCapability = false; Assert.IsFalse(f.Service.CanCast(f.Actor.Object,f.Capability.Object,f.Rules[0].Key,f.Allowances[0].Key,f.Spells[1],2).Available);
		f.HasCapability = true; Assert.AreEqual(VancianSlotStatus.Spent,f.State.Slots[0].Status);
	}
	[DataTestMethod]
	[DataRow(CharacterState.Unconscious)]
	[DataRow(CharacterState.Stasis)]
	[DataRow(CharacterState.Dead)]
	public void InvalidSleepAndOfflineGapsNeverEarnCredit(CharacterState interruption)
	{
		var f = new VancianTestFixture(); f.Capability.SetupGet(x => x.RecoveryMode).Returns(VancianRecoveryMode.SleepThenPreparation);
		var current = CharacterState.Sleeping; f.Actor.SetupGet(x => x.State).Returns(() => current);
		using var tracker = new VancianSleepTracker(f.Actor.Object,f.Service);
		f.Clock.Advance(TimeSpan.FromSeconds(5)); tracker.Tick(); current |= interruption; tracker.Tick();
		f.Clock.Advance(TimeSpan.FromSeconds(10)); tracker.Tick(); Assert.IsFalse(f.State.SleepQualified);
		current = CharacterState.Sleeping; tracker.Tick(); f.Clock.Advance(TimeSpan.FromHours(1)); tracker.Tick();
		Assert.IsFalse(f.State.SleepQualified); tracker.Dispose();
		using var reconnected = new VancianSleepTracker(f.Actor.Object,f.Service); Assert.IsFalse(f.State.SleepQualified);
	}
	[TestMethod]
	public void ContinuousSleepCreditSurvivesCancelledPreparationAndIsConsumedOnlyAtRefresh()
	{
		var f = new VancianTestFixture(); f.Select(2); f.Plan(2);
		f.Capability.SetupGet(x => x.RecoveryMode).Returns(VancianRecoveryMode.SleepThenPreparation);
		var current = CharacterState.Sleeping; f.Actor.SetupGet(x => x.State).Returns(() => current);
		using var tracker = new VancianSleepTracker(f.Actor.Object,f.Service);
		f.Clock.Advance(TimeSpan.FromSeconds(5)); tracker.Tick(); f.Clock.Advance(TimeSpan.FromSeconds(5)); tracker.Tick();
		Assert.IsTrue(f.State.SleepQualified); var writes = f.Store.Writes;
		f.Clock.Advance(TimeSpan.FromSeconds(5)); tracker.Tick(); Assert.AreEqual(writes,f.Store.Writes);
		current = CharacterState.Awake; tracker.Tick(); Assert.AreEqual(0,f.State.Generation);
		Assert.IsTrue(f.Service.RequestRefresh(f.Actor.Object,f.Capability.Object).Success); f.Action!.RemovalEffect();
		Assert.IsTrue(f.State.SleepQualified); Assert.AreEqual(0,f.State.Slots.Count);
		// Reconnection keeps completed credit but does not resume the cancelled timed action.
		f.Actor.Setup(x => x.EffectsOfType<VancianTimedAction>()).Returns([]);
		f.Refresh(); Assert.IsFalse(f.State.SleepQualified); Assert.AreEqual(1,f.State.Generation);
		Assert.IsFalse(f.Service.RequestRefresh(f.Actor.Object,f.Capability.Object).Success);
	}
	[TestMethod]
	public void AutomaticWakeRefusalRetainsCreditForExplicitRetryWithoutHeartbeatSpam()
	{
		var f = new VancianTestFixture(); f.Select(2); f.Plan(2);
		f.Capability.SetupGet(x => x.RecoveryMode).Returns(VancianRecoveryMode.SleepAutomatic);
		var permit = false; f.Policies["canrefresh"] = f.Prog("canrefresh",_ => permit).Object.Id;
		var current = CharacterState.Sleeping; f.Actor.SetupGet(x => x.State).Returns(() => current);
		using var tracker = new VancianSleepTracker(f.Actor.Object,f.Service);
		f.Clock.Advance(TimeSpan.FromSeconds(10)); tracker.Tick(); current = CharacterState.Awake; tracker.Tick();
		Assert.IsTrue(f.State.SleepQualified); Assert.AreEqual(0,f.State.Generation); var writes = f.Store.Writes;
		tracker.Tick(); tracker.Tick(); Assert.AreEqual(writes,f.Store.Writes);
		permit = true; Assert.IsTrue(f.Service.RequestRefresh(f.Actor.Object,f.Capability.Object).Success);
		Assert.IsFalse(f.State.SleepQualified); Assert.AreEqual(1,f.State.Generation); tracker.Tick(); Assert.AreEqual(1,f.State.Generation);
	}
	[TestMethod]
	public void AwakeProjectionInterruptsSleepImmediatelyAndIntervalsAreIdentityOwned()
	{
		var f = new VancianTestFixture(); f.Select(2); f.Plan(2); f.Refresh();
		f.Capability.SetupGet(x => x.MinimumRefreshInterval).Returns(TimeSpan.FromHours(3));
		f.Capability.SetupGet(x => x.RecoveryMode).Returns(VancianRecoveryMode.SleepThenPreparation);
		var identity = new Mock<ICharacterIdentity>(); var projection = new Mock<ICharacterInstance>(); var projectionState = CharacterState.Sleeping;
		projection.SetupGet(x => x.State).Returns(() => projectionState); projection.SetupGet(x => x.ControlPolicy).Returns(CharacterInstanceControlPolicy.PlayerRemoteCommandable);
		identity.SetupGet(x => x.PrimaryInstance).Returns(f.Actor.Object); identity.SetupGet(x => x.FocusedInstance).Returns(f.Actor.Object);
		identity.SetupGet(x => x.Instances).Returns([f.Actor.Object,projection.Object]); f.Actor.SetupGet(x => x.Identity).Returns(identity.Object);
		f.Actor.SetupGet(x => x.State).Returns(CharacterState.Sleeping);
		using var tracker = new VancianSleepTracker(f.Actor.Object,f.Service);
		f.Clock.Advance(TimeSpan.FromSeconds(5)); tracker.Tick(); projectionState = CharacterState.Awake;
		projection.Raise(x => x.OnStateChanged += null,projection.Object); projectionState = CharacterState.Sleeping; projection.Raise(x => x.OnStateChanged += null,projection.Object);
		f.Clock.Advance(TimeSpan.FromSeconds(5)); tracker.Tick(); Assert.IsFalse(f.State.SleepQualified);
		f.Clock.Advance(TimeSpan.FromSeconds(5)); tracker.Tick(); Assert.IsTrue(f.State.SleepQualified);
		f.Actor.SetupGet(x => x.State).Returns(CharacterState.Awake); tracker.Tick();
		Assert.IsFalse(f.Service.RequestRefresh(f.Actor.Object,f.Capability.Object).Success); Assert.IsTrue(f.State.SleepQualified);
		f.Clock.Advance(TimeSpan.FromHours(3)); f.Actor.Setup(x => x.EffectsOfType<VancianTimedAction>()).Returns([]);
		f.Refresh(); Assert.AreEqual(2,f.State.Generation);
	}
	[TestMethod]
	public void WholeCapabilityHooksUseImmutableSnapshotsAndCanonicalOwnerExactlyOnce()
	{
		var f = new VancianTestFixture(); var identity = new Mock<ICharacterIdentity>(); var projection = new Mock<ICharacterInstance>();
		identity.SetupGet(x => x.PrimaryInstance).Returns(f.Actor.Object); projection.SetupGet(x => x.Identity).Returns(identity.Object);
		projection.SetupGet(x => x.Capabilities).Returns([f.Capability.Object]); var calls = 0;
		f.Policies["canchangeknown"] = f.Prog("canchangeknown",args =>
		{
			calls++; Assert.AreSame(f.Actor.Object,args[0]);
			Assert.ThrowsException<NotSupportedException>(() => ((IList<MudSharp.Magic.IMagicSpell>)args[3]).Clear());
			return true;
		}).Object.Id;
		Assert.IsTrue(f.Service.CommitKnown(projection.Object,f.Capability.Object,0,new Dictionary<Guid,IReadOnlyList<long>> { [f.Rules[0].Key] = [1,2] }).Success);
		Assert.AreEqual(1,calls); Assert.AreEqual(2,f.State.Selections[f.Rules[0].Key].Count);
		Assert.IsTrue(f.Service.CommitKnown(projection.Object,f.Capability.Object,f.State.Version,new Dictionary<Guid,IReadOnlyList<long>> { [f.Rules[0].Key] = [2,1] }).Success);
		Assert.AreEqual(1,calls);
	}
}
