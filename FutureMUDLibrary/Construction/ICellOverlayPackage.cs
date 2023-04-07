using MudSharp.Framework.Revision;
using MudSharp.FutureProg;

namespace MudSharp.Construction {
    public interface ICellOverlayPackage : IEditableRevisableItem, IFutureProgVariable
    {
	    void SetName(string newName);
    }
}