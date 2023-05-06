using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using MoreLinq;
using MudSharp.Database;
using MudSharp.Email;
using MudSharp.Framework;
using MudSharp.Network;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace MudSharp.Server;

internal class MudSharp
{
	private static void Main(string[] args)
	{
		Console.ForegroundColor = ConsoleColor.Gray;
		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			Console.BufferHeight = short.MaxValue - 1;
			Console.Title = "FutureMUD";
			Console.WindowHeight = (int)(Console.LargestWindowHeight * 0.9);
		}

		IPAddress hostIp;
		int tcpPort;

		using (var fs = File.Create("BOOTING"))
		{
		}

		using (var fs = File.Create("Engine-Version.info"))
		{
			using (var writer = new StreamWriter(fs, Encoding.ASCII))
			{
				writer.WriteLine(Assembly.GetCallingAssembly().GetName().Version!.ToString());
				writer.Flush();
			}
		}

		if (File.Exists("Connection.config"))
		{
			try
			{
				var fs = new FileStream("Connection.config", FileMode.Open, FileAccess.Read);
				var reader = new StreamReader(fs);
				hostIp = IPAddress.Parse(reader.ReadLine());
				tcpPort = Convert.ToInt32(reader.ReadLine());
				reader.Close();
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
				var fs = new FileStream("Connection.config", FileMode.Create, FileAccess.Write);
				var writer = new StreamWriter(fs);
				writer.WriteLine("127.0.0.1");
				writer.WriteLine("4000");
				writer.Close();
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
            try {
#endif

		using (var consoleWriter =
		       new FutureMUDConsoleWriter($"./Console Log {DateTime.UtcNow:yyyy MMMM dd HH mm ss}.txt"))
		{
			var mud = new Futuremud(new TCPServer(hostIp, tcpPort));

			Thread.CurrentThread.Name = "Main Game Thread";

			((IFuturemudLoader)mud).LoadFromDatabase();
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				Console.Title = mud.Name;
			}

			File.Delete("BOOTING");
			mud.StartGameLoop();

			// Post Shutdown Sequence

			Thread.Sleep(500);
			mud.Server.Stop();
			EmailHelper.Instance.EndEmailThread();
		}
#if DEBUG
#else
            }
            catch (DbUpdateException e)
            {
                var sb = new StringBuilder();
                sb.AppendLine(e.ToString());
                sb.AppendLine();
                sb.AppendLine($"DbUpdateException error details - {e.InnerException?.InnerException?.Message}");

                foreach (var eve in e.Entries)
                {
                    sb.AppendLine($"Entity of type {eve.Entity.GetType().Name} in state {eve.State} could not be updated");
                }

                WriteCrashLog(sb.ToString());
            }
            catch (ReflectionTypeLoadException e) {
                var sb = new StringBuilder();
                sb.AppendLine(e.ToString());
                sb.AppendLine();
                sb.AppendLine("Reflection Type Errors:");
                for (var i = 0; i < e.LoaderExceptions.Length; i++) {
                    sb.AppendLine($"Type: {e.Types[i].AssemblyQualifiedName} Error: {e.LoaderExceptions[i]}");
                }
                WriteCrashLog(sb.ToString());
            }
            catch (Exception e) {
                var sb = new StringBuilder();
                sb.AppendLine(e.ToString());
                WriteCrashLog(sb.ToString());
            }
            
            finally {
                Thread.Sleep(100);
                Environment.Exit(0);
            }
#endif
	}

	public static void WriteCrashLog(string crashLog)
	{
		using (var writer = new StreamWriter($"Fatal Exception {DateTime.Now:yyyy MMMM dd HH mm ss}.txt"))
		{
			writer.Write(crashLog);
		}

		Futuremud.Games.First().DiscordConnection?.NotifyCrash(crashLog);
	}
}