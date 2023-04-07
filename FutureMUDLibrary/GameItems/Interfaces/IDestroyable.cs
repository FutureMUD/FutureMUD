using MudSharp.Health;

namespace MudSharp.GameItems.Interfaces {
    /// <summary>
    ///     An IDestroyable item can be destroyed if it suffers too many wounds
    /// </summary>
    public interface IDestroyable : IGameItemComponent {
        double MaximumDamage { get; }
        IDamage GetActualDamage(IDamage originalDamage);
    }
}