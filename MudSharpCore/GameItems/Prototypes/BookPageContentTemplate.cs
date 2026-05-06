using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Form.Colour;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

internal sealed class BookPageContentTemplate : IPageReadableContentTemplate
{
	private readonly IFuturemud _gameworld;
	private long _languageId;
	private long _scriptId;
	private long _colourId;

	public BookPageContentTemplate(IFuturemud gameworld, int page, int order, string text, ILanguage language,
		IScript script, string provenance, IColour colour, WritingStyleDescriptors style)
	{
		_gameworld = gameworld;
		Page = page;
		Order = order;
		Text = text ?? string.Empty;
		_languageId = language?.Id ?? 0;
		_scriptId = script?.Id ?? 0;
		Provenance = provenance ?? string.Empty;
		_colourId = colour?.Id ?? 0;
		Style = style == WritingStyleDescriptors.None ? WritingStyleDescriptors.MachinePrinted : style;
	}

	public BookPageContentTemplate(IFuturemud gameworld, XElement root)
	{
		_gameworld = gameworld;
		Page = int.Parse(root.Attribute("Page")?.Value ?? "1");
		Order = int.Parse(root.Attribute("Order")?.Value ?? "1");
		_languageId = long.Parse(root.Element("Language")?.Value ?? "0");
		_scriptId = long.Parse(root.Element("Script")?.Value ?? "0");
		_colourId = long.Parse(root.Element("Colour")?.Value ?? "0");
		Style = (WritingStyleDescriptors)int.Parse(root.Element("Style")?.Value ?? ((int)WritingStyleDescriptors.MachinePrinted).ToString());
		Text = root.Element("Text")?.Value ?? string.Empty;
		Provenance = root.Element("Provenance")?.Value ?? string.Empty;
		LiteracySkill = double.Parse(root.Element("LiteracySkill")?.Value ?? "100.0");
		LanguageSkill = double.Parse(root.Element("LanguageSkill")?.Value ?? "100.0");
		HandwritingSkill = double.Parse(root.Element("HandwritingSkill")?.Value ?? "100.0");
		ForgerySkill = double.Parse(root.Element("ForgerySkill")?.Value ?? "0.0");
	}

	public int Page { get; set; }
	public int Order { get; set; }
	public string Text { get; set; }
	public string Provenance { get; set; }
	public WritingStyleDescriptors Style { get; set; }
	public double LiteracySkill { get; set; } = 100.0;
	public double LanguageSkill { get; set; } = 100.0;
	public double HandwritingSkill { get; set; } = 100.0;
	public double ForgerySkill { get; set; }
	public ILanguage Language => _gameworld.Languages.Get(_languageId);
	public IScript Script => _gameworld.Scripts.Get(_scriptId);
	public IColour Colour => _gameworld.Colours.Get(_colourId);

	public int DocumentLength => Script is null ? Text.RawTextLength() : (int)(Text.RawTextLength() * Script.DocumentLengthModifier);

	public ICanBeRead CreateReadable(IFuturemud gameworld)
	{
		var language = gameworld.Languages.Get(_languageId);
		var script = gameworld.Scripts.Get(_scriptId);
		var colour = gameworld.Colours.Get(_colourId);
		if (language is null || script is null || colour is null)
		{
			throw new InvalidOperationException("Cannot create printed book content with missing language, script, or colour.");
		}

		return new PrintedWriting(gameworld, Text, language, script, Provenance, Style, colour, LiteracySkill,
			LanguageSkill, HandwritingSkill, ForgerySkill);
	}

	public XElement SaveToXml()
	{
		return new XElement("Writing",
			new XAttribute("Type", "printed"),
			new XAttribute("Page", Page),
			new XAttribute("Order", Order),
			new XElement("Language", _languageId),
			new XElement("Script", _scriptId),
			new XElement("Colour", _colourId),
			new XElement("Style", (int)Style),
			new XElement("Provenance", new XCData(Provenance ?? string.Empty)),
			new XElement("Text", new XCData(Text ?? string.Empty)),
			new XElement("LiteracySkill", LiteracySkill),
			new XElement("LanguageSkill", LanguageSkill),
			new XElement("HandwritingSkill", HandwritingSkill),
			new XElement("ForgerySkill", ForgerySkill)
		);
	}

	public BookPageContentTemplate CloneForPage(int page, int order)
	{
		return new BookPageContentTemplate(_gameworld, page, order, Text, Language, Script, Provenance, Colour, Style)
		{
			LiteracySkill = LiteracySkill,
			LanguageSkill = LanguageSkill,
			HandwritingSkill = HandwritingSkill,
			ForgerySkill = ForgerySkill
		};
	}

	public static BookPageContentTemplate FromWriting(IFuturemud gameworld, int page, int order, IWriting writing)
	{
		var provenance = writing.Author?.Name ?? (writing.GetProperty("provenance")?.GetObject as string) ?? string.Empty;
		return new BookPageContentTemplate(gameworld, page, order, writing.ParseFor(null), writing.Language, writing.Script,
			provenance, writing.WritingColour, writing.Style)
		{
			LiteracySkill = writing.LiteracySkill,
			LanguageSkill = writing.LanguageSkill,
			HandwritingSkill = writing.HandwritingSkill,
			ForgerySkill = writing.ForgerySkill
		};
	}
}
