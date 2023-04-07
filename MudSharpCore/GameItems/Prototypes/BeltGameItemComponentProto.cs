using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class BeltGameItemComponentProto : GameItemComponentProto
{
	public SizeCategory MaximumSize { get; protected set; }
	public int MaximumNumberOfBeltedItems { get; protected set; }
	public override string TypeDescription => "Belt";

	protected override void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("MaximumNumberOfBeltedItems");
		if (attr != null)
		{
			MaximumNumberOfBeltedItems = int.Parse(attr.Value);
		}

		attr = root.Attribute("MaximumSize");
		if (attr != null)
		{
			MaximumSize = (SizeCategory)int.Parse(attr.Value);
		}
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("belt", true,
			(gameworld, account) => new BeltGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Belt", (proto, gameworld) => new BeltGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Belt",
			$"Turns this item into a {"[belt]".Colour(Telnet.Yellow)} that can have things attached to it",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new BeltGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new BeltGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new BeltGameItemComponentProto(proto, gameworld));
	}

	protected override string SaveToXml()
	{
		return "<Definition MaximumNumberOfBeltedItems=\"" + MaximumNumberOfBeltedItems + "\" MaximumSize=\"" +
		       (int)MaximumSize + "\"/>";
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{3:N0}r{4:N0}, {5})\n\nThis item can have beltable items of up to size {1} and has {2:N0} slots for attachments.",
			"Belt Item Component".Colour(Telnet.Cyan),
			MaximumSize.Describe().Colour(Telnet.Cyan),
			MaximumNumberOfBeltedItems,
			Id,
			RevisionNumber,
			Name
		);
	}

	private bool BuildingCommandMaximumNumberOfBeltedItems(ICharacter character, StringStack command)
	{
		if (!int.TryParse(command.Pop(), out var value))
		{
			character.OutputHandler.Send("You must enter a valid number of items for the capacity of this belt.");
			return false;
		}

		if (value < 1)
		{
			character.OutputHandler.Send("Belts must have a capacity of at least one item.");
			return false;
		}

		MaximumNumberOfBeltedItems = value;
		Changed = true;
		character.OutputHandler.Send("This belt can now have a total of " + MaximumNumberOfBeltedItems +
		                             " items attached to it at once.");
		return true;
	}

	private bool BuildingCommandMaximumSize(ICharacter character, StringStack command)
	{
		var cmd = command.PopSpeech().ToLowerInvariant();
		if (cmd.Length == 0)
		{
			character.OutputHandler.Send("What size do you want to set the limit for this component to?");
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
			character.OutputHandler.Send("That is not a valid item size. See SHOW ITEMSIZES for a correct list.");
			return false;
		}

		MaximumSize = target;
		Changed = true;
		character.OutputHandler.Send("This belt will now only allow attachments of up to size \"" +
		                             target.Describe() + "\".");
		return true;
	}

	public override bool BuildingCommand(ICharacter character, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "maximum size":
			case "max size":
			case "maxsize":
			case "size":
				return BuildingCommandMaximumSize(character, command);
			case "capacity":
			case "slots":
			case "slot":
				return BuildingCommandMaximumNumberOfBeltedItems(character, command);
			default:
				return base.BuildingCommand(character, command);
		}
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tsize <size> - sets the maximum size of items that can be attached\n\tcapacity <number> - sets the number of slots for items";

	public override string ShowBuildingHelp => BuildingHelpText;

	#region Constructors

	protected BeltGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected BeltGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Belt")
	{
		MaximumNumberOfBeltedItems = 1;
		MaximumSize = SizeCategory.Small;
		Changed = true;
	}

	#endregion
}