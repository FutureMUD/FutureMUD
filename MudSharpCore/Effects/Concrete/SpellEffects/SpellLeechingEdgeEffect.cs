#nullable enable

using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Magic.SpellEffects;

namespace MudSharp.Effects.Concrete.SpellEffects;

/// <summary>Native parent owns duration, persistence, exclusive replacement and dispel.</summary>
public sealed class SpellLeechingEdgeEffect : MagicSpellEffectBase
{
	public static void InitialiseEffectType() => RegisterFactory("SpellLeechingEdge",
		(root, owner) => new SpellLeechingEdgeEffect(root, owner));
	public SpellLeechingEdgeEffect(IGameItem owner, IMagicSpellEffectParent parent, double fraction) : base(owner, parent, null)
	{
		Fraction = fraction;
		if (!double.IsFinite(fraction) || fraction is <= 0 or > 1) throw new ArgumentOutOfRangeException(nameof(fraction));
	}
	private SpellLeechingEdgeEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		Fraction = (double?)root.Element("Effect")?.Element("Fraction") ?? 0;
		if (!double.IsFinite(Fraction) || Fraction is <= 0 or > 1) Fraction = 0;
	}
	public double Fraction { get; }
	protected override string SpecificEffectType => "SpellLeechingEdge";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Fraction", Fraction));
	public override string Describe(IPerceiver voyeur) => $"Melee weapon vitality transfer: {Fraction.ToString("P0", voyeur).ColourValue()} of actual eligible damage.";

	internal bool IsAttached(IGameItem weapon) => Fraction > 0 && ReferenceEquals(Owner, weapon) &&
		ReferenceEquals(ParentEffect?.Owner, weapon) && weapon.Effects.Any(x => ReferenceEquals(x, this)) &&
		weapon.Effects.Any(x => ReferenceEquals(x, ParentEffect)) &&
		ParentEffect!.SpellEffects.Any(x => ReferenceEquals(x, this));

	/// <summary>Called only at the native combat injury boundary, never on an attack attempt or result replay.</summary>
	internal static VitalTransferReceipt? BeginHit(IHaveWounds target, IDamage damage, out double fraction)
	{
		fraction = 0;
		if (damage is null) return null;
		var victim = target switch { ICharacter character => character, MudSharp.Body.IBody body => body.Actor, _ => null };
		if (victim?.Body is null || damage.ActorOrigin is not { Body: not null } attacker ||
			damage.ToolOrigin is not { } weapon || ReferenceEquals(attacker, victim) || damage.VitalTransfer is not null ||
			weapon.GetItemType<IMeleeWeapon>() is null || attacker.Combat?.Friendly == true ||
			!attacker.Body.WieldedItems.Any(x => ReferenceEquals(x, weapon)) ||
			target is MudSharp.Body.IBody targetBody && !ReferenceEquals(victim.Body, targetBody)) return null;
		// Overlapping enchantments cannot each spend the same delivered vitality. The strongest wins once.
		var effect = weapon.Effects.OfType<SpellLeechingEdgeEffect>().Where(x => x.IsAttached(weapon))
			.OrderByDescending(x => x.Fraction).FirstOrDefault();
		if (effect is null) return null;
		var bodyAtHit = attacker.Body;
		fraction = effect.Fraction;
		return new VitalTransferReceipt(attacker, victim, weapon, () => effect.IsAttached(weapon) &&
			ReferenceEquals(attacker.Body, bodyAtHit) && bodyAtHit.WieldedItems.Any(x => ReferenceEquals(x, weapon)));
	}
}
