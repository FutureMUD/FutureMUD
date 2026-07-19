#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

internal static class SeederSourceTestHelper
{
	private static readonly Lazy<string> SeedersRoot = new(FindSeedersRoot);
	private static readonly ConcurrentDictionary<string, string> SourcePaths =
		new(StringComparer.OrdinalIgnoreCase);
	private static readonly ConcurrentDictionary<string, string> SourceText =
		new(StringComparer.OrdinalIgnoreCase);
	private static readonly ConcurrentDictionary<string, string> PartialFamilyText =
		new(StringComparer.OrdinalIgnoreCase);

	internal static string GetSeederSourcePath(string fileName)
	{
		return SourcePaths.GetOrAdd(fileName, static name =>
		{
			var matches = Directory
				.EnumerateFiles(SeedersRoot.Value, name, SearchOption.AllDirectories)
				.ToList();

			return matches.Count switch
			{
				1 => matches[0],
				0 => throw new FileNotFoundException($"Could not find seeder source file {name}."),
				_ => throw new InvalidOperationException(
					$"Found multiple seeder source files named {name}: {string.Join(", ", matches)}")
			};
		});
	}

	internal static string ReadSeederSource(string fileName)
	{
		return SourceText.GetOrAdd(fileName, static name => File.ReadAllText(GetSeederSourcePath(name)));
	}

	internal static string ReadPartialFamily(string filePrefix)
	{
		return PartialFamilyText.GetOrAdd(filePrefix, static prefix =>
		{
			var files = Directory
				.EnumerateFiles(SeedersRoot.Value, $"{prefix}*.cs", SearchOption.AllDirectories)
				.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
				.ToList();

			if (files.Count == 0)
			{
				throw new FileNotFoundException($"Could not find any seeder partials beginning with {prefix}.");
			}

			return string.Join(Environment.NewLine, files.Select(File.ReadAllText));
		});
	}

	private static string FindSeedersRoot()
	{
		DirectoryInfo? current = new(AppContext.BaseDirectory);
		while (current is not null)
		{
			var candidate = Path.Combine(current.FullName, "DatabaseSeeder", "Seeders");
			if (Directory.Exists(candidate))
			{
				return candidate;
			}

			current = current.Parent;
		}

		throw new DirectoryNotFoundException("Could not locate DatabaseSeeder/Seeders from the test output directory.");
	}
}
