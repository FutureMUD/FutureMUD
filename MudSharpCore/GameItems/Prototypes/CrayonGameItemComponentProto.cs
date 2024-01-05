using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Colour;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class CrayonGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Crayon";

	public IColour Colour { get; set; }
	public ICharacteristicDefinition ColourCharacteristic { get; set; }
	public int TotalUses { get; set; }

	#region Constructors

	protected CrayonGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Crayon")
	{
		Colour = Gameworld.Colours.Get(Gameworld.GetStaticLong("DefaultCrayonColour"));
		TotalUses = 5000;
	}

	protected CrayonGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		Colour = Gameworld.Colours.Get(long.Parse(root.Element("Colour")?.Value ?? "0"));
		ColourCharacteristic =
			Gameworld.Characteristics.Get(long.Parse(root.Element("ColourCharacteristic")?.Value ?? "0"));
		TotalUses = int.Parse(root.Element("TotalUses").Value);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Colour", Colour?.Id ?? 0),
			new XElement("TotalUses", TotalUses),
			new XElement("ColourCharacteristic", ColourCharacteristic?.Id ?? 0)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new CrayonGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new CrayonGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Crayon".ToLowerInvariant(), true,
			(gameworld, account) => new CrayonGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Crayon", (proto, gameworld) => new CrayonGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Crayon",
			$"Turns an item {"[writing implement]".Colour(Telnet.Yellow)} of type crayon",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new CrayonGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText = @"You can use the following options with this component:
	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
	colour <colour> - sets the colour of the crayon's writing
	variable <which> - sets a variable which controls the colour instead of the previous option
	uses <amount> - the approximate number of letters the crayon can write before being empty";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "colour":
			case "color":
				return BuildingCommandColour(actor, command);
			case "variable":
			case "characteristic":
			case "definition":
			case "characteristicdefinition":
				return BuildingCommandVariable(actor, command);
			case "uses":
			case "total":
			case "use":
			case "total uses":
			case "totaluses":
				return BuildingCommandUses(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandVariable(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which characteristic definition do you want to control the colour of this item's writing?");
			return false;
		}

		var definition = Gameworld.Characteristics.GetByIdOrName(command.SafeRemainingArgument);
		if (definition == null)
		{
			actor.OutputHandler.Send("There is no such characteristic definition.");
			return false;
		}

		if (definition.Type != CharacteristicType.Coloured)
		{
			actor.OutputHandler.Send(
				$"The {definition.Name.ColourName()} characteristic definition is not a coloured characteristic.");
			return false;
		}

		ColourCharacteristic = definition;
		Colour = null;
		Changed = true;
		actor.OutputHandler.Send(
			$"This item will now inherit the colour of the {definition.Name.ColourName()} variable on its parent rather than a hard-coded colour.");
		return true;
	}

	private bool BuildingCommandColour(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What colour do you want to set for this crayon?");
			return false;
		}

		var colour = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Colours.Get(value)
			: Gameworld.Colours.GetByName(command.SafeRemainingArgument);
		if (colour == null)
		{
			actor.Send("That is not a valid colour.");
			return false;
		}

		Colour = colour;
		ColourCharacteristic = null;
		Changed = true;
		actor.Send($"This crayon will now write in the colour {Colour.Name.Colour(Telnet.Cyan)}");
		return true;
	}

	private bool BuildingCommandUses(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"How many uses should this crayon have before it is expended? Hint: Every 80 characters or so is one use.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.Send("You must enter a valid positive number of uses for this crayon.");
			return false;
		}

		TotalUses = value;
		Changed = true;
		actor.Send($"This crayon will now have {TotalUses:N0} uses, or approximately {TotalUses * 80:N0} characters.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a {4} coloured crayon. It has {5:N0} total uses.",
			"Crayon Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			(Colour?.Name ?? $"{ColourCharacteristic?.Pattern.ToString()}").ColourName(),
			TotalUses
		);
	}
}