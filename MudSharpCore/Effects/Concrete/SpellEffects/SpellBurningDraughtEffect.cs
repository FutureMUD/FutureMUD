#nullable enable

using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete.SpellEffects;

/// <summary>A timed curse that replaces each accepted drink's refreshment with one native burn.</summary>
public sealed class SpellBurningDraughtEffect : MagicSpellEffectBase
{
	public static void InitialiseEffectType() => RegisterFactory("SpellBurningDraught",
		(root, owner) => new SpellBurningDraughtEffect(root, owner));

	public SpellBurningDraughtEffect(ICharacter owner, IMagicSpellEffectParent parent, double damagePerSip)
		: base(owner, parent, null)
	{
		DamagePerSip = ValidateDamage(damagePerSip);
	}

	private SpellBurningDraughtEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		if (owner is not ICharacter) throw new FormatException("Burning Draught requires a character owner.");
		DamagePerSip = ValidateDamage(double.Parse(root.Element("Effect")?.Element("DamagePerSip")?.Value ?? "0"));
	}

	private static double ValidateDamage(double amount) =>
		double.IsFinite(amount) && amount is >= 0.1 and <= 100.0
			? amount
			: throw new FormatException("Burning Draught damage per sip must be finite and between 0.1 and 100.");

	public double DamagePerSip { get; }
	protected override string SpecificEffectType => "SpellBurningDraught";
	protected override XElement SaveDefinition() => new("Effect", new XElement("ApplicabilityProg", 0),
		new XElement("DamagePerSip", DamagePerSip));
	public override string Describe(IPerceiver voyeur) =>
		$"Drinking inflicts {DamagePerSip.ToString("N1", voyeur)} burning damage instead of refreshment.";

	/// <summary>Return true only when the exact attached curse replaced this accepted sip.</summary>
	public static bool TryBurnSip(ICharacter character, IBody body)
	{
		foreach (var effect in character.EffectsOfType<SpellBurningDraughtEffect>())
		{
			if (!effect.Attached(character, body)) continue;
			var part = body.RandomBodypart;
			if (part is null || !body.Bodyparts.Any(x => ReferenceEquals(x, part)) ||
				!effect.Attached(character, body)) continue;
			var guard = new SipGuard(effect, character, body, part);
			try
			{
				var wounds = body.PassiveSufferDamage(new Damage
				{
					ActorOrigin = effect.ParentEffect?.Caster,
					TargetBody = body,
					Bodypart = part,
					MutationGuard = guard,
					DamageType = DamageType.Burning,
					DamageAmount = effect.DamagePerSip,
					PainAmount = effect.DamagePerSip,
					StunAmount = 0.0,
					ShockAmount = 0.0,
					AngleOfIncidentRadians = Math.PI * 0.5,
					PenetrationOutcome = new CheckOutcome { Outcome = Outcome.MajorPass }
				}).ToArray();
				wounds.ProcessPassiveWounds(body);
			}
			catch (StaleSipException)
			{
				// The accepted sip still consumes liquid, but a revoked curse cannot mutate wounds.
			}
			return true;
		}
		return false;
	}

	private bool Attached(ICharacter character, IBody body) => ReferenceEquals(Owner, character) &&
		ReferenceEquals(character.Body, body) && ReferenceEquals(body.Actor, character) &&
		ParentEffect is { } parent && ReferenceEquals(parent.Owner, character) &&
		character.Effects.Any(x => ReferenceEquals(x, parent)) &&
		character.Effects.Any(x => ReferenceEquals(x, this)) &&
		parent.SpellEffects.Any(x => ReferenceEquals(x, this));

	private sealed class StaleSipException() : InvalidOperationException(
		"Burning Draught lost its exact living body or wound custody.");

	private sealed class SipGuard(SpellBurningDraughtEffect effect, ICharacter character, IBody body,
		IBodypart part) : IDamageMutationGuard
	{
		private readonly IGameItem[] _armour = body.WornItems
			.Concat(body.Prosthetics.Select(x => x.Parent)).ToArray();

		public void Validate(IHaveWounds recipient, IDamage damage)
		{
			var currentArmour = body.WornItems.Concat(body.Prosthetics.Select(x => x.Parent));
			var validRecipient = ReferenceEquals(recipient, body) || recipient is IGameItem item &&
				_armour.Any(x => ReferenceEquals(x, item)) && !item.Deleted && !item.Destroyed &&
				currentArmour.Any(x => ReferenceEquals(x, item));
			var validPart = damage.Bodypart is null && recipient is IGameItem ||
				damage.Bodypart is { } damagedPart &&
				(body.Bodyparts.Contains(damagedPart) || body.Bones.Contains(damagedPart) ||
				 body.Organs.Contains(damagedPart));
			if (!validRecipient || !validPart || !effect.Attached(character, body) ||
				!body.Bodyparts.Any(x => ReferenceEquals(x, part)) ||
				!ReferenceEquals(damage.MutationGuard, this) || !ReferenceEquals(damage.TargetBody, body))
				throw new StaleSipException();
		}
	}
}
