#nullable enable
using System;
using System.Linq;
using MudSharp.Database;
using MudSharp.Models;
namespace DatabaseSeeder.Seeders;
public sealed partial class AgricultureSeeder
{
	internal static bool HasFarmingFoundation(FuturemudDatabaseContext context) =>
		SettingSeederIntegration.Profile?.HasFarmingFoundation(context) ?? context.TraitDefinitions.Any(x => x.Name == FarmingTraitName);
	private static TraitDefinition? ResolveFarmingTrait(FuturemudDatabaseContext context) =>
		SettingSeederIntegration.Profile?.ResolveFarmingTrait(context) ?? context.TraitDefinitions.FirstOrDefault(x => x.Name == FarmingTraitName);
	internal static TraitDefinition EnsureFarmingTrait(FuturemudDatabaseContext context) =>
		ResolveFarmingTrait(context) ?? (SettingSeederIntegration.IsInstalled(context)
			? SettingSeederIntegration.Require(context).EnsureFarmingTrait(context)
			: throw new InvalidOperationException("Agriculture requires an existing Farming skill."));
}
