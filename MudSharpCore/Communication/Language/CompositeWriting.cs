using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Form.Colour;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Communication.Language;

public class CompositeWriting : LateInitialisingItem, IWriting, ILazyLoadDuringIdleTime
{
	public CompositeWriting(IFuturemud gameworld, ICharacter author, string text, string sdesc, DrawingSize size, ICharacter trueAuthor = null)
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
		ShortDescription = sdesc;
		DrawingSize = size;
		DrawingSkill = author.GetTrait(gameworld.Traits.Get(gameworld.GetStaticLong("DrawingTraitId")))?.Value ?? 0.0;
		gameworld.SaveManager.AddInitialisation(this);
	}

	public CompositeWriting(Models.Writing writing, IFuturemud gameworld)
	{
		_id = writing.Id;
		Gameworld = gameworld;
		var definition = XElement.Parse(writing.Definition);
		Text = definition.Element("Text").Value;
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
		DrawingSize = (DrawingSize)int.Parse(definition.Element("DrawingSize").Value);
		DrawingSkill = double.Parse(definition.Element("DrawingSkill").Value);
		ShortDescription = definition.Element("ShortDescription").Value;
		IdInitialised = true;
		Gameworld.SaveManager.AddLazyLoad(this);
	}

	public CompositeWriting(CompositeWriting rhs)
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
		DrawingSize = rhs.DrawingSize;
		DrawingSkill = rhs.DrawingSkill;
		ShortDescription = rhs.ShortDescription;
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

	public DrawingSize DrawingSize { get; set; }
	public string ShortDescription { get; set; }
	public double DrawingSkill { get; set; }

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
		return new CompositeWriting(this);
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
			WritingType = "composite",
			Definition = new XElement("Definition",
				new XElement("Text", new XCData(Text)),
				new XElement("ShortDescription", new XCData(ShortDescription)),
				new XElement("DrawingSkill", DrawingSkill),
				new XElement("DrawingSize", (int)DrawingSize)
			).ToString(),
			WritingColour = WritingColour?.Id ?? 0,
			ImplementType = (int)ImplementType
		};
		FMDB.Context.Writings.Add(dbitem);
		return dbitem;
	}

	private readonly Regex LanguageRegex = new(@"""[^""]+""", RegexOptions.IgnoreCase);

	public string ParseFor(ICharacter voyeur)
	{
		LanguageRegex.Replace(Text, m =>
		{
			if (!voyeur.IsLiterate)
			{
				return "***a bunch of squiggly non-sense***";
			}

			if (!voyeur.Scripts.Contains(Script))
			{
				return $"***something written in {Script.UnknownScriptDescription.Strip_A_An()}***";
			}

			if (!voyeur.Languages.Contains(Language))
			{

			}
			return "";
		});
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
		dbitem.Definition = new XElement("Definition",
			new XElement("Text", new XCData(Text)),
			new XElement("ShortDescription", new XCData(ShortDescription)),
			new XElement("DrawingSkill", DrawingSkill),
			new XElement("DrawingSize", (int)DrawingSize)
		).ToString();
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
		if (!voyeur.IsLiterate)
		{
			return "A bunch of squiggly non-sense.";
		}

		if (!voyeur.Knowledges.Contains(Script.ScriptKnowledge))
		{
			return
				$"{DocumentLength.ToString("N0", voyeur)} characters written in {Script.UnknownScriptDescription.Strip_A_An()}.";
		}

		return voyeur.HasTrait(Language.LinkedTrait)
			? $"{DocumentLength.ToString("N0", voyeur)} characters of {Language.Name} written in {Style.Describe()} {Script.KnownScriptDescription.Strip_A_An()}."
			: $"{DocumentLength.ToString("N0", voyeur)} characters of an unknown language written in {Style.Describe()} {Script.KnownScriptDescription.Strip_A_An()}.";
	}
}