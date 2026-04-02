#nullable enable
using System;
using System.IO;
using MySql.Data.MySqlClient;

namespace MudSharp.Database;

public sealed class MySqlDatabaseBackupService : IDatabaseBackupService
{
	private const string MigrationHistoryTableName = "__EFMigrationsHistory";

	public string CreateBackup(string connectionString, string backupDirectory)
	{
		Directory.CreateDirectory(backupDirectory);
		var builder = new MySqlConnectionStringBuilder(connectionString);
		var fileName = $"{builder.Database}-DB-Backup-{DateTime.UtcNow:yyyyMMddHHmmss}.sql";
		var destination = Path.Combine(backupDirectory, fileName);

		using var connection = new MySqlConnection(connectionString);
		using var command = new MySqlCommand
		{
			Connection = connection
		};
		using var backup = new MySqlBackup(command);
		connection.Open();
		backup.ExportInfo.AddCreateDatabase = true;
		backup.ExportInfo.AddDropDatabase = true;
		backup.ExportToFile(destination);
		return destination;
	}

	public void RestoreBackup(string connectionString, string backupFilePath)
	{
		var serverConnectionString = GetServerConnectionString(connectionString);
		using var connection = new MySqlConnection(serverConnectionString);
		using var command = new MySqlCommand
		{
			Connection = connection
		};
		using var backup = new MySqlBackup(command);
		connection.Open();
		backup.ImportFromFile(backupFilePath);
	}

	public bool DatabaseLooksBlank(string connectionString)
	{
		var builder = new MySqlConnectionStringBuilder(connectionString);
		var databaseName = builder.Database;
		if (string.IsNullOrWhiteSpace(databaseName))
		{
			return true;
		}

		using var connection = new MySqlConnection(GetServerConnectionString(connectionString));
		connection.Open();

		using var existsCommand = connection.CreateCommand();
		existsCommand.CommandText =
			"SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @databaseName;";
		existsCommand.Parameters.AddWithValue("@databaseName", databaseName);
		var exists = Convert.ToInt32(existsCommand.ExecuteScalar()) > 0;
		if (!exists)
		{
			return true;
		}

		using var tableCommand = connection.CreateCommand();
		tableCommand.CommandText =
			"""
			SELECT COUNT(*)
			FROM INFORMATION_SCHEMA.TABLES
			WHERE TABLE_SCHEMA = @databaseName
			AND TABLE_TYPE = 'BASE TABLE';
			""";
		tableCommand.Parameters.AddWithValue("@databaseName", databaseName);
		var tableCount = Convert.ToInt32(tableCommand.ExecuteScalar());
		if (tableCount == 0)
		{
			return true;
		}

		using var historyCommand = connection.CreateCommand();
		historyCommand.CommandText =
			"""
			SELECT COUNT(*)
			FROM INFORMATION_SCHEMA.TABLES
			WHERE TABLE_SCHEMA = @databaseName
			AND TABLE_NAME = @historyTable;
			""";
		historyCommand.Parameters.AddWithValue("@databaseName", databaseName);
		historyCommand.Parameters.AddWithValue("@historyTable", MigrationHistoryTableName);
		var hasHistoryTable = Convert.ToInt32(historyCommand.ExecuteScalar()) > 0;
		return tableCount == 1 && hasHistoryTable;
	}

	public void RecreateEmptyDatabase(string connectionString)
	{
		var builder = new MySqlConnectionStringBuilder(connectionString);
		if (string.IsNullOrWhiteSpace(builder.Database))
		{
			throw new InvalidOperationException("Cannot recreate a database when the connection string has no database name.");
		}

		var escapedDatabaseName = builder.Database.Replace("`", "``", StringComparison.Ordinal);
		using var connection = new MySqlConnection(GetServerConnectionString(connectionString));
		connection.Open();

		using var command = connection.CreateCommand();
		command.CommandText =
			$"""
			DROP DATABASE IF EXISTS `{escapedDatabaseName}`;
			CREATE DATABASE `{escapedDatabaseName}`;
			""";
		command.ExecuteNonQuery();
	}

	public void ExecuteSqlScript(string connectionString, string scriptContents)
	{
		using var connection = new MySqlConnection(GetServerConnectionString(connectionString));
		connection.Open();
		var script = new MySqlScript(connection, scriptContents);
		script.Execute();
	}

	private static string GetServerConnectionString(string connectionString)
	{
		var builder = new MySqlConnectionStringBuilder(connectionString)
		{
			Database = string.Empty
		};
		return builder.ConnectionString;
	}
}
