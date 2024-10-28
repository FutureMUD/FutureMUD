using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class PoweredPropGameItemComponentProto : PoweredMachineBaseGameItemComponentProto
{
	public override string TypeDescription => "PoweredProp";

	#region Constructors

	protected PoweredPropGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"PoweredProp")
	{
	}

	protected PoweredPropGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		TenSecondProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("TenSecondProg").Value));
	}

	#endregion

	#region Saving

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		root.Add(new XElement("TenSecondProg", TenSecondProg?.Id ?? 0));
		return root;
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new PoweredPropGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new PoweredPropGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("PoweredProp".ToLowerInvariant(), true,
			(gameworld, account) => new PoweredPropGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("PoweredProp",
			(proto, gameworld) => new PoweredPropGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"PoweredProp",
			$"A {"[powered]".Colour(Telnet.Magenta)} machine that can execute a prog when running",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new PoweredPropGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\twattage <watts> - set power usage\n\tdiscount <watts> - a wattage discount per quality\n\tswitchable - toggles whether players can switch this on\n\tonemote <emote> - sets the emote when powered on. Use $0 for the machine.\n\toffemote <emote> - sets the emote when powered down. Use $0 for the machine.\n\tonprog <prog> - sets a prog to execute when the machine is powered on\n\toffprog <prog> - sets a prog to execute when the machine is powered down\n\ttenprog <prog> - sets a prog to execute every 10 seconds when on";

	public override string ShowBuildingHelp => BuildingHelpText;

	private bool BuildingCommandTenSecondProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which prog do you want to execute every 10 seconds while this machine is powered and on?");
			return false;
		}

		if (command.Peek().EqualToAny("clear", "none"))
		{
			TenSecondProg = null;
			Changed = true;
			actor.OutputHandler.Send(
				"This machine will no longer execute any prog every 10 seconds while switched on.");
			return true;
		}

		var prog = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.Last);
		if (prog == null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		if (!prog.MatchesParameters(new List<ProgVariableTypes> { ProgVariableTypes.Item }))
		{
			actor.OutputHandler.Send("The prog you specify must accept a single item parameter.");
			return false;
		}

		TenSecondProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This machine will now execute the prog {TenSecondProg.MXPClickableFunctionNameWithId()} every 10 seconds while switched on.");
		return true;
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "tensecondprog":
			case "10secondprog":
			case "recurringprog":
			case "ten second prog":
			case "ten_second_prog":
			case "10_second_prog":
			case "10 second prog":
			case "tenprog":
			case "ten_prog":
			case "ten prog":
			case "10prog":
			case "10_prog":
			case "10 prog":
				return BuildingCommandTenSecondProg(actor, command);
			default:
				return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
		}
	}

	#endregion

	public IFutureProg TenSecondProg { get; protected set; }

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return $"Ten Second Prog: {TenSecondProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}";
	}

	protected override string ComponentDescriptionOLCByline => "This is a general-purpose machine that consumes power";
}