#nullable enable
using System;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
namespace DatabaseSeeder;
internal static class ArmageddonSeederProgram
{
	internal static System.Collections.Generic.IReadOnlyList<IDatabaseSeeder> GetEnabledSeeders() =>
		SeederPackageDiscovery.GetEnabledSeeders(typeof(ArmageddonSeederProgram).Assembly)
			.Concat(SeederPackageDiscovery.GetEnabledSeeders(typeof(CoreDataSeeder).Assembly)).ToArray();
	private static int Main(string[] args)
	{
		try
		{
			if (args.SequenceEqual(new[] { "--list-packages" }))
			{
				foreach (var package in GetEnabledSeeders()) Console.WriteLine(package.Name);
				return 0;
			}
			if (args.Length != 0)
			{
				Console.WriteLine("FutureMUD Armageddon Seeder: run without arguments to connect to a prepared database, or use --list-packages.");
				return args.SequenceEqual(new[] { "--help" }) ? 0 : 1;
			}
			Console.WriteLine($"FutureMUD Armageddon Seeder v{typeof(ArmageddonSeederProgram).Assembly.GetName().Version?.ToString(3)}");
			Console.WriteLine("Prepare the database schema with the matching Database Seeder first. Foundation packages are available here for Armageddon-specific setup.");
#if DEBUG
			Console.Write("Local Debug database name: ");
			if (!DebugSeederConnection.TryCreateConnectionString(Console.ReadLine(), out var connectionString, out var error))
			{ Console.Error.WriteLine(error); return 1; }
#else
			Console.Write("Connection string for the prepared database: ");
			var connectionString = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(connectionString)) return 1;
#endif
			using (var context = SeederConsole.CreateContext(connectionString))
			{
				if (!context.Database.CanConnect() || !context.Database.GetMigrations().SequenceEqual(context.Database.GetAppliedMigrations(), StringComparer.Ordinal))
				{ Console.Error.WriteLine("The schema does not match this Armageddon Seeder. Prepare it with the matching Database Seeder before continuing."); return 1; }
			}
			ShowMainMenu(connectionString);
			return 0;
		}
		catch (Exception exception) { Console.Error.WriteLine($"Armageddon Seeder failed: {exception.Message}"); return 1; }
	}
	internal static void ShowMainMenu(string connectionString)
	{
		new SeederConsole(typeof(ArmageddonSeederProgram).Assembly, connectionString
#if DEBUG
				, ArmageddonSeederReplayProfiles.All, ArmageddonSeederReplayProfiles.Runner,
				"Debug replay requires a freshly migrated, unseeded local development database. Worldfile import requires the separately supplied private packet."
#endif
				, foundationAssembly: typeof(CoreDataSeeder).Assembly).ShowMainMenu();
	}

}
