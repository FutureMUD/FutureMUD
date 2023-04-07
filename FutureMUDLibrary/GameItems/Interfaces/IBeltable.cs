namespace MudSharp.GameItems.Interfaces {
    /// <summary>
    ///     An item that has an IBeltable component is able to be attached to a belt, such as a pouch, holster or scabbard
    /// </summary>
    public interface IBeltable : IGameItemComponent {
        IBelt ConnectedTo { get; set; }
    }
}