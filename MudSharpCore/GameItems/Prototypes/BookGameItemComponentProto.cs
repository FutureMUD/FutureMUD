using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    internal IEnumerable<BookPageContentTemplate> InitialReadables => _initialReadables.OrderBy(x => x.Page).ThenBy(x => x.Order);

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
        foreach (var item in root.Element("InitialReadables")?.Elements("Writing") ?? Enumerable.Empty<XElement>())
        {
            _initialReadables.Add(new BookPageContentTemplate(Gameworld, item));
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
        "You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tpaper <id|name> - sets the item prototype for pages in this book\n\tpages <number> - sets the number of pages in a fresh copy of this book\n\ttitle <title|clear> - sets the default title of fresh books\n\tcontent list - lists preloaded printed content\n\tcontent add <page> <language> <script> [provenance] - adds printed content via the editor\n\tcontent copy <page> <writing id> - copies an existing writing into the printed content template\n\tcontent edit <#> - edits the text of a preloaded content entry\n\tcontent remove <#> - removes a preloaded content entry\n\tcontent clear - removes all preloaded content";

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
            actor.Send("What default title should fresh copies of this book use? Use CLEAR to remove the default title.");
            return false;
        }

        if (command.PeekSpeech().EqualToAny("clear", "none", "reset", "delete"))
        {
            DefaultTitle = string.Empty;
            Changed = true;
            actor.Send("Fresh copies of this book will no longer start with a title.");
            return true;
        }

        DefaultTitle = command.SafeRemainingArgument;
        Changed = true;
        actor.Send($"Fresh copies of this book will start titled \"{DefaultTitle.Colour(Telnet.BoldWhite)}\".");
        return true;
    }

    private bool BuildingCommandContent(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send(BuildingHelpText.SubstituteANSIColour());
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
            case "edit":
                return BuildingCommandContentEdit(actor, command);
            case "remove":
            case "delete":
                return BuildingCommandContentRemove(actor, command);
            case "clear":
                _initialReadables.Clear();
                Changed = true;
                actor.Send("You remove all preloaded printed content from this book.");
                return true;
            default:
                actor.Send(BuildingHelpText.SubstituteANSIColour());
                return false;
        }
    }

    private bool BuildingCommandContentList(ICharacter actor)
    {
        if (!_initialReadables.Any())
        {
            actor.Send("This book does not have any preloaded printed content.");
            return true;
        }

        actor.Send(StringUtilities.GetTextTable(
            from item in InitialReadables.Select((x, i) => (Content: x, Index: i + 1))
            select new[]
            {
                item.Index.ToString("N0", actor),
                item.Content.Page.ToString("N0", actor),
                item.Content.Order.ToString("N0", actor),
                item.Content.Language?.Name ?? "Unknown",
                item.Content.Script?.Name ?? "Unknown",
                item.Content.DocumentLength.ToString("N0", actor),
                item.Content.Provenance.IfNullOrWhiteSpace(""),
                item.Content.Text.Length <= actor.LineFormatLength / 3
                    ? item.Content.Text
                    : $"{item.Content.Text[..Math.Max(0, actor.LineFormatLength / 3 - 3)]}..."
            },
            new[] { "#", "Page", "Order", "Language", "Script", "Length", "Source", "Text" },
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
            actor.Send("There is no valid default writing colour configured for printed book content.");
            return false;
        }

        actor.EditorMode((text, handler, _) =>
        {
            var template = new BookPageContentTemplate(Gameworld, page, NextContentOrder(page), text, language, script,
                provenance, colour, WritingStyleDescriptors.MachinePrinted);
            if (!CanAddTemplate(template, out var error))
            {
                handler.Send(error);
                return;
            }

            _initialReadables.Add(template);
            Changed = true;
            handler.Send($"You add preloaded printed content to page {page.ToString("N0", actor)} of this book.");
        }, (handler, _) => handler.Send("You decide not to add any preloaded content."), 1.0);
        return true;
    }

    private bool BuildingCommandContentCopy(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("Which page should the copied writing be preloaded onto?");
            return false;
        }

        if (!int.TryParse(command.PopSpeech(), out var page) || page < 1)
        {
            actor.Send("You must enter a valid page number.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.Send("Which existing writing ID do you want to copy into the book template?");
            return false;
        }

        if (!long.TryParse(command.PopSpeech(), out var writingId))
        {
            actor.Send("You must enter a valid writing ID.");
            return false;
        }

        var writing = Gameworld.Writings.Get(writingId);
        if (writing is null)
        {
            actor.Send("There is no such writing.");
            return false;
        }

        var template = BookPageContentTemplate.FromWriting(Gameworld, page, NextContentOrder(page), writing);
        if (!CanAddTemplate(template, out var error))
        {
            actor.Send(error);
            return false;
        }

        _initialReadables.Add(template);
        Changed = true;
        actor.Send($"You copy writing #{writing.Id.ToString("N0", actor)} into the preloaded content for page {page.ToString("N0", actor)}.");
        return true;
    }

    private bool BuildingCommandContentEdit(ICharacter actor, StringStack command)
    {
        if (!TryGetContentByIndex(actor, command, out var template))
        {
            return false;
        }

        actor.EditorMode((text, handler, _) =>
        {
            var oldText = template.Text;
            template.Text = text;
            if (!CanFitPage(template.Page, out var error))
            {
                template.Text = oldText;
                handler.Send(error);
                return;
            }

            Changed = true;
            handler.Send($"You edit preloaded content #{InitialReadables.ToList().IndexOf(template) + 1:N0}.");
        }, (handler, _) => handler.Send("You decide not to edit the preloaded content."), 1.0, template.Text);
        return true;
    }

    private bool BuildingCommandContentRemove(ICharacter actor, StringStack command)
    {
        if (!TryGetContentByIndex(actor, command, out var template))
        {
            return false;
        }

        _initialReadables.Remove(template);
        ReorderContent(template.Page);
        Changed = true;
        actor.Send("You remove that preloaded content from this book.");
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
            actor.Send("Which page should this preloaded content appear on?");
            return false;
        }

        if (!int.TryParse(command.PopSpeech(), out page) || page < 1 || page > PageCount)
        {
            actor.Send($"You must enter a page number between 1 and {PageCount.ToString("N0", actor)}.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.Send("Which language should this printed text be written in?");
            return false;
        }

        var text = command.PopSpeech();
        language = long.TryParse(text, out var value)
            ? Gameworld.Languages.Get(value)
            : Gameworld.Languages.GetByName(text);
        if (language is null)
        {
            actor.Send("There is no such language.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.Send("Which script should this printed text use?");
            return false;
        }

        text = command.PopSpeech();
        script = long.TryParse(text, out value)
            ? Gameworld.Scripts.Get(value)
            : Gameworld.Scripts.GetByName(text);
        if (script is null)
        {
            actor.Send("There is no such script.");
            return false;
        }

        provenance = command.SafeRemainingArgument;
        return true;
    }

    private bool TryGetContentByIndex(ICharacter actor, StringStack command, out BookPageContentTemplate template)
    {
        template = null;
        if (command.IsFinished)
        {
            actor.Send("Which preloaded content number do you want to target?");
            return false;
        }

        if (!int.TryParse(command.PopSpeech(), out var value) || value < 1 || value > _initialReadables.Count)
        {
            actor.Send($"You must enter a number between 1 and {_initialReadables.Count.ToString("N0", actor)}.");
            return false;
        }

        template = InitialReadables.ElementAt(value - 1);
        return true;
    }

    private int NextContentOrder(int page)
    {
        return _initialReadables.Where(x => x.Page == page).Select(x => x.Order).DefaultIfEmpty(0).Max() + 1;
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
        if (template.Page < 1 || template.Page > PageCount)
        {
            error = $"Preloaded content can only target pages between 1 and {PageCount:N0}.";
            return false;
        }

        if (template.Language is null)
        {
            error = "The preloaded content must use a valid language.";
            return false;
        }

        if (template.Script is null)
        {
            error = "The preloaded content must use a valid script.";
            return false;
        }

        if (template.Colour is null)
        {
            error = "The preloaded content must use a valid text colour.";
            return false;
        }

        var used = _initialReadables.Where(x => x.Page == template.Page).Sum(x => x.DocumentLength);
        if (used + template.DocumentLength > MaximumCharacterLengthOfText)
        {
            error = $"The preloaded content for page {template.Page:N0} would be {(used + template.DocumentLength):N0} characters, which exceeds the page capacity of {MaximumCharacterLengthOfText:N0}.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private bool CanFitPage(int page, out string error)
    {
        var used = _initialReadables.Where(x => x.Page == page).Sum(x => x.DocumentLength);
        if (used > MaximumCharacterLengthOfText)
        {
            error = $"The preloaded content for page {page:N0} is {used:N0} characters, which exceeds the page capacity of {MaximumCharacterLengthOfText:N0}.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private bool BuildingCommandPageCount(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("How many pages should a fresh copy of this book start with?");
            return false;
        }

        if (!int.TryParse(command.SafeRemainingArgument, out int value) || value <= 0)
        {
            actor.Send("You must enter a valid number of pages greater than zero for this book.");
            return false;
        }

        if (_initialReadables.Any(x => x.Page > value))
        {
            actor.Send("You cannot reduce the page count below the highest page that already has preloaded content.");
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

        IGameItemProto proto = long.TryParse(command.PopSpeech(), out long value)
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
            "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a book with {4} pages, that references the {5} prototype for its paper.\nFresh copies start {6} and contain {7:N0} pieces of preloaded printed content.",
            "Book Game Item Component".Colour(Telnet.Cyan),
            Id,
            RevisionNumber,
            Name,
            PageCount,
            PaperProto != null
                ? $"{PaperProto.Name.Colour(Telnet.Cyan)} (#{PaperProto.Id})"
                : "Not Set".Colour(Telnet.Red),
            string.IsNullOrWhiteSpace(DefaultTitle) ? "untitled" : $"titled \"{DefaultTitle.Colour(Telnet.BoldWhite)}\"",
            _initialReadables.Count
        );
    }
}
