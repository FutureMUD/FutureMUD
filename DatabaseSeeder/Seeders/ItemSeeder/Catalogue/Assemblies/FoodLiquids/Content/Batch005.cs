#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class FoodLiquidsStock
{
	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodLightHoneyFerment() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_light_honey_ferment",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "light honey ferment",
			Description: "pale sparkling honey drink",
			LongDescription: "A weak honey ferment shines pale yellow beneath a lively bead of bubbles.",
			Taste: "It tastes floral, fresh, and only mildly dry.",
			Smell: "Wildflower honey and yeast scent the glass.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0.03,
			WaterLitresPerLitre: 0.85,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodFruitHoneyFerment() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_fruit_honey_ferment",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "fruit-honey ferment",
			Description: "ruby honey-fruit ferment",
			LongDescription: "Fruit skins tint this fermented honey drink a clear ruby shade.",
			Taste: "It tastes tart, floral, and softly alcoholic.",
			Smell: "Ripe fruit and honey mingle in its aroma.",
			Colour: "red",
			AlcoholLitresPerLitre: 0.08,
			WaterLitresPerLitre: 0.65,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSpicedHoneyFerment() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_spiced_honey_ferment",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "spiced honey ferment",
			Description: "amber aromatic honey ferment",
			LongDescription: "Warm spice flecks deepen the amber colour of fermented honey.",
			Taste: "It tastes floral, warming, and richly dry.",
			Smell: "Honey and crushed aromatics fill the nose.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0.1,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodAppleFerment() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_apple_ferment",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "fermented apple drink",
			Description: "gold fermented apple drink",
			LongDescription: "Fine bubbles travel through a clear gold drink pressed from fermented apples.",
			Taste: "It tastes crisp, tart, and lightly tannic.",
			Smell: "Apple peel and fermentation scent the drink.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0.05,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPearFerment() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_pear_ferment",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "fermented pear drink",
			Description: "pale fermented pear drink",
			LongDescription: "This pale gold pear ferment has a delicate haze and lively bubbles.",
			Taste: "It tastes floral, dry, and softly sweet.",
			Smell: "Ripe pear gives a gentle perfumed smell.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0.05,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPlumFerment() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_plum_ferment",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "fermented plum drink",
			Description: "purple-red plum ferment",
			LongDescription: "Plum skins colour this fermented drink a dark translucent purple-red.",
			Taste: "It tastes winey, sour, and fruit-rich.",
			Smell: "The aroma is ripe plum and warm alcohol.",
			Colour: "magenta",
			AlcoholLitresPerLitre: 0.08,
			WaterLitresPerLitre: 0.65,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 1,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodBerryFerment() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_berry_ferment",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "fermented berry drink",
			Description: "crimson berry ferment",
			LongDescription: "A crimson fermented berry drink carries seed sediment at the bottom.",
			Taste: "It tastes sharply fruity, dry, and tannic.",
			Smell: "Wild berries and yeast give a vivid scent.",
			Colour: "red",
			AlcoholLitresPerLitre: 0.06,
			WaterLitresPerLitre: 0.75,
			FoodSatiatedHoursPerLitre: 0.5,
			DrinkSatiatedHoursPerLitre: 1.5,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodHerbFerment() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_herb_ferment",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "fermented herb drink",
			Description: "green-brown herb ferment",
			LongDescription: "Herbs and fermentation colour this drink a cloudy green-brown.",
			Taste: "It tastes sour, grassy, and gently bitter.",
			Smell: "The smell is leafy, yeasty, and sharp.",
			Colour: "green",
			AlcoholLitresPerLitre: 0.02,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 2,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodBreadFerment() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_bread_ferment",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.FermentedDrink,
			Name: "fermented bread drink",
			Description: "cloudy brown bread ferment",
			LongDescription: "Soaked dark bread leaves this weak ferment cloudy brown and lightly foaming.",
			Taste: "It tastes sour, toasted, and faintly sweet.",
			Smell: "Rye crust and yeast dominate the aroma.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0.02,
			WaterLitresPerLitre: 0.9,
			FoodSatiatedHoursPerLitre: 1,
			DrinkSatiatedHoursPerLitre: 3,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodRaisinRestoredGrapeWine() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_raisin_restored_grape_wine",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Wine,
			Name: "raisin-restored grape wine",
			Description: "golden raisin-restored wine",
			LongDescription: "Soaked raisins have restored body and sweetness to a pale grape wine, leaving it golden and slightly viscous.",
			Taste: "Dried-fruit richness rounds the wine's acidity before a warm honeyed finish.",
			Smell: "Raisin, grape skin, and faint fermentation rise from the cup.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0.12,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodAmberGrapeWine() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_amber_grape_wine",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Wine,
			Name: "amber grape wine",
			Description: "deep amber skin wine",
			LongDescription: "Extended skin contact gives this wine a deep amber colour.",
			Taste: "It tastes tannic, dry, and orchard-fruity.",
			Smell: "Grape skin and dried flowers shape the nose.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0.12,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPineResinGrapeWine() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_pine_resin_grape_wine",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Wine,
			Name: "pine-resin grape wine",
			Description: "amber resin-scented wine",
			LongDescription: "Pine resin used to seal the wine vessel lends the amber drink a faint haze and an unmistakable woodland perfume.",
			Taste: "Dry grape acidity is followed by sharp resin, herbs, and a warming tannic close.",
			Smell: "Its aroma combines fermented grapes, pine needles, and fresh-cut wood.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0.15,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0.25,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodRoseGrapeWine() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_rose_grape_wine",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Wine,
			Name: "rose grape wine",
			Description: "clear pink grape wine",
			LongDescription: "This clear pink wine carries the colour of briefly steeped grape skins.",
			Taste: "It tastes fresh, lightly tannic, and berry-like.",
			Smell: "Red fruit and flowers scent the wine.",
			Colour: "pink",
			AlcoholLitresPerLitre: 0.12,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSweetGrapeWine() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_sweet_grape_wine",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Wine,
			Name: "sweet grape wine",
			Description: "lush gold dessert wine",
			LongDescription: "A viscous golden wine moves slowly and shines beneath the light.",
			Taste: "It tastes honeyed, fruity, and warmly alcoholic.",
			Smell: "Ripe grapes and dried fruit perfume it.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0.15,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 0.25,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodDriedGrapeWine() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_dried_grape_wine",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Wine,
			Name: "dried-grape wine",
			Description: "dark raisin-sweet wine",
			LongDescription: "Wine from dried grapes appears dark amber with a dense glossy body.",
			Taste: "It tastes raisin-rich, sweet, and concentrated.",
			Smell: "Dried fruit and warm alcohol fill the nose.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0.18,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 0.25,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSpicedGrapeWine() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_spiced_grape_wine",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Wine,
			Name: "spiced grape wine",
			Description: "aromatic ruby spiced wine",
			LongDescription: "Whole spices lend a ruby wine a darker cast and warm perfume.",
			Taste: "It tastes fruity, warming, and pepper-fragrant.",
			Smell: "Grape, spice, and citrus peel scent it.",
			Colour: "red",
			AlcoholLitresPerLitre: 0.15,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0.25,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodHoneyedGrapeWine() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_honeyed_grape_wine",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Wine,
			Name: "honeyed grape wine",
			Description: "amber honeyed wine",
			LongDescription: "Honey softens this amber grape wine and gives it a rounded body.",
			Taste: "It tastes floral, winey, and richly sweet.",
			Smell: "Honey and ripe grape share the aroma.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0.15,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 0.25,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodAppleWine() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_apple_wine",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Wine,
			Name: "apple wine",
			Description: "clear gold apple wine",
			LongDescription: "Fully fermented apple juice produces a clear and warming gold wine.",
			Taste: "It tastes dry, tart, and orchard-fragrant.",
			Smell: "Mature apple and alcohol scent the glass.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0.12,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPearWine() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_pear_wine",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Wine,
			Name: "pear wine",
			Description: "pale floral pear wine",
			LongDescription: "A pale wine has been fermented from fragrant ripe pears, retaining slight cloudiness from the fruit.",
			Taste: "It tastes dry, floral, and softly rounded.",
			Smell: "Pear skin and white flowers mark the nose.",
			Colour: "white",
			AlcoholLitresPerLitre: 0.12,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPlumWine() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_plum_wine",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Wine,
			Name: "plum wine",
			Description: "ruby-purple plum wine",
			LongDescription: "A clear ruby-purple wine carries the dark colour of ripe plums.",
			Taste: "It tastes sweet-sour, warming, and winey.",
			Smell: "Plum skin and stone fruit scent it.",
			Colour: "magenta",
			AlcoholLitresPerLitre: 0.15,
			WaterLitresPerLitre: 0.25,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 0.25,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPomegranateWine() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_pomegranate_wine",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Wine,
			Name: "pomegranate wine",
			Description: "clear crimson pomegranate wine",
			LongDescription: "A clear crimson wine shows a vivid jewel-like colour in the cup.",
			Taste: "It tastes tart, tannic, and fruit-bright.",
			Smell: "Pomegranate rind and red fruit perfume it.",
			Colour: "red",
			AlcoholLitresPerLitre: 0.12,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0.5,
			AdmissionProfile: FoodAdmissionProfile.Islamicate
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodClearGrainSpirit() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_clear_grain_spirit",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Spirit,
			Name: "clear grain spirit",
			Description: "colourless grain spirit",
			LongDescription: "This colourless spirit catches the light without haze or sediment.",
			Taste: "It tastes fiery, dry, and faintly cereal-like.",
			Smell: "Hot alcohol carries a whisper of grain.",
			Colour: "white",
			AlcoholLitresPerLitre: 0.4,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodAgedGrainSpirit() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_aged_grain_spirit",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Spirit,
			Name: "aged grain spirit",
			Description: "amber aged grain spirit",
			LongDescription: "Time in wood gives this strong grain spirit a clear amber hue.",
			Taste: "It tastes warming, toasted, and faintly woody.",
			Smell: "Alcohol, oak, and grain fill the nose.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0.4,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodFruitSpirit() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_fruit_spirit",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Spirit,
			Name: "clear fruit spirit",
			Description: "colourless fruit spirit",
			LongDescription: "A clear strong spirit retains a delicate trace of distilled fruit.",
			Taste: "It tastes fiery, dry, and subtly fruity.",
			Smell: "Sharp alcohol carries ripe fruit fragrance.",
			Colour: "white",
			AlcoholLitresPerLitre: 0.4,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodGrapeSpirit() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_grape_spirit",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Spirit,
			Name: "grape spirit",
			Description: "pale grape-derived spirit",
			LongDescription: "This distilled grape spirit is pale gold and brilliantly clear.",
			Taste: "It tastes hot, winey, and softly sweet.",
			Smell: "Grape skins and alcohol define the aroma.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0.4,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodHoneySpirit() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_honey_spirit",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Spirit,
			Name: "honey spirit",
			Description: "clear honey-derived spirit",
			LongDescription: "A colourless spirit preserves a faint floral character from fermented honey.",
			Taste: "It tastes fiery, dry, and delicately floral.",
			Smell: "Hot alcohol carries beeswax and flowers.",
			Colour: "white",
			AlcoholLitresPerLitre: 0.4,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSpicedSpirit() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_spiced_spirit",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Spirit,
			Name: "spiced distilled spirit",
			Description: "amber aromatic spirit",
			LongDescription: "Steeped spices tint a powerful distilled spirit clear amber.",
			Taste: "It tastes hot, aromatic, and lingeringly warm.",
			Smell: "Alcohol and crushed spice dominate the smell.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0.5,
			WaterLitresPerLitre: 0,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodRoastedBarleyInfusion() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_roasted_barley_infusion",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Tea,
			Name: "roasted barley infusion",
			Description: "clear brown grain infusion",
			LongDescription: "Roasted barley steeps this clear caffeine-free infusion medium brown.",
			Taste: "It tastes nutty, toasted, and gently bitter.",
			Smell: "Roast grain and warm biscuit scent it.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 1,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 4,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodRoastedRiceInfusion() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_roasted_rice_infusion",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Tea,
			Name: "roasted rice infusion",
			Description: "pale gold rice infusion",
			LongDescription: "Toasted rice gives this clear infusion a pale golden colour.",
			Taste: "It tastes light, nutty, and delicately scorched.",
			Smell: "The aroma recalls rice and warm straw.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 1,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 4,
			AdmissionProfile: FoodAdmissionProfile.EastAsian
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodFreshLeafInfusion() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_fresh_leaf_infusion",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Tea,
			Name: "fresh leaf infusion",
			Description: "clear green leaf infusion",
			LongDescription: "Fresh edible leaves colour this clear hot infusion a light green.",
			Taste: "It tastes grassy, mild, and faintly bitter.",
			Smell: "Crushed green leaves shape its fragrance.",
			Colour: "green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 1,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 4,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodDriedLeafInfusion() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_dried_leaf_infusion",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Tea,
			Name: "dried leaf infusion",
			Description: "amber dried-leaf infusion",
			LongDescription: "Dried leaves release an amber colour and fine sediment while steeping.",
			Taste: "It tastes herbal, tannic, and softly dry.",
			Smell: "Dried foliage and warm hay scent it.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 1,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 4,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodFlowerInfusion() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_flower_infusion",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Tea,
			Name: "flower infusion",
			Description: "pale floral infusion",
			LongDescription: "Steeped petals lend this clear water a delicate blush of colour.",
			Taste: "It tastes floral, light, and gently sweet.",
			Smell: "Fresh blossoms provide a perfumed aroma.",
			Colour: "pink",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 1,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 4,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodRootSpiceInfusion() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_root_spice_infusion",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Tea,
			Name: "root and spice infusion",
			Description: "amber root-spice infusion",
			LongDescription: "Sliced roots and spice make this hot infusion deep amber.",
			Taste: "It tastes earthy, warming, and aromatic.",
			Smell: "Root, bark, and spice rise in steam.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 1,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 4,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodFruitPeelInfusion() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_fruit_peel_infusion",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Tea,
			Name: "fruit peel infusion",
			Description: "bright peel-scented infusion",
			LongDescription: "Fruit peel colours this clear infusion yellow with tiny oil droplets.",
			Taste: "It tastes tart, fragrant, and faintly bitter.",
			Smell: "Fresh rind gives a vivid citrus-like scent.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 1,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 4,
			AdmissionProfile: FoodAdmissionProfile.MaritimeTrade
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodHoneyedHerbInfusion() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_honeyed_herb_infusion",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Tea,
			Name: "honeyed herb infusion",
			Description: "golden sweet herb infusion",
			LongDescription: "Honey gives a green-flecked herbal infusion a golden glow.",
			Taste: "It tastes floral, leafy, and soothingly sweet.",
			Smell: "Honey and fresh herbs scent the steam.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.95,
			FoodSatiatedHoursPerLitre: 0.25,
			DrinkSatiatedHoursPerLitre: 3,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);
}
