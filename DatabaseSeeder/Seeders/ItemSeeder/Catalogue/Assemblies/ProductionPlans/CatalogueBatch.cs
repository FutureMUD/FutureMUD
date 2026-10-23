#nullable enable

using System.Collections.Generic;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class ProductionPlanStock
{
	public static IEnumerable<CatalogueProductionPlan> MedievalProduction()
	{
		foreach (var row in MedievalProduction001()) yield return row;
	}

	public static IEnumerable<CatalogueProductionPlan> MedievalFood()
	{
		foreach (var row in MedievalFood001()) yield return row;
		foreach (var row in MedievalFood002()) yield return row;
	}

	public static IEnumerable<CataloguePrimaryProductionContract> PrimaryProduction()
	{
		foreach (var row in PrimaryProduction001()) yield return row;
		foreach (var row in PrimaryProduction002()) yield return row;
	}

	public static IEnumerable<CataloguePrimaryProductionContract> BlackPowder()
	{
		foreach (var row in BlackPowder001()) yield return row;
	}

	public static IEnumerable<CatalogueToolProvider> RenaissanceToolProviders()
	{
		foreach (var row in RenaissanceToolProviders001()) yield return row;
	}

	public static IEnumerable<CatalogueFoodProductionPlan> PreIndustrialFoodCatalogue()
	{
		foreach (var row in PreIndustrialFoodCatalogue001()) yield return row;
		foreach (var row in PreIndustrialFoodCatalogue002()) yield return row;
		foreach (var row in PreIndustrialFoodCatalogue003()) yield return row;
		foreach (var row in PreIndustrialFoodCatalogue004()) yield return row;
		foreach (var row in PreIndustrialFoodCatalogue005()) yield return row;
		foreach (var row in PreIndustrialFoodCatalogue006()) yield return row;
		foreach (var row in PreIndustrialFoodCatalogue007()) yield return row;
		foreach (var row in PreIndustrialFoodCatalogue008()) yield return row;
		foreach (var row in PreIndustrialFoodCatalogue009()) yield return row;
		foreach (var row in PreIndustrialFoodCatalogue010()) yield return row;
		foreach (var row in PreIndustrialFoodCatalogue011()) yield return row;
		foreach (var row in PreIndustrialFoodCatalogue012()) yield return row;
		foreach (var row in PreIndustrialFoodCatalogue013()) yield return row;
		foreach (var row in PreIndustrialFoodCatalogue014()) yield return row;
	}
}
