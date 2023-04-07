using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.CharacterCreation
{
    public interface IChargenMenu : IControllable
    {
        string Display();
        void Close();
        void CloseSubContext();
        void SetContext(IControllable context);
        void MenuSetContext(IControllable context);
    }
}