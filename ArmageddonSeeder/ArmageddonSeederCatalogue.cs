#nullable enable
using System;
using System.Collections.Generic;
namespace DatabaseSeeder;
internal static class SeederCatalogue
{
	internal static IReadOnlyList<IDatabaseSeeder> GetEnabledSeeders() => ArmageddonSeederProgram.GetEnabledSeeders();
	internal static SeederDependencyPlan GetDependencyPlan(IEnumerable<IDatabaseSeeder> seeders) => SeederPackagePlan.GetDependencyPlan(seeders);
}
#if DEBUG
internal static class SeederReplayRunner
{
	internal static SeederReplayValidationResult Validate(SeederReplayProfile profile, IEnumerable<IDatabaseSeeder> seeders) => ArmageddonSeederReplayProfiles.Runner.Validate(profile, seeders);
	internal static SeederReplayRunResult Run(SeederReplayProfile profile, IEnumerable<IDatabaseSeeder> seeders, Func<MudSharp.Database.FuturemudDatabaseContext> contexts, Version version, Action<string>? progress = null) => ArmageddonSeederReplayProfiles.Runner.Run(profile, seeders, contexts, version, progress);
	internal static SeederReplayRunResult RunPrefixBefore(SeederReplayProfile profile, IEnumerable<IDatabaseSeeder> seeders, Type stop, Func<MudSharp.Database.FuturemudDatabaseContext> contexts, Version version, Action<string>? progress = null) => ArmageddonSeederReplayProfiles.Runner.RunPrefixBefore(profile, seeders, stop, contexts, version, progress);
}
#endif
