#nullable enable
using MySql.Data.MySqlClient;
using System;
using System.IO;

namespace MudSharp.Database;

public sealed class MySqlDatabaseBackupService : IDatabaseBackupService
{
    private const string MigrationHistoryTableName = "__EFMigrationsHistory";

    public string CreateBackup(string connectionString, string backupDirectory)
    {
        Directory.CreateDirectory(backupDirectory);
        MySqlConnectionStringBuilder builder = new(connectionString);
        string fileName = $"{builder.Database}-DB-Backup-{DateTime.UtcNow:yyyyMMddHHmmss}.sql";
        string destination = Path.Combine(backupDirectory, fileName);

        using MySqlConnection connection = new(connectionString);
        using MySqlCommand command = new()
        {
            Connection = connection
        };
        using MySqlBackup backup = new(command);
        connection.Open();
        backup.ExportInfo.AddCreateDatabase = true;
        backup.ExportInfo.AddDropDatabase = true;
        backup.ExportToFile(destination);
        return destination;
    }

    public void RestoreBackup(string connectionString, string backupFilePath)
    {
        string serverConnectionString = GetServerConnectionString(connectionString);
        using MySqlConnection connection = new(serverConnectionString);
        using MySqlCommand command = new()
        {
            Connection = connection
        };
        using MySqlBackup backup = new(command);
        connection.Open();
        backup.ImportFromFile(backupFilePath);
    }

    public bool DatabaseLooksBlank(string connectionString)
    {
        MySqlConnectionStringBuilder builder = new(connectionString);
        string databaseName = builder.Database;
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            return true;
        }

        using MySqlConnection connection = new(GetServerConnectionString(connectionString));
        connection.Open();

        using MySqlCommand existsCommand = connection.CreateCommand();
        existsCommand.CommandText =
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @databaseName;";
        existsCommand.Parameters.AddWithValue("@databaseName", databaseName);
        bool exists = Convert.ToInt32(existsCommand.ExecuteScalar()) > 0;
        if (!exists)
        {
            return true;
        }

        using MySqlCommand tableCommand = connection.CreateCommand();
        tableCommand.CommandText =
            """
			SELECT COUNT(*)
			FROM INFORMATION_SCHEMA.TABLES
			WHERE TABLE_SCHEMA = @databaseName
			AND TABLE_TYPE = 'BASE TABLE';
			""";
        tableCommand.Parameters.AddWithValue("@databaseName", databaseName);
        int tableCount = Convert.ToInt32(tableCommand.ExecuteScalar());
        if (tableCount == 0)
        {
            return true;
        }

        using MySqlCommand historyCommand = connection.CreateCommand();
        historyCommand.CommandText =
            """
			SELECT COUNT(*)
			FROM INFORMATION_SCHEMA.TABLES
			WHERE TABLE_SCHEMA = @databaseName
			AND TABLE_NAME = @historyTable;
			""";
        historyCommand.Parameters.AddWithValue("@databaseName", databaseName);
        historyCommand.Parameters.AddWithValue("@historyTable", MigrationHistoryTableName);
        bool hasHistoryTable = Convert.ToInt32(historyCommand.ExecuteScalar()) > 0;
        return tableCount == 1 && hasHistoryTable;
    }

    public void RecreateEmptyDatabase(string connectionString)
    {
        MySqlConnectionStringBuilder builder = new(connectionString);
        if (string.IsNullOrWhiteSpace(builder.Database))
        {
            throw new InvalidOperationException("Cannot recreate a database when the connection string has no database name.");
        }

        string escapedDatabaseName = builder.Database.Replace("`", "``", StringComparison.Ordinal);
        using MySqlConnection connection = new(GetServerConnectionString(connectionString));
        connection.Open();

        using MySqlCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
			DROP DATABASE IF EXISTS `{escapedDatabaseName}`;
			CREATE DATABASE `{escapedDatabaseName}`;
			""";
        command.ExecuteNonQuery();
    }

    public void ExecuteSqlScript(string connectionString, string scriptContents)
    {
        using MySqlConnection connection = new(GetServerConnectionString(connectionString));
        connection.Open();
        MySqlScript script = new(connection, scriptContents);
        script.Execute();
    }

    private static string GetServerConnectionString(string connectionString)
    {
        MySqlConnectionStringBuilder builder = new(connectionString)
        {
            Database = string.Empty
        };
        return builder.ConnectionString;
    }
}
