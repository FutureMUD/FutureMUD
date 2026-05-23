using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using System;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedAntiquityWritingImplementsAndDocuments()
	{
		var papyrusSheet = EnsureAntiquityPaperSheetComponent("Antiquity_Papyrus_Sheet_Surface",
			"Allows an antiquity papyrus sheet to be written on", 2400);
		var papyrusScroll = EnsureAntiquityPaperSheetComponent("Antiquity_Papyrus_Scroll_Surface",
			"Allows a papyrus scroll to be written on as a long sheet", 12000);
		var parchmentSheet = EnsureAntiquityPaperSheetComponent("Antiquity_Parchment_Sheet_Surface",
			"Allows an antiquity parchment sheet to be written on", 3600);
		var parchmentBifolium = EnsureAntiquityPaperSheetComponent("Antiquity_Parchment_Bifolium_Surface",
			"Allows a folded parchment bifolium to be written on", 7200);
		var parchmentScroll = EnsureAntiquityPaperSheetComponent("Antiquity_Parchment_Scroll_Surface",
			"Allows a parchment scroll to be written on as a long sheet", 15000);

		var waxTablet = EnsureAntiquityInscribableSurfaceComponent("Antiquity_Wax_Tablet_Surface",
			"Allows wax writing tablets to take stylus writing", 1800, WritingImplementType.Stylus);
		var waxDiptych = EnsureAntiquityInscribableSurfaceComponent("Antiquity_Wax_Diptych_Surface",
			"Allows paired wax tablets to take stylus writing", 3600, WritingImplementType.Stylus);
		var waxTriptych = EnsureAntiquityInscribableSurfaceComponent("Antiquity_Wax_Triptych_Surface",
			"Allows tripled wax tablets to take stylus writing", 5400, WritingImplementType.Stylus);
		var clayTablet = EnsureAntiquityInscribableSurfaceComponent("Antiquity_Clay_Tablet_Surface",
			"Allows clay tablets to take stylus writing", 1600, WritingImplementType.Stylus);
		var woodBlock = EnsureAntiquityInscribableSurfaceComponent("Antiquity_Wooden_Block_Surface",
			"Allows wooden writing blocks to take incised or charcoal writing", 2200,
			WritingImplementType.Stylus, WritingImplementType.Chisel, WritingImplementType.Charcoal);
		var ostracon = EnsureAntiquityInscribableSurfaceComponent("Antiquity_Ostracon_Surface",
			"Allows ostraca to take ink, brush or charcoal marks", 900,
			WritingImplementType.ReedPen, WritingImplementType.Quill, WritingImplementType.Brush,
			WritingImplementType.Charcoal);

		var reedPen = EnsureAntiquityScribingImplementComponent("Antiquity_Reed_Pen",
			"Turns an item into a finite black reed pen", WritingImplementType.ReedPen, "black", 7000);
		var quillPen = EnsureAntiquityScribingImplementComponent("Antiquity_Quill_Pen",
			"Turns an item into a finite black quill pen", WritingImplementType.Quill, "black", 9000);
		var inkBrush = EnsureAntiquityScribingImplementComponent("Antiquity_Ink_Brush",
			"Turns an item into a finite black ink brush", WritingImplementType.Brush, "black", 6000);
		var charcoalStick = EnsureAntiquityScribingImplementComponent("Antiquity_Charcoal_Stick",
			"Turns an item into a finite charcoal writing stick", WritingImplementType.Charcoal, "black", 2500);
		var boneStylus = EnsureAntiquityScribingImplementComponent("Antiquity_Bone_Stylus",
			"Turns an item into a non-consuming bone stylus", WritingImplementType.Stylus, "black", 0);
		var bronzeStylus = EnsureAntiquityScribingImplementComponent("Antiquity_Bronze_Stylus",
			"Turns an item into a non-consuming bronze stylus", WritingImplementType.Stylus, "black", 0);
		var reedStylus = EnsureAntiquityScribingImplementComponent("Antiquity_Reed_Stylus",
			"Turns an item into a non-consuming reed stylus", WritingImplementType.Stylus, "black", 0);

		void AddWritingCraftTool(string stableReference, string noun, string shortDescription,
			string fullDescription, SizeCategory size, double weightInGrams, decimal cost, string material,
			string[] functionalTags)
		{
			CreateItem(
				stableReference,
				noun,
				shortDescription,
				null,
				fullDescription,
				size,
				ItemQuality.Standard,
				weightInGrams,
				cost,
				false,
				false,
				material,
				["Market / Professional Tools / Standard Tools", .. functionalTags],
				["Holdable", (int)size >= (int)SizeCategory.Large ? "Destroyable_Furniture" : "Destroyable_Misc"],
				null,
				null,
				null,
				null
			);
		}

		AddWritingCraftTool(
			"antiquity_papyrus_strip_knife",
			"knife",
			"a bronze papyrus strip knife",
			"A narrow bronze knife with a straight edge, kept for slicing soaked papyrus pith into long even strips before pressing.",
			SizeCategory.VerySmall,
			85.0,
			7.0m,
			"bronze",
			["Functions / Tools / Papyrusmaking Tools / Papyrus Strip Knife"]);

		AddWritingCraftTool(
			"antiquity_papyrus_pressing_board",
			"board",
			"a papyrus pressing board",
			"A flat cedar board darkened by damp papyrus sheets, used with weights or clamps to press crossed pith strips into a firm writing sheet.",
			SizeCategory.Normal,
			1800.0,
			10.0m,
			"cedar",
			["Functions / Tools / Papyrusmaking Tools / Papyrus Pressing Board"]);

		AddWritingCraftTool(
			"antiquity_papyrus_burnishing_shell",
			"shell",
			"a smooth papyrus burnishing shell",
			"A polished shell worn satin-smooth along one edge, used to burnish dried papyrus sheets until ink will sit cleanly on the surface.",
			SizeCategory.Tiny,
			35.0,
			4.0m,
			"shell",
			["Functions / Tools / Papyrusmaking Tools / Papyrus Burnishing Shell"]);

		AddWritingCraftTool(
			"antiquity_parchment_scraping_knife",
			"knife",
			"a curved parchment scraping knife",
			"A curved bronze scraping knife with a dulled back and keen working edge, made for thinning stretched parchment without cutting through it.",
			SizeCategory.VerySmall,
			120.0,
			9.0m,
			"bronze",
			["Functions / Tools / Parchmentmaking Tools / Parchment Scraping Knife"]);

		AddWritingCraftTool(
			"antiquity_parchment_stretching_frame",
			"frame",
			"a wooden parchment stretching frame",
			"A rectangular wooden frame pierced around the edges for cords, used to stretch wet hide taut while it is scraped into parchment.",
			SizeCategory.Large,
			6200.0,
			22.0m,
			"wood",
			["Functions / Tools / Parchmentmaking Tools / Parchment Stretching Frame"]);

		AddWritingCraftTool(
			"antiquity_parchment_pumice",
			"pumice",
			"a piece of parchment pumice",
			"A light abrasive stone used to smooth parchment, remove grease, and prepare a surface to take ink evenly.",
			SizeCategory.Tiny,
			55.0,
			3.0m,
			"stone",
			["Functions / Tools / Parchmentmaking Tools / Parchment Pumice"]);

		AddWritingCraftTool(
			"antiquity_bookbinders_needle",
			"needle",
			"a bronze bookbinder's needle",
			"A stout bronze needle with a broad eye, sized for sewing parchment quires, scroll ties, and leather document cases.",
			SizeCategory.Tiny,
			18.0,
			5.0m,
			"bronze",
			["Functions / Tools / Bookbinding Tools / Bookbinder's Needle"]);

		AddWritingCraftTool(
			"antiquity_bookbinders_punch",
			"punch",
			"a bronze bookbinder's punch",
			"A short bronze punch with a wooden grip, made for opening clean sewing holes through quires, boards, and leather covers.",
			SizeCategory.VerySmall,
			110.0,
			7.0m,
			"bronze",
			["Functions / Tools / Bookbinding Tools / Bookbinder's Punch"]);

		AddWritingCraftTool(
			"antiquity_scroll_roller_rod",
			"rod",
			"a cedar scroll roller rod",
			"A slender cedar rod smoothed for winding sheets into a scroll, with enough stiffness to keep the document from creasing.",
			SizeCategory.Small,
			90.0,
			4.0m,
			"cedar",
			["Functions / Tools / Scrollmaking Tools / Scroll Roller Rod"]);

		AddWritingCraftTool(
			"antiquity_scroll_smoothing_stone",
			"stone",
			"a scroll smoothing stone",
			"A flat, rounded stone used to ease joins, press glued sheets, and smooth scroll surfaces without tearing the fibres.",
			SizeCategory.Tiny,
			180.0,
			3.0m,
			"stone",
			["Functions / Tools / Scrollmaking Tools / Scroll Smoothing Stone"]);

		AddWritingCraftTool(
			"antiquity_quill_curing_sand",
			"sand",
			"a tray of quill curing sand",
			"A shallow fired-clay tray filled with clean hot-work sand, used to dry and harden quills before trimming them into pens.",
			SizeCategory.Small,
			950.0,
			5.0m,
			"fired clay",
			["Functions / Tools / Calligraphy Tools / Quill Curing Sand"]);

		AddWritingCraftTool(
			"antiquity_wax_spatula",
			"spatula",
			"a bronze wax spatula",
			"A small bronze spatula with a flattened blade, used for spreading warm wax into tablet recesses and scraping it level again.",
			SizeCategory.Tiny,
			55.0,
			5.0m,
			"bronze",
			["Functions / Tools / Scribing Tools / Wax Spatula"]);

		AddWritingCraftTool(
			"antiquity_pigment_muller",
			"muller",
			"a stone pigment muller",
			"A rounded stone muller with a worn flat underside, used to grind soot, minerals, and lake pigments into fine ink and paint stock.",
			SizeCategory.Small,
			620.0,
			6.0m,
			"stone",
			["Functions / Tools / Illumination Tools / Pigment Muller"]);

		AddWritingCraftTool(
			"antiquity_pigment_shell",
			"shell",
			"a pigment mixing shell",
			"A shallow shell with a polished hollow, used as a tiny palette for wet pigment, ink binder, and writing colour.",
			SizeCategory.Tiny,
			28.0,
			3.0m,
			"shell",
			["Functions / Tools / Illumination Tools / Pigment Shell"]);

		CreateItem(
			"antiquity_loose_papyrus_sheet",
			"sheet",
			"a loose papyrus sheet",
			null,
			"This pale papyrus sheet is made from pressed strips laid crosswise and burnished smooth. Its surface is firm enough for ink while still showing faint plant fibres at the edges.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			4.0,
			1.2m,
			false,
			false,
			"papyrus",
			["Functions / Writing Surface / Loose Sheet", "Materials / Writing Product", "Market / Writing Materials / Papyrus"],
			["Holdable", "Stack_Number", "Destroyable_Paper", papyrusSheet.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_papyrus_sheet_bundle",
			"bundle",
			"a bundle of loose papyrus sheets",
			null,
			"This bundle holds trimmed papyrus sheets stacked together with a light tie. The edges show the pale grid of pressed pith, ready to be separated for letters, accounts, or records.",
			SizeCategory.Small,
			ItemQuality.Standard,
			120.0,
			12.0m,
			false,
			false,
			"papyrus",
			["Materials / Writing Product", "Market / Writing Materials / Papyrus"],
			["Holdable", "Stack_Number", "Destroyable_Paper"],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_simple_papyrus_scroll",
			"scroll",
			"a simple papyrus scroll",
			null,
			"This papyrus scroll is built from joined sheets wound around a plain wooden rod. The writing surface is smooth and pale, with a neat outer edge and enough body to unroll without tearing at once.",
			SizeCategory.Small,
			ItemQuality.Standard,
			65.0,
			10.0m,
			false,
			false,
			"papyrus",
			["Functions / Writing Surface / Scroll", "Materials / Writing Product", "Market / Writing Materials / Scrolls"],
			["Holdable", "Destroyable_Paper", papyrusScroll.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_sealed_papyrus_scroll",
			"scroll",
			"a tied and sealed papyrus scroll",
			null,
			"This papyrus scroll is rolled tight, bound with a linen tie, and prepared for a small seal at the knot. Its outer turn protects the writing surface inside from casual handling.",
			SizeCategory.Small,
			ItemQuality.Standard,
			80.0,
			14.0m,
			false,
			false,
			"papyrus",
			["Functions / Writing Surface / Scroll", "Materials / Writing Product", "Market / Writing Materials / Scrolls"],
			["Holdable", "Destroyable_Paper", papyrusScroll.Name],
			null,
			null,
			null,
			null
		);

		var looseParchmentSheet = CreateItem(
			"antiquity_loose_parchment_sheet",
			"sheet",
			"a loose parchment sheet",
			null,
			"This parchment sheet is thin, pale, and faintly translucent where it has been scraped and dressed. One side is smoother than the other, but both are fit for careful ink work.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			6.0,
			2.5m,
			false,
			false,
			"parchment",
			["Functions / Writing Surface / Loose Sheet", "Materials / Writing Product", "Market / Writing Materials / Parchment"],
			["Holdable", "Stack_Number", "Destroyable_Paper", parchmentSheet.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_parchment_bifolium",
			"bifolium",
			"a folded parchment bifolium",
			null,
			"This single parchment sheet has been folded once to make a paired leaf. The crease is clean and the outer corners have been trimmed square for use in a small quire or booklet.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			12.0,
			4.0m,
			false,
			false,
			"parchment",
			["Functions / Writing Surface / Loose Sheet", "Materials / Writing Product", "Market / Writing Materials / Parchment"],
			["Holdable", "Destroyable_Paper", parchmentBifolium.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_parchment_quire",
			"quire",
			"a nested parchment quire",
			null,
			"This quire is made from nested folded parchment leaves, pricked near the fold for later sewing. The leaves are matched by size and scraped smooth enough for a compact hand.",
			SizeCategory.Small,
			ItemQuality.Standard,
			65.0,
			16.0m,
			false,
			false,
			"parchment",
			["Materials / Writing Product", "Market / Writing Materials / Parchment"],
			["Holdable", "Destroyable_Paper"],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_parchment_scroll",
			"scroll",
			"a parchment scroll",
			null,
			"This parchment scroll is stitched from prepared sheets and rolled with the grain. The surface is tougher than papyrus and carries a faint scrape pattern beneath its smooth finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			130.0,
			22.0m,
			false,
			false,
			"parchment",
			["Functions / Writing Surface / Scroll", "Materials / Writing Product", "Market / Writing Materials / Scrolls"],
			["Holdable", "Destroyable_Paper", parchmentScroll.Name],
			null,
			null,
			null,
			null
		);

		EnsureAntiquityBookComponent("Antiquity_Parchment_Codex",
			"Allows a compact parchment codex to hold readable parchment pages", 48, looseParchmentSheet!);
		CreateItem(
			"antiquity_parchment_codex",
			"codex",
			"a small parchment codex",
			null,
			"This compact codex is made from folded parchment quires sewn between thin wooden boards. Its cover is plain leather, practical rather than ornate, and the leaves open flat enough for close writing.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			60.0m,
			false,
			false,
			"parchment",
			["Functions / Writing Surface / Codex", "Materials / Writing Product", "Market / Writing Materials / Codices"],
			["Holdable", "Destroyable_Paper", "Antiquity_Parchment_Codex"],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_wax_writing_tablet",
			"tablet",
			"a waxed wooden writing tablet",
			null,
			"This wooden tablet is recessed and filled with darkened beeswax. The wax is smooth enough to take a stylus line and soft enough to be scraped back for reuse.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			12.0m,
			false,
			false,
			"beeswax",
			["Functions / Writing Surface / Wax Tablet", "Materials / Writing Product", "Market / Writing Materials / Wax Tablets"],
			["Holdable", "Destroyable_Misc", waxTablet.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_hinged_wax_diptych",
			"diptych",
			"a hinged wax tablet diptych",
			null,
			"This pair of waxed tablets is joined with small cord hinges so the writing faces close against each other. Each recessed panel is filled with smoothed wax for stylus notes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			850.0,
			25.0m,
			false,
			false,
			"beeswax",
			["Functions / Writing Surface / Wax Tablet", "Materials / Writing Product", "Market / Writing Materials / Wax Tablets"],
			["Holdable", "Destroyable_Misc", waxDiptych.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_wax_triptych",
			"triptych",
			"a three-leaf wax tablet triptych",
			null,
			"This writing tablet has three waxed leaves bound together through drilled hinge holes. The closing leaves protect the inner wax panels, making it useful for account notes or memoranda.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1250.0,
			36.0m,
			false,
			false,
			"beeswax",
			["Functions / Writing Surface / Wax Tablet", "Materials / Writing Product", "Market / Writing Materials / Wax Tablets"],
			["Holdable", "Destroyable_Misc", waxTriptych.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_unfired_clay_tablet",
			"tablet",
			"an unfired clay writing tablet",
			null,
			"This damp clay tablet has been flattened and squared for inscription. It is still soft enough to receive a stylus, with smoothed faces and slightly raised edges.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			1.5m,
			false,
			false,
			"clay",
			["Functions / Writing Surface / Clay Tablet", "Materials / Writing Product", "Market / Writing Materials / Clay Tablets"],
			["Holdable", "Destroyable_Misc", clayTablet.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_fired_clay_tablet",
			"tablet",
			"a fired clay writing tablet",
			null,
			"This clay tablet has been fired hard after inscription or preparation. Its surface is firm and ceramic, preserving marks more permanently than a damp tablet would.",
			SizeCategory.Small,
			ItemQuality.Standard,
			610.0,
			3.0m,
			false,
			false,
			"fired clay",
			["Functions / Writing Surface / Clay Tablet", "Materials / Writing Product", "Market / Writing Materials / Clay Tablets"],
			["Holdable", "Destroyable_Misc", clayTablet.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_clay_tablet_envelope",
			"envelope",
			"a clay tablet envelope",
			null,
			"This thin clay envelope is shaped to enclose a smaller tablet before being sealed and marked on the outside. Its edges are pinched shut but still show the intended tablet shape.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			2.0m,
			false,
			false,
			"clay",
			["Functions / Writing Surface / Clay Tablet", "Materials / Writing Product", "Market / Writing Materials / Clay Tablets"],
			["Holdable", "Destroyable_Misc", clayTablet.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_smoothed_wooden_writing_block",
			"block",
			"a smoothed wooden writing block",
			null,
			"This small cedar block has a planed face prepared for charcoal, ink, or shallow incised marks. The reverse is left slightly rough so it can be gripped while writing.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			4.0m,
			false,
			false,
			"cedar",
			["Functions / Writing Surface / Wooden Writing Block", "Materials / Writing Product", "Market / Writing Materials"],
			["Holdable", "Destroyable_Misc", woodBlock.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_potsherd_ostracon",
			"ostracon",
			"a smoothed potsherd ostracon",
			null,
			"This broken piece of fired clay has been selected for its flat face and rubbed smooth at the sharper edges. It is useful for brief notes, accounts, labels, or practice writing.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			75.0,
			0.4m,
			false,
			false,
			"fired clay",
			["Functions / Writing Surface / Ostracon", "Materials / Writing Product", "Market / Writing Materials / Clay Tablets"],
			["Holdable", "Destroyable_Misc", ostracon.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_reed_pen",
			"pen",
			"a cut reed pen",
			null,
			"This reed pen is trimmed to a narrow point and split at the nib to carry ink. The shaft is light, plain, and easy to recut as the point wears down.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			6.0,
			1.2m,
			false,
			false,
			"papyrus",
			["Market / Writing Materials / Writing Implements", "Functions / Tools / Scribing Tools / Reed Pen"],
			["Holdable", "Destroyable_Misc", reedPen.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_quill_pen",
			"quill",
			"a trimmed quill pen",
			null,
			"This quill has been cured, scraped, and cut to a writing nib. The feathering is trimmed back from the hand, leaving a light pen ready for ink.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			5.0,
			1.5m,
			false,
			false,
			"feather",
			["Market / Writing Materials / Writing Implements", "Functions / Tools / Scribing Tools / Quill Pen"],
			["Holdable", "Destroyable_Misc", quillPen.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_ink_brush",
			"brush",
			"a fine ink brush",
			null,
			"This small brush has soft bristles bound into a slim wooden handle. Its point can carry ink for bold lines, labels, and more pictorial writing.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			18.0,
			3.0m,
			false,
			false,
			"wood",
			["Market / Writing Materials / Writing Implements", "Functions / Tools / Scribing Tools / Ink Brush"],
			["Holdable", "Destroyable_Misc", inkBrush.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_charcoal_writing_stick",
			"stick",
			"a charcoal writing stick",
			null,
			"This narrow charcoal stick has been rubbed clean enough to hold without crumbling at once. It makes dark, dusty marks on wood, pottery, plaster, and rough practice surfaces.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			12.0,
			0.5m,
			false,
			false,
			"charcoal",
			["Market / Writing Materials / Writing Implements", "Functions / Tools / Scribing Tools / Charcoal Stick"],
			["Holdable", "Destroyable_Misc", charcoalStick.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_bone_stylus",
			"stylus",
			"a polished bone stylus",
			null,
			"This bone stylus is smoothed to a rounded grip with a narrow writing point and a flattened end for erasing wax. It is light enough for quick tablet work.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			22.0,
			2.0m,
			false,
			false,
			"bone",
			["Market / Writing Materials / Writing Implements", "Functions / Tools / Scribing Tools / Stylus"],
			["Holdable", "Destroyable_Misc", boneStylus.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_bronze_stylus",
			"stylus",
			"a bronze stylus",
			null,
			"This bronze stylus has a tapered point for wax or clay and a broader spatulate end for smoothing marks. Its surface is polished from repeated handling.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			45.0,
			5.0m,
			false,
			false,
			"bronze",
			["Market / Writing Materials / Writing Implements", "Functions / Tools / Scribing Tools / Stylus"],
			["Holdable", "Destroyable_Misc", bronzeStylus.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_reed_stylus",
			"stylus",
			"a sharpened reed stylus",
			null,
			"This reed stylus is cut square and sharpened for pressing marks into clay. It is disposable compared to bone or bronze, but very quick to make and replace.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			5.0,
			0.5m,
			false,
			false,
			"papyrus",
			["Market / Writing Materials / Writing Implements", "Functions / Tools / Scribing Tools / Stylus"],
			["Holdable", "Destroyable_Misc", reedStylus.Name],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_bronze_pen_knife",
			"knife",
			"a small bronze pen knife",
			null,
			"This small bronze knife has a short blade made for trimming reed pens, recutting quills, and cleaning stubborn fibres from writing stock. It is more precise than forceful.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			70.0,
			6.0m,
			false,
			false,
			"bronze",
			["Market / Professional Tools / Standard Tools", "Functions / Tools / Scribing Tools / Pen Knife", "Functions / Separation / Cutting / Knife"],
			["Holdable", "Destroyable_Misc"],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_bronze_scraper_knife",
			"scraper",
			"a bronze parchment scraper",
			null,
			"This bronze scraper knife has a crescent edge for thinning parchment, scraping wax, and cleaning writing boards. Its grip is broad enough for careful pressure.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			95.0,
			8.0m,
			false,
			false,
			"bronze",
			["Market / Professional Tools / Standard Tools", "Functions / Tools / Scribing Tools / Scraper Knife", "Functions / Separation / Precision Cutting"],
			["Holdable", "Destroyable_Misc"],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_soot_ink_cake",
			"cake",
			"a soot-black ink cake",
			null,
			"This hard little ink cake is made from soot, binder, and a trace of resin. It can be rubbed with water to make black writing ink as needed.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			3.0m,
			false,
			false,
			"soot",
			["Market / Writing Materials / Ink", "Functions / Material Functions / Writing Craft Stock / Ink Stock"],
			["Holdable", "Destroyable_Misc"],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_small_inkwell",
			"inkwell",
			"a small clay inkwell",
			null,
			"This small fired-clay inkwell has a narrow mouth and a heavy base to resist tipping. Dark staining around the rim shows where brushes and pens have been dipped.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			160.0,
			4.0m,
			false,
			false,
			"fired clay",
			["Market / Writing Materials / Ink", "Functions / Tools / Scribing Tools / Inkwell"],
			["Holdable", "Destroyable_Misc"],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_liquid_black_ink_pot",
			"inkwell",
			"a small inkwell of black ink",
			null,
			"This small inkwell is filled with dark carbon ink, thick enough to cling to a reed pen or brush. A fitted stopper helps keep dust and grit out between uses.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			190.0,
			6.0m,
			false,
			false,
			"soot",
			["Market / Writing Materials / Ink", "Functions / Tools / Scribing Tools / Inkwell"],
			["Holdable", "Destroyable_Misc"],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_cedar_scroll_case",
			"case",
			"a cedar scroll case",
			null,
			"This narrow cedar case is bored and smoothed to hold one or two rolled documents. A fitted cap keeps the scrolls from sliding out, and the wood carries a dry resinous scent.",
			SizeCategory.Small,
			ItemQuality.Standard,
			450.0,
			18.0m,
			false,
			false,
			"cedar",
			["Functions / Container", "Market / Writing Materials / Document Containers"],
			["Holdable", "Destroyable_Misc", "Container_Quiver"],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_leather_codex_pouch",
			"pouch",
			"a leather document pouch",
			null,
			"This flat leather pouch is sized for folded sheets, small codices, or tied document packets. Its flap is long enough to tuck under a cord and protect the contents from dust.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			14.0m,
			false,
			false,
			"leather",
			["Functions / Container", "Market / Writing Materials / Document Containers"],
			["Holdable", "Destroyable_Misc", "Container_Pouch"],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_linen_tablet_wrap",
			"wrap",
			"a linen tablet wrap",
			null,
			"This padded linen wrap folds around wax or clay tablets and ties with two narrow bands. It is plain, washable, and meant to stop tablet faces from rubbing together in a satchel.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			90.0,
			4.0m,
			false,
			false,
			"linen",
			["Functions / Container", "Market / Writing Materials / Document Containers"],
			["Holdable", "Destroyable_Misc", "Container_Pouch"],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"antiquity_papyrus_scroll_tie",
			"tie",
			"a narrow linen scroll tie",
			null,
			"This narrow linen tie is sized to bind a small scroll without crushing it. One end is left long enough for a label tag or seal cord.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			8.0,
			0.6m,
			false,
			false,
			"linen",
			["Market / Writing Materials / Document Containers", "Functions / Joining / Tie"],
			["Holdable", "Destroyable_Misc", "Stack_Number"],
			null,
			null,
			null,
			null
		);
	}

	private GameItemComponentProto EnsureAntiquityPaperSheetComponent(string name, string description, int maxCharacters)
	{
		return EnsureAntiquityWritingComponent("PaperSheet", name, description,
			new XElement("Definition",
				new XElement("MaximumCharacterLengthOfText", maxCharacters)).ToString());
	}

	private GameItemComponentProto EnsureAntiquityBookComponent(string name, string description, int pages,
		GameItemProto pageItem)
	{
		return EnsureAntiquityWritingComponent("Book", name, description,
			new XElement("Definition",
				new XElement("PaperProto", pageItem.Id),
				new XElement("PageCount", pages)).ToString());
	}

	private GameItemComponentProto EnsureAntiquityScribingImplementComponent(string name, string description,
		WritingImplementType implementType, string colourName, int totalUses)
	{
		var colour = _context!.Colours.AsEnumerable().FirstOrDefault(x => x.Name.Equals(colourName, StringComparison.OrdinalIgnoreCase));
		return EnsureAntiquityWritingComponent("ScribingImplement", name, description,
			new XElement("Definition",
				new XElement("ImplementType", implementType.ToString()),
				new XElement("Colour", colour?.Id ?? 0),
				new XElement("ColourCharacteristic", 0),
				new XElement("TotalUses", totalUses)).ToString());
	}

	private GameItemComponentProto EnsureAntiquityInscribableSurfaceComponent(string name, string description,
		int maxCharacters, params WritingImplementType[] allowedImplements)
	{
		return EnsureAntiquityWritingComponent("InscribableSurface", name, description,
			new XElement("Definition",
				new XElement("MaximumCharacterLengthOfText", maxCharacters),
				new XElement("AllowedImplementTypes",
					allowedImplements
						.Distinct()
						.Select(x => new XElement("Type", x.ToString())))).ToString());
	}

	private GameItemComponentProto EnsureAntiquityWritingComponent(string type, string name, string description,
		string definition)
	{
		if (_components.TryGetValue(name, out var existing))
		{
			existing.Type = type;
			existing.Description = description;
			existing.Definition = definition;
			return existing;
		}

		existing = _context!.GameItemComponentProtos.Local
			.AsEnumerable()
			.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ??
		           _context.GameItemComponentProtos.AsEnumerable()
					   .FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
			                                x.EditableItem.RevisionStatus == 4);
		if (existing is not null)
		{
			existing.Type = type;
			existing.Description = description;
			existing.Definition = definition;
			_components[name] = existing;
			return existing;
		}

		var component = new GameItemComponentProto
		{
			Id = _components.Values.Select(x => x.Id).DefaultIfEmpty().Max() + 1,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = _dbAccount.Id,
				BuilderDate = _now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = _dbAccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = _now
			},
			Type = type,
			Name = name,
			Description = description,
			Definition = definition
		};

		_context.GameItemComponentProtos.Add(component);
		_components[name] = component;
		return component;
	}
}
