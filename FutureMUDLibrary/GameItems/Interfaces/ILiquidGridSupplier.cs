#nullable enable
using MudSharp.Character;
using MudSharp.Form.Material;

namespace MudSharp.GameItems.Interfaces;

public interface ILiquidGridSupplier : IGameItemComponent
{
	LiquidMixture? SuppliedMixture { get; }
	double AvailableLiquidVolume { get; }
	LiquidMixture? RemoveLiquidAmount(double amount, ICharacter? who, string action);
}
