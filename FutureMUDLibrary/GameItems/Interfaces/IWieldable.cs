using MudSharp.Body;

namespace MudSharp.GameItems.Interfaces {
    public interface IWieldable : IGameItemComponent {
        IWield PrimaryWieldedLocation { get; set; }
        bool AlwaysRequiresTwoHandsToWield { get; }
    }
}