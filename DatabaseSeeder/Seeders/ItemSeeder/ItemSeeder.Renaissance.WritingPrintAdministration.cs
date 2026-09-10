#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedRenaissanceWritingPrintAndAdministration()
	{
		var writingTags = new[] { "Era / Renaissance Era", "Market / Writing Materials / Writing Implements" };
		var paperTags = new[] { "Era / Renaissance Era", "Market / Writing Materials / Paper" };
		var bookTags = new[] { "Era / Renaissance Era", "Market / Writing Materials / Codices" };
		SeedStraightforwardEraCatalogueItems(
			"Renaissance writing, print, and administration catalogue",
			[
				new("renaissance_writing_civic_warrant", "warrant", "a folded civic warrant",
					"A folded parchment warrant bears ruled space for a civic officer's order, witness marks, and a seal impression.",
					SizeCategory.Small, ItemQuality.Standard, 35.0, 1.2m, "parchment", paperTags,
					["Holdable", "Destroyable_Misc", "Paper_A4"], "Renaissance civic and legal paper form."),
				new("renaissance_writing_port_customs_register", "register", "a bound port customs register",
					"A stout bound register provides ruled leaves for ships, cargoes, dues, and the signatures of port officers.",
					SizeCategory.Small, ItemQuality.Good, 850.0, 18.0m, "paper", bookTags,
					["Holdable", "Destroyable_Misc", "Book_Small_200_Page"], "Renaissance port and customs administration stock."),
				new("renaissance_writing_notarial_instrument", "instrument", "a sealed notarial instrument",
					"A carefully folded parchment instrument leaves a clear panel for a notary's act, witness names, and attached seal.",
					SizeCategory.Small, ItemQuality.Good, 45.0, 2.0m, "parchment", paperTags,
					["Holdable", "Destroyable_Misc", "Paper_A4"], "Renaissance legal-document form; text remains builder-authored."),
				new("renaissance_writing_sketchbook", "sketchbook", "a stitched artist's sketchbook",
					"A palm-sized stitched book holds heavy paper leaves for studies, measurements, and workshop drawings.",
					SizeCategory.Small, ItemQuality.Standard, 420.0, 8.0m, "paper", bookTags,
					["Holdable", "Destroyable_Misc", "Book_Small_90_Page"], "Renaissance drawing and workshop notebook."),
				new("renaissance_writing_commonplace_book", "book", "a leather-bound commonplace book",
					"A leather-bound volume has ordered leaves for copied quotations, household notes, accounts, and observations.",
					SizeCategory.Small, ItemQuality.Good, 650.0, 16.0m, "paper", bookTags,
					["Holdable", "Destroyable_Misc", "Book_Small_200_Page"], "Renaissance personal manuscript form."),
				new("renaissance_writing_pilot_book", "book", "a bound pilot book",
					"A water-stained bound book is arranged for bearings, hazards, soundings, and coastal sailing notes.",
					SizeCategory.Small, ItemQuality.Good, 760.0, 24.0m, "paper", bookTags,
					["Holdable", "Destroyable_Misc", "Book_Small_200_Page"], "Renaissance navigation record; no navigation bonus is implied."),
				new("renaissance_writing_portolan_chart", "chart", "a rolled portolan chart",
					"A broad parchment chart is ruled with rhumb lines and coastal margins, ready for builder-authored ports and bearings.",
					SizeCategory.Small, ItemQuality.Good, 95.0, 12.0m, "parchment", paperTags,
					["Holdable", "Destroyable_Misc", "Paper_A3"], "Renaissance chart format; no automatic route knowledge is implied."),
				new("renaissance_writing_printed_prayer_book", "book", "a small printed prayer book",
					"A compact bound prayer book has closely set leaves and a plain protective cover for regular devotional use.",
					SizeCategory.Small, ItemQuality.Good, 520.0, 14.0m, "paper", bookTags,
					["Holdable", "Destroyable_Misc", "Book_Small_90_Page"], "Renaissance printed-book format; content remains builder-authored."),
				new("renaissance_writing_printed_music_book", "book", "a stitched printed music book",
					"A stitched music book has wide pages suitable for printed staves, part names, and rehearsal annotations.",
					SizeCategory.Small, ItemQuality.Standard, 460.0, 12.0m, "paper", bookTags,
					["Holdable", "Destroyable_Misc", "Book_Small_90_Page"], "Renaissance printed-music format; no musical effect is implied."),
				new("renaissance_writing_woodblock", "woodblock", "a carved printing woodblock",
					"A close-grained wooden block has a raised carved face prepared for ink and repeated impression work.",
					SizeCategory.Small, ItemQuality.Standard, 750.0, 11.0m, "oak", writingTags,
					["Holdable", "Destroyable_Misc"], "Renaissance print-workshop stock; it makes no fixed-image claim."),
				new("renaissance_writing_copper_engraving_plate", "plate", "a polished copper engraving plate",
					"A thin polished copper plate has a clean face for incised lines, mirrored lettering, and careful hand printing.",
					SizeCategory.Small, ItemQuality.Good, 940.0, 28.0m, "copper", writingTags,
					["Holdable", "Destroyable_Misc"], "Renaissance engraving-workshop stock."),
				new("renaissance_writing_engraving_burin", "burin", "a graver's steel burin",
					"A short steel burin with a mushroom-shaped wooden handle is ground for controlled engraving cuts.",
					SizeCategory.Small, ItemQuality.Good, 180.0, 16.0m, "wrought iron", writingTags,
					["Holdable", "Destroyable_Misc"], "Renaissance engraving hand tool."),
				new("renaissance_writing_etching_needle", "needle", "an etching needle",
					"A fine pointed needle is set in a narrow handle for scratching prepared plates and retouching drawn lines.",
					SizeCategory.Small, ItemQuality.Standard, 60.0, 5.0m, "wrought iron", writingTags,
					["Holdable", "Destroyable_Misc"], "Renaissance plate-preparation tool."),
				new("renaissance_writing_seal_matrix", "matrix", "a brass seal matrix",
					"A small brass matrix has a flat engraved face intended for a household, office, or guild seal design.",
					SizeCategory.Small, ItemQuality.Good, 160.0, 22.0m, "brass", writingTags,
					["Holdable", "Destroyable_Misc"], "Renaissance sealing stock; no seal-stamp mechanics are claimed."),
				new("renaissance_writing_sealing_wax_stick", "wax", "a red sealing-wax stick",
					"A hard coloured stick of beeswax compound is sized to melt in small portions over a folded document or parcel.",
					SizeCategory.Small, ItemQuality.Standard, 85.0, 3.0m, "beeswax", writingTags,
					["Holdable", "Destroyable_Misc"], "Renaissance document-sealing consumable presentation."),
				new("renaissance_writing_courier_satchel", "satchel", "a buckled courier satchel",
					"A weathered leather satchel has a buckled flap and a narrow document compartment for letters and folded records.",
					SizeCategory.Small, ItemQuality.Standard, 920.0, 20.0m, "leather",
					["Era / Renaissance Era", "Market / Writing Materials / Document Containers"],
					["Holdable", "Destroyable_Misc", "Container_Document_Satchel"], "Renaissance courier and document-carrying container."),
				new("renaissance_writing_archive_chest", "chest", "an iron-bound archive chest",
					"A heavy oak chest is fitted with iron bands and divided space for bundled papers, books, and sealed office records.",
					SizeCategory.Large, ItemQuality.Good, 38000.0, 160.0m, "oak",
					["Era / Renaissance Era", "Market / Writing Materials / Document Containers"],
					["Destroyable_Furniture", "Container_Archive_Chest"], "Renaissance installed archive storage; intentionally not holdable.")
			]);
	}
}
