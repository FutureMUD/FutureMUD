#nullable enable

using System;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>New-seed whole-body sweat rates in litres per real minute, before exertion scaling.
/// See Design Documents/Seeding/Seeded_Sweat_Rates.md for evidence and game adaptations.</summary>
internal static class SeededSweatRates
{
	internal const double Human = 0.02;
	internal const double Horse = 0.20;
	internal const double MiddleEarthElf = 0.01;
	internal const double Hobbit = 0.01;
	internal const double Dwarf = 0.015;
	internal const double Orc = 0.02;
	internal const double Troll = 0.06;

	/// <summary>Save an added race, retaining an explicit zero despite the legacy SQL default of 0.8.
	/// Never use this as an update/repair path for an existing race.</summary>
	internal static void SaveNewRace(FuturemudDatabaseContext context, Race race)
	{
		if (context.Entry(race).State != EntityState.Added)
		{
			throw new InvalidOperationException("Sweat initialisation requires a newly added race.");
		}

		var rate = race.SweatRateInLitresPerMinute;
		context.SaveChanges();
		if (rate == 0.0)
		{
			// Zero is EF's sentinel for the generated column. Explicitly UPDATE only this new row.
			var property = context.Entry(race).Property(x => x.SweatRateInLitresPerMinute);
			property.CurrentValue = 0.0;
			property.IsModified = true;
			context.SaveChanges();
		}
	}

	// These are species defaults, not a mass-based scaling of Human.
	// Zero omits negligible/local secretion from this whole-body needs/wetting mechanism.
	internal static double Animal(string name) => name switch
	{
		"Horse" => Horse,
		"Donkey" => 0.08,
		"Mule" => 0.14,
		"Cow" or "Ox" => 0.02,
		"Bison" => 0.015,
		"Buffalo" => 0.005,
		"Sheep" or "Goat" => 0.003,
		"Deer" or "Reindeer" => 0.005,
		"Moose" or "Elk" => 0.01,
		"Llama" => 0.005,
		"Alpaca" => 0.003,
		"Camel" => 0.025,
		"Giraffe" => 0.015,
		"Macaque" or "Capybara" => 0.001,
		"Kangaroo" => 0.005,
		"Wallaby" => 0.002,
		"Rhinocerous" => 0.01,
		"Tapir" => 0.005,
		_ => 0.0
	};

	// Explicit fictional anatomy adaptations; sapience/weapon use alone does not imply sweating.
	internal static double Mythical(string name) => name switch
	{
		"Unicorn" or "Pegasus" or "Pegacorn" or "Centaur" => Horse,
		"Hippogriff" => 0.10,
		"Qilin" => 0.02,
		"Minotaur" or "Selkie" => Human,
		"Naga" => 0.01,
		_ => 0.0
	};
}
