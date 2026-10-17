#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.Magic;

public partial class MagicSpell
{
	internal void BindInvocationInputs(ICharacter caster, IPerceivable? target, IReadOnlyList<SpellAdditionalParameter> parameters)
	{
		IEnumerable<IPerceivable> targets = target is PerceivableGroup group ? group.Members : target is { } single ? [single] : Array.Empty<IPerceivable>();
		Bind(_spellEffects, targets);
		Bind(_casterSpellEffects, [caster]);
		void Bind(IEnumerable<IMagicSpellEffectTemplate> effects, IEnumerable<IPerceivable> recipients)
		{
			foreach (var binder in effects.OfType<IMagicSpellEffectInvocationInputs>())
				foreach (var recipient in recipients)
					if (!binder.TryBindInvocationInputs(caster, recipient, parameters, out var error))
						throw new InvalidOperationException(error);
		}
	}
}
