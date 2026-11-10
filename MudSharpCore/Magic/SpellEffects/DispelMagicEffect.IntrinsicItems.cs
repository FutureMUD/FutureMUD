#nullable enable

using MudSharp.Effects.Interfaces;
using MudSharp.GameItems;

namespace MudSharp.Magic.SpellEffects;

public partial class DispelMagicEffect
{
	private bool ApplyIntrinsicItemEffects(IPerceivable? target, SpellPower power)
	{
		// Permanent item states have no caster, school or spell parent. Never let
		// opting in bypass a caller's ownership or identity filters.
		if (!IncludeIntrinsicItemEffects || target is not IGameItem item || Mode != DispelMagicMode.Remove ||
			CasterPolicy != DispelCasterPolicy.AnyCaster || !AllowHostile || SpellId != 0 || SchoolId != 0 ||
			!string.IsNullOrWhiteSpace(Tag) || !string.IsNullOrWhiteSpace(IllusionKey) || !EffectKey.EqualTo("any")) return false;
		var grade = Spell is MagicSpell { InvocationGrade: { } selected } ? selected :
			Math.Clamp((int)power - (int)SpellPower.ExtremelyWeak + 1, 1, 7);
		var changed = false;
		foreach (var effect in item.EffectsOfType<IIntrinsicItemMagicEffect>()
			.Where(x => x.IsIntrinsicMagicActive).OrderBy(x => x.DispelPriority).ToArray())
		{
			if (!item.Effects.Contains(effect)) continue;
			if (grade < effect.MinimumDispelGrade)
			{
				if (effect.StopsDispelOnInsufficientGrade) break;
				continue;
			}
			changed |= effect.TryDispel(grade);
		}
		return changed;
	}
}
