using MudSharp.Body;

namespace MudSharp.Effects.Interfaces {
    public interface IOrganDamageEffect : IPertainToBodypartEffect {
        IOrganProto Organ { get; }
        double Damage { get; set; }
    }
}