#nullable enable

using System.Globalization;
using System.IO;
using System.Xml.Linq;
using MudSharp.Database;

namespace MudSharp.Construction;

internal readonly record struct RouteCellMutationActor(
	long CharacterId,
	long CharacterInstanceId,
	bool IsPrimaryInstance);

internal readonly record struct RouteCellPersistedOccupancy(
	bool HasOtherCharacters,
	bool HasTopLevelItems,
	bool HasVehicles,
	bool HasProjects,
	bool HasTracks,
	bool HasPointSurfaceLiquid);

internal readonly record struct RouteCellPersistedLengthBlockers(
	bool HasCharactersBeyondLength,
	bool HasTopLevelItemsBeyondLength,
	bool HasVehiclesBeyondLength,
	bool HasProjectsBeyondLength,
	bool HasTracksBeyondLength,
	bool HasPointSurfaceLiquidBeyondLength);

/// <summary>
/// Performs the persisted half of RouteCell geometry mutation validation. Builder commands use
/// these queries inside the same serializable transaction as the geometry mutation so dormant
/// world presences cannot be stranded between validation and commit.
/// </summary>
internal static class RouteCellMutationSafety
{
	internal static RouteCellPersistedOccupancy InspectOccupancy(
		FuturemudDatabaseContext context,
		long cellId,
		RouteCellMutationActor actor)
	{
		ArgumentNullException.ThrowIfNull(context);

		var hasOtherCharacters = context.Characters.Any(x =>
			x.Location == cellId &&
			(!actor.IsPrimaryInstance || x.Id != actor.CharacterId)) ||
			context.CharacterInstances.Any(x =>
				x.LocationId == cellId && x.Id != actor.CharacterInstanceId);
		var surfaceLiquidData = context.Cells
			.Where(x => x.Id == cellId)
			.Select(x => x.SurfaceLiquidData)
			.SingleOrDefault();

		return new RouteCellPersistedOccupancy(
			hasOtherCharacters,
			context.CellsGameItems.Any(x => x.CellId == cellId),
			context.Vehicles.Any(x => x.CurrentCellId == cellId),
			context.ActiveProjects.Any(x => x.CellId == cellId),
			context.Tracks.Any(x => x.CellId == cellId),
			PointSurfaceLiquidPositions(surfaceLiquidData).Count > 0);
	}

	internal static RouteCellPersistedLengthBlockers InspectLength(
		FuturemudDatabaseContext context,
		long cellId,
		double maximumPositionMetres)
	{
		ArgumentNullException.ThrowIfNull(context);
		if (!double.IsFinite(maximumPositionMetres) || maximumPositionMetres < 0.0)
		{
			throw new ArgumentOutOfRangeException(nameof(maximumPositionMetres));
		}

		var maximum = Math.Round((decimal)maximumPositionMetres, 3, MidpointRounding.AwayFromZero);
		var surfaceLiquidData = context.Cells
			.Where(x => x.Id == cellId)
			.Select(x => x.SurfaceLiquidData)
			.SingleOrDefault();

		return new RouteCellPersistedLengthBlockers(
			context.Characters.Any(x => x.Location == cellId && x.RoutePosition > maximum) ||
			context.CharacterInstances.Any(x => x.LocationId == cellId && x.RoutePosition > maximum),
			context.CellsGameItems
				.Where(x => x.CellId == cellId)
				.Join(
					context.GameItems,
					x => x.GameItemId,
					x => x.Id,
					(_, item) => item.RoutePosition)
				.Any(x => x > maximum),
			context.Vehicles.Any(x => x.CurrentCellId == cellId && x.CurrentRoutePosition > maximum),
			context.ActiveProjects.Any(x => x.CellId == cellId && x.RoutePosition > maximum),
			context.Tracks.Any(x => x.CellId == cellId && x.RoutePosition > maximum),
			PointSurfaceLiquidPositions(surfaceLiquidData).Any(x => x > maximumPositionMetres));
	}

	internal static void PersistActorSpatialState(
		FuturemudDatabaseContext context,
		RouteCellMutationActor actor,
		long cellId,
		RoomLayer layer,
		double? routePositionMetres)
	{
		ArgumentNullException.ThrowIfNull(context);
		decimal? persistedPosition = routePositionMetres.HasValue
			? Math.Round((decimal)routePositionMetres.Value, 3, MidpointRounding.AwayFromZero)
			: null;

		var instance = context.CharacterInstances.Find(actor.CharacterInstanceId);
		if (instance is null)
		{
			throw new InvalidDataException(
				$"Character instance #{actor.CharacterInstanceId:N0} is missing while committing RouteCell builder spatial state.");
		}

		instance.LocationId = cellId;
		instance.RoomLayer = (int)layer;
		instance.RoutePosition = persistedPosition;

		if (!actor.IsPrimaryInstance)
		{
			return;
		}

		var character = context.Characters.Find(actor.CharacterId);
		if (character is null)
		{
			throw new InvalidDataException(
				$"Character #{actor.CharacterId:N0} is missing while committing primary RouteCell builder spatial state.");
		}

		character.Location = cellId;
		character.RoomLayer = (int)layer;
		character.RoutePosition = persistedPosition;
	}

	internal static IReadOnlyList<double> PointSurfaceLiquidPositions(string? surfaceLiquidData)
	{
		if (string.IsNullOrWhiteSpace(surfaceLiquidData))
		{
			return Array.Empty<double>();
		}

		XElement root;
		try
		{
			root = XElement.Parse(surfaceLiquidData);
		}
		catch (Exception exception) when (exception is System.Xml.XmlException or InvalidOperationException)
		{
			throw new InvalidDataException("The cell has malformed persisted surface-liquid XML.", exception);
		}

		var positions = new List<double>();
		foreach (var attribute in root.Elements("Layer")
			         .Select(x => x.Attribute("position"))
			         .Where(x => x is not null))
		{
			if (!double.TryParse(attribute!.Value, NumberStyles.Float, CultureInfo.InvariantCulture,
					out var position) || !double.IsFinite(position) || position < 0.0)
			{
				throw new InvalidDataException(
					$"The cell has an invalid persisted surface-liquid RouteCell coordinate '{attribute.Value}'.");
			}

			positions.Add(position);
		}

		return positions;
	}
}
