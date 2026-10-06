#nullable enable
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text.Json;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
    private static int PreparedInstalledSqlPreflight()
    {
        Require(!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("FUTUREMUD_PREPARED_REPLAY_BOOT_SCRIPT")), "SQL qualification requires the exact-source companion; no implicit certificate.");
        using var database = TestDatabase.CreateFresh("futuremud_land_");
        using var db = NewIndependentContext(database.ConnectionString);
        db.Database.Migrate();
        var laneRoot = Directory.GetParent(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..")))!.FullName;
        var input = Path.Combine(laneRoot, "prepared-installed-sense-input_" + Guid.NewGuid().ToString("N") + ".json");
        File.WriteAllText(input, JsonSerializer.Serialize(new { sql_schema_only = true, sql_schema = InstalledSqlSchema(db),
            character = 1, body = 1, language = 1, resource = 1, capability = 1, merit = 1, spell = 1, source_skill = 1 }));
        var previous = Environment.GetEnvironmentVariable("FUTUREMUD_PREPARED_INSTALLED_INPUT");
        try
        {
            Environment.SetEnvironmentVariable("FUTUREMUD_PREPARED_INSTALLED_INPUT", input);
            RunPreparedReplayBoot(database, typeof(DatabaseSeeder.Seeders.ArmageddonMagicSeeder).Assembly,
                "PreparedInstalledMagicSmoke.py", "prepared-installed-sense-native", schemaOnly: true);
            Console.WriteLine("ARMPREP-installed-sql-schema=passed fresh-migrations exact-EF-mapping all-query-plans no-player-writes no-MUD");
            return 0;
        }
        finally { Environment.SetEnvironmentVariable("FUTUREMUD_PREPARED_INSTALLED_INPUT", previous); }
    }

    // SQL aliases are the existing DbSet names. Physical names and store types come only from EF.
    private static object InstalledSqlSchema(MudSharp.Database.FuturemudDatabaseContext db)
    {
        var queries = new Dictionary<string, string[]>
        {
            ["Accounts"] = ["Id", "Name"],
            ["Characters"] = ["Id", "AccountId", "Name", "Location", "LastLoginTime", "LastLogoutTime", "IsAdminAvatar", "EffectData", "BodyId", "NativeLanguageId"],
            ["CharactersMagicResources"] = ["CharacterId", "MagicResourceId", "Amount"],
            ["CharacterAcquiredSpells"] = ["CharacterId", "MagicSpellId", "ControlledGrade"],
            ["CharacterTraits"] = ["CharacterId", "TraitDefinitionId", "Value"],
            ["CharacterCastingEnrolments"] = ["CharacterId", "MagicCapabilityId"],
            ["MagicCastingOperations"] = ["Id", "Stage", "Definition", "Diagnostic", "CharacterId", "MagicSpellId", "CreatedUtc"],
            ["MagicGatheringOperations"] = ["Id", "Status", "Kind", "RequestedAmount", "StaminaCost", "BodilyCostApplied", "DestinationCredited", "AccountingPersisted", "OwnerId", "MagicCapabilityId", "DestinationResourceId"],
            ["MagicSpells"] = ["Id", "Definition"],
            ["StaticConfigurations"] = ["SettingName", "Definition"],
            ["Bodies"] = ["Id"], ["Languages"] = ["Id"], ["MagicResources"] = ["Id"],
            ["MagicCapabilities"] = ["Id"], ["Merits"] = ["Id"],
            ["TraitDefinitions"] = ["Id", "Type", "OwnerScope"]
        };
        return queries.Select(query =>
        {
            var set = db.GetType().GetProperty(query.Key) ?? throw new InvalidOperationException("Missing SQL DbSet " + query.Key);
            var entity = db.Model.FindEntityType(set.PropertyType.GetGenericArguments().Single()) ?? throw new InvalidOperationException("Unmapped SQL entity " + query.Key);
            var table = entity.GetTableName() ?? throw new InvalidOperationException("Missing EF table mapping");
            var store = StoreObjectIdentifier.Table(table, entity.GetSchema());
            return new
            {
                alias = query.Key, table,
                columns = query.Value.Select(name =>
                {
                    var property = entity.FindProperty(name) ?? throw new InvalidOperationException("Unmapped SQL property " + query.Key + "." + name);
                    return new { alias = name, column = property.GetColumnName(store), store_type = property.GetRelationalTypeMapping().StoreType,
                        clr_type = property.ClrType.FullName, nullable = property.IsNullable };
                }).ToArray()
            };
        }).ToArray();
    }
}
