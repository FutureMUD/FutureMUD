#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Form.Shape;
using MudSharp.Framework;

namespace MudSharp.Construction;

/// <summary>
/// Central authority for resolving locations, distance, proximity and portals in linear
/// route rooms. Consumers should use this service instead of interpreting route coordinates
/// directly.
/// </summary>
public interface IRouteSpatialService
{
	SpatialLocation GetEffectiveLocation(ILocateable locateable);

	bool TryValidateLocation(SpatialLocation location, out string error);

	double ClampPosition(IRouteRoomDefinition routeRoom, double positionMetres);

	/// <summary>
	/// Returns exact longitudinal separation when both locations occupy the same route room
	/// and layer; otherwise returns null.
	/// </summary>
	double? GetExactSeparation(SpatialLocation first, SpatialLocation second);

	Proximity GetProximity(ILocateable first, ILocateable second);

	bool CanReach(ILocateable source, ILocateable target, double maximumDistanceMetres);

	IReadOnlyCollection<IPerceivable> GetPerceivablesWithin(
		SpatialLocation origin,
		double maximumDistanceMetres,
		Func<IPerceivable, bool>? predicate = null);

	/// <summary>
	/// Performs the same indexed longitudinal query across every layer in the RouteRoom.
	/// Callers remain responsible for applying the cross-layer minimum proximity rule.
	/// </summary>
	IReadOnlyCollection<IPerceivable> GetPerceivablesWithinAcrossLayers(
		SpatialLocation origin,
		double maximumDistanceMetres,
		Func<IPerceivable, bool>? predicate = null)
	{
		return GetPerceivablesWithin(origin, maximumDistanceMetres, predicate);
	}

	bool TryGetExitAnchor(IRoomExit exit, IRoom routeRoom, out IRouteExitAnchor? anchor);

	/// <summary>
	/// Returns whether an exit is perceptually visible from the locateable's effective position.
	/// In a RouteRoom the closest point in the authored anchor band must also be inside the
	/// supplied longitudinal range. Accessibility is deliberately separate: a visible portal
	/// ahead is not traversable until the locateable reaches its band.
	/// </summary>
	bool IsExitVisible(
		IPerceiver voyeur,
		IRoomExit exit,
		double maximumDistanceMetres,
		PerceptionTypes type = PerceptionTypes.DirectVisual,
		PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None);

	bool IsExitAccessible(ILocateable locateable, IRoomExit exit);

	/// <summary>
	/// Resolves the closest coordinate in an exit's accessible band. Returns null when the
	/// exit has no anchor for the supplied route room.
	/// </summary>
	double? GetNearestAccessiblePosition(SpatialLocation origin, IRoomExit exit);

	/// <summary>
	/// Resolves the effective coordinate inherited from an owning, carrying or containing
	/// locateable. Returns null when neither entity is positioned in the supplied route room.
	/// </summary>
	double? GetInheritedRoutePosition(ILocateable locateable, ILocateable? owner);
}
