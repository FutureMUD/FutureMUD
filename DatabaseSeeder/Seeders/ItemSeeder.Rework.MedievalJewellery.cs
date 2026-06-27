#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalJewelleryAndDevotionalGoods()
	{
		CreateItem(
			"medieval_locking_religious_alms_chest",
			"chest",
			"a heavy alms chest",
			null,
			"This heavy alms chest is a large, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			34000.0,
			280.0m,
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
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Large locking chest for alms, gifts, and institutional funds."
		);

		CreateItem(
			"medieval_locking_religious_donation_box",
			"box",
			"a locking donation box",
			null,
			"This locking donation box is a small, workmanlike box built from oak boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			4200.0,
			80.0m,
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
			"Built-in-lock donation box for coins or offerings."
		);

		CreateItem(
			"medieval_locking_religious_reliquary_lockbox",
			"lockbox",
			"a locked reliquary box",
			null,
			"This locked reliquary box is a small, well-made lockbox built from oak boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			3200.0,
			150.0m,
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
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small built-in-lock box for relics, seals, or sacred keepsakes as stored contents."
		);

		CreateItem(
			"medieval_locking_religious_scripture_chest",
			"chest",
			"a locking scripture chest",
			null,
			"This locking scripture chest is a large, well-made chest built from cedar boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			28000.0,
			260.0m,
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
			"Locking chest for books, scrolls, tablets, or wrapped texts."
		);

		CreateItem(
			"medieval_locking_religious_treasury_chest",
			"chest",
			"a sanctuary treasury chest",
			null,
			"This sanctuary treasury chest is a large, well-made chest worked from wrought iron. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Large,
			ItemQuality.Good,
			68000.0,
			720.0m,
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
			"Heavy safe-chest profile for a religious treasury or secure sacristy room."
		);

		CreateItem(
			"medieval_religious_angled_scripture_lectern",
			"lectern",
			"an angled scripture lectern",
			null,
			"This angled scripture lectern is a medium-sized, well-made lectern built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8800.0,
			55.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Writing Materials / Document Containers"
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
			"Sloped reading surface for large sacred books, scrolls, tablets, or sermon notes."
		);

		CreateItem(
			"medieval_religious_bronze_incense_burner",
			"burner",
			"a bronze incense burner",
			null,
			"This bronze incense burner is a small, workmanlike burner worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1600.0,
			32.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
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
			"Small solid-fuel burner for charcoal and incense; not a dry container while using brazier behaviour."
		);

		CreateItem(
			"medieval_religious_bronze_offering_tray",
			"tray",
			"a bronze offering tray",
			null,
			"This bronze offering tray is a medium-sized, workmanlike tray worked from bronze. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1700.0,
			36.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
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
			"Flat tray for ritual food portions, flowers, incense packets, tapers, or carried offerings."
		);

		CreateItem(
			"medieval_religious_glass_relic_case",
			"case",
			"a glass relic case",
			null,
			"This glass relic case is a medium-sized, well-made case made from glass. A small framed compartment sits behind a guarded front, with the edges finished more carefully than the back. The base is steady enough for display. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			90.0m,
			true,
			false,
			"glass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Wares"
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
			"Transparent display case for reliquaries, memorial objects, sealed offerings, or other protected sacred pieces."
		);

		CreateItem(
			"medieval_religious_hanging_censer",
			"censer",
			"a hanging bronze censer",
			null,
			"This hanging bronze censer is a small, well-made censer worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1250.0,
			48.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
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
			"Suspended censer for incense smoke, modelled as a small solid-fuel burner rather than as jewellery or a personal charm."
		);

		CreateItem(
			"medieval_religious_hanging_oil_lamp",
			"lamp",
			"a hanging oil lamp",
			null,
			"This hanging oil lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1150.0,
			42.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
			"Suspended liquid-fuel lamp for a sanctuary, shrine, or religious hall."
		);

		CreateItem(
			"medieval_religious_incense_box",
			"box",
			"a cedar incense box",
			null,
			"This cedar incense box is a small, workmanlike box built from cedar boards. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1200.0,
			24.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
			"Lidded dry container for incense chips, resins, powdered aromatics, or fragrant woods."
		);

		CreateItem(
			"medieval_religious_incense_cabinet",
			"cabinet",
			"an incense-store cabinet",
			null,
			"This incense-store cabinet is a medium-sized, well-made cabinet built from cedar boards. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7600.0,
			60.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
			"Small cabinet for incense, tapers, ritual oil flasks, and other aromatic service supplies."
		);

		CreateItem(
			"medieval_religious_lamp_tray",
			"tray",
			"a bronze lamp tray",
			null,
			"This bronze lamp tray is a medium-sized, workmanlike tray worked from bronze. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Its formal proportions give it a public, ritual, and institutional presence. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2100.0,
			30.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
			"Tray for arranging oil lamps, candle cups, wicks, and lighting supplies; inert unless separate lamps are placed on it."
		);

		CreateItem(
			"medieval_religious_long_altar_candle",
			"candle",
			"a long altar candle",
			null,
			"This long altar candle is a small, well-made candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			260.0,
			8.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
			"Longer beeswax candle suited to altar, shrine, memorial, or festival lighting."
		);

		CreateItem(
			"medieval_religious_long_congregation_bench",
			"bench",
			"a long congregation bench",
			null,
			"This long congregation bench is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			24000.0,
			60.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
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
			"Long religious-hall bench with ordinary bench seating and a surface that can accept small placed objects."
		);

		CreateItem(
			"medieval_religious_low_offering_stand",
			"stand",
			"a low offering stand",
			null,
			"This low offering stand is a medium-sized, workmanlike stand built from beech boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6200.0,
			24.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Temple Offerings"
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
			"Low surface for offerings, lamps, incense bowls, small icons, tablets, or ritual vessels."
		);

		CreateItem(
			"medieval_religious_lustral_water_bucket",
			"bucket",
			"a lustral water bucket",
			null,
			"This lustral water bucket is a medium-sized, well-made bucket worked from bronze. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2800.0,
			52.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Handled liquid vessel for sprinkled, poured, or distributed sacred water."
		);

		CreateItem(
			"medieval_religious_oak_offering_table",
			"table",
			"an oak offering table",
			null,
			"This oak offering table is a large, workmanlike table built from oak boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			70.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Temple Offerings"
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
			"General offering or service table for religious halls, chapels, shrines, and household ritual rooms."
		);

		CreateItem(
			"medieval_religious_offering_bowl",
			"bowl",
			"a bronze offering bowl",
			null,
			"This bronze offering bowl is a small, workmanlike bowl worked from bronze. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			950.0,
			28.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Open bowl for grain, flowers, ashes, small offerings, or other solid ritual goods."
		);

		CreateItem(
			"medieval_religious_oil_cruet",
			"cruet",
			"a small ritual oil cruet",
			null,
			"This small ritual oil cruet is a very small, well-made cruet made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			220.0,
			28.0m,
			true,
			false,
			"glass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Flask"
			],
			null,
			null,
			null,
			null,
			"Small liquid container for lamp oil, chrism-like oil, perfumed oil, or anointing liquid."
		);

		CreateItem(
			"medieval_religious_open_devotional_shelves",
			"shelves",
			"open devotional shelves",
			null,
			"These open devotional shelves are large, workmanlike shelves built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			14500.0,
			48.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
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
			"Open shelves for religious books, lamps, offering bowls, memorial tablets, or displayed sacred objects."
		);

		CreateItem(
			"medieval_religious_painted_screen_panel",
			"screen",
			"a painted shrine screen",
			null,
			"This painted shrine screen is a large, workmanlike screen built from pine boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12000.0,
			45.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
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
			"Portable screen panel for marking off a shrine, altar side, vestry corner, or sacred display area."
		);

		CreateItem(
			"medieval_religious_plain_oil_lamp",
			"lamp",
			"a plain ritual oil lamp",
			null,
			"This plain ritual oil lamp is a small, workmanlike lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			850.0,
			26.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
			"Liquid-fuel lamp for altars, shrines, chapels, and religious household rooms."
		);

		CreateItem(
			"medieval_religious_plain_prayer_bench",
			"bench",
			"a plain prayer bench",
			null,
			"This plain prayer bench is a medium-sized, workmanlike bench built from pine boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5500.0,
			18.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture",
				"Market / Religious Goods / Devotional Goods"
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
			"Compact kneeling or seated prayer bench; a furnishing rather than a personal devotional object."
		);

		CreateItem(
			"medieval_religious_ritual_supply_cupboard",
			"cupboard",
			"a ritual supply cupboard",
			null,
			"This ritual supply cupboard is a large, workmanlike cupboard built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18500.0,
			55.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
			"General cupboard for candles, cloths, incense, oil flasks, basins, and other communal religious supplies."
		);

		CreateItem(
			"medieval_religious_ritual_washing_basin",
			"basin",
			"a ritual washing basin",
			null,
			"This ritual washing basin is a medium-sized, workmanlike basin formed from ceramic. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6200.0,
			26.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Urna"
			],
			null,
			null,
			null,
			null,
			"Water-holding basin for ablution, lustration, or ceremonial washing."
		);

		CreateItem(
			"medieval_religious_scripture_cabinet",
			"cabinet",
			"a scripture cabinet",
			null,
			"This scripture cabinet is a large, well-made cabinet built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			22000.0,
			85.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
			"Cabinet for sacred books, scroll cases, tablets, commentaries, or ritual documents."
		);

		CreateItem(
			"medieval_religious_simple_reading_stand",
			"stand",
			"a simple reading stand",
			null,
			"This simple reading stand is a medium-sized, workmanlike stand built from pine boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3400.0,
			15.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Simple Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Writing Materials / Document Containers"
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
			"Portable sloped stand for a book, scroll, scripture bundle, or ritual text."
		);

		CreateItem(
			"medieval_religious_standing_censer",
			"censer",
			"a standing iron censer",
			null,
			"This standing iron censer is a medium-sized, workmanlike censer worked from wrought iron. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5200.0,
			36.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
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
			"Freestanding incense burner for shrines, halls, porches, or side chapels."
		);

		CreateItem(
			"medieval_religious_stone_offering_basin",
			"basin",
			"a stone offering basin",
			null,
			"This stone offering basin is a medium-sized, workmanlike basin cut from limestone. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			9800.0,
			35.0m,
			true,
			false,
			"limestone",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Broad solid-offering basin for dry offerings, flower heads, grains, salt, ash, or votive tokens."
		);

		CreateItem(
			"medieval_religious_votive_candle",
			"candle",
			"a beeswax votive candle",
			null,
			"This beeswax votive candle is a very small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			80.0,
			2.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
			"Small timed candle for votive, memorial, or offering use."
		);

		CreateItem(
			"medieval_religious_wall_devotional_panel",
			"panel",
			"a wall devotional panel",
			null,
			"This wall devotional panel is a medium-sized, workmanlike panel built from pine boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3200.0,
			22.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
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
			"Wall-hung devotional panel suitable for painted, carved, inscribed, or skinned religious imagery."
		);

		CreateItem(
			"medieval_religious_wall_shrine_shelf",
			"shelf",
			"a wall shrine shelf",
			null,
			"This wall shrine shelf is a medium-sized, well-made shelf built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2300.0,
			34.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
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
			"Narrow wall shelf intended to hold lamps, offerings, icons, tablets, or small religious vessels."
		);

		CreateItem(
			"medieval_religious_water_ewer",
			"ewer",
			"a ritual water ewer",
			null,
			"This ritual water ewer is a small, workmanlike ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1400.0,
			34.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Decanter"
			],
			null,
			null,
			null,
			null,
			"Pouring vessel for water, diluted wine, or lustral liquid in a religious setting."
		);

		CreateItem(
			"medieval_church_altar_table",
			"altar",
			"an oak church altar",
			null,
			"This oak church altar is a large, well-made altar built from oak boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			36000.0,
			160.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Luxury Furniture",
				"Market / Religious Goods / Devotional Goods"
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
			"Substantial altar table with surface behaviour for chalices, books, candles, or offerings."
		);

		CreateItem(
			"medieval_church_aspersory_bucket",
			"bucket",
			"a bronze aspersory bucket",
			null,
			"This bronze aspersory bucket is a medium-sized, well-made bucket worked from bronze. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2600.0,
			60.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Portable holy-water vessel for sprinkling rites; no personal-wear or jewellery behaviour."
		);

		CreateItem(
			"medieval_church_baptismal_ewer",
			"ewer",
			"a brass baptismal ewer",
			null,
			"This brass baptismal ewer is a small, well-made ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			44.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Decanter"
			],
			null,
			null,
			null,
			null,
			"Pouring vessel for baptismal or washing water."
		);

		CreateItem(
			"medieval_church_bronze_thurible",
			"thurible",
			"a bronze thurible",
			null,
			"This bronze thurible is a small, well-made thurible worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1100.0,
			65.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
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
			"Swinging incense censer represented as a small solid-fuel burner, not as a wearable or jewellery item."
		);

		CreateItem(
			"medieval_church_candle_pricket_stand",
			"stand",
			"a candle-pricket stand",
			null,
			"This candle-pricket stand is a medium-sized, workmanlike stand worked from wrought iron. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5600.0,
			42.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
			"Iron candle stand surface for setting tapers or votive lights; separate candles provide actual light."
		);

		CreateItem(
			"medieval_church_choir_stall",
			"stall",
			"a carved choir stall",
			null,
			"This carved choir stall is a large, well-made stall built from oak boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			120.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture",
				"Market / Religious Goods / Devotional Goods"
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
			"Individual choir or clerical seat with carved timberwork; not an outfit or personal effect."
		);

		CreateItem(
			"medieval_church_eagle_lectern",
			"lectern",
			"a brass eagle lectern",
			null,
			"This brass eagle lectern is a large, well-made lectern worked from brass. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Large,
			ItemQuality.Good,
			22000.0,
			210.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Luxury Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Recognisable church lectern form with a broad reading surface; the bird detail is decorative only."
		);

		CreateItem(
			"medieval_church_funeral_bier",
			"bier",
			"a plain funeral bier",
			null,
			"This plain funeral bier is a large, workmanlike bier built from oak boards. A long flat platform rests on carrying rails, with low side edges and a plain upper surface. The handles are smoothed where bearers grip them. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			16000.0,
			54.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Funerary Goods",
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
			"Church bier or trestled surface for funerary display and procession preparation."
		);

		CreateItem(
			"medieval_church_gospel_book_stand",
			"stand",
			"a gospel book stand",
			null,
			"This gospel book stand is a medium-sized, well-made stand built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			4200.0,
			55.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Writing Materials / Document Containers"
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
			"Portable stand for a large service book or gospel volume."
		);

		CreateItem(
			"medieval_church_holy_water_stoup",
			"stoup",
			"a holy-water stoup",
			null,
			"This holy-water stoup is a medium-sized, well-made stoup cut from marble. A cupped basin sits on a sturdy bracket, with a thick rim around the water hollow. The inner surface is polished from repeated touch. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8600.0,
			90.0m,
			true,
			false,
			"marble",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LContainer_Amphora_Sextarius"
			],
			null,
			null,
			null,
			null,
			"Small stone holy-water vessel for a church entrance or chapel wall."
		);

		CreateItem(
			"medieval_church_incense_boat",
			"box",
			"an incense boat box",
			null,
			"This incense boat box is a very small, well-made box worked from brass. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			420.0,
			36.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
			"Small lidded container for incense grains or aromatic resin beside a thurible."
		);

		CreateItem(
			"medieval_church_kneeling_bench",
			"kneeler",
			"a low kneeling bench",
			null,
			"This low kneeling bench is a medium-sized, workmanlike kneeler built from pine boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			18.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture",
				"Market / Religious Goods / Devotional Goods"
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
			"Low church kneeler or prayer bench for posture and room furnishing."
		);

		CreateItem(
			"medieval_church_oak_pew",
			"pew",
			"an oak church pew",
			null,
			"This oak church pew is a large, workmanlike pew built from oak boards. A long seat runs between upright ends, with a straight back board and a narrow ledge along the rear. The front edge is polished by use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			30000.0,
			85.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
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
			"Long church seating form, using bench mechanics and a narrow surface for small placed objects."
		);

		CreateItem(
			"medieval_church_pulpit",
			"pulpit",
			"a carved wooden pulpit",
			null,
			"This carved wooden pulpit is a large, well-made pulpit built from oak boards. A raised speaking enclosure stands above a small base, with a front panel and a narrow ledge for a book. The upper rail is polished by hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			32000.0,
			150.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Luxury Furniture",
				"Market / Religious Goods / Devotional Goods"
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
			"Raised preaching or reading furnishing with a sloped surface for notes or scripture."
		);

		CreateItem(
			"medieval_church_pyx_cupboard",
			"cupboard",
			"a pyx cupboard",
			null,
			"This pyx cupboard is a medium-sized, well-made cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7600.0,
			80.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
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
			"Small church cupboard for pyxes, small vessels, folded cloths, and service goods."
		);

		CreateItem(
			"medieval_church_reliquary_display_case",
			"case",
			"a reliquary display case",
			null,
			"This reliquary display case is a medium-sized, well-made case made from glass. A small framed compartment sits behind a guarded front, with the edges finished more carefully than the back. The base is steady enough for display. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3200.0,
			120.0m,
			true,
			false,
			"glass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Wares"
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
			"Transparent church display case for relic containers or visually important sacred objects."
		);

		CreateItem(
			"medieval_church_rood_screen_panel",
			"screen",
			"a carved rood-screen panel",
			null,
			"This carved rood-screen panel is a large, well-made screen built from oak boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			18000.0,
			130.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Decorative dividing screen panel for a church interior; inert unless a later pass gives it doorway behaviour."
		);

		CreateItem(
			"medieval_church_sacristy_cupboard",
			"cupboard",
			"a sacristy cupboard",
			null,
			"This sacristy cupboard is a large, well-made cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			95.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
			"Large service cupboard for candles, incense, linens, books, vessels, and other sacristy stores."
		);

		CreateItem(
			"medieval_church_sanctuary_lamp",
			"lamp",
			"a hanging sanctuary lamp",
			null,
			"This hanging sanctuary lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1400.0,
			70.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
			"Hanging liquid-fuel sanctuary lamp suited to a church or chapel."
		);

		CreateItem(
			"medieval_church_stone_baptismal_font",
			"font",
			"a stone baptismal font",
			null,
			"This stone baptismal font is a large, workmanlike font cut from limestone. A cupped basin sits on a sturdy bracket, with a thick rim around the water hollow. The inner surface is polished from repeated touch. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Large,
			ItemQuality.Standard,
			58000.0,
			110.0m,
			true,
			false,
			"limestone",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LContainer_Drum"
			],
			null,
			null,
			null,
			null,
			"Large water-holding font for baptisms or ritual washing; liquid-container behaviour only."
		);

		CreateItem(
			"medieval_church_wall_crucifix",
			"crucifix",
			"a wall-hung crucifix",
			null,
			"This wall-hung crucifix is a medium-sized, workmanlike crucifix built from oak boards. A carved cross shape carries a raised central figure, with small fixing marks on the back. The front is smoothed around the most handled edges. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			36.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
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
			"Wall-hung cruciform furnishing for churches, chapels, or domestic devotional rooms."
		);

		CreateItem(
			"medieval_locking_church_offertory_chest",
			"chest",
			"a locked offertory chest",
			null,
			"This locked offertory chest is a large, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			32000.0,
			300.0m,
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
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking offertory chest for coin, gifts, or parish stores."
		);

		CreateItem(
			"medieval_locking_church_reliquary_casket",
			"casket",
			"a locked reliquary casket",
			null,
			"This locked reliquary casket is a small, well-made casket worked from bronze. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			4200.0,
			220.0m,
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
			"Small lockable casket for church relic storage and display-adjacent use."
		);

		CreateItem(
			"medieval_locking_church_sacristy_strongbox",
			"strongbox",
			"a sacristy strongbox",
			null,
			"This sacristy strongbox is a medium-sized, well-made strongbox built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			18000.0,
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
			"Locking strongbox for a sacristy, vestry, or church office."
		);

		CreateItem(
			"medieval_eastern_christian_analogion_lectern",
			"lectern",
			"an angled analogion lectern",
			null,
			"This angled analogion lectern is a medium-sized, well-made lectern built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7600.0,
			82.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
			"Sloped lectern for service books or gospel texts."
		);

		CreateItem(
			"medieval_eastern_christian_baptismal_basin",
			"basin",
			"a bronze baptismal basin",
			null,
			"This bronze baptismal basin is a medium-sized, well-made basin worked from bronze. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			5200.0,
			92.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Drum"
			],
			null,
			null,
			null,
			null,
			"Water-holding baptismal or washing basin for church use."
		);

		CreateItem(
			"medieval_eastern_christian_candle_sand_tray",
			"tray",
			"a candle-sand tray",
			null,
			"This candle-sand tray is a medium-sized, workmanlike tray worked from brass. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Its formal proportions give it a public, ritual, and institutional presence. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3300.0,
			38.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
			"Tray for sand, tapers, or votive candles; actual light comes from separate candle items."
		);

		CreateItem(
			"medieval_eastern_christian_chain_censer",
			"censer",
			"a chain-hung censer",
			null,
			"This chain-hung censer is a small, well-made censer worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1250.0,
			58.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
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
			"Suspended church censer for charcoal and incense."
		);

		CreateItem(
			"medieval_eastern_christian_chanting_bench",
			"bench",
			"a chanting bench",
			null,
			"This chanting bench is a large, workmanlike bench built from beech boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			52.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
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
			"Two-place bench for singers or readers in a church interior."
		);

		CreateItem(
			"medieval_eastern_christian_chrism_cruet",
			"cruet",
			"a glass chrism cruet",
			null,
			"This glass chrism cruet is a very small, well-made cruet made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			180.0,
			30.0m,
			true,
			false,
			"glass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Flask"
			],
			null,
			null,
			null,
			null,
			"Small oil container for chrism-like or perfumed ritual oil."
		);

		CreateItem(
			"medieval_eastern_christian_gospel_lectern",
			"lectern",
			"a gospel lectern",
			null,
			"This gospel lectern is a medium-sized, well-made lectern built from cypress boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			6800.0,
			70.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
			"Reading surface for large sacred books and liturgical texts."
		);

		CreateItem(
			"medieval_eastern_christian_hanging_lamp",
			"lamp",
			"a hanging icon lamp",
			null,
			"This hanging icon lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1150.0,
			62.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
			"Suspended liquid-fuel lamp suited to an icon or sanctuary area."
		);

		CreateItem(
			"medieval_eastern_christian_icon_stand",
			"stand",
			"a carved icon stand",
			null,
			"This carved icon stand is a medium-sized, well-made stand built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			5600.0,
			70.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture"
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
			"Open display stand for one or more wall-sized icons or devotional panels."
		);

		CreateItem(
			"medieval_eastern_christian_iconostasis_panel",
			"panel",
			"an iconostasis panel",
			null,
			"This iconostasis panel is a large, well-made panel built from cypress boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			17000.0,
			125.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Decorative screen panel used to define a sacred area, with skins able to supply exact painted imagery."
		);

		CreateItem(
			"medieval_eastern_christian_incense_casket",
			"casket",
			"an incense casket",
			null,
			"This incense casket is a small, well-made casket built from cedar boards. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1100.0,
			34.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
			"Small dry container for incense, aromatic wood, resin, or grains."
		);

		CreateItem(
			"medieval_eastern_christian_prothesis_table",
			"table",
			"a prothesis service table",
			null,
			"This prothesis service table is a large, well-made table built from cypress boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			21000.0,
			120.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
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
			"Side service table with surface behaviour for vessels, cloths, bread, or ritual equipment."
		);

		CreateItem(
			"medieval_eastern_christian_reliquary_icon_case",
			"case",
			"a reliquary icon case",
			null,
			"This reliquary icon case is a medium-sized, well-made case made from glass. A small framed compartment sits behind a guarded front, with the edges finished more carefully than the back. The base is steady enough for display. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3400.0,
			115.0m,
			true,
			false,
			"glass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Wares"
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
			"Display case for an icon panel, relic container, or sealed devotional object."
		);

		CreateItem(
			"medieval_eastern_christian_tetrapod_table",
			"table",
			"a tetrapod offering table",
			null,
			"This tetrapod offering table is a large, well-made table built from walnut boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			19000.0,
			110.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
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
			"Four-legged service table for icons, books, candles, and offerings."
		);

		CreateItem(
			"medieval_eastern_christian_vigil_lamp",
			"lamp",
			"a bronze vigil lamp",
			null,
			"This bronze vigil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1050.0,
			54.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
			"Small liquid-fuel vigil lamp for an icon stand, sanctuary corner, or shrine shelf."
		);

		CreateItem(
			"medieval_eastern_christian_wall_icon_panel",
			"panel",
			"a wall icon panel",
			null,
			"This wall icon panel is a medium-sized, workmanlike panel built from cypress boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2600.0,
			34.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
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
			"Wall devotional panel for sacred imagery; not a personal charm or jewellery piece."
		);

		CreateItem(
			"medieval_islamic_ablution_basin",
			"basin",
			"a ceramic ablution basin",
			null,
			"This ceramic ablution basin is a medium-sized, workmanlike basin formed from ceramic. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6200.0,
			30.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Drum"
			],
			null,
			null,
			null,
			null,
			"Water-holding basin for ritual washing before prayer."
		);

		CreateItem(
			"medieval_islamic_ablution_ewer",
			"ewer",
			"a brass ablution ewer",
			null,
			"This brass ablution ewer is a small, workmanlike ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1500.0,
			32.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Pouring vessel for ablution water."
		);

		CreateItem(
			"medieval_islamic_brass_hanging_lamp",
			"lamp",
			"a brass mosque lamp",
			null,
			"This brass mosque lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			60.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
			"Metal liquid-fuel lamp for a mosque, madrasa, or prayer hall."
		);

		CreateItem(
			"medieval_islamic_folding_book_stand",
			"stand",
			"a folding book stand",
			null,
			"This folding book stand is a small, well-made stand built from boxwood. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1200.0,
			36.0m,
			true,
			false,
			"boxwood",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Wares",
				"Market / Writing Materials / Document Containers"
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
			"Low sloped stand for scripture reading or study; furnishing rather than personal jewellery."
		);

		CreateItem(
			"medieval_islamic_incense_burner",
			"burner",
			"a brass incense burner",
			null,
			"This brass incense burner is a small, well-made burner worked from brass. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			44.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
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
			"Small burner for fragrant wood or incense in a religious or ceremonial hall."
		);

		CreateItem(
			"medieval_islamic_madrasa_text_shelves",
			"shelves",
			"madrasa text shelves",
			null,
			"These madrasa text shelves are large, well-made shelves built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			17000.0,
			78.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
			"Open shelves for religious study texts, legal works, and manuscript bundles."
		);

		CreateItem(
			"medieval_islamic_mihrab_panel",
			"panel",
			"a carved mihrab panel",
			null,
			"This carved mihrab panel is a large, well-made panel built from cypress boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			14000.0,
			115.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
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
			"Decorative niche-like direction panel for a mosque or prayer hall."
		);

		CreateItem(
			"medieval_islamic_mosque_glass_lamp",
			"lamp",
			"a glass mosque lamp",
			null,
			"This glass mosque lamp is a small, well-made lamp made from glass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			75.0m,
			true,
			false,
			"glass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
			"Hanging liquid-fuel mosque lamp with fragile glass construction."
		);

		CreateItem(
			"medieval_islamic_qibla_direction_panel",
			"panel",
			"a qibla direction panel",
			null,
			"This qibla direction panel is a medium-sized, workmanlike panel built from cedar boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			38.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
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
			"Wall panel marking prayer direction without using figurative imagery."
		);

		CreateItem(
			"medieval_islamic_quran_cabinet",
			"cabinet",
			"a Qur'an cabinet",
			null,
			"This Qur'an cabinet is a large, well-made cabinet built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			22000.0,
			95.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
			"Cabinet for bound or wrapped sacred texts, commentaries, and recitation books."
		);

		CreateItem(
			"medieval_islamic_reed_mat_rack",
			"rack",
			"a reed mat rack",
			null,
			"This reed mat rack is a large, workmanlike rack built from split bamboo. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			7000.0,
			28.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Religious Items",
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
			"Open rack for communal prayer mats without making the mats themselves personal gear."
		);

		CreateItem(
			"medieval_islamic_sandal_shelf",
			"shelf",
			"a mosque sandal shelf",
			null,
			"This mosque sandal shelf is a large, workmanlike shelf built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9600.0,
			30.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
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
			"Open shelf for footwear at a prayer hall entrance."
		);

		CreateItem(
			"medieval_islamic_water_storage_jar",
			"jar",
			"an ablution water jar",
			null,
			"This ablution water jar is a large, workmanlike jar formed from terracotta. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12000.0,
			24.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Urna"
			],
			null,
			null,
			null,
			null,
			"Large ceramic liquid vessel for stored washing water."
		);

		CreateItem(
			"medieval_islamic_wooden_minbar_steps",
			"minbar",
			"a carved wooden minbar",
			null,
			"This carved wooden minbar is a large, well-made minbar built from walnut boards. A raised speaking enclosure stands above a small base, with a front panel and a narrow ledge for a book. The upper rail is polished by hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			36000.0,
			180.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Stepped mosque pulpit furnishing; no seat, container, or door mechanics are asserted."
		);

		CreateItem(
			"medieval_locking_islamic_charity_lockbox",
			"box",
			"a charity lockbox",
			null,
			"This charity lockbox is a small, well-made box built from cedar boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			3600.0,
			120.0m,
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
			"Lockable charity box for mosque or charitable-house settings."
		);

		CreateItem(
			"medieval_locking_islamic_quran_chest",
			"chest",
			"a locked Qur'an chest",
			null,
			"This locked Qur'an chest is a large, well-made chest built from cedar boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			260.0m,
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
			"Locking chest for wrapped books and religious texts."
		);

		CreateItem(
			"medieval_jewish_bimah_table",
			"table",
			"a bimah reading table",
			null,
			"This bimah reading table is a large, well-made table built from oak boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			100.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
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
			"Central reading table surface for scrolls, books, and service objects."
		);

		CreateItem(
			"medieval_jewish_bronze_lampstand",
			"lampstand",
			"a bronze seven-branched lampstand",
			null,
			"This bronze seven-branched lampstand is a medium-sized, well-made lampstand worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			5200.0,
			110.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Recognisable ritual lampstand furnishing; actual light should come from separate candle or lamp items."
		);

		CreateItem(
			"medieval_jewish_genizah_chest",
			"chest",
			"a genizah chest",
			null,
			"This genizah chest is a large, workmanlike chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18500.0,
			65.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Funerary Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
			"Chest for worn religious papers, damaged texts, or ritually retired writing materials."
		);

		CreateItem(
			"medieval_jewish_memorial_candle",
			"candle",
			"a memorial beeswax candle",
			null,
			"This memorial beeswax candle is a small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			4.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
			"Long-burning candle for memorial or household religious observance."
		);

		CreateItem(
			"medieval_jewish_ner_tamid_lamp",
			"lamp",
			"a ner tamid lamp",
			null,
			"This ner tamid lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1200.0,
			70.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
			"Hanging perpetual-style liquid-fuel lamp for a synagogue furnishing set."
		);

		CreateItem(
			"medieval_jewish_reading_lectern",
			"lectern",
			"a synagogue reading lectern",
			null,
			"This synagogue reading lectern is a medium-sized, well-made lectern built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7200.0,
			68.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
			"Sloped reading surface for scrolls or books."
		);

		CreateItem(
			"medieval_jewish_scroll_cabinet",
			"cabinet",
			"a Torah scroll cabinet",
			null,
			"This Torah scroll cabinet is a large, well-made cabinet built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			120.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
			"Cabinet for scrolls, wrappings, and synagogue texts."
		);

		CreateItem(
			"medieval_jewish_scroll_rest",
			"rest",
			"a wooden scroll rest",
			null,
			"This wooden scroll rest is a small, well-made rest built from boxwood. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			30.0m,
			true,
			false,
			"boxwood",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Wares",
				"Market / Writing Materials / Document Containers"
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
			"Small support for resting a scroll or rolled text during reading."
		);

		CreateItem(
			"medieval_jewish_synagogue_bench",
			"bench",
			"a synagogue bench",
			null,
			"This synagogue bench is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			58.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
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
			"Long seating bench for a synagogue or teaching house."
		);

		CreateItem(
			"medieval_jewish_wall_tablet_panel",
			"panel",
			"a wall tablet panel",
			null,
			"This wall tablet panel is a medium-sized, workmanlike panel formed from stoneware. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3400.0,
			32.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			"Wall panel for inscribed or symbolic religious display, with exact writing handled by skins or writing blocks."
		);

		CreateItem(
			"medieval_jewish_washing_basin",
			"basin",
			"a ceramic washing basin",
			null,
			"This ceramic washing basin is a medium-sized, workmanlike basin formed from ceramic. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5600.0,
			28.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Drum"
			],
			null,
			null,
			null,
			null,
			"Liquid-holding basin for washing water."
		);

		CreateItem(
			"medieval_jewish_washing_ewer",
			"ewer",
			"a brass washing ewer",
			null,
			"This brass washing ewer is a small, workmanlike ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1400.0,
			34.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Decanter"
			],
			null,
			null,
			null,
			null,
			"Pouring vessel for handwashing or ritual washing."
		);

		CreateItem(
			"medieval_locking_jewish_torah_ark_chest",
			"ark",
			"a wooden Torah ark",
			null,
			"This wooden Torah ark is a large, well-made ark built from cedar boards. Small doors close the front of the case, with a raised threshold and careful trim around the opening. The interior is protected and formal. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			46000.0,
			520.0m,
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
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
			null,
			null,
			null,
			null,
			"Large built-in-lock container profile for protected scroll storage."
		);

		CreateItem(
			"medieval_locking_jewish_tzedakah_box",
			"box",
			"a tzedakah box",
			null,
			"This tzedakah box is a small, well-made box built from walnut boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			3600.0,
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
			"Small locking box for charity coin and offerings."
		);

		CreateItem(
			"medieval_hindu_abhisheka_basin",
			"basin",
			"a stone abhisheka basin",
			null,
			"This stone abhisheka basin is a medium-sized, well-made basin cut from soapstone. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8200.0,
			70.0m,
			true,
			false,
			"soapstone",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LContainer_Drum"
			],
			null,
			null,
			null,
			null,
			"Liquid-holding basin for ceremonial pouring and washing."
		);

		CreateItem(
			"medieval_hindu_brass_offering_tray",
			"tray",
			"a brass offering tray",
			null,
			"This brass offering tray is a medium-sized, well-made tray worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			45.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
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
			"Tray for flowers, lamps, powders, food offerings, or ritual vessels."
		);

		CreateItem(
			"medieval_hindu_bronze_oil_lamp",
			"lamp",
			"a bronze temple oil lamp",
			null,
			"This bronze temple oil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1000.0,
			52.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
			"Liquid-fuel ritual lamp suitable for temple or household shrine use."
		);

		CreateItem(
			"medieval_hindu_carved_shrine_screen",
			"screen",
			"a carved shrine screen",
			null,
			"This carved shrine screen is a large, well-made screen built from teak boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			16000.0,
			90.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
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
			"Decorative screen panel for a temple niche, household shrine, or ritual alcove."
		);

		CreateItem(
			"medieval_hindu_floor_offering_stand",
			"stand",
			"a low floor offering stand",
			null,
			"This low floor offering stand is a medium-sized, well-made stand built from sandalwood. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3800.0,
			58.0m,
			true,
			false,
			"sandalwood",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Temple Offerings",
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
			"Low floor stand for lamps, bowls, flower offerings, or small ritual goods."
		);

		CreateItem(
			"medieval_hindu_garland_rack",
			"rack",
			"a flower-garland rack",
			null,
			"This flower-garland rack is a medium-sized, workmanlike rack built from split bamboo. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2200.0,
			18.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Simple Wares"
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
			"Rack or shelf for garlands, flower strings, and offering preparation."
		);

		CreateItem(
			"medieval_hindu_household_shrine_cupboard",
			"cupboard",
			"a household shrine cupboard",
			null,
			"This household shrine cupboard is a medium-sized, well-made cupboard built from teak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			9500.0,
			95.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
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
			"Small shrine cupboard for lamps, vessels, images, flowers, powders, or wrapped offerings."
		);

		CreateItem(
			"medieval_hindu_image_pedestal",
			"pedestal",
			"a carved image pedestal",
			null,
			"This carved image pedestal is a large, well-made pedestal built from teak boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			18000.0,
			95.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Pedestal for a large sacred image or statue; the image itself is intentionally not specified."
		);

		CreateItem(
			"medieval_hindu_incense_burner",
			"burner",
			"a brass incense burner",
			null,
			"This brass incense burner is a small, well-made burner worked from brass. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			48.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
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
			"Solid-fuel burner for incense and fragrant woods."
		);

		CreateItem(
			"medieval_hindu_kalasha_pot",
			"pot",
			"a brass kalasha pot",
			null,
			"This brass kalasha pot is a small, well-made pot worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1100.0,
			44.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Ritual liquid vessel suitable for water, scented water, or festival use."
		);

		CreateItem(
			"medieval_hindu_multi_wick_lamp",
			"lamp",
			"a multi-wick brass lamp",
			null,
			"This multi-wick brass lamp is a medium-sized, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2600.0,
			85.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
			"Larger liquid-fuel lamp form for festival or altar lighting."
		);

		CreateItem(
			"medieval_hindu_puja_shelf",
			"shelf",
			"a carved puja shelf",
			null,
			"This carved puja shelf is a medium-sized, well-made shelf built from sandalwood. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2600.0,
			70.0m,
			true,
			false,
			"sandalwood",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
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
			"Wall shelf for lamps, offerings, images, flowers, or household shrine goods."
		);

		CreateItem(
			"medieval_hindu_puja_water_ewer",
			"ewer",
			"a puja water ewer",
			null,
			"This puja water ewer is a small, well-made ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1350.0,
			38.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Decanter"
			],
			null,
			null,
			null,
			null,
			"Pouring vessel for ritual water or scented liquid."
		);

		CreateItem(
			"medieval_hindu_stone_offering_bowl",
			"bowl",
			"a stone offering bowl",
			null,
			"This stone offering bowl is a small, workmanlike bowl cut from soapstone. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1200.0,
			24.0m,
			true,
			false,
			"soapstone",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Open bowl for rice, flowers, ash, pigment, or other solid offerings."
		);

		CreateItem(
			"medieval_hindu_temple_altar_table",
			"altar",
			"a carved temple altar",
			null,
			"This carved temple altar is a large, well-made altar built from teak boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			30000.0,
			140.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
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
			"Temple service table surface for lamps, offerings, vessels, and display objects."
		);

		CreateItem(
			"medieval_hindu_temple_supply_cupboard",
			"cupboard",
			"a temple supply cupboard",
			null,
			"This temple supply cupboard is a large, well-made cupboard built from teak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			25000.0,
			115.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
			"Large storage cupboard for lamps, trays, cloths, oils, incense, and vessels."
		);

		CreateItem(
			"medieval_locking_hindu_sandalwood_store_box",
			"box",
			"a sandalwood temple-store box",
			null,
			"This sandalwood temple-store box is a small, well-made box built from sandalwood. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			2400.0,
			170.0m,
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
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small lockable box for fragrant powders, offerings, or temple stores."
		);

		CreateItem(
			"medieval_locking_hindu_temple_donation_chest",
			"chest",
			"a locked temple donation chest",
			null,
			"This locked temple donation chest is a large, well-made chest built from teak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			36000.0,
			340.0m,
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
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Large locking donation chest for temple or shrine precincts."
		);

		CreateItem(
			"medieval_buddhist_altar_table",
			"altar",
			"a lacquered altar table",
			null,
			"This lacquered altar table is a large, well-made altar built from walnut boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			140.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
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
			"Altar table for lamps, incense, offerings, scrolls, and display objects."
		);

		CreateItem(
			"medieval_buddhist_incense_box",
			"box",
			"a sandalwood incense box",
			null,
			"This sandalwood incense box is a small, well-made box built from sandalwood. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			45.0m,
			true,
			false,
			"sandalwood",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
			"Dry container for incense sticks, powdered incense, or fragrant wood chips."
		);

		CreateItem(
			"medieval_buddhist_incense_burner",
			"burner",
			"a bronze incense burner",
			null,
			"This bronze incense burner is a small, well-made burner worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			46.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
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
			"Small burner for incense, charcoal, or fragrant wood."
		);

		CreateItem(
			"medieval_buddhist_lotus_oil_lamp",
			"lamp",
			"a lotus-shaped oil lamp",
			null,
			"This lotus-shaped oil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1150.0,
			58.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
			"Liquid-fuel lamp for Buddhist altar or shrine lighting."
		);

		CreateItem(
			"medieval_buddhist_meditation_bench",
			"bench",
			"a meditation bench",
			null,
			"This meditation bench is a medium-sized, workmanlike bench built from cypress boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3600.0,
			24.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
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
			"Low bench for seated or kneeling practice; a furnishing rather than an outfit or personal charm."
		);

		CreateItem(
			"medieval_buddhist_memorial_lamp",
			"lamp",
			"a memorial oil lamp",
			null,
			"This memorial oil lamp is a small, workmanlike lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			850.0,
			34.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
			"Small oil lamp for memorial, votive, or altar use."
		);

		CreateItem(
			"medieval_buddhist_memorial_tablet_shelf",
			"shelf",
			"a memorial tablet shelf",
			null,
			"This memorial tablet shelf is a medium-sized, well-made shelf built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			55.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture"
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
			"Open shelf for memorial tablets, small plaques, offerings, or lamps."
		);

		CreateItem(
			"medieval_buddhist_offering_bowl",
			"bowl",
			"a porcelain offering bowl",
			null,
			"This porcelain offering bowl is a small, well-made bowl formed from porcelain. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			620.0,
			32.0m,
			true,
			false,
			"porcelain",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Open bowl for rice, fruit, flowers, water cups represented as solid offerings, or other altar goods."
		);

		CreateItem(
			"medieval_buddhist_offering_tray",
			"tray",
			"a lacquered offering tray",
			null,
			"This lacquered offering tray is a medium-sized, well-made tray built from split bamboo. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			950.0,
			38.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Luxury Wares"
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
			"Flat tray for arranging flowers, incense, offerings, lamps, and ritual supplies."
		);

		CreateItem(
			"medieval_buddhist_sutra_cabinet",
			"cabinet",
			"a sutra cabinet",
			null,
			"This sutra cabinet is a large, well-made cabinet built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			22000.0,
			110.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
			"Cabinet for sutra scrolls, booklets, cases, and associated service materials."
		);

		CreateItem(
			"medieval_buddhist_sutra_lectern",
			"lectern",
			"a sutra reading lectern",
			null,
			"This sutra reading lectern is a medium-sized, workmanlike lectern built from split bamboo. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3600.0,
			30.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
			"Reading surface for sutras, commentaries, or ritual recitations."
		);

		CreateItem(
			"medieval_locking_buddhist_reliquary_box",
			"box",
			"a stupa reliquary box",
			null,
			"This stupa reliquary box is a small, well-made box worked from bronze. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			4200.0,
			220.0m,
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
			"Small bronze lockbox for reliquary or shrine storage."
		);

		CreateItem(
			"medieval_locking_buddhist_sutra_chest",
			"chest",
			"a locked sutra chest",
			null,
			"This locked sutra chest is a large, well-made chest built from cypress boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			280.0m,
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
			"Locking chest for wrapped sutras, papers, or monastery texts."
		);

		CreateItem(
			"medieval_daoist_altar_lamp",
			"lamp",
			"a Daoist altar lamp",
			null,
			"This Daoist altar lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			950.0,
			44.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
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
			"Liquid-fuel lamp for altar or shrine lighting."
		);

		CreateItem(
			"medieval_daoist_altar_table",
			"altar",
			"a Daoist altar table",
			null,
			"This Daoist altar table is a large, well-made altar built from cypress boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			115.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
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
			"Altar table for lamps, incense, tablets, offerings, and ritual documents."
		);

		CreateItem(
			"medieval_daoist_bronze_incense_urn",
			"urn",
			"a bronze incense urn",
			null,
			"This bronze incense urn is a medium-sized, well-made urn worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			4800.0,
			76.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
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
			"Freestanding incense urn represented as a solid-fuel burner."
		);

		CreateItem(
			"medieval_daoist_offering_tray",
			"tray",
			"a lacquered offering tray",
			null,
			"This lacquered offering tray is a medium-sized, workmanlike tray built from split bamboo. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			26.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
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
			"Tray for fruit, paper, incense, lamps, or dry ritual offerings."
		);

		CreateItem(
			"medieval_daoist_painted_altar_screen",
			"screen",
			"a painted altar screen",
			null,
			"This painted altar screen is a large, well-made screen built from cypress boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			12000.0,
			70.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
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
			"Painted screen panel for a shrine or altar backdrop."
		);

		CreateItem(
			"medieval_daoist_scripture_cabinet",
			"cabinet",
			"a Daoist scripture cabinet",
			null,
			"This Daoist scripture cabinet is a large, well-made cabinet built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			19000.0,
			85.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
			"Cabinet for scriptures, registers, talisman papers, and ritual manuscripts."
		);

		CreateItem(
			"medieval_daoist_tablet_shelf",
			"shelf",
			"a spirit-tablet shelf",
			null,
			"This spirit-tablet shelf is a medium-sized, well-made shelf built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3200.0,
			48.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture"
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
			"Open shelf for tablets, plaques, lamps, and offerings."
		);

		CreateItem(
			"medieval_daoist_talisman_paper_chest",
			"chest",
			"a talisman-paper chest",
			null,
			"This talisman-paper chest is a medium-sized, well-made chest built from cypress boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7600.0,
			60.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
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
			"Dry chest for paper talismans, ritual writing stock, scrolls, or folded documents."
		);

		CreateItem(
			"medieval_daoist_water_ewer",
			"ewer",
			"a ritual water ewer",
			null,
			"This ritual water ewer is a small, well-made ewer worked from bronze. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1200.0,
			40.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Decanter"
			],
			null,
			null,
			null,
			null,
			"Pouring vessel for ritual water or cleansing liquid."
		);

		CreateItem(
			"medieval_shinto_kamidana_shelf",
			"shelf",
			"a kamidana shrine shelf",
			null,
			"This kamidana shrine shelf is a medium-sized, well-made shelf built from cypress boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2200.0,
			50.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
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
			"Raised shrine shelf for offerings, small vessels, lamps, or plaques."
		);

		CreateItem(
			"medieval_shinto_offering_stand",
			"stand",
			"a whitewood offering stand",
			null,
			"This whitewood offering stand is a medium-sized, well-made stand built from cypress boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			45.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Temple Offerings",
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
			"Low offering stand for rice, salt, greenery, lamps, or vessels."
		);

		CreateItem(
			"medieval_shinto_purification_basin",
			"basin",
			"a stone purification basin",
			null,
			"This stone purification basin is a large, well-made basin cut from granite. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Large,
			ItemQuality.Good,
			65000.0,
			120.0m,
			true,
			false,
			"granite",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LContainer_Drum"
			],
			null,
			null,
			null,
			null,
			"Large liquid-holding basin for purification water."
		);

		CreateItem(
			"medieval_shinto_sake_vessel_stand",
			"stand",
			"a sake-vessel stand",
			null,
			"This sake-vessel stand is a small, workmanlike stand built from cypress boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1200.0,
			20.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Temple Offerings",
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
			"Small stand or tray for liquid vessels and shrine offerings; vessels remain separate items."
		);

		CreateItem(
			"medieval_shinto_shrine_donation_box",
			"box",
			"a slatted shrine donation box",
			null,
			"This slatted shrine donation box is a large, workmanlike box built from cypress boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			70.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
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
			"Open-lidded or slatted donation box for offerings and coins without built-in lock mechanics."
		);

		CreateItem(
			"medieval_shinto_shrine_screen_panel",
			"screen",
			"a pale shrine screen panel",
			null,
			"This pale shrine screen panel is a large, well-made screen built from cypress boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			11000.0,
			65.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
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
			"Plain pale timber screen panel for marking a shrine area or ritual backdrop."
		);

		CreateItem(
			"medieval_shinto_stone_shrine_lamp",
			"lamp",
			"a stone shrine lamp",
			null,
			"This stone shrine lamp is a medium-sized, well-made lamp cut from granite. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			12000.0,
			80.0m,
			true,
			false,
			"granite",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Stone-bodied liquid-fuel lamp for shrine approaches or altar areas."
		);

		CreateItem(
			"medieval_shinto_votive_tablet_rack",
			"rack",
			"a votive tablet rack",
			null,
			"This votive tablet rack is a large, workmanlike rack built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			36.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture"
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
			"Open display rack for hung or rested votive tablets."
		);

		CreateItem(
			"medieval_northern_ancestor_tablet_shelf",
			"shelf",
			"an ancestor tablet shelf",
			null,
			"This ancestor tablet shelf is a medium-sized, well-made shelf built from yew boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3200.0,
			45.0m,
			true,
			false,
			"yew",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture"
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
			"Shelf for ancestor plaques, small vessels, and memorial offerings."
		);

		CreateItem(
			"medieval_northern_blot_offering_bowl",
			"bowl",
			"a bronze offering bowl",
			null,
			"This bronze offering bowl is a small, workmanlike bowl worked from bronze. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			950.0,
			30.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Open bowl for grain, salt, bloodless offerings, ash, or symbolic ritual goods."
		);

		CreateItem(
			"medieval_northern_libation_bowl",
			"bowl",
			"a stone libation bowl",
			null,
			"This stone libation bowl is a medium-sized, workmanlike bowl cut from sandstone. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			7200.0,
			28.0m,
			true,
			false,
			"sandstone",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LContainer_Amphora_Sextarius"
			],
			null,
			null,
			null,
			null,
			"Liquid-holding bowl for libations, washing, or poured offerings."
		);

		CreateItem(
			"medieval_northern_sacred_post_panel",
			"post",
			"a carved sacred post",
			null,
			"This carved sacred post is a large, workmanlike post built from oak boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			15000.0,
			42.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
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
			"Large carved cult or ancestor post for a hall, grove edge, or household sacred corner."
		);

		CreateItem(
			"medieval_northern_shrine_bench",
			"bench",
			"a shrine-hall bench",
			null,
			"This shrine-hall bench is a large, workmanlike bench built from pine boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			19000.0,
			48.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
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
			"Simple two-place sacred-hall bench with ordinary bench behaviour."
		);
	}
}
