#nullable enable

using MudSharp.Commands.Trees;
using MudSharp.Accounts;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.Vehicles;
using TimeSpanParserUtil;

namespace MudSharp.Commands.Modules;

internal partial class VehicleModule
{
	private const string VehicleRouteHelp = @"The #3vehicleroute#0 command authors immutable, topology-pinned service routes.

Syntax:
	#3vehicleroute list#0
	#3vehicleroute show <id|name>#0
	#3vehicleroute preview [id|name]#0
	#3vehicleroute edit new <name>#0
	#3vehicleroute edit clone <id|name>#0
	#3vehicleroute edit <id|name>#0
	#3vehicleroute close#0
	#3vehicleroute set <setting>#0
	#3vehicleroute validate#0
	#3vehicleroute submit <comment>#0
	#3vehicleroute approve <id|name> <comment>#0

Route settings:
	#3stop add <name> [<cell id> [<layer>] [at <route distance>]]#0
	#3stop remove <stop>#0
	#3stop move <stop> <one-based position>#0
	#3stop location <stop> <cell id> [<layer>] [at <route distance>]#0
	#3stop dwell <stop> <duration>#0
	#3stop platform add <stop> <cell id> <access-point prototype id> [tolerance metres]#0
	#3stop platform remove <stop> <binding id>#0
	#3leg compile <origin stop> <destination stop>#0";

	[PlayerCommand("VehicleRoute", "vehicleroute", "vroute")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("vehicleroute", VehicleRouteHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void VehicleRouteCommand(ICharacter actor, string input)
	{
		var command = new StringStack(input.RemoveFirstWord());
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "":
			case "list":
				VehicleRouteList(actor);
				return;
			case "show":
			case "preview":
				VehicleRouteShow(actor, command);
				return;
			case "new":
				VehicleRouteNew(actor, command);
				return;
			case "edit":
				VehicleRouteEdit(actor, command);
				return;
			case "clone":
				VehicleRouteClone(actor, command);
				return;
			case "close":
				actor.SetEditingItem<IVehicleRoute>(null);
				actor.OutputHandler.Send("You stop editing vehicle routes.");
				return;
			case "set":
				VehicleRouteSet(actor, command);
				return;
			case "validate":
				VehicleRouteValidate(actor);
				return;
			case "submit":
				VehicleRouteSubmit(actor, command);
				return;
			case "approve":
				VehicleRouteApprove(actor, command);
				return;
			default:
				actor.OutputHandler.Send(VehicleRouteHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void VehicleRouteList(ICharacter actor)
	{
		var sb = new StringBuilder("Vehicle Routes:\n");
		foreach (var route in actor.Gameworld.VehicleRoutes
			.OrderBy(x => x.Id)
			.ThenByDescending(x => x.RevisionNumber))
		{
			var valid = route is VehicleRoute concrete && concrete.TopologyIsCurrent;
			sb.AppendLine($"\t#{route.Id.ToString("N0", actor)}r{route.RevisionNumber.ToString("N0", actor)} {route.Name.ColourName()} [{route.Status.DescribeColour()}] {route.Stops.Count.ToString("N0", actor).ColourValue()} stops / {(valid ? "topology current".Colour(Telnet.Green) : "topology stale".ColourError())}");
		}
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void VehicleRouteShow(ICharacter actor, StringStack command)
	{
		var route = command.IsFinished
			? actor.EditingItem<IVehicleRoute>()
			: ResolveRoute(actor, command.SafeRemainingArgument);
		actor.OutputHandler.Send(route?.Show(actor) ?? "There is no such vehicle route.");
	}

	private static void VehicleRouteNew(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name should the new vehicle route have?");
			return;
		}
		var route = new VehicleRoute(actor.Gameworld, actor.Account, command.SafeRemainingArgument);
		actor.Gameworld.Add(route);
		actor.SetEditingItem<IVehicleRoute>(route);
		actor.OutputHandler.Send($"You create and begin editing {route.EditHeader()}.");
	}

	private static void VehicleRouteEdit(ICharacter actor, StringStack command)
	{
		if (command.PeekSpeech().EqualTo("new"))
		{
			command.PopSpeech();
			VehicleRouteNew(actor, command);
			return;
		}
		if (command.PeekSpeech().EqualTo("clone"))
		{
			command.PopSpeech();
			VehicleRouteClone(actor, command);
			return;
		}

		var route = ResolveRoute(actor, command.SafeRemainingArgument);
		if (route is null)
		{
			actor.OutputHandler.Send("There is no such vehicle route.");
			return;
		}
		if (route.Status == RevisionStatus.Current)
		{
			route = (IVehicleRoute)route.CreateNewRevision(actor);
			actor.Gameworld.Add(route);
		}
		actor.SetEditingItem(route);
		actor.OutputHandler.Send($"You are now editing {route.EditHeader()}.");
	}

	private static void VehicleRouteClone(ICharacter actor, StringStack command)
	{
		var route = ResolveRoute(actor, command.SafeRemainingArgument);
		if (route is null)
		{
			actor.OutputHandler.Send("There is no such vehicle route.");
			return;
		}
		var revision = (IVehicleRoute)route.CreateNewRevision(actor);
		actor.Gameworld.Add(revision);
		actor.SetEditingItem(revision);
		actor.OutputHandler.Send($"You clone the route into revision {revision.EditHeader()}.");
	}

	private static void VehicleRouteSet(ICharacter actor, StringStack command)
	{
		var route = actor.EditingItem<IVehicleRoute>();
		if (route is null)
		{
			actor.OutputHandler.Send("You are not editing a vehicle route.");
			return;
		}
		route.BuildingCommand(actor, command);
	}

	private static void VehicleRouteValidate(ICharacter actor)
	{
		var route = actor.EditingItem<IVehicleRoute>();
		if (route is null)
		{
			actor.OutputHandler.Send("You are not editing a vehicle route.");
			return;
		}
		actor.OutputHandler.Send(route.CanSubmit()
			? "This route passes structural and topology validation.".Colour(Telnet.Green)
			: route.WhyCannotSubmit());
	}

	private static void VehicleRouteSubmit(ICharacter actor, StringStack command)
	{
		var route = actor.EditingItem<IVehicleRoute>();
		if (route is null)
		{
			actor.OutputHandler.Send("You are not editing a vehicle route.");
			return;
		}
		if (!route.CanSubmit())
		{
			actor.OutputHandler.Send(route.WhyCannotSubmit());
			return;
		}
		route.ChangeStatus(RevisionStatus.PendingRevision, command.SafeRemainingArgument, actor.Account);
		actor.SetEditingItem<IVehicleRoute>(null);
		actor.OutputHandler.Send($"You submit {route.EditHeader()} for review.");
	}

	private static void VehicleRouteApprove(ICharacter actor, StringStack command)
	{
		var routeText = command.PopSpeech();
		if (!command.IsFinished && int.TryParse(command.PeekSpeech(), out _))
		{
			routeText = $"{routeText} {command.PopSpeech()}";
		}
		var route = ResolveRoute(actor, routeText);
		if (route is null)
		{
			actor.OutputHandler.Send("There is no such vehicle route.");
			return;
		}
		if (!route.CanSubmit())
		{
			actor.OutputHandler.Send(route.WhyCannotSubmit());
			return;
		}
		foreach (var old in actor.Gameworld.VehicleRoutes.GetAll(route.Id)
			.Where(x => x.Status == RevisionStatus.Current))
		{
			old.ChangeStatus(RevisionStatus.Revised, "Revised by a newer route revision.", actor.Account);
		}
		route.ChangeStatus(RevisionStatus.Current, command.SafeRemainingArgument, actor.Account);
		actor.OutputHandler.Send($"You approve {route.EditHeader()}; its compiled steps and topology pins are now immutable.");
	}

	private const string VehicleServiceHelp = @"The #3vehicleservice#0 command manages recurring route operations.

Syntax:
	#3vehicleservice list#0
	#3vehicleservice show|audit <id|name>#0
	#3vehicleservice new <name> | <route> | <vehicle> | <reference departure> | <recurrence>#0
	#3vehicleservice edit <id|name>#0
	#3vehicleservice clone <id|name> <new name>#0
	#3vehicleservice close#0
	#3vehicleservice set name <name>#0
	#3vehicleservice set route <id> <revision>#0
	#3vehicleservice set vehicle <vehicle>#0
	#3vehicleservice set operator automatic|onboard#0
	#3vehicleservice set schedule <reference departure> | <recurrence>#0
	#3vehicleservice set retry|maxhold <duration>#0
	#3vehicleservice set enabled [true|false]#0
	#3vehicleservice cancel <id|name> [reason]#0
	#3vehicleservice resume <id|name>#0

Schedules use #3<reference departure> | <recurrence>#0. Recurrences accept forms such as #3every 15 minutes#0, #3every day#0, and #3every month on day 15#0.";

	[PlayerCommand("VehicleService", "vehicleservice", "vservice")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("vehicleservice", VehicleServiceHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void VehicleServiceCommand(ICharacter actor, string input)
	{
		var command = new StringStack(input.RemoveFirstWord());
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "":
			case "list": VehicleServiceList(actor); return;
			case "show": VehicleServiceShow(actor, command, false); return;
			case "audit": VehicleServiceShow(actor, command, true); return;
			case "new": VehicleServiceNew(actor, command.SafeRemainingArgument); return;
			case "edit": VehicleServiceEdit(actor, command); return;
			case "clone": VehicleServiceClone(actor, command); return;
			case "close": actor.SetEditingItem<IVehicleService>(null); actor.OutputHandler.Send("You stop editing vehicle services."); return;
			case "set": VehicleServiceSet(actor, command); return;
			case "cancel": VehicleServiceCancel(actor, command); return;
			case "resume": VehicleServiceResume(actor, command); return;
			default: actor.OutputHandler.Send(VehicleServiceHelp.SubstituteANSIColour()); return;
		}
	}

	private static void VehicleServiceList(ICharacter actor)
	{
		var sb = new StringBuilder("Vehicle Services:\n");
		foreach (var service in actor.Gameworld.VehicleServices.OrderBy(x => x.Id))
		{
			sb.AppendLine($"\t#{service.Id.ToString("N0", actor)} {service.Name.ColourName()} - {service.Route.Name.ColourName()} / {service.Vehicle.Name.ColourName()} / next {service.Schedule.NextDeparture.ToString().ColourValue()} / {(service.Enabled ? "enabled".Colour(Telnet.Green) : "disabled".Colour(Telnet.Yellow))}");
		}
		actor.OutputHandler.Send(sb.ToString());
	}

	private static IVehicleService? ResolveService(ICharacter actor, string text)
	{
		return actor.Gameworld.VehicleServices.GetByIdOrName(text);
	}

	private static IVehicleRoute? ResolveRoute(ICharacter actor, string text)
	{
		var trimmed = text.Trim();
		var command = new StringStack(trimmed);
		var idText = command.PopSpeech().TrimStart('#');
		var revisionSeparator = idText.LastIndexOfAny(['r', 'R']);
		if (revisionSeparator > 0 &&
		    long.TryParse(idText[..revisionSeparator], out var compactId) &&
		    int.TryParse(idText[(revisionSeparator + 1)..], out var compactRevision))
		{
			return actor.Gameworld.VehicleRoutes.Get(compactId, compactRevision);
		}
		if (long.TryParse(idText, out var id) && !command.IsFinished &&
		    int.TryParse(command.SafeRemainingArgument, out var revision))
		{
			return actor.Gameworld.VehicleRoutes.Get(id, revision);
		}

		return actor.Gameworld.VehicleRoutes.GetByIdOrName(trimmed);
	}

	private static void VehicleServiceShow(ICharacter actor, StringStack command, bool auditOnly)
	{
		var service = ResolveService(actor, command.SafeRemainingArgument);
		if (service is not VehicleService concrete)
		{
			actor.OutputHandler.Send("There is no such vehicle service.");
			return;
		}
		if (!auditOnly)
		{
			actor.OutputHandler.Send(concrete.Show(actor));
			return;
		}
		var errors = concrete.AuditErrors(includeActiveJourney: true);
		actor.OutputHandler.Send(errors.Any()
			? $"Service audit failures:\n{errors.Select(x => $"\t{x.ColourError()}").ListToString(separator: "\n", conjunction: string.Empty, twoItemJoiner: "\n")}"
			: "This service passes its operational audit.".Colour(Telnet.Green));
	}

	private static void VehicleServiceNew(ICharacter actor, string arguments)
	{
		var parts = arguments.Split('|', StringSplitOptions.TrimEntries);
		if (parts.Length != 5)
		{
			actor.OutputHandler.Send("Use vehicleservice new <name> | <route> | <vehicle> | <reference departure> | <recurrence>.");
			return;
		}
		var route = ResolveRoute(actor, parts[1]);
		var vehicle = actor.Gameworld.Vehicles.GetByIdOrName(parts[2]);
		if (route is null || vehicle is null)
		{
			actor.OutputHandler.Send("The route or vehicle could not be found.");
			return;
		}
		if (!TryParseServiceSchedule(actor, parts[3], parts[4], out var reference, out var recurrence, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}
		var service = new VehicleService(actor.Gameworld, parts[0], route, vehicle, reference, recurrence);
		actor.Gameworld.Add(service);
		actor.SetEditingItem<IVehicleService>(service);
		actor.OutputHandler.Send($"You create disabled service #{service.Id.ToString("N0", actor)} {service.Name.ColourName()} and begin editing it.");
	}

	private static void VehicleServiceEdit(ICharacter actor, StringStack command)
	{
		var service = ResolveService(actor, command.SafeRemainingArgument);
		if (service is null)
		{
			actor.OutputHandler.Send("There is no such vehicle service.");
			return;
		}
		actor.SetEditingItem(service);
		actor.OutputHandler.Send($"You are now editing service #{service.Id.ToString("N0", actor)} {service.Name.ColourName()}.");
	}

	private static void VehicleServiceClone(ICharacter actor, StringStack command)
	{
		var service = ResolveService(actor, command.PopSpeech()) as VehicleService;
		if (service is null || command.IsFinished)
		{
			actor.OutputHandler.Send("Specify a service to clone and a new name.");
			return;
		}
		var clone = service.Clone(command.SafeRemainingArgument);
		actor.Gameworld.Add(clone);
		actor.SetEditingItem<IVehicleService>(clone);
		actor.OutputHandler.Send($"You clone the service as disabled service #{clone.Id.ToString("N0", actor)} {clone.Name.ColourName()}.");
	}

	private static void VehicleServiceSet(ICharacter actor, StringStack command)
	{
		if (actor.EditingItem<IVehicleService>() is not VehicleService service)
		{
			actor.OutputHandler.Send("You are not editing a vehicle service.");
			return;
		}
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				if (command.IsFinished) { actor.OutputHandler.Send("Specify a service name."); return; }
				service.SetNameAndKeywords(command.SafeRemainingArgument); break;
			case "route":
				var route = ResolveRoute(actor, command.SafeRemainingArgument);
				if (route is null) { actor.OutputHandler.Send("There is no such route."); return; }
				if (!service.TrySetRoute(route, out var routeReason)) { actor.OutputHandler.Send(routeReason); return; }
				break;
			case "vehicle":
				var vehicle = actor.Gameworld.Vehicles.GetByIdOrName(command.SafeRemainingArgument);
				if (vehicle is null) { actor.OutputHandler.Send("There is no such vehicle."); return; }
				if (!service.TrySetVehicle(vehicle, out var vehicleReason)) { actor.OutputHandler.Send(vehicleReason); return; }
				break;
			case "operator":
				if (!command.SafeRemainingArgument.TryParseEnum<VehicleServiceOperatorMode>(out var mode)) { actor.OutputHandler.Send("Use automatic or onboard."); return; }
				service.SetOperatorMode(mode); break;
			case "retry":
				if (!TimeSpanParser.TryParse(command.SafeRemainingArgument, Units.Seconds, Units.Seconds, out var retry) || retry <= TimeSpan.Zero) { actor.OutputHandler.Send("Specify a positive retry duration."); return; }
				service.SetRetryInterval(retry); break;
			case "maxhold":
				if (!TimeSpanParser.TryParse(command.SafeRemainingArgument, Units.Minutes, Units.Seconds, out var hold) || hold < TimeSpan.Zero) { actor.OutputHandler.Send("Specify a non-negative maximum hold."); return; }
				service.SetMaximumHold(hold); break;
			case "enabled":
				var enabledText = command.SafeRemainingArgument;
				var enabled = command.IsFinished || enabledText.EqualToAny("true", "yes", "on", "enable", "enabled")
					? !command.IsFinished || !service.Enabled
					: false;
				if (!command.IsFinished && !enabled && !enabledText.EqualToAny("false", "no", "off", "disable", "disabled")) { actor.OutputHandler.Send("Use enabled, enabled true, or enabled false."); return; }
				if (enabled && service.AuditErrors(includeActiveJourney: true).Any()) { actor.OutputHandler.Send("This service cannot be enabled until its audit passes. Use vehicleservice audit."); return; }
				service.SetEnabled(enabled); break;
			case "schedule":
				var parts = command.SafeRemainingArgument.Split('|', StringSplitOptions.TrimEntries);
				if (parts.Length != 2)
				{
					actor.OutputHandler.Send("Use schedule <reference departure> | <recurrence>.");
					return;
				}
				if (!TryParseServiceSchedule(actor, parts[0], parts[1], out var reference, out var recurrence, out var error))
				{
					actor.OutputHandler.Send(error);
					return;
				}
				service.SetSchedule(reference, recurrence); break;
			default:
				actor.OutputHandler.Send("Valid settings are name, route, vehicle, operator, schedule, retry, maxhold, and enabled."); return;
		}
		actor.Gameworld.SaveManager.Flush();
		service.StartScheduling(VehicleJourneyCoordinator.For(actor.Gameworld));
		actor.OutputHandler.Send("The vehicle service has been updated.");
	}

	private static bool TryParseServiceSchedule(ICharacter actor, string referenceText, string recurrenceText,
		out MudDateTime reference, out RecurringInterval recurrence, out string error)
	{
		var calendar = actor.Location.Calendars.First();
		var clock = actor.Location.Clocks.First();
		if (!MudDateTime.TryParse(referenceText, calendar, clock, actor, out reference, out error))
		{
			recurrence = null!;
			return false;
		}
		if (!RecurringInterval.TryParse(recurrenceText, calendar, out recurrence, out error))
		{
			return false;
		}
		return true;
	}

	private static void VehicleServiceCancel(ICharacter actor, StringStack command)
	{
		var service = ResolveService(actor, command.PopSpeech());
		if (service?.ActiveJourney is not { } journey)
		{
			actor.OutputHandler.Send("That service has no active journey.");
			return;
		}
		VehicleJourneyCoordinator.For(actor.Gameworld).Cancel(journey,
			command.IsFinished ? $"Cancelled by {actor.Name}." : command.SafeRemainingArgument);
		actor.OutputHandler.Send($"Journey #{journey.Id.ToString("N0", actor)} is cancelled.");
	}

	private static void VehicleServiceResume(ICharacter actor, StringStack command)
	{
		var service = ResolveService(actor, command.SafeRemainingArgument);
		if (service?.ActiveJourney is not { } journey)
		{
			actor.OutputHandler.Send("That service has no active journey to resume.");
			return;
		}
		VehicleJourneyCoordinator.For(actor.Gameworld).Resume(journey);
		actor.OutputHandler.Send($"Journey #{journey.Id.ToString("N0", actor)} has been asked to resume from its durable checkpoint.");
	}

	private const string TransitHelp = @"Use #3transit departures [destination]#0 to see upcoming departures at your location, #3transit services [destination]#0 to browse enabled services, or #3transit status <service|vehicle>#0 for live journey status.";

	[PlayerCommand("Transit", "transit")]
	[RequiredCharacterState(CharacterState.Able)]
	[HelpInfo("transit", TransitHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Transit(ICharacter actor, string input)
	{
		var command = new StringStack(input.RemoveFirstWord());
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "departures": TransitDepartures(actor, command.SafeRemainingArgument); return;
			case "services": TransitServices(actor, command.SafeRemainingArgument); return;
			case "status": TransitStatus(actor, command.SafeRemainingArgument); return;
			default: actor.OutputHandler.Send(TransitHelp.SubstituteANSIColour()); return;
		}
	}

	private static IEnumerable<IVehicleService> FilterTransitServices(ICharacter actor, string destination)
	{
		return actor.Gameworld.VehicleServices
			.Where(x => x.Enabled)
			.Where(x => string.IsNullOrWhiteSpace(destination) || x.Route.Stops.Any(stop =>
				stop.Name.Contains(destination, StringComparison.InvariantCultureIgnoreCase)))
			.OrderBy(x => x.Schedule.NextDeparture);
	}

	internal static IVehicleRouteStop? FindLocalTransitStop(
		ICharacter actor,
		IVehicleRoute route,
		IRouteSpatialService? spatialService = null)
	{
		spatialService ??= RouteSpatialService.Instance;
		return FindLocalTransitStop(actor, route, spatialService.GetEffectiveLocation(actor));
	}

	private static IVehicleRouteStop? FindLocalTransitStop(
		ICharacter actor,
		IVehicleRoute route,
		SpatialLocation actorLocation)
	{
		var defaultTolerance = actor.Gameworld.GetStaticDouble("VehicleRouteDefaultDockingToleranceMetres");
		if (!double.IsFinite(defaultTolerance) || defaultTolerance < 0.0)
		{
			defaultTolerance = 2.0;
		}

		var candidates = new List<(IVehicleRouteStop Stop, int Priority, double Distance)>();
		foreach (var stop in route.Stops)
		{
			if (actorLocation.Cell.RouteDefinition is null &&
				stop.PlatformBindings.Any(x => x.PlatformCell == actorLocation.Cell))
			{
				candidates.Add((stop, 0, 0.0));
				continue;
			}

			if (stop.Location.Cell != actorLocation.Cell)
			{
				continue;
			}

			if (actorLocation.Cell.RouteDefinition is null)
			{
				candidates.Add((stop, 1, 0.0));
				continue;
			}

			if (stop.Location.Layer != actorLocation.Layer ||
				!stop.Location.RoutePositionMetres.HasValue ||
				!actorLocation.RoutePositionMetres.HasValue)
			{
				continue;
			}

			var tolerance = stop.PlatformBindings
				.Select(x => x.DockingToleranceMetres)
				.Where(x => double.IsFinite(x) && x >= 0.0)
				.DefaultIfEmpty(defaultTolerance)
				.Max();
			var distance = Math.Abs(
				stop.Location.RoutePositionMetres.Value - actorLocation.RoutePositionMetres.Value);
			if (distance <= tolerance)
			{
				candidates.Add((stop, 2, distance));
			}
		}

		return candidates
			.OrderBy(x => x.Priority)
			.ThenBy(x => x.Distance)
			.ThenBy(x => x.Stop.Sequence)
			.Select(x => x.Stop)
			.FirstOrDefault();
	}

	private static void TransitDepartures(ICharacter actor, string destination)
	{
		var actorLocation = RouteSpatialService.Instance.GetEffectiveLocation(actor);
		var services = FilterTransitServices(actor, destination)
			.Select(x => (Service: x, LocalStop: FindLocalTransitStop(actor, x.Route, actorLocation)))
			.Where(x => x.LocalStop is not null && DepartureIsVisibleAt(x.Service, x.LocalStop))
			.ToList();
		if (!services.Any())
		{
			actor.OutputHandler.Send("There are no matching scheduled departures at this location.");
			return;
		}
		var sb = new StringBuilder("Upcoming Departures:\n");
		foreach (var (service, localStop) in services)
		{
			var terminal = service.Route.Stops.OrderBy(x => x.Sequence).Last();
			var platform = localStop!.PlatformBindings
				.Where(x => x.PlatformCell == actorLocation.Cell)
				.Select(x => $"cell #{x.PlatformCell.Id.ToString("N0", actor)} via {x.AccessPoint.Name.ColourName()}")
				.DefaultIfEmpty("route-side stop")
				.ListToString();
			if (service.ActiveJourney is { } journey)
			{
				sb.AppendLine($"\t{service.Name.ColourName()} to {terminal.Name.ColourName()} - scheduled {journey.ScheduledDeparture.ToString().ColourValue()}, expected {journey.ExpectedDeparture.ToString().ColourValue()}, platform {platform}; {journey.State.DescribeEnum().ColourName()}, boarding {(journey.BoardingOpen ? "open".Colour(Telnet.Green) : "closed".Colour(Telnet.Yellow))}, delay {journey.Delay.Describe(actor).ColourValue()}{(string.IsNullOrWhiteSpace(journey.StatusReason) ? string.Empty : $", reason {journey.StatusReason.ColourError()}")}");
				continue;
			}
			sb.AppendLine($"\t{service.Name.ColourName()} to {terminal.Name.ColourName()} - scheduled and expected {service.Schedule.NextDeparture.ToString().ColourValue()}, platform {platform}; awaiting boarding window");
		}
		actor.OutputHandler.Send(sb.ToString());
	}

	internal static bool DepartureIsVisibleAt(IVehicleService service, IVehicleRouteStop localStop)
	{
		if (service.ActiveJourney is not { } journey)
		{
			return ReferenceEquals(
				service.Route.Stops.OrderBy(x => x.Sequence).FirstOrDefault(),
				localStop);
		}

		return ReferenceEquals(journey.CurrentStop, localStop) &&
		       journey.State is VehicleJourneyState.Boarding or
			       VehicleJourneyState.Held or
			       VehicleJourneyState.Dwelling;
	}

	private static void TransitServices(ICharacter actor, string destination)
	{
		var services = FilterTransitServices(actor, destination).ToList();
		if (!services.Any())
		{
			actor.OutputHandler.Send("There are no matching enabled vehicle services.");
			return;
		}
		var sb = new StringBuilder("Transit Services:\n");
		foreach (var service in services)
		{
			sb.AppendLine($"\t{service.Name.ColourName()}: {service.Route.Stops.OrderBy(x => x.Sequence).Select(x => x.Name.ColourName()).ListToString()} (next {service.Schedule.NextDeparture.ToString().ColourValue()})");
		}
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void TransitStatus(ICharacter actor, string serviceText)
	{
		var service = string.IsNullOrWhiteSpace(serviceText)
			? actor.Gameworld.VehicleServices.FirstOrDefault(x => x.ActiveJourney?.Vehicle.IsOccupant(actor) == true)
			: ResolveService(actor, serviceText);
		if (service is null && !string.IsNullOrWhiteSpace(serviceText) &&
		    actor.Gameworld.Vehicles.GetByIdOrName(serviceText) is { } vehicle)
		{
			service = actor.Gameworld.VehicleServices
				.Where(x => x.Vehicle.Id == vehicle.Id)
				.OrderByDescending(x => x.ActiveJourney is not null)
				.ThenBy(x => x.Schedule.NextDeparture)
				.FirstOrDefault();
		}
		if (service is null)
		{
			actor.OutputHandler.Send("There is no matching service or active journey for you.");
			return;
		}
		var journey = service.ActiveJourney ?? actor.Gameworld.VehicleJourneys
			.Where(x => x.Service.Id == service.Id)
			.OrderByDescending(x => x.Id)
			.FirstOrDefault();
		if (journey is null)
		{
			actor.OutputHandler.Send($"{service.Name.ColourName()} is not currently travelling. Its next departure is {service.Schedule.NextDeparture.ToString().ColourValue()}.");
			return;
		}
		var stop = journey.CurrentStop;
		var platforms = stop?.PlatformBindings
			.Select(x => $"cell #{x.PlatformCell.Id.ToString("N0", actor)} via {x.AccessPoint.Name.ColourName()}")
			.ToList() ?? [];
		actor.OutputHandler.Send($"{service.Name.ColourName()}: {journey.State.DescribeEnum().ColourName()}, scheduled {journey.ScheduledDeparture.ToString().ColourValue()}, expected {journey.ExpectedDeparture.ToString().ColourValue()}, current stop {journey.CurrentStop?.Name.ColourName() ?? "none"}, next stop {journey.NextStop?.Name.ColourName() ?? "none"}, platform {(platforms.Any() ? platforms.ListToString() : "none")}, delay {journey.Delay.Describe(actor).ColourValue()}, boarding {(journey.BoardingOpen ? "open".Colour(Telnet.Green) : "closed".Colour(Telnet.Yellow))}{(string.IsNullOrWhiteSpace(journey.StatusReason) ? string.Empty : $", reason: {journey.StatusReason.ColourError()}")}.");
	}
}
