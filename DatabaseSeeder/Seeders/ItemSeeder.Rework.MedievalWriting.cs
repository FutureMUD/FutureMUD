#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalWritingAdministrationAndDocuments()
	{
		CreateItem(
			"medieval_writing_plain_parchment_sheet",
			"sheet",
			"a plain parchment sheet",
			null,
			"This plain parchment sheet is a plainly finished pale parchment leaf with a scraped, smoothed face and slightly darker edges. The worked skin shows faint grain and occasional knife or pumice marks when turned in the light. It is left blank for letters, accounts, charters, or later binding into a manuscript.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			7.0,
			3.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_fine_parchment_sheet",
			"sheet",
			"a fine parchment sheet",
			null,
			"This fine parchment sheet is a carefully finished pale parchment leaf with a scraped, smoothed face and slightly darker edges. The worked skin shows faint grain and occasional knife or pumice marks when turned in the light. It is left blank for letters, accounts, charters, or later binding into a manuscript.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			6.0,
			8.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_ruled_parchment_sheet",
			"sheet",
			"a ruled parchment sheet",
			null,
			"This ruled parchment sheet is a pale parchment leaf ruled with faint guide lines for an even hand. The ruling is light enough not to dominate the page, but clear enough to keep columns and margins disciplined. Its scraped surface has a slight tooth for ink and a tougher feel than paper.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			7.0,
			5.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_scraped_parchment_bifolium",
			"bifolium",
			"a scraped parchment bifolium",
			null,
			"This scraped parchment bifolium is folded from a single sheet of pale scraped parchment, with a clean central crease and paired writing leaves. The surface is smooth but still shows faint grain, edge trimming, and the slight translucence of worked skin. It is prepared for compact manuscript work, account notes, or binding into a larger quire.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			14.0,
			7.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Bifolium_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_fine_parchment_bifolium",
			"bifolium",
			"a fine parchment bifolium",
			null,
			"This fine parchment bifolium is folded from a single sheet of pale scraped parchment, with a clean central crease and paired writing leaves. The surface is smooth but still shows faint grain, edge trimming, and the slight translucence of worked skin. It is prepared for compact manuscript work, account notes, or binding into a larger quire.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			12.0,
			12.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Bifolium_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_blank_parchment_charter",
			"charter",
			"a blank parchment charter",
			null,
			"This blank parchment charter is a broad prepared parchment leaf with squared edges and a clear open face for formal writing. The skin has been scraped thin and burnished, leaving a faintly mottled grain beneath the pale surface. There is enough lower margin for witness marks, folds, or a later seal attachment.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			10.0,
			8.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_blank_parchment_writ",
			"writ",
			"a blank parchment writ",
			null,
			"This blank parchment writ is a trimmed parchment document blank with a firm, pale writing face. The edges are cut neat and the surface has been rubbed smooth so a quill can keep a narrow line. Its compact format suits orders, deeds, notices, and other short formal records.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			8.0,
			6.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_parchment_deed_sheet",
			"deed",
			"a blank parchment deed",
			null,
			"This blank parchment deed is a trimmed parchment document blank with a firm, pale writing face. The edges are cut neat and the surface has been rubbed smooth so a quill can keep a narrow line. Its compact format suits orders, deeds, notices, and other short formal records.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			9.0,
			7.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_parchment_label_tag",
			"tag",
			"a parchment label tag",
			null,
			"This parchment label tag is cut from pale parchment into a narrow durable tag with room for a short hand-written mark. A small pierced end or narrowed corner gives it a place for cord, thread, or attachment to a roll or packet. The surface is smoothed enough for ink while remaining tougher than ordinary paper.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			2.0,
			1.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_plain_rag_paper_sheet",
			"sheet",
			"a plain rag-paper sheet",
			null,
			"This plain rag-paper sheet is a plain rag-paper leaf with a pale, lightly fibrous writing surface. Its edges are cut square and the sheet flexes softly rather than springing like parchment. It is ready for everyday letters, records, school exercises, or loose manuscript notes.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			4.0,
			1.5m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_fine_rag_paper_sheet",
			"sheet",
			"a fine rag-paper sheet",
			null,
			"This fine rag-paper sheet is a smooth rag-paper leaf with a clean pale surface and carefully trimmed edges. The sheet has enough body to resist tearing while remaining lighter and more flexible than parchment. It is suited to petitions, correspondence, copies, and other written work where a presentable page matters.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			4.0,
			4.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_ruled_rag_paper_sheet",
			"sheet",
			"a ruled rag-paper sheet",
			null,
			"This ruled rag-paper sheet is a rag-paper leaf with faint ruling lines laid across the writing face. The paper is light and slightly fibrous, with neat margins left around the working area. It is prepared for accounts, school exercises, or a copyist's orderly text.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			4.5,
			2.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_folded_rag_paper_letter",
			"letter",
			"a folded rag-paper letter",
			null,
			"This folded rag-paper letter is folded from rag paper into a compact document form with creases already set into the sheet. The paper is pale, lightly fibrous, and firm enough to take quill or reed-pen ink without immediately feathering. Its folds leave a natural outer face for an address, mark, or seal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			5.0,
			2.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Letter_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_rag_paper_account_slip",
			"slip",
			"a rag-paper account slip",
			null,
			"This rag-paper account slip is a narrow strip of rag paper cut for short account entries or temporary notes. The surface is plain and pale, with a little fibre showing at the trimmed edges. It is small enough to tuck into a ledger, pouch, or bundle of records.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			3.0,
			1.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Letter_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_rag_paper_petition_leaf",
			"leaf",
			"a petition paper leaf",
			null,
			"This petition paper leaf is a smooth rag-paper leaf with a clean pale surface and carefully trimmed edges. The sheet has enough body to resist tearing while remaining lighter and more flexible than parchment. It is suited to petitions, correspondence, copies, and other written work where a presentable page matters.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			4.0,
			2.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_plain_papyrus_sheet",
			"sheet",
			"a plain papyrus sheet",
			null,
			"This plain papyrus sheet is made from pressed papyrus strips laid crosswise and burnished into a firm writing face. The pale surface shows fine plant fibres and a slight grid where the pith has been joined. It is ready for inked notes, accounts, letters, or attachment into a longer roll.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			4.0,
			2.0m,
			true,
			false,
			"papyrus",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Papyrus"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Papyrus_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_mediterranean_papyrus_letter",
			"letter",
			"a folded papyrus letter",
			null,
			"This folded papyrus letter is folded from pale papyrus, its outer face showing the crossing grain of pressed pith. The sheet is stiffer than rag paper and has a faint reedlike texture along the folds. It is meant for short correspondence, accounts, or compact Mediterranean-style record keeping.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			5.0,
			2.5m,
			true,
			false,
			"papyrus",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Papyrus"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Papyrus_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_papyrus_account_sheet",
			"sheet",
			"a papyrus account sheet",
			null,
			"This papyrus account sheet is made from pressed papyrus strips laid crosswise and burnished into a firm writing face. The pale surface shows fine plant fibres and a slight grid where the pith has been joined. It is ready for inked notes, accounts, letters, or attachment into a longer roll.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			4.5,
			2.0m,
			true,
			false,
			"papyrus",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Papyrus"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Papyrus_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_east_asian_paper_sheet",
			"sheet",
			"an East Asian paper sheet",
			null,
			"This East Asian paper sheet is made from light paper with a soft, even surface suited to brush or fine ink work. The sheet is thin but resilient, with straight-cut edges and a slight fibrous texture visible along the margins. It is intended for calligraphy, official notes, copied text, or careful document work.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			3.5,
			2.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Paper_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_fine_east_asian_paper_sheet",
			"sheet",
			"a fine East Asian paper sheet",
			null,
			"This fine East Asian paper sheet is made from light paper with a soft, even surface suited to brush or fine ink work. The sheet is thin but resilient, with straight-cut edges and a slight fibrous texture visible along the margins. It is intended for calligraphy, official notes, copied text, or careful document work.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			3.0,
			5.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Paper_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_east_asian_calligraphy_sheet",
			"sheet",
			"a calligraphy paper sheet",
			null,
			"This calligraphy paper sheet is made from light paper with a soft, even surface suited to brush or fine ink work. The sheet is thin but resilient, with straight-cut edges and a slight fibrous texture visible along the margins. It is intended for calligraphy, official notes, copied text, or careful document work.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			3.0,
			4.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Paper_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_east_asian_official_slip",
			"slip",
			"an official paper slip",
			null,
			"This official paper slip is made from light paper with a soft, even surface suited to brush or fine ink work. The sheet is thin but resilient, with straight-cut edges and a slight fibrous texture visible along the margins. It is intended for calligraphy, official notes, copied text, or careful document work.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			2.5,
			2.5m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Paper_Sheet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_birch_bark_letter",
			"letter",
			"a birch-bark letter",
			null,
			"This birch-bark letter is cut from pale birch bark and smoothed on the writing face. The bark keeps its natural curl and faint horizontal grain, with the edges rubbed down so they do not flake at once. It is suited to brief letters, tallies, practice marks, or rough notes made with ink, charcoal, or an incising point.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			12.0,
			1.0m,
			true,
			false,
			"birch",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Birch_Bark_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_birch_bark_account_strip",
			"strip",
			"a birch-bark account strip",
			null,
			"This birch-bark account strip is cut from pale birch bark and smoothed on the writing face. The bark keeps its natural curl and faint horizontal grain, with the edges rubbed down so they do not flake at once. It is suited to brief letters, tallies, practice marks, or rough notes made with ink, charcoal, or an incising point.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			8.0,
			0.8m,
			true,
			false,
			"birch",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Birch_Bark_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_birch_bark_practice_strip",
			"strip",
			"a birch-bark practice strip",
			null,
			"This birch-bark practice strip is cut from pale birch bark and smoothed on the writing face. The bark keeps its natural curl and faint horizontal grain, with the edges rubbed down so they do not flake at once. It is suited to brief letters, tallies, practice marks, or rough notes made with ink, charcoal, or an incising point.",
			SizeCategory.Tiny,
			ItemQuality.Substandard,
			7.0,
			0.5m,
			true,
			false,
			"birch",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Birch_Bark_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_palm_leaf_strip",
			"leaf",
			"a palm-leaf manuscript strip",
			null,
			"This palm-leaf manuscript strip is a narrow palm-leaf strip dried, trimmed, and smoothed for manuscript use. The long surface is pale and fibrous, with enough stiffness to hold a line without folding like paper. It is ready for incised or inked text and can be stacked or corded into a manuscript bundle.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			5.0,
			1.0m,
			true,
			false,
			"leaf",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Palm_Leaf_Manuscript_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_pierced_palm_leaf_strip",
			"leaf",
			"a pierced palm-leaf strip",
			null,
			"This pierced palm-leaf strip is a narrow palm-leaf strip dried, trimmed, and smoothed for manuscript use. The long surface is pale and fibrous, with enough stiffness to hold a line without folding like paper. It is ready for incised or inked text and can be stacked or corded into a manuscript bundle.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			5.0,
			1.2m,
			true,
			false,
			"leaf",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Palm_Leaf_Manuscript_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_inked_palm_leaf_strip",
			"leaf",
			"an ink-ready palm-leaf strip",
			null,
			"This ink-ready palm-leaf strip is a narrow palm-leaf strip dried, trimmed, and smoothed for manuscript use. The long surface is pale and fibrous, with enough stiffness to hold a line without folding like paper. It is ready for incised or inked text and can be stacked or corded into a manuscript bundle.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			5.0,
			1.5m,
			true,
			false,
			"leaf",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Palm_Leaf_Manuscript_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_bamboo_slip",
			"slip",
			"a bamboo writing slip",
			null,
			"This bamboo writing slip is a slim bamboo strip planed smooth for writing or marking. The surface keeps a pale woody sheen and a slight lengthwise grain beneath the prepared face. It can take brush writing, inked labels, or shallow incised marks for record keeping.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			18.0,
			1.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Bamboo_Slip_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_bamboo_document_tag",
			"tag",
			"a bamboo document tag",
			null,
			"This bamboo document tag is a slim bamboo strip planed smooth for writing or marking. The surface keeps a pale woody sheen and a slight lengthwise grain beneath the prepared face. It can take brush writing, inked labels, or shallow incised marks for record keeping.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			12.0,
			0.8m,
			true,
			false,
			"bamboo",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Bamboo_Slip_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_potsherd_ostracon",
			"ostracon",
			"a smoothed potsherd ostracon",
			null,
			"This smoothed potsherd ostracon is a broken ceramic writing piece selected for a flat, usable face. Its edges have been rubbed smoother while the writing surface remains hard, pale, and slightly uneven. It is useful for short notes, labels, accounts, or practice marks that do not need a full sheet.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			80.0,
			0.5m,
			true,
			false,
			"fired clay",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Clay Tablets"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Ostracon_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_plastered_ostracon",
			"ostracon",
			"a pale plastered ostracon",
			null,
			"This pale plastered ostracon is a broken ceramic writing piece selected for a flat, usable face. Its edges have been rubbed smoother while the writing surface remains hard, pale, and slightly uneven. It is useful for short notes, labels, accounts, or practice marks that do not need a full sheet.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			90.0,
			1.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Clay Tablets"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Ostracon_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_nested_parchment_quire",
			"quire",
			"a nested parchment quire",
			null,
			"This nested parchment quire is a nested gathering of blank leaves arranged for later sewing or binding. The folded edges sit together in a compact spine while the outer corners are trimmed into a tidy block. It is ready to become part of a codex, notebook, account book, or manuscript section.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			65.0,
			20.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Stack_Number"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_rag_paper_quire",
			"quire",
			"a quire of rag-paper sheets",
			null,
			"This quire of rag-paper sheets is a nested gathering of blank leaves arranged for later sewing or binding. The folded edges sit together in a compact spine while the outer corners are trimmed into a tidy block. It is ready to become part of a codex, notebook, account book, or manuscript section.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			45.0,
			10.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Stack_Number"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_bamboo_slip_bundle",
			"bundle",
			"a bundle of bamboo slips",
			null,
			"This bundle of bamboo slips gathers several slim bamboo slips into an orderly stack. Each slip is straight, smooth, and narrow, with enough weight to hang or lie in a stable bundle. The group is useful for accounts, labels, records, or practice text that can be tied together afterward.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			260.0,
			12.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Stack_Number"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_loose_scroll_label_tabs",
			"tabs",
			"a bundle of scroll label tabs",
			null,
			"This bundle of scroll label tabs contains a stack of small blank labels cut from writing stock. The pieces are narrow and light, with enough surface for a title, seal note, tally mark, or short identifying hand. They are meant to be tied to scrolls, packets, bundles, or stored records.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			20.0,
			4.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Stack_Number"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_blank_paper_seal_tags",
			"tags",
			"a bundle of blank seal tags",
			null,
			"This bundle of blank seal tags contains a stack of small blank labels cut from writing stock. The pieces are narrow and light, with enough surface for a title, seal note, tally mark, or short identifying hand. They are meant to be tied to scrolls, packets, bundles, or stored records.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			18.0,
			4.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Stack_Number"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_parchment_roll",
			"roll",
			"a parchment writing roll",
			null,
			"This parchment writing roll is made from prepared parchment sheets joined or trimmed into a long roll. The surface is tougher and springier than paper, with pale scraped grain visible beneath the writing face. It is meant for charters, legal records, inventories, or other long formal documents.",
			SizeCategory.Small,
			ItemQuality.Standard,
			120.0,
			24.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Roll_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_charter_roll",
			"roll",
			"a blank charter roll",
			null,
			"This blank charter roll is a long prepared writing roll wound into a firm cylinder. The outer turn protects the writing surface and leaves room for a tie, label, or wrapper. It can be unrolled gradually for reading, copying, or record keeping.",
			SizeCategory.Small,
			ItemQuality.Good,
			140.0,
			32.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Roll_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_account_roll",
			"roll",
			"an account parchment roll",
			null,
			"This account parchment roll is a long prepared writing roll wound into a firm cylinder. The outer turn protects the writing surface and leaves room for a tie, label, or wrapper. It can be unrolled gradually for reading, copying, or record keeping.",
			SizeCategory.Small,
			ItemQuality.Standard,
			130.0,
			28.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Roll_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_court_roll",
			"roll",
			"a court record roll",
			null,
			"This court record roll is a long prepared writing roll wound into a firm cylinder. The outer turn protects the writing surface and leaves room for a tie, label, or wrapper. It can be unrolled gradually for reading, copying, or record keeping.",
			SizeCategory.Small,
			ItemQuality.Good,
			150.0,
			36.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Roll_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_estate_roll",
			"roll",
			"an estate account roll",
			null,
			"This estate account roll is a long prepared writing roll wound into a firm cylinder. The outer turn protects the writing surface and leaves room for a tie, label, or wrapper. It can be unrolled gradually for reading, copying, or record keeping.",
			SizeCategory.Small,
			ItemQuality.Standard,
			145.0,
			34.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Roll_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_tax_roll",
			"roll",
			"a tax account roll",
			null,
			"This tax account roll is a long prepared writing roll wound into a firm cylinder. The outer turn protects the writing surface and leaves room for a tie, label, or wrapper. It can be unrolled gradually for reading, copying, or record keeping.",
			SizeCategory.Small,
			ItemQuality.Standard,
			135.0,
			32.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Roll_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_papyrus_scroll",
			"scroll",
			"a simple papyrus scroll",
			null,
			"This simple papyrus scroll is built from joined papyrus sheets rolled into a compact cylinder. The pale writing surface has a faint plant-fibre grid and enough stiffness to unroll without collapsing. It is suited to accounts, letters, scholarly notes, or Mediterranean-style record rolls.",
			SizeCategory.Small,
			ItemQuality.Standard,
			65.0,
			10.0m,
			true,
			false,
			"papyrus",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Papyrus_Scroll_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_long_papyrus_scroll",
			"scroll",
			"a long papyrus scroll",
			null,
			"This long papyrus scroll is built from joined papyrus sheets rolled into a compact cylinder. The pale writing surface has a faint plant-fibre grid and enough stiffness to unroll without collapsing. It is suited to accounts, letters, scholarly notes, or Mediterranean-style record rolls.",
			SizeCategory.Small,
			ItemQuality.Standard,
			90.0,
			16.0m,
			true,
			false,
			"papyrus",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Papyrus_Scroll_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_papyrus_archive_roll",
			"roll",
			"a papyrus archive roll",
			null,
			"This papyrus archive roll is built from joined papyrus sheets rolled into a compact cylinder. The pale writing surface has a faint plant-fibre grid and enough stiffness to unroll without collapsing. It is suited to accounts, letters, scholarly notes, or Mediterranean-style record rolls.",
			SizeCategory.Small,
			ItemQuality.Standard,
			80.0,
			14.0m,
			true,
			false,
			"papyrus",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Papyrus_Scroll_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_rag_paper_scroll",
			"scroll",
			"a rag-paper scroll",
			null,
			"This rag-paper scroll is a long prepared writing roll wound into a firm cylinder. The outer turn protects the writing surface and leaves room for a tie, label, or wrapper. It can be unrolled gradually for reading, copying, or record keeping.",
			SizeCategory.Small,
			ItemQuality.Standard,
			55.0,
			12.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Scroll_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_east_asian_handscroll",
			"scroll",
			"an East Asian handscroll",
			null,
			"This East Asian handscroll is a long paper roll arranged for brush-written or copied text. The paper is joined in a smooth run and wound around a neat core so it can be opened section by section. Its outer turn leaves space for a label, wrapper, or protective tie.",
			SizeCategory.Small,
			ItemQuality.Good,
			80.0,
			22.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Paper_Scroll_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_east_asian_poetry_scroll",
			"scroll",
			"a blank poetry handscroll",
			null,
			"This blank poetry handscroll is a long paper roll arranged for brush-written or copied text. The paper is joined in a smooth run and wound around a neat core so it can be opened section by section. Its outer turn leaves space for a label, wrapper, or protective tie.",
			SizeCategory.Small,
			ItemQuality.Good,
			70.0,
			18.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Paper_Scroll_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_east_asian_edict_scroll",
			"scroll",
			"a formal paper edict scroll",
			null,
			"This formal paper edict scroll is a long paper roll arranged for brush-written or copied text. The paper is joined in a smooth run and wound around a neat core so it can be opened section by section. Its outer turn leaves space for a label, wrapper, or protective tie.",
			SizeCategory.Small,
			ItemQuality.Good,
			95.0,
			28.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Scroll",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Scrolls"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Paper_Scroll_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_small_parchment_codex",
			"codex",
			"a small parchment codex",
			null,
			"This small parchment codex is a bound parchment manuscript with folded leaves sewn between plain covers or boards. The pages have a firm pale surface and enough stiffness to hold their shape as the book opens. It is suited to legal, scholarly, devotional, or administrative writing that needs a durable codex form.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			52.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Codex_20_Page"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_plain_parchment_codex",
			"codex",
			"a plain parchment codex",
			null,
			"This plain parchment codex is a bound parchment manuscript with folded leaves sewn between plain covers or boards. The pages have a firm pale surface and enough stiffness to hold their shape as the book opens. It is suited to legal, scholarly, devotional, or administrative writing that needs a durable codex form.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			80.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Codex_40_Page"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_large_parchment_codex",
			"codex",
			"a large parchment codex",
			null,
			"This large parchment codex is a bound parchment manuscript with folded leaves sewn between plain covers or boards. The pages have a firm pale surface and enough stiffness to hold their shape as the book opens. It is suited to legal, scholarly, devotional, or administrative writing that needs a durable codex form.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1100.0,
			170.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Codex_90_Page"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_prayer_codex",
			"codex",
			"a small prayer codex",
			null,
			"This small prayer codex is a compact bound book with gathered leaves and a serviceable cover. The spine and boards are made to protect the writing surfaces while still allowing the leaves to turn cleanly. It is a practical manuscript form for records, lessons, copied texts, or library storage.",
			SizeCategory.Small,
			ItemQuality.Good,
			380.0,
			72.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Codex_20_Page"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_scholastic_codex",
			"codex",
			"a scholastic parchment codex",
			null,
			"This scholastic parchment codex is a compact bound book with gathered leaves and a serviceable cover. The spine and boards are made to protect the writing surfaces while still allowing the leaves to turn cleanly. It is a practical manuscript form for records, lessons, copied texts, or library storage.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1250.0,
			190.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Codex_90_Page"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_liturgical_codex",
			"codex",
			"a large liturgical codex",
			null,
			"This large liturgical codex is a compact bound book with gathered leaves and a serviceable cover. The spine and boards are made to protect the writing surfaces while still allowing the leaves to turn cleanly. It is a practical manuscript form for records, lessons, copied texts, or library storage.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			260.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Codex_90_Page"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_monastic_rule_codex",
			"codex",
			"a monastic rule codex",
			null,
			"This monastic rule codex is a compact bound book with gathered leaves and a serviceable cover. The spine and boards are made to protect the writing surfaces while still allowing the leaves to turn cleanly. It is a practical manuscript form for records, lessons, copied texts, or library storage.",
			SizeCategory.Small,
			ItemQuality.Good,
			580.0,
			95.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Codex_40_Page"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_merchant_account_ledger",
			"ledger",
			"a merchant account ledger",
			null,
			"This merchant account ledger is a bound account book with sturdy covers and orderly blank leaves inside. The pages are meant to open flat enough for columns, tallies, names, and repeated entries. Its construction is practical rather than ornamental, suitable for clerks, stewards, merchants, or estate records.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1150.0,
			140.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Account_Ledger_90_Page"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_estate_account_ledger",
			"ledger",
			"an estate account ledger",
			null,
			"This estate account ledger is a bound account book with sturdy covers and orderly blank leaves inside. The pages are meant to open flat enough for columns, tallies, names, and repeated entries. Its construction is practical rather than ornamental, suitable for clerks, stewards, merchants, or estate records.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1250.0,
			150.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Account_Ledger_90_Page"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_toll_ledger",
			"ledger",
			"a toll account ledger",
			null,
			"This toll account ledger is a bound account book with sturdy covers and orderly blank leaves inside. The pages are meant to open flat enough for columns, tallies, names, and repeated entries. Its construction is practical rather than ornamental, suitable for clerks, stewards, merchants, or estate records.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1000.0,
			130.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Account_Ledger_90_Page"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_rag_paper_codex",
			"codex",
			"a rag-paper codex",
			null,
			"This rag-paper codex is a small bound paper manuscript with gathered rag-paper leaves and plain protective covers. The pages are lighter and more flexible than parchment, with trimmed edges and a practical sewn spine. It is suited to school texts, accounts, copied notes, or portable reading.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			44.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Codex_40_Page"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_paper_scholarly_codex",
			"codex",
			"a paper scholarly codex",
			null,
			"This paper scholarly codex is a compact bound book with gathered leaves and a serviceable cover. The spine and boards are made to protect the writing surfaces while still allowing the leaves to turn cleanly. It is a practical manuscript form for records, lessons, copied texts, or library storage.",
			SizeCategory.Small,
			ItemQuality.Good,
			420.0,
			70.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Codex_40_Page"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_paper_devotional_codex",
			"codex",
			"a paper devotional codex",
			null,
			"This paper devotional codex is a compact bound book with gathered leaves and a serviceable cover. The spine and boards are made to protect the writing surfaces while still allowing the leaves to turn cleanly. It is a practical manuscript form for records, lessons, copied texts, or library storage.",
			SizeCategory.Small,
			ItemQuality.Good,
			430.0,
			78.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Codex_40_Page"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_east_asian_stitched_book",
			"book",
			"an East Asian stitched book",
			null,
			"This East Asian stitched book is a soft stitched book made from light paper leaves gathered along one edge. The cover is flexible and the sewing is visible, leaving the pages able to turn in thin, even groups. It is suited to copied text, accounts, poetry, teaching notes, or brush-written manuscript work.",
			SizeCategory.Small,
			ItemQuality.Standard,
			240.0,
			42.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Stitched_Book"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_east_asian_sutra_book",
			"book",
			"a stitched sutra-copying book",
			null,
			"This stitched sutra-copying book is a soft stitched book made from light paper leaves gathered along one edge. The cover is flexible and the sewing is visible, leaving the pages able to turn in thin, even groups. It is suited to copied text, accounts, poetry, teaching notes, or brush-written manuscript work.",
			SizeCategory.Small,
			ItemQuality.Good,
			280.0,
			64.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Stitched_Book"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_east_asian_account_book",
			"book",
			"a stitched account book",
			null,
			"This stitched account book is a soft stitched book made from light paper leaves gathered along one edge. The cover is flexible and the sewing is visible, leaving the pages able to turn in thin, even groups. It is suited to copied text, accounts, poetry, teaching notes, or brush-written manuscript work.",
			SizeCategory.Small,
			ItemQuality.Standard,
			250.0,
			45.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Stitched_Book"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_palm_leaf_manuscript_bundle",
			"bundle",
			"a palm-leaf manuscript bundle",
			null,
			"This palm-leaf manuscript bundle is a wrapped group of manuscript leaves or papers kept together as one packet. The contents are stacked square and protected by an outer wrap or tie rather than hard covers. It is useful for drafts, copied sections, travelling notes, or records awaiting binding.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			42.0m,
			true,
			false,
			"leaf",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Palm_Leaf_Manuscript_Bundle"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_temple_palm_leaf_bundle",
			"bundle",
			"a temple palm-leaf bundle",
			null,
			"This temple palm-leaf bundle is a wrapped group of manuscript leaves or papers kept together as one packet. The contents are stacked square and protected by an outer wrap or tie rather than hard covers. It is useful for drafts, copied sections, travelling notes, or records awaiting binding.",
			SizeCategory.Small,
			ItemQuality.Good,
			460.0,
			60.0m,
			true,
			false,
			"leaf",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Palm_Leaf_Manuscript_Bundle"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_scholarly_palm_leaf_bundle",
			"bundle",
			"a scholarly palm-leaf bundle",
			null,
			"This scholarly palm-leaf bundle is a wrapped group of manuscript leaves or papers kept together as one packet. The contents are stacked square and protected by an outer wrap or tie rather than hard covers. It is useful for drafts, copied sections, travelling notes, or records awaiting binding.",
			SizeCategory.Small,
			ItemQuality.Good,
			440.0,
			55.0m,
			true,
			false,
			"leaf",
			[
				"Functions / Writing Surface",
				"Functions / Writing Surface / Codex",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Palm_Leaf_Manuscript_Bundle"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_wrapped_parchment_manuscript_bundle",
			"bundle",
			"a wrapped parchment manuscript bundle",
			null,
			"This wrapped parchment manuscript bundle is a wrapped group of manuscript leaves or papers kept together as one packet. The contents are stacked square and protected by an outer wrap or tie rather than hard covers. It is useful for drafts, copied sections, travelling notes, or records awaiting binding.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			70.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Codices"
			],
			[
				"Holdable",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_wrapped_paper_manuscript_bundle",
			"bundle",
			"a wrapped paper manuscript bundle",
			null,
			"This wrapped paper manuscript bundle is a wrapped group of manuscript leaves or papers kept together as one packet. The contents are stacked square and protected by an outer wrap or tie rather than hard covers. It is useful for drafts, copied sections, travelling notes, or records awaiting binding.",
			SizeCategory.Small,
			ItemQuality.Standard,
			380.0,
			42.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
			[
				"Holdable",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_blank_commentary_gathering",
			"gathering",
			"a blank commentary gathering",
			null,
			"This blank commentary gathering is a prepared gathering of folded leaves arranged for later binding. The nesting is neat, with the folds aligned to form a temporary spine. It can be sewn into a codex or used as a loose working section for commentary and copied text.",
			SizeCategory.Small,
			ItemQuality.Standard,
			95.0,
			28.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Parchment"
			],
			[
				"Holdable",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_wax_tablet",
			"tablet",
			"a wax writing tablet",
			null,
			"This wax writing tablet is a wooden board recessed and filled with smooth beeswax for stylus writing. The wax face is dark enough for scratches to show clearly and soft enough to be scraped back for reuse. Its compact form suits school exercises, accounts, lists, and short messages.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			12.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Wax Tablets"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wax_Tablet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_small_wax_tablet",
			"tablet",
			"a small wax tablet",
			null,
			"This small wax tablet is a wooden board recessed and filled with smooth beeswax for stylus writing. The wax face is dark enough for scratches to show clearly and soft enough to be scraped back for reuse. Its compact form suits school exercises, accounts, lists, and short messages.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			240.0,
			8.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Wax Tablets"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wax_Tablet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_student_wax_tablet",
			"tablet",
			"a student wax tablet",
			null,
			"This student wax tablet is a wooden board recessed and filled with smooth beeswax for stylus writing. The wax face is dark enough for scratches to show clearly and soft enough to be scraped back for reuse. Its compact form suits school exercises, accounts, lists, and short messages.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			9.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Wax Tablets"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wax_Tablet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_account_wax_tablet",
			"tablet",
			"an account wax tablet",
			null,
			"This account wax tablet is a wooden board recessed and filled with smooth beeswax for stylus writing. The wax face is dark enough for scratches to show clearly and soft enough to be scraped back for reuse. Its compact form suits school exercises, accounts, lists, and short messages.",
			SizeCategory.Small,
			ItemQuality.Standard,
			450.0,
			14.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Wax Tablets"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wax_Tablet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_wax_diptych",
			"diptych",
			"a wax tablet diptych",
			null,
			"This wax tablet diptych is made from paired waxed boards joined so their writing faces close together. The recessed panels hold darkened wax smoothed flat enough to take stylus marks. It is portable and reusable, suited to accounts, exercises, letters, or temporary records.",
			SizeCategory.Small,
			ItemQuality.Standard,
			850.0,
			25.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Wax Tablets"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wax_Diptych_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_wax_triptych",
			"triptych",
			"a wax tablet triptych",
			null,
			"This wax tablet triptych has three waxed leaves bound together into a compact reusable writing set. Each recessed panel is filled with smoothed beeswax, and the outer boards protect the inner writing faces when closed. It is useful for longer notes, account lists, or formal temporary memoranda.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1250.0,
			36.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Wax Tablets"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wax_Triptych_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_bronze_framed_wax_tablet",
			"tablet",
			"a bronze-framed wax tablet",
			null,
			"This bronze-framed wax tablet sets a smooth wax writing face within a sturdier metal-trimmed frame. The dark wax is levelled into a shallow recess and bordered so a stylus can work close to the edges. It reads as a more durable tablet for officials, teachers, or frequent record work.",
			SizeCategory.Small,
			ItemQuality.Good,
			620.0,
			35.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Wax Tablets"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wax_Tablet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_wooden_writing_tablet",
			"tablet",
			"a wooden writing tablet",
			null,
			"This wooden writing tablet is a flat wooden surface prepared for ink, charcoal, or shallow incised marks. The face is planed smoother than the back, with edges kept thick enough to survive repeated handling. It is useful for practice writing, tallying, rosters, or reusable working notes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			7.0m,
			true,
			false,
			"wood",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wooden_Tablet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_cedar_writing_tablet",
			"tablet",
			"a cedar writing tablet",
			null,
			"This cedar writing tablet is a flat cedar surface prepared for ink, charcoal, or shallow incised marks. The face is planed smoother than the back, with edges kept thick enough to survive repeated handling. It is useful for practice writing, tallying, rosters, or reusable working notes.",
			SizeCategory.Small,
			ItemQuality.Good,
			280.0,
			9.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Wooden_Tablet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_wooden_practice_tablet",
			"tablet",
			"a wooden practice tablet",
			null,
			"This wooden practice tablet is a flat wooden surface prepared for ink, charcoal, or shallow incised marks. The face is planed smoother than the back, with edges kept thick enough to survive repeated handling. It is useful for practice writing, tallying, rosters, or reusable working notes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			5.0m,
			true,
			false,
			"wood",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Practice_Board_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_wooden_account_board",
			"board",
			"a wooden account board",
			null,
			"This wooden account board is a flat wooden surface prepared for ink, charcoal, or shallow incised marks. The face is planed smoother than the back, with edges kept thick enough to survive repeated handling. It is useful for practice writing, tallying, rosters, or reusable working notes.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			520.0,
			12.0m,
			true,
			false,
			"wood",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Practice_Board_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_school_practice_board",
			"board",
			"a school practice board",
			null,
			"This school practice board is a flat wooden surface prepared for ink, charcoal, or shallow incised marks. The face is planed smoother than the back, with edges kept thick enough to survive repeated handling. It is useful for practice writing, tallying, rosters, or reusable working notes.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			480.0,
			10.0m,
			true,
			false,
			"wood",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Practice_Board_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_slate_tablet",
			"tablet",
			"a slate writing tablet",
			null,
			"This slate writing tablet is a flat slate writing surface with a dark, smooth face and rubbed edges. The stone is heavy for its size but durable, with enough texture to show pale scratches or incised marks. It is suited to school work, rosters, calculations, or temporary notes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			10.0m,
			true,
			false,
			"slate",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Slate_Tablet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_school_slate_tablet",
			"tablet",
			"a school slate tablet",
			null,
			"This school slate tablet is a flat slate writing surface with a dark, smooth face and rubbed edges. The stone is heavy for its size but durable, with enough texture to show pale scratches or incised marks. It is suited to school work, rosters, calculations, or temporary notes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			500.0,
			8.0m,
			true,
			false,
			"slate",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Slate_Tablet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_slate_roster_tablet",
			"tablet",
			"a slate roster tablet",
			null,
			"This slate roster tablet is a flat slate writing surface with a dark, smooth face and rubbed edges. The stone is heavy for its size but durable, with enough texture to show pale scratches or incised marks. It is suited to school work, rosters, calculations, or temporary notes.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			16.0m,
			true,
			false,
			"slate",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Slate_Tablet_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_bamboo_practice_panel",
			"panel",
			"a bamboo practice panel",
			null,
			"This bamboo practice panel is made from smooth bamboo strips set as a small practice surface. The face is pale and lightly ridged, with enough spring to take brush or stylus marks. It is a light teaching or copying aid for repeated short exercises.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			160.0,
			4.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Bamboo_Slip_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_reusable_tally_board",
			"board",
			"a reusable tally board",
			null,
			"This reusable tally board is a flat wooden surface prepared for ink, charcoal, or shallow incised marks. The face is planed smoother than the back, with edges kept thick enough to survive repeated handling. It is useful for practice writing, tallying, rosters, or reusable working notes.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			650.0,
			14.0m,
			true,
			false,
			"wood",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Practice_Board_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_merchant_roster_board",
			"board",
			"a merchant roster board",
			null,
			"This merchant roster board is a flat wooden surface prepared for ink, charcoal, or shallow incised marks. The face is planed smoother than the back, with edges kept thick enough to survive repeated handling. It is useful for practice writing, tallying, rosters, or reusable working notes.",
			SizeCategory.Normal,
			ItemQuality.Good,
			780.0,
			18.0m,
			true,
			false,
			"wood",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Practice_Board_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_clay_document_envelope",
			"envelope",
			"a clay document envelope",
			null,
			"This clay document envelope is shaped from clay as a protective shell for a smaller document or tablet. The outer face is smoothed for marks and left ready to take a clay or wax seal at the closed edge. It is meant to enclose, authenticate, and preserve a written record.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			4.0m,
			true,
			false,
			"clay",
			[
				"Functions / Writing Goods",
				"Market / Writing Materials / Clay Tablets"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Sealable_Document_Clay"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_clay_seal_tablet",
			"tablet",
			"a clay seal tablet",
			null,
			"This clay seal tablet is a flattened clay tablet with a smoothed face and a prepared edge for sealing. The material gives it a firm, earthy weight and a surface suited to impressed marks or attached seal evidence. It is useful for secured account notes, labels, or authenticated document packets.",
			SizeCategory.Small,
			ItemQuality.Standard,
			600.0,
			3.0m,
			true,
			false,
			"clay",
			[
				"Functions / Writing Goods",
				"Market / Writing Materials / Clay Tablets"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Sealable_Document_Clay"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_plain_quill_pen",
			"quill",
			"a plain quill pen",
			null,
			"This plain quill pen is cut from a trimmed feather shaft with a shaped nib at the point. The plume is partly stripped back so it sits comfortably in the hand, while the slit nib is ready to hold ink. It is a light writing implement for parchment, paper, and other ink-friendly surfaces.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			5.0,
			1.5m,
			true,
			false,
			"feather",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Quill Pen"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Quill_Pen"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_goose_quill_pen",
			"quill",
			"a goose quill pen",
			null,
			"This goose quill pen is cut from a trimmed feather shaft with a shaped nib at the point. The plume is partly stripped back so it sits comfortably in the hand, while the slit nib is ready to hold ink. It is a light writing implement for parchment, paper, and other ink-friendly surfaces.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			5.0,
			1.8m,
			true,
			false,
			"feather",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Quill Pen"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Quill_Pen"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_fine_quill_pen",
			"quill",
			"a fine quill pen",
			null,
			"This fine quill pen is cut from a trimmed feather shaft with a shaped nib at the point. The plume is partly stripped back so it sits comfortably in the hand, while the slit nib is ready to hold ink. It is a light writing implement for parchment, paper, and other ink-friendly surfaces.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			4.0,
			4.0m,
			true,
			false,
			"feather",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Quill Pen"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Fine_Quill_Pen"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_swan_quill_pen",
			"quill",
			"a broad swan quill",
			null,
			"This broad swan quill is cut from a trimmed feather shaft with a shaped nib at the point. The plume is partly stripped back so it sits comfortably in the hand, while the slit nib is ready to hold ink. It is a light writing implement for parchment, paper, and other ink-friendly surfaces.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			6.0,
			5.0m,
			true,
			false,
			"feather",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Quill Pen"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Fine_Quill_Pen"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_trimmed_reed_pen",
			"pen",
			"a trimmed reed pen",
			null,
			"This trimmed reed pen is cut from reed with a shaped point and split nib for carrying ink. The shaft is straight and light, with the writing end trimmed to suit either broad or fine strokes. It is practical for paper, parchment, papyrus, and calligraphic work where a firm nib is preferred.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			6.0,
			1.2m,
			true,
			false,
			"reed",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Reed Pen"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Reed_Pen"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_broad_reed_pen",
			"pen",
			"a broad reed pen",
			null,
			"This broad reed pen is cut from reed with a shaped point and split nib for carrying ink. The shaft is straight and light, with the writing end trimmed to suit either broad or fine strokes. It is practical for paper, parchment, papyrus, and calligraphic work where a firm nib is preferred.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			7.0,
			1.4m,
			true,
			false,
			"reed",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Reed Pen"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Reed_Pen"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_qalam",
			"qalam",
			"a cut qalam pen",
			null,
			"This cut qalam pen is cut from reed with a shaped point and split nib for carrying ink. The shaft is straight and light, with the writing end trimmed to suit either broad or fine strokes. It is practical for paper, parchment, papyrus, and calligraphic work where a firm nib is preferred.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			8.0,
			2.0m,
			true,
			false,
			"reed",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Qalam Cutter"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Qalam"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_fine_qalam",
			"qalam",
			"a fine cut qalam",
			null,
			"This fine cut qalam is cut from reed with a shaped point and split nib for carrying ink. The shaft is straight and light, with the writing end trimmed to suit either broad or fine strokes. It is practical for paper, parchment, papyrus, and calligraphic work where a firm nib is preferred.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			8.0,
			4.0m,
			true,
			false,
			"reed",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Qalam Cutter"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Qalam"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_east_asian_brush",
			"brush",
			"an East Asian writing brush",
			null,
			"This East Asian writing brush has a slim handle and a fine gathered tip meant to carry liquid ink or pigment. The bristles come together to a flexible point, allowing both narrow lines and broader strokes. It is suited to calligraphy, brush-written documents, labels, and manuscript decoration.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			22.0,
			5.0m,
			true,
			false,
			"bamboo",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Ink Brush"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_East_Asian_Writing_Brush"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_fine_calligraphy_brush",
			"brush",
			"a fine calligraphy brush",
			null,
			"This fine calligraphy brush has a slim handle and a fine gathered tip meant to carry liquid ink or pigment. The bristles come together to a flexible point, allowing both narrow lines and broader strokes. It is suited to calligraphy, brush-written documents, labels, and manuscript decoration.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			28.0,
			12.0m,
			true,
			false,
			"bamboo",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Calligrapher's Brush"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Calligraphy_Brush"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_sutra_copying_brush",
			"brush",
			"a sutra-copying brush",
			null,
			"This sutra-copying brush has a slim handle and a fine gathered tip meant to carry liquid ink or pigment. The bristles come together to a flexible point, allowing both narrow lines and broader strokes. It is suited to calligraphy, brush-written documents, labels, and manuscript decoration.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			24.0,
			10.0m,
			true,
			false,
			"bamboo",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Calligrapher's Brush"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Calligraphy_Brush"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_labeling_brush",
			"brush",
			"a stiff labeling brush",
			null,
			"This stiff labeling brush has a slim handle and a fine gathered tip meant to carry liquid ink or pigment. The bristles come together to a flexible point, allowing both narrow lines and broader strokes. It is suited to calligraphy, brush-written documents, labels, and manuscript decoration.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			26.0,
			6.0m,
			true,
			false,
			"wood",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Ink Brush"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_East_Asian_Writing_Brush"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_charcoal_stick",
			"stick",
			"a charcoal writing stick",
			null,
			"This charcoal writing stick is a narrow piece of charcoal rubbed clean enough to hold without crumbling at once. It leaves a dark dusty line and shows small black smears where it has been handled. It is useful for practice work, rough notes, sketches, and marks on boards or pottery.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			12.0,
			0.5m,
			true,
			false,
			"charcoal",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Charcoal Stick"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Charcoal_Stick"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_bone_stylus",
			"stylus",
			"a polished bone stylus",
			null,
			"This polished bone stylus is a slender pointed tool made for pressing or scratching marks into a prepared surface. One end narrows to a writing point while the grip is smoothed enough for repeated use. It is suited to wax tablets, clay, slate, bark, or other surfaces that accept incised marks.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			22.0,
			2.0m,
			true,
			false,
			"bone",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Stylus"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Bone_Stylus"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_ivory_stylus",
			"stylus",
			"an ivory writing stylus",
			null,
			"This ivory writing stylus is a slender pointed tool made for pressing or scratching marks into a prepared surface. One end narrows to a writing point while the grip is smoothed enough for repeated use. It is suited to wax tablets, clay, slate, bark, or other surfaces that accept incised marks.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			25.0,
			18.0m,
			true,
			false,
			"ivory",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Stylus"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Bone_Stylus"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_bronze_stylus",
			"stylus",
			"a bronze writing stylus",
			null,
			"This bronze writing stylus is a slender pointed tool made for pressing or scratching marks into a prepared surface. One end narrows to a writing point while the grip is smoothed enough for repeated use. It is suited to wax tablets, clay, slate, bark, or other surfaces that accept incised marks.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			45.0,
			5.0m,
			true,
			false,
			"bronze",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Stylus"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Bronze_Stylus"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_iron_stylus",
			"stylus",
			"an iron writing stylus",
			null,
			"This iron writing stylus is a slender pointed tool made for pressing or scratching marks into a prepared surface. One end narrows to a writing point while the grip is smoothed enough for repeated use. It is suited to wax tablets, clay, slate, bark, or other surfaces that accept incised marks.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			55.0,
			4.0m,
			true,
			false,
			"wrought iron",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Stylus"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Iron_Stylus"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_reed_stylus",
			"stylus",
			"a sharpened reed stylus",
			null,
			"This sharpened reed stylus is a slender pointed tool made for pressing or scratching marks into a prepared surface. One end narrows to a writing point while the grip is smoothed enough for repeated use. It is suited to wax tablets, clay, slate, bark, or other surfaces that accept incised marks.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			5.0,
			0.5m,
			true,
			false,
			"reed",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Stylus"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Reed_Stylus"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_scribing_chisel",
			"chisel",
			"a small scribing chisel",
			null,
			"This small scribing chisel is a small iron-edged tool with a narrow working point for firm incised marks. Its body is compact enough for hand control rather than heavy carpentry force. It is suited to hard writing surfaces, labels, and shallow inscription work.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			85.0,
			6.0m,
			true,
			false,
			"wrought iron",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Stylus"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Medieval_Scribing_Chisel"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_bronze_pen_knife",
			"knife",
			"a bronze pen knife",
			null,
			"This bronze pen knife is a small sharp tool with a short blade made for careful writing-room work. The edge is suited to trimming quills, cutting reeds, scraping fibres, and making neat corrections or preparations. It is a precise implement rather than a heavy cutting knife.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			6.0m,
			true,
			false,
			"bronze",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Pen Knife"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_iron_pen_knife",
			"knife",
			"an iron pen knife",
			null,
			"This iron pen knife is a small sharp tool with a short blade made for careful writing-room work. The edge is suited to trimming quills, cutting reeds, scraping fibres, and making neat corrections or preparations. It is a precise implement rather than a heavy cutting knife.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			80.0,
			5.0m,
			true,
			false,
			"wrought iron",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Pen Knife"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_qalam_cutter",
			"cutter",
			"a qalam cutting knife",
			null,
			"This qalam cutting knife is cut from reed with a shaped point and split nib for carrying ink. The shaft is straight and light, with the writing end trimmed to suit either broad or fine strokes. It is practical for paper, parchment, papyrus, and calligraphic work where a firm nib is preferred.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			65.0,
			7.0m,
			true,
			false,
			"bronze",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Qalam Cutter"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_pen_rest",
			"rest",
			"a small pen rest",
			null,
			"This small pen rest is a small support for keeping pens and brushes clear of the writing surface. Its shape provides shallow grooves or raised rests where damp nibs and tips can lie without rolling away. It helps keep ink, pigment, and grit from spreading across a desk or manuscript page.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			60.0,
			4.0m,
			true,
			false,
			"wood",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Pen Rest"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_bamboo_pen_rack",
			"rack",
			"a bamboo pen rack",
			null,
			"This bamboo pen rack is a small support for keeping pens and brushes clear of the writing surface. Its shape provides shallow grooves or raised rests where damp nibs and tips can lie without rolling away. It helps keep ink, pigment, and grit from spreading across a desk or manuscript page.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			8.0m,
			true,
			false,
			"bamboo",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Pen Rack"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_linen_pen_wiper",
			"wiper",
			"a linen pen wiper",
			null,
			"This linen pen wiper is a small piece of linen kept for cleaning nibs, fingers, and wet writing tools. The cloth is plain and absorbent, with expected dark marks from ink and soot around the working edge. It belongs beside pens, brushes, knives, and inkwells in a practical writing kit.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			18.0,
			1.5m,
			true,
			false,
			"linen",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Pen Wiper"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_quill_curing_sand",
			"sand",
			"a tray of quill-curing sand",
			null,
			"This tray of quill-curing sand is cut from a trimmed feather shaft with a shaped nib at the point. The plume is partly stripped back so it sits comfortably in the hand, while the slit nib is ready to hold ink. It is a light writing implement for parchment, paper, and other ink-friendly surfaces.",
			SizeCategory.Small,
			ItemQuality.Standard,
			950.0,
			5.0m,
			true,
			false,
			"sand",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Calligraphy Tools / Quill Curing Sand"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_spare_quill_bundle",
			"bundle",
			"a bundle of spare quills",
			null,
			"This bundle of spare quills is cut from a trimmed feather shaft with a shaped nib at the point. The plume is partly stripped back so it sits comfortably in the hand, while the slit nib is ready to hold ink. It is a light writing implement for parchment, paper, and other ink-friendly surfaces.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			35.0,
			6.0m,
			true,
			false,
			"feather",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Quill Pen"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Stack_Number"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_reed_pen_bundle",
			"bundle",
			"a bundle of reed pens",
			null,
			"This bundle of reed pens is cut from reed with a shaped point and split nib for carrying ink. The shaft is straight and light, with the writing end trimmed to suit either broad or fine strokes. It is practical for paper, parchment, papyrus, and calligraphic work where a firm nib is preferred.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			60.0,
			5.0m,
			true,
			false,
			"reed",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Reed Pen"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Stack_Number"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_charcoal_stick_bundle",
			"bundle",
			"a bundle of charcoal sticks",
			null,
			"This bundle of charcoal sticks gathers several narrow charcoal writing sticks into a simple tied group. The sticks are dark, light, and a little dusty, with broken ends that leave strong marks on rough surfaces. They are useful for practice boards, temporary marks, sketches, and rough labels.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			120.0,
			3.0m,
			true,
			false,
			"charcoal",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Charcoal Stick"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Stack_Number"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_soot_ink_cake",
			"cake",
			"a soot-black ink cake",
			null,
			"This soot-black ink cake is a compact block of dark pigment and binder prepared for writing or painting. Its surface is hard and slightly polished where it has been handled, scraped, or rubbed with water. It is meant to be worked into ink or colour as needed rather than carried as loose powder.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			3.0m,
			true,
			false,
			"soot",
			[
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_carbon_ink_stick",
			"stick",
			"a carbon ink stick",
			null,
			"This carbon ink stick is a compact block of dark pigment and binder prepared for writing or painting. Its surface is hard and slightly polished where it has been handled, scraped, or rubbed with water. It is meant to be worked into ink or colour as needed rather than carried as loose powder.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			40.0,
			4.0m,
			true,
			false,
			"soot",
			[
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_lampblack_packet",
			"packet",
			"a packet of lampblack",
			null,
			"This packet of lampblack holds fine soot-black powder gathered for ink making. The contents are dark, dusty, and prone to smearing, so the packet is kept tightly folded. It is a pigment stock for carbon ink, washes, and dark manuscript details.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			25.0,
			2.0m,
			true,
			false,
			"soot",
			[
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_red_ink_cake",
			"cake",
			"a red pigment ink cake",
			null,
			"This red pigment ink cake is a compact block of cinnabar-red pigment and binder prepared for writing or painting. Its surface is hard and slightly polished where it has been handled, scraped, or rubbed with water. It is meant to be worked into ink or colour as needed rather than carried as loose powder.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			32.0,
			8.0m,
			true,
			false,
			"cinnabar",
			[
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_blue_pigment_cake",
			"cake",
			"a blue pigment cake",
			null,
			"This blue pigment cake is a compact block of azurite-blue pigment and binder prepared for writing or painting. Its surface is hard and slightly polished where it has been handled, scraped, or rubbed with water. It is meant to be worked into ink or colour as needed rather than carried as loose powder.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			30.0,
			12.0m,
			true,
			false,
			"azurite",
			[
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_green_pigment_cake",
			"cake",
			"a green pigment cake",
			null,
			"This green pigment cake is a compact block of malachite-green pigment and binder prepared for writing or painting. Its surface is hard and slightly polished where it has been handled, scraped, or rubbed with water. It is meant to be worked into ink or colour as needed rather than carried as loose powder.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			30.0,
			9.0m,
			true,
			false,
			"malachite",
			[
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_gold_leaf_packet",
			"packet",
			"a packet of gold leaf",
			null,
			"This packet of gold leaf holds fragile sheets of thin gold leaf protected between small wrappers. The leaf catches light at the edges and is delicate enough to tear with careless handling. It is intended for manuscript illumination, initials, borders, and other fine decorative work.",
			SizeCategory.Tiny,
			ItemQuality.VeryGood,
			3.0,
			80.0m,
			true,
			false,
			"gold",
			[
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_silver_leaf_packet",
			"packet",
			"a packet of silver leaf",
			null,
			"This packet of silver leaf holds fragile sheets of thin silver leaf protected between small wrappers. The leaf catches light at the edges and is delicate enough to tear with careless handling. It is intended for manuscript illumination, initials, borders, and other fine decorative work.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			4.0,
			30.0m,
			true,
			false,
			"silver",
			[
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_powdered_chalk",
			"chalk",
			"a pouch of whitening chalk",
			null,
			"This pouch of whitening chalk is a small supply of pale powder or pigment kept for manuscript preparation. The material is dry and crumbly, with dust caught in the folds or mouth of its wrapping. It is useful for whitening, surface preparation, or mixing into writing and painting stock.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			90.0,
			3.0m,
			true,
			false,
			"chalk",
			[
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_pounce_bag",
			"bag",
			"a small pounce bag",
			null,
			"This small pounce bag is a small cloth bag filled with fine powder for preparing a writing surface. The fabric is tight enough to sift lightly when tapped, leaving a dusting rather than a clump. It is used to dry, smooth, or ready a page before careful ink work.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			80.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_pumice_piece",
			"pumice",
			"a piece of parchment pumice",
			null,
			"This piece of parchment pumice is a light abrasive stone sized for rubbing parchment and paper surfaces. Its worn face is smooth in the hand but gritty enough to remove grease, raised fibres, or small roughness. It belongs with page preparation, correction, and finishing tools.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			55.0,
			3.0m,
			true,
			false,
			"pumice",
			[
				"Market / Writing Materials / Ink",
				"Functions / Material Functions / Writing Craft Stock / Ink Stock"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_small_clay_inkwell",
			"inkwell",
			"a small clay inkwell",
			null,
			"This small clay inkwell is a small vessel with a narrow mouth and stable base for holding writing ink. Dark staining gathers around the lip where pens and brushes have been dipped repeatedly. It is shaped to sit beside a page, tablet, or desk while keeping the ink pooled and accessible.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			160.0,
			4.0m,
			true,
			false,
			"fired clay",
			[
				"Market / Writing Materials / Ink",
				"Functions / Tools / Scribing Tools / Inkwell"
			],
			[
				"Holdable",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_ceramic_ink_pot",
			"inkpot",
			"a ceramic ink pot",
			null,
			"This ceramic ink pot is a small vessel with a narrow mouth and stable base for holding writing ink. Dark staining gathers around the lip where pens and brushes have been dipped repeatedly. It is shaped to sit beside a page, tablet, or desk while keeping the ink pooled and accessible.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			180.0,
			6.0m,
			true,
			false,
			"ceramic",
			[
				"Market / Writing Materials / Ink",
				"Functions / Tools / Scribing Tools / Inkwell"
			],
			[
				"Holdable",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_glass_inkwell",
			"inkwell",
			"a small glass inkwell",
			null,
			"This small glass inkwell is a small vessel with a narrow mouth and stable base for holding writing ink. Dark staining gathers around the lip where pens and brushes have been dipped repeatedly. It is shaped to sit beside a page, tablet, or desk while keeping the ink pooled and accessible.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			130.0,
			14.0m,
			true,
			false,
			"glass",
			[
				"Market / Writing Materials / Ink",
				"Functions / Tools / Scribing Tools / Inkwell"
			],
			[
				"Holdable",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_bronze_travel_inkwell",
			"inkwell",
			"a bronze travel inkwell",
			null,
			"This bronze travel inkwell is a compact lidded vessel made to carry ink without spilling at the first movement. Its mouth is narrow enough for dipping a pen or brush, and the body has enough weight to sit steady when opened. Stains around the rim show its place in a travelling writing kit.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			210.0,
			22.0m,
			true,
			false,
			"bronze",
			[
				"Market / Writing Materials / Ink",
				"Functions / Tools / Scribing Tools / Inkwell"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_double_ceramic_inkwell",
			"inkwell",
			"a double ceramic inkwell",
			null,
			"This double ceramic inkwell is a small vessel with a narrow mouth and stable base for holding writing ink. Dark staining gathers around the lip where pens and brushes have been dipped repeatedly. It is shaped to sit beside a page, tablet, or desk while keeping the ink pooled and accessible.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			320.0,
			12.0m,
			true,
			false,
			"ceramic",
			[
				"Market / Writing Materials / Ink",
				"Functions / Tools / Scribing Tools / Inkwell"
			],
			[
				"Holdable",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_slate_inkstone",
			"inkstone",
			"a slate inkstone",
			null,
			"This slate inkstone is a flat stone with a shallow grinding area and a small hollow for ink. The surface is smoothed by use, with dark stains where an ink stick would be rubbed with water. It is a central tool for brush writing, calligraphy, and careful ink preparation.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			620.0,
			18.0m,
			true,
			false,
			"slate",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Calligraphy Tools"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_stone_inkstone",
			"inkstone",
			"a polished stone inkstone",
			null,
			"This polished stone inkstone is a flat stone with a shallow grinding area and a small hollow for ink. The surface is smoothed by use, with dark stains where an ink stick would be rubbed with water. It is a central tool for brush writing, calligraphy, and careful ink preparation.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			700.0,
			20.0m,
			true,
			false,
			"stone",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Calligraphy Tools"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_agate_inkstone",
			"inkstone",
			"a fine agate inkstone",
			null,
			"This fine agate inkstone is a flat stone with a shallow grinding area and a small hollow for ink. The surface is smoothed by use, with dark stains where an ink stick would be rubbed with water. It is a central tool for brush writing, calligraphy, and careful ink preparation.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			520.0,
			60.0m,
			true,
			false,
			"agate",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Calligraphy Tools"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_jade_inkstone",
			"inkstone",
			"a small jade inkstone",
			null,
			"This small jade inkstone is a flat stone with a shallow grinding area and a small hollow for ink. The surface is smoothed by use, with dark stains where an ink stick would be rubbed with water. It is a central tool for brush writing, calligraphy, and careful ink preparation.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			480.0,
			120.0m,
			true,
			false,
			"jade",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Calligraphy Tools"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_pigment_shell",
			"shell",
			"a pigment mixing shell",
			null,
			"This pigment mixing shell is a small shallow surface for mixing ink, binder, and pigment in controlled amounts. Its hollow or wells are smooth and easy to wipe, with faint stains from repeated colours. It is meant for illumination, rubrication, and small-scale manuscript painting.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			28.0,
			3.0m,
			true,
			false,
			"shell",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Pigment Shell"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_shell_palette",
			"palette",
			"a shallow shell palette",
			null,
			"This shallow shell palette is a small shallow surface for mixing ink, binder, and pigment in controlled amounts. Its hollow or wells are smooth and easy to wipe, with faint stains from repeated colours. It is meant for illumination, rubrication, and small-scale manuscript painting.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			4.0m,
			true,
			false,
			"shell",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Pigment Shell"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_stone_pigment_muller",
			"muller",
			"a stone pigment muller",
			null,
			"This stone pigment muller is a heavy smooth tool for grinding pigment into a finer working paste. The working face is worn flat and carries faint stains from mineral colour, soot, and binder. It is used before ink, paint, or illumination colours are ready for a brush.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			620.0,
			6.0m,
			true,
			false,
			"stone",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Pigment Muller"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_palette_slab",
			"slab",
			"a stone palette slab",
			null,
			"This stone palette slab is a small shallow surface for mixing ink, binder, and pigment in controlled amounts. Its hollow or wells are smooth and easy to wipe, with faint stains from repeated colours. It is meant for illumination, rubrication, and small-scale manuscript painting.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			850.0,
			8.0m,
			true,
			false,
			"stone",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Palette Slab"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_agate_burnisher",
			"burnisher",
			"an agate burnisher",
			null,
			"This agate burnisher is a compact writing or illumination supply prepared for use at a desk or scriptorium bench. The material is kept in a form that can be rubbed, dipped, mixed, or portioned as needed. It supports ink making, page preparation, colouring, and decorative manuscript work.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			85.0,
			18.0m,
			true,
			false,
			"agate",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Agate Burnisher"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_gesso_pot",
			"pot",
			"a small gesso pot",
			null,
			"This small gesso pot is a compact writing or illumination supply prepared for use at a desk or scriptorium bench. The material is kept in a form that can be rubbed, dipped, mixed, or portioned as needed. It supports ink making, page preparation, colouring, and decorative manuscript work.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			240.0,
			8.0m,
			true,
			false,
			"ceramic",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Gesso Pot"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_gilding_knife",
			"knife",
			"a gilding knife",
			null,
			"This gilding knife is a compact writing or illumination supply prepared for use at a desk or scriptorium bench. The material is kept in a form that can be rubbed, dipped, mixed, or portioned as needed. It supports ink making, page preparation, colouring, and decorative manuscript work.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			75.0,
			9.0m,
			true,
			false,
			"wrought iron",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Gilding Knife"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_gilding_tip",
			"tip",
			"a soft gilding tip",
			null,
			"This soft gilding tip is a compact writing or illumination supply prepared for use at a desk or scriptorium bench. The material is kept in a form that can be rubbed, dipped, mixed, or portioned as needed. It supports ink making, page preparation, colouring, and decorative manuscript work.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			15.0,
			6.0m,
			true,
			false,
			"feather",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Gilding Tip"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_gold_leaf_cushion",
			"cushion",
			"a gold-leaf cushion",
			null,
			"This gold-leaf cushion holds fragile sheets of thin gold leaf protected between small wrappers. The leaf catches light at the edges and is delicate enough to tear with careless handling. It is intended for manuscript illumination, initials, borders, and other fine decorative work.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			220.0,
			16.0m,
			true,
			false,
			"leather",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Gold Leaf Cushion"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_miniature_detail_brush",
			"brush",
			"a miniature detail brush",
			null,
			"This miniature detail brush is a small brush made for manuscript colour, ink, or fine decorative work. The handle is slim and the bristles are gathered to a controlled point for narrow strokes and tiny details. It belongs with pigments, palettes, and illumination tools rather than with heavy writing stock.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			18.0,
			12.0m,
			true,
			false,
			"wood",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Illumination Tools / Miniature Detail Brush"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_cedar_scroll_tube",
			"tube",
			"a cedar scroll tube",
			null,
			"This cedar scroll tube is a narrow cedar container shaped to protect rolled documents. The long body is smooth enough not to abrade the scroll, with a fitted mouth, flap, or cap implied by its closed form. It is sized for scrolls, maps, rolls, labels, or other slender document bundles.",
			SizeCategory.Small,
			ItemQuality.Standard,
			450.0,
			18.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Scroll_Tube"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_bamboo_scroll_tube",
			"tube",
			"a bamboo scroll tube",
			null,
			"This bamboo scroll tube is a narrow bamboo container shaped to protect rolled documents. The long body is smooth enough not to abrade the scroll, with a fitted mouth, flap, or cap implied by its closed form. It is sized for scrolls, maps, rolls, labels, or other slender document bundles.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			12.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Scroll_Tube"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_leather_scroll_case",
			"case",
			"a leather scroll case",
			null,
			"This leather scroll case is a narrow leather container shaped to protect rolled documents. The long body is smooth enough not to abrade the scroll, with a fitted mouth, flap, or cap implied by its closed form. It is sized for scrolls, maps, rolls, labels, or other slender document bundles.",
			SizeCategory.Small,
			ItemQuality.Good,
			520.0,
			20.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Scroll_Tube"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_silk_scroll_wrapper",
			"wrapper",
			"a silk scroll wrapper",
			null,
			"This silk scroll wrapper is a soft protective covering made to fold around manuscripts, scrolls, or tablets. The textile lies flat but can be wrapped snugly, with enough overlap to shield corners and outer leaves from dust. It is a light document protector rather than a hard case.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			80.0,
			18.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_linen_scroll_wrap",
			"wrap",
			"a linen scroll wrap",
			null,
			"This linen scroll wrap is a soft protective covering made to fold around manuscripts, scrolls, or tablets. The textile lies flat but can be wrapped snugly, with enough overlap to shield corners and outer leaves from dust. It is a light document protector rather than a hard case.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			90.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_leather_document_pouch",
			"pouch",
			"a leather document pouch",
			null,
			"This leather document pouch is a flat closable pouch shaped for folded documents and small writing supplies. Its body is flexible, with a flap or tucked closure that protects contents from dust and casual handling. It suits letters, seals, tablets, labels, or compact scribal tools.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			14.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_linen_document_pouch",
			"pouch",
			"a linen document pouch",
			null,
			"This linen document pouch is a flat closable pouch shaped for folded documents and small writing supplies. Its body is flexible, with a flap or tucked closure that protects contents from dust and casual handling. It suits letters, seals, tablets, labels, or compact scribal tools.",
			SizeCategory.Small,
			ItemQuality.Standard,
			120.0,
			6.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_tablet_wrap",
			"wrap",
			"a padded tablet wrap",
			null,
			"This padded tablet wrap is a soft protective covering made to fold around manuscripts, scrolls, or tablets. The textile lies flat but can be wrapped snugly, with enough overlap to shield corners and outer leaves from dust. It is a light document protector rather than a hard case.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			90.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_wax_tablet_pouch",
			"pouch",
			"a wax-tablet pouch",
			null,
			"This wax-tablet pouch is a flat closable pouch shaped for folded documents and small writing supplies. Its body is flexible, with a flap or tucked closure that protects contents from dust and casual handling. It suits letters, seals, tablets, labels, or compact scribal tools.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			10.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_book_satchel",
			"satchel",
			"a leather book satchel",
			null,
			"This leather book satchel is a larger carried document bag with room for books, tablets, packets, and writing tools. The body is broad and flat so papers are not crushed into a bundle, with reinforced edges where it is handled most. It is meant for couriers, clerks, notaries, students, or travelling scribes.",
			SizeCategory.Normal,
			ItemQuality.Good,
			720.0,
			32.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Document_Satchel"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_document_satchel",
			"satchel",
			"a document satchel",
			null,
			"This document satchel is a larger carried document bag with room for books, tablets, packets, and writing tools. The body is broad and flat so papers are not crushed into a bundle, with reinforced edges where it is handled most. It is meant for couriers, clerks, notaries, students, or travelling scribes.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			640.0,
			28.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Document_Satchel"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_courier_document_satchel",
			"satchel",
			"a courier document satchel",
			null,
			"This courier document satchel is a larger carried document bag with room for books, tablets, packets, and writing tools. The body is broad and flat so papers are not crushed into a bundle, with reinforced edges where it is handled most. It is meant for couriers, clerks, notaries, students, or travelling scribes.",
			SizeCategory.Normal,
			ItemQuality.Good,
			760.0,
			40.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Document_Satchel"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_notary_satchel",
			"satchel",
			"a notary's document satchel",
			null,
			"This notary's document satchel is a larger carried document bag with room for books, tablets, packets, and writing tools. The body is broad and flat so papers are not crushed into a bundle, with reinforced edges where it is handled most. It is meant for couriers, clerks, notaries, students, or travelling scribes.",
			SizeCategory.Normal,
			ItemQuality.Good,
			820.0,
			48.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Document_Satchel"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_palm_leaf_wrapper",
			"wrapper",
			"a palm-leaf manuscript wrapper",
			null,
			"This palm-leaf manuscript wrapper is a soft protective covering made to fold around manuscripts, scrolls, or tablets. The textile lies flat but can be wrapped snugly, with enough overlap to shield corners and outer leaves from dust. It is a light document protector rather than a hard case.",
			SizeCategory.Small,
			ItemQuality.Standard,
			140.0,
			8.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_silk_manuscript_wrapper",
			"wrapper",
			"a silk manuscript wrapper",
			null,
			"This silk manuscript wrapper is a soft protective covering made to fold around manuscripts, scrolls, or tablets. The textile lies flat but can be wrapped snugly, with enough overlap to shield corners and outer leaves from dust. It is a light document protector rather than a hard case.",
			SizeCategory.Small,
			ItemQuality.Good,
			100.0,
			24.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_cypress_book_box",
			"box",
			"a cypress book box",
			null,
			"This cypress book box is a sturdy cypress storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1800.0,
			36.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Box"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_cedar_book_box",
			"box",
			"a cedar book box",
			null,
			"This cedar book box is a sturdy cedar storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1700.0,
			30.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Box"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_lacquered_book_box",
			"box",
			"a lacquered book box",
			null,
			"This lacquered book box is a sturdy lacquered storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1500.0,
			70.0m,
			true,
			false,
			"lacquer",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Box"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_small_archive_box",
			"box",
			"a small archive box",
			null,
			"This small archive box is a sturdy oak storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2600.0,
			34.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Box"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_large_archive_chest",
			"chest",
			"a large archive chest",
			null,
			"This large archive chest is a sturdy oak storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			120.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Chest"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_deed_box",
			"box",
			"a deed storage box",
			null,
			"This deed storage box is a sturdy walnut storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2400.0,
			44.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Box"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_scripture_box",
			"box",
			"a scripture storage box",
			null,
			"This scripture storage box is a sturdy cypress storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2100.0,
			58.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Box"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_seal_matrix_box",
			"box",
			"a seal matrix box",
			null,
			"This seal matrix box is a sturdy cedar storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			24.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Seal_Box"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_wax_and_seal_box",
			"box",
			"a wax and seal box",
			null,
			"This wax and seal box is a sturdy cedar storage piece made for documents, books, seals, or archive packets. It has a fitted lid, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1000.0,
			26.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Seal_Box"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_messenger_tube",
			"tube",
			"a messenger document tube",
			null,
			"This messenger document tube is a narrow bamboo container shaped to protect rolled documents. The long body is smooth enough not to abrade the scroll, with a fitted mouth, flap, or cap implied by its closed form. It is sized for scrolls, maps, rolls, labels, or other slender document bundles.",
			SizeCategory.Small,
			ItemQuality.Standard,
			320.0,
			14.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Scroll_Tube"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_courier_scroll_case",
			"case",
			"a courier scroll case",
			null,
			"This courier scroll case is a narrow leather container shaped to protect rolled documents. The long body is smooth enough not to abrade the scroll, with a fitted mouth, flap, or cap implied by its closed form. It is sized for scrolls, maps, rolls, labels, or other slender document bundles.",
			SizeCategory.Small,
			ItemQuality.Good,
			600.0,
			28.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Scroll_Tube"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_school_tablet_case",
			"case",
			"a school tablet case",
			null,
			"This school tablet case is a practical document container or protector made from wooden. Its shape is meant to keep writing goods grouped, covered, and easier to carry or shelve. It belongs with the storage side of scribal, courier, school, and archive work.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			12.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Document_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_document_bookcase_shelves",
			"shelves",
			"portable document shelves",
			null,
			"These portable document shelves are compact shelves made for books, scroll tubes, document boxes, and bundled records. The tiers are open and shallow, allowing stored documents to be seen and reached without digging through a chest. They are portable enough for the catalogue but visually suited to an archive, school, or scriptorium.",
			SizeCategory.Large,
			ItemQuality.Standard,
			7800.0,
			55.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Document_Bookcase_Shelves"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_locking_document_box",
			"box",
			"a locking document box",
			null,
			"This locking document box is a sturdy oak storage piece made for documents, books, seals, or archive packets. It has a built-in lock and keyhole, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
			SizeCategory.Small,
			ItemQuality.Good,
			3400.0,
			70.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_locking_deed_chest",
			"chest",
			"a locking deed chest",
			null,
			"This locking deed chest is a sturdy oak storage piece made for documents, books, seals, or archive packets. It has a built-in lock and keyhole, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
			SizeCategory.Normal,
			ItemQuality.Good,
			18000.0,
			180.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_locking_archive_chest",
			"chest",
			"a locking archive chest",
			null,
			"This locking archive chest is a sturdy oak storage piece made for documents, books, seals, or archive packets. It has a built-in lock and keyhole, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			220.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_chancery_strong_chest",
			"chest",
			"a chancery strong chest",
			null,
			"This chancery strong chest is a sturdy iron storage piece made for documents, books, seals, or archive packets. It has a built-in lock and keyhole, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
			SizeCategory.Large,
			ItemQuality.Good,
			62000.0,
			680.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_seal_office_lockbox",
			"lockbox",
			"a seal office lockbox",
			null,
			"This seal office lockbox is a sturdy brass storage piece made for documents, books, seals, or archive packets. It has a built-in lock and keyhole, reinforced corners, and a clean interior sized for protected records rather than loose household clutter. The form suits chancery, library, monastery, school, or merchant archive use.",
			SizeCategory.Small,
			ItemQuality.Good,
			4200.0,
			120.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_bronze_signet_stamp",
			"stamp",
			"a bronze signet stamp",
			null,
			"This bronze signet stamp is a compact seal tool with a shaped face cut for making an authority impression. The metal surface is worn bright along the high points, while the recessed design is kept sharp enough to press into wax or clay. It can serve personal, household, office, or messenger authentication.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			75.0,
			45.0m,
			true,
			false,
			"bronze",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Seal Stamp"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_BronzeSignet"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_brass_office_seal",
			"seal",
			"a brass office seal",
			null,
			"This brass office seal is a seal matrix with a flat working face and a solid grip or back. The design surface is kept clear of ornament that would blur the impression, while the body shows practical handling wear. It is used to mark wax, clay, or other sealing media with a recognizable authority sign.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			320.0,
			95.0m,
			true,
			false,
			"brass",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Seal Stamp"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_BrassOfficeSeal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_iron_seal_matrix",
			"matrix",
			"an iron seal matrix",
			null,
			"This iron seal matrix is a seal matrix with a flat working face and a solid grip or back. The design surface is kept clear of ornament that would blur the impression, while the body shows practical handling wear. It is used to mark wax, clay, or other sealing media with a recognizable authority sign.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			260.0,
			55.0m,
			true,
			false,
			"wrought iron",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Seal Stamp"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_IronSealMatrix"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_lead_seal_matrix",
			"matrix",
			"a lead seal matrix",
			null,
			"This lead seal matrix is a heavy seal matrix or bulla-related tool meant for formal packets and durable impressions. The working face is firm and plain-edged, with enough weight to leave a confident mark in a prepared medium. It suits official records, charters, rolls, and secured archive goods.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			380.0,
			65.0m,
			true,
			false,
			"lead",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Seal Stamp"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_LeadSealMatrix"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_ivory_handled_seal_stamp",
			"stamp",
			"an ivory-handled seal stamp",
			null,
			"This ivory-handled seal stamp is a seal matrix with a flat working face and a solid grip or back. The design surface is kept clear of ornament that would blur the impression, while the body shows practical handling wear. It is used to mark wax, clay, or other sealing media with a recognizable authority sign.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			120.0,
			120.0m,
			true,
			false,
			"ivory",
			[
				"Market / Writing Materials / Writing Implements",
				"Functions / Tools / Scribing Tools / Seal Stamp"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"SealStamp_Medieval_BronzeSignet"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_wax_seal_cake",
			"cake",
			"a wax seal cake",
			null,
			"This wax seal cake is a small lump or cake of wax kept for sealing documents and packets. The surface is smooth where it has been wrapped or warmed, and the colour reads dark enough to show an impressed design. It is ready to be softened, pressed, and marked by a seal stamp.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			55.0,
			4.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Writing Goods",
				"Market / Writing Materials / Document Containers",
				"Functions / Tools / Scribing Tools / Seal Stamp"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_red_wax_seal_cake",
			"cake",
			"a red wax seal cake",
			null,
			"This red wax seal cake is a small lump or cake of wax kept for sealing documents and packets. The surface is smooth where it has been wrapped or warmed, and the colour reads dark enough to show an impressed design. It is ready to be softened, pressed, and marked by a seal stamp.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			60.0,
			6.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Writing Goods",
				"Market / Writing Materials / Document Containers",
				"Functions / Tools / Scribing Tools / Seal Stamp"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_clay_sealing_lump",
			"lump",
			"a lump of sealing clay",
			null,
			"This lump of sealing clay is an authentication good associated with seals, secured documents, and official handling. Its surface or closure point is shaped to carry a mark, impression, tie, or visible authority sign. It belongs with chancery, archive, courier, mercantile, or institutional document work.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			120.0,
			1.0m,
			true,
			false,
			"clay",
			[
				"Functions / Writing Goods",
				"Market / Writing Materials / Document Containers",
				"Functions / Tools / Scribing Tools / Seal Stamp"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_lead_bulla_blank",
			"bulla",
			"a blank lead bulla",
			null,
			"This blank lead bulla is a heavy seal matrix or bulla-related tool meant for formal packets and durable impressions. The working face is firm and plain-edged, with enough weight to leave a confident mark in a prepared medium. It suits official records, charters, rolls, and secured archive goods.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			85.0,
			8.0m,
			true,
			false,
			"lead",
			[
				"Functions / Writing Goods",
				"Market / Writing Materials / Document Containers",
				"Functions / Tools / Scribing Tools / Seal Stamp"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_lead_seal_tag",
			"tag",
			"a lead seal tag",
			null,
			"This lead seal tag is a small tag prepared for a written mark and a seal impression. Its surface is compact but clear, with an attachment point for tying it to a bundle, chest, scroll, or packet. It is useful for archive control, messenger custody, and official identification.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			55.0,
			6.0m,
			true,
			false,
			"lead",
			[
				"Functions / Writing Goods",
				"Market / Writing Materials / Document Containers",
				"Functions / Tools / Scribing Tools / Seal Stamp"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_seal_cord",
			"cord",
			"a seal cord",
			null,
			"This seal cord is an authentication good associated with seals, secured documents, and official handling. Its surface or closure point is shaped to carry a mark, impression, tie, or visible authority sign. It belongs with chancery, archive, courier, mercantile, or institutional document work.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			12.0,
			1.0m,
			true,
			false,
			"linen",
			[
				"Functions / Writing Goods",
				"Market / Writing Materials / Document Containers",
				"Functions / Tools / Scribing Tools / Seal Stamp"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_sealable_parchment_charter",
			"charter",
			"a sealable parchment charter",
			null,
			"This sealable parchment charter is a formal document blank or packet prepared to receive a tamper-evident seal. The writing face is left clear while a fold, tail, cord, or lower margin is reserved for wax, clay, or lead authentication. It is suitable for official orders, contracts, letters, writs, and secured records.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			12.0,
			12.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface",
				"Sealable_Medieval_Parchment_Charter"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_sealed_parchment_roll",
			"roll",
			"a sealable parchment roll",
			null,
			"This sealable parchment roll is a rolled document form prepared with an outer tie or overlap for a seal. The outer layer protects the writing while presenting a clear place for wax, clay, or lead to hold the roll closed. It is suited to secured correspondence, charters, official copies, and archive transfers.",
			SizeCategory.Small,
			ItemQuality.Good,
			150.0,
			40.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Roll_Surface",
				"Sealable_Medieval_Parchment_Roll"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_sealable_rag_paper_letter",
			"letter",
			"a sealable rag-paper letter",
			null,
			"This sealable rag-paper letter is a formal document blank or packet prepared to receive a tamper-evident seal. The writing face is left clear while a fold, tail, cord, or lower margin is reserved for wax, clay, or lead authentication. It is suitable for official orders, contracts, letters, writs, and secured records.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			6.0,
			3.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Letter_Surface",
				"Sealable_Medieval_Rag_Paper_Letter"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_official_writ",
			"writ",
			"a sealable official writ",
			null,
			"This sealable official writ is a formal document blank or packet prepared to receive a tamper-evident seal. The writing face is left clear while a fold, tail, cord, or lower margin is reserved for wax, clay, or lead authentication. It is suitable for official orders, contracts, letters, writs, and secured records.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			10.0,
			14.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface",
				"Sealable_Medieval_Official_Writ"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_sealable_east_asian_scroll",
			"scroll",
			"a sealable paper handscroll",
			null,
			"This sealable paper handscroll is a rolled document form prepared with an outer tie or overlap for a seal. The outer layer protects the writing while presenting a clear place for wax, clay, or lead to hold the roll closed. It is suited to secured correspondence, charters, official copies, and archive transfers.",
			SizeCategory.Small,
			ItemQuality.Good,
			90.0,
			30.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_East_Asian_Paper_Scroll_Surface",
				"Sealable_Medieval_East_Asian_Scroll"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_sealable_palm_leaf_bundle",
			"bundle",
			"a sealable palm-leaf bundle",
			null,
			"This sealable palm-leaf bundle is a corded manuscript packet prepared so its tie can carry a seal. The long leaves are stacked neatly between covers or wraps, with the cord left accessible for authentication. It is meant to show whether the manuscript bundle has been opened since sealing.",
			SizeCategory.Small,
			ItemQuality.Good,
			460.0,
			62.0m,
			true,
			false,
			"leaf",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Palm_Leaf_Manuscript_Bundle",
				"Sealable_Medieval_Palm_Leaf_Bundle"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_wax_sealable_document",
			"document",
			"a wax-sealable document",
			null,
			"This wax-sealable document is a formal document blank or packet prepared to receive a tamper-evident seal. The writing face is left clear while a fold, tail, cord, or lower margin is reserved for wax, clay, or lead authentication. It is suitable for official orders, contracts, letters, writs, and secured records.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			8.0,
			9.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Sheet_Surface",
				"Sealable_Document_Wax"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_clay_sealable_document",
			"document",
			"a clay-sealable document",
			null,
			"This clay-sealable document is a formal document blank or packet prepared to receive a tamper-evident seal. The writing face is left clear while a fold, tail, cord, or lower margin is reserved for wax, clay, or lead authentication. It is suitable for official orders, contracts, letters, writs, and secured records.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			6.0,
			6.0m,
			true,
			false,
			"papyrus",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Papyrus_Sheet_Surface",
				"Sealable_Document_Clay"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_sealable_paper_envelope",
			"envelope",
			"a sealable paper envelope",
			null,
			"This sealable paper envelope is a formal document blank or packet prepared to receive a tamper-evident seal. The writing face is left clear while a fold, tail, cord, or lower margin is reserved for wax, clay, or lead authentication. It is suitable for official orders, contracts, letters, writs, and secured records.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			8.0,
			4.0m,
			true,
			false,
			"paper",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Rag_Paper_Letter_Surface",
				"Sealable_Envelope"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_sealable_scroll",
			"scroll",
			"a general sealable scroll",
			null,
			"This general sealable scroll is a rolled document form prepared with an outer tie or overlap for a seal. The outer layer protects the writing while presenting a clear place for wax, clay, or lead to hold the roll closed. It is suited to secured correspondence, charters, official copies, and archive transfers.",
			SizeCategory.Small,
			ItemQuality.Standard,
			130.0,
			28.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Medieval_Parchment_Roll_Surface",
				"Sealable_Scroll"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_sealable_document_pouch",
			"pouch",
			"a sealable document pouch",
			null,
			"This sealable document pouch is built with a closure point prepared to take a visible tamper seal. The flap, lid, or hasp gives a clear place for wax, clay, or lead to bind the opening after the contents are secured. It is intended for courier custody, archive storage, or official handling where broken sealing should be obvious.",
			SizeCategory.Small,
			ItemQuality.Good,
			280.0,
			18.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Document_Pouch",
				"Sealable_Medieval_Document_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_sealable_archive_box",
			"box",
			"a sealable archive box",
			null,
			"This sealable archive box is built with a closure point prepared to take a visible tamper seal. The flap, lid, or hasp gives a clear place for wax, clay, or lead to bind the opening after the contents are secured. It is intended for courier custody, archive storage, or official handling where broken sealing should be obvious.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2800.0,
			48.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Box",
				"Sealable_Medieval_Archive_Box"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_sealable_archive_chest",
			"chest",
			"a sealable archive chest",
			null,
			"This sealable archive chest is built with a closure point prepared to take a visible tamper seal. The flap, lid, or hasp gives a clear place for wax, clay, or lead to bind the opening after the contents are secured. It is intended for courier custody, archive storage, or official handling where broken sealing should be obvious.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			150.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Chest",
				"Sealable_Medieval_Archive_Box"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_wax_sealable_container",
			"box",
			"a wax-sealable document box",
			null,
			"This wax-sealable document box is built with a closure point prepared to take a visible tamper seal. The flap, lid, or hasp gives a clear place for wax, clay, or lead to bind the opening after the contents are secured. It is intended for courier custody, archive storage, or official handling where broken sealing should be obvious.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			42.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Market / Writing Materials / Document Containers",
				"Functions / Writing Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Archive_Box",
				"Sealable_Container_Wax"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_parchment_scraping_knife",
			"knife",
			"a parchment scraping knife",
			null,
			"This parchment scraping knife is a curved or narrow-edged tool for scraping wet hide into a smoother writing surface. The working edge is keen but controlled, with handling wear where pressure would be applied. It belongs in parchment preparation, especially thinning, smoothing, and cleaning skins on a frame.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			120.0,
			9.0m,
			true,
			false,
			"bronze",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Parchment Scraping Knife"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_parchment_lunellum",
			"lunellum",
			"a crescent parchment lunellum",
			null,
			"This crescent parchment lunellum is a curved or narrow-edged tool for scraping wet hide into a smoother writing surface. The working edge is keen but controlled, with handling wear where pressure would be applied. It belongs in parchment preparation, especially thinning, smoothing, and cleaning skins on a frame.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			180.0,
			14.0m,
			true,
			false,
			"wrought iron",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Parchment Lunellum"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_parchment_stretching_frame",
			"frame",
			"a parchment stretching frame",
			null,
			"This parchment stretching frame is part of the heavy preparation work for stretching and holding parchment stock. Its surfaces show practical wear from tension, damp hide, cord, and repeated workshop handling. It helps turn raw skin into a flatter, thinner, more reliable writing material.",
			SizeCategory.Large,
			ItemQuality.Standard,
			6200.0,
			22.0m,
			true,
			false,
			"wood",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Parchment Stretching Frame"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_parchment_lacing_cord",
			"cord",
			"a parchment lacing cord",
			null,
			"This parchment lacing cord is part of the heavy preparation work for stretching and holding parchment stock. Its surfaces show practical wear from tension, damp hide, cord, and repeated workshop handling. It helps turn raw skin into a flatter, thinner, more reliable writing material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			180.0,
			4.0m,
			true,
			false,
			"hemp",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Parchment Lacing Cord"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_parchment_pegs",
			"pegs",
			"a set of parchment pegs",
			null,
			"This set of parchment pegs is part of the heavy preparation work for stretching and holding parchment stock. Its surfaces show practical wear from tension, damp hide, cord, and repeated workshop handling. It helps turn raw skin into a flatter, thinner, more reliable writing material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			260.0,
			5.0m,
			true,
			false,
			"wood",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Parchment Pegs"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_parchment_fleshing_beam",
			"beam",
			"a parchment fleshing beam",
			null,
			"This parchment fleshing beam is part of the heavy preparation work for stretching and holding parchment stock. Its surfaces show practical wear from tension, damp hide, cord, and repeated workshop handling. It helps turn raw skin into a flatter, thinner, more reliable writing material.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9500.0,
			28.0m,
			true,
			false,
			"wood",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Parchment Fleshing Beam"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_parchment_whitening_chalk",
			"chalk",
			"a lump of whitening chalk",
			null,
			"This lump of whitening chalk is a finishing material for preparing parchment before ink work. It is dry, pale, abrasive, or powdery in the places meant to touch the writing surface. It helps smooth, whiten, dry, or degrease the page so writing sits cleanly.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			120.0,
			2.0m,
			true,
			false,
			"chalk",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Parchment Whitening Chalk"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_parchment_pumice",
			"pumice",
			"a piece of parchment pumice",
			null,
			"This piece of parchment pumice is a finishing material for preparing parchment before ink work. It is dry, pale, abrasive, or powdery in the places meant to touch the writing surface. It helps smooth, whiten, dry, or degrease the page so writing sits cleanly.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			55.0,
			3.0m,
			true,
			false,
			"pumice",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Parchment Pumice"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_parchment_pounce_bag",
			"bag",
			"a parchment pounce bag",
			null,
			"This parchment pounce bag is a finishing material for preparing parchment before ink work. It is dry, pale, abrasive, or powdery in the places meant to touch the writing surface. It helps smooth, whiten, dry, or degrease the page so writing sits cleanly.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			85.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Parchmentmaking Tools / Pounce Bag"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_papermaker_mould_deckle",
			"mould",
			"a mould and deckle",
			null,
			"This mould and deckle is a wooden papermaking frame with the removable edge needed to shape a sheet. The working face is broad and flat, meant to lift wet fibres evenly from a vat. It is central to forming handmade paper before pressing and drying.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1800.0,
			30.0m,
			true,
			false,
			"wood",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Mould and Deckle"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_papermaker_vat",
			"vat",
			"a papermaker's vat",
			null,
			"This papermaker's vat is a broad wooden vessel used for wet fibre and rag preparation. The interior is plain and work-worn, with room for soaking, beating, or stirring paper stock. It belongs to the messy production side of paper rather than to finished desk work.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			80.0m,
			true,
			false,
			"wood",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Papermaker's Vat"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_couching_blanket",
			"blanket",
			"a couching blanket",
			null,
			"This couching blanket is a thick absorbent textile used between wet paper sheets during pressing. Its surface is soft and slightly matted, meant to draw moisture away without tearing the new sheet. It is a papermaker's working material, not a household blanket.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			12.0m,
			true,
			false,
			"wool",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Couching Blanket"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_press_felt",
			"felt",
			"a papermaker's press felt",
			null,
			"This papermaker's press felt is a thick absorbent textile used between wet paper sheets during pressing. Its surface is soft and slightly matted, meant to draw moisture away without tearing the new sheet. It is a papermaker's working material, not a household blanket.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			720.0,
			12.0m,
			true,
			false,
			"felt",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Press Felt"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_lay_press",
			"press",
			"a papermaker's lay press",
			null,
			"This papermaker's lay press is a wooden pressure tool for flattening sheets, gatherings, or newly made paper stock. The frame and boards are heavy and plain, built to squeeze evenly rather than decorate a room. It is used where wet or newly assembled writing materials need to dry flat.",
			SizeCategory.Large,
			ItemQuality.Good,
			18000.0,
			90.0m,
			true,
			false,
			"wood",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Lay Press"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_rag_sorting_knife",
			"knife",
			"a rag-sorting knife",
			null,
			"This rag-sorting knife is a practical papermaking tool used in preparing, shaping, pressing, or finishing sheet stock. Its construction is workmanlike, with visible wear from wet fibres, sizing, pressure, or repeated handling. It belongs in a paper workshop rather than a finished writing kit.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			95.0,
			6.0m,
			true,
			false,
			"wrought iron",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Rag Sorting Knife"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_rag_beating_trough",
			"trough",
			"a rag-beating trough",
			null,
			"This rag-beating trough is a broad wooden vessel used for wet fibre and rag preparation. The interior is plain and work-worn, with room for soaking, beating, or stirring paper stock. It belongs to the messy production side of paper rather than to finished desk work.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12000.0,
			45.0m,
			true,
			false,
			"wood",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Rag Beating Trough"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_paper_sizing_brush",
			"brush",
			"a paper sizing brush",
			null,
			"This paper sizing brush is a broad working brush for applying sizing to paper or other writing stock. The handle is practical and the bristles are meant to spread liquid evenly over a sheet. It belongs to surface preparation before the material is ready to write on.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			120.0,
			8.0m,
			true,
			false,
			"wood",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Paper Sizing Brush"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_gelatine_sizing_pot",
			"pot",
			"a sizing pot",
			null,
			"This sizing pot is a workshop tool for making or finishing writing goods. It is plainly made from durable material and shows the form needed for repeated hand work. It belongs in a scriptorium, paper shop, bindery, or document workshop.",
			SizeCategory.Small,
			ItemQuality.Standard,
			850.0,
			10.0m,
			true,
			false,
			"ceramic",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Gelatine Sizing Pot"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_paper_burnishing_agate",
			"agate",
			"a paper burnishing agate",
			null,
			"This paper burnishing agate is a smooth hard stone used to burnish paper into a more polished writing face. The working surface is rounded and glossy from repeated rubbing. It is used after drying or sizing to reduce roughness and help ink sit cleanly.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			90.0,
			16.0m,
			true,
			false,
			"agate",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Paper Burnishing Agate"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_watermark_wire",
			"wire",
			"a watermark wire",
			null,
			"This watermark wire is a shaped brass wire intended to leave a light mark in handmade paper. The wire is thin, flexible, and careful rather than strong, with bends that form a simple device. It is fitted to papermaking equipment when a sheet needs an identifying watermark.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			20.0,
			10.0m,
			true,
			false,
			"brass",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Papermaking Tools / Watermark Wire"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_bookbinders_needle",
			"needle",
			"a bookbinder's needle",
			null,
			"This bookbinder's needle is a stout needle sized for thread, cord, and the thick work of manuscript binding. Its eye is broad enough for binding thread, and the point is made for guided holes rather than delicate cloth sewing. It is used to sew quires, endbands, covers, and support cords into a firm book structure.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			18.0,
			5.0m,
			true,
			false,
			"bronze",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Bookbinder's Needle"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_endband_needle",
			"needle",
			"an endband needle",
			null,
			"This endband needle is a stout needle sized for thread, cord, and the thick work of manuscript binding. Its eye is broad enough for binding thread, and the point is made for guided holes rather than delicate cloth sewing. It is used to sew quires, endbands, covers, and support cords into a firm book structure.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			14.0,
			5.0m,
			true,
			false,
			"bronze",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Endband Needle"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_bookbinders_punch",
			"punch",
			"a bookbinder's punch",
			null,
			"This bookbinder's punch is a short pointed tool for opening clean holes in quires, boards, and covers. The grip and point are made for steady pressure so sewing stations can line up without tearing the material. It belongs on a binding bench beside needles, cords, and presses.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			110.0,
			7.0m,
			true,
			false,
			"bronze",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Bookbinder's Punch"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_bookbinder_sewing_frame",
			"frame",
			"a bookbinder's sewing frame",
			null,
			"This bookbinder's sewing frame is a wooden frame for holding support cords while quires are sewn into a book block. The uprights and crosspieces are plain and practical, marked by cord tension and repeated adjustment. It keeps a manuscript spine orderly during the slow work of binding.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2600.0,
			34.0m,
			true,
			false,
			"wood",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Bookbinder's Sewing Frame"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_sewing_support_cords",
			"cords",
			"a set of sewing support cords",
			null,
			"This set of sewing support cords is prepared to support the sewing of a book spine. The hemp cords are firm, slightly rough, and cut to useful lengths for anchoring gatherings. Once worked into a binding, they help hold the quires together under the cover.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			150.0,
			6.0m,
			true,
			false,
			"hemp",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Sewing Support Cords"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_backing_hammer",
			"hammer",
			"a bookbinder's backing hammer",
			null,
			"This bookbinder's backing hammer is a compact iron hammer used for shaping and settling a book spine. The working face is smooth enough not to shred the leather or paper it strikes. It is a specialised binding tool for backing, rounding, and controlled pressure work.",
			SizeCategory.Small,
			ItemQuality.Good,
			620.0,
			16.0m,
			true,
			false,
			"wrought iron",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Backing Hammer"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_leather_paring_knife",
			"knife",
			"a leather paring knife",
			null,
			"This leather paring knife is a thin sharp knife for paring leather and trimming binding materials. The blade is shaped for shaving fine layers rather than cutting with force. It is used to thin cover leather, clean edges, and prepare flexible joints.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			90.0,
			10.0m,
			true,
			false,
			"wrought iron",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Leather Paring Knife"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_lying_press",
			"press",
			"a bookbinder's lying press",
			null,
			"This bookbinder's lying press is a wooden pressure tool for flattening sheets, gatherings, or newly made paper stock. The frame and boards are heavy and plain, built to squeeze evenly rather than decorate a room. It is used where wet or newly assembled writing materials need to dry flat.",
			SizeCategory.Large,
			ItemQuality.Good,
			11500.0,
			70.0m,
			true,
			false,
			"wood",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Lying Press"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_book_press",
			"press",
			"a wooden book press",
			null,
			"This wooden book press is a wooden pressure tool for flattening sheets, gatherings, or newly made paper stock. The frame and boards are heavy and plain, built to squeeze evenly rather than decorate a room. It is used where wet or newly assembled writing materials need to dry flat.",
			SizeCategory.Large,
			ItemQuality.Good,
			18000.0,
			110.0m,
			true,
			false,
			"wood",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Book Press"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_book_plough",
			"plough",
			"a bookbinder's plough",
			null,
			"This bookbinder's plough is a bookbinder's trimming tool guided against a press-held book block. The body is wooden with a cutting element set to shave the page edges gradually and evenly. It is used to square and neaten a bound manuscript's fore edge.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3200.0,
			48.0m,
			true,
			false,
			"wood",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Bookbinding Tools / Book Plough"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_scroll_roller_rod",
			"rod",
			"a scroll roller rod",
			null,
			"This scroll roller rod is a scrollmaking aid used to finish, label, tie, or protect rolled documents. It is small, practical, and shaped for contact with paper, parchment, cord, or wrapper rather than heavy carpentry. It belongs with the final preparation of scrolls and rolls before storage or delivery.",
			SizeCategory.Small,
			ItemQuality.Standard,
			90.0,
			4.0m,
			true,
			false,
			"cedar",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Scrollmaking Tools / Scroll Roller Rod"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_scroll_smoothing_stone",
			"stone",
			"a scroll smoothing stone",
			null,
			"This scroll smoothing stone is a scrollmaking aid used to finish, label, tie, or protect rolled documents. It is small, practical, and shaped for contact with paper, parchment, cord, or wrapper rather than heavy carpentry. It belongs with the final preparation of scrolls and rolls before storage or delivery.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			180.0,
			3.0m,
			true,
			false,
			"stone",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Scrollmaking Tools / Scroll Smoothing Stone"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_scroll_end_knob",
			"knob",
			"a scroll end knob",
			null,
			"This scroll end knob is a scrollmaking aid used to finish, label, tie, or protect rolled documents. It is small, practical, and shaped for contact with paper, parchment, cord, or wrapper rather than heavy carpentry. It belongs with the final preparation of scrolls and rolls before storage or delivery.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			45.0,
			3.0m,
			true,
			false,
			"wood",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Scrollmaking Tools / Scroll End Knob"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_scroll_tie_ribbon",
			"ribbon",
			"a scroll tie ribbon",
			null,
			"This scroll tie ribbon is a scrollmaking aid used to finish, label, tie, or protect rolled documents. It is small, practical, and shaped for contact with paper, parchment, cord, or wrapper rather than heavy carpentry. It belongs with the final preparation of scrolls and rolls before storage or delivery.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			8.0,
			0.6m,
			true,
			false,
			"linen",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Scrollmaking Tools / Scroll Tie Ribbon"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_scroll_seal_cord",
			"cord",
			"a scroll seal cord",
			null,
			"This scroll seal cord is a scrollmaking aid used to finish, label, tie, or protect rolled documents. It is small, practical, and shaped for contact with paper, parchment, cord, or wrapper rather than heavy carpentry. It belongs with the final preparation of scrolls and rolls before storage or delivery.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			12.0,
			1.0m,
			true,
			false,
			"linen",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Scrollmaking Tools / Scroll Seal Cord"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_scroll_label_tab",
			"tab",
			"a scroll label tab",
			null,
			"This scroll label tab is a scrollmaking aid used to finish, label, tie, or protect rolled documents. It is small, practical, and shaped for contact with paper, parchment, cord, or wrapper rather than heavy carpentry. It belongs with the final preparation of scrolls and rolls before storage or delivery.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			4.0,
			0.8m,
			true,
			false,
			"parchment",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Scrollmaking Tools / Scroll Label Tab"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_block_carving_knife",
			"knife",
			"a block carving knife",
			null,
			"This block carving knife is a small sharp tool for cutting lines into woodblock faces. The working edge is narrow and controlled, made for careful relief carving rather than heavy chopping. It belongs with block preparation, correction, and fine carving work before printing.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			85.0,
			8.0m,
			true,
			false,
			"wrought iron",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodblock Printing Tools / Block Carving Knife"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_block_clearing_chisel",
			"chisel",
			"a block clearing chisel",
			null,
			"This block clearing chisel is a small sharp tool for cutting lines into woodblock faces. The working edge is narrow and controlled, made for careful relief carving rather than heavy chopping. It belongs with block preparation, correction, and fine carving work before printing.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			110.0,
			8.0m,
			true,
			false,
			"wrought iron",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodblock Printing Tools / Block Clearing Chisel"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_woodblock_ink_dauber",
			"dauber",
			"a woodblock ink dauber",
			null,
			"This woodblock ink dauber is a tool or support good for East Asian copying and woodblock printing. Its form is practical, hand-sized, and intended for paper, ink, block faces, or drying sheets. It belongs in a manuscript-copying or block-printing workspace.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			120.0,
			6.0m,
			true,
			false,
			"bamboo",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodblock Printing Tools / Ink Dauber"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_paper_dampening_brush",
			"brush",
			"a paper dampening brush",
			null,
			"This paper dampening brush is a broad brush for spreading ink across a carved block or copying surface. The bristles are wide and flexible, meant to cover raised areas without gouging the wood. It is a practical printing tool rather than a fine writing brush.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			140.0,
			7.0m,
			true,
			false,
			"bamboo",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodblock Printing Tools / Paper Dampening Brush"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_printing_paste_pot",
			"pot",
			"a printing paste pot",
			null,
			"This printing paste pot is a tool or support good for East Asian copying and woodblock printing. Its form is practical, hand-sized, and intended for paper, ink, block faces, or drying sheets. It belongs in a manuscript-copying or block-printing workspace.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			300.0,
			8.0m,
			true,
			false,
			"ceramic",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodblock Printing Tools / Paste Pot"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_printing_baren",
			"baren",
			"a printing baren",
			null,
			"This printing baren is a hand rubbing pad used to press paper against an inked block. Its rounded face spreads pressure across the sheet without needing a heavy press. It belongs to brush-and-block printing, especially for small sheets and handscroll work.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			90.0,
			10.0m,
			true,
			false,
			"bamboo",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodblock Printing Tools / Printing Baren"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_impression_spoon",
			"spoon",
			"an impression spoon",
			null,
			"This impression spoon is a tool or support good for East Asian copying and woodblock printing. Its form is practical, hand-sized, and intended for paper, ink, block faces, or drying sheets. It belongs in a manuscript-copying or block-printing workspace.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			70.0,
			5.0m,
			true,
			false,
			"wood",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodblock Printing Tools / Impression Spoon"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_registration_pins",
			"pins",
			"a set of registration pins",
			null,
			"This set of registration pins is a simple guide used to align sheets, blocks, or repeated impressions. Its straight edges and marked positions help keep paper from drifting while copies are made. It supports more consistent woodblock printing and repeated document work.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			4.0m,
			true,
			false,
			"bamboo",
			[
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Woodblock Printing Tools / Registration Pin"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_blank_woodblock",
			"block",
			"a blank printing woodblock",
			null,
			"This blank printing woodblock is a smooth piece of wood left ready for carving into a printing surface. The grain is even enough for fine knife work, and the face has been planed flat before cutting begins. It is an unfinished but useful stock piece for block printing.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1500.0,
			12.0m,
			true,
			false,
			"wood",
			[
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_carved_text_block",
			"block",
			"a carved text woodblock",
			null,
			"This carved text woodblock is a flat wooden block prepared for relief printing. The face is smooth and even, with the working surface meant to carry carved text, lines, or image areas. It belongs to woodblock copying, where ink is applied to the raised design and transferred to paper.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			28.0m,
			true,
			false,
			"wood",
			[
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_sutra_printing_block",
			"block",
			"a sutra printing block",
			null,
			"This sutra printing block is a flat wooden block prepared for relief printing. The face is smooth and even, with the working surface meant to carry carved text, lines, or image areas. It belongs to woodblock copying, where ink is applied to the raised design and transferred to paper.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1700.0,
			36.0m,
			true,
			false,
			"wood",
			[
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_bamboo_printing_board",
			"board",
			"a bamboo printing board",
			null,
			"This bamboo printing board is a tool or support good for East Asian copying and woodblock printing. Its form is practical, hand-sized, and intended for paper, ink, block faces, or drying sheets. It belongs in a manuscript-copying or block-printing workspace.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			980.0,
			16.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_woodblock_proof_sheet",
			"sheet",
			"a woodblock proof sheet",
			null,
			"This woodblock proof sheet is a trial sheet or sample used to check the impression from a block or brush copy. The paper shows the kind of blank or test-ready surface expected before a run of finished copies. It is part of the checking and correction process around block printing.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			4.0,
			2.0m,
			true,
			false,
			"paper",
			[
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
			[
				"Holdable",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_printing_paper_stack",
			"stack",
			"a stack of printing paper",
			null,
			"This stack of printing paper is a tool or support good for East Asian copying and woodblock printing. Its form is practical, hand-sized, and intended for paper, ink, block faces, or drying sheets. It belongs in a manuscript-copying or block-printing workspace.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			24.0m,
			true,
			false,
			"paper",
			[
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools",
				"Functions / Writing Goods",
				"Materials / Writing Product",
				"Market / Writing Materials / Paper"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Stack_Number"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_inked_printing_pad",
			"pad",
			"an inked printing pad",
			null,
			"This inked printing pad is a tool or support good for East Asian copying and woodblock printing. Its form is practical, hand-sized, and intended for paper, ink, block faces, or drying sheets. It belongs in a manuscript-copying or block-printing workspace.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			180.0,
			10.0m,
			true,
			false,
			"linen",
			[
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_block_rubbing_pad",
			"pad",
			"a block rubbing pad",
			null,
			"This block rubbing pad is a tool or support good for East Asian copying and woodblock printing. Its form is practical, hand-sized, and intended for paper, ink, block faces, or drying sheets. It belongs in a manuscript-copying or block-printing workspace.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			160.0,
			8.0m,
			true,
			false,
			"linen",
			[
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_print_registration_frame",
			"frame",
			"a print registration frame",
			null,
			"This print registration frame is a simple guide used to align sheets, blocks, or repeated impressions. Its straight edges and marked positions help keep paper from drifting while copies are made. It supports more consistent woodblock printing and repeated document work.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			true,
			false,
			"wood",
			[
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_print_drying_line",
			"line",
			"a print drying line",
			null,
			"This print drying line is a light rack for holding freshly printed or brushed sheets while they dry. The supports keep paper separated so wet ink does not smear or transfer to the next leaf. It is useful in copying rooms, schools, and woodblock workshops.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			6.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Tools / Woodblock Printing Tools",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_scribes_sloped_board",
			"board",
			"a scribe's sloped board",
			null,
			"This scribe's sloped board provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			30.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_portable_lap_desk",
			"desk",
			"a portable lap desk",
			null,
			"This portable lap desk provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3200.0,
			42.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_writing_desk_drawers",
			"desk",
			"a small writing desk",
			null,
			"This small writing desk provides a stable surface for books, tablets, or loose sheets. The oak body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
			SizeCategory.Large,
			ItemQuality.Good,
			12000.0,
			85.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Drawers"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_copyists_board",
			"board",
			"a copyist's writing board",
			null,
			"This copyist's writing board provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2600.0,
			28.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_manuscript_rest",
			"rest",
			"a small manuscript rest",
			null,
			"This small manuscript rest provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1400.0,
			22.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_portable_book_stand",
			"stand",
			"a portable book stand",
			null,
			"This portable book stand provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2200.0,
			32.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_account_table_board",
			"board",
			"an account table board",
			null,
			"This account table board provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			20.0m,
			true,
			false,
			"wood",
			[
				"Functions / Writing Surface",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Medieval_Practice_Board_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_school_tablet_rack",
			"rack",
			"a school tablet rack",
			null,
			"This school tablet rack is a small portable storage support for manuscripts, scrolls, tablets, or document cases. The open structure keeps records upright or separated so they can be found quickly. It is suited to archive rooms, classrooms, scriptoria, or temporary workspaces.",
			SizeCategory.Large,
			ItemQuality.Standard,
			4200.0,
			28.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Narrow_Shelves"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_narrow_archive_tray",
			"tray",
			"a narrow archive tray",
			null,
			"This narrow archive tray is a shallow working tray sized for papers, seals, pens, or small document tools. Its raised edges keep small pieces from sliding away while leaving the contents visible and easy to sort. It is meant for desk work, archive sorting, sealing, or manuscript preparation.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			16.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Tray"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_scribe_tool_roll",
			"roll",
			"a scribe's tool roll",
			null,
			"This scribe's tool roll is a flexible tool roll with spaces for small writing implements. The material folds around pens, knives, brushes, or other narrow tools and can be tied into a compact bundle. It suits a travelling scribe, calligrapher, clerk, or copyist who needs portable equipment.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			16.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_calligrapher_tool_roll",
			"roll",
			"a calligrapher's tool roll",
			null,
			"This calligrapher's tool roll is a flexible tool roll with spaces for small writing implements. The material folds around pens, knives, brushes, or other narrow tools and can be tied into a compact bundle. It suits a travelling scribe, calligrapher, clerk, or copyist who needs portable equipment.",
			SizeCategory.Small,
			ItemQuality.Good,
			180.0,
			34.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Document_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_copyist_kit_box",
			"box",
			"a copyist's kit box",
			null,
			"This copyist's kit box is a compact storage box for writing tools, seals, ink goods, or manuscript supplies. Its interior is arranged for small working objects rather than general household storage, and the exterior is built for repeated handling. It belongs with clerks, copyists, messengers, and travelling scribes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1100.0,
			28.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Seal_Box"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_clerk_writing_box",
			"box",
			"a clerk's writing box",
			null,
			"This clerk's writing box is a compact storage box for writing tools, seals, ink goods, or manuscript supplies. Its interior is arranged for small working objects rather than general household storage, and the exterior is built for repeated handling. It belongs with clerks, copyists, messengers, and travelling scribes.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			52.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Drawers"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_travelling_scribe_box",
			"box",
			"a travelling scribe's box",
			null,
			"This travelling scribe's box is a compact storage box for writing tools, seals, ink goods, or manuscript supplies. Its interior is arranged for small working objects rather than general household storage, and the exterior is built for repeated handling. It belongs with clerks, copyists, messengers, and travelling scribes.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2900.0,
			46.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Drawers"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_scholars_book_board",
			"board",
			"a scholar's book board",
			null,
			"This scholar's book board provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2100.0,
			26.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_standing_copy_board",
			"board",
			"a standing copy board",
			null,
			"This standing copy board provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
			SizeCategory.Large,
			ItemQuality.Standard,
			5200.0,
			45.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Surface"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_document_sorting_tray",
			"tray",
			"a document sorting tray",
			null,
			"This document sorting tray is a shallow working tray sized for papers, seals, pens, or small document tools. Its raised edges keep small pieces from sliding away while leaving the contents visible and easy to sort. It is meant for desk work, archive sorting, sealing, or manuscript preparation.",
			SizeCategory.Small,
			ItemQuality.Standard,
			800.0,
			14.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Tray"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_seal_impression_tray",
			"tray",
			"a seal impression tray",
			null,
			"This seal impression tray is a shallow working tray sized for papers, seals, pens, or small document tools. Its raised edges keep small pieces from sliding away while leaving the contents visible and easy to sort. It is meant for desk work, archive sorting, sealing, or manuscript preparation.",
			SizeCategory.Small,
			ItemQuality.Good,
			850.0,
			32.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Tray"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_ink_and_pen_tray",
			"tray",
			"an ink and pen tray",
			null,
			"This ink and pen tray is a shallow working tray sized for papers, seals, pens, or small document tools. Its raised edges keep small pieces from sliding away while leaving the contents visible and easy to sort. It is meant for desk work, archive sorting, sealing, or manuscript preparation.",
			SizeCategory.Small,
			ItemQuality.Standard,
			600.0,
			12.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Tray"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_small_manuscript_shelf",
			"shelf",
			"a small manuscript shelf",
			null,
			"This small manuscript shelf is a small portable storage support for manuscripts, scrolls, tablets, or document cases. The open structure keeps records upright or separated so they can be found quickly. It is suited to archive rooms, classrooms, scriptoria, or temporary workspaces.",
			SizeCategory.Large,
			ItemQuality.Standard,
			5200.0,
			42.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Document_Bookcase_Shelves"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_portable_scroll_rack",
			"rack",
			"a portable scroll rack",
			null,
			"This portable scroll rack is a flexible tool roll with spaces for small writing implements. The material folds around pens, knives, brushes, or other narrow tools and can be tied into a compact bundle. It suits a travelling scribe, calligrapher, clerk, or copyist who needs portable equipment.",
			SizeCategory.Large,
			ItemQuality.Standard,
			4800.0,
			38.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Document_Bookcase_Shelves"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_wax_tablet_stand",
			"stand",
			"a wax-tablet stand",
			null,
			"This wax-tablet stand provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			18.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_writing_lectern_board",
			"board",
			"a portable lectern board",
			null,
			"This portable lectern board provides a stable surface for books, tablets, or loose sheets. The wooden body is plain and work-worn, shaped to hold a page at a useful angle or keep it steady while writing. It belongs on a scholar's bench, clerk's table, school desk, or travelling scribe's setup.",
			SizeCategory.Large,
			ItemQuality.Good,
			3600.0,
			40.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container",
				"Functions / Writing Goods",
				"Functions / Household Items / Household Wares",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Writing_Desk_Surface"
			],
			null,
			null,
			null,
			null
		);
	}
}
