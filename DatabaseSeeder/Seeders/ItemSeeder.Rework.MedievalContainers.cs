#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalContainers()
	{
		CreateItem(
			"medieval_locking_trade_clerk_coin_lockbox",
			"lockbox",
			"a clerk's coin lockbox",
			null,
			"This clerk's coin lockbox is a small, workmanlike lockbox built from ash boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			2600.0,
			52.0m,
			true,
			false,
			"ash",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small built-in-lock box for coin, tallies, seals, or market takings."
		);

		CreateItem(
			"medieval_locking_trade_cloth_merchant_coffer",
			"coffer",
			"a cloth merchant's coffer",
			null,
			"This cloth merchant's coffer is a medium-sized, well-made coffer built from walnut boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			17000.0,
			230.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking coffer for bolts of fine cloth, sample books, and trade accounts."
		);

		CreateItem(
			"medieval_locking_trade_customs_chest",
			"chest",
			"a customs strong chest",
			null,
			"This customs strong chest is a medium-sized, well-made chest built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			20000.0,
			250.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking chest for customs duties, sealed permits, and cargo records."
		);

		CreateItem(
			"medieval_locking_trade_fair_takings_lockbox",
			"lockbox",
			"a fair-takings lockbox",
			null,
			"This fair-takings lockbox is a small, workmanlike lockbox built from ash boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			2800.0,
			62.0m,
			true,
			false,
			"ash",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Portable lockbox for fair, booth, or stall takings."
		);

		CreateItem(
			"medieval_locking_trade_guild_strong_chest",
			"chest",
			"a guild strong chest",
			null,
			"This guild strong chest is a large, well-made chest built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			42000.0,
			460.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
			null,
			null,
			null,
			null,
			"Heavy institutional chest for guild funds, charters, and records."
		);

		CreateItem(
			"medieval_locking_trade_jeweller_parcel_lockbox",
			"lockbox",
			"a jeweller's parcel lockbox",
			null,
			"This jeweller's parcel lockbox is a small, well-made lockbox worked from brass. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			3600.0,
			150.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Compact lockbox for small high-value parcels and wrapped valuables."
		);

		CreateItem(
			"medieval_locking_trade_market_rent_chest",
			"chest",
			"a market-rent chest",
			null,
			"This market-rent chest is a medium-sized, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			18500.0,
			230.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Chest for market dues, rent rolls, and payments."
		);

		CreateItem(
			"medieval_locking_trade_merchant_strongbox",
			"strongbox",
			"a merchant strongbox",
			null,
			"This merchant strongbox is a medium-sized, well-made strongbox built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			15000.0,
			210.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Stout trade strongbox for valuables, accounts, and compact cargo."
		);

		CreateItem(
			"medieval_locking_trade_money_changer_cash_box",
			"lockbox",
			"a money-changer's cash box",
			null,
			"This money-changer's cash box is a small, well-made lockbox worked from brass. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			4200.0,
			120.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Metal cash box for money-changing counters and trade booths."
		);

		CreateItem(
			"medieval_locking_trade_notary_document_lockbox",
			"lockbox",
			"a notary's document lockbox",
			null,
			"This notary's document lockbox is a small, well-made lockbox built from walnut boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			3400.0,
			110.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small lockbox for deeds, writs, contracts, and sealed papers."
		);

		CreateItem(
			"medieval_locking_trade_spice_chest",
			"chest",
			"a locked spice chest",
			null,
			"This locked spice chest is a medium-sized, well-made chest built from cedar boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			14500.0,
			240.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Built-in-lock chest for spices, dyestuffs, or small high-value goods."
		);

		CreateItem(
			"medieval_locking_trade_spice_sample_lockbox",
			"lockbox",
			"a spice-sample lockbox",
			null,
			"This spice-sample lockbox is a small, well-made lockbox built from cedar boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			2600.0,
			95.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Built-in-lock box for valuable spice samples or small packets."
		);

		CreateItem(
			"medieval_locking_trade_tally_seal_lockbox",
			"lockbox",
			"a tally-seal lockbox",
			null,
			"This tally-seal lockbox is a small, workmanlike lockbox built from oak boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			3000.0,
			58.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Lockbox for marked tallies, wax seals, and small account packets."
		);

		CreateItem(
			"medieval_locking_trade_tax_strong_chest",
			"chest",
			"a tax strong chest",
			null,
			"This tax strong chest is a large, well-made chest worked from wrought iron. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Large,
			ItemQuality.Good,
			62000.0,
			680.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
			null,
			null,
			null,
			null,
			"Heavy metal-reinforced chest for taxes, fees, or administrative funds."
		);

		CreateItem(
			"medieval_locking_trade_toll_chest",
			"chest",
			"a toll-keeper's chest",
			null,
			"This toll-keeper's chest is a medium-sized, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			18000.0,
			225.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Built-in-lock chest for tolls, fees, or gatehouse takings."
		);

		CreateItem(
			"medieval_locking_trade_warehouse_strong_chest",
			"chest",
			"a warehouse strong chest",
			null,
			"This warehouse strong chest is a large, well-made chest built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			36000.0,
			360.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
			null,
			null,
			null,
			null,
			"Heavy built-in-lock chest for warehouse keys, bonds, and high-value stored goods."
		);

		CreateItem(
			"medieval_locking_trade_weights_lockbox",
			"lockbox",
			"a weights lockbox",
			null,
			"This weights lockbox is a small, workmanlike lockbox built from oak boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			3200.0,
			64.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Lockbox for keeping small scales weights, seals, and trade tokens together."
		);

		CreateItem(
			"medieval_trade_apothecary_ingredient_box",
			"box",
			"an apothecary ingredient box",
			null,
			"This apothecary ingredient box is a medium-sized, well-made box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3200.0,
			54.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"Lidded box for labelled packets, dried roots, gum resins, minerals, and other apothecary stock."
		);

		CreateItem(
			"medieval_trade_armorer_rivet_box",
			"box",
			"an armourer's rivet box",
			null,
			"This armourer's rivet box is a small, workmanlike box built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			2100.0,
			20.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"Small dense box for rivets, buckles, rings, plates, and armour-work fittings."
		);

		CreateItem(
			"medieval_trade_ash_fruit_crate",
			"crate",
			"an ash fruit crate",
			null,
			"This ash fruit crate is a medium-sized, workmanlike crate built from ash boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2200.0,
			18.0m,
			true,
			false,
			"ash",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Light rigid crate for orchard fruit, delicate vegetables, or packed table produce."
		);

		CreateItem(
			"medieval_trade_ash_lime_bin",
			"bin",
			"an ash lime bin",
			null,
			"This ash lime bin is a large, workmanlike bin built from ash boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			7800.0,
			26.0m,
			true,
			false,
			"ash",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Dry workshop bin for lime, chalk, ash, sand, or powdered construction material."
		);

		CreateItem(
			"medieval_trade_ash_tool_box",
			"box",
			"an ash tool box",
			null,
			"This ash tool box is a medium-sized, workmanlike box built from ash boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3800.0,
			26.0m,
			true,
			false,
			"ash",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"Sturdy box for trade tools, fittings, measures, and workshop pieces."
		);

		CreateItem(
			"medieval_trade_balance_weight_box",
			"box",
			"a balance-weight box",
			null,
			"This balance-weight box is a small, well-made box worked from bronze. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			2200.0,
			60.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Small sturdy box for balance weights, small measures, and official metal standards."
		);

		CreateItem(
			"medieval_trade_bamboo_cargo_basket",
			"basket",
			"a bamboo cargo basket",
			null,
			"This bamboo cargo basket is a large, workmanlike basket built from split bamboo. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1400.0,
			16.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Large open cargo basket for light bulky goods, market produce, or caravan packing."
		);

		CreateItem(
			"medieval_trade_bamboo_storage_tube",
			"tube",
			"a large bamboo storage tube",
			null,
			"This large bamboo storage tube is a medium-sized, workmanlike tube built from split bamboo. The body is rigid and narrow, with a capped end and a smooth outer surface. The edges are fitted closely to protect what is carried inside. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1100.0,
			14.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"Light rigid tube for scrolls, tea, spice packets, arrows, or narrow dry cargo."
		);

		CreateItem(
			"medieval_trade_bamboo_tea_tube",
			"tube",
			"a bamboo tea tube",
			null,
			"This bamboo tea tube is a small, well-made tube built from split bamboo. The body is rigid and narrow, with a capped end and a smooth outer surface. The edges are fitted closely to protect what is carried inside. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			420.0,
			20.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"Small rigid tube for tea, powders, incense sticks, and delicate sample goods."
		);

		CreateItem(
			"medieval_trade_bean_sack",
			"sack",
			"a dried bean sack",
			null,
			"This dried bean sack is a medium-sized, workmanlike sack made from woven hemp. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			720.0,
			5.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Plain sack for beans, peas, lentils, nuts, and similar loose foodstuffs."
		);

		CreateItem(
			"medieval_trade_beech_nail_box",
			"box",
			"a beech nail box",
			null,
			"This beech nail box is a small, workmanlike box built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1900.0,
			18.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"Small rigid box for nails, tacks, rivets, buckles, or other dense hardware."
		);

		CreateItem(
			"medieval_trade_biscuit_barrel",
			"barrel",
			"a biscuit barrel",
			null,
			"This biscuit barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12500.0,
			34.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Dry barrel for ship biscuit, hard bread, dried food, and travel rations."
		);

		CreateItem(
			"medieval_trade_blacksmith_scrap_bin",
			"bin",
			"a blacksmith's scrap bin",
			null,
			"This blacksmith's scrap bin is a medium-sized, workmanlike bin worked from wrought iron. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			11000.0,
			48.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Heavy open bin for iron scraps, broken fittings, failed nails, and forge returns."
		);

		CreateItem(
			"medieval_trade_bolt_cloth_chest",
			"chest",
			"a bolt-cloth chest",
			null,
			"This bolt-cloth chest is a large, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9800.0,
			46.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Long chest for bolts of cloth, folded garments, tapestries, and shop textiles."
		);

		CreateItem(
			"medieval_trade_bowyer_sinew_box",
			"box",
			"a bowyer's sinew box",
			null,
			"This bowyer's sinew box is a small, workmanlike box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1200.0,
			18.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"Dry box for sinew, horn slivers, bindings, glue cakes, and small bowmaking materials."
		);

		CreateItem(
			"medieval_trade_brass_spice_canister",
			"canister",
			"a brass spice canister",
			null,
			"This brass spice canister is a small, well-made canister worked from brass. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1100.0,
			46.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Metal canister for dry spices, aromatics, incense, and compact luxury goods."
		);

		CreateItem(
			"medieval_trade_bronze_ingot_tray",
			"tray",
			"a bronze ingot tray",
			null,
			"This bronze ingot tray is a small, well-made tray worked from bronze. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			2600.0,
			56.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
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
			"Low tray for ingots, weighed billets, coin blanks, or dense trade samples."
		);

		CreateItem(
			"medieval_trade_bronze_trade_canister",
			"canister",
			"a bronze trade canister",
			null,
			"This bronze trade canister is a small, well-made canister worked from bronze. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			42.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Durable canister for samples, powders, beads, and valuable dry trade goods."
		);

		CreateItem(
			"medieval_trade_butcher_salt_bin",
			"bin",
			"a butcher's salt bin",
			null,
			"This butcher's salt bin is a large, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8200.0,
			30.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Market or workshop bin for coarse salt, curing mix, and dry butcher's stock."
		);

		CreateItem(
			"medieval_trade_candle_stock_crate",
			"crate",
			"a candle-stock crate",
			null,
			"This candle-stock crate is a medium-sized, workmanlike crate built from pine boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2100.0,
			16.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open crate for candles, wax blocks, wick bundles, and lighting stock."
		);

		CreateItem(
			"medieval_trade_canvas_flour_sack",
			"sack",
			"a canvas flour sack",
			null,
			"This canvas flour sack is a medium-sized, workmanlike sack made from coarse canvas. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			820.0,
			6.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Tighter woven sack for flour and sifted meal; still a dry container rather than a liquid vessel."
		);

		CreateItem(
			"medieval_trade_canvas_salt_sack",
			"sack",
			"a doubled canvas salt sack",
			null,
			"This doubled canvas salt sack is a medium-sized, workmanlike sack made from coarse canvas. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			920.0,
			8.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Heavier dry-goods sack intended for salt, alum, ash, and similarly abrasive bulk materials."
		);

		CreateItem(
			"medieval_trade_caravan_cargo_chest",
			"chest",
			"a caravan cargo chest",
			null,
			"This caravan cargo chest is a large, well-made chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			12500.0,
			78.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Travel chest for caravan goods, cloth bales, spices, medicines, and packed valuables."
		);

		CreateItem(
			"medieval_trade_cedar_spice_box",
			"box",
			"a cedar spice box",
			null,
			"This cedar spice box is a small, well-made box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1400.0,
			30.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"Aromatic lidded box for spices, incense, resins, or fragrant merchant stock."
		);

		CreateItem(
			"medieval_trade_cedar_trade_tub",
			"tub",
			"a cedar trade tub",
			null,
			"This cedar trade tub is a large, well-made tub built from cedar boards. Staves form a broad open vessel with a flat bottom and a thick rim. The sides flare slightly, leaving the inside easy to reach. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			7000.0,
			52.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Well-made dry tub for aromatic goods, textiles, small parcels, or shop stock."
		);

		CreateItem(
			"medieval_trade_ceramic_spice_jar",
			"jar",
			"a ceramic spice jar",
			null,
			"This ceramic spice jar is a small, well-made jar formed from ceramic. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			22.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Small lidded jar for spice, dyestuff, medicines, or expensive powders."
		);

		CreateItem(
			"medieval_trade_charcoal_barrel",
			"barrel",
			"a charcoal barrel",
			null,
			"This charcoal barrel is a large, workmanlike barrel built from elm boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			24.0m,
			true,
			false,
			"elm",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Dry barrel for charcoal, lampblack, powdered fuel, or dirty workshop goods."
		);

		CreateItem(
			"medieval_trade_charcoal_sack",
			"sack",
			"a blackened charcoal sack",
			null,
			"This blackened charcoal sack is a medium-sized, workmanlike sack made from woven hemp. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			5.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Rough sack intended for charcoal, coke-like fuel, firewood chips, or dirty workshop stock."
		);

		CreateItem(
			"medieval_trade_clay_lined_dyers_bin",
			"bin",
			"a clay-lined dyer's bin",
			null,
			"This clay-lined dyer's bin is a large, well-made bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			9800.0,
			42.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open bin with a protective lining for dry dyestuffs, mordants, wool lots, or stained workshop materials."
		);

		CreateItem(
			"medieval_trade_coin_counting_box",
			"box",
			"a coin-counting box",
			null,
			"This coin-counting box is a small, well-made box built from walnut boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1700.0,
			40.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"Compartmented box for counted coin parcels, tallies, counters, and small weights."
		);

		CreateItem(
			"medieval_trade_cooper_stave_bin",
			"bin",
			"a cooper's stave bin",
			null,
			"This cooper's stave bin is a large, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			6900.0,
			24.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open bin for barrel staves, hoops, offcuts, wedges, and cooperage stock."
		);

		CreateItem(
			"medieval_trade_cotton_sample_bag",
			"bag",
			"a cotton sample bag",
			null,
			"This cotton sample bag is a small, workmanlike bag made from woven cotton. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			160.0,
			7.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote"
			],
			null,
			null,
			null,
			null,
			"Light sample bag for cloth swatches, herb lots, bead packets, or small market wares."
		);

		CreateItem(
			"medieval_trade_cypress_export_tub",
			"tub",
			"a cypress export tub",
			null,
			"This cypress export tub is a large, workmanlike tub built from cypress boards. Staves form a broad open vessel with a flat bottom and a thick rim. The sides flare slightly, leaving the inside easy to reach. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			7200.0,
			38.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open tub for dry export goods, packed produce, or workshop stock."
		);

		CreateItem(
			"medieval_trade_deep_willow_hamper",
			"hamper",
			"a deep willow hamper",
			null,
			"This deep willow hamper is a large, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1600.0,
			14.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Tall open hamper for bulky goods, fleece, bread loaves, vegetables, or laundry-like trade bundles."
		);

		CreateItem(
			"medieval_trade_dyers_pigment_box",
			"box",
			"a dyer's pigment box",
			null,
			"This dyer's pigment box is a medium-sized, workmanlike box built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3500.0,
			30.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"Dry box for powdered pigments, dye cakes, mordant packets, and stained trade materials."
		);

		CreateItem(
			"medieval_trade_dyestuff_barrel",
			"barrel",
			"a dyestuff barrel",
			null,
			"This dyestuff barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			11000.0,
			44.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Coopered barrel for dry dyestuff cakes, powdered pigment, mordants, or stained stock."
		);

		CreateItem(
			"medieval_trade_dyestuff_sachet",
			"sachet",
			"a dyestuff sachet",
			null,
			"This dyestuff sachet is a very small, workmanlike sachet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			80.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet"
			],
			null,
			null,
			null,
			null,
			"Small packet for powdered dye, mordant, ground pigment, or marked sample material."
		);

		CreateItem(
			"medieval_trade_earthenware_seed_jar",
			"jar",
			"an earthenware seed jar",
			null,
			"This earthenware seed jar is a medium-sized, workmanlike jar formed from earthenware. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3200.0,
			14.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Dry jar for seed, dried herbs, pigments, or measured household trade stock."
		);

		CreateItem(
			"medieval_trade_egg_crate",
			"crate",
			"a straw-lined egg crate",
			null,
			"This straw-lined egg crate is a small, workmanlike crate built from pine boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Small compartmented crate for eggs, fragile cheeses, small pots, or similarly delicate lots."
		);

		CreateItem(
			"medieval_trade_elm_charcoal_bin",
			"bin",
			"an elm charcoal bin",
			null,
			"This elm charcoal bin is a large, workmanlike bin built from elm boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			7600.0,
			24.0m,
			true,
			false,
			"elm",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Blackened open bin for charcoal, fuel lumps, ash, or forge-ready stock."
		);

		CreateItem(
			"medieval_trade_elm_sealed_chest",
			"chest",
			"an elm sealed chest",
			null,
			"This elm sealed chest is a large, workmanlike chest built from elm boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9800.0,
			54.0m,
			true,
			false,
			"elm",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Large chest for sealable but not built-in-lock cargo, merchant stores, or toll goods."
		);

		CreateItem(
			"medieval_trade_fletcher_feather_box",
			"box",
			"a fletcher's feather box",
			null,
			"This fletcher's feather box is a small, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			950.0,
			14.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
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
			"Light lidded box for feathers, bindings, points, and delicate fletching stock."
		);

		CreateItem(
			"medieval_trade_flour_barrel",
			"barrel",
			"a flour barrel",
			null,
			"This flour barrel is a large, workmanlike barrel built from pine boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12000.0,
			32.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Dry coopered barrel for flour, meal, bran, or similar milled goods."
		);

		CreateItem(
			"medieval_trade_folded_cloth_bale",
			"bale",
			"a folded cloth bale",
			null,
			"This folded cloth bale is a large, workmanlike bale made from coarse canvas. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1600.0,
			12.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Rope-tied bale wrapper for folded bolts, offcuts, and merchant cloth lots."
		);

		CreateItem(
			"medieval_trade_glassmaker_cullet_box",
			"box",
			"a glassmaker's cullet box",
			null,
			"This glassmaker's cullet box is a medium-sized, workmanlike box built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3400.0,
			24.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"Rigid box for broken glass, cullet, frit, colourants, and other glasshouse stock."
		);

		CreateItem(
			"medieval_trade_glassware_packing_crate",
			"crate",
			"a glassware packing crate",
			null,
			"This glassware packing crate is a large, well-made crate built from beech boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			4200.0,
			34.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Better-made open crate with dividers or packing space for fragile glass and glazed wares."
		);

		CreateItem(
			"medieval_trade_grain_cask",
			"cask",
			"a grain cask",
			null,
			"This grain cask is a large, workmanlike cask built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			15000.0,
			42.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Drum"
			],
			null,
			null,
			null,
			null,
			"Large coopered cask for bulk grain, malt, pulse, or similar dry stores."
		);

		CreateItem(
			"medieval_trade_hemp_grain_sack",
			"sack",
			"a hemp grain sack",
			null,
			"This hemp grain sack is a medium-sized, workmanlike sack made from woven hemp. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			760.0,
			5.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Sturdy porous sack for grain, beans, malt, or other dry bulk provisions."
		);

		CreateItem(
			"medieval_trade_hemp_scrap_sack",
			"sack",
			"a coarse hemp scrap sack",
			null,
			"This coarse hemp scrap sack is a medium-sized, workmanlike sack made from woven hemp. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			650.0,
			4.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Coarse sack for rope ends, tow, sweepings, kindling, offcuts, and other rough utility stock."
		);

		CreateItem(
			"medieval_trade_herb_packet",
			"packet",
			"a small herb packet",
			null,
			"This small herb packet is a very small, workmanlike packet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			60.0,
			3.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet"
			],
			null,
			null,
			null,
			null,
			"Single-lot packet for dry herbs, powders, seeds, and minor apothecary stock."
		);

		CreateItem(
			"medieval_trade_hide_wrapped_bale",
			"bale",
			"a hide-wrapped cargo bale",
			null,
			"This hide-wrapped cargo bale is a large, workmanlike bale made from worked leather. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2600.0,
			18.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Durable wrapped cargo bale for travel, wagon work, caravan goods, or rough weather handling."
		);

		CreateItem(
			"medieval_trade_jute_meal_sack",
			"sack",
			"a jute meal sack",
			null,
			"This jute meal sack is a medium-sized, workmanlike sack made from coarse jute. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			700.0,
			4.0m,
			true,
			false,
			"jute",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Cheap coarse sack for meal, pulse, bran, fodder, and other dry common goods."
		);

		CreateItem(
			"medieval_trade_large_dry_goods_barrel",
			"barrel",
			"a large dry-goods barrel",
			null,
			"This large dry-goods barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			52.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Drum"
			],
			null,
			null,
			null,
			null,
			"Large dry barrel for bulk goods, sealed stores, and transport lots."
		);

		CreateItem(
			"medieval_trade_large_oak_hogshead",
			"hogshead",
			"a large oak hogshead",
			null,
			"This large oak hogshead is a large, well-made hogshead built from oak boards. Curved staves form a large rounded body, held by heavy bands around the middle and ends. The top is closed with a broad visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			88.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Drum"
			],
			null,
			null,
			null,
			null,
			"Very large coopered dry container for bulk transport, export goods, or estate stores."
		);

		CreateItem(
			"medieval_trade_lead_lined_canister",
			"canister",
			"a lead-lined canister",
			null,
			"This lead-lined canister is a small, workmanlike canister worked from lead. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			2500.0,
			34.0m,
			true,
			false,
			"lead",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Heavy canister for dry materials that must be isolated or sealed from ordinary packing."
		);

		CreateItem(
			"medieval_trade_leather_waste_sack",
			"sack",
			"a leather waste sack",
			null,
			"This leather waste sack is a medium-sized, workmanlike sack made from worked leather. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			980.0,
			10.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Hard-wearing sack for leather scraps, horn offcuts, bone pieces, and workshop waste stock."
		);

		CreateItem(
			"medieval_trade_leatherworker_offcut_bin",
			"bin",
			"a leather offcut bin",
			null,
			"This leather offcut bin is a medium-sized, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4300.0,
			22.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Workshop bin for leather scraps, trimmed pieces, thongs, and repair stock."
		);

		CreateItem(
			"medieval_trade_lime_cask",
			"cask",
			"a lime cask",
			null,
			"This lime cask is a large, workmanlike cask built from ash boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12000.0,
			34.0m,
			true,
			false,
			"ash",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Dry cask for lime, chalk, ash, powdered mineral, or construction material."
		);

		CreateItem(
			"medieval_trade_lime_powder_packet",
			"packet",
			"a lime-powder packet",
			null,
			"This lime-powder packet is a very small, workmanlike packet made from coarse canvas. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			120.0,
			3.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet"
			],
			null,
			null,
			null,
			null,
			"Tough small packet for chalk, lime, ash, powdered mineral, or other dry workshop material."
		);

		CreateItem(
			"medieval_trade_linen_tally_sack",
			"sack",
			"a small linen tally sack",
			null,
			"This small linen tally sack is a small, workmanlike sack made from woven linen. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			3.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Small closable trade sack for counted lots, tallied goods, samples, or coin-wrapped parcels."
		);

		CreateItem(
			"medieval_trade_market_scales_tray",
			"tray",
			"a market scales tray",
			null,
			"This market scales tray is a small, well-made tray worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1400.0,
			48.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
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
			"Metal tray for weights, measures, counterweights, and small weighed goods."
		);

		CreateItem(
			"medieval_trade_medicine_trade_chest",
			"chest",
			"a medicine trade chest",
			null,
			"This medicine trade chest is a large, well-made chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			8700.0,
			88.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Compartmented dry chest for medicines, instruments, dried plants, and labelled packets."
		);

		CreateItem(
			"medieval_trade_merchant_counting_tray",
			"tray",
			"a merchant's counting tray",
			null,
			"This merchant's counting tray is a small, well-made tray built from walnut boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			950.0,
			28.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
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
			"Shallow tray for sorted coins, seals, weights, counters, or priced sample lots."
		);

		CreateItem(
			"medieval_trade_merchant_seal_casket",
			"casket",
			"a merchant seal casket",
			null,
			"This merchant seal casket is a small, well-made casket built from boxwood. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			44.0m,
			true,
			false,
			"boxwood",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
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
			"Small casket for seals, ring signets, wax, stamped samples, and contract tools."
		);

		CreateItem(
			"medieval_trade_metalworker_filings_box",
			"box",
			"a metalworker's filings box",
			null,
			"This metalworker's filings box is a small, workmanlike box worked from wrought iron. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			3300.0,
			42.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Heavy small box for filings, wire ends, offcuts, and dense metal scrap."
		);

		CreateItem(
			"medieval_trade_nail_keg",
			"keg",
			"a nail keg",
			null,
			"This nail keg is a medium-sized, workmanlike keg built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			20.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Small keg for nails, tacks, rivets, small fittings, and dense hardware."
		);

		CreateItem(
			"medieval_trade_nested_packing_boxes",
			"box",
			"a nest of packing boxes",
			null,
			"This nest of packing boxes is a medium-sized, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			32.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"Nested general-purpose wooden boxes for variable parcel sizes and shop packing."
		);

		CreateItem(
			"medieval_trade_nested_spice_boxes",
			"box",
			"a nest of spice boxes",
			null,
			"This nest of spice boxes is a medium-sized, well-made box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			70.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
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
			"Nested merchant boxes for separating small aromatic or high-value dry goods."
		);

		CreateItem(
			"medieval_trade_oak_lidded_trade_box",
			"box",
			"an oak lidded trade box",
			null,
			"This oak lidded trade box is a medium-sized, workmanlike box built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			28.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"General lidded box for shop stock, tools, dry parcels, and packed trade goods."
		);

		CreateItem(
			"medieval_trade_oak_malt_bin",
			"bin",
			"an oak malt bin",
			null,
			"This oak malt bin is a large, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			34.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Deep storehouse bin for malt, grain steeping stock, and brewing or baking supplies."
		);

		CreateItem(
			"medieval_trade_oak_ore_sample_bin",
			"bin",
			"an oak ore-sample bin",
			null,
			"This oak ore-sample bin is a medium-sized, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5200.0,
			28.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Stout open bin for ore samples, sorted stone, metal scrap, or assayed cargo lots."
		);

		CreateItem(
			"medieval_trade_oak_plank_coffer",
			"coffer",
			"an oak plank coffer",
			null,
			"This oak plank coffer is a large, workmanlike coffer built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12500.0,
			60.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Simple but substantial coffer for dry stock, textiles, bundles, and transportable household trade goods."
		);

		CreateItem(
			"medieval_trade_oak_salted_food_tub",
			"tub",
			"an oak salted-food tub",
			null,
			"This oak salted-food tub is a large, workmanlike tub built from oak boards. Staves form a broad open vessel with a flat bottom and a thick rim. The sides flare slightly, leaving the inside easy to reach. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8200.0,
			34.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open dry tub for cured or salted goods, not a brine-filled liquid container."
		);

		CreateItem(
			"medieval_trade_open_felt_wagon_bin",
			"bin",
			"a felt-lined wagon bin",
			null,
			"This felt-lined wagon bin is a large, workmanlike bin built from pine boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			5200.0,
			24.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Felt-lined bin for wagon-carried goods that need softer packing but not a closed trunk."
		);

		CreateItem(
			"medieval_trade_open_oak_grain_bin",
			"bin",
			"an open oak grain bin",
			null,
			"This open oak grain bin is a large, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8600.0,
			32.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open bin for grain, malt, pulse, or other dry bulk goods in a market or storehouse."
		);

		CreateItem(
			"medieval_trade_ore_barrel",
			"barrel",
			"an ore barrel",
			null,
			"This ore barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			17500.0,
			44.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Stout barrel for sorted ore, slag, heavy stone samples, or metal-bearing cargo."
		);

		CreateItem(
			"medieval_trade_packed_glassware_bundle",
			"bundle",
			"a straw-packed glassware bundle",
			null,
			"This straw-packed glassware bundle is a medium-sized, well-made bundle made from woven linen. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1100.0,
			20.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Soft packed bundle for fragile cups, beads, glass blanks, or wrapped ceramic goods."
		);

		CreateItem(
			"medieval_trade_parchment_wrapped_packet",
			"packet",
			"a parchment-wrapped packet",
			null,
			"This parchment-wrapped packet is a very small, workmanlike packet made from layered parchment. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			50.0,
			6.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Writing Materials / Document Containers",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Container_Sachet"
			],
			null,
			null,
			null,
			null,
			"Small wrapped packet for letters of credit, tallies, seals, or protected trade samples."
		);

		CreateItem(
			"medieval_trade_pine_flour_bin",
			"bin",
			"a pine flour bin",
			null,
			"This pine flour bin is a large, workmanlike bin built from pine boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			7200.0,
			26.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Dry goods bin for flour, meal, bran, or other milled foodstuffs."
		);

		CreateItem(
			"medieval_trade_pine_wool_hamper",
			"hamper",
			"a pine wool hamper",
			null,
			"This pine wool hamper is a large, workmanlike hamper built from pine boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			3000.0,
			18.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Rigid hamper for bulk wool, combed fleece, fabric scraps, and shop stock."
		);

		CreateItem(
			"medieval_trade_pitch_lined_dry_cask",
			"cask",
			"a pitch-lined dry cask",
			null,
			"This pitch-lined dry cask is a large, well-made cask built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			15500.0,
			64.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Lined dry cask for damp-sensitive goods, resins, spices, or travel stock."
		);

		CreateItem(
			"medieval_trade_porcelain_tea_canister",
			"canister",
			"a porcelain tea canister",
			null,
			"This porcelain tea canister is a small, well-made canister formed from porcelain. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			850.0,
			50.0m,
			true,
			false,
			"porcelain",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Fine dry canister for tea, spice, incense, medicine, or valuable powdered stock."
		);

		CreateItem(
			"medieval_trade_potters_clay_bin",
			"bin",
			"a potter's clay bin",
			null,
			"This potter's clay bin is a large, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8700.0,
			28.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Deep open bin for clay lumps, grog, powdered slip ingredients, and potter's dry stock."
		);

		CreateItem(
			"medieval_trade_pottery_packing_crate",
			"crate",
			"a pottery packing crate",
			null,
			"This pottery packing crate is a large, workmanlike crate built from pine boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			3600.0,
			20.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Wooden crate intended for straw-packed pots, bowls, tiles, and fragile fired goods."
		);

		CreateItem(
			"medieval_trade_pottery_shard_barrel",
			"barrel",
			"a pottery-shard barrel",
			null,
			"This pottery-shard barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12000.0,
			24.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Dry barrel for shards, kiln wasters, grog, and pottery workshop stock."
		);

		CreateItem(
			"medieval_trade_raw_wool_bale",
			"bale",
			"a raw wool bale",
			null,
			"This raw wool bale is a large, workmanlike bale made from coarse canvas. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1800.0,
			12.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Large compressible bale for raw wool, combed fleece, or bulky textile fibre."
		);

		CreateItem(
			"medieval_trade_river_cargo_chest",
			"chest",
			"a tarred river cargo chest",
			null,
			"This tarred river cargo chest is a large, workmanlike chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			15000.0,
			66.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Stout chest for barge, river, and ferry cargo; dry storage, not a liquid container."
		);

		CreateItem(
			"medieval_trade_rivet_keg",
			"keg",
			"a rivet keg",
			null,
			"This rivet keg is a medium-sized, workmanlike keg built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4000.0,
			20.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Small coopered keg for rivets, rings, staples, and armour or building fittings."
		);

		CreateItem(
			"medieval_trade_rope_coil_crate",
			"crate",
			"a rope-coil crate",
			null,
			"This rope-coil crate is a large, workmanlike crate built from pine boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			3200.0,
			16.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open crate sized for rope coils, tow bundles, netting, and cordage."
		);

		CreateItem(
			"medieval_trade_rope_maker_hemp_crate",
			"crate",
			"a hemp-stock crate",
			null,
			"This hemp-stock crate is a large, workmanlike crate built from pine boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			3000.0,
			15.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open crate for rope coils, hemp tow, twine bundles, and cordage stock."
		);

		CreateItem(
			"medieval_trade_rope_tied_bundle",
			"bundle",
			"a rope-tied trade bundle",
			null,
			"This rope-tied trade bundle is a medium-sized, workmanlike bundle made from coarse canvas. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			8.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"General bundled wrapper for small mixed wares, wrapped parcels, and non-fragile merchant stock."
		);

		CreateItem(
			"medieval_trade_saffron_sachet",
			"sachet",
			"a saffron sachet",
			null,
			"This saffron sachet is a very small, well-made sachet made from woven silk. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			35.0,
			36.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet"
			],
			null,
			null,
			null,
			null,
			"Fine sachet for very high-value spices, rare dye, perfume ingredients, or precious samples."
		);

		CreateItem(
			"medieval_trade_salted_fish_barrel",
			"barrel",
			"a salted-fish barrel",
			null,
			"This salted-fish barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			13500.0,
			36.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Dry/salted-goods barrel for preserved fish and curing stock; use liquid containers for brine-filled variants."
		);

		CreateItem(
			"medieval_trade_salted_meat_cask",
			"cask",
			"a salted-meat cask",
			null,
			"This salted-meat cask is a large, workmanlike cask built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			14500.0,
			40.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Coopered dry cask for salted meat, jerky bundles, and packed preserved provisions."
		);

		CreateItem(
			"medieval_trade_sawdust_barrel",
			"barrel",
			"a sawdust barrel",
			null,
			"This sawdust barrel is a large, workmanlike barrel built from pine boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8500.0,
			18.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Cheap dry barrel for sawdust, shavings, packing material, and workshop sweepings."
		);

		CreateItem(
			"medieval_trade_scribe_document_chest",
			"chest",
			"a scribe's document chest",
			null,
			"This scribe's document chest is a large, well-made chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			7800.0,
			72.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Writing Materials / Document Containers",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Dry chest for documents, contracts, blank parchment, tablets, and record bundles."
		);

		CreateItem(
			"medieval_trade_scroll_shipping_box",
			"box",
			"a scroll shipping box",
			null,
			"This scroll shipping box is a medium-sized, well-made box built from cypress boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2800.0,
			46.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Writing Materials / Document Containers",
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
			"Long lidded box for scrolls, rolls, maps, tallies, or narrow delicate goods."
		);

		CreateItem(
			"medieval_trade_seal_and_weight_box",
			"box",
			"a seal-and-weight box",
			null,
			"This seal-and-weight box is a small, well-made box built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1500.0,
			34.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"Small trade box for seals, counterweights, wax lumps, stamped tallies, and measuring pieces."
		);

		CreateItem(
			"medieval_trade_sealed_clay_trade_jar",
			"jar",
			"a sealed clay trade jar",
			null,
			"This sealed clay trade jar is a medium-sized, workmanlike jar formed from terracotta. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3500.0,
			16.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Sealable fired jar for dry market goods, powders, seed, or preserves packed without liquid mechanics."
		);

		CreateItem(
			"medieval_trade_seed_sack",
			"sack",
			"a barley seed sack",
			null,
			"This barley seed sack is a medium-sized, workmanlike sack made from woven linen. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			620.0,
			7.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Dry seed sack suited to measured agricultural trade lots and household stores."
		);

		CreateItem(
			"medieval_trade_silk_sample_bag",
			"bag",
			"a silk-lined sample bag",
			null,
			"This silk-lined sample bag is a small, well-made bag made from woven silk. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Good,
			120.0,
			30.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote"
			],
			null,
			null,
			null,
			null,
			"Fine sample bag for elite cloth swatches, jewels, spices, or delicate display wares."
		);

		CreateItem(
			"medieval_trade_slatted_pine_produce_crate",
			"crate",
			"a slatted pine produce crate",
			null,
			"This slatted pine produce crate is a medium-sized, workmanlike crate built from pine boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			16.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open slatted crate for fruit, roots, greens, and other goods that benefit from air."
		);

		CreateItem(
			"medieval_trade_small_dry_goods_barrel",
			"barrel",
			"a small dry-goods barrel",
			null,
			"This small dry-goods barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9800.0,
			30.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Small dry barrel for packed grain, nails, preserved food, or trade stock; not a liquid vessel."
		);

		CreateItem(
			"medieval_trade_small_oak_keg",
			"keg",
			"a small oak keg",
			null,
			"This small oak keg is a medium-sized, workmanlike keg built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3200.0,
			18.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Small coopered container for dense dry stock, fittings, resin lumps, or sealed parcels."
		);

		CreateItem(
			"medieval_trade_small_pine_packing_box",
			"box",
			"a small pine packing box",
			null,
			"This small pine packing box is a small, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1600.0,
			14.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
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
			"Compact lidded packing box for parcels, sample lots, and fragile small goods."
		);

		CreateItem(
			"medieval_trade_small_sample_casket",
			"casket",
			"a small sample casket",
			null,
			"This small sample casket is a small, well-made casket built from walnut boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1500.0,
			38.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
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
			"Fine small casket for sample stones, jewels, spice lots, and demonstration wares."
		);

		CreateItem(
			"medieval_trade_spice_display_tray",
			"tray",
			"a cedar spice tray",
			null,
			"This cedar spice tray is a small, well-made tray built from cedar boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			850.0,
			26.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
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
			"Shallow tray for small bowls, packets, or measured spice samples on a stall."
		);

		CreateItem(
			"medieval_trade_stoneware_lidded_jar",
			"jar",
			"a stoneware lidded jar",
			null,
			"This stoneware lidded jar is a medium-sized, workmanlike jar formed from stoneware. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2800.0,
			18.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Rigid lidded jar for dry powders, salt, spices, seed, and small food stores."
		);

		CreateItem(
			"medieval_trade_tanner_bark_barrel",
			"barrel",
			"a tanner's bark barrel",
			null,
			"This tanner's bark barrel is a large, workmanlike barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12500.0,
			26.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Coopered barrel for bark, tan, dry lime, and tanning-house material."
		);

		CreateItem(
			"medieval_trade_tanner_bark_bin",
			"bin",
			"a tanner's bark bin",
			null,
			"This tanner's bark bin is a large, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8400.0,
			24.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open bin for bark, tan, lime, and other dry tanner's materials."
		);

		CreateItem(
			"medieval_trade_teak_cotton_hamper",
			"hamper",
			"a teak cotton hamper",
			null,
			"This teak cotton hamper is a large, well-made hamper built from teak boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			4200.0,
			48.0m,
			true,
			false,
			"teak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Sturdier open hamper for cotton bales, dyed cloth, and workshop textiles."
		);

		CreateItem(
			"medieval_trade_terracotta_grain_jar",
			"jar",
			"a terracotta grain jar",
			null,
			"This terracotta grain jar is a large, workmanlike jar formed from terracotta. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			22.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Fired storage jar for grain, pulses, dried fruit, and dry household or market goods."
		);

		CreateItem(
			"medieval_trade_ventilated_fish_crate",
			"crate",
			"a ventilated fish crate",
			null,
			"This ventilated fish crate is a medium-sized, workmanlike crate built from pine boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2600.0,
			14.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open crate for fish, shellfish, salted catch bundles, or damp market goods."
		);

		CreateItem(
			"medieval_trade_wagon_cargo_chest",
			"chest",
			"a wagon cargo chest",
			null,
			"This wagon cargo chest is a large, workmanlike chest built from ash boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			13200.0,
			58.0m,
			true,
			false,
			"ash",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Road-travel chest for wagon freight, wrapped tools, merchant stores, and estate goods."
		);

		CreateItem(
			"medieval_trade_walnut_merchant_box",
			"box",
			"a walnut merchant box",
			null,
			"This walnut merchant box is a medium-sized, well-made box built from walnut boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			48.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"Better-finished trade box for valuable packets, ledgers, seals, and merchant samples."
		);

		CreateItem(
			"medieval_trade_wax_block_crate",
			"crate",
			"a wax-block crate",
			null,
			"This wax-block crate is a medium-sized, workmanlike crate built from beech boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			18.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open crate for beeswax blocks, tallow stock, seal wax, and candle-making materials."
		);

		CreateItem(
			"medieval_trade_wax_tablet_shipping_box",
			"box",
			"a wax-tablet shipping box",
			null,
			"This wax-tablet shipping box is a medium-sized, workmanlike box built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			28.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Writing Materials / Document Containers",
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
			"Rigid box for writing tablets, samples, small books, or other flat trade goods."
		);

		CreateItem(
			"medieval_trade_waxed_canvas_spice_sack",
			"sack",
			"a waxed canvas spice sack",
			null,
			"This waxed canvas spice sack is a small, well-made sack made from coarse canvas. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Good,
			360.0,
			18.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Wax-treated soft container for valuable dry spices, resins, aromatics, or dyestuffs."
		);

		CreateItem(
			"medieval_trade_waxed_trade_barrel",
			"barrel",
			"a waxed trade barrel",
			null,
			"This waxed trade barrel is a large, well-made barrel built from oak boards. Curved staves form a rounded body, held in place by tight bands around the middle and ends. The top is closed with a small visible bung. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			15000.0,
			60.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Drum"
			],
			null,
			null,
			null,
			null,
			"Better-sealed barrel for dry goods that must be protected from damp; still not a liquid container."
		);

		CreateItem(
			"medieval_trade_willow_market_basket",
			"basket",
			"a willow market basket",
			null,
			"This willow market basket is a medium-sized, workmanlike basket built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			8.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open basket for loose produce, small parcels, and market-table goods."
		);

		CreateItem(
			"medieval_trade_wooden_powder_tub",
			"tub",
			"a wooden powder tub",
			null,
			"This wooden powder tub is a medium-sized, workmanlike tub built from beech boards. Staves form a broad open vessel with a flat bottom and a thick rim. The sides flare slightly, leaving the inside easy to reach. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3600.0,
			22.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open tub for dry powders, grain, meal, plaster, pigments, or sorted workshop material."
		);

		CreateItem(
			"medieval_trade_wool_fleece_sack",
			"sack",
			"a wool fleece sack",
			null,
			"This wool fleece sack is a medium-sized, workmanlike sack made from woven wool. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			640.0,
			6.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Soft sack for loose fleeces, wool locks, cloth scraps, and other compressible textile goods."
		);

		CreateItem(
			"medieval_trade_wool_hamper",
			"hamper",
			"a wool sorting hamper",
			null,
			"This wool sorting hamper is a large, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1800.0,
			14.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Large open hamper for sorting fleeces, cloth scraps, raw wool, and bulky textile goods."
		);

		CreateItem(
			"medieval_trade_wool_sorting_sack",
			"sack",
			"a wool sorting sack",
			null,
			"This wool sorting sack is a medium-sized, workmanlike sack made from woven wool. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			560.0,
			6.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Soft sack for separating clean wool, waste fleece, combings, and sorted textile fibre."
		);

		CreateItem(
			"medieval_trade_wrought_iron_nail_bin",
			"bin",
			"a wrought-iron nail bin",
			null,
			"This wrought-iron nail bin is a small, workmanlike bin worked from wrought iron. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			4200.0,
			32.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Small metal bin for nails, tacks, rivets, clamps, and other dense hardware."
		);

		CreateItem(
			"medieval_locking_trade_dockside_bond_chest",
			"chest",
			"a dockside bond chest",
			null,
			"This dockside bond chest is a medium-sized, well-made chest built from cypress boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			21000.0,
			270.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking chest for port bonds, tally sticks, and cargo records."
		);

		CreateItem(
			"medieval_locking_trade_bazaar_cash_box",
			"lockbox",
			"a bazaar cash lockbox",
			null,
			"This bazaar cash lockbox is a small, well-made lockbox worked from brass. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			4200.0,
			135.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Metal lockbox for shop counters and market stalls."
		);

		CreateItem(
			"medieval_locking_trade_teak_spice_chest",
			"chest",
			"a teak spice chest",
			null,
			"This teak spice chest is a medium-sized, well-made chest built from teak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			18000.0,
			280.0m,
			true,
			false,
			"teak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking teak chest for valuable spices, dyes, or aromatics."
		);

		CreateItem(
			"medieval_locking_trade_cedar_merchant_casket",
			"casket",
			"a cedar merchant casket",
			null,
			"This cedar merchant casket is a small, well-made casket built from cedar boards. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			3000.0,
			115.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small locking casket for seals, paper money substitutes, tallies, or compact valuables."
		);

		CreateItem(
			"medieval_locking_trade_caravan_pay_chest",
			"chest",
			"a caravan pay chest",
			null,
			"This caravan pay chest is a medium-sized, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			22000.0,
			270.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking pay chest for caravans, guards, and merchants on the road."
		);

		CreateItem(
			"medieval_regional_anglo_danish_bronze_pinned_trade_coffer",
			"coffer",
			"a bronze-pinned trade coffer",
			null,
			"This bronze-pinned trade coffer is a large, well-made coffer built from birch boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			11800.0,
			76.0m,
			true,
			false,
			"birch",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Regional addition: North-Sea style coffer with metal pins for trade goods and household stores."
		);

		CreateItem(
			"medieval_regional_anglo_norman_panelled_ledger_chest",
			"chest",
			"a panelled ledger chest",
			null,
			"This panelled ledger chest is a large, well-made chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			12200.0,
			86.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Writing Materials / Document Containers",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Regional addition: panelled chest for ledgers, writs, stall records, and cloth or coin packets."
		);

		CreateItem(
			"medieval_regional_carolingian_banded_beech_goods_chest",
			"chest",
			"a banded beech goods chest",
			null,
			"This banded beech goods chest is a large, workmanlike chest built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			11600.0,
			62.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Regional addition: banded storage chest for hall goods, dues, provisions, and court supplies."
		);

		CreateItem(
			"medieval_regional_christian_iberian_olivewood_saffron_box",
			"box",
			"an olive-wood saffron box",
			null,
			"This olive-wood saffron box is a small, well-made box built from olive wood. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1200.0,
			58.0m,
			true,
			false,
			"olive wood",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"Regional addition: fine small box for saffron, coin, spices, and luxury packets."
		);

		CreateItem(
			"medieval_regional_early_anglo_saxon_ash_tally_chest",
			"chest",
			"an ash tally chest",
			null,
			"This ash tally chest is a large, workmanlike chest built from ash boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			10200.0,
			54.0m,
			true,
			false,
			"ash",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Regional addition: plain dry chest suitable for estate tallies, dues, and local market goods."
		);

		CreateItem(
			"medieval_regional_fatimid_linen_caravan_bale",
			"bale",
			"a linen caravan bale",
			null,
			"This linen caravan bale is a large, workmanlike bale made from woven linen. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1500.0,
			14.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Regional addition: light bale wrapper for cloth, packets, and caravan-packed market goods."
		);

		CreateItem(
			"medieval_regional_goryeo_pine_merchant_chest",
			"chest",
			"a pine merchant chest",
			null,
			"This pine merchant chest is a large, well-made chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			9800.0,
			66.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Regional addition: low, portable chest for cloth, tea, papers, and market goods."
		);

		CreateItem(
			"medieval_regional_heian_cypress_document_box",
			"box",
			"a cypress document box",
			null,
			"This cypress document box is a medium-sized, well-made box built from cypress boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2200.0,
			54.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Writing Materials / Document Containers",
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
			"Regional addition: long dry box for documents, scrolls, textiles, and small elite stores."
		);

		CreateItem(
			"medieval_regional_high_english_wool_staple_chest",
			"chest",
			"a wool-staple trade chest",
			null,
			"This wool-staple trade chest is a large, well-made chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			14800.0,
			92.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Regional addition: trade chest for graded wool, cloth bolts, tallies, and merchant markings."
		);

		CreateItem(
			"medieval_regional_german_stoneware_merchant_jar",
			"jar",
			"a stoneware merchant jar",
			null,
			"This stoneware merchant jar is a medium-sized, well-made jar formed from stoneware. A cylindrical body rises from a flat base to a fitted lid. The mouth is cleanly shaped, with worn edges where the lid has been lifted. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			28.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Regional addition: tough lidded jar for dry stock, salt, dyestuffs, and market goods."
		);

		CreateItem(
			"medieval_regional_irish_hide_bound_provision_chest",
			"chest",
			"a hide-bound provision chest",
			null,
			"This hide-bound provision chest is a large, workmanlike chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12500.0,
			58.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Regional addition: rugged chest with hide binding for provisions, wool, and travel stores."
		);

		CreateItem(
			"medieval_regional_magyar_bronze_mounted_saddle_crate",
			"crate",
			"a bronze-mounted saddle crate",
			null,
			"This bronze-mounted saddle crate is a medium-sized, well-made crate built from ash boards. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			52.0m,
			true,
			false,
			"ash",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Regional addition: rigid mounted-travel crate for camp goods and mobile trade lots."
		);

		CreateItem(
			"medieval_regional_norse_shipboard_sea_chest",
			"chest",
			"a shipboard sea chest",
			null,
			"This shipboard sea chest is a large, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12800.0,
			64.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Regional addition: travel chest suited to boat, shore, and cargo handling."
		);

		CreateItem(
			"medieval_regional_rus_birch_merchant_box",
			"box",
			"a birch merchant box",
			null,
			"This birch merchant box is a medium-sized, workmanlike box built from birch boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2600.0,
			28.0m,
			true,
			false,
			"birch",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
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
			"Regional addition: light lidded box for furs, wax, tablets, and river-trade parcels."
		);

		CreateItem(
			"medieval_regional_scottish_wool_staple_chest",
			"chest",
			"an iron-bound wool chest",
			null,
			"This iron-bound wool chest is a large, well-made chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			15000.0,
			88.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Regional addition: heavier chest for wool staple goods, cloth, and upland trade stores."
		);

		CreateItem(
			"medieval_regional_seljuk_walnut_caravan_chest",
			"chest",
			"a walnut caravan chest",
			null,
			"This walnut caravan chest is a large, well-made chest built from walnut boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			11800.0,
			90.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Regional addition: travel chest for caravan goods, arms fittings, cloth, and compact valuables."
		);

		CreateItem(
			"medieval_regional_song_bamboo_tea_crate",
			"crate",
			"a bamboo tea crate",
			null,
			"This bamboo tea crate is a medium-sized, well-made crate built from split bamboo. Slatted sides rise from a flat base, with square corner posts keeping the frame rigid. The open top leaves the contents visible from above. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			36.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Regional addition: light crate for tea cakes, packets, porcelain stock, or shop goods."
		);

		CreateItem(
			"medieval_regional_steppe_felt_wrapped_cargo_bundle",
			"bundle",
			"a felt-wrapped cargo bundle",
			null,
			"This felt-wrapped cargo bundle is a large, workmanlike bundle made from pressed felt. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1300.0,
			12.0m,
			true,
			false,
			"felt",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack"
			],
			null,
			null,
			null,
			null,
			"Regional addition: soft wrapped cargo bundle for mobile camps, pack animals, and caravan goods."
		);

		CreateItem(
			"medieval_domestic_alms_pouch",
			"pouch",
			"an alms pouch",
			null,
			"This alms pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			85.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small belt pouch used for alms coins, tokens, or modest household charity money."
		);

		CreateItem(
			"medieval_domestic_alms_purse",
			"purse",
			"an alms purse",
			null,
			"This alms purse is a very small, workmanlike purse made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			80.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Personal purse for alms money, tokens, or small household distributions."
		);

		CreateItem(
			"medieval_domestic_ash_document_case",
			"case",
			"an ash document case",
			null,
			"This ash document case is a small, workmanlike case built from ash boards. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			18.0m,
			true,
			false,
			"ash",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Rigid personal case for folded records, small charters, tally slips, and protected documents."
		);

		CreateItem(
			"medieval_domestic_bamboo_back_basket",
			"basket",
			"a bamboo back basket",
			null,
			"This bamboo back basket is a medium-sized, workmanlike basket built from split bamboo. The body is woven around a firm rim, with a flat base and open mouth. The weave tightens at the lower corners where the load bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			780.0,
			16.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Light rigid back basket for warm-climate household carrying and produce loads."
		);

		CreateItem(
			"medieval_domestic_bamboo_quiver",
			"quiver",
			"a bamboo quiver",
			null,
			"This bamboo quiver is a small, workmanlike quiver built from split bamboo. The body is long and narrow, with a stiffened mouth and a closed base. A shoulder strap is fixed along one side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			14.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Quiver",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Light quiver for arrows, slim canes, rolled papers, or craft rods."
		);

		CreateItem(
			"medieval_domestic_bamboo_scroll_tube",
			"tube",
			"a bamboo scroll tube",
			null,
			"This bamboo scroll tube is a small, workmanlike tube built from split bamboo. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			12.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Light tubular case for rolled documents, patterns, maps, or scrolls."
		);

		CreateItem(
			"medieval_domestic_bamboo_shoulder_basket",
			"basket",
			"a bamboo shoulder basket",
			null,
			"This bamboo shoulder basket is a small, workmanlike basket built from split bamboo. The body is woven around a firm rim, with a flat base and open mouth. The weave tightens at the lower corners where the load bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			9.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Light rigid basket worn from the shoulder for warm-climate domestic errands."
		);

		CreateItem(
			"medieval_domestic_basket_backpack",
			"pack",
			"a basket backpack",
			null,
			"This basket backpack is a medium-sized, workmanlike pack built from willow boards. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1050.0,
			18.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Woven back basket for vegetables, laundry, kindling, or household carrying."
		);

		CreateItem(
			"medieval_domestic_bead_box",
			"box",
			"a bead box",
			null,
			"This bead box is a very small, workmanlike box built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			220.0,
			10.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Small lidded box for beads, toggles, buttons, and delicate household ornaments."
		);

		CreateItem(
			"medieval_domestic_bead_sachet",
			"sachet",
			"a bead sachet",
			null,
			"This bead sachet is a tiny, workmanlike sachet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			20.0,
			3.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small sachet for loose beads, buttons, charms, or game counters."
		);

		CreateItem(
			"medieval_domestic_beaded_purse",
			"purse",
			"a beaded purse",
			null,
			"This beaded purse is a very small, well-made purse made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			95.0,
			18.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Decorated purse for feast-day coins, beads, and small valuables."
		);

		CreateItem(
			"medieval_domestic_beadwork_pouch",
			"pouch",
			"a beadwork pouch",
			null,
			"This beadwork pouch is a very small, well-made pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			100.0,
			16.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Fine pouch with decorative beadwork for jewellery, beads, or feast-day personal items."
		);

		CreateItem(
			"medieval_domestic_beeswax_pouch",
			"pouch",
			"a beeswax pouch",
			null,
			"This beeswax pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			100.0,
			6.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small pouch for wax lumps, sealing scraps, candle ends, or protected sticky goods."
		);

		CreateItem(
			"medieval_domestic_belt_quiver",
			"quiver",
			"a leather belt quiver",
			null,
			"This leather belt quiver is a small, workmanlike quiver made from worked leather. The body is long and narrow, with a stiffened mouth and a closed base. A shoulder strap is fixed along one side. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			16.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Quiver",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Belt-attached quiver for arrows, bolts, bodkins, or household rod-like tools."
		);

		CreateItem(
			"medieval_domestic_belted_ring_purse",
			"purse",
			"a belted ring purse",
			null,
			"This belted ring purse is a very small, workmanlike purse made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			120.0,
			8.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small purse for rings, pins, tokens, and modest personal valuables."
		);

		CreateItem(
			"medieval_domestic_birch_scroll_tube",
			"tube",
			"a birch scroll tube",
			null,
			"This birch scroll tube is a small, workmanlike tube built from birch boards. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			14.0m,
			true,
			false,
			"birch",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Tubular case for rolled paper, parchment, household patterns, or written instructions."
		);

		CreateItem(
			"medieval_domestic_blanket_roll_pack",
			"pack",
			"a blanket-roll pack",
			null,
			"This blanket-roll pack is a medium-sized, workmanlike pack made from woven wool. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			980.0,
			15.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Pack shaped around a rolled blanket or cloak with space for small personal goods."
		);

		CreateItem(
			"medieval_domestic_bone_lace_case",
			"case",
			"a bone lace case",
			null,
			"This bone lace case is a tiny, well-made case worked from bone. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			60.0,
			14.0m,
			true,
			false,
			"bone",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small rigid case for lace needles, fine hooks, and delicate trim."
		);

		CreateItem(
			"medieval_domestic_bone_needle_case",
			"case",
			"a bone needle case",
			null,
			"This bone needle case is a tiny, well-made case worked from bone. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			55.0,
			14.0m,
			true,
			false,
			"bone",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Hard little case for needles, fine pins, small hooks, or delicate sewing pieces."
		);

		CreateItem(
			"medieval_domestic_book_satchel",
			"satchel",
			"a book satchel",
			null,
			"This book satchel is a small, well-made satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Good,
			600.0,
			34.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Stiffened satchel for books, folded papers, or protected household records."
		);

		CreateItem(
			"medieval_domestic_boxwood_pen_case",
			"case",
			"a boxwood pen case",
			null,
			"This boxwood pen case is a very small, well-made case built from boxwood. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			120.0,
			18.0m,
			true,
			false,
			"boxwood",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Narrow case for reed pens, small brushes, stylus pieces, or writing tools."
		);

		CreateItem(
			"medieval_domestic_broad_waist_purse",
			"purse",
			"a broad waist purse",
			null,
			"This broad waist purse is a small, well-made purse made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. The opening and interior are plain to see, making the storage space easy to inspect. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Good,
			240.0,
			20.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Wear_Waist"
			],
			null,
			null,
			null,
			null,
			"Broader purse worn at the waist for coins, keys, compact writing pieces, or small valuables."
		);

		CreateItem(
			"medieval_domestic_button_and_toggle_pouch",
			"pouch",
			"a toggle-and-button pouch",
			null,
			"This toggle-and-button pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small pouch for toggles, buttons, clasp pieces, pins, and clothing repair odds."
		);

		CreateItem(
			"medieval_domestic_candle_lighter_pouch",
			"pouch",
			"a candle-lighter pouch",
			null,
			"This candle-lighter pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			110.0,
			7.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Pouch for tinder, wick ends, candle stubs, and small lighting tools."
		);

		CreateItem(
			"medieval_domestic_canvas_errand_satchel",
			"satchel",
			"a canvas errand satchel",
			null,
			"This canvas errand satchel is a small, workmanlike satchel made from coarse canvas. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			430.0,
			12.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Hard-wearing satchel for errands, market goods, mending, and daily carrying."
		);

		CreateItem(
			"medieval_domestic_canvas_flap_pouch",
			"pouch",
			"a canvas flap pouch",
			null,
			"This canvas flap pouch is a very small, workmanlike pouch made from coarse canvas. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			120.0,
			5.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Stouter cloth pouch with a flap for tools, mending gear, or spare fastenings."
		);

		CreateItem(
			"medieval_domestic_canvas_tool_roll",
			"roll",
			"a canvas tool roll",
			null,
			"This canvas tool roll is a small, workmanlike roll made from coarse canvas. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			250.0,
			8.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Roll-up carrier for small tools, awls, knives, pins, and repair pieces."
		);

		CreateItem(
			"medieval_domestic_canvas_travel_pack",
			"pack",
			"a canvas travel pack",
			null,
			"This canvas travel pack is a medium-sized, workmanlike pack made from coarse canvas. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			820.0,
			14.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Stout pack for domestic travel, errands, and household removals."
		);

		CreateItem(
			"medieval_domestic_cedar_scent_case",
			"case",
			"a cedar scent case",
			null,
			"This cedar scent case is a very small, well-made case built from cedar boards. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			140.0,
			20.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small cedar case for scented packets, combs, pins, or dressing-table goods."
		);

		CreateItem(
			"medieval_domestic_charcoal_dust_packet",
			"packet",
			"a charcoal-dust packet",
			null,
			"This charcoal-dust packet is a tiny, workmanlike packet made from layered paper. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			20.0,
			2.0m,
			true,
			false,
			"paper",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Tiny packet for charcoal dust, lamp-black, sketching powder, or household marking material."
		);

		CreateItem(
			"medieval_domestic_charm_pouch",
			"pouch",
			"a charm pouch",
			null,
			"This charm pouch is a very small, workmanlike pouch made from woven wool. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			65.0,
			5.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small drawstring pouch for tokens, charms, keepsakes, or domestic devotional pieces."
		);

		CreateItem(
			"medieval_domestic_child_carry_bag",
			"bag",
			"a small carry bag",
			null,
			"This small carry bag is a small, workmanlike bag made from woven linen. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Small shoulder bag for a child or servant carrying food, cloth, or little household goods."
		);

		CreateItem(
			"medieval_domestic_child_toy_pouch",
			"pouch",
			"a child's toy pouch",
			null,
			"This child's toy pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			3.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small pouch for toys, counters, shells, beads, and harmless keepsakes."
		);

		CreateItem(
			"medieval_domestic_clasped_purse",
			"purse",
			"a clasped purse",
			null,
			"This clasped purse is a very small, well-made purse made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			125.0,
			16.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Better-made purse with a visible clasp for household valuables and personal keepsakes."
		);

		CreateItem(
			"medieval_domestic_clasped_tablet_wallet",
			"wallet",
			"a clasped tablet wallet",
			null,
			"This clasped tablet wallet is a small, well-made wallet made from worked leather. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Good,
			280.0,
			20.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Stiff wallet for wax tablets, small notes, stylus pieces, and account slips."
		);

		CreateItem(
			"medieval_domestic_comb_case",
			"case",
			"a leather comb case",
			null,
			"This leather comb case is a very small, workmanlike case made from worked leather. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			95.0,
			7.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Slim case for a comb, hair sticks, pins, or small grooming implements."
		);

		CreateItem(
			"medieval_domestic_comb_pouch",
			"pouch",
			"a comb pouch",
			null,
			"This comb pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			120.0,
			6.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Narrow pouch suited to a comb, hair pins, small grooming tools, or wrapped personal pieces."
		);

		CreateItem(
			"medieval_domestic_cosmetic_box",
			"box",
			"a cosmetic box",
			null,
			"This cosmetic box is a small, well-made box built from sandalwood. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			360.0,
			36.0m,
			true,
			false,
			"sandalwood",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Small fragrant-wood box for powders, combs, scent sachets, and personal dressing goods."
		);

		CreateItem(
			"medieval_domestic_cosmetic_sachet",
			"sachet",
			"a cosmetic sachet",
			null,
			"This cosmetic sachet is a tiny, well-made sachet made from woven silk. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			18.0,
			12.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small sachet for powdered cosmetic, scent, or dressing-table matter."
		);

		CreateItem(
			"medieval_domestic_double_strap_waterskin",
			"waterskin",
			"a double-strap waterskin",
			null,
			"This double-strap waterskin is a medium-sized, well-made waterskin made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Normal,
			ItemQuality.Good,
			760.0,
			30.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Better-strapped waterskin for longer journeys or shared household use."
		);

		CreateItem(
			"medieval_domestic_drawstring_purse",
			"purse",
			"a drawstring purse",
			null,
			"This drawstring purse is a very small, workmanlike purse made from woven wool. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			4.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Soft drawstring purse for small change, rings, counters, or tokens."
		);

		CreateItem(
			"medieval_domestic_dried_flower_sachet",
			"sachet",
			"a dried-flower sachet",
			null,
			"This dried-flower sachet is a tiny, workmanlike sachet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			22.0,
			3.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small scent sachet for folded clothing, bedding, or fine storage chests."
		);

		CreateItem(
			"medieval_domestic_dried_herb_sachet",
			"sachet",
			"a dried-herb sachet",
			null,
			"This dried-herb sachet is a tiny, workmanlike sachet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			25.0,
			2.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Tiny sachet for dried household herbs and scenting mixtures."
		);

		CreateItem(
			"medieval_domestic_dyestuff_sachet",
			"sachet",
			"a dyestuff sachet",
			null,
			"This dyestuff sachet is a tiny, workmanlike sachet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			30.0,
			3.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small sachet for a pinch of dye, pigment, alum, or textile-working powder."
		);

		CreateItem(
			"medieval_domestic_embroidered_alms_purse",
			"purse",
			"an embroidered alms purse",
			null,
			"This embroidered alms purse is a very small, well-made purse made from woven silk. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			85.0,
			22.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Fine purse for alms money, prayer tokens, and feast-day coins."
		);

		CreateItem(
			"medieval_domestic_felt_saddle_pack",
			"pack",
			"a felt saddle pack",
			null,
			"This felt saddle pack is a medium-sized, workmanlike pack made from pressed felt. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			850.0,
			18.0m,
			true,
			false,
			"felt",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Soft felt pack for travel goods; suitable as a carried domestic pack even when not on a mount."
		);

		CreateItem(
			"medieval_domestic_fine_silk_pouch",
			"pouch",
			"a fine silk pouch",
			null,
			"This fine silk pouch is a very small, well-made pouch made from woven silk. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			50.0,
			24.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Light elite pouch for jewellery, scent packets, small letters, or formal household tokens."
		);

		CreateItem(
			"medieval_domestic_fisher_net_bag",
			"bag",
			"a net-carrying bag",
			null,
			"This net-carrying bag is a medium-sized, workmanlike bag made from woven hemp. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			700.0,
			10.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Rough pack for nets, cord, hooks, floats, and domestic fishing gear."
		);

		CreateItem(
			"medieval_domestic_flat_token_purse",
			"purse",
			"a flat token purse",
			null,
			"This flat token purse is a very small, workmanlike purse made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Flat cloth purse for market tokens, alms coins, or small counters."
		);

		CreateItem(
			"medieval_domestic_fleece_lined_pouch",
			"pouch",
			"a fleece-lined pouch",
			null,
			"This fleece-lined pouch is a very small, well-made pouch made from dressed fur. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			120.0,
			14.0m,
			true,
			false,
			"fur",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Soft-lined pouch for delicate ornaments, beads, or small precious objects."
		);

		CreateItem(
			"medieval_domestic_folded_linen_carrier",
			"carrier",
			"a folded linen carrier",
			null,
			"This folded linen carrier is a small, workmanlike carrier made from woven linen. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			6.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Light carrier for folded linen, napkins, mending, or wrapped parcels."
		);

		CreateItem(
			"medieval_domestic_folding_sewing_wallet",
			"wallet",
			"a folding sewing wallet",
			null,
			"This folding sewing wallet is a small, workmanlike wallet made from woven linen. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			150.0,
			8.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Folding wallet for needles, small scissors, thread, patches, and pins."
		);

		CreateItem(
			"medieval_domestic_forager_pack",
			"pack",
			"a forager pack",
			null,
			"This forager pack is a medium-sized, workmanlike pack made from woven hemp. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			760.0,
			11.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Rough back pack for gathered produce, herbs, mushrooms, or kindling."
		);

		CreateItem(
			"medieval_domestic_foraging_shoulder_bag",
			"bag",
			"a foraging shoulder bag",
			null,
			"This foraging shoulder bag is a small, workmanlike bag made from woven hemp. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			340.0,
			7.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Rough shoulder bag for gathered herbs, kindling, mushrooms, roots, or household foraging."
		);

		CreateItem(
			"medieval_domestic_frame_pack",
			"pack",
			"a wooden frame pack",
			null,
			"This wooden frame pack is a medium-sized, workmanlike pack built from ash boards. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1900.0,
			28.0m,
			true,
			false,
			"ash",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Rigid-framed pack for larger domestic loads, bundles, and awkward household goods."
		);

		CreateItem(
			"medieval_domestic_fur_lined_pouch",
			"pouch",
			"a fur-lined pouch",
			null,
			"This fur-lined pouch is a very small, well-made pouch made from dressed fur. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			130.0,
			20.0m,
			true,
			false,
			"fur",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Soft-lined pouch for fragile beads, polished stones, small metalwork, or delicate personal objects."
		);

		CreateItem(
			"medieval_domestic_game_piece_pouch",
			"pouch",
			"a game-piece pouch",
			null,
			"This game-piece pouch is a very small, workmanlike pouch made from woven wool. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			75.0,
			4.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Soft pouch for dice, counters, game pieces, knucklebones, or similar small objects."
		);

		CreateItem(
			"medieval_domestic_garment_pin_pouch",
			"pouch",
			"a garment-pin pouch",
			null,
			"This garment-pin pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			95.0,
			6.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small pouch for garment pins, hooks, toggles, and spare fastenings."
		);

		CreateItem(
			"medieval_domestic_goatskin_style_waterskin",
			"waterskin",
			"a goatskin-style waterskin",
			null,
			"This goatskin-style waterskin is a small, workmanlike waterskin made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			450.0,
			16.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Traditional leather liquid vessel with shoulder carry behaviour."
		);

		CreateItem(
			"medieval_domestic_hemp_household_pack",
			"pack",
			"a hemp household pack",
			null,
			"This hemp household pack is a medium-sized, workmanlike pack made from woven hemp. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			780.0,
			12.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Plain pack for carrying bedding, linen, food, and domestic loads."
		);

		CreateItem(
			"medieval_domestic_hemp_marketing_sack",
			"sack",
			"a hemp marketing sack",
			null,
			"This hemp marketing sack is a medium-sized, workmanlike sack made from woven hemp. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			430.0,
			7.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Rough shoulder-carried sack for household market purchases and dry goods."
		);

		CreateItem(
			"medieval_domestic_hemp_work_pouch",
			"pouch",
			"a hemp work pouch",
			null,
			"This hemp work pouch is a very small, workmanlike pouch made from woven hemp. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			100.0,
			3.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Plain work pouch for nails, pegs, chalk, hooks, or other rough household bits."
		);

		CreateItem(
			"medieval_domestic_herb_gathering_pouch",
			"pouch",
			"an herb-gathering pouch",
			null,
			"This herb-gathering pouch is a small, workmanlike pouch made from woven hemp. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			170.0,
			5.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Breathable pouch for gathered herbs, flowers, tinder, or kitchen greens."
		);

		CreateItem(
			"medieval_domestic_herder_pack",
			"pack",
			"a herder pack",
			null,
			"This herder pack is a medium-sized, workmanlike pack made from worked leather. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1150.0,
			24.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Weathered pack for food, tools, spare cord, and outdoor household gear."
		);

		CreateItem(
			"medieval_domestic_horn_needle_case",
			"case",
			"a horn needle case",
			null,
			"This horn needle case is a tiny, well-made case worked from horn. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			45.0,
			12.0m,
			true,
			false,
			"horn",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small rigid case for needles, pins, and small mending tools."
		);

		CreateItem(
			"medieval_domestic_horn_powder_flask",
			"flask",
			"a horn powder flask",
			null,
			"This horn powder flask is a very small, workmanlike flask worked from horn. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			160.0,
			10.0m,
			true,
			false,
			"horn",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Hard beltable dry flask for powdered pigment, salt, chalk, or spice."
		);

		CreateItem(
			"medieval_domestic_household_accounts_satchel",
			"satchel",
			"a household accounts satchel",
			null,
			"This household accounts satchel is a small, well-made satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Good,
			570.0,
			34.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Satchel for tallies, account tablets, seals, and folded household records."
		);

		CreateItem(
			"medieval_domestic_household_arrow_quiver",
			"quiver",
			"a household arrow quiver",
			null,
			"This household arrow quiver is a small, workmanlike quiver made from worked leather. The body is long and narrow, with a stiffened mouth and a closed base. A shoulder strap is fixed along one side. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			18.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Quiver",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Shoulder quiver for arrows, bolts, long pins, or narrow rods stored at home."
		);

		CreateItem(
			"medieval_domestic_household_spending_purse",
			"purse",
			"a household spending purse",
			null,
			"This household spending purse is a very small, workmanlike purse made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			110.0,
			9.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Everyday purse for small purchases, household accounts, and minor valuables."
		);

		CreateItem(
			"medieval_domestic_hunting_quiver",
			"quiver",
			"a hunting quiver",
			null,
			"This hunting quiver is a small, well-made quiver made from worked leather. The body is long and narrow, with a stiffened mouth and a closed base. A shoulder strap is fixed along one side. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Good,
			720.0,
			28.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Quiver",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Better-made shoulder quiver for hunting arrows and outdoor household equipment."
		);

		CreateItem(
			"medieval_domestic_incense_sachet",
			"sachet",
			"an incense sachet",
			null,
			"This incense sachet is a tiny, well-made sachet made from woven silk. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			22.0,
			12.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Fine sachet for incense pieces, aromatic resin, or domestic devotional scenting."
		);

		CreateItem(
			"medieval_domestic_ivory_comb_case",
			"case",
			"an ivory comb case",
			null,
			"This ivory comb case is a very small, well-made case worked from ivory. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			110.0,
			36.0m,
			true,
			false,
			"ivory",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Fine rigid case for combs, pins, and dressing-table tools."
		);

		CreateItem(
			"medieval_domestic_key_pouch",
			"pouch",
			"a household key pouch",
			null,
			"This household key pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			115.0,
			8.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Belt pouch intended for carrying keys, small latch pins, and household access pieces."
		);

		CreateItem(
			"medieval_domestic_kitchen_errand_pack",
			"pack",
			"a kitchen errand pack",
			null,
			"This kitchen errand pack is a medium-sized, workmanlike pack made from coarse canvas. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			760.0,
			12.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Pack for carried food, utensils, pantry goods, and household errands."
		);

		CreateItem(
			"medieval_domestic_lamp_wick_pouch",
			"pouch",
			"a lamp-wick pouch",
			null,
			"This lamp-wick pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			65.0,
			3.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Clean pouch for spare lamp wicks, trimmed cord, and small lighting supplies."
		);

		CreateItem(
			"medieval_domestic_large_household_bundle",
			"bundle",
			"a large household bundle",
			null,
			"This large household bundle is a medium-sized, workmanlike bundle made from coarse canvas. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			500.0,
			8.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Soft tied bundle for moving clothes, bedding, or household odds."
		);

		CreateItem(
			"medieval_domestic_large_travel_waterskin",
			"waterskin",
			"a large travel waterskin",
			null,
			"This large travel waterskin is a medium-sized, workmanlike waterskin made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			680.0,
			22.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Larger shoulder waterskin for travel, field work, and household journeys."
		);

		CreateItem(
			"medieval_domestic_laundry_back_pack",
			"pack",
			"a laundry back-pack",
			null,
			"This laundry back-pack is a medium-sized, workmanlike pack made from woven linen. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			620.0,
			9.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Soft back-worn pack for laundry, bedding, or folded household cloth."
		);

		CreateItem(
			"medieval_domestic_laundry_carry_bag",
			"bag",
			"a laundry carry bag",
			null,
			"This laundry carry bag is a medium-sized, workmanlike bag made from woven linen. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			520.0,
			9.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Large cloth bag for carrying washing, folded linen, or bedding between household spaces."
		);

		CreateItem(
			"medieval_domestic_leather_belt_pouch",
			"pouch",
			"a leather belt pouch",
			null,
			"This leather belt pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			140.0,
			8.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Durable flap pouch for carried household tools, small valuables, and personal effects."
		);

		CreateItem(
			"medieval_domestic_leather_coin_purse",
			"purse",
			"a leather coin purse",
			null,
			"This leather coin purse is a very small, workmanlike purse made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			105.0,
			8.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Durable purse for coin, keys, seals, or compact personal goods."
		);

		CreateItem(
			"medieval_domestic_leather_shoulder_satchel",
			"satchel",
			"a leather shoulder satchel",
			null,
			"This leather shoulder satchel is a small, workmanlike satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			22.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Durable satchel for household papers, food, tools, and travel goods."
		);

		CreateItem(
			"medieval_domestic_leather_tool_roll",
			"roll",
			"a leather tool roll",
			null,
			"This leather tool roll is a small, well-made roll made from worked leather. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Good,
			360.0,
			22.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Tough roll for finer tools, styluses, knives, punches, and mending implements."
		);

		CreateItem(
			"medieval_domestic_leather_travel_pack",
			"pack",
			"a leather travel pack",
			null,
			"This leather travel pack is a medium-sized, well-made pack made from worked leather. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1300.0,
			42.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Durable back-worn pack for valuable travel goods and protected personal belongings."
		);

		CreateItem(
			"medieval_domestic_leather_writing_wallet",
			"wallet",
			"a leather writing wallet",
			null,
			"This leather writing wallet is a small, workmanlike wallet made from worked leather. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			16.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Flat wallet for a wax tablet, stylus, notes, or domestic writing pieces."
		);

		CreateItem(
			"medieval_domestic_linen_belt_pouch",
			"pouch",
			"a linen belt pouch",
			null,
			"This linen belt pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			90.0,
			3.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small tied pouch for coins, household tokens, thread, or other daily pocket goods."
		);

		CreateItem(
			"medieval_domestic_linen_bread_bag",
			"bag",
			"a linen bread bag",
			null,
			"This linen bread bag is a small, workmanlike bag made from woven linen. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			190.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Breathable cloth bag for bread, rolls, wrapped food, or kitchen errands."
		);

		CreateItem(
			"medieval_domestic_linen_coin_purse",
			"purse",
			"a linen coin purse",
			null,
			"This linen coin purse is a very small, workmanlike purse made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			75.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small purse for coins, tokens, and household spending money."
		);

		CreateItem(
			"medieval_domestic_linen_kerchief_bag",
			"bag",
			"a linen kerchief bag",
			null,
			"This linen kerchief bag is a small, workmanlike bag made from woven linen. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			160.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Light bag for kerchiefs, veils, napkins, and small clean cloths."
		);

		CreateItem(
			"medieval_domestic_linen_shopping_bag",
			"bag",
			"a linen shopping bag",
			null,
			"This linen shopping bag is a small, workmanlike bag made from woven linen. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			240.0,
			6.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Light cloth bag for carrying small domestic purchases and kitchen goods."
		);

		CreateItem(
			"medieval_domestic_market_purse",
			"purse",
			"a market purse",
			null,
			"This market purse is a very small, workmanlike purse made from coarse canvas. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			95.0,
			5.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Hard-wearing purse for market errands and domestic buying money."
		);

		CreateItem(
			"medieval_domestic_market_tote",
			"bag",
			"a market tote bag",
			null,
			"This market tote bag is a small, workmanlike bag made from coarse canvas. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			8.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Reusable shoulder bag for household shopping, vegetables, bread, and parcels."
		);

		CreateItem(
			"medieval_domestic_medicine_packet",
			"packet",
			"a medicine packet",
			null,
			"This medicine packet is a tiny, workmanlike packet made from layered paper. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			18.0,
			3.0m,
			true,
			false,
			"paper",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Folded packet for small doses of household remedy powder or dried medicinal matter."
		);

		CreateItem(
			"medieval_domestic_medicine_pouch",
			"pouch",
			"a medicine pouch",
			null,
			"This medicine pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			80.0,
			6.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small cloth pouch for domestic remedies, wrapped herbs, salves, and measured packets."
		);

		CreateItem(
			"medieval_domestic_medicine_satchel",
			"satchel",
			"a medicine satchel",
			null,
			"This medicine satchel is a small, well-made satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Good,
			580.0,
			30.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Satchel for remedy packets, herb pouches, small bottles, and household treatment supplies."
		);

		CreateItem(
			"medieval_domestic_mending_satchel",
			"satchel",
			"a mending satchel",
			null,
			"This mending satchel is a small, workmanlike satchel made from coarse canvas. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			380.0,
			12.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Shoulder satchel for patches, thread, needles, laces, and repair tools."
		);

		CreateItem(
			"medieval_domestic_miniature_sewing_box",
			"box",
			"a miniature sewing box",
			null,
			"This miniature sewing box is a small, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			14.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Small box for needles, thread, patches, thimbles, and household mending tools."
		);

		CreateItem(
			"medieval_domestic_moth_herb_sachet",
			"sachet",
			"a moth-herb sachet",
			null,
			"This moth-herb sachet is a tiny, workmanlike sachet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			30.0,
			3.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small sachet intended for storing with linens, furs, or clothing chests."
		);

		CreateItem(
			"medieval_domestic_needle_packet",
			"packet",
			"a needle packet",
			null,
			"This needle packet is a tiny, workmanlike packet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			18.0,
			2.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Tiny cloth packet for needles, pins, fishhooks, or small sharp goods."
		);

		CreateItem(
			"medieval_domestic_needle_purse",
			"purse",
			"a needle purse",
			null,
			"This needle purse is a very small, workmanlike purse made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			60.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small purse for needles, pins, buttons, and delicate repair pieces."
		);

		CreateItem(
			"medieval_domestic_needlework_pouch",
			"pouch",
			"a needlework pouch",
			null,
			"This needlework pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Soft domestic pouch for needles, thread cards, thimbles, and small sewing repairs."
		);

		CreateItem(
			"medieval_domestic_oak_tablet_case",
			"case",
			"an oak tablet case",
			null,
			"This oak tablet case is a small, workmanlike case built from oak boards. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			380.0,
			16.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Rigid case sized for small wax tablets, folded notes, or household account slips."
		);

		CreateItem(
			"medieval_domestic_oiled_document_wallet",
			"wallet",
			"an oiled document wallet",
			null,
			"This oiled document wallet is a small, well-made wallet made from worked leather. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Good,
			310.0,
			24.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Oiled leather wallet for folded papers, tablets, and protected account notes."
		);

		CreateItem(
			"medieval_domestic_painted_keepsake_box",
			"box",
			"a painted keepsake box",
			null,
			"This painted keepsake box is a small, well-made box built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			520.0,
			28.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Small painted box for keepsakes, beads, ribbons, and personal mementos."
		);

		CreateItem(
			"medieval_domestic_painted_leather_purse",
			"purse",
			"a painted leather purse",
			null,
			"This painted leather purse is a very small, well-made purse made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			120.0,
			18.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Fine purse with decorative surface work for personal valuables."
		);

		CreateItem(
			"medieval_domestic_pantry_satchel",
			"satchel",
			"a pantry satchel",
			null,
			"This pantry satchel is a small, workmanlike satchel made from woven linen. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			310.0,
			7.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Light satchel for kitchen packets, herbs, counted goods, and pantry errands."
		);

		CreateItem(
			"medieval_domestic_pantry_token_pouch",
			"pouch",
			"a pantry-token pouch",
			null,
			"This pantry-token pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			75.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small household pouch for tally tokens, marked scraps, tags, and pantry-counting pieces."
		);

		CreateItem(
			"medieval_domestic_paper_pattern_tube",
			"tube",
			"a paper pattern tube",
			null,
			"This paper pattern tube is a small, workmanlike tube made from layered paper. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
			SizeCategory.Small,
			ItemQuality.Standard,
			90.0,
			5.0m,
			true,
			false,
			"paper",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Container_Pouch",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Light tube for rolled patterns, notes, and household instructions."
		);

		CreateItem(
			"medieval_domestic_parchment_roll_case",
			"case",
			"a parchment roll case",
			null,
			"This parchment roll case is a small, workmanlike case made from layered parchment. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
			SizeCategory.Small,
			ItemQuality.Standard,
			130.0,
			9.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Container_Pouch",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Flexible roll case for patterns, household notes, and small written pieces."
		);

		CreateItem(
			"medieval_domestic_parchment_satchel",
			"satchel",
			"a parchment satchel",
			null,
			"This parchment satchel is a small, well-made satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Good,
			590.0,
			32.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Stiff satchel for folded parchment, letters, and household papers."
		);

		CreateItem(
			"medieval_domestic_patchwork_purse",
			"purse",
			"a patchwork purse",
			null,
			"This patchwork purse is a very small, plain purse made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			70.0,
			1.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Crude purse made from remnant cloth for low-value daily carrying."
		);

		CreateItem(
			"medieval_domestic_personal_account_box",
			"box",
			"a personal account box",
			null,
			"This personal account box is a small, workmanlike box built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			24.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Compact box for tally sticks, wax tablets, small weights, and household account slips."
		);

		CreateItem(
			"medieval_domestic_pilgrim_shoulder_bag",
			"bag",
			"a pilgrim shoulder bag",
			null,
			"This pilgrim shoulder bag is a small, workmanlike bag made from woven hemp. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			330.0,
			8.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Plain shoulder bag for travel food, tokens, documents, and personal devotional items."
		);

		CreateItem(
			"medieval_domestic_plain_shoulder_satchel",
			"satchel",
			"a plain shoulder satchel",
			null,
			"This plain shoulder satchel is a small, workmanlike satchel made from woven linen. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			10.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Simple shoulder container for errands, food parcels, cloth, and small domestic goods."
		);

		CreateItem(
			"medieval_domestic_plain_waterskin",
			"waterskin",
			"a plain waterskin",
			null,
			"This plain waterskin is a small, workmanlike waterskin made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			14.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Shoulder-carried liquid container for water, watered wine, or other permitted contents."
		);

		CreateItem(
			"medieval_domestic_portable_nesting_box",
			"box",
			"a portable nesting box",
			null,
			"This portable nesting box is a small, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			18.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Small lidded box that can hold nested little boxes, packets, or personal goods."
		);

		CreateItem(
			"medieval_domestic_portable_relic_box",
			"box",
			"a small keepsake box",
			null,
			"This small keepsake box is a small, well-made box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			420.0,
			34.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Lidded personal box for tokens, keepsakes, prayer beads, or precious mementos."
		);

		CreateItem(
			"medieval_domestic_powder_packet",
			"packet",
			"a powder packet",
			null,
			"This powder packet is a tiny, workmanlike packet made from layered parchment. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			20.0,
			4.0m,
			true,
			false,
			"parchment",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small folded packet for powders, pigments, cosmetics, or fine dry materials."
		);

		CreateItem(
			"medieval_domestic_reedlike_scroll_quiver",
			"quiver",
			"a scroll quiver",
			null,
			"This scroll quiver is a small, workmanlike quiver made from worked leather. The body is long and narrow, with a stiffened mouth and a closed base. A shoulder strap is fixed along one side. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			500.0,
			18.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Quiver",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Quiver-shaped case for rolled documents, arrows, or similarly long narrow goods."
		);

		CreateItem(
			"medieval_domestic_relic_sachet",
			"sachet",
			"a tiny keepsake sachet",
			null,
			"This tiny keepsake sachet is a tiny, well-made sachet made from woven silk. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			18.0,
			14.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Fine tiny sachet for a keepsake, charm, bead, token, or devotional scrap."
		);

		CreateItem(
			"medieval_domestic_repair_pouch",
			"pouch",
			"a repair pouch",
			null,
			"This repair pouch is a small, workmanlike pouch made from coarse canvas. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			170.0,
			6.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small work pouch for awls, needles, scraps, laces, and mending supplies."
		);

		CreateItem(
			"medieval_domestic_ring_pouch",
			"pouch",
			"a ring pouch",
			null,
			"This ring pouch is a very small, well-made pouch made from woven silk. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			55.0,
			18.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Fine soft pouch for rings, earrings, small jewels, and other precious personal pieces."
		);

		CreateItem(
			"medieval_domestic_ring_purse",
			"purse",
			"a ring purse",
			null,
			"This ring purse is a very small, well-made purse made from woven silk. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			55.0,
			22.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Soft purse suited to rings and small precious objects."
		);

		CreateItem(
			"medieval_domestic_saffron_silk_sachet",
			"sachet",
			"a saffron silk sachet",
			null,
			"This saffron silk sachet is a tiny, well-made sachet made from woven silk. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			15.0,
			18.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Tiny fine sachet for rare spices, scent, or measured elite household goods."
		);

		CreateItem(
			"medieval_domestic_salt_sachet",
			"sachet",
			"a salt sachet",
			null,
			"This salt sachet is a tiny, workmanlike sachet made from woven hemp. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			2.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small packet for salt, coarse seasoning, or little dry household stores."
		);

		CreateItem(
			"medieval_domestic_scribe_satchel",
			"satchel",
			"a scribe satchel",
			null,
			"This scribe satchel is a small, well-made satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Good,
			620.0,
			36.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Better-made satchel for tablets, pens, packets, and domestic documents."
		);

		CreateItem(
			"medieval_domestic_seal_and_wax_box",
			"box",
			"a seal-and-wax box",
			null,
			"This seal-and-wax box is a small, well-made box built from boxwood. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			400.0,
			26.0m,
			true,
			false,
			"boxwood",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Small lidded box for seal matrices, wax pieces, cords, and domestic document fittings."
		);

		CreateItem(
			"medieval_domestic_seal_pouch",
			"pouch",
			"a seal pouch",
			null,
			"This seal pouch is a very small, well-made pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			120.0,
			14.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small protective pouch for personal seals, wax fragments, and household account tokens."
		);

		CreateItem(
			"medieval_domestic_shoe_carry_bag",
			"bag",
			"a shoe carry bag",
			null,
			"This shoe carry bag is a small, workmanlike bag made from coarse canvas. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			6.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Shoulder bag for spare shoes, sandals, shoe brushes, or household footgear."
		);

		CreateItem(
			"medieval_domestic_shoulder_quiver",
			"quiver",
			"a leather shoulder quiver",
			null,
			"This leather shoulder quiver is a small, workmanlike quiver made from worked leather. The body is long and narrow, with a stiffened mouth and a closed base. A shoulder strap is fixed along one side. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			18.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Quiver",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Quiver for arrows, bolts, short rods, or other long narrow carried items."
		);

		CreateItem(
			"medieval_domestic_shoulder_waterskin",
			"waterskin",
			"a shoulder waterskin",
			null,
			"This shoulder waterskin is a small, workmanlike waterskin made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			18.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Shoulder-carried liquid container for household travel, work, or errands."
		);

		CreateItem(
			"medieval_domestic_silk_gift_purse",
			"purse",
			"a silk gift purse",
			null,
			"This silk gift purse is a very small, well-made purse made from woven silk. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			45.0,
			26.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small fine purse suited to jewellery, gift tokens, and elite personal display."
		);

		CreateItem(
			"medieval_domestic_skin_bundle_pack",
			"pack",
			"an animal-skin pack",
			null,
			"This animal-skin pack is a medium-sized, workmanlike pack made from cured animal skin. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1100.0,
			20.0m,
			true,
			false,
			"animal skin",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Rough hide pack for outdoor carrying, domestic travel, and durable field use."
		);

		CreateItem(
			"medieval_domestic_small_belt_waterskin",
			"waterskin",
			"a small belt waterskin",
			null,
			"This small belt waterskin is a small, workmanlike waterskin made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			12.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small beltable liquid vessel for personal drinking water."
		);

		CreateItem(
			"medieval_domestic_small_food_wallet",
			"wallet",
			"a small food wallet",
			null,
			"This small food wallet is a small, workmanlike wallet made from woven linen. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Simple wallet for wrapped food, flatbread, dried fruit, or kitchen portions."
		);

		CreateItem(
			"medieval_domestic_small_prayer_book_satchel",
			"satchel",
			"a small prayer-book satchel",
			null,
			"This small prayer-book satchel is a small, well-made satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Good,
			430.0,
			28.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Compact shoulder satchel for a small book, beads, papers, and devotional items."
		);

		CreateItem(
			"medieval_domestic_small_spool_pouch",
			"pouch",
			"a small spool pouch",
			null,
			"This small spool pouch is a very small, workmanlike pouch made from woven wool. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			80.0,
			4.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Soft pouch for thread spools, lace ends, and small mending pieces."
		);

		CreateItem(
			"medieval_domestic_small_waterskin",
			"waterskin",
			"a small waterskin",
			null,
			"This small waterskin is a small, workmanlike waterskin made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			320.0,
			12.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small beltable liquid container for water or drink during errands."
		);

		CreateItem(
			"medieval_domestic_soap_flake_sachet",
			"sachet",
			"a soap-flake sachet",
			null,
			"This soap-flake sachet is a tiny, workmanlike sachet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			25.0,
			3.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small domestic sachet for soap flakes, washing scent, or linen-care materials."
		);

		CreateItem(
			"medieval_domestic_soft_cloth_bundle",
			"bundle",
			"a soft cloth bundle",
			null,
			"This soft cloth bundle is a small, workmanlike bundle made from woven wool. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			5.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Tied bundle for spare clothing, bedding pieces, or household cloth."
		);

		CreateItem(
			"medieval_domestic_spare_clothes_pack",
			"pack",
			"a spare-clothes pack",
			null,
			"This spare-clothes pack is a medium-sized, workmanlike pack made from woven linen. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			640.0,
			10.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Soft pack for clothing changes, folded linens, or travel garments."
		);

		CreateItem(
			"medieval_domestic_spice_pinch_pouch",
			"pouch",
			"a spice-pinch pouch",
			null,
			"This spice-pinch pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			55.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Tiny pouch for pinches of spice, salt, seeds, or measured dry flavourings."
		);

		CreateItem(
			"medieval_domestic_spice_sachet",
			"sachet",
			"a spice sachet",
			null,
			"This spice sachet is a tiny, workmanlike sachet made from woven linen. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			28.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Tiny sachet for measured spices, household flavourings, or aromatic packets."
		);

		CreateItem(
			"medieval_domestic_spindle_pouch",
			"pouch",
			"a spindle pouch",
			null,
			"This spindle pouch is a small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			120.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Longer pouch for a drop spindle, small distaff parts, thread, and fibre-working pieces."
		);

		CreateItem(
			"medieval_domestic_split_leather_pouch",
			"pouch",
			"a split leather pouch",
			null,
			"This split leather pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			155.0,
			9.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Pouch divided into small internal spaces for keys, rings, seals, or personal counters."
		);

		CreateItem(
			"medieval_domestic_stitched_coin_pouch",
			"pouch",
			"a stitched coin pouch",
			null,
			"This stitched coin pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			110.0,
			7.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small beltable pouch for coins and tokens without giving it built-in lock behaviour."
		);

		CreateItem(
			"medieval_domestic_strap_hung_bottle_basket",
			"basket",
			"a strap-hung bottle basket",
			null,
			"This strap-hung bottle basket is a small, workmanlike basket built from willow boards. The body is woven around a firm rim, with a flat base and open mouth. The weave tightens at the lower corners where the load bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Rigid shoulder basket for bottles, jars, and fragile kitchen goods."
		);

		CreateItem(
			"medieval_domestic_string_and_cord_pouch",
			"pouch",
			"a string-and-cord pouch",
			null,
			"This string-and-cord pouch is a very small, workmanlike pouch made from woven hemp. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			85.0,
			3.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Rough pouch for string, cord, ties, and small household bindings."
		);

		CreateItem(
			"medieval_domestic_sweet_scent_sachet",
			"sachet",
			"a sweet-scent sachet",
			null,
			"This sweet-scent sachet is a tiny, well-made sachet made from woven silk. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			20.0,
			10.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Fine sachet for scented herbs, flowers, and personal fragrance packets."
		);

		CreateItem(
			"medieval_domestic_tablet_and_stylus_pouch",
			"pouch",
			"a tablet-and-stylus pouch",
			null,
			"This tablet-and-stylus pouch is a small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			16.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Pouch fitted to carry a small tablet, stylus, and folded account pieces."
		);

		CreateItem(
			"medieval_domestic_tablet_banded_purse",
			"purse",
			"a tablet-banded purse",
			null,
			"This tablet-banded purse is a very small, well-made purse made from woven wool. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			90.0,
			14.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Decorated purse for coins, gift money, or formal-use personal goods."
		);

		CreateItem(
			"medieval_domestic_tablet_woven_pouch",
			"pouch",
			"a tablet-woven pouch",
			null,
			"This tablet-woven pouch is a very small, well-made pouch made from woven wool. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			95.0,
			12.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Decorated small pouch for household valuables, prayer beads, or gift tokens."
		);

		CreateItem(
			"medieval_domestic_tea_leaf_sachet",
			"sachet",
			"a leaf sachet",
			null,
			"This leaf sachet is a tiny, workmanlike sachet made from layered paper. The body is a small folded parcel with stitched edges and a tied neck. The seams are close enough to keep fine contents gathered in the centre. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			16.0,
			3.0m,
			true,
			false,
			"paper",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Folded packet for dried leaves, herbs, or other delicate domestic plant matter."
		);

		CreateItem(
			"medieval_domestic_thread_pouch",
			"pouch",
			"a thread pouch",
			null,
			"This thread pouch is a very small, workmanlike pouch made from woven linen. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			60.0,
			3.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small pouch for thread rolls, spare laces, patches, and domestic textile repairs."
		);

		CreateItem(
			"medieval_domestic_tinder_pouch",
			"pouch",
			"a tinder pouch",
			null,
			"This tinder pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			125.0,
			7.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Small leather pouch for tinder, flint fragments, wick scraps, and dry fire-starting supplies."
		);

		CreateItem(
			"medieval_domestic_tool_carry_satchel",
			"satchel",
			"a household tool satchel",
			null,
			"This household tool satchel is a small, workmanlike satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			20.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Shoulder satchel for small domestic tools, repair pieces, hooks, nails, and cords."
		);

		CreateItem(
			"medieval_domestic_travel_spice_box",
			"box",
			"a travel spice box",
			null,
			"This travel spice box is a small, well-made box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			480.0,
			32.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Small lidded box for travel spices, scent packets, and measured dry goods."
		);

		CreateItem(
			"medieval_domestic_walnut_document_case",
			"case",
			"a walnut document case",
			null,
			"This walnut document case is a small, well-made case built from walnut boards. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			430.0,
			30.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Finer rigid case for household papers, seals, and protected personal writing."
		);

		CreateItem(
			"medieval_domestic_washed_linen_bundle",
			"bundle",
			"a washed linen bundle",
			null,
			"This washed linen bundle is a medium-sized, workmanlike bundle made from woven linen. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			360.0,
			7.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sack",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Clean tied bundle for fresh linen, towels, napery, or folded garments."
		);

		CreateItem(
			"medieval_domestic_wax_tablet_pouch",
			"pouch",
			"a wax-tablet pouch",
			null,
			"This wax-tablet pouch is a small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			230.0,
			10.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Flat beltable pouch sized for a small wax tablet, stylus, folded note, or household account slip."
		);

		CreateItem(
			"medieval_domestic_waxed_canvas_pouch",
			"pouch",
			"a waxed canvas pouch",
			null,
			"This waxed canvas pouch is a very small, well-made pouch made from coarse canvas. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			135.0,
			10.0m,
			true,
			false,
			"canvas",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Slightly weather-shedding pouch for tinder, waxed thread, tablets, or small dry packets."
		);

		CreateItem(
			"medieval_domestic_winter_fur_pack",
			"pack",
			"a fur-lined winter pack",
			null,
			"This fur-lined winter pack is a medium-sized, well-made pack made from dressed fur. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1500.0,
			45.0m,
			true,
			false,
			"fur",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Warm-lined pack for winter travel, spare mitts, clothing, and protected goods."
		);

		CreateItem(
			"medieval_domestic_wooden_spool_case",
			"case",
			"a wooden spool case",
			null,
			"This wooden spool case is a very small, workmanlike case built from beech boards. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			180.0,
			10.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Compact case for thread spools, lace ends, and fine textile supplies."
		);

		CreateItem(
			"medieval_domestic_wool_drawstring_pouch",
			"pouch",
			"a wool drawstring pouch",
			null,
			"This wool drawstring pouch is a very small, workmanlike pouch made from woven wool. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			85.0,
			4.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Soft drawstring pouch suited to counters, small game pieces, sewing odds, or minor keepsakes."
		);

		CreateItem(
			"medieval_domestic_wool_lined_book_bag",
			"bag",
			"a wool-lined book bag",
			null,
			"This wool-lined book bag is a small, well-made bag made from woven wool. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Good,
			420.0,
			24.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Soft-lined bag for books, tablets, papers, and objects needing some padding."
		);

		CreateItem(
			"medieval_domestic_wool_travel_pack",
			"pack",
			"a wool travel pack",
			null,
			"This wool travel pack is a medium-sized, workmanlike pack made from woven wool. The storage body is broad and soft-backed, with a flap over the opening and straps fixed into the upper corners. The base is reinforced where weight settles. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			16.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pack",
				"Wear_Backpack"
			],
			null,
			null,
			null,
			null,
			"Back-worn pack for spare clothes, food, bedding, and travel goods."
		);

		CreateItem(
			"medieval_domestic_woven_willow_bag",
			"bag",
			"a woven willow shoulder bag",
			null,
			"This woven willow shoulder bag is a small, workmanlike bag built from willow boards. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			10.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Rigid woven shoulder bag for eggs, bread, vegetables, or light household goods."
		);

		CreateItem(
			"medieval_locking_personal_coin_casket",
			"casket",
			"a small coin casket",
			null,
			"This small coin casket is a small, well-made casket worked from brass. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			2600.0,
			110.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small metal casket for household coin and compact valuables."
		);

		CreateItem(
			"medieval_locking_personal_ivory_inlaid_casket",
			"casket",
			"an ivory-inlaid casket",
			null,
			"This ivory-inlaid casket is a small, well-made casket worked from ivory. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.Small,
			ItemQuality.Good,
			1500.0,
			240.0m,
			true,
			false,
			"ivory",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Fine small casket with visible ivory panels and built-in lock behaviour."
		);

		CreateItem(
			"medieval_locking_personal_jewel_casket",
			"casket",
			"a small jewel casket",
			null,
			"This small jewel casket is a small, well-made casket built from walnut boards. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1800.0,
			120.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small lockable casket for jewellery, seals, keepsakes, or precious trinkets."
		);

		CreateItem(
			"medieval_locking_personal_keepsake_lockbox",
			"lockbox",
			"a keepsake lockbox",
			null,
			"This keepsake lockbox is a small, workmanlike lockbox built from ash boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			54.0m,
			true,
			false,
			"ash",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Plain lockbox for personal letters, tokens, and small possessions."
		);

		CreateItem(
			"medieval_locking_personal_letter_lockbox",
			"lockbox",
			"a letter lockbox",
			null,
			"This letter lockbox is a small, workmanlike lockbox built from beech boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1600.0,
			48.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small built-in-lock box for private letters and folded documents."
		);

		CreateItem(
			"medieval_locking_personal_seal_casket",
			"casket",
			"a seal casket",
			null,
			"This seal casket is a small, well-made casket worked from bronze. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			2200.0,
			130.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small bronze casket for personal seals, rings, or signet equipment as stored items."
		);

		CreateItem(
			"medieval_locking_personal_thread_casket",
			"casket",
			"a needlework lock casket",
			null,
			"This needlework lock casket is a small, well-made casket built from walnut boards. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1800.0,
			85.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Lockable casket for fine thread, needles, patterns, and personal work goods."
		);

		CreateItem(
			"medieval_locking_personal_travel_lockbox",
			"lockbox",
			"a travel lockbox",
			null,
			"This travel lockbox is a small, workmanlike lockbox built from oak boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			3200.0,
			70.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Stout portable lockbox sized for travel baggage."
		);

		CreateItem(
			"medieval_locking_personal_cedar_scent_casket",
			"casket",
			"a cedar scent casket",
			null,
			"This cedar scent casket is a small, well-made casket built from cedar boards. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			100.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small lockable casket for scented cloths, sachets, and private keepsakes."
		);

		CreateItem(
			"medieval_locking_personal_bazaar_jewel_box",
			"box",
			"a brass-inlaid jewel box",
			null,
			"This brass-inlaid jewel box is a small, well-made box built from cedar boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1800.0,
			155.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small built-in-lock box with decorative inlay."
		);

		CreateItem(
			"medieval_locking_personal_teak_keepsake_box",
			"box",
			"a teak keepsake box",
			null,
			"This teak keepsake box is a small, well-made box built from teak boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1900.0,
			125.0m,
			true,
			false,
			"teak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Lockable teak box for private possessions and heirlooms."
		);

		CreateItem(
			"medieval_locking_personal_cypress_document_casket",
			"casket",
			"a cypress document casket",
			null,
			"This cypress document casket is a small, well-made casket built from cypress boards. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1700.0,
			115.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small lockable casket for papers, seals, and household records."
		);

		CreateItem(
			"medieval_abbasid_reed_pen_case",
			"case",
			"a boxwood pen case",
			null,
			"This boxwood pen case is a very small, well-made case built from boxwood. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			120.0,
			20.0m,
			true,
			false,
			"boxwood",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Personal pen case emphasizing literate urban domestic use."
		);

		CreateItem(
			"medieval_andalusian_embroidered_silk_purse",
			"purse",
			"an embroidered silk purse",
			null,
			"This embroidered silk purse is a very small, well-made purse made from woven silk. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			55.0,
			30.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Fine purse for urban elite household dress and valuable personal goods."
		);

		CreateItem(
			"medieval_anglo_danish_bronze_fitted_key_pouch",
			"pouch",
			"a bronze-fitted key pouch",
			null,
			"This bronze-fitted key pouch is a very small, well-made pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			145.0,
			16.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Distinctive key pouch for mixed insular and North-Sea domestic settings."
		);

		CreateItem(
			"medieval_byzantine_silk_reliquary_pouch",
			"pouch",
			"a silk keepsake pouch",
			null,
			"This silk keepsake pouch is a very small, well-made pouch made from woven silk. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			60.0,
			28.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Fine pouch for devotional keepsakes, jewellery, or treasured household tokens."
		);

		CreateItem(
			"medieval_capetian_french_moulded_leather_document_wallet",
			"wallet",
			"a moulded leather document wallet",
			null,
			"This moulded leather document wallet is a small, well-made wallet made from worked leather. The soft body has stitched sides, a shaped mouth, and reinforced corners. Its closure is visible on the front rather than hidden inside the folds. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Good,
			320.0,
			32.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Refined flat wallet for household records, letters, or accounts."
		);

		CreateItem(
			"medieval_carolingian_frankish_tablet_banded_alms_purse",
			"purse",
			"a tablet-banded alms purse",
			null,
			"This tablet-banded alms purse is a very small, well-made purse made from woven wool. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			90.0,
			14.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Decorated purse for alms coins and formal household display."
		);

		CreateItem(
			"medieval_christian_iberian_olivewood_document_case",
			"case",
			"an olivewood document case",
			null,
			"This olivewood document case is a small, well-made case built from olive wood. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			440.0,
			30.0m,
			true,
			false,
			"olive wood",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Regional hardwood case for domestic records and personal documents."
		);

		CreateItem(
			"medieval_early_anglo_saxon_ash_tally_pouch",
			"pouch",
			"an ash tally pouch",
			null,
			"This ash tally pouch is a very small, workmanlike pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			130.0,
			9.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Practical pouch for tally pieces, small tools, or household count tokens."
		);

		CreateItem(
			"medieval_fatimid_linen_scent_satchel",
			"satchel",
			"a linen scent satchel",
			null,
			"This linen scent satchel is a small, well-made satchel made from woven linen. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Good,
			260.0,
			18.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Light shoulder satchel for scented packets, folded cloth, and warm-climate personal goods."
		);

		CreateItem(
			"medieval_goryeo_korea_silk_lined_jewel_purse",
			"purse",
			"a silk-lined jewel purse",
			null,
			"This silk-lined jewel purse is a very small, well-made purse made from woven silk. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			60.0,
			28.0m,
			true,
			false,
			"silk",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Fine personal purse for jewellery, beads, or formal household objects."
		);

		CreateItem(
			"medieval_heian_kamakura_japan_cypress_document_box",
			"box",
			"a cypress document box",
			null,
			"This cypress document box is a small, well-made box built from cypress boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			480.0,
			36.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Light hardwood box for papers, folded cloth, keepsakes, and domestic correspondence."
		);

		CreateItem(
			"medieval_heian_kamakura_japan_paper_packet_case",
			"case",
			"a paper packet case",
			null,
			"This paper packet case is a very small, workmanlike case made from layered paper. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			45.0,
			6.0m,
			true,
			false,
			"paper",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Light packet-style case for folded papers, scent packets, or small household notes."
		);

		CreateItem(
			"medieval_holy_roman_empire_german_iron_fitted_strong_pouch",
			"pouch",
			"an iron-fitted strong pouch",
			null,
			"This iron-fitted strong pouch is a small, well-made pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Good,
			280.0,
			28.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Stiffened belt pouch with visible fittings for valuables and keys without built-in lock mechanics."
		);

		CreateItem(
			"medieval_irish_gaelic_hide_bound_shoulder_satchel",
			"satchel",
			"a hide-bound shoulder satchel",
			null,
			"This hide-bound shoulder satchel is a small, workmanlike satchel made from cured animal skin. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			18.0m,
			true,
			false,
			"animal skin",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Rugged satchel with hide-facing construction for wet or rural households."
		);

		CreateItem(
			"medieval_magyar_bronze_mounted_belt_pouch",
			"pouch",
			"a bronze-mounted belt pouch",
			null,
			"This bronze-mounted belt pouch is a very small, well-made pouch made from worked leather. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			160.0,
			20.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Metal-fitted pouch for mounted domestic travel and personal valuables."
		);

		CreateItem(
			"medieval_norse_viking_age_ship_carved_birch_document_case",
			"case",
			"a ship-carved birch case",
			null,
			"This ship-carved birch case is a small, well-made case built from birch boards. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			410.0,
			28.0m,
			true,
			false,
			"birch",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Regional rigid case emphasizing carved birch household storage for documents and keepsakes."
		);

		CreateItem(
			"medieval_north_indian_rajput_cotton_shoulder_bag",
			"bag",
			"a cotton shoulder bag",
			null,
			"This cotton shoulder bag is a small, workmanlike bag made from woven cotton. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Small,
			ItemQuality.Standard,
			240.0,
			8.0m,
			true,
			false,
			"cotton",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Light warm-climate shoulder bag for errands, cloth, food, and household goods."
		);

		CreateItem(
			"medieval_rus_novgorod_birchbark_style_packet_case",
			"case",
			"a birch packet case",
			null,
			"This birch packet case is a small, workmanlike case built from birch boards. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			250.0,
			14.0m,
			true,
			false,
			"birch",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Birch personal case for packets, wax tablets, and folded household notes."
		);

		CreateItem(
			"medieval_rus_novgorod_winter_fur_purse",
			"purse",
			"a fur-lined winter purse",
			null,
			"This fur-lined winter purse is a very small, well-made purse made from dressed fur. A folded flap covers the mouth, with a drawstring channel and tight side seams holding the small body together. Two narrow loops at the back let it sit against a belt. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			130.0,
			18.0m,
			true,
			false,
			"fur",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Purse",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Soft winter purse suited to cold-climate clothing and protected small valuables."
		);

		CreateItem(
			"medieval_seljuk_ayyubid_mamluk_tooled_leather_medicine_satchel",
			"satchel",
			"a tooled leather medicine satchel",
			null,
			"This tooled leather medicine satchel is a small, well-made satchel made from worked leather. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Good,
			620.0,
			34.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Decorated satchel for remedy packets, small vessels, and domestic treatment supplies."
		);

		CreateItem(
			"medieval_song_china_bamboo_scroll_satchel",
			"satchel",
			"a bamboo scroll satchel",
			null,
			"This bamboo scroll satchel is a small, well-made satchel built from split bamboo. A single shoulder strap rises from the sides, leaving the open mouth easy to reach. The lower corners are reinforced to carry household weight. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			360.0,
			24.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Light rigid satchel for scrolls, papers, brushes, and domestic documents."
		);

		CreateItem(
			"medieval_song_china_cypress_brush_case",
			"case",
			"a cypress brush case",
			null,
			"This cypress brush case is a very small, well-made case built from cypress boards. The body is narrow and rigid, with a capped end and a smooth outer surface. A carrying strap is fixed to the side. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			130.0,
			22.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch",
				"Beltable"
			],
			null,
			null,
			null,
			null,
			"Fine personal case for brushes, pens, and small writing implements."
		);

		CreateItem(
			"medieval_south_indian_chola_sandalwood_cosmetic_box",
			"box",
			"a sandalwood cosmetic box",
			null,
			"This sandalwood cosmetic box is a small, well-made box built from sandalwood. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			360.0,
			48.0m,
			true,
			false,
			"sandalwood",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Fragrant-wood personal box for grooming and dressing-table goods."
		);

		CreateItem(
			"medieval_south_indian_chola_teak_document_box",
			"box",
			"a small teak document box",
			null,
			"This small teak document box is a small, well-made box built from teak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			520.0,
			34.0m,
			true,
			false,
			"teak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Small hardwood personal box for documents, jewellery, or temple-household tokens."
		);

		CreateItem(
			"medieval_steppe_turkic_mongol_felt_travel_saddlebag",
			"bag",
			"a felt travel saddlebag",
			null,
			"This felt travel saddlebag is a medium-sized, workmanlike bag made from pressed felt. The body is a soft tube of fabric with a gathered mouth, stitched side seams, and a reinforced bottom. The top cinches tight against the load inside. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1000.0,
			22.0m,
			true,
			false,
			"felt",
			[
				"Functions / Container",
				"Functions / Container / Porous Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tote",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Felt shoulder bag or saddlebag-shaped personal container for mobile households."
		);

		CreateItem(
			"medieval_steppe_turkic_mongol_leather_horseman_quiver",
			"quiver",
			"a leather horseman quiver",
			null,
			"This leather horseman quiver is a small, well-made quiver made from worked leather. The body is long and narrow, with a stiffened mouth and a closed base. A shoulder strap is fixed along one side. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Good,
			760.0,
			30.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Quiver",
				"Wear_Shoulder"
			],
			null,
			null,
			null,
			null,
			"Riding-oriented quiver form useful for mounted and mobile household equipment."
		);

		CreateItem(
			"medieval_domestic_ash_peg_shelf",
			"shelf",
			"an ash peg shelf",
			null,
			"This ash peg shelf is a medium-sized, workmanlike shelf built from ash boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2500.0,
			16.0m,
			true,
			false,
			"ash",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Wall shelf with pegs or edge space for small objects and hanging household pieces."
		);

		CreateItem(
			"medieval_domestic_bed_linen_box",
			"box",
			"a bed-linen box",
			null,
			"This bed-linen box is a large, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			13000.0,
			46.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Blanket_Box"
			],
			null,
			null,
			null,
			null,
			"Long box for sheets, coverlets, pillow cloths, and bedding."
		);

		CreateItem(
			"medieval_domestic_bed_storage_surface",
			"bed",
			"a bed storage surface",
			null,
			"This bed storage surface is a large, workmanlike bed made from woven wool. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			38.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
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
			"Bed surface suitable for laying out clothing, bedding, bags, and personal containers."
		);

		CreateItem(
			"medieval_domestic_bed_surface_storage",
			"bed",
			"a storage bed surface",
			null,
			"This storage bed surface is a very large, workmanlike bed built from pine boards. A broad sleeping surface rests on a stable frame, with low edges and room for bedding. The side rails are rubbed smooth where people climb in. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			42000.0,
			110.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
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
			"Bed surface that can hold blankets, clothing, packs, and domestic goods."
		);

		CreateItem(
			"medieval_domestic_bedding_shelf",
			"shelf",
			"a bedding shelf",
			null,
			"This bedding shelf is a large, workmanlike shelf built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			6800.0,
			38.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Heavy shelf for blankets, quilts, bedding bundles, and pillows."
		);

		CreateItem(
			"medieval_domestic_bedroom_drawer_stand",
			"stand",
			"a bedroom drawer stand",
			null,
			"This bedroom drawer stand is a medium-sized, workmanlike stand built from beech boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			7200.0,
			44.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Nightstand"
			],
			null,
			null,
			null,
			null,
			"Small drawer stand for chamber goods, grooming pieces, pouches, and nightly items."
		);

		CreateItem(
			"medieval_domestic_bedside_coffer",
			"coffer",
			"a bedside coffer",
			null,
			"This bedside coffer is a medium-sized, workmanlike coffer built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			9200.0,
			46.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Small bedside coffer for personal goods, books, purses, and chamber items."
		);

		CreateItem(
			"medieval_domestic_bedside_night_chest",
			"nightstand",
			"a bedside night chest",
			null,
			"This bedside night chest is a medium-sized, workmanlike nightstand built from pine boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6500.0,
			28.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Nightstand"
			],
			null,
			null,
			null,
			null,
			"Small bedside chest for lamps, cloths, books, cups, and chamber goods."
		);

		CreateItem(
			"medieval_domestic_bedside_nightstand",
			"nightstand",
			"a bedside nightstand",
			null,
			"This bedside nightstand is a medium-sized, workmanlike nightstand built from oak boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			8200.0,
			48.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Nightstand"
			],
			null,
			null,
			null,
			null,
			"Bedside storage for lamps, books, purses, drinking vessels, and personal goods."
		);

		CreateItem(
			"medieval_domestic_beech_clothes_chest",
			"chest",
			"a beech clothes chest",
			null,
			"This beech clothes chest is a large, workmanlike chest built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			64.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Clothing chest for tunics, veils, folded garments, and small personal containers."
		);

		CreateItem(
			"medieval_domestic_bench_surface_storage",
			"bench",
			"a storage bench surface",
			null,
			"This storage bench surface is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			66.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Bench surface for satchels, blankets, trays, and everyday household goods."
		);

		CreateItem(
			"medieval_domestic_blanket_ark",
			"ark",
			"a blanket ark",
			null,
			"This blanket ark is a large, workmanlike ark built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			50.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Blanket_Box"
			],
			null,
			null,
			null,
			null,
			"Large lidded box for blankets, bolsters, bedding, and winter cloth."
		);

		CreateItem(
			"medieval_domestic_blanket_box",
			"box",
			"a plain blanket box",
			null,
			"This plain blanket box is a large, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			16500.0,
			48.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Blanket_Box"
			],
			null,
			null,
			null,
			null,
			"Long lidded box for blankets, linens, rolled bedding, and soft household goods."
		);

		CreateItem(
			"medieval_domestic_book_and_tablet_shelves",
			"shelves",
			"book-and-tablet shelves",
			null,
			"These book-and-tablet shelves are large, well-made shelves built from walnut boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			7500.0,
			82.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bookcase_Shelves"
			],
			null,
			null,
			null,
			null,
			"Fine shelves for books, tablets, documents, and household papers."
		);

		CreateItem(
			"medieval_domestic_bookcase_shelves",
			"bookcase",
			"a plain bookcase",
			null,
			"This plain bookcase is a large, workmanlike bookcase built from oak boards. Open shelves rise inside a tall frame, with the front edges rubbed smooth by repeated use. The back is plain and closely fitted behind the shelf boards. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9800.0,
			48.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bookcase_Shelves"
			],
			null,
			null,
			null,
			null,
			"Bookcase-style shelves for books, tablets, rolls, and writing boxes."
		);

		CreateItem(
			"medieval_domestic_boot_chest",
			"chest",
			"a boot chest",
			null,
			"This boot chest is a large, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			16000.0,
			46.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Sturdy chest for boots, shoes, gaiters, and spare footgear."
		);

		CreateItem(
			"medieval_domestic_bread_basket",
			"basket",
			"a bread basket",
			null,
			"This bread basket is a small, workmanlike basket built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			4.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open domestic basket for bread, rolls, cloth-covered food, and table goods."
		);

		CreateItem(
			"medieval_domestic_bread_cupboard",
			"cupboard",
			"a bread cupboard",
			null,
			"This bread cupboard is a large, workmanlike cupboard built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			10200.0,
			38.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Cupboard for bread, baked goods, cloth covers, and pantry staples."
		);

		CreateItem(
			"medieval_domestic_bread_hutch",
			"hutch",
			"a bread hutch",
			null,
			"This bread hutch is a large, workmanlike hutch built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			20000.0,
			62.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Hutch"
			],
			null,
			null,
			null,
			null,
			"Kitchen hutch for bread, cloth covers, bowls, and pantry goods."
		);

		CreateItem(
			"medieval_domestic_broom_and_tool_rack",
			"rack",
			"a broom-and-tool rack",
			null,
			"This broom-and-tool rack is a large, workmanlike rack built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			5200.0,
			20.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Weapon_Rack"
			],
			null,
			null,
			null,
			null,
			"Rack suitable for brooms, poles, rods, household tools, and staff-like objects."
		);

		CreateItem(
			"medieval_domestic_button_box",
			"box",
			"a household button box",
			null,
			"This household button box is a small, workmanlike box built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			12.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Small box for buttons, toggles, beads, and clothing repair pieces."
		);

		CreateItem(
			"medieval_domestic_candle_chest",
			"chest",
			"a candle chest",
			null,
			"This candle chest is a medium-sized, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			7800.0,
			34.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Compact chest for candles, wicks, snuffers, and lighting stores."
		);

		CreateItem(
			"medieval_domestic_candle_end_box",
			"box",
			"a candle-end box",
			null,
			"This candle-end box is a small, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			500.0,
			10.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
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
			"Small box for candle ends, spare wicks, and lighting odds."
		);

		CreateItem(
			"medieval_domestic_candle_wall_shelf",
			"shelf",
			"a candle wall shelf",
			null,
			"This candle wall shelf is a medium-sized, workmanlike shelf built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2000.0,
			12.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
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
			"Shelf for candles, wick pouches, snuffers, and lighting supplies."
		);

		CreateItem(
			"medieval_domestic_cedar_bookcase",
			"bookcase",
			"a cedar bookcase",
			null,
			"This cedar bookcase is a large, well-made bookcase built from cedar boards. Open shelves rise inside a tall frame, with the front edges rubbed smooth by repeated use. The back is plain and closely fitted behind the shelf boards. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			8200.0,
			70.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bookcase_Shelves"
			],
			null,
			null,
			null,
			null,
			"Fine bookcase for manuscripts, document boxes, and treasured written goods."
		);

		CreateItem(
			"medieval_domestic_cedar_cloak_wardrobe",
			"wardrobe",
			"a cedar cloak wardrobe",
			null,
			"This cedar cloak wardrobe is a large, well-made wardrobe built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			22500.0,
			120.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wardrobe"
			],
			null,
			null,
			null,
			null,
			"Aromatic wardrobe for cloaks, mantles, furs, and fine garments."
		);

		CreateItem(
			"medieval_domestic_cedar_linen_chest",
			"chest",
			"a cedar linen chest",
			null,
			"This cedar linen chest is a large, well-made chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			15500.0,
			92.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Aromatic chest for linens, bedding, fine cloth, and stored garments."
		);

		CreateItem(
			"medieval_domestic_ceremony_sideboard",
			"sideboard",
			"a carved ceremony sideboard",
			null,
			"This carved ceremony sideboard is a large, well-made sideboard built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			190.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Sideboard"
			],
			null,
			null,
			null,
			null,
			"Fine sideboard for feast vessels, ceremonial goods, and elite display storage."
		);

		CreateItem(
			"medieval_domestic_children_toy_chest",
			"chest",
			"a children's toy chest",
			null,
			"This children's toy chest is a large, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			11000.0,
			32.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Plain chest for toys, small clothes, wooden animals, and childhood belongings."
		);

		CreateItem(
			"medieval_domestic_childrens_cupboard",
			"cupboard",
			"a small children's cupboard",
			null,
			"This small children's cupboard is a medium-sized, workmanlike cupboard built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			7600.0,
			30.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Low cupboard for small garments, toys, teaching boards, and household keepsakes."
		);

		CreateItem(
			"medieval_domestic_childrens_wardrobe",
			"wardrobe",
			"a small children's wardrobe",
			null,
			"This small children's wardrobe is a large, workmanlike wardrobe built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			17000.0,
			58.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wardrobe"
			],
			null,
			null,
			null,
			null,
			"Smaller wardrobe for children's clothing, shoes, toys, and linens."
		);

		CreateItem(
			"medieval_domestic_clerk_desk_drawers",
			"desk",
			"a clerk's desk with drawers",
			null,
			"This clerk's desk with drawers is a large, well-made desk built from walnut boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			25000.0,
			160.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Drawers"
			],
			null,
			null,
			null,
			null,
			"Fine desk with drawers for documents, pouches, weights, tablets, and writing supplies."
		);

		CreateItem(
			"medieval_domestic_cloak_chest",
			"chest",
			"a cloak chest",
			null,
			"This cloak chest is a large, workmanlike chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			21000.0,
			78.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Large chest for cloaks, mantles, hats, hoods, and outdoor wear."
		);

		CreateItem(
			"medieval_domestic_cloth_scrap_basket",
			"basket",
			"a cloth-scrap basket",
			null,
			"This cloth-scrap basket is a medium-sized, workmanlike basket built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			800.0,
			7.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Basket for cloth scraps, rags, patches, and textile household waste."
		);

		CreateItem(
			"medieval_domestic_coin_and_weight_box",
			"box",
			"a coin-and-weight box",
			null,
			"This coin-and-weight box is a small, well-made box built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			40.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
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
			"Small lidded box for coins, weights, tally pieces, and household account tools."
		);

		CreateItem(
			"medieval_domestic_comb_and_mirror_box",
			"box",
			"a comb-and-mirror box",
			null,
			"This comb-and-mirror box is a small, workmanlike box built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			450.0,
			18.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Small chamber box for combs, mirrors, pins, and dressing goods."
		);

		CreateItem(
			"medieval_domestic_corner_cup_shelf",
			"shelf",
			"a corner cup shelf",
			null,
			"This corner cup shelf is a medium-sized, workmanlike shelf built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2200.0,
			14.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
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
			"Small corner shelf for cups, bowls, lamps, and domestic odds."
		);

		CreateItem(
			"medieval_domestic_corner_cupboard",
			"cupboard",
			"a corner cupboard",
			null,
			"This corner cupboard is a large, workmanlike cupboard built from beech boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			11800.0,
			52.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Corner-shaped cupboard for dishes, linens, jars, or chamber goods."
		);

		CreateItem(
			"medieval_domestic_corner_wall_shelf",
			"shelf",
			"a corner wall shelf",
			null,
			"This corner wall shelf is a medium-sized, workmanlike shelf built from beech boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2100.0,
			14.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Corner shelf for cups, jars, lamps, or compact household goods."
		);

		CreateItem(
			"medieval_domestic_cot_surface_storage",
			"cot",
			"a narrow cot surface",
			null,
			"This narrow cot surface is a large, workmanlike cot built from pine boards. A low sleeping surface stretches across a simple frame, with the fabric cover pulled tight. The corners are reinforced where weight bears down. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			48.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
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
			"Narrow cot surface for bedding, clothing, and small household items."
		);

		CreateItem(
			"medieval_domestic_couch_storage_surface",
			"couch",
			"a padded storage couch",
			null,
			"This padded storage couch is a large, well-made couch made from woven wool. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			13000.0,
			76.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Couch_Surface"
			],
			null,
			null,
			null,
			null,
			"Soft couch surface for cushions, blankets, trays, and chamber goods."
		);

		CreateItem(
			"medieval_domestic_couch_surface_storage",
			"couch",
			"a padded couch surface",
			null,
			"This padded couch surface is a large, well-made couch made from woven wool. A long cushioned surface rests on a low frame, with bolstered edges and enough depth for reclining. The front edge is smoothed where people sit. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			120.0m,
			true,
			false,
			"wool",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Couch_Surface"
			],
			null,
			null,
			null,
			null,
			"Soft seating surface that can hold cushions, cloth, small trays, and chamber goods."
		);

		CreateItem(
			"medieval_domestic_counter_surface",
			"counter",
			"a household counter",
			null,
			"This household counter is a large, workmanlike counter built from oak boards. A stable frame carries the weight, with the working surface placed where hands naturally reach it. The edges are worn smooth by household use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			26000.0,
			88.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Counter"
			],
			null,
			null,
			null,
			null,
			"Domestic counter surface for bowls, tools, trays, and food preparation goods."
		);

		CreateItem(
			"medieval_domestic_counting_table_drawers",
			"drawers",
			"counting-table drawers",
			null,
			"These counting-table drawers are medium-sized, well-made drawers built from oak boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			5600.0,
			58.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Drawers"
			],
			null,
			null,
			null,
			null,
			"Drawers for tally sticks, weights, tokens, and household account tools."
		);

		CreateItem(
			"medieval_domestic_covered_bread_hutch",
			"hutch",
			"a covered bread hutch",
			null,
			"This covered bread hutch is a large, workmanlike hutch built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12000.0,
			42.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Hutch"
			],
			null,
			null,
			null,
			null,
			"Hutch for bread, boards, cloth covers, and pantry goods."
		);

		CreateItem(
			"medieval_domestic_covered_kindling_bin",
			"bin",
			"a covered kindling bin",
			null,
			"This covered kindling bin is a large, workmanlike bin built from pine boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			6500.0,
			18.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open household bin for kindling, chips, and hearth wood."
		);

		CreateItem(
			"medieval_domestic_cup_shelf",
			"shelf",
			"a cup shelf",
			null,
			"This cup shelf is a medium-sized, workmanlike shelf built from beech boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2600.0,
			18.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Wall shelf for cups, mugs, beakers, and small drinking vessels."
		);

		CreateItem(
			"medieval_domestic_cupboard_hutch",
			"hutch",
			"a cupboard hutch",
			null,
			"This cupboard hutch is a large, well-made hutch built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			19000.0,
			92.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Hutch"
			],
			null,
			null,
			null,
			null,
			"Tall hutch for vessels, linens, dry goods, and visible domestic display."
		);

		CreateItem(
			"medieval_domestic_desk_surface",
			"desk",
			"a household writing desk",
			null,
			"This household writing desk is a large, well-made desk built from oak boards. A flat working top sits over a braced frame, with the front edge worn smooth by forearms. The surface is broad enough for papers, tablets, and small tools. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			110.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
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
			"Desk surface for papers, tablets, writing cases, and domestic records."
		);

		CreateItem(
			"medieval_domestic_devotional_display_shelf",
			"shelf",
			"a devotional display shelf",
			null,
			"This devotional display shelf is a medium-sized, well-made shelf built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2600.0,
			34.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Display_Shelves"
			],
			null,
			null,
			null,
			null,
			"Small display shelf for devotional objects, keepsakes, lamps, or icons."
		);

		CreateItem(
			"medieval_domestic_dish_cupboard",
			"cupboard",
			"a dish cupboard",
			null,
			"This dish cupboard is a large, workmanlike cupboard built from beech boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			14200.0,
			66.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Cupboard for plates, bowls, cups, serving vessels, and tableware."
		);

		CreateItem(
			"medieval_domestic_dish_wall_shelf",
			"shelf",
			"a dish wall shelf",
			null,
			"This dish wall shelf is a large, workmanlike shelf built from beech boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			4300.0,
			24.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Wall shelf built to hold plates, bowls, cups, and serving dishes."
		);

		CreateItem(
			"medieval_domestic_display_hutch",
			"hutch",
			"a display hutch",
			null,
			"This display hutch is a large, well-made hutch built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			150.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Hutch"
			],
			null,
			null,
			null,
			null,
			"Fine hutch for displaying bowls, cups, plates, and treasured household wares."
		);

		CreateItem(
			"medieval_domestic_display_shelves",
			"shelves",
			"a set of display shelves",
			null,
			"These set of display shelves are large, well-made shelves built from walnut boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			7800.0,
			72.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Display_Shelves"
			],
			null,
			null,
			null,
			null,
			"Open display shelves for bowls, heirlooms, fine vessels, and decorative household goods."
		);

		CreateItem(
			"medieval_domestic_dowry_chest",
			"chest",
			"a carved dowry chest",
			null,
			"This carved dowry chest is a large, well-made chest built from walnut boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			23000.0,
			140.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Fine household chest for textiles, keepsakes, jewellery boxes, and marriage goods."
		);

		CreateItem(
			"medieval_domestic_drinking_vessel_sideboard",
			"sideboard",
			"a drinking-vessel sideboard",
			null,
			"This drinking-vessel sideboard is a large, well-made sideboard built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			21000.0,
			130.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Sideboard"
			],
			null,
			null,
			null,
			null,
			"Fine sideboard for cups, ewers, flagons, and feast drinking vessels."
		);

		CreateItem(
			"medieval_domestic_egg_basket",
			"basket",
			"an egg basket",
			null,
			"This egg basket is a small, workmanlike basket built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			280.0,
			5.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Small open basket for eggs, delicate produce, or fragile household items."
		);

		CreateItem(
			"medieval_domestic_fine_guest_wardrobe",
			"wardrobe",
			"a fine guest wardrobe",
			null,
			"This fine guest wardrobe is a large, well-made wardrobe built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			150.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wardrobe"
			],
			null,
			null,
			null,
			null,
			"Fine wardrobe for guest garments, spare bedding, and valuable chamber cloth."
		);

		CreateItem(
			"medieval_domestic_fine_vessel_cabinet",
			"cabinet",
			"a fine vessel cabinet",
			null,
			"This fine vessel cabinet is a large, well-made cabinet built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			16000.0,
			135.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Large_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Fine cabinet for cups, bowls, pitchers, and valued household vessels."
		);

		CreateItem(
			"medieval_domestic_fine_walnut_armoire",
			"armoire",
			"a fine walnut armoire",
			null,
			"This fine walnut armoire is a large, well-made armoire built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			30000.0,
			180.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Armoire"
			],
			null,
			null,
			null,
			null,
			"High-status armoire for elite chamber storage, clothes, and precious boxes."
		);

		CreateItem(
			"medieval_domestic_fine_walnut_sideboard",
			"sideboard",
			"a fine walnut sideboard",
			null,
			"This fine walnut sideboard is a large, well-made sideboard built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			22000.0,
			160.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Sideboard"
			],
			null,
			null,
			null,
			null,
			"High-status sideboard for serving vessels, fine cloth, and display storage."
		);

		CreateItem(
			"medieval_domestic_firewood_niche_box",
			"box",
			"a firewood niche box",
			null,
			"This firewood niche box is a large, workmanlike box built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			28.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Sturdy open box for logs, billets, and hearth fuel."
		);

		CreateItem(
			"medieval_domestic_folded_cloth_press",
			"press",
			"a folded-cloth press",
			null,
			"This folded-cloth press is a large, workmanlike press built from beech boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			17000.0,
			72.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Large_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Broad press for folded garments, cloth lengths, and clean textiles."
		);

		CreateItem(
			"medieval_domestic_fruit_basket",
			"basket",
			"a fruit basket",
			null,
			"This fruit basket is a small, workmanlike basket built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			320.0,
			4.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open basket for fruit, nuts, vegetables, and table produce."
		);

		CreateItem(
			"medieval_domestic_fur_storage_chest",
			"chest",
			"a fur storage chest",
			null,
			"This fur storage chest is a large, well-made chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			17000.0,
			84.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Cedar chest for furs, winter hats, blankets, and fine woollens."
		);

		CreateItem(
			"medieval_domestic_fur_wardrobe",
			"wardrobe",
			"a fur wardrobe",
			null,
			"This fur wardrobe is a large, well-made wardrobe built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			135.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wardrobe"
			],
			null,
			null,
			null,
			null,
			"Wardrobe intended for storing furs, winter clothing, and scent sachets."
		);

		CreateItem(
			"medieval_domestic_glass_display_case",
			"case",
			"a glass display case",
			null,
			"This glass display case is a large, well-made case made from glass. The body is rigid and narrow, with a capped end and a smooth outer surface. The edges are fitted closely to protect what is carried inside. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.Large,
			ItemQuality.Good,
			15000.0,
			160.0m,
			true,
			false,
			"glass",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Display_Case"
			],
			null,
			null,
			null,
			null,
			"Rare transparent display case for treasured vessels, relics, jewellery, or fragile goods."
		);

		CreateItem(
			"medieval_domestic_glass_fronted_cabinet",
			"cabinet",
			"a glass-fronted cabinet",
			null,
			"This glass-fronted cabinet is a large, well-made cabinet made from glass. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.Large,
			ItemQuality.Good,
			18000.0,
			180.0m,
			true,
			false,
			"glass",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Glass_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Rare glass-fronted cabinet for elite display of vessels, books, or precious household goods."
		);

		CreateItem(
			"medieval_domestic_guest_chest",
			"chest",
			"a guest-room chest",
			null,
			"This guest-room chest is a large, workmanlike chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			19000.0,
			72.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Chamber chest for guest bedding, spare clothing, and temporary storage."
		);

		CreateItem(
			"medieval_domestic_hall_coffer",
			"coffer",
			"an oak hall coffer",
			null,
			"This oak hall coffer is a large, workmanlike coffer built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			82.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Hall coffer for mixed household storage, clothing, table linens, and guest goods."
		);

		CreateItem(
			"medieval_domestic_hall_service_sideboard",
			"sideboard",
			"a hall service sideboard",
			null,
			"This hall service sideboard is a large, workmanlike sideboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			19000.0,
			78.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Sideboard"
			],
			null,
			null,
			null,
			null,
			"Large sideboard for hall service, dishes, cups, and folded cloth."
		);

		CreateItem(
			"medieval_domestic_hall_sideboard",
			"sideboard",
			"a hall sideboard",
			null,
			"This hall sideboard is a large, workmanlike sideboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			24000.0,
			105.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Sideboard"
			],
			null,
			null,
			null,
			null,
			"Hall sideboard for cups, trays, pitchers, pouches, and guest-service goods."
		);

		CreateItem(
			"medieval_domestic_hanging_garment_cupboard",
			"cupboard",
			"a hanging garment cupboard",
			null,
			"This hanging garment cupboard is a large, workmanlike cupboard built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			15500.0,
			58.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Tall cupboard for hanging garments, cloaks, hoods, and spare belts."
		);

		CreateItem(
			"medieval_domestic_hearth_shelf",
			"shelf",
			"a hearth shelf",
			null,
			"This hearth shelf is a medium-sized, workmanlike shelf built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3200.0,
			18.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Shelf suited to utensils, small pots, tinder, and hearth-side domestic pieces."
		);

		CreateItem(
			"medieval_domestic_hearth_tool_bin",
			"bin",
			"a hearth-tool bin",
			null,
			"This hearth-tool bin is a medium-sized, workmanlike bin built from oak boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			20.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open bin for hearth tools, tongs, pokers, kindling, and fire supplies."
		);

		CreateItem(
			"medieval_domestic_herb_drawer_box",
			"box",
			"an herb drawer box",
			null,
			"This herb drawer box is a small, well-made box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			680.0,
			32.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
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
			"Small lidded box for herb packets, scent sachets, and remedy packets."
		);

		CreateItem(
			"medieval_domestic_herb_drying_shelf",
			"shelf",
			"an herb-drying shelf",
			null,
			"This herb-drying shelf is a medium-sized, workmanlike shelf built from willow boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			10.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
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
			"Open shelf for drying herbs, flowers, small packets, and kitchen plants."
		);

		CreateItem(
			"medieval_domestic_icon_display_shelves",
			"shelves",
			"a set of devotional display shelves",
			null,
			"These set of devotional display shelves are large, well-made shelves built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			7200.0,
			70.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Display_Shelves"
			],
			null,
			null,
			null,
			null,
			"Display shelves for household devotional pieces, lamps, small vessels, or keepsakes."
		);

		CreateItem(
			"medieval_domestic_ink_and_pen_box",
			"box",
			"an ink-and-pen box",
			null,
			"This ink-and-pen box is a small, well-made box built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			700.0,
			28.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
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
			"Small box for pens, ink pieces, wax, and writing supplies."
		);

		CreateItem(
			"medieval_domestic_keepsake_chest",
			"chest",
			"a keepsake chest",
			null,
			"This keepsake chest is a medium-sized, well-made chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7600.0,
			64.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
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
			"Smaller chest for keepsakes, personal papers, jewellery boxes, and fine pouches."
		);

		CreateItem(
			"medieval_domestic_kindling_basket",
			"basket",
			"a kindling basket",
			null,
			"This kindling basket is a medium-sized, workmanlike basket built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			8.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open basket for kindling, small fuel pieces, and hearth supplies."
		);

		CreateItem(
			"medieval_domestic_kitchen_bowl_cupboard",
			"cupboard",
			"a kitchen bowl cupboard",
			null,
			"This kitchen bowl cupboard is a large, workmanlike cupboard built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			13000.0,
			44.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Kitchen cupboard for bowls, cups, plates, and everyday serving wares."
		);

		CreateItem(
			"medieval_domestic_kitchen_hutch",
			"hutch",
			"an oak kitchen hutch",
			null,
			"This oak kitchen hutch is a large, workmanlike hutch built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			26000.0,
			105.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Hutch"
			],
			null,
			null,
			null,
			null,
			"Kitchen hutch for dishes, bowls, jars, towels, and cooking vessels."
		);

		CreateItem(
			"medieval_domestic_kitchen_service_hutch",
			"hutch",
			"a kitchen service hutch",
			null,
			"This kitchen service hutch is a large, workmanlike hutch built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			15000.0,
			54.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Hutch"
			],
			null,
			null,
			null,
			null,
			"Kitchen hutch for bowls, cups, spoons, towels, and common wares."
		);

		CreateItem(
			"medieval_domestic_larder_cupboard",
			"cupboard",
			"a larder cupboard",
			null,
			"This larder cupboard is a large, workmanlike cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			15500.0,
			68.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Cupboard for dry foods, sealed jars, bread, and kitchen stores."
		);

		CreateItem(
			"medieval_domestic_large_armoire",
			"armoire",
			"a large oak armoire",
			null,
			"This large oak armoire is a large, well-made armoire built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			32000.0,
			130.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Armoire"
			],
			null,
			null,
			null,
			null,
			"Large enclosed armoire for garments, chests, folded bedding, and valuables."
		);

		CreateItem(
			"medieval_domestic_large_bedding_ark",
			"ark",
			"a large bedding ark",
			null,
			"This large bedding ark is a large, workmanlike ark built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			24000.0,
			88.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Blanket_Box"
			],
			null,
			null,
			null,
			null,
			"Large chest-like ark for blankets, quilts, winter bedding, and spare cloth."
		);

		CreateItem(
			"medieval_domestic_large_oak_cabinet",
			"cabinet",
			"a large oak cabinet",
			null,
			"This large oak cabinet is a large, workmanlike cabinet built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			19000.0,
			86.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Large_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Large enclosed cabinet for domestic stores, linens, vessels, and household tools."
		);

		CreateItem(
			"medieval_domestic_large_pine_cabinet",
			"cabinet",
			"a large pine cabinet",
			null,
			"This large pine cabinet is a large, workmanlike cabinet built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			15500.0,
			56.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Large_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Large simple cabinet for bulky household goods and daily storage."
		);

		CreateItem(
			"medieval_domestic_large_table_surface",
			"table",
			"a large household table",
			null,
			"This large household table is a very large, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VeryLarge,
			ItemQuality.Standard,
			36000.0,
			120.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Large_Table"
			],
			null,
			null,
			null,
			null,
			"Large table surface for household work, dining wares, trays, and containers."
		);

		CreateItem(
			"medieval_domestic_large_walnut_cabinet",
			"cabinet",
			"a large walnut cabinet",
			null,
			"This large walnut cabinet is a large, well-made cabinet built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			17500.0,
			130.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Large_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Fine cabinet for elite household goods, vessels, writing boxes, and display storage."
		);

		CreateItem(
			"medieval_domestic_laundry_sorting_hamper",
			"hamper",
			"a laundry sorting hamper",
			null,
			"This laundry sorting hamper is a large, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1400.0,
			12.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open hamper for dirty linen, towels, clothing, and washday sorting."
		);

		CreateItem(
			"medieval_domestic_linen_armoire",
			"armoire",
			"a linen armoire",
			null,
			"This linen armoire is a large, workmanlike armoire built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			28500.0,
			110.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Armoire"
			],
			null,
			null,
			null,
			null,
			"Large armoire for sheets, coverlets, table linens, towels, and clothing."
		);

		CreateItem(
			"medieval_domestic_linen_cupboard",
			"cupboard",
			"a linen cupboard",
			null,
			"This linen cupboard is a large, workmanlike cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			15000.0,
			72.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Enclosed cupboard for folded linens, towels, bedding, and household cloth."
		);

		CreateItem(
			"medieval_domestic_linen_dresser",
			"dresser",
			"a linen dresser",
			null,
			"This linen dresser is a large, workmanlike dresser built from beech boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			21000.0,
			86.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Dresser"
			],
			null,
			null,
			null,
			null,
			"Drawer storage for folded cloth, linens, towels, and bedding pieces."
		);

		CreateItem(
			"medieval_domestic_linen_laundry_hamper",
			"hamper",
			"a linen laundry hamper",
			null,
			"This linen laundry hamper is a large, workmanlike hamper made from woven linen. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1200.0,
			10.0m,
			true,
			false,
			"linen",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Light open hamper for household laundry and soft goods."
		);

		CreateItem(
			"medieval_domestic_linen_press_cupboard",
			"cupboard",
			"a linen press cupboard",
			null,
			"This linen press cupboard is a large, well-made cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			21000.0,
			110.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Large cupboard for pressed linen, napery, and folded household cloth."
		);

		CreateItem(
			"medieval_domestic_linen_shelf",
			"shelf",
			"a linen shelf",
			null,
			"This linen shelf is a large, workmanlike shelf built from beech boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			5400.0,
			34.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Broad shelves for folded linen, towels, and domestic cloth."
		);

		CreateItem(
			"medieval_domestic_linen_sideboard",
			"sideboard",
			"a linen sideboard",
			null,
			"This linen sideboard is a large, workmanlike sideboard built from beech boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			20000.0,
			80.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Sideboard"
			],
			null,
			null,
			null,
			null,
			"Sideboard for table linens, towels, napery, and serving pieces."
		);

		CreateItem(
			"medieval_domestic_linen_sorting_hamper",
			"hamper",
			"a linen sorting hamper",
			null,
			"This linen sorting hamper is a large, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2100.0,
			16.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Large open hamper for sorting folded cloth, washing, and household linens."
		);

		CreateItem(
			"medieval_domestic_linen_wall_shelf",
			"shelf",
			"a linen wall shelf",
			null,
			"This linen wall shelf is a large, workmanlike shelf built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			4600.0,
			26.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Shelf for folded cloth, towels, household linens, and stored clothing."
		);

		CreateItem(
			"medieval_domestic_loom_accessory_chest",
			"chest",
			"a weaving chest",
			null,
			"This weaving chest is a large, workmanlike chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			70.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Chest for shuttles, heddle pieces, fibre packets, and domestic textile tools."
		);

		CreateItem(
			"medieval_domestic_loom_thread_cabinet",
			"cabinet",
			"a loom-thread cabinet",
			null,
			"This loom-thread cabinet is a large, well-made cabinet built from beech boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			14500.0,
			90.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Large_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Cabinet for skeins, shuttles, threads, and domestic textile work."
		);

		CreateItem(
			"medieval_domestic_low_bench_surface",
			"bench",
			"a low storage bench",
			null,
			"This low storage bench is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			54.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Bench surface for baskets, folded cloth, tools, and domestic objects."
		);

		CreateItem(
			"medieval_domestic_low_book_shelves",
			"shelves",
			"a set of low book shelves",
			null,
			"These set of low book shelves are medium-sized, workmanlike shelves built from beech boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5400.0,
			30.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bookcase_Shelves"
			],
			null,
			null,
			null,
			null,
			"Low shelves for books, tablets, teaching boards, and household records."
		);

		CreateItem(
			"medieval_domestic_low_chamber_sideboard",
			"sideboard",
			"a low chamber sideboard",
			null,
			"This low chamber sideboard is a large, workmanlike sideboard built from beech boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			15000.0,
			62.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Sideboard"
			],
			null,
			null,
			null,
			null,
			"Lower sideboard for chamber vessels, boxes, cloth, and dressing goods."
		);

		CreateItem(
			"medieval_domestic_medicine_cupboard",
			"cupboard",
			"a medicine cupboard",
			null,
			"This medicine cupboard is a medium-sized, well-made cupboard built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8200.0,
			64.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Small cupboard for remedies, herb packets, salves, and household treatment goods."
		);

		CreateItem(
			"medieval_domestic_medicine_nightstand",
			"nightstand",
			"a medicine nightstand",
			null,
			"This medicine nightstand is a medium-sized, well-made nightstand built from cedar boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			5600.0,
			52.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Nightstand"
			],
			null,
			null,
			null,
			null,
			"Small cedar nightstand for remedy packets, bottles, cloths, and cups."
		);

		CreateItem(
			"medieval_domestic_narrow_beech_cup_rack",
			"rack",
			"a narrow beech cup rack",
			null,
			"This narrow beech cup rack is a large, workmanlike rack built from beech boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			4800.0,
			26.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Narrow_Shelves"
			],
			null,
			null,
			null,
			null,
			"Narrow rack for cups, small bowls, drinking vessels, and serving pieces."
		);

		CreateItem(
			"medieval_domestic_narrow_cloak_cupboard",
			"cupboard",
			"a narrow cloak cupboard",
			null,
			"This narrow cloak cupboard is a large, workmanlike cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18500.0,
			76.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wardrobe"
			],
			null,
			null,
			null,
			null,
			"Narrow wardrobe-like cupboard for cloaks, hoods, belts, and travel garments."
		);

		CreateItem(
			"medieval_domestic_narrow_herb_shelf",
			"shelf",
			"a narrow herb shelf",
			null,
			"This narrow herb shelf is a medium-sized, workmanlike shelf built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2500.0,
			24.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Narrow_Shelves"
			],
			null,
			null,
			null,
			null,
			"Narrow shelves for herb bundles, sachets, remedies, and small jars."
		);

		CreateItem(
			"medieval_domestic_narrow_lamp_rack",
			"rack",
			"a narrow lamp rack",
			null,
			"This narrow lamp rack is a large, workmanlike rack built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			5400.0,
			28.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Narrow_Shelves"
			],
			null,
			null,
			null,
			null,
			"Open rack for lamps, candles, oil vessels, and lighting supplies."
		);

		CreateItem(
			"medieval_domestic_narrow_oak_shelf_rack",
			"rack",
			"a narrow oak shelf rack",
			null,
			"This narrow oak shelf rack is a large, workmanlike rack built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			6400.0,
			32.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Narrow_Shelves"
			],
			null,
			null,
			null,
			null,
			"Narrow shelf rack for a hall, pantry, chamber, or small storeroom."
		);

		CreateItem(
			"medieval_domestic_narrow_pine_pantry_rack",
			"rack",
			"a narrow pine pantry rack",
			null,
			"This narrow pine pantry rack is a large, workmanlike rack built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			5200.0,
			24.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Narrow_Shelves"
			],
			null,
			null,
			null,
			null,
			"Simple pantry rack for jars, sacks, crockery, and small food containers."
		);

		CreateItem(
			"medieval_domestic_narrow_scroll_rack",
			"rack",
			"a narrow scroll rack",
			null,
			"This narrow scroll rack is a large, well-made rack built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			5000.0,
			42.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Narrow_Shelves"
			],
			null,
			null,
			null,
			null,
			"Open rack for scroll tubes, papers, tablets, and writing cases."
		);

		CreateItem(
			"medieval_domestic_narrow_shoe_rack",
			"rack",
			"a narrow shoe rack",
			null,
			"This narrow shoe rack is a medium-sized, workmanlike rack built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3200.0,
			14.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Narrow_Shelves"
			],
			null,
			null,
			null,
			null,
			"Low narrow rack for shoes, sandals, boots, or household footgear."
		);

		CreateItem(
			"medieval_domestic_narrow_spice_cupboard",
			"cupboard",
			"a narrow spice cupboard",
			null,
			"This narrow spice cupboard is a medium-sized, well-made cupboard built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			6200.0,
			48.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Narrow cupboard for spices, scent packets, remedies, and small jars."
		);

		CreateItem(
			"medieval_domestic_needlework_stand_drawer",
			"drawer",
			"a needlework stand drawer",
			null,
			"This needlework stand drawer is a medium-sized, workmanlike drawer built from beech boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3600.0,
			24.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Drawers"
			],
			null,
			null,
			null,
			null,
			"Small drawer for thread, needles, patches, and textile work."
		);

		CreateItem(
			"medieval_domestic_nursery_chest",
			"chest",
			"a nursery chest",
			null,
			"This nursery chest is a large, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			14000.0,
			40.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Chest for children's clothing, bedding, toys, and small household goods."
		);

		CreateItem(
			"medieval_domestic_oak_chest_of_drawers",
			"dresser",
			"an oak chest of drawers",
			null,
			"This oak chest of drawers is a large, workmanlike dresser built from oak boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			24000.0,
			105.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Dresser"
			],
			null,
			null,
			null,
			null,
			"Drawer furniture for garments, linen, pouches, and personal goods."
		);

		CreateItem(
			"medieval_domestic_oak_footlocker",
			"footlocker",
			"an oak footlocker",
			null,
			"This oak footlocker is a medium-sized, workmanlike footlocker built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			11000.0,
			52.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Sturdy compact trunk for clothes, shoes, and personal containers."
		);

		CreateItem(
			"medieval_domestic_oak_household_cupboard",
			"cupboard",
			"an oak household cupboard",
			null,
			"This oak household cupboard is a large, workmanlike cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			16000.0,
			70.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Sturdy cupboard for ordinary domestic storage in a hall, chamber, or kitchen."
		);

		CreateItem(
			"medieval_domestic_oak_small_cabinet",
			"cabinet",
			"an oak small cabinet",
			null,
			"This oak small cabinet is a medium-sized, workmanlike cabinet built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			8600.0,
			46.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Sturdy compact cabinet for valuables, papers, vessels, or personal containers."
		);

		CreateItem(
			"medieval_domestic_oak_storage_chest",
			"chest",
			"an oak storage chest",
			null,
			"This oak storage chest is a large, workmanlike chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			21000.0,
			80.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Stout chest for household valuables, clothing, bedding, or stored wares."
		);

		CreateItem(
			"medieval_domestic_painted_small_cabinet",
			"cabinet",
			"a painted small cabinet",
			null,
			"This painted small cabinet is a medium-sized, well-made cabinet built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7200.0,
			48.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Decorated compact cabinet for chamber goods, small vessels, and personal storage."
		);

		CreateItem(
			"medieval_domestic_pantry_packet_cabinet",
			"cabinet",
			"a pantry packet cabinet",
			null,
			"This pantry packet cabinet is a large, workmanlike cabinet built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12500.0,
			54.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Large_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Cabinet for packets, jars, sachets, and small dry household stores."
		);

		CreateItem(
			"medieval_domestic_pine_clothes_press",
			"press",
			"a pine clothes press",
			null,
			"This pine clothes press is a large, workmanlike press built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			21000.0,
			68.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wardrobe"
			],
			null,
			null,
			null,
			null,
			"Simple clothes press for garments, folded linens, and chamber storage."
		);

		CreateItem(
			"medieval_domestic_plain_armoire",
			"armoire",
			"a plain pine armoire",
			null,
			"This plain pine armoire is a large, workmanlike armoire built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			26000.0,
			80.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Armoire"
			],
			null,
			null,
			null,
			null,
			"Large cupboard-like armoire for broad domestic storage and clothing."
		);

		CreateItem(
			"medieval_domestic_plain_clothes_press",
			"press",
			"a plain clothes press",
			null,
			"This plain clothes press is a large, workmanlike press built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			14500.0,
			50.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Large_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Plain domestic press for folded clothing and spare bedding."
		);

		CreateItem(
			"medieval_domestic_plain_desk_drawers",
			"desk",
			"a plain desk with drawers",
			null,
			"This plain desk with drawers is a large, workmanlike desk built from pine boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			21000.0,
			72.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Drawers"
			],
			null,
			null,
			null,
			null,
			"Simple writing-table storage for household records, pens, and small tools."
		);

		CreateItem(
			"medieval_domestic_plain_nightstand",
			"nightstand",
			"a plain pine nightstand",
			null,
			"This plain pine nightstand is a medium-sized, workmanlike nightstand built from pine boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6200.0,
			28.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Nightstand"
			],
			null,
			null,
			null,
			null,
			"Simple nightstand with storage for small chamber items."
		);

		CreateItem(
			"medieval_domestic_plain_oak_wardrobe",
			"wardrobe",
			"a plain oak wardrobe",
			null,
			"This plain oak wardrobe is a large, workmanlike wardrobe built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			26000.0,
			95.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wardrobe"
			],
			null,
			null,
			null,
			null,
			"Wardrobe for clothing, cloaks, belts, shoes, and folded domestic textiles."
		);

		CreateItem(
			"medieval_domestic_plain_pine_chest",
			"chest",
			"a plain pine chest",
			null,
			"This plain pine chest is a large, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			14500.0,
			42.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Basic lidded chest for clothes, linens, bedding, or household stores."
		);

		CreateItem(
			"medieval_domestic_plain_pine_counter",
			"counter",
			"a plain pine counter",
			null,
			"This plain pine counter is a large, workmanlike counter built from pine boards. A stable frame carries the weight, with the working surface placed where hands naturally reach it. The edges are worn smooth by household use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			21000.0,
			60.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Counter"
			],
			null,
			null,
			null,
			null,
			"Simple counter surface for household work and temporary storage."
		);

		CreateItem(
			"medieval_domestic_plain_pine_hutch",
			"hutch",
			"a plain pine kitchen hutch",
			null,
			"This plain pine kitchen hutch is a large, workmanlike hutch built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			21000.0,
			68.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Hutch"
			],
			null,
			null,
			null,
			null,
			"Simple kitchen hutch for ordinary crockery, food jars, and domestic wares."
		);

		CreateItem(
			"medieval_domestic_plain_pine_sideboard",
			"sideboard",
			"a plain pine sideboard",
			null,
			"This plain pine sideboard is a large, workmanlike sideboard built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			56.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Sideboard"
			],
			null,
			null,
			null,
			null,
			"Simple sideboard for cups, plates, folded cloth, and household tableware."
		);

		CreateItem(
			"medieval_domestic_plain_pine_wall_shelf",
			"shelf",
			"a plain pine wall shelf",
			null,
			"This plain pine wall shelf is a medium-sized, workmanlike shelf built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1900.0,
			10.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
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
			"Simple shelf for ordinary domestic storage above tables, beds, or work areas."
		);

		CreateItem(
			"medieval_domestic_plain_small_cabinet",
			"cabinet",
			"a plain small cabinet",
			null,
			"This plain small cabinet is a medium-sized, workmanlike cabinet built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6800.0,
			28.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Compact enclosed cabinet for simple domestic goods and chamber storage."
		);

		CreateItem(
			"medieval_domestic_plate_hutch",
			"hutch",
			"a plate hutch",
			null,
			"This plate hutch is a large, workmanlike hutch built from beech boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			14500.0,
			58.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Hutch"
			],
			null,
			null,
			null,
			null,
			"Hutch arranged for plates, platters, bowls, and serving pieces."
		);

		CreateItem(
			"medieval_domestic_prayer_bead_box",
			"box",
			"a prayer-bead box",
			null,
			"This prayer-bead box is a small, well-made box built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			400.0,
			30.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
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
			"Small cedar box for beads, devotional pieces, keepsakes, and scent sachets."
		);

		CreateItem(
			"medieval_domestic_ring_and_pin_box",
			"box",
			"a ring-and-pin box",
			null,
			"This ring-and-pin box is a very small, well-made box built from boxwood. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			180.0,
			22.0m,
			true,
			false,
			"boxwood",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
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
			"Small box for rings, pins, brooches, and fine domestic objects."
		);

		CreateItem(
			"medieval_domestic_robe_wardrobe",
			"wardrobe",
			"a robe wardrobe",
			null,
			"This robe wardrobe is a large, well-made wardrobe built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			120.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wardrobe"
			],
			null,
			null,
			null,
			null,
			"Tall wardrobe for robes, cloaks, belts, and folded fine clothing."
		);

		CreateItem(
			"medieval_domestic_root_cellar_bin",
			"bin",
			"a root-cellar bin",
			null,
			"This root-cellar bin is a large, workmanlike bin built from pine boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			7000.0,
			20.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open bin for roots, onions, cabbages, and pantry vegetables."
		);

		CreateItem(
			"medieval_domestic_rush_storage_bin",
			"bin",
			"a rush storage bin",
			null,
			"This rush storage bin is a large, workmanlike bin built from willow boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1800.0,
			12.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open bin for rushes, straw, reeds, and floor-covering materials."
		);

		CreateItem(
			"medieval_domestic_rushwork_hamper",
			"hamper",
			"a rushwork hamper",
			null,
			"This rushwork hamper is a large, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1200.0,
			11.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Light woven hamper for cloth, bedding, and household soft goods."
		);

		CreateItem(
			"medieval_domestic_servant_linen_hamper",
			"hamper",
			"a servant linen hamper",
			null,
			"This servant linen hamper is a large, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2200.0,
			14.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Simple open hamper for work linens, towels, bedding, and laundry rounds."
		);

		CreateItem(
			"medieval_domestic_serving_cupboard",
			"cupboard",
			"a serving cupboard",
			null,
			"This serving cupboard is a large, well-made cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			18000.0,
			86.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Better cupboard for feast dishes, pitchers, cups, and serving cloth."
		);

		CreateItem(
			"medieval_domestic_serving_sideboard",
			"sideboard",
			"an oak serving sideboard",
			null,
			"This oak serving sideboard is a large, workmanlike sideboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			23000.0,
			98.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Sideboard"
			],
			null,
			null,
			null,
			null,
			"Sideboard for serving dishes, cloths, cups, and table goods."
		);

		CreateItem(
			"medieval_domestic_sewing_chest",
			"chest",
			"a sewing chest",
			null,
			"This sewing chest is a medium-sized, well-made chest built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8200.0,
			58.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
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
			"Household sewing chest for cloth scraps, thread, needles, patterns, and mending tools."
		);

		CreateItem(
			"medieval_domestic_sewing_drawer_chest",
			"chest",
			"a sewing drawer chest",
			null,
			"This sewing drawer chest is a medium-sized, well-made chest built from beech boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8800.0,
			72.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Dresser"
			],
			null,
			null,
			null,
			null,
			"Drawer chest for thread, needles, lace, patches, and textile work."
		);

		CreateItem(
			"medieval_domestic_shoe_bin",
			"bin",
			"a shoe bin",
			null,
			"This shoe bin is a medium-sized, workmanlike bin built from pine boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3800.0,
			16.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open bin for shoes, sandals, boots, and household footgear."
		);

		CreateItem(
			"medieval_domestic_shoe_shelf",
			"shelf",
			"a shoe shelf",
			null,
			"This shoe shelf is a medium-sized, workmanlike shelf built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3000.0,
			14.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Narrow_Shelves"
			],
			null,
			null,
			null,
			null,
			"Low shelf for shoes, sandals, clogs, and spare footgear."
		);

		CreateItem(
			"medieval_domestic_side_table_storage",
			"table",
			"a side table with storage",
			null,
			"This side table with storage is a medium-sized, workmanlike table built from oak boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			9500.0,
			48.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Side_Table"
			],
			null,
			null,
			null,
			null,
			"Side-table container surface for lamps, bowls, books, and small chamber goods."
		);

		CreateItem(
			"medieval_domestic_sideboard_cupboards",
			"sideboard",
			"a cupboarded sideboard",
			null,
			"This cupboarded sideboard is a large, well-made sideboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			23000.0,
			112.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Sideboard"
			],
			null,
			null,
			null,
			null,
			"Sideboard with enclosed storage for vessels, cloth, knives, and serving goods."
		);

		CreateItem(
			"medieval_domestic_single_oak_wall_shelf",
			"shelf",
			"a single oak wall shelf",
			null,
			"This single oak wall shelf is a medium-sized, workmanlike shelf built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2800.0,
			18.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Single wall shelf for cups, bowls, books, candles, or small household goods."
		);

		CreateItem(
			"medieval_domestic_small_dresser",
			"dresser",
			"a small pine dresser",
			null,
			"This small pine dresser is a large, workmanlike dresser built from pine boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			17000.0,
			60.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Dresser"
			],
			null,
			null,
			null,
			null,
			"Chest-of-drawers style storage for folded garments, cloth, and small chamber goods."
		);

		CreateItem(
			"medieval_domestic_small_footlocker",
			"footlocker",
			"a small footlocker",
			null,
			"This small footlocker is a medium-sized, workmanlike footlocker built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			8200.0,
			30.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
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
			"Compact lidded footlocker for personal effects and chamber storage."
		);

		CreateItem(
			"medieval_domestic_small_pine_cupboard",
			"cupboard",
			"a small pine cupboard",
			null,
			"This small pine cupboard is a large, workmanlike cupboard built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			11500.0,
			44.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Enclosed household cupboard for pots, cloth, food packets, and everyday goods."
		);

		CreateItem(
			"medieval_domestic_small_table_surface",
			"table",
			"a small household table",
			null,
			"This small household table is a medium-sized, workmanlike table built from pine boards. A broad top rests on sturdy legs, with braces beneath to stop the frame from twisting. The upper surface is rubbed smooth from dishes, tools, and hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			11000.0,
			44.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Small table surface for domestic containers, cups, lamps, and personal effects."
		);

		CreateItem(
			"medieval_domestic_spice_cupboard",
			"cupboard",
			"a cedar spice cupboard",
			null,
			"This cedar spice cupboard is a medium-sized, well-made cupboard built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7800.0,
			62.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Compact cupboard for spice jars, sachets, scented packets, and small dry stores."
		);

		CreateItem(
			"medieval_domestic_spice_drawer_chest",
			"chest",
			"a spice drawer chest",
			null,
			"This spice drawer chest is a medium-sized, well-made chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			9500.0,
			78.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Dresser"
			],
			null,
			null,
			null,
			null,
			"Small drawer chest for spice packets, remedies, scent sachets, and kitchen valuables."
		);

		CreateItem(
			"medieval_domestic_spice_wall_shelf",
			"shelf",
			"a spice wall shelf",
			null,
			"This spice wall shelf is a medium-sized, well-made shelf built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2400.0,
			28.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
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
			"Small shelf for jars, packets, sachets, and aromatic domestic goods."
		);

		CreateItem(
			"medieval_domestic_spinning_wool_basket",
			"basket",
			"a spinning-wool basket",
			null,
			"This spinning-wool basket is a medium-sized, workmanlike basket built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			9.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open basket for wool, flax, thread, and domestic fibre work."
		);

		CreateItem(
			"medieval_domestic_table_drawer_box",
			"drawer",
			"a table drawer box",
			null,
			"This table drawer box is a medium-sized, workmanlike drawer built from oak boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4000.0,
			28.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Drawers"
			],
			null,
			null,
			null,
			null,
			"Drawer box for a table, suited to knives, wax, notes, and small domestic tools."
		);

		CreateItem(
			"medieval_domestic_table_linen_box",
			"box",
			"a table-linen box",
			null,
			"This table-linen box is a large, workmanlike box built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12000.0,
			52.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Blanket_Box"
			],
			null,
			null,
			null,
			null,
			"Long box for tablecloths, napery, folded towels, and serving cloth."
		);

		CreateItem(
			"medieval_domestic_thread_and_lace_box",
			"box",
			"a thread-and-lace box",
			null,
			"This thread-and-lace box is a small, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			500.0,
			14.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Small box for thread, lace, trim, and textile mending goods."
		);

		CreateItem(
			"medieval_domestic_towel_cupboard",
			"cupboard",
			"a towel cupboard",
			null,
			"This towel cupboard is a large, workmanlike cupboard built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			11200.0,
			42.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Simple cupboard for towels, washing cloths, and chamber linens."
		);

		CreateItem(
			"medieval_domestic_toy_basket",
			"basket",
			"a toy basket",
			null,
			"This toy basket is a medium-sized, workmanlike basket built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			850.0,
			8.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open basket for toys, balls, counters, and children's household goods."
		);

		CreateItem(
			"medieval_domestic_toy_chest",
			"chest",
			"a small toy chest",
			null,
			"This small toy chest is a medium-sized, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			7000.0,
			24.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
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
			"Low chest for toys, game pieces, teaching tablets, and children's goods."
		);

		CreateItem(
			"medieval_domestic_travel_gear_wardrobe",
			"wardrobe",
			"a travel-gear wardrobe",
			null,
			"This travel-gear wardrobe is a large, workmanlike wardrobe built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			24000.0,
			88.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wardrobe"
			],
			null,
			null,
			null,
			null,
			"Wardrobe for packs, cloaks, boots, spare straps, and outdoor household gear."
		);

		CreateItem(
			"medieval_domestic_under_bed_blanket_box",
			"box",
			"an under-bed blanket box",
			null,
			"This under-bed blanket box is a large, workmanlike box built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			13800.0,
			44.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Blanket_Box"
			],
			null,
			null,
			null,
			null,
			"Low blanket box sized for under-bed or foot-of-bed storage."
		);

		CreateItem(
			"medieval_domestic_wall_cupboard",
			"cupboard",
			"a wall-hung cupboard",
			null,
			"This wall-hung cupboard is a large, workmanlike cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12800.0,
			58.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Raised cupboard for keeping household goods away from floor damp and animals."
		);

		CreateItem(
			"medieval_domestic_wall_pegs_shelf",
			"shelf",
			"a wall-peg shelf",
			null,
			"This wall-peg shelf is a medium-sized, workmanlike shelf built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2800.0,
			20.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Wall shelf for keys, lamps, pouches, and small household goods."
		);

		CreateItem(
			"medieval_domestic_walnut_chest_of_drawers",
			"dresser",
			"a walnut chest of drawers",
			null,
			"This walnut chest of drawers is a large, well-made dresser built from walnut boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			22000.0,
			165.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Dresser"
			],
			null,
			null,
			null,
			null,
			"Fine drawer chest for elite chamber goods, clothing, and valuables."
		);

		CreateItem(
			"medieval_domestic_walnut_robe_wardrobe",
			"wardrobe",
			"a walnut robe wardrobe",
			null,
			"This walnut robe wardrobe is a large, well-made wardrobe built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			24500.0,
			155.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wardrobe"
			],
			null,
			null,
			null,
			null,
			"Fine wardrobe for robes, gowns, veils, belts, and elite household dress."
		);

		CreateItem(
			"medieval_domestic_walnut_small_cabinet",
			"cabinet",
			"a walnut small cabinet",
			null,
			"This walnut small cabinet is a medium-sized, well-made cabinet built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7600.0,
			78.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Fine small cabinet for valuables, writing goods, keepsakes, or elite chamber storage."
		);

		CreateItem(
			"medieval_domestic_washstand_drawer",
			"stand",
			"a washstand drawer",
			null,
			"This washstand drawer is a medium-sized, workmanlike stand built from beech boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5200.0,
			30.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Nightstand"
			],
			null,
			null,
			null,
			null,
			"Small drawer stand for towels, combs, soaps, and washing pieces."
		);

		CreateItem(
			"medieval_domestic_weapon_rack",
			"rack",
			"a household weapon rack",
			null,
			"This household weapon rack is a large, workmanlike rack built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			7600.0,
			38.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Weapon_Rack"
			],
			null,
			null,
			null,
			null,
			"Rack for weapons, staves, hunting gear, and similar long household objects."
		);

		CreateItem(
			"medieval_domestic_wide_beech_dish_shelves",
			"shelves",
			"a set of beech dish shelves",
			null,
			"These set of beech dish shelves are large, workmanlike shelves built from beech boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8200.0,
			38.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Wide shelves for plates, bowls, pitchers, and serving vessels."
		);

		CreateItem(
			"medieval_domestic_wide_cellar_shelves",
			"shelves",
			"a set of cellar shelves",
			null,
			"These set of cellar shelves are large, workmanlike shelves built from elm boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8800.0,
			36.0m,
			true,
			false,
			"elm",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Open shelving for cellar goods, jars, spare vessels, and household stock."
		);

		CreateItem(
			"medieval_domestic_wide_larder_shelves",
			"shelves",
			"a set of larder shelves",
			null,
			"These set of larder shelves are large, workmanlike shelves built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9400.0,
			44.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Stout open shelves for pantry jars, food boxes, sacks, and household stores."
		);

		CreateItem(
			"medieval_domestic_wide_linen_shelves",
			"shelves",
			"a set of linen shelves",
			null,
			"These set of linen shelves are large, workmanlike shelves built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8600.0,
			40.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Wide shelves for folded linens, bedding, curtains, and domestic cloth."
		);

		CreateItem(
			"medieval_domestic_wide_oak_shelves",
			"shelves",
			"a set of wide oak shelves",
			null,
			"These set of wide oak shelves are large, workmanlike shelves built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			42.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Broad shelving for household stores, crockery, folded cloth, tools, and bins."
		);

		CreateItem(
			"medieval_domestic_wide_pantry_shelf",
			"shelf",
			"a wide pantry shelf",
			null,
			"This wide pantry shelf is a large, workmanlike shelf built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			5200.0,
			30.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Wide shelves for dry stores, baskets, bowls, and packets."
		);

		CreateItem(
			"medieval_domestic_wide_pine_store_shelves",
			"shelves",
			"a set of pine store shelves",
			null,
			"These set of pine store shelves are large, workmanlike shelves built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			7600.0,
			30.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
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
			"Large simple shelving for domestic storerooms and household goods."
		);

		CreateItem(
			"medieval_domestic_willow_laundry_hamper",
			"hamper",
			"a willow laundry hamper",
			null,
			"This willow laundry hamper is a large, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2400.0,
			18.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open hamper for laundry, bedding, linens, and folded household cloth."
		);

		CreateItem(
			"medieval_domestic_winter_bedding_chest",
			"chest",
			"a winter bedding chest",
			null,
			"This winter bedding chest is a large, workmanlike chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			24000.0,
			90.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Blanket_Box"
			],
			null,
			null,
			null,
			null,
			"Large chest for blankets, furs, coverlets, and seasonal bedding."
		);

		CreateItem(
			"medieval_domestic_winter_mitten_bin",
			"bin",
			"a winter mitten bin",
			null,
			"This winter mitten bin is a medium-sized, workmanlike bin built from pine boards. Straight sides rise to a broad open mouth, and the base is braced to stand flat under a heavy load. The rim is thickened where hands grip it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3200.0,
			14.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open bin for mittens, hats, scarves, and cold-weather accessories."
		);

		CreateItem(
			"medieval_domestic_wool_mending_hamper",
			"hamper",
			"a wool mending hamper",
			null,
			"This wool mending hamper is a medium-sized, workmanlike hamper built from willow boards. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1100.0,
			10.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open hamper for mending, cloth scraps, thread pouches, and unfinished household repairs."
		);

		CreateItem(
			"medieval_domestic_writing_desk_drawers",
			"desk",
			"a writing desk with drawers",
			null,
			"This writing desk with drawers is a large, well-made desk built from oak boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			135.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Drawers"
			],
			null,
			null,
			null,
			null,
			"Writing desk with drawer storage for tablets, papers, pens, seal pouches, and account goods."
		);

		CreateItem(
			"medieval_domestic_writing_table_drawers",
			"drawers",
			"writing-table drawers",
			null,
			"These writing-table drawers are medium-sized, well-made drawers built from walnut boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			5200.0,
			64.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Drawers"
			],
			null,
			null,
			null,
			null,
			"Fine drawers for pens, wax, paper, account pieces, and seals."
		);

		CreateItem(
			"medieval_locking_domestic_bedchamber_coin_chest",
			"chest",
			"a bedchamber coin chest",
			null,
			"This bedchamber coin chest is a medium-sized, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			20000.0,
			260.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking chest for coin, tokens, and valued domestic goods."
		);

		CreateItem(
			"medieval_locking_domestic_bedding_lock_chest",
			"chest",
			"a bedding lock chest",
			null,
			"This bedding lock chest is a large, workmanlike chest built from pine boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			26000.0,
			140.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Large locking chest for bedding, blankets, or seasonal cloth."
		);

		CreateItem(
			"medieval_locking_domestic_deed_chest",
			"chest",
			"a household deed chest",
			null,
			"This household deed chest is a medium-sized, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			18000.0,
			210.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking chest for property records, charters, and household papers."
		);

		CreateItem(
			"medieval_locking_domestic_dowry_chest",
			"chest",
			"a lockable dowry chest",
			null,
			"This lockable dowry chest is a large, well-made chest built from walnut boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			34000.0,
			280.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Fine locking chest for dowry goods, heirlooms, or household display."
		);

		CreateItem(
			"medieval_locking_domestic_guesthouse_strongbox",
			"strongbox",
			"a guesthouse strongbox",
			null,
			"This guesthouse strongbox is a medium-sized, well-made strongbox built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			16000.0,
			190.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking strongbox for an inn, guesthouse, or rented chamber."
		);

		CreateItem(
			"medieval_locking_domestic_heavy_safe_chest",
			"chest",
			"a heavy safe chest",
			null,
			"This heavy safe chest is a large, well-made chest worked from wrought iron. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Large,
			ItemQuality.Good,
			70000.0,
			760.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
			null,
			null,
			null,
			null,
			"Heavy safe-chest profile for elite households, manor offices, or institutional rooms."
		);

		CreateItem(
			"medieval_locking_domestic_heirloom_coffer",
			"coffer",
			"an heirloom coffer",
			null,
			"This heirloom coffer is a medium-sized, well-made coffer built from walnut boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			15000.0,
			240.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking coffer for family heirlooms and valued household goods."
		);

		CreateItem(
			"medieval_locking_domestic_household_strong_chest",
			"chest",
			"a household strong chest",
			null,
			"This household strong chest is a large, well-made chest built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			42000.0,
			420.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
			null,
			null,
			null,
			null,
			"Heavy built-in-lock chest for household wealth and records."
		);

		CreateItem(
			"medieval_locking_domestic_ironbound_safe_chest",
			"chest",
			"an iron-bound safe chest",
			null,
			"This iron-bound safe chest is a large, well-made chest built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			52000.0,
			520.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
			null,
			null,
			null,
			null,
			"Large oak safe chest reinforced with iron bands."
		);

		CreateItem(
			"medieval_locking_domestic_linen_strong_chest",
			"chest",
			"a linen strong chest",
			null,
			"This linen strong chest is a large, well-made chest built from cedar boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			30000.0,
			220.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking chest for household linens, blankets, and better textiles."
		);

		CreateItem(
			"medieval_locking_domestic_locking_footlocker",
			"footlocker",
			"a locking footlocker",
			null,
			"This locking footlocker is a medium-sized, workmanlike footlocker built from oak boards. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			16000.0,
			130.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Compact chest for clothing, bedding, or personal effects."
		);

		CreateItem(
			"medieval_locking_domestic_medicine_chest",
			"chest",
			"a locked medicine chest",
			null,
			"This locked medicine chest is a medium-sized, well-made chest built from cedar boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			13000.0,
			185.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Built-in-lock chest for medicines, salves, and household stores."
		);

		CreateItem(
			"medieval_locking_domestic_oak_locking_chest",
			"chest",
			"a locking oak chest",
			null,
			"This locking oak chest is a large, workmanlike chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			32000.0,
			180.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Large household chest with built-in lock behaviour."
		);

		CreateItem(
			"medieval_locking_domestic_silver_chest",
			"chest",
			"a household silver chest",
			null,
			"This household silver chest is a medium-sized, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			22000.0,
			260.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Stout lockable chest for silver tableware or valuable household goods."
		);

		CreateItem(
			"medieval_locking_domestic_small_bronze_casket",
			"casket",
			"a bronze-bound casket",
			null,
			"This bronze-bound casket is a small, well-made casket worked from bronze. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			2800.0,
			160.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small metal casket for compact valuables."
		);

		CreateItem(
			"medieval_locking_domestic_wall_niche_lockbox",
			"lockbox",
			"a wall-niche lockbox",
			null,
			"This wall-niche lockbox is a small, workmanlike lockbox built from oak boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			3600.0,
			76.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small lockbox intended for storage niches, cabinets, or bedside use."
		);

		CreateItem(
			"medieval_locking_domestic_northern_sea_chest",
			"chest",
			"a sea-chest footlocker",
			null,
			"This sea-chest footlocker is a medium-sized, workmanlike chest built from pine boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			17000.0,
			135.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking footlocker suited to travel, shipboard storage, or northern households."
		);

		CreateItem(
			"medieval_locking_domestic_western_moulded_chest",
			"chest",
			"a moulded walnut chest",
			null,
			"This moulded walnut chest is a large, well-made chest built from walnut boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			34000.0,
			300.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Fine lockable chest with moulded panels."
		);

		CreateItem(
			"medieval_locking_domestic_mediterranean_cypress_chest",
			"chest",
			"a cypress linen chest",
			null,
			"This cypress linen chest is a large, well-made chest built from cypress boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			28000.0,
			230.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Cypress locking chest for linens and private household stores."
		);

		CreateItem(
			"medieval_locking_domestic_islamicate_cedar_chest",
			"chest",
			"a carved cedar chest",
			null,
			"This carved cedar chest is a large, well-made chest built from cedar boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			30000.0,
			270.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Carved cedar chest with built-in lock behaviour."
		);

		CreateItem(
			"medieval_locking_domestic_south_asian_teak_strongbox",
			"strongbox",
			"a teak strongbox",
			null,
			"This teak strongbox is a medium-sized, well-made strongbox built from teak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			22000.0,
			300.0m,
			true,
			false,
			"teak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Heavy teak strongbox for household valuables."
		);

		CreateItem(
			"medieval_locking_domestic_east_asian_cypress_chest",
			"chest",
			"a cypress household chest",
			null,
			"This cypress household chest is a medium-sized, well-made chest built from cypress boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			18000.0,
			240.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Lockable household chest for papers, clothing, and private goods."
		);

		CreateItem(
			"medieval_abbasid_cedar_document_cabinet",
			"cabinet",
			"a cedar document cabinet",
			null,
			"This cedar document cabinet is a large, well-made cabinet built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			15000.0,
			130.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Large_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Enclosed cabinet for papers, cases, writing goods, and household records."
		);

		CreateItem(
			"medieval_andalusian_carved_cedar_spice_cabinet",
			"cabinet",
			"a carved cedar spice cabinet",
			null,
			"This carved cedar spice cabinet is a medium-sized, well-made cabinet built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7800.0,
			90.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Compact cabinet for spices, scents, remedies, and fine dry household stores."
		);

		CreateItem(
			"medieval_anglo_danish_bronze_fitted_household_chest",
			"chest",
			"a bronze-fitted household chest",
			null,
			"This bronze-fitted household chest is a large, well-made chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			23000.0,
			120.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Regional metal-fitted household chest for textiles and valuables."
		);

		CreateItem(
			"medieval_anglo_norman_panelled_walnut_sideboard",
			"sideboard",
			"a panelled walnut sideboard",
			null,
			"This panelled walnut sideboard is a large, well-made sideboard built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			175.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Sideboard"
			],
			null,
			null,
			null,
			null,
			"Panelled serving storage for tableware and high-status domestic vessels."
		);

		CreateItem(
			"medieval_byzantine_walnut_display_shelves",
			"shelves",
			"a walnut display shelf",
			null,
			"These walnut display shelf are large, well-made shelves built from walnut boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			8500.0,
			100.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Display_Shelves"
			],
			null,
			null,
			null,
			null,
			"Fine display shelving for vessels, icons, lamps, and treasured household pieces."
		);

		CreateItem(
			"medieval_capetian_french_moulded_walnut_cupboard",
			"cupboard",
			"a moulded walnut cupboard",
			null,
			"This moulded walnut cupboard is a large, well-made cupboard built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			18000.0,
			145.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Refined enclosed cupboard for elite chamber storage."
		);

		CreateItem(
			"medieval_carolingian_frankish_tablet_banded_linen_chest",
			"chest",
			"a tablet-banded linen chest",
			null,
			"This tablet-banded linen chest is a large, well-made chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			21000.0,
			110.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Decorated chest for linens, clothing, and household display goods."
		);

		CreateItem(
			"medieval_christian_iberian_olivewood_linen_chest",
			"chest",
			"an olivewood linen chest",
			null,
			"This olivewood linen chest is a large, well-made chest built from olive wood. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			20000.0,
			125.0m,
			true,
			false,
			"olive wood",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Regional hardwood chest for linens and chamber textiles."
		);

		CreateItem(
			"medieval_early_anglo_saxon_ash_linen_cupboard",
			"cupboard",
			"an ash linen cupboard",
			null,
			"This ash linen cupboard is a large, workmanlike cupboard built from ash boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			14000.0,
			62.0m,
			true,
			false,
			"ash",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Practical regional cupboard for linens and clothing."
		);

		CreateItem(
			"medieval_fatimid_painted_linen_cupboard",
			"cupboard",
			"a painted linen cupboard",
			null,
			"This painted linen cupboard is a large, well-made cupboard built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			15000.0,
			115.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Decorated cupboard for folded linen, fine cloth, and warm-climate household storage."
		);

		CreateItem(
			"medieval_goryeo_korea_pine_clothing_chest",
			"chest",
			"a pine clothing chest",
			null,
			"This pine clothing chest is a large, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			17000.0,
			68.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Clothing chest for folded garments, bedding, and household textiles."
		);

		CreateItem(
			"medieval_heian_kamakura_japan_bamboo_paper_shelf",
			"shelf",
			"a bamboo paper shelf",
			null,
			"This bamboo paper shelf is a medium-sized, workmanlike shelf built from split bamboo. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3000.0,
			28.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Light shelf for paper packets, writing goods, small boxes, and domestic objects."
		);

		CreateItem(
			"medieval_heian_kamakura_japan_cypress_low_storage_box",
			"box",
			"a low cypress storage box",
			null,
			"This low cypress storage box is a large, well-made box built from cypress boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			12500.0,
			92.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Blanket_Box"
			],
			null,
			null,
			null,
			null,
			"Low storage box for folded robes, bedding, papers, and domestic goods."
		);

		CreateItem(
			"medieval_high_english_british_joined_oak_dresser",
			"dresser",
			"a joined oak dresser",
			null,
			"This joined oak dresser is a large, well-made dresser built from oak boards. Stacked drawers fill the front, each with a small pull and a narrow shadow line around it. The top is flat enough to hold a lamp and small household goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			140.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Dresser"
			],
			null,
			null,
			null,
			null,
			"Joined drawer furniture for clothing, linen, and valuables."
		);

		CreateItem(
			"medieval_holy_roman_empire_german_heavy_oak_wardrobe",
			"wardrobe",
			"a heavy oak wardrobe",
			null,
			"This heavy oak wardrobe is a large, well-made wardrobe built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			32000.0,
			150.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wardrobe"
			],
			null,
			null,
			null,
			null,
			"Substantial wardrobe for garments, furs, and domestic storage."
		);

		CreateItem(
			"medieval_irish_gaelic_hide_bound_oak_chest",
			"chest",
			"a hide-bound oak chest",
			null,
			"This hide-bound oak chest is a large, workmanlike chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			86.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Rugged hide-bound domestic chest for textiles and household stores."
		);

		CreateItem(
			"medieval_magyar_bronze_mounted_travel_chest",
			"chest",
			"a bronze-mounted travel chest",
			null,
			"This bronze-mounted travel chest is a large, well-made chest built from oak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			22000.0,
			125.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Metal-fitted chest suited to mobile households and travel goods."
		);

		CreateItem(
			"medieval_norman_arched_oak_armoire",
			"armoire",
			"an arched oak armoire",
			null,
			"This arched oak armoire is a large, well-made armoire built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			30000.0,
			140.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Armoire"
			],
			null,
			null,
			null,
			null,
			"Arched armoire for garments, fine textiles, and household chests."
		);

		CreateItem(
			"medieval_norse_viking_age_ship_carved_clothes_chest",
			"chest",
			"a ship-carved clothes chest",
			null,
			"This ship-carved clothes chest is a large, well-made chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			21000.0,
			110.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Regional carved storage chest for clothing, blankets, and domestic valuables."
		);

		CreateItem(
			"medieval_north_indian_rajput_teak_clothes_chest",
			"chest",
			"a teak clothes chest",
			null,
			"This teak clothes chest is a large, well-made chest built from teak boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			19000.0,
			120.0m,
			true,
			false,
			"teak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Hardwood chest for cotton garments, fine cloth, and domestic valuables."
		);

		CreateItem(
			"medieval_rus_novgorod_pine_winter_storage_chest",
			"chest",
			"a pine winter storage chest",
			null,
			"This pine winter storage chest is a large, workmanlike chest built from pine boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			20000.0,
			70.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Stout chest for winter cloth, furs, bedding, and household goods."
		);

		CreateItem(
			"medieval_scottish_gaelic_lowland_elm_winter_blanket_box",
			"box",
			"an elm winter blanket box",
			null,
			"This elm winter blanket box is a large, workmanlike box built from elm boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			20000.0,
			76.0m,
			true,
			false,
			"elm",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Blanket_Box"
			],
			null,
			null,
			null,
			null,
			"Cold-climate blanket box for heavy bedding and outerwear."
		);

		CreateItem(
			"medieval_seljuk_ayyubid_mamluk_tooled_chest_for_wares",
			"chest",
			"a tooled household chest",
			null,
			"This tooled household chest is a large, well-made chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			19000.0,
			125.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Decorated chest for robes, linens, and valuable domestic goods."
		);

		CreateItem(
			"medieval_song_china_bamboo_household_shelves",
			"shelves",
			"a bamboo household shelf",
			null,
			"These bamboo household shelf are large, workmanlike shelves built from split bamboo. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			5200.0,
			42.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
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
			"Light shelving for bowls, baskets, paper goods, and domestic objects."
		);

		CreateItem(
			"medieval_song_china_cypress_scroll_cabinet",
			"cabinet",
			"a cypress scroll cabinet",
			null,
			"This cypress scroll cabinet is a large, well-made cabinet built from cypress boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			13000.0,
			130.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Large_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Fine cabinet for scrolls, papers, brushes, and household records."
		);

		CreateItem(
			"medieval_south_indian_chola_bamboo_laundry_hamper",
			"hamper",
			"a bamboo laundry hamper",
			null,
			"This bamboo laundry hamper is a large, workmanlike hamper built from split bamboo. A woven body rises from a flat base to a firm rim, with the weave tightening at the corners. The open top makes the contents easy to see. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			1600.0,
			18.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Light warm-climate hamper for laundry, cloth, and domestic soft goods."
		);

		CreateItem(
			"medieval_south_indian_chola_sandalwood_jewel_cabinet",
			"cabinet",
			"a sandalwood jewel cabinet",
			null,
			"This sandalwood jewel cabinet is a medium-sized, well-made cabinet built from sandalwood. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7200.0,
			150.0m,
			true,
			false,
			"sandalwood",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Fragrant cabinet for jewels, scent, dressing goods, and small valuables."
		);

		CreateItem(
			"medieval_steppe_turkic_mongol_felt_lined_travel_coffer",
			"coffer",
			"a felt-lined travel coffer",
			null,
			"This felt-lined travel coffer is a large, well-made coffer made from pressed felt. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The weave shows at the hems, seams, and folded edges, giving it a used household character.",
			SizeCategory.Large,
			ItemQuality.Good,
			9000.0,
			76.0m,
			true,
			false,
			"felt",
			[
				"Functions / Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Soft-lined coffer form for mobile household goods and travel textiles."
		);
	}
}
