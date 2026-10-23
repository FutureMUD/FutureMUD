#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Authored04Stock
{


























































	public static CatalogueItemDefinition EarlymodernMilitaryToolCranequin() => new()
	{
		Metadata = new("earlymodern_military_tool_cranequin", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EarlyModern.MilitaryFirearmsUniformsNaval.cs:105", []),
		Noun = "cranequin",
		ShortDescription = "a steel cranequin",
		LongDescription = null,
		FullDescription = "This compact rack-and-pinion cranequin hooks over a crossbow stock and turns through a geared handle to draw an exceptionally heavy prod.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2900,
		Cost = 190.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "mild steel",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Professional Tools / Standard Tools", "Functions / Military Equipment / Crossbow Spanning Tools / Cranequin"],
		Components = ["Holdable", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stock crossbow spanning tool retained from the supported dependency-ledger closure tranche.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMilitaryToolGoatsFoot() => new()
	{
		Metadata = new("earlymodern_military_tool_goats_foot", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EarlyModern.MilitaryFirearmsUniformsNaval.cs:105", []),
		Noun = "lever",
		ShortDescription = "an iron goat's-foot lever",
		LongDescription = null,
		FullDescription = "This hinged iron goat's-foot lever braces against a crossbow stock and uses its hooked jaws to draw the string with one strong motion.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1450,
		Cost = 80.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Professional Tools / Standard Tools", "Functions / Military Equipment / Crossbow Spanning Tools / Goat's Foot"],
		Components = ["Holdable", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stock crossbow spanning tool retained from the supported dependency-ledger closure tranche.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMilitaryToolSpanningLever() => new()
	{
		Metadata = new("earlymodern_military_tool_spanning_lever", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EarlyModern.MilitaryFirearmsUniformsNaval.cs:105", []),
		Noun = "lever",
		ShortDescription = "a wooden spanning lever",
		LongDescription = null,
		FullDescription = "This stout wooden spanning lever has an iron hook and reinforced fulcrum, sized to draw a crossbow by controlled leverage.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800,
		Cost = 48.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Professional Tools / Standard Tools", "Functions / Military Equipment / Crossbow Spanning Tools / Lever"],
		Components = ["Holdable", "Destroyable_WoodenHeavy"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stock crossbow spanning tool retained from the supported dependency-ledger closure tranche.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMilitaryToolSpanningHook() => new()
	{
		Metadata = new("earlymodern_military_tool_spanning_hook", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EarlyModern.MilitaryFirearmsUniformsNaval.cs:105", []),
		Noun = "hook",
		ShortDescription = "an iron spanning hook",
		LongDescription = null,
		FullDescription = "This belt-mounted iron spanning hook catches a crossbow string while the user straightens against the stock to draw it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 620,
		Cost = 32.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Professional Tools / Standard Tools", "Functions / Military Equipment / Crossbow Spanning Tools / Spanning Hook"],
		Components = ["Holdable", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stock crossbow spanning tool retained from the supported dependency-ledger closure tranche.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernMilitaryToolWindlass() => new()
	{
		Metadata = new("earlymodern_military_tool_windlass", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EarlyModern.MilitaryFirearmsUniformsNaval.cs:105", []),
		Noun = "windlass",
		ShortDescription = "a crossbow windlass",
		LongDescription = null,
		FullDescription = "This twin-crank windlass uses cords, hooks, and geared drums to span a powerful crossbow steadily and with little wasted effort.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3400,
		Cost = 150.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = ["Era / Early Modern Era", "Functions / Military Equipment", "Market / Professional Tools / Standard Tools", "Functions / Military Equipment / Crossbow Spanning Tools / Windlass"],
		Components = ["Holdable", "Destroyable_HeavyMetal"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Stock crossbow spanning tool retained from the supported dependency-ledger closure tranche.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingGazette() => new()
	{
		Metadata = new("earlymodern_writing_gazette", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "gazette",
		ShortDescription = "a folded printed gazette",
		LongDescription = null,
		FullDescription = "A folded printed sheet has dense columns of news, notices, shipping reports, and advertisements with space for local additions.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 35,
		Cost = 1.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Paper"],
		Components = ["Holdable", "Destroyable_Misc", "Paper_A3"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern printed-news format; content remains builder-authored.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingPlaybill() => new()
	{
		Metadata = new("earlymodern_writing_playbill", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "playbill",
		ShortDescription = "a printed theatre playbill",
		LongDescription = null,
		FullDescription = "A single printed sheet leaves a bold header and clear lower space for performances, times, and admissions.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 20,
		Cost = 0.8m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Paper"],
		Components = ["Holdable", "Destroyable_Misc", "Paper_A4"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern public notice format.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingShipLog() => new()
	{
		Metadata = new("earlymodern_writing_ship_log", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "log",
		ShortDescription = "a bound ship's log",
		LongDescription = null,
		FullDescription = "A tough bound log has ruled leaves for winds, courses, watches, cargo events, and the officer's hand.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 780,
		Cost = 20.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Codices"],
		Components = ["Holdable", "Destroyable_Misc", "Book_Small_200_Page"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern maritime administration record.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingPortRegister() => new()
	{
		Metadata = new("earlymodern_writing_port_register", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "register",
		ShortDescription = "a port entry register",
		LongDescription = null,
		FullDescription = "A broad register has columns for vessel names, masters, cargoes, dues, and clearance marks.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1400,
		Cost = 32.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Codices"],
		Components = ["Holdable", "Destroyable_Misc", "Book_500_Page"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern port administration record.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingBillOfLading() => new()
	{
		Metadata = new("earlymodern_writing_bill_of_lading", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "bill",
		ShortDescription = "a folded bill of lading",
		LongDescription = null,
		FullDescription = "A folded paper bill sets out a cargo, ship, consignee, and delivery terms in a compact merchant form.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30,
		Cost = 1.4m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Paper"],
		Components = ["Holdable", "Destroyable_Misc", "Paper_A4"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern shipping document; text remains builder-authored.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingBillOfExchange() => new()
	{
		Metadata = new("earlymodern_writing_bill_of_exchange", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "bill",
		ShortDescription = "a sealed bill of exchange",
		LongDescription = null,
		FullDescription = "A formal folded paper bill provides spaces for payment instruction, named parties, date, and endorsement.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 30,
		Cost = 2.5m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Paper"],
		Components = ["Holdable", "Destroyable_Misc", "Paper_A4"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern finance document; no currency-transfer mechanic is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingInsurancePolicy() => new()
	{
		Metadata = new("earlymodern_writing_insurance_policy", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "policy",
		ShortDescription = "a marine insurance policy",
		LongDescription = null,
		FullDescription = "A stitched paper policy provides lines for a vessel, cargo interest, stated risks, and subscribing names.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 50,
		Cost = 2.5m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Paper"],
		Components = ["Holdable", "Destroyable_Misc", "Paper_A4"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern insurance document; no insurance system effect is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingPassport() => new()
	{
		Metadata = new("earlymodern_writing_passport", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "passport",
		ShortDescription = "a folded travel passport",
		LongDescription = null,
		FullDescription = "A small folded paper passport has a prominent issuing panel and enough leaves for identification, route notes, and seals.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 45,
		Cost = 3.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Paper"],
		Components = ["Holdable", "Destroyable_Misc", "Paper_A5"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern travel document; no access-control mechanic is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingIndenturedContract() => new()
	{
		Metadata = new("earlymodern_writing_indentured_contract", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "contract",
		ShortDescription = "a folded indenture contract",
		LongDescription = null,
		FullDescription = "A folded parchment contract has matching cut edges and room for terms, names, witnesses, and seals.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 55,
		Cost = 2.2m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "parchment",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Paper"],
		Components = ["Holdable", "Destroyable_Misc", "Paper_A4"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern labour-contract form; text remains builder-authored.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingCompanyLedger() => new()
	{
		Metadata = new("earlymodern_writing_company_ledger", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "ledger",
		ShortDescription = "a large company ledger",
		LongDescription = null,
		FullDescription = "A heavy bound ledger has numbered leaves for accounts, shipments, dividends, and correspondence references.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2200,
		Cost = 46.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Codices"],
		Components = ["Holdable", "Destroyable_Misc", "Book_1000_Page"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern commercial record.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingAtlas() => new()
	{
		Metadata = new("earlymodern_writing_atlas", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "atlas",
		ShortDescription = "a bound printed atlas",
		LongDescription = null,
		FullDescription = "A broad bound atlas has heavy leaves arranged for maps, explanatory notes, and hand-coloured additions.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1800,
		Cost = 52.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Codices"],
		Components = ["Holdable", "Destroyable_Misc", "Book_200_Page"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern printed-atlas format; map content remains builder-authored.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingNaturalPhilosophyJournal() => new()
	{
		Metadata = new("earlymodern_writing_natural_philosophy_journal", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "journal",
		ShortDescription = "a stitched natural-philosophy journal",
		LongDescription = null,
		FullDescription = "A stitched journal has clean leaves for observations, diagrams, correspondence extracts, and experimental notes.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 650,
		Cost = 18.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Codices"],
		Components = ["Holdable", "Destroyable_Misc", "Book_Small_200_Page"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern scholarly record.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingInkstand() => new()
	{
		Metadata = new("earlymodern_writing_inkstand", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "inkstand",
		ShortDescription = "a brass inkstand",
		LongDescription = null,
		FullDescription = "A low brass inkstand has two shallow wells and a rim for resting pens during office work.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 520,
		Cost = 11.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Writing Implements"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern writing-desk accessory.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingSandShaker() => new()
	{
		Metadata = new("earlymodern_writing_sand_shaker", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "shaker",
		ShortDescription = "a pierced brass sand shaker",
		LongDescription = null,
		FullDescription = "A small pierced brass shaker dispenses drying sand across fresh writing or account entries.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 180,
		Cost = 6.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Writing Implements"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern writing-desk accessory.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingPenknife() => new()
	{
		Metadata = new("earlymodern_writing_penknife", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "penknife",
		ShortDescription = "a small steel penknife",
		LongDescription = null,
		FullDescription = "A folding steel penknife has a fine short blade for trimming quills and cutting paper ties.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 90,
		Cost = 7.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "wrought iron",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Writing Implements"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern writing tool; no weapon claim is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingDispatchPacket() => new()
	{
		Metadata = new("earlymodern_writing_dispatch_packet", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "packet",
		ShortDescription = "a sealed dispatch packet",
		LongDescription = null,
		FullDescription = "A tied leather-wrapped packet protects folded orders, letters, and reports while in the hands of a courier.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 160,
		Cost = 5.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Paper"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern courier packet.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingDeedBox() => new()
	{
		Metadata = new("earlymodern_writing_deed_box", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "box",
		ShortDescription = "a small deed box",
		LongDescription = null,
		FullDescription = "A compact oak box has a close lid and divided interior for folded deeds, seals, and receipts.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 2200,
		Cost = 25.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Early Modern Era", "Market / Writing Materials / Document Containers"],
		Components = ["Holdable", "Destroyable_Misc", "Container_Archive_Box"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern document-storage box.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernWritingArchiveShelving() => new()
	{
		Metadata = new("earlymodern_writing_archive_shelving", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "shelving",
		ShortDescription = "a wall of archive shelving",
		LongDescription = null,
		FullDescription = "Tall oak shelves provide ordered bays for ledgers, document bundles, and archive boxes in an office or counting house.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 65000,
		Cost = 290.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Furniture"],
		Components = ["Destroyable_Furniture", "Container_Document_Bookcase_Shelves"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern installed archive furniture; intentionally not holdable.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceCompoundMicroscope() => new()
	{
		Metadata = new("earlymodern_science_compound_microscope", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "microscope",
		ShortDescription = "a brass compound microscope",
		LongDescription = null,
		FullDescription = "A brass tube sits above a small stage and mirror in a compact instrument for close visual examination.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1150,
		Cost = 96.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern optical instrument; no magnification mechanic is claimed.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceLensCase() => new()
	{
		Metadata = new("earlymodern_science_lens_case", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "case",
		ShortDescription = "a leather lens case",
		LongDescription = null,
		FullDescription = "A padded leather case has divided spaces for small optical lenses and careful workshop transport.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 480,
		Cost = 18.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "Container_PreIndustrial_CompartmentBox"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern optical-workshop storage.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceGlassLensBlank() => new()
	{
		Metadata = new("earlymodern_science_glass_lens_blank", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "blank",
		ShortDescription = "a ground glass lens blank",
		LongDescription = null,
		FullDescription = "A round clear glass blank has a rough ground edge ready for polishing into a small optical lens.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 140,
		Cost = 9.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "glass blank",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern optics stock.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceMercuryBarometer() => new()
	{
		Metadata = new("earlymodern_science_mercury_barometer", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "barometer",
		ShortDescription = "a mercury barometer",
		LongDescription = null,
		FullDescription = "A tall glass tube and cistern are mounted in a narrow wooden frame with a simple marked scale.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2200,
		Cost = 120.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern pressure instrument; no pressure-reading mechanic is claimed.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceSpiritThermometer() => new()
	{
		Metadata = new("earlymodern_science_spirit_thermometer", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "thermometer",
		ShortDescription = "a spirit thermometer",
		LongDescription = null,
		FullDescription = "A fine glass tube and coloured liquid are fixed to a small marked board for comparative temperature observation.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 380,
		Cost = 68.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern temperature instrument; no temperature-reading mechanic is claimed.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernSciencePocketWatch() => new()
	{
		Metadata = new("earlymodern_science_pocket_watch", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "watch",
		ShortDescription = "a brass pocket watch",
		LongDescription = null,
		FullDescription = "A round brass watch case has a glass face, hands, and a winding stem attached to a short chain.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 260,
		Cost = 150.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern watch form; no runtime timekeeping component is claimed.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceBackstaff() => new()
	{
		Metadata = new("earlymodern_science_backstaff", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "backstaff",
		ShortDescription = "a wooden backstaff",
		LongDescription = null,
		FullDescription = "A graduated wooden backstaff has a sliding vane and shaded sight for taking a sun observation without looking directly at it.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 820,
		Cost = 46.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern navigation instrument; no automatic observation mechanic is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceOctant() => new()
	{
		Metadata = new("earlymodern_science_octant", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "octant",
		ShortDescription = "a brass reflecting octant",
		LongDescription = null,
		FullDescription = "A framed brass arc has mirrors, a sight, and a marked limb for late-period angular observation.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1150,
		Cost = 110.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Late Early Modern navigation instrument; date admission belongs to world content.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceParallelRules() => new()
	{
		Metadata = new("earlymodern_science_parallel_rules", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "rules",
		ShortDescription = "a pair of parallel rules",
		LongDescription = null,
		FullDescription = "Two linked brass rules slide evenly across a chart for plotting parallel courses and measured lines.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 230,
		Cost = 22.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern chart-working tool; no route calculation is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceProtractor() => new()
	{
		Metadata = new("earlymodern_science_protractor", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "protractor",
		ShortDescription = "a brass protractor",
		LongDescription = null,
		FullDescription = "A semicircular brass protractor is engraved with a simple angular scale for charts and workshop drawings.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 150,
		Cost = 12.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern drawing and survey tool.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceWeightSet() => new()
	{
		Metadata = new("earlymodern_science_weight_set", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "weights",
		ShortDescription = "a nested brass weight set",
		LongDescription = null,
		FullDescription = "A fitted wooden box holds a graduated nest of brass weights for balances and merchant measures.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1700,
		Cost = 45.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "Container_PreIndustrial_CompartmentBox"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern balance accessory; no automatic measurement mechanic is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceCelestialGlobe() => new()
	{
		Metadata = new("earlymodern_science_celestial_globe", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "globe",
		ShortDescription = "a small celestial globe",
		LongDescription = null,
		FullDescription = "A painted globe is set within a brass meridian ring and wooden horizon stand for astronomical display.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3800,
		Cost = 140.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern astronomical display; no observation mechanic is implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceOrrery() => new()
	{
		Metadata = new("earlymodern_science_orrery", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "orrery",
		ShortDescription = "a small brass orrery",
		LongDescription = null,
		FullDescription = "A compact brass gearwork carries small spheres on arms around a central body as a late-period astronomical demonstration.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 5600,
		Cost = 260.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Late Early Modern astronomical display; date admission belongs to world content.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceHerbariumFolder() => new()
	{
		Metadata = new("earlymodern_science_herbarium_folder", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "folder",
		ShortDescription = "a pressed-plant herbarium folder",
		LongDescription = null,
		FullDescription = "A stiff paper folder holds leaves and small plant specimens between labelled sheets for study and comparison.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 420,
		Cost = 14.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "Container_Document_Pouch"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern natural-history storage; labels remain builder-authored.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceMineralSpecimenBox() => new()
	{
		Metadata = new("earlymodern_science_mineral_specimen_box", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "box",
		ShortDescription = "a divided mineral specimen box",
		LongDescription = null,
		FullDescription = "A shallow oak box has padded divisions for labelled stones, ores, and small mineral samples.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1450,
		Cost = 36.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "Container_PreIndustrial_CompartmentBox"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern natural-history storage.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceInsectCase() => new()
	{
		Metadata = new("earlymodern_science_insect_case", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "case",
		ShortDescription = "a glazed insect case",
		LongDescription = null,
		FullDescription = "A small glazed case has a lined back for pinned insect specimens and handwritten labels.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 780,
		Cost = 30.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "Container_Glass_Casket"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern natural-history display case.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceSampleVial() => new()
	{
		Metadata = new("earlymodern_science_sample_vial", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "vial",
		ShortDescription = "a stoppered sample vial",
		LongDescription = null,
		FullDescription = "A small clear glass vial has a ground stopper for dry specimens, powders, or a small liquid sample.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 120,
		Cost = 5.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "LContainer_Flask"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern sample vessel; accepts live liquids without requiring the optional medieval health package.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceLaboratoryRetort() => new()
	{
		Metadata = new("earlymodern_science_laboratory_retort", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "retort",
		ShortDescription = "a glass laboratory retort",
		LongDescription = null,
		FullDescription = "A round glass body tapers into a long downward neck for visible distillation demonstrations and workshop work.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 1100,
		Cost = 48.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Wares"],
		Components = ["Holdable", "Destroyable_Misc", "LContainer_PreIndustrial_Pot_12L"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern laboratory glass; distillation behaviour is not implied.",
		AllowLegacyShortDescriptionMatch = false,
	};

	public static CatalogueItemDefinition EarlymodernScienceSpecimenCabinet() => new()
	{
		Metadata = new("earlymodern_science_specimen_cabinet", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "cabinet",
		ShortDescription = "a natural-history specimen cabinet",
		LongDescription = null,
		FullDescription = "A tall oak cabinet has shallow labelled drawers for shells, mineral samples, dried plants, and other collections.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 62000,
		Cost = 320.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Furniture"],
		Components = ["Destroyable_Furniture", "Container_Dresser"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern collection furniture; intentionally not holdable.",
		AllowLegacyShortDescriptionMatch = false,
	};
}
