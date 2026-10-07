using MudSharp.Framework.Revision;
using MudSharp.FutureProg;

namespace MudSharp.Construction
{
    public interface IRoomOverlayPackage : IEditableRevisableItem, IProgVariable
    {
        void SetName(string newName);
    }
}