using System;
using System.Collections.Generic;

#nullable enable

namespace MudSharp.Models;

/// <summary>
/// Restaurant-only configuration for a shop. The primary key is also the shop key so that a
/// restaurant remains a normal permanent shop for economic and employment purposes.
/// </summary>
public class Restaurant
{
	public Restaurant()
	{
		Cells = new HashSet<RestaurantCell>();
		Tables = new HashSet<RestaurantTable>();
		MenuItems = new HashSet<RestaurantMenuItem>();
		TableSessions = new HashSet<RestaurantTableSession>();
		Orders = new HashSet<RestaurantOrder>();
	}

	public long ShopId { get; set; }
	public bool AutomatedService { get; set; }
	public bool SimulateCrafting { get; set; }
	public int HandlingSeconds { get; set; }
	public int MaximumBatchWaitSeconds { get; set; }
	public int CleanupIntervalSeconds { get; set; }
	public string ChefStartEmote { get; set; } = string.Empty;
	public string ChefOpenEmote { get; set; } = string.Empty;
	public string ChefPlateEmote { get; set; } = string.Empty;
	public string ChefReadyEmote { get; set; } = string.Empty;
	public string ServerServeEmote { get; set; } = string.Empty;
	public string ServerClearEmote { get; set; } = string.Empty;
	public string ServerReturnEmote { get; set; } = string.Empty;

	public virtual Shop Shop { get; set; } = null!;
	public virtual ICollection<RestaurantCell> Cells { get; set; }
	public virtual ICollection<RestaurantTable> Tables { get; set; }
	public virtual ICollection<RestaurantMenuItem> MenuItems { get; set; }
	public virtual ICollection<RestaurantTableSession> TableSessions { get; set; }
	public virtual ICollection<RestaurantOrder> Orders { get; set; }
}

public class RestaurantCell
{
	public long RestaurantShopId { get; set; }
	public long CellId { get; set; }
	public int Role { get; set; }

	public virtual Restaurant Restaurant { get; set; } = null!;
}

public class RestaurantTable
{
	public long RestaurantShopId { get; set; }
	public long GameItemId { get; set; }

	public virtual Restaurant Restaurant { get; set; } = null!;
}

public class RestaurantMenuItem
{
	public long Id { get; set; }
	public long RestaurantShopId { get; set; }
	public long MerchandiseId { get; set; }
	public string Description { get; set; } = string.Empty;
	public int FulfilmentMode { get; set; }
	public bool IsActive { get; set; }
	public bool DineInAvailable { get; set; }
	public bool TakeawayAvailable { get; set; }
	public int PreparationSeconds { get; set; }
	public long? CraftId { get; set; }
	public int? CraftRevisionNumber { get; set; }
	public long? ServingContainerPrototypeId { get; set; }
	public int? ServingContainerPrototypeRevisionNumber { get; set; }
	public long? TakeawayContainerPrototypeId { get; set; }
	public int? TakeawayContainerPrototypeRevisionNumber { get; set; }
	public long? TakeawayBagPrototypeId { get; set; }
	public int? TakeawayBagPrototypeRevisionNumber { get; set; }
	public int SortOrder { get; set; }

	public virtual Restaurant Restaurant { get; set; } = null!;
	public virtual Merchandise Merchandise { get; set; } = null!;
}

public class RestaurantTableSession
{
	public RestaurantTableSession()
	{
		Participants = new HashSet<RestaurantTableParticipant>();
		Orders = new HashSet<RestaurantOrder>();
	}

	public long Id { get; set; }
	public long RestaurantShopId { get; set; }
	public long TableGameItemId { get; set; }
	public int Status { get; set; }
	public DateTime CreatedAtUtc { get; set; }
	public DateTime LastUpdatedAtUtc { get; set; }
	public DateTime? ClosedAtUtc { get; set; }
	public DateTime? AbandonmentPendingAtUtc { get; set; }
	public bool AbandonmentReported { get; set; }

	public virtual Restaurant Restaurant { get; set; } = null!;
	public virtual ICollection<RestaurantTableParticipant> Participants { get; set; }
	public virtual ICollection<RestaurantOrder> Orders { get; set; }
}

public class RestaurantTableParticipant
{
	public long Id { get; set; }
	public long RestaurantTableSessionId { get; set; }
	public long CharacterId { get; set; }
	public string CharacterName { get; set; } = string.Empty;
	public bool Accepted { get; set; }
	public DateTime JoinedAtUtc { get; set; }
	public DateTime? LeftAtUtc { get; set; }

	public virtual RestaurantTableSession Session { get; set; } = null!;
}

/// <summary>
/// An independently fulfilable restaurant line. A one-item order command produces one line;
/// the table session is the grouping boundary for serving and billing.
/// </summary>
public class RestaurantOrder
{
	public RestaurantOrder()
	{
		Payments = new HashSet<RestaurantPayment>();
		ProducedItems = new HashSet<RestaurantOrderItem>();
	}

	public long Id { get; set; }
	public long RestaurantShopId { get; set; }
	public long? RestaurantTableSessionId { get; set; }
	public long RestaurantMenuItemId { get; set; }
	public int OrderType { get; set; }
	public int Status { get; set; }
	public long OrdererCharacterId { get; set; }
	public string OrdererCharacterName { get; set; } = string.Empty;
	public long RecipientCharacterId { get; set; }
	public string RecipientCharacterName { get; set; } = string.Empty;
	public int Quantity { get; set; }
	public decimal PretaxPrice { get; set; }
	public decimal Tax { get; set; }
	public decimal Price { get; set; }
	public decimal AmountPaid { get; set; }
	public bool RevenueRecognised { get; set; }
	public DateTime CreatedAtUtc { get; set; }
	public DateTime LastUpdatedAtUtc { get; set; }
	public DateTime? ExpectedReadyAtUtc { get; set; }
	public DateTime? ReadyAtUtc { get; set; }
	public DateTime? ServedAtUtc { get; set; }
	public long? PreparedByEmployeeId { get; set; }
	public long? ServedByEmployeeId { get; set; }
	public string OperationalNotes { get; set; } = string.Empty;

	public virtual Restaurant Restaurant { get; set; } = null!;
	public virtual RestaurantTableSession? TableSession { get; set; }
	public virtual RestaurantMenuItem MenuItem { get; set; } = null!;
	public virtual ICollection<RestaurantPayment> Payments { get; set; }
	public virtual ICollection<RestaurantOrderItem> ProducedItems { get; set; }
}

public class RestaurantPayment
{
	public long Id { get; set; }
	public long RestaurantOrderId { get; set; }
	public long PayerCharacterId { get; set; }
	public string PayerCharacterName { get; set; } = string.Empty;
	public decimal Amount { get; set; }
	public bool IsRefund { get; set; }
	public string PaymentMethod { get; set; } = string.Empty;
	public string Reference { get; set; } = string.Empty;
	public DateTime PaidAtUtc { get; set; }

	public virtual RestaurantOrder Order { get; set; } = null!;
}

public class RestaurantOrderItem
{
	public long Id { get; set; }
	public long RestaurantOrderId { get; set; }
	public long? GameItemId { get; set; }
	public int Role { get; set; }
	public bool Delivered { get; set; }
	public DateTime CreatedAtUtc { get; set; }
	public DateTime? DeliveredAtUtc { get; set; }

	public virtual RestaurantOrder Order { get; set; } = null!;
}
