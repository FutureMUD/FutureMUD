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

public class BenchGameItemComponentProto : GameItemComponentProto
{
	private IGameItemProto _chairProto;
	private long _chairProtoID;

	protected BenchGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected BenchGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Bench")
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

	public IGameItemProto ChairProto
	{
		get
		{
			if (_chairProtoID > 0 && _chairProto == null)
			{
				_chairProto = Gameworld.ItemProtos.Get(_chairProtoID);
			}

			return _chairProto;
		}
	}

	public int ChairCount { get; set; }
	public override string TypeDescription => "Bench";

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{4:N0}r{5:N0}, {6})\n\nThis item is a bench, with {2:N0} permanently affixed chairs (proto {3}). It can have a maximum of {1:N0} chairs assigned to it. {7}",
			"Bench Item Component".Colour(Telnet.Cyan),
			MaximumChairSlots,
			ChairCount,
			ChairProto != null
				? string.Format(actor, "{0} #{1:N0}".FluentTagMXP("send", "href='item show {1}'"), ChairProto.Name,
					ChairProto.Id)
				: "not selected",
			Id,
			RevisionNumber,
			Name,
			CoverWhenFlipped == null && CoverWhenNotFlipped == null
				? "It does not provide any cover when flipped or unflipped."
				: $"It provides the {CoverWhenFlipped?.Name.Colour(Telnet.Green) ?? "None"} cover when flipped and the {CoverWhenNotFlipped?.Name.Colour(Telnet.Green) ?? "None"} cover when not flipped, and requires that the expression {TraitsToFlipExpression.Formula.OriginalExpression.Colour(Telnet.Cyan)} is 0 or more to flip. If it cannot be flipped, it uses the error message: {CannotFlipTraitMessage.Colour(Telnet.Cyan)}"
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

		attribute = root.Attribute("Chair");
		_chairProtoID = attribute != null ? long.Parse(attribute.Value) : 0;

		attribute = root.Attribute("ChairCount");
		ChairCount = attribute != null ? int.Parse(attribute.Value) : 0;

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
		manager.AddBuilderLoader("bench", true,
			(gameworld, account) => new BenchGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Bench", (proto, gameworld) => new BenchGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Bench",
			$"A bench is a {"[table]".ColourCommand()} that comes with built-in chairs",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new BenchGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new BenchGameItemComponent(component, this, parent);
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", new XAttribute("MaximumChairSlots", MaximumChairSlots),
				new XAttribute("Chair", ChairProto?.Id ?? 0),
				new XAttribute("ChairCount", ChairCount),
				new XElement("Cover",
					new XElement("Flipped", CoverWhenFlipped?.Id ?? 0),
					new XElement("NotFlipped", CoverWhenNotFlipped?.Id ?? 0),
					new XElement("Expression", new XCData(TraitsToFlipExpression.Formula.OriginalExpression)),
					new XElement("Message", new XCData(CannotFlipTraitMessage))
				)).ToString();
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new BenchGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tchairs <number> - how many slots this table has for chairs\n\tproto <vnum> - the prototype of the chair item that the bench automatically loads\n\tcover flipped|unflipped <which> - sets the ranged cover for this table ordinarily and when flipped\n\tcover flipped|unflipped none - removes cover from this bench\n\tflip \"<expression>\" <fail message> - sets a trait check to be able to flip this table - can flip if result is >0.";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
		{
			case "maximumchairslots":
			case "chairs":
			case "maxchairs":
				return BuildingCommand_MaximumChairSlots(actor, command);
			case "chair":
			case "chair proto":
			case "chairproto":
			case "proto":
				return BuildingCommand_ChairProto(actor, command);
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
			$"This bench will now send the following message if the trait check fails:\n\t{CannotFlipTraitMessage.Colour(Telnet.Yellow)}");
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
			actor.Send("This bench will not provide any cover when flipped or unflipped.");
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
			$"This bench will now check that the expression {expression.Formula.OriginalExpression.Colour(Telnet.Cyan)} is 0 or more to determine if a character can flip it.");
		Changed = true;
	}

	private void SetCover(ICharacter actor, bool flipped, IRangedCover cover)
	{
		if (flipped)
		{
			CoverWhenFlipped = cover;
			actor.Send($"This bench will now provide the {cover.Name.Colour(Telnet.Green)} cover when flipped over.");
		}
		else
		{
			CoverWhenNotFlipped = cover;
			actor.Send(
				$"This bench will now provide the {cover.Name.Colour(Telnet.Green)} cover when not flipped (in its ordinary state).");
		}

		Changed = true;
	}

	private bool BuildingCommand_ChairProto(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which chair prototype do you want to use for this bench?");
			return false;
		}

		var proto = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.ItemProtos.Get(value)
			: Gameworld.ItemProtos.GetByName(command.Last);
		if (proto == null)
		{
			actor.Send("There is no such component prototype.");
			return false;
		}

		if (!proto.IsItemType<ChairGameItemComponentProto>())
		{
			actor.Send("That component prototype is not a chair.");
			return false;
		}

		var chairCount = 1;
		if (!command.IsFinished)
		{
			if (!int.TryParse(command.Pop(), out chairCount))
			{
				actor.Send("How many of these chairs do you want to be permanently attached to the bench?");
				return false;
			}

			if (chairCount < 1)
			{
				actor.Send("There must be at least one permanently fixed chair with a bench.");
				return false;
			}

			if (chairCount > 20)
			{
				actor.Send(
					"If you legitimately have a bench that should have more than 20 chairs, please contact Japheth@Futuremud.com to make your case.");
				return false;
			}
		}

		_chairProto = proto;
		ChairCount = chairCount;
		MaximumChairSlots = Math.Max(ChairCount, MaximumChairSlots);
		Changed = true;
		actor.Send("This bench will now have {0:N0} chairs of prototype {1} (#{2:N0}) permanently attached.",
			ChairCount, ChairProto.Name, ChairProto.Id);
		return true;
	}

	private bool BuildingCommand_MaximumChairSlots(ICharacter actor, StringStack input)
	{
		var number = input.Pop();
		if (!int.TryParse(number, out var value))
		{
			actor.OutputHandler.Send("How many chair slots do you want this bench to have?");
			return false;
		}

		if (value < 1)
		{
			actor.OutputHandler.Send("Benches must have at least one place for a chair.");
			return false;
		}

		if (value < ChairCount)
		{
			actor.OutputHandler.Send("Benches must have at least as many chair slots as they have built in chairs.");
			return false;
		}

		MaximumChairSlots = value;
		actor.OutputHandler.Send("You set the maximum number of chair slots to " + number + ".");
		Changed = true;
		return true;
	}
}