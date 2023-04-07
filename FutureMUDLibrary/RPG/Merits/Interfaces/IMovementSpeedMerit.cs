using MudSharp.Movement;

namespace MudSharp.RPG.Merits.Interfaces {
    public interface IMovementSpeedMerit : ICharacterMerit {
        double SpeedMultiplier(IMoveSpeed speed);
    }
}