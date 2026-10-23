#nullable enable

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	internal static bool ShouldSeedRenaissanceFinishedItemCraftsForTesting(string? eras) =>
		!string.IsNullOrWhiteSpace(eras) && HasAnyEra(eras, "renaissance");

	internal static bool ShouldSeedRenaissanceEarlyModernJewelleryDoorCraftsForTesting(string? eras) =>
		!string.IsNullOrWhiteSpace(eras) && HasAnyEra(eras, "renaissance", "earlymodern");
}
