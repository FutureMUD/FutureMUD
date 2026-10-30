using System.Linq;
using MudSharp.Character;

#nullable enable
namespace MudSharp.Health;

public static class NaturalRecoveryRules
{
	public static bool CanRecoverDamage(ICharacter actor) => actor.HealthStrategy is not ICriticalHealthRecoveryStrategy strategy ||
		!strategy.OverridesNaturalDamageHealing(actor);

	public static bool CanRecoverStun(ICharacter actor) => !actor.CombinedEffectsOfType<IPreventNaturalStunRecoveryEffect>()
		.Any(effect => effect.Applies() && effect.PreventNaturalStunRecovery);
}
