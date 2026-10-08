using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using MySqlConnector;

namespace FutureMUD.RoomSpatialAcceptance;

internal sealed class OwnedWorldAdmission
{
	public string ConnectionString { get; }
	private readonly string _uuid;
	private readonly string _directory;
	private readonly string _token;
	private readonly MySqlConnectionStringBuilder _target;
	private static string Required(string key) => Environment.GetEnvironmentVariable(key)
		?? throw new InvalidOperationException("Missing private acceptance descriptor: " + key);
	private static string Normalize(string path) => Path.GetFullPath(path.Replace('/', Path.DirectorySeparatorChar)).TrimEnd(Path.DirectorySeparatorChar);
	private OwnedWorldAdmission()
	{
		var ready = Path.GetFullPath(Required("FUTUREMUD_CELL_FULLBOOT_READY_PATH"));
		var readyToken = Required("FUTUREMUD_CELL_FULLBOOT_READY_TOKEN");
		if (Path.GetDirectoryName(ready) != Environment.CurrentDirectory || !Path.GetFileName(ready).StartsWith(".acceptance-ready-", StringComparison.Ordinal))
			throw new InvalidOperationException("Invalid private parent attachment receipt.");
		var deadline = DateTime.UtcNow.AddSeconds(30);
		var attached = false;
		while (!attached && DateTime.UtcNow < deadline)
		{
			try { attached = File.Exists(ready) && File.ReadAllText(ready) == readyToken + ":" + Environment.ProcessId; }
			catch (IOException) { }
			if (!attached) Thread.Sleep(25);
		}
		if (!attached)
			throw new InvalidOperationException("Parent job attachment was not confirmed before owned SQL/server startup.");
		ConnectionString = Required("FUTUREMUD_CELL_FULLBOOT_CONNECTION");
		_target = new(ConnectionString);
		_uuid = Required("FUTUREMUD_OWNED_MYSQL_UUID");
		_directory = Normalize(Required("FUTUREMUD_OWNED_MYSQL_DATADIR"));
		_token = Required("FUTUREMUD_CELL_FULLBOOT_TOKEN");
		if (_target.Server != "127.0.0.1" || _target.Port != uint.Parse(Required("FUTUREMUD_OWNED_MYSQL_PORT")) ||
			!Regex.IsMatch(_target.Database, @"^futuremud_land_\d{14}_[a-f0-9]+$") || !Regex.IsMatch(_token, "^[a-f0-9]{40}$"))
			throw new InvalidOperationException("Acceptance requires the explicit exact owned loopback target.");
	}
	public static OwnedWorldAdmission FromEnvironment() => new();
	public void VerifyTarget()
	{
		using var connection = new MySqlConnection(ConnectionString);
		Validate("before-connect", connection);
		connection.Open();
		Validate("connected", connection);
	}
	public void Validate(string boundary, DbConnection connection)
	{
		var candidate = new MySqlConnectionStringBuilder(connection.ConnectionString);
		if (candidate.Server != _target.Server || candidate.Port != _target.Port || candidate.Database != _target.Database)
			throw new InvalidOperationException("Acceptance refused an unexpected connection at " + boundary);
		if (connection.State != ConnectionState.Open) return;
		using (var command = connection.CreateCommand())
		{
			command.CommandText = "SELECT @@server_uuid, @@port, @@datadir, DATABASE()";
			using var reader = command.ExecuteReader();
			if (!reader.Read() || reader.GetString(0) != _uuid || Convert.ToUInt32(reader.GetValue(1)) != _target.Port ||
				!string.Equals(Normalize(reader.GetString(2)), _directory, StringComparison.OrdinalIgnoreCase) || reader.GetString(3) != _target.Database)
				throw new InvalidOperationException("Acceptance refused a mismatched live server descriptor.");
		}
		using var marker = connection.CreateCommand();
		marker.CommandText = "SELECT RunToken FROM __gathering_harness_ownership";
		using var rows = marker.ExecuteReader();
		if (!rows.Read() || rows.GetString(0) != _token || rows.Read()) throw new InvalidOperationException("Acceptance ownership marker mismatch.");
	}
}
