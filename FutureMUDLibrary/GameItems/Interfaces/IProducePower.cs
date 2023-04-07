namespace MudSharp.GameItems.Interfaces {
    public interface IProducePower : IGameItemComponent {
        bool PrimaryLoadTimePowerProducer { get; }
        bool PrimaryExternalConnectionPowerProducer { get; }
        double FuelLevel { get; }
        bool ProducingPower { get; }
        double MaximumPowerInWatts { get; }
        void BeginDrawdown(IConsumePower item);
        void EndDrawdown(IConsumePower item);
        bool CanBeginDrawDown(double wattage);
        bool CanDrawdownSpike(double wattage);
        bool DrawdownSpike(double wattage);
    }
}