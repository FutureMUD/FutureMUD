#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Commands.Trees;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Environment;
using MudSharp.Magic.Gathering;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.Testing.EnvironmentalMagic;
using RuntimeProg = MudSharp.FutureProg.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicGatheringServiceTests
{
	[TestMethod]
	public void SelfGathering_CompletionPaysStaminaAndCreditsOnceWithoutEnvironmentalMutation()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 3.0);
		double sourceBefore = fixture.SourceBalance;

		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0);
		Assert.IsTrue(started.Success, $"{started.Message} {fixture.SourceDiagnostic()}");
		Assert.IsNotNull(started.OperationId);
		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success, "Completion cannot skip the configured duration.");

		fixture.Advance(Duration);
		MagicGatheringResult completed = fixture.Service.Complete(fixture.Actor.Object, started.OperationId.Value);
		Assert.IsTrue(completed.Success, completed.Message);
		Assert.AreEqual(7.0, fixture.Stamina, 0.000001);
		Assert.AreEqual(5.0, fixture.DestinationBalance, 0.000001);
		Assert.AreEqual(sourceBefore, fixture.SourceBalance, 0.000001, "Self must never query or debit a room reserve.");
		Assert.AreEqual("Completed", fixture.Store.Operation(started.OperationId.Value)?.Status);
		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId.Value).Success);
		Assert.AreEqual(5.0, fixture.DestinationBalance, 0.000001);
	}

	[TestMethod]
	public void GentleGathering_RealCoordinatorTransfersExactRecordedStockAndDoesNotCreateEcologicalReceipt()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Gentle, ratio: 1.5, cells: 2);
		fixture.SetSourceBalance(50.0);
		double untouched = fixture.World.Balance(1, 1);
		EnvironmentalMagicStateSnapshot beforeState = fixture.World.Coordinator.InspectState(fixture.Cell);

		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 10.0);
		Assert.IsTrue(started.Success, $"{started.Message} {fixture.SourceDiagnostic()}");
		fixture.Advance(Duration);
		MagicGatheringResult completed = fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value);

		Assert.IsTrue(completed.Success, completed.Message);
		Assert.AreEqual(50.0 + Duration.TotalMinutes - 15.0, fixture.SourceBalance, 0.000001,
			"The coordinator may settle its accepted online segment, but the transfer debit itself is exactly 15.");
		Assert.AreEqual(10.0, fixture.DestinationBalance, 0.000001);
		Assert.AreEqual(untouched, fixture.World.Balance(1, 1), 0.000001, "Only the selected physical cell is touched.");
		EnvironmentalMagicStateSnapshot afterState = fixture.World.Coordinator.InspectState(fixture.Cell);
		Assert.AreEqual(beforeState.State.ScarDamage, afterState.State.ScarDamage, 0.000001);
		Assert.AreEqual(0, fixture.World.Operations.Receipts.Count, "A gather transfer is not an ecological operation.");
		MagicGatheringOperationSummary receipt = fixture.Service.Operation(started.OperationId.Value)!;
		Assert.IsTrue(receipt.SourceDebited);
		Assert.AreEqual(15.0, receipt.SourceDebit, 0.000001);
	}

	[TestMethod]
	public void GentlePreview_RealCoordinatorIsPureBeforeDiscoveryAndDoesNotEnumerateWorld()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Gentle, startCoordinator: false);
		fixture.SetSourceBalance(25.0);
		long saves = fixture.World.SaveRequests;
		int subscriptions = fixture.World.SecondSubscriptions;
		int schedules = fixture.World.SchedulerEntries;
		double balance = fixture.SourceBalance;
		fixture.World.Cells.ForbidEnumeration = true;
		fixture.World.Fields.ForbidEnumeration = true;

		MagicGatheringResult first = fixture.Service.Preview(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0);
		MagicGatheringResult second = fixture.Service.Preview(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0);

		Assert.IsTrue(first.Success, $"{first.Message} {fixture.SourceDiagnostic()}");
		Assert.IsTrue(second.Success, $"{second.Message} {fixture.SourceDiagnostic()}");
		Assert.AreEqual(balance, fixture.SourceBalance, 0.000001);
		Assert.AreEqual(saves, fixture.World.SaveRequests);
		Assert.AreEqual(subscriptions, fixture.World.SecondSubscriptions);
		Assert.AreEqual(schedules, fixture.World.SchedulerEntries);
		Assert.AreEqual(0, fixture.Store.Count, "Preview cannot create a commitment receipt.");
	}

	[TestMethod]
	public void GentleDisabledOrEmptySource_RefusesWithoutLegacyFallbackOrCredit()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Gentle);
		fixture.SetSourceBalance(20.0);
		fixture.World.Coordinator.SetBinding(fixture.Cell, EnvironmentalMagicBindingMode.Disabled, null);
		double retainedBalance = fixture.SourceBalance;

		Assert.IsFalse(fixture.Service.Preview(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0).Success);
		Assert.IsFalse(fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0).Success);
		Assert.AreEqual(retainedBalance, fixture.SourceBalance, 0.000001, "Retained historical stock is not a disabled Gentle source.");
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
		Assert.AreEqual(0, fixture.Store.Count);

		using GatheringFixture empty = new(MagicGatheringMethodKind.Gentle);
		Assert.IsFalse(empty.Service.Begin(empty.Actor.Object, empty.Capability.Object, "draw", 1.0).Success);
		Assert.AreEqual(0.0, empty.SourceBalance, 0.000001);
		Assert.AreEqual(0.0, empty.DestinationBalance, 0.000001);
	}

	[TestMethod]
	public void GentleInvalidAndUnboundSources_RefuseWithoutFallbackOrReceipt()
	{
		using GatheringFixture invalid = new(MagicGatheringMethodKind.Gentle);
		invalid.SetSourceBalance(20.0);
		invalid.World.CompileProg(99, "return 0");
		invalid.World.Edit("input divisor prog 99 1");
		invalid.World.Edit("output 1 maximum basecapacity / divisor");
		double invalidBalance = invalid.SourceBalance;

		Assert.IsFalse(invalid.Service.Preview(invalid.Actor.Object, invalid.Capability.Object, "draw", 5.0).Success);
		Assert.IsFalse(invalid.Service.Begin(invalid.Actor.Object, invalid.Capability.Object, "draw", 5.0).Success);
		Assert.AreEqual(invalidBalance, invalid.SourceBalance, 0.000001);
		Assert.AreEqual(0.0, invalid.DestinationBalance, 0.000001);
		Assert.AreEqual(0, invalid.Store.Count);

		using GatheringFixture unbound = new(MagicGatheringMethodKind.Gentle);
		unbound.SetSourceBalance(20.0);
		unbound.World.Coordinator.SetBinding(unbound.Cell, EnvironmentalMagicBindingMode.Explicit, 999);
		double unboundBalance = unbound.SourceBalance;

		Assert.IsFalse(unbound.Service.Preview(unbound.Actor.Object, unbound.Capability.Object, "draw", 5.0).Success);
		Assert.IsFalse(unbound.Service.Begin(unbound.Actor.Object, unbound.Capability.Object, "draw", 5.0).Success);
		Assert.AreEqual(unboundBalance, unbound.SourceBalance, 0.000001);
		Assert.AreEqual(0.0, unbound.DestinationBalance, 0.000001);
		Assert.AreEqual(0, unbound.Store.Count);
	}

	[TestMethod]
	public void GentleGathering_UnconfirmedEnvironmentalOperationRemainsAuthoritativeAndUntouched()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Gentle);
		fixture.SetSourceBalance(20.0);
		EnvironmentalMagicOperationRequest request = new(Guid.NewGuid(), null, "Gathering integration test", Damage: 1.0);
		fixture.World.Operations.FailAfterCommit = true;
		Assert.IsFalse(fixture.World.Coordinator.ApplyOperation(fixture.Cell, request).Success);
		Assert.AreEqual(request.OperationId, fixture.Cell.PendingEnvironmentalOperationId);
		int commits = fixture.World.Operations.Commits;
		int ecologicalReceipts = fixture.World.Operations.Receipts.Count;

		Assert.IsFalse(fixture.Service.Preview(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0).Success);
		Assert.IsFalse(fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0).Success);

		Assert.AreEqual(request.OperationId, fixture.Cell.PendingEnvironmentalOperationId,
			"Gathering may not clear, adopt or retry another operation's persistence freeze.");
		Assert.AreEqual(commits, fixture.World.Operations.Commits);
		Assert.AreEqual(ecologicalReceipts, fixture.World.Operations.Receipts.Count);
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
		Assert.AreEqual(0, fixture.Store.Count, "A refused quote does not create a gathering receipt.");
	}

	[TestMethod]
	public void QuoteFailures_RejectInvalidAmountsBodyAndDestinationHeadroomWithoutAnyGatheringMutation()
	{
		using GatheringFixture body = new(MagicGatheringMethodKind.Self, stamina: 11.0);
		Assert.IsFalse(body.Service.Begin(body.Actor.Object, body.Capability.Object, "draw", 1.0).Success);
		Assert.IsFalse(body.Service.Begin(body.Actor.Object, body.Capability.Object, "draw", double.NaN).Success);
		Assert.AreEqual(10.0, body.Stamina, 0.000001);
		Assert.AreEqual(0.0, body.DestinationBalance, 0.000001);

		using GatheringFixture full = new(MagicGatheringMethodKind.Self, stamina: 1.0);
		full.SetDestinationBalance(99.0);
		Assert.IsFalse(full.Service.Begin(full.Actor.Object, full.Capability.Object, "draw", 2.0).Success);
		Assert.AreEqual(10.0, full.Stamina, 0.000001);
		Assert.AreEqual(99.0, full.DestinationBalance, 0.000001);
	}

	[TestMethod]
	public void PhysicalActionBlockersAndFocus_RefuseOrCancelBeforeAnyGatheringPayment()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 2.0);
		fixture.Actor.SetupGet(x => x.Combat).Returns(Mock.Of<ICombat>());
		Assert.IsFalse(fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0).Success);

		fixture.Actor.SetupGet(x => x.Combat).Returns((ICombat)null!);
		fixture.Actor.SetupGet(x => x.Movement).Returns(Mock.Of<IMovement>());
		Assert.IsFalse(fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0).Success);

		fixture.Actor.SetupGet(x => x.Movement).Returns((IMovement)null!);
		Mock<ICharacterIdentity> identity = new();
		identity.SetupGet(x => x.PrimaryInstance).Returns(Mock.Of<ICharacterInstance>());
		identity.SetupGet(x => x.FocusedInstance).Returns(Mock.Of<ICharacterInstance>());
		fixture.Actor.SetupGet(x => x.Identity).Returns(identity.Object);
		fixture.Actor.SetupGet(x => x.IsPlayerCharacter).Returns(true);
		Assert.IsFalse(fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0).Success);

		fixture.Actor.SetupGet(x => x.Identity).Returns((ICharacterIdentity)null!);
		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0);
		Assert.IsTrue(started.Success, started.Message);
		fixture.Actor.SetupGet(x => x.Combat).Returns(Mock.Of<ICombat>());
		fixture.Advance(Duration);

		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);
		Assert.AreEqual(10.0, fixture.Stamina, 0.000001);
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
		Assert.AreEqual(0, fixture.Store.Count);
	}

	[TestMethod]
	public void GentlePreview_DoesNotSettlePendingProductionOrRetryARefusedRecordedStockDebit()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Gentle);
		fixture.SetSourceBalance(5.0);
		fixture.Advance(TimeSpan.FromMinutes(10));

		MagicGatheringResult refused = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 6.0);
		Assert.IsFalse(refused.Success);
		Assert.AreEqual(5.0, fixture.SourceBalance, 0.000001, "Quote/start cannot materialise a pending coordinator segment.");
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
		Assert.AreEqual(0, fixture.Store.Count);
	}

	[TestMethod]
	public void GentleGathering_UsesRecordedStockFromFullDormantAndZeroRateSources()
	{
		using GatheringFixture full = new(MagicGatheringMethodKind.Gentle);
		full.SetSourceBalance(100.0);
		MagicGatheringResult fullStart = full.Service.Begin(full.Actor.Object, full.Capability.Object, "draw", 5.0);
		Assert.IsTrue(fullStart.Success, full.SourceDiagnostic());
		full.Advance(Duration);
		Assert.IsTrue(full.Service.Complete(full.Actor.Object, fullStart.OperationId!.Value).Success);
		Assert.AreEqual(5.0, full.DestinationBalance, 0.000001);

		using GatheringFixture zeroRate = new(MagicGatheringMethodKind.Gentle);
		zeroRate.World.Edit("output 1 baserate 0");
		zeroRate.SetSourceBalance(10.0);
		Assert.AreEqual(0.0, zeroRate.World.Coordinator.Inspect(zeroRate.Cell).Outputs.Single(x => x.ResourceId == zeroRate.Source.Id).Rate, 0.000001);
		MagicGatheringResult zeroStart = zeroRate.Service.Begin(zeroRate.Actor.Object, zeroRate.Capability.Object, "draw", 5.0);
		Assert.IsTrue(zeroStart.Success, zeroRate.SourceDiagnostic());
		zeroRate.Advance(Duration);
		Assert.IsTrue(zeroRate.Service.Complete(zeroRate.Actor.Object, zeroStart.OperationId!.Value).Success);
		Assert.AreEqual(5.0, zeroRate.DestinationBalance, 0.000001);
		Assert.AreEqual(5.0, zeroRate.SourceBalance, 0.000001);
	}

	[TestMethod]
	public void GentleProfileChangeDuringAction_RefusesBeforeDebitOrCredit()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Gentle);
		fixture.SetSourceBalance(20.0);
		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0);
		Assert.IsTrue(started.Success, fixture.SourceDiagnostic());
		fixture.World.Coordinator.SetBinding(fixture.Cell, EnvironmentalMagicBindingMode.Disabled, null);
		fixture.Advance(Duration);

		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);
		Assert.AreEqual(20.0, fixture.SourceBalance, 0.000001);
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
		Assert.AreEqual(0, fixture.Store.Count);
	}

	[TestMethod]
	public void GatheringMethodRenamePreservesLiveIdentityButStructuralChangeCancelsBeforePayment()
	{
		using GatheringFixture renamed = new(MagicGatheringMethodKind.Gentle);
		renamed.SetSourceBalance(20.0);
		MagicGatheringResult renamedStart = renamed.Service.Begin(renamed.Actor.Object, renamed.Capability.Object, "draw", 5.0);
		Assert.IsTrue(renamedStart.Success, renamedStart.Message);
		renamed.UpdateMethod(method => method with { Alias = "renamed", Name = "Renamed Draw" });
		renamed.Advance(Duration);

		Assert.IsTrue(renamed.Service.Complete(renamed.Actor.Object, renamedStart.OperationId!.Value).Success,
			"Presentation-only changes must not turn a stable method identity into another gathering route.");
		Assert.AreEqual(5.0, renamed.DestinationBalance, 0.000001);

		using GatheringFixture changed = new(MagicGatheringMethodKind.Gentle);
		changed.SetSourceBalance(20.0);
		MagicGatheringResult changedStart = changed.Service.Begin(changed.Actor.Object, changed.Capability.Object, "draw", 5.0);
		Assert.IsTrue(changedStart.Success, changedStart.Message);
		changed.UpdateMethod(method => method with
		{
			StaminaCost = method.StaminaCost + 1.0,
			StructuralVersion = method.StructuralVersion + 1
		});
		changed.Advance(Duration);

		Assert.IsFalse(changed.Service.Complete(changed.Actor.Object, changedStart.OperationId!.Value).Success);
		Assert.AreEqual(20.0, changed.SourceBalance, 0.000001);
		Assert.AreEqual(0.0, changed.DestinationBalance, 0.000001);
		Assert.AreEqual(0, changed.Store.Count, "A rejected precommit action must not create a receipt.");
	}

	[TestMethod]
	public void GentleDestinationRefusalAfterDebit_IsQuarantinedWithoutReplayOrPartialSuccess()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Gentle);
		fixture.SetSourceBalance(20.0);
		fixture.AcceptDestinationCredit = false;
		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0);
		Assert.IsTrue(started.Success, fixture.SourceDiagnostic());
		fixture.Advance(Duration);

		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);
		MagicGatheringOperationSummary receipt = fixture.Service.Operation(started.OperationId.Value)!;
		Assert.IsTrue(receipt.SourceDebited);
		Assert.IsFalse(receipt.DestinationCredited);
		Assert.AreEqual("NeedsReview", receipt.Status);
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId.Value).Success);
	}

	[TestMethod]
	public void SelfDestinationRefusalAfterBodilyCost_IsQuarantinedWithoutReplayOrRefund()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 2.0);
		fixture.AcceptDestinationCredit = false;
		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0);
		Assert.IsTrue(started.Success, started.Message);
		fixture.Advance(Duration);

		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);
		MagicGatheringOperationSummary receipt = fixture.Service.Operation(started.OperationId.Value)!;
		Assert.IsTrue(receipt.BodilyCostApplied);
		Assert.IsFalse(receipt.DestinationCredited);
		Assert.AreEqual("NeedsReview", receipt.Status);
		Assert.AreEqual(8.0, fixture.Stamina, 0.000001, "A paid bodily price is not automatically refunded after an uncertain transfer.");
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId.Value).Success);
		Assert.IsFalse(fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0).Success);
	}

	[TestMethod]
	public void RealCharacterResourceHolder_ClampsCreditAndRefusesItWhileInStasis()
	{
		Mock<IMagicResource> resource = new();
		resource.Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(10.0);
		ResourceHolderCharacter active = ResourceHolderCharacter.Create(CharacterState.Awake);

		active.AddResource(resource.Object, 20.0);

		Assert.AreEqual(10.0, active.MagicResourceAmounts[resource.Object], 0.000001,
			"The concrete character holder clamps to its current resource cap.");
		Assert.IsTrue(active.ResourcesChanged);

		ResourceHolderCharacter stasis = ResourceHolderCharacter.Create(CharacterState.Stasis);
		stasis.AddResource(resource.Object, 1.0);

		Assert.IsFalse(stasis.MagicResourceAmounts.ContainsKey(resource.Object),
			"The concrete holder refuses a resource credit while ongoing character processes cannot run.");
	}

	[TestMethod]
	public void GatherCompletion_DuplicateTokenAndConcurrentOwnerAreRefused()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 1.0);
		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 2.0);
		Assert.IsTrue(started.Success);
		Assert.IsFalse(fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 2.0).Success);
		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, Guid.NewGuid()).Success);

		fixture.Advance(Duration);
		Assert.IsTrue(fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);
		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId.Value).Success);
		Assert.AreEqual(2.0, fixture.DestinationBalance, 0.000001);
	}

	[TestMethod]
	public void GentleGathering_CompetingActionsCannotSpendTheLastRecordedReserveTwice()
	{
		using GatheringFixture first = new(MagicGatheringMethodKind.Gentle);
		using GatheringFixture second = new(first, MagicGatheringMethodKind.Gentle);
		first.SetSourceBalance(15.0);
		MagicGatheringResult firstStart = first.Service.Begin(first.Actor.Object, first.Capability.Object, "draw", 10.0);
		MagicGatheringResult secondStart = second.Service.Begin(second.Actor.Object, second.Capability.Object, "draw", 10.0);
		Assert.IsTrue(firstStart.Success, first.SourceDiagnostic());
		Assert.IsTrue(secondStart.Success, $"{secondStart.Message} {second.SourceDiagnostic()}");
		first.Advance(Duration);

		Assert.IsTrue(first.Service.Complete(first.Actor.Object, firstStart.OperationId!.Value).Success);
		Assert.IsFalse(second.Service.Complete(second.Actor.Object, secondStart.OperationId!.Value).Success);
		Assert.AreEqual(15.0 + Duration.TotalMinutes - 10.0, first.SourceBalance, 0.000001);
		Assert.AreEqual(10.0, first.DestinationBalance + second.DestinationBalance, 0.000001);
	}

	[TestMethod]
	public void HealthPricedSelfGathering_UsesNativeIndependentDirectChannels()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, damage: 2.0, pain: 3.0, stun: 4.0,
			maximumSeverity: WoundSeverity.Moderate);
		double woundDamage = 0.0;
		double woundPain = 0.0;
		double woundStun = 0.0;
		Mock<IWound> wound = new();
		wound.SetupGet(x => x.CurrentDamage).Returns(() => woundDamage);
		wound.SetupGet(x => x.CurrentPain).Returns(() => woundPain);
		wound.SetupGet(x => x.CurrentStun).Returns(() => woundStun);
		DirectHealthCostPlan? plan = new(fixture.Bodypart, null, 2.0, 3.0, 4.0, 2.0, 3.0, 4.0);
		string? planError = null;
		fixture.Health.Setup(x => x.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart, 2.0, 3.0, 4.0,
			WoundSeverity.Moderate, out plan, out planError)).Returns(true);
		fixture.Health.Setup(x => x.ApplyDirectHealthCost(fixture.Actor.Object, plan!)).Returns(() =>
		{
			woundDamage += 2.0;
			woundPain += 3.0;
			woundStun += 4.0;
			return (IReadOnlyList<IWound>)[wound.Object];
		});

		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0);
		Assert.IsTrue(started.Success, started.Message);
		fixture.Advance(Duration);
		Assert.IsTrue(fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);
		Assert.AreEqual(2.0, woundDamage, 0.000001);
		Assert.AreEqual(3.0, woundPain, 0.000001);
		Assert.AreEqual(4.0, woundStun, 0.000001);
		CollectionAssert.AreEqual(new[] { wound.Object }, fixture.PersistedWounds,
			"The service must persist exactly the wound objects returned by the native health strategy.");
		Assert.AreEqual(1.0, fixture.DestinationBalance, 0.000001);
	}

	[TestMethod]
	public void HealthPricedPreview_IsPureAndDoesNotSampleRandomBodyparts()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, damage: 1.0,
			maximumSeverity: WoundSeverity.Moderate);
		DirectHealthCostPlan? plan = new(fixture.Bodypart, null, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0);
		string? planError = null;
		fixture.Health.Setup(x => x.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart, 1.0, 0.0, 0.0,
			WoundSeverity.Moderate, out plan, out planError)).Returns(true);

		MagicGatheringResult first = fixture.Service.Preview(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0);
		MagicGatheringResult second = fixture.Service.Preview(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0);

		Assert.IsTrue(first.Success, first.Message);
		Assert.IsTrue(second.Success, second.Message);
		fixture.Body.VerifyGet(x => x.RandomBodypart, Times.Never,
			"Health-cost preview must examine deterministic eligible parts rather than draw a random target.");
		fixture.Health.Verify(x => x.ApplyDirectHealthCost(It.IsAny<ICharacter>(), It.IsAny<DirectHealthCostPlan>()), Times.Never);
		fixture.Actor.Verify(x => x.SpendStamina(It.IsAny<double>()), Times.Never);
		fixture.Actor.Verify(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()), Times.Never);
		Assert.AreEqual(0, fixture.Wounds.Count);
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
		Assert.AreEqual(0, fixture.Store.Count);
	}

	[TestMethod]
	public void UnsupportedHealthCostStrategy_RefusesBeforeAnyStaminaDebitOrCredit()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 2.0, damage: 1.0,
			maximumSeverity: WoundSeverity.Moderate);
		fixture.Health.SetupGet(x => x.SupportedDirectHealthCostChannels).Returns(DirectHealthCostChannels.None);

		MagicGatheringResult preview = fixture.Service.Preview(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0);
		MagicGatheringResult result = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0);

		Assert.IsFalse(preview.Success);
		Assert.IsFalse(result.Success);
		Assert.AreEqual(10.0, fixture.Stamina, 0.000001);
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
		Assert.AreEqual(0, fixture.Store.Count);
	}

	[TestMethod]
	public void InexactHealthPlan_RefusesBeforeAnyStaminaDebitOrCredit()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 2.0, damage: 1.0,
			maximumSeverity: WoundSeverity.Moderate);
		DirectHealthCostPlan? inexact = new(fixture.Bodypart, null, 1.0, 0.0, 0.0, 0.5, 0.0, 0.0);
		string? planError = null;
		fixture.Health.Setup(x => x.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart, 1.0, 0.0, 0.0,
			WoundSeverity.Moderate, out inexact, out planError)).Returns(true);

		MagicGatheringResult result = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0);

		Assert.IsFalse(result.Success);
		Assert.AreEqual(10.0, fixture.Stamina, 0.000001);
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
		Assert.AreEqual(0, fixture.Store.Count);
		fixture.Health.Verify(x => x.ApplyDirectHealthCost(It.IsAny<ICharacter>(), It.IsAny<DirectHealthCostPlan>()), Times.Never);
	}

	[TestMethod]
	public void ExistingNativeWoundHealthCost_UsesAndPersistsTheCapturedWoundObject()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, damage: 2.0, pain: 3.0, stun: 4.0,
			maximumSeverity: WoundSeverity.Moderate);
		double woundDamage = 6.0;
		double woundPain = 7.0;
		double woundStun = 8.0;
		Mock<IWound> wound = new();
		wound.SetupGet(x => x.CurrentDamage).Returns(() => woundDamage);
		wound.SetupGet(x => x.CurrentPain).Returns(() => woundPain);
		wound.SetupGet(x => x.CurrentStun).Returns(() => woundStun);
		fixture.Wounds.Add(wound.Object);
		DirectHealthCostPlan? plan = new(fixture.Bodypart, wound.Object, 2.0, 3.0, 4.0, 2.0, 3.0, 4.0);
		string? planError = null;
		fixture.Health.Setup(x => x.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart, 2.0, 3.0, 4.0,
			WoundSeverity.Moderate, out plan, out planError)).Returns(true);
		fixture.Health.Setup(x => x.ApplyDirectHealthCost(fixture.Actor.Object, plan!)).Returns(() =>
		{
			woundDamage += 2.0;
			woundPain += 3.0;
			woundStun += 4.0;
			return (IReadOnlyList<IWound>)[wound.Object];
		});

		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0);
		Assert.IsTrue(started.Success, started.Message);
		fixture.Advance(Duration);
		Assert.IsTrue(fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);

		Assert.AreEqual(8.0, woundDamage, 0.000001);
		Assert.AreEqual(10.0, woundPain, 0.000001);
		Assert.AreEqual(12.0, woundStun, 0.000001);
		Assert.AreEqual(1, fixture.Wounds.Count, "An existing native wound must not be replaced by a synthetic duplicate.");
		CollectionAssert.AreEqual(new[] { wound.Object }, fixture.PersistedWounds);
	}

	[TestMethod]
	public void CapturedHealthPlanInvalidatedDuringWait_CancelsWithoutPayment()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 2.0, damage: 1.0,
			maximumSeverity: WoundSeverity.Moderate);
		Mock<IWound> wound = new();
		wound.SetupGet(x => x.CurrentDamage).Returns(0.0);
		wound.SetupGet(x => x.CurrentPain).Returns(0.0);
		wound.SetupGet(x => x.CurrentStun).Returns(0.0);
		DirectHealthCostPlan? plan = new(fixture.Bodypart, null, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0);
		string? planError = null;
		fixture.Health.Setup(x => x.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart, 1.0, 0.0, 0.0,
			WoundSeverity.Moderate, out plan, out planError)).Returns(true);
		fixture.Health.Setup(x => x.ApplyDirectHealthCost(fixture.Actor.Object, plan!)).Returns((IReadOnlyList<IWound>)[wound.Object]);

		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0);
		Assert.IsTrue(started.Success, started.Message);
		fixture.Health.Reset();
		fixture.World.Heartbeat.ManuallyFireHeartbeat5Second();
		fixture.Advance(Duration);

		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);
		Assert.AreEqual(10.0, fixture.Stamina, 0.000001);
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
		Assert.AreEqual(0, fixture.Store.Count);
	}

	[TestMethod]
	public void FailedNativeWoundCheckpoint_QuarantinesBeforeDestinationCredit()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, damage: 1.0,
			maximumSeverity: WoundSeverity.Moderate) { PersistWoundsSucceeds = false };
		double woundDamage = 0.0;
		Mock<IWound> wound = new();
		wound.SetupGet(x => x.CurrentDamage).Returns(() => woundDamage);
		wound.SetupGet(x => x.CurrentPain).Returns(0.0);
		wound.SetupGet(x => x.CurrentStun).Returns(0.0);
		DirectHealthCostPlan? plan = new(fixture.Bodypart, null, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0);
		string? planError = null;
		fixture.Health.Setup(x => x.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart, 1.0, 0.0, 0.0,
			WoundSeverity.Moderate, out plan, out planError)).Returns(true);
		fixture.Health.Setup(x => x.ApplyDirectHealthCost(fixture.Actor.Object, plan!)).Returns(() =>
		{
			woundDamage += 1.0;
			return (IReadOnlyList<IWound>)[wound.Object];
		});

		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0);
		Assert.IsTrue(started.Success, started.Message);
		fixture.Advance(Duration);
		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);

		MagicGatheringOperationSummary receipt = fixture.Service.Operation(started.OperationId.Value)!;
		Assert.IsFalse(receipt.BodilyCostApplied, "The receipt cannot advertise a health price before its native checkpoint.");
		Assert.AreEqual("NeedsReview", receipt.Status);
		Assert.AreEqual(1.0, woundDamage, 0.000001);
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
	}

	[TestMethod]
	public void NewNativeWoundCheckpoint_InitialisesOnlyTheTrackedWoundWithoutAWorldFlush()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, damage: 1.0,
			maximumSeverity: WoundSeverity.Moderate);
		MagicGatheringService service = new(fixture.World.World.Object, fixture.Store, fixture.World.Clock,
			(_, _, _) => fixture.PersistSucceeds);
		bool registered = false;
		double woundDamage = 0.0;
		Mock<IWound> wound = new();
		Mock<ILateInitialisingItem> lateWound = wound.As<ILateInitialisingItem>();
		lateWound.SetupGet(x => x.IdHasBeenRegistered).Returns(() => registered);
		wound.SetupGet(x => x.CurrentDamage).Returns(() => woundDamage);
		wound.SetupGet(x => x.CurrentPain).Returns(0.0);
		wound.SetupGet(x => x.CurrentStun).Returns(0.0);
		fixture.World.Saves.Setup(x => x.DirectInitialise(It.IsAny<ILateInitialisingItem>()))
			.Callback<ILateInitialisingItem>(_ => registered = true);
		DirectHealthCostPlan? plan = new(fixture.Bodypart, null, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0);
		string? planError = null;
		fixture.Health.Setup(x => x.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart, 1.0, 0.0, 0.0,
			WoundSeverity.Moderate, out plan, out planError)).Returns(true);
		fixture.Health.Setup(x => x.ApplyDirectHealthCost(fixture.Actor.Object, plan!)).Returns(() =>
		{
			woundDamage += 1.0;
			return (IReadOnlyList<IWound>)[wound.Object];
		});

		MagicGatheringResult started = service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0);
		Assert.IsTrue(started.Success, started.Message);
		fixture.Advance(Duration);
		Assert.IsTrue(service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);

		fixture.World.Saves.Verify(x => x.DirectInitialise(lateWound.Object), Times.Once);
		fixture.World.Saves.Verify(x => x.Flush(), Times.Never,
			"A health-priced gather must not synchronously flush unrelated world saves.");
		lateWound.Verify(x => x.Save(), Times.Never,
			"A newly initialised wound already contains the applied cost and must not be saved as an existing wound.");
	}

	[TestMethod]
	public void HealthWoundCheckpoint_PrecedesAndDoesNotDependOnPostGatherCallback()
	{
		const long progId = 903;
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, damage: 1.0,
			maximumSeverity: WoundSeverity.Moderate, onGatheredProgId: progId);
		MagicGatheringService service = new(fixture.World.World.Object, fixture.Store, fixture.World.Clock,
			(_, _, _) => fixture.PersistSucceeds);
		bool registered = false;
		bool registeredBeforeCallback = false;
		double woundDamage = 0.0;
		Mock<IWound> wound = new();
		Mock<ILateInitialisingItem> lateWound = wound.As<ILateInitialisingItem>();
		lateWound.SetupGet(x => x.IdHasBeenRegistered).Returns(() => registered);
		wound.SetupGet(x => x.CurrentDamage).Returns(() => woundDamage);
		wound.SetupGet(x => x.CurrentPain).Returns(0.0);
		wound.SetupGet(x => x.CurrentStun).Returns(0.0);
		fixture.World.Saves.Setup(x => x.DirectInitialise(It.IsAny<ILateInitialisingItem>()))
			.Callback<ILateInitialisingItem>(_ => registered = true);
		Mock<IFutureProg> callback = new();
		callback.SetupGet(x => x.Id).Returns(progId);
		callback.Setup(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>()))
			.Callback(() => registeredBeforeCallback = registered)
			.Returns(true);
		fixture.World.Progs.Add(callback.Object);
		DirectHealthCostPlan? plan = new(fixture.Bodypart, null, 1.0, 0.0, 0.0, 1.0, 0.0, 0.0);
		string? planError = null;
		fixture.Health.Setup(x => x.TryPlanDirectHealthCost(fixture.Actor.Object, fixture.Bodypart, 1.0, 0.0, 0.0,
			WoundSeverity.Moderate, out plan, out planError)).Returns(true);
		fixture.Health.Setup(x => x.ApplyDirectHealthCost(fixture.Actor.Object, plan!)).Returns(() =>
		{
			woundDamage += 1.0;
			return (IReadOnlyList<IWound>)[wound.Object];
		});

		MagicGatheringResult started = service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0);
		Assert.IsTrue(started.Success, started.Message);
		fixture.Advance(Duration);
		Assert.IsTrue(service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);

		Assert.IsTrue(registeredBeforeCallback,
			"The callback must observe the already checkpointed native wound, rather than supplying its durability.");
		fixture.World.Saves.Verify(x => x.DirectInitialise(lateWound.Object), Times.Once);
		fixture.World.Saves.Verify(x => x.Flush(), Times.Once,
			"Only the legacy post-gather callback path may request its own flush.");
	}

	[TestMethod]
	public void GeneralAndBodyActionBlockers_RefuseBeforeGatheringBegins()
	{
		using GatheringFixture general = new(MagicGatheringMethodKind.Self, stamina: 1.0);
		general.ActorEffects.Add(BlockingEffect(general.Actor.Object, "general", "You are already occupied."));
		Assert.IsFalse(general.Service.Begin(general.Actor.Object, general.Capability.Object, "draw", 1.0).Success);
		Assert.AreEqual(10.0, general.Stamina, 0.000001);

		using GatheringFixture body = new(MagicGatheringMethodKind.Self, stamina: 1.0);
		body.BodyEffects.Add(BlockingEffect(body.Actor.Object, "movement", "Your body cannot move right now."));
		Assert.IsFalse(body.Service.Begin(body.Actor.Object, body.Capability.Object, "draw", 1.0).Success);
		Assert.AreEqual(10.0, body.Stamina, 0.000001);
	}

	[TestMethod]
	public void ActionBlockerDuringWait_CancelsWithoutPayment()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 1.0);
		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0);
		Assert.IsTrue(started.Success, started.Message);
		fixture.BodyEffects.Add(BlockingEffect(fixture.Actor.Object, "general", "Your body is restrained."));
		fixture.World.Heartbeat.ManuallyFireHeartbeat5Second();
		fixture.Advance(Duration);

		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);
		Assert.AreEqual(10.0, fixture.Stamina, 0.000001);
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
	}

	[TestMethod]
	public void ActionBlockerImmediatelyBeforeCommit_RefusesWithoutPayment()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 1.0);
		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0);
		Assert.IsTrue(started.Success, started.Message);
		fixture.BodyEffects.Add(BlockingEffect(fixture.Actor.Object, "general", "Your body is restrained."));
		fixture.Advance(Duration);

		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);
		Assert.AreEqual(10.0, fixture.Stamina, 0.000001);
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
		Assert.AreEqual(0, fixture.Store.Count);
	}

	[TestMethod]
	public void GatheringTimedAction_DoesNotBlockItsOwnValidation()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 1.0);
		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0);
		Assert.IsTrue(started.Success, started.Message);
		Assert.IsTrue(fixture.ActorEffects.OfType<MagicGatheringTimedAction>().Any());
		fixture.World.Heartbeat.ManuallyFireHeartbeat5Second();
		Assert.IsTrue(fixture.ActorEffects.OfType<MagicGatheringTimedAction>().Any(),
			"The live gathering action is the only blocker ignored by its validation path.");
		fixture.Advance(Duration);
		Assert.IsTrue(fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);
	}

	[TestMethod]
	public void PlayerGatherCommand_UsesTheSameGeneralActionBlockerGate()
	{
		const string schoolVerb = "gatherblockergate";
		MagicModule.EnsureMagicSchoolVerbRegistered(schoolVerb);
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 1.0, schoolVerb: schoolVerb);
		fixture.ActorEffects.Add(BlockingEffect(fixture.Actor.Object, "general", "You are already occupied."));

		Assert.IsTrue(PlayerCommandTree.Instance.Commands.Execute(fixture.Actor.Object,
			$"{schoolVerb} gather \"Test Capability\" draw 1", fixture.Actor.Object.State,
			MudSharp.Accounts.PermissionLevel.Player, fixture.Actor.Object.OutputHandler));

		Assert.IsTrue(fixture.Output.Any(x => x.Contains("already occupied", StringComparison.OrdinalIgnoreCase)),
			string.Join("\n", fixture.Output));
		Assert.AreEqual(10.0, fixture.Stamina, 0.000001);
		Assert.AreEqual(0, fixture.Store.Count);
	}

	[TestMethod]
	public void PersistenceFailureAfterDebitAndCredit_IsQuarantinedAndNeverReplayed()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Gentle) { PersistSucceeds = false };
		fixture.SetSourceBalance(20.0);
		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0);
		Assert.IsTrue(started.Success, fixture.SourceDiagnostic());
		fixture.Advance(Duration);

		MagicGatheringResult completed = fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value);
		Assert.IsFalse(completed.Success);
		Assert.AreEqual(20.0 + Duration.TotalMinutes - 5.0, fixture.SourceBalance, 0.000001);
		Assert.AreEqual(5.0, fixture.DestinationBalance, 0.000001);
		Assert.AreEqual("NeedsReview", fixture.Store.Operation(started.OperationId.Value)?.Status);
		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId.Value).Success);
		Assert.IsFalse(fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0).Success);
		Assert.AreEqual(20.0 + Duration.TotalMinutes - 5.0, fixture.SourceBalance, 0.000001);
		Assert.AreEqual(5.0, fixture.DestinationBalance, 0.000001);
	}

	[TestMethod]
	public void UncertainGentleTransfer_QuarantinesTheAffectedSourceForOtherIdentities()
	{
		using GatheringFixture first = new(MagicGatheringMethodKind.Gentle) { PersistSucceeds = false };
		using GatheringFixture second = new(first, MagicGatheringMethodKind.Gentle);
		first.SetSourceBalance(20.0);
		MagicGatheringResult started = first.Service.Begin(first.Actor.Object, first.Capability.Object, "draw", 5.0);
		Assert.IsTrue(started.Success, first.SourceDiagnostic());
		first.Advance(Duration);
		Assert.IsFalse(first.Service.Complete(first.Actor.Object, started.OperationId!.Value).Success);

		MagicGatheringResult refused = second.Service.Begin(second.Actor.Object, second.Capability.Object, "draw", 1.0);
		Assert.IsFalse(refused.Success);
		StringAssert.Contains(refused.Message, "quarantined");
		Assert.AreEqual(0.0, second.DestinationBalance, 0.000001, "The second identity cannot normalise an uncertain source by making another transfer.");
	}

	[TestMethod]
	public void InterruptedGathering_DoesNotCommitOrResumeAfterItsLiveActionIsRemoved()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 2.0);
		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0);
		Assert.IsTrue(started.Success);
		fixture.MoveActorAway();
		fixture.World.Heartbeat.ManuallyFireHeartbeat5Second();
		fixture.Advance(Duration);

		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);
		Assert.AreEqual(10.0, fixture.Stamina, 0.000001);
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
		Assert.AreEqual(0, fixture.Store.Count);
	}

	[TestMethod]
	public void LayerChange_InterruptsTheCapturedPhysicalGatheringLocation()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 2.0);
		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0);
		Assert.IsTrue(started.Success);
		fixture.ChangeLayer(RoomLayer.InTrees);
		fixture.World.Heartbeat.ManuallyFireHeartbeat5Second();
		fixture.Advance(Duration);

		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);
		Assert.AreEqual(10.0, fixture.Stamina, 0.000001);
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
		Assert.AreEqual(0, fixture.Store.Count);
	}

	[TestMethod]
	public void GatheringMethods_AreAvailableThroughPlayerAndNpcSchoolCommandTreesButNotForAnUnconfiguredCapability()
	{
		const string schoolVerb = "gathercoverage";
		MagicModule.EnsureMagicSchoolVerbRegistered(schoolVerb);
		using GatheringFixture configured = new(MagicGatheringMethodKind.Self, stamina: 2.0, schoolVerb: schoolVerb);
		Assert.IsTrue(PlayerCommandTree.Instance.Commands.TCommands.ContainsKey(schoolVerb));
		Assert.IsTrue(NPCCommandTree.Instance.Commands.TCommands.ContainsKey(schoolVerb));

		Assert.IsTrue(PlayerCommandTree.Instance.Commands.Execute(configured.Actor.Object,
			$"{schoolVerb} gather \"Test Capability\" methods", configured.Actor.Object.State,
			MudSharp.Accounts.PermissionLevel.Player, configured.Actor.Object.OutputHandler));
		Assert.IsTrue(NPCCommandTree.Instance.Commands.Execute(configured.Actor.Object,
			$"{schoolVerb} gather \"Test Capability\" methods", configured.Actor.Object.State,
			MudSharp.Accounts.PermissionLevel.NPC, configured.Actor.Object.OutputHandler));
		Assert.IsTrue(configured.Output.Any(x => x.Contains("Test Draw")), string.Join("\n", configured.Output));

		using GatheringFixture legacy = new(MagicGatheringMethodKind.Self, stamina: 2.0, schoolVerb: schoolVerb,
			configureMethods: false);
		Assert.IsTrue(PlayerCommandTree.Instance.Commands.Execute(legacy.Actor.Object,
			$"{schoolVerb} gather \"Test Capability\" methods", legacy.Actor.Object.State,
			MudSharp.Accounts.PermissionLevel.Player, legacy.Actor.Object.OutputHandler));
		Assert.IsTrue(legacy.Output.Any(x => x.Contains("no configured gathering methods", StringComparison.OrdinalIgnoreCase)),
			string.Join("\n", legacy.Output));
	}

	[TestMethod]
	public void CompiledPostGatherProg_RunsOnceAndIsFlushedBeforeDurableCompletion()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		const long progId = 901;
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 2.0, onGatheredProgId: progId);
		Mock<IVariableRegister> register = new();
		decimal? gathered = null;
		register.Setup(x => x.GetType(ProgVariableTypes.Character, "gathered")).Returns(ProgVariableTypes.Number);
		register.Setup(x => x.SetValue(It.IsAny<IProgVariable>(), "gathered", It.IsAny<IProgVariable>()))
			.Callback<IProgVariable, string, IProgVariable>((_, _, value) => gathered = (decimal)value.GetObject)
			.Returns(true);
		fixture.World.World.SetupGet(x => x.VariableRegister).Returns(register.Object);
		MagicGatheringPolicy.Signatures.TryGetValue("onsuccess", out var signature);
		MudSharp.Models.FutureProg model = new()
		{
			Id = progId,
			FunctionName = "record_gathered_amount",
			ReturnTypeDefinition = signature.Return.ToStorageString(),
			FunctionText = "setregister @owner \"gathered\" @amount\nreturn"
		};
		string[] parameterNames = ["actor", "owner", "capability", "method", "amount", "location", "operation"];
		foreach ((ProgVariableTypes type, int index) in signature.Parameters.Select((type, index) => (type, index)))
		{
			model.FutureProgsParameters.Add(new MudSharp.Models.FutureProgsParameter
			{
				ParameterIndex = index,
				ParameterName = parameterNames[index],
				ParameterTypeDefinition = type.ToStorageString()
			});
		}
		RuntimeProg prog = new(model, fixture.World.World.Object);
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		Assert.IsTrue(MagicGatheringPolicy.ValidSignature(prog, "onsuccess"));
		fixture.World.Progs.Add(prog);

		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 4.0);
		Assert.IsTrue(started.Success, started.Message);
		fixture.Advance(Duration);
		MagicGatheringResult completed = fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value);

		Assert.IsTrue(completed.Success, completed.Message);
		Assert.AreEqual(4m, gathered);
		fixture.World.Saves.Verify(x => x.Flush(), Times.Once);
		Assert.AreEqual("Completed", fixture.Service.Operation(started.OperationId.Value)?.Status);
		Assert.IsFalse(fixture.Service.Complete(fixture.Actor.Object, started.OperationId.Value).Success);
		Assert.AreEqual(4m, gathered, "The post-gather prog cannot be replayed by duplicate completion.");
	}

	[TestMethod]
	public void FailedPostGatherProg_IsQuarantinedAndStaffAcknowledgementNeverReplaysIt()
	{
		const long progId = 902;
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 2.0, onGatheredProgId: progId);
		Mock<IFutureProg> callback = new();
		callback.SetupGet(x => x.Id).Returns(progId);
		callback.Setup(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>())).Returns(false);
		fixture.World.Progs.Add(callback.Object);

		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 4.0);
		Assert.IsTrue(started.Success, started.Message);
		fixture.Advance(Duration);
		MagicGatheringResult completed = fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value);

		Assert.IsTrue(completed.Success, "Saved transfer accounting is not undone when only notification acknowledgement fails.");
		Assert.AreEqual("NeedsReview", fixture.Service.Operation(started.OperationId.Value)?.Status);
		callback.Verify(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>()), Times.Once);
		Assert.IsTrue(fixture.Service.Acknowledge(started.OperationId.Value).Success);
		Assert.AreEqual("Acknowledged", fixture.Service.Operation(started.OperationId.Value)?.Status);
		callback.Verify(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>()), Times.Once);
	}

	[TestMethod]
	public void GatheringStartCompleteAndCancel_StayLocalWhenGlobalRegistriesRejectEnumeration()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Gentle, cells: 2);
		fixture.SetSourceBalance(20.0);
		fixture.World.Cells.ForbidEnumeration = true;
		fixture.World.Fields.ForbidEnumeration = true;

		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0);
		Assert.IsTrue(started.Success, started.Message);
		fixture.Advance(Duration);
		Assert.IsTrue(fixture.Service.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);

		MagicGatheringResult cancellable = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 1.0);
		Assert.IsTrue(cancellable.Success, cancellable.Message);
		Assert.IsTrue(fixture.Service.Cancel(fixture.Actor.Object, cancellable.OperationId).Success);
	}

	[TestMethod]
	public void RestartedGatheringService_DoesNotResumeOrAwardATransientPrecommitAction()
	{
		using GatheringFixture fixture = new(MagicGatheringMethodKind.Self, stamina: 2.0);
		MagicGatheringResult started = fixture.Service.Begin(fixture.Actor.Object, fixture.Capability.Object, "draw", 5.0);
		Assert.IsTrue(started.Success, started.Message);
		MagicGatheringService restarted = new(fixture.World.World.Object, fixture.Store, fixture.World.Clock,
			(_, _, _) => fixture.PersistSucceeds);
		fixture.Advance(Duration);

		Assert.IsFalse(restarted.Complete(fixture.Actor.Object, started.OperationId!.Value).Success);
		Assert.AreEqual(10.0, fixture.Stamina, 0.000001);
		Assert.AreEqual(0.0, fixture.DestinationBalance, 0.000001);
		Assert.AreEqual(0, fixture.Store.Count);
	}

	private static readonly TimeSpan Duration = TimeSpan.FromSeconds(2.0);

	internal static IEffect BlockingEffect(ICharacter owner, string block, string description)
	{
		return new BlockingDelayedAction(owner, _ => { }, description, block, string.Empty);
	}

	internal sealed class GatheringFixture : IDisposable
	{
		private readonly Dictionary<IMagicResource, double> _resources = [];
		private readonly Mock<IBody> _body = new();
		private readonly Mock<IBodypart> _bodypart = new();
		private readonly Mock<IHealthStrategy> _health = new();
		private readonly List<IEffect> _actorEffects = [];
		private readonly List<IEffect> _bodyEffects = [];
		private readonly bool _ownsWorld;
		private ICell _location;
		private RoomLayer _layer = RoomLayer.GroundLevel;
		private MagicGatheringMethodDefinition _method = null!;

		public EnvironmentalMagicTestWorld World { get; }
		public MagicGatheringReceiptMemoryStore Store { get; }
		public int ReceiptCount => Store.Count;
		public MagicGatheringService Service { get; }
		public Mock<ICharacter> Actor { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IMagicGatheringCapability> Capability { get; } = new();
		public IMagicResource Source { get; }
		public IMagicResource Destination { get; }
		public Cell Cell { get; }
		public List<IWound> Wounds { get; } = [];
		public List<IWound> PersistedWounds { get; } = [];
		public List<string> Output { get; } = [];
		public double Stamina { get; private set; } = 10.0;
		public bool PersistSucceeds { get; set; } = true;
		public bool PersistWoundsSucceeds { get; set; } = true;
		public bool AcceptDestinationCredit { get; set; } = true;
		public double DestinationBalance => _resources.GetValueOrDefault(Destination);
		public double SourceBalance => World.Balance(0, Source.Id);
		public MagicGatheringMethodDefinition Method => _method;
		public Mock<IHealthStrategy> Health => _health;
		public Mock<IBody> Body => _body;
		public IBodypart Bodypart => _bodypart.Object;
		public IList<IEffect> ActorEffects => _actorEffects;
		public IList<IEffect> BodyEffects => _bodyEffects;

		public GatheringFixture(MagicGatheringMethodKind kind, double ratio = 1.0, double stamina = 0.0,
			double damage = 0.0, double pain = 0.0, double stun = 0.0, WoundSeverity maximumSeverity = WoundSeverity.None,
			int cells = 1, bool startCoordinator = true, string schoolVerb = "arcane", bool configureMethods = true,
			long onGatheredProgId = 0)
		{
			_ownsWorld = true;
			World = new EnvironmentalMagicTestWorld(cells, 2, start: startCoordinator, clock: new EnvironmentalMagicTestClock());
			Cell = (Cell)World.Cells.At(0);
			_location = Cell;
			Source = World.Resources.Get(1)!;
			Mock<IMagicResource> destination = new();
			destination.SetupGet(x => x.Id).Returns(2L);
			destination.SetupGet(x => x.Name).Returns("Personal Essence");
			destination.SetupGet(x => x.ResourceType).Returns(MagicResourceType.PlayerResource | MagicResourceType.LocationResource);
			destination.Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(100.0);
			Destination = destination.Object;
			World.Resources.Add(Destination);
			_resources[Destination] = 0.0;
			Store = new MagicGatheringReceiptMemoryStore();
			Service = new MagicGatheringService(World.World.Object, Store, World.Clock, (_, _, _) => PersistSucceeds,
				(_, wounds) =>
				{
					PersistedWounds.AddRange(wounds);
					return PersistWoundsSucceeds;
				});
			World.World.SetupGet(x => x.MagicGathering).Returns(Service);

			Mock<IMagicSchool> school = new();
			school.SetupGet(x => x.Id).Returns(1L);
			school.SetupGet(x => x.Name).Returns("Arcane");
			school.SetupGet(x => x.SchoolVerb).Returns(schoolVerb);
			_method = new MagicGatheringMethodDefinition(Guid.NewGuid(), "draw", "Test Draw", kind, Destination.Id,
				kind == MagicGatheringMethodKind.Gentle ? Source.Id : null, 1.0, 20.0, Duration.TotalSeconds,
				ratio, stamina, 0.0, damage, pain, stun, maximumSeverity, OnGatheredProgId: onGatheredProgId);
			Capability.SetupGet(x => x.Id).Returns(22L);
			Capability.SetupGet(x => x.Name).Returns("Test Capability");
			Capability.SetupGet(x => x.School).Returns(school.Object);
			Capability.SetupGet(x => x.GatheringMethods).Returns(() => configureMethods ? [Method] : []);
			Capability.Setup(x => x.GatheringConfigurationErrors()).Returns(Array.Empty<string>());

			_body.SetupGet(x => x.Id).Returns(33L);
			_bodypart.SetupGet(x => x.Id).Returns(44L);
			_bodypart.SetupGet(x => x.DamageModifier).Returns(1.0);
			_bodypart.SetupGet(x => x.PainModifier).Returns(1.0);
			_bodypart.SetupGet(x => x.StunModifier).Returns(1.0);
			_body.SetupGet(x => x.RandomBodypart).Returns(_bodypart.Object);
			_body.SetupGet(x => x.Bodyparts).Returns(() => [_bodypart.Object]);
			_body.SetupGet(x => x.Effects).Returns(() => _bodyEffects);
			_health.Setup(x => x.GetSeverity(It.IsAny<double>())).Returns(WoundSeverity.Moderate);
			_health.SetupGet(x => x.SupportedDirectHealthCostChannels).Returns(
				DirectHealthCostChannels.Damage | DirectHealthCostChannels.Pain | DirectHealthCostChannels.Stun);
			Actor.SetupGet(x => x.Id).Returns(11L);
			Actor.SetupGet(x => x.Name).Returns("Gatherer");
			Actor.SetupGet(x => x.FrameworkItemType).Returns("Character");
			Actor.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
			Actor.SetupGet(x => x.GetObject).Returns(() => Actor.Object);
			Actor.SetupGet(x => x.Gameworld).Returns(World.World.Object);
			Actor.SetupGet(x => x.Identity).Returns((ICharacterIdentity)null!);
			Actor.SetupGet(x => x.Body).Returns(_body.Object);
			Actor.SetupGet(x => x.CurrentBody).Returns(_body.Object);
			Actor.SetupGet(x => x.Location).Returns(() => _location);
			Actor.SetupGet(x => x.SpatialLocation).Returns(() => new SpatialLocation(_location, _layer));
			Actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
			Actor.SetupGet(x => x.Combat).Returns((ICombat)null!);
			Actor.SetupGet(x => x.Movement).Returns((IMovement)null!);
			Actor.SetupGet(x => x.Effects).Returns(() => _actorEffects);
			Actor.Setup(x => x.IsBlocked(It.IsAny<string[]>())).Returns((string[] blocks) => FindActionBlocker(blocks));
			Actor.SetupGet(x => x.Capabilities).Returns(() => [Capability.Object]);
			Actor.SetupGet(x => x.MagicResources).Returns(() => _resources.Keys);
			Actor.SetupGet(x => x.MagicResourceAmounts).Returns(() => _resources);
			Actor.Setup(x => x.AddResource(Destination, It.IsAny<double>())).Callback<IMagicResource, double>((resource, amount) =>
			{
				if (AcceptDestinationCredit)
				{
					_resources[resource] = Math.Min(100.0, _resources.GetValueOrDefault(resource) + amount);
				}
			});
			Actor.SetupGet(x => x.CurrentStamina).Returns(() => Stamina);
			Actor.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns<double>(amount => amount <= Stamina);
			Actor.Setup(x => x.SpendStamina(It.IsAny<double>())).Callback<double>(amount => Stamina -= amount);
			Actor.SetupGet(x => x.HealthStrategy).Returns(_health.Object);
			Actor.SetupGet(x => x.Wounds).Returns(() => Wounds);
			Actor.Setup(x => x.AddWounds(It.IsAny<IEnumerable<IWound>>())).Callback<IEnumerable<IWound>>(wounds => Wounds.AddRange(wounds));
			Actor.Setup(x => x.SufferDamage(It.IsAny<IDamage>())).Returns(Array.Empty<IWound>());
			Actor.Setup(x => x.GetFormat(It.IsAny<Type>())).Returns<Type>(type => CultureInfo.InvariantCulture.GetFormat(type)!);
			Actor.SetupGet(x => x.Account).Returns(Mock.Of<IAccount>());
			Actor.SetupGet(x => x.LineFormatLength).Returns(120);
			Mock<IOutputHandler> output = new();
			output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
				.Callback<string, bool, bool>((text, _, _) => Output.Add(text)).Returns(true);
			Actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
			Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
				.Callback<IEffect, TimeSpan>((effect, _) => _actorEffects.Add(effect));
			Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, fire) =>
			{
				_actorEffects.Remove(effect);
				if (fire)
				{
					effect.RemovalEffect();
				}
			});
			World.ResetSavedFlags();
		}

		private (bool Truth, string Message) FindActionBlocker(IEnumerable<string> blocks)
		{
			foreach (IEffect effect in _actorEffects.Concat(_bodyEffects))
			{
				foreach (string block in blocks)
				{
					if (effect.IsBlockingEffect(block))
					{
						return (true, effect.BlockingDescription(block, Actor.Object));
					}
				}
			}

			return (false, string.Empty);
		}

		public GatheringFixture(GatheringFixture shared, MagicGatheringMethodKind kind)
		{
			_ownsWorld = false;
			World = shared.World;
			Cell = shared.Cell;
			_location = Cell;
			Source = shared.Source;
			Destination = shared.Destination;
			_resources[Destination] = 0.0;
			Store = shared.Store;
			Service = shared.Service;
			SetupSecondActor(kind);
		}

		private void SetupSecondActor(MagicGatheringMethodKind kind)
		{
			Mock<IMagicSchool> school = new();
			school.SetupGet(x => x.Id).Returns(1L);
			school.SetupGet(x => x.Name).Returns("Arcane");
			school.SetupGet(x => x.SchoolVerb).Returns("arcane");
			_method = new MagicGatheringMethodDefinition(Guid.NewGuid(), "draw", "Second Draw", kind, Destination.Id,
				kind == MagicGatheringMethodKind.Gentle ? Source.Id : null, 1.0, 20.0, Duration.TotalSeconds);
			Capability.SetupGet(x => x.Id).Returns(23L);
			Capability.SetupGet(x => x.Name).Returns("Second Capability");
			Capability.SetupGet(x => x.School).Returns(school.Object);
			Capability.SetupGet(x => x.GatheringMethods).Returns(() => [Method]);
			Capability.Setup(x => x.GatheringConfigurationErrors()).Returns(Array.Empty<string>());
			_body.SetupGet(x => x.Id).Returns(34L);
			Actor.SetupGet(x => x.Id).Returns(12L);
			Actor.SetupGet(x => x.Name).Returns("Second Gatherer");
			Actor.SetupGet(x => x.FrameworkItemType).Returns("Character");
			Actor.SetupGet(x => x.Gameworld).Returns(World.World.Object);
			Actor.SetupGet(x => x.Identity).Returns((ICharacterIdentity)null!);
			Actor.SetupGet(x => x.Body).Returns(_body.Object);
			Actor.SetupGet(x => x.CurrentBody).Returns(_body.Object);
			Actor.SetupGet(x => x.Location).Returns(() => _location);
			Actor.SetupGet(x => x.SpatialLocation).Returns(() => new SpatialLocation(_location, _layer));
			Actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
			Actor.SetupGet(x => x.Combat).Returns((ICombat)null!);
			Actor.SetupGet(x => x.Movement).Returns((IMovement)null!);
			Actor.SetupGet(x => x.Capabilities).Returns(() => [Capability.Object]);
			Actor.SetupGet(x => x.MagicResources).Returns(() => _resources.Keys);
			Actor.SetupGet(x => x.MagicResourceAmounts).Returns(() => _resources);
			Actor.Setup(x => x.AddResource(Destination, It.IsAny<double>())).Callback<IMagicResource, double>((resource, amount) =>
			{
				if (AcceptDestinationCredit)
				{
					_resources[resource] = Math.Min(100.0, _resources.GetValueOrDefault(resource) + amount);
				}
			});
			Actor.SetupGet(x => x.CurrentStamina).Returns(() => Stamina);
			Actor.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns<double>(amount => amount <= Stamina);
			Actor.Setup(x => x.SpendStamina(It.IsAny<double>())).Callback<double>(amount => Stamina -= amount);
			Actor.SetupGet(x => x.HealthStrategy).Returns(_health.Object);
			Actor.SetupGet(x => x.Wounds).Returns(() => Wounds);
			Actor.Setup(x => x.SufferDamage(It.IsAny<IDamage>())).Returns(Array.Empty<IWound>());
			Actor.Setup(x => x.GetFormat(It.IsAny<Type>())).Returns<Type>(type => CultureInfo.InvariantCulture.GetFormat(type)!);
			Actor.SetupGet(x => x.Account).Returns(Mock.Of<IAccount>());
			Actor.SetupGet(x => x.LineFormatLength).Returns(120);
			Mock<IOutputHandler> output = new();
			output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
				.Callback<string, bool, bool>((text, _, _) => Output.Add(text)).Returns(true);
			Actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
			Actor.Setup(x => x.AddEffect(It.IsAny<MagicGatheringTimedAction>(), It.IsAny<TimeSpan>()));
			Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, fire) =>
			{
				if (fire)
				{
					effect.RemovalEffect();
				}
			});
		}

		public void SetSourceBalance(double amount)
		{
			Cell.AddResource(Source, amount - SourceBalance);
			World.ResetSavedFlags();
		}
		public void SetDestinationBalance(double amount) => _resources[Destination] = amount;
		public void UpdateMethod(Func<MagicGatheringMethodDefinition, MagicGatheringMethodDefinition> update) =>
			_method = update(Method);

		public void Advance(TimeSpan duration) => World.Clock.Advance(duration);
		public void MoveActorAway() => _location = World.CreateCell(0.0);
		public void ChangeLayer(RoomLayer layer) => _layer = layer;
		public string SourceDiagnostic()
		{
			EnvironmentalMagicSnapshot snapshot = World.Coordinator.Inspect(Cell);
			EnvironmentalResourceSnapshot? output = snapshot.Outputs.FirstOrDefault(x => x.ResourceId == Source.Id);
			return $"Source balance={SourceBalance}, snapshot valid={snapshot.IsValid}, profile={snapshot.ProfileId}, output={output?.Balance}/{output?.Maximum}, output valid={output?.IsValid}, errors={string.Join(" | ", snapshot.Errors)}";
		}
		public void Dispose()
		{
			// A fixture sharing an existing world is only an alternate actor and must not tear down the coordinator.
			if (_ownsWorld)
			{
				World.Dispose();
			}
		}
	}

	private sealed class ResourceHolderCharacter : MudSharp.Character.Character
	{
		private static readonly FieldInfo GameworldBackingField =
			typeof(LateKeywordedInitialisingItem).GetField("<Gameworld>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!;
		private static readonly FieldInfo NoSaveField =
			typeof(LateKeywordedInitialisingItem).GetField("_noSave", BindingFlags.Instance | BindingFlags.NonPublic)!;
		private static readonly FieldInfo StateField =
			typeof(MudSharp.Character.Character).GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic)!;
		private static readonly FieldInfo ResourceAmountsField =
			typeof(MudSharp.Character.Character).GetField("_magicResourceAmounts", BindingFlags.Instance | BindingFlags.NonPublic)!;
		private static readonly FieldInfo GeneratorDelegatesField =
			typeof(MudSharp.Character.Character).GetField("_generatorDelegateDictionary", BindingFlags.Instance | BindingFlags.NonPublic)!;

		private ResourceHolderCharacter() : base(null!, null!, true)
		{
		}

		public static ResourceHolderCharacter Create(CharacterState state)
		{
			ResourceHolderCharacter character = (ResourceHolderCharacter)RuntimeHelpers.GetUninitializedObject(typeof(ResourceHolderCharacter));
			GameworldBackingField.SetValue(character, Mock.Of<IFuturemud>());
			NoSaveField.SetValue(character, true);
			StateField.SetValue(character, state);
			ResourceAmountsField.SetValue(character, new DoubleCounter<IMagicResource>());
			GeneratorDelegatesField.SetValue(character, new Dictionary<IMagicResourceRegenerator, HeartbeatManagerDelegate>());
			return character;
		}
	}

	internal sealed class MagicGatheringReceiptMemoryStore : IMagicGatheringReceiptStore
	{
		private readonly Dictionary<Guid, MagicGatheringReceipt> _operations = [];
		public int Count => _operations.Count;
		public bool TryCreate(MagicGatheringReceipt receipt) => _operations.TryAdd(receipt.Id, receipt);
		public void Record(MagicGatheringReceipt receipt) => _operations[receipt.Id] = receipt;
		public MagicGatheringReceipt? Operation(Guid id) => _operations.GetValueOrDefault(id);
		public IReadOnlyList<MagicGatheringReceipt> Unresolved(long? ownerId = null) => _operations.Values
			.Where(x => (!ownerId.HasValue || x.OwnerId == ownerId.Value) && x.Status is "Committing" or "Invoking" or "NeedsReview")
			.OrderByDescending(x => x.UpdatedUtc)
			.ToArray();
		public bool HasUnresolved(long ownerId) => Unresolved(ownerId).Count > 0;
		public bool HasUnresolvedForSource(long cellId, long sourceResourceId) => _operations.Values.Any(x =>
			x.CellId == cellId && x.SourceResourceId == sourceResourceId &&
			(x.Status is "Committing" or "Invoking" or "NeedsReview"));
	}
}
