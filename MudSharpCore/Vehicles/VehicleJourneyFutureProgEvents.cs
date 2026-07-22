#nullable enable

using MudSharp.Events;

namespace MudSharp.Vehicles;

/// <summary>
/// Projects durable journey transitions onto the vehicle exterior's FutureProg hooks. The public
/// payload stays within the legacy event type code range and carries numeric ids for the richer
/// route, service, and journey lookup functions.
/// </summary>
internal static class VehicleJourneyFutureProgEvents
{
	public static EventType? HookEventFor(VehicleJourneyEventType eventType)
	{
		return eventType switch
		{
			VehicleJourneyEventType.Departed => EventType.VehicleJourneyDeparted,
			VehicleJourneyEventType.StopArrived => EventType.VehicleJourneyArrived,
			VehicleJourneyEventType.DelayChanged => EventType.VehicleJourneyDelayChanged,
			VehicleJourneyEventType.Cancelled => EventType.VehicleJourneyCancelled,
			VehicleJourneyEventType.Faulted => EventType.VehicleJourneyFaulted,
			_ => null
		};
	}

	public static void Dispatch(IVehicleJourney journey, VehicleJourneyEventType eventType, string message)
	{
		var hookEvent = HookEventFor(eventType);
		var exterior = journey.Vehicle.ExteriorItem;
		if (!hookEvent.HasValue || exterior is null)
		{
			return;
		}

		switch (eventType)
		{
			case VehicleJourneyEventType.Departed:
			case VehicleJourneyEventType.StopArrived:
				var stop = journey.CurrentStop;
				if (stop is null)
				{
					return;
				}

				exterior.HandleEvent(
					hookEvent.Value,
					exterior,
					journey.Id,
					journey.Route.Id,
					journey.Service.Id,
					journey.Vehicle.Id,
					stop.Location.Cell,
					stop.Location.RoutePositionMetres ?? -1.0,
					message);
				return;
			case VehicleJourneyEventType.DelayChanged:
				exterior.HandleEvent(
					hookEvent.Value,
					exterior,
					journey.Id,
					journey.Route.Id,
					journey.Service.Id,
					journey.Vehicle.Id,
					journey.Delay,
					message);
				return;
			case VehicleJourneyEventType.Cancelled:
			case VehicleJourneyEventType.Faulted:
				exterior.HandleEvent(
					hookEvent.Value,
					exterior,
					journey.Id,
					journey.Route.Id,
					journey.Service.Id,
					journey.Vehicle.Id,
					message);
				return;
		}
	}
}
