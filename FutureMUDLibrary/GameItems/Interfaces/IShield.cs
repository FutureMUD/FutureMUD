using MudSharp.Combat;

namespace MudSharp.GameItems.Interfaces {
    public interface IShield : IWieldable {
        IShieldType ShieldType { get; }
    }
}