#nullable enable
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestReporting.Tests;

[TestClass]
public class TestSelectionTests
{
	[TestMethod]
	public void DefaultSelectionExcludesExtendedCategoriesEvenWithAnExplicitNameFilter()
	{
		Assert.AreEqual(TestSelection.DefaultFilter, TestSelection.EffectiveFilter(null, false));
		Assert.AreEqual($"({TestSelection.DefaultFilter})&(FullyQualifiedName~Installer|TestCategory=Integration)",
			TestSelection.EffectiveFilter("FullyQualifiedName~Installer|TestCategory=Integration", false));
	}

	[TestMethod]
	public void ExtendedSelectionPreservesTheRequestedFilterWithoutExclusions()
	{
		Assert.IsNull(TestSelection.EffectiveFilter(null, true));
		Assert.AreEqual("TestCategory=Integration", TestSelection.EffectiveFilter("TestCategory=Integration", true));
	}

	[TestMethod]
	public void DirectDotnetAndVisualStudioDefaultsMatchTheWrapperPolicy()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (!File.Exists(Path.Combine(directory.FullName, "test-default.runsettings")))
			directory = directory.Parent ?? throw new AssertFailedException("Repository settings not found.");
		var settings = XDocument.Load(Path.Combine(directory.FullName, "test-default.runsettings"));
		Assert.AreEqual(TestSelection.DefaultFilter, settings.Root!.Element("RunConfiguration")!.Element("TestCaseFilter")!.Value);
		var extended = XDocument.Load(Path.Combine(directory.FullName, "test-extended.runsettings"));
		Assert.IsFalse(extended.Descendants("TestCaseFilter").Any());
	}
}
