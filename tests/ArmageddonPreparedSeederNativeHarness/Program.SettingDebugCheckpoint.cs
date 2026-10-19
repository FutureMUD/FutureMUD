#nullable enable
using System.Text.Json;
using MudSharp.Database;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record SettingDebugCheckpoint(string Kind, string Database, string SqlFile, string SqlSha256,
		string SchemaSha256, string ContentSha256, string ProducerSha256);
	private static string SettingDebugRoot => Path.Combine(CatalogueRepository, ".artifacts", "setting-debug-fixtures");
	private static string SettingProducerHash()
	{
		var paths = new[] { "DatabaseSeeder", "FutureMUDLibrary", "MudsharpDatabaseLibrary", "ExpressionEngine", "Design Documents/Data" }
			.SelectMany(dir => Directory.EnumerateFiles(Path.Combine(CatalogueRepository, dir), "*", SearchOption.AllDirectories))
			.Where(path => !Path.GetRelativePath(CatalogueRepository,path).Replace('\\','/').Split('/').Any(x=>x is "bin" or "obj"))
			.Where(path => new[] { ".cs", ".csproj", ".json", ".tsv", ".sql", ".props", ".targets" }.Contains(Path.GetExtension(path)))
			.OrderBy(path=>Path.GetRelativePath(CatalogueRepository,path),StringComparer.Ordinal);
		return BaseValueHash(paths.Select(path=>new BaseSource(Path.GetRelativePath(CatalogueRepository,path).Replace('\\','/'),BaseHash(path))).ToArray());
	}
	private static void CaptureSettingDebugCheckpoint(TestDatabase database)
	{
		var folder=Path.Combine(SettingDebugRoot,DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ")+"_"+Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(folder);
		var sql=new MySqlDatabaseBackupService().CreateBackup(database.ConnectionString,folder);
		RequireBaseDumpTargets(File.ReadAllText(sql),database.Name);
		var receipt=new SettingDebugCheckpoint("FutureMUD-Setting-debug-checkpoint-v1",database.Name,Path.GetFileName(sql),BaseHash(sql),BaseSchemaHash(database),BaseContentHash(database),SettingProducerHash());
		var descriptor=Path.Combine(folder,"setting-debug.json");File.WriteAllText(descriptor,JsonSerializer.Serialize(receipt,BaseJsonOptions));
		File.SetAttributes(sql,File.GetAttributes(sql)|FileAttributes.ReadOnly);
		File.SetAttributes(descriptor,File.GetAttributes(descriptor)|FileAttributes.ReadOnly);
		Console.WriteLine("SETTING-debug-checkpoint="+descriptor+" SHA256="+BaseHash(descriptor)+"; debug-only, final acceptance restores the clean three-step baseline");
	}
	private static int RestoreSettingDebugCheckpoint()
	{
		var descriptor=Path.GetFullPath(Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_FIXTURE")!);
		var digest=Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_FIXTURE_SHA256");
		RequireBasePath(descriptor,SettingDebugRoot);
		Require(Path.GetFileName(descriptor)=="setting-debug.json" && digest is not null && BaseHash(descriptor)==digest && File.GetAttributes(descriptor).HasFlag(FileAttributes.ReadOnly),"Debug checkpoint descriptor differs from its explicit sealed pin.");
		var receipt=JsonSerializer.Deserialize<SettingDebugCheckpoint>(File.ReadAllText(descriptor))!;
		Require(receipt.Kind=="FutureMUD-Setting-debug-checkpoint-v1" && TestDatabase.HasOwnedPrefix(receipt.Database) &&
			receipt.ProducerSha256==SettingProducerHash() && Path.GetFileName(receipt.SqlFile)==receipt.SqlFile && receipt.SqlFile.EndsWith(".sql",StringComparison.OrdinalIgnoreCase),"Debug checkpoint producer/kind/SQL identity differs.");
		var sql=Path.Combine(Path.GetDirectoryName(descriptor)!,receipt.SqlFile);RequireBasePath(sql,SettingDebugRoot);
		Require(BaseHash(sql)==receipt.SqlSha256 && File.GetAttributes(sql).HasFlag(FileAttributes.ReadOnly),"Debug checkpoint SQL is changed or unsealed.");
		RequireBaseDumpTargets(File.ReadAllText(sql),receipt.Database);
		using(var database=TestDatabase.CreateFresh("futuremud_land_"))
		{
			database.RestoreCatalogueFixture(sql,receipt.Database);ConfigureNativeDatabase(database.ConnectionString);
			Require(BaseSchemaHash(database)==receipt.SchemaSha256 && BaseContentHash(database)==receipt.ContentSha256,"Debug checkpoint restore differs from exact captured schema/content.");
			RunSettingBindingAudit(database,"debug-restored");
			QualifySettingNativeRolesAndHeritage(database);
		}
		Require(BaseHash(descriptor)==digest && BaseHash(sql)==receipt.SqlSha256 && SettingProducerHash()==receipt.ProducerSha256,"Debug sealed checkpoint or producer changed during qualification.");
		WriteBaseRunReceipt(new { Status="PASS", Scope="Focused native gameplay debug checkpoint; no baseline install or avatar boot certificate",Descriptor=descriptor,DescriptorSha256=digest,OwnedDatabasesDisposed=true });
		return 0;
	}
}
