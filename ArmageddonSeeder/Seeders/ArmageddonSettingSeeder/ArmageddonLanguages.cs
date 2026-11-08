#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed record ArmageddonLanguageProfile(string Slug, string Name, string Description, string UnknownSpeech);

/// <summary>The ten languages listed in the 8 March 2008 language archive, independent of class grants.</summary>
internal static class ArmageddonLanguages
{
	internal const string Module = "setting-language-skills";
	internal static IReadOnlyList<ArmageddonLanguageProfile> Profiles { get; } = Array.AsReadOnly(new ArmageddonLanguageProfile[]
	{
		new("allundean", "Allundean", "Elven tribal speech is rich in vowels which its written form omits.", "a flowing, vowel-rich tongue"),
		new("anyar", "Anyar", "The northern Anyali tribes speak this soft, slurred tongue with whistles, pops and clicks.", "a soft tongue punctuated by whistles and clicks"),
		new("bendune", "Bendune", "This old nomadic human tongue favours concrete terms and is closely related to Cavilish.", "a vowel-rich, nomadic tongue"),
		new("cavilish", "Cavilish", "The Merchant's Tongue blends nomadic roots with Sirihish structures among trading houses.", "a vowel-rich tongue with measured phrases"),
		new("heshrak", "Heshrak", "The Gith language uses regular sounds in both speech and writing, but remains little studied.", "an unfamiliar, carefully articulated tongue"),
		new("kentu", "Kentu", "Halfling speech has many distinct sounds and cultural metaphors, with no known written form.", "a tongue of varied, unfamiliar sounds"),
		new("mirukkim", "Mirukkim", "Dwarven writing uses accented consonant symbols and closely follows spoken syllables.", "a strongly accented, syllabic tongue"),
		new("nrizkt", "Nrizkt", "Mantis speech uses clicks, whines and pops which other peoples struggle to reproduce.", "a series of clicks, whines and pops"),
		new("sirihish", "Sirihish", "The city-states' common human tongue descends from Tatlum and has a broad, abstract vocabulary.", "a varied tongue with complex phrases"),
		new("tatlum", "Tatlum", "This ancient ancestor of Sirihish is now chiefly spoken by Allanak's and Tuluk's templars.", "an unfamiliar tongue resembling formal Sirihish")
	});

	private static SeederManagedRecord? Claim(FuturemudDatabaseContext db, string key, Type type)
	{
		var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		if (record is null) return null;
		if (record.Retired || record.EntityType != type.Name || record.LogicalId is not { } id || record.SeedBaseline is null ||
			(record.Module != Module && record.Module != ArmageddonSkillPackage.Module &&
			 !(key.StartsWith("arm.skill.tatlum", StringComparison.Ordinal) && record.Module == ArmageddonTemplarInstaller.Module)) ||
			db.Find(type, id) is null || db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == id))
			throw new InvalidOperationException($"Invalid or competing language skill ownership: {key}.");
		return record;
	}

	internal static void Preflight(FuturemudDatabaseContext db)
	{
		foreach (var profile in Profiles)
		{
			var key = "arm.skill." + profile.Slug;
			var trait = Claim(db, key, typeof(TraitDefinition));
			var cap = Claim(db, key + ".cap", typeof(TraitExpression));
			if ((trait is null) != (cap is null)) throw new InvalidOperationException($"Incomplete language skill/cap ownership: {profile.Name}.");
			ArmageddonSettingOwnership.RequireNameAvailable(db, new TraitDefinition { Name = profile.Name }, trait);
			if (cap is null) ArmageddonSettingOwnership.RequireNameAvailable(db, new TraitExpression { Name = key + ".cap" }, null);
			if (trait is not null)
			{
				var row = db.TraitDefinitions.Single(x => x.Id == trait.LogicalId);
				if (row.Type != 0 || row.OwnerScope != (int)MudSharp.Body.Traits.TraitOwnerScope.Character || row.ExpressionId != cap!.LogicalId)
					throw new InvalidOperationException($"Invalid character language skill graph: {profile.Name}.");
			}
		}
	}

	private static void Update(FuturemudDatabaseContext db, SeederManagedRecord record, object row,
		ICollection<string> messages, params (string Field, object Value)[] changes)
	{
		var desired = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
		foreach (var (field, value) in changes) desired[field] = JsonSerializer.Serialize(value);
		ArmageddonSettingOwnership.Reconcile(db, record, row, desired, messages);
	}

	internal static Dictionary<string, TraitDefinition> InstallSkills(FuturemudDatabaseContext db,
		IReadOnlyDictionary<string, TraitDefinition> canonical, ICollection<string> messages)
	{
		var skills = canonical.ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);
		var template = skills["arm.skill.sirihish"];
		foreach (var profile in Profiles)
		{
			var key = "arm.skill." + profile.Slug;
			var record = Claim(db, key, typeof(TraitDefinition));
			TraitDefinition trait;
			if (record is null)
			{
				var cap = ArmageddonSettingOwnership.Apply(db, Module, key + ".cap",
					new TraitExpression { Name = key + ".cap", Expression = "200" }, messages);
				trait = (TraitDefinition)db.Entry(template).CurrentValues.Clone().ToObject();
				trait.Id = 0; trait.Name = profile.Name; trait.ExpressionId = cap.Id; trait.ChargenBlurb = profile.Description;
				trait = ArmageddonSettingOwnership.Apply(db, Module, key, trait, messages);
			}
			else
			{
				trait = db.TraitDefinitions.Include(x => x.Expression).Single(x => x.Id == record.LogicalId);
				Update(db, record, trait, messages, ("Name", profile.Name), ("ChargenBlurb", profile.Description),
					("TraitGroup", "Language"), ("DecoratorId", template.DecoratorId), ("ImproverId", template.ImproverId));
				Update(db, Claim(db, key + ".cap", typeof(TraitExpression))!, trait.Expression, messages, ("Expression", "200"));
			}
			skills[key] = trait;
		}
		db.SaveChanges();
		return skills;
	}

	internal static void AddInstalledSkills(FuturemudDatabaseContext db, IDictionary<string, TraitDefinition> skills)
	{
		foreach (var profile in Profiles)
		{
			var key = "arm.skill." + profile.Slug;
			if (skills.ContainsKey(key)) continue;
			if (Claim(db, key, typeof(TraitDefinition)) is { } record)
				skills.Add(key, db.TraitDefinitions.Include(x => x.Expression).Single(x => x.Id == record.LogicalId));
		}
	}
}
