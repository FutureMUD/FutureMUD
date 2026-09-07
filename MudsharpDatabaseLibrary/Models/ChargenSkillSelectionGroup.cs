using System.Collections.Generic;

#nullable enable
namespace MudSharp.Models;

public class ChargenSkillSelectionGroup
{
	public long Id { get; set; }
	public string StableKey { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public int DisplayOrder { get; set; }
	public bool Enabled { get; set; }
	public bool Retired { get; set; }
	public int MinimumPicks { get; set; }
	public int MaximumPicks { get; set; }
	public int Revision { get; set; } = 1;
	public int ExistingSkillPolicy { get; set; }
	public long? EligibilityProgId { get; set; }
	public long? MemberEligibilityProgId { get; set; }
	public string? SeedBaseline { get; set; }
	public virtual FutureProg? EligibilityProg { get; set; }
	public virtual FutureProg? MemberEligibilityProg { get; set; }
	public virtual ICollection<ChargenSkillSelectionGroupMember> Members { get; set; } = new List<ChargenSkillSelectionGroupMember>();
}

public class ChargenSkillSelectionGroupMember
{
	public long GroupId { get; set; }
	public long TraitDefinitionId { get; set; }
	public int DisplayOrder { get; set; }
	public virtual ChargenSkillSelectionGroup Group { get; set; } = null!;
	public virtual TraitDefinition TraitDefinition { get; set; } = null!;
}
