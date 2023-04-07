using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.RPG.Merits {
    public interface IMerit : IFrameworkItem, IFutureProgVariable {
        MeritScope MeritScope { get; }
        MeritType MeritType { get; }
        bool Applies(IHaveMerits owner);
        string Describe(IHaveMerits owner, IPerceiver voyeur);
    }
}