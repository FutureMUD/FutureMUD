#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.FutureProg;

namespace MudSharp.Magic.Environment;

/// <summary>A reusable, centrally coordinated definition for physical cell resources.</summary>
public interface IEnvironmentalMagicProfile : IMagicResourceRegenerator
{
	long Revision { get; }
	double PressureHalfLifeSeconds { get; }
	double NaturalRepairPerMinute { get; }
	double? IdleRecheckSeconds { get; }
	DateTimeOffset DecayReferenceUtc { get; }
	double DecayIntegral { get; }
	IReadOnlyList<EnvironmentalMagicOutput> Outputs { get; }
	IReadOnlyList<EnvironmentalMagicInput> Inputs { get; }
	IReadOnlyList<string> ValidationErrors { get; }
	bool HasOrganicConfiguration { get; }
	IReadOnlyList<NativeOrganicSourceDeclaration> OrganicSources { get; }
	IReadOnlyList<NativeOrganicPenaltyDefinition> OrganicPenalties { get; }
	long? OrganicProtectionProgId { get; }
	IFutureProg? OrganicProtectionProg { get; }
	/// <summary>Organic-only errors never disable otherwise valid legacy mana outputs.</summary>
	IReadOnlyList<string> OrganicValidationErrors { get; }
	/// <summary>All referenced built-in and named inputs, compared without case.</summary>
	IReadOnlySet<string> RequiredInputNames { get; }
	/// <summary>Cumulative half-lives; differences preserve pressure decay across profile edits.</summary>
	double PressureDecayIntegralAt(DateTimeOffset utc);
	/// <summary>Rebind edited references once per definition revision, without evaluating cell inputs.</summary>
	void RefreshReferences();
	/// <summary>Evaluate a maximum, then a rate, against one already collected immutable input snapshot.</summary>
	EnvironmentalMagicOutputEvaluation EvaluateOutput(EnvironmentalMagicOutput output,
		IReadOnlyDictionary<string, double> sharedInputs, double balance);
	/// <summary>Dependencies for one penalty only; evaluating a penalty never evaluates mana outputs.</summary>
	IReadOnlySet<string> RequiredOrganicInputNames(NativeOrganicPenaltyChannel channel);
	NativeOrganicPenaltyEvaluation EvaluateOrganicPenalty(NativeOrganicPenaltyChannel channel,
		IReadOnlyDictionary<string, double> inputs);
}

public sealed record EnvironmentalMagicOutput(long ResourceId, IMagicResource? Resource,
	string MaximumFormula, string RateFormula, double BaseCapacity, double BaseRate);

public enum EnvironmentalMagicInputKind
{
	Forage,
	Agriculture,
	Prog
}

/// <summary>Source is a forage key, an agriculture value name, or the invariant textual prog ID.</summary>
public sealed record EnvironmentalMagicInput(string Name, EnvironmentalMagicInputKind Kind,
	string Source, double Scale, long? ProgId = null, IFutureProg? Prog = null);

public readonly record struct EnvironmentalMagicOutputEvaluation(bool IsValid, double Maximum,
	double Rate, string? Error)
{
	public static EnvironmentalMagicOutputEvaluation Invalid(string error) => new(false, 0.0, 0.0, error);
}
