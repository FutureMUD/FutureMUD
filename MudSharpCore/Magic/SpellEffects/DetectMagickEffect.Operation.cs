#nullable enable

using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public partial class DetectMagickEffect
{
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not ICharacter) return new(MagicEffectOperationStatus.Rejected, null);
		var child = GetOrApplyEffect(caster, target, outcome, power, parent, additionalParameters);
		return new(child is null ? MagicEffectOperationStatus.NoChange : MagicEffectOperationStatus.Applied, child);
	}
}
