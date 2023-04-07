using MudSharp.Body;
using MudSharp.Character;

namespace MudSharp.GameItems.Interfaces {
    public enum WhyCannotInject {
        CanInject,
        CannotInjectEmpty,
        CannotInjectNoAccessToPart
    }

    public interface IInject : IGameItemComponent {
        WhyCannotInject CanInject(IBody target, IBodypart part);
        void Inject(IBody target, IBodypart part, ICharacter injector);
        void Inject(IBody target, IBodypart part, double amount, ICharacter injector);
    }
}