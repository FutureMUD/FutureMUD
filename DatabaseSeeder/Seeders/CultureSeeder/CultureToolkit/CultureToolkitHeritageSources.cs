#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders.CultureToolkit;

public sealed record CultureHeritageSource(string Key, string Module, Ethnicity Template, JsonElement? Overlay,
	string? NamingStructureOverride, string BindingReason, IReadOnlyList<string> Aliases);

/// <summary>Supplied overlays bind historical source identities. New broad phenotype reuse is explicitly recorded.</summary>
public static class CultureToolkitHeritageSources
{
	public static IReadOnlyList<string> Modules(string era) => era switch
	{
		"antiquity" => ["earthantiquity"],
		"darkages" => ["earthdarkagesandmedieval"],
		"medieval" => ["earthdarkagesandmedieval", "earthrenaissanceeurope"],
		"renaissance" or "earlymodern" => ["earthrenaissanceeurope", "earthrenaissanceworldexpansion"],
		_ => throw new ArgumentException($"Unknown toolkit era {era}.", nameof(era))
	};

	public static IReadOnlyList<CultureHeritageSource> Describe(CultureToolkitPack pack,
		IReadOnlyDictionary<string, FuturemudDatabaseContext> stages, Ethnicity genericHumanDefaults)
	{
		var all = stages.SelectMany(stage => stage.Value.Ethnicities.Include(x => x.EthnicitiesCharacteristics)
			.Include(x => x.EthnicitiesNameCultures).ThenInclude(x => x.NameCulture).AsEnumerable()
			.Select(ethnicity => (Module: stage.Key, Ethnicity: ethnicity))).ToArray();
		var activeModules = Modules(pack.Era);
		var result = new List<CultureHeritageSource>();
		var aliases = new HashSet<(string Module, long Id)>();
		foreach (var overlay in pack.Ethnicities)
		{
			var key = CultureToolkitCatalogue.Text(overlay, "key");
			var label = CultureToolkitCatalogue.Text(overlay, "label");
			var names = CultureToolkitCatalogue.Strings(overlay.GetProperty("legacy_aliases")).Append(label).ToHashSet(StringComparer.Ordinal);
			var matches = all.Where(x => names.Contains(x.Ethnicity.Name))
				.Where(x => x.Module != "earthantiquity" || key is not ("ethnicity.greek-roman" or "ethnicity.venetian"))
				.OrderByDescending(x => x.Ethnicity.Name == label)
				.ThenByDescending(x => activeModules.Contains(x.Module)).ThenBy(x => x.Module, StringComparer.Ordinal).ToArray();
			if (matches.Length > 0)
			{
				var chosen = matches[0];
				foreach (var match in matches) aliases.Add((match.Module, match.Ethnicity.Id));
				result.Add(new(key, chosen.Module, chosen.Ethnicity, overlay, null, "Exact supplied overlay label/alias; source fields retained as baseline.",
					matches.Select(x => $"source.{x.Module}.ethnicity.{x.Ethnicity.Name}").ToArray()));
				continue;
			}
			var fallback = key switch
			{
				"ethnicity.lowland-scot" => ("earthdarkagesandmedieval", "High Medieval Scottish Gael", "Scottish"),
				"ethnicity.georgian" => ("earthrenaissanceworldexpansion", "Late Medieval Georgian", "Renaissance Georgian"),
				"ethnicity.continental-saxon" => ("earthdarkagesandmedieval", "Carolingian Frank", "Medieval Carolingian Frankish"),
				"ethnicity.finnish" => ("human-prerequisite", genericHumanDefaults.Name, "Simple"),
				_ => throw new InvalidOperationException($"{key}: no supplied/verified source template binding; no unrelated ethnicity was substituted.")
			};
			var template = fallback.Item1 == "human-prerequisite" ? genericHumanDefaults :
				all.Single(x => x.Module == fallback.Item1 && x.Ethnicity.Name == fallback.Item2).Ethnicity;
			result.Add(new(key, fallback.Item1, template, overlay, fallback.Item3,
				key == "ethnicity.finnish"
					? "New supplied identity uses installed generic Human characteristic defaults; approved Finnish repertoire supplies names when requested. No Finno-Ugric pool is used to fill a gap."
					: "New supplied overlay reuses existing broad characteristic defaults and the explicitly inspected local naming structure; it does not inherit the template's language or peoplehood.", []));
		}
		foreach (var source in all.Where(x => activeModules.Contains(x.Module) && !aliases.Contains((x.Module, x.Ethnicity.Id))))
		{
			var key = $"source.{source.Module}.ethnicity.{source.Ethnicity.Name}";
			result.Add(new(key, source.Module, source.Ethnicity, null, null,
				"Retained source identity; active toolkit use requires an explicit native binding and geographic/prose review.", [key]));
		}
		return result;
	}
}
