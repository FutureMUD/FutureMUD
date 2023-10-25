using MudSharp.Body;

namespace MudSharp.GameItems.Interfaces {
    /// <summary>
    ///     An IHoldable is an IGameItemComponent implementation for any object that can be picked up
    /// </summary>
    public interface IHoldable : IGameItemComponent {
        string CurrentInventoryDescription { get; set; }
        IBody HeldBy { get; set; }
        bool IsHoldable { get; }
    }
}