#nullable enable
namespace MudSharp.Database;

public interface IDatabaseBackupService
{
    string CreateBackup(string connectionString, string backupDirectory);
    void RestoreBackup(string connectionString, string backupFilePath);
    bool DatabaseLooksBlank(string connectionString);
    void RecreateEmptyDatabase(string connectionString);
    void ExecuteSqlScript(string connectionString, string scriptContents);
}
