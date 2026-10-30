#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Functions.GameItem;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Inventory.Size;
using MudSharp.GameItems.Prototypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PurchaseStoredItemFutureProgTests
{
	[TestMethod]
	public void Purchase_NegativePriceRefusesBeforeInspectingStockOrCommitting()
	{
		var buyerBody = new Mock<IBody>(MockBehavior.Strict);
		var sellerBody = new Mock<IBody>(MockBehavior.Strict);
		var buyer = new Mock<ICharacter>();
		var seller = new Mock<ICharacter>();
		buyer.SetupGet(x => x.Body).Returns(buyerBody.Object);
		seller.SetupGet(x => x.Body).Returns(sellerBody.Object);
		var nativeItem = (GameItem)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(GameItem));
		var source = (Room)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(Room));
		var function = new PurchaseStoredItemFunction([
			Constant(buyer.Object, ProgVariableTypes.Character),
			Constant(seller.Object, ProgVariableTypes.Character),
			Constant(nativeItem, ProgVariableTypes.Item),
			Constant(source, ProgVariableTypes.Location),
			Constant(Mock.Of<ICurrency>(), ProgVariableTypes.Currency),
			new ConstantFunction(new NumberVariable(-1m)),
			Constant("2.5", ProgVariableTypes.Text)
		]);

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.IsFalse((bool)function.Result!.GetObject);
		buyerBody.VerifyNoOtherCalls();
		sellerBody.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void Prepare_RefusesReplacedPhysicalBodyWithoutPublishingFitOrLocation()
	{
		var source = NativeRoom();
		var destination = NativeRoom();
		var prototype = (IGameItemProto)TestObjectFactory.CreateUninitialized<GameItemProto>();
		var item = (GameItem)RuntimeHelpers.GetUninitializedObject(typeof(GameItem));
		Set(item, "_components", new List<IGameItemComponent>());
		Set(item, "EffectHandler", new EffectHandler(item));
		Set(item, "Prototype", prototype);
		item.SetPreparedVehicleLocation(new SpatialLocation(source, RoomLayer.GroundLevel, null));
		source.SetPreparedCurrencyRoomMembership(item, true);
		var wearablePrototype = (WearableGameItemComponentProto)TestObjectFactory.CreateUninitialized<WearableGameItemComponentProto>();
		var wearable = (WearableGameItemComponent)RuntimeHelpers.GetUninitializedObject(typeof(WearableGameItemComponent));
		Set(wearable, "_prototype", wearablePrototype);
		var originalFit = WearableFit.ForScalar(2.5);
		Set(wearable, "<Fit>k__BackingField", originalFit);
		var holdable = (HoldableGameItemComponent)RuntimeHelpers.GetUninitializedObject(typeof(HoldableGameItemComponent));
		var buyer = new Mock<ICharacter>();
		var seller = new Mock<ICharacter>();
		var buyerBody = new Mock<IBody>();
		var sellerBody = new Mock<IBody>();
		buyer.SetupGet(x => x.Body).Returns(buyerBody.Object);
		seller.SetupGet(x => x.Body).Returns(sellerBody.Object);
		buyerBody.SetupGet(x => x.Actor).Returns((ICharacter?)null);
		sellerBody.SetupGet(x => x.Actor).Returns(seller.Object);
		var participant = new PreparedStoredItemDelivery(buyer.Object, seller.Object, item, wearable,
			source, destination, buyerBody.Object, sellerBody.Object, holdable, 0m,
			WearableFit.ForScalar(3.5));

		Assert.IsFalse(participant.Prepare());
		Assert.IsFalse(participant.IsCurrent);
		Assert.AreEqual(originalFit, wearable.Fit);
		Assert.AreSame(source, item.DirectLocation);
		CollectionAssert.AreEqual(new IGameItem[] { item }, source.GameItems.ToArray());
		Assert.IsFalse(destination.GameItems.Any());
	}

	private static IFunction Constant(object value, ProgVariableTypes type) =>
		new ConstantFunction(new ValueVariable(value, type));

	private static Room NativeRoom()
	{
		var room = (Room)RuntimeHelpers.GetUninitializedObject(typeof(Room));
		Set(room, "_gameItems", new List<IGameItem>());
		Set(room, "_isCombatSimulationRoom", true);
		return room;
	}

	private static void Set(object target, string name, object value)
	{
		for (var type = target.GetType(); type is not null; type = type.BaseType)
			if (type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly) is { } field)
			{
				field.SetValue(target, value);
				return;
			}
			else if (type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly) is { } property)
			{
				property.SetValue(target, value);
				return;
			}
		Assert.Fail($"Missing fixture member {name}.");
	}

	private sealed record ValueVariable(object Value, ProgVariableTypes Type) : IProgVariable
	{
		public object GetObject => Value;
		public IProgVariable GetProperty(string property) => throw new NotSupportedException();
	}
}
