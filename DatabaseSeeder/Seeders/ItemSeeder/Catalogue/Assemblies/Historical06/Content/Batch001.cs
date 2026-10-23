#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical06Stock
{
	public static CatalogueItemDefinition MedievalWritingBlankPaperSealTags() => new()
	{
		Metadata = new("medieval_writing_blank_paper_seal_tags", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1087", []),
		Noun = "tags",
		ShortDescription = "a bundle of blank seal tags",
		LongDescription = null,
		FullDescription = "This bundle of blank seal tags contains a stack of small blank labels cut from writing stock. The pieces are narrow and light, with enough surface for a title, seal note, tally mark, or short identifying hand. They are meant to be tied to scrolls, packets, bundles, or stored records.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Stack_Number"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingParchmentRoll() => new()
	{
		Metadata = new("medieval_writing_parchment_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1116", []),
		Noun = "roll",
		ShortDescription = "a parchment writing roll",
		LongDescription = null,
		FullDescription = "This parchment writing roll is made from prepared parchment sheets joined or trimmed into a long roll. The surface is tougher and springier than paper, with pale scraped grain visible beneath the writing face. It is meant for charters, legal records, inventories, or other long formal documents.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Roll_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingCharterRoll() => new()
	{
		Metadata = new("medieval_writing_charter_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1147", []),
		Noun = "roll",
		ShortDescription = "a blank charter roll",
		LongDescription = null,
		FullDescription = "This blank charter roll is a long prepared writing roll wound into a firm cylinder. The outer turn protects the writing surface and leaves room for a tie, label, or wrapper. It can be unrolled gradually for reading, copying, or record keeping.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 140.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Roll_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingAccountRoll() => new()
	{
		Metadata = new("medieval_writing_account_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1178", []),
		Noun = "roll",
		ShortDescription = "an account parchment roll",
		LongDescription = null,
		FullDescription = "This account parchment roll is a long prepared writing roll wound into a firm cylinder. The outer turn protects the writing surface and leaves room for a tie, label, or wrapper. It can be unrolled gradually for reading, copying, or record keeping.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 130.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Roll_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingCourtRoll() => new()
	{
		Metadata = new("medieval_writing_court_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1209", []),
		Noun = "roll",
		ShortDescription = "a court record roll",
		LongDescription = null,
		FullDescription = "This court record roll is a long prepared writing roll wound into a firm cylinder. The outer turn protects the writing surface and leaves room for a tie, label, or wrapper. It can be unrolled gradually for reading, copying, or record keeping.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 150.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Roll_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingEstateRoll() => new()
	{
		Metadata = new("medieval_writing_estate_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1240", []),
		Noun = "roll",
		ShortDescription = "an estate account roll",
		LongDescription = null,
		FullDescription = "This estate account roll is a long prepared writing roll wound into a firm cylinder. The outer turn protects the writing surface and leaves room for a tie, label, or wrapper. It can be unrolled gradually for reading, copying, or record keeping.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 145.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Roll_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingTaxRoll() => new()
	{
		Metadata = new("medieval_writing_tax_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1271", []),
		Noun = "roll",
		ShortDescription = "a tax account roll",
		LongDescription = null,
		FullDescription = "This tax account roll is a long prepared writing roll wound into a firm cylinder. The outer turn protects the writing surface and leaves room for a tie, label, or wrapper. It can be unrolled gradually for reading, copying, or record keeping.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 135.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Roll_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPapyrusScroll() => new()
	{
		Metadata = new("medieval_writing_papyrus_scroll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1302", []),
		Noun = "scroll",
		ShortDescription = "a simple papyrus scroll",
		LongDescription = null,
		FullDescription = "This simple papyrus scroll is built from joined papyrus sheets rolled into a compact cylinder. The pale writing surface has a faint plant-fibre grid and enough stiffness to unroll without collapsing. It is suited to accounts, letters, scholarly notes, or Mediterranean-style record rolls.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 65.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "papyrus",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Papyrus_Scroll_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLongPapyrusScroll() => new()
	{
		Metadata = new("medieval_writing_long_papyrus_scroll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1333", []),
		Noun = "scroll",
		ShortDescription = "a long papyrus scroll",
		LongDescription = null,
		FullDescription = "This long papyrus scroll is built from joined papyrus sheets rolled into a compact cylinder. The pale writing surface has a faint plant-fibre grid and enough stiffness to unroll without collapsing. It is suited to accounts, letters, scholarly notes, or Mediterranean-style record rolls.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "papyrus",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Papyrus_Scroll_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPapyrusArchiveRoll() => new()
	{
		Metadata = new("medieval_writing_papyrus_archive_roll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1364", []),
		Noun = "roll",
		ShortDescription = "a papyrus archive roll",
		LongDescription = null,
		FullDescription = "This papyrus archive roll is built from joined papyrus sheets rolled into a compact cylinder. The pale writing surface has a faint plant-fibre grid and enough stiffness to unroll without collapsing. It is suited to accounts, letters, scholarly notes, or Mediterranean-style record rolls.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "papyrus",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Papyrus_Scroll_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingRagPaperScroll() => new()
	{
		Metadata = new("medieval_writing_rag_paper_scroll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1395", []),
		Noun = "scroll",
		ShortDescription = "a rag-paper scroll",
		LongDescription = null,
		FullDescription = "This rag-paper scroll is a long prepared writing roll wound into a firm cylinder. The outer turn protects the writing surface and leaves room for a tie, label, or wrapper. It can be unrolled gradually for reading, copying, or record keeping.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Scroll_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingEastAsianHandscroll() => new()
	{
		Metadata = new("medieval_writing_east_asian_handscroll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1426", []),
		Noun = "scroll",
		ShortDescription = "an East Asian handscroll",
		LongDescription = null,
		FullDescription = "This East Asian handscroll is a long paper roll arranged for brush-written or copied text. The paper is joined in a smooth run and wound around a neat core so it can be opened section by section. Its outer turn leaves space for a label, wrapper, or protective tie.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 80.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Paper_Scroll_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingEastAsianPoetryScroll() => new()
	{
		Metadata = new("medieval_writing_east_asian_poetry_scroll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1457", []),
		Noun = "scroll",
		ShortDescription = "a blank poetry handscroll",
		LongDescription = null,
		FullDescription = "This blank poetry handscroll is a long paper roll arranged for brush-written or copied text. The paper is joined in a smooth run and wound around a neat core so it can be opened section by section. Its outer turn leaves space for a label, wrapper, or protective tie.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 70.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Paper_Scroll_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingEastAsianEdictScroll() => new()
	{
		Metadata = new("medieval_writing_east_asian_edict_scroll", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1488", []),
		Noun = "scroll",
		ShortDescription = "a formal paper edict scroll",
		LongDescription = null,
		FullDescription = "This formal paper edict scroll is a long paper roll arranged for brush-written or copied text. The paper is joined in a smooth run and wound around a neat core so it can be opened section by section. Its outer turn leaves space for a label, wrapper, or protective tie.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 95.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Paper_Scroll_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSmallParchmentCodex() => new()
	{
		Metadata = new("medieval_writing_small_parchment_codex", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1519", []),
		Noun = "codex",
		ShortDescription = "a small parchment codex",
		LongDescription = null,
		FullDescription = "This small parchment codex is a bound parchment manuscript with folded leaves sewn between plain covers or boards. The pages have a firm pale surface and enough stiffness to hold their shape as the book opens. It is suited to legal, scholarly, devotional, or administrative writing that needs a durable codex form.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Codex_20_Page"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPlainParchmentCodex() => new()
	{
		Metadata = new("medieval_writing_plain_parchment_codex", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1550", []),
		Noun = "codex",
		ShortDescription = "a plain parchment codex",
		LongDescription = null,
		FullDescription = "This plain parchment codex is a bound parchment manuscript with folded leaves sewn between plain covers or boards. The pages have a firm pale surface and enough stiffness to hold their shape as the book opens. It is suited to legal, scholarly, devotional, or administrative writing that needs a durable codex form.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Codex_40_Page"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLargeParchmentCodex() => new()
	{
		Metadata = new("medieval_writing_large_parchment_codex", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1581", []),
		Noun = "codex",
		ShortDescription = "a large parchment codex",
		LongDescription = null,
		FullDescription = "This large parchment codex is a bound parchment manuscript with folded leaves sewn between plain covers or boards. The pages have a firm pale surface and enough stiffness to hold their shape as the book opens. It is suited to legal, scholarly, devotional, or administrative writing that needs a durable codex form.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1100.0,
		Cost = 170.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Codex_90_Page"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPrayerCodex() => new()
	{
		Metadata = new("medieval_writing_prayer_codex", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1612", []),
		Noun = "codex",
		ShortDescription = "a small prayer codex",
		LongDescription = null,
		FullDescription = "This small prayer codex is a compact bound book with gathered leaves and a serviceable cover. The spine and boards are made to protect the writing surfaces while still allowing the leaves to turn cleanly. It is a practical manuscript form for records, lessons, copied texts, or library storage.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 380.0,
		Cost = 72.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Codex_20_Page"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingScholasticCodex() => new()
	{
		Metadata = new("medieval_writing_scholastic_codex", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1643", []),
		Noun = "codex",
		ShortDescription = "a scholastic parchment codex",
		LongDescription = null,
		FullDescription = "This scholastic parchment codex is a compact bound book with gathered leaves and a serviceable cover. The spine and boards are made to protect the writing surfaces while still allowing the leaves to turn cleanly. It is a practical manuscript form for records, lessons, copied texts, or library storage.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1250.0,
		Cost = 190.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Codex_90_Page"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLiturgicalCodex() => new()
	{
		Metadata = new("medieval_writing_liturgical_codex", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1674", []),
		Noun = "codex",
		ShortDescription = "a large liturgical codex",
		LongDescription = null,
		FullDescription = "This large liturgical codex is a compact bound book with gathered leaves and a serviceable cover. The spine and boards are made to protect the writing surfaces while still allowing the leaves to turn cleanly. It is a practical manuscript form for records, lessons, copied texts, or library storage.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1600.0,
		Cost = 260.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Codex_90_Page"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingMonasticRuleCodex() => new()
	{
		Metadata = new("medieval_writing_monastic_rule_codex", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1705", []),
		Noun = "codex",
		ShortDescription = "a monastic rule codex",
		LongDescription = null,
		FullDescription = "This monastic rule codex is a compact bound book with gathered leaves and a serviceable cover. The spine and boards are made to protect the writing surfaces while still allowing the leaves to turn cleanly. It is a practical manuscript form for records, lessons, copied texts, or library storage.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 580.0,
		Cost = 95.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Codex_40_Page"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingMerchantAccountLedger() => new()
	{
		Metadata = new("medieval_writing_merchant_account_ledger", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1736", []),
		Noun = "ledger",
		ShortDescription = "a merchant account ledger",
		LongDescription = null,
		FullDescription = "This merchant account ledger is a bound account book with sturdy covers and orderly blank leaves inside. The pages are meant to open flat enough for columns, tallies, names, and repeated entries. Its construction is practical rather than ornamental, suitable for clerks, stewards, merchants, or estate records.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1150.0,
		Cost = 140.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Account_Ledger_90_Page"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingEstateAccountLedger() => new()
	{
		Metadata = new("medieval_writing_estate_account_ledger", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1767", []),
		Noun = "ledger",
		ShortDescription = "an estate account ledger",
		LongDescription = null,
		FullDescription = "This estate account ledger is a bound account book with sturdy covers and orderly blank leaves inside. The pages are meant to open flat enough for columns, tallies, names, and repeated entries. Its construction is practical rather than ornamental, suitable for clerks, stewards, merchants, or estate records.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1250.0,
		Cost = 150.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Account_Ledger_90_Page"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingTollLedger() => new()
	{
		Metadata = new("medieval_writing_toll_ledger", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1798", []),
		Noun = "ledger",
		ShortDescription = "a toll account ledger",
		LongDescription = null,
		FullDescription = "This toll account ledger is a bound account book with sturdy covers and orderly blank leaves inside. The pages are meant to open flat enough for columns, tallies, names, and repeated entries. Its construction is practical rather than ornamental, suitable for clerks, stewards, merchants, or estate records.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Account_Ledger_90_Page"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingRagPaperCodex() => new()
	{
		Metadata = new("medieval_writing_rag_paper_codex", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1829", []),
		Noun = "codex",
		ShortDescription = "a rag-paper codex",
		LongDescription = null,
		FullDescription = "This rag-paper codex is a small bound paper manuscript with gathered rag-paper leaves and plain protective covers. The pages are lighter and more flexible than parchment, with trimmed edges and a practical sewn spine. It is suited to school texts, accounts, copied notes, or portable reading.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 44.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Codex_40_Page"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPaperScholarlyCodex() => new()
	{
		Metadata = new("medieval_writing_paper_scholarly_codex", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1860", []),
		Noun = "codex",
		ShortDescription = "a paper scholarly codex",
		LongDescription = null,
		FullDescription = "This paper scholarly codex is a compact bound book with gathered leaves and a serviceable cover. The spine and boards are made to protect the writing surfaces while still allowing the leaves to turn cleanly. It is a practical manuscript form for records, lessons, copied texts, or library storage.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 420.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Codex_40_Page"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPaperDevotionalCodex() => new()
	{
		Metadata = new("medieval_writing_paper_devotional_codex", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1891", []),
		Noun = "codex",
		ShortDescription = "a paper devotional codex",
		LongDescription = null,
		FullDescription = "This paper devotional codex is a compact bound book with gathered leaves and a serviceable cover. The spine and boards are made to protect the writing surfaces while still allowing the leaves to turn cleanly. It is a practical manuscript form for records, lessons, copied texts, or library storage.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 430.0,
		Cost = 78.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Codex_40_Page"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingEastAsianStitchedBook() => new()
	{
		Metadata = new("medieval_writing_east_asian_stitched_book", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1922", []),
		Noun = "book",
		ShortDescription = "an East Asian stitched book",
		LongDescription = null,
		FullDescription = "This East Asian stitched book is a soft stitched book made from light paper leaves gathered along one edge. The cover is flexible and the sewing is visible, leaving the pages able to turn in thin, even groups. It is suited to copied text, accounts, poetry, teaching notes, or brush-written manuscript work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 240.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Stitched_Book"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingEastAsianSutraBook() => new()
	{
		Metadata = new("medieval_writing_east_asian_sutra_book", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1953", []),
		Noun = "book",
		ShortDescription = "a stitched sutra-copying book",
		LongDescription = null,
		FullDescription = "This stitched sutra-copying book is a soft stitched book made from light paper leaves gathered along one edge. The cover is flexible and the sewing is visible, leaving the pages able to turn in thin, even groups. It is suited to copied text, accounts, poetry, teaching notes, or brush-written manuscript work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 280.0,
		Cost = 64.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Stitched_Book"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingEastAsianAccountBook() => new()
	{
		Metadata = new("medieval_writing_east_asian_account_book", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:1984", []),
		Noun = "book",
		ShortDescription = "a stitched account book",
		LongDescription = null,
		FullDescription = "This stitched account book is a soft stitched book made from light paper leaves gathered along one edge. The cover is flexible and the sewing is visible, leaving the pages able to turn in thin, even groups. It is suited to copied text, accounts, poetry, teaching notes, or brush-written manuscript work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 250.0,
		Cost = 45.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Stitched_Book"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPalmLeafManuscriptBundle() => new()
	{
		Metadata = new("medieval_writing_palm_leaf_manuscript_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2015", []),
		Noun = "bundle",
		ShortDescription = "a palm-leaf manuscript bundle",
		LongDescription = null,
		FullDescription = "This palm-leaf manuscript bundle is a wrapped group of manuscript leaves or papers kept together as one packet. The contents are stacked square and protected by an outer wrap or tie rather than hard covers. It is useful for drafts, copied sections, travelling notes, or records awaiting binding.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leaf",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Palm_Leaf_Manuscript_Bundle"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingTemplePalmLeafBundle() => new()
	{
		Metadata = new("medieval_writing_temple_palm_leaf_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2046", []),
		Noun = "bundle",
		ShortDescription = "a temple palm-leaf bundle",
		LongDescription = null,
		FullDescription = "This temple palm-leaf bundle is a wrapped group of manuscript leaves or papers kept together as one packet. The contents are stacked square and protected by an outer wrap or tie rather than hard covers. It is useful for drafts, copied sections, travelling notes, or records awaiting binding.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 460.0,
		Cost = 60.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leaf",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Palm_Leaf_Manuscript_Bundle"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingScholarlyPalmLeafBundle() => new()
	{
		Metadata = new("medieval_writing_scholarly_palm_leaf_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2077", []),
		Noun = "bundle",
		ShortDescription = "a scholarly palm-leaf bundle",
		LongDescription = null,
		FullDescription = "This scholarly palm-leaf bundle is a wrapped group of manuscript leaves or papers kept together as one packet. The contents are stacked square and protected by an outer wrap or tie rather than hard covers. It is useful for drafts, copied sections, travelling notes, or records awaiting binding.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 440.0,
		Cost = 55.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leaf",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Palm_Leaf_Manuscript_Bundle"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingWrappedParchmentManuscriptBundle() => new()
	{
		Metadata = new("medieval_writing_wrapped_parchment_manuscript_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2108", []),
		Noun = "bundle",
		ShortDescription = "a wrapped parchment manuscript bundle",
		LongDescription = null,
		FullDescription = "This wrapped parchment manuscript bundle is a wrapped group of manuscript leaves or papers kept together as one packet. The contents are stacked square and protected by an outer wrap or tie rather than hard covers. It is useful for drafts, copied sections, travelling notes, or records awaiting binding.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingWrappedPaperManuscriptBundle() => new()
	{
		Metadata = new("medieval_writing_wrapped_paper_manuscript_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2136", []),
		Noun = "bundle",
		ShortDescription = "a wrapped paper manuscript bundle",
		LongDescription = null,
		FullDescription = "This wrapped paper manuscript bundle is a wrapped group of manuscript leaves or papers kept together as one packet. The contents are stacked square and protected by an outer wrap or tie rather than hard covers. It is useful for drafts, copied sections, travelling notes, or records awaiting binding.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 380.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBlankCommentaryGathering() => new()
	{
		Metadata = new("medieval_writing_blank_commentary_gathering", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2164", []),
		Noun = "gathering",
		ShortDescription = "a blank commentary gathering",
		LongDescription = null,
		FullDescription = "This blank commentary gathering is a prepared gathering of folded leaves arranged for later binding. The nesting is neat, with the folds aligned to form a temporary spine. It can be sewn into a codex or used as a loose working section for commentary and copied text.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 95.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = [
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingWaxTablet() => new()
	{
		Metadata = new("medieval_writing_wax_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2192", []),
		Noun = "tablet",
		ShortDescription = "a wax writing tablet",
		LongDescription = null,
		FullDescription = "This wax writing tablet is a wooden board recessed and filled with smooth beeswax for stylus writing. The wax face is dark enough for scratches to show clearly and soft enough to be scraped back for reuse. Its compact form suits school exercises, accounts, lists, and short messages.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Wax Tablets"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wax_Tablet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSmallWaxTablet() => new()
	{
		Metadata = new("medieval_writing_small_wax_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2222", []),
		Noun = "tablet",
		ShortDescription = "a small wax tablet",
		LongDescription = null,
		FullDescription = "This small wax tablet is a wooden board recessed and filled with smooth beeswax for stylus writing. The wax face is dark enough for scratches to show clearly and soft enough to be scraped back for reuse. Its compact form suits school exercises, accounts, lists, and short messages.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 240.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Wax Tablets"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wax_Tablet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingStudentWaxTablet() => new()
	{
		Metadata = new("medieval_writing_student_wax_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2252", []),
		Noun = "tablet",
		ShortDescription = "a student wax tablet",
		LongDescription = null,
		FullDescription = "This student wax tablet is a wooden board recessed and filled with smooth beeswax for stylus writing. The wax face is dark enough for scratches to show clearly and soft enough to be scraped back for reuse. Its compact form suits school exercises, accounts, lists, and short messages.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 360.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Wax Tablets"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wax_Tablet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingAccountWaxTablet() => new()
	{
		Metadata = new("medieval_writing_account_wax_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2282", []),
		Noun = "tablet",
		ShortDescription = "an account wax tablet",
		LongDescription = null,
		FullDescription = "This account wax tablet is a wooden board recessed and filled with smooth beeswax for stylus writing. The wax face is dark enough for scratches to show clearly and soft enough to be scraped back for reuse. Its compact form suits school exercises, accounts, lists, and short messages.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 450.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Wax Tablets"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wax_Tablet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingWaxDiptych() => new()
	{
		Metadata = new("medieval_writing_wax_diptych", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2312", []),
		Noun = "diptych",
		ShortDescription = "a wax tablet diptych",
		LongDescription = null,
		FullDescription = "This wax tablet diptych is made from paired waxed boards joined so their writing faces close together. The recessed panels hold darkened wax smoothed flat enough to take stylus marks. It is portable and reusable, suited to accounts, exercises, letters, or temporary records.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 25.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Wax Tablets"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wax_Diptych_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingWaxTriptych() => new()
	{
		Metadata = new("medieval_writing_wax_triptych", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2342", []),
		Noun = "triptych",
		ShortDescription = "a wax tablet triptych",
		LongDescription = null,
		FullDescription = "This wax tablet triptych has three waxed leaves bound together into a compact reusable writing set. Each recessed panel is filled with smoothed beeswax, and the outer boards protect the inner writing faces when closed. It is useful for longer notes, account lists, or formal temporary memoranda.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1250.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Wax Tablets"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wax_Triptych_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBronzeFramedWaxTablet() => new()
	{
		Metadata = new("medieval_writing_bronze_framed_wax_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2372", []),
		Noun = "tablet",
		ShortDescription = "a bronze-framed wax tablet",
		LongDescription = null,
		FullDescription = "This bronze-framed wax tablet sets a smooth wax writing face within a sturdier metal-trimmed frame. The dark wax is levelled into a shallow recess and bordered so a stylus can work close to the edges. It reads as a more durable tablet for officials, teachers, or frequent record work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 620.0,
		Cost = 35.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "beeswax",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Wax Tablets"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wax_Tablet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingWoodenWritingTablet() => new()
	{
		Metadata = new("medieval_writing_wooden_writing_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2402", []),
		Noun = "tablet",
		ShortDescription = "a wooden writing tablet",
		LongDescription = null,
		FullDescription = "This wooden writing tablet is a flat wooden surface prepared for ink, charcoal, or shallow incised marks. The face is planed smoother than the back, with edges kept thick enough to survive repeated handling. It is useful for practice writing, tallying, rosters, or reusable working notes.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 300.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wooden_Tablet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingCedarWritingTablet() => new()
	{
		Metadata = new("medieval_writing_cedar_writing_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2432", []),
		Noun = "tablet",
		ShortDescription = "a cedar writing tablet",
		LongDescription = null,
		FullDescription = "This cedar writing tablet is a flat cedar surface prepared for ink, charcoal, or shallow incised marks. The face is planed smoother than the back, with edges kept thick enough to survive repeated handling. It is useful for practice writing, tallying, rosters, or reusable working notes.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 280.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wooden_Tablet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingWoodenPracticeTablet() => new()
	{
		Metadata = new("medieval_writing_wooden_practice_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2462", []),
		Noun = "tablet",
		ShortDescription = "a wooden practice tablet",
		LongDescription = null,
		FullDescription = "This wooden practice tablet is a flat wooden surface prepared for ink, charcoal, or shallow incised marks. The face is planed smoother than the back, with edges kept thick enough to survive repeated handling. It is useful for practice writing, tallying, rosters, or reusable working notes.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 260.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Practice_Board_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingWoodenAccountBoard() => new()
	{
		Metadata = new("medieval_writing_wooden_account_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2492", []),
		Noun = "board",
		ShortDescription = "a wooden account board",
		LongDescription = null,
		FullDescription = "This wooden account board is a flat wooden surface prepared for ink, charcoal, or shallow incised marks. The face is planed smoother than the back, with edges kept thick enough to survive repeated handling. It is useful for practice writing, tallying, rosters, or reusable working notes.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Practice_Board_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSchoolPracticeBoard() => new()
	{
		Metadata = new("medieval_writing_school_practice_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2522", []),
		Noun = "board",
		ShortDescription = "a school practice board",
		LongDescription = null,
		FullDescription = "This school practice board is a flat wooden surface prepared for ink, charcoal, or shallow incised marks. The face is planed smoother than the back, with edges kept thick enough to survive repeated handling. It is useful for practice writing, tallying, rosters, or reusable working notes.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 480.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Practice_Board_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSlateTablet() => new()
	{
		Metadata = new("medieval_writing_slate_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2552", []),
		Noun = "tablet",
		ShortDescription = "a slate writing tablet",
		LongDescription = null,
		FullDescription = "This slate writing tablet is a flat slate writing surface with a dark, smooth face and rubbed edges. The stone is heavy for its size but durable, with enough texture to show pale scratches or incised marks. It is suited to school work, rosters, calculations, or temporary notes.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "slate",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Slate_Tablet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSchoolSlateTablet() => new()
	{
		Metadata = new("medieval_writing_school_slate_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2582", []),
		Noun = "tablet",
		ShortDescription = "a school slate tablet",
		LongDescription = null,
		FullDescription = "This school slate tablet is a flat slate writing surface with a dark, smooth face and rubbed edges. The stone is heavy for its size but durable, with enough texture to show pale scratches or incised marks. It is suited to school work, rosters, calculations, or temporary notes.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 500.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "slate",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Slate_Tablet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSlateRosterTablet() => new()
	{
		Metadata = new("medieval_writing_slate_roster_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2612", []),
		Noun = "tablet",
		ShortDescription = "a slate roster tablet",
		LongDescription = null,
		FullDescription = "This slate roster tablet is a flat slate writing surface with a dark, smooth face and rubbed edges. The stone is heavy for its size but durable, with enough texture to show pale scratches or incised marks. It is suited to school work, rosters, calculations, or temporary notes.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "slate",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Slate_Tablet_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBambooPracticePanel() => new()
	{
		Metadata = new("medieval_writing_bamboo_practice_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2642", []),
		Noun = "panel",
		ShortDescription = "a bamboo practice panel",
		LongDescription = null,
		FullDescription = "This bamboo practice panel is made from smooth bamboo strips set as a small practice surface. The face is pale and lightly ridged, with enough spring to take brush or stylus marks. It is a light teaching or copying aid for repeated short exercises.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Bamboo_Slip_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingReusableTallyBoard() => new()
	{
		Metadata = new("medieval_writing_reusable_tally_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2672", []),
		Noun = "board",
		ShortDescription = "a reusable tally board",
		LongDescription = null,
		FullDescription = "This reusable tally board is a flat wooden surface prepared for ink, charcoal, or shallow incised marks. The face is planed smoother than the back, with edges kept thick enough to survive repeated handling. It is useful for practice writing, tallying, rosters, or reusable working notes.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Practice_Board_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingMerchantRosterBoard() => new()
	{
		Metadata = new("medieval_writing_merchant_roster_board", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2702", []),
		Noun = "board",
		ShortDescription = "a merchant roster board",
		LongDescription = null,
		FullDescription = "This merchant roster board is a flat wooden surface prepared for ink, charcoal, or shallow incised marks. The face is planed smoother than the back, with edges kept thick enough to survive repeated handling. It is useful for practice writing, tallying, rosters, or reusable working notes.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 780.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Practice_Board_Surface"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingClayDocumentEnvelope() => new()
	{
		Metadata = new("medieval_writing_clay_document_envelope", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2732", []),
		Noun = "envelope",
		ShortDescription = "a clay document envelope",
		LongDescription = null,
		FullDescription = "This clay document envelope is shaped from clay as a protective shell for a smaller document or tablet. The outer face is smoothed for marks and left ready to take a clay or wax seal at the closed edge. It is meant to enclose, authenticate, and preserve a written record.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 720.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "clay",
		Tags = [
				"Functions / Writing Goods",
				"Market / Writing Materials / Clay Tablets"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Sealable_Document_Clay"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingClaySealTablet() => new()
	{
		Metadata = new("medieval_writing_clay_seal_tablet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2760", []),
		Noun = "tablet",
		ShortDescription = "a clay seal tablet",
		LongDescription = null,
		FullDescription = "This clay seal tablet is a flattened clay tablet with a smoothed face and a prepared edge for sealing. The material gives it a firm, earthy weight and a surface suited to impressed marks or attached seal evidence. It is useful for secured account notes, labels, or authenticated document packets.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 600.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "clay",
		Tags = [
				"Functions / Writing Goods",
				"Market / Writing Materials / Clay Tablets"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Sealable_Document_Clay"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPlainQuillPen() => new()
	{
		Metadata = new("medieval_writing_plain_quill_pen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2788", []),
		Noun = "quill",
		ShortDescription = "a plain quill pen",
		LongDescription = null,
		FullDescription = "This plain quill pen is cut from a trimmed feather shaft with a shaped nib at the point. The plume is partly stripped back so it sits comfortably in the hand, while the slit nib is ready to hold ink. It is a light writing implement for parchment, paper, and other ink-friendly surfaces.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5.0,
		Cost = 1.5m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "feather",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Quill Pen"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Quill_Pen"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingGooseQuillPen() => new()
	{
		Metadata = new("medieval_writing_goose_quill_pen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2816", []),
		Noun = "quill",
		ShortDescription = "a goose quill pen",
		LongDescription = null,
		FullDescription = "This goose quill pen is cut from a trimmed feather shaft with a shaped nib at the point. The plume is partly stripped back so it sits comfortably in the hand, while the slit nib is ready to hold ink. It is a light writing implement for parchment, paper, and other ink-friendly surfaces.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5.0,
		Cost = 1.8m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "feather",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Quill Pen"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Quill_Pen"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingFineQuillPen() => new()
	{
		Metadata = new("medieval_writing_fine_quill_pen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2844", []),
		Noun = "quill",
		ShortDescription = "a fine quill pen",
		LongDescription = null,
		FullDescription = "This fine quill pen is cut from a trimmed feather shaft with a shaped nib at the point. The plume is partly stripped back so it sits comfortably in the hand, while the slit nib is ready to hold ink. It is a light writing implement for parchment, paper, and other ink-friendly surfaces.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 4.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "feather",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Quill Pen"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Fine_Quill_Pen"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSwanQuillPen() => new()
	{
		Metadata = new("medieval_writing_swan_quill_pen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2872", []),
		Noun = "quill",
		ShortDescription = "a broad swan quill",
		LongDescription = null,
		FullDescription = "This broad swan quill is cut from a trimmed feather shaft with a shaped nib at the point. The plume is partly stripped back so it sits comfortably in the hand, while the slit nib is ready to hold ink. It is a light writing implement for parchment, paper, and other ink-friendly surfaces.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 6.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "feather",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Quill Pen"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Fine_Quill_Pen"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingTrimmedReedPen() => new()
	{
		Metadata = new("medieval_writing_trimmed_reed_pen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2900", []),
		Noun = "pen",
		ShortDescription = "a trimmed reed pen",
		LongDescription = null,
		FullDescription = "This trimmed reed pen is cut from reed with a shaped point and split nib for carrying ink. The shaft is straight and light, with the writing end trimmed to suit either broad or fine strokes. It is practical for paper, parchment, papyrus, and calligraphic work where a firm nib is preferred.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6.0,
		Cost = 1.2m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "reed",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Reed Pen"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Reed_Pen"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBroadReedPen() => new()
	{
		Metadata = new("medieval_writing_broad_reed_pen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2928", []),
		Noun = "pen",
		ShortDescription = "a broad reed pen",
		LongDescription = null,
		FullDescription = "This broad reed pen is cut from reed with a shaped point and split nib for carrying ink. The shaft is straight and light, with the writing end trimmed to suit either broad or fine strokes. It is practical for paper, parchment, papyrus, and calligraphic work where a firm nib is preferred.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 7.0,
		Cost = 1.4m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "reed",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Reed Pen"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Reed_Pen"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingQalam() => new()
	{
		Metadata = new("medieval_writing_qalam", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2956", []),
		Noun = "qalam",
		ShortDescription = "a cut qalam pen",
		LongDescription = null,
		FullDescription = "This cut qalam pen is cut from reed with a shaped point and split nib for carrying ink. The shaft is straight and light, with the writing end trimmed to suit either broad or fine strokes. It is practical for paper, parchment, papyrus, and calligraphic work where a firm nib is preferred.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "reed",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Qalam Cutter"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Qalam"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingFineQalam() => new()
	{
		Metadata = new("medieval_writing_fine_qalam", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:2984", []),
		Noun = "qalam",
		ShortDescription = "a fine cut qalam",
		LongDescription = null,
		FullDescription = "This fine cut qalam is cut from reed with a shaped point and split nib for carrying ink. The shaft is straight and light, with the writing end trimmed to suit either broad or fine strokes. It is practical for paper, parchment, papyrus, and calligraphic work where a firm nib is preferred.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 8.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "reed",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Qalam Cutter"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Qalam"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingEastAsianBrush() => new()
	{
		Metadata = new("medieval_writing_east_asian_brush", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3012", []),
		Noun = "brush",
		ShortDescription = "an East Asian writing brush",
		LongDescription = null,
		FullDescription = "This East Asian writing brush has a slim handle and a fine gathered tip meant to carry liquid ink or pigment. The bristles come together to a flexible point, allowing both narrow lines and broader strokes. It is suited to calligraphy, brush-written documents, labels, and manuscript decoration.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Ink Brush"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_East_Asian_Writing_Brush"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingFineCalligraphyBrush() => new()
	{
		Metadata = new("medieval_writing_fine_calligraphy_brush", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3040", []),
		Noun = "brush",
		ShortDescription = "a fine calligraphy brush",
		LongDescription = null,
		FullDescription = "This fine calligraphy brush has a slim handle and a fine gathered tip meant to carry liquid ink or pigment. The bristles come together to a flexible point, allowing both narrow lines and broader strokes. It is suited to calligraphy, brush-written documents, labels, and manuscript decoration.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 28.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Calligrapher's Brush"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Calligraphy_Brush"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSutraCopyingBrush() => new()
	{
		Metadata = new("medieval_writing_sutra_copying_brush", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3068", []),
		Noun = "brush",
		ShortDescription = "a sutra-copying brush",
		LongDescription = null,
		FullDescription = "This sutra-copying brush has a slim handle and a fine gathered tip meant to carry liquid ink or pigment. The bristles come together to a flexible point, allowing both narrow lines and broader strokes. It is suited to calligraphy, brush-written documents, labels, and manuscript decoration.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 24.0,
		Cost = 10.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Calligrapher's Brush"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Calligraphy_Brush"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLabelingBrush() => new()
	{
		Metadata = new("medieval_writing_labeling_brush", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3096", []),
		Noun = "brush",
		ShortDescription = "a stiff labeling brush",
		LongDescription = null,
		FullDescription = "This stiff labeling brush has a slim handle and a fine gathered tip meant to carry liquid ink or pigment. The bristles come together to a flexible point, allowing both narrow lines and broader strokes. It is suited to calligraphy, brush-written documents, labels, and manuscript decoration.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 26.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Ink Brush"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_East_Asian_Writing_Brush"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingCharcoalStick() => new()
	{
		Metadata = new("medieval_writing_charcoal_stick", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3124", []),
		Noun = "stick",
		ShortDescription = "a charcoal writing stick",
		LongDescription = null,
		FullDescription = "This charcoal writing stick is a narrow piece of charcoal rubbed clean enough to hold without crumbling at once. It leaves a dark dusty line and shows small black smears where it has been handled. It is useful for practice work, rough notes, sketches, and marks on boards or pottery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12.0,
		Cost = 0.5m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "charcoal",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Charcoal Stick"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Charcoal_Stick"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBoneStylus() => new()
	{
		Metadata = new("medieval_writing_bone_stylus", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3152", []),
		Noun = "stylus",
		ShortDescription = "a polished bone stylus",
		LongDescription = null,
		FullDescription = "This polished bone stylus is a slender pointed tool made for pressing or scratching marks into a prepared surface. One end narrows to a writing point while the grip is smoothed enough for repeated use. It is suited to wax tablets, clay, slate, bark, or other surfaces that accept incised marks.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 22.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Stylus"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Bone_Stylus"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingIvoryStylus() => new()
	{
		Metadata = new("medieval_writing_ivory_stylus", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3180", []),
		Noun = "stylus",
		ShortDescription = "an ivory writing stylus",
		LongDescription = null,
		FullDescription = "This ivory writing stylus is a slender pointed tool made for pressing or scratching marks into a prepared surface. One end narrows to a writing point while the grip is smoothed enough for repeated use. It is suited to wax tablets, clay, slate, bark, or other surfaces that accept incised marks.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 25.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ivory",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Stylus"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Bone_Stylus"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBronzeStylus() => new()
	{
		Metadata = new("medieval_writing_bronze_stylus", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3208", []),
		Noun = "stylus",
		ShortDescription = "a bronze writing stylus",
		LongDescription = null,
		FullDescription = "This bronze writing stylus is a slender pointed tool made for pressing or scratching marks into a prepared surface. One end narrows to a writing point while the grip is smoothed enough for repeated use. It is suited to wax tablets, clay, slate, bark, or other surfaces that accept incised marks.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 45.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Stylus"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Bronze_Stylus"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingIronStylus() => new()
	{
		Metadata = new("medieval_writing_iron_stylus", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3236", []),
		Noun = "stylus",
		ShortDescription = "an iron writing stylus",
		LongDescription = null,
		FullDescription = "This iron writing stylus is a slender pointed tool made for pressing or scratching marks into a prepared surface. One end narrows to a writing point while the grip is smoothed enough for repeated use. It is suited to wax tablets, clay, slate, bark, or other surfaces that accept incised marks.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Stylus"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Iron_Stylus"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingReedStylus() => new()
	{
		Metadata = new("medieval_writing_reed_stylus", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3264", []),
		Noun = "stylus",
		ShortDescription = "a sharpened reed stylus",
		LongDescription = null,
		FullDescription = "This sharpened reed stylus is a slender pointed tool made for pressing or scratching marks into a prepared surface. One end narrows to a writing point while the grip is smoothed enough for repeated use. It is suited to wax tablets, clay, slate, bark, or other surfaces that accept incised marks.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5.0,
		Cost = 0.5m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "reed",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Stylus"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Reed_Stylus"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingScribingChisel() => new()
	{
		Metadata = new("medieval_writing_scribing_chisel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3292", []),
		Noun = "chisel",
		ShortDescription = "a small scribing chisel",
		LongDescription = null,
		FullDescription = "This small scribing chisel is a small iron-edged tool with a narrow working point for firm incised marks. Its body is compact enough for hand control rather than heavy carpentry force. It is suited to hard writing surfaces, labels, and shallow inscription work.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 85.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Stylus"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Scribing_Chisel"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBronzePenKnife() => new()
	{
		Metadata = new("medieval_writing_bronze_pen_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3320", []),
		Noun = "knife",
		ShortDescription = "a bronze pen knife",
		LongDescription = null,
		FullDescription = "This bronze pen knife is a small sharp tool with a short blade made for careful writing-room work. The edge is suited to trimming quills, cutting reeds, scraping fibres, and making neat corrections or preparations. It is a precise implement rather than a heavy cutting knife.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 70.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Pen Knife"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingIronPenKnife() => new()
	{
		Metadata = new("medieval_writing_iron_pen_knife", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3347", []),
		Noun = "knife",
		ShortDescription = "an iron pen knife",
		LongDescription = null,
		FullDescription = "This iron pen knife is a small sharp tool with a short blade made for careful writing-room work. The edge is suited to trimming quills, cutting reeds, scraping fibres, and making neat corrections or preparations. It is a precise implement rather than a heavy cutting knife.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Pen Knife"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingQalamCutter() => new()
	{
		Metadata = new("medieval_writing_qalam_cutter", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3374", []),
		Noun = "cutter",
		ShortDescription = "a qalam cutting knife",
		LongDescription = null,
		FullDescription = "This qalam cutting knife is cut from reed with a shaped point and split nib for carrying ink. The shaft is straight and light, with the writing end trimmed to suit either broad or fine strokes. It is practical for paper, parchment, papyrus, and calligraphic work where a firm nib is preferred.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 65.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Qalam Cutter"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPenRest() => new()
	{
		Metadata = new("medieval_writing_pen_rest", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3401", []),
		Noun = "rest",
		ShortDescription = "a small pen rest",
		LongDescription = null,
		FullDescription = "This small pen rest is a small support for keeping pens and brushes clear of the writing surface. Its shape provides shallow grooves or raised rests where damp nibs and tips can lie without rolling away. It helps keep ink, pigment, and grit from spreading across a desk or manuscript page.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 60.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Pen Rest"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBambooPenRack() => new()
	{
		Metadata = new("medieval_writing_bamboo_pen_rack", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3428", []),
		Noun = "rack",
		ShortDescription = "a bamboo pen rack",
		LongDescription = null,
		FullDescription = "This bamboo pen rack is a small support for keeping pens and brushes clear of the writing surface. Its shape provides shallow grooves or raised rests where damp nibs and tips can lie without rolling away. It helps keep ink, pigment, and grit from spreading across a desk or manuscript page.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Pen Rack"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLinenPenWiper() => new()
	{
		Metadata = new("medieval_writing_linen_pen_wiper", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3455", []),
		Noun = "wiper",
		ShortDescription = "a linen pen wiper",
		LongDescription = null,
		FullDescription = "This linen pen wiper is a small piece of linen kept for cleaning nibs, fingers, and wet writing tools. The cloth is plain and absorbent, with expected dark marks from ink and soot around the working edge. It belongs beside pens, brushes, knives, and inkwells in a practical writing kit.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18.0,
		Cost = 1.5m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Pen Wiper"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingQuillCuringSand() => new()
	{
		Metadata = new("medieval_writing_quill_curing_sand", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3482", []),
		Noun = "sand",
		ShortDescription = "a tray of quill-curing sand",
		LongDescription = null,
		FullDescription = "This tray of quill-curing sand is cut from a trimmed feather shaft with a shaped nib at the point. The plume is partly stripped back so it sits comfortably in the hand, while the slit nib is ready to hold ink. It is a light writing implement for parchment, paper, and other ink-friendly surfaces.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 950.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "sand",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Quill Curing Sand"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSpareQuillBundle() => new()
	{
		Metadata = new("medieval_writing_spare_quill_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3509", []),
		Noun = "bundle",
		ShortDescription = "a bundle of spare quills",
		LongDescription = null,
		FullDescription = "This bundle of spare quills is cut from a trimmed feather shaft with a shaped nib at the point. The plume is partly stripped back so it sits comfortably in the hand, while the slit nib is ready to hold ink. It is a light writing implement for parchment, paper, and other ink-friendly surfaces.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "feather",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Quill Pen"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Stack_Number"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingReedPenBundle() => new()
	{
		Metadata = new("medieval_writing_reed_pen_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3537", []),
		Noun = "bundle",
		ShortDescription = "a bundle of reed pens",
		LongDescription = null,
		FullDescription = "This bundle of reed pens is cut from reed with a shaped point and split nib for carrying ink. The shaft is straight and light, with the writing end trimmed to suit either broad or fine strokes. It is practical for paper, parchment, papyrus, and calligraphic work where a firm nib is preferred.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 60.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "reed",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Reed Pen"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Stack_Number"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingCharcoalStickBundle() => new()
	{
		Metadata = new("medieval_writing_charcoal_stick_bundle", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3565", []),
		Noun = "bundle",
		ShortDescription = "a bundle of charcoal sticks",
		LongDescription = null,
		FullDescription = "This bundle of charcoal sticks gathers several narrow charcoal writing sticks into a simple tied group. The sticks are dark, light, and a little dusty, with broken ends that leave strong marks on rough surfaces. They are useful for practice boards, temporary marks, sketches, and rough labels.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "charcoal",
		Tags = [
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Charcoal Stick"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc",
				"Stack_Number"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSootInkCake() => new()
	{
		Metadata = new("medieval_writing_soot_ink_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3593", []),
		Noun = "cake",
		ShortDescription = "a soot-black ink cake",
		LongDescription = null,
		FullDescription = "This soot-black ink cake is a compact block of dark pigment and binder prepared for writing or painting. Its surface is hard and slightly polished where it has been handled, scraped, or rubbed with water. It is meant to be worked into ink or colour as needed rather than carried as loose powder.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "soot",
		Tags = [
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingCarbonInkStick() => new()
	{
		Metadata = new("medieval_writing_carbon_ink_stick", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3620", []),
		Noun = "stick",
		ShortDescription = "a carbon ink stick",
		LongDescription = null,
		FullDescription = "This carbon ink stick is a compact block of dark pigment and binder prepared for writing or painting. Its surface is hard and slightly polished where it has been handled, scraped, or rubbed with water. It is meant to be worked into ink or colour as needed rather than carried as loose powder.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 40.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "soot",
		Tags = [
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingLampblackPacket() => new()
	{
		Metadata = new("medieval_writing_lampblack_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3647", []),
		Noun = "packet",
		ShortDescription = "a packet of lampblack",
		LongDescription = null,
		FullDescription = "This packet of lampblack holds fine soot-black powder gathered for ink making. The contents are dark, dusty, and prone to smearing, so the packet is kept tightly folded. It is a pigment stock for carbon ink, washes, and dark manuscript details.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 25.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "soot",
		Tags = [
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingRedInkCake() => new()
	{
		Metadata = new("medieval_writing_red_ink_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3674", []),
		Noun = "cake",
		ShortDescription = "a red pigment ink cake",
		LongDescription = null,
		FullDescription = "This red pigment ink cake is a compact block of cinnabar-red pigment and binder prepared for writing or painting. Its surface is hard and slightly polished where it has been handled, scraped, or rubbed with water. It is meant to be worked into ink or colour as needed rather than carried as loose powder.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 32.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cinnabar",
		Tags = [
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBluePigmentCake() => new()
	{
		Metadata = new("medieval_writing_blue_pigment_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3701", []),
		Noun = "cake",
		ShortDescription = "a blue pigment cake",
		LongDescription = null,
		FullDescription = "This blue pigment cake is a compact block of azurite-blue pigment and binder prepared for writing or painting. Its surface is hard and slightly polished where it has been handled, scraped, or rubbed with water. It is meant to be worked into ink or colour as needed rather than carried as loose powder.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 30.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "azurite",
		Tags = [
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingGreenPigmentCake() => new()
	{
		Metadata = new("medieval_writing_green_pigment_cake", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3728", []),
		Noun = "cake",
		ShortDescription = "a green pigment cake",
		LongDescription = null,
		FullDescription = "This green pigment cake is a compact block of malachite-green pigment and binder prepared for writing or painting. Its surface is hard and slightly polished where it has been handled, scraped, or rubbed with water. It is meant to be worked into ink or colour as needed rather than carried as loose powder.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 30.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "malachite",
		Tags = [
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingGoldLeafPacket() => new()
	{
		Metadata = new("medieval_writing_gold_leaf_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3755", []),
		Noun = "packet",
		ShortDescription = "a packet of gold leaf",
		LongDescription = null,
		FullDescription = "This packet of gold leaf holds fragile sheets of thin gold leaf protected between small wrappers. The leaf catches light at the edges and is delicate enough to tear with careless handling. It is intended for manuscript illumination, initials, borders, and other fine decorative work.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.VeryGood,
		WeightInGrams = 3.0,
		Cost = 80.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "gold",
		Tags = [
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSilverLeafPacket() => new()
	{
		Metadata = new("medieval_writing_silver_leaf_packet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3782", []),
		Noun = "packet",
		ShortDescription = "a packet of silver leaf",
		LongDescription = null,
		FullDescription = "This packet of silver leaf holds fragile sheets of thin silver leaf protected between small wrappers. The leaf catches light at the edges and is delicate enough to tear with careless handling. It is intended for manuscript illumination, initials, borders, and other fine decorative work.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Good,
		WeightInGrams = 4.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silver",
		Tags = [
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPowderedChalk() => new()
	{
		Metadata = new("medieval_writing_powdered_chalk", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3809", []),
		Noun = "chalk",
		ShortDescription = "a pouch of whitening chalk",
		LongDescription = null,
		FullDescription = "This pouch of whitening chalk is a small supply of pale powder or pigment kept for manuscript preparation. The material is dry and crumbly, with dust caught in the folds or mouth of its wrapping. It is useful for whitening, surface preparation, or mixing into writing and painting stock.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "chalk",
		Tags = [
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPounceBag() => new()
	{
		Metadata = new("medieval_writing_pounce_bag", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3836", []),
		Noun = "bag",
		ShortDescription = "a small pounce bag",
		LongDescription = null,
		FullDescription = "This small pounce bag is a small cloth bag filled with fine powder for preparing a writing surface. The fabric is tight enough to sift lightly when tapped, leaving a dusting rather than a clump. It is used to dry, smooth, or ready a page before careful ink work.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 80.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingPumicePiece() => new()
	{
		Metadata = new("medieval_writing_pumice_piece", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3863", []),
		Noun = "pumice",
		ShortDescription = "a piece of parchment pumice",
		LongDescription = null,
		FullDescription = "This piece of parchment pumice is a light abrasive stone sized for rubbing parchment and paper surfaces. Its worn face is smooth in the hand but gritty enough to remove grease, raised fibres, or small roughness. It belongs with page preparation, correction, and finishing tools.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pumice",
		Tags = [
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingSmallClayInkwell() => new()
	{
		Metadata = new("medieval_writing_small_clay_inkwell", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3890", []),
		Noun = "inkwell",
		ShortDescription = "a small clay inkwell",
		LongDescription = null,
		FullDescription = "This small clay inkwell is a small vessel with a narrow mouth and stable base for holding writing ink. Dark staining gathers around the lip where pens and brushes have been dipped repeatedly. It is shaped to sit beside a page, tablet, or desk while keeping the ink pooled and accessible.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fired clay",
		Tags = [
				"Market / Writing Materials / Ink",
				"Functions / Tools / Scribing Tools / Inkwell"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingCeramicInkPot() => new()
	{
		Metadata = new("medieval_writing_ceramic_ink_pot", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3917", []),
		Noun = "inkpot",
		ShortDescription = "a ceramic ink pot",
		LongDescription = null,
		FullDescription = "This ceramic ink pot is a small vessel with a narrow mouth and stable base for holding writing ink. Dark staining gathers around the lip where pens and brushes have been dipped repeatedly. It is shaped to sit beside a page, tablet, or desk while keeping the ink pooled and accessible.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Market / Writing Materials / Ink",
				"Functions / Tools / Scribing Tools / Inkwell"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingGlassInkwell() => new()
	{
		Metadata = new("medieval_writing_glass_inkwell", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3944", []),
		Noun = "inkwell",
		ShortDescription = "a small glass inkwell",
		LongDescription = null,
		FullDescription = "This small glass inkwell is a small vessel with a narrow mouth and stable base for holding writing ink. Dark staining gathers around the lip where pens and brushes have been dipped repeatedly. It is shaped to sit beside a page, tablet, or desk while keeping the ink pooled and accessible.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 130.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Market / Writing Materials / Ink",
				"Functions / Tools / Scribing Tools / Inkwell"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalWritingBronzeTravelInkwell() => new()
	{
		Metadata = new("medieval_writing_bronze_travel_inkwell", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalWriting.cs:3971", []),
		Noun = "inkwell",
		ShortDescription = "a bronze travel inkwell",
		LongDescription = null,
		FullDescription = "This bronze travel inkwell is a compact lidded vessel made to carry ink without spilling at the first movement. Its mouth is narrow enough for dipping a pen or brush, and the body has enough weight to sit steady when opened. Stains around the rim show its place in a travelling writing kit.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Good,
		WeightInGrams = 210.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Market / Writing Materials / Ink",
				"Functions / Tools / Scribing Tools / Inkwell"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};
}
