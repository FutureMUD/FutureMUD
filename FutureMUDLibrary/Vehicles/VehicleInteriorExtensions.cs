using MudSharp.Character;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Vehicles;

public static class VehicleInteriorExtensions
{
	/// <summary>
	/// Determines whether an access point can reach an occupant slot through the vehicle's currently usable
	/// RoomScale interior graph. Non-RoomScale vehicles retain the legacy same-compartment rule.
	/// </summary>
	public static bool AccessPointCanReachSlot(this IVehicle vehicle, IVehicleAccessPoint accessPoint,
		IVehicleOccupantSlotPrototype slot)
	{
		if (vehicle is null || accessPoint is null || slot is null)
		{
			return false;
		}

		var sourcePrototype = accessPoint.Prototype.Compartment;
		var destinationPrototype = slot.Compartment;
		if (sourcePrototype is null || destinationPrototype is null)
		{
			return true;
		}

		if (sourcePrototype.Id == destinationPrototype.Id)
		{
			return true;
		}

		if (vehicle.Prototype.Scale != VehicleScale.RoomScale)
		{
			return false;
		}

		var source = vehicle.Compartments.FirstOrDefault(x => x.Prototype.Id == sourcePrototype.Id);
		if (source?.InteriorCell is null ||
			vehicle.Compartments.FirstOrDefault(x => x.Prototype.Id == destinationPrototype.Id)?.InteriorCell is null)
		{
			return false;
		}

		var visited = new HashSet<long> { source.Prototype.Id };
		var queue = new Queue<IVehicleCompartment>();
		queue.Enqueue(source);
		while (queue.TryDequeue(out var current))
		{
			foreach (var link in current.Links.Where(x => x.Exit is not null))
			{
				var next = link.SourceCompartment.Prototype.Id == current.Prototype.Id
					? link.DestinationCompartment
					: link.DestinationCompartment.Prototype.Id == current.Prototype.Id
						? link.SourceCompartment
						: null;
				if (next?.InteriorCell is null || !visited.Add(next.Prototype.Id))
				{
					continue;
				}

				if (next.Prototype.Id == destinationPrototype.Id)
				{
					return true;
				}

				queue.Enqueue(next);
			}
		}

		return false;
	}

	/// <summary>
	/// Verifies the physical control/slot location for RoomScale occupants. Occupant records are assignments;
	/// they do not make a character standing in another hosted compartment physically present at the station.
	/// </summary>
	public static bool IsAtOccupantSlotLocation(this IVehicle vehicle, ICharacter actor,
		IVehicleOccupantSlotPrototype slot)
	{
		if (vehicle is null || actor is null || slot is null)
		{
			return false;
		}

		if (vehicle.Prototype.Scale != VehicleScale.RoomScale)
		{
			return true;
		}

		var compartment = vehicle.Compartments.FirstOrDefault(x => x.Prototype.Id == slot.Compartment?.Id);
		return compartment?.InteriorCell is not null && compartment.InteriorCell.Id == actor.Location?.Id;
	}
}
