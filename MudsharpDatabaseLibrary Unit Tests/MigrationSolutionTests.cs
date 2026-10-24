#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MigrationModelCompiler;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MigrationSolutionTests
{
	[TestMethod]
	public void Solution_IncludesEveryMigrationBatchAndCompiler_InEveryBuildConfiguration()
	{
		var repository = new DirectoryInfo(AppContext.BaseDirectory);
		while (repository is not null && !File.Exists(Path.Combine(repository.FullName, "MudSharp.sln")))
		{
			repository = repository.Parent;
		}

		Assert.IsNotNull(repository, "The test must run from the repository build output.");
		var solution = File.ReadAllText(Path.Combine(repository!.FullName, "MudSharp.sln"));
		var projects = Regex.Matches(solution,
			"^Project\\(\"[^\"]+\"\\) = \"[^\"]+\", \"([^\"]+\\.csproj)\", \"([^\"]+)\"", RegexOptions.Multiline)
			.ToDictionary(match => match.Groups[1].Value.Replace('\\', '/'), match => match.Groups[2].Value,
				StringComparer.OrdinalIgnoreCase);
		var configurations = Regex.Match(solution,
			@"GlobalSection\(SolutionConfigurationPlatforms\) = preSolution\s+(.*?)EndGlobalSection",
			RegexOptions.Singleline).Groups[1].Value
			.Split('\n', StringSplitOptions.RemoveEmptyEntries)
			.Select(line => line.Split('=')[0].Trim())
			.Where(line => line.Length > 0)
			.ToArray();
		Assert.IsTrue(configurations.Length > 0, "No solution build configurations found.");

		var manifest = PartitionManifest.Load(Path.Combine(repository.FullName, "MigrationModels", "partitions.json"));
		var requiredProjects = manifest.Files.Values.Distinct()
			.Select(batch => $"MigrationModels/{batch}/{batch}.csproj")
			.Append("BuildTools/MigrationModelCompiler/MigrationModelCompiler.csproj")
			.Append("MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj");
		foreach (var project in requiredProjects)
		{
			Assert.IsTrue(projects.TryGetValue(project, out var id),
				$"Visual Studio restore requires {project} to be included in MudSharp.sln.");
			foreach (var configuration in configurations)
			{
				// These helpers use ordinary Debug/Release even in the game-specific solution configurations.
				var buildConfiguration = configuration.StartsWith("Release", StringComparison.Ordinal) ? "Release" : "Debug";
				foreach (var mapping in new[] { "ActiveCfg", "Build.0" })
				{
					StringAssert.Contains(solution,
						$"{id}.{configuration}.{mapping} = {buildConfiguration}|Any CPU",
						$"Missing or inconsistent {mapping} for {project} in {configuration}.");
				}
			}
		}
	}
}
