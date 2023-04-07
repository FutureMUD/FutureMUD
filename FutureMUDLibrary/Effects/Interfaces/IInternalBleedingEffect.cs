using MudSharp.Body;

namespace MudSharp.Effects.Interfaces {
    public interface IInternalBleedingEffect : IEffectSubtype {
        IOrganProto Organ { get; }
        double BloodlossPerTick { get; set; }
        double BloodlossTotal { get; set; }
    }
}