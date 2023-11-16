using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MudSharp.NPC.AI;

public abstract class PathingAIWithProgTargetsBase : PathingAIBase
{
	public IFutureProg PathingEnabledProg { get; protected set; }

	public IFutureProg OnStartToPathProg { get; protected set; }

	public IFutureProg TargetLocationProg { get; set; }

	public IFutureProg FallbackLocationProg { get; set; }

	public IFutureProg WayPointsProg { get; set; }

	protected override string TypeHelpText => @$"{base.TypeHelpText}
	#3pathingenabled <prog>#0 - sets a prog to control pathfinding on or off
	#3target <prog>#0 - sets a prog that controls the target of pathfinding
	#3fallback <prog>#0 - sets a prog to determine where to fallback to if pathfinding fails
	#3waypoints <prog>#0 - sets a prog to control major waypoints for pathfinding
	#3onstart <prog>#0 - sets a prog to execute on commencement of pathing
	#3onstart clear#0 - clears the OnStart prog";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "pathingenabled":
			case "pathenabled":
			case "pathingenabledprog":
			case "pathenabledprog":
				return BuildingCommandPathingEnabled(actor, command);
			case "onstart":
			case "onstartprog":
				return BuildingCommandOnStartProg(actor, command);
			case "targetprog":
			case "target":
			case "targetlocationprog":
			case "targetlocation":
				return BuildingCommandTargetLocationProg(actor, command);
			case "fallbacklocationprog":
			case "fallbacklocation":
			case "fallback":
			case "fallbackprog":
				return BuildingCommandFallbackLocationProg(actor, command);
			case "waypoints":
			case "waypoint":
			case "waypointprog":
			case "waypointsprog":
				return BuildingCommandWaypointsProg(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandWaypointsProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, FutureProgVariableTypes.Location | FutureProgVariableTypes.Collection, new[] { FutureProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WayPointsProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the {prog.MXPClickableFunctionName()} prog to control movement waypoints.");
		return true;
	}

	private bool BuildingCommandFallbackLocationProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, FutureProgVariableTypes.Location, new[] { FutureProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		FallbackLocationProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the {prog.MXPClickableFunctionName()} prog to identify a fallback location if pathing fails or is disabled.");
		return true;
	}

	private bool BuildingCommandTargetLocationProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, FutureProgVariableTypes.Location, new[] { FutureProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		TargetLocationProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the {prog.MXPClickableFunctionName()} prog to select a target location for pathfinding.");
		return true;
	}

	private bool BuildingCommandOnStartProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, FutureProgVariableTypes.Void, new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Location, FutureProgVariableTypes.Exit | FutureProgVariableTypes.Collection }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		OnStartToPathProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the {prog.MXPClickableFunctionName()} prog when it starts pathing.");
		return true;
	}

	private bool BuildingCommandPathingEnabled(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, FutureProgVariableTypes.Boolean, new[] { FutureProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		PathingEnabledProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the {prog.MXPClickableFunctionName()} prog to control whether pathing is enabled or disabled.");
		return true;
	}

	#region Overrides of PathingAIBase

	protected override bool IsPathingEnabled(ICharacter ch)
	{
		return (bool?)PathingEnabledProg.Execute(ch) ?? false;
	}

	#endregion

	protected PathingAIWithProgTargetsBase(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	#region Overrides of PathingAIBase

	protected override void LoadFromXML(XElement root)
	{
		PathingEnabledProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("PathingEnabledProg")?.Value ?? "0"));
		OnStartToPathProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnStartToPathProg")?.Value ?? "0"));
		TargetLocationProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("TargetLocationProg")?.Value ?? "0"));
		FallbackLocationProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("FallbackLocationProg")?.Value ?? "0"));
		WayPointsProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WayPointsProg")?.Value ?? "0"));
	}

	protected override void OnBeginPathing(ICharacter ch, ICell target, IEnumerable<ICellExit> exits)
	{
		OnStartToPathProg?.Execute(ch, target, exits);
	}
	#endregion
}