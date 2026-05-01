#nullable enable

using MudSharp.Health;

namespace MudSharp.Effects.Interfaces;

public interface IMagicArmourEnhancementEffect : IEffectSubtype, IAbsorbDamage
{
	double ArmourDamageReduction { get; }
}
