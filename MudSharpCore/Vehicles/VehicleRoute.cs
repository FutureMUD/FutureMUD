#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Accounts;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using TimeSpanParserUtil;
using DB = MudSharp.Models;

namespace MudSharp.Vehicles;

public sealed class VehicleRoute : EditableItem, IVehicleRoute
{
	private readonly List<IVehicleRouteStop> _stops = [];
	private readonly List<IVehicleRouteLeg> _legs = [];
	private readonly List<IVehicleRouteTopologyPin> _topologyPins = [];
	private string _description;

	public VehicleRoute(DB.VehicleRoute dbitem, IFuturemud gameworld) : base(dbitem.EditableItem)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		_description = dbitem.Description;
		LoadChildren(dbitem);
	}

	public VehicleRoute(IFuturemud gameworld, IAccount originator, string name) : base(originator)
	{
		Gameworld = gameworld;
		_id = gameworld.VehicleRoutes.NextID();
		_name = name.TitleCase();
		_description = "An undescribed vehicle route.";
		using (new FMDB())
		{
			var dbitem = new DB.VehicleRoute
			{
				Id = Id,
				RevisionNumber = RevisionNumber,
				Name = Name,
				Description = _description,
				EditableItem = new DB.EditableItem
				{
					BuilderAccountId = BuilderAccountID,
					BuilderDate = BuilderDate,
					RevisionNumber = RevisionNumber,
					RevisionStatus = (int)Status
				}
			};
			FMDB.Context.VehicleRoutes.Add(dbitem);
			FMDB.Context.SaveChanges();
		}
	}

	public override string FrameworkItemType => "VehicleRoute";
	public IReadOnlyList<IVehicleRouteStop> Stops => _stops;
	public IReadOnlyList<IVehicleRouteLeg> Legs => _legs;
	public IReadOnlyCollection<IVehicleRouteTopologyPin> TopologyPins => _topologyPins;
	public string Description => _description;
	public bool TopologyIsCurrent
	{
		get
		{
			var steps = _legs
				.SelectMany(x => x.Steps)
				.ToList();
			if (!steps.All(StepTopologyIsCurrent) ||
			    _topologyPins.Any(x => x.RouteCell.RouteDefinition?.TopologyVersion != x.TopologyVersion))
			{
				return false;
			}

			var referencedRouteCells = _stops
				.Select(x => x.Location.Cell)
				.Concat(steps.SelectMany(x => new[] { x.Origin.Cell, x.Destination.Cell }))
				.Where(x => x.RouteDefinition is not null)
				.Distinct()
				.ToList();
			return referencedRouteCells.All(cell => _topologyPins.Any(pin =>
				pin.RouteCell == cell && pin.TopologyVersion == cell.RouteDefinition!.TopologyVersion)) &&
			       _topologyPins.All(pin => referencedRouteCells.Contains(pin.RouteCell));
		}
	}

	private static bool StepTopologyIsCurrent(IVehicleRouteStep step)
	{
		return EndpointTopologyIsCurrent(step.Origin, step.OriginTopologyVersion) &&
		       EndpointTopologyIsCurrent(step.Destination, step.DestinationTopologyVersion);
	}

	private static bool EndpointTopologyIsCurrent(SpatialLocation location, long? pinnedVersion)
	{
		var definition = location.Cell.RouteDefinition;
		return definition is null
			? location.RoutePositionMetres is null && pinnedVersion is null
			: location.RoutePositionMetres.HasValue && pinnedVersion == definition.TopologyVersion;
	}

	protected override IEnumerable<IEditableRevisableItem> GetAllSameId()
	{
		return Gameworld.VehicleRoutes.GetAll(Id);
	}

	public override string EditHeader()
	{
		return $"Vehicle Route [{Name.Proper().ColourValue()}] (#{Id:N0}r{RevisionNumber:N0})";
	}

	private void LoadChildren(DB.VehicleRoute dbitem)
	{
		_stops.Clear();
		_legs.Clear();
		_topologyPins.Clear();
		var stops = dbitem.Stops
			.OrderBy(x => x.Sequence)
			.ThenBy(x => x.Id)
			.Select(x => new VehicleRouteStop(this, x, Gameworld))
			.ToList();
		_stops.AddRange(stops);
		_legs.AddRange(dbitem.Legs
			.OrderBy(x => x.Sequence)
			.ThenBy(x => x.Id)
			.Select(x => new VehicleRouteLeg(this, x, stops, Gameworld)));
		_topologyPins.AddRange(dbitem.TopologyPins
			.OrderBy(x => x.RouteCellId)
			.Select(x => (Cell: Gameworld.Cells.Get(x.RouteCellId), x.TopologyVersion))
			.Where(x => x.Cell is not null)
			.Select(x => new VehicleRouteTopologyPin(x.Cell!, x.TopologyVersion)));
	}

	private void ReloadChildren()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleRoutes
				.Include(x => x.Stops).ThenInclude(x => x.PlatformBindings)
				.Include(x => x.Legs).ThenInclude(x => x.Steps)
				.Include(x => x.TopologyPins)
				.AsNoTracking()
				.First(x => x.Id == Id && x.RevisionNumber == RevisionNumber);
			LoadChildren(dbitem);
		}
	}

	public bool TryCompileLeg(IVehicleRouteStop origin, IVehicleRouteStop destination, out string reason)
	{
		if (Status != RevisionStatus.UnderDesign)
		{
			reason = "Only an under-design route revision can be compiled.";
			return false;
		}

		if (origin.Route != this || destination.Route != this || destination.Sequence != origin.Sequence + 1)
		{
			reason = "Route legs can only be compiled between adjacent stops in stop order.";
			return false;
		}

		if (!Gameworld.ExitManager.SpatialPathfinder.TryFindPath(
				origin.Location,
				destination.Location,
				null,
				false,
				double.PositiveInfinity,
				out var path) || path is null)
		{
			reason = $"No hybrid spatial path exists from {origin.Name} to {destination.Name}.";
			return false;
		}

		if (!TryBuildCompiledSteps(path, out var compiledSteps, out reason))
		{
			return false;
		}

		using (new FMDB())
		{
			var existing = FMDB.Context.VehicleRouteLegs
				.Include(x => x.Steps)
				.FirstOrDefault(x => x.VehicleRouteId == Id &&
					x.VehicleRouteRevision == RevisionNumber &&
					x.OriginStopId == origin.Id && x.DestinationStopId == destination.Id);
			if (existing is not null)
			{
				FMDB.Context.VehicleRouteLegs.Remove(existing);
			}

			var leg = new DB.VehicleRouteLeg
			{
				VehicleRouteId = Id,
				VehicleRouteRevision = RevisionNumber,
				Sequence = origin.Sequence,
				OriginStopId = origin.Id,
				DestinationStopId = destination.Id,
				RouteDistanceMetres = (decimal)path.RouteDistanceMetres,
				RoomEquivalentCost = (decimal)path.RoomEquivalentCost
			};
			foreach (var dbstep in compiledSteps)
			{
				leg.Steps.Add(dbstep);
			}

			FMDB.Context.VehicleRouteLegs.Add(leg);
			FMDB.Context.SaveChanges();
			RebuildTopologyPins(FMDB.Context);
			FMDB.Context.SaveChanges();
		}

		ReloadChildren();
		reason = string.Empty;
		return true;
	}

	internal static bool TryBuildCompiledSteps(
		ISpatialPath path,
		out IReadOnlyList<DB.VehicleRouteStep> steps,
		out string reason)
	{
		var compiledSteps = new List<DB.VehicleRouteStep>(path.Steps.Count);
		foreach (var (step, sequence) in path.Steps.Select((x, index) => (x, index)))
		{
			var dbstep = new DB.VehicleRouteStep
			{
				Sequence = sequence,
				OriginCellId = step.Origin.Cell.Id,
				OriginRoomLayer = (int)step.Origin.Layer,
				OriginRoutePositionMetres = ToDecimal(step.Origin.RoutePositionMetres),
				DestinationCellId = step.Destination.Cell.Id,
				DestinationRoomLayer = (int)step.Destination.Layer,
				DestinationRoutePositionMetres = ToDecimal(step.Destination.RoutePositionMetres),
				RoomEquivalentCost = (decimal)step.RoomEquivalentCost,
				PinnedTopologyVersion = step.Origin.Cell.RouteDefinition?.TopologyVersion,
				DestinationTopologyVersion = step.Destination.Cell.RouteDefinition?.TopologyVersion
			};
			switch (step)
			{
				case ILinearRoutePathStep linear:
					dbstep.StepType = (int)VehicleRouteStepType.LinearRoute;
					dbstep.DistanceMetres = (decimal)linear.DistanceMetres;
					dbstep.Direction = (int)linear.Direction;
					break;
				case IExitTraversalPathStep exit:
					dbstep.StepType = (int)VehicleRouteStepType.CellExit;
					dbstep.ExitId = exit.Exit.Exit.Id;
					break;
				default:
					steps = Array.Empty<DB.VehicleRouteStep>();
					reason = $"The spatial path contains an unsupported step type {step.GetType().Name}.";
					return false;
			}

			compiledSteps.Add(dbstep);
		}

		steps = compiledSteps;
		reason = string.Empty;
		return true;
	}

	private static decimal? ToDecimal(double? value)
	{
		return value.HasValue ? (decimal)value.Value : null;
	}

	private void RebuildTopologyPins(FuturemudDatabaseContext context)
	{
		var oldPins = context.VehicleRouteTopologyPins
			.Where(x => x.VehicleRouteId == Id && x.VehicleRouteRevision == RevisionNumber)
			.ToList();
		var stepPins = context.VehicleRouteSteps
			.Where(x => x.VehicleRouteLeg.VehicleRouteId == Id &&
				x.VehicleRouteLeg.VehicleRouteRevision == RevisionNumber)
			.Select(x => new
			{
				x.OriginCellId,
				OriginVersion = x.PinnedTopologyVersion,
				x.DestinationCellId,
				x.DestinationTopologyVersion
			})
			.AsEnumerable()
			.SelectMany(x => new[]
			{
				(CellId: x.OriginCellId, Version: x.OriginVersion),
				(CellId: x.DestinationCellId, Version: x.DestinationTopologyVersion)
			})
			.Where(x => x.Version.HasValue)
			.Select(x => (x.CellId, Version: x.Version!.Value))
			.ToList();
		stepPins.AddRange(context.VehicleRouteStops
			.Where(x => x.VehicleRouteId == Id && x.VehicleRouteRevision == RevisionNumber)
			.Select(x => x.CellId)
			.AsEnumerable()
			.Select(x => Gameworld.Cells.Get(x)?.RouteDefinition)
			.Where(x => x is not null)
			.Select(x => (x!.Cell.Id, x.TopologyVersion)));
		var desiredPins = stepPins
			.GroupBy(x => x.CellId)
			.ToDictionary(x => x.Key, x => x.Max(y => y.Version));
		foreach (var pin in oldPins)
		{
			if (desiredPins.Remove(pin.RouteCellId, out var version))
			{
				pin.TopologyVersion = version;
				continue;
			}

			context.VehicleRouteTopologyPins.Remove(pin);
		}

		context.VehicleRouteTopologyPins.AddRange(desiredPins.Select(x => new DB.VehicleRouteTopologyPin
		{
			VehicleRouteId = Id,
			VehicleRouteRevision = RevisionNumber,
			RouteCellId = x.Key,
			TopologyVersion = x.Value
		}));
	}

	public override bool CanSubmit()
	{
		return ValidationErrors().Count == 0;
	}

	public override string WhyCannotSubmit()
	{
		var errors = ValidationErrors();
		return errors.Any()
			? $"This route cannot be submitted:\n{errors.Select(x => $"\t{x}").ListToString(separator: "\n", conjunction: string.Empty, twoItemJoiner: "\n")}"
			: string.Empty;
	}

	public IReadOnlyList<string> ValidationErrors()
	{
		var errors = new List<string>();
		if (_stops.Count < 2)
		{
			errors.Add("It must have at least two stops.");
		}

		if (_stops.Select(x => x.Sequence).Distinct().Count() != _stops.Count)
		{
			errors.Add("Every stop must have a unique sequence number.");
		}

		foreach (var stop in _stops)
		{
			var definition = stop.Location.Cell.RouteDefinition;
			if (definition is null && stop.Location.RoutePositionMetres.HasValue)
			{
				errors.Add($"Stop {stop.Name} supplies a route coordinate in an ordinary cell.");
			}
			else if (definition is not null && (!stop.Location.RoutePositionMetres.HasValue ||
			         stop.Location.RoutePositionMetres < 0.0 ||
			         stop.Location.RoutePositionMetres > definition.LengthMetres))
			{
				errors.Add($"Stop {stop.Name} does not have a valid coordinate in its RouteCell.");
			}

			foreach (var binding in stop.PlatformBindings)
			{
				if (binding.PlatformCell is null || binding.AccessPoint is null ||
				    binding.DockingToleranceMetres < 0.0)
				{
					errors.Add($"Stop {stop.Name} has an invalid platform binding.");
				}
			}
		}

		var ordered = _stops.OrderBy(x => x.Sequence).ToList();
		foreach (var pair in ordered.Zip(ordered.Skip(1)))
		{
			if (_legs.Count(x => x.OriginStop == pair.First && x.DestinationStop == pair.Second) != 1)
			{
				errors.Add($"The leg from {pair.First.Name} to {pair.Second.Name} is not compiled exactly once.");
			}
		}

		if (!TopologyIsCurrent)
		{
			errors.Add("One or more compiled RouteCell topology pins are stale; recompile the affected legs.");
		}

		return errors;
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (Status != RevisionStatus.UnderDesign)
		{
			actor.OutputHandler.Send("Only an under-design route revision can be changed.");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor, command);
			case "stop":
				return BuildingCommandStop(actor, command);
			case "leg":
				return BuildingCommandLeg(actor, command);
			default:
				actor.OutputHandler.Send("Valid route settings are name, description, stop and leg compile.");
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name should this vehicle route have?");
			return false;
		}

		_name = command.SafeRemainingArgument.TitleCase();
		Changed = true;
		actor.OutputHandler.Send($"This route is now named {Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description should this vehicle route have?");
			return false;
		}

		_description = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send("The route description has been updated.");
		return true;
	}

	private bool BuildingCommandStop(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
				return BuildingCommandStopAdd(actor, command);
			case "remove":
				return BuildingCommandStopRemove(actor, command);
			case "move":
				return BuildingCommandStopMove(actor, command);
			case "location":
				return BuildingCommandStopLocation(actor, command);
			case "dwell":
				return BuildingCommandStopDwell(actor, command);
			case "platform":
				return BuildingCommandStopPlatform(actor, command);
			default:
				actor.OutputHandler.Send("Use stop add, remove, move, location, dwell, or platform add/remove.");
				return false;
		}
	}

	private bool BuildingCommandStopAdd(ICharacter actor, StringStack command)
	{
		var name = command.PopSpeech();
		if (string.IsNullOrWhiteSpace(name))
		{
			actor.OutputHandler.Send("What name should the new stop have?");
			return false;
		}

		var location = actor.SpatialLocation;
		if (!command.IsFinished && !TryParseLocation(actor, command, out location))
		{
			return false;
		}
		if (location.Cell.RouteDefinition is not null && !location.RoutePositionMetres.HasValue)
		{
			actor.OutputHandler.Send("A stop in a RouteCell must have an exact route coordinate. Supply a location or move to a valid coordinate first.");
			return false;
		}

		using (new FMDB())
		{
			FMDB.Context.VehicleRouteStops.Add(new DB.VehicleRouteStop
			{
				VehicleRouteId = Id,
				VehicleRouteRevision = RevisionNumber,
				Name = name.TitleCase(),
				Sequence = _stops.Any() ? _stops.Max(x => x.Sequence) + 1 : 0,
				CellId = location.Cell.Id,
				RoomLayer = (int)location.Layer,
				RoutePositionMetres = ToDecimal(location.RoutePositionMetres),
				DwellDurationMilliseconds = 0
			});
			FMDB.Context.SaveChanges();
		}
		ReloadChildren();
		actor.OutputHandler.Send($"You add the {name.TitleCase().ColourName()} stop.");
		return true;
	}

	private bool BuildingCommandStopRemove(ICharacter actor, StringStack command)
	{
		var stop = ResolveStop(command.SafeRemainingArgument);
		if (stop is null)
		{
			actor.OutputHandler.Send("There is no such stop on this route.");
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleRouteStops.Find(stop.Id);
			if (dbitem is not null)
			{
				FMDB.Context.VehicleRouteStops.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
			NormaliseStopSequences(FMDB.Context);
			RebuildTopologyPins(FMDB.Context);
			FMDB.Context.SaveChanges();
		}
		ReloadChildren();
		actor.OutputHandler.Send($"You remove the {stop.Name.ColourName()} stop and its compiled legs.");
		return true;
	}

	private bool BuildingCommandStopMove(ICharacter actor, StringStack command)
	{
		var stop = ResolveStop(command.PopSpeech());
		if (stop is null || !int.TryParse(command.SafeRemainingArgument, out var position) ||
		    position < 1 || position > _stops.Count)
		{
			actor.OutputHandler.Send("Specify a stop and a one-based position within the route.");
			return false;
		}

		var ordered = _stops.OrderBy(x => x.Sequence).ToList();
		ordered.Remove(stop);
		ordered.Insert(position - 1, stop);
		using (new FMDB())
		{
			foreach (var (item, index) in ordered.Select((x, i) => (x, i)))
			{
				FMDB.Context.VehicleRouteStops.Find(item.Id)!.Sequence = index;
			}
			var legs = FMDB.Context.VehicleRouteLegs
				.Where(x => x.VehicleRouteId == Id && x.VehicleRouteRevision == RevisionNumber);
			FMDB.Context.VehicleRouteLegs.RemoveRange(legs);
			FMDB.Context.VehicleRouteTopologyPins.RemoveRange(FMDB.Context.VehicleRouteTopologyPins
				.Where(x => x.VehicleRouteId == Id && x.VehicleRouteRevision == RevisionNumber));
			FMDB.Context.SaveChanges();
		}
		ReloadChildren();
		actor.OutputHandler.Send("The stop order has been updated. Recompile every adjacent leg before submission.");
		return true;
	}

	private bool BuildingCommandStopLocation(ICharacter actor, StringStack command)
	{
		var stop = ResolveStop(command.PopSpeech());
		if (stop is null || !TryParseLocation(actor, command, out var location))
		{
			return false;
		}

		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleRouteStops.Find(stop.Id)!;
			dbitem.CellId = location.Cell.Id;
			dbitem.RoomLayer = (int)location.Layer;
			dbitem.RoutePositionMetres = ToDecimal(location.RoutePositionMetres);
			var legs = FMDB.Context.VehicleRouteLegs.Where(x => x.OriginStopId == stop.Id || x.DestinationStopId == stop.Id);
			FMDB.Context.VehicleRouteLegs.RemoveRange(legs);
			FMDB.Context.SaveChanges();
			RebuildTopologyPins(FMDB.Context);
			FMDB.Context.SaveChanges();
		}
		ReloadChildren();
		actor.OutputHandler.Send($"The {stop.Name.ColourName()} location has been updated; its adjacent legs must be recompiled.");
		return true;
	}

	private bool BuildingCommandStopDwell(ICharacter actor, StringStack command)
	{
		var stop = ResolveStop(command.PopSpeech());
		if (stop is null || !TimeSpanParser.TryParse(command.SafeRemainingArgument, Units.Minutes, Units.Seconds, out var dwell) || dwell < TimeSpan.Zero)
		{
			actor.OutputHandler.Send("Specify a stop and a non-negative dwell duration.");
			return false;
		}

		using (new FMDB())
		{
			FMDB.Context.VehicleRouteStops.Find(stop.Id)!.DwellDurationMilliseconds = (long)dwell.TotalMilliseconds;
			FMDB.Context.SaveChanges();
		}
		ReloadChildren();
		actor.OutputHandler.Send($"The {stop.Name.ColourName()} dwell is now {dwell.Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandStopPlatform(ICharacter actor, StringStack command)
	{
		var operation = command.PopSpeech().ToLowerInvariant();
		var stop = ResolveStop(command.PopSpeech());
		if (stop is null)
		{
			actor.OutputHandler.Send("There is no such stop on this route.");
			return false;
		}

		if (operation == "remove")
		{
			if (!long.TryParse(command.SafeRemainingArgument, out var bindingId) ||
			    stop.PlatformBindings.All(x => x.Id != bindingId))
			{
				actor.OutputHandler.Send("Specify the ID of a platform binding on that stop.");
				return false;
			}

			using (new FMDB())
			{
				FMDB.Context.VehicleRoutePlatformBindings.Remove(
					FMDB.Context.VehicleRoutePlatformBindings.Find(bindingId)!);
				FMDB.Context.SaveChanges();
			}
			ReloadChildren();
			actor.OutputHandler.Send("You remove that platform binding.");
			return true;
		}

		if (operation != "add" || !long.TryParse(command.PopSpeech(), out var cellId) ||
		    Gameworld.Cells.Get(cellId) is not { } platform ||
		    !long.TryParse(command.PopSpeech(), out var accessId))
		{
			actor.OutputHandler.Send("Use stop platform add <stop> <platform cell id> <access-point prototype id> [tolerance metres].");
			return false;
		}

		var access = Gameworld.VehiclePrototypes.SelectMany(x => x.AccessPoints).FirstOrDefault(x => x.Id == accessId);
		if (access is null)
		{
			actor.OutputHandler.Send("There is no such vehicle access-point prototype.");
			return false;
		}

		var tolerance = Gameworld.GetStaticDouble("VehicleRouteDefaultDockingToleranceMetres");
		if (!double.IsFinite(tolerance) || tolerance < 0.0)
		{
			tolerance = 2.0;
		}
		if (!command.IsFinished)
		{
			var toleranceText = command.SafeRemainingArgument;
			if (!double.TryParse(toleranceText, actor, out tolerance))
			{
				if (!actor.Gameworld.UnitManager.TryGetBaseUnits(toleranceText,
						Framework.Units.UnitType.Length, actor, out var baseUnits))
				{
					actor.OutputHandler.Send("Docking tolerance must be a non-negative distance, such as 2m.");
					return false;
				}

				tolerance = baseUnits * actor.Gameworld.UnitManager.BaseHeightToMetres;
			}

			if (!double.IsFinite(tolerance) || tolerance < 0.0)
			{
				actor.OutputHandler.Send("Docking tolerance must be a non-negative distance, such as 2m.");
				return false;
			}
		}

		using (new FMDB())
		{
			FMDB.Context.VehicleRoutePlatformBindings.Add(new DB.VehicleRoutePlatformBinding
			{
				VehicleRouteStopId = stop.Id,
				PlatformCellId = platform.Id,
				VehicleAccessPointProtoId = access.Id,
				DockingToleranceMetres = (decimal)tolerance
			});
			FMDB.Context.SaveChanges();
		}
		ReloadChildren();
		actor.OutputHandler.Send($"You bind platform cell #{platform.Id.ToString("N0", actor)} to {stop.Name.ColourName()} via {access.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandLeg(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().EqualTo("compile"))
		{
			actor.OutputHandler.Send("Use leg compile <origin stop> <destination stop>.");
			return false;
		}

		var origin = ResolveStop(command.PopSpeech());
		var destination = ResolveStop(command.SafeRemainingArgument);
		if (origin is null || destination is null)
		{
			actor.OutputHandler.Send("One or both of those stops do not exist on this route.");
			return false;
		}

		if (!TryCompileLeg(origin, destination, out var reason))
		{
			actor.OutputHandler.Send(reason);
			return false;
		}

		actor.OutputHandler.Send($"You compile the pinned spatial leg from {origin.Name.ColourName()} to {destination.Name.ColourName()}.");
		return true;
	}

	private bool TryParseLocation(ICharacter actor, StringStack command, out SpatialLocation location)
	{
		location = default;
		if (!long.TryParse(command.PopSpeech(), out var cellId) || Gameworld.Cells.Get(cellId) is not { } cell)
		{
			actor.OutputHandler.Send("Specify a valid cell ID.");
			return false;
		}

		var layer = actor.RoomLayer;
		string? positionText = null;
		if (!command.IsFinished)
		{
			var next = command.PopSpeech();
			if (next.EqualTo("at"))
			{
				positionText = command.SafeRemainingArgument;
			}
			else if (Enum.TryParse<RoomLayer>(next, true, out var parsedLayer))
			{
				layer = parsedLayer;
			}
			else if (int.TryParse(next, out var layerValue) && Enum.IsDefined(typeof(RoomLayer), layerValue))
			{
				layer = (RoomLayer)layerValue;
			}
			else if (cell.RouteDefinition is not null)
			{
				positionText = $"{next} {command.SafeRemainingArgument}".Trim();
			}
			else
			{
				actor.OutputHandler.Send("Specify a valid room layer by name or number.");
				return false;
			}

			if (positionText is null && !command.IsFinished)
			{
				if (command.PeekSpeech().EqualTo("at"))
				{
					command.PopSpeech();
				}
				positionText = command.SafeRemainingArgument;
			}
		}

		double? position = null;
		if (cell.RouteDefinition is not null)
		{
			if (string.IsNullOrWhiteSpace(positionText))
			{
				actor.OutputHandler.Send("A stop in a RouteCell must specify its position after AT, for example at 10km.");
				return false;
			}
			if (!Commands.Modules.RouteCommandUtilities.TryResolveRoutePosition(actor, cell, positionText,
				    out var parsedPosition, out var error))
			{
				actor.OutputHandler.Send(error);
				return false;
			}
			position = parsedPosition;
		}
		else if (!string.IsNullOrWhiteSpace(positionText))
		{
			actor.OutputHandler.Send("An ordinary-cell stop cannot have a longitudinal route coordinate.");
			return false;
		}

		location = new SpatialLocation(cell, layer, position);
		return true;
	}

	private IVehicleRouteStop? ResolveStop(string text)
	{
		if (int.TryParse(text, out var ordinal) && ordinal >= 1 && ordinal <= _stops.Count)
		{
			return _stops
				.OrderBy(x => x.Sequence)
				.ElementAt(ordinal - 1);
		}

		return long.TryParse(text, out var id)
			? _stops.FirstOrDefault(x => x.Id == id)
			: _stops.FirstOrDefault(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
	}

	private void NormaliseStopSequences(FuturemudDatabaseContext context)
	{
		var stops = context.VehicleRouteStops
			.Where(x => x.VehicleRouteId == Id && x.VehicleRouteRevision == RevisionNumber)
			.OrderBy(x => x.Sequence)
			.ThenBy(x => x.Id)
			.ToList();
		foreach (var (stop, sequence) in stops.Select((x, index) => (x, index)))
		{
			stop.Sequence = sequence;
		}
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Vehicle Route #{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Status: {Status.DescribeColour()}");
		sb.AppendLine($"Description: {_description}");
		sb.AppendLine($"Topology: {(TopologyIsCurrent ? "current".Colour(Telnet.Green) : "stale".ColourError())}");
		sb.AppendLine("Stops:");
		foreach (var (stop, position) in _stops.OrderBy(x => x.Sequence).Select((x, index) => (x, index + 1)))
		{
			sb.AppendLine($"\t{position.ToString("N0", actor)}. #{stop.Id.ToString("N0", actor)} {stop.Name.ColourName()} - cell #{stop.Location.Cell.Id.ToString("N0", actor)} / {stop.Location.Layer.DescribeEnum().ColourName()}{(stop.Location.RoutePositionMetres.HasValue ? $" at {stop.Location.RoutePositionMetres.Value.ToString("N2", actor).ColourValue()}m" : string.Empty)}; dwell {stop.DwellDuration.Describe(actor).ColourValue()}");
			foreach (var binding in stop.PlatformBindings)
			{
				sb.AppendLine($"\t\tPlatform #{binding.Id.ToString("N0", actor)}: cell #{binding.PlatformCell.Id.ToString("N0", actor)}, {binding.AccessPoint.Name.ColourName()}, tolerance {binding.DockingToleranceMetres.ToString("N2", actor).ColourValue()}m");
			}
		}


		sb.Append(DescribeCompiledRouteDetails(actor, _legs, _topologyPins));

		var errors = ValidationErrors();
		sb.AppendLine(errors.Any() ? "Validation:".ColourError() : "Validation: ready".Colour(Telnet.Green));
		foreach (var error in errors)
		{
			sb.AppendLine($"\t{error.ColourError()}");
		}
		return sb.ToString();
	}

	internal static string DescribeCompiledRouteDetails(ICharacter actor,
		IEnumerable<IVehicleRouteLeg> legs,
		IEnumerable<IVehicleRouteTopologyPin> topologyPins)
	{
		var sb = new StringBuilder("Compiled Legs:\n");
		var orderedLegs = legs
			.OrderBy(x => x.Sequence)
			.ToList();
		if (orderedLegs.Count == 0)
		{
			sb.AppendLine("\t(none)");
		}

		foreach (var leg in orderedLegs)
		{
			sb.AppendLine(
				$"\t#{leg.Id.ToString("N0", actor)} {leg.OriginStop.Name.ColourName()} -> {leg.DestinationStop.Name.ColourName()}: {DescribeRouteMetres(actor, leg.RouteDistanceMetres).ColourValue()}, {leg.RoomEquivalentCost.ToString("N3", actor).ColourValue()} room-equivalents, {leg.Steps.Count.ToString("N0", actor).ColourValue()} steps");
			foreach (var (step, stepNumber) in leg.Steps
				.OrderBy(x => x.Sequence)
				.Select((x, index) => (x, index + 1)))
			{
				switch (step)
				{
					case IVehicleRouteLinearStep linear:
						var direction = linear.Direction == RouteCellDirection.Positive
							? linear.RouteCell.PositiveDirectionName
							: linear.RouteCell.NegativeDirectionName;
						sb.AppendLine(
							$"\t\t{stepNumber.ToString("N0", actor)}. LinearRouteStep #{step.Id.ToString("N0", actor)}: {DescribeSpatialLocation(actor, step.Origin)} -> {DescribeSpatialLocation(actor, step.Destination)}");
						sb.AppendLine(
							$"\t\t   Direction {direction.ColourName()}; distance {DescribeRouteMetres(actor, linear.DistanceMetres).ColourValue()}; cost {linear.RoomEquivalentCost.ToString("N3", actor).ColourValue()} room-equivalents; {DescribeStepTopologyPins(actor, linear)}");
						break;
					case IVehicleRouteExitStep exit:
						sb.AppendLine(
							$"\t\t{stepNumber.ToString("N0", actor)}. CellExitStep #{step.Id.ToString("N0", actor)} via exit #{exit.Exit.Exit.Id.ToString("N0", actor)} ({exit.Exit.OutboundDirectionDescription.ColourName()}): {DescribeSpatialLocation(actor, step.Origin)} -> {DescribeSpatialLocation(actor, step.Destination)}");
						sb.AppendLine(
							$"\t\t   Cost {exit.RoomEquivalentCost.ToString("N3", actor).ColourValue()} room-equivalents; {DescribeStepTopologyPins(actor, exit)}");
						break;
					default:
						sb.AppendLine(
							$"\t\t{stepNumber.ToString("N0", actor)}. Unsupported step #{step.Id.ToString("N0", actor)} ({step.StepType.DescribeEnum()}): {DescribeSpatialLocation(actor, step.Origin)} -> {DescribeSpatialLocation(actor, step.Destination)}");
						break;
				}
			}
		}

		sb.AppendLine("RouteCell Topology Pins:");
		var orderedPins = topologyPins
			.OrderBy(x => x.RouteCell.Id)
			.ToList();
		if (orderedPins.Count == 0)
		{
			sb.AppendLine("\t(none)");
		}

		foreach (var pin in orderedPins)
		{
			var currentVersion = pin.RouteCell.RouteDefinition?.TopologyVersion;
			var state = currentVersion == pin.TopologyVersion
				? "current".Colour(Telnet.Green)
				: "stale".ColourError();
			sb.AppendLine(
				$"\tCell #{pin.RouteCell.Id.ToString("N0", actor)} {pin.RouteCell.Name.ColourName()}: pinned v{pin.TopologyVersion.ToString("N0", actor).ColourValue()}, current v{(currentVersion?.ToString("N0", actor) ?? "missing").ColourValue()} [{state}]");
		}

		return sb.ToString();
	}

	private static string DescribeSpatialLocation(ICharacter actor, SpatialLocation location)
	{
		var coordinate = location.RoutePositionMetres.HasValue
			? $" at {DescribeRouteMetres(actor, location.RoutePositionMetres.Value).ColourValue()}"
			: string.Empty;
		return $"cell #{location.Cell.Id.ToString("N0", actor)} {location.Cell.Name.ColourName()} / {location.Layer.DescribeEnum().ColourName()}{coordinate}";
	}

	private static string DescribeStepTopologyPins(ICharacter actor, IVehicleRouteStep step)
	{
		return $"topology pins: origin {DescribeStepTopologyPin(actor, step.Origin, step.OriginTopologyVersion)}, destination {DescribeStepTopologyPin(actor, step.Destination, step.DestinationTopologyVersion)}";
	}

	private static string DescribeStepTopologyPin(ICharacter actor, SpatialLocation location, long? version)
	{
		if (location.Cell.RouteDefinition is null)
		{
			return "ordinary cell";
		}

		return version.HasValue
			? $"cell #{location.Cell.Id.ToString("N0", actor)} v{version.Value.ToString("N0", actor).ColourValue()}"
			: $"cell #{location.Cell.Id.ToString("N0", actor)} {"un-pinned".ColourError()}";
	}

	private static string DescribeRouteMetres(ICharacter actor, double metres)
	{
		return actor.Gameworld.UnitManager.DescribeMostSignificantExact(
			metres / actor.Gameworld.UnitManager.BaseHeightToMetres,
			UnitType.Length,
			actor);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		using (new FMDB())
		{
			var newRevision = FMDB.Context.VehicleRoutes
				.Where(x => x.Id == Id)
				.Select(x => x.RevisionNumber)
				.AsEnumerable()
				.DefaultIfEmpty(0)
				.Max() + 1;
			var dbnew = new DB.VehicleRoute
			{
				Id = Id,
				RevisionNumber = newRevision,
				Name = Name,
				Description = _description,
				EditableItem = new DB.EditableItem
				{
					BuilderAccountId = initiator.Account.Id,
					BuilderDate = DateTime.UtcNow,
					RevisionNumber = newRevision,
					RevisionStatus = (int)RevisionStatus.UnderDesign
				}
			};
			FMDB.Context.VehicleRoutes.Add(dbnew);
			FMDB.Context.SaveChanges();

			var stopMap = new Dictionary<long, DB.VehicleRouteStop>();
			foreach (var stop in _stops)
			{
				var dbstop = new DB.VehicleRouteStop
				{
					VehicleRouteId = Id,
					VehicleRouteRevision = newRevision,
					Name = stop.Name,
					Sequence = stop.Sequence,
					CellId = stop.Location.Cell.Id,
					RoomLayer = (int)stop.Location.Layer,
					RoutePositionMetres = ToDecimal(stop.Location.RoutePositionMetres),
					DwellDurationMilliseconds = (long)stop.DwellDuration.TotalMilliseconds
				};
				FMDB.Context.VehicleRouteStops.Add(dbstop);
				stopMap[stop.Id] = dbstop;
			}
			FMDB.Context.SaveChanges();

			foreach (var stop in _stops)
			{
				foreach (var binding in stop.PlatformBindings)
				{
					FMDB.Context.VehicleRoutePlatformBindings.Add(new DB.VehicleRoutePlatformBinding
					{
						VehicleRouteStopId = stopMap[stop.Id].Id,
						PlatformCellId = binding.PlatformCell.Id,
						VehicleAccessPointProtoId = binding.AccessPoint.Id,
						DockingToleranceMetres = (decimal)binding.DockingToleranceMetres
					});
				}
			}

			foreach (var leg in _legs)
			{
				var dbleg = new DB.VehicleRouteLeg
				{
					VehicleRouteId = Id,
					VehicleRouteRevision = newRevision,
					Sequence = leg.Sequence,
					OriginStopId = stopMap[leg.OriginStop.Id].Id,
					DestinationStopId = stopMap[leg.DestinationStop.Id].Id,
					RouteDistanceMetres = (decimal)leg.RouteDistanceMetres,
					RoomEquivalentCost = (decimal)leg.RoomEquivalentCost
				};
				foreach (var step in leg.Steps)
				{
					var dbstep = new DB.VehicleRouteStep
					{
						Sequence = step.Sequence,
						StepType = (int)step.StepType,
						OriginCellId = step.Origin.Cell.Id,
						OriginRoomLayer = (int)step.Origin.Layer,
						OriginRoutePositionMetres = ToDecimal(step.Origin.RoutePositionMetres),
						DestinationCellId = step.Destination.Cell.Id,
						DestinationRoomLayer = (int)step.Destination.Layer,
						DestinationRoutePositionMetres = ToDecimal(step.Destination.RoutePositionMetres),
						RoomEquivalentCost = (decimal)step.RoomEquivalentCost,
						PinnedTopologyVersion = step.OriginTopologyVersion,
						DestinationTopologyVersion = step.DestinationTopologyVersion
					};
					if (step is IVehicleRouteLinearStep linear)
					{
						dbstep.DistanceMetres = (decimal)linear.DistanceMetres;
						dbstep.Direction = (int)linear.Direction;
					}
					else if (step is IVehicleRouteExitStep exit)
					{
						dbstep.ExitId = exit.Exit.Exit.Id;
					}
					dbleg.Steps.Add(dbstep);
				}
				FMDB.Context.VehicleRouteLegs.Add(dbleg);
			}

			foreach (var pin in _topologyPins)
			{
				FMDB.Context.VehicleRouteTopologyPins.Add(new DB.VehicleRouteTopologyPin
				{
					VehicleRouteId = Id,
					VehicleRouteRevision = newRevision,
					RouteCellId = pin.RouteCell.Id,
					TopologyVersion = pin.TopologyVersion
				});
			}
			FMDB.Context.SaveChanges();

			return new VehicleRoute(FMDB.Context.VehicleRoutes
				.Include(x => x.EditableItem)
				.Include(x => x.Stops).ThenInclude(x => x.PlatformBindings)
				.Include(x => x.Legs).ThenInclude(x => x.Steps)
				.Include(x => x.TopologyPins)
				.AsNoTracking()
				.First(x => x.Id == Id && x.RevisionNumber == newRevision), Gameworld);
		}
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.VehicleRoutes
				.Include(x => x.EditableItem)
				.First(x => x.Id == Id && x.RevisionNumber == RevisionNumber);
			Save(dbitem.EditableItem);
			dbitem.Name = Name;
			dbitem.Description = _description;
			FMDB.Context.SaveChanges();
		}
		Changed = false;
	}

	public IProgVariable GetProperty(string property)
	{
		return property.ToLowerInvariant() switch
		{
			"id" => new NumberVariable(Id),
			"name" => new TextVariable(Name),
			"revision" => new NumberVariable(RevisionNumber),
			"status" => new TextVariable(Status.DescribeEnum()),
			"stopcount" => new NumberVariable(_stops.Count),
			"legcount" => new NumberVariable(_legs.Count),
			"topologyvalid" => new BooleanVariable(TopologyIsCurrent),
			_ => throw new NotSupportedException($"Invalid VehicleRoute property {property}.")
		};
	}

	public ProgVariableTypes Type => ProgVariableTypes.VehicleRoute;
	public object GetObject => this;

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.VehicleRoute,
			new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
			{
				["id"] = ProgVariableTypes.Number,
				["name"] = ProgVariableTypes.Text,
				["revision"] = ProgVariableTypes.Number,
				["status"] = ProgVariableTypes.Text,
				["stopcount"] = ProgVariableTypes.Number,
				["legcount"] = ProgVariableTypes.Number,
				["topologyvalid"] = ProgVariableTypes.Boolean
			},
			new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
			{
				["id"] = "The stable identity of this vehicle route.",
				["name"] = "The builder-authored route name.",
				["revision"] = "The immutable route revision number.",
				["status"] = "The revision status.",
				["stopcount"] = "The number of ordered stops.",
				["legcount"] = "The number of compiled legs.",
				["topologyvalid"] = "True when every pinned RouteCell topology version is still current."
			});
	}
}

public sealed class VehicleRouteStop : FrameworkItem, IVehicleRouteStop
{
	private readonly SpatialLocation _location;
	private readonly IReadOnlyCollection<IVehicleRoutePlatformBinding> _platformBindings;

	public VehicleRouteStop(VehicleRoute route, DB.VehicleRouteStop dbitem, IFuturemud gameworld)
	{
		Route = route;
		_id = dbitem.Id;
		_name = dbitem.Name;
		Sequence = dbitem.Sequence;
		DwellDuration = TimeSpan.FromMilliseconds(dbitem.DwellDurationMilliseconds);
		_location = new SpatialLocation(
			gameworld.Cells.Get(dbitem.CellId) ??
				throw new InvalidOperationException($"Vehicle route stop #{dbitem.Id:N0} references missing cell #{dbitem.CellId:N0}."),
			(RoomLayer)dbitem.RoomLayer,
			dbitem.RoutePositionMetres is null ? null : (double)dbitem.RoutePositionMetres.Value);
		_platformBindings = dbitem.PlatformBindings
			.OrderBy(x => x.Id)
			.Select(x => (IVehicleRoutePlatformBinding)new VehicleRoutePlatformBinding(this, x, gameworld))
			.ToArray();
	}

	public override string FrameworkItemType => "VehicleRouteStop";
	public IVehicleRoute Route { get; }
	public int Sequence { get; }
	public SpatialLocation Location => _location;
	public TimeSpan DwellDuration { get; }
	public IReadOnlyCollection<IVehicleRoutePlatformBinding> PlatformBindings => _platformBindings;
}

public sealed class VehicleRoutePlatformBinding : FrameworkItem, IVehicleRoutePlatformBinding
{
	public VehicleRoutePlatformBinding(VehicleRouteStop stop, DB.VehicleRoutePlatformBinding dbitem, IFuturemud gameworld)
	{
		Stop = stop;
		_id = dbitem.Id;
		_name = $"Platform Binding #{dbitem.Id:N0}";
		PlatformCell = gameworld.Cells.Get(dbitem.PlatformCellId) ??
			throw new InvalidOperationException($"Vehicle route platform binding #{dbitem.Id:N0} references missing cell #{dbitem.PlatformCellId:N0}.");
		AccessPoint = gameworld.VehiclePrototypes
			.SelectMany(x => x.AccessPoints)
			.FirstOrDefault(x => x.Id == dbitem.VehicleAccessPointProtoId) ??
			throw new InvalidOperationException($"Vehicle route platform binding #{dbitem.Id:N0} references missing access-point prototype #{dbitem.VehicleAccessPointProtoId:N0}.");
		DockingToleranceMetres = (double)dbitem.DockingToleranceMetres;
	}

	public override string FrameworkItemType => "VehicleRoutePlatformBinding";
	public IVehicleRouteStop Stop { get; }
	public ICell PlatformCell { get; }
	public IVehicleAccessPointPrototype AccessPoint { get; }
	public double DockingToleranceMetres { get; }
}

public sealed record VehicleRouteTopologyPin(ICell RouteCell, long TopologyVersion) : IVehicleRouteTopologyPin;

public sealed class VehicleRouteLeg : FrameworkItem, IVehicleRouteLeg
{
	private readonly IReadOnlyList<IVehicleRouteStep> _steps;

	public VehicleRouteLeg(VehicleRoute route, DB.VehicleRouteLeg dbitem,
		IReadOnlyCollection<VehicleRouteStop> stops, IFuturemud gameworld)
	{
		Route = route;
		_id = dbitem.Id;
		_name = $"Vehicle Route Leg #{dbitem.Id:N0}";
		Sequence = dbitem.Sequence;
		OriginStop = stops.First(x => x.Id == dbitem.OriginStopId);
		DestinationStop = stops.First(x => x.Id == dbitem.DestinationStopId);
		RouteDistanceMetres = (double)dbitem.RouteDistanceMetres;
		RoomEquivalentCost = (double)dbitem.RoomEquivalentCost;
		_steps = dbitem.Steps
			.OrderBy(x => x.Sequence)
			.Select(x => x.StepType == (int)VehicleRouteStepType.LinearRoute
				? (IVehicleRouteStep)new VehicleRouteLinearStep(this, x, gameworld)
				: new VehicleRouteExitStep(this, x, gameworld))
			.ToArray();
	}

	public override string FrameworkItemType => "VehicleRouteLeg";
	public IVehicleRoute Route { get; }
	public int Sequence { get; }
	public IVehicleRouteStop OriginStop { get; }
	public IVehicleRouteStop DestinationStop { get; }
	public IReadOnlyList<IVehicleRouteStep> Steps => _steps;
	public double RouteDistanceMetres { get; }
	public double RoomEquivalentCost { get; }
}

public abstract class VehicleRouteStep : FrameworkItem, IVehicleRouteStep
{
	protected VehicleRouteStep(VehicleRouteLeg leg, DB.VehicleRouteStep dbitem, IFuturemud gameworld)
	{
		Leg = leg;
		_id = dbitem.Id;
		_name = $"Vehicle Route Step #{dbitem.Id:N0}";
		Sequence = dbitem.Sequence;
		StepType = (VehicleRouteStepType)dbitem.StepType;
		Origin = new SpatialLocation(gameworld.Cells.Get(dbitem.OriginCellId) ??
			throw new InvalidOperationException($"Vehicle route step #{dbitem.Id:N0} references missing origin cell #{dbitem.OriginCellId:N0}."),
			(RoomLayer)dbitem.OriginRoomLayer,
			dbitem.OriginRoutePositionMetres is null ? null : (double)dbitem.OriginRoutePositionMetres.Value);
		Destination = new SpatialLocation(gameworld.Cells.Get(dbitem.DestinationCellId) ??
			throw new InvalidOperationException($"Vehicle route step #{dbitem.Id:N0} references missing destination cell #{dbitem.DestinationCellId:N0}."),
			(RoomLayer)dbitem.DestinationRoomLayer,
			dbitem.DestinationRoutePositionMetres is null ? null : (double)dbitem.DestinationRoutePositionMetres.Value);
		RoomEquivalentCost = (double)dbitem.RoomEquivalentCost;
		OriginTopologyVersion = dbitem.PinnedTopologyVersion;
		DestinationTopologyVersion = dbitem.DestinationTopologyVersion;
	}

	public override string FrameworkItemType => "VehicleRouteStep";
	public IVehicleRouteLeg Leg { get; }
	public int Sequence { get; }
	public VehicleRouteStepType StepType { get; }
	public SpatialLocation Origin { get; }
	public SpatialLocation Destination { get; }
	public double RoomEquivalentCost { get; }
	public long? OriginTopologyVersion { get; }
	public long? DestinationTopologyVersion { get; }
}

public sealed class VehicleRouteLinearStep : VehicleRouteStep, IVehicleRouteLinearStep
{
	public VehicleRouteLinearStep(VehicleRouteLeg leg, DB.VehicleRouteStep dbitem, IFuturemud gameworld)
		: base(leg, dbitem, gameworld)
	{
		RouteCell = Origin.Cell.RouteDefinition!;
		Direction = (RouteCellDirection)dbitem.Direction!.Value;
		DistanceMetres = (double)dbitem.DistanceMetres!.Value;
		PinnedTopologyVersion = OriginTopologyVersion!.Value;
	}

	public IRouteCellDefinition RouteCell { get; }
	public RouteCellDirection Direction { get; }
	public double DistanceMetres { get; }
	public long PinnedTopologyVersion { get; }
}

public sealed class VehicleRouteExitStep : VehicleRouteStep, IVehicleRouteExitStep
{
	public VehicleRouteExitStep(VehicleRouteLeg leg, DB.VehicleRouteStep dbitem, IFuturemud gameworld)
		: base(leg, dbitem, gameworld)
	{
		Exit = LoadExit(dbitem, gameworld);
	}

	public ICellExit Exit { get; }

	private ICellExit LoadExit(DB.VehicleRouteStep dbitem, IFuturemud gameworld)
	{
		if (dbitem.ExitId is not { } exitId)
		{
			throw new InvalidOperationException(
				$"Vehicle route exit step #{dbitem.Id:N0} has no persisted exit ID.");
		}

		var exit = gameworld.ExitManager.GetExitByID(exitId);
		if (exit is null)
		{
			// Ordinary exits are loaded lazily. Vehicle routes load before the general exit cache has necessarily
			// initialised this cell, so force topology-wide initialisation rather than using actor-filtered exits.
			var cellExit = gameworld.ExitManager
				.GetAllExits(Origin.Cell)
				.FirstOrDefault(x => x.Exit.Id == exitId);
			if (cellExit is not null)
			{
				return cellExit;
			}

			exit = gameworld.ExitManager.GetExitByID(exitId);
		}

		if (exit is null)
		{
			throw new InvalidOperationException(
				$"Vehicle route exit step #{dbitem.Id:N0} references missing exit #{exitId:N0} from origin cell #{Origin.Cell.Id:N0}.");
		}

		if (exit.Cells.All(x => x.Id != Origin.Cell.Id))
		{
			throw new InvalidOperationException(
				$"Vehicle route exit step #{dbitem.Id:N0} references exit #{exitId:N0}, which is not connected to origin cell #{Origin.Cell.Id:N0}.");
		}

		return exit.CellExitFor(Origin.Cell) ??
		       throw new InvalidOperationException(
			       $"Vehicle route exit step #{dbitem.Id:N0} could not resolve exit #{exitId:N0} from origin cell #{Origin.Cell.Id:N0}.");
	}
}
