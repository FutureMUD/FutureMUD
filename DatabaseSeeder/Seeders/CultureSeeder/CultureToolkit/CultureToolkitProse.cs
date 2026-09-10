#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.Framework;

namespace DatabaseSeeder.Seeders.CultureToolkit;

/// <summary>Reviewed presentation changes are separate from byte-preserved source and historical evidence.</summary>
public static class CultureToolkitProse
{
	private sealed record Correction(string Module, string Entity, string Identity, string Field, string Original, string Replacement);
	private static readonly IReadOnlyDictionary<(string Module, string Entity, string Identity, string Field), Correction> Corrections = Load();

	public static string Rewrite(string module, string entity, string identity, string field, string original)
	{
		if (!Corrections.TryGetValue((module, entity, identity, field), out var correction)) return original;
		if (original != correction.Original && !(entity == "NameCulture" && field == "Definition" &&
			XNode.DeepEquals(XElement.Parse(original), XElement.Parse(correction.Original))))
			throw new InvalidOperationException($"Reviewed prose source changed: {module}:{entity}:{identity}:{field}; review its binding before applying the correction.");
		return correction.Replacement;
	}

	public static string Display(string? text) => text?.ConvertToLatin1() ?? string.Empty;

	private static IReadOnlyDictionary<(string, string, string, string), Correction> Load()
	{
		using var stream = typeof(CultureToolkitProse).Assembly.GetManifestResourceStream("CultureReviewedProse")
			?? throw new InvalidOperationException("Missing reviewed source prose ledger.");
		return JsonSerializer.Deserialize<Correction[]>(stream)!.ToDictionary(x => (x.Module, x.Entity, x.Identity, x.Field));
	}
}
