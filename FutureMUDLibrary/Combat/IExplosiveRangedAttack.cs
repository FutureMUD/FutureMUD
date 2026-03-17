using MudSharp.Health;
using MudSharp.GameItems;
using MudSharp.Construction;

namespace MudSharp.Combat;

public interface IExplosiveRangedAttack : IRangedNaturalAttack
{
	SizeCategory ExplosionSize { get; }
	Proximity MaximumProximity { get; }
}
