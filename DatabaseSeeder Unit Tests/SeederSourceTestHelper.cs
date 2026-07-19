#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

internal static class SeederSourceTestHelper
{
	internal static string GetSeederSourcePath(string fileName)
	{
		var matches = Directory
			.EnumerateFiles(GetSeedersRoot(), fileName, SearchOption.AllDirectories)
			.ToList();

		return matches.Count switch
		{
			1 => matches[0],
			0 => throw new FileNotFoundException($"Could not find seeder source file {fileName}."),
			_ => throw new InvalidOperationException($"Found multiple seeder source files named {fileName}: {string.Join(", ", matches)}")
		};
	}

	internal static string ReadSeederSource(string fileName)
	{
		return File.ReadAllText(GetSeederSourcePath(fileName));
	}

	internal static string ReadPartialFamily(string filePrefix)
	{
		var files = Directory
			.EnumerateFiles(GetSeedersRoot(), $"{filePrefix}*.cs", SearchOption.AllDirectories)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToList();

		if (files.Count == 0)
		{
			throw new FileNotFoundException($"Could not find any seeder partials beginning with {filePrefix}.");
		}

		return string.Join(Environment.NewLine, files.Select(File.ReadAllText));
	}

	private static string GetSeedersRoot()
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
