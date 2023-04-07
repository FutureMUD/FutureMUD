using MudSharp.Character;
using MudSharp.Construction.Boundary;

namespace MudSharp.NPC.AI.Strategies {
    public interface IMovementStrategy {
        bool TryToMove(ICharacter character, ICellExit exit);
    }
}
