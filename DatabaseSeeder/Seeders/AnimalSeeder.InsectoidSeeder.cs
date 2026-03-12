#nullable enable

using System;
using System.Linq;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	private void SeedInsectoid(BodyProto insectProto)
	{
		SeedInsectoidBody(insectProto, false);
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");
		SeedAnimalRaces(GetInsectRaceTemplates().Where(x => x.BodyKey == "Insectoid"),
			("Insectoid", insectProto));
	}

	private void SeedWingedInsectoid(BodyProto insectProto)
	{
		SeedInsectoidBody(insectProto, true);
		Console.WriteLine($"...[{_stopwatch.Elapsed.TotalSeconds:N1}s] Races...");
		SeedAnimalRaces(GetInsectRaceTemplates().Where(x => x.BodyKey == "Winged Insectoid"),
			("Winged Insectoid", insectProto));
	}
}
