#nullable enable

namespace MudSharp.Effects.Interfaces;

/// <summary>Flat absorption from physical damage and stun after armour.</summary>
public interface IPhysicalDamageAbsorptionEffect : IEffectSubtype
{
	double PhysicalAbsorption { get; }
}
