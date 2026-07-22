#nullable enable

using System.Data;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using DB = MudSharp.Models;

namespace MudSharp.Commands.Modules;

internal partial class RoomBuilderModule
{
	private const string RouteCellSetHelp = @"RouteCell authoring commands:

	#3cell set route create <length>#0
	#3cell set route clear#0
	#3cell set route length <length>#0
	#3cell set route default <distance|landmark>#0
	#3cell set route direction positive|negative <name>#0
	#3cell set route roomequivalent <length|default>#0
	#3cell set route landmark add <distance> <name>#0
	#3cell set route landmark rename|keywords|distance|description|delete <landmark> ...#0
	#3cell set route exit <exit> band <minimum> <maximum> arrival <distance>#0
	#3cell set route show#0
	#3cell set route map#0
	#3cell set route validate#0";

	private static void CellSetRoute(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || command.Peek().EqualTo("help") || command.Peek().EqualTo("?"))
		{
			actor.OutputHandler.Send(RouteCellSetHelp.SubstituteANSIColour());
			return;
		}

		if (actor.Location is not MudSharp.Construction.Cell cell)
		{
			actor.OutputHandler.Send("This cell implementation cannot host RouteCell geometry.");
			return;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "create":
			case "new":
				CellSetRouteCreate(actor, cell, command);
				return;
			case "clear":
			case "remove":
			case "delete":
				CellSetRouteClear(actor, cell);
				return;
			case "length":
				CellSetRouteLength(actor, cell, command);
				return;
			case "default":
				CellSetRouteDefault(actor, cell, command);
				return;
			case "direction":
				CellSetRouteDirection(actor, cell, command);
				return;
			case "roomequivalent":
			case "room-equivalent":
			case "room":
				CellSetRouteRoomEquivalent(actor, cell, command);
				return;
			case "landmark":
			case "landmarks":
				CellSetRouteLandmark(actor, cell, command);
				return;
			case "exit":
			case "portal":
				CellSetRouteExit(actor, cell, command);
				return;
			case "show":
			case "view":
				CellSetRouteShow(actor, cell);
				return;
			case "map":
				CellSetRouteMap(actor, cell);
				return;
			case "validate":
			case "audit":
				CellSetRouteValidate(actor, cell);
				return;
			default:
				actor.OutputHandler.Send(RouteCellSetHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void CellSetRouteCreate(ICharacter actor, MudSharp.Construction.Cell cell, StringStack command)
	{
		if (cell.RouteDefinition is not null)
		{
			actor.OutputHandler.Send("This cell is already a RouteCell.");
			return;
		}

		if (!TryParseRouteMetres(actor, command.SafeRemainingArgument, out var length) || length <= 0.0)
		{
			actor.OutputHandler.Send("You must specify a positive, finite RouteCell length using a valid unit.");
			return;
		}

		var blockers = new List<string>();
		if (cell.Characters.Any(x => !ReferenceEquals(x, actor)))
		{
			AddRouteMutationBlocker(blockers, "other live characters");
		}

		if (cell.GameItems.Any())
		{
			AddRouteMutationBlocker(blockers, "top-level items");
		}

		if (actor.Gameworld.Vehicles.Any(x => ReferenceEquals(x.Location, cell)))
		{
			AddRouteMutationBlocker(blockers, "vehicles");
		}

		if (cell.LocalProjects.Any())
		{
			AddRouteMutationBlocker(blockers, "projects");
		}

		if (cell.Tracks.Any())
		{
			AddRouteMutationBlocker(blockers, "tracks");
		}

		if (cell.HasPointSurfaceLiquid)
		{
			AddRouteMutationBlocker(blockers, "coordinate-bound surface liquid");
		}

		if (actor.Gameworld.ExitManager.GetAllExits(cell).Any())
		{
			AddRouteMutationBlocker(blockers, "unanchored exits");
		}

		var actorIdentity = RouteMutationActor(actor);
		using (new FMDB())
		using (var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable))
		{
			var persisted = RouteCellMutationSafety.InspectOccupancy(FMDB.Context, cell.Id, actorIdentity);
			if (persisted.HasOtherCharacters)
			{
				AddRouteMutationBlocker(blockers, "other persisted character locations");
			}
			if (persisted.HasTopLevelItems)
			{
				AddRouteMutationBlocker(blockers, "top-level items");
			}
			if (persisted.HasVehicles)
			{
				AddRouteMutationBlocker(blockers, "vehicles");
			}
			if (persisted.HasProjects)
			{
				AddRouteMutationBlocker(blockers, "projects");
			}
			if (persisted.HasTracks)
			{
				AddRouteMutationBlocker(blockers, "tracks");
			}
			if (persisted.HasPointSurfaceLiquid)
			{
				AddRouteMutationBlocker(blockers, "coordinate-bound surface liquid");
			}

			if (blockers.Any())
			{
				actor.OutputHandler.Send(
					$"This cell cannot become a RouteCell while it contains {blockers.ListToString()}. Clear those blockers first.");
				return;
			}

			FMDB.Context.RouteCells.Add(new DB.RouteCell
			{
				CellId = cell.Id,
				LengthMetres = ToPersistedMetres(length),
				DefaultPositionMetres = 0.000M,
				PositiveDirectionName = "forward",
				NegativeDirectionName = "backward",
				MetresPerRoomEquivalent = ToPersistedMetres(
					actor.Gameworld.GetStaticDouble("RouteCellDefaultRoomEquivalentMetres")),
				TopologyVersion = 1L
			});
			RouteCellMutationSafety.PersistActorSpatialState(
				FMDB.Context,
				actorIdentity,
				cell.Id,
				actor.RoomLayer,
				0.0);
			FMDB.Context.SaveChanges();
			transaction.Commit();
		}

		ReloadRouteDefinition(cell);
		SetCommittedActorRoutePosition(actor, 0.0);
		actor.OutputHandler.Send(
			$"You convert this cell into a {DescribeRouteMetres(actor, length).ColourValue()} linear RouteCell at topology version {1L.ToString("N0", actor).ColourValue()}.");
	}

	private static void CellSetRouteClear(ICharacter actor, MudSharp.Construction.Cell cell)
	{
		var route = cell.RouteDefinition;
		if (route is null)
		{
			actor.OutputHandler.Send("This is already an ordinary cell.");
			return;
		}

		var actorIdentity = RouteMutationActor(actor);
		using (new FMDB())
		using (var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable))
		{
			var blockers = RouteRemovalBlockers(actor, cell, route, FMDB.Context);
			if (blockers.Any())
			{
				actor.OutputHandler.Send(
					$"You cannot clear this RouteCell while it contains {blockers.ListToString()}. Remove those dependencies first.");
				return;
			}

			RouteCellMutationSafety.PersistActorSpatialState(
				FMDB.Context,
				actorIdentity,
				cell.Id,
				actor.RoomLayer,
				null);
			var dbroute = FMDB.Context.RouteCells.Find(cell.Id);
			if (dbroute is not null)
			{
				FMDB.Context.RouteCells.Remove(dbroute);
			}
			FMDB.Context.SaveChanges();
			transaction.Commit();
		}

		cell.ReloadRouteDefinition(null);
		SetCommittedActorRoutePosition(actor, null);
		actor.OutputHandler.Send("You clear the RouteCell geometry. This is now an ordinary cell.");
	}

	private static List<string> RouteRemovalBlockers(
		ICharacter actor,
		MudSharp.Construction.Cell cell,
		IRouteCellDefinition route,
		FuturemudDatabaseContext context)
	{
		var blockers = new List<string>();
		if (cell.Characters.Any(x => !ReferenceEquals(x, actor)))
		{
			AddRouteMutationBlocker(blockers, "other live characters");
		}

		if (cell.GameItems.Any())
		{
			AddRouteMutationBlocker(blockers, "top-level items");
		}

		if (actor.Gameworld.Vehicles.Any(x => ReferenceEquals(x.Location, cell)))
		{
			AddRouteMutationBlocker(blockers, "vehicles");
		}

		if (cell.LocalProjects.Any())
		{
			AddRouteMutationBlocker(blockers, "projects");
		}

		if (cell.Tracks.Any())
		{
			AddRouteMutationBlocker(blockers, "tracks");
		}

		if (cell.HasPointSurfaceLiquid)
		{
			AddRouteMutationBlocker(blockers, "coordinate-bound surface liquid");
		}

		if (route.Landmarks.Any())
		{
			AddRouteMutationBlocker(blockers, "landmarks");
		}

		if (route.ExitAnchors.Any())
		{
			AddRouteMutationBlocker(blockers, "anchored exits");
		}

		var persisted = RouteCellMutationSafety.InspectOccupancy(
			context,
			cell.Id,
			RouteMutationActor(actor));
		if (persisted.HasOtherCharacters)
		{
			AddRouteMutationBlocker(blockers, "other persisted character locations");
		}
		if (persisted.HasTopLevelItems)
		{
			AddRouteMutationBlocker(blockers, "top-level items");
		}
		if (persisted.HasVehicles)
		{
			AddRouteMutationBlocker(blockers, "vehicles");
		}
		if (persisted.HasProjects)
		{
			AddRouteMutationBlocker(blockers, "projects");
		}
		if (persisted.HasTracks)
		{
			AddRouteMutationBlocker(blockers, "tracks");
		}
		if (persisted.HasPointSurfaceLiquid)
		{
			AddRouteMutationBlocker(blockers, "coordinate-bound surface liquid");
		}

		if (context.ActiveRouteMotions.Any(x => x.RouteCellId == cell.Id))
		{
			AddRouteMutationBlocker(blockers, "active route motions");
		}

		if (context.VehicleRouteStops.Any(x => x.CellId == cell.Id) ||
			context.VehicleRouteTopologyPins.Any(x => x.RouteCellId == cell.Id))
		{
			AddRouteMutationBlocker(blockers, "vehicle routes, stops, services, or journeys");
		}

		return blockers;
	}

	private static void CellSetRouteLength(ICharacter actor, MudSharp.Construction.Cell cell, StringStack command)
	{
		if (!RequireRoute(actor, cell, out var route))
		{
			return;
		}

		if (!TryParseRouteMetres(actor, command.SafeRemainingArgument, out var length) || length <= 0.0)
		{
			actor.OutputHandler.Send("You must specify a positive, finite RouteCell length using a valid unit.");
			return;
		}

		using (new FMDB())
		using (var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable))
		{
			if (length < route.LengthMetres)
			{
				var blockers = RouteLengthBlockers(actor, cell, route, length, FMDB.Context);
				if (blockers.Any())
				{
					actor.OutputHandler.Send(
						$"You cannot shorten this RouteCell to {DescribeRouteMetres(actor, length).ColourValue()} because it would strand {blockers.ListToString()}.");
					return;
				}
			}

			var dbroute = FMDB.Context.RouteCells.Single(x => x.CellId == cell.Id);
			dbroute.LengthMetres = ToPersistedMetres(length);
			dbroute.TopologyVersion++;
			FMDB.Context.SaveChanges();
			transaction.Commit();
		}

		ReloadRouteDefinition(cell);
		actor.OutputHandler.Send(
			$"You set the RouteCell length to {DescribeRouteMetres(actor, length).ColourValue()} and increment its topology version.");
	}

	private static List<string> RouteLengthBlockers(
		ICharacter actor,
		MudSharp.Construction.Cell cell,
		IRouteCellDefinition route,
		double length,
		FuturemudDatabaseContext context)
	{
		var blockers = new List<string>();
		if (route.DefaultPositionMetres > length)
		{
			AddRouteMutationBlocker(blockers, "the default coordinate");
		}

		if (route.Landmarks.Any(x => x.PositionMetres > length))
		{
			AddRouteMutationBlocker(blockers, "one or more landmarks");
		}

		if (route.ExitAnchors.Any(x => x.MaximumPositionMetres > length || x.ArrivalPositionMetres > length))
		{
			AddRouteMutationBlocker(blockers, "one or more exit anchors");
		}

		if (cell.Perceivables.OfType<ILocateable>().Any(x => x.RoutePositionMetres > length))
		{
			AddRouteMutationBlocker(blockers, "one or more live entities");
		}

		if (actor.Gameworld.Vehicles.Any(x => ReferenceEquals(x.Location, cell) && x.RoutePositionMetres > length))
		{
			AddRouteMutationBlocker(blockers, "one or more vehicles");
		}

		if (cell.Tracks.Any(x => x.RoutePositionMetres > length))
		{
			AddRouteMutationBlocker(blockers, "one or more tracks");
		}

		if (cell.HasPointSurfaceLiquidBeyond(length))
		{
			AddRouteMutationBlocker(blockers, "coordinate-bound surface liquid");
		}

		var persisted = RouteCellMutationSafety.InspectLength(context, cell.Id, length);
		if (persisted.HasCharactersBeyondLength)
		{
			AddRouteMutationBlocker(blockers, "one or more persisted character locations");
		}
		if (persisted.HasTopLevelItemsBeyondLength)
		{
			AddRouteMutationBlocker(blockers, "one or more persisted top-level items");
		}
		if (persisted.HasVehiclesBeyondLength)
		{
			AddRouteMutationBlocker(blockers, "one or more vehicles");
		}
		if (persisted.HasProjectsBeyondLength)
		{
			AddRouteMutationBlocker(blockers, "one or more persisted projects");
		}
		if (persisted.HasTracksBeyondLength)
		{
			AddRouteMutationBlocker(blockers, "one or more tracks");
		}
		if (persisted.HasPointSurfaceLiquidBeyondLength)
		{
			AddRouteMutationBlocker(blockers, "coordinate-bound surface liquid");
		}

		if (context.ActiveRouteMotions.Any(x =>
			x.RouteCellId == cell.Id &&
			(x.CheckpointPositionMetres > (decimal)length || x.TargetMaximumPositionMetres > (decimal)length)))
		{
			AddRouteMutationBlocker(blockers, "an active route motion");
		}

		if (context.VehicleRouteStops.Any(x =>
			x.CellId == cell.Id && x.RoutePositionMetres > (decimal)length) ||
			context.VehicleRouteSteps.Any(x =>
				(x.OriginCellId == cell.Id && x.OriginRoutePositionMetres > (decimal)length) ||
				(x.DestinationCellId == cell.Id && x.DestinationRoutePositionMetres > (decimal)length)))
		{
			AddRouteMutationBlocker(blockers, "a vehicle route stop or compiled route step");
		}

		return blockers;
	}

	private static void CellSetRouteDefault(ICharacter actor, MudSharp.Construction.Cell cell, StringStack command)
	{
		if (!RequireRoute(actor, cell, out var route))
		{
			return;
		}

		if (command.IsFinished || !TryResolveRoutePosition(actor, route, command.SafeRemainingArgument, out var position))
		{
			actor.OutputHandler.Send("Specify a coordinate or one of this RouteCell's landmarks.");
			return;
		}

		MutateRouteDefinition(cell, dbroute => dbroute.DefaultPositionMetres = ToPersistedMetres(position));
		actor.OutputHandler.Send(
			$"You set the default RouteCell coordinate to {DescribeRouteMetres(actor, position).ColourValue()}.");
	}

	private static void CellSetRouteDirection(ICharacter actor, MudSharp.Construction.Cell cell, StringStack command)
	{
		if (!RequireRoute(actor, cell, out _))
		{
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to rename the positive or negative direction?");
			return;
		}

		var direction = command.PopSpeech().ToLowerInvariant();
		if (direction is not ("positive" or "negative" or "+" or "-"))
		{
			actor.OutputHandler.Send("The direction must be positive or negative.");
			return;
		}

		var name = command.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
		{
			actor.OutputHandler.Send("Specify a direction name no longer than 100 characters.");
			return;
		}

		MutateRouteDefinition(cell, dbroute =>
		{
			if (direction is "positive" or "+")
			{
				dbroute.PositiveDirectionName = name;
			}
			else
			{
				dbroute.NegativeDirectionName = name;
			}
		});
		actor.OutputHandler.Send($"You rename the {direction.ColourName()} RouteCell direction to {name.ColourName()}.");
	}

	private static void CellSetRouteRoomEquivalent(ICharacter actor, MudSharp.Construction.Cell cell, StringStack command)
	{
		if (!RequireRoute(actor, cell, out _))
		{
			return;
		}

		var text = command.SafeRemainingArgument;
		var value = text.EqualTo("default")
			? actor.Gameworld.GetStaticDouble("RouteCellDefaultRoomEquivalentMetres")
			: TryParseRouteMetres(actor, text, out var parsed) ? parsed : double.NaN;
		if (!double.IsFinite(value) || value <= 0.0)
		{
			actor.OutputHandler.Send("Specify a positive distance or DEFAULT for the room-equivalent scale.");
			return;
		}

		MutateRouteDefinition(cell, dbroute => dbroute.MetresPerRoomEquivalent = ToPersistedMetres(value));
		actor.OutputHandler.Send(
			$"You set this RouteCell's room-equivalent scale to {DescribeRouteMetres(actor, value).ColourValue()}.");
	}

	private static void CellSetRouteLandmark(ICharacter actor, MudSharp.Construction.Cell cell, StringStack command)
	{
		if (!RequireRoute(actor, cell, out var route))
		{
			return;
		}

		if (command.IsFinished)
		{
			CellSetRouteShow(actor, cell);
			return;
		}

		var action = command.PopSpeech().ToLowerInvariant();
		if (action is "add" or "new" or "create")
		{
			var distanceText = command.PopSpeech();
			if (!TryParseRouteMetres(actor, distanceText, out var position) ||
				position < 0.0 ||
				position > route.LengthMetres)
			{
				actor.OutputHandler.Send("Specify a landmark coordinate inside this RouteCell.");
				return;
			}

			var name = command.SafeRemainingArgument.Trim();
			if (string.IsNullOrWhiteSpace(name) || name.Length > 200)
			{
				actor.OutputHandler.Send("Specify a landmark name no longer than 200 characters.");
				return;
			}

			using (new FMDB())
			{
				var dbroute = FMDB.Context.RouteCells
					.Include(x => x.Landmarks)
					.Single(x => x.CellId == cell.Id);
				dbroute.Landmarks.Add(new DB.RouteCellLandmark
				{
					Name = name,
					Keywords = name.ToLowerInvariant(),
					Description = string.Empty,
					PositionMetres = ToPersistedMetres(position),
					DisplayOrder = dbroute.Landmarks.Select(x => x.DisplayOrder).DefaultIfEmpty().Max() + 1
				});
				dbroute.TopologyVersion++;
				FMDB.Context.SaveChanges();
			}

			ReloadRouteDefinition(cell);
			actor.OutputHandler.Send(
				$"You add the RouteCell landmark {name.ColourName()} at {DescribeRouteMetres(actor, position).ColourValue()}.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which RouteCell landmark do you want to edit?");
			return;
		}

		var selector = command.PopSpeech();
		var landmark = ResolveRouteLandmark(route, selector);
		if (landmark is null)
		{
			actor.OutputHandler.Send($"There is no RouteCell landmark identified by {selector.ColourCommand()}.");
			return;
		}

		switch (action)
		{
			case "rename":
			case "name":
			{
				var name = command.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(name) || name.Length > 200)
				{
					actor.OutputHandler.Send("Specify a landmark name no longer than 200 characters.");
					return;
				}

				MutateLandmark(cell, landmark.Id, x => x.Name = name);
				actor.OutputHandler.Send($"You rename the RouteCell landmark to {name.ColourName()}.");
				return;
			}
			case "keywords":
			case "keyword":
			{
				var keywords = command.SafeRemainingArgument.Trim();
				if (string.IsNullOrWhiteSpace(keywords) || keywords.Length > 500)
				{
					actor.OutputHandler.Send("Specify one or more keywords totalling no more than 500 characters.");
					return;
				}

				MutateLandmark(cell, landmark.Id, x => x.Keywords = keywords);
				actor.OutputHandler.Send($"You set the landmark keywords to {keywords.ColourCommand()}.");
				return;
			}
			case "distance":
			case "position":
			{
				if (!TryParseRouteMetres(actor, command.SafeRemainingArgument, out var position) ||
					position < 0.0 ||
					position > route.LengthMetres)
				{
					actor.OutputHandler.Send("Specify a landmark coordinate inside this RouteCell.");
					return;
				}

				MutateLandmark(cell, landmark.Id, x => x.PositionMetres = ToPersistedMetres(position));
				actor.OutputHandler.Send(
					$"You move {landmark.Name.ColourName()} to {DescribeRouteMetres(actor, position).ColourValue()}.");
				return;
			}
			case "description":
			case "desc":
				actor.OutputHandler.Send(
					$"Replacing the description for {landmark.Name.ColourName()}:\n\n{landmark.Description.Wrap(actor.InnerLineFormatLength, "\t")}\n\nEnter the new description below.");
				actor.EditorMode(
					RouteLandmarkDescriptionPost,
					RouteLandmarkDescriptionCancel,
					1.0,
					null,
					EditorOptions.None,
					[cell.Id, landmark.Id]);
				return;
			case "delete":
			case "remove":
				using (new FMDB())
				{
					var dbroute = FMDB.Context.RouteCells.Single(x => x.CellId == cell.Id);
					var dblandmark = FMDB.Context.RouteCellLandmarks.Single(x => x.Id == landmark.Id);
					FMDB.Context.RouteCellLandmarks.Remove(dblandmark);
					dbroute.TopologyVersion++;
					FMDB.Context.SaveChanges();
				}

				ReloadRouteDefinition(cell);
				actor.OutputHandler.Send($"You delete the RouteCell landmark {landmark.Name.ColourName()}.");
				return;
			default:
				actor.OutputHandler.Send(RouteCellSetHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void RouteLandmarkDescriptionPost(string description, IOutputHandler handler, object[] arguments)
	{
		var cellId = (long)arguments[0];
		var landmarkId = (long)arguments[1];
		using (new FMDB())
		{
			var landmark = FMDB.Context.RouteCellLandmarks.SingleOrDefault(x => x.Id == landmarkId);
			var route = FMDB.Context.RouteCells.SingleOrDefault(x => x.CellId == cellId);
			if (landmark is null || route is null)
			{
				handler.Send("That RouteCell landmark no longer exists.");
				return;
			}

			landmark.Description = description;
			route.TopologyVersion++;
			FMDB.Context.SaveChanges();
		}

		var cell = Futuremud.Games.FirstOrDefault()?.Cells.Get(cellId) as MudSharp.Construction.Cell;
		if (cell is not null)
		{
			ReloadRouteDefinition(cell);
		}

		handler.Send("You replace the RouteCell landmark description.");
	}

	private static void RouteLandmarkDescriptionCancel(IOutputHandler handler, object[] arguments)
	{
		handler.Send("You decide not to replace the RouteCell landmark description.");
	}

	private static void CellSetRouteExit(ICharacter actor, MudSharp.Construction.Cell cell, StringStack command)
	{
		if (!RequireRoute(actor, cell, out var route))
		{
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which exit do you want to anchor?");
			return;
		}

		var exit = GetCellExitForBuilderInput(actor.Gameworld.ExitManager.GetAllExits(cell), command, actor);
		if (exit is null)
		{
			actor.OutputHandler.Send("There is no such exit in this cell's topology.");
			return;
		}

		if (command.IsFinished || !command.PopSpeech().EqualTo("band"))
		{
			actor.OutputHandler.Send("Use: cell set route exit <exit> band <minimum> <maximum> arrival <distance>.");
			return;
		}

		var minimumText = command.PopSpeech();
		var maximumText = command.PopSpeech();
		if (!TryParseRouteMetres(actor, minimumText, out var minimum) ||
			!TryParseRouteMetres(actor, maximumText, out var maximum) ||
			minimum < 0.0 ||
			maximum < minimum ||
			maximum > route.LengthMetres)
		{
			actor.OutputHandler.Send("Specify an inclusive minimum/maximum band inside this RouteCell.");
			return;
		}

		if (command.IsFinished || !command.PopSpeech().EqualTo("arrival") ||
			!TryParseRouteMetres(actor, command.SafeRemainingArgument, out var arrival) ||
			arrival < minimum ||
			arrival > maximum)
		{
			actor.OutputHandler.Send("The deterministic arrival coordinate must lie inside the exit band.");
			return;
		}

		using (new FMDB())
		{
			var dbroute = FMDB.Context.RouteCells.Single(x => x.CellId == cell.Id);
			var anchor = FMDB.Context.RouteExitAnchors.Find(exit.Exit.Id, cell.Id);
			if (anchor is null)
			{
				anchor = new DB.RouteExitAnchor
				{
					ExitId = exit.Exit.Id,
					RouteCellId = cell.Id
				};
				FMDB.Context.RouteExitAnchors.Add(anchor);
			}

			anchor.MinimumPositionMetres = ToPersistedMetres(minimum);
			anchor.MaximumPositionMetres = ToPersistedMetres(maximum);
			anchor.ArrivalPositionMetres = ToPersistedMetres(arrival);
			dbroute.TopologyVersion++;
			FMDB.Context.SaveChanges();
		}

		ReloadRouteDefinition(cell);
		actor.OutputHandler.Send(
			$"You anchor exit #{exit.Exit.Id.ToString("N0", actor).ColourValue()} from {DescribeRouteMetres(actor, minimum).ColourValue()} through {DescribeRouteMetres(actor, maximum).ColourValue()}, arriving at {DescribeRouteMetres(actor, arrival).ColourValue()}.");
	}

	private static void CellSetRouteShow(ICharacter actor, MudSharp.Construction.Cell cell)
	{
		if (!RequireRoute(actor, cell, out var route))
		{
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"RouteCell Geometry for {cell.GetFriendlyReference(actor).ColourName()}");
		sb.AppendLine($"Length: {DescribeRouteMetres(actor, route.LengthMetres).ColourValue()}");
		sb.AppendLine($"Default: {DescribeRouteMetres(actor, route.DefaultPositionMetres).ColourValue()}");
		sb.AppendLine($"Negative: {route.NegativeDirectionName.ColourName()}");
		sb.AppendLine($"Positive: {route.PositiveDirectionName.ColourName()}");
		sb.AppendLine($"Room Equivalent: {DescribeRouteMetres(actor, route.MetresPerRoomEquivalent).ColourValue()}");
		sb.AppendLine($"Topology Version: {route.TopologyVersion.ToString("N0", actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Landmarks:");
		sb.AppendLine(StringUtilities.GetTextTable(
			route.Landmarks.Select(x => new[]
			{
				x.Id.ToString("N0", actor),
				x.Name,
				DescribeRouteMetres(actor, x.PositionMetres),
				x.Keywords.ListToString()
			}),
			["Id", "Name", "Coordinate", "Keywords"],
			actor.Account.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
		sb.AppendLine("Exit Anchors:");
		sb.AppendLine(StringUtilities.GetTextTable(
			route.ExitAnchors.Select(x => new[]
			{
				x.Exit.Exit.Id.ToString("N0", actor),
				x.Exit.OutboundDirectionDescription,
				$"{DescribeRouteMetres(actor, x.MinimumPositionMetres)} - {DescribeRouteMetres(actor, x.MaximumPositionMetres)}",
				DescribeRouteMetres(actor, x.ArrivalPositionMetres)
			}),
			["Exit", "Direction", "Band", "Arrival"],
			actor.Account.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode));
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void CellSetRouteMap(ICharacter actor, MudSharp.Construction.Cell cell)
	{
		if (!RequireRoute(actor, cell, out var route))
		{
			return;
		}

		var points = route.Landmarks
			.Select(x => (x.PositionMetres, Label: $"landmark #{x.Id:N0} {x.Name}"))
			.Concat(route.ExitAnchors.Select(x =>
				(PositionMetres: (x.MinimumPositionMetres + x.MaximumPositionMetres) / 2.0,
					Label: $"exit #{x.Exit.Exit.Id:N0} [{DescribeRouteMetres(actor, x.MinimumPositionMetres)}-{DescribeRouteMetres(actor, x.MaximumPositionMetres)}]")))
			.OrderBy(x => x.PositionMetres)
			.ToList();
		var sb = new StringBuilder();
		sb.AppendLine($"{route.NegativeDirectionName.ColourName()} 0m");
		foreach (var point in points)
		{
			sb.AppendLine($"  | {DescribeRouteMetres(actor, point.PositionMetres).ColourValue()} - {point.Label}");
		}
		sb.AppendLine($"  | {DescribeRouteMetres(actor, route.LengthMetres).ColourValue()} {route.PositiveDirectionName.ColourName()}");
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void CellSetRouteValidate(ICharacter actor, MudSharp.Construction.Cell cell)
	{
		if (!RequireRoute(actor, cell, out var route))
		{
			return;
		}

		var errors = new List<string>();
		var warnings = new List<string>();
		if (route.LengthMetres <= 0.0 || route.DefaultPositionMetres < 0.0 ||
			route.DefaultPositionMetres > route.LengthMetres || route.MetresPerRoomEquivalent <= 0.0)
		{
			errors.Add("The core geometry contains invalid lengths or coordinates.");
		}

		if (route.Landmarks.Any(x => x.PositionMetres < 0.0 || x.PositionMetres > route.LengthMetres))
		{
			errors.Add("One or more landmarks are outside the RouteCell bounds.");
		}

		var topologyExits = actor.Gameworld.ExitManager.GetAllExits(cell).ToList();
		var anchoredExitIds = route.ExitAnchors.Select(x => x.Exit.Exit.Id).ToHashSet();
		foreach (var exit in topologyExits.Where(x => !anchoredExitIds.Contains(x.Exit.Id)))
		{
			errors.Add($"Exit #{exit.Exit.Id:N0} ({exit.OutboundDirectionDescription}) has no RouteCell anchor.");
		}

		foreach (var anchor in route.ExitAnchors)
		{
			if (anchor.MinimumPositionMetres < 0.0 ||
				anchor.MaximumPositionMetres < anchor.MinimumPositionMetres ||
				anchor.MaximumPositionMetres > route.LengthMetres ||
				anchor.ArrivalPositionMetres < anchor.MinimumPositionMetres ||
				anchor.ArrivalPositionMetres > anchor.MaximumPositionMetres)
			{
				errors.Add($"Exit #{anchor.Exit.Exit.Id:N0} has an invalid band or arrival coordinate.");
			}

			if (anchor.Exit.Destination.RouteDefinition is not null &&
				anchor.Exit.Destination.RouteDefinition.ExitAnchors.All(x => x.Exit.Exit.Id != anchor.Exit.Exit.Id))
			{
				errors.Add($"Route-to-route exit #{anchor.Exit.Exit.Id:N0} has no anchor on its destination side.");
			}
		}

		foreach (var entity in cell.Perceivables.OfType<ILocateable>())
		{
			if (entity.RoutePositionMetres is null)
			{
				warnings.Add($"{entity.FrameworkItemType} #{entity.Id:N0} has no materialised RouteCell coordinate.");
			}
			else if (entity.RoutePositionMetres < 0.0 || entity.RoutePositionMetres > route.LengthMetres)
			{
				errors.Add($"{entity.FrameworkItemType} #{entity.Id:N0} is outside the RouteCell bounds.");
			}
		}

		var sb = new StringBuilder();
		sb.AppendLine($"RouteCell validation for Cell #{cell.Id.ToString("N0", actor).ColourValue()}, topology {route.TopologyVersion.ToString("N0", actor).ColourValue()}:");
		if (!errors.Any() && !warnings.Any())
		{
			sb.AppendLine("No errors or warnings were found.".Colour(Telnet.Green));
		}
		else
		{
			foreach (var error in errors)
			{
				sb.AppendLine($"Error: {error}".ColourError());
			}

			foreach (var warning in warnings)
			{
				sb.AppendLine($"Warning: {warning}".Colour(Telnet.Yellow));
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static bool RequireRoute(
		ICharacter actor,
		MudSharp.Construction.Cell cell,
		out IRouteCellDefinition route)
	{
		route = cell.RouteDefinition!;
		if (route is not null)
		{
			return true;
		}

		actor.OutputHandler.Send("This is an ordinary cell. Use CELL SET ROUTE CREATE <LENGTH> first.");
		return false;
	}

	private static IRouteCellLandmark? ResolveRouteLandmark(IRouteCellDefinition route, string selector)
	{
		if (long.TryParse(selector, out var id))
		{
			return route.Landmarks.FirstOrDefault(x => x.Id == id);
		}

		return route.Landmarks.FirstOrDefault(x =>
			x.Name.EqualTo(selector) || x.HasKeyword(selector, null, abbreviated: true));
	}

	private static bool TryResolveRoutePosition(
		ICharacter actor,
		IRouteCellDefinition route,
		string text,
		out double position)
	{
		var landmark = ResolveRouteLandmark(route, text);
		if (landmark is not null)
		{
			position = landmark.PositionMetres;
			return true;
		}

		return TryParseRouteMetres(actor, text, out position) && position >= 0.0 && position <= route.LengthMetres;
	}

	private static bool TryParseRouteMetres(ICharacter actor, string text, out double metres)
	{
		metres = 0.0;
		if (string.IsNullOrWhiteSpace(text) ||
			!actor.Gameworld.UnitManager.TryGetBaseUnits(text, UnitType.Length, actor, out var baseUnits))
		{
			return false;
		}

		metres = baseUnits * actor.Gameworld.UnitManager.BaseHeightToMetres;
		return double.IsFinite(metres);
	}

	private static string DescribeRouteMetres(ICharacter actor, double metres)
	{
		return actor.Gameworld.UnitManager.DescribeMostSignificantExact(
			metres / actor.Gameworld.UnitManager.BaseHeightToMetres,
			UnitType.Length,
			actor);
	}

	private static RouteCellMutationActor RouteMutationActor(ICharacter actor)
	{
		return new RouteCellMutationActor(
			CharacterInstanceIdentityComparer.IdentityId(actor),
			actor.InstanceId,
			actor.IsPrimaryInstance);
	}

	private static void AddRouteMutationBlocker(ICollection<string> blockers, string blocker)
	{
		if (!blockers.Contains(blocker, StringComparer.OrdinalIgnoreCase))
		{
			blockers.Add(blocker);
		}
	}

	private static void SetCommittedActorRoutePosition(ICharacter actor, double? routePositionMetres)
	{
		if (actor is not PerceivedItem perceived)
		{
			throw new InvalidOperationException(
				$"Builder #{actor.Id:N0} does not expose the spatial persistence implementation required for RouteCell mutation.");
		}

		if (!perceived.TrySetRoutePosition(routePositionMetres, out var error, noSave: true))
		{
			throw new InvalidOperationException(
				$"The committed RouteCell geometry could not be applied to builder #{actor.Id:N0}: {error}");
		}
	}

	private static decimal ToPersistedMetres(double metres)
	{
		return Math.Round((decimal)metres, 3, MidpointRounding.AwayFromZero);
	}

	private static void MutateRouteDefinition(
		MudSharp.Construction.Cell cell,
		Action<DB.RouteCell> mutation)
	{
		using (new FMDB())
		{
			var route = FMDB.Context.RouteCells.Single(x => x.CellId == cell.Id);
			mutation(route);
			route.TopologyVersion++;
			FMDB.Context.SaveChanges();
		}

		ReloadRouteDefinition(cell);
	}

	private static void MutateLandmark(
		MudSharp.Construction.Cell cell,
		long landmarkId,
		Action<DB.RouteCellLandmark> mutation)
	{
		using (new FMDB())
		{
			var route = FMDB.Context.RouteCells.Single(x => x.CellId == cell.Id);
			var landmark = FMDB.Context.RouteCellLandmarks.Single(x => x.Id == landmarkId);
			mutation(landmark);
			route.TopologyVersion++;
			FMDB.Context.SaveChanges();
		}

		ReloadRouteDefinition(cell);
	}

	private static void ReloadRouteDefinition(MudSharp.Construction.Cell cell)
	{
		using (new FMDB())
		{
			var route = FMDB.Context.RouteCells
				.AsNoTracking()
				.Include(x => x.Landmarks)
				.Include(x => x.ExitAnchors)
				.SingleOrDefault(x => x.CellId == cell.Id);
			cell.ReloadRouteDefinition(route);
		}
	}
}
