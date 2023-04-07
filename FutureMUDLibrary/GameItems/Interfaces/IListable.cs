using MudSharp.Framework;

namespace MudSharp.GameItems.Interfaces {
    public interface IListable : IGameItemComponent {
        string ShowList(IPerceiver voyeur, string arguments);
    }
}