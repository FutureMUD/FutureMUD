using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class BookGameItemComponent : GameItemComponent, IWriteable, IReadable, ITurnable, IOpenable, ITearable
{
    protected BookGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (BookGameItemComponentProto)newProto;
    }

    public override InitialisationPhase InitialisationPhase => InitialisationPhase.AfterFirstDatabaseHit;

    #region Constructors

    public BookGameItemComponent(BookGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(
        parent, proto, temporary)
    {
        _prototype = proto;
        CurrentPage = 1;
        Title = proto.DefaultTitle;
        if (!temporary)
        {
            foreach (var item in proto.InitialReadableReferences)
            {
                if (!CanAddReadable(item.Readable, item.Page))
                {
                    continue;
                }

                AddReadableReference(item.Readable, item.Page, false);
            }
        }
        CalculateWritings();
    }

    public BookGameItemComponent(MudSharp.Models.GameItemComponent component, BookGameItemComponentProto proto,
        IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public BookGameItemComponent(BookGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
        newParent, temporary)
    {
        _prototype = rhs._prototype;
        foreach ((int Page, int Order, ICanBeRead Writing) item in rhs.PagesAndReadables)
        {
            if (item.Writing is null)
            {
                continue;
            }

            var readable = temporary ? item.Writing : item.Writing.CopyReadable();
            PagesAndReadables.Add((item.Page, item.Order, readable));
            if (!temporary)
            {
                RegisterReadable(readable);
            }
        }

        TornPages = rhs.TornPages.ToHashSet();
        IsOpen = rhs.IsOpen;
        CurrentPage = rhs.CurrentPage;
        Title = rhs.Title;
        CalculateWritings();
    }

    protected void LoadFromXml(XElement root)
    {
        IsOpen = bool.Parse(root.Element("Open").Value);
        Title = root.Element("Title")?.Value;
        CurrentPage = int.Parse(root.Element("CurrentPage").Value);
        foreach (XElement item in root.Element("TornPages").Elements())
        {
            TornPages.Add(int.Parse(item.Value));
        }

        foreach (XElement item in root.Element("Writings").Elements())
        {
            var readable = item.Name.LocalName.EqualTo("Writing")
                ? (ICanBeRead)Gameworld.Writings.Get(long.Parse(item.Attribute("Id").Value))
                : Gameworld.Drawings.Get(long.Parse(item.Attribute("Id").Value));
            if (readable is null)
            {
                continue;
            }

            PagesAndReadables.Add((int.Parse(item.Attribute("Page").Value), int.Parse(item.Attribute("Order").Value), readable));
        }

        CalculateWritings();
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new BookGameItemComponent(this, newParent, temporary);
    }

    #endregion

    public bool CanAddReadable(ICanBeRead readable, int page)
    {
        if (readable is null)
        {
            return false;
        }

        if (page < 1 || page > _prototype.PageCount)
        {
            return false;
        }

        if (TornPages.Contains(page))
        {
            return false;
        }

        if (readable.DocumentLength > MaximumCharacterLengthOfText - DocumentLengthUsedForPage(page))
        {
            return false;
        }

        return true;
    }

    public string WhyCannotAddReadable(ICanBeRead readable, int page)
    {
        if (page < 1 || page > _prototype.PageCount)
        {
            return $"You can only add readable content to a page between 1 and {_prototype.PageCount:N0}.";
        }

        if (TornPages.Contains(page))
        {
            return $"The {page.ToOrdinal()} page has been torn out of {Parent.HowSeen(null)}.";
        }

        if (readable.DocumentLength > MaximumCharacterLengthOfText - DocumentLengthUsedForPage(page))
        {
            return $"There is not enough space left on page {page:N0} of {Parent.HowSeen(null)} to add that content.";
        }

        return "That readable content cannot be added to the book.";
    }

    private void RegisterReadable(ICanBeRead readable)
    {
        switch (readable)
        {
            case IWriting writing:
                if (writing is ILateInitialisingItem { IdHasBeenRegistered: false } || !Gameworld.Writings.Has(writing.Id))
                {
                    Gameworld.Add(writing);
                }
                break;
            case IDrawing drawing:
                if (drawing is ILateInitialisingItem { IdHasBeenRegistered: false } || !Gameworld.Drawings.Has(drawing.Id))
                {
                    Gameworld.Add(drawing);
                }
                break;
        }
    }

    #region Saving

    protected override string SaveToXml()
    {
        return new XElement("Definition", new[]
            {
                new XElement("Open", IsOpen),
                new XElement("TornPages",
                    from item in TornPages
                    select new XElement("TornPage", item)),
                new XElement("Title", new XCData(Title ?? "")),
                new XElement("CurrentPage", CurrentPage),
                new XElement("Writings",
                    from item in PagesAndReadables
                    where item.Writing is not null
                    select new XElement(item.Writing is IWriting ? "Writing" : "Drawing",
                        new XAttribute("Id", item.Writing.Id), new XAttribute("Page", item.Page),
                        new XAttribute("Order", item.Order)))
            }
        ).ToString();
    }

    #endregion

    #region IReadable Implementation

    IEnumerable<IWriting> IReadable.Writings => Readables.OfType<IWriting>();

    IEnumerable<IDrawing> IReadable.Drawings => Readables.OfType<IDrawing>();

    #endregion

    #region IWriteable Implementation

    public IEnumerable<ICanBeRead> Readables { get; set; }

    private void CalculateWritings()
    {
        Readables = PagesAndReadables.Where(x => x.Page == CurrentPage)
                                     .OrderBy(x => x.Order)
                                     .Select(x => x.Writing)
                                     .Where(x => x is not null)
                                     .ToList();
    }

    public int MaximumCharacterLengthOfText => _prototype.MaximumCharacterLengthOfText;

    public bool HasSpareRoom => DocumentLengthUsed < MaximumCharacterLengthOfText;

    public int DocumentLengthUsed => Readables.Sum(x => x.DocumentLength);

    public int DocumentLengthUsedForPage(int page)
    {
        return PagesAndReadables.Where(x => x.Page == page && x.Writing is not null)
                                .Sum(x => x.Writing.DocumentLength);
    }

    public IEnumerable<(int Page, int Order, ICanBeRead Readable)> AllPagesAndReadables =>
        PagesAndReadables.Where(x => x.Writing is not null)
                         .OrderBy(x => x.Page)
                         .ThenBy(x => x.Order)
                         .Select(x => (x.Page, x.Order, x.Writing));

    public int HighestWrittenPage => AllPagesAndReadables.Select(x => x.Page).DefaultIfEmpty(0).Max();

    private int NextOrderForPage(int page)
    {
        return PagesAndReadables.Where(x => x.Page == page).Select(x => x.Order).DefaultIfEmpty(0).Max() + 1;
    }

    private bool AddReadableReference(ICanBeRead readable, int page, bool markChanged = true)
    {
        if (!CanAddReadable(readable, page))
        {
            return false;
        }

        PagesAndReadables.Add((page, NextOrderForPage(page), readable));
        CalculateWritings();
        RegisterReadable(readable);
        if (markChanged)
        {
            Changed = true;
        }

        return true;
    }

    public bool AddReadable(ICanBeRead readable, int page)
    {
        return AddReadableReference(readable, page);
    }

    public bool CanAddWriting(IWriting writing)
    {
        return CanAddWriting(writing, CurrentPage);
    }

    public bool CanAddWriting(IWriting writing, int page)
    {
        return CanAddReadable(writing, page);
    }

    public string WhyCannotAddWriting(IWriting writing, int page)
    {
        return WhyCannotAddReadable(writing, page);
    }

    public bool AddWriting(IWriting newWriting)
    {
        return AddWriting(newWriting, CurrentPage);
    }

    public bool AddWriting(IWriting newWriting, int page)
    {
        return AddReadableReference(newWriting, page);
    }

    public string Title { get; set; }

    public bool CanWrite(ICharacter character, IWritingImplement implement, IWriting writing)
    {
        if (!IsOpen)
        {
            return false;
        }

        if (implement != null)
        {
            switch (implement.WritingImplementType)
            {
                case WritingImplementType.ComputerStylus:
                case WritingImplementType.Stylus:
                case WritingImplementType.Chisel:
                    return false;
            }

            if (!implement.Primed)
            {
                return false;
            }
        }

        if (writing != null &&
            writing.DocumentLength > MaximumCharacterLengthOfText - DocumentLengthUsed)
        {
            return false;
        }

        return true;
    }

    public string WhyCannotWrite(ICharacter character, IWritingImplement implement, IWriting writing)
    {
        if (!IsOpen)
        {
            return $"You cannot write on {Parent.HowSeen(character)} while it is closed.";
        }

        if (implement != null)
        {
            switch (implement.WritingImplementType)
            {
                case WritingImplementType.ComputerStylus:
                case WritingImplementType.Stylus:
                case WritingImplementType.Chisel:
                    return
                        $"{implement.Parent.HowSeen(character)} is not an appropriate writing instrument for writing on {Parent.HowSeen(character)}.";
            }

            if (!implement.Primed)
            {
                return $"{implement.Parent.HowSeen(character)} has not yet been primed for writing.";
            }
        }

        if (writing != null && writing.DocumentLength > MaximumCharacterLengthOfText - DocumentLengthUsed)
        {
            return
                $"There is not enough space left on the current page of {Parent.HowSeen(character)} to write all that.";
        }

        throw new ApplicationException("Unknown WhyCannotWrite reason in PaperSheetGameItemComponent.");
    }

    public bool Write(ICharacter character, IWritingImplement implement, IWriting writing)
    {
        if (!CanWrite(character, implement, writing))
        {
            character.Send(WhyCannotWrite(character, implement, writing));
            return false;
        }

        Difficulty difficulty = (Difficulty)Math.Max(0, Math.Min(10, (writing.Style.MinimumHandwritingSkill() + 10.0) / 15.0));
        ICheck check = Gameworld.GetCheck(CheckType.HandwritingImprovementCheck);
        for (int i = 0; i < 3; i++)
        {
            check.Check(character, difficulty);
        }

        AddReadableReference(writing, CurrentPage);
        implement?.Use(writing.DocumentLength);
        return true;
    }

    public string WhyCannotGiveTitle(ICharacter character, string title)
    {
        throw new ApplicationException(
            "BookGameItemComponent had WhyCannotGiveTitle called - which is an invalid operation.");
    }

    public bool CanGiveTitle(ICharacter character, string title)
    {
        return true;
    }

    public bool GiveTitle(ICharacter character, string title)
    {
        Title = title;
        Changed = true;
        return true;
    }

    public bool CanAddDrawing(IDrawing drawing)
    {
        return CanAddReadable(drawing, CurrentPage);
    }

    public bool CanAddDrawing(IDrawing drawing, int page)
    {
        return CanAddReadable(drawing, page);
    }

    public bool AddDrawing(IDrawing drawing)
    {
        return AddDrawing(drawing, CurrentPage);
    }

    public bool AddDrawing(IDrawing drawing, int page)
    {
        return AddReadableReference(drawing, page);
    }

    /// <summary>
    /// Determines whether the character can draw on this writeable with the given implement and text
    /// </summary>
    /// <param name="character">The character doing the drawing</param>
    /// <param name="implement">The writing implement</param>
    /// <param name="drawing">The proposed drawing to put on the writeable. Can be null if this is a check prior to having dropped into the editor.</param>
    /// <returns>True if the character can draw on the proposed writeable in the proposed way</returns>
    public bool CanDraw(ICharacter character, IWritingImplement implement, IDrawing drawing)
    {
        if (!IsOpen)
        {
            return false;
        }

        if (implement != null)
        {
            switch (implement.WritingImplementType)
            {
                case WritingImplementType.ComputerStylus:
                case WritingImplementType.Stylus:
                case WritingImplementType.Chisel:
                    return false;
            }

            if (!implement.Primed)
            {
                return false;
            }
        }

        if (drawing != null &&
            drawing.DocumentLength > MaximumCharacterLengthOfText - DocumentLengthUsed)
        {
            return false;
        }

        return true;
    }

    public string WhyCannotDraw(ICharacter character, IWritingImplement implement, IDrawing drawing)
    {
        if (!IsOpen)
        {
            return $"You cannot draw on {Parent.HowSeen(character)} while it is closed.";
        }

        if (implement != null)
        {
            switch (implement.WritingImplementType)
            {
                case WritingImplementType.ComputerStylus:
                case WritingImplementType.Stylus:
                case WritingImplementType.Chisel:
                    return
                        $"{implement.Parent.HowSeen(character)} is not an appropriate writing instrument for drawing on {Parent.HowSeen(character)}.";
            }

            if (!implement.Primed)
            {
                return $"{implement.Parent.HowSeen(character)} has not yet been primed for drawing.";
            }
        }

        if (drawing != null && drawing.DocumentLength > MaximumCharacterLengthOfText - DocumentLengthUsed)
        {
            return
                $"There is not enough space left on the current page of {Parent.HowSeen(character)} to draw a picture of that size.";
        }

        throw new ApplicationException("Unknown WhyCannotDraw reason in PaperSheetGameItemComponent.");
    }

    public bool Draw(ICharacter character, IWritingImplement implement, IDrawing drawing)
    {
        if (!CanDraw(character, implement, drawing))
        {
            character.Send(WhyCannotDraw(character, implement, drawing));
            return false;
        }

        Difficulty difficulty = Difficulty.Easy;
        switch (drawing.DrawingSize)
        {
            case DrawingSize.Doodle:
                difficulty = Difficulty.Normal;
                break;
            case DrawingSize.Figure:
                difficulty = Difficulty.Hard;
                break;
            case DrawingSize.Sketch:
                difficulty = Difficulty.VeryHard;
                break;
            case DrawingSize.Picture:
                difficulty = Difficulty.ExtremelyHard;
                break;
            case DrawingSize.Mural:
                difficulty = Difficulty.Insane;
                break;
        }

        ICheck check = Gameworld.GetCheck(CheckType.DrawingImprovementCheck);
        for (int i = 0; i < 5; i++)
        {
            check.Check(character, difficulty);
        }

        AddReadableReference(drawing, CurrentPage);
        implement?.Use(drawing.DocumentLength);
        return true;
    }

    #endregion


    public HashSet<int> TornPages { get; } = new();

    public List<(int Page, int Order, ICanBeRead Writing)> PagesAndReadables = new();

    #region ITurnable Implementation

    public int CurrentPage { get; protected set; }

    public bool Turn(ICharacter actor, double turnExtent, IEmote emote)
    {
        if (!CanTurn(actor, turnExtent))
        {
            actor?.Send(WhyCannotTurn(actor, turnExtent));
            return false;
        }

        CurrentPage = (int)turnExtent;
        CalculateWritings();
        if (actor != null)
        {
            actor.OutputHandler.Handle(
                new MixedEmoteOutput(new Emote($"@ turn|turns $0 to the {CurrentPage.ToOrdinal()} page", actor, Parent))
                    .Append(emote));
        }
        else
        {
            Parent.OutputHandler.Handle(
                new EmoteOutput(new Emote($"@ turn|turns to the {CurrentPage.ToOrdinal()} page.", Parent)));
        }

        return true;
    }

    public bool CanTurn(ICharacter actor, double turnExtent)
    {
        if (!IsOpen)
        {
            return false;
        }

        int page = (int)turnExtent;
        if (page < 1 || page > _prototype.PageCount)
        {
            return false;
        }

        if (TornPages.Contains(page))
        {
            return false;
        }

        if (CurrentPage == page)
        {
            return false;
        }

        return true;
    }

    public string WhyCannotTurn(ICharacter actor, double turnExtent)
    {
        if (!IsOpen)
        {
            return $"You cannot turn the pages of {Parent.HowSeen(actor)} while it is closed.";
        }

        int page = (int)turnExtent;
        if (page < 1 || page > _prototype.PageCount)
        {
            return $"You can only turn to a page between 1 and {_prototype.PageCount} for {Parent.HowSeen(actor)}.";
        }

        if (TornPages.Contains(page))
        {
            return $"The {page.ToOrdinal()} page has been torn out of {Parent.HowSeen(actor)}.";
        }

        if (CurrentPage == page)
        {
            return $"{Parent.HowSeen(actor, true)} is already turned to the {page.ToOrdinal()} page.";
        }

        throw new ApplicationException("Got to the end of BookGameItemComponent.WhyCannotTurn");
    }

    public double CurrentExtent => CurrentPage;

    public double MinimumExtent => 1;
    public double MaximumExtent => _prototype.PageCount;

    public double DefaultExtentIncrement => 1.0;
    public string ExtentDescriptor => "page";

    #endregion

    #region IOpenable Implementation

    public bool CanOpen(IBody opener)
    {
        return !IsOpen;
    }

    public WhyCannotOpenReason WhyCannotOpen(IBody opener)
    {
        return WhyCannotOpenReason.AlreadyOpen;
    }

    public void Open()
    {
        IsOpen = true;
        OnOpen?.Invoke(this);
        Changed = true;
    }

    public bool CanClose(IBody closer)
    {
        return IsOpen;
    }

    public WhyCannotCloseReason WhyCannotClose(IBody closer)
    {
        return WhyCannotCloseReason.AlreadyClosed;
    }

    public void Close()
    {
        for (int i = 1; i <= _prototype.PageCount; i++)
        {
            if (!TornPages.Contains(i))
            {
                CurrentPage = i;
                CalculateWritings();
            }
        }

        IsOpen = false;
        OnClose?.Invoke(this);
        Changed = true;
    }

    public bool IsOpen { get; set; }

    public event OpenableEvent OnOpen;
    public event OpenableEvent OnClose;

    #endregion

    #region GameItemComponent Overrides

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return type == DescriptionType.Short || type == DescriptionType.Full;
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        switch (type)
        {
            case DescriptionType.Short:
                return DecorateShortDesc(voyeur, description);
            case DescriptionType.Full:
                return DecorateFullDesc(voyeur, description);
        }

        return base.Decorate(voyeur, name, description, type, colour, flags);
    }

    private string DecorateFullDesc(IPerceiver voyeur, string description)
    {
        StringBuilder sb = new();
        if (voyeur is not ICharacter character)
        {
            return description;
        }

        sb.AppendLine(description);
        sb.AppendLine();
        if (!IsOpen)
        {
            sb.AppendLine(
                $"This item is a book{(string.IsNullOrEmpty(Title) ? ", which is not currently titled" : $"titled as \"{Title.Colour(Telnet.BoldWhite)}\"")}."
                    .Colour(Telnet.Yellow));
            sb.AppendLine("It is not currently open.".Colour(Telnet.Yellow));
        }
        else if (TornPages.Count >= _prototype.PageCount)
        {
            sb.AppendLine("This item is a book, but all of the pages have been torn out.".Colour(Telnet.Red));
            sb.AppendLine($"It is {(string.IsNullOrEmpty(Title) ? "not currently titled" : $"titled as \"{Title.Colour(Telnet.BoldWhite)}\"")}."
                .Colour(Telnet.Yellow));
        }
        else
        {
            sb.AppendLine(
                $"This item is a book with a total of {_prototype.PageCount.ToString("N0", voyeur).ColourValue()} pages.".Colour(Telnet.Yellow));
            sb.AppendLine($"It is currently open to the {CurrentPage.ToOrdinal().ColourValue()} page.".Colour(Telnet.Yellow));
            sb.AppendLine(
                $"Each page can contain {MaximumCharacterLengthOfText.ToString("N0", voyeur).ColourValue()} characters of written text."
                    .Colour(Telnet.Yellow));
            sb.AppendLine($"It is {(string.IsNullOrEmpty(Title) ? "not currently titled" : $"titled as \"{Title.Colour(Telnet.BoldWhite)}\"")}."
                .Colour(Telnet.Yellow));
            if (!Readables.Any())
            {
                sb.AppendLine("The current page does not presently have anything written on it.".Colour(Telnet.Red));
            }
            else
            {
                int itemNum = 1;
                sb.AppendLine(
                    $"The current page has {Readables.Count().ToString("N0", voyeur).ColourValue()} separate pieces of writing and drawing. Type {"read <number>".Colour(Telnet.Yellow)} to read each piece:\n");
                foreach (ICanBeRead item in Readables)
                {
                    sb.AppendLine($"\t#{(itemNum++).ToString("N0", voyeur)}) {item.DescribeInLook(character)}");
                }
            }
        }

        return sb.ToString();
    }

    private string DecorateShortDesc(IPerceiver voyeur, string description)
    {
        if (string.IsNullOrEmpty(Title))
        {
            return description;
        }

        if (voyeur is ILanguagePerceiver lp && !lp.IsLiterate)
        {
            return description;
        }

        string titleSetting = Gameworld.GetStaticConfiguration("WrittenItemSDescStyle");
        switch (titleSetting.ToLowerInvariant())
        {
            case "title":
                return $"\"{Title}\"";
            case "desc":
                return description;
            case "desc+title":
                return $"{description} titled \"{Title.ColourBold(Telnet.White)}\"";
            default:
                throw new ApplicationException(
                    $"Invalid option for WrittenItemSDescStyle: '{titleSetting}'. Valid options are 'title', 'desc' and 'desc+title'");
        }
    }

    #endregion

    #region ITearable Implementation

    public IGameItem Tear(ICharacter actor, IEmote emote)
    {
        if (!CanTear(actor))
        {
            actor?.Send(WhyCannotTear(actor));
            return null;
        }

        IGameItem newItem = _prototype.PaperProto.CreateNew(actor);
        PaperSheetGameItemComponent newPaper = newItem?.GetItemType<PaperSheetGameItemComponent>();
        foreach (ICanBeRead readable in Readables)
        {
            newPaper?.Readables.Add(readable);
        }

        newPaper?.Changed = true;

        TornPages.Add(CurrentPage);
        bool newPageFound = false;
        for (int i = CurrentPage + 1; i <= _prototype.PageCount; i++)
        {
            if (!TornPages.Contains(i))
            {
                CurrentPage = i;
                CalculateWritings();
                newPageFound = true;
                break;
            }
        }

        if (!newPageFound)
        {
            for (int i = CurrentPage - 1; i > 0; i--)
            {
                if (!TornPages.Contains(i))
                {
                    CurrentPage = i;
                    CalculateWritings();
                    newPageFound = true;
                    break;
                }
            }
        }

        if (!newPageFound)
        {
            CurrentPage = 0;
        }

        Changed = true;

        if (actor != null)
        {
            actor.OutputHandler.Handle(
                new MixedEmoteOutput(new Emote($"@ tear|tears out a page from $0.", actor, Parent)).Append(emote));
        }
        else
        {
            Parent.OutputHandler.Handle(new EmoteOutput(new Emote($"A page is torn out of @.", Parent)));
        }

        if (newItem is not null)
        {
            newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
            newItem.Login();
        }

        return newItem;
    }

    public bool CanTear(ICharacter actor)
    {
        if (!IsOpen)
        {
            return false;
        }

        if (TornPages.Contains(CurrentPage))
        {
            return false;
        }

        if (CurrentPage < 1 || CurrentPage > _prototype.PageCount)
        {
            return false;
        }

        return true;
    }

    public string WhyCannotTear(ICharacter actor)
    {
        if (!IsOpen)
        {
            return $"You cannot tear a page out of {Parent.HowSeen(actor)} unless it is open.";
        }

        if (TornPages.Contains(CurrentPage))
        {
            return $"You cannot tear a page out of {Parent.HowSeen(actor)} that is already torn out.";
        }

        if (CurrentPage < 1 || CurrentPage > _prototype.PageCount)
        {
            return $"There are no pages left in {Parent.HowSeen(actor)} to tear out.";
        }

        throw new ApplicationException("Got to the end of BookGameItemComponent.WhyCannotTear");
    }

    #endregion
}
