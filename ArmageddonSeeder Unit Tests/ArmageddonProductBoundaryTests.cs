#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MudSharp_Unit_Tests;
[TestClass]
public class ArmageddonProductBoundaryTests
{
	[TestMethod]
	public void OptionalHost_DiscoversItsPackagesAndSharedFoundationsWithoutAnExecutableDependency()
	{
		var seeders = ArmageddonSeederProgram.GetEnabledSeeders();
		Assert.AreEqual(1, seeders.Count(x => x is ArmageddonSettingSeeder));
		Assert.AreEqual(1, seeders.Count(x => x is ArmageddonWorldfileSeeder));
		Assert.IsTrue(seeders.Any(x => x is CoreDataSeeder));
		Assert.IsTrue(SkillPackageSeeder.Presets.Any(x => x.Complexity == "armageddon"));
		Assert.IsFalse(typeof(ArmageddonSettingSeeder).Assembly.GetReferencedAssemblies().Any(x => x.Name is "DatabaseSeeder" or "ItemSeeder"));
		Assert.AreEqual(0, SeederPackagePlan.GetDependencyPlan(seeders).Errors.Count);
	}
	[TestMethod]
	public void Distribution_StaysOutsidePublicProductRegistryAndNormalSolution()
	{
		var root = SeederSourceTestHelper.RepositoryRoot;
		using var products = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "FutureMUD.Web/Configuration/release-products.json")));
		Assert.IsFalse(products.RootElement.GetProperty("products").EnumerateArray().Any(x => x.GetProperty("id").GetString() == "armageddonseeder"));
		var normal = File.ReadAllText(Path.Combine(root, "MudSharp.sln"));
		Assert.IsFalse(normal.Contains("ArmageddonSeeder\\", StringComparison.Ordinal));
		Assert.IsFalse(normal.Contains("ArmageddonSeeder Unit Tests", StringComparison.Ordinal));
		Assert.IsFalse(typeof(ArmageddonSettingSeeder).Assembly.GetManifestResourceNames().Any(x => x.Contains("Worldfiles.json", StringComparison.OrdinalIgnoreCase)));
	}
	[TestMethod]
	public void DedicatedCi_CoversEveryExternallyEmbeddedManifest()
	{
		var root = SeederSourceTestHelper.RepositoryRoot;
		var project = XDocument.Load(Path.Combine(root, "ArmageddonSeeder/ArmageddonSeeder.csproj"));
		var workflow = File.ReadAllText(Path.Combine(root, ".github/workflows/armageddon-seeder-tests.yml"));
		foreach (var resource in project.Descendants("EmbeddedResource").Select(x => (string?)x.Attribute("Include")).Where(x => x?.StartsWith("../Design Documents/", StringComparison.Ordinal) == true))
			StringAssert.Contains(workflow, resource![3..]);
	}
#if DEBUG
	[TestMethod]
	public void ProductReplayAndPreparedWorldFixtures_HaveCompleteInventoryAndSafeRefusal()
	{
		var seeders = ArmageddonSeederProgram.GetEnabledSeeders();
		foreach (var profile in ArmageddonSeederReplayProfiles.All.Concat(ArmageddonSeederReplayProfiles.PreparedWorldProfiles))
		{
			var result = ArmageddonSeederReplayProfiles.Runner.Validate(profile, seeders);
			Assert.IsTrue(result.IsValid, string.Join("; ", result.Errors));
		}
		Assert.AreEqual(1, ArmageddonSeederReplayProfiles.All.Count);
		Assert.AreEqual("armageddon-native", ArmageddonSeederReplayProfiles.All.Single().Id);
	}
#endif
}
