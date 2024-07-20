using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;

namespace MudSharp.RPG.Merits {
    public interface IMerit : IEditableItem, IFutureProgVariable {
        MeritScope MeritScope { get; }
        MeritType MeritType { get; }
        string DatabaseType { get; }
        IFutureProg ApplicabilityProg { get; }
		bool Applies(IHaveMerits owner);
        string Describe(IHaveMerits owner, IPerceiver voyeur);
        IMerit Clone(string newName);
    }
}