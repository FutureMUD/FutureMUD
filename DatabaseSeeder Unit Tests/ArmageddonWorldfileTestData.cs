#nullable enable

using System.Reflection;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

/// <summary>Armageddon source fixtures stay outside the public repository and hosted test artifacts.</summary>
internal static class ArmageddonWorldfileTestData
{
	public static ArmageddonWorldfilePackage Load()
	{
		RequireAvailable();
		return ArmageddonWorldfilePackage.Load();
	}

	public static void RequireAvailable()
	{
		if (!ArmageddonWorldfilePackage.FileAvailable)
			Assert.Inconclusive(ArmageddonWorldfilePackage.MissingDataMessage);
	}

	public static void AssertNoPrivateCorpusEmbedded()
	{
		var resources = Assembly.Load("DatabaseSeeder").GetManifestResourceNames();
		Assert.IsFalse(resources.Contains("DatabaseSeeder.ArmageddonWorldfiles.json.gz"));
		Assert.IsFalse(resources.Contains("DatabaseSeeder.ArmageddonWorldfiles.manifest.json"));
		Assert.IsFalse(resources.Contains("DatabaseSeeder.ArmageddonWorldfileSpecialText.json"));
	}
}

[TestClass]
public class ArmageddonWorldfileDataBoundaryTests
{
	[TestMethod]
	public void PublishedDatabaseSeederAssembly_DoesNotContainPrivateArmageddonData()
	{
		ArmageddonWorldfileTestData.AssertNoPrivateCorpusEmbedded();
	}

	[TestMethod]
	public void MissingPrivateCorpus_ExplainsHowToConfigureItsLocalDirectory()
	{
		if (ArmageddonWorldfilePackage.FileAvailable)
			Assert.Inconclusive("This refusal check only applies when private Armageddon fixtures are not configured.");
		var error = Assert.ThrowsExactly<System.InvalidOperationException>(() => ArmageddonWorldfilePackage.Load());
		StringAssert.Contains(error.Message, ArmageddonWorldfilePackage.DataDirectoryEnvironmentVariable);
		StringAssert.Contains(error.Message, ArmageddonWorldfilePackage.SpecialTextFileName);
	}
}
