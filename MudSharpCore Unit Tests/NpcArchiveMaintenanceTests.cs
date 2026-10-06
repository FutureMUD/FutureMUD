#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.NPC;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NpcArchiveMaintenanceTests
{
	private static readonly DateTime Now = new(2026, 10, 3, 6, 0, 0, DateTimeKind.Utc);
	private static SpellOwnedLifecycle Eligible() => new(
		new(Guid.NewGuid(), 1, 3, 2, "maintenance-test", SpellLifecycleMode.TemporaryCleanup, Now.AddHours(-1), Now.AddMinutes(-30)),
		[new(SpellOwnedEntityKind.AutonomousCharacter, 123), new(SpellOwnedEntityKind.Body, 456)],
		SpellLifecycleState.RemainsPending, SpellRetirementReason.EarlyDeath, Now.AddMinutes(-20), null, Now.AddMinutes(-20), 7, "");

	private static bool TryArchive(IFuturemud world, IReadOnlyList<SpellOwnedLifecycle> lifecycles, long? bodyId, out string diagnostic)
	{
		object?[] args = [world, 123L, bodyId, lifecycles, Now, null];
		var result = (bool)typeof(ImplementorModule).GetMethod("TryArchiveMaintenanceNpc", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, args)!;
		diagnostic = (string)args[5]!; return result;
	}

	[DataTestMethod]
	[DataRow("ordinary")]
	[DataRow("ambiguous")]
	[DataRow("permanent")]
	[DataRow("active")]
	[DataRow("uncorrelated")]
	[DataRow("foreign-actor")]
	[DataRow("foreign-body")]
	[DataRow("extra-body")]
	[DataRow("projection")]
	[DataRow("topology")]
	[DataRow("completed")]
	[DataRow("missing-body")]
	public void TryArchiveMaintenanceNpc_UnqualifiedIdentity_RetainsWithoutLoadingOrArchiveCalls(string scenario)
	{
		var world = new Mock<IFuturemud>(MockBehavior.Strict); var life = Eligible(); long? bodyId = 456;
		IReadOnlyList<SpellOwnedLifecycle> lifecycles = [life];
		switch (scenario)
		{
			case "ordinary": lifecycles = []; break;
			case "ambiguous": lifecycles = [life, life]; break;
			case "permanent": lifecycles = [life with { Origin = life.Origin with { Mode = SpellLifecycleMode.Permanent, DeadlineUtc = null } }]; break;
			case "active": lifecycles = [life with { State = SpellLifecycleState.Active }]; break;
			case "uncorrelated": lifecycles = [life with { DeathObservedUtc = null }]; break;
			case "foreign-actor": lifecycles = [life with { Entities = [new(SpellOwnedEntityKind.AutonomousCharacter, 999), new(SpellOwnedEntityKind.Body, 456)] }]; break;
			case "foreign-body": lifecycles = [life with { Entities = [new(SpellOwnedEntityKind.AutonomousCharacter, 123), new(SpellOwnedEntityKind.Body, 999)] }]; break;
			case "extra-body": lifecycles = [life with { Entities = life.Entities.Append(new(SpellOwnedEntityKind.Body, 999)).ToArray() }]; break;
			case "projection": lifecycles = [life with { Entities = [new(SpellOwnedEntityKind.CharacterInstance, 123), new(SpellOwnedEntityKind.Body, 456)] }]; break;
			case "topology": lifecycles = [life with { Entities = life.Entities.Append(new(SpellOwnedEntityKind.Cell, 999)).ToArray() }]; break;
			case "completed": lifecycles = [life with { State = SpellLifecycleState.Completed }]; break;
			case "missing-body": bodyId = null; break;
		}
		Assert.IsFalse(TryArchive(world.Object, lifecycles, bodyId, out var diagnostic));
		Assert.IsFalse(string.IsNullOrEmpty(diagnostic)); world.VerifyNoOtherCalls();
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void TryArchiveMaintenanceNpc_ProvenNpc_UsesExactServiceVersionAndPropagatesHold(bool succeeds)
	{
		var life = Eligible(); var body = new Mock<IBody>(); body.SetupGet(x => x.Id).Returns(456);
		var npc = new Mock<INPC>(); npc.SetupGet(x => x.Id).Returns(123); npc.SetupGet(x => x.Body).Returns(body.Object);
		var archives = new Mock<ICharacterArchiveService>(); var diagnostic = succeeds ? "" : "serialized foreign reference hold";
		archives.Setup(x => x.TryArchiveNpc(life.Origin.Id, 7, npc.Object, Now, out diagnostic)).Returns(succeeds);
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.CharacterArchives).Returns(archives.Object);
		world.Setup(x => x.TryGetCharacter(123, true)).Returns(npc.Object);
		Assert.AreEqual(succeeds, TryArchive(world.Object, [life], 456, out var actual));
		Assert.AreEqual(diagnostic, actual); archives.Verify(x => x.TryArchiveNpc(life.Origin.Id, 7, npc.Object, Now, out diagnostic), Times.Once);
	}

	[TestMethod]
	public void TryArchiveMaintenanceNpc_UnavailableService_RetainsWithoutConstructingOrLoadingActor()
	{
		var world = new Mock<IFuturemud>();
		Assert.IsFalse(TryArchive(world.Object, [Eligible()], 456, out var diagnostic));
		StringAssert.Contains(diagnostic, "unavailable"); world.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void TryArchiveMaintenanceNpc_UnresolvedActor_RetainsWithoutCallingArchive()
	{
		var archives = new Mock<ICharacterArchiveService>(MockBehavior.Strict); var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.CharacterArchives).Returns(archives.Object);
		Assert.IsFalse(TryArchive(world.Object, [Eligible()], 456, out var diagnostic));
		StringAssert.Contains(diagnostic, "could not be resolved"); archives.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void CleanupAllPCsLoaded_NormalCompletion_QuitsEveryTemporaryPcAndResumesStatistics()
	{
		var statistics = new Mock<IGameStatistics>(); statistics.SetupProperty(x => x.RecordPlayersPaused, true);
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.GameStatistics).Returns(statistics.Object);
		var first = new Mock<ICharacter>(); var second = new Mock<ICharacter>();
		first.Setup(x => x.Quit(true)).Returns(true); second.Setup(x => x.Quit(true)).Returns(true);
		Cleanup(world.Object, [first.Object, second.Object]);
		first.Verify(x => x.Quit(true), Times.Once); second.Verify(x => x.Quit(true), Times.Once);
		Assert.IsFalse(statistics.Object.RecordPlayersPaused);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void CleanupAllPCsLoaded_QuitFailure_AttemptsRemainingPcResumesStatisticsAndReportsFailure(bool throws)
	{
		var statistics = new Mock<IGameStatistics>(); statistics.SetupProperty(x => x.RecordPlayersPaused, true);
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.GameStatistics).Returns(statistics.Object);
		var failed = new Mock<ICharacter>(); var other = new Mock<ICharacter>(); other.Setup(x => x.Quit(true)).Returns(true);
		if (throws) failed.Setup(x => x.Quit(true)).Throws(new InvalidOperationException("quit fault"));
		else failed.Setup(x => x.Quit(true)).Returns(false);
		var error = Assert.ThrowsException<TargetInvocationException>(() => Cleanup(world.Object, [failed.Object, other.Object]));
		Assert.IsInstanceOfType(error.InnerException, typeof(AggregateException));
		Assert.AreEqual(1, ((AggregateException)error.InnerException!).InnerExceptions.Count);
		other.Verify(x => x.Quit(true), Times.Once); Assert.IsFalse(statistics.Object.RecordPlayersPaused);
	}

	private static void Cleanup(IFuturemud world, IReadOnlyList<ICharacter> pcs) =>
		typeof(ImplementorModule).GetMethod("CleanupAllPCsLoaded", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, [world, pcs]);
}
