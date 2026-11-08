using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MudSharp.Database;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace DatabaseSeeder;

internal class Program
{
    private static readonly IDatabaseUpgradeCoordinator UpgradeCoordinator = new DatabaseUpgradeCoordinator();
    private static string? ConnectionString { get; set; }
    private static void Main(string[] args)
    {
        Version version = typeof(Program).Assembly.GetName().Version ?? new Version(1, 0, 0);
        if (args.Any(x => x.Equals("--refresh-blank-snapshot", StringComparison.OrdinalIgnoreCase)))
        {
            RefreshBlankDatabaseSnapshot(version);
            return;
        }

        if (args.Length > 0)
        {
            Console.Error.WriteLine("Run Database Seeder without arguments for interactive setup. Item catalogue commands moved to Item Seeder. Environmental exposure capture is available in the Debug SeederCatalogueTools project.");
            Environment.ExitCode = 1;
            return;
        }

        string user = "", database = "";

        Console.ForegroundColor = ConsoleColor.Magenta;

        Console.WriteLine(@"================================================================================");

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
			 ______     _                 ___  ____   _______ 
			 |  ___|   | |                |  \/  | | | |  _  \
			 | |_ _   _| |_ _   _ _ __ ___| .  . | | | | | | |
			 |  _| | | | __| | | | '__/ _ \ |\/| | | | | | | |
			 | | | |_| | |_| |_| | | |  __/ |  | | |_| | |/ / 
			 \_|  \__,_|\__|\__,_|_|  \___\_|  |_/\___/|___/  ");

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(@"                                                 
================================================================================");
        Console.ForegroundColor = ConsoleColor.White;
        @$"
Welcome to the FutureMUD Database Seeder #2v{version.Major}.{version.Minor}.{version.Build}#F

This tool will import pre-generated data into your FutureMUD database. It is 
designed to be used when the MUD is newly installed.

From time to time new updates to the seeder may bring content for those who have 
already got an established game, in this case these packages will be clearly 
marked. You should backup your database before using this tool in that scenario 
just in case.

Please press enter to begin.".WriteLineConsole();
        Console.ReadLine();
        SeederConsole.SafeClear();
#if DEBUG
		while (true)
		{
			Console.Write("Please enter the name of the local Debug database to seed: ");
			string? databaseName = Console.ReadLine();
			if (databaseName is null)
			{
				return;
			}

			if (DebugSeederConnection.TryCreateConnectionString(databaseName, out string connectionString,
					out string error))
			{
				ConnectionString = connectionString;
				break;
			}

			Console.ForegroundColor = ConsoleColor.Red;
			ConsoleLayoutHelper.WriteWrapped(error);
			Console.ForegroundColor = ConsoleColor.White;
		}
#else
		Console.WriteLine("Please enter the connection string for your database: ");
		Console.Write("This is very likely to be in the following format: ");
		Console.ForegroundColor = ConsoleColor.DarkYellow;
		Console.Write("server=localhost;port=3306;database=YOURMUDDB;uid=YOURUSERNAME;password=YOURPASSWORD;Default Command Timeout=300000");
		Console.WriteLine();
		Console.ResetColor();
		Console.WriteLine();
		Console.Write("> ");
		ConnectionString = Console.ReadLine();
#endif
        try
        {
            string installationDirectory = AppContext.BaseDirectory.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);
            using StreamWriter config =
                new(new FileStream(Path.Combine(installationDirectory, "Connection.config"), FileMode.Create,
                    FileAccess.Write));
            config.WriteLine("0.0.0.0");
            config.WriteLine("4000");
			config.WriteLine("127.0.0.1,::1");

            StartupScriptGenerationResult scriptResult = StartupScriptGenerator.EnsureStartScript(
                installationDirectory,
                ConnectionString ?? string.Empty,
                OperatingSystem.IsWindows());
            if (scriptResult == StartupScriptGenerationResult.PreservedCustom)
            {
                Console.WriteLine(
                    "An existing custom startup script was preserved. Remove or rename it before rerunning the seeder to replace it with the current generated launcher.");
            }

            if (OperatingSystem.IsWindows())
            {
                #region Backup-MUD.bat
                Directory.CreateDirectory(Path.Combine(installationDirectory, "Backups"));
                Regex regex = new(@"(?<option>[^;=]+)=(?<value>[^;=]+)");
                foreach (Match match in regex.Matches(ConnectionString ?? string.Empty))
                {
                    switch (match.Groups["option"].Value.ToLowerInvariant())
                    {
                        case "database":
                            database = match.Groups["value"].Value;
                            break;
                        case "uid":
                            user = match.Groups["value"].Value;
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(database))
                {
                    FileStream bfs = new(Path.Combine(installationDirectory, "Backup-MUD.bat"), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    using StreamReader breader = new(bfs);
                    if (breader.ReadToEnd().Length == 0)
                    {
                        using StreamWriter shortcut = new(bfs);

                        shortcut.Write(@$"@echo off
set CUR_YYYY=%date:~10,4%
set CUR_MM=%date:~4,2%
set CUR_DD=%date:~7,2%
set CUR_HH=%time:~0,2%
if %CUR_HH% lss 10 (set CUR_HH=0%time:~1,1%)

set CUR_NN=%time:~3,2%
set CUR_SS=%time:~6,2%
set CUR_MS=%time:~9,2%

SET backupdir={Path.Combine(installationDirectory, "Backups")}
SET mysqluername={user}
SET database={database}

""C:\Program Files\MySQL\MySQL Workbench 8.0\mysqldump.exe"" -u%mysqluername% -p %database% > %backupdir%\%database%_%CUR_YYYY%%CUR_MM%%CUR_DD%-%CUR_HH%%CUR_NN%%CUR_SS%.sql");
                    }
                #endregion
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Warning: could not create FutureMUD support files: {e.Message}");
        }

#if !DEBUG
		try
		{
#endif

        EnsureDatabaseCreated();
        ShowMainMenu();
#if !DEBUG
		}
		catch (Exception e)
		{
			using var writer =
				new StreamWriter(new FileStream($"Database Seeder Exception {typeof(Program).Assembly.GetName().Version} {DateTime.UtcNow:yyyyMMddHHmmss}.txt", FileMode.OpenOrCreate, FileAccess.Write));
			writer.Write(e.ToString());
			writer.Close();

			SeederConsole.SafeClear();
			Console.WriteLine(@$"Unfortunately, the database seeder has crashed. It will have written out a crash log to the directory from which you ran the program. It would be much appreciated if you could pass this crash log, along with some contextual information about what you were doing to Japheth on the Discord server.
				
The exception details were as follows:

{e}");
			Console.ReadLine();
		}
#endif
    }

    private static void ShowMainMenu()
    {
		using (var context = SeederConsole.CreateContext(ConnectionString!))
		{
			if (SettingSeederIntegration.CompatibilityIssue(context) is { } issue)
			{
				Console.Error.WriteLine(issue);
				return;
			}
		}
        new SeederConsole(typeof(Seeders.CoreDataSeeder).Assembly, ConnectionString!
#if DEBUG
			, DebugSeederReplayProfiles.All, SeederReplayRunner.Runner,
			"Debug replay profiles run a complete known seeder sequence against a freshly migrated, unseeded local database. They use the fixed Debug implementor password DebugReplayOnly!2026. Never use this workflow or credential for a reachable or production database."
#endif
		).ShowMainMenu();
    }

    private static void RefreshBlankDatabaseSnapshot(Version version)
    {
        string assetDirectory = BlankDatabaseSnapshotManifest.FindProjectAssetDirectory(AppContext.BaseDirectory);
        string? snapshotRefreshConnectionString =
            Environment.GetEnvironmentVariable("FUTUREMUD_SNAPSHOT_CONNECTION_STRING");
        BlankDatabaseSnapshotManifest manifest = BlankDatabaseSnapshotManifest.Refresh(
            assetDirectory,
            UpgradeCoordinator,
            version,
            snapshotRefreshConnectionString);
        Console.WriteLine($"Blank database snapshot refreshed at {BlankDatabaseSnapshotManifest.GetSnapshotPath(assetDirectory)}");
        Console.WriteLine($"Manifest refreshed at {BlankDatabaseSnapshotManifest.GetManifestPath(assetDirectory)}");
        Console.WriteLine($"Latest migration recorded: {manifest.LatestMigrationId}");
    }

    private static void EnsureDatabaseCreated()
    {
        Console.WriteLine("Ensuring that Database migrations are applied...");
        DatabaseBackupSettings.Load(AppContext.BaseDirectory);
        bool databaseLooksBlank = UpgradeCoordinator.DatabaseLooksBlank(ConnectionString!);
        string? latestMigrationId = UpgradeCoordinator.GetLatestMigrationId(ConnectionString!);
        BlankDatabaseSnapshotAssessment snapshotAssessment = BlankDatabaseSnapshotManager.AssessEmbedded(latestMigrationId);
        DatabaseBootstrapMode bootstrapMode = BlankDatabaseSnapshotManager.SelectBootstrapMode(databaseLooksBlank, snapshotAssessment);

        switch (bootstrapMode)
        {
            case DatabaseBootstrapMode.SnapshotImport:
                Console.WriteLine("Blank database detected. Attempting fast-forward snapshot import...");
                if (TryImportBlankDatabaseSnapshot(snapshotAssessment, latestMigrationId))
                {
                    Console.WriteLine("Database is up to date.");
                    Thread.Sleep(2500);
                    return;
                }

                ApplyFreshDatabaseMigrations();
                return;
            case DatabaseBootstrapMode.FreshMigration:
                Console.WriteLine(snapshotAssessment.Reason);
                Console.WriteLine("Falling back to a full EF migration run for this blank database.");
                ApplyFreshDatabaseMigrations();
                return;
            case DatabaseBootstrapMode.GuardedMigration:
                ApplyGuardedDatabaseMigrations();
                return;
            default:
                throw new InvalidOperationException($"Unknown bootstrap mode {bootstrapMode}.");
        }
    }

    private static bool TryImportBlankDatabaseSnapshot(BlankDatabaseSnapshotAssessment snapshotAssessment,
        string? latestMigrationId)
    {
        try
        {
			if (snapshotAssessment.UsesEmbeddedSnapshot)
			{
				using var snapshot = BlankDatabaseSnapshotManifest.OpenEmbeddedSnapshot();
				UpgradeCoordinator.ImportBlankDatabaseSnapshot(ConnectionString!, snapshot, BlankDatabaseSnapshotManifest.DatabaseNamePlaceholder);
			}
			else
			{
            UpgradeCoordinator.ImportBlankDatabaseSnapshot(
                ConnectionString!,
                snapshotAssessment.SnapshotPath,
                BlankDatabaseSnapshotManifest.DatabaseNamePlaceholder);
			}
        }
        catch (Exception e)
        {
            LogSnapshotImportFailure(e);
            ResetBlankDatabaseOrThrow(e, "importing the blank database snapshot");
            Console.WriteLine("Reset the blank database after the snapshot import failed. Falling back to EF migrations.");
            return false;
        }

        if (VerifyLatestMigrationApplied(latestMigrationId))
        {
            Console.WriteLine($"Blank database snapshot import succeeded at migration {latestMigrationId}.");
            return true;
        }

        ResetBlankDatabaseOrThrow(
            new InvalidOperationException(
                $"Snapshot import completed but did not register migration {latestMigrationId} in __EFMigrationsHistory."),
            "verifying the imported blank database snapshot");
        Console.WriteLine("Reset the blank database after the snapshot verification failed. Falling back to EF migrations.");
        return false;
    }

    private static void LogSnapshotImportFailure(Exception exception)
    {
        string logPath = Path.Combine(
            AppContext.BaseDirectory,
            $"Blank Database Snapshot Failure {DateTime.UtcNow:yyyyMMddHHmmss}.txt");
        File.WriteAllText(logPath, exception.ToString());
        Console.WriteLine($"Blank database snapshot import failed. Details were written to {Path.GetFileName(logPath)}.");
    }

    private static bool VerifyLatestMigrationApplied(string? latestMigrationId)
    {
        if (string.IsNullOrWhiteSpace(latestMigrationId))
        {
            return false;
        }

        using FuturemudDatabaseContext context = CreateContext();
        return context.Database.GetAppliedMigrations().Contains(latestMigrationId, StringComparer.Ordinal);
    }

    private static void ResetBlankDatabaseOrThrow(Exception originalException, string activityDescription)
    {
        try
        {
            UpgradeCoordinator.RecreateEmptyDatabase(ConnectionString!);
        }
        catch (Exception resetException)
        {
            throw new ApplicationException(
                $"Encountered an exception while {activityDescription} and then could not reset the blank database for fallback migrations.",
                new AggregateException(originalException, resetException));
        }
    }

    private static void ApplyFreshDatabaseMigrations()
    {
        using FuturemudDatabaseContext context = CreateContext();
        IMigrator migrator = context.GetService<IMigrator>();
        List<string> migrations = context.Database.GetPendingMigrations().ToList();

        int i = 1;
        foreach (string migration in migrations)
        {
            $"...Applying migration #2{i++}#F of #2{migrations.Count}#F: #E{migration}#F".WriteLineConsole();
            try
            {
                migrator.Migrate(migration);
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Encountered an exception while applying the {migration} migration", e);
            }
        }

        Console.WriteLine("Database is up to date.");
        Thread.Sleep(2500);
    }

    private static void ApplyGuardedDatabaseMigrations()
    {
        DatabaseUpgradePreparation preparation = UpgradeCoordinator.PrepareForStartup(new DatabaseUpgradeRequest
        {
            ConnectionString = ConnectionString!,
            WorkingDirectory = AppContext.BaseDirectory,
            ExecutableType = "DatabaseSeeder"
        });

        if (preparation.RestoredPreviousFailedUpgrade)
        {
            Console.WriteLine(
                "Recovered the previous failed database upgrade from its pre-migration backup before continuing.");
        }

        try
        {
            UpgradeCoordinator.ApplyPreparedMigrations(preparation, progress =>
            {
                $"...Applying migration #2{progress.CurrentMigrationNumber}#F of #2{progress.TotalMigrations}#F: #E{progress.MigrationName}#F"
                    .WriteLineConsole();
            });
            UpgradeCoordinator.CompletePreparedUpgrade(preparation);
        }
        catch (Exception e)
        {
            try
            {
                UpgradeCoordinator.RollbackPreparedUpgrade(preparation, e);
            }
            catch (Exception rollbackException)
            {
                throw new ApplicationException(
                    "Encountered an exception while applying startup migrations and then failed to restore the pre-upgrade backup.",
                    new AggregateException(e, rollbackException));
            }

            throw new ApplicationException(
                "Encountered an exception while applying startup migrations. The database has been restored to the last pre-upgrade backup.",
                e);
        }

        Console.WriteLine("Database is up to date.");
        Thread.Sleep(2500);
    }

    private static FuturemudDatabaseContext CreateContext(bool useLazyLoading = false) =>
        SeederConsole.CreateContext(ConnectionString!, useLazyLoading);
}
