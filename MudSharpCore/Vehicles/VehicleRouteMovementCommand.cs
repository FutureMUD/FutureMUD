#nullable enable

using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.PerceptionEngine;

namespace MudSharp.Vehicles;

internal static class VehicleRouteMovementCommand
{
	private static readonly string[] RouteActions =
	[
		"forward", "forwards", "positive", "backward", "backwards", "negative", "to", "route", "stop"
	];

	public static bool IsRouteAction(string rawInput)
	{
		return RouteActions.Contains(new StringStack(rawInput).PeekSpeech(), StringComparer.OrdinalIgnoreCase);
	}

	public static VehicleMovementCommandResult Execute(IVehicle vehicle, ICharacter actor, string rawInput)
	{
		var command = new StringStack(rawInput);
		var action = command.PopSpeech().ToLowerInvariant();
		if (action == "stop")
		{
			if (!RouteVehicleMovementStrategy.Instance.TryStop(vehicle, actor, out var stopReason))
			{
				actor.OutputHandler.Send(stopReason);
				return VehicleMovementCommandResult.Failed;
			}

			return VehicleMovementCommandResult.StartedOrQueued;
		}

		if (action == "route")
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which approved vehicle route do you want to drive?");
				return VehicleMovementCommandResult.Failed;
			}

			var route = actor.Gameworld.VehicleRoutes.GetByIdOrName(command.SafeRemainingArgument);
			if (route is null || route.Status != RevisionStatus.Current)
			{
				actor.OutputHandler.Send("There is no approved current vehicle route like that.");
				return VehicleMovementCommandResult.Failed;
			}

			if (!RouteVehicleMovementStrategy.Instance.TryBeginManualRoute(
					vehicle,
					actor,
					route,
					result => actor.OutputHandler.Send(result.Succeeded
						? $"{vehicle.Name.ColourName()} completes {route.Name.ColourName()}."
						: result.Reason),
					out var routeReason))
			{
				actor.OutputHandler.Send(routeReason);
				return VehicleMovementCommandResult.Failed;
			}

			return VehicleMovementCommandResult.StartedOrQueued;
		}

		var routeDefinition = vehicle.Location?.RouteDefinition;
		var origin = vehicle.ExteriorItem is null
			? vehicle.SpatialLocation
			: RouteSpatialService.Instance.GetEffectiveLocation(vehicle.ExteriorItem);
		if (routeDefinition is null || !origin.RoutePositionMetres.HasValue)
		{
			actor.OutputHandler.Send("That vehicle can only drive longitudinally while positioned in a RouteCell.");
			return VehicleMovementCommandResult.Failed;
		}

		double destination;
		double? targetMinimum = null;
		double? targetMaximum = null;
		long? selectedExitId = null;
		switch (action)
		{
			case "forward":
			case "forwards":
			case "positive":
				destination = routeDefinition.LengthMetres;
				if (!command.IsFinished)
				{
					if (!TryParseDistance(actor, command.SafeRemainingArgument, out var distance) || distance <= 0.0)
					{
						actor.OutputHandler.Send("How far forward do you want to drive?");
						return VehicleMovementCommandResult.Failed;
					}

					destination = Math.Min(routeDefinition.LengthMetres,
						origin.RoutePositionMetres.Value + distance);
				}
				break;
			case "backward":
			case "backwards":
			case "negative":
				destination = 0.0;
				if (!command.IsFinished)
				{
					if (!TryParseDistance(actor, command.SafeRemainingArgument, out var distance) || distance <= 0.0)
					{
						actor.OutputHandler.Send("How far backward do you want to drive?");
						return VehicleMovementCommandResult.Failed;
					}

					destination = Math.Max(0.0, origin.RoutePositionMetres.Value - distance);
				}
				break;
			case "to":
				if (command.IsFinished || !TryResolveDestination(actor, vehicle, origin,
						command.SafeRemainingArgument, out destination, out targetMinimum,
						out targetMaximum, out selectedExitId))
				{
					actor.OutputHandler.Send("Specify a RouteCell coordinate, landmark, visible exit, or route stop.");
					return VehicleMovementCommandResult.Failed;
				}
				break;
			default:
				actor.OutputHandler.Send("Use drive forward, drive backward, drive to, drive route, or drive stop.");
				return VehicleMovementCommandResult.Failed;
		}

		if (!RouteVehicleMovementStrategy.Instance.TryBeginManualMove(
				vehicle,
				actor,
				destination,
				out var reason,
				targetMinimum,
				targetMaximum,
				selectedExitId))
		{
			actor.OutputHandler.Send(reason);
			return VehicleMovementCommandResult.Failed;
		}

		return VehicleMovementCommandResult.StartedOrQueued;
	}

	private static bool TryResolveDestination(
		ICharacter actor,
		IVehicle vehicle,
		SpatialLocation origin,
		string text,
		out double destination,
		out double? targetMinimum,
		out double? targetMaximum,
		out long? selectedExitId)
	{
		targetMinimum = null;
		targetMaximum = null;
		selectedExitId = null;
		var route = origin.Cell.RouteDefinition!;
		if (TryParseDistance(actor, text, out destination) && destination >= 0.0 &&
			destination <= route.LengthMetres)
		{
			return true;
		}

		var landmark = route.Landmarks.FirstOrDefault(x =>
			x.Name.EqualTo(text) || x.HasKeyword(text, actor, abbreviated: true));
		if (landmark is not null)
		{
			destination = landmark.PositionMetres;
			return true;
		}

		var anchor = route.ExitAnchors
			.Where(x => vehicle.Location.IsExitVisible(actor, x.Exit, PerceptionTypes.DirectVisual))
			.FirstOrDefault(x => x.Exit.HasKeyword(text, actor, abbreviated: true) ||
			                     x.Exit.OutboundDirection.Describe().EqualTo(text));
		if (anchor is not null)
		{
			destination = Math.Clamp(origin.RoutePositionMetres!.Value,
				anchor.MinimumPositionMetres, anchor.MaximumPositionMetres);
			targetMinimum = anchor.MinimumPositionMetres;
			targetMaximum = anchor.MaximumPositionMetres;
			selectedExitId = anchor.Exit.Exit.Id;
			return true;
		}

		var stops = actor.Gameworld.VehicleRoutes
			.GetAllByStatus(RevisionStatus.Current)
			.SelectMany(x => x.Stops)
			.Where(x => ReferenceEquals(x.Location.Cell, origin.Cell) && x.Location.Layer == origin.Layer &&
			            x.Location.RoutePositionMetres.HasValue)
			.ToList();
		var stop = text.EqualTo("stop")
			? stops.OrderBy(x => Math.Abs(x.Location.RoutePositionMetres!.Value -
			                              origin.RoutePositionMetres!.Value)).FirstOrDefault()
			: stops.FirstOrDefault(x => x.Name.EqualTo(text) || x.Name.StartsWith(text,
				StringComparison.InvariantCultureIgnoreCase));
		if (stop is not null)
		{
			destination = stop.Location.RoutePositionMetres!.Value;
			return true;
		}

		destination = 0.0;
		return false;
	}

	private static bool TryParseDistance(ICharacter actor, string text, out double metres)
	{
		metres = 0.0;
		if (!actor.Gameworld.UnitManager.TryGetBaseUnits(text, UnitType.Length, actor, out var baseUnits))
		{
			return false;
		}

		metres = baseUnits * actor.Gameworld.UnitManager.BaseHeightToMetres;
		return double.IsFinite(metres);
	}
}
