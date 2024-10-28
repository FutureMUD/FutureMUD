using MudSharp.Framework.Revision;
using MudSharp.FutureProg;

namespace MudSharp.Construction {
    public interface ICellOverlayPackage : IEditableRevisableItem, IProgVariable
    {
	    void SetName(string newName);
    }
}