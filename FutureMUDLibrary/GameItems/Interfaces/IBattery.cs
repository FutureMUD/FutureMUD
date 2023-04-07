namespace MudSharp.GameItems.Interfaces {
    public interface IBattery : IGameItemComponent {
        string BatteryType { get; }
        double WattHoursRemaining { get; set; }
        double TotalWattHours { get; }
        bool Rechargable { get; }
    }
}