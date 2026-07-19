#nullable enable

using MudSharp.GameItems;
using System;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedPreIndustrialWritingAdministrationAndDocuments()
	{
		CreatePreIndustrialAlias(
					"medieval_writing_blank_parchment_charter",
					"preindustrial_writing_blank_parchment_charter",
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

		CreatePreIndustrialAlias(
					"medieval_writing_blank_parchment_writ",
					"preindustrial_writing_blank_parchment_writ",
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

		CreatePreIndustrialAlias(
					"medieval_writing_bone_stylus",
					"preindustrial_writing_bone_stylus",
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

		CreatePreIndustrialAlias(
					"medieval_writing_broad_reed_pen",
					"preindustrial_writing_broad_reed_pen",
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

		CreatePreIndustrialAlias(
					"medieval_writing_bronze_stylus",
					"preindustrial_writing_bronze_stylus",
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

		CreatePreIndustrialAlias(
					"medieval_writing_charcoal_stick",
					"preindustrial_writing_charcoal_stick",
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

		CreatePreIndustrialAlias(
					"medieval_writing_east_asian_brush",
					"preindustrial_writing_east_asian_brush",
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

		CreatePreIndustrialAlias(
					"medieval_writing_estate_account_ledger",
					"preindustrial_writing_estate_account_ledger",
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

		CreatePreIndustrialAlias(
					"medieval_writing_fine_calligraphy_brush",
					"preindustrial_writing_fine_calligraphy_brush",
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

		CreatePreIndustrialAlias(
					"medieval_writing_fine_parchment_bifolium",
					"preindustrial_writing_fine_parchment_bifolium",
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

		CreatePreIndustrialAlias(
					"medieval_writing_fine_parchment_sheet",
					"preindustrial_writing_fine_parchment_sheet",
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

		CreatePreIndustrialAlias(
					"medieval_writing_fine_qalam",
					"preindustrial_writing_fine_qalam",
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

		CreatePreIndustrialAlias(
					"medieval_writing_fine_quill_pen",
					"preindustrial_writing_fine_quill_pen",
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

		CreatePreIndustrialAlias(
					"medieval_writing_fine_rag_paper_sheet",
					"preindustrial_writing_fine_rag_paper_sheet",
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

		CreatePreIndustrialAlias(
					"medieval_writing_folded_rag_paper_letter",
					"preindustrial_writing_folded_rag_paper_letter",
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

		CreatePreIndustrialAlias(
					"medieval_writing_goose_quill_pen",
					"preindustrial_writing_goose_quill_pen",
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

		CreatePreIndustrialAlias(
					"medieval_writing_iron_stylus",
					"preindustrial_writing_iron_stylus",
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

		CreatePreIndustrialAlias(
					"medieval_writing_ivory_stylus",
					"preindustrial_writing_ivory_stylus",
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

		CreatePreIndustrialAlias(
					"medieval_writing_labeling_brush",
					"preindustrial_writing_labeling_brush",
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

		CreatePreIndustrialAlias(
					"medieval_writing_large_parchment_codex",
					"preindustrial_writing_large_parchment_codex",
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

		CreatePreIndustrialAlias(
					"medieval_writing_merchant_account_ledger",
					"preindustrial_writing_merchant_account_ledger",
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

		CreatePreIndustrialAlias(
					"medieval_writing_parchment_deed_sheet",
					"preindustrial_writing_parchment_deed_sheet",
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

		CreatePreIndustrialAlias(
					"medieval_writing_parchment_label_tag",
					"preindustrial_writing_parchment_label_tag",
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

		CreatePreIndustrialAlias(
					"medieval_writing_plain_parchment_codex",
					"preindustrial_writing_plain_parchment_codex",
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

		CreatePreIndustrialAlias(
					"medieval_writing_plain_parchment_sheet",
					"preindustrial_writing_plain_parchment_sheet",
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

		CreatePreIndustrialAlias(
					"medieval_writing_plain_quill_pen",
					"preindustrial_writing_plain_quill_pen",
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

		CreatePreIndustrialAlias(
					"medieval_writing_plain_rag_paper_sheet",
					"preindustrial_writing_plain_rag_paper_sheet",
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

		CreatePreIndustrialAlias(
					"medieval_writing_qalam",
					"preindustrial_writing_qalam",
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

		CreatePreIndustrialAlias(
					"medieval_writing_rag_paper_account_slip",
					"preindustrial_writing_rag_paper_account_slip",
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

		CreatePreIndustrialAlias(
					"medieval_writing_rag_paper_codex",
					"preindustrial_writing_rag_paper_codex",
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

		CreatePreIndustrialAlias(
					"medieval_writing_rag_paper_petition_leaf",
					"preindustrial_writing_rag_paper_petition_leaf",
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

		CreatePreIndustrialAlias(
					"medieval_writing_reed_stylus",
					"preindustrial_writing_reed_stylus",
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

		CreatePreIndustrialAlias(
					"medieval_writing_ruled_parchment_sheet",
					"preindustrial_writing_ruled_parchment_sheet",
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

		CreatePreIndustrialAlias(
					"medieval_writing_ruled_rag_paper_sheet",
					"preindustrial_writing_ruled_rag_paper_sheet",
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

		CreatePreIndustrialAlias(
					"medieval_writing_scraped_parchment_bifolium",
					"preindustrial_writing_scraped_parchment_bifolium",
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

		CreatePreIndustrialAlias(
					"medieval_writing_small_parchment_codex",
					"preindustrial_writing_small_parchment_codex",
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

		CreatePreIndustrialAlias(
					"medieval_writing_swan_quill_pen",
					"preindustrial_writing_swan_quill_pen",
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

		CreatePreIndustrialAlias(
					"medieval_writing_toll_ledger",
					"preindustrial_writing_toll_ledger",
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

		CreatePreIndustrialAlias(
					"medieval_writing_trimmed_reed_pen",
					"preindustrial_writing_trimmed_reed_pen",
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

		SeedPreIndustrialPrintingAndPaperAdministrationItems();
	}

	private void SeedPreIndustrialTradeContainers()
	{
		CreatePreIndustrialAlias(
					"medieval_trade_apothecary_ingredient_box",
					"preindustrial_trade_apothecary_ingredient_box",
					"box",
					"an apothecary ingredient box",
					null,
					"This apothecary ingredient box is a medium-sized, well-made box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Good,
					3200.0,
					54.0m,
					true,
					false,
					"cedar",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Lidded box for labelled packets, dried roots, gum resins, minerals, and other apothecary stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_armorer_rivet_box",
					"preindustrial_trade_armorer_rivet_box",
					"box",
					"an armourer's rivet box",
					null,
					"This armourer's rivet box is a small, workmanlike box built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Standard,
					2100.0,
					20.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Small dense box for rivets, buckles, rings, plates, and armour-work fittings."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_ash_fruit_crate",
					"preindustrial_trade_ash_fruit_crate",
					"crate",
					"an ash fruit crate",
					null,
					"This ash fruit crate is a medium-sized, workmanlike crate built from ash boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					2200.0,
					18.0m,
					true,
					false,
					"ash",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Light rigid crate for orchard fruit, delicate vegetables, or packed table produce."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_ash_lime_bin",
					"preindustrial_trade_ash_lime_bin",
					"bin",
					"an ash lime bin",
					null,
					"This ash lime bin is a large, workmanlike bin built from ash boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					7800.0,
					26.0m,
					true,
					false,
					"ash",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Dry workshop bin for lime, chalk, ash, sand, or powdered construction material."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_ash_tool_box",
					"preindustrial_trade_ash_tool_box",
					"box",
					"an ash tool box",
					null,
					"This ash tool box is a medium-sized, workmanlike box built from ash boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					3800.0,
					26.0m,
					true,
					false,
					"ash",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Sturdy box for trade tools, fittings, measures, and workshop pieces."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_balance_weight_box",
					"preindustrial_trade_balance_weight_box",
					"box",
					"a balance-weight box",
					null,
					"This balance-weight box is a small, well-made box worked from bronze. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
					SizeCategory.Small,
					ItemQuality.Good,
					2200.0,
					60.0m,
					true,
					false,
					"bronze",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_HeavyMetal",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Small sturdy box for balance weights, small measures, and official metal standards."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_bamboo_cargo_basket",
					"preindustrial_trade_bamboo_cargo_basket",
					"basket",
					"a bamboo cargo basket",
					null,
					"This bamboo cargo basket is a large, workmanlike basket built from split bamboo. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					1400.0,
					16.0m,
					true,
					false,
					"bamboo",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Large open cargo basket for light bulky goods, market produce, or caravan packing."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_bamboo_storage_tube",
					"preindustrial_trade_bamboo_storage_tube",
					"tube",
					"a large bamboo storage tube",
					null,
					"This large bamboo storage tube is a medium-sized, workmanlike tube built from split bamboo. The body is rigid and narrow, with a capped end and a smooth outer surface. The edges are fitted closely to protect what is carried inside. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					1100.0,
					14.0m,
					true,
					false,
					"bamboo",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Light rigid tube for scrolls, tea, spice packets, arrows, or narrow dry cargo."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_bamboo_tea_tube",
					"preindustrial_trade_bamboo_tea_tube",
					"tube",
					"a bamboo tea tube",
					null,
					"This bamboo tea tube is a small, well-made tube built from split bamboo. The body is rigid and narrow, with a capped end and a smooth outer surface. The edges are fitted closely to protect what is carried inside. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Good,
					420.0,
					20.0m,
					true,
					false,
					"bamboo",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Small rigid tube for tea, powders, incense sticks, and delicate sample goods."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_bazaar_cash_box",
					"preindustrial_trade_bazaar_cash_box",
					"lockbox",
					"a bazaar cash lockbox",
					null,
					"This bazaar cash lockbox is a small, well-made lockbox worked from brass. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
					SizeCategory.Small,
					ItemQuality.Good,
					4200.0,
					135.0m,
					true,
					false,
					"brass",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
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
					null,
					"Metal lockbox for shop counters and market stalls."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_bean_sack",
					"preindustrial_trade_bean_sack",
					"sack",
					"a dried bean sack",
					null,
					"This dried bean sack is a medium-sized, workmanlike sack made from woven hemp. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					720.0,
					5.0m,
					true,
					false,
					"hemp",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Plain sack for beans, peas, lentils, nuts, and similar loose foodstuffs."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_beech_nail_box",
					"preindustrial_trade_beech_nail_box",
					"box",
					"a beech nail box",
					null,
					"This beech nail box is a small, workmanlike box built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Standard,
					1900.0,
					18.0m,
					true,
					false,
					"beech",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Small rigid box for nails, tacks, rivets, buckles, or other dense hardware."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_biscuit_barrel",
					"preindustrial_trade_biscuit_barrel",
					"barrel",
					"a biscuit barrel",
					null,
					"This biscuit barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					12500.0,
					34.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Dry barrel for ship biscuit, hard bread, dried food, and travel rations."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_blacksmith_scrap_bin",
					"preindustrial_trade_blacksmith_scrap_bin",
					"bin",
					"a blacksmith's scrap bin",
					null,
					"This blacksmith's scrap bin is a medium-sized, workmanlike bin worked from wrought iron. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					11000.0,
					48.0m,
					true,
					false,
					"wrought iron",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_HeavyMetal",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Heavy open bin for iron scraps, broken fittings, failed nails, and forge returns."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_bolt_cloth_chest",
					"preindustrial_trade_bolt_cloth_chest",
					"chest",
					"a bolt-cloth chest",
					null,
					"This bolt-cloth chest is a large, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					9800.0,
					46.0m,
					true,
					false,
					"pine",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Trunk"
					],
					null,
					null,
					null,
					null,
					"Long chest for bolts of cloth, folded garments, tapestries, and shop textiles."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_bowyer_sinew_box",
					"preindustrial_trade_bowyer_sinew_box",
					"box",
					"a bowyer's sinew box",
					null,
					"This bowyer's sinew box is a small, workmanlike box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Standard,
					1200.0,
					18.0m,
					true,
					false,
					"cedar",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Dry box for sinew, horn slivers, bindings, glue cakes, and small bowmaking materials."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_brass_spice_canister",
					"preindustrial_trade_brass_spice_canister",
					"canister",
					"a brass spice canister",
					null,
					"This brass spice canister is a small, well-made canister worked from brass. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
					SizeCategory.Small,
					ItemQuality.Good,
					1100.0,
					46.0m,
					true,
					false,
					"brass",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_HeavyMetal",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Metal canister for dry spices, aromatics, incense, and compact luxury goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_bronze_ingot_tray",
					"preindustrial_trade_bronze_ingot_tray",
					"tray",
					"a bronze ingot tray",
					null,
					"This bronze ingot tray is a small, well-made tray worked from bronze. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
					SizeCategory.Small,
					ItemQuality.Good,
					2600.0,
					56.0m,
					true,
					false,
					"bronze",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_HeavyMetal",
						"Container_Tray"
					],
					null,
					null,
					null,
					null,
					"Low tray for ingots, weighed billets, coin blanks, or dense trade samples."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_bronze_trade_canister",
					"preindustrial_trade_bronze_trade_canister",
					"canister",
					"a bronze trade canister",
					null,
					"This bronze trade canister is a small, well-made canister worked from bronze. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
					SizeCategory.Small,
					ItemQuality.Good,
					1300.0,
					42.0m,
					true,
					false,
					"bronze",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_HeavyMetal",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Durable canister for samples, powders, beads, and valuable dry trade goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_butcher_salt_bin",
					"preindustrial_trade_butcher_salt_bin",
					"bin",
					"a butcher's salt bin",
					null,
					"This butcher's salt bin is a large, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					8200.0,
					30.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Market or workshop bin for coarse salt, curing mix, and dry butcher's stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_candle_stock_crate",
					"preindustrial_trade_candle_stock_crate",
					"crate",
					"a candle-stock crate",
					null,
					"This candle-stock crate is a medium-sized, workmanlike crate built from pine boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					2100.0,
					16.0m,
					true,
					false,
					"pine",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Open crate for candles, wax blocks, wick bundles, and lighting stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_canvas_flour_sack",
					"preindustrial_trade_canvas_flour_sack",
					"sack",
					"a canvas flour sack",
					null,
					"This canvas flour sack is a medium-sized, workmanlike sack made from coarse canvas. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					820.0,
					6.0m,
					true,
					false,
					"canvas",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Tighter woven sack for flour and sifted meal; still a dry container rather than a liquid vessel."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_canvas_salt_sack",
					"preindustrial_trade_canvas_salt_sack",
					"sack",
					"a doubled canvas salt sack",
					null,
					"This doubled canvas salt sack is a medium-sized, workmanlike sack made from coarse canvas. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					920.0,
					8.0m,
					true,
					false,
					"canvas",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Heavier dry-goods sack intended for salt, alum, ash, and similarly abrasive bulk materials."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_caravan_cargo_chest",
					"preindustrial_trade_caravan_cargo_chest",
					"chest",
					"a caravan cargo chest",
					null,
					"This caravan cargo chest is a large, well-made chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Good,
					12500.0,
					78.0m,
					true,
					false,
					"cedar",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Trunk"
					],
					null,
					null,
					null,
					null,
					"Travel chest for caravan goods, cloth bales, spices, medicines, and packed valuables."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_caravan_pay_chest",
					"preindustrial_trade_caravan_pay_chest",
					"chest",
					"a caravan pay chest",
					null,
					"This caravan pay chest is a medium-sized, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Good,
					22000.0,
					270.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"LockingContainer_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Locking pay chest for caravans, guards, and merchants on the road."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_cedar_merchant_casket",
					"preindustrial_trade_cedar_merchant_casket",
					"casket",
					"a cedar merchant casket",
					null,
					"This cedar merchant casket is a small, well-made casket built from cedar boards. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Good,
					3000.0,
					115.0m,
					true,
					false,
					"cedar",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"LockingContainer_Lockbox"
					],
					null,
					null,
					null,
					null,
					"Small locking casket for seals, paper money substitutes, tallies, or compact valuables."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_cedar_spice_box",
					"preindustrial_trade_cedar_spice_box",
					"box",
					"a cedar spice box",
					null,
					"This cedar spice box is a small, well-made box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Good,
					1400.0,
					30.0m,
					true,
					false,
					"cedar",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Aromatic lidded box for spices, incense, resins, or fragrant merchant stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_cedar_trade_tub",
					"preindustrial_trade_cedar_trade_tub",
					"tub",
					"a cedar trade tub",
					null,
					"This cedar trade tub is a large, well-made tub built from cedar boards. Staves form a broad open vessel with a flat bottom and a thick rim. The sides flare slightly, leaving the inside easy to reach. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Good,
					7000.0,
					52.0m,
					true,
					false,
					"cedar",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Well-made dry tub for aromatic goods, textiles, small parcels, or shop stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_ceramic_spice_jar",
					"preindustrial_trade_ceramic_spice_jar",
					"jar",
					"a ceramic spice jar",
					null,
					"This ceramic spice jar is a small, well-made jar formed from ceramic. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
					SizeCategory.Small,
					ItemQuality.Good,
					900.0,
					22.0m,
					true,
					false,
					"ceramic",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Glassware",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Small lidded jar for spice, dyestuff, medicines, or expensive powders."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_charcoal_barrel",
					"preindustrial_trade_charcoal_barrel",
					"barrel",
					"a charcoal barrel",
					null,
					"This charcoal barrel is a large, workmanlike barrel built from elm boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					9000.0,
					24.0m,
					true,
					false,
					"elm",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Dry barrel for charcoal, lampblack, powdered fuel, or dirty workshop goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_charcoal_sack",
					"preindustrial_trade_charcoal_sack",
					"sack",
					"a blackened charcoal sack",
					null,
					"This blackened charcoal sack is a medium-sized, workmanlike sack made from woven hemp. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					680.0,
					5.0m,
					true,
					false,
					"hemp",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Rough sack intended for charcoal, coke-like fuel, firewood chips, or dirty workshop stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_clay_lined_dyers_bin",
					"preindustrial_trade_clay_lined_dyers_bin",
					"bin",
					"a clay-lined dyer's bin",
					null,
					"This clay-lined dyer's bin is a large, well-made bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Good,
					9800.0,
					42.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Open bin with a protective lining for dry dyestuffs, mordants, wool lots, or stained workshop materials."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_clerk_coin_lockbox",
					"preindustrial_trade_clerk_coin_lockbox",
					"lockbox",
					"a clerk's coin lockbox",
					null,
					"This clerk's coin lockbox is a small, workmanlike lockbox built from ash boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Standard,
					2600.0,
					52.0m,
					true,
					false,
					"ash",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"LockingContainer_Lockbox"
					],
					null,
					null,
					null,
					null,
					"Small built-in-lock box for coin, tallies, seals, or market takings."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_cloth_merchant_coffer",
					"preindustrial_trade_cloth_merchant_coffer",
					"coffer",
					"a cloth merchant's coffer",
					null,
					"This cloth merchant's coffer is a medium-sized, well-made coffer built from walnut boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Good,
					17000.0,
					230.0m,
					true,
					false,
					"walnut",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"LockingContainer_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Locking coffer for bolts of fine cloth, sample books, and trade accounts."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_coin_counting_box",
					"preindustrial_trade_coin_counting_box",
					"box",
					"a coin-counting box",
					null,
					"This coin-counting box is a small, well-made box built from walnut boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Good,
					1700.0,
					40.0m,
					true,
					false,
					"walnut",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Compartmented box for counted coin parcels, tallies, counters, and small weights."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_cooper_stave_bin",
					"preindustrial_trade_cooper_stave_bin",
					"bin",
					"a cooper's stave bin",
					null,
					"This cooper's stave bin is a large, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					6900.0,
					24.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Open bin for barrel staves, hoops, offcuts, wedges, and cooperage stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_cotton_sample_bag",
					"preindustrial_trade_cotton_sample_bag",
					"bag",
					"a cotton sample bag",
					null,
					"This cotton sample bag is a small, workmanlike bag made from woven cotton. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Small,
					ItemQuality.Standard,
					160.0,
					7.0m,
					true,
					false,
					"cotton",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Tote"
					],
					null,
					null,
					null,
					null,
					"Light sample bag for cloth swatches, herb lots, bead packets, or small market wares."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_customs_chest",
					"preindustrial_trade_customs_chest",
					"chest",
					"a customs strong chest",
					null,
					"This customs strong chest is a medium-sized, well-made chest built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Good,
					20000.0,
					250.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"LockingContainer_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Locking chest for customs duties, sealed permits, and cargo records."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_cypress_export_tub",
					"preindustrial_trade_cypress_export_tub",
					"tub",
					"a cypress export tub",
					null,
					"This cypress export tub is a large, workmanlike tub built from cypress boards. Staves form a broad open vessel with a flat bottom and a thick rim. The sides flare slightly, leaving the inside easy to reach. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					7200.0,
					38.0m,
					true,
					false,
					"cypress",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Open tub for dry export goods, packed produce, or workshop stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_deep_willow_hamper",
					"preindustrial_trade_deep_willow_hamper",
					"hamper",
					"a deep willow hamper",
					null,
					"This deep willow hamper is a large, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					1600.0,
					14.0m,
					true,
					false,
					"willow",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Tall open hamper for bulky goods, fleece, bread loaves, vegetables, or laundry-like trade bundles."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_dockside_bond_chest",
					"preindustrial_trade_dockside_bond_chest",
					"chest",
					"a dockside bond chest",
					null,
					"This dockside bond chest is a medium-sized, well-made chest built from cypress boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Good,
					21000.0,
					270.0m,
					true,
					false,
					"cypress",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"LockingContainer_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Locking chest for port bonds, tally sticks, and cargo records."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_dyers_pigment_box",
					"preindustrial_trade_dyers_pigment_box",
					"box",
					"a dyer's pigment box",
					null,
					"This dyer's pigment box is a medium-sized, workmanlike box built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					3500.0,
					30.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Dry box for powdered pigments, dye cakes, mordant packets, and stained trade materials."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_dyestuff_barrel",
					"preindustrial_trade_dyestuff_barrel",
					"barrel",
					"a dyestuff barrel",
					null,
					"This dyestuff barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					11000.0,
					44.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Coopered barrel for dry dyestuff cakes, powdered pigment, mordants, or stained stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_dyestuff_sachet",
					"preindustrial_trade_dyestuff_sachet",
					"sachet",
					"a dyestuff sachet",
					null,
					"This dyestuff sachet is a very small, workmanlike sachet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.VerySmall,
					ItemQuality.Standard,
					80.0,
					4.0m,
					true,
					false,
					"linen",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sachet"
					],
					null,
					null,
					null,
					null,
					"Small packet for powdered dye, mordant, ground pigment, or marked sample material."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_earthenware_seed_jar",
					"preindustrial_trade_earthenware_seed_jar",
					"jar",
					"an earthenware seed jar",
					null,
					"This earthenware seed jar is a medium-sized, workmanlike jar formed from earthenware. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					3200.0,
					14.0m,
					true,
					false,
					"earthenware",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Glassware",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Dry jar for seed, dried herbs, pigments, or measured household trade stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_egg_crate",
					"preindustrial_trade_egg_crate",
					"crate",
					"a straw-lined egg crate",
					null,
					"This straw-lined egg crate is a small, workmanlike crate built from pine boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Standard,
					900.0,
					10.0m,
					true,
					false,
					"pine",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Small compartmented crate for eggs, fragile cheeses, small pots, or similarly delicate lots."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_elm_charcoal_bin",
					"preindustrial_trade_elm_charcoal_bin",
					"bin",
					"an elm charcoal bin",
					null,
					"This elm charcoal bin is a large, workmanlike bin built from elm boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					7600.0,
					24.0m,
					true,
					false,
					"elm",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Blackened open bin for charcoal, fuel lumps, ash, or forge-ready stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_elm_sealed_chest",
					"preindustrial_trade_elm_sealed_chest",
					"chest",
					"an elm sealed chest",
					null,
					"This elm sealed chest is a large, workmanlike chest built from elm boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					9800.0,
					54.0m,
					true,
					false,
					"elm",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Trunk"
					],
					null,
					null,
					null,
					null,
					"Large chest for sealable but not built-in-lock cargo, merchant stores, or toll goods."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_fair_takings_lockbox",
					"preindustrial_trade_fair_takings_lockbox",
					"lockbox",
					"a fair-takings lockbox",
					null,
					"This fair-takings lockbox is a small, workmanlike lockbox built from ash boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Standard,
					2800.0,
					62.0m,
					true,
					false,
					"ash",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"LockingContainer_Lockbox"
					],
					null,
					null,
					null,
					null,
					"Portable lockbox for fair, booth, or stall takings."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_fletcher_feather_box",
					"preindustrial_trade_fletcher_feather_box",
					"box",
					"a fletcher's feather box",
					null,
					"This fletcher's feather box is a small, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Standard,
					950.0,
					14.0m,
					true,
					false,
					"pine",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Light lidded box for feathers, bindings, points, and delicate fletching stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_flour_barrel",
					"preindustrial_trade_flour_barrel",
					"barrel",
					"a flour barrel",
					null,
					"This flour barrel is a large, workmanlike barrel built from pine boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					12000.0,
					32.0m,
					true,
					false,
					"pine",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Dry coopered barrel for flour, meal, bran, or similar milled goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_folded_cloth_bale",
					"preindustrial_trade_folded_cloth_bale",
					"bale",
					"a folded cloth bale",
					null,
					"This folded cloth bale is a large, workmanlike bale made from coarse canvas. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Large,
					ItemQuality.Standard,
					1600.0,
					12.0m,
					true,
					false,
					"canvas",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Rope-tied bale wrapper for folded bolts, offcuts, and merchant cloth lots."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_glassmaker_cullet_box",
					"preindustrial_trade_glassmaker_cullet_box",
					"box",
					"a glassmaker's cullet box",
					null,
					"This glassmaker's cullet box is a medium-sized, workmanlike box built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					3400.0,
					24.0m,
					true,
					false,
					"beech",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Rigid box for broken glass, cullet, frit, colourants, and other glasshouse stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_glassware_packing_crate",
					"preindustrial_trade_glassware_packing_crate",
					"crate",
					"a glassware packing crate",
					null,
					"This glassware packing crate is a large, well-made crate built from beech boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Good,
					4200.0,
					34.0m,
					true,
					false,
					"beech",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Better-made open crate with dividers or packing space for fragile glass and glazed wares."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_grain_cask",
					"preindustrial_trade_grain_cask",
					"cask",
					"a grain cask",
					null,
					"This grain cask is a large, workmanlike cask built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					15000.0,
					42.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Drum"
					],
					null,
					null,
					null,
					null,
					"Large coopered cask for bulk grain, malt, pulse, or similar dry stores."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_guild_strong_chest",
					"preindustrial_trade_guild_strong_chest",
					"chest",
					"a guild strong chest",
					null,
					"This guild strong chest is a large, well-made chest built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Good,
					42000.0,
					460.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
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
					null,
					"Heavy institutional chest for guild funds, charters, and records."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_hemp_grain_sack",
					"preindustrial_trade_hemp_grain_sack",
					"sack",
					"a hemp grain sack",
					null,
					"This hemp grain sack is a medium-sized, workmanlike sack made from woven hemp. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					760.0,
					5.0m,
					true,
					false,
					"hemp",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Sturdy porous sack for grain, beans, malt, or other dry bulk provisions."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_hemp_scrap_sack",
					"preindustrial_trade_hemp_scrap_sack",
					"sack",
					"a coarse hemp scrap sack",
					null,
					"This coarse hemp scrap sack is a medium-sized, workmanlike sack made from woven hemp. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					650.0,
					4.0m,
					true,
					false,
					"hemp",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Coarse sack for rope ends, tow, sweepings, kindling, offcuts, and other rough utility stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_herb_packet",
					"preindustrial_trade_herb_packet",
					"packet",
					"a small herb packet",
					null,
					"This small herb packet is a very small, workmanlike packet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.VerySmall,
					ItemQuality.Standard,
					60.0,
					3.0m,
					true,
					false,
					"linen",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sachet"
					],
					null,
					null,
					null,
					null,
					"Single-lot packet for dry herbs, powders, seeds, and minor apothecary stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_hide_wrapped_bale",
					"preindustrial_trade_hide_wrapped_bale",
					"bale",
					"a hide-wrapped cargo bale",
					null,
					"This hide-wrapped cargo bale is a large, workmanlike bale made from worked leather. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
					SizeCategory.Large,
					ItemQuality.Standard,
					2600.0,
					18.0m,
					true,
					false,
					"leather",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Durable wrapped cargo bale for travel, wagon work, caravan goods, or rough weather handling."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_jeweller_parcel_lockbox",
					"preindustrial_trade_jeweller_parcel_lockbox",
					"lockbox",
					"a jeweller's parcel lockbox",
					null,
					"This jeweller's parcel lockbox is a small, well-made lockbox worked from brass. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
					SizeCategory.Small,
					ItemQuality.Good,
					3600.0,
					150.0m,
					true,
					false,
					"brass",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
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
					null,
					"Compact lockbox for small high-value parcels and wrapped valuables."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_jute_meal_sack",
					"preindustrial_trade_jute_meal_sack",
					"sack",
					"a jute meal sack",
					null,
					"This jute meal sack is a medium-sized, workmanlike sack made from coarse jute. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					700.0,
					4.0m,
					true,
					false,
					"jute",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Cheap coarse sack for meal, pulse, bran, fodder, and other dry common goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_large_dry_goods_barrel",
					"preindustrial_trade_large_dry_goods_barrel",
					"barrel",
					"a large dry-goods barrel",
					null,
					"This large dry-goods barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					18000.0,
					52.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Drum"
					],
					null,
					null,
					null,
					null,
					"Large dry barrel for bulk goods, sealed stores, and transport lots."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_large_oak_hogshead",
					"preindustrial_trade_large_oak_hogshead",
					"hogshead",
					"a large oak hogshead",
					null,
					"This large oak hogshead is a large, well-made hogshead built from oak boards. Curved staves form a large rounded body, held by heavy bands around the middle and ends. The top is closed with a broad visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Good,
					26000.0,
					88.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Drum"
					],
					null,
					null,
					null,
					null,
					"Very large coopered dry container for bulk transport, export goods, or estate stores."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_lead_lined_canister",
					"preindustrial_trade_lead_lined_canister",
					"canister",
					"a lead-lined canister",
					null,
					"This lead-lined canister is a small, workmanlike canister worked from lead. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
					SizeCategory.Small,
					ItemQuality.Standard,
					2500.0,
					34.0m,
					true,
					false,
					"lead",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_HeavyMetal",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Heavy canister for dry materials that must be isolated or sealed from ordinary packing."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_leather_waste_sack",
					"preindustrial_trade_leather_waste_sack",
					"sack",
					"a leather waste sack",
					null,
					"This leather waste sack is a medium-sized, workmanlike sack made from worked leather. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					980.0,
					10.0m,
					true,
					false,
					"leather",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Hard-wearing sack for leather scraps, horn offcuts, bone pieces, and workshop waste stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_leatherworker_offcut_bin",
					"preindustrial_trade_leatherworker_offcut_bin",
					"bin",
					"a leather offcut bin",
					null,
					"This leather offcut bin is a medium-sized, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					4300.0,
					22.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Workshop bin for leather scraps, trimmed pieces, thongs, and repair stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_lime_cask",
					"preindustrial_trade_lime_cask",
					"cask",
					"a lime cask",
					null,
					"This lime cask is a large, workmanlike cask built from ash boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					12000.0,
					34.0m,
					true,
					false,
					"ash",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Dry cask for lime, chalk, ash, powdered mineral, or construction material."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_lime_powder_packet",
					"preindustrial_trade_lime_powder_packet",
					"packet",
					"a lime-powder packet",
					null,
					"This lime-powder packet is a very small, workmanlike packet made from coarse canvas. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.VerySmall,
					ItemQuality.Standard,
					120.0,
					3.0m,
					true,
					false,
					"canvas",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sachet"
					],
					null,
					null,
					null,
					null,
					"Tough small packet for chalk, lime, ash, powdered mineral, or other dry workshop material."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_linen_tally_sack",
					"preindustrial_trade_linen_tally_sack",
					"sack",
					"a small linen tally sack",
					null,
					"This small linen tally sack is a small, workmanlike sack made from woven linen. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Small,
					ItemQuality.Standard,
					180.0,
					3.0m,
					true,
					false,
					"linen",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Small closable trade sack for counted lots, tallied goods, samples, or coin-wrapped parcels."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_market_rent_chest",
					"preindustrial_trade_market_rent_chest",
					"chest",
					"a market-rent chest",
					null,
					"This market-rent chest is a medium-sized, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Good,
					18500.0,
					230.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"LockingContainer_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Chest for market dues, rent rolls, and payments."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_market_scales_tray",
					"preindustrial_trade_market_scales_tray",
					"tray",
					"a market scales tray",
					null,
					"This market scales tray is a small, well-made tray worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
					SizeCategory.Small,
					ItemQuality.Good,
					1400.0,
					48.0m,
					true,
					false,
					"brass",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_HeavyMetal",
						"Container_Tray"
					],
					null,
					null,
					null,
					null,
					"Metal tray for weights, measures, counterweights, and small weighed goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_medicine_trade_chest",
					"preindustrial_trade_medicine_trade_chest",
					"chest",
					"a medicine trade chest",
					null,
					"This medicine trade chest is a large, well-made chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Good,
					8700.0,
					88.0m,
					true,
					false,
					"cedar",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Trunk"
					],
					null,
					null,
					null,
					null,
					"Compartmented dry chest for medicines, instruments, dried plants, and labelled packets."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_merchant_counting_tray",
					"preindustrial_trade_merchant_counting_tray",
					"tray",
					"a merchant's counting tray",
					null,
					"This merchant's counting tray is a small, well-made tray built from walnut boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Good,
					950.0,
					28.0m,
					true,
					false,
					"walnut",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Tray"
					],
					null,
					null,
					null,
					null,
					"Shallow tray for sorted coins, seals, weights, counters, or priced sample lots."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_merchant_seal_casket",
					"preindustrial_trade_merchant_seal_casket",
					"casket",
					"a merchant seal casket",
					null,
					"This merchant seal casket is a small, well-made casket built from boxwood. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Good,
					1300.0,
					44.0m,
					true,
					false,
					"boxwood",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Small casket for seals, ring signets, wax, stamped samples, and contract tools."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_merchant_strongbox",
					"preindustrial_trade_merchant_strongbox",
					"strongbox",
					"a merchant strongbox",
					null,
					"This merchant strongbox is a medium-sized, well-made strongbox built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Good,
					15000.0,
					210.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"LockingContainer_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Stout trade strongbox for valuables, accounts, and compact cargo."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_metalworker_filings_box",
					"preindustrial_trade_metalworker_filings_box",
					"box",
					"a metalworker's filings box",
					null,
					"This metalworker's filings box is a small, workmanlike box worked from wrought iron. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
					SizeCategory.Small,
					ItemQuality.Standard,
					3300.0,
					42.0m,
					true,
					false,
					"wrought iron",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_HeavyMetal",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Heavy small box for filings, wire ends, offcuts, and dense metal scrap."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_money_changer_cash_box",
					"preindustrial_trade_money_changer_cash_box",
					"lockbox",
					"a money-changer's cash box",
					null,
					"This money-changer's cash box is a small, well-made lockbox worked from brass. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
					SizeCategory.Small,
					ItemQuality.Good,
					4200.0,
					120.0m,
					true,
					false,
					"brass",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
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
					null,
					"Metal cash box for money-changing counters and trade booths."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_nail_keg",
					"preindustrial_trade_nail_keg",
					"keg",
					"a nail keg",
					null,
					"This nail keg is a medium-sized, workmanlike keg built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					4200.0,
					20.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Small keg for nails, tacks, rivets, small fittings, and dense hardware."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_nested_packing_boxes",
					"preindustrial_trade_nested_packing_boxes",
					"box",
					"a nest of packing boxes",
					null,
					"This nest of packing boxes is a medium-sized, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					4200.0,
					32.0m,
					true,
					false,
					"pine",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Nested general-purpose wooden boxes for variable parcel sizes and shop packing."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_nested_spice_boxes",
					"preindustrial_trade_nested_spice_boxes",
					"box",
					"a nest of spice boxes",
					null,
					"This nest of spice boxes is a medium-sized, well-made box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Good,
					3600.0,
					70.0m,
					true,
					false,
					"cedar",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Nested merchant boxes for separating small aromatic or high-value dry goods."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_notary_document_lockbox",
					"preindustrial_trade_notary_document_lockbox",
					"lockbox",
					"a notary's document lockbox",
					null,
					"This notary's document lockbox is a small, well-made lockbox built from walnut boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Good,
					3400.0,
					110.0m,
					true,
					false,
					"walnut",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"LockingContainer_Lockbox"
					],
					null,
					null,
					null,
					null,
					"Small lockbox for deeds, writs, contracts, and sealed papers."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_oak_lidded_trade_box",
					"preindustrial_trade_oak_lidded_trade_box",
					"box",
					"an oak lidded trade box",
					null,
					"This oak lidded trade box is a medium-sized, workmanlike box built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					4200.0,
					28.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"General lidded box for shop stock, tools, dry parcels, and packed trade goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_oak_malt_bin",
					"preindustrial_trade_oak_malt_bin",
					"bin",
					"an oak malt bin",
					null,
					"This oak malt bin is a large, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					9000.0,
					34.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Deep storehouse bin for malt, grain steeping stock, and brewing or baking supplies."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_oak_ore_sample_bin",
					"preindustrial_trade_oak_ore_sample_bin",
					"bin",
					"an oak ore-sample bin",
					null,
					"This oak ore-sample bin is a medium-sized, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					5200.0,
					28.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Stout open bin for ore samples, sorted stone, metal scrap, or assayed cargo lots."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_oak_plank_coffer",
					"preindustrial_trade_oak_plank_coffer",
					"coffer",
					"an oak plank coffer",
					null,
					"This oak plank coffer is a large, workmanlike coffer built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					12500.0,
					60.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Trunk"
					],
					null,
					null,
					null,
					null,
					"Simple but substantial coffer for dry stock, textiles, bundles, and transportable household trade goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_oak_salted_food_tub",
					"preindustrial_trade_oak_salted_food_tub",
					"tub",
					"an oak salted-food tub",
					null,
					"This oak salted-food tub is a large, workmanlike tub built from oak boards. Staves form a broad open vessel with a flat bottom and a thick rim. The sides flare slightly, leaving the inside easy to reach. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					8200.0,
					34.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Open dry tub for cured or salted goods, not a brine-filled liquid container."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_open_felt_wagon_bin",
					"preindustrial_trade_open_felt_wagon_bin",
					"bin",
					"a felt-lined wagon bin",
					null,
					"This felt-lined wagon bin is a large, workmanlike bin built from pine boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					5200.0,
					24.0m,
					true,
					false,
					"pine",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Felt-lined bin for wagon-carried goods that need softer packing but not a closed trunk."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_open_oak_grain_bin",
					"preindustrial_trade_open_oak_grain_bin",
					"bin",
					"an open oak grain bin",
					null,
					"This open oak grain bin is a large, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					8600.0,
					32.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Open bin for grain, malt, pulse, or other dry bulk goods in a market or storehouse."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_ore_barrel",
					"preindustrial_trade_ore_barrel",
					"barrel",
					"an ore barrel",
					null,
					"This ore barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					17500.0,
					44.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Stout barrel for sorted ore, slag, heavy stone samples, or metal-bearing cargo."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_packed_glassware_bundle",
					"preindustrial_trade_packed_glassware_bundle",
					"bundle",
					"a straw-packed glassware bundle",
					null,
					"This straw-packed glassware bundle is a medium-sized, well-made bundle made from woven linen. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Normal,
					ItemQuality.Good,
					1100.0,
					20.0m,
					true,
					false,
					"linen",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Soft packed bundle for fragile cups, beads, glass blanks, or wrapped ceramic goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_parchment_wrapped_packet",
					"preindustrial_trade_parchment_wrapped_packet",
					"packet",
					"a parchment-wrapped packet",
					null,
					"This parchment-wrapped packet is a very small, workmanlike packet made from layered parchment. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
					SizeCategory.VerySmall,
					ItemQuality.Standard,
					50.0,
					6.0m,
					true,
					false,
					"parchment",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Writing Materials / Document Containers",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Paper",
						"Container_Sachet"
					],
					null,
					null,
					null,
					null,
					"Small wrapped packet for letters of credit, tallies, seals, or protected trade samples."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_pine_flour_bin",
					"preindustrial_trade_pine_flour_bin",
					"bin",
					"a pine flour bin",
					null,
					"This pine flour bin is a large, workmanlike bin built from pine boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					7200.0,
					26.0m,
					true,
					false,
					"pine",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Dry goods bin for flour, meal, bran, or other milled foodstuffs."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_pine_wool_hamper",
					"preindustrial_trade_pine_wool_hamper",
					"hamper",
					"a pine wool hamper",
					null,
					"This pine wool hamper is a large, workmanlike hamper built from pine boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					3000.0,
					18.0m,
					true,
					false,
					"pine",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Rigid hamper for bulk wool, combed fleece, fabric scraps, and shop stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_pitch_lined_dry_cask",
					"preindustrial_trade_pitch_lined_dry_cask",
					"cask",
					"a pitch-lined dry cask",
					null,
					"This pitch-lined dry cask is a large, well-made cask built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Good,
					15500.0,
					64.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Container / Watertight Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Lined dry cask for damp-sensitive goods, resins, spices, or travel stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_porcelain_tea_canister",
					"preindustrial_trade_porcelain_tea_canister",
					"canister",
					"a porcelain tea canister",
					null,
					"This porcelain tea canister is a small, well-made canister formed from porcelain. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
					SizeCategory.Small,
					ItemQuality.Good,
					850.0,
					50.0m,
					true,
					false,
					"porcelain",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Glassware",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Fine dry canister for tea, spice, incense, medicine, or valuable powdered stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_potters_clay_bin",
					"preindustrial_trade_potters_clay_bin",
					"bin",
					"a potter's clay bin",
					null,
					"This potter's clay bin is a large, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					8700.0,
					28.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Deep open bin for clay lumps, grog, powdered slip ingredients, and potter's dry stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_pottery_packing_crate",
					"preindustrial_trade_pottery_packing_crate",
					"crate",
					"a pottery packing crate",
					null,
					"This pottery packing crate is a large, workmanlike crate built from pine boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					3600.0,
					20.0m,
					true,
					false,
					"pine",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Wooden crate intended for straw-packed pots, bowls, tiles, and fragile fired goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_pottery_shard_barrel",
					"preindustrial_trade_pottery_shard_barrel",
					"barrel",
					"a pottery-shard barrel",
					null,
					"This pottery-shard barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					12000.0,
					24.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Dry barrel for shards, kiln wasters, grog, and pottery workshop stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_raw_wool_bale",
					"preindustrial_trade_raw_wool_bale",
					"bale",
					"a raw wool bale",
					null,
					"This raw wool bale is a large, workmanlike bale made from coarse canvas. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Large,
					ItemQuality.Standard,
					1800.0,
					12.0m,
					true,
					false,
					"canvas",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Large compressible bale for raw wool, combed fleece, or bulky textile fibre."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_river_cargo_chest",
					"preindustrial_trade_river_cargo_chest",
					"chest",
					"a tarred river cargo chest",
					null,
					"This tarred river cargo chest is a large, workmanlike chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					15000.0,
					66.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Trunk"
					],
					null,
					null,
					null,
					null,
					"Stout chest for barge, river, and ferry cargo; dry storage, not a liquid container."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_rivet_keg",
					"preindustrial_trade_rivet_keg",
					"keg",
					"a rivet keg",
					null,
					"This rivet keg is a medium-sized, workmanlike keg built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					4000.0,
					20.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Small coopered keg for rivets, rings, staples, and armour or building fittings."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_rope_coil_crate",
					"preindustrial_trade_rope_coil_crate",
					"crate",
					"a rope-coil crate",
					null,
					"This rope-coil crate is a large, workmanlike crate built from pine boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					3200.0,
					16.0m,
					true,
					false,
					"pine",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Open crate sized for rope coils, tow bundles, netting, and cordage."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_rope_maker_hemp_crate",
					"preindustrial_trade_rope_maker_hemp_crate",
					"crate",
					"a hemp-stock crate",
					null,
					"This hemp-stock crate is a large, workmanlike crate built from pine boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					3000.0,
					15.0m,
					true,
					false,
					"pine",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Open crate for rope coils, hemp tow, twine bundles, and cordage stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_rope_tied_bundle",
					"preindustrial_trade_rope_tied_bundle",
					"bundle",
					"a rope-tied trade bundle",
					null,
					"This rope-tied trade bundle is a medium-sized, workmanlike bundle made from coarse canvas. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					900.0,
					8.0m,
					true,
					false,
					"canvas",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"General bundled wrapper for small mixed wares, wrapped parcels, and non-fragile merchant stock."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_saffron_sachet",
					"preindustrial_trade_saffron_sachet",
					"sachet",
					"a saffron sachet",
					null,
					"This saffron sachet is a very small, well-made sachet made from woven silk. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.VerySmall,
					ItemQuality.Good,
					35.0,
					36.0m,
					true,
					false,
					"silk",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sachet"
					],
					null,
					null,
					null,
					null,
					"Fine sachet for very high-value spices, rare dye, perfume ingredients, or precious samples."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_salted_fish_barrel",
					"preindustrial_trade_salted_fish_barrel",
					"barrel",
					"a salted-fish barrel",
					null,
					"This salted-fish barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					13500.0,
					36.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Dry/salted-goods barrel for preserved fish and curing stock; use liquid containers for brine-filled variants."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_salted_meat_cask",
					"preindustrial_trade_salted_meat_cask",
					"cask",
					"a salted-meat cask",
					null,
					"This salted-meat cask is a large, workmanlike cask built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					14500.0,
					40.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Coopered dry cask for salted meat, jerky bundles, and packed preserved provisions."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_sawdust_barrel",
					"preindustrial_trade_sawdust_barrel",
					"barrel",
					"a sawdust barrel",
					null,
					"This sawdust barrel is a large, workmanlike barrel built from pine boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					8500.0,
					18.0m,
					true,
					false,
					"pine",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Cheap dry barrel for sawdust, shavings, packing material, and workshop sweepings."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_scribe_document_chest",
					"preindustrial_trade_scribe_document_chest",
					"chest",
					"a scribe's document chest",
					null,
					"This scribe's document chest is a large, well-made chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Good,
					7800.0,
					72.0m,
					true,
					false,
					"cedar",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Writing Materials / Document Containers",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Trunk"
					],
					null,
					null,
					null,
					null,
					"Dry chest for documents, contracts, blank parchment, tablets, and record bundles."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_scroll_shipping_box",
					"preindustrial_trade_scroll_shipping_box",
					"box",
					"a scroll shipping box",
					null,
					"This scroll shipping box is a medium-sized, well-made box built from cypress boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Good,
					2800.0,
					46.0m,
					true,
					false,
					"cypress",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Writing Materials / Document Containers",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Long lidded box for scrolls, rolls, maps, tallies, or narrow delicate goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_seal_and_weight_box",
					"preindustrial_trade_seal_and_weight_box",
					"box",
					"a seal-and-weight box",
					null,
					"This seal-and-weight box is a small, well-made box built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Good,
					1500.0,
					34.0m,
					true,
					false,
					"beech",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Small trade box for seals, counterweights, wax lumps, stamped tallies, and measuring pieces."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_sealed_clay_trade_jar",
					"preindustrial_trade_sealed_clay_trade_jar",
					"jar",
					"a sealed clay trade jar",
					null,
					"This sealed clay trade jar is a medium-sized, workmanlike jar formed from terracotta. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					3500.0,
					16.0m,
					true,
					false,
					"terracotta",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Glassware",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Sealable fired jar for dry market goods, powders, seed, or preserves packed without liquid mechanics."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_seed_sack",
					"preindustrial_trade_seed_sack",
					"sack",
					"a barley seed sack",
					null,
					"This barley seed sack is a medium-sized, workmanlike sack made from woven linen. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					620.0,
					7.0m,
					true,
					false,
					"linen",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Dry seed sack suited to measured agricultural trade lots and household stores."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_silk_sample_bag",
					"preindustrial_trade_silk_sample_bag",
					"bag",
					"a silk-lined sample bag",
					null,
					"This silk-lined sample bag is a small, well-made bag made from woven silk. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Small,
					ItemQuality.Good,
					120.0,
					30.0m,
					true,
					false,
					"silk",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Tote"
					],
					null,
					null,
					null,
					null,
					"Fine sample bag for elite cloth swatches, jewels, spices, or delicate display wares."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_slatted_pine_produce_crate",
					"preindustrial_trade_slatted_pine_produce_crate",
					"crate",
					"a slatted pine produce crate",
					null,
					"This slatted pine produce crate is a medium-sized, workmanlike crate built from pine boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					2400.0,
					16.0m,
					true,
					false,
					"pine",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Open slatted crate for fruit, roots, greens, and other goods that benefit from air."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_small_dry_goods_barrel",
					"preindustrial_trade_small_dry_goods_barrel",
					"barrel",
					"a small dry-goods barrel",
					null,
					"This small dry-goods barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					9800.0,
					30.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Small dry barrel for packed grain, nails, preserved food, or trade stock; not a liquid vessel."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_small_oak_keg",
					"preindustrial_trade_small_oak_keg",
					"keg",
					"a small oak keg",
					null,
					"This small oak keg is a medium-sized, workmanlike keg built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					3200.0,
					18.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Small coopered container for dense dry stock, fittings, resin lumps, or sealed parcels."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_small_pine_packing_box",
					"preindustrial_trade_small_pine_packing_box",
					"box",
					"a small pine packing box",
					null,
					"This small pine packing box is a small, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Standard,
					1600.0,
					14.0m,
					true,
					false,
					"pine",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Compact lidded packing box for parcels, sample lots, and fragile small goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_small_sample_casket",
					"preindustrial_trade_small_sample_casket",
					"casket",
					"a small sample casket",
					null,
					"This small sample casket is a small, well-made casket built from walnut boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Good,
					1500.0,
					38.0m,
					true,
					false,
					"walnut",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Fine small casket for sample stones, jewels, spice lots, and demonstration wares."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_spice_chest",
					"preindustrial_trade_spice_chest",
					"chest",
					"a locked spice chest",
					null,
					"This locked spice chest is a medium-sized, well-made chest built from cedar boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Good,
					14500.0,
					240.0m,
					true,
					false,
					"cedar",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"LockingContainer_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Built-in-lock chest for spices, dyestuffs, or small high-value goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_spice_display_tray",
					"preindustrial_trade_spice_display_tray",
					"tray",
					"a cedar spice tray",
					null,
					"This cedar spice tray is a small, well-made tray built from cedar boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Good,
					850.0,
					26.0m,
					true,
					false,
					"cedar",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Tray"
					],
					null,
					null,
					null,
					null,
					"Shallow tray for small bowls, packets, or measured spice samples on a stall."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_spice_sample_lockbox",
					"preindustrial_trade_spice_sample_lockbox",
					"lockbox",
					"a spice-sample lockbox",
					null,
					"This spice-sample lockbox is a small, well-made lockbox built from cedar boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Good,
					2600.0,
					95.0m,
					true,
					false,
					"cedar",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"LockingContainer_Lockbox"
					],
					null,
					null,
					null,
					null,
					"Built-in-lock box for valuable spice samples or small packets."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_stoneware_lidded_jar",
					"preindustrial_trade_stoneware_lidded_jar",
					"jar",
					"a stoneware lidded jar",
					null,
					"This stoneware lidded jar is a medium-sized, workmanlike jar formed from stoneware. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					2800.0,
					18.0m,
					true,
					false,
					"stoneware",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Glassware",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Rigid lidded jar for dry powders, salt, spices, seed, and small food stores."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_tally_seal_lockbox",
					"preindustrial_trade_tally_seal_lockbox",
					"lockbox",
					"a tally-seal lockbox",
					null,
					"This tally-seal lockbox is a small, workmanlike lockbox built from oak boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Standard,
					3000.0,
					58.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"LockingContainer_Lockbox"
					],
					null,
					null,
					null,
					null,
					"Lockbox for marked tallies, wax seals, and small account packets."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_tanner_bark_barrel",
					"preindustrial_trade_tanner_bark_barrel",
					"barrel",
					"a tanner's bark barrel",
					null,
					"This tanner's bark barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					12500.0,
					26.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Coopered barrel for bark, tan, dry lime, and tanning-house material."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_tanner_bark_bin",
					"preindustrial_trade_tanner_bark_bin",
					"bin",
					"a tanner's bark bin",
					null,
					"This tanner's bark bin is a large, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					8400.0,
					24.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Open bin for bark, tan, lime, and other dry tanner's materials."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_tax_strong_chest",
					"preindustrial_trade_tax_strong_chest",
					"chest",
					"a tax strong chest",
					null,
					"This tax strong chest is a large, well-made chest worked from wrought iron. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
					SizeCategory.Large,
					ItemQuality.Good,
					62000.0,
					680.0m,
					true,
					false,
					"wrought iron",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
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
					null,
					"Heavy metal-reinforced chest for taxes, fees, or administrative funds."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_teak_cotton_hamper",
					"preindustrial_trade_teak_cotton_hamper",
					"hamper",
					"a teak cotton hamper",
					null,
					"This teak cotton hamper is a large, well-made hamper built from teak boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Good,
					4200.0,
					48.0m,
					true,
					false,
					"teak",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Sturdier open hamper for cotton bales, dyed cloth, and workshop textiles."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_teak_spice_chest",
					"preindustrial_trade_teak_spice_chest",
					"chest",
					"a teak spice chest",
					null,
					"This teak spice chest is a medium-sized, well-made chest built from teak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Good,
					18000.0,
					280.0m,
					true,
					false,
					"teak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"LockingContainer_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Locking teak chest for valuable spices, dyes, or aromatics."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_terracotta_grain_jar",
					"preindustrial_trade_terracotta_grain_jar",
					"jar",
					"a terracotta grain jar",
					null,
					"This terracotta grain jar is a large, workmanlike jar formed from terracotta. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					9000.0,
					22.0m,
					true,
					false,
					"terracotta",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Glassware",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Fired storage jar for grain, pulses, dried fruit, and dry household or market goods."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_toll_chest",
					"preindustrial_trade_toll_chest",
					"chest",
					"a toll-keeper's chest",
					null,
					"This toll-keeper's chest is a medium-sized, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Good,
					18000.0,
					225.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Luxury Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"LockingContainer_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Built-in-lock chest for tolls, fees, or gatehouse takings."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_ventilated_fish_crate",
					"preindustrial_trade_ventilated_fish_crate",
					"crate",
					"a ventilated fish crate",
					null,
					"This ventilated fish crate is a medium-sized, workmanlike crate built from pine boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					2600.0,
					14.0m,
					true,
					false,
					"pine",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Open crate for fish, shellfish, salted catch bundles, or damp market goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_wagon_cargo_chest",
					"preindustrial_trade_wagon_cargo_chest",
					"chest",
					"a wagon cargo chest",
					null,
					"This wagon cargo chest is a large, workmanlike chest built from ash boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					13200.0,
					58.0m,
					true,
					false,
					"ash",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Trunk"
					],
					null,
					null,
					null,
					null,
					"Road-travel chest for wagon freight, wrapped tools, merchant stores, and estate goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_walnut_merchant_box",
					"preindustrial_trade_walnut_merchant_box",
					"box",
					"a walnut merchant box",
					null,
					"This walnut merchant box is a medium-sized, well-made box built from walnut boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Good,
					3600.0,
					48.0m,
					true,
					false,
					"walnut",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Better-finished trade box for valuable packets, ledgers, seals, and merchant samples."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_warehouse_strong_chest",
					"preindustrial_trade_warehouse_strong_chest",
					"chest",
					"a warehouse strong chest",
					null,
					"This warehouse strong chest is a large, well-made chest built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Good,
					36000.0,
					360.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
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
					null,
					"Heavy built-in-lock chest for warehouse keys, bonds, and high-value stored goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_wax_block_crate",
					"preindustrial_trade_wax_block_crate",
					"crate",
					"a wax-block crate",
					null,
					"This wax-block crate is a medium-sized, workmanlike crate built from beech boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					2400.0,
					18.0m,
					true,
					false,
					"beech",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Open crate for beeswax blocks, tallow stock, seal wax, and candle-making materials."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_wax_tablet_shipping_box",
					"preindustrial_trade_wax_tablet_shipping_box",
					"box",
					"a wax-tablet shipping box",
					null,
					"This wax-tablet shipping box is a medium-sized, workmanlike box built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					2400.0,
					28.0m,
					true,
					false,
					"beech",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Writing Materials / Document Containers",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Footlocker"
					],
					null,
					null,
					null,
					null,
					"Rigid box for writing tablets, samples, small books, or other flat trade goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_waxed_canvas_spice_sack",
					"preindustrial_trade_waxed_canvas_spice_sack",
					"sack",
					"a waxed canvas spice sack",
					null,
					"This waxed canvas spice sack is a small, well-made sack made from coarse canvas. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Small,
					ItemQuality.Good,
					360.0,
					18.0m,
					true,
					false,
					"canvas",
					[
						"Functions / Container",
						"Functions / Container / Watertight Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Wax-treated soft container for valuable dry spices, resins, aromatics, or dyestuffs."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_waxed_trade_barrel",
					"preindustrial_trade_waxed_trade_barrel",
					"barrel",
					"a waxed trade barrel",
					null,
					"This waxed trade barrel is a large, well-made barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Good,
					15000.0,
					60.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Container / Watertight Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Small_Drum"
					],
					null,
					null,
					null,
					null,
					"Better-sealed barrel for dry goods that must be protected from damp; still not a liquid container."
				);

		CreatePreIndustrialAlias(
					"medieval_locking_trade_weights_lockbox",
					"preindustrial_trade_weights_lockbox",
					"lockbox",
					"a weights lockbox",
					null,
					"This weights lockbox is a small, workmanlike lockbox built from oak boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Small,
					ItemQuality.Standard,
					3200.0,
					64.0m,
					true,
					false,
					"oak",
					[
						"Functions / Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"LockingContainer_Lockbox"
					],
					null,
					null,
					null,
					null,
					"Lockbox for keeping small scales weights, seals, and trade tokens together."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_willow_market_basket",
					"preindustrial_trade_willow_market_basket",
					"basket",
					"a willow market basket",
					null,
					"This willow market basket is a medium-sized, workmanlike basket built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					900.0,
					8.0m,
					true,
					false,
					"willow",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Open basket for loose produce, small parcels, and market-table goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_wooden_powder_tub",
					"preindustrial_trade_wooden_powder_tub",
					"tub",
					"a wooden powder tub",
					null,
					"This wooden powder tub is a medium-sized, workmanlike tub built from beech boards. Staves form a broad open vessel with a flat bottom and a thick rim. The sides flare slightly, leaving the inside easy to reach. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					3600.0,
					22.0m,
					true,
					false,
					"beech",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Open tub for dry powders, grain, meal, plaster, pigments, or sorted workshop material."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_wool_fleece_sack",
					"preindustrial_trade_wool_fleece_sack",
					"sack",
					"a wool fleece sack",
					null,
					"This wool fleece sack is a medium-sized, workmanlike sack made from woven wool. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					640.0,
					6.0m,
					true,
					false,
					"wool",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Soft sack for loose fleeces, wool locks, cloth scraps, and other compressible textile goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_wool_hamper",
					"preindustrial_trade_wool_hamper",
					"hamper",
					"a wool sorting hamper",
					null,
					"This wool sorting hamper is a large, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
					SizeCategory.Large,
					ItemQuality.Standard,
					1800.0,
					14.0m,
					true,
					false,
					"willow",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_Furniture",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Large open hamper for sorting fleeces, cloth scraps, raw wool, and bulky textile goods."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_wool_sorting_sack",
					"preindustrial_trade_wool_sorting_sack",
					"sack",
					"a wool sorting sack",
					null,
					"This wool sorting sack is a medium-sized, workmanlike sack made from woven wool. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					560.0,
					6.0m,
					true,
					false,
					"wool",
					[
						"Functions / Container",
						"Functions / Container / Porous Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Simple Wares"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Container_Sack"
					],
					null,
					null,
					null,
					null,
					"Soft sack for separating clean wool, waste fleece, combings, and sorted textile fibre."
				);

		CreatePreIndustrialAlias(
					"medieval_trade_wrought_iron_nail_bin",
					"preindustrial_trade_wrought_iron_nail_bin",
					"bin",
					"a wrought-iron nail bin",
					null,
					"This wrought-iron nail bin is a small, workmanlike bin worked from wrought iron. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
					SizeCategory.Small,
					ItemQuality.Standard,
					4200.0,
					32.0m,
					true,
					false,
					"wrought iron",
					[
						"Functions / Container",
						"Functions / Container / Open Container",
						"Functions / Household Items / Household Wares",
						"Market / Household Goods / Standard Wares"
					],
					[
						"Holdable",
						"Destroyable_HeavyMetal",
						"Container_Open_Bin"
					],
					null,
					null,
					null,
					null,
					"Small metal bin for nails, tacks, rivets, clamps, and other dense hardware."
				);

		SeedPreIndustrialGlobalTradePackagingItems();
	}

	private void SeedPreIndustrialDoorsLocksAndBasicHardware()
	{
		CreatePreIndustrialAlias(
					"medieval_iron_buckled_leather_belt",
					"preindustrial_clothing_iron_buckled_leather_belt",
					"belt",
					"an iron-buckled leather belt",
					null,
					"This iron-buckled leather belt is a sturdy strap fitted with a dark iron buckle and a row of cleanly punched holes. The leather is thick enough to hold its shape around the waist, with burnished edges and visible stitching near the buckle fold. Natural variations in the leather show along the grain, flex marks, and cut edge.",
					SizeCategory.Small,
					ItemQuality.Good,
					240.0,
					30.0m,
					true,
					false,
					"leather",
					[
						"Market / Clothing / Luxury Clothing",
						"Functions / Worn Items / Belts"
					],
					[
						"Holdable",
						"Wear_Waist",
						"Destroyable_Clothing",
						"Insulation_Minor",
						"Armour_LightClothing"
					],
					null,
					null,
					null,
					null
				);

		CreatePreIndustrialAlias(
					"medieval_plain_leather_belt",
					"preindustrial_clothing_plain_leather_belt",
					"belt",
					"a plain leather belt",
					null,
					"This plain leather belt is a long strap of sturdy leather with a simple fastening and burnished edges. The surface is left unornamented, relying on the grain of the hide and the evenness of the cut for its appearance. Natural variations in the leather show along the grain, flex marks, and cut edge.",
					SizeCategory.Small,
					ItemQuality.Standard,
					180.0,
					10.0m,
					true,
					false,
					"leather",
					[
						"Market / Clothing / Standard Clothing",
						"Functions / Worn Items / Belts"
					],
					[
						"Holdable",
						"Wear_Waist",
						"Destroyable_Clothing",
						"Insulation_Minor",
						"Armour_LightClothing"
					],
					null,
					null,
					null,
					null
				);

		CreatePreIndustrialAlias(
					"medieval_simple_woven_sash",
					"preindustrial_clothing_simple_woven_sash",
					"sash",
					"a $colour woven sash",
					null,
					"This $colour woven sash is a long wool band with firm selvages and simple squared ends. It is made to wrap, knot, or girdle clothing without adding a hard buckle or metal fitting. The $colour dye is simple and even, showing most clearly across the broad panels and worn edges.",
					SizeCategory.Small,
					ItemQuality.Standard,
					120.0,
					6.0m,
					true,
					false,
					"wool",
					[
						"Market / Clothing / Standard Clothing",
						"Functions / Worn Items / Belts"
					],
					[
						"Holdable",
						"Wear_Sash",
						"Destroyable_Clothing",
						"Insulation_Moderate",
						"Armour_LightClothing",
						"Variable_BasicColour"
					],
					null,
					null,
					null,
					null
				);

		CreatePreIndustrialAlias(
					"medieval_door_shared_bamboo_screen_hanging",
					"preindustrial_door_bamboo_screen_hanging",
					"screen",
					"a bamboo doorway screen",
					null,
					"This bamboo doorway screen is a large, workmanlike screen built from split bamboo. Thin slats are bound into a flat screen, with a top rail for hanging and a lightly weighted lower edge. The gaps are narrow but still visible. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
					SizeCategory.Large,
					ItemQuality.Standard,
					3000.0,
					36.0m,
					true,
					false,
					"bamboo",
					[
						"Functions / Household Items / Household Decorations",
						"Market / Household Goods / Standard Decorations"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Door_Bad_Large"
					],
					null,
					null,
					null,
					null,
					"Bamboo screen-like doorway barrier with weak door behaviour."
				);

		CreatePreIndustrialAlias(
					"medieval_door_shared_barn_door",
					"preindustrial_door_barn_door",
					"door",
					"a broad barn door",
					null,
					"This broad barn door is a very large, workmanlike door built from pine boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
					SizeCategory.VeryLarge,
					ItemQuality.Standard,
					62000.0,
					130.0m,
					true,
					false,
					"pine",
					[
						"Functions / Household Items / Household Construction Materials",
						"Market / Construction Materials / Worked Timber"
					],
					[
						"Holdable",
						"Destroyable_Door",
						"Door_Normal_VeryLarge"
					],
					null,
					null,
					null,
					null,
					"Broad plank door for barns and granaries."
				);

		CreatePreIndustrialAlias(
					"medieval_door_shared_bathhouse_screen_door",
					"preindustrial_door_bathhouse_screen_door",
					"screen",
					"a bathhouse screen door",
					null,
					"This bathhouse screen door is a large, workmanlike screen built from cedar boards. Thin slats are bound into a flat screen, with a top rail for hanging and a lightly weighted lower edge. The gaps are narrow but still visible. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
					SizeCategory.Large,
					ItemQuality.Standard,
					17000.0,
					52.0m,
					true,
					false,
					"cedar",
					[
						"Functions / Household Items / Household Decorations",
						"Market / Household Goods / Standard Decorations"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Door_Bad_Large"
					],
					null,
					null,
					null,
					null,
					"Light screen-like door for baths, washrooms, and changing spaces."
				);

		CreatePreIndustrialAlias(
					"medieval_door_shared_boarded_yard_gate",
					"preindustrial_door_boarded_yard_gate",
					"gate",
					"a boarded yard gate",
					null,
					"This boarded yard gate is a very large, workmanlike gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
					SizeCategory.VeryLarge,
					ItemQuality.Standard,
					52000.0,
					110.0m,
					true,
					false,
					"oak",
					[
						"Functions / Household Items / Household Construction Materials",
						"Market / Construction Materials / Worked Timber"
					],
					[
						"Holdable",
						"Destroyable_Door",
						"Gate_Normal_VeryLarge"
					],
					null,
					null,
					null,
					null,
					"Common yard gate; gate behaviour can be seen and fired through."
				);

		CreatePreIndustrialAlias(
					"medieval_door_shared_bone_bead_doorway_hanging",
					"preindustrial_door_bone_bead_doorway_hanging",
					"hanging",
					"a bone bead doorway hanging",
					null,
					"This bone bead doorway hanging is a large, workmanlike hanging worked from bone. Strings of beads hang from a narrow header, leaving small gaps between each strand. The lower ends are uneven from movement through the passage. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
					SizeCategory.Large,
					ItemQuality.Standard,
					2800.0,
					38.0m,
					true,
					false,
					"bone",
					[
						"Functions / Household Items / Household Decorations",
						"Market / Household Goods / Standard Decorations"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Door_Bad_Large"
					],
					null,
					null,
					null,
					null,
					"Beaded doorway hanging that marks a passage without making a secure barrier."
				);

		CreatePreIndustrialAlias(
					"medieval_door_shared_carpet_doorway_hanging",
					"preindustrial_door_carpet_doorway_hanging",
					"hanging",
					"a carpet doorway hanging",
					null,
					"This carpet doorway hanging is a large, well-made hanging made from woven wool. Heavy fabric hangs from a reinforced top edge, falling in a single soft sheet. The lower hem is weighted so it settles back after being pushed aside. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
					SizeCategory.Large,
					ItemQuality.Good,
					5200.0,
					70.0m,
					true,
					false,
					"wool",
					[
						"Functions / Household Items / Household Decorations",
						"Market / Household Goods / Luxury Decorations"
					],
					[
						"Holdable",
						"Destroyable_Misc",
						"Door_Bad_Large"
					],
					null,
					null,
					null,
					null,
					"Heavy woven doorway hanging used as a soft barrier."
				);

		CreatePreIndustrialAlias(
					"medieval_door_shared_cart_gate",
					"preindustrial_door_cart_gate",
					"gate",
					"a cart-yard gate",
					null,
					"This cart-yard gate is a very large, workmanlike gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
					SizeCategory.VeryLarge,
					ItemQuality.Standard,
					64000.0,
					135.0m,
					true,
					false,
					"oak",
					[
						"Functions / Household Items / Household Construction Materials",
						"Market / Construction Materials / Worked Timber"
					],
					[
						"Holdable",
						"Destroyable_Door",
						"Gate_Normal_VeryLarge"
					],
					null,
					null,
					null,
					null,
					"Wide yard gate suitable for carts and wagons."
				);

		CreatePreIndustrialAlias(
					"medieval_door_shared_lockable_plank_household_door",
					"preindustrial_door_lockable_plank_household_door",
					"door",
					"a lockable plank household door",
					null,
					"This lockable plank household door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
					SizeCategory.Large,
					ItemQuality.Good,
					39000.0,
					132.0m,
					true,
					false,
					"oak",
					[
						"Functions / Household Items / Household Construction Materials",
						"Market / Construction Materials / Worked Timber"
					],
					[
						"Holdable",
						"Destroyable_Door",
						"Door_Lockable_Normal_Large"
					],
					null,
					null,
					null,
					null,
					"Household door with built-in lock behaviour for houses, shops, or private chambers."
				);

		CreatePreIndustrialAlias(
					"medieval_door_shared_lockable_postern_gate",
					"preindustrial_door_lockable_postern_gate",
					"gate",
					"a lockable postern gate",
					null,
					"This lockable postern gate is a very large, well-made gate built from oak boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
					SizeCategory.VeryLarge,
					ItemQuality.Good,
					70000.0,
					255.0m,
					true,
					false,
					"oak",
					[
						"Functions / Household Items / Household Construction Materials",
						"Market / Construction Materials / Worked Timber"
					],
					[
						"Holdable",
						"Destroyable_Door",
						"Gate_Lockable_Secure_VeryLarge"
					],
					null,
					null,
					null,
					null,
					"Postern gate with built-in lock behaviour."
				);

		CreatePreIndustrialAlias(
					"medieval_door_shared_lockable_shop_door",
					"preindustrial_door_lockable_shop_door",
					"door",
					"a lockable shopfront door",
					null,
					"This lockable shopfront door is a large, well-made door built from elm boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
					SizeCategory.Large,
					ItemQuality.Good,
					37000.0,
					126.0m,
					true,
					false,
					"elm",
					[
						"Functions / Household Items / Household Construction Materials",
						"Market / Construction Materials / Worked Timber"
					],
					[
						"Holdable",
						"Destroyable_Door",
						"Door_Lockable_Normal_Large"
					],
					null,
					null,
					null,
					null,
					"Shop or workshop door with built-in lock behaviour."
				);

		CreatePreIndustrialAlias(
					"medieval_door_shared_lockable_stable_door",
					"preindustrial_door_lockable_stable_door",
					"door",
					"a lockable stable door",
					null,
					"This lockable stable door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
					SizeCategory.Large,
					ItemQuality.Good,
					41500.0,
					142.0m,
					true,
					false,
					"oak",
					[
						"Functions / Household Items / Household Construction Materials",
						"Market / Construction Materials / Worked Timber"
					],
					[
						"Holdable",
						"Destroyable_Door",
						"Door_Lockable_Normal_Large"
					],
					null,
					null,
					null,
					null,
					"Stable or byre door with built-in lock behaviour."
				);

		CreatePreIndustrialAlias(
					"medieval_door_shared_lockable_store_room_door",
					"preindustrial_door_lockable_store_room_door",
					"door",
					"a lockable storeroom door",
					null,
					"This lockable storeroom door is a large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
					SizeCategory.Large,
					ItemQuality.Good,
					40000.0,
					142.0m,
					true,
					false,
					"oak",
					[
						"Functions / Household Items / Household Construction Materials",
						"Market / Construction Materials / Worked Timber"
					],
					[
						"Holdable",
						"Destroyable_Door",
						"Door_Lockable_Secure_Large"
					],
					null,
					null,
					null,
					null,
					"Storeroom door with built-in locking behaviour."
				);

		CreatePreIndustrialAlias(
					"medieval_door_shared_lockable_warehouse_door",
					"preindustrial_door_lockable_warehouse_door",
					"door",
					"a lockable warehouse door",
					null,
					"This lockable warehouse door is a very large, well-made door built from oak boards. Planks are held inside a braced frame, with one edge prepared for hinges and the other for closure. The lower edge is scuffed from dragging near the floor. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
					SizeCategory.VeryLarge,
					ItemQuality.Good,
					67500.0,
					225.0m,
					true,
					false,
					"oak",
					[
						"Functions / Household Items / Household Construction Materials",
						"Market / Construction Materials / Worked Timber"
					],
					[
						"Holdable",
						"Destroyable_Door",
						"Door_Lockable_Normal_VeryLarge"
					],
					null,
					null,
					null,
					null,
					"Large warehouse door with built-in lock behaviour."
				);

		CreatePreIndustrialAlias(
					"medieval_door_shared_lockable_wicket_gate",
					"preindustrial_door_lockable_wicket_gate",
					"gate",
					"a lockable wicket gate",
					null,
					"This lockable wicket gate is a large, well-made gate built from ash boards. Vertical pales sit inside a braced frame, leaving open gaps between the timbers. The latch side is squared to meet a post cleanly. The hinge side, closing edge, and lower corners show the heaviest wear from use.",
					SizeCategory.Large,
					ItemQuality.Good,
					29000.0,
					88.0m,
					true,
					false,
					"ash",
					[
						"Functions / Household Items / Household Construction Materials",
						"Market / Construction Materials / Worked Timber"
					],
					[
						"Holdable",
						"Destroyable_Door",
						"Gate_Lockable_Normal_Large"
					],
					null,
					null,
					null,
					null,
					"Small lockable gate for yards, gardens, and side entries."
				);

	}

	private void SeedPreIndustrialCommonToolsAndWorkshopFixtures()
	{
		CreatePreIndustrialAlias(
					"medieval_tool_adze",
					"preindustrial_tool_adze",
			"adze",
			"an iron woodworking adze",
			null,
			"This iron adze is hafted across a curved wooden handle, its edge set at right angles for dressing boards and hollowing timber.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1300.0,
			28.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Adze" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_armourers_pliers",
					"preindustrial_tool_armourers_pliers",
			"pliers",
			"a pair of armourer's pliers",
			null,
			"These iron pliers have stout jaws for closing mail rings, gripping hot rivets, and bending lamella lacing tabs.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Armouring Tools / Armourer's Pliers", "Functions / Tools / Gripping Tools / Pliers" ],
			[ "Holdable", "Tool_Armouring_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_armourers_stake",
					"preindustrial_tool_armourers_stake",
			"stake",
			"an armourer's iron stake",
			null,
			"This heavy iron stake has a tapered foot and rounded working faces for shaping plates, scales, and helmet pieces.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8200.0,
			85.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Armouring Tools / Armourer's Stake" ],
			[ "Holdable", "Tool_Armouring_General", "Destroyable_HeavyMetal" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_ball_stake",
					"preindustrial_tool_ball_stake",
			"stake",
			"a ball stake",
			null,
			"This iron stake ends in a polished rounded ball for doming shield bosses, cups, helmet plates, and raised metal fittings.",
			SizeCategory.Normal,
			ItemQuality.Good,
			6400.0,
			72.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Armouring Tools / Ball Stake" ],
			[ "Holdable", "Tool_Armouring_General", "Destroyable_HeavyMetal" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_bow_saw",
					"preindustrial_tool_bow_saw",
			"saw",
			"a wooden bow saw",
			null,
			"This saw holds a narrow iron blade under tension in a curved wooden frame for controlled cuts in boards, curves, and small billets.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			24.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Saws / Bow Saw" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_bung_borer",
					"preindustrial_tool_bung_borer",
			"borer",
			"an iron bung borer",
			null,
			"This iron-edged borer is set into a wooden brace for opening neat round holes in casks, tubs, and brewing vessels.",
			SizeCategory.Small,
			ItemQuality.Standard,
			680.0,
			20.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Coopering Tools / Bung Borer" ],
			[ "Holdable", "Tool_Coopering_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_charcoal_rake",
					"preindustrial_tool_charcoal_rake",
			"rake",
			"a long charcoal rake",
			null,
			"This long-handled iron rake has a narrow hooked head for drawing charcoal, clinker, and ash across a hot hearth or furnace mouth.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1350.0,
			18.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_charging_bucket",
					"preindustrial_tool_charging_bucket",
			"bucket",
			"an iron charging bucket",
			null,
			"This iron bucket has a reinforced rim, a bale handle, and blackened sides from lifting ore, charcoal, flux, glass batch, or clay charges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			2400.0,
			22.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Charging Bucket" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_clay_knife",
					"preindustrial_tool_clay_knife",
			"knife",
			"a blunt clay knife",
			null,
			"This small wooden-handled knife has a blunt iron edge for cutting wet clay, trimming vessel rims, and cleaning pot feet.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			120.0,
			6.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Pottery Tools / Clay Knife" ],
			[ "Holdable", "Tool_Pottery_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_clay_stamp",
					"preindustrial_tool_clay_stamp",
			"stamp",
			"a carved clay stamp",
			null,
			"This small fired-clay stamp has a carved face for pressing simple marks, borders, or maker's signs into wet clay.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			95.0,
			6.0m,
			false,
			false,
			"earthenware",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Pottery Tools / Clay Stamp" ],
			[ "Holdable", "Tool_Pottery_General", "Destroyable_Glassware" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_coopers_adze",
					"preindustrial_tool_coopers_adze",
			"adze",
			"a cooper's iron adze",
			null,
			"This short-handled adze has a curved iron bit shaped for hollowing the inner faces of staves and tubs.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1250.0,
			28.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Coopering Tools / Cooper's Adze" ],
			[ "Holdable", "Tool_Coopering_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_coopers_jointer",
					"preindustrial_tool_coopers_jointer",
			"jointer",
			"a cooper's jointer plane",
			null,
			"This long wooden jointer holds a straight iron blade for truing barrel staves to meet cleanly at their edges.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3600.0,
			30.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Coopering Tools" ],
			[ "Holdable", "Tool_Coopering_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_crossguard_fixture",
					"preindustrial_tool_crossguard_fixture",
			"fixture",
			"a crossguard fitting fixture",
			null,
			"This compact wooden-and-iron fixture has slots and pegs for seating a sword guard square against a blade shoulder while the hilt is assembled.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Weaponsmithing Tools / Crossguard Fixture" ],
			[ "Holdable", "Tool_Weaponsmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_croze",
					"preindustrial_tool_croze",
			"croze",
			"a cooper's croze",
			null,
			"This croze has a curved wooden body and a small iron cutter for cutting grooves that receive cask heads and bucket bottoms.",
			SizeCategory.Small,
			ItemQuality.Standard,
			560.0,
			22.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Coopering Tools / Croze" ],
			[ "Holdable", "Tool_Coopering_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_crucible",
					"preindustrial_tool_crucible",
			"crucible",
			"a ceramic crucible",
			null,
			"This thick ceramic crucible is darkened around the lip and vitrified in places from repeated hot work with metal, pigment, or glass batches.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			16.0m,
			false,
			false,
			"ceramic",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Metalworking Tools / Crucible" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Glassware" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_crucible_tongs",
					"preindustrial_tool_crucible_tongs",
			"tongs",
			"a pair of crucible tongs",
			null,
			"These long iron tongs have curved jaws sized to cradle a hot crucible without crushing it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1250.0,
			24.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Crucible Tongs" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_dishing_form",
					"preindustrial_tool_dishing_form",
			"form",
			"a wooden dishing form",
			null,
			"This dense wooden block is hollowed into shallow bowls for hammering plate, bosses, and lamellar curves into shape.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			7200.0,
			34.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Armouring Tools / Dishing Form" ],
			[ "Holdable", "Tool_Armouring_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_drawknife",
					"preindustrial_tool_drawknife",
			"drawknife",
			"an iron drawknife",
			null,
			"This drawknife has a straight iron blade between two turned wooden handles for pulling shavings from staves, shafts, spokes, handles, and bow staves.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_drill_bow",
					"preindustrial_tool_drill_bow",
			"drill",
			"a bow drill with small bits",
			null,
			"This bow drill has a corded bow, hand block, spindle, and small iron bits for boring beads, shell, bone, horn, and soft stone.",
			SizeCategory.Small,
			ItemQuality.Standard,
			460.0,
			22.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Bow Drill", "Functions / Tools / Lapidary Tools / Bead Drill" ],
			[ "Holdable", "Tool_Lapidary_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_felling_axe",
					"preindustrial_tool_felling_axe",
			"axe",
			"an iron felling axe",
			null,
			"This iron axe has a broad cutting bit, a poll scarred by wedges, and a long ash haft polished by two-handed timber work.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2100.0,
			32.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Felling Axe" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Weapon" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_fine_saw",
					"preindustrial_tool_fine_saw",
			"saw",
			"a fine-toothed cabinet saw",
			null,
			"This small iron saw has a stiffened back, fine teeth, and a smooth wooden grip for boxes, inlay, book boards, pegs, and small cabinet joints.",
			SizeCategory.Small,
			ItemQuality.Good,
			420.0,
			32.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Saws / Fine Saw" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_forest_saw",
					"preindustrial_tool_forest_saw",
			"saw",
			"a two-handled forest saw",
			null,
			"This long iron saw has a wooden grip at each end and large teeth for timber work by two workers.",
			SizeCategory.Large,
			ItemQuality.Standard,
			3200.0,
			48.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Saws / Forest Saw" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_forming_bag",
					"preindustrial_tool_forming_bag",
			"bag",
			"an armourer's forming bag",
			null,
			"This heavy leather bag is packed tight with sand and grit so plate and boss blanks can be dished over it without sharp marks.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			12500.0,
			38.0m,
			false,
			false,
			"leather",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Armouring Tools / Armourer's Forming Bags" ],
			[ "Holdable", "Tool_Armouring_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_glass_blowpipe",
					"preindustrial_tool_glass_blowpipe",
			"blowpipe",
			"an iron glassblowing pipe",
			null,
			"This long iron blowpipe has a narrow bore, wrapped mouth end, and darkened gathering end for taking molten glass from a furnace.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1400.0,
			32.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Glassblowing Tools / Blowpipe" ],
			[ "Holdable", "Tool_Glassblowing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_hand_saw",
					"preindustrial_tool_hand_saw",
			"saw",
			"an iron hand saw",
			null,
			"This iron hand saw has a tapered blade, filed teeth, and a riveted wooden grip for boards, pegs, small beams, and furniture work.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			26.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Saws / Hand Saw" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_hoop_driver",
					"preindustrial_tool_hoop_driver",
			"driver",
			"a wooden hoop driver",
			null,
			"This blunt wooden driver has a broad striking face and a hand-smoothed grip for walking hoops down over staves.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			8.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Coopering Tools / Hoop Driver" ],
			[ "Holdable", "Tool_Coopering_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_jewellers_anvil",
					"preindustrial_tool_jewellers_anvil",
			"anvil",
			"a small jeweller's anvil",
			null,
			"This small iron anvil has a bright flat face, narrow horn, and fitted timber base for shaping rings, pins, settings, and ornaments.",
			SizeCategory.Small,
			ItemQuality.Good,
			2800.0,
			70.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Jewellery Tools / Jeweller's Anvil" ],
			[ "Holdable", "Tool_Jewellery_General", "Destroyable_HeavyMetal" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_jewellers_burnisher",
					"preindustrial_tool_jewellers_burnisher",
			"burnisher",
			"a jeweller's polished burnisher",
			null,
			"This fine burnisher has a rounded polished tip and boxwood handle for closing settings, smoothing wire, and finishing small ornaments.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			80.0,
			32.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Jewellery Tools / Jeweller's Burnisher" ],
			[ "Holdable", "Tool_Jewellery_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_jewellers_crimping_pliers",
					"preindustrial_tool_jewellers_crimping_pliers",
			"pliers",
			"a pair of jeweller's crimping pliers",
			null,
			"These fine iron pliers have small grooved jaws for closing crimps, clasps, wire loops, and setting tabs.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			170.0,
			45.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Jewellery Tools / Crimping Pliers" ],
			[ "Holdable", "Tool_Jewellery_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_lapidary_saw",
					"preindustrial_tool_lapidary_saw",
			"saw",
			"a bow-framed lapidary saw",
			null,
			"This small bow saw holds a thin abrasive blade for cutting shell, jet, amber, soft stone, and gem rough under grit and water.",
			SizeCategory.Small,
			ItemQuality.Good,
			520.0,
			60.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Lapidary Tools / Lapidary Saw" ],
			[ "Holdable", "Tool_Lapidary_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_lapidary_wheel",
					"preindustrial_tool_lapidary_wheel",
			"wheel",
			"a hand-turned lapidary wheel",
			null,
			"This small stone wheel sits in a wooden frame with a hand crank and shallow slurry trough for grinding beads, cabochons, shell, amber, and soft stone.",
			SizeCategory.Normal,
			ItemQuality.Good,
			12000.0,
			95.0m,
			false,
			false,
			"stone",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Lapidary Tools / Lapidary Wheel" ],
			[ "Holdable", "Tool_Lapidary_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_locksmithing_fabrication_kit",
					"preindustrial_tool_locksmithing_fabrication_kit",
			"kit",
			"a locksmith's fabrication kit",
			null,
			"This compact kit holds small files, warding picks, punches, gauges, tweezers, and narrow chisels for making locks, keys, and internal wards.",
			SizeCategory.Small,
			ItemQuality.Good,
			1100.0,
			95.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Locksmithing Tools" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_locksmithing_installation_kit",
					"preindustrial_tool_locksmithing_installation_kit",
			"kit",
			"a locksmith's installation kit",
			null,
			"This small kit bundles files, awls, punches, wedges, and measuring strips for fitting locks, hasps, latches, and keys to chests or doors.",
			SizeCategory.Small,
			ItemQuality.Good,
			1250.0,
			90.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Locksmithing Tools" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_lye_leaching_barrel",
					"preindustrial_tool_lye_leaching_barrel",
			"barrel",
			"an ash-lye leaching barrel",
			null,
			"This upright wooden barrel has a perforated false bottom, a dark drain plug, and ash stains from leaching alkali liquor.",
			SizeCategory.Large,
			ItemQuality.Standard,
			13000.0,
			38.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Primary Production Tools" ],
			[ "Holdable", "Tool_Dyeing_Fulling_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_masons_hammer",
					"preindustrial_tool_masons_hammer",
			"hammer",
			"an iron mason's hammer",
			null,
			"This mason's hammer has a square striking face, a narrow pick, and a tough ash handle for dressing stone.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1300.0,
			18.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Stoneworking Tools / Bush Hammer", "Functions / Tools / Striking Tools / Hammer" ],
			[ "Holdable", "Tool_Masonry_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_masons_line",
					"preindustrial_tool_masons_line",
			"line",
			"a mason's line and pins",
			null,
			"This small bundle holds a hemp line wrapped around two iron pins for setting straight courses, door openings, and wall faces.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			160.0,
			5.0m,
			false,
			false,
			"hemp",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Layout Tools / Stringline" ],
			[ "Holdable", "Tool_Masonry_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_masons_square",
					"preindustrial_tool_masons_square",
			"square",
			"a wooden mason's square",
			null,
			"This right-angled wooden square is reinforced with small iron plates and marked by chalk and stone dust.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			10.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Construction Tools / Construction Ruler" ],
			[ "Holdable", "Tool_Masonry_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_masons_trowel",
					"preindustrial_tool_masons_trowel",
			"trowel",
			"an iron mason's trowel",
			null,
			"This flat iron trowel has a pointed blade and wooden handle for spreading mortar, plaster, limewash, and tile bedding.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			8.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Construction Tools / Construction Trowel", "Functions / Tools / Finishing Tools / Trowel" ],
			[ "Holdable", "Tool_Masonry_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_ore_crusher",
					"preindustrial_tool_ore_crusher",
			"crusher",
			"a stone ore crusher",
			null,
			"This heavy stone crusher is set with an iron-shod striking face and a shallow anvil block for breaking ore, slag, cullet, and hard pigment stones.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			18500.0,
			45.0m,
			false,
			false,
			"stone",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Ore Crusher" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_ore_roaster",
					"preindustrial_tool_ore_roaster",
			"roaster",
			"a shallow ore-roasting pan",
			null,
			"This broad iron pan has low sides and a scarred base for spreading crushed ore over heat before smelting.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6200.0,
			38.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Ore Roaster" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_HeavyMetal" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_planishing_hammer",
					"preindustrial_tool_planishing_hammer",
			"hammer",
			"a polished planishing hammer",
			null,
			"This small iron hammer has a polished face for smoothing armour plates and sheet metal after rough forming.",
			SizeCategory.Small,
			ItemQuality.Good,
			780.0,
			36.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Armouring Tools / Planishing Hammer", "Functions / Tools / Striking Tools / Hammer" ],
			[ "Holdable", "Tool_Armouring_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_plate_snips",
					"preindustrial_tool_plate_snips",
			"snips",
			"a pair of iron plate snips",
			null,
			"These heavy iron snips have short jaws and long handles for cutting thin armour plate, sheet metal, scales, and lamella blanks.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1450.0,
			30.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Armouring Tools / Plate Snips" ],
			[ "Holdable", "Tool_Armouring_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_point_chisel",
					"preindustrial_tool_point_chisel",
			"chisel",
			"an iron point chisel",
			null,
			"This iron point chisel tapers to a strong narrow tip for roughing out stone blocks, slate, and masonry details.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Stoneworking Tools / Stone Chisel" ],
			[ "Holdable", "Tool_Masonry_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_potters_rib",
					"preindustrial_tool_potters_rib",
			"rib",
			"a wooden potter's rib",
			null,
			"This curved wooden rib is polished smooth by wet clay, with one broad face and one sharper edge for shaping vessel walls.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			55.0,
			4.0m,
			false,
			false,
			"boxwood",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Pottery Tools / Potter's Rib" ],
			[ "Holdable", "Tool_Pottery_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_potters_wheel",
					"preindustrial_tool_potters_wheel",
			"wheel",
			"a kick-driven potter's wheel",
			null,
			"This heavy wooden potter's wheel has a low kick wheel below and a smooth throwing head above for thrown vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			26000.0,
			60.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Pottery Tools / Potter's Wheel" ],
			[ "Holdable", "Tool_Pottery_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_press_mold",
					"preindustrial_tool_press_mold",
			"mould",
			"a fired-clay press mould",
			null,
			"This fired-clay mould has a shallow carved cavity for pressing tiles, lamps, relief panels, or repeated vessel parts.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1400.0,
			18.0m,
			false,
			false,
			"earthenware",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Pottery Tools / Press Mold" ],
			[ "Holdable", "Tool_Pottery_General", "Destroyable_Glassware" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_raising_hammer",
					"preindustrial_tool_raising_hammer",
			"hammer",
			"an armourer's raising hammer",
			null,
			"This iron raising hammer has a rounded face and narrow peen for lifting sheet metal into cups, bosses, and helmet bowls.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			28.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Armouring Tools / Raising Hammer", "Functions / Tools / Striking Tools / Hammer" ],
			[ "Holdable", "Tool_Armouring_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_slag_hammer",
					"preindustrial_tool_slag_hammer",
			"hammer",
			"a slag-breaking hammer",
			null,
			"This iron hammer has a squat head, one narrow peen, and a scarred wooden haft for breaking slag cakes and bloom crust.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1600.0,
			18.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Slag Hammer", "Functions / Tools / Striking Tools / Hammer" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_slag_skimmer",
					"preindustrial_tool_slag_skimmer",
			"skimmer",
			"a long slag skimmer",
			null,
			"This long iron skimmer has a flattened perforated end for drawing slag, clinker, and floating dross away from a hot charge.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1450.0,
			18.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Slag Skimmer" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_splitting_axe",
					"preindustrial_tool_splitting_axe",
			"axe",
			"an iron splitting axe",
			null,
			"This heavy iron axe has a wedge-thick head and a stout ash haft for cleaving rounds, billets, and stave stock.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			30.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Splitting Axe" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Weapon" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_spokeshave",
					"preindustrial_tool_spokeshave",
			"spokeshave",
			"an iron-edged spokeshave",
			null,
			"This small two-handled tool holds a narrow iron edge in a wooden body for rounding spokes, pegs, bows, shafts, and curved handle stock.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			14.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_tap_rod",
					"preindustrial_tool_tap_rod",
			"rod",
			"an iron furnace tap rod",
			null,
			"This long iron rod has a narrow chisel-like end for clearing slag channels, tapping furnace mouths, and opening clogged drains in hot mineral work.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1300.0,
			16.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Tap Rod" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_tooth_chisel",
					"preindustrial_tool_tooth_chisel",
			"chisel",
			"an iron tooth chisel",
			null,
			"This stone chisel has several small teeth across its cutting edge for refining rough-dressed stone after point work.",
			SizeCategory.Small,
			ItemQuality.Standard,
			440.0,
			12.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Stoneworking Tools / Stone Chisel" ],
			[ "Holdable", "Tool_Masonry_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_wire_cutter",
					"preindustrial_tool_wire_cutter",
			"cutter",
			"a potter's wire cutter",
			null,
			"This simple cutter is a taut wire set between two small wooden grips for cutting clay lumps and freeing thrown pots from a wheel head.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			60.0,
			4.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Pottery Tools / Wire Cutter" ],
			[ "Holdable", "Tool_Pottery_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_wood_auger",
					"preindustrial_tool_wood_auger",
			"auger",
			"an iron wood auger",
			null,
			"This iron auger has a spoon-like bit and a cross handle for boring peg holes, hinge sockets, bung holes, and frame joints.",
			SizeCategory.Small,
			ItemQuality.Standard,
			560.0,
			22.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Wood Auger" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_wood_chisel",
					"preindustrial_tool_wood_chisel",
			"chisel",
			"an iron wood chisel",
			null,
			"This narrow iron chisel has a bevelled edge and a faceted boxwood handle for mortises, hinge seats, carved fittings, and fine trimming.",
			SizeCategory.Small,
			ItemQuality.Standard,
			280.0,
			16.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Wood Chisel" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_wood_clamp",
					"preindustrial_tool_wood_clamp",
			"clamp",
			"a wedge-keyed wood clamp",
			null,
			"This wooden clamp has opposing jaws, peg holes, and a wedge key for holding boards, glued frames, shield layers, and book boards under pressure.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			16.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Wood Clamp" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_tool_wood_file",
					"preindustrial_tool_wood_file",
			"file",
			"an iron wood file",
			null,
			"This coarse iron file has a tang set into a simple wooden handle and a face cut for shaping wood, horn, bone, and soft stone.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			14.0m,
			false,
			false,
			"wrought iron",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Wood File" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Misc" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_annealing_lehr",
					"preindustrial_workshop_annealing_lehr",
			"lehr",
			"an unlit annealing lehr",
			null,
			"This long low lehr has a clay-lined tunnel and a soot-dark opening for cooling finished glass gradually without cracking it.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			72000.0,
			150.0m,
			false,
			false,
			"earthenware",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Glassblowing Tools / Annealing Lehr" ],
			[ "Holdable", "Tool_Glassblowing_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_bake_oven",
					"preindustrial_workshop_bake_oven",
			"oven",
			"a clay bake oven",
			null,
			"This domed clay oven has a low arched mouth, a soot-black roof inside, and a worn stone sill for raking coals and sliding loaves or pies.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			68000.0,
			90.0m,
			false,
			false,
			"earthenware",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Cooking" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_book_press",
					"preindustrial_workshop_book_press",
			"press",
			"a wooden book press",
			null,
			"This stout book press has two flat wooden boards, screw posts, and dark stains from glue, leather, and inked quires.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			110.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Bookbinding Tools / Book Press" ],
			[ "Holdable", "Tool_Bookbinding_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_bookbinders_sewing_frame",
					"preindustrial_workshop_bookbinders_sewing_frame",
			"frame",
			"a bookbinder's sewing frame",
			null,
			"This upright frame holds cords under tension between a base and crossbar for sewing folded quires into a book block.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			48.0m,
			false,
			false,
			"beech",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Bookbinding Tools / Bookbinder's Sewing Frame" ],
			[ "Holdable", "Tool_Bookbinding_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_brew_copper",
					"preindustrial_workshop_brew_copper",
			"copper",
			"a bronze brew copper",
			null,
			"This large bronze brewing copper has a hammered belly, broad rolled rim, and heavy lugs for heating mash water, wort, and decoctions.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			150.0m,
			false,
			false,
			"bronze",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Brewing Tools / Brew Copper" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_HeavyMetal" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_fermenting_gyle_tun",
					"preindustrial_workshop_fermenting_gyle_tun",
			"tun",
			"a fermenting gyle tun",
			null,
			"This open-topped gyle tun is made from tight oak staves and carries a faint sour smell from old fermenting ale.",
			SizeCategory.Large,
			ItemQuality.Standard,
			16500.0,
			52.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Brewing Tools / Fermenting Gyle Tun" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_forge",
					"preindustrial_workshop_forge",
			"forge",
			"an unlit smithing forge",
			null,
			"This clay-and-stone forge has a charcoal basin, tuyere mouth, and battered working lip for heating small bars, fittings, tools, and weapon stock.",
			SizeCategory.Large,
			ItemQuality.Standard,
			42000.0,
			120.0m,
			false,
			false,
			"earthenware",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Metalworking Tools / Forge" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_fulling_stocks",
					"preindustrial_workshop_fulling_stocks",
			"stocks",
			"a pair of fulling stocks",
			null,
			"These heavy wooden fulling stocks are built with stout hammers, a trough bed, and pegged framing for pounding wool cloth after soaking.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			46000.0,
			130.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools / Fulling Tools / Fulling Stocks" ],
			[ "Holdable", "Tool_Dyeing_Fulling_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_glass_glory_hole",
					"preindustrial_workshop_glass_glory_hole",
			"furnace",
			"an unlit glass glory hole",
			null,
			"This rounded furnace chamber is lined with pale refractory clay and has a working opening sized for reheating gathered glass.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			78000.0,
			170.0m,
			false,
			false,
			"earthenware",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Glassblowing Tools / Glory Hole" ],
			[ "Holdable", "Tool_Glassblowing_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_lime_kiln",
					"preindustrial_workshop_lime_kiln",
			"kiln",
			"a small lime kiln",
			null,
			"This upright kiln has a stone lower draw arch, a clay-lined shaft, and a blackened firing mouth for burning limestone, chalk, or shell into quicklime.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			96000.0,
			160.0m,
			false,
			false,
			"stone",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Primary Production Tools / Lime Kiln" ],
			[ "Holdable", "Tool_Masonry_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_lit_forge",
					"preindustrial_workshop_lit_forge",
			"forge",
			"a lit smithing forge",
			null,
			"This smithing forge glows with charcoal heat, the air above it wavering from steady bellows draught through the tuyere mouth.",
			SizeCategory.Large,
			ItemQuality.Standard,
			42300.0,
			128.0m,
			false,
			false,
			"earthenware",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Metalworking Tools / Forge", "Functions / Material Functions / Hot Fire" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Furniture" ],
			"preindustrial_workshop_forge",
			"$0 burns low and settles back into $1.",
			TimeSpan.FromHours(3.0),
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_lit_smelting_furnace",
					"preindustrial_workshop_lit_smelting_furnace",
			"furnace",
			"a lit bloomery furnace",
			null,
			"This bloomery furnace burns hot through its clay shaft, with charcoal glare at the charging mouth and slag crusted near the lower channel.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			88400.0,
			190.0m,
			false,
			false,
			"earthenware",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Smelting Furnace", "Functions / Tools / Smelting Tools / Smelting Furnace / Lit Smelting Furnace", "Functions / Material Functions / Hot Fire" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Furniture" ],
			"preindustrial_workshop_smelting_furnace",
			"$0 cools and dies back into $1.",
			TimeSpan.FromHours(5.0),
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_lying_press",
					"preindustrial_workshop_lying_press",
			"press",
			"a bookbinder's lying press",
			null,
			"This narrow lying press has paired wooden cheeks and a screw tightening bar for holding a book spine while trimming, backing, or sewing.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7600.0,
			70.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Bookbinding Tools / Lying Press" ],
			[ "Holdable", "Tool_Bookbinding_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_mash_tun",
					"preindustrial_workshop_mash_tun",
			"tun",
			"a wooden mash tun",
			null,
			"This coopered mash tun is broad, deep, and darkly stained by grain liquor, with tight staves and a fitted wooden cover.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			60.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Brewing Tools / Mash Tun" ],
			[ "Holdable", "Tool_Apothecary_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_papermakers_vat",
					"preindustrial_workshop_papermakers_vat",
			"vat",
			"a papermaker's vat",
			null,
			"This broad timber vat is stained by pale pulp and sized for dipping a mould and deckle during rag-paper production.",
			SizeCategory.Large,
			ItemQuality.Standard,
			34000.0,
			80.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Papermaking Tools / Papermaker's Vat" ],
			[ "Holdable", "Tool_Papermaking_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_pole_lathe",
					"preindustrial_workshop_pole_lathe",
			"lathe",
			"a pole-lathe frame",
			null,
			"This timber pole lathe has a simple bed, treadle cord, sprung pole, and paired centres for turning pegs, handles, bowls, beads, and small round parts.",
			SizeCategory.Large,
			ItemQuality.Standard,
			30000.0,
			85.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Woodcrafting Tools / Lathe" ],
			[ "Holdable", "Tool_Woodcrafting_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_rag_beating_trough",
					"preindustrial_workshop_rag_beating_trough",
			"trough",
			"a rag-beating trough",
			null,
			"This deep wooden trough has a scarred inner bed and a broad lip for beating soaked rags into paper pulp.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			48.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Papermaking Tools / Rag Beating Trough" ],
			[ "Holdable", "Tool_Papermaking_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_ropewalk",
					"preindustrial_workshop_ropewalk",
			"ropewalk",
			"a timber ropewalk frame",
			null,
			"This long timber frame carries fixed hooks, a turning handle, and spaced guide points for laying cordage under tension.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			28000.0,
			95.0m,
			false,
			false,
			"oak",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Textilecraft Tools" ],
			[ "Holdable", "Tool_Textilecraft_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		CreatePreIndustrialAlias(
					"medieval_workshop_smelting_furnace",
					"preindustrial_workshop_smelting_furnace",
			"furnace",
			"an unlit bloomery furnace",
			null,
			"This squat bloomery furnace is built from clay, stone, and repaired refractory lining, with a charging mouth above and a slag channel near the base.",
			SizeCategory.Huge,
			ItemQuality.Standard,
			88000.0,
			180.0m,
			false,
			false,
			"earthenware",
			[ "Market / Professional Tools / Standard Tools", "Functions / Tools / Smelting Tools / Smelting Furnace" ],
			[ "Holdable", "Tool_Blacksmithing_General", "Destroyable_Furniture" ],
			null,
			null,
			null,
			null);

		SeedPreIndustrialAntiquityTimeAndWaterAliases();
				SeedPreIndustrialNavigationOpticsAndMeasurementItems();
	}

	private void SeedPreIndustrialMilitarySupportGoods()
	{
		CreatePreIndustrialAlias(
					"medieval_military_archers_utility_belt",
					"preindustrial_military_support_archers_utility_belt",
					"belt",
					"an archer's utility belt",
					null,
					"This leather belt has several attachment points spaced for a small quiver, pouch, knife sheath, or other light gear. It is not as heavy as a sword harness, but the stitching and buckle are stronger than ordinary dress wear. The layout suits an archer or slinger who carries several small pieces of kit.",
					SizeCategory.Small,
					ItemQuality.Standard,
					460.0,
					40.0m,
					true,
					false,
					"leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Functions / Worn Items / Belts", "Market / Military Goods" ],
					[ "Holdable", "Wear_Waist", "Belt_4", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_back_arrow_quiver",
					"preindustrial_military_support_back_arrow_quiver",
					"quiver",
					"a back-worn arrow quiver",
					null,
					"This arrow quiver has a long leather body with two straps arranged so it can sit high across the back. The mouth is stiffened with rawhide, and the lower end is shaped to keep the shafts gathered. It is convenient for travel, scouts, and hunters who need their hands free before fighting starts.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					760.0,
					40.0m,
					true,
					false,
					"leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Military Goods / Ammunition" ],
					[ "Holdable", "Container_Quiver", "Wear_Backpack", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_barding_trestle",
					"preindustrial_military_support_barding_trestle",
					"trestle",
					"a barding trestle",
					"A broad barding trestle stands ready for horse armour.",
					"This broad trestle is built to carry horse armour, caparisons, and large pieces of tack without letting them pile on the floor. Its top rail is padded with worn leather, and the legs are spread wide for stability. The size makes it unmistakably an armoury support for mounted war gear.",
					SizeCategory.VeryLarge,
					ItemQuality.Good,
					32000.0,
					220.0m,
					true,
					false,
					"oak",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Household Goods / Standard Furniture" ],
					[ "Container_Armor_Stand", "Destroyable_WoodenHeavy" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_belt_bolt_case",
					"preindustrial_military_support_belt_bolt_case",
					"case",
					"a belt-hung bolt case",
					null,
					"This small bolt case has a stiff leather body, a reinforced mouth, and a strong rear tab meant for fastening to a belt or harness. It is sized for a modest supply of bolts rather than a full archer's load. The shape keeps quarrels upright and ready at the hip.",
					SizeCategory.Small,
					ItemQuality.Standard,
					440.0,
					28.0m,
					true,
					false,
					"cow leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Military Goods / Ammunition" ],
					[ "Holdable", "Container_Quiver", "Beltable", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_bow_rack",
					"preindustrial_military_support_bow_rack",
					"rack",
					"a bow rack",
					"A notched bow rack stands here.",
					"This tall rack has long notched arms and a narrow shelf for bowstrings, spare nocks, or small archer's gear. The notches are smoothed so bow staves will not scrape badly when lifted out. It is plainly made for orderly storage rather than display alone.",
					SizeCategory.VeryLarge,
					ItemQuality.Standard,
					16000.0,
					100.0m,
					true,
					false,
					"ash",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Household Goods / Standard Furniture" ],
					[ "Container_Weapon_Rack", "Destroyable_WoodenHeavy" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_broad_seax_sheath",
					"preindustrial_military_support_broad_seax_sheath",
					"sheath",
					"a broad seax sheath",
					null,
					"This broad sheath is cut wide and flat to take a long-backed knife or seax-like blade. Dark leather is wrapped over a thin wooden core, with little bronze rivets along the seam and a pair of suspension points set near the mouth. It looks meant to hang horizontally or at a shallow angle from a belt.",
					SizeCategory.Normal,
					ItemQuality.Good,
					260.0,
					36.0m,
					true,
					false,
					"leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Sheath_Small", "Beltable", "Destroyable_Weapon" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_broad_war_arrow_quiver",
					"preindustrial_military_support_broad_war_arrow_quiver",
					"quiver",
					"a broad war-arrow quiver",
					null,
					"This broad quiver has a wider mouth than usual, letting thick-shafted arrows and broadheads sit without crowding. The leather body is reinforced with rawhide strips and a heavy bottom plug. It is bulky, but it is clearly made for carrying serious field ammunition rather than a few hunting shafts.",
					SizeCategory.Large,
					ItemQuality.Good,
					980.0,
					80.0m,
					true,
					false,
					"cow leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Military Goods / Ammunition" ],
					[ "Holdable", "Container_Quiver", "Wear_Shoulder", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_broad_weapon_baldric",
					"preindustrial_military_support_broad_weapon_baldric",
					"baldric",
					"a broad weapon baldric",
					null,
					"This broad baldric is made from heavy leather with several iron rings and reinforced straps along the lower half. It can carry more than a single light sheath, making it suitable for larger weapons or combined sidearm gear. The whole piece is rugged, practical, and plainly military.",
					SizeCategory.Normal,
					ItemQuality.Good,
					720.0,
					72.0m,
					true,
					false,
					"cow leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Functions / Worn Items / Belts", "Market / Military Goods" ],
					[ "Holdable", "Wear_Sash", "Belt_4", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_broad_weapon_belt",
					"preindustrial_military_support_broad_weapon_belt",
					"belt",
					"a broad weapon belt",
					null,
					"This broad weapon belt is made from thick leather with a wide buckle and reinforced stitched panels. Several sturdy hanging points are spaced around it so scabbards, pouches, or other beltable gear can be arranged without crowding. It is heavier than a common belt but better suited to field equipment.",
					SizeCategory.Small,
					ItemQuality.Good,
					560.0,
					48.0m,
					true,
					false,
					"cow leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Functions / Worn Items / Belts", "Market / Military Goods" ],
					[ "Holdable", "Wear_Waist", "Belt_4", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_bronze_fitted_sword_scabbard",
					"preindustrial_military_support_bronze_fitted_sword_scabbard",
					"scabbard",
					"a bronze-fitted sword scabbard",
					null,
					"This scabbard has a smooth leather cover drawn tight over a shaped wooden core. Bronze fittings strengthen the mouth, suspension bands, and chape, giving it a warm glint without making it ostentatious. The fit and finish are better than common armoury issue.",
					SizeCategory.Normal,
					ItemQuality.Good,
					820.0,
					96.0m,
					true,
					false,
					"leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Sheath", "Beltable", "Destroyable_Weapon" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_crossbow_rack",
					"preindustrial_military_support_crossbow_rack",
					"rack",
					"a crossbow rack",
					"A sturdy crossbow rack occupies part of the room.",
					"This broad wooden rack has deep rests for crossbow stocks and a lower rail that keeps the bows from banging together. Its frame is heavier than an ordinary weapon rack, with extra bracing to support the awkward shape of loaded or unstrung crossbows. It belongs in an armoury, gatehouse, or siege store.",
					SizeCategory.VeryLarge,
					ItemQuality.Good,
					28000.0,
					180.0m,
					true,
					false,
					"oak",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Household Goods / Standard Furniture" ],
					[ "Container_Weapon_Rack", "Destroyable_WoodenHeavy" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_double_hanger_sword_belt",
					"preindustrial_military_support_double_hanger_sword_belt",
					"belt",
					"a double-hanger sword belt",
					null,
					"This sword belt has two angled hangers riveted to a firm leather body, letting a scabbard sit at a controlled slant. Additional small attachment points leave room for a dagger sheath or pouch. The buckles are brass, neat enough for a professional retainer without becoming courtly finery.",
					SizeCategory.Small,
					ItemQuality.Good,
					520.0,
					64.0m,
					true,
					false,
					"leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Functions / Worn Items / Belts", "Market / Military Goods" ],
					[ "Holdable", "Wear_Waist", "Belt_4", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_fine_bolt_case",
					"preindustrial_military_support_fine_bolt_case",
					"case",
					"a fine bolt case",
					null,
					"This well-made bolt case has a leather-covered body with neat brass fittings at the mouth and base. The shoulder strap is broad and carefully stitched, and the interior is shaped to keep bolts upright. Its polish and balance mark it as gear for a well-equipped crossbowman.",
					SizeCategory.Normal,
					ItemQuality.VeryGood,
					720.0,
					120.0m,
					true,
					false,
					"goat leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Military Goods / Ammunition" ],
					[ "Holdable", "Container_Quiver", "Wear_Shoulder", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_fine_display_armour_stand",
					"preindustrial_military_support_fine_display_armour_stand",
					"stand",
					"a fine armour display stand",
					"A fine armour display stand stands here.",
					"This fine armour stand is made from polished hardwood with shaped shoulders, a carved helm post, and brass fittings on the main joints. It remains a functional support for armour, but the smoother finish makes it suitable for a noble hall or well-kept armoury. The base is broad and carefully weighted.",
					SizeCategory.VeryLarge,
					ItemQuality.VeryGood,
					28000.0,
					480.0m,
					true,
					false,
					"walnut",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Household Goods / Standard Furniture" ],
					[ "Container_Armor_Stand", "Destroyable_WoodenHeavy" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_fine_dyed_dagger_sheath",
					"preindustrial_military_support_fine_dyed_dagger_sheath",
					"sheath",
					"a fine $colour dagger sheath",
					null,
					"This fine dagger sheath is formed from smooth dyed leather over a slim inner core. The seam is hidden beneath a raised central strip, and small brass fittings protect the throat and chape. It has the clean finish of a sidearm accessory made to be seen as well as used.",
					SizeCategory.Small,
					ItemQuality.VeryGood,
					210.0,
					72.0m,
					true,
					false,
					"goat leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Sheath_Small", "Beltable", "Destroyable_Weapon", "Variable_FineColour" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_fine_dyed_sword_scabbard",
					"preindustrial_military_support_fine_dyed_sword_scabbard",
					"scabbard",
					"a fine $colour sword scabbard",
					null,
					"This fine sword scabbard is covered in smooth dyed leather and finished with polished brass fittings. The throat is cleanly shaped, the chape is neatly seated, and the suspension bands are balanced so the weapon would hang at a comfortable angle. It is practical military equipment with enough refinement for an elite retainer.",
					SizeCategory.Normal,
					ItemQuality.VeryGood,
					780.0,
					180.0m,
					true,
					false,
					"goat leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Sheath", "Beltable", "Destroyable_Weapon", "Variable_FineColour" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_fine_sword_belt",
					"preindustrial_military_support_fine_sword_belt",
					"belt",
					"a fine $colour sword belt",
					null,
					"This fine sword belt is made from smooth dyed leather with polished brass fittings and carefully cut hangers. The edges are burnished, the stitching is even, and the belt is balanced to carry a scabbard without sagging. It is still practical military gear, but it clearly belongs to someone of means.",
					SizeCategory.Small,
					ItemQuality.VeryGood,
					500.0,
					140.0m,
					true,
					false,
					"goat leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Functions / Worn Items / Belts", "Market / Military Goods" ],
					[ "Holdable", "Wear_Waist", "Belt_4", "Destroyable_Clothing", "Variable_FineColour" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_heavy_war_belt",
					"preindustrial_military_support_heavy_war_belt",
					"belt",
					"a heavy war belt",
					null,
					"This heavy belt is cut from doubled leather and fitted with large iron buckles, rings, and suspension points. It is built to carry several pieces of attached military gear without twisting out of shape. The size and weight make it plainly a harness belt rather than everyday dress.",
					SizeCategory.Normal,
					ItemQuality.Good,
					820.0,
					84.0m,
					true,
					false,
					"cow leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Functions / Worn Items / Belts", "Market / Military Goods" ],
					[ "Holdable", "Wear_Waist", "Belt_6", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_iron_armoury_rack",
					"preindustrial_military_support_iron_armoury_rack",
					"rack",
					"an iron armoury rack",
					"An iron armoury rack stands here, dark and sturdy.",
					"This heavy iron rack has upright bars, cross rails, and hooks arranged for an assortment of weapons. The frame is blackened and workmanlike, with enough mass to stay put even when loaded unevenly. It is costly, durable armoury furniture rather than something meant for campaign travel.",
					SizeCategory.VeryLarge,
					ItemQuality.Good,
					42000.0,
					360.0m,
					true,
					false,
					"wrought iron",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Household Goods / Standard Furniture" ],
					[ "Container_Weapon_Rack", "Destroyable_HeavyMetal" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_lacquered_arrow_quiver",
					"preindustrial_military_support_lacquered_arrow_quiver",
					"quiver",
					"a lacquered arrow quiver",
					null,
					"This fine arrow quiver has a smooth lacquered body, a leather-lined mouth, and careful cord binding around the suspension points. The surface has a glossy finish that sheds grime and catches the light along its curve. It is sturdy enough for war gear while still carrying the polish of a high-status archer's kit.",
					SizeCategory.Normal,
					ItemQuality.VeryGood,
					720.0,
					160.0m,
					true,
					false,
					"lacquer",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Military Goods / Ammunition" ],
					[ "Holdable", "Container_Quiver", "Wear_Shoulder", "Destroyable_Misc" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_leather_baldric",
					"preindustrial_military_support_leather_baldric",
					"baldric",
					"a leather weapon baldric",
					null,
					"This long leather baldric is worn from shoulder to hip, with a reinforced lower section for attaching a scabbard or similar beltable gear. The strap is broad enough to spread weight across the shoulder and plain enough for field use. Its buckles and stitching are sturdy rather than ornate.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					480.0,
					36.0m,
					true,
					false,
					"leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Functions / Worn Items / Belts", "Market / Military Goods" ],
					[ "Holdable", "Wear_Sash", "Belt_2", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_leather_bound_short_sword_scabbard",
					"preindustrial_military_support_leather_bound_short_sword_scabbard",
					"scabbard",
					"a short sword scabbard",
					null,
					"This shorter scabbard has a stiff leather body over a narrow wooden lining, ending in a rounded metal chape. The suspension points sit close to the throat so it can be worn high at the hip. Its compact length suits a short sword, side sword, or other short military blade.",
					SizeCategory.Small,
					ItemQuality.Standard,
					520.0,
					32.0m,
					true,
					false,
					"leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Sheath", "Beltable", "Destroyable_Weapon" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_long_sword_scabbard",
					"preindustrial_military_support_long_sword_scabbard",
					"scabbard",
					"a long sword scabbard",
					null,
					"This long scabbard is built for a large sword with a straight double-edged blade. Its leather cover is stretched over a wooden core, with a reinforced throat and a long iron chape protecting the lower end. Broad suspension straps let it hang from a belt or baldric rather than flopping loose at the side.",
					SizeCategory.Large,
					ItemQuality.Standard,
					1050.0,
					72.0m,
					true,
					false,
					"leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Sheath_Large", "Beltable", "Destroyable_Weapon" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_mail_armour_stand",
					"preindustrial_military_support_mail_armour_stand",
					"stand",
					"a mail armour stand",
					"A mail armour stand stands here with broad wooden shoulders.",
					"This stand has broad rounded shoulders and a tall central body, shaped to support a mail shirt or long hauberk without folding it too tightly. The base is heavy, and a lower peg gives room for a coif, belt, or chausses. It is storage gear for keeping heavy linked armour accessible and aired out.",
					SizeCategory.VeryLarge,
					ItemQuality.Good,
					26000.0,
					170.0m,
					true,
					false,
					"oak",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Household Goods / Standard Furniture" ],
					[ "Container_Armor_Stand", "Destroyable_WoodenHeavy" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_mounted_harness_stand",
					"preindustrial_military_support_mounted_harness_stand",
					"stand",
					"a mounted harness stand",
					"A large stand for mounted armour occupies the floor.",
					"This large armour stand has broad supports and several projecting arms for a rider's harness, saddle pieces, and heavier limb defences. The base is wide enough to resist the pull of awkward equipment hanging to one side. It is a practical piece for an elite stable armoury or knightly store room.",
					SizeCategory.VeryLarge,
					ItemQuality.Good,
					36000.0,
					240.0m,
					true,
					false,
					"oak",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Household Goods / Standard Furniture" ],
					[ "Container_Armor_Stand", "Destroyable_WoodenHeavy" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_mounted_weapon_harness",
					"preindustrial_military_support_mounted_weapon_harness",
					"harness",
					"a mounted weapon harness",
					null,
					"This harness combines a waist belt and crossing shoulder strap, giving several strong points for attaching scabbards, pouches, or other beltable gear. The leather is thick, the rings are iron, and the straps are arranged to keep weight from shifting too much while riding. It is heavier and more complicated than an ordinary sword belt.",
					SizeCategory.Normal,
					ItemQuality.Good,
					980.0,
					120.0m,
					true,
					false,
					"cow leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Functions / Worn Items / Belts", "Market / Military Goods" ],
					[ "Holdable", "Wear_Bandolier", "Belt_6", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_plain_arming_belt",
					"preindustrial_military_support_plain_arming_belt",
					"belt",
					"a plain arming belt",
					null,
					"This sturdy leather belt is broad enough to sit comfortably over a tunic, gambeson, or mail shirt. It has a simple iron buckle and two strong attachment points for small sheaths or pouches. Nothing about it is decorative; it is meant to keep military gear where the wearer can reach it.",
					SizeCategory.Small,
					ItemQuality.Standard,
					380.0,
					24.0m,
					true,
					false,
					"cow leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Functions / Worn Items / Belts", "Market / Military Goods" ],
					[ "Holdable", "Wear_Waist", "Belt_2", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_plain_bolt_case",
					"preindustrial_military_support_plain_bolt_case",
					"case",
					"a plain bolt case",
					null,
					"This compact case is made from stiff leather with a squared mouth and a firm bottom. It is shorter and broader than an arrow quiver, shaped for crossbow bolts rather than long shafts. A shoulder strap and simple flap keep the case manageable in the field without turning it into a locked container.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					620.0,
					36.0m,
					true,
					false,
					"leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Military Goods / Ammunition" ],
					[ "Holdable", "Container_Quiver", "Wear_Shoulder", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_plain_dagger_sheath",
					"preindustrial_military_support_plain_dagger_sheath",
					"sheath",
					"a plain dagger sheath",
					null,
					"This narrow dagger sheath has a firm leather body and a tight mouth shaped to grip a straight blade. The seam is stitched and lightly glued, while the back carries a simple belt loop. It is a practical soldier's piece, darkened with oil and made without ornament.",
					SizeCategory.Small,
					ItemQuality.Standard,
					140.0,
					14.0m,
					true,
					false,
					"cow leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Sheath_Small", "Beltable", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_plain_leather_knife_sheath",
					"preindustrial_military_support_plain_leather_knife_sheath",
					"sheath",
					"a plain leather knife sheath",
					null,
					"This compact leather sheath has a straight stitched seam, a rounded point, and a small belt tab at the back. The leather is dark from oiling and shaped closely enough to hold a fighting knife without rattling. Its only decoration is a double row of neat stitching around the mouth.",
					SizeCategory.Small,
					ItemQuality.Standard,
					90.0,
					8.0m,
					true,
					false,
					"cow leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Sheath_Small", "Beltable", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_plain_shoulder_arrow_quiver",
					"preindustrial_military_support_plain_shoulder_arrow_quiver",
					"quiver",
					"a plain shoulder arrow quiver",
					null,
					"This practical arrow quiver has a firm leather tube, a reinforced mouth, and a broad strap for wearing it over the shoulder. The bottom is padded with a thick plug so arrow points do not quickly chew through the end. It is made for field use rather than display.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					680.0,
					32.0m,
					true,
					false,
					"leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Military Goods / Ammunition" ],
					[ "Holdable", "Container_Quiver", "Wear_Shoulder", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_plain_sword_scabbard",
					"preindustrial_military_support_plain_sword_scabbard",
					"scabbard",
					"a plain sword scabbard",
					null,
					"This straight sword scabbard is built around a light wooden core covered in dark leather. The mouth is neatly fitted, the point is protected by a small iron chape, and plain suspension straps are set to hang from a sword belt. It is soldierly, sturdy, and deliberately undecorated.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					700.0,
					36.0m,
					true,
					false,
					"leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Sheath", "Beltable", "Destroyable_Weapon" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_polearm_rack",
					"preindustrial_military_support_polearm_rack",
					"rack",
					"a polearm rack",
					"A tall polearm rack rises near the wall.",
					"This tall rack is built with a heavy base, a high notched rail, and enough depth to keep long weapons upright. It is suited to spears, pikes, axes on long shafts, and other awkward polearms. The timber is scarred and rubbed where many shafts have been set in and lifted out.",
					SizeCategory.VeryLarge,
					ItemQuality.Good,
					34000.0,
					200.0m,
					true,
					false,
					"oak",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Household Goods / Standard Furniture" ],
					[ "Container_Weapon_Rack", "Destroyable_WoodenHeavy" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_portable_campaign_weapon_rack",
					"preindustrial_military_support_portable_campaign_weapon_rack",
					"rack",
					"a portable campaign weapon rack",
					null,
					"This folding wooden rack is made from crossed legs, a pegged top rail, and a lower brace that can be lashed shut for travel. It is lighter than an armoury rack and plainly meant for temporary camps, guard posts, or field musters. The construction is simple enough that it can be moved by hand when empty.",
					SizeCategory.Large,
					ItemQuality.Standard,
					9500.0,
					88.0m,
					true,
					false,
					"ash",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Container_Weapon_Rack", "Destroyable_WoodenHeavy" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_rawhide_axe_edge_cover",
					"preindustrial_military_support_rawhide_axe_edge_cover",
					"cover",
					"a rawhide axe-edge cover",
					null,
					"This crescent-shaped rawhide cover is cut to wrap around the edge of an axe head. A pair of thongs lace across the back so it can be tied shut for travel or storage. It offers no carrying harness by itself, but it keeps a sharp blade from nicking shields, packs, or hands.",
					SizeCategory.Small,
					ItemQuality.Standard,
					120.0,
					6.0m,
					true,
					false,
					"rawhide",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Sheath_Small", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_reinforced_dagger_sheath",
					"preindustrial_military_support_reinforced_dagger_sheath",
					"sheath",
					"a reinforced dagger sheath",
					null,
					"This dagger sheath has a leather-covered wooden body with a small iron chape fitted over the point. The throat is bound in a narrow metal band, and the belt loop is riveted rather than merely stitched. It is made for a fighting dagger that sees hard service at the waist.",
					SizeCategory.Small,
					ItemQuality.Good,
					220.0,
					32.0m,
					true,
					false,
					"leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Sheath_Small", "Beltable", "Destroyable_Weapon" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_reinforced_large_weapon_sheath",
					"preindustrial_military_support_reinforced_large_weapon_sheath",
					"sheath",
					"a reinforced large weapon sheath",
					null,
					"This large sheath is made from heavy leather stiffened around a broad wooden liner. Its open throat and long side seam make it suitable for an oversized blade or heavy chopping weapon rather than a narrow sword alone. Iron rivets, a capped end, and a strong suspension strap show that it is meant for serious field gear.",
					SizeCategory.Large,
					ItemQuality.Good,
					1450.0,
					120.0m,
					true,
					false,
					"cow leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Sheath_Large", "Beltable", "Destroyable_Weapon" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_riders_side_quiver",
					"preindustrial_military_support_riders_side_quiver",
					"quiver",
					"a rider's side quiver",
					null,
					"This compact quiver is shaped to hang at the side rather than down the back, with a stiff mouth and angled suspension straps. The body is leather over rawhide, strong enough to hold arrows steady while mounted. Its form is suited to mobile archers who need to draw without twisting far in the saddle.",
					SizeCategory.Normal,
					ItemQuality.Good,
					820.0,
					72.0m,
					true,
					false,
					"rawhide",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Military Goods / Ammunition" ],
					[ "Holdable", "Container_Quiver", "Wear_Waist", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_riding_weapon_belt",
					"preindustrial_military_support_riding_weapon_belt",
					"belt",
					"a riding weapon belt",
					null,
					"This weapon belt is cut to sit securely while mounted, with a broad rear section and forward-set hangers. The leather is reinforced where a scabbard or quiver would pull against it, and the buckle is offset to avoid digging into the body. It is practical gear for a rider rather than a parade belt.",
					SizeCategory.Small,
					ItemQuality.Good,
					620.0,
					72.0m,
					true,
					false,
					"cow leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Functions / Worn Items / Belts", "Market / Military Goods" ],
					[ "Holdable", "Wear_Waist", "Belt_4", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_rough_leather_arrow_quiver",
					"preindustrial_military_support_rough_leather_arrow_quiver",
					"quiver",
					"a rough leather arrow quiver",
					null,
					"This simple quiver is made from stiff leather with a folded bottom and a broad shoulder strap. The mouth is left open and slightly oval, allowing arrows to be drawn quickly even though the finish is crude. It is the sort of cheap missile gear issued to militia archers or carried on campaign until it wears out.",
					SizeCategory.Normal,
					ItemQuality.Substandard,
					520.0,
					16.0m,
					true,
					false,
					"cow leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Military Goods / Ammunition" ],
					[ "Holdable", "Container_Quiver", "Wear_Shoulder", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_rough_rawhide_knife_sheath",
					"preindustrial_military_support_rough_rawhide_knife_sheath",
					"sheath",
					"a rough rawhide knife sheath",
					null,
					"This small sheath is folded from stiff rawhide and closed with coarse stitching along one edge. The mouth is slightly flared so a short knife can be pushed in without much care, and a narrow thong lets it hang from other gear. It is crude but serviceable, made more for keeping an edge covered than for display.",
					SizeCategory.Small,
					ItemQuality.Substandard,
					70.0,
					4.0m,
					true,
					false,
					"rawhide",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Sheath_Small", "Beltable", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_simple_armour_stand",
					"preindustrial_military_support_simple_armour_stand",
					"stand",
					"a simple armour stand",
					"A simple wooden armour stand waits here.",
					"This armour stand is built from a central wooden post, a shoulder bar, and a low brace for hanging a mail shirt, padded coat, or light harness. The shape is more practical than lifelike, meant to keep armour from lying in a heap. It is plain armoury furniture with no attempt at display carving.",
					SizeCategory.Large,
					ItemQuality.Standard,
					12000.0,
					80.0m,
					true,
					false,
					"pine",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Household Goods / Standard Furniture" ],
					[ "Container_Armor_Stand", "Destroyable_WoodenHeavy" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_simple_sword_belt",
					"preindustrial_military_support_simple_sword_belt",
					"belt",
					"a simple sword belt",
					null,
					"This sword belt is a plain leather strap with a serviceable buckle and a pair of hangers for a scabbard. The leather is firm but not thick, meant for an arming sword or similar sidearm rather than a great weapon. It is common military kit for someone expected to wear a blade regularly.",
					SizeCategory.Small,
					ItemQuality.Standard,
					420.0,
					32.0m,
					true,
					false,
					"leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Functions / Worn Items / Belts", "Market / Military Goods" ],
					[ "Holdable", "Wear_Waist", "Belt_2", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_simple_weapon_rack",
					"preindustrial_military_support_simple_weapon_rack",
					"rack",
					"a simple weapon rack",
					"A simple wooden weapon rack stands against the wall.",
					"This upright rack is built from plain timber with slots and rests for several hand weapons. The base is broad enough to keep it steady, while the top rail has uneven notches worn smooth from use. It is a workmanlike fixture for a guardroom, barracks, or small armoury.",
					SizeCategory.VeryLarge,
					ItemQuality.Standard,
					22000.0,
					120.0m,
					true,
					false,
					"pine",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Household Goods / Standard Furniture" ],
					[ "Container_Weapon_Rack", "Destroyable_WoodenHeavy" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_spearhead_rawhide_cover",
					"preindustrial_military_support_spearhead_rawhide_cover",
					"cover",
					"a rawhide spearhead cover",
					null,
					"This small rawhide cover is shaped like a stiff cone with a stitched side and a short tying thong. It is meant to slip over a spearhead or similar point to keep the edge from cutting hands, packs, or other gear during travel. The surface is plain, scarred, and practical.",
					SizeCategory.Small,
					ItemQuality.Standard,
					85.0,
					5.0m,
					true,
					false,
					"rawhide",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Sheath_Small", "Destroyable_Clothing" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_sturdy_armour_tree",
					"preindustrial_military_support_sturdy_armour_tree",
					"stand",
					"a sturdy armour tree",
					"A sturdy armour tree stands ready for harness.",
					"This strong armour tree has a wide base, a thick central post, and several shaped arms for helmet, mail, limb pieces, and belts. It can hold heavier armour without tipping easily. The timber is rubbed smooth where equipment is often hung and lifted away.",
					SizeCategory.VeryLarge,
					ItemQuality.Good,
					24000.0,
					160.0m,
					true,
					false,
					"oak",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Household Goods / Standard Furniture" ],
					[ "Container_Armor_Stand", "Destroyable_WoodenHeavy" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_sword_rack",
					"preindustrial_military_support_sword_rack",
					"rack",
					"a sword rack",
					"A sword rack stands ready to hold blades.",
					"This wooden rack has a series of narrow slots and padded rests sized for swords and scabbarded blades. The frame is compact but solid, with a low rail to keep points from scraping the floor. It is designed for an armoury or hall where sidearms need to be kept orderly.",
					SizeCategory.Large,
					ItemQuality.Good,
					14000.0,
					110.0m,
					true,
					false,
					"oak",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Household Goods / Standard Furniture" ],
					[ "Container_Weapon_Rack", "Destroyable_WoodenHeavy" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_tooled_leather_knife_sheath",
					"preindustrial_military_support_tooled_leather_knife_sheath",
					"sheath",
					"a $colour tooled leather knife sheath",
					null,
					"This small sheath is made from firm dyed leather, its front face pressed with simple running borders and small punched circles. The throat is reinforced with a second strip of leather to resist wear from repeated drawing. A narrow loop on the back lets it ride from a belt or larger harness.",
					SizeCategory.Small,
					ItemQuality.Good,
					110.0,
					18.0m,
					true,
					false,
					"leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Sheath_Small", "Beltable", "Destroyable_Clothing", "Variable_BasicColour" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_wall_spear_rack",
					"preindustrial_military_support_wall_spear_rack",
					"rack",
					"a wall-mounted spear rack",
					"A long spear rack is fixed along one wall.",
					"This long wooden rack has a lower rail for spear butts and an upper rail cut with shallow notches for shafts. Iron brackets hold the frame firmly, and the spacing is broad enough for spears or light polearms. It is intended as armoury furniture rather than a thing to carry about.",
					SizeCategory.VeryLarge,
					ItemQuality.Standard,
					18000.0,
					96.0m,
					true,
					false,
					"oak",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Household Goods / Standard Furniture" ],
					[ "Container_Weapon_Rack", "Destroyable_WoodenHeavy" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_wicker_arrow_quiver",
					"preindustrial_military_support_wicker_arrow_quiver",
					"quiver",
					"a wicker arrow quiver",
					null,
					"This light quiver is woven from wicker around a narrow base and bound with leather at the mouth and bottom. It is lighter than a thick leather case, though more vulnerable to hard knocks and wet weather. A shoulder strap lets it hang close against the body while keeping arrows readily to hand.",
					SizeCategory.Normal,
					ItemQuality.Standard,
					430.0,
					24.0m,
					true,
					false,
					"wicker",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Military Goods / Ammunition" ],
					[ "Holdable", "Container_Quiver", "Wear_Shoulder", "Destroyable_Misc" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_wooden_crossbow_bolt_box",
					"preindustrial_military_support_wooden_crossbow_bolt_box",
					"box",
					"a wooden crossbow bolt box",
					null,
					"This short wooden box is divided inside to keep crossbow bolts from tangling together. Its lidless upper edge allows quick access, while a leather strap lets it be carried over the shoulder. It is heavier than a soft case but protects its contents better from crushing in a wagon or armoury.",
					SizeCategory.Normal,
					ItemQuality.Good,
					1250.0,
					64.0m,
					true,
					false,
					"oak",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods", "Market / Military Goods / Ammunition" ],
					[ "Holdable", "Container_Quiver", "Wear_Shoulder", "Destroyable_WoodenHeavy" ],
					null,
					null,
					null,
					null);

		CreatePreIndustrialAlias(
					"medieval_military_worn_sword_scabbard",
					"preindustrial_military_support_worn_sword_scabbard",
					"scabbard",
					"a worn sword scabbard",
					null,
					"This sword scabbard has a leather-covered wooden body rubbed pale along the edges and repaired at the throat with mismatched stitching. A dull iron chape caps the point, and two suspension bands remain serviceable despite their age. It would suit an old sword still kept in an armoury or passed down through rough hands.",
					SizeCategory.Normal,
					ItemQuality.Substandard,
					620.0,
					18.0m,
					true,
					false,
					"leather",
					[ "Era / Medieval Era", "Functions / Military Equipment", "Market / Military Goods" ],
					[ "Holdable", "Sheath", "Beltable", "Destroyable_Weapon" ],
					null,
					null,
					null,
					null);

		SeedPreIndustrialGunpowderSupportItems();
	}

}
