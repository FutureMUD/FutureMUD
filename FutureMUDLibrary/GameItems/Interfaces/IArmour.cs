using MudSharp.Combat;
using MudSharp.Health;

namespace MudSharp.GameItems.Interfaces {
    public interface IArmour : IGameItemComponent, IAbsorbDamage {
        IArmourType ArmourType { get; }
    }
}