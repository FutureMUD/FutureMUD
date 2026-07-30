#nullable enable

using MudSharp.GameItems;
using MudSharp.Vehicles;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static VehicleSeedSpec Across(VehicleSeedSpec spec, params string[] additionalEraKeys)
	{
		return spec with { AdditionalEraKeys = additionalEraKeys };
	}

	private static IReadOnlyList<VehicleSeedSpec> ExpandedPreIndustrialVehicleExamples()
	{
		return
		[
			CreateDraftCargoVehicle(
				"vehicle_antiquity_light_war_chariot", "antiquity", "Light War Chariot",
				"A fast two-horse chariot with a waist-high fighting rail and compact equipment tray.",
				"chariot", "a light bronze-fitted war chariot",
				"Two spoked wheels flank a light bentwood body whose rawhide-lashed frame is faced with painted leather. A waist-high rail protects the standing driver and warrior without enclosing them, while a bronze-bound pole and paired yoke fittings project forward. A narrow tray at the rear holds spare reins, javelins and a shield without burdening the sprung platform.",
				SizeCategory.Enormous, ItemQuality.Good, 360000.0, 4200.0m, "ash", "Destroyable_WoodenHeavy",
				VehicleScale.ItemScale, 1, false, false, 1400000.0, 1.25,
				"equipment tray", "A shallow rear tray secures javelins, reins and other fighting equipment.",
				"WarChariot"),
			CreateDraftCargoVehicle(
				"vehicle_antiquity_racing_chariot", "antiquity", "Racing Chariot",
				"An exceptionally light two-wheeled chariot built for a driver and a fast team.",
				"chariot", "a narrow racing chariot",
				"A narrow standing platform hangs between two tall, finely spoked wheels with only a low curved guard at the front. The ash frame is pared down wherever strength permits, and rawhide bindings reinforce the axle and pole without the weight of heavy metalwork. Polished handrails and tightly wrapped reins give the vehicle the spare, purposeful finish of competition equipment.",
				SizeCategory.Huge, ItemQuality.Good, 190000.0, 5200.0m, "ash", "Destroyable_WoodenHeavy",
				VehicleScale.ItemScale, 0, false, false, 900000.0, 1.35,
				"small gear shelf", "A tiny shelf beneath the rail holds only reins, a whip and emergency tackle.",
				"RacingChariot"),
			Across(CreateDraftCargoVehicle(
				"vehicle_preindustrial_farm_wain", "antiquity", "Farm Wain",
				"A broad four-wheeled farm wagon with removable slatted sides.",
				"wain", "a broad slatted farm wain",
				"Four heavy wooden wheels support a long plank bed between tall removable slatted sides. A pivoting forecarriage, stout pole and simple brake equip the vehicle for a team of draft animals, while rope cleats along the rails secure hay, sacks or timber. Every surface is plain, thick and readily repairable by a village wheelwright.",
				SizeCategory.Enormous, ItemQuality.Standard, 820000.0, 3100.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 2, false, false, 6500000.0, 1.05,
				"slatted wagon bed", "A long open bed with removable slatted sides carries loose farm produce and bulky loads.",
				"FarmWain"),
				"medieval", "renaissance", "earlymodern"),
			Across(CreateDraftCargoVehicle(
				"vehicle_preindustrial_winter_sledge", "antiquity", "Freight Sledge",
				"A low freight platform carried on broad wooden runners instead of wheels.",
				"sledge", "a low timber freight sledge",
				"A stout plank platform rests across two long runners whose upturned noses are shod against ice and hard ground. Cross-braces, rope holes and removable stakes secure timber, barrels or bundled goods, while a central pole accepts a draft team. The low construction sacrifices speed for stability and easy hauling over snow, marsh matting or prepared slides.",
				SizeCategory.Enormous, ItemQuality.Standard, 540000.0, 1900.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 2, false, false, 4800000.0, 1.1,
				"freight platform", "The low open platform is fitted with rope holes and removable cargo stakes.",
				"FreightSledge"),
				"medieval", "renaissance", "earlymodern"),
			Across(CreateDraftCargoVehicle(
				"vehicle_medieval_timber_wagon", "medieval", "Timber Wagon",
				"A long open wagon fitted with bolsters and chains for logs and beams.",
				"wagon", "a long timber-hauling wagon",
				"Two widely separated wheel assemblies support a long open spine fitted with heavy transverse bolsters. Iron dogs, chains and upright stakes restrain logs and squared beams, while a jointed reach lets the rear wheels follow the team through bends. The driver's board and powerful drag brake are secondary to the vehicle's massive load-bearing frame.",
				SizeCategory.Gigantic, ItemQuality.Standard, 1450000.0, 6400.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 1, false, false, 12000000.0, 1.0,
				"timber bolsters", "Open bolsters, chains and upright stakes secure long logs and beams.",
				"TimberWagon"),
				"renaissance", "earlymodern"),
			Across(CreateDraftCargoVehicle(
				"vehicle_renaissance_artillery_limber", "renaissance", "Artillery Limber",
				"A compact two-wheeled military carriage for towing a gun and carrying ready ammunition.",
				"limber", "an iron-braced artillery limber",
				"A heavy axle and pair of broad wheels support a compact oak chest reinforced with iron bands. A forward pole accepts a gun team, while a stout rear pintle couples to a field piece or caisson. The chest lid doubles as a crew seat and closes over partitioned ammunition spaces kept clear of the towing gear.",
				SizeCategory.Enormous, ItemQuality.Good, 980000.0, 8800.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.ItemScale, 4, true, false, 9000000.0, 1.0,
				"ammunition chest", "A partitioned, iron-bound chest carries ready shot, tools and sealed charges.",
				"ArtilleryLimber"),
				"earlymodern"),
			CreateDraftCargoVehicle(
				"vehicle_earlymodern_hackney_coach", "earlymodern", "Hackney Coach",
				"An enclosed urban hire coach with facing passenger seats and an external driver's box.",
				"coach", "a black-painted hackney coach",
				"A square enclosed body hangs from leather thoroughbraces between four iron-rimmed wheels. Glazed side windows light two facing benches, and a folding step descends beneath the latched passenger door. The high driver's box, rear luggage rail and hard-wearing dark paint suit a vehicle built for repeated hired journeys through crowded streets.",
				SizeCategory.Gigantic, ItemQuality.Standard, 1320000.0, 11800.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 6, true, false, 7000000.0, 1.0,
				"rear luggage rail", "A railed platform behind the passenger body carries trunks and parcels.",
				"HackneyCoach"),
			CreateDraftCargoVehicle(
				"vehicle_earlymodern_post_chaise", "earlymodern", "Post Chaise",
				"A light enclosed travelling carriage intended for rapid changes of hired horses.",
				"chaise", "a light yellow post chaise",
				"A compact enclosed body sits low between four narrow wheels on flexible springs. A forward-facing upholstered bench fills the cabin, while travel cases strap to a small rear platform beneath a folding hood. Light shafts, prominent lamps and fittings for postilion-driven teams emphasise speed between posting houses rather than heavy carrying.",
				SizeCategory.Enormous, ItemQuality.Good, 760000.0, 15400.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 3, true, false, 4200000.0, 1.1,
				"travelling-case rack", "A compact rear rack carries fitted travelling cases and mail bags.",
				"PostChaise"),

			Across(CreatePaddleCraft(
				"vehicle_preindustrial_dugout_canoe", "antiquity", "Dugout Canoe",
				"A narrow log canoe paddled from an open hull.",
				"canoe", "a long dugout canoe",
				"A single massive tree trunk has been hollowed, spread and trimmed into a narrow open canoe. The rounded bottom bears adze marks beneath a dark coating of oil and pitch, while low thwarts brace the sides and provide simple seats. Pointed ends and little freeboard make the craft quick on rivers and sheltered coasts when lightly loaded.",
				SizeCategory.Enormous, ItemQuality.Standard, 180000.0, 520.0m, "cedar", "Destroyable_WoodenHeavy",
				VehicleScale.ItemScale, VehiclePropulsionType.SelfPowered, 1, 0, false, true, "DugoutCanoe"),
				"medieval", "renaissance", "earlymodern"),
			Across(CreatePaddleCraft(
				"vehicle_preindustrial_river_punt", "antiquity", "River Punt",
				"A shallow flat-bottomed punt for poles, paddles and quiet inland water.",
				"punt", "a broad flat-bottomed river punt",
				"A rectangular flat bottom rises into low, nearly vertical sides around a broad open working space. Short decks strengthen both ends, and a long setting pole lies beside plain bench seats and a single steering oar. The shallow draught and sacrificial rubbing strips suit reed beds, ferries, fish ponds and muddy river margins.",
				SizeCategory.Enormous, ItemQuality.Standard, 360000.0, 1100.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.ItemScale, VehiclePropulsionType.SelfPowered, 1, 0, false, true, "RiverPunt"),
				"medieval", "renaissance", "earlymodern"),
			Across(CreatePaddleCraft(
				"vehicle_preindustrial_ferry_barge", "antiquity", "Ferry Barge",
				"A broad open ferry propelled by sweeps across rivers and sheltered channels.",
				"barge", "a broad open ferry barge",
				"A wide flat-bottomed hull supports an unobstructed deck between low rails and reinforced landing ends. Heavy sweeps work from stout pivots along the sides, while bollards and coiled ropes stand ready to warp the vessel across a current. The capacious deck can accept passengers, animals, carts and loose freight in carefully balanced loads.",
				SizeCategory.Titanic, ItemQuality.Standard, 5200000.0, 14500.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, VehiclePropulsionType.Rowed, 1, 8, false, true, "FerryBarge"),
				"medieval", "renaissance", "earlymodern"),
			Across(CreatePaddleCraft(
				"vehicle_preindustrial_river_cargo_barge", "antiquity", "River Cargo Barge",
				"A long shallow-draught barge for bulk cargo on rivers and canals.",
				"barge", "a long high-sided cargo barge",
				"A long flat-bottomed hull encloses a deep open hold between narrow walking decks. Massive steering oars, towing posts and several working sweeps allow the barge to manoeuvre when it is not warped or hauled from the bank. Tarred planks, removable hatch boards and high coamings protect grain, stone and barrels from ordinary spray.",
				SizeCategory.Titanic, ItemQuality.Standard, 9800000.0, 22000.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, VehiclePropulsionType.Rowed, 1, 10, false, true, "RiverCargoBarge"),
				"medieval", "renaissance", "earlymodern"),
			Across(CreatePaddleCraft(
				"vehicle_preindustrial_fishing_skiff", "antiquity", "Fishing Skiff",
				"A small open fishing boat with room for nets, baskets and two pairs of oars.",
				"skiff", "a tarred open fishing skiff",
				"A short broad hull rises to a pointed bow around two rowing thwarts and an open working floor. Net floats, line cleats and replaceable rubbing strakes crowd the gunwales, while a small covered locker occupies the stern. Tar and salt have darkened the planking wherever baskets and wet gear repeatedly rub.",
				SizeCategory.Enormous, ItemQuality.Standard, 410000.0, 1350.0m, "pine", "Destroyable_WoodenHeavy",
				VehicleScale.ItemScale, VehiclePropulsionType.Rowed, 1, 3, false, true, "FishingSkiff"),
				"medieval", "renaissance", "earlymodern"),
			CreatePaddleCraft(
				"vehicle_antiquity_river_galley", "antiquity", "River Galley",
				"A shallow war and dispatch galley driven by a disciplined bank of oars.",
				"galley", "a low many-oared river galley",
				"A long shallow hull carries closely spaced rowing benches beneath low washboards. A raised steering platform overlooks the open waist, while a reinforced prow and small fighting deck provide room for archers or marines. The narrow beam and light construction favour speed, beaching and manoeuvre on rivers and enclosed waters.",
				SizeCategory.Titanic, ItemQuality.Good, 7800000.0, 28000.0m, "cedar", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, VehiclePropulsionType.Rowed, 1, 24, false, true, "RiverGalley"),
			CreatePaddleCraft(
				"vehicle_antiquity_trireme", "antiquity", "Trireme",
				"A large oared warship with three coordinated banks and a bronze-sheathed ram.",
				"trireme", "a long bronze-rammed trireme",
				"A very long, narrow hull carries outriggers and ordered ranks of oar ports above a low fighting deck. The sharp prow terminates in a heavy bronze-sheathed ram, while steering oars and a raised command platform dominate the stern. Painted eyes, furled auxiliary sailcloth and tightly stowed deck gear leave the vessel's speed and disciplined rowing crew as its defining features.",
				SizeCategory.Titanic, ItemQuality.Good, 42000000.0, 96000.0m, "cedar", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, VehiclePropulsionType.Rowed, 1, 60, false, true, "Trireme"),
			Across(CreateSailCraft(
				"vehicle_preindustrial_lateen_dhow", "antiquity", "Lateen Trading Dhow",
				"A sewn-plank lateen vessel for coastal trade and open-water passages.",
				"dhow", "a high-stemmed lateen trading dhow",
				"A high narrow bow and rising stern enclose a capacious hull built from close-fitted planks and heavy internal ribs. One raking mast carries a long lateen yard, with spare spars and coiled running rigging secured along the rails. A deep cargo hold, steering gear and weathered awning mark a vessel intended for seasonal trade rather than short harbour work.",
				SizeCategory.Titanic, ItemQuality.Good, 12500000.0, 38000.0m, "cedar", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 12, 8, true, "TradingDhow", false),
				"medieval", "renaissance", "earlymodern"),
			CreateSailCraft(
				"vehicle_medieval_longship", "medieval", "Longship",
				"A long shallow-draught vessel combining square sail, oars and beaching capability.",
				"longship", "a clinker-built square-sailed longship",
				"A long clinker-built hull rises to carved posts at bow and stern around an open run of rowing benches. A single square sail hangs from a lowering mast, while shields can be secured along the rails above banks of oar ports. The shallow keel, steering oar and light deck gear allow the vessel to cross open water, enter rivers and beach directly.",
				SizeCategory.Titanic, ItemQuality.Good, 18000000.0, 46000.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 24, 30, true, "Longship"),
			Across(CreatePaddleCraft(
				"vehicle_medieval_river_wherry", "medieval", "River Wherry",
				"A long open passenger and light-cargo boat rowed from fixed thwarts.",
				"wherry", "a long clinker-built river wherry",
				"A fine clinker-built hull surrounds several passenger benches beneath a low removable awning. A pair of long oars works from reinforced rowlocks, and the raised stern gives the steersman a clear view along crowded waterways. Small lockers, rope fenders and a shallow draught suit ferries, hire work and rapid errands.",
				SizeCategory.Gigantic, ItemQuality.Good, 760000.0, 3400.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, VehiclePropulsionType.Rowed, 1, 4, false, true, "RiverWherry"),
				"renaissance", "earlymodern"),
			Across(CreateSailCraft(
				"vehicle_renaissance_caravel", "renaissance", "Ocean-Going Caravel",
				"A handy lateen-rigged exploration and trading vessel for long ocean passages.",
				"caravel", "a high-sided ocean-going caravel",
				"A deep but fine-lined hull carries two raking masts with lateen sails above a flush working deck. A raised stern shelters the helm and chart space, while hatch covers close a modest hold amidships. Heavy standing rigging, spare water casks and sea-worn rails distinguish the vessel from a coastal boat despite its manageable size.",
				SizeCategory.Titanic, ItemQuality.Good, 52000000.0, 125000.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 18, 16, true, "OceanCaravel", false),
				"earlymodern"),
			Across(CreateSailCraft(
				"vehicle_renaissance_carrack", "renaissance", "Great Carrack",
				"A very large ocean trader with towering castles and several square-rigged masts.",
				"carrack", "a towering three-masted carrack",
				"A broad deep-bellied hull rises into massive castles at bow and stern around a crowded waist. Three masts carry a mixture of square and lateen canvas above a capacious, hatch-covered hold. Heavy rails, ship's boats, windlass gear and tiers of enclosed working spaces make the vessel a floating community built for cargo and long voyages.",
				SizeCategory.Titanic, ItemQuality.Good, 260000000.0, 420000.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 60, 45, true, "GreatCarrack", false),
				"earlymodern"),
			Across(CreateSailCraft(
				"vehicle_renaissance_galleon", "renaissance", "Ocean Galleon",
				"A large multi-deck sailing ship suited to treasure carriage, escort and warfare.",
				"galleon", "a high-sterned ocean galleon",
				"A long ocean-going hull narrows beneath a high stepped stern and a lower beak-headed bow. Multiple square-rigged masts tower over enclosed decks, broad cargo hatches and rows of shuttered gun ports. Galleries, boats, capstans and dense standing rigging reveal a vessel intended to carry people, valuable cargo and heavy armament far from land.",
				SizeCategory.Titanic, ItemQuality.Good, 420000000.0, 680000.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 90, 70, true, "OceanGalleon", false),
				"earlymodern"),
			CreateSailCraft(
				"vehicle_earlymodern_coastal_schooner", "earlymodern", "Coastal Schooner",
				"A two-masted fore-and-aft trader that combines cargo capacity with a small crew.",
				"schooner", "a two-masted coastal schooner",
				"A clean-lined hull supports two raking masts carrying fore-and-aft sails above a broad working deck. Low hatch coamings open into a useful cargo hold, while a compact stern cabin shelters the helm and charts. The uncluttered rig, shallow draught and weathered loading gear suit frequent passages between small ports.",
				SizeCategory.Titanic, ItemQuality.Good, 34000000.0, 92000.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 14, 10, true, "CoastalSchooner", false),
			CreateSailCraft(
				"vehicle_earlymodern_packet_ship", "earlymodern", "Packet Ship",
				"A fast scheduled sailing vessel carrying mail, passengers and high-value cargo.",
				"packet", "a trim ocean packet ship",
				"A long full-rigged hull combines fine entry lines with a high weather deck and enclosed passenger spaces aft. Mail lockers and a secure cargo hold sit beneath broad hatch covers, while boats and storm canvas are kept ready for an exacting schedule. The crowded rig and carefully maintained helm fittings favour dependable passage speed over maximum freight capacity.",
				SizeCategory.Titanic, ItemQuality.Good, 180000000.0, 360000.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 48, 36, true, "PacketShip", false),
			CreateSailCraft(
				"vehicle_earlymodern_sailing_frigate", "earlymodern", "Sailing Frigate",
				"A fast ocean warship with a single covered gun deck and powerful square rig.",
				"frigate", "a long-gunned sailing frigate",
				"A long, relatively narrow hull carries a continuous enclosed gun deck beneath an open weather deck. Three tall masts spread a powerful square rig above boats, capstans and orderly ranks of cannon ports. A raised quarterdeck, ornate stern and fine bow combine command presence with the lines of a vessel built to scout, escort and raid.",
				SizeCategory.Titanic, ItemQuality.Good, 650000000.0, 980000.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 120, 160, true, "SailingFrigate", false),
			CreateSailCraft(
				"vehicle_earlymodern_ship_of_the_line", "earlymodern", "Ship of the Line",
				"A massive multi-deck battle fleet vessel carrying heavy batteries and a very large crew.",
				"warship", "a towering ship of the line",
				"A massive wall-sided hull rises through several enclosed gun decks to an open weather deck crowded with boats and heavy fittings. Three towering masts carry acres of square canvas above tier after tier of shuttered gun ports. Broad quarter galleries, reinforced magazines, capstans and deep stores reveal a warship built to remain at sea and fight in an organised battle line.",
				SizeCategory.Titanic, ItemQuality.Excellent, 2200000000.0, 2400000.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 240, 420, true, "ShipOfTheLine", false)
		];
	}
}
