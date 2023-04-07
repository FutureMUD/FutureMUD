using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class VendingMachineGameItemComponentProto : GameItemComponentProto
{
	protected VendingMachineGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Vending Machine")
	{
		MaximumContentsSize = SizeCategory.Normal;
		WeightLimit = 10.0 / gameworld.UnitManager.BaseWeightToKilograms;
		InsertMoneyEmote = "$0 insert|inserts $1 into $2";
		RefundMoneyEmote = "$0 select|selects the refund option on $1, and $2 is deposited into the delivery pan.";
		ItemSelectedEmote = "$0 select|selects the {0} button on $1, and $2 is deposited into the delivery pan";
		InvalidItemSelectedEmote =
			"$0 select|selects the {0} button on $1, but nothing is dispensed. The balance screen flashes in alert.";
	}

	protected VendingMachineGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public string InsertMoneyEmote { get; set; }
	public string RefundMoneyEmote { get; set; }
	public string ItemSelectedEmote { get; set; }
	public string InvalidItemSelectedEmote { get; set; }
	public SizeCategory MaximumContentsSize { get; set; }
	public double WeightLimit { get; set; }
	public override string TypeDescription => "Vending Machine";

	protected override void LoadFromXml(XElement root)
	{
		var element = root.Element("InsertMoneyEmote");
		if (element != null)
		{
			InsertMoneyEmote = element.Value;
		}

		element = root.Element("RefundMoneyEmote");
		if (element != null)
		{
			RefundMoneyEmote = element.Value;
		}

		element = root.Element("ItemSelectedEmote");
		if (element != null)
		{
			ItemSelectedEmote = element.Value;
		}

		element = root.Element("InvalidItemSelectedEmote");
		if (element != null)
		{
			InvalidItemSelectedEmote = element.Value;
		}

		var attr = root.Attribute("Weight");
		if (attr != null)
		{
			WeightLimit = double.Parse(attr.Value);
		}

		attr = root.Attribute("MaxSize");
		if (attr != null)
		{
			MaximumContentsSize = (SizeCategory)int.Parse(attr.Value);
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a vending machine, and can be configured to dispense items. As a container, it takes items of up to {4} size and {5} weight of items.\nIt has the following emotes:\n\nInserting Money: {6}\nRefunding Money: {7}\nItem Selected: {8}\nCan't Afford Item: {9}",
			"Vending Machine Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			MaximumContentsSize.Describe().Colour(Telnet.Green),
			Gameworld.UnitManager.Describe(WeightLimit, UnitType.Mass, actor).Colour(Telnet.Green),
			InsertMoneyEmote.Colour(Telnet.Yellow),
			RefundMoneyEmote.Colour(Telnet.Yellow),
			ItemSelectedEmote.Colour(Telnet.Yellow),
			InvalidItemSelectedEmote.Colour(Telnet.Yellow)
		);
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", new XAttribute("Weight", WeightLimit),
				new XAttribute("MaxSize", (int)MaximumContentsSize),
				new XElement("InsertMoneyEmote", new XCData(InsertMoneyEmote)),
				new XElement("RefundMoneyEmote", new XCData(RefundMoneyEmote)),
				new XElement("ItemSelectedEmote", new XCData(ItemSelectedEmote)),
				new XElement("InvalidItemSelectedEmote", new XCData(InvalidItemSelectedEmote))
			).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("vending machine", true,
			(gameworld, account) => new VendingMachineGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("vending", false,
			(gameworld, account) => new VendingMachineGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("vendingmachine", false,
			(gameworld, account) => new VendingMachineGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Vending Machine",
			(proto, gameworld) => new VendingMachineGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"VendingMachine",
			$"An {"[container]".Colour(Telnet.BoldGreen)} item that allows players to {"[insert]".Colour(Telnet.Yellow)} money and {"[select]".Colour(Telnet.Yellow)} options to vend",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new VendingMachineGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new VendingMachineGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new VendingMachineGameItemComponentProto(proto, gameworld));
	}

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tcapacity <weight> - the capacity of the vending machines dispensing part\n\tsize <size> - the maximum size of the vending machines dispensing part\n\tinsert <emote> - the emote for inserting money. $0 for the person, $1 for the money, $2 for the vending machine\n\trefund <emote> - the emote when refund selected. $0 for the person, $1 for the vending machine, $2 for the money returned\n\tselected <emote> - the emote when making a selection. $0 for the person, $1 for the vending machine, $2 for the item dispensed\n\tinvalid <emote> - the emote when an invalid selection is made. $0 for the person, $1 for the vending machine";

	public override string ShowBuildingHelp => BuildingHelpText;

	private bool BuildingCommand_WeightLimit(ICharacter actor, StringStack command)
	{
		var weightCmd = command.SafeRemainingArgument;
		var result = actor.Gameworld.UnitManager.GetBaseUnits(weightCmd, UnitType.Mass, out var success);
		if (success)
		{
			WeightLimit = result;
			actor.OutputHandler.Send(
				$"This vending machine's delivery pan will now hold {actor.Gameworld.UnitManager.Describe(WeightLimit, UnitType.Mass, actor).ColourValue()}.");
			return true;
		}

		actor.OutputHandler.Send("That is not a valid weight.");
		return false;
	}

	private bool BuildingCommand_MaxSize(ICharacter actor, StringStack command)
	{
		var cmd = command.PopSpeech().ToLowerInvariant();
		if (cmd.Length == 0)
		{
			actor.OutputHandler.Send("What size do you want to set the limit for this component to?");
			return false;
		}

		var size = Enum.GetValues(typeof(SizeCategory)).OfType<SizeCategory>().ToList();
		SizeCategory target;
		if (size.Any(x => x.Describe().ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal)))
		{
			target = size.FirstOrDefault(x =>
				x.Describe().ToLowerInvariant().StartsWith(cmd, StringComparison.Ordinal));
		}
		else
		{
			actor.OutputHandler.Send("That is not a valid item size. See SHOW ITEMSIZES for a correct list.");
			return false;
		}

		MaximumContentsSize = target;
		Changed = true;
		actor.OutputHandler.Send("This vending machine's delivery pan will now only take items of up to size \"" +
		                         target.Describe() + "\".");
		return true;
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "insert":
			case "refund":
			case "selected":
			case "invalid":
				return BuildingCommand_Emotes(actor, command, command.Last.ToLowerInvariant());
			case "capacity":
			case "weight":
			case "weight limit":
			case "weight capacity":
			case "limit":
				return BuildingCommand_WeightLimit(actor, command);
			case "maximum size":
			case "max size":
			case "maxsize":
			case "size":
				return BuildingCommand_MaxSize(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommand_Emotes(ICharacter actor, StringStack command, string whichEmote)
	{
		string emoteDescription;
		switch (whichEmote)
		{
			case "insert":
				emoteDescription = "Coin Insertion Emote";
				break;
			case "refund":
				emoteDescription = "Refund Emote";
				break;
			case "selected":
				emoteDescription = "Item Selection Emote";
				break;
			case "invalid":
				emoteDescription = "Invalid Selection Emote";
				break;
			default:
				throw new ApplicationException("Invalid VendingMachine emote type " + whichEmote);
		}

		if (command.IsFinished)
		{
			actor.Send("What do you want to set this item's {0} to?", emoteDescription.ColourName());
			return false;
		}

		var newEmote = command.RemainingArgument.Trim().NormaliseSpacing();
		switch (whichEmote)
		{
			case "insert":
				InsertMoneyEmote = newEmote;
				break;
			case "refund":
				RefundMoneyEmote = newEmote;
				break;
			case "selected":
				ItemSelectedEmote = newEmote;
				break;
			case "invalid":
				InvalidItemSelectedEmote = newEmote;
				break;
		}

		Changed = true;
		actor.Send("You set the {0} for this vending machine to {1}", emoteDescription.ColourName(),
			newEmote.ColourCommand());
		return true;
	}

	#endregion
}