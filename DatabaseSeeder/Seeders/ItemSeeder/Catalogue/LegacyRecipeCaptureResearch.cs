#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

internal static class LegacyRecipeCaptureResearch
{
	internal static readonly IReadOnlySet<string> Keys = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
	{
		"Scrollmaking/assemble simple papyrus scroll",
		"Bookbinding/sew bronze parchment scraper",
		"Residual Stock/prepare bronze incense burner",
		"Residual Stock/prepare bronze thurible",
		"Residual Stock/prepare bronze incense urn",
		"Residual Stock/prepare chain-hung censer",
		"Residual Stock/prepare bedchamber ember pot",
		"Residual Stock/prepare brass warming pan",
		"Residual Stock/prepare bronze table brazier",
		"Residual Stock/prepare cast-iron room brazier",
		"Residual Stock/prepare charcoal hand warmer",
		"Residual Stock/prepare clay charcoal brazier",
		"Residual Stock/prepare courtyard fire basin",
		"Residual Stock/prepare cast-iron brazier",
		"Residual Stock/prepare hall floor brazier",
		"Residual Stock/prepare iron ember pan",
		"Residual Stock/prepare charcoal scent brazier",
		"Residual Stock/prepare low bronze brazier",
		"Residual Stock/prepare winter iron brazier",
		"Residual Stock/prepare small charcoal brazier",
		"Residual Stock/prepare brass charcoal brazier",
		"Residual Stock/prepare standing fire basket",
		"Residual Stock/prepare portable camp brazier",
		"Residual Stock/prepare willow fire basket",
		"Residual Stock/prepare terracotta fire bowl",
		"Residual Stock/prepare chamber warming brazier",
		"Residual Stock/prepare workshop coal brazier",
		"Residual Stock/prepare wrought-iron portable brazier",
		"Residual Stock/prepare brass incense burner",
		"Residual Stock/prepare bundle of rawhide cord",
		"Residual Stock/prepare brass incense burner pattern 2",
		"Residual Stock/prepare iron cresset basket",
		"Residual Stock/prepare bronze incense burner pattern 2",
		"Residual Stock/prepare hanging bronze censer",
		"Residual Stock/prepare standing iron censer",
		"Residual Stock/prepare wooden mason's square",
	};
	internal static IEnumerable<CatalogueRecipeDefinition> Definitions()
	{
		yield return Recipes02Stock.ScrollmakingAssembleSimplePapyrusScroll();
		yield return Recipes02Stock.BookbindingSewBronzeParchmentScraper();
		yield return Recipes06Stock.ResidualStockPrepareBronzeIncenseBurner();
		yield return Recipes06Stock.ResidualStockPrepareBronzeThurible();
		yield return Recipes06Stock.ResidualStockPrepareBronzeIncenseUrn();
		yield return Recipes07Stock.ResidualStockPrepareChainHungCenser();
		yield return Recipes07Stock.ResidualStockPrepareBedchamberEmberPot();
		yield return Recipes07Stock.ResidualStockPrepareBrassWarmingPan();
		yield return Recipes07Stock.ResidualStockPrepareBronzeTableBrazier();
		yield return Recipes07Stock.ResidualStockPrepareCastIronRoomBrazier();
		yield return Recipes07Stock.ResidualStockPrepareCharcoalHandWarmer();
		yield return Recipes08Stock.ResidualStockPrepareClayCharcoalBrazier();
		yield return Recipes08Stock.ResidualStockPrepareCourtyardFireBasin();
		yield return Recipes08Stock.ResidualStockPrepareCastIronBrazier();
		yield return Recipes08Stock.ResidualStockPrepareHallFloorBrazier();
		yield return Recipes08Stock.ResidualStockPrepareIronEmberPan();
		yield return Recipes08Stock.ResidualStockPrepareCharcoalScentBrazier();
		yield return Recipes08Stock.ResidualStockPrepareLowBronzeBrazier();
		yield return Recipes08Stock.ResidualStockPrepareWinterIronBrazier();
		yield return Recipes08Stock.ResidualStockPrepareSmallCharcoalBrazier();
		yield return Recipes08Stock.ResidualStockPrepareBrassCharcoalBrazier();
		yield return Recipes08Stock.ResidualStockPrepareStandingFireBasket();
		yield return Recipes08Stock.ResidualStockPreparePortableCampBrazier();
		yield return Recipes08Stock.ResidualStockPrepareWillowFireBasket();
		yield return Recipes08Stock.ResidualStockPrepareTerracottaFireBowl();
		yield return Recipes08Stock.ResidualStockPrepareChamberWarmingBrazier();
		yield return Recipes08Stock.ResidualStockPrepareWorkshopCoalBrazier();
		yield return Recipes08Stock.ResidualStockPrepareWroughtIronPortableBrazier();
		yield return Recipes08Stock.ResidualStockPrepareBrassIncenseBurner();
		yield return Recipes08Stock.ResidualStockPrepareBundleOfRawhideCord();
		yield return Recipes08Stock.ResidualStockPrepareBrassIncenseBurnerPattern2();
		yield return Recipes08Stock.ResidualStockPrepareIronCressetBasket();
		yield return Recipes08Stock.ResidualStockPrepareBronzeIncenseBurnerPattern2();
		yield return Recipes08Stock.ResidualStockPrepareHangingBronzeCenser();
		yield return Recipes08Stock.ResidualStockPrepareStandingIronCenser();
		yield return Recipes09Stock.ResidualStockPrepareWoodenMasonSSquare();
	}
}
