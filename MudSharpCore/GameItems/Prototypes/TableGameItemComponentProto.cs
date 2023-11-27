using System;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class TableGameItemComponentProto : GameItemComponentProto
{
	protected TableGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected TableGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Table")
	{
		MaximumChairSlots = 8;
		CannotFlipTraitMessage = Gameworld.GetStaticString("DefaultTableCannotFlipTraitMessage");
		TraitsToFlipExpression =
			new TraitExpression(Gameworld.GetStaticConfiguration("DefaultTableTraitsToFlipExpression"), Gameworld);
		CoverWhenFlipped = Gameworld.RangedCovers.Get(Gameworld.GetStaticLong("DefaultTableCoverWhenFlipped"));
		CoverWhenNotFlipped = Gameworld.RangedCovers.Get(Gameworld.GetStaticLong("DefaultTableCoverWhenNotFlipped"));
		Changed = true;
	}

	/// <summary>
	///     The number of slots this table has for chairs to be placed around it
	/// </summary>
	public int MaximumChairSlots { get; protected set; }

	public IRangedCover CoverWhenNotFlipped { get; protected set; }

	public IRangedCover CoverWhenFlipped { get; protected set; }

	public TraitExpression TraitsToFlipExpression { get; protected set; }

	public string CannotFlipTraitMessage { get; protected set; }

	public override string TypeDescription => "Table";

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{2:N0}r{3:N0}, {4})\n\nThis item is a table and can have a maximum of {1:N0} chairs assigned to it. {5}",
			"Table Item Component".Colour(Telnet.Cyan),
			MaximumChairSlots,
			Id,
			RevisionNumber,
			Name,
			CoverWhenFlipped == null && CoverWhenNotFlipped == null
				? "It does not provide any cover when flipped or unflipped."
				: $"It provides the {CoverWhenFlipped?.Name.Colour(Telnet.Green) ?? "None"} cover when flipped and the {CoverWhenNotFlipped?.Name.Colour(Telnet.Green) ?? "None"} cover when not flipped, and requires that the expression {TraitsToFlipExpression.OriginalFormulaText.Colour(Telnet.Cyan)} is 0 or more to flip. If it cannot be flipped, it uses the error message: {CannotFlipTraitMessage.Colour(Telnet.Cyan)}"
					.Fullstop()
		);
	}

	protected override void LoadFromXml(XElement root)
	{
		var attribute = root.Attribute("MaximumChairSlots");
		if (attribute != null)
		{
			MaximumChairSlots = Convert.ToInt32(attribute.Value);
		}

		var element = root.Element("Cover");
		if (element != null)
		{
			CoverWhenFlipped = Gameworld.RangedCovers.Get(long.Parse(element.Element("Flipped").Value));
			CoverWhenNotFlipped = Gameworld.RangedCovers.Get(long.Parse(element.Element("NotFlipped").Value));
			TraitsToFlipExpression = new TraitExpression(element.Element("Expression").Value, Gameworld);
			CannotFlipTraitMessage = element.Element("Message").Value;
		}
		else
		{
			CannotFlipTraitMessage = Gameworld.GetStaticString("DefaultTableCannotFlipTraitMessage");
			TraitsToFlipExpression =
				new TraitExpression(Gameworld.GetStaticConfiguration("DefaultTableTraitsToFlipExpression"), Gameworld);
			CoverWhenFlipped = Gameworld.RangedCovers.Get(Gameworld.GetStaticLong("DefaultTableCoverWhenFlipped"));
			CoverWhenNotFlipped =
				Gameworld.RangedCovers.Get(Gameworld.GetStaticLong("DefaultTableCoverWhenNotFlipped"));
			_noSave = false;
			Changed = true;
		}
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("table", true,
			(gameworld, account) => new TableGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Table", (proto, gameworld) => new TableGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Table",
			$"Makes an item into a {"[table]".ColourCommand()}",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new TableGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new TableGameItemComponent(component, this, parent);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XAttribute("MaximumChairSlots", MaximumChairSlots),
			new XElement("Cover",
				new XElement("Flipped", CoverWhenFlipped?.Id ?? 0),
				new XElement("NotFlipped", CoverWhenNotFlipped?.Id ?? 0),
				new XElement("Expression", new XCData(TraitsToFlipExpression.OriginalFormulaText)),
				new XElement("Message", new XCData(CannotFlipTraitMessage))
			)
		).ToString();
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new TableGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tchairs <number> - how many slots this table has for chairs\n\tcover flipped|unflipped <which> - sets the ranged cover for this table ordinarily and when flipped\n\tcover flipped|unflipped none - removes cover from this bench\n\tflip \"<expression>\" <fail message> - sets a trait check to be able to flip this table - can flip if result is >0.";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
		{
			case "maximumchairslots":
			case "chairs":
			case "maxchairs":
				return BuildingCommand_MaximumChairSlots(actor, command);
			case "cover":
				return BuildingCommandCover(actor, command);
			case "flip":
				return BuildingCommandFlip(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandFlip(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.Send(
				$"The syntax for this command is {"comp set flip \"<expression>\" <error message>".Colour(Telnet.Yellow)}");
			return false;
		}

		TraitExpression expr = null;
		try
		{
			expr = new TraitExpression(input.PopSpeech(), Gameworld);
		}
		catch (Exception)
		{
			actor.Send("That is not a valid trait expression.");
			return false;
		}

		if (input.IsFinished)
		{
			SetExpression(actor, expr);
			return true;
		}

		SetExpression(actor, expr);
		CannotFlipTraitMessage = input.SafeRemainingArgument.ProperSentences().Fullstop();
		actor.Send(
			$"This table will now send the following message if the trait check fails:\n\t{CannotFlipTraitMessage.Colour(Telnet.Yellow)}");
		return true;
	}

	private bool BuildingCommandCover(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.Send(
				$"The syntax for this command is {"comp set cover flipped|unflipped <id|name>".Colour(Telnet.Yellow)}, or {"comp set cover flipped|unflipped none".Colour(Telnet.Yellow)}");
			return false;
		}

		if (input.Peek().EqualTo("none"))
		{
			CoverWhenFlipped = null;
			CoverWhenNotFlipped = null;
			Changed = true;
			actor.Send("This table will not provide any cover when flipped or unflipped.");
			return true;
		}

		var flipped = false;
		var text = input.Pop();
		switch (text.ToLowerInvariant())
		{
			case "flipped":
			case "flip":
				flipped = true;
				break;
			case "unflipped":
			case "normal":
				break;
			default:
				flipped = true;
				break;
		}

		var cover = long.TryParse(input.PopSpeech(), out var value)
			? Gameworld.RangedCovers.Get(value)
			: Gameworld.RangedCovers.GetByName(input.Last);
		if (cover == null)
		{
			actor.Send("There is no such ranged cover.");
			return false;
		}

		SetCover(actor, flipped, cover);
		return true;
	}

	private void SetExpression(ICharacter actor, TraitExpression expression)
	{
		TraitsToFlipExpression = expression;
		actor.Send(
			$"This table will now check that the expression {expression.OriginalFormulaText.Colour(Telnet.Cyan)} is 0 or more to determine if a character can flip it.");
		Changed = true;
	}

	private void SetCover(ICharacter actor, bool flipped, IRangedCover cover)
	{
		if (flipped)
		{
			CoverWhenFlipped = cover;
			actor.Send($"This table will now provide the {cover.Name.Colour(Telnet.Green)} cover when flipped over.");
		}
		else
		{
			CoverWhenNotFlipped = cover;
			actor.Send(
				$"This table will now provide the {cover.Name.Colour(Telnet.Green)} cover when not flipped (in its ordinary state).");
		}

		Changed = true;
	}

	private bool BuildingCommand_MaximumChairSlots(ICharacter actor, StringStack input)
	{
		var number = input.Pop();
		if (!int.TryParse(number, out var value))
		{
			actor.OutputHandler.Send("How many chair slots do you want this table to have?");
			return false;
		}

		if (value < 1)
		{
			actor.OutputHandler.Send("Tables must have at least one place for a chair.");
			return false;
		}

		MaximumChairSlots = value;
		actor.OutputHandler.Send("You set the maximum number of chair slots to " + number + ".");
		Changed = true;
		return true;
	}
}