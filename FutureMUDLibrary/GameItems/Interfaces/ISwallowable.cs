using MudSharp.Body;

#nullable enable

namespace MudSharp.GameItems.Interfaces
{
    public interface ISwallowable : IGameItemComponent
    {
        string? ConsumptionEmote => null;
        void Swallow(IBody body);
    }
}
