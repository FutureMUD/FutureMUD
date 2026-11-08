#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonHeritage
{
	private static readonly (string Key, string Definition, string Name, string Basic, string Fancy)[] AppearanceValues =
	[
		("elven.ears", "Ears", "tapering and pointed", "pointed", "long ears tapering to distinct points"),
		("elven.frame", "Frame", "slender and long-limbed", "slender", "a slender frame with long limbs"),
		("dwarf.frame", "Frame", "broad and squat", "stocky", "a broad, compact and stocky frame"),
		("half_giant.frame", "Frame", "massive and broad", "massive", "a massive frame with broad proportions"),
		("mul.frame", "Frame", "powerfully muscled", "muscular", "a solid frame with pronounced musculature")
	];

	private static readonly (string Key, string Definition)[] AppearanceProfileIdentities =
	[
		("elven.ears", "Ears"), ("elven.frame", "Frame"), ("dwarf.frame", "Frame"),
		("half_giant.frame", "Frame"), ("mul.frame", "Frame"),
		("hairless.hair", "Hair Style"), ("hairless.facialhair", "Facial Hair Style")
	];

	private static void PreflightAppearance(FuturemudDatabaseContext db)
	{
		foreach (var name in AppearanceProfileIdentities.Select(x => x.Definition).Distinct())
			if (!db.CharacteristicDefinitions.Any(x => x.Name == name))
				throw new InvalidOperationException($"Armageddon appearance needs the shared Human {name} characteristic definition.");
		Preflight<CharacteristicValue>(db, AppearanceValues.Select(x => ($"arm.appearance.{x.Key}.value", x.Name)));
		Preflight<CharacteristicProfile>(db, AppearanceProfileIdentities.Select(x => ($"arm.appearance.{x.Key}.profile", "Armageddon " + x.Key + " Appearance")));
		foreach (var (definition, value) in new[] { ("Hair Style", "bald"), ("Facial Hair Style", "clean shave") })
			if (!db.CharacteristicValues.Any(x => x.Definition.Name == definition && x.Name == value))
				throw new InvalidOperationException($"Armageddon appearance needs the native {definition}/{value} choice.");
	}

	private static IReadOnlyDictionary<long, long> AppearanceProfiles(FuturemudDatabaseContext db, string profile, ICollection<string> messages)
	{
		var result = new Dictionary<long, long>();
		var selected = profile switch
		{
			"city_elf" or "desert_elf" => new[] { "elven.ears", "elven.frame" },
			"dwarf" => ["dwarf.frame", "hairless.hair", "hairless.facialhair"],
			"half_giant" => ["half_giant.frame"],
			"mul" => ["mul.frame", "hairless.hair", "hairless.facialhair"],
			_ => Array.Empty<string>()
		};
		foreach (var key in selected)
		{
			var definitionName = AppearanceProfileIdentities.Single(x => x.Key == key).Definition;
			var definition = db.CharacteristicDefinitions.Single(x => x.Name == definitionName);
			CharacteristicValue value;
			var restricted = key.StartsWith("hairless.", StringComparison.Ordinal);
			if (restricted)
				value = db.CharacteristicValues.Single(x => x.DefinitionId == definition.Id && x.Name == (key == "hairless.hair" ? "bald" : "clean shave"));
			else
			{
				var entry = AppearanceValues.Single(x => x.Key == key);
				value = Apply(db, $"arm.appearance.{key}.value", new CharacteristicValue
				{
					DefinitionId = definition.Id, Name = entry.Name, Value = entry.Basic, AdditionalValue = entry.Fancy,
					Default = false, FutureProgId = null, OngoingValidityProgId = null
				}, messages);
			}
			var xml = new XElement("Definition", restricted ? new XAttribute("RestrictStyles", true) : null, new XElement("Value", value.Id));
			var choice = Apply(db, $"arm.appearance.{key}.profile", new CharacteristicProfile
			{
				Name = "Armageddon " + key + " Appearance", TargetDefinitionId = definition.Id, Type = "Standard", Definition = xml.ToString(),
				Description = "Conservative approved racial appearance. Shared colour palettes remain available; no immunity or other gameplay advantage is implied."
			}, messages);
			result[definition.Id] = choice.Id;
		}
		return result;
	}
}
