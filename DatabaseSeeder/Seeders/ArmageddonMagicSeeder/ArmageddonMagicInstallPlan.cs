#nullable enable
using System;
using System.Collections.Generic;
using MudSharp.Magic;

namespace DatabaseSeeder.Seeders;

/// <summary>Explicit bindings for the reviewed partial package. No player or class options.</summary>
public sealed record ArmageddonMagicInstallPlan(bool Install, long School, long Resource,
	IReadOnlyDictionary<string, long> SpellSkills, long AlwaysFalseProg, long MendEligibilityProg,
	long Water, long LightPrototype, int LightRevision, long HoldableComponent, int HoldableRevision,
	long Material, long BuilderAccount, long? WaterBonusPlane = null);

public enum ArmageddonInstallStatus { Declined, Blocked, Completed, Failed, CommitOutcomeUnknown, CommittedConfirmationFailed }
public sealed record ArmageddonInstallResult(ArmageddonInstallStatus Status, IReadOnlyList<string> Messages,
	IReadOnlyDictionary<string, long> Identities)
{
	public bool Committed => Status is ArmageddonInstallStatus.Completed or ArmageddonInstallStatus.CommittedConfirmationFailed;
}

/// <summary>Fault seams belong to the installer transaction, never runtime casting.</summary>
public enum ArmageddonInstallCheckpoint { PreflightComplete, ContentCreated, BeforeCommit, AfterCommit }

public static partial class ArmageddonMagicInstaller
{
	public const string Package = "ArmageddonMagicSeeder";
	public const string Module = "reviewed-five-utilities-and-blank-devices";
	public const string ManifestVersion = "partial-1";
	public static IReadOnlyList<ArmageddonUtilitySpellContent> Content(ArmageddonMagicInstallPlan plan) =>
	[
		ArmageddonReviewedUtilityContent.SenseEnchantment(), ArmageddonReviewedUtilityContent.UnravelEnchantment(),
		ArmageddonReviewedUtilityContent.MendFlesh(), ArmageddonReviewedUtilityContent.DrawWater(plan.Water, plan.WaterBonusPlane),
		ArmageddonReviewedUtilityContent.HoveringLight(plan.LightPrototype)
	];
}
