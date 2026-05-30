#nullable enable

using MudSharp.Form.Material;
using MudSharp.GameItems;
using System;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalFoodAndBeverageItems()
	{
		#region Early Anglo-Saxon/Insular

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_early_anglo_saxon_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: oat cakes, ale, and pottage. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Food cue: oat cakes, ale, and pottage."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_early_anglo_saxon_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: oat cakes, ale, and pottage. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_early_anglo_saxon_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by oat cakes, ale, and pottage.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_early_anglo_saxon_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: oat cakes, ale, and pottage.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_early_anglo_saxon_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by oat cakes, ale, and pottage.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_early_anglo_saxon_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: oat cakes, ale, and pottage.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_early_anglo_saxon_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of oat cakes, ale, and pottage for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Early Anglo-Saxon-Insular",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Early Anglo-Saxon/Insular. Market ration cue."
		);

		#endregion

		#region Late Anglo-Saxon/Anglo-Danish

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_anglo_danish_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: rye bread, smoked fish, and ale. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Food cue: rye bread, smoked fish, and ale."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_anglo_danish_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: rye bread, smoked fish, and ale. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_anglo_danish_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by rye bread, smoked fish, and ale.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_anglo_danish_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: rye bread, smoked fish, and ale.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_anglo_danish_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by rye bread, smoked fish, and ale.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_anglo_danish_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: rye bread, smoked fish, and ale.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_anglo_danish_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of rye bread, smoked fish, and ale for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Late Anglo-Saxon-Anglo-Danish",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Late Anglo-Saxon/Anglo-Danish. Market ration cue."
		);

		#endregion

		#region Norse

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_norse_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: flatbread, stockfish, and sour milk. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Food cue: flatbread, stockfish, and sour milk."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_norse_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: flatbread, stockfish, and sour milk. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_norse_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by flatbread, stockfish, and sour milk.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_norse_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: flatbread, stockfish, and sour milk.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_norse_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by flatbread, stockfish, and sour milk.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_norse_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: flatbread, stockfish, and sour milk.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_norse_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of flatbread, stockfish, and sour milk for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norse",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norse. Market ration cue."
		);

		#endregion

		#region Norman/Angevin

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_norman_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: wheaten bread, wine, and stewed meat. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Food cue: wheaten bread, wine, and stewed meat."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_norman_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: wheaten bread, wine, and stewed meat. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_norman_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by wheaten bread, wine, and stewed meat.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_norman_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: wheaten bread, wine, and stewed meat.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_norman_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by wheaten bread, wine, and stewed meat.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_norman_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: wheaten bread, wine, and stewed meat.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_norman_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of wheaten bread, wine, and stewed meat for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Norman-Angevin",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Norman/Angevin. Market ration cue."
		);

		#endregion

		#region High Medieval Britain/Marcher

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_high_british_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: trencher bread, ale, and cheese. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Food cue: trencher bread, ale, and cheese."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_high_british_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: trencher bread, ale, and cheese. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_high_british_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by trencher bread, ale, and cheese.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_high_british_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: trencher bread, ale, and cheese.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_high_british_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by trencher bread, ale, and cheese.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_high_british_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: trencher bread, ale, and cheese.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_high_british_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of trencher bread, ale, and cheese for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / High Medieval Britain-Marcher",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: High Medieval Britain/Marcher. Market ration cue."
		);

		#endregion

		#region Gaelic/Welsh/Highland

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_gaelic_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: oat bread, curds, and smoked meat. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Food cue: oat bread, curds, and smoked meat."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_gaelic_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: oat bread, curds, and smoked meat. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_gaelic_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by oat bread, curds, and smoked meat.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_gaelic_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: oat bread, curds, and smoked meat.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_gaelic_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by oat bread, curds, and smoked meat.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_gaelic_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: oat bread, curds, and smoked meat.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_gaelic_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of oat bread, curds, and smoked meat for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Gaelic-Welsh-Highland",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Gaelic/Welsh/Highland. Market ration cue."
		);

		#endregion

		#region Carolingian/Frankish

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_carolingian_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: barley bread, pork, and ale. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Food cue: barley bread, pork, and ale."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_carolingian_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: barley bread, pork, and ale. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_carolingian_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by barley bread, pork, and ale.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_carolingian_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: barley bread, pork, and ale.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_carolingian_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by barley bread, pork, and ale.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_carolingian_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: barley bread, pork, and ale.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_carolingian_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of barley bread, pork, and ale for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Carolingian-Frankish",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Carolingian/Frankish. Market ration cue."
		);

		#endregion

		#region Capetian/Low Countries

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_capetian_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: white bread, onions, and wine. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Food cue: white bread, onions, and wine."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_capetian_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: white bread, onions, and wine. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_capetian_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by white bread, onions, and wine.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_capetian_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: white bread, onions, and wine.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_capetian_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by white bread, onions, and wine.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_capetian_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: white bread, onions, and wine.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_capetian_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of white bread, onions, and wine for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Capetian-Low Countries",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Capetian/Low Countries. Market ration cue."
		);

		#endregion

		#region German/HRE/Alpine-North Italian

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_german_hre_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: rye bread, sausage, and beer. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Food cue: rye bread, sausage, and beer."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_german_hre_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: rye bread, sausage, and beer. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_german_hre_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by rye bread, sausage, and beer.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_german_hre_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: rye bread, sausage, and beer.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_german_hre_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by rye bread, sausage, and beer.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_german_hre_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: rye bread, sausage, and beer.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_german_hre_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of rye bread, sausage, and beer for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / German-HRE-Alpine-North Italian",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: German/HRE/Alpine-North Italian. Market ration cue."
		);

		#endregion

		#region Iberian Christian

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_iberian_christian_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: wheat bread, olives, and wine. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Food cue: wheat bread, olives, and wine."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_iberian_christian_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: wheat bread, olives, and wine. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_iberian_christian_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by wheat bread, olives, and wine.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_iberian_christian_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: wheat bread, olives, and wine.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_iberian_christian_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by wheat bread, olives, and wine.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_iberian_christian_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: wheat bread, olives, and wine.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_iberian_christian_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of wheat bread, olives, and wine for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Iberian Christian",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Iberian Christian. Market ration cue."
		);

		#endregion

		#region al-Andalus/Maghreb

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_andalusi_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: flatbread, oil, dates, and spiced stew. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Food cue: flatbread, oil, dates, and spiced stew."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_andalusi_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: flatbread, oil, dates, and spiced stew. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_andalusi_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by flatbread, oil, dates, and spiced stew.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_andalusi_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: flatbread, oil, dates, and spiced stew.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_andalusi_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by flatbread, oil, dates, and spiced stew.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_andalusi_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: flatbread, oil, dates, and spiced stew.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_andalusi_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of flatbread, oil, dates, and spiced stew for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / al-Andalus-Maghreb",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: al-Andalus/Maghreb. Market ration cue."
		);

		#endregion

		#region Byzantine

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_byzantine_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: wheat bread, wine, olives, and fish sauce. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Food cue: wheat bread, wine, olives, and fish sauce."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_byzantine_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: wheat bread, wine, olives, and fish sauce. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_byzantine_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by wheat bread, wine, olives, and fish sauce.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_byzantine_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: wheat bread, wine, olives, and fish sauce.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_byzantine_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by wheat bread, wine, olives, and fish sauce.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_byzantine_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: wheat bread, wine, olives, and fish sauce.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_byzantine_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of wheat bread, wine, olives, and fish sauce for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Byzantine",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Byzantine. Market ration cue."
		);

		#endregion

		#region Abbasid/Persianate

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_abbasid_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: rice, flatbread, sour milk, and syrups. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Food cue: rice, flatbread, sour milk, and syrups."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_abbasid_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: rice, flatbread, sour milk, and syrups. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_abbasid_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by rice, flatbread, sour milk, and syrups.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_abbasid_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: rice, flatbread, sour milk, and syrups.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_abbasid_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by rice, flatbread, sour milk, and syrups.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_abbasid_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: rice, flatbread, sour milk, and syrups.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_abbasid_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of rice, flatbread, sour milk, and syrups for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Abbasid-Persianate",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Abbasid/Persianate. Market ration cue."
		);

		#endregion

		#region Fatimid Egypt/Ifriqiya

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_fatimid_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: flatbread, lentils, dates, and oil. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Food cue: flatbread, lentils, dates, and oil."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_fatimid_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: flatbread, lentils, dates, and oil. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_fatimid_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by flatbread, lentils, dates, and oil.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_fatimid_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: flatbread, lentils, dates, and oil.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_fatimid_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by flatbread, lentils, dates, and oil.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_fatimid_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: flatbread, lentils, dates, and oil.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_fatimid_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of flatbread, lentils, dates, and oil for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Fatimid Egypt-Ifriqiya",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Fatimid Egypt/Ifriqiya. Market ration cue."
		);

		#endregion

		#region Seljuk/Ayyubid/early Mamluk

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_seljuk_ayyubid_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: flatbread, yogurt, pilaf, and spiced meat. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Food cue: flatbread, yogurt, pilaf, and spiced meat."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_seljuk_ayyubid_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: flatbread, yogurt, pilaf, and spiced meat. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_seljuk_ayyubid_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by flatbread, yogurt, pilaf, and spiced meat.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_seljuk_ayyubid_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: flatbread, yogurt, pilaf, and spiced meat.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_seljuk_ayyubid_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by flatbread, yogurt, pilaf, and spiced meat.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_seljuk_ayyubid_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: flatbread, yogurt, pilaf, and spiced meat.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_seljuk_ayyubid_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of flatbread, yogurt, pilaf, and spiced meat for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Seljuk-Ayyubid-early Mamluk",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Seljuk/Ayyubid/early Mamluk. Market ration cue."
		);

		#endregion

		#region Kyivan Rus/Novgorod

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_rus_novgorod_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: rye bread, fish, mushrooms, and honey drink. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Food cue: rye bread, fish, mushrooms, and honey drink."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_rus_novgorod_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: rye bread, fish, mushrooms, and honey drink. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_rus_novgorod_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by rye bread, fish, mushrooms, and honey drink.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_rus_novgorod_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: rye bread, fish, mushrooms, and honey drink.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_rus_novgorod_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by rye bread, fish, mushrooms, and honey drink.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_rus_novgorod_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: rye bread, fish, mushrooms, and honey drink.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_rus_novgorod_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of rye bread, fish, mushrooms, and honey drink for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Kyivan Rus-Novgorod",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Kyivan Rus/Novgorod. Market ration cue."
		);

		#endregion

		#region Steppe Turkic/Cuman/Mongol-adjacent

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_steppe_turkic_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: millet, dried curds, meat, and fermented milk. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Food cue: millet, dried curds, meat, and fermented milk."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_steppe_turkic_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: millet, dried curds, meat, and fermented milk. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_steppe_turkic_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by millet, dried curds, meat, and fermented milk.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_steppe_turkic_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: millet, dried curds, meat, and fermented milk.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_steppe_turkic_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by millet, dried curds, meat, and fermented milk.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_steppe_turkic_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: millet, dried curds, meat, and fermented milk.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_steppe_turkic_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of millet, dried curds, meat, and fermented milk for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Steppe Turkic-Cuman-Mongol-adjacent",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Steppe Turkic/Cuman/Mongol-adjacent. Market ration cue."
		);

		#endregion

		#region Song China

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Food and Drink / Medieval Food / Prepared Foods"
			]
		);

		CreateItem(
			"medieval_food_song_china_meal_platter",
			"platter",
			"a regional medieval meal platter",
			null,
			"This meal platter groups reusable foodway cues for the slice: rice, wheat noodles, tea, and pickled greens. It is a prepared-food stock example rather than a complete cuisine system.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1200.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Food and Drink / Medieval Food / Prepared Foods"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Food cue: rice, wheat noodles, tea, and pickled greens."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Food and Drink / Medieval Food / Breads"
			]
		);

		CreateItem(
			"medieval_food_song_china_staple_bread",
			"bread",
			"a regional staple bread board",
			null,
			"This board presents the staple bread side of the slice's foodway cue: rice, wheat noodles, tea, and pickled greens. It is suitable for inn tables, ration stock, or household meals.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			8.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Food and Drink / Medieval Food / Breads"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Staple bread cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_song_china_pottage_bowl",
			"bowl",
			"a regional pottage bowl",
			null,
			"This earthenware bowl is set up for stews, porridges, sauces, broths, or other everyday cooked food suggested by rice, wheat noodles, tea, and pickled greens.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			7.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"Container_Plate",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Everyday bowl cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Food and Drink / Medieval Food / Preserving"
			]
		);

		CreateItem(
			"medieval_food_song_china_preserved_provision",
			"packet",
			"a wrapped preserved provision packet",
			null,
			"This tied provision packet represents the preserved, dried, smoked, salted, or travel-ready side of the slice's foodway cue: rice, wheat noodles, tea, and pickled greens.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			9.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Food and Drink / Medieval Food / Preserving"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Preserved provision cue."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_song_china_drinking_vessel",
			"cup",
			"a regional drinking vessel",
			null,
			"This drinking vessel gives the culture slice a basic cup or small jug surface for ale, wine, sour milk, tea, water, or syrups suggested by rice, wheat noodles, tea, and pickled greens.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			8.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Drinking vessel cue."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_song_china_feast_dish",
			"dish",
			"a regional feast dish",
			null,
			"This larger dish gives hosts, halls, courts, and monasteries a more generous version of the slice's foodway cue: rice, wheat noodles, tea, and pickled greens.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			24.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Feast dish cue."
		);

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_song_china_market_ration",
			"ration",
			"a wrapped market ration",
			null,
			"This compact ration bundles the portable, purchasable side of rice, wheat noodles, tea, and pickled greens for soldiers, workers, travellers, and market scenes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			10.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Eras / Medieval / Cultures / Song China",
				"Food and Drink / Medieval Food / Prepared Foods",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval culture slice: Song China. Market ration cue."
		);

		#endregion

		#region Common Food and Beverage

		EnsureMedievalItemMaterialAndTags("linen", MaterialBehaviourType.Fabric,
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Grain",
				"Functions / Tools / Measurement Tools"
			]
		);

		CreateItem(
			"medieval_food_grain_measure_sack",
			"sack",
			"a measured grain sack",
			null,
			"This tied grain sack is marked for standard portions and built to be paired with weight and grain-measure instruments.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			9000.0,
			18.0m,
			false,
			false,
			"linen",
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Grain",
				"Functions / Tools / Measurement Tools"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Medieval food or beverage item."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Vessels",
				"Functions / Tools / Measurement Tools"
			]
		);

		CreateItem(
			"medieval_food_wine_measure_jug",
			"jug",
			"a marked wine measure jug",
			null,
			"This marked wine jug has a narrow neck and scored fill lines for tavern, monastic, or customs-house measures.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1100.0,
			14.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Vessels",
				"Functions / Tools / Measurement Tools"
			],
			[
				"Holdable",
				"LContainer_WineGlass",
				"MeasuringInstrument_Antiquity_WineCup",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Vessels",
				"Functions / Tools / Measurement Tools"
			]
		);

		CreateItem(
			"medieval_food_oil_measure_jug",
			"jug",
			"a marked oil measure jug",
			null,
			"This small oil jug has a pinched lip and scored measure marks for kitchen, market, and customs work.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			12.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Vessels",
				"Functions / Tools / Measurement Tools"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"MeasuringInstrument_Antiquity_OilCup",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Uses the live SealStamp, Sealable, or MeasuringInstrument component prototypes seeded by UsefulSeeder."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Dairy",
				"Market / Professional Tools / Standard Tools"
			]
		);

		CreateItem(
			"medieval_food_cheese_mould",
			"mould",
			"a slatted cheese mould",
			null,
			"This small slatted mould drains curds and gives cheese a portable market shape for dairies, kitchens, and monastery stores.",
			SizeCategory.Small,
			ItemQuality.Standard,
			780.0,
			14.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Dairy",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval food or beverage item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Dairy",
				"Market / Professional Tools / Standard Tools"
			]
		);

		CreateItem(
			"medieval_food_butter_churn",
			"churn",
			"a small butter churn",
			null,
			"This butter churn has a fitted lid, dasher, and tight staves for household, farm, or monastic dairy work.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			7200.0,
			28.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Dairy",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Container_Cupboard",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval food or beverage item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Brewing",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_ale_cask",
			"cask",
			"a small ale cask",
			null,
			"This small ale cask has tight staves, pitch-darkened seams, and enough capacity for tavern, hall, or monastery brewing scenes.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			15000.0,
			36.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Brewing",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"LContainer_Barrel",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval food or beverage item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Brewing",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_cider_cask",
			"cask",
			"a small cider cask",
			null,
			"This small cider cask is coopered for fruit drink storage, with a bung, hoop marks, and travel scuffs.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			15000.0,
			34.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Brewing",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"LContainer_Barrel",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval food or beverage item."
		);

		EnsureMedievalItemMaterialAndTags("earthenware", MaterialBehaviourType.Ceramic,
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Brewing",
				"Food and Drink / Medieval Food / Vessels"
			]
		);

		CreateItem(
			"medieval_food_mead_crock",
			"crock",
			"a sealed mead crock",
			null,
			"This pottery crock has a waxed cloth cover and a narrow mouth for mead, small beer, vinegar, or sweet syrups.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1800.0,
			18.0m,
			false,
			false,
			"earthenware",
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Brewing",
				"Food and Drink / Medieval Food / Vessels"
			],
			[
				"Holdable",
				"LContainer_DrinkingGlass",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			"Medieval food or beverage item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Baking",
				"Market / Professional Tools / Standard Tools"
			]
		);

		CreateItem(
			"medieval_food_bakers_peel",
			"peel",
			"a long baker's peel",
			null,
			"This long baker's peel has a flat wooden blade and a heat-darkened handle for hearth loaves, trenchers, and small pies.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1400.0,
			16.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Baking",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval food or beverage item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Baking",
				"Market / Professional Tools / Standard Tools"
			]
		);

		CreateItem(
			"medieval_food_bakers_tray",
			"tray",
			"a baker's wooden tray",
			null,
			"This broad tray is flour-dusted and shallow, suited to carrying loaves, trenchers, pies, or prepared market food.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			14.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Baking",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Container_Tray",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval food or beverage item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Preserving",
				"Market / Household Goods / Standard Wares"
			]
		);

		CreateItem(
			"medieval_food_salt_box",
			"box",
			"a small salt box",
			null,
			"This small lidded box keeps salt dry for kitchens, ships, infirmaries, and preserving tables.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			12.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Preserving",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval food or beverage item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Preserving",
				"Market / Household Goods / Luxury Wares"
			]
		);

		CreateItem(
			"medieval_food_spice_box",
			"box",
			"a partitioned spice box",
			null,
			"This small partitioned box has labelled compartments for costly spices, dried herbs, saffron threads, or apothecary overlap stock.",
			SizeCategory.Small,
			ItemQuality.Good,
			950.0,
			42.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Preserving",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Container_Pouch",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval food or beverage item."
		);

		EnsureMedievalItemMaterialAndTags("oak", MaterialBehaviourType.Wood,
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Brewing",
				"Market / Professional Tools / Standard Tools"
			]
		);

		CreateItem(
			"medieval_food_brewing_tub",
			"tub",
			"a wide brewing tub",
			null,
			"This wide tub has heavy staves, an open top, and room for mash, washing, soaking, or large kitchen preparation.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			46.0m,
			false,
			false,
			"oak",
			[
				"Eras / Medieval",
				"Food and Drink / Medieval Food / Brewing",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Container_Open_Bin",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null,
			"Medieval food or beverage item."
		);

		#endregion
	}
}
