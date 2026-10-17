#nullable enable
using MudSharp.Character;
using MudSharp.CharacterCreation.Roles;

namespace MudSharp.CharacterCreation;

public partial class Chargen
{
	private void EnrolSelectedClassMagic(ICharacter character)
	{
		if (Gameworld.MagicCasting is not { } casting) return;
		foreach (var role in SelectedRoles.Where(x => x.RoleType == ChargenRoleType.Class))
			foreach (var result in casting.EnrolFromSelectedClass(character, role).Where(x => !x.Result.Allowed))
				Gameworld.SystemMessage($"Character #{character.Id} was created, but Class role #{role.Id} magic capability #{result.CapabilityId} enrolment needs an explicit idempotent retry: {result.Result.Message}", true);
		// Failure does not abort approval: this character is already durable. Recreating it would duplicate identity.
	}
}
