using MudSharp.GameItems;

namespace MudSharp.Framework
{
    public interface IFuturemudControlContext : IFuturemudAccountController, IFuturemudPlayerController
    {
        IGameItemProto GameItemEdit { get; }
        string Prompt { get; }
        void Quit();
        void CloseSubContext();
    }
}