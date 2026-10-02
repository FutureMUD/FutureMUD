#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellLifecyclePolicyTests
{
	private static readonly DateTime Created = new(2026, 10, 2, 0, 0, 0, DateTimeKind.Utc);
	private static SpellOwnedLifecycle Lifecycle(SpellLifecycleMode mode = SpellLifecycleMode.TemporaryCleanup,
		SpellLifecycleState state = SpellLifecycleState.Active, bool dead = false) => new(
		new(Guid.NewGuid(), 7, 3, 11, "guardian", mode, Created, mode == SpellLifecycleMode.Permanent ? null : Created.AddMinutes(2)),
		[new(SpellOwnedEntityKind.AutonomousCharacter, 15)], state, null, dead ? Created.AddMinutes(1) : null,
		dead ? 31 : null, Created, 1, "");

	[TestMethod]
	public void BeginRetirement_PermanentCreation_RefusesAutomaticRemoval()
	{
		var lifecycle = Lifecycle(SpellLifecycleMode.Permanent);
		lifecycle.Origin.Validate();
		Assert.IsFalse(lifecycle.MayRemoveOwnedEntities);
		Assert.ThrowsException<InvalidOperationException>(() => SpellLifecycleTransitions.BeginRetirement(lifecycle,
			SpellRetirementReason.Expiry, Created.AddHours(1)));
	}

	[TestMethod]
	public void BeginRetirement_AbsoluteDeadline_PreventsEarlyExpiry()
	{
		var lifecycle = Lifecycle();
		Assert.ThrowsException<InvalidOperationException>(() => SpellLifecycleTransitions.BeginRetirement(lifecycle,
			SpellRetirementReason.Expiry, Created.AddSeconds(119)));
		Assert.AreEqual(SpellLifecycleState.Retiring, SpellLifecycleTransitions.BeginRetirement(lifecycle,
			SpellRetirementReason.Expiry, Created.AddMinutes(2)));
	}

	[DataTestMethod]
	[DataRow(SpellRetirementReason.Dispel)]
	[DataRow(SpellRetirementReason.Dismissal)]
	[DataRow(SpellRetirementReason.CapabilityLoss)]
	[DataRow(SpellRetirementReason.Logout)]
	public void BeginRetirement_ExplicitReason_CanRetireBeforeDeadline(SpellRetirementReason reason)
	{
		Assert.AreEqual(SpellLifecycleState.Retiring, SpellLifecycleTransitions.BeginRetirement(Lifecycle(), reason, Created));
	}

	[DataTestMethod]
	[DataRow(SpellLifecycleState.Retiring)]
	[DataRow(SpellLifecycleState.RemainsPending)]
	[DataRow(SpellLifecycleState.Completed)]
	public void BeginRetirement_RepeatedExpiry_DoesNotReopenState(SpellLifecycleState state)
	{
		Assert.AreEqual(state, SpellLifecycleTransitions.BeginRetirement(Lifecycle(state: state),
			SpellRetirementReason.Expiry, Created.AddHours(1)));
	}

	[TestMethod]
	public void RequiresNativeDeath_EarlyDeath_RemovesLaterDeathIntent()
	{
		Assert.IsTrue(Lifecycle(SpellLifecycleMode.DeathOnExpiry, SpellLifecycleState.Retiring).RequiresNativeDeath);
		Assert.IsFalse(Lifecycle(SpellLifecycleMode.DeathOnExpiry, SpellLifecycleState.RemainsPending, true).RequiresNativeDeath);
		Assert.IsFalse(Lifecycle(SpellLifecycleMode.TemporaryCleanup, SpellLifecycleState.Retiring).RequiresNativeDeath);
	}

	[TestMethod]
	public void Origin_InvalidOrNonUtcDeadline_RejectsAmbiguousLifetime()
	{
		var origin = Lifecycle().Origin;
		Assert.ThrowsException<ArgumentException>(() => (origin with { DeadlineUtc = Created }).Validate());
		Assert.ThrowsException<ArgumentException>(() => (origin with { DeadlineUtc = null }).Validate());
		Assert.ThrowsException<ArgumentException>(() => (origin with { DeadlineUtc = DateTime.SpecifyKind(Created.AddHours(1), DateTimeKind.Unspecified) }).Validate());
		Assert.ThrowsException<ArgumentException>(() => (origin with { Grade = 8 }).Validate());
		Assert.ThrowsException<ArgumentException>(() => (origin with { Provenance = new string('a', 2049) }).Validate());
	}

	[TestMethod]
	public void BeginRetirement_EarlyDeathReason_RequiresPostDeathCorrelation()
	{
		Assert.ThrowsException<ArgumentException>(() => SpellLifecycleTransitions.BeginRetirement(Lifecycle(),
			SpellRetirementReason.EarlyDeath, Created));
	}
}
