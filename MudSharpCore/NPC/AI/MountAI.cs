using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Statements.Manipulation;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using Parlot.Fluent;

namespace MudSharp.NPC.AI;
#nullable enable
public class MountAI : ArtificialIntelligenceBase, IMountableAI
{
	public static void RegisterLoader()
	{
		RegisterAIType("Mount", (ai, gameworld) => new MountAI(ai, gameworld));
		RegisterAIBuilderInformation("mount", (gameworld, name) => new MountAI(gameworld, name), new MountAI().HelpText);
	}

	private MountAI()
	{
	}

	protected MountAI(ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	protected MountAI(IFuturemud gameworld, string name) : base(gameworld, name, "Mount")
	{
		MaximumNumberOfRiders = 1;
		RawMountEmote = "$1 $1|mount|mounts $0.";
		RawDismountEmote = "$1 $1|dismount|dismounts $0.";
		RawBuckEmote = "$0 try|tries to buck $1!";
		RawControlDeniedEmote = "$0 refuse|refuses to obey $1's order.";
		MountNonConsensualMountDifficulty = Difficulty.Impossible;
		MountControlDifficulty = Difficulty.Trivial;
		MountResistBuckDifficulty = Difficulty.Normal;
		DatabaseInitialise();
	}

	private void LoadFromXml(XElement root)
	{
		PermitRiderProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("PermitRiderProg").Value));
		PermitControlProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("PermitControlProg").Value));
		WhyCannotPermitRiderProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WhyCannotPermitRiderProg").Value));
		MountNonConsensualMountDifficulty = (Difficulty)int.Parse(root.Element("MountNonConsensualMountDifficulty").Value);
		MountControlDifficulty = (Difficulty)int.Parse(root.Element("MountControlDifficulty").Value);
		MountResistBuckDifficulty = (Difficulty)int.Parse(root.Element("MountResistBuckDifficulty").Value);
		MaximumNumberOfRiders = int.Parse(root.Element("MaximumNumberOfRiders").Value);
		RawMountEmote = root.Element("RawMountEmote").Value;
		RawDismountEmote = root.Element("RawDismountEmote").Value;
		RawControlDeniedEmote = root.Element("RawControlDeniedEmote").Value;
		RawMountEmote = root.Element("RawMountEmote").Value;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("PermitRiderProg", PermitRiderProg?.Id ?? 0),
			new XElement("PermitControlProg", PermitControlProg?.Id ?? 0),
			new XElement("WhyCannotPermitRiderProg", WhyCannotPermitRiderProg?.Id ?? 0),
			new XElement("MountNonConsensualMountDifficulty", (int)MountNonConsensualMountDifficulty),
			new XElement("MountControlDifficulty", (int)MountControlDifficulty),
			new XElement("MountResistBuckDifficulty", (int)MountResistBuckDifficulty), 
			new XElement("RawMountEmote", new XCData(RawMountEmote)),
			new XElement("RawDismountEmote", new XCData(RawDismountEmote)),
			new XElement("RawBuckEmote", new XCData(RawBuckEmote)),
			new XElement("RawControlDeniedEmote", new XCData(RawControlDeniedEmote)),
			new XElement("MaximumNumberOfRiders", MaximumNumberOfRiders)
		).ToString();
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				default:
					return false;
			}
		}

		return false;
	}

	public IFutureProg? PermitRiderProg { get; private set; }
	public IFutureProg? PermitControlProg { get; private set; }
	public IFutureProg? WhyCannotPermitRiderProg { get; private set; }
	public Difficulty MountNonConsensualMountDifficulty { get; private set; }
	public Difficulty MountControlDifficulty { get; private set; }
	public Difficulty MountResistBuckDifficulty { get; private set; }
	public string RawMountEmote { get; private set; }
	public string RawDismountEmote { get; private set; }
	public string RawBuckEmote { get; private set; }
	public string RawControlDeniedEmote { get; private set; }

	#region Building Commands

	#region Overrides of ArtificialIntelligenceBase

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine($"Permit Rider Prog: {PermitRiderProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Permit Control Prog: {PermitControlProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Why Can't Ride Prog: {WhyCannotPermitRiderProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Non-Consensual Mount Difficulty: {MountNonConsensualMountDifficulty.DescribeColoured()}");
		sb.AppendLine($"Control Difficulty: {MountControlDifficulty.DescribeColoured()}");
		sb.AppendLine($"Resist Buck Difficulty: {MountResistBuckDifficulty.DescribeColoured()}");
		sb.AppendLine($"Maximum Riders: {MaximumNumberOfRiders.ToStringN0Colour(actor)}");
		sb.AppendLine($"Mount Emote: {RawMountEmote.ColourCommand()}");
		sb.AppendLine($"Dismount Emote: {RawDismountEmote.ColourCommand()}");
		sb.AppendLine($"Buck Emote: {RawBuckEmote.ColourCommand()}");
		sb.AppendLine($"Resist Emote: {RawControlDeniedEmote.ColourCommand()}");
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string TypeHelpText => $@"	#3permit <prog>#0 - sets the prog for whether a rider can mount
	#3why <prog>#0 - sets the prog for the error message if a rider can't mount
	#3control <prog>#0 - sets the prog for controlling a ridden mount
	#3difficultymount <difficulty>#0 - sets the difficulty to non-consensually mount a mount
	#3difficultycontrol <difficulty>#0 - sets the difficulty to control the mount
	#3difficultybuck <difficulty>#0 - sets the difficulty to resist being bucked
	#3mountemote <emote>#0 - sets the mount emote ($0 = mount, $1 = rider)
	#3dismountemote <emote>#0 - sets the dismount emote ($0 = mount, $1 = rider)
	#3buckemote <emote>#0 - sets the mount buck emote ($0 = mount, $1 = rider)
	#3resistemote <emote>#0 - sets the control resistance emote ($0 = mount, $1 = rider)
	#3riders <##>#0 - sets the number of riders that can ride at once";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "permit":
			case "permitrider":
			case "permitriderprog":
			case "permitprog":
				return BuildingCommandPermitRiderProg(actor, command);
			case "permitcontrol":
			case "permitcontrolprog":
			case "control":
			case "controlprog":
				return BuildingCommandPermitControlProg(actor, command);
			case "why":
			case "whycant":
			case "whycantprog":
				return BuildingCommandWhyCantProg(actor, command);
			case "difficultymount":
			case "diffmount":
			case "difficultyride":
			case "diffride":
				return BuildingCommandDifficultyMount(actor, command);
			case "difficultycontrol":
			case "diffcontrol":
				return BuildingCommandDifficultyControl(actor, command);
			case "difficultybuck":
			case "diffbuck":
				return BuildingCommandDifficultyBuck(actor, command);
			case "mountemote":
			case "emotemount":
				return BuildingCommandMountEmote(actor, command);
			case "dismountemote":
			case "emotedismount":
				return BuildingCommandDismountEmote(actor, command);
			case "buckemote":
			case "emotebuck":
				return BuildingCommandBuckEmote(actor, command);
			case "resistemote":
			case "emoteresist":
				return BuildingCommandResistEmote(actor, command);
			case "maxriders":
			case "riders":
				return BuildingCommandMaxRiders(actor, command);

		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandMaxRiders(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a number of riders.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var number) || number < 1)
		{
			actor.OutputHandler.Send($"You must enter a number 1 or greater.");
			return false;
		}

		MaximumNumberOfRiders = number;
		Changed = true;
		actor.OutputHandler.Send($"This mount can now hold a maximum of {number.ToStringN0Colour(actor)} {"rider".Pluralise(number != 1)}");
		return true;
	}

	private bool BuildingCommandDismountEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify an emote.");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		RawDismountEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The emote when dismounting this mount is now {RawDismountEmote.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandBuckEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify an emote.");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		RawBuckEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The emote when this mount bucks its rider is now {RawBuckEmote.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandResistEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify an emote.");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		RawControlDeniedEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The emote when this mount resists its rider is now {RawControlDeniedEmote.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandMountEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify an emote.");
			return false;
		}

		var emote = new Emote(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		RawMountEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"The emote when a rider mounts this mount is now {RawMountEmote.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandDifficultyBuck(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What difficulty do you want to set?\nValid difficulties are {Enum.GetValues<Difficulty>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var difficulty))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid difficulty.\nValid difficulties are {Enum.GetValues<Difficulty>().ListToColouredString()}.");
			return false;
		}

		MountResistBuckDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send($"The difficulty to resist being bucked off this mount is now {difficulty.DescribeColoured()}.");
		return true;
	}

	private bool BuildingCommandDifficultyControl(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What difficulty do you want to set?\nValid difficulties are {Enum.GetValues<Difficulty>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var difficulty))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid difficulty.\nValid difficulties are {Enum.GetValues<Difficulty>().ListToColouredString()}.");
			return false;
		}

		MountControlDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send($"The difficulty to control this mount is now {difficulty.DescribeColoured()}.");
		return true;
	}

	private bool BuildingCommandDifficultyMount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What difficulty do you want to set?\nValid difficulties are {Enum.GetValues<Difficulty>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Difficulty>(out var difficulty))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid difficulty.\nValid difficulties are {Enum.GetValues<Difficulty>().ListToColouredString()}.");
			return false;
		}

		MountNonConsensualMountDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send($"The difficulty to non-consensually mount this mount is now {difficulty.DescribeColoured()}.");
		return true;
	}

	private bool BuildingCommandWhyCantProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Text, [[ProgVariableTypes.Character, ProgVariableTypes.Character], [ProgVariableTypes.Character]]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		WhyCannotPermitRiderProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"The prog {prog.MXPClickableFunctionName()} is now used to set the error message for why a mount can't be mounted.");
		return true;
	}

	private bool BuildingCommandPermitControlProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, [[ProgVariableTypes.Character, ProgVariableTypes.Character], [ProgVariableTypes.Character]]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		PermitControlProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"The prog {prog.MXPClickableFunctionName()} is now used to control whether a mount can be controlled by a rider.");
		return true;
	}

	private bool BuildingCommandPermitRiderProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, [[ProgVariableTypes.Character, ProgVariableTypes.Character], [ProgVariableTypes.Character]]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		PermitRiderProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"The prog {prog.MXPClickableFunctionName()} is now used to control whether a mount can be mounted by a rider.");
		return true;
	}

	#endregion

	#endregion

	#region Implementation of IMountableAI

	/// <inheritdoc />
	public bool PermitRider(ICharacter mount, ICharacter rider)
	{
		return PermitRiderProg?.ExecuteBool(rider, mount) == true;
	}

	/// <inheritdoc />
	public string WhyCannotPermitRider(ICharacter mount, ICharacter rider)
	{
		return WhyCannotPermitRiderProg?.ExecuteString(rider, mount) ?? $"You cannot ride {mount.HowSeen(rider)}.";
	}

	/// <inheritdoc />
	public Difficulty NonConsensualMountDifficulty(ICharacter mount, ICharacter rider)
	{
		return MountNonConsensualMountDifficulty;
	}

	/// <inheritdoc />
	public int MaximumNumberOfRiders { get; private set; }

	/// <inheritdoc />
	public int RiderSlots => MaximumNumberOfRiders;

	/// <inheritdoc />
	public int RiderSlotsOccupiedBy(ICharacter rider)
	{
		return 1;
	}

	/// <inheritdoc />
	public Difficulty ControlDifficulty(ICharacter mount, ICharacter rider)
	{
		return MountControlDifficulty;
	}

	/// <inheritdoc />
	public Difficulty ResistBuckDifficulty(ICharacter mount, ICharacter rider)
	{
		return MountResistBuckDifficulty;
	}

	/// <inheritdoc />
	public string MountEmote(ICharacter mount, ICharacter rider)
	{
		return RawMountEmote;
	}

	/// <inheritdoc />
	public string DismountEmote(ICharacter mount, ICharacter rider)
	{
		return RawDismountEmote;
	}

	/// <inheritdoc />
	public string BuckEmote(ICharacter mount, ICharacter rider)
	{
		return RawBuckEmote;
	}

	/// <inheritdoc />
	public bool PermitControl(ICharacter mount, ICharacter rider)
	{
		return PermitControlProg?.ExecuteBool(rider, mount) == true;
	}

	/// <inheritdoc />
	public void HandleDeniedControl(ICharacter mount, ICharacter rider)
	{
		rider.OutputHandler.Send(new EmoteOutput(new Emote(RawControlDeniedEmote, mount, mount, rider)));
	}

	#endregion
}
