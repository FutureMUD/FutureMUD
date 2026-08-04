#nullable enable

using MudSharp.Database;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

internal interface ICombatSeederModule
{
	string Key { get; }
	string DisplayName { get; }
	IReadOnlyCollection<string> Dependencies { get; }
	bool IsSelected(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> answers);
	CombatSeederModuleResult Reconcile(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> answers);
}

internal readonly record struct CombatSeederModuleResult(string DisplayName, int Changes)
{
	public bool HasChanges => Changes > 0;
}

internal sealed class CombatSeederModule(
	string key,
	string displayName,
	IReadOnlyCollection<string> dependencies,
	Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> isSelected,
	Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, CombatSeederModuleResult> reconcile) : ICombatSeederModule
{
	public string Key => key;
	public string DisplayName => displayName;
	public IReadOnlyCollection<string> Dependencies => dependencies;

	public bool IsSelected(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> answers) =>
		isSelected(context, answers);

	public CombatSeederModuleResult Reconcile(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers) => reconcile(context, answers);
}
