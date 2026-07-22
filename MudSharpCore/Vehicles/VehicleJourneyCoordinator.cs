#nullable enable

using System.Runtime.CompilerServices;
using MudSharp.Framework;
using MudSharp.TimeAndDate;

namespace MudSharp.Vehicles;

/// <summary>
/// Coordinates durable service journeys while delegating compiled leg execution to the
/// independently replaceable physical RouteCell movement strategy.
/// </summary>
public sealed class VehicleJourneyCoordinator : IVehicleJourneyCoordinator, IVehicleJourneyOperations
{
	private static readonly ConditionalWeakTable<IFuturemud, VehicleJourneyCoordinator> Coordinators = new();
	private readonly IFuturemud _gameworld;
	private readonly IVehicleDockingService _dockingService;
	private IVehicleRouteLegExecutor _legExecutor;

	public VehicleJourneyCoordinator(IFuturemud gameworld, IVehicleRouteLegExecutor? legExecutor = null,
		IVehicleDockingService? dockingService = null)
	{
		_gameworld = gameworld;
		_legExecutor = legExecutor ?? new UnavailableVehicleRouteLegExecutor();
		_dockingService = dockingService ?? new VehicleDockingService();
	}

	public static VehicleJourneyCoordinator For(IFuturemud gameworld)
	{
		return Coordinators.GetValue(gameworld, x => new VehicleJourneyCoordinator(x));
	}

	public void SetLegExecutor(IVehicleRouteLegExecutor executor)
	{
		_legExecutor = executor ?? throw new ArgumentNullException(nameof(executor));
	}

	public IVehicleJourney CreateJourney(IVehicleService service, MudDateTime scheduledDeparture)
	{
		var journey = CreateJourneyWithoutRegistration(service, scheduledDeparture);
		RegisterJourney(journey);
		return journey;
	}

	internal VehicleJourney CreateJourneyWithoutRegistration(IVehicleService service, MudDateTime scheduledDeparture)
	{
		return new VehicleJourney(_gameworld, service, scheduledDeparture);
	}

	internal void RegisterJourney(VehicleJourney journey)
	{
		_gameworld.Add(journey);
	}

	public void StartOrResume(IVehicleJourney journey)
	{
		if (journey is VehicleJourney concrete)
		{
			concrete.StartOrResume(this);
		}
	}

	public void Cancel(IVehicleJourney journey, string reason)
	{
		if (journey is VehicleJourney concrete)
		{
			concrete.Cancel(this, reason);
		}
	}

	public void Resume(IVehicleJourney journey)
	{
		StartOrResume(journey);
	}

	public bool TryOpenBoarding(IVehicleJourney journey, out string reason)
	{
		if (journey.Vehicle.Prototype.Scale != VehicleScale.RoomScale)
		{
			reason = string.Empty;
			return true;
		}

		var stop = journey.CurrentStop;
		if (stop is null)
		{
			reason = "The journey has no current stop at which to open boarding.";
			return false;
		}
		if (journey.Vehicle.Location != stop.Location.Cell || journey.Vehicle.RoomLayer != stop.Location.Layer)
		{
			reason = "The room-scale vehicle is not at the authored stop location and layer.";
			return false;
		}

		var routeStop = stop.Location.Cell.RouteDefinition is not null;
		if (routeStop &&
			(!journey.Vehicle.RoutePositionMetres.HasValue || !stop.Location.RoutePositionMetres.HasValue))
		{
			reason = "The room-scale vehicle does not have a valid coordinate at the authored RouteCell stop.";
			return false;
		}

		var opened = false;
		var failures = new List<string>();
		foreach (var binding in stop.PlatformBindings)
		{
			if (routeStop &&
				Math.Abs(journey.Vehicle.RoutePositionMetres!.Value - stop.Location.RoutePositionMetres!.Value) >
			    binding.DockingToleranceMetres)
			{
				continue;
			}
			var accessPoint = journey.Vehicle.AccessPoints
				.FirstOrDefault(x => x.Prototype.Id == binding.AccessPoint.Id);
			if (accessPoint is null)
			{
				failures.Add($"Platform binding #{binding.Id:N0} references an access point that is not present on the live vehicle.");
				continue;
			}
			if (accessPoint.IsDisabled)
			{
				failures.Add($"The {accessPoint.Name} access point is disabled.");
				continue;
			}
			if (accessPoint.IsLocked)
			{
				failures.Add($"The {accessPoint.Name} access point is locked.");
				continue;
			}
			if (!accessPoint.IsOpen && journey.Service.OperatorMode == VehicleServiceOperatorMode.Automatic)
			{
				accessPoint.SetOpen(true);
			}
			if (!accessPoint.IsOpen)
			{
				failures.Add(journey.Service.OperatorMode == VehicleServiceOperatorMode.Automatic
					? $"The automatic service could not open the {accessPoint.Name} access point."
					: $"The {accessPoint.Name} access point must be opened by the onboard operator before boarding can begin.");
				continue;
			}
			if (!_dockingService.CanDock(journey.Vehicle, accessPoint, binding.PlatformCell,
				    journey.Vehicle.RoomLayer, stop, out var dockingReason))
			{
				failures.Add(dockingReason);
				continue;
			}
			var docking = journey.Vehicle.Dockings
				.FirstOrDefault(x => x.AccessPoint.Id == accessPoint.Id && x.Stop?.Id == stop.Id) ??
				_dockingService.Dock(journey.Vehicle, accessPoint, binding.PlatformCell,
					journey.Vehicle.RoomLayer, stop);
			_dockingService.SetBoardingOpen(docking, true);
			opened = true;
		}

		if (opened)
		{
			reason = string.Empty;
			return true;
		}
		reason = failures.FirstOrDefault() ??
		         "No authored platform binding is within docking tolerance for this room-scale vehicle.";
		return false;
	}

	public bool TryCloseBoarding(IVehicleJourney journey, out string reason)
	{
		foreach (var docking in journey.Vehicle.Dockings
			.Where(x => x.State == VehicleDockingState.BoardingOpen)
			.ToList())
		{
			_dockingService.SetBoardingOpen(docking, false);
		}

		if (journey.Vehicle.Prototype.Scale == VehicleScale.RoomScale &&
		    journey.Service.OperatorMode == VehicleServiceOperatorMode.Automatic)
		{
			var accessPointPrototypeIds = journey.CurrentStop?.PlatformBindings
				.Select(x => x.AccessPoint.Id)
				.ToHashSet() ?? [];
			foreach (var accessPoint in journey.Vehicle.AccessPoints
				         .Where(x => accessPointPrototypeIds.Contains(x.Prototype.Id)))
			{
				if (accessPoint.IsOpen)
				{
					accessPoint.SetOpen(false);
				}
				if (accessPoint.IsOpen)
				{
					reason = $"The automatic service could not close the {accessPoint.Name} access point for departure.";
					return false;
				}
			}
		}

		reason = string.Empty;
		return true;
	}

	public bool TryBeginLeg(IVehicleJourney journey, IVehicleRouteLeg leg,
		Action<VehicleJourneyLegResult> completion, out string reason)
	{
		return _legExecutor.TryBeginLeg(journey, leg, completion, out reason);
	}

	void IVehicleJourneyOperations.Cancel(IVehicleJourney journey)
	{
		TryCloseBoarding(journey, out _);
		_legExecutor.Cancel(journey);
	}

	private sealed class UnavailableVehicleRouteLegExecutor : IVehicleRouteLegExecutor
	{
		public bool TryBeginLeg(IVehicleJourney journey, IVehicleRouteLeg leg,
			Action<VehicleJourneyLegResult> completion, out string reason)
		{
			reason = "No physical Route vehicle leg executor is registered.";
			return false;
		}

		public void Cancel(IVehicleJourney journey)
		{
		}
	}
}
