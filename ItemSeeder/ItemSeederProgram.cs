#nullable enable

using System;
using System.IO;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder;

internal static class ItemSeederProgram
{
	private static int Main(string[] args)
	{
		try
		{
			if (CatalogueCaptureCommand.TryHandle(args) || CatalogueInventoryExporter.TryHandle(args) ||
			    IndustrialisedCatalogueAudit.TryHandleCommand(args) || TryCaptureItemManifest(args) ||
			    ItemSeederManifestCatalogue.TryHandleCommand(args))
			{
				return Environment.ExitCode;
			}

			if (args.Length > 0)
			{
				Console.Error.WriteLine("Unknown Item Seeder command. Run without arguments to open the package menu.");
				return 1;
			}

			Console.WriteLine($"FutureMUD Item Seeder v{typeof(ItemSeederProgram).Assembly.GetName().Version?.ToString(3)}");
			Console.WriteLine("Install or update optional item stock in an existing FutureMUD world.");
#if DEBUG
			Console.Write("Local Debug database name: ");
			if (!DebugSeederConnection.TryCreateConnectionString(Console.ReadLine(), out var connectionString, out var error))
			{
				Console.Error.WriteLine(error);
				return 1;
			}
#else
			Console.Write("Connection string for the prepared database: ");
			var connectionString = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(connectionString)) return 1;
#endif
			using (var context = SeederConsole.CreateContext(connectionString))
			{
				if (!context.Database.CanConnect() ||
				    !context.Database.GetMigrations().SequenceEqual(context.Database.GetAppliedMigrations(), StringComparer.Ordinal))
				{
					Console.Error.WriteLine("This database is missing or its schema differs from this Item Seeder version. Prepare it with the matching Database Seeder, then run Item Seeder again.");
					return 1;
				}
			}

			new SeederConsole(typeof(ItemSeederProgram).Assembly, connectionString
#if DEBUG
				, ItemSeederReplayProfiles.All, ItemSeederReplayProfiles.Runner,
				"Debug item replay applies the selected era profile to this prepared local development world. Run Database Seeder first."
#endif
			).ShowMainMenu();
			return 0;
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine($"Item Seeder failed: {exception.Message}");
			return 1;
		}
	}

	private static bool TryCaptureItemManifest(string[] args)
	{
		var captureIndex = Array.FindIndex(args,
			x => x.Equals("--capture-item-manifest", StringComparison.OrdinalIgnoreCase));
		var captureInMemory = args.Any(x =>
			x.Equals("--capture-item-manifest-in-memory", StringComparison.OrdinalIgnoreCase));
		if (captureIndex < 0 && !captureInMemory)
		{
			return false;
		}

		string? connectionString = null;
		if (!captureInMemory)
		{
			if (captureIndex + 1 < args.Length &&
			    !args[captureIndex + 1].StartsWith("--", StringComparison.Ordinal))
			{
				throw new InvalidOperationException(
					"Do not pass database credentials on the command line. Set FUTUREMUD_ITEM_MANIFEST_CONNECTION_STRING for --capture-item-manifest.");
			}

			connectionString = Environment.GetEnvironmentVariable("FUTUREMUD_ITEM_MANIFEST_CONNECTION_STRING");
#if DEBUG
			connectionString ??=
				DebugSeederConnection.DefaultConnectionString;
#endif
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				throw new InvalidOperationException(
					"--capture-item-manifest requires FUTUREMUD_ITEM_MANIFEST_CONNECTION_STRING.");
			}
		}

		var repositoryRoot = ItemSeederManifestCatalogue.FindRepositoryRoot();
		var outputPath = Path.Combine(repositoryRoot,
			ItemSeederManifestCatalogue.DefaultRelativePath.Replace('/', Path.DirectorySeparatorChar));
		using var context = captureInMemory ? CreateItemManifestCaptureContext() : SeederConsole.CreateContext(connectionString!);
		var document = new Seeders.ItemSeeder().CaptureManifest(context, repositoryRoot);
		Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
		File.WriteAllText(outputPath, ItemSeederManifestCatalogue.Serialize(document));
		Console.WriteLine($"Captured {document.Entries.Count:N0} ItemSeeder aggregates to {outputPath}.");
		return true;
	}

	internal static FuturemudDatabaseContext CreateItemManifestCaptureContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase($"ItemSeederManifestCapture-{Guid.NewGuid():N}")
			.Options;
		var context = new FuturemudDatabaseContext(options);
		context.Accounts.Add(new Account
		{
			Id = 1,
			Name = "ItemSeeder Manifest Capture",
			CultureName = "en-AU",
			TimeZoneId = "UTC",
			UnitPreference = "Metric"
		});
		context.TraitDefinitions.Add(new TraitDefinition
		{
			Id = 1,
			Name = "Crafting",
			Type = 0,
			OwnerScope = 0,
			TraitGroup = "Crafting",
			ChargenBlurb = string.Empty,
			ValueExpression = string.Empty
		});
		context.SaveChanges();
		return context;
	}

}
