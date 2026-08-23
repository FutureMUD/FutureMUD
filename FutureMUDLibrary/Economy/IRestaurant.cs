using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Work.Crafts;
using System;
using System.Collections.Generic;

#nullable enable

namespace MudSharp.Economy;

/// <summary>
/// A permanent shop which provides made-to-order service as well as the normal shop economy
/// integration. Restaurants deliberately remain shops so they share tax, cash, till, bank,
/// virtual-cash and employment infrastructure.
/// </summary>
public interface IRestaurant : IPermanentShop
{
	bool AutomatedService { get; set; }
	bool SimulateCrafting { get; set; }
	TimeSpan HandlingTime { get; set; }
	TimeSpan MaximumBatchWait { get; set; }
	TimeSpan TableCleanupInterval { get; set; }
	string ChefStartEmote { get; set; }
	string ChefOpenEmote { get; set; }
	string ChefPlateEmote { get; set; }
	string ChefReadyEmote { get; set; }
	string ServerServeEmote { get; set; }
	string ServerClearEmote { get; set; }
	string ServerReturnEmote { get; set; }
	IEnumerable<ICell> ServiceCells { get; }
	IEnumerable<ICell> InternalCells { get; }
	IEnumerable<ICell> KitchenCells { get; }
	IEnumerable<IGameItem> RestaurantTables { get; }
	IEnumerable<IRestaurantMenuItem> MenuItems { get; }
	IEnumerable<IRestaurantTableSession> TableSessions { get; }
	IEnumerable<IRestaurantOrder> Orders { get; }

	bool IsWithinRestaurant(ICell? cell);
	IRestaurantTableSession? TableSessionFor(IGameItem table);
	IRestaurantTableSession? TableSessionFor(ICharacter character);
	TimeSpan EstimateWait(ICharacter customer, IRestaurantMenuItem menuItem, int quantity = 1);
	string ShowMenu(ICharacter actor);
}

public interface IRestaurantMenuItem : IFrameworkItem, ISaveable
{
	IRestaurant Restaurant { get; }
	IMerchandise Merchandise { get; }
	string Description { get; set; }
	RestaurantFulfilmentMode FulfilmentMode { get; set; }
	bool IsActive { get; set; }
	bool DineInAvailable { get; set; }
	bool TakeawayAvailable { get; set; }
	TimeSpan PreparationTime { get; set; }
	ICraft? Craft { get; set; }
	IGameItemProto? ServingContainerPrototype { get; set; }
	IGameItemProto? TakeawayContainerPrototype { get; set; }
	IGameItemProto? TakeawayBagPrototype { get; set; }
	bool IsValid(out string reason);
}

public interface IRestaurantTableSession : IFrameworkItem, ISaveable
{
	IRestaurant Restaurant { get; }
	long TableGameItemId { get; }
	RestaurantTableSessionStatus Status { get; }
	DateTime CreatedAtUtc { get; }
	IEnumerable<IRestaurantTableParticipant> Participants { get; }
	IEnumerable<IRestaurantOrder> Orders { get; }
	bool HasAcceptedParticipant(long characterId);
}

public interface IRestaurantTableParticipant : IFrameworkItem, ISaveable
{
	IRestaurantTableSession Session { get; }
	long CharacterId { get; }
	string CharacterName { get; }
	bool Accepted { get; }
	DateTime JoinedAtUtc { get; }
	DateTime? LeftAtUtc { get; }
}

public interface IRestaurantOrder : IFrameworkItem, ISaveable
{
	IRestaurant Restaurant { get; }
	IRestaurantTableSession? TableSession { get; }
	IRestaurantMenuItem MenuItem { get; }
	RestaurantOrderType OrderType { get; }
	RestaurantOrderStatus Status { get; }
	long OrdererCharacterId { get; }
	string OrdererCharacterName { get; }
	long RecipientCharacterId { get; }
	string RecipientCharacterName { get; }
	int Quantity { get; }
	decimal Price { get; }
	decimal AmountPaid { get; }
	decimal OutstandingBalance { get; }
	DateTime CreatedAtUtc { get; }
	IEnumerable<IRestaurantPayment> Payments { get; }
	IEnumerable<IRestaurantOrderItem> ProducedItems { get; }
}

public interface IRestaurantPayment : IFrameworkItem, ISaveable
{
	IRestaurantOrder Order { get; }
	long PayerCharacterId { get; }
	string PayerCharacterName { get; }
	decimal Amount { get; }
	bool IsRefund { get; }
	string PaymentMethod { get; }
	DateTime PaidAtUtc { get; }
}

public interface IRestaurantOrderItem : IFrameworkItem, ISaveable
{
	IRestaurantOrder Order { get; }
	long? GameItemId { get; }
	RestaurantOrderItemRole Role { get; }
	bool Delivered { get; }
}
