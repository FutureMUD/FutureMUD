#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Historical05Stock
{
	public static CatalogueItemDefinition MedievalDecorStripedWoolCarpet() => new()
	{
		Metadata = new("medieval_decor_striped_wool_carpet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13091", []),
		Noun = "carpet",
		ShortDescription = "a striped wool carpet",
		LongDescription = null,
		FullDescription = "This striped wool carpet is a large, well-made carpet made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 5000.0,
		Cost = 46.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Broad carpet with alternating woven bands.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWillowPlaitedMat() => new()
	{
		Metadata = new("medieval_decor_willow_plaited_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13119", []),
		Noun = "mat",
		ShortDescription = "a willow-plaited floor mat",
		LongDescription = null,
		FullDescription = "This willow-plaited floor mat is a medium-sized, workmanlike mat built from willow boards. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1500.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "willow",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Flexible willow mat with tight plaiting and a plain border.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWindowHanging() => new()
	{
		Metadata = new("medieval_decor_window_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13147", []),
		Noun = "hanging",
		ShortDescription = "a small window hanging",
		LongDescription = null,
		FullDescription = "This small window hanging is a medium-sized, workmanlike hanging made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 800.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Short light hanging for a window, alcove, or wall opening without door behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWinterWallDrape() => new()
	{
		Metadata = new("medieval_decor_winter_wall_drape", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13175", []),
		Noun = "drape",
		ShortDescription = "a heavy winter wall drape",
		LongDescription = null,
		FullDescription = "This heavy winter wall drape is a large, workmanlike drape made from woven wool. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3900.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy wool drape for colder rooms and drafty walls.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWoodenFoldingScreen() => new()
	{
		Metadata = new("medieval_decor_wooden_folding_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13203", []),
		Noun = "screen",
		ShortDescription = "a wooden folding screen",
		LongDescription = null,
		FullDescription = "This wooden folding screen is a large, workmanlike screen built from pine boards. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6200.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Hinged wooden screen for decorative room division; it has no door or cover component.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWoolBenchCover() => new()
	{
		Metadata = new("medieval_decor_wool_bench_cover", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13231", []),
		Noun = "cover",
		ShortDescription = "a wool bench cover",
		LongDescription = null,
		FullDescription = "This wool bench cover is a medium-sized, workmanlike cover made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1100.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long wool cover for a bench or settle; it does not provide seating behaviour by itself.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWoolStairMat() => new()
	{
		Metadata = new("medieval_decor_wool_stair_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13259", []),
		Noun = "mat",
		ShortDescription = "a narrow stair mat",
		LongDescription = null,
		FullDescription = "This narrow stair mat is a medium-sized, workmanlike mat made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1300.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Narrow textile mat suited to steps, thresholds, and raised platforms without acting as a door.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWovenBedCoverlet() => new()
	{
		Metadata = new("medieval_decor_woven_bed_coverlet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13287", []),
		Noun = "coverlet",
		ShortDescription = "a woven bed coverlet",
		LongDescription = null,
		FullDescription = "This woven bed coverlet is a large, workmanlike coverlet made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2400.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorative coverlet for a bed, pallet, or sleeping platform.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWovenFloorCarpet() => new()
	{
		Metadata = new("medieval_decor_woven_floor_carpet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13315", []),
		Noun = "carpet",
		ShortDescription = "a woven wool floor carpet",
		LongDescription = null,
		FullDescription = "This woven wool floor carpet is a large, workmanlike carpet made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4800.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "General large floor textile for halls, chambers, and better household rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWovenTapestryStrip() => new()
	{
		Metadata = new("medieval_decor_woven_tapestry_strip", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13343", []),
		Noun = "tapestry",
		ShortDescription = "a woven tapestry strip",
		LongDescription = null,
		FullDescription = "This woven tapestry strip is a large, well-made tapestry made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 2300.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long narrow tapestry strip for a wall, canopy edge, or high chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorNorthernAntlerWallPiece() => new()
	{
		Metadata = new("medieval_decor_northern_antler_wall_piece", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13371", []),
		Noun = "plaque",
		ShortDescription = "an antler wall piece",
		LongDescription = null,
		FullDescription = "This antler wall piece is a small, workmanlike plaque worked from antler. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The pale surface is smoothed at the high points and darker in the cut recesses.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Standard,
		WeightInGrams = 900.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "antler",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorative antler piece mounted or tied for household display.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorNorthernBeastCarvedPanel() => new()
	{
		Metadata = new("medieval_decor_northern_beast_carved_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13399", []),
		Noun = "panel",
		ShortDescription = "a beast-carved wall panel",
		LongDescription = null,
		FullDescription = "This beast-carved wall panel is a medium-sized, well-made panel built from oak boards. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 4200.0,
		Cost = 50.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Oak panel carved with interlaced animal forms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorNorthernCanvasRoomHanging() => new()
	{
		Metadata = new("medieval_decor_northern_canvas_room_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13427", []),
		Noun = "hanging",
		ShortDescription = "a heavy canvas room hanging",
		LongDescription = null,
		FullDescription = "This heavy canvas room hanging is a large, workmanlike hanging made from coarse canvas. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2500.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "canvas",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Sturdy hanging suited to a workshop, shipboard space, or plain household room.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorNorthernCheckedBedCover() => new()
	{
		Metadata = new("medieval_decor_northern_checked_bed_cover", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13455", []),
		Noun = "cover",
		ShortDescription = "a checked wool bed cover",
		LongDescription = null,
		FullDescription = "This checked wool bed cover is a large, workmanlike cover made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2600.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Checked wool cover suited to colder sleeping rooms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorNorthernFeltWinterMat() => new()
	{
		Metadata = new("medieval_decor_northern_felt_winter_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13483", []),
		Noun = "mat",
		ShortDescription = "a thick winter felt mat",
		LongDescription = null,
		FullDescription = "This thick winter felt mat is a medium-sized, workmanlike mat made from pressed felt. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
		Cost = 13.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Dense felt mat for a cold floor or raised sleeping platform.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorNorthernFurWallHanging() => new()
	{
		Metadata = new("medieval_decor_northern_fur_wall_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13511", []),
		Noun = "hanging",
		ShortDescription = "a fur-backed wall hanging",
		LongDescription = null,
		FullDescription = "This fur-backed wall hanging is a large, well-made hanging made from dressed fur. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 3800.0,
		Cost = 76.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fur",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Thick wall hanging with fur backing for a winter chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorNorthernHideBenchCover() => new()
	{
		Metadata = new("medieval_decor_northern_hide_bench_cover", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13539", []),
		Noun = "cover",
		ShortDescription = "a hide bench cover",
		LongDescription = null,
		FullDescription = "This hide bench cover is a medium-sized, workmanlike cover made from cured animal skin. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1300.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "animal skin",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain tanned hide cover for a bench, chest, or sleeping ledge.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorNorthernTabletWallHanging() => new()
	{
		Metadata = new("medieval_decor_northern_tablet_wall_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13567", []),
		Noun = "hanging",
		ShortDescription = "a tablet-banded wall hanging",
		LongDescription = null,
		FullDescription = "This tablet-banded wall hanging is a large, well-made hanging made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 2400.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy wool hanging with tablet-woven bands along the edges.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWesternEmbroideredBedValance() => new()
	{
		Metadata = new("medieval_decor_western_embroidered_bed_valance", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13595", []),
		Noun = "valance",
		ShortDescription = "an embroidered bed valance",
		LongDescription = null,
		FullDescription = "This embroidered bed valance is a medium-sized, well-made valance made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 720.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Worked textile valance for a bed, curtain rail, or canopy edge.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWesternFloweredTapestry() => new()
	{
		Metadata = new("medieval_decor_western_flowered_tapestry", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13623", []),
		Noun = "tapestry",
		ShortDescription = "a flowered wall tapestry",
		LongDescription = null,
		FullDescription = "This flowered wall tapestry is a large, well-made tapestry made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 3300.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine wall tapestry with small repeated floral forms.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWesternHouseholdBanner() => new()
	{
		Metadata = new("medieval_decor_western_household_banner", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13651", []),
		Noun = "banner",
		ShortDescription = "a painted household banner",
		LongDescription = null,
		FullDescription = "This painted household banner is a medium-sized, well-made banner made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Painted banner for a chamber, hall, guild room, or noble household skin.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWesternLinenPaintedCloth() => new()
	{
		Metadata = new("medieval_decor_western_linen_painted_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13679", []),
		Noun = "cloth",
		ShortDescription = "a vine-painted linen cloth",
		LongDescription = null,
		FullDescription = "This vine-painted linen cloth is a large, well-made cloth made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 1300.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Painted linen cloth with curling vine and border decoration.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWesternOakTraceryScreen() => new()
	{
		Metadata = new("medieval_decor_western_oak_tracery_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13707", []),
		Noun = "screen",
		ShortDescription = "an oak tracery screen",
		LongDescription = null,
		FullDescription = "This oak tracery screen is a large, well-made screen built from oak boards. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 7800.0,
		Cost = 88.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "oak",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Open carved oak screen with repeated arched tracery.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWesternStainedGlassPanel() => new()
	{
		Metadata = new("medieval_decor_western_stained_glass_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13735", []),
		Noun = "panel",
		ShortDescription = "a stained glass panel",
		LongDescription = null,
		FullDescription = "This stained glass panel is a medium-sized, well-made panel made from glass. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The surface catches light along the rim, base, and any raised edges.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3400.0,
		Cost = 110.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "glass",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Decorative coloured-glass panel in a portable frame.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWesternWalnutCarvedPanel() => new()
	{
		Metadata = new("medieval_decor_western_walnut_carved_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13763", []),
		Noun = "panel",
		ShortDescription = "a carved walnut panel",
		LongDescription = null,
		FullDescription = "This carved walnut panel is a medium-sized, well-made panel built from walnut boards. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "walnut",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Walnut panel with crisp carving and a polished surface.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorWesternWoolDaisCarpet() => new()
	{
		Metadata = new("medieval_decor_western_wool_dais_carpet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13791", []),
		Noun = "carpet",
		ShortDescription = "a wool dais carpet",
		LongDescription = null,
		FullDescription = "This wool dais carpet is a large, well-made carpet made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 4200.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Good wool carpet sized for a raised table, seat, or chamber platform.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorMediterraneanBrightFloorCarpet() => new()
	{
		Metadata = new("medieval_decor_mediterranean_bright_floor_carpet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13819", []),
		Noun = "carpet",
		ShortDescription = "a bright wool floor carpet",
		LongDescription = null,
		FullDescription = "This bright wool floor carpet is a large, well-made carpet made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 4700.0,
		Cost = 54.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Brightly woven carpet suited to a warm household chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorMediterraneanBronzeRoundMirror() => new()
	{
		Metadata = new("medieval_decor_mediterranean_bronze_round_mirror", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13847", []),
		Noun = "mirror",
		ShortDescription = "a round bronze mirror",
		LongDescription = null,
		FullDescription = "This round bronze mirror is a small, well-made mirror worked from bronze. A polished reflective face sits inside a raised frame, with a small loop set at the back. The edge of the frame is rubbed where it has been handled. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 850.0,
		Cost = 50.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Round polished bronze mirror with a simple rim.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorMediterraneanLinenCourtyardCurtain() => new()
	{
		Metadata = new("medieval_decor_mediterranean_linen_courtyard_curtain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13875", []),
		Noun = "curtain",
		ShortDescription = "a linen courtyard curtain",
		LongDescription = null,
		FullDescription = "This linen courtyard curtain is a large, workmanlike curtain made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1300.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "linen",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light curtain for an indoor wall, courtyard shade, or windowed room.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorMediterraneanMarbleReliefPlaque() => new()
	{
		Metadata = new("medieval_decor_mediterranean_marble_relief_plaque", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13903", []),
		Noun = "plaque",
		ShortDescription = "a marble relief plaque",
		LongDescription = null,
		FullDescription = "This marble relief plaque is a small, well-made plaque cut from marble. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 2800.0,
		Cost = 90.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "marble",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small marble plaque carved in shallow relief.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorMediterraneanMosaicTilePanel() => new()
	{
		Metadata = new("medieval_decor_mediterranean_mosaic_tile_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13931", []),
		Noun = "panel",
		ShortDescription = "a mosaic tile panel",
		LongDescription = null,
		FullDescription = "This mosaic tile panel is a medium-sized, well-made panel formed from ceramic. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 6200.0,
		Cost = 70.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small mosaic panel made from fitted coloured ceramic pieces.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorMediterraneanOlivewoodScreen() => new()
	{
		Metadata = new("medieval_decor_mediterranean_olivewood_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13959", []),
		Noun = "screen",
		ShortDescription = "an olivewood privacy screen",
		LongDescription = null,
		FullDescription = "This olivewood privacy screen is a large, well-made screen built from olive wood. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 6900.0,
		Cost = 76.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "olive wood",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Polished olivewood screen with warm grain and light carving.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorMediterraneanPaintedPlasterPanel() => new()
	{
		Metadata = new("medieval_decor_mediterranean_painted_plaster_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:13987", []),
		Noun = "panel",
		ShortDescription = "a painted plaster panel",
		LongDescription = null,
		FullDescription = "This painted plaster panel is a medium-sized, well-made panel moulded from plaster. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 3400.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "plaster",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Painted plaster panel with warm-toned border decoration.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorMediterraneanTerracottaWallTiles() => new()
	{
		Metadata = new("medieval_decor_mediterranean_terracotta_wall_tiles", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14015", []),
		Noun = "tile",
		ShortDescription = "a set of terracotta wall tiles",
		LongDescription = null,
		FullDescription = "This set of terracotta wall tiles is a medium-sized, workmanlike tile formed from terracotta. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5400.0,
		Cost = 26.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "terracotta",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Matched terracotta tiles for decorative household wall display.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorIslamicateBrassWallMirror() => new()
	{
		Metadata = new("medieval_decor_islamicate_brass_wall_mirror", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14043", []),
		Noun = "mirror",
		ShortDescription = "a polished brass wall mirror",
		LongDescription = null,
		FullDescription = "This polished brass wall mirror is a small, well-made mirror worked from brass. A polished reflective face sits inside a raised frame, with a small loop set at the back. The edge of the frame is rubbed where it has been handled. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 1000.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Polished brass mirror plate with a decorated rim.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorIslamicateCedarLatticeScreen() => new()
	{
		Metadata = new("medieval_decor_islamicate_cedar_lattice_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14071", []),
		Noun = "screen",
		ShortDescription = "a cedar lattice screen",
		LongDescription = null,
		FullDescription = "This cedar lattice screen is a large, well-made screen built from cedar boards. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 6400.0,
		Cost = 84.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cedar",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Carved cedar screen with repeated lattice openings.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorIslamicateCottonCanopyCloth() => new()
	{
		Metadata = new("medieval_decor_islamicate_cotton_canopy_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14099", []),
		Noun = "cloth",
		ShortDescription = "a cotton canopy cloth",
		LongDescription = null,
		FullDescription = "This cotton canopy cloth is a large, well-made cloth made from woven cotton. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 1200.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light canopy cloth for a bed, low platform, or shaded chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorIslamicateCottonWindowCurtain() => new()
	{
		Metadata = new("medieval_decor_islamicate_cotton_window_curtain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14127", []),
		Noun = "curtain",
		ShortDescription = "a cotton window curtain",
		LongDescription = null,
		FullDescription = "This cotton window curtain is a medium-sized, workmanlike curtain made from woven cotton. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 850.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light cotton curtain for an inner window or alcove.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorIslamicateFlatwovenFloorRug() => new()
	{
		Metadata = new("medieval_decor_islamicate_flatwoven_floor_rug", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14155", []),
		Noun = "rug",
		ShortDescription = "a flatwoven wool floor rug",
		LongDescription = null,
		FullDescription = "This flatwoven wool floor rug is a medium-sized, well-made rug made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 2100.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Flatwoven rug with narrow bands and crisp edge work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorIslamicateGeometricCarpet() => new()
	{
		Metadata = new("medieval_decor_islamicate_geometric_carpet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14183", []),
		Noun = "carpet",
		ShortDescription = "a geometric wool carpet",
		LongDescription = null,
		FullDescription = "This geometric wool carpet is a large, well-made carpet made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 5200.0,
		Cost = 68.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Wool carpet with repeated geometric field and border designs.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorIslamicateGlazedTilePanel() => new()
	{
		Metadata = new("medieval_decor_islamicate_glazed_tile_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14211", []),
		Noun = "panel",
		ShortDescription = "a glazed ceramic tile panel",
		LongDescription = null,
		FullDescription = "This glazed ceramic tile panel is a medium-sized, well-made panel formed from ceramic. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 5600.0,
		Cost = 74.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ceramic",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Glossy tile panel with fitted decorative shapes.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorIslamicatePiercedBrassScreen() => new()
	{
		Metadata = new("medieval_decor_islamicate_pierced_brass_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14239", []),
		Noun = "screen",
		ShortDescription = "a pierced brass screen",
		LongDescription = null,
		FullDescription = "This pierced brass screen is a large, well-made screen worked from brass. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 9000.0,
		Cost = 130.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Heavy brass screen with pierced decorative work.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorIslamicateSilkRoomDivider() => new()
	{
		Metadata = new("medieval_decor_islamicate_silk_room_divider", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14267", []),
		Noun = "divider",
		ShortDescription = "a silk room divider",
		LongDescription = null,
		FullDescription = "This silk room divider is a large, well-made divider made from woven silk. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 1400.0,
		Cost = 98.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine silk divider hanging for a chamber, alcove, or reception room.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorIslamicateStarWallHanging() => new()
	{
		Metadata = new("medieval_decor_islamicate_star_wall_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14295", []),
		Noun = "hanging",
		ShortDescription = "a star-patterned wall hanging",
		LongDescription = null,
		FullDescription = "This star-patterned wall hanging is a large, well-made hanging made from woven cotton. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 1700.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Cotton wall hanging with star and banded geometric decoration.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSouthAsianBambooWindowBlind() => new()
	{
		Metadata = new("medieval_decor_south_asian_bamboo_window_blind", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14323", []),
		Noun = "blind",
		ShortDescription = "a bamboo window blind",
		LongDescription = null,
		FullDescription = "This bamboo window blind is a medium-sized, workmanlike blind built from split bamboo. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1200.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light bamboo blind for a window, veranda, or room opening without door behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSouthAsianBlockPrintedFloorCloth() => new()
	{
		Metadata = new("medieval_decor_south_asian_block_printed_floor_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14351", []),
		Noun = "cloth",
		ShortDescription = "a block-printed floor cloth",
		LongDescription = null,
		FullDescription = "This block-printed floor cloth is a large, well-made cloth made from woven cotton. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 1400.0,
		Cost = 34.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Cotton floor cloth with repeated block-printed designs.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSouthAsianBronzeMirror() => new()
	{
		Metadata = new("medieval_decor_south_asian_bronze_mirror", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14379", []),
		Noun = "mirror",
		ShortDescription = "a bronze household mirror",
		LongDescription = null,
		FullDescription = "This bronze household mirror is a small, well-made mirror worked from bronze. A polished reflective face sits inside a raised frame, with a small loop set at the back. The edge of the frame is rubbed where it has been handled. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 820.0,
		Cost = 48.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bronze",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Polished bronze mirror with a simple backing loop.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSouthAsianCottonBedValance() => new()
	{
		Metadata = new("medieval_decor_south_asian_cotton_bed_valance", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14407", []),
		Noun = "valance",
		ShortDescription = "a cotton bed valance",
		LongDescription = null,
		FullDescription = "This cotton bed valance is a medium-sized, workmanlike valance made from woven cotton. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 650.0,
		Cost = 14.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Cotton valance for dressing a bed frame, platform, or low sleeping space.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSouthAsianPaintedWallCloth() => new()
	{
		Metadata = new("medieval_decor_south_asian_painted_wall_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14435", []),
		Noun = "cloth",
		ShortDescription = "a painted cotton wall cloth",
		LongDescription = null,
		FullDescription = "This painted cotton wall cloth is a large, well-made cloth made from woven cotton. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 1300.0,
		Cost = 36.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cotton",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Painted cotton cloth for a wall, alcove, or household chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSouthAsianSandalwoodPanel() => new()
	{
		Metadata = new("medieval_decor_south_asian_sandalwood_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14463", []),
		Noun = "panel",
		ShortDescription = "a sandalwood wall panel",
		LongDescription = null,
		FullDescription = "This sandalwood wall panel is a small, well-made panel built from sandalwood. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 950.0,
		Cost = 82.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "sandalwood",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Small fragrant sandalwood panel carved for household display.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSouthAsianSilkCanopyCloth() => new()
	{
		Metadata = new("medieval_decor_south_asian_silk_canopy_cloth", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14491", []),
		Noun = "cloth",
		ShortDescription = "a silk canopy cloth",
		LongDescription = null,
		FullDescription = "This silk canopy cloth is a large, well-made cloth made from woven silk. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 96.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fine silk canopy cloth for a bed, platform, or elite chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSouthAsianTeakCarvedScreen() => new()
	{
		Metadata = new("medieval_decor_south_asian_teak_carved_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14519", []),
		Noun = "screen",
		ShortDescription = "a carved teak screen",
		LongDescription = null,
		FullDescription = "This carved teak screen is a large, well-made screen built from teak boards. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 7600.0,
		Cost = 92.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "teak",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Teak screen with carved pierced panels.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorEastAsianBambooFloorMat() => new()
	{
		Metadata = new("medieval_decor_east_asian_bamboo_floor_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14547", []),
		Noun = "mat",
		ShortDescription = "a woven bamboo floor mat",
		LongDescription = null,
		FullDescription = "This woven bamboo floor mat is a medium-sized, workmanlike mat built from split bamboo. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1300.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Firm woven bamboo mat for a low room, entry, or platform.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorEastAsianBambooWindowScreen() => new()
	{
		Metadata = new("medieval_decor_east_asian_bamboo_window_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14575", []),
		Noun = "screen",
		ShortDescription = "a bamboo window screen",
		LongDescription = null,
		FullDescription = "This bamboo window screen is a medium-sized, workmanlike screen built from split bamboo. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1600.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bamboo",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Bamboo screen for filtering light in a window or room edge.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorEastAsianCypressFrameScreen() => new()
	{
		Metadata = new("medieval_decor_east_asian_cypress_frame_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14603", []),
		Noun = "screen",
		ShortDescription = "a cypress frame screen",
		LongDescription = null,
		FullDescription = "This cypress frame screen is a large, well-made screen built from cypress boards. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 4200.0,
		Cost = 64.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cypress",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Cypress-framed screen with light panels and careful joinery.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorEastAsianPaintedPaperPanel() => new()
	{
		Metadata = new("medieval_decor_east_asian_painted_paper_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14631", []),
		Noun = "panel",
		ShortDescription = "a painted paper wall panel",
		LongDescription = null,
		FullDescription = "This painted paper wall panel is a medium-sized, well-made panel made from layered paper. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 300.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Painted paper panel meant for display on a wall or screen frame.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorEastAsianPaperFoldingScreen() => new()
	{
		Metadata = new("medieval_decor_east_asian_paper_folding_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14659", []),
		Noun = "screen",
		ShortDescription = "a paper folding screen",
		LongDescription = null,
		FullDescription = "This paper folding screen is a large, well-made screen made from layered paper. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 2800.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light folding screen with paper panels in a wooden frame.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorEastAsianPaperRoomDivider() => new()
	{
		Metadata = new("medieval_decor_east_asian_paper_room_divider", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14687", []),
		Noun = "divider",
		ShortDescription = "a paper room divider",
		LongDescription = null,
		FullDescription = "This paper room divider is a large, workmanlike divider made from layered paper. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1800.0,
		Cost = 30.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "paper",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Paper"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light paper-panel divider for organising a room; it has no door behaviour.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorEastAsianPineDisplayPanel() => new()
	{
		Metadata = new("medieval_decor_east_asian_pine_display_panel", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14715", []),
		Noun = "panel",
		ShortDescription = "a pine display panel",
		LongDescription = null,
		FullDescription = "This pine display panel is a medium-sized, workmanlike panel built from pine boards. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2300.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "pine",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Furniture"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Plain display panel with a simple timber frame.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorEastAsianPorcelainScreenTile() => new()
	{
		Metadata = new("medieval_decor_east_asian_porcelain_screen_tile", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14743", []),
		Noun = "tile",
		ShortDescription = "a porcelain screen tile",
		LongDescription = null,
		FullDescription = "This porcelain screen tile is a small, well-made tile formed from porcelain. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
		Size = SizeCategory.Small,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 38.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "porcelain",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Smooth porcelain tile with painted decoration for a screen, wall, or cabinet face.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorEastAsianSilkBedCurtain() => new()
	{
		Metadata = new("medieval_decor_east_asian_silk_bed_curtain", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14771", []),
		Noun = "curtain",
		ShortDescription = "a silk bed curtain",
		LongDescription = null,
		FullDescription = "This silk bed curtain is a large, well-made curtain made from woven silk. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 900.0,
		Cost = 86.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Light silk curtain for a sleeping alcove or elite chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorEastAsianSilkScrollHanging() => new()
	{
		Metadata = new("medieval_decor_east_asian_silk_scroll_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14799", []),
		Noun = "hanging",
		ShortDescription = "a silk scroll hanging",
		LongDescription = null,
		FullDescription = "This silk scroll hanging is a medium-sized, well-made hanging made from woven silk. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Good,
		WeightInGrams = 450.0,
		Cost = 58.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "silk",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Vertical silk hanging mounted on a light roller; it carries decorative rather than readable text.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSteppeEmbroideredFeltScreen() => new()
	{
		Metadata = new("medieval_decor_steppe_embroidered_felt_screen", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14827", []),
		Noun = "screen",
		ShortDescription = "an embroidered felt screen",
		LongDescription = null,
		FullDescription = "This embroidered felt screen is a large, well-made screen made from pressed felt. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 3600.0,
		Cost = 52.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Soft felt screen with stitched decoration for a tent or household chamber.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSteppeFeltFloorCarpet() => new()
	{
		Metadata = new("medieval_decor_steppe_felt_floor_carpet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14855", []),
		Noun = "carpet",
		ShortDescription = "a felt floor carpet",
		LongDescription = null,
		FullDescription = "This felt floor carpet is a large, workmanlike carpet made from pressed felt. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4300.0,
		Cost = 32.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Thick felt carpet suited to tents, wagons, and portable households.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSteppeFeltWallHanging() => new()
	{
		Metadata = new("medieval_decor_steppe_felt_wall_hanging", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14883", []),
		Noun = "hanging",
		ShortDescription = "a felt wall hanging",
		LongDescription = null,
		FullDescription = "This felt wall hanging is a large, workmanlike hanging made from pressed felt. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2500.0,
		Cost = 24.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Felt wall hanging for warmth and decoration in a portable dwelling.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSteppeFurBeddingThrow() => new()
	{
		Metadata = new("medieval_decor_steppe_fur_bedding_throw", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14911", []),
		Noun = "throw",
		ShortDescription = "a fur bedding throw",
		LongDescription = null,
		FullDescription = "This fur bedding throw is a large, well-made throw made from dressed fur. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Good,
		WeightInGrams = 3100.0,
		Cost = 60.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fur",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Warm fur throw for a bedroll, sleeping platform, or display seat.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSteppeHideFloorMat() => new()
	{
		Metadata = new("medieval_decor_steppe_hide_floor_mat", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14939", []),
		Noun = "mat",
		ShortDescription = "a hide floor mat",
		LongDescription = null,
		FullDescription = "This hide floor mat is a medium-sized, workmanlike mat made from cured animal skin. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Normal,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1700.0,
		Cost = 12.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "animal skin",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Practical hide mat for a tent floor, wagon bed, or camp shelter.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSteppeLeatherTentLiner() => new()
	{
		Metadata = new("medieval_decor_steppe_leather_tent_liner", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14967", []),
		Noun = "liner",
		ShortDescription = "a leather tent liner",
		LongDescription = null,
		FullDescription = "This leather tent liner is a large, workmanlike liner made from worked leather. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 5200.0,
		Cost = 42.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Leather liner panel for a tent or travel shelter; it is decorative and protective, not a door.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSteppeTravelRoomDivider() => new()
	{
		Metadata = new("medieval_decor_steppe_travel_room_divider", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:14995", []),
		Noun = "divider",
		ShortDescription = "a travel room divider",
		LongDescription = null,
		FullDescription = "This travel room divider is a large, workmanlike divider made from pressed felt. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2800.0,
		Cost = 28.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "felt",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Folded felt divider for temporary rooms and travel households.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalDecorSteppeWoolTentBand() => new()
	{
		Metadata = new("medieval_decor_steppe_wool_tent_band", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalFurniture.cs:15023", []),
		Noun = "band",
		ShortDescription = "a woven wool tent band",
		LongDescription = null,
		FullDescription = "This woven wool tent band is a large, workmanlike band made from woven wool. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 1200.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
		Components = [
				"Holdable",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Long decorative band for a tent wall, storage rail, or portable screen.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryCommonTwistedCopperRing() => new()
	{
		Metadata = new("medieval_jewellery_common_twisted_copper_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:11", []),
		Noun = "ring",
		ShortDescription = "a twisted copper ring",
		LongDescription = null,
		FullDescription = "A twisted copper ring is a compact band of warm reddish copper darkened in the recesses, made with a soft twist running around the outside rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. The worked parts of the twisted copper ring are decorative, but none suggest a moving catch or concealed compartment. The twisted copper ring reads as commoner adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 8.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "copper",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryCommonPlainBoneRing() => new()
	{
		Metadata = new("medieval_jewellery_common_plain_bone_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:41", []),
		Noun = "ring",
		ShortDescription = "a plain bone ring",
		LongDescription = null,
		FullDescription = "The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Pale bone burnished by handling forms a steady circle, broken only by a shallow file mark near one edge. The worked parts of the plain bone ring are decorative, but none suggest a moving catch or concealed compartment. The piece would signal simple household taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 6.0,
		Cost = 4.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "bone",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Simple Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryCommonHornBeadBracelet() => new()
	{
		Metadata = new("medieval_jewellery_common_horn_bead_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:71", []),
		Noun = "bracelet",
		ShortDescription = "a horn bead bracelet",
		LongDescription = null,
		FullDescription = "A horn bead bracelet is made to circle a limb, with horn with layered translucent streaks through it providing the visible body of the ornament. The edges are kept low and rounded, and beads of slightly different shade gives the piece a recognisable worked surface. Small handling marks cross the horn bead bracelet unevenly, especially where it would brush cloth or skin. The horn bead bracelet carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 34.0,
		Cost = 8.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "horn",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryCommonWoodenBeadNecklace() => new()
	{
		Metadata = new("medieval_jewellery_common_wooden_bead_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:101", []),
		Noun = "necklace",
		ShortDescription = "a wooden bead necklace",
		LongDescription = null,
		FullDescription = "A wooden bead necklace is arranged for the throat or upper chest, where its line of wood would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. The worked parts of the wooden bead necklace are decorative, but none suggest a moving catch or concealed compartment. The wooden bead necklace feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 42.0,
		Cost = 6.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wood",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Simple Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryCommonShellChipBracelet() => new()
	{
		Metadata = new("medieval_jewellery_common_shell_chip_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:131", []),
		Noun = "bracelet",
		ShortDescription = "a shell chip bracelet",
		LongDescription = null,
		FullDescription = "A shell chip bracelet is made to circle a limb, with pale shell showing irregular growth lines providing the visible body of the ornament. The edges are kept low and rounded, and flat chips drilled close to the edge gives the piece a recognisable worked surface. Close inspection of the shell chip bracelet shows small tool marks rather than cast-perfect smoothness. The shell chip bracelet is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 26.0,
		Cost = 7.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "shell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryCommonCowrieShellAnklet() => new()
	{
		Metadata = new("medieval_jewellery_common_cowrie_shell_anklet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:161", []),
		Noun = "anklet",
		ShortDescription = "a cowrie shell anklet",
		LongDescription = null,
		FullDescription = "This limb ornament is practical in scale even when it is decorative in intent. Small cowrie shells with smooth backs and pale mouths shows most clearly on the outer face, while the side that rests against the wearer is quieter. Fine wear gathers around the anklet's highest places, leaving smooth shells strung mouth-outward easier to pick out. The cowrie shell anklet reads as commoner adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 9.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "cowrie shell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Anklet",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryCommonBraidedHempBracelet() => new()
	{
		Metadata = new("medieval_jewellery_common_braided_hemp_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:191", []),
		Noun = "bracelet",
		ShortDescription = "a braided hemp bracelet",
		LongDescription = null,
		FullDescription = "A braided hemp bracelet is made to circle a limb, with coarse hemp cord with small stray fibres providing the visible body of the ornament. The edges are kept low and rounded, and three strands braided with a knotted end gives the piece a recognisable worked surface. Small handling marks cross the braided hemp bracelet unevenly, especially where it would brush cloth or skin. The braided hemp bracelet leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 12.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "hemp",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Simple Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryCommonKnottedWoolWristband() => new()
	{
		Metadata = new("medieval_jewellery_common_knotted_wool_wristband", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:221", []),
		Noun = "bracelet",
		ShortDescription = "a knotted wool wristband",
		LongDescription = null,
		FullDescription = "A knotted wool wristband would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Close inspection of the knotted wool wristband shows small tool marks rather than cast-perfect smoothness. The knotted wool wristband carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 16.0,
		Cost = 2.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wool",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Simple Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryCommonSmallWoodenChildRing() => new()
	{
		Metadata = new("medieval_jewellery_common_small_wooden_child_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:251", []),
		Noun = "ring",
		ShortDescription = "a small wooden child ring",
		LongDescription = null,
		FullDescription = "At close range, small wooden child ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. A rounded edge and very small opening draws the eye around the band. Small handling marks cross the small wooden child ring unevenly, especially where it would brush cloth or skin. The small wooden child ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 4.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "boxwood",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Children's Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryCommonClayColouredFaienceBeadString() => new()
	{
		Metadata = new("medieval_jewellery_common_clay_coloured_faience_bead_string", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:281", []),
		Noun = "necklace",
		ShortDescription = "a clay-coloured faience bead string",
		LongDescription = null,
		FullDescription = "A clay-coloured faience bead string carries its value in the rhythm of its material: glazed faience with small pooling marks in the colour. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Fine wear gathers around the necklace's highest places, leaving beads with glaze gathered in small dimples easier to pick out. The clay-coloured faience bead string carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 55.0,
		Cost = 16.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "faience",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Glassware"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryCommonLittleShellChildNecklace() => new()
	{
		Metadata = new("medieval_jewellery_common_little_shell_child_necklace", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:311", []),
		Noun = "necklace",
		ShortDescription = "a little shell child necklace",
		LongDescription = null,
		FullDescription = "The little shell child necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is pale shell showing irregular growth lines, with small shells spaced on plain cord giving the eye a place to pause. The hidden side of the little shell child necklace is plainer, so attention stays on small shells spaced on plain cord and the visible finish. The piece would signal child or apprentice taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 28.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "shell",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Children's Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryCommonRushWovenFingerRing() => new()
	{
		Metadata = new("medieval_jewellery_common_rush_woven_finger_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:341", []),
		Noun = "ring",
		ShortDescription = "a rush-woven finger ring",
		LongDescription = null,
		FullDescription = "A rush-woven finger ring is a compact band of rush stems bent and plaited into springy loops, made with a woven overlap tucked beneath the band rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Small handling marks cross the rush-woven finger ring unevenly, especially where it would brush cloth or skin. The piece would signal temporary festival taste while remaining straightforward jewellery.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 3.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "rush",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryCommonStrawFestivalRing() => new()
	{
		Metadata = new("medieval_jewellery_common_straw_festival_ring", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:371", []),
		Noun = "ring",
		ShortDescription = "a straw festival ring",
		LongDescription = null,
		FullDescription = "A straw festival ring is a compact band of straw twisted into a pale, dry braid, made with dry stems twisted into a rough circle rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Fine wear gathers around the ring's highest places, leaving dry stems twisted into a rough circle easier to pick out. The straw festival ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 2.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "straw",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Ring",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryCommonLeatherTokenBracelet() => new()
	{
		Metadata = new("medieval_jewellery_common_leather_token_bracelet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:401", []),
		Noun = "bracelet",
		ShortDescription = "a leather token bracelet",
		LongDescription = null,
		FullDescription = "The form is simple enough to survive frequent wearing, but not so plain that it disappears. A single punched hole near the knot breaks up the line of worked leather with creases at the folds. The hidden side of the leather token bracelet is plainer, so attention stays on a single punched hole near the knot and the visible finish. The piece would signal commoner taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 18.0,
		Cost = 5.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "leather",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryCommonJetBeadKeepsakeString() => new()
	{
		Metadata = new("medieval_jewellery_common_jet_bead_keepsake_string", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:431", []),
		Noun = "necklace",
		ShortDescription = "a jet bead keepsake string",
		LongDescription = null,
		FullDescription = "A jet bead keepsake string carries its value in the rhythm of its material: black jet polished until it gives back a quiet reflection. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Fine wear gathers around the necklace's highest places, leaving black beads broken by one pale spacer easier to pick out. The jet bead keepsake string feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 44.0,
		Cost = 22.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jet",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Standard Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryCommonSmallAmberBeadCharm() => new()
	{
		Metadata = new("medieval_jewellery_common_small_amber_bead_charm", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:461", []),
		Noun = "pendant",
		ShortDescription = "a small amber bead charm",
		LongDescription = null,
		FullDescription = "A small amber bead charm carries its value in the rhythm of its material: honey-coloured amber with cloudy threads caught inside. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Close inspection of the small amber bead charm shows small tool marks rather than cast-perfect smoothness. The small amber bead charm reads as commoner adornment rather than a tool, charm, or container.",
		Size = SizeCategory.Tiny,
		Quality = ItemQuality.Standard,
		WeightInGrams = 10.0,
		Cost = 18.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "amber",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Commoner Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = null,
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandDriedFlowerNeckGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_dried_flower_neck_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:491", []),
		Noun = "garland",
		ShortDescription = "a dried flower neck garland",
		LongDescription = null,
		FullDescription = "The piece is light and pliant, built from dried flowers papery at the edges and brittle along the stems rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. The dry pieces rustle faintly rather than bending back into shape. The dried flower neck garland leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 24.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "dried flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Final dried morph target for a short-lived organic jewellery chain.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandWiltedFlowerNeckGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_wilted_flower_neck_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_flower_neck_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:521", []),
		Noun = "garland",
		ShortDescription = "a wilted flower neck garland",
		LongDescription = null,
		FullDescription = "Fresh or faded, wilted flower neck garland keeps the uneven look of something made by hand from living material. The binding is visible in places, with faint tool chatter along the inner face interrupting the line of petals and leaves. The drooping parts still show where the brighter fresh shape used to be. The wilted flower neck garland leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 28.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wilted flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_dried_flower_neck_garland",
		MorphEmote = null,
		MorphSeconds = 86400,
		DestroyedItemStableReference = null,
		BuilderNotes = "Intermediate wilted morph target; can dry further if retained.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandFreshFlowerNeckGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_fresh_flower_neck_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_flower_neck_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:551", []),
		Noun = "garland",
		ShortDescription = "a fresh flower neck garland",
		LongDescription = null,
		FullDescription = "Fresh or faded, fresh flower neck garland keeps the uneven look of something made by hand from living material. The binding is visible in places, with a line of tiny punch marks interrupting the line of petals and leaves. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh flower neck garland reads as festival adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "fresh flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_wilted_flower_neck_garland",
		MorphEmote = null,
		MorphSeconds = 21600,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fresh short-lived organic jewellery; use the listed timed morph target.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandDriedJasmineGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_dried_jasmine_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:582", []),
		Noun = "garland",
		ShortDescription = "a dried jasmine garland",
		LongDescription = null,
		FullDescription = "Fresh or faded, dried jasmine garland keeps the uneven look of something made by hand from living material. The binding is visible in places, with a softened seam where the maker closed it interrupting the line of petals and leaves. The dry pieces rustle faintly rather than bending back into shape. The dried jasmine garland reads as temporary festival adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 24.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "dried flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Final dried morph target for a short-lived organic jewellery chain.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandWiltedJasmineGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_wilted_jasmine_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_jasmine_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:612", []),
		Noun = "garland",
		ShortDescription = "a wilted jasmine garland",
		LongDescription = null,
		FullDescription = "The piece is light and pliant, built from wilted flowers with limp stems and darkening petal tips rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. The drooping parts still show where the brighter fresh shape used to be. The wilted jasmine garland is meant to be noticed in wear, but nothing about it implies armour or magic.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 28.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wilted flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_dried_jasmine_garland",
		MorphEmote = null,
		MorphSeconds = 86400,
		DestroyedItemStableReference = null,
		BuilderNotes = "Intermediate wilted morph target; can dry further if retained.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandFreshJasmineGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_fresh_jasmine_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_jasmine_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:642", []),
		Noun = "garland",
		ShortDescription = "a fresh jasmine garland",
		LongDescription = null,
		FullDescription = "The piece is light and pliant, built from jasmine flowers strung in small pale stars rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh jasmine garland feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "jasmine",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_wilted_jasmine_garland",
		MorphEmote = null,
		MorphSeconds = 21600,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fresh short-lived organic jewellery; use the listed timed morph target.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandDriedMarigoldGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_dried_marigold_garland", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:673", []),
		Noun = "garland",
		ShortDescription = "a dried marigold garland",
		LongDescription = null,
		FullDescription = "The piece is light and pliant, built from dried flowers papery at the edges and brittle along the stems rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. The dry pieces rustle faintly rather than bending back into shape. The piece would signal temporary festival taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 24.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "dried flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Final dried morph target for a short-lived organic jewellery chain.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandWiltedMarigoldGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_wilted_marigold_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_marigold_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:703", []),
		Noun = "garland",
		ShortDescription = "a wilted marigold garland",
		LongDescription = null,
		FullDescription = "The piece is light and pliant, built from wilted flowers with limp stems and darkening petal tips rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. The drooping parts still show where the brighter fresh shape used to be. The wilted marigold garland feels selected for appearance first, with practical wearing kept secondary.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 28.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wilted flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_dried_marigold_garland",
		MorphEmote = null,
		MorphSeconds = 86400,
		DestroyedItemStableReference = null,
		BuilderNotes = "Intermediate wilted morph target; can dry further if retained.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandFreshMarigoldGarland() => new()
	{
		Metadata = new("medieval_jewellery_garland_fresh_marigold_garland", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_marigold_garland"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:733", []),
		Noun = "garland",
		ShortDescription = "a fresh marigold garland",
		LongDescription = null,
		FullDescription = "Fresh or faded, fresh marigold garland keeps the uneven look of something made by hand from living material. The binding is visible in places, with a line of tiny punch marks interrupting the line of petals and leaves. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh marigold garland leaves room for local marks or motifs without needing a different base object.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "marigold",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_wilted_marigold_garland",
		MorphEmote = null,
		MorphSeconds = 21600,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fresh short-lived organic jewellery; use the listed timed morph target.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandDriedRoseChaplet() => new()
	{
		Metadata = new("medieval_jewellery_garland_dried_rose_chaplet", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:764", []),
		Noun = "chaplet",
		ShortDescription = "a dried rose chaplet",
		LongDescription = null,
		FullDescription = "A dried rose chaplet has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. The dry pieces rustle faintly rather than bending back into shape. The dried rose chaplet carries value through visible material and workmanship, not through hidden mechanism.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 24.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "dried flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chaplets",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Chaplet",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Final dried morph target for a short-lived organic jewellery chain.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandWiltedRoseChaplet() => new()
	{
		Metadata = new("medieval_jewellery_garland_wilted_rose_chaplet", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_rose_chaplet"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:794", []),
		Noun = "chaplet",
		ShortDescription = "a wilted rose chaplet",
		LongDescription = null,
		FullDescription = "Fresh or faded, wilted rose chaplet keeps the uneven look of something made by hand from living material. The binding is visible in places, with a small flattened spot from long handling interrupting the line of petals and leaves. The drooping parts still show where the brighter fresh shape used to be. The piece would signal temporary festival taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 28.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "wilted flower",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chaplets",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Chaplet",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_dried_rose_chaplet",
		MorphEmote = null,
		MorphSeconds = 86400,
		DestroyedItemStableReference = null,
		BuilderNotes = "Intermediate wilted morph target; can dry further if retained.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandFreshRoseChaplet() => new()
	{
		Metadata = new("medieval_jewellery_garland_fresh_rose_chaplet", "medieval", ["medieval"], ["item:medieval_jewellery_garland_wilted_rose_chaplet"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:824", []),
		Noun = "chaplet",
		ShortDescription = "a fresh rose chaplet",
		LongDescription = null,
		FullDescription = "Fresh or faded, fresh rose chaplet keeps the uneven look of something made by hand from living material. The binding is visible in places, with faint tool chatter along the inner face interrupting the line of petals and leaves. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh rose chaplet reads as festival adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Standard,
		WeightInGrams = 30.0,
		Cost = 3.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "rose",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chaplets",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Chaplet",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_wilted_rose_chaplet",
		MorphEmote = null,
		MorphSeconds = 21600,
		DestroyedItemStableReference = null,
		BuilderNotes = "Fresh short-lived organic jewellery; use the listed timed morph target.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandDriedIvyWreath() => new()
	{
		Metadata = new("medieval_jewellery_garland_dried_ivy_wreath", "medieval", ["medieval"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:855", []),
		Noun = "wreath",
		ShortDescription = "a dried ivy wreath",
		LongDescription = null,
		FullDescription = "A dried ivy wreath has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. The dry pieces rustle faintly rather than bending back into shape. The piece would signal temporary festival taste while remaining straightforward jewellery.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Substandard,
		WeightInGrams = 24.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ivy",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Functions / Worn Items / Jewellery / Wreaths / Dried Wreaths",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Final dried morph target for a short-lived organic jewellery chain.",
		AllowLegacyShortDescriptionMatch = true,
	};

	public static CatalogueItemDefinition MedievalJewelleryGarlandWiltedIvyWreath() => new()
	{
		Metadata = new("medieval_jewellery_garland_wilted_ivy_wreath", "medieval", ["medieval"], ["item:medieval_jewellery_garland_dried_ivy_wreath"], ItemSeederOwnershipPolicy.RequiredRelationship, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.MedievalJewellery.cs:886", []),
		Noun = "wreath",
		ShortDescription = "a wilted ivy wreath",
		LongDescription = null,
		FullDescription = "Fresh or faded, wilted ivy wreath keeps the uneven look of something made by hand from living material. The binding is visible in places, with a softened seam where the maker closed it interrupting the line of petals and leaves. The drooping parts still show where the brighter fresh shape used to be. The wilted ivy wreath reads as temporary festival adornment rather than a tool, charm, or container.",
		Size = SizeCategory.VerySmall,
		Quality = ItemQuality.Poor,
		WeightInGrams = 28.0,
		Cost = 1.0m,
		Skinnable = true,
		HiddenFromPlayers = false,
		Material = "ivy",
		Tags = [
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Functions / Worn Items / Jewellery / Wreaths / Flower Wreaths",
				"Market / Jewellery / Ephemeral Jewellery"
			],
		Components = [
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
		MorphToStableReference = "medieval_jewellery_garland_dried_ivy_wreath",
		MorphEmote = null,
		MorphSeconds = 86400,
		DestroyedItemStableReference = null,
		BuilderNotes = "Intermediate wilted morph target; can dry further if retained.",
		AllowLegacyShortDescriptionMatch = true,
	};
}
