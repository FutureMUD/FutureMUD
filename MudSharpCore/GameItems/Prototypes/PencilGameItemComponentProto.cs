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

public class PencilGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Pencil";

	public IColour Colour { get; set; }
	public ICharacteristicDefinition ColourCharacteristic { get; set; }
	public int UsesBeforeSharpening { get; set; }
	public int TotalUses { get; set; }

	#region Constructors

	protected PencilGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Pencil")
	{
		Colour = Gameworld.Colours.Get(Gameworld.GetStaticLong("DefaultPencilColour"));
		TotalUses = 5000;
		UsesBeforeSharpening = 100;
	}

	protected PencilGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		Colour = Gameworld.Colours.Get(long.Parse(root.Element("Colour")?.Value ?? "0"));
		ColourCharacteristic =
			Gameworld.Characteristics.Get(long.Parse(root.Element("ColourCharacteristic")?.Value ?? "0"));
		UsesBeforeSharpening = int.Parse(root.Element("UsesBeforeSharpening").Value);
		TotalUses = int.Parse(root.Element("TotalUses").Value);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Colour", Colour?.Id ?? 0),
			new XElement("TotalUses", TotalUses),
			new XElement("UsesBeforeSharpening", UsesBeforeSharpening),
			new XElement("ColourCharacteristic", ColourCharacteristic?.Id ?? 0)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new PencilGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new PencilGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Pencil".ToLowerInvariant(), true,
			(gameworld, account) => new PencilGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Pencil", (proto, gameworld) => new PencilGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Pencil",
			$"Turns an item {"[writing implement]".Colour(Telnet.Yellow)} of type pencil. Pencils require sharpening with pencil sharpeners",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new PencilGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText = @"You can use the following options with this component:
	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
	colour <colour> - sets the colour of the pencil's writing
	variable <which> - sets a variable which controls the colour instead of the previous option
	uses <amount> - the approximate number of letters the pencil can write before being empty
	sharpen <uses> - the number of uses before the pencil requires sharpening";

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
			case "sharp":
			case "sharpen":
			case "sharpening":
				return BuildingCommandSharpen(actor, command);
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
			actor.Send("What colour do you want to set for this pencil?");
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
		actor.Send($"This pencil will now write in the colour {Colour.Name.Colour(Telnet.Cyan)}");
		return true;
	}

	private bool BuildingCommandUses(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"How many uses should this pencil have before it is expended? Hint: Every 80 characters or so is one use.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.Send("You must enter a valid positive number of uses for this pencil.");
			return false;
		}

		TotalUses = value;
		Changed = true;
		actor.Send($"This pencil will now have {TotalUses:N0} uses, or approximately {TotalUses * 80:N0} characters.");
		return true;
	}

	private bool BuildingCommandSharpen(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many uses should this pencil have before it needs to be sharpened?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.Send("You must enter a valid positive number of uses for this pencil.");
			return false;
		}

		UsesBeforeSharpening = value;
		Changed = true;
		actor.Send(
			$"This pencil will now have {UsesBeforeSharpening:N0} uses before it needs sharpening, or approximately {UsesBeforeSharpening * 80:N0} characters.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a {4} coloured pencil. It has {5:N0} total uses, and needs sharpening every {6:N0} uses.",
			"Pencil Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			(Colour?.Name ?? $"{ColourCharacteristic?.Pattern.ToString()}").ColourName(),
			TotalUses,
			UsesBeforeSharpening
		);
	}
}