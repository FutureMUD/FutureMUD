#nullable enable
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Globalization;
using System.Xml.Linq;
using MudSharp.GameItems;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MySql.Data.MySqlClient;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	// Reconcile unchanged owned support rows through the normal installer, before private gameplay fixtures.
	private static void ReconcileS4VoiceSupport(TestDatabase database, string inputPath)
	{
		var before = S4TableChecksums(database);
		long progId;
		string OtherClaims()
		{
			using var db = NewIndependentContext(database.ConnectionString);
			return JsonSerializer.Serialize(db.SeederManagedRecords.AsNoTracking()
				.Where(x => x.StableKey != ArmageddonStockSupportInstaller.DeadSpeakLinkKey).OrderBy(x => x.Id).ToList());
		}
		var otherClaims = OtherClaims();
		using (var db = NewIndependentContext(database.ConnectionString))
			progId = db.SeederManagedRecords.Single(x => x.Seeder == ArmageddonMagicInstaller.Package &&
				x.StableKey == ArmageddonStockSupportInstaller.DeadSpeakLinkKey && !x.Retired).LogicalId!.Value;
		var result = ArmageddonStockSupportInstaller.Install(() => NewIndependentContext(database.ConnectionString));
		Require(result.Status == ArmageddonInstallStatus.Completed, "Owned Voice support reconciliation failed.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(db.SeederManagedRecords.Single(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == ArmageddonStockSupportInstaller.DeadSpeakLinkKey && !x.Retired).LogicalId == progId,
				"Voice support changed owned prog identity.");
			var parameters = db.FutureProgsParameters.Where(x => x.FutureProgId == progId).OrderBy(x => x.ParameterIndex).ToList();
			Require(parameters.Select(x => x.ParameterTypeDefinition).SequenceEqual(new[] { ProgVariableTypes.Character.ToStorageString(), ProgVariableTypes.Item.ToStorageString() }) && parameters[1].ParameterName == "corpse",
				"Voice support has an incompatible native signature.");
		}
		var after = S4TableChecksums(database);
		Require(before.Keys.Order().SequenceEqual(after.Keys.Order()), "Support reconciliation changed schema table inventory.");
		var changed = before.Keys.Where(x => before[x] != after[x]).Order().ToArray();
		Require(changed.All(x => x.Equals("FutureProgs_Parameters", StringComparison.OrdinalIgnoreCase) || x.Equals("SeederManagedRecords", StringComparison.OrdinalIgnoreCase)) && otherClaims == OtherClaims(),
			"Support reconciliation changed unrelated content/player tables or ownership records: " + string.Join(",", changed));
		var checksum = PreparedDatabaseChecksum(database);
		result = ArmageddonStockSupportInstaller.Install(() => NewIndependentContext(database.ConnectionString));
		Require(result.Status == ArmageddonInstallStatus.Completed && checksum == PreparedDatabaseChecksum(database), "Reconciled support rerun changed the database.");
		var input = JsonNode.Parse(File.ReadAllText(inputPath))!;
		input["support_reconciliation"] = JsonSerializer.SerializeToNode(new { prog = progId, changed_tables = changed, unrelated_claims_unchanged = true, rerun_noop = true, checksum });
		File.WriteAllText(inputPath, input.ToJsonString());
		Console.WriteLine("ARMS4-support-reconciled=same-owned-prog exact-character-item-signature unrelated-tables-preserved rerun-noop");
	}

	private static Dictionary<string, string> S4TableChecksums(TestDatabase database)
	{
		using var connection = new MySqlConnection(database.ConnectionString);
		OwnedConnections.Validate("s4-table-checksums-before-connect", connection); connection.Open();
		OwnedConnections.Validate("s4-table-checksums-connected", connection);
		using var command = connection.CreateCommand();
		command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @name ORDER BY TABLE_NAME";
		command.Parameters.AddWithValue("@name", database.Name); var tables = new List<string>();
		using (var reader = command.ExecuteReader()) while (reader.Read()) tables.Add(reader.GetString(0));
		var result = new Dictionary<string, string>(StringComparer.Ordinal);
		foreach (var table in tables)
		{
			command.Parameters.Clear(); command.CommandText = "SELECT * FROM `" + table.Replace("`", "``") + "`";
			var rows = new List<string>();
			using (var reader = command.ExecuteReader()) while (reader.Read())
			{
				var values = new object[reader.FieldCount]; reader.GetValues(values); rows.Add(JsonSerializer.Serialize(values));
			}
			result[table] = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(rows.Order(StringComparer.Ordinal)))));
		}
		return result;
	}

	private static JsonNode PrepareS4Witness(FuturemudDatabaseContext db, Db.Character source, long room)
	{
		T Copy<T>(T original) where T : class, new()
		{
			var clone = new T(); db.Entry(clone).CurrentValues.SetValues(original); return clone;
		}
		var account = Copy(db.Accounts.AsNoTracking().Single(x => x.Id == source.AccountId));
		account.Id = 0; account.Name = "S4Witness"; account.LastLoginTime = null;
		db.Add(account); db.SaveChanges();
		var body = Copy(db.Bodies.AsNoTracking().Single(x => x.Id == source.BodyId));
		body.Id = 0; db.Add(body); db.SaveChanges();
		var character = Copy(source); character.Id = 0; character.Name = "S4Witness";
		character.NameInfo = source.NameInfo.Replace(source.Name, "S4Witness", StringComparison.Ordinal);
		character.AccountId = account.Id; character.BodyId = body.Id; character.Location = room;
		character.LastLoginTime = null; character.LastLogoutTime = null;
		db.Add(character); db.SaveChanges();
		// Native loading creates the ordinary primary instance, as for the seeded Admin.
		foreach (var row in db.Traits.AsNoTracking().Where(x => x.BodyId == source.BodyId).ToList())
		{
			var clone = Copy(row); clone.BodyId = body.Id; db.Add(clone);
		}
		foreach (var row in db.CharacterTraits.AsNoTracking().Where(x => x.CharacterId == source.Id).ToList())
		{
			var clone = Copy(row); clone.CharacterId = character.Id; db.Add(clone);
		}
		foreach (var row in db.CharactersLanguages.AsNoTracking().Where(x => x.CharacterId == source.Id).ToList())
		{
			var clone = Copy(row); clone.CharacterId = character.Id; db.Add(clone);
		}
		db.SaveChanges();
		return JsonSerializer.SerializeToNode(new { account = account.Name, character = character.Id, body = body.Id })!;
	}
	private static long PrepareS4PocketBag(FuturemudDatabaseContext db, long sourceId)
	{
		T Copy<T>(T original) where T : class, new()
		{
			var clone = new T(); db.Entry(clone).CurrentValues.SetValues(original); return clone;
		}
		var seed = db.GameItemProtos.Include(x => x.EditableItem).Single(x => x.Id == sourceId);
		var hold = db.GameItemComponentProtos.First(x => x.Type == "Holdable");
		var mass = 1.0 / double.Parse(db.StaticConfigurations.Single(x => x.SettingName == "BaseWeightUOMToKilograms").Definition, CultureInfo.InvariantCulture);
		var component = new Db.GameItemComponentProto
		{
			Id = db.GameItemComponentProtos.Max(x => x.Id) + 1, RevisionNumber = 0,
			Name = "S4 residual ordinary bag", Description = "Disposable native foreign-goods container.", Type = "Container",
			Definition = new XElement("Definition", new XAttribute("Weight", 100 * mass), new XAttribute("MaxSize", (int)SizeCategory.Normal),
				new XAttribute("Closable", true), new XAttribute("Transparent", true), new XAttribute("OnceOnly", false), new XAttribute("Preposition", "in")).ToString(),
			EditableItem = Copy(seed.EditableItem)
		};
		component.EditableItem.Id = 0; db.Add(component); db.SaveChanges();
		var bag = Copy(seed); bag.Id = db.GameItemProtos.Max(x => x.Id) + 1; bag.RevisionNumber = 0;
		bag.Name = "s4residualbag"; bag.UniqueName = "s4residualbag"; bag.Keywords = "s4residualbag bag";
		bag.ShortDescription = "a s4residualbag"; bag.FullDescription = "An ordinary disposable native bag.";
		bag.Weight = 0.1 * mass; bag.Size = (int)SizeCategory.Small;
		bag.EditableItemId = 0; bag.EditableItem = Copy(seed.EditableItem); bag.EditableItem.Id = 0;
		foreach (var proto in new[] { hold, component }) bag.GameItemProtosGameItemComponentProtos.Add(new()
		{
			GameItemComponentProtoId = proto.Id, GameItemComponentRevision = proto.RevisionNumber, GameItemProto = bag
		});
		db.Add(bag); db.SaveChanges(); return bag.Id;
	}

}
