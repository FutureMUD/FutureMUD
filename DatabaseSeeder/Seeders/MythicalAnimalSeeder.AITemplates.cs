#nullable enable

using MudSharp.Database;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class MythicalAnimalSeeder
{
	internal static IReadOnlyDictionary<string, string> MythicalAnimalAIRecommendationsForTesting =>
		AnimalAIStockTemplates.MythicalRecommendationsForTesting;

	internal static IReadOnlyCollection<string> StockMythicalAnimalAITemplateNamesForTesting =>
		AnimalAIStockTemplates.MythicalTemplateNamesForTesting;

	internal static ShouldSeedResult ClassifyMythicalAnimalAIStockTemplatePresenceForTesting(
		FuturemudDatabaseContext context)
	{
		return ClassifyMythicalAnimalAIStockTemplatePresence(context);
	}

	internal static void SeedMythicalAnimalAIStockTemplatesForTesting(FuturemudDatabaseContext context)
	{
		AnimalAIStockTemplates.SeedMythicalTemplates(context);
	}

	private static ShouldSeedResult ClassifyMythicalAnimalAIStockTemplatePresence(FuturemudDatabaseContext context)
	{
		return AnimalAIStockTemplates.ClassifyPresence(
			context,
			AnimalAIStockTemplates.MythicalTemplateNamesForTesting);
	}

	private static bool HasMissingMythicalAnimalAIStockTemplates(FuturemudDatabaseContext context)
	{
		return ClassifyMythicalAnimalAIStockTemplatePresence(context) != ShouldSeedResult.MayAlreadyBeInstalled;
	}

	private void SeedMythicalAnimalAIStockTemplates()
	{
		AnimalAIStockTemplates.SeedMythicalTemplates(_context);
	}
}
