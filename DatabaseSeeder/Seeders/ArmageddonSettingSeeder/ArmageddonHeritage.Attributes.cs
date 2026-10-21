#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonHeritage
{
	private sealed record AttributeBonuses(int Strength, int Endurance, int Agility, int Wisdom);
	private sealed record AttributeLink(long AttributeId, bool Health, double Bonus, string? Dice);
	private sealed record AttributeBinding(long AttributeId, string Semantic, bool Health);

	private static AttributeBonuses BonusesFor(string profile) => profile switch
	{
		"human" => new(0, 0, 0, 0),
		"city_elf" => new(-2, -2, 3, 1),
		"desert_elf" => new(-2, -1, 3, 1),
		"dwarf" => new(2, 3, -2, -3),
		"half_elf" => new(-1, -1, 1, 1),
		"half_giant" => new(10, 5, -4, -5),
		"mul" => new(4, 4, -1, 0),
		_ => throw new ArgumentOutOfRangeException(nameof(profile), profile, "Unknown heritage profile.")
	};

	// As with the non-human attribute adapter, combined physical traits use a rounded average.
	// Wisdom is preferred; Intelligence is only a fallback when that raw trait does not exist.
	internal static string AttributeSemantic(string name, bool hasWisdom) => name.Trim().ToLowerInvariant() switch
	{
		"strength" or "upper body strength" or "lower body strength" => "strength",
		"constitution" or "endurance" or "hardiness" or "resilience" or "stamina" => "endurance",
		"agility" or "dexterity" or "speed" => "agility",
		"wisdom" => "wisdom",
		"intelligence" or "mind" when !hasWisdom => "wisdom",
		"body" or "physique" => "physical",
		_ => "neutral"
	};

	internal static int AttributeBonusFor(string profile, string semantic)
	{
		var bonus = BonusesFor(profile);
		return semantic switch
		{
			"strength" => bonus.Strength,
			"endurance" => bonus.Endurance,
			"agility" => bonus.Agility,
			"wisdom" => bonus.Wisdom,
			"physical" => (int)Math.Round((bonus.Strength + bonus.Endurance) / 2.0, MidpointRounding.AwayFromZero),
			_ => 0
		};
	}

	private static TraitDefinition[] RawAttributes(FuturemudDatabaseContext db)
	{
		var attributes = db.TraitDefinitions.Where(x => x.Type == 1 && x.OwnerScope == 0).OrderBy(x => x.Id).ToArray();
		if (attributes.Length == 0)
			throw new InvalidOperationException("Armageddon heritage requires installed body attributes; run Time/Attributes first. The Armageddon attribute selection is recommended, not required.");
		return attributes;
	}

	private static void AttributeDefaults(Race race, int rawAttributeCount)
	{
		race.DiceExpression = "3d6+1";
		race.IndividualAttributeCap = 20;
		race.AttributeTotalCap = rawAttributeCount * 12;
	}

	private static void AttributeLinks(FuturemudDatabaseContext db, string key, string profile, Race race,
		Race human, TraitDefinition[] attributes, ICollection<string> messages)
	{
		var bindingKey = key + ".attributes";
		var previous = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == bindingKey);
		var bindings = previous?.SeedBaseline is { } baseline
			? JsonSerializer.Deserialize<AttributeBinding[]>(JsonSerializer.Deserialize<Dictionary<string, string>>(baseline)!["Bindings"])!
			: null;
		if (bindings is null)
		{
			var hasWisdom = attributes.Any(x => x.Name.Equals("Wisdom", StringComparison.OrdinalIgnoreCase));
			var inherited = new Dictionary<long, bool>();
			Race? ancestor = human;
			var visited = new HashSet<long>();
			while (ancestor is not null && visited.Add(ancestor.Id))
			{
				foreach (var link in db.RacesAttributes.Where(x => x.RaceId == ancestor.Id))
					inherited.TryAdd(link.AttributeId, link.IsHealthAttribute);
				ancestor = ancestor.ParentRaceId is { } parentId ? db.Races.Find(parentId) : null;
			}
			bindings = attributes.Select(x => new AttributeBinding(x.Id, AttributeSemantic(x.Name, hasWisdom),
				inherited.GetValueOrDefault(x.Id, string.Equals(x.TraitGroup, "Physical", StringComparison.OrdinalIgnoreCase)))).ToArray();
			foreach (var attribute in attributes.Where(x => AttributeSemantic(x.Name, hasWisdom) == "neutral"))
				messages.Add($"Armageddon {profile}: {attribute.Name} retains a neutral racial adjustment in this attribute selection.");
		}
		// Retain semantic-to-ID bindings, so a builder rename does not change racial meaning.
		var storedBindings = Json(bindings);
		var rawIds = attributes.Select(x => x.Id).ToHashSet();
		if (!rawIds.SetEquals(bindings.Select(x => x.AttributeId)))
			throw new InvalidOperationException($"{key}: the originally bound raw body attribute inventory has changed. Preserve and resolve the attribute foundation before rerunning.");
		var desired = bindings.Select(x => new AttributeLink(x.AttributeId, x.Health, AttributeBonusFor(profile, x.Semantic), "3d6+1"))
			.OrderBy(x => x.AttributeId).ToArray();
		var rows = db.RacesAttributes.Where(x => x.RaceId == race.Id).ToArray();
		var current = rows.Select(x => new AttributeLink(x.AttributeId, x.IsHealthAttribute, x.AttributeBonus, x.DiceExpression))
			.OrderBy(x => x.AttributeId).ToArray();
		var merged = ArmageddonSettingLinks.Reconcile(db, bindingKey, nameof(Race), race.Id,
			new Dictionary<string, string> { ["Bindings"] = storedBindings, ["Attributes"] = Json(current) },
			new Dictionary<string, string> { ["Bindings"] = storedBindings, ["Attributes"] = Json(desired) }, messages);
		var next = JsonSerializer.Deserialize<AttributeLink[]>(merged["Attributes"])!;
		if (previous is null && current.Length > 0)
		{
			// Before this collection had stock ownership, every existing local row was a builder choice.
			// Keep those rows while adding absent defaults. The baseline remains the stock manifest,
			// so subsequent three-way reruns continue to recognise the adopted customisations.
			var customIds = current.Select(x => x.AttributeId).ToHashSet();
			next = current.Concat(desired.Where(x => !customIds.Contains(x.AttributeId))).OrderBy(x => x.AttributeId).ToArray();
			messages.Add($"{key}: preserved {current.Length} pre-existing builder attribute links while adopting stock ownership.");
		}
		db.RacesAttributes.RemoveRange(rows.Where(x => next.All(y => y.AttributeId != x.AttributeId)));
		foreach (var value in next)
		{
			var row = rows.SingleOrDefault(x => x.AttributeId == value.AttributeId);
			if (row is null)
			{
				row = new RacesAttributes { RaceId = race.Id, AttributeId = value.AttributeId };
				db.RacesAttributes.Add(row);
			}
			row.IsHealthAttribute = value.Health;
			row.AttributeBonus = value.Bonus;
			row.DiceExpression = value.Dice;
		}
		db.SaveChanges();
	}
}
