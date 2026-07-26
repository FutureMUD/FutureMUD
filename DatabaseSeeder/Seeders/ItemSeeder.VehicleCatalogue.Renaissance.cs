#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static VehiclePrototypeSeedSpec RenaissancePassengerCoach()
	{
		const string key = "renaissance_vehicle_passenger_coach";
		return new VehiclePrototypeSeedSpec(
			key,
			"renaissance",
			"Renaissance Passenger Coach",
			"A suspended enclosed passenger coach with a box seat, side door, luggage boot, and team harness hitch.",
			VehicleScale.RoomContainer,
			Exterior(
				$"{key}_exterior",
				"coach",
				"an enclosed leather-slung coach",
				"An enclosed leather-slung coach stands here.",
				"This enclosed coach body hangs from thick leather braces above four tall, iron-tyred wheels. Panelled oak sides frame small shuttered windows and a single side door, while a raised box seat places the driver above the forward axle. The undercarriage is heavily timbered, with a rear luggage boot and a long pole arranged for a matched team.",
				SizeCategory.Huge,
				ItemQuality.Good,
				980000.0,
				7200.0m,
				"oak",
				RenaissanceVehicleEraTag,
				"Functions / Vehicles / Terrestrial Vehicles",
				"Functions / Vehicles / Animal-Drawn Vehicles",
				"Market / Transportation / Passenger Transportation / Wagon Passage"),
			[Compartment("cabin", "Passenger Cabin", "The enclosed passenger cabin and raised exterior box seat.", 1)],
			[],
			[
				DriverSlot("driver", "cabin", "coachman's box", true),
				PassengerSlot("passengers", "cabin", "cabin seats", 4)
			],
			[PrimaryStation("driver", "reins and coach brake")],
			[GroundMovement(true)],
			[
				Access(
					"side_door",
					"cabin",
					"Side Door",
					"A panelled side door opening into the passenger cabin.",
					VehicleAccessPointType.Door,
					false,
					true,
					1,
					AccessItem(
						$"{key}_side_door",
						"door",
						"a panelled coach door",
						"This narrow oak door is panelled on the outside and padded with cloth on the cabin face. Iron hinges, a leather pull, and a simple latch plate fit it closely into the coach body.",
						SizeCategory.Large,
						34000.0,
						"oak"))
			],
			[
				Cargo(
					"boot",
					"cabin",
					null,
					"Rear Luggage Boot",
					"A strapped rear boot for luggage and travelling necessities.",
					1,
					CargoItem(
						$"{key}_luggage_boot",
						"boot",
						"a strapped coach luggage boot",
						"This stout leather-covered boot is fixed behind the coach body beneath a web of buckled straps. Its oak-lined cavity is sized for travelling cases, folded cloaks, tools, and compact baggage.",
						SizeCategory.Large,
						46000.0,
						"leather",
						"Container_Trunk",
						"Destroyable_Clothing"))
			],
			[],
			[
				FrontTow("team hitch", "A long pole and harness tree for a coach team.", "harness", 1),
				RearTow("rear hitch", "A rear hitch for a small luggage trailer or recovery line.", "hitch", 1200.0, 2)
			],
			[
				Damage(
					"undercarriage",
					"Undercarriage and Coach Body",
					"The wheels, axles, suspension braces, body frame, and team pole.",
					300.0,
					4.0,
					185.0,
					300.0,
					true,
					new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.AccessPoint, "side_door", VehicleSystemStatus.Disabled),
					new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.CargoSpace, "boot", VehicleSystemStatus.Disabled))
			]);
	}

	private static VehiclePrototypeSeedSpec RenaissanceSupplyWagon()
	{
		const string key = "renaissance_vehicle_heavy_supply_wagon";
		return new VehiclePrototypeSeedSpec(
			key,
			"renaissance",
			"Renaissance Heavy Supply Wagon",
			"A large open supply wagon with a driver's bench, removable tail ramp, capacious freight bed, and front and rear hitches.",
			VehicleScale.RoomContainer,
			Exterior(
				$"{key}_exterior",
				"wagon",
				"a heavy four-wheeled supply wagon",
				"A heavy four-wheeled supply wagon stands here.",
				"This large supply wagon is built on a deep oak chassis with broad iron-tyred wheels and a high plank body. A narrow bench spans the front behind a reinforced draught pole, while the rear board lowers to form a loading ramp. Iron straps bind the hubs, corners, and drawgear against the punishment of poor roads and heavy military or mercantile loads.",
				SizeCategory.Huge,
				ItemQuality.Standard,
				1100000.0,
				3400.0m,
				"oak",
				RenaissanceVehicleEraTag,
				"Functions / Vehicles / Terrestrial Vehicles",
				"Functions / Vehicles / Animal-Drawn Vehicles",
				"Market / Transportation / Cargo Transportation / Cart Haulage"),
			[Compartment("bed", "Supply Bed", "The high-sided freight bed and forward driver's bench.", 1)],
			[],
			[
				DriverSlot("driver", "bed", "driver bench", true),
				PassengerSlot("guards", "bed", "riding rail", 2)
			],
			[PrimaryStation("driver", "reins and wagon brake")],
			[GroundMovement(true)],
			[
				Access(
					"tail_ramp",
					"bed",
					"Tail Ramp",
					"The wagon's hinged rear board, lowered for loading.",
					VehicleAccessPointType.Ramp,
					false,
					true,
					1,
					AccessItem(
						$"{key}_tail_ramp",
						"ramp",
						"a broad wagon tail ramp",
						"This broad oak tailboard is cross-braced and hung on heavy iron hinges. Chains limit its drop when lowered, allowing barrels, powder chests, provisions, or field equipment to be rolled into the wagon bed.",
						SizeCategory.VeryLarge,
						68000.0,
						"oak"))
			],
			[
				Cargo(
					"bed",
					"bed",
					"tail_ramp",
					"Supply Bed",
					"The wagon's high-sided open freight bed.",
					1,
					CargoItem(
						$"{key}_supply_bed",
						"bed",
						"a high-sided wagon supply bed",
						"This deep oak bed has a plank floor, high reinforced sides, and rows of rope eyes beneath the top rail. Scuffs, iron stains, and crushed straw mark a space made for dense cargo rather than comfort.",
						SizeCategory.Huge,
						150000.0,
						"oak",
						"Container_Colossal"))
			],
			[],
			[
				FrontTow("team hitch", "A reinforced harness tree and draught pole for a heavy team.", "harness", 1),
				RearTow("rear tow bar", "A reinforced rear drawbar for another light wagon or field piece.", "towbar", 2400.0, 2)
			],
			[
				Damage(
					"running_gear",
					"Running Gear and Load Body",
					"The chassis, wheels, axles, draught gear, tailboard, and plank body.",
					340.0,
					5.0,
					210.0,
					340.0,
					true,
					new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.AccessPoint, "tail_ramp", VehicleSystemStatus.Disabled),
					new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.CargoSpace, "bed", VehicleSystemStatus.Disabled))
			]);
	}

	private static VehiclePrototypeSeedSpec RenaissanceSailingPinnace()
	{
		const string key = "renaissance_vehicle_sailing_pinnace";
		return new VehiclePrototypeSeedSpec(
			key,
			"renaissance",
			"Renaissance Sailing Pinnace",
			"A light open utility boat with steering, rowing positions, passenger space, and alternate rowed or sail propulsion.",
			VehicleScale.RoomContainer,
			Exterior(
				$"{key}_exterior",
				"pinnace",
				"a light oared sailing pinnace",
				"A light oared sailing pinnace floats here.",
				"This light open boat has a sharp clinker-built hull, several rowing thwarts, and a short mast carrying a simple fore-and-aft sail. A stern tiller controls the rudder, while removable floorboards keep feet and baggage above the bilge. The narrow bow and moderate freeboard suit ship-to-shore work, scouting, and short coastal passages.",
				SizeCategory.Enormous,
				ItemQuality.Good,
				920000.0,
				4200.0m,
				"oak",
				RenaissanceVehicleEraTag,
				"Functions / Vehicles / Aquatic Vehicles",
				"Functions / Vehicles / Human-Powered Vehicles",
				"Functions / Vehicles / Sailing Vehicles",
				"Market / Transportation / Passenger Transportation / Ship Passage"),
			[Compartment("hull", "Open Hull", "The thwarts, stern sheets, and open working space of the pinnace.", 1)],
			[],
			[
				DriverSlot("driver", "hull", "stern tiller position", true, false, Difficulty.Normal),
				CrewSlot("rowers", "hull", "rowing thwarts", 4, true, Difficulty.Normal),
				PassengerSlot("passengers", "hull", "passenger space", 4, Difficulty.Normal)
			],
			[PrimaryStation("driver", "tiller and sail controls")],
			[WaterMovement(true, RowedPropulsion(12000.0, Difficulty.Normal, true), SailPropulsion(10000.0, false))],
			[],
			[
				Cargo(
					"locker",
					"hull",
					null,
					"Stern Locker",
					"A low locker beneath the stern sheets.",
					1,
					CargoItem(
						$"{key}_stern_locker",
						"locker",
						"a low pinnace stern locker",
						"This low oak locker sits beneath the stern bench with a close-fitting lid and rope pull. It is large enough for spare cordage, tools, provisions, and a few compact travelling bundles.",
						SizeCategory.Large,
						28000.0,
						"oak",
						"Container_Trunk"))
			],
			[],
			[RearTow("stern towing eye", "A stern towing eye and bitt for rope work.", "rope", 1600.0, 1)],
			[
				Damage(
					"hull_and_rig",
					"Hull and Rig",
					"The clinker hull, rudder, mast step, thwarts, and standing rigging.",
					190.0,
					3.0,
					115.0,
					190.0,
					true,
					new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.CargoSpace, "locker", VehicleSystemStatus.Disabled))
			]);
	}

	private static VehiclePrototypeSeedSpec RenaissanceLateenMerchantBoat()
	{
		const string key = "renaissance_vehicle_lateen_merchant_boat";
		return new VehiclePrototypeSeedSpec(
			key,
			"renaissance",
			"Renaissance Lateen Merchant Boat",
			"A decked coastal merchant boat with a lateen sail, stern steering position, working crew, boarding ramp, and cargo hold.",
			VehicleScale.RoomContainer,
			Exterior(
				$"{key}_exterior",
				"boat",
				"a lateen-rigged merchant boat",
				"A lateen-rigged merchant boat rides the water here.",
				"This beamy merchant boat carries a long slanting yard and triangular sail above a carvel-planked oak hull. A short afterdeck shelters the tiller, while the open waist surrounds a broad cargo hatch and working space for the crew. Tarred rigging, fenders, and patches of rubbed paint speak of frequent coastal loading rather than naval display.",
				SizeCategory.Gigantic,
				ItemQuality.Standard,
				22000000.0,
				62000.0m,
				"oak",
				RenaissanceVehicleEraTag,
				"Functions / Vehicles / Aquatic Vehicles",
				"Functions / Vehicles / Sailing Vehicles",
				"Market / Transportation / Cargo Transportation / Ship Haulage",
				"Market / Transportation / Passenger Transportation / Ship Passage"),
			[Compartment("deck", "Working Deck", "The open waist, afterdeck, and forward working space.", 1)],
			[],
			[
				DriverSlot("driver", "deck", "stern tiller position", true, false, Difficulty.Easy),
				CrewSlot("crew", "deck", "sailing crew positions", 5, false, Difficulty.Easy),
				PassengerSlot("passengers", "deck", "deck passenger space", 10, Difficulty.Easy)
			],
			[PrimaryStation("driver", "tiller and lateen rig controls")],
			[WaterMovement(false, SailPropulsion(16000.0, true))],
			[
				Access(
					"boarding_ramp",
					"deck",
					"Boarding Ramp",
					"A side ramp used for boarding and cargo work at a quay.",
					VehicleAccessPointType.Ramp,
					false,
					true,
					1,
					AccessItem(
						$"{key}_boarding_ramp",
						"ramp",
						"a cleated merchant-boat ramp",
						"This oak ramp has closely spaced cleats and rope lifting tackles fixed near its outer corners. Iron straps protect the hinge edge where the ramp bears against a quay or the vessel's side.",
						SizeCategory.VeryLarge,
						72000.0,
						"oak")),
				Access(
					"hold_hatch",
					"deck",
					"Cargo Hatch",
					"A broad hatch giving access to the main hold.",
					VehicleAccessPointType.Hatch,
					false,
					true,
					2,
					AccessItem(
						$"{key}_hold_hatch",
						"hatch",
						"a broad canvas-sealed hatch",
						"This broad oak hatch is built from removable sections fitted inside a raised coaming. Waxed canvas covers and rope battens can be drawn across it to protect the hold from ordinary spray and rain.",
						SizeCategory.VeryLarge,
						98000.0,
						"oak"))
			],
			[
				Cargo(
					"hold",
					"deck",
					"hold_hatch",
					"Main Cargo Hold",
					"The broad central hold beneath the working deck.",
					1,
					CargoItem(
						$"{key}_cargo_hold",
						"hold",
						"a broad coastal cargo hold",
						"This broad hold follows the full body of the hull beneath the working deck. Partitions, rope rings, and timber dunnage divide the space for amphorae, barrels, sacks, bales, and boxed trade goods.",
						SizeCategory.Huge,
						420000.0,
						"oak",
						"Container_Colossal"))
			],
			[],
			[RearTow("stern tow bitt", "A heavy stern bitt for towing a tender or warping the vessel.", "rope", 6000.0, 1)],
			[
				Damage(
					"hull_and_rig",
					"Hull, Rig, and Steering",
					"The planked hull, mast step, lateen rig, rudder, and deck beams.",
					560.0,
					8.0,
					350.0,
					560.0,
					true,
					new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.AccessPoint, "boarding_ramp", VehicleSystemStatus.Disabled),
					new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.AccessPoint, "hold_hatch", VehicleSystemStatus.Disabled),
					new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.CargoSpace, "hold", VehicleSystemStatus.Disabled))
			]);
	}

}
