using MudSharp.Form.Material;

namespace MudSharp.Combat;

public interface ISpitAttack : IRangedNaturalAttack
{
	ILiquid Liquid { get; }
	double MaximumQuantity { get; }
}
