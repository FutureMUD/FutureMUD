#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	internal static IEnumerable<string> SerpentRaceNames => RaceTemplates.Values
		.Where(x => x.BodyKey == "Serpentine").Select(x => x.Name);

	internal static void EnsureStockAirBreathing(FuturemudDatabaseContext context)
	{
		var air = context.Gases.FirstOrDefault(x => x.Name == "Breathable Atmosphere");
		if (air is null) return;
		var names = RaceTemplates.Values.Where(x => x.BreathingMode is
			AnimalBreathingMode.Simple or AnimalBreathingMode.Insect or AnimalBreathingMode.Blowhole)
			.Select(x => x.Name).ToArray();
		foreach (var race in context.Races.Where(x => names.Contains(x.Name)).ToList())
		{
			if (race.BreathingModel is not ("simple" or "partless" or "blowhole")) continue;
			if (context.RacesBreathableGases.Any(x => x.RaceId == race.Id && x.GasId == air.Id)) continue;
			// Repair the old arbitrary-first-gas default without deleting authored gas tolerances.
			context.RacesBreathableGases.Add(new RacesBreathableGases
			{
				Race = race, RaceId = race.Id, Gas = air, GasId = air.Id, Multiplier = 1.0
			});
		}
		context.SaveChanges();
	}
}
