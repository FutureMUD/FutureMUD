#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedEarlyModernWritingPrintAdministrationAndFinance()
	{
		var paperTags = new[] { "Era / Early Modern Era", "Market / Writing Materials / Paper" };
		var bookTags = new[] { "Era / Early Modern Era", "Market / Writing Materials / Codices" };
		var implementTags = new[] { "Era / Early Modern Era", "Market / Writing Materials / Writing Implements" };
		SeedStraightforwardEraCatalogueItems(
			"Early Modern writing, print, administration, and finance catalogue",
			[
				new("earlymodern_writing_gazette", "gazette", "a folded printed gazette", "A folded printed sheet has dense columns of news, notices, shipping reports, and advertisements with space for local additions.", SizeCategory.Small, ItemQuality.Standard, 35.0, 1.0m, "paper", paperTags, ["Holdable", "Destroyable_Misc", "Paper_A3"], "Early Modern printed-news format; content remains builder-authored."),
				new("earlymodern_writing_playbill", "playbill", "a printed theatre playbill", "A single printed sheet leaves a bold header and clear lower space for performances, times, and admissions.", SizeCategory.Small, ItemQuality.Standard, 20.0, 0.8m, "paper", paperTags, ["Holdable", "Destroyable_Misc", "Paper_A4"], "Early Modern public notice format."),
				new("earlymodern_writing_ship_log", "log", "a bound ship's log", "A tough bound log has ruled leaves for winds, courses, watches, cargo events, and the officer's hand.", SizeCategory.Small, ItemQuality.Good, 780.0, 20.0m, "paper", bookTags, ["Holdable", "Destroyable_Misc", "Book_Small_200_Page"], "Early Modern maritime administration record."),
				new("earlymodern_writing_port_register", "register", "a port entry register", "A broad register has columns for vessel names, masters, cargoes, dues, and clearance marks.", SizeCategory.Normal, ItemQuality.Good, 1400.0, 32.0m, "paper", bookTags, ["Holdable", "Destroyable_Misc", "Book_500_Page"], "Early Modern port administration record."),
				new("earlymodern_writing_bill_of_lading", "bill", "a folded bill of lading", "A folded paper bill sets out a cargo, ship, consignee, and delivery terms in a compact merchant form.", SizeCategory.Small, ItemQuality.Standard, 30.0, 1.4m, "paper", paperTags, ["Holdable", "Destroyable_Misc", "Paper_A4"], "Early Modern shipping document; text remains builder-authored."),
				new("earlymodern_writing_bill_of_exchange", "bill", "a sealed bill of exchange", "A formal folded paper bill provides spaces for payment instruction, named parties, date, and endorsement.", SizeCategory.Small, ItemQuality.Good, 30.0, 2.5m, "paper", paperTags, ["Holdable", "Destroyable_Misc", "Paper_A4"], "Early Modern finance document; no currency-transfer mechanic is implied."),
				new("earlymodern_writing_insurance_policy", "policy", "a marine insurance policy", "A stitched paper policy provides lines for a vessel, cargo interest, stated risks, and subscribing names.", SizeCategory.Small, ItemQuality.Good, 50.0, 2.5m, "paper", paperTags, ["Holdable", "Destroyable_Misc", "Paper_A4"], "Early Modern insurance document; no insurance system effect is implied."),
				new("earlymodern_writing_passport", "passport", "a folded travel passport", "A small folded paper passport has a prominent issuing panel and enough leaves for identification, route notes, and seals.", SizeCategory.Small, ItemQuality.Good, 45.0, 3.0m, "paper", paperTags, ["Holdable", "Destroyable_Misc", "Paper_A5"], "Early Modern travel document; no access-control mechanic is implied."),
				new("earlymodern_writing_indentured_contract", "contract", "a folded indenture contract", "A folded parchment contract has matching cut edges and room for terms, names, witnesses, and seals.", SizeCategory.Small, ItemQuality.Good, 55.0, 2.2m, "parchment", paperTags, ["Holdable", "Destroyable_Misc", "Paper_A4"], "Early Modern labour-contract form; text remains builder-authored."),
				new("earlymodern_writing_company_ledger", "ledger", "a large company ledger", "A heavy bound ledger has numbered leaves for accounts, shipments, dividends, and correspondence references.", SizeCategory.Normal, ItemQuality.Good, 2200.0, 46.0m, "paper", bookTags, ["Holdable", "Destroyable_Misc", "Book_1000_Page"], "Early Modern commercial record."),
				new("earlymodern_writing_atlas", "atlas", "a bound printed atlas", "A broad bound atlas has heavy leaves arranged for maps, explanatory notes, and hand-coloured additions.", SizeCategory.Normal, ItemQuality.Good, 1800.0, 52.0m, "paper", bookTags, ["Holdable", "Destroyable_Misc", "Book_200_Page"], "Early Modern printed-atlas format; map content remains builder-authored."),
				new("earlymodern_writing_natural_philosophy_journal", "journal", "a stitched natural-philosophy journal", "A stitched journal has clean leaves for observations, diagrams, correspondence extracts, and experimental notes.", SizeCategory.Small, ItemQuality.Good, 650.0, 18.0m, "paper", bookTags, ["Holdable", "Destroyable_Misc", "Book_Small_200_Page"], "Early Modern scholarly record."),
				new("earlymodern_writing_inkstand", "inkstand", "a brass inkstand", "A low brass inkstand has two shallow wells and a rim for resting pens during office work.", SizeCategory.Small, ItemQuality.Standard, 520.0, 11.0m, "brass", implementTags, ["Holdable", "Destroyable_Misc"], "Early Modern writing-desk accessory."),
				new("earlymodern_writing_sand_shaker", "shaker", "a pierced brass sand shaker", "A small pierced brass shaker dispenses drying sand across fresh writing or account entries.", SizeCategory.Small, ItemQuality.Standard, 180.0, 6.0m, "brass", implementTags, ["Holdable", "Destroyable_Misc"], "Early Modern writing-desk accessory."),
				new("earlymodern_writing_penknife", "penknife", "a small steel penknife", "A folding steel penknife has a fine short blade for trimming quills and cutting paper ties.", SizeCategory.Small, ItemQuality.Standard, 90.0, 7.0m, "wrought iron", implementTags, ["Holdable", "Destroyable_Misc"], "Early Modern writing tool; no weapon claim is implied."),
				new("earlymodern_writing_dispatch_packet", "packet", "a sealed dispatch packet", "A tied leather-wrapped packet protects folded orders, letters, and reports while in the hands of a courier.", SizeCategory.Small, ItemQuality.Standard, 160.0, 5.0m, "leather", paperTags, ["Holdable", "Destroyable_Misc"], "Early Modern courier packet."),
				new("earlymodern_writing_deed_box", "box", "a small deed box", "A compact oak box has a close lid and divided interior for folded deeds, seals, and receipts.", SizeCategory.Small, ItemQuality.Good, 2200.0, 25.0m, "oak", ["Era / Early Modern Era", "Market / Writing Materials / Document Containers"], ["Holdable", "Destroyable_Misc", "Container_Archive_Box"], "Early Modern document-storage box."),
				new("earlymodern_writing_archive_shelving", "shelving", "a wall of archive shelving", "Tall oak shelves provide ordered bays for ledgers, document bundles, and archive boxes in an office or counting house.", SizeCategory.Large, ItemQuality.Standard, 65000.0, 290.0m, "oak", ["Era / Early Modern Era", "Market / Household Goods / Standard Furniture"], ["Destroyable_Furniture", "Container_Document_Bookcase_Shelves"], "Early Modern installed archive furniture; intentionally not holdable.")
			]);
	}
}
