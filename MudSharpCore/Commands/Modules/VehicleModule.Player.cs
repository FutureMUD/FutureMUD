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
		if (vehicle.MovementProfile?.MovementEnvironment == VehicleMovementEnvironment.SurfaceWater)
		{
			var modes = vehicle.MovementProfile.PropulsionProfiles?.ToList() ?? [];
			sb.AppendLine($"Propulsion: {(vehicle.ActivePropulsionProfile?.PropulsionType.DescribeEnum().ColourName() ?? "legacy implicit movement".Colour(Telnet.Yellow))}");
			sb.AppendLine($"Supported propulsion: {(modes.Any() ? modes.Select(x => x.PropulsionType.DescribeEnum().ColourName()).ListToString() : "none authored".Colour(Telnet.Yellow))}");
		}
		sb.AppendLine($"Controller: {(vehicle.Controller is null ? "none".Colour(Telnet.Yellow) : vehicle.Controller.HowSeen(actor))}");
		sb.AppendLine($"Crew: {vehicle.Occupancies.Count().ToString("N0", actor).ColourValue()} occupying {vehicle.Prototype.OccupantSlots.Count().ToString("N0", actor).ColourValue()} configured slot type{(vehicle.Prototype.OccupantSlots.Count() == 1 ? "" : "s")}");
		sb.AppendLine($"Access: {(vehicle.AccessPoints.Any() ? vehicle.AccessPoints.Select(x => $"{x.Name} ({(x.IsOpen ? "open" : "closed")}{(x.IsLocked ? ", locked" : "")})").ListToString() : "none")}");
		sb.AppendLine($"Cargo: {(vehicle.CargoSpaces.Any() ? vehicle.CargoSpaces.Select(x => x.Name).ListToString() : "none")}");
		sb.AppendLine($"Modules: {(vehicle.Installations.Any() ? vehicle.Installations.Select(x => $"{x.Prototype.Name} ({(x.InstalledItem is null ? "empty" : "installed")})").ListToString() : "none")}");
		sb.AppendLine($"Damage: {(vehicle.Destroyed ? "destroyed".ColourError() : vehicle.Disabled ? "disabled".ColourError() : vehicle.DamageZones.Any(x => x.CurrentDamage > 0.0) ? "damaged".Colour(Telnet.Yellow) : "serviceable".Colour(Telnet.Green))}");
		if (vehicle.ActiveJourney is { } journey)
		{
			AppendActiveJourneyStatus(sb, actor, journey);
		}

		if (vehicle.Controller?.SamePhysicalInstance(actor) == true)
		{
			AppendMovementReadiness(sb, actor, vehicle, OperationalReadinessService);
		}
		else
		{
			sb.AppendLine("Cell-exit readiness: take control from a driver slot to perform a full preflight check.".Colour(Telnet.Yellow));
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	internal static void AppendMovementReadiness(StringBuilder sb, ICharacter actor, IVehicle vehicle,
		IVehicleOperationalReadinessService readinessService)
	{
		var routeProfile = vehicle.Location?.RouteDefinition is null
			? null
			: vehicle.Prototype.MovementProfiles
				.Where(x => x.MovementType == VehicleMovementProfileType.Route)
				.OrderByDescending(x => x.IsDefault)
				.ThenBy(x => x.Id)
				.FirstOrDefault();
		if (routeProfile is not null)
		{
			var continuingMovement = vehicle.MovementState.MovementStatus == VehicleMovementStatus.Moving
				? actor.Movement
				: null;
			var readiness = readinessService.BuildLongitudinalMovementReadiness(
				new VehicleLongitudinalReadinessRequest(
					vehicle,
					actor,
					routeProfile,
					0.0,
					TimeSpan.Zero,
					ContinuingMovement: continuingMovement));
			sb.AppendLine($"RouteCell readiness: {(readiness.CanMove ? "ready".Colour(Telnet.Green) : readiness.Reason.ColourError())}");
			return;
		}

		var cellExitReadiness = readinessService.BuildMovementReadiness(
			new VehicleMovementReadinessRequest(vehicle, actor, null));
		sb.AppendLine($"Cell-exit readiness: {(cellExitReadiness.CanMove ? "ready".Colour(Telnet.Green) : cellExitReadiness.Reason.ColourError())}");
	}

	internal static void AppendActiveJourneyStatus(StringBuilder sb, ICharacter actor, IVehicleJourney journey)
	{
		var platforms = journey.CurrentStop?.PlatformBindings
			.Select(x => x.PlatformCell.HowSeen(actor))
			.ToList() ?? [];
		sb.AppendLine($"Journey: {journey.State.DescribeEnum().ColourName()} on {journey.Service.Name.ColourName()}");
		sb.AppendLine($"Timetable: scheduled {journey.ScheduledDeparture.ToString().ColourValue()}, expected {journey.ExpectedDeparture.ToString().ColourValue()}, delay {journey.Delay.Describe(actor).ColourValue()}");
		sb.AppendLine($"Stops: current {journey.CurrentStop?.Name.ColourName() ?? "none"}, next {journey.NextStop?.Name.ColourName() ?? "none"}");
		sb.AppendLine($"Platform: {(platforms.Any() ? platforms.ListToString() : "none")}");
		sb.AppendLine($"Boarding: {(journey.BoardingOpen ? "open".Colour(Telnet.Green) : "closed".Colour(Telnet.Yellow))}{(string.IsNullOrWhiteSpace(journey.StatusReason) ? string.Empty : $"; {journey.StatusReason.ColourError()}")}");
	}
}
