using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.EntityFrameworkCore.Internal;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.RPG.Law.PatrolStrategies;
using MudSharp.ThirdPartyCode;
using MudSharp.TimeAndDate;

namespace MudSharp.RPG.Law;

public class PatrolRoute : SaveableItem, IPatrolRoute, IEditableItem
{
	public PatrolRoute(ILegalAuthority authority, string name)
	{
		Gameworld = authority.Gameworld;
		LegalAuthority = authority;
		_name = name;
		LingerTimeMajorNode =
			TimeSpan.FromSeconds(Gameworld.GetStaticDouble("DefaultPatrolLingerTimeMajorNodeSeconds"));
		LingerTimeMinorNode =
			TimeSpan.FromSeconds(Gameworld.GetStaticDouble("DefaultPatrolLingerTimeMinorNodeSeconds"));
		_timeOfDays.AddRange(Enum.GetValues(typeof(TimeOfDay)).OfType<TimeOfDay>());
		PatrolStrategy = new ArmedPatrolStrategy(Gameworld);
		using (new FMDB())
		{
			var dbitem = new Models.PatrolRoute
			{
				Name = name,
				LingerTimeMajorNode = LingerTimeMajorNode.TotalSeconds,
				LingerTimeMinorNode = LingerTimeMinorNode.TotalSeconds,
				LegalAuthorityId = authority.Id,
				PatrolStrategy = PatrolStrategy.Name,
				Priority = Priority
			};
			FMDB.Context.PatrolRoutes.Add(dbitem);
			foreach (var tod in _timeOfDays)
			{
				dbitem.TimesOfDay.Add(new Models.PatrolRouteTimeOfDay { PatrolRoute = dbitem, TimeOfDay = (int)tod });
			}

			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public PatrolRoute(Models.PatrolRoute route, ILegalAuthority authority)
	{
		Gameworld = authority.Gameworld;
		LegalAuthority = authority;
		_id = route.Id;
		_name = route.Name;
		Priority = route.Priority;
		LingerTimeMajorNode = TimeSpan.FromSeconds(route.LingerTimeMajorNode);
		LingerTimeMinorNode = TimeSpan.FromSeconds(route.LingerTimeMinorNode);
		_timeOfDays.AddRange(route.TimesOfDay.Select(x => (TimeOfDay)x.TimeOfDay));
		PatrolStrategy = PatrolStrategyFactory.GetStrategy(route.PatrolStrategy, Gameworld);
		StartPatrolProg = Gameworld.FutureProgs.Get(route.StartPatrolProgId ?? 0);
		_patrolNodes.AddRange(route.PatrolRouteNodes.OrderBy(x => x.Order)
								   .SelectNotNull(x => Gameworld.Cells.Get(x.CellId)));
		foreach (var node in _patrolNodes)
		{
			node.CellProposedForDeletion -= Node_CellProposedForDeletion;
			node.CellProposedForDeletion += Node_CellProposedForDeletion;
		}
		IsReady = route.IsReady;
		foreach (var number in route.PatrolRouteNumbers)
		{
			PatrollerNumbers[
					LegalAuthority.EnforcementAuthorities.FirstOrDefault(x => x.Id == number.EnforcementAuthorityId)] =
				number.NumberRequired;
		}
	}

	private void Node_CellProposedForDeletion(ICell cell, ProposalRejectionResponse response)
	{
		response.RejectWithReason($"That room is a patrol node for patrol route #{Id:N0} ({Name.ColourName()})");
	}

	public override string FrameworkItemType => "PatrolRoute";

	public override void Save()
	{
		var dbitem = FMDB.Context.PatrolRoutes.Find(Id);
		dbitem.Name = Name;
		dbitem.PatrolStrategy = PatrolStrategy.Name;
		dbitem.LingerTimeMajorNode = LingerTimeMajorNode.TotalSeconds;
		dbitem.LingerTimeMinorNode = LingerTimeMinorNode.TotalSeconds;
		dbitem.Priority = Priority;
		dbitem.StartPatrolProgId = StartPatrolProg?.Id;
		dbitem.IsReady = IsReady;
		FMDB.Context.PatrolRoutesNodes.RemoveRange(dbitem.PatrolRouteNodes);
		var order = 0;
		foreach (var node in _patrolNodes)
		{
			dbitem.PatrolRouteNodes.Add(new Models.PatrolRouteNode
				{ PatrolRoute = dbitem, CellId = node.Id, Order = order++ });
		}

		FMDB.Context.PatrolRoutesTimesOfDay.RemoveRange(dbitem.TimesOfDay);
		foreach (var tod in _timeOfDays)
		{
			dbitem.TimesOfDay.Add(new Models.PatrolRouteTimeOfDay { PatrolRoute = dbitem, TimeOfDay = (int)tod });
		}

		FMDB.Context.PatrolRoutesNumbers.RemoveRange(dbitem.PatrolRouteNumbers);
		foreach (var number in PatrollerNumbers)
		{
			dbitem.PatrolRouteNumbers.Add(new Models.PatrolRouteNumbers
				{ PatrolRoute = dbitem, EnforcementAuthorityId = number.Key.Id, NumberRequired = number.Value });
		}

		Changed = false;
	}

	public void Delete()
	{
		foreach (var node in _patrolNodes)
		{
			node.CellProposedForDeletion -= Node_CellProposedForDeletion;
		}
		Gameworld.SaveManager.Abort(this);
		using (new FMDB())
		{
			Gameworld.SaveManager.Flush();
			var dbitem = FMDB.Context.PatrolRoutes.Find(Id);
			if (dbitem != null)
			{
				FMDB.Context.PatrolRoutes.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}
	}

	public ILegalAuthority LegalAuthority { get; protected set; }

	private readonly List<ICell> _patrolNodes = new();
	public IEnumerable<ICell> PatrolNodes => _patrolNodes;

	public Counter<IEnforcementAuthority> PatrollerNumbers { get; } = new();

	private readonly List<TimeOfDay> _timeOfDays = new();
	public IEnumerable<TimeOfDay> TimeOfDays => _timeOfDays;
	public TimeSpan LingerTimeMajorNode { get; protected set; }
	public TimeSpan LingerTimeMinorNode { get; protected set; }
	public IPatrolStrategy PatrolStrategy { get; protected set; }
	public IFutureProg StartPatrolProg { get; protected set; }
	public bool IsReady { get; protected set; }

	public bool ShouldBeginPatrol()
	{
		return
			IsReady &&
			PatrolNodes.Any() &&
			StartPatrolProg?.Execute<bool?>() != false &&
			PatrollerNumbers.Any() &&
			TimeOfDays.Contains(LegalAuthority.EnforcementZones.First().CurrentTimeOfDay);
	}

	public int Priority { get; protected set; }

	public string HelpInfo => @"You can use the following options with this command:

	#3name <name>#0 - renames the patrol route
	#3time <list of time of days>#0 - sets the valid times of day
	#3linger <major> <minor>#0 - sets the linger time on major and minor patrol nodes
	#3strategy <which>#0 - sets the strategy for the patrol
	#3priority <number>#0 - sets the priority of resourcing this patrol
	#3numbers <enforcement> <number>#0 - sets the numbers of a particular enforcer type required
	#3prog <prog>#0 - sets the prog that controls whether the patrol can start
	#3prog clear#0 - clears the start prog
	#3node#0 - adds the current location as a node to the end of the list
	#3node delete <##>#0 - deletes a node
	#3node swap <##1> <##2>#0 - swaps the position of two nodes
	#3node insert <##>#0 - inserts the current location as a node at position #
	#3ready#0 - toggles whether this patrol is ready to be used";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "time":
			case "times":
				return BuildingCommandTime(actor, command);
			case "linger":
				return BuildingCommandLinger(actor, command);
			case "strategy":
				return BuildingCommandStrategy(actor, command);
			case "priority":
				return BuildingCommandPriority(actor, command);
			case "node":
				return BuildingCommandNode(actor, command);
			case "numbers":
				return BuildingCommandNumbers(actor, command);
			case "prog":
				return BuildingCommandProg(actor, command);
			case "ready":
				return BuildingCommandReady(actor, command);
			case "":
				actor.OutputHandler.Send(Show(actor));
				return true;
			default:
				actor.OutputHandler.Send(HelpInfo.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandReady(ICharacter actor, StringStack command)
	{
		IsReady = !IsReady;
		Changed = true;
		actor.OutputHandler.Send($"This patrol route is {(IsReady ? "now" : "no longer")} ready for use.");
		return true;
	}

	private bool BuildingCommandNumbers(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which enforcement authority do you want to toggle the numbers for?");
			return false;
		}

		var enforcement = long.TryParse(command.PopSpeech(), out var value)
			? LegalAuthority.EnforcementAuthorities.FirstOrDefault(x => x.Id == value)
			: LegalAuthority.EnforcementAuthorities.FirstOrDefault(x => x.Name.EqualTo(command.Last)) ??
			  LegalAuthority.EnforcementAuthorities.FirstOrDefault(x =>
				  x.Name.StartsWith(command.Last, StringComparison.InvariantCultureIgnoreCase));
		if (enforcement == null)
		{
			actor.OutputHandler.Send(
				$"The {LegalAuthority.Name.ColourName()} legal authority has no such enforcement authority.");
			return false;
		}

		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var ivalue))
		{
			actor.OutputHandler.Send(
				$"How many patrol members of authority {enforcement.Name.ColourName()} should this patrol require before beginning?");
			return false;
		}

		if (ivalue <= 0)
		{
			PatrollerNumbers.Remove(enforcement);
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} patrol will no longer require any members of authority {enforcement.Name.ColourName()} to begin.");
			Changed = true;
			return true;
		}

		PatrollerNumbers[enforcement] = ivalue;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} patrol will now require {ivalue.ToString("N0", actor).ColourValue()} members of authority {enforcement.Name.ColourName()} to begin.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandNode(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			if (PatrolNodes.Contains(actor.Location))
			{
				actor.OutputHandler.Send(
					"Your current location is already a patrol nodes. Patrol nodes can only appear once.");
				return false;
			}

			_patrolNodes.Add(actor.Location);
			actor.Location.CellProposedForDeletion -= Node_CellProposedForDeletion;
			actor.Location.CellProposedForDeletion += Node_CellProposedForDeletion;
			actor.OutputHandler.Send(
				$"You add your current location ({actor.Location.HowSeen(actor)}) as a major node for the {Name.ColourName()} patrol.");
			if (_patrolNodes.Count >= 2)
			{
				var last = _patrolNodes[_patrolNodes.Count - 2];
				var path = last.PathBetween(actor.Location, 50, PathSearch.PathIncludeUnlockableDoors(actor));
				if (!path.Any())
				{
					actor.OutputHandler.Send(
						$"Warning: Cannot find a path between the new node and the former node.".Colour(Telnet.Red));
				}
				else
				{
					actor.OutputHandler.Send(
						$"Path between the last node and this node is: {path.DescribeDirectionKeywords(Telnet.Yellow)}.");
				}
			}

			Changed = true;
			return true;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "remove":
			case "rem":
			case "delete":
			case "del":
				return BuildingCommandNodeDelete(actor, command);
			case "swap":
				return BuildingCommandNodeSwap(actor, command);
			case "insert":
				return BuildingCommandNodeInsert(actor, command);
		}

		actor.OutputHandler.Send(
			$"If you specify an argument to the 'node' subcommand, it must be remove, swap or insert.");
		return false;
	}

	private bool BuildingCommandNodeInsert(ICharacter actor, StringStack command)
	{
		if (PatrolNodes.Contains(actor.Location))
		{
			actor.OutputHandler.Send(
				"Your current location is already a patrol nodes. Patrol nodes can only appear once.");
			return false;
		}

		if (!_patrolNodes.Any())
		{
			actor.OutputHandler.Send("There are no existing patrol nodes, just use the simple addition method.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What position in the list of nodes do you want to insert this location as?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1 || value > _patrolNodes.Count)
		{
			actor.OutputHandler.Send(
				$"You must enter a valid position to insert this location as a node in, between {1.ToString("N0", actor)} and {_patrolNodes.Count.ToString("N0", actor)}.");
			return false;
		}

		_patrolNodes.Insert(value - 1, actor.Location);
		Changed = true;
		actor.OutputHandler.Send(
			$"You insert your current location ({actor.Location.HowSeen(actor)}) as a major patrol node at position #{value.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandNodeSwap(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send("Todo");
		return false;
	}

	private bool BuildingCommandNodeDelete(ICharacter actor, StringStack command)
	{
		if (command.IsFinished && _patrolNodes.Contains(actor.Location))
		{
			_patrolNodes.Remove(actor.Location);
			actor.Location.CellProposedForDeletion -= Node_CellProposedForDeletion;
			actor.Location.CellProposedForDeletion += Node_CellProposedForDeletion;
			Changed = true;
			actor.OutputHandler.Send(
				$"You remove your current location ({actor.Location.HowSeen(actor)}) from the list of patrol nodes.");
			return true;
		}

		if (command.IsFinished || !long.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send("Which location do you want to remove as a patrol node?");
			return false;
		}

		var location = Gameworld.Cells.Get(value);
		if (location == null || !_patrolNodes.Contains(location))
		{
			actor.OutputHandler.Send("There is no such location on this patrol route.");
			return false;
		}

		_patrolNodes.Remove(location);
		actor.Location.CellProposedForDeletion -= Node_CellProposedForDeletion;
		actor.Location.CellProposedForDeletion += Node_CellProposedForDeletion;
		Changed = true;
		actor.OutputHandler.Send(
			$"You remove the location {actor.Location.HowSeen(actor)} from the list of patrol nodes.");
		return true;
	}

	private bool BuildingCommandPriority(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send(
				"What integer priority should this patrol route have in determining the allocation of resources? Higher is more important.");
			return false;
		}

		Priority = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"The {Name.ColourName()} patrol now has a priority of {Priority.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandStrategy(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which strategy do you want to use for this patrol?\nValid options are {PatrolStrategyFactory.Strategies.Select(x => x.ColourValue()).Humanize()}.");
			return false;
		}

		try
		{
			var strategy = PatrolStrategyFactory.GetStrategy(command.SafeRemainingArgument, actor.Gameworld);
			PatrolStrategy = strategy;
			Changed = true;
			actor.OutputHandler.Send($"This patrol will now use the {strategy.Name.ColourName()} patrol strategy.");
			return true;
		}
		catch
		{
			actor.OutputHandler.Send(
				$"There is no such patrol strategy.\nValid options are {PatrolStrategyFactory.Strategies.Select(x => x.ColourValue()).Humanize()}.");
			return false;
		}
	}

	private bool BuildingCommandLinger(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to set the linger time for major or minor nodes?");
			return false;
		}

		var type = command.PopSpeech().ToLowerInvariant();
		if (!type.In("major", "minor"))
		{
			actor.OutputHandler.Send("You must choose either major or minor nodes.");
			return false;
		}

		var major = type.EqualTo("major");
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"How long should this patrol linger on {(major ? "major" : "minor")} nodes?");
			return false;
		}
		
		if (!MudTimeSpan.TryParse(command.SafeRemainingArgument, out var tspan))
		{
			actor.OutputHandler.Send("That is not a valid length of time.");
			return false;
		}

		if (major)
		{
			LingerTimeMajorNode = tspan;
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} patrol will now linger for {tspan.Describe().ColourValue()} on major nodes.");
		}
		else
		{
			LingerTimeMinorNode = tspan;
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} patrol will now linger for {tspan.Describe().ColourValue()} on minor nodes.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandTime(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which times of day did you want to set for this patrol to be active?\nValid options are: {Enum.GetValues(typeof(TimeOfDay)).OfType<TimeOfDay>().Select(x => x.DescribeColour()).ListToString()}.");
			return false;
		}

		var values = new List<TimeOfDay>();
		while (!command.IsFinished)
		{
			var text = command.PopSpeech();
			if (!text.TryParseEnum<TimeOfDay>(out var value))
			{
				actor.OutputHandler.Send(
					$"'{text}' is not a valid Time of Day. Valid options are: {Enum.GetValues(typeof(TimeOfDay)).OfType<TimeOfDay>().Select(x => x.DescribeColour()).ListToString()}.");
				return false;
			}

			if (!values.Contains(value))
			{
				values.Add(value);
			}
		}

		_timeOfDays.Clear();
		_timeOfDays.AddRange(values);
		Changed = true;
		actor.OutputHandler.Send(
			$"The {_name.ColourName()} patrol will now be active in the following times: {_timeOfDays.Select(x => x.DescribeColour()).ListToString()}.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to rename this patrol route?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (LegalAuthority.PatrolRoutes.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"That legal authority already has a patrol route with that name. Names must be unique per legal authority.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the patrol route {_name.ColourName()} to {_name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify a prog to control whether or not to run the patrol or use 'none' to clear the existing prog.");
			return false;
		}

		if (command.PeekSpeech().EqualTo("clear"))
		{
			StartPatrolProg = null;
			Changed = true;
			actor.OutputHandler.Send("This patrol route no longer uses a prog to determine whether it can begin.");
			return true;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.ReturnType.CompatibleWith(FutureProgVariableTypes.Boolean))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that returns a boolean value, whereas {prog.MXPClickableFunctionName()} returns {prog.ReturnType.Describe().ColourValue()}.");
			return false;
		}

		if (!prog.MatchesParameters(new List<FutureProgVariableTypes> { }))
		{
			actor.OutputHandler.Send(
				$"You must specify a prog that does not accept any parameters, whereas {prog.MXPClickableFunctionName()} does not meet that criteria.");
			return false;
		}

		StartPatrolProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This patrol route will now use the {prog.MXPClickableFunctionName()} prog to control whether it can start.");
		return true;
	}

	#region BuildingCommands

	#endregion

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Patrol Route #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Ready: {IsReady.ToColouredString()}");
		sb.AppendLine($"Priority: {Priority.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Strategy: {PatrolStrategy.Name.ColourValue()}");
		sb.AppendLine($"Linger (Major): {LingerTimeMajorNode.Describe(actor).ColourValue()}");
		sb.AppendLine($"Linger (Minor): {LingerTimeMinorNode.Describe(actor).ColourValue()}");
		sb.AppendLine($"Active Times: {TimeOfDays.Select(x => x.DescribeColour()).ListToString()}");
		sb.AppendLine(
			$"Patroller Numbers: {PatrollerNumbers.Select(x => $"{x.Value.ToString("N0", actor)} {(x.Value == 1 ? x.Key.Name : x.Key.Name.Pluralise())}".ColourValue()).ListToString()}");
		sb.AppendLine("Nodes:");
		foreach (var node in PatrolNodes)
		{
			sb.AppendLine($"\t{node.Id.ToString("N0", actor)}) {node.HowSeen(actor)}");
		}

		if (_patrolNodes.Any())
		{
			sb.AppendLine();
			var directions = new List<string>();
			for (var i = 0; i < _patrolNodes.Count; i++)
			{
				var thisNode = _patrolNodes[i];
				ICell nextNode = null;
				if (i + 1 < _patrolNodes.Count)
				{
					nextNode = _patrolNodes[i + 1];
				}
				else
				{
					nextNode = _patrolNodes[0];
				}

				if (thisNode == nextNode)
				{
					continue;
				}

				var path = thisNode.PathBetween(nextNode, 50, PathSearch.PathIncludeUnlockableDoors(actor));
				if (!path.Any())
				{
					directions.Add($"[Broken Path between {thisNode.HowSeen(actor)} and {nextNode.HowSeen(actor)}]"
						.Colour(Telnet.Red));
					continue;
				}

				foreach (var exit in path)
				{
					directions.Add((exit is NonCardinalCellExit ncce
						? $"{ncce.Verb} {ncce.PrimaryKeyword}".ToLowerInvariant()
						: exit.OutboundDirection.DescribeBrief()).Colour(Telnet.Green));
				}
			}

			sb.AppendLine($"Path Summary: {directions.ListToString(conjunction: "", twoItemJoiner: "")}");
		}

		return sb.ToString();
	}
}