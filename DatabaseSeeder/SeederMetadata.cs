#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Database;

namespace DatabaseSeeder;

public enum SeederRepeatabilityMode
{
	OneShot,
	Additive,
	Idempotent
}

public enum SeederUpdateCapability
{
	None,
	InstallMissing,
	RepairExisting,
	FullReconcile
}

public enum SeederAssessmentStatus
{
	Blocked,
	ReadyToInstall,
	AdditiveInstallAvailable,
	UpdateAvailable,
	InstalledCurrent
}

public sealed record SeederPrerequisite(string Description, Func<FuturemudDatabaseContext, bool> IsSatisfied);

public sealed record SeederMetadata(
	SeederRepeatabilityMode RepeatabilityMode,
	SeederUpdateCapability UpdateCapability,
	IReadOnlyCollection<SeederPrerequisite> Prerequisites,
	string? RerunSummary = null,
	string? UpdateSummary = null,
	string? OwnershipSummary = null)
{
	public static SeederMetadata Default { get; } = new(
		SeederRepeatabilityMode.OneShot,
		SeederUpdateCapability.None,
		Array.Empty<SeederPrerequisite>()
	);
}

public sealed record SeederAssessment(
	SeederAssessmentStatus Status,
	string Explanation,
	IReadOnlyCollection<string> MissingPrerequisites,
	IReadOnlyCollection<string> Warnings,
	IReadOnlyCollection<string> Notes);
