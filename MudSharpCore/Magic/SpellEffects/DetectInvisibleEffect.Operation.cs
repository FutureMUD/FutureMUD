#nullable enable

using MudSharp.Effects;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public partial class DetectInvisibleEffect
{
	/// <summary>
	/// The configured operation owns attachment so its report describes retained state,
	/// not a child merely allocated for later attachment. Legacy factory calls are unchanged.
	/// </summary>
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not ICharacter character || parent is null || !ReferenceEquals(parent.Owner, target))
		{
			return new(MagicEffectOperationStatus.Rejected, null);
		}

		var existing = parent.SpellEffects.OfType<SpellDetectInvisibleEffect>().ToArray();
		if (existing.Length > 0)
		{
			return new(existing.All(x => character.Effects.Contains(x))
				? MagicEffectOperationStatus.NoChange : MagicEffectOperationStatus.Unknown, null);
		}

		var before = character.Effects.ToArray();
		var child = GetOrApplyEffect(caster, character, outcome, power, parent, additionalParameters);
		if (child is null)
		{
			return new(Unchanged(before, character.Effects) ? MagicEffectOperationStatus.NoChange : MagicEffectOperationStatus.Unknown, null);
		}

		parent.AddSpellEffect(child);
		// If this throws after retaining the child, the existing casting exception path
		// finalises its parent lifetime and records paid uncertainty without mastery.
		try { character.AddEffect(child); }
		catch
		{
			// Proven absence permits removing only this provisional ownership link.
			// A retained child keeps its parent so finalisation can bound its lifetime.
			if (!character.Effects.Contains(child)) parent.RemoveSpellEffect(child);
			throw;
		}
		if (character.Effects.Contains(child) && parent.SpellEffects.Contains(child))
		{
			return new(MagicEffectOperationStatus.Applied, null);
		}

		if (character.Effects.Contains(child))
		{
			return new(MagicEffectOperationStatus.Unknown, null);
		}

		// A receiver that retained nothing must not leave an orphaned parent child.
		parent.RemoveSpellEffect(child);
		return new(Unchanged(before, character.Effects) ? MagicEffectOperationStatus.NoChange : MagicEffectOperationStatus.Unknown, null);
	}

	private static bool Unchanged(IEffect[] before, IEnumerable<IEffect> after) =>
		before.SequenceEqual(after, ReferenceEqualityComparer.Instance);
}
