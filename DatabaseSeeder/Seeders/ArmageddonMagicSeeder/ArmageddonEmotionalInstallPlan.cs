#nullable enable
using System.Collections.Generic;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public sealed record ArmageddonEmotionalTerrains(long Air, long City, long Inside, long Hills,
	long Mountain, long Thornlands, long Earth);

/// <summary>Null attribute permits unambiguous native seeder inference; all other mappings are explicit.</summary>
public sealed record ArmageddonEmotionalBindings(long? FuryAttribute, double UnitsPerSourcePoint,
	double FuryIntensity, long FuryEligibilityProg, long CalmSaveTrait, double CalmIntensity,
	long CalmEligibilityProg, IReadOnlyList<Difficulty> CalmSaves, ArmageddonEmotionalTerrains Terrains);

public sealed record ArmageddonEmotionalInstallPlan(bool Install, long School, long Resource,
	long AlwaysFalseProg, long FurySkill, long CalmSkill, ArmageddonEmotionalBindings Bindings);
