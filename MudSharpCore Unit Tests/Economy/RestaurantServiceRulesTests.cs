#nullable enable
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Economy;
using MudSharp.Economy.Shops;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using System;
using System.Globalization;
using System.Linq;

namespace MudSharpCore_Unit_Tests.Economy;

[TestClass]
public class RestaurantServiceRulesTests
{
	[TestMethod]
	public void CanAutomaticallyJoin_AllowsCurrentPartyMembership()
	{
		Assert.IsTrue(RestaurantServiceRules.CanAutomaticallyJoin(true, false));
	}

	[TestMethod]
	public void CanAutomaticallyJoin_AllowsAllyStatusGrantedByExistingParticipant()
	{
		Assert.IsTrue(RestaurantServiceRules.CanAutomaticallyJoin(false, true));
	}

	[TestMethod]
	public void CanAutomaticallyJoin_RejectsReverseOnlyOrNoRelationship()
	{
		// The caller supplies the existing participant's IsAlly(requester) result. A requester
		// marking the participant as an ally therefore remains false here and cannot self-grant entry.
		Assert.IsFalse(RestaurantServiceRules.CanAutomaticallyJoin(false, false));
	}

	[TestMethod]
	public void EstimateWait_CombinesPreparationQueueHandlingAndBatchWindow()
	{
		var wait = RestaurantServiceRules.EstimateWait(
			TimeSpan.FromMinutes(2),
			TimeSpan.FromSeconds(30),
			queuedOrdersAhead: 3,
			maximumBatchWait: TimeSpan.FromMinutes(1),
			quantity: 2);

		Assert.AreEqual(TimeSpan.FromMinutes(7), wait);
	}

	[TestMethod]
	public void PreparationTime_IncludesConfiguredCraftPhasesAndIgnoresInvalidNegativeDurations()
	{
		var preparation = RestaurantServiceRules.PreparationTime(
			TimeSpan.FromSeconds(15),
			[TimeSpan.FromMinutes(2), TimeSpan.FromSeconds(45), TimeSpan.FromSeconds(-10)]);

		Assert.AreEqual(TimeSpan.FromMinutes(3), preparation);
	}

	[TestMethod]
	public void SuggestEqualSplit_DistributesRemainderDeterministicallyWithoutChangingLiability()
	{
		var suggestion = RestaurantServiceRules.SuggestEqualSplit([23L, 11L, 17L], 10.0M);

		Assert.AreEqual(4.0M, suggestion[11L]);
		Assert.AreEqual(3.0M, suggestion[17L]);
		Assert.AreEqual(3.0M, suggestion[23L]);
		Assert.AreEqual(10.0M, suggestion.Values.Sum());
	}

	[TestMethod]
	public void PaymentToApply_CapsPartialPaymentAtOutstandingBalance()
	{
		Assert.AreEqual(7.0M, RestaurantServiceRules.PaymentToApply(7.0M, 10.0M));
		Assert.AreEqual(10.0M, RestaurantServiceRules.PaymentToApply(15.0M, 10.0M));
		Assert.AreEqual(0.0M, RestaurantServiceRules.PaymentToApply(-2.0M, 10.0M));
	}

	[TestMethod]
	public void OutstandingLiability_PartialPaymentRetainsOnlyUnpaidActiveBalance()
	{
		// The value supplied here is the gross non-refund payment total. Refund records are kept
		// separately so a partial refund cannot turn a previously settled line back into debt.
		Assert.AreEqual(3.0M, RestaurantServiceRules.OutstandingLiability(RestaurantOrderStatus.ReadyForService, 10.0M, 7.0M));
		Assert.AreEqual(0.0M, RestaurantServiceRules.OutstandingLiability(RestaurantOrderStatus.Served, 10.0M, 10.0M));
		Assert.AreEqual(0.0M, RestaurantServiceRules.OutstandingLiability(RestaurantOrderStatus.Cancelled, 10.0M, 0.0M));
		Assert.AreEqual(0.0M, RestaurantServiceRules.OutstandingLiability(RestaurantOrderStatus.Failed, 10.0M, 4.0M));
		Assert.AreEqual(0.0M, RestaurantServiceRules.OutstandingLiability(RestaurantOrderStatus.Refunded, 10.0M, 0.0M));
	}

	[TestMethod]
	public void IsBatchReady_ReleasesWhenAllRelatedOrdersAreReadyOrDelayExpires()
	{
		var now = new DateTime(2031, 5, 12, 12, 0, 0, DateTimeKind.Utc);

		Assert.IsTrue(RestaurantServiceRules.IsBatchReady(true, null, TimeSpan.FromMinutes(3), now));
		Assert.IsFalse(RestaurantServiceRules.IsBatchReady(false, now - TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(3), now));
		Assert.IsTrue(RestaurantServiceRules.IsBatchReady(false, now - TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3), now));
	}

	[TestMethod]
	public void IsTableCleanupDue_UsesTheConfiguredMinuteScaleCadence()
	{
		var now = new DateTime(2031, 5, 12, 12, 0, 0, DateTimeKind.Utc);

		Assert.IsTrue(RestaurantServiceRules.IsTableCleanupDue(null, TimeSpan.FromMinutes(2), now));
		Assert.IsFalse(RestaurantServiceRules.IsTableCleanupDue(now - TimeSpan.FromSeconds(119), TimeSpan.FromMinutes(2), now));
		Assert.IsTrue(RestaurantServiceRules.IsTableCleanupDue(now - TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(2), now));
		Assert.IsTrue(RestaurantServiceRules.IsTableCleanupDue(now, TimeSpan.Zero, now));
	}

	[TestMethod]
	public void NewlyProducedQuantity_PreservesARealCraftResultMergedIntoExistingStock()
	{
		Assert.AreEqual(1, RestaurantServiceRules.NewlyProducedQuantity(currentQuantity: 3, quantityBeforeCraft: 2));
		Assert.AreEqual(0, RestaurantServiceRules.NewlyProducedQuantity(currentQuantity: 2, quantityBeforeCraft: 2));
		Assert.AreEqual(0, RestaurantServiceRules.NewlyProducedQuantity(currentQuantity: 1, quantityBeforeCraft: 2));
	}

	[TestMethod]
	public void RestaurantServiceEmotes_HaveStableDefaultsAndBuilderAliases()
	{
		Assert.AreEqual("@ place|places $0 before $1 on $2.",
			RestaurantServiceEmotes.DefaultFor(RestaurantServiceEmoteType.ServerServe));
		Assert.IsTrue(RestaurantServiceEmotes.TryParse("plate", out var plateType));
		Assert.AreEqual(RestaurantServiceEmoteType.ChefPlate, plateType);
		Assert.AreEqual(RestaurantServiceEmotes.DefaultChefPlate,
			RestaurantServiceEmotes.Normalize(RestaurantServiceEmoteType.ChefPlate, "  "));
	}

	[TestMethod]
	public void ValidateFulfilmentConfiguration_RejectsUnavailableAndInvalidOpenService()
	{
		Assert.IsFalse(Validate(RestaurantFulfilmentMode.BringUnaltered, out var unavailableReason,
			dineInAvailable: false, takeawayAvailable: false));
		StringAssert.Contains(unavailableReason, "neither dine-in nor takeaway");

		Assert.IsFalse(Validate(RestaurantFulfilmentMode.OpenAndBring, out var openReason,
			itemCanBeOpened: false));
		StringAssert.Contains(openReason, "cannot be opened");
	}

	[TestMethod]
	public void ValidateFulfilmentConfiguration_RequiresRealValidCraftOutputAndPlate()
	{
		Assert.IsFalse(Validate(RestaurantFulfilmentMode.CraftAndBring, out var noCraftReason,
			hasCraft: false, craftIsValidAndProducesOutput: false));
		StringAssert.Contains(noCraftReason, "no craft assigned");

		Assert.IsFalse(Validate(RestaurantFulfilmentMode.CraftAndBring, out var badCraftReason,
			hasCraft: true, craftIsValidAndProducesOutput: false));
		StringAssert.Contains(badCraftReason, "does not produce");

		Assert.IsFalse(Validate(RestaurantFulfilmentMode.CraftAndPlate, out var noPlateReason,
			hasCraft: true, craftIsValidAndProducesOutput: true, hasServingContainer: false));
		StringAssert.Contains(noPlateReason, "no serving container");
		Assert.IsTrue(Validate(RestaurantFulfilmentMode.CraftAndPlate, out _,
			hasCraft: true, craftIsValidAndProducesOutput: true, hasServingContainer: true));
	}

	[TestMethod]
	public void ValidateFulfilmentConfiguration_RequiresCompatibleTakeawayPackaging()
	{
		Assert.IsFalse(Validate(RestaurantFulfilmentMode.PackageTakeaway, out var noContainerReason,
			hasTakeawayContainer: false));
		StringAssert.Contains(noContainerReason, "no inner container");

		Assert.IsFalse(Validate(RestaurantFulfilmentMode.PackageTakeaway, out var incompatibleReason,
			hasTakeawayContainer: true, takeawayContainerIsCompatible: false));
		StringAssert.Contains(incompatibleReason, "not a container");

		Assert.IsTrue(Validate(RestaurantFulfilmentMode.PackageTakeaway, out _,
			hasTakeawayContainer: true, hasTakeawayBag: true, takeawayBagIsCompatible: true));
	}

	[TestMethod]
	public void TakeawayBagPacking_GroupsTheEntireCollectionIntoTheFewestPossibleBags()
	{
		var items = new[]
		{
			TakeawayItem(1, 6.0),
			TakeawayItem(2, 4.0),
			TakeawayItem(3, 5.0),
			TakeawayItem(4, 5.0)
		};

		Assert.IsTrue(RestaurantTakeawayBagPacking.TryPlan(TakeawayBag(10.0), items, out var plan, out var reason),
			reason);
		Assert.AreEqual(2, plan.Count, "Four takeaway items should not be assigned one bag each.");
		Assert.IsTrue(plan.All(x => x.Sum(y => y.Weight) <= 10.0));
		CollectionAssert.AreEquivalent(items.Select(x => x.Id).ToList(),
			plan.SelectMany(x => x).Select(x => x.Id).ToList());
	}

	[TestMethod]
	public void TakeawayBagPacking_RejectsAnAmbiguousMultiContainerBagWithoutThrowing()
	{
		Assert.IsFalse(RestaurantTakeawayBagPacking.TryPlan(TakeawayBag(10.0, 2), [TakeawayItem(1, 1.0)],
			out _, out var reason));
		StringAssert.Contains(reason, "exactly one standard Container component");
	}

	private static bool Validate(RestaurantFulfilmentMode mode, out string reason, bool dineInAvailable = true,
		bool takeawayAvailable = true, bool hasCraft = true, bool craftIsValidAndProducesOutput = true,
		bool itemCanBeOpened = true, bool hasServingContainer = true, bool servingContainerIsCompatible = true,
		bool hasTakeawayContainer = true, bool takeawayContainerIsCompatible = true, bool hasTakeawayBag = false,
		bool takeawayBagIsCompatible = true)
	{
		return RestaurantServiceRules.ValidateFulfilmentConfiguration(
			mode,
			dineInAvailable,
			takeawayAvailable,
			hasCraft,
			craftIsValidAndProducesOutput,
			itemCanBeOpened,
			hasServingContainer,
			servingContainerIsCompatible,
			hasTakeawayContainer,
			takeawayContainerIsCompatible,
			hasTakeawayBag,
			takeawayBagIsCompatible,
			out reason);
	}

	private static IGameItemProto TakeawayBag(double capacity, int containerCount = 1)
	{
		var bag = new Mock<IGameItemProto>();
		bag.SetupGet(x => x.Components).Returns(Enumerable.Range(0, containerCount)
			.Select(_ => (IGameItemComponentProto)new TestContainerGameItemComponentProto(capacity))
			.ToList());
		return bag.Object;
	}

	private static IGameItem TakeawayItem(long id, double weight)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Weight).Returns(weight);
		item.SetupGet(x => x.Size).Returns(SizeCategory.Small);
		return item.Object;
	}

	private sealed class TestContainerGameItemComponentProto : ContainerGameItemComponentProto
	{
		public TestContainerGameItemComponentProto(double capacity) : base(new MudSharp.Models.GameItemComponentProto
		{
			Id = 1,
			Name = "test takeaway container",
			Description = string.Empty,
			Definition = $"<Definition Weight=\"{capacity.ToString(CultureInfo.InvariantCulture)}\" MaxSize=\"{(int)SizeCategory.Gigantic}\"><AllowedTags /><BlockedTags /></Definition>",
			EditableItem = new MudSharp.Models.EditableItem()
		}, new Mock<IFuturemud>().Object)
		{
		}
	}
}
