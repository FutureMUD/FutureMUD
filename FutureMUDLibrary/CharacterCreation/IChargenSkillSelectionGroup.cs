using System.Collections.Generic;
using MudSharp.Body.Traits;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;

#nullable enable
namespace MudSharp.CharacterCreation;

public enum ExistingSkillPolicy { NewOnly, CountKnown }

public interface IChargenSkillSelectionGroup : IEditableItem
{
	string StableKey { get; }
	string Description { get; }
	int DisplayOrder { get; }
	bool Enabled { get; }
	bool Retired { get; }
	int MinimumPicks { get; }
	int MaximumPicks { get; }
	int Revision { get; }
	ExistingSkillPolicy ExistingSkillPolicy { get; }
	IFutureProg? EligibilityProg { get; }
	IFutureProg? MemberEligibilityProg { get; }
	IReadOnlyList<ITraitDefinition> Members { get; }
	IEnumerable<string> Validate();
}
