#nullable enable
using System.Reflection;
using DatabaseSeeder.Seeders;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static string PreparedMenu(TestDatabase database, bool install, string? bindings)
	{
		var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		var program = typeof(ArmageddonMagicSeeder).Assembly.GetType("DatabaseSeeder.Program", true)!;
		var connection = program.GetProperty("ConnectionString", flags)!; var previous = connection.GetValue(null);
		var previousIn = Console.In; var previousOut = Console.Out;
		using var input = new StringReader(new ArmageddonMagicSeeder().Name + "\nyes\n" + (install ? "yes\n" + bindings + "\n" : "\n") + "\nquit\n");
		using var output = new StringWriter();
		try
		{
			// No bootstrap/connection prompt, no Debug fixed endpoint: inject only this already
			// ownership-validated disposable connection, then exercise the real menu/ShowSeeder/
			// DoSeederQuestions/shared executor path. Restore all process-global state afterwards.
			using (var candidate = NewIndependentContext(database.ConnectionString)) { _ = candidate.Accounts.Count(); }
			connection.SetValue(null, database.ConnectionString); Console.SetIn(input); Console.SetOut(output);
			program.GetMethod("ShowMainMenu", flags)!.Invoke(null, null);
		}
		finally { connection.SetValue(null, previous); Console.SetIn(previousIn); Console.SetOut(previousOut); }
		var transcript = output.ToString(); Console.WriteLine("ARMPREP-menu-transcript-begin"); Console.Write(transcript); Console.WriteLine("ARMPREP-menu-transcript-end");
		Console.WriteLine("ARMPREP-menu=executed actual-reflective-menu questions shared-executor mode:" + (install ? "opt-in" : "default-decline"));
		return transcript;
	}
}
