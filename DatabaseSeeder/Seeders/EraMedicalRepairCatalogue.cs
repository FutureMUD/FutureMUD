#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

internal sealed record EraMedicalRepairCatalogueEntry(
	string StableReference, string Noun, string ShortDescription, string FullDescription,
	SizeCategory Size, ItemQuality Quality, double WeightInGrams, decimal Cost, string Material,
	string[] Tags, string[] Components, string BuilderNotes, string Category)
{
	internal ItemSeeder.EraCatalogueItemSpec ToItemSpec() => new(StableReference, Noun, ShortDescription, FullDescription,
		Size, Quality, WeightInGrams, Cost, Material, Tags, Components, BuilderNotes);
}

internal static class EraMedicalRepairCatalogue
{
	private const string Header = "stable_reference\tnoun\tshort_description\tfull_description\tsize\tquality\tweight_grams\tcost\tmaterial\ttags\tcomponents\tbuilder_notes\tcategory";
	private static readonly Lazy<IReadOnlyList<EraMedicalRepairCatalogueEntry>> EntriesLazy = new(ReadResources);
	internal static IReadOnlyList<EraMedicalRepairCatalogueEntry> Entries => EntriesLazy.Value;
	internal static IReadOnlyList<EraMedicalRepairCatalogueEntry> Renaissance => Entries.Where(x => x.StableReference.StartsWith("renaissance_", StringComparison.Ordinal)).ToArray();
	internal static IReadOnlyList<EraMedicalRepairCatalogueEntry> EarlyModern => Entries.Where(x => x.StableReference.StartsWith("earlymodern_", StringComparison.Ordinal)).ToArray();

	internal static IReadOnlyList<EraMedicalRepairCatalogueEntry> ReadResources() => ReadResources(typeof(EraMedicalRepairCatalogue).Assembly);
	internal static IReadOnlyList<EraMedicalRepairCatalogueEntry> ReadResources(Assembly assembly)
	{
		var entries = new List<EraMedicalRepairCatalogueEntry>();
		var resources = assembly.GetManifestResourceNames().Where(x => x.EndsWith(".medical-repair.tsv", StringComparison.OrdinalIgnoreCase)).OrderBy(x => x).ToArray();
		if (resources.Length != 2) throw new InvalidOperationException($"Expected two embedded medical-repair catalogues, found {resources.Length}.");
		foreach (var resource in resources)
		{
			using var stream = assembly.GetManifestResourceStream(resource) ?? throw new InvalidOperationException($"Cannot read {resource}.");
			using var reader = new StreamReader(stream);
			if (!string.Equals(reader.ReadLine()?.TrimStart('\uFEFF'), Header, StringComparison.Ordinal)) throw new InvalidDataException($"Unexpected header in {resource}.");
			var line = 1;
			while (reader.ReadLine() is { } value)
			{
				line++;
				if (string.IsNullOrWhiteSpace(value)) continue;
				var cells = value.Split('\t');
				if (cells.Length != 13) throw new InvalidDataException($"{resource} line {line} has {cells.Length} cells.");
				entries.Add(new EraMedicalRepairCatalogueEntry(cells[0], cells[1], cells[2], cells[3],
					Enum.Parse<SizeCategory>(cells[4], true), Enum.Parse<ItemQuality>(cells[5], true),
					double.Parse(cells[6], CultureInfo.InvariantCulture), decimal.Parse(cells[7], CultureInfo.InvariantCulture), cells[8],
					Split(cells[9]), Split(cells[10]), cells[11], cells[12]));
			}
		}
		Validate(entries);
		return entries;
	}

	private static string[] Split(string value) => value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
	private static void Validate(IReadOnlyCollection<EraMedicalRepairCatalogueEntry> entries)
	{
		if (entries.Count != 1098 || entries.Count(x => x.StableReference.StartsWith("renaissance_", StringComparison.Ordinal)) != 366 || entries.Count(x => x.StableReference.StartsWith("earlymodern_", StringComparison.Ordinal)) != 732)
			throw new InvalidDataException("Medical-repair catalogue counts are not 366 Renaissance and 732 Early Modern.");
		if (entries.Select(x => x.StableReference).Distinct(StringComparer.OrdinalIgnoreCase).Count() != entries.Count || entries.Select(x => x.ShortDescription).Distinct(StringComparer.OrdinalIgnoreCase).Count() != entries.Count)
			throw new InvalidDataException("Medical-repair catalogue has duplicate product references or short descriptions.");
		ValidateCategoryCounts(entries, "renaissance_", new Dictionary<string, int>
		{
			["Clinical surgery"] = 96, ["Apothecary"] = 70, ["Drugs delivery"] = 60,
			["Public health"] = 38, ["Mobility prosthesis"] = 32, ["Veterinary"] = 22,
			["Repair"] = 30, ["Raw medical stock"] = 18
		});
		ValidateCategoryCounts(entries, "earlymodern_", new Dictionary<string, int>
		{
			["Clinical surgery"] = 168, ["Apothecary pharmacy"] = 142, ["Drugs delivery"] = 154,
			["Public health"] = 82, ["Mobility prosthesis"] = 74, ["Veterinary"] = 46, ["Repair"] = 66
		});
		if (entries.Any(x => x.FullDescription.Count(c => c == '.') != 3)) throw new InvalidDataException("Medical-repair full descriptions must have three sentences.");
		if (entries.Any(x => x.Tags.Distinct(StringComparer.OrdinalIgnoreCase).Count() != x.Tags.Length ||
		                     x.Components.Distinct(StringComparer.OrdinalIgnoreCase).Count() != x.Components.Length))
			throw new InvalidDataException("Medical-repair rows cannot repeat tags or components.");
		if (entries.Any(x => x.StableReference.Contains("cupping_glass", StringComparison.Ordinal) && !string.Equals(x.Material, "glass", StringComparison.OrdinalIgnoreCase))) throw new InvalidDataException("Cupping glasses must be glass.");
		if (entries.Where(x => x.Category == "Repair").Any(x => x.Components.Count(y => y.StartsWith("Repair_", StringComparison.Ordinal)) != 1 || !x.Tags.Any(y => y.StartsWith("Functions / Repairing / ", StringComparison.Ordinal)))) throw new InvalidDataException("Repair rows must carry one repair component and repair target tag.");
		if (RenaissanceDrugViolations(entries)) throw new InvalidDataException("Renaissance catalogue contains Early Modern delivery components.");
	}

	private static void ValidateCategoryCounts(IEnumerable<EraMedicalRepairCatalogueEntry> entries, string prefix,
		IReadOnlyDictionary<string, int> expected)
	{
		var actual = entries.Where(x => x.StableReference.StartsWith(prefix, StringComparison.Ordinal))
			.GroupBy(x => x.Category).ToDictionary(x => x.Key, x => x.Count(), StringComparer.Ordinal);
		if (actual.Count != expected.Count || expected.Any(x => !actual.TryGetValue(x.Key, out var count) || count != x.Value))
			throw new InvalidDataException($"Medical-repair category allocation is invalid for {prefix.TrimEnd('_')}.");
	}

	private static bool RenaissanceDrugViolations(IEnumerable<EraMedicalRepairCatalogueEntry> entries) => entries.Where(x => x.StableReference.StartsWith("renaissance_drug_", StringComparison.Ordinal)).SelectMany(x => x.Components).Any(x => x.Contains("Jesuit_Bark", StringComparison.Ordinal) || x.Contains("Ipecacuanha", StringComparison.Ordinal) || x.Contains("Dover_s", StringComparison.Ordinal) || x.Contains("Paregor", StringComparison.Ordinal) || x.Contains("Tartar_Emetic", StringComparison.Ordinal) || x.Contains("Calomel", StringComparison.Ordinal) || x.Contains("Epsom", StringComparison.Ordinal) || x.Contains("Daffy", StringComparison.Ordinal) || x.Contains("Godfrey", StringComparison.Ordinal));
}
