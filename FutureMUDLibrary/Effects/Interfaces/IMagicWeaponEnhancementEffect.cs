#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Effects.Interfaces;

public interface IMagicWeaponEnhancementEffect : IEffectSubtype
{
	double AttackCheckBonus { get; }
	double QualityBonus { get; }
	double DamageBonus { get; }
	double PainBonus { get; }
	double StunBonus { get; }
	bool AppliesToWeaponAttack(ICharacter attacker, IPerceivable target, IGameItem weapon);
}
