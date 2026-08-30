#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using System;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ModernApplianceComponentRegistrationTests
{
	[TestMethod]
	public void GameItemComponentManagerRegistersModernApplianceTypes()
	{
		var manager = new GameItemComponentManager();
		var primaryTypes = manager.PrimaryTypes.ToList();
		var helpTypes = manager.TypeHelpInfo.Select(x => x.Name).ToList();

		CollectionAssert.Contains(primaryTypes, "refrigerator");
		CollectionAssert.Contains(primaryTypes, "dryer");
		CollectionAssert.Contains(primaryTypes, "implantrefrigerator");
		CollectionAssert.Contains(primaryTypes, "powerbank");
		CollectionAssert.Contains(helpTypes, "Refrigerator");
		CollectionAssert.Contains(helpTypes, "Dryer");
		CollectionAssert.Contains(helpTypes, "ImplantRefrigerator");
		CollectionAssert.Contains(helpTypes, "PowerBank");
	}

	[TestMethod]
	public void WashingMachineCycleMathUsesTotalSecondsForLongCycles()
	{
		var result = WashingMachineGameItemComponent.CalculateCycleLength(TimeSpan.FromMinutes(2.0), 1.5);

		Assert.AreEqual(TimeSpan.FromMinutes(3.0), result);
	}

	[TestMethod]
	public void WashingMachineDetergentMathHandlesEmptyLoads()
	{
		Assert.AreEqual(0.0, WashingMachineGameItemComponent.DetergentPerItem(0.1, 0,
			TimeSpan.FromMinutes(2.0), TimeSpan.FromSeconds(5.0)));
		Assert.AreEqual(0.00125, WashingMachineGameItemComponent.DetergentPerItem(0.1, 2,
			TimeSpan.FromSeconds(200.0), TimeSpan.FromSeconds(5.0)), 0.0000001);
	}

	[TestMethod]
	public void PreparedFoodMergeCompatibilityUsesEffectiveAgeRatherThanCreationTime()
	{
		Assert.IsTrue(PreparedFoodGameItemComponent.EffectiveAgesAreMergeCompatible(
			TimeSpan.FromMinutes(10.0), TimeSpan.FromMinutes(10.0) + TimeSpan.FromMilliseconds(500.0)));
		Assert.IsFalse(PreparedFoodGameItemComponent.EffectiveAgesAreMergeCompatible(
			TimeSpan.FromMinutes(10.0), TimeSpan.FromMinutes(12.0)));
	}
}
