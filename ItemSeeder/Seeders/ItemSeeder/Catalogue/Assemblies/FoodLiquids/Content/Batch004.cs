#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class FoodLiquidsStock
{
	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPressedAppleMust() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_pressed_apple_must",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "pressed apple must",
			Description: "cloudy gold apple must",
			LongDescription: "Fresh pressing leaves this apple liquid cloudy gold with fine fruit sediment.",
			Taste: "It tastes crisp, tart, and naturally sweet.",
			Smell: "Raw apple and crushed peel scent the must.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPressedPearMust() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_pressed_pear_must",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "pressed pear must",
			Description: "pale fragrant pear must",
			LongDescription: "This pale fruit must has a soft haze and delicate floral perfume.",
			Taste: "It tastes mellow, juicy, and honey-sweet.",
			Smell: "Ripe pear and fresh-cut flesh shape the aroma.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPressedGrapeMust() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_pressed_grape_must",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "pressed grape must",
			Description: "purple-red grape must",
			LongDescription: "Skins tint this freshly pressed grape must a vivid translucent purple-red.",
			Taste: "It tastes winey, sweet, and lightly tannic.",
			Smell: "Crushed grapes give it a lush fruity fragrance.",
			Colour: "magenta",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodMixedBerryPressing() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_mixed_berry_pressing",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "mixed berry pressing",
			Description: "dark crimson berry drink",
			LongDescription: "Tiny seeds and pulp cloud this dark crimson pressed berry liquid.",
			Taste: "It tastes sharply fruity with concentrated sweetness.",
			Smell: "The smell is vivid berries and crushed leaves.",
			Colour: "red",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPlumPressing() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_plum_pressing",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "pressed plum drink",
			Description: "ruby-purple plum drink",
			LongDescription: "Soft plum pulp gives the pressed drink a rich ruby-purple haze.",
			Taste: "It tastes sweet-sour, winey, and full-bodied.",
			Smell: "Ripe plum skins lend a dark fruity aroma.",
			Colour: "magenta",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodFigWaterDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_fig_water_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "fig-steeped fruit drink",
			Description: "amber fig-steeped drink",
			LongDescription: "Steeped figs tint the water amber and leave tiny seeds suspended.",
			Taste: "It tastes earthy, honeyed, and softly fruity.",
			Smell: "The aroma is dried fig and warm sweetness.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodDateWaterDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_date_water_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "date-steeped fruit drink",
			Description: "brown-gold date drink",
			LongDescription: "Soft dates lend this strained drink a clear brown-gold colour.",
			Taste: "Caramel sweetness follows a faint earthy note.",
			Smell: "It smells of ripe dates and dark syrup.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.Islamicate
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodMelonWaterDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_melon_water_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "melon water drink",
			Description: "pale green melon drink",
			LongDescription: "Crushed melon flesh makes this drink pale green and softly cloudy.",
			Taste: "It tastes cooling, watery, and delicately sweet.",
			Smell: "Fresh melon gives a mild floral fragrance.",
			Colour: "green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 3,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodTartCitrusPressing() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_tart_citrus_pressing",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "tart citrus pressing",
			Description: "bright yellow citrus drink",
			LongDescription: "Fresh citrus juice shines bright yellow with fine pulp throughout.",
			Taste: "It tastes sharply acidic, fragrant, and refreshing.",
			Smell: "The scent is vivid peel and fresh-cut citrus.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.MaritimeTrade
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodTamarindWaterDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_tamarind_water_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "tamarind water drink",
			Description: "russet tart fruit drink",
			LongDescription: "Tamarind pulp clouds this water a warm russet-brown shade.",
			Taste: "It tastes fruity, sour, and faintly caramel-like.",
			Smell: "The aroma is tart fruit with an earthy sweetness.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.SouthAsian
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPineapplePressing() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_pineapple_pressing",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "pressed pineapple drink",
			Description: "bright gold tropical drink",
			LongDescription: "Fibrous pulp clouds this vivid gold pressed tropical fruit liquid.",
			Taste: "It tastes piercingly tart, lush, and sweet.",
			Smell: "The scent is powerful pineapple and fresh rind.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.IndigenousAmerican
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodGuavaPressing() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_guava_pressing",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "pressed guava drink",
			Description: "cloudy pink guava drink",
			LongDescription: "Guava pulp makes this pressed drink cloudy pink and seed-flecked.",
			Taste: "It tastes floral, grainy, and sweet-tart.",
			Smell: "Ripe guava gives an intense perfumed aroma.",
			Colour: "pink",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.IndigenousAmerican
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodBarleyWaterDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_barley_water_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.GrainDrink,
			Name: "barley water drink",
			Description: "pale cloudy barley water",
			LongDescription: "Boiled barley leaves this strained drink pale, cloudy, and softly starchy.",
			Taste: "It tastes mildly malty with gentle cereal sweetness.",
			Smell: "The smell is warm grain and faint toast.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodToastedBarleyDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_toasted_barley_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.GrainDrink,
			Name: "toasted barley drink",
			Description: "clear brown toasted-grain drink",
			LongDescription: "Roasted barley colours this strained drink a clear medium brown.",
			Taste: "It tastes nutty, toasted, and slightly bitter.",
			Smell: "The aroma suggests malt, biscuit, and roast grain.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodWheatWaterDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_wheat_water_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.GrainDrink,
			Name: "wheat water drink",
			Description: "cloudy cream wheat water",
			LongDescription: "Cooked wheat starch gives this water a cloudy cream appearance.",
			Taste: "It tastes softly wheaten, mild, and faintly sweet.",
			Smell: "Warm bread grain shapes its gentle aroma.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodRyeWaterDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_rye_water_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.GrainDrink,
			Name: "rye water drink",
			Description: "grey-brown rye water",
			LongDescription: "Dark rye lends this strained grain water a grey-brown hue.",
			Taste: "It tastes earthy, sour-tinged, and robust.",
			Smell: "The scent is dark bread and damp grain.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.European
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodRiceWaterDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_rice_water_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.GrainDrink,
			Name: "rice water drink",
			Description: "milky white rice water",
			LongDescription: "Rice starch makes this drink milky white despite its light body.",
			Taste: "It tastes clean, soothing, and gently starchy.",
			Smell: "Steamed rice gives it a quiet sweet aroma.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.EastAsian
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodToastedRiceDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_toasted_rice_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.GrainDrink,
			Name: "toasted rice drink",
			Description: "pale gold roasted-rice drink",
			LongDescription: "Toasted rice colours the strained liquor pale gold and clear.",
			Taste: "It tastes nutty, light, and delicately scorched.",
			Smell: "The fragrance recalls roasted rice and warm straw.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.EastAsian
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodMilletWaterDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_millet_water_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.GrainDrink,
			Name: "millet water drink",
			Description: "soft yellow millet water",
			LongDescription: "Fine millet particles leave this grain drink soft yellow and hazy.",
			Taste: "It tastes grassy, mild, and faintly nutty.",
			Smell: "Warm millet gives a dry cereal fragrance.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSorghumWaterDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_sorghum_water_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.GrainDrink,
			Name: "sorghum water drink",
			Description: "russet cloudy sorghum water",
			LongDescription: "Reddish sorghum meal gives the light drink a russet cloudiness.",
			Taste: "It tastes earthy, mineral, and subtly sweet.",
			Smell: "The smell is warm sorghum and dry grass.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.SubSaharanAfrican
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodMaizeWaterDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_maize_water_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.GrainDrink,
			Name: "maize water drink",
			Description: "cloudy yellow maize water",
			LongDescription: "Fine maize meal remains suspended in this cloudy yellow drink.",
			Taste: "It tastes distinctly corny, mild, and sweet.",
			Smell: "Fresh meal gives it a warm maize aroma.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.IndigenousAmerican
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSproutedGrainDrink() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_sprouted_grain_drink",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.GrainDrink,
			Name: "sprouted grain drink",
			Description: "pale sweet sprout drink",
			LongDescription: "Crushed sprouts make this strained grain drink pale and faintly green.",
			Taste: "It tastes fresh, grassy, and naturally malty.",
			Smell: "The aroma is young grain and damp straw.",
			Colour: "green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodHoneyedGrainWater() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_honeyed_grain_water",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.GrainDrink,
			Name: "honeyed grain water",
			Description: "amber-sweet grain water",
			LongDescription: "A little honey gives cloudy grain water an amber surface glint.",
			Taste: "It tastes floral, cereal-soft, and gently sweet.",
			Smell: "Honey and warm meal mingle in the scent.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1.5,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSpicedGrainWater() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_spiced_grain_water",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.GrainDrink,
			Name: "spiced grain water",
			Description: "aromatic tan grain drink",
			LongDescription: "Ground spice flecks remain suspended through a light tan grain drink.",
			Taste: "It tastes warm, cereal-like, and gently aromatic.",
			Smell: "The fragrance combines grain with crushed spice.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodJuniperSmokedSmallBeer() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_juniper_smoked_small_beer",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "juniper-smoked small beer",
			Description: "pale smoky juniper beer",
			LongDescription: "Juniper twigs used over the malt fire leave this weak beer pale gold beneath a thin resin-scented haze.",
			Taste: "Light cereal sweetness meets dry evergreen bitterness and a restrained smoky close.",
			Smell: "Fresh yeast mingles with juniper wood and cool malt.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0.03,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSmokedMaltStrongBeer() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_smoked_malt_strong_beer",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "smoked-malt strong beer",
			Description: "deep amber smoked beer",
			LongDescription: "Dark malt dried over a smoky fire gives this strong beer a deep amber body and a persistent cream-coloured head.",
			Taste: "Toast and caramel grain lead into woody smoke, firm bitterness, and alcoholic warmth.",
			Smell: "The foam smells of browned malt, hearth smoke, and active yeast.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0.08,
			WaterLitresPerLitre: 0.65,
			FoodSatiatedHoursPerLitre: 1.5,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodWheatBeer() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_wheat_beer",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "wheat beer",
			Description: "pale hazy wheat beer",
			LongDescription: "Suspended yeast makes this pale wheat beer opaquely golden.",
			Taste: "It tastes soft, bready, and lightly tart.",
			Smell: "Fresh dough and fermentation shape its smell.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0.05,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodRyeBeer() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_rye_beer",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "rye beer",
			Description: "dark russet rye beer",
			LongDescription: "Rye malt gives this beer a dark russet colour and close foam.",
			Taste: "It tastes earthy, spicy, and faintly sour.",
			Smell: "Dark bread and warm yeast rise together.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0.05,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.European
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodMilletBeer() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_millet_beer",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "millet beer",
			Description: "cloudy yellow millet beer",
			LongDescription: "Fine grain sediment clouds this softly fermenting yellow beer.",
			Taste: "It tastes grassy, tart, and mildly yeasty.",
			Smell: "The scent is sour grain and dry straw.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0.03,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.SubSaharanAfrican
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSorghumBeer() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_sorghum_beer",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "sorghum beer",
			Description: "cloudy red-brown sorghum beer",
			LongDescription: "Russet grain sediment gives this sorghum beer a thick red-brown cast.",
			Taste: "It tastes earthy, sour, and faintly sweet.",
			Smell: "Fermented sorghum lends a robust aroma.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0.04,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 1.5,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.SubSaharanAfrican
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodMaizeBeer() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_maize_beer",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "maize beer",
			Description: "cloudy golden maize beer",
			LongDescription: "A loose layer of foam rests over a cloudy golden fermented maize drink.",
			Taste: "It tastes corny, tart, and gently alcoholic.",
			Smell: "Sour maize and yeast scent the liquid.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0.03,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 1.5,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.IndigenousAmerican
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodRiceBeer() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_rice_beer",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "rice beer",
			Description: "pale milky rice beer",
			LongDescription: "Fermented rice leaves this drink pale, milky, and softly effervescent.",
			Taste: "It tastes mellow, sour-sweet, and yeasty.",
			Smell: "Rice mash and light alcohol shape its aroma.",
			Colour: "white",
			AlcoholLitresPerLitre: 0.04,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.EastAsian
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSproutedGrainAle() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_sprouted_grain_ale",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "sprouted grain ale",
			Description: "amber sprouted-grain ale",
			LongDescription: "Mixed sprouted grains give this ale an uneven amber haze.",
			Taste: "It tastes malty, grassy, and modestly bitter.",
			Smell: "Sprouts, toast, and yeast fill the nose.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0.05,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 1.5,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSmallGrainBeer() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_small_grain_beer",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "small grain beer",
			Description: "thin pale household beer",
			LongDescription: "This weak pale beer shows little foam and a light grain haze.",
			Taste: "It tastes thin, bready, and gently sour.",
			Smell: "The smell is mild malt and active yeast.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0.02,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 3,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodHoneyFerment() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_honey_ferment",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "honey ferment",
			Description: "clear gold honey ferment",
			LongDescription: "Fermented honey produces a clear gold drink with sparse rising bubbles.",
			Taste: "It tastes floral, dry, and warmly alcoholic.",
			Smell: "Honey, wax, and yeast perfume the drink.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0.08,
			WaterLitresPerLitre: 0.65,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);
}
