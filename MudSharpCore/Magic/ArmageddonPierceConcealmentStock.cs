#nullable enable

using System.Xml.Linq;
using MudSharp.Body.Traits;

namespace MudSharp.Magic;

public static class ArmageddonPierceConcealmentStock
{
	public const string Key = "arm.spell.pierce_concealment";
	public const string Name = "Pierce Concealment";
	public const string LifetimeSeconds = "3000*grade";
	public const double MinimumEnergy = 7;
	public const string LifetimeGroup = "armageddon.detect_invisibility";
	internal const string EligibilitySource = "return lowercase(@caster.location.terrain.name) != \"silt\"";

	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait, IMagicResource resource) =>
		ArmageddonUtilityStock.Create(world, school, trait, resource, Name,
			"Let the recipient perceive eligible invisible entities through native perception. Historical detect_invisibility refuses Silt and adds five source hours per grade, mapped to 3000 real seconds per grade, to remaining whole source hours up to 48 hours. The replacement retains the strongest source grade. Edit its lifetime group, unit seconds and cap through the normal effect builder. Native exact deadlines normalize the source's inclusive endpoint and use existing saved remaining-time behavior offline. Native planar and other perception restrictions remain; this spell does not remove blindness. Edit the support prog for your terrain names. No source consumed component is required. Ordinary character/self targeting is provided; historical area and device wrappers are separate delivery policies. Sorcerer opening 30, cap 90, minimum energy 7; add acquisition separately.",
			LifetimeSeconds, MinimumEnergy, "$0 open|opens $1's senses to the unseen.", Definition, EligibilitySource);

	internal static XElement Definition(long resource, long cost, long filter) =>
		ArmageddonUtilityStock.Definition(Key, "character", resource, cost, filter, 30, MinimumEnergy,
			new XElement("Effect", new XAttribute("type", "detectinvisible"),
				SpellEffects.DetectInvisibleEffect.WritePolicy(new(LifetimeGroup, 600, 48))));
}
