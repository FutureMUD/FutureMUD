using System.Collections;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;

namespace FutureMUD.RoomSpatialAcceptance;

internal static class Program
{
	private static int Main()
	{
		try
		{
			var admission = OwnedWorldAdmission.FromEnvironment();
			admission.VerifyTarget();
			var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
				.UseLazyLoadingProxies()
				.UseMySql(admission.ConnectionString, ServerVersion.AutoDetect(admission.ConnectionString)).Options;
			using (var db = new FuturemudDatabaseContext(options))
			{
				if (db.Accounts.Any() || db.SeederChoices.Any()) throw new InvalidOperationException("Historical seeder requires an unseeded owned world.");
				if (db.Database.GetPendingMigrations().Any()) throw new InvalidOperationException("Historical schema must already match the frozen seeder revision.");
			}
			var assembly = typeof(CoreDataSeeder).Assembly;
			var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			var profiles = (IEnumerable)assembly.GetType("DatabaseSeeder.DebugSeederReplayProfiles", true)!.GetProperty("All", flags)!.GetValue(null)!;
			var profile = profiles.Cast<object>().Single(x => (string)x.GetType().GetProperty("Id")!.GetValue(x)! == "medieval-standard");
			var seeders = assembly.GetType("DatabaseSeeder.SeederCatalogue", true)!.GetMethod("GetEnabledSeeders", flags)!.Invoke(null, null)!;
			Func<FuturemudDatabaseContext> factory = () => { admission.VerifyTarget(); return new FuturemudDatabaseContext(options); };
			var result = assembly.GetType("DatabaseSeeder.SeederReplayRunner", true)!.GetMethod("Run", flags)!
				.Invoke(null, [profile, seeders, factory, new Version(1, 0), (Action<string>)(s => Console.WriteLine(s))])!;
			var resultFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			if (!(bool)result.GetType().GetProperty("Success", resultFlags)!.GetValue(result)!)
				throw new InvalidOperationException("Historical complete replay failed: " + result.GetType().GetProperty("Failure", resultFlags)!.GetValue(result),
					result.GetType().GetProperty("Exception", resultFlags)!.GetValue(result) as Exception);
			Console.WriteLine("CellFullBoot-historical-replay=PASS complete-medieval-standard frozen-source");
			return 0;
		}
		catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
	}
}
