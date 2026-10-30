#nullable enable

using MudSharp.Character;

#nullable enable

namespace MudSharp.Effects.Interfaces;

/// <summary>Provides a fixed additive damage bonus for physical melee attacks.</summary>
public interface IFixedMeleeDamageBonusEffect : IEffectSubtype
{
	double MeleeDamageBonus { get; }
}

/// <summary>Provides a check bonus whose value depends on the check being resolved.</summary>
public interface IContextualCheckBonusEffect : ICheckBonusEffect
{
	double CheckBonusFor(MudSharp.RPG.Checks.CheckType type);
}

/// <summary>Prevents mercy or surrender while the effect applies.</summary>
public interface INoMercyEffect : IEffectSubtype
{
}

/// <summary>Prevents unconsciousness caused by stun, without preventing pain pass-out or death.</summary>
public interface IPreventStunUnconsciousnessEffect : IEffectSubtype
{
}

/// <summary>Allows an effect to end immediately when its character reaches zero stamina.</summary>
public interface IExpireAtZeroStaminaEffect : IEffectSubtype
{
}

/// <summary>Requests a native health-status check after the effect is removed.</summary>
public interface IHealthCheckAfterRemovalEffect : IEffectSubtype
{
	bool ShouldCheckHealthAfterRemoval(ICharacter character);
}
