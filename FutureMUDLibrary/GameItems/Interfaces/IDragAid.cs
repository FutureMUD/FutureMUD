namespace MudSharp.GameItems.Interfaces {
    public interface IDragAid : IGameItemComponent {
        double EffortMultiplier { get; }
        int MaximumUsers { get; }
    }
}
