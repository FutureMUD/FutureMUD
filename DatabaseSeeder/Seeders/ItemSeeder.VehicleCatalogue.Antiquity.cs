#nullable enable

using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static VehiclePrototypeSeedSpec AntiquityLightChariot()
	{
		const string key = "antiquity_vehicle_light_chariot";
		return new VehiclePrototypeSeedSpec(
			key,
			"antiquity",
			"Antiquity Light Chariot",
			"A light, two-wheeled chariot intended for a driver and one passenger, with a forward harness hitch and a shallow rear basket.",
			VehicleScale.ItemScale,
			Exterior(
				$"{key}_exterior",
				"chariot",
				"a light two-wheeled chariot",
				"A light two-wheeled chariot stands here.",
				"This light chariot is built around a springy oak frame with a narrow standing platform between two tall spoked wheels. Rawhide lashings reinforce the joints, while bronze-coloured caps protect the axle ends and rail corners. A low front rail and a forward pole leave the body open enough for a driver and one companion to stand close together.",
				SizeCategory.VeryLarge,
				ItemQuality.Good,
				72000.0,
				620.0m,
				"oak",
				AntiquityVehicleEraTag,
				"Functions / Vehicles / Terrestrial Vehicles",
				"Functions / Vehicles / Animal-Drawn Vehicles",
				"Market / Transportation / Passenger Transportation / Cart Passage"),
			[Compartment("platform", "Standing Platform", "The open standing platform between the chariot's wheels.", 1)],
			[],
			[
				DriverSlot("driver", "platform", "driver position", true),
				PassengerSlot("passenger", "platform", "passenger position", 1)
			],
			[PrimaryStation("driver", "rein and pole controls")],
			[GroundMovement(false)],
			[],
			[
				Cargo(
					"basket",
					"platform",
					null,
					"Rear Basket",
					"A shallow basket behind the standing rail for a small load.",
					1,
					CargoItem(
						$"{key}_rear_basket",
						"basket",
						"a shallow chariot basket",
						"This shallow wicker basket is lashed behind the chariot rail, with enough depth for a shield, water jar, or tightly bundled provisions. Its rim is bound in leather where gear would otherwise rub through the weave.",
						SizeCategory.Normal,
						4800.0,
						"wicker",
						"Container_Open_Bin"))
			],
			[],
			[FrontTow("team hitch", "A reinforced forward harness point for the chariot team.", "harness", 1)],
			[Damage("frame", "Frame and Running Gear", "The frame, axle, wheels, rails, and draught pole.", 90.0, 2.0, 55.0, 90.0, true)]);
	}

	private static VehiclePrototypeSeedSpec AntiquityOxCart()
	{
		const string key = "antiquity_vehicle_ox_freight_cart";
		return new VehiclePrototypeSeedSpec(
			key,
			"antiquity",
			"Antiquity Ox Freight Cart",
			"A slow, sturdy two-wheeled freight cart with an open load bed, a driver's perch, and a yoke-compatible front hitch.",
			VehicleScale.RoomContainer,
			Exterior(
				$"{key}_exterior",
				"cart",
				"a heavy plank-sided ox cart",
				"A heavy plank-sided ox cart rests here.",
				"This broad freight cart has two thick, iron-tyred wheels beneath a deep bed of oak planks. Upright stakes and removable sideboards contain loose sacks or jars, while a narrow front board serves as a driver's perch. The long draught pole ends in stout fittings shaped to receive a yoke and traces.",
				SizeCategory.Huge,
				ItemQuality.Standard,
				360000.0,
				850.0m,
				"oak",
				AntiquityVehicleEraTag,
				"Functions / Vehicles / Terrestrial Vehicles",
				"Functions / Vehicles / Animal-Drawn Vehicles",
				"Market / Transportation / Cargo Transportation / Cart Haulage"),
			[Compartment("bed", "Cart Bed", "The open plank load bed and narrow driver's board.", 1)],
			[],
			[
				DriverSlot("driver", "bed", "driver perch", true),
				PassengerSlot("passengers", "bed", "riding space", 2)
			],
			[PrimaryStation("driver", "reins and brake lever")],
			[GroundMovement(false)],
			[],
			[
				Cargo(
					"load_bed",
					"bed",
					null,
					"Freight Bed",
					"The cart's broad open load bed.",
					1,
					CargoItem(
						$"{key}_freight_bed",
						"bed",
						"an open cart freight bed",
						"This broad oak freight bed is enclosed by removable plank sides and upright stakes. The floorboards are scarred by barrels, amphorae, stone, and sacks, with drainage gaps left between the planks.",
						SizeCategory.VeryLarge,
						72000.0,
						"oak",
						"Container_Open_Bin"))
			],
			[],
			[
				FrontTow("yoke hitch", "A forward draught point built for an ox yoke and traces.", "yoke", 1),
				RearTow("rear hitch", "A rear hitch for a light trailing load.", "hitch", 900.0, 2)
			],
			[
				Damage(
					"running_gear",
					"Running Gear",
					"The wheels, axle, draught pole, and load-bearing frame.",
					180.0,
					3.0,
					110.0,
					180.0,
					true,
					new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.CargoSpace, "load_bed", VehicleSystemStatus.Disabled))
			]);
	}

	private static VehiclePrototypeSeedSpec AntiquityRiverCanoe()
	{
		const string key = "antiquity_vehicle_river_canoe";
		return new VehiclePrototypeSeedSpec(
			key,
			"antiquity",
			"Antiquity River Canoe",
			"A narrow open canoe paddled by its driver, with one passenger place and no enclosed cargo hold.",
			VehicleScale.ItemScale,
			Exterior(
				$"{key}_exterior",
				"canoe",
				"a narrow dugout river canoe",
				"A narrow dugout river canoe floats here.",
				"This narrow canoe has been hollowed from a single cedar trunk, its interior adzed smooth and its ends drawn into blunt rising points. Two low thwarts brace the sides without enclosing the hull. Dark pitch fills shallow checks in the wood, and the gunwales are polished by hands, paddles, and repeated beaching.",
				SizeCategory.VeryLarge,
				ItemQuality.Standard,
				68000.0,
				260.0m,
				"cedar",
				AntiquityVehicleEraTag,
				"Functions / Vehicles / Aquatic Vehicles",
				"Functions / Vehicles / Human-Powered Vehicles",
				"Market / Transportation / Passenger Transportation / Ship Passage"),
			[Compartment("hull", "Open Hull", "The narrow open interior of the canoe.", 1)],
			[],
			[
				DriverSlot("driver", "hull", "stern paddler position", true, true, Difficulty.Hard),
				PassengerSlot("passenger", "hull", "forward passenger position", 1, Difficulty.Hard)
			],
			[PrimaryStation("driver", "paddling position")],
			[WaterMovement(true, PaddlePropulsion(9000.0, Difficulty.Normal))],
			[],
			[],
			[],
			[],
			[Damage("hull", "Hull", "The dugout hull, thwarts, and gunwales.", 75.0, 1.5, 45.0, 75.0, true)]);
	}

	private static VehiclePrototypeSeedSpec AntiquityOaredTradingBoat()
	{
		const string key = "antiquity_vehicle_oared_trading_boat";
		return new VehiclePrototypeSeedSpec(
			key,
			"antiquity",
			"Antiquity Oared Trading Boat",
			"A broad open trading boat with a steering position, rowing benches, a cargo hold, and alternate rowed or sail propulsion.",
			VehicleScale.RoomContainer,
			Exterior(
				$"{key}_exterior",
				"boat",
				"a broad oared trading boat",
				"A broad oared trading boat rides the water here.",
				"This broad-bellied trading boat is built from cedar planks drawn up around closely spaced ribs. Low rowing benches line the open waist, while a raised steering platform and a single square sail give the crew two ways to work along a coast or river. The seams are dark with pitch and the deep centre of the hull is decked over above a modest cargo space.",
				SizeCategory.Enormous,
				ItemQuality.Standard,
				4800000.0,
				9600.0m,
				"cedar",
				AntiquityVehicleEraTag,
				"Functions / Vehicles / Aquatic Vehicles",
				"Functions / Vehicles / Human-Powered Vehicles",
				"Functions / Vehicles / Sailing Vehicles",
				"Market / Transportation / Cargo Transportation / Ship Haulage",
				"Market / Transportation / Passenger Transportation / Ship Passage"),
			[Compartment("deck", "Open Deck", "The rowing benches, steering platform, and open working deck.", 1)],
			[],
			[
				DriverSlot("driver", "deck", "steersman's position", true, false, Difficulty.Normal),
				CrewSlot("rowers", "deck", "rowing benches", 4, true, Difficulty.Normal),
				PassengerSlot("passengers", "deck", "passenger deck space", 4, Difficulty.Normal)
			],
			[PrimaryStation("driver", "steering oar and sail controls")],
			[WaterMovement(false, RowedPropulsion(14000.0, Difficulty.Normal, true), SailPropulsion(11000.0, false))],
			[
				Access(
					"cargo_hatch",
					"deck",
					"Cargo Hatch",
					"A low hatch giving access to the central cargo space.",
					VehicleAccessPointType.Hatch,
					false,
					true,
					1,
					AccessItem(
						$"{key}_cargo_hatch",
						"hatch",
						"a low planked cargo hatch",
						"This low cedar hatch is framed with raised coamings and fitted with rope pulls. Pitch-darkened seams and a close overlap help keep ordinary spray from washing directly into the hold.",
						SizeCategory.Large,
						24000.0,
						"cedar"))
			],
			[
				Cargo(
					"hold",
					"deck",
					"cargo_hatch",
					"Cargo Hold",
					"The decked-over central hold for amphorae, sacks, and compact trade goods.",
					1,
					CargoItem(
						$"{key}_cargo_hold",
						"hold",
						"a decked trading-boat hold",
						"This deep central hold is lined by the boat's curved ribs and capped by removable deck boards. Wooden dunnage keeps jars and sacks above bilge water, while tie points along the sides let a crew brace a shifting load.",
						SizeCategory.Huge,
						180000.0,
						"cedar",
						"Container_Colossal"))
			],
			[],
			[RearTow("stern tow point", "A strong stern bitt for towing a light boat or taking a line.", "rope", 2500.0, 1)],
			[
				Damage(
					"hull",
					"Hull and Steering Gear",
					"The planked hull, ribs, steering oar, mast partners, and deck structure.",
					260.0,
					5.0,
					160.0,
					260.0,
					true,
					new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.CargoSpace, "hold", VehicleSystemStatus.Disabled),
					new VehicleDamageEffectSeedSpec(VehicleDamageEffectTargetType.AccessPoint, "cargo_hatch", VehicleSystemStatus.Disabled))
			]);
	}

}
