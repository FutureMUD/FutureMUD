#nullable enable

using MudSharp.Body.Traits;
using MudSharp.CharacterCreation;

namespace MudSharp.RPG.Merits.Interfaces;

public interface IRoleSkillProgressionMerit : ICharacterMerit
{
	RoleSkillProgression Progression { get; }
	bool AppliesToTemplate(ICharacterTemplate template);
}
