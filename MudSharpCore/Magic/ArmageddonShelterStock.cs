#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Magic.SpellEffects;

namespace MudSharp.Magic;

public static class ArmageddonShelterStock
{
	public static string Key(SpellShelterKind kind) => kind switch
	{
		SpellShelterKind.SpringHaven => "arm.spell.spring_haven", SpellShelterKind.BurrowRefuge => "arm.spell.burrow_refuge",
		SpellShelterKind.SandShelter => "arm.spell.sand_shelter", SpellShelterKind.SeveringRefuge => "arm.spell.severing_refuge", _ => throw new ArgumentOutOfRangeException(nameof(kind))
	};
	public static string Name(SpellShelterKind kind) => kind switch
	{
		SpellShelterKind.SpringHaven => "Spring Haven", SpellShelterKind.BurrowRefuge => "Burrow Refuge",
		SpellShelterKind.SandShelter => "Sand Shelter", SpellShelterKind.SeveringRefuge => "Severing Refuge", _ => throw new ArgumentOutOfRangeException(nameof(kind))
	};
	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait, IMagicResource resource,
		SpellShelterConfiguration configuration)
	{
		if ((configuration.Kind == SpellShelterKind.SeveringRefuge) != (configuration.Ward is not null))
			throw new InvalidOperationException("Only Severing Refuge requires an explicitly authored ward.");
		configuration.Ward?.Validate();
		return ArmageddonUtilityStock.Create(world, school, trait, resource, Name(configuration.Kind),
			"Create a temporary enterable native shelter from explicitly bound indoor, terrain and fallback templates. " +
			"Its physical capacity includes offline instances. Native indoor classification provides weather shelter. " +
			"Visitors and their goods return to the recorded source or configured fallback on expiry or destruction. " +
			"Unavailable safe destinations retain a closed recoverable shelter with diagnostics. " +
			(configuration.Ward is null ? "" : "Its owned fail-mode ward blocks only selected native invocation schools or tags, with authored incoming/outgoing coverage. Physical hazards remain governed by native simulation. ") +
			(configuration.Kind == SpellShelterKind.SpringHaven ? "The water is a finite, paid initial volume, never refilled on reload. Remaining liquid survives as a native puddle when its container dissipates. " : "") +
			"The explicitly selected duration in real seconds and native template/terrain/capacity choices are authored policy; historical parity remains a separate source gate.",
			"0", 9, "$0 shape|shapes a sheltered space.", (resourceId, costId, _) => Definition(configuration, resourceId, costId));
	}
	public static XElement Definition(SpellShelterConfiguration configuration, long resourceId, long costId) =>
		ArmageddonUtilityStock.Definition(Key(configuration.Kind), "room", resourceId, costId, 0, 30, 9,
			CreateShelterEffect.Definition(configuration));
}
