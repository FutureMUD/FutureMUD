using System;

#nullable enable
namespace MudSharp.Models;

public class CharacterAcquiredSpell
{
	public long CharacterId { get; set; }
	public long MagicSpellId { get; set; }
	public DateTime AcquiredUtc { get; set; }
	public string Provenance { get; set; } = "";
	public int ControlledGrade { get; set; }
	public int ProfileVersion { get; set; }
	public DateTime NextMasteryUtc { get; set; }
	public long StateVersion { get; set; }
	public virtual Character Character { get; set; } = null!;
	public virtual MagicSpell MagicSpell { get; set; } = null!;
}

public class CharacterMagicSkillOpportunity
{
	public long CharacterId { get; set; }
	public long TraitDefinitionId { get; set; }
	public DateTime NextOpportunityUtc { get; set; }
	public long StateVersion { get; set; }
	public virtual Character Character { get; set; } = null!;
	public virtual TraitDefinition TraitDefinition { get; set; } = null!;
}

public class CharacterCastingEnrolment
{
	public long CharacterId { get; set; }
	public Guid CapabilityIdentity { get; set; }
	public long MagicCapabilityId { get; set; }
	public DateTime EnrolledUtc { get; set; }
	public int CompletedStartingGrantVersion { get; set; }
	public virtual Character Character { get; set; } = null!;
}

/// <summary>Durable evidence of a narrow paid invocation; unresolved rows quarantine their inputs.</summary>
public class MagicCastingOperation
{
	public Guid Id { get; set; }
	public long CharacterId { get; set; }
	public long ActorId { get; set; }
	public long BodyId { get; set; }
	public long MagicCapabilityId { get; set; }
	public long MagicSpellId { get; set; }
	public long TraitDefinitionId { get; set; }
	public long ReserveId { get; set; }
	public string Stage { get; set; } = "";
	public string Definition { get; set; } = "";
	public DateTime CreatedUtc { get; set; }
	public DateTime UpdatedUtc { get; set; }
	public string Diagnostic { get; set; } = "";
}
