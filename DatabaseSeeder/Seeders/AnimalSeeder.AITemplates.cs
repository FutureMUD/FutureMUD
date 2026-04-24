#nullable enable

using MudSharp.Database;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	internal static IReadOnlyDictionary<string, string> AnimalAIRecommendationsForTesting =>
		AnimalAIStockTemplates.AnimalRecommendationsForTesting;

	internal static IReadOnlyCollection<string> StockAnimalAITemplateNamesForTesting =>
		AnimalAIStockTemplates.AnimalTemplateNamesForTesting;

	internal static IReadOnlyList<AnimalAIStockTemplateDefinition> StockAnimalAITemplatesForTesting =>
		AnimalAIStockTemplates.TemplatesForTesting;

	internal static ShouldSeedResult ClassifyAnimalAIStockTemplatePresenceForTesting(FuturemudDatabaseContext context)
	{
		return ClassifyAnimalAIStockTemplatePresence(context);
	}

	internal static void SeedAnimalAIStockTemplatesForTesting(FuturemudDatabaseContext context)
	{
		AnimalAIStockTemplates.SeedAnimalTemplates(context);
	}

	private static ShouldSeedResult ClassifyAnimalAIStockTemplatePresence(FuturemudDatabaseContext context)
	{
		return AnimalAIStockTemplates.ClassifyPresence(
			context,
			AnimalAIStockTemplates.AnimalTemplateNamesForTesting);
	}

	private static bool HasMissingAnimalAIStockTemplates(FuturemudDatabaseContext context)
	{
		return ClassifyAnimalAIStockTemplatePresence(context) != ShouldSeedResult.MayAlreadyBeInstalled;
	}

	private void SeedAnimalAIStockTemplates()
	{
		AnimalAIStockTemplates.SeedAnimalTemplates(_context);
	}
}
