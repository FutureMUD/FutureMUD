#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalHouseholdFurniture()
	{
		CreateItem(
			"medieval_water_brackish_wellhead",
			"wellhead",
			"a brackish wellhead",
			null,
			"This brackish wellhead is a large, workmanlike wellhead cut from stone. A raised rim surrounds the draw opening, with worn contact marks where rope and bucket have passed. The top edge is smooth from repeated use. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			48000.0,
			70.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_BrackishWaterSource"
			],
			null,
			null,
			null,
			null,
			"Wellhead or draw point for brackish water that is not clean potable supply."
		);

		CreateItem(
			"medieval_water_bronze_laver_basin",
			"basin",
			"a bronze laver basin",
			null,
			"This bronze laver basin is a medium-sized, well-made basin worked from bronze. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Normal,
			ItemQuality.Good,
			4300.0,
			70.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Sink_20L"
			],
			null,
			null,
			null,
			null,
			"Better metal basin for handwashing, fine service, and chamber use."
		);

		CreateItem(
			"medieval_water_bucket_filling_stand",
			"stand",
			"a bucket-filling cistern stand",
			null,
			"This bucket-filling cistern stand is a medium-sized, workmanlike stand built from oak boards. A covered box surrounds the water chamber, with a filling lip and a darker seam below the lid. The front face is marked where vessels are brought close. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			9000.0,
			36.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicCisternWaterSource"
			],
			null,
			null,
			null,
			null,
			"Raised filling stand for buckets, pails, and small tubs."
		);

		CreateItem(
			"medieval_water_butcher_wash_sink",
			"sink",
			"a butcher's wash sink",
			null,
			"This butcher's wash sink is a large, workmanlike sink cut from stone. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			26000.0,
			70.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Sink_50L"
			],
			null,
			null,
			null,
			null,
			"Large stone sink for butchery, dyeing, fish cleaning, or other messy household work."
		);

		CreateItem(
			"medieval_water_controlled_stone_basin",
			"basin",
			"a controlled stone water basin",
			null,
			"This controlled stone water basin is a large, well-made basin cut from stone. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Good,
			32000.0,
			110.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"WaterSource_ProgControlled"
			],
			null,
			null,
			null,
			null,
			"Prog-controlled water basin for areas where supply depends on local script or builder logic."
		);

		CreateItem(
			"medieval_water_covered_cistern_chest",
			"cistern",
			"a covered timber cistern",
			null,
			"This covered timber cistern is a large, workmanlike cistern built from oak boards. A covered box surrounds the water chamber, with a filling lip and a darker seam below the lid. The front face is marked where vessels are brought close. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			24000.0,
			92.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicCisternWaterSource"
			],
			null,
			null,
			null,
			null,
			"Covered timber cistern casing for a managed household or yard water supply."
		);

		CreateItem(
			"medieval_water_earthenware_laver_basin",
			"basin",
			"an earthenware laver basin",
			null,
			"This earthenware laver basin is a medium-sized, workmanlike basin formed from earthenware. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5200.0,
			20.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Sink_20L"
			],
			null,
			null,
			null,
			null,
			"Broad fillable laver basin for household washing and rinsing."
		);

		CreateItem(
			"medieval_water_iron_well_pump",
			"pump",
			"an iron well pump",
			null,
			"This iron well pump is a large, well-made pump worked from wrought iron. A handle rises from the pump body, with a spout projecting over the filling space. The base plate is broad and marked by bolt holes. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Good,
			22000.0,
			180.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Infinite_PublicPumpWaterSource"
			],
			null,
			null,
			null,
			null,
			"Sturdier hand-pump fixture for a well, cistern, or controlled public water point."
		);

		CreateItem(
			"medieval_water_lake_draw_basin",
			"basin",
			"a lake-water draw basin",
			null,
			"This lake-water draw basin is a large, workmanlike basin cut from stone. A shaped rim marks the draw point, with a lowered front edge for dipping vessels. The wet contact line is visible around the inside. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			28000.0,
			60.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_LakeWaterSource"
			],
			null,
			null,
			null,
			null,
			"Draw basin representing self-refilling lake-water access."
		);

		CreateItem(
			"medieval_water_large_laundry_tub",
			"tub",
			"a large laundry tub",
			null,
			"This large laundry tub is a large, workmanlike tub built from oak boards. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			46.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Sink_50L"
			],
			null,
			null,
			null,
			null,
			"Large fillable tub for laundry, soaking cloth, and heavy household washing."
		);

		CreateItem(
			"medieval_water_laundry_rinse_trough",
			"trough",
			"a laundry rinse trough",
			null,
			"This laundry rinse trough is a large, workmanlike trough cut from stone. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			33000.0,
			78.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Sink_50L"
			],
			null,
			null,
			null,
			null,
			"Large fillable rinse trough for laundry, cloth preparation, and soaking."
		);

		CreateItem(
			"medieval_water_oak_bath_tub",
			"tub",
			"an oak bath tub",
			null,
			"This oak bath tub is a very large, workmanlike tub built from oak boards. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			30000.0,
			120.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bathtub"
			],
			null,
			null,
			null,
			null,
			"Large wooden bathing tub for bathhouse, inn, or wealthy household use."
		);

		CreateItem(
			"medieval_water_oak_livestock_trough",
			"trough",
			"an oak livestock trough",
			null,
			"This oak livestock trough is a large, workmanlike trough built from oak boards. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			48.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicTroughWaterSource"
			],
			null,
			null,
			null,
			null,
			"Timber water trough suited to stables, yards, and byres."
		);

		CreateItem(
			"medieval_water_oak_wash_tub",
			"tub",
			"an oak wash tub",
			null,
			"This oak wash tub is a large, workmanlike tub built from oak boards. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			28.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Sink_20L"
			],
			null,
			null,
			null,
			null,
			"Wooden wash tub for laundry, scullery work, or household rinsing."
		);

		CreateItem(
			"medieval_water_pine_plank_bath_tub",
			"tub",
			"a pine plank bath tub",
			null,
			"This pine plank bath tub is a very large, workmanlike tub built from pine boards. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			26000.0,
			90.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bathtub"
			],
			null,
			null,
			null,
			null,
			"Planked wooden tub sized for bathing and hot-water service."
		);

		CreateItem(
			"medieval_water_public_stone_cistern",
			"cistern",
			"a public stone cistern",
			null,
			"This public stone cistern is a very large, workmanlike cistern cut from stone. A covered box surrounds the water chamber, with a filling lip and a darker seam below the lid. The front face is marked where vessels are brought close. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			52000.0,
			160.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicCisternWaterSource"
			],
			null,
			null,
			null,
			null,
			"Self-refilling public cistern for communal potable water supply."
		);

		CreateItem(
			"medieval_water_public_stone_trough",
			"trough",
			"a public stone water trough",
			null,
			"This public stone water trough is a very large, workmanlike trough cut from stone. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			52000.0,
			130.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicTroughWaterSource"
			],
			null,
			null,
			null,
			null,
			"Self-refilling trough for animals, carts, washing, and public water access."
		);

		CreateItem(
			"medieval_water_refilling_oak_tub",
			"tub",
			"a refilling oak water tub",
			null,
			"This refilling oak water tub is a large, workmanlike tub built from oak boards. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			20000.0,
			60.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_WaterSource"
			],
			null,
			null,
			null,
			null,
			"Generic self-refilling water tub for scripted household or service-yard use."
		);

		CreateItem(
			"medieval_water_river_filling_plank",
			"plank",
			"a river filling plank",
			null,
			"This river filling plank is a medium-sized, workmanlike plank built from oak boards. A shaped rim marks the draw point, with a lowered front edge for dipping vessels. The wet contact line is visible around the inside. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			7000.0,
			20.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_RiverWaterSource"
			],
			null,
			null,
			null,
			null,
			"Planked river-edge filling point for buckets, tubs, and washing."
		);

		CreateItem(
			"medieval_water_river_wash_stand",
			"stand",
			"a river wash stand",
			null,
			"This river wash stand is a large, workmanlike stand built from oak boards. A shaped rim marks the draw point, with a lowered front edge for dipping vessels. The wet contact line is visible around the inside. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12000.0,
			32.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_RiverWaterSource"
			],
			null,
			null,
			null,
			null,
			"Frame and wash point representing access to a self-refilling river water source."
		);

		CreateItem(
			"medieval_water_saltwater_wash_trough",
			"trough",
			"a saltwater wash trough",
			null,
			"This saltwater wash trough is a large, workmanlike trough cut from stone. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			30000.0,
			56.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SaltWaterSource"
			],
			null,
			null,
			null,
			null,
			"Wash trough drawing from salt water for coastal cleaning, fish work, or rinsing."
		);

		CreateItem(
			"medieval_water_small_ceramic_hand_basin",
			"basin",
			"a small ceramic hand basin",
			null,
			"This small ceramic hand basin is a small, workmanlike basin formed from ceramic. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1300.0,
			10.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Sink_5L"
			],
			null,
			null,
			null,
			null,
			"Small fillable basin for hand washing, shaving, rinsing cloths, or bedside water use."
		);

		CreateItem(
			"medieval_water_stone_bath_basin",
			"basin",
			"a stone bath basin",
			null,
			"This stone bath basin is a very large, well-made basin cut from stone. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			76000.0,
			180.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bathtub"
			],
			null,
			null,
			null,
			null,
			"Very heavy stone bathing basin for a bathhouse, courtly chamber, or wealthy household."
		);

		CreateItem(
			"medieval_water_stone_handwashing_basin",
			"basin",
			"a stone handwashing basin",
			null,
			"This stone handwashing basin is a medium-sized, workmanlike basin cut from stone. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			18.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Sink_5L"
			],
			null,
			null,
			null,
			null,
			"Heavy stone basin for handwashing near a hall, kitchen, or chamber door."
		);

		CreateItem(
			"medieval_water_stone_scullery_sink",
			"sink",
			"a stone scullery sink",
			null,
			"This stone scullery sink is a large, workmanlike sink cut from stone. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			54.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Sink_20L"
			],
			null,
			null,
			null,
			null,
			"Heavy fillable sink for scullery, kitchen, and workshop washing."
		);

		CreateItem(
			"medieval_water_stone_spring_basin",
			"basin",
			"a stone spring basin",
			null,
			"This stone spring basin is a large, workmanlike basin cut from stone. A shaped rim marks the draw point, with a lowered front edge for dipping vessels. The wet contact line is visible around the inside. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			30000.0,
			82.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SpringWaterSource"
			],
			null,
			null,
			null,
			null,
			"Stone basin built around a self-refilling spring source."
		);

		CreateItem(
			"medieval_water_stone_washing_trough",
			"trough",
			"a stone washing trough",
			null,
			"This stone washing trough is a large, workmanlike trough cut from stone. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			25000.0,
			58.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Sink_50L"
			],
			null,
			null,
			null,
			null,
			"Heavy fillable stone trough for rinsing, scullery work, and yard washing."
		);

		CreateItem(
			"medieval_water_stone_wellhead",
			"wellhead",
			"a stone wellhead",
			null,
			"This stone wellhead is a very large, workmanlike wellhead cut from stone. A raised rim surrounds the draw opening, with worn contact marks where rope and bucket have passed. The top edge is smooth from repeated use. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			68000.0,
			150.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_WaterSource"
			],
			null,
			null,
			null,
			null,
			"Portable/installable wellhead prototype for a generic self-refilling water source."
		);

		CreateItem(
			"medieval_water_swamp_drain_basin",
			"basin",
			"a swamp-water drain basin",
			null,
			"This swamp-water drain basin is a large, plain basin built from plain wood. A shaped rim marks the draw point, with a lowered front edge for dipping vessels. The wet contact line is visible around the inside. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Poor,
			16000.0,
			26.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SwampWaterSource"
			],
			null,
			null,
			null,
			null,
			"Rough draw basin for swamp or fen water; useful as an environmental source rather than clean household water."
		);

		CreateItem(
			"medieval_water_timber_spring_box",
			"box",
			"a timber spring box",
			null,
			"This timber spring box is a large, workmanlike box built from oak boards. A shaped rim marks the draw point, with a lowered front edge for dipping vessels. The wet contact line is visible around the inside. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			16000.0,
			54.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SpringWaterSource"
			],
			null,
			null,
			null,
			null,
			"Timber spring box for collecting clean spring water at a yard or roadside."
		);

		CreateItem(
			"medieval_water_timber_wellhead",
			"wellhead",
			"a timber wellhead",
			null,
			"This timber wellhead is a large, workmanlike wellhead built from oak boards. A raised rim surrounds the draw opening, with worn contact marks where rope and bucket have passed. The top edge is smooth from repeated use. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			24000.0,
			78.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_WaterSource"
			],
			null,
			null,
			null,
			null,
			"Timber wellhead or draw-point for a household, yard, or village water supply."
		);

		CreateItem(
			"medieval_water_village_hand_pump",
			"pump",
			"a village hand pump",
			null,
			"This village hand pump is a large, workmanlike pump worked from wrought iron. A handle rises from the pump body, with a spout projecting over the filling space. The base plate is broad and marked by bolt holes. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			130.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Infinite_PublicPumpWaterSource"
			],
			null,
			null,
			null,
			null,
			"Hand-pump style public potable water source for villages, yards, or service courts."
		);

		CreateItem(
			"medieval_water_northern_bogwater_trough",
			"trough",
			"a bog-water trough",
			null,
			"This bog-water trough is a large, plain trough built from oak boards. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Poor,
			18000.0,
			28.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SwampWaterSource"
			],
			null,
			null,
			null,
			null,
			"Rough trough for fen, bog, or marsh water access."
		);

		CreateItem(
			"medieval_water_northern_spring_trough",
			"trough",
			"a cold spring trough",
			null,
			"This cold spring trough is a large, workmanlike trough cut from stone. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			30000.0,
			72.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SpringWaterSource"
			],
			null,
			null,
			null,
			null,
			"Stone trough fed by a cold spring, suitable for northern yards or roadside supply."
		);

		CreateItem(
			"medieval_water_northern_stave_bath_tub",
			"tub",
			"a stave-built bath tub",
			null,
			"This stave-built bath tub is a very large, workmanlike tub built from oak boards. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			28000.0,
			105.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bathtub"
			],
			null,
			null,
			null,
			null,
			"Stave-built wooden bath tub suited to cold-climate bathhouse and hall use."
		);

		CreateItem(
			"medieval_water_western_lavatorium_basin",
			"basin",
			"a lavatorium stone basin",
			null,
			"This lavatorium stone basin is a large, well-made basin cut from stone. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Good,
			36000.0,
			120.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Sink_50L"
			],
			null,
			null,
			null,
			null,
			"Large formal washing basin for institutional, refectory, or hall settings."
		);

		CreateItem(
			"medieval_water_western_manor_well_pump",
			"pump",
			"a manor well pump",
			null,
			"This manor well pump is a large, well-made pump worked from wrought iron. A handle rises from the pump body, with a spout projecting over the filling space. The base plate is broad and marked by bolt holes. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Good,
			23000.0,
			190.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Infinite_PublicPumpWaterSource"
			],
			null,
			null,
			null,
			null,
			"Sturdy pump for a manor yard, service court, or enclosed well."
		);

		CreateItem(
			"medieval_water_western_town_cistern",
			"cistern",
			"a town conduit cistern",
			null,
			"This town conduit cistern is a very large, well-made cistern cut from stone. A covered box surrounds the water chamber, with a filling lip and a darker seam below the lid. The front face is marked where vessels are brought close. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			62000.0,
			190.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicCisternWaterSource"
			],
			null,
			null,
			null,
			null,
			"Communal cistern or conduit head for town water supply."
		);

		CreateItem(
			"medieval_water_mediterranean_courtyard_fountain",
			"basin",
			"a courtyard fountain basin",
			null,
			"This courtyard fountain basin is a large, well-made basin cut from marble. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Good,
			46000.0,
			210.0m,
			true,
			false,
			"marble",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_WaterSource"
			],
			null,
			null,
			null,
			null,
			"Self-refilling courtyard fountain basin for wealthy domestic or public settings."
		);

		CreateItem(
			"medieval_water_mediterranean_limestone_cistern_basin",
			"basin",
			"a limestone cistern basin",
			null,
			"This limestone cistern basin is a large, workmanlike basin cut from limestone. A covered box surrounds the water chamber, with a filling lip and a darker seam below the lid. The front face is marked where vessels are brought close. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			38000.0,
			100.0m,
			true,
			false,
			"limestone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicCisternWaterSource"
			],
			null,
			null,
			null,
			null,
			"Limestone draw basin linked to a managed cistern supply."
		);

		CreateItem(
			"medieval_water_mediterranean_marble_bath",
			"basin",
			"a marble bath basin",
			null,
			"This marble bath basin is a very large, well-made basin cut from marble. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			82000.0,
			300.0m,
			true,
			false,
			"marble",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bathtub"
			],
			null,
			null,
			null,
			null,
			"Heavy marble bath basin for elite bathhouse or courtyard bathing."
		);

		CreateItem(
			"medieval_water_mediterranean_terracotta_cistern",
			"cistern",
			"a terracotta cistern jar",
			null,
			"This terracotta cistern jar is a large, workmanlike cistern formed from terracotta. A rounded jar body rises to a covered mouth, with a thick shoulder and a steady base. Dark water marks gather below the rim where vessels are filled. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			64.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Infinite_PublicCisternWaterSource"
			],
			null,
			null,
			null,
			null,
			"Large terracotta cistern vessel for warm-climate household water storage."
		);

		CreateItem(
			"medieval_water_islamicate_ablution_basin",
			"basin",
			"a marble ablution basin",
			null,
			"This marble ablution basin is a large, well-made basin cut from marble. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Good,
			42000.0,
			210.0m,
			true,
			false,
			"marble",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicCisternWaterSource"
			],
			null,
			null,
			null,
			null,
			"Public or courtyard washing basin suited to ablution and household washing."
		);

		CreateItem(
			"medieval_water_islamicate_brass_wash_basin",
			"basin",
			"a brass washing basin",
			null,
			"This brass washing basin is a medium-sized, well-made basin worked from brass. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			82.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Sink_20L"
			],
			null,
			null,
			null,
			null,
			"Bright metal fillable basin for urban household washing and guest hospitality."
		);

		CreateItem(
			"medieval_water_islamicate_hammam_bath",
			"basin",
			"a hammam bath basin",
			null,
			"This hammam bath basin is a very large, well-made basin cut from stone. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			78000.0,
			240.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bathtub"
			],
			null,
			null,
			null,
			null,
			"Large bath basin for bathhouse or elite domestic washing."
		);

		CreateItem(
			"medieval_water_islamicate_sabil_cistern",
			"cistern",
			"a public sabil cistern",
			null,
			"This public sabil cistern is a very large, well-made cistern cut from stone. A covered box surrounds the water chamber, with a filling lip and a darker seam below the lid. The front face is marked where vessels are brought close. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			64000.0,
			220.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicCisternWaterSource"
			],
			null,
			null,
			null,
			null,
			"Charitable public cistern or drinking-water fixture for an urban street or courtyard."
		);

		CreateItem(
			"medieval_water_south_asian_brass_washing_basin",
			"basin",
			"a brass washing basin",
			null,
			"This brass washing basin is a medium-sized, well-made basin worked from brass. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3400.0,
			78.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Sink_20L"
			],
			null,
			null,
			null,
			null,
			"Metal basin for washing, rinsing, and household water service."
		);

		CreateItem(
			"medieval_water_south_asian_public_trough",
			"trough",
			"a stone public water trough",
			null,
			"This stone public water trough is a very large, workmanlike trough cut from stone. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			54000.0,
			130.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicTroughWaterSource"
			],
			null,
			null,
			null,
			null,
			"Public water trough for travellers, animals, and household drawing."
		);

		CreateItem(
			"medieval_water_south_asian_stepwell_draw_basin",
			"basin",
			"a stepwell draw basin",
			null,
			"This stepwell draw basin is a large, workmanlike basin cut from stone. A raised rim surrounds the draw opening, with worn contact marks where rope and bucket have passed. The top edge is smooth from repeated use. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			42000.0,
			110.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SpringWaterSource"
			],
			null,
			null,
			null,
			null,
			"Draw basin representing a spring-fed or stepwell water source."
		);

		CreateItem(
			"medieval_water_south_asian_teak_bath_tub",
			"tub",
			"a teak bathing tub",
			null,
			"This teak bathing tub is a very large, well-made tub built from teak boards. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			27000.0,
			150.0m,
			true,
			false,
			"teak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bathtub"
			],
			null,
			null,
			null,
			null,
			"Large teak tub for bathing in a warm-climate household or bathhouse."
		);

		CreateItem(
			"medieval_water_east_asian_bamboo_spring_trough",
			"trough",
			"a bamboo spring trough",
			null,
			"This bamboo spring trough is a large, workmanlike trough built from split bamboo. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8000.0,
			36.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SpringWaterSource"
			],
			null,
			null,
			null,
			null,
			"Bamboo-lined trough fed by a small spring or watercourse."
		);

		CreateItem(
			"medieval_water_east_asian_cypress_bath_tub",
			"tub",
			"a cypress bath tub",
			null,
			"This cypress bath tub is a very large, well-made tub built from cypress boards. A deep oval basin stands on a stable base, with a rolled rim broad enough to grip. The inside is smooth and worn where water has stood. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			24000.0,
			160.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bathtub"
			],
			null,
			null,
			null,
			null,
			"Large cypress bath tub for household bathing or inn use."
		);

		CreateItem(
			"medieval_water_east_asian_cypress_wash_basin",
			"basin",
			"a cypress wash basin",
			null,
			"This cypress wash basin is a medium-sized, well-made basin built from cypress boards. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2600.0,
			54.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Sink_20L"
			],
			null,
			null,
			null,
			null,
			"Clean wooden wash basin for domestic, inn, or bathhouse use."
		);

		CreateItem(
			"medieval_water_east_asian_stone_garden_basin",
			"basin",
			"a stone garden water basin",
			null,
			"This stone garden water basin is a large, well-made basin cut from stone. A hollow basin sits within a steady frame, with a broad rim and a clear filling space. The inside slopes gently toward the deepest point. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Good,
			36000.0,
			130.0m,
			true,
			false,
			"stone",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_SpringWaterSource"
			],
			null,
			null,
			null,
			null,
			"Stone water basin suited to a garden, courtyard, or refined household setting."
		);

		CreateItem(
			"medieval_water_steppe_camp_trough",
			"trough",
			"a camp water trough",
			null,
			"This camp water trough is a large, workmanlike trough built from plain wood. A long open channel forms the water space, with thick ends and a flat base. The inside is smoothed where water and buckets have worn it. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			14000.0,
			38.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicTroughWaterSource"
			],
			null,
			null,
			null,
			null,
			"Portable/installable trough for a camp, herd, caravan, or way station."
		);

		CreateItem(
			"medieval_water_steppe_caravan_cistern_box",
			"cistern",
			"a caravan cistern box",
			null,
			"This caravan cistern box is a large, well-made cistern built from oak boards. A covered box surrounds the water chamber, with a filling lip and a darker seam below the lid. The front face is marked where vessels are brought close. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			120.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_PublicCisternWaterSource"
			],
			null,
			null,
			null,
			null,
			"Boxed cistern fixture for a caravanserai yard or mobile-camp support setting."
		);

		CreateItem(
			"medieval_water_steppe_river_draw_frame",
			"frame",
			"a river draw frame",
			null,
			"This river draw frame is a large, workmanlike frame built from plain wood. A shaped rim marks the draw point, with a lowered front edge for dipping vessels. The wet contact line is visible around the inside. Damp staining and smoothed contact points mark the places most often touched by water vessels.",
			SizeCategory.Large,
			ItemQuality.Standard,
			10000.0,
			28.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Infinite_RiverWaterSource"
			],
			null,
			null,
			null,
			null,
			"Light frame marking a controlled river-water drawing point for camp use."
		);

		CreateItem(
			"medieval_furniture_account_desk",
			"desk",
			"an account-keeper's desk",
			null,
			"This account-keeper's desk is a large, well-made desk built from oak boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			150.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Sturdy desk surface for counting, writing, seals, and household or guild accounts."
		);

		CreateItem(
			"medieval_furniture_backless_bench",
			"bench",
			"a backless wooden bench",
			null,
			"This backless wooden bench is a large, workmanlike bench built from pine boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			11000.0,
			28.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Plain three-seat bench for halls, kitchens, workshops, or dormitories."
		);

		CreateItem(
			"medieval_furniture_bakers_table",
			"table",
			"a flour-dusted baker's table",
			null,
			"This flour-dusted baker's table is a large, workmanlike table built from beech boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			24000.0,
			66.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Six",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Broad smooth-topped table for kneading, cooling loaves, and laying out baking vessels."
		);

		CreateItem(
			"medieval_furniture_bedside_table",
			"table",
			"a small bedside table",
			null,
			"This small bedside table is a medium-sized, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6200.0,
			24.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Side_Table"
			],
			null,
			null,
			null,
			null,
			"Small table for bedside lamps, cups, basins, books, or chamber goods."
		);

		CreateItem(
			"medieval_furniture_butcher_block_table",
			"table",
			"a thick butcher-block table",
			null,
			"This thick butcher-block table is a large, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			36000.0,
			120.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Heavy block-topped work table for butchery, curing, and other rough household or trade preparation."
		);

		CreateItem(
			"medieval_furniture_campaign_table",
			"table",
			"a folding campaign table",
			null,
			"This folding campaign table is a medium-sized, well-made table built from ash boards. A rectangular top rests on hinged legs that fold beneath it. The joints are pinned and rubbed bright where the frame moves. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			9000.0,
			90.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Compact folding table for travel, military camps, counting houses, and temporary halls."
		);

		CreateItem(
			"medieval_furniture_candle_table",
			"table",
			"a small candle table",
			null,
			"This small candle table is a medium-sized, workmanlike table built from ash boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6500.0,
			22.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Side_Table"
			],
			null,
			null,
			null,
			null,
			"Small table for lamps, candle trays, cups, or other chamber-side objects."
		);

		CreateItem(
			"medieval_furniture_canvas_field_cot",
			"cot",
			"a canvas field cot",
			null,
			"This canvas field cot is a large, workmanlike cot made from coarse canvas. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			7200.0,
			45.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Portable cot with canvas as the primary visible material for camp or travel use."
		);

		CreateItem(
			"medieval_furniture_carved_armchair",
			"chair",
			"a carved armchair",
			null,
			"This carved armchair is a large, well-made chair built from walnut boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			13000.0,
			150.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Fine single chair with arms and carved visible joinery."
		);

		CreateItem(
			"medieval_furniture_carved_court_table",
			"table",
			"a carved court table",
			null,
			"This carved court table is a large, well-made table built from walnut boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			220.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Fine carved table for a court chamber, solar, hall dais, or elite household room."
		);

		CreateItem(
			"medieval_furniture_carved_cradle",
			"cradle",
			"a carved wooden cradle",
			null,
			"This carved wooden cradle is a large, well-made cradle built from walnut boards. A small sleeping box sits on curved rockers, with low sides and rounded ends. The inner surface is smooth enough for blankets. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			9000.0,
			100.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Fine infant cradle with visible carved sides and a narrow surface."
		);

		CreateItem(
			"medieval_furniture_carved_lectern_stand",
			"stand",
			"a carved reading stand",
			null,
			"This carved reading stand is a large, well-made stand built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			11000.0,
			120.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Fine carved stand usable for public reading, accounts, or display without religious specificity."
		);

		CreateItem(
			"medieval_furniture_carved_throne",
			"throne",
			"a carved wooden throne",
			null,
			"This carved wooden throne is a large, well-made throne built from walnut boards. A raised seat stands above shaped supports, with a taller back and more formal proportions than an ordinary chair. The front edge is carefully finished. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			22000.0,
			300.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Elaborate single seat with carved arms and a tall back; not a storage object."
		);

		CreateItem(
			"medieval_furniture_chamber_settle",
			"settle",
			"a small chamber settle",
			null,
			"This small chamber settle is a large, workmanlike settle built from beech boards. A long seat is set below a high back, with enclosed ends that make it feel sheltered. The sitting edge is rounded from use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			19000.0,
			85.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Double",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Two-seat high-backed bench suited to a private chamber or quiet hearth corner."
		);

		CreateItem(
			"medieval_furniture_childs_small_table",
			"table",
			"a child-sized small table",
			null,
			"This child-sized small table is a medium-sized, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5200.0,
			16.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Small low table for children, light chamber use, or cramped household corners."
		);

		CreateItem(
			"medieval_furniture_clerk_stool",
			"stool",
			"a clerk's writing stool",
			null,
			"This clerk's writing stool is a medium-sized, workmanlike stool built from ash boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3200.0,
			18.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Simple single stool sized for desks, counters, and writing tables."
		);

		CreateItem(
			"medieval_furniture_clerks_desk",
			"desk",
			"a clerk's narrow desk",
			null,
			"This clerk's narrow desk is a large, workmanlike desk built from oak boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			17500.0,
			90.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Narrow desk surface for account rolls, ledgers, seals, and correspondence."
		);

		CreateItem(
			"medieval_furniture_coarse_hemp_pallet",
			"pallet",
			"a coarse hemp pallet",
			null,
			"This coarse hemp pallet is a large, workmanlike pallet made from woven hemp. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			7000.0,
			14.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Cheap stuffed sleeping pallet for servants, camps, and poor households."
		);

		CreateItem(
			"medieval_furniture_compact_bench",
			"bench",
			"a compact two-seat bench",
			null,
			"This compact two-seat bench is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			10000.0,
			30.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Double",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Two-seat bench for smaller rooms, entries, and chamber walls."
		);

		CreateItem(
			"medieval_furniture_copyists_table",
			"table",
			"a copyist's table",
			null,
			"This copyist's table is a large, well-made table built from beech boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			20000.0,
			110.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Broad writing and copying table for manuscripts, drafts, or formal documents."
		);

		CreateItem(
			"medieval_furniture_counting_table",
			"table",
			"a narrow counting table",
			null,
			"This narrow counting table is a large, well-made table built from walnut boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			16000.0,
			120.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Long narrow table for coin counting, account tablets, tallies, and small document bundles."
		);

		CreateItem(
			"medieval_furniture_cushioned_bench",
			"bench",
			"a cushioned wooden bench",
			null,
			"This cushioned wooden bench is a large, well-made bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			16000.0,
			90.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Three-seat bench with visible textile cushioning and a usable bench surface."
		);

		CreateItem(
			"medieval_furniture_cushioned_chair",
			"chair",
			"a cushioned wooden chair",
			null,
			"This cushioned wooden chair is a large, well-made chair built from oak boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			11000.0,
			95.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Single wooden chair with visible textile padding and a better-finished frame."
		);

		CreateItem(
			"medieval_furniture_daybed_frame",
			"daybed",
			"a plain wooden daybed",
			null,
			"This plain wooden daybed is a very large, workmanlike daybed built from beech boards. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			30000.0,
			95.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Long wooden daybed frame treated as a bed surface rather than a couch seat."
		);

		CreateItem(
			"medieval_furniture_display_table",
			"table",
			"a plain display table",
			null,
			"This plain display table is a large, workmanlike table built from ash boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			17000.0,
			60.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Table with a clear surface for laying out goods, household objects, or wares for inspection."
		);

		CreateItem(
			"medieval_furniture_document_sorting_table",
			"table",
			"a document sorting table",
			null,
			"This document sorting table is a large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			85.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Six",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Large desk-like table for bundles, seals, accounts, records, and sorting work."
		);

		CreateItem(
			"medieval_furniture_dyer_sorting_table",
			"table",
			"a stained sorting table",
			null,
			"This stained sorting table is a large, workmanlike table built from elm boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			55.0m,
			true,
			false,
			"elm",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Six",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Work table with worn, stained boards suited to sorting cloth, skeins, or workshop goods."
		);

		CreateItem(
			"medieval_furniture_feast_table",
			"table",
			"a massive feast table",
			null,
			"This massive feast table is a huge, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Huge,
			ItemQuality.Good,
			74000.0,
			260.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Twenty",
				"Container_Large_Table"
			],
			null,
			null,
			null,
			null,
			"Large twenty-place feasting table for great halls, courts, guild halls, or large households."
		);

		CreateItem(
			"medieval_furniture_felt_sleeping_mat",
			"mat",
			"a felt sleeping mat",
			null,
			"This felt sleeping mat is a large, workmanlike mat made from pressed felt. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			4800.0,
			24.0m,
			true,
			false,
			"felt",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Rollable felt-like sleeping mat represented as a cot-width surface."
		);

		CreateItem(
			"medieval_furniture_fine_desk_chair",
			"chair",
			"a fine desk chair",
			null,
			"This fine desk chair is a medium-sized, well-made chair built from walnut boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8500.0,
			110.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Fine single chair intended for a writing desk, account table, or solar chamber."
		);

		CreateItem(
			"medieval_furniture_fine_silk_cushion",
			"cushion",
			"a fine silk floor cushion",
			null,
			"This fine silk floor cushion is a medium-sized, well-made cushion made from woven silk. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1200.0,
			85.0m,
			true,
			false,
			"silk",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Fine single floor cushion for elite chambers, receptions, and low seating."
		);

		CreateItem(
			"medieval_furniture_folded_stool",
			"stool",
			"a folding wooden stool",
			null,
			"This folding wooden stool is a small, well-made stool built from ash boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			2100.0,
			34.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Portable folding stool; mechanically a simple single seat."
		);

		CreateItem(
			"medieval_furniture_folded_trestle_table",
			"table",
			"a folding trestle table",
			null,
			"This folding trestle table is a large, workmanlike table built from pine boards. A rectangular top rests on hinged legs that fold beneath it. The joints are pinned and rubbed bright where the frame moves. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			15000.0,
			54.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Portable table with trestle-style supports; treated mechanically as a normal four-place table."
		);

		CreateItem(
			"medieval_furniture_folding_writing_desk",
			"desk",
			"a folding writing desk",
			null,
			"This folding writing desk is a medium-sized, well-made desk built from ash boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8500.0,
			95.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Portable folding desk with a writing surface but no special storage behaviour."
		);

		CreateItem(
			"medieval_furniture_fur_sleeping_roll",
			"bedroll",
			"a fur sleeping roll",
			null,
			"This fur sleeping roll is a medium-sized, well-made bedroll made from dressed fur. Rolled bedding is tied with straps, with the outer layer wrapped tight around the softer middle. The ends show folded cloth and compressed padding. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Normal,
			ItemQuality.Good,
			6200.0,
			85.0m,
			true,
			false,
			"fur",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Warm fur-lined sleeping roll for travel or cold chambers."
		);

		CreateItem(
			"medieval_furniture_games_table",
			"table",
			"a marked games table",
			null,
			"This marked games table is a large, workmanlike table built from beech boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			16000.0,
			70.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Four-place table with faint marked squares and lines suitable for games, dice, and boards."
		);

		CreateItem(
			"medieval_furniture_guardroom_cot",
			"cot",
			"a guardroom wooden cot",
			null,
			"This guardroom wooden cot is a large, workmanlike cot built from pine boards. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			40.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Plain narrow cot suited to watches, barracks, gatehouses, and communal rooms."
		);

		CreateItem(
			"medieval_furniture_guest_bedstead",
			"bedstead",
			"a guest-chamber bedstead",
			null,
			"This guest-chamber bedstead is a very large, well-made bedstead built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			48000.0,
			145.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Better-finished bedstead suited to a guest chamber, inn room, or household solar."
		);

		CreateItem(
			"medieval_furniture_hallboard_table",
			"table",
			"a narrow hallboard table",
			null,
			"This narrow hallboard table is a large, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			120.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Six",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Narrow formal serving table for hall vessels, bread, candles, and household display."
		);

		CreateItem(
			"medieval_furniture_hanging_cradle_frame",
			"cradle",
			"a small hanging cradle",
			null,
			"This small hanging cradle is a large, workmanlike cradle built from willow boards. A small sleeping box sits on curved rockers, with low sides and rounded ends. The inner surface is smooth enough for blankets. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			5000.0,
			32.0m,
			true,
			false,
			"willow",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Light infant cradle with visible suspension points; no hanging mechanics are implied."
		);

		CreateItem(
			"medieval_furniture_hearth_bench",
			"bench",
			"a low hearth bench",
			null,
			"This low hearth bench is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			14000.0,
			36.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Double",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Low bench for hearthside sitting, warming, and household work."
		);

		CreateItem(
			"medieval_furniture_heavy_oak_bedstead",
			"bedstead",
			"a heavy oak bedstead",
			null,
			"This heavy oak bedstead is a huge, well-made bedstead built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Huge,
			ItemQuality.Good,
			68000.0,
			220.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Large heavy bedstead with visible posts and a broad surface."
		);

		CreateItem(
			"medieval_furniture_hemp_sitting_mat",
			"mat",
			"a coarse hemp sitting mat",
			null,
			"This coarse hemp sitting mat is a medium-sized, workmanlike mat made from woven hemp. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3000.0,
			10.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Double"
			],
			null,
			null,
			null,
			null,
			"Cheap two-person floor mat for workrooms, kitchens, camps, or common rooms."
		);

		CreateItem(
			"medieval_furniture_high_backed_chair",
			"chair",
			"a high-backed chair",
			null,
			"This high-backed chair is a large, well-made chair built from oak boards. A raised seat stands above shaped supports, with a taller back and more formal proportions than an ordinary chair. The front edge is carefully finished. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			14500.0,
			130.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Tall-backed elite or formal chair with a single seating position."
		);

		CreateItem(
			"medieval_furniture_inn_round_table",
			"table",
			"a sturdy inn table",
			null,
			"This sturdy inn table is a large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			19000.0,
			54.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Practical four-place table suited to a rented chamber, common room, or small dining room."
		);

		CreateItem(
			"medieval_furniture_joined_chair",
			"chair",
			"a joined wooden chair",
			null,
			"This joined wooden chair is a medium-sized, well-made chair built from oak boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8500.0,
			58.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Better-made single chair with joined frame construction and a firm seat."
		);

		CreateItem(
			"medieval_furniture_kitchen_prep_table",
			"table",
			"a scarred kitchen prep table",
			null,
			"This scarred kitchen prep table is a large, workmanlike table built from beech boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			56.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Six",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Hard-wearing preparation table marked by ordinary household cutting, scraping, and washing."
		);

		CreateItem(
			"medieval_furniture_ladderback_chair",
			"chair",
			"a ladder-back chair",
			null,
			"This ladder-back chair is a medium-sized, workmanlike chair built from ash boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6500.0,
			34.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Single chair with a narrow back made from horizontal slats."
		);

		CreateItem(
			"medieval_furniture_lap_writing_board",
			"board",
			"a lap writing board",
			null,
			"This lap writing board is a small, well-made board built from walnut boards. A stable frame carries the weight, with the working surface placed where hands naturally reach it. The edges are worn smooth by household use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			2200.0,
			40.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Small writing board with desk-surface behaviour for compact work."
		);

		CreateItem(
			"medieval_furniture_large_floor_mat",
			"mat",
			"a broad floor sitting mat",
			null,
			"This broad floor sitting mat is a large, workmanlike mat made from coarse jute. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			5200.0,
			28.0m,
			true,
			false,
			"jute",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Quad"
			],
			null,
			null,
			null,
			null,
			"Large four-person floor-seating mat for low tables and communal rooms."
		);

		CreateItem(
			"medieval_furniture_laundry_table",
			"table",
			"a broad laundry table",
			null,
			"This broad laundry table is a large, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			20000.0,
			50.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Six",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Broad utilitarian table for folding cloth, sorting linens, or staging wash basins."
		);

		CreateItem(
			"medieval_furniture_leather_floor_cushion",
			"cushion",
			"a leather floor cushion",
			null,
			"This leather floor cushion is a medium-sized, workmanlike cushion made from worked leather. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2200.0,
			24.0m,
			true,
			false,
			"leather",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Durable single floor cushion covered in leather."
		);

		CreateItem(
			"medieval_furniture_leather_travel_bedroll",
			"bedroll",
			"a leather travel bedroll",
			null,
			"This leather travel bedroll is a medium-sized, workmanlike bedroll made from worked leather. Rolled bedding is tied with straps, with the outer layer wrapped tight around the softer middle. The ends show folded cloth and compressed padding. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5200.0,
			38.0m,
			true,
			false,
			"leather",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Durable travel bedroll for camps, caravans, and poor lodgings."
		);

		CreateItem(
			"medieval_furniture_long_hall_bench",
			"bench",
			"a long hall bench",
			null,
			"This long hall bench is a very large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			22000.0,
			60.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Long three-seat bench intended for halls, refectories, guardrooms, and feasting spaces."
		);

		CreateItem(
			"medieval_furniture_long_hall_table",
			"table",
			"a long hall table",
			null,
			"This long hall table is a very large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			42000.0,
			120.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Twelve",
				"Container_Large_Table"
			],
			null,
			null,
			null,
			null,
			"Twelve-place hall table for communal meals, meetings, and household service."
		);

		CreateItem(
			"medieval_furniture_low_book_stand",
			"stand",
			"a low book stand",
			null,
			"This low book stand is a small, workmanlike stand built from cedar boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			2600.0,
			24.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Low angled reading support for use on the floor, bed, or low table."
		);

		CreateItem(
			"medieval_furniture_low_box_bed",
			"bed",
			"a low box bed",
			null,
			"This low box bed is a very large, well-made bed built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			52000.0,
			160.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Solid low bed frame with boxed-in sides; no enclosed storage component."
		);

		CreateItem(
			"medieval_furniture_low_desk_cushion",
			"cushion",
			"a low desk cushion",
			null,
			"This low desk cushion is a medium-sized, workmanlike cushion made from woven cotton. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1400.0,
			22.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Single floor cushion sized for low desks and writing boards."
		);

		CreateItem(
			"medieval_furniture_low_dining_table",
			"table",
			"a low dining table",
			null,
			"This low dining table is a large, workmanlike table built from cedar boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			14500.0,
			48.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Low table intended for floor seating or cushion seating rather than high chairs."
		);

		CreateItem(
			"medieval_furniture_low_divan_seat",
			"divan",
			"a low padded divan",
			null,
			"This low padded divan is a large, well-made divan made from woven cotton. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			12000.0,
			110.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Triple",
				"Container_Couch_Surface"
			],
			null,
			null,
			null,
			null,
			"Low three-person padded seat usable in chambers, reception rooms, or sleeping spaces."
		);

		CreateItem(
			"medieval_furniture_low_footstool",
			"stool",
			"a low wooden footstool",
			null,
			"This low wooden footstool is a small, workmanlike stool built from pine boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1600.0,
			8.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Low single stool or footstool for chamber seating, hearthside work, or simple household use."
		);

		CreateItem(
			"medieval_furniture_low_platform_seat",
			"platform",
			"a low seating platform",
			null,
			"This low seating platform is a large, workmanlike platform built from pine boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			50.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Quad",
				"Container_Couch_Surface"
			],
			null,
			null,
			null,
			null,
			"Broad low platform for several seated occupants; not treated as a storage container."
		);

		CreateItem(
			"medieval_furniture_low_round_table",
			"table",
			"a low round table",
			null,
			"This low round table is a large, workmanlike table built from cedar boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			15000.0,
			52.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Low circular table for shared dishes, cups, writing boards, or household service."
		);

		CreateItem(
			"medieval_furniture_low_side_table",
			"table",
			"a low side table",
			null,
			"This low side table is a medium-sized, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6200.0,
			20.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Side_Table"
			],
			null,
			null,
			null,
			null,
			"Low portable side table with a simple surface for small household goods."
		);

		CreateItem(
			"medieval_furniture_low_sleeping_platform",
			"platform",
			"a low sleeping platform",
			null,
			"This low sleeping platform is a very large, workmanlike platform built from pine boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			32000.0,
			70.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Low raised platform with a broad bed-surface behaviour."
		);

		CreateItem(
			"medieval_furniture_low_wooden_chair",
			"chair",
			"a low wooden chair",
			null,
			"This low wooden chair is a medium-sized, workmanlike chair built from pine boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5800.0,
			24.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Low single seat suited to floor-level tasks or a cramped chamber."
		);

		CreateItem(
			"medieval_furniture_low_writing_desk",
			"desk",
			"a low writing desk",
			null,
			"This low writing desk is a medium-sized, workmanlike desk built from cedar boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			9000.0,
			55.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Low desk intended for use while seated on a stool, cushion, or floor mat."
		);

		CreateItem(
			"medieval_furniture_map_table",
			"table",
			"a broad map table",
			null,
			"This broad map table is a very large, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			34000.0,
			150.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Eight",
				"Container_Large_Table"
			],
			null,
			null,
			null,
			null,
			"Wide table with enough surface for maps, plans, manuscripts, and measuring tools."
		);

		CreateItem(
			"medieval_furniture_merchant_account_table",
			"table",
			"a merchant's account table",
			null,
			"This merchant's account table is a large, well-made table built from walnut boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			21000.0,
			160.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Well-finished table for account books, tally sticks, seals, coin trays, and written reckoning."
		);

		CreateItem(
			"medieval_furniture_milking_stool",
			"stool",
			"a low milking stool",
			null,
			"This low milking stool is a small, workmanlike stool built from beech boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			7.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Low practical stool for animal work, hearth work, and other crouched tasks."
		);

		CreateItem(
			"medieval_furniture_narrow_cot",
			"cot",
			"a narrow wooden cot",
			null,
			"This narrow wooden cot is a large, workmanlike cot built from pine boards. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			42.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Narrow cot or small bed surface for cramped rooms, servants, guards, or travel lodging."
		);

		CreateItem(
			"medieval_furniture_narrow_side_table",
			"table",
			"a narrow side table",
			null,
			"This narrow side table is a medium-sized, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			7200.0,
			24.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Side_Table"
			],
			null,
			null,
			null,
			null,
			"Small side table for a chamber, hall corner, bedside, or service area."
		);

		CreateItem(
			"medieval_furniture_padded_day_seat",
			"seat",
			"a padded day seat",
			null,
			"This padded day seat is a large, workmanlike seat made from woven wool. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			6500.0,
			60.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Double",
				"Container_Couch_Surface"
			],
			null,
			null,
			null,
			null,
			"Soft two-person day seat with couch-surface behaviour for placed items."
		);

		CreateItem(
			"medieval_furniture_padded_floor_cushion",
			"cushion",
			"a padded floor cushion",
			null,
			"This padded floor cushion is a medium-sized, workmanlike cushion made from woven wool. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			18.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Soft portable single seat represented by chair behaviour."
		);

		CreateItem(
			"medieval_furniture_peddlers_display_table",
			"table",
			"a peddler's display table",
			null,
			"This peddler's display table is a large, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12000.0,
			38.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Light portable table for laying out wares at a stall, inn yard, or market corner."
		);

		CreateItem(
			"medieval_furniture_plain_high_seat",
			"seat",
			"a plain high seat",
			null,
			"This plain high seat is a large, well-made seat built from oak boards. A raised seat stands above shaped supports, with a taller back and more formal proportions than an ordinary chair. The front edge is carefully finished. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			15000.0,
			120.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Formal single high seat suitable for a dais, hall, or household head."
		);

		CreateItem(
			"medieval_furniture_plain_reading_stand",
			"stand",
			"a plain reading stand",
			null,
			"This plain reading stand is a medium-sized, workmanlike stand built from oak boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6200.0,
			38.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Angled stand for books, tablets, account rolls, or scrolls; not tied to any religious use."
		);

		CreateItem(
			"medieval_furniture_plain_single_chair",
			"chair",
			"a plain wooden chair",
			null,
			"This plain wooden chair is a medium-sized, workmanlike chair built from oak boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			7000.0,
			30.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Plain single chair for ordinary household, tavern, or workshop use."
		);

		CreateItem(
			"medieval_furniture_plain_small_table",
			"table",
			"a plain small wooden table",
			null,
			"This plain small wooden table is a large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			14000.0,
			42.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Four-place general household table with ordinary item-on-surface behaviour."
		);

		CreateItem(
			"medieval_furniture_plain_wooden_bed",
			"bed",
			"a plain wooden bed",
			null,
			"This plain wooden bed is a very large, workmanlike bed built from pine boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			36000.0,
			82.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Plain wooden bedstead represented as a bed surface rather than a storage item."
		);

		CreateItem(
			"medieval_furniture_plain_writing_desk",
			"desk",
			"a plain writing desk",
			null,
			"This plain writing desk is a large, workmanlike desk built from ash boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			19000.0,
			96.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Desk surface for writing and reading; drawer storage is a separate container behaviour."
		);

		CreateItem(
			"medieval_furniture_plank_serving_table",
			"table",
			"a plank serving table",
			null,
			"This plank serving table is a large, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			46.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Six",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Plain service table for platters, jugs, bread boards, and kitchen-to-hall serving work."
		);

		CreateItem(
			"medieval_furniture_polished_small_table",
			"table",
			"a polished small table",
			null,
			"This polished small table is a large, well-made table built from walnut boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			15000.0,
			110.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Well-finished small table with a polished surface and careful joinery."
		);

		CreateItem(
			"medieval_furniture_portable_book_stand",
			"stand",
			"a portable book stand",
			null,
			"This portable book stand is a medium-sized, workmanlike stand built from oak boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			34.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Small angled stand for an open book, wax tablet, or tablet board."
		);

		CreateItem(
			"medieval_furniture_portable_cot",
			"cot",
			"a portable folding cot",
			null,
			"This portable folding cot is a large, workmanlike cot built from ash boards. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9500.0,
			50.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Light folding cot with a narrow surface; no storage behaviour."
		);

		CreateItem(
			"medieval_furniture_portable_writing_table",
			"table",
			"a portable writing table",
			null,
			"This portable writing table is a medium-sized, well-made table built from ash boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7500.0,
			80.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Small table surface for writing that can be moved between rooms or taken on a journey."
		);

		CreateItem(
			"medieval_furniture_posted_bed_frame",
			"bed",
			"a posted wooden bed",
			null,
			"This posted wooden bed is a huge, well-made bed built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Huge,
			ItemQuality.Good,
			70000.0,
			240.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Large posted bed frame suited to elite chambers and curtain hangings, though no hanging behaviour is implied."
		);

		CreateItem(
			"medieval_furniture_raised_sleeping_platform",
			"platform",
			"a raised sleeping platform",
			null,
			"This raised sleeping platform is a very large, well-made platform built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			46000.0,
			130.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Broad raised platform suitable for bedding, mats, and chamber sleeping arrangements."
		);

		CreateItem(
			"medieval_furniture_record_chamber_table",
			"table",
			"a record-chamber table",
			null,
			"This record-chamber table is a very large, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			32000.0,
			150.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Eight",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Large formal table for records, charts, ledgers, and sealed documents."
		);

		CreateItem(
			"medieval_furniture_round_eating_table",
			"table",
			"a round wooden eating table",
			null,
			"This round wooden eating table is a large, workmanlike table built from beech boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			15500.0,
			48.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Round four-place eating table for domestic rooms, taverns, and small halls."
		);

		CreateItem(
			"medieval_furniture_round_floor_cushion",
			"cushion",
			"a round floor cushion",
			null,
			"This round floor cushion is a medium-sized, workmanlike cushion made from woven wool. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			16.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Round padded cushion for floor seating and low tables."
		);

		CreateItem(
			"medieval_furniture_round_stool",
			"stool",
			"a round wooden stool",
			null,
			"This round wooden stool is a medium-sized, workmanlike stool built from elm boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3400.0,
			16.0m,
			true,
			false,
			"elm",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Compact round stool with a single seating position."
		);

		CreateItem(
			"medieval_furniture_school_desk",
			"desk",
			"a simple school desk",
			null,
			"This simple school desk is a large, workmanlike desk built from pine boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12000.0,
			45.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Plain writing surface for teaching, copying, lessons, or apprentices' work."
		);

		CreateItem(
			"medieval_furniture_scribe_chair",
			"chair",
			"a scribe's wooden chair",
			null,
			"This scribe's wooden chair is a medium-sized, workmanlike chair built from beech boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6200.0,
			42.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Plain single chair suited to long desk work and copying."
		);

		CreateItem(
			"medieval_furniture_scribes_desk",
			"desk",
			"a scribe's writing desk",
			null,
			"This scribe's writing desk is a large, well-made desk built from beech boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			18000.0,
			125.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Orderly writing desk with space for papers, ink vessels, tablets, and writing tools."
		);

		CreateItem(
			"medieval_furniture_secretarys_desk",
			"desk",
			"a secretary's writing desk",
			null,
			"This secretary's writing desk is a large, well-made desk built from walnut boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			22000.0,
			170.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Fine writing desk for a private chamber, chancery, solar, or household office."
		);

		CreateItem(
			"medieval_furniture_servants_pallet_frame",
			"frame",
			"a servant's pallet frame",
			null,
			"This servant's pallet frame is a large, workmanlike frame built from pine boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			16000.0,
			28.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Plain low frame for a narrow pallet or sleeping mat."
		);

		CreateItem(
			"medieval_furniture_settle_bench",
			"settle",
			"a high-backed settle",
			null,
			"This high-backed settle is a large, well-made settle built from oak boards. A long seat is set below a high back, with enclosed ends that make it feel sheltered. The sitting edge is rounded from use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			140.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Three-seat high-backed bench for a hall, solar, chamber, or tavern corner."
		);

		CreateItem(
			"medieval_furniture_shallow_counter_table",
			"table",
			"a shallow counter table",
			null,
			"This shallow counter table is a large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			28000.0,
			78.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Six",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"High practical table for counters, workshops, store rooms, or household service."
		);

		CreateItem(
			"medieval_furniture_ship_bunk",
			"bunk",
			"a narrow wooden bunk",
			null,
			"This narrow wooden bunk is a large, workmanlike bunk built from pine boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			17000.0,
			38.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Narrow bunk-like sleeping surface for ships, barracks, towers, or cramped lodgings."
		);

		CreateItem(
			"medieval_furniture_simple_cradle",
			"cradle",
			"a simple wooden cradle",
			null,
			"This simple wooden cradle is a large, workmanlike cradle built from pine boards. A small sleeping box sits on curved rockers, with low sides and rounded ends. The inner surface is smooth enough for blankets. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8000.0,
			30.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Small rocking-style infant bed represented as a narrow cot surface."
		);

		CreateItem(
			"medieval_furniture_simple_hall_bench",
			"bench",
			"a simple hall bench",
			null,
			"This simple hall bench is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			15000.0,
			38.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Three-seat bench with a surface that can support placed items."
		);

		CreateItem(
			"medieval_furniture_simple_rope_bed",
			"bed",
			"a simple rope bed",
			null,
			"This simple rope bed is a very large, workmanlike bed built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			42000.0,
			100.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Wooden bed frame with a rope-slung surface; uses bed-surface container behaviour."
		);

		CreateItem(
			"medieval_furniture_simple_sideboard_table",
			"table",
			"a plain sideboard table",
			null,
			"This plain sideboard table is a large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			26000.0,
			85.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Six",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Long open table used like a sideboard surface without enclosed storage behaviour."
		);

		CreateItem(
			"medieval_furniture_simple_stool",
			"stool",
			"a simple wooden stool",
			null,
			"This simple wooden stool is a small, workmanlike stool built from pine boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			2200.0,
			8.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Small single-person stool for household, kitchen, workshop, or stable use."
		);

		CreateItem(
			"medieval_furniture_simple_writing_bench",
			"bench",
			"a narrow writing bench",
			null,
			"This narrow writing bench is a large, workmanlike bench built from pine boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			30.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Double",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Two-seat bench useful with writing tables, teaching tables, or work desks."
		);

		CreateItem(
			"medieval_furniture_sloped_writing_desk",
			"desk",
			"a sloped writing desk",
			null,
			"This sloped writing desk is a large, well-made desk built from walnut boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			21000.0,
			140.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Writing desk with a visible sloped top and a surface suited to manuscripts or tablets."
		);

		CreateItem(
			"medieval_furniture_square_work_table",
			"table",
			"a square wooden work table",
			null,
			"This square wooden work table is a large, workmanlike table built from ash boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			17000.0,
			52.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Sturdy four-place work and preparation table with a broad flat surface."
		);

		CreateItem(
			"medieval_furniture_stewards_table",
			"table",
			"a steward's service table",
			null,
			"This steward's service table is a large, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			23000.0,
			110.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Six",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Well-made service and account table for household officers, stewards, and clerks."
		);

		CreateItem(
			"medieval_furniture_stout_work_chair",
			"chair",
			"a stout work chair",
			null,
			"This stout work chair is a medium-sized, workmanlike chair built from beech boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			8000.0,
			32.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Heavy practical chair for workshops, kitchens, guardrooms, and service spaces."
		);

		CreateItem(
			"medieval_furniture_stuffed_linen_pallet",
			"pallet",
			"a stuffed linen pallet",
			null,
			"This stuffed linen pallet is a large, workmanlike pallet made from woven linen. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			6500.0,
			20.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Soft stuffed pallet for sleeping on floors, platforms, or simple frames."
		);

		CreateItem(
			"medieval_furniture_tall_desk_stool",
			"stool",
			"a tall desk stool",
			null,
			"This tall desk stool is a medium-sized, workmanlike stool built from oak boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			22.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Taller single stool for high tables, counters, and desk surfaces."
		);

		CreateItem(
			"medieval_furniture_tall_reading_stand",
			"stand",
			"a tall reading stand",
			null,
			"This tall reading stand is a large, well-made stand built from ash boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			9000.0,
			75.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Tall free-standing reading surface for proclamations, lessons, accounts, or recitation."
		);

		CreateItem(
			"medieval_furniture_tall_work_table",
			"table",
			"a tall work table",
			null,
			"This tall work table is a large, workmanlike table built from ash boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			25000.0,
			70.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Raised work surface for standing tasks, measuring, cutting, sorting, or laying out tools."
		);

		CreateItem(
			"medieval_furniture_tally_table_desk",
			"desk",
			"a tally-marked desk",
			null,
			"This tally-marked desk is a large, workmanlike desk built from beech boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			16000.0,
			70.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Desk top scarred with tally-like marks and ordinary clerkly wear."
		);

		CreateItem(
			"medieval_furniture_tavern_common_table",
			"table",
			"a stained tavern table",
			null,
			"This stained tavern table is a large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			21000.0,
			48.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Six",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Rough six-place common table for inns, alehouses, guardrooms, and servants' halls."
		);

		CreateItem(
			"medieval_furniture_thick_sleeping_mat",
			"mat",
			"a thick sleeping mat",
			null,
			"This thick sleeping mat is a large, workmanlike mat made from coarse jute. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			5400.0,
			18.0m,
			true,
			false,
			"jute",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Coarse thick sleeping mat for simple household or camp sleeping arrangements."
		);

		CreateItem(
			"medieval_furniture_three_legged_stool",
			"stool",
			"a three-legged stool",
			null,
			"This three-legged stool is a small, workmanlike stool built from ash boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			2400.0,
			10.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Small three-legged stool intended to sit firmly on uneven floors."
		);

		CreateItem(
			"medieval_furniture_three_legged_table",
			"table",
			"a three-legged small table",
			null,
			"This three-legged small table is a medium-sized, workmanlike table built from elm boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6800.0,
			22.0m,
			true,
			false,
			"elm",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Compact small table with a three-legged stance suitable for uneven floors."
		);

		CreateItem(
			"medieval_furniture_tilted_drafting_table",
			"table",
			"a tilted drafting table",
			null,
			"This tilted drafting table is a large, well-made table built from ash boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			21000.0,
			130.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Sloped table for plans, patterns, diagrams, maps, and careful writing work."
		);

		CreateItem(
			"medieval_furniture_tray_stand_table",
			"stand",
			"a folding tray stand",
			null,
			"This folding tray stand is a medium-sized, workmanlike stand built from ash boards. A rectangular top rests on hinged legs that fold beneath it. The joints are pinned and rubbed bright where the frame moves. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5000.0,
			28.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Light stand-like table for holding a tray, ewer, platter, or small household vessel."
		);

		CreateItem(
			"medieval_furniture_trestle_bench",
			"bench",
			"a trestle hall bench",
			null,
			"This trestle hall bench is a large, workmanlike bench built from pine boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			13000.0,
			34.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Portable-looking trestle bench with a long plank seat and simple supports."
		);

		CreateItem(
			"medieval_furniture_trestle_dining_table",
			"table",
			"a trestle dining table",
			null,
			"This trestle dining table is a very large, workmanlike table built from oak boards. A long board top rests across two trestle supports, with simple pegs holding the parts in line. The underside shows scuffs from assembly and storage. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			30000.0,
			82.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Eight",
				"Container_Large_Table"
			],
			null,
			null,
			null,
			null,
			"Eight-place trestle dining table with a removable-looking plank top and broad surface."
		);

		CreateItem(
			"medieval_furniture_wash_table",
			"table",
			"a plain wash table",
			null,
			"This plain wash table is a large, workmanlike table built from elm boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18500.0,
			48.0m,
			true,
			false,
			"elm",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Household wash table sized for basins, ewers, cloths, and other washing goods."
		);

		CreateItem(
			"medieval_furniture_wicker_cradle",
			"cradle",
			"a woven willow cradle",
			null,
			"This woven willow cradle is a large, workmanlike cradle built from willow boards. A small sleeping box sits on curved rockers, with low sides and rounded ends. The inner surface is smooth enough for blankets. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			4500.0,
			26.0m,
			true,
			false,
			"willow",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Light woven cradle with a narrow surface suitable for bedding."
		);

		CreateItem(
			"medieval_furniture_wide_bed_frame",
			"bed",
			"a wide wooden bed",
			null,
			"This wide wooden bed is a huge, well-made bed built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Huge,
			ItemQuality.Good,
			64000.0,
			180.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Wide bed frame with a large surface for bedding and placed objects."
		);

		CreateItem(
			"medieval_furniture_window_bench",
			"bench",
			"a narrow window bench",
			null,
			"This narrow window bench is a large, workmanlike bench built from ash boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12000.0,
			42.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Double",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Narrow two-seat bench intended for a wall, window bay, or chamber side."
		);

		CreateItem(
			"medieval_furniture_wooden_daybed_seat",
			"daybed",
			"a wooden daybed seat",
			null,
			"This wooden daybed seat is a large, well-made daybed built from oak boards. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			130.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Triple",
				"Container_Couch_Surface"
			],
			null,
			null,
			null,
			null,
			"Raised three-person daybed-like seat with a couch surface, distinct from enclosed bed storage."
		);

		CreateItem(
			"medieval_furniture_wool_bedding_roll",
			"bedroll",
			"a rolled wool bedroll",
			null,
			"This rolled wool bedroll is a medium-sized, workmanlike bedroll made from woven wool. Rolled bedding is tied with straps, with the outer layer wrapped tight around the softer middle. The ends show folded cloth and compressed padding. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Portable bedding roll represented as a narrow cot-like surface when set down."
		);

		CreateItem(
			"medieval_furniture_wool_sitting_mat",
			"mat",
			"a wool sitting mat",
			null,
			"This wool sitting mat is a medium-sized, workmanlike mat made from woven wool. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2500.0,
			18.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Double"
			],
			null,
			null,
			null,
			null,
			"Soft two-person floor-seating mat using chair behaviour."
		);

		CreateItem(
			"medieval_furniture_wool_sleeping_pallet",
			"pallet",
			"a wool sleeping pallet",
			null,
			"This wool sleeping pallet is a large, workmanlike pallet made from woven wool. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			7600.0,
			34.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Warmer padded sleeping pallet with a narrow surface profile."
		);

		CreateItem(
			"medieval_furniture_work_stool",
			"stool",
			"a sturdy work stool",
			null,
			"This sturdy work stool is a medium-sized, workmanlike stool built from oak boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3600.0,
			14.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Plain robust stool for workshops, kitchens, barns, and servants' rooms."
		);

		CreateItem(
			"medieval_furniture_northern_box_bed_frame",
			"bed",
			"a boxed timber bed",
			null,
			"This boxed timber bed is a very large, well-made bed built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			56000.0,
			170.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Heavy timber bed with boxed sides and a broad surface; no storage behaviour."
		);

		CreateItem(
			"medieval_furniture_northern_carved_chest_table",
			"table",
			"a carved end table",
			null,
			"This carved end table is a medium-sized, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			9000.0,
			70.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Side_Table"
			],
			null,
			null,
			null,
			null,
			"Carved side table for a high seat, chamber, or hall corner; no storage behaviour."
		);

		CreateItem(
			"medieval_furniture_northern_carved_high_seat",
			"seat",
			"a carved northern high seat",
			null,
			"This carved northern high seat is a large, well-made seat built from oak boards. A raised seat stands above shaped supports, with a taller back and more formal proportions than an ordinary chair. The front edge is carefully finished. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			18000.0,
			160.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Formal carved high seat for a northern hall or dais; public wording remains broad."
		);

		CreateItem(
			"medieval_furniture_northern_fur_sleeping_bench",
			"bench",
			"a fur-covered sleeping bench",
			null,
			"This fur-covered sleeping bench is a large, well-made bench made from dressed fur. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Large,
			ItemQuality.Good,
			12000.0,
			95.0m,
			true,
			false,
			"fur",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Triple",
				"Container_Couch_Surface"
			],
			null,
			null,
			null,
			null,
			"Long padded bench-seat with fur covering and couch-surface behaviour."
		);

		CreateItem(
			"medieval_furniture_northern_heavy_workbench",
			"table",
			"a heavy timber workbench",
			null,
			"This heavy timber workbench is a large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			32000.0,
			75.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Heavy bench-height work table for timber halls and workshops."
		);

		CreateItem(
			"medieval_furniture_northern_long_fire_bench",
			"bench",
			"a long fireside bench",
			null,
			"This long fireside bench is a very large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			23000.0,
			58.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Heavy bench for a hearth wall, hall fire, or long-house style interior."
		);

		CreateItem(
			"medieval_furniture_northern_narrow_bunk",
			"bunk",
			"a narrow plank bunk",
			null,
			"This narrow plank bunk is a large, workmanlike bunk built from pine boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			17000.0,
			35.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Narrow plank sleeping surface for ships, guardrooms, or crowded household spaces."
		);

		CreateItem(
			"medieval_furniture_northern_plank_hall_bench",
			"bench",
			"a plank-built hall bench",
			null,
			"This plank-built hall bench is a large, workmanlike bench built from pine boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			14000.0,
			34.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Long simple bench with plank construction suited to timber halls."
		);

		CreateItem(
			"medieval_furniture_northern_shipboard_table",
			"table",
			"a pegged shipboard table",
			null,
			"This pegged shipboard table is a large, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			14000.0,
			48.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Compact pegged table suited to ships, camps, and small timber rooms."
		);

		CreateItem(
			"medieval_furniture_northern_trestle_feast_table",
			"table",
			"a long plank feast table",
			null,
			"This long plank feast table is a huge, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Huge,
			ItemQuality.Good,
			62000.0,
			210.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Twenty",
				"Container_Large_Table"
			],
			null,
			null,
			null,
			null,
			"Large plank table for a northern hall, feast, or assembly."
		);

		CreateItem(
			"medieval_furniture_northern_tripod_stool",
			"stool",
			"a carved tripod stool",
			null,
			"This carved tripod stool is a small, workmanlike stool built from ash boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			2400.0,
			18.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Carved three-legged stool suitable for hall, workshop, or hearth use."
		);

		CreateItem(
			"medieval_furniture_northern_wool_sleeping_roll",
			"bedroll",
			"a checked wool bedroll",
			null,
			"This checked wool bedroll is a medium-sized, workmanlike bedroll made from woven wool. Rolled bedding is tied with straps, with the outer layer wrapped tight around the softer middle. The ends show folded cloth and compressed padding. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4400.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Warm wool bedding roll for northern travel and hall sleeping."
		);

		CreateItem(
			"medieval_furniture_western_accounting_desk",
			"desk",
			"a western account desk",
			null,
			"This western account desk is a large, well-made desk built from walnut boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			23000.0,
			165.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Good-quality desk for accounts, letters, seals, and household or guild paperwork."
		);

		CreateItem(
			"medieval_furniture_western_canopy_bed_frame",
			"bed",
			"a posted canopy bed",
			null,
			"This posted canopy bed is a huge, well-made bed built from oak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Huge,
			ItemQuality.Good,
			76000.0,
			280.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Large posted bed intended for hangings or curtains; no curtain mechanics are implied."
		);

		CreateItem(
			"medieval_furniture_western_carved_settle",
			"settle",
			"a carved hall settle",
			null,
			"This carved hall settle is a large, well-made settle built from oak boards. A long seat is set below a high back, with enclosed ends that make it feel sheltered. The sitting edge is rounded from use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			28000.0,
			155.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"High-backed carved three-seat settle for halls, solars, and chambers."
		);

		CreateItem(
			"medieval_furniture_western_joined_armchair",
			"chair",
			"a joined armchair",
			null,
			"This joined armchair is a large, well-made chair built from walnut boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			13500.0,
			140.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Fine single chair with arms and close-fitted joinery."
		);

		CreateItem(
			"medieval_furniture_western_joined_dining_table",
			"table",
			"a joined dining table",
			null,
			"This joined dining table is a large, well-made table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			120.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Six",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Well-joined six-place dining table for townhouses, manors, and guild halls."
		);

		CreateItem(
			"medieval_furniture_western_padded_bench",
			"bench",
			"a linen-padded bench",
			null,
			"This linen-padded bench is a large, well-made bench made from woven linen. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			11500.0,
			80.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Bench_Double",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Two-seat padded bench for a chamber, solar, or refined hall corner."
		);

		CreateItem(
			"medieval_furniture_western_refectory_table",
			"table",
			"a refectory-style table",
			null,
			"This refectory-style table is a very large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			42000.0,
			130.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Twelve",
				"Container_Large_Table"
			],
			null,
			null,
			null,
			null,
			"Long communal table for halls, schools, dormitories, and institutional dining."
		);

		CreateItem(
			"medieval_furniture_western_sloped_book_stand",
			"stand",
			"a sloped book stand",
			null,
			"This sloped book stand is a medium-sized, workmanlike stand built from oak boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5200.0,
			45.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Angled wooden support for books, ledgers, tablets, and papers."
		);

		CreateItem(
			"medieval_furniture_western_tall_stool",
			"stool",
			"a tall joined stool",
			null,
			"This tall joined stool is a medium-sized, well-made stool built from oak boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			4200.0,
			35.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Well-joined tall stool for counters, desks, and high tables."
		);

		CreateItem(
			"medieval_furniture_western_trestle_bench_pair",
			"bench",
			"a trestle dining bench",
			null,
			"This trestle dining bench is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			14000.0,
			44.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Long bench suited to a trestle table or formal hall service."
		);

		CreateItem(
			"medieval_furniture_western_truckle_bed",
			"bed",
			"a low truckle bed",
			null,
			"This low truckle bed is a large, workmanlike bed built from pine boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			60.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Low narrow bed surface that can sit beneath or beside a larger bed."
		);

		CreateItem(
			"medieval_furniture_western_wainscot_chair",
			"chair",
			"a panel-backed chair",
			null,
			"This panel-backed chair is a large, well-made chair built from oak boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			13000.0,
			130.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Single chair with a broad panel-like back and formal presence."
		);

		CreateItem(
			"medieval_furniture_mediterranean_basin_stand",
			"stand",
			"a basin-height table stand",
			null,
			"This basin-height table stand is a medium-sized, workmanlike stand built from olive wood. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			7000.0,
			42.0m,
			true,
			false,
			"olive wood",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Small stand-like table for basins, ewers, lamps, or chamber washing goods."
		);

		CreateItem(
			"medieval_furniture_mediterranean_bench_couch",
			"bench",
			"a low couch-bench",
			null,
			"This low couch-bench is a large, well-made bench built from cypress boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			19000.0,
			105.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Double",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Two-seat low bench with couch-like proportions and refined joinery."
		);

		CreateItem(
			"medieval_furniture_mediterranean_bronze_fitted_stool",
			"stool",
			"a bronze-fitted stool",
			null,
			"This bronze-fitted stool is a medium-sized, well-made stool built from walnut boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			4200.0,
			55.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Single stool with visible bronze fittings; primary material remains wood."
		);

		CreateItem(
			"medieval_furniture_mediterranean_courtyard_bench",
			"bench",
			"a courtyard wooden bench",
			null,
			"This courtyard wooden bench is a large, workmanlike bench built from cypress boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			13000.0,
			42.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Plain three-seat bench suited to courtyards, shaded rooms, and outdoor household spaces."
		);

		CreateItem(
			"medieval_furniture_mediterranean_cypress_table",
			"table",
			"a cypress dining table",
			null,
			"This cypress dining table is a large, well-made table built from cypress boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			21000.0,
			95.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Polished cypress table for warm-climate dining rooms, courtyards, or chambers."
		);

		CreateItem(
			"medieval_furniture_mediterranean_daybed",
			"daybed",
			"a cypress daybed",
			null,
			"This cypress daybed is a very large, well-made daybed built from cypress boards. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			32000.0,
			150.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Low daybed or sleeping frame for warm chambers and covered verandas."
		);

		CreateItem(
			"medieval_furniture_mediterranean_folded_chair",
			"chair",
			"a folding frame chair",
			null,
			"This folding frame chair is a medium-sized, well-made chair built from walnut boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			5600.0,
			75.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Portable folding-frame single chair for travel, halls, or formal chamber use."
		);

		CreateItem(
			"medieval_furniture_mediterranean_low_couch",
			"couch",
			"a low wooden dining couch",
			null,
			"This low wooden dining couch is a large, well-made couch built from olive wood. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			25000.0,
			145.0m,
			true,
			false,
			"olive wood",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Triple",
				"Container_Couch_Surface"
			],
			null,
			null,
			null,
			null,
			"Low couch-like seat for reclining, day use, or chamber seating."
		);

		CreateItem(
			"medieval_furniture_mediterranean_low_guest_bed",
			"bed",
			"a low guest bed",
			null,
			"This low guest bed is a very large, workmanlike bed built from olive wood. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			34000.0,
			110.0m,
			true,
			false,
			"olive wood",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Low bed surface for guest chambers, urban houses, and courtyard dwellings."
		);

		CreateItem(
			"medieval_furniture_mediterranean_tile_top_table",
			"table",
			"a tiled-top table",
			null,
			"This tiled-top table is a large, well-made table formed from ceramic. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			30000.0,
			150.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Sturdy table with a ceramic primary material for a tiled or inset top."
		);

		CreateItem(
			"medieval_furniture_mediterranean_tripod_table",
			"table",
			"a small tripod table",
			null,
			"This small tripod table is a medium-sized, well-made table built from olive wood. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			6200.0,
			50.0m,
			true,
			false,
			"olive wood",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Small three-legged table for cups, lamps, basin work, or chamber service."
		);

		CreateItem(
			"medieval_furniture_mediterranean_writing_stand",
			"stand",
			"a cypress writing stand",
			null,
			"This cypress writing stand is a medium-sized, well-made stand built from cypress boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			5200.0,
			70.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Fine angled writing or reading stand with a warm-climate wood profile."
		);

		CreateItem(
			"medieval_furniture_islamicate_bolster_daybed",
			"daybed",
			"a bolstered daybed",
			null,
			"This bolstered daybed is a very large, well-made daybed made from woven silk. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			24000.0,
			220.0m,
			true,
			false,
			"silk",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Triple",
				"Container_Couch_Surface"
			],
			null,
			null,
			null,
			null,
			"Fine daybed-like seat with bolstered textile surface and couch behaviour."
		);

		CreateItem(
			"medieval_furniture_islamicate_carpet_platform",
			"platform",
			"a padded sitting platform",
			null,
			"This padded sitting platform is a large, well-made platform made from woven wool. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			9000.0,
			95.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Quad",
				"Container_Couch_Surface"
			],
			null,
			null,
			null,
			null,
			"Soft four-person sitting platform distinct from decorative carpets and wall hangings."
		);

		CreateItem(
			"medieval_furniture_islamicate_carved_stool",
			"stool",
			"a carved low stool",
			null,
			"This carved low stool is a medium-sized, well-made stool built from cedar boards. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3500.0,
			38.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Low single stool suited to reception rooms, baths, and chambers."
		);

		CreateItem(
			"medieval_furniture_islamicate_cushion_set",
			"cushion",
			"a broad cushion seat",
			null,
			"This broad cushion seat is a large, well-made cushion made from woven cotton. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			4200.0,
			55.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Double"
			],
			null,
			null,
			null,
			null,
			"Broad two-person cushion for reception rooms, chambers, and low table seating."
		);

		CreateItem(
			"medieval_furniture_islamicate_divan_platform",
			"divan",
			"a padded divan platform",
			null,
			"This padded divan platform is a very large, well-made divan made from woven cotton. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			22000.0,
			160.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Quad",
				"Container_Couch_Surface"
			],
			null,
			null,
			null,
			null,
			"Broad padded seating platform for reception rooms and household chambers."
		);

		CreateItem(
			"medieval_furniture_islamicate_inlaid_side_table",
			"table",
			"an inlaid side table",
			null,
			"This inlaid side table is a medium-sized, well-made table built from sandalwood. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			6500.0,
			120.0m,
			true,
			false,
			"sandalwood",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Side_Table"
			],
			null,
			null,
			null,
			null,
			"Fine small side table with decorative inlay described without special mechanics."
		);

		CreateItem(
			"medieval_furniture_islamicate_low_bedstead",
			"bedstead",
			"a low cedar bedstead",
			null,
			"This low cedar bedstead is a very large, well-made bedstead built from cedar boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			36000.0,
			145.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Low polished bedstead suited to urban chambers and curtained sleeping spaces."
		);

		CreateItem(
			"medieval_furniture_islamicate_low_feast_table",
			"table",
			"a low feast table",
			null,
			"This low feast table is a large, well-made table built from cedar boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			16000.0,
			85.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Low table for shared dishes, cups, and floor-cushion seating."
		);

		CreateItem(
			"medieval_furniture_islamicate_low_writing_desk",
			"desk",
			"a low cedar writing desk",
			null,
			"This low cedar writing desk is a medium-sized, well-made desk built from cedar boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8000.0,
			80.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Low writing desk suitable for cushion seating and manuscript work."
		);

		CreateItem(
			"medieval_furniture_islamicate_merchant_desk",
			"desk",
			"a merchant's low desk",
			null,
			"This merchant's low desk is a large, well-made desk built from walnut boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			17000.0,
			140.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Low account desk for ledgers, coins, seals, letters, and commercial paperwork."
		);

		CreateItem(
			"medieval_furniture_islamicate_reading_stand",
			"stand",
			"a folding reading stand",
			null,
			"This folding reading stand is a small, well-made stand built from cedar boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			2600.0,
			48.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Folding reading stand for books, tablets, or letters without religious specificity."
		);

		CreateItem(
			"medieval_furniture_islamicate_screened_couch",
			"couch",
			"a low reception couch",
			null,
			"This low reception couch is a large, well-made couch built from walnut boards. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			21000.0,
			130.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Triple",
				"Container_Couch_Surface"
			],
			null,
			null,
			null,
			null,
			"Three-person couch-like reception seat with a usable surface."
		);

		CreateItem(
			"medieval_furniture_south_asian_bamboo_stool",
			"stool",
			"a woven bamboo stool",
			null,
			"This woven bamboo stool is a small, workmanlike stool built from split bamboo. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1600.0,
			12.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Light single-person stool of woven or lashed bamboo."
		);

		CreateItem(
			"medieval_furniture_south_asian_bolster_seat",
			"seat",
			"a bolstered floor seat",
			null,
			"This bolstered floor seat is a large, workmanlike seat made from woven cotton. A raised seat stands above shaped supports, with a taller back and more formal proportions than an ordinary chair. The front edge is carefully finished. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			4800.0,
			45.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Double"
			],
			null,
			null,
			null,
			null,
			"Two-person soft floor seat with bolsters and no storage behaviour."
		);

		CreateItem(
			"medieval_furniture_south_asian_carved_swing_bench",
			"bench",
			"a carved swing-bench seat",
			null,
			"This carved swing-bench seat is a large, well-made bench built from teak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			160.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Double",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Two-seat carved bench-like seat; no swinging mechanics are implied."
		);

		CreateItem(
			"medieval_furniture_south_asian_charpoy_bed",
			"bed",
			"a rope-woven charpoy bed",
			null,
			"This rope-woven charpoy bed is a very large, workmanlike bed built from teak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			24000.0,
			78.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Rope-woven bed frame suited to warm-climate household and courtyard sleeping."
		);

		CreateItem(
			"medieval_furniture_south_asian_chowki_table",
			"table",
			"a low chowki table",
			null,
			"This low chowki table is a medium-sized, well-made table built from teak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			6500.0,
			60.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Low square table or platform for household service, seating-adjacent work, or display."
		);

		CreateItem(
			"medieval_furniture_south_asian_daybed",
			"daybed",
			"a teak daybed",
			null,
			"This teak daybed is a very large, well-made daybed built from teak boards. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			28000.0,
			130.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Low daybed surface for courtyards, shaded rooms, and warm-climate chambers."
		);

		CreateItem(
			"medieval_furniture_south_asian_floor_cushion",
			"cushion",
			"a cotton floor cushion",
			null,
			"This cotton floor cushion is a medium-sized, workmanlike cushion made from woven cotton. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1500.0,
			18.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Single cotton cushion for floor seating and low tables."
		);

		CreateItem(
			"medieval_furniture_south_asian_low_cot",
			"cot",
			"a low rope-woven cot",
			null,
			"This low rope-woven cot is a large, workmanlike cot built from split bamboo. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			34.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Light low cot with a woven surface for sleep, rest, or travel."
		);

		CreateItem(
			"medieval_furniture_south_asian_low_platform_bench",
			"platform",
			"a low wooden platform",
			null,
			"This low wooden platform is a large, workmanlike platform built from teak boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			20000.0,
			70.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Quad",
				"Container_Couch_Surface"
			],
			null,
			null,
			null,
			null,
			"Low platform for sitting, resting, or placing light household goods."
		);

		CreateItem(
			"medieval_furniture_south_asian_low_writing_board",
			"board",
			"a low writing board",
			null,
			"This low writing board is a small, workmanlike board built from teak boards. A stable frame carries the weight, with the working surface placed where hands naturally reach it. The edges are worn smooth by household use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1900.0,
			28.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Small low writing board for accounts, letters, or lessons."
		);

		CreateItem(
			"medieval_furniture_south_asian_merchant_desk",
			"desk",
			"a low merchant's desk",
			null,
			"This low merchant's desk is a medium-sized, well-made desk built from teak boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8000.0,
			85.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Low desk surface for coin trays, account papers, seals, and trade records."
		);

		CreateItem(
			"medieval_furniture_south_asian_teak_chair",
			"chair",
			"a carved teak chair",
			null,
			"This carved teak chair is a medium-sized, well-made chair built from teak boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8500.0,
			90.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Single carved chair with warm-climate hardwood construction."
		);

		CreateItem(
			"medieval_furniture_east_asian_bamboo_bench",
			"bench",
			"a bamboo two-seat bench",
			null,
			"This bamboo two-seat bench is a large, workmanlike bench built from split bamboo. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8000.0,
			32.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Double",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Light two-seat bench suitable for chambers, verandas, and courtyards."
		);

		CreateItem(
			"medieval_furniture_east_asian_bamboo_stool",
			"stool",
			"a bamboo square stool",
			null,
			"This bamboo square stool is a medium-sized, workmanlike stool built from split bamboo. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2200.0,
			18.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Light square stool suitable for low desks or household use."
		);

		CreateItem(
			"medieval_furniture_east_asian_cot_frame",
			"cot",
			"a narrow bamboo cot",
			null,
			"This narrow bamboo cot is a large, workmanlike cot built from split bamboo. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8000.0,
			34.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Light narrow cot frame with a simple surface."
		);

		CreateItem(
			"medieval_furniture_east_asian_daybed_couch",
			"daybed",
			"a wooden daybed couch",
			null,
			"This wooden daybed couch is a very large, well-made daybed built from cedar boards. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			26000.0,
			145.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Triple",
				"Container_Couch_Surface"
			],
			null,
			null,
			null,
			null,
			"Three-person daybed-like couch for study, reception, or rest."
		);

		CreateItem(
			"medieval_furniture_east_asian_floor_cushion",
			"cushion",
			"a square floor cushion",
			null,
			"This square floor cushion is a medium-sized, workmanlike cushion made from woven cotton. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1300.0,
			18.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Single square cushion for floor seating beside low tables."
		);

		CreateItem(
			"medieval_furniture_east_asian_low_dining_table",
			"table",
			"a low lacquered table",
			null,
			"This low lacquered table is a large, well-made table built from split bamboo. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			12000.0,
			85.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Low table for floor seating, tea service, writing, or shared dishes."
		);

		CreateItem(
			"medieval_furniture_east_asian_official_desk",
			"desk",
			"a polished official desk",
			null,
			"This polished official desk is a large, well-made desk built from walnut boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			20000.0,
			170.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Formal desk surface for documents, seals, brushes, and books."
		);

		CreateItem(
			"medieval_furniture_east_asian_platform_bed",
			"bed",
			"a low platform bed",
			null,
			"This low platform bed is a very large, well-made bed built from cedar boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Good,
			32000.0,
			140.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bed_Surface"
			],
			null,
			null,
			null,
			null,
			"Low platform bed surface for mattresses, bedding, or sleeping mats."
		);

		CreateItem(
			"medieval_furniture_east_asian_reed_like_mat",
			"mat",
			"a woven bamboo sitting mat",
			null,
			"This woven bamboo sitting mat is a large, workmanlike mat built from split bamboo. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			3800.0,
			28.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Quad"
			],
			null,
			null,
			null,
			null,
			"Large four-person floor seating mat represented by chair behaviour."
		);

		CreateItem(
			"medieval_furniture_east_asian_round_stool",
			"stool",
			"a round drum stool",
			null,
			"This round drum stool is a medium-sized, well-made stool formed from ceramic. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8500.0,
			65.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Round ceramic single seat for refined rooms, courtyards, or garden chambers."
		);

		CreateItem(
			"medieval_furniture_east_asian_scholar_desk",
			"desk",
			"a low scholar's desk",
			null,
			"This low scholar's desk is a large, well-made desk built from walnut boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			15000.0,
			130.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Low desk for writing, brush work, books, and formal study."
		);

		CreateItem(
			"medieval_furniture_east_asian_silk_cushion",
			"cushion",
			"a silk floor cushion",
			null,
			"This silk floor cushion is a medium-sized, well-made cushion made from woven silk. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Good,
			900.0,
			65.0m,
			true,
			false,
			"silk",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Fine single cushion for reception rooms, study spaces, and low tables."
		);

		CreateItem(
			"medieval_furniture_east_asian_tea_table",
			"table",
			"a small tea table",
			null,
			"This small tea table is a medium-sized, well-made table built from split bamboo. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			5200.0,
			52.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Small low table for cups, bowls, writing tools, or domestic tea service."
		);

		CreateItem(
			"medieval_furniture_east_asian_writing_stand",
			"stand",
			"a low writing stand",
			null,
			"This low writing stand is a small, well-made stand built from cedar boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			2400.0,
			40.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Small low stand for a book, writing board, or brush work."
		);

		CreateItem(
			"medieval_furniture_steppe_collapsible_stool",
			"stool",
			"a collapsible camp stool",
			null,
			"This collapsible camp stool is a small, workmanlike stool made from worked leather. A compact seat stands on short legs, with cross braces near the base. The top is slightly dished from repeated sitting. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1600.0,
			24.0m,
			true,
			false,
			"leather",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Compact folding stool for riding camps, wagons, and caravan stops."
		);

		CreateItem(
			"medieval_furniture_steppe_felt_sitting_mat",
			"mat",
			"a felt sitting mat",
			null,
			"This felt sitting mat is a large, workmanlike mat made from pressed felt. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			4200.0,
			24.0m,
			true,
			false,
			"felt",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Double"
			],
			null,
			null,
			null,
			null,
			"Two-person felt floor-seating mat for tents, camps, and cold ground."
		);

		CreateItem(
			"medieval_furniture_steppe_felt_sleeping_mat",
			"mat",
			"a felt sleeping mat",
			null,
			"This felt sleeping mat is a large, workmanlike mat made from pressed felt. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			4800.0,
			28.0m,
			true,
			false,
			"felt",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Portable felt sleeping surface for tent, wagon, or camp use."
		);

		CreateItem(
			"medieval_furniture_steppe_fine_cushion_seat",
			"cushion",
			"a patterned felt cushion",
			null,
			"This patterned felt cushion is a medium-sized, well-made cushion made from pressed felt. The padded surface is low and broad, with stitched edges keeping the filling in place. The centre is slightly compressed from use. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1700.0,
			42.0m,
			true,
			false,
			"felt",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Fine single cushion for a tent interior, wagon, or elite travelling household."
		);

		CreateItem(
			"medieval_furniture_steppe_folding_camp_table",
			"table",
			"a folding camp table",
			null,
			"This folding camp table is a medium-sized, workmanlike table built from ash boards. A rectangular top rests on hinged legs that fold beneath it. The joints are pinned and rubbed bright where the frame moves. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6500.0,
			42.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Portable table for camps, caravans, and temporary encampments."
		);

		CreateItem(
			"medieval_furniture_steppe_leather_bedroll",
			"bedroll",
			"a leather-bound bedroll",
			null,
			"This leather-bound bedroll is a medium-sized, workmanlike bedroll made from worked leather. Rolled bedding is tied with straps, with the outer layer wrapped tight around the softer middle. The ends show folded cloth and compressed padding. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5200.0,
			42.0m,
			true,
			false,
			"leather",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Travel bedroll with leather as the primary visible material."
		);

		CreateItem(
			"medieval_furniture_steppe_low_travel_table",
			"table",
			"a low travel table",
			null,
			"This low travel table is a medium-sized, workmanlike table built from birch boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5200.0,
			34.0m,
			true,
			false,
			"birch",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Low table for portable meals, account work, and tent interiors."
		);

		CreateItem(
			"medieval_furniture_steppe_pack_cot",
			"cot",
			"a folding pack cot",
			null,
			"This folding pack cot is a large, workmanlike cot built from birch boards. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8500.0,
			50.0m,
			true,
			false,
			"birch",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cot_Surface"
			],
			null,
			null,
			null,
			null,
			"Light folding cot designed for portability and camp use."
		);

		CreateItem(
			"medieval_furniture_steppe_tent_chest_table",
			"table",
			"a low tent table",
			null,
			"This low tent table is a medium-sized, workmanlike table built from birch boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			7000.0,
			38.0m,
			true,
			false,
			"birch",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Low tent table for bowls, cups, lamps, and travel goods; not a storage chest."
		);

		CreateItem(
			"medieval_furniture_steppe_wagon_bench",
			"bench",
			"a narrow wagon bench",
			null,
			"This narrow wagon bench is a large, workmanlike bench built from birch boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			30.0m,
			true,
			false,
			"birch",
			[
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Double",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Narrow two-seat bench for wagons, camps, or portable interiors."
		);

		CreateItem(
			"medieval_heat_bedchamber_ember_pot",
			"pot",
			"a bedchamber ember pot",
			null,
			"This bedchamber ember pot is a small, workmanlike pot formed from earthenware. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1400.0,
			10.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Small covered pot for safely carrying or holding a modest ember charge."
		);

		CreateItem(
			"medieval_heat_brass_warming_pan",
			"pan",
			"a brass warming pan",
			null,
			"This brass warming pan is a small, well-made pan worked from brass. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			2100.0,
			38.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Covered warming pan with a shallow fuel chamber and a long practical handle."
		);

		CreateItem(
			"medieval_heat_bronze_table_brazier",
			"brazier",
			"a bronze table brazier",
			null,
			"This bronze table brazier is a medium-sized, well-made brazier worked from bronze. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3900.0,
			70.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Fine small brazier for a table, counter, or chamber where controlled heat is useful."
		);

		CreateItem(
			"medieval_heat_cast_iron_room_brazier",
			"brazier",
			"a cast-iron room brazier",
			null,
			"This cast-iron room brazier is a large, well-made brazier worked from cast iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Large,
			ItemQuality.Good,
			12500.0,
			84.0m,
			true,
			false,
			"cast iron",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Heavy room brazier with a deep bowl and stout feet for sustained heat."
		);

		CreateItem(
			"medieval_heat_charcoal_hand_warmer",
			"warmer",
			"a charcoal hand warmer",
			null,
			"This charcoal hand warmer is a small, well-made warmer worked from brass. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			24.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Small vented warming vessel intended for a limited charcoal charge."
		);

		CreateItem(
			"medieval_heat_clay_charcoal_brazier",
			"brazier",
			"a clay charcoal brazier",
			null,
			"This clay charcoal brazier is a medium-sized, workmanlike brazier formed from fired clay. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3200.0,
			16.0m,
			true,
			false,
			"clay",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Low clay brazier blackened by charcoal use around the bowl and rim."
		);

		CreateItem(
			"medieval_heat_courtyard_fire_basin",
			"basin",
			"a courtyard fire basin",
			null,
			"This courtyard fire basin is a large, workmanlike basin worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9600.0,
			54.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Broad metal basin for outdoor warmth and gathering light in a courtyard or yard."
		);

		CreateItem(
			"medieval_heat_hall_floor_brazier",
			"brazier",
			"a hall floor brazier",
			null,
			"This hall floor brazier is a large, well-made brazier worked from cast iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Large,
			ItemQuality.Good,
			14200.0,
			96.0m,
			true,
			false,
			"cast iron",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Large floor brazier for a hall, common room, watch chamber, or feasting space."
		);

		CreateItem(
			"medieval_heat_iron_ember_pan",
			"pan",
			"an iron ember pan",
			null,
			"This iron ember pan is a small, workmanlike pan worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			24.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Handled metal pan for carrying embers or warming a small place."
		);

		CreateItem(
			"medieval_heat_small_charcoal_brazier",
			"brazier",
			"a small charcoal brazier",
			null,
			"This small charcoal brazier is a small, workmanlike brazier worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			2600.0,
			28.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Compact brazier for personal warmth, small chambers, or workshop tasks."
		);

		CreateItem(
			"medieval_heat_standing_fire_basket",
			"basket",
			"a standing fire basket",
			null,
			"This standing fire basket is a medium-sized, workmanlike basket worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4600.0,
			34.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Open iron basket raised on legs, intended for glowing fuel rather than storage."
		);

		CreateItem(
			"medieval_heat_terracotta_fire_bowl",
			"bowl",
			"a terracotta fire bowl",
			null,
			"This terracotta fire bowl is a medium-sized, workmanlike bowl formed from terracotta. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3600.0,
			18.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Reddish fire bowl meant to cradle embers or charcoal for light warmth."
		);

		CreateItem(
			"medieval_heat_workshop_coal_brazier",
			"brazier",
			"a workshop coal brazier",
			null,
			"This workshop coal brazier is a large, workmanlike brazier worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8800.0,
			58.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Sturdy workshop brazier with broad feet and blackened sides."
		);

		CreateItem(
			"medieval_heat_wrought_iron_brazier",
			"brazier",
			"a wrought-iron portable brazier",
			null,
			"This wrought-iron portable brazier is a medium-sized, workmanlike brazier worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5400.0,
			42.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Portable metal brazier for charcoal or other solid fuel, suited to warming a small room or work area."
		);

		CreateItem(
			"medieval_light_ash_watch_torch",
			"torch",
			"an ash watch torch",
			null,
			"This ash watch torch is a small, workmanlike torch built from ash boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			760.0,
			5.0m,
			true,
			false,
			"ash",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Torch_3Hour"
			],
			null,
			null,
			null,
			null,
			"Heavier torch for watchmen, night patrols, and road or wall service."
		);

		CreateItem(
			"medieval_light_beech_wall_torch",
			"torch",
			"a beech wall torch",
			null,
			"This beech wall torch is a small, workmanlike torch built from beech boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			690.0,
			4.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Torch_2Hour"
			],
			null,
			null,
			null,
			null,
			"Plain torch sized for a wall bracket or carried use in a hall, yard, or gatehouse."
		);

		CreateItem(
			"medieval_light_birch_bark_torch",
			"torch",
			"a birch-bark torch",
			null,
			"This birch-bark torch is a small, workmanlike torch built from birch boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			430.0,
			3.0m,
			true,
			false,
			"birch",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Torch_1Hour"
			],
			null,
			null,
			null,
			null,
			"Light torch wrapped with curled bark for quick fire and short outdoor use."
		);

		CreateItem(
			"medieval_light_brass_hanging_lamp",
			"lamp",
			"a brass hanging lamp",
			null,
			"This brass hanging lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1100.0,
			32.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Metal lamp with loops and a suspended body for hanging from a chain, beam, or bracket."
		);

		CreateItem(
			"medieval_light_brass_wall_sconce",
			"sconce",
			"a brass wall sconce",
			null,
			"This brass wall sconce is a small, well-made sconce worked from brass. A metal socket projects from the front face, with soot gathered above the cup. The fixing plate is worn around its holes. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			760.0,
			34.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Wall_Shelf"
			],
			null,
			null,
			null,
			null,
			"Polished brass wall support for chamber lighting, made as a decorative holder rather than a light source."
		);

		CreateItem(
			"medieval_light_bright_work_candle",
			"candle",
			"a bright work candle",
			null,
			"This bright work candle is a very small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			130.0,
			4.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle_Bright"
			],
			null,
			null,
			null,
			null,
			"Brighter candle for close work, accounts, sewing, cutting, or inspections after dark."
		);

		CreateItem(
			"medieval_light_bronze_lamp_tray",
			"tray",
			"a bronze lamp tray",
			null,
			"This bronze lamp tray is a small, well-made tray worked from bronze. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			950.0,
			34.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Lighting",
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
			"Fine tray sized for small lamps, wick tools, or oil spills; it provides surface storage only."
		);

		CreateItem(
			"medieval_light_bronze_table_lamp",
			"lamp",
			"a bronze table oil lamp",
			null,
			"This bronze table oil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			28.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Durable bronze lamp for a table, counter, desk, or high-status chamber."
		);

		CreateItem(
			"medieval_light_bundle_of_brands",
			"brand",
			"a bundle of pine brands",
			null,
			"This bundle of pine brands is a small, workmanlike brand built from pine boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			480.0,
			3.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Torch_1Hour"
			],
			null,
			null,
			null,
			null,
			"Several thin firebrands bound together for short-lived carried light."
		);

		CreateItem(
			"medieval_light_candle_storage_rack",
			"rack",
			"a candle storage rack",
			null,
			"This candle storage rack is a medium-sized, workmanlike rack built from pine boards. A row of shallow sockets and notches runs across the frame, with soot marks above the most used places. The lower rail is smoothed by handling. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			18.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wide_Shelves"
			],
			null,
			null,
			null,
			null,
			"Open rack for spare candles, tapers, spills, and small lighting goods."
		);

		CreateItem(
			"medieval_light_cedar_resin_torch",
			"torch",
			"a cedar resin torch",
			null,
			"This cedar resin torch is a small, workmanlike torch built from cedar boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			600.0,
			5.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Torch_2Hour"
			],
			null,
			null,
			null,
			null,
			"Resin-scented torch suited to gates, yards, docks, and dark approaches."
		);

		CreateItem(
			"medieval_light_clay_saucer_lamp",
			"lamp",
			"a clay saucer oil lamp",
			null,
			"This clay saucer oil lamp is a very small, workmanlike lamp formed from fired clay. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			280.0,
			3.0m,
			true,
			false,
			"clay",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Open saucer-like lamp body with a pinched place for a wick and liquid fuel."
		);

		CreateItem(
			"medieval_light_covered_boat_lantern",
			"lantern",
			"a covered boat lantern",
			null,
			"This covered boat lantern is a small, well-made lantern worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1350.0,
			42.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Shielded brass lantern with a covered flame suitable for damp air and night travel."
		);

		CreateItem(
			"medieval_light_cup_wax_candle",
			"candle",
			"a small cup candle",
			null,
			"This small cup candle is a very small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			160.0,
			5.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle"
			],
			null,
			null,
			null,
			null,
			"Low candle set into a shallow cup-like body for stable table or shelf use."
		);

		CreateItem(
			"medieval_light_desk_lantern",
			"lantern",
			"a small desk lantern",
			null,
			"This small desk lantern is a small, well-made lantern worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			980.0,
			36.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Stable lantern for reading, accounts, copying, or close table work."
		);

		CreateItem(
			"medieval_light_double_wicked_candle",
			"candle",
			"a double-wicked candle",
			null,
			"This double-wicked candle is a very small, well-made candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			150.0,
			6.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle_Bright"
			],
			null,
			null,
			null,
			null,
			"Better-made candle with two visible wicks, intended to cast stronger light for work or display."
		);

		CreateItem(
			"medieval_light_earthenware_wick_lamp",
			"lamp",
			"an earthenware wick lamp",
			null,
			"This earthenware wick lamp is a very small, workmanlike lamp formed from earthenware. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			360.0,
			4.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Small fired earthenware oil lamp with a rounded reservoir and wick mouth."
		);

		CreateItem(
			"medieval_light_fired_clay_pinched_lamp",
			"lamp",
			"a pinched fired-clay lamp",
			null,
			"This pinched fired-clay lamp is a very small, workmanlike lamp formed from fired clay. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			260.0,
			3.0m,
			true,
			false,
			"fired clay",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Plain hand-pinched lamp with an open oil bowl and narrowed wick point."
		);

		CreateItem(
			"medieval_light_gatehouse_cresset_torch",
			"cresset",
			"a gatehouse cresset torch",
			null,
			"This gatehouse cresset torch is a medium-sized, workmanlike cresset worked from wrought iron. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3100.0,
			24.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Torch_3Hour"
			],
			null,
			null,
			null,
			null,
			"Iron cresset-style torch made for strong open flame near a gate or watch post."
		);

		CreateItem(
			"medieval_light_glass_window_lantern",
			"lantern",
			"a glass-window lantern",
			null,
			"This glass-window lantern is a small, well-made lantern made from glass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1200.0,
			48.0m,
			true,
			false,
			"glass",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Lantern with glass panes set into a metal frame, suitable for brighter indoor display."
		);

		CreateItem(
			"medieval_light_glazed_ceramic_lamp",
			"lamp",
			"a glazed ceramic lamp",
			null,
			"This glazed ceramic lamp is a very small, well-made lamp formed from glazed ceramic. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			380.0,
			9.0m,
			true,
			false,
			"glazed ceramic",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Small glazed lamp with a smoother finish and enclosed reservoir."
		);

		CreateItem(
			"medieval_light_hall_hanging_lantern",
			"lantern",
			"a hall hanging lantern",
			null,
			"This hall hanging lantern is a medium-sized, well-made lantern worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2400.0,
			64.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Larger hanging lantern intended to light a hall, refectory, or shared chamber."
		);

		CreateItem(
			"medieval_light_hanging_lamp_frame",
			"frame",
			"a hanging lamp frame",
			null,
			"This hanging lamp frame is a medium-sized, workmanlike frame worked from wrought iron. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1900.0,
			24.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Lighting",
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
			"Iron frame meant to cradle a small lamp or candle dish while suspended."
		);

		CreateItem(
			"medieval_light_hemp_rushlight",
			"rushlight",
			"a hemp-wick rushlight",
			null,
			"This hemp-wick rushlight is a tiny, workmanlike rushlight made from woven hemp. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			28.0,
			1.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle"
			],
			null,
			null,
			null,
			null,
			"Cheap small light made around a fibrous wick, useful where only weak illumination is needed."
		);

		CreateItem(
			"medieval_light_horn_window_lantern",
			"lantern",
			"a horn-window lantern",
			null,
			"This horn-window lantern is a small, well-made lantern worked from horn. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			950.0,
			34.0m,
			true,
			false,
			"horn",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Lantern with translucent horn panels protecting the flame from draughts."
		);

		CreateItem(
			"medieval_light_iron_candle_tray",
			"tray",
			"an iron candle tray",
			null,
			"This iron candle tray is a small, workmanlike tray worked from wrought iron. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			700.0,
			14.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Lighting",
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
			"Shallow metal tray for carrying or grouping candles; it is a support surface, not a light source on its own."
		);

		CreateItem(
			"medieval_light_iron_torch_bracket",
			"bracket",
			"an iron torch bracket",
			null,
			"This iron torch bracket is a small, workmanlike bracket worked from wrought iron. A metal socket projects from the front face, with soot gathered above the cup. The fixing plate is worn around its holes. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			980.0,
			16.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Wall_Shelf"
			],
			null,
			null,
			null,
			null,
			"Stout bracket shaped to support a torch or brand against a wall."
		);

		CreateItem(
			"medieval_light_lamp_mending_tray",
			"tray",
			"a lamp-mending tray",
			null,
			"This lamp-mending tray is a small, workmanlike tray built from beech boards. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			10.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Lighting",
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
			"Shallow tray for wicks, snuffers, lamp pins, and small lighting repairs."
		);

		CreateItem(
			"medieval_light_lantern_carrying_hook",
			"hook",
			"a lantern carrying hook",
			null,
			"This lantern carrying hook is a small, workmanlike hook worked from wrought iron. A metal socket projects from the front face, with soot gathered above the cup. The fixing plate is worn around its holes. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			8.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Curved iron hook for carrying or hanging a lantern; it has no container or light behaviour."
		);

		CreateItem(
			"medieval_light_lidded_oil_lamp",
			"lamp",
			"a lidded oil lamp",
			null,
			"This lidded oil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			850.0,
			30.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Metal oil lamp with a fitted lid, handle, and narrowed wick opening."
		);

		CreateItem(
			"medieval_light_linen_wick_night_light",
			"night-light",
			"a linen-wick night-light",
			null,
			"This linen-wick night-light is a tiny, workmanlike night-light made from woven linen. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			40.0,
			1.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle_Long"
			],
			null,
			null,
			null,
			null,
			"Very small dim light for marking a chamber or passage through the night."
		);

		CreateItem(
			"medieval_light_long_beeswax_taper",
			"taper",
			"a long beeswax taper",
			null,
			"This long beeswax taper is a tiny, workmanlike taper made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			55.0,
			2.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle"
			],
			null,
			null,
			null,
			null,
			"Slender household taper intended to give modest light over a longer interval."
		);

		CreateItem(
			"medieval_light_long_watch_candle",
			"candle",
			"a long-burning watch candle",
			null,
			"This long-burning watch candle is a small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			6.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle_Long"
			],
			null,
			null,
			null,
			null,
			"Thick slow-burning candle for night watches, sickrooms, and long household vigils."
		);

		CreateItem(
			"medieval_light_oak_hand_torch",
			"torch",
			"an oak hand torch",
			null,
			"This oak hand torch is a small, workmanlike torch built from oak boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			4.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Torch_2Hour"
			],
			null,
			null,
			null,
			null,
			"Stout hand torch made from an oak shaft with a wrapped burning head."
		);

		CreateItem(
			"medieval_light_oak_lamp_stand",
			"stand",
			"an oak lamp stand",
			null,
			"This oak lamp stand is a medium-sized, workmanlike stand built from oak boards. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			26.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Small stand with a flat top for a lamp, lantern, or candle dish."
		);

		CreateItem(
			"medieval_light_oak_torch_stand",
			"stand",
			"an oak torch stand",
			null,
			"This oak torch stand is a medium-sized, workmanlike stand built from oak boards. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5200.0,
			28.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Freestanding wooden stand with a metal cup or socket for a torch or lantern."
		);

		CreateItem(
			"medieval_light_pine_lantern_pole",
			"pole",
			"a pine lantern pole",
			null,
			"This pine lantern pole is a large, workmanlike pole built from pine boards. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2600.0,
			14.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Plain pole with notches and an iron hook for suspending a lantern."
		);

		CreateItem(
			"medieval_light_pine_pitch_torch",
			"torch",
			"a pine pitch torch",
			null,
			"This pine pitch torch is a small, workmanlike torch built from pine boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			5.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Torch_3Hour"
			],
			null,
			null,
			null,
			null,
			"Pitch-rich pine torch intended for stronger outdoor light and longer burning."
		);

		CreateItem(
			"medieval_light_pine_spill_taper",
			"taper",
			"a pine spill taper",
			null,
			"This pine spill taper is a tiny, workmanlike taper built from pine boards. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			22.0,
			1.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Torch_1Hour"
			],
			null,
			null,
			null,
			null,
			"Thin resinous spill used to transfer fire from hearth to candle, lamp, or torch."
		);

		CreateItem(
			"medieval_light_plain_dipped_candle",
			"candle",
			"a plain dipped candle",
			null,
			"This plain dipped candle is a very small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			90.0,
			2.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle"
			],
			null,
			null,
			null,
			null,
			"Simple repeatedly dipped candle with a slightly uneven body and ordinary wick."
		);

		CreateItem(
			"medieval_light_pricket_candlestick",
			"candlestick",
			"a pricket candlestick",
			null,
			"This pricket candlestick is a small, well-made candlestick worked from bronze. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			880.0,
			30.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Luxury Wares"
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
			"Candlestick with a central spike and shallow base, useful as a candle support."
		);

		CreateItem(
			"medieval_light_ring_chandelier",
			"chandelier",
			"a ring chandelier",
			null,
			"This ring chandelier is a large, well-made chandelier worked from wrought iron. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Large,
			ItemQuality.Good,
			7400.0,
			74.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Lighting",
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
			"Heavy ring with candle cups or small lamp rests, intended as a suspended support rather than an active light source."
		);

		CreateItem(
			"medieval_light_rolled_wax_candle",
			"candle",
			"a rolled beeswax candle",
			null,
			"This rolled beeswax candle is a very small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			100.0,
			4.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle"
			],
			null,
			null,
			null,
			null,
			"Rolled wax candle with the faint ridges of layered sheet wax around the body."
		);

		CreateItem(
			"medieval_light_rushlight_holder",
			"holder",
			"a rushlight holder",
			null,
			"This rushlight holder is a small, workmanlike holder worked from wrought iron. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			12.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Lighting",
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
			"Simple clamp-and-dish holder for a small rushlight or taper; it is a support object only."
		);

		CreateItem(
			"medieval_light_shipboard_signal_fire",
			"beacon",
			"a shipboard signal fire",
			null,
			"This shipboard signal fire is a medium-sized, workmanlike beacon worked from wrought iron. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			30.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SignalFire"
			],
			null,
			null,
			null,
			null,
			"Contained signal-fire basket for docks, ships, harbour walls, and night signalling."
		);

		CreateItem(
			"medieval_light_short_beeswax_taper",
			"taper",
			"a short beeswax taper",
			null,
			"This short beeswax taper is a tiny, workmanlike taper made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			1.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle_Bright"
			],
			null,
			null,
			null,
			null,
			"Short clean-burning taper for brief domestic tasks, desk work, or bedside use."
		);

		CreateItem(
			"medieval_light_stable_lantern",
			"lantern",
			"a stable lantern",
			null,
			"This stable lantern is a small, workmanlike lantern worked from wrought iron. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1450.0,
			24.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Plain protected lantern for barns, stables, sheds, and outbuildings."
		);

		CreateItem(
			"medieval_light_tarred_rope_torch",
			"torch",
			"a tarred rope torch",
			null,
			"This tarred rope torch is a small, workmanlike torch made from woven hemp. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			700.0,
			5.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Torch_2Hour"
			],
			null,
			null,
			null,
			null,
			"Torch with a wrapped rope head, meant to hold fuel and burn steadily."
		);

		CreateItem(
			"medieval_light_terracotta_spouted_lamp",
			"lamp",
			"a terracotta spouted lamp",
			null,
			"This terracotta spouted lamp is a very small, workmanlike lamp formed from terracotta. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			330.0,
			5.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Reddish terracotta lamp with a pinched spout for the wick and a low oil chamber."
		);

		CreateItem(
			"medieval_light_thick_household_candle",
			"candle",
			"a thick household candle",
			null,
			"This thick household candle is a very small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			120.0,
			3.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle"
			],
			null,
			null,
			null,
			null,
			"Ordinary household candle with enough wax for sustained room or table light."
		);

		CreateItem(
			"medieval_light_travel_oil_lantern",
			"lantern",
			"a travel oil lantern",
			null,
			"This travel oil lantern is a small, workmanlike lantern worked from wrought iron. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1250.0,
			28.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Compact shielded lantern with a carrying handle and reinforced frame."
		);

		CreateItem(
			"medieval_light_two_spout_lamp",
			"lamp",
			"a two-spout oil lamp",
			null,
			"This two-spout oil lamp is a small, workmanlike lamp formed from earthenware. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			7.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Broader earthenware lamp with two wick points for slightly wider illumination."
		);

		CreateItem(
			"medieval_light_walnut_candle_stand",
			"stand",
			"a walnut candle stand",
			null,
			"This walnut candle stand is a medium-sized, well-made stand built from walnut boards. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			44.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Better-finished stand intended to support a candlestick or small lamp."
		);

		CreateItem(
			"medieval_light_watch_signal_basket",
			"beacon",
			"a watch signal basket",
			null,
			"This watch signal basket is a medium-sized, workmanlike beacon worked from wrought iron. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3800.0,
			26.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SignalFire"
			],
			null,
			null,
			null,
			null,
			"Metal basket for a bright warning or signalling flame on a tower, wall, or hilltop."
		);

		CreateItem(
			"medieval_light_willow_yard_torch",
			"torch",
			"a willow yard torch",
			null,
			"This willow yard torch is a small, workmanlike torch built from willow boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			3.0m,
			true,
			false,
			"willow",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Torch_1Hour"
			],
			null,
			null,
			null,
			null,
			"Light torch for yard work, stables, or brief errands between buildings."
		);

		CreateItem(
			"medieval_light_workshop_lantern",
			"lantern",
			"a workshop lantern",
			null,
			"This workshop lantern is a small, workmanlike lantern worked from wrought iron. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1600.0,
			30.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Rugged lantern for benches, sheds, and workshops where the flame needs protection."
		);

		CreateItem(
			"medieval_light_wrought_iron_oil_lantern",
			"lantern",
			"a wrought-iron oil lantern",
			null,
			"This wrought-iron oil lantern is a small, workmanlike lantern worked from wrought iron. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1350.0,
			26.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Sturdy lantern frame with a protected reservoir, handle, and darkened ironwork."
		);

		CreateItem(
			"medieval_light_wrought_iron_wall_sconce",
			"sconce",
			"a wrought-iron wall sconce",
			null,
			"This wrought-iron wall sconce is a small, workmanlike sconce worked from wrought iron. A metal socket projects from the front face, with soot gathered above the cup. The fixing plate is worn around its holes. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			850.0,
			18.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Wall_Shelf"
			],
			null,
			null,
			null,
			null,
			"Wall sconce with a small support shelf or cup for a candle or lamp; it does not produce light by itself."
		);

		CreateItem(
			"medieval_heat_northern_winter_brazier",
			"brazier",
			"a winter iron brazier",
			null,
			"This winter iron brazier is a large, workmanlike brazier worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9800.0,
			62.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Large blackened brazier suited to cold chambers, workshops, and guardrooms."
		);

		CreateItem(
			"medieval_light_northern_birch_splint_torch",
			"torch",
			"a birch splint torch",
			null,
			"This birch splint torch is a small, workmanlike torch built from birch boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			390.0,
			3.0m,
			true,
			false,
			"birch",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Torch_1Hour"
			],
			null,
			null,
			null,
			null,
			"Fast-lighting splint torch for short outdoor or outbuilding use."
		);

		CreateItem(
			"medieval_light_northern_horn_stable_lantern",
			"lantern",
			"a horn stable lantern",
			null,
			"This horn stable lantern is a small, workmanlike lantern worked from horn. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			880.0,
			26.0m,
			true,
			false,
			"horn",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Practical horn-panel lantern for barns, stables, byres, and winter passages."
		);

		CreateItem(
			"medieval_light_northern_iron_cresset_basket",
			"cresset",
			"an iron cresset basket",
			null,
			"This iron cresset basket is a medium-sized, workmanlike cresset worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3400.0,
			28.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Open iron cresset basket for fuel and night work in cold or windy places."
		);

		CreateItem(
			"medieval_light_northern_long_hemp_rushlight",
			"rushlight",
			"a long hemp rushlight",
			null,
			"This long hemp rushlight is a tiny, workmanlike rushlight made from woven hemp. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			34.0,
			1.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle_Long"
			],
			null,
			null,
			null,
			null,
			"Dim slow-burning rushlight for watchful nights and small chambers."
		);

		CreateItem(
			"medieval_light_northern_oak_hearth_lamp_stand",
			"stand",
			"an oak hearth-lamp stand",
			null,
			"This oak hearth-lamp stand is a medium-sized, workmanlike stand built from oak boards. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4800.0,
			26.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Stout wooden stand intended to keep a lamp or candle dish near the hearth."
		);

		CreateItem(
			"medieval_light_northern_pine_pitch_brand",
			"brand",
			"a pine pitch brand",
			null,
			"This pine pitch brand is a small, workmanlike brand built from pine boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			460.0,
			4.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Torch_2Hour"
			],
			null,
			null,
			null,
			null,
			"Resin-rich pine brand suited to cold yards, tracks, and timber buildings."
		);

		CreateItem(
			"medieval_light_northern_soapstone_grease_lamp",
			"lamp",
			"a soapstone grease lamp",
			null,
			"This soapstone grease lamp is a small, workmanlike lamp cut from soapstone. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1250.0,
			20.0m,
			true,
			false,
			"soapstone",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Dense stone lamp suited to northern households and workrooms, with a shallow fuel reservoir."
		);

		CreateItem(
			"medieval_heat_western_chamber_brazier",
			"brazier",
			"a chamber warming brazier",
			null,
			"This chamber warming brazier is a medium-sized, well-made brazier worked from brass. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			4700.0,
			72.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Well-made warming brazier for a private chamber or better lodging."
		);

		CreateItem(
			"medieval_light_western_chamber_candlestick",
			"candlestick",
			"a brass chamber candlestick",
			null,
			"This brass chamber candlestick is a small, well-made candlestick worked from brass. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			720.0,
			26.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Luxury Wares"
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
			"Polished candlestick with a broad base for chamber or table use."
		);

		CreateItem(
			"medieval_light_western_feast_candle",
			"candle",
			"a beeswax feast candle",
			null,
			"This beeswax feast candle is a small, well-made candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			240.0,
			8.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle_Bright"
			],
			null,
			null,
			null,
			null,
			"Large bright candle suitable for a feast board, guild table, or high chamber."
		);

		CreateItem(
			"medieval_light_western_glazed_chamber_lamp",
			"lamp",
			"a glazed chamber lamp",
			null,
			"This glazed chamber lamp is a very small, well-made lamp formed from glazed ceramic. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			410.0,
			10.0m,
			true,
			false,
			"glazed ceramic",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Refined glazed oil lamp for a chamber, writing room, or table."
		);

		CreateItem(
			"medieval_light_western_hall_lantern",
			"lantern",
			"a bronze hall lantern",
			null,
			"This bronze hall lantern is a medium-sized, well-made lantern worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2600.0,
			68.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Large bronze lantern with a formal, well-finished frame for a hall or manor chamber."
		);

		CreateItem(
			"medieval_light_western_iron_cresset",
			"cresset",
			"a wrought-iron cresset",
			null,
			"This wrought-iron cresset is a medium-sized, workmanlike cresset worked from wrought iron. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3300.0,
			28.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Torch_3Hour"
			],
			null,
			null,
			null,
			null,
			"Iron cresset intended for strong flame in a hall, yard, or street-front setting."
		);

		CreateItem(
			"medieval_light_western_oak_torch_stand",
			"stand",
			"an oak hall torch stand",
			null,
			"This oak hall torch stand is a medium-sized, workmanlike stand built from oak boards. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5600.0,
			30.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Standing timber support for a torch, brand, or lantern in a hall."
		);

		CreateItem(
			"medieval_heat_mediterranean_low_bronze_brazier",
			"brazier",
			"a low bronze brazier",
			null,
			"This low bronze brazier is a medium-sized, well-made brazier worked from bronze. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			4300.0,
			74.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Low bronze brazier for a courtyard, reception room, or shaded interior."
		);

		CreateItem(
			"medieval_light_mediterranean_brass_pierced_lantern",
			"lantern",
			"a pierced brass lantern",
			null,
			"This pierced brass lantern is a small, well-made lantern worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1150.0,
			42.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Brass lantern with pierced openings that shield and pattern the light."
		);

		CreateItem(
			"medieval_light_mediterranean_bronze_hanging_lamp",
			"lamp",
			"a bronze hanging oil lamp",
			null,
			"This bronze hanging oil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1050.0,
			34.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Suspended bronze lamp with small loops and a rounded fuel body."
		);

		CreateItem(
			"medieval_light_mediterranean_ceramic_saucer_lamp",
			"lamp",
			"a ceramic saucer lamp",
			null,
			"This ceramic saucer lamp is a very small, workmanlike lamp formed from ceramic. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			320.0,
			6.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Small saucer-shaped lamp for a niche, table, or outdoor step."
		);

		CreateItem(
			"medieval_light_mediterranean_clay_window_lamp",
			"lamp",
			"a clay window lamp",
			null,
			"This clay window lamp is a small, workmanlike lamp formed from fired clay. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			430.0,
			5.0m,
			true,
			false,
			"clay",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Compact oil lamp shaped to sit on a sill, niche, or wall recess."
		);

		CreateItem(
			"medieval_light_mediterranean_marble_lamp_stand",
			"stand",
			"a marble lamp stand",
			null,
			"This marble lamp stand is a medium-sized, well-made stand cut from marble. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			14500.0,
			90.0m,
			true,
			false,
			"marble",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Heavy stone stand for a lamp or candle dish, decorative but still movable with effort."
		);

		CreateItem(
			"medieval_light_mediterranean_olivewood_torch",
			"torch",
			"an olivewood hand torch",
			null,
			"This olivewood hand torch is a small, workmanlike torch built from olive wood. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			590.0,
			5.0m,
			true,
			false,
			"olive wood",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Torch_2Hour"
			],
			null,
			null,
			null,
			null,
			"Regional hardwood torch for outdoor courtyards, lanes, and walls."
		);

		CreateItem(
			"medieval_light_mediterranean_terracotta_lamp",
			"lamp",
			"a terracotta wick lamp",
			null,
			"This terracotta wick lamp is a very small, workmanlike lamp formed from terracotta. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			300.0,
			4.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Low reddish lamp with a wick spout and oil chamber, suited to warm-climate interiors."
		);

		CreateItem(
			"medieval_heat_islamicate_charcoal_scent_brazier",
			"brazier",
			"a charcoal scent brazier",
			null,
			"This charcoal scent brazier is a small, well-made brazier worked from brass. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1850.0,
			44.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Vented brazier for small charcoal charges and household scenting, not a religious censer."
		);

		CreateItem(
			"medieval_light_islamicate_brass_cresset",
			"cresset",
			"a brass cresset lamp",
			null,
			"This brass cresset lamp is a medium-sized, well-made cresset worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2400.0,
			52.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Open metal lamp form with a broad fuel body and decorative pierced work."
		);

		CreateItem(
			"medieval_light_islamicate_bronze_chain_lamp",
			"lamp",
			"a bronze chain-hung lamp",
			null,
			"This bronze chain-hung lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1380.0,
			46.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Bronze lamp body set up for hanging by short chains from a bracket or beam."
		);

		CreateItem(
			"medieval_light_islamicate_cedar_lamp_shelf",
			"shelf",
			"a carved cedar lamp shelf",
			null,
			"This carved cedar lamp shelf is a small, well-made shelf built from cedar boards. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			980.0,
			30.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wall_Shelf"
			],
			null,
			null,
			null,
			null,
			"Carved wooden shelf for holding a small lamp or incense-safe vessel; it is only a support surface."
		);

		CreateItem(
			"medieval_light_islamicate_courtyard_lantern",
			"lantern",
			"a courtyard oil lantern",
			null,
			"This courtyard oil lantern is a small, workmanlike lantern worked from wrought iron. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1450.0,
			30.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Rugged shielded lantern for a courtyard, passage, roof terrace, or shopfront."
		);

		CreateItem(
			"medieval_light_islamicate_glazed_oil_lamp",
			"lamp",
			"a glazed ceramic oil lamp",
			null,
			"This glazed ceramic oil lamp is a very small, well-made lamp formed from glazed ceramic. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			390.0,
			11.0m,
			true,
			false,
			"glazed ceramic",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Glazed oil lamp with a smooth reservoir and narrowed wick channel."
		);

		CreateItem(
			"medieval_light_islamicate_long_neck_lamp",
			"lamp",
			"a long-necked oil lamp",
			null,
			"This long-necked oil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			930.0,
			36.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Metal lamp with a drawn-out neck and handle for careful pouring and lighting."
		);

		CreateItem(
			"medieval_light_islamicate_pierced_brass_lantern",
			"lantern",
			"a pierced brass courtyard lantern",
			null,
			"This pierced brass courtyard lantern is a small, well-made lantern worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1220.0,
			48.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Urban brass lantern with patterned piercings and a protected flame."
		);

		CreateItem(
			"medieval_heat_south_asian_brass_charcoal_brazier",
			"brazier",
			"a brass charcoal brazier",
			null,
			"This brass charcoal brazier is a medium-sized, well-made brazier worked from brass. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			60.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Polished brass brazier sized for a modest charcoal charge."
		);

		CreateItem(
			"medieval_light_south_asian_bamboo_courtyard_torch",
			"torch",
			"a bamboo courtyard torch",
			null,
			"This bamboo courtyard torch is a small, workmanlike torch built from split bamboo. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			3.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Torch_1Hour"
			],
			null,
			null,
			null,
			null,
			"Light bamboo-shafted torch for courtyards, lanes, and short outdoor tasks."
		);

		CreateItem(
			"medieval_light_south_asian_brass_cup_lamp",
			"lamp",
			"a brass cup oil lamp",
			null,
			"This brass cup oil lamp is a very small, workmanlike lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			420.0,
			12.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Small brass cup lamp for a shelf, threshold, courtyard, or household niche."
		);

		CreateItem(
			"medieval_light_south_asian_bronze_standing_lamp",
			"lamp",
			"a bronze standing oil lamp",
			null,
			"This bronze standing oil lamp is a medium-sized, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			5200.0,
			86.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Tall bronze oil lamp with a raised stem and small reservoir cups."
		);

		CreateItem(
			"medieval_light_south_asian_bronze_wall_lamp",
			"sconce",
			"a bronze wall-lamp bracket",
			null,
			"This bronze wall-lamp bracket is a small, well-made sconce worked from bronze. A metal socket projects from the front face, with soot gathered above the cup. The fixing plate is worn around its holes. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			34.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Wall_Shelf"
			],
			null,
			null,
			null,
			null,
			"Bronze wall bracket shaped to support a small lamp or candle cup."
		);

		CreateItem(
			"medieval_light_south_asian_glass_lantern",
			"lantern",
			"a glass-panel lantern",
			null,
			"This glass-panel lantern is a small, well-made lantern made from glass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1150.0,
			44.0m,
			true,
			false,
			"glass",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Glass-panel lantern with a protected flame and hand-carry frame."
		);

		CreateItem(
			"medieval_light_south_asian_teak_lamp_stand",
			"stand",
			"a teak lamp stand",
			null,
			"This teak lamp stand is a medium-sized, well-made stand built from teak boards. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3900.0,
			46.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Carved teak stand for a lamp, bowl, or candle dish."
		);

		CreateItem(
			"medieval_light_south_asian_terracotta_diya_lamp",
			"lamp",
			"a terracotta cup lamp",
			null,
			"This terracotta cup lamp is a tiny, workmanlike lamp formed from terracotta. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			160.0,
			2.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Tiny terracotta oil cup with a pinched wick lip."
		);

		CreateItem(
			"medieval_heat_east_asian_cast_iron_brazier",
			"brazier",
			"a cast-iron brazier",
			null,
			"This cast-iron brazier is a medium-sized, well-made brazier worked from cast iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			6200.0,
			72.0m,
			true,
			false,
			"cast iron",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Compact iron brazier suited to a chamber, tea room, workroom, or winter interior."
		);

		CreateItem(
			"medieval_light_east_asian_bamboo_hand_torch",
			"torch",
			"a bamboo hand torch",
			null,
			"This bamboo hand torch is a small, workmanlike torch built from split bamboo. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			410.0,
			3.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Torch_1Hour"
			],
			null,
			null,
			null,
			null,
			"Light torch with a bamboo shaft, good for short outdoor movement."
		);

		CreateItem(
			"medieval_light_east_asian_bamboo_paper_lantern",
			"lantern",
			"a bamboo paper lantern",
			null,
			"This bamboo paper lantern is a small, workmanlike lantern built from split bamboo. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			12.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle"
			],
			null,
			null,
			null,
			null,
			"Light bamboo-framed lantern with a paper shade around a small candle light."
		);

		CreateItem(
			"medieval_light_east_asian_bronze_oil_lamp",
			"lamp",
			"a bronze oil lamp",
			null,
			"This bronze oil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			850.0,
			32.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Compact bronze oil lamp for a room, desk, or household shrine-neutral alcove."
		);

		CreateItem(
			"medieval_light_east_asian_cypress_frame_lantern",
			"lantern",
			"a cypress frame lantern",
			null,
			"This cypress frame lantern is a small, well-made lantern built from cypress boards. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			720.0,
			24.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Candle_Long"
			],
			null,
			null,
			null,
			null,
			"Wood-framed lantern intended to shield a slow candle flame behind pale panels."
		);

		CreateItem(
			"medieval_light_east_asian_paper_candle_shade",
			"shade",
			"a paper candle shade",
			null,
			"This paper candle shade is a small, workmanlike shade made from layered paper. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			120.0,
			4.0m,
			true,
			false,
			"paper",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Folded paper shade for softening candle light; it has no light-source behaviour by itself."
		);

		CreateItem(
			"medieval_light_east_asian_pine_lamp_stand",
			"stand",
			"a low pine lamp stand",
			null,
			"This low pine lamp stand is a medium-sized, workmanlike stand built from pine boards. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2100.0,
			22.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Low timber stand for a lamp, candle lantern, or small vessel."
		);

		CreateItem(
			"medieval_light_east_asian_porcelain_table_lamp",
			"lamp",
			"a porcelain table lamp",
			null,
			"This porcelain table lamp is a small, well-made lamp formed from porcelain. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			680.0,
			38.0m,
			true,
			false,
			"porcelain",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Porcelain-bodied lamp with a smooth pale finish and a small fuel reservoir."
		);

		CreateItem(
			"medieval_heat_steppe_portable_brazier",
			"brazier",
			"a portable camp brazier",
			null,
			"This portable camp brazier is a medium-sized, workmanlike brazier worked from wrought iron. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3600.0,
			34.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Compact brazier for camps, tents, caravans, and temporary shelters."
		);

		CreateItem(
			"medieval_heat_steppe_willow_fire_basket",
			"basket",
			"a willow fire basket",
			null,
			"This willow fire basket is a medium-sized, workmanlike basket built from willow boards. A vented fuel bowl holds the fuel bed, with heat-darkened marks around the rim. The base is raised slightly to keep heat off the floor beneath. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			16.0m,
			true,
			false,
			"willow",
			[
				"Functions / Household Items / Household Heating",
				"Market / Domestic Heating / Combustion Heating"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Light basket-like fuel holder for small camp embers, reinforced enough for careful use."
		);

		CreateItem(
			"medieval_light_steppe_bronze_camp_lamp",
			"lamp",
			"a compact bronze camp lamp",
			null,
			"This compact bronze camp lamp is a very small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			620.0,
			24.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Small durable oil lamp suited to a tent, wagon, camp, or roadside lodging."
		);

		CreateItem(
			"medieval_light_steppe_cedar_resin_torch",
			"torch",
			"a cedar resin travel torch",
			null,
			"This cedar resin travel torch is a small, workmanlike torch built from cedar boards. A straight handle is wrapped near the head with fuel-soaked binding. The burning end is darker and bulkier than the hand grip. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			560.0,
			5.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Torches"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Torch_2Hour"
			],
			null,
			null,
			null,
			null,
			"Resinous torch for road, camp, and caravan use."
		);

		CreateItem(
			"medieval_light_steppe_horn_riding_lantern",
			"lantern",
			"a horn riding lantern",
			null,
			"This horn riding lantern is a small, well-made lantern worked from horn. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			820.0,
			30.0m,
			true,
			false,
			"horn",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Compact horn-panel lantern made to be carried or hung during travel."
		);

		CreateItem(
			"medieval_light_steppe_lamp_box",
			"box",
			"a travelling lamp box",
			null,
			"This travelling lamp box is a small, workmanlike box built from pine boards. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Soot-darkened points and heat marks show the places most exposed to flame. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1150.0,
			18.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Lighting",
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
			"Compact box for carrying a small lamp, wick pieces, and lighting tools; it is storage rather than a light source."
		);

		CreateItem(
			"medieval_light_steppe_travel_lantern",
			"lantern",
			"a leather-covered travel lantern",
			null,
			"This leather-covered travel lantern is a small, workmanlike lantern made from worked leather. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1050.0,
			28.0m,
			true,
			false,
			"leather",
			[
				"Functions / Household Items / Household Lighting",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Travel lantern with protective leather covering and a compact fuel body."
		);

		CreateItem(
			"medieval_decor_bed_canopy_cloth",
			"cloth",
			"a bed canopy cloth",
			null,
			"This bed canopy cloth is a large, well-made cloth made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			1900.0,
			34.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Broad canopy cloth meant to dress a bed frame or suspended sleeping space."
		);

		CreateItem(
			"medieval_decor_bed_curtain_pair",
			"curtain",
			"a pair of bed curtains",
			null,
			"This pair of bed curtains is large, workmanlike curtain made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2100.0,
			28.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Pair of hanging bed curtains for privacy and decoration; they are not doorway curtains."
		);

		CreateItem(
			"medieval_decor_bone_inlay_panel",
			"panel",
			"a bone-inlaid wall panel",
			null,
			"This bone-inlaid wall panel is a small, well-made panel worked from bone. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.Small,
			ItemQuality.Good,
			850.0,
			42.0m,
			true,
			false,
			"bone",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Decorative wall panel set with pale bone inlay in simple patterns."
		);

		CreateItem(
			"medieval_decor_bordered_valance",
			"valance",
			"a bordered cloth valance",
			null,
			"This bordered cloth valance is a medium-sized, well-made valance made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Good,
			650.0,
			18.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Decorative hanging strip for a bed, shelf, canopy, or wall edge."
		);

		CreateItem(
			"medieval_decor_brass_wall_mirror",
			"mirror",
			"a brass-framed wall mirror",
			null,
			"This brass-framed wall mirror is a small, well-made mirror worked from brass. A polished reflective face sits inside a raised frame, with a small loop set at the back. The edge of the frame is rubbed where it has been handled. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1100.0,
			54.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Small mirror with a brass frame and visible suspension loop."
		);

		CreateItem(
			"medieval_decor_carved_household_board",
			"board",
			"a carved household board",
			null,
			"This carved household board is a medium-sized, workmanlike board built from oak boards. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2800.0,
			20.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Carved display board for a hall, workshop, or household room."
		);

		CreateItem(
			"medieval_decor_carved_oak_wall_panel",
			"panel",
			"a carved oak wall panel",
			null,
			"This carved oak wall panel is a medium-sized, well-made panel built from oak boards. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			4200.0,
			48.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Portable carved panel suitable for hanging or leaning against a wall."
		);

		CreateItem(
			"medieval_decor_ceiling_canopy_cloth",
			"cloth",
			"a ceiling canopy cloth",
			null,
			"This ceiling canopy cloth is a large, well-made cloth made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			1800.0,
			34.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Broad cloth intended to be suspended above a bed, dais, or seat."
		);

		CreateItem(
			"medieval_decor_ceramic_tile_panel",
			"panel",
			"a ceramic tile panel",
			null,
			"This ceramic tile panel is a medium-sized, well-made panel formed from ceramic. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			5200.0,
			44.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Panel made from fitted decorative tiles, meant for a wall, hearth surround, or chamber display."
		);

		CreateItem(
			"medieval_decor_checked_wool_rug",
			"rug",
			"a checked wool rug",
			null,
			"This checked wool rug is a medium-sized, workmanlike rug made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2200.0,
			18.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Checked household rug suited to a bench, bed, or sitting space."
		);

		CreateItem(
			"medieval_decor_chest_cover_cloth",
			"cloth",
			"a chest cover cloth",
			null,
			"This chest cover cloth is a medium-sized, workmanlike cloth made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			10.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Decorative cloth sized to cover a chest, coffer, or storage bench."
		);

		CreateItem(
			"medieval_decor_coarse_wool_floor_rug",
			"rug",
			"a coarse wool floor rug",
			null,
			"This coarse wool floor rug is a medium-sized, workmanlike rug made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			12.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Plain woven rug for a room, chamber, stall, or sleeping space."
		);

		CreateItem(
			"medieval_decor_cotton_floor_spread",
			"spread",
			"a cotton floor spread",
			null,
			"This cotton floor spread is a large, workmanlike spread made from woven cotton. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1600.0,
			16.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Light floor spread for warm rooms, guest spaces, and low seating arrangements."
		);

		CreateItem(
			"medieval_decor_embroidered_wall_hanging",
			"hanging",
			"an embroidered wall hanging",
			null,
			"This embroidered wall hanging is a large, well-made hanging made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			2400.0,
			52.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Wool hanging with visible embroidered borders and worked panels."
		);

		CreateItem(
			"medieval_decor_feast_table_runner",
			"runner",
			"a woven feast-table runner",
			null,
			"This woven feast-table runner is a large, well-made runner made from woven linen. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			950.0,
			24.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Long decorative runner for a trestle, feast board, or high table."
		);

		CreateItem(
			"medieval_decor_felt_floor_pad",
			"pad",
			"a thick felt floor pad",
			null,
			"This thick felt floor pad is a medium-sized, workmanlike pad made from pressed felt. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1500.0,
			10.0m,
			true,
			false,
			"felt",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Thick felt pad for kneeling, sitting, or softening a hard floor; it has no seating component."
		);

		CreateItem(
			"medieval_decor_fleece_floor_rug",
			"rug",
			"a soft fleece floor rug",
			null,
			"This soft fleece floor rug is a medium-sized, workmanlike rug made from dressed fur. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1700.0,
			18.0m,
			true,
			false,
			"fur",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Soft pelt-like rug with a warm woolly surface."
		);

		CreateItem(
			"medieval_decor_fur_wall_drape",
			"drape",
			"a fur-backed wall drape",
			null,
			"This fur-backed wall drape is a large, well-made drape made from dressed fur. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Large,
			ItemQuality.Good,
			3600.0,
			70.0m,
			true,
			false,
			"fur",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Warm wall drape with fur backing and a plain textile face."
		);

		CreateItem(
			"medieval_decor_glass_wall_mirror",
			"mirror",
			"a small glass wall mirror",
			null,
			"This small glass wall mirror is a small, well-made mirror made from glass. A polished reflective face sits inside a raised frame, with a small loop set at the back. The edge of the frame is rubbed where it has been handled. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.Small,
			ItemQuality.Good,
			700.0,
			72.0m,
			true,
			false,
			"glass",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			"Small glass mirror backed and framed for display on a wall or chest top."
		);

		CreateItem(
			"medieval_decor_hearthside_rug",
			"rug",
			"a hearthside wool rug",
			null,
			"This hearthside wool rug is a medium-sized, workmanlike rug made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2100.0,
			16.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Dense rug intended for a hearthside, bench-end, or warm sitting corner."
		);

		CreateItem(
			"medieval_decor_hemp_floor_mat",
			"mat",
			"a plaited hemp floor mat",
			null,
			"This plaited hemp floor mat is a medium-sized, workmanlike mat made from woven hemp. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			7.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Hard-wearing plaited mat for an entry, service room, or work space."
		);

		CreateItem(
			"medieval_decor_hide_floor_rug",
			"rug",
			"a tanned hide floor rug",
			null,
			"This tanned hide floor rug is a medium-sized, workmanlike rug made from cured animal skin. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1900.0,
			14.0m,
			true,
			false,
			"animal skin",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Flat tanned hide used as a simple floor covering."
		);

		CreateItem(
			"medieval_decor_household_wall_banner",
			"banner",
			"a household wall banner",
			null,
			"This household wall banner is a medium-sized, well-made banner made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1100.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Decorative banner for a hall or chamber; skins can define exact symbols or colours."
		);

		CreateItem(
			"medieval_decor_ivory_inlay_panel",
			"panel",
			"an ivory-inlaid panel",
			null,
			"This ivory-inlaid panel is a small, well-made panel worked from ivory. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.Small,
			ItemQuality.Good,
			650.0,
			96.0m,
			true,
			false,
			"ivory",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Small decorative panel with pale inlay pieces set into a darker backing."
		);

		CreateItem(
			"medieval_decor_large_hall_carpet",
			"carpet",
			"a large hall carpet",
			null,
			"This large hall carpet is a large, well-made carpet made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			7200.0,
			72.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Large heavy carpet for a hall, solar, or public receiving room."
		);

		CreateItem(
			"medieval_decor_linen_floor_cloth",
			"cloth",
			"a plain linen floor cloth",
			null,
			"This plain linen floor cloth is a medium-sized, workmanlike cloth made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			8.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Washable floor cloth for a clean chamber, nursery, or work corner."
		);

		CreateItem(
			"medieval_decor_linen_pillow_cover",
			"cover",
			"a linen pillow cover",
			null,
			"This linen pillow cover is a small, workmanlike cover made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			140.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Decorative removable cover for a pillow or cushion, without any wearable behaviour."
		);

		CreateItem(
			"medieval_decor_linen_wall_curtain",
			"curtain",
			"a linen wall curtain",
			null,
			"This linen wall curtain is a large, workmanlike curtain made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1500.0,
			18.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Loose curtain intended for a wall, alcove, or window area rather than a doorway."
		);

		CreateItem(
			"medieval_decor_narrow_hall_runner",
			"runner",
			"a narrow wool hall runner",
			null,
			"This narrow wool hall runner is a large, workmanlike runner made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2600.0,
			22.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Long narrow floor textile for passages, aisles, and the space beside benches or beds."
		);

		CreateItem(
			"medieval_decor_padded_floor_throw",
			"throw",
			"a padded floor throw",
			null,
			"This padded floor throw is a medium-sized, workmanlike throw made from woven wool. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1700.0,
			18.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Soft layered textile that can be spread on a floor or platform for comfort."
		);

		CreateItem(
			"medieval_decor_padded_seat_cover",
			"cover",
			"a padded seat cover",
			null,
			"This padded seat cover is a small, workmanlike cover made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			500.0,
			8.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Soft cover for an existing chair or bench; it has no chair component by itself."
		);

		CreateItem(
			"medieval_decor_painted_ceiling_panel",
			"panel",
			"a painted ceiling panel",
			null,
			"This painted ceiling panel is a medium-sized, well-made panel built from pine boards. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2400.0,
			34.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Light painted panel intended for overhead or high-wall decoration."
		);

		CreateItem(
			"medieval_decor_painted_pine_panel",
			"panel",
			"a painted pine wall panel",
			null,
			"This painted pine wall panel is a medium-sized, workmanlike panel built from pine boards. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2600.0,
			22.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Light timber panel with simple painted border work."
		);

		CreateItem(
			"medieval_decor_painted_screen_panel",
			"screen",
			"a painted screen panel",
			null,
			"This painted screen panel is a large, well-made screen built from pine boards. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			3800.0,
			46.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Freestanding painted panel for a room corner, alcove, or private chamber."
		);

		CreateItem(
			"medieval_decor_painted_wall_cloth",
			"cloth",
			"a painted wall cloth",
			null,
			"This painted wall cloth is a large, well-made cloth made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			1400.0,
			36.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Linen wall cloth painted with faded decorative shapes and borders."
		);

		CreateItem(
			"medieval_decor_plain_table_cover",
			"cover",
			"a plain linen table cover",
			null,
			"This plain linen table cover is a medium-sized, workmanlike cover made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			700.0,
			8.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Simple cover for a table, chest top, or serving board."
		);

		CreateItem(
			"medieval_decor_plain_wool_wall_hanging",
			"hanging",
			"a plain wool wall hanging",
			null,
			"This plain wool wall hanging is a large, workmanlike hanging made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2100.0,
			20.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Plain wall textile for warmth, decoration, and softening bare walls."
		);

		CreateItem(
			"medieval_decor_plaster_wall_plaque",
			"plaque",
			"a plaster wall plaque",
			null,
			"This plaster wall plaque is a small, workmanlike plaque moulded from plaster. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			8.0m,
			true,
			false,
			"plaster",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Cast plaster plaque for household wall decoration."
		);

		CreateItem(
			"medieval_decor_polished_bronze_wall_mirror",
			"mirror",
			"a polished bronze wall mirror",
			null,
			"This polished bronze wall mirror is a small, well-made mirror worked from bronze. A polished reflective face sits inside a raised frame, with a small loop set at the back. The edge of the frame is rubbed where it has been handled. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			48.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Polished bronze mirror plate in a simple hanging frame."
		);

		CreateItem(
			"medieval_decor_portable_display_screen",
			"screen",
			"a portable display screen",
			null,
			"This portable display screen is a large, well-made screen built from walnut boards. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			7200.0,
			86.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Good-quality freestanding screen with a polished wood frame."
		);

		CreateItem(
			"medieval_decor_room_divider_hanging",
			"hanging",
			"a room-divider hanging",
			null,
			"This room-divider hanging is a large, workmanlike hanging made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2600.0,
			28.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Large textile divider for a chamber or workroom; it has no door or movement-blocking behaviour."
		);

		CreateItem(
			"medieval_decor_round_floor_mat",
			"mat",
			"a round woven floor mat",
			null,
			"This round woven floor mat is a medium-sized, workmanlike mat made from woven hemp. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1300.0,
			8.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Round plaited mat for a small table, stool cluster, or bare floor."
		);

		CreateItem(
			"medieval_decor_shelf_cover_cloth",
			"cloth",
			"a shelf cover cloth",
			null,
			"This shelf cover cloth is a small, workmanlike cloth made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Small cloth for dressing a shelf, chest top, or side table."
		);

		CreateItem(
			"medieval_decor_silk_table_cover",
			"cover",
			"a fine silk table cover",
			null,
			"This fine silk table cover is a medium-sized, well-made cover made from woven silk. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Good,
			520.0,
			64.0m,
			true,
			false,
			"silk",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Fine table cover for an elite chamber, feast setting, or display surface."
		);

		CreateItem(
			"medieval_decor_silk_wall_hanging",
			"hanging",
			"a silk wall hanging",
			null,
			"This silk wall hanging is a large, well-made hanging made from woven silk. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			1200.0,
			92.0m,
			true,
			false,
			"silk",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Fine wall hanging with a smooth lustrous surface and narrow worked edging."
		);

		CreateItem(
			"medieval_decor_small_bedside_mat",
			"mat",
			"a small bedside floor mat",
			null,
			"This small bedside floor mat is a small, workmanlike mat made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			6.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Small soft mat for the side of a bed, cot, pallet, or sleeping platform."
		);

		CreateItem(
			"medieval_decor_small_chair_throw",
			"throw",
			"a small chair throw",
			null,
			"This small chair throw is a small, workmanlike throw made from woven wool. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			600.0,
			8.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Small textile throw for a chair, stool, or high seat."
		);

		CreateItem(
			"medieval_decor_story_woven_tapestry",
			"tapestry",
			"a story-woven tapestry",
			null,
			"This story-woven tapestry is a large, well-made tapestry made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			3600.0,
			96.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Large figured tapestry intended for narrative, heraldic, household, or fantasy-world skin variants."
		);

		CreateItem(
			"medieval_decor_striped_wool_carpet",
			"carpet",
			"a striped wool carpet",
			null,
			"This striped wool carpet is a large, well-made carpet made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			5000.0,
			46.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Broad carpet with alternating woven bands."
		);

		CreateItem(
			"medieval_decor_willow_plaited_mat",
			"mat",
			"a willow-plaited floor mat",
			null,
			"This willow-plaited floor mat is a medium-sized, workmanlike mat built from willow boards. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1500.0,
			8.0m,
			true,
			false,
			"willow",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Flexible willow mat with tight plaiting and a plain border."
		);

		CreateItem(
			"medieval_decor_window_hanging",
			"hanging",
			"a small window hanging",
			null,
			"This small window hanging is a medium-sized, workmanlike hanging made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			800.0,
			9.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Short light hanging for a window, alcove, or wall opening without door behaviour."
		);

		CreateItem(
			"medieval_decor_winter_wall_drape",
			"drape",
			"a heavy winter wall drape",
			null,
			"This heavy winter wall drape is a large, workmanlike drape made from woven wool. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			3900.0,
			34.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Heavy wool drape for colder rooms and drafty walls."
		);

		CreateItem(
			"medieval_decor_wooden_folding_screen",
			"screen",
			"a wooden folding screen",
			null,
			"This wooden folding screen is a large, workmanlike screen built from pine boards. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			6200.0,
			42.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Hinged wooden screen for decorative room division; it has no door or cover component."
		);

		CreateItem(
			"medieval_decor_wool_bench_cover",
			"cover",
			"a wool bench cover",
			null,
			"This wool bench cover is a medium-sized, workmanlike cover made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1100.0,
			12.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Long wool cover for a bench or settle; it does not provide seating behaviour by itself."
		);

		CreateItem(
			"medieval_decor_wool_stair_mat",
			"mat",
			"a narrow stair mat",
			null,
			"This narrow stair mat is a medium-sized, workmanlike mat made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1300.0,
			14.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Narrow textile mat suited to steps, thresholds, and raised platforms without acting as a door."
		);

		CreateItem(
			"medieval_decor_woven_bed_coverlet",
			"coverlet",
			"a woven bed coverlet",
			null,
			"This woven bed coverlet is a large, workmanlike coverlet made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2400.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Decorative coverlet for a bed, pallet, or sleeping platform."
		);

		CreateItem(
			"medieval_decor_woven_floor_carpet",
			"carpet",
			"a woven wool floor carpet",
			null,
			"This woven wool floor carpet is a large, workmanlike carpet made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			4800.0,
			38.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"General large floor textile for halls, chambers, and better household rooms."
		);

		CreateItem(
			"medieval_decor_woven_tapestry_strip",
			"tapestry",
			"a woven tapestry strip",
			null,
			"This woven tapestry strip is a large, well-made tapestry made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			2300.0,
			58.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Long narrow tapestry strip for a wall, canopy edge, or high chamber."
		);

		CreateItem(
			"medieval_decor_northern_antler_wall_piece",
			"plaque",
			"an antler wall piece",
			null,
			"This antler wall piece is a small, workmanlike plaque worked from antler. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			16.0m,
			true,
			false,
			"antler",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Decorative antler piece mounted or tied for household display."
		);

		CreateItem(
			"medieval_decor_northern_beast_carved_panel",
			"panel",
			"a beast-carved wall panel",
			null,
			"This beast-carved wall panel is a medium-sized, well-made panel built from oak boards. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			4200.0,
			50.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Oak panel carved with interlaced animal forms."
		);

		CreateItem(
			"medieval_decor_northern_canvas_room_hanging",
			"hanging",
			"a heavy canvas room hanging",
			null,
			"This heavy canvas room hanging is a large, workmanlike hanging made from coarse canvas. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2500.0,
			18.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Sturdy hanging suited to a workshop, shipboard space, or plain household room."
		);

		CreateItem(
			"medieval_decor_northern_checked_bed_cover",
			"cover",
			"a checked wool bed cover",
			null,
			"This checked wool bed cover is a large, workmanlike cover made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2600.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Checked wool cover suited to colder sleeping rooms."
		);

		CreateItem(
			"medieval_decor_northern_felt_winter_mat",
			"mat",
			"a thick winter felt mat",
			null,
			"This thick winter felt mat is a medium-sized, workmanlike mat made from pressed felt. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			13.0m,
			true,
			false,
			"felt",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Dense felt mat for a cold floor or raised sleeping platform."
		);

		CreateItem(
			"medieval_decor_northern_fur_wall_hanging",
			"hanging",
			"a fur-backed wall hanging",
			null,
			"This fur-backed wall hanging is a large, well-made hanging made from dressed fur. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Large,
			ItemQuality.Good,
			3800.0,
			76.0m,
			true,
			false,
			"fur",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Thick wall hanging with fur backing for a winter chamber."
		);

		CreateItem(
			"medieval_decor_northern_hide_bench_cover",
			"cover",
			"a hide bench cover",
			null,
			"This hide bench cover is a medium-sized, workmanlike cover made from cured animal skin. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1300.0,
			14.0m,
			true,
			false,
			"animal skin",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Plain tanned hide cover for a bench, chest, or sleeping ledge."
		);

		CreateItem(
			"medieval_decor_northern_tablet_wall_hanging",
			"hanging",
			"a tablet-banded wall hanging",
			null,
			"This tablet-banded wall hanging is a large, well-made hanging made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			2400.0,
			48.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Heavy wool hanging with tablet-woven bands along the edges."
		);

		CreateItem(
			"medieval_decor_western_embroidered_bed_valance",
			"valance",
			"an embroidered bed valance",
			null,
			"This embroidered bed valance is a medium-sized, well-made valance made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Good,
			720.0,
			30.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Worked textile valance for a bed, curtain rail, or canopy edge."
		);

		CreateItem(
			"medieval_decor_western_flowered_tapestry",
			"tapestry",
			"a flowered wall tapestry",
			null,
			"This flowered wall tapestry is a large, well-made tapestry made from woven wool. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			3300.0,
			90.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Fine wall tapestry with small repeated floral forms."
		);

		CreateItem(
			"medieval_decor_western_household_banner",
			"banner",
			"a painted household banner",
			null,
			"This painted household banner is a medium-sized, well-made banner made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Good,
			900.0,
			28.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Painted banner for a chamber, hall, guild room, or noble household skin."
		);

		CreateItem(
			"medieval_decor_western_linen_painted_cloth",
			"cloth",
			"a vine-painted linen cloth",
			null,
			"This vine-painted linen cloth is a large, well-made cloth made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			1300.0,
			38.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Painted linen cloth with curling vine and border decoration."
		);

		CreateItem(
			"medieval_decor_western_oak_tracery_screen",
			"screen",
			"an oak tracery screen",
			null,
			"This oak tracery screen is a large, well-made screen built from oak boards. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			7800.0,
			88.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Open carved oak screen with repeated arched tracery."
		);

		CreateItem(
			"medieval_decor_western_stained_glass_panel",
			"panel",
			"a stained glass panel",
			null,
			"This stained glass panel is a medium-sized, well-made panel made from glass. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3400.0,
			110.0m,
			true,
			false,
			"glass",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			"Decorative coloured-glass panel in a portable frame."
		);

		CreateItem(
			"medieval_decor_western_walnut_carved_panel",
			"panel",
			"a carved walnut panel",
			null,
			"This carved walnut panel is a medium-sized, well-made panel built from walnut boards. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			58.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Walnut panel with crisp carving and a polished surface."
		);

		CreateItem(
			"medieval_decor_western_wool_dais_carpet",
			"carpet",
			"a wool dais carpet",
			null,
			"This wool dais carpet is a large, well-made carpet made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			4200.0,
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
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Good wool carpet sized for a raised table, seat, or chamber platform."
		);

		CreateItem(
			"medieval_decor_mediterranean_bright_floor_carpet",
			"carpet",
			"a bright wool floor carpet",
			null,
			"This bright wool floor carpet is a large, well-made carpet made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			4700.0,
			54.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Brightly woven carpet suited to a warm household chamber."
		);

		CreateItem(
			"medieval_decor_mediterranean_bronze_round_mirror",
			"mirror",
			"a round bronze mirror",
			null,
			"This round bronze mirror is a small, well-made mirror worked from bronze. A polished reflective face sits inside a raised frame, with a small loop set at the back. The edge of the frame is rubbed where it has been handled. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			850.0,
			50.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Round polished bronze mirror with a simple rim."
		);

		CreateItem(
			"medieval_decor_mediterranean_linen_courtyard_curtain",
			"curtain",
			"a linen courtyard curtain",
			null,
			"This linen courtyard curtain is a large, workmanlike curtain made from woven linen. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1300.0,
			18.0m,
			true,
			false,
			"linen",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Light curtain for an indoor wall, courtyard shade, or windowed room."
		);

		CreateItem(
			"medieval_decor_mediterranean_marble_relief_plaque",
			"plaque",
			"a marble relief plaque",
			null,
			"This marble relief plaque is a small, well-made plaque cut from marble. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Small,
			ItemQuality.Good,
			2800.0,
			90.0m,
			true,
			false,
			"marble",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Small marble plaque carved in shallow relief."
		);

		CreateItem(
			"medieval_decor_mediterranean_mosaic_tile_panel",
			"panel",
			"a mosaic tile panel",
			null,
			"This mosaic tile panel is a medium-sized, well-made panel formed from ceramic. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			6200.0,
			70.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Small mosaic panel made from fitted coloured ceramic pieces."
		);

		CreateItem(
			"medieval_decor_mediterranean_olivewood_screen",
			"screen",
			"an olivewood privacy screen",
			null,
			"This olivewood privacy screen is a large, well-made screen built from olive wood. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			6900.0,
			76.0m,
			true,
			false,
			"olive wood",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Polished olivewood screen with warm grain and light carving."
		);

		CreateItem(
			"medieval_decor_mediterranean_painted_plaster_panel",
			"panel",
			"a painted plaster panel",
			null,
			"This painted plaster panel is a medium-sized, well-made panel moulded from plaster. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3400.0,
			34.0m,
			true,
			false,
			"plaster",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Painted plaster panel with warm-toned border decoration."
		);

		CreateItem(
			"medieval_decor_mediterranean_terracotta_wall_tiles",
			"tile",
			"a set of terracotta wall tiles",
			null,
			"This set of terracotta wall tiles is a medium-sized, workmanlike tile formed from terracotta. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5400.0,
			26.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Matched terracotta tiles for decorative household wall display."
		);

		CreateItem(
			"medieval_decor_islamicate_brass_wall_mirror",
			"mirror",
			"a polished brass wall mirror",
			null,
			"This polished brass wall mirror is a small, well-made mirror worked from brass. A polished reflective face sits inside a raised frame, with a small loop set at the back. The edge of the frame is rubbed where it has been handled. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1000.0,
			52.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Polished brass mirror plate with a decorated rim."
		);

		CreateItem(
			"medieval_decor_islamicate_cedar_lattice_screen",
			"screen",
			"a cedar lattice screen",
			null,
			"This cedar lattice screen is a large, well-made screen built from cedar boards. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			6400.0,
			84.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Carved cedar screen with repeated lattice openings."
		);

		CreateItem(
			"medieval_decor_islamicate_cotton_canopy_cloth",
			"cloth",
			"a cotton canopy cloth",
			null,
			"This cotton canopy cloth is a large, well-made cloth made from woven cotton. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			1200.0,
			34.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Light canopy cloth for a bed, low platform, or shaded chamber."
		);

		CreateItem(
			"medieval_decor_islamicate_cotton_window_curtain",
			"curtain",
			"a cotton window curtain",
			null,
			"This cotton window curtain is a medium-sized, workmanlike curtain made from woven cotton. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			850.0,
			12.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Light cotton curtain for an inner window or alcove."
		);

		CreateItem(
			"medieval_decor_islamicate_flatwoven_floor_rug",
			"rug",
			"a flatwoven wool floor rug",
			null,
			"This flatwoven wool floor rug is a medium-sized, well-made rug made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2100.0,
			30.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Flatwoven rug with narrow bands and crisp edge work."
		);

		CreateItem(
			"medieval_decor_islamicate_geometric_carpet",
			"carpet",
			"a geometric wool carpet",
			null,
			"This geometric wool carpet is a large, well-made carpet made from woven wool. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			5200.0,
			68.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Wool carpet with repeated geometric field and border designs."
		);

		CreateItem(
			"medieval_decor_islamicate_glazed_tile_panel",
			"panel",
			"a glazed ceramic tile panel",
			null,
			"This glazed ceramic tile panel is a medium-sized, well-made panel formed from ceramic. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			5600.0,
			74.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Glossy tile panel with fitted decorative shapes."
		);

		CreateItem(
			"medieval_decor_islamicate_pierced_brass_screen",
			"screen",
			"a pierced brass screen",
			null,
			"This pierced brass screen is a large, well-made screen worked from brass. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Large,
			ItemQuality.Good,
			9000.0,
			130.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Heavy brass screen with pierced decorative work."
		);

		CreateItem(
			"medieval_decor_islamicate_silk_room_divider",
			"divider",
			"a silk room divider",
			null,
			"This silk room divider is a large, well-made divider made from woven silk. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			1400.0,
			98.0m,
			true,
			false,
			"silk",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Fine silk divider hanging for a chamber, alcove, or reception room."
		);

		CreateItem(
			"medieval_decor_islamicate_star_wall_hanging",
			"hanging",
			"a star-patterned wall hanging",
			null,
			"This star-patterned wall hanging is a large, well-made hanging made from woven cotton. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			1700.0,
			42.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Cotton wall hanging with star and banded geometric decoration."
		);

		CreateItem(
			"medieval_decor_south_asian_bamboo_window_blind",
			"blind",
			"a bamboo window blind",
			null,
			"This bamboo window blind is a medium-sized, workmanlike blind built from split bamboo. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			12.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Light bamboo blind for a window, veranda, or room opening without door behaviour."
		);

		CreateItem(
			"medieval_decor_south_asian_block_printed_floor_cloth",
			"cloth",
			"a block-printed floor cloth",
			null,
			"This block-printed floor cloth is a large, well-made cloth made from woven cotton. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			1400.0,
			34.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Cotton floor cloth with repeated block-printed designs."
		);

		CreateItem(
			"medieval_decor_south_asian_bronze_mirror",
			"mirror",
			"a bronze household mirror",
			null,
			"This bronze household mirror is a small, well-made mirror worked from bronze. A polished reflective face sits inside a raised frame, with a small loop set at the back. The edge of the frame is rubbed where it has been handled. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			820.0,
			48.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Polished bronze mirror with a simple backing loop."
		);

		CreateItem(
			"medieval_decor_south_asian_cotton_bed_valance",
			"valance",
			"a cotton bed valance",
			null,
			"This cotton bed valance is a medium-sized, workmanlike valance made from woven cotton. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			650.0,
			14.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Cotton valance for dressing a bed frame, platform, or low sleeping space."
		);

		CreateItem(
			"medieval_decor_south_asian_painted_wall_cloth",
			"cloth",
			"a painted cotton wall cloth",
			null,
			"This painted cotton wall cloth is a large, well-made cloth made from woven cotton. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			1300.0,
			36.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Painted cotton cloth for a wall, alcove, or household chamber."
		);

		CreateItem(
			"medieval_decor_south_asian_sandalwood_panel",
			"panel",
			"a sandalwood wall panel",
			null,
			"This sandalwood wall panel is a small, well-made panel built from sandalwood. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			950.0,
			82.0m,
			true,
			false,
			"sandalwood",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Small fragrant sandalwood panel carved for household display."
		);

		CreateItem(
			"medieval_decor_south_asian_silk_canopy_cloth",
			"cloth",
			"a silk canopy cloth",
			null,
			"This silk canopy cloth is a large, well-made cloth made from woven silk. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			900.0,
			96.0m,
			true,
			false,
			"silk",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Fine silk canopy cloth for a bed, platform, or elite chamber."
		);

		CreateItem(
			"medieval_decor_south_asian_teak_carved_screen",
			"screen",
			"a carved teak screen",
			null,
			"This carved teak screen is a large, well-made screen built from teak boards. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			7600.0,
			92.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Teak screen with carved pierced panels."
		);

		CreateItem(
			"medieval_decor_east_asian_bamboo_floor_mat",
			"mat",
			"a woven bamboo floor mat",
			null,
			"This woven bamboo floor mat is a medium-sized, workmanlike mat built from split bamboo. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1300.0,
			12.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Firm woven bamboo mat for a low room, entry, or platform."
		);

		CreateItem(
			"medieval_decor_east_asian_bamboo_window_screen",
			"screen",
			"a bamboo window screen",
			null,
			"This bamboo window screen is a medium-sized, workmanlike screen built from split bamboo. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			18.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Bamboo screen for filtering light in a window or room edge."
		);

		CreateItem(
			"medieval_decor_east_asian_cypress_frame_screen",
			"screen",
			"a cypress frame screen",
			null,
			"This cypress frame screen is a large, well-made screen built from cypress boards. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			4200.0,
			64.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Cypress-framed screen with light panels and careful joinery."
		);

		CreateItem(
			"medieval_decor_east_asian_painted_paper_panel",
			"panel",
			"a painted paper wall panel",
			null,
			"This painted paper wall panel is a medium-sized, well-made panel made from layered paper. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
			SizeCategory.Normal,
			ItemQuality.Good,
			300.0,
			24.0m,
			true,
			false,
			"paper",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Painted paper panel meant for display on a wall or screen frame."
		);

		CreateItem(
			"medieval_decor_east_asian_paper_folding_screen",
			"screen",
			"a paper folding screen",
			null,
			"This paper folding screen is a large, well-made screen made from layered paper. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
			SizeCategory.Large,
			ItemQuality.Good,
			2800.0,
			52.0m,
			true,
			false,
			"paper",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Light folding screen with paper panels in a wooden frame."
		);

		CreateItem(
			"medieval_decor_east_asian_paper_room_divider",
			"divider",
			"a paper room divider",
			null,
			"This paper room divider is a large, workmanlike divider made from layered paper. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1800.0,
			30.0m,
			true,
			false,
			"paper",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null,
			"Light paper-panel divider for organising a room; it has no door behaviour."
		);

		CreateItem(
			"medieval_decor_east_asian_pine_display_panel",
			"panel",
			"a pine display panel",
			null,
			"This pine display panel is a medium-sized, workmanlike panel built from pine boards. The front face is carved and decorated, while the back is plainer and flatter. Small fixing marks show how it sits against a wall. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2300.0,
			22.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Plain display panel with a simple timber frame."
		);

		CreateItem(
			"medieval_decor_east_asian_porcelain_screen_tile",
			"tile",
			"a porcelain screen tile",
			null,
			"This porcelain screen tile is a small, well-made tile formed from porcelain. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			38.0m,
			true,
			false,
			"porcelain",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Smooth porcelain tile with painted decoration for a screen, wall, or cabinet face."
		);

		CreateItem(
			"medieval_decor_east_asian_silk_bed_curtain",
			"curtain",
			"a silk bed curtain",
			null,
			"This silk bed curtain is a large, well-made curtain made from woven silk. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			900.0,
			86.0m,
			true,
			false,
			"silk",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Light silk curtain for a sleeping alcove or elite chamber."
		);

		CreateItem(
			"medieval_decor_east_asian_silk_scroll_hanging",
			"hanging",
			"a silk scroll hanging",
			null,
			"This silk scroll hanging is a medium-sized, well-made hanging made from woven silk. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Good,
			450.0,
			58.0m,
			true,
			false,
			"silk",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Vertical silk hanging mounted on a light roller; it carries decorative rather than readable text."
		);

		CreateItem(
			"medieval_decor_steppe_embroidered_felt_screen",
			"screen",
			"an embroidered felt screen",
			null,
			"This embroidered felt screen is a large, well-made screen made from pressed felt. Joined panels form a standing face, with hinged joints between the sections. The visible side carries the stronger patterning. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			3600.0,
			52.0m,
			true,
			false,
			"felt",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Soft felt screen with stitched decoration for a tent or household chamber."
		);

		CreateItem(
			"medieval_decor_steppe_felt_floor_carpet",
			"carpet",
			"a felt floor carpet",
			null,
			"This felt floor carpet is a large, workmanlike carpet made from pressed felt. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			4300.0,
			32.0m,
			true,
			false,
			"felt",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Thick felt carpet suited to tents, wagons, and portable households."
		);

		CreateItem(
			"medieval_decor_steppe_felt_wall_hanging",
			"hanging",
			"a felt wall hanging",
			null,
			"This felt wall hanging is a large, workmanlike hanging made from pressed felt. A reinforced top edge carries stitched tabs along the upper hem. The visible face is bordered, with the lower hem weighted enough to fall straight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2500.0,
			24.0m,
			true,
			false,
			"felt",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Felt wall hanging for warmth and decoration in a portable dwelling."
		);

		CreateItem(
			"medieval_decor_steppe_fur_bedding_throw",
			"throw",
			"a fur bedding throw",
			null,
			"This fur bedding throw is a large, well-made throw made from dressed fur. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Large,
			ItemQuality.Good,
			3100.0,
			60.0m,
			true,
			false,
			"fur",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Warm fur throw for a bedroll, sleeping platform, or display seat."
		);

		CreateItem(
			"medieval_decor_steppe_hide_floor_mat",
			"mat",
			"a hide floor mat",
			null,
			"This hide floor mat is a medium-sized, workmanlike mat made from cured animal skin. The upper face carries a woven pattern, while the underside is flatter and more tightly worn. The edges are bound to stop the fabric from fraying. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1700.0,
			12.0m,
			true,
			false,
			"animal skin",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Practical hide mat for a tent floor, wagon bed, or camp shelter."
		);

		CreateItem(
			"medieval_decor_steppe_leather_tent_liner",
			"liner",
			"a leather tent liner",
			null,
			"This leather tent liner is a large, workmanlike liner made from worked leather. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Large,
			ItemQuality.Standard,
			5200.0,
			42.0m,
			true,
			false,
			"leather",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Leather liner panel for a tent or travel shelter; it is decorative and protective, not a door."
		);

		CreateItem(
			"medieval_decor_steppe_travel_room_divider",
			"divider",
			"a travel room divider",
			null,
			"This travel room divider is a large, workmanlike divider made from pressed felt. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2800.0,
			28.0m,
			true,
			false,
			"felt",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Folded felt divider for temporary rooms and travel households."
		);

		CreateItem(
			"medieval_decor_steppe_wool_tent_band",
			"band",
			"a woven wool tent band",
			null,
			"This woven wool tent band is a large, workmanlike band made from woven wool. The visible face carries most of the colour and texture, while the back is plainer. The edges are finished neatly for display inside a room. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			true,
			false,
			"wool",
			[
				"Functions / Household Items / Household Decorations",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Long decorative band for a tent wall, storage rail, or portable screen."
		);
	}
}
