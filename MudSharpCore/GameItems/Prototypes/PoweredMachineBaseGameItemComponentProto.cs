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

public abstract class PoweredMachineBaseGameItemComponentProto : GameItemComponentProto
{
	#region Constructors

	protected PoweredMachineBaseGameItemComponentProto(IFuturemud gameworld, IAccount originator, string type) : base(
		gameworld, originator, type)
	{
		Switchable = true;
		Wattage = 650;
		WattageDiscountPerQuality = 30;
		PowerOnEmote = "@ hum|hums briefly as it powers on";
		PowerOffEmote = "@ shudder|shudders as it powers down.";
	}

	protected PoweredMachineBaseGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		Wattage = double.Parse(root.Element("Wattage").Value);
		WattageDiscountPerQuality = double.Parse(root.Element("WattageDiscount").Value);
		Switchable = bool.Parse(root.Element("Switchable").Value);
		PowerOnEmote = root.Element("PowerOnEmote").Value;
		PowerOffEmote = root.Element("PowerOffEmote").Value;
		OnPoweredProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnPoweredProg").Value));
		OnUnpoweredProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnUnpoweredProg").Value));
	}

	#endregion

	#region Saving

	protected abstract XElement SaveSubtypeToXml(XElement root);

	protected sealed override string SaveToXml()
	{
		return SaveSubtypeToXml(new XElement("Definition",
			new XElement("Wattage", Wattage),
			new XElement("WattageDiscount", WattageDiscountPerQuality),
			new XElement("Switchable", Switchable),
			new XElement("PowerOnEmote", new XCData(PowerOnEmote)),
			new XElement("PowerOffEmote", new XCData(PowerOffEmote)),
			new XElement("OnPoweredProg", OnPoweredProg?.Id ?? 0),
			new XElement("OnUnpoweredProg", OnUnpoweredProg?.Id ?? 0)
		)).ToString();
	}

	#endregion

	#region Building Commands

	private bool BuildingCommand_Wattage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many watts should this machine use when powered on?");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var value))
		{
			actor.Send("How many watts should this machine use when powered on?");
			return false;
		}

		if (value < 0)
		{
			actor.Send("You must enter a positive number of watts for this machine to use.");
			return false;
		}

		Wattage = value;
		Changed = true;

		actor.Send($"This machine will now use {Wattage.ToString("N2", actor).ColourValue()} watts when switched on.");
		return true;
	}

	private bool BuildingCommandOnProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which prog do you want to execute when this machine switches on?");
			return false;
		}

		if (command.Peek().EqualToAny("none", "clear"))
		{
			OnPoweredProg = null;
			Changed = true;
			actor.Send("There will no longer be any prog executed when this machine switches on.");
			return true;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.Send("There is no such prog by that name or ID.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { ProgVariableTypes.Item }))
		{
			actor.Send("The prog must take only a single item as a parameter.");
			return false;
		}

		OnPoweredProg = prog;
		actor.Send($"This machine will now execute prog {prog.MXPClickableFunctionName()} when switched on.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandOnEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for this machine becoming powered? Use $0 for the machine.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for this machine becoming powered is now \"{0}\"",
			command.RemainingArgument.ColourCommand());
		PowerOnEmote = command.RemainingArgument;
		Changed = true;
		return true;
	}

	private bool BuildingCommandOffEmote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What emote do you want to set for this machine becoming unpowered? Use $0 for the machine.");
			return false;
		}

		// This line is essentially just leveraging Emote.ScoutTargets to make sure they didn't make a bad emote
		var emote = new Emote(command.RemainingArgument, actor, actor);
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}

		actor.Send("The emote for this machine becoming unpowered is now \"{0}\"",
			command.RemainingArgument.ColourCommand());
		PowerOffEmote = command.RemainingArgument;
		Changed = true;
		return true;
	}

	private bool BuildingCommandOffProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which prog do you want to execute when this machine switches off?");
			return false;
		}

		if (command.Peek().EqualToAny("clear", "none"))
		{
			OnUnpoweredProg = null;
			Changed = true;
			actor.Send("There will no longer be any prog executed when this machine switches off.");
			return true;
		}

		var prog = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.FutureProgs.Get(value)
			: Gameworld.FutureProgs.GetByName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.Send("There is no such prog by that name or ID.");
			return false;
		}

		if (!prog.MatchesParameters(new[] { ProgVariableTypes.Item }))
		{
			actor.Send("The prog must take only a single item as a parameter.");
			return false;
		}

		OnUnpoweredProg = prog;
		actor.Send($"This machine will now execute prog {prog.MXPClickableFunctionName()} when switched off.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandSwitchable(ICharacter actor, StringStack command)
	{
		Switchable = !Switchable;
		Changed = true;
		actor.OutputHandler.Send(
			$"This machine is {(Switchable ? "now" : "no longer")} switchable on and off by players.");
		return true;
	}

	private bool BuildingCommandQualityDiscount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How many watts less power should this machine use per point of quality?\nNote: Quality ranges from 0-12 and 5 is 'normal' quality.");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid number.");
			return false;
		}

		WattageDiscountPerQuality = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This machine will now use {WattageDiscountPerQuality.ToString("N2", actor).ColourValue()} watts less power per point of quality.");
		if (Wattage - WattageDiscountPerQuality * 12 <= 0.0)
		{
			actor.OutputHandler.Send(
				"Please note that the value you have selected could result in nil or negative power usage for certain qualities.");
		}

		return true;
	}

	public override string ShowBuildingHelp =>
		"You can use the following options with this command:\n\twattage <watts> - set power usage\n\tdiscount <watts> - a wattage discount per quality\n\tswitchable - toggles whether players can switch this on\n\tonemote <emote> - sets the emote when powered on. Use $0 for the machine.\n\toffemote <emote> - sets the emote when powered down. Use $0 for the machine.\n\tonprog <prog> - sets a prog to execute when the machine is powered on\n\toffprog <prog> - sets a prog to execute when the machine is powered down";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "wattage":
			case "watts":
			case "watt":
			case "power":
			case "pow":
				return BuildingCommand_Wattage(actor, command);
			case "discount":
				return BuildingCommandQualityDiscount(actor, command);
			case "on":
			case "onemote":
			case "on emote":
			case "on_emote":
				return BuildingCommandOnEmote(actor, command);
			case "off":
			case "offemote":
			case "off emote":
			case "off_emote":
				return BuildingCommandOffEmote(actor, command);
			case "onprog":
			case "on prog":
			case "on_prog":
				return BuildingCommandOnProg(actor, command);
			case "offprog":
			case "off prog":
			case "off_prog":
				return BuildingCommandOffProg(actor, command);
			case "switchable":
				return BuildingCommandSwitchable(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	#endregion

	public double Wattage { get; protected set; }
	public double WattageDiscountPerQuality { get; protected set; }
	public string PowerOnEmote { get; protected set; }
	public string PowerOffEmote { get; protected set; }
	public IFutureProg OnPoweredProg { get; protected set; }
	public IFutureProg OnUnpoweredProg { get; protected set; }
	public bool Switchable { get; protected set; }

	protected abstract string ComponentDescriptionOLCByline { get; }

	protected abstract string ComponentDescriptionOLCAddendum(ICharacter actor);

	public sealed override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1}r{2}, {3})\r\n\r\n{4}. It consumes {5} watts of power and {6}.\nPower On Emote: {7}\nPower Off Emote: {8}\nPower On Prog: {9}\nPower Off Prog {10}\n{11}",
			$"{TypeDescription} Game Item Component".Colour(Telnet.Cyan),
			Id.ToString("N0", actor),
			RevisionNumber.ToString("N0", actor),
			Name,
			ComponentDescriptionOLCByline,
			$"{Wattage.ToString("N2", actor)}-(quality*{WattageDiscountPerQuality.ToString("N2", actor)})"
				.ColourValue(),
			Switchable ? "can be switched on by players" : "can only be switched on by progs",
			PowerOnEmote.ColourCommand(),
			PowerOffEmote.ColourCommand(),
			OnPoweredProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red),
			OnUnpoweredProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red),
			ComponentDescriptionOLCAddendum(actor)
		);
	}
}