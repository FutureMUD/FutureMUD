#nullable enable

using MudSharp.Form.Shape;
using MudSharp.GameItems;

namespace MudSharp.Construction;

/// <summary>
/// Source-aware placement helpers for top-level game items.
/// </summary>
public static class GameItemSpatialPlacementExtensions
{
	/// <summary>
	/// Inserts an item at a previously captured exact spatial location. This is used when the
	/// original source may move or be deleted before the replacement or released item is inserted.
	/// </summary>
	public static void InsertAtSpatialLocation(
		this IGameItem item,
		SpatialLocation location,
		bool newStack = false,
		IRouteSpatialService? spatialService = null)
	{
		ArgumentNullException.ThrowIfNull(item);
		var service = spatialService ?? RouteSpatialService.Instance;
		if (!service.TryValidateLocation(location, out var error))
		{
			throw new InvalidOperationException(error);
		}

		item.RoomLayer = location.Layer;
		if (location.Cell.RouteDefinition is not null)
		{
			item.MoveTo(location);
		}

		location.Cell.Insert(item, newStack);
	}

	/// <summary>
	/// Inserts an item into the source's effective cell, inheriting the source's effective
	/// RouteCell coordinate when applicable. Ordinary-cell insertion remains unchanged.
	/// </summary>
	public static void InsertAtSource(
		this IGameItem item,
		ILocateable source,
		bool newStack = false,
		IRouteSpatialService? spatialService = null)
	{
		ArgumentNullException.ThrowIfNull(item);
		ArgumentNullException.ThrowIfNull(source);

		// Keep the legacy ordinary-cell path completely independent of the optional spatial
		// contract. This is important for older ILocateable implementations (and test doubles)
		// that expose Location/RoomLayer but rely on the default spatial members.
		var sourceCell = source.Location;
		if (sourceCell is not null && sourceCell.RouteDefinition is null)
		{
			sourceCell.Insert(item, newStack);
			return;
		}

		var service = spatialService ?? RouteSpatialService.Instance;
		var effectiveLocation = service.GetEffectiveLocation(source);
		if (effectiveLocation.Cell is null)
		{
			throw new InvalidOperationException("The placement source does not have a valid cell.");
		}

		if (sourceCell is not null && !ReferenceEquals(sourceCell, effectiveLocation.Cell))
		{
			throw new InvalidOperationException(
				"The placement source's effective spatial cell does not match its reported location.");
		}

		item.InsertAtSpatialLocation(new SpatialLocation(
			effectiveLocation.Cell,
			item.RoomLayer,
			effectiveLocation.RoutePositionMetres), newStack, service);
	}
}
