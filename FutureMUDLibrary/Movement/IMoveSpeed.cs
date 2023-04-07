using MudSharp.Body.Position;
using MudSharp.Framework;

namespace MudSharp.Movement
{
    public interface IMoveSpeed : IFrameworkItem
    {
        double Multiplier { get; }
        string FirstPersonVerb { get; }
        string ThirdPersonVerb { get; }
        string PresentParticiple { get; }
        double StaminaMultiplier { get; }
        IPositionState Position { get; }
    }
}