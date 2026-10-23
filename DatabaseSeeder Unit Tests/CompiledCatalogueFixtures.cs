#nullable enable

using DatabaseSeeder.Seeders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

/// <summary>Inspects emitted literal factories; no production discovery or catalogue generation uses reflection.</summary>
internal static class CompiledCatalogueFixtures
{
	private static readonly Lazy<IReadOnlyList<CatalogueItemDefinition>> ItemsLazy = new(() =>
		Directory.EnumerateFiles(AppContext.BaseDirectory, "DatabaseSeeder.Catalogue.Historical*.dll")
			.OrderBy(path => path, StringComparer.Ordinal)
			.Select(path => Assembly.Load(AssemblyName.GetAssemblyName(path)))
			.SelectMany(assembly => assembly.GetExportedTypes())
			.SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static))
			.Where(method => method.ReturnType == typeof(CatalogueItemDefinition) && method.GetParameters().Length == 0)
			.Select(method => (CatalogueItemDefinition)method.Invoke(null, null)!)
			.ToArray());

	internal static IReadOnlyList<CatalogueItemDefinition> Items => ItemsLazy.Value;

	internal static CatalogueItemDefinition[] FromSource(string file) => Items
		.Where(item => item.Metadata.Source.Contains($"/{file}:", StringComparison.Ordinal))
		.OrderBy(item => int.Parse(item.Metadata.Source[(item.Metadata.Source.LastIndexOf(':') + 1)..], System.Globalization.CultureInfo.InvariantCulture))
		.ToArray();

	internal static CatalogueItemDefinition Item(string stableKey) => Items.Single(item => item.Metadata.StableKey == stableKey);
}
