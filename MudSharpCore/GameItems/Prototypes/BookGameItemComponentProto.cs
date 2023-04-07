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
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class BookGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Book";

	private long _paperProtoId;
	public IGameItemProto PaperProto => Gameworld.ItemProtos.Get(_paperProtoId);

	public int PageCount { get; protected set; }

	public int MaximumCharacterLengthOfText =>
		PaperProto?.GetItemType<PaperSheetGameItemComponentProto>()?.MaximumCharacterLengthOfText ?? 0;

	public override bool WarnBeforePurge => true;

	#region Constructors

	protected BookGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Book")
	{
		PageCount = Gameworld.GetStaticInt("DefaultBookPageCount");
		_paperProtoId = Gameworld.GetStaticLong("DefaultBookPaperProto");
	}

	protected BookGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		_paperProtoId = long.Parse(root.Element("PaperProto")?.Value ?? "0");
		PageCount = int.Parse(root.Element("PageCount")?.Value ?? "0");
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new[]
		{
			new XElement("PaperProto", PaperProto?.Id ?? 0),
			new XElement("PageCount", PageCount)
		}).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new BookGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new BookGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Book".ToLowerInvariant(), true,
			(gameworld, account) => new BookGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Book", (proto, gameworld) => new BookGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Book",
			$"Makes an item into a {"[book]".Colour(Telnet.BoldWhite)}",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new BookGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tpaper <id|name> - sets the item prototype for pages in this book\n\tpages <number> - sets the number of pages in a fresh copy of this book";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "paper":
			case "proto":
			case "paperproto":
			case "paper proto":
			case "paper_proto":
				return BuildingCommandPaperProto(actor, command);
			case "pages":
			case "count":
			case "page":
			case "page count":
			case "pagecount":
			case "page_count":
				return BuildingCommandPageCount(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandPageCount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many pages should a fresh copy of this book start with?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.Send("You must enter a valid number of pages greater than zero for this book.");
			return false;
		}

		PageCount = value;
		Changed = true;
		actor.Send($"This book will now load up with {PageCount:N0} pages.");
		return true;
	}

	private bool BuildingCommandPaperProto(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"You must specify the id or name of a paper prototype to use when pages are torn out of this book.");
			return false;
		}

		var proto = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.ItemProtos.Get(value)
			: Gameworld.ItemProtos.GetByName(command.Last);
		if (proto == null)
		{
			actor.Send("There is no such prototype.");
			return false;
		}

		if (!proto.IsItemType<PaperSheetGameItemComponentProto>())
		{
			actor.Send(
				$"The prototype {proto.Name.Colour(Telnet.Cyan)} (#{proto.Id}r{proto.RevisionNumber}) is not a paper sheet prototype.");
			return false;
		}

		if (proto.Status != RevisionStatus.Current)
		{
			actor.Send(
				$"The prototype {proto.Name.Colour(Telnet.Cyan)} (#{proto.Id}r{proto.RevisionNumber}) is not a currently approved paper sheet prototype.");
			return false;
		}

		_paperProtoId = proto.Id;
		Changed = true;
		actor.Send(
			$"This book will now use the prototype {proto.Name.Colour(Telnet.Cyan)} (#{proto.Id}r{proto.RevisionNumber}) for its torn pages.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a book with {4} pages, that references the {5} prototype for its paper.",
			"Book Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			PageCount,
			PaperProto != null
				? $"{PaperProto.Name.Colour(Telnet.Cyan)} (#{PaperProto.Id})"
				: "Not Set".Colour(Telnet.Red)
		);
	}
}