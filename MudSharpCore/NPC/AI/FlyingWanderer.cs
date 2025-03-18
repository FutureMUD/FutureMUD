using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character.Name;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.Body.Position;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Construction;

namespace MudSharp.NPC.AI;
internal class FlyingWanderer : PathingAIBase
{
	protected FlyingWanderer(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	private FlyingWanderer()
	{

	}

	private FlyingWanderer(IFuturemud gameworld, string name) : base(gameworld, name, "FlyingWanderer")
	{
		TargetFlyingLayer = RoomLayer.InAir;
		TargetRestingLayer = RoomLayer.HighInTrees;
		WanderTimeDiceExpression = "1d200+300";
		IsWanderingProg = Gameworld.AlwaysTrueProg;
		WillWanderIntoCellProg = Gameworld.AlwaysTrueProg;
		EmoteText = string.Empty;
		OpenDoors = false;
		UseKeys = false;
		UseDoorguards = false;
		CloseDoorsBehind = false;
		MoveEvenIfObstructionInWay = false;
		SmashLockedDoors = false;
		DatabaseInitialise();
	}

	protected override void LoadFromXML(XElement root)
	{
		base.LoadFromXML(root);
		WillWanderIntoCellProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("FutureProg")?.Value ?? "0"));
		IsWanderingProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("IsWanderingProg")?.Value ?? "0")) ?? Gameworld.AlwaysTrueProg;
		WanderTimeDiceExpression = root.Element("WanderTimeDiceExpression")?.Value ?? "1d200+300";
		EmoteText = root.Element("EmoteText")?.Value;
		TargetRestingLayer = (RoomLayer)int.Parse(root.Element("TargetRestingLayer").Value);
		TargetFlyingLayer = (RoomLayer)int.Parse(root.Element("TargetFlyingLayer").Value);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("FutureProg", WillWanderIntoCellProg?.Id ?? 0),
			new XElement("WanderTimeDiceExpression", new XCData(WanderTimeDiceExpression)),
			new XElement("EmoteText", new XCData(EmoteText)),
			new XElement("TargetRestingLayer", (int)TargetRestingLayer),
			new XElement("TargetFlyingLayer", (int)TargetFlyingLayer),
			new XElement("OpenDoors", OpenDoors),
			new XElement("UseKeys", UseKeys),
			new XElement("SmashLockedDoors", SmashLockedDoors),
			new XElement("CloseDoorsBehind", CloseDoorsBehind),
			new XElement("UseDoorguards", UseDoorguards),
			new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay)
		).ToString();
	}

	public static void RegisterLoader()
	{
		RegisterAIType("FlyingWanderer", (ai, gameworld) => new FlyingWanderer(ai, gameworld));
		RegisterAIBuilderInformation("flyingwanderer", (game, name) => new FlyingWanderer(game, name), new FlyingWanderer().HelpText);
	}

	protected string WanderTimeDiceExpression;
	protected IFutureProg WillWanderIntoCellProg;
	protected IFutureProg IsWanderingProg;
	protected string EmoteText;
	protected RoomLayer TargetFlyingLayer;
	protected RoomLayer TargetRestingLayer;

	protected override (ICell Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		var vicinity = ch.CellsAndDistancesInVicinity(10, 
			GetSuitabilityFunction(ch, true), 
			cell => WillWanderIntoCellProg?.ExecuteBool(false, ch, cell, ch.Location) == true);
		var target = vicinity.GetWeightedRandom(x => Math.Sqrt(x.Distance)).Cell;
		if (target is null)
		{
			return (null, []);
		}
		var path = ch.PathBetween(target, 10, GetSuitabilityFunction(ch, true)).ToList();
		if (path.Count == 0)
		{
			return (null, []);
		}

		return (path.Last().Destination, path);
	}

	private void CreateEvaluateAffect(ICharacter character)
	{
		if (character.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		if (character.Effects.Any(x => x.IsEffectType<WandererWaiting>() || x.IsEffectType<FollowingPath>()))
		{
			return;
		}

		if (character.Movement != null || character.Combat != null)
		{
			return;
		}

		character.AddEffect(new WandererWaiting(character, actor => EvaluateWander(character)),
			TimeSpan.FromSeconds(Dice.Roll(WanderTimeDiceExpression)));
	}

	private void CancelEvaluateAffect(ICharacter character)
	{
		character.RemoveAllEffects(
			x => x.IsEffectType<WandererWaiting>());
	}

	private void EvaluateWander(ICharacter character)
	{
		if (character.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		if (!IsWanderingProg.ExecuteBool(false, character))
		{
			return;
		}

		if (character.Movement != null)
		{
			CreateEvaluateAffect(character);
			return;
		}

		if (character.EffectsOfType<FollowingPath>().Any())
		{
			// Don't wander while following a path from other AI
			CreateEvaluateAffect(character);
			return;
		}

		if (!CharacterState.Able.HasFlag(character.State) || character.Combat != null ||
			character.Effects.Any(x => x.IsBlockingEffect("movement")))
		{
			CreateEvaluateAffect(character);
			return;
		}

		if (character.Effects.Any(x => x.IsEffectType<WandererWaiting>()))
		{
			return;
		}

		if ((character.CurrentStamina / character.MaximumStamina) < 0.5)
		{
			CreateEvaluateAffect(character);
			return;
		}

		if (!CreatePathingEffectIfPathExists(character))
		{
			CreateEvaluateAffect(character);
		}
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		var ch = (type == EventType.EngagedInCombat ?
			arguments[1] :
			arguments[0]) as ICharacter;
		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		switch (type)
		{
			case EventType.CharacterEnterCellFinish:
			case EventType.CharacterStopMovement:
			case EventType.CharacterStopMovementClosedDoor:
			case EventType.CharacterCannotMove:
				CreateEvaluateAffect(arguments[0]);
				return false;

			case EventType.EngagedInCombat:
				CancelEvaluateAffect(arguments[1]);
				return false;
			case EventType.EngageInCombat:
				CancelEvaluateAffect(arguments[0]);
				return false;

			case EventType.CharacterEntersGame:
			case EventType.LeaveCombat:
				CreateEvaluateAffect(arguments[0]);
				return false;

			case EventType.FiveSecondTick:
				CreateEvaluateAffect(arguments[0]);
				return false;
			case EventType.TenSecondTick:
				CheckPathingEffect(arguments[0], true);
				return false;
			case EventType.LayerChangeBlockExpired:
				HandleLayerChangeBlockExpired(arguments[0]);
				return false;
		}

		return base.HandleEvent(type, arguments);
	}

	private void HandleLayerChangeBlockExpired(ICharacter character)
	{
		CheckPathingEffect(character, false);
	}

	#region Overrides of PathingAIBase

	/// <inheritdoc />
	protected override FollowingPath CreatePathingEffect(ICharacter ch, IEnumerable<ICellExit> path)
	{
		return new FollowingMultiLayerPath(ch, path, TargetFlyingLayer, TargetRestingLayer);
	}

	#endregion

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.CharacterEnterCellFinish:
				case EventType.CharacterStopMovement:
				case EventType.CharacterStopMovementClosedDoor:
				case EventType.CharacterCannotMove:
				case EventType.EngagedInCombat:
				case EventType.EngageInCombat:
				case EventType.CharacterEntersGame:
				case EventType.LeaveCombat:
				case EventType.FiveSecondTick:
				case EventType.TenSecondTick:
				case EventType.LayerChangeBlockExpired:
					return true;
			}
		}

		return base.HandlesEvent(types);
	}

	#region Overrides of ArtificialIntelligenceBase
	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine($"Is Enabled: {IsWanderingProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Will Wander Room: {WillWanderIntoCellProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Target Resting Layer: {TargetRestingLayer.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Target Flying Layer: {TargetFlyingLayer.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Wander Time Dice: {WanderTimeDiceExpression.ColourValue()} seconds");
		sb.AppendLine($"Travel String: {EmoteText?.ColourCommand() ?? ""}");
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string TypeHelpText => @"	#3doors#0 - toggles this AI opening doors
	#3enabled <prog>#0 - sets the prog that controls whether wandering is enabled
	#3room <prog>#0 - sets a prog that evalutes rooms the AI will wander into
	#3delay <dice expression>#0 - sets the delay in seconds between wandering
	#3emote clear#0 - clears any emote when the NPC wanders
	#3emote <text>#0 - sets an emote the NPC will do when it wanders";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "emote":
				return BuildingCommandEmote(actor, command);
			case "delay":
				return BuildingCommandDelay(actor, command);
			case "room":
			case "roomprog":
				return BuildingCommandRoomProg(actor, command);
			case "enabled":
			case "enabledprog":
				return BuildingCommandEnabledProg(actor, command);
			case "doors":
				return BuildingCommandDoors(actor);
			case "flyinglayer":
				return BuildingCommandFlyingLayer(actor, command);
			case "restinglayer":
				return BuildingCommandRestingLayer(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandRestingLayer(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must supply a room layer.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<RoomLayer>(out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid room layer.\nValid values are {Enum.GetValues<RoomLayer>().ListToColouredString()}.");
			return false;
		}

		TargetRestingLayer = value;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now try to rest in the {value.DescribeEnum().ColourValue()} layer.");
		return true;
	}

	private bool BuildingCommandFlyingLayer(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must supply a room layer.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<RoomLayer>(out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid room layer.\nValid values are {Enum.GetValues<RoomLayer>().ListToColouredString()}.");
			return false;
		}

		TargetFlyingLayer = value;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now try to rest in the {value.DescribeEnum().ColourValue()} layer.");
		return true;
	}

	private bool BuildingCommandDoors(ICharacter actor)
	{
		OpenDoors = !OpenDoors;
		actor.OutputHandler.Send($"This AI will {OpenDoors.NowNoLonger()} open doors during its wandering.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandEnabledProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to control whether wandering is enabled?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new List<ProgVariableTypes>
			{
				ProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		IsWanderingProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the prog {prog.MXPClickableFunctionName()} to determine whether wandering is enabled.");
		return true;
	}

	private bool BuildingCommandRoomProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should be used to control whether an NPC will wander into a room?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new[]{
				new List<ProgVariableTypes>
				{
					ProgVariableTypes.Character,
					ProgVariableTypes.Location,
				},
				new List<ProgVariableTypes>
				{
					ProgVariableTypes.Character,
					ProgVariableTypes.Location,
					ProgVariableTypes.Location,
				}
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WillWanderIntoCellProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the prog {prog.MXPClickableFunctionName()} to determine whether rooms are suitable for wandering.");
		return true;
	}

	private bool BuildingCommandDelay(ICharacter actor, StringStack command)
	{
		var expr = command.SafeRemainingArgument;
		if (!Dice.IsDiceExpression(expr))
		{
			actor.OutputHandler.Send($"The text {expr.ColourCommand()} is not a valid dice expression.");
			return false;
		}

		WanderTimeDiceExpression = expr;

		Changed = true;
		actor.OutputHandler.Send($"This AI will now check for wandering every {expr.ColourCommand()} seconds.");
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote should this AI append to their movement?");
			return false;
		}

		EmoteText = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This NPC will now add the following emote to the end of its movement commands: {EmoteText.ColourCommand()}");
		return true;
	}

	#endregion
}
