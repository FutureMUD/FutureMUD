#nullable enable
using System;
using MudSharp.Database;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonTraditionInstaller
{
	// This composition phase preserves the ordinary transaction and ownership checks,
	// but cannot advance an existing capability or merit's stock policy baseline.
	internal static ArmageddonTraditionInstallResult BootstrapDefinitions(FuturemudDatabaseContext db,
		ArmageddonTraditionInstallPlan plan, Action<ArmageddonInstallCheckpoint>? checkpoint = null)
		=> Install(db, plan, true, checkpoint);
}
