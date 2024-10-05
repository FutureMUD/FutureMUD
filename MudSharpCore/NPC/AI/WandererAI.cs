using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Movement;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.NPC.AI;

public class WandererAI : ArtificialIntelligenceBase
{
	protected string EmoteText;
	protected bool OpenDoors;
	protected IBodyPrototype TargetBodyPrototype;
	protected IMoveSpeed TargetMoveSpeed;
	protected string WanderTimeDiceExpression;
	protected IFutureProg WillWanderIntoCellProg;
	protected IFutureProg IsWanderingProg;

	public WandererAI(ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	protected WandererAI(IFuturemud gameworld, string name) : base(gameworld, name, "Wanderer")
	{
		EmoteText = string.Empty;
		OpenDoors = false;
		WanderTimeDiceExpression = "1d40+100";
		WillWanderIntoCellProg = Gameworld.AlwaysTrueProg;
		TargetBodyPrototype = null;
		IsWanderingProg = Gameworld.AlwaysTrueProg;
		TargetMoveSpeed = TargetBodyPrototype?.Speeds.FirstOrDefault(x => x.Position == PositionStanding.Instance);
		DatabaseInitialise();
	}

	private WandererAI() : base()
	{

	}

	public static void RegisterLoader()
	{
		RegisterAIType("Wanderer", (ai, gameworld) => new WandererAI(ai, gameworld));
		RegisterAIBuilderInformation("wanderer", (gameworld,name) => new WandererAI(gameworld, name), new WandererAI().HelpText);
	}

	private void LoadFromXml(XElement root)
	{
		WillWanderIntoCellProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("FutureProg")?.Value ?? "0"));
		IsWanderingProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("IsWanderingProg")?.Value ?? "0")) ?? Gameworld.AlwaysTrueProg;
		WanderTimeDiceExpression = root.Element("WanderTimeDiceExpression")?.Value ?? "1d40+100";
		TargetBodyPrototype = Gameworld.BodyPrototypes.Get(long.Parse(root.Element("TargetBody")?.Value ?? "0"));
		TargetMoveSpeed = TargetBodyPrototype?.Speeds.Get(long.Parse(root.Element("TargetSpeed")?.Value ?? "0"));
		EmoteText = root.Element("EmoteText")?.Value;
		var element = root.Element("OpenDoors");
		OpenDoors = element != null && bool.Parse(element.Value);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("FutureProg", WillWanderIntoCellProg?.Id ?? 0),
			new XElement("WanderTimeDiceExpression", new XCData(WanderTimeDiceExpression)),
			new XElement("TargetBody", TargetBodyPrototype?.Id ?? 0),
			new XElement("TargetSpeed", TargetMoveSpeed?.Id ?? 0),
			new XElement("OpenDoors", OpenDoors),
			new XElement("EmoteText", new XCData(EmoteText))
		).ToString();
	}

	private void CreateEvaluateAffect(ICharacter character, int seconds = 10)
	{
		if (character.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		if (character.Effects.Any(x => x.IsEffectType<WandererWaiting>()))
		{
			return;
		}

		if (character.Movement != null || character.Combat != null)
		{
			return;
		}

		character.AddEffect(new WandererWaiting(character, actor => EvaluateWander(character)),
			TimeSpan.FromSeconds(seconds));
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

		IEnumerable<ICellExit> options =
			character.Location.ExitsFor(character)
					 .Where(
						 x => (bool?)WillWanderIntoCellProg?.Execute(character, x.Destination, character.Location) !=
							  false)
					 .Where(
						 x =>
							 x.Exit.Door?.IsOpen != false || (x.Exit.Door.CanOpen(character.Body) && OpenDoors))
					 .ToList();

		if (!options.Any())
		{
			CreateEvaluateAffect(character);
			return;
		}

		if (character.PositionState.MoveRestrictions != MovementAbility.Free)
		{
			var upright = character.MostUprightMobilePosition();
			if (upright == null)
			{
				CreateEvaluateAffect(character);
				return;
			}

			character.MovePosition(upright, null, null);
		}

		var choice = options.GetRandomElement();
		if (!character.Move(choice, !string.IsNullOrEmpty(EmoteText) ? new Emote(EmoteText, character) : null))
		{
			CreateEvaluateAffect(character);
			return;
		}

		if (!(choice.Exit?.Door?.IsOpen ?? true) && OpenDoors)
		{
			character.Body.Open(choice.Exit.Door, null, null);
		}
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		var ch = type == EventType.EngagedInCombat ?
			(ICharacter)arguments[1] :
			(ICharacter)arguments[0];
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
				CreateEvaluateAffect(arguments[0], Dice.Roll(WanderTimeDiceExpression));
				return false;

			case EventType.EngagedInCombat:
				CancelEvaluateAffect(arguments[1]);
				return false;
			case EventType.EngageInCombat:
				CancelEvaluateAffect(arguments[0]);
				return false;

			case EventType.CharacterEntersGame:
			case EventType.LeaveCombat:
				var character = (ICharacter)arguments[0];
				if (character.Body.Prototype.CountsAs(TargetBodyPrototype) && TargetMoveSpeed != null)
				{
					character.CurrentSpeeds[PositionStanding.Instance] = TargetMoveSpeed;
				}

				CreateEvaluateAffect(arguments[0], Dice.Roll(WanderTimeDiceExpression));
				return false;

			case EventType.FiveSecondTick:
				CreateEvaluateAffect(arguments[0]);
				return false;
		}

		return false;
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
					return true;
			}
		}

		return false;
	}

	#region Overrides of ArtificialIntelligenceBase
	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Artificial Intelligence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Type: {AIType.ColourValue()}");
		sb.AppendLine();
		sb.AppendLine($"Target Body: {TargetBodyPrototype?.Name.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine($"Target Move Speed: {TargetMoveSpeed?.Name.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine($"Is Enabled: {IsWanderingProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Will Wander Room: {WillWanderIntoCellProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Wander Time Dice: {WanderTimeDiceExpression.ColourValue()} seconds");
		sb.AppendLine($"Open Doors: {OpenDoors.ToColouredString()}");
		sb.AppendLine($"Travel String: {EmoteText?.ColourCommand() ?? ""}");
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string TypeHelpText => @"	#3doors#0 - toggles this AI opening doors
	#3body <proto>#0 - sets the body prototype this AI applies to
	#3speed <which>#0 - sets the target speed for wandering
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
			case "body":
				return BuildingCommandBody(actor, command);
			case "speed":
				return BuildingCommandSpeed(actor, command);
			case "room":
			case "roomprog":
				return BuildingCommandRoomProg(actor, command);
			case "enabled":
			case "enabledprog":
				return BuildingCommandEnabledProg(actor, command);
			case "doors":
				return BuildingCommandDoors(actor);
		}
		return base.BuildingCommand(actor, command.GetUndo());
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
			FutureProgVariableTypes.Boolean, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character
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
			FutureProgVariableTypes.Boolean, new []{
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Location,
				},
				new List<FutureProgVariableTypes>
				{
					FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Location,
					FutureProgVariableTypes.Location,
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

	private bool BuildingCommandSpeed(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What movement speed should the AI try to use?");
			return false;
		}

		if (TargetBodyPrototype is null)
		{
			actor.OutputHandler.Send("You must first set a target body prototype before you set a speed.");
			return false;
		}

		var move = TargetBodyPrototype.Speeds.GetByIdOrName(command.SafeRemainingArgument);
		if (move is null)
		{
			actor.OutputHandler.Send($"The {TargetBodyPrototype.Name.ColourName()} body has no such movement speed.");
			return false;
		}

		TargetMoveSpeed = move;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now try to move at the {TargetMoveSpeed.Name.ColourName()} speed.");
		return true;
	}

	private bool BuildingCommandBody(ICharacter actor, StringStack ss)
	{
		var npcs = Gameworld.NPCs.OfType<INPC>().Where(x => x.AIs.Contains(this)).ToList();
		var templates = Gameworld.NpcTemplates
								 .GetAllApprovedOrMostRecent()
								 .Where(x => x.Status.In(RevisionStatus.Current, RevisionStatus.PendingRevision,
									 RevisionStatus.UnderDesign))
								 .Where(x => x.ArtificialIntelligences.Contains(this))
								 .ToList();
		if (npcs.Any() || templates.Any())
		{
			var sb = new StringBuilder();
			sb.AppendLine($"This property cannot be edited while the AI is attached to any NPCs or NPC Templates.");
			if (templates.Any())
			{
				sb.AppendLine();
				sb.AppendLine($"The following NPC Templates are causing issues:");
				sb.AppendLine(StringUtilities.GetTextTable(
				from npc in templates
					select new List<string>
					{
						$"{npc.Id.ToString("N0", actor)}r{npc.RevisionNumber.ToString("N0", actor)}",
						npc.Name,
						npc.Status.DescribeColour()
					},
				new List<string>
				{
					"Id",
					"Name",
					"Status"
				},
				actor,
				Telnet.Orange
					));
			}

			if (npcs.Any())
			{
				sb.AppendLine();
				sb.AppendLine($"The following NPCs are causing issues:");
				sb.AppendLine(StringUtilities.GetTextTable(
					from npc in npcs
					select new List<string>
					{
						npc.Id.ToString("N0", actor),
						npc.PersonalName.GetName(NameStyle.FullName),
						npc.HowSeen(npc, flags: PerceiveIgnoreFlags.IgnoreObscured | PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreCanSee),
						npc.Location.GetFriendlyReference(actor),
						$"{npc.Template.Id.ToString("N0", actor)}r{npc.Template.RevisionNumber.ToString("N0", actor)} ({npc.Name.ColourName()})"
					},
					new List<string>
					{
						"Id",
						"Name",
						"SDesc",
						"Location",
						"Template"
					},
					actor,
					Telnet.Orange
				));
			}

			actor.OutputHandler.Send(sb.ToString());
			return false;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which body prototype should this AI be designed to work for?");
			return false;
		}

		var proto = Gameworld.BodyPrototypes.GetByIdOrName(ss.SafeRemainingArgument);
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such body prototype.");
			return false;
		}

		TargetBodyPrototype = proto;
		if (!proto.Speeds.Contains(TargetMoveSpeed))
		{
			TargetMoveSpeed = proto.Speeds
								   .Where(x => x.Position == TargetMoveSpeed?.Position)
								   .FirstMin(x => Math.Abs(x.Multiplier - TargetMoveSpeed?.Multiplier ?? 0));
		}

		actor.OutputHandler.Send($"This AI is now designed to function with the {proto.Name.ColourName()} body prototype.");
		Changed = true;
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