using MudSharp.Body;

namespace MudSharp.Effects.Interfaces {
    /// <summary>
    /// Puts a "Floor" on organ function, but is removed by further stress
    /// </summary>
    public interface IStablisedOrganFunction : IPertainToBodypartEffect {
        IOrganProto Organ { get; }
        double Floor { get; }
        ExertionLevel ExertionCap { get; }
    }
}
