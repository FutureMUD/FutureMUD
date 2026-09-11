#nullable enable

using System;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public static class CultureToolkitSocialCultures
{
	public static IReadOnlyDictionary<string, Culture> Upsert(FuturemudDatabaseContext context, CultureToolkitPack pack,
		IReadOnlyDictionary<string, NameCulture> fallbackStructures, Calendar calendar, FutureProg originalStartingValue,
		FutureProg availability, ICollection<string> conflicts,
		CultureToolkitCatalogue? catalogue = null, IReadOnlyDictionary<string, Language>? languages = null)
	{
		CultureToolkitProgSeeder.Validate(originalStartingValue, MudSharp.FutureProg.ProgVariableTypes.Number,
			MudSharp.FutureProg.ProgVariableTypes.Toon, MudSharp.FutureProg.ProgVariableTypes.Trait, MudSharp.FutureProg.ProgVariableTypes.Number);
		foreach (var row in pack.Cultures)
		{
			var key = CultureToolkitCatalogue.Text(row, "key");
			var label = CultureToolkitCatalogue.Text(row, "label");
			if (!fallbackStructures.ContainsKey(CultureToolkitCatalogue.Text(row, "naming_fallback")))
				throw new InvalidOperationException($"Unresolved fallback naming structure for {key}.");
			if (CultureToolkitManagedEntities.Find(context, "Culture", key) is null && context.Cultures.Any(x => x.Name == label))
				throw new InvalidOperationException($"Social culture {key} has an unowned label collision: {label}.");
		}
		var writer = new CultureToolkitEntityWriter(context, pack.Era, conflicts);
		var result = new Dictionary<string, Culture>();
		foreach (var row in pack.Cultures)
		{
			var key = CultureToolkitCatalogue.Text(row, "key");
			var culture = writer.Upsert(key, new Culture
			{
				NativeLanguageId = catalogue is not null && languages is not null
					? NativeReferences(catalogue, row, pack.Era)
						.Where(languages.ContainsKey).Select(x => (long?)languages[x].Id).FirstOrDefault() : null,
				Name = CultureToolkitCatalogue.Text(row, "label"), Description = CultureToolkitCatalogue.Text(row, "description"),
				PersonWordMale = "man", PersonWordFemale = "woman", PersonWordNeuter = "person", PersonWordIndeterminate = "person",
				PrimaryCalendarId = calendar.Id, SkillStartingValueProgId = originalStartingValue.Id, AvailabilityProgId = availability.Id
			}, independentlyManagedFields: languages is not null ? new HashSet<string> { nameof(Culture.SkillStartingValueProgId) } :
				new HashSet<string> { nameof(Culture.SkillStartingValueProgId), nameof(Culture.NativeLanguageId) });
			result[key] = culture;
			var fallback = fallbackStructures[CultureToolkitCatalogue.Text(row, "naming_fallback")];
			context.Entry(culture).Collection(x => x.CulturesNameCultures).Load();
			foreach (var gender in Enum.GetValues<Gender>())
			{
				var link = culture.CulturesNameCultures.SingleOrDefault(x => x.Gender == (short)gender);
				var linkKey = key + $".naming.{(short)gender}";
				var previous = CultureToolkitManagedEntities.Find(context, "CultureNameCulture", linkKey);
				var merged = CultureToolkitManagedEntities.Reconcile(context, pack.Era, "CultureNameCulture", linkKey, culture.Id,
					link is null && previous is null, link is null ? new Dictionary<string, string>() : new Dictionary<string, string> { ["culture"] = link.NameCultureId.ToString(CultureInfo.InvariantCulture) },
					new Dictionary<string, string> { ["culture"] = fallback.Id.ToString(CultureInfo.InvariantCulture) }, conflicts);
				if (!merged.TryGetValue("culture", out var value)) continue;
				var id = long.Parse(value, CultureInfo.InvariantCulture);
				if (link?.NameCultureId == id) continue;
				if (link is not null) { culture.CulturesNameCultures.Remove(link); context.CulturesNameCultures.Remove(link); }
				culture.CulturesNameCultures.Add(new CulturesNameCultures { CultureId = culture.Id, NameCultureId = id, Gender = (short)gender });
			}
			context.SaveChanges();
		}
		return result;
	}

	internal static IReadOnlyList<string> NativeReferences(CultureToolkitCatalogue catalogue, System.Text.Json.JsonElement row, string era)
		=> row.TryGetProperty("native_default_by_era", out var defaults) && defaults.TryGetProperty(era, out var reference)
			? [reference.GetString()!]
			: catalogue.ResolveSelector(CultureToolkitCatalogue.Text(row, "vernacular_selector"), era, []);
}
