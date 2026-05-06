using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Form.Colour;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Communication.Language;

public class PrintedWriting : LateInitialisingItem, IWriting
{
	public PrintedWriting(
		IFuturemud gameworld,
		string text,
		ILanguage language,
		IScript script,
		string provenance,
		WritingStyleDescriptors style,
		IColour writingColour,
		double literacySkill = 100.0,
		double languageSkill = 100.0,
		double handwritingSkill = 100.0,
		double forgerySkill = 0.0)
	{
		Gameworld = gameworld;
		Text = text;
		Language = language;
		Script = script;
		Provenance = provenance;
		Style = style == WritingStyleDescriptors.None ? WritingStyleDescriptors.MachinePrinted : style;
		WritingColour = writingColour;
		LiteracySkill = literacySkill;
		LanguageSkill = languageSkill;
		HandwritingSkill = handwritingSkill;
		ForgerySkill = forgerySkill;
		DocumentLength = (int)(Text.RawTextLength() * Script.DocumentLengthModifier);
		Gameworld.SaveManager.AddInitialisation(this);
	}

	public PrintedWriting(Writing writing, IFuturemud gameworld)
	{
		_id = writing.Id;
		Gameworld = gameworld;
		var definition = XElement.Parse(writing.Definition);
		Text = definition.Element("Text")?.Value ?? string.Empty;
		Provenance = definition.Element("Provenance")?.Value ?? string.Empty;
		ForgerySkill = writing.ForgerySkill;
		HandwritingSkill = writing.HandwritingSkill;
		LiteracySkill = writing.LiteracySkill;
		LanguageSkill = writing.LanguageSkill;
		Language = gameworld.Languages.Get(writing.LanguageId);
		Script = gameworld.Scripts.Get(writing.ScriptId);
		Style = (WritingStyleDescriptors)writing.Style;
		DocumentLength = (int)(Text.RawTextLength() * Script.DocumentLengthModifier);
		WritingColour = gameworld.Colours.Get(writing.WritingColour);
		IdInitialised = true;
	}

	public PrintedWriting(PrintedWriting rhs)
	{
		Gameworld = rhs.Gameworld;
		Text = rhs.Text;
		Provenance = rhs.Provenance;
		DocumentLength = rhs.DocumentLength;
		ForgerySkill = rhs.ForgerySkill;
		HandwritingSkill = rhs.HandwritingSkill;
		LiteracySkill = rhs.LiteracySkill;
		LanguageSkill = rhs.LanguageSkill;
		Script = rhs.Script;
		Language = rhs.Language;
		Style = rhs.Style;
		WritingColour = rhs.WritingColour;
		Gameworld.SaveManager.AddInitialisation(this);
	}

	public string Text { get; set; }
	public string Provenance { get; set; }
	public WritingImplementType ImplementType => WritingImplementType.Printed;
	public IColour WritingColour { get; init; }
	public ICharacter Author => null;
	public ICharacter TrueAuthor => null;
	public int DocumentLength { get; set; }
	public double ForgerySkill { get; set; }
	public double HandwritingSkill { get; set; }
	public double LanguageSkill { get; set; }
	public override string FrameworkItemType => "Writing";
	public WritingStyleDescriptors Style { get; set; }
	public ILanguage Language { get; set; }
	public double LiteracySkill { get; set; }
	public IScript Script { get; set; }

	public IWriting Copy()
	{
		return new PrintedWriting(this);
	}

	public override object DatabaseInsert()
	{
		var dbitem = new Writing
		{
			ScriptId = Script.Id,
			LanguageId = Language.Id,
			AuthorId = null,
			TrueAuthorId = null,
			ForgerySkill = ForgerySkill,
			HandwritingSkill = HandwritingSkill,
			LanguageSkill = LanguageSkill,
			LiteracySkill = LiteracySkill,
			Style = (int)Style,
			WritingType = "printed",
			Definition = SaveDefinition(),
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
		dbitem.TrueAuthorId = null;
		dbitem.AuthorId = null;
		dbitem.ForgerySkill = ForgerySkill;
		dbitem.HandwritingSkill = HandwritingSkill;
		dbitem.LanguageSkill = LanguageSkill;
		dbitem.LiteracySkill = LiteracySkill;
		dbitem.Style = (int)Style;
		dbitem.Definition = SaveDefinition();
		dbitem.WritingColour = WritingColour?.Id ?? 0;
		dbitem.ImplementType = (int)ImplementType;
		Changed = false;
	}

	private string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Text", new XCData(Text ?? string.Empty)),
			new XElement("Provenance", new XCData(Provenance ?? string.Empty))
		).ToString();
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		_id = ((Writing)dbitem)?.Id ?? 0;
	}

	public string DescribeInLook(ICharacter voyeur)
	{
		var provenance = string.IsNullOrWhiteSpace(Provenance)
			? string.Empty
			: $" from {Provenance}";

		if (voyeur is null)
		{
			return $"{DocumentLength:N0} characters of {Language.Name} printed in {Style.Describe()} {Script.KnownScriptDescription.Strip_A_An()}{provenance}.".Colour(Telnet.BoldCyan);
		}

		if (!voyeur.IsLiterate)
		{
			return "A bunch of printed symbols that you cannot read.".Colour(Telnet.BoldCyan);
		}

		if (!voyeur.Knowledges.Contains(Script.ScriptKnowledge))
		{
			return $"{DocumentLength.ToString("N0", voyeur)} printed characters in {Script.UnknownScriptDescription.Strip_A_An()}{provenance}.".Colour(Telnet.BoldCyan);
		}

		return voyeur.HasTrait(Language.LinkedTrait)
			? $"{DocumentLength.ToString("N0", voyeur)} characters of {Language.Name} printed in {Style.Describe()} {Script.KnownScriptDescription.Strip_A_An()}{provenance}.".Colour(Telnet.BoldCyan)
			: $"{DocumentLength.ToString("N0", voyeur)} characters of an unknown language printed in {Style.Describe()} {Script.KnownScriptDescription.Strip_A_An()}{provenance}.".Colour(Telnet.BoldCyan);
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
			case "trueauthor":
				return new NullVariable(ProgVariableTypes.Character);
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
				return new BooleanVariable(false);
			case "printed":
				return new BooleanVariable(true);
			case "provenance":
				return new TextVariable(Provenance ?? string.Empty);
			default:
				throw new NotSupportedException();
		}
	}

	public ProgVariableTypes Type => ProgVariableTypes.Writing;

	public object GetObject => this;
	#endregion
}
