using Microsoft.EntityFrameworkCore;
using MoreLinq;
using MudSharp.Database;
using MudSharp.Documentation.Export;
using MudSharp.Email;
using MudSharp.Network;
using System.IO;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;

namespace MudSharp.Server;

internal class MudSharp
{
    private static async Task Main(string[] args)
    {
        if (TryRunDocumentationExport(args))
        {
            return;
        }

		if (TryRunIndustrialisedPrerequisiteAudit(args))
		{
			return;
		}

		if (TryRunGoogleEmailAuthorization(args))
		{
			return;
		}

        ConfigureConsoleHost();

        IPAddress hostIp;
        int tcpPort;
		IReadOnlyCollection<IPAddress> trustedProxyAddresses =
			[IPAddress.Loopback, IPAddress.IPv6Loopback];

        using (FileStream fs = File.Create("BOOTING"))
        {
        }

        using (FileStream fs = File.Create("Engine-Version.info"))
        {
            using (StreamWriter writer = new(fs, Encoding.ASCII))
            {
                writer.WriteLine(Assembly.GetCallingAssembly().GetName().Version!.ToString());
                writer.Flush();
            }
        }

        if (File.Exists("Connection.config"))
        {
            try
            {
				var connectionSettings = File.ReadAllLines("Connection.config");
				hostIp = IPAddress.Parse(connectionSettings[0]);
				tcpPort = Convert.ToInt32(connectionSettings[1]);
				trustedProxyAddresses = connectionSettings.Length >= 3
					? ParseTrustedProxyAddresses(connectionSettings[2])
					: [IPAddress.Loopback, IPAddress.IPv6Loopback];
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "FutureMUD Failed to start up due to being unable to open the connection configuration: " +
                    e.Message);
                return;
            }
        }
        else
        {
            hostIp = IPAddress.Parse("127.0.0.1");
            tcpPort = 4000;
            try
            {
                FileStream fs = new("Connection.config", FileMode.Create, FileAccess.Write);
                StreamWriter writer = new(fs);
                writer.WriteLine("127.0.0.1");
                writer.WriteLine("4000");
				writer.WriteLine("127.0.0.1,::1");
                writer.Close();
				trustedProxyAddresses = [IPAddress.Loopback, IPAddress.IPv6Loopback];
            }
            catch (Exception e)
            {
                Console.WriteLine("[Non-fatal error] Failed to write Connection.config with exception: " + e.Message);
            }
        }

        // Programmatically setting the connection string
        if (args.Length == 2)
        {
            FMDB.Provider = args[0];
            FMDB.ConnectionString = args[1];
        }
        else
        {
#if DEBUG
            FMDB.Provider = "MySql.Data.MySqlClient";
            //FMDB.ConnectionString = "Data Source=(LocalDB)\\v11.0;AttachDbFilename=\"|DataDirectory|\\FuturemudDatabase.mdf\";Integrated Security=True;MultipleActiveResultSets=True;App=EntityFramework";
            FMDB.ConnectionString =
                "server=localhost;port=3306;database=test_dbo;uid=futuremud;password=rpiengine2020;Default Command Timeout=300000;";
#endif
        }
#if DEBUG
#else
		try
		{
#endif

        using (FutureMUDConsoleWriter consoleWriter =
               new($"./Console Log {DateTime.UtcNow:yyyy MMMM dd HH mm ss}.txt"))
        {
            Futuremud mud = new(new TCPServer(
				hostIp,
				tcpPort,
				trustedProxyAddresses: trustedProxyAddresses));

            Thread.CurrentThread.Name = "Main Game Thread";

            ((IFuturemudLoader)mud).LoadFromDatabase();
            TrySetConsoleTitle(mud.Name);

            File.Delete("BOOTING");
            mud.MarkStartupDatabaseUpgradeComplete();
            mud.StartGameLoop();

            // Post Shutdown Sequence

			await Task.Delay(500);
			if (mud.Server is IAsyncServer asyncServer)
			{
				await asyncServer.StopAsync();
			}
			else
			{
				mud.Server.Stop();
			}
            EmailHelper.Instance.EndEmailThread();
        }
#if DEBUG
#else
		}
		catch (DbUpdateException e)
		{
			var version = Assembly.GetCallingAssembly().GetName().Version;
			var sb = new StringBuilder();
			sb.AppendLine($"Crash Log for Engine Version v{version.Major}.{version.Minor}.{version.Build}.{version.Revision.ToString("0000")}");
			sb.AppendLine();
			sb.AppendLine(e.ToString());
			sb.AppendLine();
			sb.AppendLine($"DbUpdateException error details - {e.InnerException?.InnerException?.Message}");

			foreach (var eve in e.Entries)
			{
				sb.AppendLine($"Entity of type {eve.Entity.GetType().Name} in state {eve.State} could not be updated");
			}

			WriteCrashLog(sb.ToString());
		}
		catch (ReflectionTypeLoadException e)
		{
			var version = Assembly.GetCallingAssembly().GetName().Version;
			var sb = new StringBuilder();
			sb.AppendLine($"Crash Log for Engine Version v{version.Major}.{version.Minor}.{version.Build}.{version.Revision.ToString("0000")}");
			sb.AppendLine();
			sb.AppendLine(e.ToString());
			sb.AppendLine();
			sb.AppendLine("Reflection Type Errors:");
			for (var i = 0; i < e.LoaderExceptions.Length; i++)
			{
				sb.AppendLine($"Type: {e.Types[i].AssemblyQualifiedName} Error: {e.LoaderExceptions[i]}");
			}

			WriteCrashLog(sb.ToString());
		}
		catch (Exception e)
		{
			var version = Assembly.GetCallingAssembly().GetName().Version;
			var sb = new StringBuilder();
			sb.AppendLine($"Crash Log for Engine Version v{version.Major}.{version.Minor}.{version.Build}.{version.Revision.ToString("0000")}");
			sb.AppendLine();
			sb.AppendLine(e.ToString());
			WriteCrashLog(sb.ToString());
		}

		finally
		{
			Thread.Sleep(100);
			Environment.Exit(0);
		}
#endif
    }

	internal static IReadOnlyCollection<IPAddress> ParseTrustedProxyAddresses(string configurationLine)
	{
		if (string.IsNullOrWhiteSpace(configurationLine))
		{
			return [];
		}

		return configurationLine
			.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(IPAddress.Parse)
			.Select(address => address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address)
			.Distinct()
			.ToArray();
	}

	private static bool TryRunDocumentationExport(string[] args)
	{
		if (args.Length == 0 || !args[0].Equals("--export-documentation", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		if (args.Length != 4 || !args[2].Equals("--source-revision", StringComparison.OrdinalIgnoreCase))
		{
			Console.Error.WriteLine("Usage: MudSharp --export-documentation <output-path> --source-revision <sha>");
			Environment.ExitCode = 2;
			return true;
		}

		try
		{
			DocumentationCatalogueExporter.ExportAsync(args[1], args[3]).GetAwaiter().GetResult();
			Console.WriteLine($"Documentation catalogue written to {Path.GetFullPath(args[1])}.");
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine($"Documentation export failed: {exception.Message}");
			Environment.ExitCode = 1;
		}

		return true;
	}

	private static bool TryRunIndustrialisedPrerequisiteAudit(string[] args)
	{
		if (args.Length == 0 ||
		    !args[0].Equals("--audit-industrialised-prerequisites", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		if (args.Length is < 2 or > 3 ||
		    (args.Length == 3 && !args[2].Equals("--check", StringComparison.OrdinalIgnoreCase)))
		{
			Console.Error.WriteLine(
				"Usage: MudSharp --audit-industrialised-prerequisites <repository-root> [--check]");
			Environment.ExitCode = 2;
			return true;
		}

		try
		{
			var result = IndustrialisedPrerequisiteAuditExporter.Run(args[1], args.Length == 3);
			Console.WriteLine(
				$"Audited {result.RuntimeComponentCount:N0} runtime component types " +
				$"({result.ModernComponentCount:N0} modern, {result.FuturisticComponentCount:N0} futuristic, " +
				$"{result.GeneralComponentCount:N0} general) against {result.SeededPrototypeCount:N0} seeded profiles; " +
				$"{result.MissingSameTypeStockCount:N0} types have no same-type exported stock.");
			foreach (var changedFile in result.ChangedFiles)
			{
				Console.WriteLine($"Updated {changedFile}.");
			}

			foreach (var error in result.Errors)
			{
				Console.Error.WriteLine(error);
			}

			if (result.Errors.Count > 0)
			{
				Environment.ExitCode = 1;
			}
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine($"Industrialised prerequisite audit failed: {exception.Message}");
			Environment.ExitCode = 1;
		}

		return true;
	}

	private static bool TryRunGoogleEmailAuthorization(string[] args)
	{
		if (args.Length == 0 || !args[0].Equals("--authorize-google-email", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		if (args.Length != 3 || !GoogleEmailAuthorization.TryGetScope(args[1], out _))
		{
			Console.Error.WriteLine("Usage: MudSharp --authorize-google-email <gmail-api|smtp> <client-secrets.json>");
			Environment.ExitCode = 2;
			return true;
		}

		try
		{
			GoogleEmailAuthorization.AuthorizeAsync(args[1], args[2], CancellationToken.None).GetAwaiter().GetResult();
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine($"Google email authorization failed: {exception.GetType().Name}.");
			Environment.ExitCode = 1;
		}

		return true;
	}

    public static void WriteCrashLog(string crashLog)
    {
        using (StreamWriter writer = new($"Fatal Exception {DateTime.Now:yyyy MMMM dd HH mm ss}.txt"))
        {
            writer.Write(crashLog);
        }

        Futuremud.Games.First().DiscordConnection?.NotifyCrash(crashLog);
    }

    private static void ConfigureConsoleHost()
    {
        TrySetConsoleForeground(ConsoleColor.Gray);
        if (!OperatingSystem.IsWindows() || !HasInteractiveConsole())
        {
            return;
        }

        TryResizeWindowsConsole();
        TrySetConsoleTitle("FutureMUD");
    }

    private static bool HasInteractiveConsole()
    {
        if (Console.IsOutputRedirected)
        {
            return false;
        }

        try
        {
            _ = Console.WindowHeight;
            _ = Console.BufferHeight;
            return true;
        }
        catch (IOException)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (PlatformNotSupportedException)
        {
            return false;
        }
    }

    [SupportedOSPlatform("windows")]
    private static void TryResizeWindowsConsole()
    {
        try
        {
            Console.BufferHeight = short.MaxValue - 1;
            Console.WindowHeight = (int)(Console.LargestWindowHeight * 0.9);
        }
        catch (IOException)
        {
            // Headless launchers and automated smoke tests can run without a real Windows console.
        }
        catch (ArgumentOutOfRangeException)
        {
            // Some console hosts expose restricted buffer/window sizes; keep booting with defaults.
        }
    }

    private static void TrySetConsoleTitle(string title)
    {
        if (!OperatingSystem.IsWindows() || !HasInteractiveConsole())
        {
            return;
        }

        try
        {
            Console.Title = title;
        }
        catch (IOException)
        {
            // Headless launchers and automated smoke tests can run without a real Windows console.
        }
        catch (InvalidOperationException)
        {
            // Some hosts expose an output stream but do not support console window operations.
        }
    }

    private static void TrySetConsoleForeground(ConsoleColor colour)
    {
        try
        {
            Console.ForegroundColor = colour;
        }
        catch (IOException)
        {
            // Colour is cosmetic only; startup should not depend on console capabilities.
        }
        catch (InvalidOperationException)
        {
            // Some hosts expose an output stream but do not support colour operations.
        }
    }
}
