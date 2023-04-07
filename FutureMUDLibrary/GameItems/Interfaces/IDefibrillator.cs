using MudSharp.Body;
using MudSharp.Character;

namespace MudSharp.GameItems.Interfaces {
    public interface IDefibrillator : IConsumePower {
        bool CanShock(ICharacter shocker, IBody target);
        string WhyCannotShock(ICharacter shocker, IBody target);
        void Shock(ICharacter shocker, IBody target);
    }
}
