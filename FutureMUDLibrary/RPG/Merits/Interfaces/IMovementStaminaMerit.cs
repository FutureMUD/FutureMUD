using MudSharp.Movement;

namespace MudSharp.RPG.Merits.Interfaces {
    public interface IMovementStaminaMerit : ICharacterMerit {
        double StaminaMultiplier(IMoveSpeed speed);
    }
}