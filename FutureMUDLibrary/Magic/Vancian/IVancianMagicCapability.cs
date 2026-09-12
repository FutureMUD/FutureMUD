using System;
using System.Collections.Generic;
using MudSharp.Body.Traits;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic.Vancian;

public enum VancianRepertoireSource { Selected, Spellbook }
public enum VancianAllowanceMode { Memorised, Spontaneous, AtWill }
public enum VancianBookPolicy { EveryRefresh, PatternChangesOnly }
public enum VancianRecoveryMode { PreparationAction, SleepAutomatic, SleepThenPreparation }
public enum VancianSlotStatus { Unassigned, Prepared, AvailableSpontaneous, Reserved, Spent, Suspended }
public enum VancianCallbackStatus { Pending, Invoking, Completed, NeedsReview }

/// <summary>Keys are identity; aliases and order are presentation only.</summary>
public sealed record VancianRepertoireDefinition(
	Guid Key, string Alias, string Name, int SortOrder, VancianRepertoireSource Source,
	int MinimumSpellLevel = 0, int MaximumSpellLevel = 0, long CandidateProgId = 0,
	long SelectionLimitProgId = 0, VancianBookPolicy BookPolicy = VancianBookPolicy.EveryRefresh);

public sealed record VancianCastingAllowanceDefinition(
	Guid Key, string Alias, string Name, int SortOrder, VancianAllowanceMode Mode,
	int? SlotLevel, IReadOnlyList<Guid> RepertoireKeys, int StructuralVersion = 1,
	int MinimumSpellLevel = 0, int MaximumSpellLevel = 0, long SlotCountProgId = 0,
	long SpellEligibilityProgId = 0);

/// <summary>Configuration contains no per-character capacity or expenditure.</summary>
public interface IVancianMagicCapability : IMagicCapability
{
	IReadOnlyList<VancianRepertoireDefinition> Repertoires { get; }
	IReadOnlyList<VancianCastingAllowanceDefinition> Allowances { get; }
	IReadOnlyDictionary<string, long> PolicyProgs { get; }
	SpellPower BasePower { get; }
	int PowerStepPerSlotLevel { get; }
	Outcome ReliableOutcome { get; }
	int MaximumSavedLoadouts { get; }
	VancianRecoveryMode RecoveryMode { get; }
	TimeSpan PreparationDuration { get; }
	TimeSpan RequiredSleepDuration { get; }
	TimeSpan MinimumRefreshInterval { get; }
	ITraitDefinition ScrollCheckTrait { get; }
	Outcome ScrollMinimumOutcome { get; }
	IReadOnlyList<string> ConfigurationErrors();
}
