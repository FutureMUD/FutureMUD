#nullable enable
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MySqlScriptBatchParserTests
{
	[TestMethod]
	public void Parse_MysqlClientDelimiterDirectives_CreateExecutableBatches()
	{
		const string script = """
		                      START TRANSACTION;
		                      DROP PROCEDURE IF EXISTS MigrationsScript;
		                      DELIMITER //
		                      CREATE PROCEDURE MigrationsScript()
		                      BEGIN
		                          SELECT 1;
		                      END //
		                      DELIMITER ;
		                      CALL MigrationsScript();
		                      DROP PROCEDURE MigrationsScript;
		                      COMMIT;
		                      """;

		var batches = MySqlScriptBatchParser.Parse(script).ToArray();

		Assert.AreEqual(3, batches.Length);
		Assert.AreEqual(";", batches[0].Delimiter);
		StringAssert.Contains(batches[0].Script, "START TRANSACTION;");
		Assert.AreEqual("//", batches[1].Delimiter);
		StringAssert.Contains(batches[1].Script, "CREATE PROCEDURE MigrationsScript()");
		StringAssert.Contains(batches[1].Script, "END //");
		Assert.AreEqual(";", batches[2].Delimiter);
		StringAssert.Contains(batches[2].Script, "CALL MigrationsScript();");
		Assert.IsFalse(batches.Any(x => x.Script.Contains("DELIMITER", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void Parse_CommittedBlankSnapshotDeltas_ConsumesEveryDelimiterDirective()
	{
		var snapshotPath = GetCommittedSnapshotPath();
		var snapshot = File.ReadAllText(snapshotPath);
		var markerIndex = snapshot.IndexOf("-- EF-generated idempotent delta", StringComparison.Ordinal);
		// A full regeneration folds previous deltas into the schema. Both forms are valid installer assets.
		var batches = MySqlScriptBatchParser.Parse(markerIndex >= 0 ? snapshot[markerIndex..] : snapshot).ToArray();

		Assert.IsTrue(batches.Length >= 1);
		Assert.IsTrue(batches.All(x => !string.IsNullOrWhiteSpace(x.Script)));
		Assert.IsTrue(batches.All(x => x.Delimiter is ";" or "//"));
		Assert.IsFalse(batches.Any(x => x.Script.Contains("DELIMITER", StringComparison.OrdinalIgnoreCase)));
		Assert.IsTrue(batches.Any(x =>
			x.Script.Contains("20260829082253_AddOutfitTemplateItemSkin", StringComparison.Ordinal)));
	}

	private static string GetCommittedSnapshotPath([CallerFilePath] string sourceFilePath = "")
	{
		return Path.GetFullPath(Path.Combine(
			Path.GetDirectoryName(sourceFilePath)!,
			"..",
			"DatabaseSeeder",
			"BlankDatabaseSnapshot.sql"));
	}
}
