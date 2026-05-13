using MudSharp.Form.Material;

namespace MudSharp.Effects.Interfaces;

public interface IWeaponPoisonCoatingEffect : ILiquidContaminationEffect
{
	LiquidMixture RemovePoisonVolume(double amount);
}
