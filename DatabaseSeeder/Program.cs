using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MudSharp.Database;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace DatabaseSeeder;

internal class Program
{
    private static readonly IDatabaseUpgradeCoordinator UpgradeCoordinator = new DatabaseUpgradeCoordinator();
    private static string? ConnectionString { get; set; }

    private static void Main(string[] args)
    {
        Version version = Assembly.GetCallingAssembly().GetName().Version ?? new Version(1, 0, 0);
        if (args.Any(x => x.Equals("--refresh-blank-snapshot", StringComparison.OrdinalIgnoreCase)))
        {
            RefreshBlankDatabaseSnapshot(version);
            return;
        }

        string password = "", user = "", database = "";

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
        Console.Clear();
#if DEBUG
        ConnectionString =
            "server=localhost;port=3307;database=demo_dbo;uid=futuremud;password=rpiengine2020;Default Command Timeout=300000;";
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
            using StreamWriter config =
                new(new FileStream("Connection.config", FileMode.OpenOrCreate, FileAccess.Write));
            config.WriteLine("127.0.0.1");
            config.WriteLine("4000");
            config.Close();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(
                    Assembly.GetEntryAssembly()!.Location) + "\\Binaries");

                #region Start-MUD.bat
                FileStream fs = new("Start-MUD.bat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                using StreamReader reader = new(fs);
                if (reader.ReadToEnd().Length == 0)
                {
                    using StreamWriter shortcut = new(fs);

                    shortcut.Write($@"set MUDDIR={Path.GetDirectoryName(
                        Assembly.GetEntryAssembly()!.Location)}
set CODEDIR=%MUDDIR%\Binaries
set SERVER=localhost
set loopcount=100
cd /D %MUDDIR%
del %MUDDIR%\STOP-REBOOTING
del %MUDDIR%\BOOTING
:loop
xcopy %CODEDIR%\*.exe %MUDDIR%\ /C /Y
xcopy %CODEDIR%\*.dll %MUDDIR%\ /C /Y
xcopy %CODEDIR%\*.pdb %MUDDIR%\ /C /Y
xcopy %CODEDIR%\*.json %MUDDIR%\ /C /Y
%MUDDIR%\MudSharp.exe ""MySql.Data.MySqlClient"" ""{ConnectionString}""
if exist %MUDDIR%\STOP-REBOOTING goto :endloop
if exist %MUDDIR%\BOOTING goto :crashed
echo MUD Crashed - will attempt to reboot %loopcount%0 more times.
set /a loopcount=%loopcount%-1
if %loopcount%==0 goto exitloop
goto loop
:crashed
echo Mud crashed during boot up sequence, will not attempt to restart
goto :exitloop
:endloop
echo Mud was shut down and requested boot loop to end.
:exitloop");
                }
                #endregion

                #region Backup-MUD.bat
                Directory.CreateDirectory(Path.GetDirectoryName(
                    Assembly.GetEntryAssembly()!.Location) + "\\Backups");
                Regex regex = new(@"(?<option>[^;=]+)=(?<value>[^;=]+)");
                foreach (Match match in regex.Matches(ConnectionString))
                {
                    switch (match.Groups["option"].Value.ToLowerInvariant())
                    {
                        case "database":
                            database = match.Groups["value"].Value;
                            break;
                        case "uid":
                            user = match.Groups["value"].Value;
                            break;
                        case "password":
                            password = match.Groups["value"].Value;
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(database))
                {
                    FileStream bfs = new("Backup-MUD.bat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
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

SET backupdir={Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location) + "\\Backups"}
SET mysqluername={user}
SET mysqlpassword={password}
SET database={database}

""C:\Program Files\MySQL\MySQL Workbench 8.0\mysqldump.exe"" -u%mysqluername% -p%mysqlpassword% %database% > %backupdir%\%database%_%CUR_YYYY%%CUR_MM%%CUR_DD%-%CUR_HH%%CUR_NN%%CUR_SS%.sql");
                    }
                #endregion
                }
            }
            else
            {
                FileStream sfs = new("Start-MUD.sh", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                using StreamReader sreader = new(sfs);
                if (sreader.ReadToEnd().Length == 0)
                {
                    using StreamWriter shortcut = new(sfs);
                    shortcut.Write(
                        $@"#!/bin/sh

SERVER_PORT_BASEDIR="".""
cd $SERVER_PORT_BASEDIR
echo ""The working directory is now"" `pwd`
echo ""Starting the game engine. Will attempt 100 restarts.""
rm -r ""$SERVER_PORT_BASEDIR/BOOTING""
rm -r ""$SERVER_PORT_BASEDIR/STOP-REBOOTING""
for i in 'seq 1 100'
do
  $SERVER_PORT_BASEDIR/MudSharp ""MySql.Data.MySqlClient"" ""{ConnectionString}""
  
  if [ -f ""$SERVER_PORT_BASEDIR/BOOTING"" ]
  then
	echo ""Server quit during boot sequence.""
	break;
  fi
  
  if [ -f ""$SERVER_PORT_BASEDIR/STOP-REBOOTING"" ]
  then
	echo ""Server was shut down with a request to end the boot loop.""
	break;
  fi
wait
done

echo ""The game engine has shut down.""");
                }
            }
        }
        catch (Exception)
        {
        }

#if DEBUG
#else
		try
		{
#endif

        EnsureDatabaseCreated();
        ShowMainMenu();
#if DEBUG
#else
		}
		catch (Exception e)
		{
			using var writer =
				new StreamWriter(new FileStream($"Database Seeder Exception {Assembly.GetCallingAssembly().GetName().Version} {DateTime.UtcNow:yyyyMMddHHmmss}.txt", FileMode.OpenOrCreate, FileAccess.Write));
			writer.Write(e.ToString());
			writer.Close();

			Console.Clear();
			Console.WriteLine(@$"Unfortunately, the database seeder has crashed. It will have written out a crash log to the directory from which you ran the program. It would be much appreciated if you could pass this crash log, along with some contextual information about what you were doing to Japheth on the Discord server.
				
The exception details were as follows:

{e}");
			Console.ReadLine();
		}
#endif
    }

    private static void RefreshBlankDatabaseSnapshot(Version version)
    {
        string assetDirectory = BlankDatabaseSnapshotManifest.FindProjectAssetDirectory(AppContext.BaseDirectory);
        BlankDatabaseSnapshotManifest manifest = BlankDatabaseSnapshotManifest.Refresh(assetDirectory, UpgradeCoordinator, version);
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
        BlankDatabaseSnapshotAssessment snapshotAssessment = BlankDatabaseSnapshotManager.Assess(AppContext.BaseDirectory, latestMigrationId);
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
            UpgradeCoordinator.ImportBlankDatabaseSnapshot(
                ConnectionString!,
                snapshotAssessment.SnapshotPath,
                BlankDatabaseSnapshotManifest.DatabaseNamePlaceholder);
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

    private static FuturemudDatabaseContext CreateContext(bool useLazyLoading = false)
    {
        DbContextOptionsBuilder<FuturemudDatabaseContext> builder = new();
        if (useLazyLoading)
        {
            builder.UseLazyLoadingProxies();
        }

        builder.UseMySql(ConnectionString!, ServerVersion.AutoDetect(ConnectionString));
        return new FuturemudDatabaseContext(builder.Options);
    }

    private static void ShowMainMenu()
    {
        string errorMessage = string.Empty;
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Loading seeders...");
            Type iType = typeof(IDatabaseSeeder);
            List<IDatabaseSeeder> seeders = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(x => x.GetInterfaces().Contains(iType))
                    .Where(x => !x.IsAbstract)
                    .Select(x => Activator.CreateInstance(x))
                    .OfType<IDatabaseSeeder>()
                    .Where(x => x.Enabled)
                    .ToList()
                ;

            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ConsoleLayoutHelper.WriteWrapped(errorMessage);
                Console.ForegroundColor = ConsoleColor.White;
            }

            int i = 1;
            List<(IDatabaseSeeder Seeder, SeederAssessment Assessment)> assessedSeeders;
            using (FuturemudDatabaseContext context = CreateContext(useLazyLoading: true))
            {
                assessedSeeders = seeders
                    .Select(seeder => (Seeder: seeder, Assessment: seeder.AssessSeedData(context)))
                    .OrderBy(x => GetMenuSortRank(x.Assessment.Status))
                    .ThenBy(x => x.Seeder.SortOrder)
                    .ThenBy(x => x.Seeder.Name)
                    .ToList();

                Console.Clear();
                ConsoleLayoutHelper.WriteWrapped("Please enter the number of the package you wish to import, or QUIT to exit: ");
                Console.WriteLine();
                foreach ((IDatabaseSeeder Seeder, SeederAssessment Assessment) assessedSeeder in assessedSeeders)
                {
                    Console.ForegroundColor = GetAssessmentColour(assessedSeeder.Assessment.Status);
                    foreach (string line in ConsoleLayoutHelper.FormatMenuEntry(
                                 i++,
                                 assessedSeeder.Seeder.Name,
                                 GetAssessmentLabel(assessedSeeder.Assessment.Status),
                                 assessedSeeder.Seeder.Tagline))
                    {
                        Console.WriteLine(line);
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            Console.WriteLine();
            Console.Write("Your choice: ");
            string choice = Console.ReadLine() ?? string.Empty;
            if (choice.EqualToAny("quit", "q", "exit", "stop"))
            {
                return;
            }

            IDatabaseSeeder? pick = uint.TryParse(choice, out uint value)
                ? assessedSeeders.ElementAtOrDefault((int)value - 1).Seeder
                : assessedSeeders.Select(x => x.Seeder).FirstOrDefault(x => x.Name.EqualTo(choice)) ??
                  assessedSeeders.Select(x => x.Seeder)
                      .FirstOrDefault(x => x.Name.StartsWith(choice, StringComparison.OrdinalIgnoreCase));

            if (pick == null)
            {
                errorMessage = "That is not a valid selection.";
                continue;
            }

            ShowSeeder(pick);
            errorMessage = string.Empty;
        }
    }

    private static void ShowSeeder(IDatabaseSeeder seeder)
    {
        Console.Clear();
        Console.WriteLine("Loading package...");
        using FuturemudDatabaseContext context = CreateContext(useLazyLoading: true);
        SeederAssessment assessment = seeder.AssessSeedData(context);
        Console.Clear();
        $"Package: #A{seeder.Name}#F\nTagline: #A{seeder.Tagline}\n\n#3{seeder.FullDescription.Wrap(90, "\t")}#F\n"
            .WriteLineConsole();

        Console.ForegroundColor = GetAssessmentColour(assessment.Status);
        Console.WriteLine($"Status: {GetAssessmentLabel(assessment.Status)}");
        Console.ForegroundColor = ConsoleColor.White;
        ConsoleLayoutHelper.WriteWrapped(assessment.Explanation);
        Console.WriteLine();

        if (assessment.MissingPrerequisites.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Missing prerequisites:");
            foreach (string prerequisite in assessment.MissingPrerequisites)
            {
                ConsoleLayoutHelper.WriteWrapped($" - {prerequisite}", indent: "   ");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        if (assessment.Warnings.Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (string warning in assessment.Warnings)
            {
                ConsoleLayoutHelper.WriteWrapped($"Warning: {warning}", indent: "         ");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        if (assessment.Notes.Any())
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (string note in assessment.Notes)
            {
                ConsoleLayoutHelper.WriteWrapped(note);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }

        Console.WriteLine("Do you want to install this package?");
        while (true)
        {
            "Please type #3yes#F or #3no#F: ".WriteLineConsole();
            Console.WriteLine();
            string? answer = Console.ReadLine();
            if (answer.EqualToAny("yes", "y"))
            {
                DoSeederQuestions(context, seeder);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Press any key to return to main menu.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();
                return;
            }

            if (answer.EqualToAny("no", "n", "quit", "abort", "exit"))
            {
                return;
            }
        }
    }

    private static void DoSeederQuestions(FuturemudDatabaseContext context, IDatabaseSeeder seeder)
    {
        DictionaryWithDefault<string, string> answers = new();
        List<SeederQuestion> questions = seeder.Questions.ToList();
        string banner = $"Initial Setup questions for the {seeder.Name} seeder";
        string topline =
            $"╔{new string('═', banner.Length + 2)}╗\n║ {banner} ║\n╚{new string('═', banner.Length + 2)}╝\n";
        foreach (SeederQuestion? question in questions)
        {
            if (!question.Filter(context, answers))
            {
                continue;
            }

            string errorText = "";
            string? rememberedAnswer = SeederAnswerMemory.GetRememberedAnswer(context, seeder, question, answers);
            string? resolvedDefaultAnswer = null;
            if (!string.IsNullOrWhiteSpace(rememberedAnswer))
            {
                (bool success, string? error) = question.Validator(rememberedAnswer, context);
                if (success)
                {
                    if (question.AutoReuseLastAnswer)
                    {
                        answers[question.Id] = rememberedAnswer;
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        topline.WriteLineConsole();
                        Console.ForegroundColor = ConsoleColor.White;
                        ConsoleLayoutHelper.WriteWrapped($"Reusing previous answer for {question.Id}: {rememberedAnswer}");
                        Thread.Sleep(1000);
                        continue;
                    }

                    resolvedDefaultAnswer = rememberedAnswer;
                }
                else
                {
                    errorText =
                        $"The previously remembered answer for {question.Id} is no longer valid and will be ignored.{(string.IsNullOrWhiteSpace(error) ? "" : $"\n{error}")}";
                }
            }

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                topline.WriteLineConsole();
                Console.ForegroundColor = ConsoleColor.White;
                if (!string.IsNullOrEmpty(errorText))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    ConsoleLayoutHelper.WriteWrapped(errorText);
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    errorText = "";
                }

                ConsoleQuestionDisplay display = question.ResolveDisplay(context, answers);
                if (!string.IsNullOrWhiteSpace(display.DefaultAnswer) &&
                    string.IsNullOrWhiteSpace(resolvedDefaultAnswer))
                {
                    (bool defaultSuccess, string? defaultError) = question.Validator(display.DefaultAnswer, context);
                    if (defaultSuccess)
                    {
                        resolvedDefaultAnswer = display.DefaultAnswer;
                    }
                    else
                    {
                        errorText =
                            $"The suggested default answer for {question.Id} is no longer valid and will be ignored.{(string.IsNullOrWhiteSpace(defaultError) ? "" : $"\n{defaultError}")}";
                    }
                }

                display.Prompt.Wrap(90).WriteLineConsole();
                if (!string.IsNullOrWhiteSpace(resolvedDefaultAnswer))
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    ConsoleLayoutHelper.WriteWrapped($"Suggested default: {resolvedDefaultAnswer} (press Enter to accept)");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                Console.WriteLine();
                Console.Write("> ");
                string answer = Console.ReadLine() ?? string.Empty;
                if (answer.EqualToAny("quit", "back", "exit"))
                {
                    return;
                }

                answer = ResolveQuestionAnswer(answer, resolvedDefaultAnswer);

                (bool success, string? error) = question.Validator(answer, context);
                if (!success)
                {
                    errorText = error;
                    continue;
                }

                answers[question.Id] = answer;
                break;
            }
        }

        Console.Clear();
        $"Applying the data from the #2{seeder.Name}#F seeder...".WriteLineConsole();
        string result = seeder.SeedData(context, answers);
        Console.WriteLine(result);
        string version = (Assembly.GetCallingAssembly().GetName().Version ?? new Version(1, 0, 0)).ToString();
        DateTime now = DateTime.UtcNow;
        SeederAnswerMemory.PersistAnswers(context, seeder, questions, answers, version, now);
        context.SaveChanges();
    }

    internal static string ResolveQuestionAnswer(string answer, string? defaultAnswer)
    {
        return string.IsNullOrWhiteSpace(answer) && !string.IsNullOrWhiteSpace(defaultAnswer)
            ? defaultAnswer
            : answer;
    }

    private static int GetMenuSortRank(SeederAssessmentStatus status)
    {
        return status switch
        {
            SeederAssessmentStatus.ReadyToInstall => 0,
            SeederAssessmentStatus.UpdateAvailable => 1,
            SeederAssessmentStatus.AdditiveInstallAvailable => 2,
            SeederAssessmentStatus.InstalledCurrent => 3,
            _ => 4
        };
    }

    private static ConsoleColor GetAssessmentColour(SeederAssessmentStatus status)
    {
        return status switch
        {
            SeederAssessmentStatus.Blocked => ConsoleColor.DarkRed,
            SeederAssessmentStatus.ReadyToInstall => ConsoleColor.Green,
            SeederAssessmentStatus.AdditiveInstallAvailable => ConsoleColor.Cyan,
            SeederAssessmentStatus.UpdateAvailable => ConsoleColor.Yellow,
            _ => ConsoleColor.DarkYellow
        };
    }

    private static string GetAssessmentLabel(SeederAssessmentStatus status)
    {
        return status switch
        {
            SeederAssessmentStatus.Blocked => "Blocked",
            SeederAssessmentStatus.ReadyToInstall => "Ready",
            SeederAssessmentStatus.AdditiveInstallAvailable => "Additive",
            SeederAssessmentStatus.UpdateAvailable => "Update",
            _ => "Current"
        };
    }
}
