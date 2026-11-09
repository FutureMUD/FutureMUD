#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class IndustrialisedClothingDependencyPlanTests
{

	[TestMethod]
	public void Plan_CollectionsAreReadOnlyAndOrdered()
	{
		var plan = IndustrialisedClothingDependencyPlan.Rows;
		CollectionAssert.AreEqual(plan.Select(x => x.ItemReference).OrderBy(x => x, StringComparer.Ordinal).ToArray(), plan.Select(x => x.ItemReference).ToArray());
		Assert.ThrowsExactly<NotSupportedException>(() => ((IList<ClothingDependencyPlanRow>)plan).Clear());
		foreach (var row in plan)
		{
			foreach (var list in new[] { row.EraAdmissions, row.Components, row.Tags, row.OpenRequirements })
				Assert.ThrowsExactly<NotSupportedException>(() => ((IList<string>)list).Clear());
		}
	}

	[TestMethod]
	public void Audit_IsInvariantOrderedAndDoesNotRewriteInputs()
	{
		using var fixture = new DependencyDirectory();
		var before = fixture.Bytes();
		var first = IndustrialisedClothingDependencyAudit.Generate(fixture.Root);
		var culture = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
			Assert.AreEqual(first, IndustrialisedClothingDependencyAudit.Generate(fixture.Root, IndustrialisedClothingDependencyPlan.Rows.Reverse().ToArray()));
		}
		finally { CultureInfo.CurrentCulture = culture; }
		foreach (var (key, value) in before) CollectionAssert.AreEqual(value, fixture.Bytes()[key]);
	}

	[TestMethod]
	[DataRow("missing")]
	[DataRow("duplicate")]
	[DataRow("unapproved")]
	[DataRow("admissions")]
	[DataRow("reuse")]
	[DataRow("weight")]
	[DataRow("component")]
	public void Audit_RejectsInvalidOrIncompletePlan(string defect)
	{
		var plan = IndustrialisedClothingDependencyPlan.Rows.ToList();
		var index = plan.FindIndex(x => !x.Reused);
		var row = plan[index];
		switch (defect)
		{
			case "missing": plan.RemoveAt(index); break;
			case "duplicate": plan.Add(row); break;
			case "unapproved": plan.Add(row with { ItemReference = "unapproved_garment" }); break;
			case "admissions": plan[index] = row with { EraAdmissions = [] }; break;
			case "reuse": plan[index] = row with { Reused = true }; break;
			case "weight": plan[index] = row with { RequiredLayerWeight = double.NaN }; break;
			case "component": plan[index] = row with { Components = row.Components.Append(row.Components[0].ToLowerInvariant()).ToArray() }; break;
		}
		Assert.ThrowsExactly<InvalidDataException>(() => IndustrialisedClothingDependencyAudit.Generate(ItemSeederManifestCatalogue.FindRepositoryRoot(), plan));
	}

	[TestMethod]
	[DataRow(IndustrialisedClothingDependencyAudit.MaterialsPath, "Material Name", "Material name")]
	[DataRow(IndustrialisedClothingDependencyAudit.TagsPath, "Tag hierarchy", "Tag path")]
	[DataRow(IndustrialisedClothingDependencyAudit.InventoryPath, "| Family |", "| Invalid |")]
	[DataRow(IndustrialisedClothingDependencyAudit.OutfitsPath, "drawstring_drawers;short_undershirt", "unknown_garment;short_undershirt")]
	[DataRow(IndustrialisedClothingDependencyAudit.OutfitsPath, "drawstring_drawers;short_undershirt", "drawstring_drawers@unknown-skin;short_undershirt")]
	[DataRow(IndustrialisedClothingDependencyAudit.ComponentsPath, "\"ActiveCraft\"", "\"not-a-type\"")]
	public void Audit_RejectsMalformedAuthoritativeInputs(string relative, string from, string to)
	{
		using var fixture = new DependencyDirectory();
		var path = Path.Combine(fixture.Root, relative);
		var text = File.ReadAllText(path);
		Assert.IsTrue(text.Contains(from, StringComparison.Ordinal));
		File.WriteAllText(path, text.Replace(from, to, StringComparison.Ordinal));
		Assert.ThrowsExactly<InvalidDataException>(() => IndustrialisedClothingDependencyAudit.Generate(fixture.Root));
	}

	[TestMethod]
	public void Audit_ChangedResourceNamesAndPlanInvalidateFingerprintAndReportExactMissingDependency()
	{
		using var fixture = new DependencyDirectory();
		var first = Rows(IndustrialisedClothingDependencyAudit.Generate(fixture.Root));
		var path = Path.Combine(fixture.Root, IndustrialisedClothingDependencyAudit.MaterialsPath);
		File.WriteAllText(path, File.ReadAllText(path).Replace("\"cotton\"", "\"Cotton\"", StringComparison.Ordinal));
		var changed = Rows(IndustrialisedClothingDependencyAudit.Generate(fixture.Root));
		Assert.AreNotEqual(first[0]["DependencySourceSha256"], changed[0]["DependencySourceSha256"]);
		Assert.IsTrue(changed.Where(x => x["Material"] == "cotton").All(x => x["MissingStockNames"].Contains("material:cotton", StringComparison.Ordinal)));
		var plan = IndustrialisedClothingDependencyPlan.Rows.Select(x => x with { OpenRequirements = ["changed rationale"] }).ToArray();
		Assert.AreNotEqual(changed[0]["DependencySourceSha256"], Rows(IndustrialisedClothingDependencyAudit.Generate(fixture.Root, plan))[0]["DependencySourceSha256"]);
	}

	[TestMethod]
	public void Audit_RejectsCaseAmbiguousStockKeys()
	{
		using var fixture = new DependencyDirectory();
		var path = Path.Combine(fixture.Root, IndustrialisedClothingDependencyAudit.MaterialsPath);
		File.WriteAllText(path, File.ReadAllText(path).Replace("\"wool\"", "\"Cotton\"", StringComparison.Ordinal));
		StringAssert.Contains(Assert.ThrowsExactly<InvalidDataException>(() => IndustrialisedClothingDependencyAudit.Generate(fixture.Root)).Message, "Duplicate");
	}

	internal static Dictionary<string, string>[] Rows(string audit)
	{
		var lines = audit.Split('\n', StringSplitOptions.RemoveEmptyEntries);
		Assert.AreEqual(IndustrialisedClothingDependencyAudit.Header, lines[0]);
		var columns = lines[0].Split('\t');
		return lines.Skip(1).Select(line =>
		{
			var rooms = line.Split('\t');
			Assert.AreEqual(columns.Length, rooms.Length);
			return columns.Zip(rooms).ToDictionary(x => x.First, x => x.Second, StringComparer.Ordinal);
		}).ToArray();
	}

	private sealed class DependencyDirectory : IDisposable
	{
		private readonly DirectoryInfo _directory = Directory.CreateTempSubdirectory("FutureMUD-clothing-dependencies-");
		internal string Root => _directory.FullName;
		internal DependencyDirectory()
		{
			foreach (var relative in IndustrialisedClothingDependencyAudit.InputPaths)
			{
				var target = Path.Combine(Root, relative);
				Directory.CreateDirectory(Path.GetDirectoryName(target)!);
				File.Copy(Path.Combine(ItemSeederManifestCatalogue.FindRepositoryRoot(), relative), target);
			}
		}
		internal Dictionary<string, byte[]> Bytes() => IndustrialisedClothingDependencyAudit.InputPaths.ToDictionary(x => x, x => File.ReadAllBytes(Path.Combine(Root, x)));
		public void Dispose() => _directory.Delete(true);
	}
}
