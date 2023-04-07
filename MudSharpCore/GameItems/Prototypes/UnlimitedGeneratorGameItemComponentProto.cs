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
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class UnlimitedGeneratorGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "UnlimitedGenerator";

	public double WattageProvided { get; protected set; }
	public string SwitchOnEmote { get; protected set; }
	public string SwitchOffEmote { get; protected set; }
	public IFutureProg SwitchOnProg { get; protected set; }
	public IFutureProg SwitchOffProg { get; protected set; }

	#region Constructors

	protected UnlimitedGeneratorGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "UnlimitedGenerator")
	{
		Changed = true;
		WattageProvided = 1000000;
		SwitchOnEmote = "@ switch|switches $1 on";
		SwitchOffEmote = "@ switch|switches $1 off";
	}

	protected UnlimitedGeneratorGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		SwitchOnEmote = root.Element("SwitchOnEmote").Value;
		SwitchOffEmote = root.Element("SwitchOffEmote").Value;
		WattageProvided = double.Parse(root.Element("WattageProvided").Value);
		SwitchOnProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("SwitchOnProg").Value));
		SwitchOffProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("SwitchOffProg").Value));
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
				new XElement("SwitchOnEmote", new XCData(SwitchOnEmote)),
				new XElement("SwitchOffEmote", new XCData(SwitchOffEmote)),
				new XElement("WattageProvided", WattageProvided),
				new XElement("SwitchOnProg", SwitchOnProg?.Id ?? 0),
				new XElement("SwitchOffProg", SwitchOffProg?.Id ?? 0)
			).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new UnlimitedGeneratorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new UnlimitedGeneratorGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("UnlimitedGenerator".ToLowerInvariant(), true,
			(gameworld, account) => new UnlimitedGeneratorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("UnlimitedGenerator",
			(proto, gameworld) => new UnlimitedGeneratorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"UnlimitedGenerator",
			$"An item that {"[produces power]".Colour(Telnet.BoldMagenta)} by its very nature",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new UnlimitedGeneratorGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText = @"You can use the following options with this component:

	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
    watts <watts> - how many watts supplied when operating
    onemote <emote> - sets the echo when switched on. $0 for the person turning it on, $1 for the generator item.
    offemote <emote> - sets the echo when switched off. $0 for the person turning it off, $1 for the generator item.
    onprog <prog> - sets a prog to be executed when the generator switches on
    offprog <prog> - sets a prog to be executed when the generator switches off";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "onemote":
				return BuildingCommand_OnEmote(actor, command);
			case "onprog":
				return BuildingCommand_OnProg(actor, command);
			case "offemote":
				return BuildingCommand_OffEmote(actor, command);
			case "offprog":
				return BuildingCommand_OffProg(actor, command);
			case "wattage":
			case "watts":
			case "watt":
			case "power":
				return BuildingCommand_Wattage(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommand_Wattage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many watts should this generator produce when switched on?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.Send("You must enter a valid number of watts.");
			return false;
		}

		if (value <= 0)
		{
			actor.Send("You must enter a positive number of watts for this generator to produce.");
			return false;
		}

		WattageProvided = value;
		actor.Send($"This generator now produces {WattageProvided:N2} watts of power.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_OffProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			if (SwitchOffProg != null)
			{
				SwitchOffProg = null;
				Changed = true;
				actor.Send("There will no longer be any prog executed when this generator is switched off.");
				return true;
			}

			actor.Send("Which prog do you want to execute when this generator is switched off?");
			return false;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.Send("There is no such prog by that name or ID.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item }))
		{
			actor.Send("The prog must take only a single character and item as a parameter.");
			return false;
		}

		SwitchOffProg = prog;
		actor.Send($"This generator will now execute prog {prog.FunctionName} (#{prog.Id}) when it is switched off.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_OffEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What emote do you want to set for the turning off of this generator? Use $0 for the lightee, $1 for the generator.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for turning this generator off is now \"{0}\"", command.RemainingArgument);
		SwitchOffEmote = command.RemainingArgument;
		Changed = true;
		return true;
	}

	private bool BuildingCommand_OnProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			if (SwitchOnProg != null)
			{
				SwitchOnProg = null;
				Changed = true;
				actor.Send("There will no longer be any prog executed when this generator is switched on.");
				return true;
			}

			actor.Send("Which prog do you want to execute when this generator is switched on?");
			return false;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.Send("There is no such prog by that name or ID.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Character, FutureProgVariableTypes.Item }))
		{
			actor.Send("The prog must take only a single character and item as a parameter.");
			return false;
		}

		SwitchOnProg = prog;
		actor.Send($"This generator will now execute prog {prog.FunctionName} (#{prog.Id}) when it is switched on.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_OnEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What emote do you want to set for the turning on of this generator? Use $0 for the lightee, $1 for the generator.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for turning this generator on is now \"{0}\"", command.RemainingArgument);
		SwitchOnEmote = command.RemainingArgument;
		Changed = true;
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item needs a description.",
			"UnlimitedGenerator Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name
		);
	}
}