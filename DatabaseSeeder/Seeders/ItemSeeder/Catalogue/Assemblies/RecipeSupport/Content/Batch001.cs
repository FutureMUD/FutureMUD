#nullable enable

using MudSharp.FutureProg;

namespace DatabaseSeeder.Seeders;

public static partial class RecipeSupportStock
{
	public static CatalogueRecipeProgSupport ItemSeederAntiquityFoodFishSauceRelish() => new(
		new("ItemSeederAntiquityFood_fish_sauce_relish", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederAntiquityFood_fish_sauce_relish", "Crafting", "Antiquity Foodways", ProgVariableTypes.Item,
		"Selects one Antiquity culture prepared-food prototype from the generalized foodways craft.",
		"var products as item collection\r\nadditem products loaditem(\"antiquity_food_hellenic_fish_sauce_relish\")\r\nadditem products loaditem(\"antiquity_food_egyptian_fish_sauce_relish\")\r\nadditem products loaditem(\"antiquity_food_roman_fish_sauce_relish\")\r\nadditem products loaditem(\"antiquity_food_celtic_fish_sauce_relish\")\r\nadditem products loaditem(\"antiquity_food_germanic_fish_sauce_relish\")\r\nadditem products loaditem(\"antiquity_food_kushite_fish_sauce_relish\")\r\nadditem products loaditem(\"antiquity_food_punic_fish_sauce_relish\")\r\nadditem products loaditem(\"antiquity_food_persian_fish_sauce_relish\")\r\nadditem products loaditem(\"antiquity_food_etruscan_fish_sauce_relish\")\r\nadditem products loaditem(\"antiquity_food_anatolian_fish_sauce_relish\")\r\nadditem products loaditem(\"antiquity_food_scythian_sarmatian_fish_sauce_relish\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederAntiquityFoodFlatbread() => new(
		new("ItemSeederAntiquityFood_flatbread", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederAntiquityFood_flatbread", "Crafting", "Antiquity Foodways", ProgVariableTypes.Item,
		"Selects one Antiquity culture prepared-food prototype from the generalized foodways craft.",
		"var products as item collection\r\nadditem products loaditem(\"antiquity_food_hellenic_flatbread\")\r\nadditem products loaditem(\"antiquity_food_egyptian_flatbread\")\r\nadditem products loaditem(\"antiquity_food_roman_flatbread\")\r\nadditem products loaditem(\"antiquity_food_celtic_flatbread\")\r\nadditem products loaditem(\"antiquity_food_germanic_flatbread\")\r\nadditem products loaditem(\"antiquity_food_kushite_flatbread\")\r\nadditem products loaditem(\"antiquity_food_punic_flatbread\")\r\nadditem products loaditem(\"antiquity_food_persian_flatbread\")\r\nadditem products loaditem(\"antiquity_food_etruscan_flatbread\")\r\nadditem products loaditem(\"antiquity_food_anatolian_flatbread\")\r\nadditem products loaditem(\"antiquity_food_scythian_sarmatian_flatbread\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederAntiquityFoodFruitPlatter() => new(
		new("ItemSeederAntiquityFood_fruit_platter", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederAntiquityFood_fruit_platter", "Crafting", "Antiquity Foodways", ProgVariableTypes.Item,
		"Selects one Antiquity culture prepared-food prototype from the generalized foodways craft.",
		"var products as item collection\r\nadditem products loaditem(\"antiquity_food_hellenic_fruit_platter\")\r\nadditem products loaditem(\"antiquity_food_egyptian_fruit_platter\")\r\nadditem products loaditem(\"antiquity_food_roman_fruit_platter\")\r\nadditem products loaditem(\"antiquity_food_celtic_fruit_platter\")\r\nadditem products loaditem(\"antiquity_food_germanic_fruit_platter\")\r\nadditem products loaditem(\"antiquity_food_kushite_fruit_platter\")\r\nadditem products loaditem(\"antiquity_food_punic_fruit_platter\")\r\nadditem products loaditem(\"antiquity_food_persian_fruit_platter\")\r\nadditem products loaditem(\"antiquity_food_etruscan_fruit_platter\")\r\nadditem products loaditem(\"antiquity_food_anatolian_fruit_platter\")\r\nadditem products loaditem(\"antiquity_food_scythian_sarmatian_fruit_platter\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederAntiquityFoodHoneyedPastry() => new(
		new("ItemSeederAntiquityFood_honeyed_pastry", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederAntiquityFood_honeyed_pastry", "Crafting", "Antiquity Foodways", ProgVariableTypes.Item,
		"Selects one Antiquity culture prepared-food prototype from the generalized foodways craft.",
		"var products as item collection\r\nadditem products loaditem(\"antiquity_food_hellenic_honeyed_pastry\")\r\nadditem products loaditem(\"antiquity_food_egyptian_honeyed_pastry\")\r\nadditem products loaditem(\"antiquity_food_roman_honeyed_pastry\")\r\nadditem products loaditem(\"antiquity_food_celtic_honeyed_pastry\")\r\nadditem products loaditem(\"antiquity_food_germanic_honeyed_pastry\")\r\nadditem products loaditem(\"antiquity_food_kushite_honeyed_pastry\")\r\nadditem products loaditem(\"antiquity_food_punic_honeyed_pastry\")\r\nadditem products loaditem(\"antiquity_food_persian_honeyed_pastry\")\r\nadditem products loaditem(\"antiquity_food_etruscan_honeyed_pastry\")\r\nadditem products loaditem(\"antiquity_food_anatolian_honeyed_pastry\")\r\nadditem products loaditem(\"antiquity_food_scythian_sarmatian_honeyed_pastry\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederAntiquityFoodMeatDish() => new(
		new("ItemSeederAntiquityFood_meat_dish", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederAntiquityFood_meat_dish", "Crafting", "Antiquity Foodways", ProgVariableTypes.Item,
		"Selects one Antiquity culture prepared-food prototype from the generalized foodways craft.",
		"var products as item collection\r\nadditem products loaditem(\"antiquity_food_hellenic_meat_dish\")\r\nadditem products loaditem(\"antiquity_food_egyptian_meat_dish\")\r\nadditem products loaditem(\"antiquity_food_roman_meat_dish\")\r\nadditem products loaditem(\"antiquity_food_celtic_meat_dish\")\r\nadditem products loaditem(\"antiquity_food_germanic_meat_dish\")\r\nadditem products loaditem(\"antiquity_food_kushite_meat_dish\")\r\nadditem products loaditem(\"antiquity_food_punic_meat_dish\")\r\nadditem products loaditem(\"antiquity_food_persian_meat_dish\")\r\nadditem products loaditem(\"antiquity_food_etruscan_meat_dish\")\r\nadditem products loaditem(\"antiquity_food_anatolian_meat_dish\")\r\nadditem products loaditem(\"antiquity_food_scythian_sarmatian_meat_dish\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederAntiquityFoodOilseedCake() => new(
		new("ItemSeederAntiquityFood_oilseed_cake", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederAntiquityFood_oilseed_cake", "Crafting", "Antiquity Foodways", ProgVariableTypes.Item,
		"Selects one Antiquity culture prepared-food prototype from the generalized foodways craft.",
		"var products as item collection\r\nadditem products loaditem(\"antiquity_food_hellenic_oilseed_cake\")\r\nadditem products loaditem(\"antiquity_food_egyptian_oilseed_cake\")\r\nadditem products loaditem(\"antiquity_food_roman_oilseed_cake\")\r\nadditem products loaditem(\"antiquity_food_celtic_oilseed_cake\")\r\nadditem products loaditem(\"antiquity_food_germanic_oilseed_cake\")\r\nadditem products loaditem(\"antiquity_food_kushite_oilseed_cake\")\r\nadditem products loaditem(\"antiquity_food_punic_oilseed_cake\")\r\nadditem products loaditem(\"antiquity_food_persian_oilseed_cake\")\r\nadditem products loaditem(\"antiquity_food_etruscan_oilseed_cake\")\r\nadditem products loaditem(\"antiquity_food_anatolian_oilseed_cake\")\r\nadditem products loaditem(\"antiquity_food_scythian_sarmatian_oilseed_cake\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederAntiquityFoodPorridge() => new(
		new("ItemSeederAntiquityFood_porridge", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederAntiquityFood_porridge", "Crafting", "Antiquity Foodways", ProgVariableTypes.Item,
		"Selects one Antiquity culture prepared-food prototype from the generalized foodways craft.",
		"var products as item collection\r\nadditem products loaditem(\"antiquity_food_hellenic_porridge\")\r\nadditem products loaditem(\"antiquity_food_egyptian_porridge\")\r\nadditem products loaditem(\"antiquity_food_roman_porridge\")\r\nadditem products loaditem(\"antiquity_food_celtic_porridge\")\r\nadditem products loaditem(\"antiquity_food_germanic_porridge\")\r\nadditem products loaditem(\"antiquity_food_kushite_porridge\")\r\nadditem products loaditem(\"antiquity_food_punic_porridge\")\r\nadditem products loaditem(\"antiquity_food_persian_porridge\")\r\nadditem products loaditem(\"antiquity_food_etruscan_porridge\")\r\nadditem products loaditem(\"antiquity_food_anatolian_porridge\")\r\nadditem products loaditem(\"antiquity_food_scythian_sarmatian_porridge\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederAntiquityFoodPreservedMeat() => new(
		new("ItemSeederAntiquityFood_preserved_meat", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederAntiquityFood_preserved_meat", "Crafting", "Antiquity Foodways", ProgVariableTypes.Item,
		"Selects one Antiquity culture prepared-food prototype from the generalized foodways craft.",
		"var products as item collection\r\nadditem products loaditem(\"antiquity_food_hellenic_preserved_meat\")\r\nadditem products loaditem(\"antiquity_food_egyptian_preserved_meat\")\r\nadditem products loaditem(\"antiquity_food_roman_preserved_meat\")\r\nadditem products loaditem(\"antiquity_food_celtic_preserved_meat\")\r\nadditem products loaditem(\"antiquity_food_germanic_preserved_meat\")\r\nadditem products loaditem(\"antiquity_food_kushite_preserved_meat\")\r\nadditem products loaditem(\"antiquity_food_punic_preserved_meat\")\r\nadditem products loaditem(\"antiquity_food_persian_preserved_meat\")\r\nadditem products loaditem(\"antiquity_food_etruscan_preserved_meat\")\r\nadditem products loaditem(\"antiquity_food_anatolian_preserved_meat\")\r\nadditem products loaditem(\"antiquity_food_scythian_sarmatian_preserved_meat\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederAntiquityFoodPulseStew() => new(
		new("ItemSeederAntiquityFood_pulse_stew", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederAntiquityFood_pulse_stew", "Crafting", "Antiquity Foodways", ProgVariableTypes.Item,
		"Selects one Antiquity culture prepared-food prototype from the generalized foodways craft.",
		"var products as item collection\r\nadditem products loaditem(\"antiquity_food_hellenic_pulse_stew\")\r\nadditem products loaditem(\"antiquity_food_egyptian_pulse_stew\")\r\nadditem products loaditem(\"antiquity_food_roman_pulse_stew\")\r\nadditem products loaditem(\"antiquity_food_celtic_pulse_stew\")\r\nadditem products loaditem(\"antiquity_food_germanic_pulse_stew\")\r\nadditem products loaditem(\"antiquity_food_kushite_pulse_stew\")\r\nadditem products loaditem(\"antiquity_food_punic_pulse_stew\")\r\nadditem products loaditem(\"antiquity_food_persian_pulse_stew\")\r\nadditem products loaditem(\"antiquity_food_etruscan_pulse_stew\")\r\nadditem products loaditem(\"antiquity_food_anatolian_pulse_stew\")\r\nadditem products loaditem(\"antiquity_food_scythian_sarmatian_pulse_stew\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederAntiquityFoodSpicedMeatStew() => new(
		new("ItemSeederAntiquityFood_spiced_meat_stew", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederAntiquityFood_spiced_meat_stew", "Crafting", "Antiquity Foodways", ProgVariableTypes.Item,
		"Selects one Antiquity culture prepared-food prototype from the generalized foodways craft.",
		"var products as item collection\r\nadditem products loaditem(\"antiquity_food_hellenic_spiced_meat_stew\")\r\nadditem products loaditem(\"antiquity_food_egyptian_spiced_meat_stew\")\r\nadditem products loaditem(\"antiquity_food_roman_spiced_meat_stew\")\r\nadditem products loaditem(\"antiquity_food_celtic_spiced_meat_stew\")\r\nadditem products loaditem(\"antiquity_food_germanic_spiced_meat_stew\")\r\nadditem products loaditem(\"antiquity_food_kushite_spiced_meat_stew\")\r\nadditem products loaditem(\"antiquity_food_punic_spiced_meat_stew\")\r\nadditem products loaditem(\"antiquity_food_persian_spiced_meat_stew\")\r\nadditem products loaditem(\"antiquity_food_etruscan_spiced_meat_stew\")\r\nadditem products loaditem(\"antiquity_food_anatolian_spiced_meat_stew\")\r\nadditem products loaditem(\"antiquity_food_scythian_sarmatian_spiced_meat_stew\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederAntiquityFoodStuffedFlatbread() => new(
		new("ItemSeederAntiquityFood_stuffed_flatbread", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederAntiquityFood_stuffed_flatbread", "Crafting", "Antiquity Foodways", ProgVariableTypes.Item,
		"Selects one Antiquity culture prepared-food prototype from the generalized foodways craft.",
		"var products as item collection\r\nadditem products loaditem(\"antiquity_food_hellenic_stuffed_flatbread\")\r\nadditem products loaditem(\"antiquity_food_egyptian_stuffed_flatbread\")\r\nadditem products loaditem(\"antiquity_food_roman_stuffed_flatbread\")\r\nadditem products loaditem(\"antiquity_food_celtic_stuffed_flatbread\")\r\nadditem products loaditem(\"antiquity_food_germanic_stuffed_flatbread\")\r\nadditem products loaditem(\"antiquity_food_kushite_stuffed_flatbread\")\r\nadditem products loaditem(\"antiquity_food_punic_stuffed_flatbread\")\r\nadditem products loaditem(\"antiquity_food_persian_stuffed_flatbread\")\r\nadditem products loaditem(\"antiquity_food_etruscan_stuffed_flatbread\")\r\nadditem products loaditem(\"antiquity_food_anatolian_stuffed_flatbread\")\r\nadditem products loaditem(\"antiquity_food_scythian_sarmatian_stuffed_flatbread\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederAntiquityFoodSweet() => new(
		new("ItemSeederAntiquityFood_sweet", "crafts", ["antiquity"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederAntiquityFood_sweet", "Crafting", "Antiquity Foodways", ProgVariableTypes.Item,
		"Selects one Antiquity culture prepared-food prototype from the generalized foodways craft.",
		"var products as item collection\r\nadditem products loaditem(\"antiquity_food_hellenic_sweet\")\r\nadditem products loaditem(\"antiquity_food_egyptian_sweet\")\r\nadditem products loaditem(\"antiquity_food_roman_sweet\")\r\nadditem products loaditem(\"antiquity_food_celtic_sweet\")\r\nadditem products loaditem(\"antiquity_food_germanic_sweet\")\r\nadditem products loaditem(\"antiquity_food_kushite_sweet\")\r\nadditem products loaditem(\"antiquity_food_punic_sweet\")\r\nadditem products loaditem(\"antiquity_food_persian_sweet\")\r\nadditem products loaditem(\"antiquity_food_etruscan_sweet\")\r\nadditem products loaditem(\"antiquity_food_anatolian_sweet\")\r\nadditem products loaditem(\"antiquity_food_scythian_sarmatian_sweet\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernIntermediateIntermediateEarlymodernFoodCassavaDetoxPressCake() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_cassava_detox_press_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_cassava_detox_press_cake", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_cassava_detox_press_cake\")\r\nadditem products loaditem(\"earlymodern_food_company_curry_spice_paste\")\r\nadditem products loaditem(\"earlymodern_food_grated_raw_potato_dumpling_mix\")\r\nadditem products loaditem(\"earlymodern_food_kimchi_salted_cabbage_stage\")\r\nadditem products loaditem(\"earlymodern_food_tomato_conserve_reduction_base\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernIntermediateIntermediateEarlymodernFoodCoffeeBiscuitRawSheet() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_coffee_biscuit_raw_sheet", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_coffee_biscuit_raw_sheet", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_coffee_biscuit_raw_sheet\")\r\nadditem products loaditem(\"earlymodern_food_pale_roux_working_ball\")\r\nadditem products loaditem(\"earlymodern_food_warm_choux_working_paste\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernIntermediateIntermediateEarlymodernFoodElasticStarchDelightMass() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_elastic_starch_delight_mass", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_elastic_starch_delight_mass", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_elastic_starch_delight_mass\")\r\nadditem products loaditem(\"earlymodern_food_ground_chocolate_conching_paste\")\r\nadditem products loaditem(\"earlymodern_food_molasses_refinery_scum_cake\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernIntermediateIntermediateEarlymodernFoodIceCreamCustardBase() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_ice_cream_custard_base", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_ice_cream_custard_base", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_ice_cream_custard_base\")\r\nadditem products loaditem(\"earlymodern_food_unbaked_meringue_foam\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernIntermediateIntermediateEarlymodernFoodLaminatedPuffPastryBlock() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_laminated_puff_pastry_block", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_laminated_puff_pastry_block", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_laminated_puff_pastry_block\")\r\nadditem products loaditem(\"earlymodern_food_tea_cake_proving_dough\")\r\nadditem products loaditem(\"earlymodern_food_tied_raw_plum_pudding_cloth\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernIntermediateIntermediateEarlymodernFoodMughalRawKebabMarinade() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_mughal_raw_kebab_marinade", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_mughal_raw_kebab_marinade", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_mughal_raw_kebab_marinade\")\r\nadditem products loaditem(\"earlymodern_food_ottoman_uncooked_phyllo_coil\")\r\nadditem products loaditem(\"earlymodern_food_portable_soup_reduction_slab\")\r\nadditem products loaditem(\"earlymodern_food_tea_smoking_cured_duck\")\r\nadditem products loaditem(\"earlymodern_food_unbaked_ship_sea_pie_layers\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernIntermediateIntermediateEarlymodernFoodNixtamalMaizeGrindingMass() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_nixtamal_maize_grinding_mass", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_nixtamal_maize_grinding_mass", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_nixtamal_maize_grinding_mass\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernIntermediateIntermediateEarlymodernFoodPulledSugarDecorationRibbon() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_pulled_sugar_decoration_ribbon", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_pulled_sugar_decoration_ribbon", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_pulled_sugar_decoration_ribbon\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernIntermediateIntermediateEarlymodernFoodSoakingSaltCodBundle() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_soaking_salt_cod_bundle", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_soaking_salt_cod_bundle", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_soaking_salt_cod_bundle\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernIntermediateIntermediateEarlymodernFoodSushiVinegarRiceCooling() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_sushi_vinegar_rice_cooling", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Intermediate_Intermediate_earlymodern_food_sushi_vinegar_rice_cooling", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_sushi_vinegar_rice_cooling\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedBreadEarlymodernFoodBritishTeaRusk() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Bread_earlymodern_food_british_tea_rusk", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Bread_earlymodern_food_british_tea_rusk", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_british_tea_rusk\")\r\nadditem products loaditem(\"earlymodern_food_caribbean_festival_like_fried_dough\")\r\nadditem products loaditem(\"earlymodern_food_french_coffee_house_brioche\")\r\nadditem products loaditem(\"earlymodern_food_ottoman_ramadan_pide_loaf\")\r\nadditem products loaditem(\"earlymodern_food_plantation_molasses_ship_biscuit\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedBreadEarlymodernFoodCaribbeanCassavaBammy() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Bread_earlymodern_food_caribbean_cassava_bammy", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Bread_earlymodern_food_caribbean_cassava_bammy", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_caribbean_cassava_bammy\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedCondimentEarlymodernFoodCompanyMangoLikeChutneyToast() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Condiment_earlymodern_food_company_mango_like_chutney_toast", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Condiment_earlymodern_food_company_mango_like_chutney_toast", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_company_mango_like_chutney_toast\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedDairyEarlymodernFoodFrenchCheeseGougerePuffs() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Dairy_earlymodern_food_french_cheese_gougere_puffs", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Dairy_earlymodern_food_french_cheese_gougere_puffs", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_french_cheese_gougere_puffs\")\r\nadditem products loaditem(\"earlymodern_food_mesoamerican_cheese_chile_tamale\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedDumplingEarlymodernFoodBritishSteamedSuetDumplings() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Dumpling_earlymodern_food_british_steamed_suet_dumplings", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Dumpling_earlymodern_food_british_steamed_suet_dumplings", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_british_steamed_suet_dumplings\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedDumplingEarlymodernFoodGermanPotatoDumplings() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Dumpling_earlymodern_food_german_potato_dumplings", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Dumpling_earlymodern_food_german_potato_dumplings", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_german_potato_dumplings\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedEggEarlymodernFoodAndeanChunoEggCake() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Egg_earlymodern_food_andean_chuno_egg_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Egg_earlymodern_food_andean_chuno_egg_cake", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_andean_chuno_egg_cake\")\r\nadditem products loaditem(\"earlymodern_food_qing_tea_egg_plate\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedFishEarlymodernFoodBrazilianCassavaFishPirao() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_brazilian_cassava_fish_pirao", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_brazilian_cassava_fish_pirao", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_brazilian_cassava_fish_pirao\")\r\nadditem products loaditem(\"earlymodern_food_cape_pickled_fish_curry\")\r\nadditem products loaditem(\"earlymodern_food_caribbean_enslaved_saltfish_cornmeal\")\r\nadditem products loaditem(\"earlymodern_food_dutch_pickled_herring_onion_plate\")\r\nadditem products loaditem(\"earlymodern_food_french_bechamel_baked_fish\")\r\nadditem products loaditem(\"earlymodern_food_goan_coconut_like_fish_curry\")\r\nadditem products loaditem(\"earlymodern_food_gold_coast_smoked_fish_maize\")\r\nadditem products loaditem(\"earlymodern_food_iberian_cod_fritter_plate\")\r\nadditem products loaditem(\"earlymodern_food_iberian_cod_pil_pil\")\r\nadditem products loaditem(\"earlymodern_food_iberian_pepper_stuffed_cod\")\r\nadditem products loaditem(\"earlymodern_food_qing_shark_fin_like_braise\")\r\nadditem products loaditem(\"earlymodern_food_senegambian_chile_fish_cake\")\r\nadditem products loaditem(\"earlymodern_food_senegambian_tomato_fish_rice\")\r\nadditem products loaditem(\"earlymodern_food_tokugawa_grilled_eel_rice\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedFishEarlymodernFoodCompanyKedgereePredecessor() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_company_kedgeree_predecessor", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_company_kedgeree_predecessor", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_company_kedgeree_predecessor\")\r\nadditem products loaditem(\"earlymodern_food_tokugawa_sushi_box\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedFishEarlymodernFoodFrenchHorsDoeuvreAnchovyToasts() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_french_hors_doeuvre_anchovy_toasts", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_french_hors_doeuvre_anchovy_toasts", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_french_hors_doeuvre_anchovy_toasts\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedFishEarlymodernFoodTokugawaTempuraAssortment() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_tokugawa_tempura_assortment", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fish_earlymodern_food_tokugawa_tempura_assortment", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_tokugawa_tempura_assortment\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedFruitEarlymodernFoodOttomanQuinceJamCheesePlate() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fruit_earlymodern_food_ottoman_quince_jam_cheese_plate", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Fruit_earlymodern_food_ottoman_quince_jam_cheese_plate", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_ottoman_quince_jam_cheese_plate\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedGrainEarlymodernFoodBritishYorkshireBatterPudding() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_british_yorkshire_batter_pudding", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_british_yorkshire_batter_pudding", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_british_yorkshire_batter_pudding\")\r\nadditem products loaditem(\"earlymodern_food_ottoman_shipboard_bulgur_pilav\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedGrainEarlymodernFoodCaribbeanEnslavedCornmealRation() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_caribbean_enslaved_cornmeal_ration", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_caribbean_enslaved_cornmeal_ration", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_caribbean_enslaved_cornmeal_ration\")\r\nadditem products loaditem(\"earlymodern_food_gold_coast_maize_kenkey_like\")\r\nadditem products loaditem(\"earlymodern_food_kongo_maize_groundnut_cake\")\r\nadditem products loaditem(\"earlymodern_food_mediterranean_maize_cheese_porridge\")\r\nadditem products loaditem(\"earlymodern_food_woodlands_hominy_bean_cake\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedGrainEarlymodernFoodJoseonRoyalGujeolpan() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_joseon_royal_gujeolpan", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_joseon_royal_gujeolpan", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_joseon_royal_gujeolpan\")\r\nadditem products loaditem(\"earlymodern_food_tokugawa_bento_theatre_box\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedGrainEarlymodernFoodLouisianaAfricanRiceFritter() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_louisiana_african_rice_fritter", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Grain_earlymodern_food_louisiana_african_rice_fritter", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_louisiana_african_rice_fritter\")\r\nadditem products loaditem(\"earlymodern_food_lowcountry_enslaved_rice_peas\")\r\nadditem products loaditem(\"earlymodern_food_mughal_company_dinner_pilaf\")\r\nadditem products loaditem(\"earlymodern_food_mughal_dum_pukht_meat_rice\")\r\nadditem products loaditem(\"earlymodern_food_safavid_crisp_rice_crust_cake\")\r\nadditem products loaditem(\"earlymodern_food_west_african_rice_bean_feast\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedMeatEarlymodernFoodBritishMincedTongueSandwich() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Meat_earlymodern_food_british_minced_tongue_sandwich", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Meat_earlymodern_food_british_minced_tongue_sandwich", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_british_minced_tongue_sandwich\")\r\nadditem products loaditem(\"earlymodern_food_british_steak_kidney_pudding\")\r\nadditem products loaditem(\"earlymodern_food_cape_malay_spiced_meat_pie\")\r\nadditem products loaditem(\"earlymodern_food_chocolate_house_savory_meat_puff\")\r\nadditem products loaditem(\"earlymodern_food_company_madras_curry_plate\")\r\nadditem products loaditem(\"earlymodern_food_company_ship_curry_pie\")\r\nadditem products loaditem(\"earlymodern_food_dutch_east_indies_meat_pie\")\r\nadditem products loaditem(\"earlymodern_food_french_brown_ragout_meat\")\r\nadditem products loaditem(\"earlymodern_food_french_puff_pastry_game_pie\")\r\nadditem products loaditem(\"earlymodern_food_goan_vindaloo_like_pork\")\r\nadditem products loaditem(\"earlymodern_food_joseon_royal_neobiani\")\r\nadditem products loaditem(\"earlymodern_food_mesoamerican_pork_tamale\")\r\nadditem products loaditem(\"earlymodern_food_mexican_barbacoa_consomme_bowl\")\r\nadditem products loaditem(\"earlymodern_food_mughal_chile_lamb_skewers\")\r\nadditem products loaditem(\"earlymodern_food_mughal_kofta_nut_sauce\")\r\nadditem products loaditem(\"earlymodern_food_mughal_tomato_meat_qorma\")\r\nadditem products loaditem(\"earlymodern_food_naval_sea_pie_layers\")\r\nadditem products loaditem(\"earlymodern_food_ottoman_hunkar_style_aubergine\")\r\nadditem products loaditem(\"earlymodern_food_ottoman_imaret_meat_rice_ration\")\r\nadditem products loaditem(\"earlymodern_food_ottoman_imperial_kebab_array\")\r\nadditem products loaditem(\"earlymodern_food_ottoman_preserved_cherry_meat\")\r\nadditem products loaditem(\"earlymodern_food_ottoman_tomato_lamb_dolma\")\r\nadditem products loaditem(\"earlymodern_food_plains_bison_maize_pouch\")\r\nadditem products loaditem(\"earlymodern_food_punch_house_salted_meat_pie\")\r\nadditem products loaditem(\"earlymodern_food_qing_lions_head_meatballs\")\r\nadditem products loaditem(\"earlymodern_food_safavid_stuffed_fruit_platter\")\r\nadditem products loaditem(\"earlymodern_food_swahili_clove_meat_pilau\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedMeatEarlymodernFoodQingImperialManhanPlatter() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Meat_earlymodern_food_qing_imperial_manhan_platter", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Meat_earlymodern_food_qing_imperial_manhan_platter", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_qing_imperial_manhan_platter\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedNoodleEarlymodernFoodQingChileOilNoodles() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Noodle_earlymodern_food_qing_chile_oil_noodles", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Noodle_earlymodern_food_qing_chile_oil_noodles", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_qing_chile_oil_noodles\")\r\nadditem products loaditem(\"earlymodern_food_tokugawa_buckwheat_noodle_stall\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedPorridgeEarlymodernFoodQingMaizeMilletReliefPorridge() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Porridge_earlymodern_food_qing_maize_millet_relief_porridge", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Porridge_earlymodern_food_qing_maize_millet_relief_porridge", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_qing_maize_millet_relief_porridge\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedPoultryEarlymodernFoodFrenchRouxVelouteFowl() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Poultry_earlymodern_food_french_roux_veloute_fowl", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Poultry_earlymodern_food_french_roux_veloute_fowl", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_french_roux_veloute_fowl\")\r\nadditem products loaditem(\"earlymodern_food_french_service_royal_poultry\")\r\nadditem products loaditem(\"earlymodern_food_french_white_fricassee_chicken\")\r\nadditem products loaditem(\"earlymodern_food_mesoamerican_convent_mole\")\r\nadditem products loaditem(\"earlymodern_food_mughal_murgh_musallam\")\r\nadditem products loaditem(\"earlymodern_food_qing_beggars_chicken\")\r\nadditem products loaditem(\"earlymodern_food_qing_eight_treasure_duck\")\r\nadditem products loaditem(\"earlymodern_food_qing_peking_duck_service\")\r\nadditem products loaditem(\"earlymodern_food_qing_tea_smoked_duck\")\r\nadditem products loaditem(\"earlymodern_food_safavid_jeweled_fowl_rice\")\r\nadditem products loaditem(\"earlymodern_food_safavid_pomegranate_nut_fowl\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedPreservedEarlymodernFoodBritishPortableSoupCake() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_british_portable_soup_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_british_portable_soup_cake", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_british_portable_soup_cake\")\r\nadditem products loaditem(\"earlymodern_food_woodlands_pemmican_berry_block\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedPreservedEarlymodernFoodDutchShipboardCheeseRation() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_dutch_shipboard_cheese_ration", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_dutch_shipboard_cheese_ration", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_dutch_shipboard_cheese_ration\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedPreservedEarlymodernFoodJoseonChileCabbageKimchi() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_joseon_chile_cabbage_kimchi", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_joseon_chile_cabbage_kimchi", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_joseon_chile_cabbage_kimchi\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedPreservedEarlymodernFoodNavalSalmagundiPlatter() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_naval_salmagundi_platter", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_naval_salmagundi_platter", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_naval_salmagundi_platter\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedPreservedEarlymodernFoodTokugawaMisoPickledFish() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_tokugawa_miso_pickled_fish", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Preserved_earlymodern_food_tokugawa_miso_pickled_fish", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_tokugawa_miso_pickled_fish\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedPulseEarlymodernFoodBrazilianEnslavedBeanCake() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Pulse_earlymodern_food_brazilian_enslaved_bean_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Pulse_earlymodern_food_brazilian_enslaved_bean_cake", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_brazilian_enslaved_bean_cake\")\r\nadditem products loaditem(\"earlymodern_food_british_naval_pease_pudding\")\r\nadditem products loaditem(\"earlymodern_food_slave_ship_coarse_bean_ration\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedShellfishEarlymodernFoodBritishPottedShrimpToast() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Shellfish_earlymodern_food_british_potted_shrimp_toast", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Shellfish_earlymodern_food_british_potted_shrimp_toast", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_british_potted_shrimp_toast\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedSoupEarlymodernFoodBeninChilePepperSoup() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Soup_earlymodern_food_benin_chile_pepper_soup", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Soup_earlymodern_food_benin_chile_pepper_soup", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_benin_chile_pepper_soup\")\r\nadditem products loaditem(\"earlymodern_food_company_mulligatawny_like_soup\")\r\nadditem products loaditem(\"earlymodern_food_safavid_sour_fruit_leather_soup\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedSoupEarlymodernFoodJoseonTteokgukRitualSoup() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Soup_earlymodern_food_joseon_tteokguk_ritual_soup", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Soup_earlymodern_food_joseon_tteokguk_ritual_soup", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_joseon_tteokguk_ritual_soup\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedSoupEarlymodernFoodQingBirdNestLikeSoup() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Soup_earlymodern_food_qing_bird_nest_like_soup", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Soup_earlymodern_food_qing_bird_nest_like_soup", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_qing_bird_nest_like_soup\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedStewEarlymodernFoodBeninPlantainLikeMeatPottage() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Stew_earlymodern_food_benin_plantain_like_meat_pottage", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Stew_earlymodern_food_benin_plantain_like_meat_pottage", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_benin_plantain_like_meat_pottage\")\r\nadditem products loaditem(\"earlymodern_food_caribbean_pepperpot_preserve_stew\")\r\nadditem products loaditem(\"earlymodern_food_naval_lobscouse_pot\")\r\nadditem products loaditem(\"earlymodern_food_safavid_herb_plum_meat_stew\")\r\nadditem products loaditem(\"earlymodern_food_senegambian_groundnut_meat_stew\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedStewEarlymodernFoodJoseonSinseolloBrazier() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Stew_earlymodern_food_joseon_sinseollo_brazier", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Stew_earlymodern_food_joseon_sinseollo_brazier", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_joseon_sinseollo_brazier\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedStewEarlymodernFoodWoodlandsSuccotashBowl() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Stew_earlymodern_food_woodlands_succotash_bowl", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Stew_earlymodern_food_woodlands_succotash_bowl", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_woodlands_succotash_bowl\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedSweetEarlymodernFoodBrazilianSugarEggConventSweet() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_brazilian_sugar_egg_convent_sweet", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_brazilian_sugar_egg_convent_sweet", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_brazilian_sugar_egg_convent_sweet\")\r\nadditem products loaditem(\"earlymodern_food_french_meringue_kisses\")\r\nadditem products loaditem(\"earlymodern_food_iberian_convent_yolk_threads\")\r\nadditem products loaditem(\"earlymodern_food_iberian_crema_burnt_sugar\")\r\nadditem products loaditem(\"earlymodern_food_qing_mooncake_salted_yolk\")\r\nadditem products loaditem(\"earlymodern_food_swahili_portuguese_custard_pastry\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedSweetEarlymodernFoodBritishBathSugarBun() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_bath_sugar_bun", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_bath_sugar_bun", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_british_bath_sugar_bun\")\r\nadditem products loaditem(\"earlymodern_food_british_chelsea_currant_bun\")\r\nadditem products loaditem(\"earlymodern_food_british_cloth_boiled_plum_pudding\")\r\nadditem products loaditem(\"earlymodern_food_british_hot_cross_bun\")\r\nadditem products loaditem(\"earlymodern_food_british_layered_trifle\")\r\nadditem products loaditem(\"earlymodern_food_british_naval_duff_pudding\")\r\nadditem products loaditem(\"earlymodern_food_caribbean_rum_fruit_pudding\")\r\nadditem products loaditem(\"earlymodern_food_dutch_company_spice_cake\")\r\nadditem products loaditem(\"earlymodern_food_german_coffee_house_kugelhopf\")\r\nadditem products loaditem(\"earlymodern_food_mesoamerican_chocolate_pan_dulce\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedSweetEarlymodernFoodBritishCarawaySeedCake() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_caraway_seed_cake", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_caraway_seed_cake", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_british_caraway_seed_cake\")\r\nadditem products loaditem(\"earlymodern_food_british_pound_cake\")\r\nadditem products loaditem(\"earlymodern_food_dutch_oliebol_currant_fritter\")\r\nadditem products loaditem(\"earlymodern_food_dutch_poffertjes_plate\")\r\nadditem products loaditem(\"earlymodern_food_dutch_spiced_speculaas\")\r\nadditem products loaditem(\"earlymodern_food_french_chocolate_dipping_biscuit\")\r\nadditem products loaditem(\"earlymodern_food_french_choux_cream_puffs\")\r\nadditem products loaditem(\"earlymodern_food_french_coffee_biscuit\")\r\nadditem products loaditem(\"earlymodern_food_french_croquant_almond_biscuit\")\r\nadditem products loaditem(\"earlymodern_food_french_puff_pastry_cream_slice\")\r\nadditem products loaditem(\"earlymodern_food_german_spiced_chocolate_cake\")\r\nadditem products loaditem(\"earlymodern_food_iberian_chocolate_churros\")\r\nadditem products loaditem(\"earlymodern_food_joseon_tea_honey_cookie\")\r\nadditem products loaditem(\"earlymodern_food_mughal_jalebi_yogurt_service\")\r\nadditem products loaditem(\"earlymodern_food_ottoman_coffee_house_lokma\")\r\nadditem products loaditem(\"earlymodern_food_ottoman_tea_sesame_biscuit\")\r\nadditem products loaditem(\"earlymodern_food_punch_house_spiced_seed_cake\")\r\nadditem products loaditem(\"earlymodern_food_safavid_coffee_house_saffron_cake\")\r\nadditem products loaditem(\"earlymodern_food_tokugawa_castella_tea_slice\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

	public static CatalogueRecipeProgSupport ItemSeederPreIndustrialFoodEarlyModernPreparedSweetEarlymodernFoodBritishChocolatePuff() => new(
		new("ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_chocolate_puff", "crafts", ["earlymodern", "medieval", "renaissance"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "Seeders/ItemSeeder/ItemSeeder.Crafting.cs:557", []),
		"ItemSeederPreIndustrialFood_EarlyModern_Prepared_Sweet_earlymodern_food_british_chocolate_puff", "Crafting", "Pre-Industrial Food Catalogue", ProgVariableTypes.Item,
		"Selects one catalogue prepared-food or intermediate prototype from the generalized family/material recipe.",
		"var products as item collection\r\nadditem products loaditem(\"earlymodern_food_british_chocolate_puff\")\r\nadditem products loaditem(\"earlymodern_food_british_tea_table_almond_cake\")\r\nadditem products loaditem(\"earlymodern_food_caribbean_molasses_tamarind_sweet\")\r\nadditem products loaditem(\"earlymodern_food_french_almond_macaron_pair\")\r\nadditem products loaditem(\"earlymodern_food_french_candied_chestnut\")\r\nadditem products loaditem(\"earlymodern_food_french_chocolate_almond_dragees\")\r\nadditem products loaditem(\"earlymodern_food_french_chocolate_ice_cream\")\r\nadditem products loaditem(\"earlymodern_food_french_fruit_sorbet\")\r\nadditem products loaditem(\"earlymodern_food_french_petit_four_selection\")\r\nadditem products loaditem(\"earlymodern_food_german_coffee_house_linzer_torte\")\r\nadditem products loaditem(\"earlymodern_food_iberian_cacao_almond_turron\")\r\nadditem products loaditem(\"earlymodern_food_joseon_royal_fruit_punch_cake\")\r\nadditem products loaditem(\"earlymodern_food_mexican_cacao_almond_marzipan\")\r\nadditem products loaditem(\"earlymodern_food_mughal_mango_like_ice\")\r\nadditem products loaditem(\"earlymodern_food_ottoman_asure_palace_garnish\")\r\nadditem products loaditem(\"earlymodern_food_ottoman_starch_delight_roll\")\r\nadditem products loaditem(\"earlymodern_food_safavid_rose_nut_nougat\")\r\nadditem products loaditem(\"earlymodern_food_safavid_tea_almond_biscuit\")\r\nadditem products loaditem(\"earlymodern_food_tokugawa_nerikiri_sweet\")\r\nreturn collectionfirst(collectionshuffle(@products))", []);

}
