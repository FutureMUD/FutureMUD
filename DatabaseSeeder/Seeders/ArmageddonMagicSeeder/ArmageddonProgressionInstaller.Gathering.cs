#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonProgressionInstaller
{
	private const string SourceGatheringMarker = "$sourceGatheringPolicy";
	private const string SourceGatheringVersion = "source-gathering-1";

	private static bool HasSourceGatheringPolicy(SeederManagedRecord record)
	{
		if (record.SeedBaseline is null) return false;
		var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline)
			?? throw new InvalidOperationException($"{record.StableKey}: invalid progression baseline.");
		if (!baseline.TryGetValue(SourceGatheringMarker, out var version)) return false;
		if (version != JsonSerializer.Serialize(SourceGatheringVersion))
			throw new InvalidOperationException($"{record.StableKey}: incompatible source gathering policy version.");
		return true;
	}
}
