using MudSharp.Form.Material;

namespace MudSharp.Effects.Interfaces;

public interface IWeaponPoisonCoatingEffect : IEffectSubtype
{
	LiquidMixture ContaminatingLiquid { get; }
	void AddLiquid(LiquidMixture liquid);
	LiquidMixture RemovePoisonVolume(double amount);
}
