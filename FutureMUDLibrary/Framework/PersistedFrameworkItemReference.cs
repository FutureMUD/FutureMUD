using System;

#nullable enable

namespace MudSharp.Framework;

/// <summary>Discriminates current Room IDs from the removed legacy Room identity space.</summary>
public static class PersistedFrameworkItemReference
{
	public const string RoomType = "Room:v2";
	public const string LegacyCellType = "Cell";
	public const string LegacyRoomType = "Room";
	private static string? ComparisonType(string? type) => type switch
	{
		LegacyCellType => RoomType,
		"CellOverlay" => "RoomOverlay",
		"CellOverlayPackage" => "RoomOverlayPackage",
		"RouteCellLandmark" => "RouteRoomLandmark",
		_ => type
	};

	public static bool SamePersistedReferenceType(string? first, string? second) =>
		string.Equals(ComparisonType(first), ComparisonType(second), StringComparison.InvariantCulture);

	public static string? PublicReferenceType(string? type) => type is RoomType or LegacyCellType
		? "Room" : type == LegacyRoomType ? "Legacy Room" : ComparisonType(type);

	public static string GetPersistedReferenceType(this IFrameworkItem item) =>
		item.FrameworkItemType == "Room" ? RoomType : item.FrameworkItemType;

	public static bool MatchesPersistedReferenceType(this IFrameworkItem item, string? type) =>
		item.FrameworkItemType switch
		{
			"Room" => type is LegacyCellType or RoomType,
			"RoomOverlay" => type is "RoomOverlay" or "CellOverlay",
			"RoomOverlayPackage" => type is "RoomOverlayPackage" or "CellOverlayPackage",
			"RouteRoomLandmark" => type is "RouteRoomLandmark" or "RouteCellLandmark",
			_ => string.Equals(item.FrameworkItemType, type, StringComparison.InvariantCulture)
		};
}
