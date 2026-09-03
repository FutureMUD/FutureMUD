namespace MudSharp.GameItems.Interfaces
{
    public interface IProducePower : IGameItemComponent
    {
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

		/// <summary>
		/// Tests whether this producer can sustain a discrete load for a known duration. Producers with finite stored
		/// energy should override this overload; continuous producers may use the instantaneous-load implementation.
		/// </summary>
		bool CanDrawdownSpike(double wattage, System.TimeSpan duration)
		{
			return double.IsFinite(wattage) && wattage >= 0.0 && duration > System.TimeSpan.Zero &&
			       CanDrawdownSpike(wattage);
		}

		/// <summary>
		/// Draws a discrete load for a known duration. The wattage remains an instantaneous power requirement; the
		/// duration lets finite stores account for the corresponding energy without changing the legacy spike contract.
		/// </summary>
		bool DrawdownSpike(double wattage, System.TimeSpan duration)
		{
			return CanDrawdownSpike(wattage, duration) && DrawdownSpike(wattage);
		}
    }
}
