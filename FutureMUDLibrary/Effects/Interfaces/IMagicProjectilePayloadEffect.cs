#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Effects.Interfaces;

public interface IMagicProjectilePayloadEffect : IEffectSubtype
{
	double ProjectileQualityBonus { get; }
	double ProjectileDamageBonus { get; }
	double ProjectilePainBonus { get; }
	double ProjectileStunBonus { get; }
	bool AppliesToProjectileAttack(ICharacter attacker, IPerceiver target, IGameItem projectile);
}
