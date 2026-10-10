#nullable enable

using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;

namespace MudSharp.Body;

/// <summary>Exact, unscheduled native limb state that custody operations do not mutate.</summary>
internal static class RetirementBodyEffects
{
	internal static bool TryCapture(IBody body, out Func<bool> unchanged)
	{
		var effects = body.Effects.ToArray();
		unchanged = () => false;
		bool Valid(IEffect effect) => effect.GetType() == typeof(LimbSpinalDamageEffect) &&
			effect is LimbSpinalDamageEffect spinal && ReferenceEquals(effect.Owner, body) &&
			ReferenceEquals(spinal.BodyOwner, body) && ReferenceEquals(spinal.Gameworld, body.Gameworld) &&
			body.Limbs.Any(x => ReferenceEquals(x, spinal.Limb)) && !effect.SavingEffect &&
			spinal.ApplicabilityProg is null && !body.Gameworld.EffectScheduler.IsScheduled(effect);
		if (effects.Length > 0 && (!body.Actor.State.HasFlag(CharacterState.Dead) || effects.Any(x => !Valid(x)))) return false;
		var limbs = effects.Cast<LimbSpinalDamageEffect>().Select(x => x.Limb).ToArray();
		unchanged = () => body.Effects.SequenceEqual(effects, ReferenceEqualityComparer.Instance) &&
			(effects.Length == 0 || body.Actor.State.HasFlag(CharacterState.Dead)) &&
			effects.Select((effect, index) => Valid(effect) && ReferenceEquals(((LimbSpinalDamageEffect)effect).Limb, limbs[index])).All(x => x);
		return true;
	}
}
