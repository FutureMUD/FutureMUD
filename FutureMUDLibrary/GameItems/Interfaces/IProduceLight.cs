namespace MudSharp.GameItems.Interfaces {
    public interface IProduceLight : IGameItemComponent {
        double CurrentIllumination { get; }
    }
}