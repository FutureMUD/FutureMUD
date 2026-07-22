#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.FutureProg;

namespace FutureMUDLibrary_Unit_Tests.FutureProg;

[TestClass]
public class VehicleOperationalProgTypeTests
{
	[DataTestMethod]
	[DataRow("vehicleroute")]
	[DataRow("vehicleservice")]
	[DataRow("vehiclejourney")]
	public void TryParse_OperationalVehicleType_RoundTripsStorage(string typeName)
	{
		Assert.IsTrue(ProgVariableTypeRegistry.TryParse(typeName, out var type));
		var storage = type.ToStorageString();
		Assert.IsTrue(ProgVariableTypeRegistry.TryParse(storage, out var roundTripped));
		Assert.AreEqual(type, roundTripped);
	}

	[TestMethod]
	public void CollectionItem_IncludesOperationalVehicleTypes()
	{
		Assert.IsTrue(ProgVariableTypes.CollectionItem.HasFlag(ProgVariableTypes.VehicleRoute));
		Assert.IsTrue(ProgVariableTypes.CollectionItem.HasFlag(ProgVariableTypes.VehicleService));
		Assert.IsTrue(ProgVariableTypes.CollectionItem.HasFlag(ProgVariableTypes.VehicleJourney));
	}
}
