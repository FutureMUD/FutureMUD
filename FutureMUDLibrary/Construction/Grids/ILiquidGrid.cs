#nullable enable
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Construction.Grids;

public interface ILiquidGrid : IGrid
{
	double TotalLiquidVolume { get; }
	LiquidMixture CurrentLiquidMixture { get; }
	void JoinGrid(ILiquidGridSupplier supplier);
	void LeaveGrid(ILiquidGridSupplier supplier);
	LiquidMixture? RemoveLiquidAmount(double amount, ICharacter? who, string action);
}
