#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static VehiclePrototypeSeedSpec MedievalHandcart()
	{
		const string key = "medieval_vehicle_two_wheel_handcart";
		return new VehiclePrototypeSeedSpec(
			key,
			"medieval",
			"Medieval Two-Wheel Handcart",
			"A small hand-pulled cart with an open bed, handles usable as a control station, and a direct-pull front hitch.",
			VehicleScale.ItemScale,
			Exterior(
				$"{key}_exterior",
				"handcart",
				"a sturdy two-wheeled handcart",
				"A sturdy two-wheeled handcart stands here.",
				"This compact handcart has a shallow ash bed balanced over two iron-rimmed wheels. Long handles project from the front, their grips polished by use, and a folding rear leg keeps the cart level when set down. Peg holes around the rim accept ropes or removable stakes for awkward loads.",
				SizeCategory.VeryLarge,
				ItemQuality.Standard,
				78000.0,
				220.0m,
				"ash",
				MedievalVehicleEraTag,
				"Functions / Vehicles / Terrestrial Vehicles",
				"Functions / Vehicles / Human-Powered Vehicles",
				"Market / Transportation / Cargo Transportation / Manual Haulage"),
			[Compartment("bed", "Cart Bed", "The shallow handcart bed and its forward handles.", 1)],
			[],
			[DriverSlot("handler", "bed", "cart handler position", true)],
			[PrimaryStation("handler", "handcart handles")],
			[GroundMovement(false)],
			[],
			[
				Cargo(
					"bed",
					"bed",
					null,
					"Open Load Bed",
					"The handcart's shallow open cargo bed.",
					1,
					CargoItem(
						$"{key}_load_bed",
						"bed",
						"a shallow handcart bed",
						"This shallow ash bed is bounded by low rails and pierced with tie holes around the rim. Its boards are worn pale where sacks, firewood, tools, and market baskets have rubbed across them.",
						SizeCategory.Large,
						16000.0,
						"ash",
						"Container_Open_Bin"))
			],
			[],
			[FrontTow("pulling handles", "The long handles can be used as a direct hand-pull hitch.", "hand", 1, 1.6)],
			[Damage("frame", "Frame and Wheels", "The load bed, axle, wheels, handles, and supporting frame.", 85.0, 2.0, 50.0, 85.0, true)]);
	}

	private static VehiclePrototypeSeedSpec MedievalCoveredWagon()
	{
		const string key = "medieval_vehicle_covered_freight_wagon";
		return new VehiclePrototypeSeedSpec(
			key,
			"medieval",
			"Medieval Covered Freight Wagon",
			"A four-wheeled freight wagon with a canvas tilt, driver's bench, rear access flap, cargo space, and harness hitch.",
			VehicleScale.RoomContainer,
			Exterior(
				$"{key}_exterior",
				"wagon",
				"a canvas-covered freight wagon",
				"A canvas-covered freight wagon stands here.",
				"This long wagon rides on four iron-tyred wheels beneath a stout oak chassis. Bent hoops carry a weathered canvas tilt over the plank bed, leaving a driver's bench exposed at the front and a laced flap at the rear. The tongue, evener, and trace fittings are heavy enough for a draught team and a substantial road load.",
				SizeCategory.Huge,
				ItemQuality.Standard,
				820000.0,
				2400.0m,
				"oak",
				MedievalVehicleEraTag,
				"Functions / Vehicles / Terrestrial Vehicles",
				"Functions / Vehicles / Animal-Drawn Vehicles",
				"Market / Transportation / Cargo Transportation / Cart Haulage",
				"Market / Transportation / Passenger Transportation / Wagon Passage"),
			[Compartment("wagon", "Covered Wagon Bed", "The covered freight bed and forward driver's bench.", 1)],
			[],
			[
				DriverSlot("driver", "wagon", "driver bench", true),
				PassengerSlot("passengers", "wagon", "covered riding space", 4)
			],
			[PrimaryStation("driver", "reins and wheel brake")],
			[GroundMovement(true)],
			[
				Access(
					"rear_flap",
					"wagon",
					"Rear Tilt Flap",
					"A laced canvas flap at the rear of the wagon.",
					VehicleAccessPointType.Canopy,
					false,
					true,
					1,
					AccessItem(
						$"{key}_rear_flap",
						"flap",
						"a laced canvas wagon flap",
						"This heavy canvas flap is edged with leather and pierced for lacing down across the wagon's rear hoop. Rain stains and road dust mark its lower edge, while several interior ties let it be rolled upward when loading.",
						SizeCategory.Large,
						9000.0,
						"canvas",
						"Destroyable_Clothing"))
			],
			[
				Cargo(
					"freight_bed",
					"wagon",
					"rear_flap",
					"Covered Freight Bed",
					"The enclosed plank bed beneath the wagon tilt.",
					1,
					CargoItem(
						$"{key}_freight_bed",
						"bed",
						"a covered wagon freight bed",
						"This long plank bed lies beneath the canvas tilt, with raised sides, rope cleats, and crossbars for bracing cargo. The boards are darkened by damp straw and deeply scored by casks, chests, and iron-bound loads.",
						SizeCategory.Huge,
						120000.0,
						"oak",
						"Container_Colossal"))
			],
			[],
			[
				FrontTow("team hitch", "A reinforced harness hitch and evener for the draught team.", "harness", 1),
				RearTow("rear tow hitch", "A rear hitch suitable for a light cart or dragged load.", "hitch", 1600.0, 2)
			],
			[
				Damage(
					"running_gear",
					"Running Gear and Body",
					"The wheels, axles, chassis, tongue, bed, and tilt hoops.",
					260.0,
					4.0,
					165.0,
					260.0,
					true,
					new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.AccessPoint, "rear_flap", VehicleSystemStatus.Disabled),
					new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.CargoSpace, "freight_bed", VehicleSystemStatus.Disabled))
			]);
	}

	private static VehiclePrototypeSeedSpec MedievalClinkerRowboat()
	{
		const string key = "medieval_vehicle_clinker_rowboat";
		return new VehiclePrototypeSeedSpec(
			key,
			"medieval",
			"Medieval Clinker Rowboat",
			"A sturdy open rowboat with a stern steering place, two propulsion benches, and limited passenger capacity.",
			VehicleScale.ItemScale,
			Exterior(
				$"{key}_exterior",
				"rowboat",
				"a clinker-built open rowboat",
				"A clinker-built open rowboat floats here.",
				"This open boat is formed from overlapping oak strakes clenched to curved ribs with dark iron nails. Three thwarts cross the hull, the middle pair worn smooth by rowers, while a short stern board gives the steersman a firm place to work. Tarred fibres show between the lowest planks and a rope fender hangs along one gunwale.",
				SizeCategory.Enormous,
				ItemQuality.Standard,
				260000.0,
				900.0m,
				"oak",
				MedievalVehicleEraTag,
				"Functions / Vehicles / Aquatic Vehicles",
				"Functions / Vehicles / Human-Powered Vehicles",
				"Market / Transportation / Passenger Transportation / Ship Passage"),
			[Compartment("hull", "Open Hull", "The open hull, thwarts, and stern steering place.", 1)],
			[],
			[
				DriverSlot("driver", "hull", "stern steering position", true, false, Difficulty.Normal),
				CrewSlot("rowers", "hull", "rowing thwarts", 2, true, Difficulty.Normal),
				PassengerSlot("passengers", "hull", "passenger space", 2, Difficulty.Normal)
			],
			[PrimaryStation("driver", "tiller and steering oar")],
			[WaterMovement(true, RowedPropulsion(10000.0, Difficulty.Normal, true))],
			[],
			[],
			[],
			[RearTow("stern line", "A stern eye and bitt for a rope tow or mooring line.", "rope", 600.0, 1)],
			[Damage("hull", "Hull", "The clinker hull, ribs, thwarts, and steering fittings.", 110.0, 2.0, 65.0, 110.0, true)]);
	}

	private static VehiclePrototypeSeedSpec MedievalCoastalCog()
	{
		const string key = "medieval_vehicle_small_coastal_cog";
		return new VehiclePrototypeSeedSpec(
			key,
			"medieval",
			"Medieval Small Coastal Cog",
			"A compact decked sailing vessel with a stern control position, working crew, boarding ramp, and capacious cargo hold.",
			VehicleScale.RoomContainer,
			Exterior(
				$"{key}_exterior",
				"cog",
				"a high-sided coastal cog",
				"A high-sided coastal cog rides at anchor here.",
				"This compact merchant vessel has a broad oak hull with high clinker-built sides and a single square-rigged mast. Short raised platforms at bow and stern overlook an open waist, while a heavy side-hung steering rudder works beside the after platform. Thick wales, tarred seams, and a deep hold give the ship a blunt, workmanlike profile.",
				SizeCategory.Gigantic,
				ItemQuality.Standard,
				18000000.0,
				48000.0m,
				"oak",
				MedievalVehicleEraTag,
				"Functions / Vehicles / Aquatic Vehicles",
				"Functions / Vehicles / Sailing Vehicles",
				"Market / Transportation / Cargo Transportation / Ship Haulage",
				"Market / Transportation / Passenger Transportation / Ship Passage"),
			[Compartment("deck", "Main Deck", "The open waist and raised fore and stern working platforms.", 1)],
			[],
			[
				DriverSlot("driver", "deck", "stern steering position", true, false, Difficulty.Easy),
				CrewSlot("crew", "deck", "working deck positions", 5, false, Difficulty.Easy),
				PassengerSlot("passengers", "deck", "deck passenger space", 8, Difficulty.Easy)
			],
			[PrimaryStation("driver", "side rudder and sail commands")],
			[WaterMovement(false, SailPropulsion(18000.0, true))],
			[
				Access(
					"boarding_ramp",
					"deck",
					"Boarding Ramp",
					"A heavy side ramp used for boarding and quay access.",
					VehicleAccessPointType.Ramp,
					false,
					true,
					1,
					AccessItem(
						$"{key}_boarding_ramp",
						"ramp",
						"a heavy oak boarding ramp",
						"This heavy oak ramp is cross-cleated underfoot and bound with iron at its hinge points. Thick rope falls run through lifting rings so the crew can raise it against the ship's side before sailing.",
						SizeCategory.VeryLarge,
						78000.0,
						"oak")),
				Access(
					"hold_hatch",
					"deck",
					"Hold Hatch",
					"A broad weathered hatch above the cargo hold.",
					VehicleAccessPointType.Hatch,
					false,
					true,
					2,
					AccessItem(
						$"{key}_hold_hatch",
						"hatch",
						"a broad tarred hold hatch",
						"This broad hatch is assembled from cross-braced oak boards with raised coamings around its opening. Tarred canvas strips and rope lashings help secure it when weather threatens the cargo below.",
						SizeCategory.VeryLarge,
						92000.0,
						"oak"))
			],
			[
				Cargo(
					"hold",
					"deck",
					"hold_hatch",
					"Main Cargo Hold",
					"The deep central hold beneath the main deck.",
					1,
					CargoItem(
						$"{key}_cargo_hold",
						"hold",
						"a deep timber cargo hold",
						"This deep hold follows the vessel's broad curved ribs beneath a deck of removable boards. Timber dunnage, rope eyes, and low partitions provide places to brace barrels, bales, and chests against the ship's motion.",
						SizeCategory.Huge,
						360000.0,
						"oak",
						"Container_Colossal"))
			],
			[],
			[RearTow("stern tow bitt", "A heavy stern bitt for towing a boat or taking a warping line.", "rope", 5000.0, 1)],
			[
				Damage(
					"hull",
					"Hull, Rig, and Steering",
					"The hull, mast partners, rigging anchors, steering rudder, and deck structure.",
					520.0,
					8.0,
					330.0,
					520.0,
					true,
					new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.AccessPoint, "boarding_ramp", VehicleSystemStatus.Disabled),
					new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.AccessPoint, "hold_hatch", VehicleSystemStatus.Disabled),
					new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.CargoSpace, "hold", VehicleSystemStatus.Disabled))
			]);
	}

}
