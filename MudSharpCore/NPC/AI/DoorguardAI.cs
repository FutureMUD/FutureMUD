using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Statements.Manipulation;
using System.Text;

namespace MudSharp.NPC.AI;

public enum WouldOpenResponseType
{
	WontOpen,
	WillOpenIfMove,
	WillOpenIfSocial,
	WillOpenIfKnock
}

public class WouldOpenResponse
{
	public WouldOpenResponseType Response { get; init; }
	public string Social { get; init; }
	public bool DirectionRequired { get; init; }
	public bool SocialTargetRequired { get; init; }
}

public class DoorguardAI : ArtificialIntelligenceBase
{
	protected IFutureProg BaseDelayProg;
	protected IFutureProg CantOpenDoorActionProg;
	protected IFutureProg CloseDoorActionProg;
	protected IFutureProg OnWitnessDoorStopProg;
	protected IFutureProg OpenCloseDelayProg;
	protected IFutureProg OpenDoorActionProg;
	protected bool OwnSideOnly;
	protected string RequiredSocialTrigger;
	protected bool RespectGameRulesForOpeningDoors;
	protected bool RespondToSocialDirection;
	protected bool SocialTargettedOnly;

	protected IFutureProg WillOpenDoorForProg;
	protected IFutureProg WontOpenDoorForActionProg;

	public WouldOpenResponse WouldOpen(ICharacter doorguard, ICharacter target, ICellExit direction)
	{
		if (doorguard.State.HasFlag(CharacterState.Dead))
		{
			return new WouldOpenResponse
			{
				Response = WouldOpenResponseType.WontOpen
			};
		}

		if (WillOpenDoorForProg?.Execute<bool?>(doorguard, target, direction) != true)
		{
			return new WouldOpenResponse
			{
				Response = WouldOpenResponseType.WontOpen
			};
		}

		if (doorguard.Location != direction.Origin)
		{
			if (OwnSideOnly)
			{
				return new WouldOpenResponse
				{
					Response = WouldOpenResponseType.WontOpen
				};
			}

			return new WouldOpenResponse
			{
				Response = WouldOpenResponseType.WillOpenIfKnock
			};
		}

		if (SocialTargettedOnly)
		{
			return new WouldOpenResponse
			{
				Response = WouldOpenResponseType.WillOpenIfSocial,
				Social = RequiredSocialTrigger,
				DirectionRequired = RespondToSocialDirection,
				SocialTargetRequired = SocialTargettedOnly
			};
		}

		return new WouldOpenResponse
		{
			Response = WouldOpenResponseType.WillOpenIfMove
		};
	}

	private DoorguardAI(ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	private DoorguardAI()
	{

	}

	private DoorguardAI(IFuturemud gameworld, string name) : base(gameworld, name, "Doorguard")
	{
		WillOpenDoorForProg = Gameworld.AlwaysFalseProg;
		BaseDelayProg = Gameworld.AlwaysOneHundredProg;
		OpenCloseDelayProg = Gameworld.AlwaysTenThousandProg;
		RespectGameRulesForOpeningDoors = true;
		DatabaseInitialise();
	}

	protected virtual void LoadFromXml(XElement root)
	{
		OwnSideOnly = bool.Parse(root.Element("OwnSideOnly")?.Value ?? "false");
		WillOpenDoorForProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("WillOpenDoorForProg")?.Value ?? "0"));
		WontOpenDoorForActionProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("WontOpenDoorForActionProg")?.Value ?? "0"));
		CantOpenDoorActionProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("CantOpenDoorActionProg")?.Value ?? "0"));
		OpenDoorActionProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OpenDoorActionProg")?.Value ?? "0"));
		CloseDoorActionProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("CloseDoorActionProg")?.Value ?? "0"));
		BaseDelayProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("BaseDelayProg")?.Value ?? "0"));
		OpenCloseDelayProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OpenCloseDelayProg")?.Value ?? "0"));
		OnWitnessDoorStopProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("OnWitnessDoorStopProg")?.Value ?? "0"));
		RespectGameRulesForOpeningDoors =
			bool.Parse(root.Element("RespectGameRulesForOpeningDoors")?.Value ?? "true");

		var element = root.Element("Social");
		if (element != null)
		{
			RequiredSocialTrigger = element.Attribute("Trigger").Value;
			SocialTargettedOnly = bool.Parse(element.Attribute("TargettedOnly").Value);
			RespondToSocialDirection = bool.Parse(element.Attribute("Direction").Value);
		}
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("BaseDelayProg", BaseDelayProg?.Id ?? 0L),
			new XElement("CantOpenDoorActionProg", CantOpenDoorActionProg?.Id ?? 0L),
			new XElement("CloseDoorActionProg", CloseDoorActionProg?.Id ?? 0L),
			new XElement("OnWitnessDoorStopProg", OnWitnessDoorStopProg?.Id ?? 0L),
			new XElement("OpenCloseDelayProg", OpenCloseDelayProg?.Id ?? 0L),
			new XElement("OpenDoorActionProg", OpenDoorActionProg?.Id ?? 0L),
			new XElement("RequiredSocialTrigger", new XCData(RequiredSocialTrigger ?? "")),
			new XElement("OwnSideOnly", OwnSideOnly),
			new XElement("RespectGameRulesForOpeningDoors", RespectGameRulesForOpeningDoors),
			new XElement("RespondToSocialDirection", RespondToSocialDirection),
			new XElement("SocialTargettedOnly", SocialTargettedOnly),
			new XElement("WillOpenDoorForProg", WillOpenDoorForProg?.Id ?? 0L),
			new XElement("WontOpenDoorForActionProg", WontOpenDoorForActionProg?.Id ?? 0L)
		).ToString();
	}
	public static void RegisterLoader()
	{
		RegisterAIType("Doorguard", (ai, gameworld) => new DoorguardAI(ai, gameworld));
		RegisterAIBuilderInformation("doorguard", (gameworld, name) => new DoorguardAI(gameworld, name), new DoorguardAI().HelpText);
	}

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Artificial Intelligence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Type: {AIType.ColourValue()}");
		sb.AppendLine();
		sb.AppendLine($"Will Open Prog: {WillOpenDoorForProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Won't Open Prog: {WontOpenDoorForActionProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Can't Open Prog: {CantOpenDoorActionProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Base Delay Prog: {BaseDelayProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Close Delay Prog: {OpenCloseDelayProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Open Action Prog: {OpenDoorActionProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Close Action Prog: {CloseDoorActionProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Witness Stop Prog: {OnWitnessDoorStopProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Own Side Only: {OwnSideOnly.ToColouredString()}");
		sb.AppendLine($"Respect Game Rules For Doors: {RespectGameRulesForOpeningDoors.ToColouredString()}");
		sb.AppendLine($"Respond to Social Direction: {RespondToSocialDirection.ToColouredString()}");
		sb.AppendLine($"Social Targetted Only: {SocialTargettedOnly.ToColouredString()}");
		sb.AppendLine($"Required Social: {RequiredSocialTrigger?.ColourCommand() ?? ""}");
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string TypeHelpText => 
		@"	#3basedelay <prog>#0 - a prog that controls the millisecond delay on command responses
	#3closedelay <prog>#0 - a prog that controls the millisecond delay before closing opened doors
	#3will <prog>#0 - sets the prog that controls whether the guard will open the door for someone
	#3wont <prog>#0 - sets a prog executed when the guard won't open the door for someone
	#3wont clear#0 - clears the won't open door prog
	#3cant <prog>#0 - sets a prog executed when the guard can't open the door for someone
	#3cant clear#0 - clears the can't open door prog
	#3open <prog>#0 - sets a prog executed when the guard wants to open the door
	#3open clear#0 - clears the open door prog
	#3close <prog>#0 - sets a prog executed when this guard wants to close the door
	#3close clear#0 - clears the close door prog
	#3stop <prog>#0 - sets a prog executed when this guard sees someone run in to the door
	#3stop clear#0 - clears the stop prog
	#3social <which>#0 - sets the social required to trigger the doorguard
	#3social clear#0 - clears the social required to trigger the doorguard
	#3socialonly#0 - toggles only responding to the social (ignoring movement)
	#3ownside#0 - toggles ignoring requests from the other side of the door (e.g. knocks)
	#3respectrules#0 - toggles using in-game engine logic to open doors (as opposed to leaving it to the progs)
	#3socialdirection#0 - toggles using directional queues from socials to cover which door they'll open";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "basedelay":
			case "basedelayprog":
				return BuildingCommandBaseDelay(actor, command);
			case "closedelay":
			case "closedelayprog":
				return BuildingCommandCloseDelay(actor, command);
			case "will":
			case "willopen":
			case "willopenprog":
				return BuildingCommandWillOpenProg(actor, command);
			case "wont":
			case "won't":
			case "wontopen":
			case "won'topen":
			case "wontopenprog":
			case "won'topenprog":
				return BuildingCommandWontOpenProg(actor, command);
			case "cant":
			case "can't":
			case "cantopen":
			case "can'topen":
			case "cantopenprog":
			case "can'topenprog":
				return BuildingCommandCantOpenProg(actor, command);
			case "openaction":
			case "open":
			case "openactionprog":
			case "openprog":
				return BuildingCommandOpenActionProg(actor, command);
			case "closeaction":
			case "close":
			case "closeactionprog":
			case "closeprog":
				return BuildingCommandCloseActionProg(actor, command);
			case "stop":
			case "stopprog":
			case "witnessstop":
			case "onstop":
			case "onwitnessstop":
			case "witnessstopprog":
			case "onstopprog":
			case "onwitnessstopprog":
				return BuildingCommandOnWitnessStopProg(actor, command);
			case "social":
				return BuildingCommandSocial(actor, command);
			case "socialonly":
				return BuildingCommandSocialOnly(actor);
			case "ownside":
			case "ownsideonly":
				return BuildingCommandOwnSideOnly(actor);
			case "respectrules":
			case "respectgamerules":
				return BuildingCommandRespectGameRules(actor);
			case "socialdirection":
				return BuildingCommandSocialDirection(actor);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandSocialDirection(ICharacter actor)
	{
		RespondToSocialDirection = !RespondToSocialDirection;
		Changed = true;
		actor.OutputHandler.Send($"This AI will {RespondToSocialDirection.NowNoLonger()} respond to directional cues in socials.");
		return true;
	}

	private bool BuildingCommandRespectGameRules(ICharacter actor)
	{
		RespectGameRulesForOpeningDoors = !RespectGameRulesForOpeningDoors;
		Changed = true;
		if (RespectGameRulesForOpeningDoors)
		{
			actor.OutputHandler.Send("This AI will now respect the game rules for opening doors. This means it will try to open doors using built-in commands rather than relying on the on-action prog.");
		}
		else
		{
			actor.OutputHandler.Send("This AI will no longer respect the game rules for opening doors. This means that only the #6CanOpenProg#0 and #6OnOpenActionProg#0 will do all the actual checking and manipulation.".SubstituteANSIColour());
		}
		return true;
	}

	private bool BuildingCommandOwnSideOnly(ICharacter actor)
	{
		OwnSideOnly = !OwnSideOnly;
		Changed = true;
		if (OwnSideOnly)
		{
			actor.OutputHandler.Send("This AI will now only respond to actions from its own side - socials or movement depending on other settings, but it will ignore knock.");
		}
		else
		{
			actor.OutputHandler.Send("This AI will once again respond to knocks and interactions from the other side of its door.");
		}
		return true;
	}

	private bool BuildingCommandSocialOnly(ICharacter actor)
	{
		SocialTargettedOnly = !SocialTargettedOnly;
		Changed = true;
		if (SocialTargettedOnly)
		{
			actor.OutputHandler.Send("This AI will now only respond to socials rather than movement to trigger opening the door.");
		}
		else
		{
			actor.OutputHandler.Send("This AI will once again respond to movement for the opening of its door.");
		}
		return true;
	}

	private bool BuildingCommandSocial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a social that must be used or use #3clear#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "delete", "remove", "none"))
		{
			RequiredSocialTrigger = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This AI will no longer respond to social triggers.");
			return true;
		}

		var text = command.SafeRemainingArgument;
		var social = actor.Gameworld.Socials.FirstOrDefault(x => x.Name.EqualTo(text));
		if (social is null)
		{
			actor.OutputHandler.Send("There is no social like that.");
			return false;
		}

		RequiredSocialTrigger = social.Name;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now respond to the {RequiredSocialTrigger.ColourName()} social.");
		return true;
	}

	private bool BuildingCommandOnWitnessStopProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog or use #3clear#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			OnWitnessDoorStopProg = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will no longer execute any prog when it witnesses someone walk into a door it could have opened.");
			return true;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Void, new[]
			{
				new[] { FutureProgVariableTypes.Character, },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character, FutureProgVariableTypes.Exit }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		OnWitnessDoorStopProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now execute the {prog.MXPClickableFunctionName()} prog when it witnesses someone run into a door that it could have opened for them.");
		return true;
	}

	private bool BuildingCommandCloseActionProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog or use #3clear#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			CloseDoorActionProg = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will no longer execute any prog when it's time to close the door. Make sure there is some other means of closing the door afterwards, or the AI is using the #6Respect Game Rules#0 setting.");
			return true;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Void, new[]
			{
				new[] { FutureProgVariableTypes.Character, },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character, FutureProgVariableTypes.Exit }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		CloseDoorActionProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now execute the {prog.MXPClickableFunctionName()} prog when it's time to close the door.");
		return true;
	}

	private bool BuildingCommandOpenActionProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog or use #3clear#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			OpenDoorActionProg = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will no longer execute any prog when it wants to open the door. Make sure the AI is using the #6Respect Game Rules#0 setting.");
			return true;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Void, new[]
			{
				new[] { FutureProgVariableTypes.Character, },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character, FutureProgVariableTypes.Exit }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		OpenDoorActionProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now execute the {prog.MXPClickableFunctionName()} prog when it wants to open the door.");
		return true;
	}

	private bool BuildingCommandCantOpenProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog or use #3clear#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			CantOpenDoorActionProg = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will no longer execute any prog when it can't open the door (like if it's missing keys or whatnot).");
			return true;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Void, new[]
			{
				new[] { FutureProgVariableTypes.Character, },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character, FutureProgVariableTypes.Exit }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		CantOpenDoorActionProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now execute the {prog.MXPClickableFunctionName()} prog when it can't open the door (like if it's missing keys or whatnot).");
		return true;
	}

	private bool BuildingCommandWontOpenProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog or use #3clear#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
		{
			WontOpenDoorForActionProg = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will no longer execute any prog when it won't open the door for someone but they request it (via social or knock etc).");
			return true;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Void, new[]
			{
				new[] { FutureProgVariableTypes.Character, },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character, FutureProgVariableTypes.Exit }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WontOpenDoorForActionProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now execute the {prog.MXPClickableFunctionName()} prog when it won't open the door for someone but they request it (via social or knock etc).");
		return true;
	}

	private bool BuildingCommandWillOpenProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new[]
			{
				new[] { FutureProgVariableTypes.Character, },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character, FutureProgVariableTypes.Exit }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WillOpenDoorForProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the {prog.MXPClickableFunctionName()} prog to determine whether it will open the door for someone.");
		return true;
	}

	private bool BuildingCommandCloseDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Number, new[]
			{
				new[] { FutureProgVariableTypes.Character, },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character, FutureProgVariableTypes.Exit }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		OpenCloseDelayProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the {prog.MXPClickableFunctionName()} prog to determine how many milliseconds to wait before closing the door after opening it.");
		return true;
	}

	private bool BuildingCommandBaseDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			FutureProgVariableTypes.Number, new[]
			{
				new[] { FutureProgVariableTypes.Character, },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character },
				new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Character, FutureProgVariableTypes.Exit }
			}).LookupProg();
		if (prog is null)
		{
			return false;
		}

		BaseDelayProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now use the {prog.MXPClickableFunctionName()} prog to determine how many milliseconds to wait before responding to movememnt, socials, or knocks.");
		return true;
	}

	protected virtual bool OnWitnessMove(ICharacter doorguard, ICharacter mover, ICellExit exit)
	{
		if (doorguard.State.HasFlag(CharacterState.Dead))
		{
			return false;
		}

		if (exit.Exit.Door == null || !string.IsNullOrEmpty(RequiredSocialTrigger))
		{
			return false;
		}

		if (!doorguard.AffectedBy<IDoorguardModeEffect>() ||
		    doorguard.AffectedBy<IDoorguardOpeningDoorEffect>())
		{
			return false;
		}

		if ((bool?)WillOpenDoorForProg.Execute(doorguard, mover, exit) ?? false)
		{
			// TODO - can open might need to be more AI-based than capability based
			if (RespectGameRulesForOpeningDoors && !exit.Exit.Door.IsOpen && !exit.Exit.Door.CanOpen(doorguard.Body))
			{
				CantOpenDoorActionProg?.Execute(doorguard, mover, exit);
				return true;
			}

			var baseDelay = Convert.ToDouble(BaseDelayProg.Execute(doorguard, mover, exit));
			doorguard.AddEffect(new DoorguardOpenDoor(doorguard,
					perceivable => { OpenDoorActionProg.Execute(doorguard, mover, exit); }),
				TimeSpan.FromMilliseconds(baseDelay));

			doorguard.AddEffect(new DoorguardOpeningDoor(doorguard));

			doorguard.AddEffect(new DoorguardCloseDoor(doorguard,
					perceivable => { CloseDoorIfStillOpen(doorguard, mover, exit); }),
				TimeSpan.FromMilliseconds(baseDelay +
				                          Convert.ToDouble(OpenCloseDelayProg.Execute(doorguard, mover, exit))));
			return true;
		}

		return false;
	}

	protected virtual bool OnWitnessSocial(ICharacter doorguard, ICharacter socialite, string social,
		bool socialTarget, ICellExit socialDirection)
	{
		if (doorguard.State.HasFlag(CharacterState.Dead))
		{
			return false;
		}

		if (string.IsNullOrEmpty(RequiredSocialTrigger) || (SocialTargettedOnly && !socialTarget))
		{
			return false;
		}

		if (!doorguard.AffectedBy<IDoorguardModeEffect>() ||
		    doorguard.AffectedBy<IDoorguardOpeningDoorEffect>())
		{
			return false;
		}

		if (!((bool?)WillOpenDoorForProg.Execute(doorguard, socialite, socialDirection) ?? false))
		{
			if (WontOpenDoorForActionProg != null)
			{
				WontOpenDoorForActionProg.Execute(doorguard, socialite, socialDirection);
				return true;
			}

			return false;
		}

		var exit = RespondToSocialDirection ? socialDirection : null;
		if (exit == null)
		{
			foreach (var direction in doorguard.Location.ExitsFor(doorguard))
			{
				if (direction.Exit.Door?.IsOpen == false &&
				    ((bool?)WillOpenDoorForProg.Execute(doorguard, socialite, direction) ?? false))
				{
					exit = direction;
					break;
				}
			}
		}

		if (exit?.Exit.Door == null)
		{
			return false;
		}

		// TODO - can open might need to be more AI-based than capability based
		if (RespectGameRulesForOpeningDoors && !exit.Exit.Door.IsOpen && !exit.Exit.Door.CanOpen(doorguard.Body))
		{
			CantOpenDoorActionProg?.Execute(doorguard, socialite, exit);
			return true;
		}

		var baseDelay = Convert.ToDouble(BaseDelayProg.Execute(doorguard, socialite, exit));
		doorguard.AddEffect(new DoorguardOpenDoor(doorguard,
				perceiver => { OpenDoorActionProg.Execute(doorguard, socialite, exit); }),
			TimeSpan.FromMilliseconds(baseDelay));
		doorguard.AddEffect(new DoorguardOpeningDoor(doorguard));
		doorguard.AddEffect(new DoorguardCloseDoor(doorguard,
				perceiver => { CloseDoorIfStillOpen(doorguard, socialite, exit); }),
			TimeSpan.FromMilliseconds(baseDelay +
			                          Convert.ToDouble(OpenCloseDelayProg.Execute(doorguard, socialite, exit))));
		return true;
	}

	protected virtual void CloseDoorIfStillOpen(ICharacter doorguard, ICharacter mover, ICellExit exit)
	{
		if (doorguard.State.HasFlag(CharacterState.Dead) || doorguard.Location == null || exit == null)
		{
			return;
		}

		if (exit.Exit.Door?.IsOpen == true)
		{
			if (doorguard.Location.Characters.SelectNotNull(x => x.Movement).Any(x => x.Exit == exit))
			{
				doorguard.AddEffect(new DoorguardCloseDoor(doorguard,
					perceivable => { CloseDoorIfStillOpen(doorguard, mover, exit); }
				), TimeSpan.FromSeconds(3));
				return;
			}

			CloseDoorActionProg?.Execute(doorguard, mover, exit);
		}

		doorguard.RemoveAllEffects(
			x =>
				x.IsEffectType<IDoorguardOpeningDoorEffect>() || x.IsEffectType<DoorguardCloseDoor>());
	}

	protected virtual bool OnWitnessLeave(ICharacter doorguard, ICharacter mover, ICellExit exit)
	{
		if (doorguard.State.HasFlag(CharacterState.Dead))
		{
			return false;
		}

		if (!doorguard.AffectedBy<IDoorguardOpeningDoorEffect>())
		{
			return false;
		}

		CloseDoorIfStillOpen(doorguard, mover, exit);
		return true;
	}

	protected virtual bool OnStopMovementWitness(ICharacter doorguard, ICharacter mover, ICellExit exit)
	{
		if (doorguard.State.HasFlag(CharacterState.Dead))
		{
			return false;
		}

		if (!doorguard.AffectedBy<IDoorguardModeEffect>())
		{
			return false;
		}

		CloseDoorIfStillOpen(doorguard, mover, exit);
		return true;
	}

	protected virtual bool OnStopMovementClosedDoorWitness(ICharacter doorguard, ICharacter mover, ICellExit exit)
	{
		if (doorguard.State.HasFlag(CharacterState.Dead))
		{
			return false;
		}

		if (!doorguard.AffectedBy<IDoorguardModeEffect>())
		{
			return false;
		}

		if (OnWitnessDoorStopProg != null)
		{
			OnWitnessDoorStopProg?.Execute(doorguard, mover, exit);
			return true;
		}

		return false;
	}

	protected virtual bool OnDoorKnock(ICharacter doorguard, ICharacter knocker, ICellExit exit)
	{
		if (doorguard.State.HasFlag(CharacterState.Dead))
		{
			return false;
		}

		if (OwnSideOnly || !doorguard.AffectedBy<IDoorguardModeEffect>() || exit.Exit.Door == null ||
		    exit.Exit.Door.IsOpen)
		{
			return false;
		}

		if (!((bool?)WillOpenDoorForProg?.Execute(doorguard, knocker, exit) ?? false))
		{
			if (WontOpenDoorForActionProg != null)
			{
				WontOpenDoorForActionProg.Execute(doorguard, knocker, exit);
				return true;
			}

			return false;
		}

		if (!exit.Exit.Door.CanOpen(doorguard.Body))
		{
			if (CantOpenDoorActionProg != null)
			{
				CantOpenDoorActionProg.Execute(doorguard, knocker, exit);
				return true;
			}

			return false;
		}

		var baseDelay = Convert.ToDouble(BaseDelayProg.Execute(doorguard, knocker, exit));
		doorguard.AddEffect(
			new DoorguardOpenDoor(doorguard, perceivable => { OpenDoorActionProg.Execute(doorguard, knocker, exit); }),
			TimeSpan.FromMilliseconds(baseDelay));
		doorguard.AddEffect(new DoorguardOpeningDoor(doorguard));
		doorguard.AddEffect(new DoorguardCloseDoor(doorguard,
				perceiver => { CloseDoorIfStillOpen(doorguard, knocker, exit); }
			),
			TimeSpan.FromMilliseconds(
				baseDelay + Convert.ToDouble(OpenCloseDelayProg.Execute(doorguard, knocker, exit))));
		return true;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.CharacterStopMovementWitness:
				return OnStopMovementWitness(arguments[3], arguments[0], arguments[2]);
			case EventType.CharacterStopMovementClosedDoorWitness:
				return OnStopMovementClosedDoorWitness(arguments[3], arguments[0], arguments[2]);
			case EventType.CharacterBeginMovementWitness:
				return OnWitnessMove(arguments[3], arguments[0], arguments[2]);
			case EventType.CharacterLeaveCellWitness:
				return OnWitnessLeave(arguments[3], arguments[0], arguments[2]);
			case EventType.CharacterSocialTarget:
				return OnWitnessSocial(arguments[2], arguments[0], arguments[1].Name, true, arguments[3]);
			case EventType.CharacterSocialWitness:
				return OnWitnessSocial(arguments[4], arguments[0], arguments[1].Name, false, arguments[3]);
			case EventType.CharacterDoorKnockedOtherSide:
				return OnDoorKnock(arguments[3], arguments[0], arguments[2]);
			default:
				return false;
		}
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.CharacterStopMovementWitness:
				case EventType.CharacterStopMovementClosedDoorWitness:
				case EventType.CharacterBeginMovementWitness:
				case EventType.CharacterLeaveCellWitness:
				case EventType.CharacterSocialTarget:
				case EventType.CharacterSocialWitness:
				case EventType.CharacterDoorKnockedOtherSide:
					return true;
			}
		}

		return false;
	}
}