#nullable enable
using System.Collections;
using System.Text.Json;
using DatabaseSeeder.Seeders;
using MudSharp.Database;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
#if DEBUG
	private static string FullReplayFixtureRoot => Path.Combine(CatalogueRepository, ".artifacts", "full-setting-replay-fixtures");
	private sealed record FullReplayFixture(string Kind, string Database, string SqlFile, string SqlSha256,
		string SchemaSha256, string ContentSha256, string ProductionSha256, string AnswersSha256);

	private static string FullReplayProductionHash() => BaseValueHash(new[]
		{ "DatabaseSeeder", "MudSharpCore", "FutureMUDLibrary", "MudsharpDatabaseLibrary", "ExpressionEngine" }
		.SelectMany(root => Directory.EnumerateFiles(Path.Combine(CatalogueRepository, root), "*", SearchOption.AllDirectories))
		.Where(path => !Path.GetRelativePath(CatalogueRepository, path).Replace('\\', '/').Split('/').Any(x => x is "bin" or "obj"))
		.Select(path => new BaseSource(Path.GetRelativePath(CatalogueRepository, path).Replace('\\', '/'), BaseHash(path)))
		.OrderBy(x => x.Path, StringComparer.Ordinal).ToArray());

	private static string FullReplayAnswersHash(object profile) => BaseValueHash(
		((IEnumerable)profile.GetType().GetProperty("Steps")!.GetValue(profile)!).Cast<object>().Select(step => new BaseStep(
			((Type)step.GetType().GetProperty("SeederType")!.GetValue(step)!).FullName!,
			((IEnumerable)step.GetType().GetProperty("Answers")!.GetValue(step)!).Cast<object>().Select(answer =>
			{
				var id = (string)answer.GetType().GetProperty("Id")!.GetValue(answer)!;
				return new BaseAnswer(id, id == ArmageddonSettingSeeder.CapacityQuestion ? "explicit-LabMUD-Willpower" :
					(string)answer.GetType().GetProperty("Answer")!.GetValue(answer)!);
			}).ToArray())).ToArray());

	private static (string Descriptor, string Sha256) SealFullSettingReplay(TestDatabase database, object profile, string productionHash)
	{
		Require(productionHash == FullReplayProductionHash(), "Production inputs changed during full replay.");
		var folder = Path.Combine(FullReplayFixtureRoot, DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ") + "_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(folder);
		var sql = new MySqlDatabaseBackupService().CreateBackup(database.ConnectionString, folder);
		RequireBaseDumpTargets(File.ReadAllText(sql), database.Name);
		var receipt = new FullReplayFixture("FutureMUD-full-Armageddon-replay-v1", database.Name, Path.GetFileName(sql), BaseHash(sql),
			BaseSchemaHash(database), BaseContentHash(database), productionHash, FullReplayAnswersHash(profile));
		var descriptor = Path.Combine(folder, "full-replay.json");
		File.WriteAllText(descriptor, JsonSerializer.Serialize(receipt, BaseJsonOptions));
		File.SetAttributes(sql, File.GetAttributes(sql) | FileAttributes.ReadOnly);
		File.SetAttributes(descriptor, File.GetAttributes(descriptor) | FileAttributes.ReadOnly);
		Console.WriteLine("SETTING-full-sealed=" + descriptor + " sha256:" + BaseHash(descriptor));
		return (descriptor, BaseHash(descriptor));
	}

	private static int RestoreFullSettingReplay(string supplied, object profile, string? capturedHash = null)
	{
		var descriptor = Path.GetFullPath(supplied);
		RequireBasePath(descriptor, FullReplayFixtureRoot);
		var suppliedHash = capturedHash ?? Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_FIXTURE_SHA256");
		Require(Path.GetFileName(descriptor) == "full-replay.json" && suppliedHash is not null &&
			suppliedHash.Length == 64 && suppliedHash.All(Uri.IsHexDigit) && BaseHash(descriptor) == suppliedHash &&
			File.GetAttributes(descriptor).HasFlag(FileAttributes.ReadOnly), "Full replay descriptor must be explicitly pinned and sealed.");
		var receipt = JsonSerializer.Deserialize<FullReplayFixture>(File.ReadAllText(descriptor))!;
		Require(receipt.Kind == "FutureMUD-full-Armageddon-replay-v1" && TestDatabase.HasOwnedPrefix(receipt.Database) &&
			receipt.ProductionSha256 == FullReplayProductionHash() && receipt.AnswersSha256 == FullReplayAnswersHash(profile) &&
			Path.GetFileName(receipt.SqlFile) == receipt.SqlFile && receipt.SqlFile.EndsWith(".sql", StringComparison.OrdinalIgnoreCase),
			"Full replay fixture identity, production closure or answer inventory differs.");
		var sql = Path.Combine(Path.GetDirectoryName(descriptor)!, receipt.SqlFile);
		RequireBasePath(sql, FullReplayFixtureRoot);
		Require(BaseHash(sql) == receipt.SqlSha256 && File.GetAttributes(sql).HasFlag(FileAttributes.ReadOnly), "Full replay SQL differs from its seal.");
		RequireBaseDumpTargets(File.ReadAllText(sql), receipt.Database);
		using var database = TestDatabase.CreateFresh("futuremud_land_");
		database.RestoreCatalogueFixture(sql, receipt.Database);
		ConfigureNativeDatabase(database.ConnectionString);
		Require(BaseSchemaHash(database) == receipt.SchemaSha256 && BaseContentHash(database) == receipt.ContentSha256,
			"Restored full replay schema/content differs from its sealed checkpoint.");
		Console.WriteLine("SETTING-full-restore=PASS exact sealed checkpoint; no seeders replayed " + descriptor);
		return QualifyFullSettingReplay(database);
	}
#endif
}
