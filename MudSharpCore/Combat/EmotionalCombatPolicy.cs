#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Interfaces;

namespace MudSharp.Combat;

/// <summary>One applicable physical character/body query. Existing guard wiring remains coordinator-owned.</summary>
public static partial class EmotionalCombatPolicy
{
	public static bool IsPeaceful(ICharacter actor, bool superPeaceful = false) =>
		actor.CombinedEffectsOfType<IPacifismEffect>()
			.Distinct<IPacifismEffect>(ReferenceEqualityComparer.Instance)
			.Any(effect => effect.Applies(actor) && (superPeaceful ? effect.IsSuperPeaceful : effect.IsPeaceful));

	public static bool IsRaging(ICharacter actor, bool superRaging = false) =>
		actor.CombinedEffectsOfType<IRageEffect>()
			.Distinct<IRageEffect>(ReferenceEqualityComparer.Instance)
			.Any(effect => effect.Applies(actor) && (superRaging ? effect.IsSuperRaging : effect.IsRaging));
}
