using System;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class ChairGameItemComponentProto : GameItemComponentProto
{
	protected ChairGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected ChairGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Chair")
	{
		ChairOccupantCapacity = 1;
		ChairSlotsUsed = 1;
		Changed = true;
	}

	public int ChairSlotsUsed { get; protected set; }
	public int ChairOccupantCapacity { get; protected set; }
	public override string TypeDescription => "Chair";

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{3:N0}r{4:N0}, {5})\n\nThis item is a chair and uses up {1:N0} chair slots at any table. It can have {2:N0} occupants.",
			"Chair Item Component".Colour(Telnet.Cyan),
			ChairSlotsUsed,
			ChairOccupantCapacity,
			Id,
			RevisionNumber,
			Name
		);
	}

	protected override void LoadFromXml(XElement root)
	{
		var attribute = root.Attribute("ChairSlotsUsed");
		if (attribute != null)
		{
			ChairSlotsUsed = Convert.ToInt32(attribute.Value);
		}

		attribute = root.Attribute("ChairOccupantCapacity");
		if (attribute != null)
		{
			ChairOccupantCapacity = Convert.ToInt32(attribute.Value);
		}
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", new XAttribute("ChairSlotsUsed", ChairSlotsUsed),
				new XAttribute("ChairOccupantCapacity", ChairOccupantCapacity)).ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("chair", true,
			(gameworld, account) => new ChairGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Chair", (proto, gameworld) => new ChairGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Chair",
			$"Makes the item a {"[chair]".Colour(Telnet.Yellow)} that can be attached to a {"[table]".Colour(Telnet.Yellow)}, or stand free.",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ChairGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ChairGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new ChairGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tslots <#> - how many chair slots this chair takes up\n\toccupants <#> - how many people can simultaneously use this chair";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "chairslots":
			case "chair slots":
			case "slots":
				return BuildingCommand_ChairSlots(actor, command);
			case "capacity":
			case "occupants":
				return BuildingCommand_Occupants(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommand_ChairSlots(ICharacter actor, StringStack command)
	{
		var number = command.Pop();
		if (!int.TryParse(number, out var value))
		{
			actor.OutputHandler.Send("How many chair slots do you want this chair to use up?");
			return false;
		}

		if (value < 1)
		{
			actor.OutputHandler.Send("Chair slots must take up at least one space.");
			return false;
		}

		ChairSlotsUsed = value;
		actor.OutputHandler.Send("You set this chair's chair slot usage to " + number + ".");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Occupants(ICharacter actor, StringStack command)
	{
		var number = command.Pop();
		if (!int.TryParse(number, out var value))
		{
			actor.OutputHandler.Send("How many places for occupants do you want this chair to have?");
			return false;
		}

		if (value < 1)
		{
			actor.OutputHandler.Send("Chairs must have space for at least one occupant.");
			return false;
		}

		ChairOccupantCapacity = value;
		actor.OutputHandler.Send("You set this chair's occupant capacity to " + number + ".");
		Changed = true;
		return true;
	}
}