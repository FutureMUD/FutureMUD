using System;
using System.Collections.Generic;
using MudSharp.Magic;

#nullable enable
namespace MudSharp.GameItems.Interfaces;

public sealed record SpellbookFormula(Guid EntryId, long SpellId, DateTime CopiedUtc, long? SourceItemId);

/// <summary>Structured formulae are instance data, independent of readable prose and prototype revisions.</summary>
public interface ISpellbook : IGameItemComponent
{
	IReadOnlyList<SpellbookFormula> Formulae { get; }
	int FormulaCapacity { get; }
	string? DataError { get; }
}

public interface ISpellScroll : IGameItemComponent
{
	bool IsCharged { get; }
	bool IsBlank { get; }
	long? SpellId { get; }
	int? CastingLevel { get; }
	SpellPower? StoredPower { get; }
	Guid? ChargeId { get; }
	string? DataError { get; }
}
