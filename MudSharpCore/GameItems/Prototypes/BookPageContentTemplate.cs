using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using System;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

internal sealed class BookPageContentTemplate : IPageReadableContentTemplate
{
	private readonly IFuturemud _gameworld;

	public BookPageContentTemplate(IFuturemud gameworld, int page, int order, ICanBeRead readable)
	{
		_gameworld = gameworld;
		Page = page;
		Order = order;
		Readable = readable;
		RegisterReadable(gameworld, readable);
	}

	public BookPageContentTemplate(IFuturemud gameworld, XElement root)
	{
		_gameworld = gameworld;
		Page = int.Parse(root.Attribute("Page")?.Value ?? "1");
		Order = int.Parse(root.Attribute("Order")?.Value ?? "1");
		if (long.TryParse(root.Attribute("Id")?.Value, out var id))
		{
			Readable = root.Name.LocalName.EqualTo("Drawing")
				? gameworld.Drawings.Get(id)
				: gameworld.Writings.Get(id);
			return;
		}

		WasLoadedFromLegacyXml = true;
		Readable = CreateLegacyPrintedWriting(gameworld, root);
		if (Readable is ILateInitialisingItem lateInitialisingItem)
		{
			gameworld.SaveManager.DirectInitialise(lateInitialisingItem);
		}

		RegisterReadable(gameworld, Readable);
	}

	public int Page { get; set; }
	public int Order { get; set; }
	public ICanBeRead? Readable { get; }
	public bool WasLoadedFromLegacyXml { get; }
	public int DocumentLength => Readable?.DocumentLength ?? 0;

	public ICanBeRead CreateReadable(IFuturemud gameworld)
	{
		return Readable ?? throw new InvalidOperationException("Cannot create book content for a missing readable reference.");
	}

	public XElement SaveToXml()
	{
		if (Readable is null)
		{
			return new XElement("Missing", new XAttribute("Page", Page), new XAttribute("Order", Order));
		}

		return new XElement(Readable is IWriting ? "Writing" : "Drawing",
			new XAttribute("Id", Readable.Id),
			new XAttribute("Page", Page),
			new XAttribute("Order", Order));
	}

	public BookPageContentTemplate CloneForPage(int page, int order)
	{
		return new BookPageContentTemplate(_gameworld, page, order, Readable!);
	}

	public static BookPageContentTemplate FromReadable(IFuturemud gameworld, int page, int order, ICanBeRead readable)
	{
		return new BookPageContentTemplate(gameworld, page, order, readable);
	}

	public static BookPageContentTemplate FromWriting(IFuturemud gameworld, int page, int order, IWriting writing)
	{
		return FromReadable(gameworld, page, order, writing);
	}

	private static ICanBeRead CreateLegacyPrintedWriting(IFuturemud gameworld, XElement root)
	{
		var languageId = long.Parse(root.Element("Language")?.Value ?? "0");
		var scriptId = long.Parse(root.Element("Script")?.Value ?? "0");
		var colourId = long.Parse(root.Element("Colour")?.Value ?? "0");
		var language = gameworld.Languages.Get(languageId);
		var script = gameworld.Scripts.Get(scriptId);
		var colour = gameworld.Colours.Get(colourId);
		if (language is null || script is null || colour is null)
		{
			throw new InvalidOperationException("Cannot convert legacy printed book content with missing language, script, or colour.");
		}

		var style = (WritingStyleDescriptors)int.Parse(root.Element("Style")?.Value ?? ((int)WritingStyleDescriptors.MachinePrinted).ToString());
		return new PrintedWriting(gameworld,
			root.Element("Text")?.Value ?? string.Empty,
			language,
			script,
			root.Element("Provenance")?.Value ?? string.Empty,
			style,
			colour,
			double.Parse(root.Element("LiteracySkill")?.Value ?? "100.0"),
			double.Parse(root.Element("LanguageSkill")?.Value ?? "100.0"),
			double.Parse(root.Element("HandwritingSkill")?.Value ?? "100.0"),
			double.Parse(root.Element("ForgerySkill")?.Value ?? "0.0"));
	}

	private static void RegisterReadable(IFuturemud gameworld, ICanBeRead? readable)
	{
		switch (readable)
		{
			case IWriting writing:
				if (writing is ILateInitialisingItem { IdHasBeenRegistered: false } || !gameworld.Writings.Has(writing.Id))
				{
					gameworld.Add(writing);
				}
				break;
			case IDrawing drawing:
				if (drawing is ILateInitialisingItem { IdHasBeenRegistered: false } || !gameworld.Drawings.Has(drawing.Id))
				{
					gameworld.Add(drawing);
				}
				break;
		}
	}
}

internal sealed class BookCollectionContentTemplate
{
	private readonly IFuturemud _gameworld;

	public BookCollectionContentTemplate(IFuturemud gameworld, long collectionId, int startPage)
	{
		_gameworld = gameworld;
		CollectionId = collectionId;
		StartPage = startPage;
	}

	public BookCollectionContentTemplate(IFuturemud gameworld, XElement root)
	{
		_gameworld = gameworld;
		CollectionId = long.Parse(root.Attribute("Id")?.Value ?? "0");
		StartPage = int.Parse(root.Attribute("StartPage")?.Value ?? "1");
	}

	public long CollectionId { get; }
	public int StartPage { get; }
	public IWritingCollection? Collection => _gameworld.WritingCollections.Get(CollectionId);

	public XElement SaveToXml()
	{
		return new XElement("Collection",
			new XAttribute("Id", CollectionId),
			new XAttribute("StartPage", StartPage));
	}
}