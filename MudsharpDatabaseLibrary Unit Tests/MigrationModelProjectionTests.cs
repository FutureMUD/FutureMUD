#nullable enable
using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MigrationModelCompiler;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MigrationModelProjectionTests
{
	private const string Body = "{ modelBuilder.Entity(\"Example\", b => { b.Property<string>(\"Name\").HasAnnotation(\"Value\", \"unchanged\"); }); }";
	private static string Source(string members) => "using Microsoft.EntityFrameworkCore;\nusing MudSharp.Database;\nnamespace MudSharp.Migrations { [DbContext(typeof(FuturemudDatabaseContext))] [Migration(\"20260101000000_Example\")] partial class Example { " + members + " } }";
	private static string Model(string body) => Source("protected override void BuildTargetModel(ModelBuilder modelBuilder) " + body);

	[TestMethod]
	public void Projection_PreservesModelBodyAndDiscoveryAttributes_AndOnlyForwardsImplementation()
	{
		var result = ModelProjection.Create(Model(Body), "Batch0001", "example");
		StringAssert.Contains(result.Helper!, Body);
		StringAssert.Contains(result.Helper!, "namespace MudSharp.MigrationModels.Batch0001;");
		Assert.IsFalse(result.Helper!.Contains("MudSharp.Database"));
		StringAssert.Contains(result.Forwarder, "[DbContext(typeof(FuturemudDatabaseContext))]");
		StringAssert.Contains(result.Forwarder, "[Migration(\"20260101000000_Example\")]");
		StringAssert.Contains(result.Forwarder, "global::MudSharp.MigrationModels.Batch0001.Model_Example.Build(modelBuilder);");
		Assert.IsFalse(result.Forwarder.Contains("unchanged"));
		Assert.AreEqual(result, ModelProjection.Create(Model(Body), "Batch0001", "example"));
	}

	[TestMethod]
	public void Projection_EmptyHistoricalDesigner_PreservesDiscoveryWithoutAddingModel()
	{
		var source = Source("");
		var result = ModelProjection.Create(source, "Batch0001", "example");
		Assert.AreEqual(source, result.Forwarder);
		Assert.IsNull(result.Helper);
	}

	[DataTestMethod]
	[DataRow("protected override void BuildTargetModel(ModelBuilder modelBuilder) { base.BuildTargetModel(modelBuilder); }")]
	[DataRow("protected override void BuildTargetModel(ModelBuilder modelBuilder) { this.Other(); }")]
	[DataRow("private int state; protected override void BuildTargetModel(ModelBuilder modelBuilder) { }")]
	[DataRow("protected override void BuildTargetModel(ModelBuilder modelBuilder) => modelBuilder.HasAnnotation(\"a\", \"b\");")]
	public void Projection_UnsupportedShape_FailsClosed(string members) =>
		Assert.ThrowsException<InvalidDataException>(() => ModelProjection.Create(Source(members), "Batch0001", "example"));

	[TestMethod]
	public void Manifest_RejectsDuplicateAssignments()
	{
		InDirectory(directory =>
		{
			var path = Path.Combine(directory, "manifest.json");
			File.WriteAllText(path, "{\"Files\":{\"one.Designer.cs\":\"Batch0001\",\"one.Designer.cs\":\"Batch0002\"}}");
			Assert.ThrowsException<InvalidDataException>(() => PartitionManifest.Load(path));
		});
	}

	[TestMethod]
	public void Manifest_RejectsMissingExtraAndOversizedAssignments_AndNormalisesLineEndings()
	{
		InDirectory(directory =>
		{
			var path = Path.Combine(directory, "one.Designer.cs");
			File.WriteAllText(path, Model(Body).Replace("\n", "\r\n"));
			var manifest = PartitionManifest.Partition(directory);
			manifest.Validate(directory);
			File.WriteAllText(path, Model(Body));
			CollectionAssert.AreEqual(manifest.Files.ToArray(), PartitionManifest.Partition(directory).Files.ToArray());
			manifest.Files.Clear();
			Assert.ThrowsException<InvalidDataException>(() => manifest.Validate(directory));
			manifest.Files.Add("one.Designer.cs", "Batch0001");
			manifest.Files.Add("missing.Designer.cs", "Batch0001");
			Assert.ThrowsException<InvalidDataException>(() => manifest.Validate(directory));
			manifest.Files.Remove("missing.Designer.cs");
			File.WriteAllText(path, new string(' ', PartitionManifest.MaximumBatchBytes + 1));
			Assert.ThrowsException<InvalidDataException>(() => manifest.Validate(directory));
			Assert.ThrowsException<InvalidDataException>(() => PartitionManifest.Partition(directory));
		});
	}

	private static void InDirectory(Action<string> action)
	{
		var directory = Path.Combine(Path.GetTempPath(), "migration-projection-" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(directory);
		try { action(directory); }
		finally
		{
			foreach (var file in Directory.GetFiles(directory)) File.Delete(file);
			Directory.Delete(directory);
		}
	}

	[TestMethod]
	public void Generation_TracksSourceChanges_RemovesStaleFiles_AndRepairsMissingOrTamperedOutputs()
	{
		InDirectory(directory =>
		{
			var input = Path.Combine(directory, "one.Designer.cs");
			File.WriteAllText(input, Model(Body));
			var manifest = PartitionManifest.Partition(directory);
			var generated = Path.Combine(directory, "one.Designer.cs.g.cs");
			ProjectionWriter.Write(manifest, directory, "Batch0001", directory);
			var original = File.ReadAllText(generated);
			var timestamp = File.GetLastWriteTimeUtc(generated);
			ProjectionWriter.Write(manifest, directory, "Batch0001", directory);
			Assert.AreEqual(timestamp, File.GetLastWriteTimeUtc(generated), "Unchanged projections should not trigger recompilation.");
			File.WriteAllText(generated, "corrupted output");
			ProjectionWriter.Write(manifest, directory, "Batch0001", directory);
			Assert.AreEqual(original, File.ReadAllText(generated));
			File.Delete(generated);
			ProjectionWriter.Write(manifest, directory, "Batch0001", directory);
			Assert.AreEqual(original, File.ReadAllText(generated));
			File.WriteAllText(input, Model(Body.Replace("unchanged", "changed")));
			ProjectionWriter.Write(manifest, directory, "Batch0001", directory);
			StringAssert.Contains(File.ReadAllText(generated), "changed");
			Assert.AreNotEqual(original, File.ReadAllText(generated));
			File.WriteAllText(Path.Combine(directory, "obsolete.Designer.cs.g.cs"), "old projection");
			File.WriteAllText(Path.Combine(directory, "unrelated.txt"), "keep");
			ProjectionWriter.Write(manifest, directory, "forwarders", directory);
			StringAssert.Contains(File.ReadAllText(generated), "global::MudSharp.MigrationModels.Batch0001.Model_Example.Build");
			Assert.IsFalse(File.Exists(Path.Combine(directory, "obsolete.Designer.cs.g.cs")));
			Assert.AreEqual("keep", File.ReadAllText(Path.Combine(directory, "unrelated.txt")));
			File.WriteAllText(Path.Combine(directory, "projection-cache.json"), "damaged JSON");
			manifest.Files["one.Designer.cs"] = "Batch0002";
			ProjectionWriter.Write(manifest, directory, "forwarders", directory);
			StringAssert.Contains(File.ReadAllText(generated), "global::MudSharp.MigrationModels.Batch0002.Model_Example.Build");
		});
	}

	[TestMethod]
	public void Generation_CaseOnlyRename_DoesNotRetainDuplicateOutput()
	{
		InDirectory(directory =>
		{
			var oldPath = Path.Combine(directory, "one.Designer.cs");
			File.WriteAllText(oldPath, Model(Body));
			ProjectionWriter.Write(PartitionManifest.Partition(directory), directory, "Batch0001", directory);
			File.Delete(oldPath);
			File.WriteAllText(Path.Combine(directory, "One.Designer.cs"), Model(Body));
			ProjectionWriter.Write(PartitionManifest.Partition(directory), directory, "Batch0001", directory);
			if (!OperatingSystem.IsWindows()) Assert.IsFalse(File.Exists(Path.Combine(directory, "one.Designer.cs.g.cs")));
			Assert.AreEqual(1, Directory.GetFiles(directory, "*.Designer.cs.g.cs").Length);
			Assert.IsTrue(File.Exists(Path.Combine(directory, "One.Designer.cs.g.cs")));
		});
	}
}
