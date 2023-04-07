namespace MudSharp.GameItems.Interfaces {
    public delegate void PowerEvent();

    public interface IConsumePower : IGameItemComponent {
        double PowerConsumptionInWatts { get; }
        void OnPowerCutIn();
        void OnPowerCutOut();
    }
}