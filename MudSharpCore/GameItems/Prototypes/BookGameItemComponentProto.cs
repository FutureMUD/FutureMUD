using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

#nullable enable annotations

namespace MudSharp.GameItems.Prototypes;

public class BookGameItemComponentProto : GameItemComponentProto, IWriteablePrototype, IReadablePrototype, ITurnablePrototype, IOpenablePrototype, ITearablePrototype
{
	public override string TypeDescription => "Book";

	private long _paperProtoId;
	public IGameItemProto PaperProto => Gameworld.ItemProtos.Get(_paperProtoId);

	public int PageCount { get; protected set; }
	public string DefaultTitle { get; protected set; } = string.Empty;

	private readonly List<BookPageContentTemplate> _initialReadables = new();
	private readonly List<BookCollectionContentTemplate> _initialCollections = new();
	internal IEnumerable<BookPageContentTemplate> InitialReadables => _initialReadables.OrderBy(x => x.Page).ThenBy(x => x.Order);
	internal IEnumerable<BookCollectionContentTemplate> InitialCollections => _initialCollections.OrderBy(x => x.StartPage).ThenBy(x => x.CollectionId);

	internal IEnumerable<(int Page, ICanBeRead Readable)> InitialReadableReferences
	{
		get
		{
			foreach (var item in InitialReadables)
			{
				if (item.Readable is not null)
				{
					yield return (item.Page, item.Readable);
				}
			}

			foreach (var item in InitialCollections)
			{
				var collection = item.Collection;
				if (collection is null || !collection.Entries.Any())
				{
					continue;
				}

				var firstPage = collection.Entries.Min(x => x.Page);
				foreach (var entry in collection.Entries.OrderBy(x => x.Page).ThenBy(x => x.Order))
				{
					yield return (item.StartPage + entry.Page - firstPage, entry.Readable);
				}
			}
		}
	}

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
		DefaultTitle = root.Element("DefaultTitle")?.Value ?? string.Empty;
		_initialReadables.Clear();
		_initialCollections.Clear();
		var convertedLegacy = false;
		foreach (var item in root.Element("InitialReadables")?.Elements() ?? Enumerable.Empty<XElement>())
		{
			if (!item.Name.LocalName.EqualToAny("Writing", "Drawing"))
			{
				continue;
			}

			var template = new BookPageContentTemplate(Gameworld, item);
			if (template.Readable is null)
			{
				continue;
			}

			convertedLegacy = convertedLegacy || template.WasLoadedFromLegacyXml;
			_initialReadables.Add(template);
		}

		foreach (var item in root.Element("InitialCollections")?.Elements("Collection") ?? Enumerable.Empty<XElement>())
		{
			_initialCollections.Add(new BookCollectionContentTemplate(Gameworld, item));
		}

		if (convertedLegacy)
		{
			Changed = true;
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new[]
		{
			new XElement("PaperProto", PaperProto?.Id ?? 0),
			new XElement("PageCount", PageCount),
			new XElement("DefaultTitle", new XCData(DefaultTitle ?? string.Empty)),
			new XElement("InitialReadables",
				from item in InitialReadables
				where item.Readable is not null
				select item.SaveToXml()),
			new XElement("InitialCollections",
				from item in InitialCollections
				select item.SaveToXml())
		}).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
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
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tpaper <id|name> - sets the item prototype for pages in this book\n\tpages <number> - sets the number of pages in a fresh copy of this book\n\ttitle <title|clear> - sets the default title of fresh books\n\tcontent list - lists preloaded readable content\n\tcontent add <page> <language> <script> [provenance] - creates immutable printed content via the editor\n\tcontent copy <page> <writing id> - references an existing immutable writing\n\tcontent drawing <page> <drawing id> - references an existing immutable drawing\n\tcontent collection <collection> [append|page <number>] - expands a writing collection into fresh books\n\tcontent edit <#> - replaces a direct writing reference with edited printed text\n\tcontent remove <#> - removes a direct readable or collection reference\n\tcontent clear - removes all preloaded content";

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
			case "title":
				return BuildingCommandDefaultTitle(actor, command);
			case "content":
			case "contents":
				return BuildingCommandContent(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandDefaultTitle(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What default title should fresh copies of this book use? Use CLEAR to remove the default title.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("clear", "none", "reset", "delete"))
		{
			DefaultTitle = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("Fresh copies of this book will no longer start with a title.");
			return true;
		}

		DefaultTitle = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"Fresh copies of this book will start titled \"{DefaultTitle.Colour(Telnet.BoldWhite)}\".");
		return true;
	}

	private bool BuildingCommandContent(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "list":
			case "show":
				return BuildingCommandContentList(actor);
			case "add":
				return BuildingCommandContentAdd(actor, command);
			case "copy":
				return BuildingCommandContentCopy(actor, command);
			case "drawing":
			case "draw":
				return BuildingCommandContentDrawing(actor, command);
			case "collection":
			case "collections":
				return BuildingCommandContentCollection(actor, command);
			case "edit":
				return BuildingCommandContentEdit(actor, command);
			case "remove":
			case "delete":
				return BuildingCommandContentRemove(actor, command);
			case "clear":
				_initialReadables.Clear();
				_initialCollections.Clear();
				Changed = true;
				actor.OutputHandler.Send("You remove all preloaded readable content from this book.");
				return true;
			default:
				actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandContentList(ICharacter actor)
	{
		if (!_initialReadables.Any() && !_initialCollections.Any())
		{
			actor.OutputHandler.Send("This book does not have any preloaded readable content.");
			return true;
		}

		var rows = new List<string[]>();
		var index = 1;
		foreach (var item in InitialReadables)
		{
			var readable = item.Readable;
			rows.Add(new[]
			{
				index++.ToString("N0", actor),
				readable is IDrawing ? "Drawing" : "Writing",
				item.Page.ToString("N0", actor),
				item.Order.ToString("N0", actor),
				readable is null ? "Missing" : $"#{readable.Id.ToString("N0", actor)}",
				readable?.DocumentLength.ToString("N0", actor) ?? "0",
				readable?.DescribeInLook(actor).RawText() ?? "Missing readable"
			});
		}

		foreach (var item in InitialCollections)
		{
			var collection = item.Collection;
			rows.Add(new[]
			{
				index++.ToString("N0", actor),
				"Collection",
				item.StartPage.ToString("N0", actor),
				"-",
				collection is null ? $"Missing #{item.CollectionId.ToString("N0", actor)}" : $"#{collection.Id.ToString("N0", actor)} {collection.Name}",
				collection?.Entries.Sum(x => x.Readable.DocumentLength).ToString("N0", actor) ?? "0",
				collection?.DefaultTitle.IfNullOrWhiteSpace(collection.Description).IfNullOrWhiteSpace("Untitled collection") ?? "Missing collection"
			});
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			rows,
			new[] { "#", "Kind", "Page", "Order", "Reference", "Length", "Description" },
			actor.LineFormatLength,
			colour: Telnet.Cyan,
			unicodeTable: actor.Account.UseUnicode));
		return true;
	}

	private bool BuildingCommandContentAdd(ICharacter actor, StringStack command)
	{
		if (!TryParseContentHeader(actor, command, out var page, out var language, out var script, out var provenance))
		{
			return false;
		}

		var colour = Gameworld.Colours.Get(Gameworld.GetStaticLong("DefaultWritingColourInText"));
		if (colour is null)
		{
			actor.OutputHandler.Send("There is no valid default writing colour configured for printed book content.");
			return false;
		}

		actor.EditorMode((text, handler, _) =>
		{
			var writing = new PrintedWriting(Gameworld, text, language, script, provenance, WritingStyleDescriptors.MachinePrinted, colour);
			if (!CanAddReadableToPage(page, writing.DocumentLength, out var error))
			{
				Gameworld.SaveManager.Abort(writing);
				handler.Send(error);
				return;
			}

			var template = BookPageContentTemplate.FromReadable(Gameworld, page, NextContentOrder(page), writing);
			_initialReadables.Add(template);
			Changed = true;
			handler.Send($"You add immutable printed content to page {page.ToString("N0", actor)} of this book.");
		}, (handler, _) => handler.Send("You decide not to add any preloaded content."), 1.0);
		return true;
	}

	private bool BuildingCommandContentCopy(ICharacter actor, StringStack command)
	{
		if (!TryParsePageAndId(actor, command, "writing", out var page, out var writingId))
		{
			return false;
		}

		var writing = Gameworld.Writings.Get(writingId);
		if (writing is null)
		{
			actor.OutputHandler.Send("There is no such writing.");
			return false;
		}

		var template = BookPageContentTemplate.FromWriting(Gameworld, page, NextContentOrder(page), writing);
		if (!CanAddTemplate(template, out var error))
		{
			actor.OutputHandler.Send(error);
			return false;
		}

		_initialReadables.Add(template);
		Changed = true;
		actor.OutputHandler.Send($"You reference writing #{writing.Id.ToString("N0", actor)} in the preloaded content for page {page.ToString("N0", actor)}.");
		return true;
	}

	private bool BuildingCommandContentDrawing(ICharacter actor, StringStack command)
	{
		if (!TryParsePageAndId(actor, command, "drawing", out var page, out var drawingId))
		{
			return false;
		}

		var drawing = Gameworld.Drawings.Get(drawingId);
		if (drawing is null)
		{
			actor.OutputHandler.Send("There is no such drawing.");
			return false;
		}

		var template = BookPageContentTemplate.FromReadable(Gameworld, page, NextContentOrder(page), drawing);
		if (!CanAddTemplate(template, out var error))
		{
			actor.OutputHandler.Send(error);
			return false;
		}

		_initialReadables.Add(template);
		Changed = true;
		actor.OutputHandler.Send($"You reference drawing #{drawing.Id.ToString("N0", actor)} in the preloaded content for page {page.ToString("N0", actor)}.");
		return true;
	}

	private bool BuildingCommandContentCollection(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which writing collection do you want to add to this book template?");
			return false;
		}

		var collection = Gameworld.WritingCollections.GetByIdOrName(command.PopSpeech());
		if (collection is null)
		{
			actor.OutputHandler.Send("There is no such writing collection.");
			return false;
		}

		var startPage = HighestContentPage() + 1;
		if (!command.IsFinished)
		{
			var mode = command.PopSpeech();
			if (mode.EqualTo("append"))
			{
				startPage = HighestContentPage() + 1;
			}
			else if (mode.EqualTo("page"))
			{
				if (command.IsFinished || !int.TryParse(command.PopSpeech(), out startPage) || startPage < 1)
				{
					actor.OutputHandler.Send("You must enter a valid starting page number.");
					return false;
				}
			}
			else if (!int.TryParse(mode, out startPage) || startPage < 1)
			{
				actor.OutputHandler.Send("Use APPEND or PAGE <number> after the collection name.");
				return false;
			}
		}

		if (!CanAddCollection(collection, startPage, out var error))
		{
			actor.OutputHandler.Send(error);
			return false;
		}

		_initialCollections.Add(new BookCollectionContentTemplate(Gameworld, collection.Id, startPage));
		Changed = true;
		actor.OutputHandler.Send($"Fresh copies of this book will expand writing collection #{collection.Id.ToString("N0", actor)} ({collection.Name.ColourName()}) starting at page {startPage.ToString("N0", actor)}.");
		return true;
	}

	private bool BuildingCommandContentEdit(ICharacter actor, StringStack command)
	{
		if (!TryGetDirectContentByIndex(actor, command, out var template))
		{
			return false;
		}

		if (template.Readable is not IWriting writing)
		{
			actor.OutputHandler.Send("Only direct writing references can be edited this way. Drawings and collection references must be edited at their source.");
			return false;
		}

		actor.EditorMode((text, handler, _) =>
		{
			var provenance = writing.Author?.Name ?? (writing.GetProperty("provenance")?.GetObject as string) ?? string.Empty;
			var replacement = new PrintedWriting(Gameworld, text, writing.Language, writing.Script, provenance, writing.Style,
				writing.WritingColour, writing.LiteracySkill, writing.LanguageSkill, writing.HandwritingSkill, writing.ForgerySkill);
			var index = _initialReadables.IndexOf(template);
			_initialReadables.Remove(template);
			if (!CanAddReadableToPage(template.Page, replacement.DocumentLength, out var error))
			{
				_initialReadables.Insert(index, template);
				Gameworld.SaveManager.Abort(replacement);
				handler.Send(error);
				return;
			}

			var replacementTemplate = BookPageContentTemplate.FromReadable(Gameworld, template.Page, template.Order, replacement);
			_initialReadables.Insert(index, replacementTemplate);
			Changed = true;
			handler.Send($"You replace preloaded content #{index + 1:N0} with a new immutable printed writing.");
		}, (handler, _) => handler.Send("You decide not to edit the preloaded content."), 1.0, writing.ParseFor(actor));
		return true;
	}

	private bool BuildingCommandContentRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which preloaded content number do you want to remove?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value < 1 || value > _initialReadables.Count + _initialCollections.Count)
		{
			actor.OutputHandler.Send($"You must enter a number between 1 and {(_initialReadables.Count + _initialCollections.Count).ToString("N0", actor)}.");
			return false;
		}

		if (value <= _initialReadables.Count)
		{
			var template = InitialReadables.ElementAt(value - 1);
			_initialReadables.Remove(template);
			ReorderContent(template.Page);
		}
		else
		{
			_initialCollections.Remove(InitialCollections.ElementAt(value - _initialReadables.Count - 1));
		}

		Changed = true;
		actor.OutputHandler.Send("You remove that preloaded content reference from this book.");
		return true;
	}

	private bool TryParseContentHeader(ICharacter actor, StringStack command, out int page, out ILanguage language,
		out IScript script, out string provenance)
	{
		page = 0;
		language = null;
		script = null;
		provenance = string.Empty;

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which page should this preloaded content appear on?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out page) || page < 1 || page > PageCount)
		{
			actor.OutputHandler.Send($"You must enter a page number between 1 and {PageCount.ToString("N0", actor)}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which language should this printed text be written in?");
			return false;
		}

		var text = command.PopSpeech();
		language = long.TryParse(text, out var value)
			? Gameworld.Languages.Get(value)
			: Gameworld.Languages.GetByName(text);
		if (language is null)
		{
			actor.OutputHandler.Send("There is no such language.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which script should this printed text use?");
			return false;
		}

		text = command.PopSpeech();
		script = long.TryParse(text, out value)
			? Gameworld.Scripts.Get(value)
			: Gameworld.Scripts.GetByName(text);
		if (script is null)
		{
			actor.OutputHandler.Send("There is no such script.");
			return false;
		}

		provenance = command.SafeRemainingArgument;
		return true;
	}

	private bool TryParsePageAndId(ICharacter actor, StringStack command, string itemType, out int page, out long id)
	{
		page = 0;
		id = 0;
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which page should the {itemType} be preloaded onto?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out page) || page < 1 || page > PageCount)
		{
			actor.OutputHandler.Send($"You must enter a page number between 1 and {PageCount.ToString("N0", actor)}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which existing {itemType} ID do you want to reference in the book template?");
			return false;
		}

		if (!long.TryParse(command.PopSpeech(), out id))
		{
			actor.OutputHandler.Send($"You must enter a valid {itemType} ID.");
			return false;
		}

		return true;
	}

	private bool TryGetDirectContentByIndex(ICharacter actor, StringStack command, out BookPageContentTemplate template)
	{
		template = null;
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which direct preloaded content number do you want to target?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value < 1 || value > _initialReadables.Count)
		{
			actor.OutputHandler.Send($"You must enter a direct readable number between 1 and {_initialReadables.Count.ToString("N0", actor)}.");
			return false;
		}

		template = InitialReadables.ElementAt(value - 1);
		return true;
	}

	private int NextContentOrder(int page)
	{
		return _initialReadables.Where(x => x.Page == page).Select(x => x.Order).DefaultIfEmpty(0).Max() + 1;
	}

	private int HighestContentPage()
	{
		return InitialReadableReferences.Select(x => x.Page).DefaultIfEmpty(0).Max();
	}

	private int DocumentLengthUsedForPage(int page)
	{
		return InitialReadableReferences.Where(x => x.Page == page).Sum(x => x.Readable.DocumentLength);
	}

	private void ReorderContent(int page)
	{
		var order = 1;
		foreach (var item in _initialReadables.Where(x => x.Page == page).OrderBy(x => x.Order))
		{
			item.Order = order++;
		}
	}

	private bool CanAddTemplate(BookPageContentTemplate template, out string error)
	{
		if (template.Readable is null)
		{
			error = "The preloaded content must refer to a valid readable.";
			return false;
		}

		return CanAddReadableToPage(template.Page, template.DocumentLength, out error);
	}

	private bool CanAddReadableToPage(int page, int documentLength, out string error)
	{
		if (page < 1 || page > PageCount)
		{
			error = $"Preloaded content can only target pages between 1 and {PageCount:N0}.";
			return false;
		}

		var used = DocumentLengthUsedForPage(page);
		if (used + documentLength > MaximumCharacterLengthOfText)
		{
			error = $"The preloaded content for page {page:N0} would be {(used + documentLength):N0} characters, which exceeds the page capacity of {MaximumCharacterLengthOfText:N0}.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	private bool CanAddCollection(IWritingCollection collection, int startPage, out string error)
	{
		if (!collection.Entries.Any())
		{
			error = "That writing collection does not have any readable entries.";
			return false;
		}

		var firstPage = collection.Entries.Min(x => x.Page);
		foreach (var group in collection.Entries.GroupBy(x => startPage + x.Page - firstPage))
		{
			if (group.Key < 1 || group.Key > PageCount)
			{
				error = $"That collection would write to page {group.Key:N0}, but this book only has pages 1 to {PageCount:N0}.";
				return false;
			}

			var total = DocumentLengthUsedForPage(group.Key) + group.Sum(x => x.Readable.DocumentLength);
			if (total > MaximumCharacterLengthOfText)
			{
				error = $"That collection would put {total:N0} characters on page {group.Key:N0}, which exceeds the page capacity of {MaximumCharacterLengthOfText:N0}.";
				return false;
			}
		}

		error = string.Empty;
		return true;
	}

	private bool BuildingCommandPageCount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many pages should a fresh copy of this book start with?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid number of pages greater than zero for this book.");
			return false;
		}

		if (InitialReadableReferences.Any(x => x.Page > value))
		{
			actor.OutputHandler.Send("You cannot reduce the page count below the highest page that already has preloaded content.");
			return false;
		}

		PageCount = value;
		Changed = true;
		actor.OutputHandler.Send($"This book will now load up with {PageCount:N0} pages.");
		return true;
	}

	private bool BuildingCommandPaperProto(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must specify the id or name of a paper prototype to use when pages are torn out of this book.");
			return false;
		}

		var proto = Gameworld.ItemProtos.GetByIdOrUniqueNameOrName(command.PopSpeech());
		if (proto == null)
		{
			actor.OutputHandler.Send("There is no such prototype.");
			return false;
		}

		if (!proto.IsItemType<PaperSheetGameItemComponentProto>())
		{
			actor.OutputHandler.Send(
				$"The prototype {proto.Name.Colour(Telnet.Cyan)} (#{proto.Id}r{proto.RevisionNumber}) is not a paper sheet prototype.");
			return false;
		}

		if (proto.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send(
				$"The prototype {proto.Name.Colour(Telnet.Cyan)} (#{proto.Id}r{proto.RevisionNumber}) is not a currently approved paper sheet prototype.");
			return false;
		}

		_paperProtoId = proto.Id;
		Changed = true;
		actor.OutputHandler.Send(
			$"This book will now use the prototype {proto.Name.Colour(Telnet.Cyan)} (#{proto.Id}r{proto.RevisionNumber}) for its torn pages.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a book with {4} pages, that references the {5} prototype for its paper.\nFresh copies start {6} and contain {7:N0} direct readable references plus {8:N0} writing collection references.",
			"Book Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			PageCount,
			PaperProto != null
				? $"{PaperProto.Name.Colour(Telnet.Cyan)} (#{PaperProto.Id})"
				: "Not Set".Colour(Telnet.Red),
			string.IsNullOrWhiteSpace(DefaultTitle) ? "untitled" : $"titled \"{DefaultTitle.Colour(Telnet.BoldWhite)}\"",
			_initialReadables.Count,
			_initialCollections.Count
		);
	}
}