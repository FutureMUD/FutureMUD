using MudSharp.Database;
using MudSharp.Form.Colour;
using MudSharp.Framework.Save;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using Colour = MudSharp.Form.Colour.Colour;

namespace MudSharp.Communication.Language;

public class SimpleWriting : LateInitialisingItem, IWriting, ILazyLoadDuringIdleTime
{
    public SimpleWriting(IFuturemud gameworld, ICharacter author, string text, ICharacter trueAuthor = null)
    {
        Gameworld = gameworld;
        _authorId = author.Id;
        _author = new WeakReference<ICharacter>(author);
        _trueAuthorId = trueAuthor?.Id;
        _trueAuthor = trueAuthor is null ? null : new WeakReference<ICharacter>(trueAuthor);
        Text = text;
        Language = author.CurrentWritingLanguage;
        Script = author.CurrentScript;
        Style = author.WritingStyle;
        LiteracySkill = author.GetTrait(gameworld.Traits.Get(gameworld.GetStaticLong("LiteracySkillId")))?.Value ?? 0.0;
        HandwritingSkill =
            author.GetTrait(gameworld.Traits.Get(gameworld.GetStaticLong("HandwritingSkillId")))?.Value ?? 0.0;
        ForgerySkill = author.GetTrait(gameworld.Traits.Get(gameworld.GetStaticLong("ForgerySkillId")))?.Value ?? 0.0;
        LanguageSkill = author.GetTrait(Language.LinkedTrait)?.Value ?? 0.0;
        DocumentLength = (int)(Text.RawTextLength() * Script.DocumentLengthModifier);
        gameworld.SaveManager.AddInitialisation(this);
    }

    public SimpleWriting(Models.Writing writing, IFuturemud gameworld)
    {
        _id = writing.Id;
        Gameworld = gameworld;
        Text = writing.Definition;
        ForgerySkill = writing.ForgerySkill;
        HandwritingSkill = writing.HandwritingSkill;
        LiteracySkill = writing.LiteracySkill;
        LanguageSkill = writing.LanguageSkill;
        Language = gameworld.Languages.Get(writing.LanguageId);
        Script = gameworld.Scripts.Get(writing.ScriptId);
        Style = (WritingStyleDescriptors)writing.Style;
        _authorId = writing.AuthorId;
        _trueAuthorId = writing.TrueAuthorId;
        DocumentLength = (int)(Text.RawTextLength() * Script.DocumentLengthModifier);
        WritingColour = gameworld.Colours.Get(writing.WritingColour);
        ImplementType = (WritingImplementType)writing.ImplementType;
        IdInitialised = true;
    }

    public SimpleWriting(SimpleWriting rhs)
    {
        _authorId = rhs._authorId;
        _author = rhs._author;
        DocumentLength = rhs.DocumentLength;
        ForgerySkill = rhs.ForgerySkill;
        HandwritingSkill = rhs.HandwritingSkill;
        LiteracySkill = rhs.LiteracySkill;
        LanguageSkill = rhs.LanguageSkill;
        Script = rhs.Script;
        _trueAuthorId = rhs._trueAuthorId;
        _trueAuthor = rhs._trueAuthor;
        Language = rhs.Language;
        Gameworld = rhs.Gameworld;
        Text = rhs.Text;
        Style = rhs.Style;
        WritingColour = rhs.WritingColour;
        ImplementType = rhs.ImplementType;
        Gameworld.SaveManager.AddInitialisation(this);
    }

    public WritingImplementType ImplementType { get; init; }
    public IColour WritingColour { get; init; }

    private long? _authorId;
    private WeakReference<ICharacter> _author;

    public long? AuthorId => _authorId;
    public MudSharp.Character.ArchivedCharacterIdentity ArchivedAuthor =>
        HistoricalAuthorReference.Archived(Gameworld, _authorId, ref _author);

    public ICharacter Author
    {
        get => HistoricalAuthorReference.Live(Gameworld, _authorId, ref _author);
        init
        {
            _author = value is null ? null : new WeakReference<ICharacter>(value);
            _authorId = value?.Id;
            Changed = true;
        }
    }

    public int DocumentLength { get; set; }

    public double ForgerySkill { get; set; }

    public double HandwritingSkill { get; set; }

    public double LanguageSkill { get; set; }

    public override string FrameworkItemType => "Writing";

    public WritingStyleDescriptors Style { get; set; }

    public ILanguage Language { get; set; }

    public double LiteracySkill { get; set; }

    public IScript Script { get; set; }

    private long? _trueAuthorId;
    private WeakReference<ICharacter> _trueAuthor;

    public long? TrueAuthorId => _trueAuthorId;
    public MudSharp.Character.ArchivedCharacterIdentity ArchivedTrueAuthor =>
        HistoricalAuthorReference.Archived(Gameworld, _trueAuthorId, ref _trueAuthor);

    public ICharacter TrueAuthor
    {
        get => HistoricalAuthorReference.Live(Gameworld, _trueAuthorId, ref _trueAuthor);
        init
        {
            _trueAuthor = value is null ? null : new WeakReference<ICharacter>(value);
            _trueAuthorId = value?.Id;
            Changed = true;
        }
    }

    public string Text { get; set; }

    public IWriting Copy()
    {
        return new SimpleWriting(this);
    }

    public override object DatabaseInsert()
    {
        Writing dbitem = new()
        {
            ScriptId = Script.Id,
            LanguageId = Language.Id,
            TrueAuthorId = _trueAuthorId,
            AuthorId = _authorId,
            ForgerySkill = ForgerySkill,
            HandwritingSkill = HandwritingSkill,
            LanguageSkill = LanguageSkill,
            LiteracySkill = LiteracySkill,
            Style = (int)Style,
            WritingType = "simple",
            Definition = Text,
            WritingColour = WritingColour?.Id ?? 0,
            ImplementType = (int)ImplementType
        };
        FMDB.Context.Writings.Add(dbitem);
        return dbitem;
    }

    public string ParseFor(ICharacter voyeur)
    {
        return Text;
    }

    public override void Save()
    {
        Writing dbitem = FMDB.Context.Writings.Find(Id);
        dbitem.ScriptId = Script.Id;
        dbitem.LanguageId = Language.Id;
        dbitem.TrueAuthorId = _trueAuthorId;
        dbitem.AuthorId = _authorId;
        dbitem.ForgerySkill = ForgerySkill;
        dbitem.HandwritingSkill = HandwritingSkill;
        dbitem.LanguageSkill = LanguageSkill;
        dbitem.LiteracySkill = LiteracySkill;
        dbitem.Style = (int)Style;
        dbitem.Definition = Text;
        dbitem.WritingColour = WritingColour?.Id ?? 0;
        dbitem.ImplementType = (int)ImplementType;
        Changed = false;
    }

    public override void SetIDFromDatabase(object dbitem)
    {
        _id = ((Models.Writing)dbitem)?.Id ?? 0;
    }

    void ILazyLoadDuringIdleTime.DoLoad()
    {
        _ = Author;
        _ = TrueAuthor;
    }

    public string DescribeInLook(ICharacter voyeur)
    {
        if (voyeur is null)
        {
            return $"{DocumentLength:N0} characters of {Language.Name} written in {Style.Describe()} {Script.KnownScriptDescription.Strip_A_An()}.".Colour(Telnet.BoldCyan);
        }

        if (!voyeur.IsLiterate)
        {
            return "A bunch of squiggly non-sense.".Colour(Telnet.BoldCyan);
        }

        if (!voyeur.Knowledges.Contains(Script.ScriptKnowledge))
        {
            return
                $"{DocumentLength.ToString("N0", voyeur)} characters written in {Script.UnknownScriptDescription.Strip_A_An()}.".Colour(Telnet.BoldCyan);
        }

        return voyeur.HasTrait(Language.LinkedTrait)
            ? $"{DocumentLength.ToString("N0", voyeur)} characters of {Language.Name} written in {Style.Describe()} {Script.KnownScriptDescription.Strip_A_An()}.".Colour(Telnet.BoldCyan)
            : $"{DocumentLength.ToString("N0", voyeur)} characters of an unknown language written in {Style.Describe()} {Script.KnownScriptDescription.Strip_A_An()}.".Colour(Telnet.BoldCyan);
    }

    #region Implementation of IProgVariable
    public IProgVariable GetProperty(string property)
    {
        switch (property.ToLowerInvariant())
        {
            case "id":
                return new NumberVariable(Id);
            case "name":
                return new TextVariable(Name);
            case "author":
                return Author;
            case "trueauthor":
                return TrueAuthor;
            case "script":
                return Script;
            case "language":
                return Language;
            case "handwriting":
                return new NumberVariable(HandwritingSkill);
            case "literacy":
                return new NumberVariable(LiteracySkill);
            case "forgery":
                return new NumberVariable(ForgerySkill);
            case "languageskill":
                return new NumberVariable(LanguageSkill);
            case "text":
                return new TextVariable(string.Empty);
            case "simple":
                return new BooleanVariable(true);
            case "printed":
                return new BooleanVariable(false);
            case "provenance":
                return new TextVariable(string.Empty);
            default:
                throw new NotSupportedException();
        }
    }

    public ProgVariableTypes Type => ProgVariableTypes.Writing;

    public object GetObject => this;

    private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
    {
        return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "id", ProgVariableTypes.Number },
            { "name", ProgVariableTypes.Text },
            { "author", ProgVariableTypes.Character },
            { "trueauthor", ProgVariableTypes.Character },
            { "script", ProgVariableTypes.Script },
            { "language", ProgVariableTypes.Language },
            { "handwriting", ProgVariableTypes.Number },
            { "literacy", ProgVariableTypes.Number },
            { "forgery", ProgVariableTypes.Number },
            { "languageskill", ProgVariableTypes.Number },
            { "text", ProgVariableTypes.Text },
            { "simple", ProgVariableTypes.Boolean},
            { "printed", ProgVariableTypes.Boolean},
            { "provenance", ProgVariableTypes.Text}
        };
    }

    private static IReadOnlyDictionary<string, string> DotReferenceHelp()
    {
        return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "id", "The ID of the writing" },
            { "name", "The name of the writing" },
            { "author", "The author of the writing"},
            { "trueauthor", "The true author of the writing, if forged (otherwise null)"},
            { "script", "The script it is written in" },
            { "language", "The language it is written in" },
            { "handwriting", "The handwriting skill of the author" },
            { "literacy", "The literacy skill of the author" },
            { "forgery", "The forgery skill of the author" },
            { "languageskill", "The language skill of the author" },
            { "text", "Writing text is not exposed through FutureProg; use in-character read workflows instead" },
            { "simple", "True if simple writing, false is composite (drawing+writing) or printed" },
            { "printed", "True if this is printed writing without a character author" },
            { "provenance", "The publisher, source, or other provenance text for printed writing" }
        };
    }

    public static void RegisterFutureProgCompiler()
    {
        ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Writing, DotReferenceHandler(), DotReferenceHelp());
    }
    #endregion
}
