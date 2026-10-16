#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Magic.SpellEffects;

internal sealed class RecognisedIdentitySnapshot
{
	private readonly List<Func<bool>> _checks = [];
	public bool Current => _checks.All(x => x());
	public RecognisedIdentitySnapshot(ICharacter caster, ICharacter target, bool captureTransientEffects = true)
	{
		void Value<T>(Func<T> read) { var before = read(); _checks.Add(() => EqualityComparer<T>.Default.Equals(before, read())); }
		void Sequence<T>(Func<IEnumerable<T>> read) where T : class
		{ var before = read().ToArray(); _checks.Add(() => before.SequenceEqual(read(), ReferenceEqualityComparer.Instance)); }
		Sequence(() => caster.Dubs);
		foreach (var dub in caster.Dubs)
		{
			Value(() => dub.Id); Value(() => dub.TargetId); Value(() => dub.TargetType); Value(() => dub.HasVerifiedIdentity);
			Value(() => dub.WasIdentityConcealed); Value(() => string.Join("\0", dub.Keywords));
			var owner = dub.Owner; _checks.Add(() => ReferenceEquals(dub.Owner, owner));
		}
		Value(() => target.IsPrimaryInstance);
		foreach (var actor in new[] { caster, target })
		{
			if (captureTransientEffects) { Sequence(() => actor.Effects); Sequence(() => actor.Body.Effects); }
			else
			{
				// Native movement may remove unrelated contact/action effects. Identity suppression remains authoritative.
				Sequence(() => actor.Effects.OfType<PsychicSuppressionEffect>());
				Sequence(() => actor.Body.Effects.OfType<PsychicSuppressionEffect>());
			}
			Sequence(() => actor.Body.WornItems);
			foreach (var item in actor.Body.WornItems)
			{
				Sequence(() => item.Components); Sequence(() => item.Effects);
				foreach (var obscurer in item.Components.OfType<IObscureIdentity>()) Value(() => obscurer.CurrentlyApplies);
			}
		}
	}
}
