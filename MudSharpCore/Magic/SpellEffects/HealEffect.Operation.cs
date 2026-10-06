#nullable enable

using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public partial class HealEffect
{
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not ICharacter character) return new(MagicEffectOperationStatus.Rejected, null);
		var wounds = character.Body.Wounds.Where(x => x.CanBeTreated(TreatmentType.Mend) != Difficulty.Impossible).ToArray();
		var before = wounds.Sum(x => x.CurrentDamage);
		GetOrApplyEffect(caster, target, outcome, power, parent, additionalParameters);
		var after = wounds.Sum(x => x.CurrentDamage);
		return new(!double.IsFinite(before) || !double.IsFinite(after) ? MagicEffectOperationStatus.Unknown :
			after < before ? MagicEffectOperationStatus.Applied : MagicEffectOperationStatus.NoChange, null);
	}
}
