#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Vehicles;

namespace MudSharp.Framework;

public sealed partial class Futuremud
{
	void IFuturemudLoader.LoadVehicleRoutes()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Vehicle Routes#0...");
		var routes = FMDB.Context.VehicleRoutes
			.Include(x => x.EditableItem)
			.Include(x => x.Stops).ThenInclude(x => x.PlatformBindings)
			.Include(x => x.Legs).ThenInclude(x => x.Steps)
			.Include(x => x.TopologyPins)
			.AsNoTracking()
			.ToList();
		foreach (var route in routes)
		{
			_vehicleRoutes.Add(new VehicleRoute(route, this));
		}

		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 {1}.", routes.Count,
			routes.Count == 1 ? "Vehicle Route" : "Vehicle Routes");
	}

	void IFuturemudLoader.LoadVehicleOperations()
	{
		ConsoleUtilities.WriteLine("\nLoading #5Vehicle Services and Journeys#0...");
		var services = FMDB.Context.VehicleServices
			.Include(x => x.Schedule)
			.AsNoTracking()
			.ToList();
		foreach (var service in services)
		{
			_vehicleServices.Add(new VehicleService(service, this));
		}

		var journeys = FMDB.Context.VehicleJourneys
			.Include(x => x.Events)
			.AsNoTracking()
			.ToList();
		foreach (var journey in journeys)
		{
			_vehicleJourneys.Add(new VehicleJourney(journey, this));
		}

		var coordinator = VehicleJourneyCoordinator.For(this);
		coordinator.SetLegExecutor(RouteVehicleMovementStrategy.Instance);
		foreach (var journey in _vehicleJourneys
			.Where(x => x.State is not (VehicleJourneyState.Arrived or VehicleJourneyState.Cancelled or VehicleJourneyState.Faulted))
			.OfType<VehicleJourney>())
		{
			var downtime = VehicleJourneyStateRules.DowntimeDelay(
				journey.State, journey.LastCheckpointUtc, DateTimeOffset.UtcNow);
			if (downtime > TimeSpan.Zero)
			{
				journey.AddDelay(downtime, $"Recovered after {downtime.TotalSeconds:N0} seconds of downtime.");
			}
			journey.Checkpoint("Journey recovered from its durable checkpoint.");
			coordinator.StartOrResume(journey);
		}

		foreach (var service in _vehicleServices.OfType<VehicleService>())
		{
			service.StartScheduling(coordinator);
		}

		ConsoleUtilities.WriteLine("Loaded #2{0:N0}#0 vehicle services and #2{1:N0}#0 vehicle journeys.",
			services.Count, journeys.Count);
	}
}
