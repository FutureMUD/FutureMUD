#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp_Unit_Tests;

[TestClass]
public class GameItemComponentTypeVisibilityTests
{
	[TestMethod]
	public void DefaultStaticSettings_ShowModernAndFuturisticComponentTypes()
	{
		Assert.AreEqual("true", DefaultStaticSettings.DefaultStaticConfigurations[
			GameItemComponentTypeVisibility.ShowModernSettingName]);
		Assert.AreEqual("true", DefaultStaticSettings.DefaultStaticConfigurations[
			GameItemComponentTypeVisibility.ShowFuturisticSettingName]);
	}

	[TestMethod]
	public void GameItemComponentManager_ComponentTypesCarryTechnologyMetadata()
	{
		var manager = new GameItemComponentManager();

		Assert.IsTrue(manager.TypeHelpInfo.Single(x => x.Name == "Refrigerator").IsModern);
		Assert.IsFalse(manager.TypeHelpInfo.Single(x => x.Name == "Refrigerator").IsFuturistic);
		Assert.IsTrue(manager.TypeHelpInfo.Single(x => x.Name == "ImplantRefrigerator").IsFuturistic);
		Assert.IsFalse(manager.TypeHelpInfo.Single(x => x.Name == "ImplantRefrigerator").IsModern);
		Assert.AreEqual(GameItemComponentTypeTechnology.None,
			manager.TypeHelpInfo.Single(x => x.Name == "Container").Technology);
	}

	[TestMethod]
	public void GetTypeHelpInfo_HidesOnlyDisabledTechnologySets()
	{
		var manager = new GameItemComponentManager();

		var historicalTypes = manager.GetTypeHelpInfo(false, false).Select(x => x.Name).ToList();
		CollectionAssert.DoesNotContain(historicalTypes, "Refrigerator");
		CollectionAssert.DoesNotContain(historicalTypes, "ImplantRefrigerator");
		CollectionAssert.Contains(historicalTypes, "Container");

		var futuristicTypes = manager.GetTypeHelpInfo(false, true).Select(x => x.Name).ToList();
		CollectionAssert.DoesNotContain(futuristicTypes, "Refrigerator");
		CollectionAssert.Contains(futuristicTypes, "ImplantRefrigerator");

		var allTypes = manager.GetTypeHelpInfo(true, true).Select(x => x.Name).ToList();
		CollectionAssert.Contains(allTypes, "Refrigerator");
		CollectionAssert.Contains(allTypes, "ImplantRefrigerator");
	}

	[TestMethod]
	public void VisibilityFiltering_DoesNotRemoveBuilderLoadersOrTypeHelp()
	{
		var manager = new GameItemComponentManager();

		CollectionAssert.Contains(manager.PrimaryTypes.ToList(), "refrigerator");
		Assert.IsNotNull(manager.TypeHelpInfo.SingleOrDefault(x => x.Name == "Refrigerator"));
		CollectionAssert.DoesNotContain(manager.GetTypeHelpInfo(false, true).Select(x => x.Name).ToList(),
			"Refrigerator");
	}
}
