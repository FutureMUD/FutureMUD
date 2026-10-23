#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class FoodLiquidsStock
{
	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidOttomanCloveCoffee() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_ottoman_clove_coffee",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Coffee,
			Name: "Ottoman clove coffeehouse liquor",
			Description: "black clove coffeehouse coffee",
			LongDescription: "Fine grounds make this Ottoman coffeehouse serving nearly black, with clove specks caught beneath a narrow ring of tan foam.",
			Taste: "Forceful roast bitterness gives way to clove warmth and a dry, lingering finish.",
			Smell: "Its steam carries dark coffee, crushed clove bud, and hot copper.",
			Colour: "Bold Black",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidSafavidRoseCoffee() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_safavid_rose_coffee",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Coffee,
			Name: "Safavid rosewater coffee",
			Description: "dark rosewater coffee",
			LongDescription: "A few drops of rosewater perfume this dark Safavid coffee without clearing the fine sediment gathered at the cup's base.",
			Taste: "Bitter coffee and floral rose alternate across the palate, ending clean and aromatic.",
			Smell: "It smells of roasted beans, distilled rose petals, and faint cardamom.",
			Colour: "Magenta",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidMughalAlmondCoffee() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_mughal_almond_coffee",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Coffee,
			Name: "Mughal almond-cardamom coffee",
			Description: "brown almond cardamom coffee",
			LongDescription: "Strained almond milk softens this Mughal coffee to warm brown, while green cardamom flecks remain within its light foam.",
			Taste: "Roast bitterness is rounded by nutty creaminess and the cool warmth of cardamom.",
			Smell: "The cup gives off coffee, crushed almonds, cardamom pod, and rose.",
			Colour: "Bold Orange",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 1.5,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidCompanyBoheaTea() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_company_bohea_tea",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Tea,
			Name: "company-house bohea tea liquor",
			Description: "clear copper bohea tea",
			LongDescription: "Dark bohea leaves produce a clear copper infusion served in European company houses tied to the expanding Asian tea trade.",
			Taste: "Dry tannin and faint smoke dominate, with a woody sweetness emerging after swallowing.",
			Smell: "It smells of dark tea leaf, seasoned packing wood, and warm porcelain.",
			Colour: "Orange",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidQingJasmineTea() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_qing_jasmine_tea",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Tea,
			Name: "Qing jasmine-scented tea liquor",
			Description: "pale jasmine-scented tea",
			LongDescription: "Repeated scenting with jasmine blossoms leaves this Qing tea pale gold and brilliantly clear, with no petals in the finished cup.",
			Taste: "Delicate leaf astringency supports fresh jasmine and a softly sweet floral close.",
			Smell: "Its refined fragrance joins young tea, night-blooming jasmine, and clean steam.",
			Colour: "Bold Yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidTokugawaRoastedTwigTea() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_tokugawa_roasted_twig_tea",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Tea,
			Name: "Tokugawa roasted-twig tea liquor",
			Description: "amber roasted twig tea",
			LongDescription: "Roasted tea stems and coarse leaf make this Tokugawa household infusion amber, clear, and lighter than powdered ceremonial tea.",
			Taste: "Toasty wood and mild green leaf create a low-tannin drink with a nutty finish.",
			Smell: "It smells of warmed twigs, dry tea leaf, and a faint charcoal fire.",
			Colour: "Yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidJoseonOmijaPunch() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_joseon_omija_punch",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "Joseon omija-cinnamon punch",
			Description: "crimson omija cinnamon punch",
			LongDescription: "Steeped five-flavour berries colour this Joseon court punch crimson, with pine nuts floating above a clear cinnamon-scented liquor.",
			Taste: "Sweetness, acidity, bitterness, salt, and spice unfold around the vivid red fruit.",
			Smell: "The bouquet suggests tart berries, pine kernel, cinnamon bark, and honey.",
			Colour: "Bold Red",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidSpanishCinnamonChocolate() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_spanish_cinnamon_chocolate",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Chocolate,
			Name: "Spanish Atlantic cinnamon chocolate",
			Description: "dark cinnamon drinking chocolate",
			LongDescription: "Ground cacao, cane sugar, and cinnamon form a dark Spanish Atlantic chocolate, with a dense froth raised by repeated beating.",
			Taste: "Roasted cacao bitterness meets pronounced sugar and cinnamon before a thick, warming finish.",
			Smell: "It smells richly of toasted cacao, cassia, hot sugar, and earthenware.",
			Colour: "Bold Black",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.65,
			FoodSatiatedHoursPerLitre: 4,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidFrenchVanillaMilkChocolate() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_french_vanilla_milk_chocolate",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Chocolate,
			Name: "French court vanilla milk chocolate",
			Description: "silky vanilla milk chocolate",
			LongDescription: "Milk gives this French court chocolate a silky brown body, capped by pale foam and scented with imported vanilla.",
			Taste: "Creamy sweetness softens the cacao while vanilla supplies a rounded floral depth.",
			Smell: "Its aroma combines warmed milk, roasted cacao, vanilla pod, and sugar.",
			Colour: "Bold Orange",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.65,
			FoodSatiatedHoursPerLitre: 5,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidCaribbeanCacaoShellInfusion() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_caribbean_cacao_shell_infusion",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Chocolate,
			Name: "Caribbean cacao-shell plantation infusion",
			Description: "reddish cacao-shell infusion",
			LongDescription: "Cacao shells from a Caribbean plantation economy built on coerced labour tint this thrifty infusion a transparent reddish brown.",
			Taste: "Light cacao, woody husk, and a little molasses create a dry, modest chocolate flavour.",
			Smell: "It smells of toasted cacao shells, cane molasses, smoke, and boiling water.",
			Colour: "Red",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidBataviaArrackPunch() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_batavia_arrack_punch",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Spirit,
			Name: "Batavia arrack punch",
			Description: "cloudy amber arrack punch",
			LongDescription: "Arrack, citrus, sugar, and spice mingle in this Batavian colonial-port punch, producing a cloudy amber bowl with floating peel.",
			Taste: "Strong cane-and-rice spirit is checked by sour citrus and dark sugar sweetness.",
			Smell: "Its heady scent carries arrack, lime peel, nutmeg, and warm syrup.",
			Colour: "Orange",
			AlcoholLitresPerLitre: 0.15,
			WaterLitresPerLitre: 0.65,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 0.25,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidVernonNavalGrog() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_vernon_naval_grog",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Spirit,
			Name: "Vernon naval rum-water",
			Description: "pale brown naval rum-water",
			LongDescription: "A measured rum ration is diluted into this pale brown naval drink following the shipboard practice ordered by Admiral Vernon in 1740.",
			Taste: "Molasses-rich spirit is thinned by water, leaving a brisk taste and moderate alcoholic warmth.",
			Smell: "It smells of dark rum, fresh water, damp wood, and a tarred deck.",
			Colour: "Yellow",
			AlcoholLitresPerLitre: 0.08,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidShipboardSpruceBeer() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_shipboard_spruce_beer",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "shipboard spruce-molasses beer",
			Description: "brown spruce molasses beer",
			LongDescription: "Spruce tips and molasses ferment into a brown shipboard beer, with lively bubbles and a loose tan head.",
			Taste: "Resinous evergreen meets dark cane sweetness over a lightly sour fermented base.",
			Smell: "Its aroma suggests crushed spruce, molasses, yeast, and a tarred cask.",
			Colour: "Bold Orange",
			AlcoholLitresPerLitre: 0.03,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidCompanyTamarindPunch() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_company_tamarind_punch",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "company-settlement tamarind punch",
			Description: "cloudy brown tamarind punch",
			LongDescription: "Tamarind pulp clouds this Indian Ocean company-settlement punch brown, while citrus peel and spice drift near the surface.",
			Taste: "Assertive sour fruit is rounded by sugar, ginger, and a faint peppery bite.",
			Smell: "It smells of tamarind pod, lime rind, ginger root, and palm sugar.",
			Colour: "Bold Orange",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidRouxBrownGravy() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_roux_brown_gravy",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Sauce,
			Name: "roux-bound brown gravy",
			Description: "glossy roux-bound gravy",
			LongDescription: "Flour browned in fat binds this refined meat gravy into a smooth chestnut sauce that coats the back of a spoon.",
			Taste: "Roasted stock and toasted flour give deep savour, balanced by onion and pepper.",
			Smell: "Its dense fragrance recalls pan drippings, brown flour, onion, and herbs.",
			Colour: "Bold Orange",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidButterCaperSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_butter_caper_sauce",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Sauce,
			Name: "butter-mounted caper sauce",
			Description: "pale butter caper sauce",
			LongDescription: "Carefully worked butter makes this court sauce pale and glossy, with chopped capers dispersed through the warm emulsion.",
			Taste: "Rich butter is cut by caper brine, lemonlike acidity, and a salty finish.",
			Smell: "It smells of melted butter, crushed capers, sour juice, and parsley.",
			Colour: "Bold Yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidCreamMushroomSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_cream_mushroom_sauce",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Sauce,
			Name: "cream-bound mushroom sauce",
			Description: "taupe cream mushroom sauce",
			LongDescription: "Finely minced mushrooms fleck a smooth taupe cream sauce whose slow pour gathers thickly around roasted food.",
			Taste: "Dairy richness carries woodland mushroom, nutmeg, and a restrained pepper warmth.",
			Smell: "Its aroma blends cooked cream, browned caps, nutmeg, and shallot.",
			Colour: "White",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidAnchovyLemonEssence() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_anchovy_lemon_essence",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Sauce,
			Name: "anchovy-lemon table essence",
			Description: "clear brown anchovy essence",
			LongDescription: "Clarified anchovy liquor and citrus make this early bottled table essence clear brown, with only fine sediment below.",
			Taste: "Intense marine salt is sharpened by citrus peel, spice, and fermented depth.",
			Smell: "It has a penetrating smell of cured fish, lemon rind, vinegar, and clove.",
			Colour: "Bold Black",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidTomatoHerbSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_tomato_herb_sauce",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Sauce,
			Name: "early tomato-herb cooking sauce",
			Description: "red tomato herb sauce",
			LongDescription: "Cooked New World tomatoes form a red seedy sauce in early European adoption contexts, brightened by chopped garden herbs.",
			Taste: "Fruit acidity and gentle sweetness support garlic, pepper, and a fresh herbal finish.",
			Smell: "It smells of simmered tomato, crushed garlic, basil-like leaves, and warm oil.",
			Colour: "Bold Red",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 1.5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidTokugawaPonzu() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_tokugawa_ponzu",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Sauce,
			Name: "Tokugawa citrus-soy ponzu",
			Description: "dark citrus soy seasoning",
			LongDescription: "Soy liquor and pressed citrus form this Tokugawa seasoning, a clear dark brown liquid with a sharp aromatic surface.",
			Taste: "Salty fermentation meets brisk citrus and a clean, savoury marine undertone.",
			Smell: "The sauce smells of soy, fresh peel, dried fish, and rice wine.",
			Colour: "Bold Black",
			AlcoholLitresPerLitre: 0.01,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidCherryRatafia() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_cherry_ratafia",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Spirit,
			Name: "cherry-kernel ratafia cordial",
			Description: "ruby cherry ratafia",
			LongDescription: "Cherries and cracked kernels steep in brandy to produce this ruby cordial, with a slight syrupy weight in the glass.",
			Taste: "Sweet red fruit leads into almondlike kernel bitterness and warming spirit.",
			Smell: "It smells of ripe cherries, stone kernels, brandy, and refined sugar.",
			Colour: "Bold Red",
			AlcoholLitresPerLitre: 0.25,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidOrangeflowerBrandyCordial() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_orangeflower_brandy_cordial",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Spirit,
			Name: "orange-flower brandy cordial",
			Description: "clear orange-flower cordial",
			LongDescription: "Distilled orange blossom water perfumes this clear brandy cordial, which carries no colour beyond a faint golden cast.",
			Taste: "Alcoholic heat opens into citrus flower, honey, and a delicate bitter-orange close.",
			Smell: "Its fragrance is bright with brandy, white blossom, orange peel, and syrup.",
			Colour: "Bold Yellow",
			AlcoholLitresPerLitre: 0.25,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidJuniperCorianderCordial() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_juniper_coriander_cordial",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Spirit,
			Name: "juniper-coriander cordial spirit",
			Description: "clear juniper coriander spirit",
			LongDescription: "Juniper berries and coriander seed flavour this clear distilled cordial, leaving a few droplets of aromatic oil at the rim.",
			Taste: "Piney juniper dominates before citruslike coriander and dry spirit take over.",
			Smell: "It smells sharply of crushed evergreen berries, coriander, and clean alcohol.",
			Colour: "Green",
			AlcoholLitresPerLitre: 0.4,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidPlantationMolassesSyrup() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_plantation_molasses_syrup",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Syrup,
			Name: "Atlantic plantation molasses syrup",
			Description: "black plantation molasses",
			LongDescription: "This nearly black refinery syrup comes from an Atlantic plantation sugar system dependent on enslaved and coerced labour.",
			Taste: "Burnt cane sweetness, mineral bitterness, and smoke make its flavour heavy and persistent.",
			Smell: "It smells of boiled cane juice, dark caramel, iron kettles, and char.",
			Colour: "Bold Black",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 4,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry EarlymodernFoodLiquidCoffeehouseOrgeat() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "earlymodern_food_liquid_coffeehouse_orgeat",
			Scope: FoodCatalogueScope.EarlyModern,
			Family: FoodCatalogueFamily.Syrup,
			Name: "coffeehouse orgeat concentrate",
			Description: "milky coffeehouse orgeat",
			LongDescription: "Almonds and orange-flower water make this coffeehouse syrup milky white, with sugar giving it a slow ribboning pour.",
			Taste: "Dense almond sweetness is lifted by citrus blossom and a faint kernel bitterness.",
			Smell: "Its perfume combines almond paste, orange flowers, sugar, and cool water.",
			Colour: "White",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 3,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidLentenAlmondDraught() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_lenten_almond_draught",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.DairyDrink,
			Name: "Lenten almond draught",
			Description: "cloudy ivory almond draught",
			LongDescription: "This fasting-day drink is an opaque ivory emulsion, with a fine almond sediment settling slowly beneath its creamy surface.",
			Taste: "It tastes softly nutty and faintly sweet, leaving a clean almond richness without any dairy tang.",
			Smell: "Its mild scent recalls freshly crushed almonds, warm water, and a trace of honey.",
			Colour: "White",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 3,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidConventWalnutMilk() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_convent_walnut_milk",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.DairyDrink,
			Name: "convent walnut milk",
			Description: "pale brown walnut milk",
			LongDescription: "Ground walnuts give this monastic fasting milk a pale taupe colour, a slightly grainy body, and a thin layer of oil at the rim.",
			Taste: "Deep walnut flavour brings gentle bitterness and earthy richness, rounded by a restrained touch of honey.",
			Smell: "It smells of cracked walnut shells, fresh kernels, and cool stoneware.",
			Colour: "Bold White",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.65,
			FoodSatiatedHoursPerLitre: 3,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidPoppySeedFastEmulsion() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_poppy_seed_fast_emulsion",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.DairyDrink,
			Name: "poppy-seed fast emulsion",
			Description: "speckled poppy-seed emulsion",
			LongDescription: "Tiny grey seed flecks remain suspended through this light fasting emulsion, which pours smoothly despite its visibly ground texture.",
			Taste: "The drink is delicately nutty and grassy, with a mild seed bitterness at the finish.",
			Smell: "A quiet aroma of crushed poppy seed and damp meal rises from it.",
			Colour: "White",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidGreenGrapeVerjuice() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_green_grape_verjuice",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.Sauce,
			Name: "green-grape cook's verjuice",
			Description: "sharp green-grape verjuice",
			LongDescription: "Pressed from unripe grapes, this clear yellow-green cookery liquid has a brisk acidity and a faint natural haze near the bottom.",
			Taste: "Its first impression is sharply sour, followed by green grape, mineral freshness, and almost no sweetness.",
			Smell: "It carries the bright smell of crushed young grapes and snapped vine stems.",
			Colour: "Green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidSorrelVerjuiceSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_sorrel_verjuice_sauce",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.Sauce,
			Name: "sorrel verjuice sauce",
			Description: "green sorrel-verjuice sauce",
			LongDescription: "Chopped sorrel clouds this tart sauce a deep leaf-green, while strained herb fragments collect along the sides of its vessel.",
			Taste: "The sauce tastes vividly sour and leafy, with pepper and salt sharpening its clean finish.",
			Smell: "Its aroma combines bruised sorrel, green grape juice, and fresh-cut herbs.",
			Colour: "Bold Green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidClarifiedHippocras() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_clarified_hippocras",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.Wine,
			Name: "clarified court hippocras",
			Description: "clear ruby court hippocras",
			LongDescription: "Careful straining leaves this spiced red wine brilliantly clear, with a ruby colour and no visible spice sediment.",
			Taste: "Sweet wine leads into cinnamon, ginger, and clove, then closes with warming alcohol and dry spice.",
			Smell: "It smells richly of red wine, cassia bark, ginger, and a little honey.",
			Colour: "Bold Red",
			AlcoholLitresPerLitre: 0.12,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 0.25,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidWarmedPepperedWine() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_warmed_peppered_wine",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.Wine,
			Name: "warmed peppered wine",
			Description: "dark warmed pepper wine",
			LongDescription: "This heated wine is dark garnet and lightly steaming, with ground pepper settling in a fine ring around the cup.",
			Taste: "Warm red wine and black pepper dominate, followed by muted sweetness and a firmly alcoholic finish.",
			Smell: "Steam carries vinous fruit, cracked pepper, and the smoky scent of the heating pot.",
			Colour: "Red",
			AlcoholLitresPerLitre: 0.1,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 0.25,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidMonasticHerbWine() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_monastic_herb_wine",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.Wine,
			Name: "monastic bitter-herb wine",
			Description: "green-brown monastic herb wine",
			LongDescription: "A measured steeping of bitter herbs gives this table wine a green-brown cast and leaves a few fine particles after straining.",
			Taste: "Dry wine supports wormwood-like bitterness, garden herbs, and a long medicinal warmth.",
			Smell: "Its stern fragrance suggests dried leaves, resinous stems, and pale wine.",
			Colour: "Yellow",
			AlcoholLitresPerLitre: 0.08,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 0.25,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidHoneyedPomegranateDraught() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_honeyed_pomegranate_draught",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "honeyed pomegranate table draught",
			Description: "crimson honeyed pomegranate draught",
			LongDescription: "Diluted pomegranate juice colours this courtly table drink a clear crimson, with honey adding a slight viscosity to its flow.",
			Taste: "Tart red fruit arrives first, then floral honey softens the acidity and leaves a gently sweet finish.",
			Smell: "The draught smells of split pomegranate, warm honey, and fresh water.",
			Colour: "Magenta",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidInfirmaryBarleyWater() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_infirmary_barley_water",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.GrainDrink,
			Name: "infirmary barley water",
			Description: "cloudy pale barley water",
			LongDescription: "Long-simmered barley has left this infirmary drink pale and faintly cloudy, with only a few grains escaping the final straining.",
			Taste: "It tastes mild, starchy, and lightly toasted, carrying little more than barley sweetness and clean water.",
			Smell: "A soft scent of boiled grain and warm earthenware hangs above it.",
			Colour: "Bold White",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);
}
