#if DEBUG
#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder;

internal static class SeederReplayRunner
{
	internal static ProductReplayRunner Runner { get; } = new([typeof(SkillSeeder)], context =>
		context.Accounts.Any() || context.SeederChoices.Any()
			? "Replay profiles require a freshly migrated, unseeded database. This database already has bootstrap or seeder-answer data."
			: null);

	internal static SeederReplayValidationResult Validate(SeederReplayProfile profile, IEnumerable<IDatabaseSeeder> seeders) =>
		Runner.Validate(profile, seeders);

	internal static SeederReplayRunResult Run(SeederReplayProfile profile, IEnumerable<IDatabaseSeeder> seeders,
		Func<FuturemudDatabaseContext> contextFactory, Version version, Action<string>? progress = null) =>
		Runner.Run(profile, seeders, contextFactory, version, progress);

	internal static SeederReplayRunResult RunPrefixBefore(SeederReplayProfile profile, IEnumerable<IDatabaseSeeder> seeders,
		Type stopBeforeSeederType, Func<FuturemudDatabaseContext> contextFactory, Version version, Action<string>? progress = null) =>
		Runner.RunPrefixBefore(profile, seeders, stopBeforeSeederType, contextFactory, version, progress);
}

#endif
