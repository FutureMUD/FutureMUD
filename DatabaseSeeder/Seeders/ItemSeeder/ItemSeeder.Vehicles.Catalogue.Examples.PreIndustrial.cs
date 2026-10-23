#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string VehicleLandEngineMountType = "land_engine";
	private const string VehicleElectricDriveMountType = "electric_drive";
	private static readonly Lazy<IReadOnlyList<VehicleSeedSpec>> CompiledVehicleExampleSpecs = new(() => VehicleStock.All().ToArray());
	private static IReadOnlyList<VehicleSeedSpec> VehicleExampleSpecs => CompiledVehicleExampleSpecs.Value;
}
