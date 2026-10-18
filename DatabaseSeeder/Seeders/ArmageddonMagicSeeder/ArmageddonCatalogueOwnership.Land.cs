#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonCatalogueOwnership
{
	private sealed record LifeYield(double? Maximum, double? Hourly);
	private void LoadLandChildren(object row)
	{
		if (row is not ForagableProfile profile) return;
		_db.Entry(profile).Collection(x => x.ForagableProfilesMaximumYields).Load();
		_db.Entry(profile).Collection(x => x.ForagableProfilesHourlyYieldGains).Load();
	}
	private static void LandFields(object row, Dictionary<string, string> fields)
	{
		if (row is not ForagableProfile profile) return;
		fields["LifeYield"] = JsonSerializer.Serialize(new LifeYield(
			profile.ForagableProfilesMaximumYields.SingleOrDefault(x => x.ForageType == ArmageddonRegionalLandInstaller.LifeYieldKey)?.Yield,
			profile.ForagableProfilesHourlyYieldGains.SingleOrDefault(x => x.ForageType == ArmageddonRegionalLandInstaller.LifeYieldKey)?.Yield));
	}
	private void ApplyLandChildren(object row, IReadOnlyDictionary<string, string> current, IReadOnlyDictionary<string, string> merged)
	{
		if (row is not ForagableProfile profile || current["LifeYield"] == merged["LifeYield"]) return;
		var next = JsonSerializer.Deserialize<LifeYield>(merged["LifeYield"])!;
		var maximum = profile.ForagableProfilesMaximumYields.SingleOrDefault(x => x.ForageType == ArmageddonRegionalLandInstaller.LifeYieldKey);
		var hourly = profile.ForagableProfilesHourlyYieldGains.SingleOrDefault(x => x.ForageType == ArmageddonRegionalLandInstaller.LifeYieldKey);
		if (next.Maximum.HasValue)
		{
			if (maximum is null) profile.ForagableProfilesMaximumYields.Add(new() { ForageType = ArmageddonRegionalLandInstaller.LifeYieldKey, Yield = next.Maximum.Value });
			else maximum.Yield = next.Maximum.Value;
		}
		else if (maximum is not null) { _db.Remove(maximum); profile.ForagableProfilesMaximumYields.Remove(maximum); }
		if (next.Hourly.HasValue)
		{
			if (hourly is null) profile.ForagableProfilesHourlyYieldGains.Add(new() { ForageType = ArmageddonRegionalLandInstaller.LifeYieldKey, Yield = next.Hourly.Value });
			else hourly.Yield = next.Hourly.Value;
		}
		else if (hourly is not null) { _db.Remove(hourly); profile.ForagableProfilesHourlyYieldGains.Remove(hourly); }
		// Other native yield keys and forage item outcomes belong to the builder and are never removed here.
	}
}
