#nullable enable

namespace MudSharp.Framework;

public sealed partial class Futuremud
{
	public void ForgetCombatSimulationActor(ICharacter actor)
	{
		_cachedActors.Remove(actor);
		MudSharp.Character.PossessionEffectIndex.For(this).UnregisterOwner(actor);
	}
}
