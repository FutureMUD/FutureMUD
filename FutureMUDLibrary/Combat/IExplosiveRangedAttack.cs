using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.Health;

namespace MudSharp.Combat;

public interface IExplosiveRangedAttack : IRangedNaturalAttack
{
    SizeCategory ExplosionSize { get; }
    Proximity MaximumProximity { get; }
}
