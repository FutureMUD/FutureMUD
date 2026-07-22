#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.Vehicles;

namespace MudSharp.Movement;

/// <summary>
/// Publishes longitudinal movement phases without widening a local RouteCell event to the
/// entire linear cell. Vehicle exterior and hosted-interior audiences are deliberately separate.
/// </summary>
public static class RouteMovementOutput
{
	public static void CharacterBegin(ICharacter mover, string directionName)
	{
		EchoCharacter(mover, $"@ begin|begins travelling {directionName.ColourName()}.");
	}

	public static void CharacterProgress(ICharacter mover, string directionName)
	{
		EchoCharacter(mover, $"@ continue|continues travelling {directionName.ColourName()}.");
	}

	public static void CharacterFinished(ICharacter mover, string echo)
	{
		EchoCharacter(mover, $"@ {echo.Trim().TrimEnd('.')}.");
	}

	public static void VehicleBegin(IVehicle vehicle, string directionName)
	{
		EchoVehicleExterior(vehicle,
			$"@ begin|begins travelling {directionName.ColourName()} along the route.");
		EchoHostedInteriors(vehicle,
			$"The vehicle shudders as it begins travelling {directionName} along the route.");
	}

	public static void VehicleProgress(IVehicle vehicle, string directionName)
	{
		EchoVehicleExterior(vehicle,
			$"@ continue|continues travelling {directionName.ColourName()} along the route.");
		EchoHostedInteriors(vehicle,
			$"The vehicle continues travelling {directionName} along the route.");
	}

	public static void VehicleFinished(IVehicle vehicle, bool succeeded)
	{
		if (succeeded)
		{
			EchoVehicleExterior(vehicle, "@ arrive|arrives at the requested route position.");
			EchoHostedInteriors(vehicle, "The vehicle settles as it arrives at its requested route position.");
			return;
		}

		EchoVehicleExterior(vehicle, "@ stop|stops at its current route position.");
		EchoHostedInteriors(vehicle, "The vehicle slows and stops at its current route position.");
	}

	private static void EchoCharacter(ICharacter mover, string emote)
	{
		mover.OutputHandler.Handle(
			new EmoteOutput(new Emote(emote, mover)),
			OutputRange.Local);
	}

	private static void EchoVehicleExterior(IVehicle vehicle, string emote)
	{
		var exterior = vehicle.ExteriorItem;
		exterior.OutputHandler.Handle(
			new EmoteOutput(new Emote(emote, exterior)),
			OutputRange.Local);
	}

	private static void EchoHostedInteriors(IVehicle vehicle, string message)
	{
		if (vehicle.Prototype.Scale != VehicleScale.RoomScale)
		{
			return;
		}

		foreach (var cell in vehicle.Compartments
			         .Select(x => x.InteriorCell)
			         .Where(x => x is not null)
			         .Distinct())
		{
			cell!.Handle(message);
		}
	}
}
