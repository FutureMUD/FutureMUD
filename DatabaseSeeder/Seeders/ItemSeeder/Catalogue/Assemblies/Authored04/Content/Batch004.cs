#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public static partial class Authored04Stock
{
	public static CatalogueItemDefinition EarlymodernScienceInstrumentStand() => new()
	{
		Metadata = new("earlymodern_science_instrument_stand", "earlymodern", ["earlymodern"], [], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.EraCatalogueSupport.cs:93", []),
		Noun = "stand",
		ShortDescription = "a brass instrument stand",
		LongDescription = null,
		FullDescription = "A three-legged brass stand supports a portable instrument at a stable working height in an observatory or survey room.",
		Size = SizeCategory.Large,
		Quality = ItemQuality.Standard,
		WeightInGrams = 9800,
		Cost = 74.0m,
		Skinnable = false,
		HiddenFromPlayers = false,
		Material = "brass",
		Tags = ["Era / Early Modern Era", "Market / Household Goods / Standard Furniture"],
		Components = ["Destroyable_Furniture"],
		MorphToStableReference = null,
		MorphEmote = null,
		MorphSeconds = null,
		DestroyedItemStableReference = null,
		BuilderNotes = "Early Modern instrument support; mounting mechanics are not implied.",
		AllowLegacyShortDescriptionMatch = false,
	};



































































































}
