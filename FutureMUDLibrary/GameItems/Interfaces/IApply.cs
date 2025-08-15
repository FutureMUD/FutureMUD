using MudSharp.Body;
using MudSharp.Character;

namespace MudSharp.GameItems.Interfaces {
    public enum WhyCannotApply {
        CanApply,
        CannotApplyEmpty,
        CannotApplyNoAccessToPart
    }

    public interface IApply : IGameItemComponent {
        WhyCannotApply CanApply(IBody target, IBodypart part);
        void Apply(IBody target, IBodypart part, ICharacter applier);
        void Apply(IBody target, IBodypart part, double amount, ICharacter applier);
    }
}
