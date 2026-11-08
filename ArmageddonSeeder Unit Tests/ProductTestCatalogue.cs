#nullable enable
using System.Collections.Generic;
using DatabaseSeeder;
namespace MudSharp_Unit_Tests;
internal static class SeederCatalogue
{
	internal static IReadOnlyList<IDatabaseSeeder> GetEnabledSeeders() => ArmageddonSeederProgram.GetEnabledSeeders();
	internal static SeederDependencyPlan GetDependencyPlan(IEnumerable<IDatabaseSeeder> seeders) => SeederPackagePlan.GetDependencyPlan(seeders);
}
#if DEBUG
internal static class SeederReplayRunner
{
	internal static SeederReplayValidationResult Validate(SeederReplayProfile profile, IEnumerable<IDatabaseSeeder> seeders) => ArmageddonSeederReplayProfiles.Runner.Validate(profile, seeders);
	internal static SeederReplayRunResult Run(SeederReplayProfile profile, IEnumerable<IDatabaseSeeder> seeders,
		System.Func<MudSharp.Database.FuturemudDatabaseContext> contexts, System.Version version) => ArmageddonSeederReplayProfiles.Runner.Run(profile, seeders, contexts, version);
}
#endif
