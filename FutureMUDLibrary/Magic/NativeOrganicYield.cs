#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Work.Agriculture;

namespace MudSharp.Magic.Environment;

/// <summary>The only native organic stock families that an environmental profile may authorise.</summary>
public enum NativeOrganicSourceKind
{
	Forage,
	Crop,
	Woodland,
	Pasture
}

/// <summary>The explicit result of resolving an environmental organic source.</summary>
public enum NativeOrganicSourceStatus
{
	Available,
	Exhausted,
	Absent,
	Unauthorised,
	Invalid,
	Indeterminate
}

/// <summary>Positive native production channels that ecological damage may suppress.</summary>
public enum NativeOrganicPenaltyChannel
{
	ForageReplenishment,
	CropHealthRecovery,
	CropYieldRecovery,
	WoodlandHealthRecovery,
	WoodlandYieldRecovery,
	PastureRecovery,
	CropInitialisation,
	WoodlandInitialisation,
	PastureInitialisation
}

/// <summary>
/// A profile-owned authorisation declaration. Empty definition and field-use lists mean any compatible
/// native definition/use for an explicitly enabled channel; they do not enable a channel by themselves.
/// </summary>
public sealed record NativeOrganicSourceDeclaration(
	string Selector,
	NativeOrganicSourceKind Kind,
	string? ForageKey,
	IReadOnlyList<AgricultureFieldUse> AllowedFieldUses,
	IReadOnlyList<long> DefinitionIds);

/// <summary>A profile-owned suppression formula and its exact, separately compiled dependencies.</summary>
public sealed record NativeOrganicPenaltyDefinition(
	NativeOrganicPenaltyChannel Channel,
	string Formula,
	IReadOnlyList<string> RequiredInputNames);

/// <summary>
/// Stable native lifecycle identity. A forage source uses its effective forage profile identity and zero
/// generation/definition; a field source uses FieldId, generation and vegetation definition.
/// </summary>
public sealed record NativeOrganicLifecycleIdentity(
	long CellId,
	long? FieldId,
	long Generation,
	long DefinitionId,
	long? ForageProfileId,
	int ForageProfileRevision,
	long ForageDefinitionRevision);

/// <summary>Separate saved progress pools. They are production progress, never prepaid conversion credit.</summary>
public sealed record NativeOrganicRecoveryRemainders(
	decimal Health,
	decimal Yield,
	decimal Biomass)
{
	public static NativeOrganicRecoveryRemainders Empty { get; } = new(0m, 0m, 0m);
}

/// <summary>
/// A pure current native-source observation. NativeStock is the stock seen by ordinary native consumers;
/// ConvertibleStock additionally includes an already-paid field fraction. Taking a snapshot never creates,
/// synchronises, advances, dirties or saves native state.
/// </summary>
public sealed record NativeOrganicSourceSnapshot(
	string Selector,
	NativeOrganicSourceKind Kind,
	NativeOrganicSourceStatus Status,
	NativeOrganicLifecycleIdentity? Lifecycle,
	double NativeStock,
	decimal PrepaidFraction,
	NativeOrganicRecoveryRemainders RecoveryRemainders,
	long SourceRevision,
	long? EnvironmentalProfileId,
	long EnvironmentalProfileRevision,
	AgricultureFieldUse? FieldUse,
	string? Diagnostic)
{
	public double ConvertibleStock => NativeStock + (double)PrepaidFraction;
	public bool IsEligible => Status is NativeOrganicSourceStatus.Available or NativeOrganicSourceStatus.Exhausted;
}

/// <summary>
/// A validated, short-lived native debit plan. It is deliberately not idempotent, a reservation, a receipt,
/// or a transaction across a native owner and a magic-resource holder. Apply must revalidate every captured
/// owner/profile/lifecycle token and the arithmetic before making one owner mutation.
/// </summary>
public sealed record NativeOrganicDebitPlan(
	string Selector,
	NativeOrganicSourceKind Kind,
	NativeOrganicLifecycleIdentity Lifecycle,
	long SourceRevision,
	long EnvironmentalProfileId,
	long EnvironmentalProfileRevision,
	decimal RequestedAmount,
	decimal OpeningPrepaidFraction,
	int WholeNativeDebit,
	decimal ClosingPrepaidFraction,
	double ExpectedNativeStock);

/// <summary>Read-only native values supplied to one ecological suppression formula.</summary>
public sealed record NativeOrganicPenaltyContext(
	NativeOrganicSourceKind Kind,
	string Selector,
	double NativeStock,
	double NativeHealth,
	double NativeYield,
	double NativeCapacity,
	double FieldCondition,
	double BaselineIncrease);

public readonly record struct NativeOrganicPenaltyEvaluation(
	bool IsConfigured,
	bool IsValid,
	double Factor,
	string? Error)
{
	public static NativeOrganicPenaltyEvaluation Neutral => new(false, true, 1.0, null);
	public static NativeOrganicPenaltyEvaluation Invalid(string error) => new(true, false, 0.0, error);
}

/// <summary>A pure effective-forage-owner observation used by compare-and-apply native debits.</summary>
public sealed record NativeForageYieldSnapshot(
	string Key,
	long ProfileId,
	int ProfileRevision,
	long DefinitionRevision,
	double Maximum,
	double Stock,
	long SourceRevision);

public static class NativeOrganicSourceSelectors
{
	public const int MaximumForageKeyLength = 128;

	public static string Canonical(NativeOrganicSourceKind kind, string? forageKey = null) => kind switch
	{
		NativeOrganicSourceKind.Forage => $"forage:{NormaliseForageKey(forageKey)}",
		NativeOrganicSourceKind.Crop => "crop",
		NativeOrganicSourceKind.Woodland => "woodland",
		NativeOrganicSourceKind.Pasture => "pasture",
		_ => string.Empty
	};

	public static string NormaliseForageKey(string? key) => (key ?? string.Empty).Trim().ToLowerInvariant();

	public static bool IsValidForageKey(string? key)
	{
		var normalised = NormaliseForageKey(key);
		return normalised.Length is > 0 and <= MaximumForageKeyLength &&
		       normalised.All(character => !char.IsControl(character));
	}
}

/// <summary>Shared exact arithmetic used by both pure planning and native-owner revalidation.</summary>
public static class NativeOrganicAccountingMath
{
	public const decimal MinimumDebit = 0.000000001m;
	public const decimal MaximumDebit = 1000000000000m;

	public static bool TryAmount(double value, out decimal amount, out string? error)
	{
		amount = 0m;
		if (!double.IsFinite(value) || value <= 0.0)
		{
			error = "The native debit must be a finite positive amount.";
			return false;
		}

		try
		{
			amount = (decimal)value;
		}
		catch (OverflowException)
		{
			error = "The native debit is outside the supported accounting range.";
			return false;
		}

		if (amount < MinimumDebit || amount > MaximumDebit)
		{
			error = $"The native debit must be from {MinimumDebit} through {MaximumDebit} source units.";
			return false;
		}

		error = null;
		return true;
	}

	public static bool TryPlanInteger(int stock, decimal prepaid, decimal requested, out int wholeDebit,
		out decimal closingPrepaid, out string? error)
	{
		wholeDebit = 0;
		closingPrepaid = prepaid;
		if (stock < 0 || prepaid is < 0m or >= 1m || requested < MinimumDebit || requested > MaximumDebit)
		{
			error = "The native stock, prepaid fraction or requested amount is outside the supported accounting range.";
			return false;
		}

		var unpaid = requested - prepaid;
		var whole = unpaid <= 0m ? 0m : decimal.Ceiling(unpaid);
		if (whole > int.MaxValue || whole > stock)
		{
			error = "The source does not have enough native stock and prepaid credit for that complete debit.";
			return false;
		}

		wholeDebit = (int)whole;
		closingPrepaid = prepaid + wholeDebit - requested;
		if (closingPrepaid is < 0m or >= 1m)
		{
			error = "The native debit would produce an invalid prepaid fraction.";
			return false;
		}

		error = null;
		return true;
	}
}
