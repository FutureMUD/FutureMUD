using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MudSharp.Database;
using MudSharp.Framework;

namespace DatabaseSeeder;

internal class Program
{
	private static string? ConnectionString { get; set; }

	private static void Main(string[] args)
	{
		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			Console.WindowWidth = (int)(Console.LargestWindowWidth * 0.75);
			Console.WindowHeight = (int)(Console.LargestWindowHeight * 0.75);
		}
		string password = "", user = "", database = "";
		var version = Assembly.GetCallingAssembly().GetName().Version ?? new Version(1, 0, 0);

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
			"server=localhost;port=3306;database=demo_dbo;uid=futuremud;password=rpiengine2020;Default Command Timeout=300000;";
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
			using var config =
				new StreamWriter(new FileStream("Connection.config", FileMode.OpenOrCreate, FileAccess.Write));
			config.WriteLine("127.0.0.1");
			config.WriteLine("4000");
			config.Close();

			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				Directory.CreateDirectory(Path.GetDirectoryName(
					Assembly.GetEntryAssembly()!.Location) + "\\Binaries");

				#region Start-MUD.bat
				var fs = new FileStream("Start-MUD.bat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
				using var reader = new StreamReader(fs);
				if (reader.ReadToEnd().Length == 0)
				{
					using var shortcut = new StreamWriter(fs);

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
				var regex = new Regex(@"(?<option>[^;=]+)=(?<value>[^;=]+)");
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
					var bfs = new FileStream("Backup-MUD.bat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
					using var breader = new StreamReader(bfs);
					if (breader.ReadToEnd().Length == 0)
					{
						using var shortcut = new StreamWriter(bfs);

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
				var sfs = new FileStream("Start-MUD.sh", FileMode.OpenOrCreate, FileAccess.ReadWrite);
				using var sreader = new StreamReader(sfs);
				if (sreader.ReadToEnd().Length == 0)
				{
					using var shortcut =
						new StreamWriter(sfs);
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
		catch (Exception e)
		{
		}

#if DEBUG
#else
			try
			{
#endif

		EnsureDatabaseCreated(database);
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

	private static void EnsureDatabaseCreated(string database)
	{
		
		Console.WriteLine("Ensuring that Database migrations are applied...");
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString)).Options);
		var migrator = context.GetService<IMigrator>();
		var migrations = context.Database.GetPendingMigrations().ToList();

		// Try to create the database via script to save time
		//if (migrations.Count > 0 && migrations[0] == "20200626070704_InitialDatabase")
		//{
			
		//	"No database detected, attempting to create...".WriteLineConsole();
		//	var dbSQL = DatabaseConstant.DatabaseSQL.Replace("demo_dbo", database);
		//	context.Database.ExecuteSqlRaw(dbSQL);
		//	"...created, detecting migrations...".WriteLineConsole();
		//	migrations = context.Database.GetPendingMigrations().ToList();
		//}

		var i = 1;
		foreach (var migration in migrations)
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

	private static void ShowMainMenu()
	{
		var errorMessage = string.Empty;
		while (true)
		{
			Console.Clear();
			Console.WriteLine("Loading seeders...");
			var iType = typeof(IDatabaseSeeder);
			var seeders = Assembly
					.GetExecutingAssembly()
					.GetTypes()
					.Where(x => x.GetInterfaces().Contains(iType))
					.Where(x => !x.IsAbstract)
					.Select(x => Activator.CreateInstance(x))
					.OfType<IDatabaseSeeder>()
					.Where(x => x.Enabled)
					.OrderBy(x => x.SortOrder)
					.ToList()
				;

			if (!string.IsNullOrEmpty(errorMessage))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(errorMessage);
				Console.ForegroundColor = ConsoleColor.White;
			}

			var i = 1;
			using (var context = new FuturemudDatabaseContext(
					   new DbContextOptionsBuilder<FuturemudDatabaseContext>().UseLazyLoadingProxies()
						   .UseMySql(ConnectionString!, ServerVersion.AutoDetect(ConnectionString)).Options))
			{
				seeders.First().ShouldSeedData(context);
				Console.Clear();
				Console.WriteLine("Please enter the number of the package you wish to import, or QUIT to exit: ");
				Console.WriteLine();
				foreach (var seeder in seeders)
				{
					switch (seeder.ShouldSeedData(context))
					{
						case ShouldSeedResult.MayAlreadyBeInstalled:
							Console.ForegroundColor = ConsoleColor.Yellow;
							break;
						case ShouldSeedResult.PrerequisitesNotMet:
							Console.ForegroundColor = ConsoleColor.DarkRed;
							break;
						case ShouldSeedResult.ReadyToInstall:
							Console.ForegroundColor = ConsoleColor.Green;
							break;
						case ShouldSeedResult.ExtraPackagesAvailable:
							Console.ForegroundColor = ConsoleColor.Cyan;
							break;
					}

					Console.WriteLine($"{i++}) [{seeder.Name:20}] {seeder.Tagline}");
					Console.ForegroundColor = ConsoleColor.White;
				}
			}

			Console.WriteLine();
			Console.Write("Your choice: ");
			var choice = Console.ReadLine() ?? string.Empty;
			if (choice.EqualToAny("quit", "q", "exit", "stop")) return;

			var pick = uint.TryParse(choice, out var value)
				? seeders.ElementAtOrDefault((int)value - 1)
				: seeders.FirstOrDefault(x => x.Name.EqualTo(choice)) ??
				  seeders.FirstOrDefault(x => x.Name.StartsWith(choice, StringComparison.OrdinalIgnoreCase));

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
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseLazyLoadingProxies().UseMySql(ConnectionString!, ServerVersion.AutoDetect(ConnectionString)).Options);
		var shouldseed = seeder.ShouldSeedData(context);
		Console.Clear();
		$"Package: #A{seeder.Name}#F\nTagline: #A{seeder.Tagline}\n\n#3{seeder.FullDescription.Wrap(120, "\t")}#F\n"
			.WriteLineConsole();

		switch (shouldseed)
		{
			case ShouldSeedResult.PrerequisitesNotMet:
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(
					"Warning: Package is reporting its prerequisites are not met and that it should not be used on your database. Proceed with caution.");
				Console.ForegroundColor = ConsoleColor.White;
				break;
			case ShouldSeedResult.MayAlreadyBeInstalled:
				if (seeder.SafeToRunMoreThanOnce) break;

				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(
					"Warning: Package is reporting it may already be installed. It is not advised to run this seeder.");
				Console.ForegroundColor = ConsoleColor.White;
				break;
		}

		Console.WriteLine("Do you want to install this package?");
		while (true)
		{
			"Please type #3yes#F or #3no#F: ".WriteLineConsole();
			var answer = Console.ReadLine();
			if (answer.EqualToAny("yes", "y"))
			{
				DoSeederQuestions(context, seeder);
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine("Press any key to return to main menu.");
				Console.ForegroundColor = ConsoleColor.White;
				Console.ReadKey();
				return;
			}

			if (answer.EqualToAny("no", "n", "quit", "abort", "exit")) return;
		}
	}

	private static void DoSeederQuestions(FuturemudDatabaseContext context, IDatabaseSeeder seeder)
	{
		var answers = new DictionaryWithDefault<string, string>();
		var banner = $"Initial Setup questions for the {seeder.Name} seeder";
		var topline =
			$"╔{new string('═', banner.Length + 2)}╗\n║ {banner} ║\n╚{new string('═', banner.Length + 2)}╝\n";
		foreach (var question in seeder.SeederQuestions)
		{
			if (!question.Filter(context, answers)) continue;

			var errorText = "";
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
					Console.WriteLine(errorText);
					Console.WriteLine();
					Console.ForegroundColor = ConsoleColor.White;
					errorText = "";
				}

				question.Question.Wrap(120).WriteLineConsole();
				Console.WriteLine();
				Console.Write("> ");
				var answer = Console.ReadLine();
				if (answer.EqualToAny("quit", "back", "exit")) return;

				var (success, error) = question.Validator(answer, context);
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
		var result = seeder.SeedData(context, answers);
		Console.WriteLine(result);
		var version = (Assembly.GetCallingAssembly().GetName().Version ?? new Version(1, 0, 0)).ToString();
		var now = DateTime.UtcNow;
		foreach (var item in answers)
		{
			context.SeederChoices.Add(new MudSharp.Models.SeederChoice
			{
				Version = version,
				Seeder = seeder.Name,
				Choice = item.Key,
				Answer = item.Value,
				DateTime = now
			});
		}
		context.SaveChanges();
	}
}