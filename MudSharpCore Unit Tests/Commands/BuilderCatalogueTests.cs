#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Commands.Helpers;
using MudSharp.Commands.Modules;
using MudSharp.Framework;
using MudSharp.Planes;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
public class BuilderCatalogueTests
{
	public TestContext TestContext { get; set; } = null!;

	[DataTestMethod]
	[DataRow("--limit 0")]
	[DataRow("--limit 1001")]
	[DataRow("--page -1")]
	[DataRow("--page 2 --page 3")]
	[DataRow("--sort revision")]
	[DataRow("--export xml")]
	[DataRow("--count --export csv")]
	[DataRow("--unknown")]
	public void Options_InvalidOrConflictingInputIsRefused(string input) =>
		Assert.IsFalse(BuilderCatalogueOptions.TryParse(new StringStack(input), false, out _, out _, out _));

	[TestMethod]
	public void Options_QuotedDomainOperandsRetainTheirBoundaries()
	{
		Assert.IsTrue(BuilderCatalogueOptions.TryParse(new StringStack("zone \"South Coast\" \"+two words\" --page 2"), false,
			out var options, out var filters, out var error), error);
		Assert.AreEqual(50, options.Limit);
		CollectionAssert.AreEqual(new[] { "zone", "South Coast", "+two words" }, filters.PopSpeechAll().ToArray());
	}

	[TestMethod]
	public void Selection_SortAndPagingAreStableAndOverflowSafe()
	{
		var items = new[] { new Entry(3, "Alpha"), new Entry(1, "Beta"), new Entry(2, "alpha") };
		Assert.IsTrue(BuilderCatalogueOptions.TryParse(new StringStack("--sort name --limit 2"), false, out var options, out _, out _));
		CollectionAssert.AreEqual(new long[] { 2, 3 }, options.Select(items, out var total).Select(x => x.Id).ToArray());
		Assert.AreEqual(3, total);
		Assert.IsTrue(BuilderCatalogueOptions.TryParse(new StringStack("--page 2147483647 --limit 1000"), false, out options, out _, out _));
		Assert.AreEqual(0, options.Select(items, out total).Count);
		Assert.AreEqual(3, total);
	}

	[DataTestMethod]
	[DataRow("a,\"b\"\nc", "\"a,\"\"b\"\"\nc\"")]
	[DataRow("=HYPERLINK(1)", "\"'=HYPERLINK(1)\"")]
	[DataRow("  +SUM(1)", "\"'  +SUM(1)\"")]
	public void CsvExport_EscapesDataAndNeutralisesSpreadsheetFormulas(string value, string expected) =>
		Assert.AreEqual(expected, BuilderCatalogueOptions.CsvCell(value));

	[TestMethod]
	public void LargeCatalogue_OnlySelectedRowsAreFormattedAndCountFormatsNone()
	{
		const int size = 25000;
		var entries = Enumerable.Range(1, size).Select(x => new Entry(x, $"Catalogue entry {x:00000}")).ToArray();
		var actor = BuilderWorkflowRegressionTests.Actor(BuilderWorkflowRegressionTests.World().Object);
		var formatted = 0;
		IEnumerable<IEnumerable<string>> Rows(IEnumerable<Entry> selected) => selected.Select(x =>
		{
			formatted++;
			return new[] { x.Id.ToString(), x.Name, "Representative description and reference metadata" };
		});
		foreach (var input in new[] { "--limit 50", "", "--count" })
		{
			Assert.IsTrue(BuilderCatalogueOptions.TryParse(new StringStack(input), false, out var options, out _, out _));
			formatted = 0;
			var before = GC.GetAllocatedBytesForCurrentThread();
			var watch = Stopwatch.StartNew();
			options.Display(actor.Object, entries, Rows, new[] { "ID", "Name", "Details" });
			watch.Stop();
			TestContext.WriteLine($"Catalogue={size}; options='{input}'; formatted={formatted}; elapsedMs={watch.Elapsed.TotalMilliseconds:F2}; allocatedBytes={GC.GetAllocatedBytesForCurrentThread() - before}");
			Assert.AreEqual(input == "--count" ? 0 : input.Length == 0 ? size : 50, formatted);
		}
	}

	[DataTestMethod]
	[DataRow("--limit 1", "First Displayed", "Last Displayed")]
	[DataRow("--limit 1 --page 2", "Last Displayed", "First Displayed")]
	[DataRow("--limit 1 --sort id", "Last Displayed", "First Displayed")]
	public void PlaneList_PagesFollowFamilyOrderUnlessSortIsExplicit(string options, string expected, string excluded)
	{
		var world = BuilderWorkflowRegressionTests.World(); var actor = BuilderWorkflowRegressionTests.Actor(world.Object);
		var planes = new All<IPlane>();
		planes.Add(Mock.Of<IPlane>(x => x.Id == 1 && x.Name == "Last Displayed" && x.DisplayOrder == 100));
		planes.Add(Mock.Of<IPlane>(x => x.Id == 2 && x.Name == "First Displayed" && x.DisplayOrder == 0));
		world.SetupGet(x => x.Planes).Returns(planes);
		var output = BuilderWorkflowImprovementTests.Capture(actor);
		BaseBuilderModule.GenericList(actor.Object, new StringStack(options), EditableItemHelper.PlaneHelper);
		var text = string.Join("\n", output);
		StringAssert.Contains(text, expected);
		Assert.IsFalse(text.Contains(excluded));
	}

	private sealed record Entry(long Id, string Name) : IFrameworkItem
	{
		public string FrameworkItemType => "Catalogue fixture";
	}
}
