#nullable enable
using System;
using System.Collections.Generic;
using MudSharp.Magic;

namespace DatabaseSeeder.Seeders;

/// <summary>Explicit selections for a prerequisite-closed partial package; never an enrolment request.</summary>
public sealed record ArmageddonTraditionInstallPlan(bool Install, long School, long SourceResource,
	long ReserveResource, long Decorator, long AlwaysFalseProg, long AlwaysTrueProg,
	long GatheringTemplate, IReadOnlyDictionary<string, long> ImplementedSpells,
	IReadOnlyDictionary<string, long> SupportSkills,
	IReadOnlyDictionary<string, IReadOnlyList<MagicGatheringMethodKind>>? AllowedMethods = null);

public sealed record ArmageddonSourceRow(int Order, string Kind, string HistoricalName, string Key,
	string? ParentHistoricalName, string? ParentKey, double? ParentThreshold, double Opening,
	double RawCap, double BranchesAt, double? PrintedMinimumMana);

public sealed record ArmageddonTraditionInstallResult(ArmageddonInstallStatus Status,
	IReadOnlyList<string> Messages, IReadOnlyDictionary<string, long> Identities,
	IReadOnlyList<string> AvailableSpells, IReadOnlyList<string> UnavailableSpells);

public static partial class ArmageddonTraditionInstaller
{
	public const string Package = ArmageddonMagicInstaller.Package;
	public const string Module = "partial-source-traditions";
	public const string Version = "partial-1";
	public static readonly IReadOnlyList<string> Variants = Array.AsReadOnly(new[] { "sorcerer", "preserver", "defiler" });
}
