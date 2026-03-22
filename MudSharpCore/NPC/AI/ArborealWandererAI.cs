#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.NPC.AI;

public class ArborealWandererAI : PathingAIBase
{
	protected string WanderTimeDiceExpression = "1d180+180";
	protected IFutureProg WillWanderIntoCellProg = null!;
	protected IFutureProg IsWanderingProg = null!;
	protected IFutureProg AllowDescentProg = null!;
	protected string EmoteText = string.Empty;
	protected RoomLayer PreferredTreeLayer = RoomLayer.HighInTrees;
	protected RoomLayer SecondaryTreeLayer = RoomLayer.InTrees;

	protected ArborealWandererAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	private ArborealWandererAI()
	{
	}

	private ArborealWandererAI(IFuturemud gameworld, string name) : base(gameworld, name, "ArborealWanderer")
	{
		WillWanderIntoCellProg = Gameworld.AlwaysTrueProg;
		IsWanderingProg = Gameworld.AlwaysTrueProg;
		AllowDescentProg = Gameworld.AlwaysFalseProg;
		OpenDoors = false;
		UseKeys = false;
		UseDoorguards = false;
		CloseDoorsBehind = false;
		MoveEvenIfObstructionInWay = false;
		SmashLockedDoors = false;
		DatabaseInitialise();
	}

	public static void RegisterLoader()
	{
		RegisterAIType("ArborealWanderer", (ai, gameworld) => new ArborealWandererAI(ai, gameworld));
		RegisterAIBuilderInformation("arborealwanderer",
			(gameworld, name) => new ArborealWandererAI(gameworld, name),
			new ArborealWandererAI().HelpText);
	}

	protected override void LoadFromXML(XElement root)
	{
		base.LoadFromXML(root);
		WillWanderIntoCellProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WillWanderIntoCellProg")?.Value ?? "0")) ?? Gameworld.AlwaysTrueProg;
		IsWanderingProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("IsWanderingProg")?.Value ?? "0")) ?? Gameworld.AlwaysTrueProg;
		AllowDescentProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("AllowDescentProg")?.Value ?? "0")) ?? Gameworld.AlwaysFalseProg;
		WanderTimeDiceExpression = root.Element("WanderTimeDiceExpression")?.Value ?? "1d180+180";
		EmoteText = root.Element("EmoteText")?.Value ?? string.Empty;
		PreferredTreeLayer = (RoomLayer)int.Parse(root.Element("PreferredTreeLayer")?.Value ?? ((int)RoomLayer.HighInTrees).ToString());
		SecondaryTreeLayer = (RoomLayer)int.Parse(root.Element("SecondaryTreeLayer")?.Value ?? ((int)RoomLayer.InTrees).ToString());
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("WillWanderIntoCellProg", WillWanderIntoCellProg?.Id ?? 0),
			new XElement("IsWanderingProg", IsWanderingProg?.Id ?? 0),
			new XElement("AllowDescentProg", AllowDescentProg?.Id ?? 0),
			new XElement("WanderTimeDiceExpression", new XCData(WanderTimeDiceExpression)),
			new XElement("EmoteText", new XCData(EmoteText)),
			new XElement("PreferredTreeLayer", (int)PreferredTreeLayer),
			new XElement("SecondaryTreeLayer", (int)SecondaryTreeLayer),
			new XElement("OpenDoors", OpenDoors),
			new XElement("UseKeys", UseKeys),
			new XElement("SmashLockedDoors", SmashLockedDoors),
			new XElement("CloseDoorsBehind", CloseDoorsBehind),
			new XElement("UseDoorguards", UseDoorguards),
			new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay)
		).ToString();
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine($"Enabled Prog: {IsWanderingProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Wander Cell Prog: {WillWanderIntoCellProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Allow Descent Prog: {AllowDescentProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Preferred Tree Layer: {PreferredTreeLayer.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Secondary Tree Layer: {SecondaryTreeLayer.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Wander Delay: {WanderTimeDiceExpression.ColourValue()} seconds");
		sb.AppendLine($"Travel String: {EmoteText.ColourCommand()}");
		return sb.ToString();
	}

	protected override string TypeHelpText => $@"{base.TypeHelpText}
	#3enabled <prog>#0 - sets whether arboreal wandering is enabled
	#3room <prog>#0 - sets which cells are valid arboreal wander targets
	#3descent <prog>#0 - sets whether this AI may descend out of the trees into a target cell
	#3delay <expression>#0 - sets the delay between wander evaluations
	#3emote <text>#0 - sets the optional wander emote
	#3emote clear#0 - clears the wander emote
	#3preferred <layer>#0 - sets the preferred tree layer
	#3secondary <layer>#0 - sets the fallback tree layer";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "enabled":
			case "enabledprog":
				return BuildingCommandEnabledProg(actor, command);
			case "room":
			case "roomprog":
				return BuildingCommandRoomProg(actor, command);
			case "descent":
			case "descentprog":
				return BuildingCommandDescentProg(actor, command);
			case "delay":
				return BuildingCommandDelay(actor, command);
			case "emote":
				return BuildingCommandEmote(actor, command);
			case "preferred":
			case "preferredlayer":
				return BuildingCommandPreferredLayer(actor, command);
			case "secondary":
			case "secondarylayer":
				return BuildingCommandSecondaryLayer(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandEnabledProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control whether arboreal wandering is enabled?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, new[] { ProgVariableTypes.Character }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		IsWanderingProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} to determine whether it wanders.");
		return true;
	}

	private bool BuildingCommandRoomProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide which cells are suitable?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[]
			{
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location },
				new List<ProgVariableTypes> { ProgVariableTypes.Character, ProgVariableTypes.Location, ProgVariableTypes.Location }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WillWanderIntoCellProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} to evaluate wander cells.");
		return true;
	}

	private bool BuildingCommandDescentProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should decide when this AI may descend to the ground?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Location }).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AllowDescentProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {prog.MXPClickableFunctionName()} to gate descent.");
		return true;
	}

	private bool BuildingCommandDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !Dice.IsDiceExpression(command.SafeRemainingArgument))
		{
			actor.OutputHandler.Send("You must supply a valid dice expression.");
			return false;
		}

		WanderTimeDiceExpression = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now evaluate wandering every {WanderTimeDiceExpression.ColourValue()} seconds.");
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What emote should this AI use when wandering?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "delete", "remove"))
		{
			EmoteText = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This AI will no longer use a wander emote.");
			return true;
		}

		EmoteText = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use {EmoteText.ColourCommand()} as its wander emote.");
		return true;
	}

	private bool BuildingCommandPreferredLayer(ICharacter actor, StringStack command)
	{
		return BuildingCommandTreeLayer(actor, command, true);
	}

	private bool BuildingCommandSecondaryLayer(ICharacter actor, StringStack command)
	{
		return BuildingCommandTreeLayer(actor, command, false);
	}

	private bool BuildingCommandTreeLayer(ICharacter actor, StringStack command, bool preferred)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum<RoomLayer>(out var value) ||
		    !value.In(RoomLayer.InTrees, RoomLayer.HighInTrees))
		{
			actor.OutputHandler.Send("You must specify either #3InTrees#0 or #3HighInTrees#0."
				.SubstituteANSIColour());
			return false;
		}

		if (preferred)
		{
			PreferredTreeLayer = value;
		}
		else
		{
			SecondaryTreeLayer = value;
		}

		Changed = true;
		actor.OutputHandler.Send($"This AI will now {(preferred ? "prefer" : "fallback to")} {value.DescribeEnum().ColourValue()}.");
		return true;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		var ch = (type == EventType.EngagedInCombat ? arguments[1] : arguments[0]) as ICharacter;
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
			case EventType.CharacterEntersGame:
			case EventType.LeaveCombat:
			case EventType.FiveSecondTick:
				CreateEvaluateAffect(ch);
				break;
			case EventType.EngagedInCombat:
			case EventType.EngageInCombat:
				CancelEvaluateAffect(ch);
				break;
			case EventType.TenSecondTick:
				CheckPathingEffect(ch, true);
				break;
			case EventType.LayerChangeBlockExpired:
				CheckPathingEffect(ch, false);
				break;
		}

		return base.HandleEvent(type, arguments);
	}

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

	internal static bool CellSupportsTreeLayers(ICharacter character, ICell cell)
	{
		var terrain = cell.Terrain(character);
		return terrain?.TerrainLayers.Any(x => x.In(RoomLayer.InTrees, RoomLayer.HighInTrees)) == true;
	}

	private void CreateEvaluateAffect(ICharacter character)
	{
		if (character.State.HasFlag(CharacterState.Dead) ||
		    character.Effects.Any(x => x.IsEffectType<WandererWaiting>() || x.IsEffectType<FollowingPath>()) ||
		    character.Movement != null ||
		    character.Combat != null)
		{
			return;
		}

		character.AddEffect(new WandererWaiting(character, _ => EvaluateWander(character)),
			TimeSpan.FromSeconds(Dice.Roll(WanderTimeDiceExpression)));
	}

	private void CancelEvaluateAffect(ICharacter character)
	{
		character.RemoveAllEffects(x => x.IsEffectType<WandererWaiting>());
	}

	private void EvaluateWander(ICharacter character)
	{
		if (character.State.HasFlag(CharacterState.Dead) || !IsWanderingProg.ExecuteBool(false, character))
		{
			return;
		}

		if (character.Movement != null || character.EffectsOfType<FollowingPath>().Any())
		{
			CreateEvaluateAffect(character);
			return;
		}

		if (!CharacterState.Able.HasFlag(character.State) || character.Combat != null ||
		    character.Effects.Any(x => x.IsBlockingEffect("movement")))
		{
			CreateEvaluateAffect(character);
			return;
		}

		if (!CreatePathingEffectIfPathExists(character))
		{
			CreateEvaluateAffect(character);
		}
	}

	private bool CellMatchesProg(ICharacter character, ICell cell)
	{
		return WillWanderIntoCellProg?.ExecuteBool(false, character, cell, character.Location) ?? true;
	}

	private RoomLayer ChooseTreeLayer(ICharacter character, ICell cell)
	{
		var layers = cell.Terrain(character)?.TerrainLayers.ToList() ?? new List<RoomLayer>();
		if (layers.Contains(PreferredTreeLayer))
		{
			return PreferredTreeLayer;
		}

		if (layers.Contains(SecondaryTreeLayer))
		{
			return SecondaryTreeLayer;
		}

		if (layers.Contains(RoomLayer.HighInTrees))
		{
			return RoomLayer.HighInTrees;
		}

		if (layers.Contains(RoomLayer.InTrees))
		{
			return RoomLayer.InTrees;
		}

		return RoomLayer.GroundLevel;
	}

	protected override FollowingPath CreatePathingEffect(ICharacter ch, IEnumerable<ICellExit> path)
	{
		var destination = path.Last().Destination;
		var targetLayer = ChooseTreeLayer(ch, destination);
		return new FollowingMultiLayerPath(ch, path, targetLayer, targetLayer);
	}

	protected override (ICell Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		var treeTargets = ch.CellsAndDistancesInVicinity(10,
			GetSuitabilityFunction(ch, true),
			cell => CellMatchesProg(ch, cell) && CellSupportsTreeLayers(ch, cell))
			.ToList();

		var target = treeTargets.GetWeightedRandom(x => Math.Sqrt(x.Distance)).Cell;
		if (target is not null)
		{
			var path = ch.PathBetween(target, 10, GetSuitabilityFunction(ch, true)).ToList();
			if (path.Any())
			{
				return (target, path);
			}
		}

		var descentTargets = ch.CellsAndDistancesInVicinity(10,
			GetSuitabilityFunction(ch, true),
			cell => CellMatchesProg(ch, cell) &&
			        !CellSupportsTreeLayers(ch, cell) &&
			        (AllowDescentProg?.ExecuteBool(false, ch, cell) ?? false))
			.ToList();
		target = descentTargets.GetWeightedRandom(x => Math.Sqrt(x.Distance)).Cell;
		if (target is null)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		var descentPath = ch.PathBetween(target, 10, GetSuitabilityFunction(ch, true)).ToList();
		return descentPath.Any()
			? (target, descentPath)
			: (null, Enumerable.Empty<ICellExit>());
	}
}
