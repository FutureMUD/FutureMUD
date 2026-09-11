#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders.CultureToolkit;

/// <summary>Authored neighbour contacts; endpoints must already exist in the selected toolkit.</summary>
internal static class CultureToolkitForeignAccents
{
	internal static void Upsert(FuturemudDatabaseContext context, CultureToolkitCatalogue catalogue, string era,
		IReadOnlyDictionary<string, Language> languages, ICollection<string> conflicts)
	{
		var languageIds = languages.Values.Select(x => x.Id).Distinct().ToArray();
		context.Accents.Where(x => languageIds.Contains(x.LanguageId)).Include(x => x.AssociatedLanguages).Load();
		var writer = new CultureToolkitEntityWriter(context, era, conflicts);
		foreach (var row in catalogue.Document("data.historical_foreign_accents.json").EnumerateArray()
			.Where(x => CultureToolkitCatalogue.Strings(x.GetProperty("packs")).Contains(era)))
		{
			var key = CultureToolkitCatalogue.Text(row, "key");
			if (!languages.TryGetValue(CultureToolkitCatalogue.Text(row, "target"), out var target) ||
				!languages.TryGetValue(CultureToolkitCatalogue.Text(row, "source"), out var source))
			{
				conflicts.Add($"{key}: neighbour accent deferred because a language endpoint is not installed.");
				continue;
			}
			if (CultureToolkitManagedEntities.Find(context, "Accent", key) is null &&
				target.Accents.Any(x => x.Role == 1 && x.AssociatedLanguages.Any(y => y.Id == source.Id))) continue;
			var accent = writer.Upsert(key, new Accent
			{
				LanguageId = target.Id, Name = $"{source.Name}-speaking", Group = "foreign", Role = 1,
				Suffix = $"with a {source.Name}-speaking accent", VagueSuffix = "with a foreign accent",
				Description = $"The pronunciation of {target.Name} by a native speaker of {source.Name}, encountered through neighbouring communities, trade or migration.",
				Difficulty = (int)Difficulty.Normal
			}, independentlyManagedFields: new HashSet<string> { nameof(Accent.ChargenAvailabilityProgId) });
			var associationKey = key + ".associated-languages";
			var fresh = CultureToolkitManagedEntities.Find(context, "AccentAssociations", associationKey) is null;
			var merged = CultureToolkitManagedEntities.Reconcile(context, era, "AccentAssociations", associationKey,
				accent.Id, fresh,
				new Dictionary<string, string> { ["languages"] = JsonSerializer.Serialize(accent.AssociatedLanguages.Select(x => x.Id).Order().ToArray()) },
				new Dictionary<string, string> { ["languages"] = JsonSerializer.Serialize(new[] { source.Id }) }, conflicts);
			accent.AssociatedLanguages.Clear();
			foreach (var id in JsonSerializer.Deserialize<long[]>(merged["languages"])!)
				accent.AssociatedLanguages.Add(context.Languages.Find(id)!);
		}
		context.SaveChanges();
	}
}
