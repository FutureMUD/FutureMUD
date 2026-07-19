using MudSharp.Commands.Trees;
using MudSharp.Vehicles;

namespace MudSharp.Commands.Modules;

internal partial class VehicleModule
{
	private const string VehicleControlHelp = @"Use #3vehiclecontrol#0 while occupying a driver slot to take control of your vehicle after its previous controller releases it.

Syntax:
	#3vehiclecontrol#0
	#3vehiclecontrol release#0
	#3takecontrol#0
	#3releasecontrol#0";

	[PlayerCommand("VehicleControl", "vehiclecontrol", "takecontrol", "releasecontrol")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoCombatCommand]
	[NoMovementCommand]
	[NoHideCommand]
	[HelpInfo("vehiclecontrol", VehicleControlHelp, AutoHelp.HelpArg)]
	protected static void VehicleControl(ICharacter actor, string input)
	{
		var ss = new StringStack(input);
		var invokedCommand = ss.PopSpeech();
		var release = invokedCommand.EqualTo("releasecontrol") || ss.PopSpeech().EqualTo("release");
		var vehicle = actor.Gameworld.Vehicles.FirstOrDefault(x => x.IsOccupant(actor));
		if (vehicle is null)
		{
			actor.OutputHandler.Send("You are not aboard a vehicle.");
			return;
		}

		if (release)
		{
			if (!vehicle.ReleaseControl(actor))
			{
				actor.OutputHandler.Send("You are not controlling that vehicle.");
				return;
			}

			actor.OutputHandler.Send($"You release control of {vehicle.Name.ColourName()}.");
			return;
		}

		if (!vehicle.CanTakeControl(actor, out var reason))
		{
			actor.OutputHandler.Send(reason);
			return;
		}

		vehicle.TakeControl(actor);
		actor.OutputHandler.Send($"You take control of {vehicle.Name.ColourName()}.");
	}

	private const string VehicleStatusHelp = @"Use #3vehiclestatus#0 to see the condition and movement readiness of the vehicle you are aboard, or #3vehiclestatus <vehicle>#0 to inspect a vehicle exterior in your location.";

	[PlayerCommand("VehicleStatus", "vehiclestatus", "vehiclecheck")]
	[HelpInfo("vehiclestatus", VehicleStatusHelp, AutoHelp.HelpArg)]
	protected static void VehicleStatus(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		IVehicle vehicle;
		if (ss.IsFinished)
		{
			vehicle = actor.Gameworld.Vehicles.FirstOrDefault(x => x.IsOccupant(actor));
		}
		else
		{
			var item = actor.TargetLocalItem(ss.PopSpeech());
			vehicle = item?.GetItemType<IVehicleExterior>()?.Vehicle;
		}

		if (vehicle is null)
		{
			actor.OutputHandler.Send("You are not aboard a vehicle and do not see that vehicle here.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"{vehicle.Name.ColourName()} - Vehicle Status".GetLineWithTitle(actor, Telnet.Cyan,
			Telnet.BoldWhite));
		sb.AppendLine($"Movement: {vehicle.MovementState.MovementStatus.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Controller: {(vehicle.Controller is null ? "none".Colour(Telnet.Yellow) : vehicle.Controller.HowSeen(actor))}");
		sb.AppendLine($"Crew: {vehicle.Occupancies.Count().ToString("N0", actor).ColourValue()} occupying {vehicle.Prototype.OccupantSlots.Count().ToString("N0", actor).ColourValue()} configured slot type{(vehicle.Prototype.OccupantSlots.Count() == 1 ? "" : "s")}");
		sb.AppendLine($"Access: {(vehicle.AccessPoints.Any() ? vehicle.AccessPoints.Select(x => $"{x.Name} ({(x.IsOpen ? "open" : "closed")}{(x.IsLocked ? ", locked" : "")})").ListToString() : "none")}");
		sb.AppendLine($"Cargo: {(vehicle.CargoSpaces.Any() ? vehicle.CargoSpaces.Select(x => x.Name).ListToString() : "none")}");
		sb.AppendLine($"Modules: {(vehicle.Installations.Any() ? vehicle.Installations.Select(x => $"{x.Prototype.Name} ({(x.InstalledItem is null ? "empty" : "installed")})").ListToString() : "none")}");
		sb.AppendLine($"Damage: {(vehicle.Destroyed ? "destroyed".ColourError() : vehicle.Disabled ? "disabled".ColourError() : vehicle.DamageZones.Any(x => x.CurrentDamage > 0.0) ? "damaged".Colour(Telnet.Yellow) : "serviceable".Colour(Telnet.Green))}");

		if (vehicle.Controller?.SamePhysicalInstance(actor) == true)
		{
			var readiness = OperationalReadinessService.BuildMovementReadiness(
				new VehicleMovementReadinessRequest(vehicle, actor, null));
			sb.AppendLine($"Cell-exit readiness: {(readiness.CanMove ? "ready".Colour(Telnet.Green) : readiness.Reason.ColourError())}");
		}
		else
		{
			sb.AppendLine("Cell-exit readiness: take control from a driver slot to perform a full preflight check.".Colour(Telnet.Yellow));
		}

		actor.OutputHandler.Send(sb.ToString());
	}
}
