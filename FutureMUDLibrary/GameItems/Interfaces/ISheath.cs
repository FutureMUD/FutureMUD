using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Interfaces {
    public interface ISheath : IGameItemComponent {
        SizeCategory MaximumSize { get; }
        IWieldable Content { get; set; }
        Difficulty StealthDrawDifficulty { get; }
        bool DesignedForGuns { get; }
        bool CanSheath(IGameItem item);
        string WhyCannotSheath(IGameItem item);
    }
}