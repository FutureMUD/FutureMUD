using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Interfaces;
using System.Linq;
using MudSharp.Form.Colour;
using Colour = MudSharp.Form.Colour.Colour;
using MudSharp.FutureProg.Variables;
using MudSharp.FutureProg;
using System.Collections.Generic;
using System;

namespace MudSharp.Communication.Language;

public class SimpleWriting : LateInitialisingItem, IWriting, ILazyLoadDuringIdleTime
{
	public SimpleWriting(IFuturemud gameworld, ICharacter author, string text, ICharacter trueAuthor = null)
	{
		Gameworld = gameworld;
		_authorId = author.Id;
		_author = author;
		_trueAuthorId = trueAuthor?.Id;
		_trueAuthor = trueAuthor;
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
		Gameworld.SaveManager.AddLazyLoad(this);
	}

	public SimpleWriting(SimpleWriting rhs)
	{
		Author = rhs.Author;
		DocumentLength = rhs.DocumentLength;
		ForgerySkill = rhs.ForgerySkill;
		HandwritingSkill = rhs.HandwritingSkill;
		LiteracySkill = rhs.LiteracySkill;
		LanguageSkill = rhs.LanguageSkill;
		Script = rhs.Script;
		TrueAuthor = rhs.TrueAuthor;
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

	private long _authorId;
	private ICharacter _author;

	public ICharacter Author
	{
		get
		{
			if (_author == null && _authorId != 0)
			{
				_author = Gameworld.TryGetCharacter(_authorId, true);
			}

			return _author;
		}
		init
		{
			_author = value;
			_authorId = value?.Id ?? 0;
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
	private ICharacter _trueAuthor;

	public ICharacter TrueAuthor
	{
		get
		{
			if (_trueAuthor == null && (_trueAuthorId ?? 0) != 0)
			{
				_trueAuthor = Gameworld.TryGetCharacter(_trueAuthorId ?? 0L, true);
			}

			return _trueAuthor;
		}
		init
		{
			_trueAuthor = value;
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
		var dbitem = new Models.Writing
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
		var dbitem = FMDB.Context.Writings.Find(Id);
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
		if (_author == null && _authorId != 0)
		{
			_author = Gameworld.TryGetCharacter(_authorId, true);
		}

		if (_trueAuthor == null && (_trueAuthorId ?? 0) != 0)
		{
			_trueAuthor = Gameworld.TryGetCharacter(_trueAuthorId ?? 0L, true);
		}
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
				return new TextVariable(ParseFor(null));
			case "simple":
				return new BooleanVariable(true);
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
			{ "simple", ProgVariableTypes.Boolean}
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
			{ "text", "The parsed text of the writing" },
			{ "simple", "True if simple writing, false is composite (drawing+writing)"}
		};
	}

	public new static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Writing, DotReferenceHandler(), DotReferenceHelp());
	}
	#endregion
}