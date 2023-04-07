using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class ExternalOrganGameItemComponentProto : GameItemComponentProto
{
	private readonly List<IOrganProto> _organs = new();

	public bool OxygenatesBlood { get; protected set; }

	public IBodyPrototype Body { get; protected set; }

	public IEnumerable<IOrganProto> Organs => _organs;

	public string AddendumDescription { get; protected set; }

	public ConnectorType VenousConnection { get; protected set; }
	public ConnectorType ArterialConnection { get; protected set; }

	public double BasePowerConsumptionInWatts { get; protected set; }

	public double PowerConsumptionDiscountPerQuality { get; protected set; }

	public string SwitchOnEmote { get; protected set; }

	public string SwitchOffEmote { get; protected set; }

	public override string TypeDescription => "ExternalOrgan";

	#region Constructors

	protected ExternalOrganGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "ExternalOrgan")
	{
		AddendumDescription = "";
		VenousConnection = new ConnectorType(Gameworld.GetStaticConfiguration("DefaultExternalOrganVenousConnector"));
		ArterialConnection =
			new ConnectorType(Gameworld.GetStaticConfiguration("DefaultExternalOrganArterialConnector"));
		Body = Gameworld.BodyPrototypes.Get(Gameworld.GetStaticLong("DefaultExternalOrganBody"));
		BasePowerConsumptionInWatts = 500;
		PowerConsumptionDiscountPerQuality = 25;
		SwitchOnEmote = "@ rumble|rumbles to life as its components become powered.";
		SwitchOffEmote = "@ clunk|clunks off as its components become unpowered.";
	}

	protected ExternalOrganGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		Body = Gameworld.BodyPrototypes.Get(long.Parse(root.Element("Body").Value));
		OxygenatesBlood = bool.Parse(root.Element("OxygenatesBlood").Value);
		AddendumDescription = root.Element("Addendum").Value;
		VenousConnection = new ConnectorType(root.Element("VenousConnection").Value);
		ArterialConnection = new ConnectorType(root.Element("ArterialConnection").Value);
		BasePowerConsumptionInWatts = double.Parse(root.Element("BasePowerConsumptionInWatts").Value);
		PowerConsumptionDiscountPerQuality = double.Parse(root.Element("PowerConsumptionDiscountPerQuality").Value);
		SwitchOnEmote = root.Element("SwitchOnEmote").Value;
		SwitchOffEmote = root.Element("SwitchOffEmote").Value;
		foreach (var item in root.Element("Organs").Elements().Select(x => long.Parse(x.Value)))
		{
			var organ = Body?.Organs.FirstOrDefault(x => x.Id == item);
			if (organ != null)
			{
				_organs.Add(organ);
			}
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Body", Body?.Id ?? 0),
			new XElement("OxygenatesBlood", OxygenatesBlood),
			new XElement("Addendum", new XCData(AddendumDescription)),
			new XElement("VenousConnection", VenousConnection.ToString()),
			new XElement("ArterialConnection", ArterialConnection.ToString()),
			new XElement("BasePowerConsumptionInWatts", BasePowerConsumptionInWatts),
			new XElement("PowerConsumptionDiscountPerQuality", PowerConsumptionDiscountPerQuality),
			new XElement("SwitchOnEmote", new XCData(SwitchOnEmote)),
			new XElement("SwitchOffEmote", new XCData(SwitchOffEmote)),
			new XElement("Organs", from organ in Organs select new XElement("Organ", organ.Id))
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ExternalOrganGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ExternalOrganGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ExternalOrgan".ToLowerInvariant(), true,
			(gameworld, account) => new ExternalOrganGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("cpbm", false,
			(gameworld, account) => new ExternalOrganGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ExternalOrgan",
			(proto, gameworld) => new ExternalOrganGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"ExternalOrgan",
			$"This is a {"[powered]".Colour(Telnet.Magenta)} machine that is {"[connectable]".Colour(Telnet.BoldBlue)} to a cannula, and provides organ function to a character",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ExternalOrganGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tbody <body> - sets which body this machine is designed for\n\toxygenate - toggles whether this machine oxygenates the blood\n\torgan <organ> - toggles whether a particular organ function is provided\n\taddendum <text> - an addendum to the item description when working\n\tvein <gender> <name> - sets the venous cannula connection\n\tarterial <gender> <name> - sets the arterial cannula connection\n\twatts <watts> - sets the wattage of this machine when in use\n\tpoweron <emote> - sets the emote when this machine is powered on\n\tpoweroff <emote> - sets the emote when this machine is powered off";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "body":
			case "proto":
			case "bodyproto":
			case "body proto":
			case "body type":
			case "target body":
			case "target":
				return BuildingCommandBody(actor, command);
			case "oxygen":
			case "oxygenate":
				return BuildingCommandOxygenate(actor, command);
			case "organ":
				return BuildingCommandOrgan(actor, command);
			case "addendum":
			case "additional":
				return BuildingCommandAddendum(actor, command);
			case "connect":
			case "conn":
			case "connection":
			case "vein":
			case "venous":
			case "vein connection":
				return BuildingCommandVenousConnection(actor, command);
			case "artery":
			case "arterial":
			case "arterial connection":
			case "artery connection":
				return BuildingCommandArterialConnection(actor, command);
			case "switchon":
			case "on":
				return BuildingCommandSwitchOn(actor, command);
			case "switchoff":
			case "off":
				return BuildingCommandSwitchOff(actor, command);
			case "power":
			case "watts":
			case "watt":
			case "wattage":
				return BuildingCommandWattage(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandWattage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What wattage would you like this item to consume as a base amount?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var wattage))
		{
			actor.OutputHandler.Send("That is not a valid amount of wattage for this item to consume.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How many watts of power usage should be discounted for each point of quality this item has? (Hint: Normal quality is 5 quality)");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var discount))
		{
			actor.OutputHandler.Send("That is not a valid amount of wattage discount for this item.");
			return false;
		}

		BasePowerConsumptionInWatts = wattage;
		PowerConsumptionDiscountPerQuality = discount;
		Changed = true;
		actor.OutputHandler.Send(
			$"When switched on, this item will now consume {BasePowerConsumptionInWatts.ToString("N2", actor).Colour(Telnet.Green)} watts of power minus {PowerConsumptionDiscountPerQuality.ToString("N2", actor).Colour(Telnet.Green)} watts per point of quality.");
		return true;
	}

	private bool BuildingCommandSwitchOff(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What emote do you want this item to do when it is switched off or unpowered? Hint, $0 is the item.");
			return false;
		}

		SwitchOffEmote = command.RemainingArgument;
		Changed = true;
		actor.OutputHandler.Send("You change the switch off emote for this item.");
		return true;
	}

	private bool BuildingCommandSwitchOn(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What emote do you want this item to do when it is switched on and powered up? Hint, $0 is the item.");
			return false;
		}

		SwitchOnEmote = command.RemainingArgument;
		Changed = true;
		actor.OutputHandler.Send("You change the switch off emote for this item.");
		return true;
	}

	private bool BuildingCommandBody(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which body prototype should this machine target?");
			return false;
		}

		var body = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.BodyPrototypes.Get(value)
			: Gameworld.BodyPrototypes.GetByName(command.SafeRemainingArgument);
		if (body == null)
		{
			actor.OutputHandler.Send("There is no such body prototype.");
			return false;
		}

		Body = body;
		Changed = true;
		actor.OutputHandler.Send(
			$"This machine is now designed for entities with the {Body.Name.Colour(Telnet.Cyan)} body type.");
		return true;
	}

	private bool BuildingCommandVenousConnection(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What gender of connection do you want to set?");
			return false;
		}

		var gendering = Gendering.Get(command.Pop());
		if (gendering.Enum == Form.Shape.Gender.Indeterminate)
		{
			actor.Send("You can either set the connection type to male, female or neuter.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send("What type of connection do you want this connector to be?");
			return false;
		}

		var type = command.PopSpeech();
		type =
			Gameworld.ItemComponentProtos.Except(this)
			         .OfType<IConnectableItemProto>()
			         .SelectMany(x => x.Connections.Select(y => y.ConnectionType))
			         .FirstOrDefault(x => x.EqualTo(type) && !x.Equals(type, StringComparison.InvariantCulture)) ??
			type;

		VenousConnection = new ConnectorType(gendering.Enum, type, false);
		actor.Send(
			$"This machine will now have a venous connection of type {type.Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandArterialConnection(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What gender of arterial connection do you want to set?");
			return false;
		}

		var gendering = Gendering.Get(command.Pop());
		if (gendering.Enum == Form.Shape.Gender.Indeterminate)
		{
			actor.Send("You can either set the connection type to male, female or neuter.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send("What type of connection do you want this connector to be?");
			return false;
		}

		var type = command.PopSpeech();
		type =
			Gameworld.ItemComponentProtos.Except(this)
			         .OfType<IConnectableItemProto>()
			         .SelectMany(x => x.Connections.Select(y => y.ConnectionType))
			         .FirstOrDefault(x => x.EqualTo(type) && !x.Equals(type, StringComparison.InvariantCulture)) ??
			type;

		ArterialConnection = new ConnectorType(gendering.Enum, type, false);
		actor.Send(
			$"This machine will now have an arterial connection of type {type.Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandAddendum(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What addendum do you want to add to the description of the item when this machine is switched on?");
			return false;
		}

		AddendumDescription = command.SafeRemainingArgument.Fullstop().ProperSentences().SubstituteANSIColour();
		actor.OutputHandler.Send(
			$"The following addendum will now be added to the description of this item when the machine is switched on:\n\n{AddendumDescription}");
		Changed = true;
		return true;
	}

	private bool BuildingCommandOrgan(ICharacter actor, StringStack command)
	{
		if (Body == null)
		{
			actor.OutputHandler.Send("You must first set a target body before you can set any organs.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which organ do you want to toggle on or off for this machine?");
			return false;
		}

		var text = command.SafeRemainingArgument;
		var organ = Body.Organs.FirstOrDefault(x => x.FullDescription().EqualTo(text)) ??
		            Body.Organs.FirstOrDefault(x =>
			            x.FullDescription().StartsWith(text, StringComparison.InvariantCultureIgnoreCase)) ??
		            Body.Organs.FirstOrDefault(x => x.Name.EqualTo(text)) ??
		            Body.Organs.FirstOrDefault(
			            x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)) ??
		            Body.Organs.FirstOrDefault(x =>
			            x.FullDescription().Contains(text, StringComparison.InvariantCultureIgnoreCase))
			;
		if (organ == null)
		{
			actor.OutputHandler.Send("There is no such organ for that body.");
			return false;
		}

		if (_organs.Contains(organ))
		{
			_organs.Remove(organ);
			actor.OutputHandler.Send(
				$"This machine no longer provides the functionality of the {organ.FullDescription().Colour(Telnet.Green)} organ.");
		}
		else
		{
			_organs.Add(organ);
			actor.OutputHandler.Send(
				$"This machine now provides the functionality of the {organ.FullDescription().Colour(Telnet.Green)} organ.");
		}

		Changed = true;
		return true;
	}

	private bool BuildingCommandOxygenate(ICharacter actor, StringStack command)
	{
		OxygenatesBlood = !OxygenatesBlood;
		if (OxygenatesBlood)
		{
			actor.OutputHandler.Send("This item will now oxygenate blood.");
		}
		else
		{
			actor.OutputHandler.Send("This item will no longer oxygenate blood.");
		}

		Changed = true;
		return true;
	}

	public override bool CanSubmit()
	{
		return Body != null && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (Body == null)
		{
			return "You must first set a target body.";
		}

		return base.WhyCannotSubmit();
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(
			actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item provides external organ function for the {4} body. It {5} provide blood oxygenation. It mimics the function of {6}. It connects to a {7} venous cannula and a {13} arterial cannula. It consumes {8} watts minus {9} watts per quality.\nSwitchOnEmote: {10}\nSwitchOffEmote: {11}\nIt provides the following addendum text: \n\n{12}",
			"ExternalOrgan Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Body?.Name.Colour(Telnet.Cyan) ?? "Not Set".Colour(Telnet.Red),
			OxygenatesBlood ? "does" : "does not",
			Organs.Any()
				? Organs.Select(x => x.FullDescription().Colour(Telnet.Green)).ListToString("the ")
				: "no organs".Colour(Telnet.Red),
			$"{Gendering.Get(VenousConnection.Gender).GenderClass()} {VenousConnection.ConnectionType}".Colour(
				Telnet.Green),
			BasePowerConsumptionInWatts.ToString("N2", actor).Colour(Telnet.Green),
			PowerConsumptionDiscountPerQuality.ToString("N2", actor).Colour(Telnet.Green),
			SwitchOnEmote,
			SwitchOffEmote,
			AddendumDescription,
			$"{Gendering.Get(ArterialConnection.Gender).GenderClass()} {ArterialConnection.ConnectionType}".Colour(
				Telnet.Green)
		);
	}
}