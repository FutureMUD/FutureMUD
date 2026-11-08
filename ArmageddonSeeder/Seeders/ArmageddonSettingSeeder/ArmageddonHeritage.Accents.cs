#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonHeritage
{
	private sealed record RacialAccent(string Slug, string Name, string Suffix, string Description, string[] Profiles);

	private static readonly RacialAccent[] SirihishRacialAccents =
	[
		new("elvish", "Elvish", "with an elvish accent", "Sirihish spoken with the influence of Allundean and an elven upbringing.", ["city_elf", "desert_elf"]),
		new("dwarvish", "Dwarvish", "with a dwarvish accent", "Sirihish spoken with the influence of Mirukkim and a dwarven upbringing.", ["dwarf"]),
		new("half_elven", "Half-Elven", "with a half-elven accent", "A Sirihish accent blending human and elven speech traditions.", ["half_elf"]),
		new("half_giant", "Half-Giant", "with a half-giant accent", "Sirihish as spoken among half-giants raised in the cities.", ["half_giant"]),
		new("mul", "Mul", "with a mul accent", "Sirihish as spoken by muls, reflecting the speech of their upbringing.", ["mul"])
	];

	private static string RacialAccentKey(RacialAccent accent) => "arm.accent.sirihish." + accent.Slug;
	private static string RacialAccentProgName(RacialAccent accent) => "ArmageddonSirihishAccent" + accent.Slug.Replace("_", "");

	private static void InstallSirihishRacialAccents(FuturemudDatabaseContext db, Language language,
		IReadOnlyDictionary<string, Race> races, ICollection<string> messages)
	{
		foreach (var accent in SirihishRacialAccents)
		{
			var key = RacialAccentKey(accent);
			var condition = string.Join(" or ", accent.Profiles.Select(x => $"@ch.Race == ToRace({races[x].Id})"));
			var available = Prog(db, key + ".availability", RacialAccentProgName(accent), $"return ({condition})",
				ProgVariableTypes.Boolean, messages, (ProgVariableTypes.Chargen, "ch"));
			Apply(db, key, new Accent
			{
				LanguageId = language.Id, Name = accent.Name, Suffix = accent.Suffix, VagueSuffix = "with an unfamiliar accent",
				Group = "racial", Role = 0, Difficulty = 5, Description = accent.Description, ChargenAvailabilityProgId = available.Id
			}, messages);
		}
	}
}
