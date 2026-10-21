#nullable enable
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

/// <summary>Explicit native mappings. Null on the prepared-world bindings preserves an existing module.</summary>
public sealed record ArmageddonWaterSeeBindings(IReadOnlyList<long> WaterLiquids, long SiltTerrain,
	long ShadowTerrain, IReadOnlyList<long> DivinationRankTags);

/// <summary>The composer supplies the already-owned source skills; this module creates no skills or players.</summary>
public sealed record ArmageddonWaterSeeInstallPlan(bool Install, long School, long Resource, long AlwaysFalseProg,
	long WaterBreathingSkill, long SeeTheUnbodiedSkill, ArmageddonWaterSeeBindings Bindings);
