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
			InstallCatalogueItem(CompiledAuthoredItemCatalogue.Get(stableReference, "SeedAntiquityWritingImplementsAndDocuments"));
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

		InstallCatalogueItem(Historical01Stock.AntiquityLoosePapyrusSheet());

		InstallCatalogueItem(Historical01Stock.AntiquityPapyrusSheetBundle());

		InstallCatalogueItem(Historical01Stock.AntiquitySimplePapyrusScroll());

		InstallCatalogueItem(Historical01Stock.AntiquitySealedPapyrusScroll());

		var looseParchmentSheet = InstallCatalogueItem(Historical01Stock.AntiquityLooseParchmentSheet());

		InstallCatalogueItem(Historical01Stock.AntiquityParchmentBifolium());

		InstallCatalogueItem(Historical01Stock.AntiquityParchmentQuire());

		InstallCatalogueItem(Historical01Stock.AntiquityParchmentScroll());

		EnsureAntiquityBookComponent("Antiquity_Parchment_Codex",
			"Allows a compact parchment codex to hold readable parchment pages", 48, looseParchmentSheet!);
		InstallCatalogueItem(Historical01Stock.AntiquityParchmentCodex());

		InstallCatalogueItem(Historical01Stock.AntiquityWaxWritingTablet());

		InstallCatalogueItem(Historical01Stock.AntiquityHingedWaxDiptych());

		InstallCatalogueItem(Historical01Stock.AntiquityWaxTriptych());

		InstallCatalogueItem(Historical01Stock.AntiquityUnfiredClayTablet());

		InstallCatalogueItem(Historical01Stock.AntiquityFiredClayTablet());

		InstallCatalogueItem(Historical01Stock.AntiquityClayTabletEnvelope());

		InstallCatalogueItem(Historical01Stock.AntiquitySmoothedWoodenWritingBlock());

		InstallCatalogueItem(Historical01Stock.AntiquityPotsherdOstracon());

		InstallCatalogueItem(Historical01Stock.AntiquityReedPen());

		InstallCatalogueItem(Historical01Stock.AntiquityQuillPen());

		InstallCatalogueItem(Historical01Stock.AntiquityInkBrush());

		InstallCatalogueItem(Historical01Stock.AntiquityCharcoalWritingStick());

		InstallCatalogueItem(Historical01Stock.AntiquityBoneStylus());

		InstallCatalogueItem(Historical01Stock.AntiquityBronzeStylus());

		InstallCatalogueItem(Historical01Stock.AntiquityReedStylus());

		InstallCatalogueItem(Historical01Stock.AntiquityBronzePenKnife());

		InstallCatalogueItem(Historical01Stock.AntiquityBronzeScraperKnife());

		InstallCatalogueItem(Historical01Stock.AntiquitySootInkCake());

		InstallCatalogueItem(Historical01Stock.AntiquitySmallInkwell());

		InstallCatalogueItem(Historical01Stock.AntiquityLiquidBlackInkPot());

		InstallCatalogueItem(Historical01Stock.AntiquityCedarScrollCase());

		InstallCatalogueItem(Historical01Stock.AntiquityLeatherCodexPouch());

		InstallCatalogueItem(Historical01Stock.AntiquityLinenTabletWrap());

		InstallCatalogueItem(Historical01Stock.AntiquityPapyrusScrollTie());
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
		var manifestDefinition = new ComponentManifestDefinition(name, description, type, 0, definition);
		var manifestEntry = RegisterManifestAggregate("component", name, manifestDefinition);
		if (_components.TryGetValue(name, out var existing))
		{
			var liveDefinition = new ComponentManifestDefinition(
				existing.Name, existing.Description, existing.Type, existing.RevisionNumber, existing.Definition);
			var disposition = InspectManifestAggregate(manifestEntry, existing.Id, liveDefinition);
			if (disposition == ManifestAggregateDisposition.Customized)
			{
				return CacheAntiquityWritingComponent(existing);
			}
			existing.Type = type;
			existing.Description = description;
			existing.Definition = definition;
			if (disposition == ManifestAggregateDisposition.Update)
			{
				CompleteManifestAggregate(manifestEntry, existing.Id, manifestDefinition, disposition);
			}
			return CacheAntiquityWritingComponent(existing);
		}

		existing = _context!.GameItemComponentProtos.Local
			.AsEnumerable()
			.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ??
		           _context.GameItemComponentProtos.AsEnumerable()
					   .FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
			                                x.EditableItem.RevisionStatus == 4);
		if (existing is not null)
		{
			var liveDefinition = new ComponentManifestDefinition(
				existing.Name, existing.Description, existing.Type, existing.RevisionNumber, existing.Definition);
			var disposition = InspectManifestAggregate(manifestEntry, existing.Id, liveDefinition);
			if (disposition == ManifestAggregateDisposition.Customized)
			{
				return CacheAntiquityWritingComponent(existing);
			}
			existing.Type = type;
			existing.Description = description;
			existing.Definition = definition;
			if (disposition == ManifestAggregateDisposition.Update)
			{
				CompleteManifestAggregate(manifestEntry, existing.Id, manifestDefinition, disposition);
			}
			return CacheAntiquityWritingComponent(existing);
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
		CompleteManifestAggregate(manifestEntry, component.Id, manifestDefinition, ManifestAggregateDisposition.Insert);
		return CacheAntiquityWritingComponent(component);
	}

	private GameItemComponentProto CacheAntiquityWritingComponent(GameItemComponentProto component)
	{
		_components[component.Name] = component;
		_componentNamesByKey[(component.Id, component.RevisionNumber)] = component.Name;
		return component;
	}
}
