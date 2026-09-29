using Microsoft.EntityFrameworkCore;
using MudSharp.Database;

#nullable enable
namespace MudSharp.Magic.Casting;

public sealed record CastingSkillOpportunity(long CharacterId, long TraitId, DateTime NextUtc, long Version);
public sealed record CastingEnrolment(long CharacterId, Guid CapabilityIdentity, long CapabilityId, DateTime EnrolledUtc, int StartingVersion);
public sealed record CastingOperation(Guid Id, long CharacterId, long ActorId, long BodyId, long CapabilityId,
	long SpellId, long TraitId, long ReserveId, string Stage, string Definition, DateTime CreatedUtc, DateTime UpdatedUtc,
	string Diagnostic = "");

public interface IMagicCastingStateStore
{
	AcquiredSpell? Acquisition(long characterId, long spellId);
	CastingSkillOpportunity? Opportunity(long characterId, long traitId);
	CastingEnrolment? Enrolment(long characterId, Guid capabilityIdentity);
	IReadOnlyList<CastingOperation> Unresolved(long? characterId = null);
	CastingOperation? Operation(Guid id);
	void Write(CastingOperation? operation = null, AcquiredSpell? acquired = null,
		CastingSkillOpportunity? opportunity = null, CastingEnrolment? enrolment = null);
}

/// <summary>Immediate isolated writes. Progress and the operation stage cross one optimistic transaction.</summary>
public sealed class MagicCastingStateStore : IMagicCastingStateStore
{
	public AcquiredSpell? Acquisition(long characterId, long spellId)
	{
		using (new FMDB())
		{
			var row = FMDB.Context.CharacterAcquiredSpells.AsNoTracking().SingleOrDefault(x => x.CharacterId == characterId && x.MagicSpellId == spellId);
			return row is null ? null : new(row.CharacterId, row.MagicSpellId, row.ControlledGrade, row.ProfileVersion,
				Utc(row.AcquiredUtc), row.Provenance, Utc(row.NextMasteryUtc), row.StateVersion);
		}
	}
	public CastingSkillOpportunity? Opportunity(long characterId, long traitId)
	{
		using (new FMDB())
		{
			var row = FMDB.Context.CharacterMagicSkillOpportunities.AsNoTracking().SingleOrDefault(x => x.CharacterId == characterId && x.TraitDefinitionId == traitId);
			return row is null ? null : new(row.CharacterId, row.TraitDefinitionId, Utc(row.NextOpportunityUtc), row.StateVersion);
		}
	}
	public CastingEnrolment? Enrolment(long characterId, Guid capabilityIdentity)
	{
		using (new FMDB())
		{
			var row = FMDB.Context.CharacterCastingEnrolments.AsNoTracking().SingleOrDefault(x => x.CharacterId == characterId && x.CapabilityIdentity == capabilityIdentity);
			return row is null ? null : new(row.CharacterId, row.CapabilityIdentity, row.MagicCapabilityId, Utc(row.EnrolledUtc), row.CompletedStartingGrantVersion);
		}
	}
	public IReadOnlyList<CastingOperation> Unresolved(long? characterId = null)
	{
		using (new FMDB()) return FMDB.Context.MagicCastingOperations.AsNoTracking().Where(x => (!characterId.HasValue || x.CharacterId == characterId) &&
			x.Stage != "Completed" && x.Stage != "Reconciled").AsEnumerable().Select(Read).ToArray();
	}
	public CastingOperation? Operation(Guid id)
	{
		using (new FMDB()) return FMDB.Context.MagicCastingOperations.AsNoTracking().SingleOrDefault(x => x.Id == id) is { } row ? Read(row) : null;
	}

	public void Write(CastingOperation? operation = null, AcquiredSpell? acquired = null,
		CastingSkillOpportunity? opportunity = null, CastingEnrolment? enrolment = null)
	{
		using var isolated = FMDB.IsIsolated ? null : FMDB.BeginIsolatedScope();
		using (new FMDB())
		using (var transaction = FMDB.Context.Database.BeginTransaction())
		{
			if (acquired is { } a)
			{
				var row = FMDB.Context.CharacterAcquiredSpells.Find(a.CharacterId, a.SpellId);
				if ((row?.StateVersion ?? 0) != a.Version) throw new InvalidOperationException("Acquired-spell state changed concurrently.");
				if (row is null) { row = new() { CharacterId = a.CharacterId, MagicSpellId = a.SpellId }; FMDB.Context.CharacterAcquiredSpells.Add(row); }
				row.ControlledGrade = a.ControlledGrade; row.ProfileVersion = a.ProfileVersion; row.AcquiredUtc = a.AcquiredUtc;
				row.Provenance = a.Provenance; row.NextMasteryUtc = a.NextMasteryUtc; row.StateVersion = checked(a.Version + 1);
			}
			if (opportunity is { } o)
			{
				var row = FMDB.Context.CharacterMagicSkillOpportunities.Find(o.CharacterId, o.TraitId);
				if ((row?.StateVersion ?? 0) != o.Version) throw new InvalidOperationException("Skill opportunity changed concurrently.");
				if (row is null) { row = new() { CharacterId = o.CharacterId, TraitDefinitionId = o.TraitId }; FMDB.Context.CharacterMagicSkillOpportunities.Add(row); }
				row.NextOpportunityUtc = o.NextUtc; row.StateVersion = checked(o.Version + 1);
			}
			if (enrolment is { } e)
			{
				var row = FMDB.Context.CharacterCastingEnrolments.Find(e.CharacterId, e.CapabilityIdentity);
				if (row is null) { row = new() { CharacterId = e.CharacterId, CapabilityIdentity = e.CapabilityIdentity }; FMDB.Context.CharacterCastingEnrolments.Add(row); }
				row.MagicCapabilityId = e.CapabilityId; row.EnrolledUtc = e.EnrolledUtc; row.CompletedStartingGrantVersion = e.StartingVersion;
			}
			if (operation is { } op)
			{
				var row = FMDB.Context.MagicCastingOperations.Find(op.Id);
				if (row is null) { row = new() { Id = op.Id }; FMDB.Context.MagicCastingOperations.Add(row); }
				row.CharacterId = op.CharacterId; row.ActorId = op.ActorId; row.BodyId = op.BodyId; row.MagicCapabilityId = op.CapabilityId;
				row.MagicSpellId = op.SpellId; row.TraitDefinitionId = op.TraitId; row.ReserveId = op.ReserveId;
				row.Stage = op.Stage; row.Definition = op.Definition; row.CreatedUtc = op.CreatedUtc; row.UpdatedUtc = op.UpdatedUtc; row.Diagnostic = op.Diagnostic;
			}
			FMDB.Context.SaveChanges(); transaction.Commit();
		}
	}
	private static DateTime Utc(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Utc);
	private static CastingOperation Read(Models.MagicCastingOperation x) => new(x.Id, x.CharacterId, x.ActorId, x.BodyId,
		x.MagicCapabilityId, x.MagicSpellId, x.TraitDefinitionId, x.ReserveId, x.Stage, x.Definition, Utc(x.CreatedUtc), Utc(x.UpdatedUtc), x.Diagnostic);
}
