using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private sealed record AntiquityComponentGapItemSpec(
		string StableReference,
		string Noun,
		string ShortDescription,
		string FullDescription,
		SizeCategory Size,
		ItemQuality Quality,
		double WeightInGrams,
		decimal Cost,
		string Material,
		MaterialBehaviourType MaterialType,
		string[] Tags,
		string[] Components,
		string? BuilderNotes = null);

	private void SeedAntiquityComponentGapItems()
	{
		EnsureAntiquityComponentGapWritingComponents();

		foreach (var spec in AntiquityComponentGapItemSpecs())
		{
			EnsureAntiquityComponentGapMaterial(spec.Material, spec.MaterialType);
			foreach (var tag in spec.Tags)
			{
				EnsureAntiquityTagPath(tag);
			}

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
				spec.BuilderNotes ?? "Seeded from the antiquity item component gap report.");
		}
	}

	internal void SeedAntiquityComponentGapItemsForTesting(FuturemudDatabaseContext context)
	{
		if (!ReferenceEquals(_context, context))
		{
			_context = context;
			InitialiseDependencies();
		}

		SeedAntiquityComponentGapItems();
		_context!.SaveChanges();
	}

	internal static IReadOnlyCollection<string> AntiquityComponentGapItemStableReferencesForTesting =>
		AntiquityComponentGapItemSpecs().Select(x => x.StableReference).ToArray();

	private void EnsureAntiquityComponentGapWritingComponents()
	{
		EnsureAntiquityPaperSheetComponent("Antiquity_Papyrus_Scroll_Surface",
			"Allows an antiquity papyrus scroll to be written on as a long sheet", 12000);
		EnsureAntiquityPaperSheetComponent("Antiquity_Papyrus_Sheet_Surface",
			"Allows an antiquity papyrus sheet to be written on", 2400);
		EnsureAntiquityInscribableSurfaceComponent("Antiquity_Clay_Tablet_Surface",
			"Allows clay tablets to take stylus writing", 1600, WritingImplementType.Stylus);
		EnsureAntiquityInscribableSurfaceComponent("Antiquity_Wooden_Block_Surface",
			"Allows wooden boards and blocks to take incised or charcoal writing", 2200,
			WritingImplementType.Stylus, WritingImplementType.Chisel, WritingImplementType.Charcoal);
	}

	private Material EnsureAntiquityComponentGapMaterial(string name, MaterialBehaviourType behaviourType)
	{
		if (_materials.TryGetValue(name, out var existing))
		{
			return existing;
		}

		existing = _context!.Materials.Local
			.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ??
			_context.Materials.AsEnumerable()
			        .FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		if (existing is not null)
		{
			_materials[name] = existing;
			return existing;
		}

		var material = new Material
		{
			Id = NextMaterialId(),
			Name = name,
			MaterialDescription = name.ToLowerInvariant(),
			BehaviourType = (int)behaviourType,
			ResidueSdesc = string.Empty,
			ResidueDesc = string.Empty,
			ResidueColour = string.Empty
		};
		_context.Materials.Add(material);
		_materials[name] = material;
		return material;
	}

	private long NextMaterialId()
	{
		var existing = _context!.Materials.Any() ? _context.Materials.Max(x => x.Id) : 0L;
		var local = _context.Materials.Local.Any() ? _context.Materials.Local.Max(x => x.Id) : 0L;
		return Math.Max(existing, local) + 1L;
	}

	private static IReadOnlyList<AntiquityComponentGapItemSpec> AntiquityComponentGapItemSpecs()
	{
		const string ToolTag = "Market / Professional Tools / Standard Tools";
		const string MeasureTag = "Functions / Tools / Measurement Tools";
		const string TimeTag = "Functions / Tools / Timekeeping Tools";
		const string WritingTag = "Market / Writing Materials";
		const string CivicTag = "Functions / Civic Fixtures";
		const string MarketTag = "Market / Household Goods / Standard Wares";
		const string FurnitureTag = "Market / Household Goods / Standard Furniture";
		const string ReligiousTag = "Market / Religious Goods";
		const string SecurityTag = "Functions / Security Tools";
		const string LeisureTag = "Functions / Household Items / Leisure Goods";
		const string MedicalTag = "Functions / Medical Items";
		const string IncenseFuelTag = "Functions / Household Items / Household Religious Items / Incense Fuel";
		const string RidingTackTag = "Functions / Animal Equipment / Riding Tack";
		const string PackGearTag = "Functions / Animal Equipment / Pack Gear";
		const string DraftGearTag = "Functions / Animal Equipment / Draft Gear";
		const string AnimalArmourTag = "Functions / Animal Equipment / Animal Armour";

		return
		[
			new("antiquity_stone_courtyard_sundial", "sundial", "a carved stone courtyard sundial",
				"This heavy courtyard sundial is carved from pale stone, with a fixed gnomon and hour marks cut deeply enough to remain legible in hard public weather.",
				SizeCategory.VeryLarge, ItemQuality.Standard, 120000.0, 65.0m, "stone", MaterialBehaviourType.Stone,
				[ToolTag, TimeTag, CivicTag], ["TimePiece_Antiquity_Sundial", "Destroyable_Misc"]),
			new("antiquity_bronze_portable_gnomon", "gnomon", "a bronze portable gnomon",
				"This small bronze gnomon folds into a simple travelling frame. Priests, surveyors, officers, and merchants can set it upright to judge the hour by sun shadow.",
				SizeCategory.Small, ItemQuality.Standard, 450.0, 24.0m, "bronze", MaterialBehaviourType.Metal,
				[ToolTag, TimeTag], ["Holdable", "TimePiece_Antiquity_Sundial", "Destroyable_HeavyMetal"]),
			new("antiquity_marked_watch_candle", "candle", "a marked wax watch candle",
				"This thick wax candle is scored with dark watch marks. Even unlit, its careful divisions make it a useful temple, guard, or household time marker.",
				SizeCategory.Small, ItemQuality.Standard, 320.0, 6.0m, "wax", MaterialBehaviourType.Wax,
				[ToolTag, TimeTag], ["Holdable", "TimePiece_Antiquity_MarkedCandle", "Destroyable_Misc"]),
			new("antiquity_temple_water_clock", "clock", "a clay temple water clock",
				"This clay water clock has a marked inner wall and a narrow drain hole. Its weathered basin is suited to temple, court, or watch-house timekeeping.",
				SizeCategory.Large, ItemQuality.Standard, 18500.0, 48.0m, "clay", MaterialBehaviourType.Ceramic,
				[ToolTag, TimeTag, ReligiousTag], ["TimePiece_Antiquity_WaterClock", "Destroyable_Misc"]),
			new("antiquity_watchman_hour_board", "board", "a painted watchman's hour board",
				"This wooden board lists the night watches in painted bands, with room for fresh marks or duty notes beside each hour.",
				SizeCategory.Large, ItemQuality.Standard, 6200.0, 18.0m, "oak", MaterialBehaviourType.Wood,
				[ToolTag, TimeTag, CivicTag], ["TimePiece_Antiquity_WatchBoard", "Antiquity_Wooden_Block_Surface", "Destroyable_WoodenHeavy"]),

			new("antiquity_stone_public_well", "well", "a stone public well",
				"This public well is built from fitted stone blocks around a dark shaft, with worn lip marks from ropes, buckets, and daily civic use.",
				SizeCategory.Huge, ItemQuality.Standard, 450000.0, 150.0m, "stone", MaterialBehaviourType.Stone,
				[FurnitureTag, CivicTag], ["WaterSource_Antiquity_PublicWell", "Destroyable_Furniture"]),
			new("antiquity_lined_cistern", "cistern", "a lined stone cistern",
				"This lined stone cistern has a plastered interior and a heavy rim, built to store household, fortress, or courtyard water through dry weather.",
				SizeCategory.Huge, ItemQuality.Standard, 600000.0, 180.0m, "stone", MaterialBehaviourType.Stone,
				[FurnitureTag, CivicTag], ["WaterSource_Antiquity_Cistern", "Destroyable_Furniture"]),
			new("antiquity_bronze_fountain_spout", "spout", "a bronze fountain spout",
				"This bronze fountain spout is shaped for a wall or basin, its mouth polished bright where water would sheet into a public fountain.",
				SizeCategory.Normal, ItemQuality.Good, 2400.0, 55.0m, "bronze", MaterialBehaviourType.Metal,
				[FurnitureTag, CivicTag], ["WaterSource_Antiquity_Fountain", "Destroyable_HeavyMetal"]),
			new("antiquity_bathhouse_plunge_pool", "pool", "a tiled bathhouse plunge pool",
				"This tiled plunge pool is broad, cold, and public-facing, with a low rim and stained water line from repeated bathhouse use.",
				SizeCategory.Huge, ItemQuality.Good, 900000.0, 260.0m, "tile", MaterialBehaviourType.Ceramic,
				[FurnitureTag, CivicTag], ["WaterSource_Antiquity_BathPool", "Destroyable_Furniture"]),
			new("antiquity_temple_purification_basin", "basin", "a temple purification basin",
				"This temple basin is shallow and carefully cleaned, with a smooth stone lip suited to ritual washing before prayer, sacrifice, or official entry.",
				SizeCategory.Large, ItemQuality.Good, 42000.0, 70.0m, "stone", MaterialBehaviourType.Stone,
				[ReligiousTag, CivicTag], ["WaterSource_Antiquity_RitualBasin", "Destroyable_Misc"]),
			new("antiquity_bronze_incense_censer", "censer", "a bronze incense censer",
				"This pierced bronze censer has a handled bowl and darkened vents for fragrant resin smoke, suited to household shrines, rites, and processions.",
				SizeCategory.Small, ItemQuality.Good, 850.0, 36.0m, "bronze", MaterialBehaviourType.Metal,
				[ReligiousTag, MarketTag], ["Holdable", "IncenseBurner_Antiquity_BronzeCenser", "Destroyable_HeavyMetal"],
				"Burns items tagged as incense fuel, producing room-scale scent text and smell-trackable ambience."),
			new("antiquity_resin_incense_pellets", "incense", "a packet of resin incense pellets",
				"This small packet holds hard amber and dark resin pellets, mixed for a sweet aromatic smoke when burned in a censer.",
				SizeCategory.Tiny, ItemQuality.Standard, 120.0, 12.0m, "resin", MaterialBehaviourType.Plant,
				[ReligiousTag, IncenseFuelTag], ["Holdable", "Destroyable_Misc"],
				"Tagged as incense fuel for the bronze incense censer component."),
			new("antiquity_household_altar", "altar", "a low household altar",
				"This low stone altar has a smoothed top, shallow soot marks, and enough working surface for small votive gifts, food, tablets, or tokens.",
				SizeCategory.Large, ItemQuality.Standard, 40000.0, 90.0m, "stone", MaterialBehaviourType.Stone,
				[ReligiousTag, FurnitureTag], ["OfferingReceiver_Antiquity_HouseholdAltar", "Destroyable_Furniture"],
				"Receives broad item offerings and supports explicit offering burns; direct liquid libations remain future work."),
			new("antiquity_votive_offering_basin", "basin", "a bronze votive offering basin",
				"This bronze basin is smoke-darkened inside, with a low rim and tripod feet for receiving small offerings that are meant to be burned at once.",
				SizeCategory.Normal, ItemQuality.Standard, 3800.0, 48.0m, "bronze", MaterialBehaviourType.Metal,
				[ReligiousTag, MarketTag], ["Holdable", "OfferingReceiver_Antiquity_VotiveBasin", "Destroyable_HeavyMetal"],
				"Burns accepted item offerings immediately on offer; not a poured-liquid libation vessel."),
			new("antiquity_funeral_offering_tray", "tray", "a wooden funeral offering tray",
				"This wooden tray has a raised lip, linen cord handles, and faint soot stains from offerings prepared beside a bier or grave.",
				SizeCategory.Normal, ItemQuality.Standard, 1800.0, 24.0m, "cedar", MaterialBehaviourType.Wood,
				[ReligiousTag, MedicalTag], ["Holdable", "OfferingReceiver_Antiquity_FuneralTray", "Destroyable_WoodenHeavy"],
				"Receives funeral offerings for later manual burning; direct liquid libations remain future work."),
			new("antiquity_irrigation_channel_outlet", "outlet", "an irrigation channel outlet",
				"This shaped stone outlet guides water from a channel into fields or garden beds, with silt marks and tool-cut edges around its mouth.",
				SizeCategory.VeryLarge, ItemQuality.Standard, 160000.0, 85.0m, "stone", MaterialBehaviourType.Stone,
				[FurnitureTag, CivicTag], ["WaterSource_Antiquity_IrrigationOutlet", "Destroyable_Furniture"]),

			new("antiquity_canvas_field_stretcher", "stretcher", "a canvas field stretcher",
				"This field stretcher is a pair of wooden poles laced through stout canvas, built for carrying wounded people across streets, camps, or battlefields.",
				SizeCategory.Large, ItemQuality.Standard, 5200.0, 28.0m, "linen", MaterialBehaviourType.Fabric,
				[ToolTag, MedicalTag], ["Holdable", "DragAid_Antiquity_FieldStretcher", "Destroyable_Misc"]),
			new("antiquity_wooden_corpse_bier", "bier", "a wooden corpse bier",
				"This wooden bier has carrying rails and a flat slatted bed, suitable for funerary procession, temple preparation, or moving the dead with dignity.",
				SizeCategory.VeryLarge, ItemQuality.Standard, 18000.0, 42.0m, "oak", MaterialBehaviourType.Wood,
				[ReligiousTag, MedicalTag], ["DragAid_Antiquity_CorpseBier", "Destroyable_WoodenHeavy"]),
			new("antiquity_rope_carrying_sling", "sling", "a rope carrying sling",
				"This rope sling knots into a broad carrying cradle for awkward bundles, injured limbs, jars, or field loads that need an extra hand.",
				SizeCategory.Small, ItemQuality.Standard, 680.0, 8.0m, "hemp", MaterialBehaviourType.Plant,
				[ToolTag], ["Holdable", "DragAid_Antiquity_CarryingSling", "Destroyable_Misc"]),
			new("antiquity_pack_travois", "travois", "a rawhide pack travois",
				"This rawhide travois binds two long poles with cross-bracing and hide webbing, ready to drag baggage behind a person or pack animal.",
				SizeCategory.Large, ItemQuality.Standard, 9800.0, 30.0m, "leather", MaterialBehaviourType.Leather,
				[ToolTag], ["DragAid_Antiquity_PackTravois", "Destroyable_WoodenHeavy"]),
			new("antiquity_cargo_sled", "sled", "a low wooden cargo sled",
				"This low cargo sled has scuffed runners and rope lashing points, built for warehouses, quarries, docks, and military stores.",
				SizeCategory.Large, ItemQuality.Standard, 15000.0, 36.0m, "oak", MaterialBehaviourType.Wood,
				[ToolTag], ["DragAid_Antiquity_CargoSled", "Destroyable_WoodenHeavy"]),

			new("antiquity_bone_knucklebones", "knucklebones", "a set of bone knucklebones",
				"This small set of polished knucklebones has four uneven faces on each piece, good for gambling, idle play, or informal divination.",
				SizeCategory.Tiny, ItemQuality.Standard, 90.0, 5.0m, "bone", MaterialBehaviourType.Bone,
				[MarketTag, LeisureTag], ["Holdable", "Dice_Antiquity_Knucklebones", "Destroyable_Misc"]),
			new("antiquity_ivory_six_sided_die", "die", "an ivory six-sided die",
				"This tiny ivory die is drilled with dark pips and worn smooth on its corners from cups, tavern tables, and travel pouches.",
				SizeCategory.Tiny, ItemQuality.Good, 12.0, 16.0m, "ivory", MaterialBehaviourType.Bone,
				[MarketTag, LeisureTag], ["Holdable", "Dice_d6", "Destroyable_Misc"]),
			new("antiquity_casting_sticks", "sticks", "a bundle of marked casting sticks",
				"This bundle of slender casting sticks is marked on one side and plain on the other, tied together with a narrow cord.",
				SizeCategory.Tiny, ItemQuality.Standard, 80.0, 6.0m, "reed", MaterialBehaviourType.Plant,
				[MarketTag, LeisureTag], ["Holdable", "Dice_Antiquity_CastingSticks", "Destroyable_Misc"]),
			new("antiquity_senet_game_board", "board", "a painted senet game board",
				"This painted wooden senet board is divided into neat squares and has a shallow lip for keeping counters and casting sticks together.",
				SizeCategory.Normal, ItemQuality.Good, 1600.0, 34.0m, "oak", MaterialBehaviourType.Wood,
				[MarketTag, LeisureTag], ["Holdable", "Container_Tray", "Destroyable_WoodenHeavy"]),
			new("antiquity_latrunculi_board", "board", "a gridded latrunculi board",
				"This gridded game board is scratched and inked into dark wood, with enough shallow grooves to keep counters aligned during play.",
				SizeCategory.Normal, ItemQuality.Standard, 1400.0, 24.0m, "oak", MaterialBehaviourType.Wood,
				[MarketTag, LeisureTag], ["Holdable", "Container_Tray", "Destroyable_WoodenHeavy"]),
			new("antiquity_tavern_gambling_cup", "cup", "a leather dice cup",
				"This stiff leather cup has a darkened rim and weighted base, made for rattling dice or knucklebones before a throw.",
				SizeCategory.Small, ItemQuality.Standard, 160.0, 8.0m, "leather", MaterialBehaviourType.Leather,
				[MarketTag, LeisureTag], ["Holdable", "Container_Pouch", "Destroyable_Misc"]),
			new("antiquity_ivory_loaded_die", "die", "an ivory loaded die",
				"This ivory die looks well made at a glance, but its polish, edge wear, and balance are just suspicious enough to reward a careful eye.",
				SizeCategory.Tiny, ItemQuality.Good, 14.0, 24.0m, "ivory", MaterialBehaviourType.Bone,
				[MarketTag, LeisureTag], ["Holdable", "Dice_Antiquity_LoadedD6", "Destroyable_Misc"]),
			new("antiquity_divination_lots", "lots", "a set of marked divination lots",
				"This set of marked lots carries tiny symbols for favour, warning, delay, journey, conflict, and sacrifice, kept together in a simple cord tie.",
				SizeCategory.Tiny, ItemQuality.Standard, 65.0, 9.0m, "bone", MaterialBehaviourType.Bone,
				[ReligiousTag, LeisureTag], ["Holdable", "Dice_Antiquity_DivinationLots", "Destroyable_Misc"]),
			new("antiquity_tavern_gambling_set", "set", "a tavern gambling set",
				"This compact gambling set bundles a leather cup, a cloth wrap, and a few spare counters for a quick game at camp or tavern.",
				SizeCategory.Small, ItemQuality.Standard, 420.0, 16.0m, "leather", MaterialBehaviourType.Leather,
				[MarketTag, LeisureTag], ["Holdable", "Container_Pouch", "Destroyable_Misc"]),

			new("antiquity_market_canvas_stall", "stall", "a canvas-covered market stall",
				"This portable market stall combines a light counter frame with a weathered canvas awning and tie-down cords for a temporary pitch.",
				SizeCategory.VeryLarge, ItemQuality.Standard, 36000.0, 85.0m, "oak", MaterialBehaviourType.Wood,
				[MarketTag, CivicTag], ["ShopStall_Antiquity_PortableBooth", "Destroyable_Furniture"]),
			new("antiquity_market_counter_chest", "chest", "a lockable merchant counter chest",
				"This merchant counter chest has a broad trading top and a secured compartment beneath, built for unattended market goods and coin-ready trade.",
				SizeCategory.Large, ItemQuality.Standard, 28000.0, 70.0m, "oak", MaterialBehaviourType.Wood,
				[MarketTag, CivicTag], ["ShopStall_Antiquity_LockableCounter", "Destroyable_WoodenHeavy"]),
			new("antiquity_bronze_standard_weight_set", "weights", "a bronze standard weight set",
				"This bronze weight set nests several marked weights in a small case, each piece worn shiny where merchants have handled it.",
				SizeCategory.Small, ItemQuality.Good, 2600.0, 55.0m, "bronze", MaterialBehaviourType.Metal,
				[ToolTag, MeasureTag], ["Holdable", "Container_Pouch", "MarketGoodWeight_Antiquity_StapleFood", "MeasuringInstrument_Antiquity_StandardWeights", "Destroyable_HeavyMetal"]),
			new("antiquity_stone_grain_measure", "measure", "a stone grain measure",
				"This squat stone measure has a level rim and worn interior, sized for fair grain portions in storehouses, mills, and tax courts.",
				SizeCategory.Normal, ItemQuality.Standard, 4200.0, 22.0m, "stone", MaterialBehaviourType.Stone,
				[ToolTag, MeasureTag], ["Holdable", "MarketGoodWeight_Antiquity_StapleFood", "MeasuringInstrument_Antiquity_GrainMeasure", "Destroyable_Misc"]),
			new("antiquity_wooden_measuring_rod", "rod", "a marked wooden measuring rod",
				"This wooden measuring rod is marked in repeated hand and cubit divisions. It remains a reference prop until physical length measurement is supported.",
				SizeCategory.Normal, ItemQuality.Standard, 520.0, 9.0m, "oak", MaterialBehaviourType.Wood,
				[ToolTag, MeasureTag], ["Holdable", "Destroyable_Misc"],
				"Length measurement is intentionally deferred until item dimensions exist."),
			new("antiquity_balance_scale", "scale", "a portable balance scale",
				"This compact balance scale has a bronze beam, cord hangers, and shallow pans for comparing small trade goods against known weights.",
				SizeCategory.Small, ItemQuality.Standard, 1800.0, 38.0m, "bronze", MaterialBehaviourType.Metal,
				[ToolTag, MeasureTag], ["Holdable", "MeasuringInstrument_Antiquity_BalanceScale", "Destroyable_HeavyMetal"]),
			new("antiquity_merchant_tax_tablet", "tablet", "a merchant tax tablet",
				"This clay tablet is ruled for taxable goods and standard portions, with room for fresh stylus marks beside common market categories.",
				SizeCategory.Small, ItemQuality.Standard, 650.0, 10.0m, "clay", MaterialBehaviourType.Ceramic,
				[WritingTag, ToolTag, MeasureTag], ["Holdable", "Antiquity_Clay_Tablet_Surface", "MarketGoodWeight_Antiquity_StapleFood", "Destroyable_Misc"]),
			new("antiquity_armoury_supply_counter", "counter", "an armoury supply counter",
				"This rugged armoury counter has a secured trade surface for issuing weapons, shields, arrows, and repair gear under official oversight.",
				SizeCategory.VeryLarge, ItemQuality.Standard, 42000.0, 95.0m, "oak", MaterialBehaviourType.Wood,
				[MarketTag, "Market / Military Goods", CivicTag], ["ShopStall_Antiquity_LockableCounter", "MarketGoodWeight_Antiquity_MilitarySupply", "Destroyable_WoodenHeavy"]),

			new("antiquity_bronze_lockpick_set", "lockpicks", "a bronze lockpick set",
				"This small bronze lockpick set holds flat picks, hooked probes, and a simple tension tool in a battered wrap.",
				SizeCategory.Tiny, ItemQuality.Standard, 120.0, 22.0m, "bronze", MaterialBehaviourType.Metal,
				[ToolTag, SecurityTag], ["Holdable", "Locksmithing_Antiquity_BronzeStandard", "Destroyable_Misc"]),
			new("antiquity_locksmith_probe_roll", "roll", "a roll of lock probes",
				"This leather roll carries bronze probes, shims, and narrow feelers for inspecting low-tech locks without immediately forcing them.",
				SizeCategory.Small, ItemQuality.Standard, 260.0, 26.0m, "leather", MaterialBehaviourType.Leather,
				[ToolTag, SecurityTag], ["Holdable", "Locksmithing_Antiquity_BronzeStandard", "Destroyable_Misc"]),
			new("antiquity_lock_installation_kit", "kit", "a locksmith's installation kit",
				"This installation kit gathers punches, pins, wedges, and small files for seating locks, latches, hasps, and fittings into chests or doors.",
				SizeCategory.Small, ItemQuality.Standard, 1600.0, 42.0m, "bronze", MaterialBehaviourType.Metal,
				[ToolTag, SecurityTag], ["Holdable", "Container_Pouch", "Locksmithing_Antiquity_Installation", "Destroyable_Misc"]),
			new("antiquity_key_filing_kit", "kit", "a key filing kit",
				"This key filing kit keeps narrow files, blanks, and rubbing wax ready for shaping or adjusting keys at a locksmith's bench.",
				SizeCategory.Small, ItemQuality.Standard, 900.0, 32.0m, "bronze", MaterialBehaviourType.Metal,
				[ToolTag, SecurityTag], ["Holdable", "Locksmithing_Antiquity_Fabrication", "Destroyable_Misc"]),
			new("antiquity_guard_key_cord", "cord", "a cord of guard keys",
				"This heavy cord strings together several official-looking keys and tags for a watchman, jailer, warehouse guard, or gate officer.",
				SizeCategory.Small, ItemQuality.Standard, 540.0, 18.0m, "bronze", MaterialBehaviourType.Metal,
				[ToolTag, SecurityTag], ["Holdable", "Keyring_Large", "Wear_Waist", "Destroyable_HeavyMetal"]),
			new("antiquity_fine_steel_lockpick_case", "case", "a fine steel lockpick case",
				"This slim case holds unusually fine steel picks and probes, carefully oiled and wrapped for delicate locks or expert dishonest work.",
				SizeCategory.Tiny, ItemQuality.Great, 180.0, 70.0m, "steel", MaterialBehaviourType.Metal,
				[ToolTag, SecurityTag], ["Holdable", "Container_Pouch", "Locksmithing_Antiquity_FineSteel", "Destroyable_Misc"]),

			new("antiquity_forum_notice_board", "board", "a whitewashed forum notice board",
				"This whitewashed public board is broad enough for notices, decrees, sales, and accusations, its surface scraped and repainted many times.",
				SizeCategory.Large, ItemQuality.Standard, 11000.0, 30.0m, "oak", MaterialBehaviourType.Wood,
				[FurnitureTag, CivicTag], ["Antiquity_Wooden_Block_Surface", "Destroyable_WoodenHeavy"]),
			new("antiquity_temple_decree_board", "board", "a temple decree board",
				"This temple board is painted in sober colours and framed for formal notices, rosters, vows, and religious decrees.",
				SizeCategory.Large, ItemQuality.Good, 9500.0, 38.0m, "cedar", MaterialBehaviourType.Wood,
				[ReligiousTag, CivicTag], ["Antiquity_Wooden_Block_Surface", "Destroyable_WoodenHeavy"]),
			new("antiquity_barracks_roster_board", "board", "a barracks roster board",
				"This barracks roster board is cut with neat rows for names, watches, duties, penalties, and temporary chalk or charcoal notes.",
				SizeCategory.Large, ItemQuality.Standard, 8200.0, 24.0m, "oak", MaterialBehaviourType.Wood,
				["Market / Military Goods", CivicTag], ["Antiquity_Wooden_Block_Surface", "Destroyable_WoodenHeavy"]),

			new("antiquity_bronze_signet_ring", "ring", "a bronze signet ring",
				"This bronze signet ring has a raised face cut with an official seal design, suitable for wax, clay, and careful comparison.",
				SizeCategory.Tiny, ItemQuality.Good, 28.0, 45.0m, "bronze", MaterialBehaviourType.Metal,
				[ToolTag, SecurityTag], ["Holdable", "Wear_Ring", "SealStamp_Antiquity_BronzeSignet", "Destroyable_HeavyMetal"]),
			new("antiquity_cylinder_seal", "seal", "a carved cylinder seal",
				"This small cylinder seal is pierced for a cord and carved around its body so it can roll a continuous authority mark through clay.",
				SizeCategory.Tiny, ItemQuality.Good, 95.0, 55.0m, "stone", MaterialBehaviourType.Stone,
				[ToolTag, SecurityTag], ["Holdable", "SealStamp_Antiquity_CylinderSeal", "Destroyable_Misc"]),
			new("antiquity_clay_bulla", "bulla", "a clay document bulla",
				"This small clay bulla is shaped for cord impressions and a sealing mark, ready to authenticate a tied document or packet.",
				SizeCategory.Tiny, ItemQuality.Standard, 85.0, 3.0m, "clay", MaterialBehaviourType.Ceramic,
				[WritingTag, SecurityTag], ["Holdable", "Antiquity_Clay_Tablet_Surface", "Sealable_Document_Clay", "Destroyable_Misc"]),
			new("antiquity_wax_seal_cake", "cake", "a wax seal cake",
				"This cake of dark wax is wrapped in a scrap of cloth and kept ready for melting into a document, scroll, or container seal.",
				SizeCategory.Tiny, ItemQuality.Standard, 55.0, 4.0m, "wax", MaterialBehaviourType.Wax,
				[WritingTag, SecurityTag], ["Holdable", "Destroyable_Misc"]),
			new("antiquity_sealed_papyrus_scroll", "scroll", "a sealable papyrus scroll",
				"This papyrus scroll is rolled with enough overlap for wax or clay to secure the tie and leave tamper evidence when broken.",
				SizeCategory.Small, ItemQuality.Standard, 95.0, 12.0m, "papyrus", MaterialBehaviourType.Plant,
				[WritingTag, SecurityTag], ["Holdable", "Antiquity_Papyrus_Scroll_Surface", "Sealable_Scroll", "Destroyable_Paper"]),
			new("antiquity_sealed_clay_tablet", "tablet", "a sealable clay tablet",
				"This clay tablet has a smoothed writing face and an edge left ready to take a bulla, clay seal, or official impression.",
				SizeCategory.Small, ItemQuality.Standard, 750.0, 8.0m, "clay", MaterialBehaviourType.Ceramic,
				[WritingTag, SecurityTag], ["Holdable", "Antiquity_Clay_Tablet_Surface", "Sealable_Document_Clay", "Destroyable_Misc"]),
			new("antiquity_tax_office_seal_box", "box", "a tax office seal box",
				"This compact tax office box has a built-in lock and sealing points for wax or clay, meant to carry tokens, receipts, and official tablets.",
				SizeCategory.Small, ItemQuality.Standard, 2200.0, 42.0m, "cedar", MaterialBehaviourType.Wood,
				[ToolTag, SecurityTag, CivicTag], ["Holdable", "LockingContainer_Lockbox", "Sealable_Container_Wax", "Destroyable_WoodenHeavy"]),
			new("antiquity_merchant_contract_bundle", "bundle", "a merchant contract bundle",
				"This bundled contract is made from tied papyrus sheets and a scroll wrapper, ready for a wax seal once the bargain is witnessed.",
				SizeCategory.Small, ItemQuality.Standard, 160.0, 16.0m, "papyrus", MaterialBehaviourType.Plant,
				[WritingTag, SecurityTag, MarketTag], ["Holdable", "Antiquity_Papyrus_Scroll_Surface", "Sealable_Document_Wax", "Destroyable_Paper"]),
			new("antiquity_grain_amphora_seal", "amphora", "a sealable grain amphora",
				"This earthenware grain amphora has a tied cover and prepared sealing points so stores can show whether the vessel has been opened.",
				SizeCategory.Large, ItemQuality.Standard, 12000.0, 20.0m, "earthenware", MaterialBehaviourType.Ceramic,
				[MarketTag, SecurityTag], ["Holdable", "LContainer_Amphora_Urna", "Sealable_Container_Wax", "Destroyable_Misc"]),
			new("antiquity_sealable_envelope", "envelope", "a papyrus sealable envelope",
				"This folded papyrus envelope has a small writable face, a closable flap, and a spot prepared for wax, clay, or paste.",
				SizeCategory.Tiny, ItemQuality.Standard, 18.0, 5.0m, "papyrus", MaterialBehaviourType.Plant,
				[WritingTag, SecurityTag], ["Holdable", "Container_Envelope", "PaperSheet_Envelope", "Sealable_Envelope", "Destroyable_Paper"]),
			new("antiquity_sealable_scroll", "scroll", "a blank sealable papyrus scroll",
				"This blank papyrus scroll is long enough for a formal letter or contract and can be sealed after writing.",
				SizeCategory.Small, ItemQuality.Standard, 85.0, 10.0m, "papyrus", MaterialBehaviourType.Plant,
				[WritingTag, SecurityTag], ["Holdable", "Antiquity_Papyrus_Scroll_Surface", "Sealable_Scroll", "Destroyable_Paper"]),

			new("antiquity_bronze_balance_scale", "scale", "a bronze balance scale",
				"This bronze balance scale has a central beam, matched pans, and a small suspension loop for market or tax-table use.",
				SizeCategory.Small, ItemQuality.Good, 2100.0, 48.0m, "bronze", MaterialBehaviourType.Metal,
				[ToolTag, MeasureTag], ["Holdable", "MeasuringInstrument_Antiquity_BalanceScale", "Destroyable_HeavyMetal"]),
			new("antiquity_standard_weight_set", "weights", "an official standard weight set",
				"This official standard weight set is stamped and nested in a small pouch for checking measures at market, warehouse, or tax gate.",
				SizeCategory.Small, ItemQuality.Good, 3000.0, 65.0m, "bronze", MaterialBehaviourType.Metal,
				[ToolTag, MeasureTag, CivicTag], ["Holdable", "Container_Pouch", "MarketGoodWeight_Antiquity_StapleFood", "MeasuringInstrument_Antiquity_StandardWeights", "Destroyable_HeavyMetal"]),
			new("antiquity_false_weight_set", "weights", "a false weight set",
				"This false weight set looks respectable, but the marked pieces are subtly biased for dishonest market measures.",
				SizeCategory.Small, ItemQuality.Standard, 2900.0, 45.0m, "bronze", MaterialBehaviourType.Metal,
				[ToolTag, MeasureTag], ["Holdable", "Container_Pouch", "MarketGoodWeight_Antiquity_StapleFood", "MeasuringInstrument_Antiquity_FalseWeights", "Destroyable_HeavyMetal"]),
			new("antiquity_oil_measure_cup", "cup", "an oil measure cup",
				"This small measure cup has a clean pouring lip and a marked inside line for oil, perfume, and other valuable fluids.",
				SizeCategory.Small, ItemQuality.Standard, 260.0, 14.0m, "bronze", MaterialBehaviourType.Metal,
				[ToolTag, MeasureTag], ["Holdable", "LContainer_DrinkingGlass", "MeasuringInstrument_Antiquity_OilCup", "Destroyable_HeavyMetal"]),
			new("antiquity_wine_measure_cup", "cup", "a wine measure cup",
				"This wine measure cup is larger than an oil cup, with dark lines around the interior and a steady base for public pours.",
				SizeCategory.Small, ItemQuality.Standard, 420.0, 16.0m, "bronze", MaterialBehaviourType.Metal,
				[ToolTag, MeasureTag], ["Holdable", "LContainer_WineGlass", "MeasuringInstrument_Antiquity_WineCup", "Destroyable_HeavyMetal"]),
			new("antiquity_tax_assessor_measure_kit", "kit", "a tax assessor's measure kit",
				"This official measure kit combines a folding scale beam, reference weights, tally tablets, cord, and a small seal-ready pouch.",
				SizeCategory.Normal, ItemQuality.Good, 5200.0, 95.0m, "bronze", MaterialBehaviourType.Metal,
				[ToolTag, MeasureTag, CivicTag], ["Holdable", "Container_Pouch", "MeasuringInstrument_Antiquity_TaxAssessorKit", "Destroyable_HeavyMetal"]),

			new("antiquity_leather_bridle", "bridle", "a plain leather bridle",
				"This plain leather bridle has a browband, cheek straps, and reins arranged for steady everyday control of a riding animal.",
				SizeCategory.Normal, ItemQuality.Standard, 1200.0, 24.0m, "leather", MaterialBehaviourType.Leather,
				[RidingTackTag, "Market / Transportation / Horse Tack"], ["Holdable", "Wear_Bridle", "RidingGear_Bridle", "Destroyable_Clothing"]),
			new("antiquity_pack_saddle", "saddle", "a leather pack saddle",
				"This broad leather pack saddle is padded beneath a rigid frame and fitted with many lash points for balancing cargo over an animal's back.",
				SizeCategory.Large, ItemQuality.Standard, 8500.0, 65.0m, "leather", MaterialBehaviourType.Leather,
				[PackGearTag, "Market / Transportation / Horse Tack"], ["Holdable", "Wear_Saddle", "RidingGear_PackSaddle", "Destroyable_Clothing"]),
			new("antiquity_mule_pannier_set", "panniers", "a pair of mule panniers",
				"This matched pair of deep woven panniers hangs from a padded leather frame, keeping a mule's cargo divided and balanced.",
				SizeCategory.Large, ItemQuality.Standard, 7200.0, 54.0m, "wicker", MaterialBehaviourType.Wood,
				[PackGearTag, "Market / Transportation / Horse Tack"], ["Holdable", "Wear_Saddle", "RidingGear_PackSaddle", "Container_PreIndustrial_LiddedHamper", "Destroyable_Misc"]),
			new("antiquity_ox_yoke", "yoke", "a heavy ox yoke",
				"This heavy shaped oak yoke has smoothed neck bows and iron fastening points for joining a pair of oxen to a plough, cart, or wagon.",
				SizeCategory.VeryLarge, ItemQuality.Standard, 26000.0, 58.0m, "oak", MaterialBehaviourType.Wood,
				[DraftGearTag, "Market / Transportation / Horse Tack"], ["Holdable", "Wear_Saddle", "HitchGear_Yoke", "Destroyable_WoodenHeavy"]),
			new("antiquity_chariot_harness", "harness", "a leather chariot harness",
				"This layered leather chariot harness carries reinforced breast straps, traces, and bronze rings for transferring a team's effort into the pole.",
				SizeCategory.Large, ItemQuality.Good, 6800.0, 82.0m, "leather", MaterialBehaviourType.Leather,
				[DraftGearTag, "Market / Military Goods"], ["Holdable", "Wear_Saddle", "HitchGear_Harness", "Destroyable_Clothing"]),
			new("antiquity_camel_cargo_saddle", "saddle", "a padded camel cargo saddle",
				"This high padded cargo saddle is shaped around a camel's back and hung with broad straps and lash rings for bulky desert loads.",
				SizeCategory.Large, ItemQuality.Standard, 9800.0, 76.0m, "leather", MaterialBehaviourType.Leather,
				[PackGearTag, "Market / Transportation / Horse Tack"], ["Holdable", "Wear_Saddle", "RidingGear_PackSaddle", "Destroyable_Clothing"]),
			new("antiquity_warhorse_barding_harness", "barding", "a reinforced warhorse barding harness",
				"This reinforced leather-scale barding harness protects a warhorse's body while retaining the straps and control fittings needed under arms.",
				SizeCategory.VeryLarge, ItemQuality.Good, 18000.0, 145.0m, "leather", MaterialBehaviourType.Leather,
				[AnimalArmourTag, RidingTackTag, "Market / Military Goods"], ["Holdable", "Wear_Saddle", "RidingGear_RidingHarness", "Armour_LeatherScale", "Destroyable_Armour"]),
			new("antiquity_rope_lead_halter", "halter", "a rope lead halter",
				"This soft hemp halter loops around an animal's muzzle and poll, ending in a stout lead rope for bitless control or light hitching.",
				SizeCategory.Normal, ItemQuality.Standard, 900.0, 12.0m, "hemp", MaterialBehaviourType.Plant,
				[RidingTackTag, DraftGearTag, "Market / Transportation / Horse Tack"], ["Holdable", "Wear_Bridle", "RidingGear_BitlessBridle", "HitchGear_LeadRope", "Destroyable_Misc"])
		];
	}
}
