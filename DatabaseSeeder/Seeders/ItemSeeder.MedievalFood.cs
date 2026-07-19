#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalFoodAndBeverageItems()
	{
		CreateItem(
			"medieval_tableware_ale_jug",
			"jug",
			"a brown ale jug",
			null,
			"This brown ale jug is a small, workmanlike jug formed from stoneware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1300.0,
			12.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Jug sized for carrying ale to a table."
		);

		CreateItem(
			"medieval_tableware_ale_tray",
			"tray",
			"an ale-cup tray",
			null,
			"This ale-cup tray is a medium-sized, workmanlike tray built from oak boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1100.0,
			14.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Service tray for ale cups, tankards, or small mugs."
		);

		CreateItem(
			"medieval_tableware_almond_milk_bowl",
			"bowl",
			"an almond-milk bowl",
			null,
			"This almond-milk bowl is a small, workmanlike bowl formed from earthenware. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			6.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
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
			"Shallow bowl for thick prepared foods; not a liquid-storage vessel."
		);

		CreateItem(
			"medieval_tableware_ash_trencher",
			"trencher",
			"an ashwood trencher",
			null,
			"This ashwood trencher is a small, workmanlike trencher built from ash boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			500.0,
			6.0m,
			true,
			false,
			"ash",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Durable ash trencher for solid foods and dry table portions."
		);

		CreateItem(
			"medieval_tableware_beech_trencher",
			"trencher",
			"a beechwood trencher",
			null,
			"This beechwood trencher is a small, workmanlike trencher built from beech boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			480.0,
			6.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Plain reusable eating trencher suited to hall or tavern service."
		);

		CreateItem(
			"medieval_tableware_brass_beaker",
			"beaker",
			"a brass beaker",
			null,
			"This brass beaker is a very small, well-made beaker worked from brass. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			330.0,
			30.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Metal beaker for a prosperous household table."
		);

		CreateItem(
			"medieval_tableware_brass_drinking_cup",
			"cup",
			"a brass drinking cup",
			null,
			"This brass drinking cup is a very small, well-made cup worked from brass. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			300.0,
			28.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Bright metal cup for domestic or guest service."
		);

		CreateItem(
			"medieval_tableware_brass_ewer",
			"ewer",
			"a brass table ewer",
			null,
			"This brass table ewer is a small, well-made ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			44.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Bright ewer for water, wine, or wash service."
		);

		CreateItem(
			"medieval_tableware_brass_flagon",
			"flagon",
			"a brass table flagon",
			null,
			"This brass table flagon is a small, well-made flagon worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1500.0,
			45.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Bright flagon for drink service at a hall table."
		);

		CreateItem(
			"medieval_tableware_brass_goblet",
			"goblet",
			"a brass table goblet",
			null,
			"This brass table goblet is a small, well-made goblet worked from brass. A rounded bowl rises from a narrow stem and foot, leaving the rim clean and prominent. The base is weighted enough to stand firmly. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			500.0,
			32.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_WineGlass"
			],
			null,
			null,
			null,
			null,
			"Bright goblet for formal drink service."
		);

		CreateItem(
			"medieval_tableware_brass_hand_basin",
			"basin",
			"a brass hand basin",
			null,
			"This brass hand basin is a medium-sized, well-made basin worked from brass. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3000.0,
			48.0m,
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
				"LContainer_Amphora_Urna"
			],
			null,
			null,
			null,
			null,
			"Metal basin for handwashing before meals or chamber use."
		);

		CreateItem(
			"medieval_tableware_brass_oil_flask",
			"flask",
			"a brass oil flask",
			null,
			"This brass oil flask is a very small, well-made flask worked from brass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			300.0,
			24.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Flask"
			],
			null,
			null,
			null,
			null,
			"Metal flask for oil or other small-volume household liquid."
		);

		CreateItem(
			"medieval_tableware_brass_pitcher",
			"pitcher",
			"a brass water pitcher",
			null,
			"This brass water pitcher is a small, well-made pitcher worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1600.0,
			38.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Bright metal pitcher for guest service or washing water."
		);

		CreateItem(
			"medieval_tableware_brass_serving_platter",
			"platter",
			"a brass serving platter",
			null,
			"This brass serving platter is a medium-sized, well-made platter worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1700.0,
			38.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Polished brass platter for display and service."
		);

		CreateItem(
			"medieval_tableware_brass_tankard",
			"tankard",
			"a brass tankard",
			null,
			"This brass tankard is a small, well-made tankard worked from brass. A broad handle joins the side, and the open rim sits above a thickened base. The inside is smooth from repeated drink service. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			820.0,
			38.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_UKPint"
			],
			null,
			null,
			null,
			null,
			"Bright metal tankard for ale or mead."
		);

		CreateItem(
			"medieval_tableware_brass_water_bucket",
			"bucket",
			"a brass water bucket",
			null,
			"This brass water bucket is a medium-sized, well-made bucket worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3200.0,
			50.0m,
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
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Metal water vessel for chambers, kitchens, or guests."
		);

		CreateItem(
			"medieval_tableware_bread_platter",
			"platter",
			"a broad bread platter",
			null,
			"This broad bread platter is a medium-sized, workmanlike platter built from beech boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1000.0,
			12.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Broad platter for bread rounds, trenchers, and flat loaves."
		);

		CreateItem(
			"medieval_tableware_bread_trencher",
			"trencher",
			"a thick bread trencher",
			null,
			"This thick bread trencher is a small, workmanlike trencher made from firm baked bread. The centre is pressed shallow, while the rim is left thick and firm enough to hold food. The crust is dry and browned across the upper edge. The crust is firm and dry, with a shallow centre pressed down to take food.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			2.0m,
			true,
			false,
			"bread",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Edible trencher-like serving base; modelled as an open solid-food container for table service."
		);

		CreateItem(
			"medieval_tableware_breakfast_tray",
			"tray",
			"a low breakfast tray",
			null,
			"This low breakfast tray is a medium-sized, well-made tray built from cedar boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			900.0,
			24.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Low tray for carrying a small meal or chamber service."
		);

		CreateItem(
			"medieval_tableware_bronze_drinking_cup",
			"cup",
			"a bronze drinking cup",
			null,
			"This bronze drinking cup is a very small, well-made cup worked from bronze. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			300.0,
			30.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Polished metal cup for higher-status drink service."
		);

		CreateItem(
			"medieval_tableware_bronze_ewer",
			"ewer",
			"a bronze table ewer",
			null,
			"This bronze table ewer is a small, well-made ewer worked from bronze. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1350.0,
			48.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Fine metal ewer for table or chamber service."
		);

		CreateItem(
			"medieval_tableware_bronze_goblet",
			"goblet",
			"a bronze table goblet",
			null,
			"This bronze table goblet is a small, well-made goblet worked from bronze. A rounded bowl rises from a narrow stem and foot, leaving the rim clean and prominent. The base is weighted enough to stand firmly. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			520.0,
			34.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_WineGlass"
			],
			null,
			null,
			null,
			null,
			"Fine goblet using a wine-glass capacity profile."
		);

		CreateItem(
			"medieval_tableware_bronze_hand_basin",
			"basin",
			"a bronze hand basin",
			null,
			"This bronze hand basin is a medium-sized, well-made basin worked from bronze. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3300.0,
			52.0m,
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
				"LContainer_Amphora_Urna"
			],
			null,
			null,
			null,
			null,
			"Fine hand basin for formal washing service."
		);

		CreateItem(
			"medieval_tableware_bronze_pitcher",
			"pitcher",
			"a bronze water pitcher",
			null,
			"This bronze water pitcher is a small, well-made pitcher worked from bronze. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1700.0,
			42.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Metal pitcher for formal water service."
		);

		CreateItem(
			"medieval_tableware_bronze_serving_platter",
			"platter",
			"a bronze serving platter",
			null,
			"This bronze serving platter is a medium-sized, well-made platter worked from bronze. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1750.0,
			42.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Metal platter for formal hall service."
		);

		CreateItem(
			"medieval_tableware_butchered_meat_tray",
			"tray",
			"a butchered-meat tray",
			null,
			"This butchered-meat tray is a medium-sized, workmanlike tray built from oak boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			16.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Sturdy tray for carrying raw or cooked meat portions within a household."
		);

		CreateItem(
			"medieval_tableware_butter_dish",
			"dish",
			"a small butter dish",
			null,
			"This small butter dish is a very small, workmanlike dish formed from stoneware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			350.0,
			7.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
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
			"Small table dish for butter, curds, or soft cheese."
		);

		CreateItem(
			"medieval_tableware_carved_bone_tray",
			"tray",
			"a carved bone tray",
			null,
			"This carved bone tray is a small, well-made tray worked from bone. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.Small,
			ItemQuality.Good,
			420.0,
			36.0m,
			true,
			false,
			"bone",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Small decorated tray for refined table service or guest offerings."
		);

		CreateItem(
			"medieval_tableware_carved_ivory_bowl",
			"bowl",
			"a carved ivory bowl",
			null,
			"This carved ivory bowl is a very small, well-made bowl worked from ivory. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			260.0,
			90.0m,
			true,
			false,
			"ivory",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Small luxury bowl for spices, nuts, sweetmeats, or display service."
		);

		CreateItem(
			"medieval_tableware_ceramic_decanter",
			"decanter",
			"a ceramic wine decanter",
			null,
			"This ceramic wine decanter is a small, well-made decanter formed from ceramic. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1050.0,
			28.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Decanter"
			],
			null,
			null,
			null,
			null,
			"Ceramic decanter for table wine service."
		);

		CreateItem(
			"medieval_tableware_ceramic_painted_plate",
			"plate",
			"a painted ceramic plate",
			null,
			"This painted ceramic plate is a small, well-made plate formed from ceramic. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			720.0,
			20.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Decorated ceramic plate suited to wealthier household service."
		);

		CreateItem(
			"medieval_tableware_cheese_board",
			"board",
			"a broad cheese board",
			null,
			"This broad cheese board is a medium-sized, workmanlike board built from maple boards. A flat working face spans the board, with bevelled edges and a plain underside. Knife marks and rubbed patches cross the upper surface. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1050.0,
			13.0m,
			true,
			false,
			"maple",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Serving board for cheeses, fruit, and table knives."
		);

		CreateItem(
			"medieval_tableware_cheese_plate",
			"plate",
			"a cheese plate",
			null,
			"This cheese plate is a small, workmanlike plate built from beech boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			7.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Wooden plate for cheese, fruit, and other dry table portions."
		);

		CreateItem(
			"medieval_tableware_coarse_clay_bowl",
			"bowl",
			"a coarse clay bowl",
			null,
			"This coarse clay bowl is a small, plain bowl formed from fired clay. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Poor,
			720.0,
			2.0m,
			true,
			false,
			"clay",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
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
			"Low-status open bowl for rough household or servant use."
		);

		CreateItem(
			"medieval_tableware_communal_mazer",
			"mazer",
			"a communal maple mazer",
			null,
			"This communal maple mazer is a small, well-made mazer built from maple boards. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			420.0,
			26.0m,
			true,
			false,
			"maple",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Pint"
			],
			null,
			null,
			null,
			null,
			"Shared wooden drinking bowl for ale, mead, or spiced drink."
		);

		CreateItem(
			"medieval_tableware_corded_wooden_platter",
			"platter",
			"a corded wooden platter",
			null,
			"This corded wooden platter is a medium-sized, workmanlike platter built from pine boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			800.0,
			8.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Cheap platter with a simple corded edge for grip."
		);

		CreateItem(
			"medieval_tableware_covered_earthenware_jug",
			"jug",
			"a covered earthenware jug",
			null,
			"This covered earthenware jug is a small, workmanlike jug formed from earthenware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1250.0,
			12.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Covered jug for table drink; cover is descriptive, not an openable component."
		);

		CreateItem(
			"medieval_tableware_covered_salt_dish",
			"dish",
			"a lidded salt dish",
			null,
			"This lidded salt dish is a very small, well-made dish worked from pewter. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			320.0,
			28.0m,
			true,
			false,
			"pewter",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
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
			"Fine salt dish; the component models it as open table service rather than a locking container."
		);

		CreateItem(
			"medieval_tableware_covered_waterskin",
			"waterskin",
			"a covered waterskin",
			null,
			"This covered waterskin is a small, workmanlike waterskin made from cured animal skin. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			14.0m,
			true,
			false,
			"animal skin",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin"
			],
			null,
			null,
			null,
			null,
			"Skin vessel for water; included here as household and travel liquid ware, not wearable gear."
		);

		CreateItem(
			"medieval_tableware_cup_bearing_tray",
			"tray",
			"a cup-bearing tray",
			null,
			"This cup-bearing tray is a medium-sized, workmanlike tray built from ash boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			12.0m,
			true,
			false,
			"ash",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Tray sized for moving multiple cups or beakers."
		);

		CreateItem(
			"medieval_tableware_deep_eating_bowl",
			"bowl",
			"a deep eating bowl",
			null,
			"This deep eating bowl is a small, workmanlike bowl formed from earthenware. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			850.0,
			8.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
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
			"Deep bowl for stew-like or grain-heavy meals; not intended as liquid storage."
		);

		CreateItem(
			"medieval_tableware_drinking_horn",
			"horn",
			"a polished drinking horn",
			null,
			"This polished drinking horn is a small, well-made horn worked from horn. The drinking mouth is polished smooth, and the tapering body curves down to a capped point. A small resting foot keeps it steady when set down. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.Small,
			ItemQuality.Good,
			260.0,
			22.0m,
			true,
			false,
			"horn",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Pint"
			],
			null,
			null,
			null,
			null,
			"Polished horn vessel for feasts, halls, or travellers."
		);

		CreateItem(
			"medieval_tableware_earthenware_beaker",
			"beaker",
			"an earthenware beaker",
			null,
			"This earthenware beaker is a very small, workmanlike beaker formed from earthenware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			340.0,
			5.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Straight-sided cup for common drinks."
		);

		CreateItem(
			"medieval_tableware_earthenware_bottle",
			"bottle",
			"an earthenware bottle",
			null,
			"This earthenware bottle is a small, workmanlike bottle formed from earthenware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			8.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_PintBottle"
			],
			null,
			null,
			null,
			null,
			"Small ceramic bottle for household liquids."
		);

		CreateItem(
			"medieval_tableware_earthenware_dinner_plate",
			"plate",
			"an earthenware dinner plate",
			null,
			"This earthenware dinner plate is a small, workmanlike plate formed from earthenware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			5.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
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
			"Common ceramic plate for individual table service."
		);

		CreateItem(
			"medieval_tableware_earthenware_drinking_cup",
			"cup",
			"an earthenware drinking cup",
			null,
			"This earthenware drinking cup is a very small, workmanlike cup formed from earthenware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			320.0,
			5.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Common cup for water, ale, watered wine, or milk."
		);

		CreateItem(
			"medieval_tableware_earthenware_flagon",
			"flagon",
			"an earthenware flagon",
			null,
			"This earthenware flagon is a small, workmanlike flagon formed from earthenware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1250.0,
			12.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Decanter"
			],
			null,
			null,
			null,
			null,
			"Common ceramic flagon for drink service."
		);

		CreateItem(
			"medieval_tableware_earthenware_mug",
			"mug",
			"an earthenware mug",
			null,
			"This earthenware mug is a small, workmanlike mug formed from earthenware. A broad handle joins the side, and the open rim sits above a thickened base. The inside is smooth from repeated drink service. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			7.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_HalfPint"
			],
			null,
			null,
			null,
			null,
			"Handled clay mug for table drink."
		);

		CreateItem(
			"medieval_tableware_earthenware_table_jug",
			"jug",
			"an earthenware table jug",
			null,
			"This earthenware table jug is a small, workmanlike jug formed from earthenware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1100.0,
			10.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Household jug for water, milk, ale, or watered wine."
		);

		CreateItem(
			"medieval_tableware_earthenware_wash_basin",
			"basin",
			"an earthenware wash basin",
			null,
			"This earthenware wash basin is a medium-sized, workmanlike basin formed from earthenware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
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
				"LContainer_Amphora_Urna"
			],
			null,
			null,
			null,
			null,
			"Common household basin for washing hands, cups, or utensils."
		);

		CreateItem(
			"medieval_tableware_ewer_tray",
			"tray",
			"an ewer standing tray",
			null,
			"This ewer standing tray is a small, well-made tray worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			35.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Metal tray for setting an ewer or flagon without wetting the table."
		);

		CreateItem(
			"medieval_tableware_fine_ceramic_bowl",
			"bowl",
			"a fine ceramic bowl",
			null,
			"This fine ceramic bowl is a small, well-made bowl formed from ceramic. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			640.0,
			24.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Well-fired decorated bowl for higher-status service."
		);

		CreateItem(
			"medieval_tableware_fine_court_cup",
			"cup",
			"a fine court cup",
			null,
			"This fine court cup is a very small, finely made cup worked from gold. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			360.0,
			420.0m,
			true,
			false,
			"gold",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_SmallWineGlass"
			],
			null,
			null,
			null,
			null,
			"Exceptional precious-metal cup for court or treasury service."
		);

		CreateItem(
			"medieval_tableware_fish_platter",
			"platter",
			"a long fish platter",
			null,
			"This long fish platter is a medium-sized, workmanlike platter formed from earthenware. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			13.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Long platter for fish or other elongated dishes."
		);

		CreateItem(
			"medieval_tableware_flat_fish_plate",
			"plate",
			"a flat fish plate",
			null,
			"This flat fish plate is a small, workmanlike plate formed from earthenware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			7.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
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
			"Oblong plate for small fish, cutlets, or long portions."
		);

		CreateItem(
			"medieval_tableware_fruit_dish",
			"dish",
			"a shallow fruit dish",
			null,
			"This shallow fruit dish is a small, workmanlike dish formed from earthenware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			680.0,
			6.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
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
			"Shallow dish for fruit, nuts, or small preserved foods."
		);

		CreateItem(
			"medieval_tableware_fruit_platter",
			"platter",
			"a shallow fruit platter",
			null,
			"This shallow fruit platter is a medium-sized, workmanlike platter formed from stoneware. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1450.0,
			16.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Shallow platter for fruit, nuts, or preserved goods."
		);

		CreateItem(
			"medieval_tableware_glass_beaker",
			"beaker",
			"a glass beaker",
			null,
			"This glass beaker is a very small, well-made beaker made from glass. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			190.0,
			35.0m,
			true,
			false,
			"glass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Fine glass beaker for wine, ale, or clear drinks."
		);

		CreateItem(
			"medieval_tableware_glass_bottle",
			"bottle",
			"a stoppered glass bottle",
			null,
			"This stoppered glass bottle is a small, well-made bottle made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.Small,
			ItemQuality.Good,
			420.0,
			34.0m,
			true,
			false,
			"glass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_WineBottle"
			],
			null,
			null,
			null,
			null,
			"Stoppered bottle profile for wine, oil, vinegar, or fine liquids."
		);

		CreateItem(
			"medieval_tableware_glass_decanter",
			"decanter",
			"a glass wine decanter",
			null,
			"This glass wine decanter is a small, well-made decanter made from glass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.Small,
			ItemQuality.Good,
			780.0,
			52.0m,
			true,
			false,
			"glass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Decanter"
			],
			null,
			null,
			null,
			null,
			"Transparent decanter for wine or spiced drink."
		);

		CreateItem(
			"medieval_tableware_glass_drinking_cup",
			"cup",
			"a glass drinking cup",
			null,
			"This glass drinking cup is a very small, well-made cup made from glass. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			180.0,
			36.0m,
			true,
			false,
			"glass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Fragile transparent cup for fine drink service."
		);

		CreateItem(
			"medieval_tableware_glass_goblet",
			"goblet",
			"a glass table goblet",
			null,
			"This glass table goblet is a small, well-made goblet made from glass. A rounded bowl rises from a narrow stem and foot, leaving the rim clean and prominent. The base is weighted enough to stand firmly. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.Small,
			ItemQuality.Good,
			260.0,
			46.0m,
			true,
			false,
			"glass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_WineGlass"
			],
			null,
			null,
			null,
			null,
			"Fragile glass goblet for fine wine or spiced drink."
		);

		CreateItem(
			"medieval_tableware_glass_oil_flask",
			"flask",
			"a glass oil flask",
			null,
			"This glass oil flask is a very small, well-made flask made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			180.0,
			26.0m,
			true,
			false,
			"glass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Fine flask for oil, scent, or medicine."
		);

		CreateItem(
			"medieval_tableware_greenware_table_bowl",
			"bowl",
			"a green-glazed table bowl",
			null,
			"This green-glazed table bowl is a small, well-made bowl formed from ceramic. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			680.0,
			22.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Glazed bowl for refined table presentation; skins can vary colour or motif."
		);

		CreateItem(
			"medieval_tableware_handled_oak_tray",
			"tray",
			"a handled oak tray",
			null,
			"This handled oak tray is a medium-sized, workmanlike tray built from oak boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1250.0,
			15.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"General domestic tray for carrying cups, bowls, and table wares."
		);

		CreateItem(
			"medieval_tableware_handled_porringer",
			"porringer",
			"a handled earthenware porringer",
			null,
			"This handled earthenware porringer is a small, workmanlike porringer formed from earthenware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			640.0,
			8.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
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
			"Handled bowl for spoon foods, broths, and soft meals; component remains an open container."
		);

		CreateItem(
			"medieval_tableware_honey_flask",
			"flask",
			"a honey flask",
			null,
			"This honey flask is a very small, workmanlike flask formed from earthenware. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			280.0,
			7.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
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
			"Small flask for honey, syrup, or thick sweet liquid."
		);

		CreateItem(
			"medieval_tableware_honeycomb_plate",
			"plate",
			"a honeycomb plate",
			null,
			"This honeycomb plate is a small, workmanlike plate formed from earthenware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			560.0,
			6.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
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
			"Small plate for honeycomb, sweetmeats, or sticky table treats."
		);

		CreateItem(
			"medieval_tableware_horn_drinking_cup",
			"cup",
			"a horn drinking cup",
			null,
			"This horn drinking cup is a very small, workmanlike cup worked from horn. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			160.0,
			10.0m,
			true,
			false,
			"horn",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Hollowed horn cup for table or camp drinking."
		);

		CreateItem(
			"medieval_tableware_household_waterskin",
			"waterskin",
			"a household waterskin",
			null,
			"This household waterskin is a small, workmanlike waterskin made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			480.0,
			15.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin"
			],
			null,
			null,
			null,
			null,
			"Leather water container for kitchen, cart, or field use."
		);

		CreateItem(
			"medieval_tableware_humble_clay_cup",
			"cup",
			"a rough clay cup",
			null,
			"This rough clay cup is a very small, plain cup formed from fired clay. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			300.0,
			2.0m,
			true,
			false,
			"clay",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Very cheap clay cup for rough inns or poor households."
		);

		CreateItem(
			"medieval_tableware_large_ale_stein",
			"stein",
			"a large ale stein",
			null,
			"This large ale stein is a small, well-made stein formed from stoneware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			980.0,
			22.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Stein"
			],
			null,
			null,
			null,
			null,
			"Large stoneware drinking vessel using a stein-sized capacity profile."
		);

		CreateItem(
			"medieval_tableware_large_hall_platter",
			"platter",
			"a large hall platter",
			null,
			"This large hall platter is a large, workmanlike platter built from oak boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			2900.0,
			28.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Large wooden platter for communal hall portions; still portable."
		);

		CreateItem(
			"medieval_tableware_large_serving_bowl",
			"bowl",
			"a large serving bowl",
			null,
			"This large serving bowl is a medium-sized, workmanlike bowl formed from earthenware. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			14.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
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
			"Large open serving bowl for bread, fruit, greens, or thick foods."
		);

		CreateItem(
			"medieval_tableware_large_storage_urn",
			"urn",
			"a large storage urn",
			null,
			"This large storage urn is a large, workmanlike urn formed from earthenware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12000.0,
			34.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Quadrantal"
			],
			null,
			null,
			null,
			null,
			"Large urn for water, oil, or other household liquid storage."
		);

		CreateItem(
			"medieval_tableware_large_wash_basin",
			"basin",
			"a large wash basin",
			null,
			"This large wash basin is a medium-sized, workmanlike basin formed from ceramic. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6200.0,
			26.0m,
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
				"LContainer_Amphora_Urna"
			],
			null,
			null,
			null,
			null,
			"Liquid-holding basin for household handwashing or rinsing."
		);

		CreateItem(
			"medieval_tableware_large_water_pitcher",
			"pitcher",
			"a large water pitcher",
			null,
			"This large water pitcher is a medium-sized, workmanlike pitcher formed from earthenware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2600.0,
			18.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Congius"
			],
			null,
			null,
			null,
			null,
			"Larger household pitcher for refilling cups or basins."
		);

		CreateItem(
			"medieval_tableware_laundry_rinsing_basin",
			"basin",
			"a laundry rinsing basin",
			null,
			"This laundry rinsing basin is a large, workmanlike basin formed from earthenware. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9800.0,
			30.0m,
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
				"LContainer_Drum"
			],
			null,
			null,
			null,
			null,
			"Large household basin for rinsing cloth or soaking wares."
		);

		CreateItem(
			"medieval_tableware_leather_canteen",
			"canteen",
			"a leather water canteen",
			null,
			"This leather water canteen is a small, workmanlike canteen made from worked leather. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			14.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Canteen"
			],
			null,
			null,
			null,
			null,
			"Portable water vessel for work, travel, or bedchamber use."
		);

		CreateItem(
			"medieval_tableware_leather_drinking_cup",
			"cup",
			"a leather drinking cup",
			null,
			"This leather drinking cup is a very small, workmanlike cup made from worked leather. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			90.0,
			6.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Supple leather cup for rough travel or household use."
		);

		CreateItem(
			"medieval_tableware_leather_jack",
			"jack",
			"a leather drinking jack",
			null,
			"This leather drinking jack is a small, workmanlike jack made from worked leather. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			12.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_UKPint"
			],
			null,
			null,
			null,
			null,
			"Hardened leather drinking vessel for ale or water."
		);

		CreateItem(
			"medieval_tableware_leather_travel_flask",
			"flask",
			"a leather travel flask",
			null,
			"This leather travel flask is a small, workmanlike flask made from worked leather. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			300.0,
			12.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Flask"
			],
			null,
			null,
			null,
			null,
			"Portable flask for short journeys, messengers, or bedside liquid."
		);

		CreateItem(
			"medieval_tableware_lidded_stoneware_stein",
			"stein",
			"a lidded stoneware stein",
			null,
			"This lidded stoneware stein is a small, well-made stein formed from stoneware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1120.0,
			30.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Stein"
			],
			null,
			null,
			null,
			null,
			"Large lidded vessel for ale; lid is descriptive, not a separate openable component."
		);

		CreateItem(
			"medieval_tableware_linen_lined_tray",
			"tray",
			"a linen-lined tray",
			null,
			"This linen-lined tray is a medium-sized, well-made tray built from plain wood. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			920.0,
			22.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Tray with a fitted textile pad for finer domestic service."
		);

		CreateItem(
			"medieval_tableware_long_bread_board",
			"board",
			"a long bread board",
			null,
			"This long bread board is a medium-sized, workmanlike board built from oak boards. A flat working face spans the board, with bevelled edges and a plain underside. Knife marks and rubbed patches cross the upper surface. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			14.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Long board for bread loaves, trenchers, or slicing at table."
		);

		CreateItem(
			"medieval_tableware_maple_eating_board",
			"board",
			"a maple eating board",
			null,
			"This maple eating board is a small, workmanlike board built from maple boards. A flat working face spans the board, with bevelled edges and a plain underside. Knife marks and rubbed patches cross the upper surface. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			560.0,
			8.0m,
			true,
			false,
			"maple",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Flat personal board for bread, cheese, cut meat, or fruit."
		);

		CreateItem(
			"medieval_tableware_market_sample_tray",
			"tray",
			"a small sample tray",
			null,
			"This small sample tray is a small, workmanlike tray built from willow boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			280.0,
			4.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Light tray for samples, street food, or small table portions."
		);

		CreateItem(
			"medieval_tableware_meat_platter",
			"platter",
			"a heavy meat platter",
			null,
			"This heavy meat platter is a medium-sized, workmanlike platter built from oak boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1500.0,
			18.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Heavy platter for roasts, cut meats, or shared hall portions."
		);

		CreateItem(
			"medieval_tableware_medicine_flask",
			"flask",
			"a medicine flask",
			null,
			"This medicine flask is a very small, well-made flask made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			160.0,
			24.0m,
			true,
			false,
			"glass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Small flask for medicines, cordials, or infusions."
		);

		CreateItem(
			"medieval_tableware_milk_jug",
			"jug",
			"a small milk jug",
			null,
			"This small milk jug is a small, workmanlike jug formed from earthenware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			850.0,
			8.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Small jug for milk, cream, or watered dairy service."
		);

		CreateItem(
			"medieval_tableware_mounted_drinking_horn",
			"horn",
			"a brass-mounted drinking horn",
			null,
			"This brass-mounted drinking horn is a small, well-made horn worked from horn. The drinking mouth is polished smooth, and the tapering body curves down to a capped point. A small resting foot keeps it steady when set down. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.Small,
			ItemQuality.Good,
			340.0,
			36.0m,
			true,
			false,
			"horn",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Pint"
			],
			null,
			null,
			null,
			null,
			"Horn vessel with bright metal fittings described as part of its presentation."
		);

		CreateItem(
			"medieval_tableware_narrow_neck_oil_jar",
			"jar",
			"a narrow-necked oil jar",
			null,
			"This narrow-necked oil jar is a medium-sized, workmanlike jar formed from terracotta. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3200.0,
			18.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Congius"
			],
			null,
			null,
			null,
			null,
			"Narrow-necked jar for oil or other precious household liquid."
		);

		CreateItem(
			"medieval_tableware_narrow_wine_tray",
			"tray",
			"a narrow wine tray",
			null,
			"This narrow wine tray is a small, well-made tray worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			760.0,
			32.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Narrow tray for cups, small goblets, or a decanter."
		);

		CreateItem(
			"medieval_tableware_oak_cup",
			"cup",
			"an oak drinking cup",
			null,
			"This oak drinking cup is a very small, workmanlike cup built from oak boards. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			210.0,
			7.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Durable wooden cup for ale, water, or table drink."
		);

		CreateItem(
			"medieval_tableware_oak_tankard",
			"tankard",
			"an oak tankard",
			null,
			"This oak tankard is a small, workmanlike tankard built from oak boards. A broad handle joins the side, and the open rim sits above a thickened base. The inside is smooth from repeated drink service. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			580.0,
			12.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_UKPint"
			],
			null,
			null,
			null,
			null,
			"Wooden tankard for ale or cider service."
		);

		CreateItem(
			"medieval_tableware_oak_trencher_board",
			"trencher",
			"an oak trencher board",
			null,
			"This oak trencher board is a small, workmanlike trencher built from oak boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			7.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Reusable wooden trencher for bread, meat, cheese, and cut portions."
		);

		CreateItem(
			"medieval_tableware_oaken_feast_board",
			"board",
			"an oaken feast board",
			null,
			"This oaken feast board is a large, well-made board built from oak boards. A flat working face spans the board, with bevelled edges and a plain underside. Knife marks and rubbed patches cross the upper surface. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			3400.0,
			38.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Large board for laying out shared feast portions."
		);

		CreateItem(
			"medieval_tableware_oil_amphora",
			"amphora",
			"an earthenware oil amphora",
			null,
			"This earthenware oil amphora is a large, workmanlike amphora formed from earthenware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8900.0,
			30.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
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
			"Large ceramic vessel for oil storage."
		);

		CreateItem(
			"medieval_tableware_oil_cruet",
			"cruet",
			"a small oil cruet",
			null,
			"This small oil cruet is a very small, well-made cruet made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			190.0,
			26.0m,
			true,
			false,
			"glass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Fine cruet for pouring oil at table."
		);

		CreateItem(
			"medieval_tableware_painted_ceramic_platter",
			"platter",
			"a painted ceramic platter",
			null,
			"This painted ceramic platter is a medium-sized, well-made platter formed from ceramic. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1650.0,
			32.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Decorated platter for feasts or wealthier tables."
		);

		CreateItem(
			"medieval_tableware_paired_table_cruet",
			"cruet",
			"a paired table cruet",
			null,
			"This paired table cruet is a small, well-made cruet formed from ceramic. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			420.0,
			36.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Small double-bodied vessel represented by one liquid-container profile."
		);

		CreateItem(
			"medieval_tableware_pewter_drinking_cup",
			"cup",
			"a pewter drinking cup",
			null,
			"This pewter drinking cup is a very small, well-made cup worked from pewter. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			260.0,
			28.0m,
			true,
			false,
			"pewter",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Soft-metal cup for a wealthier table."
		);

		CreateItem(
			"medieval_tableware_pewter_ewer",
			"ewer",
			"a pewter table ewer",
			null,
			"This pewter table ewer is a small, well-made ewer worked from pewter. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1450.0,
			42.0m,
			true,
			false,
			"pewter",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Soft-metal ewer for household or inn service."
		);

		CreateItem(
			"medieval_tableware_pewter_flagon",
			"flagon",
			"a pewter table flagon",
			null,
			"This pewter table flagon is a small, well-made flagon worked from pewter. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1500.0,
			46.0m,
			true,
			false,
			"pewter",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Lidded-looking table flagon for wine or ale; no separate lock/openable behaviour."
		);

		CreateItem(
			"medieval_tableware_pewter_goblet",
			"goblet",
			"a pewter table goblet",
			null,
			"This pewter table goblet is a small, well-made goblet worked from pewter. A rounded bowl rises from a narrow stem and foot, leaving the rim clean and prominent. The base is weighted enough to stand firmly. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			520.0,
			32.0m,
			true,
			false,
			"pewter",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_WineGlass"
			],
			null,
			null,
			null,
			null,
			"Soft-metal goblet for wine, ale, or ceremonial toasts."
		);

		CreateItem(
			"medieval_tableware_pewter_serving_platter",
			"platter",
			"a pewter serving platter",
			null,
			"This pewter serving platter is a medium-sized, well-made platter worked from pewter. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1650.0,
			34.0m,
			true,
			false,
			"pewter",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Soft-metal platter for higher-status table service."
		);

		CreateItem(
			"medieval_tableware_pewter_tankard",
			"tankard",
			"a pewter tankard",
			null,
			"This pewter tankard is a small, well-made tankard worked from pewter. A broad handle joins the side, and the open rim sits above a thickened base. The inside is smooth from repeated drink service. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			760.0,
			36.0m,
			true,
			false,
			"pewter",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_UKPint"
			],
			null,
			null,
			null,
			null,
			"Metal tankard for a higher-status tavern or household."
		);

		CreateItem(
			"medieval_tableware_plain_drinking_horn",
			"horn",
			"a plain drinking horn",
			null,
			"This plain drinking horn is a small, workmanlike horn worked from horn. The drinking mouth is polished smooth, and the tapering body curves down to a capped point. A small resting foot keeps it steady when set down. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			12.0m,
			true,
			false,
			"horn",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_HalfPint"
			],
			null,
			null,
			null,
			null,
			"Simple horn drinking vessel for common use."
		);

		CreateItem(
			"medieval_tableware_plain_wooden_bowl",
			"bowl",
			"a plain wooden bowl",
			null,
			"This plain wooden bowl is a small, workmanlike bowl built from plain wood. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			380.0,
			5.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Everyday bowl for dry or thick foods."
		);

		CreateItem(
			"medieval_tableware_polished_bone_dish",
			"dish",
			"a polished bone dish",
			null,
			"This polished bone dish is a very small, well-made dish worked from bone. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			220.0,
			24.0m,
			true,
			false,
			"bone",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Small polished dish for fine condiments, sweetmeats, or gaming-table snacks."
		);

		CreateItem(
			"medieval_tableware_porcelain_serving_dish",
			"dish",
			"a porcelain serving dish",
			null,
			"This porcelain serving dish is a medium-sized, well-made dish formed from porcelain. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1100.0,
			70.0m,
			true,
			false,
			"porcelain",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Fine serving dish for elite or imported table service."
		);

		CreateItem(
			"medieval_tableware_porcelain_table_plate",
			"plate",
			"a porcelain table plate",
			null,
			"This porcelain table plate is a small, well-made plate formed from porcelain. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			540.0,
			42.0m,
			true,
			false,
			"porcelain",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Fine pale plate for elite, imported, or court-facing table service."
		);

		CreateItem(
			"medieval_tableware_porridge_bowl",
			"bowl",
			"a wooden porridge bowl",
			null,
			"This wooden porridge bowl is a small, workmanlike bowl built from maple boards. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			7.0m,
			true,
			false,
			"maple",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Wooden bowl for porridge and other thick foods."
		);

		CreateItem(
			"medieval_tableware_pottage_bowl",
			"bowl",
			"a broad pottage bowl",
			null,
			"This broad pottage bowl is a small, workmanlike bowl formed from earthenware. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			760.0,
			7.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
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
			"Broad bowl for pottage, porridge, and spoon foods; modelled as an open food container."
		);

		CreateItem(
			"medieval_tableware_raised_edge_platter",
			"platter",
			"a raised-edge platter",
			null,
			"This raised-edge platter is a medium-sized, workmanlike platter formed from earthenware. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			14.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Raised-edge platter for loose cooked foods and shared portions."
		);

		CreateItem(
			"medieval_tableware_raised_foot_dish",
			"dish",
			"a raised-foot table dish",
			null,
			"This raised-foot table dish is a small, well-made dish formed from ceramic. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			760.0,
			26.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Raised dish for presenting fruit, sweetmeats, or fine portions."
		);

		CreateItem(
			"medieval_tableware_refectory_food_tray",
			"tray",
			"a refectory food tray",
			null,
			"This refectory food tray is a medium-sized, workmanlike tray built from beech boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1000.0,
			10.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Plain service tray suited to communal dining halls."
		);

		CreateItem(
			"medieval_tableware_relish_tray",
			"tray",
			"a narrow relish tray",
			null,
			"This narrow relish tray is a small, well-made tray formed from ceramic. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			620.0,
			20.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Narrow tray for pickles, relishes, or small sauces."
		);

		CreateItem(
			"medieval_tableware_rimmed_earthenware_plate",
			"plate",
			"a rimmed earthenware plate",
			null,
			"This rimmed earthenware plate is a small, workmanlike plate formed from earthenware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			700.0,
			6.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
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
			"Low-rimmed individual plate for sauced or crumbly foods."
		);

		CreateItem(
			"medieval_tableware_rimmed_wooden_plate",
			"plate",
			"a rimmed wooden plate",
			null,
			"This rimmed wooden plate is a small, workmanlike plate built from beech boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Low-rimmed plate that keeps loose dry foods gathered."
		);

		CreateItem(
			"medieval_tableware_rimmed_wooden_tray",
			"tray",
			"a rimmed wooden tray",
			null,
			"This rimmed wooden tray is a medium-sized, workmanlike tray built from pine boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			850.0,
			9.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Simple rimmed tray for carrying cups or small dishes."
		);

		CreateItem(
			"medieval_tableware_rinsing_bowl",
			"bowl",
			"a rinsing bowl",
			null,
			"This rinsing bowl is a small, workmanlike bowl formed from ceramic. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1600.0,
			16.0m,
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
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Liquid-holding bowl for rinsing fingers, cups, or small utensils."
		);

		CreateItem(
			"medieval_tableware_round_wooden_plate",
			"plate",
			"a round wooden plate",
			null,
			"This round wooden plate is a small, workmanlike plate built from oak boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			460.0,
			6.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Simple round plate for common household meals."
		);

		CreateItem(
			"medieval_tableware_rush_bottomed_service_tray",
			"tray",
			"a rush-bottomed service tray",
			null,
			"This rush-bottomed service tray is a medium-sized, workmanlike tray built from willow boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			520.0,
			6.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Light woven service tray for bread, fruit, or dry goods."
		);

		CreateItem(
			"medieval_tableware_salt_dish",
			"dish",
			"a tiny salt dish",
			null,
			"This tiny salt dish is a tiny, workmanlike dish formed from ceramic. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			120.0,
			4.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
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
			"Tiny open dish for salt or other dry table seasoning."
		);

		CreateItem(
			"medieval_tableware_sauce_dish",
			"dish",
			"a lipped sauce dish",
			null,
			"This lipped sauce dish is a very small, workmanlike dish formed from earthenware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			260.0,
			5.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
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
			"Small lipped dish for thick sauces, relishes, or dipping condiments."
		);

		CreateItem(
			"medieval_tableware_sealed_water_urn",
			"urn",
			"a sealed water urn",
			null,
			"This sealed water urn is a large, well-made urn formed from stoneware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			13500.0,
			48.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Quadrantal"
			],
			null,
			null,
			null,
			null,
			"Sturdier urn for storing clean water or wine."
		);

		CreateItem(
			"medieval_tableware_serving_platter_earthenware",
			"platter",
			"an earthenware serving platter",
			null,
			"This earthenware serving platter is a medium-sized, workmanlike platter formed from earthenware. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1700.0,
			11.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Common ceramic platter for shared foods."
		);

		CreateItem(
			"medieval_tableware_serving_platter_stoneware",
			"platter",
			"a stoneware serving platter",
			null,
			"This stoneware serving platter is a medium-sized, workmanlike platter formed from stoneware. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1900.0,
			18.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Sturdier serving platter for repeated household use."
		);

		CreateItem(
			"medieval_tableware_shallow_beech_tray",
			"tray",
			"a shallow beech tray",
			null,
			"This shallow beech tray is a medium-sized, workmanlike tray built from beech boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			12.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Light tray for household service and table clearing."
		);

		CreateItem(
			"medieval_tableware_shaving_basin",
			"basin",
			"a small shaving basin",
			null,
			"This small shaving basin is a small, well-made basin worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1600.0,
			36.0m,
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
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Small metal basin for grooming and washing; represented by a jug-sized liquid profile."
		);

		CreateItem(
			"medieval_tableware_shot_sized_medicinal_cup",
			"cup",
			"a tiny medicinal cup",
			null,
			"This tiny medicinal cup is a tiny, well-made cup made from glass. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			70.0,
			18.0m,
			true,
			false,
			"glass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_ShotGlass"
			],
			null,
			null,
			null,
			null,
			"Tiny glass for measured medicinal draughts or strong cordials."
		);

		CreateItem(
			"medieval_tableware_silvered_goblet",
			"goblet",
			"a silver table goblet",
			null,
			"This silver table goblet is a small, finely made goblet worked from silver. A rounded bowl rises from a narrow stem and foot, leaving the rim clean and prominent. The base is weighted enough to stand firmly. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.VeryGood,
			480.0,
			180.0m,
			true,
			false,
			"silver",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_WineGlass"
			],
			null,
			null,
			null,
			null,
			"Luxury silver goblet for elite tables, ceremonial gifts, or treasure rooms."
		);

		CreateItem(
			"medieval_tableware_small_bread_plate",
			"plate",
			"a small bread plate",
			null,
			"This small bread plate is a very small, workmanlike plate built from plain wood. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			300.0,
			4.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Compact plate for bread portions or small side servings."
		);

		CreateItem(
			"medieval_tableware_small_breaking_board",
			"board",
			"a small breaking board",
			null,
			"This small breaking board is a small, workmanlike board built from beech boards. A flat working face spans the board, with bevelled edges and a plain underside. Knife marks and rubbed patches cross the upper surface. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			450.0,
			6.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Small board for bread-breaking, cheese-cutting, and quick service."
		);

		CreateItem(
			"medieval_tableware_small_carved_wooden_plate",
			"plate",
			"a carved wooden plate",
			null,
			"This carved wooden plate is a small, well-made plate built from walnut boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			520.0,
			18.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Finer wooden plate with shallow carved ornament around the rim."
		);

		CreateItem(
			"medieval_tableware_small_carving_platter",
			"platter",
			"a small carving platter",
			null,
			"This small carving platter is a medium-sized, workmanlike platter built from oak boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1350.0,
			15.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Platter used for carving smaller roasts or poultry at table."
		);

		CreateItem(
			"medieval_tableware_small_cordial_cup",
			"cup",
			"a small cordial cup",
			null,
			"This small cordial cup is a tiny, well-made cup made from glass. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			90.0,
			28.0m,
			true,
			false,
			"glass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Liqueur"
			],
			null,
			null,
			null,
			null,
			"Very small cup for cordial, strong drink, or medicinal draughts."
		);

		CreateItem(
			"medieval_tableware_small_glass_goblet",
			"goblet",
			"a small glass goblet",
			null,
			"This small glass goblet is a very small, well-made goblet made from glass. A rounded bowl rises from a narrow stem and foot, leaving the rim clean and prominent. The base is weighted enough to stand firmly. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			200.0,
			38.0m,
			true,
			false,
			"glass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_SmallWineGlass"
			],
			null,
			null,
			null,
			null,
			"Smaller goblet for wine or liqueur-like portions."
		);

		CreateItem(
			"medieval_tableware_small_laver_basin",
			"basin",
			"a small laver basin",
			null,
			"This small laver basin is a medium-sized, well-made basin worked from bronze. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3400.0,
			60.0m,
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
				"LContainer_Amphora_Urna"
			],
			null,
			null,
			null,
			null,
			"Fine basin for washing hands at table or in a chamber."
		);

		CreateItem(
			"medieval_tableware_small_mazer",
			"mazer",
			"a small wooden mazer",
			null,
			"This small wooden mazer is a small, well-made mazer built from walnut boards. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			300.0,
			24.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_HalfPint"
			],
			null,
			null,
			null,
			null,
			"Fine wooden drinking bowl for individual service."
		);

		CreateItem(
			"medieval_tableware_small_nut_dish",
			"dish",
			"a small nut dish",
			null,
			"This small nut dish is a very small, workmanlike dish built from walnut boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			180.0,
			8.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Compact dish for nuts, dried fruit, or table nibbles."
		);

		CreateItem(
			"medieval_tableware_small_oil_flask",
			"flask",
			"a small earthenware oil flask",
			null,
			"This small earthenware oil flask is a very small, workmanlike flask formed from earthenware. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			260.0,
			7.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
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
			"Small flask for oil, scent, medicine, or concentrated drink."
		);

		CreateItem(
			"medieval_tableware_small_serving_bowl",
			"bowl",
			"a small serving bowl",
			null,
			"This small serving bowl is a small, workmanlike bowl formed from stoneware. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
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
			"Small shared bowl for table portions or side dishes."
		);

		CreateItem(
			"medieval_tableware_small_side_dish",
			"dish",
			"a small side dish",
			null,
			"This small side dish is a very small, workmanlike dish formed from ceramic. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			300.0,
			6.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
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
			"Small open dish for pickles, herbs, sauces, or side portions."
		);

		CreateItem(
			"medieval_tableware_small_table_amphora",
			"amphora",
			"a small table amphora",
			null,
			"This small table amphora is a medium-sized, workmanlike amphora formed from terracotta. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2600.0,
			16.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Congius"
			],
			null,
			null,
			null,
			null,
			"Smaller amphora-like vessel for table or pantry use."
		);

		CreateItem(
			"medieval_tableware_small_taster_cup",
			"cup",
			"a tiny taster cup",
			null,
			"This tiny taster cup is a tiny, workmanlike cup worked from pewter. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			90.0,
			10.0m,
			true,
			false,
			"pewter",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Pony"
			],
			null,
			null,
			null,
			null,
			"Tiny vessel for tasting wine, ale, or medicine."
		);

		CreateItem(
			"medieval_tableware_small_wine_cask",
			"cask",
			"a small wine cask",
			null,
			"This small wine cask is a medium-sized, workmanlike cask built from oak boards. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			40.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_GallonCask"
			],
			null,
			null,
			null,
			null,
			"Small household cask for wine, ale, vinegar, or water."
		);

		CreateItem(
			"medieval_tableware_small_wine_cup",
			"cup",
			"a small wine cup",
			null,
			"This small wine cup is a very small, well-made cup formed from ceramic. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			220.0,
			16.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_SmallWineGlass"
			],
			null,
			null,
			null,
			null,
			"Small cup suited to wine or watered wine; not necessarily made of glass."
		);

		CreateItem(
			"medieval_tableware_small_wine_flagon",
			"flagon",
			"a small wine flagon",
			null,
			"This small wine flagon is a small, workmanlike flagon formed from stoneware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1000.0,
			13.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Decanter"
			],
			null,
			null,
			null,
			null,
			"Compact flagon for wine or cordial service."
		);

		CreateItem(
			"medieval_tableware_spice_dish",
			"dish",
			"a small spice dish",
			null,
			"This small spice dish is a tiny, workmanlike dish built from plain wood. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			90.0,
			3.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Small open dish for dry spice pinches or powdered condiments."
		);

		CreateItem(
			"medieval_tableware_spice_tray",
			"tray",
			"a small spice tray",
			null,
			"This small spice tray is a small, well-made tray built from walnut boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			420.0,
			18.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Small tray for condiment dishes, salt, or costly spices."
		);

		CreateItem(
			"medieval_tableware_spouted_pitcher",
			"pitcher",
			"a spouted ceramic pitcher",
			null,
			"This spouted ceramic pitcher is a small, well-made pitcher formed from ceramic. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1200.0,
			24.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Fine spouted pitcher for cleaner pouring at table."
		);

		CreateItem(
			"medieval_tableware_stave_built_tankard",
			"tankard",
			"a stave-built tankard",
			null,
			"This stave-built tankard is a small, workmanlike tankard built from oak boards. A broad handle joins the side, and the open rim sits above a thickened base. The inside is smooth from repeated drink service. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			640.0,
			14.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_UKPint"
			],
			null,
			null,
			null,
			null,
			"Stave-built tankard with hoop-like construction described visually only."
		);

		CreateItem(
			"medieval_tableware_stoneware_beaker",
			"beaker",
			"a stoneware beaker",
			null,
			"This stoneware beaker is a very small, workmanlike beaker formed from stoneware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			390.0,
			8.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Durable beaker for household or tavern use."
		);

		CreateItem(
			"medieval_tableware_stoneware_bottle",
			"bottle",
			"a stoneware bottle",
			null,
			"This stoneware bottle is a small, workmanlike bottle formed from stoneware. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			700.0,
			10.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_QuartBottle"
			],
			null,
			null,
			null,
			null,
			"Sturdy bottle for ale, vinegar, or table liquid."
		);

		CreateItem(
			"medieval_tableware_stoneware_dinner_plate",
			"plate",
			"a stoneware dinner plate",
			null,
			"This stoneware dinner plate is a small, workmanlike plate formed from stoneware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			760.0,
			9.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
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
			"Sturdier stoneware plate for repeated household use."
		);

		CreateItem(
			"medieval_tableware_stoneware_drinking_cup",
			"cup",
			"a stoneware drinking cup",
			null,
			"This stoneware drinking cup is a very small, workmanlike cup formed from stoneware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			360.0,
			8.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Sturdy cup for everyday household drinking."
		);

		CreateItem(
			"medieval_tableware_stoneware_mug",
			"mug",
			"a stoneware mug",
			null,
			"This stoneware mug is a small, workmanlike mug formed from stoneware. A broad handle joins the side, and the open rim sits above a thickened base. The inside is smooth from repeated drink service. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			560.0,
			10.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_HalfPint"
			],
			null,
			null,
			null,
			null,
			"Sturdy handled mug for household or inn service."
		);

		CreateItem(
			"medieval_tableware_stoneware_porringer",
			"porringer",
			"a stoneware porringer",
			null,
			"This stoneware porringer is a small, workmanlike porringer formed from stoneware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			700.0,
			10.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
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
			"Sturdy porringer for common household table use."
		);

		CreateItem(
			"medieval_tableware_stoneware_table_jug",
			"jug",
			"a stoneware table jug",
			null,
			"This stoneware table jug is a small, workmanlike jug formed from stoneware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1250.0,
			14.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Sturdy jug for repeated table and kitchen service."
		);

		CreateItem(
			"medieval_tableware_stoneware_washing_basin",
			"basin",
			"a stoneware washing basin",
			null,
			"This stoneware washing basin is a medium-sized, workmanlike basin formed from stoneware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5600.0,
			24.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
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
			"Sturdy wash basin for kitchen or chamber work."
		);

		CreateItem(
			"medieval_tableware_sweetmeat_tray",
			"tray",
			"a sweetmeat tray",
			null,
			"This sweetmeat tray is a small, well-made tray worked from ivory. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The pale surface is smoothed at the high points and darker in the cut recesses.",
			SizeCategory.Small,
			ItemQuality.Good,
			340.0,
			95.0m,
			true,
			false,
			"ivory",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Small luxury tray for sweetmeats, spiced nuts, or delicacies."
		);

		CreateItem(
			"medieval_tableware_table_beer_cask",
			"cask",
			"a table beer cask",
			null,
			"This table beer cask is a medium-sized, workmanlike cask built from oak boards. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4500.0,
			38.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_GallonCask"
			],
			null,
			null,
			null,
			null,
			"Small service cask for beer or ale at a hall or tavern table."
		);

		CreateItem(
			"medieval_tableware_table_ewer",
			"ewer",
			"an earthenware table ewer",
			null,
			"This earthenware table ewer is a small, workmanlike ewer formed from earthenware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1200.0,
			12.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Decanter"
			],
			null,
			null,
			null,
			null,
			"Pouring vessel for drink or washing water using a decanter-sized profile."
		);

		CreateItem(
			"medieval_tableware_table_olive_dish",
			"dish",
			"a narrow olive dish",
			null,
			"This narrow olive dish is a very small, workmanlike dish formed from ceramic. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			280.0,
			8.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
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
			"Narrow dish for olives, pickles, or preserved fruit."
		);

		CreateItem(
			"medieval_tableware_tall_ale_beaker",
			"beaker",
			"a tall ale beaker",
			null,
			"This tall ale beaker is a small, workmanlike beaker formed from stoneware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			10.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Pint"
			],
			null,
			null,
			null,
			null,
			"Taller beaker for ale or other common drink."
		);

		CreateItem(
			"medieval_tableware_tavern_carry_tray",
			"tray",
			"a tavern carry tray",
			null,
			"This tavern carry tray is a medium-sized, workmanlike tray built from pine boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			780.0,
			7.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Cheap tray for cups, bowls, and rough tavern service."
		);

		CreateItem(
			"medieval_tableware_terracotta_serving_platter",
			"platter",
			"a terracotta serving platter",
			null,
			"This terracotta serving platter is a medium-sized, workmanlike platter formed from terracotta. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			12.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Warm-fired platter for common table service."
		);

		CreateItem(
			"medieval_tableware_terracotta_table_plate",
			"plate",
			"a terracotta table plate",
			null,
			"This terracotta table plate is a small, workmanlike plate formed from terracotta. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			670.0,
			6.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
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
			"Warm-coloured fired-clay plate for common meals."
		);

		CreateItem(
			"medieval_tableware_two_handled_serving_tray",
			"tray",
			"a two-handled serving tray",
			null,
			"This two-handled serving tray is a medium-sized, well-made tray built from oak boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1450.0,
			22.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Sturdier tray with side grips for hall service."
		);

		CreateItem(
			"medieval_tableware_vinegar_amphora",
			"amphora",
			"a vinegar amphora",
			null,
			"This vinegar amphora is a large, workmanlike amphora formed from terracotta. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8800.0,
			28.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
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
			"Large vessel for vinegar or sour wine."
		);

		CreateItem(
			"medieval_tableware_vinegar_cruet",
			"cruet",
			"a small vinegar cruet",
			null,
			"This small vinegar cruet is a very small, well-made cruet made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			190.0,
			26.0m,
			true,
			false,
			"glass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Fine cruet for vinegar or sharp sauces."
		);

		CreateItem(
			"medieval_tableware_vinegar_flask",
			"flask",
			"a small vinegar flask",
			null,
			"This small vinegar flask is a very small, workmanlike flask formed from stoneware. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			280.0,
			8.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
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
			"Small flask for vinegar or sharp table liquids."
		);

		CreateItem(
			"medieval_tableware_warm_crock_platter",
			"platter",
			"a thick crock platter",
			null,
			"This thick crock platter is a medium-sized, workmanlike platter formed from stoneware. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2200.0,
			18.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Thick stoneware platter for hearty food service; does not provide heat mechanics."
		);

		CreateItem(
			"medieval_tableware_wash_ewer",
			"ewer",
			"a handwashing ewer",
			null,
			"This handwashing ewer is a small, well-made ewer formed from ceramic. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			26.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Decanter"
			],
			null,
			null,
			null,
			null,
			"Ewer for washing hands before or after meals."
		);

		CreateItem(
			"medieval_tableware_water_pitcher",
			"pitcher",
			"a water pitcher",
			null,
			"This water pitcher is a small, workmanlike pitcher formed from earthenware. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1200.0,
			10.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Handled pitcher for water service."
		);

		CreateItem(
			"medieval_tableware_water_storage_jar",
			"jar",
			"a water storage jar",
			null,
			"This water storage jar is a large, workmanlike jar formed from earthenware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9500.0,
			26.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
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
			"Large ceramic vessel for household water storage."
		);

		CreateItem(
			"medieval_tableware_whiskey_sized_spirit_cup",
			"cup",
			"a small spirit cup",
			null,
			"This small spirit cup is a tiny, workmanlike cup formed from ceramic. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			120.0,
			8.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_WhiskeyGlass"
			],
			null,
			null,
			null,
			null,
			"Small cup for concentrated drink, medicine, or spirits in fantasy settings."
		);

		CreateItem(
			"medieval_tableware_whiteware_wine_cup",
			"cup",
			"a whiteware wine cup",
			null,
			"This whiteware wine cup is a very small, well-made cup formed from ceramic. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			240.0,
			18.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_SmallWineGlass"
			],
			null,
			null,
			null,
			null,
			"Pale ceramic wine cup for finer tables."
		);

		CreateItem(
			"medieval_tableware_wide_mouthed_water_jar",
			"jar",
			"a wide-mouthed water jar",
			null,
			"This wide-mouthed water jar is a medium-sized, workmanlike jar formed from earthenware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3400.0,
			17.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Congius"
			],
			null,
			null,
			null,
			null,
			"Wide-mouthed jar for water kept near the hearth or door."
		);

		CreateItem(
			"medieval_tableware_wide_table_dish",
			"dish",
			"a wide table dish",
			null,
			"This wide table dish is a small, workmanlike dish formed from stoneware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
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
			"Broad open dish for shared table portions."
		);

		CreateItem(
			"medieval_tableware_willow_peeling_dish",
			"dish",
			"a willow peeling dish",
			null,
			"This willow peeling dish is a small, workmanlike dish built from willow boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			4.0m,
			true,
			false,
			"willow",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Light open dish for vegetable peelings, scraps, or simple kitchen service."
		);

		CreateItem(
			"medieval_tableware_wine_amphora",
			"amphora",
			"an earthenware wine amphora",
			null,
			"This earthenware wine amphora is a large, workmanlike amphora formed from earthenware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			30.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
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
			"Large amphora-like vessel for wine or other stored liquid."
		);

		CreateItem(
			"medieval_tableware_wood_hooped_water_bucket",
			"bucket",
			"a wood-hooped water bucket",
			null,
			"This wood-hooped water bucket is a medium-sized, workmanlike bucket built from oak boards. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			18.0m,
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
				"Destroyable_Misc",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Handled bucket-like vessel for carrying water inside a household."
		);

		CreateItem(
			"medieval_tableware_wooden_ale_bowl",
			"bowl",
			"a wooden ale bowl",
			null,
			"This wooden ale bowl is a small, workmanlike bowl built from maple boards. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			8.0m,
			true,
			false,
			"maple",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Small rounded drinking bowl for ale or water."
		);

		CreateItem(
			"medieval_tableware_wooden_canteen",
			"canteen",
			"a wooden water canteen",
			null,
			"This wooden water canteen is a small, workmanlike canteen built from plain wood. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			600.0,
			15.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Canteen"
			],
			null,
			null,
			null,
			null,
			"Flat portable water vessel for travel or work."
		);

		CreateItem(
			"medieval_tableware_wooden_drinking_cup",
			"cup",
			"a wooden drinking cup",
			null,
			"This wooden drinking cup is a very small, workmanlike cup built from beech boards. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			180.0,
			6.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Light carved cup for common drink service."
		);

		CreateItem(
			"medieval_tableware_wooden_mug",
			"mug",
			"a wooden table mug",
			null,
			"This wooden table mug is a small, workmanlike mug built from oak boards. A broad handle joins the side, and the open rim sits above a thickened base. The inside is smooth from repeated drink service. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			380.0,
			9.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_HalfPint"
			],
			null,
			null,
			null,
			null,
			"Handled wooden mug for everyday ale or water."
		);

		CreateItem(
			"medieval_tableware_wooden_porringer",
			"porringer",
			"a wooden porringer",
			null,
			"This wooden porringer is a small, workmanlike porringer built from beech boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			7.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Light wooden porringer for thick foods or children's servings."
		);

		CreateItem(
			"medieval_tableware_wooden_table_jug",
			"jug",
			"a wooden table jug",
			null,
			"This wooden table jug is a small, workmanlike jug built from oak boards. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			800.0,
			12.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Wooden jug for common household drink service."
		);

		CreateItem(
			"medieval_tableware_shared_northern_north_sea_birch_bark_cup",
			"cup",
			"a birch-bark drinking cup",
			null,
			"This birch-bark drinking cup is a very small, workmanlike cup built from birch boards. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			120.0,
			6.0m,
			true,
			false,
			"birch",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Light northern cup made from bark-like birch material for travel or simple household use."
		);

		CreateItem(
			"medieval_tableware_shared_northern_north_sea_flat_oatcake_trencher",
			"trencher",
			"a flat oatcake trencher",
			null,
			"This flat oatcake trencher is a small, workmanlike trencher made from firm baked bread. The centre is pressed shallow, while the rim is left thick and firm enough to hold food. The crust is dry and browned across the upper edge. The crust is firm and dry, with a shallow centre pressed down to take food.",
			SizeCategory.Small,
			ItemQuality.Standard,
			240.0,
			2.0m,
			true,
			false,
			"bread",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Edible flat trencher suited to northern bread or oatcake service."
		);

		CreateItem(
			"medieval_tableware_shared_northern_north_sea_horn_feast_set_tray",
			"tray",
			"a horn-cup feast tray",
			null,
			"This horn-cup feast tray is a medium-sized, well-made tray built from oak boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1500.0,
			30.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Tray shaped to carry drinking horns or cups at a feast."
		);

		CreateItem(
			"medieval_tableware_shared_northern_north_sea_ship_carved_drinking_bowl",
			"bowl",
			"a ship-carved drinking bowl",
			null,
			"This ship-carved drinking bowl is a small, well-made bowl built from pine boards. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			360.0,
			22.0m,
			true,
			false,
			"pine",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Pint"
			],
			null,
			null,
			null,
			null,
			"Decorated wooden drinking bowl suited to coastal hall culture."
		);

		CreateItem(
			"medieval_tableware_shared_northern_north_sea_smoked_fish_platter",
			"platter",
			"a smoked-fish platter",
			null,
			"This smoked-fish platter is a medium-sized, workmanlike platter built from birch boards. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			10.0m,
			true,
			false,
			"birch",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Long light platter for smoked fish or preserved foods."
		);

		CreateItem(
			"medieval_tableware_shared_northern_north_sea_stave_ale_tankard",
			"tankard",
			"a hooped stave tankard",
			null,
			"This hooped stave tankard is a small, workmanlike tankard built from oak boards. A broad handle joins the side, and the open rim sits above a thickened base. The inside is smooth from repeated drink service. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			700.0,
			16.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_UKPint"
			],
			null,
			null,
			null,
			null,
			"North-sea style stave-built tankard for ale or mead."
		);

		CreateItem(
			"medieval_tableware_shared_western_european_gilded_court_goblet",
			"goblet",
			"a gilded court goblet",
			null,
			"This gilded court goblet is a small, finely made goblet worked from gold. A rounded bowl rises from a narrow stem and foot, leaving the rim clean and prominent. The base is weighted enough to stand firmly. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.VeryGood,
			500.0,
			450.0m,
			true,
			false,
			"gold",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_WineGlass"
			],
			null,
			null,
			null,
			null,
			"Precious court-facing goblet for elite halls and treasuries."
		);

		CreateItem(
			"medieval_tableware_shared_western_european_manor_wash_set_tray",
			"tray",
			"a manor wash-set tray",
			null,
			"This manor wash-set tray is a medium-sized, well-made tray worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1800.0,
			60.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Metal tray meant to support an ewer-and-basin service set."
		);

		CreateItem(
			"medieval_tableware_shared_western_european_monastic_refectory_trencher",
			"trencher",
			"a refectory trencher",
			null,
			"This refectory trencher is a small, workmanlike trencher built from beech boards. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			5.0m,
			true,
			false,
			"beech",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Plain reusable trencher for communal dining."
		);

		CreateItem(
			"medieval_tableware_shared_western_european_pewter_dinner_plate",
			"plate",
			"a pewter dinner plate",
			null,
			"This pewter dinner plate is a small, well-made plate worked from pewter. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			720.0,
			38.0m,
			true,
			false,
			"pewter",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
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
			"Higher-status metal plate for urban or manorial service."
		);

		CreateItem(
			"medieval_tableware_shared_western_european_pewter_salt_cellar",
			"cellar",
			"a pewter salt cellar",
			null,
			"This pewter salt cellar is a very small, well-made cellar worked from pewter. A small hollow sits inside a thick rim, with a flat base beneath it. The lip is polished where fingers have lifted it from the table. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			420.0,
			42.0m,
			true,
			false,
			"pewter",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
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
			"Prominent table salt vessel; open-container profile."
		);

		CreateItem(
			"medieval_tableware_shared_western_european_tin_glazed_serving_dish",
			"dish",
			"a tin-glazed serving dish",
			null,
			"This tin-glazed serving dish is a medium-sized, well-made dish formed from ceramic. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1300.0,
			38.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Bright glazed serving dish for wealthy western tables."
		);

		CreateItem(
			"medieval_tableware_shared_mediterranean_bronze_handwashing_basin",
			"basin",
			"a bronze handwashing basin",
			null,
			"This bronze handwashing basin is a medium-sized, well-made basin worked from bronze. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			58.0m,
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
				"LContainer_Amphora_Urna"
			],
			null,
			null,
			null,
			null,
			"Basin suited to formal handwashing before meals."
		);

		CreateItem(
			"medieval_tableware_shared_mediterranean_marble_fruit_platter",
			"platter",
			"a marble fruit platter",
			null,
			"This marble fruit platter is a medium-sized, well-made platter cut from marble. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			75.0m,
			true,
			false,
			"marble",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Heavy luxury platter for fruit or sweetmeats."
		);

		CreateItem(
			"medieval_tableware_shared_mediterranean_olive_oil_cruet",
			"cruet",
			"an olive-oil cruet",
			null,
			"This olive-oil cruet is a very small, well-made cruet made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			210.0,
			30.0m,
			true,
			false,
			"glass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Small vessel for olive oil at table or in the kitchen."
		);

		CreateItem(
			"medieval_tableware_shared_mediterranean_painted_table_amphora",
			"amphora",
			"a painted table amphora",
			null,
			"This painted table amphora is a medium-sized, well-made amphora formed from ceramic. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2800.0,
			38.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Congius"
			],
			null,
			null,
			null,
			null,
			"Decorated smaller amphora for table or pantry service."
		);

		CreateItem(
			"medieval_tableware_shared_mediterranean_shallow_oil_dish",
			"dish",
			"a shallow oil dish",
			null,
			"This shallow oil dish is a very small, workmanlike dish formed from earthenware. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			260.0,
			5.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
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
			"Small open dish for oil-soaked bread, olives, or condiments."
		);

		CreateItem(
			"medieval_tableware_shared_mediterranean_terracotta_water_amphora",
			"amphora",
			"a terracotta water amphora",
			null,
			"This terracotta water amphora is a large, workmanlike amphora formed from terracotta. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9400.0,
			30.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
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
			"Mediterranean-style household water amphora."
		);

		CreateItem(
			"medieval_tableware_shared_mediterranean_two_handled_wine_jar",
			"jar",
			"a two-handled wine jar",
			null,
			"This two-handled wine jar is a medium-sized, workmanlike jar formed from terracotta. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3600.0,
			20.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Congius"
			],
			null,
			null,
			null,
			null,
			"Handled ceramic jar for wine or watered wine."
		);

		CreateItem(
			"medieval_tableware_shared_mediterranean_wine_mixing_bowl",
			"bowl",
			"a wine-mixing bowl",
			null,
			"This wine-mixing bowl is a medium-sized, well-made bowl formed from ceramic. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2400.0,
			36.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Congius"
			],
			null,
			null,
			null,
			null,
			"Open-looking but liquid-holding bowl for mixing or serving wine."
		);

		CreateItem(
			"medieval_tableware_shared_islamicate_urban_brass_wash_basin",
			"basin",
			"an engraved brass basin",
			null,
			"This engraved brass basin is a medium-sized, well-made basin worked from brass. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3400.0,
			58.0m,
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
				"LContainer_Amphora_Urna"
			],
			null,
			null,
			null,
			null,
			"Fine basin for washing, guest service, or table ritual."
		);

		CreateItem(
			"medieval_tableware_shared_islamicate_urban_date_serving_tray",
			"tray",
			"a date-serving tray",
			null,
			"This date-serving tray is a small, well-made tray worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			660.0,
			30.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Metal tray for dates, sweetmeats, or small guest offerings."
		);

		CreateItem(
			"medieval_tableware_shared_islamicate_urban_engraved_brass_ewer",
			"ewer",
			"an engraved brass ewer",
			null,
			"This engraved brass ewer is a small, well-made ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1450.0,
			55.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Fine engraved pouring vessel for water or scented drink."
		);

		CreateItem(
			"medieval_tableware_shared_islamicate_urban_flatbread_serving_platter",
			"platter",
			"a flatbread serving platter",
			null,
			"This flatbread serving platter is a medium-sized, workmanlike platter built from plain wood. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			780.0,
			8.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Low platter for flatbread, dates, and shared table foods."
		);

		CreateItem(
			"medieval_tableware_shared_islamicate_urban_glazed_sherbet_cup",
			"cup",
			"a glazed sherbet cup",
			null,
			"This glazed sherbet cup is a very small, well-made cup formed from ceramic. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			220.0,
			20.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_SmallWineGlass"
			],
			null,
			null,
			null,
			null,
			"Small glazed cup for sweetened drinks, cordials, or cool water."
		);

		CreateItem(
			"medieval_tableware_shared_islamicate_urban_long_necked_ceramic_jug",
			"jug",
			"a long-necked ceramic jug",
			null,
			"This long-necked ceramic jug is a small, well-made jug formed from ceramic. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1200.0,
			28.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Long-necked jug form for water or sherbet-like drinks."
		);

		CreateItem(
			"medieval_tableware_shared_islamicate_urban_lusterware_bowl",
			"bowl",
			"a lustred ceramic bowl",
			null,
			"This lustred ceramic bowl is a small, well-made bowl formed from ceramic. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			620.0,
			34.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Fine decorated bowl for table presentation; dry/open container profile."
		);

		CreateItem(
			"medieval_tableware_shared_islamicate_urban_rosewater_flask",
			"flask",
			"a rosewater flask",
			null,
			"This rosewater flask is a very small, well-made flask made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			160.0,
			32.0m,
			true,
			false,
			"glass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Small glass flask for rosewater or scented liquid."
		);

		CreateItem(
			"medieval_tableware_shared_islamicate_urban_water_cooling_jar",
			"jar",
			"a porous water-cooling jar",
			null,
			"This porous water-cooling jar is a large, workmanlike jar formed from earthenware. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			7600.0,
			24.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
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
			"Porous ceramic water jar for domestic cooling and storage."
		);

		CreateItem(
			"medieval_tableware_shared_islamicate_urban_wide_couscous_bowl",
			"bowl",
			"a wide grain bowl",
			null,
			"This wide grain bowl is a medium-sized, workmanlike bowl formed from earthenware. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			14.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
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
			"Wide open bowl for grain, pottage, or shared thick foods."
		);

		CreateItem(
			"medieval_tableware_shared_south_asian_banana_leaf_tray",
			"tray",
			"a broad leaf tray",
			null,
			"This broad leaf tray is a medium-sized, workmanlike tray made from layered leaf fibre. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			120.0,
			1.0m,
			true,
			false,
			"leaf",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Large disposable serving tray made from broad leaves."
		);

		CreateItem(
			"medieval_tableware_shared_south_asian_brass_lota",
			"lota",
			"a brass lota vessel",
			null,
			"This brass lota vessel is a small, well-made lota worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			700.0,
			32.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Rounded water vessel for drinking, washing, or domestic pouring."
		);

		CreateItem(
			"medieval_tableware_shared_south_asian_copper_lota",
			"lota",
			"a copper lota vessel",
			null,
			"This copper lota vessel is a small, well-made lota worked from copper. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			720.0,
			34.0m,
			true,
			false,
			"copper",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Rounded copper water vessel for household use."
		);

		CreateItem(
			"medieval_tableware_shared_south_asian_curry_bowl",
			"bowl",
			"a small curry bowl",
			null,
			"This small curry bowl is a small, workmanlike bowl formed from earthenware. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			6.0m,
			true,
			false,
			"earthenware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
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
			"Small open bowl for thick stews, sauces, or side dishes."
		);

		CreateItem(
			"medieval_tableware_shared_south_asian_ghee_flask",
			"flask",
			"a ghee flask",
			null,
			"This ghee flask is a very small, well-made flask worked from brass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			300.0,
			24.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Flask"
			],
			null,
			null,
			null,
			null,
			"Small vessel for clarified butter or oil."
		);

		CreateItem(
			"medieval_tableware_shared_south_asian_leaf_platter",
			"platter",
			"a stitched leaf platter",
			null,
			"This stitched leaf platter is a small, workmanlike platter made from layered leaf fibre. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Its surfaces are plain, serviceable, and visibly shaped for repeated household handling.",
			SizeCategory.Small,
			ItemQuality.Standard,
			80.0,
			1.0m,
			true,
			false,
			"leaf",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Disposable or cheap leaf platter for simple food service."
		);

		CreateItem(
			"medieval_tableware_shared_south_asian_spice_cup_set",
			"cup",
			"a small spice cup",
			null,
			"This small spice cup is a very small, well-made cup worked from brass. A small hollow sits inside a thick rim, with a flat base beneath it. The lip is polished where fingers have lifted it from the table. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			160.0,
			16.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
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
			"Small metal cup for dry spices or condiments; open-container profile."
		);

		CreateItem(
			"medieval_tableware_shared_south_asian_stone_grinding_bowl",
			"bowl",
			"a stone condiment bowl",
			null,
			"This stone condiment bowl is a small, workmanlike bowl cut from soapstone. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1100.0,
			14.0m,
			true,
			false,
			"soapstone",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Heavy bowl for chutneys, salt, or thick condiments; not a tool component."
		);

		CreateItem(
			"medieval_tableware_shared_south_asian_thali_platter",
			"platter",
			"a round thali platter",
			null,
			"This round thali platter is a medium-sized, well-made platter worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1500.0,
			42.0m,
			true,
			false,
			"brass",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Broad metal platter for several small dishes at once."
		);

		CreateItem(
			"medieval_tableware_shared_south_asian_water_pot",
			"pot",
			"a red clay water pot",
			null,
			"This red clay water pot is a large, workmanlike pot formed from terracotta. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8200.0,
			20.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
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
			"Large water pot for domestic storage and cooling."
		);

		CreateItem(
			"medieval_tableware_shared_east_asian_bamboo_serving_tray",
			"tray",
			"a bamboo serving tray",
			null,
			"This bamboo serving tray is a medium-sized, workmanlike tray built from split bamboo. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			480.0,
			10.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Light tray for cups, bowls, and tea or meal service."
		);

		CreateItem(
			"medieval_tableware_shared_east_asian_bamboo_steaming_tray",
			"tray",
			"a bamboo steaming tray",
			null,
			"This bamboo steaming tray is a medium-sized, workmanlike tray built from split bamboo. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			600.0,
			12.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Tray-like food vessel for dumplings, buns, or steamed foods; does not provide heat mechanics."
		);

		CreateItem(
			"medieval_tableware_shared_east_asian_celadon_table_bowl",
			"bowl",
			"a celadon table bowl",
			null,
			"This celadon table bowl is a small, well-made bowl formed from ceramic. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			420.0,
			34.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Greenish fine ceramic bowl for refined table service."
		);

		CreateItem(
			"medieval_tableware_shared_east_asian_ceramic_teapot",
			"pot",
			"a ceramic tea pot",
			null,
			"This ceramic tea pot is a small, well-made pot formed from ceramic. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			720.0,
			38.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Small pouring pot using a jug capacity profile."
		);

		CreateItem(
			"medieval_tableware_shared_east_asian_covered_food_bowl",
			"bowl",
			"a covered food bowl",
			null,
			"This covered food bowl is a small, well-made bowl formed from porcelain. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			420.0,
			38.0m,
			true,
			false,
			"porcelain",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Covered-looking bowl for rice or prepared foods; cover is descriptive only."
		);

		CreateItem(
			"medieval_tableware_shared_east_asian_low_lacquered_tray",
			"tray",
			"a low lacquered tray",
			null,
			"This low lacquered tray is a medium-sized, well-made tray built from plain wood. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			720.0,
			32.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Low polished tray; lacquer effect is presentation rather than a material entry."
		);

		CreateItem(
			"medieval_tableware_shared_east_asian_porcelain_rice_bowl",
			"bowl",
			"a porcelain rice bowl",
			null,
			"This porcelain rice bowl is a small, well-made bowl formed from porcelain. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			340.0,
			32.0m,
			true,
			false,
			"porcelain",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
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
			"Fine bowl for rice or small foods; dry/open container profile."
		);

		CreateItem(
			"medieval_tableware_shared_east_asian_rice_wine_flask",
			"flask",
			"a rice-wine flask",
			null,
			"This rice-wine flask is a very small, well-made flask formed from ceramic. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			260.0,
			24.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Small flask for rice wine or other table drink."
		);

		CreateItem(
			"medieval_tableware_shared_east_asian_small_tea_cup",
			"cup",
			"a small tea cup",
			null,
			"This small tea cup is a tiny, well-made cup formed from porcelain. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			90.0,
			24.0m,
			true,
			false,
			"porcelain",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Pony"
			],
			null,
			null,
			null,
			null,
			"Tiny cup for tea, medicine, or refined drink service."
		);

		CreateItem(
			"medieval_tableware_shared_east_asian_soy_cruet",
			"cruet",
			"a small sauce cruet",
			null,
			"This small sauce cruet is a very small, well-made cruet formed from ceramic. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			160.0,
			18.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
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
			"Small pouring vessel for sauce, vinegar, or oil."
		);

		CreateItem(
			"medieval_tableware_shared_east_asian_stoneware_tea_cup",
			"cup",
			"a stoneware tea cup",
			null,
			"This stoneware tea cup is a very small, workmanlike cup formed from stoneware. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			160.0,
			12.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_WhiskeyGlass"
			],
			null,
			null,
			null,
			null,
			"Small cup for tea or warm drink service."
		);

		CreateItem(
			"medieval_tableware_shared_east_asian_wide_noodle_bowl",
			"bowl",
			"a wide noodle bowl",
			null,
			"This wide noodle bowl is a small, workmanlike bowl formed from stoneware. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			10.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
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
			"Wide bowl for noodles or grain dishes; not a liquid-storage container."
		);

		CreateItem(
			"medieval_tableware_shared_steppe_and_caravan_camp_drinking_bowl",
			"bowl",
			"a camp drinking bowl",
			null,
			"This camp drinking bowl is a small, workmanlike bowl built from plain wood. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			240.0,
			6.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Portable wooden bowl for water, milk, or broth."
		);

		CreateItem(
			"medieval_tableware_shared_steppe_and_caravan_felt_wrapped_canteen",
			"canteen",
			"a felt-wrapped canteen",
			null,
			"This felt-wrapped canteen is a small, workmanlike canteen built from plain wood. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			18.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Canteen"
			],
			null,
			null,
			null,
			null,
			"Flat canteen with protective wrap described visually."
		);

		CreateItem(
			"medieval_tableware_shared_steppe_and_caravan_hide_waterskin",
			"waterskin",
			"a hide waterskin",
			null,
			"This hide waterskin is a small, workmanlike waterskin made from cured animal skin. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			13.0m,
			true,
			false,
			"animal skin",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Waterskin"
			],
			null,
			null,
			null,
			null,
			"Travel water vessel suited to mounted or caravan households."
		);

		CreateItem(
			"medieval_tableware_shared_steppe_and_caravan_leather_kumis_flask",
			"flask",
			"a leather kumis flask",
			null,
			"This leather kumis flask is a small, workmanlike flask made from worked leather. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			14.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Flask"
			],
			null,
			null,
			null,
			null,
			"Portable flask for fermented milk or other travelling drink."
		);

		CreateItem(
			"medieval_tableware_shared_steppe_and_caravan_portable_cheese_tray",
			"tray",
			"a portable cheese tray",
			null,
			"This portable cheese tray is a small, workmanlike tray made from worked leather. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			8.0m,
			true,
			false,
			"leather",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Light tray or mat-like surface for cheese, curds, or travel foods."
		);

		CreateItem(
			"medieval_tableware_shared_steppe_and_caravan_small_fermenting_skin",
			"skin",
			"a small fermenting skin",
			null,
			"This small fermenting skin is a medium-sized, workmanlike skin made from cured animal skin. The vessel has a shaped belly, a clear mouth, and a steady base. The interior is smooth enough to hold liquid without catching residue. Creases, darkened edges, and firm stitching show where hands have flexed the material.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			20.0m,
			true,
			false,
			"animal skin",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Fermenting Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_GallonCask"
			],
			null,
			null,
			null,
			null,
			"Skin vessel for fermenting or carrying dairy drink."
		);

		CreateItem(
			"medieval_tableware_shared_steppe_and_caravan_travel_meat_board",
			"board",
			"a travel meat board",
			null,
			"This travel meat board is a small, workmanlike board built from birch boards. A flat working face spans the board, with bevelled edges and a plain underside. Knife marks and rubbed patches cross the upper surface. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			6.0m,
			true,
			false,
			"birch",
			[
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Compact board for cutting or serving dried meat and cheese."
		);

		CreateItem(
			"medieval_tableware_shared_steppe_and_caravan_wooden_saddle_cup",
			"cup",
			"a saddle-carried wooden cup",
			null,
			"This saddle-carried wooden cup is a very small, workmanlike cup built from plain wood. The open rim is smooth, the sides taper slightly, and the base is firm enough for table use. The inside is shaped as a single clean drinking hollow. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			160.0,
			7.0m,
			true,
			false,
			"wood",
			[
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Food and Drink / Vessels / Beverage Serving Vessel",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_DrinkingGlass"
			],
			null,
			null,
			null,
			null,
			"Small rugged cup for travel, herding, or caravan use."
		);
	}
}
