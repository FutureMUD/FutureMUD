#nullable enable

using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private enum PreIndustrialItemGroup
	{
		Printing,
		NavigationAndScience,
		GunpowderSupport,
		GlobalTradePackaging
	}

	private sealed record PreIndustrialItemSpec(
		PreIndustrialItemGroup Group,
		string StableReference,
		string Noun,
		string ShortDescription,
		string FullDescription,
		SizeCategory Size,
		ItemQuality Quality,
		double WeightInGrams,
		decimal Cost,
		string Material,
		string[] Tags,
		string[] Components,
		bool FixedFixture = false);

	internal sealed record PreIndustrialItemSpecTestData(
		string Group,
		string StableReference,
		string Material,
		IReadOnlyCollection<string> Tags,
		IReadOnlyCollection<string> Components,
		bool FixedFixture);

	private static readonly PreIndustrialItemSpec[] PreIndustrialNewItemSpecs =
	[
		new(PreIndustrialItemGroup.Printing, "preindustrial_printing_hand_press", "press", "a timber hand press", "This tall timber press has a broad platen, a heavy screw, and a waist-high bed framed by stout uprights.", SizeCategory.VeryLarge, ItemQuality.Standard, 48000.0, 120.0m, "oak", ["Era / Pre-Industrial Era", "Functions / Tools / Movable Type Printing Tools", "Market / Professional Tools / Standard Tools"], ["Destroyable_Furniture"], true),
		new(PreIndustrialItemGroup.Printing, "preindustrial_printing_type_case", "case", "a shallow type case", "This shallow oak case is divided into many small compartments, each marked by handling and dark residue.", SizeCategory.Normal, ItemQuality.Standard, 4800.0, 36.0m, "oak", ["Era / Pre-Industrial Era", "Functions / Container", "Functions / Tools / Movable Type Printing Tools", "Market / Professional Tools / Standard Tools"], ["Holdable", "Destroyable_Furniture", "Container_Archive_Box"]),
		new(PreIndustrialItemGroup.Printing, "preindustrial_printing_composing_stick", "stick", "a brass composing stick", "This hand-length brass tray has a low back, a sliding stop, and thumb-polished edges.", SizeCategory.Small, ItemQuality.Good, 420.0, 28.0m, "brass", ["Era / Pre-Industrial Era", "Functions / Tools / Movable Type Printing Tools", "Market / Professional Tools / Standard Tools"], ["Holdable"]),
		new(PreIndustrialItemGroup.Printing, "preindustrial_printing_chase", "chase", "a rectangular printer's chase", "This rigid brass frame is rectangular, flat, and worn along its inner edges by repeated packing and tightening.", SizeCategory.Normal, ItemQuality.Standard, 1800.0, 32.0m, "brass", ["Era / Pre-Industrial Era", "Functions / Tools / Movable Type Printing Tools", "Market / Professional Tools / Standard Tools"], ["Holdable"]),
		new(PreIndustrialItemGroup.Printing, "preindustrial_printing_inking_balls", "balls", "a pair of leather inking balls", "These two padded leather balls have short wooden handles and dark, tacky staining across their rounded faces.", SizeCategory.Small, ItemQuality.Standard, 520.0, 18.0m, "leather", ["Era / Pre-Industrial Era", "Functions / Tools / Movable Type Printing Tools", "Market / Professional Tools / Standard Tools"], ["Holdable"]),
		new(PreIndustrialItemGroup.Printing, "preindustrial_printing_drying_rack", "rack", "a slatted paper drying rack", "This tall oak rack presents many narrow slats and open gaps for laying or hanging fresh sheets apart.", SizeCategory.VeryLarge, ItemQuality.Standard, 18500.0, 52.0m, "oak", ["Era / Pre-Industrial Era", "Functions / Tools / Movable Type Printing Tools", "Market / Professional Tools / Standard Tools"], ["Destroyable_Furniture"], true),
		new(PreIndustrialItemGroup.Printing, "preindustrial_printing_broadside_sheet", "broadside", "a printed broadside sheet", "This single paper sheet bears a dense block of dark text beneath a large heading, with generous margins around the impression.", SizeCategory.Small, ItemQuality.Standard, 8.0, 3.0m, "paper", ["Era / Pre-Industrial Era", "Functions / Writing Goods", "Market / Writing Materials / Paper"], ["Holdable", "Destroyable_Paper"]),
		new(PreIndustrialItemGroup.Printing, "preindustrial_printing_pamphlet", "pamphlet", "a stitched paper pamphlet", "Several folded paper leaves are nested and secured by a simple thread stitch, their outer leaf carrying a dark title block.", SizeCategory.Small, ItemQuality.Standard, 42.0, 8.0m, "paper", ["Era / Pre-Industrial Era", "Functions / Writing Goods", "Market / Writing Materials / Paper"], ["Holdable", "Destroyable_Paper"]),
		new(PreIndustrialItemGroup.Printing, "preindustrial_printing_almanac", "almanac", "a compact printed almanac", "This compact paper booklet has closely set pages, small tables, and a plain stitched spine beneath a heavier cover leaf.", SizeCategory.Small, ItemQuality.Standard, 95.0, 14.0m, "paper", ["Era / Pre-Industrial Era", "Functions / Writing Goods", "Market / Writing Materials / Paper"], ["Holdable", "Destroyable_Paper"]),
		new(PreIndustrialItemGroup.Printing, "preindustrial_printing_blank_form", "form", "a ruled blank paper form", "This clean paper sheet is divided by straight rules into labelled spaces, leaving broad blank fields for later entries.", SizeCategory.Small, ItemQuality.Standard, 8.0, 3.0m, "paper", ["Era / Pre-Industrial Era", "Functions / Writing Surface", "Market / Writing Materials / Paper"], ["Holdable", "Medieval_Rag_Paper_Sheet_Surface", "Destroyable_Paper"]),
		new(PreIndustrialItemGroup.Printing, "preindustrial_printing_printed_map_sheet", "map", "a printed map sheet", "This broad paper sheet carries fine dark lines, small place labels, and a ruled border around a carefully arranged map.", SizeCategory.Small, ItemQuality.Good, 12.0, 12.0m, "paper", ["Era / Pre-Industrial Era", "Functions / Writing Goods", "Market / Writing Materials / Paper"], ["Holdable", "Destroyable_Paper"]),

		new(PreIndustrialItemGroup.NavigationAndScience, "preindustrial_navigation_magnetic_compass", "compass", "a brass magnetic compass", "A small brass case surrounds a balanced dark needle and a plainly divided circular card beneath glass.", SizeCategory.VerySmall, ItemQuality.Good, 280.0, 45.0m, "brass", ["Era / Pre-Industrial Era", "Functions / Tools / Scientific Tools / Navigational Tools", "Market / Professional Tools / Standard Tools"], ["Holdable"]),
		new(PreIndustrialItemGroup.NavigationAndScience, "preindustrial_navigation_pair_of_dividers", "dividers", "a pair of brass dividers", "Two straight brass legs meet at a firm hinge, ending in fine points polished by repeated adjustment.", SizeCategory.Small, ItemQuality.Good, 180.0, 24.0m, "brass", ["Era / Pre-Industrial Era", "Functions / Tools / Scientific Tools / Measurement Tools", "Market / Professional Tools / Standard Tools"], ["Holdable"]),
		new(PreIndustrialItemGroup.NavigationAndScience, "preindustrial_navigation_cross_staff", "staff", "a graduated wooden cross-staff", "This straight oak staff is cut with measured marks and carries a sliding crosspiece with squared edges.", SizeCategory.Normal, ItemQuality.Standard, 950.0, 22.0m, "oak", ["Era / Pre-Industrial Era", "Functions / Tools / Scientific Tools / Navigational Tools / Mariner's Cross-Staff", "Market / Professional Tools / Standard Tools"], ["Holdable"]),
		new(PreIndustrialItemGroup.NavigationAndScience, "preindustrial_navigation_mariner_astrolabe", "astrolabe", "a brass mariner's astrolabe", "This heavy brass ring encloses a pivoting sighting rule and a plainly graduated lower arc.", SizeCategory.Small, ItemQuality.Good, 1100.0, 68.0m, "brass", ["Era / Pre-Industrial Era", "Functions / Tools / Scientific Tools / Navigational Tools / Astrolabe", "Market / Professional Tools / Standard Tools"], ["Holdable"]),
		new(PreIndustrialItemGroup.NavigationAndScience, "preindustrial_navigation_chart_case", "case", "a leather chart case", "This stiff leather case is long and cylindrical, with a fitted cap and a carrying loop beside its stitched seam.", SizeCategory.Normal, ItemQuality.Standard, 760.0, 18.0m, "leather", ["Era / Pre-Industrial Era", "Functions / Container", "Market / Writing Materials / Document Containers"], ["Holdable", "Container_Document_Pouch"]),
		new(PreIndustrialItemGroup.NavigationAndScience, "preindustrial_surveying_measuring_chain", "chain", "a brass measuring chain", "This long chain is assembled from short brass links, with larger marker links and a ring at either end.", SizeCategory.Normal, ItemQuality.Standard, 4200.0, 42.0m, "brass", ["Era / Pre-Industrial Era", "Functions / Tools / Scientific Tools / Measurement Tools / Surveying Equipment / Surveyor's Chain", "Market / Professional Tools / Standard Tools"], ["Holdable"]),
		new(PreIndustrialItemGroup.NavigationAndScience, "preindustrial_surveying_plane_table", "table", "a wooden plane table", "This waist-high oak table has a broad flat board, folding legs, and a smooth upper face marked by pinholes and faint ruled lines.", SizeCategory.Large, ItemQuality.Standard, 14500.0, 48.0m, "oak", ["Era / Pre-Industrial Era", "Functions / Tools / Scientific Tools / Plane Table", "Market / Professional Tools / Standard Tools"], ["Destroyable_Furniture"], true),
		new(PreIndustrialItemGroup.NavigationAndScience, "preindustrial_optics_spectacles", "spectacles", "a pair of brass-rimmed spectacles", "Two clear round lenses sit in narrow brass rims joined by a curved bridge, with short folding arms at the sides.", SizeCategory.Tiny, ItemQuality.Good, 65.0, 36.0m, "brass", ["Era / Pre-Industrial Era", "Functions / Tools / Scientific Tools", "Market / Professional Tools / Standard Tools"], ["Holdable"]),
		new(PreIndustrialItemGroup.NavigationAndScience, "preindustrial_optics_magnifying_lens", "lens", "a hand-held glass lens", "A clear convex glass disc is secured in a brass rim with a short turned handle.", SizeCategory.VerySmall, ItemQuality.Good, 190.0, 38.0m, "glass", ["Era / Pre-Industrial Era", "Functions / Tools / Scientific Tools", "Market / Professional Tools / Standard Tools"], ["Holdable"]),
		new(PreIndustrialItemGroup.NavigationAndScience, "preindustrial_optics_telescope", "telescope", "a brass draw-tube telescope", "Several narrow brass tubes slide within one another between a glass front lens and a small eyepiece.", SizeCategory.Small, ItemQuality.Good, 1250.0, 95.0m, "brass", ["Era / Pre-Industrial Era", "Functions / Tools / Scientific Tools / Navigational Tools / Telescope", "Market / Professional Tools / Standard Tools"], ["Holdable"]),
		new(PreIndustrialItemGroup.NavigationAndScience, "preindustrial_science_balance_scales", "scales", "a set of brass balance scales", "A slender brass beam hangs above two shallow pans on fine chains, supported by a compact folding stand.", SizeCategory.Small, ItemQuality.Good, 1800.0, 55.0m, "brass", ["Era / Pre-Industrial Era", "Functions / Tools / Scientific Tools / Measurement Tools", "Market / Professional Tools / Standard Tools"], ["Holdable"]),
		new(PreIndustrialItemGroup.NavigationAndScience, "preindustrial_science_glass_specimen_jar", "jar", "a clear glass specimen jar", "This thick-walled clear glass jar has a wide mouth, a fitted stopper, and a flat base beneath its cylindrical body.", SizeCategory.Small, ItemQuality.Standard, 620.0, 18.0m, "glass", ["Era / Pre-Industrial Era", "Functions / Container", "Functions / Tools / Scientific Tools / Laboratory Equipment", "Market / Professional Tools / Standard Tools"], ["Holdable", "Container_Pouch", "Destroyable_Glassware"]),

		new(PreIndustrialItemGroup.GunpowderSupport, "preindustrial_firearms_powder_horn", "horn", "a stoppered powder horn", "This curved horn has a fitted wooden plug, a narrow pouring spout, and a leather carrying cord.", SizeCategory.Small, ItemQuality.Standard, 420.0, 18.0m, "bone", ["Era / Pre-Industrial Era", "Functions / Container", "Market / Military Goods / Ammunition / Blackpowder"], ["Holdable", "Container_Pouch"]),
		new(PreIndustrialItemGroup.GunpowderSupport, "preindustrial_firearms_powder_flask", "flask", "a brass powder flask", "This flattened brass flask has a narrow spring-capped spout and a pair of small carrying loops at its shoulders.", SizeCategory.Small, ItemQuality.Good, 650.0, 36.0m, "brass", ["Era / Pre-Industrial Era", "Functions / Container", "Market / Military Goods / Ammunition / Blackpowder"], ["Holdable", "Container_Pouch"]),
		new(PreIndustrialItemGroup.GunpowderSupport, "preindustrial_firearms_priming_flask", "flask", "a small priming flask", "This palm-sized brass flask has a fine pouring neck, a close cap, and a short suspension loop.", SizeCategory.VerySmall, ItemQuality.Good, 260.0, 24.0m, "brass", ["Era / Pre-Industrial Era", "Functions / Container", "Market / Military Goods / Ammunition / Blackpowder"], ["Holdable", "Container_Pouch"]),
		new(PreIndustrialItemGroup.GunpowderSupport, "preindustrial_firearms_shot_pouch", "pouch", "a leather shot pouch", "This stout leather pouch has a rounded bottom, a close-fitting flap, and a broad loop sewn across its back.", SizeCategory.Small, ItemQuality.Standard, 380.0, 14.0m, "leather", ["Era / Pre-Industrial Era", "Functions / Container", "Market / Military Goods / Ammunition / Blackpowder"], ["Holdable", "Container_Pouch"]),
		new(PreIndustrialItemGroup.GunpowderSupport, "preindustrial_firearms_match_cord_bundle", "cord", "a bundle of match cord", "Several lengths of thick hemp cord are wound into a compact bundle and tied through the middle with thinner twine.", SizeCategory.Small, ItemQuality.Standard, 620.0, 8.0m, "hemp", ["Era / Pre-Industrial Era", "Market / Military Goods / Ammunition / Blackpowder"], ["Holdable", "Stack_Number"]),
		new(PreIndustrialItemGroup.GunpowderSupport, "preindustrial_firearms_musket_wadding_packet", "packet", "a packet of musket wadding", "This folded paper packet holds many small, evenly cut discs of coarse fibre and cloth.", SizeCategory.Tiny, ItemQuality.Standard, 90.0, 5.0m, "paper", ["Era / Pre-Industrial Era", "Functions / Material Functions / Musket Wadding", "Market / Military Goods / Ammunition / Blackpowder"], ["Holdable", "Stack_Number"]),
		new(PreIndustrialItemGroup.GunpowderSupport, "preindustrial_firearms_bullet_mould", "mould", "a hinged brass bullet mould", "Two brass jaws meet around a small rounded cavity and close on long, plain wooden handles.", SizeCategory.Small, ItemQuality.Good, 760.0, 42.0m, "brass", ["Era / Pre-Industrial Era", "Functions / Tools", "Market / Military Goods / Ammunition / Blackpowder"], ["Holdable"]),
		new(PreIndustrialItemGroup.GunpowderSupport, "preindustrial_firearms_ramrod", "ramrod", "a straight wooden ramrod", "This long straight oak rod has a rounded grip at one end and a narrow brass cap at the other.", SizeCategory.Normal, ItemQuality.Standard, 360.0, 10.0m, "oak", ["Era / Pre-Industrial Era", "Functions / Tools", "Market / Military Goods / Ammunition / Blackpowder"], ["Holdable"]),
		new(PreIndustrialItemGroup.GunpowderSupport, "preindustrial_firearms_touchhole_pick", "pick", "a fine brass touchhole pick", "This slender brass pick narrows to a fine point and ends in a small finger ring.", SizeCategory.Tiny, ItemQuality.Standard, 28.0, 8.0m, "brass", ["Era / Pre-Industrial Era", "Functions / Tools", "Market / Military Goods / Ammunition / Blackpowder"], ["Holdable"]),
		new(PreIndustrialItemGroup.GunpowderSupport, "preindustrial_firearms_cleaning_rod", "rod", "a brass-tipped cleaning rod", "This narrow oak rod carries a grooved brass tip and a small turned handle at the opposite end.", SizeCategory.Normal, ItemQuality.Standard, 420.0, 12.0m, "oak", ["Era / Pre-Industrial Era", "Functions / Tools", "Market / Military Goods / Ammunition / Blackpowder"], ["Holdable"]),

		new(PreIndustrialItemGroup.GlobalTradePackaging, "preindustrial_trade_tea_chest", "chest", "a lined tea chest", "This stout oak chest has a close lid, reinforced corners, and a pale lining fitted neatly against its inner walls.", SizeCategory.Large, ItemQuality.Good, 11500.0, 52.0m, "oak", ["Era / Pre-Industrial Era", "Functions / Container", "Market / Household Goods / Standard Wares"], ["Holdable", "Destroyable_Furniture", "Container_Archive_Chest"]),
		new(PreIndustrialItemGroup.GlobalTradePackaging, "preindustrial_trade_coffee_sack", "sack", "a coarse coffee sack", "This large linen sack has a gathered mouth, doubled seams, and dark handling marks across its tightly woven body.", SizeCategory.Large, ItemQuality.Standard, 680.0, 12.0m, "linen", ["Era / Pre-Industrial Era", "Functions / Container", "Functions / Container / Porous Container", "Market / Household Goods / Standard Wares"], ["Holdable", "Container_Sack"]),
		new(PreIndustrialItemGroup.GlobalTradePackaging, "preindustrial_trade_cacao_sack", "sack", "a stout cacao sack", "This stout linen sack has a wide gathered mouth, reinforced lower corners, and a plain cord tie.", SizeCategory.Large, ItemQuality.Standard, 720.0, 13.0m, "linen", ["Era / Pre-Industrial Era", "Functions / Container", "Functions / Container / Porous Container", "Market / Household Goods / Standard Wares"], ["Holdable", "Container_Sack"]),
		new(PreIndustrialItemGroup.GlobalTradePackaging, "preindustrial_trade_tobacco_bale", "bale", "a canvas-wrapped tobacco bale", "A compact rectangular bale is wrapped in heavy linen canvas and held by several tight crossing cords.", SizeCategory.Large, ItemQuality.Standard, 1500.0, 18.0m, "linen", ["Era / Pre-Industrial Era", "Functions / Container", "Functions / Container / Porous Container", "Market / Household Goods / Standard Wares"], ["Holdable", "Container_Sack"]),
		new(PreIndustrialItemGroup.GlobalTradePackaging, "preindustrial_trade_sugar_hogshead", "hogshead", "an oak sugar hogshead", "This broad oak hogshead is built from close staves, heavy hoops, and a fitted circular head with a small bung.", SizeCategory.Large, ItemQuality.Good, 26000.0, 88.0m, "oak", ["Era / Pre-Industrial Era", "Functions / Container", "Market / Household Goods / Standard Wares"], ["Holdable", "Destroyable_Furniture", "Container_Drum"]),
		new(PreIndustrialItemGroup.GlobalTradePackaging, "preindustrial_trade_indigo_cake_box", "box", "an indigo cake box", "This close-fitted oak box is divided by thin slats into shallow spaces, its inner corners darkly stained.", SizeCategory.Normal, ItemQuality.Standard, 3600.0, 24.0m, "oak", ["Era / Pre-Industrial Era", "Functions / Container", "Market / Household Goods / Standard Wares"], ["Holdable", "Destroyable_Furniture", "Container_Archive_Box"]),
		new(PreIndustrialItemGroup.GlobalTradePackaging, "preindustrial_trade_porcelain_packing_crate", "crate", "a porcelain packing crate", "This slatted oak crate has square corner posts and a deep open body packed with narrow wooden dividers.", SizeCategory.Large, ItemQuality.Standard, 4400.0, 30.0m, "oak", ["Era / Pre-Industrial Era", "Functions / Container", "Functions / Container / Open Container", "Market / Household Goods / Standard Wares"], ["Holdable", "Destroyable_Furniture", "Container_Open_Bin"]),
		new(PreIndustrialItemGroup.GlobalTradePackaging, "preindustrial_trade_glass_bottle_crate", "crate", "a divided glass-bottle crate", "This open oak crate is divided into a regular grid of narrow compartments, with high slatted sides and reinforced corners.", SizeCategory.Large, ItemQuality.Standard, 4200.0, 28.0m, "oak", ["Era / Pre-Industrial Era", "Functions / Container", "Functions / Container / Open Container", "Market / Household Goods / Standard Wares"], ["Holdable", "Destroyable_Furniture", "Container_Open_Bin"]),
		new(PreIndustrialItemGroup.GlobalTradePackaging, "preindustrial_trade_silk_bale", "bale", "a wrapped silk bale", "A long rectangular bale is wrapped in smooth linen canvas, stitched closed, and bound by flat protective cords.", SizeCategory.Large, ItemQuality.Good, 1200.0, 36.0m, "linen", ["Era / Pre-Industrial Era", "Functions / Container", "Functions / Container / Porous Container", "Market / Household Goods / Standard Wares"], ["Holdable", "Container_Sack"]),
		new(PreIndustrialItemGroup.GlobalTradePackaging, "preindustrial_trade_cotton_bale", "bale", "a canvas-wrapped cotton bale", "This bulky rectangular bale is compressed beneath coarse linen canvas and secured by several tight crossing cords.", SizeCategory.Large, ItemQuality.Standard, 1800.0, 16.0m, "linen", ["Era / Pre-Industrial Era", "Functions / Container", "Functions / Container / Porous Container", "Market / Household Goods / Standard Wares"], ["Holdable", "Container_Sack"])
	];

	private static readonly IReadOnlyDictionary<string, string> PreIndustrialAntiquityAliasStableReferences =
		new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			["antiquity_stone_courtyard_sundial"] = "preindustrial_time_stone_courtyard_sundial",
			["antiquity_bronze_portable_gnomon"] = "preindustrial_time_bronze_portable_gnomon",
			["antiquity_marked_watch_candle"] = "preindustrial_time_marked_watch_candle",
			["antiquity_temple_water_clock"] = "preindustrial_time_water_clock",
			["antiquity_watchman_hour_board"] = "preindustrial_time_watchman_hour_board",
			["antiquity_stone_public_well"] = "preindustrial_water_stone_public_well",
			["antiquity_lined_cistern"] = "preindustrial_water_lined_cistern",
			["antiquity_bronze_fountain_spout"] = "preindustrial_water_bronze_fountain_spout",
			["antiquity_temple_purification_basin"] = "preindustrial_water_purification_basin",
			["antiquity_irrigation_channel_outlet"] = "preindustrial_water_irrigation_channel_outlet"
		};

	internal static IReadOnlyCollection<PreIndustrialItemSpecTestData> PreIndustrialNewItemSpecsForTesting =>
		PreIndustrialNewItemSpecs
			.Select(x => new PreIndustrialItemSpecTestData(
				x.Group.ToString(),
				x.StableReference,
				x.Material,
				x.Tags,
				x.Components,
				x.FixedFixture))
			.ToArray();

	internal static IReadOnlyCollection<PreIndustrialItemSpecTestData> PreIndustrialAntiquityAliasSpecsForTesting =>
		AntiquityComponentGapItemSpecs()
			.Where(x => PreIndustrialAntiquityAliasStableReferences.ContainsKey(x.StableReference))
			.Select(x => new PreIndustrialItemSpecTestData(
				"AntiquityAlias",
				PreIndustrialAntiquityAliasStableReferences[x.StableReference],
				GetPreIndustrialAliasMaterial(x.Material),
				BuildPreIndustrialAliasTags(x.Tags),
				x.Components,
				!x.Components.Contains("Holdable", StringComparer.InvariantCultureIgnoreCase)))
			.ToArray();

	internal static bool ShouldSeedSharedPreIndustrialBaselineForTesting(string? eras)
	{
		return !string.IsNullOrWhiteSpace(eras) &&
		       HasAnyEra(eras, "antiquity", "medieval", "renaissance", "earlymodern");
	}

	private void SeedSharedPreIndustrialBaselineItems()
	{
		SeedHistoricCommonWorkshopItems();
		SeedPrimaryProductionToolsAndProps();
		SeedPreIndustrialWritingAdministrationAndDocuments();
		SeedPreIndustrialTradeContainers();
		SeedPreIndustrialDoorsLocksAndBasicHardware();
		SeedPreIndustrialCommonToolsAndWorkshopFixtures();
		SeedPreIndustrialMilitarySupportGoods();
	}

	private void SeedRenaissanceItems()
	{
		// Era-specific Renaissance item seeders go here.
	}

	private void SeedEarlyModernItems()
	{
		// Era-specific Early Modern item seeders go here.
	}

	private GameItemProto? CreatePreIndustrialAlias(
		string sourceStableReference,
		string stableReference,
		string noun,
		string sdesc,
		string? ldesc,
		string fdesc,
		SizeCategory size,
		ItemQuality quality,
		double weightInGrams,
		decimal inherentCost,
		bool skinnable,
		bool hideFromPlayers,
		string material,
		IEnumerable<string> tags,
		IEnumerable<string> components,
		string? morphToUniqueReference,
		string? morphEmote,
		TimeSpan? morphTimer,
		string? destroyedItemUniqueReference,
		string? builderNotes = null)
	{
		var aliasNote = BuildPreIndustrialAliasBuilderNotesForTesting(sourceStableReference);
		return CreateItem(
			stableReference,
			noun,
			sdesc,
			ldesc,
			fdesc,
			size,
			quality,
			weightInGrams,
			inherentCost,
			skinnable,
			hideFromPlayers,
			material,
			BuildPreIndustrialAliasTags(tags),
			components,
			morphToUniqueReference,
			morphEmote,
			morphTimer,
			destroyedItemUniqueReference,
			MergeBuilderNotes(builderNotes, aliasNote),
			allowLegacyShortDescriptionMatch: false);
	}

	internal static string BuildPreIndustrialAliasBuilderNotesForTesting(string sourceStableReference)
	{
		return $"Shared pre-industrial alias derived from {sourceStableReference}; original {GetSourceEraName(sourceStableReference)} reference retained for compatibility.";
	}

	private static string GetSourceEraName(string sourceStableReference)
	{
		return sourceStableReference.StartsWith("antiquity_", StringComparison.InvariantCultureIgnoreCase)
			? "antiquity"
			: "medieval";
	}

	private static IReadOnlyCollection<string> BuildPreIndustrialAliasTags(IEnumerable<string> tags)
	{
		return tags
			.Where(x => !x.Equals("Era / Antiquity Era", StringComparison.InvariantCultureIgnoreCase) &&
			            !x.Equals("Era / Medieval Era", StringComparison.InvariantCultureIgnoreCase))
			.Select(x => x.Equals("Functions / Tools / Construction Tools / Construction Ruler",
				StringComparison.InvariantCultureIgnoreCase)
				? "Functions / Tools / Layout Tools / Ruler / Construction Ruler"
				: x)
			.Append("Era / Pre-Industrial Era")
			.Distinct(StringComparer.InvariantCultureIgnoreCase)
			.ToArray();
	}

	private void SeedPreIndustrialAntiquityTimeAndWaterAliases()
	{
		EnsureAntiquityComponentGapWritingComponents();
		foreach (var spec in AntiquityComponentGapItemSpecs().Where(x =>
			         PreIndustrialAntiquityAliasStableReferences.ContainsKey(x.StableReference)))
		{
			CreatePreIndustrialAlias(
				spec.StableReference,
				PreIndustrialAntiquityAliasStableReferences[spec.StableReference],
				spec.Noun,
				spec.ShortDescription,
				null,
				spec.FullDescription,
				spec.Size,
				spec.Quality,
				spec.WeightInGrams,
				spec.Cost,
				false,
				false,
				GetPreIndustrialAliasMaterial(spec.Material),
				spec.Tags,
				spec.Components,
				null,
				null,
				null,
				null,
				spec.BuilderNotes);
		}
	}

	private static string GetPreIndustrialAliasMaterial(string sourceMaterial)
	{
		return sourceMaterial.Equals("wax", StringComparison.InvariantCultureIgnoreCase)
			? "beeswax"
			: sourceMaterial;
	}

	private void SeedPreIndustrialPrintingAndPaperAdministrationItems()
	{
		SeedPreIndustrialNewItems(PreIndustrialItemGroup.Printing);
	}

	private void SeedPreIndustrialNavigationOpticsAndMeasurementItems()
	{
		SeedPreIndustrialNewItems(PreIndustrialItemGroup.NavigationAndScience);
	}

	private void SeedPreIndustrialGunpowderSupportItems()
	{
		SeedPreIndustrialNewItems(PreIndustrialItemGroup.GunpowderSupport);
	}

	private void SeedPreIndustrialGlobalTradePackagingItems()
	{
		SeedPreIndustrialNewItems(PreIndustrialItemGroup.GlobalTradePackaging);
	}

	private void SeedPreIndustrialNewItems(PreIndustrialItemGroup group)
	{
		foreach (var spec in PreIndustrialNewItemSpecs.Where(x => x.Group == group))
		{
			CreateItem(
				spec.StableReference,
				spec.Noun,
				spec.ShortDescription,
				null,
				spec.FullDescription,
				spec.Size,
				spec.Quality,
				spec.WeightInGrams,
				spec.Cost,
				false,
				false,
				spec.Material,
				spec.Tags,
				spec.Components,
				null,
				null,
				null,
				null,
				"Shared pre-industrial baseline stock.");
		}
	}
}
