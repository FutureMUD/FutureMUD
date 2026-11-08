#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class FoodLiquidsStock
{
	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidChileMaizeCacao() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_chile_maize_cacao",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Chocolate,
			Name: "chile-maize cacao froth",
			Description: "dark chile-maize cacao",
			LongDescription: "Maize thickens this dark cacao drink beneath a coarse foam, while red chile flecks remain visible in the cup.",
			Taste: "Roasted bitterness and grain body are followed by a clean, rising chile heat.",
			Smell: "It smells of ground cacao, toasted maize, dried chile, and smoke.",
			Colour: "Bold Black",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 3,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidVanillaFlowerCacao() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_vanilla_flower_cacao",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Chocolate,
			Name: "vanilla-flower cacao liquor",
			Description: "foamy vanilla-flower cacao",
			LongDescription: "A tall cap of tan foam crowns this dark cacao preparation, perfumed with vanilla orchid and other fragrant flowers.",
			Taste: "Cacao remains dry and bitter beneath rounded vanilla and a delicate floral sweetness.",
			Smell: "The rich aroma carries roasted beans, vanilla pod, and freshly gathered blossoms.",
			Colour: "Magenta",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidColonialAndeanAniseQuinoaChicha() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_colonial_andean_anise_quinoa_chicha",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "colonial anise-quinoa chicha",
			Description: "cloudy anise quinoa chicha",
			LongDescription: "Fermented quinoa gives this early colonial drink a cloudy straw colour, while introduced anise seed dots the loose surface foam.",
			Taste: "Mild sourness and grainy nuttiness lead into an aromatic liquorice-like finish.",
			Smell: "It smells of wet quinoa, active fermentation, anise, and cool earthen jars.",
			Colour: "Bold White",
			AlcoholLitresPerLitre: 0.03,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidAtlanticMaizeMilletBeer() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_atlantic_maize_millet_beer",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "Atlantic maize-millet beer",
			Description: "ochre maize and millet beer",
			LongDescription: "Newly arrived maize ferments beside millet in this contact-zone beer, whose ochre sediment gathers beneath a thin pale foam.",
			Taste: "Sweet maize lightens the earthy millet and finishes with mild fermented bitterness.",
			Smell: "Its aroma brings damp grain, fresh yeast, faint smoke, and calabash.",
			Colour: "Bold Orange",
			AlcoholLitresPerLitre: 0.04,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidCanarySackCookingWine() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_canary_sack_cooking_wine",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Wine,
			Name: "Canary sack cooking wine",
			Description: "deep amber Canary sack",
			LongDescription: "This fortified island wine is deep amber and brilliantly clear, pouring with more weight than an ordinary table vintage.",
			Taste: "Ripe grape sweetness, oxidation, and warming alcohol finish on dried fruit and nuts.",
			Smell: "It smells richly of sun-dried grapes, old wood, orange peel, and spirit.",
			Colour: "Orange",
			AlcoholLitresPerLitre: 0.15,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 0.25,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodClearBoneBroth() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_clear_bone_broth",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "clear bone broth",
			Description: "clear golden bone broth",
			LongDescription: "A clear golden stock carries tiny beads of marrow fat across its surface.",
			Taste: "It tastes cleanly meaty, salty, and gently gelatinous.",
			Smell: "Its aroma is warm bone, marrow, and steam.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1.5,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodRoastedBoneStock() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_roasted_bone_stock",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "roasted bone stock",
			Description: "dark roasted bone stock",
			LongDescription: "Deep brown stock shows the colour of bones browned before their long simmer.",
			Taste: "Roast savour and marrow richness finish faintly smoky.",
			Smell: "It smells of browned bones and hearth smoke.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodBeefStock() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_beef_stock",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "beef cooking stock",
			Description: "rich brown beef stock",
			LongDescription: "A robust brown stock gleams with dissolved juices and sparse golden fat.",
			Taste: "It tastes deeply beefy with pepper and clean salt.",
			Smell: "Roasted beef and warm onion scent the liquor.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodMuttonStock() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_mutton_stock",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "mutton cooking stock",
			Description: "fat-rich mutton stock",
			LongDescription: "Amber mutton stock bears round droplets of pale sheep fat on top.",
			Taste: "It is pastoral, savoury, and distinctly rich.",
			Smell: "The steam carries mutton fat and mild herbs.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPorkStock() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_pork_stock",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "pork cooking stock",
			Description: "pale savoury pork stock",
			LongDescription: "This pale stock has a soft haze and a sweet aroma from simmered pork.",
			Taste: "Gentle meat sweetness meets salt and rendered fat.",
			Smell: "It smells of pork, softened onion, and steam.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodFowlStock() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_fowl_stock",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "fowl cooking stock",
			Description: "clear golden fowl stock",
			LongDescription: "A bright gold stock holds minute droplets of poultry fat and herb flecks.",
			Taste: "The flavour is light, savoury, and softly fatty.",
			Smell: "Cooked fowl and garden herbs rise in its steam.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1.5,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodGameStock() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_game_stock",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "game cooking stock",
			Description: "dark fragrant game stock",
			LongDescription: "Wild meat and bones give this clear stock a dark amber-brown hue.",
			Taste: "It tastes gamey, mineral, and warmly aromatic.",
			Smell: "The scent recalls roast game and woodland herbs.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodFishStock() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_fish_stock",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "fish cooking stock",
			Description: "pale clear fish stock",
			LongDescription: "A nearly clear fish stock shows a pearly sheen and tiny herb fragments.",
			Taste: "Delicate brine and fish sweetness end cleanly.",
			Smell: "It smells lightly marine, herbal, and fresh.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.MaritimeTrade
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodShellfishStock() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_shellfish_stock",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "shellfish cooking stock",
			Description: "peach-gold shellfish stock",
			LongDescription: "Shells have coloured this translucent stock a warm peach-gold.",
			Taste: "It tastes sweetly briny with a mineral finish.",
			Smell: "The aroma suggests warm shells and clean sea air.",
			Colour: "orange",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.MaritimeTrade
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodMushroomStock() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_mushroom_stock",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "mushroom cooking stock",
			Description: "clear brown mushroom stock",
			LongDescription: "A clear brown liquor holds the concentrated colour of simmered mushrooms.",
			Taste: "Woodsy savour gives way to pepper and herbs.",
			Smell: "It smells earthy, damp, and gently roasted.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodRootVegetableStock() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_root_vegetable_stock",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "root vegetable stock",
			Description: "golden root vegetable stock",
			LongDescription: "Mixed roots lend a translucent golden colour and mild natural sweetness.",
			Taste: "It tastes earthy, sweet, and lightly salted.",
			Smell: "Warm roots and green herbs scent the steam.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodGreenVegetableStock() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_green_vegetable_stock",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "green vegetable stock",
			Description: "pale green vegetable stock",
			LongDescription: "Leafy trimmings tint this clear vegetable stock a subdued green.",
			Taste: "It tastes fresh, mineral, and faintly bitter.",
			Smell: "The aroma is leafy, clean, and herbaceous.",
			Colour: "green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodOnionStock() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_onion_stock",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "onion cooking stock",
			Description: "clear amber onion stock",
			LongDescription: "Long-simmered onions leave this strained liquor clear and warm amber.",
			Taste: "Deep allium sweetness balances a savoury saltiness.",
			Smell: "It smells of softened onions and gentle caramel.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodGarlicBrothLiquid() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_garlic_broth_liquid",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "garlic cooking broth",
			Description: "pale aromatic garlic broth",
			LongDescription: "Crushed garlic gives this clear broth a slight ivory cloudiness.",
			Taste: "It tastes warmly pungent, savoury, and clean.",
			Smell: "Soft garlic and herbs dominate its aroma.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodHerbBrothLiquid() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_herb_broth_liquid",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "fresh herb broth",
			Description: "green-flecked clear herb broth",
			LongDescription: "Chopped leaves drift through clear stock tinted faintly green.",
			Taste: "Fresh aromatics lead over restrained savour and salt.",
			Smell: "It smells grassy, leafy, and newly crushed.",
			Colour: "green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodGrainCookingBroth() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_grain_cooking_broth",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "grain cooking broth",
			Description: "cloudy beige grain broth",
			LongDescription: "Starch from boiled grain gives this broth a cloudy beige body.",
			Taste: "It tastes mild, cereal-sweet, and gently savoury.",
			Smell: "Warm grain and faint malt scent the liquid.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPulseCookingBroth() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_pulse_cooking_broth",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "pulse cooking broth",
			Description: "cloudy tan pulse broth",
			LongDescription: "Cooked pulses leave this strained broth tan and softly opaque.",
			Taste: "It tastes earthy, starchy, and modestly salted.",
			Smell: "The smell is nutty, warm, and bean-like.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSpicedMeatBroth() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_spiced_meat_broth",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "spiced meat broth",
			Description: "aromatic brown meat broth",
			LongDescription: "Ground and whole spices perfume a brown stock with a glossy surface.",
			Taste: "Warm spice unfolds over concentrated meat savour.",
			Smell: "It smells peppery, meaty, and richly aromatic.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSourHerbBroth() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_sour_herb_broth",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "sour herb broth",
			Description: "sharp green-flecked broth",
			LongDescription: "A tarting ingredient brightens this clear broth scattered with fresh herbs.",
			Taste: "Clean acidity wakes a leafy, lightly savoury base.",
			Smell: "It smells sharp, herbal, and appetisingly fresh.",
			Colour: "green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodMarrowBrothLiquid() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_marrow_broth_liquid",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Broth,
			Name: "marrow-rich broth",
			Description: "pearly marrow-rich broth",
			LongDescription: "Dissolved marrow lends this broth a pearly haze and generous golden beads.",
			Taste: "It tastes buttery, beef-rich, and deeply gelatinous.",
			Smell: "Warm marrow and roast bone fill the aroma.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 3,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodCulturedCowDairyDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_cultured_cow_dairy_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.DairyDrink,
			Name: "cultured cow dairy drink",
			Description: "thick white cultured dairy",
			LongDescription: "A smooth white cultured drink clings lightly to the sides of its vessel.",
			Taste: "It tastes creamy, cleanly sour, and refreshing.",
			Smell: "The aroma is lactic, cool, and faintly grassy.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 3,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodCulturedSheepDairyDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_cultured_sheep_dairy_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.DairyDrink,
			Name: "cultured sheep dairy drink",
			Description: "rich ivory cultured dairy",
			LongDescription: "Sheep dairy makes this cultured drink ivory, dense, and faintly glossy.",
			Taste: "Rich milk sweetness meets an assertive tartness.",
			Smell: "It smells creamy, pastoral, and pleasantly sour.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 4,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodCulturedGoatDairyDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_cultured_goat_dairy_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.DairyDrink,
			Name: "cultured goat dairy drink",
			Description: "tangy pale cultured dairy",
			LongDescription: "This pale cultured dairy drink has a light body and lively sour aroma.",
			Taste: "It tastes tangy, mineral, and distinctly goaty.",
			Smell: "The scent is sharp dairy with a grassy note.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 3,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodThinnedYoghurtDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_thinned_yoghurt_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.DairyDrink,
			Name: "thinned yoghurt drink",
			Description: "cool salted yoghurt drink",
			LongDescription: "Water and salt loosen yoghurt into a smooth, cooling white drink.",
			Taste: "It tastes tart, lightly salty, and very refreshing.",
			Smell: "Cultured milk gives it a clean sour fragrance.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 3,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodHerbedYoghurtDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_herbed_yoghurt_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.DairyDrink,
			Name: "herbed yoghurt drink",
			Description: "green-flecked yoghurt drink",
			LongDescription: "Fine green herbs remain suspended through this pale cultured drink.",
			Taste: "Fresh leaves brighten its creamy, salty tang.",
			Smell: "It smells of yoghurt and newly chopped herbs.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 3,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodHoneyedCulturedDairy() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_honeyed_cultured_dairy",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.DairyDrink,
			Name: "honeyed cultured dairy",
			Description: "sweet ivory cultured drink",
			LongDescription: "Honey lends an ivory cultured dairy drink a faint amber sheen.",
			Taste: "Floral sweetness softens the clean lactic sourness.",
			Smell: "It smells creamy, honeyed, and gently fermented.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 3,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSpicedCulturedDairy() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_spiced_cultured_dairy",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.DairyDrink,
			Name: "spiced cultured dairy",
			Description: "aromatic pale dairy drink",
			LongDescription: "Warm spice flecks show against the pale body of cultured dairy.",
			Taste: "Aromatic heat follows its cooling creamy tang.",
			Smell: "The fragrance combines sour milk and ground spice.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 3,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSweetWheyDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_sweet_whey_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.DairyDrink,
			Name: "sweet whey drink",
			Description: "translucent yellow whey drink",
			LongDescription: "Fresh whey appears translucent yellow with a very light dairy body.",
			Taste: "It tastes gently sweet, tart, and faintly milky.",
			Smell: "The smell is clean whey and fresh curd.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSouredWheyDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_soured_whey_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.DairyDrink,
			Name: "soured whey drink",
			Description: "sharp cloudy whey drink",
			LongDescription: "Fermentation clouds this whey slightly and gives it a sharper aroma.",
			Taste: "It tastes briskly sour with a mineral dairy finish.",
			Smell: "It smells lactic, yeasty, and lightly cheesy.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0.01,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodButtermilkTableDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_buttermilk_table_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.DairyDrink,
			Name: "buttermilk table drink",
			Description: "thin tangy buttermilk",
			LongDescription: "Butter-making leaves this pale drink thin, cloudy, and lightly flecked.",
			Taste: "It tastes refreshing, tart, and softly buttery.",
			Smell: "The aroma is cultured cream and fresh churn.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 1.5,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);
}
