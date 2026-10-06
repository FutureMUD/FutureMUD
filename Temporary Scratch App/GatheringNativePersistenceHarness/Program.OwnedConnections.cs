#nullable enable

using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static class OwnedConnections
	{
		private sealed record Registration(string Token, bool Ready);
		private static readonly Dictionary<string, Registration> Databases = new(StringComparer.Ordinal);
		private static readonly ConditionalWeakTable<DbConnection, object> VerifiedOpenConnections = new();
		private static string? _activeDatabase;
		private static string ExpectedUuid => Environment.GetEnvironmentVariable("FUTUREMUD_OWNED_MYSQL_UUID")
			?? throw new InvalidOperationException("The native process requires its isolated MySQL ownership descriptor.");
		private static uint ExpectedPort => uint.TryParse(Environment.GetEnvironmentVariable("FUTUREMUD_OWNED_MYSQL_PORT"), out var port) && port > 0
			? port : throw new InvalidOperationException("The native process requires its isolated MySQL port.");
		private static string ExpectedData => Environment.GetEnvironmentVariable("FUTUREMUD_OWNED_MYSQL_DATADIR")
			?? throw new InvalidOperationException("The native process requires its isolated MySQL data directory.");

		internal static void Install()
		{
			_ = ExpectedUuid; _ = ExpectedPort; _ = ExpectedData;
			FMDB.ConnectionValidator = (boundary, connection) => Validate("FMDB-" + boundary, connection, requireActive: true);
			Console.WriteLine($"ownedDb=descriptor-present boundary=process-start pid:{Environment.ProcessId} endpoint:127.0.0.1:{ExpectedPort}");
		}

		internal static void Register(string name, string token, bool ready)
		{
			if (!TestDatabase.HasOwnedPrefix(name) || token.Length != 40 || !token.All(Uri.IsHexDigit))
				throw new InvalidOperationException("A native database needs an exact harness name and ownership marker.");
			Databases[name] = new(token, ready);
		}

		internal static void CheckRefusals()
		{
			foreach (var supplied in new[] {
				$"Server=localhost;Port={ExpectedPort};Database=futuremud_land_unknown",
				$"Server=127.0.0.1;Port={ExpectedPort};Database=shared_default",
				$"Server=127.0.0.1;Port={ExpectedPort};Database=",
				$"Server=127.0.0.1;Port={ExpectedPort};Database=futuremud_land_unregistered"
			})
			{
				using var candidate = new MySqlConnector.MySqlConnection(supplied);
				var refused = false;
				try { Validate("negative-preconnect", candidate); }
				catch (InvalidOperationException) { refused = true; }
				Require(refused && candidate.State == ConnectionState.Closed, "An unowned/default native endpoint passed admission.");
			}
			Console.WriteLine("ARM03B2B-connection-refusals=passed unowned-host shared-default blank-database unregistered-owned-prefix all-refused-before-connect");
		}

		internal static void SetActive(string connectionString)
		{
			using var candidate = new MySqlConnector.MySqlConnection(connectionString);
			Validate("service-configuration", candidate);
			_activeDatabase = new MySqlConnector.MySqlConnectionStringBuilder(connectionString).Database;
			// Globals cannot silently redirect a previously opened ambient/default session.
			if (FMDB.Connection is { } connection) Validate("FMDB-service-existing-explicit", connection, requireActive: true);
			if (FMDB.Context is { } context) Validate("FMDB-service-existing-context", context.Database.GetDbConnection(), requireActive: true);
		}

		internal static void ValidateIndependentConnection(string boundary, DbConnection connection, string ownedConnectionString)
		{
			if (!string.IsNullOrEmpty(new MySqlConnector.MySqlConnectionStringBuilder(connection.ConnectionString).Database))
			{
				Validate("independent-" + boundary, connection);
				return;
			}
			// Pomelo 9's Exists opens a server-only clone and executes USE for the original
			// context's database. This allowance is bound to that already admitted exact target;
			// gameplay FMDB sessions and explicit blank context requests still refuse.
			using var target = new MySqlConnector.MySqlConnection(ownedConnectionString);
			Validate("provider-metadata-target", target);
			Validate("provider-metadata-" + boundary, connection, allowServer: true);
			if (connection.State != ConnectionState.Open) return;
			var database = new MySqlConnector.MySqlConnectionStringBuilder(ownedConnectionString).Database;
			using var marker = connection.CreateCommand();
			marker.CommandText = $"SELECT RunToken FROM `{database.Replace("`", "``")}`.`{OwnershipTable}`";
			using var reader = marker.ExecuteReader();
			if (!reader.Read() || reader.GetString(0) != Databases[database].Token || reader.Read())
				throw new InvalidOperationException("Provider metadata refused an unowned target database.");
			Console.WriteLine($"ownedDb=verified boundary=provider-metadata-target-marker pid:{Environment.ProcessId} database:{database} exact-target=true");
		}

		internal static void Validate(string boundary, DbConnection connection, bool allowServer = false,
			bool bootstrap = false, bool requireActive = false)
		{
			var builder = new MySqlConnector.MySqlConnectionStringBuilder(connection.ConnectionString);
			if (builder.Server != "127.0.0.1" || builder.Port != ExpectedPort)
				throw new InvalidOperationException("Native database boundary refused an endpoint outside the owned loopback server.");
			var database = builder.Database;
			if (string.IsNullOrEmpty(database))
			{
				if (!allowServer) throw new InvalidOperationException("Native services refuse a blank/default database.");
			}
			else if (!TestDatabase.HasOwnedPrefix(database) || !Databases.TryGetValue(database, out var registration) ||
				!bootstrap && !registration.Ready || requireActive && database != _activeDatabase)
			{
				throw new InvalidOperationException("Native database boundary refused an unregistered, unfinished or stale database.");
			}

			if (connection.State == ConnectionState.Open)
			{
				if (connection.Database != database)
					throw new InvalidOperationException("The live native connection changed its selected database.");
				if (!VerifiedOpenConnections.TryGetValue(connection, out _))
				{
					using (var command = connection.CreateCommand())
					{
						command.CommandText = "SELECT @@server_uuid, @@datadir, @@port, DATABASE()";
						using var reader = command.ExecuteReader();
						if (!reader.Read() || reader.GetString(0) != ExpectedUuid || Convert.ToUInt32(reader.GetValue(2)) != ExpectedPort ||
							NormalizeDirectory(reader.GetString(1)) != NormalizeDirectory(ExpectedData) ||
							(reader.IsDBNull(3) ? "" : reader.GetString(3)) != database)
							throw new InvalidOperationException("The live native connection does not match the owned server/database descriptor.");
					}
					if (!string.IsNullOrEmpty(database) && !bootstrap)
					{
						using var marker = connection.CreateCommand();
						marker.CommandText = $"SELECT RunToken FROM `{OwnershipTable}`";
						using var reader = marker.ExecuteReader();
						if (!reader.Read() || reader.GetString(0) != Databases[database].Token || reader.Read())
							throw new InvalidOperationException("The live native database has no exact ownership marker.");
					}
					VerifiedOpenConnections.Add(connection, new object());
				}
			}
			else VerifiedOpenConnections.Remove(connection);
			var fingerprint = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(ExpectedUuid)))[..12];
			Console.WriteLine($"ownedDb=verified boundary={boundary} pid:{Environment.ProcessId} endpoint:127.0.0.1:{ExpectedPort} database:{(database.Length == 0 ? "server-only" : database)} state:{connection.State} server:{fingerprint} marker:{(!bootstrap && database.Length > 0 ? "required" : "bootstrap")}");
		}

		internal static void RegisterReaderDatabase(DbConnection connection, string name)
		{
			// This connection may only reach the already verified owned server. Registering a reader
			// needs the actual marker; a prefix or descriptor filename alone supplies no DB authority.
			Register(name, new string('0', 40), ready: false);
			Validate("reader-before-connect", connection, bootstrap: true);
			connection.Open();
			Validate("reader-opened-server", connection, bootstrap: true);
			using var command = connection.CreateCommand(); command.CommandText = $"SELECT RunToken FROM `{OwnershipTable}`";
			using var reader = command.ExecuteReader();
			if (!reader.Read()) throw new InvalidOperationException("The native reader refuses a database without an ownership marker.");
			var token = reader.GetString(0);
			if (reader.Read()) throw new InvalidOperationException("The native reader refuses ambiguous database ownership.");
			Register(name, token, ready: true);
			VerifiedOpenConnections.Remove(connection);
		}

		private static string NormalizeDirectory(string path) => Path.GetFullPath(path.Replace('/', '\\')).TrimEnd('\\').ToUpperInvariant();
	}
}
