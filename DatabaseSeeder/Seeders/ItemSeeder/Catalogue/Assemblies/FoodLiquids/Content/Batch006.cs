#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class FoodLiquidsStock
{
	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodLightRoastCoffee() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_light_roast_coffee",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Coffee,
			Name: "light-roast coffee infusion",
			Description: "clear russet coffee",
			LongDescription: "A light roasting leaves this coffee infusion clear russet rather than black.",
			Taste: "It tastes bright, bitter, and faintly fruity.",
			Smell: "Roasted beans give a sharp fragrant aroma.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 3,
			AdmissionProfile: FoodAdmissionProfile.CoffeeTrade
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodCoffeeHuskInfusion() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_coffee_husk_infusion",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Coffee,
			Name: "coffee-husk infusion",
			Description: "clear russet coffee-fruit tea",
			LongDescription: "Dried coffee-fruit husks tint this clear infusion russet and leave a few curled fragments at the bottom.",
			Taste: "It tastes gently tart and raisin-like, with only a distant roasted bitterness.",
			Smell: "Dried cherry skin, hay, and warm fruit scent the rising steam.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 3,
			AdmissionProfile: FoodAdmissionProfile.CoffeeTrade
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSpicedCoffeeInfusion() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_spiced_coffee_infusion",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Coffee,
			Name: "spiced coffee infusion",
			Description: "aromatic black coffee",
			LongDescription: "Warm spice flecks show against the dark body of brewed coffee.",
			Taste: "It tastes bitter, aromatic, and gently hot.",
			Smell: "Coffee roast mingles with crushed spice.",
			Colour: "black",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 3,
			AdmissionProfile: FoodAdmissionProfile.CoffeeTrade
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodHoneyedCoffeeInfusion() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_honeyed_coffee_infusion",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Coffee,
			Name: "honeyed coffee infusion",
			Description: "dark sweetened coffee",
			LongDescription: "Honey gives dark coffee a slight amber sheen around the edge.",
			Taste: "It tastes bittersweet, floral, and strongly roasted.",
			Smell: "Honey softens the powerful coffee fragrance.",
			Colour: "black",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 3,
			AdmissionProfile: FoodAdmissionProfile.CoffeeTrade
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPlainCacaoDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_plain_cacao_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Chocolate,
			Name: "cacao drink",
			Description: "foamy dark cacao drink",
			LongDescription: "Whisking leaves a light foam over this dark water-based cacao drink.",
			Taste: "It tastes bitter, earthy, and richly chocolate-like.",
			Smell: "Roasted cacao gives a deep warm aroma.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.CacaoTrade
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSpicedCacaoDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_spiced_cacao_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Chocolate,
			Name: "spiced cacao drink",
			Description: "aromatic dark cacao drink",
			LongDescription: "Ground spice remains suspended beneath the foam of dark cacao.",
			Taste: "It tastes bitter, warming, and intensely aromatic.",
			Smell: "Cacao and spice fill the rising steam.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.CacaoTrade
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodHoneyedCacaoDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_honeyed_cacao_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Chocolate,
			Name: "honeyed cacao drink",
			Description: "sweet foamy cacao drink",
			LongDescription: "Honey softens a dark cacao drink and gives its foam an amber edge.",
			Taste: "It tastes bittersweet, floral, and richly chocolate-like.",
			Smell: "Honey rounds the roasted cacao fragrance.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 1.5,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.CacaoTrade
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodCreamyCacaoDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_creamy_cacao_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Chocolate,
			Name: "creamy cacao drink",
			Description: "thick pale-brown cacao drink",
			LongDescription: "Dairy gives this cacao drink a thick, smooth, pale-brown body.",
			Taste: "It tastes creamy, bitter, and gently sweet.",
			Smell: "Warm milk and roasted cacao scent it.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 3,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.CacaoTrade
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodBrownMeatSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_brown_meat_sauce",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Sauce,
			Name: "brown meat sauce",
			Description: "glossy brown meat sauce",
			LongDescription: "Reduced meat juices form a glossy sauce that clings heavily to food.",
			Taste: "It tastes roasted, salty, and deeply savoury.",
			Smell: "Browned meat and onion scent it.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 3,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodLightFowlSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_light_fowl_sauce",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Sauce,
			Name: "light fowl sauce",
			Description: "pale golden fowl sauce",
			LongDescription: "Fowl stock and fat reduce into a pale gold, spoon-coating sauce.",
			Taste: "It tastes delicate, buttery, and herb-savoury.",
			Smell: "Poultry, herbs, and warm fat rise gently.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodFishSavourSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_fish_savour_sauce",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Sauce,
			Name: "fish savour sauce",
			Description: "thin brown fish sauce",
			LongDescription: "Salted fish gives this thin brown cooking sauce a clear appearance.",
			Taste: "It tastes intensely briny, fermented, and savoury.",
			Smell: "Pungent fish and salt dominate its smell.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.MaritimeTrade
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodShellfishReduction() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_shellfish_reduction",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Sauce,
			Name: "shellfish reduction",
			Description: "peach-brown shellfish sauce",
			LongDescription: "Shellfish liquor reduces into a glossy peach-brown finishing sauce.",
			Taste: "It tastes mineral, sweetly briny, and concentrated.",
			Smell: "Warm shells and sea air scent it.",
			Colour: "orange",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.MaritimeTrade
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodMushroomReductionSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_mushroom_reduction_sauce",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Sauce,
			Name: "mushroom reduction sauce",
			Description: "dark mushroom sauce",
			LongDescription: "Mushroom juices form a dark, silky sauce with fine brown sediment.",
			Taste: "It tastes woodsy, peppery, and richly savoury.",
			Smell: "Earthy mushrooms give a damp roasted aroma.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodOnionGravy() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_onion_gravy",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Sauce,
			Name: "onion gravy",
			Description: "glossy amber onion gravy",
			LongDescription: "Slow-cooked onions thicken this amber gravy into a glossy pour.",
			Taste: "It tastes caramel-sweet, savoury, and gently salty.",
			Smell: "Soft onions and roast juices scent it.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodGarlicHerbSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_garlic_herb_sauce",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Sauce,
			Name: "garlic herb sauce",
			Description: "green-flecked garlic sauce",
			LongDescription: "Crushed garlic and herbs make a pale sauce densely flecked with green.",
			Taste: "It tastes pungent, fresh, and warmly savoury.",
			Smell: "Garlic and cut leaves fill the aroma.",
			Colour: "green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSourHerbSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_sour_herb_sauce",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Sauce,
			Name: "sour herb sauce",
			Description: "sharp green herb sauce",
			LongDescription: "Fresh herbs suspend in a bright sour sauce of clear green-gold.",
			Taste: "It tastes tart, leafy, and appetite-waking.",
			Smell: "Acid and crushed herbs scent it sharply.",
			Colour: "green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodYoghurtSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_yoghurt_sauce",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Sauce,
			Name: "yoghurt sauce",
			Description: "cool white yoghurt sauce",
			LongDescription: "Strained yoghurt makes this white sauce smooth, thick, and cooling.",
			Taste: "It tastes creamy, sour, and lightly salted.",
			Smell: "Cultured dairy gives a clean tangy smell.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodCheeseSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_cheese_sauce",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Sauce,
			Name: "melted cheese sauce",
			Description: "thick golden cheese sauce",
			LongDescription: "Melted cheese forms a thick golden sauce that stretches when poured.",
			Taste: "It tastes salty, creamy, and richly matured.",
			Smell: "Warm cheese provides a powerful dairy aroma.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 4,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodNutSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_nut_sauce",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Sauce,
			Name: "ground nut sauce",
			Description: "creamy tan nut sauce",
			LongDescription: "Ground nuts give this tan cooking sauce a smooth, oily body.",
			Taste: "It tastes toasted, creamy, and softly savoury.",
			Smell: "Roasted nuts and spice shape the smell.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 4,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodFruitMeatSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_fruit_meat_sauce",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Sauce,
			Name: "tart fruit meat sauce",
			Description: "glossy ruby fruit sauce",
			LongDescription: "Reduced tart fruit makes a ruby sauce intended for rich roasted meat.",
			Taste: "It tastes sweet-sour, fruity, and warmly spiced.",
			Smell: "Cooked fruit and pepper perfume it.",
			Colour: "red",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodChiliSavourSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_chili_savour_sauce",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Sauce,
			Name: "hot chile savour sauce",
			Description: "vivid red hot sauce",
			LongDescription: "Crushed chile gives this thin savoury sauce a vivid red colour.",
			Taste: "It tastes sharply hot, salty, and fruity.",
			Smell: "Fresh pepper and fermentation scent it.",
			Colour: "red",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.Mesoamerican
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodCacaoSavourSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_cacao_savour_sauce",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Sauce,
			Name: "savoury cacao sauce",
			Description: "dark bitter cacao sauce",
			LongDescription: "Cacao and spice make this savoury sauce dark brown and velvety.",
			Taste: "It tastes bitter, aromatic, and richly complex.",
			Smell: "Roasted cacao and warm spice scent it.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.CacaoTrade
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodWalnutCookingOil() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_walnut_cooking_oil",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Oil,
			Name: "walnut cooking oil",
			Description: "clear gold walnut oil",
			LongDescription: "Pressed walnuts produce a clear golden oil with a delicate body.",
			Taste: "It tastes nutty, rich, and faintly bitter.",
			Smell: "Fresh walnut gives a soft roasted scent.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0,
			FoodSatiatedHoursPerLitre: 6,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodHazelnutCookingOil() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_hazelnut_cooking_oil",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Oil,
			Name: "hazelnut cooking oil",
			Description: "pale amber hazelnut oil",
			LongDescription: "This pale amber oil shines clearly after careful nut pressing.",
			Taste: "It tastes toasted, sweet, and deeply nutty.",
			Smell: "Roasted hazelnut perfumes the oil.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0,
			FoodSatiatedHoursPerLitre: 6,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodAlmondCookingOil() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_almond_cooking_oil",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Oil,
			Name: "almond cooking oil",
			Description: "pale gold almond oil",
			LongDescription: "Almond pressing yields a pale gold oil with very little sediment.",
			Taste: "It tastes mild, buttery, and softly nutty.",
			Smell: "Sweet almond gives a delicate fragrance.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0,
			FoodSatiatedHoursPerLitre: 6,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPoppyseedCookingOil() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_poppyseed_cooking_oil",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Oil,
			Name: "poppyseed cooking oil",
			Description: "clear yellow poppyseed oil",
			LongDescription: "Tiny seeds produce a clear yellow oil of light consistency.",
			Taste: "It tastes earthy, mild, and pleasantly nutty.",
			Smell: "Poppyseed lends a quiet grassy aroma.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0,
			FoodSatiatedHoursPerLitre: 6,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.European
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodLinseedCookingOil() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_linseed_cooking_oil",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Oil,
			Name: "linseed cooking oil",
			Description: "deep gold linseed oil",
			LongDescription: "Pressed linseed gives this edible oil a deep golden colour.",
			Taste: "It tastes grassy, earthy, and slightly bitter.",
			Smell: "Seeds and cut grass shape the aroma.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0,
			FoodSatiatedHoursPerLitre: 6,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodHempseedCookingOil() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_hempseed_cooking_oil",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Oil,
			Name: "hempseed cooking oil",
			Description: "green-gold hempseed oil",
			LongDescription: "Hempseed tints this clear cooking oil a muted green-gold.",
			Taste: "It tastes grassy, robust, and nut-like.",
			Smell: "Crushed seed gives an earthy smell.",
			Colour: "green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0,
			FoodSatiatedHoursPerLitre: 6,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodMustardSeedOil() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_mustard_seed_oil",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Oil,
			Name: "mustard seed oil",
			Description: "bright yellow pungent oil",
			LongDescription: "Mustard seed yields a bright yellow oil with a sharp vapour.",
			Taste: "It tastes pungent, hot, and richly oily.",
			Smell: "Mustard heat dominates the nose.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0,
			FoodSatiatedHoursPerLitre: 6,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.SouthAsian
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPalmFruitOil() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_palm_fruit_oil",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Oil,
			Name: "palm fruit oil",
			Description: "deep orange palm oil",
			LongDescription: "Palm fruit produces an opaque orange-red oil that sets softly when cool.",
			Taste: "It tastes earthy, fruity, and richly fatty.",
			Smell: "Warm palm fruit gives a distinctive scent.",
			Colour: "orange",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0,
			FoodSatiatedHoursPerLitre: 6,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.SubSaharanAfrican
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodCoconutCookingOil() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_coconut_cooking_oil",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Oil,
			Name: "coconut cooking oil",
			Description: "clear pale coconut oil",
			LongDescription: "This pale oil is clear when warm and firm white when cooled.",
			Taste: "It tastes creamy, sweet, and coconut-fragrant.",
			Smell: "Fresh coconut gives a tropical aroma.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0,
			FoodSatiatedHoursPerLitre: 6,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.SouthAsian
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodGrapeseedCookingOil() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_grapeseed_cooking_oil",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Oil,
			Name: "grapeseed cooking oil",
			Description: "pale green-gold grapeseed oil",
			LongDescription: "Grape seeds yield a light, clear oil with green-gold highlights.",
			Taste: "It tastes mild, dry, and faintly nutty.",
			Smell: "Crushed grape seed gives little aroma.",
			Colour: "green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0,
			FoodSatiatedHoursPerLitre: 6,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodDarkGrainVinegar() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_dark_grain_vinegar",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Vinegar,
			Name: "dark grain vinegar",
			Description: "deep brown grain vinegar",
			LongDescription: "Fermented grain gives this clear vinegar a deep brown colour.",
			Taste: "It tastes sharply sour, malty, and robust.",
			Smell: "Dark grain and acid fill the nose.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPaleGrainVinegar() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_pale_grain_vinegar",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Vinegar,
			Name: "pale grain vinegar",
			Description: "clear straw grain vinegar",
			LongDescription: "A pale straw vinegar retains only a light tint from fermented grain.",
			Taste: "It tastes clean, sour, and faintly cereal-like.",
			Smell: "Sharp acid carries a mild grain scent.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodRiceVinegar() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_rice_vinegar",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Vinegar,
			Name: "rice vinegar",
			Description: "clear pale rice vinegar",
			LongDescription: "Rice fermentation produces a nearly colourless, softly aromatic vinegar.",
			Taste: "It tastes mild, clean, and gently sour.",
			Smell: "Soft acid and rice shape the aroma.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EastAsian
		);
}
