using MudSharp.Form.Material;

#nullable enable

namespace MudSharp.Effects.Interfaces;

public interface IWeaponPoisonCoatingEffect : IEffectSubtype
{
	/// <summary>Optional delivery policy invoked with attacker, victim and weapon after an eligible damaging wound.</summary>
	MudSharp.FutureProg.IFutureProg? DeliveryProg => null;
	bool SingleUseOnWound => false;
	LiquidMixture ContaminatingLiquid { get; }
	void AddLiquid(LiquidMixture liquid);
	LiquidMixture RemovePoisonVolume(double amount);
}
