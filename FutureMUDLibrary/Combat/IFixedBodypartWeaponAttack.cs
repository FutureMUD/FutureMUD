using MudSharp.Form.Shape;

namespace MudSharp.Combat {
    public interface IFixedBodypartWeaponAttack : IWeaponAttack {
        IBodypartShape Bodypart { get; }
    }
}