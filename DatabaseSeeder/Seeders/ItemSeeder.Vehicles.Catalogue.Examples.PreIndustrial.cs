#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string VehicleLandEngineMountType = "land_engine";
	private const string VehicleElectricDriveMountType = "electric_drive";

	private static readonly IReadOnlyList<VehicleSeedSpec> VehicleExampleSpecs = BuildVehicleExampleSpecs();

	private static IReadOnlyList<VehicleSeedSpec> BuildVehicleExampleSpecs()
	{
		return
		[
			.. AntiquityVehicleExamples(),
			.. MedievalVehicleExamples(),
			.. RenaissanceVehicleExamples(),
			.. EarlyModernVehicleExamples(),
			.. RevolutionVehicleExamples(),
			.. ModernVehicleExamples(),
			.. AtomicVehicleExamples(),
			.. ComputerVehicleExamples()
		];
	}

	private static IReadOnlyList<VehicleSeedSpec> AntiquityVehicleExamples()
	{
		return
		[
			CreateDraftCargoVehicle(
				"vehicle_antiquity_two_wheeled_handcart", "antiquity", "Hand-Pulled Cart",
				"A narrow two-wheeled cart built for a single porter or a small draft animal.",
				"handcart", "a narrow two-wheeled handcart",
				"A pair of tall wooden wheels flanks a narrow plank bed, with long shafts projecting forward for a porter or small draft animal. Pegged joints, rawhide lashings and a low rail keep modest loads secure without adding much weight. The axle and wheel hubs are darkened by grease and road dust.",
				SizeCategory.Huge, ItemQuality.Standard, 95000.0, 420.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.ItemScale, 0, false, false, 750000.0, 1.35,
				"open load bed", "The cart's shallow plank bed is open above and bounded by low wooden rails.",
				"OpenCart"),
			CreateDraftCargoVehicle(
				"vehicle_antiquity_heavy_ox_wagon", "antiquity", "Heavy Ox Wagon",
				"A broad four-wheeled wagon intended for slow, heavy overland haulage.",
				"wagon", "a broad timber ox wagon",
				"Four thick wooden wheels support a broad, deeply railed wagon bed. A heavy pole and yoke fittings project from the front axle, while iron straps bind the most highly stressed joints. The body is deliberately massive and plain, suited to sacks, amphorae and timber rather than speed.",
				SizeCategory.Enormous, ItemQuality.Standard, 650000.0, 2400.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 2, false, false, 5000000.0, 1.1,
				"wagon bed", "The wagon bed forms a broad open platform enclosed by waist-high slatted sides.",
				"DraftWagon"),
			CreatePaddleCraft(
				"vehicle_antiquity_reed_coracle", "antiquity", "Reed Coracle",
				"A light basket-framed coracle for sheltered water and river crossings.",
				"coracle", "a round hide-covered reed coracle",
				"A shallow round basket of bundled reeds and flexible ribs has been drawn tight beneath a dark waterproof hide. A simple thwart crosses the centre, leaving just enough room for one paddler and a compact bundle. The little craft is light enough to beach or carry, but its broad flat bottom promises lively handling in rough water.",
				SizeCategory.VeryLarge, ItemQuality.Standard, 32000.0, 180.0m, "reed", "Destroyable_Misc",
				VehicleScale.ItemScale, VehiclePropulsionType.SelfPowered, 1, 0, true, false, "PaddleCraft"),
			CreateSailCraft(
				"vehicle_antiquity_coastal_sailing_boat", "antiquity", "Coastal Sailing Boat",
				"A broad-bellied coastal trader driven by a single square sail and steering oars.",
				"boat", "a broad cedar coastal sailing boat",
				"A high-sided cedar hull curves around a broad working deck, its seams packed and sealed beneath dark pitch. A single mast carries a square yard and a heavy woven sail, while paired steering oars project near the stern. The deep central hold and stout gunwales mark it as a practical coastal carrier rather than a racing craft.",
				SizeCategory.Gigantic, ItemQuality.Good, 1800000.0, 9200.0m, "cedar", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 4, 2, true, "SailingTrader"),
		];
	}

	private static IReadOnlyList<VehicleSeedSpec> MedievalVehicleExamples()
	{
		return
		[
			CreateDraftCargoVehicle(
				"vehicle_medieval_market_cart", "medieval", "Market Cart",
				"A compact two-wheeled market cart with removable sideboards.",
				"cart", "a compact oak market cart",
				"This compact cart rides on two iron-rimmed wheels beneath a shallow oak bed. Removable sideboards slot into upright stakes, allowing the same vehicle to carry baskets, firewood or loose produce. A pair of polished shafts and a simple hand brake show the care expected of a working town cart.",
				SizeCategory.Huge, ItemQuality.Good, 140000.0, 850.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.ItemScale, 1, false, false, 1200000.0, 1.25,
				"market bed", "The shallow cart bed is enclosed by removable slatted sideboards.",
				"MarketCart"),
			CreateDraftCargoVehicle(
				"vehicle_medieval_covered_wagon", "medieval", "Covered Road Wagon",
				"A high-sided road wagon protected by a bowed canvas cover.",
				"wagon", "a canvas-covered road wagon",
				"A sturdy four-wheeled chassis carries a deep plank body beneath a series of bent ash hoops. Waxed canvas stretches over the hoops to form a weatherproof cover, with a laced flap closing the rear. A raised driving bench, iron-shod wheels and a long draft pole equip the wagon for sustained road travel.",
				SizeCategory.Enormous, ItemQuality.Standard, 780000.0, 3900.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 4, true, false, 6000000.0, 1.05,
				"covered cargo bay", "The wagon's deep cargo bay lies beneath a bowed canvas cover and behind a laced rear flap.",
				"CoveredWagon"),
			CreatePaddleCraft(
				"vehicle_medieval_clinker_rowboat", "medieval", "Clinker-Built Rowboat",
				"A sturdy open rowboat assembled from overlapping oak strakes.",
				"rowboat", "a clinker-built oak rowboat",
				"Overlapping oak planks rise from a strong keel to form the flared sides of this open boat. Riveted seams, stout thwarts and worn wooden rowlocks give the hull a rugged, workmanlike character. A pointed bow and modest stern platform make it equally suited to fishing, ferrying and service beside a larger ship.",
				SizeCategory.Enormous, ItemQuality.Good, 420000.0, 1800.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.ItemScale, VehiclePropulsionType.Rowed, 1, 3, false, true, "Rowboat"),
			CreateSailCraft(
				"vehicle_medieval_trading_cog", "medieval", "Trading Cog",
				"A bluff-bowed merchant cog with a single square sail and enclosed hold.",
				"cog", "a bluff-bowed trading cog",
				"A broad clinker-built hull rises to high castles at bow and stern around a deep central waist. One heavy mast bears a square sail, while a sternpost rudder hangs from stout iron fittings. A hatch-covered hold occupies much of the vessel's interior, and the thick rails and capacious body favour cargo and endurance over nimble handling.",
				SizeCategory.Titanic, ItemQuality.Standard, 9500000.0, 46000.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 8, 6, true, "SailingCargoShip"),
		];
	}

	private static IReadOnlyList<VehicleSeedSpec> RenaissanceVehicleExamples()
	{
		return
		[
			CreateDraftCargoVehicle(
				"vehicle_renaissance_city_carriage", "renaissance", "City Carriage",
				"A compact enclosed carriage for prosperous urban passengers.",
				"carriage", "a painted enclosed city carriage",
				"A sprung four-wheeled chassis supports a compact enclosed body with glazed side openings and a padded bench within. Painted panels, turned corner posts and a small folding step lend the carriage a restrained elegance. The driver's raised seat and paired shafts are arranged for a light team on paved streets.",
				SizeCategory.Enormous, ItemQuality.Good, 720000.0, 7200.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 4, true, false, 4000000.0, 1.05,
				"rear luggage rack", "A railed luggage platform is fixed behind the enclosed passenger body.",
				"PassengerCarriage"),
			CreateDraftCargoVehicle(
				"vehicle_renaissance_artillery_wagon", "renaissance", "Artillery Wagon",
				"A reinforced four-wheeled wagon for moving powder, shot and siege equipment.",
				"wagon", "a reinforced artillery wagon",
				"Heavy oak beams form a low, wide wagon chassis braced by iron straps and through-bolts. Deep sideboards restrain powder barrels, shot baskets and tools, while broad wheels spread the load over broken ground. Multiple hitch points and a stout rear drag shoe reflect the punishing work expected of the vehicle.",
				SizeCategory.Enormous, ItemQuality.Standard, 1100000.0, 5600.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 2, false, false, 8500000.0, 1.0,
				"reinforced load bed", "The low wagon bed is strengthened with iron straps and deep removable sideboards.",
				"MilitaryWagon"),
			CreatePaddleCraft(
				"vehicle_renaissance_ship_launch", "renaissance", "Ship's Launch",
				"A broad pulling boat used to move people and stores between ship and shore.",
				"launch", "a broad many-oared ship's launch",
				"A long open hull encloses several rowing thwarts and a clear passage down the centre. Iron rowlocks line both gunwales, while lifting eyes and rubbing strakes show that the boat is meant to be hoisted aboard a larger vessel. The broad stern and capacious floor leave room for passengers, casks and bundled stores.",
				SizeCategory.Gigantic, ItemQuality.Good, 960000.0, 4300.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, VehiclePropulsionType.Rowed, 1, 7, false, true, "ShipsLaunch"),
			CreateSailCraft(
				"vehicle_renaissance_lateen_pinnace", "renaissance", "Lateen-Rigged Pinnace",
				"A light dispatch and trading vessel with a raking lateen sail.",
				"pinnace", "a light lateen-rigged pinnace",
				"A fine-lined wooden hull carries a low deck, a modest covered hold and a single mast raked to support a long lateen yard. The triangular sail and clean underwater shape promise better manoeuvrability than a broad cargo ship. A steering tiller, spare sweeps and neatly coiled running rigging make the vessel ready for coastal passages.",
				SizeCategory.Gigantic, ItemQuality.Good, 2600000.0, 15800.0m, "pine", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 5, 3, true, "LateenSailcraft"),
		];
	}

	private static IReadOnlyList<VehicleSeedSpec> EarlyModernVehicleExamples()
	{
		return
		[
			CreateDraftCargoVehicle(
				"vehicle_earlymodern_stagecoach", "earlymodern", "Stagecoach",
				"A substantial enclosed coach built for scheduled passenger travel.",
				"stagecoach", "a high-bodied road stagecoach",
				"A tall enclosed coach body hangs on leather thoroughbraces between high iron-rimmed wheels. Bench seats fill the cabin, luggage rails crown the roof and a folding step serves the side door. The driver's box, guard's perch and fittings for a four-horse team give the vehicle the purposeful appearance of regular long-distance service.",
				SizeCategory.Gigantic, ItemQuality.Good, 1450000.0, 14500.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 8, true, false, 8000000.0, 1.0,
				"roof luggage rack", "A broad iron-railed rack spans the coach roof for trunks and mail sacks.",
				"Stagecoach"),
			CreateDraftCargoVehicle(
				"vehicle_earlymodern_freight_dray", "earlymodern", "Freight Dray",
				"A low heavy dray for casks and bulky goods on urban streets.",
				"dray", "a low heavy freight dray",
				"A long, low platform rests over four broad wheels, its stout oak beams left largely open for easy loading. Iron corner hoops, rope cleats and removable stakes secure barrels and bales without enclosing them. A compact driving board and short turning forecarriage suit slow work through crowded streets and yards.",
				SizeCategory.Enormous, ItemQuality.Standard, 920000.0, 6100.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 1, false, false, 9000000.0, 1.0,
				"open freight platform", "The dray presents a broad, low platform fitted with cleats and removable cargo stakes.",
				"FreightDray"),
			CreatePaddleCraft(
				"vehicle_earlymodern_whaleboat", "earlymodern", "Whaleboat",
				"A long double-ended pulling boat made for rough water and rapid handling.",
				"whaleboat", "a long double-ended whaleboat",
				"A narrow double-ended hull rises sharply at bow and stern around a run of evenly spaced rowing thwarts. The light planking is reinforced at the gunwales, where sturdy rowlocks and line cleats stand ready for hard use. Its clean lines, open interior and steering oar favour speed, surf work and coordinated rowing.",
				SizeCategory.Gigantic, ItemQuality.Good, 680000.0, 3900.0m, "pine", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, VehiclePropulsionType.Rowed, 1, 5, false, true, "Whaleboat"),
			CreateSailCraft(
				"vehicle_earlymodern_coastal_sloop", "earlymodern", "Coastal Sloop",
				"A handy single-masted sloop arranged for coastal trade and passenger work.",
				"sloop", "a weathered coastal trading sloop",
				"A full but clean-lined hull supports a single mast, fore-and-aft sails and a short bowsprit. The open working deck surrounds a hatch to a modest cargo hold, while a small stern shelter protects the tiller and charts. Tarred standing rigging, patched canvas and polished wear at the rail speak of regular coastal employment.",
				SizeCategory.Titanic, ItemQuality.Standard, 5400000.0, 28000.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 6, 4, true, "CoastalSloop"),
		];
	}
}
