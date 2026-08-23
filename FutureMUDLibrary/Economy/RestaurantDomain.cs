using System;

#nullable enable

namespace MudSharp.Economy;

/// <summary>
/// The purpose for which a cell belongs to a restaurant. Service and internal cells are both
/// within the restaurant boundary; kitchen cells are also a useful builder-facing distinction.
/// </summary>
[Flags]
public enum RestaurantStorageRole
{
	None = 0,
	Ingredients = 1,
	Tools = 2,
	Servingware = 4,
	TakeawayContainers = 8,
	TakeawayBags = 16
}

public enum RestaurantCellRole
{
	Service,
	Internal,
	Kitchen
}

/// <summary>
/// How a restaurant menu item is turned into the item served to a customer.
/// </summary>
public enum RestaurantFulfilmentMode
{
	BringUnaltered,
	OpenAndBring,
	CraftAndBring,
	CraftAndPlate,
	PackageTakeaway
}

public enum RestaurantOrderType
{
	DineIn,
	Takeaway
}

public enum RestaurantOrderStatus
{
	Queued,
	Preparing,
	ReadyForService,
	Served,
	Cancelled,
	Failed,
	Refunded
}

public enum RestaurantTableSessionStatus
{
	Active,
	AbandonmentPending,
	Abandoned,
	Closed,
	OrderingClosed
}

/// <summary>
/// The role of a game item which was produced or consumed while fulfilling a restaurant order.
/// Keeping these separate makes the audit trail useful even if an item is later destroyed.
/// </summary>
public enum RestaurantOrderItemRole
{
	Product,
	ServingContainer,
	TakeawayContainer,
	TakeawayBag
}

/// <summary>
/// The observable service moments which a restaurant builder may theme with a custom emote.
/// The employee remains the emote's actor; <c>$0</c>, <c>$1</c> and <c>$2</c> refer to the
/// supplied item, recipient and table where those concepts apply to a particular moment.
/// </summary>
public enum RestaurantServiceEmoteType
{
	ChefStart,
	ChefOpen,
	ChefPlate,
	ChefReady,
	ServerServe,
	ServerClear,
	ServerReturn
}

/// <summary>
/// Defaults and parser-friendly names for restaurant service presentation. Keeping these here
/// makes the public restaurant contract, runtime defaults and builder surface agree.
/// </summary>
public static class RestaurantServiceEmotes
{
	public const string DefaultChefStart = "@ begin|begins preparing $0.";
	public const string DefaultChefOpen = "@ open|opens $0 for service.";
	public const string DefaultChefPlate = "@ plate|plates $0 on $1.";
	public const string DefaultChefReady = "@ finish|finishes preparing $0 for service.";
	public const string DefaultServerServe = "@ place|places $0 before $1 on $2.";
	public const string DefaultServerClear = "@ clear|clears $0 from $1.";
	public const string DefaultServerReturn = "@ put|puts $0 aside in the kitchen.";

	public static string DefaultFor(RestaurantServiceEmoteType type)
	{
		return type switch
		{
			RestaurantServiceEmoteType.ChefStart => DefaultChefStart,
			RestaurantServiceEmoteType.ChefOpen => DefaultChefOpen,
			RestaurantServiceEmoteType.ChefPlate => DefaultChefPlate,
			RestaurantServiceEmoteType.ChefReady => DefaultChefReady,
			RestaurantServiceEmoteType.ServerServe => DefaultServerServe,
			RestaurantServiceEmoteType.ServerClear => DefaultServerClear,
			RestaurantServiceEmoteType.ServerReturn => DefaultServerReturn,
			_ => DefaultChefStart
		};
	}

	public static string Normalize(RestaurantServiceEmoteType type, string? text)
	{
		return string.IsNullOrWhiteSpace(text) ? DefaultFor(type) : text.Trim();
	}

	public static bool TryParse(string? text, out RestaurantServiceEmoteType type)
	{
		switch (text?.Trim().ToLowerInvariant())
		{
			case "chefstart":
			case "start":
				type = RestaurantServiceEmoteType.ChefStart;
				return true;
			case "chefopen":
			case "open":
				type = RestaurantServiceEmoteType.ChefOpen;
				return true;
			case "chefplate":
			case "plate":
				type = RestaurantServiceEmoteType.ChefPlate;
				return true;
			case "chefready":
			case "ready":
				type = RestaurantServiceEmoteType.ChefReady;
				return true;
			case "serverserve":
			case "serve":
				type = RestaurantServiceEmoteType.ServerServe;
				return true;
			case "serverclear":
			case "clear":
				type = RestaurantServiceEmoteType.ServerClear;
				return true;
			case "serverreturn":
			case "return":
				type = RestaurantServiceEmoteType.ServerReturn;
				return true;
			default:
				type = default;
				return false;
		}
	}

	public static string Describe(RestaurantServiceEmoteType type)
	{
		return type switch
		{
			RestaurantServiceEmoteType.ChefStart => "Chef Start",
			RestaurantServiceEmoteType.ChefOpen => "Chef Open",
			RestaurantServiceEmoteType.ChefPlate => "Chef Plate",
			RestaurantServiceEmoteType.ChefReady => "Chef Ready",
			RestaurantServiceEmoteType.ServerServe => "Server Serve",
			RestaurantServiceEmoteType.ServerClear => "Server Clear",
			RestaurantServiceEmoteType.ServerReturn => "Server Return",
			_ => type.ToString()
		};
	}
}

public readonly record struct RestaurantOperationResult(bool Success, string Message)
{
	public static RestaurantOperationResult Succeed(string message) => new(true, message);
	public static RestaurantOperationResult Fail(string message) => new(false, message);
}
