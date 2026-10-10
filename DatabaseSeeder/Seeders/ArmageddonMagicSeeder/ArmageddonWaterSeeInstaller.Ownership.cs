#nullable enable
using System.Collections.Generic;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonWaterSeeInstaller
{
	private static ArmageddonStockContribution Spec(Contribution contribution) => new(contribution.Type, contribution.Key, contribution.Name);
	private static ArmageddonStockContributionReconciler Reconciler(FuturemudDatabaseContext db) => new(db, Package, Module, Version);
	private static object? Find(FuturemudDatabaseContext db, Contribution contribution, SeederManagedRecord record) => Reconciler(db).Find(Spec(contribution), record);
	private static void Allocate<T>(FuturemudDatabaseContext db, Contribution contribution, T row, Dictionary<string, SeederManagedRecord> records) where T : class =>
		Reconciler(db).Allocate(Spec(contribution), row, records);
	private static T Apply<T>(FuturemudDatabaseContext db, Contribution contribution, T desired, Dictionary<string, SeederManagedRecord> records,
		ArmageddonWaterSeeInstallPlan plan, List<string> messages, bool allocated = false) where T : class =>
		Reconciler(db).Apply(Spec(contribution), desired, records, plan, messages, allocated);
}