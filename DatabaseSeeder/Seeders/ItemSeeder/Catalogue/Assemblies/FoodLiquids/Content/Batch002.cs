#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class FoodLiquidsStock
{
	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidBreadCaudle() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_bread_caudle",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.GrainDrink,
			Name: "bread-thickened sickroom caudle",
			Description: "warm beige bread caudle",
			LongDescription: "Fine bread crumbs thicken this warm sickroom drink to a smooth beige pour, with spice dust visible across the top.",
			Taste: "Soft wheat and warm broth give way to ginger and a modest salted richness.",
			Smell: "It smells of soaked bread, gentle spice, and recently heated stock.",
			Colour: "Yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 3,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidAlmondSaffronCaudle() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_almond_saffron_caudle",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.DairyDrink,
			Name: "almond-saffron caudle",
			Description: "golden almond-saffron caudle",
			LongDescription: "Almond emulsion and saffron make this fasting caudle creamy and golden, with an occasional red thread drifting through it.",
			Taste: "Nutty almond body carries floral saffron, mild sweetness, and a warming ginger close.",
			Smell: "Its perfume blends crushed almonds, haylike saffron, and honeyed spice.",
			Colour: "Bold Yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.65,
			FoodSatiatedHoursPerLitre: 4,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidMonasticParsleyBroth() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_monastic_parsley_broth",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.Broth,
			Name: "monastic parsley broth",
			Description: "clear green-flecked parsley broth",
			LongDescription: "This modest monastic broth is nearly clear, scattered with chopped parsley and a few softened leek threads.",
			Taste: "Fresh parsley and mild allium lift a clean salted broth with very little fat.",
			Smell: "It gives off the humble fragrance of garden herbs, leek, and steaming stock.",
			Colour: "Green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidFishDaySaffronBroth() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_fish_day_saffron_broth",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.Broth,
			Name: "fish-day saffron broth",
			Description: "golden fish-day saffron broth",
			LongDescription: "Saffron turns this lean fish-day broth a luminous gold, while tiny herb flecks and a pearly sheen mark its surface.",
			Taste: "A delicate briny savour is brightened by saffron, parsley, and gentle pepper.",
			Smell: "The steam smells of light fish stock, saffron threads, and chopped greens.",
			Colour: "Bold Yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 1.5,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidCaponRestorationBroth() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_capon_restoration_broth",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.Broth,
			Name: "capon restoration broth",
			Description: "rich amber capon broth",
			LongDescription: "Slowly strained capon stock forms a clear amber broth with small golden beads of fat moving across the surface.",
			Taste: "It tastes deeply poultry-rich yet clean, finishing with sweet root vegetables and warming herbs.",
			Smell: "A full aroma of roasted bird, parsley roots, and pepper rises with the steam.",
			Colour: "Orange",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 3,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidAleOnionPotLiquor() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_ale_onion_pot_liquor",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.Broth,
			Name: "ale-onion pottage liquor",
			Description: "brown ale-onion pot liquor",
			LongDescription: "Onion-softened ale makes this pottage liquor brown and faintly opaque, with a thin foam gathering wherever it is stirred.",
			Taste: "Malted bitterness sits beneath caramelised onion, salt, and a lingering breadlike savour.",
			Smell: "It smells of dark ale, browned onions, and the warm interior of a cooking pot.",
			Colour: "Bold Orange",
			AlcoholLitresPerLitre: 0.02,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 1.5,
			DrinkSatiatedHoursPerLitre: 0.25,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidSpicedGalantineSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_spiced_galantine_sauce",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.Sauce,
			Name: "spiced galantine sauce",
			Description: "dark spiced galantine sauce",
			LongDescription: "Bread and dark stock give this banquet sauce a smooth, spoon-coating body, stained brown by wine and ground spices.",
			Taste: "Sour wine, pepper, and cinnamon cut through its rich meat-stock depth with a sharp finish.",
			Smell: "Its dense aroma mixes roasted stock, wine vinegar, warm spice, and bread.",
			Colour: "Bold Black",
			AlcoholLitresPerLitre: 0.01,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidGreenSawse() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_green_sawse",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.Sauce,
			Name: "medieval green herb sawse",
			Description: "vivid medieval green sawse",
			LongDescription: "Pounded parsley and other garden herbs colour this cold sauce vivid green, with bread lending a lightly thickened texture.",
			Taste: "Fresh herbs, vinegar, and garlic make it bright, pungent, and sharply appetite-wakening.",
			Smell: "It smells intensely of crushed parsley, sour wine, garlic, and damp bread.",
			Colour: "Bold Green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidBreadGarlicJance() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_bread_garlic_jance",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.Sauce,
			Name: "breaded garlic jance",
			Description: "pale breaded garlic sauce",
			LongDescription: "Soaked bread and crushed garlic form a pale, creamy medieval sauce with a rougher grain than dairy cream.",
			Taste: "Strong garlic leads, softened by wheat, broth, and enough acidity to keep the sauce lively.",
			Smell: "The sauce smells of raw garlic, wet bread, and a small measure of vinegar.",
			Colour: "White",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 1.5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidSpicedMustReduction() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_spiced_must_reduction",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.Syrup,
			Name: "spiced must reduction",
			Description: "purple-brown spiced must syrup",
			LongDescription: "Grape must has been simmered to a purple-brown syrup, carrying fine spice sediment and a slow, sticky pour.",
			Taste: "Concentrated grape sweetness meets cinnamon and pepper before a dark caramel finish.",
			Smell: "It has the heady scent of boiling must, dried spice, and scorched sugar.",
			Colour: "Magenta",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidQuinceHoneySyrup() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_quince_honey_syrup",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.Syrup,
			Name: "quince-and-honey electuary syrup",
			Description: "amber quince honey syrup",
			LongDescription: "Cooked quince and honey produce a translucent amber syrup, thick enough to trail slowly from a spoon.",
			Taste: "Floral honey surrounds tart quince, leaving a perfumed sweetness and slight fruit astringency.",
			Smell: "Its fragrance recalls poached quince, meadow honey, and a trace of warm spice.",
			Colour: "Orange",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidVioletSugarSyrup() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_violet_sugar_syrup",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.Syrup,
			Name: "late-medieval violet syrup",
			Description: "purple floral violet syrup",
			LongDescription: "Steeped violet petals tint this prized syrup a delicate purple, with clarified sugar giving it a glassy consistency.",
			Taste: "The sweetness is distinctly floral, softened by cool water and a faint green petal note.",
			Smell: "It smells of fresh violets, clean sugar syrup, and spring leaves.",
			Colour: "Bold Magenta",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidSpicedOxymel() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_spiced_oxymel",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "spiced monastic oxymel",
			Description: "cloudy amber spiced oxymel",
			LongDescription: "Honey and vinegar mingle in this cloudy amber monastic preparation, with crushed spices settling below the surface.",
			Taste: "It tastes equally sweet and sour, followed by ginger warmth and a dry vinegar prickle.",
			Smell: "A sharp honeyed aroma carries vinegar, ginger, and bruised medicinal herbs.",
			Colour: "Yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.65,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidAleMustardSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_ale_mustard_sauce",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.Sauce,
			Name: "ale-mustard trencher sauce",
			Description: "ochre ale-mustard sauce",
			LongDescription: "Ground mustard suspended in small ale makes this trencher sauce ochre and grainy, with bubbles clinging around its rim.",
			Taste: "Hot mustard bites first, then malt sweetness and light bitterness temper the heat.",
			Smell: "Its pungent smell joins crushed mustard seed, fresh ale, and sour bread.",
			Colour: "Bold Yellow",
			AlcoholLitresPerLitre: 0.02,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry MedievalFoodLiquidOatSouringLiquor() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "medieval_food_liquid_oat_souring_liquor",
			Scope: FoodCatalogueScope.Medieval,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "oat-pottage souring liquor",
			Description: "cloudy grey oat souring liquor",
			LongDescription: "A short fermentation has made this oat liquor cloudy grey and lightly effervescent, with starch settling in soft layers.",
			Taste: "Gentle lactic sourness runs through raw oat flavour, ending dry, yeasty, and faintly salty.",
			Smell: "It smells of wet oats, active yeast, and the clean tang of fermentation.",
			Colour: "Bold White",
			AlcoholLitresPerLitre: 0.01,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidRosewaterVerjuiceEmulsion() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_rosewater_verjuice_emulsion",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Sauce,
			Name: "rosewater verjuice emulsion",
			Description: "pale rosewater-verjuice emulsion",
			LongDescription: "Whisked almond and verjuice make this pale court sauce lightly opaque, with rosewater leaving a delicate sheen above the sediment.",
			Taste: "Sharp green grape is softened by almond body and a restrained floral finish.",
			Smell: "It smells of unripe grapes, crushed almonds, and newly distilled rosewater.",
			Colour: "White",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidBitterOrangeAgrodolce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_bitter_orange_agrodolce",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Sauce,
			Name: "bitter-orange agrodolce reduction",
			Description: "amber bitter-orange agrodolce",
			LongDescription: "Reduced citrus juice, wine vinegar, and sugar form a clear amber sauce whose slow drops cling to the serving spoon.",
			Taste: "Bitter orange peel meets bright acidity and dark caramel sweetness in quick succession.",
			Smell: "Its fragrance combines scorched sugar, fresh citrus rind, and sharp vinegar.",
			Colour: "Orange",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidCaperCinnamonCourtSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_caper_cinnamon_court_sauce",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Sauce,
			Name: "caper-cinnamon court sauce",
			Description: "brown caper and cinnamon sauce",
			LongDescription: "Finely minced capers freckle this smooth brown banquet sauce, with cinnamon giving the reduced stock a warmer colour.",
			Taste: "Salty caper bite cuts through savoury stock before cinnamon supplies dry warmth.",
			Smell: "It carries aromas of brined capers, roasted meat liquor, and cassia bark.",
			Colour: "Bold Orange",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidRaisinPinenutSauce() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_raisin_pinenut_sauce",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Sauce,
			Name: "raisin-pine-nut court sauce",
			Description: "purple-brown raisin pine-nut sauce",
			LongDescription: "Pounded pine nuts thicken a purple-brown raisin reduction, leaving tiny ivory grains suspended in its glossy body.",
			Taste: "Fruit sweetness and creamy nuts are checked by sour wine and black pepper.",
			Smell: "The sauce smells of soaked raisins, crushed pine kernels, and warming spice.",
			Colour: "Magenta",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidRosemaryAquavitae() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_rosemary_aquavitae",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Spirit,
			Name: "rosemary aqua-vitae cordial",
			Description: "clear rosemary cordial spirit",
			LongDescription: "Fresh rosemary has steeped in a clear distilled cordial, tinting it barely green while a few needles remain below the stopper.",
			Taste: "Hot spirit gives way to resinous rosemary, honey, and a dry medicinal close.",
			Smell: "Its piercing scent blends clean alcohol, crushed rosemary, and beeswax.",
			Colour: "Green",
			AlcoholLitresPerLitre: 0.3,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidCitrusPeelAquavitae() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_citrus_peel_aquavitae",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Spirit,
			Name: "citrus-peel aqua-vitae cordial",
			Description: "golden citrus cordial spirit",
			LongDescription: "Strips of bitter citrus peel colour this refined aqua vitae pale gold, with aromatic oil forming fleeting rings on top.",
			Taste: "The cordial burns warmly, then releases orange zest, lemon bitterness, and slight sweetness.",
			Smell: "It smells sharply of distilled spirit and freshly pared citrus skin.",
			Colour: "Yellow",
			AlcoholLitresPerLitre: 0.3,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidSageHoneyCordial() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_sage_honey_cordial",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Spirit,
			Name: "sage-and-honey cordial spirit",
			Description: "amber sage honey cordial",
			LongDescription: "Honey gives this sage-steeped spirit an amber cast and a heavier pour than unflavoured aqua vitae.",
			Taste: "Alcoholic heat carries earthy sage and meadow honey into a long herbal finish.",
			Smell: "A heady vapour of spirit, dried sage, and warm honey rises from it.",
			Colour: "Orange",
			AlcoholLitresPerLitre: 0.25,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidOttomanRoseSherbet() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_ottoman_rose_sherbet",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "Ottoman rose-petal sherbet",
			Description: "blush rose-petal sherbet",
			LongDescription: "A clarified petal infusion gives this chilled sherbet a faint blush, with no fragments left in the crystalline drink.",
			Taste: "Cool floral sweetness is balanced by lemonlike acidity and a clean watery finish.",
			Smell: "It has the perfume of fresh damask roses, sugar, and citrus peel.",
			Colour: "Bold Magenta",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidOttomanSourCherrySherbet() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_ottoman_sour_cherry_sherbet",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "Ottoman sour-cherry sherbet",
			Description: "ruby sour-cherry sherbet",
			LongDescription: "Pressed sour cherries colour this palace sherbet a brilliant ruby, while dilution keeps it clear and freely pouring.",
			Taste: "Tart cherry leads decisively before sugar rounds the fruit and softens its astringency.",
			Smell: "The drink smells of crushed red cherries and a little fragrant blossom water.",
			Colour: "Bold Red",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidSafavidMintSekanjabin() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_safavid_mint_sekanjabin",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Syrup,
			Name: "Safavid mint sekanjabin concentrate",
			Description: "green-amber mint sekanjabin",
			LongDescription: "Honey, vinegar, and mint have reduced to a translucent green-amber concentrate with a slow syrupy thread.",
			Taste: "Sweetness and vinegar strike together, followed by cooling mint and herbal sharpness.",
			Smell: "Its concentrated aroma joins honeycomb, wine vinegar, and bruised mint leaves.",
			Colour: "Green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidMughalSaffronAlmondSharbat() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_mughal_saffron_almond_sharbat",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.DairyDrink,
			Name: "Mughal saffron-almond sharbat",
			Description: "golden saffron almond sharbat",
			LongDescription: "Finely strained almonds make this sharbat creamy, while saffron stains it gold and leaves occasional crimson threads.",
			Taste: "Nutty richness supports saffron, cardamom, and gentle sugar without becoming cloying.",
			Smell: "It smells of almond meal, haylike saffron, green cardamom, and rose.",
			Colour: "Bold Yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 3,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidMughalSandalRoseSharbat() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_mughal_sandal_rose_sharbat",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "Mughal sandalwood-rose sharbat",
			Description: "pale pink sandalwood sharbat",
			LongDescription: "Rose infusion gives this clear court sharbat a pale pink hue, with sandalwood contributing no visible sediment after straining.",
			Taste: "Floral rose opens the taste before cool woody spice and light sweetness emerge.",
			Smell: "A refined scent of rose petals, damp sandalwood, and sugar hangs over the cup.",
			Colour: "Pink",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidOttomanCardamomCoffee() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_ottoman_cardamom_coffee",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Coffee,
			Name: "Ottoman cardamom coffee liquor",
			Description: "black cardamom coffee",
			LongDescription: "Very finely ground coffee clouds this small black serving, with pale foam and cardamom specks gathering at the rim.",
			Taste: "Intense roast bitterness is lifted by green cardamom and a fleeting caramel note.",
			Smell: "Its powerful aroma combines fresh coffee grounds, cardamom pod, and hot metal.",
			Colour: "Bold Black",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidMingSmokedPlumTea() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_ming_smoked_plum_tea",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Tea,
			Name: "Ming smoked-plum tea liquor",
			Description: "reddish smoked-plum tea",
			LongDescription: "Tea and smoked plum produce a clear reddish-brown infusion, with a single wrinkled fruit sometimes left in the pot.",
			Taste: "Tannic leaf, tart plum, and hearth smoke create a dry and refreshing flavour.",
			Smell: "Steam brings the scents of dark tea, singed plum skin, and charcoal.",
			Colour: "Red",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidMingChrysanthemumTea() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_ming_chrysanthemum_tea",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Tea,
			Name: "Ming chrysanthemum tea liquor",
			Description: "clear golden chrysanthemum tea",
			LongDescription: "Whole chrysanthemum petals tint this refined infusion clear gold, drifting open like small flowers near the surface.",
			Taste: "It tastes lightly floral and grassy, with subtle bitterness and a cooling finish.",
			Smell: "The vapour is delicate, recalling dried chrysanthemum heads and spring hay.",
			Colour: "Yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidJapaneseWhiskedTea() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_japanese_whisked_tea",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Tea,
			Name: "Japanese whisked tea liquor",
			Description: "foaming jade whisked tea",
			LongDescription: "Powdered tea has been whisked into an opaque jade-green liquor capped by a close layer of tiny bubbles.",
			Taste: "Concentrated green tea tastes grassy, creamy, and briskly bitter with lasting umami.",
			Smell: "It gives off a vivid scent of ground tea leaf, fresh grass, and warm ceramic.",
			Colour: "Bold Green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidJoseonFiveFlavourPunch() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_joseon_five_flavour_punch",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.FruitDrink,
			Name: "Joseon five-flavour berry punch",
			Description: "crimson five-flavour berry punch",
			LongDescription: "Steeped magnolia berries make this court punch a luminous crimson, with pine nuts floating sparsely across the top.",
			Taste: "Sweet, sour, bitter, salty, and pungent impressions unfold over a bright berry core.",
			Smell: "Its unusual bouquet suggests red berries, pine kernel, and faint cinnamon bark.",
			Colour: "Bold Red",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidMingBlackBeanLiquor() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_ming_black_bean_liquor",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Sauce,
			Name: "Ming fermented-black-bean cooking liquor",
			Description: "dark fermented bean liquor",
			LongDescription: "Rinsed fermented beans cloud this dark cooking liquid, leaving black fragments and a slight oily lustre behind.",
			Taste: "It is intensely salty and savoury, carrying fermented bean depth and mild garlic.",
			Smell: "The strong smell recalls aged black beans, brine, and a warm wok.",
			Colour: "Bold Black",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidJapaneseIrizake() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_japanese_irizake",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Sauce,
			Name: "Japanese iri-zake cooking liquor",
			Description: "pale amber iri-zake",
			LongDescription: "Reduced rice wine, dried plum, and fish shaving make this clear amber seasoning liquor delicately glossy.",
			Taste: "Dry rice wine supports sour plum and marine savour with restrained salt.",
			Smell: "It smells of heated sake, preserved plum, and clean dried fish.",
			Colour: "Yellow",
			AlcoholLitresPerLitre: 0.05,
			WaterLitresPerLitre: 0.65,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);

	public static PreIndustrialFoodLiquidCatalogueEntry RenaissanceFoodLiquidAchioteCacao() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "renaissance_food_liquid_achiote_cacao",
			Scope: FoodCatalogueScope.Renaissance,
			Family: FoodCatalogueFamily.Chocolate,
			Name: "achiote-red cacao drink",
			Description: "red-brown achiote cacao",
			LongDescription: "Ground cacao and achiote form a red-brown drink with persistent foam and fine dark sediment under the froth.",
			Taste: "Bitter cacao is earthy and dry, with peppery achiote and almost no sweetness.",
			Smell: "Its scent blends roasted cacao, annatto seed, maize, and aromatic flowers.",
			Colour: "Red",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 2,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.EraSpecific
		);
}
