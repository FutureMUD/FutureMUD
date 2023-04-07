using MudSharp.Body;

namespace MudSharp.GameItems.Interfaces {
    public interface ISwallowable : IGameItemComponent {
        void Swallow(IBody body);
    }
}