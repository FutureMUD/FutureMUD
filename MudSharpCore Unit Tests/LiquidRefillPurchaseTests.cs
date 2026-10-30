#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg.Functions.GameItem;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Components;
using System.Runtime.CompilerServices;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LiquidRefillPurchaseTests
{
	[TestMethod]
	public void NativePreparedCommit_WritesMixtureWithoutExposureOrParentCallbacks()
	{
		// No parent/world is available: the ordinary setter would enter exposure and throw.
		var native = (LiquidContainerGameItemComponent)RuntimeHelpers.GetUninitializedObject(typeof(LiquidContainerGameItemComponent));
		var mixture = new Fixture(0.5).Container.Object.LiquidMixture;
		native.CommitPreparedLiquidMixture(mixture);
		Assert.AreSame(mixture, native.LiquidMixture);
	}
	[TestMethod]
	[DataRow(0.0, 60)]
	[DataRow(0.95, 3)]
	[DataRow(0.975, 2)]
	[DataRow(0.999, 1)]
	[DataRow(1.0, 0)]
	public void Quote_PartialSourceUnits_ProrratesAndRoundsUp(double current, int price)
	{
		var fixture = new Fixture(current);
		Assert.AreEqual((decimal)price, fixture.Quote());
	}

	[TestMethod]
	public void Quote_ClosedProjectedOrOtherLiquid_RefusesWithoutMutation()
	{
		var fixture = new Fixture(0.5);
		fixture.Container.SetupGet(x => x.IsOpen).Returns(false);
		Assert.AreEqual(-1m, fixture.Quote());
		fixture.Container.SetupGet(x => x.IsOpen).Returns(true);
		fixture.Container.SetupGet(x => x.OwnsLiquidMixture).Returns(false);
		Assert.AreEqual(-1m, fixture.Quote());
		fixture.Container.SetupGet(x => x.OwnsLiquidMixture).Returns(true);
		var other = new Mock<ILiquid>();
		other.SetupGet(x => x.Id).Returns(99);
		fixture.Container.Object.LiquidMixture = new LiquidMixture(other.Object, 0.5, fixture.World.Object);
		Assert.AreEqual(-1m, fixture.Quote());
	}

	[TestMethod]
	public void Quote_InvalidVolumesOrPrices_Refuses()
	{
		var fixture = new Fixture(0);
		Assert.AreEqual(-1m, LiquidRefillPurchase.Quote(fixture.Container.Object, fixture.Water.Object, 0, 3));
		Assert.AreEqual(-1m, LiquidRefillPurchase.Quote(fixture.Container.Object, fixture.Water.Object, double.NaN, 3));
		Assert.AreEqual(-1m, LiquidRefillPurchase.Quote(fixture.Container.Object, fixture.Water.Object, 0.05, -1));
		fixture.Container.SetupGet(x => x.LiquidCapacity).Returns(double.PositiveInfinity);
		Assert.AreEqual(-1m, fixture.Quote());
	}

	[TestMethod]
	public void PrepareAndCommit_LeavesOriginalUntouchedUntilCommitThenFillsMissingWater()
	{
		var fixture = new Fixture(0.975);
		var original = fixture.Container.Object.LiquidMixture;
		using var purchase = fixture.Purchase();
		Assert.AreEqual(2m, purchase.Price);
		Assert.IsTrue(purchase.Prepare());
		Assert.AreSame(original, fixture.Container.Object.LiquidMixture);
		purchase.Commit();
		Assert.AreEqual(1.0, fixture.Container.Object.LiquidMixture.TotalVolume, 0.000000001);
		Assert.AreEqual(0.975, original.TotalVolume, 0.000000001);
	}

	[TestMethod]
	public void PreparedPurchase_LiquidMutatedInPlace_RejectsStalePrice()
	{
		var fixture = new Fixture(0.5);
		using var purchase = fixture.Purchase();
		Assert.IsTrue(purchase.Prepare());
		fixture.Container.Object.LiquidMixture.SetLiquidVolume(0.75);
		Assert.IsFalse(purchase.IsCurrent);
		Assert.IsFalse(purchase.Prepare());
	}

	[TestMethod]
	public void PreparedPurchase_ContainerMovedOrSellerUnavailable_RejectsWithoutRefill()
	{
		var fixture = new Fixture(0.5);
		using var purchase = fixture.Purchase();
		fixture.Item.SetupGet(x => x.InInventoryOf).Returns((IBody)null!);
		Assert.IsFalse(purchase.Prepare());
		fixture.Item.SetupGet(x => x.InInventoryOf).Returns(fixture.Body.Object);
		fixture.Seller.SetupGet(x => x.State).Returns(CharacterState.Sleeping);
		Assert.IsFalse(purchase.Prepare());
		Assert.AreEqual(0.5, fixture.Container.Object.LiquidVolume);
	}

	[TestMethod]
	public void LiquidLookup_NonIntegralOrOversizedIds_RefusesWithoutOverflow()
	{
		var world = new Mock<IFuturemud>(MockBehavior.Strict);
		foreach (var id in new[] { -1m, 0m, 1.5m, decimal.MaxValue })
			Assert.IsNull(LiquidRefillPurchase.ResolveLiquid(world.Object, id));
	}

	private sealed class Fixture
	{
		internal Mock<IFuturemud> World { get; } = new();
		internal Mock<ILiquid> Water { get; } = new();
		internal Mock<IPreparedLiquidContainer> Container { get; } = new();
		internal Mock<IGameItem> Item { get; } = new();
		internal Mock<IBody> Body { get; } = new();
		internal Mock<ICharacter> Buyer { get; } = new();
		internal Mock<ICharacter> Seller { get; } = new();

		internal Fixture(double current)
		{
			Water.SetupGet(x => x.Id).Returns(65);
			Water.SetupGet(x => x.Density).Returns(1.0);
			Container.SetupProperty(x => x.LiquidMixture, new LiquidMixture(Water.Object, current, World.Object));
			Container.Setup(x => x.CommitPreparedLiquidMixture(It.IsAny<LiquidMixture>()))
				.Callback<LiquidMixture>(value => Container.Object.LiquidMixture = value);
			Container.SetupGet(x => x.LiquidVolume).Returns(() => Container.Object.LiquidMixture.TotalVolume);
			Container.SetupGet(x => x.LiquidCapacity).Returns(1.0);
			Container.SetupGet(x => x.IsOpen).Returns(true);
			Container.SetupGet(x => x.OwnsLiquidMixture).Returns(true);
			Buyer.SetupGet(x => x.Body).Returns(Body.Object);
			Buyer.SetupGet(x => x.State).Returns(CharacterState.Awake);
			Seller.SetupGet(x => x.State).Returns(CharacterState.Awake);
			Seller.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
			Buyer.Setup(x => x.ColocatedWith(Seller.Object)).Returns(true);
			Item.SetupGet(x => x.Gameworld).Returns(World.Object);
			Item.SetupGet(x => x.InInventoryOf).Returns(Body.Object);
			Item.Setup(x => x.GetItemType<ILiquidContainer>()).Returns(Container.Object);
			Body.SetupGet(x => x.HeldOrWieldedItems).Returns([Item.Object]);
		}

		internal decimal Quote() => LiquidRefillPurchase.Quote(Container.Object, Water.Object, 0.05, 3);
		internal LiquidRefillPurchase Purchase() => new(Buyer.Object, Seller.Object, Item.Object, Container.Object, Water.Object, 0.05, 3);
	}
}
