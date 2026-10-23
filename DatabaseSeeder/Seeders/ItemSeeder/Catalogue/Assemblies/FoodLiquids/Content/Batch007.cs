#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class FoodLiquidsStock
{
	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodAppleVinegar() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_apple_vinegar",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Vinegar,
			Name: "apple vinegar",
			Description: "cloudy gold apple vinegar",
			LongDescription: "Apple sediment leaves this golden vinegar faintly cloudy.",
			Taste: "It tastes fruity, tart, and sharply acidic.",
			Smell: "Apple peel rides above the vinegar scent.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPearVinegar() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_pear_vinegar",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Vinegar,
			Name: "pear vinegar",
			Description: "pale floral pear vinegar",
			LongDescription: "This pale vinegar preserves a delicate perfume from ripe pears.",
			Taste: "It tastes floral, sour, and softly fruity.",
			Smell: "Pear fragrance tempers the sharp acid smell.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPlumVinegar() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_plum_vinegar",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Vinegar,
			Name: "plum vinegar",
			Description: "clear ruby plum vinegar",
			LongDescription: "Plum skins give this clear fruit vinegar a ruby tint.",
			Taste: "It tastes winey, sour, and faintly sweet.",
			Smell: "Dark fruit mingles with bright acidity.",
			Colour: "red",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodBerryVinegar() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_berry_vinegar",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Vinegar,
			Name: "berry vinegar",
			Description: "crimson berry vinegar",
			LongDescription: "A vivid crimson vinegar carries fine sediment from crushed berries.",
			Taste: "It tastes sharply fruity and mouth-puckeringly sour.",
			Smell: "Berry perfume rises through strong acid.",
			Colour: "red",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodDateVinegar() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_date_vinegar",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Vinegar,
			Name: "date vinegar",
			Description: "dark amber date vinegar",
			LongDescription: "Dates lend this clear vinegar a dark amber, syrup-like colour.",
			Taste: "It tastes caramel-fruity, sour, and rich.",
			Smell: "Date sweetness underlies the acid aroma.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.Islamicate
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodHoneyVinegar() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_honey_vinegar",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Vinegar,
			Name: "honey vinegar",
			Description: "clear golden honey vinegar",
			LongDescription: "Fermented honey creates a clear gold vinegar with floral traces.",
			Taste: "It tastes brightly sour with a delicate sweetness.",
			Smell: "Flowers and acid share the fragrance.",
			Colour: "yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodHerbedVinegar() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_herbed_vinegar",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Vinegar,
			Name: "herbed vinegar",
			Description: "green-flecked aromatic vinegar",
			LongDescription: "Steeped herbs tint clear vinegar green and leave small leaf fragments.",
			Taste: "It tastes sharp, leafy, and strongly aromatic.",
			Smell: "Crushed herbs soften the piercing acid smell.",
			Colour: "green",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.5,
			FoodSatiatedHoursPerLitre: 0,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodPlainHoneySyrup() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_plain_honey_syrup",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Syrup,
			Name: "honey dilution syrup",
			Description: "clear amber honey syrup",
			LongDescription: "Honey thinned for pouring forms a clear amber, slow-moving syrup.",
			Taste: "It tastes floral, rich, and purely sweet.",
			Smell: "Warm honey and beeswax scent it.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSpicedHoneySyrup() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_spiced_honey_syrup",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Syrup,
			Name: "spiced honey syrup",
			Description: "aromatic amber honey syrup",
			LongDescription: "Ground spice darkens flowing honey syrup and leaves fine flecks.",
			Taste: "It tastes warming, floral, and intensely sweet.",
			Smell: "Honey and spice fill the aroma.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodGrapeReductionSyrup() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_grape_reduction_syrup",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Syrup,
			Name: "grape reduction syrup",
			Description: "dark purple grape syrup",
			LongDescription: "Reduced grape must forms a thick, glossy purple-black syrup.",
			Taste: "It tastes winey, fruity, and richly sweet.",
			Smell: "Cooked grape and caramel scent it.",
			Colour: "magenta",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodDateReductionSyrup() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_date_reduction_syrup",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Syrup,
			Name: "date reduction syrup",
			Description: "dark brown date syrup",
			LongDescription: "Cooked dates yield a dark brown syrup with a heavy pour.",
			Taste: "It tastes caramel-like, earthy, and very sweet.",
			Smell: "Ripe date and dark sugar shape the smell.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.Islamicate
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodFigReductionSyrup() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_fig_reduction_syrup",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Syrup,
			Name: "fig reduction syrup",
			Description: "amber-brown fig syrup",
			LongDescription: "Fig pulp reduces into a seed-flecked amber-brown syrup.",
			Taste: "It tastes jammy, earthy, and honey-sweet.",
			Smell: "Cooked fig gives a warm fruity aroma.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodAppleReductionSyrup() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_apple_reduction_syrup",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Syrup,
			Name: "apple reduction syrup",
			Description: "clear russet apple syrup",
			LongDescription: "Concentrated apple juice forms a clear russet pouring syrup.",
			Taste: "It tastes tart, caramelised, and richly fruity.",
			Smell: "Cooked apple and browned sugar scent it.",
			Colour: "bold yellow",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.RegionalOldWorld
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodBerryReductionSyrup() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_berry_reduction_syrup",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Syrup,
			Name: "berry reduction syrup",
			Description: "deep crimson berry syrup",
			LongDescription: "Reduced mixed berry juice makes a glossy deep crimson syrup.",
			Taste: "It tastes tart, vivid, and intensely sweet.",
			Smell: "Cooked berries give a powerful fruit smell.",
			Colour: "red",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 5,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.Universal
		);

	public static PreIndustrialFoodLiquidCatalogueEntry PreindustrialFoodSugarCookingSyrup() =>
		new PreIndustrialFoodLiquidCatalogueEntry(
			StableReference: "preindustrial_food_sugar_cooking_syrup",
			Scope: FoodCatalogueScope.Shared,
			Family: FoodCatalogueFamily.Syrup,
			Name: "refined sugar cooking syrup",
			Description: "clear heavy sugar syrup",
			LongDescription: "Refined sugar dissolved for confectionery makes a clear, heavy syrup.",
			Taste: "It tastes cleanly and overwhelmingly sweet.",
			Smell: "Hot sugar gives a faint caramel smell.",
			Colour: "white",
			AlcoholLitresPerLitre: 0,
			WaterLitresPerLitre: 0.1,
			FoodSatiatedHoursPerLitre: 6,
			DrinkSatiatedHoursPerLitre: 0,
			AdmissionProfile: FoodAdmissionProfile.SugarTrade
		);
}
