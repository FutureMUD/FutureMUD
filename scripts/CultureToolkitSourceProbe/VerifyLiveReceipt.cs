#nullable enable

using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MySqlConnector;

internal static class VerifyLiveReceipt
{
	public static void Run(string connection, string database, string output)
	{
		var builder = new MySqlConnectionStringBuilder(connection) { Database = database };
		if (builder.Server is not ("localhost" or "127.0.0.1" or "::1") || !database.StartsWith("futuremud_culture_live_2145_", StringComparison.Ordinal))
			throw new InvalidOperationException("Only the local disposable CultureSeeder fixture may be inspected.");
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(builder.ConnectionString, ServerVersion.AutoDetect(builder.ConnectionString)).Options);
		var identities = context.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == "CultureSeeder").ToArray();
		var progIds = identities.Where(x => x.EntityType == "FutureProg").Select(x => x.LogicalId).ToHashSet();
		var progs = context.FutureProgs.AsNoTracking().Include(x => x.FutureProgsParameters).AsEnumerable()
			.Where(x => progIds.Contains(x.Id) || x.FunctionName is "ChargenFreeSkills" or "ChargenFreeKnowledges")
			.Select(x => new
			{
				x.Id, x.FunctionName, ReturnType = ((ProgVariableTypes)x.ReturnType).ToString(), x.AcceptsAnyParameters,
				Parameters = x.FutureProgsParameters.OrderBy(p => p.ParameterIndex).Select(p => new { p.ParameterIndex, p.ParameterName, Type = ((ProgVariableTypes)p.ParameterType).ToString() }),
				Utf8Bytes = Encoding.UTF8.GetByteCount(x.FunctionText),
				BodySha256 = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(x.FunctionText))),
				NativeBaseBody = x.FunctionName == "CultureLanguageNativeBase" ? x.FunctionText : null,
				SourceKeys = identities.Where(r => r.EntityType == "FutureProg" && r.LogicalId == x.Id).Select(r => r.StableKey)
			}).ToArray();
		File.WriteAllText(output, JsonSerializer.Serialize(new
		{
			Scope = "Read-only live MySQL identity/signature receipt. Compilation is established separately by installer and runtime checks; this query does not compile or execute progs.",
			Database = database, Progs = progs,
			GeneratedCharacters = context.Npcs.AsNoTracking().Where(x => x.Template.UniqueName == "culture_native_pipeline")
				.Select(x => new
				{
					x.CharacterId, x.TemplateId, x.TemplateRevnum, x.Character.Name,
					Traits = x.Character.CharacterTraits.Select(t => new { t.TraitDefinitionId, t.TraitDefinition.Name, t.Value }),
					BodyTraits = x.Character.Body.Traits.Select(t => new { t.TraitDefinitionId, t.TraitDefinition.Name, t.Value }),
					Accents = x.Character.CharactersAccents.Select(a => new { a.AccentId, a.Accent.Name, a.Accent.LanguageId, a.Accent.Group, a.IsPreferred })
				}).ToArray(),
			ManagedIdentities = identities.Select(x => new { x.EntityType, x.StableKey, x.Module, x.LogicalId, HasBaseline = x.SeedBaseline is not null })
		}, new JsonSerializerOptions { WriteIndented = true }));
		Console.WriteLine($"Read {progs.Length} live prog contracts and {identities.Length} managed identities from {database}.");
	}
}
