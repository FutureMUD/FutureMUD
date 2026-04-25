using MudSharp.GameItems.Interfaces;

#nullable enable

namespace MudSharp.Effects.Interfaces;

public interface IIngredientTransferEffect : IEffect
{
	void TransferToFood(IPreparedFood food, double proportion);
}
