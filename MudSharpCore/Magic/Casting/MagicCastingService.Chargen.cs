#nullable enable
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
	public IReadOnlyList<MagicCastingClassEnrolment> EnrolFromSelectedClass(ICharacter target, IChargenRole selectedClass)
	{
		if (target.Id <= 0 || selectedClass.RoleType != ChargenRoleType.Class) return Array.Empty<MagicCastingClassEnrolment>();
		return selectedClass.AdditionalMerits.OfType<IMagicCapabilityMerit>()
			.SelectMany(x => x.Capabilities)
			.OfType<IMagicCastingCapability>()
			.Where(x => x.CastingPolicy is { Enabled: true, EnrolOnClassSelection: true })
			.DistinctBy(x => x.Id)
			.Select(x => new MagicCastingClassEnrolment(x.Id, EnrolAuthorised(target, x.Id, $"Chargen selected Class role {selectedClass.Id}")))
			.ToArray();
	}
}
