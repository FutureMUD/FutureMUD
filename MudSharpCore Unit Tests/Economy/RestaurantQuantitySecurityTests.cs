#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Economy.Shops;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp_Unit_Tests.Economy;

[TestClass]
public class RestaurantQuantitySecurityTests
{
	[TestMethod]
	public void RestaurantQuantity_DefaultRequiresNoOperatorSetup()
	{
		Assert.AreEqual("100", DefaultStaticSettings.DefaultStaticConfigurations[RestaurantServiceRules.MaximumOrderQuantityConfiguration]);
	}

	private sealed class Fixture
	{
		public Restaurant Restaurant { get; } = TestObjectFactory.CreateUninitialized<Restaurant>();
		public Mock<IFuturemud> World { get; } = new();
		public List<IRestaurantOrder> Orders { get; } = new();
		public Mock<ICharacter> Customer { get; } = new();

		public Fixture(int configuredLimit = 100)
		{
			World.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			World.Setup(x => x.GetStaticInt(RestaurantServiceRules.MaximumOrderQuantityConfiguration)).Returns(configuredLimit);
			typeof(Restaurant).GetProperty(nameof(Restaurant.Gameworld))!.SetValue(Restaurant, World.Object);
			typeof(Restaurant).GetProperty(nameof(Restaurant.IsTrading))!.SetValue(Restaurant, true);
			Customer.SetupGet(x => x.Id).Returns(42);
			SetField("_orders", Orders);
			SetField("_tableSessions", new List<IRestaurantTableSession>());
			var requests = typeof(Restaurant).GetField("_pendingJoinRequests", BindingFlags.Instance | BindingFlags.NonPublic)!;
			requests.SetValue(Restaurant, Activator.CreateInstance(requests.FieldType));
			SetField("_automatedService", true);
			SetField("_simulateCrafting", true);
		}

		public void SetField(string name, object value) => typeof(Restaurant)
			.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(Restaurant, value);

		public RestaurantOperationResult Create(int quantity, RestaurantOrderType type, RestaurantMenuItem? menu = null) =>
			(RestaurantOperationResult)typeof(Restaurant).GetMethod("CreateOrder", BindingFlags.Instance | BindingFlags.NonPublic)!
				.Invoke(Restaurant, [Customer.Object, Customer.Object, null, menu, quantity, type, null])!;

		public RestaurantOrder Add(int quantity, RestaurantOrderStatus status = RestaurantOrderStatus.Queued)
		{
			var order = new RestaurantOrder(new MudSharp.Models.RestaurantOrder
			{
				Id = Orders.Count + 1, Quantity = quantity, Status = (int)status, OrdererCharacterId = 42,
				AmountPaid = 20m, Price = 20m, OperationalNotes = string.Empty
			}, Restaurant, null, null!);
			Orders.Add(order);
			return order;
		}
	}

	[DataTestMethod]
	[DataRow(int.MaxValue, RestaurantOrderType.DineIn)]
	[DataRow(int.MaxValue, RestaurantOrderType.Takeaway)]
	[DataRow(101, RestaurantOrderType.DineIn)]
	[DataRow(0, RestaurantOrderType.Takeaway)]
	[DataRow(-1, RestaurantOrderType.DineIn)]
	public void CreateOrder_InvalidQuantity_RefusesBeforeMenuPricingPaymentOrPersistence(int quantity, RestaurantOrderType type)
	{
		var f = new Fixture();
		var result = f.Create(quantity, type);
		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Message, "between 1 and 100");
		Assert.AreEqual(0, f.Orders.Count);
		f.World.VerifyGet(x => x.SaveManager, Times.Never);
	}

	[TestMethod]
	public void CreateOrder_ExcessiveConfiguredLimit_StillEnforcesTheSafetyCeiling()
	{
		var f = new Fixture(int.MaxValue);
		var result = f.Create(1001, RestaurantOrderType.DineIn);
		Assert.IsFalse(result.Success);
		Assert.AreEqual(0, f.Orders.Count);
	}

	[DataTestMethod]
	[DataRow(1, 100)]
	[DataRow(100, 100)]
	[DataRow(500, 500)]
	public void CreateOrder_WithinConfiguredRange_ContinuesToNormalMenuValidation(int quantity, int limit)
	{
		var f = new Fixture(limit);
		var menu = TestObjectFactory.CreateUninitialized<RestaurantMenuItem>();
		var result = f.Create(quantity, RestaurantOrderType.DineIn, menu);
		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Message, "menu item is not currently active");
	}

	[TestMethod]
	public void TakeStockForOrder_UnsafePersistedQuantity_FailsBeforeReadingStock()
	{
		var f = new Fixture();
		var order = f.Add(int.MaxValue);
		var items = (IEnumerable<MudSharp.GameItems.IGameItem>)typeof(Restaurant)
			.GetMethod("TakeStockForOrder", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(f.Restaurant, [order, null])!;
		Assert.AreEqual(0, items.Count());
		Assert.AreEqual(RestaurantOrderStatus.Failed, order.Status);
		Assert.AreEqual(20m, order.AmountPaid);
	}

	[TestMethod]
	public void CreateOrder_AlreadyHasMaximumUnservedQuantity_RefusesAnotherOrder()
	{
		var f = new Fixture();
		f.Add(RestaurantServiceRules.MaximumSafeQuantity);
		var result = f.Create(1, RestaurantOrderType.DineIn);
		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Message, "Finish or cancel existing orders");
		Assert.AreEqual(1, f.Orders.Count);
	}

	[DataTestMethod]
	[DataRow(int.MaxValue)]
	[DataRow(0)]
	public void RestaurantHeartbeat_UnsafePersistedQuantity_FailsForRecoveryBeforeAnyCreation(int quantity)
	{
		var f = new Fixture();
		var order = f.Add(quantity);
		typeof(Restaurant).GetMethod("RestaurantHeartbeat", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(f.Restaurant, null);
		Assert.AreEqual(RestaurantOrderStatus.Failed, order.Status);
		Assert.AreEqual(20m, order.AmountPaid);
		StringAssert.Contains(order.OperationalNotes, "manager recovery");
		Assert.IsFalse(f.World.Invocations.Any(x => x.Method.Name == "Add"));
	}

	[TestMethod]
	public void EstimateWait_HugeDurationsAndQueue_SaturatesWithoutOverflowing()
	{
		var wait = RestaurantServiceRules.EstimateWait(TimeSpan.MaxValue, TimeSpan.MaxValue,
			int.MaxValue, TimeSpan.MaxValue, int.MaxValue);
		Assert.AreEqual(TimeSpan.MaxValue, wait);
		Assert.AreEqual(TimeSpan.MaxValue, RestaurantServiceRules.PreparationTime(TimeSpan.MaxValue, [TimeSpan.FromTicks(1)]));
		var now = new DateTime(2026, 10, 9, 0, 0, 0, DateTimeKind.Utc);
		var ready = RestaurantServiceRules.ExpectedReadyAt(now, wait);
		Assert.AreEqual(DateTime.MaxValue.Ticks, ready.Ticks);
		Assert.AreEqual(DateTimeKind.Utc, ready.Kind);
	}
}
