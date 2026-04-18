using MudSharp.Body;

namespace MudSharp.Combat;

public interface IRangedAttackMove : ICombatMove
{
    IBodypart TargetBodypart { get; }
}
