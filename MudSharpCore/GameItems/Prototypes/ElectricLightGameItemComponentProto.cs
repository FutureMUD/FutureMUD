using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class ElectricLightGameItemComponentProto : GameItemComponentProto
{
	protected ElectricLightGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "ElectricLight")
	{
		IlluminationProvided = 50;
		LightOnEmote = "@ light|lights up";
		LightOffEmote = "@ go|goes dark";
		Wattage = 40;
		Changed = true;
	}

	protected ElectricLightGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public double IlluminationProvided { get; protected set; }
	public double Wattage { get; protected set; }
	public string LightOnEmote { get; protected set; }
	public string LightOffEmote { get; protected set; }
	public IFutureProg OnLightProg { get; protected set; }
	public IFutureProg OnOffProg { get; protected set; }

	public override string TypeDescription => "ElectricLight";

	protected override void LoadFromXml(XElement root)
	{
		IlluminationProvided = double.Parse(root.Element("IlluminationProvided").Value);
		Wattage = double.Parse(root.Element("Wattage").Value);
		LightOnEmote = root.Element("LightOnEmote").Value;
		LightOffEmote = root.Element("LightOffEmote").Value;
		OnLightProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnLightProg").Value));
		OnOffProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnOffProg").Value));
	}

	private bool BuildingCommand_Wattage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many watts should this light use when powered on?");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var value))
		{
			actor.Send("How many watts should this light use when powered on?");
			return false;
		}

		if (value < 0)
		{
			actor.Send("You must enter a positive number of watts for this light to use.");
			return false;
		}

		Wattage = value;
		Changed = true;

		actor.Send("This light will now use {0:N2} watts when lit.", Wattage);
		return true;
	}

	private bool BuildingCommand_Illumination(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many lux of illumination should this light provide when lit?");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var value))
		{
			actor.Send("How many lux of illumination should this light provide when lit?");
			return false;
		}

		if (value <= 0)
		{
			actor.Send("Lights must provide a positive amount of illumination.");
			return false;
		}

		IlluminationProvided = value;
		Changed = true;
		actor.Send("This light will now provide {0:N2} lux of illumination when lit.", IlluminationProvided);
		return true;
	}

	private bool BuildingCommand_OnLightProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			if (OnLightProg != null)
			{
				OnLightProg = null;
				Changed = true;
				actor.Send("There will no longer be any prog executed when this light switches on.");
				return true;
			}

			actor.Send("Which prog do you want to execute when this light switches on?");
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

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Item }))
		{
			actor.Send("The prog must take only a single item as a parameter.");
			return false;
		}

		OnLightProg = prog;
		actor.Send($"This light will now execute prog {prog.FunctionName} (#{prog.Id}) when switched on.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_LightOnEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What emote do you want to set for this light switching on? Use $0 for the light.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for this light switching on is now \"{0}\"", command.RemainingArgument);
		LightOnEmote = command.RemainingArgument;
		Changed = true;
		return true;
	}

	private bool BuildingCommand_LightOffEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What emote do you want to set for this light switching off? Use $0 for the light.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for this light switching off is now \"{0}\"", command.RemainingArgument);
		LightOffEmote = command.RemainingArgument;
		Changed = true;
		return true;
	}

	private bool BuildingCommand_OnOffProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			if (OnOffProg != null)
			{
				OnOffProg = null;
				Changed = true;
				actor.Send("There will no longer be any prog executed when this light switches off.");
				return true;
			}

			actor.Send("Which prog do you want to execute when this light switches off?");
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

		if (!prog.MatchesParameters(new[] { FutureProgVariableTypes.Item }))
		{
			actor.Send("The prog must take only a single item as a parameter.");
			return false;
		}

		OnOffProg = prog;
		actor.Send($"This light will now execute prog {prog.FunctionName} (#{prog.Id}) when switched off.");
		Changed = true;
		return true;
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
		{
			case "wattage":
			case "watts":
			case "watt":
			case "power":
			case "pow":
				return BuildingCommand_Wattage(actor, command);
			case "lux":
			case "illumination":
				return BuildingCommand_Illumination(actor, command);
			case "lit":
			case "light":
				return BuildingCommand_LightOnEmote(actor, command);
			case "unlit":
			case "off":
				return BuildingCommand_LightOffEmote(actor, command);
			case "onlight":
				return BuildingCommand_OnLightProg(actor, command);
			case "onoff":
				return BuildingCommand_OnOffProg(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\twatts <watts> - the wattage of the light\n\tillumination <lux> - the illumination in lux provided by the light\n\tlit <emote> - the emote when the light turns on. Use $0 for the light item\n\tunlit <emote> - the emote when the light turns off. Use $0 for the light item\n\tonlight <prog> - sets a prog that executes when the light turns on\n\tonlight - clears an onlight prog\n\tonoff <prog> - sets a prog that executes when the light turns off\n\tonoff - clears an onoff prog";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Electric Light Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber}, {Name})\n\nThis is an electric light that provides {IlluminationProvided:N2} lux of illumination when lit. It uses {Wattage:N2} watts of power when lit.\n\nWhen lit, it executes the following prog: {(OnLightProg != null ? OnLightProg.FunctionName.Colour(Telnet.Cyan).FluentTagMXP("send", $"href='show futureprog {OnLightProg.Id}'") : "None".Colour(Telnet.Red))}\nWhen lit, it executes the following prog: {(OnOffProg != null ? OnOffProg.FunctionName.Colour(Telnet.Cyan).FluentTagMXP("send", $"href='show futureprog {OnOffProg.Id}'") : "None".Colour(Telnet.Red))}\nWhen turned on, it echoes: {LightOnEmote}\nWhen turned off, it echoes: {LightOffEmote}";
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", new XElement("IlluminationProvided", IlluminationProvided),
				new XElement("Wattage", Wattage),
				new XElement("OnLightProg", OnLightProg?.Id ?? 0),
				new XElement("OnOffProg", OnOffProg?.Id ?? 0),
				new XElement("LightOnEmote", new XCData(LightOnEmote)),
				new XElement("LightOffEmote", new XCData(LightOffEmote))
			).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("electric light", true,
			(gameworld, account) => new ElectricLightGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("electriclight", false,
			(gameworld, account) => new ElectricLightGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("electric_light", false,
			(gameworld, account) => new ElectricLightGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("elight", false,
			(gameworld, account) => new ElectricLightGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ElectricLight",
			(proto, gameworld) => new ElectricLightGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"ElectricLight",
			$"A {"[light source]".Colour(Telnet.BoldPink)} when {"[powered]".Colour(Telnet.Magenta)}, can be {"[lit]".Colour(Telnet.Red)} by players",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ElectricLightGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ElectricLightGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ElectricLightGameItemComponentProto(proto, gameworld));
	}
}