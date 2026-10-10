#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Magic.SpellEffects;

namespace MudSharp.Magic;

public static class ArmageddonProjectionStock
{
	public static string Key(SpellProjectionKind kind) => kind switch { SpellProjectionKind.SandEffigy => "arm.spell.sand_effigy", SpellProjectionKind.WalkingShadow => "arm.spell.walking_shadow", _ => throw new ArgumentOutOfRangeException(nameof(kind)) };
	public static string Name(SpellProjectionKind kind) => kind switch { SpellProjectionKind.SandEffigy => "Sand Effigy", SpellProjectionKind.WalkingShadow => "Walking Shadow", _ => throw new ArgumentOutOfRangeException(nameof(kind)) };
	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition skill, IMagicResource resource, SpellProjectionConfiguration c, double energy)
	{
		if (c.Error(7) is { } error) throw new InvalidOperationException(error);
		if (!double.IsFinite(energy) || energy <= 0) throw new InvalidOperationException("Select a positive authored energy cost per grade.");
		return ArmageddonUtilityStock.Create(world, school, skill, resource, Name(c.Kind),
			"Create an owned temporary presence of your existing identity. Canonical skills and reserves are shared; inventory is never cloned. " +
			(c.Kind == SpellProjectionKind.SandEffigy ? "One paid invocation creates a figurine anchor and an immobile identifiable sand effigy. Damage or anchor removal collapses it. " :
				"Focus a walking shadow while your primary body remains vulnerable and helpless in trance. Observe and move through admitted native exits within the authored anchor range. ") +
			"Physical manipulation, material combat and teleportation are unavailable. Expiry, damage, severing, logout and reboot return focus and retire only the owned graph. " +
			"Plane, real seconds, range, energy and native backlash are authored policy. Historical two-stage figurine casting, high-power resemblance and legacy timing parity remain separate gates.",
			"0", energy, "$0 shape|shapes a temporary presence.", (resourceId, costId, _) => Definition(c, resourceId, costId, energy));
	}
	public static XElement Definition(SpellProjectionConfiguration c, long resource, long cost, double energy) => ArmageddonUtilityStock.Definition(Key(c.Kind), "character", resource, cost, 0, 30, energy, CreateProjectionEffect.Definition(c));
}
