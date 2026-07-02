using MudSharp.Character;
using MudSharp.Construction.Boundary;

namespace MudSharp.NPC.AI.Strategies
{
	public enum MovementStrategyResult
	{
		Failed,
		Waiting,
		Moved
	}

    public interface IMovementStrategy
    {
        MovementStrategyResult TryToMove(ICharacter character, ICellExit exit);
    }
}
