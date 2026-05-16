using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Colour;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using System;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class ScribingImplementGameItemComponentProto : GameItemComponentProto, IWritingImplementPrototype
{
	public override string TypeDescription => "ScribingImplement";

	public WritingImplementType ImplementType { get; set; }
	public IColour? Colour { get; set; }
	public ICharacteristicDefinition? ColourCharacteristic { get; set; }
	public int TotalUses { get; set; }

	#region Constructors

	protected ScribingImplementGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "ScribingImplement")
	{
		ImplementType = WritingImplementType.Quill;
		Colour = Gameworld.Colours.Get(Gameworld.GetStaticLong("DefaultWritingColourInText")) ??
		         Gameworld.Colours.Get(Gameworld.GetStaticLong("DefaultBiroColour"));
		TotalUses = 5000;
	}

	protected ScribingImplementGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		ImplementType = ParseImplementType(root.Element("ImplementType")?.Value ?? root.Element("Type")?.Value);
		var colourId = long.Parse(root.Element("Colour")?.Value ?? "0");
		Colour = colourId > 0 ? Gameworld.Colours.Get(colourId) : null;
		var colourCharacteristicId = long.Parse(root.Element("ColourCharacteristic")?.Value ?? "0");
		ColourCharacteristic = colourCharacteristicId > 0
			? Gameworld.Characteristics.Get(colourCharacteristicId)
			: null;
		TotalUses = int.Parse(root.Element("TotalUses")?.Value ?? "0");
	}

	private static WritingImplementType ParseImplementType(string? text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return WritingImplementType.Quill;
		}

		return int.TryParse(text, out var value)
			? (WritingImplementType)value
			: Enum.Parse<WritingImplementType>(text, true);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("ImplementType", ImplementType.ToString()),
			new XElement("Colour", Colour?.Id ?? 0),
			new XElement("ColourCharacteristic", ColourCharacteristic?.Id ?? 0),
			new XElement("TotalUses", TotalUses)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new ScribingImplementGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ScribingImplementGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ScribingImplement".ToLowerInvariant(), true,
			(gameworld, account) => new ScribingImplementGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("Scribing Implement".ToLowerInvariant(), false,
			(gameworld, account) => new ScribingImplementGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ScribingImplement",
			(proto, gameworld) => new ScribingImplementGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ScribingImplement",
			$"Turns an item into a configurable {"[writing implement]".Colour(Telnet.Yellow)} such as a quill, stylus, brush, chisel or charcoal stick",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ScribingImplementGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText = @"You can use the following options with this component:
	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
	type <type> - sets the WritingImplementType value
	colour <colour> - sets the colour of the writing
	variable <which> - sets a variable which controls the colour instead of the previous option
	uses <amount> - the approximate number of characters before the implement is spent; 0 means it is not consumed";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "type":
			case "implement":
			case "implementtype":
				return BuildingCommandType(actor, command);
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

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which writing implement type should this item use?");
			return false;
		}

		if (!Enum.TryParse<WritingImplementType>(command.SafeRemainingArgument, true, out var value))
		{
			actor.Send("That is not a valid writing implement type.");
			return false;
		}

		ImplementType = value;
		Changed = true;
		actor.Send($"This item is now a {ImplementType.Describe().ColourName()} writing implement.");
		return true;
	}

	private bool BuildingCommandColour(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What colour do you want this writing implement to make?");
			return false;
		}

		var colour = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Colours.Get(value)
			: Gameworld.Colours.GetByName(command.SafeRemainingArgument);
		if (colour is null)
		{
			actor.Send("That is not a valid colour.");
			return false;
		}

		Colour = colour;
		ColourCharacteristic = null;
		Changed = true;
		actor.Send($"This writing implement will now write in {Colour.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandVariable(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which characteristic definition should control this item's writing colour?");
			return false;
		}

		var definition = Gameworld.Characteristics.GetByIdOrName(command.SafeRemainingArgument);
		if (definition is null)
		{
			actor.Send("There is no such characteristic definition.");
			return false;
		}

		if (definition.Type != CharacteristicType.Coloured)
		{
			actor.Send($"The {definition.Name.ColourName()} characteristic definition is not a coloured characteristic.");
			return false;
		}

		ColourCharacteristic = definition;
		Colour = null;
		Changed = true;
		actor.Send(
			$"This item will now inherit the colour of the {definition.Name.ColourName()} variable on its parent.");
		return true;
	}

	private bool BuildingCommandUses(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many characters should this implement write before it is spent? Use 0 for a non-consuming tool.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.Send("You must enter a valid number of uses greater than or equal to zero.");
			return false;
		}

		TotalUses = value;
		Changed = true;
		actor.Send(value == 0
			? "This writing implement will not be consumed by writing."
			: $"This writing implement will now have {TotalUses:N0} characters of use.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		var colourText = Colour?.Name ?? $"{ColourCharacteristic?.Pattern}";
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a {4} writing implement that writes in {5}. It {6}.",
			"ScribingImplement Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			ImplementType.Describe().ColourName(),
			colourText.ColourName(),
			TotalUses <= 0
				? "is not consumed by writing"
				: $"has {TotalUses:N0} characters of writing before it is spent"
		);
	}
}
