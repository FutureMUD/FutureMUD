using MudSharp.Character;

#nullable enable
namespace MudSharp.Health;

/// <summary>Optional recovery on the active body minute heartbeat, never offline healing.</summary>
public interface ICriticalHealthRecoveryStrategy
{
	bool OverridesNaturalDamageHealing(IHaveWounds owner);
	void PerformCriticalRecovery(ICharacter actor);
}
