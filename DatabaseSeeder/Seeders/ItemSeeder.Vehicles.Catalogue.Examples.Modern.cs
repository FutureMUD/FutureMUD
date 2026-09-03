#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static IReadOnlyList<VehicleSeedSpec> RevolutionVehicleExamples()
	{
		var examples = new List<VehicleSeedSpec>
		{
			CreateDraftCargoVehicle(
				"vehicle_revolution_horse_tram", "revolution", "Horse Tramcar",
				"A rail-guided passenger tramcar intended to be hauled by a horse or mule.",
				"tramcar", "a varnished horse-drawn tramcar",
				"A long enclosed passenger body stands on flanged iron wheels beneath a clerestory roof. Glazed windows line both sides, with narrow end platforms for the driver and conductor. A reinforced drawbar projects from the leading platform, allowing a horse or mule to haul the car steadily along prepared rails.",
				SizeCategory.Gigantic, ItemQuality.Good, 3200000.0, 21000.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 18, true, true, 7000000.0, 1.0,
				"under-seat luggage bay", "A long shallow luggage space runs beneath the passenger benches.",
				"HorseTram"),
			CreateDraftCargoVehicle(
				"vehicle_revolution_factory_delivery_wagon", "revolution", "Factory Delivery Wagon",
				"A strong iron-braced wagon for regular commercial deliveries.",
				"wagon", "an iron-braced factory delivery wagon",
				"A rectangular plank body sits on a sprung four-wheel chassis strengthened with wrought-iron brackets. Hinged rear boards and a canvas tilt protect stacked crates while permitting rapid loading at a warehouse dock. The driver's bench, brake lever and paired draft shafts are worn smooth by repetitive commercial service.",
				SizeCategory.Enormous, ItemQuality.Standard, 1350000.0, 7800.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 2, true, false, 10000000.0, 1.0,
				"delivery compartment", "The high-sided delivery body is covered by a canvas tilt and closed by hinged rear boards.",
				"IndustrialWagon"),
			CreatePaddleCraft(
				"vehicle_revolution_canal_skiff", "revolution", "Canal Skiff",
				"A flat-bottomed work skiff for canals, docks and sheltered rivers.",
				"skiff", "a flat-bottomed canal skiff",
				"A broad flat bottom and nearly vertical sides give this open skiff generous carrying room at shallow draught. Two rowing thwarts, iron rowlocks and a squared working bow suit quiet water and frequent contact with wharves. Tarred seams and replaceable rubbing boards emphasise durability over elegance.",
				SizeCategory.Enormous, ItemQuality.Standard, 520000.0, 2600.0m, "pine", "Destroyable_WoodenHeavy",
				VehicleScale.ItemScale, VehiclePropulsionType.Rowed, 1, 2, false, true, "CanalSkiff"),
			CreateSailCraft(
				"vehicle_revolution_sailing_cutter", "revolution", "Sailing Cutter",
				"A fast fore-and-aft-rigged cutter for patrol, dispatch and pilotage.",
				"cutter", "a sharp-lined sailing cutter",
				"A deep narrow hull carries a tall mast, a long bowsprit and a powerful spread of fore-and-aft canvas. The deck is kept clear around a small hatch and low aft cabin, with running rigging led neatly to belaying points near the helm. Copper-toned sheathing at the waterline and a fine entry suggest sustained speed and hard coastal service.",
				SizeCategory.Titanic, ItemQuality.Good, 7200000.0, 41000.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, 8, 4, true, "SailingCutter"),
		};

		// Route-bound rail examples deliberately remain independent vehicles. The current runtime has no
		// rail-consist, coupling or steam-drive strategy, so these do not claim either capability.
		foreach (var (key, name, noun, capacity, cargo) in new[]
		         {
			         ("rail_passenger_car", "Rail Passenger Car", "railcar", 24, "passenger luggage space"),
			         ("rail_luggage_van", "Rail Luggage Van", "van", 2, "luggage compartment"),
			         ("rail_covered_goods_van", "Covered Goods Van", "van", 0, "covered goods compartment"),
			         ("rail_open_goods_wagon", "Open Goods Wagon", "wagon", 0, "open freight bed"),
			         ("rail_flat_wagon", "Rail Flat Wagon", "wagon", 0, "flat freight deck"),
			         ("rail_livestock_wagon", "Livestock Rail Wagon", "wagon", 0, "slatted livestock compartment"),
			         ("rail_brake_van", "Rail Guard Van", "van", 4, "guard equipment compartment"),
			         ("mine_tram_wagon", "Mine Tram Wagon", "tram", 0, "ore hopper"),
			         ("quarry_tram_wagon", "Quarry Tram Wagon", "tram", 0, "stone load bed")
		         })
		{
			examples.Add(CreateDraftCargoVehicle(
				$"vehicle_revolution_{key}", "revolution", name,
				$"An independent route-bound {name.ToLowerInvariant()} for industrial rail and works traffic.",
				noun, $"an iron-framed {name.ToLowerInvariant()}",
				$"Iron framing, flanged wheels and a plainly finished working body identify this {name.ToLowerInvariant()}. Its fittings are arranged for regular guided-route service, with scuffed surfaces and grease-darkened running gear showing sustained industrial use.",
				SizeCategory.Gigantic, ItemQuality.Standard, 1800000.0, 9500.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, capacity, true, true, 8500000.0, 1.0, cargo,
				$"A fixed {cargo} is built into the vehicle body.", "IndustrialRailVehicle"));
		}

		foreach (var (key, name, noun, passengerCapacity, cargo) in new[]
		         {
			         ("street_omnibus", "Horse Omnibus", "omnibus", 14, "passenger luggage rack"),
			         ("mail_cart", "Mail Cart", "cart", 1, "locked mail chest"),
			         ("fire_engine", "Horse-Drawn Fire Engine", "engine", 4, "firefighting equipment bed"),
			         ("brewery_dray", "Brewery Dray", "dray", 1, "barrel load bed"),
			         ("coal_wagon", "Coal Delivery Wagon", "wagon", 0, "coal delivery bed"),
			         ("farm_tractor_wagon", "Steam Works Wagon", "wagon", 1, "works load bed"),
			         ("hearse", "Funeral Hearse", "hearse", 2, "coffin compartment"),
			         ("cab", "Four-Wheeled Cab", "cab", 2, "small luggage boot"),
			         ("delivery_tricycle", "Delivery Tricycle", "tricycle", 0, "front delivery box")
		         })
		{
			examples.Add(CreateDraftCargoVehicle(
				$"vehicle_revolution_{key}", "revolution", name,
				$"A practical {name.ToLowerInvariant()} for regular urban or commercial journeys.",
				noun, $"a working {name.ToLowerInvariant()}",
				$"A stout timber body sits on iron-tyred wheels beneath the functional fittings of this {name.ToLowerInvariant()}. Its shafts, brake gear and load-bearing surfaces are worn smooth where drivers and handlers use them every day.",
				SizeCategory.Huge, ItemQuality.Standard, 650000.0, 4200.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, passengerCapacity, true, false, 3500000.0, 1.0, cargo,
				$"A purpose-built {cargo} is integrated into the vehicle body.", "IndustrialRoadVehicle"));
		}

		foreach (var (key, name, noun, propulsion, crew, passengers, cargo) in new[]
		         {
			         ("canal_narrowboat", "Canal Narrowboat", "narrowboat", VehiclePropulsionType.Rowed, 2, 4, true),
			         ("canal_cargo_boat", "Canal Cargo Boat", "boat", VehiclePropulsionType.Rowed, 3, 1, true),
			         ("river_workboat", "River Workboat", "workboat", VehiclePropulsionType.Rowed, 2, 2, true),
			         ("harbour_launch", "Harbour Launch", "launch", VehiclePropulsionType.Rowed, 2, 6, true),
			         ("fishing_dory", "Fishing Dory", "dory", VehiclePropulsionType.Rowed, 2, 1, true),
			         ("lifeboat", "Pulling Lifeboat", "lifeboat", VehiclePropulsionType.Rowed, 6, 6, false),
			         ("river_ferry_boat", "River Ferry Boat", "ferry", VehiclePropulsionType.Rowed, 4, 12, true),
			         ("canal_inspection_skiff", "Canal Inspection Skiff", "skiff", VehiclePropulsionType.Rowed, 1, 1, false),
			         ("coastal_fishing_smack", "Coastal Fishing Smack", "smack", VehiclePropulsionType.Rowed, 3, 2, true)
		         })
		{
			examples.Add(CreatePaddleCraft(
				$"vehicle_revolution_{key}", "revolution", name,
				$"A working {name.ToLowerInvariant()} for sheltered industrial-era waters.",
				noun, $"a working {name.ToLowerInvariant()}",
				$"Planked sides, reinforced thwarts and practical iron fittings give this {name.ToLowerInvariant()} a serviceable appearance. Its rubbed gunwales and tarred seams show regular contact with wharves, banks and working water.",
				SizeCategory.Enormous, ItemQuality.Standard, 480000.0, 3700.0m, "pine", "Destroyable_WoodenHeavy",
				VehicleScale.ItemScale, propulsion, 1, crew, false, cargo, "IndustrialWatercraft"));
		}

		foreach (var (key, name, passengers, crew) in new[]
		         {
			         ("coastal_sloop", "Coastal Sloop", 4, 3),
			         ("pilot_cutter", "Pilot Cutter", 5, 4),
			         ("sailing_barge", "Sailing Barge", 2, 4),
			         ("packet_schooner", "Packet Schooner", 12, 6),
			         ("fishing_lugger", "Fishing Lugger", 2, 5),
			         ("coastal_cargo_ketch", "Coastal Cargo Ketch", 3, 5),
			         ("river_sailing_barge", "River Sailing Barge", 4, 3),
			         ("survey_cutter", "Survey Cutter", 5, 4),
			         ("harbour_tender", "Harbour Tender", 8, 3)
		         })
		{
			examples.Add(CreateSailCraft(
				$"vehicle_revolution_{key}", "revolution", name,
				$"A sail-driven {name.ToLowerInvariant()} for industrial-era coastal and harbour work.",
				"vessel", $"a working {name.ToLowerInvariant()}",
				$"A practical planked hull supports the masts, rigging and broad working deck of this {name.ToLowerInvariant()}. Its deck fittings favour cargo, passengers and repeated coastal service over decorative finish.",
				SizeCategory.Titanic, ItemQuality.Standard, 4200000.0, 28500.0m, "oak", "Destroyable_WoodenHeavy",
				VehicleScale.RoomContainer, passengers, crew, true, "IndustrialSailingVessel"));
		}

		return examples;
	}

	private static IReadOnlyList<VehicleSeedSpec> ModernVehicleExamples()
	{
		return
		[
			CreatePoweredRoadVehicle(
				"vehicle_modern_petrol_touring_car", "modern", "Petrol Touring Car",
				"A steel-bodied touring car powered by a removable petrol drive module.",
				"car", "a steel-bodied petrol touring car",
				"A long bonnet, separate mudguards and a compact enclosed cabin define this early mass-produced touring car. Painted steel panels cover a ladder chassis, while broad running boards connect the front and rear doors. A steering wheel, mechanical controls and a rear luggage deck complete a practical road vehicle built for four occupants.",
				SizeCategory.Gigantic, ItemQuality.Standard, 1250000.0, 18500.0m, "mild steel", "Destroyable_HeavyMetal",
				VehicleScale.RoomContainer, 4, true, false, "gasoline", VehicleLandEngineMountType,
				false, false, 0.0, 0.0, "TouringCar"),
			CreatePoweredRoadVehicle(
				"vehicle_modern_diesel_delivery_lorry", "modern", "Diesel Delivery Lorry",
				"A rigid commercial lorry with an enclosed cargo body and diesel drive module.",
				"lorry", "a box-bodied diesel delivery lorry",
				"A forward steel cab sits ahead of a long rectangular cargo body on a heavy ladder frame. Twin rear wheels, leaf springs and a broad loading door reveal the vehicle's commercial purpose. The plain painted panels carry scuffs and repairs around the corners, while mirrors and lamps bracket the upright cab.",
				SizeCategory.Titanic, ItemQuality.Standard, 5200000.0, 49000.0m, "mild steel", "Destroyable_HeavyMetal",
				VehicleScale.RoomContainer, 2, true, false, "diesel", VehicleLandEngineMountType,
				false, false, 0.0, 0.0, "DeliveryLorry"),
			CreatePaddleCraft(
				"vehicle_modern_aluminium_dinghy", "modern", "Aluminium Dinghy",
				"A light riveted dinghy for rowing, fishing and short shore work.",
				"dinghy", "a riveted aluminium dinghy",
				"Pressed aluminium panels form a light open hull with a shallow vee and broad transom. Three bench seats brace the sides, while fitted rowlocks and a reinforced stern permit either oars or a small motor. Dull oxidation, shallow dents and replaceable rubbing strips give it the appearance of durable utility equipment.",
				SizeCategory.Enormous, ItemQuality.Standard, 135000.0, 3400.0m, "aluminium", "Destroyable_Misc",
				VehicleScale.ItemScale, VehiclePropulsionType.Rowed, 1, 2, false, true, "AluminiumDinghy"),
			CreateMotorCraft(
				"vehicle_modern_petrol_motor_launch", "modern", "Petrol Motor Launch",
				"A compact open launch prepared for an outboard motor and sheltered-water service.",
				"launch", "a compact petrol motor launch",
				"A hard-chined painted hull surrounds an open cockpit with two bench seats and a small forward locker. The transom is heavily reinforced for an outboard motor, and simple wheel-and-cable steering leads to the stern. Cleats, grab rails and a low spray screen equip the launch for practical transport rather than luxury.",
				SizeCategory.Gigantic, ItemQuality.Standard, 780000.0, 12500.0m, "mild steel", "Destroyable_HeavyMetal",
				VehicleScale.RoomContainer, 5, true, "MotorLaunch"),
		];
	}

	private static IReadOnlyList<VehicleSeedSpec> AtomicVehicleExamples()
	{
		return
		[
			CreatePoweredRoadVehicle(
				"vehicle_atomic_family_saloon", "atomic", "Family Saloon",
				"A rounded steel family saloon with a petrol drive module and enclosed boot.",
				"car", "a rounded family saloon",
				"Smooth pressed-steel panels enclose a broad passenger cabin beneath a single curved roof. Four doors, upholstered bench seats and a separate rear boot make the car plainly domestic, while chrome trim and a wide radiator grille provide restrained ornament. The suspension and controls favour easy road use over sporting performance.",
				SizeCategory.Gigantic, ItemQuality.Good, 1420000.0, 23500.0m, "mild steel", "Destroyable_HeavyMetal",
				VehicleScale.RoomContainer, 5, true, false, "gasoline", VehicleLandEngineMountType,
				false, false, 0.0, 0.0, "FamilySaloon"),
			CreatePoweredRoadVehicle(
				"vehicle_atomic_intercity_coach", "atomic", "Intercity Coach",
				"A long diesel passenger coach designed for scheduled route service.",
				"coach", "a long diesel intercity coach",
				"A monocoque steel body stretches over closely spaced passenger windows and two broad axles. Folding entry doors open beside the driver's position, while rows of high-backed seats stand above underfloor luggage lockers. Destination panels, roof vents and substantial bumpers identify a vehicle intended for repeated scheduled journeys.",
				SizeCategory.Titanic, ItemQuality.Standard, 11800000.0, 98000.0m, "mild steel", "Destroyable_HeavyMetal",
				VehicleScale.RoomContainer, 36, true, true, "diesel", VehicleLandEngineMountType,
				false, true, 18.0, 0.00008, "IntercityCoach"),
			CreateMotorCraft(
				"vehicle_atomic_fiberglass_runabout", "atomic", "Fiberglass Runabout",
				"A small planing runabout with a moulded hull and outboard mounting.",
				"runabout", "a bright fiberglass runabout",
				"A glossy moulded hull sweeps back from a flared bow to a reinforced outboard transom. A wraparound windscreen shelters the wheel and forward seats, with a padded bench spanning the cockpit behind them. Stainless fittings, vinyl upholstery and a shallow planing bottom give the craft a distinctly recreational character.",
				SizeCategory.Gigantic, ItemQuality.Good, 720000.0, 16800.0m, "fiberglass", "Destroyable_Misc",
				VehicleScale.RoomContainer, 5, false, "Runabout"),
			CreateMotorCraft(
				"vehicle_atomic_cabin_cruiser", "atomic", "Cabin Cruiser",
				"A compact leisure cruiser with an enclosed cabin, cockpit and outboard installation.",
				"cruiser", "a compact fiberglass cabin cruiser",
				"A moulded white hull supports an open aft cockpit and a low enclosed cabin beneath a raked windscreen. Stainless rails follow the foredeck, while cushioned seating and a small covered berth make the boat suitable for overnight coastal trips. The broad stern carries a reinforced motor well and a narrow swim platform.",
				SizeCategory.Titanic, ItemQuality.Good, 3400000.0, 52000.0m, "fiberglass", "Destroyable_Misc",
				VehicleScale.RoomContainer, 7, true, "CabinCruiser"),
		];
	}

	private static IReadOnlyList<VehicleSeedSpec> ComputerVehicleExamples()
	{
		return
		[
			CreatePoweredRoadVehicle(
				"vehicle_computer_electric_city_car", "computer", "Electric City Car",
				"A compact aluminium city car powered by a removable electric drive module.",
				"car", "a compact electric city car",
				"A short aluminium body encloses a tall four-seat cabin with a steep windscreen and minimal overhangs. Flush lamps, simple door handles and a sealed nose replace the openings expected on a combustion vehicle. A digital instrument panel faces the driver, while a rear hatch opens onto a small but regular load space.",
				SizeCategory.Gigantic, ItemQuality.Good, 1380000.0, 42000.0m, "aluminium", "Destroyable_HeavyMetal",
				VehicleScale.RoomContainer, 4, true, false, null, VehicleElectricDriveMountType,
				false, false, 0.0, 0.0, "ElectricCityCar"),
			CreatePoweredRoadVehicle(
				"vehicle_computer_autonomous_shuttle", "computer", "Autonomous Shuttle",
				"A low-speed electric passenger shuttle approved for automatic route operation.",
				"shuttle", "a windowed autonomous electric shuttle",
				"A boxy aluminium passenger pod rides on four small enclosed wheels beneath a roof crowded with cameras and range sensors. Wide sliding doors open onto a level floor and facing bench seats, with only a compact manual control console interrupting the cabin. External status lights and destination displays make its route-service role immediately apparent.",
				SizeCategory.Titanic, ItemQuality.Good, 4200000.0, 86000.0m, "aluminium", "Destroyable_HeavyMetal",
				VehicleScale.RoomContainer, 12, true, true, null, VehicleElectricDriveMountType,
				true, true, 8.0, 35.0, "AutonomousShuttle"),
			CreatePaddleCraft(
				"vehicle_computer_recreational_kayak", "computer", "Recreational Kayak",
				"A moulded sit-in kayak for one paddler and light personal gear.",
				"kayak", "a moulded recreational kayak",
				"A slender moulded hull surrounds a single keyhole cockpit with an adjustable seat and foot braces. Deck lines cross the pointed bow and stern, and a sealed hatch gives access to a small dry compartment. The shallow vee, modest rocker and scuffed underside suit calm rivers, lakes and sheltered coasts.",
				SizeCategory.VeryLarge, ItemQuality.Good, 24000.0, 1800.0m, "fiberglass", "Destroyable_Misc",
				VehicleScale.ItemScale, VehiclePropulsionType.SelfPowered, 1, 0, true, false, "Kayak"),
			CreateMotorCraft(
				"vehicle_computer_rescue_rib", "computer", "Rescue Rigid-Inflatable Boat",
				"A fast rescue boat with a rigid hull, buoyant collar and outboard installation.",
				"boat", "a high-visibility rescue rigid-inflatable boat",
				"A deep-vee rigid hull is wrapped by a thick segmented buoyancy collar with grab lines along both sides. A central steering console, shock-mitigating seats and an open working deck leave room for crew and casualties. Reinforced lifting points, navigation lights and a broad outboard transom equip the craft for rapid rescue work in difficult water.",
				SizeCategory.Titanic, ItemQuality.Good, 1850000.0, 76000.0m, "fiberglass", "Destroyable_Misc",
				VehicleScale.RoomContainer, 8, true, "RescueRIB")
		];
	}
}
