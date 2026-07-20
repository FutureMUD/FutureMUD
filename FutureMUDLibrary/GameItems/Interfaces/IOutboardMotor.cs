#nullable enable

using MudSharp.Vehicles;

namespace MudSharp.GameItems.Interfaces;

public interface IOutboardMotor : IGameItemComponent
{
	OutboardMotorEnergySource EnergySource { get; }
	double OutputMultiplier { get; }
	long? FuelLiquidId { get; }
	double FuelVolumePerMove { get; }
	double RequiredPowerSpikeInWatts { get; }
}
