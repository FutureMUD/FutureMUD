#nullable enable

using DatabaseSeeder.Seeders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DatabaseSeeder;

/// <summary>Developer-only source audit. It never writes an item, craft, outfit or evidence source.</summary>
internal static class IndustrialisedCatalogueAudit
{
	internal const string RelativePath = "Design Documents/Seeding/Industrialised_Item_Catalogue_Audit.tsv";
	internal const string Header = "StableReference\tLayer\tDomain\tSourceFile\tSourceLine\tEraAdmissions\tMaterial\tComponents\tProfileBindings\tPriceEvidence\tCraftable\tLifecycle\tValidation\tResolvedNeutralComponents\tCraftReferences\tOutfitReferences\tLifecycleTarget\tCatalogueSourceSha256";

	internal static string Generate(string catalogueDirectory)
	{
		var document = IndustrialisedItemCatalogue.LoadDirectory(catalogueDirectory);
		var sourceHash = SourceFingerprint(catalogueDirectory);
		return Generate(document, sourceHash);
	}

	internal static string Generate(IndustrialisedItemCatalogueDocument document, string sourceHash)
	{
		var bindings = document.TechnologyBindings.Where(x => x.Profile == "neutral")
			.ToDictionary(x => $"{x.Dimension}:{x.Family}", StringComparer.Ordinal);
		var craftReferences = document.Crafts.Select(x => (Item: x.ProductStableReference, Craft: x.StableKey))
			.Concat(document.Clothing.CraftProducts.Where(x => x.Kind == ClothingProductKind.Item && !x.FailureProduct)
				.Select(x => (Item: x.Reference, Craft: x.CraftReference))).ToLookup(x => x.Item, x => x.Craft, StringComparer.Ordinal);
		var outfitReferences = document.Outfits.SelectMany(outfit => outfit.ItemStableReferences.Select(item =>
			(Item: item, Outfit: outfit.OutfitReference)))
			.Concat(document.Clothing.OutfitEntries.Select(x => (Item: x.ItemReference, Outfit: x.OutfitReference)))
			.ToLookup(x => x.Item, x => x.Outfit, StringComparer.Ordinal);
		var output = new StringBuilder(Header).Append('\n');
		foreach (var row in document.Items.OrderBy(x => x.Source, StringComparer.Ordinal).ThenBy(x => x.Line))
		{
			var neutralComponents = row.FixedComponents.ToList();
			foreach (var key in row.ProfileBindings)
			{
				if (!bindings.TryGetValue(key, out var binding))
				{
					throw new InvalidDataException($"{row.Source}:{row.Line}: unresolved neutral profile binding {key}.");
				}

				if (binding.ComponentBacked)
				{
					neutralComponents.AddRange(binding.Values);
				}
			}

			output.AppendJoin('\t', new[]
			{
				row.StableReference, row.Layer, row.Domain, row.Source, row.Line.ToString(CultureInfo.InvariantCulture),
				string.Join(';', row.EraAdmissions), row.Material, string.Join(';', row.FixedComponents),
				string.Join(';', row.ProfileBindings), string.Join(';', row.PriceEvidence),
				row.Craftable || craftReferences[row.StableReference].Any() ? "true" : "false", row.LifecycleKind ?? string.Empty,
				"parsed;production-unreviewed",
				Sorted(neutralComponents), Sorted(craftReferences[row.StableReference]),
				Sorted(outfitReferences[row.StableReference]), row.DestroyedItem ?? row.MorphTo ?? string.Empty,
				sourceHash
			}).Append('\n');
		}

		return output.ToString();
	}

	internal static string SourceFingerprint(string directory)
	{
		using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
		foreach (var path in Directory.EnumerateFiles(directory, "*.tsv", SearchOption.AllDirectories)
			.OrderBy(x => Path.GetRelativePath(directory, x).Replace('\\', '/'), StringComparer.Ordinal))
		{
			hash.AppendData(Encoding.UTF8.GetBytes(Path.GetRelativePath(directory, path).Replace('\\', '/')));
			hash.AppendData([0]);
			hash.AppendData(Encoding.UTF8.GetBytes(NormalizeLines(File.ReadAllText(path))));
			hash.AppendData([0]);
		}

		return Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
	}

	internal static void RefreshOrCheck(string repositoryRoot, bool check)
	{
		var directory = Path.Combine(repositoryRoot, "DatabaseSeeder", "Seeders", "IndustrialisedCatalogue");
		var document = IndustrialisedItemCatalogue.LoadDirectory(directory);
		var sourceHash = SourceFingerprint(directory);
		// Generate and validate every output before writing any; invalid graph or scope data must not partially refresh audits.
		var outputs = new Dictionary<string, string>(StringComparer.Ordinal)
		{
			[RelativePath] = Generate(document, sourceHash),
			[IndustrialisedClothingAudit.RelativePath] = IndustrialisedClothingAudit.Generate(document.Clothing, sourceHash),
			[IndustrialisedClothingDependencyAudit.RelativePath] = IndustrialisedClothingDependencyAudit.Generate(repositoryRoot)
		};
		if (document.Food is not null)
		{
			outputs[IndustrialisedFoodDependencyAudit.RelativePath] = IndustrialisedFoodDependencyAudit.Generate(document.Food, sourceHash);
		}
		if (check)
		{
			var stale = outputs.Where(pair =>
			{
				var path = Path.Combine(repositoryRoot, pair.Key.Replace('/', Path.DirectorySeparatorChar));
				return !File.Exists(path) || NormalizeLines(File.ReadAllText(path)) != pair.Value;
			}).Select(x => x.Key).ToArray();
			if (stale.Length > 0) throw new InvalidDataException($"Stale catalogue audits: {string.Join(", ", stale)}. Review source changes and refresh the derived audits.");
		}
		else
		{
			foreach (var (relativePath, content) in outputs)
			{
				var path = Path.Combine(repositoryRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
				Directory.CreateDirectory(Path.GetDirectoryName(path)!);
				File.WriteAllText(path, content, new UTF8Encoding(false));
			}
		}
	}

	internal static bool TryHandleCommand(string[] args)
	{
		var check = args.Contains("--check-industrialised-catalogue", StringComparer.Ordinal);
		var refresh = args.Contains("--export-industrialised-catalogue", StringComparer.Ordinal);
		if (!check && !refresh)
		{
			return false;
		}

		try
		{
			if (check == refresh || args.Length != 1)
			{
				throw new ArgumentException("Choose exactly one Industrialised catalogue check or export command, without other arguments.");
			}

			RefreshOrCheck(ItemSeederManifestCatalogue.FindRepositoryRoot(), check);
			Console.WriteLine("Industrialised item, clothing graph and full-inventory dependency audits are current. Authored sources were read, not rewritten. Physical and production acceptance are separate.");
		}
		catch (Exception ex) when (ex is IOException or InvalidDataException or ArgumentException or InvalidOperationException)
		{
			Console.Error.WriteLine(ex.Message);
			Environment.ExitCode = 1;
		}

		return true;
	}

	private static string Sorted(IEnumerable<string> values) => string.Join(';', values.Distinct(StringComparer.Ordinal)
		.OrderBy(x => x, StringComparer.Ordinal));
	private static string NormalizeLines(string text) => text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
}
