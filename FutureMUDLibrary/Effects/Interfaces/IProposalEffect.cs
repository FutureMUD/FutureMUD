using MudSharp.Framework;

namespace MudSharp.Effects.Interfaces {
    public interface IProposalEffect : IEffectSubtype, IKeyworded {
        IProposal Proposal { get; }
    }
}