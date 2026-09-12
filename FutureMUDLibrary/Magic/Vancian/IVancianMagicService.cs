using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;

#nullable enable
namespace MudSharp.Magic.Vancian;

public sealed record VancianAssignment(Guid AllowanceKey, int AllowanceVersion, int Ordinal,
	Guid RepertoireKey, long SpellId, int SlotLevel, int SpellLevel, SpellPower Power);

public sealed record VancianSlotView(Guid AllowanceKey, int Ordinal, VancianSlotStatus Status,
	VancianAssignment? Preparation, string? SuspensionReason);

public sealed record VancianAvailability(bool Available, string Reason, int? Ordinal = null,
	int CastingLevel = 0, SpellPower Power = SpellPower.Standard);

public sealed record VancianResult(bool Success, string Message, Guid? OperationId = null)
{
	public static VancianResult Refused(string reason) => new(false, reason);
}

/// <summary>One service owns all mutations. Queries must never initialise a ledger or roll a check.</summary>
public interface IVancianMagicService
{
	int CasterLevel(ICharacter actor, IVancianMagicCapability capability);
	int Capacity(ICharacter actor, IVancianMagicCapability capability, VancianCastingAllowanceDefinition allowance);
	IReadOnlyList<IMagicSpell> Candidates(ICharacter actor, IVancianMagicCapability capability, Guid repertoire);
	IReadOnlyList<IMagicSpell> KnownSpells(ICharacter actor, IVancianMagicCapability capability, Guid? repertoire = null);
	IReadOnlyList<VancianSlotView> Slots(ICharacter actor, IVancianMagicCapability capability);
	VancianAvailability CanCast(ICharacter actor, IVancianMagicCapability capability, Guid repertoire,
		Guid allowance, IMagicSpell spell, int? ordinal = null);
	VancianResult CommitKnown(ICharacter actor, IVancianMagicCapability capability, long expectedVersion,
		IReadOnlyDictionary<Guid, IReadOnlyList<long>> selections);
	VancianResult SelectLoadout(ICharacter actor, IVancianMagicCapability capability, string name);
	VancianResult EditLoadout(ICharacter actor, IVancianMagicCapability capability, string operation, string name,
		string? newName = null, VancianAssignment? assignment = null, Guid? allowance = null, int? ordinal = null);
	bool CanRefresh(ICharacter actor, IVancianMagicCapability capability);
	VancianResult RequestRefresh(ICharacter actor, IVancianMagicCapability capability, bool last = false);
	VancianResult Cast(ICharacter actor, IVancianMagicCapability capability, Guid repertoire,
		Guid allowance, IMagicSpell spell, int? ordinal, StringStack targets);
	VancianResult BeginInscription(ICharacter actor, IVancianMagicCapability capability, Guid repertoire,
		Guid allowance, IMagicSpell spell, int? ordinal, IGameItem blank);
	VancianResult BeginTranscription(ICharacter actor, IVancianMagicCapability capability, IGameItem source,
		IMagicSpell spell, IGameItem destination);
	VancianResult CompleteWriting(ICharacter actor, Guid token);
	VancianResult CancelWriting(ICharacter actor, Guid token);
	VancianResult ActivateScroll(ICharacter actor, IVancianMagicCapability capability, IGameItem item, StringStack targets);
}
