#nullable enable

using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Character;

namespace MudSharp.Construction;

/// <summary>
/// Contextual locality queries for call sites that previously enumerated an entire cell.
/// Ordinary cells retain their historical layer/cell semantics; RouteCells query the
/// ordered spatial index around the source's effective lazy coordinate.
/// </summary>
public static class SpatialQueryExtensions
{
	/// <summary>
	/// Returns perceivables close enough for direct physical interaction. Ordinary cells preserve
	/// their historical same-layer behaviour; RouteCells use the configured Immediate threshold.
	/// </summary>
	public static IEnumerable<IPerceivable> PerceivablesInImmediateVicinity(
		this ICell cell,
		ILocateable source,
		bool sameLayerOnly = true)
	{
		ArgumentNullException.ThrowIfNull(cell);
		ArgumentNullException.ThrowIfNull(source);

		if (cell.RouteDefinition is null)
		{
			// Merge the long-standing typed collections with the aggregate projection. Concrete
			// Cells populate both, while legacy implementations and lightweight consumers may
			// intentionally expose only one view.
			var typedPerceivables = sameLayerOnly
				? cell.LayerCharacters(source.RoomLayer)
					.Cast<IPerceivable>()
					.Concat(cell.LayerGameItems(source.RoomLayer))
				: cell.Characters
					.Cast<IPerceivable>()
					.Concat(cell.GameItems);
			var aggregatePerceivables = cell.Perceivables
				.Where(x => !sameLayerOnly || x.RoomLayer == source.RoomLayer);
			return typedPerceivables
				.Concat(aggregatePerceivables)
				.Distinct();
		}

		var configuration = RouteSpatialConfiguration.FromGameworld(
			source is IHaveFuturemud haveFuturemud ? haveFuturemud.Gameworld : null);
		return cell.PerceivablesInSpatialVicinity(
			source,
			sameLayerOnly,
			configuration.ImmediateDistanceMetres);
	}

	public static IEnumerable<ICharacter> CharactersInImmediateVicinity(
		this ICell cell,
		ILocateable source,
		bool sameLayerOnly = true)
	{
		return cell
			.PerceivablesInImmediateVicinity(source, sameLayerOnly)
			.OfType<ICharacter>();
	}

	public static IEnumerable<IGameItem> GameItemsInImmediateVicinity(
		this ICell cell,
		ILocateable source,
		bool sameLayerOnly = true)
	{
		return cell
			.PerceivablesInImmediateVicinity(source, sameLayerOnly)
			.OfType<IGameItem>();
	}

	/// <summary>
	/// Tests whether two locateables share the same longitudinal neighbourhood while deliberately
	/// ignoring layer. Ordinary cells retain their historical raw-cell meaning. This is intended
	/// for deciding whether a layer transition can bring one RouteCell entity toward another.
	/// </summary>
	public static bool SharesLongitudinalVicinityWith(
		this ILocateable source,
		ILocateable target,
		double? maximumDistanceMetres = null)
	{
		ArgumentNullException.ThrowIfNull(source);
		ArgumentNullException.ThrowIfNull(target);
		if (!ReferenceEquals(source.Location, target.Location))
		{
			return false;
		}

		if (source.Location.RouteDefinition is null)
		{
			return true;
		}

		var sourceLocation = RouteSpatialService.Instance.GetEffectiveLocation(source);
		var targetLocation = RouteSpatialService.Instance.GetEffectiveLocation(target);
		if (!sourceLocation.RoutePositionMetres.HasValue || !targetLocation.RoutePositionMetres.HasValue)
		{
			return false;
		}

		var configuration = RouteSpatialConfiguration.FromGameworld(
			source is IHaveFuturemud haveFuturemud ? haveFuturemud.Gameworld : null);
		var maximumDistance = maximumDistanceMetres ?? configuration.ImmediateDistanceMetres;
		return double.IsFinite(maximumDistance) &&
		       maximumDistance >= 0.0 &&
		       Math.Abs(sourceLocation.RoutePositionMetres.Value - targetLocation.RoutePositionMetres.Value) <=
		       maximumDistance;
	}

	public static IEnumerable<IPerceivable> PerceivablesInSpatialVicinity(
		this ICell cell,
		ILocateable source,
		bool sameLayerOnly = true,
		double? maximumDistanceMetres = null)
	{
		ArgumentNullException.ThrowIfNull(cell);
		ArgumentNullException.ThrowIfNull(source);

		if (cell.RouteDefinition is null)
		{
			// ILocation.Perceivables is the convenient concrete Cell projection, but the older
			// contract exposes characters and game items independently. Merge both surfaces so
			// ordinary-cell implementations and existing builders keep their historical results.
			var legacyPerceivables = sameLayerOnly
				? (cell.LayerCharacters(source.RoomLayer) ?? [])
					.Cast<IPerceivable>()
					.Concat((cell.LayerGameItems(source.RoomLayer) ?? []).Cast<IPerceivable>())
				: (cell.Characters ?? [])
					.Cast<IPerceivable>()
					.Concat((cell.GameItems ?? []).Cast<IPerceivable>());
			var projectedPerceivables = (cell.Perceivables ?? [])
				.Where(x => !sameLayerOnly || x.RoomLayer == source.RoomLayer);
			return legacyPerceivables
				.Concat(projectedPerceivables)
				.Distinct();
		}

		var origin = RouteSpatialService.Instance.GetEffectiveLocation(source);
		if (!ReferenceEquals(origin.Cell, cell) || !origin.RoutePositionMetres.HasValue)
		{
			return Array.Empty<IPerceivable>();
		}

		var configuration = RouteSpatialConfiguration.FromGameworld(
			source is IHaveFuturemud haveFuturemud ? haveFuturemud.Gameworld : null);
		configuration.Validate();
		var distance = maximumDistanceMetres ?? configuration.VeryDistantDistanceMetres;
		if (!double.IsFinite(distance) || distance < 0.0)
		{
			throw new ArgumentOutOfRangeException(nameof(maximumDistanceMetres));
		}

		return sameLayerOnly
			? RouteSpatialService.Instance.GetPerceivablesWithin(origin, distance)
			: RouteSpatialService.Instance.GetPerceivablesWithinAcrossLayers(origin, distance);
	}

	public static IEnumerable<ICharacter> CharactersInSpatialVicinity(
		this ICell cell,
		ILocateable source,
		bool sameLayerOnly = true,
		double? maximumDistanceMetres = null)
	{
		return cell
			.PerceivablesInSpatialVicinity(source, sameLayerOnly, maximumDistanceMetres)
			.OfType<ICharacter>();
	}

	public static IEnumerable<IGameItem> GameItemsInSpatialVicinity(
		this ICell cell,
		ILocateable source,
		bool sameLayerOnly = true,
		double? maximumDistanceMetres = null)
	{
		return cell
			.PerceivablesInSpatialVicinity(source, sameLayerOnly, maximumDistanceMetres)
			.OfType<IGameItem>();
	}
}
